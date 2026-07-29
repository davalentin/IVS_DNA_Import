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
    public class DAGestioneDL407
    {
        public static void GetDL407ByIdPensione(long idPensione, out DL407 dl407)
        {
            dl407 = new DL407();
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    dl407 = (from p in db.DL407s where p.IdPensione == idPensione select p).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaDL407(DL407 dl407)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertDL407(dl407.IdPensione, dl407.NSettimaneQuotaA, dl407.NSettimaneQuotaB, dl407.NSettimaneQuotaC,
                                            dl407.NSettimaneQuotaD, dl407.RMSQuotaA, dl407.RMSQuotaB, dl407.RMSQuotaD,dl407.ServizioUtileAAQuotaA,dl407.RetribPensQuotaA,
                                            dl407.RetribPensSL336QuotaA,dl407.ServizioUtileAAQuotaB,dl407.RetribPensQuotaB,dl407.RetribPensSL336QuotaB,dl407.ServizioUtileAAQuotaC);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertDL407");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaDL407ByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteDL407(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteDL407");
                }
                db.Connection.Close();
            }
        }
    }
}
