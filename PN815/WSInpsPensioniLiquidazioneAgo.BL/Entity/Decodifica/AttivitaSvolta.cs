using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class AttivitaSvolta
    {
        public AttivitaSvolta()
        {
        }

        internal AttivitaSvolta(GestioneDecodifica.AttivitaSvolta attivitaSvolta)
        {
            this._Id = attivitaSvolta.Id;
            this._Descrizione = attivitaSvolta.Descrizione;
            this._Fondo = attivitaSvolta.Fondo;
            this._TraduzioneSuGp = attivitaSvolta.TraduzioneSuGp;
            this._InizioValidita = attivitaSvolta.InizioValidita.HasValue ? attivitaSvolta.InizioValidita : null;
            this._FineValidita = attivitaSvolta.FineValidita.HasValue ? attivitaSvolta.FineValidita : null;
            this._LimiteEta = attivitaSvolta.LimiteEta.HasValue ? attivitaSvolta.LimiteEta : null;
            this._LimiteServizio = attivitaSvolta.LimiteServizio.HasValue ? attivitaSvolta.LimiteServizio : null;
            this._PersonaleViaggiante = attivitaSvolta.PersonaleViaggiante.HasValue ? attivitaSvolta.PersonaleViaggiante : null;
        }

        #region private properties
        private string _Id;
        private string _Descrizione;
        private string _Fondo;
        private string _TraduzioneSuGp;
        private System.Nullable<System.DateTime> _InizioValidita;
        private System.Nullable<System.DateTime> _FineValidita;
        private System.Nullable<byte> _LimiteEta;
        private System.Nullable<byte> _LimiteServizio;
        private System.Nullable<bool> _PersonaleViaggiante;
        #endregion

        #region public data member
        public string Id { get { return _Id; } set { _Id = value; } }
        public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
        public string Fondo { get { return _Fondo; } set { _Fondo = value; } }
        public string TraduzioneSuGp { get { return _TraduzioneSuGp; } set { _TraduzioneSuGp = value; } }
        public System.Nullable<System.DateTime> InizioValidita { get { return _InizioValidita; } set { _InizioValidita = value; } }
        public System.Nullable<System.DateTime> FineValidita { get { return _FineValidita; } set { _FineValidita = value; } }
        public System.Nullable<byte> LimiteEta { get { return _LimiteEta; } set { _LimiteEta = value; } }
        public System.Nullable<byte> LimiteServizio { get { return _LimiteServizio; } set { _LimiteServizio = value; } }
        public System.Nullable<bool> PersonaleViaggiante { get { return _PersonaleViaggiante; } set { _PersonaleViaggiante = value; } }
        #endregion
    }
}
