using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using INPS.DNA.Data;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public static class DAEsitoAggiornamentoINPDAP
    {
        public static void SalvaEsitoAggiornamentoINPDAP(EsitoAggiornamentoINPDAP esitoAggINPDAP)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertEsitoAggiornamentoINPDAP(esitoAggINPDAP.Ndomus, esitoAggINPDAP.ProgStorico, esitoAggINPDAP.TipoApp, esitoAggINPDAP.Esito, esitoAggINPDAP.Errore);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertEsitoAggiornamentoINPDAP");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaEsitoAggiornamentoINPDAP(string tipoApp)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllEsitoAggiornamentoINPDAP(tipoApp);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllEsitoAggiornamentoINPDAP");
                }
                db.Connection.Close();
            }
        }

        public static void GetEsitoAggiornamentoINPDAP(string tipoApp, out List<EsitoAggiornamentoINPDAP> lstEsitoAggiornamentoINPDAP)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lstEsitoAggiornamentoINPDAP = (from elem in db.EsitoAggiornamentoINPDAPs
                                                where elem.TipoApp == tipoApp
                                                select elem).ToList<EsitoAggiornamentoINPDAP>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
