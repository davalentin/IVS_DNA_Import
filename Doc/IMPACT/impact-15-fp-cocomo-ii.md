# FP & COCOMO II - Progetto IVS_DNA

## Sezioni Principali
### 1. Function Points Calculation
La repository analysis non consente una conta IFPUG dettagliata transazione-per-transazione; è quindi stata usata una **stima backfiring** a partire dalle LOC C# hand-written.

### 2. Backfiring Method (LOC to FP)
**Assunzioni dichiarate:**
- Baseline LOC: **630.434** linee C# stimate hand-written.
- Rapporto C# ↔ FP: **53 LOC/FP** come valore medio legacy enterprise.
- Range sensibilità: 47-60 LOC/FP.

| Scenario | LOC/FP | FP stimati |
|---|---:|---:|
| Ottimistico | 47 | 13.413 |
| Base | 53 | 11.895 |
| Conservativo | 60 | 10.507 |

### 3. COCOMO II Models (Organic, Semi-detached, Embedded)
Per un sistema pensionistico integrato con host e vincoli enterprise, il profilo è più vicino a **semi-detached / embedded** che a organic.

| Profilo | Lettura |
|---|---|
| Organic | Poco realistico per IVS_DNA |
| Semi-detached | Base line per manutenzione evolutiva ampia |
| Embedded | Scenario più realistico per re-architecture con vincoli alti |

### 4. Effort Estimation (Person-Months)
Stima indicativa per tre scenari di trasformazione, non per singolo change request.

| Scenario | Scope | PM stimati |
|---|---|---:|
| A - Stabilizzazione + hardening | Security, config, osservabilità, test smoke | 120-180 |
| B - Modernizzazione incrementale per capability | Frontend nuovo + estrazione API capability by capability | 900-1.200 |
| C - Re-architecture ampia | Sostituzione progressiva di UI, orchestrazione e fondi | 1.300-1.700 |

### 5. Duration Estimation (Calendar Months)
| Scenario | Team medio | Durata stimata |
|---|---:|---:|
| A | 6-8 persone | 12-18 mesi |
| B | 12-16 persone | 24-32 mesi |
| C | 16-22 persone | 30-42 mesi |

### 6. Cost Estimation
Assunzione economica solo per ordine di grandezza: **10.000 € / PM** blended cost.

| Scenario | PM | Costo stimato |
|---|---:|---:|
| A | 120-180 | 1,2M€ - 1,8M€ |
| B | 900-1.200 | 9,0M€ - 12,0M€ |
| C | 1.300-1.700 | 13,0M€ - 17,0M€ |

### 7. Team Mix Proposal
| Ruolo | Scenario B/C |
|---|---:|
| Solution / enterprise architect | 1-2 |
| Business analyst dominio pensioni | 2-3 |
| Tech lead backend | 2 |
| Tech lead frontend | 1 |
| Developer backend | 4-8 |
| Developer frontend | 3-5 |
| QA / test automation | 2-4 |
| DevOps / platform | 1-2 |
| DBA / integration specialist | 1-2 |

### 8. Staffing Profile
- Avvio con discovery + hardening.
- Picco team durante estrazione capability e parallel run.
- Riduzione progressiva dopo dismissione moduli legacy più critici.

### 9. Note metodologiche
- Le stime sono **indicative** e vanno validate con business decomposition reale.
- Le LOC includono ancora una quota di generated code; le stime sono quindi conservative.
- La produttività reale dipenderà da accesso ambienti, disponibilità esperti dominio e dipendenze da sistemi esterni.

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
