# Analisi Tecnica – Sistema Legacy IVS Liquidazione Pensioni

**Versione:** 1.0  
**Data:** Marzo 2026  
**Fonte:** Analisi diretta del codice sorgente C# (1.646 file)  
**Progetti analizzati:** PN809, PN812, PN813, PN815, PN818

---

## 1. Scopo del Sistema

Il sistema è l'applicazione INPS per la **liquidazione delle pensioni**, ovvero il processo
attraverso cui un operatore di sede calcola, verifica e registra l'importo di una
pensione da liquidare (erogare per la prima volta) a partire da una domanda prelevata
dall'archivio centrale (WebDom / sistema "Domus").

Il sistema gestisce **tre fondi pensionistici distinti**:

| Sigla | Denominazione | Progetto BL |
|---|---|---|
| **FS** | Fondo Speciale / GDP (Gestione Dipendenti Pubblici) | PN813 |
| **AGO** | Assicurazione Generale Obbligatoria (pensioni INPS standard) | PN815 |
| **CI** | Convenzioni Internazionali (pensioni con quota estera / cumulo) | PN818 |

---

## 2. Architettura del Sistema

```
┌─────────────────────────────────────────────────────────────────────┐
│                    PN809 – WebApp ASP.NET                           │
│           LiquidazionePensioniFS (+ .Presenter)                     │
│                                                                     │
│  Pattern MVP (Model-View-Presenter)                                 │
│  ~80 Presenter | ~80 IView | Autenticazione Windows (INPS.DNA)      │
└────────────────────────────┬────────────────────────────────────────┘
                             │ WCF (SOAP)
                             ▼
┌─────────────────────────────────────────────────────────────────────┐
│                    PN812 – WCF Wrapper Service                      │
│           WSInpsPensioniLiquidazione (ServizioLiquidazione.svc)     │
│                                                                     │
│  Namespace: http://soa.inps.it/domainservices/pensioni/             │
│             servicecontracts/Liquidazione/1_0                       │
│                                                                     │
│  Strati:  BL (Business Logic) | BLCommon | Data | DataCommon        │
│  Smista le chiamate verso il servizio specializzato in base         │
│  al tipo di appartenenza (FS / AGO / CI)                            │
└──────────────┬───────────────────┬───────────────────┬─────────────┘
               │ WCF               │ WCF               │ WCF
               ▼                   ▼                   ▼
┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐
│   PN813 – FS     │  │   PN815 – AGO    │  │   PN818 – CI     │
│ ServizioLiquid.  │  │ ServizioLiquid.  │  │ ServizioLiquid.  │
│ azioneFs.svc     │  │ azioneAgo.svc    │  │ azioneCi.svc     │
│                  │  │                  │  │                  │
│  BL | Data       │  │  BL | Data       │  │  BL | Data       │
└────────┬─────────┘  └────────┬─────────┘  └────────┬─────────┘
         │                     │                      │
         └─────────────────────┴──────────────────────┘
                                │
                                ▼
                    ┌───────────────────────┐
                    │  SQL Server            │
                    │  Pensioni_Fs_WIP /     │
                    │  Pensioni_Fs           │
                    └───────────────────────┘
                                │
         ┌──────────────────────┼────────────────────┐
         ▼                      ▼                    ▼
   Sistema WebDom          FELPE                   SAI / INPDAP
   (domande pensione)   (aggiornamento)         (sistemi esterni INPS)
```

### 2.1 Comunicazione tra i livelli

```
PN809 (WebApp)
  └─ Presenter → IView → chiama ServizioLiquidazioneClient (proxy WCF)
       └─ PN812 (Wrapper)
            ├─ Operazioni comuni (Titolare, Familiare, Pagamento, Redditi,
            │   Supplementi, DanteCausa, Detrazioni, Calcolo, Stampa, …)
            │   → Smista a PN813/815/818 in base a TipoAppartenenza
            └─ Operazioni di amministrazione (LiquidazioniAbilitate,
                TrasformazioniAbilitate, TipologieNonAbilitate, Aggiornamenti)
```

---

## 3. PN809 – WebApp ASP.NET (LiquidazionePensioniFS)

### 3.1 Pattern Architetturale: MVP

```
IView (interfaccia)  ←→  Presenter  ←→  WCF Proxy (ServizioLiquidazioneClient)
      ↑
  Page .aspx.cs (implementa IView)
```

Ogni schermata è composta da:
- `IView/IXxx.cs` → contratto della vista (proprietà esposte alla pagina)
- `PresenterXxx.cs` → logica di recupero/salvataggio dati chiamando il WCF
- `Page.aspx.cs` → implementa IView, gestisce gli eventi UI

### 3.2 Moduli e Schermate

#### 3.2.1 Ricerca e Navigazione

| Presenter | Funzione |
|---|---|
| `PresenterRicercaPosizione` | Ricerca una pratica pensionistica per: NumeroDomanda, ProgStorico, CodiceFiscale, Nome+Cognome+DataNascita |
| `PresenterVisualizzaStatoPratiche` | Lista pratiche filtrabili per: Sede, Fondo, Cassa, Matricola, Stato, Date presentazione/elaborazione, Gruppo/Prodotto/Tipo |
| `PresenterLiquidazione` | Recupera le info generali di una domanda in lavorazione (NDomus, Categoria, Tipo, Sede, Certificato, CF, Nome, Cognome) |
| `PresenterMenuLeft` | Menu laterale di navigazione tra i quadri della pratica |
| `PresenterMenuLeftAltreFunzioni` | Sotto-menu funzioni amministrative (disponibile agli amministratori) |
| `PresenterHomePage` | Home page con avvisi, versioni, messaggi Hermes per tipo di appartenenza |

#### 3.2.2 Quadro Titolare e Anagrafica

| Presenter | Funzione |
|---|---|
| `PresenterTitolare` | Visualizzazione/modifica dei dati anagrafici del titolare (CF, residenza, stato civile, residenze estere). Integrazione con ARCA per aggiornamento anagrafica |
| `PresenterDanteCausa` | Dati del dante causa (de cuius) nelle reversibilità: anagrafica DC, altra pensione, pensione CI, pensione diretta, redditi sentenza 49593 |
| `PresenterDelegatoTutore` | Gestione delegato e tutore (ricerca per CF o dati parziali) |

#### 3.2.3 Quadro Liquidazione Pensione

| Presenter | Funzione |
|---|---|
| `PresenterLiquidazionePensione` | Dati generici, dati assicurativi, Legge 460, bititolarità INAIL. Chiama `ServizioLiquidazioneFsClient` (PN813) o equivalente per AGO/CI |
| `PresenterLiquidazionePensione` (AGO/CI) | Include in più: opzione, istruttoria, provenienza, INAIL, sentenza art.4, sentenze, dati istruttoria |

#### 3.2.4 Quadro Dati Contributivi

| Presenter | Funzione |
|---|---|
| `PresenterDatiContributivi` | Dati contributivi comuni (recupera da DB / host). Calcolo principale delle contribuzioni |
| `PresenterDatiContributiviAGO` | AGO-specifico: Pro-Rata, stati esteri cumulo, Quote Miglioramenti Contrattuali, Quota Fondo Integrativo, Quota Fondo INPGI |
| `PresenterDatiContributiviCI` | CI-specifico: Pro-Rata, stati esteri (con cittadinanza), Importi Esteri, Maternità ACNA, Lavoratori Autonomi, Redditi Integrazione Virtuale, Dati Post-Dec Originaria |

#### 3.2.5 Quadro Dati Fondo

| Presenter | Funzione |
|---|---|
| `PresenterDatiFondo` | Gestione record fondo (FS): add/delete registrazioni fondo, dati calcolo, dati calcolo 707, legge 460 per fondo, privilegiate, articolo 2 |
| `PresenterDatiFondoAgo` | Gestione record fondo (AGO): struttura simile ma con dati specifici AGO |

#### 3.2.6 Quadro Maggiorazioni e Benefici

| Presenter | Funzione |
|---|---|
| `PresenterMaggiorazioneBenefici` | Benefici comuni: ex-combattente, benefici generali, DL407, privilegiate, articolo 2 (FS) |
| `PresenterMaggiorazioneBeneficiAgo` | AGO: in più — maggiorazioni generali, beneficio vittime terrorismo |
| `PresenterMaggiorazioneBeneficiCi` | CI: ex-combattente, benefici, maggiorazioni, vittime terrorismo |

#### 3.2.7 Quadro Familiari e Detrazioni

| Presenter | Funzione |
|---|---|
| `PresenterFamiliari` | Gestione familiari a carico (ricerca ARCA, salvataggio, cancellazione). Consultazione ANF (Assegno Nucleo Familiare) |
| `PresenterDetrazioni` | Detrazioni fiscali: visualizzazione soggetti, verifica, salvataggio |
| `PresenterAbilitazioneUniDetra` | Abilitazione UniDetra (detrazioni unificate) |

#### 3.2.8 Quadro Redditi

| Presenter | Funzione |
|---|---|
| `PresenterRedditi` | Redditi del titolare per calcolo integrazioni. Verifica e salvataggio. Chiamate a servizi REDEX/INPS |

#### 3.2.9 Quadro Supplementi

| Presenter | Funzione |
|---|---|
| `PresenterSupplementi` | Gestione supplementi di pensione: caricamento, salvataggio, cancellazione |

#### 3.2.10 Quadro Oneri

| Presenter | Funzione |
|---|---|
| `PresenterOneri` | Gestione oneri (es. riscatti, ricongiunzioni): caricamento dal sistema ONERI |

#### 3.2.11 Quadro Pagamento

| Presenter | Funzione |
|---|---|
| `PresenterPagamento` | Modalità di pagamento della pensione: banca/posta, IBAN, ufficio pagatore. Ricerca uffici pagatori. Salvataggio e cancellazione |

#### 3.2.12 Quadro Periodi e Aventi Diritto

| Presenter | Funzione |
|---|---|
| `PresenterPeriodi` | Periodi contributivi del titolare |
| `PresenterAventiDiritto` | Aventi diritto (es. figli per reversibilità) con relativi periodi |

#### 3.2.13 Quadro Bititolarità

| Presenter | Funzione |
|---|---|
| `PresenterBititolarita` | Gestione bititolarità: altra pensione collegata (es. pensione diretta + reversibilità cumulate) |

#### 3.2.14 Calcolo e Invio

| Presenter | Funzione |
|---|---|
| `PresenterInvioCalcolo` | Avvio del calcolo della domanda (`CalcolaDomanda`). Gestisce il flag `isVerify` (solo verifica) vs calcolo definitivo. Restituisce stato pensione, certificato, chiave pensione |
| `PresenterElaborazionePosizione` | Prelievo/sprenotazione della domanda dall'archivio centrale (host) |
| `PresenterProvvisorieCoefficienti` | Gestione decorrenze provvisorie e coefficienti |
| `PresenterNoCalcolo` | Gestione domande che non richiedono calcolo automatico |
| `PresenterAggiornaCalcoloNoInd` | Aggiornamento calcolo senza indennizzo |

#### 3.2.15 Quadro Eliminazione

| Presenter | Funzione |
|---|---|
| `PresenterEliminazione` | Eliminazione completa di una pensione (`EliminaPensioneByNumeroDomanda`). Richiede ruolo adeguato. Eseguita tramite ~130 stored procedure in transazione |

#### 3.2.16 Stampa

| Presenter | Funzione |
|---|---|
| `PresenterStampa` | Generazione stampa PDF della domanda (per numero domanda o chiave pensione) |

#### 3.2.17 Aggiornamenti Batch (funzione Admin)

| Presenter | Tipo Aggiornamento | Descrizione |
|---|---|---|
| `PresenterAggiornamento` | WebDom | Aggiorna dati da sistema WebDom (domande web) |
| `PresenterAggiornamento` | FELPE | Aggiorna FELPE (sistema di controllo esistenza/importi) |
| `PresenterAggiornamento` | Oneri | Aggiorna oneri da sistema ONERI |
| `PresenterAggiornamento` | Cumulo | Aggiornamento cumulo pensioni (CI05) |
| `PresenterAggiornamento` | SAI | Aggiornamento SAI (Sistema Addebiti INPS) |
| `PresenterAggiornamento` | INPDAP | Aggiornamento dati previdenza ex-INPDAP |
| `PresenterAggiornamento` | Tot | Aggiornamento totale |
| `PresenterAggiornamento` | NoteDiDebito | Aggiornamento note di debito |
| `PresenterAggiornamento` | PianiDiPagamento | Aggiornamento piani di pagamento |
| `PresenterAggiornamentoWebDom` | WebDom puntuale | Aggiornamento WebDom per singola domanda |

#### 3.2.18 Funzioni Amministrative

| Presenter | Funzione |
|---|---|
| `PresenterLiquidazioniAbilitate` | Configurazione dei tipi di liquidazione abilitati per sede |
| `PresenterTrasformazioniAbilitate` | Configurazione delle trasformazioni abilitate per sede |
| `PresenterTipologieNonAbilitate` | Configurazione delle tipologie non abilitate per fondo |
| `PresenterBypassControlli` | Gestione bypass dei controlli automatici per una pratica |
| `PresenterBypassTipologieNonAbilitate` | Bypass delle tipologie non abilitate |
| `PresenterControlliDinamici` | Visualizzazione e gestione dei controlli dinamici |
| `PresenterSbloccoDomanda` | Sblocco di una domanda bloccata (con verifica sede) |
| `PresenterSbloccoCancellazione` | Sblocco della cancellazione |
| `PresenterCambioStatoDomanda` | Cambio manuale dello stato di una domanda |
| `PresenterRiassegnazioneDomanda` | Riassegnazione di una domanda a un'altra sede/operatore |
| `PresenterPuliziaDomanda` | Pulizia dei dati di una domanda (reset) |
| `PresenterLavorazioneManualeAutomatiche` | Gestione lavorazione manuale vs automatica |
| `PresenterSedi` | Visualizzazione e gestione sedi |

#### 3.2.19 Altre Funzioni

| Presenter | Funzione |
|---|---|
| `PresenterFaq` | FAQ integrate nell'applicazione |
| `PresenterMessaggiHermes` | Messaggi di sistema Hermes (comunicazioni INPS) |
| `PresenterVersioni` | Gestione versioni applicazione con note di rilascio |
| `PresenterInvioSegnalazione` | Invio segnalazioni/ticket |
| `PresenterAvvisi` | Avvisi operativi visualizzati in home page |
| `PresenterRichiestaBonus` | Gestione richiesta bonus pensionistici |

#### 3.2.20 Funzioni Specifiche per Tipologia Esodo/Prepensionamento

| Presenter | Funzione |
|---|---|
| `PresenterAziendeCredito` | Gestione aziende con credito |
| `PresenterAziendeEditoriali` / `LetteraB` / `PerTipo0171` / `PerTipo0179` | Aziende del settore editoriale per vari tipi di esodo |
| `PresenterAziendeESOAMB` / `ESOTEL` / `ESOTRA` / `ESPA` / `ESOPMI` | Aziende per esodo (ambiente, telecomunicazioni, trasporti, PA, PMI) |
| `PresenterAziendeVESO29` / `VESO33` / `VOESO` | Versamenti aziende per esodo |
| `PresenterBancaFideiussione` / `ESOPMI` / `ESPA` | Banche fideiussorie per le varie tipologie di esodo |
| `PresenterCambioDataLimiteINDCOM` | Data limite domande INDCOM (indennizzo commercianti) |
| `PresenterCambioDataLimitePrepensionamentoLetteraB` | Data limite prepensionamento Lettera B |
| `PresenterContribuzioneEnpals` | Contribuzione ENPALS (ex-fondo spettacolo) |

---

## 4. PN812 – WCF Wrapper Service (ServizioLiquidazione.svc)

### 4.1 Ruolo

PN812 è il **punto di accesso unico** dalla WebApp PN809 verso i servizi specializzati.
Espone un unico `IServizioLiquidazione` che:
1. Implementa le operazioni **comuni a tutti e tre i fondi** (titolare, familiare, pagamento, detrazioni, redditi, supplementi, dante causa, calcolo, stampa, eliminazione)
2. **Smista** le chiamate fondo-specifiche verso PN813 (FS), PN815 (AGO) o PN818 (CI) in base al `TipoAppartenenza` della domanda

### 4.2 Strati

```
WSInpsPensioniLiquidazione       ← WCF endpoint (.svc)
WSInpsPensioniLiquidazione.BL    ← Business Logic (Gestione*)
WSInpsPensioniLiquidazione.BLCommon  ← Logica condivisa (ANF, Familiari, etc.)
WSInpsPensioniLiquidazione.Data  ← Accesso DB specifico
WSInpsPensioniLiquidazione.DataCommon ← Accesso DB condiviso
```

### 4.3 Operazioni Principali (IServizioLiquidazione)

#### Riepilogo e Stato

| Metodo | Descrizione |
|---|---|
| `GetRiepilogoByKey` | Recupera il riepilogo completo di una domanda (dati anagrafici, stato, fondo) |
| `GetStatoPraticaByKey` | Stato della pratica filtrato per vari criteri |
| `GetAreaHomepage` | Dati homepage (avvisi, versioni, messaggi) per tipo appartenenza |
| `GetListaVersioni` | Lista versioni applicazione |
| `GetMessaggiHermes` | Messaggi Hermes per tipo appartenenza |

#### Titolare / Anagrafica

| Metodo | Descrizione |
|---|---|
| `GetAreaTitolareByDomanda` | Recupera titolare completo |
| `StoreAreaTitolare` | Salva titolare (con flag isTabAnagraficaSaved) |
| `StoreAnagrafica` | Salva solo anagrafica |
| `StoreStatoCivile` / `DeleteStatoCivile` | Salva/elimina stato civile |
| `StoreResidenzeEstere` / `DeleteResidenzeEstere` | Salva/elimina residenze estere |
| `GetAnagraficaSoggettoByCodiceFiscale` | Ricerca anagrafica da CF |
| `GetAnagraficaByDatiPersonaliParziali` | Ricerca anagrafica da dati parziali |
| `AggiornaAnagraficaTitolareByArca` | Aggiorna anagrafica dal sistema ARCA |
| `AggiornaAnagraficaSoggetto` | Aggiorna anagrafica soggetto |

#### Delegato / Tutore

| Metodo | Descrizione |
|---|---|
| `GetDelegatoByNumeroDomanda` | Recupera delegato |
| `GetTutoreByNumeroDomanda` | Recupera tutore |
| `StoreDelegatoTutore` | Salva entrambi |
| `StoreDelegato` / `StoreTutore` | Salva singolarmente |
| `DeleteDelegato` / `DeleteTutore` | Elimina |
| `IsNotDelegatoOrTutorePresent` | Verifica assenza |

#### Familiari e Detrazioni

| Metodo | Descrizione |
|---|---|
| `GetFamiliareByNumeroDomanda` | Recupera familiari + decodifiche |
| `SalvaFamiliari` | Salva familiari (con integrazione ANF) |
| `CancelFamiliari` | Cancella familiari |
| `GetDetrazioniByDomanda` | Recupera detrazioni |
| `GetSoggettiDetrazioniByDomanda` | Recupera soggetti per detrazioni |
| `VerifyDetrazioniByDomanda` | Verifica coerenza detrazioni |

#### Redditi

| Metodo | Descrizione |
|---|---|
| `GetRedditiByDomanda` | Recupera redditi |
| `VerifyRedditiByDomanda` | Verifica redditi (con confronto versione originale) |

#### Supplementi

| Metodo | Descrizione |
|---|---|
| `GetSupplementiByDomanda` | Recupera supplementi |
| `SalvaSupplementiByDomanda` | Salva supplementi |
| `StoreDatiSupplementi` | Salva dati supplementi |
| `DeleteDatiSupplementiByDomanda` | Cancella supplementi |

#### Dante Causa

| Metodo | Descrizione |
|---|---|
| `GetDanteCausaByDomanda` | Recupera dati dante causa |
| `StoreDanteCausa` | Salva dati completi DC |
| `CancelDanteCausa` | Cancella |
| `StoreDatiAnagraficaDC` | Salva anagrafica DC |
| `StoreDatiAltraPensione` | Altra pensione DC |
| `StoreDatiPensioneCI` | Pensione CI (internaz.) del DC |
| `StoreDatiPensioneDiretta` | Pensione diretta DC |
| `StoreDatiRedditiSentenza49593` | Redditi sentenza 49593 DC |

#### Pagamento

| Metodo | Descrizione |
|---|---|
| `GetPagamentoByNumeroDomanda` | Recupera dati pagamento (con ABI cassa sede) |
| `StorePagamento` | Salva modalità pagamento |
| `CancelPagamentoByNumeroDomanda` | Cancella pagamento |
| `GetUfficiPagatori` | Ricerca uffici pagatori |

#### Calcolo e Stampa

| Metodo | Descrizione |
|---|---|
| `CalcolaDomanda` | **Calcolo principale** della pensione. Restituisce: stato, certificato, chiave pensione, lista ANF, prenotazioni elaborazioni, transactionId, flagIndennizzo |
| `GetIsDomandaVerify` | Verifica se la domanda è in stato "verify" |
| `GetStampaDomanda` | Genera PDF stampa domanda |
| `GetStampaDomandaByChiavePensione` | Genera PDF da chiave pensione |

#### Eliminazione

| Metodo | Descrizione |
|---|---|
| `EliminaPensioneByNumeroDomanda` | Elimina completamente una pensione. Richiede: matricola, sede, centro operativo, TipoAppartenenza, Ruolo, sedeDiAppartenenza |

#### Aggiornamenti da sistemi esterni

| Metodo | Sistema esterno |
|---|---|
| `AggiornaCI05` | CI05 (cumulo internazionale) |
| `AggiornaWebDom` | WebDom (domande web) |
| `AggiornaFelpe` | FELPE |
| `AggiornaOneri` | Sistema ONERI |
| `AggiornaSai` | SAI (Sistema Addebiti) |
| `AggiornaINPDAP` | Ex-INPDAP |
| `AggiornaNoteDiDebito` | Note di debito |
| `AggiornaPianiDiPagamento` | Piani di pagamento |
| `AggiornaEquoInd` | Equo indennizzo |
| `AggiornaIndennSpec` | Indennità speciale |

#### Gestione Domanda (admin)

| Metodo | Descrizione |
|---|---|
| `SbloccoDomanda` | Sblocca domanda (verifica sede diversa) |
| `RiassegnazioneDomanda` | Riassegna domanda |
| `GetAllLiquidazioniAbilitate` / `Store` / `Delete` | CRUD liquidazioni abilitate |
| `GetAllTrasformazioniAbilitate` / `Store` / `Delete` | CRUD trasformazioni abilitate |
| `GetAllTipologieNonAbilitate` / `Store` / `Delete` | CRUD tipologie non abilitate |
| `InvioSegnalazione` | Invio segnalazione/ticket |

---

## 5. PN813 – WSInpsPensioniLiquidazioneFS (Fondo Speciale / GDP)

### 5.1 Specificità del Fondo FS

Il Fondo Speciale copre i **dipendenti pubblici** (ex-INPDAP). Caratteristiche peculiari:
- **DatiContributivi**: calcolo 707 (art.707 L.335/95), dati Art.14 e Art.11, Ante-67 (periodi ante-1967), SL336
- **DatiFondo**: gestione di record multipli per fondo con struttura a record (RecordFondo)
- **LiquidazionePensione**: Legge 460, Bititolarità INAIL, dati precedente pensione
- **MaggiorazioniBenefici**: ex-combattente, benefici, DL407, privilegiate, articolo 2
- **Prelievo domanda** da archivio host (con `PrelevaDomanda` / `EseguiSprenotazione`)
- Integrazione con: **FELPE** (Fondo Elettivo Lavoratori Pubblici Esclusi), deleghe sindacali

### 5.2 Business Logic PN813

| Classe BL | Responsabilità |
|---|---|
| `GestioneCalcoloDomanda` | Orchestrazione calcolo FS |
| `GestioneContrib` | Dati contributivi specifici FS (707, art.14, art.11, ante-67, SL336) |
| `GestioneDatiFondo` | Record fondo e sotto-sezioni |
| `GestioneAreaRecordFondo` | Singolo record del fondo |
| `GestioneAreaNoCalcolo` | Domande no-calcolo |
| `GestioneLiquidazionePensione` | Dati liquidazione FS |
| `GestioneMaggiorazioniBenefici` | Maggiorazioni FS |
| `GestionePrelievo` | Prelievo/sprenotazione da host |
| `GestioneAMGFelpe` | Integrazione FELPE |
| `GestioneAggiornamentoPECO` | Aggiornamento PECO (sistema host) |
| `GestioneDelegheSindacali` | Deleghe sindacali |
| `GestioneANF` | ANF (Assegno Nucleo Familiare) |
| `GestioneControlli` | Controlli di validazione FS |
| `MappingDaHost` / `MappingVersoHost` | Mapping dati ↔ formato host legacy |
| `MappingDaHostNew` / `MappingVersoHostNew` | Mapping dati ↔ nuovo formato host |

---

## 6. PN815 – WSInpsPensioniLiquidazioneAgo (AGO)

### 6.1 Specificità del Fondo AGO

L'AGO è l'assicurazione pensionistica **standard INPS** per lavoratori dipendenti e autonomi. Caratteristiche peculiari:
- **DatiContributivi**: Pro-Rata (calcolo proporzionale), stati esteri cumulo (per pensionati con periodi all'estero), Quote Miglioramenti Contrattuali, Quota Fondo Integrativo, Quota Fondo INPGI
- **LiquidazionePensione**: in più rispetto a FS — opzione regime, istruttoria, provenienza, INAIL, Sentenza Art.4, Sentenze generali
- **Bititolarità**: gestione altra pensione collegata (diretta + reversibilità)
- **Vittime Terrorismo**: beneficio specifico AGO e CI
- **NuovoCalcolo**: flag per ricalcolo con nuovo algoritmo (reingegnerizzazione)
- **GP4**: prelievo specifico da archivio GP4 (Gestione Prestazioni 4)

### 6.2 Business Logic PN815 (in più rispetto a PN813)

| Classe BL | Responsabilità |
|---|---|
| `GestioneBititolarita` | Bititolarità e altra pensione |
| `GestioneNACI` | Interfaccia NACI (Nucleo Anagrafico Contribuenti) |
| `GestioneSIN` | Interfaccia SIN (Sistema Informativo Nazionale) |
| `GestioneTotalIvs` | Calcolo totale IVS (Invalidità, Vecchiaia, Superstiti) |
| `GestioneAllegatiConvenzioni` | Allegati per convenzioni internazionali AGO |
| `GestioneDatiPensioni` | Dati pensioni associate AGO |

---

## 7. PN818 – WSInpsPensioniLiquidazioneCi (Convenzioni Internazionali)

### 7.1 Specificità del Fondo CI

Le **Convenzioni Internazionali** gestiscono le pensioni di lavoratori che hanno periodi
contributivi in più paesi (UE o con accordi bilaterali). Caratteristiche peculiari:
- **DatiContributivi**: Pro-Rata (fondamentale per calcolo quota italiana), stati esteri + cittadinanza del titolare, importi esteri, dati post-decorrenza originaria
- **Specifici CI**: maternità ACNA, lavoratori autonomi, redditi per integrazione virtuale
- **Non presenti in CI**: Legge 460 (FS-only), record fondo strutturato, nuovo calcolo GP4

### 7.2 Differenze vs AGO per DatiContributivi

| Funzionalità | AGO | CI |
|---|---|---|
| Pro-Rata | ✅ (con stati esteri cumulo) | ✅ (con cittadinanza) |
| Quote Miglioramenti Contrattuali | ✅ | ❌ |
| Quota Fondo Integrativo / INPGI | ✅ | ❌ |
| Sentenza Art.4 | ✅ | ❌ |
| Maternità ACNA | ❌ | ✅ |
| Lavoratori Autonomi | ❌ | ✅ |
| Redditi Integrazione Virtuale | ❌ | ✅ |
| Dati Post-Dec Originaria | ❌ | ✅ |
| Importi Esteri | ❌ | ✅ |

---

## 8. Confronto Funzionalità tra i Tre Fondi

| Funzionalità | FS (PN813) | AGO (PN815) | CI (PN818) |
|---|---|---|---|
| LiquidazionePensione dati generici | ✅ | ✅ | ✅ |
| LiquidazionePensione dati assicurativi | ✅ | ✅ | ✅ |
| Legge 460 | ✅ | ❌ | ❌ |
| Bititolarità INAIL | ✅ | ✅ | ✅ |
| Opzione regime | ❌ | ✅ | ✅ |
| Istruttoria | ❌ | ✅ | ✅ |
| Provenienza | ❌ | ✅ | ✅ |
| INAIL (pensioni) | ❌ | ✅ | ✅ |
| Dati precedente pensione | ✅ | ❌ | ❌ |
| Calcolo 707 | ✅ | ❌ | ❌ |
| Dati Art.14 e Art.11 | ✅ | ❌ | ❌ |
| Ante-67 / SL336 | ✅ | ❌ | ❌ |
| Record Fondo multipli | ✅ | ✅ | ❌ |
| Pro-Rata | ❌ | ✅ | ✅ |
| Stati Esteri Cumulo | ❌ | ✅ | ✅ (+ cittadinanza) |
| Vittime Terrorismo | ❌ | ✅ | ✅ |
| Nuovo Calcolo (flag) | ❌ | ✅ | ❌ |
| GP4 Prelievo | ❌ | ✅ | ❌ |
| FELPE | ✅ | ❌ | ❌ |
| Deleghe Sindacali | ✅ | ❌ | ❌ |
| Maternità ACNA | ❌ | ❌ | ✅ |
| Lavoratori Autonomi | ❌ | ❌ | ✅ |
| Redditi Integrazione Virtuale | ❌ | ❌ | ✅ |
| Importi Esteri | ❌ | ❌ | ✅ |

---

## 9. Ruoli Utente e Autorizzazioni

| Codice Ruolo | Descrizione | Fondo |
|---|---|---|
| `P4697` | Operatore | Tutti |
| `P4677` | Amministratore FS/GDP | FS |
| `P8854` | Amministratore AGO | AGO |
| `P8855` | Amministratore CI | CI |
| `P4678` | Utente FS/GDP | FS |
| `P8856` | Utente AGO | AGO |
| `P8857` | Utente CI | CI |
| `P8974` | Direttore / Capo Processo FS/GDP | FS |
| `P8975` | Direttore / Capo Processo AGO | AGO |
| `P8976` | Direttore / Capo Processo CI | CI |

### Matrice permessi (funzionalità chiave)

| Funzionalità | Operatore | Utente | Amministratore | Direttore |
|---|---|---|---|---|
| Ricerca pratiche | ✅ | ✅ | ✅ | ✅ |
| Compilazione quadri | ✅ | ✅ | ✅ | ✅ |
| Calcolo domanda | ✅ | ✅ | ✅ | ✅ |
| Eliminazione pensione | ❌ | ❌ | ✅ | ✅ |
| Configurazione Liquidazioni Abilitate | ❌ | ❌ | ✅ | ✅ |
| Aggiornamenti batch | ❌ | ❌ | ✅ | ✅ |
| Sblocco domanda | ❌ | ❌ | ✅ | ✅ |
| Cambio stato domanda | ❌ | ❌ | ✅ | ✅ |
| Riassegnazione domanda | ❌ | ❌ | ✅ | ✅ |

---

## 10. Flusso Principale: Liquidazione di una Pensione

```
1. ACCESSO
   └─ Autenticazione Windows → verifica ruolo (INPS.DNA)
   └─ Home page: avvisi operativi, messaggi Hermes, versioni

2. RICERCA POSIZIONE
   └─ Per NumeroDomanda / CF / Nome+Cognome+DataNascita
   └─ Presenter: PresenterRicercaPosizione
   └─ WCF: GetRiepilogoByKey

3. PRELIEVO DOMANDA (solo FS / AGO)
   └─ Prelieva la domanda dall'archivio host (blocca la pratica)
   └─ WCF: PrelevaDomanda → PN813/815

4. COMPILAZIONE QUADRI (in ordine tipico)
   ┌── Titolare (anagrafica, stato civile, residenze estere)
   ├── Dante Causa (per reversibilità)
   ├── Liquidazione Pensione (dati generici, assicurativi, fondo-specifici)
   ├── Dati Contributivi (periodi, calcoli, pro-rata se CI/AGO)
   ├── Maggiorazioni e Benefici
   ├── Supplementi
   ├── Redditi
   ├── Detrazioni (fiscali)
   ├── Familiari (ANF)
   ├── Pagamento (IBAN / ufficio pagatore)
   ├── Oneri
   ├── Periodi e Aventi Diritto
   └── Delegato / Tutore

5. AGGIORNAMENTI PRE-CALCOLO (se necessario)
   ├── AggiornaWebDom (dati domanda aggiornati da WebDom)
   ├── AggiornaFelpe (controllo FELPE)
   └── AggiornaCI05 / AggiornaOneri / AggiornaSai (per fondo)

6. VERIFICA (CalcolaDomanda con isVerify=true)
   └─ WCF: CalcolaDomanda → PN813/815/818
   └─ Restituisce errori/avvisi senza salvare definitivamente

7. CALCOLO DEFINITIVO (CalcolaDomanda con isVerify=false)
   └─ WCF: CalcolaDomanda → PN813/815/818
   └─ Output: statoPensione, certificato, chiavePensione
   └─ Gestisce: consultazioni ANF, prenotazioni elaborazioni

8. STAMPA
   └─ GetStampaDomanda / GetStampaDomandaByChiavePensione
   └─ Output: PDF del provvedimento di liquidazione

9. QUADRI SEMAFORI (verifica completezza)
   └─ PresenterQuadriSemafori: verifica che tutti i quadri
      obbligatori siano compilati prima del calcolo

10. ELIMINAZIONE (solo admin/direttore, se necessario)
    └─ EliminaPensioneByNumeroDomanda → 130+ SP in transazione
```

---

## 11. Dipendenze Esterne (Servizi INPS)

| Sistema | Tipo | Uso nel sistema |
|---|---|---|
| **WebDom** | SOAP/HTTP | Recupero domande pensione dall'archivio centrale |
| **ARCA** | WCF/SOAP | Anagrafica centralizzata INPS (aggiornamento dati anagrafici) |
| **FELPE** | SOAP/Batch | Controllo importi pensioni esistenti (solo FS) |
| **SAI** | WCF | Sistema Addebiti INPS |
| **INPDAP** | WCF | Dati previdenziali ex-INPDAP |
| **ANF** | WCF | Assegno Nucleo Familiare (consultazione unificata) |
| **SIN** | WCF | Sistema Informativo Nazionale (solo AGO) |
| **NACI** | WCF | Nucleo Anagrafico Contribuenti (solo AGO) |
| **Host (sistema mainframe)** | Mapping | Dati contributivi dal mainframe INPS (MappingDaHost/VersoHost) |
| **Hermes** | WCF | Messaggi di sistema operativi |
| **ONERI** | WCF | Sistema oneri (riscatti, ricongiunzioni) |
| **CI05** | WCF | Cumulo internazionale (solo CI/AGO) |
| **INPS.DNA** | Framework | Framework proprietario INPS (auth, logging, WCF hosting) |
| **AggPec** | WCF | Aggiornamento PEC (notifiche digitali) |

---

## 12. Pattern Tecnici e Qualità del Codice

### 12.1 Pattern utilizzati

| Pattern | Dove | Note |
|---|---|---|
| **MVP** (Model-View-Presenter) | PN809 | Separazione UI/logica. ~80 Presenter. Riduce testabilità diretta |
| **WCF Service Layer** | PN812/813/815/818 | SOA (Service-Oriented Architecture). SOAP/HTTP |
| **Data Contract / Service Contract** | PN812 | Contratti WCF ben definiti (Area* data contracts) |
| **Repository / Data Layer** | PN813/815/818 (.Data) | Separazione accesso DB |
| **Mapping Layer** | PN813/815 | MappingDaHost / MappingVersoHost (host mainframe ↔ .NET) |

### 12.2 Framework e Tecnologie

| Tecnologia | Versione/Dettaglio |
|---|---|
| **.NET Framework** | 4.x (dedotto da WCF e ASP.NET WebForms) |
| **ASP.NET** | WebForms (pagine .aspx) |
| **WCF** | Windows Communication Foundation (SOAP) |
| **SQL Server** | 2016 (Pensioni_Fs_WIP) |
| **Autenticazione** | Windows Authentication (INPS.DNA) |
| **INPS.DNA** | Framework proprietario INPS (hosting WCF, logging, sicurezza) |

### 12.3 Debito Tecnico Identificato

| # | Problema | Impatto |
|---|---|---|
| 1 | WebForms (ASP.NET) — tecnologia obsoleta | 🔴 Alto — difficile manutenzione, no mobile, no REST |
| 2 | WCF SOAP — tecnologia obsoleta | 🔴 Alto — non portabile su .NET Core/5+ nativamente |
| 3 | Codice duplicato PN813/815/818 | 🟡 Medio — logica simile (titolare, familiare, calcolo) duplicata per ogni fondo |
| 4 | Mapping manuale da/verso mainframe | 🟡 Medio — fragile, dipendente da formato host legacy |
| 5 | ~130 stored procedure per l'eliminazione | 🟡 Medio — difficile versionamento e test |
| 6 | Nessuna unit test visibile nel progetto principale | 🟡 Medio — poca copertura test |
| 7 | Password in `appsettings.json` in chiaro | 🔴 Alto — rischio sicurezza |
| 8 | SQL Server 2016 (EOL) | 🟡 Medio — aggiornare a 2019/2022 |
| 9 | Presenter istanziano il WCF proxy con `new` | 🟢 Basso — nessun DI, difficile mock per test |
| 10 | Nomi variabili misti IT/EN e abbreviazioni | 🟢 Basso — leggibilità del codice |

---

## 13. Struttura Fisica dei Progetti

```
C:\Training\IVS\Legacy\
├── PN809\                                    WebApp principale
│   ├── LiquidazionePensioniFS\               → Pagine ASPX (UI)
│   └── LiquidazionePensioniFS.Presenter\     → Presenter, IView, Contract, Enum
│       ├── IView\                            → ~80 interfacce vista
│       ├── Contract\                         → DTO condivisi (RicercaPosizione, StatoPratica, etc.)
│       ├── Enum\                             → Ruoli, TipoAppartenenzaRuolo
│       └── Service References\              → Proxy WCF generati per PN812, PN813, PN815, PN818
│
├── PN812\                                    WCF Wrapper
│   ├── WSInpsPensioniLiquidazione\           → .svc, Contracts (DataContracts + ServiceContracts)
│   ├── WSInpsPensioniLiquidazione.BL\        → Logica di business
│   ├── WSInpsPensioniLiquidazione.BLCommon\  → Logica condivisa (Familiari, ANF, etc.)
│   ├── WSInpsPensioniLiquidazione.Data\      → Accesso DB specifico
│   └── WSInpsPensioniLiquidazione.DataCommon → Accesso DB condiviso
│
├── PN813\                                    WCF Fondo Speciale (FS)
│   ├── WSInpsPensioniLiquidazioneFS\         → .svc, Contracts
│   ├── WSInpsPensioniLiquidazioneFS.BL\      → GestioneCalcolo, GestioneContrib, DatiFondo, etc.
│   └── WSInpsPensioniLiquidazioneFS.Data\    → SQL per FS
│
├── PN815\                                    WCF AGO
│   ├── WSInpsPensioniLiquidazioneAgo\        → .svc, Contracts
│   ├── WSInpsPensioniLiquidazioneAgo.BL\     → Gestione*, SIN, NACI, TotalIvs, etc.
│   └── WSInpsPensioniLiquidazioneAgo.Data\   → SQL per AGO
│
└── PN818\                                    WCF CI
    ├── WSInpsPensioniLiquidazioneCi\         → .svc, Contracts
    ├── WSInpsPensioniLiquidazioneCi.BL\      → Gestione* CI-specifici
    └── WSInpsPensioniLiquidazioneCi.Data\    → SQL per CI
```

---

## 14. Glossario

| Termine | Significato |
|---|---|
| **NDomus** / **NumeroDomanda** | Numero identificativo della domanda di pensione |
| **ProgStorico** | Progressivo storico della domanda |
| **FS / GDP** | Fondo Speciale / Gestione Dipendenti Pubblici |
| **AGO** | Assicurazione Generale Obbligatoria |
| **CI** | Convenzioni Internazionali |
| **Calcolo** | Il processo che determina l'importo lordo della pensione |
| **Liquidazione** | Prima erogazione della pensione |
| **Prelievo** | Acquisizione della domanda dall'archivio host per la lavorazione |
| **Sprenotazione** | Rilascio della domanda senza completare la lavorazione |
| **Certificato** | Numero del certificato di pensione generato dopo il calcolo |
| **Chiave Pensione** | Identificativo univoco della pensione liquidata |
| **FELPE** | Sistema di controllo importi esistenti |
| **WebDom** | Sistema web di ricezione domande (interfaccia cittadino) |
| **ARCA** | Archivio Centralizzato Anagrafica INPS |
| **ANF** | Assegno per il Nucleo Familiare |
| **SIN** | Sistema Informativo Nazionale |
| **NACI** | Nucleo Anagrafico Contribuenti INPS |
| **Pro-Rata** | Calcolo quota pensione proporzionale ai periodi in Italia vs estero |
| **Bititolarità** | Situazione in cui una stessa persona è titolare di due pensioni |
| **Dante Causa** | Il defunto da cui deriva il diritto alla pensione di reversibilità |
| **Avente Diritto** | Chi ha diritto alla pensione (es. figlio orfano) |
| **Quadro** | Sezione di dati della pratica di pensione (es. "Quadro Titolare") |
| **Esito OK/KO** | Risposta WCF: OK = operazione riuscita, KO = errore con messaggio |

---

*Documento generato da analisi del codice sorgente – Marzo 2026*

---

## 15. Stato della Reingegnerizzazione

**Aggiornamento:** 20/03/2026

### 15.1 Componenti legacy già reingegnerizzati

#### v1 — Microservizio Ricerca + WebApp base (completato 09/03/2026)

La funzionalità `PresenterVisualizzaStatoPratiche` (PN809/PN812) che espone la ricerca domande è stata
reingegnerizzata come:

- **Microservizio REST** `IVS.Pensioni.Search` (endpoint `POST /api/v1/pensioni/search`)
- **WebApp Razor Pages** `IVS.Pensioni.WebApp` con pagina `/Ricerca` e layout Bootstrap 5

#### v2 — Gestione Titolare CRUD (completato 20/03/2026)

I User Control del quadro Titolare (precedentemente in `PN812`/`PN813`) sono stati reingegnerizzati
come pagina Razor Pages con tre tab, ciascuno corrispondente a un UC legacy:

| Componente legacy | Progetto | → Nuovo componente |
|---|---|---|
| `UCAnagrafica.ascx` | PN812/PN813 | Tab **Anagrafica** — `/Pratiche/{nDomus}/Titolare` |
| `UCStatoCivile.ascx` | PN812/PN813 | Tab **Stato Civile** — griglia inline-editable |
| `UCResidenzeEstere.ascx` | PN812/PN813 | Tab **Residenze Estero** — griglia inline-editable |

Tutti e tre i tab delegano le operazioni CRUD al microservizio `IVS.Pensioni.Search` tramite
gli 11 nuovi endpoint del `TitolareController`.

### 15.2 Mapping completo legacy → nuovo

| Funzionalità legacy | Presenter/UC legacy | Stato reingegnerizzazione |
|---|---|---|
| Ricerca / Visualizza Stato Pratiche | `PresenterVisualizzaStatoPratiche` | ✅ Completato — v1 |
| Anagrafica titolare | `UCAnagrafica.ascx` | ✅ Completato — v2 |
| Stato civile titolare | `UCStatoCivile.ascx` | ✅ Completato — v2 |
| Residenze estero titolare | `UCResidenzeEstere.ascx` | ✅ Completato — v2 |
| Ricostituzioni | `UCAnagraficaRIC.ascx` | ⏳ Non ancora implementato |
| Gateway ARCA | `GestioneAnagrafica` (proxy ARCA) | ⏳ Non ancora implementato |
| Gateway WebDom | `WebDom` integration | ⏳ Non ancora implementato |
| Autenticazione reale | `INPS.DNA` Windows Auth | ⏳ Non ancora implementato |
| Calcolo/Liquidazione | tutti i BL di PN813/815/818 | 🔴 Fuori scope attuale |

### 15.3 Cosa manca ancora

Le seguenti funzionalità legacy non sono ancora state reingegnerizzate e rimangono in scope
per versioni future:

1. **`UCAnagraficaRIC.ascx` — Ricostituzioni:** gestione delle variazioni dell'anagrafica
   a seguito di ricostituzioni della pensione. Richiede tabelle aggiuntive e logica di business
   specifica.

2. **Gateway ARCA:** il sistema legacy legge i dati anagrafici dall'Archivio Centralizzato
   ARCA tramite `GestioneAnagrafica`. Nella v2 l'anagrafica è letta/scritta direttamente su DB;
   l'integrazione con ARCA (per sincronizzazione o lettura autoritativa) rimane da implementare.
   Vedere `Requisiti_Microservizi_Riepilogo_ARCA_WebDom_Gateway.md`.

3. **Gateway WebDom:** integrazione con il sistema di ricezione domande WebDom per il prelievo
   automatico delle pratiche. Vedere `Requisiti_Microservizi_Riepilogo_ARCA_WebDom_Gateway.md`.

4. **Autenticazione reale:** l'`OperatoreContext` (ruolo, sede, matricola) è attualmente
   hardcoded/mock nella WebApp. Va collegato al sistema IAM INPS (sostituto di `INPS.DNA`).

5. **Calcolo e Liquidazione:** l'intera logica di calcolo della pensione (PN813/815/818 BL)
   è fuori scope dell'attuale progetto di reingegnerizzazione.
