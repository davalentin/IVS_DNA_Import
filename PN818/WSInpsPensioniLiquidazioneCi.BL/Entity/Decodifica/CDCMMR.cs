using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneCi.Entity
{
    public class CDCMMR
    {
        #region public properties
        public byte Id { get { return _Id; } set { _Id = value; } }

        public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
        #endregion public properties

        #region private properties
        private byte _Id;

        private string _Descrizione;
        #endregion private properties
    }
}
