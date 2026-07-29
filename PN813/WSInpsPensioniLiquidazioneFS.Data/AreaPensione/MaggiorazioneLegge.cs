using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA
{
    public class MaggiorazioneLegge : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //  01   TRI-ML407.
        //     02 TRITIPOR                         PIC X VALUE "I".
        //     02 TRIUTIAA                         PIC 9.
        //     02 TRIRETQA                         PIC 9(6)V9999.
        //     02 TRIUTIAB                         PIC 9.
        //     02 TRIRETQB                         PIC 9(6)V9999.
        //     02 TRIINCRA.
        //        03 TRIINCAA                      PIC 9.
        //        03 TRIINCAM                      PIC 99.
        //     02 TRIINCRB.
        //        03 TRIINCBA                      PIC 9.
        //        03 TRIINCBM                      PIC 99.
        //*- PER GAS ED ES SERVIZIO UTILE IN SETTIMANE (MAX 260)
        //     02 TRISEUTA                         PIC 999.
        //     02 TRISEUTB                         PIC 999.
        //     02 TRISEUTC                         PIC 9.
        //     02 TRIRA336                         PIC 9(6)V9999 COMP-3.
        //     02 TRIRB336                         PIC 9(6)V9999 COMP-3.
        //     02 FILLER                           PIC X(6).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01   TRI-ML407.
        /// <summary>
        /// TRITIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string TRITIPOR { get; set; }

        /// <summary>
        /// TRIUTIAA 9  
        /// </summary>
        [HisFieldInfoMapping(1, 1, CobolType = CobolType.Unsigned)]
        public short TRIUTIAA { get; set; }

        /// <summary>
        /// TRIRETQA 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(2, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal TRIRETQA { get; set; }

        /// <summary>
        /// TRIUTIAB 9  
        /// </summary>
        [HisFieldInfoMapping(3, 1, CobolType = CobolType.Unsigned)]
        public short TRIUTIAB { get; set; }

        /// <summary>
        /// TRIRETQB 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(4, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal TRIRETQB { get; set; }

        // 02 TRIINCRA.
        /// <summary>
        /// TRIINCAA 9  
        /// </summary>
        [HisFieldInfoMapping(5, 1, CobolType = CobolType.Unsigned)]
        public short TRIINCAA { get; set; }

        /// <summary>
        /// TRIINCAM 99  
        /// </summary>
        [HisFieldInfoMapping(6, 2, CobolType = CobolType.Unsigned)]
        public short TRIINCAM { get; set; }

        // 02 TRIINCRB.
        /// <summary>
        /// TRIINCBA 9  
        /// </summary>
        [HisFieldInfoMapping(7, 1, CobolType = CobolType.Unsigned)]
        public short TRIINCBA { get; set; }

        /// <summary>
        /// TRIINCBM 99  
        /// </summary>
        [HisFieldInfoMapping(8, 2, CobolType = CobolType.Unsigned)]
        public short TRIINCBM { get; set; }

        // *- PER GAS ED ES SERVIZIO UTILE IN SETTIMANE (MAX 260)
        /// <summary>
        /// TRISEUTA 999  
        /// </summary>
        [HisFieldInfoMapping(9, 3, CobolType = CobolType.Unsigned)]
        public short TRISEUTA { get; set; }

        /// <summary>
        /// TRISEUTB 999  
        /// </summary>
        [HisFieldInfoMapping(10, 3, CobolType = CobolType.Unsigned)]
        public short TRISEUTB { get; set; }

        /// <summary>
        /// TRISEUTC 9  
        /// </summary>
        [HisFieldInfoMapping(11, 1, CobolType = CobolType.Unsigned)]
        public short TRISEUTC { get; set; }

        /// <summary>
        /// TRIRA336 9(6)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(12, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal TRIRA336 { get; set; }

        /// <summary>
        /// TRIRB336 9(6)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(13, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal TRIRB336 { get; set; }

        /// <summary>
        /// FILLER X(6)  
        /// </summary>
        [HisFieldInfoMapping(14, 6)]
        public string FILLER { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "MaggiorazioneLegge"; }
        }
        #endregion Properties
    }
}
