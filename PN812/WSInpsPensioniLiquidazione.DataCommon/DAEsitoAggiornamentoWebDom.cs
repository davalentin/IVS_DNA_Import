using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public static class DAEsitoAggiornamentoWebDom
    {
        public static void SalvaEsitoAggiornamentoWebDom(EsitoAggiornamentoWebDom esitoAggWebDom)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertEsitoAggiornamentoWebDom(esitoAggWebDom.Ndomus,esitoAggWebDom.TipoApp,esitoAggWebDom.Esito,esitoAggWebDom.Errore);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertEsitoAggiornamentoWebDom");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaEsitoAggiornamentoWebDom(string tipoApp)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllEsitoAggiornamentoWebDom(tipoApp);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllEsitoAggiornamentoWebDom");
                }
                db.Connection.Close();
            }
        }


        public static void GetEsitoAggiornamentoWebDom(string tipoApp, out List<EsitoAggiornamentoWebDom> lstEsitoAggiornamentoWebDom)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lstEsitoAggiornamentoWebDom = (from elem in db.EsitoAggiornamentoWebDoms
                                                   where elem.TipoApp == tipoApp
                                                   select elem).ToList<EsitoAggiornamentoWebDom>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
