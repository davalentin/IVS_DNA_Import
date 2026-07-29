using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;
using System.Linq.Expressions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlSediRicMotContribFsPtIndirette
    {
        public static void GetCtrlSediRicMotContribFsPtIndirette(short sede, string fondo, out List<CtrlSediRicMotContribFsPtIndirette> lstCtrl)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    //query build
                    IEnumerable<CtrlSediRicMotContribFsPtIndirette> lstResult = (from cr in db.CtrlSediRicMotContribFsPtIndirettes select cr)
                    .Where(x => x.Sede == sede)
                    .Where(x => (x.Fondo == null && fondo == null) || x.Fondo == fondo || x.Fondo == "ALL");
                    //query to db
                    lstCtrl = lstResult.ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
