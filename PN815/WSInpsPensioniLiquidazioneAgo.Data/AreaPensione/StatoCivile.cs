using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class StatoCivile
    {
        #region Properties

        #region Tracciato COBOL
        //   *DATI DEL PANNELLO MRCAN25 (STATO CIVILE)
        //02 T-GPAN25.
        //   03 T-GP2KM7A  OCCURS 10.
        //      04 T-GP2KM72.
        //         05 T-GP2KM72A       PIC 9(4).
        //         05 T-GP2KM72M       PIC 9(2).
        //      04 T-GP2KM76           PIC X.
        //   03 FILLER                 PIC X(10).
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 10)]
        public List<T_GP2KM7A> LISTT_GP2KM7A { get; set; }
        [HisFieldInfoMapping(1, 10)]
        public string FILLER { get; set; }
        #endregion Tracciato Host
        #region nested class
        public class T_GP2KM7A
        {
            #region Properties

            #region Tracciato COBOL
            //   *DATI DEL PANNELLO MRCAN25 (STATO CIVILE)
            //02 T-GPAN25.
            //   03 T-GP2KM7A  OCCURS 10.
            //      04 T-GP2KM72.
            //         05 T-GP2KM72A       PIC 9(4).
            //         05 T-GP2KM72M       PIC 9(2).
            //      04 T-GP2KM76           PIC X.
            #endregion Tracciato COBOL

            #region Tracciato Host
            // *DATI DEL PANNELLO MRCAN25 (STATO CIVILE)
            // 02 T-GPAN25.
            // 03 T-GP2KM7A  OCCURS 10.
            // 04 T-GP2KM72.
            /// <summary>
            /// T_GP2KM72A 9(4)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2KM72A { get; set; }

            /// <summary>
            /// T_GP2KM72M 9(2)  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2KM72M { get; set; }

            /// <summary>
            /// T_GP2KM76 X  
            /// </summary>
            [HisFieldInfoMapping(2, 1)]
            public string T_GP2KM76 { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}