using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using INPS.DNA.Data;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public static class DAEsitoAggiornamentoNoteDiDebito
    {
        public static void SalvaEsitoAggiornamentoNoteDiDebito(EsitoAggiornamentoNoteDiDebito esitoAggNoteDiDebito)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertEsitoAggiornamentoNoteDiDebito(esitoAggNoteDiDebito.Ndomus, esitoAggNoteDiDebito.ProgStorico, esitoAggNoteDiDebito.TipoApp, esitoAggNoteDiDebito.Esito, esitoAggNoteDiDebito.Errore);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertEsitoAggiornamentoNoteDiDebito");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaEsitoAggiornamentoNoteDiDebito(string tipoApp)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllEsitoAggiornamentoNoteDiDebito(tipoApp);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllEsitoAggiornamentoNoteDiDebito");
                }
                db.Connection.Close();
            }
        }

        public static void GetEsitoAggiornamentoNoteDiDebito(string tipoApp, out List<EsitoAggiornamentoNoteDiDebito> lstEsitoAggiornamentoNoteDiDebito)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lstEsitoAggiornamentoNoteDiDebito = (from elem in db.EsitoAggiornamentoNoteDiDebitos
                                                where elem.TipoApp == tipoApp
                                                select elem).ToList<EsitoAggiornamentoNoteDiDebito>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
