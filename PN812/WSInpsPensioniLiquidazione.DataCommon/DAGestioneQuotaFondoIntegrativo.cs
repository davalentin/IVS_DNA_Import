using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneQuotaFondoIntegrativo
    {
        public static void GetQuotaFondoIntegrativoByIdPensione(Int64 idPensione, out List<QuotaFondoIntegrativo> quotaFondoIntegrativo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quotaFondoIntegrativo = (from v in db.QuotaFondoIntegrativos
                                                where v.IdPensione == idPensione && !v.IsStorico
                                             select v).ToList<QuotaFondoIntegrativo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetQuotaFondoIntegrativoStoricoByIdPensione(Int64 idPensione, out List<QuotaFondoIntegrativo> quotaFondoIntegrativo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quotaFondoIntegrativo = (from v in db.QuotaFondoIntegrativos
                                             where v.IdPensione == idPensione && v.IsStorico
                                             select v).ToList<QuotaFondoIntegrativo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaQuotaFondoIntegrativo(QuotaFondoIntegrativo quotaFondoIntegrativo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuotaFondoIntegrativo(quotaFondoIntegrativo.IdPensione, quotaFondoIntegrativo.CodiceGestione, quotaFondoIntegrativo.Montante, 
                    quotaFondoIntegrativo.ImportoContributivoTotale, quotaFondoIntegrativo.NSettimane, quotaFondoIntegrativo.MontanteQuotaD, quotaFondoIntegrativo.ImportoContribTotaleQuotaD,
                    quotaFondoIntegrativo.NSettimaneQuotaD, quotaFondoIntegrativo.PL_Quotac, quotaFondoIntegrativo.IsStorico);
                
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuotaFondoIntegrativo");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaQuotaFondoIntegrativoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuotaFondoIntegrativo(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuotaFondoIntegrativo");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaQuotaFondoIntegrativoNoStoricoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuotaFondoIntegrativoNoStorico(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuotaFondoIntegrativo");
                }
                db.Connection.Close();
            }
        }
    }
}
