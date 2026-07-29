using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class AreaNonPresenzaREDD
    {
        #region Constructor
        internal AreaNonPresenzaREDD()
        {
            this.AreaDatiStampa = new AreaDatiStampa();
        }
        #endregion Constructor

        #region Properties
        [HisComplexAreaInfoMapping(0)]
        public AreaDatiStampa AreaDatiStampa { get; set; }

        #endregion Properties

        #region Nested class

        #endregion Nested class
    }
}
