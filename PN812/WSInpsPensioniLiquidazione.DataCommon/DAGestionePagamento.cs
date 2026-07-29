using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestionePagamento
    {
        public static void GetPagamentoByIdPensione(Int64 idPensione, out Pagamento pagamento)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    pagamento = (from pag in db.Pagamentos
                                 where pag.IdPensione == idPensione
                                 select pag).SingleOrDefault<Pagamento>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaPagamentoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePagamento(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePagamento");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaPagamento(Pagamento pagamento)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPagamento(pagamento.IdPensione, pagamento.IBAN, pagamento.DecorrenzaPagamento, pagamento.ModalitaPagamento,
                    pagamento.UfficioPagatore, pagamento.ABI, pagamento.CAB, pagamento.Frazionario, pagamento.BIC, pagamento.Libretto, pagamento.UltimoMesePagamento, pagamento.ImportoPensioneAltroEnte,
                    pagamento.QuotaFissa, pagamento.Percentuale, pagamento.QuotaConcorsoAltroEnte, pagamento.TrattenutaInpdap,
                    pagamento.TipoPagamento, pagamento.StatoEstero, pagamento.DataRinunciaTrattenutaInpdap, pagamento.NomeUfficioPagatore, pagamento.AgenziaUfficioPagatore,
                    pagamento.CapUfficioPagatore, pagamento.CittaUfficioPagatore, pagamento.IndirizzoUfficioPagatore, pagamento.CodCatastaleEstero, pagamento.IsFromWebDom);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPagamento");
                }
                db.Connection.Close();
            }
        }
    }
}