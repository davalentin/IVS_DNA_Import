using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo
{
    public class PM : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  XPM-RECFOND-BIS REDEFINES COMUNE-RECFOND.
        //          02  XPM-RECFOND.
        //              03 XPMTIPOR                      PIC X.
        //              03 XPMFONDO                      PIC X(3).
        //              03 XPMTPENS                      PIC 9.
        //              03 XPMNATUR.
        //                 04 XPMNATU1                   PIC 9.
        //                 04 XPMNATU2                   PIC X.
        //                 04 XPMNATU3                   PIC X.
        //D2000         03 XPMDECOR.
        //D2NEW            04 XPMDECAA                   PIC 9999.                
        //                 04 XPMDECMM                   PIC 99.
        //D2000         03 XPMSOSP.
        //D2NEW            04 XPMSOSAA                   PIC 9999.                
        //                 04 XPMSOSMM                   PIC 99.
        //D2000         03 XPMPRIVE.
        //D2NEW            04 XPMPRIAA                   PIC 9999.                
        //                 04 XPMPRIMM                   PIC 99.
        //                 04 XPMPRIGG                   PIC 99.
        //D2000         03 XPMULTVE.
        //D2NEW            04 XPMULTAA                   PIC 9999.                
        //                 04 XPMULTMM                   PIC 99.
        //                 04 XPMULTGG                   PIC 99.
        //              03 XPMSERVA                      PIC 99.
        //              03 XPMSERVM                      PIC 99.
        //              03 XPMRETPN                      PIC 9(6)V9999.
        //              03 XPMCONVI                      PIC X.
        //              03 XPMNCALC                      PIC 9.
        //              03 XPMATTIV                      PIC XX.
        //              03 XPMFISSE                      PIC 9.
        //              03 XPMEFFMM                      PIC 999.
        //              03 XPMEFFGG                      PIC 99.
        //              03 XPMRAPMM                      PIC 999.
        //              03 XPMRAPGG                      PIC 99.
        //              03 XPMTBCMM                      PIC 999.
        //              03 XPMTBCGG                      PIC 99.
        //              03 XPMMALMM                      PIC 999.
        //              03 XPMMALGG                      PIC 99.
        //              03 XPMESTMM                      PIC 999.
        //              03 XPMESTGG                      PIC 99.
        //              03 XPMALTMM                      PIC 999.
        //              03 XPMALTGG                      PIC 99.
        //              03 XPMMILMM                      PIC 999.
        //              03 XPMMILGG                      PIC 99.
        //              03 XPMMILDM                      PIC 999.
        //              03 XPMMILDG                      PIC 99.
        //              03 XPMMERMM                      PIC 999.
        //              03 XPMMERGG                      PIC 99.
        //              03 XPMTERMM                      PIC 999.
        //              03 XPMTERGG                      PIC 99.
        //              03 XPMMACAA                      PIC 99.
        //              03 XPMMACMM                      PIC 99.
        //              03 XPMCODIF                      PIC 9.
        //              03 XPMDIFAA                      PIC 99.
        //              03 XPMDIFMM                      PIC 99.
        //              03 XPMCORIP                      PIC 9.
        //              03 XPMRIPMM                      PIC 99.
        //D2NEW         03 XPMRIPAA                      PIC 9999.                
        //              03 XPMESCLU                      PIC 9(3).
        //              03 XPMSTATO                      PIC 9(3).
        //              03 XPMRENDI                      PIC 9(4)V9999.
        //              03 XPMSUPP1                      PIC 9(4)V9999.
        //              03 XPMSUPP2                      PIC 9(4)V9999.
        //              03 XPMTILIQ                      PIC 9.
        //              03 XPMDIAGO                      PIC 9.
        //              03 XPMANULT                      PIC 9.
        //              03 XPMNONVE                      PIC 9.
        //              03 XPMDPCDC                      PIC 9(6).                
        //              03 XPMDPCRT                      PIC 9(6)V9999.
        //              03 XPMS72RT                      PIC 9(6)V9999.
        //              03 XPMPROGR                      PIC 99.
        //           02 FILLER                             PIC X(76).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  XPM-RECFOND-BIS REDEFINES COMUNE-RECFOND.
        // 02  XPM-RECFOND.
        /// <summary>
        /// XPMTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string XPMTIPOR { get; set; }

        /// <summary>
        /// XPMFONDO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string XPMFONDO { get; set; }

        /// <summary>
        /// XPMTPENS 9  
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short XPMTPENS { get; set; }

        // 03 XPMNATUR.
        /// <summary>
        /// XPMNATU1 9  
        /// </summary>
        [HisFieldInfoMapping(3, 1, CobolType = CobolType.Unsigned)]
        public short XPMNATU1 { get; set; }

        /// <summary>
        /// XPMNATU2 X  
        /// </summary>
        [HisFieldInfoMapping(4, 1)]
        public string XPMNATU2 { get; set; }

        /// <summary>
        /// XPMNATU3 X  
        /// </summary>
        [HisFieldInfoMapping(5, 1)]
        public string XPMNATU3 { get; set; }

        // D2000         03 XPMDECOR.
        /// <summary>
        /// XPMDECAA 9999  
        /// </summary>
        [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
        public short XPMDECAA { get; set; }

        /// <summary>
        /// XPMDECMM 99  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short XPMDECMM { get; set; }

        // D2000         03 XPMSOSP.
        /// <summary>
        /// XPMSOSAA 9999  
        /// </summary>
        [HisFieldInfoMapping(8, 4, CobolType = CobolType.Unsigned)]
        public short XPMSOSAA { get; set; }

        /// <summary>
        /// XPMSOSMM 99  
        /// </summary>
        [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
        public short XPMSOSMM { get; set; }

        // D2000         03 XPMPRIVE.
        /// <summary>
        /// XPMPRIAA 9999  
        /// </summary>
        [HisFieldInfoMapping(10, 4, CobolType = CobolType.Unsigned)]
        public short XPMPRIAA { get; set; }

        /// <summary>
        /// XPMPRIMM 99  
        /// </summary>
        [HisFieldInfoMapping(11, 2, CobolType = CobolType.Unsigned)]
        public short XPMPRIMM { get; set; }

        /// <summary>
        /// XPMPRIGG 99  
        /// </summary>
        [HisFieldInfoMapping(12, 2, CobolType = CobolType.Unsigned)]
        public short XPMPRIGG { get; set; }

        // D2000         03 XPMULTVE.
        /// <summary>
        /// XPMULTAA 9999  
        /// </summary>
        [HisFieldInfoMapping(13, 4, CobolType = CobolType.Unsigned)]
        public short XPMULTAA { get; set; }

        /// <summary>
        /// XPMULTMM 99  
        /// </summary>
        [HisFieldInfoMapping(14, 2, CobolType = CobolType.Unsigned)]
        public short XPMULTMM { get; set; }

        /// <summary>
        /// XPMULTGG 99  
        /// </summary>
        [HisFieldInfoMapping(15, 2, CobolType = CobolType.Unsigned)]
        public short XPMULTGG { get; set; }

        /// <summary>
        /// XPMSERVA 99  
        /// </summary>
        [HisFieldInfoMapping(16, 2, CobolType = CobolType.Unsigned)]
        public short XPMSERVA { get; set; }

        /// <summary>
        /// XPMSERVM 99  
        /// </summary>
        [HisFieldInfoMapping(17, 2, CobolType = CobolType.Unsigned)]
        public short XPMSERVM { get; set; }

        /// <summary>
        /// XPMRETPN 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(18, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XPMRETPN { get; set; }

        /// <summary>
        /// XPMCONVI X  
        /// </summary>
        [HisFieldInfoMapping(19, 1)]
        public string XPMCONVI { get; set; }

        /// <summary>
        /// XPMNCALC 9  
        /// </summary>
        [HisFieldInfoMapping(20, 1, CobolType = CobolType.Unsigned)]
        public short XPMNCALC { get; set; }

        /// <summary>
        /// XPMATTIV XX  
        /// </summary>
        [HisFieldInfoMapping(21, 2)]
        public string XPMATTIV { get; set; }

        /// <summary>
        /// XPMFISSE 9  
        /// </summary>
        [HisFieldInfoMapping(22, 1, CobolType = CobolType.Unsigned)]
        public short XPMFISSE { get; set; }

        /// <summary>
        /// XPMEFFMM 999  
        /// </summary>
        [HisFieldInfoMapping(23, 3, CobolType = CobolType.Unsigned)]
        public short XPMEFFMM { get; set; }

        /// <summary>
        /// XPMEFFGG 99  
        /// </summary>
        [HisFieldInfoMapping(24, 2, CobolType = CobolType.Unsigned)]
        public short XPMEFFGG { get; set; }

        /// <summary>
        /// XPMRAPMM 999  
        /// </summary>
        [HisFieldInfoMapping(25, 3, CobolType = CobolType.Unsigned)]
        public short XPMRAPMM { get; set; }

        /// <summary>
        /// XPMRAPGG 99  
        /// </summary>
        [HisFieldInfoMapping(26, 2, CobolType = CobolType.Unsigned)]
        public short XPMRAPGG { get; set; }

        /// <summary>
        /// XPMTBCMM 999  
        /// </summary>
        [HisFieldInfoMapping(27, 3, CobolType = CobolType.Unsigned)]
        public short XPMTBCMM { get; set; }

        /// <summary>
        /// XPMTBCGG 99  
        /// </summary>
        [HisFieldInfoMapping(28, 2, CobolType = CobolType.Unsigned)]
        public short XPMTBCGG { get; set; }

        /// <summary>
        /// XPMMALMM 999  
        /// </summary>
        [HisFieldInfoMapping(29, 3, CobolType = CobolType.Unsigned)]
        public short XPMMALMM { get; set; }

        /// <summary>
        /// XPMMALGG 99  
        /// </summary>
        [HisFieldInfoMapping(30, 2, CobolType = CobolType.Unsigned)]
        public short XPMMALGG { get; set; }

        /// <summary>
        /// XPMESTMM 999  
        /// </summary>
        [HisFieldInfoMapping(31, 3, CobolType = CobolType.Unsigned)]
        public short XPMESTMM { get; set; }

        /// <summary>
        /// XPMESTGG 99  
        /// </summary>
        [HisFieldInfoMapping(32, 2, CobolType = CobolType.Unsigned)]
        public short XPMESTGG { get; set; }

        /// <summary>
        /// XPMALTMM 999  
        /// </summary>
        [HisFieldInfoMapping(33, 3, CobolType = CobolType.Unsigned)]
        public short XPMALTMM { get; set; }

        /// <summary>
        /// XPMALTGG 99  
        /// </summary>
        [HisFieldInfoMapping(34, 2, CobolType = CobolType.Unsigned)]
        public short XPMALTGG { get; set; }

        /// <summary>
        /// XPMMILMM 999  
        /// </summary>
        [HisFieldInfoMapping(35, 3, CobolType = CobolType.Unsigned)]
        public short XPMMILMM { get; set; }

        /// <summary>
        /// XPMMILGG 99  
        /// </summary>
        [HisFieldInfoMapping(36, 2, CobolType = CobolType.Unsigned)]
        public short XPMMILGG { get; set; }

        /// <summary>
        /// XPMMILDM 999  
        /// </summary>
        [HisFieldInfoMapping(37, 3, CobolType = CobolType.Unsigned)]
        public short XPMMILDM { get; set; }

        /// <summary>
        /// XPMMILDG 99  
        /// </summary>
        [HisFieldInfoMapping(38, 2, CobolType = CobolType.Unsigned)]
        public short XPMMILDG { get; set; }

        /// <summary>
        /// XPMMERMM 999  
        /// </summary>
        [HisFieldInfoMapping(39, 3, CobolType = CobolType.Unsigned)]
        public short XPMMERMM { get; set; }

        /// <summary>
        /// XPMMERGG 99  
        /// </summary>
        [HisFieldInfoMapping(40, 2, CobolType = CobolType.Unsigned)]
        public short XPMMERGG { get; set; }

        /// <summary>
        /// XPMTERMM 999  
        /// </summary>
        [HisFieldInfoMapping(41, 3, CobolType = CobolType.Unsigned)]
        public short XPMTERMM { get; set; }

        /// <summary>
        /// XPMTERGG 99  
        /// </summary>
        [HisFieldInfoMapping(42, 2, CobolType = CobolType.Unsigned)]
        public short XPMTERGG { get; set; }

        /// <summary>
        /// XPMMACAA 99  
        /// </summary>
        [HisFieldInfoMapping(43, 2, CobolType = CobolType.Unsigned)]
        public short XPMMACAA { get; set; }

        /// <summary>
        /// XPMMACMM 99  
        /// </summary>
        [HisFieldInfoMapping(44, 2, CobolType = CobolType.Unsigned)]
        public short XPMMACMM { get; set; }

        /// <summary>
        /// XPMCODIF 9  
        /// </summary>
        [HisFieldInfoMapping(45, 1, CobolType = CobolType.Unsigned)]
        public short XPMCODIF { get; set; }

        /// <summary>
        /// XPMDIFAA 99  
        /// </summary>
        [HisFieldInfoMapping(46, 2, CobolType = CobolType.Unsigned)]
        public short XPMDIFAA { get; set; }

        /// <summary>
        /// XPMDIFMM 99  
        /// </summary>
        [HisFieldInfoMapping(47, 2, CobolType = CobolType.Unsigned)]
        public short XPMDIFMM { get; set; }

        /// <summary>
        /// XPMCORIP 9  
        /// </summary>
        [HisFieldInfoMapping(48, 1, CobolType = CobolType.Unsigned)]
        public short XPMCORIP { get; set; }

        /// <summary>
        /// XPMRIPMM 99  
        /// </summary>
        [HisFieldInfoMapping(49, 2, CobolType = CobolType.Unsigned)]
        public short XPMRIPMM { get; set; }

        /// <summary>
        /// XPMRIPAA 9999  
        /// </summary>
        [HisFieldInfoMapping(50, 4, CobolType = CobolType.Unsigned)]
        public short XPMRIPAA { get; set; }

        /// <summary>
        /// XPMESCLU 9(3)  
        /// </summary>
        [HisFieldInfoMapping(51, 3, CobolType = CobolType.Unsigned)]
        public short XPMESCLU { get; set; }

        /// <summary>
        /// XPMSTATO 9(3)  
        /// </summary>
        [HisFieldInfoMapping(52, 3, CobolType = CobolType.Unsigned)]
        public short XPMSTATO { get; set; }

        /// <summary>
        /// XPMRENDI 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(53, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XPMRENDI { get; set; }

        /// <summary>
        /// XPMSUPP1 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(54, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XPMSUPP1 { get; set; }

        /// <summary>
        /// XPMSUPP2 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(55, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XPMSUPP2 { get; set; }

        /// <summary>
        /// XPMTILIQ 9  
        /// </summary>
        [HisFieldInfoMapping(56, 1, CobolType = CobolType.Unsigned)]
        public short XPMTILIQ { get; set; }

        /// <summary>
        /// XPMDIAGO 9  
        /// </summary>
        [HisFieldInfoMapping(57, 1, CobolType = CobolType.Unsigned)]
        public short XPMDIAGO { get; set; }

        /// <summary>
        /// XPMANULT 9  
        /// </summary>
        [HisFieldInfoMapping(58, 1, CobolType = CobolType.Unsigned)]
        public short XPMANULT { get; set; }

        /// <summary>
        /// XPMNONVE 9  
        /// </summary>
        [HisFieldInfoMapping(59, 1, CobolType = CobolType.Unsigned)]
        public short XPMNONVE { get; set; }

        /// <summary>
        /// XPMDPCDC 9(6)  
        /// </summary>
        [HisFieldInfoMapping(60, 6, CobolType = CobolType.Unsigned)]
        public int XPMDPCDC { get; set; }

        /// <summary>
        /// XPMDPCRT 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(61, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XPMDPCRT { get; set; }

        /// <summary>
        /// XPMS72RT 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(62, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XPMS72RT { get; set; }

        /// <summary>
        /// XPMPROGR 99  
        /// </summary>
        [HisFieldInfoMapping(63, 2, CobolType = CobolType.Unsigned)]
        public short XPMPROGR { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "PM"; }
        }
        #endregion Properties
    }
}