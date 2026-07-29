using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo
{
    public class VL : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  XVL-RECFOND-BIS REDEFINES COMUNE-RECFOND.
        //    02  XVL-RECFOND.
        //        03 XVLTIPOR                      PIC X.
        //        03 XVLFONDO                      PIC X(3).
        //        03 XVLTPENS                      PIC 9.
        //        03 XVLNATUR.
        //           04 XVLNATU1                   PIC 9.
        //           04 XVLNATU2                   PIC X.
        //           04 XVLNATU3                   PIC X.
        //        03 XVLDECOR.
        //           04 XVLDECAA                   PIC 9999.                
        //           04 XVLDECMM                   PIC 99.
        //        03 XVLSOSPE.
        //           04 XVLSOSAA                   PIC 9999.                
        //           04 XVLSOSMM                   PIC 99.
        //        03 XVLPVERS.
        //           04 XVLPVRAA                   PIC 9999.                
        //           04 XVLPVRMM                   PIC 99.
        //           04 XVLPVRGG                   PIC 99.
        //        03 XVLUVERS.
        //           04 XVLUVRAA                   PIC 9999.                
        //           04 XVLUVRMM                   PIC 99.
        //           04 XVLUVRGG                   PIC 99.
        //        03 XVLUTIAA                        PIC 99.
        //        03 XVLUTIMM                        PIC 99.
        //        03 XVLUTIGG                        PIC 99.
        //        03 XVLRETPN                        PIC 9(6)V9999.
        //        03 XVLFISSE                        PIC 9.
        //        03 XVLATTI1                        PIC 9.
        //        03 XVLATTI2                        PIC 9.
        //        03 XVLART22                        PIC 9.
        //        03 XVLINVDT.
        //           04 XVLINVAA                     PIC 9999.              
        //           04 XVLINVMM                     PIC 99.
        //           04 XVLINVGG                     PIC 99.
        //        03 XVLA65AA                        PIC 99.
        //        03 XVLA65MM                        PIC 99.
        //        03 XVLA65GG                        PIC 99.
        //        03 XVLP65AA                        PIC 99.
        //        03 XVLP65MM                        PIC 99.
        //        03 XVLP65GG                        PIC 99.
        //        03 XVLVOLAA                        PIC 99.
        //        03 XVLVOLMM                        PIC 99.
        //        03 XVLVOLGG                        PIC 99.
        //        03 XVLRISAA                        PIC 99.
        //        03 XVLRISMM                        PIC 99.
        //        03 XVLRISGG                        PIC 99.
        //        03 XVLAZAGO                        PIC 9(4).
        //        03 XVLSTAGO                        PIC 9(4)V9999.
        //        03 XVLCODCP                        PIC 9.
        //        03 XVLIMPCP                        PIC 9(5)V9999.
        //        03 XVLQUOCP                        PIC 9(5)V9999.
        //        03 XVLVALCP                        PIC 9(7)V9999.
        //        03 XVLIMPEN                        PIC 9(5)V9999.
        //        03 XVLNONCA                        PIC 9.
        //        03 XVLNONVE                        PIC 9.
        //        03 XVLSPECI                        PIC X.
        //*-DATI POST 311292 E 311294
        //        03 XVLREQU1                        PIC X.
        //        03 XVLREQU2                        PIC 9.
        //        03 XVLUT1AA                        PIC 99.
        //        03 XVLUT1MM                        PIC 99.
        //        03 XVLUT1GG                        PIC 99.
        //        03 XVLUTBAA                        PIC 99.
        //        03 XVLUTBMM                        PIC 99.
        //        03 XVLUTBGG                        PIC 99.
        //        03 XVLUTCAA                        PIC 99.
        //        03 XVLUTCMM                        PIC 99.
        //        03 XVLUTCGG                        PIC 99.
        //        03 XVLRE1PN                        PIC 9(6)V9999.
        //        03 XVLIRPEF.
        //           04 XVLIRINT                   PIC 99.
        //           04 XVLIRDEC                   PIC 999.
        //        03 XVLSETT1                        PIC 9(6)V9999.
        //        03 XVLSETT2                        PIC 9(9)V9999.
        //        03 XVLPROGR                        PIC 99.
        //     02 FILLER                             PIC X(57).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  XVL-RECFOND-BIS REDEFINES COMUNE-RECFOND.
        // 02  XVL-RECFOND.
        /// <summary>
        /// XVLTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string XVLTIPOR { get; set; }

        /// <summary>
        /// XVLFONDO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string XVLFONDO { get; set; }

        /// <summary>
        /// XVLTPENS 9  
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short XVLTPENS { get; set; }

        // 03 XVLNATUR.
        /// <summary>
        /// XVLNATU1 9  
        /// </summary>
        [HisFieldInfoMapping(3, 1, CobolType = CobolType.Unsigned)]
        public short XVLNATU1 { get; set; }

        /// <summary>
        /// XVLNATU2 X  
        /// </summary>
        [HisFieldInfoMapping(4, 1)]
        public string XVLNATU2 { get; set; }

        /// <summary>
        /// XVLNATU3 X  
        /// </summary>
        [HisFieldInfoMapping(5, 1)]
        public string XVLNATU3 { get; set; }

        // 03 XVLDECOR.
        /// <summary>
        /// XVLDECAA 9999  
        /// </summary>
        [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
        public short XVLDECAA { get; set; }

        /// <summary>
        /// XVLDECMM 99  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short XVLDECMM { get; set; }

        // 03 XVLSOSPE.
        /// <summary>
        /// XVLSOSAA 9999  
        /// </summary>
        [HisFieldInfoMapping(8, 4, CobolType = CobolType.Unsigned)]
        public short XVLSOSAA { get; set; }

        /// <summary>
        /// XVLSOSMM 99  
        /// </summary>
        [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
        public short XVLSOSMM { get; set; }

        // 03 XVLPVERS.
        /// <summary>
        /// XVLPVRAA 9999  
        /// </summary>
        [HisFieldInfoMapping(10, 4, CobolType = CobolType.Unsigned)]
        public short XVLPVRAA { get; set; }

        /// <summary>
        /// XVLPVRMM 99  
        /// </summary>
        [HisFieldInfoMapping(11, 2, CobolType = CobolType.Unsigned)]
        public short XVLPVRMM { get; set; }

        /// <summary>
        /// XVLPVRGG 99  
        /// </summary>
        [HisFieldInfoMapping(12, 2, CobolType = CobolType.Unsigned)]
        public short XVLPVRGG { get; set; }

        // 03 XVLUVERS.
        /// <summary>
        /// XVLUVRAA 9999  
        /// </summary>
        [HisFieldInfoMapping(13, 4, CobolType = CobolType.Unsigned)]
        public short XVLUVRAA { get; set; }

        /// <summary>
        /// XVLUVRMM 99  
        /// </summary>
        [HisFieldInfoMapping(14, 2, CobolType = CobolType.Unsigned)]
        public short XVLUVRMM { get; set; }

        /// <summary>
        /// XVLUVRGG 99  
        /// </summary>
        [HisFieldInfoMapping(15, 2, CobolType = CobolType.Unsigned)]
        public short XVLUVRGG { get; set; }

        /// <summary>
        /// XVLUTIAA 99  
        /// </summary>
        [HisFieldInfoMapping(16, 2, CobolType = CobolType.Unsigned)]
        public short XVLUTIAA { get; set; }

        /// <summary>
        /// XVLUTIMM 99  
        /// </summary>
        [HisFieldInfoMapping(17, 2, CobolType = CobolType.Unsigned)]
        public short XVLUTIMM { get; set; }

        /// <summary>
        /// XVLUTIGG 99  
        /// </summary>
        [HisFieldInfoMapping(18, 2, CobolType = CobolType.Unsigned)]
        public short XVLUTIGG { get; set; }

        /// <summary>
        /// XVLRETPN 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(19, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XVLRETPN { get; set; }

        /// <summary>
        /// XVLFISSE 9  
        /// </summary>
        [HisFieldInfoMapping(20, 1, CobolType = CobolType.Unsigned)]
        public short XVLFISSE { get; set; }

        /// <summary>
        /// XVLATTI1 9  
        /// </summary>
        [HisFieldInfoMapping(21, 1, CobolType = CobolType.Unsigned)]
        public short XVLATTI1 { get; set; }

        /// <summary>
        /// XVLATTI2 9  
        /// </summary>
        [HisFieldInfoMapping(22, 1, CobolType = CobolType.Unsigned)]
        public short XVLATTI2 { get; set; }

        /// <summary>
        /// XVLART22 9  
        /// </summary>
        [HisFieldInfoMapping(23, 1, CobolType = CobolType.Unsigned)]
        public short XVLART22 { get; set; }

        // 03 XVLINVDT.
        /// <summary>
        /// XVLINVAA 9999  
        /// </summary>
        [HisFieldInfoMapping(24, 4, CobolType = CobolType.Unsigned)]
        public short XVLINVAA { get; set; }

        /// <summary>
        /// XVLINVMM 99  
        /// </summary>
        [HisFieldInfoMapping(25, 2, CobolType = CobolType.Unsigned)]
        public short XVLINVMM { get; set; }

        /// <summary>
        /// XVLINVGG 99  
        /// </summary>
        [HisFieldInfoMapping(26, 2, CobolType = CobolType.Unsigned)]
        public short XVLINVGG { get; set; }

        /// <summary>
        /// XVLA65AA 99  
        /// </summary>
        [HisFieldInfoMapping(27, 2, CobolType = CobolType.Unsigned)]
        public short XVLA65AA { get; set; }

        /// <summary>
        /// XVLA65MM 99  
        /// </summary>
        [HisFieldInfoMapping(28, 2, CobolType = CobolType.Unsigned)]
        public short XVLA65MM { get; set; }

        /// <summary>
        /// XVLA65GG 99  
        /// </summary>
        [HisFieldInfoMapping(29, 2, CobolType = CobolType.Unsigned)]
        public short XVLA65GG { get; set; }

        /// <summary>
        /// XVLP65AA 99  
        /// </summary>
        [HisFieldInfoMapping(30, 2, CobolType = CobolType.Unsigned)]
        public short XVLP65AA { get; set; }

        /// <summary>
        /// XVLP65MM 99  
        /// </summary>
        [HisFieldInfoMapping(31, 2, CobolType = CobolType.Unsigned)]
        public short XVLP65MM { get; set; }

        /// <summary>
        /// XVLP65GG 99  
        /// </summary>
        [HisFieldInfoMapping(32, 2, CobolType = CobolType.Unsigned)]
        public short XVLP65GG { get; set; }

        /// <summary>
        /// XVLVOLAA 99  
        /// </summary>
        [HisFieldInfoMapping(33, 2, CobolType = CobolType.Unsigned)]
        public short XVLVOLAA { get; set; }

        /// <summary>
        /// XVLVOLMM 99  
        /// </summary>
        [HisFieldInfoMapping(34, 2, CobolType = CobolType.Unsigned)]
        public short XVLVOLMM { get; set; }

        /// <summary>
        /// XVLVOLGG 99  
        /// </summary>
        [HisFieldInfoMapping(35, 2, CobolType = CobolType.Unsigned)]
        public short XVLVOLGG { get; set; }

        /// <summary>
        /// XVLRISAA 99  
        /// </summary>
        [HisFieldInfoMapping(36, 2, CobolType = CobolType.Unsigned)]
        public short XVLRISAA { get; set; }

        /// <summary>
        /// XVLRISMM 99  
        /// </summary>
        [HisFieldInfoMapping(37, 2, CobolType = CobolType.Unsigned)]
        public short XVLRISMM { get; set; }

        /// <summary>
        /// XVLRISGG 99  
        /// </summary>
        [HisFieldInfoMapping(38, 2, CobolType = CobolType.Unsigned)]
        public short XVLRISGG { get; set; }

        /// <summary>
        /// XVLAZAGO 9(4)  
        /// </summary>
        [HisFieldInfoMapping(39, 4, CobolType = CobolType.Unsigned)]
        public short XVLAZAGO { get; set; }

        /// <summary>
        /// XVLSTAGO 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(40, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XVLSTAGO { get; set; }

        /// <summary>
        /// XVLCODCP 9  
        /// </summary>
        [HisFieldInfoMapping(41, 1, CobolType = CobolType.Unsigned)]
        public short XVLCODCP { get; set; }

        /// <summary>
        /// XVLIMPCP 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(42, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XVLIMPCP { get; set; }

        /// <summary>
        /// XVLQUOCP 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(43, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XVLQUOCP { get; set; }

        /// <summary>
        /// XVLVALCP 9(7)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(44, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XVLVALCP { get; set; }

        /// <summary>
        /// XVLIMPEN 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(45, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XVLIMPEN { get; set; }

        /// <summary>
        /// XVLNONCA 9  
        /// </summary>
        [HisFieldInfoMapping(46, 1, CobolType = CobolType.Unsigned)]
        public short XVLNONCA { get; set; }

        /// <summary>
        /// XVLNONVE 9  
        /// </summary>
        [HisFieldInfoMapping(47, 1, CobolType = CobolType.Unsigned)]
        public short XVLNONVE { get; set; }

        /// <summary>
        /// XVLSPECI X  
        /// </summary>
        [HisFieldInfoMapping(48, 1)]
        public string XVLSPECI { get; set; }

        // *-DATI POST 311292 E 311294
        /// <summary>
        /// XVLREQU1 X  
        /// </summary>
        [HisFieldInfoMapping(49, 1)]
        public string XVLREQU1 { get; set; }

        /// <summary>
        /// XVLREQU2 9  
        /// </summary>
        [HisFieldInfoMapping(50, 1, CobolType = CobolType.Unsigned)]
        public short XVLREQU2 { get; set; }

        /// <summary>
        /// XVLUT1AA 99  
        /// </summary>
        [HisFieldInfoMapping(51, 2, CobolType = CobolType.Unsigned)]
        public short XVLUT1AA { get; set; }

        /// <summary>
        /// XVLUT1MM 99  
        /// </summary>
        [HisFieldInfoMapping(52, 2, CobolType = CobolType.Unsigned)]
        public short XVLUT1MM { get; set; }

        /// <summary>
        /// XVLUT1GG 99  
        /// </summary>
        [HisFieldInfoMapping(53, 2, CobolType = CobolType.Unsigned)]
        public short XVLUT1GG { get; set; }

        /// <summary>
        /// XVLUTBAA 99  
        /// </summary>
        [HisFieldInfoMapping(54, 2, CobolType = CobolType.Unsigned)]
        public short XVLUTBAA { get; set; }

        /// <summary>
        /// XVLUTBMM 99  
        /// </summary>
        [HisFieldInfoMapping(55, 2, CobolType = CobolType.Unsigned)]
        public short XVLUTBMM { get; set; }

        /// <summary>
        /// XVLUTBGG 99  
        /// </summary>
        [HisFieldInfoMapping(56, 2, CobolType = CobolType.Unsigned)]
        public short XVLUTBGG { get; set; }

        /// <summary>
        /// XVLUTCAA 99  
        /// </summary>
        [HisFieldInfoMapping(57, 2, CobolType = CobolType.Unsigned)]
        public short XVLUTCAA { get; set; }

        /// <summary>
        /// XVLUTCMM 99  
        /// </summary>
        [HisFieldInfoMapping(58, 2, CobolType = CobolType.Unsigned)]
        public short XVLUTCMM { get; set; }

        /// <summary>
        /// XVLUTCGG 99  
        /// </summary>
        [HisFieldInfoMapping(59, 2, CobolType = CobolType.Unsigned)]
        public short XVLUTCGG { get; set; }

        /// <summary>
        /// XVLRE1PN 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(60, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XVLRE1PN { get; set; }

        // 03 XVLIRPEF.
        /// <summary>
        /// XVLIRINT 99  
        /// </summary>
        [HisFieldInfoMapping(61, 2, CobolType = CobolType.Unsigned)]
        public short XVLIRINT { get; set; }

        /// <summary>
        /// XVLIRDEC 999  
        /// </summary>
        [HisFieldInfoMapping(62, 3, CobolType = CobolType.Unsigned)]
        public short XVLIRDEC { get; set; }

        /// <summary>
        /// XVLSETT1 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(63, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XVLSETT1 { get; set; }

        /// <summary>
        /// XVLSETT2 9(9)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(64, 13, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XVLSETT2 { get; set; }

        /// <summary>
        /// XVLPROGR 99  
        /// </summary>
        [HisFieldInfoMapping(65, 2, CobolType = CobolType.Unsigned)]
        public short XVLPROGR { get; set; }

        #endregion Tracciato Host

        public string TransactionName
        {
            get { return "VL"; }
        }
        #endregion Properties
    }
}
