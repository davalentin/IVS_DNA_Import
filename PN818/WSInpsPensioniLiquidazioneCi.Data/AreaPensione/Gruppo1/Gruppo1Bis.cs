using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class Gruppo1Bis
    {
        #region Constructor
        public Gruppo1Bis()
        {
            this.AreaTP11 = new AreaTP11Bis();
            this.AreaTP12 = new AreaTP12();
            this.AreaDelegato = new AreaDelegato();
            this.AreaTutore = new AreaTutore();
            this.AreaDati = new AreaDati();
            this.AreaW1L = new AreaW1L();
            this.AreaW2CL = new AreaW2CL();
            this.AreaW2 = new AreaW2();
            this.AreaVarie = new AreaVarie();
        }
        #endregion Constructor

        #region Properties
        [HisComplexAreaInfoMapping(0)]
        public AreaTP11Bis AreaTP11 { get; set; }

        [HisComplexAreaInfoMapping(1)]
        public AreaTP12 AreaTP12 { get; set; }

        [HisComplexAreaInfoMapping(2)]
        public AreaDelegato AreaDelegato { get; set; }

        [HisComplexAreaInfoMapping(3)]
        public AreaTutore AreaTutore { get; set; }

        [HisComplexAreaInfoMapping(4)]
        public AreaDati AreaDati { get; set; }

        [HisComplexAreaInfoMapping(5)]
        public AreaW1L AreaW1L { get; set; }

        [HisComplexAreaInfoMapping(6)]
        public AreaW2CL AreaW2CL { get; set; }

        [HisComplexAreaInfoMapping(7)]
        public AreaW2 AreaW2 { get; set; }

        [HisComplexAreaInfoMapping(8)]
        public AreaVarie AreaVarie { get; set; }
        #endregion Properties
    }
}
