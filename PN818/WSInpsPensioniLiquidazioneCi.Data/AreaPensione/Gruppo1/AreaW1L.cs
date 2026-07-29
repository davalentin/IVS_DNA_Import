using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaW1L
    {
        #region tracciato COBOL
        //    04  AREAW1L.
        //* TS1WK1AA.CPY
        //**********  LIVELLO DI AGGIORNAMENTO  LUGLIO 1990
        //**********  LUNGHEZZA AREA WK1 = 160 BYTES
        //     05  IW1TIPEL            PIC A.
        //*+COD.TIPO ELABORAZIONE
        //*+DATA NASCITA DEL TITOL.
        //         10  IW1SECAN        PIC 9999.
        //         10  IW1NATITM       PIC 99.
        //         10  IW1NATITG       PIC 99.
        //* DATA DI NASCITA CONTITOLARE PIU' ANZIANO
        //         10  IW1NAANZA       PIC 9999.
        //         10  IW1NAANZM       PIC 99.
        //         10  IW1NAANZG       PIC 99.
        //*+DEC. ORIGINARIA PENSIONE
        //              15  IW1DEOSEC           PIC 99.
        //              15  IW1DEOAA            PIC 99.
        //         10  IW1DEORM        PIC 99.
        //* DECORRENZA OPZIONE
        //         10  IW1DEOPA        PIC 9999.
        //         10  IW1DEOPM        PIC 99.
        //* DECORRENZA RIPRISTINO
        //         10  IW1DERIPA       PIC 9999.
        //         10  IW1DERIPM       PIC 99.
        //*+DEC. PENSIONE DIRETTA
        //         10  IW1DEDIRA       PIC 9999.
        //         10  IW1DEDIRM       PIC 99.
        //     05  IW1DANTE.
        //* DATI DEL DANTE CAUSA
        //         10  IW1DPROV        PIC 9.
        //*+PROVENIENZA DANTE C. 0=DI ASS. 1=DA VO 2=DA IO
        //         10  IW1DSES         PIC X.
        //*+SESSO DANTE CAUSA
        //*+DATA DI NASCITA D.C.
        //             15  IW1DNASA    PIC 9999.
        //             15  IW1DNASM    PIC 99.
        //             15  IW1DNASG    PIC 99.
        //*+DATA DI MORTE D.C.
        //             15  IW1DMORA    PIC 9999.
        //             15  IW1DMORM    PIC 99.
        //             15  IW1DMORG    PIC 99.
        //         10  IW1780CD            PIC 9.
        //*+PEN.DIR.+780: 4=NO,3=SI
        //     05  IW1SESTIT           PIC X.
        //*+SESSO DEL TITOLARE
        //     05  IW1CATPEN           PIC 999.
        //*+CAT.PENS.IN CODICE P18
        //     05  IW1CODC             PIC 9.
        //* CODICE CIECO
        //     05  IW1CODEX            PIC 9.
        //*+DEC. ART.6,LG.140/85
        //         10  IW1DECEXA       PIC 9(4).
        //         10  IW1DECEXM       PIC 99.
        //*+CODICE EX-COMBAT.(0/1)
        //     05  IW1COSCIS           PIC 9.
        //* CODICE SCISSIONE
        //     05  IW1CRIRIL           PIC 9.
        //* CODICE PENSIONE RILIQUIDATA PER CRISTALLIZZAZIONE
        //* 0 = PENSIONE NON CRISTALLIZATA
        //* 1 = PENSIONE DIRETTA RILIQUIDATA PER CRISTALL. 1983
        //* 2 = PENSIONE DIRETTA RILIQUIDATA PER CRISTALL. POST 1983
        //* 3 = PENSIONE SUPERST RILIQUIDATA PER CRISTALL. 1983
        //* 4 = PENSIONE SUPERST RILIQUIDATA PER CRISTALL. POST 1983
        //     05  IW1CARIC            PIC 9.
        //*+CODICE CAUSA CARICO
        //     05  IW1TIPINV           PIC 9.
        //*+222:1=ASS.INV. 2=P.INAB.
        //     05  IW1ELAB             PIC 9.
        //*+T. ELAB. 0=NL/RC 1=RIN
        //     05  IW1CMS1             PIC 9.
        //*+1=MAG.SOC.MINIMI ART.1
        //*+DEC. MAG. SOC.ART.1/140
        //         10  IW1DECMS1A      PIC 9(4).
        //         10  IW1DECMS1M      PIC 99.
        //*+DEC.AUMENTO SOC.LG.544
        //         10  IW1DEC544A      PIC 9(4).
        //         10  IW1DEC544M      PIC 9(2).
        //     05  IW1IDENT.
        //* IDENTIFICATIVO PENSIONE (SEDE E CERTIFICATO)
        //         10  IW1SESEZ        PIC 9(4).
        //*+IDENT.PENSIONE:(SEDE)
        //         10  X-IW1CERT.
        //             15  IW1CERT         PIC 9(8).
        //*+IDENT.PENSIONE:(CERTIF)
        //     05  IW1CAT8             PIC X(8).
        //*+CAT.ALFABETICA (8 CAR)
        //     05  IW1CALSO            PIC 9.
        //* 1 = CALCOLO DIRETTA DI PROVENIENZA DELLA PENSIONE REVERSIBILITA'
        //* 0 = CALCOLO DELLA PENSIONE DI REVERSIBILITA'
        //     05  IW1NOAF86           PIC 9.
        //* SEGNALATORE PRESENZA CONIUGE
        //* 0=CONIUGE MAI A CARICO
        //* 1=CONIUGE A CARICO  9=PRESENTE X CALCOLO SENZA ALCUN DIRITTO
        //     05  IW1CODCAT           PIC X.
        //*+0=MI 1=OB 2=CD 3=AR 4=C0
        //     05  IW1DIRSUP           PIC 9.
        //*+1=VEC 2=INV 3=IND 4=REV
        //     05  IW1DATE.
        //*+DATA INIZIO CALCOLO
        //              15 IW1DA1A     PIC 9(4).
        //              15 IW1DA1M     PIC 9(2).
        //*+DATA FINE CALCOLO
        //              15 IW1DA2A     PIC 9(4).
        //              15 IW1DA2M     PIC 9(2).
        //     05  IW1MENS0            PIC X.
        //* INDICATORE MENSILE = 0
        //* 1 = PENSIONE NON CALCOLABILE PER ASSENZA DEI DATI ELEMENTARI
        //*     (SETTIMANE DI ANZIANITA'CONTRIBUTIVA, RETRIBUZIONE MEDIA
        //*     SETTIMANALE, IMPORTO DELLA PENSIONE BASE, ECC...)
        //* 2 = PENSIONE AI SUPERSTITI NON CALCOLABILE ALLA DECORRENZA
        //*     ORIGINARIA PER ASSENZA DI CONTITOLARI
        //* 3 = PENSIONE INTEGRABILE AL TRATTAMENTO MINIMO SENZA
        //*     SEGNALAZIONE DEL REDDITO
        //* 4 = PENSIONE DI IMPORTO SUPERIORE AL TRATTAMENTO MINIMO MA
        //*     INFERIORE AL TRATTAMENTO MINIMO MAGGIORATO PER 781
        //*     CONTRIBUTI SENZA SEGNALAZIONE DEL REDDITO
        //* 5 = PENSIONE INTEGRABILE AL TRATTAMENTO MINIMO CON CCC=8
        //* 6 = PENSIONE DEI LAVORATORI AUTONOMI SENZA LA SEGNALAZIONE
        //*     DEL NUMERO DI SETTIMANE DI ANZIANITA'CONTRIBUTIVA
        //* 7 = PENSIONE SEGNALATA CON CONTRIBUZIONE SUPERIORE A 781
        //*     SENZA IL NUMERO DI SETTIMANE. SCARTO PER NON ATTRIBUZIONE
        //*     ARTICOLO 4 DELLA 140
        //* 8 = PENSIONE CON RETRIBUZIONE MEDIA SETTIMANALE DI IMPORTO
        //*     SUPERIORE AL LIMITE MASSIMO CONSENTITO
        //* 9 = PENSIONE CON IMPORTO MENSILE MAGGIORE DI 9900000
        //* A = PENSIONE CON PIU'DI 130 RICORRENZE DI CALCOLO
        //* B = ANOMALIE IN DATI REDDITUALI PER TRATTAMENTO DI FAMIGLIA
        //     05  IW1ESTCUM            PIC 9.
        //*+0(=POST 1.1.91)/1(=SI)/2(=NO)
        //*                        1 = IDENTIFICA LE PENSIONI AVENTI DECORRE
        //*                            ANTERIORE AL 1/2/1991 IN PAGAMENTO AL
        //*                            ESTERO O RI RESIDENTI ALL'ESTERO LIQU
        //*                            TE SULLA BASE DEL CUMULO DEI CONTRIBU
        //*                            ITALIANI ED ESTERI.
        //*                        2 = IDENTIFICA LE PENSIONI IN PAGAMENTO A
        //*                            ESTERO O DI RESIDENTI ALL'ESTERO LIQU
        //*                            TE SENZA IL CUMULO DEI CONTRIBUTI ITA
        //*                            NI ED ESTERI.
        //     05  IW1TM59B             PIC 9(5)V9(4)       COMP-3.
        //*EURO  AUMENTO TOTALE LEGGE 59/91 ART.1 COMMA 9/BIS
        //     05  IW1SCA107            PIC X.
        //* SEGNALATORE SCADENZE CONTITOLARI NEL CALCOLO
        //* AUMENTO EX ART. 6/140 ATTIVATO DA PGM GC0107
        //     05  IW1CART6             PIC 9.
        //* CODICE MIGLIOR. ART.6/140
        #endregion tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// IW1TIPEL X  
        /// *+COD.TIPO ELABORAZIONE
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string IW1TIPEL { get; set; }

        /// <summary>
        /// IW1SECAN 9999  
        /// *+DATA NASCITA DEL TITOL.
        /// </summary>
        [HisFieldInfoMapping(1, 4)]
        public short IW1SECAN { get; set; }

        /// <summary>
        /// IW1NATITM 99  
        /// *+DATA NASCITA DEL TITOL.
        /// </summary>
        [HisFieldInfoMapping(2, 2)]
        public short IW1NATITM { get; set; }

        /// <summary>
        /// IW1NATITG 99  
        /// *+DATA NASCITA DEL TITOL.
        /// </summary>
        [HisFieldInfoMapping(3, 2)]
        public short IW1NATITG { get; set; }

        ///// <summary>
        ///// nel caso di valore null occorre far riferimento ai valori precedenti
        ///// </summary>
        //[HisFieldInfoMapping(4, 8, Offset = -8, DateFormat = "yyyyMMdd")]
        //public DateTime? IW1NATIT { get; set; }

        /// <summary>
        /// IW1NAANZA 9999  
        /// * DATA DI NASCITA CONTITOLARE PIU' ANZIANO
        /// </summary>
        [HisFieldInfoMapping(5, 4)]
        public short IW1NAANZA { get; set; }

        /// <summary>
        /// IW1NAANZM 99  
        /// * DATA DI NASCITA CONTITOLARE PIU' ANZIANO
        /// </summary>
        [HisFieldInfoMapping(6, 2)]
        public short IW1NAANZM { get; set; }

        /// <summary>
        /// IW1NAANZG 99  
        /// * DATA DI NASCITA CONTITOLARE PIU' ANZIANO
        /// </summary>
        [HisFieldInfoMapping(7, 2)]
        public short IW1NAANZG { get; set; }

        /// <summary>
        /// IW1DEOSEC 99  
        /// *+DEC. ORIGINARIA PENSIONE
        /// </summary>
        [HisFieldInfoMapping(8, 2)]
        public short IW1DEOSEC { get; set; }

        /// <summary>
        /// IW1DEOAA 99  
        /// *+DEC. ORIGINARIA PENSIONE
        /// </summary>
        [HisFieldInfoMapping(9, 2)]
        public short IW1DEOAA { get; set; }

        /// <summary>
        /// IW1DEORM 99  
        /// *+DEC. ORIGINARIA PENSIONE
        /// </summary>
        [HisFieldInfoMapping(10, 2)]
        public short IW1DEORM { get; set; }

        /// <summary>
        /// IW1DEOPA 9999  
        /// * DECORRENZA OPZIONE
        /// </summary>
        [HisFieldInfoMapping(11, 4)]
        public short IW1DEOPA { get; set; }

        /// <summary>
        /// IW1DEOPM 99  
        /// * DECORRENZA OPZIONE
        /// </summary>
        [HisFieldInfoMapping(12, 2)]
        public short IW1DEOPM { get; set; }

        /// <summary>
        /// IW1DERIPA 9999  
        /// * DECORRENZA RIPRISTINO
        /// </summary>
        [HisFieldInfoMapping(13, 4)]
        public short IW1DERIPA { get; set; }

        /// <summary>
        /// IW1DERIPM 99  
        /// * DECORRENZA RIPRISTINO
        /// </summary>
        [HisFieldInfoMapping(14, 2)]
        public short IW1DERIPM { get; set; }

        /// <summary>
        /// IW1DEDIRA 9999  
        /// *+DEC. PENSIONE DIRETTA
        /// </summary>
        [HisFieldInfoMapping(15, 4)]
        public short IW1DEDIRA { get; set; }

        /// <summary>
        /// IW1DEDIRM 99  
        /// *+DEC. PENSIONE DIRETTA
        /// </summary>
        [HisFieldInfoMapping(16, 2)]
        public short IW1DEDIRM { get; set; }

        // 05  IW1DANTE.
        /// * DATI DEL DANTE CAUSA
        /// <summary>
        /// IW1DPROV 9  
        /// *+PROVENIENZA DANTE C. 0=DI ASS. 1=DA VO 2=DA IO
        /// </summary>
        [HisFieldInfoMapping(17, 1)]
        public short IW1DPROV { get; set; }

        /// <summary>
        /// IW1DSES X  
        /// *+SESSO DANTE CAUSA
        /// </summary>
        [HisFieldInfoMapping(18, 1)]
        public string IW1DSES { get; set; }

        /// <summary>
        /// IW1DNASA 9999  
        /// *+DATA DI NASCITA D.C.
        /// </summary>
        [HisFieldInfoMapping(19, 4)]
        public short IW1DNASA { get; set; }

        /// <summary>
        /// IW1DNASM 99  
        /// *+DATA DI NASCITA D.C.
        /// </summary>
        [HisFieldInfoMapping(20, 2)]
        public short IW1DNASM { get; set; }

        /// <summary>
        /// IW1DNASG 99  
        /// *+DATA DI NASCITA D.C.
        /// </summary>
        [HisFieldInfoMapping(21, 2)]
        public short IW1DNASG { get; set; }

        /// <summary>
        /// IW1DMORA 9999  
        /// *+DATA DI MORTE D.C.
        /// </summary>
        [HisFieldInfoMapping(22, 4)]
        public short IW1DMORA { get; set; }

        /// <summary>
        /// IW1DMORM 99  
        /// *+DATA DI MORTE D.C.
        /// </summary>
        [HisFieldInfoMapping(23, 2)]
        public short IW1DMORM { get; set; }

        /// <summary>
        /// IW1DMORG 99  
        /// *+DATA DI MORTE D.C.
        /// </summary>
        [HisFieldInfoMapping(24, 2)]
        public short IW1DMORG { get; set; }

        /// <summary>
        /// IW1780CD 9  
        /// *+PEN.DIR.+780: 4=NO,3=SI
        /// </summary>
        [HisFieldInfoMapping(25, 1)]
        public short IW1780CD { get; set; }

        /// <summary>
        /// IW1SESTIT X  
        // *+SESSO DEL TITOLARE
        /// </summary>
        [HisFieldInfoMapping(26, 1)]
        public string IW1SESTIT { get; set; }

        /// <summary>
        /// IW1CATPEN 999  
        /// *+CAT.PENS.IN CODICE P18
        /// </summary>
        [HisFieldInfoMapping(27, 3)]
        public short IW1CATPEN { get; set; }

        /// <summary>
        /// IW1CODC 9  
        /// * CODICE CIECO
        /// </summary>
        [HisFieldInfoMapping(28, 1)]
        public short IW1CODC { get; set; }

        /// <summary>
        /// IW1CODEX 9  
        /// *+CODICE EX-COMBAT.(0/1)
        /// *+DEC. ART.6,LG.140/85
        /// </summary>
        [HisFieldInfoMapping(29, 1)]
        public short IW1CODEX { get; set; }

        /// <summary>
        /// IW1DECEXA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(30, 4)]
        public short IW1DECEXA { get; set; }

        /// <summary>
        /// IW1DECEXM 99  
        /// </summary>
        [HisFieldInfoMapping(31, 2)]
        public short IW1DECEXM { get; set; }

        /// <summary>
        /// IW1COSCIS 9  
        /// * CODICE SCISSIONE
        /// </summary>
        [HisFieldInfoMapping(32, 1)]
        public short IW1COSCIS { get; set; }

        /// <summary>
        /// IW1CRIRIL 9  
        /// * CODICE PENSIONE RILIQUIDATA PER CRISTALLIZZAZIONE
        /// * 0 = PENSIONE NON CRISTALLIZATA
        /// * 1 = PENSIONE DIRETTA RILIQUIDATA PER CRISTALL. 1983
        /// * 2 = PENSIONE DIRETTA RILIQUIDATA PER CRISTALL. POST 1983
        /// * 3 = PENSIONE SUPERST RILIQUIDATA PER CRISTALL. 1983
        /// * 4 = PENSIONE SUPERST RILIQUIDATA PER CRISTALL. POST 1983
        /// </summary>
        [HisFieldInfoMapping(33, 1)]
        public short IW1CRIRIL { get; set; }

        /// <summary>
        /// IW1CARIC 9  
        /// *+CODICE CAUSA CARICO
        /// </summary>
        [HisFieldInfoMapping(34, 1)]
        public short IW1CARIC { get; set; }

        /// <summary>
        /// IW1TIPINV 9  
        /// *+222:1=ASS.INV. 2=P.INAB.
        /// </summary>
        [HisFieldInfoMapping(35, 1)]
        public short IW1TIPINV { get; set; }

        /// <summary>
        /// IW1ELAB 9  
        /// *+T. ELAB. 0=NL/RC 1=RIN
        /// </summary>
        [HisFieldInfoMapping(36, 1)]
        public short IW1ELAB { get; set; }

        /// <summary>
        /// IW1CMS1 9  
        /// *+1=MAG.SOC.MINIMI ART.1
        /// </summary>
        [HisFieldInfoMapping(37, 1)]
        public short IW1CMS1 { get; set; }

        /// <summary>
        /// IW1DECMS1A 9(4)  
        /// *+DEC. MAG. SOC.ART.1/140
        /// </summary>
        [HisFieldInfoMapping(38, 4)]
        public short IW1DECMS1A { get; set; }

        /// <summary>
        /// IW1DECMS1M 99  
        /// *+DEC. MAG. SOC.ART.1/140
        /// </summary>
        [HisFieldInfoMapping(39, 2)]
        public short IW1DECMS1M { get; set; }

        /// <summary>
        /// IW1DEC544A 9(4)  
        /// *+DEC.AUMENTO SOC.LG.544
        /// </summary>
        [HisFieldInfoMapping(40, 4)]
        public short IW1DEC544A { get; set; }

        /// <summary>
        /// IW1DEC544M 9(2)  
        /// *+DEC.AUMENTO SOC.LG.544
        /// </summary>
        [HisFieldInfoMapping(41, 2)]
        public short IW1DEC544M { get; set; }

        // 05  IW1IDENT.
        // * IDENTIFICATIVO PENSIONE (SEDE E CERTIFICATO)
        /// <summary>
        /// IW1SESEZ 9(4)  
        /// *+IDENT.PENSIONE:(SEDE)
        /// </summary>
        [HisFieldInfoMapping(42, 4)]
        public short IW1SESEZ { get; set; }

        // 10  X-IW1CERT.
        /// <summary>
        /// IW1CERT 9(8)  
        /// *+IDENT.PENSIONE:(CERTIF)
        /// </summary>
        [HisFieldInfoMapping(43, 8)]
        public int IW1CERT { get; set; }

        /// <summary>
        /// IW1CAT8 X(8)  
        /// *+CAT.ALFABETICA (8 CAR)
        /// </summary>
        [HisFieldInfoMapping(44, 8)]
        public string IW1CAT8 { get; set; }

        /// <summary>
        /// IW1CALSO 9  
        /// * 1 = CALCOLO DIRETTA DI PROVENIENZA DELLA PENSIONE REVERSIBILITA'
        /// * 0 = CALCOLO DELLA PENSIONE DI REVERSIBILITA'
        /// </summary>
        [HisFieldInfoMapping(45, 1)]
        public short IW1CALSO { get; set; }

        /// <summary>
        /// IW1NOAF86 9  
        /// * SEGNALATORE PRESENZA CONIUGE
        /// * 0=CONIUGE MAI A CARICO
        /// * 1=CONIUGE A CARICO  9=PRESENTE X CALCOLO SENZA ALCUN DIRITTO
        /// </summary>
        [HisFieldInfoMapping(46, 1)]
        public short IW1NOAF86 { get; set; }

        /// <summary>
        /// IW1CODCAT X  
        /// *+0=MI 1=OB 2=CD 3=AR 4=C0
        /// </summary>
        [HisFieldInfoMapping(47, 1)]
        public string IW1CODCAT { get; set; }

        /// <summary>
        /// IW1DIRSUP 9  
        /// *+1=VEC 2=INV 3=IND 4=REV
        /// </summary>
        [HisFieldInfoMapping(48, 1)]
        public short IW1DIRSUP { get; set; }

        // 05  IW1DATE.
        /// <summary>
        /// IW1DA1A 9(4)  
        /// *+DATA INIZIO CALCOLO
        /// </summary>
        [HisFieldInfoMapping(49, 4)]
        public short IW1DA1A { get; set; }

        /// <summary>
        /// IW1DA1M 9(2)  
        /// *+DATA INIZIO CALCOLO
        /// </summary>
        [HisFieldInfoMapping(50, 2)]
        public short IW1DA1M { get; set; }

        /// <summary>
        /// IW1DA2A 9(4)  
        /// *+DATA FINE CALCOLO
        /// </summary>
        [HisFieldInfoMapping(51, 4)]
        public short IW1DA2A { get; set; }

        /// <summary>
        /// IW1DA2M 9(2)  
        /// *+DATA FINE CALCOLO
        /// </summary>
        [HisFieldInfoMapping(52, 2)]
        public short IW1DA2M { get; set; }

        /// <summary>
        /// IW1MENS0 X  
        /// * INDICATORE MENSILE = 0
        /// * 1 = PENSIONE NON CALCOLABILE PER ASSENZA DEI DATI ELEMENTARI
        /// *     (SETTIMANE DI ANZIANITA'CONTRIBUTIVA, RETRIBUZIONE MEDIA
        /// *     SETTIMANALE, IMPORTO DELLA PENSIONE BASE, ECC...)
        /// * 2 = PENSIONE AI SUPERSTITI NON CALCOLABILE ALLA DECORRENZA
        /// *     ORIGINARIA PER ASSENZA DI CONTITOLARI
        /// * 3 = PENSIONE INTEGRABILE AL TRATTAMENTO MINIMO SENZA
        /// *     SEGNALAZIONE DEL REDDITO
        /// * 4 = PENSIONE DI IMPORTO SUPERIORE AL TRATTAMENTO MINIMO MA
        /// *     INFERIORE AL TRATTAMENTO MINIMO MAGGIORATO PER 781
        /// *     CONTRIBUTI SENZA SEGNALAZIONE DEL REDDITO
        /// * 5 = PENSIONE INTEGRABILE AL TRATTAMENTO MINIMO CON CCC=8
        /// * 6 = PENSIONE DEI LAVORATORI AUTONOMI SENZA LA SEGNALAZIONE
        /// *     DEL NUMERO DI SETTIMANE DI ANZIANITA'CONTRIBUTIVA
        /// * 7 = PENSIONE SEGNALATA CON CONTRIBUZIONE SUPERIORE A 781
        /// *     SENZA IL NUMERO DI SETTIMANE. SCARTO PER NON ATTRIBUZIONE
        /// *     ARTICOLO 4 DELLA 140
        /// * 8 = PENSIONE CON RETRIBUZIONE MEDIA SETTIMANALE DI IMPORTO
        /// *     SUPERIORE AL LIMITE MASSIMO CONSENTITO
        /// * 9 = PENSIONE CON IMPORTO MENSILE MAGGIORE DI 9900000
        /// * A = PENSIONE CON PIU'DI 130 RICORRENZE DI CALCOLO
        /// * B = ANOMALIE IN DATI REDDITUALI PER TRATTAMENTO DI FAMIGLIA
        /// </summary>
        [HisFieldInfoMapping(53, 1)]
        public string IW1MENS0 { get; set; }

        /// <summary>
        /// IW1ESTCUM 9  
        /// *+0(=POST 1.1.91)/1(=SI)/2(=NO)
        /// *                        1 = IDENTIFICA LE PENSIONI AVENTI DECORRE
        /// *                            ANTERIORE AL 1/2/1991 IN PAGAMENTO AL
        /// *                            ESTERO O RI RESIDENTI ALL'ESTERO LIQU
        /// *                            TE SULLA BASE DEL CUMULO DEI CONTRIBU
        /// *                            ITALIANI ED ESTERI.
        /// *                        2 = IDENTIFICA LE PENSIONI IN PAGAMENTO A
        /// *                            ESTERO O DI RESIDENTI ALL'ESTERO LIQU
        /// *                            TE SENZA IL CUMULO DEI CONTRIBUTI ITA
        /// *                            NI ED ESTERI.
        /// </summary>
        [HisFieldInfoMapping(54, 1)]
        public short IW1ESTCUM { get; set; }

        /// <summary>
        /// IW1TM59B 9(5)V9(4) COMP-3 
        /// *EURO  AUMENTO TOTALE LEGGE 59/91 ART.1 COMMA 9/BIS
        /// </summary>
        [HisFieldInfoMapping(55, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal IW1TM59B { get; set; }

        /// <summary>
        /// IW1SCA107 X  
        /// * SEGNALATORE SCADENZE CONTITOLARI NEL CALCOLO
        /// * AUMENTO EX ART. 6/140 ATTIVATO DA PGM GC0107
        /// </summary>
        [HisFieldInfoMapping(56, 1)]
        public string IW1SCA107 { get; set; }

        /// <summary>
        /// IW1CART6 9  
        /// * CODICE MIGLIOR. ART.6/140
        /// </summary>
        [HisFieldInfoMapping(57, 1)]
        public short IW1CART6 { get; set; }

        #endregion Tracciato Host
    }
}
