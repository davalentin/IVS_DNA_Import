using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using INPS.DNA.Data;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public static class DAEsitoAggiornamentoSAI
    {
        public static void SalvaEsitoAggiornamentoSAI(EsitoAggiornamentoSAI esitoAggSAI)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertEsitoAggiornamentoSAI(esitoAggSAI.Ndomus, esitoAggSAI.ProgStorico, esitoAggSAI.TipoApp, esitoAggSAI.Esito, esitoAggSAI.Errore);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertEsitoAggiornamentoSai");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaEsitoAggiornamentoSAI(string tipoApp)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllEsitoAggiornamentoSAI(tipoApp);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllEsitoAggiornamentoSai");
                }
                db.Connection.Close();
            }
        }

        public static void GetEsitoAggiornamentoSAI(string tipoApp, out List<EsitoAggiornamentoSAI> lstEsitoAggiornamentoSAI)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lstEsitoAggiornamentoSAI = (from elem in db.EsitoAggiornamentoSAIs
                                                  where elem.TipoApp == tipoApp
                                                  select elem).ToList<EsitoAggiornamentoSAI>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
