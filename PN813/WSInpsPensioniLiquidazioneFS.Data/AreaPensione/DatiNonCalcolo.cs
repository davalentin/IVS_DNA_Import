using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA
{
    public class DatiNonCalcolo : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01 TRW-NOCAL.
        //02  TRWTPREC                     PIC X.
        //02  TRWFONDO                     PIC XXX.
        //02  TRWDECAA                     PIC 9999.                
        //02  TRWDECMM                     PIC 99.
        //02  TRWDECGG                     PIC 99.
        //02  TRWTPVAR                     PIC X.
        //02  TRWFAM01                     PIC X.
        //02  TRWFAM02                     PIC X.
        //02  TRWFAM03                     PIC X.
        //02  TRWFAM04                     PIC X.
        //02  TRWFAM05                     PIC X.
        //02  TRWFAM06                     PIC X.
        //02  TRWFAM07                     PIC X.
        //02  TRWFAM08                     PIC X.
        //02  TRWFAM09                     PIC X.
        //02  TRWFAM10                     PIC X.
        //02  TRWFAM11                     PIC X.
        //02  TRWFAM12                     PIC X.
        //02  TRWFAM13                     PIC X.
        //02  TRWFAM14                     PIC X.
        //02  TRWFAM15                     PIC X.
        //02  TRWCOL01                     PIC 9(6)V9999.
        //02  TRWCOL02                     PIC 9(6)V9999.
        //02  TRWCOL03                     PIC 9(6)V9999.
        //02  TRWCOL04                     PIC 9(6)V9999.
        //02  TRWCOL05                     PIC 9(6)V9999.
        //02  TRWCOL06                     PIC 9(6)V9999.
        //02  TRWCOL07                     PIC 9(6)V9999.
        //02  TRWCOL08                     PIC 9(6)V9999.
        //02  TRWCOL09                     PIC 9(6)V9999.
        //02  TRWCOL10                     PIC 9(6)V9999.
        //02  TRWCOL11                     PIC 9(6)V9999.
        //02  TRWCOL12                     PIC 9(6)V9999.
        //02  TRWCOL13                     PIC 9(6)V9999.
        //02  TRWCOL14                     PIC 9(6)V9999.
        //02  TRWEURO                      PIC 9(5)V9(4) COMP-3.
        //02  TRWABC                       PIC 9(3)V9(2) COMP-3.
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01 TRW-NOCAL.
        /// <summary>
        /// TRWTPREC X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string TRWTPREC { get; set; }

        /// <summary>
        /// TRWFONDO XXX  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string TRWFONDO { get; set; }

        /// <summary>
        /// TRWDECAA 9999  
        /// </summary>
        [HisFieldInfoMapping(2, 4, CobolType = CobolType.Unsigned)]
        public short TRWDECAA { get; set; }

        /// <summary>
        /// TRWDECMM 99  
        /// </summary>
        [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
        public short TRWDECMM { get; set; }

        /// <summary>
        /// TRWDECGG 99  
        /// </summary>
        [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
        public short TRWDECGG { get; set; }

        /// <summary>
        /// TRWTPVAR X  
        /// </summary>
        [HisFieldInfoMapping(5, 1)]
        public string TRWTPVAR { get; set; }

        /// <summary>
        /// TRWFAM01 X  
        /// </summary>
        [HisFieldInfoMapping(6, 1)]
        public string TRWFAM01 { get; set; }

        /// <summary>
        /// TRWFAM02 X  
        /// </summary>
        [HisFieldInfoMapping(7, 1)]
        public string TRWFAM02 { get; set; }

        /// <summary>
        /// TRWFAM03 X  
        /// </summary>
        [HisFieldInfoMapping(8, 1)]
        public string TRWFAM03 { get; set; }

        /// <summary>
        /// TRWFAM04 X  
        /// </summary>
        [HisFieldInfoMapping(9, 1)]
        public string TRWFAM04 { get; set; }

        /// <summary>
        /// TRWFAM05 X  
        /// </summary>
        [HisFieldInfoMapping(10, 1)]
        public string TRWFAM05 { get; set; }

        /// <summary>
        /// TRWFAM06 X  
        /// </summary>
        [HisFieldInfoMapping(11, 1)]
        public string TRWFAM06 { get; set; }

        /// <summary>
        /// TRWFAM07 X  
        /// </summary>
        [HisFieldInfoMapping(12, 1)]
        public string TRWFAM07 { get; set; }

        /// <summary>
        /// TRWFAM08 X  
        /// </summary>
        [HisFieldInfoMapping(13, 1)]
        public string TRWFAM08 { get; set; }

        /// <summary>
        /// TRWFAM09 X  
        /// </summary>
        [HisFieldInfoMapping(14, 1)]
        public string TRWFAM09 { get; set; }

        /// <summary>
        /// TRWFAM10 X  
        /// </summary>
        [HisFieldInfoMapping(15, 1)]
        public string TRWFAM10 { get; set; }

        /// <summary>
        /// TRWFAM11 X  
        /// </summary>
        [HisFieldInfoMapping(16, 1)]
        public string TRWFAM11 { get; set; }

        /// <summary>
        /// TRWFAM12 X  
        /// </summary>
        [HisFieldInfoMapping(17, 1)]
        public string TRWFAM12 { get; set; }

        /// <summary>
        /// TRWFAM13 X  
        /// </summary>
        [HisFieldInfoMapping(18, 1)]
        public string TRWFAM13 { get; set; }

        /// <summary>
        /// TRWFAM14 X  
        /// </summary>
        [HisFieldInfoMapping(19, 1)]
        public string TRWFAM14 { get; set; }

        /// <summary>
        /// TRWFAM15 X  
        /// </summary>
        [HisFieldInfoMapping(20, 1)]
        public string TRWFAM15 { get; set; }

        /// <summary>
        /// TRWCOL01 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(21, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal TRWCOL01 { get; set; }

        /// <summary>
        /// TRWCOL02 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(22, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal TRWCOL02 { get; set; }

        /// <summary>
        /// TRWCOL03 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(23, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal TRWCOL03 { get; set; }

        /// <summary>
        /// TRWCOL04 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(24, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal TRWCOL04 { get; set; }

        /// <summary>
        /// TRWCOL05 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(25, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal TRWCOL05 { get; set; }

        /// <summary>
        /// TRWCOL06 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(26, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal TRWCOL06 { get; set; }

        /// <summary>
        /// TRWCOL07 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(27, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal TRWCOL07 { get; set; }

        /// <summary>
        /// TRWCOL08 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(28, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal TRWCOL08 { get; set; }

        /// <summary>
        /// TRWCOL09 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(29, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal TRWCOL09 { get; set; }

        /// <summary>
        /// TRWCOL10 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(30, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal TRWCOL10 { get; set; }

        /// <summary>
        /// TRWCOL11 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(31, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal TRWCOL11 { get; set; }

        /// <summary>
        /// TRWCOL12 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(32, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal TRWCOL12 { get; set; }

        /// <summary>
        /// TRWCOL13 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(33, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal TRWCOL13 { get; set; }

        /// <summary>
        /// TRWCOL14 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(34, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal TRWCOL14 { get; set; }

        /// <summary>
        //TRWEURO 9(5)V9(4) COMP-3.
        /// <summary>
        [HisFieldInfoMapping(35, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal TRWEURO { get; set; }

        /// <summary>
        //TRWABC 9(3)V9(2) COMP-3.
        /// <summary>
        [HisFieldInfoMapping(36, 3, Scale = 2, CobolType = CobolType.Comp3Unsigned)]
        public decimal TRWABC { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "DatiNonCalcolo"; }
        }
        #endregion Properties
    }
}
