using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class Ricoveri
    {
        #region Properties

        #region Tracciato COBOL
        //   *DATI PANNELLO RICOVERI
        //02 T-GPRIC0.
        //   03 T-GP2IC20 OCCURS 50.
        //      04 T-GP2IC21.
        //         05 T-GP2IC21G       PIC 9(2).
        //         05 T-GP2IC21M       PIC 9(2).
        //         05 T-GP2IC21A       PIC 9(4).
        //      04 T-GP2IC22.
        //         05 T-GP2IC22G       PIC 9(2).
        //         05 T-GP2IC22M       PIC 9(2).
        //         05 T-GP2IC22A       PIC 9(4).
        //      04 T-GP2IC23           PIC 9(2).
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 50)]
        public List<T_GP2IC20> LISTT_GP2IC20 { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class T_GP2IC20
        {
            #region Properties

            #region Tracciato COBOL
            //   *DATI PANNELLO RICOVERI
            //02 T-GPRIC0.
            //   03 T-GP2IC20 OCCURS 50.
            //      04 T-GP2IC21.
            //         05 T-GP2IC21G       PIC 9(2).
            //         05 T-GP2IC21M       PIC 9(2).
            //         05 T-GP2IC21A       PIC 9(4).
            //      04 T-GP2IC22.
            //         05 T-GP2IC22G       PIC 9(2).
            //         05 T-GP2IC22M       PIC 9(2).
            //         05 T-GP2IC22A       PIC 9(4).
            //      04 T-GP2IC23           PIC 9(2).
            #endregion Tracciato COBOL

            #region Tracciato Host
            // *DATI PANNELLO RICOVERI
            // 02 T-GPRIC0.
            // 03 T-GP2IC20 OCCURS 50.
            // 04 T-GP2IC21.
            /// <summary>
            /// T_GP2IC21G 9(2)  
            /// </summary>
            [HisFieldInfoMapping(0, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2IC21G { get; set; }

            /// <summary>
            /// T_GP2IC21M 9(2)  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2IC21M { get; set; }

            /// <summary>
            /// T_GP2IC21A 9(4)  
            /// </summary>
            [HisFieldInfoMapping(2, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2IC21A { get; set; }

            // 04 T-GP2IC22.
            /// <summary>
            /// T_GP2IC22G 9(2)  
            /// </summary>
            [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2IC22G { get; set; }

            /// <summary>
            /// T_GP2IC22M 9(2)  
            /// </summary>
            [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2IC22M { get; set; }

            /// <summary>
            /// T_GP2IC22A 9(4)  
            /// </summary>
            [HisFieldInfoMapping(5, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2IC22A { get; set; }

            /// <summary>
            /// T_GP2IC23 9(2)  
            /// </summary>
            [HisFieldInfoMapping(6, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2IC23 { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
