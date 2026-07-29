# Software Architecture - Progetto IVS_DNA

## Sezioni Principali
### 1. Architecture Style & Patterns
- **Style dominante:** layered SOA intra-enterprise.
- **Frontend pattern:** WebForms + MVP.
- **Backend pattern:** orchestratore centrale + servizi specializzati per fondo.
- **Data access pattern:** repository/DAO legacy con LINQ-to-SQL e stored procedure.
- **Integration pattern:** synchronous RPC via WCF SOAP / custom binding.

### 2. Containers & Technology Choices
| Container | Tecnologia | Responsabilità |
|---|---|---|
| PN809 | ASP.NET WebForms | UI operatore, navigazione, orchestrazione presenter |
| PN812 | WCF service | Facade/orchestratore e capability comuni |
| PN813 | WCF service | Domini fondo FS |
| PN815 | WCF service | Domini fondo AGO |
| PN818 | WCF service | Domini fondo CI |
| SQL Server | RDBMS | Persistenza pensioni, quadri, anagrafiche, decodifiche |
| DB2/Host | DB2OLEDB/CICS | Oneri e dati legacy integrati |

### 3. Component Diagram
```mermaid
graph TB
    subgraph Frontend
        Page[ASPX Page]
        View[IView]
        Pres[Presenter]
        UCs[User Controls]
    end
    subgraph Orchestrator
        SVC[ServizioLiquidazione.svc]
        Contract[IServizioLiquidazione + Area*]
        BL[Gestione* PN812]
        DCOMMON[DataCommon / Data]
    end
    subgraph Funds
        FS[PN813 FS]
        AGO[PN815 AGO]
        CI[PN818 CI]
    end
    subgraph External
        SQL[(SQL Server)]
        DB2[(DB2 / Host)]
        EXT[ARCA / WebDom / SAI / ANF / SCRIWO]
    end

    Page --> View
    View --> Pres
    UCs --> Page
    Pres --> SVC
    SVC --> Contract
    SVC --> BL
    BL --> DCOMMON
    BL --> FS
    BL --> AGO
    BL --> CI
    DCOMMON --> SQL
    BL --> EXT
    FS --> SQL
    AGO --> SQL
    CI --> SQL
    BL --> DB2
```

### 4. Deployment Diagram
```mermaid
flowchart LR
    User[Browser intranet] --> IIS1[IIS - PN809]
    IIS1 --> NETTCP[NetTcp / HTTP WCF]
    NETTCP --> IIS2[IIS - PN812]
    IIS2 --> IIS3[IIS - PN813]
    IIS2 --> IIS4[IIS - PN815]
    IIS2 --> IIS5[IIS - PN818]
    IIS2 --> SQL[(SQL Server)]
    IIS2 --> DB2[(DB2 Host)]
    IIS2 --> API[API interne / servizi enterprise]
```

### 5. Integration Architecture
| Flusso | Pattern |
|---|---|
| PN809 → PN812 | Presenter → generated proxy WCF |
| PN812 → PN813/815/818 | BL orchestrator → proxy WCF generato |
| PN812 / fondi → DB | LINQ-to-SQL / SP / DataContext |
| PN812 → sistemi enterprise | SOAP/WCF, HTTP intranet, host DB2 |
| Logging | Logger DNA + log SOAP + log generici |

### 6. Architectural Risks Mitigation
| Rischio | Mitigazione esistente | Gap |
|---|---|---|
| Errore su fondo specifico | Separazione moduli PN813/815/818 | Orchestratore resta SPOF logico |
| Cambi config ambiente | Varianti `CHG*` | Processo manuale e fragile |
| Errori di business | `AreaEsito`, controlli dinamici, semafori | Poche metriche oggettive |
| Consistenza dati | `TransactionScope` | Nessuna evidenza di saga/distributed transaction |
| Eterogeneità integrazioni | Proxy e mapping dedicati | Alto costo di manutenzione |

### 7. Architectural Notes
- L’architettura non è microservizi cloud-native: è più corretto definirla **monolite distribuito per domini di fondo**.
- La qualità principale del design è la chiara separazione del perimetro funzionale per fondo.
- Il principale difetto è la centralizzazione eccessiva di contratti/orchestrazione in PN812, che amplifica l’impatto di qualsiasi change.

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
