using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo
{
    public class ES : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  XES-RECFOND-BIS REDEFINES COMUNE-RECFOND.
        //          02  XES-RECFOND.
        //              03 XESTIPOR                      PIC X.
        //              03 XESFONDO                      PIC X(3).
        //              03 XESTPENS                      PIC 9.
        //              03 XESNATUR.
        //                 04 XESNATU1                   PIC 9.
        //                 04 XESNATU2                   PIC X.
        //                 04 XESNATU3                   PIC X.
        //D2000         03 XESDECOR.
        //D2NEW            04 XESDECAA                   PIC 9999.                
        //                 04 XESDECMM                   PIC 99.
        //D2000         03 XESSOSPE.
        //D2NEW            04 XESSOSAA                   PIC 9999.                
        //                 04 XESSOSMM                   PIC 99.
        //D2000         03 XESPVERS.
        //D2NEW            04 XESPVRAA                   PIC 9999.                
        //                 04 XESPVRMM                   PIC 99.
        //                 04 XESPVRGG                   PIC 99.
        //D2000         03 XESUVERS.
        //D2NEW            04 XESUVRAA                   PIC 9999.                
        //                 04 XESUVRMM                   PIC 99.
        //                 04 XESUVRGG                   PIC 99.
        //              03 XESUTIAA                        PIC 99.
        //              03 XESUTIMM                        PIC 99.
        //              03 XESRETPN                        PIC 9(6)V9999.
        //              03 XESCONVE                        PIC X.
        //              03 XESNOCAL                        PIC 9.
        //              03 XESATTIV                        PIC 9.
        //              03 XESFISSE                        PIC 9.
        //              03 XESCOMBA                        PIC XX.
        //              03 XESNO336                        PIC 9(6)V9999.
        //              03 XESRISAA                        PIC 99.
        //              03 XESRISMM                        PIC 99.
        //              03 XESPNFON                        PIC 9(4)V9999.
        //D2000         03 XES24DEC.
        //D2NEW            04 XES24DAA                     PIC 9999.              
        //                 04 XES24DMM                     PIC 99.
        //              03 XES24CTR                        PIC 9(4)V9999.
        //D2000         03 XES57ELE OCCURS 3 TIMES.
        //D2000            04 XES57DEC.
        //D2NEW               05 XES57DAA                  PIC 9999.              
        //                    05 XES57DMM                  PIC 99.
        //                 04 XES57CTR                     PIC 9(4)V9999.
        //              03 XESIMPAG                        PIC 9(4)V9999.
        //              03 XESCODPN                        PIC X.
        //              03 XESCODES                        PIC 9(4).
        //              03 XESCLASS                        PIC 99.
        //              03 XESART58                        PIC 9.
        //              03 XESART59                        PIC 9.
        //              03 XESOPTAN                        PIC 9.
        //              03 XESSALTU                        PIC 9.
        //              03 XESPROMI                        PIC 9.
        //              03 XESCODIC                        PIC 9.
        //              03 XESANNUT                        PIC 9.
        //              03 XESCDRET                        PIC 9.
        //              03 XESPRIVI                        PIC 9.
        //              03 XESCALCO OCCURS 4 TIMES.
        //                 04 XESCALMM                     PIC 9(3).
        //                 04 XESCALRT                     PIC 9(6)V9999.
        //              03 XESNONVE                        PIC 9.
        //              03 XESSPECI                        PIC X.
        //              03 XESSETA-707                     PIC 9(4).
        //              03 XESSETB-707                     PIC 9(4).
        //              03 XESCALC707                      PIC X(01).
        //              03 XESDEC58                        PIC 9(6).
        //              03 XESMONT58                       PIC 9(7)V9(4) COMP-3.
        //              03 XESPROGR                        PIC 99.
        //           02 FILLER                             PIC X(45).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  XES-RECFOND-BIS REDEFINES COMUNE-RECFOND.
        // 02  XES-RECFOND.
        /// <summary>
        /// XESTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string XESTIPOR { get; set; }

        /// <summary>
        /// XESFONDO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string XESFONDO { get; set; }

        /// <summary>
        /// XESTPENS 9  
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short XESTPENS { get; set; }

        // 03 XESNATUR.
        /// <summary>
        /// XESNATU1 9  
        /// </summary>
        [HisFieldInfoMapping(3, 1, CobolType = CobolType.Unsigned)]
        public short XESNATU1 { get; set; }

        /// <summary>
        /// XESNATU2 X  
        /// </summary>
        [HisFieldInfoMapping(4, 1)]
        public string XESNATU2 { get; set; }

        /// <summary>
        /// XESNATU3 X  
        /// </summary>
        [HisFieldInfoMapping(5, 1)]
        public string XESNATU3 { get; set; }

        // D2000         03 XESDECOR.
        /// <summary>
        /// XESDECAA 9999  
        /// </summary>
        [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
        public short XESDECAA { get; set; }

        /// <summary>
        /// XESDECMM 99  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short XESDECMM { get; set; }

        // D2000         03 XESSOSPE.
        /// <summary>
        /// XESSOSAA 9999  
        /// </summary>
        [HisFieldInfoMapping(8, 4, CobolType = CobolType.Unsigned)]
        public short XESSOSAA { get; set; }

        /// <summary>
        /// XESSOSMM 99  
        /// </summary>
        [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
        public short XESSOSMM { get; set; }

        // D2000         03 XESPVERS.
        /// <summary>
        /// XESPVRAA 9999  
        /// </summary>
        [HisFieldInfoMapping(10, 4, CobolType = CobolType.Unsigned)]
        public short XESPVRAA { get; set; }

        /// <summary>
        /// XESPVRMM 99  
        /// </summary>
        [HisFieldInfoMapping(11, 2, CobolType = CobolType.Unsigned)]
        public short XESPVRMM { get; set; }

        /// <summary>
        /// XESPVRGG 99  
        /// </summary>
        [HisFieldInfoMapping(12, 2, CobolType = CobolType.Unsigned)]
        public short XESPVRGG { get; set; }

        // D2000         03 XESUVERS.
        /// <summary>
        /// XESUVRAA 9999  
        /// </summary>
        [HisFieldInfoMapping(13, 4, CobolType = CobolType.Unsigned)]
        public short XESUVRAA { get; set; }

        /// <summary>
        /// XESUVRMM 99  
        /// </summary>
        [HisFieldInfoMapping(14, 2, CobolType = CobolType.Unsigned)]
        public short XESUVRMM { get; set; }

        /// <summary>
        /// XESUVRGG 99  
        /// </summary>
        [HisFieldInfoMapping(15, 2, CobolType = CobolType.Unsigned)]
        public short XESUVRGG { get; set; }

        /// <summary>
        /// XESUTIAA 99  
        /// </summary>
        [HisFieldInfoMapping(16, 2, CobolType = CobolType.Unsigned)]
        public short XESUTIAA { get; set; }

        /// <summary>
        /// XESUTIMM 99  
        /// </summary>
        [HisFieldInfoMapping(17, 2, CobolType = CobolType.Unsigned)]
        public short XESUTIMM { get; set; }

        /// <summary>
        /// XESRETPN 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(18, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XESRETPN { get; set; }

        /// <summary>
        /// XESCONVE X  
        /// </summary>
        [HisFieldInfoMapping(19, 1)]
        public string XESCONVE { get; set; }

        /// <summary>
        /// XESNOCAL 9  
        /// </summary>
        [HisFieldInfoMapping(20, 1, CobolType = CobolType.Unsigned)]
        public short XESNOCAL { get; set; }

        /// <summary>
        /// XESATTIV 9  
        /// </summary>
        [HisFieldInfoMapping(21, 1, CobolType = CobolType.Unsigned)]
        public short XESATTIV { get; set; }

        /// <summary>
        /// XESFISSE 9  
        /// </summary>
        [HisFieldInfoMapping(22, 1, CobolType = CobolType.Unsigned)]
        public short XESFISSE { get; set; }

        /// <summary>
        /// XESCOMBA XX  
        /// </summary>
        [HisFieldInfoMapping(23, 2)]
        public string XESCOMBA { get; set; }

        /// <summary>
        /// XESNO336 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(24, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XESNO336 { get; set; }

        /// <summary>
        /// XESRISAA 99  
        /// </summary>
        [HisFieldInfoMapping(25, 2, CobolType = CobolType.Unsigned)]
        public short XESRISAA { get; set; }

        /// <summary>
        /// XESRISMM 99  
        /// </summary>
        [HisFieldInfoMapping(26, 2, CobolType = CobolType.Unsigned)]
        public short XESRISMM { get; set; }

        /// <summary>
        /// XESPNFON 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(27, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XESPNFON { get; set; }

        // D2000         03 XES24DEC.
        /// <summary>
        /// XES24DAA 9999  
        /// </summary>
        [HisFieldInfoMapping(28, 4, CobolType = CobolType.Unsigned)]
        public short XES24DAA { get; set; }

        /// <summary>
        /// XES24DMM 99  
        /// </summary>
        [HisFieldInfoMapping(29, 2, CobolType = CobolType.Unsigned)]
        public short XES24DMM { get; set; }

        /// <summary>
        /// XES24CTR 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(30, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XES24CTR { get; set; }

        /// <summary>
        /// XES57ELE OCCURS 3 TIMES.
        /// </summary>
        [HisComplexAreaInfoMapping(31, ListCount = 3)]
        public List<XES57ELE> LISTXES57ELE { get; set; }

        /// <summary>
        /// XESIMPAG 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(32, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XESIMPAG { get; set; }

        /// <summary>
        /// XESCODPN X  
        /// </summary>
        [HisFieldInfoMapping(33, 1)]
        public string XESCODPN { get; set; }

        /// <summary>
        /// XESCODES 9(4)  
        /// </summary>
        [HisFieldInfoMapping(34, 4, CobolType = CobolType.Unsigned)]
        public short XESCODES { get; set; }

        /// <summary>
        /// XESCLASS 99  
        /// </summary>
        [HisFieldInfoMapping(35, 2, CobolType = CobolType.Unsigned)]
        public short XESCLASS { get; set; }

        /// <summary>
        /// XESART58 9  
        /// </summary>
        [HisFieldInfoMapping(36, 1, CobolType = CobolType.Unsigned)]
        public short XESART58 { get; set; }

        /// <summary>
        /// XESART59 9  
        /// </summary>
        [HisFieldInfoMapping(37, 1, CobolType = CobolType.Unsigned)]
        public short XESART59 { get; set; }

        /// <summary>
        /// XESOPTAN 9  
        /// </summary>
        [HisFieldInfoMapping(38, 1, CobolType = CobolType.Unsigned)]
        public short XESOPTAN { get; set; }

        /// <summary>
        /// XESSALTU 9  
        /// </summary>
        [HisFieldInfoMapping(39, 1, CobolType = CobolType.Unsigned)]
        public short XESSALTU { get; set; }

        /// <summary>
        /// XESPROMI 9  
        /// </summary>
        [HisFieldInfoMapping(40, 1, CobolType = CobolType.Unsigned)]
        public short XESPROMI { get; set; }

        /// <summary>
        /// XESCODIC 9  
        /// </summary>
        [HisFieldInfoMapping(41, 1, CobolType = CobolType.Unsigned)]
        public short XESCODIC { get; set; }

        /// <summary>
        /// XESANNUT 9  
        /// </summary>
        [HisFieldInfoMapping(42, 1, CobolType = CobolType.Unsigned)]
        public short XESANNUT { get; set; }

        /// <summary>
        /// XESCDRET 9  
        /// </summary>
        [HisFieldInfoMapping(43, 1, CobolType = CobolType.Unsigned)]
        public short XESCDRET { get; set; }

        /// <summary>
        /// XESPRIVI 9  
        /// </summary>
        [HisFieldInfoMapping(44, 1, CobolType = CobolType.Unsigned)]
        public short XESPRIVI { get; set; }

        /// <summary>
        /// XESCALCO OCCURS 4 TIMES.
        /// </summary>
        [HisComplexAreaInfoMapping(45, ListCount = 4)]
        public List<XESCALCO> LISTXESCALCO { get; set; }

        /// <summary>
        /// XESNONVE 9  
        /// </summary>
        [HisFieldInfoMapping(46, 1, CobolType = CobolType.Unsigned)]
        public short XESNONVE { get; set; }

        /// <summary>
        /// XESSPECI X  
        /// </summary>
        [HisFieldInfoMapping(47, 1)]
        public string XESSPECI { get; set; }

        /// <summary>
        /// XESSETA-707 9(4)  
        /// </summary>
        [HisFieldInfoMapping(48, 4, CobolType = CobolType.Unsigned)]
        public short XESSETA_707 { get; set; }

        /// <summary>
        /// XESSETB-707 9(4)  
        /// </summary>
        [HisFieldInfoMapping(49, 4, CobolType = CobolType.Unsigned)]
        public short XESSETB_707 { get; set; }

        /// <summary>
        /// XESCALC707 X(01)  
        /// </summary>
        [HisFieldInfoMapping(50, 1)]
        public string XESCALC707 { get; set; }

        /// <summary>
        /// XESDEC58 PIC 9(6)  
        /// </summary>
        [HisFieldInfoMapping(51, 6, CobolType = CobolType.Unsigned)]
        public int XESDEC58 { get; set; }

        /// <summary>
        /// XESMONT58 PIC 9(7)V9(4) COMP-3  
        /// </summary>
        [HisFieldInfoMapping(52, 6, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XESMONT58 { get; set; }

        /// <summary>
        /// XESPROGR 99  
        /// </summary>
        [HisFieldInfoMapping(53, 2, CobolType = CobolType.Unsigned)]
        public short XESPROGR { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "ES"; }
        }
        #endregion Properties

        #region nested class
        public class XES57ELE
        {
            #region Constructor
            public XES57ELE()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //D2000         03 XES57ELE OCCURS 3 TIMES.
            //D2000            04 XES57DEC.
            //D2NEW               05 XES57DAA                  PIC 9999.              
            //                    05 XES57DMM                  PIC 99.
            //                 04 XES57CTR                     PIC 9(4)V9999.
            #endregion Tracciato COBOL

            #region Tracciato Host
            // D2000         03 XES57ELE OCCURS 3 TIMES.
            // D2000            04 XES57DEC.
            /// <summary>
            /// XES57DAA 9999  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short XES57DAA { get; set; }

            /// <summary>
            /// XES57DMM 99  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short XES57DMM { get; set; }

            /// <summary>
            /// XES57CTR 9(4)V9(4)  
            /// </summary>
            [HisFieldInfoMapping(2, 8, Scale = 4, CobolType = CobolType.Unsigned)]
            public decimal XES57CTR { get; set; }

            #endregion Tracciato Host

            #endregion Properties
        }

        public class XESCALCO
        {
            #region Constructor
            public XESCALCO()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //              03 XESCALCO OCCURS 4 TIMES.
            //                 04 XESCALMM                     PIC 9(3).
            //                 04 XESCALRT                     PIC 9(6)V9999.
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 03 XESCALCO OCCURS 4 TIMES.
            /// <summary>
            /// XESCALMM 9(3)  
            /// </summary>
            [HisFieldInfoMapping(0, 3, CobolType = CobolType.Unsigned)]
            public short XESCALMM { get; set; }

            /// <summary>
            /// XESCALRT 9(6)V9(4)  
            /// </summary>
            [HisFieldInfoMapping(1, 10, Scale = 4, CobolType = CobolType.Unsigned)]
            public decimal XESCALRT { get; set; }

            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class
    }
}
