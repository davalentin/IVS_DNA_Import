using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class DatiBenefici
    {
        #region private properties

        private DateTime? _DecorrenzaMaggiorazioneSociale;
        private DateTime? _CessazioneMaggiorazioneSociale;
        private int? _NSettimaneBeneficio;
        private string _TipoSettimaneBeneficio;
        private short? _SettimaneBeneficioAA;
        private short? _SettimaneBeneficioMM;
        private short? _SettimaneBeneficioGG;
        private List<OneriTerrorismo> _OneriTerrorismo;
        private short? _SettAnzContribPost311295;
        private DateTime? _DataNonVedenteDal;

        #endregion private properties

        #region public properties

        public DateTime? DecorrenzaMaggiorazioneSociale { get { return _DecorrenzaMaggiorazioneSociale; } set { _DecorrenzaMaggiorazioneSociale = value; } }
        public DateTime? CessazioneMaggiorazioneSociale { get { return _CessazioneMaggiorazioneSociale; } set { _CessazioneMaggiorazioneSociale = value; } }
        public int? NSettimaneBeneficio { get { return _NSettimaneBeneficio; } set { _NSettimaneBeneficio = value; } }
        public string TipoSettimaneBeneficio { get { return _TipoSettimaneBeneficio; } set { _TipoSettimaneBeneficio = value; } }
        public short? SettimaneBeneficioAA { get { return _SettimaneBeneficioAA; } set { _SettimaneBeneficioAA = value; } }
        public short? SettimaneBeneficioMM { get { return _SettimaneBeneficioMM; } set { _SettimaneBeneficioMM = value; } }
        public short? SettimaneBeneficioGG { get { return _SettimaneBeneficioGG; } set { _SettimaneBeneficioGG = value; } }
        public List<OneriTerrorismo> ListOneriTerrorismo { get { return _OneriTerrorismo; } set { _OneriTerrorismo = value; } }
        public short? SettAnzContribPost311295 { get { return _SettAnzContribPost311295; } set { _SettAnzContribPost311295 = value; } }
        public DateTime? DataNonVedenteDal { get { return _DataNonVedenteDal; } set { _DataNonVedenteDal = value; } }

        #endregion public properties

        public bool IsDatiBeneficiNull()
        {
            if (this._DecorrenzaMaggiorazioneSociale.HasValue || !string.IsNullOrEmpty(this._TipoSettimaneBeneficio) ||
                this._CessazioneMaggiorazioneSociale.HasValue || this._NSettimaneBeneficio.HasValue ||
                this._SettimaneBeneficioAA.HasValue || this._SettimaneBeneficioMM.HasValue || this._SettimaneBeneficioGG.HasValue || (this._OneriTerrorismo != null && this._OneriTerrorismo.Count > 0) ||
                this._SettAnzContribPost311295.HasValue)
                return false;
            else
                return true;
        }

        public class OneriTerrorismo
        {
            #region Private
            private long _IdPensione;
            private int? _CodiceAltroFondo;
            private decimal? _Importo;
            private int? _Progressivo;
            #endregion Private

            #region Public
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public int? CodiceAltroFondo { get { return _CodiceAltroFondo; } set { _CodiceAltroFondo = value; } }
            public decimal? Importo { get { return _Importo; } set { _Importo = value; } }
            public int? Progressivo { get { return _Progressivo; } set { _Progressivo = value; } }
            #endregion Public
        }
    }
}
