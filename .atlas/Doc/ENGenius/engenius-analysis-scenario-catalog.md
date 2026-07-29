# scenario_catalog

## scenarios

| scenario_id | uc_id | scenario_name | scenario_type | related_feature_ids | trigger | expected_outcome | exception_notes |
|---|---|---|---|---|---|---|---|
| SCN-001 | UC-001 | Accesso federato con selezione contesto valida | Main | FEAT-001; FEAT-002; FEAT-003 | L’utente apre l’applicazione dalla intranet INPS. | Sessione autenticata e contesto ruolo-sede attivo con homepage disponibile. | Nessuna eccezione; flusso nominale. |
| SCN-002 | UC-001 | Tentativo di accesso con sede non autorizzata | Alternate | FEAT-001; FEAT-002; FEAT-003 | L’utente autenticato seleziona una sede non compresa fra quelle abilitate. | Il contesto non viene attivato e l’utente riceve un messaggio di autorizzazione negata. | Il tentativo deve produrre evento di audit e richiedere una nuova selezione. |
| SCN-003 | UC-002 | Ricerca pratica e apertura di domanda univoca | Main | FEAT-004; FEAT-006 | L’operatore ricerca una domanda per NDomus o codice fiscale. | La pratica corretta viene trovata, acquisita e aperta in lavorazione. | Nessuna eccezione; risultati univoci. |
| SCN-004 | UC-003 | Acquisizione negata per pratica già in lavorazione | Exception | FEAT-005; FEAT-006 | L’operatore tenta di acquisire una pratica già detenuta da un altro operatore. | Il sistema blocca la presa in carico e indica chi detiene la pratica. | Il conflitto non deve compromettere la sessione o i risultati di ricerca. |
| SCN-005 | UC-004 | Compilazione quadri con controlli tutti verdi | Main | FEAT-007; FEAT-008; FEAT-009; FEAT-010 | L’operatore apre la pratica e compila i quadri richiesti. | I dati vengono salvati e i semafori consentono il passaggio al passo successivo. | Nessuna eccezione; controlli dinamici superati. |
| SCN-006 | UC-006 | Controllo dinamico bloccante su salvataggio quadro | Exception | FEAT-009; FEAT-010 | L’operatore salva un quadro con dati incoerenti rispetto alle regole attive. | Il sistema segnala KO bloccante con messaggio comprensibile e impedisce l’avanzamento. | La pratica resta aperta e correggibile; nessun consolidamento deve avvenire. |
| SCN-007 | UC-008 | Calcolo verify con esito positivo | Main | FEAT-011; FEAT-013; FEAT-014 | L’operatore avvia il verify su pratica compilata. | Il sistema restituisce esito verify, certificato e identificativi di tracciamento senza consolidare la pratica. | Nessuna eccezione; warning assenti o non bloccanti. |
| SCN-008 | UC-008 | Verify con timeout o warning di integrazione | Alternate | FEAT-011; FEAT-013; FEAT-014 | Durante il verify un sistema integrato risponde oltre il tempo atteso o con warning. | L’operatore riceve feedback chiaro di timeout o warning e la pratica resta non consolidata. | Il sistema deve degradare in modo controllato e mantenere i dati già inseriti. |
| SCN-009 | UC-009 | Calcolo definitivo con consolidamento e aggiornamenti downstream | Main | FEAT-012; FEAT-013; FEAT-014; FEAT-016 | L’operatore conferma il calcolo definitivo. | La pratica viene consolidata e i sistemi downstream vengono aggiornati con esito tracciato. | Nessuna eccezione; integrazioni disponibili. |
| SCN-010 | UC-009 | Calcolo definitivo bloccato da errore di integrazione | Exception | FEAT-012; FEAT-013; FEAT-014; FEAT-016 | Durante il definitivo una integrazione obbligatoria restituisce errore bloccante. | Il consolidamento viene annullato e l’operatore riceve semaforo rosso con indicazione correttiva. | L’anomalia deve essere tracciata con transactionId e senza perdita di dati applicativi. |
| SCN-011 | UC-012 | Sblocco e riassegnazione amministrativa di pratica bloccata | Main | FEAT-018 | Un amministratore applicativo apre la funzione dedicata a una pratica bloccata. | La pratica viene sbloccata o riassegnata con audit completo e torna lavorabile. | Nessuna eccezione; autorizzazione valida. |
| SCN-012 | UC-013 | Richiesta di bypass rifiutata per motivazione assente | Exception | FEAT-019 | Un amministratore tenta di bypassare un controllo dinamico senza indicare la motivazione obbligatoria. | Il sistema rifiuta il bypass e richiede la motivazione prima di consentire l’operazione. | Il tentativo deve essere registrato e nessuna regola di controllo deve essere alterata. |

## scenario_steps

| scenario_id | step_id | step_order | actor_id | action | system_response |
|---|---|---|---|---|---|
| SCN-001 | STEP-001 | 1 | role-001 | Apre l’applicazione IVS modernizzata dalla intranet INPS. | Il sistema reindirizza al provider di identità federata INPS. |
| SCN-001 | STEP-002 | 2 | role-001 | Completa l’autenticazione federata. | Il sistema valida l’identità e recupera ruoli e sedi associati al profilo. |
| SCN-001 | STEP-003 | 3 | role-001 | Seleziona il ruolo operativo desiderato. | Il sistema filtra le sedi coerenti con il ruolo selezionato. |
| SCN-001 | STEP-004 | 4 | role-001 | Seleziona la sede di lavoro e conferma il contesto. | Il sistema attiva il contesto, registra l’evento di audit e prepara la homepage. |
| SCN-001 | STEP-005 | 5 | role-001 | Visualizza la homepage operativa. | Il sistema mostra ruolo, sede, matricola, messaggi di sistema e funzioni abilitate. |
| SCN-002 | STEP-006 | 1 | role-001 | Apre l’applicazione dalla intranet INPS. | Il sistema presenta il flusso di autenticazione federata. |
| SCN-002 | STEP-007 | 2 | role-001 | Si autentica con il provider INPS. | Il sistema recupera il profilo e richiede la selezione del contesto. |
| SCN-002 | STEP-008 | 3 | role-001 | Seleziona un ruolo valido. | Il sistema mostra l’elenco delle sedi disponibili per quel ruolo. |
| SCN-002 | STEP-009 | 4 | role-001 | Seleziona una sede non autorizzata o non più disponibile. | Il sistema intercetta l’incongruenza prima dell’attivazione del contesto. |
| SCN-002 | STEP-010 | 5 | role-001 | Conferma la selezione del contesto. | Il sistema nega l’accesso applicativo, registra l’evento di audit e richiede una nuova selezione. |
| SCN-003 | STEP-011 | 1 | role-001 | Accede alla funzione di ricerca pratica dalla homepage. | Il sistema mostra i campi NDomus, codice fiscale e dati anagrafici. |
| SCN-003 | STEP-012 | 2 | role-001 | Inserisce NDomus o codice fiscale e avvia la ricerca. | Il sistema valida i criteri e interroga gli archivi pratiche. |
| SCN-003 | STEP-013 | 3 | role-001 | Esamina il risultato univoco restituito. | Il sistema espone stato corrente della pratica e indicazioni di lavorazione. |
| SCN-003 | STEP-014 | 4 | role-001 | Conferma l’acquisizione della pratica. | Il sistema applica i controlli di sede e ruolo e prenota la domanda. |
| SCN-003 | STEP-015 | 5 | role-001 | Attende l’apertura della pratica in lavorazione. | Il sistema normalizza i dati con WebDom e ARCA e apre i quadri della pratica. |
| SCN-004 | STEP-016 | 1 | role-001 | Ricerca la pratica per codice fiscale. | Il sistema restituisce i risultati con le informazioni di stato disponibili. |
| SCN-004 | STEP-017 | 2 | role-001 | Seleziona la domanda che intende lavorare. | Il sistema mostra il dettaglio della pratica e l’eventuale detentore corrente. |
| SCN-004 | STEP-018 | 3 | role-001 | Richiede la presa in carico della domanda. | Il sistema verifica stato, ruolo e sede rispetto alla pratica selezionata. |
| SCN-004 | STEP-019 | 4 | role-001 | Attende l’esito di acquisizione. | Il sistema rileva che la pratica è già in lavorazione presso un altro operatore. |
| SCN-004 | STEP-020 | 5 | role-001 | Legge il messaggio di conflitto. | Il sistema blocca l’acquisizione e indica chi detiene la pratica. |
| SCN-004 | STEP-021 | 6 | role-001 | Ritorna all’elenco risultati o annulla l’operazione. | Il sistema mantiene la sessione attiva e lascia disponibili gli altri risultati. |
| SCN-005 | STEP-022 | 1 | role-001 | Apre il quadro titolare della pratica acquisita. | Il sistema carica i dati di base e mostra il contesto pratica. |
| SCN-005 | STEP-023 | 2 | role-001 | Compila i quadri anagrafici e familiari richiesti. | Il sistema evidenzia i campi modificati e mantiene il contesto della pratica. |
| SCN-005 | STEP-024 | 3 | role-001 | Naviga ai quadri tecnico-economici successivi. | Il sistema conserva la posizione di lavoro e rende disponibili i quadri coerenti con il fondo. |
| SCN-005 | STEP-025 | 4 | role-001 | Salva i dati inseriti. | Il sistema persiste i quadri in modo transazionale e aggiorna lo stato di compilazione. |
| SCN-005 | STEP-026 | 5 | role-001 | Avvia i controlli dinamici. | Il sistema esegue i controlli configurati per il fondo e calcola i semafori. |
| SCN-005 | STEP-027 | 6 | role-001 | Verifica il pannello semafori. | Il sistema mostra esiti verdi e abilita il passaggio al successivo passo operativo. |
| SCN-006 | STEP-028 | 1 | role-001 | Apre un quadro già compilato per aggiornare i dati. | Il sistema ripristina i valori correnti della pratica. |
| SCN-006 | STEP-029 | 2 | role-001 | Inserisce dati non coerenti con le regole attive del quadro. | Il sistema accetta l’input e marca il quadro come modificato. |
| SCN-006 | STEP-030 | 3 | role-001 | Salva il quadro aggiornato. | Il sistema persiste i dati e avvia i controlli dinamici collegati. |
| SCN-006 | STEP-031 | 4 | role-001 | Attende l’esito dei controlli. | Il sistema genera almeno un KO bloccante e aggiorna i semafori in rosso. |
| SCN-006 | STEP-032 | 5 | role-001 | Apre il dettaglio del controllo fallito. | Il sistema mostra un messaggio comprensibile con l’azione correttiva richiesta. |
| SCN-006 | STEP-033 | 6 | role-001 | Tenta di proseguire senza correggere il dato. | Il sistema impedisce l’avanzamento finché il controllo bloccante non risulta risolto. |
| SCN-007 | STEP-034 | 1 | role-001 | Seleziona l’azione di calcolo verify dalla pratica compilata. | Il sistema verifica che i prerequisiti minimi siano soddisfatti. |
| SCN-007 | STEP-035 | 2 | role-001 | Conferma l’avvio del verify. | Il sistema applica i prerequisiti di fondo e i controlli preliminari come ANF. |
| SCN-007 | STEP-036 | 3 | role-001 | Attende l’elaborazione del calcolo. | Il sistema instrada la richiesta al motore del fondo corretto. |
| SCN-007 | STEP-037 | 4 | role-001 | Consulta l’esito restituito dal motore. | Il sistema mostra stato pensione, certificato, semafori ed eventuali warning non bloccanti. |
| SCN-007 | STEP-038 | 5 | role-001 | Prende visione degli identificativi tecnici generati. | Il sistema espone transactionId e chiave pensione associati all’elaborazione. |
| SCN-007 | STEP-039 | 6 | role-001 | Prosegue la lavorazione senza consolidare. | Il sistema mantiene la pratica modificabile e non altera lo stato definitivo. |
| SCN-008 | STEP-040 | 1 | role-001 | Avvia il calcolo verify su una pratica compilata. | Il sistema avvia il flusso di verifica e mostra un indicatore di avanzamento. |
| SCN-008 | STEP-041 | 2 | role-001 | Attende la risposta delle integrazioni coinvolte. | Il sistema instrada la richiesta e monitora i tempi di risposta dei sistemi esterni. |
| SCN-008 | STEP-042 | 3 | role-001 | Rimane in attesa oltre il tempo atteso o riceve un warning dal sistema integrato. | Il sistema rileva timeout o warning in una dipendenza di integrazione. |
| SCN-008 | STEP-043 | 4 | role-001 | Consulta il messaggio di esito. | Il sistema distingue chiaramente tra warning non bloccante e anomalia di integrazione temporanea. |
| SCN-008 | STEP-044 | 5 | role-001 | Decide se riprovare o correggere la pratica. | Il sistema mantiene i dati già salvati e non consolida la pratica. |
| SCN-008 | STEP-045 | 6 | role-001 | Ritorna alla schermata di lavoro. | Il sistema registra l’evento tecnico e lascia disponibile la pratica per ulteriori azioni. |
| SCN-009 | STEP-046 | 1 | role-001 | Seleziona il comando di calcolo definitivo. | Il sistema richiede conferma dell’operazione sensibile. |
| SCN-009 | STEP-047 | 2 | role-001 | Conferma il definitivo. | Il sistema verifica autorizzazioni, semafori e prerequisiti di processo. |
| SCN-009 | STEP-048 | 3 | role-001 | Attende l’elaborazione definitiva. | Il sistema instrada il calcolo al motore del fondo corretto ed esegue gli aggiornamenti preliminari necessari. |
| SCN-009 | STEP-049 | 4 | role-001 | Attende il completamento delle integrazioni downstream. | Il sistema aggiorna i sistemi previsti e raccoglie gli esiti tecnici delle chiamate. |
| SCN-009 | STEP-050 | 5 | role-001 | Consulta l’esito della liquidazione. | Il sistema mostra semafori finali, transactionId e chiave pensione del calcolo. |
| SCN-009 | STEP-051 | 6 | role-001 | Passa alle attività di stampa e output. | Il sistema consolida lo stato della pratica e abilita le funzioni post-calcolo. |
| SCN-010 | STEP-052 | 1 | role-001 | Avvia il calcolo definitivo da una pratica pronta alla liquidazione. | Il sistema esegue verifiche iniziali e parte con il flusso di calcolo. |
| SCN-010 | STEP-053 | 2 | role-001 | Conferma il definitivo e attende l’elaborazione. | Il sistema instrada il calcolo e chiama i sistemi downstream obbligatori. |
| SCN-010 | STEP-054 | 3 | role-001 | Attende la risposta delle integrazioni obbligatorie. | Il sistema riceve un errore bloccante da una integrazione necessaria al consolidamento. |
| SCN-010 | STEP-055 | 4 | role-001 | Consulta il dettaglio dell’errore. | Il sistema mostra semaforo rosso, messaggio operativo e riferimenti tecnici della transazione. |
| SCN-010 | STEP-056 | 5 | role-001 | Verifica che la pratica non sia stata consolidata. | Il sistema mantiene invariato lo stato definitivo della pratica e conserva i dati inseriti. |
| SCN-010 | STEP-057 | 6 | role-001 | Segnala l’anomalia per la risoluzione. | Il sistema registra audit ed evidenze tecniche utili alla diagnosi. |
| SCN-011 | STEP-058 | 1 | role-002 | Apre la funzione amministrativa di gestione pratiche bloccate. | Il sistema mostra i filtri e le informazioni amministrative disponibili. |
| SCN-011 | STEP-059 | 2 | role-002 | Ricerca la pratica bloccata da gestire. | Il sistema restituisce lo stato, il blocco presente e l’assegnatario corrente. |
| SCN-011 | STEP-060 | 3 | role-002 | Seleziona l’azione di sblocco o riassegnazione. | Il sistema richiede motivazione e destinatario se previsto dall’azione. |
| SCN-011 | STEP-061 | 4 | role-002 | Compila i dati richiesti e conferma. | Il sistema verifica le autorizzazioni del ruolo amministrativo. |
| SCN-011 | STEP-062 | 5 | role-002 | Attende il completamento dell’operazione. | Il sistema esegue sblocco o riassegnazione e aggiorna lo stato amministrativo della pratica. |
| SCN-011 | STEP-063 | 6 | role-002 | Controlla l’esito dell’intervento. | Il sistema registra audit completo e rende la pratica lavorabile nel nuovo contesto. |
| SCN-012 | STEP-064 | 1 | role-002 | Apre il pannello amministrativo dedicato ai bypass. | Il sistema mostra l’elenco dei controlli e delle pratiche gestibili. |
| SCN-012 | STEP-065 | 2 | role-002 | Seleziona pratica e controllo da bypassare. | Il sistema prepara il form con motivazione obbligatoria. |
| SCN-012 | STEP-066 | 3 | role-002 | Lascia vuoto il campo motivazione e conferma il bypass. | Il sistema valida i dati immessi e rileva l’assenza della motivazione. |
| SCN-012 | STEP-067 | 4 | role-002 | Consulta il messaggio di errore. | Il sistema rifiuta il bypass e richiede una motivazione esplicita per proseguire. |
| SCN-012 | STEP-068 | 5 | role-002 | Verifica che il controllo non sia stato disattivato. | Il sistema mantiene inalterate le regole applicative della pratica. |
| SCN-012 | STEP-069 | 6 | role-002 | Rientra sul form per eventuale correzione. | Il sistema registra il tentativo non valido in audit e lascia la pratica invariata. |

SCENARIOS_COMPLETED
