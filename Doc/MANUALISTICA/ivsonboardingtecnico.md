# IVS Legacy - Onboarding Tecnico Nuova Risorsa

## 1. Obiettivo
Questo documento guida una nuova risorsa nell'ingresso operativo sul progetto IVS Legacy, con focus su architettura, flussi applicativi, componenti chiave e modalità di lavoro.

## 2. Panorama rapido (che cosa è IVS)
- Sistema legacy INPS per liquidazione pensioni.
- Dominio articolato su tre fondi: **FS**, **AGO**, **CI**.
- Architettura distribuita in 5 moduli principali:
  - `PN809` (front-end operatore);
  - `PN812` (orchestratore centrale);
  - `PN813`, `PN815`, `PN818` (servizi specializzati per fondo).

## 3. Mappa repository

| Percorso | Ruolo |
|---|---|
| `Doc/` | documentazione analisi tecnica e manuale utente |
| `PN809/` | Web UI (WebForms/MVP) |
| `PN812/` | servizio core orchestrazione, BL, DataCommon, test |
| `PN813/` | servizio fondo FS |
| `PN815/` | servizio fondo AGO |
| `PN818/` | servizio fondo CI |

## 4. Architettura essenziale

## 4.1 PN809 (UI)
- WebForms con Presenter e interfacce IView.
- Gestisce:
  - accesso operatore;
  - scelta ruolo/sede;
  - ricerca pratica;
  - stato pratiche;
  - accessi da canali esterni (WebDom, Scriwo, ecc.).

## 4.2 PN812 (orchestratore)
- Servizio WCF centrale.
- Smista chiamate ai moduli fondo in base al tipo pensione.
- Coordina aggiornamento quadri/stato pratica, integrazioni esterne, logging tecnico.

## 4.3 PN813/PN815/PN818 (servizi fondo)
- Ogni modulo implementa logica specifica di prelievo, salvataggio e calcolo.
- Pattern comune: Service -> BL -> Data/host adapters.
- Gestione old/new tracciato in base a controlli dinamici e finestre temporali.

## 5. Flussi da conoscere subito

## 5.1 Ricerca e apertura pratica
1. PN809 raccoglie criteri ricerca.
2. PN812 recupera riepilogo/stato.
3. PN809 mostra pratiche e abilita azioni.

## 5.2 Prelievo domanda
1. Da PN809 parte la richiesta.
2. PN812 instrada al modulo fondo.
3. Modulo fondo integra host/sistemi esterni e normalizza dati.

## 5.3 Calcolo verify/definitivo
1. Operatore invia calcolo.
2. PN812 valida semafori/quadri.
3. Modulo fondo esegue calcolo.
4. Esito ritornato e stato pratica aggiornato.
5. In definitivo: pratica non più modificabile.

## 5.4 Stato pratiche e funzioni admin
1. Ricerca avanzata stato pratiche.
2. Apertura/eliminazione/sblocco/cambio stato secondo ruolo.

## 6. Prerequisiti tecnici per lavorare
- Conoscenza C# legacy (.NET 3.5), WCF, WebForms.
- Comprensione pattern MVP e DAO/LINQ-to-SQL.
- Accesso agli ambienti e configurazioni per endpoint interni.
- Utenze IDM con ruoli applicativi coerenti.

## 7. Ordine di studio consigliato (prima settimana)

1. **Documentazione**:
   - `Doc/Legacy_IVS_AnalisiTecnica.md`
   - `Doc/Manuale_utente.md`
2. **UI**:
   - `PN809/LiquidazionePensioniFS/ElaborazionePosizione.aspx.cs`
   - `PN809/LiquidazionePensioniFS/VisualizzazioneStatoPratiche.aspx.cs`
3. **Orchestratore**:
   - `PN812/WSInpsPensioniLiquidazione/ServizioLiquidazione.svc.cs`
   - `PN812/WSInpsPensioniLiquidazione.BL/GestioneCalcoloDomanda.cs`
4. **Servizi fondo**:
   - `PN813/.../ServizioLiquidazioneFs.svc.cs`
   - `PN815/.../ServizioLiquidazioneAgo.svc.cs`
   - `PN818/.../ServizioLiquidazioneCi.svc.cs`
5. **Data access e quadri**:
   - `PN812/.../DataCommon/DAGestionePensione.cs`
   - `PN812/.../DataCommon/DAGestioneQuadri.cs`

## 8. Convenzioni pratiche del progetto
- `AreaEsito` è il contratto base di esito (OK/KO + messaggio).
- Molti flussi sono governati da appSettings/controlli dinamici.
- Le configurazioni ambientali sono replicate in file `CHG*`.
- Forte uso di `TransactionScope` nei salvataggi.

## 9. Come fare troubleshooting efficace

## 9.1 Dove guardare per primi
- UI issue: `PN809` code-behind + presenter.
- Flusso business: `PN812` service + classi `Gestione*`.
- Errore dominio fondo: `PN813/PN815/PN818` (service + BL + host adapter).

## 9.2 Sequenza diagnostica consigliata
1. Conferma contesto utente (ruolo/sede/abilitazioni).
2. Verifica semafori/quadri obbligatori.
3. Traccia endpoint chiamato e payload.
4. Verifica mapping verso servizio fondo.
5. Isola anomalia in host integration o persistence.
6. Correlazione log applicativo + log SOAP.

## 10. Rischi tecnici da conoscere
- Presenza di componenti monolitici con molte responsabilità.
- Duplicazioni old/new tracciato.
- Configurazioni legacy con possibile esposizione dati sensibili.
- Contratti WCF molto estesi (impatto alto in caso di modifica).

## 11. Best practice per contribuire senza regressioni

1. Limitare le modifiche al modulo strettamente necessario.
2. Preservare compatibilità dei DataContract esistenti.
3. Evitare side effect su configurazioni ambientali.
4. Aggiornare logica in modo coerente tra orchestratore e modulo fondo.
5. Testare almeno i flussi: ricerca, prelievo, calcolo verify, calcolo definitivo, stato pratica.

## 12. Piano onboarding operativo (30-60-90, senza date)

### Fase 1
- Setup ambiente.
- Lettura documentazione e mappa moduli.
- Shadowing su incident e change minori.

### Fase 2
- Presa in carico di bug a bassa complessità su PN809/PN812.
- Analisi guidata di un flusso completo fino a modulo fondo.

### Fase 3
- Ownership di una capability verticale (es. prelievo o stato pratiche).
- Proposta di miglioramento tecnico (refactoring o hardening sicurezza).

## 13. Glossario minimo
- **NDomus**: identificativo domanda.
- **Quadri**: sezioni dati della pratica con stato di completezza.
- **Semaforo rosso**: blocco funzionale che impedisce avanzamento.
- **Verify**: calcolo di verifica.
- **Definitivo**: calcolo finale con blocco modifiche.
- **TE08**: output di stampa post-calcolo.

## 14. Checklist di ingresso pronta all'uso
- Accesso repository e documentazione.
- Accesso tecnico ai moduli PN809/PN812/PN813/PN815/PN818.
- Conoscenza percorso end-to-end di almeno 1 caso d'uso.
- Capacità di localizzare errore tra UI/orchestratore/modulo fondo.
- Capacità di eseguire e verificare un fix con impatto controllato.