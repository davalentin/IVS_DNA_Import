using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DecModalitaLiquidazione
    {
        #region public properties

        public string ValoreAggPeco { get { return _ValoreAggPeco; } set { _ValoreAggPeco = value; } }
        public char TraduzioneGp { get { return _TraduzioneGp; } set { _TraduzioneGp = value; } }
        public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

        #endregion public properties

        #region private properties

        private string _ValoreAggPeco;
        private char _TraduzioneGp;
        private string _Descrizione;
        
        #endregion private properties
    }
}
