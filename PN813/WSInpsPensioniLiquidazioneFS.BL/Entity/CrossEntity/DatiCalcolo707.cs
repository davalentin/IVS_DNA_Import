using INPS.Pensioni.Liquidazione.BLCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class DatiCalcolo707
    {
        
        #region private

        private decimal? _PensioneAnnuaLorda707;
        private List<DatiServizioUtile707> _LDatiServizioUtile707;
        //ENG - PL Reversibilita 024
        private bool? _IsPensioneAnnuaLorda707DaPrelievo { get; set; }

        #endregion private

        #region public

        public byte? Semaforo { get; set; }
        public decimal? PensioneAnnuaLorda707 { get { return _PensioneAnnuaLorda707; } set { _PensioneAnnuaLorda707 = value; } }
        public List<DatiServizioUtile707> LDatiServizioUtile707 { get { return _LDatiServizioUtile707; } set { _LDatiServizioUtile707 = value; } }
        public bool? IsPensioneAnnuaLorda707DaPrelievo { get { return _IsPensioneAnnuaLorda707DaPrelievo; } set { _IsPensioneAnnuaLorda707DaPrelievo = value; } }
        #endregion public

        public class DatiServizioUtile707
        {
            #region private properties
            private string _Quota;

            private System.Nullable<short> _ServizioUtileAA;

            private System.Nullable<short> _ServizioUtileMM;

            private System.Nullable<short> _ServizioUtileGG;

            private System.Nullable<short> _ServizioUtileCessazioneAA;

            private System.Nullable<short> _ServizioUtileCessazioneMM;

            private System.Nullable<short> _ServizioUtileCessazioneGG;

            private System.Nullable<decimal> _QuotaPensioneRetributivaAnnua;
            #endregion private properties

            #region public properties
            public string Quota { get { return _Quota; } set { _Quota = value; } }

            public System.Nullable<short> ServizioUtileAA { get { return _ServizioUtileAA; } set { _ServizioUtileAA = value; } }

            public System.Nullable<short> ServizioUtileMM { get { return _ServizioUtileMM; } set { _ServizioUtileMM = value; } }

            public System.Nullable<short> ServizioUtileGG { get { return _ServizioUtileGG; } set { _ServizioUtileGG = value; } }

            public System.Nullable<short> ServizioUtileCessazioneAA { get { return _ServizioUtileCessazioneAA; } set { _ServizioUtileCessazioneAA = value; } }

            public System.Nullable<short> ServizioUtileCessazioneMM { get { return _ServizioUtileCessazioneMM; } set { _ServizioUtileCessazioneMM = value; } }

            public System.Nullable<short> ServizioUtileCessazioneGG { get { return _ServizioUtileCessazioneGG; } set { _ServizioUtileCessazioneGG = value; } }

            public System.Nullable<decimal> QuotaPensioneRetributivaAnnua { get { return _QuotaPensioneRetributivaAnnua; } set { _QuotaPensioneRetributivaAnnua = value; } }
            #endregion public properties

            public bool IsNull()
            {
                if (string.IsNullOrEmpty(this._Quota) &&
                    !_ServizioUtileAA.HasValue &&
                    !_ServizioUtileMM.HasValue &&
                    !_ServizioUtileGG.HasValue &&
                    !_ServizioUtileCessazioneAA.HasValue &&
                    !_ServizioUtileCessazioneMM.HasValue &&
                    !_ServizioUtileCessazioneGG.HasValue &&
                    !_QuotaPensioneRetributivaAnnua.HasValue)
                    return true;

                return false;
            }
        }
    }


}
