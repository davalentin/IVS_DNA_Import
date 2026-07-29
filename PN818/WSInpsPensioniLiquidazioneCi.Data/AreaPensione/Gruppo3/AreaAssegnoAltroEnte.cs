using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaAssegnoAltroEnte
    {
        #region tracciato COBOL
        //  04  IASALTRO.
        //        05  ICODALTRO                   PIC XX.
        //* CODIC ENTE EROGATORE
        //* DECORRENZA
        //            10  IDECASA                 PIC 9999.
        //            10  IDECASM                 PIC 99.
        //        05  IIMPASALT                   PIC 9(7)V9(4) COMP-3.
        //*EURO  IMPORTO ALTRO ASSEGNO
        //* CESSAZIONE
        //            10  ICESASA                 PIC 9999.
        //            10  ICESASM                 PIC 99.
        #endregion tracciato COBOL

        #region Tracciato Host
        // 04  IASALTRO.
        /// <summary>
        /// ICODALTRO XX  
        /// </summary>
        [HisFieldInfoMapping(0, 2)]
        public string ICODALTRO { get; set; }

        // * CODIC ENTE EROGATORE
        /// <summary>
        /// IDECASA 9999  
        /// * DECORRENZA
        /// </summary>
        [HisFieldInfoMapping(1, 4)]
        public short IDECASA { get; set; }

        /// <summary>
        /// IDECASM 99  
        /// * DECORRENZA
        /// </summary>
        [HisFieldInfoMapping(2, 2)]
        public short IDECASM { get; set; }

        /// <summary>
        /// IIMPASALT 9(7)V9(4) COMP-3 
        /// *EURO  IMPORTO ALTRO ASSEGNO
        /// </summary>
        [HisFieldInfoMapping(3, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal IIMPASALT { get; set; }

        /// <summary>
        /// ICESASA 9999 
        /// * CESSAZIONE 
        /// </summary>
        [HisFieldInfoMapping(4, 4)]
        public short ICESASA { get; set; }

        /// <summary>
        /// ICESASM 99  
        /// * CESSAZIONE
        /// </summary>
        [HisFieldInfoMapping(5, 2)]
        public short ICESASM { get; set; }
        #endregion Tracciato Host
    }
}
