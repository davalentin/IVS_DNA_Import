using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago
{
    public class EL : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  YEL-RECAGO-BIS REDEFINES COMUNE-RECAGO.
        //D2000     02  YEL-RECAGO.
        //              03 YELTIPOR                      PIC X.
        //              03 YELFONDO                      PIC X(3).
        //              03 YELTIPEN                      PIC 9.
        //              03 YELDECAA                      PIC 9999.                
        //              03 YELDECMM                      PIC 99.
        //              03 YELSCAAA                      PIC 9999.                
        //              03 YELSCAMM                      PIC 99.
        //              03 YELCONTR                      PIC 9(6)V9999.
        //              03 YELMONTA                      PIC 9(7)V9999.
        //              03 YELRSETA                      PIC 9(6)V9999.
        //              03 YELSETTA                      PIC 9(5).
        //              03 YELRSETB                      PIC 9(6)V9999.
        //              03 YELSETTB                      PIC 9(5).
        //              03 YELSETTC                      PIC 9(5).
        //              03 YELRSETD                      PIC 9(6)V9999.
        //              03 YELSETTD                      PIC 9(5).
        //              03 YELTETTO                      PIC 9(6)V9999.
        //              03 YELDECSS                      PIC 99.
        //              03 YELSOSSS                      PIC 99.
        //              03 YELRETRA                      PIC 9(6)V9999.
        //              03 YELRETTA                      PIC 9(5).
        //              03 YELRETRB                      PIC 9(6)V9999.
        //              03 YELRETTB                      PIC 9(5).
        //              03 YELRETTC                      PIC 9(5).
        //              03 YELRETRD                      PIC 9(6)V9999.
        //              03 YELRETTD                      PIC 9(5).
        //GD1009        03 YELSETTE                      PIC 9(4).
        //GD0212        03 YELIMPCRT                     PIC 9(6)V9999.  
        //GD0212        03 YELMONTA2012                  PIC 9(7)V9999.      
        //GD0212        03 YELSETT2012                   PIC 9(4).  
        //GD1012        03 YELFLAG214                    PIC X.
        //GD1012        03 YELPERC214                    PIC 99V99.
        //GD0316* MODIFICHE PER LEGGE COMMA 707 (doppio calcolo)             
        //              03 YELIMP707                     PIC 9(7)V9999.     
        //              03 YELSETA707                    PIC 9(4).          
        //              03 YELSETB707                    PIC 9(4).          
        //              03 YELSETC707                    PIC 9(4).          
        //              03 YELSETD707                    PIC 9(4).          
        //              03 YELCALC707                    PIC X(01).
        //              03 YELSETDIR                     PIC 9(4).    
        //              03 YELPROGR                      PIC 99.
        //            02 YELDISPO                        PIC X(188).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  YEL-RECAGO-BIS REDEFINES COMUNE-RECAGO.
        // D2000     02  YEL-RECAGO.
        /// <summary>
        /// YELTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string YELTIPOR { get; set; }

        /// <summary>
        /// YELFONDO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string YELFONDO { get; set; }

        /// <summary>
        /// YELTIPEN 9  
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short YELTIPEN { get; set; }

        /// <summary>
        /// YELDECAA 9999  
        /// </summary>
        [HisFieldInfoMapping(3, 4, CobolType = CobolType.Unsigned)]
        public short YELDECAA { get; set; }

        /// <summary>
        /// YELDECMM 99  
        /// </summary>
        [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
        public short YELDECMM { get; set; }

        /// <summary>
        /// YELSCAAA 9999  
        /// </summary>
        [HisFieldInfoMapping(5, 4, CobolType = CobolType.Unsigned)]
        public short YELSCAAA { get; set; }

        /// <summary>
        /// YELSCAMM 99  
        /// </summary>
        [HisFieldInfoMapping(6, 2, CobolType = CobolType.Unsigned)]
        public short YELSCAMM { get; set; }

        /// <summary>
        /// YELCONTR 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(7, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YELCONTR { get; set; }

        /// <summary>
        /// YELMONTA 9(7)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(8, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YELMONTA { get; set; }

        /// <summary>
        /// YELRSETA 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(9, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YELRSETA { get; set; }

        /// <summary>
        /// YELSETTA 9(5)  
        /// </summary>
        [HisFieldInfoMapping(10, 5, CobolType = CobolType.Unsigned)]
        public int YELSETTA { get; set; }

        /// <summary>
        /// YELRSETB 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(11, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YELRSETB { get; set; }

        /// <summary>
        /// YELSETTB 9(5)  
        /// </summary>
        [HisFieldInfoMapping(12, 5, CobolType = CobolType.Unsigned)]
        public int YELSETTB { get; set; }

        /// <summary>
        /// YELSETTC 9(5)  
        /// </summary>
        [HisFieldInfoMapping(13, 5, CobolType = CobolType.Unsigned)]
        public int YELSETTC { get; set; }

        /// <summary>
        /// YELRSETD 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(14, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YELRSETD { get; set; }

        /// <summary>
        /// YELSETTD 9(5)  
        /// </summary>
        [HisFieldInfoMapping(15, 5, CobolType = CobolType.Unsigned)]
        public int YELSETTD { get; set; }

        /// <summary>
        /// YELTETTO 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(16, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YELTETTO { get; set; }

        /// <summary>
        /// YELDECSS 99  
        /// </summary>
        [HisFieldInfoMapping(17, 2, CobolType = CobolType.Unsigned)]
        public short YELDECSS { get; set; }

        /// <summary>
        /// YELSOSSS 99  
        /// </summary>
        [HisFieldInfoMapping(18, 2, CobolType = CobolType.Unsigned)]
        public short YELSOSSS { get; set; }

        /// <summary>
        /// YELRETRA 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(19, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YELRETRA { get; set; }

        /// <summary>
        /// YELRETTA 9(5)  
        /// </summary>
        [HisFieldInfoMapping(20, 5, CobolType = CobolType.Unsigned)]
        public int YELRETTA { get; set; }

        /// <summary>
        /// YELRETRB 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(21, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YELRETRB { get; set; }

        /// <summary>
        /// YELRETTB 9(5)  
        /// </summary>
        [HisFieldInfoMapping(22, 5, CobolType = CobolType.Unsigned)]
        public int YELRETTB { get; set; }

        /// <summary>
        /// YELRETTC 9(5)  
        /// </summary>
        [HisFieldInfoMapping(23, 5, CobolType = CobolType.Unsigned)]
        public int YELRETTC { get; set; }

        /// <summary>
        /// YELRETRD 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(24, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YELRETRD { get; set; }

        /// <summary>
        /// YELRETTD 9(5)  
        /// </summary>
        [HisFieldInfoMapping(25, 5, CobolType = CobolType.Unsigned)]
        public int YELRETTD { get; set; }

        /// <summary>
        /// YELSETTE 9(4)  
        /// </summary>
        [HisFieldInfoMapping(26, 4, CobolType = CobolType.Unsigned)]
        public short YELSETTE { get; set; }

        /// <summary>
        /// YELIMPCRT 9(6)V9999  
        /// <summary>
        [HisFieldInfoMapping(27, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YELIMPCRT { get; set; }

        /// <summary>
        /// YELMONTA2012 9(7)V9999      
        /// <summary>
        [HisFieldInfoMapping(28, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YELMONTA2012 { get; set; }

        /// <summary>
        /// YELSETT2012 9(4)  
        /// <summary>
        [HisFieldInfoMapping(29, 4, CobolType = CobolType.Unsigned)]
        public short YELSETT2012 { get; set; }

        /// <summary>
        /// YELFLAG214 X
        /// <summary>
        [HisFieldInfoMapping(30, 1)]
        public string YELFLAG214 { get; set; }

        /// <summary>
        /// YELPERC214 99V99
        /// <summary>
        [HisFieldInfoMapping(31, 4, Scale = 2, CobolType = CobolType.Unsigned)]
        public decimal YELPERC214 { get; set; }

        /// <summary>
        /// YELIMP707 9(7)V9999
        /// <summary>
        [HisFieldInfoMapping(32, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YELIMP707 { get; set; }
        
        /// <summary>
        /// YELSETA707 9(4)  
        /// </summary>
        [HisFieldInfoMapping(33, 4, CobolType = CobolType.Unsigned)]
        public short YELSETA707 { get; set; }
           
        /// <summary>
        /// YELSETB707 9(4)  
        /// </summary>
        [HisFieldInfoMapping(34, 4, CobolType = CobolType.Unsigned)]
        public short YELSETB707 { get; set; }

        /// <summary>
        /// YELSETC707 9(4)  
        /// </summary>
        [HisFieldInfoMapping(35, 4, CobolType = CobolType.Unsigned)]
        public short YELSETC707 { get; set; }

        /// <summary>
        /// YELSETD707 9(4)  
        /// </summary>
        [HisFieldInfoMapping(36, 4, CobolType = CobolType.Unsigned)]
        public short YELSETD707 { get; set; }
           
        /// <summary>
        /// YELCALC707 X(01)  
        /// </summary>
        [HisFieldInfoMapping(37, 1)]
        public string YELCALC707 { get; set; }

        /// <summary>
        /// YELSETDIR 9(4)     
        /// <summary>
        [HisFieldInfoMapping(38, 4, CobolType = CobolType.Unsigned)]
        public int YELSETDIR { get; set; }

        /// <summary>
        /// YELPROGR 99  
        /// </summary>
        [HisFieldInfoMapping(39, 2, CobolType = CobolType.Unsigned)]
        public short YELPROGR { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "EL"; }
        }
        #endregion Properties
    }
}
