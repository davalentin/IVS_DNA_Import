using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaAziendeVOESO
    {
        public AreaAziendeVOESO()
        {
            ElencoAziendeVOESO = new List<Entity.AziendeVOESO>();
        }

        [DataMember]
        public List<Entity.AziendeVOESO> ElencoAziendeVOESO { get; set; }

        [DataMember]
        public Entity.AziendeVOESO AziendaVOESO { get; set; }
    }
}