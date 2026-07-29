# Corso di Formazione — Nuova Risorsa Tecnica IVS Legacy

> **Progetto:** IVS Legacy (Liquidazione Pensioni INPS)  
> **Fonte:** IVS Onboarding Tecnico  
> **Durata stimata:** 5 giorni (prima settimana operativa)  
> **Destinatario:** Sviluppatore/Analista tecnico in ingresso

---

## MODULO 1 — Contesto e Panoramica del Sistema
**Durata:** ~2 ore

### Obiettivi di apprendimento
Al termine di questo modulo il partecipante sarà in grado di:
- Descrivere lo scopo del sistema IVS e il suo dominio pensionistico
- Identificare i tre fondi gestiti (FS, AGO, CI)
- Elencare i 5 moduli principali e il loro ruolo

### Contenuti

#### 1.1 Cos'è IVS
IVS è il **sistema legacy INPS per la liquidazione delle pensioni**. Gestisce l'intero ciclo di vita delle domande pensionistiche, dalla ricerca e apertura della pratica fino al calcolo definitivo e alla stampa.

Il sistema è articolato su tre fondi pensionistici:
| Fondo | Descrizione |
|-------|-------------|
| **FS** | Fondo Speciale |
| **AGO** | Assicurazione Generale Obbligatoria |
| **CI** | Cassa Integrazione (fondo speciale) |

#### 1.2 I 5 Moduli Principali

| Modulo | Tipo | Responsabilità |
|--------|------|----------------|
| `PN809` | Front-end | UI operatore (WebForms/MVP) |
| `PN812` | Back-end core | Orchestratore centrale (WCF) |
| `PN813` | Back-end fondo | Servizio specializzato fondo FS |
| `PN815` | Back-end fondo | Servizio specializzato fondo AGO |
| `PN818` | Back-end fondo | Servizio specializzato fondo CI |

#### 1.3 Mappa del Repository
```
Doc/        → Documentazione analisi tecnica e manuale utente
PN809/      → Web UI
PN812/      → Orchestratore + BL + DataCommon + test
PN813/      → Servizio fondo FS
PN815/      → Servizio fondo AGO
PN818/      → Servizio fondo CI
```

### Attività pratiche
- [ ] Aprire il repository e navigare la struttura delle cartelle
- [ ] Leggere `Doc/Legacy_IVS_AnalisiTecnica.md`
- [ ] Identificare visivamente dove si trovano i file principali di ogni modulo

---

## MODULO 2 — Architettura Tecnica
**Durata:** ~3 ore

### Obiettivi di apprendimento
- Comprendere il pattern MVP usato in PN809
- Descrivere il ruolo di orchestrazione di PN812
- Capire il pattern comune Service → BL → Data nei moduli fondo

### Contenuti

#### 2.1 PN809 — Front-end (UI)
- Tecnologia: **ASP.NET WebForms** con pattern **MVP** (Model-View-Presenter)
- Gestisce:
  - Accesso e autenticazione operatore
  - Selezione ruolo e sede
  - Ricerca pratica pensionistica
  - Visualizzazione stato pratiche
  - Accessi da canali esterni (WebDom, Scriwo, ecc.)
- File chiave da studiare:
  - `ElaborazionePosizione.aspx.cs`
  - `VisualizzazioneStatoPratiche.aspx.cs`

#### 2.2 PN812 — Orchestratore Centrale
- Tecnologia: **WCF** (Windows Communication Foundation)
- Smista le chiamate ai moduli fondo in base al tipo di pensione
- Coordina:
  - Aggiornamento quadri e stato pratica
  - Integrazioni con sistemi esterni INPS
  - Logging tecnico delle operazioni
- File chiave da studiare:
  - `ServizioLiquidazione.svc.cs`
  - `GestioneCalcoloDomanda.cs`
  - `DAGestionePensione.cs`
  - `DAGestioneQuadri.cs`

#### 2.3 PN813 / PN815 / PN818 — Servizi Fondo
- **Pattern comune:** `Service → BL → Data/Host Adapters`
- Ogni modulo implementa la logica specifica di: prelievo, salvataggio e calcolo
- Gestiscono il tracciato old/new tramite controlli dinamici e finestre temporali
- File chiave:
  - `ServizioLiquidazioneFs.svc.cs` (PN813)
  - `ServizioLiquidazioneAgo.svc.cs` (PN815)
  - `ServizioLiquidazioneCi.svc.cs` (PN818)

### Diagramma concettuale
```
┌─────────────────────────────────────────────────────┐
│                   OPERATORE (PN809)                  │
│         WebForms + MVP + Presenter + IView           │
└──────────────────────┬──────────────────────────────┘
                       │ chiamate WCF
┌──────────────────────▼──────────────────────────────┐
│              ORCHESTRATORE (PN812)                   │
│         WCF Core + BL + DataCommon + Logging         │
└──────┬───────────────┬───────────────┬──────────────┘
       │               │               │
┌──────▼──────┐ ┌──────▼──────┐ ┌─────▼───────┐
│ PN813 (FS)  │ │ PN815 (AGO) │ │ PN818 (CI)  │
│ Svc+BL+Data │ │ Svc+BL+Data │ │ Svc+BL+Data │
└─────────────┘ └─────────────┘ └─────────────┘
```

### Attività pratiche
- [ ] Aprire e leggere `ServizioLiquidazione.svc.cs` identificando i principali metodi esposti
- [ ] Tracciare il percorso di una chiamata da PN809 a PN812 a uno dei moduli fondo
- [ ] Identificare il pattern MVP in almeno un form di PN809

---

## MODULO 3 — Flussi Applicativi Chiave
**Durata:** ~4 ore

### Obiettivi di apprendimento
- Tracciare end-to-end i 4 flussi principali del sistema
- Distinguere le responsabilità di ogni modulo in ciascun flusso
- Comprendere il significato funzionale di verify vs definitivo

### Contenuti

#### 3.1 Flusso: Ricerca e Apertura Pratica
```
1. PN809 → raccoglie i criteri di ricerca dall'operatore
2. PN812 → recupera riepilogo e stato della pratica
3. PN809 → visualizza le pratiche e abilita le azioni disponibili
```
**Concetti chiave:** NDomus, ruolo operatore, abilitazioni per sede

#### 3.2 Flusso: Prelievo Domanda
```
1. PN809 → invia la richiesta di prelievo
2. PN812 → instrada al modulo fondo corretto (FS/AGO/CI)
3. Modulo fondo → integra host/sistemi esterni e normalizza i dati
```
**Concetti chiave:** routing per tipo pensione, host integration, normalizzazione tracciato

#### 3.3 Flusso: Calcolo Verify e Definitivo
```
1. Operatore invia il calcolo
2. PN812 → valida semafori e quadri obbligatori
3. Modulo fondo → esegue il calcolo
4. Esito ritornato → stato pratica aggiornato
5. In DEFINITIVO: pratica non più modificabile (+ generazione TE08)
```
**Concetti chiave:** semaforo rosso, quadri, verify vs definitivo, TE08

#### 3.4 Flusso: Stato Pratiche e Funzioni Admin
```
1. Ricerca avanzata per stato pratiche
2. Azioni disponibili in base al ruolo:
   - Apertura / Eliminazione / Sblocco / Cambio stato
```
**Concetti chiave:** ruoli IDM, abilitazioni applicative

### Glossario dei flussi
| Termine | Significato |
|---------|-------------|
| **NDomus** | Identificativo univoco della domanda pensionistica |
| **Quadri** | Sezioni dati della pratica con stato di completezza |
| **Semaforo rosso** | Blocco funzionale che impedisce l'avanzamento del flusso |
| **Verify** | Calcolo di verifica (non definitivo, modificabile) |
| **Definitivo** | Calcolo finale — blocca la pratica a ulteriori modifiche |
| **TE08** | Output di stampa generato dopo il calcolo definitivo |

### Attività pratiche
- [ ] Seguire un flusso di prelievo nel codice dalla UI al modulo fondo, annotando ogni passaggio
- [ ] Identificare dove vengono validati i semafori in PN812
- [ ] Localizzare la logica del calcolo definitivo in uno dei moduli fondo

---

## MODULO 4 — Prerequisiti Tecnici e Convenzioni
**Durata:** ~2 ore

### Obiettivi di apprendimento
- Conoscere lo stack tecnologico richiesto
- Comprendere le convenzioni di configurazione e codice del progetto
- Essere operativo sull'ambiente di sviluppo

### Contenuti

#### 4.1 Stack Tecnologico
| Tecnologia | Utilizzo |
|------------|---------|
| **C# / .NET 3.5** | Linguaggio e framework principale (legacy) |
| **WCF** | Communication layer tra PN812 e i moduli fondo |
| **ASP.NET WebForms** | UI in PN809 |
| **MVP Pattern** | Separazione logica UI in PN809 |
| **DAO / LINQ-to-SQL** | Data access layer |
| **TransactionScope** | Gestione transazioni nei salvataggi |

#### 4.2 Convenzioni del Progetto
- **`AreaEsito`**: contratto base di esito usato in tutto il sistema (OK/KO + messaggio)
- **AppSettings / Controlli dinamici**: molti flussi sono governati da flag di configurazione
- **File `CHG*`**: replicano configurazioni ambientali per ogni deployment
- **TransactionScope**: usato estensivamente nei salvataggi — da rispettare nelle modifiche
- **DataContract WCF**: contratti molto estesi, modificarli ha alto impatto — massima cautela

#### 4.3 Setup Ambiente
Accessi necessari prima di iniziare:
- [ ] Accesso al repository (tutti i moduli PN809/PN812/PN813/PN815/PN818)
- [ ] Utenza IDM con ruoli applicativi corretti
- [ ] Accesso agli endpoint degli ambienti (dev, collaudo)
- [ ] Configurazioni locali per connessione ai servizi interni

### Attività pratiche
- [ ] Configurare l'ambiente locale e verificare la compilazione di almeno PN809 e PN812
- [ ] Localizzare un `AreaEsito` nel codice e tracciarne l'uso
- [ ] Identificare un file `CHG*` e capirne la struttura

---

## MODULO 5 — Troubleshooting e Best Practice
**Durata:** ~2 ore

### Obiettivi di apprendimento
- Applicare la sequenza diagnostica su un errore reale
- Conoscere i rischi tecnici del sistema e come mitigarli
- Contribuire in modo sicuro evitando regressioni

### Contenuti

#### 5.1 Dove Guardare per Primo
| Tipo di problema | Dove cercare |
|-----------------|--------------|
| Problema UI | `PN809` code-behind + presenter |
| Errore flusso business | `PN812` service + classi `Gestione*` |
| Anomalia dominio fondo | `PN813/PN815/PN818` service + BL + host adapter |

#### 5.2 Sequenza Diagnostica
1. Conferma contesto utente (ruolo, sede, abilitazioni)
2. Verifica semafori e quadri obbligatori
3. Traccia endpoint chiamato e payload
4. Verifica mapping verso il servizio fondo corretto
5. Isola l'anomalia in host integration o persistence
6. Correla log applicativo + log SOAP

#### 5.3 Rischi Tecnici da Conoscere
| Rischio | Descrizione |
|---------|-------------|
| **Monoliti con molte responsabilità** | Componenti difficili da modificare in isolamento |
| **Duplicazioni old/new tracciato** | Logica duplicata che può causare divergenze |
| **Configurazioni legacy con dati sensibili** | Verificare sempre prima di condividere file CHG* |
| **Contratti WCF estesi** | Impatto alto in caso di modifica — testare con cura |

#### 5.4 Best Practice per Contribuire Senza Regressioni
1. **Limitare** le modifiche al modulo strettamente necessario
2. **Preservare** la compatibilità dei DataContract esistenti
3. **Evitare** side effect su configurazioni ambientali
4. **Aggiornare** la logica in modo coerente tra orchestratore e modulo fondo
5. **Testare sempre** almeno i flussi: ricerca, prelievo, calcolo verify, calcolo definitivo, stato pratica

### Attività pratiche
- [ ] Eseguire una simulazione di troubleshooting su un log fornito dal team
- [ ] Effettuare una piccola modifica guidata su PN809 verificando che non rompa il flusso di ricerca

---

## PIANO ONBOARDING OPERATIVO (30-60-90 giorni)

### Fase 1 — Primi 30 giorni (Setup e Comprensione)
| Obiettivo | Azione |
|-----------|--------|
| Ambiente operativo | Setup completo con accesso a tutti i moduli |
| Base documentale | Lettura documentazione + mappa moduli |
| Osservazione guidata | Shadowing su incident e change minori |
| Completamento corso | Superamento dei 5 moduli del corso |

### Fase 2 — Giorni 31-60 (Operatività Guidata)
| Obiettivo | Azione |
|-----------|--------|
| Contribuzione base | Presa in carico di bug a bassa complessità su PN809/PN812 |
| Analisi approfondita | Analisi guidata di un flusso completo fino al modulo fondo |
| Autonomia diagnostica | Troubleshooting autonomo su almeno 2 incident |

### Fase 3 — Giorni 61-90 (Ownership e Miglioramento)
| Obiettivo | Azione |
|-----------|--------|
| Ownership verticale | Responsabilità su una capability (es. prelievo o stato pratiche) |
| Proposta miglioramento | Proposta tecnica di refactoring o hardening sicurezza |
| Mentoring | Supporto a nuove risorse che entrano nel team |

---

## CHECKLIST FINALE DI INGRESSO

Prima di considerarsi operativo, verificare di aver completato:

- [ ] Accesso al repository e alla documentazione
- [ ] Accesso tecnico ai moduli PN809/PN812/PN813/PN815/PN818
- [ ] Lettura di `Doc/Legacy_IVS_AnalisiTecnica.md` e `Doc/Manuale_utente.md`
- [ ] Comprensione del percorso end-to-end di almeno 1 caso d'uso
- [ ] Capacità di localizzare un errore tra UI/orchestratore/modulo fondo
- [ ] Capacità di eseguire e verificare un fix con impatto controllato
- [ ] Conoscenza del glossario minimo (NDomus, Quadri, Semaforo, Verify, Definitivo, TE08)

---

## QUESTIONARIO DI VERIFICA APPRENDIMENTO
### 5 Domande a Risposta Multipla

---

**Domanda 1**
*Quale modulo del sistema IVS ha il ruolo di orchestratore centrale, smistando le chiamate ai moduli fondo in base al tipo di pensione?*

- A) PN809
- **B) PN812** ✅
- C) PN813
- D) PN818

> **Spiegazione:** PN812 è il servizio WCF centrale che coordina tutte le operazioni verso i moduli fondo (PN813/PN815/PN818) in base al tipo di pensione richiesta.

---

**Domanda 2**
*Nel sistema IVS, cosa rappresenta un "semaforo rosso"?*

- A) Un log di errore critico nel file di trace applicativo
- B) Un flag di configurazione nei file CHG* che disabilita una funzionalità
- **C) Un blocco funzionale che impedisce l'avanzamento del flusso** ✅
- D) Un errore di connessione al servizio WCF del modulo fondo

> **Spiegazione:** Il semaforo rosso è una condizione di blocco funzionale che il sistema valuta in PN812 prima di procedere con il calcolo, impedendo l'avanzamento se i quadri non sono correttamente compilati.

---

**Domanda 3**
*Qual è la differenza principale tra il calcolo "Verify" e il calcolo "Definitivo"?*

- A) Il Verify usa dati reali, il Definitivo usa dati simulati
- B) Il Verify è eseguito da PN813, il Definitivo da PN812
- **C) Il Definitivo rende la pratica non più modificabile e genera il TE08** ✅
- D) Il Verify richiede un ruolo amministratore, il Definitivo no

> **Spiegazione:** Dopo il calcolo Definitivo la pratica viene bloccata a ulteriori modifiche e viene generato il documento di stampa TE08. Il Verify è invece un calcolo di verifica intermedio ancora modificabile.

---

**Domanda 4**
*Qual è il pattern architetturale adottato in PN809 per separare la logica di presentazione dalla UI?*

- A) MVC (Model-View-Controller)
- **B) MVP (Model-View-Presenter)** ✅
- C) MVVM (Model-View-ViewModel)
- D) Repository Pattern

> **Spiegazione:** PN809 utilizza il pattern MVP con interfacce IView e classi Presenter, tipico delle applicazioni ASP.NET WebForms dell'epoca. Questo permette una separazione (parziale) tra logica e UI.

---

**Domanda 5**
*Se durante il troubleshooting si individua un'anomalia nel calcolo specifico del fondo AGO, in quale modulo si deve cercare la causa principale?*

- A) PN809, nel code-behind del form di calcolo
- B) PN812, nella classe `GestioneCalcoloDomanda.cs`
- **C) PN815, nel service, BL e host adapter specifici del fondo AGO** ✅
- D) Nei file CHG* di configurazione ambientale

> **Spiegazione:** Ogni fondo ha il proprio modulo dedicato: AGO → PN815, FS → PN813, CI → PN818. Le anomalie di calcolo specifiche del dominio fondo vanno cercate nel rispettivo modulo (Service → BL → Host Adapter).

