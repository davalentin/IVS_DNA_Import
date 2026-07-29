using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class AreaRecordZeroCentro
    {
        #region Constructor
        internal AreaRecordZeroCentro()
        { }
        #endregion Constructor

        #region tracciato COBOL
        //   05  REC-ZERO-CENTRO.
        //10  FILLER            PIC X(4).
        //10  RIS-DOM-NUM       PIC X(8).
        //10  RIS-CAT-ALF       PIC X(8).
        //10  RIS-CER-PD        PIC X(4).
        //10  RIS-COD-SCI       PIC X(2).
        //10  RIS-COD-ERR.
        //  15  RIS-COD-ER1       PIC X(3).
        //  15  RIS-COD-ER2       PIC X(3).
        //  15  RIS-COD-ER3       PIC X(3).
        //  15  RIS-COD-ER4       PIC X(3).
        //  15  RIS-COD-ER5       PIC X(3).
        //  15  FILLER            PIC X(15).
        //10  RIS-DATA-AAMMGG   PIC X(8).
        //10  RIS-669-ERR    OCCURS 8 TIMES.
        //  15  RIS-669-ANN       PIC X(4).
        //  15  RIS-669-KEY.
        //      20  RIS-669-KEY-C     PIC X(3).
        //      20  RIS-669-KEY-S     PIC X(4).
        //      20  RIS-669-KEY-N     PIC X(8).
        //  15  RIS-669-COD       PIC X(1).
        //10  FILLER            PIC X(16).
        #endregion tracciato COBOL

        #region Tracciato Host
        // 05  REC-ZERO-CENTRO.
        /// <summary>
        /// FILLER X(4)  
        /// </summary>
        [HisFieldInfoMapping(0, 4)]
        public string FILLER1 { get; set; }

        /// <summary>
        /// RIS_DOM_NUM X(8)  
        /// </summary>
        [HisFieldInfoMapping(1, 8)]
        public string RIS_DOM_NUM { get; set; }

        /// <summary>
        /// RIS_CAT_ALF X(8)  
        /// </summary>
        [HisFieldInfoMapping(2, 8)]
        public string RIS_CAT_ALF { get; set; }

        /// <summary>
        /// RIS_CER_PD X(4)  
        /// </summary>
        [HisFieldInfoMapping(3, 4, CobolType = CobolType.Untraslate)]
        public int RIS_CER_PD { get; set; }

        /// <summary>
        /// RIS_COD_SCI X(2)  
        /// </summary>
        [HisFieldInfoMapping(4, 2)]
        public string RIS_COD_SCI { get; set; }

        // 10  RIS-COD-ERR.
        /// <summary>
        /// RIS_COD_ER1 X(3)  
        /// </summary>
        [HisFieldInfoMapping(5, 3)]
        public string RIS_COD_ER1 { get; set; }

        /// <summary>
        /// RIS_COD_ER2 X(3)  
        /// </summary>
        [HisFieldInfoMapping(6, 3)]
        public string RIS_COD_ER2 { get; set; }

        /// <summary>
        /// RIS_COD_ER3 X(3)  
        /// </summary>
        [HisFieldInfoMapping(7, 3)]
        public string RIS_COD_ER3 { get; set; }

        /// <summary>
        /// RIS_COD_ER4 X(3)  
        /// </summary>
        [HisFieldInfoMapping(8, 3)]
        public string RIS_COD_ER4 { get; set; }

        /// <summary>
        /// RIS_COD_ER5 X(3)  
        /// </summary>
        [HisFieldInfoMapping(9, 3)]
        public string RIS_COD_ER5 { get; set; }

        /// <summary>
        /// FILLER X(15)  
        /// </summary>
        [HisFieldInfoMapping(10, 15)]
        public string FILLER2 { get; set; }

        /// <summary>
        /// RIS_DATA_AAMMGG X(8)  
        /// </summary>
        [HisFieldInfoMapping(11, 8)]
        public string RIS_DATA_AAMMGG { get; set; }

        [HisComplexAreaInfoMapping(12, ListCount = 8)]
        public List<Risposta669> RISPOSTE669 { get; set; }

        
        [HisFieldInfoMapping(13, 1)]
        public string FLAG_INDEB  { get; set; }
        /// <summary>
        /// FILLER X(16)  
        /// </summary>
        [HisFieldInfoMapping(14, 17)]
        public string FILLER3 { get; set; }

        #endregion Tracciato Host

        #region nested class
        public class Risposta669
        {
            #region Constructor
            internal Risposta669()
            { }
            #endregion Constructor

            #region tracciato COBOL
            //10  RIS-669-ERR    OCCURS 8 TIMES.
            //  15  RIS-669-ANN       PIC X(4).
            //  15  RIS-669-KEY.
            //      20  RIS-669-KEY-C     PIC X(3).
            //      20  RIS-669-KEY-S     PIC X(4).
            //      20  RIS-669-KEY-N     PIC X(8).
            //  15  RIS-669-COD       PIC X(1).
            #endregion tracciato COBOL

            #region Tracciato Host
            // 10  RIS-669-ERR    OCCURS 8 TIMES.
            /// <summary>
            /// RIS_669_ANN X(4)  
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public string RIS_669_ANN { get; set; }

            // 15  RIS-669-KEY.
            /// <summary>
            /// RIS_669_KEY_C X(3)  
            /// </summary>
            [HisFieldInfoMapping(1, 3)]
            public string RIS_669_KEY_C { get; set; }

            /// <summary>
            /// RIS_669_KEY_S X(4)  
            /// </summary>
            [HisFieldInfoMapping(2, 4)]
            public string RIS_669_KEY_S { get; set; }

            /// <summary>
            /// RIS_669_KEY_N X(8)  
            /// </summary>
            [HisFieldInfoMapping(3, 8)]
            public string RIS_669_KEY_N { get; set; }

            /// <summary>
            /// RIS_669_COD X(1)  
            /// </summary>
            [HisFieldInfoMapping(4, 1)]
            public string RIS_669_COD { get; set; }
            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
