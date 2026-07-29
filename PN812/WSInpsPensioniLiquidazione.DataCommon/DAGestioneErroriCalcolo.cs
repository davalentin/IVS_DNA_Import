using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using INPS.DNA.Data;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public static class DAGestioneErroriCalcolo
    {
        public static void GetErroriCalcolo(int codice, string procedura, string gestione,out ErroriCalcolo erroriCalcolo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope scope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    erroriCalcolo = (from errCalc in db.ErroriCalcolos
                                     where (errCalc.Codice == codice && errCalc.Procedura == procedura && errCalc.Gestione == gestione)
                                     select errCalc).FirstOrDefault();
 
                }

            }
        }

    }
}
