using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class Tutore
    {
        #region Properties

        #region Tracciato COBOL
        //             *DATI DEL PANNELLO MRCTUT0 (DATI TUTORE)
        //          02 T-GPTUT.
        //             03 T-GP1AP61-V            PIC X.
        //             03 T-GP1AP66-V            PIC X(16).
        //             03 T-GP1TCOGNOME-V        PIC X(36).
        //             03 T-GP1TNOME-V           PIC X(36).
        //             03 T-GP1AP62-V.
        //                04 T-GP1AP62G-V        PIC 9(2).
        //                04 T-GP1AP62M-V        PIC 9(2).
        //                04 T-GP1AP62A-V        PIC 9(4).
        //             03 T-GP1AP67-V            PIC X.
        //             03 T-GP1AP68-V            PIC X(3).
        //BL23A        03 T-GP1AP69-V            PIC 9(8) BINARY.
        //             03 T-GP1TRESIDOM-V        PIC X.
        //             03 T-GP1TIND-V.
        //                04 T-GP1TIND1-V        PIC X(52).
        //                04 T-GP1TIND2-V        PIC X(52).
        //                04 T-GP1TIND3-V        PIC X(52).
        //             03 T-GP1TCIVICO-V         PIC X(18).
        //             03 T-GP1TFRAZIONE-V       PIC X(35).
        //             03 T-GP1TINDIRIZD-V       PIC X(52).
        //             03 T-GP1TCODCOM-V         PIC X(4).
        //             03 T-GP1TCOMUNE-V         PIC X(37).
        //             03 T-GP1TPROV-V           PIC X(3).
        //             03 T-GP1TCAP-V            PIC X(9).
        //             03 T-GP1AP63-V            PIC 9(5).
        //             03 T-GP1AP64-V            PIC X(60).
        //             03 T-GP1AP65-V            PIC X(3).
        //             03 T-GP1AP70.
        //                04 T-GP1AP70A          PIC X(4).
        //                04 T-GP1AP70M          PIC X(2).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // *DATI DEL PANNELLO MRCTUT0 (DATI TUTORE)
        // 02 T-GPTUT.
        /// <summary>
        /// T_GP1AP61_V X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string T_GP1AP61_V { get; set; }

        /// <summary>
        /// T_GP1AP66_V X(16)  
        /// </summary>
        [HisFieldInfoMapping(1, 16)]
        public string T_GP1AP66_V { get; set; }

        /// <summary>
        /// T_GP1TCOGNOME_V X(36)  
        /// </summary>
        [HisFieldInfoMapping(2, 36)]
        public string T_GP1TCOGNOME_V { get; set; }

        /// <summary>
        /// T_GP1TNOME_V X(36)  
        /// </summary>
        [HisFieldInfoMapping(3, 36)]
        public string T_GP1TNOME_V { get; set; }

        // 03 T-GP1AP62-V.
        /// <summary>
        /// T_GP1AP62G_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AP62G_V { get; set; }

        /// <summary>
        /// T_GP1AP62M_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AP62M_V { get; set; }

        /// <summary>
        /// T_GP1AP62A_V 9(4)  
        /// </summary>
        [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1AP62A_V { get; set; }

        /// <summary>
        /// T_GP1AP67_V X  
        /// </summary>
        [HisFieldInfoMapping(7, 1)]
        public string T_GP1AP67_V { get; set; }

        /// <summary>
        /// T_GP1AP68_V X(3)  
        /// </summary>
        [HisFieldInfoMapping(8, 3)]
        public string T_GP1AP68_V { get; set; }

        /// <summary>
        /// T_GP1AP69_V 9(8)  BINARY
        /// </summary>
        [HisFieldInfoMapping(9, 4, CobolType = CobolType.Binary)]
        public int T_GP1AP69_V { get; set; }

        /// <summary>
        /// T_GP1TRESIDOM_V X  
        /// </summary>
        [HisFieldInfoMapping(10, 1)]
        public string T_GP1TRESIDOM_V { get; set; }

        // 03 T-GP1TIND-V.
        /// <summary>
        /// T_GP1TIND1_V X(52)  
        /// </summary>
        [HisFieldInfoMapping(11, 52)]
        public string T_GP1TIND1_V { get; set; }

        /// <summary>
        /// T_GP1TIND2_V X(52)  
        /// </summary>
        [HisFieldInfoMapping(12, 52)]
        public string T_GP1TIND2_V { get; set; }

        /// <summary>
        /// T_GP1TIND3_V X(52)  
        /// </summary>
        [HisFieldInfoMapping(13, 52)]
        public string T_GP1TIND3_V { get; set; }

        /// <summary>
        /// T_GP1TCIVICO_V X(18)  
        /// </summary>
        [HisFieldInfoMapping(14, 18)]
        public string T_GP1TCIVICO_V { get; set; }

        /// <summary>
        /// T_GP1TFRAZIONE_V X(35)  
        /// </summary>
        [HisFieldInfoMapping(15, 35)]
        public string T_GP1TFRAZIONE_V { get; set; }

        /// <summary>
        /// T_GP1TINDIRIZD_V X(52)  
        /// </summary>
        [HisFieldInfoMapping(16, 52)]
        public string T_GP1TINDIRIZD_V { get; set; }

        /// <summary>
        /// T_GP1TCODCOM_V X(4)  
        /// </summary>
        [HisFieldInfoMapping(17, 4)]
        public string T_GP1TCODCOM_V { get; set; }

        /// <summary>
        /// T_GP1TCOMUNE_V X(37)  
        /// </summary>
        [HisFieldInfoMapping(18, 37)]
        public string T_GP1TCOMUNE_V { get; set; }

        /// <summary>
        /// T_GP1TPROV_V X(3)  
        /// </summary>
        [HisFieldInfoMapping(19, 3)]
        public string T_GP1TPROV_V { get; set; }

        /// <summary>
        /// T_GP1TCAP_V X(9)  
        /// </summary>
        [HisFieldInfoMapping(20, 9)]
        public string T_GP1TCAP_V { get; set; }

        /// <summary>
        /// T_GP1AP63_V 9(5)  
        /// </summary>
        [HisFieldInfoMapping(21, 5, CobolType = CobolType.Unsigned)]
        public int T_GP1AP63_V { get; set; }

        /// <summary>
        /// T_GP1AP64_V X(60)  
        /// </summary>
        [HisFieldInfoMapping(22, 60)]
        public string T_GP1AP64_V { get; set; }

        /// <summary>
        /// T_GP1AP65_V X(3)  
        /// </summary>
        [HisFieldInfoMapping(23, 3)]
        public string T_GP1AP65_V { get; set; }

        /// <summary>
        /// T_GP1AP70A X(4)
        /// <summary>
        [HisFieldInfoMapping(24, 4)]
        public string T_GP1AP70A { get; set; }

        /// <summary>
        /// T_GP1AP70M X(2)
        /// <summary>
        [HisFieldInfoMapping(25, 2)]
        public string T_GP1AP70M { get; set; }
        #endregion Tracciato Host

        #endregion Properties
    }
}
