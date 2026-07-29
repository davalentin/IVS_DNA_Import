using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlDecorrenzaRetrExINPDAI
    {
        public static void GetDecorrenzaRetrExINPDAI(out List<CtrlDecorrenzaRetrExINPDAI> ctrlDecorrenzaRetrExINPDAI)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    ctrlDecorrenzaRetrExINPDAI = (from d in db.CtrlDecorrenzaRetrExINPDAIs select d).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
