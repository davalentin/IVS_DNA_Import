using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago
{
    public class PI : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  YPI-RECAGO-BIS REDEFINES COMUNE-RECAGO.
        //          02  YPI-RECAGO.
        //              03 YPITIPOR                      PIC X.
        //              03 YPIFONDO                      PIC X(3).
        //              03 YPITPENS                      PIC 9.
        //              03 YPITPAGO                      PIC 9.
        //D2NEW         03 YPIDECAA                      PIC 9999.                
        //              03 YPIDECMM                      PIC 99.
        //D2NEW         03 YPISOSAA                      PIC 9999.                
        //              03 YPISOSMM                      PIC 99.
        //              03 YPICARIC                      PIC X.
        //              03 YPIQUOFI                      PIC 9.
        //              03 YPIALTR1                      PIC 9.
        //              03 YPIALTR2                      PIC X.
        //              03 YPIALTR3                      PIC X.
        //              03 YPIRTSET                      PIC 9(5)V9999.
        //              03 YPISZRET                      PIC 9(5)V9999.
        //              03 YPISZTOT                      PIC 9(5).
        //              03 YPISZESC                      PIC 9(5).
        //              03 YPINZTOT                      PIC 9(5).
        //              03 YPINZESC                      PIC 9(5).
        //              03 YPICTRTT                      PIC 9(3)V9999.
        //              03 YPICTRES                      PIC 9(5)V9999.
        //              03 YPIES336                      PIC 9(3)V9999.
        //              03 YPIINT11                      PIC 9(3)V9999.
        //              03 YPISETVV                      PIC 9(4).
        //              03 YPIMUTSC                      PIC 9(3)V9999.
        //              03 YPIMART2                      PIC 9(4).
        //              03 YPIMDSET                      PIC 9(3)V9999.
        //              03 YPICT336                      PIC 9(3).
        //              03 YPIST336                      PIC 9(3).
        //              03 YPIRDCIE                      PIC 9.
        //D2000         03 YPIDPCDC.
        //D2NEW            04 YPICDCAA                   PIC 9999.                
        //                 04 YPICDCMM                   PIC 99.
        //              03 YPIDPCRT                      PIC 9(5)V9999.
        //              03 YPIS72RT                      PIC 9(5)V9999.
        //D2NEW         03 YPICB140                      PIC 9(6).                
        //      *-L.503
        //              03 YPIREQUA                      PIC X.
        //              03 YPIREQUB                      PIC 9.
        //              03 YPIRSETB                      PIC 9(5)V9999.
        //              03 YPISRETB                      PIC 9(5)V9999.
        //              03 YPISTOTB                      PIC 9(5).
        //              03 YPISESCB                      PIC 9(5).
        //              03 YPINTOTB                      PIC 9(5).
        //              03 YPINESCB                      PIC 9(5).
        //              03 YPISPECI                      PIC X.
        //              03 YPIDECSS                      PIC 99.
        //              03 YPISOSSS                      PIC 99.
        //              03 YPIAUTON                      PIC XX.
        //      * - 233
        //              03 YPICOULT                      PIC 9.
        //              03 YPICODCD                      PIC 9.
        //              03 YPIIVSCD                      PIC 9(5)V9999.
        //              03 YPIRCDA                       PIC 9(6)V9999.
        //              03 YPIACDA                       PIC 9(4).
        //              03 YPIRCDB                       PIC 9(6)V9999.
        //              03 YPIACDB                       PIC 9(4).
        //              03 YPIATCD                       PIC 9(4).
        //              03 YPICODAR                      PIC 9.
        //              03 YPIIVSAR                      PIC 9(5)V9999.
        //              03 YPIRARTA                      PIC 9(6)V9999.
        //              03 YPIAARTA                      PIC 9(4).
        //              03 YPIRARTB                      PIC 9(6)V9999.
        //              03 YPIAARTB                      PIC 9(4).
        //              03 YPIATART                      PIC 9(4).
        //              03 YPICODCO                      PIC 9.
        //              03 YPIIVSCO                      PIC 9(5)V9999.
        //              03 YPIRCOMA                      PIC 9(6)V9999.
        //              03 YPIACOMA                      PIC 9(4).
        //              03 YPIRCOMB                      PIC 9(6)V9999.
        //              03 YPIACOMB                      PIC 9(4).
        //              03 YPIATCOM                      PIC 9(4).
        //              03 YPIPROGR                      PIC 99.
        //           02 YPIDISPO                         PIC X(78).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  YPI-RECAGO-BIS REDEFINES COMUNE-RECAGO.
        // 02  YPI-RECAGO.
        /// <summary>
        /// YPITIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string YPITIPOR { get; set; }

        /// <summary>
        /// YPIFONDO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string YPIFONDO { get; set; }

        /// <summary>
        /// YPITPENS 9  
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short YPITPENS { get; set; }

        /// <summary>
        /// YPITPAGO 9  
        /// </summary>
        [HisFieldInfoMapping(3, 1, CobolType = CobolType.Unsigned)]
        public short YPITPAGO { get; set; }

        /// <summary>
        /// YPIDECAA 9999  
        /// </summary>
        [HisFieldInfoMapping(4, 4, CobolType = CobolType.Unsigned)]
        public short YPIDECAA { get; set; }

        /// <summary>
        /// YPIDECMM 99  
        /// </summary>
        [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
        public short YPIDECMM { get; set; }

        /// <summary>
        /// YPISOSAA 9999  
        /// </summary>
        [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
        public short YPISOSAA { get; set; }

        /// <summary>
        /// YPISOSMM 99  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short YPISOSMM { get; set; }

        /// <summary>
        /// YPICARIC X  
        /// </summary>
        [HisFieldInfoMapping(8, 1)]
        public string YPICARIC { get; set; }

        /// <summary>
        /// YPIQUOFI 9  
        /// </summary>
        [HisFieldInfoMapping(9, 1, CobolType = CobolType.Unsigned)]
        public short YPIQUOFI { get; set; }

        /// <summary>
        /// YPIALTR1 9  
        /// </summary>
        [HisFieldInfoMapping(10, 1, CobolType = CobolType.Unsigned)]
        public short YPIALTR1 { get; set; }

        /// <summary>
        /// YPIALTR2 X  
        /// </summary>
        [HisFieldInfoMapping(11, 1)]
        public string YPIALTR2 { get; set; }

        /// <summary>
        /// YPIALTR3 X  
        /// </summary>
        [HisFieldInfoMapping(12, 1)]
        public string YPIALTR3 { get; set; }

        /// <summary>
        /// YPIRTSET 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(13, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPIRTSET { get; set; }

        /// <summary>
        /// YPISZRET 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(14, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPISZRET { get; set; }

        /// <summary>
        /// YPISZTOT 9(5)  
        /// </summary>
        [HisFieldInfoMapping(15, 5, CobolType = CobolType.Unsigned)]
        public int YPISZTOT { get; set; }

        /// <summary>
        /// YPISZESC 9(5)  
        /// </summary>
        [HisFieldInfoMapping(16, 5, CobolType = CobolType.Unsigned)]
        public int YPISZESC { get; set; }

        /// <summary>
        /// YPINZTOT 9(5)  
        /// </summary>
        [HisFieldInfoMapping(17, 5, CobolType = CobolType.Unsigned)]
        public int YPINZTOT { get; set; }

        /// <summary>
        /// YPINZESC 9(5)  
        /// </summary>
        [HisFieldInfoMapping(18, 5, CobolType = CobolType.Unsigned)]
        public int YPINZESC { get; set; }

        /// <summary>
        /// YPICTRTT 9(3)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(19, 7, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPICTRTT { get; set; }

        /// <summary>
        /// YPICTRES 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(20, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPICTRES { get; set; }

        /// <summary>
        /// YPIES336 9(3)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(21, 7, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPIES336 { get; set; }

        /// <summary>
        /// YPIINT11 9(3)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(22, 7, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPIINT11 { get; set; }

        /// <summary>
        /// YPISETVV 9(4)  
        /// </summary>
        [HisFieldInfoMapping(23, 4, CobolType = CobolType.Unsigned)]
        public short YPISETVV { get; set; }

        /// <summary>
        /// YPIMUTSC 9(3)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(24, 7, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPIMUTSC { get; set; }

        /// <summary>
        /// YPIMART2 9(4)  
        /// </summary>
        [HisFieldInfoMapping(25, 4, CobolType = CobolType.Unsigned)]
        public short YPIMART2 { get; set; }

        /// <summary>
        /// YPIMDSET 9(3)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(26, 7, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPIMDSET { get; set; }

        /// <summary>
        /// YPICT336 9(3)  
        /// </summary>
        [HisFieldInfoMapping(27, 3, CobolType = CobolType.Unsigned)]
        public short YPICT336 { get; set; }

        /// <summary>
        /// YPIST336 9(3)  
        /// </summary>
        [HisFieldInfoMapping(28, 3, CobolType = CobolType.Unsigned)]
        public short YPIST336 { get; set; }

        /// <summary>
        /// YPIRDCIE 9  
        /// </summary>
        [HisFieldInfoMapping(29, 1, CobolType = CobolType.Unsigned)]
        public short YPIRDCIE { get; set; }

        // D2000         03 YPIDPCDC.
        /// <summary>
        /// YPICDCAA 9999  
        /// </summary>
        [HisFieldInfoMapping(30, 4, CobolType = CobolType.Unsigned)]
        public short YPICDCAA { get; set; }

        /// <summary>
        /// YPICDCMM 99  
        /// </summary>
        [HisFieldInfoMapping(31, 2, CobolType = CobolType.Unsigned)]
        public short YPICDCMM { get; set; }

        /// <summary>
        /// YPIDPCRT 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(32, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPIDPCRT { get; set; }

        /// <summary>
        /// YPIS72RT 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(33, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPIS72RT { get; set; }

        /// <summary>
        /// YPICB140 9(6)  
        /// </summary>
        [HisFieldInfoMapping(34, 6, CobolType = CobolType.Unsigned)]
        public int YPICB140 { get; set; }

        // *-L.503
        /// <summary>
        /// YPIREQUA X  
        /// </summary>
        [HisFieldInfoMapping(35, 1)]
        public string YPIREQUA { get; set; }

        /// <summary>
        /// YPIREQUB 9  
        /// </summary>
        [HisFieldInfoMapping(36, 1, CobolType = CobolType.Unsigned)]
        public short YPIREQUB { get; set; }

        /// <summary>
        /// YPIRSETB 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(37, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPIRSETB { get; set; }

        /// <summary>
        /// YPISRETB 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(38, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPISRETB { get; set; }

        /// <summary>
        /// YPISTOTB 9(5)  
        /// </summary>
        [HisFieldInfoMapping(39, 5, CobolType = CobolType.Unsigned)]
        public int YPISTOTB { get; set; }

        /// <summary>
        /// YPISESCB 9(5)  
        /// </summary>
        [HisFieldInfoMapping(40, 5, CobolType = CobolType.Unsigned)]
        public int YPISESCB { get; set; }

        /// <summary>
        /// YPINTOTB 9(5)  
        /// </summary>
        [HisFieldInfoMapping(41, 5, CobolType = CobolType.Unsigned)]
        public int YPINTOTB { get; set; }

        /// <summary>
        /// YPINESCB 9(5)  
        /// </summary>
        [HisFieldInfoMapping(42, 5, CobolType = CobolType.Unsigned)]
        public int YPINESCB { get; set; }

        /// <summary>
        /// YPISPECI X  
        /// </summary>
        [HisFieldInfoMapping(43, 1)]
        public string YPISPECI { get; set; }

        /// <summary>
        /// YPIDECSS 99  
        /// </summary>
        [HisFieldInfoMapping(44, 2, CobolType = CobolType.Unsigned)]
        public short YPIDECSS { get; set; }

        /// <summary>
        /// YPISOSSS 99  
        /// </summary>
        [HisFieldInfoMapping(45, 2, CobolType = CobolType.Unsigned)]
        public short YPISOSSS { get; set; }

        /// <summary>
        /// YPIAUTON XX  
        /// </summary>
        [HisFieldInfoMapping(46, 2)]
        public string YPIAUTON { get; set; }

        // * - 233
        /// <summary>
        /// YPICOULT 9  
        /// </summary>
        [HisFieldInfoMapping(47, 1, CobolType = CobolType.Unsigned)]
        public short YPICOULT { get; set; }

        /// <summary>
        /// YPICODCD 9  
        /// </summary>
        [HisFieldInfoMapping(48, 1, CobolType = CobolType.Unsigned)]
        public short YPICODCD { get; set; }

        /// <summary>
        /// YPIIVSCD 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(49, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPIIVSCD { get; set; }

        /// <summary>
        /// YPIRCDA 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(50, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPIRCDA { get; set; }

        /// <summary>
        /// YPIACDA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(51, 4, CobolType = CobolType.Unsigned)]
        public short YPIACDA { get; set; }

        /// <summary>
        /// YPIRCDB 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(52, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPIRCDB { get; set; }

        /// <summary>
        /// YPIACDB 9(4)  
        /// </summary>
        [HisFieldInfoMapping(53, 4, CobolType = CobolType.Unsigned)]
        public short YPIACDB { get; set; }

        /// <summary>
        /// YPIATCD 9(4)  
        /// </summary>
        [HisFieldInfoMapping(54, 4, CobolType = CobolType.Unsigned)]
        public short YPIATCD { get; set; }

        /// <summary>
        /// YPICODAR 9  
        /// </summary>
        [HisFieldInfoMapping(55, 1, CobolType = CobolType.Unsigned)]
        public short YPICODAR { get; set; }

        /// <summary>
        /// YPIIVSAR 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(56, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPIIVSAR { get; set; }

        /// <summary>
        /// YPIRARTA 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(57, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPIRARTA { get; set; }

        /// <summary>
        /// YPIAARTA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(58, 4, CobolType = CobolType.Unsigned)]
        public short YPIAARTA { get; set; }

        /// <summary>
        /// YPIRARTB 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(59, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPIRARTB { get; set; }

        /// <summary>
        /// YPIAARTB 9(4)  
        /// </summary>
        [HisFieldInfoMapping(60, 4, CobolType = CobolType.Unsigned)]
        public short YPIAARTB { get; set; }

        /// <summary>
        /// YPIATART 9(4)  
        /// </summary>
        [HisFieldInfoMapping(61, 4, CobolType = CobolType.Unsigned)]
        public short YPIATART { get; set; }

        /// <summary>
        /// YPICODCO 9  
        /// </summary>
        [HisFieldInfoMapping(62, 1, CobolType = CobolType.Unsigned)]
        public short YPICODCO { get; set; }

        /// <summary>
        /// YPIIVSCO 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(63, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPIIVSCO { get; set; }

        /// <summary>
        /// YPIRCOMA 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(64, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPIRCOMA { get; set; }

        /// <summary>
        /// YPIACOMA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(65, 4, CobolType = CobolType.Unsigned)]
        public short YPIACOMA { get; set; }

        /// <summary>
        /// YPIRCOMB 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(66, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPIRCOMB { get; set; }

        /// <summary>
        /// YPIACOMB 9(4)  
        /// </summary>
        [HisFieldInfoMapping(67, 4, CobolType = CobolType.Unsigned)]
        public short YPIACOMB { get; set; }

        /// <summary>
        /// YPIATCOM 9(4)  
        /// </summary>
        [HisFieldInfoMapping(68, 4, CobolType = CobolType.Unsigned)]
        public short YPIATCOM { get; set; }

        /// <summary>
        /// YPIPROGR 99  
        /// </summary>
        [HisFieldInfoMapping(69, 2, CobolType = CobolType.Unsigned)]
        public short YPIPROGR { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "PI"; }
        }
        #endregion Properties
    }
}
