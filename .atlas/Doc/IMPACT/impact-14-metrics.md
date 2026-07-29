# Metrics - Progetto IVS_DNA

## Sezioni Principali
### 1. Total Lines of Code (SLOC)
| Misura | Valore | Note |
|---|---:|---|
| Comando richiesto dal committente (`find | xargs wc -l`) | 295.016 | Sottostima dovuta a path con spazi |
| Safe count null-delimited | 667.010 | Conteggio affidabile di tutti i `.cs` |
| Hand-written estimate | 630.434 | Esclusi proxy generati, `bin/obj`, `*.designer.cs` |

### 2. Language Distribution
| Estensione | File | Osservazioni |
|---|---:|---|
| `.cs` | 2.007 | Dominante |
| `.aspx` | 112 | Frontend pagine |
| `.ascx` | 190 | Frontend user control |
| `.config` | 41 | Config ambiente e runtime |
| `.csproj` | 34 | Soluzione multi-progetto |
| `.sln` | 5 | Una per macro-modulo |
| `.dbml` | 4 | Modellazione dati |
| `.js` | 15 | JS limitato |
| `.css` | 20 | Theme / stile UI |

### 3. Complexity Metrics
| Indicatore | Valore | Lettura |
|---|---:|---|
| OperationContract PN812 | 252 | Contratto centralizzato molto ampio |
| Session usages PN809 | 1.805 | Stato applicativo disperso |
| ViewState usages PN809 | 3.344 | Forte dipendenza da WebForms |
| File con TransactionScope | 252 | Persistenza e business logic transazionali |
| TODO/FIXME/HACK | 150 | Debito tecnico esplicito |

### 4. Module/Component Count
| Modulo | File C# | LOC stimate hand-written |
|---|---:|---:|
| PN809 | 788 | 169.728 |
| PN812 | 616 | 185.701 |
| PN813 | 209 | 113.906 |
| PN815 | 212 | 91.464 |
| PN818 | 182 | 69.635 |

### 5. Dependency Metrics
| Indicatore | Valore |
|---|---:|
| Cartelle di service references backend | 12 |
| Generated client files PN812 BL | 75 |
| Generated client files PN813 BL | 8 |
| Generated client files PN815 BL | 17 |
| Generated client files PN818 BL | 9 |

### 6. Code Quality Metrics
| Metrica qualitativa | Valutazione |
|---|---|
| Coesione moduli business | Media |
| Accoppiamento orchestratore-contratti | Alto |
| Testabilità frontend | Bassa |
| Testabilità backend | Media-bassa |
| Sicurezza configurazione | Bassa |
| Portabilità stack | Molto bassa |

### 7. Test Coverage Metrics
| Misura | Valore |
|---|---|
| File test unitari | 42 |
| File service test | 6 |
| Coverage percentuale reale | TBD |
| UI automation | N/A |
| Integration/E2E automation | N/A |

### 8. Largest Files / Hotspots
| File | Indicazione |
|---|---|
| `Pensioni.designer.cs` | DBML generated code molto voluminoso |
| Generated client WCF (`DatiPensioniClient.cs`, `WebDomClient.cs`, ecc.) | Peso significativo del codice generato |
| `ServizioLiquidazione.svc.cs` | Facade centrale molto estesa |
| `GestioneCalcoloDomanda.cs` | Hotspot di regole business e decisioni runtime |

### 9. Metric Interpretation
Le metriche indicano un sistema grande ma ancora trattabile se affrontato per capability. I valori più critici non sono tanto le LOC assolute, quanto l’ampiezza del contratto WCF, il debito di stato lato UI e la superficie di configurazione/integrazione.

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
