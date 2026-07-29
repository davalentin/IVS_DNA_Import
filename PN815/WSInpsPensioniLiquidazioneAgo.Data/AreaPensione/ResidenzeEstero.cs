using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class ResidenzeEstero
    {
        #region Properties

        #region Tracciato COBOL
        //   *DATI DEL PANNELLO MRCRES0 (RESIDENZE ESTERO)
        //02 T-GPRES0.
        //   03 T-GP2BS00 OCCURS 20.
        //      04 T-GP2BS01.
        //         05 T-GP2BS01A       PIC 9(4).
        //         05 T-GP2BS01M       PIC 9(2).
        //      04 T-GP2BS02           PIC X(3).
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 20)]
        public List<T_GP2BS00> LISTT_GP2BS00 { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class T_GP2BS00
        {
            #region Properties

            #region Tracciato COBOL
            //   *DATI DEL PANNELLO MRCRES0 (RESIDENZE ESTERO)
            //02 T-GPRES0.
            //   03 T-GP2BS00 OCCURS 20.
            //      04 T-GP2BS01.
            //         05 T-GP2BS01A       PIC 9(4).
            //         05 T-GP2BS01M       PIC 9(2).
            //      04 T-GP2BS02           PIC X(3).
            #endregion Tracciato COBOL

            #region Tracciato Host
            // *DATI DEL PANNELLO MRCRES0 (RESIDENZE ESTERO)
            // 02 T-GPRES0.
            // 03 T-GP2BS00 OCCURS 20.
            // 04 T-GP2BS01.
            /// <summary>
            /// T_GP2BS01A 9(4)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BS01A { get; set; }

            /// <summary>
            /// T_GP2BS01M 9(2)  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BS01M { get; set; }

            /// <summary>
            /// T_GP2BS02 X(3)  
            /// </summary>
            [HisFieldInfoMapping(2, 3)]
            public string T_GP2BS02 { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
