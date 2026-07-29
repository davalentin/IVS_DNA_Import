using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using INPS.DNA.Data;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAEsitoAggiornamentoOneri
    {
        public static void SalvaEsitoAggiornamentoOneri(EsitoAggiornamentoOneri esitoAggOneri)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertEsitoAggiornamentoOneri(esitoAggOneri.Ndomus, esitoAggOneri.ProgStorico, esitoAggOneri.TipoApp, esitoAggOneri.Esito, esitoAggOneri.Errore);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertEsitoAggiornamentoOneri");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaEsitoAggiornamentoOneri(string tipoApp)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllEsitoAggiornamentoOneri(tipoApp);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllEsitoAggiornamentoOneri");
                }
                db.Connection.Close();
            }
        }


        public static void GetEsitoAggiornamentoOneri(string tipoApp, out List<EsitoAggiornamentoOneri> lstEsitoAggiornamentoOneri)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lstEsitoAggiornamentoOneri = (from elem in db.EsitoAggiornamentoOneris
                                                   where elem.TipoApp == tipoApp
                                                   select elem).ToList<EsitoAggiornamentoOneri>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
