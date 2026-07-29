using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaStoricoDataLimitePrepensionementoLetteraB
    {
        [DataMember]
        public List<StoricoDataLimiteDomandePrepensionementoLetteraB> ListStoricoDataLimiteDomandePrepensionementoLetteraB { get; set; }

        public AreaStoricoDataLimitePrepensionementoLetteraB()
        {
            this.ListStoricoDataLimiteDomandePrepensionementoLetteraB = new List<StoricoDataLimiteDomandePrepensionementoLetteraB>();
        }

        #region Nested Class

        [DataContract]
        public class StoricoDataLimiteDomandePrepensionementoLetteraB   
        {
            [DataMember]
            public long Id { get; set; }

            [DataMember]
            public DateTime DataModifica { get; set; }

            [DataMember]
            public DateTime DataLimitePoligraficiLetteraB { get; set; }

            [DataMember]
            public string Matricola { get; set; }

            [DataMember]
            public string Note { get; set; }
        }

        #endregion
    }
}