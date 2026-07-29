using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DatiOpzione
    {
        #region private properties

        #region istruttoria
        private DateTime? _DecorrenzaOpzione;
        private DateTime? _DataDomandaOpzione;

        #endregion istruttoria

        #endregion private properties

        #region public properties

        #region istruttoria
        public DateTime? DecorrenzaOpzione { get { return _DecorrenzaOpzione; } set { _DecorrenzaOpzione = value; } }
        public DateTime? DataDomandaOpzione { get { return _DataDomandaOpzione; } set { _DataDomandaOpzione = value; } }

        #endregion istruttoria

        #endregion public properties

        public bool IsDatiOpzioneNull()
        {
            if (!this._DecorrenzaOpzione.HasValue && !this._DataDomandaOpzione.HasValue)
                return true;
            else
                return false;
        }
    }
}
