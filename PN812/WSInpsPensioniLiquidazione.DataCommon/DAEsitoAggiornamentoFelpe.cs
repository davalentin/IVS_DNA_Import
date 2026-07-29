using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using INPS.DNA.Data;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAEsitoAggiornamentoFelpe
    {
        public static void SalvaEsitoAggiornamentoFelpe(EsitoAggiornamentoFelpe esitoAggFelpe)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertEsitoAggiornamentoFelpe(esitoAggFelpe.Ndomus, esitoAggFelpe.ProgStorico, esitoAggFelpe.TipoApp, esitoAggFelpe.Esito, esitoAggFelpe.Errore);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertEsitoAggiornamentoFelpe");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaEsitoAggiornamentoFelpe(string tipoApp)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllEsitoAggiornamentoFelpe(tipoApp);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllEsitoAggiornamentoFelpe");
                }
                db.Connection.Close();
            }
        }

        public static void GetEsitoAggiornamentoFelpe(string tipoApp, out List<EsitoAggiornamentoFelpe> lstEsitoAggiornamentoFelpe)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lstEsitoAggiornamentoFelpe = (from elem in db.EsitoAggiornamentoFelpes
                                                   where elem.TipoApp == tipoApp
                                                   select elem).ToList<EsitoAggiornamentoFelpe>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
