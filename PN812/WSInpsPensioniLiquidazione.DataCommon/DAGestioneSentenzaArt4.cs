using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using INPS.DNA.Data;
using System.Transactions;


namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneSentenzaArt4
    {
        public static void SalvaSentenzaArt4(SentenzaArt4 sentenzaArt4)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertSentenzaArt4(sentenzaArt4.IdPensione, sentenzaArt4.DecorrenzaSentenza, sentenzaArt4.ImportoSentenza, sentenzaArt4.IsFromGP);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertSentenzaArt4");
                }
                db.Connection.Close();
            }
        }

        public static void GetDatiSentenzaArt4(Int64 idPensione, out List<SentenzaArt4> lDatiSentenzaArt4)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lDatiSentenzaArt4 = (from sa in db.SentenzaArt4s
                                         where sa.IdPensione == idPensione
                                         select sa).ToList<SentenzaArt4>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaDatiSentenzaArt4ByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteSentenzaArt4(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteSentenzaArt4");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaDatiSentenzaArt4NoGPByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteSentenzaArt4NoGP(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteSentenzaArt4NoGP");
                }
                db.Connection.Close();
            }
        }
    }
}
