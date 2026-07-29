using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlCodiceNatura2
    {
        public static void GetCodiciNatura2ByCodiceTipoRichiesta(string codiceTipoRichiesta , out CtrlCodiceNatura2 ctrlCodiceNatura2)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    ctrlCodiceNatura2 = (from d in db.CtrlCodiceNatura2s where d.CodiceTipoRichiesta == codiceTipoRichiesta select d).SingleOrDefault<CtrlCodiceNatura2>();
                   
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
