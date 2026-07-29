using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaAziendeEditorialiPerTipo0171
    {
        public AreaAziendeEditorialiPerTipo0171()
        {
            ElencoAnagraficheAccordi = new List<Entity.AnagraficaAccordoPerTipo0171>();
            ElencoAnagraficheAziende = new List<Entity.AnagraficaAziendaPerTipo0171>();
        }

        [DataMember]
        public List<Entity.AnagraficaAccordoPerTipo0171> ElencoAnagraficheAccordi { get; set; }

        [DataMember]
        public Entity.AnagraficaAccordoPerTipo0171 AnagraficheAccordi { get; set; }

        [DataMember]
        public List<Entity.AnagraficaAziendaPerTipo0171> ElencoAnagraficheAziende { get; set; }

        [DataMember]
        public Entity.AnagraficaAziendaPerTipo0171 AnagraficheAziende { get; set; }
    }
}