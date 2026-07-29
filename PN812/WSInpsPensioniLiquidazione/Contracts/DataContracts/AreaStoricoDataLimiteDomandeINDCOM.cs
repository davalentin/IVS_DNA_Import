using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaStoricoDataLimiteDomandeINDCOM
    {
        [DataMember]
        public List<StoricoDataLimiteDomandeINDCOM> ListStoricoDataLimiteDomandeINDCOM { get; set; }

        public AreaStoricoDataLimiteDomandeINDCOM()
        {
            this.ListStoricoDataLimiteDomandeINDCOM = new List<StoricoDataLimiteDomandeINDCOM>();
        }

        #region Nested Class

        [DataContract]
        public class StoricoDataLimiteDomandeINDCOM
        {
            [DataMember]
            public long Id { get; set; }

            [DataMember]
            public DateTime DataModifica { get; set; }

            [DataMember]
            public DateTime DataLimiteDomandeINDCOM { get; set; }

            [DataMember]
            public string Matricola { get; set; }

            [DataMember]
            public string Note { get; set; }
        }


        #endregion
    }
}