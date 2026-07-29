using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneBeneficiParticolari
    {
        public static void GetDatiBeneficiParticolariByIdPensione(long idPensione, out List<BeneficiParticolari> LstBeneficiParticolari)
        {
            LstBeneficiParticolari = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    LstBeneficiParticolari = (from a in db.BeneficiParticolaris where a.IdPensione == idPensione && !a.IsStorico select a).ToList<BeneficiParticolari>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDatiBeneficiParticolariStoricoByIdPensione(long idPensione, out List<BeneficiParticolari> LstBeneficiParticolari)
        {
            LstBeneficiParticolari = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    LstBeneficiParticolari = (from a in db.BeneficiParticolaris where a.IdPensione == idPensione && a.IsStorico select a).ToList<BeneficiParticolari>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaDatiBeneficiParticolari(BeneficiParticolari benToDB)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertBeneficiParticolari(benToDB.IdPensione, benToDB.CodiceBenefici, benToDB.Settimane, benToDB.IsStorico);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertBeneficiParticolari");
                }
                db.Connection.Close();
            }
        }

        public static void CancellaDatiBeneficiParticolariByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int a = db.DeleteAllBeneficiParticolari(idPensione);
                db.Connection.Close();
            }
        }

        public static void CancellaDatiBeneficiParticolariNoStoricoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int a = db.DeleteAllBeneficiParticolariNoStorico(idPensione);
                db.Connection.Close();
            }
        }

        public static void CancellaSingleDatiBeneficiParticolari(long idPensione, long idSupplemento)
        {
            PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
            db.DeleteBeneficiParticolari(idSupplemento, idPensione);
            db.Connection.Close();
        }
    }
}
