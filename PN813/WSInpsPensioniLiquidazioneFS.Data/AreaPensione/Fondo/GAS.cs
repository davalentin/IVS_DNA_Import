using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo
{
    public class GAS : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  XGA-RECFOND-BIS REDEFINES COMUNE-RECFOND.
        //D2000     02  XGA-RECFOND.
        //              03 XGATIPOR                      PIC X.
        //              03 XGAFONDO                      PIC X(3).
        //              03 XGATPENS                      PIC 9.
        //              03 XGANATUR.
        //                 04 XGANATU1                   PIC 9.
        //                 04 XGANATU2                   PIC X.
        //                 04 XGANATU3                   PIC X.
        //D2000         03 XGADECOR.
        //D2NEW            04 XGADECAA                   PIC 9999.                
        //                 04 XGADECMM                   PIC 99.
        //D2000         03 XGASOSPE.
        //D2NEW            04 XGASOSAA                   PIC 9999.                
        //                 04 XGASOSMM                   PIC 99.
        //D2000         03 XGAPVERS.
        //D2NEW              04 XGAPVRAA                   PIC 9999.              
        //                   04 XGAPVRMM                   PIC 99.
        //                   04 XGAPVRGG                   PIC 99.
        //D2000         03 XGAUVERS.
        //D2NEW              04 XGAUVRAA                   PIC 9999.              
        //                   04 XGAUVRMM                   PIC 99.
        //                   04 XGAUVRGG                   PIC 99.
        //              03 XGAUTIAA                        PIC 99.
        //              03 XGAUTIMM                        PIC 99.
        //              03 XGARETPN                        PIC 9(6)V9999.
        //              03 XGACONVE                        PIC X.
        //              03 XGANOCAL                        PIC 9.
        //              03 XGAATTIV                        PIC 9.
        //              03 XGAFISSE                        PIC 9.
        //              03 XGAANT46                        PIC 9(3).
        //              03 XGAPOS46                        PIC 9(3).
        //              03 XGARISCU                        PIC 9(3).
        //              03 XGARISCN                        PIC 9(3).
        //              03 XGAINDMM                        PIC 9(3).
        //              03 XGAINDRT                        PIC 9(4)V9999.
        //              03 XGADITTA                        PIC X(4).
        //              03 XGARIDUZ                        PIC 99.
        //              03 XGADIMIS                        PIC 9.
        //              03 XGAPNRID                        PIC 9.
        //              03 XGACONGU                        PIC 9(4)V9999.
        //              03 XGANONVE                        PIC 9.
        //              03 XGASPECI                        PIC X.
        //              03 XGAPROGR                        PIC 99.
        //           02 FILLER                             PIC X(162).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  XGA-RECFOND-BIS REDEFINES COMUNE-RECFOND.
        // D2000     02  XGA-RECFOND.
        /// <summary>
        /// XGATIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string XGATIPOR { get; set; }

        /// <summary>
        /// XGAFONDO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string XGAFONDO { get; set; }

        /// <summary>
        /// XGATPENS 9  
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short XGATPENS { get; set; }

        // 03 XGANATUR.
        /// <summary>
        /// XGANATU1 9  
        /// </summary>
        [HisFieldInfoMapping(3, 1, CobolType = CobolType.Unsigned)]
        public short XGANATU1 { get; set; }

        /// <summary>
        /// XGANATU2 X  
        /// </summary>
        [HisFieldInfoMapping(4, 1)]
        public string XGANATU2 { get; set; }

        /// <summary>
        /// XGANATU3 X  
        /// </summary>
        [HisFieldInfoMapping(5, 1)]
        public string XGANATU3 { get; set; }

        // D2000         03 XGADECOR.
        /// <summary>
        /// XGADECAA 9999  
        /// </summary>
        [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
        public short XGADECAA { get; set; }

        /// <summary>
        /// XGADECMM 99  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short XGADECMM { get; set; }

        // D2000         03 XGASOSPE.
        /// <summary>
        /// XGASOSAA 9999  
        /// </summary>
        [HisFieldInfoMapping(8, 4, CobolType = CobolType.Unsigned)]
        public short XGASOSAA { get; set; }

        /// <summary>
        /// XGASOSMM 99  
        /// </summary>
        [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
        public short XGASOSMM { get; set; }

        // D2000         03 XGAPVERS.
        /// <summary>
        /// XGAPVRAA 9999  
        /// </summary>
        [HisFieldInfoMapping(10, 4, CobolType = CobolType.Unsigned)]
        public short XGAPVRAA { get; set; }

        /// <summary>
        /// XGAPVRMM 99  
        /// </summary>
        [HisFieldInfoMapping(11, 2, CobolType = CobolType.Unsigned)]
        public short XGAPVRMM { get; set; }

        /// <summary>
        /// XGAPVRGG 99  
        /// </summary>
        [HisFieldInfoMapping(12, 2, CobolType = CobolType.Unsigned)]
        public short XGAPVRGG { get; set; }

        // D2000         03 XGAUVERS.
        /// <summary>
        /// XGAUVRAA 9999  
        /// </summary>
        [HisFieldInfoMapping(13, 4, CobolType = CobolType.Unsigned)]
        public short XGAUVRAA { get; set; }

        /// <summary>
        /// XGAUVRMM 99  
        /// </summary>
        [HisFieldInfoMapping(14, 2, CobolType = CobolType.Unsigned)]
        public short XGAUVRMM { get; set; }

        /// <summary>
        /// XGAUVRGG 99  
        /// </summary>
        [HisFieldInfoMapping(15, 2, CobolType = CobolType.Unsigned)]
        public short XGAUVRGG { get; set; }

        /// <summary>
        /// XGAUTIAA 99  
        /// </summary>
        [HisFieldInfoMapping(16, 2, CobolType = CobolType.Unsigned)]
        public short XGAUTIAA { get; set; }

        /// <summary>
        /// XGAUTIMM 99  
        /// </summary>
        [HisFieldInfoMapping(17, 2, CobolType = CobolType.Unsigned)]
        public short XGAUTIMM { get; set; }

        /// <summary>
        /// XGARETPN 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(18, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XGARETPN { get; set; }

        /// <summary>
        /// XGACONVE X  
        /// </summary>
        [HisFieldInfoMapping(19, 1)]
        public string XGACONVE { get; set; }

        /// <summary>
        /// XGANOCAL 9  
        /// </summary>
        [HisFieldInfoMapping(20, 1, CobolType = CobolType.Unsigned)]
        public short XGANOCAL { get; set; }

        /// <summary>
        /// XGAATTIV 9  
        /// </summary>
        [HisFieldInfoMapping(21, 1, CobolType = CobolType.Unsigned)]
        public short XGAATTIV { get; set; }

        /// <summary>
        /// XGAFISSE 9  
        /// </summary>
        [HisFieldInfoMapping(22, 1, CobolType = CobolType.Unsigned)]
        public short XGAFISSE { get; set; }

        /// <summary>
        /// XGAANT46 9(3)  
        /// </summary>
        [HisFieldInfoMapping(23, 3, CobolType = CobolType.Unsigned)]
        public short XGAANT46 { get; set; }

        /// <summary>
        /// XGAPOS46 9(3)  
        /// </summary>
        [HisFieldInfoMapping(24, 3, CobolType = CobolType.Unsigned)]
        public short XGAPOS46 { get; set; }

        /// <summary>
        /// XGARISCU 9(3)  
        /// </summary>
        [HisFieldInfoMapping(25, 3, CobolType = CobolType.Unsigned)]
        public short XGARISCU { get; set; }

        /// <summary>
        /// XGARISCN 9(3)  
        /// </summary>
        [HisFieldInfoMapping(26, 3, CobolType = CobolType.Unsigned)]
        public short XGARISCN { get; set; }

        /// <summary>
        /// XGAINDMM 9(3)  
        /// </summary>
        [HisFieldInfoMapping(27, 3, CobolType = CobolType.Unsigned)]
        public short XGAINDMM { get; set; }

        /// <summary>
        /// XGAINDRT 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(28, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XGAINDRT { get; set; }

        /// <summary>
        /// XGADITTA X(4)  
        /// </summary>
        [HisFieldInfoMapping(29, 4)]
        public string XGADITTA { get; set; }

        /// <summary>
        /// XGARIDUZ 99  
        /// </summary>
        [HisFieldInfoMapping(30, 2, CobolType = CobolType.Unsigned)]
        public short XGARIDUZ { get; set; }

        /// <summary>
        /// XGADIMIS 9  
        /// </summary>
        [HisFieldInfoMapping(31, 1, CobolType = CobolType.Unsigned)]
        public short XGADIMIS { get; set; }

        /// <summary>
        /// XGAPNRID 9  
        /// </summary>
        [HisFieldInfoMapping(32, 1, CobolType = CobolType.Unsigned)]
        public short XGAPNRID { get; set; }

        /// <summary>
        /// XGACONGU 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(33, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XGACONGU { get; set; }

        /// <summary>
        /// XGANONVE 9  
        /// </summary>
        [HisFieldInfoMapping(34, 1, CobolType = CobolType.Unsigned)]
        public short XGANONVE { get; set; }

        /// <summary>
        /// XGASPECI X  
        /// </summary>
        [HisFieldInfoMapping(35, 1)]
        public string XGASPECI { get; set; }

        /// <summary>
        /// XGAPROGR 99  
        /// </summary>
        [HisFieldInfoMapping(36, 2, CobolType = CobolType.Unsigned)]
        public short XGAPROGR { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "GAS"; }
        }
        #endregion Properties
    }
}
