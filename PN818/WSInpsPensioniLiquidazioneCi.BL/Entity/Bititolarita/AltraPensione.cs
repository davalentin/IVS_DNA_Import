using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneCi.Entity
{
    public class AltraPensione
    {
        #region private properties
        private System.Nullable<long> _IdPensione;
        private string _Categoria;
        private System.Nullable<byte> _Ente;
        private System.Nullable<int> _Certificato;
        private System.Nullable<System.DateTime> _Decorrenza;
        private System.Nullable<System.DateTime> _Cessazione;
        private System.Nullable<char> _CodiceUC;
        private System.Nullable<char> _CodiceImporto;
        #endregion private properties

        #region public properties
        public System.Nullable<long> IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
        public string Categoria { get { return _Categoria; } set { _Categoria = value; } }
        public System.Nullable<byte> Ente { get { return _Ente; } set { _Ente = value; } }
        public System.Nullable<int> Certificato { get { return _Certificato; } set { _Certificato = value; } }
        public System.Nullable<char> CodiceUC { get { return _CodiceUC; } set { _CodiceUC = value; } }
        public System.Nullable<char> CodiceImporto { get { return _CodiceImporto; } set { _CodiceImporto = value; } }
        public System.Nullable<System.DateTime> Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
        public System.Nullable<System.DateTime> Cessazione { get { return _Cessazione; } set { _Cessazione = value; } }
        #endregion public properties

        public bool IsDatiAltraPensioneNull()
        {
            if (string.IsNullOrEmpty(this._Categoria) && this._Ente == null && this._Certificato == null &&
                this._Decorrenza == null && this._Cessazione == null && this._CodiceUC == null && this._CodiceImporto == null)
                return true;
            else
                return false;
        }
    }
}
