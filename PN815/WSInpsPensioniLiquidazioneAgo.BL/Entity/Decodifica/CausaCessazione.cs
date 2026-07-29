using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class CausaCessazione
    {
        public CausaCessazione()
        { }

        #region Private Properties

        private long _Id;
        private char? _TipoPensione;
        private string _TraduzioneSuGP;
        private DateTime? _InizioValidita;
        private DateTime? _FineValidita;
        private string _Descrizione;
        private string _Fondo;

        #endregion

        #region Public Properties

        public long Id { get { return _Id; } set { _Id = value; } }
        public char? TipoPensione { get { return _TipoPensione; } set { _TipoPensione = value; } }
        public string TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }
        public DateTime? InizioValidita { get { return _InizioValidita; } set { _InizioValidita = value; } }
        public DateTime? FineValidita { get { return _FineValidita; } set { _FineValidita = value; } }
        public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
        public string Fondo { get { return _Fondo; } set { _Fondo = value; } }

        #endregion
    }
}
