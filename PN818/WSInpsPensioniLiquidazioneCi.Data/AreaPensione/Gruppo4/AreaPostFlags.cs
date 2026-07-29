using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaPostFlags
    {
        #region tracciato COBOL
        //04 FILLER-24999
        //04 T-CISPRFIL25
        //04 FILLER-99995
        //04 FINE-REC-100K

        #endregion tracciato COBOL

        #region Tracciato Host
  
        [HisFieldInfoMapping(0, 3999)]
        public string FILLER_24999 { get; set; }

        [HisFieldInfoMapping(1, 45015)]
        public string T_CISPRFIL25 { get; set; }

        [HisFieldInfoMapping(2, 29971)]
        public string FILLER_99995 { get; set; }

        [HisFieldInfoMapping(3, 7)]
        public string FILLER_FINE_ZERI { get; set; }

        [HisFieldInfoMapping(4, 4)]
        public string FINE_REC_100K { get; set; }
        
        #endregion Tracciato Host
    }
}
