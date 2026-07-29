using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneDatiGenericiAgoCi
    {
        public static void GetDatiGenericiByIdPensione(Int64 idPensione, out PensioniDatiGenerici datiGenerici)
        {
            DataCommon.PensioniDatiGenerici datiGenericiDA = null;
            datiGenerici = null;
            DAGestioneDatiContributiviCi.GetDatiGenericiByIdPensione(idPensione, out datiGenericiDA);
            if (datiGenericiDA == null)
                return;
            datiGenerici = new PensioniDatiGenerici();
            Utility.ValorizzaOggetti(datiGenericiDA, datiGenerici);
        }

        public static void SalvaDatiGenerici(long idPensione, PensioniDatiGenerici datiGenerici)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DataCommon.PensioniDatiGenerici datiGenericiDA = new DataCommon.PensioniDatiGenerici();
                Utility.ValorizzaOggetti(datiGenerici, datiGenericiDA);
                datiGenericiDA.IdPensione = idPensione;
                DAGestioneDatiContributiviCi.SalvaDatiGenerici(datiGenericiDA);
                transactionScope.Complete();
            }
        }

        public static void EliminaDatiGenericiByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDatiContributiviCi.DeleteDatiGenericiByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static bool IsDatiGenericiNull(PensioniDatiGenerici datiGenerici)
        {
            if (!datiGenerici.RegimeLiquidazione.HasValue &&
                !datiGenerici.ContributiItalianiEdEsteriAl1295.HasValue &&
                !datiGenerici.NSettFittiziePrepensionamento.HasValue &&
                !datiGenerici.CodiceVirtuale.HasValue &&
                !datiGenerici.ConiugeSuperstite.HasValue &&
                !datiGenerici.DecorrenzaCodiceVirtuale.HasValue &&
                !datiGenerici.ImportoPensioneEEInvalido.HasValue &&
                !datiGenerici.DataRicalcoloPrestazioneEE.HasValue &&
                !datiGenerici.NContributiItalia.HasValue &&
                !datiGenerici.DecorrenzaBonus.HasValue &&
                !datiGenerici.DeliberaCee126.HasValue &&
                !datiGenerici.ImportoCristallizzazione3481.HasValue &&
                !datiGenerici.UfficioPagatoreArretratiEE.HasValue &&
                (String.IsNullOrEmpty(datiGenerici.CodiceScadenzaAssegno)) &&
                !datiGenerici.AnniDifferimento.HasValue &&
                (String.IsNullOrEmpty(datiGenerici.CodiciMotivazioniCi281)) &&
                (String.IsNullOrEmpty(datiGenerici.CodiciMotivazioniCi282)) &&
                !datiGenerici.CodiciCi21.HasValue &&
                !datiGenerici.CodiceBloccoArretratiEE.HasValue &&
                (String.IsNullOrEmpty(datiGenerici.CodicePensioneRiliquidata)) &&
                !datiGenerici.ApplicazioneSentenza49593.HasValue &&
                !datiGenerici.DecorrenzaArt2Dpcm.HasValue &&
                !datiGenerici.DataPrecedenteLiquidazione.HasValue &&
                !datiGenerici.DataArrivoDomanda.HasValue &&
                !datiGenerici.Data1Domanda.HasValue &&
                !datiGenerici.VVMisuraAl1292.HasValue &&
                !datiGenerici.VVMisuraDL50392.HasValue &&
                !datiGenerici.SettimanePerCalcoloContributivo.HasValue &&
                !datiGenerici.SettimaneItalianeDiritto.HasValue &&
                !datiGenerici.ImportoIVS.HasValue &&
                !datiGenerici.MaternitaAcna.HasValue &&
                !datiGenerici.RMS8888.HasValue &&
                !datiGenerici.RMS9090.HasValue &&
                !datiGenerici.CMSM.HasValue &&
                !datiGenerici.RiduzioneRetributiva &&
                !datiGenerici.RiduzioneRetributivaPercentuale.HasValue &&
                !datiGenerici.CodRicalcoloSentenza.HasValue &&
                !datiGenerici.ReqArt2DL503.HasValue
                )
            {
                return true;
            }
            else
                return false;
        }

        #region nested classes

        public class PensioniDatiGenerici
        {
            #region private properties
            private System.Nullable<char> _RegimeLiquidazione;

            private System.Nullable<int> _ContributiItalianiEdEsteriAl1295;

            private System.Nullable<int> _NSettFittiziePrepensionamento;

            private System.Nullable<char> _CodiceVirtuale;

            private System.Nullable<byte> _ConiugeSuperstite;

            private System.Nullable<System.DateTime> _DecorrenzaCodiceVirtuale;

            private System.Nullable<decimal> _ImportoPensioneEEInvalido;

            private System.Nullable<System.DateTime> _DataRicalcoloPrestazioneEE;

            private System.Nullable<int> _NContributiItalia;

            private System.Nullable<System.DateTime> _DecorrenzaBonus;

            private bool? _DeliberaCee126;

            private System.Nullable<decimal> _ImportoCristallizzazione3481;

            private int? _UfficioPagatoreArretratiEE;

            private string _CodiceScadenzaAssegno;

            private System.Nullable<int> _AnniDifferimento;

            private string _CodiciMotivazioniCi281;

            private string _CodiciMotivazioniCi282;

            private char? _CodiciCi21;

            private bool? _CodiceBloccoArretratiEE;

            private string _CodicePensioneRiliquidata;

            private char? _ApplicazioneSentenza49593;

            private System.Nullable<System.DateTime> _DecorrenzaArt2Dpcm;

            private System.Nullable<System.DateTime> _DataPrecedenteLiquidazione;

            private System.Nullable<System.DateTime> _DataArrivoDomanda;

            private System.Nullable<System.DateTime> _Data1Domanda;

            private int? _VVMisuraAl1292;

            private int? _VVMisuraDL50392;

            private int? _SettimanePerCalcoloContributivo;

            private int? _SettimaneItalianeDiritto;

            private decimal? _ImportoIVS;

            private bool? _MaternitaAcna;

            private decimal? _RMS8888;

            private decimal? _RMS9090;

            private decimal? _CMSM;

            private bool _RiduzioneRetributiva;

            private System.Nullable<decimal> _RiduzioneRetributivaPercentuale;

            private System.Nullable<decimal> _AnzAl95;

            private System.Nullable<decimal> _QuotaAl95;

            private System.Nullable<decimal> _ImportoAl200312;

            private long? _EnteCassa;

            private bool? _EnteIstruttoreExInpdap;

            private bool? _FacoltaComputo;

            private System.Nullable<System.DateTime> _ScadenzaAssegno;

            private System.Nullable<decimal> _ImportoUltimaRetribuzione;

            private System.Nullable<System.DateTime> _InizioUltimoLavoro;

            private System.Nullable<System.DateTime> _FineUltimoLavoro;

            private decimal? _ImportoLordoAllaDecorrenza;

            private decimal? _ImportoLordo;

            private short? _AnnoBancaFideiussoria;

            private byte? _ProgressivoBancaFideiussoria;

            private bool? _TipoCumulo;

            private char? _CumuloEsterno;

            private int? _SettimaneItalianeMisura;

            private byte? _CodRicalcoloSentenza;

            private decimal? _PL_Coeftrasf;

            private char? _TipologiaCumulo;

            private decimal? _ImportoMensileAllaDecorrenzaOriginaria;

            private decimal? _ImportoMensileAlGennaio2001;

            private decimal? _ImportoMensilePensioneEstera;

            //ENG - Gestione Nuovo Codice CI28
            private char? _CodiceCI28;

            private System.Nullable<byte> _CodiceConvenzioneAgo;

            private short? _TotaleSettimaneEstereUtiliPerDiritto;

            private int? _ContribuzioneEsteraTotale;

            #endregion private properties

            #region public properties
            public System.Nullable<char> RegimeLiquidazione { get { return _RegimeLiquidazione; } set { _RegimeLiquidazione = value; } }

            public System.Nullable<int> ContributiItalianiEdEsteriAl1295 { get { return _ContributiItalianiEdEsteriAl1295; } set { _ContributiItalianiEdEsteriAl1295 = value; } }

            public System.Nullable<int> NSettFittiziePrepensionamento { get { return _NSettFittiziePrepensionamento; } set { _NSettFittiziePrepensionamento = value; } }

            public System.Nullable<char> CodiceVirtuale { get { return _CodiceVirtuale; } set { _CodiceVirtuale = value; } }

            public System.Nullable<byte> ConiugeSuperstite { get { return _ConiugeSuperstite; } set { _ConiugeSuperstite = value; } }

            public System.Nullable<System.DateTime> DecorrenzaCodiceVirtuale { get { return _DecorrenzaCodiceVirtuale; } set { _DecorrenzaCodiceVirtuale = value; } }

            public System.Nullable<decimal> ImportoPensioneEEInvalido { get { return _ImportoPensioneEEInvalido; } set { _ImportoPensioneEEInvalido = value; } }

            public System.Nullable<System.DateTime> DataRicalcoloPrestazioneEE { get { return _DataRicalcoloPrestazioneEE; } set { _DataRicalcoloPrestazioneEE = value; } }

            public System.Nullable<int> NContributiItalia { get { return _NContributiItalia; } set { _NContributiItalia = value; } }

            public System.Nullable<System.DateTime> DecorrenzaBonus { get { return _DecorrenzaBonus; } set { _DecorrenzaBonus = value; } }

            public bool? DeliberaCee126 { get { return _DeliberaCee126; } set { _DeliberaCee126 = value; } }

            public System.Nullable<decimal> ImportoCristallizzazione3481 { get { return _ImportoCristallizzazione3481; } set { _ImportoCristallizzazione3481 = value; } }

            public int? UfficioPagatoreArretratiEE { get { return _UfficioPagatoreArretratiEE; } set { _UfficioPagatoreArretratiEE = value; } }

            public string CodiceScadenzaAssegno { get { return _CodiceScadenzaAssegno; } set { _CodiceScadenzaAssegno = value; } }

            public System.Nullable<int> AnniDifferimento { get { return _AnniDifferimento; } set { _AnniDifferimento = value; } }

            public string CodiciMotivazioniCi281 { get { return _CodiciMotivazioniCi281; } set { _CodiciMotivazioniCi281 = value; } }

            public string CodiciMotivazioniCi282 { get { return _CodiciMotivazioniCi282; } set { _CodiciMotivazioniCi282 = value; } }

            public char? CodiciCi21 { get { return _CodiciCi21; } set { _CodiciCi21 = value; } }

            public bool? CodiceBloccoArretratiEE { get { return _CodiceBloccoArretratiEE; } set { _CodiceBloccoArretratiEE = value; } }

            public string CodicePensioneRiliquidata { get { return _CodicePensioneRiliquidata; } set { _CodicePensioneRiliquidata = value; } }

            public char? ApplicazioneSentenza49593 { get { return _ApplicazioneSentenza49593; } set { _ApplicazioneSentenza49593 = value; } }

            public System.Nullable<System.DateTime> DecorrenzaArt2Dpcm { get { return _DecorrenzaArt2Dpcm; } set { _DecorrenzaArt2Dpcm = value; } }

            public System.Nullable<System.DateTime> DataPrecedenteLiquidazione { get { return _DataPrecedenteLiquidazione; } set { _DataPrecedenteLiquidazione = value; } }

            public System.Nullable<System.DateTime> DataArrivoDomanda { get { return _DataArrivoDomanda; } set { _DataArrivoDomanda = value; } }

            public System.Nullable<System.DateTime> Data1Domanda { get { return _Data1Domanda; } set { _Data1Domanda = value; } }

            public int? VVMisuraAl1292 { get { return _VVMisuraAl1292; } set { _VVMisuraAl1292 = value; } }

            public int? VVMisuraDL50392 { get { return _VVMisuraDL50392; } set { _VVMisuraDL50392 = value; } }

            public int? SettimanePerCalcoloContributivo { get { return _SettimanePerCalcoloContributivo; } set { _SettimanePerCalcoloContributivo = value; } }

            public int? SettimaneItalianeDiritto { get { return _SettimaneItalianeDiritto; } set { _SettimaneItalianeDiritto = value; } }

            public decimal? ImportoIVS { get { return _ImportoIVS; } set { _ImportoIVS = value; } }

            public bool? MaternitaAcna { get { return _MaternitaAcna; } set { _MaternitaAcna = value; } }

            public decimal? RMS8888 { get { return _RMS8888; } set { _RMS8888 = value; } }

            public decimal? RMS9090 { get { return _RMS9090; } set { _RMS9090 = value; } }

            public decimal? CMSM { get { return _CMSM; } set { _CMSM = value; } }

            public bool RiduzioneRetributiva { get { return _RiduzioneRetributiva; } set { _RiduzioneRetributiva = value; } }

            public System.Nullable<decimal> RiduzioneRetributivaPercentuale { get { return _RiduzioneRetributivaPercentuale; } set { _RiduzioneRetributivaPercentuale = value; } }

            public System.Nullable<decimal> AnzAl95 { get { return _AnzAl95; } set { _AnzAl95 = value; } }

            public System.Nullable<decimal> QuotaAl95 { get { return _QuotaAl95; } set { _QuotaAl95 = value; } }

            public System.Nullable<decimal> ImportoAl200312 { get { return _ImportoAl200312; } set { _ImportoAl200312 = value; } }

            public long? EnteCassa { get { return _EnteCassa; } set { _EnteCassa = value; } }

            public bool? EnteIstruttoreExInpdap { get { return _EnteIstruttoreExInpdap; } set { _EnteIstruttoreExInpdap = value; } }

            public bool? FacoltaComputo { get { return _FacoltaComputo; } set { _FacoltaComputo = value; } }

            public System.Nullable<System.DateTime> ScadenzaAssegno { get { return _ScadenzaAssegno; } set { _ScadenzaAssegno = value; } }

            public System.Nullable<decimal> ImportoUltimaRetribuzione { get { return _ImportoUltimaRetribuzione; } set { _ImportoUltimaRetribuzione = value; } }

            public System.Nullable<System.DateTime> InizioUltimoLavoro { get { return _InizioUltimoLavoro; } set { _InizioUltimoLavoro = value; } }

            public System.Nullable<System.DateTime> FineUltimoLavoro { get { return _FineUltimoLavoro; } set { _FineUltimoLavoro = value; } }

            public decimal? ImportoLordoAllaDecorrenza { get { return _ImportoLordoAllaDecorrenza; } set { _ImportoLordoAllaDecorrenza = value; } }

            public decimal? ImportoLordo { get { return _ImportoLordo; } set { _ImportoLordo = value; } }

            public short? AnnoBancaFideiussoria { get { return _AnnoBancaFideiussoria; } set { _AnnoBancaFideiussoria = value; } }

            public byte? ProgressivoBancaFideiussoria { get { return _ProgressivoBancaFideiussoria; } set { _ProgressivoBancaFideiussoria = value; } }

            public DateTime? DataAssunzioneCarico { get; set; }

            public bool? TipoCumulo { get { return _TipoCumulo; } set { _TipoCumulo = value; } }

            public char? CumuloEsterno { get { return _CumuloEsterno; } set { _CumuloEsterno = value; } }

            public int? SettimaneItalianeMisura { get { return _SettimaneItalianeMisura; } set { _SettimaneItalianeMisura = value; } }

            public string TipoCertificazioneFelpe { get; set; }

            public byte? CodRicalcoloSentenza { get { return _CodRicalcoloSentenza; } set { _CodRicalcoloSentenza = value; } }

            public short? ReqArt2DL503 { get; set; }

            public decimal? PL_Coeftrasf { get { return _PL_Coeftrasf; } set { _PL_Coeftrasf = value; } }

            public char? TipologiaCumulo { get { return _TipologiaCumulo; } set { _TipologiaCumulo = value; } }

            public decimal? ImportoMensileAllaDecorrenzaOriginaria { get { return _ImportoMensileAllaDecorrenzaOriginaria; } set { _ImportoMensileAllaDecorrenzaOriginaria = value; } }

            public decimal? ImportoMensileAlGennaio2001 { get { return _ImportoMensileAlGennaio2001; } set { _ImportoMensileAlGennaio2001 = value; } }

            //ENG - Gestione Pensione Estera
            public decimal? ImportoMensilePensioneEstera { get { return _ImportoMensilePensioneEstera; } set { _ImportoMensilePensioneEstera = value; } }

            //ENG - Gestione Nuovo Codice CI28
            public char? CodiceCI28 { get { return _CodiceCI28; } set { _CodiceCI28 = value; } }

            public System.Nullable<byte> CodiceConvenzioneAgo { get { return _CodiceConvenzioneAgo; } set { _CodiceConvenzioneAgo = value; } }

            public short? TotaleSettimaneEstereUtiliPerDiritto { get { return _TotaleSettimaneEstereUtiliPerDiritto; } set { _TotaleSettimaneEstereUtiliPerDiritto = value; } }

            public int? ContribuzioneEsteraTotale { get { return _ContribuzioneEsteraTotale; } set { _ContribuzioneEsteraTotale = value; } }

            #endregion public properties

            public override bool Equals(object obj)
            {
                return Utility.ConfrontaOggetti(this, obj);
            }

            //TODO GETHASHCODE
            //public override int GetHashCode()
            //{
            //    int hash = 13;
            //    hash = (hash * 7) + (this._RegimeLiquidazione != null ? this._RegimeLiquidazione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ContributiItalianiEdEsteriAl1295 != null ? this._ContributiItalianiEdEsteriAl1295.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._NSettFittiziePrepensionamento != null ? this._NSettFittiziePrepensionamento.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodiceVirtuale != null ? this._CodiceVirtuale.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ConiugeSuperstite != null ? this._ConiugeSuperstite.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._DecorrenzaCodiceVirtuale != null ? this._DecorrenzaCodiceVirtuale.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ImportoPensioneEEInvalido != null ? this._ImportoPensioneEEInvalido.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._DataRicalcoloPrestazioneEE != null ? this._DataRicalcoloPrestazioneEE.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._NContributiItalia != null ? this._NContributiItalia.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._DecorrenzaBonus != null ? this._DecorrenzaBonus.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._DeliberaCee126 != null ? this._DeliberaCee126.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ImportoCristallizzazione3481 != null ? this._ImportoCristallizzazione3481.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._UfficioPagatoreArretratiEE != null ? this._UfficioPagatoreArretratiEE.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodiceScadenzaAssegno != null ? this._CodiceScadenzaAssegno.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._AnniDifferimento != null ? this._AnniDifferimento.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodiciMotivazioniCi281 != null ? this._CodiciMotivazioniCi281.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodiciMotivazioniCi282 != null ? this._CodiciMotivazioniCi282.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodiciCi21 != null ? this._CodiciCi21.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodiceBloccoArretratiEE != null ? this._CodiceBloccoArretratiEE.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodicePensioneRiliquidata != null ? this._CodicePensioneRiliquidata.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ApplicazioneSentenza49593 != null ? this._ApplicazioneSentenza49593.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._DecorrenzaArt2Dpcm != null ? this._DecorrenzaArt2Dpcm.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._DataPrecedenteLiquidazione != null ? this._DataPrecedenteLiquidazione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._DataArrivoDomanda != null ? this._DataArrivoDomanda.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._Data1Domanda != null ? this._Data1Domanda.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._VVMisuraAl1292 != null ? this._VVMisuraAl1292.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._VVMisuraDL50392 != null ? this._VVMisuraDL50392.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._SettimanePerCalcoloContributivo != null ? this._SettimanePerCalcoloContributivo.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._SettimaneItalianeDiritto != null ? this._SettimaneItalianeDiritto.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ImportoIVS != null ? this._ImportoIVS.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._MaternitaAcna != null ? this._MaternitaAcna.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._RMS8888 != null ? this._RMS8888.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._RMS9090 != null ? this._RMS9090.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CMSM != null ? this._CMSM.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._RiduzioneRetributiva.GetHashCode());
            //    hash = (hash * 7) + (this._RiduzioneRetributivaPercentuale != null ? this._RiduzioneRetributivaPercentuale.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._AnzAl95 != null ? this._AnzAl95.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._QuotaAl95 != null ? this._QuotaAl95.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._EnteCassa != null ? this._EnteCassa.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._EnteIstruttoreExInpdap != null ? this._EnteIstruttoreExInpdap.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._FacoltaComputo != null ? this._FacoltaComputo.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ScadenzaAssegno != null ? this._ScadenzaAssegno.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ImportoUltimaRetribuzione != null ? this._ImportoUltimaRetribuzione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._InizioUltimoLavoro != null ? this._InizioUltimoLavoro.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._FineUltimoLavoro != null ? this._FineUltimoLavoro.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ImportoLordoAllaDecorrenza != null ? this._ImportoLordoAllaDecorrenza.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._AnnoBancaFideiussoria != null ? this._AnnoBancaFideiussoria.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ProgressivoBancaFideiussoria != null ? this._ProgressivoBancaFideiussoria.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._AliquotaMediaINPDAP != null ? this._AliquotaMediaINPDAP.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._DataRivalsaINPDAP != null ? this._DataRivalsaINPDAP.GetHashCode() : 0);
            //    return hash;

            //}
        }

        #endregion nested classes

    }
}
