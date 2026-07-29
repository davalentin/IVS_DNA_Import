using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA
{
    public class TipoFondo : ITransactionInfo
    {
        #region Constructor
        internal TipoFondo()
        {

        }
        #endregion Constructor

        #region Properties


        #region Tracciato Host
        [HisFieldInfoMapping(0, 3)]
        public string TIPO_FONDO { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "TipoFondo"; }
        }
        #endregion Properties
    }
}
