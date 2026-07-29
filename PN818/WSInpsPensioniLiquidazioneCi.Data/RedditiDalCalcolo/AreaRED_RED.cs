using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIRED4
{
    public class AreaRED_RED
    {
        #region Constructor
        internal AreaRED_RED()
        {
        }
        #endregion Constructor

        #region tracciato COBOL
        //             05  RED-RED                       PIC X(4).                  
        //*                                                VALORE "REDD" 
        #endregion tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// RED_RED X(4)  
        // *VALORE "REDD"
        /// </summary>
        [HisFieldInfoMapping(0, 4)]
        public string RED_RED { get; set; }

        #endregion Tracciato Host
    }
}
