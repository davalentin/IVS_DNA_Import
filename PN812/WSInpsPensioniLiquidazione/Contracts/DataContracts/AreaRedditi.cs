using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Data;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaRedditi
    {
        #region private properties
        private GestioneRedditi.AreaRedditi _Redditi;
        #endregion private properties

        #region public data member
        [DataMember]
        public GestioneRedditi.AreaRedditi Redditi { get { return _Redditi; } set { _Redditi = value; } }
        #endregion public data member
    }
}