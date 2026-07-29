using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA
{
    public class Residenza : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  TRH-REDSO.
        //02 TRHTIPOR            PIC X VALUE "H".
        //02 TRHELERD  OCCURS 30 TIMES.
        //   03 TRHAAR01         PIC 9999.                              
        //   03 TRHMMR01         PIC 99.
        //   03 TRHSTA01         PIC XXX.
        //02 TRHONERE  OCCURS 8 TIMES.
        //     03 TRH-DECONERE            PIC X(08).
        //     03 TRH-SCADONERE           PIC X(08).
        //     03 TRH-CODGRUP             PIC X(04).
        //     03 TRH-CODSGRUP            PIC X(04).
        //     03 TRH-ANZCON              PIC S9(04) COMP-3.
        //     03 TRH-ONERE               PIC S9(07)V9(04) COMP-3.
        //     03 TRH-CODBENEF            PIC X(02).
        //     03 TRH-ANZBENEF            PIC S9(04) COMP-3.
        //     03 TRH-CODINV              PIC X(02).
        //02 TRH-CESINCUM        PIC 9(6).
        //02 THR-SET-NONVE       PIC 9(3).
        //02 THR-SET-NONVE-P95   PIC 9(3). 
        //02 TRH-NUM-FIGLI       PIC X.
        //02 FILLER              PIC X(3).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  TRH-REDSO.
        /// <summary>
        /// TRHTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string TRHTIPOR { get; set; }

        /// <summary>
        /// TRHELERD  OCCURS 30 TIMES.
        /// </summary>
        [HisComplexAreaInfoMapping(1, ListCount = 30)]
        public List<TRHELERD> LISTTRHELERD { get; set; }

        /// <summary>
        /// TRHONERE  OCCURS 8 TIMES.
        /// <summary>
        [HisComplexAreaInfoMapping(2, ListCount = 8)]
        public List<TRHONERE> LISTTRHONERE { get; set; }

        /// <summary>
        /// TRH-CESINCUM PIC 9(6).
        /// </summary>
        [HisFieldInfoMapping(3, 6)]
        public int TRH_CESINCUM { get; set; }

        /// <summary>
        /// THR-SET-NONVE PIC 9(3).
        /// </summary>
        [HisFieldInfoMapping(4, 3)]
        public int THR_SET_NONVE { get; set; }

        /// <summary>
        /// THR-SET-NONVE-P95 PIC 9(3).
        /// </summary>
        [HisFieldInfoMapping(5, 3)]
        public int THR_SET_NONVE_P95 { get; set; }

        /// <summary>
        /// TRH-NUM-FIGLI PIC X.
        /// </summary>
        [HisFieldInfoMapping(6, 1)]
        public string TRH_NUM_FIGLI { get; set; }

        /// <summary>
        /// FILLER X(4)  
        /// </summary>
        [HisFieldInfoMapping(7, 3)]
        public string FILLER { get; set; }

        #endregion Tracciato Host

        public string TransactionName
        {
            get { return "Residenza"; }
        }
        #endregion Properties

        #region nested class
        public class TRHELERD
        {
            #region Properties

            #region Tracciato COBOL
            //02 TRHELERD  OCCURS 30 TIMES.
            //   03 TRHAAR01         PIC 9999.                              
            //   03 TRHMMR01         PIC 99.
            //   03 TRHSTA01         PIC XXX.
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 TRHELERD  OCCURS 30 TIMES.
            /// <summary>
            /// TRHAAR01 9999  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short TRHAAR01 { get; set; }

            /// <summary>
            /// TRHMMR01 99  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short TRHMMR01 { get; set; }

            /// <summary>
            /// TRHSTA01 XXX  
            /// </summary>
            [HisFieldInfoMapping(2, 3)]
            public string TRHSTA01 { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }

        public class TRHONERE
        {
            #region Properties

            #region Tracciato COBOL
            //02 TRHONERE  OCCURS 8 TIMES.
            // 03 TRH-DECONERE            PIC X(08).
            // 03 TRH-SCADONERE           PIC X(08).
            // 03 TRH-CODGRUP             PIC X(04).
            // 03 TRH-CODSGRUP            PIC X(04).
            // 03 TRH-ANZCON              PIC S9(04) COMP-3.
            // 03 TRH-ONERE               PIC S9(07)V9(04) COMP-3.
            // 03 TRH-CODBENEF            PIC X(02).
            // 03 TRH-ANZBENEF            PIC S9(04) COMP-3.
            // 03 TRH-CODINV              PIC X(02).
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// TRH-DECONERE X(08)  
            /// </summary>
            [HisFieldInfoMapping(0, 8)]
            public string TRH_DECONERE { get; set; }

            /// <summary>
            /// TRH-SCADONERE X(08)  
            /// </summary>
            [HisFieldInfoMapping(1, 8)]
            public string TRH_SCADONERE { get; set; }

            /// <summary>
            /// TRH-CODGRUP X(04)  
            /// </summary>
            [HisFieldInfoMapping(2, 4)]
            public string TRH_CODGRUP { get; set; }

            /// <summary>
            /// TRH-CODSGRUP X(04) 
            /// </summary>
            [HisFieldInfoMapping(3, 4)]
            public string TRH_CODSGRUP { get; set; }

            /// <summary>
            /// TRH-ANZCON S9(04) COMP-3
            /// </summary>
            [HisFieldInfoMapping(4, 3, CobolType = CobolType.Comp3)]
            public int TRH_ANZCON { get; set; }

            /// <summary>
            /// TRH-ONERE S9(07)V9(04) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(5, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal TRH_ONERE { get; set; }

            /// <summary>
            /// TRH-CODBENEF X(02)  
            /// </summary>
            [HisFieldInfoMapping(6, 2)]
            public string TRH_CODBENEF { get; set; }

            /// <summary>
            /// TRH-ANZBENEF S9(04) COMP-3
            /// </summary>
            [HisFieldInfoMapping(7, 3, CobolType = CobolType.Comp3)]
            public int TRH_ANZBENEF { get; set; }

            /// <summary>
            /// TRH-CODINV X(02)  
            /// </summary>
            [HisFieldInfoMapping(8, 2)]
            public string TRH_CODINV { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class
    }
}