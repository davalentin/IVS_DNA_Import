using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;


namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlCodiciFiscaliAbilitatiPerTipo0179
    {
        public static void GetCtrlCodiciFiscaliAbilitatiPerTipo0179byCodiceFiscale(string codiceFiscale, out CtrlCodiciFiscaliAbilitatiPerTipo0179 ctrlCodiciFiscaliAbilitatiPerTipo0179)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    ctrlCodiciFiscaliAbilitatiPerTipo0179 = (from d in db.CtrlCodiciFiscaliAbilitatiPerTipo0179s where d.CodiceFiscale == codiceFiscale select d).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
