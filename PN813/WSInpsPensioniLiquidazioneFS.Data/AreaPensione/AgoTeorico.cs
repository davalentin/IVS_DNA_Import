using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA
{
    public class AgoTeorico : ITransactionInfo
    {
        #region Constructor
        //internal AgoTeorico()
        //{

        //}
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     01  TRN-TEORICO.
        //          02 TRNTIPOR            PIC X VALUE "N".
        //D2NEW     02 TRNDECAA            PIC 9(4).                              
        //          02 TRNDECMM            PIC 9(2).
        //D2NEW     02 TRNSOSAA            PIC 9(4).                              
        //          02 TRNSOSMM            PIC 9(2).
        //          02 TRNTPENS            PIC 9.
        //          02 TRNTIPLQ            PIC 9.
        //          02 TRNRSETA            PIC 9(4)V9999.
        //          02 TRNRSETB            PIC 9(4)V9999.
        //          02 TRNSTOTA            PIC 9(5).
        //          02 TRNSTOTB            PIC 9(5).
        //          02 TRNSESCA            PIC 9(5).
        //          02 TRNSESCB            PIC 9(5).
        //          02 TRNTEORA            PIC 9(4)V9999.
        //          02 TRNTEORB            PIC 9(4)V9999.
        //          02 TRNDISPO            PIC X(141).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  TRN-TEORICO.
        /// <summary>
        /// TRNTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string TRNTIPOR { get; set; }

        /// <summary>
        /// TRNDECAA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(1, 4, CobolType = CobolType.Unsigned)]
        public short TRNDECAA { get; set; }

        /// <summary>
        /// TRNDECMM 9(2)  
        /// </summary>
        [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
        public short TRNDECMM { get; set; }

        /// <summary>
        /// TRNSOSAA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(3, 4, CobolType = CobolType.Unsigned)]
        public short TRNSOSAA { get; set; }

        /// <summary>
        /// TRNSOSMM 9(2)  
        /// </summary>
        [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
        public short TRNSOSMM { get; set; }

        /// <summary>
        /// TRNTPENS 9  
        /// </summary>
        [HisFieldInfoMapping(5, 1, CobolType = CobolType.Unsigned)]
        public short TRNTPENS { get; set; }

        /// <summary>
        /// TRNTIPLQ 9  
        /// </summary>
        [HisFieldInfoMapping(6, 1, CobolType = CobolType.Unsigned)]
        public short TRNTIPLQ { get; set; }

        /// <summary>
        /// TRNRSETA 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(7, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal TRNRSETA { get; set; }

        /// <summary>
        /// TRNRSETB 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(8, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal TRNRSETB { get; set; }

        /// <summary>
        /// TRNSTOTA 9(5)  
        /// </summary>
        [HisFieldInfoMapping(9, 5, CobolType = CobolType.Unsigned)]
        public int TRNSTOTA { get; set; }

        /// <summary>
        /// TRNSTOTB 9(5)  
        /// </summary>
        [HisFieldInfoMapping(10, 5, CobolType = CobolType.Unsigned)]
        public int TRNSTOTB { get; set; }

        /// <summary>
        /// TRNSESCA 9(5)  
        /// </summary>
        [HisFieldInfoMapping(11, 5, CobolType = CobolType.Unsigned)]
        public int TRNSESCA { get; set; }

        /// <summary>
        /// TRNSESCB 9(5)  
        /// </summary>
        [HisFieldInfoMapping(12, 5, CobolType = CobolType.Unsigned)]
        public int TRNSESCB { get; set; }

        /// <summary>
        /// TRNTEORA 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(13, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal TRNTEORA { get; set; }

        /// <summary>
        /// TRNTEORB 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(14, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal TRNTEORB { get; set; }

        /// <summary>
        /// TRNDISPO X(141)  
        /// </summary>
        [HisFieldInfoMapping(15, 141)]
        public string TRNDISPO { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "AgoTeorico"; }
        }
        #endregion Properties
    }
}
