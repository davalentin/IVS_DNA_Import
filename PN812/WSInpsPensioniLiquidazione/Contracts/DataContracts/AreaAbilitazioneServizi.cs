using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Data;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaAbilitazioneServizi
    {
        [DataMember]
        public bool IsPolarizzazioneENPALSAbilitata { get; set; }

        [DataMember]
        public bool IsPolarizzazioneSuperstitiENPALSAbilitata { get; set; }
    }
}