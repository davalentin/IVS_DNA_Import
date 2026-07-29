using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using INPS.Pensioni.Liquidazione.Entity;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaAggiornamenti
    {        
        #region public data members
        [DataMember]
        public List<Aggiornamenti> ElencoAggiornamenti { get; set; }
        #endregion public data members
    }
}