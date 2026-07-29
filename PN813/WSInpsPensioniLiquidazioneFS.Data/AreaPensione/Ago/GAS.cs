using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago
{
    public class GAS : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  YGA-RECAGO-BIS REDEFINES COMUNE-RECAGO.
        //          02  YGA-RECAGO.
        //              03 YGATIPOR                      PIC X.
        //              03 YGAFONDO                      PIC X(3).
        //              03 YGATPENS                      PIC 9.
        //              03 YGATPLIQ                      PIC 9.
        //D2NEW         03 YGADECAA                      PIC 9999.                
        //              03 YGADECMM                      PIC 99.
        //D2NEW         03 YGASOSAA                      PIC 9999.                
        //              03 YGASOSMM                      PIC 99.
        //D2NEW         03 YGATEOAA                      PIC 9999.                
        //              03 YGATEOMM                      PIC 99.
        //              03 YGARETPN                      PIC 9(6)V9999.
        //              03 YGAANZTO                      PIC 9(5).
        //              03 YGAANZES                      PIC 9(5).
        //              03 YGAANZVV                      PIC 9(5).
        //              03 YGACNTOT                      PIC 9(5)V9999.
        //              03 YGACNESC                      PIC 9(5)V9999.
        //              03 YGACNT14                      PIC 9(3)V9999.
        //              03 YGACNE14                      PIC 9(3)V9999.
        //              03 YGACNT11                      PIC 9(3)V9999.
        //              03 YGACNE11                      PIC 9(5)V9999.
        //              03 YGADIFFE                      PIC 9(3).
        //              03 YGAMATUR                      PIC 99.
        //D2000         03 YGADPCDC.
        //D2NEW            04 YGACDCAA                   PIC 9999.                
        //                 04 YGACDCMM                   PIC 99.
        //              03 YGADPCRT                      PIC 9(5)V9999.
        //              03 YGAS72RT                      PIC 9(5)V9999.
        //D2NEW         03 YGACB140                      PIC 9(6).                
        //      *-LEGGE 503
        //              03 YGAANZT2                      PIC 9(5).
        //              03 YGAANZE2                      PIC 9(5).
        //              03 YGARE2PN                      PIC 9(6)V9999.
        //              03 YGAREQU1                      PIC X.
        //              03 YGAREQU2                      PIC 9.
        //              03 YGASPECI                      PIC X.
        //              03 YGADECSS                      PIC 99.
        //              03 YGASOSSS                      PIC 99.
        //              03 YGAAUTON                      PIC XX.
        //      * - 233
        //              03 YGACOULT                      PIC 9.
        //              03 YGACODCD                      PIC 9.
        //              03 YGAIVSCD                      PIC 9(5)V9999.
        //              03 YGARCDA                       PIC 9(6)V9999.
        //              03 YGAACDA                       PIC 9(4).
        //              03 YGARCDB                       PIC 9(6)V9999.
        //              03 YGAACDB                       PIC 9(4).
        //              03 YGAATCD                       PIC 9(4).
        //              03 YGACODAR                      PIC 9.
        //              03 YGAIVSAR                      PIC 9(5)V9999.
        //              03 YGARARTA                      PIC 9(6)V9999.
        //              03 YGAAARTA                      PIC 9(4).
        //              03 YGARARTB                      PIC 9(6)V9999.
        //              03 YGAAARTB                      PIC 9(4).
        //              03 YGAATART                      PIC 9(4).
        //              03 YGACODCO                      PIC 9.
        //              03 YGAIVSCO                      PIC 9(5)V9999.
        //              03 YGARCOMA                      PIC 9(6)V9999.
        //              03 YGAACOMA                      PIC 9(4).
        //              03 YGARCOMB                      PIC 9(6)V9999.
        //              03 YGAACOMB                      PIC 9(4).
        //              03 YGAATCOM                      PIC 9(4).
        //              03 YGAMONTA                      PIC 9(7)V9999.
        //              03 YGAESCLU                      PIC 9(7)V9999.
        //GD1109        03 YGASETTE                      PIC 9(4). 
        //GD0212        03 YGAMONTA2012                  PIC 9(7)V9999.
        //GD0212        03 YGASETT2012                   PIC 9(4).   
        //GD1012        03 YGAFLAG214                    PIC X.
        //GD1012        03 YGAPERC214                    PIC 99V99.
        //GD1014        03 YGAMONTAE2012                 PIC 9(7)V9999.
        //GD0617        03 YGASETA-707                   PIC 9(4).
        //              03 YGASETB-707                   PIC 9(4).
        //              03 YGASETAES-707                 PIC 9(4).
        //              03 YGASETBES-707                 PIC 9(4).
        //              03 YGACALC707                    PIC X(01).    
        //              03 YGAPROGR                      PIC 99.
        //           02 YGADISPO                         PIC X(40).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  YGA-RECAGO-BIS REDEFINES COMUNE-RECAGO.
        // 02  YGA-RECAGO.
        /// <summary>
        /// YGATIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string YGATIPOR { get; set; }

        /// <summary>
        /// YGAFONDO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string YGAFONDO { get; set; }

        /// <summary>
        /// YGATPENS 9  
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short YGATPENS { get; set; }

        /// <summary>
        /// YGATPLIQ 9  
        /// </summary>
        [HisFieldInfoMapping(3, 1, CobolType = CobolType.Unsigned)]
        public short YGATPLIQ { get; set; }

        /// <summary>
        /// YGADECAA 9999  
        /// </summary>
        [HisFieldInfoMapping(4, 4, CobolType = CobolType.Unsigned)]
        public short YGADECAA { get; set; }

        /// <summary>
        /// YGADECMM 99  
        /// </summary>
        [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
        public short YGADECMM { get; set; }

        /// <summary>
        /// YGASOSAA 9999  
        /// </summary>
        [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
        public short YGASOSAA { get; set; }

        /// <summary>
        /// YGASOSMM 99  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short YGASOSMM { get; set; }

        /// <summary>
        /// YGATEOAA 9999  
        /// </summary>
        [HisFieldInfoMapping(8, 4, CobolType = CobolType.Unsigned)]
        public short YGATEOAA { get; set; }

        /// <summary>
        /// YGATEOMM 99  
        /// </summary>
        [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
        public short YGATEOMM { get; set; }

        /// <summary>
        /// YGARETPN 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(10, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGARETPN { get; set; }

        /// <summary>
        /// YGAANZTO 9(5)  
        /// </summary>
        [HisFieldInfoMapping(11, 5, CobolType = CobolType.Unsigned)]
        public int YGAANZTO { get; set; }

        /// <summary>
        /// YGAANZES 9(5)  
        /// </summary>
        [HisFieldInfoMapping(12, 5, CobolType = CobolType.Unsigned)]
        public int YGAANZES { get; set; }

        /// <summary>
        /// YGAANZVV 9(5)  
        /// </summary>
        [HisFieldInfoMapping(13, 5, CobolType = CobolType.Unsigned)]
        public int YGAANZVV { get; set; }

        /// <summary>
        /// YGACNTOT 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(14, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGACNTOT { get; set; }

        /// <summary>
        /// YGACNESC 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(15, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGACNESC { get; set; }

        /// <summary>
        /// YGACNT14 9(3)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(16, 7, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGACNT14 { get; set; }

        /// <summary>
        /// YGACNE14 9(3)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(17, 7, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGACNE14 { get; set; }

        /// <summary>
        /// YGACNT11 9(3)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(18, 7, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGACNT11 { get; set; }

        /// <summary>
        /// YGACNE11 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(19, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGACNE11 { get; set; }

        /// <summary>
        /// YGADIFFE 9(3)  
        /// </summary>
        [HisFieldInfoMapping(20, 3, CobolType = CobolType.Unsigned)]
        public short YGADIFFE { get; set; }

        /// <summary>
        /// YGAMATUR 99  
        /// </summary>
        [HisFieldInfoMapping(21, 2, CobolType = CobolType.Unsigned)]
        public short YGAMATUR { get; set; }

        // D2000         03 YGADPCDC.
        /// <summary>
        /// YGACDCAA 9999  
        /// </summary>
        [HisFieldInfoMapping(22, 4, CobolType = CobolType.Unsigned)]
        public short YGACDCAA { get; set; }

        /// <summary>
        /// YGACDCMM 99  
        /// </summary>
        [HisFieldInfoMapping(23, 2, CobolType = CobolType.Unsigned)]
        public short YGACDCMM { get; set; }

        /// <summary>
        /// YGADPCRT 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(24, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGADPCRT { get; set; }

        /// <summary>
        /// YGAS72RT 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(25, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGAS72RT { get; set; }

        /// <summary>
        /// YGACB140 9(6)  
        /// </summary>
        [HisFieldInfoMapping(26, 6, CobolType = CobolType.Unsigned)]
        public int YGACB140 { get; set; }

        // *-LEGGE 503
        /// <summary>
        /// YGAANZT2 9(5)  
        /// </summary>
        [HisFieldInfoMapping(27, 5, CobolType = CobolType.Unsigned)]
        public int YGAANZT2 { get; set; }

        /// <summary>
        /// YGAANZE2 9(5)  
        /// </summary>
        [HisFieldInfoMapping(28, 5, CobolType = CobolType.Unsigned)]
        public int YGAANZE2 { get; set; }

        /// <summary>
        /// YGARE2PN 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(29, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGARE2PN { get; set; }

        /// <summary>
        /// YGAREQU1 X  
        /// </summary>
        [HisFieldInfoMapping(30, 1)]
        public string YGAREQU1 { get; set; }

        /// <summary>
        /// YGAREQU2 9  
        /// </summary>
        [HisFieldInfoMapping(31, 1, CobolType = CobolType.Unsigned)]
        public short YGAREQU2 { get; set; }

        /// <summary>
        /// YGASPECI X  
        /// </summary>
        [HisFieldInfoMapping(32, 1)]
        public string YGASPECI { get; set; }

        /// <summary>
        /// YGADECSS 99  
        /// </summary>
        [HisFieldInfoMapping(33, 2, CobolType = CobolType.Unsigned)]
        public short YGADECSS { get; set; }

        /// <summary>
        /// YGASOSSS 99  
        /// </summary>
        [HisFieldInfoMapping(34, 2, CobolType = CobolType.Unsigned)]
        public short YGASOSSS { get; set; }

        /// <summary>
        /// YGAAUTON XX  
        /// </summary>
        [HisFieldInfoMapping(35, 2)]
        public string YGAAUTON { get; set; }

        // * - 233
        /// <summary>
        /// YGACOULT 9  
        /// </summary>
        [HisFieldInfoMapping(36, 1, CobolType = CobolType.Unsigned)]
        public short YGACOULT { get; set; }

        /// <summary>
        /// YGACODCD 9  
        /// </summary>
        [HisFieldInfoMapping(37, 1, CobolType = CobolType.Unsigned)]
        public short YGACODCD { get; set; }

        /// <summary>
        /// YGAIVSCD 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(38, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGAIVSCD { get; set; }

        /// <summary>
        /// YGARCDA 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(39, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGARCDA { get; set; }

        /// <summary>
        /// YGAACDA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(40, 4, CobolType = CobolType.Unsigned)]
        public short YGAACDA { get; set; }

        /// <summary>
        /// YGARCDB 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(41, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGARCDB { get; set; }

        /// <summary>
        /// YGAACDB 9(4)  
        /// </summary>
        [HisFieldInfoMapping(42, 4, CobolType = CobolType.Unsigned)]
        public short YGAACDB { get; set; }

        /// <summary>
        /// YGAATCD 9(4)  
        /// </summary>
        [HisFieldInfoMapping(43, 4, CobolType = CobolType.Unsigned)]
        public short YGAATCD { get; set; }

        /// <summary>
        /// YGACODAR 9  
        /// </summary>
        [HisFieldInfoMapping(44, 1, CobolType = CobolType.Unsigned)]
        public short YGACODAR { get; set; }

        /// <summary>
        /// YGAIVSAR 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(45, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGAIVSAR { get; set; }

        /// <summary>
        /// YGARARTA 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(46, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGARARTA { get; set; }

        /// <summary>
        /// YGAAARTA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(47, 4, CobolType = CobolType.Unsigned)]
        public short YGAAARTA { get; set; }

        /// <summary>
        /// YGARARTB 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(48, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGARARTB { get; set; }

        /// <summary>
        /// YGAAARTB 9(4)  
        /// </summary>
        [HisFieldInfoMapping(49, 4, CobolType = CobolType.Unsigned)]
        public short YGAAARTB { get; set; }

        /// <summary>
        /// YGAATART 9(4)  
        /// </summary>
        [HisFieldInfoMapping(50, 4, CobolType = CobolType.Unsigned)]
        public short YGAATART { get; set; }

        /// <summary>
        /// YGACODCO 9  
        /// </summary>
        [HisFieldInfoMapping(51, 1, CobolType = CobolType.Unsigned)]
        public short YGACODCO { get; set; }

        /// <summary>
        /// YGAIVSCO 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(52, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGAIVSCO { get; set; }

        /// <summary>
        /// YGARCOMA 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(53, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGARCOMA { get; set; }

        /// <summary>
        /// YGAACOMA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(54, 4, CobolType = CobolType.Unsigned)]
        public short YGAACOMA { get; set; }

        /// <summary>
        /// YGARCOMB 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(55, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGARCOMB { get; set; }

        /// <summary>
        /// YGAACOMB 9(4)  
        /// </summary>
        [HisFieldInfoMapping(56, 4, CobolType = CobolType.Unsigned)]
        public short YGAACOMB { get; set; }

        /// <summary>
        /// YGAATCOM 9(4)  
        /// </summary>
        [HisFieldInfoMapping(57, 4, CobolType = CobolType.Unsigned)]
        public short YGAATCOM { get; set; }

        /// <summary>
        /// YGAMONTA 9(7)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(58, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGAMONTA { get; set; }

        /// <summary>
        /// YGAESCLU 9(7)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(59, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGAESCLU { get; set; }

        /// <summary>
        /// YGASETTE 9(4)  
        /// </summary>
        [HisFieldInfoMapping(60, 4, CobolType = CobolType.Unsigned)]
        public short YGASETTE { get; set; }

        /// <summary>
        /// YGAMONTA2012 9(7)V9999
        /// <summary>
        [HisFieldInfoMapping(61, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGAMONTA2012 { get; set; }

        /// <summary>
        /// YGASETT2012 9(4)
        /// <summary>
        [HisFieldInfoMapping(62, 4, CobolType = CobolType.Unsigned)]
        public short YGASETT2012 { get; set; }

        /// <summary>
        /// YGAFLAG214 X
        /// <summary>
        [HisFieldInfoMapping(63, 1)]
        public string YGAFLAG214 { get; set; }

        /// <summary>
        /// YGAPERC214 99V99
        /// <summary>
        [HisFieldInfoMapping(64, 4, Scale = 2, CobolType = CobolType.Unsigned)]
        public decimal YGAPERC214 { get; set; }

        /// <summary>
        ///YGAMONTAE2012 9(7)V9999
        /// </summary>
        [HisFieldInfoMapping(65, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YGAMONTAE2012 { get; set; }

        /// <summary>
        /// YGASETA-707 9(4)
        /// </summary>
        [HisFieldInfoMapping(66, 4, CobolType = CobolType.Unsigned)]
        public short YGASETA_707 { get; set; }

        /// <summary>
        /// YGASETB-707 9(4)
        /// </summary>
        [HisFieldInfoMapping(67, 4, CobolType = CobolType.Unsigned)]
        public short YGASETB_707 { get; set; }

        /// <summary>
        /// YGASETAES-707 9(4)
        /// </summary>
        [HisFieldInfoMapping(68, 4, CobolType = CobolType.Unsigned)]
        public short YGASETAES_707 { get; set; }

        /// <summary>
        /// YGASETBES-707 9(4)
        /// </summary>
        [HisFieldInfoMapping(69, 4, CobolType = CobolType.Unsigned)]
        public short YGASETBES_707 { get; set; }

        /// <summary>
        /// YGACALC707 X(01)
        /// <summary>
        [HisFieldInfoMapping(70, 1)]
        public string YGACALC707 { get; set; }

        /// <summary>
        /// YGAPROGR 99  
        /// </summary>
        [HisFieldInfoMapping(71, 2, CobolType = CobolType.Unsigned)]
        public short YGAPROGR { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "GAS"; }
        }
        #endregion Properties
    }
}
