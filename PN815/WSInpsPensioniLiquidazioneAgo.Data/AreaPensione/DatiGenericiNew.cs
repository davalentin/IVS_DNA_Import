using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class DatiGenericiNew
    {
        #region Properties

        #region Tracciato COBOL
        //     02 T-TP1AV1.
        //03 T-GP1AB01-V            PIC X(3).
        //03 T-GP1AB02-V            PIC 9(4).
        //03 T-GP1AB03-V            PIC 9(8).
        //03 T-TP1CAT8-V            PIC X(6).
        //03 T-TP1COP-V             PIC 9(2).
        //03 T-TP1IS                PIC 9(2).
        //03 T-GP1AT22              PIC X(4).
        //03 FILLER                 PIC X(8).
        //03 T-TP1ELABR.
        //   04 T-TP1ELABG          PIC 9(2).
        //   04 T-TP1ELABM          PIC 9(2).
        //   04 T-TP1ELABA          PIC 9(4).
        //03 T-TP1ACQ.
        //   04 T-TP1MATRICOLA      PIC 9(8).
        //   04 T-TP1DATACQ.
        //      05 T-TP1DATACQA     PIC 9(4).
        //      05 T-TP1DATACQM     PIC 9(2).
        //      05 T-TP1DATACQG     PIC 9(2).
        //03 T-INCALC               PIC X.
        //03 T-VERS                 PIC X.
        //03 T-CODPRO               PIC XX.
        //03 T-NDOMUS               PIC 9(13).
        //03 T-GP1AV91M             PIC 9.
        //03 T-GP1ALA2              PIC 9.
        //03 T-GP1ALA3              PIC 9.
        //03 T-QRED                 PIC X.
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 02 T-TP1AV1.
        /// <summary>
        /// T_GP1AB01_V X(3)  
        /// </summary>
        [HisFieldInfoMapping(0, 3)]
        public string T_GP1AB01_V { get; set; }

        /// <summary>
        /// T_GP1AB02_V 9(4)  
        /// </summary>
        [HisFieldInfoMapping(1, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1AB02_V { get; set; }

        /// <summary>
        /// T_GP1AB03_V 9(8)  
        /// </summary>
        [HisFieldInfoMapping(2, 8, CobolType = CobolType.Unsigned)]
        public int T_GP1AB03_V { get; set; }

        /// <summary>
        /// T_TP1CAT8_V X(6)  
        /// </summary>
        [HisFieldInfoMapping(3, 6)]
        public string T_TP1CAT8_V { get; set; }

        /// <summary>
        /// T_TP1COP_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
        public short T_TP1COP_V { get; set; }

        /// <summary>
        /// T_TP1IS 9(2)  
        /// </summary>
        [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
        public short T_TP1IS { get; set; }

        /// <summary>
        /// T_GP1AT22 X(4)  
        /// </summary>
        [HisFieldInfoMapping(6, 4)]
        public string T_GP1AT22 { get; set; }

        /// <summary>
        /// T_GP1AT22 X(4)  
        /// </summary>
        [HisFieldInfoMapping(7, 1)]
        public string T_GP1AJSP { get; set; }

        /// <summary>
        /// FILLER X(4)  
        /// </summary>
        [HisFieldInfoMapping(8, 3)]
        public string FILLER { get; set; }

        // 03 T-TP1ELABR.
        /// <summary>
        /// T_TP1ELABG 9(2)  
        /// </summary>
        [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
        public short T_TP1ELABG { get; set; }

        /// <summary>
        /// T_TP1ELABM 9(2)  
        /// </summary>
        [HisFieldInfoMapping(10, 2, CobolType = CobolType.Unsigned)]
        public short T_TP1ELABM { get; set; }

        /// <summary>
        /// T_TP1ELABA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(11, 4, CobolType = CobolType.Unsigned)]
        public short T_TP1ELABA { get; set; }

        // 03 T-TP1ACQ.
        /// <summary>
        /// T_TP1MATRICOLA 9(8)  
        /// </summary>
        [HisFieldInfoMapping(12, 8, CobolType = CobolType.Unsigned)]
        public int T_TP1MATRICOLA { get; set; }

        // 04 T-TP1DATACQ.
        /// <summary>
        /// T_TP1DATACQA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(13, 4, CobolType = CobolType.Unsigned)]
        public short T_TP1DATACQA { get; set; }

        /// <summary>
        /// T_TP1DATACQM 9(2)  
        /// </summary>
        [HisFieldInfoMapping(14, 2, CobolType = CobolType.Unsigned)]
        public short T_TP1DATACQM { get; set; }

        /// <summary>
        /// T_TP1DATACQG 9(2)  
        /// </summary>
        [HisFieldInfoMapping(15, 2, CobolType = CobolType.Unsigned)]
        public short T_TP1DATACQG { get; set; }

        /// <summary>
        /// T_INCALC X  
        /// </summary>
        [HisFieldInfoMapping(16, 1)]
        public string T_INCALC { get; set; }

        /// <summary>
        /// T_VERS X  
        /// </summary>
        [HisFieldInfoMapping(17, 1)]
        public string T_VERS { get; set; }

        /// <summary>
        /// T_CODPRO XX  
        /// </summary>
        [HisFieldInfoMapping(18, 2)]
        public string T_CODPRO { get; set; }

        /// <summary>
        /// T_NDOMUS 9(13).
        /// <summary>
        [HisFieldInfoMapping(19, 13, CobolType = CobolType.Unsigned)]
        public long T_NDOMUS { get; set; }

        /// <summary>
        /// T_GP1AV91M 9  
        /// </summary>
        [HisFieldInfoMapping(20, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1AV91M { get; set; }

        /// <summary>
        /// T_GP1ALA2 9  
        /// </summary>
        [HisFieldInfoMapping(21, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1ALA2 { get; set; }

        /// <summary>
        /// T_GP1ALA3 9  
        /// </summary>
        [HisFieldInfoMapping(22, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1ALA3 { get; set; }

        /// <summary>
        /// T_QRED X  
        /// </summary>
        [HisFieldInfoMapping(23, 1)]
        public string T_QRED { get; set; }
        #endregion Tracciato Host

        #endregion Properties
    }
}