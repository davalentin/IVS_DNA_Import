using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaLavorazioneManualeAutomatiche
    {
        [DataMember]
        public List<LavorazioneManualeAutomatiche> ListLavorazioneManualeAutomatiche { get; set; }

        public AreaLavorazioneManualeAutomatiche()
        {
            this.ListLavorazioneManualeAutomatiche = new List<LavorazioneManualeAutomatiche>();
        }

        #region Nested Class

        [DataContract]
        public class LavorazioneManualeAutomatiche
        {
            [DataMember]
            public long? Id { get; set; }
            [DataMember]
            public long? NDomus { get; set; }
            [DataMember]
            public string SiglaCategoria { get; set; }
            [DataMember]
            public short CodiceSede { get; set; }
            [DataMember]
            public string Gruppo { get; set; }
            [DataMember]
            public string Prodotto { get; set; }
            [DataMember]
            public string Tipo { get; set; }
            [DataMember]
            public DateTime? DecorrenzaOriginaria { get; set; }
            [DataMember]
            public bool? AutorizzazioneManuale { get; set; }
            [DataMember]
            public string MatricolaUtente { get; set; }
            [DataMember]
            public string TipoApp { get; set; }
        }

        #endregion Nested Class
    }
}