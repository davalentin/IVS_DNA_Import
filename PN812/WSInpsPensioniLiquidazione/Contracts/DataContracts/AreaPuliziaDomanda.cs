using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using INPS.Pensioni.Liquidazione.Entity;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaPuliziaDomanda
    {
        [DataMember]
        public PuliziaDomanda EntityPuliziaDomanda { get; set; }
        [DataMember]
        public bool IsPuliziaDisponibile { get; set; }
        [DataMember]
        public string SedeDiversa { get; set; }
    }
}