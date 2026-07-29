using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class Errori
    {
        #region Properties

        #region Tracciato COBOL
        //     02 T-TP1ERRC.
        //03 T-TP1TABERR OCCURS 10.
        //   04 T-TP1CODERR         PIC X(3).
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 10)]
        public List<T_TP1TABERR> LISTT_TP1TABERR { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class T_TP1TABERR
        {
            #region Properties

            #region Tracciato COBOL
            //     02 T-TP1ERRC.
            //03 T-TP1TABERR OCCURS 10.
            //   04 T-TP1CODERR         PIC X(3).
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 T-TP1ERRC.
            // 03 T-TP1TABERR OCCURS 10.
            /// <summary>
            /// T_TP1CODERR X(3)  
            /// </summary>
            [HisFieldInfoMapping(0, 3)]
            public string T_TP1CODERR { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
