using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class CodiceSpecifico
    {
        public CodiceSpecifico()
        {
        }

        #region public properties
        public byte? Id { get { return _Id; } set { _Id = value; } }
        public char? TraduzioneGp { get { return _TraduzioneGp; } set { _TraduzioneGp = value; } }
        public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
        public char? TipoPensione { get { return _TipoPensione; } set { _TipoPensione = value; } }
        public byte? TipoSelezionabile { get { return _TipoSelezionabile; } set { _TipoSelezionabile = value; } }
        public string Fondo { get { return _Fondo; } set { _Fondo = value; } }
        public char? EnteFondo { get; set; }
        #endregion

        #region private properties
        private byte? _Id;
        private char? _TraduzioneGp;
        private string _Descrizione;
        private char? _TipoPensione;
        private byte? _TipoSelezionabile;
        private string _Fondo;
        #endregion
    }
}
