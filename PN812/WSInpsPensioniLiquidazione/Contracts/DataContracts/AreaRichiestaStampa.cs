using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaRichiestaStampa
    {
        #region public data member
        [DataMember]
        public string SiglaCategoria { get; set; }
        [DataMember]
        public string CodiceSede { get; set; }
        [DataMember]
        public string Certificato { get; set; }
        #endregion public data member
    }
}