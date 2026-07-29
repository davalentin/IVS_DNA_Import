using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneCi.Entity
{
    public class DatiOpzione
    {
        #region private properties

        #region istruttoria
        private DateTime? _DecorrenzaOpzione;
        private DateTime? _DataDomandaOpzione;
        private byte? _CodiceOpzioneRiliquidazione;
        #endregion istruttoria

        #region PensioniCiDatiGenerici
        private DateTime? _DecorrenzaArt2Dpcm;
        #endregion PensioniCiDatiGenerici

        #endregion private properties

        #region public properties

        #region istruttoria
        public DateTime? DecorrenzaOpzione { get { return _DecorrenzaOpzione; } set { _DecorrenzaOpzione = value; } }
        public DateTime? DataDomandaOpzione { get { return _DataDomandaOpzione; } set { _DataDomandaOpzione = value; } }
        public byte? CodiceOpzioneRiliquidazione { get { return _CodiceOpzioneRiliquidazione; } set { _CodiceOpzioneRiliquidazione = value; } }
        #endregion istruttoria

        #region PensioniDatiGenerici
        public DateTime? DecorrenzaArt2Dpcm { get { return _DecorrenzaArt2Dpcm; } set { _DecorrenzaArt2Dpcm = value; } }
        #endregion PensioniDatiGenerici

        #endregion public properties

        public bool IsDatiOpzioneIstruttoriaNull()
        {
            if (!this._DecorrenzaOpzione.HasValue && !this._DataDomandaOpzione.HasValue && !this._CodiceOpzioneRiliquidazione.HasValue)
                return true;
            else
                return false;
        }

        public bool IsDatiOpzionePensioniCiDatiGenericiNull()
        {
            if (!this._DecorrenzaArt2Dpcm.HasValue)
                return true;
            else
                return false;
        }
    }
}
