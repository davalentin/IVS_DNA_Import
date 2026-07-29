using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneLogGenerico
    {
        public static void SalvaLogGenerico(LogGenerico log)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                db.InsertLogGenerico(log.NumDomanda, log.LogType, log.MethodName, log.Message, log.Parameters, log.StackTrace, log.Progressivo);
                db.Connection.Close();
            }
        }

        public static void DeleteLogGenerico(long numDomanda)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                db.DeleteLogGenerico(numDomanda);
                db.Connection.Close();
            }
        }
    }
}
