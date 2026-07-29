using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class Intestazione
    {
        #region Properties

        #region Tracciato COBOL
        //        02 FILLER                    PIC XXX.
        //02 TESTATA.
        //   03 FILLER                 PIC X.
        //   03 PNINSIEME              PIC X(3).
        //   03 T-WEBDOAS4             PIC X(2)
        //   03 FILLER                 PIC X(2).
        //02 T-TP1NL.
        //   03 T-RAFLGPRW             PIC X.
        //   03 T-CONTRIBUTIVA         PIC X(2).
        //   03 T-AFFINE               PIC 9.
        //   03 T-CONIUGE              PIC 9.
        //   03 T-FIGLI                PIC 9(2).
        //   03 T-GP1AM07              PIC X(3).
        //   03 T-GP1AM08              PIC 9(4).
        //   03 T-GP1AM09              PIC 9(8).
        //   03 T-PROCESSO             PIC 99.
        //   03 T-GP1AF09Z             PIC 9(6).
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// FILLER XXX  
        /// </summary>
        [HisFieldInfoMapping(0, 3)]
        public string FILLER1 { get; set; }

        // 02 TESTATA.
        /// <summary>
        /// FILLER X  
        /// </summary>
        [HisFieldInfoMapping(1, 1)]
        public string FILLER2 { get; set; }

        /// <summary>
        /// PNINSIEME X(3)  
        /// </summary>
        [HisFieldInfoMapping(2, 3)]
        public string PNINSIEME { get; set; }

        /// <summary>
        /// T-WEBDOAS4 X(2)  
        /// </summary>
        [HisFieldInfoMapping(3, 2)]
        public string T_WEBDOAS4 { get; set; }

        /// <summary>
        /// FILLER X(2)  
        /// </summary>
        [HisFieldInfoMapping(4, 2)]
        public string FILLER3 { get; set; }

        // 02 T-TP1NL.
        /// <summary>
        /// T_RAFLGPRW X  
        /// </summary>
        [HisFieldInfoMapping(5, 1)]
        public string T_RAFLGPRW { get; set; }

        /// <summary>
        /// T_CONTRIBUTIVA X(2)  
        /// </summary>
        [HisFieldInfoMapping(6, 2)]
        public string T_CONTRIBUTIVA { get; set; }

        /// <summary>
        /// T_AFFINE 9  
        /// </summary>
        [HisFieldInfoMapping(7, 1, CobolType = CobolType.Unsigned)]
        public short T_AFFINE { get; set; }

        /// <summary>
        /// T_CONIUGE 9  
        /// </summary>
        [HisFieldInfoMapping(8, 1, CobolType = CobolType.Unsigned)]
        public short T_CONIUGE { get; set; }

        /// <summary>
        /// T_FIGLI 9(2)  
        /// </summary>
        [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
        public short T_FIGLI { get; set; }

        /// <summary>
        /// T_GP1AM07 X(3)  
        /// </summary>
        [HisFieldInfoMapping(10, 3)]
        public string T_GP1AM07 { get; set; }

        /// <summary>
        /// T_GP1AM08 9(4)  
        /// </summary>
        [HisFieldInfoMapping(11, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1AM08 { get; set; }

        /// <summary>
        /// T_GP1AM09 9(8)  
        /// </summary>
        [HisFieldInfoMapping(12, 8, CobolType = CobolType.Unsigned)]
        public int T_GP1AM09 { get; set; }

        /// <summary>
        /// T_PROCESSO 99  
        /// </summary>
        [HisFieldInfoMapping(13, 2, CobolType = CobolType.Unsigned)]
        public short T_PROCESSO { get; set; }

        /// <summary>
        /// T_GP1AF09Z 9(6)  
        /// </summary>
        [HisFieldInfoMapping(14, 6, CobolType = CobolType.Unsigned)]
        public int T_GP1AF09Z { get; set; }
        #endregion Tracciato Host

        #endregion Properties
    }
}

