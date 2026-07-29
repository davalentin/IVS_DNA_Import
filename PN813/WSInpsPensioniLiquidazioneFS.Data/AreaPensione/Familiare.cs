using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA
{
    public class Familiare : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //      01  TRC-FAMIG.
        //          02 TRCTIPOR            PIC X VALUE "C".
        //          02 TRCCONOM            PIC X(32).
        //          02 TRCCOACQ            PIC X(16).
        //          02 TRCSESSO            PIC X.
        //          02 TRCAANAS            PIC 9(4).
        //          02 TRCMMNAS            PIC 99.
        //          02 TRCGGNAS            PIC 99.
        //          02 TRCCONAS            PIC 9(5).
        //          02 TRCPRNAS            PIC 99.
        //          02 TRCCOFIS            PIC X(16).
        //          02 TRCCFSIT            PIC X.
        //          02 TRCDETR1            PIC 9.
        //          02 TRCDETR2            PIC 9.
        //          02 TRCDETR3            PIC 9.
        //          02 TRCDETR4            PIC 9.
        //          02 TRCDETR5            PIC 9.
        //          02 TRCDETR6            PIC 99.
        //          02 TRCDETR7            PIC 99.
        //          02 TRCDETR8            PIC 99.
        //          02 TRCDETR9            PIC 99.
        //          02 TRCDET10            PIC 9.
        //          02 TRCCODFM            PIC X.
        //          02 TRCCONTI OCCURS 8 TIMES.
        //D2NEW        03 TRCDECAA         PIC 9999.                              
        //             03 TRCDECMM         PIC 99.
        //D2NEW        03 TRCSOSAA         PIC 9999.                              
        //             03 TRCSOSMM         PIC 99.
        //             03 TRCDIRAF         PIC XX.
        //             03 TRCQUOTA         PIC XX.
        //             03 TRCCNFON         PIC XX.
        //             03 TRCCNAGO         PIC XX.
        //          02 TRCPRFAM            PIC X.
        //          02 TRCPRREC            PIC 99.    
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  TRC-FAMIG.
        /// <summary>
        /// TRCTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string TRCTIPOR { get; set; }

        /// <summary>
        /// TRCCONOM X(32)  
        /// </summary>
        [HisFieldInfoMapping(1, 32)]
        public string TRCCONOM { get; set; }

        /// <summary>
        /// TRCCOACQ X(16)  
        /// </summary>
        [HisFieldInfoMapping(2, 16)]
        public string TRCCOACQ { get; set; }

        /// <summary>
        /// TRCSESSO X  
        /// </summary>
        [HisFieldInfoMapping(3, 1)]
        public string TRCSESSO { get; set; }

        /// <summary>
        /// TRCAANAS 9(4)  
        /// </summary>
        [HisFieldInfoMapping(4, 4, CobolType = CobolType.Unsigned)]
        public short TRCAANAS { get; set; }

        /// <summary>
        /// TRCMMNAS 99  
        /// </summary>
        [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
        public short TRCMMNAS { get; set; }

        /// <summary>
        /// TRCGGNAS 99  
        /// </summary>
        [HisFieldInfoMapping(6, 2, CobolType = CobolType.Unsigned)]
        public short TRCGGNAS { get; set; }

        /// <summary>
        /// TRCCONAS 9(5)  
        /// </summary>
        [HisFieldInfoMapping(7, 5, CobolType = CobolType.Unsigned)]
        public int TRCCONAS { get; set; }

        /// <summary>
        /// TRCPRNAS 99  
        /// </summary>
        [HisFieldInfoMapping(8, 2, CobolType = CobolType.Unsigned)]
        public short TRCPRNAS { get; set; }

        /// <summary>
        /// TRCCOFIS X(16)  
        /// </summary>
        [HisFieldInfoMapping(9, 16)]
        public string TRCCOFIS { get; set; }

        /// <summary>
        /// TRCCFSIT X  
        /// </summary>
        [HisFieldInfoMapping(10, 1)]
        public string TRCCFSIT { get; set; }

        /// <summary>
        /// TRCDETR1 9  
        /// </summary>
        [HisFieldInfoMapping(11, 1, CobolType = CobolType.Unsigned)]
        public short TRCDETR1 { get; set; }

        /// <summary>
        /// TRCDETR2 9  
        /// </summary>
        [HisFieldInfoMapping(12, 1, CobolType = CobolType.Unsigned)]
        public short TRCDETR2 { get; set; }

        /// <summary>
        /// TRCDETR3 9  
        /// </summary>
        [HisFieldInfoMapping(13, 1, CobolType = CobolType.Unsigned)]
        public short TRCDETR3 { get; set; }

        /// <summary>
        /// TRCDETR4 9  
        /// </summary>
        [HisFieldInfoMapping(14, 1, CobolType = CobolType.Unsigned)]
        public short TRCDETR4 { get; set; }

        /// <summary>
        /// TRCDETR5 9  
        /// </summary>
        [HisFieldInfoMapping(15, 1, CobolType = CobolType.Unsigned)]
        public short TRCDETR5 { get; set; }

        /// <summary>
        /// TRCDETR6 99  
        /// </summary>
        [HisFieldInfoMapping(16, 2, CobolType = CobolType.Unsigned)]
        public short TRCDETR6 { get; set; }

        /// <summary>
        /// TRCDETR7 99  
        /// </summary>
        [HisFieldInfoMapping(17, 2, CobolType = CobolType.Unsigned)]
        public short TRCDETR7 { get; set; }

        /// <summary>
        /// TRCDETR8 99  
        /// </summary>
        [HisFieldInfoMapping(18, 2, CobolType = CobolType.Unsigned)]
        public short TRCDETR8 { get; set; }

        /// <summary>
        /// TRCDETR9 99  
        /// </summary>
        [HisFieldInfoMapping(19, 2, CobolType = CobolType.Unsigned)]
        public short TRCDETR9 { get; set; }

        /// <summary>
        /// TRCDET10 9  
        /// </summary>
        [HisFieldInfoMapping(20, 1, CobolType = CobolType.Unsigned)]
        public short TRCDET10 { get; set; }

        /// <summary>
        /// TRCCODFM X  
        /// </summary>
        [HisFieldInfoMapping(21, 1)]
        public string TRCCODFM { get; set; }

        /// <summary>
        /// TRCCONTI OCCURS 8 TIMES.
        /// </summary>
        [HisComplexAreaInfoMapping(22, ListCount = 8)]
        public List<TRCCONTI> LISTTRCCONTI { get; set; }

        /// <summary>
        /// TRCPRFAM X  
        /// </summary>
        [HisFieldInfoMapping(23, 1)]
        public string TRCPRFAM { get; set; }

        /// <summary>
        /// TRCPRREC 99  
        /// </summary>
        [HisFieldInfoMapping(24, 2, CobolType = CobolType.Unsigned)]
        public short TRCPRREC { get; set; }
        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "Familiare"; }
        }
        #endregion Properties

        #region nested class
        public class TRCCONTI
        {
            #region Properties

            #region Tracciato COBOL
            //          02 TRCCONTI OCCURS 8 TIMES.
            //D2NEW        03 TRCDECAA         PIC 9999.                              
            //             03 TRCDECMM         PIC 99.
            //D2NEW        03 TRCSOSAA         PIC 9999.                              
            //             03 TRCSOSMM         PIC 99.
            //             03 TRCDIRAF         PIC XX.
            //             03 TRCQUOTA         PIC XX.
            //             03 TRCCNFON         PIC XX.
            //             03 TRCCNAGO         PIC XX.
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// TRCDECAA 9999  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short TRCDECAA { get; set; }

            /// <summary>
            /// TRCDECMM 99  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short TRCDECMM { get; set; }

            /// <summary>
            /// TRCSOSAA 9999  
            /// </summary>
            [HisFieldInfoMapping(2, 4, CobolType = CobolType.Unsigned)]
            public short TRCSOSAA { get; set; }

            /// <summary>
            /// TRCSOSMM 99  
            /// </summary>
            [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
            public short TRCSOSMM { get; set; }

            /// <summary>
            /// TRCDIRAF XX  
            /// </summary>
            [HisFieldInfoMapping(4, 2)]
            public string TRCDIRAF { get; set; }

            /// <summary>
            /// TRCQUOTA XX  
            /// </summary>
            [HisFieldInfoMapping(5, 2)]
            public string TRCQUOTA { get; set; }

            /// <summary>
            /// TRCCNFON XX  
            /// </summary>
            [HisFieldInfoMapping(6, 2)]
            public string TRCCNFON { get; set; }

            /// <summary>
            /// TRCCNAGO XX  
            /// </summary>
            [HisFieldInfoMapping(7, 2)]
            public string TRCCNAGO { get; set; }

            #endregion Tracciato Host

            #endregion Properties
        #endregion nested class
        }
    }
}
