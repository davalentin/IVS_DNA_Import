using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaAziendeEditorialiPerTipo0179
    {
        public AreaAziendeEditorialiPerTipo0179()
        {
            ElencoAnagraficheAccordi = new List<Entity.AnagraficaAccordoPerTipo0179>();
            ElencoAnagraficheAziende = new List<Entity.AnagraficaAziendaPerTipo0179>();
        }

        [DataMember]
        public List<Entity.AnagraficaAccordoPerTipo0179> ElencoAnagraficheAccordi { get; set; }

        [DataMember]
        public Entity.AnagraficaAccordoPerTipo0179 AnagraficheAccordi { get; set; }

        [DataMember]
        public List<Entity.AnagraficaAziendaPerTipo0179> ElencoAnagraficheAziende { get; set; }

        [DataMember]
        public Entity.AnagraficaAziendaPerTipo0179 AnagraficheAziende { get; set; }
    }
}