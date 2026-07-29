# Analisi Preliminare — Database Parallelo di Storico e Batch di Sincronizzazione

**Progetto:** IVS_DNA — Storicizzazione dati Pensioni
**Tipo documento:** Analisi architetturale preliminare (da validare con il cliente prima dello sviluppo)
**Versione:** v1.4
**Data:** 22/07/2026

## 1. Contesto e obiettivo

A seguito dell'analisi preliminare condotta sul codice IVS (moduli PN809/PN812/PN813/PN815/PN818) e sulla documentazione tecnica del DB (`Pensioni_Fs`), è stata definita una nuova architettura di storicizzazione basata su un **database parallelo** anziché sulla sola rimozione/archiviazione dei dati dal DB operativo. Obiettivo: permettere la consultazione dei dati storicizzati dall'applicativo IVS esistente, con impatto minimo sul codice, mantenendo il DB operativo `Pensioni_Fs` alleggerito dai dati non più necessari all'operatività corrente.

## 2. Architettura del DB parallelo `Pensioni_Fs_Storico`

- Stessa istanza SQL Server (o istanza dedicata), DB distinto `Pensioni_Fs_Storico`, con schema **identico** a `Pensioni_Fs`: stesse tabelle, stesse stored procedure, stesse funzioni. Si sviluppa sulla stessa edizione SQL Server già in uso per `Pensioni_Fs`, senza vincoli aggiuntivi.
- **Governance obbligatoria**: ogni modifica DDL o di SP rilasciata su `Pensioni_Fs` deve essere applicata, nella stessa release, anche su `Pensioni_Fs_Storico`. Questo perché il meccanismo di consultazione (vedi §3) presuppone SP identiche invocabili dallo stesso DAL cambiando solo la connection string.
- Copia iniziale (bulk, "big bang") di tutti i dati legacy eleggibili secondo i criteri di storicizzazione (§6), poi mantenimento tramite il batch notturno descritto al §5.

## 3. Meccanismo di switch: checkbox in "Visualizza Stato Pratiche"

- Checkbox `false` (default) → connection string `PensioniConnectionString` (Pensioni_Fs).
- Checkbox `true` → connection string `PensioniConnectionStringStorico` (Pensioni_Fs_Storico).
- Va propagato un parametro "sorgente dati" dalla UI (PN809, `UCVisualizzaStatoPratiche`/`UCStatoPratiche`) fino al layer dati (PN812), dove oggi la connection string è risolta in modo statico (`ConnectionFactory.GetConnection("PensioniConnectionString")`). **Questa è l'unica modifica di codice richiesta**: nessuna modifica alla logica di business, ai controlli sui semafori, alla UI dei quadri.
- **Accesso riservato**: la funzionalità di consultazione storico è abilitata **solo per il profilo Direttore di Sede** — il checkbox va reso visibile/attivabile solo per quel ruolo.
- **Sola lettura garantita**: quando la sorgente dati è lo storico, nessuna azione di salvataggio/modifica deve essere raggiungibile in UI; a livello DB, l'utente applicativo verso `Pensioni_Fs_Storico` va configurato con permessi **SELECT-only**, come ulteriore barriera indipendente dalla UI. Si presume l'utilizzo delle stesse credenziali/permessi già in uso dall'applicativo IVS verso `Pensioni_Fs`, adattati per sola lettura sullo storico.
- **Concorrenza**: nessun rischio di sovrapposizione. Il routing è esclusivo per singola richiesta (o Fs o Storico), e lo storico è alimentato solo da pratiche già chiuse (`StatoPensione = 4`, non più modificabili in IVS). Non esiste uno stato in cui la stessa pratica sia scrivibile su entrambi i DB contemporaneamente.

## 4. Preservazione degli ID (nessuna rigenerazione)

`SET IDENTITY_INSERT <tabella> ON` è l'unico modo T-SQL per specificare esplicitamente il valore della colonna identity in un insert — **non genera nuovi ID**, ne consente la copia esatta di quelli esistenti. Con `OFF` (default), SQL Server ignorerebbe il valore fornito e genererebbe un nuovo ID, rendendo impossibile la copia 1:1 richiesta per non impattare il codice applicativo di ricerca.

- Ogni SP di insert verso lo storico incapsula internamente `SET IDENTITY_INSERT ON` → insert → `SET IDENTITY_INSERT OFF`, per la durata minima necessaria.
- **Nessun rischio di collisione ID**: la colonna IDENTITY di `Pensioni_Fs` non riassegna mai un valore già utilizzato, anche dopo cancellazione della riga (il contatore è monotono crescente, salvo un RESEED esplicito non previsto). Pertanto un ID copiato in `Pensioni_Fs_Storico` non potrà mai coincidere con un ID futuro generato su `Pensioni_Fs`, indipendentemente dall'esito o dai tempi della successiva cancellazione (gestita comunque solo a copia riuscita, cfr. §5.5, per evitare perdita dati in caso di errore).

## 5. Batch di sincronizzazione — approccio a catalogo

Per evitare i problemi di manutenibilità di un ETL tradizionale (esplicitamente escluso, vista la continua evoluzione delle tabelle operative), si adotta un **motore generico pilotato da una tabella di catalogo**, anziché codice specifico per ciascuna delle ~132 tabelle coinvolte (elenco derivato dal metodo `EliminaPensione`, `PN812/WSInpsPensioniLiquidazione.DataCommon/DAGestionePensione.cs`).

### 5.1 Tabella di catalogo `CTL_TabelleStoricizzabili`

| Colonna | Descrizione |
|---|---|
| `Ordine` | ordine di esecuzione (copia: padri→figli; cancellazione: stesso ordine di `EliminaPensione`) |
| `NomeTabella` | es. `Pensione`, `Anagrafica`, `RecordFondo`, ... |
| `SpSelect` | SP di estrazione dati pratica da `Pensioni_Fs` |
| `SpInsertStorico` | SP di insert su `Pensioni_Fs_Storico` (gestisce internamente IDENTITY_INSERT) |
| `SpDeleteOperativo` | SP esistente di cancellazione (riusata da `EliminaPensione`), `NULL` se la tabella non va mai cancellata dall'operativo |
| `Categoria` | `Operativa` / `Decodifica` |
| `Attiva` | consente di disattivare temporaneamente una tabella senza modificare codice |
| `NomeColonnaChiave` | nome della colonna IDENTITY propria della tabella da preservare in copia (può essere assente, se la tabella non ha una propria colonna IDENTITY, es. PK composta) |

Aggiungere una nuova tabella al perimetro = una riga di configurazione + due SP dedicate, generabili da template — **non si tocca il motore**.

### 5.2 Esempio pratico: `Pensione` (padre), `Titolare` (figlio) e `Anagrafica` (indipendente)

```sql
INSERT INTO CTL_TabelleStoricizzabili (Ordine, NomeTabella, SpSelect, SpInsertStorico, SpDeleteOperativo, Categoria, Attiva, NomeColonnaChiave) VALUES
 (10, 'Anagrafica', 'GetAnagraficaByIdPensione_ForStorico', 'InsertAnagraficaStorico', NULL, 'Operativa', 1, 'Id'),
 (50, 'Titolare',   'GetTitolareByIdPensione_ForStorico',   'InsertTitolareStorico',   'DeleteTitolare', 'Operativa', 1, NULL),
 (99, 'Pensione',   'GetPensioneByIdPensione_ForStorico',   'InsertPensioneStorico',   'DeletePensione', 'Operativa', 1, 'Id');
```

`Anagrafica` ha `SpDeleteOperativo = NULL`: verifica sul codice conferma che `EliminaPensione` **non cancella mai l'anagrafica** (condivisa tra più domande dello stesso soggetto) — va copiata ma mai rimossa dall'operativo. Il motore non necessita di logica speciale per questo caso: è solo un dato mancante nella riga di configurazione.

**Pensione** (10 campi):
```sql
CREATE PROCEDURE GetPensioneByIdPensione_ForStorico
    @IdPensione BIGINT
AS
SELECT Id, NDomus, SiglaCategoria, CodiceSede, NCertificato,
       DataPresentazioneDomanda, StatoPensione, DataTentativoCalcoloDefinitivo,
       CentroOperativo, DataElaborazione
FROM dbo.Pensione WHERE Id = @IdPensione;

CREATE PROCEDURE InsertPensioneStorico
    @Id BIGINT, @NDomus BIGINT, @SiglaCategoria CHAR(8), @CodiceSede SMALLINT,
    @NCertificato INT, @DataPresentazioneDomanda DATETIME, @StatoPensione TINYINT,
    @DataTentativoCalcoloDefinitivo DATETIME, @CentroOperativo TINYINT, @DataElaborazione DATETIME
AS
BEGIN
    SET IDENTITY_INSERT dbo.Pensione ON;
    INSERT INTO dbo.Pensione (Id, NDomus, SiglaCategoria, CodiceSede, NCertificato,
        DataPresentazioneDomanda, StatoPensione, DataTentativoCalcoloDefinitivo, CentroOperativo, DataElaborazione)
    VALUES (@Id, @NDomus, @SiglaCategoria, @CodiceSede, @NCertificato,
        @DataPresentazioneDomanda, @StatoPensione, @DataTentativoCalcoloDefinitivo, @CentroOperativo, @DataElaborazione);
    SET IDENTITY_INSERT dbo.Pensione OFF;
END
```

**Titolare** (tabella figlia — junction tra `Pensione` e `Anagrafica`, 3 campi, PK composta `IdAnagrafica + IdPensione`, **nessuna colonna IDENTITY propria**): esempio di un pattern diverso dagli altri due, in cui la copia non richiede affatto `IDENTITY_INSERT`, perché la chiave è interamente composta da FK già preservate dalle rispettive tabelle padre (`Anagrafica.Id` e `Pensione.Id`, entrambe copiate in precedenza con i propri ID originali):

```sql
CREATE PROCEDURE GetTitolareByIdPensione_ForStorico
    @IdPensione BIGINT
AS
SELECT IdAnagrafica, IdPensione, DataMorte
FROM dbo.Titolare WHERE IdPensione = @IdPensione;

CREATE PROCEDURE InsertTitolareStorico
    @IdAnagrafica BIGINT, @IdPensione BIGINT, @DataMorte DATETIME
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.Titolare WHERE IdAnagrafica = @IdAnagrafica AND IdPensione = @IdPensione)
    BEGIN
        INSERT INTO dbo.Titolare (IdAnagrafica, IdPensione, DataMorte)
        VALUES (@IdAnagrafica, @IdPensione, @DataMorte);
    END
END
```

Nel catalogo, `Titolare` ha `Ordine = 50` (tra `Anagrafica` e `Pensione`): deve essere copiato **dopo** entrambe le sue tabelle padre, perché referenzia sia `Anagrafica.Id` sia `Pensione.Id` tramite la PK composta — se una delle due non fosse ancora presente in `Pensioni_Fs_Storico`, l'insert violerebbe l'integrità referenziale.

**Anagrafica** (10 campi, gestione idempotenza — persona con più domande già storicizzate):
```sql
CREATE PROCEDURE GetAnagraficaByIdPensione_ForStorico
    @IdPensione BIGINT
AS
SELECT a.Id, a.CodiceFiscale, a.Cognome, a.Nome, a.CognomeAcquisito,
       a.Sesso, a.DataNascita, a.ComuneNascita, a.ProvinciaNascita, a.Cittadinanza
FROM dbo.Anagrafica a
JOIN dbo.Titolare t ON t.IdAnagrafica = a.Id
WHERE t.IdPensione = @IdPensione;

CREATE PROCEDURE InsertAnagraficaStorico
    @Id BIGINT, @CodiceFiscale CHAR(16), @Cognome VARCHAR(32), @Nome VARCHAR(32),
    @CognomeAcquisito VARCHAR(31), @Sesso CHAR(1), @DataNascita DATETIME,
    @ComuneNascita VARCHAR(36), @ProvinciaNascita CHAR(3), @Cittadinanza CHAR(4)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.Anagrafica WHERE Id = @Id)
    BEGIN
        SET IDENTITY_INSERT dbo.Anagrafica ON;
        INSERT INTO dbo.Anagrafica (Id, CodiceFiscale, Cognome, Nome, CognomeAcquisito,
            Sesso, DataNascita, ComuneNascita, ProvinciaNascita, Cittadinanza)
        VALUES (@Id, @CodiceFiscale, @Cognome, @Nome, @CognomeAcquisito,
            @Sesso, @DataNascita, @ComuneNascita, @ProvinciaNascita, @Cittadinanza);
        SET IDENTITY_INSERT dbo.Anagrafica OFF;
    END
END
```

Motore generico (C#, semplificato):
```csharp
foreach (var riga in catalogo.OrderByDescending(r => r.Ordine).Where(r => r.Attiva && r.Categoria == "Operativa"))
{
    var dati = EseguiSpSelect(riga.SpSelect, idPensione);
    if (dati.Rows.Count > 0)
        EseguiSpInsert(riga.SpInsertStorico, dati, storicoConnection);
}
```
`EseguiSpSelect`/`EseguiSpInsert` non conoscono nomi di tabelle specifici: leggono il catalogo ed eseguono per nome. Aggiungere una tabella non modifica questo metodo.

### 5.3 Casi particolari verificati nel codice

L'analisi del codice (`DAGestionePensione.cs`, `Pensioni.dbml`/`.designer.cs`) ha evidenziato tre pattern ricorrenti tra le tabelle operative, tutti gestibili con il modello a catalogo senza modifiche al motore generico, ma da esplicitare in configurazione tabella per tabella.

**a) Tabelle con proprio ID surrogato, diverso da `IdPensione`** (es. `Eliminazione`)

`Eliminazione` ha una colonna `Id BIGINT IDENTITY` propria come PK, mentre `IdPensione` è solo una colonna FK/filtro (una pratica può avere più righe di `Eliminazione`). La copia deve preservare `Id` (colonna indicata da `NomeColonnaChiave` nel catalogo) tramite IDENTITY_INSERT, mentre `IdPensione` viene copiato come valore normale:

```sql
CREATE PROCEDURE InsertEliminazioneStorico
    @Id BIGINT, @IdPensione BIGINT, @CodiceMotivo TINYINT, @DecorrenzaEliminazione DATETIME, ...
AS
BEGIN
    SET IDENTITY_INSERT dbo.Eliminazione ON;
    INSERT INTO dbo.Eliminazione (Id, IdPensione, CodiceMotivo, DecorrenzaEliminazione, ...)
    VALUES (@Id, @IdPensione, @CodiceMotivo, @DecorrenzaEliminazione, ...);
    SET IDENTITY_INSERT dbo.Eliminazione OFF;
END
```

Il motore generico gestisce già più righe per pratica (itera su un `DataTable`), quindi non richiede modifiche: serve solo dichiarare `NomeColonnaChiave = 'Id'` nel catalogo per questa tabella.

**b) Tabelle collegate a `Pensione` tramite FK indiretta a più hop, senza `IdPensione` diretta** (es. `PensioneFondoDZ` → `RecordFondo` → `Pensione`)

`PensioneFondoDZ` non contiene affatto `IdPensione`: ha `Id` (IDENTITY, PK) e `IdRecordFondo` (FK verso `RecordFondo.Id`). Il collegamento alla pratica passa per la tabella intermedia `RecordFondo` (che ha `Id` proprio e `IdPensione` diretta). La `SpSelect` deve incapsulare il join a due hop:

```sql
CREATE PROCEDURE GetPensioneFondoDZByIdPensione_ForStorico
    @IdPensione BIGINT
AS
SELECT dz.Id, dz.IdRecordFondo, dz.RiscattiAA, dz.CodiceDZ, ...
FROM dbo.PensioneFondoDZ dz
JOIN dbo.RecordFondo rf ON rf.Id = dz.IdRecordFondo
WHERE rf.IdPensione = @IdPensione;
```

In questo caso l'**ordine di copia nel catalogo è vincolante**: `RecordFondo` (padre) deve essere copiato prima di `PensioneFondoDZ` (figlio), perché l'insert su `PensioneFondoDZ` scrive `IdRecordFondo` come valore letterale, valido solo se la riga corrispondente su `RecordFondo` esiste già in `Pensioni_Fs_Storico` con lo stesso `Id` (preservato via IDENTITY_INSERT). Poiché entrambi gli ID sono copiati identici, il legame FK resta coerente tra i due DB senza rimappature — ma solo se l'ordine è verificato esplicitamente per ogni coppia padre/figlio con FK indiretta, non assunto automaticamente dall'ordine inverso di `EliminaPensione` (che garantisce la sequenza corretta di cancellazione, non necessariamente quella di inserimento per FK a più hop).

**c) Tabella con Insert ma senza alcun percorso di cancellazione (`PrenotazioneElaborazioni`)**

Verifica sistematica su tutte le 320 tabelle dello schema (confronto tra le 101 tabelle con FK diretta `IdPensione` e le 233 SP `Delete*` definite nel codice, non solo quelle invocate da `EliminaPensione`): **100 tabelle su 101 sono coperte** (alcune tramite nomi plurali/varianti, es. `DeleteAllStatiCivili` → `StatoCivile`, `DeleteAllComponentiFamiliari` → `ComponenteFamiliare`). Una sola tabella risulta priva di qualunque SP di cancellazione:

- `PrenotazioneElaborazioni`: ha FK diretta `IdPensione`, ha una SP di Insert realmente usata (`InsertPrenotazioneElaborazioni`, invocata da `DAGestioneAnniRichiestaBonus.SalvaPrenotazioneElaborazioni` per la gestione "Richiesta Bonus"), ma **nessuna SP di Delete mappata**: nel designer esiste solo uno stub tecnico ORM (`partial void DeletePrenotazioneElaborazioni`), mai collegato a una SP reale né invocato da alcun flusso applicativo.
- Questa tabella **non sarebbe intercettata** da un catalogo costruito solo a partire da `EliminaPensione`, in quanto semplicemente non vi compare. Va aggiunta manualmente al catalogo: per la copia basta la `SpSelect`/`SpInsertStorico` standard; per la cancellazione dall'operativo serve **creare una nuova SP `DeletePrenotazioneElaborazioni(idPensione)`** (non esiste oggi), oppure — da concordare col cliente — valutare se l'assenza di cancellazione sia intenzionale (dati aggregati per anno/bonus con vita utile indipendente dallo stato della pratica), nel qual caso va trattata come `Anagrafica` (copiata, mai cancellata dall'operativo, `SpDeleteOperativo = NULL`), documentandone esplicitamente il motivo.

Verifica analoga (SP di cancellazione mai invocate da nessuna parte del codice) non ha rilevato alcuna SP orfana tra le 233 esistenti: le 101 SP non usate da `EliminaPensione` sono tutte invocate altrove, in flussi di gestione puntuale della pratica ancora aperta (varianti `*ByIdRecordFondo`, `*NoStorico`, ecc.), coerenti col fatto che il perimetro di `EliminaPensione` rappresenta specificamente la cancellazione di fine vita della pratica.

### 5.4 Rischio di disallineamento schema e controllo di validazione (obbligatorio)

Con SP che enumerano colonne esplicitamente (necessario per gestire IDENTITY_INSERT e idempotenza), un disallineamento tra `Pensioni_Fs` e `Pensioni_Fs_Storico` **non sempre produce un errore immediato**:

| Tipo di modifica su Fs non replicata su Storico | Effetto |
|---|---|
| Nuova colonna aggiunta | **Nessun errore** — il dato viene semplicemente perso in silenzio nello storico |
| Tipo colonna ristretto (es. VarChar più corto) | Nessun errore finché i valori restano compatibili; troncamento/errore solo quando arriva un valore "grande" — **difetto latente** |
| Colonna rinominata/rimossa | Errore immediato e bloccante sulla SP di select/insert |
| Colonna resa NOT NULL senza default | Errore differito, solo al primo insert di un valore NULL |

Per evitare che i casi silenziosi/latenti passino inosservati, si introduce un **controllo di validazione schema obbligatorio, eseguito prima di ogni esecuzione del batch**:

```sql
-- Confronto struttura colonne tra Fs e Storico, limitato alle tabelle in catalogo
SELECT c.TABLE_NAME, c.COLUMN_NAME, c.DATA_TYPE, c.CHARACTER_MAXIMUM_LENGTH, c.IS_NULLABLE
FROM Pensioni_Fs.INFORMATION_SCHEMA.COLUMNS c
WHERE c.TABLE_NAME IN (SELECT NomeTabella FROM CTL_TabelleStoricizzabili WHERE Attiva = 1)
EXCEPT
SELECT c.TABLE_NAME, c.COLUMN_NAME, c.DATA_TYPE, c.CHARACTER_MAXIMUM_LENGTH, c.IS_NULLABLE
FROM Pensioni_Fs_Storico.INFORMATION_SCHEMA.COLUMNS c
WHERE c.TABLE_NAME IN (SELECT NomeTabella FROM CTL_TabelleStoricizzabili WHERE Attiva = 1)
```

- Se il confronto restituisce righe → il batch **si arresta prima di copiare qualunque dato**, registra la discrepanza sulla tabella di log (§5.6) e genera un alert.
- Trasforma i disallineamenti silenziosi/latenti (righe 1 e 2 della tabella sopra) in un **errore esplicito e bloccante**, coerente con il principio "meglio un batch che non parte piuttosto che uno storico con dati persi o troncati".

### 5.5 Transazionalità a due fasi (copia → cancellazione)

Esecuzione **notturna**, per ciascuna pratica eleggibile:

1. **Fase A — Copia** (transazione su `Pensioni_Fs_Storico`): per ogni tabella del catalogo (ordine padri→figli), select da Fs, insert su Storico. Se un insert fallisce → **rollback completo** della fase A, nessuna cancellazione viene tentata, log dell'esito KO.
2. **Fase B — Cancellazione** (transazione su `Pensioni_Fs`), **eseguita solo se la Fase A è andata a buon fine**: invocazione delle stesse SP `Delete*` già usate da `EliminaPensione`, nello stesso ordine. Se una delete fallisce → rollback della fase B (i dati restano quindi presenti in entrambi i DB, non persi: situazione da segnalare e rieseguire, non un'inconsistenza pericolosa).

Non esiste quindi mai uno stato in cui la pratica sia assente da entrambi i DB.

### 5.6 Tabella di log

`LOG_Storicizzazione`: `IdPensione`, `NDomus`, `Timestamp`, `Fase` (ValidazioneSchema/Copia/Cancellazione), `Esito` (OK/KO/Rollback), `MessaggioErrore`. Permette audit verso il cliente e rielaborazione mirata delle sole pratiche in KO, senza rieseguire l'intero batch.

### 5.7 Microservizio "decodifiche/lookup"

Stesso catalogo, righe con `Categoria = 'Decodifica'`. Logica più semplice, non per-pratica: confronto/versionamento (hash riga o timestamp di modifica) tra Fs e Storico. **Va eseguito prima del microservizio "Operative"** in ogni run notturna (motivazione al §5.8): le tabelle operative referenziano tabelle di decodifica (es. `Eliminazione` → `DecodificaCodiceEliminazione`), quindi lo storico deve avere le decodifiche allineate prima di ricevere righe operative che le referenziano, per evitare violazioni di integrità referenziale o disallineamenti logici silenziosi.

### 5.8 Architettura applicativa dei componenti

Il batch non è un blocco monolitico né un singolo microservizio: sono **tre componenti con responsabilità distinte**, invocati in sequenza fissa.

**1. Batch Orchestratore** (schedulato, notturno, nessuna API esposta)
- Esegue il controllo di validazione schema (§5.4); se fallisce, si ferma senza procedere oltre.
- Estrae l'elenco delle pratiche eleggibili (criteri §6).
- Invoca in sequenza i due microservizi, in **modalità sincrona** (attende la risposta di ciascuno prima di procedere).
- Registra l'esito complessivo sulla tabella di log (§5.6).

**2. Microservizio "Decodifiche/Lookup"** — invocato per **primo**, una volta per intera run (non per singola pratica): allinea le tabelle di decodifica tra Fs e Storico.

**3. Microservizio "Operative"** — invocato **dopo**, per ciascuna pratica eleggibile: esegue la transazionalità a due fasi copia→cancellazione (§5.5).

```
[Scheduler notturno]
        │
        ▼
[Batch Orchestratore] ──valida schema (§5.4)──> STOP se KO
        │
        ├─ 1) [Microservizio Decodifiche/Lookup] (una volta per run) ──> Storico
        │      (deve completarsi con successo prima del passo 2)
        │
        └─ 2) [Microservizio Operative] (per ogni pratica eleggibile, sincrono) ──> Storico + Fs (2 fasi transazionali)
        │
        ▼
[Tabella di Log condivisa] ← scritta da tutti i componenti
[Catalogo CTL_TabelleStoricizzabili] ← letto da entrambi i microservizi
```

**Perché sincrono e non asincrono/parallelo:**
- Requisito esplicito del cliente (batch sincrono).
- **Dipendenza di dati tra i due servizi**: le tabelle operative hanno FK verso tabelle di decodifica. Se i due microservizi girassero in parallelo, il microservizio Operative potrebbe tentare di inserire su `Pensioni_Fs_Storico` una riga con FK verso una decodifica non ancora allineata — violazione di vincolo o disallineamento logico non deterministico, a seconda di quale processo termina per primo.
- **Tracciabilità**: esecuzione sequenziale produce un ordine causale chiaro nei log; in asincrono servirebbe un correlation ID e una logica di sincronizzazione per ricostruire ciò che il sincrono garantisce per costruzione.
- **Contesa di risorse**: due processi paralleli pesanti sullo stesso DB in una finestra notturna limitata non offrono un reale vantaggio di throughput (il collo di bottiglia resta il DB, non l'orchestratore), a fronte di una complessità maggiore.
- Un eventuale disaccoppiamento futuro (job lookup con schedulazione propria, indipendente dal batch operativo) è un'ottimizzazione da valutare solo dopo aver misurato l'SLA reale (cfr. §7), non da introdurre preventivamente.

**Perché Lookup prima di Operative** (e non viceversa): le tabelle operative referenziano le tabelle di decodifica tramite FK. Copiare prima le operative rischierebbe di inserire su `Pensioni_Fs_Storico` righe che puntano a decodifiche non ancora presenti o non ancora allineate alla versione corrente — con conseguente fallimento per violazione di integrità referenziale, oppure, nel caso peggiore, un inserimento riuscito ma logicamente incoerente (decodifica presente ma con significato non aggiornato). Allineare prima le decodifiche elimina questo rischio alla radice.

## 6. Criteri di eleggibilità storicizzazione (confermati, invariati)

**Ambito Pensioni**: `StatoPensione = 4` (definitiva) con `DataTentativoCalcoloDefinitivo` > 3 anni. Eccezioni: non storicizzare se TRF non calcolata (riapertura provvisoria→definitiva incompleta); non storicizzare domande spacchettate; non storicizzare `LogGenerico`.

**`LogSoap`**: fuori perimetro — è già storicizzato su un DB separato esistente. Nessuno sviluppo previsto su di esso in questo progetto.

## 7. Punti aperti residui

1. **Volumi/SLA della finestra batch notturna**: durata massima e volumi attesi per notte da definire e concordare con il cliente.

2. **Creazione del DB `Pensioni_Fs_Storico`**: verificato che la login applicativa `Pensioni_Fs` non dispone di permessi server-level (`dbcreator`/`sysadmin`/`CREATE ANY DATABASE`), ma solo di `db_owner` all'interno del DB `Pensioni_Fs` esistente. La creazione del nuovo DB richiede quindi un intervento del DBA (creazione del DB vuoto sull'istanza, o concessione temporanea del permesso di creazione). Una volta creato il DB vuoto, lo sviluppo di schema/tabelle/SP può procedere con i permessi già disponibili.

3. **Procedura di ribaltamento dati iniziale senza cancellazione (fase di collaudo del parallelismo)**: prima di attivare la cancellazione dalle tabelle operative, prevedere una prima esecuzione del batch in modalità "solo copia" (fasi 1 e 2 del §5.5, senza fase 3 di cancellazione), per verificare in produzione/collaudo che il meccanismo di switch e la lettura parallela tra `Pensioni_Fs` e `Pensioni_Fs_Storico` funzionino correttamente, senza impatto sui dati operativi esistenti.

4. **Procedura di cancellazione delle domande trasferite**: solo dopo il collaudo positivo del punto 3, va previsto e attivato lo step di cancellazione dalle tabelle operative (fase 3 del §5.5) per le domande già copiate con successo nello storico, secondo la logica transazionale già descritta.

5. **Repository dei sorgenti**: da stabilire dove verrà versionato il codice del batch e dei microservizi (nuovo repository dedicato o area del repository esistente `IVS_DNA`).

6. **Struttura dei sorgenti**: da definire in conformità alle linee guida/standard dell'Istituto per l'organizzazione dei progetti (struttura cartelle, convenzioni di solution/progetto, pipeline di build/deploy), una volta individuato il repository di destinazione (punto 5).

---

*Documento di analisi preliminare — soggetto a validazione con il cliente prima dell'avvio dello sviluppo.*
