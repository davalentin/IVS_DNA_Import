using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaControlliDinamici
    {
        #region private properties
        private Dictionary<string, string> _ListaVersioni;
        private DateTime? _DataSistema;
        private string _NomeControllo;
        private string _ValoreControllo;
        #endregion private properties

        #region public data member
        [DataMember]
        public Dictionary<string, string> ListaVersioni { get { return _ListaVersioni;} set { _ListaVersioni = value;} }
        [DataMember]
        public DateTime? DataSistema { get { return _DataSistema; } set { _DataSistema = value; } }
        [DataMember]
        public string NomeControllo { get { return _NomeControllo; } set { _NomeControllo = value; } }
        [DataMember]
        public string ValoreControllo { get { return _ValoreControllo; } set { _ValoreControllo = value; } }
        #endregion public data member
    }
}
