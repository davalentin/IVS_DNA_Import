using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlAziendeVOCRED_DAP
    {
        public static void GetDecodificaAziendeVOCRED_DAP(out List<CtrlAziendeVOCRED_DAP> elencoDecodificaAziendeVOCRED_DAP)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaAziendeVOCRED_DAP = (from d in db.CtrlAziendeVOCRED_DAPs select d).ToList<CtrlAziendeVOCRED_DAP>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
