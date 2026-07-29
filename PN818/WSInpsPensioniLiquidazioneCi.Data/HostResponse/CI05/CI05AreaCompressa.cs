using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class CI05AreaCompressa: ITransactionInfo
    {
        #region Constructor
        internal CI05AreaCompressa()
        {

        }
        #endregion Constructor

        #region Properties
        [HisFieldInfoMapping(0, 2429)]
        public string AREA_COMPRESSIONE { get; set; }
        #endregion Properties

        public string TransactionName
        {
            get { return "Area Compressa tradotta"; }
        }
    }
}
