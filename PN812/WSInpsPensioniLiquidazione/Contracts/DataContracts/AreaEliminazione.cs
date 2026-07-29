using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    public class AreaEliminazione
    {
        [DataMember]
        public GestionePensione.DatiEliminazione DatiEliminazione { get; set; }

        [DataMember]
        public List<GestioneAreaEliminazione.CodiceEliminazione> ListaCodiceEliminazione { get; set; }

        [DataMember]
        public DateTime? DataFineCalcoloArretratiCalcolata { get; set; }

        [DataMember]
        public DateTime? DataFineCalcoloArretratiStorico { get; set; }

        [DataMember]
        public bool? IsMemo102Abilitato { get; set; }
    }
}