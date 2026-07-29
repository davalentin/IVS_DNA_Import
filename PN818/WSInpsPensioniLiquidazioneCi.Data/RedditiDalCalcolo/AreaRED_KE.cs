using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIRED4
{
    public class AreaRED_KE
    {
        #region Constructor
        internal AreaRED_KE()
        {
        }
        #endregion Constructor

        #region tracciato COBOL
        //************************************REDDITI X T.M.                       
        //2011       02  GP2KE00  OCCURS 50.                                       
        //               03  GP2KE09Z                         PIC 9999.            
        //               03  GP2KE10E                     PIC S9(7)V9(2) COMP-3.   
        //2011           03  GP2KE10CP                    PIC X.                   
        //2011           03  GP2KE10CD                    PIC X.                   
        //2011           03  GP2KE10P                     PIC S9(7)V9(2) COMP-3.   
        //2011           03  GP2KE10D                     PIC S9(7)V9(2) COMP-3.   
        //               03  GP2KE11                          PIC 9(7).            
        //               03  GP2KE11R  REDEFINES GP2KE11.                          
        //                   04  GP2KE11A                     PIC 9.               
        //                   04  GP2KE11B                     PIC 99.              
        //                   04  GP2KE11C                     PIC 99.              
        //                   04  GP2KE11D                     PIC 99.              
        //               03  GP2KE11R1 REDEFINES GP2KE11.                          
        //                   04  GP2KE11R1A                   PIC 9.               
        //                   04  GP2KE11R1B                   PIC 9.               
        //                   04  GP2KE11R1C                   PIC 9.               
        //                   04  GP2KE11R1D                   PIC 9.               
        //                   04  GP2KE11R1E                   PIC 9.               
        //                   04  GP2KE11R1F                   PIC 99.              
        //               03  GP2KE12E                     PIC S9(7)V9(2) COMP-3.   
        //2011           03  GP2KE12CP                    PIC X.                   
        //2011           03  GP2KE12CD                    PIC X.                   
        //2011           03  GP2KE12P                     PIC S9(7)V9(2) COMP-3.   
        //2011           03  GP2KE12D                     PIC S9(7)V9(2) COMP-3.   
        //               03  GP2KE13E                     PIC S9(7)V9(2) COMP-3.   
        //2011           03  GP2KE13CP                    PIC X.                   
        //2011           03  GP2KE13CD                    PIC X.                   
        //2011           03  GP2KE13P                     PIC S9(7)V9(2) COMP-3.   
        //2011           03  GP2KE13D                     PIC S9(7)V9(2) COMP-3.   
        //2005           03  GP2KERIL                         PIC X(20).           
        //2010           03  GP2KERIL2                        PIC X(20).
        #endregion tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 50)]
        public List<Gp2ke00> LISTAGP2KE00 { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class Gp2ke00
        {
            #region Constructor
            internal Gp2ke00()
            {
            }
            #endregion Constructor

            #region tracciato COBOL
            //************************************REDDITI X T.M.                       
            //2011       02  GP2KE00  OCCURS 50.                                       
            //               03  GP2KE09Z                         PIC 9999.            
            //               03  GP2KE10E                     PIC S9(7)V9(2) COMP-3.   
            //2011           03  GP2KE10CP                    PIC X.                   
            //2011           03  GP2KE10CD                    PIC X.                   
            //2011           03  GP2KE10P                     PIC S9(7)V9(2) COMP-3.   
            //2011           03  GP2KE10D                     PIC S9(7)V9(2) COMP-3.   
            //               03  GP2KE11                          PIC 9(7).            
            //               03  GP2KE11R  REDEFINES GP2KE11.                          
            //                   04  GP2KE11A                     PIC 9.               
            //                   04  GP2KE11B                     PIC 99.              
            //                   04  GP2KE11C                     PIC 99.              
            //                   04  GP2KE11D                     PIC 99.              
            //               03  GP2KE11R1 REDEFINES GP2KE11.                          
            //                   04  GP2KE11R1A                   PIC 9.               
            //                   04  GP2KE11R1B                   PIC 9.               
            //                   04  GP2KE11R1C                   PIC 9.               
            //                   04  GP2KE11R1D                   PIC 9.               
            //                   04  GP2KE11R1E                   PIC 9.               
            //                   04  GP2KE11R1F                   PIC 99.              
            //               03  GP2KE12E                     PIC S9(7)V9(2) COMP-3.   
            //2011           03  GP2KE12CP                    PIC X.                   
            //2011           03  GP2KE12CD                    PIC X.                   
            //2011           03  GP2KE12P                     PIC S9(7)V9(2) COMP-3.   
            //2011           03  GP2KE12D                     PIC S9(7)V9(2) COMP-3.   
            //               03  GP2KE13E                     PIC S9(7)V9(2) COMP-3.   
            //2011           03  GP2KE13CP                    PIC X.                   
            //2011           03  GP2KE13CD                    PIC X.                   
            //2011           03  GP2KE13P                     PIC S9(7)V9(2) COMP-3.   
            //2011           03  GP2KE13D                     PIC S9(7)V9(2) COMP-3.   
            //2005           03  GP2KERIL                         PIC X(20).           
            //2010           03  GP2KERIL2                        PIC X(20).
            #endregion tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP2KE09Z 9999  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short GP2KE09Z { get; set; }


            /// <summary>
            /// GP2KE10E S9(7)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(1, 5, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal GP2KE10E { get; set; }


            /// <summary>
            /// GP2KE10CP X  
            /// </summary>
            [HisFieldInfoMapping(2, 1)]
            public string GP2KE10CP { get; set; }


            /// <summary>
            /// GP2KE10CD X  
            /// </summary>
            [HisFieldInfoMapping(3, 1)]
            public string GP2KE10CD { get; set; }


            /// <summary>
            /// GP2KE10P S9(7)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(4, 5, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal GP2KE10P { get; set; }


            /// <summary>
            /// GP2KE10D S9(7)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(5, 5, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal GP2KE10D { get; set; }


            /// <summary>
            /// GP2KE11R1A 9  
            /// </summary>
            [HisFieldInfoMapping(6, 1, CobolType = CobolType.Unsigned)]
            public short GP2KE11R1A { get; set; }


            /// <summary>
            /// GP2KE11R1B 9  
            /// </summary>
            [HisFieldInfoMapping(7, 1, CobolType = CobolType.Unsigned)]
            public short GP2KE11R1B { get; set; }


            /// <summary>
            /// GP2KE11R1C 9  
            /// </summary>
            [HisFieldInfoMapping(8, 1, CobolType = CobolType.Unsigned)]
            public short GP2KE11R1C { get; set; }


            /// <summary>
            /// GP2KE11R1D 9  
            /// </summary>
            [HisFieldInfoMapping(9, 1, CobolType = CobolType.Unsigned)]
            public short GP2KE11R1D { get; set; }


            /// <summary>
            /// GP2KE11R1E 9  
            /// </summary>
            [HisFieldInfoMapping(10, 1, CobolType = CobolType.Unsigned)]
            public short GP2KE11R1E { get; set; }


            /// <summary>
            /// GP2KE11R1F 99  
            /// </summary>
            [HisFieldInfoMapping(11, 2, CobolType = CobolType.Unsigned)]
            public short GP2KE11R1F { get; set; }


            /// <summary>
            /// GP2KE12E S9(7)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(12, 5, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal GP2KE12E { get; set; }


            /// <summary>
            /// GP2KE12CP X  
            /// </summary>
            [HisFieldInfoMapping(13, 1)]
            public string GP2KE12CP { get; set; }


            /// <summary>
            /// GP2KE12CD X  
            /// </summary>
            [HisFieldInfoMapping(14, 1)]
            public string GP2KE12CD { get; set; }


            /// <summary>
            /// GP2KE12P S9(7)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(15, 5, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal GP2KE12P { get; set; }


            /// <summary>
            /// GP2KE12D S9(7)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(16, 5, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal GP2KE12D { get; set; }


            /// <summary>
            /// GP2KE13E S9(7)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(17, 5, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal GP2KE13E { get; set; }


            /// <summary>
            /// GP2KE13CP X  
            /// </summary>
            [HisFieldInfoMapping(18, 1)]
            public string GP2KE13CP { get; set; }


            /// <summary>
            /// GP2KE13CD X  
            /// </summary>
            [HisFieldInfoMapping(19, 1)]
            public string GP2KE13CD { get; set; }


            /// <summary>
            /// GP2KE13P S9(7)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(20, 5, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal GP2KE13P { get; set; }


            /// <summary>
            /// GP2KE13D S9(7)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(21, 5, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal GP2KE13D { get; set; }


            /// <summary>
            /// GP2KERIL X(20)  
            /// </summary>
            [HisFieldInfoMapping(22, 20)]
            public string GP2KERIL { get; set; }


            /// <summary>
            /// GP2KERIL2 X(20)  
            /// </summary>
            [HisFieldInfoMapping(23, 20)]
            public string GP2KERIL2 { get; set; }

            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
