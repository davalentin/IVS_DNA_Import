using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaAziendeEditoriali
    {
        public AreaAziendeEditoriali()
        {
            ElencoAnagraficheAccordi = new List<GestioneAnagraficaAccordi.DecodAnagraficaAccordi>();
            ElencoAnagraficheAziende = new List<GestioneAnagraficaAziende.DecodAnagraficaAziende>();
        }

        [DataMember]
        public List<GestioneAnagraficaAccordi.DecodAnagraficaAccordi> ElencoAnagraficheAccordi { get; set; }

        [DataMember]
        public GestioneAnagraficaAccordi.DecodAnagraficaAccordi AnagraficheAccordi { get; set; }

        [DataMember]
        public List<GestioneAnagraficaAziende.DecodAnagraficaAziende> ElencoAnagraficheAziende { get; set; }

        [DataMember]
        public GestioneAnagraficaAziende.DecodAnagraficaAziende AnagraficheAziende { get; set; }
    }
}