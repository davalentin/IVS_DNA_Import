using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class DatiNuovi
    {
        #region Properties

        #region Tracciato COBOL
        //      02 T-DATINUOVI.
        //03 T-ERRORI-QRED.
        //   04 T-TABERR-QRED OCCURS 3.
        //      05 T-KEYERR-QRED       PIC 9(15).
        //      05 T-ANNOERR-QRED      PIC 9(4).
        //      05 T-TIPOERR-QRED      PIC X.
        //03 T-PENCONTR                PIC 9.
        //03 T-GP1AN06Z.
        //   04 T-GP1AN06ZG            PIC 9(2).
        //   04 T-GP1AN06ZM            PIC 9(2).
        //   04 T-GP1AN06ZA            PIC 9(4).
        //03 FILLER-C36                PIC XXX.
        //03 T-GP1AV91H                PIC 9.
        //03 FILLER-AF09               PIC X(6).
        //03 T-TP1NDOM-V                  PIC 9(8).
        //03 T-GP1AXBA                 PIC X(3).
        //03 T-F-TELE                  PIC X.
        //03 FILLER                    PIC X(4).     
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 3)]
        public List<T_TABERR_QRED> LISTT_TABERR_QRED { get; set; }

        /// <summary>
        /// T_PENCONTR 9  
        /// </summary>
        [HisFieldInfoMapping(1, 1, CobolType = CobolType.Unsigned)]
        public short T_PENCONTR { get; set; }

        // 03 T-GP1AN06Z.
        /// <summary>
        /// T_GP1AN06ZG 9(2)  
        /// </summary>
        [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AN06ZG { get; set; }

        /// <summary>
        /// T_GP1AN06ZM 9(2)  
        /// </summary>
        [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AN06ZM { get; set; }

        /// <summary>
        /// T_GP1AN06ZA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(4, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1AN06ZA { get; set; }

        /// <summary>
        /// FILLER_C36 XXX  
        /// </summary>
        [HisFieldInfoMapping(5, 3)]
        public string FILLER_C36 { get; set; }

        /// <summary>
        /// T_GP1AV91H 9  
        /// </summary>
        [HisFieldInfoMapping(6, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1AV91H { get; set; }

        /// <summary>
        /// FILLER_AF09 X(6)  
        /// </summary>
        [HisFieldInfoMapping(7, 6)]
        public string FILLER_AF09 { get; set; }

        /// <summary>
        /// T_TP1NDOM_V 9(8)  
        /// </summary>
        [HisFieldInfoMapping(8, 8, CobolType = CobolType.Unsigned)]
        public long T_TP1NDOM_V { get; set; }

        /// <summary>
        /// T_GP1AXBA X(3)  
        /// </summary>
        [HisFieldInfoMapping(9, 3)]
        public string T_GP1AXBA { get; set; }

        /// <summary>
        /// T_F_TELE X  
        /// </summary>
        [HisFieldInfoMapping(10, 1)]
        public string T_F_TELE { get; set; }

        /// <summary>
        /// FILLER X(4)  
        /// </summary>
        [HisFieldInfoMapping(11, 4)]
        public string FILLER { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class T_TABERR_QRED
        {
            #region Properties

            #region Tracciato COBOL
            //      02 T-DATINUOVI.
            //03 T-ERRORI-QRED.
            //   04 T-TABERR-QRED OCCURS 3.
            //      05 T-KEYERR-QRED       PIC 9(15).
            //      05 T-ANNOERR-QRED      PIC 9(4).
            //      05 T-TIPOERR-QRED      PIC X.     
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 T-DATINUOVI.
            // 03 T-ERRORI-QRED.
            // 04 T-TABERR-QRED OCCURS 3.
            /// <summary>
            /// T_KEYERR_QRED 9(15)  
            /// </summary>
            [HisFieldInfoMapping(0, 15, CobolType = CobolType.Unsigned)]
            public long T_KEYERR_QRED { get; set; }

            /// <summary>
            /// T_ANNOERR_QRED 9(4)  
            /// </summary>
            [HisFieldInfoMapping(1, 4, CobolType = CobolType.Unsigned)]
            public short T_ANNOERR_QRED { get; set; }

            /// <summary>
            /// T_TIPOERR_QRED X  
            /// </summary>
            [HisFieldInfoMapping(2, 1)]
            public string T_TIPOERR_QRED { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
