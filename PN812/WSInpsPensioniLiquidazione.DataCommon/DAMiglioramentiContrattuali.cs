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
    public class DAMiglioramentiContrattuali
    {
        public static void GetDatiQuoteMiglioramentiContrattualiByIdPensione(long idPensione, out List<QuoteMiglioramentiContrattuali> LstQuoteMiglioramentiContrattuali)
        {
            LstQuoteMiglioramentiContrattuali = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    var quoteMiglioramentiContrattuali = (from a in db.QuoteMiglioramentiContrattualis where a.IdPensione == idPensione select a);
                    LstQuoteMiglioramentiContrattuali = new List<QuoteMiglioramentiContrattuali>();
                    foreach (QuoteMiglioramentiContrattuali sup in quoteMiglioramentiContrattuali)
                    {
                        LstQuoteMiglioramentiContrattuali.Add(sup);
                    }
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDatiQuoteMiglioramentiContrattualiNoStoricoByIdPensione(long idPensione, out List<QuoteMiglioramentiContrattuali> LstQuoteMiglioramentiContrattuali)
        {
            LstQuoteMiglioramentiContrattuali = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    var quoteMiglioramentiContrattuali = (from a in db.QuoteMiglioramentiContrattualis where a.IdPensione == idPensione && !a.IsStorico.GetValueOrDefault() select a);
                    LstQuoteMiglioramentiContrattuali = new List<QuoteMiglioramentiContrattuali>();
                    foreach (QuoteMiglioramentiContrattuali sup in quoteMiglioramentiContrattuali)
                    {
                        LstQuoteMiglioramentiContrattuali.Add(sup);
                    }
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDatiQuoteMiglioramentiContrattualiStoricoByIdPensione(long idPensione, out List<QuoteMiglioramentiContrattuali> LstQuoteMiglioramentiContrattuali)
        {
            LstQuoteMiglioramentiContrattuali = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    var quoteMiglioramentiCumulo = (from a in db.QuoteMiglioramentiContrattualis where a.IdPensione == idPensione && a.IsStorico.GetValueOrDefault() select a);
                    LstQuoteMiglioramentiContrattuali = new List<QuoteMiglioramentiContrattuali>();
                    foreach (QuoteMiglioramentiContrattuali sup in quoteMiglioramentiCumulo)
                    {
                        LstQuoteMiglioramentiContrattuali.Add(sup);
                    }
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDatiMiglioramentiContrattualiByIdPensione(long idPensione, out MiglioramentiContrattuali MiglioramentiContrattuali)
        {
            MiglioramentiContrattuali = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    MiglioramentiContrattuali = (from a in db.MiglioramentiContrattualis where a.IdPensione == idPensione select a).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaMiglioramentiContrattuali(MiglioramentiContrattuali miglioramentiContrattuali)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertMiglioramentiContrattuali(
                    miglioramentiContrattuali.IdPensione,
                    miglioramentiContrattuali.CodiceEnte,
                    miglioramentiContrattuali.CodiceCessazione,
                    miglioramentiContrattuali.MotivoCessazione
                );

                if (result != 0)
                {
                    //TODO gestione errore
                }
                db.Connection.Close();
            }
        }

        public static void SalvaQuotaMiglioramentiContrattuali(QuoteMiglioramentiContrattuali quoteMiglioramentiContrattuali)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuoteMiglioramentiContrattuali(
                    quoteMiglioramentiContrattuali.IdPensione,
                    quoteMiglioramentiContrattuali.Codice,
                    quoteMiglioramentiContrattuali.DataDecorrenza,
                    quoteMiglioramentiContrattuali.Quota,
                    quoteMiglioramentiContrattuali.IsStorico
                );

                if (result != 0)
                {
                    //TODO gestione errore
                }
                db.Connection.Close();
            }
        }
    }
}
