using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago
{
    public class TT : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  YTT-RECAGO-BIS REDEFINES COMUNE-RECAGO.
        //          02  YTT-RECAGO.
        //              03 YTTTIPOR                      PIC X.
        //              03 YTTFONDO                      PIC X(3).
        //              03 YTTTIPEN                      PIC 9.
        //              03 YTTDECAA                      PIC 9999.                
        //              03 YTTDECMM                      PIC 99.
        //              03 YTTSCAAA                      PIC 9999.                
        //              03 YTTSCAMM                      PIC 99.
        //              03 YTTCONTR                      PIC 9(7)V9999.
        //              03 YTTMONTA                      PIC 9(7)V9999.
        //              03 YTTRSETA                      PIC 9(6)V9999.
        //              03 YTTSETTA                      PIC 9(5).
        //              03 YTTRSETB                      PIC 9(6)V9999.
        //              03 YTTSETTB                      PIC 9(5).
        //              03 YTTSETTC                      PIC 9(5).
        //              03 YTTRSETD                      PIC 9(6)V9999.
        //              03 YTTSETTD                      PIC 9(5).
        //              03 YTTTETTO                      PIC 9(6)V9999.
        //              03 YTTDECSS                      PIC 99.
        //              03 YTTSOSSS                      PIC 99.
        //              03 YTTRETRA                      PIC 9(6)V9999.
        //              03 YTTRETTA                      PIC 9(5).
        //              03 YTTRETRB                      PIC 9(6)V9999.
        //              03 YTTRETTB                      PIC 9(5).
        //              03 YTTRETTC                      PIC 9(5).
        //              03 YTTRETRD                      PIC 9(6)V9999.
        //              03 YTTRETTD                      PIC 9(5).
        //GD1009        03 YTTSETTE                      PIC 9(4).  
        //GD0212        03 YTTIMPCRT                     PIC 9(7)V9999.  
        //GD0212        03 YTTMONTA2012                  PIC 9(7)V9999.      
        //GD0212        03 YTTSETT2012                   PIC 9(4).
        //GD1012        03 YTTFLAG214                    PIC X.
        //GD1012        03 YTTPERC214                    PIC 99V99.
        //////////////////////////////////////////////////////////////////
        //              03 YTTIMP707                      PIC 9(7)V9999.
        //              03 YTTSETA707                     PIC 9(4).     
        //              03 YTTSETB707                     PIC 9(4).     
        //              03 YTTSETC707                     PIC 9(4).     
        //              03 YTTSETD707                     PIC 9(4).   
        //              03 YTTCALC707                     PIC X(01).                  
        //////////////////////////////////////////////////////////////////
        //              03 YTTSETDIR                      PIC 9(4).
        //              03 YTTPROGR                      PIC 99.
        //              03 YTTDISPO                      PIC X(186).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  YTT-RECAGO-BIS REDEFINES COMUNE-RECAGO.
        // 02  YTT-RECAGO.
        /// <summary>
        /// YTTTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string YTTTIPOR { get; set; }

        /// <summary>
        /// YTTFONDO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string YTTFONDO { get; set; }

        /// <summary>
        /// YTTTIPEN 9  
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short YTTTIPEN { get; set; }

        /// <summary>
        /// YTTDECAA 9999  
        /// </summary>
        [HisFieldInfoMapping(3, 4, CobolType = CobolType.Unsigned)]
        public short YTTDECAA { get; set; }

        /// <summary>
        /// YTTDECMM 99  
        /// </summary>
        [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
        public short YTTDECMM { get; set; }

        /// <summary>
        /// YTTSCAAA 9999  
        /// </summary>
        [HisFieldInfoMapping(5, 4, CobolType = CobolType.Unsigned)]
        public short YTTSCAAA { get; set; }

        /// <summary>
        /// YTTSCAMM 99  
        /// </summary>
        [HisFieldInfoMapping(6, 2, CobolType = CobolType.Unsigned)]
        public short YTTSCAMM { get; set; }

        /// <summary>
        /// YTTCONTR 9(7)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(7, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YTTCONTR { get; set; }

        /// <summary>
        /// YTTMONTA 9(7)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(8, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YTTMONTA { get; set; }

        /// <summary>
        /// YTTRSETA 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(9, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YTTRSETA { get; set; }

        /// <summary>
        /// YTTSETTA 9(5)  
        /// </summary>
        [HisFieldInfoMapping(10, 5, CobolType = CobolType.Unsigned)]
        public int YTTSETTA { get; set; }

        /// <summary>
        /// YTTRSETB 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(11, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YTTRSETB { get; set; }

        /// <summary>
        /// YTTSETTB 9(5)  
        /// </summary>
        [HisFieldInfoMapping(12, 5, CobolType = CobolType.Unsigned)]
        public int YTTSETTB { get; set; }

        /// <summary>
        /// YTTSETTC 9(5)  
        /// </summary>
        [HisFieldInfoMapping(13, 5, CobolType = CobolType.Unsigned)]
        public int YTTSETTC { get; set; }

        /// <summary>
        /// YTTRSETD 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(14, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YTTRSETD { get; set; }

        /// <summary>
        /// YTTSETTD 9(5)  
        /// </summary>
        [HisFieldInfoMapping(15, 5, CobolType = CobolType.Unsigned)]
        public int YTTSETTD { get; set; }

        /// <summary>
        /// YTTTETTO 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(16, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YTTTETTO { get; set; }

        /// <summary>
        /// YTTDECSS 99  
        /// </summary>
        [HisFieldInfoMapping(17, 2, CobolType = CobolType.Unsigned)]
        public short YTTDECSS { get; set; }

        /// <summary>
        /// YTTSOSSS 99  
        /// </summary>
        [HisFieldInfoMapping(18, 2, CobolType = CobolType.Unsigned)]
        public short YTTSOSSS { get; set; }

        /// <summary>
        /// YTTRETRA 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(19, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YTTRETRA { get; set; }

        /// <summary>
        /// YTTRETTA 9(5)  
        /// </summary>
        [HisFieldInfoMapping(20, 5, CobolType = CobolType.Unsigned)]
        public int YTTRETTA { get; set; }

        /// <summary>
        /// YTTRETRB 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(21, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YTTRETRB { get; set; }

        /// <summary>
        /// YTTRETTB 9(5)  
        /// </summary>
        [HisFieldInfoMapping(22, 5, CobolType = CobolType.Unsigned)]
        public int YTTRETTB { get; set; }

        /// <summary>
        /// YTTRETTC 9(5)  
        /// </summary>
        [HisFieldInfoMapping(23, 5, CobolType = CobolType.Unsigned)]
        public int YTTRETTC { get; set; }

        /// <summary>
        /// YTTRETRD 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(24, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YTTRETRD { get; set; }

        /// <summary>
        /// YTTRETTD 9(5)  
        /// </summary>
        [HisFieldInfoMapping(25, 5, CobolType = CobolType.Unsigned)]
        public int YTTRETTD { get; set; }

        /// <summary>
        /// YTTSETTE 9(4)  
        /// </summary>
        [HisFieldInfoMapping(26, 4, CobolType = CobolType.Unsigned)]
        public short YTTSETTE { get; set; }

        /// <summary>
        /// YTTIMPCRT 9(7)V9999
        /// <summary>
        [HisFieldInfoMapping(27, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YTTIMPCRT { get; set; }

        /// <summary>
        /// YTTMONTA2012 9(7)V9999      
        /// <summary>
        [HisFieldInfoMapping(28, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YTTMONTA2012 { get; set; }

        /// <summary>
        /// YTTSETT2012 9(4)
        /// <summary>
        [HisFieldInfoMapping(29, 4, CobolType = CobolType.Unsigned)]
        public short YTTSETT2012 { get; set; }

        /// <summary>
        /// YTTFLAG214 X
        /// <summary>
        [HisFieldInfoMapping(30, 1)]
        public string YTTFLAG214 { get; set; }

        /// <summary>
        /// YTTPERC214 99V99
        /// <summary>
        [HisFieldInfoMapping(31, 4, Scale = 2, CobolType = CobolType.Unsigned)]
        public decimal YTTPERC214 { get; set; }
             
        /// <summary>
        /// YTTIMP707 9(7)V9999.
        /// </summary>
        [HisFieldInfoMapping(32, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YTTIMP707 { get; set; }

        /// <summary>
        /// YTTSETA707 9(4).
        /// </summary>
        [HisFieldInfoMapping(33, 4, CobolType = CobolType.Unsigned)]
        public short YTTSETA707 { get; set; }

        /// <summary>
        /// YTTSETB707 9(4)
        /// </summary>
        [HisFieldInfoMapping(34, 4, CobolType = CobolType.Unsigned)]
        public short YTTSETB707 { get; set; }

        /// <summary>
        /// YTTSETC707 9(4)
        /// </summary>
        [HisFieldInfoMapping(35, 4, CobolType = CobolType.Unsigned)]
        public short YTTSETC707 { get; set; }

        /// <summary>
        /// YTTSETD707 9(4)
        /// </summary>
        [HisFieldInfoMapping(36, 4, CobolType = CobolType.Unsigned)]
        public short YTTSETD707 { get; set; }
  
        /// <summary>
        /// YTTCALC707 X(01)
        /// </summary>
        [HisFieldInfoMapping(37, 1)]
        public string YTTCALC707 { get; set; }

        /// <summary>
        /// YTTSETDIR 9(4)     
        /// <summary>
        [HisFieldInfoMapping(38, 4, CobolType = CobolType.Unsigned)]
        public int YTTSETDIR { get; set; }

        /// <summary>
        /// YTTPROGR 99  
        /// </summary>
        [HisFieldInfoMapping(39, 2, CobolType = CobolType.Unsigned)]
        public short YTTPROGR { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "TT"; }
        }
        #endregion Properties
    }
}
