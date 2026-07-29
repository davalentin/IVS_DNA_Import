using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class DatiRetributivi_Contributivi
    {
        #region Properties

        #region Tracciato COBOL
        //   *DATI DEL PANNELLO MRCRET0 (DATI RETRIBUTIVI-CONTRIBUTIVI)
        //02 T-GPRET0.
        //   03 T-GP2BC00 OCCURS 30.
        //      04 T-GP2BC01.
        //         05 T-GP2BC01A       PIC 9(4).
        //         05 T-GP2BC01M       PIC 9(2).
        //      04 T-GP2BC02           PIC S9(5) COMP-3.
        //      04 T-GP2BC03           PIC S9(7)V9(6) COMP-3.
        //      04 T-GP2BC04           PIC S9(5) COMP-3.
        //      04 T-GP2BC05           PIC S9(5)V9(6) COMP-3.
        //      04 T-GP2BC08           PIC S9(5) COMP-3.
        //      04 T-GP2BC09           PIC X(2).
        //      04 T-GP2BC10           PIC S9(5) COMP-3.
        //      04 T-GP2BC0A           PIC 9.
        //      04 T-GP2BC0B           PIC X.
        //      04 T-GP2BC0C           PIC X.
        //      04 T-GP2BC0D           PIC S9(7)V9(4) COMP-3.
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 30)]
        public List<T_GP2BC00> LISTT_GP2BC00 { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class T_GP2BC00
        {
            #region Properties

            #region Tracciato COBOL
            //   *DATI DEL PANNELLO MRCRET0 (DATI RETRIBUTIVI-CONTRIBUTIVI)
            //02 T-GPRET0.
            //   03 T-GP2BC00 OCCURS 30.
            //      04 T-GP2BC01.
            //         05 T-GP2BC01A       PIC 9(4).
            //         05 T-GP2BC01M       PIC 9(2).
            //      04 T-GP2BC02           PIC S9(5) COMP-3.
            //      04 T-GP2BC03           PIC S9(7)V9(6) COMP-3.
            //      04 T-GP2BC04           PIC S9(5) COMP-3.
            //      04 T-GP2BC05           PIC S9(5)V9(6) COMP-3.
            //      04 T-GP2BC08           PIC S9(5) COMP-3.
            //      04 T-GP2BC09           PIC X(2).
            //      04 T-GP2BC10           PIC S9(5) COMP-3.
            //      04 T-GP2BC0A           PIC 9.
            //      04 T-GP2BC0B           PIC X.
            //      04 T-GP2BC0C           PIC X.
            //      04 T-GP2BC0D           PIC S9(7)V9(4) COMP-3.
            #endregion Tracciato COBOL

            #region Tracciato Host
            // *DATI DEL PANNELLO MRCRET0 (DATI RETRIBUTIVI-CONTRIBUTIVI)
            // 02 T-GPRET0.
            // 03 T-GP2BC00 OCCURS 30.
            // 04 T-GP2BC01.
            /// <summary>
            /// T_GP2BC01A 9(4)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BC01A { get; set; }

            /// <summary>
            /// T_GP2BC01M 9(2)  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BC01M { get; set; }

            /// <summary>
            /// T_GP2BC02 S9(5) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(2, 3, CobolType = CobolType.Comp3)]
            public int T_GP2BC02 { get; set; }

            /// <summary>
            /// T_GP2BC03 S9(7)V9(6) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(3, 7, Scale = 6, CobolType = CobolType.Comp3)]
            public decimal T_GP2BC03 { get; set; }

            /// <summary>
            /// T_GP2BC04 S9(5) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(4, 3, CobolType = CobolType.Comp3)]
            public int T_GP2BC04 { get; set; }

            /// <summary>
            /// T_GP2BC05 S9(5)V9(6) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(5, 6, Scale = 6, CobolType = CobolType.Comp3)]
            public decimal T_GP2BC05 { get; set; }

            /// <summary>
            /// T_GP2BC08 S9(5) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(6, 3, CobolType = CobolType.Comp3)]
            public int T_GP2BC08 { get; set; }

            /// <summary>
            /// T_GP2BC09 X(2)  
            /// </summary>
            [HisFieldInfoMapping(7, 2)]
            public string T_GP2BC09 { get; set; }

            /// <summary>
            /// T_GP2BC10 S9(5) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(8, 3, CobolType = CobolType.Comp3)]
            public int T_GP2BC10 { get; set; }

            /// <summary>
            /// T_GP2BC0A 9  
            /// </summary>
            [HisFieldInfoMapping(9, 1, CobolType = CobolType.Unsigned)]
            public short T_GP2BC0A { get; set; }

            /// <summary>
            /// T_GP2BC0B X
            /// <summary>
            [HisFieldInfoMapping(10, 1)]
            public string T_GP2BC0B { get; set; }

            /// <summary>
            /// T_GP2BC0C X
            /// <summary>
            [HisFieldInfoMapping(11, 1)]
            public string T_GP2BC0C { get; set; }

            /// <summary>
            /// T_GP2BC0D S9(7)V9(4) COMP-3
            /// <summary>
            [HisFieldInfoMapping(12, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal T_GP2BC0D { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
