using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class DanteCausa
    {
        #region Properties

        #region Tracciato COBOL
        //   *DATI DEL PANNELLO MRCAN40 (DANTE CAUSA)
        //02 T-GPAN40.
        //   03 T-TP1DANTEC.
        //      04 T-GP7LC11           PIC X(32).
        //      04 T-GP7LC21           PIC X(32).
        //      04 T-GP7LC31           PIC X.
        //      04 T-GP7LC41.
        //         05 T-GP7LC41G       PIC 9(2).
        //         05 T-GP7LC41M       PIC 9(2).
        //         05 T-GP7LC41A       PIC 9(4).
        //      04 T-GP7LC01           PIC X(16).
        //      04 T-GP7LC51           PIC 9(5).
        //      04 T-GP7LC03.
        //         05 T-GP7LC03G       PIC 9(2).
        //         05 T-GP7LC03M       PIC 9(2).
        //         05 T-GP7LC03A       PIC 9(4).
        //      04 T-GP7LC04           PIC 9.
        //   03 T-TP1DATIDIR.
        //      04 T-GP7LC02.
        //         05 T-GP7LC02A       PIC 9(4).
        //         05 T-GP7LC02M       PIC 9(2).
        //      04 T-GP7LB01           PIC 9(3).
        //      04 T-GP7LH01           PIC X(3).
        //      04 T-GP7LB02           PIC 9(4).
        //      04 T-GP7LB03           PIC 9(8).
        //      04 T-GP7LC09.
        //         05 T-GP7LC19        PIC X.
        //         05 T-GP7LC29        PIC 9.
        //         05 T-GP7LC39        PIC 9.
        //      04 T-TP1CONTDIR.
        //         05 T-GP7LE01-V      PIC S9(5)V9(4) COMP-3.
        //         05 T-GP7LE02-V      PIC S9(7)V9(4) COMP-3.
        //         05 T-GP7LE03-V      PIC S9(7)V9(4) COMP-3.
        //         05 T-GP7LE04        PIC S9(5) COMP-3.
        //   03 T-TP1APDC.
        //      04 T-GP7LCAT           PIC X(3).
        //      04 T-GP7LENT           PIC 9(4).
        //      04 T-GP7LCUC           PIC X.
        //      04 T-GP7LCIM           PIC X.
        //      04 T-GP7LACQ.
        //         05 T-GP7LACQA       PIC 9(4).
        //         05 T-GP7LACQM       PIC 9(2).
        //      04 T-GP7LCES.
        //         05 T-GP7LCESA       PIC 9(4).
        //         05 T-GP7LCESM       PIC 9(2).
        //      04 T-GP7LNPE           PIC X(3).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // *DATI DEL PANNELLO MRCAN40 (DANTE CAUSA)
        // 02 T-GPAN40.
        // 03 T-TP1DANTEC.
        /// <summary>
        /// T_GP7LC11 X(32)  
        /// </summary>
        [HisFieldInfoMapping(0, 32)]
        public string T_GP7LC11 { get; set; }

        /// <summary>
        /// T_GP7LC21 X(32)  
        /// </summary>
        [HisFieldInfoMapping(1, 32)]
        public string T_GP7LC21 { get; set; }

        /// <summary>
        /// T_GP7LC31 X  
        /// </summary>
        [HisFieldInfoMapping(2, 1)]
        public string T_GP7LC31 { get; set; }

        // 04 T-GP7LC41.
        /// <summary>
        /// T_GP7LC41G 9(2)  
        /// </summary>
        [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
        public short T_GP7LC41G { get; set; }

        /// <summary>
        /// T_GP7LC41M 9(2)  
        /// </summary>
        [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
        public short T_GP7LC41M { get; set; }

        /// <summary>
        /// T_GP7LC41A 9(4)  
        /// </summary>
        [HisFieldInfoMapping(5, 4, CobolType = CobolType.Unsigned)]
        public short T_GP7LC41A { get; set; }

        /// <summary>
        /// T_GP7LC01 X(16)  
        /// </summary>
        [HisFieldInfoMapping(6, 16)]
        public string T_GP7LC01 { get; set; }

        /// <summary>
        /// T_GP7LC51 9(5)  
        /// </summary>
        [HisFieldInfoMapping(7, 5, CobolType = CobolType.Unsigned)]
        public int T_GP7LC51 { get; set; }

        // 04 T-GP7LC03.
        /// <summary>
        /// T_GP7LC03G 9(2)  
        /// </summary>
        [HisFieldInfoMapping(8, 2, CobolType = CobolType.Unsigned)]
        public short T_GP7LC03G { get; set; }

        /// <summary>
        /// T_GP7LC03M 9(2)  
        /// </summary>
        [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
        public short T_GP7LC03M { get; set; }

        /// <summary>
        /// T_GP7LC03A 9(4)  
        /// </summary>
        [HisFieldInfoMapping(10, 4, CobolType = CobolType.Unsigned)]
        public short T_GP7LC03A { get; set; }

        /// <summary>
        /// T_GP7LC04 9  
        /// </summary>
        [HisFieldInfoMapping(11, 1, CobolType = CobolType.Unsigned)]
        public short T_GP7LC04 { get; set; }

        // 03 T-TP1DATIDIR.
        // 04 T-GP7LC02.
        /// <summary>
        /// T_GP7LC02A 9(4)  
        /// </summary>
        [HisFieldInfoMapping(12, 4, CobolType = CobolType.Unsigned)]
        public short T_GP7LC02A { get; set; }

        /// <summary>
        /// T_GP7LC02M 9(2)  
        /// </summary>
        [HisFieldInfoMapping(13, 2, CobolType = CobolType.Unsigned)]
        public short T_GP7LC02M { get; set; }

        /// <summary>
        /// T_GP7LB01 9(3)  
        /// </summary>
        [HisFieldInfoMapping(14, 3, CobolType = CobolType.Unsigned)]
        public short T_GP7LB01 { get; set; }

        /// <summary>
        /// T_GP7LH01 X(3)  
        /// </summary>
        [HisFieldInfoMapping(15, 3)]
        public string T_GP7LH01 { get; set; }

        /// <summary>
        /// T_GP7LB02 9(4)  
        /// </summary>
        [HisFieldInfoMapping(16, 4, CobolType = CobolType.Unsigned)]
        public short T_GP7LB02 { get; set; }

        /// <summary>
        /// T_GP7LB03 9(8)  
        /// </summary>
        [HisFieldInfoMapping(17, 8, CobolType = CobolType.Unsigned)]
        public int T_GP7LB03 { get; set; }

        // 04 T-GP7LC09.
        /// <summary>
        /// T_GP7LC19 X  
        /// </summary>
        [HisFieldInfoMapping(18, 1)]
        public string T_GP7LC19 { get; set; }

        /// <summary>
        /// T_GP7LC29 9  
        /// </summary>
        [HisFieldInfoMapping(19, 1, CobolType = CobolType.Unsigned)]
        public short T_GP7LC29 { get; set; }

        /// <summary>
        /// T_GP7LC39 9  
        /// </summary>
        [HisFieldInfoMapping(20, 1, CobolType = CobolType.Unsigned)]
        public short T_GP7LC39 { get; set; }

        // 04 T-TP1CONTDIR.
        /// <summary>
        /// T_GP7LE01_V S9(5)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(21, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal T_GP7LE01_V { get; set; }

        /// <summary>
        /// T_GP7LE02_V S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(22, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal T_GP7LE02_V { get; set; }

        /// <summary>
        /// T_GP7LE03_V S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(23, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal T_GP7LE03_V { get; set; }

        /// <summary>
        /// T_GP7LE04 S9(5) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(24, 3, CobolType = CobolType.Comp3)]
        public int T_GP7LE04 { get; set; }

        // 03 T-TP1APDC.
        /// <summary>
        /// T_GP7LCAT X(3)  
        /// </summary>
        [HisFieldInfoMapping(25, 3)]
        public string T_GP7LCAT { get; set; }

        /// <summary>
        /// T_GP7LENT 9(4)  
        /// </summary>
        [HisFieldInfoMapping(26, 4, CobolType = CobolType.Unsigned)]
        public short T_GP7LENT { get; set; }

        /// <summary>
        /// T_GP7LCUC X  
        /// </summary>
        [HisFieldInfoMapping(27, 1)]
        public string T_GP7LCUC { get; set; }

        /// <summary>
        /// T_GP7LCIM X  
        /// </summary>
        [HisFieldInfoMapping(28, 1)]
        public string T_GP7LCIM { get; set; }

        // 04 T-GP7LACQ.
        /// <summary>
        /// T_GP7LACQA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(29, 4, CobolType = CobolType.Unsigned)]
        public short T_GP7LACQA { get; set; }

        /// <summary>
        /// T_GP7LACQM 9(2)  
        /// </summary>
        [HisFieldInfoMapping(30, 2, CobolType = CobolType.Unsigned)]
        public short T_GP7LACQM { get; set; }

        // 04 T-GP7LCES.
        /// <summary>
        /// T_GP7LCESA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(31, 4, CobolType = CobolType.Unsigned)]
        public short T_GP7LCESA { get; set; }

        /// <summary>
        /// T_GP7LCESM 9(2)  
        /// </summary>
        [HisFieldInfoMapping(32, 2, CobolType = CobolType.Unsigned)]
        public short T_GP7LCESM { get; set; }

        /// <summary>
        /// T_GP7LNPE X(3)  
        /// </summary>
        [HisFieldInfoMapping(33, 3)]
        public string T_GP7LNPE { get; set; }
        #endregion Tracciato Host

        #endregion Properties
    }
}
