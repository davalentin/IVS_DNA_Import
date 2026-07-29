# coverage_matrix

## coverage_summary

| dimension | total_items | covered | uncovered | coverage_pct |
|---|---|---|---|---|
| scenari | 12 | 12 | 0 | 100% |
| feature | 20 | 20 | 0 | 100% |
| requisiti_funzionali | 25 | 25 | 0 | 100% |
| componenti | 12 | 12 | 0 | 100% |

## traceability_matrix_wbs_items

| wbs_item_id | level | item_name | covered_scenarios | covered_features | covered_requirements | covered_components | status |
|---|---|---|---|---|---|---|---|
| F1 | phase | Fondazione e Setup Piattaforma | — | FEAT-003 | FR-025, FR-011, FR-001, FR-004, FR-003 | comp-001, comp-002, comp-003, comp-012, comp-005, comp-006, comp-011 | ⚠️ ABILITANTE |
| F1.D1 | deliverable | Infrastruttura di Sviluppo e CI/CD | — | — | FR-025, FR-011 | comp-001, comp-002, comp-003, comp-012, comp-005, comp-006, comp-011 | ⚠️ ABILITANTE |
| F1.D1.WP1 | work_package | Bootstrap ambiente sviluppo locale | — | — | FR-025 | comp-001, comp-002, comp-003, comp-012 | ⚠️ ABILITANTE |
| F1.D1.WP2 | work_package | Setup CI/CD pipeline | — | — | FR-025 | comp-002, comp-012 | ⚠️ ABILITANTE |
| F1.D1.WP3 | work_package | Setup Database baseline | — | — | FR-011 | comp-005, comp-006, comp-011 | ⚠️ ABILITANTE |
| F1.D2 | deliverable | Hardening sicurezza e bonifica configurazione | — | FEAT-003 | FR-001, FR-004, FR-003 | comp-003, comp-011 | ⚠️ ABILITANTE |
| F1.D2.WP1 | work_package | Secret vault e rimozione hardcoded credentials | — | — | FR-001, FR-004 | comp-003, comp-011 | ⚠️ ABILITANTE |
| F1.D2.WP2 | work_package | Security baseline (TLS, audit trail framework) | — | FEAT-003 | FR-003, FR-004 | comp-003, comp-011 | ⚠️ ABILITANTE |
| F2 | phase | Autenticazione e Contesto Operatore | SCN-001, SCN-002 | FEAT-001, FEAT-003, FEAT-002 | FR-001, FR-003, FR-004, FR-002 | comp-002, comp-003, comp-011, comp-001 | ✅ TRACCIABILE |
| F2.D1 | deliverable | Identity e RBAC modernizzati | SCN-001, SCN-002 | FEAT-001, FEAT-003, FEAT-002 | FR-001, FR-003, FR-004, FR-002 | comp-002, comp-003, comp-011, comp-001 | ✅ TRACCIABILE |
| F2.D1.WP1 | work_package | Integrazione INPS Federation OIDC | SCN-001, SCN-002 | FEAT-001 | FR-001 | comp-002, comp-003 | ✅ TRACCIABILE |
| F2.D1.WP2 | work_package | RBAC middleware e audit trail | SCN-001, SCN-002 | FEAT-003 | FR-003, FR-004 | comp-002, comp-003, comp-011 | ✅ TRACCIABILE |
| F2.D1.WP3 | work_package | UI contesto operatore (ruolo/sede) | SCN-001, SCN-002 | FEAT-002 | FR-002 | comp-001, comp-002, comp-003 | ✅ TRACCIABILE |
| F3 | phase | Gestione Pratiche e Ricerca | SCN-003, SCN-004 | FEAT-004, FEAT-005, FEAT-006 | FR-005, FR-006, FR-008, FR-023, FR-007 | comp-002, comp-004, comp-012, comp-003, comp-001 | ✅ TRACCIABILE |
| F3.D1 | deliverable | API Ricerca Pratiche | SCN-003, SCN-004 | FEAT-004, FEAT-005, FEAT-006 | FR-005, FR-006, FR-008, FR-023, FR-007 | comp-002, comp-004, comp-012, comp-003 | ✅ TRACCIABILE |
| F3.D1.WP1 | work_package | REST API ricerca (NDomus, CF, anagrafica) | SCN-003 | FEAT-004 | FR-005 | comp-002, comp-004 | ✅ TRACCIABILE |
| F3.D1.WP2 | work_package | Integrazione WebDom e ARCA per acquisizione | SCN-003 | FEAT-005, FEAT-006 | FR-006, FR-008, FR-023 | comp-004, comp-012 | ✅ TRACCIABILE |
| F3.D1.WP3 | work_package | Presa in carico e prenotazione pratica | SCN-003, SCN-004 | FEAT-006 | FR-007, FR-008 | comp-003, comp-004, comp-012 | ✅ TRACCIABILE |
| F3.D2 | deliverable | Frontend SPA ricerca e acquisizione | SCN-003, SCN-004 | FEAT-004, FEAT-005, FEAT-006 | FR-005, FR-006, FR-007, FR-008 | comp-001, comp-002, comp-004 | ✅ TRACCIABILE |
| F3.D2.WP1 | work_package | UI ricerca pratica | SCN-003, SCN-004 | FEAT-004, FEAT-005 | FR-005, FR-006 | comp-001, comp-002, comp-004 | ✅ TRACCIABILE |
| F3.D2.WP2 | work_package | UI presa in carico e stato pratica | SCN-003, SCN-004 | FEAT-006 | FR-007, FR-008 | comp-001, comp-002, comp-004 | ✅ TRACCIABILE |
| F4 | phase | Quadri Applicativi | SCN-005, SCN-006 | FEAT-007, FEAT-009, FEAT-008, FEAT-010, FEAT-013 | FR-009, FR-011, FR-010, FR-012, FR-015 | comp-005, comp-011, comp-007, comp-008, comp-009, comp-001, comp-002 | ✅ TRACCIABILE |
| F4.D1 | deliverable | API Quadri | SCN-005, SCN-006 | FEAT-007, FEAT-009, FEAT-008, FEAT-010, FEAT-013 | FR-009, FR-011, FR-010, FR-012, FR-015 | comp-005, comp-011, comp-007, comp-008, comp-009 | ✅ TRACCIABILE |
| F4.D1.WP1 | work_package | CRUD quadri anagrafici | SCN-005, SCN-006 | FEAT-007, FEAT-009 | FR-009, FR-011 | comp-005 | ✅ TRACCIABILE |
| F4.D1.WP2 | work_package | CRUD quadri tecnico-economici | SCN-005, SCN-006 | FEAT-008, FEAT-009 | FR-010, FR-011 | comp-005 | ✅ TRACCIABILE |
| F4.D1.WP3 | work_package | Controlli dinamici e semafori | SCN-005, SCN-006 | FEAT-010 | FR-012 | comp-005, comp-011 | ✅ TRACCIABILE |
| F4.D1.WP4 | work_package | Varianti fondo FS/AGO/CI per quadri | SCN-005, SCN-006 | FEAT-010, FEAT-013 | FR-012, FR-015 | comp-005, comp-007, comp-008, comp-009 | ✅ TRACCIABILE |
| F4.D2 | deliverable | Frontend SPA quadri | SCN-005, SCN-006 | FEAT-007, FEAT-008, FEAT-009, FEAT-010 | FR-009, FR-010, FR-011, FR-012 | comp-001, comp-002, comp-005, comp-011 | ✅ TRACCIABILE |
| F4.D2.WP1 | work_package | UI navigazione quadri con persistenza contesto | SCN-005, SCN-006 | FEAT-007, FEAT-008, FEAT-009 | FR-009, FR-010, FR-011 | comp-001, comp-002, comp-005 | ✅ TRACCIABILE |
| F4.D2.WP2 | work_package | UI feedback semafori e validazione | SCN-005, SCN-006 | FEAT-010 | FR-012 | comp-001, comp-002, comp-005, comp-011 | ✅ TRACCIABILE |
| F5 | phase | Calcolo Pensione | SCN-007, SCN-008, SCN-009, SCN-010 | FEAT-011, FEAT-013, FEAT-014, FEAT-012 | FR-013, FR-015, FR-016, FR-014 | comp-002, comp-006, comp-012, comp-007, comp-008, comp-009, comp-001 | ✅ TRACCIABILE |
| F5.D1 | deliverable | Servizio Calcolo Orchestratore | SCN-007, SCN-008, SCN-009, SCN-010 | FEAT-011, FEAT-013, FEAT-014, FEAT-012 | FR-013, FR-015, FR-016, FR-014 | comp-002, comp-006, comp-012, comp-007, comp-008, comp-009 | ✅ TRACCIABILE |
| F5.D1.WP1 | work_package | REST API calcolo verify | SCN-007, SCN-008 | FEAT-011, FEAT-013, FEAT-014 | FR-013, FR-015, FR-016 | comp-002, comp-006 | ✅ TRACCIABILE |
| F5.D1.WP2 | work_package | REST API calcolo definitivo con routing fondo | SCN-009, SCN-010 | FEAT-012, FEAT-013, FEAT-014 | FR-014, FR-015, FR-016 | comp-002, comp-006, comp-012 | ✅ TRACCIABILE |
| F5.D1.WP3 | work_package | Motore calcolo FS (PN813 modernizzato) | SCN-007, SCN-008, SCN-009, SCN-010 | FEAT-013, FEAT-014 | FR-015, FR-016 | comp-006, comp-007 | ✅ TRACCIABILE |
| F5.D1.WP4 | work_package | Motore calcolo AGO (PN815 modernizzato) | SCN-007, SCN-008, SCN-009, SCN-010 | FEAT-013, FEAT-014 | FR-015, FR-016 | comp-006, comp-008 | ✅ TRACCIABILE |
| F5.D1.WP5 | work_package | Motore calcolo CI (PN818 modernizzato) | SCN-007, SCN-008, SCN-009, SCN-010 | FEAT-013, FEAT-014 | FR-015, FR-016 | comp-006, comp-009 | ✅ TRACCIABILE |
| F5.D2 | deliverable | Frontend SPA calcolo | SCN-007, SCN-008, SCN-009, SCN-010 | FEAT-011, FEAT-012, FEAT-014 | FR-013, FR-014, FR-016 | comp-001, comp-002, comp-006 | ✅ TRACCIABILE |
| F5.D2.WP1 | work_package | UI avvio calcolo e visualizzazione esito | SCN-007, SCN-008, SCN-009, SCN-010 | FEAT-011, FEAT-012, FEAT-014 | FR-013, FR-014, FR-016 | comp-001, comp-002, comp-006 | ✅ TRACCIABILE |
| F6 | phase | Post-Calcolo, Stampa e Downstream | SCN-009, SCN-010 | FEAT-015, FEAT-016, FEAT-020, FEAT-017 | FR-017, FR-014, FR-018, FR-024, FR-025, FR-019 | comp-010, comp-012, comp-002, comp-001 | ✅ TRACCIABILE |
| F6.D1 | deliverable | Stampa e Output | SCN-009, SCN-010 | FEAT-015, FEAT-016, FEAT-020, FEAT-017 | FR-017, FR-014, FR-018, FR-024, FR-025, FR-019 | comp-010, comp-012, comp-002, comp-001 | ✅ TRACCIABILE |
| F6.D1.WP1 | work_package | Integrazione StampeWeb/certificati per PDF | SCN-009 | FEAT-015 | FR-017 | comp-010, comp-012 | ✅ TRACCIABILE |
| F6.D1.WP2 | work_package | Aggiornamenti downstream (SCRIWO, WebDom, FELPE, SAI etc.) | SCN-009, SCN-010 | FEAT-016, FEAT-020 | FR-014, FR-018, FR-024, FR-025 | comp-002, comp-010, comp-012 | ✅ TRACCIABILE |
| F6.D1.WP3 | work_package | Consultazione e ristampa output | — | FEAT-017 | FR-019 | comp-001, comp-010 | ⚠️ ABILITANTE |
| F7 | phase | Amministrazione e Utility | SCN-011, SCN-012 | FEAT-018, FEAT-019 | FR-020, FR-021, FR-022 | comp-004, comp-011, comp-005, comp-001, comp-002 | ✅ TRACCIABILE |
| F7.D1 | deliverable | API Amministrazione | SCN-011, SCN-012 | FEAT-018, FEAT-019 | FR-020, FR-021, FR-022 | comp-004, comp-011, comp-005 | ✅ TRACCIABILE |
| F7.D1.WP1 | work_package | Sblocco pratica e sblocco cancellazione | SCN-011 | FEAT-018 | FR-020 | comp-004, comp-011 | ✅ TRACCIABILE |
| F7.D1.WP2 | work_package | Riassegnazione e cambio stato | SCN-011 | FEAT-018 | FR-021 | comp-004, comp-011 | ✅ TRACCIABILE |
| F7.D1.WP3 | work_package | Configurazioni e bypass controlli | SCN-012 | FEAT-019 | FR-022 | comp-005, comp-011 | ✅ TRACCIABILE |
| F7.D2 | deliverable | Frontend SPA admin | SCN-011, SCN-012 | FEAT-018, FEAT-019 | FR-020, FR-021, FR-022 | comp-001, comp-002, comp-011 | ✅ TRACCIABILE |
| F7.D2.WP1 | work_package | UI funzioni amministrative | SCN-011, SCN-012 | FEAT-018, FEAT-019 | FR-020, FR-021, FR-022 | comp-001, comp-002, comp-011 | ✅ TRACCIABILE |
| F8 | phase | Integrazione Enterprise e Compatibilità Transitoria | SCN-008, SCN-009, SCN-010 | FEAT-020 | FR-025, FR-023, FR-024 | comp-002, comp-012, comp-004, comp-006, comp-010 | ✅ TRACCIABILE |
| F8.D1 | deliverable | Integration Gateway | SCN-008, SCN-009, SCN-010 | FEAT-020 | FR-025, FR-023, FR-024 | comp-002, comp-012, comp-004, comp-006, comp-010 | ✅ TRACCIABILE |
| F8.D1.WP1 | work_package | Adapter WCF compatibilità consumer legacy | — | FEAT-020 | FR-025 | comp-002, comp-012 | ⚠️ ABILITANTE |
| F8.D1.WP2 | work_package | REST API versionate con OpenAPI | — | FEAT-020 | FR-025 | comp-002, comp-012 | ⚠️ ABILITANTE |
| F8.D1.WP3 | work_package | Integration Gateway sistemi host (DB2, INPDAP, ANF) | SCN-008, SCN-009, SCN-010 | FEAT-020 | FR-023, FR-024 | comp-004, comp-006, comp-010, comp-012 | ✅ TRACCIABILE |
| F9 | phase | Osservabilità, Test e Hardening | SCN-008, SCN-010, SCN-011, SCN-012, SCN-003, SCN-004, SCN-005, SCN-006, SCN-007, SCN-009, SCN-001 | FEAT-003, FEAT-014, FEAT-016, FEAT-004, FEAT-006, FEAT-010, FEAT-011, FEAT-012, FEAT-018, FEAT-019, FEAT-020 | FR-004, FR-016, FR-018, FR-005, FR-007, FR-012, FR-013, FR-014, FR-020, FR-021, FR-022, FR-025 | comp-002, comp-003, comp-006, comp-010, comp-011, comp-012, comp-001, comp-004, comp-005 | ✅ TRACCIABILE |
| F9.D1 | deliverable | Osservabilità e Monitoring | SCN-008, SCN-010, SCN-011, SCN-012 | FEAT-003, FEAT-014, FEAT-016 | FR-004, FR-016, FR-018 | comp-002, comp-003, comp-006, comp-010, comp-011, comp-012 | ✅ TRACCIABILE |
| F9.D1.WP1 | work_package | Structured logging e correlation-id | SCN-008, SCN-010, SCN-011, SCN-012 | FEAT-003, FEAT-014, FEAT-016 | FR-004, FR-016, FR-018 | comp-002, comp-003, comp-006, comp-010, comp-011, comp-012 | ✅ TRACCIABILE |
| F9.D1.WP2 | work_package | Metriche e dashboard operativa | — | — | — | comp-006, comp-010, comp-011, comp-012 | ⚠️ ABILITANTE |
| F9.D2 | deliverable | Test e Quality Gate | SCN-003, SCN-004, SCN-005, SCN-006, SCN-007, SCN-008, SCN-009, SCN-010, SCN-011, SCN-012, SCN-001 | FEAT-004, FEAT-006, FEAT-010, FEAT-011, FEAT-012, FEAT-016, FEAT-018, FEAT-019, FEAT-020 | FR-005, FR-007, FR-012, FR-013, FR-014, FR-018, FR-020, FR-021, FR-022, FR-025 | comp-001, comp-002, comp-004, comp-005, comp-006, comp-010, comp-011, comp-012, comp-003 | ✅ TRACCIABILE |
| F9.D2.WP1 | work_package | Test automatizzati flussi critici (verify, definitivo) | SCN-003, SCN-004, SCN-005, SCN-006, SCN-007, SCN-008, SCN-009, SCN-010, SCN-011, SCN-012 | FEAT-004, FEAT-006, FEAT-010, FEAT-011, FEAT-012, FEAT-016, FEAT-018, FEAT-019 | FR-005, FR-007, FR-012, FR-013, FR-014, FR-018, FR-020, FR-021, FR-022 | comp-001, comp-002, comp-004, comp-005, comp-006, comp-010, comp-011, comp-012 | ✅ TRACCIABILE |
| F9.D2.WP2 | work_package | Smoke test ambiente di collaudo | SCN-001, SCN-003, SCN-007, SCN-009, SCN-011, SCN-012 | FEAT-020 | FR-025 | comp-001, comp-002, comp-003, comp-004, comp-005, comp-006, comp-010, comp-011, comp-012 | ✅ TRACCIABILE |
| F10 | phase | Dismissione Legacy e Go-Live | SCN-003, SCN-005, SCN-007, SCN-009, SCN-001, SCN-011 | FEAT-020 | FR-025 | comp-002, comp-004, comp-005, comp-006, comp-012, comp-010, comp-011 | ✅ TRACCIABILE |
| F10.D1 | deliverable | Strangler Fig completion | SCN-003, SCN-005, SCN-007, SCN-009, SCN-001, SCN-011 | FEAT-020 | FR-025 | comp-002, comp-004, comp-005, comp-006, comp-012, comp-010, comp-011 | ✅ TRACCIABILE |
| F10.D1.WP1 | work_package | Parallel run e validazione parity PN809/PN812 | SCN-003, SCN-005, SCN-007, SCN-009 | FEAT-020 | FR-025 | comp-002, comp-004, comp-005, comp-006, comp-012 | ✅ TRACCIABILE |
| F10.D1.WP2 | work_package | Cutover progressivo per modulo | SCN-001, SCN-003, SCN-005, SCN-007, SCN-009, SCN-011 | FEAT-020 | FR-025 | comp-002, comp-004, comp-005, comp-006, comp-010, comp-011, comp-012 | ✅ TRACCIABILE |
| F10.D1.WP3 | work_package | Dismissione moduli legacy con verifica consumer | — | FEAT-020 | FR-025 | comp-002, comp-012 | ⚠️ ABILITANTE |

## traceability_matrix_scenarios

| scenario_id | title | covered_by_wbs_wps | status |
|---|---|---|---|
| SCN-001 | Accesso federato con selezione contesto valida | F2.D1.WP1, F2.D1.WP2, F2.D1.WP3, F9.D2.WP2, F10.D1.WP2 | ✅ COPERTO |
| SCN-002 | Tentativo di accesso con sede non autorizzata | F2.D1.WP1, F2.D1.WP2, F2.D1.WP3 | ✅ COPERTO |
| SCN-003 | Ricerca pratica e apertura di domanda univoca | F3.D1.WP1, F3.D1.WP2, F3.D1.WP3, F3.D2.WP1, F3.D2.WP2, F9.D2.WP1, F9.D2.WP2, F10.D1.WP1, F10.D1.WP2 | ✅ COPERTO |
| SCN-004 | Acquisizione negata per pratica già in lavorazione | F3.D1.WP3, F3.D2.WP1, F3.D2.WP2, F9.D2.WP1 | ✅ COPERTO |
| SCN-005 | Compilazione quadri con controlli tutti verdi | F4.D1.WP1, F4.D1.WP2, F4.D1.WP3, F4.D1.WP4, F4.D2.WP1, F4.D2.WP2, F9.D2.WP1, F10.D1.WP1, F10.D1.WP2 | ✅ COPERTO |
| SCN-006 | Controllo dinamico bloccante su salvataggio quadro | F4.D1.WP1, F4.D1.WP2, F4.D1.WP3, F4.D1.WP4, F4.D2.WP1, F4.D2.WP2, F9.D2.WP1 | ✅ COPERTO |
| SCN-007 | Calcolo verify con esito positivo | F5.D1.WP1, F5.D1.WP3, F5.D1.WP4, F5.D1.WP5, F5.D2.WP1, F9.D2.WP1, F9.D2.WP2, F10.D1.WP1, F10.D1.WP2 | ✅ COPERTO |
| SCN-008 | Verify con timeout o warning di integrazione | F5.D1.WP1, F5.D1.WP3, F5.D1.WP4, F5.D1.WP5, F5.D2.WP1, F8.D1.WP3, F9.D1.WP1, F9.D2.WP1 | ✅ COPERTO |
| SCN-009 | Calcolo definitivo con consolidamento e aggiornamenti downstream | F5.D1.WP2, F5.D1.WP3, F5.D1.WP4, F5.D1.WP5, F5.D2.WP1, F6.D1.WP1, F6.D1.WP2, F8.D1.WP3, F9.D2.WP1, F9.D2.WP2, F10.D1.WP1, F10.D1.WP2 | ✅ COPERTO |
| SCN-010 | Calcolo definitivo bloccato da errore di integrazione | F5.D1.WP2, F5.D1.WP3, F5.D1.WP4, F5.D1.WP5, F5.D2.WP1, F6.D1.WP2, F8.D1.WP3, F9.D1.WP1, F9.D2.WP1 | ✅ COPERTO |
| SCN-011 | Sblocco e riassegnazione amministrativa di pratica bloccata | F7.D1.WP1, F7.D1.WP2, F7.D2.WP1, F9.D1.WP1, F9.D2.WP1, F9.D2.WP2, F10.D1.WP2 | ✅ COPERTO |
| SCN-012 | Richiesta di bypass rifiutata per motivazione assente | F7.D1.WP3, F7.D2.WP1, F9.D1.WP1, F9.D2.WP1, F9.D2.WP2 | ✅ COPERTO |

## traceability_matrix_features

| feature_id | feature_name | related_scenarios | covered_by_wbs_wps | coverage_status |
|---|---|---|---|---|
| FEAT-001 | Accesso federato INPS | SCN-001, SCN-002 | F2.D1.WP1 | ✅ COPERTA |
| FEAT-002 | Selezione contesto operativo multi-sede | SCN-001, SCN-002 | F2.D1.WP3 | ✅ COPERTA |
| FEAT-003 | Autorizzazioni, audit e gestione sessione | SCN-001, SCN-002 | F1.D2.WP2, F2.D1.WP2, F9.D1.WP1 | ✅ COPERTA |
| FEAT-004 | Ricerca multi-criterio pratiche | SCN-003 | F3.D1.WP1, F3.D2.WP1, F9.D2.WP1 | ✅ COPERTA |
| FEAT-005 | Disambiguazione domande e correlazioni | SCN-004 | F3.D1.WP2, F3.D2.WP1 | ✅ COPERTA |
| FEAT-006 | Acquisizione e prenotazione pratica | SCN-003, SCN-004 | F3.D1.WP2, F3.D1.WP3, F3.D2.WP2, F9.D2.WP1 | ✅ COPERTA |
| FEAT-007 | Quadri anagrafici e familiari | SCN-005 | F4.D1.WP1, F4.D2.WP1 | ✅ COPERTA |
| FEAT-008 | Quadri tecnico-economici di liquidazione | SCN-005 | F4.D1.WP2, F4.D2.WP1 | ✅ COPERTA |
| FEAT-009 | Salvataggio transazionale e ripresa pratica | SCN-005, SCN-006 | F4.D1.WP1, F4.D1.WP2, F4.D2.WP1 | ✅ COPERTA |
| FEAT-010 | Controlli dinamici, semafori e varianti fondo | SCN-005, SCN-006 | F4.D1.WP3, F4.D1.WP4, F4.D2.WP2, F9.D2.WP1 | ✅ COPERTA |
| FEAT-011 | Calcolo verify | SCN-007, SCN-008 | F5.D1.WP1, F5.D2.WP1, F9.D2.WP1 | ✅ COPERTA |
| FEAT-012 | Calcolo definitivo | SCN-009, SCN-010 | F5.D1.WP2, F5.D2.WP1, F9.D2.WP1 | ✅ COPERTA |
| FEAT-013 | Routing motore fondo e prerequisiti ANF | SCN-007, SCN-008, SCN-009, SCN-010 | F4.D1.WP4, F5.D1.WP1, F5.D1.WP2, F5.D1.WP3, F5.D1.WP4, F5.D1.WP5 | ✅ COPERTA |
| FEAT-014 | Esito calcolo e tracciabilità | SCN-007, SCN-008, SCN-009, SCN-010 | F5.D1.WP1, F5.D1.WP2, F5.D1.WP3, F5.D1.WP4, F5.D1.WP5, F5.D2.WP1, F9.D1.WP1 | ✅ COPERTA |
| FEAT-015 | Stampa finale PDF e certificati | — | F6.D1.WP1 | ✅ COPERTA |
| FEAT-016 | Aggiornamenti downstream e deposito documentale | SCN-009, SCN-010 | F6.D1.WP2, F9.D1.WP1, F9.D2.WP1 | ✅ COPERTA |
| FEAT-017 | Consultazione e ristampa output | — | F6.D1.WP3 | ✅ COPERTA |
| FEAT-018 | Sblocco, riassegnazione e cambio stato pratica | SCN-011 | F7.D1.WP1, F7.D1.WP2, F7.D2.WP1, F9.D2.WP1 | ✅ COPERTA |
| FEAT-019 | Configurazioni amministrative e bypass controlli | SCN-012 | F7.D1.WP3, F7.D2.WP1, F9.D2.WP1 | ✅ COPERTA |
| FEAT-020 | Integrazioni enterprise e API di compatibilità | — | F6.D1.WP2, F8.D1.WP1, F8.D1.WP2, F8.D1.WP3, F9.D2.WP2, F10.D1.WP1, F10.D1.WP2, F10.D1.WP3 | ✅ COPERTA |

## traceability_matrix_requirements_or_epics

| requirement_id | requirement_name | related_features | covered_by_wbs_wps | coverage_status |
|---|---|---|---|---|
| FR-001 | Autenticazione federata INPS | FEAT-001 | F1.D2.WP1, F2.D1.WP1 | ✅ COPERTO |
| FR-002 | Selezione contesto operativo | FEAT-002 | F2.D1.WP3 | ✅ COPERTO |
| FR-003 | Autorizzazione per ruolo e sede | FEAT-003 | F1.D2.WP2, F2.D1.WP2 | ✅ COPERTO |
| FR-004 | Audit trail e timeout di sessione | FEAT-003 | F1.D2.WP1, F1.D2.WP2, F2.D1.WP2, F9.D1.WP1 | ✅ COPERTO |
| FR-005 | Ricerca multi-criterio pratica | FEAT-004 | F3.D1.WP1, F3.D2.WP1, F9.D2.WP1 | ✅ COPERTO |
| FR-006 | Disambiguazione domande e correlazioni anagrafiche | FEAT-005 | F3.D1.WP2, F3.D2.WP1 | ✅ COPERTO |
| FR-007 | Controlli di presa in carico pratica | FEAT-006 | F3.D1.WP3, F3.D2.WP2, F9.D2.WP1 | ✅ COPERTO |
| FR-008 | Prenotazione e normalizzazione della domanda | FEAT-006 | F3.D1.WP2, F3.D1.WP3, F3.D2.WP2 | ✅ COPERTO |
| FR-009 | Gestione quadri anagrafici e familiari | FEAT-007 | F4.D1.WP1, F4.D2.WP1 | ✅ COPERTO |
| FR-010 | Gestione quadri tecnico-economici | FEAT-008 | F4.D1.WP2, F4.D2.WP1 | ✅ COPERTO |
| FR-011 | Persistenza transazionale e ripresa del contesto | FEAT-009 | F1.D1.WP3, F4.D1.WP1, F4.D1.WP2, F4.D2.WP1 | ✅ COPERTO |
| FR-012 | Controlli dinamici, semafori e varianti per fondo | FEAT-010 | F4.D1.WP3, F4.D1.WP4, F4.D2.WP2, F9.D2.WP1 | ✅ COPERTO |
| FR-013 | Calcolo verify non definitivo | FEAT-011 | F5.D1.WP1, F5.D2.WP1, F9.D2.WP1 | ✅ COPERTO |
| FR-014 | Calcolo definitivo con consolidamento | FEAT-012 | F5.D1.WP2, F5.D2.WP1, F6.D1.WP2, F9.D2.WP1 | ✅ COPERTO |
| FR-015 | Routing motore fondo e prerequisiti di calcolo | FEAT-013 | F4.D1.WP4, F5.D1.WP1, F5.D1.WP2, F5.D1.WP3, F5.D1.WP4, F5.D1.WP5 | ✅ COPERTO |
| FR-016 | Esito calcolo e tracciabilità tecnica | FEAT-014 | F5.D1.WP1, F5.D1.WP2, F5.D1.WP3, F5.D1.WP4, F5.D1.WP5, F5.D2.WP1, F9.D1.WP1 | ✅ COPERTO |
| FR-017 | Produzione stampa finale PDF | FEAT-015 | F6.D1.WP1 | ✅ COPERTO |
| FR-018 | Aggiornamenti post-calcolo e documentali | FEAT-016 | F6.D1.WP2, F9.D1.WP1, F9.D2.WP1 | ✅ COPERTO |
| FR-019 | Consultazione e ristampa output | FEAT-017 | F6.D1.WP3 | ✅ COPERTO |
| FR-020 | Sblocco pratica e sblocco per cancellazione | FEAT-018 | F7.D1.WP1, F7.D2.WP1, F9.D2.WP1 | ✅ COPERTO |
| FR-021 | Riassegnazione e cambio stato domanda | FEAT-018 | F7.D1.WP2, F7.D2.WP1, F9.D2.WP1 | ✅ COPERTO |
| FR-022 | Configurazioni amministrative e bypass controlli | FEAT-019 | F7.D1.WP3, F7.D2.WP1, F9.D2.WP1 | ✅ COPERTO |
| FR-023 | Integrazione acquisizione con WebDom e ARCA | FEAT-020 | F3.D1.WP2, F8.D1.WP3 | ✅ COPERTO |
| FR-024 | Integrazione host legacy e sistemi downstream | FEAT-016, FEAT-020 | F6.D1.WP2, F8.D1.WP3 | ✅ COPERTO |
| FR-025 | Compatibilità contrattuale e API versionate | FEAT-020 | F1.D1.WP1, F1.D1.WP2, F6.D1.WP2, F8.D1.WP1, F8.D1.WP2, F9.D2.WP2, F10.D1.WP1, F10.D1.WP2, F10.D1.WP3 | ✅ COPERTO |

## traceability_matrix_components

| component_id | component_name | supported_features | covered_by_wbs_wps | coverage_status |
|---|---|---|---|---|
| comp-001 | Frontend Operativo Blazor Server | FEAT-001, FEAT-004, FEAT-007, FEAT-008, FEAT-011, FEAT-015, FEAT-018 | F1.D1.WP1, F2.D1.WP3, F3.D2.WP1, F3.D2.WP2, F4.D2.WP1, F4.D2.WP2, F5.D2.WP1, F6.D1.WP3, F7.D2.WP1, F9.D2.WP1, F9.D2.WP2 | ✅ COPERTO |
| comp-002 | API Gateway / BFF | FEAT-003, FEAT-016, FEAT-020 | F1.D1.WP1, F1.D1.WP2, F2.D1.WP1, F2.D1.WP2, F2.D1.WP3, F3.D1.WP1, F3.D2.WP1, F3.D2.WP2, F4.D2.WP1, F4.D2.WP2, F5.D1.WP1, F5.D1.WP2, F5.D2.WP1, F6.D1.WP2, F7.D2.WP1, F8.D1.WP1, F8.D1.WP2, F9.D1.WP1, F9.D2.WP1, F9.D2.WP2, F10.D1.WP1, F10.D1.WP2, F10.D1.WP3 | ✅ COPERTO |
| comp-003 | Servizio Autenticazione e Contesto | FEAT-001, FEAT-002, FEAT-003 | F1.D1.WP1, F1.D2.WP1, F1.D2.WP2, F2.D1.WP1, F2.D1.WP2, F2.D1.WP3, F3.D1.WP3, F9.D1.WP1, F9.D2.WP2 | ✅ COPERTO |
| comp-004 | Servizio Ricerca Pratiche | FEAT-004, FEAT-005, FEAT-006 | F3.D1.WP1, F3.D1.WP2, F3.D1.WP3, F3.D2.WP1, F3.D2.WP2, F7.D1.WP1, F7.D1.WP2, F8.D1.WP3, F9.D2.WP1, F9.D2.WP2, F10.D1.WP1, F10.D1.WP2 | ✅ COPERTO |
| comp-005 | Servizio Quadri Applicativi | FEAT-007, FEAT-008, FEAT-009, FEAT-010 | F1.D1.WP3, F4.D1.WP1, F4.D1.WP2, F4.D1.WP3, F4.D1.WP4, F4.D2.WP1, F4.D2.WP2, F7.D1.WP3, F9.D2.WP1, F9.D2.WP2, F10.D1.WP1, F10.D1.WP2 | ✅ COPERTO |
| comp-006 | Servizio Calcolo | FEAT-011, FEAT-012, FEAT-013, FEAT-014 | F1.D1.WP3, F5.D1.WP1, F5.D1.WP2, F5.D1.WP3, F5.D1.WP4, F5.D1.WP5, F5.D2.WP1, F8.D1.WP3, F9.D1.WP1, F9.D1.WP2, F9.D2.WP1, F9.D2.WP2, F10.D1.WP1, F10.D1.WP2 | ✅ COPERTO |
| comp-007 | Motore Calcolo FS | FEAT-013, FEAT-014 | F4.D1.WP4, F5.D1.WP3 | ✅ COPERTO |
| comp-008 | Motore Calcolo AGO | FEAT-013, FEAT-014 | F4.D1.WP4, F5.D1.WP4 | ✅ COPERTO |
| comp-009 | Motore Calcolo CI | FEAT-013, FEAT-014 | F4.D1.WP4, F5.D1.WP5 | ✅ COPERTO |
| comp-010 | Servizio Post-Calcolo e Stampa | FEAT-015, FEAT-016, FEAT-017 | F6.D1.WP1, F6.D1.WP2, F6.D1.WP3, F8.D1.WP3, F9.D1.WP1, F9.D1.WP2, F9.D2.WP1, F9.D2.WP2, F10.D1.WP2 | ✅ COPERTO |
| comp-011 | Servizio Amministrazione | FEAT-018, FEAT-019 | F1.D1.WP3, F1.D2.WP1, F1.D2.WP2, F2.D1.WP2, F4.D1.WP3, F4.D2.WP2, F7.D1.WP1, F7.D1.WP2, F7.D1.WP3, F7.D2.WP1, F9.D1.WP1, F9.D1.WP2, F9.D2.WP1, F9.D2.WP2, F10.D1.WP2 | ✅ COPERTO |
| comp-012 | Integration Gateway | FEAT-006, FEAT-016, FEAT-020 | F1.D1.WP1, F1.D1.WP2, F3.D1.WP2, F3.D1.WP3, F5.D1.WP2, F6.D1.WP1, F6.D1.WP2, F8.D1.WP1, F8.D1.WP2, F8.D1.WP3, F9.D1.WP1, F9.D1.WP2, F9.D2.WP1, F9.D2.WP2, F10.D1.WP1, F10.D1.WP2, F10.D1.WP3 | ✅ COPERTO |

## gap_analysis

### Uncovered scenari
- Nessun gap rilevato: tutti gli scenari SCN-001..SCN-012 sono coperti da almeno un Work Package.

### Uncovered feature
- Nessun gap rilevato: tutte le feature FEAT-001..FEAT-020 hanno almeno un Work Package assegnato.

### Uncovered requisiti
- Nessun gap rilevato: tutti i requisiti FR-001..FR-025 risultano tracciati verso la WBS.

### Uncovered componenti
- Nessun gap rilevato: tutti i componenti comp-001..comp-012 sono presi in carico da uno o più Work Package.

### Recommendations
- Mantenere la matrice aggiornata durante eventuali rigenerazioni delle Activity o ripianificazioni di cutover, soprattutto per FEAT-020 e FR-025 che si distribuiscono su più fasi.
- Usare la matrice come check-list di completamento prima del parallel run e del go-live progressivo.

COVERAGE_COMPLETED: 100% scenari, 100% feature, 100% requisiti, 100% componenti — 0 gap rilevati
