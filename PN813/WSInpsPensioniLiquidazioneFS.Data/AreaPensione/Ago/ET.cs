using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago
{
    public class ET : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  YET-RECAGO-BIS REDEFINES COMUNE-RECAGO.
        //          02  YET-RECAGO.
        //              03 YETTIPOR                      PIC X.
        //              03 YETFONDO                      PIC X(3).
        //              03 YETCATEG                      PIC X(3).
        //              03 YETCERTI                      PIC 9(8).
        //              03 YETBASEA                      PIC 9(5)V99999.
        //              03 YETTIPLQ                      PIC 9.
        //              03 YETDECAA                      PIC 9999.                
        //              03 YETDECMM                      PIC 99.
        //              03 YETSCAAA                      PIC 9999.                
        //              03 YETSCAMM                      PIC 99.
        //              03 YETORIAA                      PIC 9999.                
        //              03 YETORIMM                      PIC 99.
        //              03 YETMEDIM                      PIC 9(5)V9999.
        //              03 YETANZTO                      PIC 9(3).
        //              03 YETRIVPR                      PIC 9(3).
        //D2NEW         03 YETSP1AA                      PIC 9999.                
        //              03 YETSP1MM                      PIC 99.
        //              03 YETSP1CT                      PIC 9(5)V9999.
        //D2NEW         03 YETSP2AA                      PIC 9999.                
        //              03 YETSP2MM                      PIC 99.
        //              03 YETSP2CT                      PIC 9(5)V9999.
        //D2NEW         03 YETCB140                      PIC 9(6).  
        //              03 YETCONTR                      PIC 9(6)V9999.
        //              03 YETMONTA                      PIC 9(7)V9999.
        //              03 YETRSETA                      PIC 9(6)V9999.
        //              03 YETSETTA                      PIC 9(5).
        //              03 YETRSETB                      PIC 9(6)V9999.
        //              03 YETSETTB                      PIC 9(5).
        //              03 YETSETTC                      PIC 9(5).
        //              03 YETRSETD                      PIC 9(6)V9999.
        //              03 YETSETTD                      PIC 9(5).
        //              03 YETTETTO                      PIC 9(6)V9999.
        //              03 YETDECSS                      PIC 99.
        //              03 YETSOSSS                      PIC 99.
        //              03 YETRETRA                      PIC 9(6)V9999.
        //              03 YETRETTA                      PIC 9(5).
        //              03 YETRETRB                      PIC 9(6)V9999.
        //              03 YETRETTB                      PIC 9(5).
        //              03 YETRETTC                      PIC 9(5).
        //              03 YETRETRD                      PIC 9(6)V9999.
        //              03 YETRETTD                      PIC 9(5).
        //GD1009        03 YETSETTE                      PIC 9(4).
        //GD0212        03 YETIMPCRT                     PIC 9(6)V9999.  
        //GD0212        03 YETMONTA2012                  PIC 9(7)V9999.      
        //GD0212        03 YETSETT2012                   PIC 9(4).  
        //GD1012        03 YETFLAG214                    PIC X.
        //GD1012        03 YETPERC214                    PIC 99V99.
        //GD0616* MODIFICHE PER LEGGE COMMA 707 (doppio calcolo)             
        //              03 YETIMP707                      PIC 9(7)V9999.     
        //              03 YETSETAFAA707                  PIC 9(2). 
        //              03 YETSETAFMM707                  PIC 9(2). 
        //              03 YETSETAFGG707                  PIC 9(2).   
        //              03 YETSETBFAA707                  PIC 9(2).  
        //              03 YETSETBFMM707                  PIC 9(2). 
        //              03 YETSETBFGG707                  PIC 9(2).  
        //              03 YETSETCFAA707                  PIC 9(2).  
        //              03 YETSETCFMM707                  PIC 9(2). 
        //              03 YETSETCFGG707                  PIC 9(2). 
        //              03 YETSETAGOA707                  PIC 9(4).          
        //              03 YETSETAGOB707                  PIC 9(4).          
        //              03 YETCALC707                     PIC X(01).
        //              03 YETSETDIR                      PIC 9(4).
        //              03 YETPROGR                       PIC 99.
        //           02 YETDISPO                           PIC X(100).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  YET-RECAGO-BIS REDEFINES COMUNE-RECAGO.
        // 02  YET-RECAGO.
        /// <summary>
        /// YETTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string YETTIPOR { get; set; }

        /// <summary>
        /// YETFONDO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string YETFONDO { get; set; }

        /// <summary>
        /// YETCATEG X(3)  
        /// </summary>
        [HisFieldInfoMapping(2, 3)]
        public string YETCATEG { get; set; }

        /// <summary>
        /// YETCERTI 9(8)  
        /// </summary>
        [HisFieldInfoMapping(3, 8, CobolType = CobolType.Unsigned)]
        public int YETCERTI { get; set; }

        /// <summary>
        /// YETBASEA 9(5)V9(4)9  
        /// </summary>
        [HisFieldInfoMapping(4, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YETBASEA { get; set; }

        /// <summary>
        /// YETTIPLQ 9  
        /// </summary>
        [HisFieldInfoMapping(5, 1, CobolType = CobolType.Unsigned)]
        public short YETTIPLQ { get; set; }

        /// <summary>
        /// YETDECAA 9999  
        /// </summary>
        [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
        public short YETDECAA { get; set; }

        /// <summary>
        /// YETDECMM 99  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short YETDECMM { get; set; }

        /// <summary>
        /// YETSCAAA 9999  
        /// </summary>
        [HisFieldInfoMapping(8, 4, CobolType = CobolType.Unsigned)]
        public short YETSCAAA { get; set; }

        /// <summary>
        /// YETSCAMM 99  
        /// </summary>
        [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
        public short YETSCAMM { get; set; }

        /// <summary>
        /// YETORIAA 9999  
        /// </summary>
        [HisFieldInfoMapping(10, 4, CobolType = CobolType.Unsigned)]
        public short YETORIAA { get; set; }

        /// <summary>
        /// YETORIMM 99  
        /// </summary>
        [HisFieldInfoMapping(11, 2, CobolType = CobolType.Unsigned)]
        public short YETORIMM { get; set; }

        /// <summary>
        /// YETMEDIM 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(12, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YETMEDIM { get; set; }

        /// <summary>
        /// YETANZTO 9(3)  
        /// </summary>
        [HisFieldInfoMapping(13, 3, CobolType = CobolType.Unsigned)]
        public short YETANZTO { get; set; }

        /// <summary>
        /// YETRIVPR 9(3)  
        /// </summary>
        [HisFieldInfoMapping(14, 3, CobolType = CobolType.Unsigned)]
        public short YETRIVPR { get; set; }

        /// <summary>
        /// YETSP1AA 9999  
        /// </summary>
        [HisFieldInfoMapping(15, 4, CobolType = CobolType.Unsigned)]
        public short YETSP1AA { get; set; }

        /// <summary>
        /// YETSP1MM 99  
        /// </summary>
        [HisFieldInfoMapping(16, 2, CobolType = CobolType.Unsigned)]
        public short YETSP1MM { get; set; }

        /// <summary>
        /// YETSP1CT 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(17, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YETSP1CT { get; set; }

        /// <summary>
        /// YETSP2AA 9999  
        /// </summary>
        [HisFieldInfoMapping(18, 4, CobolType = CobolType.Unsigned)]
        public short YETSP2AA { get; set; }

        /// <summary>
        /// YETSP2MM 99  
        /// </summary>
        [HisFieldInfoMapping(19, 2, CobolType = CobolType.Unsigned)]
        public short YETSP2MM { get; set; }

        /// <summary>
        /// YETSP2CT 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(20, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YETSP2CT { get; set; }

        /// <summary>
        /// YETCB140 9(6)  
        /// </summary>
        [HisFieldInfoMapping(21, 6, CobolType = CobolType.Unsigned)]
        public int YETCB140 { get; set; }

        /// <summary>
        /// YETCONTR 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(22, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YETCONTR { get; set; }

        /// <summary>
        /// YETMONTA 9(7)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(23, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YETMONTA { get; set; }

        /// <summary>
        /// YETRSETA 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(24, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YETRSETA { get; set; }

        /// <summary>
        /// YETSETTA 9(5)  
        /// </summary>
        [HisFieldInfoMapping(25, 5, CobolType = CobolType.Unsigned)]
        public int YETSETTA { get; set; }

        /// <summary>
        /// YETRSETB 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(26, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YETRSETB { get; set; }

        /// <summary>
        /// YETSETTB 9(5)  
        /// </summary>
        [HisFieldInfoMapping(27, 5, CobolType = CobolType.Unsigned)]
        public int YETSETTB { get; set; }

        /// <summary>
        /// YETSETTC 9(5)  
        /// </summary>
        [HisFieldInfoMapping(28, 5, CobolType = CobolType.Unsigned)]
        public int YETSETTC { get; set; }

        /// <summary>
        /// YETRSETD 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(29, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YETRSETD { get; set; }

        /// <summary>
        /// YETSETTD 9(5)  
        /// </summary>
        [HisFieldInfoMapping(30, 5, CobolType = CobolType.Unsigned)]
        public int YETSETTD { get; set; }

        /// <summary>
        /// YETTETTO 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(31, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YETTETTO { get; set; }

        /// <summary>
        /// YETDECSS 99  
        /// </summary>
        [HisFieldInfoMapping(32, 2, CobolType = CobolType.Unsigned)]
        public short YETDECSS { get; set; }

        /// <summary>
        /// YETSOSSS 99  
        /// </summary>
        [HisFieldInfoMapping(33, 2, CobolType = CobolType.Unsigned)]
        public short YETSOSSS { get; set; }

        /// <summary>
        /// YETRETRA 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(34, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YETRETRA { get; set; }

        /// <summary>
        /// YETRETTA 9(5)  
        /// </summary>
        [HisFieldInfoMapping(35, 5, CobolType = CobolType.Unsigned)]
        public int YETRETTA { get; set; }

        /// <summary>
        /// YETRETRB 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(36, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YETRETRB { get; set; }

        /// <summary>
        /// YETRETTB 9(5)  
        /// </summary>
        [HisFieldInfoMapping(37, 5, CobolType = CobolType.Unsigned)]
        public int YETRETTB { get; set; }

        /// <summary>
        /// YETRETTC 9(5)  
        /// </summary>
        [HisFieldInfoMapping(38, 5, CobolType = CobolType.Unsigned)]
        public int YETRETTC { get; set; }

        /// <summary>
        /// YETRETRD 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(39, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YETRETRD { get; set; }

        /// <summary>
        /// YETRETTD 9(5)  
        /// </summary>
        [HisFieldInfoMapping(40, 5, CobolType = CobolType.Unsigned)]
        public int YETRETTD { get; set; }

        /// <summary>
        /// YETSETTE 9(4)  
        /// </summary>
        [HisFieldInfoMapping(41, 4, CobolType = CobolType.Unsigned)]
        public short YETSETTE { get; set; }
        
        /// <summary>
        /// YETIMPCRT 9(6)V9999
        /// <summary>
        [HisFieldInfoMapping(42, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YETIMPCRT { get; set; }
        
        /// <summary>
        /// YETMONTA2012 9(7)V9999    
        /// <summary>
        [HisFieldInfoMapping(43, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YETMONTA2012 { get; set; }
        
        /// <summary>
        /// YETSETT2012 9(4)
        /// <summary>
        [HisFieldInfoMapping(44, 4, CobolType = CobolType.Unsigned)]
        public short YETSETT2012 { get; set; }
        
        /// <summary>
        /// YETFLAG214 X
        /// <summary>
        [HisFieldInfoMapping(45, 1)]
        public string YETFLAG214 { get; set; }
        
        /// <summary>
        /// YETPERC214 99V99
        /// <summary>
        [HisFieldInfoMapping(46, 4, Scale = 2, CobolType = CobolType.Unsigned)]
        public decimal YETPERC214 { get; set; }

        /// <summary>
        /// YETIMP707 9(7)V9999 
        /// <summary>
        [HisFieldInfoMapping(47, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YETIMP707 { get; set; }
        
        /// <summary>
        /// YETSETAFAA707 9(2)
        /// <summary>
        [HisFieldInfoMapping(48, 2, CobolType = CobolType.Unsigned)]
        public short YETSETAFAA707 { get; set; }

        /// <summary>
        /// YETSETAFMM707 9(2)
        /// <summary>
        [HisFieldInfoMapping(49, 2, CobolType = CobolType.Unsigned)]
        public short YETSETAFMM707 { get; set; }

        /// <summary>
        /// YETSETAFGG707 9(2)   
        /// <summary>
        [HisFieldInfoMapping(50, 2, CobolType = CobolType.Unsigned)]
        public short YETSETAFGG707 { get; set; }

        /// <summary>
        /// YETSETBFAA707 9(2)
        /// <summary>
        [HisFieldInfoMapping(51, 2, CobolType = CobolType.Unsigned)]
        public short YETSETBFAA707 { get; set; }

        /// <summary>
        /// YETSETBFMM707 PIC 9(2) 
        /// <summary>
        [HisFieldInfoMapping(52, 2, CobolType = CobolType.Unsigned)]
        public short YETSETBFMM707 { get; set; }
        
        /// <summary>
        /// YETSETBFGG707 PIC 9(2) 
        /// <summary>
        [HisFieldInfoMapping(53, 2, CobolType = CobolType.Unsigned)]
        public short YETSETBFGG707 { get; set; }
        
        /// <summary>
        /// YETSETCFAA707 9(2) 
        /// <summary>
        [HisFieldInfoMapping(54, 2, CobolType = CobolType.Unsigned)]
        public short YETSETCFAA707 { get; set; }
        
        /// <summary>
        /// YETSETCFMM707 9(2)
        /// <summary>
        [HisFieldInfoMapping(55, 2, CobolType = CobolType.Unsigned)]
        public short YETSETCFMM707 { get; set; }
        
        /// <summary>
        /// YETSETCFGG707 9(2)
        /// <summary>
        [HisFieldInfoMapping(56, 2, CobolType = CobolType.Unsigned)]
        public short YETSETCFGG707 { get; set; }
        
        /// <summary>
        /// YETSETAGOA707 9(4)     
        /// <summary>
        [HisFieldInfoMapping(57, 4, CobolType = CobolType.Unsigned)]
        public short YETSETAGOA707 { get; set; }
        
        /// <summary>
        /// YETSETAGOB707 9(4)     
        /// <summary>
        [HisFieldInfoMapping(58, 4, CobolType = CobolType.Unsigned)]
        public short YETSETAGOB707 { get; set; }
        
        /// <summary>
        /// YETCALC707 X(01)
        /// <summary>
        [HisFieldInfoMapping(59, 1)]
        public string YETCALC707 { get; set; }

        /// <summary>
        /// YETSETDIR 9(4)     
        /// <summary>
        [HisFieldInfoMapping(60, 4, CobolType = CobolType.Unsigned)]
        public int YETSETDIR { get; set; }
        
        /// <summary>
        /// YETPROGR 99  
        /// </summary>
        [HisFieldInfoMapping(61, 2, CobolType = CobolType.Unsigned)]
        public short YETPROGR { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "ET"; }
        }
        #endregion Properties
    }
}
