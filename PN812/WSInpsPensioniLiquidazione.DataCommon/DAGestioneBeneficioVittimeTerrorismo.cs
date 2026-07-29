using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneBeneficioVittimeTerrorismo
    {
        public static void GetBeneficioVittimeTerrorismoByIdPensione(Int64 idPensione, out BeneficioVittimeTerrorismo beneficioVittimeTerrorismo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    beneficioVittimeTerrorismo = (from v in db.BeneficioVittimeTerrorismos
                                         where v.IdPensione == idPensione
                                         select v).SingleOrDefault<BeneficioVittimeTerrorismo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaBeneficioVittimeTerrorismo(BeneficioVittimeTerrorismo beneficioVittimeTerrorismo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertBeneficioVittimeTerrorismo(beneficioVittimeTerrorismo.IdPensione, beneficioVittimeTerrorismo.SoggettoBeneficiario, beneficioVittimeTerrorismo.CodiceEvento,
                    beneficioVittimeTerrorismo.DataEventoTerroristico, beneficioVittimeTerrorismo.TipologiaPrestazione, beneficioVittimeTerrorismo.TipologiaBeneficio
                    );
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertBeneficioVittimeTerrorismo");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaBeneficioVittimeTerrorismoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteBeneficioVittimeTerrorismo(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteBeneficioVittimeTerrorismo");
                }
                db.Connection.Close();
            }
        }
    }
}