using INPS.DNA.Data;
using INPS.DNA.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlMatricoleAutomazione
    {
        public static void IsMatricolaForAutomazione(string matricola, out CtrlMatricoleAutomazione matricolaDb)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    matricolaDb = (from e in db.CtrlMatricoleAutomaziones
                                            where e.Matricola == matricola
                                            select e).FirstOrDefault();

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}

