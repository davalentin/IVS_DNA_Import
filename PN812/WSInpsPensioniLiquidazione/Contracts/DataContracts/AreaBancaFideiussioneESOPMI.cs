using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaBancaFideiussioneESOPMI
    {
        public AreaBancaFideiussioneESOPMI()
        {
            ElencoBancheFideiussione = new List<GestioneBancheFideiussioneESOPMI.DecBancaFideiussione>();
            ElencoAziende = new List<GestioneDecodificaAzienda.DecAzienda>();
            ElencoAziendeAssegnoGGmmAAAA = new List<GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA>();
        }

        [DataMember]
        public List<GestioneBancheFideiussioneESOPMI.DecBancaFideiussione> ElencoBancheFideiussione { get; set; }

        [DataMember]
        public GestioneBancheFideiussioneESOPMI.DecBancaFideiussione BancaFideiussione { get; set; }

        [DataMember]
        public List<GestioneDecodificaAzienda.DecAzienda> ElencoAziende { get; set; }

        [DataMember]
        public GestioneDecodificaAzienda.DecAzienda Azienda { get; set; }

        [DataMember]
        public GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA AziendaGGmmAAAA { get; set; }

        [DataMember]
        public List<GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA> ElencoAziendeAssegnoGGmmAAAA { get; set; }
    }
}