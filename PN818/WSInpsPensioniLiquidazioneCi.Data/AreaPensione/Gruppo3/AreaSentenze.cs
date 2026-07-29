using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaSentenze
    {
        #region tracciato COBOL
        //       04 IAREA-SENT.
        //        05  ICISEN OCCURS 3.
        //            10  ICISEN1              PIC 9.
        //* PRESENZA DI SENTENZE C.C. 0/1
        //            10  ICISEN2              PIC 9.
        //* SENTENZE C.C.: 1=495
        //* DEC. SENTENZA C.C.
        //                15  ICISEN3A        PIC 9999.
        //                15  ICISEN3M        PIC 99.
        #endregion tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 3)]
        public List<Sentenza> SENTENZE { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class Sentenza
        {
            #region tracciato COBOL
            //       04 IAREA-SENT.
            //        05  ICISEN OCCURS 3.
            //            10  ICISEN1              PIC 9.
            //* PRESENZA DI SENTENZE C.C. 0/1
            //            10  ICISEN2              PIC 9.
            //* SENTENZE C.C.: 1=495
            //* DEC. SENTENZA C.C.
            //                15  ICISEN3A        PIC 9999.
            //                15  ICISEN3M        PIC 99.
            #endregion tracciato COBOL

            #region Tracciato Host
            // 04 IAREA-SENT.
            // 05  ICISEN OCCURS 3.
            /// <summary>
            /// ICISEN1 9  
            /// * PRESENZA DI SENTENZE C.C. 0/1
            /// </summary>
            [HisFieldInfoMapping(0, 1)]
            public short ICISEN1 { get; set; }

            /// <summary>
            /// ICISEN2 9  
            /// * SENTENZE C.C.: 1=495
            /// </summary>
            [HisFieldInfoMapping(1, 1)]
            public short ICISEN2 { get; set; }

            /// <summary>
            /// ICISEN3A 9999  
            /// * DEC. SENTENZA C.C.
            /// </summary>
            [HisFieldInfoMapping(2, 4)]
            public short ICISEN3A { get; set; }

            /// <summary>
            /// ICISEN3M 99  
            /// * DEC. SENTENZA C.C.
            /// </summary>
            [HisFieldInfoMapping(3, 2)]
            public short ICISEN3M { get; set; }
            #endregion Tracciato Host
        }
        #endregion nested class

    }
}
