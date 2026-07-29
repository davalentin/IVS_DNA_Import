# Constraints - Progetto IVS_DNA

## Sezioni Principali
### 1. Time, Budget, Resources
| Vincolo | Stato |
|---|---|
| Budget di programma | TBD |
| Time-to-market | TBD |
| Numero team / capacity | TBD |
| Continuità operativa durante la modernizzazione | Vincolo implicito alto, dato il dominio pensionistico |

### 2. Technology Constraints
| Vincolo | Evidenza | Impatto |
|---|---|---|
| `.NET Framework 3.5` | File `.csproj` | Richiede toolchain legacy Windows |
| WCF / WebForms | Struttura PN809/PN812/PN813/PN815/PN818 | Bassa portabilità verso stack moderni |
| LINQ-to-SQL / DBML | `Pensioni.dbml`, `WebDom.dbml`, `CIBase.dbml` | Forte coupling schema-codice |
| Framework proprietario `INPS.DNA` | Riferimenti assembly e config | Vendor/platform lock-in interno |
| ReportViewer 9 | Config PN809 | Dipendenza da tecnologia reporting legacy |

### 3. Existing Systems & Reuse
- Riutilizzo obbligato dei sistemi enterprise esistenti: WebDom, ARCA, SAI, ANF, FELPE, INPDAP, SCRIWO, host DB2.
- I moduli fondo non sono indipendenti dal modello di dominio centrale gestito in PN812.
- Le DataContract `Area*` e il contratto `IServizioLiquidazione` costituiscono un vincolo di retrocompatibilità.

### 4. Integration Standards
| Standard / paradigma | Osservazione |
|---|---|
| WCF SOAP | Standard principale di integrazione applicativa |
| customBinding / netTcp / netPipe | Pattern di trasporto intra-enterprise |
| DTO `Area*` + `AreaEsito` | Convenzione comune di interoperabilità |
| `ControlliDinamici` | Standard interno per abilitazioni/regole runtime |
| DB2 OLEDB | Vincolo tecnologico per integrazioni host |

### 5. Team Size & Skills
Competenze minime richieste per operare sul sistema:
- C# legacy e .NET Framework 3.5.
- ASP.NET WebForms e MVP.
- WCF, Service Contracts, generated proxy.
- SQL Server, LINQ-to-SQL e stored procedure.
- Conoscenza dominio pensioni e differenze FS/AGO/CI.
- Conoscenza ecosistema INPS.DNA e sistemi interni.

**Disponibilità effettiva di tali skill nel team attuale:** TBD.

### 6. Legal Constraints (GDPR, Accessibility, Licensing, Log Retention)
| Vincolo | Stato |
|---|---|
| GDPR / dati personali | Implicito e rilevante, ma policy formali non nel repo |
| Accessibilità | Nessuna evidenza di WCAG testata; probabile gap |
| Licenze terze parti | N/A dal repository, salvo DLL incluse localmente |
| Retention log | TBD |
| Audit operazioni sensibili | Requisito esplicito in documentazione tecnica |

### 7. Constraint Summary
1. **Vincoli fortemente tecnologici**: toolchain e runtime legacy non facilmente eseguibili su ambienti moderni.
2. **Vincoli architetturali**: necessità di preservare comportamento e contratti per fondi diversi.
3. **Vincoli operativi**: alta criticità del dominio e dipendenza da sistemi enterprise esterni.
4. **Vincoli di sicurezza/compliance**: necessità di bonifica secret e protezione dati senza interrompere il servizio.

## Reference Documents
- `/root/IVS_DNA/README.md`
- `/root/IVS_DNA/Doc/Legacy_IVS_AnalisiTecnica.md`
- `/root/IVS_DNA/Doc/IVS_Onboarding_Tecnico.md`
- `/root/IVS_DNA/Doc/IVS_Requisiti_Tecnici_Approfonditi.md`
- `/root/IVS_DNA/Doc/Manuale_utente.md`
- `PN809/LiquidazionePensioniFS/Web.config`
- `PN812/WSInpsPensioniLiquidazione/Web.config`
- `PN812/WSInpsPensioniLiquidazione/Contracts/ServiceContracts/IServizioLiquidazione.cs`

## Change Log
- 2026-06-24 — Documento generato da analisi del repository, dei file di configurazione e della documentazione tecnica esistente.
