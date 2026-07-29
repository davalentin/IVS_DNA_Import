using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class DatiIstruttoriaINPDAP
    {
        #region private properties

        #region Istruttoria
        private long? _CodiceParticolareSoggettoDerogato;
        #endregion Istruttoria

        #region PensioneFondoDatiGenerici
        private bool _RiduzioneRetributiva;
        private decimal? _RiduzioneRetributivaPercentuale;
        #endregion PensioneFondoDatiGenerici

        #endregion private properties

        #region public properties

        #region Istruttoria
        public long? CodiceParticolareSoggettoDerogato { get { return _CodiceParticolareSoggettoDerogato; } set { _CodiceParticolareSoggettoDerogato = value; } }
        #endregion Istruttoria

        #region PensioneFondoDatiGenerici
        public bool RiduzioneRetributiva { get { return _RiduzioneRetributiva; } set { _RiduzioneRetributiva = value; } }
        public decimal? RiduzioneRetributivaPercentuale { get { return _RiduzioneRetributivaPercentuale; } set { _RiduzioneRetributivaPercentuale = value; } }
        #endregion PensioneFondoDatiGenerici

        #endregion public properties

        #region public methods
        public bool IsDatiIstruttoriaIstruttoriaNull()
        {
            if (!this._CodiceParticolareSoggettoDerogato.HasValue)
                return true;
            else
                return false;
        }

        public bool IsDatiIstruttoriaPensioneFondoDatiGenericiNull()
        {
            if (!this._RiduzioneRetributiva && !this._RiduzioneRetributivaPercentuale.HasValue)
                return true;
            else
                return false;
        }
        #endregion public methods
    }
}
