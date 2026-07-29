using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class CI01NonPresenzaREDD : ITransactionInfo
    {
        #region Constructor
        internal CI01NonPresenzaREDD()
        {
            this.Australia = new CI01Record_Australia();
            this.NonPresenzaREDD = new AreaNonPresenzaREDD();
        }
        #endregion Constructor

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0)]
        public CI01Record_Australia Australia { get; set; }

        [HisFieldInfoMapping(1, 240)]
        public string FILLER { get; set; }

        [HisComplexAreaInfoMapping(2)]
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
