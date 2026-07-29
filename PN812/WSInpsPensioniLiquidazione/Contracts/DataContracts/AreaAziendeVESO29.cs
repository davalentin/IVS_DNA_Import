using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaAziendeVESO29
    {
        public AreaAziendeVESO29()
        {
            elencoAziendeVESO29 = new List<Entity.AziendeVESO29>();
        }

        [DataMember]
        public List<Entity.AziendeVESO29> elencoAziendeVESO29 { get; set; }

        [DataMember]
        public Entity.AziendeVESO29 AziendaVESO29 { get; set; }
    }
}