using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace INPS.Pensioni.LiquidazioneFs.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaNoCalcolo
    {
        [DataMember]
        public List<Entity.DatiRecordNoCalcolo> LstRecordNoCalcolo { get; set; }

        [DataMember]
        public Entity.DatiNoCalcolo DatiNoCalcolo { get; set; }

        [DataMember]
        public long? IdRecordNoCalcolo { get; set; }

        [DataMember]
        public Liquidazione.BLCommon.Utility.CategoriaFondoPI? CategoriaPI { get; set; }
    }
}