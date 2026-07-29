using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneCi.Entity
{
    public class DecodificaLegge44997
    {
        #region public properties

        public long Id { get { return _Id; } set { _Id = value; } }
        public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

        #endregion public properties

        #region private properties

        private long _Id;
        private string _Descrizione;

        #endregion private properties
    }
}
