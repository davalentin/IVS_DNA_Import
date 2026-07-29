using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class CI05AreaDecompressa: ITransactionInfo
    {
        #region Constructor
        internal CI05AreaDecompressa()
        {
            this.Richiedente = new CI05AreaRichiedente();
            this.DanteCausa = new CI05AreaDanteCausa();
        }
        #endregion Constructor

        #region Properties
        [HisComplexAreaInfoMapping(0)]
        public CI05AreaRichiedente Richiedente { get; set; }

        [HisComplexAreaInfoMapping(1)]
        public CI05AreaDanteCausa DanteCausa { get; set; }
        #endregion Properties

        public string TransactionName
        {
            get { return "Area decompressa tradotta"; }
        }
    }
}
