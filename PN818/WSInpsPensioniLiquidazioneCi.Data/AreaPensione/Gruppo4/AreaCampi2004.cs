using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaCampi2004
    {

        #region tracciato COBOL
        //  04  CAMPI-2004.
        //         05  FILLER2017-2                  PIC X(300).
        //         05 N-INWKC2.
        //* IMPORTI STATI ESTERI     LUNGHEZZA TOTALE 6912
        //           10 ELEMEN OCCURS 6.
        //            15 STATO                 PIC 99.
        //*+CODICE STATO
        //            15 ISTIT                 PIC 999.
        //*+CODICE ISTITUZIONE
        //            15 MATRIC                PIC X(16).
        //*+MATRICOLA STATO ESTERO
        //            15 SETT1                 PIC S9(5) COMP-3.
        //*+SETT.EST A DEC.CALC.
        //            15 SETT2                 PIC S9(5) COMP-3.
        //*+SETT.EST A RICALCOLO
        //            15 SETTDIR               PIC S9(5) COMP-3.
        //*+SETT.EST PER IL DIRITTO
        //            15 ART48                 PIC X.
        //*+APPLICAZIONE ART.48 (-52) NO=0 SI=1
        //               20 DECART48A          PIC 9(4).
        //               20 DECART48M          PIC 9(2).
        //*+DECORENZA APPLICAZIONE ART.48 (-52)
        //               20 RICALSTATOA        PIC 9(4).
        //               20 RICALSTATOM        PIC 9(2).
        //* 1996 RICALCOLO A RICHIESTA PER STATI SENZA SETT1 O SETT2
        //*+DATA PRECEDENTE LIQUIDAZIONE
        //               20 IDAPLIQA           PIC 9999.
        //               20 IDAPLIQM           PIC 99.
        //* DECORRENZA PRECEDENTE LIQUIDAZIONE DEGLI STATI ESTERI
        //            15 COD-SOSP-ESTERO       PIC X.
        //* CODICE SOSPENSIONE PER ETA' 1=SOSP. 2=NON SOSP. (EST.NON PAGA)
        //            15 ETA-SOSP-ESTERO       PIC 99.
        //* ETA' SOSPENSIONE INTEGRAZIONE


        //            15 IMPORTI OCCURS 50.
        //*+DECORRENZA IMP.EST.
        //                  25 DECAA           PIC 9999.
        //                  25 DECMM           PIC 99.
        //               20 IMPEST             PIC S9(9)V9(8) COMP-3.
        //*EURO +IMPORTO ESTERO A DEC1
        //               20 PERIODIC          PIC X.
        //*+IMP.EST. A DEC1 IN LIRE
        //*+DECORRENZA CESSAZ.
        //                  25 CESAA           PIC 9999.
        //                  25 CESMM           PIC 99.
        #endregion tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// FILLER2017_2 X(300)  
        /// </summary>
        [HisFieldInfoMapping(0, 300)]
        public string FILLER2017_2 { get; set; }

        [HisComplexAreaInfoMapping(1, ListCount = 6)]
        public List<StatoEstero> STATIESTERI { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class StatoEstero
        {

            #region tracciato COBOL
            //      05 N-INWKC2.
            //* IMPORTI STATI ESTERI     LUNGHEZZA TOTALE 6912
            //           10 ELEMEN OCCURS 6.
            //            15 STATO                 PIC 99.
            //*+CODICE STATO
            //            15 ISTIT                 PIC 999.
            //*+CODICE ISTITUZIONE
            //            15 MATRIC                PIC X(16).
            //*+MATRICOLA STATO ESTERO
            //            15 SETT1                 PIC S9(5) COMP-3.
            //*+SETT.EST A DEC.CALC.
            //            15 SETT2                 PIC S9(5) COMP-3.
            //*+SETT.EST A RICALCOLO
            //            15 SETTDIR               PIC S9(5) COMP-3.
            //*+SETT.EST PER IL DIRITTO
            //            15 ART48                 PIC X.
            //*+APPLICAZIONE ART.48 (-52) NO=0 SI=1
            //               20 DECART48A          PIC 9(4).
            //               20 DECART48M          PIC 9(2).
            //*+DECORENZA APPLICAZIONE ART.48 (-52)
            //               20 RICALSTATOA        PIC 9(4).
            //               20 RICALSTATOM        PIC 9(2).
            //* 1996 RICALCOLO A RICHIESTA PER STATI SENZA SETT1 O SETT2
            //*+DATA PRECEDENTE LIQUIDAZIONE
            //               20 IDAPLIQA           PIC 9999.
            //               20 IDAPLIQM           PIC 99.
            //* DECORRENZA PRECEDENTE LIQUIDAZIONE DEGLI STATI ESTERI
            //            15 COD-SOSP-ESTERO       PIC X.
            //* CODICE SOSPENSIONE PER ETA' 1=SOSP. 2=NON SOSP. (EST.NON PAGA)
            //            15 ETA-SOSP-ESTERO       PIC 99.
            //* ETA' SOSPENSIONE INTEGRAZIONE


            //            15 IMPORTI OCCURS 50.
            //*+DECORRENZA IMP.EST.
            //                  25 DECAA           PIC 9999.
            //                  25 DECMM           PIC 99.
            //               20 IMPEST             PIC S9(9)V9(8) COMP-3.
            //*EURO +IMPORTO ESTERO A DEC1
            //               20 PERIODIC          PIC X.
            //*+IMP.EST. A DEC1 IN LIRE
            //*+DECORRENZA CESSAZ.
            //                  25 CESAA           PIC 9999.
            //                  25 CESMM           PIC 99.
            #endregion tracciato COBOL

            #region Tracciato Host
            // 05 N-INWKC2.
            // * IMPORTI STATI ESTERI     LUNGHEZZA TOTALE 6912
            // 10 ELEMEN OCCURS 6.
            /// <summary>
            /// STATO 99  
            /// *+CODICE STATO
            /// </summary>
            [HisFieldInfoMapping(0, 2)]
            public short STATO { get; set; }

            /// <summary>
            /// ISTIT 999  
            /// *+CODICE ISTITUZIONE
            /// </summary>
            [HisFieldInfoMapping(1, 3)]
            public short ISTIT { get; set; }

            /// <summary>
            /// MATRIC X(16)  
            /// *+MATRICOLA STATO ESTERO
            /// </summary>
            [HisFieldInfoMapping(2, 16)]
            public string MATRIC { get; set; }

            /// <summary>
            /// SETT1 S9(5) COMP-3 
            /// *+SETT.EST A DEC.CALC.
            /// </summary>
            [HisFieldInfoMapping(3, 3, CobolType = CobolType.Comp3)]
            public int SETT1 { get; set; }

            /// <summary>
            /// SETT2 S9(5) COMP-3 
            /// *+SETT.EST A RICALCOLO
            /// </summary>
            [HisFieldInfoMapping(4, 3, CobolType = CobolType.Comp3)]
            public int SETT2 { get; set; }

            /// <summary>
            /// SETTDIR S9(5) COMP-3 
            /// *+SETT.EST PER IL DIRITTO
            /// </summary>
            [HisFieldInfoMapping(5, 3, CobolType = CobolType.Comp3)]
            public int SETTDIR { get; set; }

            /// <summary>
            /// ART48 X  
            /// *+APPLICAZIONE ART.48 (-52) NO=0 SI=1
            /// </summary>
            [HisFieldInfoMapping(6, 1)]
            public string ART48 { get; set; }

            /// <summary>
            /// DECART48A 9(4)  
            /// *+DECORENZA APPLICAZIONE ART.48 (-52)
            /// </summary>
            [HisFieldInfoMapping(7, 4)]
            public short DECART48A { get; set; }

            /// <summary>
            /// DECART48M 9(2)  
            /// *+DECORENZA APPLICAZIONE ART.48 (-52)
            /// </summary>
            [HisFieldInfoMapping(8, 2)]
            public short DECART48M { get; set; }

            /// <summary>
            /// RICALSTATOA 9(4)  
            /// * 1996 RICALCOLO A RICHIESTA PER STATI SENZA SETT1 O SETT2
            /// </summary>
            [HisFieldInfoMapping(9, 4)]
            public short RICALSTATOA { get; set; }

            /// <summary>
            /// RICALSTATOM 9(2)  
            /// * 1996 RICALCOLO A RICHIESTA PER STATI SENZA SETT1 O SETT2
            /// </summary>
            [HisFieldInfoMapping(10, 2)]
            public short RICALSTATOM { get; set; }

            // *+DATA PRECEDENTE LIQUIDAZIONE
            /// <summary>
            /// IDAPLIQA 9999  
            /// * DECORRENZA PRECEDENTE LIQUIDAZIONE DEGLI STATI ESTERI
            /// </summary>
            [HisFieldInfoMapping(11, 4)]
            public short IDAPLIQA { get; set; }

            /// <summary>
            /// IDAPLIQM 99  
            /// * DECORRENZA PRECEDENTE LIQUIDAZIONE DEGLI STATI ESTERI
            /// </summary>
            [HisFieldInfoMapping(12, 2)]
            public short IDAPLIQM { get; set; }

            /// <summary>
            /// COD_SOSP_ESTERO X  
            /// * CODICE SOSPENSIONE PER ETA' 1=SOSP. 2=NON SOSP. (EST.NON PAGA)
            /// </summary>
            [HisFieldInfoMapping(13, 1)]
            public string COD_SOSP_ESTERO { get; set; }

            /// <summary>
            /// ETA_SOSP_ESTERO 99  
            /// * ETA' SOSPENSIONE INTEGRAZIONE
            /// </summary>
            [HisFieldInfoMapping(14, 2)]
            public short ETA_SOSP_ESTERO { get; set; }

            [HisComplexAreaInfoMapping(15, ListCount = 50)]
            public List<Importo> IMPORTI { get; set; }

            #endregion Tracciato Host

            #region nested class
            public class Importo
            {
                #region tracciato COBOL
                //          15 IMPORTI OCCURS 50.
                //*+DECORRENZA IMP.EST.
                //                  25 DECAA           PIC 9999.
                //                  25 DECMM           PIC 99.
                //               20 IMPEST             PIC S9(9)V9(8) COMP-3.
                //*EURO +IMPORTO ESTERO A DEC1
                //               20 PERIODIC          PIC X.
                //*+IMP.EST. A DEC1 IN LIRE
                //*+DECORRENZA CESSAZ.
                //                  25 CESAA           PIC 9999.
                //                  25 CESMM           PIC 99.
                #endregion tracciato COBOL

                #region Tracciato Host
                // 15 IMPORTI OCCURS 50.
                /// <summary>
                /// DECAA 9999  
                /// *+DECORRENZA IMP.EST.
                /// </summary>
                [HisFieldInfoMapping(0, 4)]
                public short DECAA { get; set; }

                /// <summary>
                /// DECMM 99  
                /// *+DECORRENZA IMP.EST.
                /// </summary>
                [HisFieldInfoMapping(1, 2)]
                public short DECMM { get; set; }

                /// <summary>
                /// IMPEST S9(9)V9(8) COMP-3 
                /// *EURO +IMPORTO ESTERO A DEC1
                /// </summary>
                [HisFieldInfoMapping(2, 9, Scale = 8, CobolType = CobolType.Comp3)]
                public decimal IMPEST { get; set; }

                /// <summary>
                /// PERIODIC X  
                /// *+IMP.EST. A DEC1 IN LIRE
                /// </summary>
                [HisFieldInfoMapping(3, 1)]
                public string PERIODIC { get; set; }

                /// <summary>
                /// CESAA 9999  
                /// *+DECORRENZA CESSAZ.
                /// </summary>
                [HisFieldInfoMapping(4, 4)]
                public short CESAA { get; set; }

                /// <summary>
                /// CESMM 99  
                /// *+DECORRENZA CESSAZ.
                /// </summary>
                [HisFieldInfoMapping(5, 2)]
                public short CESMM { get; set; }

                #endregion Tracciato Host
            }
            #endregion nested class
        }

        #endregion nested class
    }
}
