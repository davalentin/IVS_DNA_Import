using INPS.DNA.Data;
using INPS.DNA.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlScadenzaIndennizzoINDCOM0175
    {
        public static void GetCtrlScadenzaIndennizzoINDCOM0175(out List<CtrlScadenzaIndennizzoINDCOM0175> ctrlScadenzaIndennizzoINDCOM0175)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    ctrlScadenzaIndennizzoINDCOM0175 = (from c in db.CtrlScadenzaIndennizzoINDCOM0175s select c).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
