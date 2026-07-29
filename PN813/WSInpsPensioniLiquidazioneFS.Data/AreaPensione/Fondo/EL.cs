using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo
{
    public class EL : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  XEL-RECFOND-BIS REDEFINES COMUNE-RECFOND.
        //          02  XEL-RECFOND.
        //              03 XELTIPOR                      PIC X.
        //              03 XELFONDO                      PIC X(3).
        //              03 XELTPENS                      PIC 9.
        //              03 XELNATUR.
        //                 04 XELNATU1                   PIC 9.
        //                 04 XELNATU2                   PIC X.
        //                 04 XELNATU3                   PIC X.
        //D2000         03 XELDECOR.
        //D2NEW            04 XELDECAA                   PIC 9999.                
        //                 04 XELDECMM                   PIC 99.
        //D2000         03 XELSOSPE.
        //D2NEW            04 XELSOSAA                   PIC 9999.                
        //                 04 XELSOSMM                   PIC 99.
        //D2000         03 XELPVERS.
        //D2NEW              04 XELPVRAA                   PIC 9999.              
        //                   04 XELPVRMM                   PIC 99.
        //                   04 XELPVRGG                   PIC 99.
        //D2000         03 XELUVERS.
        //D2NEW              04 XELUVRAA                   PIC 9999.              
        //                   04 XELUVRMM                   PIC 99.
        //                   04 XELUVRGG                   PIC 99.
        //D2000         03 XELTEODC.
        //D2NEW            04 XELTEOAA                   PIC 9999.                
        //                 04 XELTEOMM                   PIC 99.
        //              03 XELRETPN                        PIC 9(6)V9999.
        //              03 XELCONVE                        PIC X.
        //              03 XELNONCA                        PIC 9.
        //              03 XELATTIV                        PIC 9.
        //              03 XELUTIAA                        PIC 99.
        //              03 XELUTIMM                        PIC 99.
        //              03 XELRISAA                        PIC 99.
        //              03 XELRISMM                        PIC 99.
        //              03 XELPREAA                        PIC 99.
        //              03 XELPREMM                        PIC 99.
        //              03 XELMILAA                        PIC 99.
        //              03 XELMILMM                        PIC 99.
        //              03 XELAR3AA                        PIC 99.
        //              03 XELAR3MM                        PIC 99.
        //              03 XELFISSE                        PIC 9.
        //              03 XELGRADO                        PIC 9.
        //              03 XELMAGGI                        PIC 99.
        //              03 XELPRENE                        PIC 9.
        //              03 XELCOMBA                        PIC XX.
        //              03 XELNO336                        PIC 9(6)V9999.
        //              03 XELMG336                        PIC 99.
        //              03 XELAZIEN                        PIC 9.
        //              03 XELNONVE                        PIC 9.
        //              03 XELSPECI                        PIC X.
        //      *-L.503
        //              03 XELREQU1                        PIC X.
        //              03 XELREQU2                        PIC 9.
        //              03 XELN2336                        PIC 9(6)V9999.
        //              03 XELUT2AA                        PIC 99.
        //              03 XELUT2MM                        PIC 99.
        //              03 XELRE2PN                        PIC 9(6)V9999.
        //              03 XELUT3AA                        PIC 99.
        //              03 XELUT3MM                        PIC 99.
        //              03 XELTETTO                        PIC 9(6)V9999.
        //              03 XELCATEG                        PIC 9(3).
        //              03 XELSEDE                         PIC 9(4).
        //              03 XELCERTI                        PIC 9(8).
        //              03 XELPROGR                        PIC 99.
        //           02 FILLER                             PIC X(105).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  XEL-RECFOND-BIS REDEFINES COMUNE-RECFOND.
        // 02  XEL-RECFOND.
        /// <summary>
        /// XELTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string XELTIPOR { get; set; }

        /// <summary>
        /// XELFONDO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string XELFONDO { get; set; }

        /// <summary>
        /// XELTPENS 9  
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short XELTPENS { get; set; }

        // 03 XELNATUR.
        /// <summary>
        /// XELNATU1 9  
        /// </summary>
        [HisFieldInfoMapping(3, 1, CobolType = CobolType.Unsigned)]
        public short XELNATU1 { get; set; }

        /// <summary>
        /// XELNATU2 X  
        /// </summary>
        [HisFieldInfoMapping(4, 1)]
        public string XELNATU2 { get; set; }

        /// <summary>
        /// XELNATU3 X  
        /// </summary>
        [HisFieldInfoMapping(5, 1)]
        public string XELNATU3 { get; set; }

        // D2000         03 XELDECOR.
        /// <summary>
        /// XELDECAA 9999  
        /// </summary>
        [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
        public short XELDECAA { get; set; }

        /// <summary>
        /// XELDECMM 99  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short XELDECMM { get; set; }

        // D2000         03 XELSOSPE.
        /// <summary>
        /// XELSOSAA 9999  
        /// </summary>
        [HisFieldInfoMapping(8, 4, CobolType = CobolType.Unsigned)]
        public short XELSOSAA { get; set; }

        /// <summary>
        /// XELSOSMM 99  
        /// </summary>
        [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
        public short XELSOSMM { get; set; }

        // D2000         03 XELPVERS.
        /// <summary>
        /// XELPVRAA 9999  
        /// </summary>
        [HisFieldInfoMapping(10, 4, CobolType = CobolType.Unsigned)]
        public short XELPVRAA { get; set; }

        /// <summary>
        /// XELPVRMM 99  
        /// </summary>
        [HisFieldInfoMapping(11, 2, CobolType = CobolType.Unsigned)]
        public short XELPVRMM { get; set; }

        /// <summary>
        /// XELPVRGG 99  
        /// </summary>
        [HisFieldInfoMapping(12, 2, CobolType = CobolType.Unsigned)]
        public short XELPVRGG { get; set; }

        // D2000         03 XELUVERS.
        /// <summary>
        /// XELUVRAA 9999  
        /// </summary>
        [HisFieldInfoMapping(13, 4, CobolType = CobolType.Unsigned)]
        public short XELUVRAA { get; set; }

        /// <summary>
        /// XELUVRMM 99  
        /// </summary>
        [HisFieldInfoMapping(14, 2, CobolType = CobolType.Unsigned)]
        public short XELUVRMM { get; set; }

        /// <summary>
        /// XELUVRGG 99  
        /// </summary>
        [HisFieldInfoMapping(15, 2, CobolType = CobolType.Unsigned)]
        public short XELUVRGG { get; set; }

        // D2000         03 XELTEODC.
        /// <summary>
        /// XELTEOAA 9999  
        /// </summary>
        [HisFieldInfoMapping(16, 4, CobolType = CobolType.Unsigned)]
        public short XELTEOAA { get; set; }

        /// <summary>
        /// XELTEOMM 99  
        /// </summary>
        [HisFieldInfoMapping(17, 2, CobolType = CobolType.Unsigned)]
        public short XELTEOMM { get; set; }

        /// <summary>
        /// XELRETPN 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(18, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XELRETPN { get; set; }

        /// <summary>
        /// XELCONVE X  
        /// </summary>
        [HisFieldInfoMapping(19, 1)]
        public string XELCONVE { get; set; }

        /// <summary>
        /// XELNONCA 9  
        /// </summary>
        [HisFieldInfoMapping(20, 1, CobolType = CobolType.Unsigned)]
        public short XELNONCA { get; set; }

        /// <summary>
        /// XELATTIV 9  
        /// </summary>
        [HisFieldInfoMapping(21, 1, CobolType = CobolType.Unsigned)]
        public short XELATTIV { get; set; }

        /// <summary>
        /// XELUTIAA 99  
        /// </summary>
        [HisFieldInfoMapping(22, 2, CobolType = CobolType.Unsigned)]
        public short XELUTIAA { get; set; }

        /// <summary>
        /// XELUTIMM 99  
        /// </summary>
        [HisFieldInfoMapping(23, 2, CobolType = CobolType.Unsigned)]
        public short XELUTIMM { get; set; }

        /// <summary>
        /// XELRISAA 99  
        /// </summary>
        [HisFieldInfoMapping(24, 2, CobolType = CobolType.Unsigned)]
        public short XELRISAA { get; set; }

        /// <summary>
        /// XELRISMM 99  
        /// </summary>
        [HisFieldInfoMapping(25, 2, CobolType = CobolType.Unsigned)]
        public short XELRISMM { get; set; }

        /// <summary>
        /// XELPREAA 99  
        /// </summary>
        [HisFieldInfoMapping(26, 2, CobolType = CobolType.Unsigned)]
        public short XELPREAA { get; set; }

        /// <summary>
        /// XELPREMM 99  
        /// </summary>
        [HisFieldInfoMapping(27, 2, CobolType = CobolType.Unsigned)]
        public short XELPREMM { get; set; }

        /// <summary>
        /// XELMILAA 99  
        /// </summary>
        [HisFieldInfoMapping(28, 2, CobolType = CobolType.Unsigned)]
        public short XELMILAA { get; set; }

        /// <summary>
        /// XELMILMM 99  
        /// </summary>
        [HisFieldInfoMapping(29, 2, CobolType = CobolType.Unsigned)]
        public short XELMILMM { get; set; }

        /// <summary>
        /// XELAR3AA 99  
        /// </summary>
        [HisFieldInfoMapping(30, 2, CobolType = CobolType.Unsigned)]
        public short XELAR3AA { get; set; }

        /// <summary>
        /// XELAR3MM 99  
        /// </summary>
        [HisFieldInfoMapping(31, 2, CobolType = CobolType.Unsigned)]
        public short XELAR3MM { get; set; }

        /// <summary>
        /// XELFISSE 9  
        /// </summary>
        [HisFieldInfoMapping(32, 1, CobolType = CobolType.Unsigned)]
        public short XELFISSE { get; set; }

        /// <summary>
        /// XELGRADO 9  
        /// </summary>
        [HisFieldInfoMapping(33, 1, CobolType = CobolType.Unsigned)]
        public short XELGRADO { get; set; }

        /// <summary>
        /// XELMAGGI 99  
        /// </summary>
        [HisFieldInfoMapping(34, 2, CobolType = CobolType.Unsigned)]
        public short XELMAGGI { get; set; }

        /// <summary>
        /// XELPRENE 9  
        /// </summary>
        [HisFieldInfoMapping(35, 1, CobolType = CobolType.Unsigned)]
        public short XELPRENE { get; set; }

        /// <summary>
        /// XELCOMBA XX  
        /// </summary>
        [HisFieldInfoMapping(36, 2)]
        public string XELCOMBA { get; set; }

        /// <summary>
        /// XELNO336 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(37, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XELNO336 { get; set; }

        /// <summary>
        /// XELMG336 99  
        /// </summary>
        [HisFieldInfoMapping(38, 2, CobolType = CobolType.Unsigned)]
        public short XELMG336 { get; set; }

        /// <summary>
        /// XELAZIEN 9  
        /// </summary>
        [HisFieldInfoMapping(39, 1, CobolType = CobolType.Unsigned)]
        public short XELAZIEN { get; set; }

        /// <summary>
        /// XELNONVE 9  
        /// </summary>
        [HisFieldInfoMapping(40, 1, CobolType = CobolType.Unsigned)]
        public short XELNONVE { get; set; }

        /// <summary>
        /// XELSPECI X  
        /// </summary>
        [HisFieldInfoMapping(41, 1)]
        public string XELSPECI { get; set; }

        // *-L.503
        /// <summary>
        /// XELREQU1 X  
        /// </summary>
        [HisFieldInfoMapping(42, 1)]
        public string XELREQU1 { get; set; }

        /// <summary>
        /// XELREQU2 9  
        /// </summary>
        [HisFieldInfoMapping(43, 1, CobolType = CobolType.Unsigned)]
        public short XELREQU2 { get; set; }

        /// <summary>
        /// XELN2336 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(44, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XELN2336 { get; set; }

        /// <summary>
        /// XELUT2AA 99  
        /// </summary>
        [HisFieldInfoMapping(45, 2, CobolType = CobolType.Unsigned)]
        public short XELUT2AA { get; set; }

        /// <summary>
        /// XELUT2MM 99  
        /// </summary>
        [HisFieldInfoMapping(46, 2, CobolType = CobolType.Unsigned)]
        public short XELUT2MM { get; set; }

        /// <summary>
        /// XELRE2PN 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(47, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XELRE2PN { get; set; }

        /// <summary>
        /// XELUT3AA 99  
        /// </summary>
        [HisFieldInfoMapping(48, 2, CobolType = CobolType.Unsigned)]
        public short XELUT3AA { get; set; }

        /// <summary>
        /// XELUT3MM 99  
        /// </summary>
        [HisFieldInfoMapping(49, 2, CobolType = CobolType.Unsigned)]
        public short XELUT3MM { get; set; }

        /// <summary>
        /// XELTETTO 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(50, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XELTETTO { get; set; }

        /// <summary>
        /// XELCATEG 9(3)  
        /// </summary>
        [HisFieldInfoMapping(51, 3, CobolType = CobolType.Unsigned)]
        public short XELCATEG { get; set; }

        /// <summary>
        /// XELSEDE 9(4)  
        /// </summary>
        [HisFieldInfoMapping(52, 4, CobolType = CobolType.Unsigned)]
        public short XELSEDE { get; set; }

        /// <summary>
        /// XELCERTI 9(8)  
        /// </summary>
        [HisFieldInfoMapping(53, 8, CobolType = CobolType.Unsigned)]
        public int XELCERTI { get; set; }

        /// <summary>
        /// XELPROGR 99  
        /// </summary>
        [HisFieldInfoMapping(54, 2, CobolType = CobolType.Unsigned)]
        public short XELPROGR { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "EL"; }
        }
        #endregion Properties
    }
}
