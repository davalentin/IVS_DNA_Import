# Product Requirements Document
## IVS Modernizzazione — Liquidazione Pensioni INPS
**Versione:** 1.0 | **Data:** 2026-06-24 | **Fase:** CONCEPT | **Progetto:** IVS_DNA

---

# 1. Project Overview

## Contesto Organizzativo

L'INPS (Istituto Nazionale della Previdenza Sociale) gestisce attraverso il sistema IVS_DNA il processo di liquidazione iniziale delle pensioni per operatori interni di sede. Il sistema presidia tre fondi previdenziali distinti — Fondo Speciale (FS), Assicurazione Generale Obbligatoria (AGO) e Convenzioni Internazionali (CI) — garantendo la continuità operativa di un processo istituzionale ad alta criticità che coinvolge operatori di sede, responsabili di processo e figure amministrative.

## Sfide Attuali

Il sistema IVS_DNA è costruito su una piattaforma tecnologica profondamente obsoleta: .NET Framework 3.5, ASP.NET WebForms e WCF SOAP, tecnologie ormai prossime o già oltre i limiti di supporto ufficiale. L'analisi di reverse engineering evidenzia un debito tecnico significativo: un contratto di servizio con 252 operazioni centralizzate in un unico componente (God Contract), 1.805 accessi a sessione server-side, 3.344 utilizzi di ViewState, credenziali in chiaro nei file di configurazione, e 150 TODO/FIXME espliciti nel codice. Il sistema comprende 630.434 linee di codice hand-written distribuite su cinque moduli principali (PN809, PN812, PN813, PN815, PN818), con dipendenze da oltre dieci sistemi enterprise interni (ARCA, WebDom, SAI, ANF, FELPE, SCRIWO, DB2). La portabilità è valutata molto bassa, rendendo qualsiasi evoluzione costosa, rischiosa e rallentata dall'inerzia tecnica accumulata.

## Obiettivo Strategico

L'iniziativa di modernizzazione nasce dalla necessità strategica di eliminare la dipendenza da tecnologie legacy non più supportate, ridurre l'inerzia tecnica che rallenta ogni evoluzione del sistema e allineare la piattaforma agli standard moderni di sviluppo e sicurezza adottati da INPS. La trasformazione deve avvenire in modo incrementale, secondo un approccio Strangler Fig che consenta la sostituzione progressiva dei moduli legacy preservando la continuità operativa del processo pensionistico, senza compromettere le funzionalità esistenti né interrompere il servizio agli operatori di sede.

## Risultati di Business Attesi

La modernizzazione del sistema IVS_DNA produrrà un insieme misurabile di benefici organizzativi: una riduzione significativa del costo di manutenzione ordinaria ed evolutiva, grazie all'adozione di stack tecnologici con ampia disponibilità di competenze sul mercato; il miglioramento della sicurezza e della conformità normativa attraverso l'eliminazione di secret in chiaro e l'adozione di pratiche DevSecOps; l'abilitazione della scalabilità orizzontale, oggi preclusa dall'architettura stateful; il miglioramento dell'esperienza operativa per gli utenti INPS, con interfacce più moderne e processi più guidati; e infine la riduzione del rischio operativo derivante dall'obsolescenza tecnologica, proteggendo la continuità di un processo previdenziale di rilevanza pubblica.

---

# 2. Business Requirements

| ID     | Nome                                      | Descrizione |
| ------ | ----------------------------------------- | ----------- |
| BR-001 | Continuità Operativa durante Trasformazione | Il processo di liquidazione pensionistica deve rimanere operativo e affidabile per tutti gli operatori INPS durante l'intero arco temporale della modernizzazione. Nessuna funzionalità esistente può essere rimossa o degradata senza un piano di sostituzione approvato. Il rischio di interruzione del servizio deve essere minimizzato attraverso un approccio di migrazione incrementale. |
| BR-002 | Riduzione del Debito Tecnico Strutturale  | L'organizzazione deve ottenere una riduzione misurabile del debito tecnico nei moduli critici (God Contract, Session/ViewState, configuration leakage). L'obiettivo è eliminare i principali antipattern che aumentano il costo del cambiamento e il rischio di regressione in ogni intervento evolutivo. |
| BR-003 | Miglioramento della Manutenibilità        | Il sistema modernizzato deve essere gestibile da team di sviluppo con competenze standard di mercato (.NET moderno), riducendo la dipendenza da specialisti legacy sempre più difficili da reperire. Il tempo medio di onboarding tecnico di un nuovo sviluppatore deve diminuire rispetto allo stato attuale. |
| BR-004 | Conformità Sicurezza e Normativa          | Il sistema deve essere allineato agli standard di sicurezza INPS e ai requisiti normativi vigenti (GDPR per il trattamento di dati previdenziali personali), con eliminazione di credenziali in chiaro, adozione di vault segreti, e implementazione di audit trail verificabile per tutte le operazioni sensibili. |
| BR-005 | Scalabilità e Sostenibilità Operativa     | L'architettura del sistema modernizzato deve supportare la scalabilità orizzontale per rispondere a variazioni del carico operativo, superando i limiti dell'architettura stateful server-side attuale che rende impossibile l'aggiunta di istanze senza impatto sulla sessione utente. |
| BR-006 | Preservazione Integrale delle Capacità Funzionali | Tutte le funzionalità attualmente erogate dal sistema IVS_DNA — ricerca pratiche, compilazione quadri, calcolo verify e definitivo, stampa, integrazione con sistemi enterprise, gestione amministrativa — devono essere disponibili nel sistema modernizzato, senza alcuna perdita di capacità operativa per gli utenti di sede. |
| BR-007 | Osservabilità e Governabilità del Processo | Il sistema modernizzato deve fornire strumenti adeguati per monitorare lo stato del processo operativo, diagnosticare rapidamente eventuali anomalie e misurare le performance, colmando il gap attuale in cui le metriche di produzione sono assenti (TBD) e il triage richiede competenze specialistiche distribuite tra più moduli. |

---

# 3. Functional Requirements

## 3.1 Autenticazione e Gestione del Contesto Operatore
> *Traces to: BR-006, BR-004*

- Il sistema deve autenticare gli utenti tramite meccanismo di identità federata INPS, sostituendo l'autenticazione legacy attuale.
- Il sistema deve consentire la selezione del ruolo operativo e della sede di lavoro prima dell'accesso alle funzionalità applicative.
- Il sistema deve applicare controlli di autorizzazione basati su ruolo (operatore di sede, amministratore applicativo, direttore/capo processo) per ogni azione sensibile.
- Il sistema deve supportare la gestione multi-ruolo e multi-sede per operatori con profili compositi.
- Il sistema deve tracciare tutte le azioni degli utenti con audit trail immutabile, registrando operatore, ruolo, sede, azione, data e ora.
- Il sistema deve terminare la sessione utente dopo un periodo configurabile di inattività.

## 3.2 Ricerca e Acquisizione Pratica
> *Traces to: BR-006, BR-003*

- Il sistema deve permettere la ricerca di pratiche pensionistiche per NDomus (numero domanda), codice fiscale, e dati anagrafici (nome, cognome, data di nascita).
- Il sistema deve gestire il caso di più domande associate allo stesso soggetto, presentando una lista disambiguata.
- Il sistema deve applicare controlli di sede e ruolo prima di consentire la presa in carico di una pratica.
- Il sistema deve esporre lo stato corrente di ciascuna pratica trovata, includendo indicazioni su eventuali blocchi o lavorazioni in corso.
- Il sistema deve supportare il prelievo/prenotazione della domanda, integrando i sistemi upstream (WebDom, ARCA) per normalizzare i dati prima della lavorazione.
- Il sistema deve gestire i sinonimi anagrafici e le domande collegate con relativo segnalamento all'operatore.

## 3.3 Compilazione e Gestione Quadri Applicativi
> *Traces to: BR-006, BR-003, BR-007*

- Il sistema deve presentare e consentire la compilazione dei quadri: titolare, residenze estere, stato civile, dante causa, aventi diritto, domande collegate, liquidazione pensione, dati contributivi, dati fondo, maggiorazioni e benefici, redditi, detrazioni, pagamento, oneri, supplementi, delegato/tutore, periodi, bitolarità.
- Il sistema deve salvare in modo persistente e transazionale i dati di ciascun quadro, garantendo la coerenza tra quadri correlati.
- Il sistema deve applicare i controlli dinamici di validazione configurabili (ControlliDinamici), visualizzando messaggi di esito chiari (OK/KO con descrizione).
- Il sistema deve supportare varianti di quadri specifiche per fondo (FS, AGO, CI), applicando la logica differenziata appropriata.
- Il sistema deve gestire i semafori di stato per guidare l'operatore attraverso la sequenza obbligatoria di compilazione.
- Il sistema deve consentire la modifica e il salvataggio parziale dei quadri, con ripristino del contesto al rientro nella pratica.

## 3.4 Calcolo Pensione (Verify e Definitivo)
> *Traces to: BR-006, BR-004, BR-007*

- Il sistema deve eseguire il calcolo di verifica (verify) non definitivo, restituendo stato pensione, certificato ed eventuali warning senza consolidare lo stato della pratica.
- Il sistema deve eseguire il calcolo definitivo con consolidamento dello stato, aggiornando tutti i sistemi downstream coinvolti (WebDom, FELPE, SAI, INPDAP, CI05, Oneri, piani di pagamento).
- Il sistema deve effettuare il routing del calcolo verso il motore fondo appropriato (FS/AGO/CI) in base al tipo di pratica.
- Il sistema deve restituire all'operatore lo stato del calcolo con visualizzazione dei semafori di esito e degli eventuali errori bloccanti.
- Il sistema deve gestire i flag ANF e gli aggiornamenti preliminari richiesti prima del calcolo per ciascun fondo.
- Il sistema deve produrre chiave pensione e transactionId tracciabili per ogni calcolo eseguito.

## 3.5 Post-Calcolo, Stampa e Produzione Output
> *Traces to: BR-006, BR-007*

- Il sistema deve generare la stampa finale della pratica in formato PDF, interfacciandosi con StampeWeb/certificati.
- Il sistema deve aggiornare SCRIWO con i documenti e i contenuti correlati alla pratica conclusa.
- Il sistema deve registrare tutti gli aggiornamenti post-calcolo verso i sistemi downstream con traccia degli esiti.
- Il sistema deve supportare la ristampa e la consultazione degli output prodotti per pratiche già liquidate.

## 3.6 Amministrazione, Utility e Gestione Eccezioni
> *Traces to: BR-006, BR-004*

- Il sistema deve consentire agli utenti con ruolo amministrativo lo sblocco di pratiche bloccate, incluso lo sblocco per cancellazione.
- Il sistema deve supportare la riassegnazione di pratiche tra operatori e sedi, con traccia dell'azione.
- Il sistema deve consentire il cambio di stato di una domanda, con le verifiche di autorizzazione appropriate.
- Il sistema deve gestire la configurazione delle liquidazioni abilitate, tipologie non abilitate e trasformazioni abilitate.
- Il sistema deve consentire il bypass dei controlli dinamici per casistiche gestite amministrativamente, con registrazione obbligatoria del motivo.
- Il sistema deve supportare le funzioni di pulizia domanda e di commutazione tra lavorazione manuale e automatica.

## 3.7 Integrazione con Sistemi Enterprise INPS
> *Traces to: BR-006, BR-001*

- Il sistema deve integrare WebDom per acquisire domande, metadati di richiesta e aggiornare lo stato post-calcolo.
- Il sistema deve integrare ARCA per ottenere e allineare dati anagrafici dei soggetti.
- Il sistema deve integrare i sistemi host legacy (DB2/Oneri, INPDAP, GP4) per accesso a dati specialistici di fondo.
- Il sistema deve integrare SAI, ANF, FELPE, Redditi, Uffici Pagatori, CI05, Total IVS, NACI, SIN per gli aggiornamenti post-calcolo.
- Il sistema deve mantenere la compatibilità contrattuale con i consumer esistenti durante la fase di transizione, attraverso adapter o API gateway di compatibilità.
- Il sistema deve esporre le nuove API con contratto esplicito e versionato, indipendentemente dai contratti legacy WCF.

---

# 4. Technical Constraints

## 4.1 Stack Target e Runtime

- **Target Runtime:** Il sistema modernizzato deve essere sviluppato in .NET 8 LTS o superiore; il .NET Framework 3.5 deve essere abbandonato al completamento della migrazione di ogni modulo.
- **Frontend Target:** L'interfaccia utente deve essere sviluppata con tecnologie web moderne (Blazor Server, Blazor WebAssembly, o React); ASP.NET WebForms deve essere dismesso.
- **Backend Target:** I servizi devono esporre API RESTful con contratti OpenAPI; WCF SOAP può essere mantenuto temporaneamente solo come adapter di compatibilità per consumer legacy non ancora migrati.
- **ORM e Data Access:** LINQ-to-SQL deve essere sostituito con Entity Framework Core 8+; le stored procedure critiche possono essere mantenute tramite raw SQL.

## 4.2 Approccio di Migrazione

- **Strangler Fig obbligatorio:** La migrazione deve avvenire in modo incrementale per capability, senza big-bang replacement; il sistema legacy deve rimanere operativo per le capability non ancora migrate (per organizational policy di continuità operativa).
- **Parallel Run:** Le capability critiche (calcolo definitivo, stampa) devono prevedere un periodo di parallel run con confronto degli output tra sistema legacy e moderno.
- **Backward Compatibility Transitoria:** I DataContract `Area*` e l'interfaccia `IServizioLiquidazione` devono essere mantenuti come adapter durante la transizione, fino alla dismissione programmata degli ultimi consumer legacy.

## 4.3 Infrastruttura e Hosting

- **Ambiente INPS:** Il sistema deve essere deployato nell'ecosistema infrastrutturale INPS (on-premise o hybrid INPS cloud); deployment su cloud pubblico esterno non è consentito (vincolo di data sovereignty istituzionale).
- **IIS Compatibility:** Il deployment su IIS deve essere supportato per la durata della transizione; Kestrel standalone o container Docker sono ammessi per le nuove capability sul layer target.
- **Ambienti multipli:** Devono essere gestiti ambienti separati (TEST, COLL, ESERCIZIO) con configurazioni isolate e pipeline di promozione automatizzata.

## 4.4 Integrazione e Interoperabilità

- **Sistemi enterprise da preservare:** L'integrazione con WebDom, ARCA, SAI, ANF, FELPE, INPDAP, SCRIWO, DB2/Host è obbligatoria; i contratti di integrazione devono essere mantenuti o adattati senza interrompere i sistemi upstream/downstream.
- **DB2/OLEDB:** L'integrazione con host legacy DB2 tramite DB2OLEDB deve essere mantenuta almeno nella fase intermedia, finché non esiste un piano di migrazione approvato per i dati host.
- **Framework INPS.DNA:** La dipendenza dal framework istituzionale `INPS.DNA` deve essere valutata per ogni modulo; dove possibile, le funzionalità devono essere implementate su equivalenti standard moderni.

## 4.5 Dati e Compliance

- **SQL Server:** Il database relazionale SQL Server deve essere mantenuto come sistema di persistenza principale; il passaggio a un altro RDBMS è fuori scope.
- **GDPR:** Il sistema tratta dati previdenziali personali sensibili; devono essere rispettati i requisiti GDPR e le policy INPS per il trattamento, la retention e la protezione dei dati (dettagli operativi da validare con il DPO INPS).
- **Audit e Retention Log:** Tutte le operazioni sensibili devono essere registrate con retention minima da definire con il responsabile della conformità INPS.

## 4.6 Sicurezza e Accesso

- **Secret Management:** Nessuna credenziale, connection string o API key deve essere presente in chiaro nei file di configurazione o nel VCS; è obbligatorio l'utilizzo di un secret vault (es. Azure Key Vault o equivalente INPS).
- **Autenticazione:** Il sistema deve adottare il meccanismo di identità federata INPS esistente; l'autenticazione proprietaria legacy deve essere sostituita.
- **Debug in produzione:** La configurazione `debug=true` nei file `Web.config` di produzione deve essere rimossa come pre-condizione alla messa in produzione.
- **Crittografia:** `connectionStringCryptography` deve essere abilitata; tutte le comunicazioni inter-servizio devono essere cifrate (TLS 1.2+).

## 4.7 Sviluppo e Delivery

- **Linguaggio:** C# come linguaggio primario; TypeScript per le componenti frontend SPA se applicabile.
- **Pipeline CI/CD:** Deve essere implementata una pipeline CI/CD con step di build, test, analisi statica e promozione per ambiente; l'assenza di pipeline è da considerarsi un gap bloccante da colmare nella fase di hardening.
- **Test Automation:** La suite di test automatizzati (unit, integration, smoke) deve raggiungere una copertura adeguata sui flussi critici prima della dismissione di ogni modulo legacy.

---

# 5. Non-Functional Requirements

## 5.1 Performance e Tempi di Risposta
> *Traces to: BR-005, BR-006*

| ID      | Requisito                    | Target / Condizione                                                                    | Si Applica Quando                                    |
| ------- | ---------------------------- | -------------------------------------------------------------------------------------- | ---------------------------------------------------- |
| NFR-001 | Tempo di risposta ricerca    | La ricerca pratica deve restituire risultati entro 3 secondi nel 95° percentile        | Operatore esegue ricerca per NDomus o CF in orario operativo |
| NFR-002 | Tempo di risposta calcolo verify | Il calcolo verify deve completarsi entro 10 secondi nel 95° percentile             | Operatore avvia verify su pratica compilata          |
| NFR-003 | Tempo di risposta definitivo | Il calcolo definitivo deve completarsi entro 30 secondi nel 95° percentile             | Operatore avvia calcolo definitivo                   |
| NFR-004 | Caricamento pagine UI        | Il caricamento delle pagine principali (quadri, lista pratiche) non deve superare 2 secondi | Utente naviga nell'applicazione modernizzata         |
| NFR-005 | Payload API                  | Le response delle API RESTful non devono superare 5 MB per chiamata standard [TO BE REFINED] | Chiamate API standard da frontend modernizzato       |

## 5.2 Disponibilità e Affidabilità
> *Traces to: BR-001, BR-006*

| ID      | Requisito                          | Target / Condizione                                                                  | Si Applica Quando                                  |
| ------- | ---------------------------------- | ------------------------------------------------------------------------------------ | -------------------------------------------------- |
| NFR-006 | Disponibilità in orario operativo  | Il sistema deve essere disponibile almeno il 99,5% del tempo in orario operativo INPS (lun-ven 8:00-18:00) [TO BE REFINED] | Orario operativo delle sedi INPS                   |
| NFR-007 | Recovery Time Objective (RTO)      | In caso di interruzione, il ripristino del servizio deve avvenire entro 4 ore [TO BE REFINED] | Qualsiasi interruzione non pianificata             |
| NFR-008 | Recovery Point Objective (RPO)     | La perdita massima di dati accettabile è di 1 ora [TO BE REFINED]                    | Scenari di failure con perdita dati                |
| NFR-009 | Gestione errori integrazioni       | Il sistema deve degradare in modo controllato quando un sistema integrato non è disponibile, segnalando l'anomalia senza bloccare le funzionalità non dipendenti | Indisponibilità parziale di sistemi upstream/downstream |

## 5.3 Sicurezza
> *Traces to: BR-004, BR-001*

| ID      | Requisito                         | Target / Condizione                                                                    | Si Applica Quando                                     |
| ------- | --------------------------------- | -------------------------------------------------------------------------------------- | ----------------------------------------------------- |
| NFR-010 | Autenticazione forte               | Tutti gli accessi al sistema devono essere autenticati tramite il meccanismo di identità INPS; nessun accesso anonimo è consentito | Sempre, su tutti gli ambienti di produzione           |
| NFR-011 | Autorizzazione granulare          | Ogni operazione sensibile deve essere autorizzata in base al ruolo e alla sede dell'utente; tentativi di accesso non autorizzato devono generare un evento di audit | Ogni operazione su pratica pensionistica              |
| NFR-012 | Crittografia in transito          | Tutte le comunicazioni inter-servizio e tra client e server devono avvenire su TLS 1.2 o superiore | Tutte le comunicazioni di produzione                  |
| NFR-013 | Assenza di secret in codice       | Nessuna credenziale, token o connection string deve essere presente nei file di configurazione nel VCS o negli artefatti deployati | In ogni ambiente (test, collaudo, produzione)         |
| NFR-014 | Audit trail immutabile            | Tutte le operazioni di calcolo definitivo, cambio stato, sblocco, eliminazione devono essere registrate con traccia non modificabile (utente, ruolo, sede, timestamp, payload sintetico) | Per ogni operazione amministrativa e di calcolo definitivo |
| NFR-015 | Scadenza sessione                 | Le sessioni utente devono scadere dopo 30 minuti di inattività [TO BE REFINED]         | Utenti autenticati inattivi                           |

## 5.4 Usabilità e Accessibilità
> *Traces to: BR-003, BR-006*

| ID      | Requisito                         | Target / Condizione                                                                   | Si Applica Quando                                     |
| ------- | --------------------------------- | ------------------------------------------------------------------------------------- | ----------------------------------------------------- |
| NFR-016 | Accessibilità WCAG                | L'interfaccia utente modernizzata deve rispettare le linee guida WCAG 2.1 livello AA, in conformità con le normative PA italiane (Legge Stanca, AgID) | Su tutta l'interfaccia utente modernizzata           |
| NFR-017 | Compatibilità browser             | Il sistema deve funzionare correttamente sui browser moderni (Chrome, Firefox, Edge nelle ultime 2 versioni major); il requisito di compatibilità IE deve essere rimosso | Utilizzo da parte di operatori INPS                  |
| NFR-018 | Completamento task senza training | Un operatore già esperto del dominio pensionistico deve poter completare il ciclo principale (ricerca → quadri → calcolo definitivo) nel sistema modernizzato senza training aggiuntivo, grazie alla conservazione del modello mentale esistente | Periodo di transizione / rollout iniziale            |

## 5.5 Manutenibilità e Operabilità
> *Traces to: BR-002, BR-003, BR-007*

| ID      | Requisito                              | Target / Condizione                                                                  | Si Applica Quando                                    |
| ------- | -------------------------------------- | ------------------------------------------------------------------------------------ | ---------------------------------------------------- |
| NFR-019 | Osservabilità                          | Il sistema deve emettere log strutturati con correlation-id per ogni request; metriche tecniche (latenza, error rate, throughput) devono essere disponibili su dashboard operativa | In produzione e collaudo                             |
| NFR-020 | Onboarding sviluppatori               | Un nuovo sviluppatore .NET mid-level deve poter completare il setup dell'ambiente di sviluppo e la prima modifica in meno di 2 giorni lavorativi [TO BE REFINED] | Onboarding nuovi membri del team                    |
| NFR-021 | Copertura test automatizzati          | I flussi critici (ricerca, calcolo verify, calcolo definitivo) devono essere coperti da test automatizzati (unit + integration) prima della dismissione del corrispondente modulo legacy | Prima di ogni dismissione di modulo legacy           |
| NFR-022 | Configurazione per ambiente           | Tutte le configurazioni ambiente-specifiche devono essere iniettabili senza modifiche al codice (environment variables, configuration service) | In tutti gli ambienti                               |

## 5.6 Scalabilità
> *Traces to: BR-005*

| ID      | Requisito                         | Target / Condizione                                                                   | Si Applica Quando                                     |
| ------- | --------------------------------- | ------------------------------------------------------------------------------------- | ----------------------------------------------------- |
| NFR-023 | Scalabilità orizzontale           | I nuovi moduli REST devono supportare l'esecuzione in più istanze parallele senza dipendenza da stato server-side (stateless API) | Picchi di carico operativo o necessità di scaling     |
| NFR-024 | Carico concorrente                | Il sistema deve supportare almeno 200 utenti concorrenti attivi senza degradazione delle performance [TO BE REFINED] | Orario di punta delle sedi INPS                      |

## 5.7 Portabilità e Modernizzazione
> *Traces to: BR-002, BR-003*

| ID      | Requisito                         | Target / Condizione                                                                   | Si Applica Quando                                     |
| ------- | --------------------------------- | ------------------------------------------------------------------------------------- | ----------------------------------------------------- |
| NFR-025 | Eliminazione dipendenza .NET 3.5  | Al completamento della migrazione di ogni modulo, nessun componente del sistema modernizzato deve dipendere da .NET Framework 3.5 o WCF SOAP nativo | Per ogni modulo al termine della relativa fase di migrazione |
| NFR-026 | Build cross-platform              | Il sistema modernizzato deve essere buildabile su ambienti Linux/Windows con toolchain standard .NET 8+, eliminando la dipendenza da `Microsoft.WebApplication.targets` legacy | In CI/CD e ambienti di sviluppo                      |

---

# 6. Customer Experience Requirements

## 6.1 Tipologie Utenti e Contesto d'Uso

- Gli utenti primari del sistema sono operatori di sede INPS che utilizzano l'applicazione in ambiente intranet aziendale, su postazioni desktop, durante la giornata lavorativa per lavorare pratiche pensionistiche end-to-end.
- Gli utenti amministrativi (amministratori applicativi, direttori/capi processo) devono poter accedere alle funzioni di gestione eccezioni, sblocchi e riassegnazioni da interfacce dedicate e separate dal flusso principale.
- Il sistema deve distinguere visivamente e navigativamente i profili utente, mostrando solo le funzioni per cui l'utente è autorizzato in base a ruolo e sede selezionati.
- Gli utenti del supporto tecnico applicativo devono poter accedere a strumenti di diagnostica e log senza necessità di accesso diretto ai server applicativi.

## 6.2 Device e Canale di Accesso

- Il sistema deve essere accessibile tramite browser web moderno su postazione desktop (Windows) in rete intranet INPS; l'accesso mobile non è un requisito primario ma non deve essere attivamente impedito.
- Il sistema deve funzionare correttamente con Chrome, Firefox ed Edge nelle versioni correnti; il requisito di compatibilità Internet Explorer 9 deve essere rimosso nel sistema modernizzato.
- Il sistema deve operare esclusivamente all'interno della rete intranet INPS; l'accesso da reti esterne non è in scope.

## 6.3 Azioni Utente Primarie

- Gli operatori devono poter ricercare e aprire una pratica pensionistica in meno di 3 interazioni dall'homepage.
- Gli operatori devono poter navigare tra i quadri applicativi di una pratica senza perdere il contesto della pratica in lavorazione.
- Gli operatori devono poter avviare il calcolo verify e il calcolo definitivo con un'azione singola, ricevendo l'esito in modo chiaro entro il timeout di risposta atteso.
- Gli amministrativi devono poter completare operazioni di sblocco e riassegnazione in meno di 5 passi dalla funzione dedicata.

## 6.4 Visibilità delle Informazioni

- L'interfaccia deve mostrare in modo persistente e visibile il contesto dell'operatore corrente (ruolo, sede, matricola) durante l'intera sessione di lavoro.
- Lo stato della pratica in lavorazione (fase corrente, semafori di validazione, eventuali blocchi) deve essere sempre visibile nella schermata di lavoro dei quadri.
- I messaggi di esito (OK/KO) del calcolo e dei controlli dinamici devono essere espressi in linguaggio comprensibile all'operatore, con indicazione chiara dell'azione correttiva quando necessario.
- La homepage deve mostrare avvisi operativi, messaggi di sistema e informazioni sulla versione dell'applicazione.

## 6.5 Notifiche e Feedback di Sistema

- Il sistema deve notificare all'operatore quando una pratica che sta cercando di acquisire è già in lavorazione da un altro operatore, indicando chi la detiene.
- Il sistema deve fornire feedback visivo immediato (spinner, progress indicator) durante le operazioni di calcolo che richiedono elaborazione server-side.
- Il sistema deve avvisare l'operatore in caso di timeout o errore di integrazione con un sistema esterno, distinguendo tra errori bloccanti e warning non bloccanti.
- Il sistema deve segnalare le condizioni di warning (semafori arancioni) in modo distinguibile dagli errori bloccanti (semafori rossi) e dagli stati di successo (semafori verdi).

## 6.6 Accessibilità e Standard PA

- L'interfaccia utente modernizzata deve rispettare i requisiti di accessibilità WCAG 2.1 AA come previsto dalla normativa italiana per le PA (Legge Stanca, Circolare AgID).
- Il sistema deve supportare la navigazione da tastiera per tutti i flussi operativi primari (ricerca, compilazione quadri, calcolo), senza dipendenza esclusiva da dispositivi di puntamento.
- I colori e i contrasti dell'interfaccia devono rispettare i requisiti minimi WCAG per utenti con deficit visivi.
- Tutti i messaggi di errore e le etichette dei campi devono essere comprensibili senza dipendere esclusivamente dalla codifica cromatica.

## 6.7 Lingua e Tono

- L'interfaccia deve essere in lingua italiana, utilizzando la terminologia del dominio pensionistico INPS (NDomus, quadri, fondo FS/AGO/CI, verify, definitivo) consolidata nel sistema legacy e familiare agli operatori.
- I messaggi di sistema devono essere espressi in linguaggio operativo chiaro, evitando jargon tecnico non comprensibile all'operatore di sede.
