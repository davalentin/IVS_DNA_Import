using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class Delegato
    {
        #region Properties

        #region Tracciato COBOL
        //             *DATI DEL PANNELLO MRCDEL0 (DATI DELEGATO)
        //          02 T-GPDEL.
        //             03 T-GP1AP01-V            PIC X.
        //             03 T-GP1AP26-V            PIC X(16).
        //             03 T-GP1DCOGNOME-V        PIC X(36).
        //             03 T-GP1DNOME-V           PIC X(36).
        //             03 T-GP1AP22-V.
        //                04 T-GP1AP22G-V        PIC 9(2).
        //                04 T-GP1AP22M-V        PIC 9(2).
        //                04 T-GP1AP22A-V        PIC 9(4).
        //             03 T-GP1AP27-V            PIC X.
        //             03 T-GP1AP28-V            PIC X(3).
        //BL23A        03 T-GP1AP29-V            PIC 9(8) BINARY.
        //             03 T-GP1DRESIDOM-V        PIC X.
        //             03 T-GP1DIND-V.
        //                04 T-GP1DIND1-V        PIC X(52).
        //                04 T-GP1DIND2-V        PIC X(52).
        //                04 T-GP1DIND3-V        PIC X(52).
        //             03 T-GP1DCIVICO-V         PIC X(18).
        //             03 T-GP1DFRAZIONE-V       PIC X(35).
        //             03 T-GP1DINDIRIZD-V       PIC X(52).
        //             03 T-GP1DCODCOM-V         PIC X(4).
        //             03 T-GP1DCOMUNE-V         PIC X(37).
        //             03 T-GP1DPROV-V           PIC X(3).
        //             03 T-GP1DCAP-V            PIC X(9).
        //             03 T-GP1AP23-V            PIC 9(5).
        //             03 T-GP1AP24-V            PIC X(60).
        //             03 T-GP1AP25-V            PIC X(3).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // *DATI DEL PANNELLO MRCDEL0 (DATI DELEGATO)
        // 02 T-GPDEL.
        /// <summary>
        /// T_GP1AP01_V X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string T_GP1AP01_V { get; set; }

        /// <summary>
        /// T_GP1AP26_V X(16)  
        /// </summary>
        [HisFieldInfoMapping(1, 16)]
        public string T_GP1AP26_V { get; set; }

        /// <summary>
        /// T_GP1DCOGNOME_V X(36)  
        /// </summary>
        [HisFieldInfoMapping(2, 36)]
        public string T_GP1DCOGNOME_V { get; set; }

        /// <summary>
        /// T_GP1DNOME_V X(36)  
        /// </summary>
        [HisFieldInfoMapping(3, 36)]
        public string T_GP1DNOME_V { get; set; }

        // 03 T-GP1AP22-V.
        /// <summary>
        /// T_GP1AP22G_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AP22G_V { get; set; }

        /// <summary>
        /// T_GP1AP22M_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AP22M_V { get; set; }

        /// <summary>
        /// T_GP1AP22A_V 9(4)  
        /// </summary>
        [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1AP22A_V { get; set; }

        /// <summary>
        /// T_GP1AP27_V X  
        /// </summary>
        [HisFieldInfoMapping(7, 1)]
        public string T_GP1AP27_V { get; set; }

        /// <summary>
        /// T_GP1AP28_V X(3)  
        /// </summary>
        [HisFieldInfoMapping(8, 3)]
        public string T_GP1AP28_V { get; set; }

        /// <summary>
        /// T_GP1AP29_V 9(8)  BINARY
        /// </summary>
        [HisFieldInfoMapping(9, 4, CobolType = CobolType.Binary)]
        public int T_GP1AP29_V { get; set; }

        /// <summary>
        /// T_GP1DRESIDOM_V X  
        /// </summary>
        [HisFieldInfoMapping(10, 1)]
        public string T_GP1DRESIDOM_V { get; set; }

        // 03 T-GP1DIND-V.
        /// <summary>
        /// T_GP1DIND1_V X(52)  
        /// </summary>
        [HisFieldInfoMapping(11, 52)]
        public string T_GP1DIND1_V { get; set; }

        /// <summary>
        /// T_GP1DIND2_V X(52)  
        /// </summary>
        [HisFieldInfoMapping(12, 52)]
        public string T_GP1DIND2_V { get; set; }

        /// <summary>
        /// T_GP1DIND3_V X(52)  
        /// </summary>
        [HisFieldInfoMapping(13, 52)]
        public string T_GP1DIND3_V { get; set; }

        /// <summary>
        /// T_GP1DCIVICO_V X(18)  
        /// </summary>
        [HisFieldInfoMapping(14, 18)]
        public string T_GP1DCIVICO_V { get; set; }

        /// <summary>
        /// T_GP1DFRAZIONE_V X(35)  
        /// </summary>
        [HisFieldInfoMapping(15, 35)]
        public string T_GP1DFRAZIONE_V { get; set; }

        /// <summary>
        /// T_GP1DINDIRIZD_V X(52)  
        /// </summary>
        [HisFieldInfoMapping(16, 52)]
        public string T_GP1DINDIRIZD_V { get; set; }

        /// <summary>
        /// T_GP1DCODCOM_V X(4)  
        /// </summary>
        [HisFieldInfoMapping(17, 4)]
        public string T_GP1DCODCOM_V { get; set; }

        /// <summary>
        /// T_GP1DCOMUNE_V X(37)  
        /// </summary>
        [HisFieldInfoMapping(18, 37)]
        public string T_GP1DCOMUNE_V { get; set; }

        /// <summary>
        /// T_GP1DPROV_V X(3)  
        /// </summary>
        [HisFieldInfoMapping(19, 3)]
        public string T_GP1DPROV_V { get; set; }

        /// <summary>
        /// T_GP1DCAP_V X(9)  
        /// </summary>
        [HisFieldInfoMapping(20, 9)]
        public string T_GP1DCAP_V { get; set; }

        /// <summary>
        /// T_GP1AP23_V 9(5)  
        /// </summary>
        [HisFieldInfoMapping(21, 5, CobolType = CobolType.Unsigned)]
        public int T_GP1AP23_V { get; set; }

        /// <summary>
        /// T_GP1AP24_V X(60)  
        /// </summary>
        [HisFieldInfoMapping(22, 60)]
        public string T_GP1AP24_V { get; set; }

        /// <summary>
        /// T_GP1AP25_V X(3)  
        /// </summary>
        [HisFieldInfoMapping(23, 3)]
        public string T_GP1AP25_V { get; set; }
        #endregion Tracciato Host

        #endregion Properties
    }
}
