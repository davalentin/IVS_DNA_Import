using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class Pensionato
    {
        #region Properties

        #region Tracciato COBOL
        //*DATI DEL PANNELLO MRCAN20
        //                  02 T-GPAN20.
        //             03 T-GP3CB02T-V           PIC X(32).
        //             03 T-GP3CB03T-V           PIC X(32).
        //             03 T-GP3CB04T-V           PIC X(31).
        //             03 T-GP3CB05T-V           PIC X.
        //             03 T-GP3CB06T.
        //                04 T-GP3CB06TG-V       PIC 9(2).
        //                04 T-GP3CB06TM-V       PIC 9(2).
        //                04 T-GP3CB06TA-V       PIC 9(4).
        //             03 T-GP3CB17T-V           PIC X(36).
        //             03 T-GP3CB27T-V           PIC X(3).
        //             03 T-GP3CB07T-V           PIC 9(5).
        //             03 T-GP3CB08T-V           PIC X(16).
        //             03 T-GP3CB10T-V           PIC X(3).
        //BL23A        03 T-GP3CB11T-V           PIC 9(8) BINARY.
        //             03 T-GP1RRESIDOM-V        PIC X.
        //             03 T-GP1RIND-V.
        //                04 T-GP1RIND1-V        PIC X(52).
        //                04 T-GP1RIND2-V        PIC X(52).
        //                04 T-GP1RIND3-V        PIC X(52).
        //             03 T-GP1RCIVICO-V         PIC X(18).
        //             03 T-GP1RFRAZIONE-V       PIC X(35).
        //             03 T-GP1RINDIRZD-V        PIC X(52).
        //             03 T-GP1RCODCOM-V         PIC X(4).
        //             03 T-GP1RCAP-V            PIC X(9).
        //             03 T-GP1RCOMUNE-V         PIC X(37).
        //             03 T-GP1RPROV-V           PIC X(3).
        //             03 T-GP1AZ03              PIC 9.
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 02 T-GPAN20.
        /// <summary>
        /// T_GP3CB02T_V X(32)  
        /// </summary>
        [HisFieldInfoMapping(0, 32)]
        public string T_GP3CB02T_V { get; set; }

        /// <summary>
        /// T_GP3CB03T_V X(32)  
        /// </summary>
        [HisFieldInfoMapping(1, 32)]
        public string T_GP3CB03T_V { get; set; }

        /// <summary>
        /// T_GP3CB04T_V X(31)  
        /// </summary>
        [HisFieldInfoMapping(2, 31)]
        public string T_GP3CB04T_V { get; set; }

        /// <summary>
        /// T_GP3CB05T_V X  
        /// </summary>
        [HisFieldInfoMapping(3, 1)]
        public string T_GP3CB05T_V { get; set; }

        // 03 T-GP3CB06T.
        /// <summary>
        /// T_GP3CB06TG_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
        public short T_GP3CB06TG_V { get; set; }

        /// <summary>
        /// T_GP3CB06TM_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
        public short T_GP3CB06TM_V { get; set; }

        /// <summary>
        /// T_GP3CB06TA_V 9(4)  
        /// </summary>
        [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
        public short T_GP3CB06TA_V { get; set; }

        /// <summary>
        /// T_GP3CB17T_V X(36)  
        /// </summary>
        [HisFieldInfoMapping(7, 36)]
        public string T_GP3CB17T_V { get; set; }

        /// <summary>
        /// T_GP3CB27T_V X(3)  
        /// </summary>
        [HisFieldInfoMapping(8, 3)]
        public string T_GP3CB27T_V { get; set; }

        /// <summary>
        /// T_GP3CB07T_V 9(5)  
        /// </summary>
        [HisFieldInfoMapping(9, 5, CobolType = CobolType.Unsigned)]
        public int T_GP3CB07T_V { get; set; }

        /// <summary>
        /// T_GP3CB08T_V X(16)  
        /// </summary>
        [HisFieldInfoMapping(10, 16)]
        public string T_GP3CB08T_V { get; set; }

        /// <summary>
        /// T_GP3CB10T_V X(3)  
        /// </summary>
        [HisFieldInfoMapping(11, 3)]
        public string T_GP3CB10T_V { get; set; }

        /// <summary>
        /// T_GP3CB11T_V 9(8)  BINARY
        /// </summary>
        [HisFieldInfoMapping(12, 4, CobolType = CobolType.Binary)]
        public int T_GP3CB11T_V { get; set; }

        /// <summary>
        /// T_GP1RRESIDOM_V X  
        /// </summary>
        [HisFieldInfoMapping(13, 1)]
        public string T_GP1RRESIDOM_V { get; set; }

        // 03 T-GP1RIND-V.
        /// <summary>
        /// T_GP1RIND1_V X(52)  
        /// </summary>
        [HisFieldInfoMapping(14, 52)]
        public string T_GP1RIND1_V { get; set; }

        /// <summary>
        /// T_GP1RIND2_V X(52)  
        /// </summary>
        [HisFieldInfoMapping(15, 52)]
        public string T_GP1RIND2_V { get; set; }

        /// <summary>
        /// T_GP1RIND3_V X(52)  
        /// </summary>
        [HisFieldInfoMapping(16, 52)]
        public string T_GP1RIND3_V { get; set; }

        /// <summary>
        /// T_GP1RCIVICO_V X(18)  
        /// </summary>
        [HisFieldInfoMapping(17, 18)]
        public string T_GP1RCIVICO_V { get; set; }

        /// <summary>
        /// T_GP1RFRAZIONE_V X(35)  
        /// </summary>
        [HisFieldInfoMapping(18, 35)]
        public string T_GP1RFRAZIONE_V { get; set; }

        /// <summary>
        /// T_GP1RINDIRZD_V X(52)  
        /// </summary>
        [HisFieldInfoMapping(19, 52)]
        public string T_GP1RINDIRZD_V { get; set; }

        /// <summary>
        /// T_GP1RCODCOM_V X(4)  
        /// </summary>
        [HisFieldInfoMapping(20, 4)]
        public string T_GP1RCODCOM_V { get; set; }

        /// <summary>
        /// T_GP1RCAP_V X(9)  
        /// </summary>
        [HisFieldInfoMapping(21, 9)]
        public string T_GP1RCAP_V { get; set; }

        /// <summary>
        /// T_GP1RCOMUNE_V X(37)  
        /// </summary>
        [HisFieldInfoMapping(22, 37)]
        public string T_GP1RCOMUNE_V { get; set; }

        /// <summary>
        /// T_GP1RPROV_V X(3)  
        /// </summary>
        [HisFieldInfoMapping(23, 3)]
        public string T_GP1RPROV_V { get; set; }

        /// <summary>
        /// T_GP1AZ03 9  
        /// </summary>
        [HisFieldInfoMapping(24, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1AZ03 { get; set; }
        #endregion Tracciato Host

        #endregion Properties
    }
}
