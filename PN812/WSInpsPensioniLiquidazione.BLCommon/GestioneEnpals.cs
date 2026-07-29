using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneEnpals
    {
        public static void GetDatiEnpalsByIdPensione(long idPensione, out DatiEnpals datiEnpals)
        {
            Enpal enpals = null;
            datiEnpals = null;
            DAGestioneEnpals.GetEnpalsByIdPensione(idPensione, out enpals);
            if (enpals == null)
                return;
            datiEnpals = new DatiEnpals();
            Utility.ValorizzaOggetti(enpals, datiEnpals);
        }

        public static void SalvaDatiEnpalsEnpals(DatiEnpals datiEnpals)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                Enpal enpals = new Enpal();
                Utility.ValorizzaOggetti(datiEnpals, enpals);
                DAGestioneEnpals.SalvaEnpals(enpals);
                transactionScope.Complete();
            }
        }

        public static void EliminaDatiEnpalsByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneEnpals.EliminaEnpalsByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #region nested class

        public class DatiEnpals
        {
            #region private properties

            private long _IdPensione;
            private System.Nullable<short> _AADiritto;
            private System.Nullable<short> _MMDiritto;
            private System.Nullable<char> _RaggruppamentoPrevalente;
            private System.Nullable<char> _GruppoPrevalente;
            private System.Nullable<char> _GruppoDiritto;
            private System.Nullable<int> _NTotContributi;
            private System.Nullable<int> _NTotContributiEnpals;
            private System.Nullable<short> _EtaDirittoAA;
            private System.Nullable<short> _EtaDirittoMM;
            private System.Nullable<short> _EtaMisuraAA;
            private System.Nullable<short> _EtaMisuraMM;
            private string _Qualifica;
            private System.Nullable<System.DateTime> _DataFinestra;
            private System.Nullable<int> _NContributiMisura;
            private System.Nullable<int> _NTotDiritto;
            private System.Nullable<int> _NTotQualifica;
            private System.Nullable<int> _NContributiQuinquennio;
            private System.Nullable<int> _NContributiTriennio;
            private System.Nullable<int> _NContributiNL222;
            private System.Nullable<int> _NContributiNL155;
            private string _CodiceDeroga1;
            private string _CodiceDeroga2;
            private string _CodiceDeroga3;
            private string _CodiceDeroga4;
            private System.Nullable<int> _NumeroContributiNLNonVedenti;
            private System.Nullable<char> _IndicatoreInvalidita80;
            private System.Nullable<decimal> _ImportoPensione;
            private string _TipoLiquidazione;
            private char? _CodiceRitorno;
            private string _CodiceTipoDomanda;
            private System.Nullable<char> _TipoPensione;
            private decimal? _ImportoPensione707;
            private string _TipoLiquidazioneProvvisoria;
            private string _DecorrenzaImportoPensione;
            private string _DecorrenzaImportoPensione707;
            private string _GP1AN87B;
            private System.Nullable<short> _AnzianitaContributiva;
            private decimal? _ImportoIIS;
            private DateTime? _DecorrenzaImportoIIS;

            #endregion private properties

            #region public properties

            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public short? AADiritto { get { return _AADiritto; } set { _AADiritto = value; } }
            public short? MMDiritto { get { return _MMDiritto; } set { _MMDiritto = value; } }
            public char? RaggruppamentoPrevalente { get { return _RaggruppamentoPrevalente; } set { _RaggruppamentoPrevalente = value; } }
            public char? GruppoPrevalente { get { return _GruppoPrevalente; } set { _GruppoPrevalente = value; } }
            public char? GruppoDiritto { get { return _GruppoDiritto; } set { _GruppoDiritto = value; } }
            public int? NTotContributi { get { return _NTotContributi; } set { _NTotContributi = value; } }
            public int? NTotContributiEnpals { get { return _NTotContributiEnpals; } set { _NTotContributiEnpals = value; } }
            public short? EtaDirittoAA { get { return _EtaDirittoAA; } set { _EtaDirittoAA = value; } }
            public short? EtaDirittoMM { get { return _EtaDirittoMM; } set { _EtaDirittoMM = value; } }
            public short? EtaMisuraAA { get { return _EtaMisuraAA; } set { _EtaMisuraAA = value; } }
            public short? EtaMisuraMM { get { return _EtaMisuraMM; } set { _EtaMisuraMM = value; } }
            public string Qualifica { get { return _Qualifica; } set { _Qualifica = value; } }
            public DateTime? DataFinestra { get { return _DataFinestra; } set { _DataFinestra = value; } }
            public int? NContributiMisura { get { return _NContributiMisura; } set { _NContributiMisura = value; } }
            public int? NTotDiritto { get { return _NTotDiritto; } set { _NTotDiritto = value; } }
            public int? NTotQualifica { get { return _NTotQualifica; } set { _NTotQualifica = value; } }
            public int? NContributiQuinquennio { get { return _NContributiQuinquennio; } set { _NContributiQuinquennio = value; } }
            public int? NContributiTriennio { get { return _NContributiTriennio; } set { _NContributiTriennio = value; } }
            public int? NContributiNL222 { get { return _NContributiNL222; } set { _NContributiNL222 = value; } }
            public int? NContributiNL155 { get { return _NContributiNL155; } set { _NContributiNL155 = value; } }
            public string CodiceDeroga1 { get { return _CodiceDeroga1; } set { _CodiceDeroga1 = value; } }
            public string CodiceDeroga2 { get { return _CodiceDeroga2; } set { _CodiceDeroga2 = value; } }
            public string CodiceDeroga3 { get { return _CodiceDeroga3; } set { _CodiceDeroga3 = value; } }
            public string CodiceDeroga4 { get { return _CodiceDeroga4; } set { _CodiceDeroga4 = value; } }
            public int? NumeroContributiNLNonVedenti { get { return _NumeroContributiNLNonVedenti; } set { _NumeroContributiNLNonVedenti = value; } }
            public char? IndicatoreInvalidita80 { get { return _IndicatoreInvalidita80; } set { _IndicatoreInvalidita80 = value; } }
            public decimal? ImportoPensione { get { return _ImportoPensione; } set { _ImportoPensione = value; } }
            public string TipoLiquidazione { get { return _TipoLiquidazione; } set { _TipoLiquidazione = value; } }
            public char? CodiceRitorno { get { return _CodiceRitorno; } set { _CodiceRitorno = value; } }
            public System.Nullable<char> TipoPensione { get { return _TipoPensione; } set { _TipoPensione = value; } }
            public string CodiceTipoDomanda { get { return _CodiceTipoDomanda; } set { _CodiceTipoDomanda = value; } }
            public decimal? ImportoPensione707 { get { return _ImportoPensione707; } set { _ImportoPensione707 = value; } }
            public string TipoLiquidazioneProvvisoria { get { return _TipoLiquidazioneProvvisoria; } set { _TipoLiquidazioneProvvisoria = value; } }
            public string DecorrenzaImportoPensione { get { return _DecorrenzaImportoPensione; } set { _DecorrenzaImportoPensione = value; } }
            public string DecorrenzaImportoPensione707 { get { return _DecorrenzaImportoPensione707; } set { _DecorrenzaImportoPensione707 = value; } }
            public string GP1AN87B { get { return _GP1AN87B; } set { _GP1AN87B = value; } }
            public short? AnzianitaContributiva { get { return _AnzianitaContributiva; } set { _AnzianitaContributiva = value; } }
            public decimal? ImportoIIS { get { return _ImportoIIS; } set { _ImportoIIS = value; } }
            public DateTime? DecorrenzaImportoIIS { get { return _DecorrenzaImportoIIS; } set { _DecorrenzaImportoIIS = value; } }

            #endregion public properties

            public bool IsDatiEnpalsNull()
            {
                if (!this._AADiritto.HasValue && !this._MMDiritto.HasValue && !this._RaggruppamentoPrevalente.HasValue && !this._GruppoPrevalente.HasValue &&
                    !this._GruppoDiritto.HasValue && !this._NTotContributi.HasValue && !this._NTotContributiEnpals.HasValue && !this._EtaDirittoAA.HasValue &&
                    !this._EtaDirittoMM.HasValue && !this._EtaMisuraAA.HasValue && !this._EtaMisuraMM.HasValue && string.IsNullOrEmpty(this._Qualifica) &&
                    !this._DataFinestra.HasValue && !this._NContributiMisura.HasValue && !this._NTotDiritto.HasValue && !this._NTotQualifica.HasValue &&
                    !this._NContributiQuinquennio.HasValue && !this._NContributiTriennio.HasValue && !this._NContributiNL222.HasValue && !this._NContributiNL155.HasValue &&
                    string.IsNullOrEmpty(this._CodiceDeroga1) && string.IsNullOrEmpty(this._CodiceDeroga2) && string.IsNullOrEmpty(this._CodiceDeroga3) &&
                    string.IsNullOrEmpty(this._CodiceDeroga4) && !this._NumeroContributiNLNonVedenti.HasValue && !this.IndicatoreInvalidita80.HasValue &&
                    !this._ImportoPensione.HasValue && string.IsNullOrEmpty(this._TipoLiquidazione) && !this._CodiceRitorno.HasValue && string.IsNullOrEmpty(this._CodiceTipoDomanda) &&
                    !this._TipoPensione.HasValue && !this._ImportoPensione707.HasValue && string.IsNullOrEmpty(this._TipoLiquidazioneProvvisoria) && string.IsNullOrEmpty(this._DecorrenzaImportoPensione) &&
                    string.IsNullOrEmpty(this._DecorrenzaImportoPensione707) && string.IsNullOrEmpty(this._GP1AN87B) && !this.AnzianitaContributiva.HasValue && !this.ImportoIIS.HasValue && 
                    !this.DecorrenzaImportoIIS.HasValue)
                    return true;
                else
                    return false;
            }

            public bool IsIstruttoriaNull()
            {
                return string.IsNullOrEmpty(this._CodiceDeroga1) &&
                       string.IsNullOrEmpty(this._CodiceDeroga2) &&
                       string.IsNullOrEmpty(this._CodiceDeroga3) &&
                       string.IsNullOrEmpty(this._CodiceDeroga4);
            }
        }

        #endregion nested class
    }
}
