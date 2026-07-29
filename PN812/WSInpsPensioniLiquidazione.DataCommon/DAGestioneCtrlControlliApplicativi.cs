using INPS.DNA.Data;
using INPS.DNA.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlControlliApplicativi
    {
        public static void GetControlloApplicativo(string nomeControllo, string tipoApp, out CtrlControlliApplicativi controlloApplicativo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    controlloApplicativo = (from e in db.CtrlControlliApplicativis
                                            where e.Nome == nomeControllo && e.TipoApp == tipoApp
                                            select e).FirstOrDefault();

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
