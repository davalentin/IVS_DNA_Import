using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class CI01Record_RDA
    {
        #region Constructor
        internal CI01Record_RDA()
        {
            this.INTERESSILEGALI = new InteressiLegali();
        }
        #endregion Constructor

        #region tracciato COBOL
        //         02  REC-RDA.                                                 
        //            03  DFA103  OCCURS 15 TIMES.                                
        //***********    =  ANNO   AAAA                                           
        //                04  DFA1031                     PIC 9(4).               
        //***********    =  IMP.  ANNUO  PENSIONE                                 
        //                04  DFA1032                     PIC S9(9)V9(4) COMP-3.  
        //***********    =  IMP.  ANNUO  TRATT.FAMIGL.                            
        //                04  DFA1033                     PIC S9(7)V9(4) COMP-3.  
        //***********    =  IMP.  ANNUO  MAGG.SOCIALE                             
        //                04  DFA1034                     PIC S9(7)V9(4) COMP-3.  
        //***********    =  IMP.  ANNUO  MAGG.EX COMB.                            
        //                04  DFA1035                     PIC S9(7)V9(4) COMP-3.  
        //***********    =  IMP.  ANNUO  ASSEGNO ACCOMPAGNO                       
        //                04  DFA1036                     PIC S9(7)V9(4) COMP-3.  
        //***********    =  LIBERI                                                
        //                04  FILLER                      PIC X(7).               


        //      *                                                              ** 
        //      *  TABELLA LEGGE 335-ART1-COMMA6                                  
        //      *  CALCOLO DELLA PENSIONE COL SISTEMA CONTRIBUTIVO                
        //      *                                                              ** 
        //            03  DFA601    OCCURS 8.                                     
        //***********    =  DECORR SSAAMMGG                                       
        //                04  DFA6011                  PIC 9(8).                  
        //***********    =  P =  PENSIONE      S = SUPPLEMENTI                    
        //                04  DFA6012                  PIC X.                     
        //***********    =  COD. GESTIONE                                         
        //                04  DFA6013                  PIC 9.                     
        //***********    =  MONTANTE CONTRIBUTIVO                                 
        //                04  DFA6014                  PIC S9(9)V9(4) COMP-3.     
        //***********    =  IMPORTO CONTRIBUTI                                    
        //                04  DFA6015                  PIC S9(7)V9(4) COMP-3.     
        //***********    =  COEFF.                                                
        //                04  DFA6016.                                            
        //                    05  DFA6016A             PIC 9.                     
        //                    05  DFA6016B             PIC 9(4).                  
        //***********    =  NUMERO CONTRIBUTI                                     
        //                04  DFA6017                  PIC 9(4).                  
        //                04  FILLER                   PIC XX.         


        //      ***************************************************************** 
        //      *   PENSIONI ABBINATE                                          ** 
        //      *                                                              ** 
        //            03  DFA702     OCCURS 10.                                   
        //      *   CATEGORIA           (PD)                                      
        //                04  DFA702A                    PIC XX.                  
        //      *   CERTIFICATO         (PD)                                      
        //                04  DFA702B                    PIC X(4).                
        //      *   ABBINATA FISCALMENTE  = 1     (PD)                            
        //                04  DFA702C                    PIC X.                   
        //      *   ABBINATA PAGAMENTO = 1        (PD)                            
        //                04  DFA702D                    PIC X.    



        //          03  INTERESSI-LEGALI.
        //            04 DT-INT-LEG                      PIC X(8).
        //            04 DT-FIN-LEG                      PIC X(8).
        //            04 INT-CRED                        PIC S9(9)V9(4) COMP-3.
        //            04 INT-DEB                         PIC S9(9)V9(4) COMP-3.
        //            04 INT-SALDO                       PIC S9(9)V9(4) COMP-3.
        //            04 RIVAL-CRED                      PIC S9(9)V9(4) COMP-3.
        //            04 RIVAL-DEB                       PIC S9(9)V9(4) COMP-3.
        //            04 RIVAL-SALDO                     PIC S9(9)V9(4) COMP-3.
        //            03  DFA701B                        PIC X(50).               
        //      ***************************************************************** 
        //               03  RECSET1C.                                            
        //***********                                      CHIAVE                 
        //                   04  AGG03-C       PIC X(4).                          
        //***********                                   TABELLA VARIAZIONI TASSE  
        //***********                                     AAMM DEC.PENSIONE       
        //***********                SOLO X REVESR.PENSIONE GP7LC02Z AAMM (PD)    
        //                   04  DF9000-C      PIC XX.                            
        //***********                            SCAD.REV.MEDICA AAMM  (PD)       
        //                   04  DF9017-C      PIC XX.                            
        //***********                                    COD.COMUNE NASCITA (PD)  
        //                   04  DF9018-C      PIC XXX.                           
        //***********                                    TIPO MOVIMENTO B=PV T=PN 
        //                   04  DF9019-C      PIC X.                             
        //***********                                    NUM.DOMANDA              
        //                   04  DF9020-C      PIC 9(8).                          
        //***********                                    CODICI RETRIBUTIVE       
        //                   04  DF9022-C      PIC X(4).  
        #endregion tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 15)]
        public List<DFA103> LISTADFA103 { get; set; }

        [HisComplexAreaInfoMapping(1, ListCount = 8)]
        public List<SistemaContributivo> LISTASISTEMACONTRIBUTIVO { get; set; }

        [HisComplexAreaInfoMapping(2, ListCount = 10)]
        public List<PensioneAbbinata> LISTAPENSIONIABBINATE { get; set; }

        [HisComplexAreaInfoMapping(3)]
        public InteressiLegali INTERESSILEGALI { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class DFA103
        {
            #region Constructor
            internal DFA103()
            {
            }
            #endregion Constructor

            #region tracciato COBOL
            //                       02  REC-RDA.                                                 
            //            03  DFA103  OCCURS 15 TIMES.                                
            //***********    =  ANNO   AAAA                                           
            //                04  DFA1031                     PIC 9(4).               
            //***********    =  IMP.  ANNUO  PENSIONE                                 
            //                04  DFA1032                     PIC S9(9)V9(4) COMP-3.  
            //***********    =  IMP.  ANNUO  TRATT.FAMIGL.                            
            //                04  DFA1033                     PIC S9(7)V9(4) COMP-3.  
            //***********    =  IMP.  ANNUO  MAGG.SOCIALE                             
            //                04  DFA1034                     PIC S9(7)V9(4) COMP-3.  
            //***********    =  IMP.  ANNUO  MAGG.EX COMB.                            
            //                04  DFA1035                     PIC S9(7)V9(4) COMP-3.  
            //***********    =  IMP.  ANNUO  ASSEGNO ACCOMPAGNO                       
            //                04  DFA1036                     PIC S9(7)V9(4) COMP-3.  
            //***********    =  LIBERI                                                
            //                04  FILLER                      PIC X(7).      
            #endregion tracciato COBOL

            #region Tracciato Host
            // 02  REC-RDA.
            // 03  DFA103  OCCURS 15 TIMES.
            //***********    =  ANNO   AAAA
            /// <summary>
            /// DFA1031 9(4)  
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public short DFA1031 { get; set; }

            //***********    =  IMP.  ANNUO  PENSIONE
            /// <summary>
            /// DFA1032 S9(9)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(1, 7, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal DFA1032 { get; set; }

            //***********    =  IMP.  ANNUO  TRATT.FAMIGL.
            /// <summary>
            /// DFA1033 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(2, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal DFA1033 { get; set; }

            //***********    =  IMP.  ANNUO  MAGG.SOCIALE
            /// <summary>
            /// DFA1034 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(3, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal DFA1034 { get; set; }

            //***********    =  IMP.  ANNUO  MAGG.EX COMB.
            /// <summary>
            /// DFA1035 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(4, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal DFA1035 { get; set; }

            //***********    =  IMP.  ANNUO  ASSEGNO ACCOMPAGNO
            /// <summary>
            /// DFA1036 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(5, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal DFA1036 { get; set; }

            //***********    =  LIBERI
            /// <summary>
            /// FILLER X(7)  
            /// </summary>
            [HisFieldInfoMapping(6, 7)]
            public string FILLER { get; set; }
            #endregion Tracciato Host
        }

        public class SistemaContributivo
        {
            #region Constructor
            internal SistemaContributivo()
            {
            }
            #endregion Constructor

            #region tracciato COBOL
            //                  *                                                              ** 
            //      *  TABELLA LEGGE 335-ART1-COMMA6                                  
            //      *  CALCOLO DELLA PENSIONE COL SISTEMA CONTRIBUTIVO                
            //      *                                                              ** 
            //            03  DFA601    OCCURS 8.                                     
            //***********    =  DECORR SSAAMMGG                                       
            //                04  DFA6011                  PIC 9(8).                  
            //***********    =  P =  PENSIONE      S = SUPPLEMENTI                    
            //                04  DFA6012                  PIC X.                     
            //***********    =  COD. GESTIONE                                         
            //                04  DFA6013                  PIC 9.                     
            //***********    =  MONTANTE CONTRIBUTIVO                                 
            //                04  DFA6014                  PIC S9(9)V9(4) COMP-3.     
            //***********    =  IMPORTO CONTRIBUTI                                    
            //                04  DFA6015                  PIC S9(7)V9(4) COMP-3.     
            //***********    =  COEFF.                                                
            //                04  DFA6016.                                            
            //                    05  DFA6016A             PIC 9.                     
            //                    05  DFA6016B             PIC 9(4).                  
            //***********    =  NUMERO CONTRIBUTI                                     
            //                04  DFA6017                  PIC 9(4).                  
            //                04  FILLER                   PIC XX.   
            #endregion tracciato COBOL

            #region Tracciato Host
            //*                                                              **
            // *  TABELLA LEGGE 335-ART1-COMMA6
            // *  CALCOLO DELLA PENSIONE COL SISTEMA CONTRIBUTIVO
            //*                                                              **
            // 03  DFA601    OCCURS 8.
            //***********    =  DECORR SSAAMMGG
            /// <summary>
            /// DFA6011 9(8)  
            /// </summary>
            [HisFieldInfoMapping(0, 8)]
            public int DFA6011 { get; set; }

            //***********    =  P =  PENSIONE      S = SUPPLEMENTI
            /// <summary>
            /// DFA6012 X  
            /// </summary>
            [HisFieldInfoMapping(1, 1)]
            public string DFA6012 { get; set; }

            //***********    =  COD. GESTIONE
            /// <summary>
            /// DFA6013 9  
            /// </summary>
            [HisFieldInfoMapping(2, 1)]
            public short DFA6013 { get; set; }

            //***********    =  MONTANTE CONTRIBUTIVO
            /// <summary>
            /// DFA6014 S9(9)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(3, 7, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal DFA6014 { get; set; }

            //***********    =  IMPORTO CONTRIBUTI
            /// <summary>
            /// DFA6015 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(4, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal DFA6015 { get; set; }

            //***********    =  COEFF.
            // 04  DFA6016.
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

            //***********    =  NUMERO CONTRIBUTI
            /// <summary>
            /// DFA6017 9(4)  
            /// </summary>
            [HisFieldInfoMapping(7, 4)]
            public short DFA6017 { get; set; }

            /// <summary>
            /// FILLER XX  
            /// </summary>
            [HisFieldInfoMapping(8, 2)]
            public string FILLER { get; set; }
            #endregion Tracciato Host
        }

        public class PensioneAbbinata
        {
            #region Constructor
            internal PensioneAbbinata()
            {
            }
            #endregion Constructor

            #region tracciato COBOL
            //            ***************************************************************** 
            //*   PENSIONI ABBINATE                                          ** 
            //*                                                              ** 
            //      03  DFA702     OCCURS 10.                                   
            //*   CATEGORIA           (PD)                                      
            //          04  DFA702A                    PIC XX.                  
            //*   CERTIFICATO         (PD)                                      
            //          04  DFA702B                    PIC X(4).                
            //*   ABBINATA FISCALMENTE  = 1     (PD)                            
            //          04  DFA702C                    PIC X.                   
            //*   ABBINATA PAGAMENTO = 1        (PD)                            
            //          04  DFA702D                    PIC X.  
            #endregion tracciato COBOL

            #region Tracciato Host
            //*****************************************************************
            // *   PENSIONI ABBINATE                                          **
            //*                                                              **
            // 03  DFA702     OCCURS 10.
            // *   CATEGORIA           (PD)
            /// <summary>
            /// DFA702A XX  
            /// </summary>
            [HisFieldInfoMapping(0, 2, CobolType =CobolType.Untraslate)]
            public int DFA702A { get; set; }

            // *   CERTIFICATO         (PD)
            /// <summary>
            /// DFA702B X(4)  
            /// </summary>
            [HisFieldInfoMapping(1, 4, CobolType = CobolType.Untraslate)]
            public int DFA702B { get; set; }

            // *   ABBINATA FISCALMENTE  = 1     (PD)
            /// <summary>
            /// DFA702C X  
            /// </summary>
            [HisFieldInfoMapping(2, 1, CobolType = CobolType.Untraslate)]
            public short DFA702C { get; set; }

            // *   ABBINATA PAGAMENTO = 1        (PD)
            /// <summary>
            /// DFA702D X  
            /// </summary>
            [HisFieldInfoMapping(3, 1, CobolType = CobolType.Untraslate)]
            public short DFA702D { get; set; }
            #endregion Tracciato Host
        }

        public class InteressiLegali
        {
            #region Constructor
            internal InteressiLegali()
            { }
            #endregion Constructor

            #region tracciato COBOL
            //          03  INTERESSI-LEGALI.
            //            04 DT-INT-LEG                      PIC X(8).
            //            04 DT-FIN-LEG                      PIC X(8).
            //            04 INT-CRED                        PIC S9(9)V9(4) COMP-3.
            //            04 INT-DEB                         PIC S9(9)V9(4) COMP-3.
            //            04 INT-SALDO                       PIC S9(9)V9(4) COMP-3.
            //            04 RIVAL-CRED                      PIC S9(9)V9(4) COMP-3.
            //            04 RIVAL-DEB                       PIC S9(9)V9(4) COMP-3.
            //            04 RIVAL-SALDO                     PIC S9(9)V9(4) COMP-3.
            //            03  DFA701B                        PIC X(50).               
            //      ***************************************************************** 
            //               03  RECSET1C.                                            
            //***********                                      CHIAVE                 
            //                   04  AGG03-C       PIC X(4).                          
            //***********                                   TABELLA VARIAZIONI TASSE  
            //***********                                     AAMM DEC.PENSIONE       
            //***********                SOLO X REVESR.PENSIONE GP7LC02Z AAMM (PD)    
            //                   04  DF9000-C      PIC XX.                            
            //***********                            SCAD.REV.MEDICA AAMM  (PD)       
            //                   04  DF9017-C      PIC XX.                            
            //***********                                    COD.COMUNE NASCITA (PD)  
            //                   04  DF9018-C      PIC XXX.                           
            //***********                                    TIPO MOVIMENTO B=PV T=PN 
            //                   04  DF9019-C      PIC X.                             
            //***********                                    NUM.DOMANDA              
            //                   04  DF9020-C      PIC 9(8).                          
            //***********                                    CODICI RETRIBUTIVE       
            //                   04  DF9022-C      PIC X(4).  
            #endregion tracciato COBOL

            #region Tracciato Host
            // 03  INTERESSI-LEGALI.
            /// <summary>
            /// DT_INT_LEG X(8)  
            /// </summary>
            [HisFieldInfoMapping(0, 8)]
            public string DT_INT_LEG { get; set; }

            /// <summary>
            /// DT_FIN_LEG X(8)  
            /// </summary>
            [HisFieldInfoMapping(1, 8)]
            public string DT_FIN_LEG { get; set; }

            /// <summary>
            /// INT_CRED S9(9)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(2, 7, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal INT_CRED { get; set; }

            /// <summary>
            /// INT_DEB S9(9)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(3, 7, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal INT_DEB { get; set; }

            /// <summary>
            /// INT_SALDO S9(9)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(4, 7, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal INT_SALDO { get; set; }

            /// <summary>
            /// RIVAL_CRED S9(9)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(5, 7, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal RIVAL_CRED { get; set; }

            /// <summary>
            /// RIVAL_DEB S9(9)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(6, 7, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal RIVAL_DEB { get; set; }

            /// <summary>
            /// RIVAL_SALDO S9(9)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(7, 7, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal RIVAL_SALDO { get; set; }

            /// <summary>
            /// DFA701B X(50)  
            /// </summary>
            [HisFieldInfoMapping(8, 50)]
            public string DFA701B { get; set; }

            //*****************************************************************
            // 03  RECSET1C.
            //***********                                      CHIAVE
            /// <summary>
            /// AGG03_C X(4)  
            /// </summary>
            [HisFieldInfoMapping(9, 4)]
            public string AGG03_C { get; set; }

            //***********                                   TABELLA VARIAZIONI TASSE
            //***********                                     AAMM DEC.PENSIONE
            //***********                SOLO X REVESR.PENSIONE GP7LC02Z AAMM (PD)
            /// <summary>
            /// DF9000_C XX  
            /// </summary>
            [HisFieldInfoMapping(10, 2, CobolType = CobolType.Untraslate)]
            public int DF9000_C { get; set; }

            //***********                            SCAD.REV.MEDICA AAMM  (PD)
            /// <summary>
            /// DF9017_C XX  
            /// </summary>
            [HisFieldInfoMapping(11, 2, CobolType = CobolType.Untraslate)]
            public int DF9017_C { get; set; }

            //***********                                    COD.COMUNE NASCITA (PD)
            /// <summary>
            /// DF9018_C XXX  
            /// </summary>
            [HisFieldInfoMapping(12, 3, CobolType = CobolType.Untraslate)]
            public int DF9018_C { get; set; }

            //***********                                    TIPO MOVIMENTO B=PV T=PN
            /// <summary>
            /// DF9019_C X  
            /// </summary>
            [HisFieldInfoMapping(13, 1)]
            public string DF9019_C { get; set; }

            //***********                                    NUM.DOMANDA
            /// <summary>
            /// DF9020_C 9(8)  
            /// </summary>
            [HisFieldInfoMapping(14, 8)]
            public int DF9020_C { get; set; }

            //***********                                    CODICI RETRIBUTIVE
            /// <summary>
            /// DF9022_C X(4)  
            /// </summary>
            [HisFieldInfoMapping(15, 4)]
            public string DF9022_C { get; set; }
            #endregion Tracciato Host

        }
        #endregion nested class
    }
}
