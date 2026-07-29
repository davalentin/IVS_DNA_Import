using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA
{
    public class MaggiorazioneSociale : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  TRP-REDMS.
        //02 TRPTIPOR            PIC X VALUE "P".
        //02 TRPELERD  OCCURS 27 TIMES.
        //   03 TRPAAR01         PIC 9999.                              
        //   03 TRPMDA01         PIC 99.
        //   03 TRPCON01         PIC 99.
        //   03 TRPREP01         PIC 9(5)V99.
        //   03 TRPREF01         PIC 9(5)V99.
        //   03 TRPFIL01         PIC 99.
        //02 TRPDCMAG            PIC 9(8).
        //02 TRPDATDO            PIC 9(8).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  TRP-REDMS.
        /// <summary>
        /// TRPTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string TRPTIPOR { get; set; }

        /// <summary>
        /// TRPELERD  OCCURS 27 TIMES.
        /// </summary>
        [HisComplexAreaInfoMapping(1, ListCount = 27)]
        public List<TRPELERD> LISTTRPELERD { get; set; }

        /// <summary>
        /// TRPDCMAG 9(8)  
        /// </summary>
        [HisFieldInfoMapping(2, 8, CobolType = CobolType.Unsigned)]
        public int TRPDCMAG { get; set; }

        /// <summary>
        /// TRPDATDO 9(8)  
        /// </summary>
        [HisFieldInfoMapping(3, 8, CobolType = CobolType.Unsigned)]
        public int TRPDATDO { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "MaggiorazioneSociale"; }
        }
        #endregion Properties

        #region nested class
        public class TRPELERD
        {
            #region Properties

            #region Tracciato COBOL
            //02 TRPELERD  OCCURS 27 TIMES.
            //   03 TRPAAR01         PIC 9999.                              
            //   03 TRPMDA01         PIC 99.
            //   03 TRPCON01         PIC 99.
            //   03 TRPREP01         PIC 9(5)V99.
            //   03 TRPREF01         PIC 9(5)V99.
            //   03 TRPFIL01         PIC 99.
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 TRPELERD  OCCURS 27 TIMES.
            /// <summary>
            /// TRPAAR01 9999  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short TRPAAR01 { get; set; }

            /// <summary>
            /// TRPMDA01 99  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short TRPMDA01 { get; set; }

            /// <summary>
            /// TRPCON01 99  
            /// </summary>
            [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
            public short TRPCON01 { get; set; }

            /// <summary>
            /// TRPREP01 9(5)V9(2)  
            /// </summary>
            [HisFieldInfoMapping(3, 7, Scale = 2, CobolType = CobolType.Unsigned)]
            public decimal TRPREP01 { get; set; }

            /// <summary>
            /// TRPREF01 9(5)V9(2)  
            /// </summary>
            [HisFieldInfoMapping(4, 7, Scale = 2, CobolType = CobolType.Unsigned)]
            public decimal TRPREF01 { get; set; }

            /// <summary>
            /// TRPFIL01 99  
            /// </summary>
            [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
            public short TRPFIL01 { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class
    }
}
