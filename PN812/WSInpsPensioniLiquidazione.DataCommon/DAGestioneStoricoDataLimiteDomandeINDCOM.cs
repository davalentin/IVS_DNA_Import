using INPS.DNA.Data;
using INPS.DNA.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneStoricoDataLimiteDomandeINDCOM
    {
        public static void InsertStoricoDataLimiteINDCOM(StoricoDataLimiteDomandeINDCOM storicoDataLimiteDomandeINDCOM)
        {
            PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
            int result = db.InsertStoricoDataLimiteDomandeINDCOM(DateTime.UtcNow, storicoDataLimiteDomandeINDCOM.DataLimiteDomandeINDCOM, null, storicoDataLimiteDomandeINDCOM.Matricola);
            if (result != 0)
            {
                throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertStoricoDataLimiteDomandeINDCOM");
            }
            db.Connection.Close();
        }

        public static void GetAllStoricoDataLimiteDOmandeINDCOM(out List<StoricoDataLimiteDomandeINDCOM> lstStoricoDataLimiteDomandeINDCOM)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lstStoricoDataLimiteDomandeINDCOM = db.StoricoDataLimiteDomandeINDCOMs.OrderByDescending(x => x.Id).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void UpdateAllDataLimiteDOmandeINDCOMNote(int id, string note)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    int result = db.UpdateNoteStoricoDataLimiteDomandeINDCOM(id, note);
                    if (result != 0)
                    {
                        throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure UpdateAllDataLimiteDOmandeINDCOMNote");
                    }
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
