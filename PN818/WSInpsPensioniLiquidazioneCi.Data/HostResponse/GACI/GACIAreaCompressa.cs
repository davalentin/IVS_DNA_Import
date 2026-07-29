using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class GACIAreaCompressa: ITransactionInfo
    {
        #region Constructor
        internal GACIAreaCompressa()
        {

        }
        #endregion Constructor

        #region Properties
        [HisFieldInfoMapping(5, 31192)]
        public string AREA_COMPRESSIONE { get; set; }
        #endregion

        public string TransactionName
        {
            get { return "Area Compressa tradotta"; }
        }

    }
}
