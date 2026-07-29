using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.BLCommon.Entity
{
    public class DatiSupplementi
    {
        #region private properties

        private long _Id;
        private long _IdPensione;
        private long? _NumDomanda;
        private char? _TipoSupplemento;
        private DateTime? _DecorrenzaSupplemento;
        private string _CodGestioneSupplemento;
        private int? _NSettimaneSupplemento;
        private decimal? _RMSSupplemento;
        private decimal? _MontanteSupplemento;
        private char? _QuotaSupplemento;
        private string _CodTipoQuota;
        private decimal? _AmmontareContributivo;
        private byte? _CodiceLiquidazione;
        private bool _IsFromPrelievo;
        private bool _IsStorico;

        #endregion private properties

        #region public properties

        public long Id { get { return _Id; } set { _Id = value; } }

        public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }

        public long? NumDomanda { get { return _NumDomanda; } set { _NumDomanda = value; } }

        public char? TipoSupplemento { get { return _TipoSupplemento; } set { _TipoSupplemento = value; } }

        public DateTime? DecorrenzaSupplemento { get { return _DecorrenzaSupplemento; } set { _DecorrenzaSupplemento = value; } }

        public string CodGestioneSupplemento { get { return _CodGestioneSupplemento; } set { _CodGestioneSupplemento = value; } }

        public int? NSettimaneSupplemento { get { return _NSettimaneSupplemento; } set { _NSettimaneSupplemento = value; } }

        public decimal? RMSSupplemento { get { return _RMSSupplemento; } set { _RMSSupplemento = value; } }

        public decimal? MontanteSupplemento { get { return _MontanteSupplemento; } set { _MontanteSupplemento = value; } }

        public char? QuotaSupplemento { get { return _QuotaSupplemento; } set { _QuotaSupplemento = value; } }

        public string CodTipoQuota { get { return _CodTipoQuota; } set { _CodTipoQuota = value; } }

        public decimal? AmmontareContributivo { get { return _AmmontareContributivo; } set { _AmmontareContributivo = value; } }

        public byte? CodiceLiquidazione { get { return _CodiceLiquidazione; } set { _CodiceLiquidazione = value; } }

        public bool IsFromPrelievo { get { return _IsFromPrelievo; } set { _IsFromPrelievo = value; } }

        public bool IsStorico { get { return _IsStorico; } set { _IsStorico = value; } }

        #endregion public properties

        public bool IsDatiSupplementiNull()
        {
            if (this.TipoSupplemento.HasValue ||
                this.DecorrenzaSupplemento.HasValue || this.CodGestioneSupplemento != null || this.NSettimaneSupplemento.HasValue ||
                this.RMSSupplemento.HasValue || this.MontanteSupplemento.HasValue || this.QuotaSupplemento.HasValue)
                return false;
            else
                return true;
        }
    }

    public class SupplementiBase
    {
        #region private properties

        private long _Id;
        private long _IdPensione;
        private decimal? _RMS7290;
        private decimal? _ImportoIVS;
        private decimal? _RMSArt2DPCM161289;
        private decimal? _ContributiLegge335;
        private int? _NSettimaneUtiliDiritto;
        private int? _NSettimaneUtiliMisura;
        private int? _Legge407AnniServizioQuotaA;
        private int? _Legge407AnniServizioQuotaB;
        private int? _Legge407SettimaneServizioQuotaB;
        private int? _Legge407AnniServizioQuotaC;
        private decimal? _Legge407RetribuzionePensionabileRMSQuotaA;
        private decimal? _Legge407RetribuzionePensionabileRMSQuotaB;
        private decimal? _Legge407RetribuzionePensionabileQuotaC;
        private int? _Legge407SettimaneIncrementoQuotaA;
        private int? _Legge407SettimaneIncrementoQuotaB;
        private int? _AnniIncremento1Percento;
        private int? _AnniIncremento05Percento;
        private decimal? _RenditaFacoltativaOrdinaria;
        private decimal? _RenditaFacoltativaConvenzionale;

        #endregion private properties

        #region public properties

        public long Id { get { return _Id; } set { _Id = value; } }
        public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
        public decimal? RMS7290 { get { return _RMS7290; } set { _RMS7290 = value; } }
        public decimal? ImportoIVS { get { return _ImportoIVS; } set { _ImportoIVS = value; } }
        public decimal? RMSArt2DPCM161289 { get { return _RMSArt2DPCM161289; } set { _RMSArt2DPCM161289 = value; } }
        public decimal? ContributiLegge335 { get { return _ContributiLegge335; } set { _ContributiLegge335 = value; } }
        public int? NSettimaneUtiliDiritto { get { return _NSettimaneUtiliDiritto; } set { _NSettimaneUtiliDiritto = value; } }
        public int? NSettimaneUtiliMisura { get { return _NSettimaneUtiliMisura; } set { _NSettimaneUtiliMisura = value; } }
        public int? Legge407AnniServizioQuotaA { get { return _Legge407AnniServizioQuotaA; } set { _Legge407AnniServizioQuotaA = value; } }
        public int? Legge407AnniServizioQuotaB { get { return _Legge407AnniServizioQuotaB; } set { _Legge407AnniServizioQuotaB = value; } }
        public int? Legge407SettimaneServizioQuotaB { get { return _Legge407SettimaneServizioQuotaB; } set { _Legge407SettimaneServizioQuotaB = value; } }
        public int? Legge407AnniServizioQuotaC { get { return _Legge407AnniServizioQuotaC; } set { _Legge407AnniServizioQuotaC = value; } }
        public decimal? Legge407RetribuzionePensionabileRMSQuotaA { get { return _Legge407RetribuzionePensionabileRMSQuotaA; } set { _Legge407RetribuzionePensionabileRMSQuotaA = value; } }
        public decimal? Legge407RetribuzionePensionabileRMSQuotaB { get { return _Legge407RetribuzionePensionabileRMSQuotaB; } set { _Legge407RetribuzionePensionabileRMSQuotaB = value; } }
        public decimal? Legge407RetribuzionePensionabileQuotaC { get { return _Legge407RetribuzionePensionabileQuotaC; } set { _Legge407RetribuzionePensionabileQuotaC = value; } }
        public int? Legge407SettimaneIncrementoQuotaA { get { return _Legge407SettimaneIncrementoQuotaA; } set { _Legge407SettimaneIncrementoQuotaA = value; } }
        public int? Legge407SettimaneIncrementoQuotaB { get { return _Legge407SettimaneIncrementoQuotaB; } set { _Legge407SettimaneIncrementoQuotaB = value; } }
        public int? AnniIncremento1Percento { get { return _AnniIncremento1Percento; } set { _AnniIncremento1Percento = value; } }
        public int? AnniIncremento05Percento { get { return _AnniIncremento05Percento; } set { _AnniIncremento05Percento = value; } }
        public decimal? RenditaFacoltativaOrdinaria { get { return _RenditaFacoltativaOrdinaria; } set { _RenditaFacoltativaOrdinaria = value; } }
        public decimal? RenditaFacoltativaConvenzionale { get { return _RenditaFacoltativaConvenzionale; } set { _RenditaFacoltativaConvenzionale = value; } }


        #endregion public properties

        #region public members

        public bool IsSupplementiBaseNull()
        {
            if (!this.RenditaFacoltativaOrdinaria.HasValue && !this.RenditaFacoltativaConvenzionale.HasValue)
                return true;
            else
                return false;
        }

        public override bool Equals(object obj)
        {
            SupplementiBase supplementoBase = (SupplementiBase)obj;
            try
            {
                if (this._RMS7290 != supplementoBase._RMS7290 ||
                    this._ImportoIVS != supplementoBase._ImportoIVS ||
                    this._RMSArt2DPCM161289 != supplementoBase._RMSArt2DPCM161289 ||
                    this._ContributiLegge335 != supplementoBase._ContributiLegge335 ||
                    this._NSettimaneUtiliDiritto != supplementoBase._NSettimaneUtiliDiritto ||
                    this._NSettimaneUtiliMisura != supplementoBase._NSettimaneUtiliMisura ||
                    this._Legge407AnniServizioQuotaA != supplementoBase._Legge407AnniServizioQuotaA ||
                    this._Legge407AnniServizioQuotaB != supplementoBase._Legge407AnniServizioQuotaB ||
                    this._Legge407SettimaneServizioQuotaB != supplementoBase._Legge407SettimaneServizioQuotaB ||
                    this._Legge407AnniServizioQuotaC != supplementoBase._Legge407AnniServizioQuotaC ||
                    this._Legge407RetribuzionePensionabileRMSQuotaA != supplementoBase._Legge407RetribuzionePensionabileRMSQuotaA ||
                    this._Legge407RetribuzionePensionabileRMSQuotaB != supplementoBase._Legge407RetribuzionePensionabileRMSQuotaB ||
                    this._Legge407RetribuzionePensionabileQuotaC != supplementoBase._Legge407RetribuzionePensionabileQuotaC ||
                    this._Legge407SettimaneIncrementoQuotaA != supplementoBase._Legge407SettimaneIncrementoQuotaA ||
                    this._Legge407SettimaneIncrementoQuotaB != supplementoBase._Legge407SettimaneIncrementoQuotaB ||
                    this._AnniIncremento1Percento != supplementoBase._AnniIncremento1Percento ||
                    this._AnniIncremento05Percento != supplementoBase._AnniIncremento05Percento ||
                    this._RenditaFacoltativaOrdinaria != supplementoBase._RenditaFacoltativaOrdinaria ||
                    this._RenditaFacoltativaConvenzionale != supplementoBase._RenditaFacoltativaConvenzionale

                    )
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        //TODO GETHASHCODE
        //public override int GetHashCode()
        //{
        ////    int hash = 13;
        //    hash = (hash * 7) + (this._RMS7290 != null ? this._RMS7290.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._ImportoIVS != null ? this._ImportoIVS.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._RMSArt2DPCM161289 != null ? this._RMSArt2DPCM161289.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._ContributiLegge335 != null ? this._ContributiLegge335.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._NSettimaneUtiliDiritto != null ? this._NSettimaneUtiliDiritto.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._NSettimaneUtiliMisura != null ? this._NSettimaneUtiliMisura.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._Legge407AnniServizioQuotaA != null ? this._Legge407AnniServizioQuotaA.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._Legge407AnniServizioQuotaB != null ? this._Legge407AnniServizioQuotaB.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._Legge407SettimaneServizioQuotaB != null ? this._Legge407SettimaneServizioQuotaB.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._Legge407AnniServizioQuotaC != null ? this._Legge407AnniServizioQuotaC.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._Legge407RetribuzionePensionabileRMSQuotaA != null ? this._Legge407RetribuzionePensionabileRMSQuotaA.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._Legge407RetribuzionePensionabileRMSQuotaB != null ? this._Legge407RetribuzionePensionabileRMSQuotaB.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._Legge407RetribuzionePensionabileQuotaC != null ? this._Legge407RetribuzionePensionabileQuotaC.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._Legge407SettimaneIncrementoQuotaA != null ? this._Legge407SettimaneIncrementoQuotaA.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._Legge407SettimaneIncrementoQuotaB != null ? this._Legge407SettimaneIncrementoQuotaB.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._AnniIncremento1Percento != null ? this._AnniIncremento1Percento.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._AnniIncremento05Percento != null ? this._AnniIncremento05Percento.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._RenditaFacoltativaOrdinaria != null ? this._RenditaFacoltativaOrdinaria.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._RenditaFacoltativaConvenzionale != null ? this._RenditaFacoltativaConvenzionale.GetHashCode() : 0);
        //    return hash;
        //    return 1;
        //}

        #endregion public members
    }

    public class IntegrazioneArt11
    {
        #region private properties

        private long _IdPensione;
        private decimal? _ImportoIVS;
        private DateTime? _Decorrenza;

        #endregion private properties

        #region public properties

        public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
        public decimal? ImportoIVS { get { return _ImportoIVS; } set { _ImportoIVS = value; } }
        public DateTime? Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }

        #endregion public properties

        #region public members

        public bool IsIntegrazioneArt11Null()
        {
            if (!this.ImportoIVS.HasValue && !this.Decorrenza.HasValue)
                return true;
            else
                return false;
        }

        public override bool Equals(object obj)
        {
            IntegrazioneArt11 integrazioneArt11 = (IntegrazioneArt11)obj;
            try
            {
                if (this._ImportoIVS != integrazioneArt11._ImportoIVS ||
                    this._Decorrenza != integrazioneArt11._Decorrenza)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        //TODO GETHASHCODE
        //public override int GetHashCode()
        //{
        //    int hash = 13;
        //    hash = (hash * 7) + (this._ImportoIVS != null ? this._ImportoIVS.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._Decorrenza != null ? this._Decorrenza.GetHashCode() : 0);
        //    return hash;
        //}

        #endregion public members
    }

    public class DatiSupplementiENPALS
    {
        #region private properties

        private long _IdPensione;
        private long _IdSuppRecordENPALS;
        private System.Nullable<char> _TipoSupplemento;
        private System.Nullable<char> _Quota;
        private System.Nullable<short> _Periodi;
        private System.Nullable<short> _NTotaleContributiCalcolo;
        private System.Nullable<decimal> _RM;
        private System.Nullable<decimal> _Importo;
        private System.Nullable<decimal> _ImportoProRataTemporis;
        private System.Nullable<decimal> _CoefficienteTrasformazione;
        private System.Nullable<decimal> _ImportoContributivoTotale;
        private System.Nullable<decimal> _Montante;
        private DateTime? _Decorrenza;
        private bool _IsFromSAS;
        private bool _IsFromGP;

        #endregion private properties

        #region public properties

        public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
        public long IdSuppRecordENPALS { get { return _IdSuppRecordENPALS; } set { _IdSuppRecordENPALS = value; } }
        public char? TipoSupplemento { get { return _TipoSupplemento; } set { _TipoSupplemento = value; } }
        public char? Quota { get { return _Quota; } set { _Quota = value; } }
        public short? Periodi { get { return _Periodi; } set { _Periodi = value; } }
        public short? NTotaleContributiCalcolo { get { return _NTotaleContributiCalcolo; } set { _NTotaleContributiCalcolo = value; } }
        public decimal? RM { get { return _RM; } set { _RM = value; } }
        public decimal? Importo { get { return _Importo; } set { _Importo = value; } }
        public decimal? ImportoProRataTemporis { get { return _ImportoProRataTemporis; } set { _ImportoProRataTemporis = value; } }
        public decimal? CoefficienteTrasformazione { get { return _CoefficienteTrasformazione; } set { _CoefficienteTrasformazione = value; } }
        public decimal? ImportoContributivoTotale { get { return _ImportoContributivoTotale; } set { _ImportoContributivoTotale = value; } }
        public decimal? Montante { get { return _Montante; } set { _Montante = value; } }
        public DateTime? Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
        public bool IsFromSAS { get { return _IsFromSAS; } set { _IsFromSAS = value; } }
        public bool IsFromGP { get { return _IsFromGP; } set { _IsFromGP = value; } }

        #endregion public properties

        #region public members

        public bool IsSupplementiEnpalsNull()
        {
            if (!this._TipoSupplemento.HasValue && !this._Quota.HasValue && !this._Periodi.HasValue && !this._NTotaleContributiCalcolo.HasValue && !this._RM.HasValue && !this._Importo.HasValue &&
                !this._ImportoProRataTemporis.HasValue && !this._CoefficienteTrasformazione.HasValue && !this._ImportoContributivoTotale.HasValue && !this._Montante.HasValue && !this._Decorrenza.HasValue)
                return true;
            else
                return false;
        }

        public override bool Equals(object obj)
        {
            DatiSupplementiENPALS supplementiEnpals = (DatiSupplementiENPALS)obj;
            try
            {
                if (this._TipoSupplemento != supplementiEnpals._TipoSupplemento ||
                    this._Quota != supplementiEnpals._Quota ||
                    this._Periodi != supplementiEnpals._Periodi ||
                    this._NTotaleContributiCalcolo != supplementiEnpals._NTotaleContributiCalcolo ||
                    this._RM != supplementiEnpals._RM ||
                    this._Importo != supplementiEnpals._Importo ||
                    this._ImportoProRataTemporis != supplementiEnpals._ImportoProRataTemporis ||
                    this._CoefficienteTrasformazione != supplementiEnpals._CoefficienteTrasformazione ||
                    this._ImportoContributivoTotale != supplementiEnpals._ImportoContributivoTotale ||
                    this._Montante != supplementiEnpals._Montante ||
                    this._Decorrenza != supplementiEnpals._Decorrenza ||
                    this._IsFromSAS != supplementiEnpals._IsFromSAS ||
                    this._IsFromGP != supplementiEnpals._IsFromGP
                    )
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        //TODO GETHASHCODE
        //public override int GetHashCode()
        //{
        //    int hash = 13;
        //    hash = (hash * 7) + (this._TipoSupplemento != null ? this._TipoSupplemento.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._Quota != null ? this._Quota.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._Periodi != null ? this._Periodi.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._NTotaleContributiCalcolo != null ? this._NTotaleContributiCalcolo.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._RM != null ? this._RM.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._Importo != null ? this._Importo.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._ImportoProRataTemporis != null ? this._ImportoProRataTemporis.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._CoefficienteTrasformazione != null ? this._CoefficienteTrasformazione.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._ImportoContributivoTotale != null ? this._ImportoContributivoTotale.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._Montante != null ? this._Montante.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._Decorrenza != null ? this._Decorrenza.GetHashCode() : 0);
        //    hash = (hash * 7) + (this._IsFromSAS.GetHashCode());
        //    hash = (hash * 7) + (this._IsFromGP.GetHashCode());
        //    return hash;
        //}

        #endregion public members
    }

    public class DatiSuppRecordENPALS
    {
        public long IdSuppRecordEnpals { get; set; }
        public DateTime? Decorrenza { get; set; }
        public DateTime? InizioSupplemento { get; set; }
        public DateTime? FineSupplemento { get; set; }
        public decimal? Importo { get; set; }
        public decimal? RenditaFacoltativaOrdinaria { get; set; }
        public decimal? RenditaFacoltativaConvenzionale { get; set; }
        public bool IsFromSas { get; set; }
        public bool IsFromGP { get; set; }
        public bool DettaglioSalvato { get; set; }
    }

    public class DatiSupplementiCumulo
    {
        #region public properties
        public long Id { get; set; }
        public long IdPensione { get; set; }
        public long EnteGestioneFondo { get; set; }
        public int? Settimane { get; set; }
        public decimal? Importo { get; set; }
        public DateTime? Decorrenza { get; set; }
        public bool IsStorico { get; set; }
        public bool? AdeguamentoProQuotaCasse { get; set; }
        //ENG - Memo 32_a/2018
        public int? TipoVariazione { get; set; }
        #endregion public properties

        public bool IsSupplementiCumuloNull()
        {
            if (this.EnteGestioneFondo == 0 && !this.Settimane.HasValue && !this.Importo.HasValue && !this.Decorrenza.HasValue)
                return true;
            else
                return false;
        }
    }
}
