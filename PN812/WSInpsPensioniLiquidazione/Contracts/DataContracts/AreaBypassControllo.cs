using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaBypassControllo
    {
        [DataMember]
        public List<BypassControllo> ListBypassControllo {get;set;}

        [DataMember]
        public List<DecBypassControllo> ListDecBypassControllo { get; set; }

        public AreaBypassControllo()
        {
            this.ListBypassControllo = new List<BypassControllo>();
        }

        #region Nested Class

        [DataContract]
        public class BypassControllo
        {
            [DataMember]
            public long Id { get; set; }
            [DataMember]
            public long? NDomus { get; set; }
            [DataMember]
            public string CodCategoria { get; set; }
            [DataMember]
            public short? CodiceSede { get; set; }
            [DataMember]
            public int? NCertificato { get; set; }
            [DataMember]
            public string Note { get; set; }
            [DataMember]
            public string Matricola { get; set; }
            [DataMember]
            public bool Lock { get; set; }
            [DataMember]
            public long IdDecBypassControllo { get; set; }
        }

        [DataContract]
        public class DecBypassControllo
        {
            [DataMember]
            public long Id { get; set; }
            [DataMember]
            public string Nome { get; set; }
            [DataMember]
            public string Descrizione { get; set; }
            [DataMember]
            public string TipoApp { get; set; }
        }


        #endregion Nested Class

    }

   
}