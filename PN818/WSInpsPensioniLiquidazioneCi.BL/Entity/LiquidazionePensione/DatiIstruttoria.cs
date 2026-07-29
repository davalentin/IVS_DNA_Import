using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneCi.Entity
{
    public class DatiIstruttoria
    {
        #region private properties

        #region Istruttoria
         private byte? _Legge44997;
         private long? _CodiceParticolareSoggettoDerogato;
         private short? _CodiceContrattoEquiparato;
         private short? _CodiceLivelloEquip;
        #endregion Istruttoria

        #region PensioniDatiGenerici

        private bool _RiduzioneRetributiva;
         private System.Nullable<decimal> _RiduzioneRetributivaPercentuale;

        #endregion PensioniDatiGenerici

        #endregion private properties

        #region public properties

        #region Istruttoria
        public byte? Legge44997 { get { return _Legge44997; } set { _Legge44997 = value; } }
        public long? CodiceParticolareSoggettoDerogato { get { return _CodiceParticolareSoggettoDerogato; } set { _CodiceParticolareSoggettoDerogato = value; } }
        public short? CodiceContrattoEquiparato { get { return _CodiceContrattoEquiparato; } set { _CodiceContrattoEquiparato = value; } }
        public short? CodiceLivelloEquip { get { return _CodiceLivelloEquip; } set { _CodiceLivelloEquip = value; } }
        #endregion Istruttoria

        #region PensioniDatiGenerici

        public bool RiduzioneRetributiva { get { return _RiduzioneRetributiva; } set { _RiduzioneRetributiva = value; } }
        public System.Nullable<decimal> RiduzioneRetributivaPercentuale { get { return _RiduzioneRetributivaPercentuale; } set { _RiduzioneRetributivaPercentuale = value; } }

        #endregion PensioniDatiGenerici

        public bool IsDatiIstruttoriaIstruttoriaNull()
        {
            if (!this._Legge44997.HasValue && !this._CodiceParticolareSoggettoDerogato.HasValue &&
                !this._CodiceContrattoEquiparato.HasValue && !this._CodiceLivelloEquip.HasValue)
                return true;
            else
                return false;
        }

        public bool IsDatiIstruttoriaDatiGenericiNull()
        {
            if (!this._RiduzioneRetributiva && !this._RiduzioneRetributivaPercentuale.HasValue)
                return true;
            else
                return false;
        }

        #endregion public properties

    }
}


