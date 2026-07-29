using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlStatiSenzaEsenzioneFiscaleEstera
    {
        public static void GetCtrlStatiSenzaEsenzioneFiscaleEstera(string codCatastale, out CtrlStatiSenzaEsenzioneFiscaleEstera ctrl)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    ctrl = (from cr in db.CtrlStatiSenzaEsenzioneFiscaleEsteras where cr.CodCatastale == codCatastale select cr).SingleOrDefault<CtrlStatiSenzaEsenzioneFiscaleEstera>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
