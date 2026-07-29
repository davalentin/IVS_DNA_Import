using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaAziendeVESO33
    {
        public AreaAziendeVESO33()
        {
            elencoAziendeVESO33 = new List<Entity.AziendeVESO33>();
        }

        [DataMember]
        public List<Entity.AziendeVESO33> elencoAziendeVESO33 { get; set; }

        [DataMember]
        public Entity.AziendeVESO33 AziendaVESO33 { get; set; }
    }
}