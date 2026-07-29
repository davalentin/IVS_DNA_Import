using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIRED4
{
    public class AreaRED_BLE
    {
        #region Constructor
        internal AreaRED_BLE()
        {
        }
        #endregion Constructor

        #region tracciato COBOL
        //       ************************************REDDITI X LAVORO DIPENDENTE ESTERO   
        //2004R      02  GP2BLE      OCCURS 10 TIMES.                              
        //               03  GP2BLE1                          PIC 9999.            
        //               03  GP2BLE2E                    PIC S9(7)V9(2) COMP-3.    
        //               03  GP2BLE3                          PIC 9999.            
        //               03  GP2BLE3R REDEFINES GP2BLE3.                           
        //                  04  GP2BLE3-M1                    PIC 99.              
        //                  04  GP2BLE3-M2                    PIC 99.              
        //2004R          03  GP2BLE11                         PIC 9(7).            
        //2004R          03  GP2BLE11R REDEFINES GP2BLE11.                         
        //2004R              04  GP2BLE11A                     PIC 9.              
        //2004R              04  GP2BLE11B                     PIC 99.             
        //2004R              04  GP2BLE11C                     PIC 99.             
        //2004R              04  GP2BLE11D                     PIC 99.             
        //2004R          03  GP2BLE11R1 REDEFINES GP2BLE11.                        
        //2004R              04  GP2BLE11RA                    PIC 9.              
        //2004R              04  GP2BLE11RB                    PIC 9.              
        //2004R              04  GP2BLE11RC                    PIC 9.              
        //2004R              04  GP2BLE11RD                    PIC 9.              
        //2004R              04  GP2BLE11RE                    PIC 9.              
        //2004R              04  GP2BLE11RF                    PIC 99.             
        //2005           03  GP2BLRIL                         PIC X(20).           
        //2010           03  GP2BLRIL2                        PIC X(20).    
        #endregion tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 10)]
        public List<Gp2ble> LISTAGP2BLE { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class Gp2ble
        {
            #region Constructor
            internal Gp2ble()
            {
            }
            #endregion Constructor

            #region tracciato COBOL
            //************************************REDDITI X LAVORO DIPENDENTE ESTERO   
            //2004R      02  GP2BLE      OCCURS 10 TIMES.                              
            //               03  GP2BLE1                          PIC 9999.            
            //               03  GP2BLE2E                    PIC S9(7)V9(2) COMP-3.    
            //               03  GP2BLE3                          PIC 9999.            
            //               03  GP2BLE3R REDEFINES GP2BLE3.                           
            //                  04  GP2BLE3-M1                    PIC 99.              
            //                  04  GP2BLE3-M2                    PIC 99.              
            //2004R          03  GP2BLE11                         PIC 9(7).            
            //2004R          03  GP2BLE11R REDEFINES GP2BLE11.                         
            //2004R              04  GP2BLE11A                     PIC 9.              
            //2004R              04  GP2BLE11B                     PIC 99.             
            //2004R              04  GP2BLE11C                     PIC 99.             
            //2004R              04  GP2BLE11D                     PIC 99.             
            //2004R          03  GP2BLE11R1 REDEFINES GP2BLE11.                        
            //2004R              04  GP2BLE11RA                    PIC 9.              
            //2004R              04  GP2BLE11RB                    PIC 9.              
            //2004R              04  GP2BLE11RC                    PIC 9.              
            //2004R              04  GP2BLE11RD                    PIC 9.              
            //2004R              04  GP2BLE11RE                    PIC 9.              
            //2004R              04  GP2BLE11RF                    PIC 99.             
            //2005           03  GP2BLRIL                         PIC X(20).           
            //2010           03  GP2BLRIL2                        PIC X(20).    
            #endregion tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP2BLE1 9999  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short GP2BLE1 { get; set; }


            /// <summary>
            /// GP2BLE2E S9(7)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(1, 5, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal GP2BLE2E { get; set; }


            /// <summary>
            /// GP2BLE3_M1 99  
            /// </summary>
            [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
            public short GP2BLE3_M1 { get; set; }


            /// <summary>
            /// GP2BLE3_M2 99  
            /// </summary>
            [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
            public short GP2BLE3_M2 { get; set; }


            /// <summary>
            /// GP2BLE11RA 9  
            /// </summary>
            [HisFieldInfoMapping(4, 1, CobolType = CobolType.Unsigned)]
            public short GP2BLE11RA { get; set; }


            /// <summary>
            /// GP2BLE11RB 9  
            /// </summary>
            [HisFieldInfoMapping(5, 1, CobolType = CobolType.Unsigned)]
            public short GP2BLE11RB { get; set; }


            /// <summary>
            /// GP2BLE11RC 9  
            /// </summary>
            [HisFieldInfoMapping(6, 1, CobolType = CobolType.Unsigned)]
            public short GP2BLE11RC { get; set; }


            /// <summary>
            /// GP2BLE11RD 9  
            /// </summary>
            [HisFieldInfoMapping(7, 1, CobolType = CobolType.Unsigned)]
            public short GP2BLE11RD { get; set; }


            /// <summary>
            /// GP2BLE11RE 9  
            /// </summary>
            [HisFieldInfoMapping(8, 1, CobolType = CobolType.Unsigned)]
            public short GP2BLE11RE { get; set; }


            /// <summary>
            /// GP2BLE11RF 99  
            /// </summary>
            [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
            public short GP2BLE11RF { get; set; }


            /// <summary>
            /// GP2BLRIL X(20)  
            /// </summary>
            [HisFieldInfoMapping(10, 20)]
            public string GP2BLRIL { get; set; }


            /// <summary>
            /// GP2BLRIL2 X(20)  
            /// </summary>
            [HisFieldInfoMapping(11, 20)]
            public string GP2BLRIL2 { get; set; }


            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
