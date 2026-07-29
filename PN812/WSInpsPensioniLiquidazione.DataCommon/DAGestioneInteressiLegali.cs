using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneInteressiLegali
    {
        public static void GetInteressiLegaliByIdPensione(long idPensione, out List<InteressiLegali> lInteressiLegali)
        {
            lInteressiLegali = new List<InteressiLegali>();
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lInteressiLegali = (from p in db.InteressiLegalis where p.IdPensione == idPensione select p).ToList<InteressiLegali>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaInteresseLegale(InteressiLegali interesseLegale)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertInteresseLegale(interesseLegale.IdPensione, interesseLegale.TipoInteresseLegale, interesseLegale.DataInizio, interesseLegale.DataFine, interesseLegale.Importo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertInteresseLegale");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaAllInteressiLegaliByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllInteressiLegali(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllInteressiLegali");
                }
                db.Connection.Close();
            }
        }
    }
}
