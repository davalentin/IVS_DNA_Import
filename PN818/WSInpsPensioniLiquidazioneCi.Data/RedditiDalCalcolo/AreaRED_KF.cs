using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIRED4
{
    public class AreaRED_KF
    {
        #region Constructor
        internal AreaRED_KF()
        {
        }
        #endregion Constructor

        #region tracciato COBOL
        //       ************************************REDDITI X AFN                        
        //2011       02  GP2KF00  OCCURS 60.                                       
        //               03  GP2KF01Z                         PIC 9999.            
        //               03  GP2KM70RZ.                                            
        //                   04 GP2KM70SA                     PIC 9(4).            
        //                   04 GP2KM70SAR REDEFINES                               
        //                      GP2KM70SA.                                         
        //                      05 GP2KM70S                   PIC 99.              
        //                      05 GP2KM70A                   PIC 99.              
        //                   04 GP2KM70M                      PIC 99.              
        //               03  GP2KM70Z REDEFINES GP2KM70RZ     PIC 9(6).            
        //               03  GP2KF02                          PIC X.               
        //               03  GP2KF03E                     PIC S9(7)V9(2) COMP-3.   
        //               03  GP2KG07E                     PIC S9(7)V9(2) COMP-3.   
        //               03  GP2KM73                          PIC 99.              
        //               03  GP2KM71                          PIC 9.               
        //               03  GP2KM75                          PIC 9.               
        //2004R          03  GP2KF11                          PIC 9(7).            
        //2004R          03  GP2KF11R REDEFINES GP2KF11.                           
        //2004R              04  GP2KF11A                     PIC 9.               
        //2004R              04  GP2KF11B                     PIC 99.              
        //2004R              04  GP2KF11C                     PIC 99.              
        //2004R              04  GP2KF11D                     PIC 99.              
        //2004R          03  GP2KG11R1 REDEFINES GP2KF11.                          
        //2004R              04  GP2KF11RA                    PIC 9.               
        //2004R              04  GP2KF11RB                    PIC 9.               
        //2004R              04  GP2KF11RC                    PIC 9.               
        //2004R              04  GP2KF11RD                    PIC 9.               
        //2004R              04  GP2KF11RE                    PIC 9.               
        //2004R              04  GP2KF11RF                    PIC 99. 
        //2005           03  GP2KFRIL                         PIC X(20).           
        //2010           03  GP2KFRIL2                        PIC X(20). 
        #endregion tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 60)]
        public List<Gp2kf00> LISTAGP2KF00 { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class Gp2kf00
        {
            #region Constructor
            internal Gp2kf00()
            {
            }
            #endregion Constructor

            #region tracciato COBOL
            //           ************************************REDDITI X AFN                        
            //2011       02  GP2KF00  OCCURS 60.                                       
            //               03  GP2KF01Z                         PIC 9999.            
            //               03  GP2KM70RZ.                                            
            //                   04 GP2KM70SA                     PIC 9(4).            
            //                   04 GP2KM70SAR REDEFINES                               
            //                      GP2KM70SA.                                         
            //                      05 GP2KM70S                   PIC 99.              
            //                      05 GP2KM70A                   PIC 99.              
            //                   04 GP2KM70M                      PIC 99.              
            //               03  GP2KM70Z REDEFINES GP2KM70RZ     PIC 9(6).            
            //               03  GP2KF02                          PIC X.               
            //               03  GP2KF03E                     PIC S9(7)V9(2) COMP-3.   
            //               03  GP2KG07E                     PIC S9(7)V9(2) COMP-3.   
            //               03  GP2KM73                          PIC 99.              
            //               03  GP2KM71                          PIC 9.               
            //               03  GP2KM75                          PIC 9.               
            //2004R          03  GP2KF11                          PIC 9(7).            
            //2004R          03  GP2KF11R REDEFINES GP2KF11.                           
            //2004R              04  GP2KF11A                     PIC 9.               
            //2004R              04  GP2KF11B                     PIC 99.              
            //2004R              04  GP2KF11C                     PIC 99.              
            //2004R              04  GP2KF11D                     PIC 99.              
            //2004R          03  GP2KG11R1 REDEFINES GP2KF11.                          
            //2004R              04  GP2KF11RA                    PIC 9.               
            //2004R              04  GP2KF11RB                    PIC 9.               
            //2004R              04  GP2KF11RC                    PIC 9.               
            //2004R              04  GP2KF11RD                    PIC 9.               
            //2004R              04  GP2KF11RE                    PIC 9.               
            //2004R              04  GP2KF11RF                    PIC 99. 
            //2005           03  GP2KFRIL                         PIC X(20).           
            //2010           03  GP2KFRIL2                        PIC X(20). 
            #endregion tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP2KF01Z 9999  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short GP2KF01Z { get; set; }

            // 03  GP2KM70RZ.

            /// <summary>
            /// GP2KM70S 99  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short GP2KM70S { get; set; }


            /// <summary>
            /// GP2KM70A 99  
            /// </summary>
            [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
            public short GP2KM70A { get; set; }


            /// <summary>
            /// GP2KM70M 99  
            /// </summary>
            [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
            public short GP2KM70M { get; set; }


            /// <summary>
            /// GP2KF02 X  
            /// </summary>
            [HisFieldInfoMapping(4, 1)]
            public string GP2KF02 { get; set; }


            /// <summary>
            /// GP2KF03E S9(7)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(5, 5, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal GP2KF03E { get; set; }


            /// <summary>
            /// GP2KG07E S9(7)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(6, 5, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal GP2KG07E { get; set; }


            /// <summary>
            /// GP2KM73 99  
            /// </summary>
            [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
            public short GP2KM73 { get; set; }


            /// <summary>
            /// GP2KM71 9  
            /// </summary>
            [HisFieldInfoMapping(8, 1, CobolType = CobolType.Unsigned)]
            public short GP2KM71 { get; set; }


            /// <summary>
            /// GP2KM75 9  
            /// </summary>
            [HisFieldInfoMapping(9, 1, CobolType = CobolType.Unsigned)]
            public short GP2KM75 { get; set; }


            /// <summary>
            /// GP2KF11RA 9  
            /// </summary>
            [HisFieldInfoMapping(10, 1, CobolType = CobolType.Unsigned)]
            public short GP2KF11RA { get; set; }


            /// <summary>
            /// GP2KF11RB 9  
            /// </summary>
            [HisFieldInfoMapping(11, 1, CobolType = CobolType.Unsigned)]
            public short GP2KF11RB { get; set; }


            /// <summary>
            /// GP2KF11RC 9  
            /// </summary>
            [HisFieldInfoMapping(12, 1, CobolType = CobolType.Unsigned)]
            public short GP2KF11RC { get; set; }


            /// <summary>
            /// GP2KF11RD 9  
            /// </summary>
            [HisFieldInfoMapping(13, 1, CobolType = CobolType.Unsigned)]
            public short GP2KF11RD { get; set; }


            /// <summary>
            /// GP2KF11RE 9  
            /// </summary>
            [HisFieldInfoMapping(14, 1, CobolType = CobolType.Unsigned)]
            public short GP2KF11RE { get; set; }


            /// <summary>
            /// GP2KF11RF 99  
            /// </summary>
            [HisFieldInfoMapping(15, 2, CobolType = CobolType.Unsigned)]
            public short GP2KF11RF { get; set; }


            /// <summary>
            /// GP2KFRIL X(20)  
            /// </summary>
            [HisFieldInfoMapping(16, 20)]
            public string GP2KFRIL { get; set; }


            /// <summary>
            /// GP2KFRIL2 X(20)  
            /// </summary>
            [HisFieldInfoMapping(17, 20)]
            public string GP2KFRIL2 { get; set; }

            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
