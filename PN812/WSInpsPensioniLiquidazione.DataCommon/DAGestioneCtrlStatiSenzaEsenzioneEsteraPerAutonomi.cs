using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlStatiSenzaEsenzioneEsteraPerAutonomi
    {
        public static void GetListaStatiSenzaEsenzione(out List<CtrlStatiSenzaEsenzioneFiscaleEsteraPerAutonomi> ctrl)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    ctrl = (from cr in db.CtrlStatiSenzaEsenzioneFiscaleEsteraPerAutonomis select cr).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
