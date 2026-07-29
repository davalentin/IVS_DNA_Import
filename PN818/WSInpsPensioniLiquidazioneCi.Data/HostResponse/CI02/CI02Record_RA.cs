using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class CI02Record_RA
    {
        #region Constructor
        internal CI02Record_RA()
        {
        }
        #endregion Constructor

        #region tracciato COBOL
        //               01  CIRCRISP.                                                    
        //      ****************************************************************  
        //      *** RECORD DI CONTROLLO DEL MESSAGGIO DI RISPOSTA      *********  
        //      *** LUNGHEZZA 240 BYTES       **********************************  
        //      ****************************************************************                  
        //      ***********************************************************       
        //      *               INIZIO 1 SETTORE                          *       
        //      ***********************************************************       
        //      *                                                                 
        //      * CODICE SEDE                                                     
        //           02  DF01                      PIC X(4).                      
        //      * CATEGORIA IN CHIARO                                             
        //           02  DF02-A                    PIC X(8).                      
        //      *  CERTIFICATO                                                    
        //           02  DF03-A                    PIC 9(8).                      
        //      * COGNOME NOME                       
        //           02  DF04-A			         PIC X(32).                   
        //      * SESSO                                                           
        //           02  DF05                      PIC X.                         
        //      * INDIRIZZO                                                       
        //           02  DF06-IND.                                                
        //               05  DF06-IND1     PIC X(52).                             
        //               05  DF06-IND2     PIC X(52).                             
        //               05  DF06-IND3     PIC X(52).                             
        //           02  DF06-CIV       PIC X(18).                                
        //           02  FILLER                    PIC X(35).                     
        //           02  DF06-FRAZ      PIC X(35).                                
        //           02  DF06-IND4     PIC X(52).                                 
        //      * CAP                                                             
        //           02  DF07-A        PIC X(9).                                  
        //      * COMUNE E PROVINCIA                                              
        //           02  DF081-A       PIC X(37).                                 
        //           02  DF082-A       PIC XXX.                                   
        //      * DATA NASCITA  GGMMAAAA                                       
        //2000           03 DF09-G                 PIC 99.                        
        //2000           03 DF09-M                 PIC 99.                        
        //2000           03 DF09-AA                PIC 9(4).                      
        //      * CODICE SINDACATO                                                
        //2000       02  DF34X-A                   PIC XX.                        
        //      * DECORRENZA PENSIONE    AAAAMM                                 
        //2000           03  DF10-AA               PIC 9(4).                      
        //2000           03  DF10-M                PIC 99.                        
        //      * DATA FINALE CALC ARRETRATI   GGMMAAAA                              
        //2000           03  DF111-G               PIC 99.                        
        //2000           03  DF112-M               PIC 99.                        
        //2000           03  DF113-A               PIC 9(4).                      
        //      * N. DOMANDA                                                      
        //2000       02  DFXDOM                    PIC 9(8).                      
        //      *                                                                 
        //2000       02  FILLER                    PIC X(6).                      
        //      * NATURA PENSIONE                                                 
        //           02  DF15                      PIC X(3).                      
        //      * ARRETRATI LORDI                                                 
        //           02  DF16-A                    PIC S9(7)V9(4) COMP-3.         
        //      * ONPI SU ARRETRATO                                               
        //           02  DF17-A                    PIC S9(5)V9(4) COMP-3.         
        //      * ERAR SU ARRETRATO                                               
        //           02  DF18-A                    PIC S9(7)V9(4) COMP-3.         
        //      * SIND SU ARRETRATO                                               
        //           02  DF19-A                    PIC S9(7)V9(4) COMP-3.         
        //      * TIPO SENTENZA 495 E 240                                         
        //           02  DF27                      PIC 9.                         
        //      * COD UFF PAG                                                     
        //      *    02  DF28A                     PIC X(3).                      
        //           02  DF28-A.                                                  
        //               03 DF281-A                PIC X.
        //               03 DF282-A                PIC X.
        //               03 DF283-A                PIC X.
        //      * ANNO VALIDITA DEL P1 AAAA                                       
        //2000       02  DF29                      PIC 9(4).                      
        //      * CODICE FISCALE                                                  
        //           02  DFFISC                    PIC X(16).                     
        //      *                                                                 
        //           02  FILLER                        PIC X(5).                  
        //      *                                                                 
        //      * COMPOSIZIONE FAMILIARE                                          
        //2000       02  DF32.                                                    
        //               03  DF321                 PIC 9.                         
        //               03  DF322                 PIC 9.                         
        //2000           03  DF323                 PIC 99.                        
        //      * NUM COMPONENTI FAMILIARI                                        
        //2000       02  DF35                      PIC 99.                        
        //      * CODICE PAGAMENTO ARRETRATI                                      
        //2000       02  DF36                      PIC 9.                         
        //      * DATA EMISSIONE   AAAAMM                                              
        //2000           03  DF372A                PIC 9(4).                      
        //2000           03  DF372M                PIC 99.                        
        //      * ULTIMO MENSILE LORDO                                            
        //2000       02  DF40                      PIC S9(7)V9(4) COMP-3.         
        //      * 13A ANNO IN CORSO LORDA                                         
        //2000       02  DF41                      PIC S9(7)V9(4) COMP-3.         
        //      * COD UFF PAG 2 QUOTA                                             
        //           02  DF42                      PIC X(3).                      
        //      * ULTIMA TRATT SINDACALE MENSILE                                  
        //2000       02  DF43                      PIC S9(5)V9(4) COMP-3.         
        //      * ARRETRATO ANTE 1-1-96 PER SENTENZA 495-240                      
        //           02  DF44                      PIC S9(7)V9(4) COMP-3.         
        //      * ARRETRATO POST 1-1-96 PER SENTENZA 495-240                      
        //           02  DF45                      PIC S9(7)V9(4) COMP-3.         
        //      * CAUSA CARICO                                                    
        //           02  DF46                      PIC 9. 
        //      ***********************************************************       
        //      *        FINE 1 SETTORE DEL  PRIMO RECORD                 *       
        //      ***********************************************************       
        //      ********************************************************          
        //      *        INIZIO SECONDO    SETTORE                     *          
        //      ********************************************************          
        //      * FLAG VIA CAVO                                                   
        //           02  DFMEC                     PIC X.                         
        //      *                                                                 
        //           02  DFRED1                    PIC X.                         
        //      * DETRAZIONI IMPOSTA                                              
        //           02  DFDETR                    PIC S9(7)V9(4) COMP-3.         
        //      * ALIQUOTA MEDIA                                                  
        //           02  DFALIQ                    PIC 9(4).                      
        //      * CODICI DETRAZ IMPOSTA                                            
        //      *                                                                 
        //2000       02  DFNCOD                    PIC 99.                        
        //      * IMPONIB ARRETRATO ANNI PRECEDENTI                               
        //           02  DF68                      PIC S9(7)V9(4) COMP-3.         
        //      * IMPOSTA NETTA     ANNI PRECEDENTI                               
        //           02  DF69-A                    PIC S9(7)V9(4) COMP-3.         
        //      * IMPOSTA NETTA     ANNO CORRENTE                                 
        //           02  DF71-A                    PIC S9(7)V9(4) COMP-3.         
        //      * TRATT   ERAR 13A                                                
        //           02  DF72                      PIC S9(7)V9(4) COMP-3.         
        //      * TRATT NON DEDUCIBILI                                            
        //2000       02  DF81                      PIC S9(7)V9(4) COMP-3.         
        //      * DATA RIPRISTINO    AAAAMM                                  
        //2000           03  DF62XA                PIC 9(4).                      
        //2000           03  DF62XM                PIC 99.                        
        //      * DECORRENZA INTERESSI LEGALI  GGMMAAAA                           
        //               03  DFDILG                PIC 99.                        
        //               03  DFDILM                PIC 99.                        
        //               03  DFDILS                PIC 99.                        
        //               03  DFDILA                PIC 99.                        
        //      * PATRONATO                                                       
        //2000       02  DF12.                                                    
        //      * COD PATRONATO                                                   
        //2000           03  DF121                     PIC 99.                    
        //      * COD PATRONATO                                                   
        //2000           03  DF122                     PIC X.                     
        //      * PATRONATO IN CHIARO                                             
        //2000           03  DF123                     PIC X(10).                 
        //      * N PRATICA PATRONATO                                             
        //2000           03  DF124                     PIC S9(7) COMP-3.          
        //      *                                                                 
        //****************  DECORRENZA ASSEGNO DI ACCOMPAGNO AAAAMM               
        //           02  DEC-ACCO                      PIC 9(6).                  
        //           02  DF96542-C                     PIC S9(9).                 
        //2000       02  FILLER                        PIC X(10).                 
        //      *                                                                 
        //      * DECORRENZA SUPPLEMENTO AAAAMM                                   
        //               03  DF9329-CA                 PIC 9(4).                  
        //               03  DF9329-CM                 PIC 99.                    
        //      * IMPORTO ART1 L 140-544                                          
        //           02  DF9316-C                      PIC 9(7)V9(4).             
        //      * CODICE ART 3-4-5-DPCM                                           
        //           02  DF9322-C                      PIC X.                     
        //      * DECORRENZA ART 6 L 140 AAAAMM                                   
        //           02  DF9314C-C                      PIC 9(6).                 
        //      * CODICE LEGGE 59                                                 
        //           02  DF9342-C                      PIC X.                     
        //      * CONTRIBUTI  GP1AV08 GP2BN52                                     
        //           02  DF9326-C                      PIC 9(4).                  
        //      * CONTRIBUTI  GP1AV09                                             
        //           02  DF9327-C                      PIC 9(4).                  
        //      * RMS                                                             
        //           02  DF9325-C                      PIC 9(7)V9(4).             
        //      * CONTRIBUTI BC02 + BC08                                          
        //           02  DF9324-C                      PIC 9(4).                  
        //      * PENSIONE NON CAMBIA                                             
        //           02  DF477                         PIC 9.                     
        //      * IVS                                                             
        //           02  DF76                          PIC 9(7)V9(4).             
        //      ********************************************************          
        //      *    FINE SECONDO SETTORE DEL PRIMO RECORD             *          
        //      ********************************************************          
        //      *                                                           



        //      ********************************************************          
        //      *    INIZIO TERZO SETTORE DEL PRIMO RECORD             *          
        //      ********************************************************          
        //      *                                                                 
        //      * TIPO PROVENIENZA                                                
        //           02  DFPROV                    PIC 9.                         
        //      * MOTIV                                                           
        //           02  DFMOT1                    PIC X.                         
        //      * MOTIV                                                           
        //           02  DFMOT2                    PIC X.                         
        //      * INDIRIZZO TUTORE                                                
        //           02  DFAP31                    PIC X(35).                     
        //      * COMUNE RESIDENZA                                                
        //           02  DFAP32                    PIC X(22).                     
        //      * PROV   RESIDENZA                                                
        //           02  DFAP33                    PIC XXX.                       
        //      *                                                                 
        //           02  FILLER                    PIC X.                         
        //      * CODICE DELEGATO                                                 
        //           02  DF30C                     PIC X.                         
        //      * COGNOME NOME                                                    
        //           02  DF30N                     PIC X(31).                     
        //      * CODICE FISCALE                                                  
        //           02  DF30F                     PIC X(16).                     
        //      * DATA NASCITA           GGMMAAAA                                 
        //           02  DF30D                     PIC 9(8).                      
        //      * CAP                                                             
        //           02  DFAP34                    PIC 9(5).                      
        //      * DA RIVEDERE                                                     
        //**********    DEC CALC RIC  = W1DERIP   ********                        
        //2000       02  DF9314-C.                                                
        //               03  DF9314A-C                 PIC 9(4).                  
        //               03  DF9314M-C                 PIC 99.                    
        //           02  ESTERO                        PIC X.                     
        //2000*******  ELIMINATO IL CAMPO SOSTITUITO CON DFDETR                  
        //           02  DEC-CAL-ARR-SEDE              PIC 9(6).                  
        //************   AAAAMM                                     
        //               03 DATA-CAL-ARR-AA            PIC 9(4).
        //               03 DATA-CAL-ARR-MM            PIC 99.
        //           02  TIPO-PEREQUAZIONE             PIC X.                     
        //      *                                                                 
        //      * CODICE SENTENXA                                                 
        //      *                                                                 
        //           02  DF9712                        PIC 99.                    
        //           02  FILLER                        PIC X(2).                  
        //      *******************************************************           
        //      *   FINE TERZO SETTORE DEL PRIMO RECORD               *           
        //      *******************************************************   
        //        2000       02  DF70 OCCURS 16.                                         
        //               03  DF701-A 				  PIC X(30).                  
        //      * SESSO                                                           
        //               03  DF702-A                PIC X.                        
        //      * SIGLA                                                           
        //               03  DF703-A                PIC X.                        
        //      * DATA NASCITA  GGMMAAAA                                         
        //2000               04  DF704-G            PIC 99.                       
        //2000               04  DF704-M            PIC 99.                       
        //2000               04  DF704-AA           PIC 9999.                     
        //      * AGGIUNTA DI FAMIGLIA                                            
        //2000           03  DF705-A                PIC S9(5)V9999 COMP-3.        
        //      * IMPORTO NON CUMULABILE                                          
        //2000           03  DF708-A                PIC S9(5)V9999 COMP-3.        
        //      * DATA CESSAZIONE           AAAAMM                               
        //2000               04  DF706-AA           PIC 9(4).                     
        //2000               04  DF706-M            PIC 99.                       
        //      * DATA ACQUISIZIONE         AAAAMM                              
        //2000               04  DF707-AA           PIC 9(4).                     
        //2000               04  DF707-M            PIC 99.                       
        //2000           03  FILLER                 PIC X.   

        #endregion tracciato COBOL

        #region Tracciato Host
        // 01  CIRCRISP.
        //****************************************************************
        //*** RECORD DI CONTROLLO DEL MESSAGGIO DI RISPOSTA      *********
        //*** LUNGHEZZA 240 BYTES       **********************************
        //****************************************************************
        //***********************************************************
        // *               INIZIO 1 SETTORE                          *
        //***********************************************************
        // * CODICE SEDE
        /// <summary>
        /// DF01 X(4)  
        /// </summary>
        [HisFieldInfoMapping(0, 4)]
        public string DF01 { get; set; }

        // * CATEGORIA IN CHIARO
        /// <summary>
        /// DF02_A X(8)  
        /// </summary>
        [HisFieldInfoMapping(1, 8)]
        public string DF02_A { get; set; }

        // *  CERTIFICATO
        /// <summary>
        /// DF03_A 9(8)  
        /// </summary>
        [HisFieldInfoMapping(2, 8)]
        public int DF03_A { get; set; }

        // * COGNOME NOME
        /// <summary>
        /// DF04_A X(32)  
        /// </summary>
        [HisFieldInfoMapping(3, 32)]
        public string DF04_A { get; set; }

        // * SESSO
        /// <summary>
        /// DF05 X  
        /// </summary>
        [HisFieldInfoMapping(4, 1)]
        public string DF05 { get; set; }

        // * INDIRIZZO
        // 02  DF06-IND.
        /// <summary>
        /// DF06_IND1 X(52)  
        /// </summary>
        [HisFieldInfoMapping(5, 52)]
        public string DF06_IND1 { get; set; }

        /// <summary>
        /// DF06_IND2 X(52)  
        /// </summary>
        [HisFieldInfoMapping(6, 52)]
        public string DF06_IND2 { get; set; }

        /// <summary>
        /// DF06_IND3 X(52)  
        /// </summary>
        [HisFieldInfoMapping(7, 52)]
        public string DF06_IND3 { get; set; }

        /// <summary>
        /// DF06_CIV X(18)  
        /// </summary>
        [HisFieldInfoMapping(8, 18)]
        public string DF06_CIV { get; set; }

        /// <summary>
        /// FILLER X(35)  
        /// </summary>
        [HisFieldInfoMapping(9, 35)]
        public string FILLER { get; set; }

        /// <summary>
        /// DF06_FRAZ X(35)  
        /// </summary>
        [HisFieldInfoMapping(10, 35)]
        public string DF06_FRAZ { get; set; }

        /// <summary>
        /// DF06_IND4 X(52)  
        /// </summary>
        [HisFieldInfoMapping(11, 52)]
        public string DF06_IND4 { get; set; }

        // * CAP
        /// <summary>
        /// DF07_A X(9)  
        /// </summary>
        [HisFieldInfoMapping(12, 9)]
        public string DF07_A { get; set; }

        // * COMUNE E PROVINCIA
        /// <summary>
        /// DF081_A X(37)  
        /// </summary>
        [HisFieldInfoMapping(13, 37)]
        public string DF081_A { get; set; }

        /// <summary>
        /// DF082_A XXX  
        /// </summary>
        [HisFieldInfoMapping(14, 3)]
        public string DF082_A { get; set; }

        // * DATA NASCITA  GGMMAAAA
        /// <summary>
        /// DF09_G 99  
        /// </summary>
        [HisFieldInfoMapping(15, 2)]
        public short DF09_G { get; set; }

        /// <summary>
        /// DF09_M 99  
        /// </summary>
        [HisFieldInfoMapping(16, 2)]
        public short DF09_M { get; set; }

        /// <summary>
        /// DF09_AA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(17, 4)]
        public short DF09_AA { get; set; }

        // * CODICE SINDACATO
        /// <summary>
        /// DF34X_A XX  
        /// </summary>
        [HisFieldInfoMapping(18, 2)]
        public string DF34X_A { get; set; }

        // * DECORRENZA PENSIONE    AAAAMM
        /// <summary>
        /// DF10_AA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(19, 4)]
        public short DF10_AA { get; set; }

        /// <summary>
        /// DF10_M 99  
        /// </summary>
        [HisFieldInfoMapping(20, 2)]
        public short DF10_M { get; set; }

        // * DATA FINALE CALC ARRETRATI   GGMMAAAA
        /// <summary>
        /// DF111_G 99  
        /// </summary>
        [HisFieldInfoMapping(21, 2)]
        public short DF111_G { get; set; }

        /// <summary>
        /// DF112_M 99  
        /// </summary>
        [HisFieldInfoMapping(22, 2)]
        public short DF112_M { get; set; }

        /// <summary>
        /// DF113_A 9(4)  
        /// </summary>
        [HisFieldInfoMapping(23, 4)]
        public short DF113_A { get; set; }

        // * N. DOMANDA
        /// <summary>
        /// DFXDOM 9(8)  
        /// </summary>
        [HisFieldInfoMapping(24, 8)]
        public int DFXDOM { get; set; }

        /// <summary>
        /// FILLER X(6)  
        /// </summary>
        [HisFieldInfoMapping(25, 6)]
        public string FILLER1 { get; set; }

        // * NATURA PENSIONE
        /// <summary>
        /// DF15 X(3)  
        /// </summary>
        [HisFieldInfoMapping(26, 3)]
        public string DF15 { get; set; }

        // * ARRETRATI LORDI
        /// <summary>
        /// DF16_A S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(27, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF16_A { get; set; }

        // * ONPI SU ARRETRATO
        /// <summary>
        /// DF17_A S9(5)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(28, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF17_A { get; set; }

        // * ERAR SU ARRETRATO
        /// <summary>
        /// DF18_A S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(29, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF18_A { get; set; }

        // * SIND SU ARRETRATO
        /// <summary>
        /// DF19_A S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(30, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF19_A { get; set; }

        // * TIPO SENTENZA 495 E 240
        /// <summary>
        /// DF27 9  
        /// </summary>
        [HisFieldInfoMapping(31, 1)]
        public short DF27 { get; set; }

        // * COD UFF PAG
        // 02  DF28-A.
        /// <summary>
        /// DF281_A X  
        /// </summary>
        [HisFieldInfoMapping(33, 1)]
        public string DF281_A { get; set; }

        /// <summary>
        /// DF282_A X  
        /// </summary>
        [HisFieldInfoMapping(34, 1)]
        public string DF282_A { get; set; }

        /// <summary>
        /// DF283_A X  
        /// </summary>
        [HisFieldInfoMapping(35, 1)]
        public string DF283_A { get; set; }

        // * ANNO VALIDITA DEL P1 AAAA
        /// <summary>
        /// DF29 9(4)  
        /// </summary>
        [HisFieldInfoMapping(36, 4)]
        public short DF29 { get; set; }

        // * CODICE FISCALE
        /// <summary>
        /// DFFISC X(16)  
        /// </summary>
        [HisFieldInfoMapping(37, 16)]
        public string DFFISC { get; set; }

        /// <summary>
        /// FILLER X(5)  
        /// </summary>
        [HisFieldInfoMapping(38, 5)]
        public string FILLER2 { get; set; }

        // * COMPOSIZIONE FAMILIARE
        // 2000       02  DF32.
        /// <summary>
        /// DF321 9  
        /// </summary>
        [HisFieldInfoMapping(39, 1)]
        public short DF321 { get; set; }

        /// <summary>
        /// DF322 9  
        /// </summary>
        [HisFieldInfoMapping(40, 1)]
        public short DF322 { get; set; }

        /// <summary>
        /// DF323 99  
        /// </summary>
        [HisFieldInfoMapping(41, 2)]
        public short DF323 { get; set; }

        // * NUM COMPONENTI FAMILIARI
        /// <summary>
        /// DF35 99  
        /// </summary>
        [HisFieldInfoMapping(42, 2)]
        public short DF35 { get; set; }

        // * CODICE PAGAMENTO ARRETRATI
        /// <summary>
        /// DF36 9  
        /// </summary>
        [HisFieldInfoMapping(43, 1)]
        public short DF36 { get; set; }

        // * DATA EMISSIONE   AAAAMM
        /// <summary>
        /// DF372A 9(4)  
        /// </summary>
        [HisFieldInfoMapping(44, 4)]
        public short DF372A { get; set; }

        /// <summary>
        /// DF372M 99  
        /// </summary>
        [HisFieldInfoMapping(45, 2)]
        public short DF372M { get; set; }

        // * ULTIMO MENSILE LORDO
        /// <summary>
        /// DF40 S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(46, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF40 { get; set; }

        // * 13A ANNO IN CORSO LORDA
        /// <summary>
        /// DF41 S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(47, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF41 { get; set; }

        // * COD UFF PAG 2 QUOTA
        /// <summary>
        /// DF42 X(3)  
        /// </summary>
        [HisFieldInfoMapping(48, 3)]
        public string DF42 { get; set; }

        // * ULTIMA TRATT SINDACALE MENSILE
        /// <summary>
        /// DF43 S9(5)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(49, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF43 { get; set; }

        // * ARRETRATO ANTE 1-1-96 PER SENTENZA 495-240
        /// <summary>
        /// DF44 S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(50, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF44 { get; set; }

        // * ARRETRATO POST 1-1-96 PER SENTENZA 495-240
        /// <summary>
        /// DF45 S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(51, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF45 { get; set; }

        // * CAUSA CARICO
        /// <summary>
        /// DF46 9  
        /// </summary>
        [HisFieldInfoMapping(52, 1)]
        public short DF46 { get; set; }

        //***********************************************************
        // *        FINE 1 SETTORE DEL  PRIMO RECORD                 *
        //***********************************************************
        //********************************************************
        // *        INIZIO SECONDO    SETTORE                     *
        //********************************************************
        // * FLAG VIA CAVO
        /// <summary>
        /// DFMEC X  
        /// </summary>
        [HisFieldInfoMapping(53, 1)]
        public string DFMEC { get; set; }

        /// <summary>
        /// DFRED1 X  
        /// </summary>
        [HisFieldInfoMapping(54, 1)]
        public string DFRED1 { get; set; }

        // * DETRAZIONI IMPOSTA
        /// <summary>
        /// DFDETR S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(55, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DFDETR { get; set; }

        // * ALIQUOTA MEDIA
        /// <summary>
        /// DFALIQ 9(4)  
        /// </summary>
        [HisFieldInfoMapping(56, 4)]
        public short DFALIQ { get; set; }

        // * CODICI DETRAZ IMPOSTA
        /// <summary>
        /// DFNCOD 99  
        /// </summary>
        [HisFieldInfoMapping(57, 2)]
        public short DFNCOD { get; set; }

        // * IMPONIB ARRETRATO ANNI PRECEDENTI
        /// <summary>
        /// DF68 S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(58, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF68 { get; set; }

        // * IMPOSTA NETTA     ANNI PRECEDENTI
        /// <summary>
        /// DF69_A S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(59, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF69_A { get; set; }

        // * IMPOSTA NETTA     ANNO CORRENTE
        /// <summary>
        /// DF71_A S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(60, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF71_A { get; set; }

        // * TRATT   ERAR 13A
        /// <summary>
        /// DF72 S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(61, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF72 { get; set; }

        // * TRATT NON DEDUCIBILI
        /// <summary>
        /// DF81 S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(62, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF81 { get; set; }

        // * DATA RIPRISTINO    AAAAMM
        /// <summary>
        /// DF62XA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(63, 4)]
        public short DF62XA { get; set; }

        /// <summary>
        /// DF62XM 99  
        /// </summary>
        [HisFieldInfoMapping(64, 2)]
        public short DF62XM { get; set; }

        // * DECORRENZA INTERESSI LEGALI  GGMMAAAA
        /// <summary>
        /// DFDILG 99  
        /// </summary>
        [HisFieldInfoMapping(65, 2)]
        public short DFDILG { get; set; }

        /// <summary>
        /// DFDILM 99  
        /// </summary>
        [HisFieldInfoMapping(66, 2)]
        public short DFDILM { get; set; }

        /// <summary>
        /// DFDILS 99  
        /// </summary>
        [HisFieldInfoMapping(67, 2)]
        public short DFDILS { get; set; }

        /// <summary>
        /// DFDILA 99  
        /// </summary>
        [HisFieldInfoMapping(68, 2)]
        public short DFDILA { get; set; }

        // * PATRONATO
        // 2000       02  DF12.
        // * COD PATRONATO
        /// <summary>
        /// DF121 99  
        /// </summary>
        [HisFieldInfoMapping(69, 2)]
        public short DF121 { get; set; }

        // * COD PATRONATO
        /// <summary>
        /// DF122 X  
        /// </summary>
        [HisFieldInfoMapping(70, 1)]
        public string DF122 { get; set; }

        // * PATRONATO IN CHIARO
        /// <summary>
        /// DF123 X(10)  
        /// </summary>
        [HisFieldInfoMapping(71, 10)]
        public string DF123 { get; set; }

        // * N PRATICA PATRONATO
        /// <summary>
        /// DF124 S9(7) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(72, 4, CobolType = CobolType.Comp3)]
        public int DF124 { get; set; }

        //****************  DECORRENZA ASSEGNO DI ACCOMPAGNO AAAAMM
        /// <summary>
        /// DEC_ACCO 9(6)  
        /// </summary>
        [HisFieldInfoMapping(73, 6)]
        public int DEC_ACCO { get; set; }

        /// <summary>
        /// DF96542_C S9(9)  
        /// </summary>
        [HisFieldInfoMapping(74, 9, CobolType = CobolType.Signed)]
        public int DF96542_C { get; set; }

        /// <summary>
        /// FILLER X(10)  
        /// </summary>
        [HisFieldInfoMapping(75, 10)]
        public string FILLER3 { get; set; }

        // * DECORRENZA SUPPLEMENTO AAAAMM
        /// <summary>
        /// DF9329_CA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(76, 4)]
        public short DF9329_CA { get; set; }

        /// <summary>
        /// DF9329_CM 99  
        /// </summary>
        [HisFieldInfoMapping(77, 2)]
        public short DF9329_CM { get; set; }

        // * IMPORTO ART1 L 140-544
        /// <summary>
        /// DF9316_C 9(7)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(78, 11, Scale = 4)]
        public decimal DF9316_C { get; set; }

        // * CODICE ART 3-4-5-DPCM
        /// <summary>
        /// DF9322_C X  
        /// </summary>
        [HisFieldInfoMapping(79, 1)]
        public string DF9322_C { get; set; }

        // * DECORRENZA ART 6 L 140 AAAAMM
        /// <summary>
        /// DF9314C_C 9(6)  
        /// </summary>
        [HisFieldInfoMapping(80, 6)]
        public int DF9314C_C { get; set; }

        // * CODICE LEGGE 59
        /// <summary>
        /// DF9342_C X  
        /// </summary>
        [HisFieldInfoMapping(81, 1)]
        public string DF9342_C { get; set; }

        // * CONTRIBUTI  GP1AV08 GP2BN52
        /// <summary>
        /// DF9326_C 9(4)  
        /// </summary>
        [HisFieldInfoMapping(82, 4)]
        public short DF9326_C { get; set; }

        // * CONTRIBUTI  GP1AV09
        /// <summary>
        /// DF9327_C 9(4)  
        /// </summary>
        [HisFieldInfoMapping(83, 4)]
        public short DF9327_C { get; set; }

        // * RMS
        /// <summary>
        /// DF9325_C 9(7)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(84, 11, Scale = 4)]
        public decimal DF9325_C { get; set; }

        // * CONTRIBUTI BC02 + BC08
        /// <summary>
        /// DF9324_C 9(4)  
        /// </summary>
        [HisFieldInfoMapping(85, 4)]
        public short DF9324_C { get; set; }

        // * PENSIONE NON CAMBIA
        /// <summary>
        /// DF477 9  
        /// </summary>
        [HisFieldInfoMapping(86, 1)]
        public short DF477 { get; set; }

        // * IVS
        /// <summary>
        /// DF76 9(7)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(87, 11, Scale = 4)]
        public decimal DF76 { get; set; }

        //********************************************************
        // *    FINE SECONDO SETTORE DEL PRIMO RECORD             *
        //********************************************************
        //********************************************************
        // *    INIZIO TERZO SETTORE DEL PRIMO RECORD             *
        //********************************************************
        // * TIPO PROVENIENZA
        /// <summary>
        /// DFPROV 9  
        /// </summary>
        [HisFieldInfoMapping(88, 1)]
        public short DFPROV { get; set; }

        // * MOTIV
        /// <summary>
        /// DFMOT1 X  
        /// </summary>
        [HisFieldInfoMapping(89, 1)]
        public string DFMOT1 { get; set; }

        // * MOTIV
        /// <summary>
        /// DFMOT2 X  
        /// </summary>
        [HisFieldInfoMapping(90, 1)]
        public string DFMOT2 { get; set; }

        // * INDIRIZZO TUTORE
        /// <summary>
        /// DFAP31 X(35)  
        /// </summary>
        [HisFieldInfoMapping(91, 35)]
        public string DFAP31 { get; set; }

        // * COMUNE RESIDENZA
        /// <summary>
        /// DFAP32 X(22)  
        /// </summary>
        [HisFieldInfoMapping(92, 22)]
        public string DFAP32 { get; set; }

        // * PROV   RESIDENZA
        /// <summary>
        /// DFAP33 XXX  
        /// </summary>
        [HisFieldInfoMapping(93, 3)]
        public string DFAP33 { get; set; }

        /// <summary>
        /// FILLER X  
        /// </summary>
        [HisFieldInfoMapping(94, 1)]
        public string FILLER4 { get; set; }

        // * CODICE DELEGATO
        /// <summary>
        /// DF30C X  
        /// </summary>
        [HisFieldInfoMapping(95, 1)]
        public string DF30C { get; set; }

        // * COGNOME NOME
        /// <summary>
        /// DF30N X(31)  
        /// </summary>
        [HisFieldInfoMapping(96, 31)]
        public string DF30N { get; set; }

        // * CODICE FISCALE
        /// <summary>
        /// DF30F X(16)  
        /// </summary>
        [HisFieldInfoMapping(97, 16)]
        public string DF30F { get; set; }

        // * DATA NASCITA           GGMMAAAA
        /// <summary>
        /// DF30D 9(8)  
        /// </summary>
        [HisFieldInfoMapping(98, 8)]
        public int DF30D { get; set; }

        // * CAP
        /// <summary>
        /// DFAP34 9(5)  
        /// </summary>
        [HisFieldInfoMapping(99, 5)]
        public int DFAP34 { get; set; }

        // * DA RIVEDERE
        //**********    DEC CALC RIC  = W1DERIP   ********
        // 2000       02  DF9314-C.
        /// <summary>
        /// DF9314A_C 9(4)  
        /// </summary>
        [HisFieldInfoMapping(100, 4)]
        public short DF9314A_C { get; set; }

        /// <summary>
        /// DF9314M_C 99  
        /// </summary>
        [HisFieldInfoMapping(101, 2)]
        public short DF9314M_C { get; set; }

        /// <summary>
        /// ESTERO X  
        /// </summary>
        [HisFieldInfoMapping(102, 1)]
        public string ESTERO { get; set; }

        //2000*******  ELIMINATO IL CAMPO SOSTITUITO CON DFDETR
        /// <summary>
        /// DEC_CAL_ARR_SEDE 9(6)  
        /// </summary>
        [HisFieldInfoMapping(103, 6)]
        public int DEC_CAL_ARR_SEDE { get; set; }

        //************   AAAAMM
        /// <summary>
        /// DATA_CAL_ARR_AA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(104, 4)]
        public short DATA_CAL_ARR_AA { get; set; }

        /// <summary>
        /// DATA_CAL_ARR_MM 99  
        /// </summary>
        [HisFieldInfoMapping(105, 2)]
        public short DATA_CAL_ARR_MM { get; set; }

        /// <summary>
        /// TIPO_PEREQUAZIONE X  
        /// </summary>
        [HisFieldInfoMapping(106, 1)]
        public string TIPO_PEREQUAZIONE { get; set; }

        // * CODICE SENTENXA
        /// <summary>
        /// DF9712 99  
        /// </summary>
        [HisFieldInfoMapping(107, 2)]
        public short DF9712 { get; set; }

        /// <summary>
        /// FILLER X(2)  
        /// </summary>
        [HisFieldInfoMapping(108, 2)]
        public string FILLER5 { get; set; }

        //*******************************************************
        // *   FINE TERZO SETTORE DEL PRIMO RECORD               *
        //*******************************************************

        [HisComplexAreaInfoMapping(109, ListCount = 16)]
        public List<Familiare> FAMILIARI { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class Familiare
        {
            #region Constructor
            internal Familiare()
            { }
            #endregion Constructor
            #region tracciato COBOL
            //        2000       02  DF70 OCCURS 16.                                         
            //               03  DF701-A 				  PIC X(30).                  
            //      * SESSO                                                           
            //               03  DF702-A                PIC X.                        
            //      * SIGLA                                                           
            //               03  DF703-A                PIC X.                        
            //      * DATA NASCITA  GGMMAAAA                                         
            //2000               04  DF704-G            PIC 99.                       
            //2000               04  DF704-M            PIC 99.                       
            //2000               04  DF704-AA           PIC 9999.                     
            //      * AGGIUNTA DI FAMIGLIA                                            
            //2000           03  DF705-A                PIC S9(5)V9999 COMP-3.        
            //      * IMPORTO NON CUMULABILE                                          
            //2000           03  DF708-A                PIC S9(5)V9999 COMP-3.        
            //      * DATA CESSAZIONE           AAAAMM                               
            //2000               04  DF706-AA           PIC 9(4).                     
            //2000               04  DF706-M            PIC 99.                       
            //      * DATA ACQUISIZIONE         AAAAMM                              
            //2000               04  DF707-AA           PIC 9(4).                     
            //2000               04  DF707-M            PIC 99.                       
            //2000           03  FILLER                 PIC X.   
            #endregion tracciato COBOL

            #region Tracciato Host
            // 2000       02  DF70 OCCURS 16.
            /// <summary>
            /// DF701_A X(30)  
            /// </summary>
            [HisFieldInfoMapping(0, 30)]
            public string DF701_A { get; set; }

            // * SESSO
            /// <summary>
            /// DF702_A X  
            /// </summary>
            [HisFieldInfoMapping(1, 1)]
            public string DF702_A { get; set; }

            // * SIGLA
            /// <summary>
            /// DF703_A X  
            /// </summary>
            [HisFieldInfoMapping(2, 1)]
            public string DF703_A { get; set; }

            // * DATA NASCITA  GGMMAAAA
            /// <summary>
            /// DF704_G 99  
            /// </summary>
            [HisFieldInfoMapping(3, 2)]
            public short DF704_G { get; set; }

            /// <summary>
            /// DF704_M 99  
            /// </summary>
            [HisFieldInfoMapping(4, 2)]
            public short DF704_M { get; set; }

            /// <summary>
            /// DF704_AA 9(4)  
            /// </summary>
            [HisFieldInfoMapping(5, 4)]
            public short DF704_AA { get; set; }

            // * AGGIUNTA DI FAMIGLIA
            /// <summary>
            /// DF705_A S9(5)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(6, 5, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal DF705_A { get; set; }

            // * IMPORTO NON CUMULABILE
            /// <summary>
            /// DF708_A S9(5)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(7, 5, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal DF708_A { get; set; }

            // * DATA CESSAZIONE           AAAAMM
            /// <summary>
            /// DF706_AA 9(4)  
            /// </summary>
            [HisFieldInfoMapping(8, 4)]
            public short DF706_AA { get; set; }

            /// <summary>
            /// DF706_M 99  
            /// </summary>
            [HisFieldInfoMapping(9, 2)]
            public short DF706_M { get; set; }

            // * DATA ACQUISIZIONE         AAAAMM
            /// <summary>
            /// DF707_AA 9(4)  
            /// </summary>
            [HisFieldInfoMapping(10, 4)]
            public short DF707_AA { get; set; }

            /// <summary>
            /// DF707_M 99  
            /// </summary>
            [HisFieldInfoMapping(11, 2)]
            public short DF707_M { get; set; }

            /// <summary>
            /// FILLER X  
            /// </summary>
            [HisFieldInfoMapping(12, 1)]
            public string FILLER { get; set; }
            #endregion Tracciato Host

        }
        #endregion nested class
    }
}
