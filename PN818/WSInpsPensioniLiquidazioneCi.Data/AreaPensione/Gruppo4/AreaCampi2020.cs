using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaCampi2020
    {
        #region Properties
        #region Tracciato COBOL
        //      02  T-DATI2021		 			
        //         03 T-GP1NUMDECR		PIC X(10)
        //         03 T-GP1DATDECR		PIC X(8)
        #endregion Tracciato COBOL
        #region Tracciato HOST
        /// <summary>
        /// T-GP1NUMDECR		PIC X(10)
        /// </summary>
        [HisFieldInfoMapping(0, 10)]
        public string GP1NUMDECR { get; set; }

        /// <summary>
        /// T-GP1DATDECR		PIC X(8)
        /// </summary>
        [HisFieldInfoMapping(1, 8)]
        public string GP1DATDECR { get; set; }
        #endregion Tracciato HOST
        #endregion Properties
    }
}
