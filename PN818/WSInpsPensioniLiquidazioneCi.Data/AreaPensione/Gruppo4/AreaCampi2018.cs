using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaCampi2018
    {
        #region tracciato COBOL
        //  04  DATI-2018.
        //      05  FELPE-TABONERI.
        //          06  FELPE-ONERI OCCURS 8.
        //              07  FELPE-DECONERE      PIC X(08).
        //              07  FELPE-SCADENZA      PIC X(08).
        //              07  FELPE-CODGRUP       PIC X(04).
        //              07  FELPE-CODSGRUP      PIC X(04).
        //              07  FELPE-ANZCON        PIC 9(04).
        //              07  FELPE-ONERE         PIC S9(07)V9(04).
        //              07  FELPE-CODBENEF      PIC X(02).
        //              07  FELPE-ANZBENEF      PIC 9(04).
        //              07  FELPE-CODINV        PIC X(02).
        //              07  FELPE-GP2PBNFGL     PIC X(02).
        //      05	GP2BC0DOBG                  PIC S9(7)V9(4) COMP-3.
        //      05	GP2BC0DART                  PIC S9(7)V9(4) COMP-3.
        //      05	GP2BC0DCOM                  PIC S9(7)V9(4) COMP-3.
        //      05	GP2BC0DCDM                  PIC S9(7)V9(4) COMP-3.
        //      05	GP2BB0DOBG                  PIC S9(7)V9(4) COMP-3.
        //      05	GP2BB0DART                  PIC S9(7)V9(4) COMP-3.
        //      05	GP2BB0DCOM                  PIC S9(7)V9(4) COMP-3.
        //      05	GP2BB0DCDM                  PIC S9(7)V9(4) COMP-3.
        //      05	GP1CARPE                    PIC X.
        //      05	TIPO-ELABORAZIONE           PIC X.
        //      05	FILLER2017-3                PIC X(1077).

        #endregion tracciato COBOL

        #region Tracciato Host
        /// <summary>
        ///      05  FELPE-TABONERI.
        ///          06  FELPE-ONERI OCCURS 8.
        /// <summary>
        [HisComplexAreaInfoMapping(0, ListCount = 8)]
        public List<Felpe_Oneri> FELPE_ONERI { get; set; }

        /// <summary>
        /// GP2BC0DOBG S9(7)V9(4) COMP-3
        /// </summary>
        [HisFieldInfoMapping(1, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal GP2BC0DOBG { get; set; }

        /// <summary>
        /// GP2BC0DART S9(7)V9(4) COMP-3
        /// </summary>
        [HisFieldInfoMapping(2, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal GP2BC0DART { get; set; }

        /// <summary>
        /// GP2BC0DCOM S9(7)V9(4) COMP-3
        /// </summary>
        [HisFieldInfoMapping(3, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal GP2BC0DCOM { get; set; }

        /// <summary>
        /// GP2BC0DCDM S9(7)V9(4) COMP-3
        /// </summary>
        [HisFieldInfoMapping(4, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal GP2BC0DCDM { get; set; }

        /// <summary>
        /// GP2BB0DOBG S9(7)V9(4) COMP-3
        /// </summary>
        [HisFieldInfoMapping(5, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal GP2BB0DOBG { get; set; }

        /// <summary>
        /// GP2BB0DART S9(7)V9(4) COMP-3
        /// </summary>
        [HisFieldInfoMapping(6, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal GP2BB0DART { get; set; }

        /// <summary>
        /// GP2BB0DCOM S9(7)V9(4) COMP-3
        /// </summary>
        [HisFieldInfoMapping(7, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal GP2BB0DCOM { get; set; }

        /// <summary>
        /// GP2BB0DCDM S9(7)V9(4) COMP-3
        /// </summary>
        [HisFieldInfoMapping(8, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal GP2BB0DCDM { get; set; }

        /// <summary>
        /// GP1CARPE X
        /// </summary>
        [HisFieldInfoMapping(9, 1)]
        public string GP1CARPE { get; set; }

        /// <summary>
        /// TIPO-ELABORAZIONE X
        /// </summary>
        [HisFieldInfoMapping(10, 1)]
        public string TIPO_ELABORAZIONE { get; set; }

        /// <summary>
        /// FLAG-5000 X
        /// </summary>
        [HisFieldInfoMapping(11, 1)]
        public string FLAG_5000 { get; set; }

        [HisComplexAreaInfoMapping(12)]
        public AreaINAIL AreaINAIL { get; set; }

        [HisFieldInfoMapping(13, 4)]
        public string IGP2BO10 { get; set; }

        [HisFieldInfoMapping(14, 1)]
        public string IGP1AJSP { get; set; }
        /// <summary>
        /// FILLER2017_3 X(491)
        /// </summary>
        [HisFieldInfoMapping(15, 485)]
        public string FILLER2017_3 { get; set; }
        #endregion Tracciato Host

        #region nested classes
        public class Felpe_Oneri
        {
            #region tracciato COBOL
            /// FELPE_DECONERE X(08)
            /// FELPE_SCADENZA X(08)
            /// FELPE_CODGRUP X(04)
            /// FELPE_CODSGRUP X(04)
            /// FELPE_ANZCON 9(04)
            /// FELPE_ONERE S9(07)V9(04)
            /// FELPE_CODBENEF X(02)
            /// FELPE_ANZBENEF 9(04)
            /// FELPE_CODINV X(02) 
            /// FELPE_GP2PBNFGL X(02)
            #endregion tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// FELPE_DECONERE X(08)
            /// <summary>
            [HisFieldInfoMapping(0, 8)]
            public string FELPE_DECONERE { get; set; }

            /// <summary>
            /// FELPE_SCADENZA X(08)
            /// <summary>
            [HisFieldInfoMapping(1, 8)]
            public string FELPE_SCADENZA { get; set; }

            /// <summary>
            /// FELPE_CODGRUP X(04)
            /// <summary>
            [HisFieldInfoMapping(2, 4)]
            public string FELPE_CODGRUP { get; set; }

            /// <summary>
            /// FELPE_CODSGRUP X(04)
            /// <summary>
            [HisFieldInfoMapping(3, 4)]
            public string FELPE_CODSGRUP { get; set; }

            /// <summary>
            /// FELPE_ANZCON 9(04)
            /// <summary>
            [HisFieldInfoMapping(4, 4)]
            public int FELPE_ANZCON { get; set; }

            /// <summary>
            /// FELPE_ONERE S9(07)V9(04)
            /// <summary>
            [HisFieldInfoMapping(5, 11, Scale = 4, CobolType = CobolType.Signed)]
            public decimal FELPE_ONERE { get; set; }

            /// <summary>
            /// FELPE_CODBENEF X(02)
            /// <summary>
            [HisFieldInfoMapping(6, 2)]
            public string FELPE_CODBENEF { get; set; }

            /// <summary>
            /// FELPE_ANZBENEF 9(04)
            /// <summary>
            [HisFieldInfoMapping(7, 4)]
            public int FELPE_ANZBENEF { get; set; }

            /// <summary>
            /// FELPE_CODINV X(02) 
            /// <summary>
            [HisFieldInfoMapping(8, 2)]
            public string FELPE_CODINV { get; set; }

            /// <summary>
            /// FELPE-GP2PBNFGL X(02).
            /// </summary>
            [HisFieldInfoMapping(9, 2)]
            public string FELPE_GP2PBNFGL { get; set; }
            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
