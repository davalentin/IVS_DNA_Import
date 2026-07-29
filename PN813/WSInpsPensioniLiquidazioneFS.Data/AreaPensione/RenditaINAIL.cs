using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA
{
    public class RenditaINAIL : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //        01  TRL-INAIL.
        //          02 TRLTIPOR            PIC X VALUE "L".
        //          02 TRGELERD  OCCURS 24 TIMES.
        //D2NEW        03 TRLDEC01         PIC 9(6).                              
        //             03 TRLIMP01         PIC 9(6)V9999.
        //             03 TRLEVE01         PIC X.
        //          02 TRLDIRIT            PIC X.
        //D2NEW     02 TRLDECAC            PIC 9(6).                               
        //GD        02 FILLER              PIC X(34).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  TRL-INAIL.
        /// <summary>
        /// TRLTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string TRLTIPOR { get; set; }

        /// <summary>
        /// TRGELERD  OCCURS 24 TIMES.
        /// </summary>
        [HisComplexAreaInfoMapping(1, ListCount = 24)]
        public List<TRGELERD> LISTTRGELERD { get; set; }

        /// <summary>
        /// TRLDIRIT X  
        /// </summary>
        [HisFieldInfoMapping(2, 1)]
        public string TRLDIRIT { get; set; }

        /// <summary>
        /// TRLDECAC 9(6)  
        /// </summary>
        [HisFieldInfoMapping(3, 6, CobolType = CobolType.Unsigned)]
        public int TRLDECAC { get; set; }

        /// <summary>
        /// FILLER X(34)  
        /// </summary>
        [HisFieldInfoMapping(4, 34)]
        public string FILLER { get; set; }
        #endregion Tracciato Host

        public string TransactionName
        {
            get { return "RenditaINAIL"; }
        }
        #endregion Properties

        #region nested class
        public class TRGELERD
        {
            #region Properties

            #region Tracciato COBOL
            //          02 TRGELERD  OCCURS 24 TIMES.
            //D2NEW        03 TRLDEC01         PIC 9(6).                              
            //             03 TRLIMP01         PIC 9(6)V9999.
            //             03 TRLEVE01         PIC X.
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 TRGELERD  OCCURS 24 TIMES.
            /// <summary>
            /// TRLDEC01 9(6)  
            /// </summary>
            [HisFieldInfoMapping(0, 6, CobolType = CobolType.Unsigned)]
            public int TRLDEC01 { get; set; }

            /// <summary>
            /// TRLIMP01 9(6)V9(4)  
            /// </summary>
            [HisFieldInfoMapping(1, 10, Scale = 4, CobolType = CobolType.Unsigned)]
            public decimal TRLIMP01 { get; set; }

            /// <summary>
            /// TRLEVE01 X  
            /// </summary>
            [HisFieldInfoMapping(2, 1)]
            public string TRLEVE01 { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class
    }
}
