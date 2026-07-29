using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class Gruppo4
    {
        #region Constructor
        public Gruppo4()
        {
            this.AreaCampi2004 = new AreaCampi2004();
            this.AreaCampiVar = new AreaCampiVar();
            this.AreaFlags = new AreaFlags();
        }
        #endregion Constructor

        #region tracciato COBOL
        #endregion tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0)]
        public AreaCampi2004 AreaCampi2004 { get; set; }

        [HisComplexAreaInfoMapping(1)]
        public AreaCampiVar AreaCampiVar { get; set; }

        [HisComplexAreaInfoMapping(2)]
        public AreaCampi2017 AreaCampi2017 { get; set; }

        [HisComplexAreaInfoMapping(3)]
        public AreaCampi2018 AreaCampi2018 { get; set; }

        [HisComplexAreaInfoMapping(4)]
        public AreaFlags AreaFlags { get; set; }
        #endregion Tracciato Host
    }
}
