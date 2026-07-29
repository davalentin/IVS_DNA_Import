using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneSbloccoDomanda
    {
        public static void GetSbloccoDomandaByNumeroDomanda(Int64 numeroDomanda, out SbloccoDomanda sbloccoDomanda)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    sbloccoDomanda = (from sd in db.SbloccoDomandas
                                      where sd.NDomus == numeroDomanda
                                      select sd).SingleOrDefault<SbloccoDomanda>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaSbloccoDomanda(SbloccoDomanda sbloccoDomanda)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteSbloccoDomanda(sbloccoDomanda.NDomus);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteSbloccoDomanda");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaSbloccoDomanda(SbloccoDomanda sbloccoDomanda)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString", ConnectionTypeEnum.Application));
                int result = db.InsertSbloccoDomanda(sbloccoDomanda.NDomus, sbloccoDomanda.MatricolaBlocco, sbloccoDomanda.TimeStampBlocco);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertSbloccoDomanda");
                }
                db.Connection.Close();
            }
        }
    }
}
