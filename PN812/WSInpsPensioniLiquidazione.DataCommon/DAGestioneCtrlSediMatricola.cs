using INPS.DNA.Data;
using INPS.DNA.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlSediMatricola
    { 
        public static void GetDecodificaSediMatricole( string sede, out List<CtrlSediMatricola> elencoDecodificaSediMatricole)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaSediMatricole = (from d in db.CtrlSediMatricolas where d.Sede == sede select d).ToList<CtrlSediMatricola>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
