using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA
{
    public class TrattamentiFamiglia : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //       01  TRF-REDFA.
        //          02 TRFTIPOR            PIC X VALUE "F".
        //          02 TRFELERD  OCCURS 30 TIMES.
        //D2NEW        03 TRFAAR01         PIC 9999.                              
        //             03 TRFREN01         PIC 9(5)V99.
        //             03 TRFTIP01         PIC X.
        //             03 TRFL7901         PIC XX.
        //             03 TRFREP01         PIC 9(5)V99.
        //          02 TRFELENU  OCCURS 12 TIMES.
        //D2000        03 TRFDEC01.
        //D2NEW           04 TRFDECAA      PIC 9999.                              
        //D2000           04 TRFDECMM      PIC 99.
        //             03 TRFNUM01         PIC 99.
        //             03 TRFSTA01         PIC 9.
        //             03 TRFINA01         PIC XX.
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  TRF-REDFA.
        /// <summary>
        /// TRFTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string TRFTIPOR { get; set; }

        /// <summary>
        /// TRFELERD  OCCURS 30 TIMES.
        /// </summary>
        [HisComplexAreaInfoMapping(1, ListCount = 30)]
        public List<TRFELERD> LISTTRFELERD { get; set; }
        /// <summary>
        /// TRFELENU  OCCURS 12 TIMES.
        /// </summary>
        [HisComplexAreaInfoMapping(2, ListCount = 12)]
        public List<TRFELENU> LISTTRFELENU { get; set; }
        #endregion Tracciato Host

        public string TransactionName
        {
            get { return "TrattamentiFamiglia"; }
        }
        #endregion Properties

        #region nested class
        public class TRFELERD
        {
            #region Properties

            #region Tracciato COBOL
            //          02 TRFELERD  OCCURS 30 TIMES.
            //D2NEW        03 TRFAAR01         PIC 9999.                              
            //             03 TRFREN01         PIC 9(5)V99.
            //             03 TRFTIP01         PIC X.
            //             03 TRFL7901         PIC XX.
            //             03 TRFREP01         PIC 9(5)V99.
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 TRFELERD  OCCURS 30 TIMES.
            /// <summary>
            /// TRFAAR01 9999  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short TRFAAR01 { get; set; }

            /// <summary>
            /// TRFREN01 9(5)V9(2)  
            /// </summary>
            [HisFieldInfoMapping(1, 7, Scale = 2, CobolType = CobolType.Unsigned)]
            public decimal TRFREN01 { get; set; }

            /// <summary>
            /// TRFTIP01 X  
            /// </summary>
            [HisFieldInfoMapping(2, 1)]
            public string TRFTIP01 { get; set; }

            /// <summary>
            /// TRFL7901 XX  
            /// </summary>
            [HisFieldInfoMapping(3, 2)]
            public string TRFL7901 { get; set; }

            /// <summary>
            /// TRFREP01 9(5)V9(2)  
            /// </summary>
            [HisFieldInfoMapping(4, 7, Scale = 2, CobolType = CobolType.Unsigned)]
            public decimal TRFREP01 { get; set; }
            #endregion Tracciato Host
            #endregion Properties
        }
        public class TRFELENU
        {
            #region Properties

            #region Tracciato COBOL
            //          02 TRFELENU  OCCURS 12 TIMES.
            //D2000        03 TRFDEC01.
            //D2NEW           04 TRFDECAA      PIC 9999.                              
            //D2000           04 TRFDECMM      PIC 99.
            //             03 TRFNUM01         PIC 99.
            //             03 TRFSTA01         PIC 9.
            //             03 TRFINA01         PIC XX.
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 TRFELENU  OCCURS 12 TIMES.
            // D2000        03 TRFDEC01.
            /// <summary>
            /// TRFDECAA 9999  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short TRFDECAA { get; set; }

            /// <summary>
            /// TRFDECMM 99  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short TRFDECMM { get; set; }

            /// <summary>
            /// TRFNUM01 99  
            /// </summary>
            [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
            public short TRFNUM01 { get; set; }

            /// <summary>
            /// TRFSTA01 9  
            /// </summary>
            [HisFieldInfoMapping(3, 1)]
            public string TRFSTA01 { get; set; }

            /// <summary>
            /// TRFINA01 XX  
            /// </summary>
            [HisFieldInfoMapping(4, 2)]
            public string TRFINA01 { get; set; }
            #endregion Tracciato Host
            #endregion Properties
        }
        #endregion
    }
}