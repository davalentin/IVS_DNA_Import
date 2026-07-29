using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaSpazio
    {
        #region tracciato COBOL
        //  04  ISPAZIO.
        //        05  ISETAUTVV-D                PIC 9(4).
        //*2007-Numero settimane VV utili diritto per lavoratori autonomi
        //        05  ISETAUTVV-M                PIC 9(4).
        //*2007-Numero settimane VV utili misura per lavoratori autonomi
        //        05  OPZIONE-CONTRIBUTIVA       PIC X.
        //*2007-S/N
        //        05  FILLER                   PIC X(3).
        //** 02/12/2003 SETTIMANE MATERNITA' UTILI 05/2001 *****************
        //        05  IMATERNITA.
        //            10  ISETMAT1             PIC 9999.
        //*NUM.CONTRIBUTI MATERNITA' PRIMA QUOTA CONT/RET/233-71
        //            10  ISETMAT2             PIC 9999.
        //*NUM.CONTRIBUTI MATERNITA' SECONDA QUOTA 503-61
        //            10  ISETMAT3             PIC 9999.
        //*NUM.CONTRIBUTI MATERNITA' TERZA QUOTA 335-01
        //            10  IIVSMAT1             PIC 9(5)V9(6)  COMP-3.
        //*I.V.S.  PRIMA QUOTA CONTRIBUTIVA
        //            10  IRMSMAT1             PIC 9(5)V9(6)  COMP-3.
        //*R.M.S.  PRIMA QUOTA RETRIBUTIVA E 233-71
        //            10  IRMSMAT2             PIC 9(5)V9(6)  COMP-3.
        //*R.M.S. MATERNITA' SECONDA QUOTA 503-61
        //            10  IMONMAT3             PIC 9(5)V9(6)  COMP-3.
        //*MONTANTE MATERNITA' TERZA QUOTA 335-01
        //            10  ICONMAT3             PIC 9(5)V9(6)  COMP-3.
        //*IMPORTO CONTRIBUTI MATERNITA' TERZA QUOTA 335-01
        //            10  ICI2IMPMAT           PIC 9(5)V9(6)  COMP-3.
        //*IMPORTO A DEC. CALCOLO MATERNITA' (PER RINNOVO TIPO 2 0 1)
        //** TOT.SET.MATERNITA'=48 + ETA PER ESTERO=12 TOTALE NUOVI=60******
        //** 30/09/2004 SETTIMANE ACNA DI CENGIO UTILI 02/2004 *************
        //        05  IACNA-CENGIO.
        //            10  ISETCEN1             PIC 9999.
        //*NUM.CONTRIBUTI CENGIO PRIMA QUOTA CONT/RET/233-71
        //            10  ISETCEN2             PIC 9999.
        //*NUM.CONTRIBUTI CENGIO SECONDA QUOTA 503-61
        //            10  ISETCEN3             PIC 9999.
        //*NUM.CONTRIBUTI CENGIO TERZA QUOTA 335-01
        //            10  IIVSCEN1             PIC 9(5)V9(6)  COMP-3.
        //*I.V.S.  PRIMA QUOTA CONTRIBUTIVA
        //            10  IRMSCEN1             PIC 9(5)V9(6)  COMP-3.
        //*R.M.S.  PRIMA QUOTA RETRIBUTIVA E 233-71
        //            10  IRMSCEN2             PIC 9(5)V9(6)  COMP-3.
        //*R.M.S. CENGIO SECONDA QUOTA 503-61
        //            10  IMONCEN3             PIC 9(5)V9(6)  COMP-3.
        //*MONTANTE CENGIO TERZA QUOTA 335-01
        //            10  ICONCEN3             PIC 9(5)V9(6)  COMP-3.
        //*IMPORTO CONTRIBUTI CENGIO TERZA QUOTA 335-01
        //            10  ICI2IMPCEN           PIC 9(5)V9(6)  COMP-3.
        //*IMPORTO A DEC. CALCOLO CENGIO (PER RINNOVO TIPO 2 O 1)
        //** TOT.SET.ACNA-CENGIO=48 + 60 ALTRI NUOVI TOTALE NUOVI=108******
        //        05  TP1COMPR.
        //            10  TP1COMPA                   PIC 9999.
        //            10  TP1COMPM                   PIC 99.
        //            10  TP1COMPG                   PIC 99.
        //* DEC. COMPLETEZZA DOMANDA
        //        05  FILLER               PIC X(8).
        //*
        //        05  ANNI-ANTICIPO-544    PIC 9.
        //*NUMERO ANNI ANTICIPO PER MAGGIORAZIONE SOCIALE (MILIONE)
        //*DATA ART.44 L.1289.2002(FINANZIARIA) CUMUL.PENS/LAVORO
        //            15  DECART44A        PIC 9999.
        //            15  DECART44M        PIC 99.


        //        05  IELBLE3RED  OCCURS 10  PIC S9(7)V9(2)  COMP-3.


        //*REDDITI DA LAVORO ALL'ESTERO
        //        05  ITOT-EST-95          PIC 9(4).
        //*TOTALE SETTIMANE ESTERE AL 31/12/1995
        //        05  CODCOMUNE-R          PIC X(4).


        //        05  TP1REV-F OCCURS 15.
        //            10  TP1REVF          PIC 9(6).
        //                15  TP1REVFA     PIC 9999.
        //                15  TP1REVFM     PIC 99.
        //*REVISIONE SANITARIA PREVISTA SUI SINGOLI FAMILIARI


        //        05  IREQA2C3-385         PIC X.
        //*REQUISITO ART.2,COMMA 3,LG.385/2000: 0/N=NO 1/S=SI 2/A=AUTOMATICO
        //        05  CASRED-SINO          PIC X(2).
        //        05  ARRIVODO.
        //            10  ARRI-DOA         PIC 9999.
        //            10  ARRI-DOM         PIC 99.
        //            10  ARRI-DOG         PIC 99.
        #endregion tracciato COBOL

        #region Tracciato Host
        // 04  ISPAZIO.
        /// <summary>
        /// ISETAUTVV_D 9(4)  
        /// *2007-Numero settimane VV utili diritto per lavoratori autonomi
        /// </summary>
        [HisFieldInfoMapping(0, 4)]
        public short ISETAUTVV_D { get; set; }

        /// <summary>
        /// ISETAUTVV_M 9(4)  
        /// *2007-Numero settimane VV utili misura per lavoratori autonomi
        /// </summary>
        [HisFieldInfoMapping(1, 4)]
        public short ISETAUTVV_M { get; set; }

        /// <summary>
        /// OPZIONE_CONTRIBUTIVA X  
        /// *2007-S/N
        /// </summary>
        [HisFieldInfoMapping(2, 1)]
        public string OPZIONE_CONTRIBUTIVA { get; set; }

        /// <summary>
        /// FILLER X(3)  
        ///** 02/12/2003 SETTIMANE MATERNITA' UTILI 05/2001 *****************
        /// </summary>
        [HisFieldInfoMapping(3, 3)]
        public string FILLER1 { get; set; }

        // 05  IMATERNITA.
        /// <summary>
        /// ISETMAT1 9999  
        /// *NUM.CONTRIBUTI MATERNITA' PRIMA QUOTA CONT/RET/233-71
        /// </summary>
        [HisFieldInfoMapping(4, 4)]
        public short ISETMAT1 { get; set; }

        /// <summary>
        /// ISETMAT2 9999  
        /// *NUM.CONTRIBUTI MATERNITA' SECONDA QUOTA 503-61
        /// </summary>
        [HisFieldInfoMapping(5, 4)]
        public short ISETMAT2 { get; set; }

        /// <summary>
        /// ISETMAT3 9999  
        /// *NUM.CONTRIBUTI MATERNITA' TERZA QUOTA 335-01
        /// </summary>
        [HisFieldInfoMapping(6, 4)]
        public short ISETMAT3 { get; set; }

        /// <summary>
        /// IIVSMAT1 9(5)V9(6) COMP-3 
        /// *I.V.S.  PRIMA QUOTA CONTRIBUTIVA
        /// </summary>
        [HisFieldInfoMapping(7, 6, Scale = 6, CobolType = CobolType.Comp3Unsigned)]
        public decimal IIVSMAT1 { get; set; }

        /// <summary>
        /// IRMSMAT1 9(5)V9(6) COMP-3 
        /// *R.M.S.  PRIMA QUOTA RETRIBUTIVA E 233-71
        /// </summary>
        [HisFieldInfoMapping(8, 6, Scale = 6, CobolType = CobolType.Comp3Unsigned)]
        public decimal IRMSMAT1 { get; set; }

        /// <summary>
        /// IRMSMAT2 9(5)V9(6) COMP-3 
        /// *R.M.S. MATERNITA' SECONDA QUOTA 503-61
        /// </summary>
        [HisFieldInfoMapping(9, 6, Scale = 6, CobolType = CobolType.Comp3Unsigned)]
        public decimal IRMSMAT2 { get; set; }

        /// <summary>
        /// IMONMAT3 9(5)V9(6) COMP-3 
        /// *MONTANTE MATERNITA' TERZA QUOTA 335-01
        /// </summary>
        [HisFieldInfoMapping(10, 6, Scale = 6, CobolType = CobolType.Comp3Unsigned)]
        public decimal IMONMAT3 { get; set; }

        /// <summary>
        /// ICONMAT3 9(5)V9(6) COMP-3 
        /// *IMPORTO CONTRIBUTI MATERNITA' TERZA QUOTA 335-01
        /// </summary>
        [HisFieldInfoMapping(11, 6, Scale = 6, CobolType = CobolType.Comp3Unsigned)]
        public decimal ICONMAT3 { get; set; }

        /// <summary>
        /// ICI2IMPMAT 9(5)V9(6) COMP-3 
        /// *IMPORTO A DEC. CALCOLO MATERNITA' (PER RINNOVO TIPO 2 0 1)
        /// </summary>
        [HisFieldInfoMapping(12, 6, Scale = 6, CobolType = CobolType.Comp3Unsigned)]
        public decimal ICI2IMPMAT { get; set; }

        //** TOT.SET.MATERNITA'=48 + ETA PER ESTERO=12 TOTALE NUOVI=60******
        //** 30/09/2004 SETTIMANE ACNA DI CENGIO UTILI 02/2004 *************
        // 05  IACNA-CENGIO.
        /// <summary>
        /// ISETCEN1 9999  
        /// *NUM.CONTRIBUTI CENGIO PRIMA QUOTA CONT/RET/233-71
        /// </summary>
        [HisFieldInfoMapping(13, 4)]
        public short ISETCEN1 { get; set; }

        /// <summary>
        /// ISETCEN2 9999  
        /// *NUM.CONTRIBUTI CENGIO SECONDA QUOTA 503-61
        /// </summary>
        [HisFieldInfoMapping(14, 4)]
        public short ISETCEN2 { get; set; }

        /// <summary>
        /// ISETCEN3 9999  
        /// *NUM.CONTRIBUTI CENGIO TERZA QUOTA 335-01
        /// </summary>
        [HisFieldInfoMapping(15, 4)]
        public short ISETCEN3 { get; set; }

        /// <summary>
        /// IIVSCEN1 9(5)V9(6) COMP-3 
        /// *I.V.S.  PRIMA QUOTA CONTRIBUTIVA
        /// </summary>
        [HisFieldInfoMapping(16, 6, Scale = 6, CobolType = CobolType.Comp3Unsigned)]
        public decimal IIVSCEN1 { get; set; }

        /// <summary>
        /// IRMSCEN1 9(5)V9(6) COMP-3 
        // *R.M.S.  PRIMA QUOTA RETRIBUTIVA E 233-71
        /// </summary>
        [HisFieldInfoMapping(17, 6, Scale = 6, CobolType = CobolType.Comp3Unsigned)]
        public decimal IRMSCEN1 { get; set; }

        /// <summary>
        /// IRMSCEN2 9(5)V9(6) COMP-3 
        /// *R.M.S. CENGIO SECONDA QUOTA 503-61
        /// </summary>
        [HisFieldInfoMapping(18, 6, Scale = 6, CobolType = CobolType.Comp3Unsigned)]
        public decimal IRMSCEN2 { get; set; }

        /// <summary>
        /// IMONCEN3 9(5)V9(6) COMP-3 
        /// *MONTANTE CENGIO TERZA QUOTA 335-01
        /// </summary>
        [HisFieldInfoMapping(19, 6, Scale = 6, CobolType = CobolType.Comp3Unsigned)]
        public decimal IMONCEN3 { get; set; }

        /// <summary>
        /// ICONCEN3 9(5)V9(6) COMP-3 
        // *IMPORTO CONTRIBUTI CENGIO TERZA QUOTA 335-01
        /// </summary>
        [HisFieldInfoMapping(20, 6, Scale = 6, CobolType = CobolType.Comp3Unsigned)]
        public decimal ICONCEN3 { get; set; }

        /// <summary>
        /// ICI2IMPCEN 9(5)V9(6) COMP-3 
        /// *IMPORTO A DEC. CALCOLO CENGIO (PER RINNOVO TIPO 2 O 1)
        /// </summary>
        [HisFieldInfoMapping(21, 6, Scale = 6, CobolType = CobolType.Comp3Unsigned)]
        public decimal ICI2IMPCEN { get; set; }

        //** TOT.SET.ACNA-CENGIO=48 + 60 ALTRI NUOVI TOTALE NUOVI=108******
        // 05  TP1COMPR.
        /// <summary>
        /// TP1COMPA 9999  
        /// * DEC. COMPLETEZZA DOMANDA
        /// </summary>
        [HisFieldInfoMapping(22, 4)]
        public short TP1COMPA { get; set; }

        /// <summary>
        /// TP1COMPM 99  
        // * DEC. COMPLETEZZA DOMANDA
        /// </summary>
        [HisFieldInfoMapping(23, 2)]
        public short TP1COMPM { get; set; }

        /// <summary>
        /// TP1COMPG 99  
        /// * DEC. COMPLETEZZA DOMANDA
        /// </summary>
        [HisFieldInfoMapping(24, 2)]
        public short TP1COMPG { get; set; }

        /// <summary>
        /// FILLER X(8)  
        /// </summary>
        [HisFieldInfoMapping(25, 8)]
        public string FILLER2 { get; set; }

        //*
        /// <summary>
        /// ANNI_ANTICIPO_544 9  
        /// *NUMERO ANNI ANTICIPO PER MAGGIORAZIONE SOCIALE (MILIONE)
        /// </summary>
        [HisFieldInfoMapping(26, 1)]
        public short ANNI_ANTICIPO_544 { get; set; }

        /// <summary>
        /// DECART44A 9999  
        /// *DATA ART.44 L.1289.2002(FINANZIARIA) CUMUL.PENS/LAVORO
        /// </summary>
        [HisFieldInfoMapping(27, 4)]
        public short DECART44A { get; set; }

        /// <summary>
        /// DECART44M 99  
        /// *DATA ART.44 L.1289.2002(FINANZIARIA) CUMUL.PENS/LAVORO
        /// </summary>
        [HisFieldInfoMapping(28, 2)]
        public short DECART44M { get; set; }


        // *REDDITI DA LAVORO ALL'ESTERO
        [HisComplexAreaInfoMapping(29, ListCount = 10)]
        public List<RedditiLavoroEstero> REDDITILAVOROESTERO { get; set; }

        /// <summary>
        /// ITOT_EST_95 9(4) 
        /// *TOTALE SETTIMANE ESTERE AL 31/12/1995
        /// </summary>
        [HisFieldInfoMapping(30, 4)]
        public short ITOT_EST_95 { get; set; }

        /// <summary>
        /// CODCOMUNE_R X(4)  
        /// </summary>
        [HisFieldInfoMapping(31, 4)]
        public string CODCOMUNE_R { get; set; }

        [HisComplexAreaInfoMapping(32, ListCount = 15)]
        public List<RevisioneSanitaria> REVISIONISANITARIE { get; set; }

        /// <summary>
        /// IREQA2C3_385 X  
        /// *REQUISITO ART.2,COMMA 3,LG.385/2000: 0/N=NO 1/S=SI 2/A=AUTOMATICO
        /// </summary>
        [HisFieldInfoMapping(33, 1)]
        public string IREQA2C3_385 { get; set; }

        /// <summary>
        /// CASRED_SINO X(2)  
        /// </summary>
        [HisFieldInfoMapping(34, 2)]
        public string CASRED_SINO { get; set; }

        // 05  ARRIVODO.
        /// <summary>
        /// ARRI_DOA 9999  
        /// </summary>
        [HisFieldInfoMapping(35, 4)]
        public short ARRI_DOA { get; set; }

        /// <summary>
        /// ARRI_DOM 99  
        /// </summary>
        [HisFieldInfoMapping(36, 2)]
        public short ARRI_DOM { get; set; }

        /// <summary>
        /// ARRI_DOG 99  
        /// </summary>
        [HisFieldInfoMapping(37, 2)]
        public short ARRI_DOG { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class RedditiLavoroEstero
        {
            #region tracciato COBOL
            //05  IELBLE3RED  OCCURS 10  PIC S9(7)V9(2)  COMP-3.
            #endregion tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// IELBLE3RED S9(7)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal IELBLE3RED { get; set; }
            #endregion Tracciato Host
        }

        public class RevisioneSanitaria
        {
            #region tracciato COBOL
            //                    05  TP1REV-F OCCURS 15.
            //                15  TP1REVFA     PIC 9999.
            //                15  TP1REVFM     PIC 99.
            //*REVISIONE SANITARIA PREVISTA SUI SINGOLI FAMILIARI
            #endregion tracciato COBOL

            #region Tracciato Host
            // 05  TP1REV-F OCCURS 15.
            /// <summary>
            /// TP1REVFA 9999  
            /// *REVISIONE SANITARIA PREVISTA SUI SINGOLI FAMILIARI
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public short TP1REVFA { get; set; }

            /// <summary>
            /// TP1REVFM 99  
            /// *REVISIONE SANITARIA PREVISTA SUI SINGOLI FAMILIARI
            /// </summary>
            [HisFieldInfoMapping(1, 2)]
            public short TP1REVFM { get; set; }

            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
