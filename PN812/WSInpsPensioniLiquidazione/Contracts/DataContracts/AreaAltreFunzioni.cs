using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaAltreFunzioni
    {
        [DataMember]
        public Entity.AltreFunzioni Abilitazioni { get; set; }
    }
}