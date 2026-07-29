using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class Gruppo2
    {
        #region Constructor
        public Gruppo2()
        {
            this.AreaW3 = new AreaW3();
            this.AreaW4 = new AreaW4();
        }
        #endregion Constructor

        #region Properties
        [HisComplexAreaInfoMapping(0)]
        public AreaW3 AreaW3 { get; set; }

        [HisComplexAreaInfoMapping(1)]
        public AreaW4 AreaW4 { get; set; }

        /// <summary>
        /// FILLER2017_1 X(60)  
        /// </summary>
        [HisFieldInfoMapping(2, 60)]
        public string FILLER2017_1 { get; set; }
        #endregion Properties
    }
}

