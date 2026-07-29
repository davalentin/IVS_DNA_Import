using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DACtrlNuovoCalcolo
    {
        public static void GetCtrlNuovoCalcolo(long ndomus, out CtrlNuovoCalcolo ctrlNuovoCalcolo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    ctrlNuovoCalcolo = (from d in db.CtrlNuovoCalcolos where d.NDomus == ndomus select d).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDomandePuntualiCtrlNuovoCalcolo(long ndomus, out CtrlDomandePuntualiNuovoCalcolo ctrlNuovoCalcolo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    ctrlNuovoCalcolo = (from d in db.CtrlDomandePuntualiNuovoCalcolos where d.NDomus == ndomus select d).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
