using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostResponse.AreaStampa
{
    public class Importi
    {
         #region Constructor
        internal Importi()
		{

		}
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 IMP-CALC-DEC     PIC 9(07)V9(04) COMP-3.
        //*                            IMP.TOT.A CALCOLO ALLA DECORR. 2000
        //*                            GP5/GP6KC04
        //*       (PER LE INDIRETTE COMPRENSIVA DELLA EVENTUALE   )
        //*       (INTEGRAZIONE AL MINIMO SPETTANTE AL DANTE CAUSA)
        //     02 INTEGRATA-DEC    PIC 9(07)V9(04) COMP-3.
        //*                            IMP.PENS.EVENTUAL.INTEGRATA    2006
        //*                            ALLA DECORRENZA ORIGINARIA
        //*                            GP6KC05 FINO AL 8/95
        //*                            GP6KC10 DAL     9/95
        //*       (PER LE INDIRETTE COMPRENSIVA DELLA EVENTUALE   )
        //*       (INTEGRAZIONE AL MINIMO SPETTANTE AL DANTE CAUSA)
        //*
        //     02 IMP-RATINS       PIC 9(07)V9(04) COMP-3.
        //*                            IMP.RATEI INSOLUTI ELIMINATE   2012
        //     02 MAGG-COMB        PIC 9(05)V9(04) COMP-3.
        //*                            IMP.ATTUALE MAGG.EX COMB.      2018
        //     02 MAGG-SOC-V       PIC 9(05)V9(04) COMP-3.
        //*                            IMP.ATTUALE MAGG.SOCIALE       2023
        //     02 PROV-PENS-SUP    PIC X.
        //*                GP5LC04     PROVENIENZA PENS.X SUPERSTITI  2028
        //*                0 = INDIRETTA
        //*                1 = REVERSIBILITA'
        //     02 INTEGR-MIN       PIC X.
        //*                GP5KE06     PRESENZA INTEGRAZ.AL MINIMO    2029
        //*                1 = SUPERIORE AL MINIMO
        //*                2 = INTEGRATA
        //*                3 = MINIMO CON PARZIALE INTEGRAZIONE
        //*                5 = PENSIONE A CALCOLO PURO
        //*                6 = PENSIONE CRISTALLIZZATA
        //     02 FILLER           PIC X(20).
        //*                            LIBERI                         2030
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// IMP_CALC_DEC 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal IMP_CALC_DEC { get; set; }

        // *                            IMP.TOT.A CALCOLO ALLA DECORR. 2000
        // *                            GP5/GP6KC04
        // *       (PER LE INDIRETTE COMPRENSIVA DELLA EVENTUALE   )
        // *       (INTEGRAZIONE AL MINIMO SPETTANTE AL DANTE CAUSA)
        /// <summary>
        /// INTEGRATA_DEC 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(1, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal INTEGRATA_DEC { get; set; }

        // *                            IMP.PENS.EVENTUAL.INTEGRATA    2006
        // *                            ALLA DECORRENZA ORIGINARIA
        // *                            GP6KC05 FINO AL 8/95
        // *                            GP6KC10 DAL     9/95
        // *       (PER LE INDIRETTE COMPRENSIVA DELLA EVENTUALE   )
        // *       (INTEGRAZIONE AL MINIMO SPETTANTE AL DANTE CAUSA)
        //*
        /// <summary>
        /// IMP_RATINS 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(2, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal IMP_RATINS { get; set; }

        // *                            IMP.RATEI INSOLUTI ELIMINATE   2012
        /// <summary>
        /// MAGG_COMB 9(05)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(3, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal MAGG_COMB { get; set; }

        // *                            IMP.ATTUALE MAGG.EX COMB.      2018
        /// <summary>
        /// MAGG_SOC_V 9(05)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(4, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal MAGG_SOC_V { get; set; }

        // *                            IMP.ATTUALE MAGG.SOCIALE       2023
        /// <summary>
        /// PROV_PENS_SUP X  
        /// </summary>
        [HisFieldInfoMapping(5, 1)]
        public string PROV_PENS_SUP { get; set; }

        // *                GP5LC04     PROVENIENZA PENS.X SUPERSTITI  2028
        // *                0 = INDIRETTA
        // *                1 = REVERSIBILITA'
        /// <summary>
        /// INTEGR_MIN X  
        /// </summary>
        [HisFieldInfoMapping(6, 1)]
        public string INTEGR_MIN { get; set; }

        // *                GP5KE06     PRESENZA INTEGRAZ.AL MINIMO    2029
        // *                1 = SUPERIORE AL MINIMO
        // *                2 = INTEGRATA
        // *                3 = MINIMO CON PARZIALE INTEGRAZIONE
        // *                5 = PENSIONE A CALCOLO PURO
        // *                6 = PENSIONE CRISTALLIZZATA
        /// <summary>
        /// FILLER X(20)  
        /// </summary>
        [HisFieldInfoMapping(7, 20)]
        public string FILLER { get; set; }

        // *                            LIBERI                         2030
        #endregion Tracciato Host

        #region nested class

        #endregion nested class

        #endregion Properties
    }
}
