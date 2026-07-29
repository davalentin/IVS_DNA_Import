using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostRequest
{
    public class CI01_CI02Request
    {
        #region Constructor
        public CI01_CI02Request()
        {
            this.Gruppo1 = new PCIINPU7.Gruppo1();
            this.Gruppo2 = new PCIINPU7.Gruppo2();
            this.Gruppo3 = new PCIINPU7.Gruppo3();
            this.Gruppo4 = new PCIINPU7.Gruppo4();
        }
        #endregion Constructor

        #region Properties


        [HisFieldInfoMapping(0, 3)]
        public string FILLER { get; set; }

        [HisComplexAreaInfoMapping(1)]
        public PCIINPU7.Gruppo1 Gruppo1 { get; set; }

        [HisComplexAreaInfoMapping(2)]
        public PCIINPU7.Gruppo2 Gruppo2 { get; set; }

        [HisComplexAreaInfoMapping(3)]
        public PCIINPU7.Gruppo3 Gruppo3 { get; set; }

        [HisComplexAreaInfoMapping(4)]
        public PCIINPU7.Gruppo4 Gruppo4 { get; set; }
        #endregion Properties
    }
}
