using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using INPS.DNA.Data;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public static class DAEsitoAggiornamentoPianiDiPagamento
    {
        public static void SalvaEsitoAggiornamentoPianiDiPagamento(EsitoAggiornamentoPianiDiPagamento esitoAggPianiDiPagamento)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertEsitoAggiornamentoPianiDiPagamento(esitoAggPianiDiPagamento.NDomus, esitoAggPianiDiPagamento.ProgStorico, esitoAggPianiDiPagamento.TipoApp, esitoAggPianiDiPagamento.Esito, esitoAggPianiDiPagamento.Errore);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertEsitoAggiornamentoPianiDiPagamento");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaEsitoAggiornamentoPianiDiPagamento(string tipoApp)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllEsitoAggiornamentoPianiDiPagamento(tipoApp);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllEsitoAggiornamentoPianiDiPagamento");
                }
                db.Connection.Close();
            }
        }

        public static void GetEsitoAggiornamentoPianiDiPagamento(string tipoApp, out List<EsitoAggiornamentoPianiDiPagamento> lstEsitoAggiornamentoPianiDiPagamento)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lstEsitoAggiornamentoPianiDiPagamento = (from elem in db.EsitoAggiornamentoPianiDiPagamentos
                                                             where elem.TipoApp == tipoApp
                                                         select elem).ToList<EsitoAggiornamentoPianiDiPagamento>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}

