using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo
{
    public class TT : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  XTT-RECFOND-BIS REDEFINES COMUNE-RECFOND.
        //    02  XTT-RECFOND.
        //        03 XTTTIPOR                      PIC X.
        //        03 XTTFONDO                      PIC X(3).
        //        03 XTTTPENS                      PIC 9.
        //        03 XTTNATUR.
        //           04 XTTNATU1                   PIC 9.
        //           04 XTTNATU2                   PIC X.
        //           04 XTTNATU3                   PIC X.
        //        03 XTTDECOR.
        //           04 XTTDECAA                   PIC 9999.                
        //           04 XTTDECMM                   PIC 99.
        //        03 XTTSOSPE.
        //           04 XTTSOSAA                   PIC 9999.                
        //           04 XTTSOSMM                   PIC 99.
        //        03 XTTPVERS.
        //             04 XTTPVRAA                   PIC 9999.              
        //             04 XTTPVRMM                   PIC 99.
        //             04 XTTPVRGG                   PIC 99.
        //        03 XTTUVERS.
        //             04 XTTUVRAA                   PIC 9999.              
        //             04 XTTUVRMM                   PIC 99.
        //             04 XTTUVRGG                   PIC 99.
        //        03 XTTUTIAA                        PIC 99.
        //        03 XTTUTIMM                        PIC 99.
        //        03 XTTFISSE                        PIC 9.
        //        03 XTTCONVE                        PIC X.
        //        03 XTTNOCAL                        PIC 9.
        //        03 XTTATTIV                        PIC 99.
        //        03 XTTRISFI                        PIC 9(6).
        //        03 XTTRISMT                        PIC 9(6).
        //        03 XTTRISFG                        PIC 9(6).
        //        03 XTTTEODC.
        //           04 XTTTEOAA                   PIC 9999.                
        //           04 XTTTEOMM                   PIC 99.
        //        03 XTTSPOBG                        PIC 9(2)V9999.
        //        03 XTTDITTA                        PIC XX.
        //        03 XTTUTRAA                        PIC 99.
        //        03 XTTUTRMM                        PIC 99.
        //        03 XTTPEN53                        PIC 9(4)V9999.
        //        03 XTTRTULT                        PIC 9(6)V9999.
        //        03 XTTRTBIE                        PIC 9(6)V9999.
        //        03 XTTACCES                        PIC 9(6)V9999.
        //        03 XTTINARE                        PIC 9(6)V9999.
        //        03 XTTINAEF                        PIC 9(4)V9999.
        //        03 XTTPNGEN                        PIC 9(4)V9999.
        //        03 XTTRTSUP                        PIC 9(6)V9999.
        //        03 XTTNONVE                        PIC 9.
        //        03 XTTSPECI                        PIC X.
        //*-L.503
        //        03 XTTREQU1                        PIC X.
        //        03 XTTREQU2                        PIC 9.
        //        03 XTTLEG58                        PIC X.
        //        03 XTTUT2AA                        PIC 99.
        //        03 XTTUT2MM                        PIC 99.
        //        03 XTTUTR2A                        PIC 99.
        //        03 XTTUTR2M                        PIC 99.
        //        03 XTTRETPN                        PIC 9(6)V9999.
        //        03 XTTUT3AA                        PIC 99.
        //        03 XTTUT3MM                        PIC 99.
        //        03 XTTUTR3A                        PIC 99.
        //        03 XTTUTR3M                        PIC 99.
        //        03 XTTTETTO                        PIC 9(6)V9999.
        //        03 XTTUT4AA                        PIC 99.
        //        03 XTTUT4MM                        PIC 99.
        //        03 XTTUTR4A                        PIC 99.
        //        03 XTTUTR4M                        PIC 99.
        //        03 XTTRETPD                        PIC 9(6)V9999.
        //        03 XTTCATEG                        PIC 9(3).
        //        03 XTTSEDE                         PIC 9(4).
        //        03 XTTCERTI                        PIC 9(8).
        //        03 XTTPROGR                        PIC 99.
        //     02 FILLER                             PIC X(28).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  XTT-RECFOND-BIS REDEFINES COMUNE-RECFOND.
        // 02  XTT-RECFOND.
        /// <summary>
        /// XTTTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string XTTTIPOR { get; set; }

        /// <summary>
        /// XTTFONDO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string XTTFONDO { get; set; }

        /// <summary>
        /// XTTTPENS 9  
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short XTTTPENS { get; set; }

        // 03 XTTNATUR.
        /// <summary>
        /// XTTNATU1 9  
        /// </summary>
        [HisFieldInfoMapping(3, 1, CobolType = CobolType.Unsigned)]
        public short XTTNATU1 { get; set; }

        /// <summary>
        /// XTTNATU2 X  
        /// </summary>
        [HisFieldInfoMapping(4, 1)]
        public string XTTNATU2 { get; set; }

        /// <summary>
        /// XTTNATU3 X  
        /// </summary>
        [HisFieldInfoMapping(5, 1)]
        public string XTTNATU3 { get; set; }

        // 03 XTTDECOR.
        /// <summary>
        /// XTTDECAA 9999  
        /// </summary>
        [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
        public short XTTDECAA { get; set; }

        /// <summary>
        /// XTTDECMM 99  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short XTTDECMM { get; set; }

        // 03 XTTSOSPE.
        /// <summary>
        /// XTTSOSAA 9999  
        /// </summary>
        [HisFieldInfoMapping(8, 4, CobolType = CobolType.Unsigned)]
        public short XTTSOSAA { get; set; }

        /// <summary>
        /// XTTSOSMM 99  
        /// </summary>
        [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
        public short XTTSOSMM { get; set; }

        // 03 XTTPVERS.
        /// <summary>
        /// XTTPVRAA 9999  
        /// </summary>
        [HisFieldInfoMapping(10, 4, CobolType = CobolType.Unsigned)]
        public short XTTPVRAA { get; set; }

        /// <summary>
        /// XTTPVRMM 99  
        /// </summary>
        [HisFieldInfoMapping(11, 2, CobolType = CobolType.Unsigned)]
        public short XTTPVRMM { get; set; }

        /// <summary>
        /// XTTPVRGG 99  
        /// </summary>
        [HisFieldInfoMapping(12, 2, CobolType = CobolType.Unsigned)]
        public short XTTPVRGG { get; set; }

        // 03 XTTUVERS.
        /// <summary>
        /// XTTUVRAA 9999  
        /// </summary>
        [HisFieldInfoMapping(13, 4, CobolType = CobolType.Unsigned)]
        public short XTTUVRAA { get; set; }

        /// <summary>
        /// XTTUVRMM 99  
        /// </summary>
        [HisFieldInfoMapping(14, 2, CobolType = CobolType.Unsigned)]
        public short XTTUVRMM { get; set; }

        /// <summary>
        /// XTTUVRGG 99  
        /// </summary>
        [HisFieldInfoMapping(15, 2, CobolType = CobolType.Unsigned)]
        public short XTTUVRGG { get; set; }

        /// <summary>
        /// XTTUTIAA 99  
        /// </summary>
        [HisFieldInfoMapping(16, 2, CobolType = CobolType.Unsigned)]
        public short XTTUTIAA { get; set; }

        /// <summary>
        /// XTTUTIMM 99  
        /// </summary>
        [HisFieldInfoMapping(17, 2, CobolType = CobolType.Unsigned)]
        public short XTTUTIMM { get; set; }

        /// <summary>
        /// XTTFISSE 9  
        /// </summary>
        [HisFieldInfoMapping(18, 1, CobolType = CobolType.Unsigned)]
        public short XTTFISSE { get; set; }

        /// <summary>
        /// XTTCONVE X  
        /// </summary>
        [HisFieldInfoMapping(19, 1)]
        public string XTTCONVE { get; set; }

        /// <summary>
        /// XTTNOCAL 9  
        /// </summary>
        [HisFieldInfoMapping(20, 1, CobolType = CobolType.Unsigned)]
        public short XTTNOCAL { get; set; }

        /// <summary>
        /// XTTATTIV 99  
        /// </summary>
        [HisFieldInfoMapping(21, 2, CobolType = CobolType.Unsigned)]
        public short XTTATTIV { get; set; }

        /// <summary>
        /// XTTRISFIGG 9(2)  
        /// </summary>
        [HisFieldInfoMapping(22, 2, CobolType = CobolType.Unsigned)]
        public short XTTRISFIGG { get; set; }

        /// <summary>
        /// XTTRISFIMM 9(2)  
        /// </summary>
        [HisFieldInfoMapping(23, 2, CobolType = CobolType.Unsigned)]
        public short XTTRISFIMM { get; set; }

        /// <summary>
        /// XTTRISFIAA 9(2)  
        /// </summary>
        [HisFieldInfoMapping(24, 2, CobolType = CobolType.Unsigned)]
        public short XTTRISFIAA { get; set; }

        /// <summary>
        /// XTTRISMTGG 9(2)  
        /// </summary>
        [HisFieldInfoMapping(25, 2, CobolType = CobolType.Unsigned)]
        public short XTTRISMTGG { get; set; }

        /// <summary>
        /// XTTRISMTMM 9(2)  
        /// </summary>
        [HisFieldInfoMapping(26, 2, CobolType = CobolType.Unsigned)]
        public short XTTRISMTMM { get; set; }

        /// <summary>
        /// XTTRISMTAA 9(2)  
        /// </summary>
        [HisFieldInfoMapping(27, 2, CobolType = CobolType.Unsigned)]
        public short XTTRISMTAA { get; set; }

        /// <summary>
        /// XTTRISFGGG 9(2)  
        /// </summary>
        [HisFieldInfoMapping(28, 2, CobolType = CobolType.Unsigned)]
        public short XTTRISFGGG { get; set; }

        /// <summary>
        /// XTTRISFGMM 9(2)  
        /// </summary>
        [HisFieldInfoMapping(29, 2, CobolType = CobolType.Unsigned)]
        public short XTTRISFGMM { get; set; }

        /// <summary>
        /// XTTRISFGAA 9(2)  
        /// </summary>
        [HisFieldInfoMapping(30, 2, CobolType = CobolType.Unsigned)]
        public short XTTRISFGAA { get; set; }

        // 03 XTTTEODC.
        /// <summary>
        /// XTTTEOAA 9999  
        /// </summary>
        [HisFieldInfoMapping(31, 4, CobolType = CobolType.Unsigned)]
        public short XTTTEOAA { get; set; }

        /// <summary>
        /// XTTTEOMM 99  
        /// </summary>
        [HisFieldInfoMapping(32, 2, CobolType = CobolType.Unsigned)]
        public short XTTTEOMM { get; set; }

        /// <summary>
        /// XTTSPOBG 9(2)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(33, 6, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XTTSPOBG { get; set; }

        /// <summary>
        /// XTTDITTA XX  
        /// </summary>
        [HisFieldInfoMapping(34, 2)]
        public string XTTDITTA { get; set; }

        /// <summary>
        /// XTTUTRAA 99  
        /// </summary>
        [HisFieldInfoMapping(35, 2, CobolType = CobolType.Unsigned)]
        public short XTTUTRAA { get; set; }

        /// <summary>
        /// XTTUTRMM 99  
        /// </summary>
        [HisFieldInfoMapping(36, 2, CobolType = CobolType.Unsigned)]
        public short XTTUTRMM { get; set; }

        /// <summary>
        /// XTTPEN53 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(37, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XTTPEN53 { get; set; }

        /// <summary>
        /// XTTRTULT 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(38, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XTTRTULT { get; set; }

        /// <summary>
        /// XTTRTBIE 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(39, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XTTRTBIE { get; set; }

        /// <summary>
        /// XTTACCES 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(40, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XTTACCES { get; set; }

        /// <summary>
        /// XTTINARE 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(41, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XTTINARE { get; set; }

        /// <summary>
        /// XTTINAEF 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(42, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XTTINAEF { get; set; }

        /// <summary>
        /// XTTPNGEN 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(43, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XTTPNGEN { get; set; }

        /// <summary>
        /// XTTRTSUP 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(44, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XTTRTSUP { get; set; }

        /// <summary>
        /// XTTNONVE 9  
        /// </summary>
        [HisFieldInfoMapping(45, 1, CobolType = CobolType.Unsigned)]
        public short XTTNONVE { get; set; }

        /// <summary>
        /// XTTSPECI X  
        /// </summary>
        [HisFieldInfoMapping(46, 1)]
        public string XTTSPECI { get; set; }

        // *-L.503
        /// <summary>
        /// XTTREQU1 X  
        /// </summary>
        [HisFieldInfoMapping(47, 1)]
        public string XTTREQU1 { get; set; }

        /// <summary>
        /// XTTREQU2 9  
        /// </summary>
        [HisFieldInfoMapping(48, 1, CobolType = CobolType.Unsigned)]
        public short XTTREQU2 { get; set; }

        /// <summary>
        /// XTTLEG58 X  
        /// </summary>
        [HisFieldInfoMapping(49, 1)]
        public string XTTLEG58 { get; set; }

        /// <summary>
        /// XTTUT2AA 99  
        /// </summary>
        [HisFieldInfoMapping(50, 2, CobolType = CobolType.Unsigned)]
        public short XTTUT2AA { get; set; }

        /// <summary>
        /// XTTUT2MM 99  
        /// </summary>
        [HisFieldInfoMapping(51, 2, CobolType = CobolType.Unsigned)]
        public short XTTUT2MM { get; set; }

        /// <summary>
        /// XTTUTR2A 99  
        /// </summary>
        [HisFieldInfoMapping(52, 2, CobolType = CobolType.Unsigned)]
        public short XTTUTR2A { get; set; }

        /// <summary>
        /// XTTUTR2M 99  
        /// </summary>
        [HisFieldInfoMapping(53, 2, CobolType = CobolType.Unsigned)]
        public short XTTUTR2M { get; set; }

        /// <summary>
        /// XTTRETPN 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(54, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XTTRETPN { get; set; }

        /// <summary>
        /// XTTUT3AA 99  
        /// </summary>
        [HisFieldInfoMapping(55, 2, CobolType = CobolType.Unsigned)]
        public short XTTUT3AA { get; set; }

        /// <summary>
        /// XTTUT3MM 99  
        /// </summary>
        [HisFieldInfoMapping(56, 2, CobolType = CobolType.Unsigned)]
        public short XTTUT3MM { get; set; }

        /// <summary>
        /// XTTUTR3A 99  
        /// </summary>
        [HisFieldInfoMapping(57, 2, CobolType = CobolType.Unsigned)]
        public short XTTUTR3A { get; set; }

        /// <summary>
        /// XTTUTR3M 99  
        /// </summary>
        [HisFieldInfoMapping(58, 2, CobolType = CobolType.Unsigned)]
        public short XTTUTR3M { get; set; }

        /// <summary>
        /// XTTTETTO 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(59, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XTTTETTO { get; set; }

        /// <summary>
        /// XTTUT4AA 99  
        /// </summary>
        [HisFieldInfoMapping(60, 2, CobolType = CobolType.Unsigned)]
        public short XTTUT4AA { get; set; }

        /// <summary>
        /// XTTUT4MM 99  
        /// </summary>
        [HisFieldInfoMapping(61, 2, CobolType = CobolType.Unsigned)]
        public short XTTUT4MM { get; set; }

        /// <summary>
        /// XTTUTR4A 99  
        /// </summary>
        [HisFieldInfoMapping(62, 2, CobolType = CobolType.Unsigned)]
        public short XTTUTR4A { get; set; }

        /// <summary>
        /// XTTUTR4M 99  
        /// </summary>
        [HisFieldInfoMapping(63, 2, CobolType = CobolType.Unsigned)]
        public short XTTUTR4M { get; set; }

        /// <summary>
        /// XTTRETPD 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(64, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XTTRETPD { get; set; }

        /// <summary>
        /// XTTCATEG 9(3)  
        /// </summary>
        [HisFieldInfoMapping(65, 3, CobolType = CobolType.Unsigned)]
        public short XTTCATEG { get; set; }

        /// <summary>
        /// XTTSEDE 9(4)  
        /// </summary>
        [HisFieldInfoMapping(66, 4, CobolType = CobolType.Unsigned)]
        public short XTTSEDE { get; set; }

        /// <summary>
        /// XTTCERTI 9(8)  
        /// </summary>
        [HisFieldInfoMapping(67, 8, CobolType = CobolType.Unsigned)]
        public int XTTCERTI { get; set; }

        /// <summary>
        /// XTTPROGR 99  
        /// </summary>
        [HisFieldInfoMapping(68, 2, CobolType = CobolType.Unsigned)]
        public short XTTPROGR { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "TT"; }
        }
        #endregion Properties
    }
}