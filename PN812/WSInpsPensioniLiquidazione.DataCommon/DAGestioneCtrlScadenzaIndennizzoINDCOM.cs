using INPS.DNA.Data;
using INPS.DNA.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlScadenzaIndennizzoINDCOM
    {
        public static void GetCtrlScadenzaIndennizzoINDCOM(out List<CtrlScadenzaIndennizzoINDCOM> ctrlScadenzaIndennizzoINDCOM)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    ctrlScadenzaIndennizzoINDCOM = (from c in db.CtrlScadenzaIndennizzoINDCOMs select c).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
