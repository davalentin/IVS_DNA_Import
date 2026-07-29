using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class CI02NonPresenzaREDD: ITransactionInfo
    {
        #region Constructor
        internal CI02NonPresenzaREDD()
        {
            this.NonPresenzaREDD = new AreaNonPresenzaREDD();
        }
        #endregion Constructor

        #region Tracciato Host
        [HisFieldInfoMapping(0, 1920)]
        public string FILLER { get; set; }

        [HisComplexAreaInfoMapping(1)]
        public AreaNonPresenzaREDD NonPresenzaREDD { get; set; }
        #endregion Tracciato Host

        #region Properties
        public string TransactionName
        {
            get { return "Area Non Presenza Redditi tradotta"; }
        }
        #endregion Properties
    }
}
