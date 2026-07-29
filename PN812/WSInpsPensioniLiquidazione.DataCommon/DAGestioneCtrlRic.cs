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
    public class DAGestioneCtrlRic
    {

        public static void GetCtrlTabRic(string prodotto, string tipologia, out CtrlTabRic ctrlRic)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    ctrlRic = (from cr in db.CtrlTabRics  where cr.Prodotto == prodotto && cr.Tipologia == tipologia select cr).SingleOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
