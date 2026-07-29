using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo
{
    public class CL : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  XCL-RECFOND-BIS REDEFINES COMUNE-RECFOND.
        //02  XCL-RECFOND.
        //    03 XCLTIPOR                      PIC X.
        //    03 XCLFONDO                      PIC X(3).
        //    03 XCLTPENS                      PIC 9.
        //    03 XCLNATUR                      PIC 9.
        //    03 XCLDECOR.
        //       04 XCLDECAA                   PIC 9999.                
        //       04 XCLDECMM                   PIC 99.
        //    03 XCLSCADE.
        //       04 XCLSCAAA                   PIC 9999.                
        //       04 XCLSCAMM                   PIC 99.
        //    03 XCLPVERS.
        //       04 XCLPVRAA                   PIC 9999.                
        //       04 XCLPVRMM                   PIC 99.
        //       04 XCLPVRGG                   PIC 99.
        //    03 XCLUVERS.
        //       04 XCLUVRAA                   PIC 9999.                
        //       04 XCLUVRMM                   PIC 99.
        //       04 XCLUVRGG                   PIC 99.
        //    03 XCLUTIAA                        PIC 99.
        //    03 XCLUTIMM                        PIC 99.
        //    03 XCLVITAL                        PIC 9(4)V9999.
        //    03 XCLNOREQ                        PIC 9.
        //    03 XCLDIFFE                        PIC 99.
        //    03 XCLPERFE                        PIC 99.
        //    03 XCLATTIV                        PIC 9.
        //    03 XCLNONCA                        PIC 9.
        //    03 XCLNONVE                        PIC 9.
        //    03 XCLNAFIL                        PIC XX.
        //    03 XCLREQU1                        PIC X.
        //    03 XCLREQU2                        PIC 9.
        //    03 XCLMMREQ                        PIC 99.
        //    03 XCLAAREQ                        PIC 9(4).
        //    03 XCLPROGR                        PIC 99.
        //    03 XCLCONTR-PROV                   PIC X(01).
        // 02 FILLER                             PIC X(192).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  XCL-RECFOND-BIS REDEFINES COMUNE-RECFOND.
        // 02  XCL-RECFOND.
        /// <summary>
        /// XCLTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string XCLTIPOR { get; set; }

        /// <summary>
        /// XCLFONDO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string XCLFONDO { get; set; }

        /// <summary>
        /// XCLTPENS 9  
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short XCLTPENS { get; set; }

        /// <summary>
        /// XCLNATUR 9  
        /// </summary>
        [HisFieldInfoMapping(3, 1, CobolType = CobolType.Unsigned)]
        public short XCLNATUR { get; set; }

        // 03 XCLDECOR.
        /// <summary>
        /// XCLDECAA 9999  
        /// </summary>
        [HisFieldInfoMapping(4, 4, CobolType = CobolType.Unsigned)]
        public short XCLDECAA { get; set; }

        /// <summary>
        /// XCLDECMM 99  
        /// </summary>
        [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
        public short XCLDECMM { get; set; }

        // 03 XCLSCADE.
        /// <summary>
        /// XCLSCAAA 9999  
        /// </summary>
        [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
        public short XCLSCAAA { get; set; }

        /// <summary>
        /// XCLSCAMM 99  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short XCLSCAMM { get; set; }

        // 03 XCLPVERS.
        /// <summary>
        /// XCLPVRAA 9999  
        /// </summary>
        [HisFieldInfoMapping(8, 4, CobolType = CobolType.Unsigned)]
        public short XCLPVRAA { get; set; }

        /// <summary>
        /// XCLPVRMM 99  
        /// </summary>
        [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
        public short XCLPVRMM { get; set; }

        /// <summary>
        /// XCLPVRGG 99  
        /// </summary>
        [HisFieldInfoMapping(10, 2, CobolType = CobolType.Unsigned)]
        public short XCLPVRGG { get; set; }

        // 03 XCLUVERS.
        /// <summary>
        /// XCLUVRAA 9999  
        /// </summary>
        [HisFieldInfoMapping(11, 4, CobolType = CobolType.Unsigned)]
        public short XCLUVRAA { get; set; }

        /// <summary>
        /// XCLUVRMM 99  
        /// </summary>
        [HisFieldInfoMapping(12, 2, CobolType = CobolType.Unsigned)]
        public short XCLUVRMM { get; set; }

        /// <summary>
        /// XCLUVRGG 99  
        /// </summary>
        [HisFieldInfoMapping(13, 2, CobolType = CobolType.Unsigned)]
        public short XCLUVRGG { get; set; }

        /// <summary>
        /// XCLUTIAA 99  
        /// </summary>
        [HisFieldInfoMapping(14, 2, CobolType = CobolType.Unsigned)]
        public short XCLUTIAA { get; set; }

        /// <summary>
        /// XCLUTIMM 99  
        /// </summary>
        [HisFieldInfoMapping(15, 2, CobolType = CobolType.Unsigned)]
        public short XCLUTIMM { get; set; }

        /// <summary>
        /// XCLVITAL 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(16, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XCLVITAL { get; set; }

        /// <summary>
        /// XCLNOREQ 9  
        /// </summary>
        [HisFieldInfoMapping(17, 1, CobolType = CobolType.Unsigned)]
        public short XCLNOREQ { get; set; }

        /// <summary>
        /// XCLDIFFE 99  
        /// </summary>
        [HisFieldInfoMapping(18, 2, CobolType = CobolType.Unsigned)]
        public short XCLDIFFE { get; set; }

        /// <summary>
        /// XCLPERFE 99  
        /// </summary>
        [HisFieldInfoMapping(19, 2, CobolType = CobolType.Unsigned)]
        public short XCLPERFE { get; set; }

        /// <summary>
        /// XCLATTIV 9  
        /// </summary>
        [HisFieldInfoMapping(20, 1, CobolType = CobolType.Unsigned)]
        public short XCLATTIV { get; set; }

        /// <summary>
        /// XCLNONCA 9  
        /// </summary>
        [HisFieldInfoMapping(21, 1, CobolType = CobolType.Unsigned)]
        public short XCLNONCA { get; set; }

        /// <summary>
        /// XCLNONVE 9  
        /// </summary>
        [HisFieldInfoMapping(22, 1, CobolType = CobolType.Unsigned)]
        public short XCLNONVE { get; set; }

        /// <summary>
        /// XCLNAFIL XX  
        /// </summary>
        [HisFieldInfoMapping(23, 2)]
        public string XCLNAFIL { get; set; }

        /// <summary>
        /// XCLREQU1 X  
        /// </summary>
        [HisFieldInfoMapping(24, 1)]
        public string XCLREQU1 { get; set; }

        /// <summary>
        /// XCLREQU2 9  
        /// </summary>
        [HisFieldInfoMapping(25, 1, CobolType = CobolType.Unsigned)]
        public short XCLREQU2 { get; set; }

        /// <summary>
        /// XCLMMREQ 99  
        /// </summary>
        [HisFieldInfoMapping(26, 2, CobolType = CobolType.Unsigned)]
        public short XCLMMREQ { get; set; }

        /// <summary>
        /// XCLAAREQ 9(4)  
        /// </summary>
        [HisFieldInfoMapping(27, 4, CobolType = CobolType.Unsigned)]
        public short XCLAAREQ { get; set; }

        /// <summary>
        /// XCLCONTR_PROV X  
        /// </summary>
        [HisFieldInfoMapping(28, 1)]
        public string XCLCONTR_PROV { get; set; }

        /// <summary>
        /// XCLPROGR 99  
        /// </summary>
        [HisFieldInfoMapping(29, 2, CobolType = CobolType.Unsigned)]
        public short XCLPROGR { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "CL"; }
        }
        #endregion Properties
    }
}
