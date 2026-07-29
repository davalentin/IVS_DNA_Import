using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DatiIstruttoria
    {
        #region private properties

        private ENPALS _DatiENPALS;

        #region Pensione
        //private bool? _ExCombattente;
        private decimal? _AliquotaTFREsodati;
        private Int16? _CodiceBancaEsodati;
        private string _CodiceBancaEsodatiTraduzioneSuGP;
        private short? _CodiceEnte;
        #endregion Pensione

        #region Istruttoria

        private byte? _CodiceCdCmMr;
        private byte? _Legge44997;
        private long? _CodiceParticolareSoggettoDerogato;
        private decimal? _RiduzioneAssegno;
        private short? _CodiceAziendaEditoria;
        private short? _CodiceAziendaEditoriaPerTipo0171;
        private short? _CodiceAziendaEditoriaPerTipo0179;
        private short? _CodiceAziendaEditoriaLetteraB;
        private byte? _TipoCalcoloPrecedente;

        #endregion Istruttoria

        #region PensioniDatiGenerici

        private bool _RiduzioneRetributiva;
        private System.Nullable<decimal> _RiduzioneRetributivaPercentuale;
        private short? _AnnoBancaFideiussoria;
        private byte? _ProgressivoBancaFideiussoria;
        private DateTime? _ScadenzaAssegno;

        #endregion PensioniDatiGenerici

        #region MaggiorazioneBenefici

        private bool? _Attivitausuranti;

        #endregion MaggiorazioneBenefici

        #endregion private properties

        #region public properties

        public ENPALS DatiENPALS { get { return _DatiENPALS; } set { _DatiENPALS = value; } }

        #region Pensione
        //public bool? ExCombattente { get { return _ExCombattente; } set { _ExCombattente = value; } }
        public decimal? AliquotaTFREsodati { get { return _AliquotaTFREsodati; } set { _AliquotaTFREsodati = value; } }
        public Int16? CodiceBancaEsodati { get { return _CodiceBancaEsodati; } set { _CodiceBancaEsodati = value; } }
        public string CodiceBancaEsodatiTraduzioneSuGP { get { return _CodiceBancaEsodatiTraduzioneSuGP; } set { _CodiceBancaEsodatiTraduzioneSuGP = value; } }
        #endregion Pensione

        #region Istruttoria

        public byte? CodiceCdCmMr { get { return _CodiceCdCmMr; } set { _CodiceCdCmMr = value; } }
        public byte? Legge44997 { get { return _Legge44997; } set { _Legge44997 = value; } }
        public long? CodiceParticolareSoggettoDerogato { get { return _CodiceParticolareSoggettoDerogato; } set { _CodiceParticolareSoggettoDerogato = value; } }
        public decimal? RiduzioneAssegno { get { return _RiduzioneAssegno; } set { _RiduzioneAssegno = value; } }
        public short? CodiceAziendaEditoria { get { return _CodiceAziendaEditoria; } set { _CodiceAziendaEditoria = value; } }
        public short? CodiceAziendaEditoriaPerTipo0171 { get { return _CodiceAziendaEditoriaPerTipo0171; } set { _CodiceAziendaEditoriaPerTipo0171 = value; } }
        public short? CodiceAziendaEditoriaPerTipo0179 { get { return _CodiceAziendaEditoriaPerTipo0179; } set { _CodiceAziendaEditoriaPerTipo0179 = value; } }
        public short? CodiceAziendaEditoriaLetteraB { get { return _CodiceAziendaEditoriaLetteraB; } set { _CodiceAziendaEditoriaLetteraB = value; } }
        public short? CodiceEnte { get { return _CodiceEnte; } set { _CodiceEnte = value; } }
        public byte? TipoCalcoloPrecedente { get { return _TipoCalcoloPrecedente; } set { _TipoCalcoloPrecedente = value; } }
        #endregion Istruttoria

        #region PensioniDatiGenerici

        public bool RiduzioneRetributiva { get { return _RiduzioneRetributiva; } set { _RiduzioneRetributiva = value; } }
        public System.Nullable<decimal> RiduzioneRetributivaPercentuale { get { return _RiduzioneRetributivaPercentuale; } set { _RiduzioneRetributivaPercentuale = value; } }
        public short? AnnoBancaFideiussoria { get { return _AnnoBancaFideiussoria; } set { _AnnoBancaFideiussoria = value; } }
        public byte? ProgressivoBancaFideiussoria { get { return _ProgressivoBancaFideiussoria; } set { _ProgressivoBancaFideiussoria = value; } }
        public DateTime? ScadenzaAssegno { get { return _ScadenzaAssegno; } set { _ScadenzaAssegno = value; } }

        #endregion PensioniDatiGenerici

        #region MaggiorazioneBenefici

        public bool? Attivitausuranti { get { return _Attivitausuranti; } set { _Attivitausuranti = value; } }

        #endregion MaggiorazioneBenefici

        public bool IsDatiIstruttoriaPensioneNull()
        {
            if (/*(!this._ExCombattente.HasValue || !this._ExCombattente.Value) &&*/ !this._AliquotaTFREsodati.HasValue && !this._CodiceBancaEsodati.HasValue)
                return true;
            else
                return false;
        }

        public bool IsDatiIstruttoriaIstruttoriaNull()
        {
            if (!this._CodiceCdCmMr.HasValue && !this._Legge44997.HasValue && !this._CodiceParticolareSoggettoDerogato.HasValue && !this._RiduzioneAssegno.HasValue &&
                !this.CodiceAziendaEditoria.HasValue && !this.CodiceAziendaEditoriaPerTipo0171.HasValue && !this.CodiceAziendaEditoriaPerTipo0179.HasValue && !this.CodiceAziendaEditoriaLetteraB.HasValue && !this.CodiceEnte.HasValue)
                return true;
            else
                return false;
        }

        public bool IsDatiIstruttoriaDatiGenericiNull()
        {
            if (!this._RiduzioneRetributiva && !this._RiduzioneRetributivaPercentuale.HasValue && !this._AnnoBancaFideiussoria.HasValue && !this._ProgressivoBancaFideiussoria.HasValue && !this._ScadenzaAssegno.HasValue)
                return true;
            else
                return false;
        }

        public bool IsDatiIstruttoriaMaggiorazioneBeneficiNull()
        {
            if (!this._Attivitausuranti.HasValue)
                return true;
            else
                return false;
        }

        public bool IsDatiIstruttoriaENPALSNull()
        {
            if (this.DatiENPALS == null || this.DatiENPALS.IsDatiIstruttoriaEnpalsNull())
                return true;

            return false;
        }

        #endregion public properties

        #region Nested Class

        public class ENPALS
        {
            #region Private properties

            private string _CodiceDeroga1;
            private string _CodiceDeroga2;
            private string _CodiceDeroga3;
            private string _CodiceDeroga4;

            #endregion Private properties

            #region Public properties

            public string CodiceDeroga1 { get { return _CodiceDeroga1; } set { _CodiceDeroga1 = value; } }
            public string CodiceDeroga2 { get { return _CodiceDeroga2; } set { _CodiceDeroga2 = value; } }
            public string CodiceDeroga3 { get { return _CodiceDeroga3; } set { _CodiceDeroga3 = value; } }
            public string CodiceDeroga4 { get { return _CodiceDeroga4; } set { _CodiceDeroga4 = value; } }

            #endregion Public properties

            public bool IsDatiIstruttoriaEnpalsNull()
            {
                if (string.IsNullOrEmpty(this._CodiceDeroga1) && string.IsNullOrEmpty(this._CodiceDeroga2) && string.IsNullOrEmpty(this._CodiceDeroga3) && string.IsNullOrEmpty(this._CodiceDeroga4))
                    return true;
                else
                    return false;
            }
        }

        #endregion Nested Class
    }
}