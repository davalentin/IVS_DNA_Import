using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class PensioniAbbinate
    {
        #region Properties

        #region Tracciato COBOL
        //   *DATI DEL PANNELLO MRCABB0 (PENSIONI ABBINATE)
        //02 T-GPABB0.
        //   03 T-GP2BYB OCCURS 5.
        //      04 T-GP2BYB1-V         PIC 9(15) COMP-3.
        //      04 T-GP2BYB2-V         PIC 9(4).
        //   03 T-GP2BYC0 OCCURS 10.
        //      04 T-GP2BYC1-V         PIC 9(15) COMP-3.
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 5)]
        public List<T_GPABB0> LISTT_GPABB0 { get; set; }

        [HisComplexAreaInfoMapping(1, ListCount = 10)]
        public List<T_GP2BYC0> LISTT_GP2BYC0 { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class T_GPABB0
        {
            #region Properties

            #region Tracciato COBOL
            //   *DATI DEL PANNELLO MRCABB0 (PENSIONI ABBINATE)
            //02 T-GPABB0.
            //   03 T-GP2BYB OCCURS 5.
            //      04 T-GP2BYB1-V         PIC 9(15) COMP-3.
            //      04 T-GP2BYB2-V         PIC 9(4).
            #endregion Tracciato COBOL

            #region Tracciato Host
            // *DATI DEL PANNELLO MRCABB0 (PENSIONI ABBINATE)
            // 02 T-GPABB0.
            // 03 T-GP2BYB OCCURS 5.
            /// <summary>
            /// T_GP2BYB1_V 9(15) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(0, 8, CobolType = CobolType.Comp3Unsigned)]
            public long T_GP2BYB1_V { get; set; }

            /// <summary>
            /// T_GP2BYB2_V 9(4)  
            /// </summary>
            [HisFieldInfoMapping(1, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BYB2_V { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }

        public class T_GP2BYC0
        {
            #region Properties

            #region Tracciato COBOL
            //   03 T-GP2BYC0 OCCURS 10.
            //      04 T-GP2BYC1-V         PIC 9(15) COMP-3.
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 03 T-GP2BYC0 OCCURS 10.
            /// <summary>
            /// T_GP2BYC1_V 9(15) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(0, 8, CobolType = CobolType.Comp3Unsigned)]
            public long T_GP2BYC1_V { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
