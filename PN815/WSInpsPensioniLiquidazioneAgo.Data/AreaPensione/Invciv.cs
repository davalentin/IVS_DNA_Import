using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class Invciv
    {
        #region Properties

        #region Tracciato COBOL
        //   *DATI DEL PANNELLO MRCINV0 (DATI INVCIV-AS-PS)
        //02 T-GPINV0.
        //   03 T-GP2IC10 OCCURS 50.
        //      04 T-GP2IC11.
        //         05 T-GP2IC11A       PIC 9(4).
        //         05 T-GP2IC11M       PIC 9(2).
        //      04 T-GP2IC12           PIC X(2).
        //   03 T-GP2BB061-V           PIC S9(8)V9(7) COMP-3.
        //   03 T-GP2BB062-V           PIC S9(8)V9(7) COMP-3.
        //   03 T-GP1AV31              PIC 9.
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 50)]
        public List<T_GP2IC10> LISTT_GP2IC10 { get; set; }

        /// <summary>
        /// T_GP2BB061_V S9(8)V9(7) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(1, 8, Scale = 7, CobolType = CobolType.Comp3)]
        public decimal T_GP2BB061_V { get; set; }

        /// <summary>
        /// T_GP2BB062_V S9(8)V9(7) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(2, 8, Scale = 7, CobolType = CobolType.Comp3)]
        public decimal T_GP2BB062_V { get; set; }

        /// <summary>
        /// T_GP1AV31 9  
        /// </summary>
        [HisFieldInfoMapping(3, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1AV31 { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class T_GP2IC10
        {
            #region Properties

            #region Tracciato COBOL
            //   *DATI DEL PANNELLO MRCINV0 (DATI INVCIV-AS-PS)
            //02 T-GPINV0.
            //   03 T-GP2IC10 OCCURS 50.
            //      04 T-GP2IC11.
            //         05 T-GP2IC11A       PIC 9(4).
            //         05 T-GP2IC11M       PIC 9(2).
            //      04 T-GP2IC12           PIC X(2).
            #endregion Tracciato COBOL

            #region Tracciato Host
            // *DATI DEL PANNELLO MRCINV0 (DATI INVCIV-AS-PS)
            // 02 T-GPINV0.
            // 03 T-GP2IC10 OCCURS 50.
            // 04 T-GP2IC11.
            /// <summary>
            /// T_GP2IC11A 9(4)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2IC11A { get; set; }

            /// <summary>
            /// T_GP2IC11M 9(2)  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2IC11M { get; set; }

            /// <summary>
            /// T_GP2IC12 X(2)  
            /// </summary>
            [HisFieldInfoMapping(2, 2)]
            public string T_GP2IC12 { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
