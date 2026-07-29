using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class CodiceArt22
    {
        #region public properties

        public byte Id { get { return _Id; } set { _Id = value; } }
        public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
        public byte? TipoSelezionabile { get { return _TipoSelezionabile; } set { _TipoSelezionabile = value; } }
        public string Fondo { get { return _Fondo; } set { _Fondo = value; } }

        #endregion public properties

        #region private properties

        private byte _Id;
        private string _Descrizione;
        private byte? _TipoSelezionabile;
        private string _Fondo;

        #endregion private properties
    }
}
