using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaVarie
    {
        #region tracciato COBOL
        //    04  AREAVARIE.
        //* INVARIE.CPY
        //**********  LIVELLO DI AGGIORNAMENTO  LUGLIO 1990
        //**********  LUNGHEZZA AREA WKVARIE = 178 BYTES
        //          05  FILLER  PIC X.
        //         15  ICODRES      PIC XXX.
        //*+SIGLA STATO DI RESIDENZA
        //       10  DISPON1.
        //*                             PIC X(84).
        //         15 IALTRAPEN-DC.
        //* DATI ALTRA PENSIONE D.C.                     TOT. 18+5
        //             20 IAPCATEG-DC     PIC X(3).
        //*+CATEGORIA  ALTRA PENS.
        //             20 IAPENTE-DC      PIC X.
        //*+ENTE  ALTRA PENSIONE
        //             20 IAPUNIC-DC      PIC X.
        //*+ U = UNICO; C = +CONTITOLARI
        //             20 IAPCODIMP-DC    PIC 9.
        //*+CODICE IMPORTO ALTRA P.
        //*+DEC. ALTRA PENSIONE
        //               25 IAPDECORA-DC    PIC 9(4).
        //               25 IAPDECORM-DC    PIC 99.
        //*+CESS. ALTRA PENSIONE
        //               25  IAPCESSAA-DC    PIC 9(4).
        //               25  IAPCESSAM-DC    PIC 99.
        //         15  IAPIMPO-DC          PIC 9(7)V9(4) COMP-3.
        //*EURO +IMPORTO ALTRA PENSIONE
        //         15  IABSENT             PIC S9(5)V9(4) COMP-3.
        //*EURO  IMPORTO BENEFICIO SENT.495
        //         15  IW8REDCON-DC        PIC S9(7)V9(4) COMP-3.
        //*EURO  REDDITI DEL DANTE CAUSA X APPLICAZIONE SENT.495/93
        //         15  IIMPASSEST          PIC S9(5)V9(4) COMP-3.
        //*EURO  1996 IMPORTO Q.E. PER ASS.INVALIDITA'
        //         15 ICI2CRIS335          PIC S9(5)V9(4) COMP-3.
        //*EURO  1996 IMPORTO CRISTALIZZAZIONE DA GENNAIO 96 PER Q.E. NON AG
        //             20 IENTRACEEA            PIC 9(4).
        //             20 IENTRACEEM            PIC 99.
        //* 1996 DECORRENZA RICALCOLO IN CONV.12 PER CONV.9 E 20
        //* 2004 CAMPI SPOSTATI IN NELL'AREA STATI >> RESTANO 24 FILLER
        //         15 ALTRE-DATE.
        //* DECORRENZA BONUS 2004 PER ANZIANITA' CHE CONTINUA A LAVORARE
        //                25  IW1DEBONA        PIC 9999.
        //                25  IW1DEBONM        PIC 99.
        //*+DATA PRIMA DOMANDA
        //                25  IPRIMADAAA       PIC 9999.
        //                25  IPRIMADAMM       PIC 99.
        //                25  IPRIMADAGG       PIC 99.
        //*2007-CES. DIRITTO AUMENTO SOC.LG.544
        //                25  IW1CES544A      PIC 9(4).
        //                25  IW1CES544M      PIC 9(2).
        //            20  FILLER               PIC X(3).
        //         15 GP7LC19                  PIC X.
        //* 1996 RICALCOLO A RICHIESTA PER STATI SENZA SETT1 O SETT2
        //         15 ICI2335                  PIC 9999.
        //*+LIVELLO CRISTALLIZZAZIONE LG.335
        //* DATI OPERATORE
        //       10  DATI-OP.
        //* 1996 DEFINIZIONE IN BASE ALLA VERSIONE 'E' VALIDA DAL 01/1996:
        //        12  IRIGA-OP  OCCURS 4.
        //* RIGA IMPORTI CALCOLATI DA OPERATORE.      TOT.38 X 4 = 152
        //* AUMENTARE LA LUNGHEZZA DELL'ADEG E VIRT A 9
        //*+DEC.IMPORTI CALCOLATI OP
        //              20 IDECOPA  PIC 9999.
        //              20 IDECOPM  PIC 99.
        //         15  IADEGOP      PIC S9(7)V9(4) COMP-3.
        //*EURO +ADEGUATA CALCOLATA DA OP
        //         15  IVIRTOP      PIC 9(7)V9(4) COMP-3.
        //*EURO +VIRTUALE CALCOLATA DA OP
        //         15  IVINTOP      PIC 9(7)V9(4) COMP-3.
        //*EURO +VIRTUALE CALCOLATA DA OP
        //         15  IAUMFOP      PIC 9(5)V9(4) COMP-3.
        //*EURO +TOT. AUM.F. CALC.  DA OP
        //         15  IL1401OP     PIC 9(5)V9(4) COMP-3.
        //*EURO +IMP ART.3 4 5 E 8  DA OP
        //         15  IL1402OP     PIC 9(5)V9(4) COMP-3.
        //*EURO +IMP.ART 6 LG.140   DA OP
        //         15  ITOTSUP      PIC 9(7)V9(4) COMP-3.
        //*EURO +TOT.SUPP.CALCOLATI DA OP
        //*
        //*DEC. PRESCRIZIONE PENSIONE (DECORRENZA ARRETRATI PENSIONE)
        //            15 IDECARPENA      PIC 9(4).
        //            15 IDECARPENM      PIC 9(2).
        //*DEC. PRESCRIZIONE AA.FF.(DECORRENZA ARRETRATI AA.FF.)
        //            15 IDECARAFFA      PIC 9(4).
        //            15 IDECARAFFM      PIC 9(2).
        //       10 ICODVIRT             PIC X.
        //*CODICE VIRTUALE
        //       10 IDEL126              PIC X.
        //*DELIBERA 126 ( ANZIANITA' CON +780 IN ITALIA) S/N
        //       10 IVERSIONE            PIC X.
        //       10 IPRECONV.
        //           15 ICI2PRECONV      PIC 99.
        //* 1996 CODICE PRECEDENTE CONVENZIONE
        //       10 IW1C495              PIC X.
        //* APPLICAZIONE SENT.495/93:  0=NO  9=SI
        //      05  IALTREVARIE.
        //       10  ICI1INTRYU          PIC 9(5)V9(4) COMP-3.
        //*EURO IMPORTO INTEGRAZIONE A CARICO YUGOSLAVIA
        //           15  ICI1DECYUA      PIC 9(4).
        //           15  ICI1DECYUM      PIC 9(2).
        //       10  IW8DEC-DC           PIC 9(4).
        //* ANNO REDDITI DEL DANTE CAUSA X APPLICAZIONE SENT.495/93
        //       10  IW8RED-DC           PIC S9(7)V9(4) COMP-3.
        //*EURO  REDDITI DEL DANTE CAUSA X APPLICAZIONE SENT.495/93 IN MIGL.
        //       10  IW8NAT-DC.
        //           15  IW8NAT1-DC           PIC X.
        //           15  IW8NAT2-DC           PIC X.
        //           15  IW8NAT3-DC           PIC X.
        //* NATURA PENSIONE DEL DANTA CAUSA
        //       10  IREQ300996       PIC X.
        //*1997 REQUISITO 30.09.96 PER TRATTENUTE LAVORO
        //       10  NUOVA            PIC X.
        //*1997 PER RINNOVO: 1=LIQ. NELL'ULTIMO ANNO(PER SOSPENS.INT.X ETA')
        //       10  NONSOSP          PIC X.
        //*1997 PER RINNOVO: 1=PENS.INTEGRATA AL ULTIMO GP5 SENZA Q.E.
        //*
        //       10  IREQVE1294        PIC X.
        //* REQUISITO VECCHIAIA AL 31.12.94
        //       10  IREQPARD          PIC 9.
        //* REQUISITO PART. DIRITTO: 1/2/3 5/6/7/8
        //* DECORRENZA ASSEGNO DI INVALIDITA' SE REQPARD=7
        //         15 IDECASSA         PIC 9(4).
        //         15 IDECASSM         PIC 9(2).
        //       10 IADASS             PIC S9(7)V9(4) COMP-3.
        //*EURO  IMPORTO ADEGUATA ASSEGNO DI INVALIDITA' SE REQPARD=7
        //       10 IIMPASS            PIC S9(7)V9(4) COMP-3.
        //*EURO  IMPORTO TOTALE ASSEGNO DI INVALIDITA' SE REQPARD=7
        //*
        //       10 I-AREA-AGGANCIO.
        //            15 I-AGGANCIO              PIC X.
        //            15 I-CRIRIL                PIC 9.
        //            15 I-SETTEST               PIC 9999.
        //            15 I-VINTERA               PIC 9(7)V9(4)   COMP-3.
        //*EURO
        //            15 I-VIRT                  PIC 9(7)V9(4)   COMP-3.
        //*EURO
        //      15 I-ADEG                  PIC 9(7)V9(4)   COMP-3.
        //      *EURO
        //      *
        //           05 EX-INWKC2.
        //      *2007-AREA EX IMPORTI STATI ESTERI TOTALE 2748 BYTES
        //                 10  IBAN                  PIC X(34).
        //      *2007-CODICE IBAN DEL PENSIONATO
        //                 10  BIC                   PIC X(11).
        //      *2007-CODICE BIC DEL PENSIONATO
        //                 10 COD-C-OPERATIVO        PIC 99.                      
        //      *26/05/2008: CODICE CENTRO OPERATIVO                              
        //                 10 COD-PROCESSO           PIC 9(2).                    
        //      *26/05/2008: CODICE DEL PROCESSO                                  
        //      *                                                                 
        //cg2008           10 FILLER                 PIC X(2).
        //cg2008           10 ANNO-COMPETENZA        PIC 9(4).
        //      *
        //                 10 N-DOMUS-13                PIC X(13).
        //                 10 N-DOMUS-02                PIC X(02).
        //      *2007-Numero domanda DOMUS lng 15 char
        //                 10 PAESEPAG               PIC X.
        //      *PAESE DI PAGAMENTO DELLA PENSIONE: I=ITALIA; E=ESTERO
        //                 10 XDECRESDC.
        //                      20 DECRESDCA              PIC 9(4).
        //                      20 DECRESDCM              PIC 9(2).
        //                 10 DECRESDC REDEFINES XDECRESDC PIC 9(6).
        //      *2007-DECORRENZA RESIDENZA DANTE CAUSA
        //                 10 CITTDC                 PIC X(3).
        //      *2007-CITTADINANZA DANTE CAUSA
        //                 10 AN87A                  PIC XX.
        //      *2007-CODICE CESSASZIONE/RECESSO CONTRIB INPDAP : SI/NO
        //                 10 AN87DATA.
        //                   15 AN87DATAA           PIC 9(4).
        //                   15 AN87DATAM           PIC 9(2).
        //                 10 AN87D REDEFINES AN87DATA  PIC 9(6).
        //      *2007-DATA CESSAZIONE/RECESSO CONTRIB. INPDAP
        //                 10 RICPTUFF               PIC 9(3).
        //      *GP1RICPTUFF TIPO UFFICIO 9(3)
        //                 10 RICPCOD                PIC 9(3).
        //      *GP1RICPCOD CODICE ENTE DI PATRONATO 9(3)
        //                 10 RICPZON                PIC X(10).
        //      *GP1RICPZON CODICE UFF ZONALE ENTE DI PATRONATO X(10)
        //                 10 RICPNUM                PIC 9(8).
        //      *GP1RICPNUM NUMERO PRATICA DI PATRONATO 9(8)
        //      *                                                                 
        //cg2008           10 IW4-CODICI-DETRAZIONI.                              
        //cg2008               12 IW4-CODE OCCURS 15.                              
        //cg2008*CODICI DETRAZIONE IMPOSTA DI OGNI CONTITOLARE 12.09.2008         
        //cg2008                  15 IW4-CO1               PIC 9.                  
        //cg2008                  15 IW4-CO2               PIC 9.                  
        //cg2008                  15 IW4-CO3               PIC 9.                  
        //cg2008                  15 IW4-CO4               PIC 9.                  
        //cg2008                  15 IW4-CO5               PIC 9.                  
        //cg2008                  15 IW4-CO6               PIC 9.                  
        //cg2008                  15 IW4-CO7               PIC 9.                  
        //cg2008                  15 IW4-CO8               PIC 9.                  
        //cg2008                  15 IW4-CO9               PIC 9.                  
        //cg2008                  15 IW4-CO10              PIC 9.                  
        //cg2008                  15 IW4-CO11              PIC 9.                  
        //cg2008                  15 IW4-CO12              PIC 9.                  
        //cg2008                  15 IW4-CO13              PIC 9.                  
        //cg2008                  15 IW4-CO14              PIC 9. 

        //cg2009*29.10.2009: INSERITI NUOVI CAMPI PER COSTANTINO: TOTALE 196 BYTES
        //cg2009           10 ESEFIS-TERR        PIC XX.
        //cg2009*ESENZIONE FISCALE VITTIME TERRORISMO (SI/NO)
        //cg2009           10 ESEFIS-EST         PIC XX.
        //cg2009*ESENZIONE FISCALE ESTERO (SI/NO)
        //cg2009           10 T-ARCA-UNO         PIC XXX.
        //cg2009*CODICE ARCA-1 DEL TITOLARE
        //cg2009           10 T-ARCA-DUE         PIC 9(9).
        //cg2009*CODICE ARCA-2 DEL TITOLARE
        //cg2009           10 F-ARCA      OCCURS 15.
        //cg2009              15 F-ARCA-UNO      PIC XXX.
        //cg2009*CODICE ARCA-1 FAMILIARE
        //cg2009              15 F-ARCA-DUE      PIC 9(9).
        //cg2009*CODICE ARCA-2 FAMILIARE
        //cg2009*29.10.09  10 FILLER                 PIC X(2426).   -196                  
        //cg2010           10  TP1COFI-DC            PIC X(16).    
        //cg2009*21.02.10  10 codice fisc. dante causa   X(2230).   -16                   
        //cg2010*          10 FILLER                 PIC X(2214).  
        //cg2011           10 DAFELPE.                                 
        //cg2011            15  DAFELPE-DATA           PIC 9(8).                           
        //cg2011            15  DAFELPE-TIPCA          PIC X(1).                           
        //cg2011            15  DAFELPE-CPROV          PIC X(1).                           
        //cg2011*23.03.11 ---Dati prelievo da FELPE      2214      -10                   
        //cg2011*          10 FILLER                 PIC X(2204).  
        //cg2011           10 DECPERFREQ             PIC 9(8).
        //cg2011*+DEC. perfezionamento requisiti   28.8.2011
        //cg2011           10   DECPERFREQ-R  REDEFINES DECPERFREQ.
        //cg2011               15  DECPERFREQ-A             PIC 9999.
        //cg2011               15  DECPERFREQ-M             PIC 99.
        //cg2011               15  DECPERFREQ-G             PIC 99.
        //cg2011*23.03.11 ---Dati perf-requisito           2204  -8                   
        //cg2012***        10 FILLER                 PIC X(2196).  
        //cg2012           10 DATA-MATRIM           PIC 9(8).
        //cg2012***Data matrimonio       14.11.2011
        //cg2012           10   DATA-MATRIM-R  REDEFINES DATA-MATRIM.
        //cg2012               15  DATA-MATRIM-A             PIC 9999.
        //cg2012               15  DATA-MATRIM-M             PIC 99.
        //cg2012               15  DATA-MATRIM-G             PIC 99.
        //cg2012*14.11.11 ---Dati matrimonio             2196  -8 
        //cg2012*23.11.11 ---Dati arca d.c.              2188  -12 
        //cg2012           10 DC-ARCA-UNO         PIC XXX.
        //cg2012*CODICE ARCA-1 DEL DANTE CAUSA
        //cg2012           10 DC-ARCA-DUE         PIC 9(9).
        //cg2012*CODICE ARCA-2 DEL DANTE CAUSA

        //cg2012********   10 FILLER                 PIC X(2176).   
        //cg2012******************** usuranti ed oneri prepensionamento  3 + (47 x 8) = 379       
        //                 10 FELPE-TIPCERT                  PIC X(03).
        //      05	TABELLA-DATI-E211
        //                 10 ELEMENTO-E211 OCCURS 6
        //	                    15	C-PE-E211 PIC X
        //	                    15  CODICE-STATO-E211 PIC XX
        //	                    15	CODICE-ISTITUZ-E211 PIC XXX
        //      05  GP1TPCL PIC X(8)
        //cg2012*TIPCERT= "USR"  =  lavoratore usurante
        //          05 FILLER2018-1                         PIC X(332).
        //cg2012****       10 FILLER                 PIC X(1797).   
        //                 10  CAMPIQUOTAD.
        //      **2012** quota D  - contributiva(DATI RETRIBUTIVI PER CALCOLO CON DEC. DAL 01.02.1996  -17x4=68
        //                     15  ICISTOBG012          PIC 9(5)     COMP-3.
        //      * N. SETTIMANE DI CONTRIBUZIONE OBG
        //                     15  ICICONOBG012         PIC S9(9)V9(4)   COMP-3.
        //      *EURO AMMONTARE DEI CONTRIBUTI OBG
        //                     15  ICIRETOBG012         PIC S9(9)V9(4)   COMP-3.
        //      *EURO  MONTANTE OBG
        //                     15  ICISTCDM012          PIC 9(5)     COMP-3.
        //      * N. SETTIMANE DI CONTRIBUZIONE CDM
        //                     15  ICICONCDM012         PIC S9(9)V9(4)   COMP-3.
        //      *EURO AMMONTARE DEI CONTRIBUTI CDM
        //                     15  ICIRETCDM012         PIC S9(9)V9(4)   COMP-3.
        //      *EURO  MONTANTE CDM
        //                     15  ICISTART012          PIC 9(5)     COMP-3.
        //      * N. SETTIMANE DI CONTRIBUZIONE ART
        //                     15  ICICONART012         PIC S9(9)V9(4)   COMP-3.
        //      *EURO AMMONTARE DEI CONTRIBUTI ART
        //                     15  ICIRETART012         PIC S9(9)V9(4)   COMP-3.
        //      *EURO  MONTANTE ART
        //                     15  ICISTCOM012          PIC 9(5)     COMP-3.
        //      * N. SETTIMANE DI CONTRIBUZIONE COM
        //                     15  ICICONCOM012         PIC S9(9)V9(4)   COMP-3.
        //      *EURO AMMONTARE DEI CONTRIBUTI COM
        //                     15  ICIRETCOM012         PIC S9(9)V9(4)   COMP-3.
        //      *EURO  MONTANTE COM
        //cg2012           10 COD-RIDUZIONE          PIC X.
        //cg2012           10 PER-RIDUZIONE          PIC 99V99.                     
        //      *                                                                         
        //           05 IMPEST-IN-EURO.                                                   
        //                 10 IMPORTIL.
        //                  12 IMPORTIES OCCURS 30.
        //                   15 DECESTX.
        //                      20 DECESTLA              PIC 9(4).
        //                      20 DECESTLM              PIC 9(2).
        //                   15 DECESTL REDEFINES DECESTX PIC 9(6).
        //                   15 IMPESTL              PIC S9(5)V9(4) COMP-3.
        //      *EURO +IMPORTI ESTERI IN EURO PER RINNOVO
        //           05 VAR-SCIV OCCURS 10.
        //      *VARIAZIONI STATO CIVILE: DECIRRENZA E CODICE
        //               10 DECSCIV                   PIC 9(6).
        //               10 RDECSVIV REDEFINES DECSCIV.
        //                  15 DECSCIVA               PIC 9(4).
        //                  15 DECSCIVM               PIC 9(2).
        //               10 CODSCIV                   PIC X.
        #endregion tracciato COBOL

        #region Tracciato Host
        // 10   FILLER2020_1    PIC X(45).
        // *  sostituisce i dati residenza spostati 
        [HisFieldInfoMapping(0, 45)]
        public string FILLER2020_1 { get; set; }

        // 10  DISPON1.
        // *                             PIC X(84).
        // 15 IALTRAPEN-DC.
        // * DATI ALTRA PENSIONE D.C.                     TOT. 18+5
        /// <summary>
        /// IAPCATEG_DC X(3)  
        /// *+CATEGORIA  ALTRA PENS.
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string IAPCATEG_DC { get; set; }

        /// <summary>
        /// IAPENTE_DC X  
        /// *+ENTE  ALTRA PENSIONE
        /// </summary>
        [HisFieldInfoMapping(2, 1)]
        public string IAPENTE_DC { get; set; }

        /// <summary>
        /// IAPUNIC_DC X  
        /// *+ U = UNICO; C = +CONTITOLARI
        /// </summary>
        [HisFieldInfoMapping(3, 1)]
        public string IAPUNIC_DC { get; set; }

        /// <summary>
        /// IAPCODIMP_DC 9  
        /// *+CODICE IMPORTO ALTRA P.
        /// </summary>
        [HisFieldInfoMapping(4, 1)]
        public short IAPCODIMP_DC { get; set; }

        /// <summary>
        /// IAPDECORA_DC 9(4)  
        /// *+DEC. ALTRA PENSIONE
        /// </summary>
        [HisFieldInfoMapping(5, 4)]
        public short IAPDECORA_DC { get; set; }

        /// <summary>
        /// IAPDECORM_DC 99  
        /// *+DEC. ALTRA PENSIONE
        /// </summary>
        [HisFieldInfoMapping(6, 2)]
        public short IAPDECORM_DC { get; set; }

        /// <summary>
        /// IAPCESSAA_DC 9(4)  
        /// *+CESS. ALTRA PENSIONE
        /// </summary>
        [HisFieldInfoMapping(7, 4)]
        public short IAPCESSAA_DC { get; set; }

        /// <summary>
        /// IAPCESSAM_DC 99  
        /// *+CESS. ALTRA PENSIONE
        /// </summary>
        [HisFieldInfoMapping(8, 2)]
        public short IAPCESSAM_DC { get; set; }

        /// <summary>
        /// IAPIMPO_DC 9(7)V9(4) COMP-3 
        /// *EURO +IMPORTO ALTRA PENSIONE
        /// </summary>
        [HisFieldInfoMapping(9, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal IAPIMPO_DC { get; set; }

        /// <summary>
        /// IABSENT S9(5)V9(4) COMP-3 
        /// *EURO  IMPORTO BENEFICIO SENT.495
        /// </summary>
        [HisFieldInfoMapping(10, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IABSENT { get; set; }

        /// <summary>
        /// IW8REDCON_DC S9(7)V9(4) COMP-3 
        /// *EURO  REDDITI DEL DANTE CAUSA X APPLICAZIONE SENT.495/93
        /// </summary>
        [HisFieldInfoMapping(11, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IW8REDCON_DC { get; set; }

        /// <summary>
        /// IIMPASSEST S9(5)V9(4) COMP-3 
        /// *EURO  1996 IMPORTO Q.E. PER ASS.INVALIDITA'
        /// </summary>
        [HisFieldInfoMapping(12, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IIMPASSEST { get; set; }

        /// <summary>
        /// ICI2CRIS335 S9(5)V9(4) COMP-3 
        /// *EURO  1996 IMPORTO CRISTALIZZAZIONE DA GENNAIO 96 PER Q.E. NON AG
        /// </summary>
        [HisFieldInfoMapping(13, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICI2CRIS335 { get; set; }

        /// <summary>
        /// IENTRACEEA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(14, 4)]
        public short IENTRACEEA { get; set; }

        /// <summary>
        /// IENTRACEEM 99  
        /// </summary>
        [HisFieldInfoMapping(15, 2)]
        public short IENTRACEEM { get; set; }

        // * 1996 DECORRENZA RICALCOLO IN CONV.12 PER CONV.9 E 20
        // * 2004 CAMPI SPOSTATI IN NELL'AREA STATI >> RESTANO 24 FILLER
        // 15 ALTRE-DATE.
        /// <summary>
        /// IW1DEBONA 9999  
        /// * DECORRENZA BONUS 2004 PER ANZIANITA' CHE CONTINUA A LAVORARE
        /// </summary>
        [HisFieldInfoMapping(16, 4)]
        public short IW1DEBONA { get; set; }

        /// <summary>
        /// IW1DEBONM 99  
        /// * DECORRENZA BONUS 2004 PER ANZIANITA' CHE CONTINUA A LAVORARE
        /// </summary>
        [HisFieldInfoMapping(17, 2)]
        public short IW1DEBONM { get; set; }

        /// <summary>
        /// IPRIMADAAA 9999  
        /// *+DATA PRIMA DOMANDA
        /// </summary>
        [HisFieldInfoMapping(18, 4)]
        public short IPRIMADAAA { get; set; }

        /// <summary>
        /// IPRIMADAMM 99  
        /// *+DATA PRIMA DOMANDA
        /// </summary>
        [HisFieldInfoMapping(19, 2)]
        public short IPRIMADAMM { get; set; }

        /// <summary>
        /// IPRIMADAGG 99  
        /// *+DATA PRIMA DOMANDA
        /// </summary>
        [HisFieldInfoMapping(20, 2)]
        public short IPRIMADAGG { get; set; }

        /// <summary>
        /// IW1CES544A 9(4)  
        /// *2007-CES. DIRITTO AUMENTO SOC.LG.544
        /// </summary>
        [HisFieldInfoMapping(21, 4)]
        public short IW1CES544A { get; set; }

        /// <summary>
        /// IW1CES544M 9(2)  
        /// *2007-CES. DIRITTO AUMENTO SOC.LG.544
        /// </summary>
        [HisFieldInfoMapping(22, 2)]
        public short IW1CES544M { get; set; }

        /// <summary>
        /// FILLER X(3)  
        /// </summary>
        [HisFieldInfoMapping(23, 3)]
        public string FILLER1 { get; set; }

        /// <summary>
        /// GP7LC19 X(1)  
        /// </summary>
        [HisFieldInfoMapping(24, 1)]
        public string GP7LC19 { get; set; }

        /// <summary>
        /// ICI2335 9999  
        /// * 1996 RICALCOLO A RICHIESTA PER STATI SENZA SETT1 O SETT2
        /// </summary>
        [HisFieldInfoMapping(25, 4)]
        public short ICI2335 { get; set; }
        //  12  IRIGA-OP  OCCURS 4.
        [HisComplexAreaInfoMapping(26, ListCount = 4)]
        public List<DatiOperatore> DATIOPERATORE { get; set; }

        //            15 IDECARPENA      PIC 9(4).
        //            15 IDECARPENM      PIC 9(2).
        //*DEC. PRESCRIZIONE AA.FF.(DECORRENZA ARRETRATI AA.FF.)
        //            15 IDECARAFFA      PIC 9(4).
        //            15 IDECARAFFM      PIC 9(2).

        // *EURO +TOT.SUPP.CALCOLATI DA OP
        //*
        /// <summary>
        /// IDECARPENA 9(4)  
        /// *DEC. PRESCRIZIONE PENSIONE (DECORRENZA ARRETRATI PENSIONE)
        /// </summary>
        [HisFieldInfoMapping(27, 4)]
        public short IDECARPENA { get; set; }

        /// <summary>
        /// IDECARPENM 9(2)  
        /// *DEC. PRESCRIZIONE PENSIONE (DECORRENZA ARRETRATI PENSIONE)
        /// </summary>
        [HisFieldInfoMapping(28, 2)]
        public short IDECARPENM { get; set; }

        /// <summary>
        /// IDECARAFFA 9(4)  
        /// *DEC. PRESCRIZIONE AA.FF.(DECORRENZA ARRETRATI AA.FF.)
        /// </summary>
        [HisFieldInfoMapping(29, 4)]
        public short IDECARAFFA { get; set; }

        /// <summary>
        /// IDECARAFFM 9(2)  
        /// *DEC. PRESCRIZIONE AA.FF.(DECORRENZA ARRETRATI AA.FF.)
        /// </summary>
        [HisFieldInfoMapping(30, 2)]
        public short IDECARAFFM { get; set; }
        //-----------------
        /// <summary>
        /// ICODVIRT X  
        /// *CODICE VIRTUALE
        /// </summary>
        [HisFieldInfoMapping(31, 1)]
        public string ICODVIRT { get; set; }

        /// <summary>
        /// IDEL126 X  
        /// *DELIBERA 126 ( ANZIANITA' CON +780 IN ITALIA) S/N
        /// </summary>
        [HisFieldInfoMapping(32, 1)]
        public string IDEL126 { get; set; }

        /// <summary>
        /// IVERSIONE X  
        /// </summary>
        [HisFieldInfoMapping(33, 1)]
        public string IVERSIONE { get; set; }

        // 10 IPRECONV.
        /// <summary>
        /// ICI2PRECONV 99  
        /// * 1996 CODICE PRECEDENTE CONVENZIONE
        /// </summary>
        [HisFieldInfoMapping(34, 2)]
        public short ICI2PRECONV { get; set; }

        /// <summary>
        /// IW1C495 X  
        /// * APPLICAZIONE SENT.495/93:  0=NO  9=SI
        /// </summary>
        [HisFieldInfoMapping(35, 1)]
        public string IW1C495 { get; set; }

        // 05  IALTREVARIE.
        /// <summary>
        /// ICI1INTRYU 9(5)V9(4) COMP-3 
        /// *EURO IMPORTO INTEGRAZIONE A CARICO YUGOSLAVIA
        /// </summary>
        [HisFieldInfoMapping(36, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal ICI1INTRYU { get; set; }

        /// <summary>
        /// ICI1DECYUA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(37, 4)]
        public short ICI1DECYUA { get; set; }

        /// <summary>
        /// ICI1DECYUM 9(2)  
        /// </summary>
        [HisFieldInfoMapping(38, 2)]
        public short ICI1DECYUM { get; set; }

        /// <summary>
        /// IW8DEC_DC 9(4)  
        /// * ANNO REDDITI DEL DANTE CAUSA X APPLICAZIONE SENT.495/93
        /// </summary>
        [HisFieldInfoMapping(39, 4)]
        public short IW8DEC_DC { get; set; }

        /// <summary>
        /// IW8RED_DC S9(7)V9(4) COMP-3 
        /// *EURO  REDDITI DEL DANTE CAUSA X APPLICAZIONE SENT.495/93 IN MIGL.
        /// </summary>
        [HisFieldInfoMapping(40, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IW8RED_DC { get; set; }

        // 10  IW8NAT-DC.
        /// <summary>
        /// IW8NAT1_DC X  
        /// </summary>
        [HisFieldInfoMapping(41, 1)]
        public string IW8NAT1_DC { get; set; }

        /// <summary>
        /// IW8NAT2_DC X  
        /// </summary>
        [HisFieldInfoMapping(42, 1)]
        public string IW8NAT2_DC { get; set; }

        /// <summary>
        /// IW8NAT3_DC X  
        /// * NATURA PENSIONE DEL DANTA CAUSA
        /// </summary>
        [HisFieldInfoMapping(43, 1)]
        public string IW8NAT3_DC { get; set; }

        /// <summary>
        /// IREQ300996 X  
        /// *1997 REQUISITO 30.09.96 PER TRATTENUTE LAVORO
        /// </summary>
        [HisFieldInfoMapping(44, 1)]
        public string IREQ300996 { get; set; }

        /// <summary>
        /// NUOVA X  
        /// *1997 PER RINNOVO: 1=LIQ. NELL'ULTIMO ANNO(PER SOSPENS.INT.X ETA')
        /// </summary>
        [HisFieldInfoMapping(45, 1)]
        public string NUOVA { get; set; }

        /// <summary>
        /// NONSOSP X  
        /// *1997 PER RINNOVO: 1=PENS.INTEGRATA AL ULTIMO GP5 SENZA Q.E.
        /// </summary>
        [HisFieldInfoMapping(46, 1)]
        public string NONSOSP { get; set; }

        //*
        /// <summary>
        /// IREQVE1294 X  
        /// * REQUISITO VECCHIAIA AL 31.12.94
        /// </summary>
        [HisFieldInfoMapping(47, 1)]
        public string IREQVE1294 { get; set; }

        /// <summary>
        /// * REQUISITO PART. DIRITTO: 1/2/3 5/6/7/8
        /// IREQPARD 9  
        /// </summary>
        [HisFieldInfoMapping(48, 1)]
        public short IREQPARD { get; set; }

        /// <summary>
        /// IDECASSA 9(4)  
        /// * DECORRENZA ASSEGNO DI INVALIDITA' SE REQPARD=7
        /// </summary>
        [HisFieldInfoMapping(49, 4)]
        public short IDECASSA { get; set; }

        /// <summary>
        /// IDECASSM 9(2)  
        /// * DECORRENZA ASSEGNO DI INVALIDITA' SE REQPARD=7
        /// </summary>
        [HisFieldInfoMapping(50, 2)]
        public short IDECASSM { get; set; }

        /// <summary>
        /// IADASS S9(7)V9(4) COMP-3 
        /// *EURO  IMPORTO ADEGUATA ASSEGNO DI INVALIDITA' SE REQPARD=7
        /// </summary>
        [HisFieldInfoMapping(51, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IADASS { get; set; }

        /// <summary>
        /// IIMPASS S9(7)V9(4) COMP-3 
        /// *EURO  IMPORTO TOTALE ASSEGNO DI INVALIDITA' SE REQPARD=7
        /// </summary>
        [HisFieldInfoMapping(52, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IIMPASS { get; set; }

        //*
        // 10 I-AREA-AGGANCIO.
        /// <summary>
        /// I_AGGANCIO X  
        /// </summary>
        [HisFieldInfoMapping(53, 1)]
        public string I_AGGANCIO { get; set; }

        /// <summary>
        /// I_CRIRIL 9  
        /// </summary>
        [HisFieldInfoMapping(54, 1)]
        public short I_CRIRIL { get; set; }

        /// <summary>
        /// I_SETTEST 9999  
        /// </summary>
        [HisFieldInfoMapping(55, 4)]
        public short I_SETTEST { get; set; }

        /// <summary>
        /// I_VINTERA 9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(56, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal I_VINTERA { get; set; }

        // *EURO
        /// <summary>
        /// I_VIRT 9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(57, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal I_VIRT { get; set; }
        // *EURO
        /// <summary>
        /// I_ADEG 9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(58, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal I_ADEG { get; set; }

        // *EURO
        //*
        // 05 EX-INWKC2.
        // *2007-AREA EX IMPORTI STATI ESTERI TOTALE 2748 BYTES
        /// <summary>
        /// IBAN X(34)  
        /// *2007-CODICE IBAN DEL PENSIONATO
        /// </summary>
        [HisFieldInfoMapping(59, 34)]
        public string IBAN { get; set; }

        /// <summary>
        /// BIC X(11)  
        /// *2007-CODICE BIC DEL PENSIONATO
        /// </summary>
        [HisFieldInfoMapping(60, 11)]
        public string BIC { get; set; }

        /// <summary>
        /// COD_C_OPERATIVO 99  
        /// *26/05/2008: CODICE CENTRO OPERATIVO
        /// </summary>
        [HisFieldInfoMapping(61, 2)]
        public short COD_C_OPERATIVO { get; set; }

        /// <summary>
        /// COD_PROCESSO 9(2)  
        /// *26/05/2008: CODICE DEL PROCESSO
        /// </summary>
        [HisFieldInfoMapping(62, 2)]
        public short COD_PROCESSO { get; set; }

        /// <summary>
        /// COD_PROCEDURA X(1)  
        /// CODICE PROCEDURA
        /// </summary>
        [HisFieldInfoMapping(63, 1)]
        public string COD_PROCEDURA { get; set; }

        /// <summary>
        /// FILLER X(1)  
        /// </summary>
        [HisFieldInfoMapping(64, 1)]
        public string FILLER2 { get; set; }

        /// <summary>
        /// ANNO_COMPETENZA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(65, 4)]
        public short ANNO_COMPETENZA { get; set; }

        /// <summary>
        /// N_DOMUS_13 X(13)  
        /// Numero domanda
        /// </summary>
        [HisFieldInfoMapping(66, 13)]
        public string N_DOMUS_13 { get; set; }

        /// <summary>
        /// N_DOMUS_02 X(02)  
        /// Numero domanda
        /// </summary>
        [HisFieldInfoMapping(67, 2)]
        public string N_DOMUS_02 { get; set; }

        /// <summary>
        /// PAESEPAG X  
        /// *PAESE DI PAGAMENTO DELLA PENSIONE: I=ITALIA; E=ESTERO
        /// </summary>
        [HisFieldInfoMapping(68, 1)]
        public string PAESEPAG { get; set; }

        /// <summary>
        /// DECRESDCA 9(4)  
        /// *2007-DECORRENZA RESIDENZA DANTE CAUSA
        /// </summary>
        [HisFieldInfoMapping(69, 4)]
        public short DECRESDCA { get; set; }

        /// <summary>
        /// DECRESDCM 9(2)  
        /// *2007-DECORRENZA RESIDENZA DANTE CAUSA
        /// </summary>
        [HisFieldInfoMapping(70, 2)]
        public short DECRESDCM { get; set; }

        /// <summary>
        /// CITTDC X(3)  
        /// *2007-CITTADINANZA DANTE CAUSA
        /// </summary>
        [HisFieldInfoMapping(71, 3)]
        public string CITTDC { get; set; }

        /// <summary>
        /// AN87A XX  
        /// *2007-CODICE CESSASZIONE/RECESSO CONTRIB INPDAP : SI/NO
        /// </summary>
        [HisFieldInfoMapping(72, 2)]
        public string AN87A { get; set; }

        // 10 AN87DATA.
        /// <summary>
        /// AN87DATAA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(73, 4)]
        public short AN87DATAA { get; set; }

        /// <summary>
        /// AN87DATAM 9(2)  
        /// *2007-DATA CESSAZIONE/RECESSO CONTRIB. INPDAP
        /// </summary>
        [HisFieldInfoMapping(74, 2)]
        public short AN87DATAM { get; set; }

        /// <summary>
        /// RICPTUFF 9(3)  
        /// *GP1RICPTUFF TIPO UFFICIO 9(3)
        /// </summary>
        [HisFieldInfoMapping(75, 3)]
        public short RICPTUFF { get; set; }

        /// <summary>
        /// RICPCOD 9(3)  
        /// *GP1RICPCOD CODICE ENTE DI PATRONATO 9(3)
        /// </summary>
        [HisFieldInfoMapping(76, 3)]
        public short RICPCOD { get; set; }

        /// <summary>
        /// RICPZON X(10)  
        /// *GP1RICPZON CODICE UFF ZONALE ENTE DI PATRONATO X(10)
        /// </summary>
        [HisFieldInfoMapping(77, 10)]
        public string RICPZON { get; set; }

        /// <summary>
        /// RICPNUM 9(8)  
        /// *GP1RICPNUM NUMERO PRATICA DI PATRONATO 9(8)
        /// </summary>
        [HisFieldInfoMapping(78, 8)]
        public int RICPNUM { get; set; }


        //cg2008           10 IW4-CODICI-DETRAZIONI.                              
        //cg2008               12 IW4-CODE OCCURS 15.
        [HisComplexAreaInfoMapping(79, ListCount = 15)]
        public List<CodiciDetrazioni> CODICIDETRAZIONI { get; set; }

        //cg2009*29.10.2009: INSERITI NUOVI CAMPI PER COSTANTINO: TOTALE 196 BYTES
        //cg2009           10 ESEFIS-TERR        PIC XX.
        //cg2009*ESENZIONE FISCALE VITTIME TERRORISMO (SI/NO)
        //cg2009           10 ESEFIS-EST         PIC XX.
        //cg2009*ESENZIONE FISCALE ESTERO (SI/NO)
        //cg2009           10 T-ARCA-UNO         PIC XXX.
        //cg2009*CODICE ARCA-1 DEL TITOLARE
        //cg2009           10 T-ARCA-DUE         PIC 9(9).
        //cg2009*CODICE ARCA-2 DEL TITOLARE

        //cg2009*CODICE ARCA-2 FAMILIARE
        //cg2009*29.10.09  10 FILLER                 PIC X(2426).   -196                  
        //cg2010           10  TP1COFI-DC            PIC X(16).                           
        //cg2009*21.02.10  10 codice fisc. dante causa   X(2230).   -16                   
        //cg2010           10 FILLER                 PIC X(2214).         

        // cg2009*29.10.2009: INSERITI NUOVI CAMPI PER COSTANTINO: TOTALE 196 BYTES
        /// <summary>
        /// ESEFIS_TERR XX  
        /// cg2009*ESENZIONE FISCALE VITTIME TERRORISMO (SI/NO)
        /// </summary>
        [HisFieldInfoMapping(80, 2)]
        public string ESEFIS_TERR { get; set; }

        /// <summary>
        /// ESEFIS_EST XX  
        /// cg2009*ESENZIONE FISCALE ESTERO (SI/NO)
        /// </summary>
        [HisFieldInfoMapping(81, 2)]
        public string ESEFIS_EST { get; set; }

        /// <summary>
        /// T_ARCA_UNO XXX  
        /// cg2009*CODICE ARCA-1 DEL TITOLARE
        /// </summary>
        [HisFieldInfoMapping(82, 3)]
        public string T_ARCA_UNO { get; set; }

        /// <summary>
        /// T_ARCA_DUE 9(9)  
        /// </summary>
        [HisFieldInfoMapping(83, 9)]
        public int T_ARCA_DUE { get; set; }

        //cg2009           10 F-ARCA      OCCURS 15
        [HisComplexAreaInfoMapping(84, ListCount = 15)]
        public List<FArca> F_ARCA { get; set; }

        /// <summary>
        /// TP1COFI_DC X(16)  
        /// cg2009*21.02.10  10 codice fisc. dante causa   X(2230).   -16
        /// </summary>
        [HisFieldInfoMapping(85, 16)]
        public string TP1COFI_DC { get; set; }

        /// <summary>
        /// DAFELPE_DATA 9(8)  
        /// </summary>
        [HisFieldInfoMapping(86, 8)]
        public int DAFELPE_DATA { get; set; }

        /// <summary>
        /// DAFELPE_TIPCA X(1)  
        /// </summary>
        [HisFieldInfoMapping(87, 1)]
        public string DAFELPE_TIPCA { get; set; }

        /// <summary>
        /// DAFELPE_CPROV X(1)  
        /// </summary>
        [HisFieldInfoMapping(88, 1)]
        public string DAFELPE_CPROV { get; set; }

        /// <summary>
        /// DECPERFREQ_A 9(4)  
        /// </summary>
        [HisFieldInfoMapping(89, 4)]
        public short DECPERFREQ_A { get; set; }

        /// <summary>
        /// DECPERFREQ_M 9(2)  
        /// </summary>
        [HisFieldInfoMapping(90, 2)]
        public short DECPERFREQ_M { get; set; }

        /// <summary>
        /// DECPERFREQ_G 9(2)  
        /// </summary>
        [HisFieldInfoMapping(91, 2)]
        public short DECPERFREQ_G { get; set; }

        /// <summary>
        /// DATA-MATRIM-A 9(4)
        /// </summary>
        [HisFieldInfoMapping(92, 4)]
        public short DATA_MATRIM_A { get; set; }

        /// <summary>
        /// DATA-MATRIM-M 9(4)
        /// </summary>
        [HisFieldInfoMapping(93, 2)]
        public short DATA_MATRIM_M { get; set; }

        /// <summary>
        /// DATA-MATRIM-G 9(4)
        /// </summary>
        [HisFieldInfoMapping(94, 2)]
        public short DATA_MATRIM_G { get; set; }

        /// <summary>
        /// DC_ARCA_UNO X(3)  
        /// </summary>
        [HisFieldInfoMapping(95, 3)]
        public string DC_ARCA_UNO { get; set; }

        /// <summary>
        /// DC_ARCA_DUE 9(9)  
        /// </summary>
        [HisFieldInfoMapping(96, 9)]
        public int DC_ARCA_DUE { get; set; }

        /// <summary>
        /// FELPE_TIPCERT X(3)  
        /// </summary>
        [HisFieldInfoMapping(97, 3)]
        public string FELPE_TIPCERT { get; set; }

        [HisComplexAreaInfoMapping(98, ListCount = 6)]
        public List<E211> T_E211 { get; set; }

        /// <summary>
        /// GP1TPCLC X(8)
        /// </summary>
        [HisFieldInfoMapping(99, 8)]
        public string GP1TPCLC { get; set; }

        /// <summary>
        /// Q100_PUBBL X(2)
        /// </summary>
        [HisFieldInfoMapping(100, 2)]
        public string Q100_PUBBL { get; set; }

        //  11  IRESIDENZA OCCURS 20.
        //* VARIAZIONI DI RESIDENZA
        //*+DEC. VARIAZ. RESIDENZA
        [HisComplexAreaInfoMapping(101, ListCount = 20)]
        public List<DatiResidenza> DATIRESIDENZA { get; set; }

        /// <summary>
        /// IW1_SEDE_LAVO_METAPRO
        /// </summary>
        [HisFieldInfoMapping(102, 6)]
        public string IW1_SEDE_LAVO_METAPRO { get; set; }

        /// <summary>
        /// IW1_DES_SEDE_DOMANDA
        /// </summary>
        [HisFieldInfoMapping(103, 22)]
        public string IW1_DES_SEDE_DOMANDA { get; set; }


        /// <summary>
        /// FILLER2020_2 X(150)
        /// </summary>
        [HisFieldInfoMapping(104, 122)]
        public string FILLER2020_2 { get; set; }


        /// <summary>
        /// CAMPIQUOTAD.
        /// **2012** quota D  - contributiva(DATI RETRIBUTIVI PER CALCOLO CON DEC. DAL 01.02.1996  -17x4=68
        /// ICISTOBG012 9(5) COMP-3
        /// * N. SETTIMANE DI CONTRIBUZIONE OBG
        /// <summary>
        [HisFieldInfoMapping(105, 3, CobolType = CobolType.Comp3Unsigned)]
        public int ICISTOBG012 { get; set; }

        /// <summary>
        ///                     15  ICICONOBG012         PIC S9(9)V9(4)   COMP-3.
        ///      *EURO AMMONTARE DEI CONTRIBUTI OBG
        /// <summary>
        [HisFieldInfoMapping(106, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICICONOBG012 { get; set; }

        /// <summary>
        /// ICIRETOBG012 S9(9)V9(4)   COMP-3
        /// *EURO  MONTANTE OBG
        /// <summary>
        [HisFieldInfoMapping(107, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICIRETOBG012 { get; set; }

        /// <summary>
        /// ICISTCDM012 9(5)     COMP-3
        /// * N. SETTIMANE DI CONTRIBUZIONE CDM
        /// <summary>
        [HisFieldInfoMapping(108, 3, CobolType = CobolType.Comp3Unsigned)]
        public int ICISTCDM012 { get; set; }

        /// <summary>
        /// ICICONCDM012 S9(9)V9(4)   COMP-3
        /// *EURO AMMONTARE DEI CONTRIBUTI CDM
        /// <summary>
        [HisFieldInfoMapping(109, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICICONCDM012 { get; set; }

        /// <summary>
        /// ICIRETCDM012 S9(9)V9(4)   COMP-3
        /// *EURO  MONTANTE CDM
        /// <summary>
        [HisFieldInfoMapping(110, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICIRETCDM012 { get; set; }

        /// <summary>
        /// ICISTART012 9(5)     COMP-3
        /// * N. SETTIMANE DI CONTRIBUZIONE ART
        /// <summary>
        [HisFieldInfoMapping(111, 3, CobolType = CobolType.Comp3Unsigned)]
        public int ICISTART012 { get; set; }

        /// <summary>
        /// ICICONART012 S9(9)V9(4)   COMP-3
        /// *EURO AMMONTARE DEI CONTRIBUTI ART
        /// <summary>
        [HisFieldInfoMapping(112, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICICONART012 { get; set; }

        /// <summary>
        /// ICIRETART012 S9(9)V9(4)   COMP-3
        /// *EURO  MONTANTE ART
        /// <summary>
        [HisFieldInfoMapping(113, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICIRETART012 { get; set; }

        /// <summary>
        /// ICISTCOM012 9(5)     COMP-3
        /// * N. SETTIMANE DI CONTRIBUZIONE COM
        /// <summary>
        [HisFieldInfoMapping(114, 3, CobolType = CobolType.Comp3Unsigned)]
        public int ICISTCOM012 { get; set; }

        /// <summary>
        /// ICICONCOM012 S9(9)V9(4)   COMP-3
        /// *EURO AMMONTARE DEI CONTRIBUTI COM
        /// <summary>
        [HisFieldInfoMapping(115, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICICONCOM012 { get; set; }

        /// <summary>
        /// ICIRETCOM012 S9(9)V9(4)   COMP-3
        /// *EURO  MONTANTE COM
        /// <summary>
        [HisFieldInfoMapping(116, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICIRETCOM012 { get; set; }

        /// <summary>
        /// COD_RIDUZIONE X
        /// <summary>
        [HisFieldInfoMapping(117, 1)]
        public string COD_RIDUZIONE { get; set; }

        /// <summary>
        /// PER_RIDUZIONE 99V99
        /// <summary>
        [HisFieldInfoMapping(118, 4, Scale = 2, CobolType = CobolType.Unsigned)]
        public decimal PER_RIDUZIONE { get; set; }

        /// <summary>
        /// PEN_PROVV X
        /// <summary>
        [HisFieldInfoMapping(119, 1)]
        public string PEN_PROVV { get; set; }

        //  05 IMPEST-IN-EURO.                                                   
        //10 IMPORTIL.
        // 12 IMPORTIES OCCURS 30.
        [HisComplexAreaInfoMapping(120, ListCount = 30)]
        public List<ImportiEsteri> IMPORTIESTERI { get; set; }

        //05 VAR-SCIV OCCURS 10.
        [HisComplexAreaInfoMapping(121, ListCount = 10)]
        public List<VarStatiCivili> VARSTATICIVILI { get; set; }
        #endregion Tracciato Host

        #region Properties

        #endregion Properties

        #region nested class
        public class DatiResidenza
        {
            #region tracciato COBOL
            //             20  IDECRESAA    PIC 9(4).
            //             20  IDECRESMM    PIC 99.
            //             20  ICODRES XXX 
            #endregion tracciato COBOL

            #region Tracciato Host
            //             20  IDECRESAA    PIC 9(4).
            // *+SIGLA STATO DI RESIDENZA
            [HisFieldInfoMapping(0, 4)]
            public int IDECRESAA { get; set; }

            // 20  IDECRESMM    PIC 99.
            [HisFieldInfoMapping(1, 2)]
            public short IDECRESMM { get; set; }

            /// <summary>
            /// ICODRES XXX  
            /// </summary>
            [HisFieldInfoMapping(2, 3)]
            public string ICODRES { get; set; }
            #endregion Tracciato Host
        }

        public class DatiOperatore
        {
            #region tracciato COBOL
            //            * DATI OPERATORE
            //       10  DATI-OP.
            //* 1996 DEFINIZIONE IN BASE ALLA VERSIONE 'E' VALIDA DAL 01/1996:
            //        12  IRIGA-OP  OCCURS 4.
            //* RIGA IMPORTI CALCOLATI DA OPERATORE.      TOT.38 X 4 = 152
            //* AUMENTARE LA LUNGHEZZA DELL'ADEG E VIRT A 9
            //*+DEC.IMPORTI CALCOLATI OP
            //              20 IDECOPA  PIC 9999.
            //              20 IDECOPM  PIC 99.
            //         15  IADEGOP      PIC S9(7)V9(4) COMP-3.
            //*EURO +ADEGUATA CALCOLATA DA OP
            //         15  IVIRTOP      PIC 9(7)V9(4) COMP-3.
            //*EURO +VIRTUALE CALCOLATA DA OP
            //         15  IVINTOP      PIC 9(7)V9(4) COMP-3.
            //*EURO +VIRTUALE CALCOLATA DA OP
            //         15  IAUMFOP      PIC 9(5)V9(4) COMP-3.
            //*EURO +TOT. AUM.F. CALC.  DA OP
            //         15  IL1401OP     PIC 9(5)V9(4) COMP-3.
            //*EURO +IMP ART.3 4 5 E 8  DA OP
            //         15  IL1402OP     PIC 9(5)V9(4) COMP-3.
            //*EURO +IMP.ART 6 LG.140   DA OP
            //         15  ITOTSUP      PIC 9(7)V9(4) COMP-3.
            //*EURO +TOT.SUPP.CALCOLATI DA OP
            //*

            #endregion tracciato COBOL

            #region Tracciato Host
            // * DATI OPERATORE
            // 10  DATI-OP.
            // * 1996 DEFINIZIONE IN BASE ALLA VERSIONE 'E' VALIDA DAL 01/1996:
            // 12  IRIGA-OP  OCCURS 4.
            // * RIGA IMPORTI CALCOLATI DA OPERATORE.      TOT.38 X 4 = 152
            // * AUMENTARE LA LUNGHEZZA DELL'ADEG E VIRT A 9
            // *+DEC.IMPORTI CALCOLATI OP
            /// <summary>
            /// IDECOPA 9999  
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public short IDECOPA { get; set; }

            /// <summary>
            /// IDECOPM 99  
            /// </summary>
            [HisFieldInfoMapping(1, 2)]
            public short IDECOPM { get; set; }

            /// <summary>
            /// IADEGOP S9(7)V9(4) COMP-3 
            /// *EURO +ADEGUATA CALCOLATA DA OP
            /// </summary>
            [HisFieldInfoMapping(2, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal IADEGOP { get; set; }

            /// <summary>
            /// IVIRTOP 9(7)V9(4) COMP-3 
            /// *EURO +VIRTUALE CALCOLATA DA OP
            /// </summary>
            [HisFieldInfoMapping(3, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal IVIRTOP { get; set; }

            /// <summary>
            /// IVINTOP 9(7)V9(4) COMP-3 
            /// *EURO +VIRTUALE CALCOLATA DA OP
            /// </summary>
            [HisFieldInfoMapping(4, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal IVINTOP { get; set; }

            /// <summary>
            /// IAUMFOP 9(5)V9(4) COMP-3 
            /// *EURO +TOT. AUM.F. CALC.  DA OP
            /// </summary>
            [HisFieldInfoMapping(5, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal IAUMFOP { get; set; }

            /// <summary>
            /// IL1401OP 9(5)V9(4) COMP-3 
            /// *EURO +IMP ART.3 4 5 E 8  DA OP
            /// </summary>
            [HisFieldInfoMapping(6, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal IL1401OP { get; set; }

            /// <summary>
            /// IL1402OP 9(5)V9(4) COMP-3 
            /// *EURO +IMP.ART 6 LG.140   DA OP
            /// </summary>
            [HisFieldInfoMapping(7, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal IL1402OP { get; set; }

            /// <summary>
            /// ITOTSUP 9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(8, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal ITOTSUP { get; set; }

            #endregion Tracciato Host
        }

        public class CodiciDetrazioni
        {
            #region tracciato COBOL
            //cg2008                  15 IW4-CO1               PIC 9.                  
            //cg2008                  15 IW4-CO2               PIC 9.                  
            //cg2008                  15 IW4-CO3               PIC 9.                  
            //cg2008                  15 IW4-CO4               PIC 9.                  
            //cg2008                  15 IW4-CO5               PIC 9.                  
            //cg2008                  15 IW4-CO6               PIC 9.                  
            //cg2008                  15 IW4-CO7               PIC 9.                  
            //cg2008                  15 IW4-CO8               PIC 9.                  
            //cg2008                  15 IW4-CO9               PIC 9.                  
            //cg2008                  15 IW4-CO10              PIC 9.                  
            //cg2008                  15 IW4-CO11              PIC 9.                  
            //cg2008                  15 IW4-CO12              PIC 9.                  
            //cg2008                  15 IW4-CO13              PIC 9.                  
            //cg2008                  15 IW4-CO14              PIC 9. 
            #endregion tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// IW4_CO1 9  
            /// </summary>
            [HisFieldInfoMapping(0, 1)]
            public short IW4_CO1 { get; set; }

            /// <summary>
            /// IW4_CO2 9  
            /// </summary>
            [HisFieldInfoMapping(1, 1)]
            public short IW4_CO2 { get; set; }

            /// <summary>
            /// IW4_CO3 9  
            /// </summary>
            [HisFieldInfoMapping(2, 1)]
            public short IW4_CO3 { get; set; }

            /// <summary>
            /// IW4_CO4 9  
            /// </summary>
            [HisFieldInfoMapping(3, 1)]
            public short IW4_CO4 { get; set; }

            /// <summary>
            /// IW4_CO5 9  
            /// </summary>
            [HisFieldInfoMapping(4, 1)]
            public short IW4_CO5 { get; set; }

            /// <summary>
            /// IW4_CO6 9  
            /// </summary>
            [HisFieldInfoMapping(5, 1)]
            public short IW4_CO6 { get; set; }

            /// <summary>
            /// IW4_CO7 9  
            /// </summary>
            [HisFieldInfoMapping(6, 1)]
            public short IW4_CO7 { get; set; }

            /// <summary>
            /// IW4_CO8 9  
            /// </summary>
            [HisFieldInfoMapping(7, 1)]
            public short IW4_CO8 { get; set; }

            /// <summary>
            /// IW4_CO9 9  
            /// </summary>
            [HisFieldInfoMapping(8, 1)]
            public short IW4_CO9 { get; set; }

            /// <summary>
            /// IW4_CO10 9  
            /// </summary>
            [HisFieldInfoMapping(9, 1)]
            public short IW4_CO10 { get; set; }

            /// <summary>
            /// IW4_CO11 9  
            /// </summary>
            [HisFieldInfoMapping(10, 1)]
            public short IW4_CO11 { get; set; }

            /// <summary>
            /// IW4_CO12 9  
            /// </summary>
            [HisFieldInfoMapping(11, 1)]
            public short IW4_CO12 { get; set; }

            /// <summary>
            /// IW4_CO13 9  
            /// </summary>
            [HisFieldInfoMapping(12, 1)]
            public short IW4_CO13 { get; set; }

            /// <summary>
            /// IW4_CO14 9  
            /// </summary>
            [HisFieldInfoMapping(13, 1)]
            public short IW4_CO14 { get; set; }

            #endregion Tracciato Host
        }

        public class ImportiEsteri
        {
            #region tracciato COBOL
            //   20 DECESTLA              PIC 9(4).
            //   20 DECESTLM              PIC 9(2).
            //15 IMPESTL              PIC S9(5)V9(4) COMP-3.
            #endregion tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// DECESTLA 9(4)  
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public short DECESTLA { get; set; }

            /// <summary>
            /// DECESTLM 9(2)  
            /// </summary>
            [HisFieldInfoMapping(1, 2)]
            public short DECESTLM { get; set; }

            /// <summary>
            /// IMPESTL S9(5)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(2, 5, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal IMPESTL { get; set; }
            #endregion Tracciato Host
        }

        public class FArca
        {
            #region tracciato COBOL
            //cg2009              15 F-ARCA-UNO      PIC XXX.
            //cg2009*CODICE ARCA-1 FAMILIARE
            //cg2009              15 F-ARCA-DUE      PIC 9(9).
            // cg2009*CODICE ARCA-2 DEL TITOLARE
            // cg2009           10 F-ARCA      OCCURS 15.
            #endregion tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// F_ARCA_UNO XXX  
            /// cg2009*CODICE ARCA-1 FAMILIARE
            /// </summary>
            [HisFieldInfoMapping(82, 3)]
            public string F_ARCA_UNO { get; set; }

            /// <summary>
            /// F_ARCA_DUE 9(9)  
            /// cg2009*CODICE ARCA-2 FAMILIARE
            /// </summary>
            [HisFieldInfoMapping(83, 9)]
            public int F_ARCA_DUE { get; set; }
            #endregion Tracciato Host
        }

        public class VarStatiCivili
        {
            #region tracciato COBOL
            //   15 DECSCIVA               PIC 9(4).
            //   15 DECSCIVM               PIC 9(2).
            //10 CODSCIV                   PIC X.
            #endregion tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// DECSCIVA 9(4)  
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public short DECSCIVA { get; set; }

            /// <summary>
            /// DECSCIVM 9(2)  
            /// </summary>
            [HisFieldInfoMapping(1, 2)]
            public short DECSCIVM { get; set; }

            /// <summary>
            /// CODSCIV X  
            /// </summary>
            [HisFieldInfoMapping(2, 1)]
            public string CODSCIV { get; set; }
            #endregion Tracciato Host
        }

        public class E211
        {
            #region tracciato COBOL
            //      05	TABELLA-DATI-E211
            //                 10 ELEMENTO-E211 OCCURS 6
            //	                    15	C-PE-E211 PIC X
            //	                    15  CODICE-STATO-E211 PIC XX
            //	                    15	CODICE-ISTITUZ-E211 PIC XXX

            #endregion tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// C-PE-E211 X  
            /// </summary>
            [HisFieldInfoMapping(0, 1)]
            public string C_PE_E211 { get; set; }

            /// <summary>
            /// CODICE-STATO-E211 XX  
            /// </summary>
            [HisFieldInfoMapping(1, 2)]
            public string CODICE_STATO_E211 { get; set; }

            /// <summary>
            /// CODICE-ISTITUZ-E211 XXX  
            /// </summary>
            [HisFieldInfoMapping(2, 3)]
            public string CODICE_ISTITUZ_E211 { get; set; }
            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
