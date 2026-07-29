using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DatiGenerici
    {
        #region private properties

        #region pensione
        private DateTime? _DataInteressiLegali;
        private byte? _CodiceArretrati;
        private byte? _CausaCarico;
        private string _NaturaPensione;
        private DateTime? _DataInizioCalcolo;
        private DateTime? _DecorrenzaCalcoloArretrati;
        private bool? _Benefici;
        private bool? _ExCombattente;
        private bool? _TrasformazioneAOI;
        private DateTime? _DataCompletezza;
        private byte? _TipoCalcolo;
        private bool? _Maggiorazioni;
        private char? _Contributivo;
        private bool? _IsRichiestaBonus;
        private string _AnnoDecorrenzaBonus;

        #endregion pensione

        #region istruttoria
        private DateTime? _ScadenzaRevisioneSanitaria;
        private byte? _CodiceMobilita;
        private byte? _CodiceDomandaRicorso;
        private byte? _CodiceComunicazioneCampo4;
        private string _ModalitaLiquidazione;
        private char? _CodiceLiquidazione;
        private byte? _NRiconoscimentiInvalidita;
        private bool? _TrattamentoDisagi;
        #endregion istruttoria

        #region Pagamento
        private bool? _TrattenutaInpdap;
        private DateTime? _DataRinunciaTrattenutaInpdap;
        #endregion Pagamento

        #region Nuove Liquidate
        private bool? _FlagProvvisoria;
        #endregion Nuove Liquidate

        #region PensioniDatiGenerici
        private long? _EnteCassa;
        private bool? _EnteIstruttoreExInpdap;
        private bool? _TipoCumulo;
        private char? _CumuloEsterno;
        #endregion PensioniDatiGenerici

        #endregion private properties

        #region public properties

        #region pensione
        public DateTime? DataInteressiLegali { get { return _DataInteressiLegali; } set { _DataInteressiLegali = value; } }
        public byte? CodiceArretrati { get { return _CodiceArretrati; } set { _CodiceArretrati = value; } }
        public byte? CausaCarico { get { return _CausaCarico; } set { _CausaCarico = value; } }
        public string NaturaPensione { get { return _NaturaPensione; } set { _NaturaPensione = value; } }
        public DateTime? DataInizioCalcolo { get { return _DataInizioCalcolo; } set { _DataInizioCalcolo = value; } }
        public DateTime? DecorrenzaCalcoloArretrati { get { return _DecorrenzaCalcoloArretrati; } set { _DecorrenzaCalcoloArretrati = value; } }
        public bool? Benefici { get { return _Benefici; } set { _Benefici = value; } }
        public bool? ExCombattente { get { return _ExCombattente; } set { _ExCombattente = value; } }
        public bool? TrasformazioneAOI { get { return _TrasformazioneAOI; } set { _TrasformazioneAOI = value; } }
        public DateTime? DataCompletezza { get { return _DataCompletezza; } set { _DataCompletezza = value; } }
        public byte? TipoCalcolo { get { return _TipoCalcolo; } set { _TipoCalcolo = value; } }
        public bool? Maggiorazioni { get { return _Maggiorazioni; } set { _Maggiorazioni = value; } }
        public char? Contributivo { get { return _Contributivo; } set { _Contributivo = value; } }
        public bool? IsRichiestaBonus { get { return _IsRichiestaBonus; } set { _IsRichiestaBonus = value; } }
        public string AnnoDecorrenzaBonus { get { return _AnnoDecorrenzaBonus; } set { _AnnoDecorrenzaBonus = value; } }

        #endregion pensione

        #region istruttoria
        public DateTime? ScadenzaRevisioneSanitaria { get { return _ScadenzaRevisioneSanitaria; } set { _ScadenzaRevisioneSanitaria = value; } }
        public byte? CodiceMobilita { get { return _CodiceMobilita; } set { _CodiceMobilita = value; } }
        public byte? CodiceDomandaRicorso { get { return _CodiceDomandaRicorso; } set { _CodiceDomandaRicorso = value; } }
        public byte? CodiceComunicazioneCampo4 { get { return _CodiceComunicazioneCampo4; } set { _CodiceComunicazioneCampo4 = value; } }
        public string ModalitaLiquidazione { get { return _ModalitaLiquidazione; } set { _ModalitaLiquidazione = value; } }
        public char? CodiceLiquidazione { get { return _CodiceLiquidazione; } set { _CodiceLiquidazione = value; } }
        public byte? NRiconoscimentiInvalidita { get { return _NRiconoscimentiInvalidita; } set { _NRiconoscimentiInvalidita = value; } }
        public bool? TrattamentoDisagi { get { return _TrattamentoDisagi; } set { _TrattamentoDisagi = value; } }
        #endregion istruttoria

        #region Pagamento
        public bool? TrattenutaInpdap { get { return _TrattenutaInpdap; } set { _TrattenutaInpdap = value; } }
        public DateTime? DataRinunciaTrattenutaInpdap { get { return _DataRinunciaTrattenutaInpdap; } set { _DataRinunciaTrattenutaInpdap = value; } }
        #endregion Pagamento

        #region Nuove Liquidate
        public bool? FlagProvvisoria { get { return _FlagProvvisoria; } set { _FlagProvvisoria = value; } }
        #endregion Nuove Liquidate

        #region PensioniDatiGenerici
        public long? EnteCassa { get { return _EnteCassa; } set { _EnteCassa = value; } }
        public bool? EnteIstruttoreExInpdap { get { return _EnteIstruttoreExInpdap; } set { _EnteIstruttoreExInpdap = value; } }
        public bool? TipoCumulo { get { return _TipoCumulo; } set { _TipoCumulo = value; } }
        public char? CumuloEsterno { get { return _CumuloEsterno; } set { _CumuloEsterno = value; } }
        #endregion PensioniDatiGenerici

        #endregion public properties

        public bool IsDatiGenericiPensioneNull()
        {
            if (!this._DataInteressiLegali.HasValue && !this._CodiceArretrati.HasValue &&
                !this._CausaCarico.HasValue && !this._DataInizioCalcolo.HasValue &&
                String.IsNullOrEmpty(this._NaturaPensione) && !this._DataInizioCalcolo.HasValue &&
                !this._DecorrenzaCalcoloArretrati.HasValue && !this._TrasformazioneAOI.HasValue &&
                !this._Benefici.HasValue && !this._ExCombattente.HasValue && !this._DataCompletezza.HasValue && !this._TipoCalcolo.HasValue &&
                !this._Maggiorazioni.HasValue && !this._Contributivo.HasValue && !this._IsRichiestaBonus.HasValue && String.IsNullOrEmpty(this._AnnoDecorrenzaBonus))
                return true;
            else
                return false;
        }

        public bool IsDatiGenericiIstruttoriaNull()
        {
            if (!this._ScadenzaRevisioneSanitaria.HasValue &&
                !this._CodiceMobilita.HasValue && !this._CodiceDomandaRicorso.HasValue && !this._NRiconoscimentiInvalidita.HasValue &&
                !this._CodiceComunicazioneCampo4.HasValue && String.IsNullOrEmpty(this._ModalitaLiquidazione) && !this._CodiceLiquidazione.HasValue &&
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

        public bool IsDatiGenericiNuoveLiquidateNull()
        {
            if (!this._FlagProvvisoria.HasValue)
                return true;
            else
                return false;
        }

        public bool IsDatiGenericiPensioniDatiGenericiNull()
        {
            if (!this._EnteCassa.HasValue && !this._EnteIstruttoreExInpdap.HasValue && !this._TipoCumulo.HasValue && !this._CumuloEsterno.HasValue)
                return true;

            return false;
        }

        public char GetCodNatura1()
        {
            char codNat1 = ' ';
            char codNat2 = ' ';
            char codNat3 = ' ';
            if(this.NaturaPensione!=null)
                Utility.GetCodiciNatura(this.NaturaPensione, out codNat1, out codNat2, out codNat3);
            return codNat1;
        }
        public char GetCodNatura2()
        {
            char codNat1 = ' ';
            char codNat2 = ' ';
            char codNat3 = ' ';
            if (this.NaturaPensione != null)
                Utility.GetCodiciNatura(this.NaturaPensione, out codNat1, out codNat2, out codNat3);
            return codNat2;
        }
        public char GetCodNatura3()
        {
            char codNat1 = ' ';
            char codNat2 = ' ';
            char codNat3 = ' ';
            if (this.NaturaPensione != null)
                Utility.GetCodiciNatura(this.NaturaPensione, out codNat1, out codNat2, out codNat3);
            return codNat3;
        }
    }
}
