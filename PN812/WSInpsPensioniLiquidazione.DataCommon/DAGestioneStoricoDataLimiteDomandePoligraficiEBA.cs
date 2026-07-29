using INPS.DNA.Data;
using INPS.DNA.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneStoricoDataLimiteDomandePoligraficiLetteraB
    {
        public static void InsertStoricoDataLimitePoligraficiLetteraB(StoricoDataLimitePoligraficiLettB storicoDataLimitePoligraficiLetteraB)
        {
            PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
            int result = db.InsertStoricoDataLimitePoligraficiLetteraB(DateTime.UtcNow, storicoDataLimitePoligraficiLetteraB.DataLimitePoligraficiLetteraB, null, storicoDataLimitePoligraficiLetteraB.Matricola);
            if (result != 0)
            {
                throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertStoricoDataLimitePoligraficiLetteraB");
            }
            db.Connection.Close();
        }

        public static void GetAllStoricoDataLimiteDomandePoligraficiLetteraB(out List<StoricoDataLimitePoligraficiLettB> lstStoricoDataLimitePoligraficiLetteraB)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lstStoricoDataLimitePoligraficiLetteraB = db.StoricoDataLimitePoligraficiLettBs.OrderByDescending(x => x.Id).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void UpdateAllDataLimiteDomandePoligraficiLetteraBNote(int id, string note)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    int result = db.UpdateNoteStoricoDataLimitePoligraficiLetteraB(id, note);
                    if (result != 0)
                    {
                        throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure UpdateAllDataLimitePoligraficiLetteraBNote");
                    }
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
