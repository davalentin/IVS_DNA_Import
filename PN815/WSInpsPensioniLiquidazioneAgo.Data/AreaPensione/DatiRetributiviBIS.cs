using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class DatiRetributiviBIS
    {
        #region Properties

        #region Tracciato COBOL
        //02 T-GPRET0-BIS.
        //   03 T-GP2BC00-BIS OCCURS 30.
        //      04 T-GP2BC01-BIS.
        //         05 T-GP2BC01A-BIS       PIC 9(4).
        //         05 T-GP2BC01M-BIS       PIC 9(2).
        //      04 T-GP2BC02-BIS           PIC S9(5) COMP-3.
        //      04 T-GP2BC03-BIS           PIC S9(7)V9(6) COMP-3.
        //      04 T-GP2BC04-BIS           PIC S9(5) COMP-3.
        //      04 T-GP2BC05-BIS           PIC S9(5)V9(6) COMP-3.
        //      04 T-GP2BC08-BIS           PIC S9(5) COMP-3.
        //      04 T-GP2BC09-BIS           PIC X(2).
        //      04 T-GP2BC10-BIS           PIC S9(5) COMP-3.
        //      04 T-GP2BC0A-BIS           PIC 9.
        //      04 T-GP2BC0B-BIS           PIC X.
        //      04 T-GP2BC0C-BIS           PIC X.
        //      04 T-GP2BC0D-BIS           PIC S9(7)V9(4) COMP-3.
        //      04 T-GP2BC0F-BIS           PIC S9(7)V9(4) COMP-3.
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 30)]
        public List<T_GP2BC00_BIS> LISTT_GP2BC00_BIS { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class T_GP2BC00_BIS
        {
            #region Properties

            #region Tracciato COBOL
            //02 T-GPRET0-BIS.
            //   03 T-GP2BC00-BIS OCCURS 30.
            //      04 T-GP2BC01-BIS.
            //         05 T-GP2BC01A-BIS       PIC 9(4).
            //         05 T-GP2BC01M-BIS       PIC 9(2).
            //      04 T-GP2BC02-BIS           PIC S9(5) COMP-3.
            //      04 T-GP2BC03-BIS           PIC S9(7)V9(6) COMP-3.
            //      04 T-GP2BC04-BIS           PIC S9(5) COMP-3.
            //      04 T-GP2BC05-BIS           PIC S9(5)V9(6) COMP-3.
            //      04 T-GP2BC08-BIS           PIC S9(5) COMP-3.
            //      04 T-GP2BC09-BIS           PIC X(2).
            //      04 T-GP2BC10-BIS           PIC S9(5) COMP-3.
            //      04 T-GP2BC0A-BIS           PIC 9.
            //      04 T-GP2BC0B-BIS           PIC X.
            //      04 T-GP2BC0C-BIS           PIC X.
            //      04 T-GP2BC0D-BIS           PIC S9(7)V9(4) COMP-3.
            //      04 T-GP2BC0F-BIS           PIC S9(7)V9(4) COMP-3.
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 T-GPRET0-BIS.
            // 03 T-GP2BC00-BIS OCCURS 30.
            // 04 T-GP2BC01-BIS.
            /// <summary>
            /// T_GP2BC01A_BIS 9(4)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BC01A_BIS { get; set; }

            /// <summary>
            /// T_GP2BC01M_BIS 9(2)  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BC01M_BIS { get; set; }

            /// <summary>
            /// T_GP2BC02_BIS S9(5) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(2, 3, CobolType = CobolType.Comp3)]
            public int T_GP2BC02_BIS { get; set; }

            /// <summary>
            /// T_GP2BC03_BIS S9(7)V9(6) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(3, 7, Scale = 6, CobolType = CobolType.Comp3)]
            public decimal T_GP2BC03_BIS { get; set; }

            /// <summary>
            /// T_GP2BC04_BIS S9(5) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(4, 3, CobolType = CobolType.Comp3)]
            public int T_GP2BC04_BIS { get; set; }

            /// <summary>
            /// T_GP2BC05_BIS S9(5)V9(6) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(5, 6, Scale = 6, CobolType = CobolType.Comp3)]
            public decimal T_GP2BC05_BIS { get; set; }

            /// <summary>
            /// T_GP2BC08_BIS S9(5) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(6, 3, CobolType = CobolType.Comp3)]
            public int T_GP2BC08_BIS { get; set; }

            /// <summary>
            /// T_GP2BC09_BIS X(2)  
            /// </summary>
            [HisFieldInfoMapping(7, 2)]
            public string T_GP2BC09_BIS { get; set; }

            /// <summary>
            /// T_GP2BC10_BIS S9(5) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(8, 3, CobolType = CobolType.Comp3)]
            public int T_GP2BC10_BIS { get; set; }

            /// <summary>
            /// T_GP2BC0A_BIS 9  
            /// </summary>
            [HisFieldInfoMapping(9, 1, CobolType = CobolType.Unsigned)]
            public short T_GP2BC0A_BIS { get; set; }

            /// <summary>
            /// T_GP2BC0B_BIS X
            /// <summary>
            [HisFieldInfoMapping(10, 1)]
            public string T_GP2BC0B_BIS { get; set; }

            /// <summary>
            /// T_GP2BC0C_BIS X
            /// <summary>
            [HisFieldInfoMapping(11, 1)]
            public string T_GP2BC0C_BIS { get; set; }

            /// <summary>
            /// T_GP2BC0D_BIS S9(7)V9(4) COMP-3
            /// <summary>
            [HisFieldInfoMapping(12, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal T_GP2BC0D_BIS { get; set; }
            /// <summary>
            /// T_GP2BC0F_BIS S9(7)V9(4) COMP-3
            /// <summary>
            [HisFieldInfoMapping(13, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal T_GP2BC0F_BIS { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
