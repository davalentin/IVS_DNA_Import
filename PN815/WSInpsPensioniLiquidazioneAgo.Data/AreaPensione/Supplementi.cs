using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class Supplementi
    {
        #region Properties

        #region Tracciato COBOL
        //   * DATI DEL PANNELLO MRCSUP0 (SUPPLEMENTI)
        //02 T-GPSUP0.
        //   03 T-GP2BE00 OCCURS 30.
        //      04 T-GP2BE01.
        //         05 T-GP2BE01A       PIC 9(4).
        //         05 T-GP2BE01M       PIC 9(2).
        //      04 T-GP2BE02           PIC XX.
        //      04 T-GP2BE03           PIC S9(7)V9(4) COMP-3.
        //      04 T-GP2BE04           PIC S9(7)V9(4) COMP-3.
        //      04 T-GP2BE05           PIC S9(7)V9(4) COMP-3.
        //      04 T-GP2BE06           PIC S9(7) COMP-3.
        //      04 T-GP2BE07           PIC 9.
        //      04 T-GP2BE08           PIC S9(7) COMP-3.
        //      04 T-GP2BE09           PIC S9(7) COMP-3.
        //      04 T-GP2BE10           PIC X.
        //04 T-GP2BE11RZ.
        //            05 T-GP2BE11RZG     PIC 9(2).
        //            05 T-GP2BE11RZM     PIC 9(2).
        //            05 T-GP2BE11RZA     PIC 9(4). 
        //04 T-GP2BE12RZ.
        //            05 T-GP2BE12RZG     PIC 9(2).
        //            05 T-GP2BE12RZM     PIC 9(2).
        //            05 T-GP2BE12RZA     PIC 9(4). 
        //      04 T-GP2BE0B           PIC X.
        //      04 T-GP2BE0C           PIC X.
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 30)]
        public List<T_GP2BE00> LISTT_GP2BE00 { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class T_GP2BE00
        {
            #region Properties

            #region Tracciato COBOL
            //   * DATI DEL PANNELLO MRCSUP0 (SUPPLEMENTI)
            //02 T-GPSUP0.
            //   03 T-GP2BE00 OCCURS 30.
            //      04 T-GP2BE01.
            //         05 T-GP2BE01A       PIC 9(4).
            //         05 T-GP2BE01M       PIC 9(2).
            //      04 T-GP2BE02           PIC XX.
            //      04 T-GP2BE03           PIC S9(7)V9(4) COMP-3.
            //      04 T-GP2BE04           PIC S9(7)V9(4) COMP-3.
            //      04 T-GP2BE05           PIC S9(7)V9(4) COMP-3.
            //      04 T-GP2BE06           PIC S9(7) COMP-3.
            //      04 T-GP2BE07           PIC 9.
            //      04 T-GP2BE08           PIC S9(7) COMP-3.
            //      04 T-GP2BE09           PIC S9(7) COMP-3.
            //      04 T-GP2BE10           PIC X.
            //04 T-GP2BE11RZ.
            //            05 T-GP2BE11RZG     PIC 9(2).
            //            05 T-GP2BE11RZM     PIC 9(2).
            //            05 T-GP2BE11RZA     PIC 9(4). 
            //04 T-GP2BE12RZ.
            //            05 T-GP2BE12RZG     PIC 9(2).
            //            05 T-GP2BE12RZM     PIC 9(2).
            //            05 T-GP2BE12RZA     PIC 9(4).
            //      04 T-GP2BE0B           PIC X.
            //      04 T-GP2BE0C           PIC X.
            #endregion Tracciato COBOL

            #region Tracciato Host
            // * DATI DEL PANNELLO MRCSUP0 (SUPPLEMENTI)
            // 02 T-GPSUP0.
            // 03 T-GP2BE00 OCCURS 30.
            // 04 T-GP2BE01.
            /// <summary>
            /// T_GP2BE01A 9(4)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BE01A { get; set; }

            /// <summary>
            /// T_GP2BE01M 9(2)  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BE01M { get; set; }

            /// <summary>
            /// T_GP2BE02 XX  
            /// </summary>
            [HisFieldInfoMapping(2, 2)]
            public string T_GP2BE02 { get; set; }

            /// <summary>
            /// T_GP2BE03 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(3, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal T_GP2BE03 { get; set; }

            /// <summary>
            /// T_GP2BE04 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(4, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal T_GP2BE04 { get; set; }

            /// <summary>
            /// T_GP2BE05 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(5, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal T_GP2BE05 { get; set; }

            /// <summary>
            /// T_GP2BE06 S9(7) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(6, 4, CobolType = CobolType.Comp3)]
            public int T_GP2BE06 { get; set; }

            /// <summary>
            /// T_GP2BE07 9  
            /// </summary>
            [HisFieldInfoMapping(7, 1, CobolType = CobolType.Unsigned)]
            public short T_GP2BE07 { get; set; }

            /// <summary>
            /// T_GP2BE08 S9(7) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(8, 4, CobolType = CobolType.Comp3)]
            public int T_GP2BE08 { get; set; }

            /// <summary>
            /// T_GP2BE09 S9(7) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(9, 4, CobolType = CobolType.Comp3)]
            public int T_GP2BE09 { get; set; }

            /// <summary>
            /// T_GP2BE10 X  
            /// </summary>
            [HisFieldInfoMapping(10, 1)]
            public string T_GP2BE10 { get; set; }

            /// <summary>
            /// T_GP2BE11RZG     PIC 9(2).
            /// <summary>
            [HisFieldInfoMapping(11, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BE11RZG { get; set; }

            /// <summary>
            /// T_GP2BE11RZM     PIC 9(2).
            /// <summary>
            [HisFieldInfoMapping(12, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BE11RZM { get; set; }

            /// <summary>
            /// T_GP2BE11RZA     PIC 9(4). 
            /// <summary>
            [HisFieldInfoMapping(13, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BE11RZA { get; set; }

            /// <summary>
            /// T_GP2BE12RZG     PIC 9(2).
            /// <summary>
            [HisFieldInfoMapping(14, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BE12RZG { get; set; }

            /// <summary>
            /// T_GP2BE12RZM     PIC 9(2).
            /// <summary>
            [HisFieldInfoMapping(15, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BE12RZM { get; set; }

            /// <summary>
            /// T_GP2BE12RZA     PIC 9(4).
            /// <summary>
            [HisFieldInfoMapping(16, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BE12RZA { get; set; }

            /// <summary>
            /// T_GP2BE0B X
            /// <summary>
            [HisFieldInfoMapping(17, 1)]
            public string T_GP2BE0B { get; set; }

            /// <summary>
            /// T_GP2BE0C X
            /// <summary>
            [HisFieldInfoMapping(18, 1)]
            public string T_GP2BE0C { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
