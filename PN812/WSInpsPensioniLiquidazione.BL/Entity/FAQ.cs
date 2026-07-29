using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class FAQ
    {
        #region private properties
        private long _Id;
        private string _Domanda;
        private string _Risposta;
        private string _TipoApp;
        private string _Codice;
        private string _Tipologia;
        private bool _Visibilita;
        #endregion private properties

        #region public properties
        public long Id { get { return _Id; } set { _Id = value; } }
        public string Domanda { get { return _Domanda; } set { _Domanda = value; } }
        public string Risposta { get { return _Risposta; } set { _Risposta = value; } }
        public string TipoApp { get { return _TipoApp; } set { _TipoApp = value; } }
        public string Codice { get { return _Codice; } set { _Codice = value; } }
        public string Tipologia { get { return _Tipologia; } set { _Tipologia = value; } }
        public bool Visibilita { get { return _Visibilita; } set { _Visibilita = value; } }
        #endregion public properties
    }
}
