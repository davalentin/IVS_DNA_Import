using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class CI02Record_RB
    {
        #region Constructor
        internal CI02Record_RB()
        {
        }
        #endregion Constructor

        #region tracciato COBOL
        //              *                                                                 
        //      * **************************************************************  
        //      * *******       INIZIO SECONDO TIPO RECORD                  ****  
        //      * *******  POSSONO ESSERE PRESENTI FINO A TRE TIPO RECORD DUE **  
        //      * **************************************************************  
        //           02  REC-RB.                                                  
        //               03  ST01-B                    PIC X(4).                  
        //               03  FILLER                    PIC X(76).                 
        //               03  RECSET1B.                                            
        //***********  LE DECORRENZE SONO                                         
        //***********         23 + 1(VUOTA) + 23 + 1(VUOTA) + 22. (80 BYTES) 


        //                   04  ELEM-IMPORTI-B OCCURS 71.                        
        //                       05  KC01.                                        
        //                           06  KC01A         PIC 9(4).                  
        //                           06  KC01M         PIC 99.                    
        //                       05  KC04              PIC S9(7)V9(4) COMP-3.     
        //                       05  KC10              PIC S9(7)V9(4) COMP-3.     
        //                       05  KC03              PIC S9(5)V9(4) COMP-3.     
        //                       05  KE07              PIC S9(7)V9(4) COMP-3.     
        //                       05  KM63              PIC S9(7)V9(4) COMP-3.     
        //                       05  KE08              PIC S9(7)V9(4) COMP-3.     
        //                       05  KE04              PIC S9(7)V9(4) COMP-3.     
        //                       05  HI01              PIC S9(5)V9(4) COMP-3.     
        //                       05  FO77              PIC S9(5)V9(4) COMP-3.     
        //                       05  C335              PIC 99.                    
        //                       05  FO335             PIC S9(7)V9(4) COMP-3.     
        //                       05  FO80              PIC S9(7)V9(4) COMP-3.     
        //                       05  FO87              PIC S9(7)V9(4) COMP-3.     
        //                       05  KD01              PIC S9(7)V9(4) COMP-3.     
        //                       05  YUGO              PIC S9(7)V9(4) COMP-3.     
        //                       05  KD03              PIC S9(7)V9(4) COMP-3.     
        //                       05  KD-INPDAP         PIC S9(3)V99 COMP-3.       
        //      **********  METTERE A LOW-VALUE                                   
        //                       05  CAMPO-LOW-VALUE-B PIC X(1).     
        #endregion tracciato COBOL

        #region Tracciato Host
        //* **************************************************************
        //* *******       INIZIO SECONDO TIPO RECORD                  ****
        //* *******  POSSONO ESSERE PRESENTI FINO A TRE TIPO RECORD DUE **
        //* **************************************************************
        // 02  REC-RB.
        /// <summary>
        /// ST01_B X(4)  
        /// </summary>
        [HisFieldInfoMapping(0, 4)]
        public string ST01_B { get; set; }

        /// <summary>
        /// FILLER X(76)  
        /// </summary>
        [HisFieldInfoMapping(1, 76)]
        public string FILLER { get; set; }

        // 03  RECSET1B.
        //***********  LE DECORRENZE SONO
        //***********         23 + 1(VUOTA) + 23 + 1(VUOTA) + 22. (80 BYTES)

        [HisComplexAreaInfoMapping(2, ListCount = 71)]
        public List<Importo> IMPORTI { get; set; }
        #endregion Tracciato Host

        #region nested class

        public class Importo
        {
            #region Constructor
            internal Importo()
            { }
            #endregion Constructor

            #region tracciato COBOL
            //                   04  ELEM-IMPORTI-B OCCURS 71.                        
            //                       05  KC01.                                        
            //                           06  KC01A         PIC 9(4).                  
            //                           06  KC01M         PIC 99.                    
            //                       05  KC04              PIC S9(7)V9(4) COMP-3.     
            //                       05  KC10              PIC S9(7)V9(4) COMP-3.     
            //                       05  KC03              PIC S9(5)V9(4) COMP-3.     
            //                       05  KE07              PIC S9(7)V9(4) COMP-3.     
            //                       05  KM63              PIC S9(7)V9(4) COMP-3.     
            //                       05  KE08              PIC S9(7)V9(4) COMP-3.     
            //                       05  KE04              PIC S9(7)V9(4) COMP-3.     
            //                       05  HI01              PIC S9(5)V9(4) COMP-3.     
            //                       05  FO77              PIC S9(5)V9(4) COMP-3.     
            //                       05  C335              PIC 99.                    
            //                       05  FO335             PIC S9(7)V9(4) COMP-3.     
            //                       05  FO80              PIC S9(7)V9(4) COMP-3.     
            //                       05  FO87              PIC S9(7)V9(4) COMP-3.     
            //                       05  KD01              PIC S9(7)V9(4) COMP-3.     
            //                       05  YUGO              PIC S9(7)V9(4) COMP-3.     
            //                       05  KD03              PIC S9(7)V9(4) COMP-3.     
            //                       05  KD-INPDAP         PIC S9(3)V99 COMP-3.       
            //      **********  METTERE A LOW-VALUE                                   
            //                       05  CAMPO-LOW-VALUE-B PIC X(1).    
            #endregion tracciato COBOL

            #region Tracciato Host
            // 05  KC01.
            /// <summary>
            /// KC01A 9(4)  
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public short KC01A { get; set; }

            /// <summary>
            /// KC01M 9(2)  
            /// </summary>
            [HisFieldInfoMapping(1, 2)]
            public short KC01M { get; set; }

            /// <summary>
            /// KC04 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(2, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal KC04 { get; set; }

            /// <summary>
            /// KC10 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(3, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal KC10 { get; set; }

            /// <summary>
            /// KC03 S9(5)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(4, 5, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal KC03 { get; set; }

            /// <summary>
            /// KE07 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(5, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal KE07 { get; set; }

            /// <summary>
            /// KM63 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(6, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal KM63 { get; set; }

            /// <summary>
            /// KE08 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(7, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal KE08 { get; set; }

            /// <summary>
            /// KE04 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(8, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal KE04 { get; set; }

            /// <summary>
            /// HI01 S9(5)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(9, 5, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal HI01 { get; set; }

            /// <summary>
            /// FO77 S9(5)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(10, 5, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal FO77 { get; set; }

            /// <summary>
            /// C335 9(2)  
            /// </summary>
            [HisFieldInfoMapping(11, 2)]
            public short C335 { get; set; }

            /// <summary>
            /// FO335 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(12, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal FO335 { get; set; }

            /// <summary>
            /// FO80 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(13, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal FO80 { get; set; }

            /// <summary>
            /// FO87 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(14, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal FO87 { get; set; }

            /// <summary>
            /// KD01 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(15, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal KD01 { get; set; }

            /// <summary>
            /// YUGO S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(16, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal YUGO { get; set; }

            /// <summary>
            /// KD03 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(17, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal KD03 { get; set; }

            /// <summary>
            /// KD_INPDAP S9(3)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(18, 3, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal KD_INPDAP { get; set; }

            //**********  METTERE A LOW-VALUE
            // 05  CAMPO-LOW-VALUE-B PIC X(1).
            [HisFieldInfoMapping(19, 1)]
            public string CAMPO_LOW_VALUE_B { get; set; }
            #endregion Tracciato Host
        }

        #endregion nested class
    }
}
