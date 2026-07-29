using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo
{
    public class PI : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  XPI-RECFOND-BIS REDEFINES COMUNE-RECFOND.
        //          02  XPI-RECFOND.
        //              03 XPITIPOR                      PIC X.
        //              03 XPIFONDO                      PIC X(3).
        //              03 XPITPENS                      PIC 9.
        //              03 XPINATUR.
        //                 04 XPINATU1                   PIC X.
        //                 04 XPINATU2                   PIC 9.
        //                 04 XPINATU3                   PIC X.
        //D2000         03 XPIDECOR.
        //D2NEW            04 XPIDECAA                   PIC 9999.                
        //                 04 XPIDECMM                   PIC 99.
        //                 04 XPIDECGG                   PIC 99.
        //D2000         03 XPISCADE.
        //D2NEW            04 XPISCAAA                   PIC 9999.                
        //                 04 XPISCAMM                   PIC 99.
        //D2000         03 XPIASSUN.
        //D2NEW            04 XPIASSAA                   PIC 9999.                
        //                 04 XPIASSMM                   PIC 99.
        //                 04 XPIASSGG                   PIC 99.
        //D2000         03 XPICESSA.
        //D2NEW            04 XPICESAA                   PIC 9999.                
        //                 04 XPICESMM                   PIC 99.
        //                 04 XPICESGG                   PIC 99.
        //              03 XPISERVA                      PIC 99.
        //              03 XPISERVM                      PIC 99.
        //              03 XPISERVG                      PIC 99.
        //              03 XPINCALC                      PIC 9.
        //              03 XPIATTIV                      PIC 99.
        //              03 XPIFISSE                      PIC 9.
        //              03 XPIEXCBT                      PIC XX.
        //              03 XPINO336                      PIC 9(5)V9999.
        //              03 XPIRISCA                      PIC 99.
        //              03 XPIRISCM                      PIC 99.
        //              03 XPIRISCG                      PIC 99.
        //              03 XPICAMPA                      PIC 99.
        //              03 XPICAMPM                      PIC 99.
        //              03 XPICAMPG                      PIC 99.
        //              03 XPIQUALI                      PIC X(5).
        //              03 XPISCATT                      PIC X(2).
        //              03 XPICAPIN                      PIC 99.
        //              03 XPICAPDE                      PIC 9(4).
        //              03 XPIINAIL                      PIC 9(6)V9999.
        //              03 XPIRDISI                      PIC 99.
        //              03 XPIRDISD                      PIC 9(4).
        //              03 XPIRDPNI                      PIC 99.
        //              03 XPIRDPND                      PIC 9(4).
        //              03 XPIOKIIS                      PIC X.
        //              03 XPIINTEG                      PIC 9(6)V99.
        //              03 XPIFACOL                      PIC 9(4)V9999.
        //              03 XPISTIPE                      PIC 9(6)V9999.
        //              03 XPI36BIS                      PIC 9(6)V9999.
        //              03 XPIPRIVI                      PIC 9.
        //              03 XPIAS762                      PIC 9(6)V9999.
        //              03 XPIMEDIC                      PIC X.
        //              03 XPICVITA                      PIC 9(3)V9999.
        //              03 XPIONLEG                      PIC 9(6)V9999.
        //              03 XPIDP346                      PIC 9(6)V9999.
        //              03 XPISANIT                      PIC 9(6)V9999.
        //              03 XPITIPRG                      PIC XX.
        //              03 XPIAR22F                      PIC 9.
        //              03 XPICODAF                      PIC X.
        //              03 XPIULTSE                      PIC 9(4).
        //              03 XPINONVE                      PIC 9.
        //              03 XPISPECI                      PIC X.
        //              03 XPIPROGR                      PIC 99.
        //          02  FILLER                           PIC X(55).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  XPI-RECFOND-BIS REDEFINES COMUNE-RECFOND.
        // 02  XPI-RECFOND.
        /// <summary>
        /// XPITIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string XPITIPOR { get; set; }

        /// <summary>
        /// XPIFONDO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string XPIFONDO { get; set; }

        /// <summary>
        /// XPITPENS 9  
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short XPITPENS { get; set; }

        // 03 XPINATUR.
        /// <summary>
        /// XPINATU1 X  
        /// </summary>
        [HisFieldInfoMapping(3, 1)]
        public string XPINATU1 { get; set; }

        /// <summary>
        /// XPINATU2 9  
        /// </summary>
        [HisFieldInfoMapping(4, 1, CobolType = CobolType.Unsigned)]
        public short XPINATU2 { get; set; }

        /// <summary>
        /// XPINATU3 X  
        /// </summary>
        [HisFieldInfoMapping(5, 1)]
        public string XPINATU3 { get; set; }

        // D2000         03 XPIDECOR.
        /// <summary>
        /// XPIDECAA 9999  
        /// </summary>
        [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
        public short XPIDECAA { get; set; }

        /// <summary>
        /// XPIDECMM 99  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short XPIDECMM { get; set; }

        /// <summary>
        /// XPIDECGG 99  
        /// </summary>
        [HisFieldInfoMapping(8, 2, CobolType = CobolType.Unsigned)]
        public short XPIDECGG { get; set; }

        // D2000         03 XPISCADE.
        /// <summary>
        /// XPISCAAA 9999  
        /// </summary>
        [HisFieldInfoMapping(9, 4, CobolType = CobolType.Unsigned)]
        public short XPISCAAA { get; set; }

        /// <summary>
        /// XPISCAMM 99  
        /// </summary>
        [HisFieldInfoMapping(10, 2, CobolType = CobolType.Unsigned)]
        public short XPISCAMM { get; set; }

        // D2000         03 XPIASSUN.
        /// <summary>
        /// XPIASSAA 9999  
        /// </summary>
        [HisFieldInfoMapping(11, 4, CobolType = CobolType.Unsigned)]
        public short XPIASSAA { get; set; }

        /// <summary>
        /// XPIASSMM 99  
        /// </summary>
        [HisFieldInfoMapping(12, 2, CobolType = CobolType.Unsigned)]
        public short XPIASSMM { get; set; }

        /// <summary>
        /// XPIASSGG 99  
        /// </summary>
        [HisFieldInfoMapping(13, 2, CobolType = CobolType.Unsigned)]
        public short XPIASSGG { get; set; }

        // D2000         03 XPICESSA.
        /// <summary>
        /// XPICESAA 9999  
        /// </summary>
        [HisFieldInfoMapping(14, 4, CobolType = CobolType.Unsigned)]
        public short XPICESAA { get; set; }

        /// <summary>
        /// XPICESMM 99  
        /// </summary>
        [HisFieldInfoMapping(15, 2, CobolType = CobolType.Unsigned)]
        public short XPICESMM { get; set; }

        /// <summary>
        /// XPICESGG 99  
        /// </summary>
        [HisFieldInfoMapping(16, 2, CobolType = CobolType.Unsigned)]
        public short XPICESGG { get; set; }

        /// <summary>
        /// XPISERVA 99  
        /// </summary>
        [HisFieldInfoMapping(17, 2, CobolType = CobolType.Unsigned)]
        public short XPISERVA { get; set; }

        /// <summary>
        /// XPISERVM 99  
        /// </summary>
        [HisFieldInfoMapping(18, 2, CobolType = CobolType.Unsigned)]
        public short XPISERVM { get; set; }

        /// <summary>
        /// XPISERVG 99  
        /// </summary>
        [HisFieldInfoMapping(19, 2, CobolType = CobolType.Unsigned)]
        public short XPISERVG { get; set; }

        /// <summary>
        /// XPINCALC 9  
        /// </summary>
        [HisFieldInfoMapping(20, 1, CobolType = CobolType.Unsigned)]
        public short XPINCALC { get; set; }

        /// <summary>
        /// XPIATTIV 99  
        /// </summary>
        [HisFieldInfoMapping(21, 2, CobolType = CobolType.Unsigned)]
        public short XPIATTIV { get; set; }

        /// <summary>
        /// XPIFISSE 9  
        /// </summary>
        [HisFieldInfoMapping(22, 1, CobolType = CobolType.Unsigned)]
        public short XPIFISSE { get; set; }

        /// <summary>
        /// XPIEXCBT XX  
        /// </summary>
        [HisFieldInfoMapping(23, 2)]
        public string XPIEXCBT { get; set; }

        /// <summary>
        /// XPINO336 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(24, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XPINO336 { get; set; }

        /// <summary>
        /// XPIRISCA 99  
        /// </summary>
        [HisFieldInfoMapping(25, 2, CobolType = CobolType.Unsigned)]
        public short XPIRISCA { get; set; }

        /// <summary>
        /// XPIRISCM 99  
        /// </summary>
        [HisFieldInfoMapping(26, 2, CobolType = CobolType.Unsigned)]
        public short XPIRISCM { get; set; }

        /// <summary>
        /// XPIRISCG 99  
        /// </summary>
        [HisFieldInfoMapping(27, 2, CobolType = CobolType.Unsigned)]
        public short XPIRISCG { get; set; }

        /// <summary>
        /// XPICAMPA 99  
        /// </summary>
        [HisFieldInfoMapping(28, 2, CobolType = CobolType.Unsigned)]
        public short XPICAMPA { get; set; }

        /// <summary>
        /// XPICAMPM 99  
        /// </summary>
        [HisFieldInfoMapping(29, 2, CobolType = CobolType.Unsigned)]
        public short XPICAMPM { get; set; }

        /// <summary>
        /// XPICAMPG 99  
        /// </summary>
        [HisFieldInfoMapping(30, 2, CobolType = CobolType.Unsigned)]
        public short XPICAMPG { get; set; }

        /// <summary>
        /// XPIQUALI X(5)  
        /// </summary>
        [HisFieldInfoMapping(31, 5)]
        public string XPIQUALI { get; set; }

        /// <summary>
        /// XPISCATT X(2)  
        /// </summary>
        [HisFieldInfoMapping(32, 2)]
        public string XPISCATT { get; set; }

        /// <summary>
        /// XPICAPIN 99  
        /// </summary>
        [HisFieldInfoMapping(33, 2, CobolType = CobolType.Unsigned)]
        public short XPICAPIN { get; set; }

        /// <summary>
        /// XPICAPDE 9(4)  
        /// </summary>
        [HisFieldInfoMapping(34, 4, CobolType = CobolType.Unsigned)]
        public short XPICAPDE { get; set; }

        /// <summary>
        /// XPIINAIL 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(35, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XPIINAIL { get; set; }

        /// <summary>
        /// XPIRDISI 99  
        /// </summary>
        [HisFieldInfoMapping(36, 2, CobolType = CobolType.Unsigned)]
        public short XPIRDISI { get; set; }

        /// <summary>
        /// XPIRDISD 9(4)  
        /// </summary>
        [HisFieldInfoMapping(37, 4, CobolType = CobolType.Unsigned)]
        public short XPIRDISD { get; set; }

        /// <summary>
        /// XPIRDPNI 99  
        /// </summary>
        [HisFieldInfoMapping(38, 2, CobolType = CobolType.Unsigned)]
        public short XPIRDPNI { get; set; }

        /// <summary>
        /// XPIRDPND 9(4)  
        /// </summary>
        [HisFieldInfoMapping(39, 4, CobolType = CobolType.Unsigned)]
        public short XPIRDPND { get; set; }

        /// <summary>
        /// XPIOKIIS X  
        /// </summary>
        [HisFieldInfoMapping(40, 1)]
        public string XPIOKIIS { get; set; }

        /// <summary>
        /// XPIINTEG 9(6)V9(2)  
        /// </summary>
        [HisFieldInfoMapping(41, 8, Scale = 2, CobolType = CobolType.Unsigned)]
        public decimal XPIINTEG { get; set; }

        /// <summary>
        /// XPIFACOL 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(42, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XPIFACOL { get; set; }

        /// <summary>
        /// XPISTIPE 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(43, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XPISTIPE { get; set; }

        /// <summary>
        /// XPI36BIS 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(44, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XPI36BIS { get; set; }

        /// <summary>
        /// XPIPRIVI 9  
        /// </summary>
        [HisFieldInfoMapping(45, 1, CobolType = CobolType.Unsigned)]
        public short XPIPRIVI { get; set; }

        /// <summary>
        /// XPIAS762 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(46, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XPIAS762 { get; set; }

        /// <summary>
        /// XPIMEDIC X  
        /// </summary>
        [HisFieldInfoMapping(47, 1)]
        public string XPIMEDIC { get; set; }

        /// <summary>
        /// XPICVITA 9(3)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(48, 7, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XPICVITA { get; set; }

        /// <summary>
        /// XPIONLEG 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(49, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XPIONLEG { get; set; }

        /// <summary>
        /// XPIDP346 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(50, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XPIDP346 { get; set; }

        /// <summary>
        /// XPISANIT 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(51, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XPISANIT { get; set; }

        /// <summary>
        /// XPITIPRG XX  
        /// </summary>
        [HisFieldInfoMapping(52, 2)]
        public string XPITIPRG { get; set; }

        /// <summary>
        /// XPIAR22F 9  
        /// </summary>
        [HisFieldInfoMapping(53, 1, CobolType = CobolType.Unsigned)]
        public short XPIAR22F { get; set; }

        /// <summary>
        /// XPICODAF X  
        /// </summary>
        [HisFieldInfoMapping(54, 1)]
        public string XPICODAF { get; set; }

        /// <summary>
        /// XPIULTSE 9(4)  
        /// </summary>
        [HisFieldInfoMapping(55, 4, CobolType = CobolType.Unsigned)]
        public short XPIULTSE { get; set; }

        /// <summary>
        /// XPINONVE 9  
        /// </summary>
        [HisFieldInfoMapping(56, 1, CobolType = CobolType.Unsigned)]
        public short XPINONVE { get; set; }

        /// <summary>
        /// XPISPECI X  
        /// </summary>
        [HisFieldInfoMapping(57, 1)]
        public string XPISPECI { get; set; }

        /// <summary>
        /// XPIPROGR 99  
        /// </summary>
        [HisFieldInfoMapping(58, 2, CobolType = CobolType.Unsigned)]
        public short XPIPROGR { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "PI"; }
        }
        #endregion Properties
    }
}
