using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneLogSoap
    {
        public static void SalvaLogSoap(LogSoap log)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                db.Insert_LogSoap(log.NumDomanda, log.ServiceName, log.MethodName, log.Direction, log.Xml, log.Guid, log.Progressivo);
                db.Connection.Close();
            }
        }

        public static void DeleteLogSoap(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                db.DeleteLogSoap(idPensione);
                db.Connection.Close();
            }
        }

        //ENG - Aggiornamento Memo86
        public static void GetTimestampMinimo(Int64 ndomus, out DateTime? dataTimestampMinimo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    dataTimestampMinimo = (from i in db.LogSoaps
                                     where i.NumDomanda == ndomus
                                     orderby i.TimeStamp ascending
                                     select i.TimeStamp).FirstOrDefault<DateTime>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
