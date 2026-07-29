using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA
{
    public class MiglioramentiContrattuali : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //03  W-REC-S.
        //05  W-TIPO-REC-S                     PIC X.
        //05  W-AUMENTI-CONTR.
        //    07  WS-GP2BB04DT.
        //        09  WS-GP2BB04RZ.
        //            11  WS-GP2BB04SA         PIC 9(4).
        //            11  WS-GP2BB04SAR REDEFINES WS-GP2BB04SA.
        //                13 WS-GP2BB04S        PIC 99.
        //                13 WS-GP2BB04A        PIC 99.
        //            11  WS-GP2BB04M          PIC 99.
        //        09  WS-GP2BB04Z REDEFINES WS-GP2BB04RZ PIC 9(6).
        //        09  WS-GP2BB04GG             PIC 99.
        //    07  WS-GP2BB05N                  PIC X(2).
        //    07  WS-GP2BB06E                  PIC S9(8)V9(7) COMP-3.
        //05  FILLER                           PIC X(<complemento a 2000 bytes>).

        #endregion Tracciato COBOL


        #region Tracciato Host

        /// <summary>
        /// W-TIPO-REC-S (PIC X)
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string TRSTPREC { get; set; }

        /// <summary>
        /// W-AUMENTI-CONTR
        /// </summary>
        [HisComplexAreaInfoMapping(1, ListCount = 3)]
        public List<W_AUMENTI_CONTR> LISTWAUMENTICONTR { get; set; }
        /// <summary>
        /// FILLER (complemento a 2000 bytes, 1966)
        /// </summary>
        [HisFieldInfoMapping(2, 1945)]
        public string FILLER { get; set; }

        #endregion Tracciato Host


        public string TransactionName
        {
            get { return "MiglioramentiContrattuali"; }
        }

        #endregion Properties

        #region nested class
        public class W_AUMENTI_CONTR
        {
            #region Properties

            #region Tracciato COBOL
            //05  W-AUMENTI-CONTR.
            //    07  WS-GP2BB04DT.
            //        09  WS-GP2BB04RZ.
            //            11  WS-GP2BB04SA         PIC 9(4).
            //            11  WS-GP2BB04SAR REDEFINES WS-GP2BB04SA.
            //                13 WS-GP2BB04S        PIC 99.
            //                13 WS-GP2BB04A        PIC 99.
            //            11  WS-GP2BB04M          PIC 99.
            //        09  WS-GP2BB04Z REDEFINES WS-GP2BB04RZ PIC 9(6).
            //        09  WS-GP2BB04GG             PIC 99.
            //    07  WS-GP2BB05N                  PIC X(2).
            //    07  WS-GP2BB06E                  PIC S9(8)V9(7) COMP-3.
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// TRCDECAA 9999  
            /// </summary>
            [HisFieldInfoMapping(0, 8, CobolType = CobolType.Unsigned)]
            public int WS_GP2BB04DT { get; set; }

            /// <summary>
            /// WS-GP2BB05N (PIC X(2))
            /// </summary>
            [HisFieldInfoMapping(1, 2)]
            public string WS_GP2BB05N { get; set; }

            ///// <summary>
            ///// WS-GP2BB06E (PIC S9(8)V9(7) COMP-3)
            ///// </summary>
            //[HisFieldInfoMapping(2, 8)]
            //public decimal WS_GP2BB06E { get; set; }

            /// <summary>
            /// WS-GP2BB06E PIC S9(8)V9(7) COMP-3
            /// </summary>
            [HisFieldInfoMapping(2, 8, Scale = 7, CobolType = CobolType.Comp3Unsigned)]
            public decimal WS_GP2BB06E { get; set; }

            #endregion Tracciato Host

            #endregion Properties
            #endregion nested class
        }
    }
}
