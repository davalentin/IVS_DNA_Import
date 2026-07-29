using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class CI01Record_RB
    {
        #region Constructor
        internal CI01Record_RB()
        {
        }
        #endregion Constructor
        #region tracciato COBOL
        //           02  REC-RB.                                                  
        //               03  RECSET1B.                                            
        //***********  LE DECORRENZE SONO 71 PIENE                                
        //                   04  ELEM-IMPORTI-B OCCURS 71.                        
        //                       05  KC01-B.                                      
        //                           06  KC01A-B       PIC 9(4).                  
        //                           06  KC01M-B       PIC 99.                    
        //                       05  KC04-B            PIC S9(7)V9(4) COMP-3.     
        //                       05  KC10-B            PIC S9(7)V9(4) COMP-3.     
        //                       05  KC03-B            PIC S9(5)V9(4) COMP-3.     
        //                       05  KE07-B            PIC S9(7)V9(4) COMP-3.     
        //                       05  KM63-B            PIC S9(7)V9(4) COMP-3.     
        //                       05  KE08-B            PIC S9(7)V9(4) COMP-3.     
        //                       05  KE04-B            PIC S9(7)V9(4) COMP-3.     
        //                       05  HI01-B            PIC S9(5)V9(4) COMP-3.     
        //                       05  FO77-B            PIC S9(5)V9(4) COMP-3.     
        //                       05  C335-B            PIC 99.                    
        //                       05  FO335-B           PIC S9(7)V9(4) COMP-3.     
        //                       05  FO80-B            PIC S9(7)V9(4) COMP-3.     
        //                       05  FO87-B            PIC S9(7)V9(4) COMP-3.     
        //                       05  KD01-B            PIC S9(7)V9(4) COMP-3.     
        //                       05  YUGO-B            PIC S9(7)V9(4) COMP-3.     
        //                       05  KD03-B            PIC S9(7)V9(4) COMP-3.     
        //                       05  KD-INPDAP         PIC S9(3)V99 COMP-3.       
        //      **********  METTERE A LOW-VALUE                                   
        //                       05  CAMPO-LOW-VALUE-B PIC X(1).  
        #endregion tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 71)]
        public List<Importo> LISTAIMPORTI { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class Importo
        {
            #region Constructor
            internal Importo()
            {
            }
            #endregion Constructor
            #region tracciato COBOL
            //           02  REC-RB.                                                  
            //               03  RECSET1B.                                            
            //***********  LE DECORRENZE SONO 71 PIENE                                
            //                   04  ELEM-IMPORTI-B OCCURS 71.                        
            //                       05  KC01-B.                                      
            //                           06  KC01A-B       PIC 9(4).                  
            //                           06  KC01M-B       PIC 99.                    
            //                       05  KC04-B            PIC S9(7)V9(4) COMP-3.     
            //                       05  KC10-B            PIC S9(7)V9(4) COMP-3.     
            //                       05  KC03-B            PIC S9(5)V9(4) COMP-3.     
            //                       05  KE07-B            PIC S9(7)V9(4) COMP-3.     
            //                       05  KM63-B            PIC S9(7)V9(4) COMP-3.     
            //                       05  KE08-B            PIC S9(7)V9(4) COMP-3.     
            //                       05  KE04-B            PIC S9(7)V9(4) COMP-3.     
            //                       05  HI01-B            PIC S9(5)V9(4) COMP-3.     
            //                       05  FO77-B            PIC S9(5)V9(4) COMP-3.     
            //                       05  C335-B            PIC 99.                    
            //                       05  FO335-B           PIC S9(7)V9(4) COMP-3.     
            //                       05  FO80-B            PIC S9(7)V9(4) COMP-3.     
            //                       05  FO87-B            PIC S9(7)V9(4) COMP-3.     
            //                       05  KD01-B            PIC S9(7)V9(4) COMP-3.     
            //                       05  YUGO-B            PIC S9(7)V9(4) COMP-3.     
            //                       05  KD03-B            PIC S9(7)V9(4) COMP-3.     
            //                       05  KD-INPDAP         PIC S9(3)V99 COMP-3.       
            //      **********  METTERE A LOW-VALUE                                   
            //                       05  CAMPO-LOW-VALUE-B PIC X(1).  
            #endregion tracciato COBOL

            #region Tracciato Host
            // 02  REC-RB.
            // 03  RECSET1B.
            //***********  LE DECORRENZE SONO 71 PIENE
            // 04  ELEM-IMPORTI-B OCCURS 71.
            // 05  KC01-B.
            /// <summary>
            /// KC01A_B 9(4)  
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public short KC01A_B { get; set; }

            /// <summary>
            /// KC01M_B 99  
            /// </summary>
            [HisFieldInfoMapping(1, 2)]
            public short KC01M_B { get; set; }

            /// <summary>
            /// KC04_B S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(2, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal KC04_B { get; set; }

            /// <summary>
            /// KC10_B S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(3, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal KC10_B { get; set; }

            /// <summary>
            /// KC03_B S9(5)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(4, 5, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal KC03_B { get; set; }

            /// <summary>
            /// KE07_B S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(5, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal KE07_B { get; set; }

            /// <summary>
            /// KM63_B S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(6, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal KM63_B { get; set; }

            /// <summary>
            /// KE08_B S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(7, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal KE08_B { get; set; }

            /// <summary>
            /// KE04_B S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(8, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal KE04_B { get; set; }

            /// <summary>
            /// HI01_B S9(5)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(9, 5, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal HI01_B { get; set; }

            /// <summary>
            /// FO77_B S9(5)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(10, 5, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal FO77_B { get; set; }

            /// <summary>
            /// C335_B 99  
            /// </summary>
            [HisFieldInfoMapping(11, 2)]
            public short C335_B { get; set; }

            /// <summary>
            /// FO335_B S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(12, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal FO335_B { get; set; }

            /// <summary>
            /// FO80_B S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(13, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal FO80_B { get; set; }

            /// <summary>
            /// FO87_B S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(14, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal FO87_B { get; set; }

            /// <summary>
            /// KD01_B S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(15, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal KD01_B { get; set; }

            /// <summary>
            /// YUGO_B S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(16, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal YUGO_B { get; set; }

            /// <summary>
            /// KD03_B S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(17, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal KD03_B { get; set; }

            /// <summary>
            /// KD_INPDAP S9(3)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(18, 3, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal KD_INPDAP { get; set; }

            //**********  METTERE A LOW-VALUE
            // 05  CAMPO-LOW-VALUE-B PIC X(1).
            [HisFieldInfoMapping(19, 1)]
            public short CAMPO_LOW_VALUE_B { get; set; }
            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
