using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class IntegrazioneArticolo11
    {
        #region Properties

        #region Tracciato COBOL
        //   *DATI DEL PANNELLO INTEGRAZIONE ART.11
        //02 T-GPINT0.
        //   03 GPINTAR11 OCCURS 8.
        //      04 T-GP2BC06.
        //         05 T-GP2BC06A       PIC 9(4).
        //         05 T-GP2BC06M       PIC 9(2).
        //      04 T-GP2BC07           PIC S9(5)V9(4) COMP-3.
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 8)]
        public List<GPINTAR11> LISTGPINTAR11 { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class GPINTAR11
        {
            #region Properties

            #region Tracciato COBOL
            //   *DATI DEL PANNELLO INTEGRAZIONE ART.11
            //02 T-GPINT0.
            //   03 GPINTAR11 OCCURS 8.
            //      04 T-GP2BC06.
            //         05 T-GP2BC06A       PIC 9(4).
            //         05 T-GP2BC06M       PIC 9(2).
            //      04 T-GP2BC07           PIC S9(5)V9(4) COMP-3.
            #endregion Tracciato COBOL

            #region Tracciato Host
            // *DATI DEL PANNELLO INTEGRAZIONE ART.11
            // 02 T-GPINT0.
            // 03 GPINTAR11 OCCURS 8.
            // 04 T-GP2BC06.
            /// <summary>
            /// T_GP2BC06A 9(4)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BC06A { get; set; }

            /// <summary>
            /// T_GP2BC06M 9(2)  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BC06M { get; set; }

            /// <summary>
            /// T_GP2BC07 S9(6)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(2, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal T_GP2BC07 { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
