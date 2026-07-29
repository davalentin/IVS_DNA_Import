using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneCi.Entity
{
    public class DatiGenerici
    {
        #region private properties

        #region pensione
        private DateTime? _DecorrenzaCalcoloArretrati;
        private byte? _CodiceArretrati;
        private DateTime? _DataCompletezza;
        private DateTime? _DataInteressiLegali;
        private string _NaturaPensione;
        private byte? _CausaCarico;
        private bool? _TrasformazioneAOI;
        private bool? _Benefici;
        private bool? _ExCombattente;
        private DateTime? _DataInizioCalcolo;
        private DateTime? _DataRicezionePrenotazioneCentrale;
        private bool? _Maggiorazioni;
        private bool? _IsRichiestaBonus;
        private string _AnnoDecorrenzaBonus;
        #endregion pensione

        #region istruttoria
        private DateTime? _ScadenzaRevisioneSanitaria;
        private byte? _CodiceMobilita;
        private byte? _CodiceDomandaRicorso;
        private byte? _CodiceComunicazioneCampo4;
        private byte? _NRiconoscimentiInvalidita;
        private string _ModalitaLiquidazione;
        private bool? _TrattamentoDisagi;
        #endregion istruttoria

        #region Pagamento
        private bool? _TrattenutaInpdap;
        private DateTime? _DataRinunciaTrattenutaInpdap;
        #endregion Pagamento

        #region PensioneCiDatiDenerici
        private DateTime? _DecorrenzaBonus;
        #endregion PensioneCiDatiDenerici

        #region NuoveLiquidate
        private bool? _FlagContributiva;
        private bool? _FlagProvvisoria;
        #endregion NuoveLiquidate

        #endregion private properties

        #region public properties

        #region pensione
        public DateTime? DecorrenzaCalcoloArretrati { get { return _DecorrenzaCalcoloArretrati; } set { _DecorrenzaCalcoloArretrati = value; } }
        public byte? CodiceArretrati { get { return _CodiceArretrati; } set { _CodiceArretrati = value; } }
        public DateTime? DataCompletezza { get { return _DataCompletezza; } set { _DataCompletezza = value; } }
        public DateTime? DataInteressiLegali { get { return _DataInteressiLegali; } set { _DataInteressiLegali = value; } }
        public string NaturaPensione { get { return _NaturaPensione; } set { _NaturaPensione = value; } }
        public byte? CausaCarico { get { return _CausaCarico; } set { _CausaCarico = value; } }
        public bool? TrasformazioneAOI { get { return _TrasformazioneAOI; } set { _TrasformazioneAOI = value; } }
        public bool? Benefici { get { return _Benefici; } set { _Benefici = value; } }
        public bool? ExCombattente { get { return _ExCombattente; } set { _ExCombattente = value; } }
        public DateTime? DataInizioCalcolo { get { return _DataInizioCalcolo; } set { _DataInizioCalcolo = value; } }
        public DateTime? DataRicezionePrenotazioneCentrale { get { return _DataRicezionePrenotazioneCentrale; } set { _DataRicezionePrenotazioneCentrale = value; } }
        public bool? Maggiorazioni { get { return _Maggiorazioni; } set { _Maggiorazioni = value; } }
        public bool? IsRichiestaBonus { get { return _IsRichiestaBonus; } set { _IsRichiestaBonus = value; } }
        public string AnnoDecorrenzaBonus { get { return _AnnoDecorrenzaBonus; } set { _AnnoDecorrenzaBonus = value; } }

        #endregion pensione

        #region istruttoria
        public DateTime? ScadenzaRevisioneSanitaria { get { return _ScadenzaRevisioneSanitaria; } set { _ScadenzaRevisioneSanitaria = value; } }
        public byte? CodiceMobilita { get { return _CodiceMobilita; } set { _CodiceMobilita = value; } }
        public byte? CodiceDomandaRicorso { get { return _CodiceDomandaRicorso; } set { _CodiceDomandaRicorso = value; } }
        public byte? CodiceComunicazioneCampo4 { get { return _CodiceComunicazioneCampo4; } set { _CodiceComunicazioneCampo4 = value; } }
        public byte? NRiconoscimentiInvalidita { get { return _NRiconoscimentiInvalidita; } set { _NRiconoscimentiInvalidita = value; } }
        public string ModalitaLiquidazione { get { return _ModalitaLiquidazione; } set { _ModalitaLiquidazione = value; } }
        public bool? TrattamentoDisagi { get { return _TrattamentoDisagi; } set { _TrattamentoDisagi = value; } }
        #endregion istruttoria

        #region Pagamento
        public bool? TrattenutaInpdap { get { return _TrattenutaInpdap; } set { _TrattenutaInpdap = value; } }
        public DateTime? DataRinunciaTrattenutaInpdap { get { return _DataRinunciaTrattenutaInpdap; } set { _DataRinunciaTrattenutaInpdap = value; } }
        #endregion Pagamento

        #region PensioniDatiGenerici
        public DateTime? DecorrenzaBonus { get { return _DecorrenzaBonus; } set { _DecorrenzaBonus = value; } }
        #endregion PensioniDatiGenerici

        #region NuoveLiquidate
        public bool? FlagContributiva { get { return _FlagContributiva; } set { _FlagContributiva = value; } }
        public bool? FlagProvvisoria { get { return _FlagProvvisoria; } set { _FlagProvvisoria = value; } }
        #endregion NuoveLiquidate

        #endregion public properties


        public bool IsDatiGenericiPensioneNull()
        {
            if (!this._DataInteressiLegali.HasValue && !this._CodiceArretrati.HasValue &&
                !this._CausaCarico.HasValue && !this._DataInizioCalcolo.HasValue &&
                String.IsNullOrEmpty(this._NaturaPensione) && !this._DataInizioCalcolo.HasValue &&
                !this._DecorrenzaCalcoloArretrati.HasValue && !this._TrasformazioneAOI.HasValue &&
                !this._Benefici.HasValue && !this._ExCombattente.HasValue && !this._DataCompletezza.HasValue &&
                !this._DataRicezionePrenotazioneCentrale.HasValue && !this._Maggiorazioni.HasValue && !this._IsRichiestaBonus.HasValue &&
                String.IsNullOrEmpty(_AnnoDecorrenzaBonus))
                return true;
            else
                return false;
        }

        public bool IsDatiGenericiIstruttoriaNull()
        {
            if (!this._ScadenzaRevisioneSanitaria.HasValue && !this._CodiceMobilita.HasValue && !this.CodiceDomandaRicorso.HasValue && !this._CodiceComunicazioneCampo4.HasValue &&
                !this.TrattamentoDisagi.HasValue)
                return true;
            else
                return false;
        }

        public bool IsDatiGenericiPagamentoNull()
        {
            if (!this._TrattenutaInpdap.HasValue && !this._DataRinunciaTrattenutaInpdap.HasValue)
                return true;
            else
                return false;
        }

        public bool IsDatiGenericiPensioneCiDatiDenericiNull()
        {
            if (!this._DecorrenzaBonus.HasValue)
                return true;
            else
                return false;
        }

        public bool IsDatiGenericiNuoveLiquidateNull()
        {
            if (!this._FlagContributiva.HasValue)
                return true;
            else
                return false;
        }
    }
}