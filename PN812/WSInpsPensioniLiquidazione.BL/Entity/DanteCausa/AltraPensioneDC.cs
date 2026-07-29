using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class AltraPensioneDC
    {
        private string _CategoriaAltraPensione;
        private System.Nullable<short> _EnteAltraPensione;
        private System.Nullable<char> _CodiceUCAltraPensione;
        private System.Nullable<char> _CodiceImportoAltraPensione;
        private System.Nullable<System.DateTime> _DecorrenzaAltraPensione;
        private System.Nullable<System.DateTime> _CessazioneAltraPensione;
        private string _NaturaPensioneAltraPensione;

        public string CategoriaAltraPensione { get { return _CategoriaAltraPensione; } set { _CategoriaAltraPensione = value; } }
        public System.Nullable<short> EnteAltraPensione { get { return _EnteAltraPensione; } set { _EnteAltraPensione = value; } }
        public System.Nullable<char> CodiceUCAltraPensione { get { return _CodiceUCAltraPensione; } set { _CodiceUCAltraPensione = value; } }
        public System.Nullable<char> CodiceImportoAltraPensione { get { return _CodiceImportoAltraPensione; } set { _CodiceImportoAltraPensione = value; } }
        public System.Nullable<System.DateTime> DecorrenzaAltraPensione { get { return _DecorrenzaAltraPensione; } set { _DecorrenzaAltraPensione = value; } }
        public System.Nullable<System.DateTime> CessazioneAltraPensione { get { return _CessazioneAltraPensione; } set { _CessazioneAltraPensione = value; } }
        public string NaturaPensioneAltraPensione { get { return _NaturaPensioneAltraPensione; } set { _NaturaPensioneAltraPensione = value; } }

        public bool IsAllDatiAltraPensioneDCNull()
        {
            if (string.IsNullOrEmpty(this._CategoriaAltraPensione) && this._EnteAltraPensione == null && this._CodiceUCAltraPensione == null &&
                this._CodiceImportoAltraPensione == null && this._DecorrenzaAltraPensione == null && !this._CessazioneAltraPensione.HasValue)
                return true;
            else
                return false;
        }

        public bool IsDatiAltraPensioneDCObbligatoriNull()
        {
            if (string.IsNullOrEmpty(this._CategoriaAltraPensione) || this._EnteAltraPensione == null || this._CodiceUCAltraPensione == null ||
                this._CodiceImportoAltraPensione == null || this._DecorrenzaAltraPensione == null)
                return true;
            else
                return false;
        }
    }
}
