using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaRichiestaDomanda
    {
        #region public data member
        [DataMember]
        public long NumeroDomanda { get; set; }
        [DataMember]
        public byte? ProgStorico { get; set; }
        #endregion public data member
    }
}