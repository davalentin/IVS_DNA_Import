# Logiche di storicizzazione e ricerca dati - analisi approfondita

## 1. Obiettivo

Questo documento consolida i contenuti del file `Logiche di storicizzazione e ricerca dati.docx` e li estende con un'analisi più profonda, basata sul codice IVS_DNA, sulla documentazione in `Doc/ENGenius` e sulla documentazione DB presente in DocMind per `IVS_DB` / `Pensioni_FS_WIP`.

## 2. Sintesi del documento originale

### Regole di storicizzazione

- **Dati pensione (ambito storico principale)**: storicizzare tutte le domande calcolate in definitiva (`StatoPensione = 4`) con `DataTentativoCalcoloDefinitivo < 3 anni`.
- **Eccezione 1**: non storicizzare le domande calcolate in definitiva con data tentativo definitiva inferiore a 3 anni se **non** hanno la TRF calcolata (caso di riapertura domanda / trasformazione da provvisoria a definitiva non completata).
- **Eccezione 2**: non storicizzare le domande **spacchettate**.
- **Eccezione 3**: non storicizzare `LogGenerico`.
- **LogSoap**: storicizzare tutte le domande calcolate in definitiva con `DataTentativoCalcoloDefinitivo < 1 anno`, con le stesse eccezioni su TRF e domande spacchettate.

### Modello di archiviazione

- Creare un **DB dedicato alla storicizzazione** con copia delle tabelle dei dati pensione.
- Mantenere allineato lo schema del DB storico con quello del DB corrente.
- Realizzare un **software di trasferimento** che:
  1. copia i dati dal DB operativo al DB storico;
  2. verifica l'esito;
  3. cancella i dati dal DB operativo usando le stesse logiche di storicizzazione.

### Ricerca dati

- La ricerca ordinaria deve continuare a funzionare sulle tabelle correnti del DB `Pensioni_FS`.
- Per i dati storici va prevista una sezione nel menù **Utility di Sistema** per report di totalizzazione e ricerca per:
  - Tipologia Domanda (`PL/TRF` e/o `RIC`);
  - Stato Lavorazione Domanda;
  - altri filtri da definire.
- La sezione potrebbe essere riservata al **Direttore di Sede**.

## 3. Analisi più profonda

### 3.1 Criterio di eleggibilità

La soglia temporale da sola non basta: la storicizzazione deve rispettare il **contesto funzionale** della pratica.
In pratica, il criterio consigliato è:

1. la pratica è in stato definitivo;
2. la data di consolidamento è oltre soglia;
3. non esistono eccezioni funzionali (TRF mancante, pratica spacchettata, casi amministrativi da tenere ancora caldi);
4. i log applicativi vengono trattati con retention separata.

### 3.2 Chiavi e integrità

La documentazione DB indica che il modello è fortemente centrato su `Pensione` e sulle sue dipendenze. Per lo storico conviene:

- preservare `IdPensione` come chiave tecnica primaria di correlazione;
- preservare `NDomus/NumeroDomanda` come chiave di business;
- mantenere i riferimenti tra tabelle figlie e tabelle padre;
- evitare di rigenerare nuove identity nello storico, salvo chiavi tecniche di supporto;
- introdurre eventualmente un `IdStorico` solo come chiave interna del DB storico, non come sostituto del business key.

### 3.3 Ricerca sullo storico

La ricerca storica deve essere distinta dalla ricerca operativa:

- **operativa**: continua a usare il DB corrente;
- **storica**: usa il DB storico o viste/servizi dedicati;
- **reportistica aggregata**: unisce attuale + storico senza alterare il comportamento della ricerca standard.

Campi da indicizzare con priorità:

- `NDomus / NumeroDomanda`
- `IdPensione`
- `CodiceFiscale`
- `StatoPensione`
- `DataTentativoCalcoloDefinitivo`
- `ProgStorico`
- `SiglaCategoria`
- eventuali chiavi di sede / centro operativo / fondo

### 3.4 Processo tecnico consigliato

Il flusso più sicuro è:

1. selezione dei record eleggibili;
2. copia nel DB storico in transazione o batch atomico;
3. validazione conteggi e chiavi;
4. cancellazione dal DB operativo seguendo l'ordine di dipendenze già usato da `EliminaPensione`;
5. registrazione esito del job.

## 4. Pianificazione richiesta

| Fase | Periodo |
|---|---|
| 1. Analisi Impatto e regole di storicizzazione | 01/07/2026 - 31/07/2026 |
| 2. Modello logico DB di Storicizzazione e fornitura tabelle | 03/08/2026 - 11/09/2026 |
| 3. Software di Storicizzazione | 14/09/2026 - 30/11/2026 |
| 4. Reingegnerizzazione delle funzioni di ricerca di Visualizzazione Stato Pratiche | 01/12/2026 - 31/01/2027 |
| 5. Nuove Funzionalità di visualizzazione Report dati aggregati (Storico + Attuale) | 01/02/2027 - 26/02/2027 |

## 5. Tabelle da storicizzare

La lista seguente è costruita a partire da:

- `PN812/WSInpsPensioniLiquidazione.DataCommon/DAGestionePensione.cs` (`EliminaPensione`)
- `PN812/WSInpsPensioniLiquidazione.DataCommon/Pensioni.dbml`
- `Pensioni FS WIP DatabaseDoc v2` in DocMind (`IVS` / `IVS_DB`)

> Nota: `LogGenerico` va escluso dallo storico; `LogSoap` va trattato con retention separata.

### 5.1 Core, controllo e pratiche principali

`Pensione`, `Eliminazione`, `Titolare`, `Lavorazione`, `EsitoCalcolo`, `Stampa`, `ByPassCancellazione`, `LogSoap`, `LogGenerico`

### 5.2 Calcolo, validazioni e stato pratica

`CalcoloContributivo`, `CalcoloContributivoENPALS`, `CalcoloRetributivo`, `CalcoloRetributivoENPALS`, `DetrazioniImposta`, `IntegrazioneArt11`, `Istruttoria`, `MaggiorazioniBenefici`, `NuoveLiquidate`, `Pagamento`, `DatiControlloFelpe`, `DatiStoricoGP`

### 5.3 Anagrafiche, relazioni, collegamenti e dati accessori

`Familiari`, `Delegato`, `Tutore`, `Patronato`, `Sindacato`, `Inabilita`, `PsAsInvCiv`, `Ricoveri`, `Sentenze`, `PensioniAbbinate`, `PensioniDatiGenerici`, `PensioniINAIL`, `PensioniEstereDC`, `AltrePensioni`, `AllRichiestaRicercaDomandeANF`, `AllCodMaggiorazioneFamiliari`, `AllDanteCausa`, `AllResidenzeEstero`, `AllStatiCivili`, `AllComponentiFamiliari`

### 5.4 Quadri applicativi

`QuadroTitolare`, `QuadroDetrazioni`, `QuadroPagamento`, `QuadroLiquidazionePensione`, `QuadroDelegatoTutore`, `QuadroDatiContributivi`, `QuadroRedditi`, `QuadroFamiliari`, `QuadroDanteCausa`, `QuadroMaggiorazioniBenefici`, `QuadroSupplementi`, `QuadroBititolarita`, `QuadroEliminazione`, `QuadroDatiNoCalcolo`, `QuadroOneri`, `QuadroAventiDiritto`, `QuadroPeriodi`, `QuadroAltreDomandeCollegate`, `QuadroRichiestaBonus`, `AllQuadroDatiRecordFondo`, `QuadroDatiFondo`, `AllQuadroDatiRecordNoCalcolo`, `AllRecordDatiNoCalcolo`, `AllRecordDatiFondoINPDAP`, `AllRecordFondo`

### 5.5 Fondi, CASistiche speciali, INPDAP e estero

`PensioneFondoGAS`, `PensioneFondoET`, `PensioneFondoEL`, `PensioneFondoES`, `PensioneFondoDZ`, `PensioneFondoCL`, `PensioneFondoVL`, `PensioneFondoTT`, `PensioneFondoFST`, `PensioneFondoPT`, `PensioneFondopms`, `PensioneFondopm`, `DatiAgoPensioneFondoPI`, `DatiAgoPensioneFondoPM`, `DatiAgoTeoricoPensioneFondoPI`, `PensioneFondoPI`, `DatiServizioUtile`, `DatiServizioUtile707`, `DatiServizioUtileINPDAP707`, `PensioneFondoDatiGenerici`, `PensioneINPDAP`, `PensioniCiContributiEE`, `PensioniCiImportiValuta`, `AllPensioniCiImportiEsteri`, `AllPensioniCiPrestazioniEE`, `AllPensioneImportiEsteriCumulo`, `PensioneEsteraCumuloNoStorico`

### 5.6 Redditi, supplementi, benefici, quote e oneri

`RedditiEstero`, `RedditiFamiliari`, `RedditiIntegrazioni`, `RedditiLavoroAutonomo`, `RedditiMaggiorazioni`, `ReddS24094`, `ReddS49593`, `AllRedditiDRedd`, `RedditiPerIntegrazioneVirtuale`, `SupplementiENPALS`, `AllSupplementi`, `AllSupplementiBase`, `AllSupplementiRecordENPALS`, `SupplementiCumulo`, `BeneficioVittimeTerrorismo`, `CalcoloVittimeTerrorismo`, `VittimeTerrorismo`, `RipartizioneFondi`, `RipartizioneINPDAP`, `DL407`, `Oneri`, `AllBeneficiParticolari`, `AllDatiPostDecOriginaria`, `Enpals`, `Prepensionamento`, `AllContribuzioneEnpals`, `TrattenuteQuotePensione`, `QuotePensione`, `QuoteMiglioramentiContrattuali`, `MiglioramentiContrattuali`, `AllDatiServizioUtileINPDAP`, `AllPensioneImportiEsteriCumulo`, `AllPensioniCiImportiEsteri`, `AllPensioniCiPrestazioniEE`, `CalcoloContributivoINPGI`, `CalcoloRetributivoINPGI`, `QuotaFondoIntegrativo`, `AnniRichiestaBonus`, `MaternitaAcna`, `DatiServizioUtile`, `DatiServizioUtile707`, `DatiServizioUtileINPDAP707`

### 5.7 Supporto al prelievo, allo stato e alla ricerca

`ByPassCancellazione`, `LogSoap`, `LogGenerico`, `DatiControlloFelpe`, `DatiStoricoGP`, `Lavorazione`, `EsitoCalcolo`, `Pensione`, `QuadroEliminazione`

## 6. Tabelle e oggetti da mantenere fuori dallo storico

- `LogGenerico`: esclusione esplicita dal documento origine.
- Tabelle di decodifica/lookup: in linea generale restano nel DB operativo, salvo esigenze di replica funzionale.
- Oggetti di supporto non correlati alla conservazione della pratica.

## 7. Considerazioni architetturali aggiuntive

1. Lo storico deve essere **schema-compatible** con l'operativo, ma non necessariamente identico sul piano fisico.
2. Il processo di trasferimento deve essere **idempotente** e tracciabile.
3. La cancellazione operativa deve rispettare l'ordine delle dipendenze già codificato in `EliminaPensione`.
4. La reportistica storica va separata dalla ricerca transazionale.
5. La presenza di `ByPassCancellazione` indica che il dominio include eccezioni amministrative: lo storico deve conservarne il contesto.

## 8. Riferimenti

- `Doc/SpotActivity/Storicizzazione/Logiche di storicizzazione e ricerca dati.docx`
- `PN812/WSInpsPensioniLiquidazione.DataCommon/DAGestionePensione.cs`
- `PN812/WSInpsPensioniLiquidazione.DataCommon/Pensioni.dbml`
- DocMind: `Pensioni FS WIP DatabaseDoc v2`
- `Doc/ENGenius`
