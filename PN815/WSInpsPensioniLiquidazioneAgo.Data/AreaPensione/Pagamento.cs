using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class Pagamento
    {
        #region Properties

        #region Tracciato COBOL
        //   *DATI DEL PANNELLO MRCPAG0 (INFORMAZIONI SU PAGAMENTO)
        //02 T-GPPAG0.
        //   03 T-TP1DATIPAG.
        //      04 T-GP1AC01-V         PIC X(3).
        //      04 T-GP1CTIPPAG-V      PIC X.
        //      04 T-GP1CNCC-V         PIC X(12).
        //      04 T-GP1CABI-V         PIC 9(5).
        //      04 T-GP1CCAB-V         PIC 9(7).
        //      04 T-GP1ALZ5-V         PIC 9(2).
        //      04 T-TP1PAGDISG.
        //         05 T-GP2BD01-V.
        //            06 T-GP2BD01A-V  PIC 9(4).
        //            06 T-GP2BD01M-V  PIC 9(2).
        //         05 T-GP2BD02-V      PIC S9(7)V9(4) COMP-3.
        //         05 T-GP2BD03-V      PIC S9(5)V9(4) COMP-3.
        //         05 T-GP2BD04-V      PIC S9(7) COMP-3.
        //         05 T-GP2BD05-V      PIC S9(7)V9(4) COMP-3.
        //   03 T-TP1TIPELIPRO.
        //      04 T-GP1DMPN-V.
        //         05 T-GP1DMPNG-V     PIC 9(2).
        //         05 T-GP1DMPNM-V     PIC 9(2).
        //         05 T-GP1DMPNA-V     PIC 9(4).
        //      04 T-GP1CMPNTIP-V      PIC XX.
        //      04 T-GP1AF05-V         PIC 9.
        //      04 T-GP1AM01-V         PIC X.
        //      04 T-GP1AM02-V.
        //         05 T-GP1AM02A-V     PIC 9(4).
        //         05 T-GP1AM02M-V     PIC 9(2).
        //      04 T-GP1AM03-V.
        //         05 T-GP1AM03G-V     PIC 9(2).
        //         05 T-GP1AM03M-V     PIC 9(2).
        //         05 T-GP1AM03A-V     PIC 9(4).
        //      04 T-GP1AM04-V.
        //         05 T-GP1AM04A-V     PIC 9(4).
        //         05 T-GP1AM04M-V     PIC 9(2).
        //      04 T-GP1AM05-V.
        //         05 T-GP1AM05G-V     PIC 9(2).
        //         05 T-GP1AM05M-V     PIC 9(2).
        //         05 T-GP1AM05A-V     PIC 9(4).
        //      04 T-GP1AP2.
        //         05 T-GP1AP2A        PIC 9(4).
        //         05 T-GP1AP2M        PIC 9(2).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // *DATI DEL PANNELLO MRCPAG0 (INFORMAZIONI SU PAGAMENTO)
        // 02 T-GPPAG0.
        // 03 T-TP1DATIPAG.
        /// <summary>
        /// T_GP1AC01_V X(3)  
        /// </summary>
        [HisFieldInfoMapping(0, 3)]
        public string T_GP1AC01_V { get; set; }

        /// <summary>
        /// T_GP1CTIPPAG_V X  
        /// </summary>
        [HisFieldInfoMapping(1, 1)]
        public string T_GP1CTIPPAG_V { get; set; }

        /// <summary>
        /// T_GP1CNCC_V X(12)  
        /// </summary>
        [HisFieldInfoMapping(2, 12)]
        public string T_GP1CNCC_V { get; set; }

        /// <summary>
        /// T_GP1CABI_V 9(5)  
        /// </summary>
        [HisFieldInfoMapping(3, 5, CobolType = CobolType.Unsigned)]
        public int T_GP1CABI_V { get; set; }

        /// <summary>
        /// T_GP1CCAB_V 9(7)  
        /// </summary>
        [HisFieldInfoMapping(4, 7, CobolType = CobolType.Unsigned)]
        public int T_GP1CCAB_V { get; set; }

        /// <summary>
        /// T_GP1ALZ5_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1ALZ5_V { get; set; }

        // 04 T-TP1PAGDISG.
        // 05 T-GP2BD01-V.
        /// <summary>
        /// T_GP2BD01A_V 9(4)  
        /// </summary>
        [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
        public short T_GP2BD01A_V { get; set; }

        /// <summary>
        /// T_GP2BD01M_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short T_GP2BD01M_V { get; set; }

        /// <summary>
        /// T_GP2BD02_V S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(8, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal T_GP2BD02_V { get; set; }

        /// <summary>
        /// T_GP2BD03_V S9(5)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(9, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal T_GP2BD03_V { get; set; }

        /// <summary>
        /// T_GP2BD04_V S9(7) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(10, 4, CobolType = CobolType.Comp3)]
        public int T_GP2BD04_V { get; set; }

        /// <summary>
        /// T_GP2BD05_V S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(11, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal T_GP2BD05_V { get; set; }

        // 03 T-TP1TIPELIPRO.
        // 04 T-GP1DMPN-V.
        /// <summary>
        /// T_GP1DMPNG_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(12, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1DMPNG_V { get; set; }

        /// <summary>
        /// T_GP1DMPNM_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(13, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1DMPNM_V { get; set; }

        /// <summary>
        /// T_GP1DMPNA_V 9(4)  
        /// </summary>
        [HisFieldInfoMapping(14, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1DMPNA_V { get; set; }

        /// <summary>
        /// T_GP1CMPNTIP_V XX  
        /// </summary>
        [HisFieldInfoMapping(15, 2)]
        public string T_GP1CMPNTIP_V { get; set; }

        /// <summary>
        /// T_GP1AF05_V 9  
        /// </summary>
        [HisFieldInfoMapping(16, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1AF05_V { get; set; }

        /// <summary>
        /// T_GP1AM01_V X  
        /// </summary>
        [HisFieldInfoMapping(17, 1)]
        public string T_GP1AM01_V { get; set; }

        // 04 T-GP1AM02-V.
        /// <summary>
        /// T_GP1AM02A_V 9(4)  
        /// </summary>
        [HisFieldInfoMapping(18, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1AM02A_V { get; set; }

        /// <summary>
        /// T_GP1AM02M_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(19, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AM02M_V { get; set; }

        // 04 T-GP1AM03-V.
        /// <summary>
        /// T_GP1AM03G_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(20, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AM03G_V { get; set; }

        /// <summary>
        /// T_GP1AM03M_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(21, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AM03M_V { get; set; }

        /// <summary>
        /// T_GP1AM03A_V 9(4)  
        /// </summary>
        [HisFieldInfoMapping(22, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1AM03A_V { get; set; }

        // 04 T-GP1AM04-V.
        /// <summary>
        /// T_GP1AM04A_V 9(4)  
        /// </summary>
        [HisFieldInfoMapping(23, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1AM04A_V { get; set; }

        /// <summary>
        /// T_GP1AM04M_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(24, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AM04M_V { get; set; }

        // 04 T-GP1AM05-V.
        /// <summary>
        /// T_GP1AM05G_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(25, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AM05G_V { get; set; }

        /// <summary>
        /// T_GP1AM05M_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(26, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AM05M_V { get; set; }

        /// <summary>
        /// T_GP1AM05A_V 9(4)  
        /// </summary>
        [HisFieldInfoMapping(27, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1AM05A_V { get; set; }

        // 04 T-GP1AP2.
        /// <summary>
        /// T_GP1AP2A 9(4)  
        /// </summary>
        [HisFieldInfoMapping(28, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1AP2A { get; set; }

        /// <summary>
        /// T_GP1AP2M 9(2)  
        /// </summary>
        [HisFieldInfoMapping(29, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AP2M { get; set; }
        #endregion Tracciato Host

        #endregion Properties
    }
}
