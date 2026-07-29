using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneQuotaFondoINPGI
    {
        #region Contributivo

        public static void GetCalcoloContributivoINPGIByIdPensione(Int64 idPensione, out List<CalcoloContributivoINPGI> lcalcoloContributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lcalcoloContributivo = (from cc in db.CalcoloContributivoINPGIs where cc.IdPensione == idPensione && !cc.IsStorico select cc).ToList<CalcoloContributivoINPGI>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCalcoloContributivoINPGIStoricoByIdPensione(Int64 idPensione, out List<CalcoloContributivoINPGI> lcalcoloContributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lcalcoloContributivo = (from cc in db.CalcoloContributivoINPGIs where cc.IdPensione == idPensione && cc.IsStorico select cc).ToList<CalcoloContributivoINPGI>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaCalcoloContributivoINPGI(CalcoloContributivoINPGI calcoloContributivo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertCalcoloContributivoINPGI(calcoloContributivo.IdPensione, calcoloContributivo.CodiceGestione,
                                calcoloContributivo.Montante, calcoloContributivo.Quota, calcoloContributivo.Settimane, calcoloContributivo.IsStorico);

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertCalcoloContributivoINPGI");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaCalcoloContributivoINPGIByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteCalcoloContributivoINPGI(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteCalcoloContributivoINPGI");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaCalcoloContributivoINPGINoStoricoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteCalcoloContributivoINPGINoStorico(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteCalcoloContributivoINPGINoStorico");
                }
                db.Connection.Close();
            }
        }

        #endregion Contributivo

        #region Retributivo

        public static void GetCalcoloRetributivoINPGIByIdPensione(Int64 idPensione, out List<CalcoloRetributivoINPGI> lcalcoloRetributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lcalcoloRetributivo = (from cc in db.CalcoloRetributivoINPGIs where cc.IdPensione == idPensione && !cc.IsStorico select cc).ToList<CalcoloRetributivoINPGI>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCalcoloRetributivoINPGIStoricoByIdPensione(Int64 idPensione, out List<CalcoloRetributivoINPGI> lcalcoloRetributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lcalcoloRetributivo = (from cc in db.CalcoloRetributivoINPGIs where cc.IdPensione == idPensione && cc.IsStorico select cc).ToList<CalcoloRetributivoINPGI>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaCalcoloRetributivoINPGI(CalcoloRetributivoINPGI calcoloRetributivo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertCalcoloRetributivoINPGI(calcoloRetributivo.IdPensione, calcoloRetributivo.CodiceGestione,
                                calcoloRetributivo.Settimane, calcoloRetributivo.ImportoCalcolato, calcoloRetributivo.ImportoComma707, calcoloRetributivo.SettimaneComma707, calcoloRetributivo.RetribuzioneMediaSettimanale, calcoloRetributivo.IsStorico);

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertCalcoloRetributivoINPGI");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaCalcoloRetributivoINPGIByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteCalcoloRetributivoINPGI(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteCalcoloRetributivoINPGI");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaCalcoloRetributivoINPGINoStoricoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteCalcoloRetributivoINPGINoStorico(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteCalcoloRetributivoINPGINoStorico");
                }
                db.Connection.Close();
            }
        }

        #endregion Retributivo
    }
}
