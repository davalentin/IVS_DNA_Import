# wbs

## wbs_summary

| metrica | valore |
|---|---|
| total_phases | 10 |
| total_deliverables | 16 |
| total_work_packages | 42 |
| total_activities | 128 |
| high_priority_phases | 6 |
| high_priority_deliverables | 10 |

## wbs_table

| wbs_id | level | phase_name / deliverable_name / wp_name / activity_name | item_type | priority | predecessor_id | source_refs | activity_type | notes |
|---|---|---|---|---|---|---|---|---|
| F1 | 1 | Fondazione e Setup Piattaforma | phase | Alta | — | FR-025; FR-011; FR-001; FR-004; FR-003; FEAT-003; comp-001; comp-002; comp-003; comp-012 | — | Crea la baseline .NET 8, CI/CD, sicurezza e persistenza necessaria a tutte le capability successive. |
| F1.D1 | 2 | Infrastruttura di Sviluppo e CI/CD | deliverable | Alta | — | FR-025; FR-011; comp-001; comp-002; comp-003; comp-012; comp-005; comp-006; comp-011; proc-008 | — | Imposta solution, pipeline e baseline SQL Server/EF Core per l’intero programma di modernizzazione. |
| F1.D1.WP1 | 3 | Bootstrap ambiente sviluppo locale | work_package | Alta | — | FR-025; comp-001; comp-002; comp-003; comp-012; proc-008 | — | Bootstrap tecnico iniziale della piattaforma target e dei suoi canali principali. |
| F1.D1.WP1.A1 | 4 | Inizializzare solution .NET 8 multi-progetto con struttura per moduli PN809/PN812/PN813/PN815/PN818 | activity | Alta | — | FR-025; comp-001; comp-002 | Architettura/Setup | Ossatura condivisa per frontend, BFF e servizi di dominio. |
| F1.D1.WP1.A2 | 4 | Predisporre docker-compose di sviluppo con SQL Server, gateway di integrazione e stub enterprise | activity | Alta | F1.D1.WP1.A1 | comp-012; FR-025 | Architettura/Setup | Ambiente locale ripetibile per sviluppo e debug end-to-end. |
| F1.D1.WP1.A3 | 4 | Configurare profili ambiente, secret placeholder e bootstrap locale della soluzione | activity | Alta | F1.D1.WP1.A2 | comp-003; comp-002; FR-025 | DevOps | Parametri esternalizzati e onboarding rapido del team moderno .NET 8. |
| F1.D1.WP2 | 3 | Setup CI/CD pipeline | work_package | Alta | F1.D1.WP1 | FR-025; comp-002; comp-012; proc-008 | — | Rende eseguibili build, test e deploy frequenti della nuova architettura service-based. |
| F1.D1.WP2.A1 | 4 | Configurare pipeline build multi-stage per solution .NET 8, quality gate e packaging container | activity | Alta | — | FR-025; comp-002 | DevOps | Build ripetibile per tutti i servizi modernizzati. |
| F1.D1.WP2.A2 | 4 | Integrare esecuzione automatica di test unitari, integration test e contract test in pipeline | activity | Alta | F1.D1.WP2.A1 | FR-025; comp-012 | DevOps | Quality gate minimo prima di ogni promozione ambiente. |
| F1.D1.WP2.A3 | 4 | Automatizzare deploy verso ambienti di sviluppo e collaudo con promozione parametrica | activity | Alta | F1.D1.WP2.A2 | comp-012; FR-025 | DevOps | Riduce errori manuali nella delivery incrementale Strangler Fig. |
| F1.D1.WP3 | 3 | Setup Database baseline | work_package | Alta | F1.D1.WP1 | FR-011; comp-005; comp-006; comp-011; proc-003; proc-005; proc-008; UC-007 | — | Prepara la persistenza condivisa di transizione e le fondamenta EF Core del target. |
| F1.D1.WP3.A1 | 4 | Definire schema iniziale SQL Server e convenzioni di ownership logica per bounded context | activity | Alta | — | FR-011; comp-005 | Architettura/Setup | Baseline coerente con strategia shared-to-bounded definita in design. |
| F1.D1.WP3.A2 | 4 | Configurare migrazioni EF Core 8 e seed tecnico per ruoli, sedi, configurazioni e dataset iniziali | activity | Alta | F1.D1.WP3.A1 | FR-011; comp-011 | Backend | Abilita bootstrap dati tecnico-amministrativi senza script manuali. |
| F1.D1.WP3.A3 | 4 | Creare DbContext, mapping e policy di transazione per servizi pratica, quadri e calcolo | activity | Alta | F1.D1.WP3.A2 | FR-011; comp-005; comp-006 | Backend | Punto comune per persistenza coerente e auditabile. |
| F1.D2 | 2 | Hardening sicurezza e bonifica configurazione | deliverable | Alta | F1.D1 | FR-001; FR-004; FR-003; FEAT-003; comp-003; comp-011; proc-001; proc-008 | — | Rimuove credenziali hardcoded e introduce baseline di sicurezza, audit e policy operative cross-cutting. |
| F1.D2.WP1 | 3 | Secret vault e rimozione hardcoded credentials | work_package | Alta | F1.D1.WP1 | FR-001; FR-004; comp-003; comp-011; proc-001; proc-008; UC-001; UC-014 | — | Allinea la modernizzazione ai vincoli INPS di secret management e separazione configurazione/codice. |
| F1.D2.WP1.A1 | 4 | Centralizzare connection string, certificati e credenziali tecniche in vault istituzionale | activity | Alta | — | FR-001; comp-003 | Architettura/Setup | Elimina secret in chiaro da repository e pacchetti di deploy. |
| F1.D2.WP1.A2 | 4 | Refattorizzare bootstrap servizi e job tecnici per leggere configurazione solo da provider sicuri | activity | Alta | F1.D2.WP1.A1 | FR-004; comp-011 | Backend | Bonifica configurazione in vista di ambienti multipli e audit compliance. |
| F1.D2.WP2 | 3 | Security baseline (TLS, audit trail framework) | work_package | Alta | F1.D2.WP1 | FR-003; FR-004; FEAT-003; comp-003; comp-011; proc-001; proc-008; UC-001; UC-014 | — | Stabilisce la baseline di sicurezza applicativa e tracciatura comune a tutte le capability. |
| F1.D2.WP2.A1 | 4 | Configurare enforcement TLS, header di sicurezza e policy di sessione sulla superficie edge | activity | Alta | — | FR-003; comp-003 | Architettura/Setup | Baseline uniforme per comunicazioni client, API e integrazioni moderne. |
| F1.D2.WP2.A2 | 4 | Introdurre framework condiviso di audit trail, correlation-id e gestione timeout di sessione | activity | Alta | F1.D2.WP2.A1 | FR-004; FEAT-003; comp-011 | Backend | Pattern riusabile da autenticazione, calcolo e funzioni amministrative. |
| F2 | 1 | Autenticazione e Contesto Operatore | phase | Alta | F1 | FR-001; FR-003; FR-004; FR-002; FEAT-001; FEAT-003; FEAT-002; comp-002; comp-003; comp-011 | — | Modernizza accesso federato, contesto ruolo-sede e audit iniziale degli operatori INPS. |
| F2.D1 | 2 | Identity e RBAC modernizzati | deliverable | Alta | F1.D2 | FR-001; FR-003; FR-004; FR-002; FEAT-001; FEAT-003; FEAT-002; comp-002; comp-003; comp-011 | — | Realizza OIDC federato, RBAC e UI di contesto per tutti i ruoli operativi e di supporto. |
| F2.D1.WP1 | 3 | Integrazione INPS Federation OIDC | work_package | Alta | F1.D2.WP1 | FR-001; FEAT-001; comp-002; comp-003; proc-001; UC-001; SCN-001; SCN-002 | — | Porta il sistema dal login proprietario legacy a identità federata INPS. |
| F2.D1.WP1.A1 | 4 | Configurare client OIDC/OAuth2, metadata federation e trust verso Identity Provider INPS | activity | Alta | — | FR-001; comp-003 | Architettura/Setup | Allinea il nuovo accesso al meccanismo federato istituzionale. |
| F2.D1.WP1.A2 | 4 | Implementare callback, validazione token e mapping claims applicative verso contesto utente | activity | Alta | F2.D1.WP1.A1 | FR-001; FEAT-001; comp-003 | Backend | Trasforma identità federata in sessione applicativa utilizzabile dal BFF. |
| F2.D1.WP1.A3 | 4 | Verificare login, logout e casi di errore federation con suite automatizzata di autenticazione | activity | Alta | F2.D1.WP1.A2 | SCN-001; SCN-002; UC-001 | Test | Riduce il rischio di regressioni sul canale di accesso principale. |
| F2.D1.WP2 | 3 | RBAC middleware e audit trail | work_package | Alta | F2.D1.WP1 | FR-003; FR-004; FEAT-003; comp-002; comp-003; comp-011; proc-001; proc-008; UC-001; UC-014; SCN-001; SCN-002 | — | Consolida autorizzazione, audit e gestione sessione come capability comune riusabile. |
| F2.D1.WP2.A1 | 4 | Definire matrice ruolo-sede, policy autorizzative e punti di enforcement applicativo | activity | Alta | — | FR-003; comp-003 | Architettura/Setup | Esplicita le regole autorizzative oggi implicite nel legacy. |
| F2.D1.WP2.A2 | 4 | Implementare middleware RBAC, audit accessi negati e timeout sessione configurabile | activity | Alta | F2.D1.WP2.A1 | FR-003; FR-004; comp-011 | Backend | Applica controlli uniformi a BFF, servizi e console amministrative. |
| F2.D1.WP2.A3 | 4 | Eseguire test su accessi non autorizzati, audit immutabile e scadenza sessione | activity | Alta | F2.D1.WP2.A2 | SCN-001; SCN-002; UC-014 | Test | Copre i percorsi più sensibili di sicurezza e conformità. |
| F2.D1.WP3 | 3 | UI contesto operatore (ruolo/sede) | work_package | Alta | F2.D1.WP2 | FR-002; FEAT-002; comp-001; comp-002; comp-003; proc-001; UC-001; SCN-001; SCN-002 | — | Completa lato frontend la capability di autenticazione e contesto operativo. |
| F2.D1.WP3.A1 | 4 | Implementare schermata di selezione ruolo-sede e messaggi di contesto coerenti con il dominio INPS | activity | Alta | — | FR-002; comp-001 | Frontend | Rende esplicita la scelta del contesto prima dell’uso dell’applicazione. |
| F2.D1.WP3.A2 | 4 | Validare in UI attivazione contesto, accesso negato e persistenza del profilo corrente | activity | Alta | F2.D1.WP3.A1 | SCN-001; SCN-002; UC-001 | Test | Assicura usabilità operativa e comportamento coerente sui profili multi-sede. |
| F3 | 1 | Gestione Pratiche e Ricerca | phase | Alta | F2 | FR-005; FR-006; FR-008; FR-023; FR-007; FEAT-004; FEAT-005; FEAT-006; comp-002; comp-004 | — | Copre ricerca multi-criterio, disambiguazione, acquisizione, prenotazione e relativo front-end operativo. |
| F3.D1 | 2 | API Ricerca Pratiche | deliverable | Alta | F2.D1 | FR-005; FR-006; FR-008; FR-023; FR-007; FEAT-004; FEAT-005; FEAT-006; comp-002; comp-004 | — | Realizza servizi REST per ricerca, acquisizione e presa in carico della pratica pensionistica. |
| F3.D1.WP1 | 3 | REST API ricerca (NDomus, CF, anagrafica) | work_package | Alta | F2.D1.WP2 | FR-005; FEAT-004; comp-002; comp-004; proc-002; UC-002; SCN-003 | — | Costruisce l’entry point operativo per individuare la pratica da lavorare. |
| F3.D1.WP1.A1 | 4 | Definire contratti REST di ricerca, filtri, ordinamenti e paginazione coerenti con il processo attuale | activity | Alta | — | FR-005; comp-004 | Architettura/Setup | Esplicita il contratto target della capability ricerca. |
| F3.D1.WP1.A2 | 4 | Implementare endpoint di ricerca per NDomus, codice fiscale e dati anagrafici composti | activity | Alta | F3.D1.WP1.A1 | FR-005; FEAT-004; comp-004 | Backend | Ricostruisce il comportamento core della funzione di ricerca pratica. |
| F3.D1.WP1.A3 | 4 | Ottimizzare query, criteri di validazione input e payload di risposta per il BFF | activity | Alta | F3.D1.WP1.A2 | FR-005; comp-002; comp-004 | Backend | Rispetta i target prestazionali della ricerca operativa. |
| F3.D1.WP1.A4 | 4 | Eseguire test automatici su criteri validi, risultati univoci e casi di ricerca vuota | activity | Alta | F3.D1.WP1.A3 | SCN-003; UC-002 | Test | Stabilizza comportamento e performance della capability di ingresso al processo. |
| F3.D1.WP2 | 3 | Integrazione WebDom e ARCA per acquisizione | work_package | Alta | F3.D1.WP1 | FR-006; FR-008; FR-023; FEAT-005; FEAT-006; comp-004; comp-012; proc-002; UC-003; SCN-003 | — | Trasforma fonti enterprise eterogenee in input consistente per la lavorazione pratica. |
| F3.D1.WP2.A1 | 4 | Definire adapter e mapping canonico per metadati domanda, correlazioni e allineamento anagrafico | activity | Alta | — | FR-023; comp-012 | Integrazione | Isola i contratti enterprise dal core della capability pratiche. |
| F3.D1.WP2.A2 | 4 | Implementare normalizzazione dati, correlazioni anagrafiche e arricchimento pratica dai sistemi upstream | activity | Alta | F3.D1.WP2.A1 | FR-006; FR-008; FEAT-005; comp-004 | Backend | Produce un dataset coerente prima dell’apertura quadri. |
| F3.D1.WP2.A3 | 4 | Validare timeout, warning e fallback controllati delle integrazioni WebDom/ARCA | activity | Alta | F3.D1.WP2.A2 | SCN-003; UC-003 | Test | Riduce il rischio di fault esterni bloccanti in acquisizione. |
| F3.D1.WP3 | 3 | Presa in carico e prenotazione pratica | work_package | Alta | F3.D1.WP2 | FR-007; FR-008; FEAT-006; comp-003; comp-004; comp-012; proc-002; UC-003; SCN-003; SCN-004 | — | Gestisce l’handoff sicuro da ricerca a ownership esplicita della pratica. |
| F3.D1.WP3.A1 | 4 | Implementare verifiche di ruolo, sede, stato pratica e lock logico di lavorazione | activity | Alta | — | FR-007; comp-003; comp-004 | Backend | Replica i vincoli di ownership operativa presenti nel processo pensionistico. |
| F3.D1.WP3.A2 | 4 | Esportare API di prenotazione, conflitto di presa in carico e apertura dataset normalizzato | activity | Alta | F3.D1.WP3.A1 | FR-008; FEAT-006; comp-004 | Backend | Rende esplicita la transizione tra ricerca e lavorazione. |
| F3.D1.WP3.A3 | 4 | Testare concorrenza, conflitti di acquisizione e audit dell’evento pratica prenotata | activity | Alta | F3.D1.WP3.A2 | SCN-003; SCN-004; UC-003 | Test | Copre il principale edge case di coordinamento tra operatori di sede. |
| F3.D2 | 2 | Frontend SPA ricerca e acquisizione | deliverable | Alta | F3.D1 | FR-005; FR-006; FR-007; FR-008; FEAT-004; FEAT-005; FEAT-006; comp-001; comp-002; comp-004 | — | Realizza maschere operative per ricerca, disambiguazione, acquisizione e stato di lavorazione. |
| F3.D2.WP1 | 3 | UI ricerca pratica | work_package | Alta | F3.D1.WP1 | FR-005; FR-006; FEAT-004; FEAT-005; comp-001; comp-002; comp-004; proc-002; UC-002; UC-003; SCN-003; SCN-004 | — | Traduzione UX della capability di ricerca con attenzione a disambiguazione e velocità operativa. |
| F3.D2.WP1.A1 | 4 | Progettare form di ricerca, lista risultati e segnali di correlazione anagrafica | activity | Alta | — | FR-005; FR-006; comp-001 | UI/UX e design | Conserva il modello mentale degli operatori di sede sul lookup pratica. |
| F3.D2.WP1.A2 | 4 | Implementare maschera di ricerca con risultati disambiguati, filtri e stati di caricamento | activity | Alta | F3.D2.WP1.A1 | FEAT-004; FEAT-005; comp-001; comp-002 | Frontend | Collega il portale operativo alle nuove API di ricerca. |
| F3.D2.WP1.A3 | 4 | Verificare accessibilità, empty state e messaggi di conflitto o non trovato | activity | Alta | F3.D2.WP1.A2 | SCN-003; SCN-004 | Test | Rende robusta la UX sugli esiti principali e sui casi ambigui. |
| F3.D2.WP2 | 3 | UI presa in carico e stato pratica | work_package | Alta | F3.D2.WP1 | FR-007; FR-008; FEAT-006; comp-001; comp-002; comp-004; proc-002; UC-003; SCN-003; SCN-004 | — | Completa il journey di acquisizione mostrando ownership e blocchi prima dell’apertura pratica. |
| F3.D2.WP2.A1 | 4 | Implementare pulsanti di acquisizione, indicatori di ownership e dettaglio stato pratica | activity | Alta | — | FR-007; FEAT-006; comp-001 | Frontend | Rende immediato il passaggio da risultato a lavorazione. |
| F3.D2.WP2.A2 | 4 | Validare end-to-end presa in carico, blocchi di concorrenza e apertura della pratica | activity | Alta | F3.D2.WP2.A1 | SCN-003; SCN-004; UC-003 | Test | Conferma comportamento coerente tra UI, BFF e backend di prenotazione. |
| F4 | 1 | Quadri Applicativi | phase | Alta | F3 | FR-009; FR-011; FR-010; FR-012; FR-015; FEAT-007; FEAT-009; FEAT-008; FEAT-010; FEAT-013 | — | Preserva e modernizza i quadri anagrafici e tecnico-economici, inclusi controlli dinamici e varianti di fondo. |
| F4.D1 | 2 | API Quadri | deliverable | Alta | F3.D1 | FR-009; FR-011; FR-010; FR-012; FR-015; FEAT-007; FEAT-009; FEAT-008; FEAT-010; FEAT-013 | — | Realizza il backend dei quadri, la persistenza transazionale e il motore di controlli/semafori. |
| F4.D1.WP1 | 3 | CRUD quadri anagrafici | work_package | Alta | F3.D1.WP3 | FR-009; FR-011; FEAT-007; FEAT-009; comp-005; proc-003; UC-004; UC-007; SCN-005; SCN-006 | — | Modernizza il primo blocco di quadri indispensabile per l’avvio della compilazione pratica. |
| F4.D1.WP1.A1 | 4 | Definire modello dominio, DTO e contratti CRUD per titolare, familiari e stato civile | activity | Alta | — | FR-009; comp-005 | Architettura/Setup | Stabilisce il perimetro dei quadri anagrafici del nuovo servizio. |
| F4.D1.WP1.A2 | 4 | Implementare endpoint CRUD e regole di salvataggio parziale dei quadri anagrafici | activity | Alta | F4.D1.WP1.A1 | FEAT-007; FR-009; comp-005 | Backend | Ricostruisce le maschere base richieste prima del calcolo. |
| F4.D1.WP1.A3 | 4 | Abilitare versionamento quadro, recupero contesto e coerenza minima pratica/quadro | activity | Alta | F4.D1.WP1.A2 | FR-011; FEAT-009; comp-005 | Backend | Permette interruzione e ripresa della lavorazione senza perdita dati. |
| F4.D1.WP1.A4 | 4 | Eseguire test transazionali di salvataggio, ripresa pratica e regressione anagrafica | activity | Alta | F4.D1.WP1.A3 | SCN-005; SCN-006; UC-007 | Test | Riduce il rischio di perdita contesto nei flussi lunghi di lavorazione. |
| F4.D1.WP2 | 3 | CRUD quadri tecnico-economici | work_package | Alta | F4.D1.WP1 | FR-010; FR-011; FEAT-008; FEAT-009; comp-005; proc-003; UC-005; UC-007; SCN-005; SCN-006 | — | Copre il sottodominio più vasto e ad alta densità di regole della liquidazione pensionistica. |
| F4.D1.WP2.A1 | 4 | Mappare sezioni tecnico-economiche, dipendenze tra quadri e vincoli di fondo/pagamento | activity | Alta | — | FR-010; comp-005 | Architettura/Setup | Definisce il sottodominio più esteso del processo di liquidazione. |
| F4.D1.WP2.A2 | 4 | Implementare endpoint CRUD per liquidazione, contributi, redditi, pagamento, oneri e supplementi | activity | Alta | F4.D1.WP2.A1 | FR-010; FEAT-008; comp-005 | Backend | Copre il blocco dati necessario al verify e al definitivo. |
| F4.D1.WP2.A3 | 4 | Gestire coerenza cross-quadro, autosave e transazioni applicative su aggiornamenti correlati | activity | Alta | F4.D1.WP2.A2 | FR-011; FEAT-009; comp-005 | Backend | Evita stati incoerenti durante la compilazione progressiva. |
| F4.D1.WP2.A4 | 4 | Validare regressione quadri tecnico-economici con dataset rappresentativi dei tre fondi | activity | Alta | F4.D1.WP2.A3 | SCN-005; SCN-006; UC-005 | Test | Aumenta la fiducia sulla copertura del patrimonio funzionale legacy. |
| F4.D1.WP3 | 3 | Controlli dinamici e semafori | work_package | Alta | F4.D1.WP2 | FR-012; FEAT-010; comp-005; comp-011; proc-003; proc-008; UC-006; SCN-005; SCN-006 | — | Estrae in capability esplicita il cuore delle regole dinamiche e dei semafori di processo. |
| F4.D1.WP3.A1 | 4 | Definire motore regole, severità, catalogo controlli e formato messaggi operatore | activity | Alta | — | FR-012; comp-011 | Architettura/Setup | Formalizza la logica oggi distribuita e runtime-configurable. |
| F4.D1.WP3.A2 | 4 | Implementare esecuzione controlli, calcolo semafori e applicazione bypass autorizzati | activity | Alta | F4.D1.WP3.A1 | FEAT-010; FR-012; comp-005; comp-011 | Backend | Guida l’operatore sullo stato di completezza e conformità della pratica. |
| F4.D1.WP3.A3 | 4 | Verificare KO bloccanti, warning, descrizioni correttive e tracciatura degli esiti | activity | Alta | F4.D1.WP3.A2 | SCN-005; SCN-006; UC-006 | Test | Copre l’edge case principale che blocca l’avanzamento al calcolo. |
| F4.D1.WP4 | 3 | Varianti fondo FS/AGO/CI per quadri | work_package | Alta | F4.D1.WP3 | FR-012; FR-015; FEAT-010; FEAT-013; comp-005; comp-007; comp-008; comp-009; proc-003; proc-004; proc-005; UC-005; UC-006; SCN-005; SCN-006 | — | Gestisce la variabilità di fondo prima del passaggio ai motori di calcolo. |
| F4.D1.WP4.A1 | 4 | Mappare differenze per fondo su quadri, prerequisiti e percorsi di compilazione | activity | Alta | — | FR-012; FR-015; comp-005 | Architettura/Setup | Esplicita la variabilità FS/AGO/CI nel dominio quadri. |
| F4.D1.WP4.A2 | 4 | Implementare strategy fondo-specifiche per visibilità campi, regole e semafori | activity | Alta | F4.D1.WP4.A1 | FEAT-010; FEAT-013; comp-005 | Backend | Rende i quadri adattivi rispetto al fondo di appartenenza. |
| F4.D1.WP4.A3 | 4 | Validare parità funzionale delle varianti su casi campione FS, AGO e CI | activity | Alta | F4.D1.WP4.A2 | SCN-005; SCN-006 | Test | Riduce il rischio di differenze non intenzionali tra fondi. |
| F4.D2 | 2 | Frontend SPA quadri | deliverable | Alta | F4.D1 | FR-009; FR-010; FR-011; FR-012; FEAT-007; FEAT-008; FEAT-009; FEAT-010; comp-001; comp-002 | — | Realizza navigazione dei quadri, persistenza del contesto e feedback visuale dei controlli. |
| F4.D2.WP1 | 3 | UI navigazione quadri con persistenza contesto | work_package | Alta | F4.D1.WP1 | FR-009; FR-010; FR-011; FEAT-007; FEAT-008; FEAT-009; comp-001; comp-002; comp-005; proc-003; UC-004; UC-005; UC-007; SCN-005; SCN-006 | — | Organizza il journey principale di compilazione quadri sul nuovo canale operativo. |
| F4.D2.WP1.A1 | 4 | Disegnare shell di navigazione quadri, breadcrumb pratica e stato di avanzamento per fondo | activity | Alta | — | FR-009; FR-010; comp-001 | UI/UX e design | Preserva il modello mentale degli operatori durante flussi lunghi. |
| F4.D2.WP1.A2 | 4 | Implementare componenti di navigazione, routing interno e persistenza del contesto di lavoro | activity | Alta | F4.D2.WP1.A1 | FEAT-007; FEAT-009; comp-001; comp-002 | Frontend | Mantiene coerenza di sessione e posizione al rientro sulla pratica. |
| F4.D2.WP1.A3 | 4 | Integrare autosave e resume pratico con BFF e servizi quadri | activity | Alta | F4.D2.WP1.A2 | FR-011; comp-005 | Frontend | Riduce perdita di lavoro e interruzioni non presidiate. |
| F4.D2.WP2 | 3 | UI feedback semafori e validazione | work_package | Alta | F4.D2.WP1 | FR-012; FEAT-010; comp-001; comp-002; comp-005; comp-011; proc-003; proc-008; UC-006; SCN-005; SCN-006 | — | Espone in modo chiaro i feedback dei controlli e le azioni correttive richieste. |
| F4.D2.WP2.A1 | 4 | Implementare pannello semafori, messaggi di controllo e deep-link ai quadri in errore | activity | Alta | — | FEAT-010; FR-012; comp-001 | Frontend | Rende azionabili gli esiti dei controlli dinamici direttamente dalla UI. |
| F4.D2.WP2.A2 | 4 | Validare usabilità, accessibilità e blocco avanzamento in presenza di KO bloccanti | activity | Alta | F4.D2.WP2.A1 | SCN-005; SCN-006; UC-006 | Test | Assicura comprensibilità e comportamento corretto dei semafori in lavorazione. |
| F5 | 1 | Calcolo Pensione | phase | Alta | F4 | FR-013; FR-015; FR-016; FR-014; FEAT-011; FEAT-013; FEAT-014; FEAT-012; comp-002; comp-006 | — | Modernizza verify, definitivo e motori di calcolo fondo-specifici con piena tracciabilità tecnica. |
| F5.D1 | 2 | Servizio Calcolo Orchestratore | deliverable | Alta | F4.D1 | FR-013; FR-015; FR-016; FR-014; FEAT-011; FEAT-013; FEAT-014; FEAT-012; comp-002; comp-006 | — | Realizza API di calcolo, routing fondo e modernizzazione dei motori FS/AGO/CI. |
| F5.D1.WP1 | 3 | REST API calcolo verify | work_package | Alta | F4.D1.WP3 | FR-013; FR-015; FR-016; FEAT-011; FEAT-013; FEAT-014; comp-002; comp-006; proc-004; UC-008; SCN-007; SCN-008 | — | Introduce il flusso verify moderno mantenendo modificabile la pratica e tracciando gli esiti. |
| F5.D1.WP1.A1 | 4 | Definire contratto verify, chiavi di correlazione e requisiti di idempotenza del comando | activity | Alta | — | FR-013; comp-006 | Architettura/Setup | Stabilisce l’API pubblica del percorso verify non consolidante. |
| F5.D1.WP1.A2 | 4 | Implementare endpoint verify e lifecycle applicativo del calcolo nel servizio orchestratore | activity | Alta | F5.D1.WP1.A1 | FEAT-011; FR-013; comp-006 | Backend | Costruisce il flusso sincrono di simulazione per l’operatore. |
| F5.D1.WP1.A3 | 4 | Gestire warning, timeout di integrazione e persistenza transactionId/chiave tecnica | activity | Alta | F5.D1.WP1.A2 | FR-015; FR-016; comp-006 | Backend | Rende il verify tracciabile e resiliente verso dipendenze esterne. |
| F5.D1.WP1.A4 | 4 | Testare verify positivo, warning di integrazione e assenza di consolidamento pratica | activity | Alta | F5.D1.WP1.A3 | SCN-007; SCN-008; UC-008 | Test | Coprendo il comportamento funzionale e tecnico più critico del verify. |
| F5.D1.WP2 | 3 | REST API calcolo definitivo con routing fondo | work_package | Alta | F5.D1.WP1 | FR-014; FR-015; FR-016; FEAT-012; FEAT-013; FEAT-014; comp-002; comp-006; comp-012; proc-005; UC-009; SCN-009; SCN-010 | — | Modernizza il definitivo con regole di consolidamento e fault handling espliciti. |
| F5.D1.WP2.A1 | 4 | Modellare comando definitivo, condizioni di consolidamento e rollback logico in errore | activity | Alta | — | FR-014; comp-006 | Architettura/Setup | Formalizza il passaggio più sensibile del processo pensionistico. |
| F5.D1.WP2.A2 | 4 | Implementare endpoint definitivo con verifiche di semaforo, autorizzazione e stato pratica | activity | Alta | F5.D1.WP2.A1 | FEAT-012; FR-014; comp-006 | Backend | Espone il nuovo flusso di liquidazione ufficiale. |
| F5.D1.WP2.A3 | 4 | Orchestrare routing fondo, integrazioni obbligatorie e gestione dei fault bloccanti | activity | Alta | F5.D1.WP2.A2 | FR-015; FR-016; comp-006; comp-012 | Integrazione | Coordina motori e sistemi enterprise richiesti per il consolidamento. |
| F5.D1.WP2.A4 | 4 | Validare consolidamento riuscito, errore bloccante e preservazione dati in caso di fault | activity | Alta | F5.D1.WP2.A3 | SCN-009; SCN-010; UC-009 | Test | Protegge il punto di maggior rischio business e tecnico del programma. |
| F5.D1.WP3 | 3 | Motore calcolo FS (PN813 modernizzato) | work_package | Alta | F5.D1.WP2 | FR-015; FR-016; FEAT-013; FEAT-014; comp-006; comp-007; proc-004; proc-005; UC-008; UC-009; SCN-007; SCN-008; SCN-009; SCN-010 | — | Trasforma il motore FS in modulo moderno sotto controllo del nuovo orchestratore. |
| F5.D1.WP3.A1 | 4 | Estrarre regole e formule fondo FS dal modulo PN813 verso libreria .NET 8 testabile | activity | Alta | — | FR-015; comp-007 | Backend | Riduce il coupling al modulo legacy di calcolo FS. |
| F5.D1.WP3.A2 | 4 | Implementare algoritmi, validazioni e mapping output specifici del fondo FS | activity | Alta | F5.D1.WP3.A1 | FEAT-013; comp-007 | Backend | Rende il motore eseguibile sotto orchestrazione moderna. |
| F5.D1.WP3.A3 | 4 | Integrare produzione di esiti, certificato e chiave pensione per il fondo FS | activity | Alta | F5.D1.WP3.A2 | FR-016; comp-006; comp-007 | Backend | Uniforma l’output fondo-specifico al contratto del servizio calcolo. |
| F5.D1.WP3.A4 | 4 | Eseguire test di parity verify/definitivo FS contro casi legacy campione | activity | Alta | F5.D1.WP3.A3 | SCN-007; SCN-009 | Test | Misura la convergenza funzionale rispetto al comportamento storico. |
| F5.D1.WP4 | 3 | Motore calcolo AGO (PN815 modernizzato) | work_package | Alta | F5.D1.WP2 | FR-015; FR-016; FEAT-013; FEAT-014; comp-006; comp-008; proc-004; proc-005; UC-008; UC-009; SCN-007; SCN-008; SCN-009; SCN-010 | — | Modernizza il motore AGO mantenendo correttezza funzionale e integrazione nel nuovo servizio. |
| F5.D1.WP4.A1 | 4 | Estrarre regole e formule fondo AGO dal modulo PN815 verso componente .NET 8 isolato | activity | Alta | — | FR-015; comp-008 | Backend | Sposta il motore AGO fuori dal perimetro legacy nativo. |
| F5.D1.WP4.A2 | 4 | Implementare logiche fondo-specifiche AGO e mapping dei risultati al contratto comune | activity | Alta | F5.D1.WP4.A1 | FEAT-013; comp-008 | Backend | Garantisce uniformità di invocazione dal servizio calcolo. |
| F5.D1.WP4.A3 | 4 | Collegare esiti, warning e prerequisiti AGO al ciclo di vita transactionId del calcolo | activity | Alta | F5.D1.WP4.A2 | FR-016; comp-006; comp-008 | Backend | Allinea il motore ai meccanismi di tracing e orchestration moderni. |
| F5.D1.WP4.A4 | 4 | Verificare parity AGO su verify, definitivo e casi di errore applicativo | activity | Alta | F5.D1.WP4.A3 | SCN-007; SCN-010 | Test | Riduce il rischio di regressioni sul fondo a maggiore diffusione. |
| F5.D1.WP5 | 3 | Motore calcolo CI (PN818 modernizzato) | work_package | Alta | F5.D1.WP2 | FR-015; FR-016; FEAT-013; FEAT-014; comp-006; comp-009; proc-004; proc-005; UC-008; UC-009; SCN-007; SCN-008; SCN-009; SCN-010 | — | Porta nel target moderno il motore CI, tipicamente più specialistico e meno standardizzabile. |
| F5.D1.WP5.A1 | 4 | Estrarre regole e varianti CI dal modulo PN818 in componente moderno separato | activity | Alta | — | FR-015; comp-009 | Backend | Isola il perimetro convenzioni internazionali in modulo dedicato. |
| F5.D1.WP5.A2 | 4 | Implementare formule, eccezioni e output CI compatibili con il contratto del servizio calcolo | activity | Alta | F5.D1.WP5.A1 | FEAT-013; comp-009 | Backend | Preserva le specificità del fondo CI nel nuovo stack. |
| F5.D1.WP5.A3 | 4 | Integrare gestione prerequisiti e tracciabilità tecnica CI nel ciclo verify/definitivo | activity | Alta | F5.D1.WP5.A2 | FR-016; comp-006; comp-009 | Backend | Allinea il motore CI agli standard di osservabilità e audit tecnico. |
| F5.D1.WP5.A4 | 4 | Validare parity CI su casi internazionali e condizioni di errore bloccante | activity | Alta | F5.D1.WP5.A3 | SCN-008; SCN-010 | Test | Copre il fondo più specialistico e sensibile alle eccezioni di dominio. |
| F5.D2 | 2 | Frontend SPA calcolo | deliverable | Alta | F5.D1 | FR-013; FR-014; FR-016; FEAT-011; FEAT-012; FEAT-014; comp-001; comp-002; comp-006; proc-004 | — | Espone verify e definitivo agli operatori con feedback, warning e conferme sensibili. |
| F5.D2.WP1 | 3 | UI avvio calcolo e visualizzazione esito | work_package | Alta | F5.D1.WP2 | FR-013; FR-014; FR-016; FEAT-011; FEAT-012; FEAT-014; comp-001; comp-002; comp-006; proc-004; proc-005; UC-008; UC-009; SCN-007; SCN-008; SCN-009; SCN-010 | — | Completa il percorso di calcolo rendendo intelligibili esiti e conferme all’operatore. |
| F5.D2.WP1.A1 | 4 | Progettare CTA verify/definitivo, conferme sensibili e layout dell’esito calcolo | activity | Alta | — | FR-013; FR-014; comp-001 | UI/UX e design | Mantiene chiaro all’operatore il passaggio tra simulazione e consolidamento. |
| F5.D2.WP1.A2 | 4 | Implementare schermata esito con semafori, warning, chiave pensione e transactionId | activity | Alta | F5.D2.WP1.A1 | FEAT-011; FEAT-012; FEAT-014; comp-001 | Frontend | Mostra in UI l’output tecnico-funzionale del nuovo servizio calcolo. |
| F5.D2.WP1.A3 | 4 | Validare conferma definitivo, gestione warning e messaggi di errore bloccante | activity | Alta | F5.D2.WP1.A2 | SCN-007; SCN-008; SCN-009; SCN-010 | Test | Riduce errori umani sul passaggio più critico del processo. |
| F6 | 1 | Post-Calcolo, Stampa e Downstream | phase | Media | F5 | FR-017; FR-014; FR-018; FR-024; FR-025; FR-019; FEAT-015; FEAT-016; FEAT-020; FEAT-017 | — | Gestisce PDF finali, deposito documentale, aggiornamenti enterprise e consultazione output. |
| F6.D1 | 2 | Stampa e Output | deliverable | Media | F5.D1 | FR-017; FR-014; FR-018; FR-024; FR-025; FR-019; FEAT-015; FEAT-016; FEAT-020; FEAT-017 | — | Realizza stampa PDF, propagazione documentale/downstream e funzioni di consultazione output. |
| F6.D1.WP1 | 3 | Integrazione StampeWeb/certificati per PDF | work_package | Media | F5.D1.WP2 | FR-017; FEAT-015; comp-010; comp-012; proc-006; UC-010; SCN-009 | — | Realizza l’output PDF ufficiale necessario alla chiusura del processo di liquidazione. |
| F6.D1.WP1.A1 | 4 | Definire metadati documento, template certificato e contratto di generazione PDF | activity | Media | — | FR-017; comp-010 | Architettura/Setup | Stabilisce l’output formale della pratica liquidata. |
| F6.D1.WP1.A2 | 4 | Implementare adapter verso StampeWeb o servizio certificato equivalente per la produzione PDF | activity | Media | F6.D1.WP1.A1 | FEAT-015; comp-010; comp-012 | Integrazione | Connette il nuovo servizio output alla piattaforma documentale INPS. |
| F6.D1.WP1.A3 | 4 | Verificare generazione PDF, certificati allegati e gestione errori di stampa | activity | Media | F6.D1.WP1.A2 | SCN-009; UC-010 | Test | Coprendo il primo passo post-definitivo visibile all’operatore. |
| F6.D1.WP2 | 3 | Aggiornamenti downstream (SCRIWO, WebDom, FELPE, SAI etc.) | work_package | Media | F6.D1.WP1 | FR-014; FR-018; FR-024; FR-025; FEAT-016; FEAT-020; comp-002; comp-010; comp-012; proc-005; proc-006; proc-008; UC-010; UC-015; SCN-009; SCN-010 | — | Presidia il fan-out downstream che chiude il ciclo di liquidazione verso l’ecosistema enterprise. |
| F6.D1.WP2.A1 | 4 | Catalogare contratti downstream obbligatori, payload minimi e regole di idempotenza post-calcolo | activity | Media | — | FR-018; FR-024; comp-012 | Documentazione | Rende espliciti i confini contrattuali verso i sistemi enterprise. |
| F6.D1.WP2.A2 | 4 | Implementare publisher di esiti e documenti verso SCRIWO e sistemi enterprise destinatari | activity | Media | F6.D1.WP2.A1 | FEAT-016; FR-018; comp-010; comp-012 | Integrazione | Preserva la continuità end-to-end del processo pensionistico. |
| F6.D1.WP2.A3 | 4 | Configurare retry, correlation-id e gestione recuperi per integrazioni non bloccanti | activity | Media | F6.D1.WP2.A2 | FR-024; FR-025; comp-012 | Backend | Riduce impatto operativo dei fault downstream post-consolidamento. |
| F6.D1.WP2.A4 | 4 | Testare completamento, tracciatura esiti e percorsi di recupero in caso di fault esterno | activity | Media | F6.D1.WP2.A3 | SCN-009; SCN-010; UC-010 | Test | Assicura visibilità e resilienza sulle propagazioni enterprise. |
| F6.D1.WP3 | 3 | Consultazione e ristampa output | work_package | Media | F6.D1.WP2 | FR-019; FEAT-017; comp-001; comp-010; proc-006; UC-011 | — | Abilita la fruizione differita degli output senza riaprire il processo di liquidazione. |
| F6.D1.WP3.A1 | 4 | Implementare API storico output, recupero documento e ristampa on-demand per pratiche liquidate | activity | Media | — | FR-019; comp-010 | Backend | Espone consultazione e ristampa come capability dedicata di post-calcolo. |
| F6.D1.WP3.A2 | 4 | Validare consultazione/ristampa in UI e coerenza con output archiviati | activity | Media | F6.D1.WP3.A1 | UC-011; FEAT-017 | Test | Conferma riuso affidabile degli output già generati. |
| F7 | 1 | Amministrazione e Utility | phase | Media | F4 | FR-020; FR-021; FR-022; FEAT-018; FEAT-019; comp-004; comp-011; comp-005; comp-001; comp-002 | — | Raccoglie le funzioni amministrative di sblocco, riassegnazione, cambio stato e bypass controlli. |
| F7.D1 | 2 | API Amministrazione | deliverable | Media | F4.D1 | FR-020; FR-021; FR-022; FEAT-018; FEAT-019; comp-004; comp-011; comp-005; proc-007; proc-008 | — | Espone servizi amministrativi e di governance del comportamento runtime della piattaforma. |
| F7.D1.WP1 | 3 | Sblocco pratica e sblocco cancellazione | work_package | Media | F3.D1.WP3 | FR-020; FEAT-018; comp-004; comp-011; proc-007; UC-012; SCN-011 | — | Modernizza le operazioni di sblocco pratica mantenendo tracciabilità e controllo autorizzativo. |
| F7.D1.WP1.A1 | 4 | Definire workflow autorizzativo, motivazioni obbligatorie e condizioni di sblocco pratica | activity | Media | — | FR-020; comp-011 | Architettura/Setup | Formalizza un intervento amministrativo sensibile prima implicito nel legacy. |
| F7.D1.WP1.A2 | 4 | Implementare endpoint di sblocco, sblocco per cancellazione e controllo permessi ruolo-sede | activity | Media | F7.D1.WP1.A1 | FEAT-018; FR-020; comp-011 | Backend | Rende tracciabile e governato il ripristino di pratiche bloccate. |
| F7.D1.WP1.A3 | 4 | Testare audit, negazione operazione e ritorno pratica in stato lavorabile | activity | Media | F7.D1.WP1.A2 | SCN-011; UC-012 | Test | Copre il caso operativo più frequente di intervento amministrativo. |
| F7.D1.WP2 | 3 | Riassegnazione e cambio stato | work_package | Media | F7.D1.WP1 | FR-021; FEAT-018; comp-004; comp-011; proc-007; UC-012; SCN-011 | — | Gestisce la riallocazione amministrativa di pratiche e i relativi passaggi di stato. |
| F7.D1.WP2.A1 | 4 | Modellare comandi di riassegnazione, cambio stato e ownership della pratica | activity | Media | — | FR-021; comp-011 | Architettura/Setup | Rende esplicite le transizioni amministrative consentite. |
| F7.D1.WP2.A2 | 4 | Implementare API di riassegnazione, cambio stato e propagazione degli eventi di intervento | activity | Media | F7.D1.WP2.A1 | FEAT-018; FR-021; comp-011 | Backend | Consente riallineamenti organizzativi senza accessi diretti al database. |
| F7.D1.WP2.A3 | 4 | Verificare concorrenza, audit e riapertura della lavorabilità nel nuovo contesto | activity | Media | F7.D1.WP2.A2 | SCN-011; UC-012 | Test | Protegge correttezza dello stato pratica dopo interventi correttivi. |
| F7.D1.WP3 | 3 | Configurazioni e bypass controlli | work_package | Media | F7.D1.WP2 | FR-022; FEAT-019; comp-005; comp-011; proc-007; proc-008; UC-013; UC-014; SCN-012 | — | Presidia la governance delle eccezioni e delle configurazioni applicative più sensibili. |
| F7.D1.WP3.A1 | 4 | Implementare repository/versioning delle configurazioni runtime dei controlli dinamici | activity | Media | — | FR-022; comp-011 | Backend | Abilita gestione governata delle regole applicative in produzione. |
| F7.D1.WP3.A2 | 4 | Implementare endpoint per bypass, pulizia domanda e commutazione manuale/automatica con motivazione | activity | Media | F7.D1.WP3.A1 | FEAT-019; FR-022; comp-011; comp-005 | Backend | Supporta i casi eccezionali senza ricorrere a interventi diretti sui dati. |
| F7.D1.WP3.A3 | 4 | Testare rifiuto del bypass senza motivazione, audit e propagazione delle nuove regole | activity | Media | F7.D1.WP3.A2 | SCN-012; UC-013 | Test | Protegge il rischio di alterazioni non governate del comportamento runtime. |
| F7.D2 | 2 | Frontend SPA admin | deliverable | Media | F7.D1 | FR-020; FR-021; FR-022; FEAT-018; FEAT-019; comp-001; comp-002; comp-011; proc-007; proc-008 | — | Fornisce la console amministrativa per sblocco, riassegnazione, bypass e consultazione audit. |
| F7.D2.WP1 | 3 | UI funzioni amministrative | work_package | Media | F7.D1.WP3 | FR-020; FR-021; FR-022; FEAT-018; FEAT-019; comp-001; comp-002; comp-011; proc-007; proc-008; UC-012; UC-013; UC-014; SCN-011; SCN-012 | — | Rende operative le capability amministrative nel nuovo portale senza accessi tecnici diretti. |
| F7.D2.WP1.A1 | 4 | Implementare console amministrativa con form di sblocco, riassegnazione, cambio stato e bypass | activity | Media | — | FEAT-018; FEAT-019; comp-001 | Frontend | Raccoglie in un unico punto le operazioni eccezionali di governo pratica. |
| F7.D2.WP1.A2 | 4 | Validare guardie autorizzative, motivazioni obbligatorie e consultazione audit lato UI | activity | Media | F7.D2.WP1.A1 | SCN-011; SCN-012; UC-014 | Test | Conferma uso corretto della console da parte dei ruoli autorizzati. |
| F8 | 1 | Integrazione Enterprise e Compatibilità Transitoria | phase | Alta | F1 | FR-025; FR-023; FR-024; FEAT-020; comp-002; comp-012; comp-004; comp-006; comp-010; proc-008 | — | Isola i contratti enterprise e preserva compatibilità WCF/REST durante la migrazione incrementale. |
| F8.D1 | 2 | Integration Gateway | deliverable | Alta | F1.D1 | FR-025; FR-023; FR-024; FEAT-020; comp-002; comp-012; comp-004; comp-006; comp-010; proc-008 | — | Realizza adapter legacy, API versionate e gateway per sistemi host e consumer transitori. |
| F8.D1.WP1 | 3 | Adapter WCF compatibilità consumer legacy | work_package | Alta | F1.D1.WP2 | FR-025; FEAT-020; comp-002; comp-012; proc-008; UC-015 | — | Preserva la compatibilità con consumer legacy mentre il core viene strangolato per capability. |
| F8.D1.WP1.A1 | 4 | Censire contratti legacy attivi, consumer residui e priorità di compatibilità transitoria | activity | Alta | — | FR-025; UC-015 | Documentazione | Stabilisce il perimetro reale della backward compatibility da preservare. |
| F8.D1.WP1.A2 | 4 | Implementare skeleton adapter WCF/REST bridge e mapping dei contratti verso il gateway | activity | Alta | F8.D1.WP1.A1 | FEAT-020; comp-012 | Integrazione | Confinando la complessità legacy fuori dai servizi di dominio moderni. |
| F8.D1.WP1.A3 | 4 | Sviluppare translation layer request/response, propagation security e fault mapping | activity | Alta | F8.D1.WP1.A2 | FR-025; comp-002; comp-012 | Integrazione | Rende trasparente la convivenza tra consumer legacy e nuovi servizi. |
| F8.D1.WP1.A4 | 4 | Eseguire contract test di compatibilità sui principali consumer legacy ancora attivi | activity | Alta | F8.D1.WP1.A3 | UC-015; FR-025 | Test | Riduce il rischio di rotture contrattuali durante il rilascio incrementale. |
| F8.D1.WP2 | 3 | REST API versionate con OpenAPI | work_package | Alta | F8.D1.WP1 | FR-025; FEAT-020; comp-002; comp-012; proc-008; UC-015 | — | Rende governabile il catalogo API del target moderno e la sua evoluzione nel tempo. |
| F8.D1.WP2.A1 | 4 | Definire strategia di versioning, naming, error model e governance dei contratti pubblici | activity | Alta | — | FR-025; comp-002 | Architettura/Setup | Evita deriva dei contratti nella transizione tra client legacy e nuovi client. |
| F8.D1.WP2.A2 | 4 | Implementare endpoint versionati e pubblicare specifiche OpenAPI integrate nel gateway/BFF | activity | Alta | F8.D1.WP2.A1 | FEAT-020; comp-002; comp-012 | Integrazione | Espone contratti espliciti e documentati per il nuovo ecosistema client. |
| F8.D1.WP2.A3 | 4 | Automatizzare validazione schema, backward compatibility e pubblicazione artefatti di contratto | activity | Alta | F8.D1.WP2.A2 | FR-025; UC-015 | Test | Stabilizza l’evoluzione contrattuale nel tempo. |
| F8.D1.WP3 | 3 | Integration Gateway sistemi host (DB2, INPDAP, ANF) | work_package | Alta | F8.D1.WP2 | FR-023; FR-024; FEAT-020; comp-004; comp-006; comp-010; comp-012; proc-002; proc-005; proc-006; proc-008; UC-010; UC-015; SCN-008; SCN-009; SCN-010 | — | Isola l’accesso ai sistemi host e alle integrazioni enterprise più critiche del dominio. |
| F8.D1.WP3.A1 | 4 | Modellare catalogo connettori host, canonical model e policy di resilienza per sistema target | activity | Alta | — | FR-023; FR-024; comp-012 | Architettura/Setup | Centralizza la complessità dei protocolli enterprise nel gateway. |
| F8.D1.WP3.A2 | 4 | Implementare connettori DB2/host, INPDAP, ANF e mapping verso richieste di dominio | activity | Alta | F8.D1.WP3.A1 | FR-024; comp-012 | Integrazione | Abilita ricerca, calcolo e post-calcolo senza propagare protocolli legacy nel core. |
| F8.D1.WP3.A3 | 4 | Configurare circuit breaker, retry, timeout e telemetria operativa del gateway | activity | Alta | F8.D1.WP3.A2 | FR-024; comp-012 | DevOps | Riduce il rischio di fault esterni bloccanti e semplifica diagnosi. |
| F8.D1.WP3.A4 | 4 | Testare end-to-end i percorsi host e fault injection su dipendenze enterprise critiche | activity | Alta | F8.D1.WP3.A3 | SCN-008; SCN-009; SCN-010 | Test | Conferma degradazione controllata e correttezza delle trasformazioni. |
| F9 | 1 | Osservabilità, Test e Hardening | phase | Media | F5 | FR-004; FR-016; FR-018; FR-005; FR-007; FR-012; FR-013; FR-014; FR-020; FR-021 | — | Rende la soluzione osservabile, introduce metriche operative e istituzionalizza quality gate e smoke test. |
| F9.D1 | 2 | Osservabilità e Monitoring | deliverable | Media | F1.D2 | FR-004; FR-016; FR-018; FEAT-003; FEAT-014; FEAT-016; comp-002; comp-003; comp-006; comp-010 | — | Applica structured logging, correlation-id, metriche e dashboard lungo tutti i flussi modernizzati. |
| F9.D1.WP1 | 3 | Structured logging e correlation-id | work_package | Media | F1.D2.WP2 | FR-004; FR-016; FR-018; FEAT-003; FEAT-014; FEAT-016; comp-002; comp-003; comp-006; comp-010; comp-011; comp-012; proc-001; proc-004; proc-005; proc-006; proc-008; UC-014; SCN-008; SCN-010; SCN-011; SCN-012 | — | Introduce la baseline osservabile necessaria a governare integrazioni e operazioni sensibili. |
| F9.D1.WP1.A1 | 4 | Definire schema log, correlation-id e tassonomia eventi tecnici/business condivisa | activity | Media | — | FR-004; FR-016; comp-006 | Architettura/Setup | Separa audit di business da telemetria tecnica mantenendo correlazione univoca. |
| F9.D1.WP1.A2 | 4 | Implementare middleware di structured logging e propagation correlation-id su API, BFF e gateway | activity | Media | F9.D1.WP1.A1 | FEAT-014; comp-002; comp-012 | Backend | Rende diagnosi e troubleshooting omogenei in tutti i servizi. |
| F9.D1.WP1.A3 | 4 | Validare tracciatura end-to-end di warning, fault amministrativi e errori bloccanti | activity | Media | F9.D1.WP1.A2 | SCN-008; SCN-010; SCN-011; SCN-012 | Test | Misura l’efficacia della nuova baseline osservabile nei casi di eccezione. |
| F9.D1.WP2 | 3 | Metriche e dashboard operativa | work_package | Media | F9.D1.WP1 | comp-006; comp-010; comp-011; comp-012; proc-004; proc-005; proc-006; proc-008; UC-014 | — | Completa l’osservabilità con viste operative e allarmi per supporto e governance. |
| F9.D1.WP2.A1 | 4 | Pubblicare metriche SLI/SLO, dashboard di latenza/error rate/throughput e viste per supporto applicativo | activity | Media | — | comp-006; comp-012; UC-014 | DevOps | Rende visibile la salute operativa dei flussi core e delle integrazioni. |
| F9.D1.WP2.A2 | 4 | Validare soglie, alerting e adeguatezza diagnostica in ambiente di collaudo | activity | Media | F9.D1.WP2.A1 | comp-010; comp-011 | Test | Assicura che le dashboard siano realmente utili al governo quotidiano. |
| F9.D2 | 2 | Test e Quality Gate | deliverable | Media | F5.D1 | FR-005; FR-007; FR-012; FR-013; FR-014; FR-018; FR-020; FR-021; FR-022; FR-025 | — | Consolida suite automatiche, dataset di parity e smoke test di collaudo per i flussi critici. |
| F9.D2.WP1 | 3 | Test automatizzati flussi critici (verify, definitivo) | work_package | Media | F5.D1.WP2 | FR-005; FR-007; FR-012; FR-013; FR-014; FR-018; FR-020; FR-021; FR-022; FEAT-004; FEAT-006; FEAT-010; FEAT-011; FEAT-012; FEAT-016; FEAT-018; FEAT-019; comp-001; comp-002; comp-004; comp-005; comp-006; comp-010; comp-011; comp-012; proc-002; proc-003; proc-004; proc-005; proc-006; proc-007; UC-003; UC-006; UC-008; UC-009; UC-010; UC-012; UC-013; SCN-003; SCN-004; SCN-005; SCN-006; SCN-007; SCN-008; SCN-009; SCN-010; SCN-011; SCN-012 | — | Istituzionalizza la difesa automatizzata dei flussi critici prima di ogni dismissione legacy. |
| F9.D2.WP1.A1 | 4 | Costruire suite unit e integration per ricerca, acquisizione e controlli quadri | activity | Media | — | FR-005; FR-012; comp-004; comp-005 | Test | Copre i prerequisiti del percorso di calcolo e amministrazione. |
| F9.D2.WP1.A2 | 4 | Costruire suite verify/definitivo con stub integrazioni e scenari nominali/eccezione | activity | Media | F9.D2.WP1.A1 | FR-013; FR-014; comp-006; comp-012 | Test | Protegge il cuore del processo pensionistico modernizzato. |
| F9.D2.WP1.A3 | 4 | Preparare dataset golden master e casi di parity per fondi FS, AGO e CI | activity | Media | F9.D2.WP1.A2 | FEAT-011; FEAT-012; comp-006 | Test | Rende misurabile la convergenza con i moduli legacy di calcolo. |
| F9.D2.WP1.A4 | 4 | Integrare quality gate e report automatici nella pipeline di build e rilascio | activity | Media | F9.D2.WP1.A3 | FR-022; comp-002 | DevOps | Istituzionalizza il criterio di go/no-go sulle capability modernizzate. |
| F9.D2.WP2 | 3 | Smoke test ambiente di collaudo | work_package | Media | F9.D2.WP1 | FR-025; FEAT-020; comp-001; comp-002; comp-003; comp-004; comp-005; comp-006; comp-010; comp-011; comp-012; proc-001; proc-002; proc-004; proc-005; proc-007; proc-008; UC-015; SCN-001; SCN-003; SCN-007; SCN-009; SCN-011; SCN-012 | — | Fornisce un controllo rapido e ripetibile della salute end-to-end in ambiente di collaudo. |
| F9.D2.WP2.A1 | 4 | Predisporre script smoke test multi-capability su autenticazione, ricerca, calcolo e funzioni admin | activity | Media | — | FR-025; comp-001; comp-012 | Test | Verifica che l’ambiente di collaudo sia integro dopo ogni deploy. |
| F9.D2.WP2.A2 | 4 | Documentare checklist go/no-go, esiti attesi e triage iniziale dei problemi di collaudo | activity | Media | F9.D2.WP2.A1 | UC-015; comp-011 | Documentazione | Uniforma la valutazione operativa prima di cutover e parallel run. |
| F10 | 1 | Dismissione Legacy e Go-Live | phase | Bassa | F9 | FR-025; FEAT-020; comp-002; comp-004; comp-005; comp-006; comp-012; comp-010; comp-011; proc-002 | — | Chiude il ciclo Strangler Fig con parallel run, cutover progressivo e dismissione controllata del legacy. |
| F10.D1 | 2 | Strangler Fig completion | deliverable | Bassa | F9.D2 | FR-025; FEAT-020; comp-002; comp-004; comp-005; comp-006; comp-012; comp-010; comp-011; proc-002 | — | Gestisce le attività finali di parity, cutover e spegnimento dei moduli legacy residui. |
| F10.D1.WP1 | 3 | Parallel run e validazione parity PN809/PN812 | work_package | Bassa | F9.D2.WP2 | FR-025; FEAT-020; comp-002; comp-004; comp-005; comp-006; comp-012; proc-002; proc-003; proc-004; proc-005; proc-008; UC-015; SCN-003; SCN-005; SCN-007; SCN-009 | — | Misura la convergenza reale del nuovo sistema rispetto al legacy sui flussi core. |
| F10.D1.WP1.A1 | 4 | Definire campione pratiche, metriche di parity e criteri di accettazione rispetto a PN809/PN812 | activity | Bassa | — | FR-025; UC-015 | Documentazione | Stabilisce cosa significa equivalenza funzionale nel parallel run. |
| F10.D1.WP1.A2 | 4 | Eseguire parallel run controllato sui flussi ricerca, quadri e calcolo con confronto esiti | activity | Bassa | F10.D1.WP1.A1 | SCN-003; SCN-005; SCN-007; SCN-009 | Test | Misura il comportamento del target contro il legacy in condizioni reali. |
| F10.D1.WP1.A3 | 4 | Analizzare scostamenti, approvare exit criteria e aggiornare backlog di correzione finale | activity | Bassa | F10.D1.WP1.A2 | FR-025; comp-006 | Test | Supporta la decisione di avanzare verso il cutover progressivo. |
| F10.D1.WP2 | 3 | Cutover progressivo per modulo | work_package | Bassa | F10.D1.WP1 | FR-025; FEAT-020; comp-002; comp-004; comp-005; comp-006; comp-010; comp-011; comp-012; proc-001; proc-002; proc-003; proc-004; proc-005; proc-006; proc-007; proc-008; UC-015; SCN-001; SCN-003; SCN-005; SCN-007; SCN-009; SCN-011 | — | Gestisce l’attivazione progressiva del target per modulo mantenendo opzioni di fallback. |
| F10.D1.WP2.A1 | 4 | Pianificare sequenza di cutover per PN809, PN812, PN813, PN815 e PN818 con finestre operative | activity | Bassa | — | FR-025; UC-015 | Documentazione | Coordina il rilascio incrementale secondo strategia Strangler Fig. |
| F10.D1.WP2.A2 | 4 | Applicare feature toggle, routing progressivo e fallback operativo per ciascun modulo | activity | Bassa | F10.D1.WP2.A1 | FEAT-020; comp-002; comp-012 | DevOps | Permette il passaggio graduale al target con controllo del rischio. |
| F10.D1.WP2.A3 | 4 | Validare monitoraggio post-cutover, rollback e issue management operativo | activity | Bassa | F10.D1.WP2.A2 | SCN-001; SCN-009; SCN-011 | Test | Conferma la sostenibilità del cutover in esercizio reale. |
| F10.D1.WP3 | 3 | Dismissione moduli legacy con verifica consumer | work_package | Bassa | F10.D1.WP2 | FR-025; FEAT-020; comp-002; comp-012; proc-008; UC-015 | — | Completa la rimozione controllata delle ultime dipendenze legacy ancora attive. |
| F10.D1.WP3.A1 | 4 | Disattivare endpoint legacy residui e aggiornare matrice consumer/owner delle dipendenze sopravvissute | activity | Bassa | — | FR-025; comp-012 | DevOps | Completa il distacco tecnico dai moduli .NET 3.5 e WCF nativi. |
| F10.D1.WP3.A2 | 4 | Certificare rimozione dipendenze legacy, chiudere runbook e formalizzare evidenze di dismissione | activity | Bassa | F10.D1.WP3.A1 | UC-015; comp-002 | Documentazione | Chiude amministrativamente e tecnicamente il percorso di modernizzazione incrementale. |

## wbs_per_phase

### F1 — Fondazione e Setup Piattaforma
- **Priorità:** Alta
- **Predecessore:** —
- **Deliverable:** 2
- **Work Package:** 5
- **Activity:** 13
- **Focus:** Crea la baseline .NET 8, CI/CD, sicurezza e persistenza necessaria a tutte le capability successive.
  - **F1.D1 — Infrastruttura di Sviluppo e CI/CD** (3 WP, predecessore: —)
    - F1.D1.WP1: Bootstrap ambiente sviluppo locale → Bootstrap tecnico iniziale della piattaforma target e dei suoi canali principali.
    - F1.D1.WP2: Setup CI/CD pipeline → Rende eseguibili build, test e deploy frequenti della nuova architettura service-based.
    - F1.D1.WP3: Setup Database baseline → Prepara la persistenza condivisa di transizione e le fondamenta EF Core del target.
  - **F1.D2 — Hardening sicurezza e bonifica configurazione** (2 WP, predecessore: F1.D1)
    - F1.D2.WP1: Secret vault e rimozione hardcoded credentials → Allinea la modernizzazione ai vincoli INPS di secret management e separazione configurazione/codice.
    - F1.D2.WP2: Security baseline (TLS, audit trail framework) → Stabilisce la baseline di sicurezza applicativa e tracciatura comune a tutte le capability.

### F2 — Autenticazione e Contesto Operatore
- **Priorità:** Alta
- **Predecessore:** F1
- **Deliverable:** 1
- **Work Package:** 3
- **Activity:** 8
- **Focus:** Modernizza accesso federato, contesto ruolo-sede e audit iniziale degli operatori INPS.
  - **F2.D1 — Identity e RBAC modernizzati** (3 WP, predecessore: F1.D2)
    - F2.D1.WP1: Integrazione INPS Federation OIDC → Porta il sistema dal login proprietario legacy a identità federata INPS.
    - F2.D1.WP2: RBAC middleware e audit trail → Consolida autorizzazione, audit e gestione sessione come capability comune riusabile.
    - F2.D1.WP3: UI contesto operatore (ruolo/sede) → Completa lato frontend la capability di autenticazione e contesto operativo.

### F3 — Gestione Pratiche e Ricerca
- **Priorità:** Alta
- **Predecessore:** F2
- **Deliverable:** 2
- **Work Package:** 5
- **Activity:** 15
- **Focus:** Copre ricerca multi-criterio, disambiguazione, acquisizione, prenotazione e relativo front-end operativo.
  - **F3.D1 — API Ricerca Pratiche** (3 WP, predecessore: F2.D1)
    - F3.D1.WP1: REST API ricerca (NDomus, CF, anagrafica) → Costruisce l’entry point operativo per individuare la pratica da lavorare.
    - F3.D1.WP2: Integrazione WebDom e ARCA per acquisizione → Trasforma fonti enterprise eterogenee in input consistente per la lavorazione pratica.
    - F3.D1.WP3: Presa in carico e prenotazione pratica → Gestisce l’handoff sicuro da ricerca a ownership esplicita della pratica.
  - **F3.D2 — Frontend SPA ricerca e acquisizione** (2 WP, predecessore: F3.D1)
    - F3.D2.WP1: UI ricerca pratica → Traduzione UX della capability di ricerca con attenzione a disambiguazione e velocità operativa.
    - F3.D2.WP2: UI presa in carico e stato pratica → Completa il journey di acquisizione mostrando ownership e blocchi prima dell’apertura pratica.

### F4 — Quadri Applicativi
- **Priorità:** Alta
- **Predecessore:** F3
- **Deliverable:** 2
- **Work Package:** 6
- **Activity:** 19
- **Focus:** Preserva e modernizza i quadri anagrafici e tecnico-economici, inclusi controlli dinamici e varianti di fondo.
  - **F4.D1 — API Quadri** (4 WP, predecessore: F3.D1)
    - F4.D1.WP1: CRUD quadri anagrafici → Modernizza il primo blocco di quadri indispensabile per l’avvio della compilazione pratica.
    - F4.D1.WP2: CRUD quadri tecnico-economici → Copre il sottodominio più vasto e ad alta densità di regole della liquidazione pensionistica.
    - F4.D1.WP3: Controlli dinamici e semafori → Estrae in capability esplicita il cuore delle regole dinamiche e dei semafori di processo.
    - F4.D1.WP4: Varianti fondo FS/AGO/CI per quadri → Gestisce la variabilità di fondo prima del passaggio ai motori di calcolo.
  - **F4.D2 — Frontend SPA quadri** (2 WP, predecessore: F4.D1)
    - F4.D2.WP1: UI navigazione quadri con persistenza contesto → Organizza il journey principale di compilazione quadri sul nuovo canale operativo.
    - F4.D2.WP2: UI feedback semafori e validazione → Espone in modo chiaro i feedback dei controlli e le azioni correttive richieste.

### F5 — Calcolo Pensione
- **Priorità:** Alta
- **Predecessore:** F4
- **Deliverable:** 2
- **Work Package:** 6
- **Activity:** 23
- **Focus:** Modernizza verify, definitivo e motori di calcolo fondo-specifici con piena tracciabilità tecnica.
  - **F5.D1 — Servizio Calcolo Orchestratore** (5 WP, predecessore: F4.D1)
    - F5.D1.WP1: REST API calcolo verify → Introduce il flusso verify moderno mantenendo modificabile la pratica e tracciando gli esiti.
    - F5.D1.WP2: REST API calcolo definitivo con routing fondo → Modernizza il definitivo con regole di consolidamento e fault handling espliciti.
    - F5.D1.WP3: Motore calcolo FS (PN813 modernizzato) → Trasforma il motore FS in modulo moderno sotto controllo del nuovo orchestratore.
    - F5.D1.WP4: Motore calcolo AGO (PN815 modernizzato) → Modernizza il motore AGO mantenendo correttezza funzionale e integrazione nel nuovo servizio.
    - F5.D1.WP5: Motore calcolo CI (PN818 modernizzato) → Porta nel target moderno il motore CI, tipicamente più specialistico e meno standardizzabile.
  - **F5.D2 — Frontend SPA calcolo** (1 WP, predecessore: F5.D1)
    - F5.D2.WP1: UI avvio calcolo e visualizzazione esito → Completa il percorso di calcolo rendendo intelligibili esiti e conferme all’operatore.

### F6 — Post-Calcolo, Stampa e Downstream
- **Priorità:** Media
- **Predecessore:** F5
- **Deliverable:** 1
- **Work Package:** 3
- **Activity:** 9
- **Focus:** Gestisce PDF finali, deposito documentale, aggiornamenti enterprise e consultazione output.
  - **F6.D1 — Stampa e Output** (3 WP, predecessore: F5.D1)
    - F6.D1.WP1: Integrazione StampeWeb/certificati per PDF → Realizza l’output PDF ufficiale necessario alla chiusura del processo di liquidazione.
    - F6.D1.WP2: Aggiornamenti downstream (SCRIWO, WebDom, FELPE, SAI etc.) → Presidia il fan-out downstream che chiude il ciclo di liquidazione verso l’ecosistema enterprise.
    - F6.D1.WP3: Consultazione e ristampa output → Abilita la fruizione differita degli output senza riaprire il processo di liquidazione.

### F7 — Amministrazione e Utility
- **Priorità:** Media
- **Predecessore:** F4
- **Deliverable:** 2
- **Work Package:** 4
- **Activity:** 11
- **Focus:** Raccoglie le funzioni amministrative di sblocco, riassegnazione, cambio stato e bypass controlli.
  - **F7.D1 — API Amministrazione** (3 WP, predecessore: F4.D1)
    - F7.D1.WP1: Sblocco pratica e sblocco cancellazione → Modernizza le operazioni di sblocco pratica mantenendo tracciabilità e controllo autorizzativo.
    - F7.D1.WP2: Riassegnazione e cambio stato → Gestisce la riallocazione amministrativa di pratiche e i relativi passaggi di stato.
    - F7.D1.WP3: Configurazioni e bypass controlli → Presidia la governance delle eccezioni e delle configurazioni applicative più sensibili.
  - **F7.D2 — Frontend SPA admin** (1 WP, predecessore: F7.D1)
    - F7.D2.WP1: UI funzioni amministrative → Rende operative le capability amministrative nel nuovo portale senza accessi tecnici diretti.

### F8 — Integrazione Enterprise e Compatibilità Transitoria
- **Priorità:** Alta
- **Predecessore:** F1
- **Deliverable:** 1
- **Work Package:** 3
- **Activity:** 11
- **Focus:** Isola i contratti enterprise e preserva compatibilità WCF/REST durante la migrazione incrementale.
  - **F8.D1 — Integration Gateway** (3 WP, predecessore: F1.D1)
    - F8.D1.WP1: Adapter WCF compatibilità consumer legacy → Preserva la compatibilità con consumer legacy mentre il core viene strangolato per capability.
    - F8.D1.WP2: REST API versionate con OpenAPI → Rende governabile il catalogo API del target moderno e la sua evoluzione nel tempo.
    - F8.D1.WP3: Integration Gateway sistemi host (DB2, INPDAP, ANF) → Isola l’accesso ai sistemi host e alle integrazioni enterprise più critiche del dominio.

### F9 — Osservabilità, Test e Hardening
- **Priorità:** Media
- **Predecessore:** F5
- **Deliverable:** 2
- **Work Package:** 4
- **Activity:** 11
- **Focus:** Rende la soluzione osservabile, introduce metriche operative e istituzionalizza quality gate e smoke test.
  - **F9.D1 — Osservabilità e Monitoring** (2 WP, predecessore: F1.D2)
    - F9.D1.WP1: Structured logging e correlation-id → Introduce la baseline osservabile necessaria a governare integrazioni e operazioni sensibili.
    - F9.D1.WP2: Metriche e dashboard operativa → Completa l’osservabilità con viste operative e allarmi per supporto e governance.
  - **F9.D2 — Test e Quality Gate** (2 WP, predecessore: F5.D1)
    - F9.D2.WP1: Test automatizzati flussi critici (verify, definitivo) → Istituzionalizza la difesa automatizzata dei flussi critici prima di ogni dismissione legacy.
    - F9.D2.WP2: Smoke test ambiente di collaudo → Fornisce un controllo rapido e ripetibile della salute end-to-end in ambiente di collaudo.

### F10 — Dismissione Legacy e Go-Live
- **Priorità:** Bassa
- **Predecessore:** F9
- **Deliverable:** 1
- **Work Package:** 3
- **Activity:** 8
- **Focus:** Chiude il ciclo Strangler Fig con parallel run, cutover progressivo e dismissione controllata del legacy.
  - **F10.D1 — Strangler Fig completion** (3 WP, predecessore: F9.D2)
    - F10.D1.WP1: Parallel run e validazione parity PN809/PN812 → Misura la convergenza reale del nuovo sistema rispetto al legacy sui flussi core.
    - F10.D1.WP2: Cutover progressivo per modulo → Gestisce l’attivazione progressiva del target per modulo mantenendo opzioni di fallback.
    - F10.D1.WP3: Dismissione moduli legacy con verifica consumer → Completa la rimozione controllata delle ultime dipendenze legacy ancora attive.

## note_dipendenze

- Le dipendenze tra Activity sono limitate allo stesso Work Package e seguono una sequenza interna lineare.
- Le dipendenze tra Work Package sono espresse solo tramite predecessor_id al livello del WP o del Deliverable per evitare cross-link sporchi tra Activity di rami diversi.
- Le Fasi seguono il percorso architetturale Strangler Fig: fondazione, capability core, integrazione, hardening, cutover e dismissione.

WBS_COMPLETED
