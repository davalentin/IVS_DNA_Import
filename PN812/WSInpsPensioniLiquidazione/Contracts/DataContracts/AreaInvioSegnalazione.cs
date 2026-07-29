using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using INPS.Pensioni.Liquidazione.Entity;
using System.Runtime.Serialization;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaInvioSegnalazione
    {
        #region public data members
        [DataMember]
        public Segnalazione Segnalazione { get; set; }
        #endregion public data members
    }
}
