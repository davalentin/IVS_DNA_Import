using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class PannelloContributivo
    {
        #region Properties

        #region Tracciato COBOL
        //        *DATI DEL PANNELLO CONTRIBUTIVO
        //     02 T-GPCTR0.
        //*LOMAR 03/11/2010 - I e F    
        //        03 T-GP2BB03 OCCURS 20.
        //           04 T-GP2BB04.
        //              05 T-GP2BB04A       PIC 9(4).
        //              05 T-GP2BB04M       PIC 9(2).
        //              05 T-GP2BB04G       PIC 9(2).
        //           04 T-GP2BB05           PIC XX.
        //           04 T-GP2BB06           PIC S9(8)V9(7) COMP-3.
        //           04 T-GP2BB07           PIC S9(9)V9(4) COMP-3.
        //           04 T-GP2BB08           PIC 9(5).
        //           04 T-GP2BB09           PIC S9(9)V9(4) COMP-3.
        //           04 T-GP2BB0A           PIC X.
        //           04 T-GP2BB0B           PIC X.
        //           04 T-GP2BB0C           PIC X.
        //           04 T-GP2BB0D           PIC S9(7)V9(4) COMP-3.
        //        03 T-GP1AF04              PIC S9(5)V9(4) COMP-3.
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 20)]
        public List<T_GP2BB03> LISTT_GP2BB03 { get; set; }

        /// <summary>
        /// T_GP1AF04 S9(5)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(1, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal T_GP1AF04 { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class T_GP2BB03
        {
            #region Properties

            #region Tracciato COBOL
            //        *DATI DEL PANNELLO CONTRIBUTIVO
            //     02 T-GPCTR0.
            //*LOMAR 03/11/2010 - I e F    
            //        03 T-GP2BB03 OCCURS 20.
            //           04 T-GP2BB04.
            //              05 T-GP2BB04A       PIC 9(4).
            //              05 T-GP2BB04M       PIC 9(2).
            //              05 T-GP2BB04G       PIC 9(2).
            //           04 T-GP2BB05           PIC XX.
            //           04 T-GP2BB06           PIC S9(8)V9(7) COMP-3.
            //           04 T-GP2BB07           PIC S9(9)V9(4) COMP-3.
            //           04 T-GP2BB08           PIC 9(5).
            //           04 T-GP2BB09           PIC S9(9)V9(4) COMP-3.
            //           04 T-GP2BB0A           PIC X.
            //           04 T-GP2BB0B           PIC X.
            //           04 T-GP2BB0C           PIC X.
            //           04 T-GP2BB0D           PIC S9(7)V9(4) COMP-3.
            #endregion Tracciato COBOL

            #region Tracciato Host
            // *DATI DEL PANNELLO CONTRIBUTIVO
            // 02 T-GPCTR0.
            // *LOMAR 03/11/2010 - I e F
            // 03 T-GP2BB03 OCCURS 20.
            // 04 T-GP2BB04.
            /// <summary>
            /// T_GP2BB04A 9(4)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BB04A { get; set; }

            /// <summary>
            /// T_GP2BB04M 9(2)  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BB04M { get; set; }

            /// <summary>
            /// T_GP2BB04G 9(2)  
            /// </summary>
            [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BB04G { get; set; }

            /// <summary>
            /// T_GP2BB05 XX  
            /// </summary>
            [HisFieldInfoMapping(3, 2)]
            public string T_GP2BB05 { get; set; }

            /// <summary>
            /// T_GP2BB06 S9(8)V9(7) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(4, 8, Scale = 7, CobolType = CobolType.Comp3)]
            public decimal T_GP2BB06 { get; set; }

            /// <summary>
            /// T_GP2BB07 S9(9)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(5, 7, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal T_GP2BB07 { get; set; }

            /// <summary>
            /// T_GP2BB08 9(5)  
            /// </summary>
            [HisFieldInfoMapping(6, 5, CobolType = CobolType.Unsigned)]
            public int T_GP2BB08 { get; set; }

            /// <summary>
            /// T_GP2BB09 S9(9)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(7, 7, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal T_GP2BB09 { get; set; }

            /// <summary>
            /// T_GP2BB0A X
            /// <summary>
            [HisFieldInfoMapping(8, 1)]
            public string T_GP2BB0A { get; set; }

            /// <summary>
            /// T_GP2BB0B X
            /// <summary>
            [HisFieldInfoMapping(9, 1)]
            public string T_GP2BB0B { get; set; }

            /// <summary>
            /// T_GP2BB0C X
            /// <summary>
            [HisFieldInfoMapping(10, 1)]
            public string T_GP2BB0C { get; set; }

            /// <summary>
            /// T_GP2BB0D S9(7)V9(4) COMP-3
            /// <summary>
            [HisFieldInfoMapping(11, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal T_GP2BB0D { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
