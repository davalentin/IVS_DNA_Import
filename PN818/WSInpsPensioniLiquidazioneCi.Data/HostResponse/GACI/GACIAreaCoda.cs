using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class GACIAreaCoda
    {
        #region Constructor
        internal GACIAreaCoda()
        {
        }
        #endregion Constructor

        #region tracciato COBOL
        //   01   AREA-STO.
        //      02   STOART                     PIC X.
        //*          CODICE 0/1/6/7 X ART 140
        //      02   STOMIN                     PIC X.
        //*          1 = MINIMALE
        //      02   STO3351                    PIC X.
        //*          1 = COMMA 41
        //      02   STO3352                    PIC X.
        //*          1 = COMMA 42
        //      02   STO3353                    PIC X.
        //*          1 = COMMA43
        //      02   STO3354                    PIC X.
        //*          1 = CRISTALLIZZATA
        //      02   STOUP2                     PIC XXX.
        //*          U.P. SCISSA
        //      02   FLEGN                      PIC X.
        //*          N = NUOVO TRACCIATO
        //      02   STOUFF                     PIC X.
        //*          1 = DA RICOST.D'UFFICIO


        //      02   STO-ERARIALI.
        //        05   STO-ERA     OCCURS 6 TIMES.
        //*            TRATT.ERARIALI (MESE/IMPORTO)
        //           10 STO-ERA-MES                PIC 99.
        //           10 STO-ERA-IMP                PIC S9(7)V9999 COMP-3.


        //      02   STO-IMPORTI.
        //        05   STO-IMP     OCCURS 75 TIMES.
        //*            IMPORTI (LNG 64 X 75 =4800)
        //           10 STO-KC01.
        //*            DECORRENZA AAMM
        //              15 STO-KC01AA              PIC XXXX.
        //              15 STO-KC01MM              PIC XX.
        //           10 STO-KC04                   PIC S9(7)V9999  COMP-3.
        //*            ADEGUATA
        //           10 STO-KC10                   PIC S9(7)V9999  COMP-3.
        //*            IN PAGAMENTO SENZA AAFF
        //           10 STO-KC03                   PIC S9(7)V9999  COMP-3.
        //*            AA FF
        //           10 STO-KE04                   PIC S9(5)V9999  COMP-3.
        //*            PRO RATA ESTERO
        //           10 STO-HI01                   PIC S9(5)V9999  COMP-3.
        //*            TRATT.SINDACALE
        //           10 STO-HG87                   PIC S9(7)V9999  COMP-3.
        //*
        //           10 STO-HG7576                 PIC S9(7)V9999  COMP-3.
        //*            TRATT 553 (41/42)
        //           10 STO-HG74                   PIC S9(7)V9999  COMP-3.
        //*            TRATT 553 (REND.INFORT)
        //           10 STO-HG80                   PIC S9(7)V9999  COMP-3.
        //*            TRATTENUTA LAVORA AUTONOMO
        //           10 STO-KD01                   PIC S9(7)V9999  COMP-3.
        //*            TRATTENUTA LAV.DIPENDENTE


        //      02   STO-SOLIDARI.
        //        05   STO-SOL     OCCURS 6 TIMES.
        //*            TRATT.SOLIDAR. (MESE/IMPORTO)
        //           10 STO-SOL-MES                PIC 99.
        //           10 STO-SOL-IMP                PIC S9(7)V9999 COMP-3.
        #endregion tracciato COBOL

        #region Tracciato Host
        // 01   AREA-STO.
        /// <summary>
        /// STOART X  
        // *          CODICE 0/1/6/7 X ART 140
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string STOART { get; set; }

        /// <summary>
        /// STOMIN X  
        // *          1 = MINIMALE
        /// </summary>
        [HisFieldInfoMapping(1, 1)]
        public string STOMIN { get; set; }

        /// <summary>
        /// STO3351 X  
        // *          1 = COMMA 41
        /// </summary>
        [HisFieldInfoMapping(2, 1)]
        public string STO3351 { get; set; }

        /// <summary>
        /// STO3352 X  
        // *          1 = COMMA 42
        /// </summary>
        [HisFieldInfoMapping(3, 1)]
        public string STO3352 { get; set; }

        /// <summary>
        /// STO3353 X  
        // *          1 = COMMA43
        /// </summary>
        [HisFieldInfoMapping(4, 1)]
        public string STO3353 { get; set; }

        /// <summary>
        /// STO3354 X  
        // *          1 = CRISTALLIZZATA
        /// </summary>
        [HisFieldInfoMapping(5, 1)]
        public string STO3354 { get; set; }

        /// <summary>
        /// STOUP2 XXX  
        // *          U.P. SCISSA
        /// </summary>
        [HisFieldInfoMapping(6, 3)]
        public string STOUP2 { get; set; }

        /// <summary>
        /// FLEGN X  
        // *          N = NUOVO TRACCIATO
        /// </summary>
        [HisFieldInfoMapping(7, 1)]
        public string FLEGN { get; set; }

        /// <summary>
        /// STOUFF X  
        // *          1 = DA RICOST.D'UFFICIO
        /// </summary>
        [HisFieldInfoMapping(8, 1)]
        public string STOUFF { get; set; }

        [HisComplexAreaInfoMapping(9, ListCount = 6)]
        public List<Erariale> ERARIALI { get; set; }

        [HisComplexAreaInfoMapping(10, ListCount = 75)]
        public List<Importo> IMPORTI { get; set; }

        [HisComplexAreaInfoMapping(11, ListCount = 6)]
        public List<Solidare> SOLIDARI { get; set; }

        #endregion Tracciato Host

        #region nested class
        public class Erariale
        {
            #region Constructor
            internal Erariale()
            {
            }
            #endregion Constructor

            #region tracciato COBOL
            //      02   STO-ERARIALI.
            //        05   STO-ERA     OCCURS 6 TIMES.
            //*            TRATT.ERARIALI (MESE/IMPORTO)
            //           10 STO-ERA-MES                PIC 99.
            //           10 STO-ERA-IMP                PIC S9(7)V9999 COMP-3.
            #endregion tracciato COBOL

            #region Tracciato Host
            // 02   STO-ERARIALI.
            // 05   STO-ERA     OCCURS 6 TIMES.
            // *            TRATT.ERARIALI (MESE/IMPORTO)
            /// <summary>
            /// STO_ERA_MES 99  
            /// </summary>
            [HisFieldInfoMapping(0, 2)]
            public short STO_ERA_MES { get; set; }

            /// <summary>
            /// STO_ERA_IMP S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(1, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal STO_ERA_IMP { get; set; }
            #endregion Tracciato Host
        }
        public class Importo
        {
            #region Constructor
            internal Importo()
            {
            }
            #endregion Constructor

            #region tracciato COBOL
            //      02   STO-IMPORTI.
            //        05   STO-IMP     OCCURS 75 TIMES.
            //*            IMPORTI (LNG 64 X 75 =4800)
            //           10 STO-KC01.
            //*            DECORRENZA AAMM
            //              15 STO-KC01AA              PIC XXXX.
            //              15 STO-KC01MM              PIC XX.
            //           10 STO-KC04                   PIC S9(7)V9999  COMP-3.
            //*            ADEGUATA
            //           10 STO-KC10                   PIC S9(7)V9999  COMP-3.
            //*            IN PAGAMENTO SENZA AAFF
            //           10 STO-KC03                   PIC S9(7)V9999  COMP-3.
            //*            AA FF
            //           10 STO-KE04                   PIC S9(5)V9999  COMP-3.
            //*            PRO RATA ESTERO
            //           10 STO-HI01                   PIC S9(5)V9999  COMP-3.
            //*            TRATT.SINDACALE
            //           10 STO-HG87                   PIC S9(7)V9999  COMP-3.
            //*
            //           10 STO-HG7576                 PIC S9(7)V9999  COMP-3.
            //*            TRATT 553 (41/42)
            //           10 STO-HG74                   PIC S9(7)V9999  COMP-3.
            //*            TRATT 553 (REND.INFORT)
            //           10 STO-HG80                   PIC S9(7)V9999  COMP-3.
            //*            TRATTENUTA LAVORA AUTONOMO
            //           10 STO-KD01                   PIC S9(7)V9999  COMP-3.
            //*            TRATTENUTA LAV.DIPENDENTE
            #endregion tracciato COBOL

            #region Tracciato Host
            // 02   STO-IMPORTI.
            // 05   STO-IMP     OCCURS 75 TIMES.
            // *            IMPORTI (LNG 64 X 75 =4800)
            // 10 STO-KC01.
            // *            DECORRENZA AAMM
            /// <summary>
            /// STO_KC01AA XXXX  
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public string STO_KC01AA { get; set; }

            /// <summary>
            /// STO_KC01MM XX  
            /// </summary>
            [HisFieldInfoMapping(1, 2)]
            public string STO_KC01MM { get; set; }

            /// <summary>
            /// STO_KC04 S9(7)V9(4) COMP-3 
            // *            ADEGUATA
            /// </summary>
            [HisFieldInfoMapping(2, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal STO_KC04 { get; set; }

            /// <summary>
            /// STO_KC10 S9(7)V9(4) COMP-3 
            // *            IN PAGAMENTO SENZA AAFF
            /// </summary>
            [HisFieldInfoMapping(3, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal STO_KC10 { get; set; }

            /// <summary>
            /// STO_KC03 S9(7)V9(4) COMP-3 
            // *            AA FF
            /// </summary>
            [HisFieldInfoMapping(4, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal STO_KC03 { get; set; }

            /// <summary>
            /// STO_KE04 S9(5)V9(4) COMP-3 
            // *            PRO RATA ESTERO
            /// </summary>
            [HisFieldInfoMapping(5, 5, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal STO_KE04 { get; set; }

            /// <summary>
            /// STO_HI01 S9(5)V9(4) COMP-3 
            // *            TRATT.SINDACALE
            /// </summary>
            [HisFieldInfoMapping(6, 5, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal STO_HI01 { get; set; }

            /// <summary>
            /// STO_HG87 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(7, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal STO_HG87 { get; set; }

            //*
            /// <summary>
            /// STO_HG7576 S9(7)V9(4) COMP-3 
            // *            TRATT 553 (41/42)
            /// </summary>
            [HisFieldInfoMapping(8, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal STO_HG7576 { get; set; }

            /// <summary>
            /// STO_HG74 S9(7)V9(4) COMP-3 
            // *            TRATT 553 (REND.INFORT)
            /// </summary>
            [HisFieldInfoMapping(9, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal STO_HG74 { get; set; }

            /// <summary>
            /// STO_HG80 S9(7)V9(4) COMP-3
            // *            TRATTENUTA LAVORA AUTONOMO 
            /// </summary>
            [HisFieldInfoMapping(10, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal STO_HG80 { get; set; }

            /// <summary>
            /// STO_KD01 S9(7)V9(4) COMP-3 
            // *            TRATTENUTA LAV.DIPENDENTE
            /// </summary>
            [HisFieldInfoMapping(11, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal STO_KD01 { get; set; }


            #endregion Tracciato Host
        }

        public class Solidare
        {
            #region Constructor
            internal Solidare()
            {
            }
            #endregion Constructor

            #region tracciato COBOL
            //      02   STO-SOLIDARI.
            //        05   STO-SOL     OCCURS 6 TIMES.
            //*            TRATT.SOLIDAR. (MESE/IMPORTO)
            //           10 STO-SOL-MES                PIC 99.
            //           10 STO-SOL-IMP                PIC S9(7)V9999 COMP-3.
            #endregion tracciato COBOL

            #region Tracciato Host
            // 02   STO-SOLIDARI.
            // 05   STO-SOL     OCCURS 6 TIMES.
            // *            TRATT.SOLIDAR. (MESE/IMPORTO)
            /// <summary>
            /// STO_SOL_MES 99  
            /// </summary>
            [HisFieldInfoMapping(0, 2)]
            public short STO_SOL_MES { get; set; }

            /// <summary>
            /// STO_SOL_IMP S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(1, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal STO_SOL_IMP { get; set; }
            #endregion Tracciato Host
        }

        #endregion nested class
    }
}
