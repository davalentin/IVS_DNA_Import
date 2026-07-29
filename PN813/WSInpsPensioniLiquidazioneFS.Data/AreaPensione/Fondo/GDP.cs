using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo
{
    public class GDP : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //  03  FONDO-INPDAP REDEFINES FONDO-PI.
        //      04    TIPOR-GDP PIC X.
        //      04    FONDO-GDP PIC X(3).
        //      04    TPENS-GPD PIC 9.
        //      04    NATPENS-GDP.
        //          05  NATPENS1-GDP PIC X.
        //          05  NATPENS2-GDP PIC X.
        //          05  NATPENS3-GDP PIC X.
        //      04    DECPENS-GDP PIC 9(8).
        //      04    SOSPENS-GDP PIC 9(6).
        //      04    DECEC-GDP PIC 9(8).
        //      04    DATASS-GDP PIC 9(8).
        //      04    DATACES-GDP PIC 9(8).
        //      04    MATR-GDP PIC X(7).
        //      04    CSPEC-GDP PIC X.
        //      04    CAUSA-GDP PIC 9(4).
        //      04    PROF-GDP PIC X(4).
        //      04    NCALC-GDP PIC X.
        //      04    PAL-GDP-EURO PIC 9(6)V9(4).
        //      04    FLINP-GDP PIC X.
        //      04    DIIS-GDP PIC 9.
        //      04    F13ME-GDP PIC 9.
        //      04    FAAGO-GDP PIC 9.
        //      04    ASSAC-GDP PIC 9(8).
        //      04    SU92-GDP PIC 9(5).
        //      04    SU94-GDP PIC 9(5).
        //      04    SU95-GDP PIC 9(5).
        //      04    SU97-GDP PIC 9(5).
        //      04    SUCE-GDP PIC 9(5).
        //      04    SUAN-GDP PIC 9(2).
        //      04    RETR-GDP-EURO PIC 9(6)V9(4).
        //      04    QA14-GDP-EURO PIC 9(4)V9(4).
        //      04    IIS-GDP-EURO PIC 9(6)V9(4).
        //      04    NO336-GDP-EURO PIC 9(6)V9(4).
        //      04    RETRM-GDP-EURO PIC 9(9)V9(4).
        //      04    IISLOR-GDP-EURO PIC 9(6)V9(4).
        //      04    FILLER-GDP PIC 9(8).
        //      04    POLOPL-GDP PIC 9(4).
        //      04    PROGR-GDP PIC 9(2).
        //      04    DECCALC-GDP PIC 9(8).
        //      04    L537-ANNI-UT-GDP PIC 9.
        //      04    IIS-CONG-DIR-MIN-GDP PIC 99.
        //      04    PAL-A2C12L33595-GDP PIC 9(6)V9(4).
        //      04    RIP-INPDAP-GDP PIC 9(3)V9(4).
        //      04    INCR-IPOST-GDP PIC S9(7)V9(4).
        //      04    RIP-INPS-GDP.
        //          06  RIP-INPS-NT03-GDP PIC 9(15) COMP-3.
        //          06  RIP-INPS-NT04-GDP PIC 9(3) COMP-3.
        //          06  RIP-INPS-NT05-GDP PIC 9(3) COMP-3.
        //      04    FLG-MEF-GDP PIC X.
        //      04    DIVISORE-GDP PIC 99.
        //      04    CAPITOLO-GDP PIC X(3).
        //      04    ANNI-MAX-GDP PIC 9(2).
        //      04  SUAN-MM-GDP PIC 9(2).             
        //      04  SUAN-GG-GDP PIC 9(2).             
        //      04  SU-SETT-DIR-GDP PIC 9(4).             
        //      04  SU-SETT-MIS-GDP PIC 9(4).  
        //      04    FILLER PIC X(1).

        #endregion Tracciato COBOL

        #region Tracciato Host
        // 03  FONDO-INPDAP REDEFINES FONDO-PI.
        /// <summary>
        /// TIPOR-GDP X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string TIPOR_GDP { get; set; }

        /// <summary>
        /// FONDO-GDP X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string FONDO_GDP { get; set; }

        /// <summary>
        /// TPENS-GPD 9
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short TPENS_GDP { get; set; }

        // 04    NATPENS-GDP.
        /// <summary>
        /// NATPENS1-GDP X
        /// </summary>
        [HisFieldInfoMapping(3, 1)]
        public string NATPENS1_GDP { get; set; }

        /// <summary>
        /// NATPENS2-GDP X
        /// </summary>
        [HisFieldInfoMapping(4, 1)]
        public string NATPENS2_GDP { get; set; }

        /// <summary>
        /// NATPENS3-GDP X
        /// </summary>
        [HisFieldInfoMapping(5, 1)]
        public string NATPENS3_GDP { get; set; }

        /// <summary>
        /// DECPENS-GDP 9(8)
        /// </summary>
        [HisFieldInfoMapping(6, 8, CobolType = CobolType.Unsigned)]
        public int DECPENS_GDP { get; set; }

        /// <summary>
        /// SOSPENS-GDP 9(6)
        /// </summary>
        [HisFieldInfoMapping(7, 6, CobolType = CobolType.Unsigned)]
        public int SOSPENS_GDP { get; set; }

        /// <summary>
        /// DECEC-GDP 9(8)
        /// </summary>
        [HisFieldInfoMapping(8, 8, CobolType = CobolType.Unsigned)]
        public int DECEC_GDP { get; set; }

        /// <summary>
        /// DATASS-GDP 9(8)
        /// </summary>
        [HisFieldInfoMapping(9, 8, CobolType = CobolType.Unsigned)]
        public int DATASS_GDP { get; set; }

        /// <summary>
        /// DATACES-GDP 9(8)
        /// </summary>
        [HisFieldInfoMapping(10, 8, CobolType = CobolType.Unsigned)]
        public int DATACES_GDP { get; set; }

        /// <summary>
        /// MATR-GDP X(7)
        /// </summary>
        [HisFieldInfoMapping(11, 7)]
        public string MATR_GDP { get; set; }

        /// <summary>
        /// CSPEC-GDP X
        /// </summary>
        [HisFieldInfoMapping(12, 1)]
        public string CSPEC_GDP { get; set; }

        /// <summary>
        /// CAUSA-GDP 9(4)
        /// </summary>
        [HisFieldInfoMapping(13, 4, CobolType = CobolType.Unsigned)]
        public short CAUSA_GDP { get; set; }

        /// <summary>
        /// PROF-GDP X(4)
        /// </summary>
        [HisFieldInfoMapping(14, 4)]
        public string PROF_GDP { get; set; }

        /// <summary>
        /// NCALC-GDP X
        /// </summary>
        [HisFieldInfoMapping(15, 1)]
        public string NCALC_GDP { get; set; }

        /// <summary>
        /// PAL-GDP-EURO 9(6)V9(4)
        /// </summary>
        [HisFieldInfoMapping(16, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal PAL_GDP_EURO { get; set; }

        /// <summary>
        /// FLINP-GDP X
        /// </summary>
        [HisFieldInfoMapping(17, 1)]
        public string FLINP_GDP { get; set; }

        /// <summary>
        /// DIIS-GDP 9
        /// </summary>
        [HisFieldInfoMapping(18, 1, CobolType = CobolType.Unsigned)]
        public short DIIS_GDP { get; set; }

        /// <summary>
        /// F13ME-GDP 9
        /// </summary>
        [HisFieldInfoMapping(19, 1, CobolType = CobolType.Unsigned)]
        public short F13ME_GDP { get; set; }

        /// <summary>
        /// FAAGO-GDP 9
        /// </summary>
        [HisFieldInfoMapping(20, 1, CobolType = CobolType.Unsigned)]
        public short FAAGO_GDP { get; set; }

        /// <summary>
        /// ASSAC-GDP 9(8)
        /// </summary>
        [HisFieldInfoMapping(21, 8, CobolType = CobolType.Unsigned)]
        public int ASSAC_GDP { get; set; }

        /// <summary>
        /// SU92-GDP 9(5)
        /// </summary>
        [HisFieldInfoMapping(22, 5, CobolType = CobolType.Unsigned)]
        public int SU92_GDP { get; set; }

        /// <summary>
        /// SU94-GDP 9(5)
        /// </summary>
        [HisFieldInfoMapping(23, 5, CobolType = CobolType.Unsigned)]
        public int SU94_GDP { get; set; }

        /// <summary>
        /// SU95-GDP 9(5)
        /// </summary>
        [HisFieldInfoMapping(24, 5, CobolType = CobolType.Unsigned)]
        public int SU95_GDP { get; set; }

        /// <summary>
        /// SU97-GDP 9(5)
        /// </summary>
        [HisFieldInfoMapping(25, 5, CobolType = CobolType.Unsigned)]
        public int SU97_GDP { get; set; }

        /// <summary>
        /// SUCE-GDP 9(5)
        /// </summary>
        [HisFieldInfoMapping(26, 5, CobolType = CobolType.Unsigned)]
        public int SUCE_GDP { get; set; }

        /// <summary>
        /// SUAN-GDP 9(2)
        /// </summary>
        [HisFieldInfoMapping(27, 2, CobolType = CobolType.Unsigned)]
        public short SUAN_GDP { get; set; }

        /// <summary>
        /// RETR-GDP-EURO 9(6)V9(4)
        /// </summary>
        [HisFieldInfoMapping(28, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal RETR_GDP_EURO { get; set; }

        /// <summary>
        /// QA14-GDP-EURO 9(4)V9(4)
        /// </summary>
        [HisFieldInfoMapping(29, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal QA14_GDP_EURO { get; set; }

        /// <summary>
        /// IIS-GDP-EURO 9(6)V9(4)
        /// </summary>
        [HisFieldInfoMapping(30, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal IIS_GDP_EURO { get; set; }

        /// <summary>
        /// NO336-GDP-EURO 9(6)V9(4)
        /// </summary>
        [HisFieldInfoMapping(31, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal NO336_GDP_EURO { get; set; }

        /// <summary>
        /// RETRM-GDP-EURO 9(9)V9(4)
        /// </summary>
        [HisFieldInfoMapping(32, 13, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal RETRM_GDP_EURO { get; set; }

        /// <summary>
        /// IISLOR-GDP-EURO 9(6)V9(4)
        /// </summary>
        [HisFieldInfoMapping(33, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal IISLOR_GDP_EURO { get; set; }

        /// <summary>
        /// FILLER-GDP 9(8)
        /// </summary>
        [HisFieldInfoMapping(34, 8, CobolType = CobolType.Unsigned)]
        public int FILLER_GDP { get; set; }

        /// <summary>
        /// POLOPL-GDP 9(4)
        /// </summary>
        [HisFieldInfoMapping(35, 4, CobolType = CobolType.Unsigned)]
        public short POLOPL_GDP { get; set; }

        /// <summary>
        /// PROGR-GDP 9(2)
        /// </summary>
        [HisFieldInfoMapping(36, 2, CobolType = CobolType.Unsigned)]
        public short PROGR_GDP { get; set; }

        /// <summary>
        /// DECCALC-GDP 9(8)
        /// </summary>
        [HisFieldInfoMapping(37, 8, CobolType = CobolType.Unsigned)]
        public int DECCALC_GDP { get; set; }

        /// <summary>
        /// L537-ANNI-UT-GDP 9
        /// </summary>
        [HisFieldInfoMapping(38, 1, CobolType = CobolType.Unsigned)]
        public short L537_ANNI_UT_GDP { get; set; }

        /// <summary>
        /// IIS-CONG-DIR-MIN-GDP 99
        /// </summary>
        [HisFieldInfoMapping(39, 2, CobolType = CobolType.Unsigned)]
        public short IIS_CONG_DIR_MIN_GDP { get; set; }

        /// <summary>
        /// PAL-A2C12L33595-GDP 9(6)V9(4)
        /// </summary>
        [HisFieldInfoMapping(40, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal PAL_A2C12L33595_GDP { get; set; }

        /// <summary>
        /// RIP-INPDAP-GDP 9(3)V9(4)
        /// </summary>
        [HisFieldInfoMapping(41, 7, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal RIP_INPDAP_GDP { get; set; }

        /// <summary>
        /// INCR-IPOST-GDP S9(7)V9(4)
        /// </summary>
        [HisFieldInfoMapping(42, 11, Scale = 4, CobolType = CobolType.Signed)]
        public decimal INCR_IPOST_GDP { get; set; }

        // 04    RIP-INPS-GDP.
        /// <summary>
        /// RIP-INPS-NT03-GDP 9(15) COMP-3
        /// </summary>
        [HisFieldInfoMapping(43, 8, CobolType = CobolType.Comp3Unsigned)]
        public long RIP_INPS_NT03_GDP { get; set; }

        /// <summary>
        /// RIP-INPS-NT04-GDP 9(3) COMP-3
        /// </summary>
        [HisFieldInfoMapping(44, 2, CobolType = CobolType.Comp3Unsigned)]
        public long RIP_INPS_NT04_GDP { get; set; }

        /// <summary>
        /// RIP-INPS-NT05-GDP 9(3) COMP-3
        /// </summary>
        [HisFieldInfoMapping(45, 2, CobolType = CobolType.Comp3Unsigned)]
        public long RIP_INPS_NT05_GDP { get; set; }

        /// <summary>
        /// FLG-MEF-GDP X
        /// </summary>
        [HisFieldInfoMapping(46, 1)]
        public string FLG_MEF_GDP { get; set; }

        /// <summary>
        ///  DIVISORE-GDP 99
        /// </summary>
        [HisFieldInfoMapping(47, 2, CobolType = CobolType.Unsigned)]
        public short DIVISORE_GDP { get; set; }

        /// <summary>
        /// CAPITOLO-GDP X(3)
        /// </summary>
        [HisFieldInfoMapping(48, 3)]
        public string CAPITOLO_GDP { get; set; }

        /// <summary>
        /// ANNI-MAX-GDP 9(2)
        /// </summary>
        [HisFieldInfoMapping(49, 2, CobolType = CobolType.Unsigned)]
        public short ANNI_MAX_GDP { get; set; }

        /// <summary>
        /// SUAN-MM-GDP 9(2)
        /// </summary>
        [HisFieldInfoMapping(50, 2, CobolType = CobolType.Unsigned)]
        public short SUAN_MM_GDP { get; set; }

        /// <summary>
        /// SUAN-GG-GDP 9(2)
        /// </summary>
        [HisFieldInfoMapping(51, 2, CobolType = CobolType.Unsigned)]
        public short SUAN_GG_GDP { get; set; }

        /// <summary>
        /// SU-SETT-DIR-GDP 9(4)
        /// </summary>
        [HisFieldInfoMapping(52, 4, CobolType = CobolType.Unsigned)]
        public short SU_SETT_DIR_GDP { get; set; }

        /// <summary>
        /// SU-SETT-MIS-GDP 9(4)
        /// </summary>
        [HisFieldInfoMapping(53, 4, CobolType = CobolType.Unsigned)]
        public short SU_SETT_MIS_GDP { get; set; }

        /// <summary>
        /// FILLER X(1)
        /// </summary>
        [HisFieldInfoMapping(54, 1)]
        public string FILLER { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "GDP"; }
        }
        #endregion Properties
    }
}
