using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class DatiLegge460
    {
        public DatiLegge460()
        { }

        #region Private Properties

        private int? _SiglaCategoria;
        private short? _CodiceSede;
        private string _NCertificato;
        private int? _NMesiRiscattati;
        private int? _NMesiTotali;
        private DateTime? _DecorrenzaSecondaria;

        #endregion

        #region Public Properties

        public int? SiglaCategoria { get { return _SiglaCategoria; } set { _SiglaCategoria = value; } }
        public short? CodiceSede { get { return _CodiceSede; } set { _CodiceSede = value; } }
        public string NCertificato { get { return _NCertificato; } set { _NCertificato = value; } }
        public int? NMesiRiscattati { get { return _NMesiRiscattati; } set { _NMesiRiscattati = value; } }
        public int? NMesiTotali { get { return _NMesiTotali; } set { _NMesiTotali = value; } }
        public DateTime? DecorrenzaSecondaria { get { return _DecorrenzaSecondaria; } set { _DecorrenzaSecondaria = value; } }


        #endregion

        public bool IsDatiLegge460Null()
        {
            if (!this._SiglaCategoria.HasValue && !this._CodiceSede.HasValue && string.IsNullOrEmpty(this._NCertificato) &&
                !this._NMesiRiscattati.HasValue && !this._NMesiTotali.HasValue && !this._DecorrenzaSecondaria.HasValue)
                return true;
            else
                return false;
        }
    }
}
