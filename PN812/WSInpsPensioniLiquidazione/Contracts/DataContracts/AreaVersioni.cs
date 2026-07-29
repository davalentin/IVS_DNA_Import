using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaVersioni
    {
        #region private properties
        private Dictionary<string, string> _ListaVersioni;
        #endregion private properties

        #region public data member
        [DataMember]
        public Dictionary<string, string> ListaVersioni { get { return _ListaVersioni; } set { _ListaVersioni = value; } }
        #endregion public data member
    }
}