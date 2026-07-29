using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA
{
    public class Gp4IPOST : ITransactionInfo
    {
        #region Properties
        #region Tracciato COBOL
        // 02  SPAC-IPOST.
        //*** CODICE FASCICOLO                                              
        //       03  K-GP4DAAA.
        //         04 K-GP4DAA1 PIC 9(3).                      
        //         04 K-GP4DAA2.
        //            05 K-GP4DAA2-1         PIC 9(4).                      
        //            05 K-GP4DAA2-2         PIC 9(8).                      
        //***************
        //			03  K-GP4DB00 OCCURS 25.                                   
        //*** CATEGORIA PENSIONE LIQUIDATA                                  
        //         04 K-GP4KA01 PIC X(3).                      
        //*** SEDE PENSIONE LIQUIDATA                                       
        //         04 K-GP4KA02 PIC X(2).                      
        //*** ZONA PENSIONE LIQUIDATA                                       
        //         04 K-GP4KA03 PIC X(2).                      
        //*** CERTIFICATO PENSIONE LIQUIDATA                                
        //         04 K-GP4KA04 PIC X(8).                      
        //*** CODICE FISCALE                                                
        //         04 K-GP4DB09 PIC X(16).                     
        //*** CSOG                                                          
        //         04 K-GP4DB13 PIC 9(9).                      
        //*** DATA DI MATRIMONIO                                            
        //         04 K-GP4DB14 PIC 9(8).                      
        //*** CODICE NUCLEO                                                 
        //         04 K-GP4DB15 PIC X(2).                      
        //*** A DISPOSIZIONE                                                
        //         04 FILLER PIC X(50).                     
        //**************                                                                 
        //         04 K-GP4DC00 OCCURS 20.                                  
        //*** PERCENTUALE SPETTANTE                                         
        //            05 K-GP4DC01 PIC 9(3)V9(4).                 
        //*** DECORRENZA PERIODO(AAAA/MM)
        //            05 K-GP4DC02 PIC 9(6).                      
        //*** CESSAZIONE PERIODO(AAAA/MM)
        //            05 K-GP4DC03 PIC 9(6).                      
        //*** CODICE FAMILIARE                                              
        //            05 K-GP4DC04 PIC X(2).                      
        //*** COEFFICIENTE RIDUZIONE                                        
        //            05 K-GP4DC05 PIC 9(3)V9(4).                 
        //*** PERCENTUALE DA SENTENZA CODICE 'E'                            
        //            05 K-GP4DC07 PIC 9(3)V9(4).                 
        //*** A DISPOSIZIONE                                                
        //            05 FILLER PIC X(50).                     
        //*                                                                 
        //     03      FILLER PIC X(28717).
        #endregion
        #region Tracciato Host
        /// <summary>
        /// K-GP4DAA1	CATEGORIA FASCICOLO	9(3)
        /// <summary>
        [HisFieldInfoMapping(0, 3, CobolType = CobolType.Unsigned)]
        public short K_GP4DAA1 { get; set; }

        /// <summary>
        /// K-GP4DAA2-1	SEDE FITTIZIA FASCICOLO (“9990”)	9(4)
        /// <summary>
        [HisFieldInfoMapping(1, 4, CobolType = CobolType.Unsigned)]
        public short K_GP4DAA2_1 { get; set; }

        /// <summary>
        /// K-GP4DAA2-2	PROGRESSIVO FASCICOLO	9(8)
        /// <summary>
        [HisFieldInfoMapping(2, 8, CobolType = CobolType.Unsigned)]
        public int K_GP4DAA2_2 { get; set; }

        /// <summary>
        /// K-GP4DB00	TABELLA CON 25 RIPETIZIONI
        /// <summary>
        [HisComplexAreaInfoMapping(3, ListCount = 25)]
        public List<K_GP4DB00> LISTK_GP4DB00 { get; set; }

        /// <summary>
        /// FILLER  PIC X(29977).
        /// 
        /// RIDOTTO DI 11 PER NON CONSIDERARE FILLER INIZIALE PRECEDENTE IL BYTE 1 DI RIFERIMENTO
        /// <summary>
        [HisFieldInfoMapping(4, 28706)]
        public string FILLER { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class K_GP4DB00
        {
            #region Properties

            #region Tracciato COBOL
            //  03  K-GP4DB00 OCCURS 25.                                   
            //*** CATEGORIA PENSIONE LIQUIDATA                                  
            //         04 K-GP4KA01 PIC X(3).                      
            //*** SEDE PENSIONE LIQUIDATA                                       
            //         04 K-GP4KA02 PIC X(2).                      
            //*** ZONA PENSIONE LIQUIDATA                                       
            //         04 K-GP4KA03 PIC X(2).                      
            //*** CERTIFICATO PENSIONE LIQUIDATA                                
            //         04 K-GP4KA04 PIC X(8).                      
            //*** CODICE FISCALE                                                
            //         04 K-GP4DB09 PIC X(16).                     
            //*** CSOG                                                          
            //         04 K-GP4DB13 PIC 9(9).                      
            //*** DATA DI MATRIMONIO                                            
            //         04 K-GP4DB14 PIC 9(8).                      
            //*** CODICE NUCLEO                                                 
            //         04 K-GP4DB15 PIC X(2).                      
            //*** A DISPOSIZIONE                                                
            //         04 FILLER PIC X(50).                     
            //**************                                                                 
            //         04 K-GP4DC00 OCCURS 20.                                  
            //*** PERCENTUALE SPETTANTE                                         
            //            05 K-GP4DC01 PIC 9(3)V9(4).                 
            //*** DECORRENZA PERIODO(AAAA/MM)
            //            05 K-GP4DC02 PIC 9(6).                      
            //*** CESSAZIONE PERIODO(AAAA/MM)
            //            05 K-GP4DC03 PIC 9(6).                      
            //*** CODICE FAMILIARE                                              
            //            05 K-GP4DC04 PIC X(2).                      
            //*** COEFFICIENTE RIDUZIONE                                        
            //            05 K-GP4DC05 PIC 9(3)V9(4).                 
            //*** PERCENTUALE DA SENTENZA CODICE 'E'                            
            //            05 K-GP4DC07 PIC 9(3)V9(4).                 
            //*** A DISPOSIZIONE                                                
            //            05 FILLER PIC X(50).     
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// K-GP4KA01	CATEGORIA PENSIONE LIQUIDATA	X(3)
            /// <summary>
            [HisFieldInfoMapping(0, 3)]
            public string K_GP4KA01 { get; set; }

            /// <summary>
            /// K-GP4KA02	SEDE PENSIONE LIQUIDATA	X(2)
            /// <summary>
            [HisFieldInfoMapping(1, 2)]
            public string K_GP4KA02 { get; set; }

            /// <summary>
            /// K-GP4KA03	ZONA PENSIONE LIQUIDATA	X(2)
            /// <summary>
            [HisFieldInfoMapping(2, 2)]
            public string K_GP4KA03 { get; set; }

            /// <summary>
            /// K-GP4KA04	CERTIFICATO PENSIONE LIQUIDATA	X(8)
            /// <summary>
            [HisFieldInfoMapping(3, 8)]
            public string K_GP4KA04 { get; set; }

            /// <summary>
            /// K-GP4DB09	CODICE FISCALE	X(16)
            /// <summary>
            [HisFieldInfoMapping(4, 16)]
            public string K_GP4DB09 { get; set; }

            /// <summary>
            /// K-GP4DB13	CSOG	9(9)
            /// <summary>
            [HisFieldInfoMapping(5, 9, CobolType = CobolType.Unsigned)]
            public int K_GP4DB13 { get; set; }

            /// <summary>
            /// K-GP4DB14	DATA DI MATRIMONIO	9(8)
            /// <summary>
            [HisFieldInfoMapping(6, 8, CobolType = CobolType.Unsigned)]
            public int K_GP4DB14 { get; set; }

            /// <summary>
            /// K-GP4DB15	CODICE NUCLEO	X(2) 
            /// <summary>
            [HisFieldInfoMapping(7, 2)]
            public string K_GP4DB15 { get; set; }

            /// <summary>
            /// FILLER	A DISPOSIZIONE 	X(50)
            /// <summary>
            [HisFieldInfoMapping(8, 50)]
            public string FILLER { get; set; }

            /// <summary>
            /// K-GP4DC00	TABELLA CON 20 RIPETIZIONI
            /// <summary>
            [HisComplexAreaInfoMapping(9, ListCount = 20)]
            public List<K_GP4DC00> LISTK_GP4DC00 { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }

        public class K_GP4DC00
        {
            #region Properties

            #region Tracciato COBOL
            //         04 K-GP4DC00 OCCURS 20.                                  
            //*** PERCENTUALE SPETTANTE                                         
            //            05 K-GP4DC01 PIC 9(3)V9(4).                 
            //*** DECORRENZA PERIODO(AAAA/MM)
            //            05 K-GP4DC02 PIC 9(6).                      
            //*** CESSAZIONE PERIODO(AAAA/MM)
            //            05 K-GP4DC03 PIC 9(6).                      
            //*** CODICE FAMILIARE                                              
            //            05 K-GP4DC04 PIC X(2).                      
            //*** COEFFICIENTE RIDUZIONE                                        
            //            05 K-GP4DC05 PIC 9(3)V9(4).                 
            //*** PERCENTUALE DA SENTENZA CODICE 'E'                            
            //            05 K-GP4DC07 PIC 9(3)V9(4).                 
            //*** A DISPOSIZIONE                                                
            //            05 FILLER PIC X(50).
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// K-GP4DC01	PERCENTUALE SPETTANTE 	9(3) V9(4) 
            /// </summary>
            [HisFieldInfoMapping(0, 7, Scale = 4, CobolType = CobolType.Unsigned)]
            public decimal K_GP4DC01 { get; set; }

            /// <summary>
            /// K-GP4DC02	DECORRENZA PERIODO (AAAA/MM)	9(6)
            /// <summary>
            [HisFieldInfoMapping(1, 6)]
            public int K_GP4DC02 { get; set; }

            /// <summary>
            /// K-GP4DC03	CESSAZIONE PERIODO (AAAA/MM)	9(6)
            /// <summary>
            [HisFieldInfoMapping(2, 6)]
            public int K_GP4DC03 { get; set; }

            /// <summary>
            /// K-GP4DC04	CODICE FAMILIARE	X(2)
            /// <summary>
            [HisFieldInfoMapping(3, 2)]
            public string K_GP4DC04 { get; set; }

            /// <summary>
            /// K-GP4DC05	COEFFICIENTE RIDUZIONE	9(3) V9(4)
            /// </summary>
            [HisFieldInfoMapping(4, 7, Scale = 4, CobolType = CobolType.Unsigned)]
            public decimal K_GP4DC05 { get; set; }

            /// <summary>
            /// K-GP4DC07	PERCENTUALE GIUDICE CODICE “E”	9(3) V9(4)  
            /// </summary>
            [HisFieldInfoMapping(5, 7, Scale = 4, CobolType = CobolType.Unsigned)]
            public decimal K_GP4DC07 { get; set; }

            /// <summary>
            /// FILLER	A DISPOSIZIONE 	X(50)
            /// <summary>
            [HisFieldInfoMapping(6, 50)]
            public string FILLER { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class
        public string TransactionName
        {
            get { return "AreaIPOST"; }
        }
        #endregion
    }
}
