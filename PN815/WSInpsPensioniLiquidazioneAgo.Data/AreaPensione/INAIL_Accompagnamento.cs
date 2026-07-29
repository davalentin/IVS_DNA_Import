using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class INAIL_Accompagnamento
    {
        #region Properties

        #region Tracciato COBOL
        //   *DATI DEL PANNELLO MRCINA0 (DATI INAIL E ACCOMPAGNAMENTO)
        //02 T-GPINA0.
        //   03 T-GP2BINA  OCCURS 45.
        //      04 T-GP2BIN1.
        //         05 T-GP2BIN1A       PIC 9(4).
        //         05 T-GP2BIN1M       PIC 9(2).
        //      04 T-GP2BIN2           PIC S9(7)V9(4) COMP-3.
        //      04 T-GP2BIN3           PIC 9.
        //   03 T-GP2BACC.
        //      04 T-GP2BACCA          PIC 9(4).
        //      04 T-GP2BACCM          PIC 9(2).
        //   03 T-GP5KM21              PIC S9(5)V9(4) COMP-3.
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 45)]
        public List<T_GP2BINA> LISTT_GP2BINA { get; set; }

        // 03 T-GP2BACC.
        /// <summary>
        /// T_GP2BACCA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(1, 4, CobolType = CobolType.Unsigned)]
        public short T_GP2BACCA { get; set; }

        /// <summary>
        /// T_GP2BACCM 9(2)  
        /// </summary>
        [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
        public short T_GP2BACCM { get; set; }

        /// <summary>
        /// T_GP5KM21 S9(5)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(3, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal T_GP5KM21 { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class T_GP2BINA
        {
            #region Properties

            #region Tracciato COBOL
            //   *DATI DEL PANNELLO MRCINA0 (DATI INAIL E ACCOMPAGNAMENTO)
            //02 T-GPINA0.
            //   03 T-GP2BINA  OCCURS 45.
            //      04 T-GP2BIN1.
            //         05 T-GP2BIN1A       PIC 9(4).
            //         05 T-GP2BIN1M       PIC 9(2).
            //      04 T-GP2BIN2           PIC S9(7)V9(4) COMP-3.
            //      04 T-GP2BIN3           PIC 9.
            #endregion Tracciato COBOL

            #region Tracciato Host
            // *DATI DEL PANNELLO MRCINA0 (DATI INAIL E ACCOMPAGNAMENTO)
            // 02 T-GPINA0.
            // 03 T-GP2BINA  OCCURS 45.
            // 04 T-GP2BIN1.
            /// <summary>
            /// T_GP2BIN1A 9(4)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BIN1A { get; set; }

            /// <summary>
            /// T_GP2BIN1M 9(2)  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BIN1M { get; set; }

            /// <summary>
            /// T_GP2BIN2 S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(2, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal T_GP2BIN2 { get; set; }

            /// <summary>
            /// T_GP2BIN3 9  
            /// </summary>
            [HisFieldInfoMapping(3, 1, CobolType = CobolType.Unsigned)]
            public short T_GP2BIN3 { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
