using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCalcoloVittimeTerrorismo
    {
        public static void GetCalcoloVittimeTerrorismoByIdPensione(Int64 idPensione, out List<CalcoloVittimeTerrorismo> calcoloVittimeTerrorismo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    calcoloVittimeTerrorismo = (from v in db.CalcoloVittimeTerrorismos
                                                  where v.IdPensione == idPensione
                                                  select v).ToList<CalcoloVittimeTerrorismo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaCalcoloVittimeTerrorismo(CalcoloVittimeTerrorismo calcoloVittimeTerrorismo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertCalcoloVittimeTerrorismo(calcoloVittimeTerrorismo.IdPensione, calcoloVittimeTerrorismo.Tipo, calcoloVittimeTerrorismo.DecorrenzaBeneficio, 
                    calcoloVittimeTerrorismo.CodiceGestioneRetr, calcoloVittimeTerrorismo.CodiceGestioneContr, calcoloVittimeTerrorismo.Quota, calcoloVittimeTerrorismo.CodiceTipoQuota, calcoloVittimeTerrorismo.Settimane,
                    calcoloVittimeTerrorismo.RMS, calcoloVittimeTerrorismo.Beneficio, calcoloVittimeTerrorismo.Ammontare, calcoloVittimeTerrorismo.Montante, calcoloVittimeTerrorismo.ImportoPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertCalcoloVittimeTerrorismo");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaCalcoloVittimeTerrorismoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteCalcoloVittimeTerrorismo(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteCalcoloVittimeTerrorismo");
                }
                db.Connection.Close();
            }
        }
    }
}
