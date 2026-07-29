using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class DatiPersonaliParziali
    {
        #region private properties
        protected string _Nome;
        protected string _Cognome;
        protected DateTime? _DataNascita;
        protected string _CodiceFiscale;
        #endregion private properties

        #region public data member
        public string Nome { get { return _Nome; } set { _Nome = value; } }
        public string Cognome { get { return _Cognome; } set { _Cognome = value; } }
        public DateTime? DataNascita { get { return _DataNascita; } set { _DataNascita = value; } }
        public string CodiceFiscale { get { return _CodiceFiscale; } set { _CodiceFiscale = value; } }
        #endregion public data member
    }
}
