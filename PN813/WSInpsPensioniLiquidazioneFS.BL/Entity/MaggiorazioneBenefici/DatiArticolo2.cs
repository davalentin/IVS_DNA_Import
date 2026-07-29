using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class DatiArticolo2
    {
        #region private

        private DateTime? _DataInzioBeneficioArt2;
        private DateTime? _DataFineBeneficioArt2;


        #endregion private

        #region public

        public DateTime? DataInzioBeneficioArt2 { get { return _DataInzioBeneficioArt2; } set { _DataInzioBeneficioArt2 = value; } }
        public DateTime? DataFineBeneficioArt2 { get { return _DataFineBeneficioArt2; } set { _DataFineBeneficioArt2 = value; } }

        #endregion public

        public bool IsDatiArticolo2Null()
        {
            if (!this._DataInzioBeneficioArt2.HasValue &&
                !this._DataFineBeneficioArt2.HasValue)
                return true;
            else
                return false;
        }
    }
}
