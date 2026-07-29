using INPS.DNA.Data;
using INPS.DNA.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlAttEconProfInd_PensProv
    {
        public static void GetCtrlAttEconProfInd_PensProv(out List<CtrlAttEconProfInd_PensProv> elencoCtrlAttEconProfInd_PensProv)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCtrlAttEconProfInd_PensProv = (from d in db.CtrlAttEconProfInd_PensProvs select d).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
