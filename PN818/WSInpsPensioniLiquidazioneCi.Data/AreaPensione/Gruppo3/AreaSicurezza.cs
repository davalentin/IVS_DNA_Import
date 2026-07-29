using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaSicurezza
    {
        #region tracciato COBOL
        //04  DATI-SICUREZZA.
        //       05  MATRICOLA-OPER             PIC 9(8).
        //           10 DATA-OPER-A             PIC 9999.
        //           10 DATA-OPER-M             PIC 99.
        //           10 DATA-OPER-G             PIC 99.
        #endregion tracciato COBOL

        #region Tracciato Host
        // 04  DATI-SICUREZZA.
        /// <summary>
        /// MATRICOLA_OPER 9(8)  
        /// </summary>
        [HisFieldInfoMapping(0, 8)]
        public int MATRICOLA_OPER { get; set; }

        /// <summary>
        /// DATA_OPER_A 9999  
        /// </summary>
        [HisFieldInfoMapping(1, 4)]
        public short DATA_OPER_A { get; set; }

        /// <summary>
        /// DATA_OPER_M 99  
        /// </summary>
        [HisFieldInfoMapping(2, 2)]
        public short DATA_OPER_M { get; set; }

        /// <summary>
        /// DATA_OPER_G 99  
        /// </summary>
        [HisFieldInfoMapping(3, 2)]
        public short DATA_OPER_G { get; set; }
        #endregion Tracciato Host
    }
}
