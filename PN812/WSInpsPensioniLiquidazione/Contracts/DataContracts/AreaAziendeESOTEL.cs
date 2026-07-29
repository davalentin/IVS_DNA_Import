using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaAziendeESOTEL
    {
        public AreaAziendeESOTEL()
        {
            elencoAziendeESOTEL = new List<Entity.AziendeESOTEL>();
        }

        [DataMember]
        public List<Entity.AziendeESOTEL> elencoAziendeESOTEL { get; set; }

        [DataMember]
        public Entity.AziendeESOTEL AziendaESOTEL { get; set; }
    }
}