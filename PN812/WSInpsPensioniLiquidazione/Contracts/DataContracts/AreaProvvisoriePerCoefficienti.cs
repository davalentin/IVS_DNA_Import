using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaProvvisoriePerCoefficienti
    {
        public AreaProvvisoriePerCoefficienti()
        {
            DataDecorrenzaProvvisoriaObbligatoria = new DateTime?();
        }
        
        [DataMember]
        public DateTime? DataDecorrenzaProvvisoriaObbligatoria { get; set; }
    }
}