using INPS.DNA.Data;
using INPS.DNA.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlStatiEsenzioneFiscaleEsteraINPDAP
    {
        public static void GetCtrlStatiEsenzioneFiscaleEsteraINPDAP(string codCatastale, out CtrlStatiEsenzioneFiscaleEsteraINPDAP ctrl)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    ctrl = (from cr in db.CtrlStatiEsenzioneFiscaleEsteraINPDAPs where cr.CodCatastale == codCatastale select cr).SingleOrDefault<CtrlStatiEsenzioneFiscaleEsteraINPDAP>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
