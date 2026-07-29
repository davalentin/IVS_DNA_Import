using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIRED4
{
    public class AreaRED_KM
    {
        #region Constructor
        internal AreaRED_KM()
        {
        }
        #endregion Constructor

        #region tracciato COBOL
        //        ************************************REDDITI X MAGG.SOC.                  
        //2011       02  GP2KM50  OCCURS 50.                                       
        //               03  GP2KM51Z                         PIC 9999.            
        //               03  GP2KM5ARZ.                                            
        //                   04 GP2KM5SA                      PIC 9(4).            
        //                   04 GP2KM5SAR REDEFINES                                
        //                      GP2KM5SA.                                          
        //                      05 GP2KM5SS                   PIC 99.              
        //                      05 GP2KM5AA                   PIC 99.              
        //                   04 GP2KM5AM                      PIC 99.              
        //               03  GP2KM5AZ REDEFINES GP2KM5ARZ     PIC 9(6).            
        //               03  GP2KM52                          PIC 99.              
        //               03  GP2KM53E                     PIC S9(7)V9(2) COMP-3.   
        //2011           03  GP2KM53CP                    PIC X.                   
        //2011           03  GP2KM53CD                    PIC X.                   
        //2011           03  GP2KM53P                     PIC S9(7)V9(2) COMP-3.   
        //2011           03  GP2KM53D                     PIC S9(7)V9(2) COMP-3.   
        //               03  GP2KM54E                     PIC S9(7)V9(2) COMP-3.   
        //2011           03  GP2KM54CP                    PIC X.                   
        //2011           03  GP2KM54CD                    PIC X.                   
        //2011           03  GP2KM54P                     PIC S9(7)V9(2) COMP-3.   
        //2011           03  GP2KM54D                     PIC S9(7)V9(2) COMP-3.   
        //2004R          03  GP2KM11                          PIC 9(7).            
        //2004R          03  GP2KM11R REDEFINES GP2KM11.                           
        //2004R              04  GP2KM11A                     PIC 9.               
        //2004R              04  GP2KM11B                     PIC 99.              
        //2004R              04  GP2KM11C                     PIC 99.              
        //2004R              04  GP2KM11D                     PIC 99.              
        //2004R          03  GP2KM11R1 REDEFINES GP2KM11.                          
        //2004R              04  GP2KM11RA                    PIC 9.               
        //2004R              04  GP2KM11RB                    PIC 9.               
        //2004R              04  GP2KM11RC                    PIC 9.               
        //2004R              04  GP2KM11RD                    PIC 9.               
        //2004R              04  GP2KM11RE                    PIC 9.               
        //2004R              04  GP2KM11RF                    PIC 99.              
        //2005           03  GP2KMRIL                         PIC X(20).           
        //2010           03  GP2KMRIL2                        PIC X(20). 
        #endregion tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 50)]
        public List<Gp2km50> LISTAGP2KM50 { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class Gp2km50
        {
            #region Constructor
            internal Gp2km50()
            {
            }
            #endregion Constructor

            #region tracciato COBOL
            //************************************REDDITI X MAGG.SOC.                  
            //2011       02  GP2KM50  OCCURS 50.                                       
            //               03  GP2KM51Z                         PIC 9999.            
            //               03  GP2KM5ARZ.                                            
            //                   04 GP2KM5SA                      PIC 9(4).            
            //                   04 GP2KM5SAR REDEFINES                                
            //                      GP2KM5SA.                                          
            //                      05 GP2KM5SS                   PIC 99.              
            //                      05 GP2KM5AA                   PIC 99.              
            //                   04 GP2KM5AM                      PIC 99.              
            //               03  GP2KM5AZ REDEFINES GP2KM5ARZ     PIC 9(6).            
            //               03  GP2KM52                          PIC 99.              
            //               03  GP2KM53E                     PIC S9(7)V9(2) COMP-3.   
            //2011           03  GP2KM53CP                    PIC X.                   
            //2011           03  GP2KM53CD                    PIC X.                   
            //2011           03  GP2KM53P                     PIC S9(7)V9(2) COMP-3.   
            //2011           03  GP2KM53D                     PIC S9(7)V9(2) COMP-3.   
            //               03  GP2KM54E                     PIC S9(7)V9(2) COMP-3.   
            //2011           03  GP2KM54CP                    PIC X.                   
            //2011           03  GP2KM54CD                    PIC X.                   
            //2011           03  GP2KM54P                     PIC S9(7)V9(2) COMP-3.   
            //2011           03  GP2KM54D                     PIC S9(7)V9(2) COMP-3.   
            //2004R          03  GP2KM11                          PIC 9(7).            
            //2004R          03  GP2KM11R REDEFINES GP2KM11.                           
            //2004R              04  GP2KM11A                     PIC 9.               
            //2004R              04  GP2KM11B                     PIC 99.              
            //2004R              04  GP2KM11C                     PIC 99.              
            //2004R              04  GP2KM11D                     PIC 99.              
            //2004R          03  GP2KM11R1 REDEFINES GP2KM11.                          
            //2004R              04  GP2KM11RA                    PIC 9.               
            //2004R              04  GP2KM11RB                    PIC 9.               
            //2004R              04  GP2KM11RC                    PIC 9.               
            //2004R              04  GP2KM11RD                    PIC 9.               
            //2004R              04  GP2KM11RE                    PIC 9.               
            //2004R              04  GP2KM11RF                    PIC 99.              
            //2005           03  GP2KMRIL                         PIC X(20).           
            //2010           03  GP2KMRIL2                        PIC X(20). 
            #endregion tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP2KM51Z 9999  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short GP2KM51Z { get; set; }


            /// <summary>
            /// GP2KM5SS 99  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short GP2KM5SS { get; set; }


            /// <summary>
            /// GP2KM5AA 99  
            /// </summary>
            [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
            public short GP2KM5AA { get; set; }


            /// <summary>
            /// GP2KM5AM 99  
            /// </summary>
            [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
            public short GP2KM5AM { get; set; }


            /// <summary>
            /// GP2KM52 99  
            /// </summary>
            [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
            public short GP2KM52 { get; set; }


            /// <summary>
            /// GP2KM53E S9(7)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(5, 5, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal GP2KM53E { get; set; }


            /// <summary>
            /// GP2KM53CP X  
            /// </summary>
            [HisFieldInfoMapping(6, 1)]
            public string GP2KM53CP { get; set; }


            /// <summary>
            /// GP2KM53CD X  
            /// </summary>
            [HisFieldInfoMapping(7, 1)]
            public string GP2KM53CD { get; set; }


            /// <summary>
            /// GP2KM53P S9(7)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(8, 5, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal GP2KM53P { get; set; }


            /// <summary>
            /// GP2KM53D S9(7)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(9, 5, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal GP2KM53D { get; set; }


            /// <summary>
            /// GP2KM54E S9(7)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(10, 5, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal GP2KM54E { get; set; }


            /// <summary>
            /// GP2KM54CP X  
            /// </summary>
            [HisFieldInfoMapping(11, 1)]
            public string GP2KM54CP { get; set; }


            /// <summary>
            /// GP2KM54CD X  
            /// </summary>
            [HisFieldInfoMapping(12, 1)]
            public string GP2KM54CD { get; set; }


            /// <summary>
            /// GP2KM54P S9(7)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(13, 5, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal GP2KM54P { get; set; }


            /// <summary>
            /// GP2KM54D S9(7)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(14, 5, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal GP2KM54D { get; set; }


            /// <summary>
            /// GP2KM11RA 9  
            /// </summary>
            [HisFieldInfoMapping(15, 1, CobolType = CobolType.Unsigned)]
            public short GP2KM11RA { get; set; }


            /// <summary>
            /// GP2KM11RB 9  
            /// </summary>
            [HisFieldInfoMapping(16, 1, CobolType = CobolType.Unsigned)]
            public short GP2KM11RB { get; set; }


            /// <summary>
            /// GP2KM11RC 9  
            /// </summary>
            [HisFieldInfoMapping(17, 1, CobolType = CobolType.Unsigned)]
            public short GP2KM11RC { get; set; }


            /// <summary>
            /// GP2KM11RD 9  
            /// </summary>
            [HisFieldInfoMapping(18, 1, CobolType = CobolType.Unsigned)]
            public short GP2KM11RD { get; set; }


            /// <summary>
            /// GP2KM11RE 9  
            /// </summary>
            [HisFieldInfoMapping(19, 1, CobolType = CobolType.Unsigned)]
            public short GP2KM11RE { get; set; }


            /// <summary>
            /// GP2KM11RF 99  
            /// </summary>
            [HisFieldInfoMapping(20, 2, CobolType = CobolType.Unsigned)]
            public short GP2KM11RF { get; set; }


            /// <summary>
            /// GP2KMRIL X(20)  
            /// </summary>
            [HisFieldInfoMapping(21, 20)]
            public string GP2KMRIL { get; set; }


            /// <summary>
            /// GP2KMRIL2 X(20)  
            /// </summary>
            [HisFieldInfoMapping(22, 20)]
            public string GP2KMRIL2 { get; set; }


            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
