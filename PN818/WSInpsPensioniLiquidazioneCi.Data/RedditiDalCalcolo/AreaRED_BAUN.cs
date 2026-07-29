using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIRED4
{
    public class AreaRED_BAUN
    {
        #region Constructor
        internal AreaRED_BAUN()
        {
        }
        #endregion Constructor

        #region tracciato COBOL
        //       ************************************REDDITI X LAVORO AUTONOMO            
        //2011       02  GP2BAUN     OCCURS 50 TIMES.                              
        //               03  GP2BAU1                          PIC 9999.            
        //               03  GP2BAU2E                   PIC S9(7)V9(2) COMP-3.     
        //               03  GP2BAU3                          PIC 9999.            
        //               03  GP2BAU3R REDEFINES GP2BAU3.                           
        //                  04  GP2BAU3-M1                    PIC 99.              
        //                  04  GP2BAU3-M2                    PIC 99.              
        //2004R          03  GP2BAU11                         PIC 9(7).            
        //2004R          03  GP2BAU11R REDEFINES GP2BAU11.                         
        //2004R              04  GP2BAU11A                     PIC 9.              
        //2004R              04  GP2BAU11B                     PIC 99.             
        //2004R              04  GP2BAU11C                     PIC 99.             
        //2004R              04  GP2BAU11D                     PIC 99.             
        //2004R          03  GP2BAU11R1 REDEFINES GP2BAU11.                        
        //2004R              04  GP2BAU11RA                    PIC 9.              
        //2004R              04  GP2BAU11RB                    PIC 9.              
        //2004R              04  GP2BAU11RC                    PIC 9.              
        //2004R              04  GP2BAU11RD                    PIC 9.              
        //2004R              04  GP2BAU11RE                    PIC 9.              
        //2004R              04  GP2BAU11RF                    PIC 99.             
        //2005           03  GP2BARIL                         PIC X(20).           
        //2010           03  GP2BARIL2                        PIC X(20). 
        #endregion tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 50)]
        public List<Gp2baun> LISTAGP2BAUN { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class Gp2baun
        {
            #region Constructor
            internal Gp2baun()
            {
            }
            #endregion Constructor

            #region tracciato COBOL
            //           ************************************REDDITI X LAVORO AUTONOMO            
            //2011       02  GP2BAUN     OCCURS 50 TIMES.                              
            //               03  GP2BAU1                          PIC 9999.            
            //               03  GP2BAU2E                   PIC S9(7)V9(2) COMP-3.     
            //               03  GP2BAU3                          PIC 9999.            
            //               03  GP2BAU3R REDEFINES GP2BAU3.                           
            //                  04  GP2BAU3-M1                    PIC 99.              
            //                  04  GP2BAU3-M2                    PIC 99.              
            //2004R          03  GP2BAU11                         PIC 9(7).            
            //2004R          03  GP2BAU11R REDEFINES GP2BAU11.                         
            //2004R              04  GP2BAU11A                     PIC 9.              
            //2004R              04  GP2BAU11B                     PIC 99.             
            //2004R              04  GP2BAU11C                     PIC 99.             
            //2004R              04  GP2BAU11D                     PIC 99.             
            //2004R          03  GP2BAU11R1 REDEFINES GP2BAU11.                        
            //2004R              04  GP2BAU11RA                    PIC 9.              
            //2004R              04  GP2BAU11RB                    PIC 9.              
            //2004R              04  GP2BAU11RC                    PIC 9.              
            //2004R              04  GP2BAU11RD                    PIC 9.              
            //2004R              04  GP2BAU11RE                    PIC 9.              
            //2004R              04  GP2BAU11RF                    PIC 99.             
            //2005           03  GP2BARIL                         PIC X(20).           
            //2010           03  GP2BARIL2                        PIC X(20). 
            #endregion tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP2BAU1 9999  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short GP2BAU1 { get; set; }


            /// <summary>
            /// GP2BAU2E S9(7)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(1, 5, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal GP2BAU2E { get; set; }


            /// <summary>
            /// GP2BAU3_M1 99  
            /// </summary>
            [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
            public short GP2BAU3_M1 { get; set; }


            /// <summary>
            /// GP2BAU3_M2 99  
            /// </summary>
            [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
            public short GP2BAU3_M2 { get; set; }


            /// <summary>
            /// GP2BAU11RA 9  
            /// </summary>
            [HisFieldInfoMapping(4, 1, CobolType = CobolType.Unsigned)]
            public short GP2BAU11RA { get; set; }


            /// <summary>
            /// GP2BAU11RB 9  
            /// </summary>
            [HisFieldInfoMapping(5, 1, CobolType = CobolType.Unsigned)]
            public short GP2BAU11RB { get; set; }


            /// <summary>
            /// GP2BAU11RC 9  
            /// </summary>
            [HisFieldInfoMapping(6, 1, CobolType = CobolType.Unsigned)]
            public short GP2BAU11RC { get; set; }


            /// <summary>
            /// GP2BAU11RD 9  
            /// </summary>
            [HisFieldInfoMapping(7, 1, CobolType = CobolType.Unsigned)]
            public short GP2BAU11RD { get; set; }


            /// <summary>
            /// GP2BAU11RE 9  
            /// </summary>
            [HisFieldInfoMapping(8, 1, CobolType = CobolType.Unsigned)]
            public short GP2BAU11RE { get; set; }


            /// <summary>
            /// GP2BAU11RF 99  
            /// </summary>
            [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
            public short GP2BAU11RF { get; set; }


            /// <summary>
            /// GP2BARIL X(20)  
            /// </summary>
            [HisFieldInfoMapping(10, 20)]
            public string GP2BARIL { get; set; }


            /// <summary>
            /// GP2BARIL2 X(20)  
            /// </summary>
            [HisFieldInfoMapping(11, 20)]
            public string GP2BARIL2 { get; set; }

            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
