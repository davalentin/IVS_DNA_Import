using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostResponse.AreaStampa
{
    public class Conguagli_Arretrati
    {
        #region Constructor
        internal Conguagli_Arretrati()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 COD-PAG-ARR      PIC 9(01).
        //*                            COD.PAG. ARRETRATI             1920
        //     02 LORDO-ARR        PIC S9(09)V9(04) COMP-3.
        //*                            CONG.ARR.LORDO TRATT.          1921
        //*                            KC05 + KM21 + KC03
        //     02 ONPI-ARR         PIC S9(03)V9(04) COMP-3.
        //*                            CONG. ONPI SU ARRETRATO        1928
        //     02 SIND-ARR         PIC S9(05)V9(04) COMP-3.
        //*                            CONG. SIND. SU ARRETRATI       1932
        //     02 SOLID-ARR        PIC S9(05)V9(04) COMP-3.
        //*                            CONG. SOLIDARIETA'SU ARRETRATI 1937
        //     02 ERAR-ARR         PIC S9(07)V9(04) COMP-3.
        //*                            CONG. FISCALE A DEBITO/CREDITO 1942
        //     02 NETTO-ARR        PIC S9(07)V9(04) COMP-3.
        //*                            TOT.ARR.NETTO TRATT.(GP1AXC1)  1948
        //*                            1° LIQUIDATE (COD.ARRETR.=1)
        //     02 IMPO-AC          PIC S9(09)V9(04) COMP-3.
        //*                            IMPONIBILE ANNO IN CORSO       1954
        //     02 IMPO-AP           PIC S9(09)V9(04) COMP-3.
        //*                            IMPONIBILE ANNI PRECEDENTI     1961
        //     02 IRPEF-AC         PIC S9(07)V9(04) COMP-3.
        //*                            IRPEF ANNO IN CORSO            1968
        //     02 IRPEF-AP         PIC S9(07)V9(04) COMP-3.
        //*                            IRPEF ANNI PRECEDENTI          1974
        //     02 DETR-AP          PIC S9(07)V9(04) COMP-3.
        //*                            DETRAZIONI ANNI PRECEDENTI     1980
        //     02 ALIQ-MEDIA       PIC 99V9999.
        //*                            ALIQUOTA MEDIA SU ARRET. A.P.  1986
        //     02 SOLID-PI-ARR     PIC S9(05)V9(04) COMP-3.
        //*                            CONG. SOLID. PI SU ARRETRATI   1992
        //     02 FILLER           PIC X(03).
        //*                                                           1997
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// COD_PAG_ARR 9(01)  
        /// </summary>
        [HisFieldInfoMapping(0, 1, CobolType = CobolType.Unsigned)]
        public short COD_PAG_ARR { get; set; }

        // *                            COD.PAG. ARRETRATI             1920
        /// <summary>
        /// LORDO_ARR S9(09)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(1, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal LORDO_ARR { get; set; }

        // *                            CONG.ARR.LORDO TRATT.          1921
        // *                            KC05 + KM21 + KC03
        /// <summary>
        /// ONPI_ARR S9(03)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(2, 4, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ONPI_ARR { get; set; }

        // *                            CONG. ONPI SU ARRETRATO        1928
        /// <summary>
        /// SIND_ARR S9(05)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(3, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal SIND_ARR { get; set; }

        // *                            CONG. SIND. SU ARRETRATI       1932
        /// <summary>
        /// SOLID_ARR S9(05)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(4, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal SOLID_ARR { get; set; }

        // *                            CONG. SOLIDARIETA'SU ARRETRATI 1937
        /// <summary>
        /// ERAR_ARR S9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(5, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ERAR_ARR { get; set; }

        // *                            CONG. FISCALE A DEBITO/CREDITO 1942
        /// <summary>
        /// NETTO_ARR S9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(6, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal NETTO_ARR { get; set; }

        // *                            TOT.ARR.NETTO TRATT.(GP1AXC1)  1948
        // *                            1° LIQUIDATE (COD.ARRETR.=1)
        /// <summary>
        /// IMPO_AC S9(09)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(7, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IMPO_AC { get; set; }

        // *                            IMPONIBILE ANNO IN CORSO       1954
        /// <summary>
        /// IMPO_AP S9(09)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(8, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IMPO_AP { get; set; }

        // *                            IMPONIBILE ANNI PRECEDENTI     1961
        /// <summary>
        /// IRPEF_AC S9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(9, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IRPEF_AC { get; set; }

        // *                            IRPEF ANNO IN CORSO            1968
        /// <summary>
        /// IRPEF_AP S9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(10, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IRPEF_AP { get; set; }

        // *                            IRPEF ANNI PRECEDENTI          1974
        /// <summary>
        /// DETR_AP S9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(11, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal DETR_AP { get; set; }

        // *                            DETRAZIONI ANNI PRECEDENTI     1980
        /// <summary>
        /// ALIQ_MEDIA 99V9(04)  
        /// </summary>
        [HisFieldInfoMapping(12, 6, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal ALIQ_MEDIA { get; set; }

        // *                            ALIQUOTA MEDIA SU ARRET. A.P.  1986
        /// <summary>
        /// SOLID_PI_ARR S9(05)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(13, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal SOLID_PI_ARR { get; set; }

        // *                            CONG. SOLID. PI SU ARRETRATI   1992
        /// <summary>
        /// FILLER X(03)  
        /// </summary>
        [HisFieldInfoMapping(14, 3)]
        public string FILLER { get; set; }

        // *                                                           1997
        #endregion Tracciato Host

        #region nested class

        #endregion nested class

        #endregion Properties
    }
}
