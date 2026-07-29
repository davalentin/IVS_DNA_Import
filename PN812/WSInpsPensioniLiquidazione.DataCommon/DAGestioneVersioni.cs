using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using INPS.DNA.Data;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneVersioni
    {
        public static void GetVersioni(out List<Versioni> lVersioni)
        {
            lVersioni = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lVersioni = (from p in db.Versionis select p).ToList<Versioni>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void AggiornaDatiVersione(string applicativo, long numVersione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertVersioni(applicativo, numVersione, DateTime.Now);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertVersioni");
                }
                db.Connection.Close();
            }
        }
    }
}
