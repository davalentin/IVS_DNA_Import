using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago
{
    public class DZ : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  YDZ-RECAGO-BIS REDEFINES COMUNE-RECAGO.
        //    02  YDZ-RECAGO.
        //        03 YDZTIPOR                      PIC X.
        //        03 YDZFONDO                      PIC X(3).
        //        03 YDZTIPEN                      PIC 9.
        //        03 YDZDECSS                      PIC 99.
        //        03 YDZDECAA                      PIC 99.
        //        03 YDZDECMM                      PIC 99.
        //        03 YDZSCASS                      PIC 99.
        //        03 YDZSCAAA                      PIC 99.
        //        03 YDZSCAMM                      PIC 99.
        //*-RETRIBUZIONE MEDIA SETTIMANALE (A)
        //        03 YDZRSETA                      PIC 9(6)V9999.
        //*-SETTIMANE DI CONTRIBUZIONE (A)
        //        03 YDZSETTA                      PIC 9(5).
        //*-RETRIBUZIONE MEDIA SETTIMANALE (B)
        //        03 YDZRSETB                      PIC 9(6)V9999.
        //*-SETTIMANE DI CONTRIBUZIONE (B)
        //        03 YDZSETTB                      PIC 9(5).
        //GD0212        03 YDZIMPCRT                     PIC 9(6)V9999.  
        //GD0212        03 YDZMONTA2012                  PIC 9(7)V9999.      
        //GD0212        03 YDZSETT2012                   PIC 9(4). 
        //GD1012        03 YDZFLAG214                    PIC X.
        //GD1012        03 YDZPERC214                    PIC 99V99.
        //GD1017        03 YDZSETA-707                     PIC 9(4).
        //              03 YDZSETB-707                     PIC 9(4).
        //              03 YDZCALC707 PIC X(01).   
        //*-PROGRESSIVO RECORD
        //        03 YDZPROGR                      PIC 99.
        //*-AREA A DISPOSIZIONE
        //        03 YDZFILLER                     PIC X(316).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  YDZ-RECAGO-BIS REDEFINES COMUNE-RECAGO.
        // 02  YDZ-RECAGO.
        /// <summary>
        /// YDZTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string YDZTIPOR { get; set; }

        /// <summary>
        /// YDZFONDO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string YDZFONDO { get; set; }

        /// <summary>
        /// YDZTIPEN 9  
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short YDZTIPEN { get; set; }

        /// <summary>
        /// YDZDECSS 99  
        /// </summary>
        [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
        public short YDZDECSS { get; set; }

        /// <summary>
        /// YDZDECAA 99  
        /// </summary>
        [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
        public short YDZDECAA { get; set; }

        /// <summary>
        /// YDZDECMM 99  
        /// </summary>
        [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
        public short YDZDECMM { get; set; }

        /// <summary>
        /// YDZSCASS 99  
        /// </summary>
        [HisFieldInfoMapping(6, 2, CobolType = CobolType.Unsigned)]
        public short YDZSCASS { get; set; }

        /// <summary>
        /// YDZSCAAA 99  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short YDZSCAAA { get; set; }

        /// <summary>
        /// YDZSCAMM 99  
        /// </summary>
        [HisFieldInfoMapping(8, 2, CobolType = CobolType.Unsigned)]
        public short YDZSCAMM { get; set; }

        // *-RETRIBUZIONE MEDIA SETTIMANALE (A)
        /// <summary>
        /// YDZRSETA 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(9, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YDZRSETA { get; set; }

        // *-SETTIMANE DI CONTRIBUZIONE (A)
        /// <summary>
        /// YDZSETTA 9(5)  
        /// </summary>
        [HisFieldInfoMapping(10, 5, CobolType = CobolType.Unsigned)]
        public int YDZSETTA { get; set; }

        // *-RETRIBUZIONE MEDIA SETTIMANALE (B)
        /// <summary>
        /// YDZRSETB 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(11, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YDZRSETB { get; set; }

        // *-SETTIMANE DI CONTRIBUZIONE (B)
        /// <summary>
        /// YDZSETTB 9(5)  
        /// </summary>
        [HisFieldInfoMapping(12, 5, CobolType = CobolType.Unsigned)]
        public int YDZSETTB { get; set; }

        /// </summary>
        /// YDZIMPCRT 9(6)V9999 
        /// </summary>
        [HisFieldInfoMapping(13, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YDZIMPCRT { get; set; }

        /// </summary>
        /// YDZMONTA2012 9(7)V9999      
        /// </summary>
        [HisFieldInfoMapping(14, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YDZMONTA2012 { get; set; }

        /// </summary>
        /// YDZSETT2012 9(4)
        /// </summary>
        [HisFieldInfoMapping(15, 4, CobolType = CobolType.Unsigned)]
        public int YDZSETT2012 { get; set; }

        /// </summary>
        /// YDZFLAG214 X
        /// </summary>
        [HisFieldInfoMapping(16, 1)]
        public string YDZFLAG214 { get; set; }

        /// </summary>
        /// YDZPERC214 99V99
        /// </summary>
        [HisFieldInfoMapping(17, 4, Scale = 2, CobolType = CobolType.Unsigned)]
        public decimal YDZPERC214 { get; set; }

        /// </summary>
        /// YDZSETA-707 PIC 9(4).
        /// </summary>
        [HisFieldInfoMapping(18, 4, CobolType = CobolType.Unsigned)]
        public int YDZSETA_707 { get; set; }

        /// </summary>
        /// YDZSETB-707 9(4)
        /// </summary>
        [HisFieldInfoMapping(19, 4, CobolType = CobolType.Unsigned)]
        public int YDZSETB_707 { get; set; }

        /// </summary>
        /// YDZCALC707 X(01)
        /// </summary>
        [HisFieldInfoMapping(20, 1)]
        public string YDZCALC707 { get; set; }

        // *-PROGRESSIVO RECORD
        /// <summary>
        /// YDZPROGR 99  
        /// </summary>
        [HisFieldInfoMapping(21, 2, CobolType = CobolType.Unsigned)]
        public short YDZPROGR { get; set; }

        //YDZIMPCRT 9(6)V9999
        //YDZMONTA2012 9(7)V9999      
        //YDZSETT2012 9(4)
        //YDZFLAG214 X
        //YDZPERC214 99V99
        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "DZ"; }
        }
        #endregion Properties
    }
}
