using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class CI01Record_RA
    {
        #region Constructor
        internal CI01Record_RA()
        {
        }
        #endregion Constructor

        #region tracciato COBOL
        //               01  REC-RISPOSTA.                                                
        //           02  REC-RA.                                                  
        //               03  RECSET1A.                                            
        //**********                                        PER COSTANTINO        
        //                   04  DF01-A               PIC X(4).                   
        //**********                                        CATEG IN CHIARO       
        //                   04  DF02-A.                                          
        //                       05  CATFS      PIC XXX.                          
        //                       05  RESTOCAT   PIC X(5).                         
        //**********                                        NUM. CERTIFICATO      
        //                   04  DF03-A        PIC 9(8).                          
        //**********                                        COGN/NOME             
        //                   04  DF04-A        PIC X(32).                         
        //**********                                        SESSO                 
        //                   04  DF05-A        PIC X.                             
        //**********                                        INDIRIZZO             
        //                   04  DF06-IND.
        //                       05  DF06-IND1     PIC X(52).                     
        //                       05  DF06-IND2     PIC X(52).                     
        //                       05  DF06-IND3     PIC X(52).                     
        //                   04  DF06-CIV       PIC X(18).                        
        //                   04  DF06-FRAZ      PIC X(35).                        
        //                   04  DF06-IND4     PIC X(52).                         
        //**********                                        CAP                   
        //                   04  DF07-A        PIC X(9).                          
        //**********                                        COMUNE                
        //                   04  DF081-A       PIC X(37).                         
        //**********                                        PROVINCIA             
        //                   04  DF082-A       PIC XXX.                           
        //**********                                    DATA NAS. 0GGMMAAA (PD)   
        //                   04  DF09-A        PIC X(4).                          
        //**********                                    DEC.PENS. GGMMAA   (PD)   
        //                   04  DF10-A.                                          
        //                       05  DF101-A   PIC X.                             
        //                       05  DF102-A   PIC X.                             
        //                       05  DF103-A   PIC X.                             
        //***********                                DATA FINALE CALC.ARR. (PD)   
        //                   04  DF11-A.                                          
        //                       05  DF111-A   PIC X.                             
        //                       05  DF112-A   PIC X.                             
        //                       05  DF113-A   PIC X.                             
        //***********                                       COD.PATRONATO         
        //                   04  DF121-A       PIC 99.                            
        //***********                                       ZONA PATRONATO        
        //                   04  DF122-A       PIC X.                             
        //***********                                       NOME PATRONATO        
        //                   04  DF123-A       PIC X(7).                          
        //***********                              NUM PATRONATO NON USATO        
        //                   04  DF124-A       PIC X(4).                          
        //***********                                     DEC.INTER. LEGALI (PD)  
        //                   04  DF13-A.                                          
        //                       05  DF131-A   PIC X.                             
        //                       05  DF132-A   PIC X.                             
        //                       05  DF133-A   PIC X.                             
        //***********                                       NATURA PENSIONE       
        //                   04  DF15-A.                                          
        //                       05  DF15-A1       PIC XX.                        
        //                       05  DF15-A2       PIC X.                         
        //***********                                     ARR.AL LORDO TRATT      
        //                   04  DF16-A        PIC S9(7)V9(4) COMP-3.             
        //***********                                       ONPI SU ARRETR        
        //                   04  DF17-A        PIC S9(5)V9(4) COMP-3.             
        //***********                                       ERAR SU ARRETR        
        //                   04  DF18-A        PIC S9(7)V9(4) COMP-3.             
        //***********                                       SIND.SU ARRETR        
        //                   04  DF19-A        PIC S9(7)V9(4) COMP-3.             
        //***********                                       MESI DI PAGAMENTO     
        //                   04  DF26-A        PIC 9.                             
        //***********                                       N.CEDOLE DA STAMPARE  
        //                   04  DF27-A        PIC 9.                             
        //***********                                       UFF.PAGATORE          
        //                   04  DF28-A.                                          
        //                       05 DF281-A        PIC X.
        //                       05 DF282-A        PIC X.
        //                       05 DF283-A        PIC X.
        //***********                                                             
        //                   04  DF29-A        PIC X.                             
        //***********                                       DELEGATO O TUTORE     
        //                   04  DF30-A.                                          
        //***********                                       D/T                   
        //                       05  DF301-A       PIC X.                         
        //***********                                          NOME               
        //                       05  DF302-A       PIC X(72).                     
        //***********                                       COMP FAMILIARE        
        //                   04  DF32-A.                                          
        //***********                                       ASCENDENTI            
        //                       05  DF321     PIC 9.                             
        //***********                                       CONIUGE               
        //                       05  DF322     PIC 9.                             
        //***********                                       FIGLI                 
        //                       05  DF323     PIC 99.                            
        //***********                                       CODICE SINDACATO      
        //                   04  DF34X-A       PIC X.                             
        //***********                                       NUM.FAMILIARI         
        //                   04  DF35-A        PIC 99.                            
        //***********                                       COD.PAG.ARRETRATI     
        //                   04  DF36-A        PIC 9.                             
        //***********                              DATA EMISSIONE MMAA (PD)       
        //                   04  DF37-A        PIC XX.                            
        //***********                                    ULTIMO MENS. LORDO       
        //                   04  DF40-A        PIC S9(7)V9(4) COMP-3.             
        //***********                                    13 LORDA                 
        //                   04  DF41-A        PIC S9(7)V9(4) COMP-3.             
        //***********                                    ULTIMA TRAT.ERAR.SU MENS 
        //                   04  DF42-A        PIC S9(7)V9(4) COMP-3.             
        //***********                                    ULTIMA TRAT.SIND.MENS    
        //                   04  DF43-A        PIC S9(7)V9(4) COMP-3.             
        //***********                                    FLAG DIFFERIMENTO        
        //                   04  DF471-A       PIC 9.                             
        //***********                                    FLAG SCISSE              
        //                   04  DF473-A       PIC 9.                             
        //***********                                    FLAG OPZIONE             
        //                   04  DF474-A       PIC 9.                             
        //***********                                    CAUSA CARICO             
        //                   04  DF46-A        PIC 9.                             
        //***********                                    FLAG STAMPA INT.LEGALI   
        //                   04  DF476-A       PIC 9.                             
        //***********                                    FLAG RISPOSTA PROCEDURA  
        //***********                                    BANCHE MECCANIZZATE      
        //                   04  DF6009-A      PIC 9.                             
        //               03  RECSET2A.                                            
        //***********                                     1-CD  2-CM  3-MR        
        //                  04  DF53-A         PIC 9.   


        //                  04  DF76AC-A     OCCURS 2.                            
        //      **********                                 TOT IVS                
        //                      05  DF76       PIC S9(7)V9(4) COMP-3.   



        //               03  RECSET3A.                                            
        //                   04  FILLER         PIC X.                            
        //************                                 TIPO      1=PV 2=PS 3=FS   
        //                   04  DF13TIPO       PIC 9.                            
        //************                                   SIGLA NAZIONE            
        //                   04  DF13NAZ        PIC XXX.                          
        //************                                   CODICE RICORSO           
        //                   04  DF91           PIC X.                            
        //************                                   NUM.CENTR OPERATIVO      
        //                   04  DF13OPE-A      PIC X.                            
        //************                                 SE SPETTANO LE 2'DETR.     
        //                   04  DF57TER-A      PIC X.                            
        //************                                 ALTRI 2 COD DETRAZ.IMP     
        //                   04  DF57BIS-A      PIC X.                            
        //                   04  DF73-A         PIC XXX.                          
        //************                                     U.P. IN CHIARO         
        //                   04  DF56-A         PIC X(24).                        
        //************                                     NUOVI COD.DET.IMPOS    
        //                   04  ZDF57-A         PIC X(5).                        
        //*************                                    TRATT. ERARIALI SU TRED
        //                   04  DF72BIS-A             PIC S9(7)V9(4) COMP-3.     
        //*************                                                           
        //                   04  DF62B-A.                                         
        //*************                                    CODICE FISCALE         
        //                       05  DF62-A     PIC X(16).                        
        //*************                                    RESIDUO DEBITO 1       
        //                       05  DF64-A       PIC S9(7)V9(4) COMP-3.          
        //*************                                    IMPOSTA NETTA A.P.     
        //                       05  DF69-A     PIC S9(7)V9(4) COMP-3.            
        //*************                                    IMPOSTA NETTA A.C.     
        //                   04  DF71-A            PIC S9(7)V9(4) COMP-3.         
        //*************                               TIPO TE10 DA STAMPARE       
        //                   04  DF10TE-A       PIC 99.                           
        //*************                               LIBERI                      
        //                   04  DF72-A         PIC X.                            
        //*************                            ADDIZIONALE IRPEF              
        //                   04  DF60-A         PIC S9(9)V9(4) COMP-3.            
        //*************                            DEC CALC ARRETR AAMM  (PD)     
        //                   04  DF61-A         PIC XX.                           
        //*************                                     LIBERI                
        //                   04  DF61X-A        PIC XX.                           
        //*************                            GP1AXA4Z     AAMM    (PD)      
        //                   04  DF62X-A.                                         
        //                       05 DF62X1-A      PIC X.
        //                       05 DF62X2-A      PIC X.



        //               03  RECSET4-6A.                                          
        //************                                       FAMILIARI            
        //                   04  DF70-A     OCCURS 18.                            
        //************                                       NOME                 
        //                       05  DF701-A    PIC X(26).                        
        //************                                       SESSO                
        //                       05  DF702-A    PIC X.                            
        //************                                       SIGLA                
        //                       05  DF703-A    PIC X.                            
        //************                                 DATA NASCITA GGMMAA (PD)   
        //                       05  DF704-A    PIC XXX.                          
        //************                                       AGGIUNTA FAMIGL      
        //                       05  DF705-A    PIC S9(7)V9(4) COMP-3.            
        //************                                       LIBERO               
        //                       05  DF708-A    PIC XX.                           
        //************                                       SCADENZA  AAMM (PD)  
        //                       05  DF706-A.                                     
        //                           06  DF7061-A    PIC X.                       
        //                           06  DF7062-A    PIC X.                       
        //************                                     DECORRENZA  AAMM (PD)  
        //                       05  DF707-A.                                     
        //                           06  DF7071-A    PIC X.                       
        //                           06  DF7072-A    PIC X.         



        //               03  RECSET3C.                                            
        //**********                     GGMMAAAA    DATA NASC DELEGATO  (PD)     
        //                   04  DF9301-C      PIC X(4).                          
        //**********                                      COD FISCALE DELEG       
        //                   04  DF9302-C      PIC X(16).                         
        //**********                                    TRATT.LAVORO GIORNAL.
        //                   04  DF44-A        PIC S9(7)V9(4).
        //**********                                    TRATT.LAVORO 13A
        //                   04  DF45-A        PIC S9(7)V9(4).
        //**********                                     COD.DET.IMPOS (PD)
        //                   04  DF57-A         PIC X(5).
        //**********                                     COD.SINDACATO  2002
        //                   04  SINDACATO      PIC XX.
        //**********                                     ABI + CAB
        //                   04  ABI            PIC 9(5).
        //                   04  CAB            PIC 9(7).
        //**********************  DATI PATRONATO:  24 BYTE
        //      *GP1RICPTUFF TIPO UFFICIO
        //                   04 PATUFF          PIC 9(3).
        //      *GP1RICPCOD CODICE ENTE DI PATRONATO
        //                   04 PATCOD          PIC 9(3).
        //      *GP1RICPZON CODICE UFF ZONALE ENTE DI PATRONATO
        //                   04 PATZON          PIC X(10).
        //      *GP1RICPZON CODICE UFF ZONALE ENTE DI PATRONATO
        //                   04 PATNUM          PIC 9(8).
        //**********                                A DISPOSIZIONE
        //****************** 04  FILLER         PIC X(27).
        //                   04  FILLER         PIC X(3).
        //**********                                      STATO CIVILE
        //                   04  DF9307-C      PIC 9.
        //**********                                  INVALIDO/SORDOMUTO          
        //                   04  DF9308-C      PIC 9.                             
        //**********                                     LIBERO                   
        //                   04  DF9309-C      PIC X.                             
        //**********                                  X PS                        
        //                   04  DF9310-C      PIC X.                             
        //**********                                  "                           
        //                   04  DF9311-C      PIC X.                             
        //**********                                  "                           
        //                   04  DF9312-C      PIC X.                             
        //**********                                  "                           
        //                   04  DF9313-C      PIC X.                             
        //**********                               DATA IN.CALC.ARR. MMAA(PD)     
        //                   04  DF9314-C.                                        
        //                       05  DF9314M-C PIC X.                             
        //                       05  DF9314A-C PIC X.                             
        //**********                                     COMUNE NASC DELEG(PD)    
        //                   04  DF9306A-C     PIC XXX.                           
        //**********                                     PROV NASC.DELEG (PD)     
        //                   04  DF9306B-C     PIC X.                             
        //**********                                     DEC.EX-COMB  MMAA (PD)   
        //                   04  DF9314C-C     PIC XX.                            
        //**********                                     CODICE TIPO PENS         
        //      *                                        ALLA 1' DEC ANNO CORSO   
        //      *                                   01:  > MINIMO SENZA Q.F.      
        //      *                                   02:  INTEGRATA                
        //      *                                   03:  < MINIMO CON PARZ.INTEG. 
        //      *                                   04:  > MINIMO CON  Q.F.       
        //      *                                   05:  PENSIONE AL CALCOLO PURO 
        //      *                                   06:  CRISTALLIZZATA 638       
        //      *                                   08:  CRISTALLIZZATA -52 CTR   
        //      *                                   09:  CRISTALLIZZATA '1'       
        //                   04  DF9315-C      PIC 99.                            
        //**********                                     ART 1/2 L 140            
        //      *                                        ULTIMA  DEC ANNO CORSO   
        //                   04  DF9316-C        PIC S9(7)V9(4) COMP-3.           
        //**********                                     COD 781                  
        //                   04  DF9319-C      PIC 9.                             
        //**********                              DATA INIZIO ASS. GGMMAA (PD)    
        //                   04  DF9320-C      PIC XXXX.                          
        //**********                                     DATA FINE ASS. GGMMAA    
        //                   04  DF9321-C      PIC XXXX.                          
        //**********                                     COD L.140     (3-4-5)    
        //                   04  DF9322-C      PIC 9.                             
        //**********                                     CTR ANZ. X CALCOLO       
        //                   04  DF9324-C      PIC 9(4).                          
        //**********                                     R.M.S.                   
        //                   04  DF9325-C      PIC S9(7)V9(4) COMP-3.             
        //**********                                     CTR PER DIRITTO          
        //                   04  DF9326-C      PIC S9(4).                         
        //**********                                     VV PER DIRITTO           
        //                   04  DF9327-C      PIC S9(4).                         
        //**********                                     VV PER ANZIANITA         
        //                   04  DF9328-C      PIC S9(4).                         
        //**********                                     DEC ULT.SUPPL (MMAA PD)  
        //                   04  DF9329-C      PIC XX.         


        //**********                        CODICE DIRITTO A.F (X SO)   
        //                   04  DF9330X-C.                                       
        //                       05  DF9330-C PIC X OCCURS 15.                    
        //**********                                   SIGLA INTEST PENS SO       


        //                   04  DF9331-C      PIC X.                             
        //**********                                       RRN BASE INFORM. (PD)  
        //                   04  DF9337-C      PIC X(4).                          
        //**********                                    CODICE PROVVISORIA        
        //                   04  DF9338-C      PIC X.                             
        //**********                                   SEDE PROVENIENZA           
        //                   04  DF9339-C      PIC XX.                            
        //**********                                   RESI.ESTERO = 1            
        //                   04  DF9340-C      PIC X.      



        //**********                       FLAGS X 201                            
        //                   04  DF9341-C.       


        //                       05  DF9341A-C.                                   
        //                           06  DF9341A1-C   PIC X  OCCURS 10.       


        //                       05  DF9341B-C.                                   
        //                           06  DF9341B1-C   PIC XX.                     
        //                           06  DF9341B2-C   PIC XX.                     
        //                       05  DF9341-C         PIC X(6).                   
        //**********                                  COD AUMENTO L.59            
        //                   04  DF9342-C      PIC X.                             
        //                   04  TIPO-PEREQUAZIONE  PIC X.                        
        //*************  CAMPI SPOSTATI   ****************                        
        //*********                                      CATEGORIA = GP1AB01      
        //                   04  DF9638-C        PIC 999.                         
        //*********                                      ASS.DI ACCOM KM21        
        //                   04  DF96542-C       PIC S9(7)V9(4) COMP-3.           
        //*********                                      FLAG ELIMINATA           
        //                   04  DF9639          PIC 9.                           
        //*********                                      N. RICONOSC ASS INV      
        //                   04  DF9636-C        PIC 9.                           
        //*********                                      DEC DANTE CAUSA          
        //                   04  DF9635-C        PIC XX.                          
        //*********                                DEC.ART.1/2 L.140 MMAA (PD)    
        //                   04  DF9402-C      PIC XX.                            
        //*********                                DEC.ART.1/2 L.544 MMAA (PD)    
        //                   04  DF9403-C      PIC XX.                            
        //*********                                DEC CONTABILE OBIS AAMM (PD)   
        //                   04  DF9404-C      PIC XX.                            
        //*********                                      GP1AJ03                  
        //                   04  DF9631-C      PIC X.                             
        //                   04  DF9632-C      PIC 9(7)V9(4).                     
        //****************  DECORRENZA ASSEGNO DI ACCOMPAGNO AAAAMM               
        //                   04  DEC-ACCO      PIC 9(6).                          
        //                   04  FILLER        PIC X(6).                          
        //               03  REC-SET7-C.                                          
        //      **********                           LITER SU TE08                
        //                   04  DF9712-C              PIC X.                     
        //      **********                           VUOTI LIBERI                 
        //                   04  DF97XV-C              PIC X(3).                  
        //      **********                      DATA INIZIO ASSICURAZ  (PD)       
        //                 04  DF9705-C                PIC X(4).                  
        //      **********                      DATA FINE ASSICURAZ. (PD)         
        //                 04  DF9706-C                PIC X(4).                  
        //      **                      NUOVA TABELLA CALCOLO CONTRIBUTIVO  ***** 
        //      **********                                                        
        //      **                      DATI PER OBIS 95                    ***** 
        //      **********                           PERIODICITA PAGAMENTO        
        //                 04  DF9707-C                PIC X.                     
        //      **********                        = 3 SPORTEL                     
        //                 04  DF9711-C                PIC X. 
        #endregion tracciato COBOL

        #region Tracciato Host
        // 01  REC-RISPOSTA.
        // 02  REC-RA.
        // 03  RECSET1A.
        //**********                                        PER COSTANTINO
        /// <summary>
        /// DF01_A X(4)  
        /// </summary>
        [HisFieldInfoMapping(0, 4)]
        public string DF01_A { get; set; }

        //**********                                        CATEG IN CHIARO
        // 04  DF02-A.
        /// <summary>
        /// CATFS XXX  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string CATFS { get; set; }

        /// <summary>
        /// RESTOCAT X(5)  
        /// </summary>
        [HisFieldInfoMapping(2, 5)]
        public string RESTOCAT { get; set; }

        //**********                                        NUM. CERTIFICATO
        /// <summary>
        /// DF03_A 9(8)  
        /// </summary>
        [HisFieldInfoMapping(3, 8)]
        public int DF03_A { get; set; }

        //**********                                        COGN/NOME
        /// <summary>
        /// DF04_A X(32)  
        /// </summary>
        [HisFieldInfoMapping(4, 32)]
        public string DF04_A { get; set; }

        //**********                                        SESSO
        /// <summary>
        /// DF05_A X  
        /// </summary>
        [HisFieldInfoMapping(5, 1)]
        public string DF05_A { get; set; }

        //**********                                        INDIRIZZO
        // 04  DF06-IND.
        /// <summary>
        /// DF06_IND1 X(52)  
        /// </summary>
        [HisFieldInfoMapping(6, 52)]
        public string DF06_IND1 { get; set; }

        /// <summary>
        /// DF06_IND2 X(52)  
        /// </summary>
        [HisFieldInfoMapping(7, 52)]
        public string DF06_IND2 { get; set; }

        /// <summary>
        /// DF06_IND3 X(52)  
        /// </summary>
        [HisFieldInfoMapping(8, 52)]
        public string DF06_IND3 { get; set; }

        /// <summary>
        /// DF06_CIV X(18)  
        /// </summary>
        [HisFieldInfoMapping(9, 18)]
        public string DF06_CIV { get; set; }

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

        //**********                                        CAP
        /// <summary>
        /// DF07_A X(9)  
        /// </summary>
        [HisFieldInfoMapping(12, 9)]
        public string DF07_A { get; set; }

        //**********                                        COMUNE
        /// <summary>
        /// DF081_A X(37)  
        /// </summary>
        [HisFieldInfoMapping(13, 37)]
        public string DF081_A { get; set; }

        //**********                                        PROVINCIA
        /// <summary>
        /// DF082_A XXX  
        /// </summary>
        [HisFieldInfoMapping(14, 3)]
        public string DF082_A { get; set; }

        //**********                                    DATA NAS. 0GGMMAAA (PD)
        /// <summary>
        /// DF09_A X(4)  
        /// </summary>
        [HisFieldInfoMapping(15, 4, CobolType = CobolType.Untraslate)]
        public int DF09_A { get; set; }

        //**********                                    DEC.PENS. GGMMAA   (PD)
        // 04  DF10-A.
        /// <summary>
        /// DF101_A X  
        /// </summary>
        [HisFieldInfoMapping(16, 1, CobolType = CobolType.Untraslate)]
        public short DF101_A { get; set; }

        /// <summary>
        /// DF102_A X  
        /// </summary>
        [HisFieldInfoMapping(17, 1, CobolType = CobolType.Untraslate)]
        public short DF102_A { get; set; }

        /// <summary>
        /// DF103_A X  
        /// </summary>
        [HisFieldInfoMapping(18, 1, CobolType = CobolType.Untraslate)]
        public short DF103_A { get; set; }

        //***********                                DATA FINALE CALC.ARR. (PD)
        // 04  DF11-A.
        /// <summary>
        /// DF111_A X  
        /// </summary>
        [HisFieldInfoMapping(19, 1, CobolType =CobolType.Untraslate)]
        public short DF111_A { get; set; }

        /// <summary>
        /// DF112_A X  
        /// </summary>
        [HisFieldInfoMapping(20, 1, CobolType = CobolType.Untraslate)]
        public short DF112_A { get; set; }

        /// <summary>
        /// DF113_A X  
        /// </summary>
        [HisFieldInfoMapping(21, 1, CobolType = CobolType.Untraslate)]
        public short DF113_A { get; set; }

        //***********                                       COD.PATRONATO
        /// <summary>
        /// DF121_A 99  
        /// </summary>
        [HisFieldInfoMapping(22, 2)]
        public short DF121_A { get; set; }

        //***********                                       ZONA PATRONATO
        /// <summary>
        /// DF122_A X  
        /// </summary>
        [HisFieldInfoMapping(23, 1)]
        public string DF122_A { get; set; }

        //***********                                       NOME PATRONATO
        /// <summary>
        /// DF123_A X(7)  
        /// </summary>
        [HisFieldInfoMapping(24, 7)]
        public string DF123_A { get; set; }

        //***********                              NUM PATRONATO NON USATO
        /// <summary>
        /// DF124_A X(4)  
        /// </summary>
        [HisFieldInfoMapping(25, 4)]
        public string DF124_A { get; set; }

        //***********                                     DEC.INTER. LEGALI (PD)
        // 04  DF13-A.
        /// <summary>
        /// DF131_A X  
        /// </summary>
        [HisFieldInfoMapping(26, 1, CobolType = CobolType.Untraslate)]
        public short DF131_A { get; set; }

        /// <summary>
        /// DF132_A X  
        /// </summary>
        [HisFieldInfoMapping(27, 1, CobolType = CobolType.Untraslate)]
        public short DF132_A { get; set; }

        /// <summary>
        /// DF133_A X  
        /// </summary>
        [HisFieldInfoMapping(28, 1, CobolType = CobolType.Untraslate)]
        public short DF133_A { get; set; }

        //***********                                       NATURA PENSIONE
        // 04  DF15-A.
        /// <summary>
        /// DF15_A1 XX  
        /// </summary>
        [HisFieldInfoMapping(29, 2)]
        public string DF15_A1 { get; set; }

        /// <summary>
        /// DF15_A2 X  
        /// </summary>
        [HisFieldInfoMapping(30, 1)]
        public string DF15_A2 { get; set; }

        //***********                                     ARR.AL LORDO TRATT
        /// <summary>
        /// DF16_A S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(31, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF16_A { get; set; }

        //***********                                       ONPI SU ARRETR
        /// <summary>
        /// DF17_A S9(5)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(32, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF17_A { get; set; }

        //***********                                       ERAR SU ARRETR
        /// <summary>
        /// DF18_A S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(33, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF18_A { get; set; }

        //***********                                       SIND.SU ARRETR
        /// <summary>
        /// DF19_A S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(34, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF19_A { get; set; }

        //***********                                       MESI DI PAGAMENTO
        /// <summary>
        /// DF26_A 9  
        /// </summary>
        [HisFieldInfoMapping(35, 1)]
        public short DF26_A { get; set; }

        //***********                                       N.CEDOLE DA STAMPARE
        /// <summary>
        /// DF27_A 9  
        /// </summary>
        [HisFieldInfoMapping(36, 1)]
        public short DF27_A { get; set; }

        //***********                                       UFF.PAGATORE
        // 04  DF28-A.
        /// <summary>
        /// DF281_A X  
        /// </summary>
        [HisFieldInfoMapping(37, 1)]
        public string DF281_A { get; set; }

        /// <summary>
        /// DF282_A X  
        /// </summary>
        [HisFieldInfoMapping(38, 1)]
        public string DF282_A { get; set; }

        /// <summary>
        /// DF283_A X  
        /// </summary>
        [HisFieldInfoMapping(39, 1)]
        public string DF283_A { get; set; }

        //***********
        /// <summary>
        /// DF29_A X  
        /// </summary>
        [HisFieldInfoMapping(40, 1)]
        public string DF29_A { get; set; }

        //***********                                       DELEGATO O TUTORE
        // 04  DF30-A.
        //***********                                       D/T
        /// <summary>
        /// DF301_A X  
        /// </summary>
        [HisFieldInfoMapping(41, 1)]
        public string DF301_A { get; set; }

        //***********                                          NOME
        /// <summary>
        /// DF302_A X(72)  
        /// </summary>
        [HisFieldInfoMapping(42, 72)]
        public string DF302_A { get; set; }

        //***********                                       COMP FAMILIARE
        // 04  DF32-A.
        //***********                                       ASCENDENTI
        /// <summary>
        /// DF321 9  
        /// </summary>
        [HisFieldInfoMapping(43, 1)]
        public short DF321 { get; set; }

        //***********                                       CONIUGE
        /// <summary>
        /// DF322 9  
        /// </summary>
        [HisFieldInfoMapping(44, 1)]
        public short DF322 { get; set; }

        //***********                                       FIGLI
        /// <summary>
        /// DF323 99  
        /// </summary>
        [HisFieldInfoMapping(45, 2)]
        public short DF323 { get; set; }

        //***********                                       CODICE SINDACATO
        /// <summary>
        /// DF34X_A X  
        /// </summary>
        [HisFieldInfoMapping(46, 1)]
        public string DF34X_A { get; set; }

        //***********                                       NUM.FAMILIARI
        /// <summary>
        /// DF35_A 99  
        /// </summary>
        [HisFieldInfoMapping(47, 2)]
        public short DF35_A { get; set; }

        //***********                                       COD.PAG.ARRETRATI
        /// <summary>
        /// DF36_A 9  
        /// </summary>
        [HisFieldInfoMapping(48, 1)]
        public short DF36_A { get; set; }

        //***********                              DATA EMISSIONE MMAA (PD)
        /// <summary>
        /// DF37_A XX  
        /// </summary>
        [HisFieldInfoMapping(49, 2, CobolType = CobolType.Untraslate)]
        public short DF37_A { get; set; }

        //***********                                    ULTIMO MENS. LORDO
        /// <summary>
        /// DF40_A S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(50, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF40_A { get; set; }

        //***********                                    13 LORDA
        /// <summary>
        /// DF41_A S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(51, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF41_A { get; set; }

        //***********                                    ULTIMA TRAT.ERAR.SU MENS
        /// <summary>
        /// DF42_A S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(52, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF42_A { get; set; }

        //***********                                    ULTIMA TRAT.SIND.MENS
        /// <summary>
        /// DF43_A S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(53, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF43_A { get; set; }

        //***********                                    FLAG DIFFERIMENTO
        /// <summary>
        /// DF471_A 9  
        /// </summary>
        [HisFieldInfoMapping(54, 1)]
        public short DF471_A { get; set; }

        //***********                                    FLAG SCISSE
        /// <summary>
        /// DF473_A 9  
        /// </summary>
        [HisFieldInfoMapping(55, 1)]
        public short DF473_A { get; set; }

        //***********                                    FLAG OPZIONE
        /// <summary>
        /// DF474_A 9  
        /// </summary>
        [HisFieldInfoMapping(56, 1)]
        public short DF474_A { get; set; }

        //***********                                    CAUSA CARICO
        /// <summary>
        /// DF46_A 9  
        /// </summary>
        [HisFieldInfoMapping(57, 1)]
        public short DF46_A { get; set; }

        //***********                                    FLAG STAMPA INT.LEGALI
        /// <summary>
        /// DF476_A 9  
        /// </summary>
        [HisFieldInfoMapping(58, 1)]
        public short DF476_A { get; set; }

        //***********                                    FLAG RISPOSTA PROCEDURA
        //***********                                    BANCHE MECCANIZZATE
        /// <summary>
        /// DF6009_A 9  
        /// </summary>
        [HisFieldInfoMapping(59, 1)]
        public short DF6009_A { get; set; }

        // 03  RECSET2A.
        //***********                                     1-CD  2-CM  3-MR
        /// <summary>
        /// DF53_A 9  
        /// </summary>
        [HisFieldInfoMapping(60, 1)]
        public short DF53_A { get; set; }

        [HisComplexAreaInfoMapping(61, ListCount = 2)]
        public List<Ivs> LISTAIVS { get; set; }

        // 03  RECSET3A.
        /// <summary>
        /// FILLER X  
        /// </summary>
        [HisFieldInfoMapping(62, 1)]
        public string FILLER { get; set; }

        //************                                 TIPO      1=PV 2=PS 3=FS
        /// <summary>
        /// DF13TIPO 9  
        /// </summary>
        [HisFieldInfoMapping(63, 1)]
        public short DF13TIPO { get; set; }

        //************                                   SIGLA NAZIONE
        /// <summary>
        /// DF13NAZ XXX  
        /// </summary>
        [HisFieldInfoMapping(64, 3)]
        public string DF13NAZ { get; set; }

        //************                                   CODICE RICORSO
        /// <summary>
        /// DF91 X  
        /// </summary>
        [HisFieldInfoMapping(65, 1)]
        public string DF91 { get; set; }

        //************                                   NUM.CENTR OPERATIVO
        /// <summary>
        /// DF13OPE_A X  
        /// </summary>
        [HisFieldInfoMapping(66, 1)]
        public string DF13OPE_A { get; set; }

        //************                                 SE SPETTANO LE 2'DETR.
        /// <summary>
        /// DF57TER_A X  
        /// </summary>
        [HisFieldInfoMapping(67, 1)]
        public string DF57TER_A { get; set; }

        //************                                 ALTRI 2 COD DETRAZ.IMP
        /// <summary>
        /// DF57BIS_A X  
        /// </summary>
        [HisFieldInfoMapping(68, 1)]
        public string DF57BIS_A { get; set; }

        /// <summary>
        /// DF73_A XXX  
        /// </summary>
        [HisFieldInfoMapping(69, 3)]
        public string DF73_A { get; set; }

        //************                                     U.P. IN CHIARO
        /// <summary>
        /// DF56_A X(24)  
        /// </summary>
        [HisFieldInfoMapping(70, 24)]
        public string DF56_A { get; set; }

        //************                                     NUOVI COD.DET.IMPOS
        /// <summary>
        /// ZDF57_A X(5)  
        /// </summary>
        [HisFieldInfoMapping(71, 5)]
        public string ZDF57_A { get; set; }

        //*************                                    TRATT. ERARIALI SU TRED
        /// <summary>
        /// DF72BIS_A S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(72, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF72BIS_A { get; set; }

        //*************
        // 04  DF62B-A.
        //*************                                    CODICE FISCALE
        /// <summary>
        /// DF62_A X(16)  
        /// </summary>
        [HisFieldInfoMapping(73, 16)]
        public string DF62_A { get; set; }

        //*************                                    RESIDUO DEBITO 1
        /// <summary>
        /// DF64_A S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(74, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF64_A { get; set; }

        //*************                                    IMPOSTA NETTA A.P.
        /// <summary>
        /// DF69_A S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(75, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF69_A { get; set; }

        //*************                                    IMPOSTA NETTA A.C.
        /// <summary>
        /// DF71_A S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(76, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF71_A { get; set; }

        //*************                               TIPO TE10 DA STAMPARE
        /// <summary>
        /// DF10TE_A 99  
        /// </summary>
        [HisFieldInfoMapping(77, 2)]
        public short DF10TE_A { get; set; }

        //*************                               LIBERI
        /// <summary>
        /// DF72_A X  
        /// </summary>
        [HisFieldInfoMapping(78, 1)]
        public string DF72_A { get; set; }

        //*************                            ADDIZIONALE IRPEF
        /// <summary>
        /// DF60_A S9(9)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(79, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF60_A { get; set; }

        //*************                            DEC CALC ARRETR AAMM  (PD)
        /// <summary>
        /// DF61_A XX  
        /// </summary>
        [HisFieldInfoMapping(80, 2, CobolType = CobolType.Untraslate)]
        public int DF61_A { get; set; }

        //*************                                     LIBERI
        /// <summary>
        /// DF61X_A XX  
        /// </summary>
        [HisFieldInfoMapping(81, 2, CobolType = CobolType.Untraslate)]
        public short DF61X_A { get; set; }

        //*************                            GP1AXA4Z     AAMM    (PD)
        // 04  DF62X-A.
        /// <summary>
        /// DF62X1_A X  
        /// </summary>
        [HisFieldInfoMapping(82, 1, CobolType = CobolType.Untraslate)]
        public short DF62X1_A { get; set; }

        /// <summary>
        /// DF62X2_A X  
        /// </summary>
        [HisFieldInfoMapping(83, 1)]
        public string DF62X2_A { get; set; }

        [HisComplexAreaInfoMapping(84, ListCount = 18)]
        public List<Familiare> LISTAFAMILIARI { get; set; }

        // 03  RECSET3C.
        //**********                     GGMMAAAA    DATA NASC DELEGATO  (PD)
        /// <summary>
        /// DF9301_C X(4)  
        /// </summary>
        [HisFieldInfoMapping(85, 4, CobolType =CobolType.Untraslate)]
        public int DF9301_C { get; set; }

        //**********                                      COD FISCALE DELEG
        /// <summary>
        /// DF9302_C X(16)  
        /// </summary>
        [HisFieldInfoMapping(86, 16)]
        public string DF9302_C { get; set; }

        //**********                                    TRATT.LAVORO GIORNAL.
        /// <summary>
        /// DF44_A S9(7)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(87, 11, Scale = 4, CobolType = CobolType.Signed)]
        public decimal DF44_A { get; set; }

        //**********                                    TRATT.LAVORO 13A
        /// <summary>
        /// DF45_A S9(7)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(88, 11, Scale = 4, CobolType = CobolType.Signed)]
        public decimal DF45_A { get; set; }

        //**********                                     COD.DET.IMPOS (PD)
        /// <summary>
        /// DF57_A X(5)  
        /// </summary>
        [HisFieldInfoMapping(89, 5, CobolType = CobolType.Untraslate)]
        public long DF57_A { get; set; }

        //**********                                     COD.SINDACATO  2002
        /// <summary>
        /// SINDACATO XX  
        /// </summary>
        [HisFieldInfoMapping(90, 2)]
        public string SINDACATO { get; set; }

        //**********                                     ABI + CAB
        /// <summary>
        /// ABI 9(5)  
        /// </summary>
        [HisFieldInfoMapping(91, 5)]
        public int ABI { get; set; }

        /// <summary>
        /// CAB 9(7)  
        /// </summary>
        [HisFieldInfoMapping(92, 7)]
        public int CAB { get; set; }

        //**********************  DATI PATRONATO:  24 BYTE
        // *GP1RICPTUFF TIPO UFFICIO
        /// <summary>
        /// PATUFF 9(3)  
        /// </summary>
        [HisFieldInfoMapping(93, 3)]
        public short PATUFF { get; set; }

        // *GP1RICPCOD CODICE ENTE DI PATRONATO
        /// <summary>
        /// PATCOD 9(3)  
        /// </summary>
        [HisFieldInfoMapping(94, 3)]
        public short PATCOD { get; set; }

        // *GP1RICPZON CODICE UFF ZONALE ENTE DI PATRONATO
        /// <summary>
        /// PATZON X(10)  
        /// </summary>
        [HisFieldInfoMapping(95, 10)]
        public string PATZON { get; set; }

        // *GP1RICPZON CODICE UFF ZONALE ENTE DI PATRONATO
        /// <summary>
        /// PATNUM 9(8)  
        /// </summary>
        [HisFieldInfoMapping(96, 8)]
        public int PATNUM { get; set; }

        //**********                                A DISPOSIZIONE
        //****************** 04  FILLER         PIC X(27).
        /// <summary>
        /// FILLER X(3)  
        /// </summary>
        [HisFieldInfoMapping(97, 3)]
        public string FILLER1 { get; set; }

        //**********                                      STATO CIVILE
        /// <summary>
        /// DF9307_C 9  
        /// </summary>
        [HisFieldInfoMapping(98, 1)]
        public short DF9307_C { get; set; }

        //**********                                  INVALIDO/SORDOMUTO
        /// <summary>
        /// DF9308_C 9  
        /// </summary>
        [HisFieldInfoMapping(99, 1)]
        public short DF9308_C { get; set; }

        //**********                                     LIBERO
        /// <summary>
        /// DF9309_C X  
        /// </summary>
        [HisFieldInfoMapping(100, 1)]
        public string DF9309_C { get; set; }

        //**********                                  X PS
        /// <summary>
        /// DF9310_C X  
        /// </summary>
        [HisFieldInfoMapping(101, 1)]
        public string DF9310_C { get; set; }

        //**********                                  "
        /// <summary>
        /// DF9311_C X  
        /// </summary>
        [HisFieldInfoMapping(102, 1)]
        public string DF9311_C { get; set; }

        //**********                                  "
        /// <summary>
        /// DF9312_C X  
        /// </summary>
        [HisFieldInfoMapping(103, 1)]
        public string DF9312_C { get; set; }

        //**********                                  "
        /// <summary>
        /// DF9313_C X  
        /// </summary>
        [HisFieldInfoMapping(104, 1)]
        public string DF9313_C { get; set; }

        //**********                               DATA IN.CALC.ARR. MMAA(PD)
        // 04  DF9314-C.
        // 05  DF9314M-C PIC X.
        [HisFieldInfoMapping(105, 1, CobolType = CobolType.Untraslate)]
        public short DF9314M_C { get; set; }
        // 05  DF9314A-C PIC X.
        [HisFieldInfoMapping(106, 1, CobolType = CobolType.Untraslate)]
        public short DF9314A_C { get; set; }
        //**********                                     COMUNE NASC DELEG(PD)
        /// <summary>
        /// DF9306A_C XXX  
        /// </summary>
        [HisFieldInfoMapping(107, 3, CobolType = CobolType.Untraslate)]
        public int DF9306A_C { get; set; }

        //**********                                     PROV NASC.DELEG (PD)
        /// <summary>
        /// DF9306B_C X  
        /// </summary>
        [HisFieldInfoMapping(108, 1, CobolType = CobolType.Untraslate)]
        public short DF9306B_C { get; set; }

        //**********                                     DEC.EX-COMB  MMAA (PD)
        /// <summary>
        /// DF9314C_C XX  
        /// </summary>
        [HisFieldInfoMapping(109, 2, CobolType = CobolType.Untraslate)]
        public int DF9314C_C { get; set; }

        //**********                                     CODICE TIPO PENS
        // *                                        ALLA 1' DEC ANNO CORSO
        // *                                   01:  > MINIMO SENZA Q.F.
        // *                                   02:  INTEGRATA
        // *                                   03:  < MINIMO CON PARZ.INTEG.
        // *                                   04:  > MINIMO CON  Q.F.
        // *                                   05:  PENSIONE AL CALCOLO PURO
        // *                                   06:  CRISTALLIZZATA 638
        // *                                   08:  CRISTALLIZZATA -52 CTR
        // *                                   09:  CRISTALLIZZATA '1'
        /// <summary>
        /// DF9315_C 99  
        /// </summary>
        [HisFieldInfoMapping(110, 2)]
        public short DF9315_C { get; set; }

        //**********                                     ART 1/2 L 140
        // *                                        ULTIMA  DEC ANNO CORSO
        /// <summary>
        /// DF9316_C S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(111, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF9316_C { get; set; }

        //**********                                     COD 781
        /// <summary>
        /// DF9319_C 9  
        /// </summary>
        [HisFieldInfoMapping(112, 1)]
        public short DF9319_C { get; set; }

        //**********                              DATA INIZIO ASS. GGMMAA (PD)
        /// <summary>
        /// DF9320_C XXXX  
        /// </summary>
        [HisFieldInfoMapping(113, 4, CobolType = CobolType.Untraslate)]
        public int DF9320_C { get; set; }

        //**********                                     DATA FINE ASS. GGMMAA
        /// <summary>
        /// DF9321_C XXXX  
        /// </summary>
        [HisFieldInfoMapping(114, 4, CobolType = CobolType.Untraslate)]
        public int DF9321_C { get; set; }

        //**********                                     COD L.140     (3-4-5)
        /// <summary>
        /// DF9322_C 9  
        /// </summary>
        [HisFieldInfoMapping(115, 1)]
        public short DF9322_C { get; set; }

        //**********                                     CTR ANZ. X CALCOLO
        /// <summary>
        /// DF9324_C 9(4)  
        /// </summary>
        [HisFieldInfoMapping(116, 4)]
        public short DF9324_C { get; set; }

        //**********                                     R.M.S.
        /// <summary>
        /// DF9325_C S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(117, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF9325_C { get; set; }

        //**********                                     CTR PER DIRITTO
        /// <summary>
        /// DF9326_C S9(4)  
        /// </summary>
        [HisFieldInfoMapping(118, 4, CobolType = CobolType.Signed)]
        public short DF9326_C { get; set; }

        //**********                                     VV PER DIRITTO
        /// <summary>
        /// DF9327_C S9(4)  
        /// </summary>
        [HisFieldInfoMapping(119, 4, CobolType = CobolType.Signed)]
        public short DF9327_C { get; set; }

        //**********                                     VV PER ANZIANITA
        /// <summary>
        /// DF9328_C S9(4)  
        /// </summary>
        [HisFieldInfoMapping(120, 4, CobolType = CobolType.Signed)]
        public short DF9328_C { get; set; }

        //**********                                     DEC ULT.SUPPL (MMAA PD)
        /// <summary>
        /// DF9329_C XX  
        /// </summary>
        [HisFieldInfoMapping(121, 2, CobolType = CobolType.Untraslate)]
        public int DF9329_C { get; set; }

        [HisComplexAreaInfoMapping(122, ListCount = 15)]
        public List<CodiceDiritto> LISTACODICIDIRITTO { get; set; }

        //**********                                   SIGLA INTEST PENS SO
        /// <summary>
        /// DF9331_C X  
        /// </summary>
        [HisFieldInfoMapping(123, 1)]
        public string DF9331_C { get; set; }

        //**********                                       RRN BASE INFORM. (PD)
        /// <summary>
        /// DF9337_C X(4)  
        /// </summary>
        [HisFieldInfoMapping(124, 4, CobolType =CobolType.Untraslate)]
        public int DF9337_C { get; set; }

        //**********                                    CODICE PROVVISORIA
        /// <summary>
        /// DF9338_C X  
        /// </summary>
        [HisFieldInfoMapping(125, 1)]
        public string DF9338_C { get; set; }

        //**********                                   SEDE PROVENIENZA
        /// <summary>
        /// DF9339_C XX  
        /// </summary>
        [HisFieldInfoMapping(126, 2)]
        public string DF9339_C { get; set; }

        //**********                                   RESI.ESTERO = 1
        /// <summary>
        /// DF9340_C X  
        /// </summary>
        [HisFieldInfoMapping(127, 1)]
        public string DF9340_C { get; set; }

        [HisComplexAreaInfoMapping(128, ListCount = 10)]
        public List<Flag> LISTAFLAG { get; set; }

        // 05  DF9341B-C.
        /// <summary>
        /// DF9341B1_C XX  
        /// </summary>
        [HisFieldInfoMapping(129, 2)]
        public string DF9341B1_C { get; set; }

        /// <summary>
        /// DF9341B2_C XX  
        /// </summary>
        [HisFieldInfoMapping(130, 2)]
        public string DF9341B2_C { get; set; }

        /// <summary>
        /// DF9341_C X(6)  
        /// </summary>
        [HisFieldInfoMapping(131, 6)]
        public string DF9341_C { get; set; }

        //**********                                  COD AUMENTO L.59
        /// <summary>
        /// DF9342_C X  
        /// </summary>
        [HisFieldInfoMapping(132, 1)]
        public string DF9342_C { get; set; }

        /// <summary>
        /// TIPO_PEREQUAZIONE X  
        /// </summary>
        [HisFieldInfoMapping(133, 1)]
        public string TIPO_PEREQUAZIONE { get; set; }

        //*************  CAMPI SPOSTATI   ****************
        //*********                                      CATEGORIA = GP1AB01
        /// <summary>
        /// DF9638_C 999  
        /// </summary>
        [HisFieldInfoMapping(134, 3)]
        public short DF9638_C { get; set; }

        //*********                                      ASS.DI ACCOM KM21
        /// <summary>
        /// DF96542_C S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(135, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DF96542_C { get; set; }

        //*********                                      FLAG ELIMINATA
        /// <summary>
        /// DF9639 9  
        /// </summary>
        [HisFieldInfoMapping(136, 1)]
        public short DF9639 { get; set; }

        //*********                                      N. RICONOSC ASS INV
        /// <summary>
        /// DF9636_C 9  
        /// </summary>
        [HisFieldInfoMapping(137, 1)]
        public short DF9636_C { get; set; }

        //*********                                      DEC DANTE CAUSA
        /// <summary>
        /// DF9635_C XX  
        /// </summary>
        [HisFieldInfoMapping(138, 2)]
        public string DF9635_C { get; set; }

        //*********                                DEC.ART.1/2 L.140 MMAA (PD)
        /// <summary>
        /// DF9402_C XX  
        /// </summary>
        [HisFieldInfoMapping(139, 2, CobolType =CobolType.Untraslate)]
        public int DF9402_C { get; set; }

        //*********                                DEC.ART.1/2 L.544 MMAA (PD)
        /// <summary>
        /// DF9403_C XX  
        /// </summary>
        [HisFieldInfoMapping(140, 2, CobolType = CobolType.Untraslate)]
        public int DF9403_C { get; set; }

        //*********                                DEC CONTABILE OBIS AAMM (PD)
        /// <summary>
        /// DF9404_C XX  
        /// </summary>
        [HisFieldInfoMapping(141, 2, CobolType = CobolType.Untraslate)]
        public int DF9404_C { get; set; }

        //*********                                      GP1AJ03
        /// <summary>
        /// DF9631_C X  
        /// </summary>
        [HisFieldInfoMapping(142, 1)]
        public string DF9631_C { get; set; }

        /// <summary>
        /// DF9632_C 9(7)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(143, 11, Scale = 4)]
        public decimal DF9632_C { get; set; }

        //****************  DECORRENZA ASSEGNO DI ACCOMPAGNO AAAAMM
        /// <summary>
        /// DEC_ACCO 9(6)  
        /// </summary>
        [HisFieldInfoMapping(144, 6)]
        public int DEC_ACCO { get; set; }

        /// <summary>
        /// FILLER X(6)  
        /// </summary>
        [HisFieldInfoMapping(145, 6)]
        public string FILLER2 { get; set; }

        // 03  REC-SET7-C.
        //**********                           LITER SU TE08
        /// <summary>
        /// DF9712_C X  
        /// </summary>
        [HisFieldInfoMapping(146, 1)]
        public string DF9712_C { get; set; }

        //**********                           VUOTI LIBERI
        /// <summary>
        /// DF97XV_C X(3)  
        /// </summary>
        [HisFieldInfoMapping(147, 3)]
        public string DF97XV_C { get; set; }

        //**********                      DATA INIZIO ASSICURAZ  (PD)
        /// <summary>
        /// DF9705_C X(4)  
        /// </summary>
        [HisFieldInfoMapping(148, 4, CobolType = CobolType.Untraslate)]
        public int DF9705_C { get; set; }

        //**********                      DATA FINE ASSICURAZ. (PD)
        /// <summary>
        /// DF9706_C X(4)  
        /// </summary>
        [HisFieldInfoMapping(149, 4, CobolType = CobolType.Untraslate)]
        public int DF9706_C { get; set; }

        //**                      NUOVA TABELLA CALCOLO CONTRIBUTIVO  *****
        //**********
        //**                      DATI PER OBIS 95                    *****
        //**********                           PERIODICITA PAGAMENTO
        /// <summary>
        /// DF9707_C X  
        /// </summary>
        [HisFieldInfoMapping(150, 1)]
        public string DF9707_C { get; set; }

        //**********                        = 3 SPORTEL
        /// <summary>
        /// DF9711_C X  
        /// </summary>
        [HisFieldInfoMapping(151, 1)]
        public string DF9711_C { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class Ivs
        {
            #region Constructor
            internal Ivs()
            { }
            #endregion Constructor

            #region tracciato COBOL
            //                        04  DF76AC-A     OCCURS 2.                            
            //**********                                 TOT IVS                
            //                05  DF76       PIC S9(7)V9(4) COMP-3. 
            #endregion tracciato COBOL

            #region Tracciato Host
            // 04  DF76AC-A     OCCURS 2.
            //**********                                 TOT IVS
            /// <summary>
            /// DF76 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal DF76 { get; set; }
            #endregion Tracciato Host
        }

        public class Familiare
        {
            #region Constructor
            internal Familiare()
            { }
            #endregion Constructor

            #region tracciato COBOL
            //                           03  RECSET4-6A.                                          
            //************                                       FAMILIARI            
            //                   04  DF70-A     OCCURS 18.                            
            //************                                       NOME                 
            //                       05  DF701-A    PIC X(26).                        
            //************                                       SESSO                
            //                       05  DF702-A    PIC X.                            
            //************                                       SIGLA                
            //                       05  DF703-A    PIC X.                            
            //************                                 DATA NASCITA GGMMAA (PD)   
            //                       05  DF704-A    PIC XXX.                          
            //************                                       AGGIUNTA FAMIGL      
            //                       05  DF705-A    PIC S9(7)V9(4) COMP-3.            
            //************                                       LIBERO               
            //                       05  DF708-A    PIC XX.                           
            //************                                       SCADENZA  AAMM (PD)  
            //                       05  DF706-A.                                     
            //                           06  DF7061-A    PIC X.                       
            //                           06  DF7062-A    PIC X.                       
            //************                                     DECORRENZA  AAMM (PD)  
            //                       05  DF707-A.                                     
            //                           06  DF7071-A    PIC X.                       
            //                           06  DF7072-A    PIC X.    
            #endregion tracciato COBOL

            #region Tracciato Host
            // 03  RECSET4-6A.
            //************                                       FAMILIARI
            // 04  DF70-A     OCCURS 18.
            //************                                       NOME
            /// <summary>
            /// DF701_A X(26)  
            /// </summary>
            [HisFieldInfoMapping(0, 26)]
            public string DF701_A { get; set; }

            //************                                       SESSO
            /// <summary>
            /// DF702_A X  
            /// </summary>
            [HisFieldInfoMapping(1, 1)]
            public string DF702_A { get; set; }

            //************                                       SIGLA
            /// <summary>
            /// DF703_A X  
            /// </summary>
            [HisFieldInfoMapping(2, 1)]
            public string DF703_A { get; set; }

            //************                                 DATA NASCITA GGMMAA (PD)
            /// <summary>
            /// DF704_A XXX  
            /// </summary>
            [HisFieldInfoMapping(3, 3, CobolType = CobolType.Untraslate)]
            public int DF704_A { get; set; }

            //************                                       AGGIUNTA FAMIGL
            /// <summary>
            /// DF705_A S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(4, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal DF705_A { get; set; }

            //************                                       LIBERO
            /// <summary>
            /// DF708_A XX  
            /// </summary>
            [HisFieldInfoMapping(5, 2)]
            public string DF708_A { get; set; }

            //************                                       SCADENZA  AAMM (PD)
            // 05  DF706-A.
            /// <summary>
            /// DF7061_A X  
            /// </summary>
            [HisFieldInfoMapping(6, 1, CobolType = CobolType.Untraslate)]
            public short DF7061_A { get; set; }

            /// <summary>
            /// DF7062_A X  
            /// </summary>
            [HisFieldInfoMapping(7, 1, CobolType = CobolType.Untraslate)]
            public short DF7062_A { get; set; }

            //************                                     DECORRENZA  AAMM (PD)
            // 05  DF707-A.
            /// <summary>
            /// DF7071_A X  
            /// </summary>
            [HisFieldInfoMapping(8, 1, CobolType = CobolType.Untraslate)]
            public short DF7071_A { get; set; }

            /// <summary>
            /// DF7072_A X  
            /// </summary>
            [HisFieldInfoMapping(9, 1, CobolType = CobolType.Untraslate)]
            public short DF7072_A { get; set; }
            #endregion Tracciato Host
        }

        public class CodiceDiritto
        {
            #region Constructor
            internal CodiceDiritto()
            {}
            #endregion Constructor

            #region tracciato COBOL
            //**********                        CODICE DIRITTO A.F (X SO)   
            //       04  DF9330X-C.                                       
            //           05  DF9330-C PIC X OCCURS 15.   
            #endregion tracciato COBOL

            #region Tracciato Host
            //**********                        CODICE DIRITTO A.F (X SO)
            // 04  DF9330X-C.
            /// <summary>
            /// DF9330_C X  
            /// </summary>
            [HisFieldInfoMapping(0, 1)]
            public string DF9330_C { get; set; }
            #endregion Tracciato Host
        }

        public class Flag
        {
            #region Constructor
            internal Flag()
            { }
            #endregion Constructor

            #region tracciato COBOL
            //**********                       FLAGS X 201                            
            //       04  DF9341-C.       
            //           05  DF9341A-C.                                   
            //               06  DF9341A1-C   PIC X  OCCURS 10.   
            #endregion tracciato COBOL

            #region Tracciato Host
            //**********                       FLAGS X 201
            // 04  DF9341-C.
            // 05  DF9341A-C.
            /// <summary>
            /// DF9341A1_C X  
            /// </summary>
            [HisFieldInfoMapping(0, 1)]
            public string DF9341A1_C { get; set; }
            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
