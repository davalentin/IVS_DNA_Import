using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class DatiFamiliari
    {        
        #region private properties
        private GestioneFamiliari.Familiare _Familiare;
        private List<GestioneFamiliari.CodMaggFamiliari> _ElencoCodMaggFamiliari;
        #endregion private properties

        #region public properties
        public GestioneFamiliari.Familiare Familiare { get { return _Familiare; } set { _Familiare = value; } }
        public List<GestioneFamiliari.CodMaggFamiliari> ElencoCodMaggFamiliari { get { return _ElencoCodMaggFamiliari; } set { _ElencoCodMaggFamiliari = value; } }
        #endregion public properties
    }
}
