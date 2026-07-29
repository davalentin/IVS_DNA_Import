using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaAziendeEditorialiLetteraB
    {
         public AreaAziendeEditorialiLetteraB()
        {
            ElencoAnagraficheAccordi = new List<GestioneAnagraficaAccordiLetteraB.DecodAnagraficaAccordiLetteraB>();
            ElencoAnagraficheAziende = new List<GestioneAnagraficaAziendeLetteraB.DecodAnagraficaAziendeLetteraB>();
        }

        [DataMember]
         public List<GestioneAnagraficaAccordiLetteraB.DecodAnagraficaAccordiLetteraB> ElencoAnagraficheAccordi { get; set; }

        [DataMember]
        public GestioneAnagraficaAccordiLetteraB.DecodAnagraficaAccordiLetteraB AnagraficheAccordi { get; set; }

        [DataMember]
        public List<GestioneAnagraficaAziendeLetteraB.DecodAnagraficaAziendeLetteraB> ElencoAnagraficheAziende { get; set; }

        [DataMember]
        public GestioneAnagraficaAziendeLetteraB.DecodAnagraficaAziendeLetteraB AnagraficheAziende { get; set; }
    }
}