using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class Bititolarieta
    {
        #region Constructor
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //   *DATI DEL PANNELLO MRCBIT0 (BITITOLARITA')
        //02 T-GPBIT0.
        //   03 T-GP2A15  OCCURS 5.
        //      04 T-GP2CAT            PIC X(3).
        //      04 T-GP2ENTE           PIC 9(4).
        //      04 T-GP2CER            PIC 9(10).
        //      04 T-GP2DEC.
        //         05 T-GP2DECA        PIC 9(4).
        //         05 T-GP2DECM        PIC 9(2).
        //      04 T-GP2CES.
        //         05 T-GP2CESA        PIC 9(4).
        //         05 T-GP2CESM        PIC 9(2).
        //      04 T-GP2CODU           PIC X.
        //      04 T-GP2CTM            PIC X.
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 5)]
        public List<T_GP2A15> LISTT_GP2A15 { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class T_GP2A15
        {
            #region Properties

            #region Tracciato COBOL
            //   *DATI DEL PANNELLO MRCBIT0 (BITITOLARITA')
            //02 T-GPBIT0.
            //   03 T-GP2A15  OCCURS 5.
            //      04 T-GP2CAT            PIC X(3).
            //      04 T-GP2ENTE           PIC 9(4).
            //      04 T-GP2CER            PIC 9(10).
            //      04 T-GP2DEC.
            //         05 T-GP2DECA        PIC 9(4).
            //         05 T-GP2DECM        PIC 9(2).
            //      04 T-GP2CES.
            //         05 T-GP2CESA        PIC 9(4).
            //         05 T-GP2CESM        PIC 9(2).
            //      04 T-GP2CODU           PIC X.
            //      04 T-GP2CTM            PIC X.
            #endregion Tracciato COBOL

            #region Tracciato Host
            // *DATI DEL PANNELLO MRCBIT0 (BITITOLARITA')
            // 02 T-GPBIT0.
            // 03 T-GP2A15  OCCURS 5.
            /// <summary>
            /// T_GP2CAT X(3)  
            /// </summary>
            [HisFieldInfoMapping(0, 3)]
            public string T_GP2CAT { get; set; }

            /// <summary>
            /// T_GP2ENTE 9(4)  
            /// </summary>
            [HisFieldInfoMapping(1, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2ENTE { get; set; }

            /// <summary>
            /// T_GP2CER 9(10)  
            /// </summary>
            [HisFieldInfoMapping(2, 10, CobolType = CobolType.Unsigned)]
            public long T_GP2CER { get; set; }

            // 04 T-GP2DEC.
            /// <summary>
            /// T_GP2DECA 9(4)  
            /// </summary>
            [HisFieldInfoMapping(3, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2DECA { get; set; }

            /// <summary>
            /// T_GP2DECM 9(2)  
            /// </summary>
            [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2DECM { get; set; }

            // 04 T-GP2CES.
            /// <summary>
            /// T_GP2CESA 9(4)  
            /// </summary>
            [HisFieldInfoMapping(5, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2CESA { get; set; }

            /// <summary>
            /// T_GP2CESM 9(2)  
            /// </summary>
            [HisFieldInfoMapping(6, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2CESM { get; set; }

            /// <summary>
            /// T_GP2CODU X  
            /// </summary>
            [HisFieldInfoMapping(7, 1)]
            public string T_GP2CODU { get; set; }

            /// <summary>
            /// T_GP2CTM X  
            /// </summary>
            [HisFieldInfoMapping(8, 1)]
            public string T_GP2CTM { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
