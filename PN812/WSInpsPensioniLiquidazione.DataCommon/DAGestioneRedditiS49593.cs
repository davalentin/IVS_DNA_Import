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
    public class DAGestioneRedditiS49593
    {
        public static void GetRedditiS49593ByIdPensione(long idPensione, out List<ReddS49593> reddS49593)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    reddS49593 = (from d in db.ReddS49593s
                                  where d.IdPensione == idPensione
                                  select d).OrderBy(x => x.AnnoReddito).ToList<ReddS49593>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            } 
        }

        public static void EliminaAllRedditiS49593ByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteReddS49593(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteReddS49593");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaRedditiS49593(ReddS49593 reddS49593)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertReddS49593(reddS49593.IdPensione, reddS49593.AnnoReddito, reddS49593.RedditoTitolare,
                    reddS49593.RedditoConiuge, reddS49593.RedditoDaPensioneConiuge, reddS49593.RedditoDaPensioneDC, reddS49593.CodiceDiReddito, reddS49593.FlagSentenza, reddS49593.ICISEN2, reddS49593.MeseReddito, reddS49593.AnnoSentenza);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertReddS49593");
                }
                db.Connection.Close();
            }
        }
    }
}
