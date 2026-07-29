using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using INPS.DNA.Data;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAEsitoAggiornamentoTot
    {
        public static void SalvaEsitoAggiornamentoTot(EsitoAggiornamentoTot esitoAggTot)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertEsitoAggiornamentoTot(esitoAggTot.Ndomus, esitoAggTot.ProgStorico, esitoAggTot.TipoApp, esitoAggTot.Esito, esitoAggTot.Errore);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertEsitoAggiornamentoTot");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaEsitoAggiornamentoTot(string tipoApp)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllEsitoAggiornamentoTot(tipoApp);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllEsitoAggiornamentoTot");
                }
                db.Connection.Close();
            }
        }

        public static void GetEsitoAggiornamentoTot(string tipoApp, out List<EsitoAggiornamentoTot> lstEsitoAggiornamentoTot)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lstEsitoAggiornamentoTot = (from elem in db.EsitoAggiornamentoTots
                                                   where elem.TipoApp == tipoApp
                                                   select elem).ToList<EsitoAggiornamentoTot>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
