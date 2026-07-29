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
    public class AreaRichiestaBonus
    {
        #region private properties
        private GestioneRichiestaBonus.AreaRichiestaBonus _RichiestaBonus;
        #endregion private properties

        #region public data member
        [DataMember]
        public GestioneRichiestaBonus.AreaRichiestaBonus RichiestaBonus { get { return _RichiestaBonus; } set { _RichiestaBonus = value; } }
        #endregion public data member
    }
}