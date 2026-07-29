using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;
using System.Data.SqlClient;
using System.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneFondoCredito
    {
        public static bool VerificaAdesioneFondoCredito(string codiceFiscaleTitolare)
        {
            bool esisteFondoCredito = false;
            int? result = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    db.CheckAdesioneFondoCredito(codiceFiscaleTitolare, ref result);              
                    if (result.HasValue && result.Value > 0)
                        esisteFondoCredito = true;
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }

            return esisteFondoCredito;
        }
    }
}
