using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaAziendeESOTRA
    {
        public AreaAziendeESOTRA()
        {
            elencoAziendeESOTRA = new List<Entity.AziendeESOTRA>();
        }

        [DataMember]
        public List<Entity.AziendeESOTRA> elencoAziendeESOTRA { get; set; }

        [DataMember]
        public Entity.AziendeESOTRA AziendaESOTRA { get; set; }
    }
}