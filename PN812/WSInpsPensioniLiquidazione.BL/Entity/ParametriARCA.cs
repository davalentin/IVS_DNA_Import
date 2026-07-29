using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class ParametriARCA
    {
        #region private properties
        private string _Applicazione;
        private string _Matricola;
        private short _SedeOperatore;
        private string _Provenienza;
        private string _Ruolo;
        private string _CodiceFiscaleRichiedente;
        #endregion private properties

        #region public properties
        public string Applicazione { get { return _Applicazione; } set { _Applicazione = value; } }
        public string Matricola { get { return _Matricola; } set { _Matricola = value; } }
        public short SedeOperatore { get { return _SedeOperatore; } set { _SedeOperatore = value; } }
        public string Provenienza { get { return _Provenienza; } set { _Provenienza = value; } }
        public string Ruolo { get { return _Ruolo; } set { _Ruolo = value; } }
        public string CodiceFiscaleRichiedente { get { return _CodiceFiscaleRichiedente; } set { _CodiceFiscaleRichiedente = value; } }
        #endregion public properties
    }
}
