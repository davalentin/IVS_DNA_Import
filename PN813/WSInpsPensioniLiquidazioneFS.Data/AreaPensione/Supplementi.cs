using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA
{
    public class Supplementi : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //       01  TRE-SUPPL.
        //          02 TRETIPOR            PIC X VALUE "E".
        //          02 TRE-SUP14 OCCURS 14 TIMES.
        //             03 TRENAT01         PIC X.
        //             03 TRETIP01         PIC X.
        //D2000        03 TREDEC01.
        //D2NEW           04 TREDECAA         PIC 9999.                           
        //                04 TREDECMM         PIC 99.
        //             03 TRETOT01            PIC 9(6).
        //             03 TREESC01            PIC 9(6).
        //             03 TRERMS01            PIC 9(6)V99.
        //             03 TREDPC01            PIC 9(4)V9999.
        //             03 TRES7201            PIC 9(4)V9999.
        //             03 TREFLG01            PIC X.
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  TRE-SUPPL.
        /// <summary>
        /// TRETIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string TRETIPOR { get; set; }

        /// <summary>
        /// TRE-SUP14 OCCURS 14 TIMES.
        /// </summary>
        [HisComplexAreaInfoMapping(1, ListCount = 14)]
        public List<TRE_SUP14> LISTTRE_SUP14 { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "Supplementi"; }
        }
        #endregion Properties

        #region nested class
        public class TRE_SUP14
        {
            #region Properties

            #region Tracciato COBOL
            //          02 TRE-SUP14 OCCURS 14 TIMES.
            //             03 TRENAT01         PIC X.
            //             03 TRETIP01         PIC X.
            //D2000        03 TREDEC01.
            //D2NEW           04 TREDECAA         PIC 9999.                           
            //                04 TREDECMM         PIC 99.
            //             03 TRETOT01            PIC 9(6).
            //             03 TREESC01            PIC 9(6).
            //             03 TRERMS01            PIC 9(6)V99.
            //             03 TREDPC01            PIC 9(4)V9999.
            //             03 TRES7201            PIC 9(4)V9999.
            //             03 TREFLG01            PIC X.
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 TRE-SUP14 OCCURS 14 TIMES.
            /// <summary>
            /// TRENAT01 X  
            /// </summary>
            [HisFieldInfoMapping(0, 1)]
            public string TRENAT01 { get; set; }

            /// <summary>
            /// TRETIP01 X  
            /// </summary>
            [HisFieldInfoMapping(1, 1)]
            public string TRETIP01 { get; set; }

            // D2000        03 TREDEC01.
            /// <summary>
            /// TREDECAA 9999  
            /// </summary>
            [HisFieldInfoMapping(2, 4, CobolType = CobolType.Unsigned)]
            public short TREDECAA { get; set; }

            /// <summary>
            /// TREDECMM 99  
            /// </summary>
            [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
            public short TREDECMM { get; set; }

            /// <summary>
            /// TRETOT01 9(6)  
            /// </summary>
            [HisFieldInfoMapping(4, 6, CobolType = CobolType.Unsigned)]
            public int TRETOT01 { get; set; }

            /// <summary>
            /// TREESC01 9(6)  
            /// </summary>
            [HisFieldInfoMapping(5, 6, CobolType = CobolType.Unsigned)]
            public int TREESC01 { get; set; }

            /// <summary>
            /// TRERMS01 9(6)V9(2)  
            /// </summary>
            [HisFieldInfoMapping(6, 8, Scale = 2, CobolType = CobolType.Unsigned)]
            public decimal TRERMS01 { get; set; }

            /// <summary>
            /// TREDPC01 9(4)V9(4)  
            /// </summary>
            [HisFieldInfoMapping(7, 8, Scale = 4, CobolType = CobolType.Unsigned)]
            public decimal TREDPC01 { get; set; }

            /// <summary>
            /// TRES7201 9(4)V9(4)  
            /// </summary>
            [HisFieldInfoMapping(8, 8, Scale = 4, CobolType = CobolType.Unsigned)]
            public decimal TRES7201 { get; set; }

            /// <summary>
            /// TREFLG01 X  
            /// </summary>
            [HisFieldInfoMapping(9, 1)]
            public string TREFLG01 { get; set; }

            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class
    }
}
