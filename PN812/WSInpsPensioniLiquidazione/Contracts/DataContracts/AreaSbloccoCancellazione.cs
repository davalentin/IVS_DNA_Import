using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaSbloccoCancellazione
    {
        [DataMember]
        public long NumeroDomanda { get; set; }
        [DataMember]
        public short CodiceSede { get; set; }
        [DataMember]
        public byte CentroOperativo { get; set; }
        [DataMember]
        public string SiglaCategoria { get; set; }
        [DataMember]
        public Utility.TipoOperazione? TipoOperazione { get; set; }
    }
}