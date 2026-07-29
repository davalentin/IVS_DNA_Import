using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class CI02Record_RE
    {
        #region Constructor
        internal CI02Record_RE()
        {
        }
        #endregion Constructor

        #region tracciato COBOL
        //        ************************************************************************
        //***********    QUARTO TIPO RECORD   SETTORE 1^                **********
        //************************************************************************
        //           02  ST04                    PIC X(4).                        
        //           02  ST04ER                  PIC X(24).                       
        //           02  DFRNUM                  PIC XX.       


        //           02  DF1TAB   OCCURS 55.                                      
        //               03   DFANNO1            PIC 9999.                        
        //               03   DFCPEN1            PIC S9(7)V9999 COMP-3.           
        //               03   DFCFAM1            PIC S9(7)V9999 COMP-3.           
        //               03   DFCMSOC            PIC S9(7)V9999 COMP-3.           
        //               03   DFCCOMB            PIC S9(7)V9999 COMP-3.           
        //               03   DFCAACC            PIC S9(7)V9999 COMP-3.           
        //               03   FILLER             PIC X.                           
        //      ***************************************************************** 
        //      ***************************************************************** 
        //      **** RECORD "E" CORRISPONDENTE AL 5 RECORD DPCX  (1500) ********* 
        //      **** LUNGHEZZA 1920 BYTES      *    PER  C.I.            ******** 
        //      ***************************************************************** 
        //           02  REC-RE.                                                  
        //               03  REC-SET1-E.                                          
        //      * I DATI SONO ORGANIZZATI COME SE FOSSE UN UNICO SETTORE DA 1680  
        //      * BYTES: (240 X 8)                                                
        //**********                                            CHIAVE            
        //                       05  D15101-E      PIC X(4).                      
        //**********      SUPPLEMENTI  DAL 2001                   


        //   *****               TABELLA DATI CALCOLO SUPPLEMETI                  
        //                       05  TAB-SUPPLEMENTI.                             
        //                           06  DF9703-C     OCCURS 10.                  
        //     **********                           CODICE GESTIONE               
        //                               07  DF97031-C         PIC X.             
        //     **********                           DECORRENZA                    
        //                               07  DF97032-CA        PIC 9999.          
        //                               07  DF97032-CM        PIC 99.            
        //   **********                           N.  SETTIMANE                   
        //                               07  DF97033-C         PIC 9(4).          
        //   **********                           RETRIBUZ. MEDIA SETTIM.         
        //                               07  DF97034-C     PIC S9(7)V9999 COMP-3. 
        //   **********                           IMPORTO IVS                     
        //                               07  DF97035-C     PIC S9(7)V9999 COMP-3. 
        //   **********                  CONTRIBUTO DI SOLIDARIETA    


        //                 05  TAB-SOLIDARIETA.                                   
        //                     06  EL-SOLIDARIETA   OCCURS 6.                     
        //                        07  MESE-SOLIDA   PIC 99.                       
        //                        07  IMP-SOLIDA    PIC S9(7)V9999 COMP-3.        
        //**********************  DATI PATRONATO:  24 BYTE



        //      *GP1RICPTUFF TIPO UFFICIO
        //                 05 PATUFF          PIC 9(3).
        //      *GP1RICPCOD CODICE ENTE DI PATRONATO
        //                 05 PATCOD          PIC 9(3).
        //      *GP1RICPZON CODICE UFF ZONALE ENTE DI PATRONATO
        //                 05 PATZON          PIC X(10).
        //      *GP1RICPZON CODICE UFF ZONALE ENTE DI PATRONATO
        //                 05 PATNUM          PIC 9(8).
        //   ****     A  DISPOSIZIONE                                             
        //                 05  FILLER               PIC X(39).                    
        //   ****                 A DISPOSIZIONE                                  
        //                    05  FILLER            PIC X(197).                


        //      * DATI PER STAMPA PLCI :   37 BYTES RIPETUTI 5 VOLTE = 185        
        //                       05  D15105-E   OCCURS 5.                         
        //                         06  D15105-E-ITA.                              
        //**********                                           DECORRENZA              
        //                         10  D15105-E-1.                                
        //                             15 D15105-E-1A   PIC X.
        //                             15 D15105-E-1B   PIC X.
        //**********                                           CTR ITALIANI       
        //                         10  D15105-E-2    PIC S9999.                   
        //**********                                           VIRTUALE           
        //                         10  D15105-E-3    PIC S9(7)V9999 COMP-3.       
        //**********                                           COEFF.RIDUZ.       
        //                         10  D15105-E-4    PIC S9(5).                   
        //**********                                           PENS.DIRETTA       
        //                         10  D15105-E-5    PIC S9(7)V9999 COMP-3.       
        //**********                                           PERC.SUPERSTITI    
        //                         10  D15105-E-6    PIC 9V9999 COMP-3.                     
        //**********                                           PENS.SUPERSTITI    
        //                         10  D15105-E-7    PIC S9(7)V9999 COMP-3.       
        //**********                                           PENS.ESTERA        
        //                         10  D15105-E-8    PIC S9(7)V9999 COMP-3.       
        //**********                                           STATO 1    


        //                       06  D15105-E-ST   OCCURS 4.                      
        //                         10  D15105-E-ST1  PIC 99.                      
        //**********                                           CTR STATO 1        
        //                         10  D15105-E-CTR1 PIC 9999.                    
        //      * DATI PER STAMPA PLCI :  253 BYTES RIPETUTI 4 VOLTE = 1012       
        //      * DATI STATI ESTERI                                               


        //***********                                             (253 X 4 = 1012)
        //                       05  D15106-E   OCCURS 4.                         
        //***********                                             CODICE STATO 1  
        //                         10  D15107-E      PIC 99.                      
        //***********                                             CODICE ISTITUZ  
        //                         10  D15108-E      PIC 999.                     
        //***********                                             (250 BYTES)     
        //                         10  D15109-E    OCCURS 25.                     
        //***********                                             DECORRENZA              
        //                           15  D15109-E-DEC.                            
        //                               20  D15109-E-DEC1        PIC 9999.
        //                               20  D15109-E-DEC2        PIC 99.
        //***********                                             CESSAZIONE         
        //                           15  D15109-E-CES.                            
        //                               20  D15109-E-CES1        PIC 9999.
        //                               20  D15109-E-CES2        PIC 99.
        //***********                                             IMP.ESTERO      
        //                           15  D15109-E-IMP  PIC S9(7)V9(8).   


        //***********           CODICI PARAGRAFI PER IL CI28                      
        //                       05  PARAGRAFI-CI28.                              
        //                           06  PAR-CI28   OCCURS  5     PIC XX.  


        //***********    = 1 VEC = 2 ANZ = 3 SUP = 4 INAB = 5 INVAL               
        //                       05  TIPO-PENSIONE                PIC X.          
        //***********       DATA DELLA DOMANDA GGMMAA                             
        //                       05  DATA-DOMANDA                 PIC 9(6).       
        //***********    = 7 VO DA AOI                                            
        //                       05  REQUISITO-PARTICOLARE        PIC X.          
        //***********    =   FLAG 495  1 = SI    0 = NO                           
        //                       05  FLAG495                      PIC 9.          
        //***********    =   FLAG MINIMALE 1 = SI    0 = NO                       
        //                       05  FLAGMINIMALE                 PIC 9.          
        //***********    =   FLAG CRIST 335                                       
        //                       05  FLAGCRI335                   PIC X.          
        //***********    =   ARRETRATI BLOCCATI ESTERO   PD                       
        //                       05  BLOCCO-ARRETRATI             PIC X.          
        //***********    = 9 CODICE PER TRATTENUTA GIORNALIERA                    
        //                       05  CODICE-IMPORTO-MENSILE       PIC X.          
        //***********    = ANNO DI LIVELLO 335                                    
        //                       05  ANNO-LIV335                  PIC 9(4).       
        //***********    = NUMERO RICONOSCIMENTI ASSEGNO                          
        //                       05  N-RICON-ASSEGNO              PIC X.          
        //***********    =  NUMERO CONTRIBUTI EFFETTIVI                           
        //                       05  N-CONTR-EFF                  PIC 9999.       
        //***********    =  SIGLA STATO ESTERO                                    
        //                       05  SIGLA-STATO-ESTERO           PIC XXX.        
        //***********    =  PENSIONE CONTRB/RETRIB                                
        //                       05  TIPO-CONTR-RETR              PIC X.    


        //***********    =  TABELLA TASSE                                         
        //                       05  TAB-TASSE.                                   
        //                           06  EL-TASSE   OCCURS 6.                     
        //                               07  MESE-TASSE         PIC 99.           
        //                               07  IMPO-TASSE COMP-3  PIC S9(5)V9999.   


        //***********    =  FLAG PER STAMPE LETTERE JUGO PER OPZIONE              
        //                       05  FLAG-YUGOSLAVIA            PIC X.            
        //***********    =  FLAG385 = 1 = APPLICATA SENTENZA 385                  
        //                       05  FLAG385                    PIC X.            
        //***********    =  FLAGMILIO = 1 = DATA IL MILIONE                       
        //                       05  FLAGMILIO                  PIC X.            
        //***********    =  FLAG PER APPLICAZIONE SENT 16
        //                       05  FLAGS16                    PIC X.
        //***********    =  FLAG PER APPLICAZIONE ART38 SUPER PREMIO
        //                       05  FLAGART38                  PIC X.
        //***********    =  ESISTE MATERNITA
        //                       05  FLAGMATER                  PIC X.
        //***********    =  GP1AXE3                                               
        //                       05  AXE3                       PIC X.            
        //**********    =  MONTANTE FITTIZIE                                      
        //                       05  MONTANTE-FITT         PIC S9(7)V9999 COMP-3. 
        //**********    =  DEC DANTE CAUSA                                        
        //                       05  DEC-DANTE-CAUSA            PIC 9(4).    


        //***********    =  DATI PER 503                                          
        //                       05  ELEM-503   OCCURS 6.                         
        //                           06  DF9325-C-503      PIC S9(7)V9999 COMP-3. 
        //                           06  DF9326-C-503      PIC S9999.       


        //***********    =  SETTIMANE FITTIZIE                                    
        //                       05  SETT-FITT                  PIC 9999.         
        //***********    =  TIPO PROCEDURA                                        
        //                       05  RIC-AF        PIC X.                         
        //                       05  FILLER        PIC X(47).                     
        //                       05  NUME-DOMUS    PIC X(13).                     
        //                       05  FLAG-ACNA     PIC X.
        //                       05  ARRI-DOMANDA  PIC 9(8).                      
        //      * NON A DISPOSIZIONE                                              
        //************* DATI DANTE CAUSA (PER STAMPA FORMULARI)--- 91 BYTES       
        //                       05 COD-CONVEN             PIC X(2).              
        //                       05 COGN-D-C               PIC X(32).             
        //                       05 NOME-D-C               PIC X(32).             
        //                       05 SESS-D-C               PIC X(1).              
        //                       05 NASC-D-C               PIC 9(8).              
        //                       05 MATRICOLA              PIC X(16).             
        //      *    CERTIFICATO PER RIPARTENZA                                   
        //                       05  CERTIFICATO-PER-RIPAR   PIC 9(8).      
        #endregion tracciato COBOL

        #region Tracciato Host

        /// <summary>
        /// ST04 X(4)  
        /// </summary>
        [HisFieldInfoMapping(0, 4)]
        public string ST04 { get; set; }

        /// <summary>
        /// ST04ER X(24)  
        /// </summary>
        [HisFieldInfoMapping(1, 24)]
        public string ST04ER { get; set; }

        /// <summary>
        /// DFRNUM XX  
        /// </summary>
        [HisFieldInfoMapping(2, 2)]
        public string DFRNUM { get; set; }

        [HisComplexAreaInfoMapping(3, ListCount = 55)]
        public List<DF1TAB> LISTADF1TAB { get; set; }

        //**********                                            CHIAVE
        /// <summary>
        /// D15101_E X(4)  
        /// </summary>
        [HisFieldInfoMapping(4, 4)]
        public string D15101_E { get; set; }

        [HisComplexAreaInfoMapping(5, ListCount = 10)]
        public List<Supplemento> SUPPLEMENTI { get; set; }

        [HisComplexAreaInfoMapping(6, ListCount = 6)]
        public List<Solidarieta> LISTASOLIDARIETA { get; set; }

        //**********************  DATI PATRONATO:  24 BYTE
        // *GP1RICPTUFF TIPO UFFICIO
        /// <summary>
        /// PATUFF 9(3)  
        /// </summary>
        [HisFieldInfoMapping(7, 3)]
        public short PATUFF { get; set; }

        // *GP1RICPCOD CODICE ENTE DI PATRONATO
        /// <summary>
        /// PATCOD 9(3)  
        /// </summary>
        [HisFieldInfoMapping(8, 3)]
        public short PATCOD { get; set; }

        // *GP1RICPZON CODICE UFF ZONALE ENTE DI PATRONATO
        /// <summary>
        /// PATZON X(10)  
        /// </summary>
        [HisFieldInfoMapping(9, 10)]
        public string PATZON { get; set; }

        // *GP1RICPZON CODICE UFF ZONALE ENTE DI PATRONATO
        /// <summary>
        /// PATNUM 9(8)  
        /// </summary>
        [HisFieldInfoMapping(10, 8)]
        public int PATNUM { get; set; }

        // ****     A  DISPOSIZIONE
        /// <summary>
        /// FILLER X(39)  
        /// </summary>
        [HisFieldInfoMapping(11, 39)]
        public string FILLER1 { get; set; }

        // ****                 A DISPOSIZIONE
        /// <summary>
        /// FILLER X(197)  
        /// </summary>
        [HisFieldInfoMapping(12, 197)]
        public string FILLER2 { get; set; }

        [HisComplexAreaInfoMapping(13, ListCount = 5)]
        public List<StampaPLCI> LISTASTAMPAPLCI { get; set; }

        [HisComplexAreaInfoMapping(14, ListCount = 4)]
        public List<D15106> LISTAD15106 { get; set; }

        [HisComplexAreaInfoMapping(15, ListCount = 5)]
        public List<Paragrafi_CI28> LISTAPARAMETRICI28 { get; set; }

        //***********    = 1 VEC = 2 ANZ = 3 SUP = 4 INAB = 5 INVAL
        /// <summary>
        /// TIPO_PENSIONE X  
        /// </summary>
        [HisFieldInfoMapping(16, 1)]
        public string TIPO_PENSIONE { get; set; }

        //***********       DATA DELLA DOMANDA GGMMAA
        /// <summary>
        /// DATA_DOMANDA 9(6)  
        /// </summary>
        [HisFieldInfoMapping(17, 6)]
        public int DATA_DOMANDA { get; set; }

        //***********    = 7 VO DA AOI
        /// <summary>
        /// REQUISITO_PARTICOLARE X  
        /// </summary>
        [HisFieldInfoMapping(18, 1)]
        public string REQUISITO_PARTICOLARE { get; set; }

        //***********    =   FLAG 495  1 = SI    0 = NO
        /// <summary>
        /// FLAG495 9  
        /// </summary>
        [HisFieldInfoMapping(19, 1)]
        public short FLAG495 { get; set; }

        //***********    =   FLAG MINIMALE 1 = SI    0 = NO
        /// <summary>
        /// FLAGMINIMALE 9  
        /// </summary>
        [HisFieldInfoMapping(20, 1)]
        public short FLAGMINIMALE { get; set; }

        //***********    =   FLAG CRIST 335
        /// <summary>
        /// FLAGCRI335 X  
        /// </summary>
        [HisFieldInfoMapping(21, 1)]
        public string FLAGCRI335 { get; set; }

        //***********    =   ARRETRATI BLOCCATI ESTERO   PD
        /// <summary>
        /// BLOCCO_ARRETRATI X  
        /// </summary>
        [HisFieldInfoMapping(22, 1)]
        public string BLOCCO_ARRETRATI { get; set; }

        //***********    = 9 CODICE PER TRATTENUTA GIORNALIERA
        /// <summary>
        /// CODICE_IMPORTO_MENSILE X  
        /// </summary>
        [HisFieldInfoMapping(23, 1)]
        public string CODICE_IMPORTO_MENSILE { get; set; }

        //***********    = ANNO DI LIVELLO 335
        /// <summary>
        /// ANNO_LIV335 9(4)  
        /// </summary>
        [HisFieldInfoMapping(24, 4)]
        public short ANNO_LIV335 { get; set; }

        //***********    = NUMERO RICONOSCIMENTI ASSEGNO
        /// <summary>
        /// N_RICON_ASSEGNO X  
        /// </summary>
        [HisFieldInfoMapping(25, 1)]
        public string N_RICON_ASSEGNO { get; set; }

        //***********    =  NUMERO CONTRIBUTI EFFETTIVI
        /// <summary>
        /// N_CONTR_EFF 9999  
        /// </summary>
        [HisFieldInfoMapping(26, 4)]
        public short N_CONTR_EFF { get; set; }

        //***********    =  SIGLA STATO ESTERO
        /// <summary>
        /// SIGLA_STATO_ESTERO XXX  
        /// </summary>
        [HisFieldInfoMapping(27, 3)]
        public string SIGLA_STATO_ESTERO { get; set; }

        //***********    =  PENSIONE CONTRB/RETRIB
        /// <summary>
        /// TIPO_CONTR_RETR X  
        /// </summary>
        [HisFieldInfoMapping(28, 1)]
        public string TIPO_CONTR_RETR { get; set; }

        [HisComplexAreaInfoMapping(29, ListCount = 6)]
        public List<Tassa> TASSE { get; set; }

        //***********    =  FLAG PER STAMPE LETTERE JUGO PER OPZIONE
        /// <summary>
        /// FLAG_YUGOSLAVIA X  
        /// </summary>
        [HisFieldInfoMapping(30, 1)]
        public string FLAG_YUGOSLAVIA { get; set; }

        //***********    =  FLAG385 = 1 = APPLICATA SENTENZA 385
        /// <summary>
        /// FLAG385 X  
        /// </summary>
        [HisFieldInfoMapping(31, 1)]
        public string FLAG385 { get; set; }

        //***********    =  FLAGMILIO = 1 = DATA IL MILIONE
        /// <summary>
        /// FLAGMILIO X  
        /// </summary>
        [HisFieldInfoMapping(32, 1)]
        public string FLAGMILIO { get; set; }

        //***********    =  FLAG PER APPLICAZIONE SENT 16
        /// <summary>
        /// FLAGS16 X  
        /// </summary>
        [HisFieldInfoMapping(33, 1)]
        public string FLAGS16 { get; set; }

        //***********    =  FLAG PER APPLICAZIONE ART38 SUPER PREMIO
        /// <summary>
        /// FLAGART38 X  
        /// </summary>
        [HisFieldInfoMapping(34, 1)]
        public string FLAGART38 { get; set; }

        //***********    =  ESISTE MATERNITA
        /// <summary>
        /// FLAGMATER X  
        /// </summary>
        [HisFieldInfoMapping(35, 1)]
        public string FLAGMATER { get; set; }

        //***********    =  GP1AXE3
        /// <summary>
        /// AXE3 X  
        /// </summary>
        [HisFieldInfoMapping(36, 1)]
        public string AXE3 { get; set; }

        //**********    =  MONTANTE FITTIZIE
        /// <summary>
        /// MONTANTE_FITT S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(37, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal MONTANTE_FITT { get; set; }

        //**********    =  DEC DANTE CAUSA
        /// <summary>
        /// DEC_DANTE_CAUSA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(38, 4)]
        public short DEC_DANTE_CAUSA { get; set; }

        [HisComplexAreaInfoMapping(39, ListCount = 6)]
        public List<Dati503> LISTADATI503 { get; set; }

        //***********    =  SETTIMANE FITTIZIE
        /// <summary>
        /// SETT_FITT 9999  
        /// </summary>
        [HisFieldInfoMapping(40, 4)]
        public short SETT_FITT { get; set; }

        //***********    =  TIPO PROCEDURA
        /// <summary>
        /// RIC_AF X  
        /// </summary>
        [HisFieldInfoMapping(41, 1)]
        public string RIC_AF { get; set; }

        /// <summary>
        /// FILLER X(47)  
        /// </summary>
        [HisFieldInfoMapping(42, 47)]
        public string FILLER3 { get; set; }

        /// <summary>
        /// NUME_DOMUS X(13)  
        /// </summary>
        [HisFieldInfoMapping(43, 13)]
        public string NUME_DOMUS { get; set; }

        /// <summary>
        /// FLAG_ACNA X  
        /// </summary>
        [HisFieldInfoMapping(44, 1)]
        public string FLAG_ACNA { get; set; }

        /// <summary>
        /// ARRI_DOMANDA 9(8)  
        /// </summary>
        [HisFieldInfoMapping(45, 8)]
        public int ARRI_DOMANDA { get; set; }

        // * NON A DISPOSIZIONE
        //************* DATI DANTE CAUSA (PER STAMPA FORMULARI)--- 91 BYTES
        /// <summary>
        /// COD_CONVEN X(2)  
        /// </summary>
        [HisFieldInfoMapping(46, 2)]
        public string COD_CONVEN { get; set; }

        /// <summary>
        /// COGN_D_C X(32)  
        /// </summary>
        [HisFieldInfoMapping(47, 32)]
        public string COGN_D_C { get; set; }

        /// <summary>
        /// NOME_D_C X(32)  
        /// </summary>
        [HisFieldInfoMapping(48, 32)]
        public string NOME_D_C { get; set; }

        /// <summary>
        /// SESS_D_C X(1)  
        /// </summary>
        [HisFieldInfoMapping(49, 1)]
        public string SESS_D_C { get; set; }

        /// <summary>
        /// NASC_D_C 9(8)  
        /// </summary>
        [HisFieldInfoMapping(50, 8)]
        public int NASC_D_C { get; set; }

        /// <summary>
        /// MATRICOLA X(16)  
        /// </summary>
        [HisFieldInfoMapping(51, 16)]
        public string MATRICOLA { get; set; }

        // *    CERTIFICATO PER RIPARTENZA
        /// <summary>
        /// CERTIFICATO_PER_RIPAR 9(8)  
        /// </summary>
        [HisFieldInfoMapping(52, 8)]
        public int CERTIFICATO_PER_RIPAR { get; set; }

        #endregion Tracciato Host

        #region nested class
        public class DF1TAB 
        {
            #region Constructor
            internal DF1TAB()
            { }
            #endregion Constructor

            #region tracciato COBOL
            //02  DF1TAB   OCCURS 55.                                      
            //03   DFANNO1            PIC 9(4).                        
            //03   DFCPEN1            PIC S9(7)V9(4) COMP-3.           
            //03   DFCFAM1            PIC S9(7)V9(4) COMP-3.           
            //03   DFCMSOC            PIC S9(7)V9(4) COMP-3.           
            //03   DFCCOMB            PIC S9(7)V9(4) COMP-3.           
            //03   DFCAACC            PIC S9(7)V9(4) COMP-3.           
            //03   FILLER             PIC X.  
            #endregion tracciato COBOL

            #region Tracciato Host
            // 02  DF1TAB   OCCURS 55.
            /// <summary>
            /// DFANNO1 9(4)  
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public short DFANNO1 { get; set; }

            /// <summary>
            /// DFCPEN1 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(1, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal DFCPEN1 { get; set; }

            /// <summary>
            /// DFCFAM1 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(2, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal DFCFAM1 { get; set; }

            /// <summary>
            /// DFCMSOC S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(3, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal DFCMSOC { get; set; }

            /// <summary>
            /// DFCCOMB S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(4, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal DFCCOMB { get; set; }

            /// <summary>
            /// DFCAACC S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(5, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal DFCAACC { get; set; }

            /// <summary>
            /// FILLER X  
            /// </summary>
            [HisFieldInfoMapping(6, 1)]
            public string FILLER4 { get; set; }
            #endregion Tracciato Host
        }

        public class Supplemento
        {
            #region Constructor
            internal Supplemento()
            { }
            #endregion Constructor

            #region tracciato COBOL
            //            *****               TABELLA DATI CALCOLO SUPPLEMETI                  
            //                    05  TAB-SUPPLEMENTI.                             
            //                        06  DF9703-C     OCCURS 10.                  
            //  **********                           CODICE GESTIONE               
            //                            07  DF97031-C         PIC X.             
            //  **********                           DECORRENZA                    
            //                            07  DF97032-CA        PIC 9999.          
            //                            07  DF97032-CM        PIC 99.            
            //**********                           N.  SETTIMANE                   
            //                            07  DF97033-C         PIC 9(4).          
            //**********                           RETRIBUZ. MEDIA SETTIM.         
            //                            07  DF97034-C     PIC S9(7)V9999 COMP-3. 
            //**********                           IMPORTO IVS                     
            //                            07  DF97035-C     PIC S9(7)V9999 COMP-3. 
            #endregion tracciato COBOL

            #region Tracciato Host
            //*****               TABELLA DATI CALCOLO SUPPLEMETI
            // 05  TAB-SUPPLEMENTI.
            // 06  DF9703-C     OCCURS 10.
            //**********                           CODICE GESTIONE
            /// <summary>
            /// DF97031_C X  
            /// </summary>
            [HisFieldInfoMapping(0, 1)]
            public string DF97031_C { get; set; }

            //**********                           DECORRENZA
            /// <summary>
            /// DF97032_CA 9(4)  
            /// </summary>
            [HisFieldInfoMapping(1, 4)]
            public short DF97032_CA { get; set; }

            /// <summary>
            /// DF97032_CM 99  
            /// </summary>
            [HisFieldInfoMapping(2, 2)]
            public short DF97032_CM { get; set; }

            //**********                           N.  SETTIMANE
            /// <summary>
            /// DF97033_C 9(4)  
            /// </summary>
            [HisFieldInfoMapping(3, 4)]
            public short DF97033_C { get; set; }

            //**********                           RETRIBUZ. MEDIA SETTIM.
            /// <summary>
            /// DF97034_C S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(4, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal DF97034_C { get; set; }

            //**********                           IMPORTO IVS
            /// <summary>
            /// DF97035_C S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(5, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal DF97035_C { get; set; }

            #endregion Tracciato Host
        }

        public class Solidarieta
        {
            #region Constructor
            internal Solidarieta()
            { }
            #endregion Constructor

            #region tracciato COBOL
            //**********                  CONTRIBUTO DI SOLIDARIETA    
            //  05  TAB-SOLIDARIETA.                                   
            //      06  EL-SOLIDARIETA   OCCURS 6.                     
            //         07  MESE-SOLIDA   PIC 99.                       
            //         07  IMP-SOLIDA    PIC S9(7)V9999 COMP-3.   
            #endregion tracciato COBOL

            #region Tracciato Host
            //**********                  CONTRIBUTO DI SOLIDARIETA
            // 05  TAB-SOLIDARIETA.
            // 06  EL-SOLIDARIETA   OCCURS 6.
            /// <summary>
            /// MESE_SOLIDA 99  
            /// </summary>
            [HisFieldInfoMapping(0, 2)]
            public short MESE_SOLIDA { get; set; }

            /// <summary>
            /// IMP_SOLIDA S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(1, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal IMP_SOLIDA { get; set; }
            #endregion Tracciato Host
        }

        public class StampaPLCI
        {
            #region Constructor
            internal StampaPLCI()
            { }
            #endregion Constructor

            #region tracciato COBOL  

            //      * DATI PER STAMPA PLCI :   37 BYTES RIPETUTI 5 VOLTE = 185        
            //                       05  D15105-E   OCCURS 5.                         
            //                         06  D15105-E-ITA.                              
            //**********                                           DECORRENZA              
            //                         10  D15105-E-1.                                
            //                             15 D15105-E-1A   PIC X.
            //                             15 D15105-E-1B   PIC X.
            //**********                                           CTR ITALIANI       
            //                         10  D15105-E-2    PIC S9999.                   
            //**********                                           VIRTUALE           
            //                         10  D15105-E-3    PIC S9(7)V9999 COMP-3.       
            //**********                                           COEFF.RIDUZ.       
            //                         10  D15105-E-4    PIC S9(5).                   
            //**********                                           PENS.DIRETTA       
            //                         10  D15105-E-5    PIC S9(7)V9999 COMP-3.       
            //**********                                           PERC.SUPERSTITI    
            //                         10  D15105-E-6    PIC 9V9999 COMP-3.                     
            //**********                                           PENS.SUPERSTITI    
            //                         10  D15105-E-7    PIC S9(7)V9999 COMP-3.       
            //**********                                           PENS.ESTERA        
            //                         10  D15105-E-8    PIC S9(7)V9999 COMP-3.       
            //**********                                           STATO 1    


            //                       06  D15105-E-ST   OCCURS 4.                      
            //                         10  D15105-E-ST1  PIC 99.                      
            //**********                                           CTR STATO 1        
            //                         10  D15105-E-CTR1 PIC 9999.                    
            //      * DATI PER STAMPA PLCI :  253 BYTES RIPETUTI 4 VOLTE = 1012       
            //      * DATI STATI ESTERI    
            #endregion tracciato COBOL

            #region Tracciato Host
            // * DATI PER STAMPA PLCI :   37 BYTES RIPETUTI 5 VOLTE = 185
            // 05  D15105-E   OCCURS 5.
            // 06  D15105-E-ITA.
            //**********                                           DECORRENZA
            // 10  D15105-E-1.
            /// <summary>
            /// D15105_E_1A X  NONE
            /// </summary>
            [HisFieldInfoMapping(0, 1, CobolType = CobolType.Untraslate)]
            public short D15105_E_1A { get; set; }

            /// <summary>
            /// D15105_E_1B X  NONE
            /// </summary>
            [HisFieldInfoMapping(1, 1, CobolType = CobolType.Untraslate)]
            public short D15105_E_1B { get; set; }

            //**********                                           CTR ITALIANI
            /// <summary>
            /// D15105_E_2 S9(4)  
            /// </summary>
            [HisFieldInfoMapping(2, 4, CobolType = CobolType.Signed)]
            public short D15105_E_2 { get; set; }

            //**********                                           VIRTUALE
            /// <summary>
            /// D15105_E_3 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(3, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal D15105_E_3 { get; set; }

            //**********                                           COEFF.RIDUZ.
            /// <summary>
            /// D15105_E_4 S9(5)  
            /// </summary>
            [HisFieldInfoMapping(4, 5, CobolType = CobolType.Signed)]
            public int D15105_E_4 { get; set; }

            //**********                                           PENS.DIRETTA
            /// <summary>
            /// D15105_E_5 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(5, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal D15105_E_5 { get; set; }

            //**********                                           PERC.SUPERSTITI
            /// <summary>
            /// D15105_E_6 9V9999 COMP-3  
            /// </summary>
            [HisFieldInfoMapping(6, 3, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal D15105_E_6 { get; set; }

            //**********                                           PENS.SUPERSTITI
            /// <summary>
            /// D15105_E_7 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(7, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal D15105_E_7 { get; set; }

            //**********                                           PENS.ESTERA
            /// <summary>
            /// D15105_E_8 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(8, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal D15105_E_8 { get; set; }

            //**********                                           STATO 1

            [HisComplexAreaInfoMapping(9, ListCount = 4)]
            public List<D15105> LISTAD15105 { get; set; }
            #endregion Tracciato Host
        }

        public class D15105
        {
            #region Constructor
            internal D15105()
            { }
            #endregion Constructor

            #region tracciato COBOL
            //                                   06  D15105-E-ST   OCCURS 4.                      
            //                         10  D15105-E-ST1  PIC 99.                      
            //**********                                           CTR STATO 1        
            //                         10  D15105-E-CTR1 PIC 9999. 
            #endregion tracciato COBOL

            #region Tracciato Host
            // 06  D15105-E-ST   OCCURS 4.
            /// <summary>
            /// D15105_E_ST1 99  
            /// </summary>
            [HisFieldInfoMapping(0, 2)]
            public short D15105_E_ST1 { get; set; }

            //**********                                           CTR STATO 1
            // 10  D15105-E-CTR1 PIC 9(4).
            [HisFieldInfoMapping(1, 4)]
            public short D15105_E_CTR1 { get; set; }
            #endregion Tracciato Host
        }

        public class D15106
        {
            #region Constructor
            internal D15106()
            { }
            #endregion Constructor

            #region tracciato COBOL
            //                                   05  D15106-E   OCCURS 4.                         
            //***********                                             CODICE STATO 1  
            //                         10  D15107-E      PIC 99.                      
            //***********                                             CODICE ISTITUZ  
            //                         10  D15108-E      PIC 999.                     
            //***********                                             (250 BYTES)     
            //                         10  D15109-E    OCCURS 25.                     
            //***********                                             DECORRENZA              
            //                           15  D15109-E-DEC.                            
            //                               20  D15109-E-DEC1        PIC 9(4).
            //                               20  D15109-E-DEC2        PIC 99.
            //***********                                             CESSAZIONE         
            //                           15  D15109-E-CES.                            
            //                               20  D15109-E-CES1        PIC 9(4).
            //                               20  D15109-E-CES2        PIC 99.
            //***********                                             IMP.ESTERO      
            //                           15  D15109-E-IMP  PIC S9(7)V9(8).   
            #endregion tracciato COBOL

            #region Tracciato Host
            // 05  D15106-E   OCCURS 4.
            //***********                                             CODICE STATO 1
            /// <summary>
            /// D15107_E 99  
            /// </summary>
            [HisFieldInfoMapping(0, 2)]
            public short D15107_E { get; set; }

            //***********                                             CODICE ISTITUZ
            /// <summary>
            /// D15108_E 999  
            /// </summary>
            [HisFieldInfoMapping(1, 3)]
            public short D15108_E { get; set; }

            [HisComplexAreaInfoMapping(2, ListCount = 25)]
            public List<D15109_E> LISTAD15109_E { get; set; }

            #endregion Tracciato Host
        }

        public class D15109_E
        {
            #region Constructor
            internal D15109_E()
            { }
            #endregion Constructor

            #region tracciato COBOL
            //                         10  D15109-E    OCCURS 25.                     
            //***********                                             DECORRENZA              
            //                           15  D15109-E-DEC.                            
            //                               20  D15109-E-DEC1        PIC 9(4).
            //                               20  D15109-E-DEC2        PIC 99.
            //***********                                             CESSAZIONE         
            //                           15  D15109-E-CES.                            
            //                               20  D15109-E-CES1        PIC 9(4).
            //                               20  D15109-E-CES2        PIC 99.
            //***********                                             IMP.ESTERO      
            //                           15  D15109-E-IMP  PIC S9(7)V9(8). 
            #endregion tracciato COBOL

            #region Tracciato Host
            //***********                                             (250 BYTES)
            // 10  D15109-E    OCCURS 25.
            //***********                                             DECORRENZA
            // 15  D15109-E-DEC.
            /// <summary>
            /// D15109_E_DEC1 9(4)  
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public short D15109_E_DEC1 { get; set; }

            /// <summary>
            /// D15109_E_DEC2 99  
            /// </summary>
            [HisFieldInfoMapping(1, 2)]
            public short D15109_E_DEC2 { get; set; }

            //***********                                             CESSAZIONE
            // 15  D15109-E-CES.
            /// <summary>
            /// D15109_E_CES1 9(4)  
            /// </summary>
            [HisFieldInfoMapping(2, 4)]
            public short D15109_E_CES1 { get; set; }

            /// <summary>
            /// D15109_E_CES2 99  
            /// </summary>
            [HisFieldInfoMapping(3, 2)]
            public short D15109_E_CES2 { get; set; }

            //***********                                             IMP.ESTERO
            /// <summary>
            /// D15109_E_IMP S9(7)V9(8)  
            /// </summary>
            [HisFieldInfoMapping(4, 15, Scale = 8, CobolType = CobolType.Signed)]
            public decimal D15109_E_IMP { get; set; }
            #endregion Tracciato Host
        }

        public class Paragrafi_CI28
        {
            #region Constructor
            internal Paragrafi_CI28()
            { }
            #endregion Constructor

            #region tracciato COBOL
            //***********           CODICI PARAGRAFI PER IL CI28                      
            //           05  PARAGRAFI-CI28.                              
            //               06  PAR-CI28   OCCURS  5     PIC XX.  
            #endregion tracciato COBOL

            #region Tracciato Host
            //***********           CODICI PARAGRAFI PER IL CI28
            // 05  PARAGRAFI-CI28.
            /// <summary>
            /// PAR_CI28 XX  
            /// </summary>
            [HisFieldInfoMapping(0, 2)]
            public string PAR_CI28 { get; set; }
            #endregion Tracciato Host
        }

        public class Tassa
        {
            #region Constructor
            internal Tassa()
            { }
            #endregion Constructor

            #region tracciato COBOL
            //***********    =  TABELLA TASSE                                         
            //           05  TAB-TASSE.                                   
            //               06  EL-TASSE   OCCURS 6.                     
            //                   07  MESE-TASSE         PIC 99.           
            //                   07  IMPO-TASSE COMP-3  PIC S9(5)V9999.  
            #endregion tracciato COBOL

            #region Tracciato Host
            //***********    =  TABELLA TASSE
            // 05  TAB-TASSE.
            // 06  EL-TASSE   OCCURS 6.
            /// <summary>
            /// MESE_TASSE 99  
            /// </summary>
            [HisFieldInfoMapping(0, 2)]
            public short MESE_TASSE { get; set; }

            /// <summary>
            /// IMPO_TASSE S9(5)V9(4)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(1, 5, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal IMPO_TASSE { get; set; }
            #endregion Tracciato Host
        }

        public class Dati503
        {
            #region Constructor
            internal Dati503()
            { }
            #endregion Constructor

            #region tracciato COBOL
            //***********    =  DATI PER 503                                          
            //           05  ELEM-503   OCCURS 6.                         
            //               06  DF9325-C-503      PIC S9(7)V9999 COMP-3. 
            //               06  DF9326-C-503      PIC S9999.    
            #endregion tracciato COBOL

            #region Tracciato Host
            //***********    =  DATI PER 503
            // 05  ELEM-503   OCCURS 6.
            /// <summary>
            /// DF9325_C_503 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal DF9325_C_503 { get; set; }

            /// <summary>
            /// DF9326_C_503 S9(4)  
            /// </summary>
            [HisFieldInfoMapping(1, 4, CobolType = CobolType.Signed)]
            public short DF9326_C_503 { get; set; }
            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
