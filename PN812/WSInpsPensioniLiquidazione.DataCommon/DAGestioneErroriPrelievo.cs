using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneErroriPrelievo
    {
        public static void GetErroriPrelievo(string codice, string tipoAppartenenza, out ErroriPrelievo erroriPrelievo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope scope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    erroriPrelievo = (from err in db.ErroriPrelievos
                                     where (err.Codice == codice && err.TipoAppartenenza == tipoAppartenenza)
                                     select err).FirstOrDefault();

                }
            }
        }
    }
}
