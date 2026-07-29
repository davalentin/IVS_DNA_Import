using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago
{
    public class ES : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  YES-RECAGO-BIS REDEFINES COMUNE-RECAGO.
        //          02  YES-RECAGO.
        //              03 YESTIPOR                      PIC X.
        //              03 YESFONDO                      PIC X(3).
        //              03 YESTPENS                      PIC 9.
        //              03 YESTPLIQ                      PIC 9.
        //D2NEW         03 YESDECAA                      PIC 9999.                
        //              03 YESDECMM                      PIC 99.
        //D2NEW         03 YESSOSAA                      PIC 9999.                
        //              03 YESSOSMM                      PIC 99.
        //D2NEW         03 YESTEOAA                      PIC 9999.                
        //              03 YESTEOMM                      PIC 99.
        //              03 YESRETPN                      PIC 9(5)V9999.
        //              03 YESANZTO                      PIC 9(5).
        //              03 YESSAR24                      PIC 9(5).
        //              03 YESSAR57                      PIC 9(5).
        //              03 YESVOLON                      PIC 9(5).
        //              03 YESART11                      PIC 9(3)V9999.
        //              03 YESTOTRT                      PIC 9(5)V9999.
        //              03 YESCTR24                      PIC 9(4)V9999.
        //              03 YESCTR57                      PIC 9(3)V9999.
        //              03 YESSUP14                      PIC 9(3)V9999.
        //              03 YESBALTR                      PIC 9(3)V99999.
        //              03 YESCALTR                      PIC X(3).
        //              03 YESDIFFQ                      PIC 9(4)V9999.
        //              03 YESDIFFA                      PIC 9(3).
        //              03 YESMATUR                      PIC 99.
        //              03 YESZANZI                      PIC 9(5).
        //              03 YESZST24                      PIC 9(5).
        //              03 YESZST57                      PIC 9(5).
        //              03 YESZRETS                      PIC 9(6)V9999.
        //              03 YESZTOTC                      PIC 9(5)V9999.
        //              03 YESZA14C                      PIC 9(4)V9999.
        //              03 YESZSPAG                      PIC 9(4)V9999.
        //              03 YESZSPFO                      PIC 9(4)V9999.
        //D2000         03 YESDPCDC.
        //D2NEW            04 YESCDCAA                   PIC 9999.                
        //                 04 YESCDCMM                   PIC 99.
        //              03 YESDPCRT                      PIC 9(5)V9999.
        //              03 YESS72RT                      PIC 9(5)V9999.
        //D2NEW         03 YESCB140                      PIC 9(6).                
        //              03 YESNONVE                      PIC 9.
        //      *-LEGGE 503
        //              03 YESANZT2                      PIC 9(4).
        //              03 YESSA224                      PIC 9(4).
        //              03 YESRE2PN                      PIC 9(5)V9999.
        //              03 YESZRET2                      PIC 9(6)V9999.
        //              03 YESREQU1                      PIC X.
        //              03 YESREQU2                      PIC 9.
        //              03 YESSPECI                      PIC X.
        //              03 YESDECSS                      PIC 99.
        //              03 YESSOSSS                      PIC 99.
        //              03 YESAUTON                      PIC XX.
        //      * - 233
        //              03 YESCOULT                      PIC 9.
        //              03 YESCODCD                      PIC 9.
        //              03 YESIVSCD                      PIC 9(5)V9999.
        //              03 YESRCDA                       PIC 9(6)V9999.
        //              03 YESACDA                       PIC 9(4).
        //              03 YESRCDB                       PIC 9(6)V9999.
        //              03 YESACDB                       PIC 9(4).
        //              03 YESATCD                       PIC 9(4).
        //              03 YESCODAR                      PIC 9.
        //              03 YESIVSAR                      PIC 9(5)V9999.
        //              03 YESRARTA                      PIC 9(6)V9999.
        //              03 YESAARTA                      PIC 9(4).
        //              03 YESRARTB                      PIC 9(6)V9999.
        //              03 YESAARTB                      PIC 9(4).
        //              03 YESATART                      PIC 9(4).
        //              03 YESCODCO                      PIC 9.
        //              03 YESIVSCO                      PIC 9(5)V9999.
        //              03 YESRCOMA                      PIC 9(6)V9999.
        //              03 YESACOMA                      PIC 9(4).
        //              03 YESRCOMB                      PIC 9(6)V9999.
        //              03 YESACOMB                      PIC 9(4).
        //              03 YESATCOM                      PIC 9(4).
        //              03 YESMONTA                      PIC 9(7)V9999.
        //              03 YESESCLU                      PIC 9(7)V9999.
        //GD1109        03 YESSETTE                      PIC 9(4).
        //GD0212        03 YESMONTA2012                  PIC 9(5)V9(2) COMP-3.
        //GD1012        03 YESIMPCRT                     PIC 9(5) COMP-3.      
        //GD0212        03 YESSETT2012                   PIC 9(3) COMP-3.   
        //GD1012        03 YESFLAG214                    PIC X.
        //GD1012        03 YESPERC214                    PIC 99V99.
        //              03 YESPROGR                      PIC 99.
        //GD1012*     02 YESDISPO                           PIC X(09).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  YES-RECAGO-BIS REDEFINES COMUNE-RECAGO.
        // 02  YES-RECAGO.
        /// <summary>
        /// YESTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string YESTIPOR { get; set; }

        /// <summary>
        /// YESFONDO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string YESFONDO { get; set; }

        /// <summary>
        /// YESTPENS 9  
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short YESTPENS { get; set; }

        /// <summary>
        /// YESTPLIQ 9  
        /// </summary>
        [HisFieldInfoMapping(3, 1, CobolType = CobolType.Unsigned)]
        public short YESTPLIQ { get; set; }

        /// <summary>
        /// YESDECAA 9999  
        /// </summary>
        [HisFieldInfoMapping(4, 4, CobolType = CobolType.Unsigned)]
        public short YESDECAA { get; set; }

        /// <summary>
        /// YESDECMM 99  
        /// </summary>
        [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
        public short YESDECMM { get; set; }

        /// <summary>
        /// YESSOSAA 9999  
        /// </summary>
        [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
        public short YESSOSAA { get; set; }

        /// <summary>
        /// YESSOSMM 99  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short YESSOSMM { get; set; }

        /// <summary>
        /// YESTEOAA 9999  
        /// </summary>
        [HisFieldInfoMapping(8, 4, CobolType = CobolType.Unsigned)]
        public short YESTEOAA { get; set; }

        /// <summary>
        /// YESTEOMM 99  
        /// </summary>
        [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
        public short YESTEOMM { get; set; }

        /// <summary>
        /// YESRETPN 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(10, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESRETPN { get; set; }

        /// <summary>
        /// YESANZTO 9(5)  
        /// </summary>
        [HisFieldInfoMapping(11, 5, CobolType = CobolType.Unsigned)]
        public int YESANZTO { get; set; }

        /// <summary>
        /// YESSAR24 9(5)  
        /// </summary>
        [HisFieldInfoMapping(12, 5, CobolType = CobolType.Unsigned)]
        public int YESSAR24 { get; set; }

        /// <summary>
        /// YESSAR57 9(5)  
        /// </summary>
        [HisFieldInfoMapping(13, 5, CobolType = CobolType.Unsigned)]
        public int YESSAR57 { get; set; }

        /// <summary>
        /// YESVOLON 9(5)  
        /// </summary>
        [HisFieldInfoMapping(14, 5, CobolType = CobolType.Unsigned)]
        public int YESVOLON { get; set; }

        /// <summary>
        /// YESART11 9(3)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(15, 7, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESART11 { get; set; }

        /// <summary>
        /// YESTOTRT 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(16, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESTOTRT { get; set; }

        /// <summary>
        /// YESCTR24 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(17, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESCTR24 { get; set; }

        /// <summary>
        /// YESCTR57 9(3)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(18, 7, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESCTR57 { get; set; }

        /// <summary>
        /// YESSUP14 9(3)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(19, 7, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESSUP14 { get; set; }

        /// <summary>
        /// YESBALTR 9(3)V9(4)9  
        /// </summary>
        [HisFieldInfoMapping(20, 8, Scale = 5, CobolType = CobolType.Unsigned)]
        public decimal YESBALTR { get; set; }

        /// <summary>
        /// YESCALTR X(3)  
        /// </summary>
        [HisFieldInfoMapping(21, 3)]
        public string YESCALTR { get; set; }

        /// <summary>
        /// YESDIFFQ 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(22, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESDIFFQ { get; set; }

        /// <summary>
        /// YESDIFFA 9(3)  
        /// </summary>
        [HisFieldInfoMapping(23, 3, CobolType = CobolType.Unsigned)]
        public short YESDIFFA { get; set; }

        /// <summary>
        /// YESMATUR 99  
        /// </summary>
        [HisFieldInfoMapping(24, 2, CobolType = CobolType.Unsigned)]
        public short YESMATUR { get; set; }

        /// <summary>
        /// YESZANZI 9(5)  
        /// </summary>
        [HisFieldInfoMapping(25, 5, CobolType = CobolType.Unsigned)]
        public int YESZANZI { get; set; }

        /// <summary>
        /// YESZST24 9(5)  
        /// </summary>
        [HisFieldInfoMapping(26, 5, CobolType = CobolType.Unsigned)]
        public int YESZST24 { get; set; }

        /// <summary>
        /// YESZST57 9(5)  
        /// </summary>
        [HisFieldInfoMapping(27, 5, CobolType = CobolType.Unsigned)]
        public int YESZST57 { get; set; }

        /// <summary>
        /// YESZRETS 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(28, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESZRETS { get; set; }

        /// <summary>
        /// YESZTOTC 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(29, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESZTOTC { get; set; }

        /// <summary>
        /// YESZA14C 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(30, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESZA14C { get; set; }

        /// <summary>
        /// YESZSPAG 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(31, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESZSPAG { get; set; }

        /// <summary>
        /// YESZSPFO 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(32, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESZSPFO { get; set; }

        // D2000         03 YESDPCDC.
        /// <summary>
        /// YESCDCAA 9999  
        /// </summary>
        [HisFieldInfoMapping(33, 4, CobolType = CobolType.Unsigned)]
        public short YESCDCAA { get; set; }

        /// <summary>
        /// YESCDCMM 99  
        /// </summary>
        [HisFieldInfoMapping(34, 2, CobolType = CobolType.Unsigned)]
        public short YESCDCMM { get; set; }

        /// <summary>
        /// YESDPCRT 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(35, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESDPCRT { get; set; }

        /// <summary>
        /// YESS72RT 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(36, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESS72RT { get; set; }

        /// <summary>
        /// YESCB140 9(6)  
        /// </summary>
        [HisFieldInfoMapping(37, 6, CobolType = CobolType.Unsigned)]
        public int YESCB140 { get; set; }

        /// <summary>
        /// YESNONVE 9  
        /// </summary>
        [HisFieldInfoMapping(38, 1, CobolType = CobolType.Unsigned)]
        public short YESNONVE { get; set; }

        // *-LEGGE 503
        /// <summary>
        /// YESANZT2 9(4)  
        /// </summary>
        [HisFieldInfoMapping(39, 4, CobolType = CobolType.Unsigned)]
        public short YESANZT2 { get; set; }

        /// <summary>
        /// YESSA224 9(4)  
        /// </summary>
        [HisFieldInfoMapping(40, 4, CobolType = CobolType.Unsigned)]
        public short YESSA224 { get; set; }

        /// <summary>
        /// YESRE2PN 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(41, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESRE2PN { get; set; }

        /// <summary>
        /// YESZRET2 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(42, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESZRET2 { get; set; }

        /// <summary>
        /// YESREQU1 X  
        /// </summary>
        [HisFieldInfoMapping(43, 1)]
        public string YESREQU1 { get; set; }

        /// <summary>
        /// YESREQU2 9  
        /// </summary>
        [HisFieldInfoMapping(44, 1, CobolType = CobolType.Unsigned)]
        public short YESREQU2 { get; set; }

        /// <summary>
        /// YESSPECI X  
        /// </summary>
        [HisFieldInfoMapping(45, 1)]
        public string YESSPECI { get; set; }

        /// <summary>
        /// YESDECSS 99  
        /// </summary>
        [HisFieldInfoMapping(46, 2, CobolType = CobolType.Unsigned)]
        public short YESDECSS { get; set; }

        /// <summary>
        /// YESSOSSS 99  
        /// </summary>
        [HisFieldInfoMapping(47, 2, CobolType = CobolType.Unsigned)]
        public short YESSOSSS { get; set; }

        /// <summary>
        /// YESAUTON XX  
        /// </summary>
        [HisFieldInfoMapping(48, 2)]
        public string YESAUTON { get; set; }

        // * - 233
        /// <summary>
        /// YESCOULT 9  
        /// </summary>
        [HisFieldInfoMapping(49, 1, CobolType = CobolType.Unsigned)]
        public short YESCOULT { get; set; }

        /// <summary>
        /// YESCODCD 9  
        /// </summary>
        [HisFieldInfoMapping(50, 1, CobolType = CobolType.Unsigned)]
        public short YESCODCD { get; set; }

        /// <summary>
        /// YESIVSCD 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(51, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESIVSCD { get; set; }

        /// <summary>
        /// YESRCDA 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(52, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESRCDA { get; set; }

        /// <summary>
        /// YESACDA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(53, 4, CobolType = CobolType.Unsigned)]
        public short YESACDA { get; set; }

        /// <summary>
        /// YESRCDB 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(54, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESRCDB { get; set; }

        /// <summary>
        /// YESACDB 9(4)  
        /// </summary>
        [HisFieldInfoMapping(55, 4, CobolType = CobolType.Unsigned)]
        public short YESACDB { get; set; }

        /// <summary>
        /// YESATCD 9(4)  
        /// </summary>
        [HisFieldInfoMapping(56, 4, CobolType = CobolType.Unsigned)]
        public short YESATCD { get; set; }

        /// <summary>
        /// YESCODAR 9  
        /// </summary>
        [HisFieldInfoMapping(57, 1, CobolType = CobolType.Unsigned)]
        public short YESCODAR { get; set; }

        /// <summary>
        /// YESIVSAR 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(58, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESIVSAR { get; set; }

        /// <summary>
        /// YESRARTA 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(59, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESRARTA { get; set; }

        /// <summary>
        /// YESAARTA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(60, 4, CobolType = CobolType.Unsigned)]
        public short YESAARTA { get; set; }

        /// <summary>
        /// YESRARTB 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(61, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESRARTB { get; set; }

        /// <summary>
        /// YESAARTB 9(4)  
        /// </summary>
        [HisFieldInfoMapping(62, 4, CobolType = CobolType.Unsigned)]
        public short YESAARTB { get; set; }

        /// <summary>
        /// YESATART 9(4)  
        /// </summary>
        [HisFieldInfoMapping(63, 4, CobolType = CobolType.Unsigned)]
        public short YESATART { get; set; }

        /// <summary>
        /// YESCODCO 9  
        /// </summary>
        [HisFieldInfoMapping(64, 1, CobolType = CobolType.Unsigned)]
        public short YESCODCO { get; set; }

        /// <summary>
        /// YESIVSCO 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(65, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESIVSCO { get; set; }

        /// <summary>
        /// YESRCOMA 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(66, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESRCOMA { get; set; }

        /// <summary>
        /// YESACOMA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(67, 4, CobolType = CobolType.Unsigned)]
        public short YESACOMA { get; set; }

        /// <summary>
        /// YESRCOMB 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(68, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESRCOMB { get; set; }

        /// <summary>
        /// YESACOMB 9(4)  
        /// </summary>
        [HisFieldInfoMapping(69, 4, CobolType = CobolType.Unsigned)]
        public short YESACOMB { get; set; }

        /// <summary>
        /// YESATCOM 9(4)  
        /// </summary>
        [HisFieldInfoMapping(70, 4, CobolType = CobolType.Unsigned)]
        public short YESATCOM { get; set; }

        /// <summary>
        /// YESMONTA 9(7)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(71, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESMONTA { get; set; }

        /// <summary>
        /// YESESCLU 9(7)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(72, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YESESCLU { get; set; }

        /// <summary>
        /// YESSETTE 9(4)  
        /// </summary>
        [HisFieldInfoMapping(73, 4, CobolType = CobolType.Unsigned)]
        public short YESSETTE { get; set; }

        /// <summary>
        /// YESMONTA2012 9(7) COMP-3
        /// <summary>
        [HisFieldInfoMapping(74, 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal YESMONTA2012 { get; set; }

        /// <summary>
        /// YESIMPCRT 9(5) COMP-3  
        /// <summary>
        [HisFieldInfoMapping(75, 3, CobolType = CobolType.Comp3Unsigned)]
        public int YESIMPCRT { get; set; }

        /// <summary>
        /// YESSETT2012 9(3) COMP-3
        /// <summary>
        [HisFieldInfoMapping(76, 2, CobolType = CobolType.Comp3Unsigned)]
        public int YESSETT2012 { get; set; }

        /// <summary>
        /// YESFLAG214 X
        /// <summary>
        [HisFieldInfoMapping(77, 1)]
        public string YESFLAG214 { get; set; }
        
        /// <summary>
        /// YESPERC214 99V99
        /// <summary>
        [HisFieldInfoMapping(78, 4, Scale = 2, CobolType = CobolType.Unsigned)]
        public decimal YESPERC214 { get; set; }

        /// <summary>
        /// YESPROGR 99  
        /// </summary>
        [HisFieldInfoMapping(79, 2, CobolType = CobolType.Unsigned)]
        public short YESPROGR { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "ES"; }
        }
        #endregion Properties
    }
}
