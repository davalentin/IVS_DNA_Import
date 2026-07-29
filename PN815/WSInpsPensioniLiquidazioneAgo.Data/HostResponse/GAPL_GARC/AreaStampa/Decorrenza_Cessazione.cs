using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostResponse.AreaStampa
{
    public class Decorrenza_Cessazione
    {
        #region Constructor
        internal Decorrenza_Cessazione()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 ANNO-VALID       PIC 9(04).
        //*                            ANNO VALIDITA'                 1790
        //     02 DEC-PENS.
        //        03 DEC-PENS-AA   PIC 9(04).
        //        03 DEC-PENS-MM   PIC 9(02).
        //        03 DEC-PENS-GG   PIC 9(02).
        //*                            DECORRENZA PENSIONE            1794
        //     02 DEC-PENS-II.
        //        03 DEC-II-AA     PIC 9(04).
        //        03 DEC-II-MM     PIC 9(02).
        //        03 DEC-II-GG     PIC 9(02).
        //*                            II DECORRENZA PENSIONE         1802
        //     02 INI-CALC-ARR.
        //        03 INI-GG        PIC 9(02).
        //        03 INI-MM        PIC 9(02).
        //        03 INI-AA        PIC 9(04).
        //*                            DATA INIZIO CALC.ARRETR.       1810
        //     02 FINE-CALC-ARR.
        //        03 FINE-GG       PIC 9(02).
        //        03 FINE-MM       PIC 9(02).
        //        03 FINE-AA       PIC 9(04).
        //*                            DATA FINALE CALCOLO ARRETRATI  1818
        //     02 DT-EMISS.
        //        03 EMISS-AA      PIC 9(04).
        //        03 EMISS-MM      PIC 9(02).
        //*                            DATA EMISSIONE                 1826
        //     02 DT-RIPRIS.
        //        03 RIPRIS-AA     PIC 9(04).
        //        03 RIPRIS-MM     PIC 9(02).
        //*                            DATA RIPRISTINO                1832
        //     02 DT-INT-LEG.
        //        03 INT-GG        PIC 9(02).
        //        03 INT-MM        PIC 9(02).
        //        03 INT-AA        PIC 9(04).
        //*                            DECORRENZA INT.LEGALI          1838
        //     02 DT-CESS-ASS.
        //        03 CES-AA        PIC 9(04).
        //        03 CES-MM        PIC 9(02).
        //*                            CESSAZIONE ASSEGNO             1846
        //     02 DT-FIN-LEG.
        //        03 FIN-GG        PIC 9(02).
        //        03 FIN-MM        PIC 9(02).
        //        03 FIN-AA        PIC 9(04).
        //*                            DATA FINE  INT.LEGALI          1852
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// ANNO_VALID 9(04)  
        /// </summary>
        [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
        public short ANNO_VALID { get; set; }

        // *                            ANNO VALIDITA'                 1790
        // 02 DEC-PENS.
        /// <summary>
        /// DEC_PENS_AA 9(04)  
        /// </summary>
        [HisFieldInfoMapping(1, 4, CobolType = CobolType.Unsigned)]
        public short DEC_PENS_AA { get; set; }

        /// <summary>
        /// DEC_PENS_MM 9(02)  
        /// </summary>
        [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
        public short DEC_PENS_MM { get; set; }

        /// <summary>
        /// DEC_PENS_GG 9(02)  
        /// </summary>
        [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
        public short DEC_PENS_GG { get; set; }

        // *                            DECORRENZA PENSIONE            1794
        // 02 DEC-PENS-II.
        /// <summary>
        /// DEC_II_AA 9(04)  
        /// </summary>
        [HisFieldInfoMapping(4, 4, CobolType = CobolType.Unsigned)]
        public short DEC_II_AA { get; set; }

        /// <summary>
        /// DEC_II_MM 9(02)  
        /// </summary>
        [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
        public short DEC_II_MM { get; set; }

        /// <summary>
        /// DEC_II_GG 9(02)  
        /// </summary>
        [HisFieldInfoMapping(6, 2, CobolType = CobolType.Unsigned)]
        public short DEC_II_GG { get; set; }

        // *                            II DECORRENZA PENSIONE         1802
        // 02 INI-CALC-ARR.
        /// <summary>
        /// INI_GG 9(02)  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short INI_GG { get; set; }

        /// <summary>
        /// INI_MM 9(02)  
        /// </summary>
        [HisFieldInfoMapping(8, 2, CobolType = CobolType.Unsigned)]
        public short INI_MM { get; set; }

        /// <summary>
        /// INI_AA 9(04)  
        /// </summary>
        [HisFieldInfoMapping(9, 4, CobolType = CobolType.Unsigned)]
        public short INI_AA { get; set; }

        // *                            DATA INIZIO CALC.ARRETR.       1810
        // 02 FINE-CALC-ARR.
        /// <summary>
        /// FINE_GG 9(02)  
        /// </summary>
        [HisFieldInfoMapping(10, 2, CobolType = CobolType.Unsigned)]
        public short FINE_GG { get; set; }

        /// <summary>
        /// FINE_MM 9(02)  
        /// </summary>
        [HisFieldInfoMapping(11, 2, CobolType = CobolType.Unsigned)]
        public short FINE_MM { get; set; }

        /// <summary>
        /// FINE_AA 9(04)  
        /// </summary>
        [HisFieldInfoMapping(12, 4, CobolType = CobolType.Unsigned)]
        public short FINE_AA { get; set; }

        // *                            DATA FINALE CALCOLO ARRETRATI  1818
        // 02 DT-EMISS.
        /// <summary>
        /// EMISS_AA 9(04)  
        /// </summary>
        [HisFieldInfoMapping(13, 4, CobolType = CobolType.Unsigned)]
        public short EMISS_AA { get; set; }

        /// <summary>
        /// EMISS_MM 9(02)  
        /// </summary>
        [HisFieldInfoMapping(14, 2, CobolType = CobolType.Unsigned)]
        public short EMISS_MM { get; set; }

        // *                            DATA EMISSIONE                 1826
        // 02 DT-RIPRIS.
        /// <summary>
        /// RIPRIS_AA 9(04)  
        /// </summary>
        [HisFieldInfoMapping(15, 4, CobolType = CobolType.Unsigned)]
        public short RIPRIS_AA { get; set; }

        /// <summary>
        /// RIPRIS_MM 9(02)  
        /// </summary>
        [HisFieldInfoMapping(16, 2, CobolType = CobolType.Unsigned)]
        public short RIPRIS_MM { get; set; }

        // *                            DATA RIPRISTINO                1832
        // 02 DT-INT-LEG.
        /// <summary>
        /// INT_GG 9(02)  
        /// </summary>
        [HisFieldInfoMapping(17, 2, CobolType = CobolType.Unsigned)]
        public short INT_GG { get; set; }

        /// <summary>
        /// INT_MM 9(02)  
        /// </summary>
        [HisFieldInfoMapping(18, 2, CobolType = CobolType.Unsigned)]
        public short INT_MM { get; set; }

        /// <summary>
        /// INT_AA 9(04)  
        /// </summary>
        [HisFieldInfoMapping(19, 4, CobolType = CobolType.Unsigned)]
        public short INT_AA { get; set; }

        // *                            DECORRENZA INT.LEGALI          1838
        // 02 DT-CESS-ASS.
        /// <summary>
        /// CES_AA 9(04)  
        /// </summary>
        [HisFieldInfoMapping(20, 4, CobolType = CobolType.Unsigned)]
        public short CES_AA { get; set; }

        /// <summary>
        /// CES_MM 9(02)  
        /// </summary>
        [HisFieldInfoMapping(21, 2, CobolType = CobolType.Unsigned)]
        public short CES_MM { get; set; }

        // *                            CESSAZIONE ASSEGNO             1846
        // 02 DT-FIN-LEG.
        /// <summary>
        /// FIN_GG 9(02)  
        /// </summary>
        [HisFieldInfoMapping(22, 2, CobolType = CobolType.Unsigned)]
        public short FIN_GG { get; set; }

        /// <summary>
        /// FIN_MM 9(02)  
        /// </summary>
        [HisFieldInfoMapping(23, 2, CobolType = CobolType.Unsigned)]
        public short FIN_MM { get; set; }

        /// <summary>
        /// FIN_AA 9(04)  
        /// </summary>
        [HisFieldInfoMapping(24, 4, CobolType = CobolType.Unsigned)]
        public short FIN_AA { get; set; }

        // *                            DATA FINE  INT.LEGALI          1852
        #endregion Tracciato Host

        #region nested class

        #endregion nested class

        #endregion Properties
    }
}
