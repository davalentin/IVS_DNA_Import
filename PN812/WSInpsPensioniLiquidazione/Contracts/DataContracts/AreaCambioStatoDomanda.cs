using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaCambioStatoDomanda
    {
        #region Input/Output Parameters
        [DataMember]
        public long NumeroDomanda { get; set; }
        [DataMember]
        public string StatoPensione { get; set; }

        [DataMember]
        public string NCertificato { get; set; }

        [DataMember]
        public DateTime? DataElaborazioneWebdom { get; set; }

        [DataMember]
        public Utility.TipoAppartenenza? TipoAppOperatore { get; set; }
        //update operation
        [DataMember]
        public long NumeroDomandaUpdate { get; set; }
        [DataMember]
        public string NuovoStatoPensione { get; set; }
        [DataMember]
        public string NuovoNCertificato { get; set; }
        [DataMember]
        public DateTime? NuovaDataElaborazioneWebdom { get; set; }
        #endregion Input/Output Parameters

        #region Input Parameters
        [DataMember]
        public Utility.Ruolo? Ruolo { get; set; }
        [DataMember]
        public int? Sede { get; set; }
        [DataMember]
        public bool IsUpdateOperation { get; set; }
        #endregion Input Parameters

        #region Output Parameters
        [DataMember]
        public string SedeDiversa { get; set; }
        #endregion Outpur Parameters
    }
}