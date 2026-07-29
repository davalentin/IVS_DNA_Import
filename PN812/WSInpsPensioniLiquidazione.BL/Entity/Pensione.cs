using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class Pensione
    {
        #region private properties
        private string _Certificato;
        private string _Categoria;
        private string _Sede;
        private DateTime? _DataCalcolo;
        private char? _TipoComponente;
        private string _Eliminazione;
        private bool _IsRicostituibile;
        #endregion private properties

        #region public data member
        public string Certificato { get { return _Certificato; } set { _Certificato = value; } }
        public string Categoria { get { return _Categoria; } set { _Categoria = value; } }
        public string Sede { get { return _Sede; } set { _Sede = value; } }
        public DateTime? DataCalcolo { get { return _DataCalcolo; } set { _DataCalcolo = value; } }
        public char? TipoComponente { get { return _TipoComponente; } set { _TipoComponente = value; } }
        public string Eliminazione { get { return _Eliminazione; } set { _Eliminazione = value; } }
        public bool IsRicostituibile { get { return _IsRicostituibile; } set { _IsRicostituibile = value; } }
        #endregion public data member
    }
}
