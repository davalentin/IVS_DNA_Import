# cost_matrix

## estimation_parameters

| parametro | valore |
|---|---|
| data_stima | 2026-06-24 |
| metodologia | WBS work-package sizing + factor-based scoring |
| livello_confidenza | Medio |
| assunzioni | Team misto: 2 tech lead + 4 senior + 3 mid + 2 QA + 1 DevOps; PDR ~2-3 MD/FP per stack moderno .NET 8; stima basata su 630K SLOC → ~11.895 FP base scenario |

## cost_summary

| metrica | valore |
|---|---|
| total_work_packages | 42 |
| total_man_days_min | 328.0 |
| total_man_days_max | 655.0 |
| total_man_days_typical | 500.0 |
| average_per_work_package | 11.9 |

## cost_by_phase

| phase_id | phase_name | wp_count | man_days_min | man_days_max | man_days_typical | pct_of_total |
|---|---|---|---|---|---|---|
| F1 | Fondazione e Setup Piattaforma | 5 | 40.0 | 80.0 | 61.0 | 12.2% |
| F2 | Autenticazione e Contesto Operatore | 3 | 25.0 | 50.0 | 38.0 | 7.6% |
| F3 | Gestione Pratiche e Ricerca | 5 | 35.0 | 70.0 | 54.0 | 10.8% |
| F4 | Quadri Applicativi | 6 | 50.0 | 100.0 | 76.0 | 15.2% |
| F5 | Calcolo Pensione | 6 | 55.0 | 110.0 | 83.0 | 16.6% |
| F6 | Post-Calcolo, Stampa e Downstream | 3 | 20.0 | 40.0 | 31.0 | 6.2% |
| F7 | Amministrazione e Utility | 4 | 25.0 | 50.0 | 39.0 | 7.8% |
| F8 | Integrazione Enterprise e Compatibilità Transitoria | 3 | 25.0 | 50.0 | 38.0 | 7.6% |
| F9 | Osservabilità, Test e Hardening | 4 | 28.0 | 55.0 | 42.0 | 8.4% |
| F10 | Dismissione Legacy e Go-Live | 3 | 25.0 | 50.0 | 38.0 | 7.6% |

## cost_by_priority

| priority | work_package_count | man_days_min | man_days_max | man_days_typical | pct_of_total |
|---|---|---|---|---|---|
| Alta | 28 | 230.0 | 460.0 | 350.0 | 70.0% |
| Media | 11 | 73.0 | 145.0 | 112.0 | 22.4% |
| Bassa | 3 | 25.0 | 50.0 | 38.0 | 7.6% |

## cost_by_tshirt

| tshirt | work_package_count | man_days_min | man_days_max | man_days_typical |
|---|---|---|---|---|
| XS | 0 | 0.0 | 0.0 | 0.0 |
| S | 0 | 0.0 | 0.0 | 0.0 |
| M | 1 | 3.0 | 5.0 | 4.0 |
| L | 17 | 85.0 | 170.0 | 136.0 |
| XL | 24 | 240.0 | 480.0 | 360.0 |

## work_package_estimates

| wbs_wp_id | deliverable_id | work_package_name | priority | scenarios_count | components_count | func_cx | tech_cx | integ_cx | data_cx | risk | total_score | tshirt | man_days_min | man_days_max | man_days_typical |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| F1.D1.WP1 | F1.D1 | Bootstrap ambiente sviluppo locale | Alta | 0 | 4 | 3 | 3 | 2 | 3 | 3 | 14 | XL | 10.0 | 20.0 | 15.0 |
| F1.D1.WP2 | F1.D1 | Setup CI/CD pipeline | Alta | 0 | 2 | 3 | 3 | 2 | 3 | 3 | 14 | XL | 10.0 | 20.0 | 15.0 |
| F1.D1.WP3 | F1.D1 | Setup Database baseline | Alta | 0 | 3 | 3 | 3 | 2 | 3 | 3 | 14 | XL | 10.0 | 20.0 | 15.0 |
| F1.D2.WP1 | F1.D2 | Secret vault e rimozione hardcoded credentials | Alta | 0 | 2 | 2 | 2 | 2 | 3 | 3 | 12 | L | 5.0 | 10.0 | 8.0 |
| F1.D2.WP2 | F1.D2 | Security baseline (TLS, audit trail framework) | Alta | 0 | 2 | 2 | 3 | 2 | 3 | 3 | 13 | L | 5.0 | 10.0 | 8.0 |
| F2.D1.WP1 | F2.D1 | Integrazione INPS Federation OIDC | Alta | 2 | 2 | 3 | 3 | 2 | 3 | 3 | 14 | XL | 10.0 | 20.0 | 15.0 |
| F2.D1.WP2 | F2.D1 | RBAC middleware e audit trail | Alta | 2 | 3 | 3 | 3 | 2 | 3 | 3 | 14 | XL | 10.0 | 20.0 | 15.0 |
| F2.D1.WP3 | F2.D1 | UI contesto operatore (ruolo/sede) | Alta | 2 | 3 | 2 | 2 | 2 | 3 | 3 | 12 | L | 5.0 | 10.0 | 8.0 |
| F3.D1.WP1 | F3.D1 | REST API ricerca (NDomus, CF, anagrafica) | Alta | 1 | 2 | 3 | 3 | 2 | 3 | 3 | 14 | XL | 10.0 | 20.0 | 15.0 |
| F3.D1.WP2 | F3.D1 | Integrazione WebDom e ARCA per acquisizione | Alta | 1 | 2 | 3 | 3 | 3 | 3 | 3 | 15 | XL | 10.0 | 20.0 | 15.0 |
| F3.D1.WP3 | F3.D1 | Presa in carico e prenotazione pratica | Alta | 2 | 3 | 2 | 3 | 2 | 3 | 3 | 13 | L | 5.0 | 10.0 | 8.0 |
| F3.D2.WP1 | F3.D2 | UI ricerca pratica | Alta | 2 | 3 | 2 | 2 | 2 | 3 | 3 | 12 | L | 5.0 | 10.0 | 8.0 |
| F3.D2.WP2 | F3.D2 | UI presa in carico e stato pratica | Alta | 2 | 3 | 2 | 2 | 2 | 3 | 3 | 12 | L | 5.0 | 10.0 | 8.0 |
| F4.D1.WP1 | F4.D1 | CRUD quadri anagrafici | Alta | 2 | 1 | 3 | 3 | 2 | 3 | 3 | 14 | XL | 10.0 | 20.0 | 15.0 |
| F4.D1.WP2 | F4.D1 | CRUD quadri tecnico-economici | Alta | 2 | 1 | 3 | 3 | 3 | 3 | 3 | 15 | XL | 10.0 | 20.0 | 15.0 |
| F4.D1.WP3 | F4.D1 | Controlli dinamici e semafori | Alta | 2 | 2 | 3 | 3 | 3 | 3 | 3 | 15 | XL | 10.0 | 20.0 | 15.0 |
| F4.D1.WP4 | F4.D1 | Varianti fondo FS/AGO/CI per quadri | Alta | 2 | 4 | 3 | 3 | 2 | 3 | 3 | 14 | XL | 10.0 | 20.0 | 15.0 |
| F4.D2.WP1 | F4.D2 | UI navigazione quadri con persistenza contesto | Alta | 2 | 3 | 3 | 2 | 2 | 3 | 3 | 13 | L | 5.0 | 10.0 | 8.0 |
| F4.D2.WP2 | F4.D2 | UI feedback semafori e validazione | Alta | 2 | 4 | 2 | 2 | 2 | 3 | 3 | 12 | L | 5.0 | 10.0 | 8.0 |
| F5.D1.WP1 | F5.D1 | REST API calcolo verify | Alta | 2 | 2 | 3 | 3 | 2 | 3 | 3 | 14 | XL | 10.0 | 20.0 | 15.0 |
| F5.D1.WP2 | F5.D1 | REST API calcolo definitivo con routing fondo | Alta | 2 | 3 | 3 | 3 | 3 | 3 | 3 | 15 | XL | 10.0 | 20.0 | 15.0 |
| F5.D1.WP3 | F5.D1 | Motore calcolo FS (PN813 modernizzato) | Alta | 4 | 2 | 3 | 3 | 3 | 3 | 3 | 15 | XL | 10.0 | 20.0 | 15.0 |
| F5.D1.WP4 | F5.D1 | Motore calcolo AGO (PN815 modernizzato) | Alta | 4 | 2 | 3 | 3 | 3 | 3 | 3 | 15 | XL | 10.0 | 20.0 | 15.0 |
| F5.D1.WP5 | F5.D1 | Motore calcolo CI (PN818 modernizzato) | Alta | 4 | 2 | 3 | 3 | 3 | 3 | 3 | 15 | XL | 10.0 | 20.0 | 15.0 |
| F5.D2.WP1 | F5.D2 | UI avvio calcolo e visualizzazione esito | Alta | 4 | 3 | 3 | 2 | 2 | 3 | 3 | 13 | L | 5.0 | 10.0 | 8.0 |
| F6.D1.WP1 | F6.D1 | Integrazione StampeWeb/certificati per PDF | Media | 1 | 2 | 2 | 3 | 2 | 3 | 3 | 13 | L | 5.0 | 10.0 | 8.0 |
| F6.D1.WP2 | F6.D1 | Aggiornamenti downstream (SCRIWO, WebDom, FELPE, SAI etc.) | Media | 2 | 3 | 3 | 3 | 3 | 3 | 3 | 15 | XL | 10.0 | 20.0 | 15.0 |
| F6.D1.WP3 | F6.D1 | Consultazione e ristampa output | Media | 0 | 2 | 2 | 2 | 2 | 3 | 3 | 12 | L | 5.0 | 10.0 | 8.0 |
| F7.D1.WP1 | F7.D1 | Sblocco pratica e sblocco cancellazione | Media | 1 | 2 | 2 | 2 | 2 | 3 | 3 | 12 | L | 5.0 | 10.0 | 8.0 |
| F7.D1.WP2 | F7.D1 | Riassegnazione e cambio stato | Media | 1 | 2 | 2 | 2 | 2 | 3 | 3 | 12 | L | 5.0 | 10.0 | 8.0 |
| F7.D1.WP3 | F7.D1 | Configurazioni e bypass controlli | Media | 1 | 2 | 2 | 3 | 3 | 3 | 3 | 14 | XL | 10.0 | 20.0 | 15.0 |
| F7.D2.WP1 | F7.D2 | UI funzioni amministrative | Media | 2 | 3 | 2 | 2 | 2 | 3 | 3 | 12 | L | 5.0 | 10.0 | 8.0 |
| F8.D1.WP1 | F8.D1 | Adapter WCF compatibilità consumer legacy | Alta | 0 | 2 | 3 | 3 | 3 | 3 | 3 | 15 | XL | 10.0 | 20.0 | 15.0 |
| F8.D1.WP2 | F8.D1 | REST API versionate con OpenAPI | Alta | 0 | 2 | 2 | 3 | 2 | 3 | 3 | 13 | L | 5.0 | 10.0 | 8.0 |
| F8.D1.WP3 | F8.D1 | Integration Gateway sistemi host (DB2, INPDAP, ANF) | Alta | 3 | 4 | 3 | 3 | 3 | 3 | 3 | 15 | XL | 10.0 | 20.0 | 15.0 |
| F9.D1.WP1 | F9.D1 | Structured logging e correlation-id | Media | 4 | 6 | 2 | 3 | 3 | 3 | 3 | 14 | XL | 10.0 | 20.0 | 15.0 |
| F9.D1.WP2 | F9.D1 | Metriche e dashboard operativa | Media | 0 | 4 | 1 | 2 | 2 | 3 | 3 | 11 | M | 3.0 | 5.0 | 4.0 |
| F9.D2.WP1 | F9.D2 | Test automatizzati flussi critici (verify, definitivo) | Media | 10 | 8 | 3 | 3 | 3 | 3 | 3 | 15 | XL | 10.0 | 20.0 | 15.0 |
| F9.D2.WP2 | F9.D2 | Smoke test ambiente di collaudo | Media | 6 | 9 | 2 | 3 | 2 | 3 | 3 | 13 | L | 5.0 | 10.0 | 8.0 |
| F10.D1.WP1 | F10.D1 | Parallel run e validazione parity PN809/PN812 | Bassa | 4 | 5 | 3 | 3 | 3 | 3 | 3 | 15 | XL | 10.0 | 20.0 | 15.0 |
| F10.D1.WP2 | F10.D1 | Cutover progressivo per modulo | Bassa | 6 | 7 | 3 | 3 | 3 | 3 | 3 | 15 | XL | 10.0 | 20.0 | 15.0 |
| F10.D1.WP3 | F10.D1 | Dismissione moduli legacy con verifica consumer | Bassa | 0 | 2 | 2 | 3 | 2 | 3 | 3 | 13 | L | 5.0 | 10.0 | 8.0 |

## estimation_notes

| wbs_wp_id | tshirt | driver_principali | rischi_principali | assunzioni |
|---|---|---|---|---|
| F1.D1.WP1 | XL | Coinvolge più deployment unit e definisce la forma esecutiva dell’intera soluzione. | Rischio di divergenza tra moduli e ambienti locali se la baseline non è unificata. | Disponibilità di naming condiviso moduli e convenzioni repository già approvate. |
| F1.D1.WP2 | XL | Punto di controllo per quality gate e compatibilità contrattuale durante la transizione. | Rischio di feedback lenti o pipeline instabili se test e deploy non sono standardizzati. | Runner CI già disponibili e accesso agli ambienti target predisposto dal programma. |
| F1.D1.WP3 | XL | Coinvolge schema condiviso, transazioni e dati base comuni a più bounded context. | Rischio di coupling sullo schema e regressioni di migrazione se la baseline è debole. | Le entità chiave di pratica, quadri, audit e calcolo sono già consolidate a livello di design. |
| F1.D2.WP1 | L | Impatta tutti i runtime e le credenziali condivise dal programma. | Rischio di blocchi ambientali se i secret non sono mappati correttamente su tutti i servizi. | Vault, certificati e canali di accesso istituzionali sono disponibili prima del rollout applicativo. |
| F1.D2.WP2 | L | È un abilitatore trasversale per accessi negati, timeout, audit immutabile e diagnosi. | Rischio di incoerenza tra servizi se middleware e policy non sono standardizzati subito. | Le policy centralizzate saranno poi riusate dai WP di autenticazione, calcolo e amministrazione. |
| F2.D1.WP1 | XL | Tocca flusso di accesso primario, sicurezza e bootstrap di tutte le sessioni applicative. | Rischio di incompatibilità claims/redirect e blocco del rollout se la federation non è stabile. | Provider INPS e metadati OIDC sono accessibili dagli ambienti di sviluppo/collaudo. |
| F2.D1.WP2 | XL | Driver elevati su compliance, sicurezza e governance operativa multi-sede. | Rischio di divergenze tra servizi o audit incompleto se il middleware non è centralizzato. | Le regole di ruolo-sede possono essere ricavate da profili esistenti e validate dai referenti di processo. |
| F2.D1.WP3 | L | Interazione centrale per tutti i ruoli, ma con perimetro tecnico più contenuto del backend di identity. | Rischio di errori operativi se le transizioni ruolo-sede non sono chiare o persistenti. | Layout, messaggi e campi obbligatori seguono convenzioni UX già accettate per operatori di sede. |
| F3.D1.WP1 | XL | Scenario core ad alta frequenza con requisiti di performance e qualità dati stringenti. | Rischio di scarsa aderenza al processo di sede se i criteri o i payload differiscono dal legacy. | Gli archivi pratica esistenti sono interrogabili tramite viste/adapter e possono essere indicizzati. |
| F3.D1.WP2 | XL | Work package ad alta integrazione con dipendenze esterne e trasformazioni dati significative. | Rischio di fault esterni o mapping incompleti che impediscono apertura pratica affidabile. | Sono disponibili ambienti stub o finestre di test verso WebDom e ARCA per iterazioni progressive. |
| F3.D1.WP3 | L | Ha forte rilevanza operativa per lock logici, conflitti e correttezza dello stato pratica. | Rischio di doppia lavorazione o blocchi impropri se le verifiche di presa in carico non sono atomiche. | Le policy di autorizzazione F2 sono già disponibili e riusabili dal servizio pratiche. |
| F3.D2.WP1 | L | Coinvolge frontend, composizione BFF e comportamento coerente con dati correlati. | Rischio di scarsa usabilità se la disambiguazione non riflette il linguaggio del dominio pensionistico. | BFF e API di ricerca espongono contratti stabili per la composizione delle schermate. |
| F3.D2.WP2 | L | Perimetro contenuto ma critico per correttezza operativa e messaggi di conflitto. | Rischio di ambiguità di stato se la UI non espone chiaramente il detentore della pratica. | Il servizio pratiche restituisce dettagli di ownership e motivazioni di blocco standardizzate. |
| F4.D1.WP1 | XL | Coinvolge dominio stateful, persistenza e ripresa di lavorazione su dati anagrafici sensibili. | Rischio di regressione funzionale se il modello dei quadri non preserva il comportamento legacy. | I campi e le dipendenze dei quadri anagrafici sono stati catalogati nei documenti di analisi. |
| F4.D1.WP2 | XL | Driver massimi per numerosità quadri, relazioni dati e peso sul percorso di calcolo. | Rischio di regressioni ampie e complessità di test elevata per la varietà di sezioni supportate. | Il team dispone di campioni pratiche e supporto funzionale per validare i quadri più critici. |
| F4.D1.WP3 | XL | Perimetro ad alta criticità funzionale e cross-cutting fra quadri, amministrazione e calcolo. | Rischio di comportamenti divergenti o bypass non governati se il motore non è rigoroso. | Le configurazioni amministrative e i casi di controllo sono disponibili per bootstrap iniziale. |
| F4.D1.WP4 | XL | Coinvolge regole differenziali e dipendenza funzionale dai moduli motore per i tre fondi. | Rischio di drift tra comportamento dei quadri e prerequisiti attesi dai motori di calcolo. | Le principali differenze fondo-specifiche sono note e possono essere isolate in strategy configurabili. |
| F4.D2.WP1 | L | Coinvolge shell UI persistente, routing e interazione con persistenza transazionale. | Rischio di scarsa continuità operativa se il resume pratica non è percepito come affidabile. | I servizi quadri espongono stati di compilazione e contesto ripristinabile in modo consistente. |
| F4.D2.WP2 | L | Perimetro UI mirato ma essenziale per prevenire errori e supportare l’operatore. | Rischio di incomprensione dei KO se la visualizzazione non collega bene regola e campo interessato. | Il motore controlli restituisce severità, messaggio e riferimenti al quadro/campo interessato. |
| F5.D1.WP1 | XL | Coinvolge orchestrazione, idempotenza, prerequisiti e feedback tecnico verso l’operatore. | Rischio di effetti collaterali sullo stato pratica se il lifecycle verify non è ben isolato dal definitivo. | I prerequisiti minimi dai quadri sono già esposti dal servizio F4 e riusabili in orchestrazione. |
| F5.D1.WP2 | XL | Driver massimi per criticità business, integrazioni obbligatorie e rischio operativo. | Rischio di consolidamenti errati o incompleti se la gestione fault/rollback non è rigorosa. | Le dipendenze downstream obbligatorie sono note e classificabili come bloccanti/non bloccanti. |
| F5.D1.WP3 | XL | Elevata densità di regole fondo-specifiche e necessità di parity con comportamento legacy. | Rischio di divergenza algoritmica se l’estrazione da PN813 non è guidata da golden master. | Sono disponibili casi campione e SME di dominio FS per validazione incrementale. |
| F5.D1.WP4 | XL | Alta complessità di dominio e impatto ampio sul carico operativo complessivo. | Rischio di scostamenti economici o temporali se la parity con il motore storico non è misurata. | Le regole AGO prioritarie sono estraibili per iterazioni progressive supportate da dataset di confronto. |
| F5.D1.WP5 | XL | Complessità alta per varianti di dominio, casi eccezionali e dipendenze tecniche di routing. | Rischio di knowledge gap se la logica convenzioni internazionali non è documentata a sufficienza. | Disponibilità di esempi CI rappresentativi e supporto specialistico per i casi meno frequenti. |
| F5.D2.WP1 | L | Interazione critica ma con complessità tecnica inferiore rispetto ai motori sottostanti. | Rischio di azioni non intenzionali o incomprensione dei warning se la UI non è chiara. | Gli endpoint del servizio calcolo restituiscono payload completi per semafori, warning e identificativi. |
| F6.D1.WP1 | L | Complessità medio-alta per integrazione con servizi certificati e formati documentali istituzionali. | Rischio di non conformità del documento finale o indisponibilità dei servizi di stampa. | Template e metadati documentali possono essere allineati con gli standard già in uso presso INPS. |
| F6.D1.WP2 | XL | Massima complessità di integrazione e tracciatura tra output, documenti e sistemi esterni eterogenei. | Rischio di inconsistenze o recuperi manuali costosi se idempotenza e telemetria non sono robuste. | I sistemi downstream obbligatori sono classificati e testabili tramite gateway o stub dedicati. |
| F6.D1.WP3 | L | Perimetro funzionale più contenuto ma importante per supporto sedi e verifiche successive. | Rischio di incoerenza tra archivio documentale e metadata se il recupero non è tracciato bene. | Lo storico documentale è accessibile tramite il servizio output con identificativi stabili. |
| F7.D1.WP1 | L | Flusso amministrativo mirato ma critico per ripristinare la continuità operativa delle sedi. | Rischio di sblocchi impropri o non auditati se il workflow non è formalizzato e testato. | Le motivazioni amministrative e gli stati pratica ammissibili sono noti ai referenti di processo. |
| F7.D1.WP2 | L | Impatta processi di eccezione e governance ma con perimetro tecnico più contenuto del core business. | Rischio di stati non coerenti se riassegnazione e cambio stato non sono atomici e auditati. | Le regole di ownership pratica e i ruoli autorizzati sono già disponibili dal contesto F2. |
| F7.D1.WP3 | XL | Alta complessità per impatto cross-cutting su controlli, audit e comportamento runtime. | Rischio di abuso dei bypass o drift configurativo se versioning e audit non sono stringenti. | È disponibile un workflow approvativo minimo per le variazioni di configurazione in produzione. |
| F7.D2.WP1 | L | Perimetro frontend focalizzato su funzioni sensibili e messaggi guidati da policy. | Rischio di errore operativo se motivazioni, permessi e conseguenze non sono esposti con chiarezza. | Le API amministrative espongono esiti standardizzati e dataset sufficienti per una console unificata. |
| F8.D1.WP1 | XL | Perimetro molto rischioso per eterogeneità contratti, translation layer e retrocompatibilità. | Rischio di rottura ai consumer residui o reintroduzione di un nuovo monolite di compatibilità. | È disponibile una lista preliminare dei consumer da mantenere in vita fino al completamento del cutover. |
| F8.D1.WP2 | L | Alta criticità architetturale perché stabilisce standard di compatibilità e discoverability. | Rischio di drift contrattuale se versioni, spec e test di compatibilità non sono automatizzati. | Il gateway edge è già predisposto per esporre documentazione e routing versionato. |
| F8.D1.WP3 | XL | Massima complessità per eterogeneità protocolli, resilienza e centralità nel critical path. | Rischio di SPOF o colli di bottiglia se scaling, caching e observability non sono progettati bene. | Le dipendenze enterprise obbligatorie sono note e possono essere simulate con stub per i test iniziali. |
| F9.D1.WP1 | XL | Forte impatto cross-cutting su molti servizi e sui percorsi di anomalia più critici. | Rischio di silos informativi o tracing incompleto se la tassonomia comune non è adottata ovunque. | Tutti i servizi espongono hook middleware e possono propagare gli identificativi tecnici condivisi. |
| F9.D2.WP1 | XL | Coinvolge molti componenti, numerosi scenari e dataset di parity multi-fondo. | Rischio di falsa sicurezza se stub, golden master e quality gate non rappresentano il comportamento reale. | Sono disponibili ambienti stub e campioni di pratiche sufficienti a costruire le suite automatiche. |
| F9.D2.WP2 | L | Scope medio ma molto utile a stabilizzare rilasci incrementali e verifiche pre-go-live. | Rischio di copertura insufficiente se la smoke suite non segue l’evoluzione delle capability rilasciate. | Le principali credenziali tecniche e dataset demo sono disponibili in collaudo per esecuzioni automatiche. |
| F10.D1.WP1 | XL | Altissima sensibilità per impatto business, numero di capability coinvolte e necessità di evidenze oggettive. | Rischio di go-live prematuro se metriche parity o campioni di confronto non sono rappresentativi. | Sono disponibili accessi paralleli a legacy e target e un campione di pratiche concordato con il business. |
| F10.D1.WP2 | XL | Ha complessità molto alta per coordinamento tecnico-operativo e ampiezza del perimetro coinvolto. | Rischio di regressioni intermodulo o rollback lenti se toggle e routing non sono ben orchestrati. | Monitoraggio, smoke test e contract test delle fasi precedenti sono già operativi. |
| F10.D1.WP3 | L | Scope medio ma delicato per il numero di consumer potenzialmente residui e per gli aspetti di compliance. | Rischio di consumer nascosti o riferimenti tecnici non censiti che impediscono lo spegnimento totale. | La matrice consumer è stata aggiornata nel corso del parallel run e dei contract test precedenti. |

COST_COMPLETED: 42 work packages estimated — Total: 328.0–655.0 man-days (typical: 500.0)
