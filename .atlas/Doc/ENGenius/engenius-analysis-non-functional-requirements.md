# non_functional_requirements

## quality_areas

| quality_area_id | quality_area_name | area_description | related_br_ids |
|---|---|---|---|
| QA-001 | Performance | Tempo di risposta e peso delle interazioni utente e API nei flussi operativi principali. | BR-005; BR-006 |
| QA-002 | Availability | Disponibilità del servizio, affidabilità operativa e gestione dei fault di integrazione. | BR-001; BR-006 |
| QA-003 | Security | Protezione di accessi, comunicazioni, sessioni e segreti applicativi. | BR-004; BR-001 |
| QA-004 | Usability | Esperienza d’uso coerente con il dominio pensionistico e con il modello mentale degli operatori. | BR-003; BR-006 |
| QA-005 | Scalability | Capacità del sistema di gestire più carico e più utenti senza dipendere da stato server-side. | BR-005 |
| QA-006 | Maintainability | Osservabilità tecnica, facilità di evoluzione e indipendenza dalle dipendenze legacy. | BR-002; BR-003; BR-007 |
| QA-007 | Compliance | Aderenza a obblighi di audit, accessibilità e normative della PA e dell’ecosistema INPS. | BR-004; BR-007 |

## requirement_catalog

| nfr_id | quality_area_id | requirement_name | target_or_condition | applies_when | rationale |
|---|---|---|---|---|---|
| NFR-001 | QA-001 | Tempo di risposta ricerca | La ricerca pratica deve restituire risultati entro 3 secondi nel 95° percentile. | Operatore esegue ricerca per NDomus o codice fiscale in orario operativo. | Supporta la rapida apertura della pratica richiesta agli operatori di sede. |
| NFR-002 | QA-001 | Tempo di risposta calcolo verify | Il calcolo verify deve completarsi entro 10 secondi nel 95° percentile. | Operatore avvia verify su pratica compilata. | Mantiene il verify utilizzabile come controllo operativo prima del definitivo. |
| NFR-003 | QA-001 | Tempo di risposta definitivo | Il calcolo definitivo deve completarsi entro 30 secondi nel 95° percentile. | Operatore avvia il calcolo definitivo. | Contiene i tempi di attesa nel passaggio più critico del processo pensionistico. |
| NFR-004 | QA-001 | Caricamento pagine UI | Le pagine principali non devono superare 2 secondi di caricamento. | Utente naviga tra lista pratiche e quadri della soluzione modernizzata. | Favorisce fluidità di navigazione e conservazione del contesto operativo. |
| NFR-005 | QA-001 | Payload API | Le response REST standard non devono superare 5 MB per chiamata. | Frontend modernizzato invoca API standard. | Riduce overhead di rete e protegge la reattività delle interazioni standard. |
| NFR-006 | QA-002 | Disponibilità in orario operativo | Il sistema deve essere disponibile almeno il 99,5% del tempo in orario operativo INPS. | Fascia lunedì-venerdì 8:00-18:00. | Sostiene il requisito di continuità operativa delle sedi INPS. |
| NFR-007 | QA-002 | Recovery Time Objective | In caso di interruzione il ripristino del servizio deve avvenire entro 4 ore. | Qualsiasi interruzione non pianificata. | Limita la finestra di indisponibilità del servizio critico. |
| NFR-008 | QA-002 | Recovery Point Objective | La perdita massima di dati accettabile è di 1 ora. | Scenari di failure con perdita dati. | Contiene l’impatto di eventi di recovery sul lavoro delle sedi. |
| NFR-009 | QA-002 | Gestione errori integrazioni | Il sistema deve degradare in modo controllato quando un sistema integrato non è disponibile, segnalando l’anomalia senza bloccare le funzionalità non dipendenti. | Indisponibilità parziale di sistemi upstream o downstream. | Riduce il perimetro dei blocchi operativi durante fault parziali. |
| NFR-010 | QA-003 | Autenticazione forte | Tutti gli accessi devono usare il meccanismo di identità INPS e nessun accesso anonimo è consentito. | Sempre, su tutti gli ambienti di produzione. | Allinea l’accesso alla policy di identità e rimuove varchi legacy. |
| NFR-011 | QA-003 | Autorizzazione granulare | Ogni operazione sensibile deve essere autorizzata in base a ruolo e sede e ogni tentativo non autorizzato deve produrre un evento di audit. | Ogni operazione su pratica pensionistica. | Protegge dati previdenziali e funzioni critiche in base all’organizzazione di sede. |
| NFR-012 | QA-003 | Crittografia in transito | Tutte le comunicazioni devono usare TLS 1.2 o superiore. | Tutte le comunicazioni di produzione tra client, servizi e integrazioni. | Preserva confidenzialità e integrità del traffico applicativo. |
| NFR-013 | QA-003 | Assenza di secret in codice | Nessuna credenziale, token o connection string deve essere presente in file di configurazione nel VCS o in artefatti deployati. | In ogni ambiente di test, collaudo e produzione. | Riduce il rischio di leakage e soddisfa il vincolo di secret management del PRD. |
| NFR-015 | QA-003 | Scadenza sessione | Le sessioni utente devono scadere dopo 30 minuti di inattività. | Utenti autenticati inattivi. | Limita il rischio di sessioni aperte non presidiate in ambiente di sede. |
| NFR-017 | QA-004 | Compatibilità browser | Il sistema deve funzionare sulle ultime 2 versioni major di Chrome, Firefox ed Edge e rimuovere il requisito IE. | Utilizzo da parte degli operatori INPS. | Allinea la UX al parco browser moderno della intranet aziendale. |
| NFR-018 | QA-004 | Completamento task senza training | Un operatore esperto del dominio deve completare il ciclo ricerca, quadri, definitivo senza training aggiuntivo. | Rollout iniziale e periodo di transizione. | Preserva il modello mentale degli operatori e riduce impatto organizzativo della modernizzazione. |
| NFR-023 | QA-005 | Scalabilità orizzontale | I nuovi moduli REST devono supportare più istanze parallele senza dipendenza da stato server-side. | Picchi di carico o necessità di scaling. | Rimuove il limite strutturale della soluzione stateful attuale. |
| NFR-024 | QA-005 | Carico concorrente | Il sistema deve supportare almeno 200 utenti concorrenti attivi senza degradazione percepibile delle performance. | Orario di punta delle sedi INPS. | Garantisce sostenibilità operativa nei periodi di maggior utilizzo. |
| NFR-019 | QA-006 | Osservabilità | Il sistema deve emettere log strutturati con correlation-id e rendere disponibili dashboard su latenza, error rate e throughput. | In collaudo e produzione. | Rende possibile diagnosi rapida e governo tecnico del servizio. |
| NFR-020 | QA-006 | Onboarding sviluppatori | Un nuovo sviluppatore .NET mid-level deve completare setup e prima modifica in meno di 2 giorni lavorativi. | Inserimento di nuovi membri nel team. | Misura concretamente il miglioramento di manutenibilità atteso dalla modernizzazione. |
| NFR-021 | QA-006 | Copertura test automatizzati | I flussi critici ricerca, verify e definitivo devono essere coperti da test unit e integration prima della dismissione del modulo legacy corrispondente. | Prima di ogni dismissione di modulo legacy. | Riduce regressioni e rende più sicura la migrazione incrementale. |
| NFR-022 | QA-006 | Configurazione per ambiente | Le configurazioni ambiente-specifiche devono essere iniettabili senza modifiche al codice. | In tutti gli ambienti di esecuzione. | Semplifica delivery, promozione ambienti e gestione del ciclo di rilascio. |
| NFR-025 | QA-006 | Eliminazione dipendenza .NET 3.5 | A fine migrazione di ciascun modulo non devono restare dipendenze da .NET Framework 3.5 o WCF SOAP nativo. | Per ogni modulo al termine della relativa fase di migrazione. | Formalizza l’uscita progressiva dal vincolo legacy più critico. |
| NFR-026 | QA-006 | Build cross-platform | Il sistema modernizzato deve essere buildabile su Linux e Windows con toolchain standard .NET 8+. | In CI/CD e negli ambienti di sviluppo. | Abilita pipeline moderne e riduce dipendenze da tooling legacy specifico di piattaforma. |
| NFR-014 | QA-007 | Audit trail immutabile | Tutte le operazioni di calcolo definitivo, cambio stato, sblocco ed eliminazione devono essere registrate con traccia non modificabile di utente, ruolo, sede, timestamp e payload sintetico. | Per ogni operazione amministrativa e di calcolo definitivo. | Fornisce evidenza verificabile per audit, controlli interni e accountability. |
| NFR-016 | QA-007 | Accessibilità WCAG | L’interfaccia modernizzata deve rispettare WCAG 2.1 livello AA in conformità alla normativa PA italiana. | Su tutta l’interfaccia utente modernizzata. | Garantisce conformità agli standard PA e accesso inclusivo agli utenti interni. |

NON_FUNCTIONAL_REQUIREMENTS_COMPLETED
