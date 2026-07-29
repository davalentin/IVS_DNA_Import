using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class Sentenze
    {
        #region Properties

        #region Tracciato COBOL
        //   *DATI DEL PANNELLO MRCSEN0 (SENTENZE)
        //02 T-GPSENT.
        //   03 T-GP2SEN0 OCCURS  5.
        //      04 T-GP2SEN1           PIC X(2).
        //      04 T-GP2SEN2           PIC X(2).
        //      04 T-GP2SEN3.
        //         05 T-GP2SEN3A       PIC 9(4).
        //         05 T-GP2SEN3M       PIC 9(2).
        //      04 T-GP2SEN4.
        //         05 T-GP2SEN4A       PIC 9(4).
        //         05 T-GP2SEN4M       PIC 9(2).
        //         04 T-GP2GRSEN          PIC X.
        //         04 T-GP2TRBSEN         PIC X(11).                         
        //         04 T-GP2ASEN           PIC X(4).
        //         04 T-GP2NSEN           PIC X(8).                         
        //         04 T-GP2DPSEN          PIC X(10).
        //   03 T-GP1AXE1-V            PIC 9.
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 5)]
        public List<T_GP2SEN0> LISTT_GP2SEN0 { get; set; }

        /// <summary>
        /// T_GP1AXE1_V 9  
        /// </summary>
        [HisFieldInfoMapping(1, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1AXE1_V { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class T_GP2SEN0
        {
            #region Properties

            #region Tracciato COBOL
            //   *DATI DEL PANNELLO MRCSEN0 (SENTENZE)
            //02 T-GPSENT.
            //   03 T-GP2SEN0 OCCURS  3.
            //      04 T-GP2SEN1           PIC X.
            //      04 T-GP2SEN2           PIC X.
            //      04 T-GP2SEN3.
            //         05 T-GP2SEN3A       PIC 9(4).
            //         05 T-GP2SEN3M       PIC 9(2).
            //      04 T-GP2SEN4.
            //         05 T-GP2SEN4A       PIC 9(4).
            //         05 T-GP2SEN4M       PIC 9(2).
            #endregion Tracciato COBOL

            #region Tracciato Host
            // *DATI DEL PANNELLO MRCSEN0 (SENTENZE)
            // 02 T-GPSENT.
            // 03 T-GP2SEN0 OCCURS  3.
            /// <summary>
            /// T_GP2SEN1 XX 
            /// </summary>
            [HisFieldInfoMapping(0, 2)]
            public string T_GP2SEN1 { get; set; }

            /// <summary>
            /// T_GP2SEN2 XX  
            /// </summary>
            [HisFieldInfoMapping(1, 2)]
            public string T_GP2SEN2 { get; set; }

            // 04 T-GP2SEN3.
            /// <summary>
            /// T_GP2SEN3A 9(4)  
            /// </summary>
            [HisFieldInfoMapping(2, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2SEN3A { get; set; }

            /// <summary>
            /// T_GP2SEN3M 9(2)  
            /// </summary>
            [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2SEN3M { get; set; }

            // 04 T-GP2SEN4.
            /// <summary>
            /// T_GP2SEN4A 9(4)  
            /// </summary>
            [HisFieldInfoMapping(4, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2SEN4A { get; set; }

            /// <summary>
            /// T_GP2SEN4M 9(2)  
            /// </summary>
            [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2SEN4M { get; set; }

            /// <summary>
            /// T_GP2GRSEN          PIC X.
            /// </summary>
            [HisFieldInfoMapping(6, 1)]
            public string T_GP2GRSEN { get; set; }

            /// <summary>
            /// T_GP2TRBSEN         PIC X(11).  
            /// </summary>
            [HisFieldInfoMapping(7, 11)]
            public string T_GP2TRBSEN { get; set; }

            /// <summary>
            /// T_GP2ASEN           PIC X(4).
            /// </summary>
            [HisFieldInfoMapping(8, 4)]
            public string T_GP2ASEN { get; set; }

            /// <summary>
            /// T_GP2NSEN           PIC X(8).
            /// </summary>
            [HisFieldInfoMapping(9, 8)]
            public string T_GP2NSEN { get; set; }

            /// <summary>
            /// T_GP2DPSEN          PIC X(10).
            /// </summary>
            [HisFieldInfoMapping(10, 10)]
            public string T_GP2DPSEN { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
