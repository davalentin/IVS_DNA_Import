using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneCi.Entity
{
    public class DatiBenefici
    {
        #region private properties

        private int? _NSettimaneBeneficio;
        private string _TipoSettimaneBeneficio;
        private List<OneriTerrorismo> _OneriTerrorismo;
        private int? _NSettimaneIncremento1Percento;
        private int? _NSettimaneIncremento05Percento;
        private byte? _Sentenza495240;
        private short? _SettAnzContribPost311295;
        private DateTime? _DataNonVedenteDal;

        #endregion private properties

        #region public properties

        public int? NSettimaneBeneficio { get { return _NSettimaneBeneficio; } set { _NSettimaneBeneficio = value; } }
        public string TipoSettimaneBeneficio { get { return _TipoSettimaneBeneficio; } set { _TipoSettimaneBeneficio = value; } }
        public List<OneriTerrorismo> ListOneriTerrorismo { get { return _OneriTerrorismo; } set { _OneriTerrorismo = value; } }
        public int? NSettimaneIncremento1Percento { get { return _NSettimaneIncremento1Percento; } set { _NSettimaneIncremento1Percento = value; } }
        public int? NSettimaneIncremento05Percento { get { return _NSettimaneIncremento05Percento; } set { _NSettimaneIncremento05Percento = value; } }
        public byte? Sentenza495240 { get { return _Sentenza495240; } set { _Sentenza495240 = value; } }
        public short? SettAnzContribPost311295 { get { return _SettAnzContribPost311295; } set { _SettAnzContribPost311295 = value; } }
        public DateTime? DataNonVedenteDal { get { return _DataNonVedenteDal; } set { _DataNonVedenteDal = value; } }

        #endregion public properties

        public bool IsDatiBeneficiNull()
        {
            if (!string.IsNullOrEmpty(this._TipoSettimaneBeneficio) || this._NSettimaneBeneficio.HasValue || (this._OneriTerrorismo != null && this._OneriTerrorismo.Count > 0) ||
                this._NSettimaneIncremento1Percento.HasValue || this._NSettimaneIncremento05Percento.HasValue || this._Sentenza495240.HasValue || this._SettAnzContribPost311295.HasValue)
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
