using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using INPS.DNA.Data;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneSentenze
    {
        public static void SalvaSentenze(Sentenze sentenza)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertSentenze(sentenza.IdPensione, sentenza.CodSentenzaMerito, sentenza.CodSentenza, sentenza.DecorrenzaDal, sentenza.DecorrenzaAl);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertSentenze");
                }
                db.Connection.Close();
            }
        }

        public static void GetDatiSentenze(Int64 idPensione, out List<Sentenze> lDatiSentenze)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lDatiSentenze = (from se in db.Sentenzes
                                     where se.IdPensione == idPensione
                                     select se).ToList<Sentenze>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaDatiSentenzeByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteSentenze(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteSentenze");
                }
                db.Connection.Close();
            }
        }
    }
}
