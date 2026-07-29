using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class GACIAreaDecompressa : ITransactionInfo
    {
        #region Constructor
        public GACIAreaDecompressa()
        {
            this.Gruppo1 = new PCIINPU7.Gruppo1();
            this.Gruppo2 = new PCIINPU7.Gruppo2();
            this.Gruppo3 = new PCIINPU7.Gruppo3();
            this.Gruppo4 = new PCIINPU7.Gruppo4();
            this.AreaCoda = new GACIAreaCoda();
        }
        #endregion Constructor

        #region Properties
        [HisComplexAreaInfoMapping(0)]
        public PCIINPU7.Gruppo1 Gruppo1 { get; set; }

        [HisComplexAreaInfoMapping(1)]
        public PCIINPU7.Gruppo2 Gruppo2 { get; set; }

        [HisComplexAreaInfoMapping(2)]
        public PCIINPU7.Gruppo3 Gruppo3 { get; set; }

        [HisComplexAreaInfoMapping(3)]
        public PCIINPU7.Gruppo4 Gruppo4 { get; set; }

        [HisComplexAreaInfoMapping(4)]
        public GACIAreaCoda AreaCoda { get; set; }
        #endregion

        public string TransactionName
        {
            get { return "Area decompressa tradotta"; }
        }
    }
}
