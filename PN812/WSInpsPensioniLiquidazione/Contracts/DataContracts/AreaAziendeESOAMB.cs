using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione.Entity;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaAziendeESOAMB
    {
        public AreaAziendeESOAMB()
        {
            ElencoAziendeESOAMB = new List<AziendeESOAMB>();
            ElencoAziendeAssegnoGGmmAAAA = new List<GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA>();
        }

        [DataMember]
        public List<AziendeESOAMB> ElencoAziendeESOAMB { get; set; }

        [DataMember]
        public AziendeESOAMB AziendaESOAMB { get; set; }

        [DataMember]
        public GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA AziendaGGmmAAAA { get; set; }

        [DataMember]
        public List<GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA> ElencoAziendeAssegnoGGmmAAAA { get; set; }
    }
}