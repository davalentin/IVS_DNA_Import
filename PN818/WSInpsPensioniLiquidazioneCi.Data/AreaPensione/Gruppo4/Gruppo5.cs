using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class Gruppo5
    {
        #region Constructor
        public Gruppo5()
        {
            this.AreaPostFlags = new AreaPostFlags();
        }
        #endregion Constructor

        #region tracciato COBOL
        #endregion tracciato COBOL

        #region Tracciato Host
        
        [HisComplexAreaInfoMapping(0)]
        public AreaPostFlags AreaPostFlags { get; set; }
        #endregion Tracciato Host
    }
}
