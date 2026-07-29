using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;
using INPS.DNA;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneTutore
    {
        public static void GetAnagraficaTutoreByIdPensione(long idPensione, out Anagrafica anagrafica, out char codiceTutore, out DateTime? cessValAmmSost)
        {
            anagrafica = null;
            codiceTutore = default(char);
            cessValAmmSost = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    var anagraficaTutore = (from t in db.Tutores where t.IdPensione == idPensione select new { t.Anagrafica, t.CodiceTutore, t.CessValAmmSost }).FirstOrDefault();
                    if (anagraficaTutore != null)
                    {
                        anagrafica = anagraficaTutore.Anagrafica;
                        codiceTutore = anagraficaTutore.CodiceTutore;
                        cessValAmmSost = anagraficaTutore.CessValAmmSost;
                    }
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaTutore(long idPensione, Anagrafica anagrafica, char? codiceTutore, DateTime? cessValAmmSost)
        {
            PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
            DAGestioneAnagrafica.SalvaAnagrafica(anagrafica);
            db.InsertTutore(anagrafica.Id, idPensione, codiceTutore, cessValAmmSost);
            db.Connection.Close();
        }

        public static void CancellaTutoreByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                db.DeleteTutore(idPensione);
                db.Connection.Close();
            }
        }
    }
}
