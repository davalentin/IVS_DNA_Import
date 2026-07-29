using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using INPS.DNA.Data;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAEsitoAggiornamentoCumulo
    {
        public static void SalvaEsitoAggiornamentoCumulo(EsitoAggiornamentoCumulo esitoAggCumulo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertEsitoAggiornamentoCumulo(esitoAggCumulo.Ndomus, esitoAggCumulo.ProgStorico, esitoAggCumulo.TipoApp, esitoAggCumulo.Esito, esitoAggCumulo.Errore);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertEsitoAggiornamentoCumulo");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaEsitoAggiornamentoCumulo(string tipoApp)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllEsitoAggiornamentoCumulo(tipoApp);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllEsitoAggiornamentoCumulo");
                }
                db.Connection.Close();
            }
        }

        public static void GetEsitoAggiornamentoCumulo(string tipoApp, out List<EsitoAggiornamentoCumulo> lstEsitoAggiornamentoCumulo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lstEsitoAggiornamentoCumulo = (from elem in db.EsitoAggiornamentoCumulos
                                                   where elem.TipoApp == tipoApp
                                                   select elem).ToList<EsitoAggiornamentoCumulo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
