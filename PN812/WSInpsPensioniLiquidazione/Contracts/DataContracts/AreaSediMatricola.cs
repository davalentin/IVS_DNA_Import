using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaSediMatricola
    {
        public AreaSediMatricola()
        {
            elencoSediMatricole = new List<Entity.SediMatricola>();
        }

        [DataMember]
        public List<Entity.SediMatricola> elencoSediMatricole { get; set; }

        [DataMember]
        public Entity.SediMatricola SediMatricola { get; set; }
    }
}