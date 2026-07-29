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
    public class DAGestioneOneri
    {
        public static void GetOneriByIdPensione(long idPensione, out List<Oneri> lOneri)
        {
            lOneri = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lOneri = (from p in db.Oneris where p.IdPensione == idPensione && !p.IsStorico select p).ToList<Oneri>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetOneriStoricoByIdPensione(long idPensione, out List<Oneri> lOneri)
        {
            lOneri = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lOneri = (from p in db.Oneris where p.IdPensione == idPensione && p.IsStorico select p).ToList<Oneri>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaOneriOnere(Oneri oneri)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertOneri(oneri.IdPensione, oneri.Decorrenza, oneri.Scadenza, oneri.ScadenzaBeneficio, oneri.IdCodeGruppo, oneri.IdCodeSottoGruppo, oneri.Settimane, oneri.Onere, oneri.IsStorico, oneri.GP2PBB80);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertOneri");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaOneriByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {                          
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteOneri(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure deleteOneri");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaOneriNoStoricoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteOneriNoStorico(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteOneriNoStorico");
                }
                db.Connection.Close();
            }
        }
    }
}
