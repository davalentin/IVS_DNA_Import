using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo
{
    public class ET : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  XET-RECFOND-BIS REDEFINES COMUNE-RECFOND.
        //          02  XET-RECFOND.
        //              03 XETTIPOR                      PIC X.
        //              03 XETFONDO                      PIC X(3).
        //              03 XETTPENS                      PIC 9.
        //              03 XETNATUR.
        //                 04 XETNATU1                   PIC 9.
        //                 04 XETNATU2                   PIC X.
        //                 04 XETNATU3                   PIC X.
        //              03 XETDECOR.
        //                 04 XETDECAA                   PIC 9999.                
        //                 04 XETDECMM                   PIC 99.
        //              03 XETSOSPE.
        //                 04 XETSOSAA                   PIC 9999.                
        //                 04 XETSOSMM                   PIC 99.
        //              03 XETPVERS.
        //                   04 XETPVRAA                   PIC 9999.              
        //                   04 XETPVRMM                   PIC 99.
        //                   04 XETPVRGG                   PIC 99.
        //              03 XETUVERS.
        //                   04 XETUVRAA                   PIC 9999.              
        //                   04 XETUVRMM                   PIC 99.
        //                   04 XETUVRGG                   PIC 99.
        //              03 XETUTIAA                        PIC 99.
        //              03 XETUTIMM                        PIC 99.
        //              03 XETUTIGG                        PIC 99.
        //              03 XETPTCOD                        PIC 9.
        //              03 XETRETPN                        PIC 9(6)V9999.
        //              03 XETNOCAL                        PIC 9.
        //              03 XETFISSE                        PIC 9.
        //              03 XETEXCBT                        PIC XX.
        //              03 XETNO336                        PIC 9(6)V9999.
        //D2000         03 XETDTESO.
        //D2NEW            04 XETAAESO                     PIC 9999.              
        //                 04 XETMMESO                     PIC 99.
        //                 04 XETGGESO                     PIC 99.
        //              03 XETINTER.
        //                 04 XETINTAA                     PIC 99.
        //                 04 XETINTMM                     PIC 99.
        //                 04 XETINTGG                     PIC 99.
        //D2000         03 XETTEODC.
        //D2NEW            04 XETTEOAA                     PIC 9999.              
        //                 04 XETTEOMM                     PIC 99.
        //              03 XETSLEVA                        PIC 9(3).
        //              03 XETSRICH                        PIC 9(3).
        //              03 XETADPER                        PIC 9(4)V9999.
        //              03 XETAG402                        PIC 9(4)V9999.
        //              03 XETAG140                        PIC 9(4)V9999.
        //              03 XETCODAZ                        PIC X.
        //              03 XETNUMAZ                        PIC 9(5).
        //              03 XETPGTAB                        PIC 9(6)V9999.
        //              03 XETMES13                        PIC 9(4)V9999.
        //              03 XETMES14                        PIC 9(4)V9999.
        //              03 XETACCES                        PIC 9(6)V9999.
        //              03 XETCOM40                        PIC 9(6)V9999.
        //              03 XETGRADO                        PIC 9(3).
        //              03 XETINAIL                        PIC 9(6)V9999.
        //              03 XETEFFET                        PIC 9(6)V9999.
        //              03 XETMILIT                        PIC 9.
        //              03 XETVECCH                        PIC 9.
        //              03 XETDONNA                        PIC 9.
        //              03 XETCDCIE                        PIC 9.
        //              03 XETCODES                        PIC 9.
        //              03 XETRETES                        PIC 9(6)V9999.
        //              03 XETSPECI                        PIC X.
        //      *-L.503
        //              03 XETREQU1                        PIC X.
        //              03 XETREQU2                        PIC 9.
        //              03 XETN2336                        PIC 9(6)V9999.
        //              03 XETUT2AA                        PIC 99.
        //              03 XETUT2MM                        PIC 99.
        //              03 XETUT2GG                        PIC 99.
        //              03 XETRE2PN                        PIC 9(6)V9999.
        //              03 XETUT3AA                        PIC 99.
        //              03 XETUT3MM                        PIC 99.
        //              03 XETUT3GG                        PIC 99.
        //              03 XETCATEG                        PIC 9(3).
        //              03 XETSEDE                         PIC 9(4).
        //              03 XETCERTI                        PIC 9(8).
        //              03 XETPROGR                        PIC 99.
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  XET-RECFOND-BIS REDEFINES COMUNE-RECFOND.
        // 02  XET-RECFOND.
        /// <summary>
        /// XETTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string XETTIPOR { get; set; }

        /// <summary>
        /// XETFONDO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string XETFONDO { get; set; }

        /// <summary>
        /// XETTPENS 9  
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short XETTPENS { get; set; }

        // 03 XETNATUR.
        /// <summary>
        /// XETNATU1 9  
        /// </summary>
        [HisFieldInfoMapping(3, 1, CobolType = CobolType.Unsigned)]
        public short XETNATU1 { get; set; }

        /// <summary>
        /// XETNATU2 X  
        /// </summary>
        [HisFieldInfoMapping(4, 1)]
        public string XETNATU2 { get; set; }

        /// <summary>
        /// XETNATU3 X  
        /// </summary>
        [HisFieldInfoMapping(5, 1)]
        public string XETNATU3 { get; set; }

        // 03 XETDECOR.
        /// <summary>
        /// XETDECAA 9999  
        /// </summary>
        [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
        public short XETDECAA { get; set; }

        /// <summary>
        /// XETDECMM 99  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short XETDECMM { get; set; }

        // 03 XETSOSPE.
        /// <summary>
        /// XETSOSAA 9999  
        /// </summary>
        [HisFieldInfoMapping(8, 4, CobolType = CobolType.Unsigned)]
        public short XETSOSAA { get; set; }

        /// <summary>
        /// XETSOSMM 99  
        /// </summary>
        [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
        public short XETSOSMM { get; set; }

        // 03 XETPVERS.
        /// <summary>
        /// XETPVRAA 9999  
        /// </summary>
        [HisFieldInfoMapping(10, 4, CobolType = CobolType.Unsigned)]
        public short XETPVRAA { get; set; }

        /// <summary>
        /// XETPVRMM 99  
        /// </summary>
        [HisFieldInfoMapping(11, 2, CobolType = CobolType.Unsigned)]
        public short XETPVRMM { get; set; }

        /// <summary>
        /// XETPVRGG 99  
        /// </summary>
        [HisFieldInfoMapping(12, 2, CobolType = CobolType.Unsigned)]
        public short XETPVRGG { get; set; }

        // 03 XETUVERS.
        /// <summary>
        /// XETUVRAA 9999  
        /// </summary>
        [HisFieldInfoMapping(13, 4, CobolType = CobolType.Unsigned)]
        public short XETUVRAA { get; set; }

        /// <summary>
        /// XETUVRMM 99  
        /// </summary>
        [HisFieldInfoMapping(14, 2, CobolType = CobolType.Unsigned)]
        public short XETUVRMM { get; set; }

        /// <summary>
        /// XETUVRGG 99  
        /// </summary>
        [HisFieldInfoMapping(15, 2, CobolType = CobolType.Unsigned)]
        public short XETUVRGG { get; set; }

        /// <summary>
        /// XETUTIAA 99  
        /// </summary>
        [HisFieldInfoMapping(16, 2, CobolType = CobolType.Unsigned)]
        public short XETUTIAA { get; set; }

        /// <summary>
        /// XETUTIMM 99  
        /// </summary>
        [HisFieldInfoMapping(17, 2, CobolType = CobolType.Unsigned)]
        public short XETUTIMM { get; set; }

        /// <summary>
        /// XETUTIGG 99  
        /// </summary>
        [HisFieldInfoMapping(18, 2, CobolType = CobolType.Unsigned)]
        public short XETUTIGG { get; set; }

        /// <summary>
        /// XETPTCOD 9  
        /// </summary>
        [HisFieldInfoMapping(19, 1, CobolType = CobolType.Unsigned)]
        public short XETPTCOD { get; set; }

        /// <summary>
        /// XETRETPN 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(20, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XETRETPN { get; set; }

        /// <summary>
        /// XETNOCAL 9  
        /// </summary>
        [HisFieldInfoMapping(21, 1, CobolType = CobolType.Unsigned)]
        public short XETNOCAL { get; set; }

        /// <summary>
        /// XETFISSE 9  
        /// </summary>
        [HisFieldInfoMapping(22, 1, CobolType = CobolType.Unsigned)]
        public short XETFISSE { get; set; }

        /// <summary>
        /// XETEXCBT XX  
        /// </summary>
        [HisFieldInfoMapping(23, 2)]
        public string XETEXCBT { get; set; }

        /// <summary>
        /// XETNO336 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(24, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XETNO336 { get; set; }

        // D2000         03 XETDTESO.
        /// <summary>
        /// XETAAESO 9999  
        /// </summary>
        [HisFieldInfoMapping(25, 4, CobolType = CobolType.Unsigned)]
        public short XETAAESO { get; set; }

        /// <summary>
        /// XETMMESO 99  
        /// </summary>
        [HisFieldInfoMapping(26, 2, CobolType = CobolType.Unsigned)]
        public short XETMMESO { get; set; }

        /// <summary>
        /// XETGGESO 99  
        /// </summary>
        [HisFieldInfoMapping(27, 2, CobolType = CobolType.Unsigned)]
        public short XETGGESO { get; set; }

        // 03 XETINTER.
        /// <summary>
        /// XETINTAA 99  
        /// </summary>
        [HisFieldInfoMapping(28, 2, CobolType = CobolType.Unsigned)]
        public short XETINTAA { get; set; }

        /// <summary>
        /// XETINTMM 99  
        /// </summary>
        [HisFieldInfoMapping(29, 2, CobolType = CobolType.Unsigned)]
        public short XETINTMM { get; set; }

        /// <summary>
        /// XETINTGG 99  
        /// </summary>
        [HisFieldInfoMapping(30, 2, CobolType = CobolType.Unsigned)]
        public short XETINTGG { get; set; }

        // D2000         03 XETTEODC.
        /// <summary>
        /// XETTEOAA 9999  
        /// </summary>
        [HisFieldInfoMapping(31, 4, CobolType = CobolType.Unsigned)]
        public short XETTEOAA { get; set; }

        /// <summary>
        /// XETTEOMM 99  
        /// </summary>
        [HisFieldInfoMapping(32, 2, CobolType = CobolType.Unsigned)]
        public short XETTEOMM { get; set; }

        /// <summary>
        /// XETSLEVA 9(3)  
        /// </summary>
        [HisFieldInfoMapping(33, 3, CobolType = CobolType.Unsigned)]
        public short XETSLEVA { get; set; }

        /// <summary>
        /// XETSRICH 9(3)  
        /// </summary>
        [HisFieldInfoMapping(34, 3, CobolType = CobolType.Unsigned)]
        public short XETSRICH { get; set; }

        /// <summary>
        /// XETADPER 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(35, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XETADPER { get; set; }

        /// <summary>
        /// XETAG402 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(36, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XETAG402 { get; set; }

        /// <summary>
        /// XETAG140 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(37, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XETAG140 { get; set; }

        /// <summary>
        /// XETCODAZ X  
        /// </summary>
        [HisFieldInfoMapping(38, 1)]
        public string XETCODAZ { get; set; }

        /// <summary>
        /// XETNUMAZ 9(5)  
        /// </summary>
        [HisFieldInfoMapping(39, 5, CobolType = CobolType.Unsigned)]
        public int XETNUMAZ { get; set; }

        /// <summary>
        /// XETPGTAB 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(40, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XETPGTAB { get; set; }

        /// <summary>
        /// XETMES13 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(41, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XETMES13 { get; set; }

        /// <summary>
        /// XETMES14 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(42, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XETMES14 { get; set; }

        /// <summary>
        /// XETACCES 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(43, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XETACCES { get; set; }

        /// <summary>
        /// XETCOM40 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(44, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XETCOM40 { get; set; }

        /// <summary>
        /// XETGRADO 9(3)  
        /// </summary>
        [HisFieldInfoMapping(45, 3, CobolType = CobolType.Unsigned)]
        public short XETGRADO { get; set; }

        /// <summary>
        /// XETINAIL 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(46, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XETINAIL { get; set; }

        /// <summary>
        /// XETEFFET 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(47, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XETEFFET { get; set; }

        /// <summary>
        /// XETMILIT 9  
        /// </summary>
        [HisFieldInfoMapping(48, 1, CobolType = CobolType.Unsigned)]
        public short XETMILIT { get; set; }

        /// <summary>
        /// XETVECCH 9  
        /// </summary>
        [HisFieldInfoMapping(49, 1, CobolType = CobolType.Unsigned)]
        public short XETVECCH { get; set; }

        /// <summary>
        /// XETDONNA 9  
        /// </summary>
        [HisFieldInfoMapping(50, 1, CobolType = CobolType.Unsigned)]
        public short XETDONNA { get; set; }

        /// <summary>
        /// XETCDCIE 9  
        /// </summary>
        [HisFieldInfoMapping(51, 1, CobolType = CobolType.Unsigned)]
        public short XETCDCIE { get; set; }

        /// <summary>
        /// XETCODES 9  
        /// </summary>
        [HisFieldInfoMapping(52, 1, CobolType = CobolType.Unsigned)]
        public short XETCODES { get; set; }

        /// <summary>
        /// XETRETES 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(53, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XETRETES { get; set; }

        /// <summary>
        /// XETSPECI X  
        /// </summary>
        [HisFieldInfoMapping(54, 1)]
        public string XETSPECI { get; set; }

        // *-L.503
        /// <summary>
        /// XETREQU1 X  
        /// </summary>
        [HisFieldInfoMapping(55, 1)]
        public string XETREQU1 { get; set; }

        /// <summary>
        /// XETREQU2 9  
        /// </summary>
        [HisFieldInfoMapping(56, 1, CobolType = CobolType.Unsigned)]
        public short XETREQU2 { get; set; }

        /// <summary>
        /// XETN2336 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(57, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XETN2336 { get; set; }

        /// <summary>
        /// XETUT2AA 99  
        /// </summary>
        [HisFieldInfoMapping(58, 2, CobolType = CobolType.Unsigned)]
        public short XETUT2AA { get; set; }

        /// <summary>
        /// XETUT2MM 99  
        /// </summary>
        [HisFieldInfoMapping(59, 2, CobolType = CobolType.Unsigned)]
        public short XETUT2MM { get; set; }

        /// <summary>
        /// XETUT2GG 99  
        /// </summary>
        [HisFieldInfoMapping(60, 2, CobolType = CobolType.Unsigned)]
        public short XETUT2GG { get; set; }

        /// <summary>
        /// XETRE2PN 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(61, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XETRE2PN { get; set; }

        /// <summary>
        /// XETUT3AA 99  
        /// </summary>
        [HisFieldInfoMapping(62, 2, CobolType = CobolType.Unsigned)]
        public short XETUT3AA { get; set; }

        /// <summary>
        /// XETUT3MM 99  
        /// </summary>
        [HisFieldInfoMapping(63, 2, CobolType = CobolType.Unsigned)]
        public short XETUT3MM { get; set; }

        /// <summary>
        /// XETUT3GG 99  
        /// </summary>
        [HisFieldInfoMapping(64, 2, CobolType = CobolType.Unsigned)]
        public short XETUT3GG { get; set; }

        /// <summary>
        /// XETCATEG 9(3)  
        /// </summary>
        [HisFieldInfoMapping(65, 3, CobolType = CobolType.Unsigned)]
        public short XETCATEG { get; set; }

        /// <summary>
        /// XETSEDE 9(4)  
        /// </summary>
        [HisFieldInfoMapping(66, 4, CobolType = CobolType.Unsigned)]
        public short XETSEDE { get; set; }

        /// <summary>
        /// XETCERTI 9(8)  
        /// </summary>
        [HisFieldInfoMapping(67, 8, CobolType = CobolType.Unsigned)]
        public int XETCERTI { get; set; }

        /// <summary>
        /// XETPROGR 99  
        /// </summary>
        [HisFieldInfoMapping(68, 2, CobolType = CobolType.Unsigned)]
        public short XETPROGR { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "ET"; }
        }
        #endregion Properties
    }
}
