using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneContribuzioneEnpals
    {
        public static void GetDatiContribuzioneEnpals(long idPensione, string tipologia, out List<ContribuzioneEnpals> lstContribuzioneEnpals)
        {
            lstContribuzioneEnpals = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lstContribuzioneEnpals = (from c in db.ContribuzioneEnpals where c.IdPensione == idPensione && c.Tipologia == tipologia select c).ToList<ContribuzioneEnpals>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaDatiContribuzioneEnpals(ContribuzioneEnpals objDb)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertContribuzioneEnpals(objDb.IdPensione,objDb.Tipologia,objDb.Quota,objDb.Enpals,objDb.Figurativa,objDb.Ufficio,objDb.Inps,objDb.Volontaria,objDb.Estera);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertContribuzioneEnpals");
                }
                db.Connection.Close();
            }
        }

        public static void CancellaDatiContribuzioneEnpalsByIdPensione(long idPensione,string tipologia)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int a = db.DeleteContribuzioneEnpals(idPensione,tipologia);
                db.Connection.Close();
            }
        }

        public static void CancellaAllDatiContributzioneEnpalsByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int a = db.DeleteAllContribuzioneEnpals(idPensione);
                db.Connection.Close();
            }
        }
    }
}
