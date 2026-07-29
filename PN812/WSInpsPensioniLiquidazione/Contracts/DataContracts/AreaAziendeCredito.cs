using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaAziendeCredito
    {
        public AreaAziendeCredito()
        {
            elencoAziendeCredito = new List<Entity.AziendeCredito>();
        }

        [DataMember]
        public List<Entity.AziendeCredito> elencoAziendeCredito { get; set; }

        [DataMember]
        public Entity.AziendeCredito AziendaCredito { get; set; }
    }
}