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
    public class GestionePagamento
    {
        public static void GetPagamentoByIdPensione(Int64 idPensione, out DatiPagamento datiPagamento)
        {
            Pagamento pagamento = null;
            datiPagamento = null;
            DAGestionePagamento.GetPagamentoByIdPensione(idPensione, out pagamento);
            if (pagamento == null)
                return;
            datiPagamento = new DatiPagamento();
            Utility.ValorizzaOggetti(pagamento, datiPagamento);
        }

        public static void SalvaPagamento(long idPensione, DatiPagamento datiPagamento)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                Pagamento pagamento = new Pagamento();
                Utility.ValorizzaOggetti(datiPagamento, pagamento);
                pagamento.IdPensione = idPensione;
                DAGestionePagamento.SalvaPagamento(pagamento);

                transactionScope.Complete();
            }
        }

        public static void EliminaPagamentoByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestionePagamento.EliminaPagamentoByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #region nested class
        public class DatiPagamento
        {
            public DatiPagamento()
            { }

            public DatiPagamento(string iban, System.Nullable<System.DateTime> decorrenzaPagamento, System.Nullable<char> modalitaPagamento,
                string ufficioPagatore, System.Nullable<int> abi, System.Nullable<int> cab, System.Nullable<int> frazionario, string bic, string libretto, System.Nullable<byte> ultimoMesePagamento, 
                System.Nullable<decimal> importoPensioneAltroEnte, System.Nullable<decimal> quotaFissa, System.Nullable<decimal> percentuale, System.Nullable<decimal> quotaConcorsoAltroEnte, 
                System.Nullable<bool> trattenutaInpdap, System.Nullable<char> tipoPagamento, string statoEstero, System.Nullable<System.DateTime>dataRinunciaTrattenutaInpdap, string nomeUfficioPagatore, 
                string agenziaUfficioPagatore, string capUfficioPagatore, string cittaUfficioPagatore, string indirizzoUfficioPagatore, string codCatastaleEstero, bool isFromWebDom)
            {
                this._IBAN = !string.IsNullOrEmpty(iban) ? iban.ToUpperInvariant() : iban;

                this._DecorrenzaPagamento = decorrenzaPagamento;

                this._ModalitaPagamento = modalitaPagamento;

                this._UfficioPagatore = !string.IsNullOrEmpty(ufficioPagatore) ? ufficioPagatore.ToUpperInvariant() : ufficioPagatore;

                this._ABI = abi;

                this._CAB = cab;

                this._Frazionario = frazionario;

                this._BIC = !string.IsNullOrEmpty(bic) ? bic.ToUpperInvariant() : bic;

                this._Libretto = !string.IsNullOrEmpty(libretto) ? libretto.ToUpperInvariant() : libretto;

                this._UltimoMesePagamento = ultimoMesePagamento;

                this._ImportoPensioneAltroEnte = importoPensioneAltroEnte;

                this._QuotaFissa = quotaFissa;

                this._Percentuale = percentuale;

                this._QuotaConcorsoAltroEnte = quotaConcorsoAltroEnte;

                this._TrattenutaInpdap = trattenutaInpdap;

                this._TipoPagamento = tipoPagamento;

                this._StatoEstero = !string.IsNullOrEmpty(statoEstero) ? statoEstero.ToUpperInvariant() : statoEstero;

                this._DataRinunciaTrattenutaInpdap = dataRinunciaTrattenutaInpdap;

                this._NomeUfficioPagatore = !string.IsNullOrEmpty(nomeUfficioPagatore) ? nomeUfficioPagatore.ToUpperInvariant() : nomeUfficioPagatore;

                this._AgenziaUfficioPagatore = !string.IsNullOrEmpty(agenziaUfficioPagatore) ? agenziaUfficioPagatore.ToUpperInvariant() : agenziaUfficioPagatore;

                this._CapUfficioPagatore = !string.IsNullOrEmpty(capUfficioPagatore) ? capUfficioPagatore.ToUpperInvariant() : capUfficioPagatore;

                this._CittaUfficioPagatore = !string.IsNullOrEmpty(cittaUfficioPagatore) ? cittaUfficioPagatore.ToUpperInvariant() : cittaUfficioPagatore;

                this._IndirizzoUfficioPagatore = !string.IsNullOrEmpty(indirizzoUfficioPagatore) ? indirizzoUfficioPagatore.ToUpperInvariant() : indirizzoUfficioPagatore;

                this._CodCatastaleEstero = !string.IsNullOrEmpty(codCatastaleEstero) ? codCatastaleEstero.ToUpperInvariant() : codCatastaleEstero;

                this._IsFromWebDom = isFromWebDom;
            }

            #region private properties
            private string _IBAN;

            private System.Nullable<System.DateTime> _DecorrenzaPagamento;

            private System.Nullable<char> _ModalitaPagamento;

            private string _UfficioPagatore;

            private System.Nullable<int> _ABI;

            private System.Nullable<int> _CAB;

            private System.Nullable<int> _Frazionario;

            private string _BIC;

            private string _Libretto;

            private System.Nullable<byte> _UltimoMesePagamento;

            private System.Nullable<decimal> _ImportoPensioneAltroEnte;

            private System.Nullable<decimal> _QuotaFissa;

            private System.Nullable<decimal> _Percentuale;

            private System.Nullable<decimal> _QuotaConcorsoAltroEnte;

            private System.Nullable<bool> _TrattenutaInpdap;

            private System.Nullable<char> _TipoPagamento;

            private string _StatoEstero;

            private System.Nullable<System.DateTime> _DataRinunciaTrattenutaInpdap;

            private string _NomeUfficioPagatore;

            private string _AgenziaUfficioPagatore;

            private string _CapUfficioPagatore;

            private string _CittaUfficioPagatore;

            private string _IndirizzoUfficioPagatore;

            private string _CodCatastaleEstero;

            private bool _IsFromWebDom;
            #endregion private properties

            #region public properties
            public string IBAN { get { return _IBAN; } set { _IBAN = value; } }

            public System.Nullable<System.DateTime> DecorrenzaPagamento { get { return _DecorrenzaPagamento; } set { _DecorrenzaPagamento = value; } }

            public System.Nullable<char> ModalitaPagamento { get { return _ModalitaPagamento; } set { _ModalitaPagamento = value; } }

            public string UfficioPagatore { get { return _UfficioPagatore; } set { _UfficioPagatore = value; } }

            public System.Nullable<int> ABI { get { return _ABI; } set { _ABI = value; } }

            public System.Nullable<int> CAB { get { return _CAB; } set { _CAB = value; } }

            public System.Nullable<int> Frazionario { get { return _Frazionario; } set { _Frazionario = value; } }

            public string BIC { get { return _BIC; } set { _BIC = value; } }

            public string Libretto { get { return _Libretto; } set { _Libretto = value; } }

            public System.Nullable<byte> UltimoMesePagamento { get { return _UltimoMesePagamento; } set { _UltimoMesePagamento = value; } }

            public System.Nullable<decimal> ImportoPensioneAltroEnte { get { return _ImportoPensioneAltroEnte; } set { _ImportoPensioneAltroEnte = value; } }

            public System.Nullable<decimal> QuotaFissa { get { return _QuotaFissa; } set { _QuotaFissa = value; } }

            public System.Nullable<decimal> Percentuale { get { return _Percentuale; } set { _Percentuale = value; } }

            public System.Nullable<decimal> QuotaConcorsoAltroEnte { get { return _QuotaConcorsoAltroEnte; } set { _QuotaConcorsoAltroEnte = value; } }

            public System.Nullable<bool> TrattenutaInpdap { get { return _TrattenutaInpdap; } set { _TrattenutaInpdap = value; } }

            public System.Nullable<char> TipoPagamento { get { return _TipoPagamento; } set { _TipoPagamento = value; } }

            public string StatoEstero { get { return _StatoEstero; } set { _StatoEstero = value; } }

            public System.Nullable<System.DateTime> DataRinunciaTrattenutaInpdap { get { return _DataRinunciaTrattenutaInpdap; } set { _DataRinunciaTrattenutaInpdap = value; } }

            public string NomeUfficioPagatore { get { return _NomeUfficioPagatore; } set { _NomeUfficioPagatore = value; } }

            public string AgenziaUfficioPagatore { get { return _AgenziaUfficioPagatore; } set { _AgenziaUfficioPagatore = value; } }

            public string CapUfficioPagatore { get { return _CapUfficioPagatore; } set { _CapUfficioPagatore = value; } }

            public string CittaUfficioPagatore { get { return _CittaUfficioPagatore; } set { _CittaUfficioPagatore = value; } }

            public string IndirizzoUfficioPagatore { get { return _IndirizzoUfficioPagatore; } set { _IndirizzoUfficioPagatore = value; } } 

            public string CodCatastaleEstero { get { return _CodCatastaleEstero; } set { _CodCatastaleEstero = value; } }

            public bool IsFromWebDom { get { return _IsFromWebDom; } set { _IsFromWebDom = value; } }

            #endregion public properties

            public override bool Equals(object obj)
            {
                DatiPagamento pagamento = (DatiPagamento)obj;
                try
                {
                    if (this._DecorrenzaPagamento != pagamento._DecorrenzaPagamento ||
                        this._ModalitaPagamento != pagamento._ModalitaPagamento ||
                        this._UfficioPagatore != pagamento._UfficioPagatore ||
                        this._ABI != pagamento._ABI ||
                        this._CAB != pagamento._CAB ||
                        this._Frazionario != pagamento._Frazionario ||
                        this._BIC != pagamento._BIC ||
                        this._Libretto != pagamento._Libretto ||
                        this._UltimoMesePagamento != pagamento._UltimoMesePagamento ||
                        this._ImportoPensioneAltroEnte != pagamento._ImportoPensioneAltroEnte ||
                        this._QuotaFissa != pagamento._QuotaFissa ||
                        this._Percentuale != pagamento._Percentuale ||
                        this._QuotaConcorsoAltroEnte != pagamento._QuotaConcorsoAltroEnte ||
                        this._TrattenutaInpdap != pagamento._TrattenutaInpdap ||
                        this._TipoPagamento != pagamento._TipoPagamento ||
                        this._StatoEstero != pagamento._StatoEstero ||
                        this._DataRinunciaTrattenutaInpdap != pagamento._DataRinunciaTrattenutaInpdap ||
                        this._NomeUfficioPagatore != pagamento._NomeUfficioPagatore ||
                        this._AgenziaUfficioPagatore != pagamento._AgenziaUfficioPagatore ||
                        this._CapUfficioPagatore != pagamento._CapUfficioPagatore ||
                        this._CittaUfficioPagatore != pagamento._CittaUfficioPagatore ||
                        this._IndirizzoUfficioPagatore != pagamento._IndirizzoUfficioPagatore ||
                        this._CodCatastaleEstero != pagamento._CodCatastaleEstero ||
                        this._IsFromWebDom != pagamento._IsFromWebDom)
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
        }
        #endregion nested class
    }
}



