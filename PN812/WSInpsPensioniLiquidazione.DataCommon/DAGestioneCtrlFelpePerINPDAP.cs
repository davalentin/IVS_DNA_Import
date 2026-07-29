using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlFelpePerINPDAP
    {
        public static bool IsFlussoFelpePerINPDAP(string siglaCategoria, string gruppo, string prodotto)
        {
            bool res = false;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    var resQuery = (from ctrl in db.CtrlFelpePerINPDAPs
                           where ctrl.SiglaCategoria == siglaCategoria && ctrl.Gruppo == gruppo && ctrl.Prodotto == prodotto &&
                        ((ctrl.AttivoDal == null && ctrl.AttivoAl == null) || (ctrl.AttivoDal.Value.Date <= DateTime.Now.Date && ctrl.AttivoAl.Value.Date >= DateTime.Now.Date))
                           select ctrl).FirstOrDefault() == null ? false : true;
                    db.Connection.Close();
                    transactionScope.Complete();
                    res = resQuery;
                }
            }
            return res;
        }
    }
}
