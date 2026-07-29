using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using INPS.Pensioni.Liquidazione.Entity;
using System.IO;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaFAQ
    {
        [DataMember]
        public List<FAQ> ElencoFAQ { get; set; }
        [DataMember]
        public List<TipologiaFAQ> ElencoTipologiaFAQ { get; set; }
        [DataMember]
        public MemoryStream PdfDoc { get; set; }
    }
}