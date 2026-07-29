using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class Aggiornamenti
    {
        #region private properties
        private long _Id;
        private string _Titolo;
        private string _Testo;
        private bool _Attivo;
        private string _Tipologia;
        private System.DateTime _TimeStamp;
        #endregion private properties

        #region public properties
        public long Id { get { return _Id; } set { _Id = value; } }
        public string Titolo { get { return _Titolo; } set { _Titolo = value; } }
        public string Testo { get { return _Testo; } set { _Testo = value; } }
        public bool Attivo { get { return _Attivo; } set { _Attivo = value; } }
        public string Tipologia { get { return _Tipologia; } set { _Tipologia = value; } }
        public System.DateTime TimeStamp { get { return _TimeStamp; } set { _TimeStamp = value; } }
        #endregion public properties
    }
}
