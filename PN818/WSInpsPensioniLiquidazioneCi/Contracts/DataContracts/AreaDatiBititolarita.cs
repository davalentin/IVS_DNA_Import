using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Data;

namespace INPS.Pensioni.LiquidazioneCi.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaDatiBititolarita
    {
        #region private properties

        private List<Entity.AltraPensione> _ElencoAltraPensione;
        private List<GestioneBititolarita.DecodificaEnte> _ElencoDecodificaEnte;
        private List<GestioneBititolarita.DecCatEnte> _ElencoCatEnte;

        #endregion private properties

        #region public data member
        [DataMember]
        public List<Entity.AltraPensione> ElencoAltraPensione { get { return _ElencoAltraPensione; } set { _ElencoAltraPensione = value; } }
        [DataMember]
        public List<GestioneBititolarita.DecodificaEnte> ElencoDecodificaEnte { get { return _ElencoDecodificaEnte; } set { _ElencoDecodificaEnte = value; } }
        [DataMember]
        public List<GestioneBititolarita.DecCatEnte> ElencoCatEnte { get { return _ElencoCatEnte; } set { _ElencoCatEnte = value; } }
        #endregion public data member
    }
}
