using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class CI02Record_RDA
    {
        #region Constructor
        internal CI02Record_RDA()
        {
        }
        #endregion Constructor

        #region tracciato COBOL
        //              * **************************************************************  
        //      * *******   INIZIO TERZO TIPO RECORD      1^ SETTORE   *********  
        //      * **************************************************************  
        //           02  ST03-B                  PIC X(4).                        
        //           02  FLAGAV91                PIC 9.                           
        //           02  IMPADDC                 PIC S9(9) COMP-3.                
        //           02  IMPADD                  PIC S9(9) COMP-3.                
        //           02  KEY1AP                  PIC X(15).                       
        //           02  KEY2AP                  PIC X(15).                       
        //           02  KEY3AP                  PIC X(15).                       
        //           02  KEY4AP                  PIC X(15).                       
        //           02  KEY5AP                  PIC X(15).                       
        //**********                                     ABI + CAB
        //           02  ABI                     PIC 9(5).
        //           02  CAB                     PIC 9(7).
        //           02  FILLER                  PIC X(3).                        
        //           02  DF90 OCCURS 7 TIMES.                                     
        //2000           03  TABSCI PIC S9(9) COMP-3.                             
        //           02  DF95 OCCURS 3.                                           
        //2000           03  DF95ERM             PIC 99.                          
        //2000           03  DF95ERI             PIC S9(7) COMP-3.                
        //           02  DF96                    PIC S9(7) COMP-3.                
        //      * INDIRIZZO PATRONATO                                             
        //           02  DFPIND                  PIC X(33).                       
        //      * CAP       PATRONATO                                             
        //           02  DFPCAP                  PIC X(5).                        
        //      * COMUNE    PATRONATO                                             
        //           02  DFPCOM                  PIC X(22).                       
        //      * PROV      PATRONATO                                             
        //           02  DFPPRO                  PIC X(3).                        
        //      *                                                                 
        //           02  DFNDECOR                PIC 99.                          
        //           02  DFNPROG                 PIC 9(6).                        
        //      * TIPO TE09                                                       
        //           02  TE09                    PIC X.                           
        //           02  DFCPROC                 PIC XX.                          
        //           02  DFCSEL                  PIC XX.                          
        //           02  DFCOPER                 PIC X.                           
        //           02  DFNLAV                  PIC XXX.                         
        //           02  DFCANC                  PIC X.                           
        //           02  DFDUPL                  PIC X.                           
        //      * TIPO TE10                                                       
        //           02  TE10                    PIC 99.                          
        //      * **************************************************************  


        //      * TERZO   TIPO   RECORD  PENSIONI PAGAMENTO CUMULATO              
        //      * **************************************************************  
        //      *   PENSIONI CUMULATE                                             
        //      *    02  RK3CUM   OCCURS 10      PIC 9(15).       ALF 2/00        
        //           02  RK3CUM   OCCURS 10.                                      
        //              03  RK3CUM-CAT        PIC 9(3).                           
        //              03  RK3CUM-SEDE       PIC 9(4).                           
        //              03  RK3CUM-CERT       PIC 9(8).                           
        //           02  RK3IC11                 PIC 9(8).                        
        //           02  RK3IC12                 PIC 99.                          
        //           02  FILLER                  PIC X(80).                       
        //      *                                                                 
        //           02  FILLER                        PIC X(5).                  
        //***********    DECORR  AAAAMM                                           
        //           02  DFK3504.                                                 
        //               03  DFK3504A                  PIC 9999.                  
        //               03  DFK3504M                  PIC 99.                    
        //***********    PERIODICITA PAGAMENTO                                    
        //           02  DF9707                        PIC X.                     
        //           02  DF61                          PIC S9(7)V9999 COMP-3.     
        //***********    IMPORTO RECUPERO CREDITI                                 
        //           02  DFK3507                       PIC S9(7)V9999 COMP-3.     
        //**GEN.*97**    TRATTENUTE DEDUCIBILI ANNI PRECEDENTI                    
        //           02  DF61V                         PIC S9(7)V9999 COMP-3.     
        //      *                                                                 
        //           02  FILLER                        PIC X(10).                 
        //      *                                                                 
        //      *                                                                


        //      * **************************************************************  
        //      * TERZO   TIPO   RECORD          6^ SETTORE                       
        //      * **************************************************************  
        //      *              14/11/96              NUOVO   CALCOLO CONTRIBUTIVO 
        //      *                                                                 
        //**GEN.*97*****                                                          
        //           02  DFA601  OCCURS 8 TIMES.                                  
        //***********    DECORR      GGMMAAAA                                     
        //               03  DFA6011                         PIC 9(8).            
        //***********    P = PENSIONE      S = SUPPLEMENTI                        
        //               03  DFA6012                         PIC X.               
        //***********    COD. GESTIONE                                            
        //               03  DFA6013                         PIC X.               
        //***********    MONTANTE CONTRIBUTIVO                                    
        //               03  DFA6014                    PIC S9(9)V9999 COMP-3.    
        //***********    IMPORTO CONTRIBUTI                                       
        //               03  DFA6015                    PIC S9(7)V9999 COMP-3.    
        //***********    COEFF.                                                   
        //               03  DFA6016.                                             
        //                   04  DFA6016A                    PIC 9.               
        //                   04  DFA6016B                    PIC 9(4).            
        //***********    NUMERO CONTRIBUTI                                        
        //               03  DFA6017                         PIC 9(4).            
        //      *                                                                 
        //               03  FILLER                           PIC XX.             
        //      *                                                                 


        //      * *************************************************************** 
        //      *                                                                 
        //      * **************************************************************  
        //      * TERZO TIPO RECORD              8^ SETTORE                       
        //      * **************************************************************  
        //      *                                                                 
        //           02  R3FELIM                  PIC X.                          
        //           02  R3DTDAL.                                                 
        //               03  R3DALSS              PIC 99.                         
        //               03  R3DALAA              PIC 99.                         
        //               03  R3DALMM              PIC 99.                         
        //           02  R3DATAL.                                                 
        //               03  R3ALSS               PIC 99.                         
        //               03  R3ALAA               PIC 99.                         
        //               03  R3ALMM               PIC 99.                         
        //           02  R3CNGTO                  PIC S9(7)V9999 COMP-3.          
        //           02  R3TRAAC                  PIC S9(7)V9999 COMP-3.          
        //           02  R3CNRAC                  PIC S9(7)V9999 COMP-3.          
        //           02  R3IMPAP                  PIC S9(7)V9999 COMP-3.          
        //           02  R3IMPAC                  PIC S9(7)V9999 COMP-3.          
        //           02  R3CONGP                  PIC S9(7)V9999 COMP-3.          
        //           02  R3CONGC                  PIC S9(7)V9999 COMP-3.          
        //           02  R3CGSIN                  PIC S9(7)V9999 COMP-3.          
        //           02  R3CGONP                  PIC S9(7)V9999 COMP-3.          
        //           02  R3PENTO                  PIC S9(7)V9999 COMP-3.          
        //           02  R3ASSTO                  PIC S9(7)V9999 COMP-3.          
        //           02  R3INDTO                  PIC S9(7)V9999 COMP-3.          
        //           02  R3MAGTO                  PIC S9(7)V9999 COMP-3.          
        //           02  R3EXCTO                  PIC S9(7)V9999 COMP-3.          
        //           02  R3C74TO                  PIC S9(7)V9999 COMP-3.          
        //           02  R3C75TO                  PIC S9(7)V9999 COMP-3.          
        //           02  R3C76TO                  PIC S9(7)V9999 COMP-3.          
        //           02  R3C80TO                  PIC S9(7)V9999 COMP-3.          
        //           02  R3C87TO                  PIC S9(7)V9999 COMP-3.          
        //           02  R3C77TO                  PIC S9(7)V9999 COMP-3.          
        //           02  R3IMCNR                  PIC S9(7)V9999 COMP-3.          
        //           02  R3K5M95                  PIC S9(7)V9999 COMP-3.          
        //           02  R3IK395                  PIC S9(7)V9999 COMP-3.          
        //           02  R3K5M96                  PIC S9(7)V9999 COMP-3.          
        //           02  R3IK396                  PIC S9(7)V9999 COMP-3.          
        //      *                                                                 
        //           02  NUM-EAD75                PIC 9(8).                       
        //           02  DTRICH-EAD               PIC 9(8).                       
        //           02  COD-REC-CRED             PIC X.                          
        //           02  TOT-INDEBITO             PIC S9(7)V9999 COMP-3.          
        //           02  IMP-TASSATO              PIC S9(7)V9999 COMP-3.          
        //           02  LORDO-ACCANTONATO        PIC S9(7)V9999 COMP-3.          
        //           02  RCOD-RECRED              PIC X.                          
        //           02  NUM-RECRED               PIC 9(7).                       
        //           02  DT-RICH                  PIC 9(8).                       
        //           02  TIPO-PROV                PIC 9.                          
        //           02  MOT-1A                   PIC X.                          
        //           02  MOT-2A                   PIC X.                          
        //           02  FILLER                   PIC X(7).                       
        //      *    02  FILLER                   PIC X(34).                      
        //      *      
        #endregion tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// ST03_B X(4)  
        /// </summary>
        [HisFieldInfoMapping(0, 4)]
        public string ST03_B { get; set; }

        /// <summary>
        /// FLAGAV91 9  
        /// </summary>
        [HisFieldInfoMapping(1, 1)]
        public short FLAGAV91 { get; set; }

        /// <summary>
        /// IMPADDC S9(9) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(2, 5, CobolType = CobolType.Comp3)]
        public int IMPADDC { get; set; }

        /// <summary>
        /// IMPADD S9(9) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(3, 5, CobolType = CobolType.Comp3)]
        public int IMPADD { get; set; }

        /// <summary>
        /// KEY1AP X(15)  
        /// </summary>
        [HisFieldInfoMapping(4, 15)]
        public string KEY1AP { get; set; }

        /// <summary>
        /// KEY2AP X(15)  
        /// </summary>
        [HisFieldInfoMapping(5, 15)]
        public string KEY2AP { get; set; }

        /// <summary>
        /// KEY3AP X(15)  
        /// </summary>
        [HisFieldInfoMapping(6, 15)]
        public string KEY3AP { get; set; }

        /// <summary>
        /// KEY4AP X(15)  
        /// </summary>
        [HisFieldInfoMapping(7, 15)]
        public string KEY4AP { get; set; }

        /// <summary>
        /// KEY5AP X(15)  
        /// </summary>
        [HisFieldInfoMapping(8, 15)]
        public string KEY5AP { get; set; }

        //**********                                     ABI + CAB
        /// <summary>
        /// ABI 9(5)  
        /// </summary>
        [HisFieldInfoMapping(9, 5)]
        public int ABI { get; set; }

        /// <summary>
        /// CAB 9(7)  
        /// </summary>
        [HisFieldInfoMapping(10, 7)]
        public int CAB { get; set; }

        /// <summary>
        /// FILLER X(3)  
        /// </summary>
        [HisFieldInfoMapping(11, 3)]
        public string FILLER1 { get; set; }

        [HisComplexAreaInfoMapping(12, ListCount = 7)]
        public List<DF90> LISTADF90 { get; set; }

        [HisComplexAreaInfoMapping(13, ListCount = 3)]
        public List<DF95> LISTADF95 { get; set; }

        /// <summary>
        /// DF96 S9(7) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(14, 4, CobolType = CobolType.Comp3)]
        public int DF96 { get; set; }

        // * INDIRIZZO PATRONATO
        /// <summary>
        /// DFPIND X(33)  
        /// </summary>
        [HisFieldInfoMapping(15, 33)]
        public string DFPIND { get; set; }

        // * CAP       PATRONATO
        /// <summary>
        /// DFPCAP X(5)  
        /// </summary>
        [HisFieldInfoMapping(16, 5)]
        public string DFPCAP { get; set; }

        // * COMUNE    PATRONATO
        /// <summary>
        /// DFPCOM X(22)  
        /// </summary>
        [HisFieldInfoMapping(17, 22)]
        public string DFPCOM { get; set; }

        // * PROV      PATRONATO
        /// <summary>
        /// DFPPRO X(3)  
        /// </summary>
        [HisFieldInfoMapping(18, 3)]
        public string DFPPRO { get; set; }

        /// <summary>
        /// DFNDECOR 99  
        /// </summary>
        [HisFieldInfoMapping(19, 2)]
        public short DFNDECOR { get; set; }

        /// <summary>
        /// DFNPROG 9(6)  
        /// </summary>
        [HisFieldInfoMapping(20, 6)]
        public int DFNPROG { get; set; }

        // * TIPO TE09
        /// <summary>
        /// TE09 X  
        /// </summary>
        [HisFieldInfoMapping(21, 1)]
        public string TE09 { get; set; }

        /// <summary>
        /// DFCPROC XX  
        /// </summary>
        [HisFieldInfoMapping(22, 2)]
        public string DFCPROC { get; set; }

        /// <summary>
        /// DFCSEL XX  
        /// </summary>
        [HisFieldInfoMapping(23, 2)]
        public string DFCSEL { get; set; }

        /// <summary>
        /// DFCOPER X  
        /// </summary>
        [HisFieldInfoMapping(24, 1)]
        public string DFCOPER { get; set; }

        /// <summary>
        /// DFNLAV XXX  
        /// </summary>
        [HisFieldInfoMapping(25, 3)]
        public string DFNLAV { get; set; }

        /// <summary>
        /// DFCANC X  
        /// </summary>
        [HisFieldInfoMapping(26, 1)]
        public string DFCANC { get; set; }

        /// <summary>
        /// DFDUPL X  
        /// </summary>
        [HisFieldInfoMapping(27, 1)]
        public string DFDUPL { get; set; }

        // * TIPO TE10
        /// <summary>
        /// TE10 99  
        /// </summary>
        [HisFieldInfoMapping(28, 2)]
        public short TE10 { get; set; }

        //* **************************************************************

        [HisComplexAreaInfoMapping(29, ListCount = 10)]
        public List<RK3CUM> LISTARK3CUM { get; set; }

        /// <summary>
        /// RK3IC11 9(8)  
        /// </summary>
        [HisFieldInfoMapping(30, 8)]
        public int RK3IC11 { get; set; }

        /// <summary>
        /// RK3IC12 99  
        /// </summary>
        [HisFieldInfoMapping(31, 2)]
        public short RK3IC12 { get; set; }

        /// <summary>
        /// FILLER X(80)  
        /// </summary>
        [HisFieldInfoMapping(32, 80)]
        public string FILLER2 { get; set; }

        /// <summary>
        /// FILLER X(5)  
        /// </summary>
        [HisFieldInfoMapping(33, 5)]
        public string FILLER3 { get; set; }

        //***********    DECORR  AAAAMM
        // 02  DFK3504.
        /// <summary>
        /// DFK3504A 9(4)  
        /// </summary>
        [HisFieldInfoMapping(34, 4)]
        public short DFK3504A { get; set; }

        /// <summary>
        /// DFK3504M 99  
        /// </summary>
        [HisFieldInfoMapping(35, 2)]
        public short DFK3504M { get; set; }

        //***********    PERIODICITA PAGAMENTO
        /// <summary>
        /// DF9707 X  
        /// </summary>
        [HisFieldInfoMapping(36, 1)]
        public string DF9707 { get; set; }

        /// <summary>
        /// DF61 S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(37, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF61 { get; set; }

        //***********    IMPORTO RECUPERO CREDITI
        /// <summary>
        /// DFK3507 S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(38, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DFK3507 { get; set; }

        // **GEN.*97**    TRATTENUTE DEDUCIBILI ANNI PRECEDENTI
        /// <summary>
        /// DF61V S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(39, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF61V { get; set; }

        /// <summary>
        /// FILLER X(10)  
        /// </summary>
        [HisFieldInfoMapping(40, 10)]
        public string FILLER4 { get; set; }

        [HisComplexAreaInfoMapping(41, ListCount = 8)]
        public List<CalcoloContributivo> CALCOLOCONTRIBUTIVO { get; set; }

        //* **************************************************************
        // * TERZO TIPO RECORD              8^ SETTORE
        //* **************************************************************
        /// <summary>
        /// R3FELIM X  
        /// </summary>
        [HisFieldInfoMapping(42, 1)]
        public string R3FELIM { get; set; }

        // 02  R3DTDAL.
        /// <summary>
        /// R3DALSS 99  
        /// </summary>
        [HisFieldInfoMapping(43, 2)]
        public short R3DALSS { get; set; }

        /// <summary>
        /// R3DALAA 99  
        /// </summary>
        [HisFieldInfoMapping(44, 2)]
        public short R3DALAA { get; set; }

        /// <summary>
        /// R3DALMM 99  
        /// </summary>
        [HisFieldInfoMapping(45, 2)]
        public short R3DALMM { get; set; }

        // 02  R3DATAL.
        /// <summary>
        /// R3ALSS 99  
        /// </summary>
        [HisFieldInfoMapping(46, 2)]
        public short R3ALSS { get; set; }

        /// <summary>
        /// R3ALAA 99  
        /// </summary>
        [HisFieldInfoMapping(47, 2)]
        public short R3ALAA { get; set; }

        /// <summary>
        /// R3ALMM 99  
        /// </summary>
        [HisFieldInfoMapping(48, 2)]
        public short R3ALMM { get; set; }

        /// <summary>
        /// R3CNGTO S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(49, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3CNGTO { get; set; }

        /// <summary>
        /// R3TRAAC S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(50, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3TRAAC { get; set; }

        /// <summary>
        /// R3CNRAC S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(51, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3CNRAC { get; set; }

        /// <summary>
        /// R3IMPAP S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(52, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3IMPAP { get; set; }

        /// <summary>
        /// R3IMPAC S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(53, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3IMPAC { get; set; }

        /// <summary>
        /// R3CONGP S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(54, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3CONGP { get; set; }

        /// <summary>
        /// R3CONGC S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(55, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3CONGC { get; set; }

        /// <summary>
        /// R3CGSIN S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(56, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3CGSIN { get; set; }

        /// <summary>
        /// R3CGONP S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(57, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3CGONP { get; set; }

        /// <summary>
        /// R3PENTO S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(58, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3PENTO { get; set; }

        /// <summary>
        /// R3ASSTO S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(59, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3ASSTO { get; set; }

        /// <summary>
        /// R3INDTO S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(60, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3INDTO { get; set; }

        /// <summary>
        /// R3MAGTO S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(61, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3MAGTO { get; set; }

        /// <summary>
        /// R3EXCTO S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(62, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3EXCTO { get; set; }

        /// <summary>
        /// R3C74TO S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(63, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3C74TO { get; set; }

        /// <summary>
        /// R3C75TO S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(64, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3C75TO { get; set; }

        /// <summary>
        /// R3C76TO S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(65, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3C76TO { get; set; }

        /// <summary>
        /// R3C80TO S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(66, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3C80TO { get; set; }

        /// <summary>
        /// R3C87TO S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(67, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3C87TO { get; set; }

        /// <summary>
        /// R3C77TO S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(68, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3C77TO { get; set; }

        /// <summary>
        /// R3IMCNR S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(69, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3IMCNR { get; set; }

        /// <summary>
        /// R3K5M95 S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(70, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3K5M95 { get; set; }

        /// <summary>
        /// R3IK395 S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(71, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3IK395 { get; set; }

        /// <summary>
        /// R3K5M96 S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(72, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3K5M96 { get; set; }

        /// <summary>
        /// R3IK396 S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(73, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal R3IK396 { get; set; }

        /// <summary>
        /// NUM_EAD75 9(8)  
        /// </summary>
        [HisFieldInfoMapping(74, 8)]
        public int NUM_EAD75 { get; set; }

        /// <summary>
        /// DTRICH_EAD 9(8)  
        /// </summary>
        [HisFieldInfoMapping(75, 8)]
        public int DTRICH_EAD { get; set; }

        /// <summary>
        /// COD_REC_CRED X  
        /// </summary>
        [HisFieldInfoMapping(76, 1)]
        public string COD_REC_CRED { get; set; }

        /// <summary>
        /// TOT_INDEBITO S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(77, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal TOT_INDEBITO { get; set; }

        /// <summary>
        /// IMP_TASSATO S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(78, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IMP_TASSATO { get; set; }

        /// <summary>
        /// LORDO_ACCANTONATO S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(79, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal LORDO_ACCANTONATO { get; set; }

        /// <summary>
        /// RCOD_RECRED X  
        /// </summary>
        [HisFieldInfoMapping(80, 1)]
        public string RCOD_RECRED { get; set; }

        /// <summary>
        /// NUM_RECRED 9(7)  
        /// </summary>
        [HisFieldInfoMapping(81, 7)]
        public int NUM_RECRED { get; set; }

        /// <summary>
        /// DT_RICH 9(8)  
        /// </summary>
        [HisFieldInfoMapping(82, 8)]
        public int DT_RICH { get; set; }

        /// <summary>
        /// TIPO_PROV 9  
        /// </summary>
        [HisFieldInfoMapping(83, 1)]
        public short TIPO_PROV { get; set; }

        /// <summary>
        /// MOT_1A X  
        /// </summary>
        [HisFieldInfoMapping(84, 1)]
        public string MOT_1A { get; set; }

        /// <summary>
        /// MOT_2A X  
        /// </summary>
        [HisFieldInfoMapping(85, 1)]
        public string MOT_2A { get; set; }

        /// <summary>
        /// FILLER X(7)  
        /// </summary>
        [HisFieldInfoMapping(86, 7)]
        public string FILLER5 { get; set; }

        #endregion Tracciato Host

        #region nested class
        public class DF90
        {
            #region Constructor
            internal DF90()
            { }
            #endregion Constructor

            #region tracciato COBOL
            //     02  DF90 OCCURS 7 TIMES.                                     
            //2000           03  TABSCI PIC S9(9) COMP-3.  
            #endregion tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// TABSCI S9(9) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(0, 5, CobolType = CobolType.Comp3)]
            public int TABSCI { get; set; }
            #endregion Tracciato Host
        }

        public class DF95
        {
            #region Constructor
            internal DF95()
            { }
            #endregion Constructor

            #region tracciato COBOL 
            //        02  DF95 OCCURS 3.                                           
            //2000           03  DF95ERM             PIC 99.                          
            //2000           03  DF95ERI             PIC S9(7) COMP-3. 
            #endregion tracciato COBOL

            #region Tracciato Host
            // 02  DF95 OCCURS 3.
            /// <summary>
            /// DF95ERM 99  
            /// </summary>
            [HisFieldInfoMapping(0, 2)]
            public short DF95ERM { get; set; }

            /// <summary>
            /// DF95ERI S9(7) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(1, 4, CobolType = CobolType.Comp3)]
            public int DF95ERI { get; set; }
            #endregion Tracciato Host
        }

        public class RK3CUM
        {
            #region Constructor
            internal RK3CUM()
            { }
            #endregion Constructor

            #region tracciato COBOL
            //       *   PENSIONI CUMULATE                                   
            //02  RK3CUM   OCCURS 10.                                      
            //   03  RK3CUM-CAT        PIC 9(3).                           
            //   03  RK3CUM-SEDE       PIC 9(4).                           
            //   03  RK3CUM-CERT       PIC 9(8). 
            #endregion tracciato COBOL

            #region Tracciato Host
            // *   PENSIONI CUMULATE
            // 02  RK3CUM   OCCURS 10.
            /// <summary>
            /// RK3CUM_CAT 9(3)  
            /// </summary>
            [HisFieldInfoMapping(0, 3)]
            public short RK3CUM_CAT { get; set; }

            /// <summary>
            /// RK3CUM_SEDE 9(4)  
            /// </summary>
            [HisFieldInfoMapping(1, 4)]
            public short RK3CUM_SEDE { get; set; }

            /// <summary>
            /// RK3CUM_CERT 9(8)  
            /// </summary>
            [HisFieldInfoMapping(2, 8)]
            public int RK3CUM_CERT { get; set; }
            #endregion Tracciato Host
        }


        public class CalcoloContributivo
        {
            #region Constructor
            internal CalcoloContributivo()
            { }
            #endregion Constructor

            #region tracciato COBOL

            //      * **************************************************************  
            //      * TERZO   TIPO   RECORD          6^ SETTORE                       
            //      * **************************************************************  
            //      *              14/11/96              NUOVO   CALCOLO CONTRIBUTIVO 
            //      *                                                                 
            //**GEN.*97*****                                                          
            //           02  DFA601  OCCURS 8 TIMES.                                  
            //***********    DECORR      GGMMAAAA                                     
            //               03  DFA6011                         PIC 9(8).            
            //***********    P = PENSIONE      S = SUPPLEMENTI                        
            //               03  DFA6012                         PIC X.               
            //***********    COD. GESTIONE                                            
            //               03  DFA6013                         PIC X.               
            //***********    MONTANTE CONTRIBUTIVO                                    
            //               03  DFA6014                    PIC S9(9)V9(4) COMP-3.    
            //***********    IMPORTO CONTRIBUTI                                       
            //               03  DFA6015                    PIC S9(7)V9(4) COMP-3.    
            //***********    COEFF.                                                   
            //               03  DFA6016.                                             
            //                   04  DFA6016A                    PIC 9.               
            //                   04  DFA6016B                    PIC 9(4).            
            //***********    NUMERO CONTRIBUTI                                        
            //               03  DFA6017                         PIC 9(4).            
            //      *                                                                 
            //               03  FILLER                           PIC XX.       
            #endregion tracciato COBOL

            #region Tracciato Host
            //* **************************************************************
            // * TERZO   TIPO   RECORD          6^ SETTORE
            //* **************************************************************
            // *              14/11/96              NUOVO   CALCOLO CONTRIBUTIVO
            // **GEN.*97*****
            // 02  DFA601  OCCURS 8 TIMES.
            //***********    DECORR      GGMMAAAA
            /// <summary>
            /// DFA6011 9(8)  
            /// </summary>
            [HisFieldInfoMapping(0, 8)]
            public int DFA6011 { get; set; }

            //***********    P = PENSIONE      S = SUPPLEMENTI
            /// <summary>
            /// DFA6012 X  
            /// </summary>
            [HisFieldInfoMapping(1, 1)]
            public string DFA6012 { get; set; }

            //***********    COD. GESTIONE
            /// <summary>
            /// DFA6013 X  
            /// </summary>
            [HisFieldInfoMapping(2, 1)]
            public string DFA6013 { get; set; }

            //***********    MONTANTE CONTRIBUTIVO
            /// <summary>
            /// DFA6014 S9(9)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(3, 7, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal DFA6014 { get; set; }

            //***********    IMPORTO CONTRIBUTI
            /// <summary>
            /// DFA6015 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(4, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal DFA6015 { get; set; }

            //***********    COEFF.
            // 03  DFA6016.
            /// <summary>
            /// DFA6016A 9  
            /// </summary>
            [HisFieldInfoMapping(5, 1)]
            public short DFA6016A { get; set; }

            /// <summary>
            /// DFA6016B 9(4)  
            /// </summary>
            [HisFieldInfoMapping(6, 4)]
            public short DFA6016B { get; set; }

            //***********    NUMERO CONTRIBUTI
            /// <summary>
            /// DFA6017 9(4)  
            /// </summary>
            [HisFieldInfoMapping(7, 4)]
            public short DFA6017 { get; set; }

            /// <summary>
            /// FILLER XX  
            /// </summary>
            [HisFieldInfoMapping(8, 2)]
            public string FILLER7 { get; set; }
            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
