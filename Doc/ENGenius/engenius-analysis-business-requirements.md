# business_analysis

## business_requirements

| br_id | br_name | br_description | stakeholder_list |
|---|---|---|---|
| BR-001 | Continuità Operativa durante Trasformazione | Il processo di liquidazione pensionistica deve rimanere operativo e affidabile per tutti gli operatori INPS durante l’intero arco della modernizzazione, con migrazione incrementale e senza rimozione o degrado di funzionalità prive di piano di sostituzione approvato. | role-001; role-002; role-003; role-004 |
| BR-002 | Riduzione del Debito Tecnico Strutturale | La modernizzazione deve ridurre in modo misurabile gli antipattern strutturali più critici, in particolare God Contract, dipendenze da Session e ViewState e leakage di configurazione, così da abbassare rischio di regressione e costo del cambiamento. | role-002; role-003; role-004 |
| BR-003 | Miglioramento della Manutenibilità | Il sistema modernizzato deve poter essere gestito da team .NET moderni con competenze di mercato, riducendo la dipendenza da specialisti legacy e il tempo medio di onboarding tecnico. | role-002; role-003; role-004 |
| BR-004 | Conformità Sicurezza e Normativa | Il sistema deve allinearsi agli standard di sicurezza INPS e ai requisiti normativi vigenti, eliminando secret in chiaro, adottando vault dedicati e garantendo audit trail verificabile per le operazioni sensibili. | role-001; role-002; role-003; role-004 |
| BR-005 | Scalabilità e Sostenibilità Operativa | L’architettura target deve supportare scalabilità orizzontale e gestione dei picchi di carico, superando i limiti dell’attuale modello stateful server-side. | role-001; role-002; role-003 |
| BR-006 | Preservazione Integrale delle Capacità Funzionali | Tutte le capacità operative oggi presenti in IVS_DNA, dalla ricerca pratica alla gestione amministrativa e alle integrazioni enterprise, devono restare disponibili nel sistema modernizzato senza perdita di operatività. | role-001; role-002; role-003 |
| BR-007 | Osservabilità e Governabilità del Processo | La soluzione modernizzata deve rendere monitorabile lo stato del processo, accelerare la diagnosi delle anomalie e fornire metriche e tracciature utili alla governance operativa. | role-001; role-002; role-003; role-004 |

## list_of_roles

| role_id | role_name | role_description |
|---|---|---|
| role-001 | Operatore di sede | Utente primario intranet che ricerca pratiche, compila quadri ed esegue i calcoli di liquidazione pensionistica end-to-end. |
| role-002 | Amministratore applicativo | Utente amministrativo che gestisce sblocchi, riassegnazioni, configurazioni applicative e bypass dei controlli dinamici. |
| role-003 | Direttore/Capo processo | Responsabile operativo che supervisiona il processo e autorizza interventi eccezionali su pratiche e stati di lavorazione. |
| role-004 | Supporto tecnico applicativo | Figura tecnica autorizzata alla consultazione di diagnostica, log e evidenze applicative senza accesso diretto ai server. |

BUSINESS_ANALYSIS_COMPLETED
