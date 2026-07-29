using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaAvvisiMessaggi
    {
        public AreaAvvisiMessaggi()
        {
        }

        #region public data members
        [DataMember]
        public AreaAvvisi AreaAvvisi { get; set; }
        [DataMember]
        public AreaMessaggiHermes AreaMessaggiHermes { get; set; }
        #endregion
    }
}