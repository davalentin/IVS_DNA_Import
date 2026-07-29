using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Entity.Oneri
{
    public class DatiPrepensionamento
    {
        #region private properties
        private int? _CodiceLegge;
        private int? _SettimaneUtiliDiritto;
        private int? _SettimaneUtiliMisura;
        private int? _SettimaneMaggioreAnzianita;
        private decimal? _OnereMancataContribuzione;
        private long? _CodiceAzienda;
        private DateTime? _CessazioneBeneficioPrepensionamento;
        private int? _SettimaneAmianto;
        private DateTime? _CessazioneAmianto;
        #endregion private properties

        #region public properties
        public int? CodiceLegge { get { return _CodiceLegge; } set { _CodiceLegge = value; } }
        public int? SettimaneUtiliDiritto { get { return _SettimaneUtiliDiritto; } set { _SettimaneUtiliDiritto = value; } }
        public int? SettimaneUtiliMisura { get { return _SettimaneUtiliMisura; } set { _SettimaneUtiliMisura = value; } }
        public int? SettimaneMaggioreAnzianita { get { return _SettimaneMaggioreAnzianita; } set { _SettimaneMaggioreAnzianita = value; } }
        public decimal? OnereMancataContribuzione { get { return _OnereMancataContribuzione; } set { _OnereMancataContribuzione = value; } }
        public long? CodiceAzienda { get { return _CodiceAzienda; } set { _CodiceAzienda = value; } }
        public DateTime? CessazioneBeneficioPrepensionamento { get { return _CessazioneBeneficioPrepensionamento; } set { _CessazioneBeneficioPrepensionamento = value; } }
        public int? SettimaneAmianto { get { return _SettimaneAmianto; } set { _SettimaneAmianto = value; } }
        public DateTime? CessazioneAmianto { get { return _CessazioneAmianto; } set { _CessazioneAmianto = value; } }
        #endregion public properties

        public bool IsDatiPrepensionamentoNull()
        {
            if (!this._CodiceLegge.HasValue && !this._SettimaneUtiliDiritto.HasValue && !this._SettimaneUtiliMisura.HasValue && !this._SettimaneMaggioreAnzianita.HasValue && !this._OnereMancataContribuzione.HasValue &&
                !this._CodiceAzienda.HasValue && !this._CessazioneBeneficioPrepensionamento.HasValue && !this._SettimaneAmianto.HasValue && !this._CessazioneAmianto.HasValue)
                return true;

            return false;
        }
    }
}
