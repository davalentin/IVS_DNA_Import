using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DecodificaAziendaEditoria
    {
        public DecodificaAziendaEditoria()
        { }
        #region public properties

        public short Id { get { return _Id; } set { _Id = value; } }
        public string TraduzioneGp { get { return _TraduzioneGp; } set { _TraduzioneGp = value; } }
        public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

        #endregion

        #region private properties

        private short _Id;
        private string _TraduzioneGp;
        private string _Descrizione;

        #endregion
    }
}
