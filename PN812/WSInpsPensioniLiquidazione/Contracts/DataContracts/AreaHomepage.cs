using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaHomepage
    {
        #region public data members
        [DataMember]
        public AreaAvvisi AreaAvvisi { get; set; }
        [DataMember]
        public AreaMessaggiHermes AreaMessaggiHermes { get; set; }
        [DataMember]
        public AreaAggiornamenti AreaAggiornamenti { get; set; }
        #endregion public data members


    }
}