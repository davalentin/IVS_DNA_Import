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
    public class DaGestioneVittimeTerrorismo
    {
        public static void GetVittimeTerrorismoByIdPensione(Int64 idPensione, out VittimeTerrorismo vittimeTerrorismo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    vittimeTerrorismo = (from v in db.VittimeTerrorismos
                                         where v.IdPensione == idPensione
                                         select v).SingleOrDefault<VittimeTerrorismo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaVittimeTerrorismo(VittimeTerrorismo vittimeTerrorismo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertVittimeTerrorismo(vittimeTerrorismo.IdPensione,
                    vittimeTerrorismo.TipoPrestazione, vittimeTerrorismo.CodiceBeneficio, vittimeTerrorismo.TipoBeneficio, vittimeTerrorismo.DataEvento,
                    vittimeTerrorismo.Beneficiario, vittimeTerrorismo.DecorrenzaBeneficiario, vittimeTerrorismo.CodiceGestione1, vittimeTerrorismo.CodiceGestione2,
                    vittimeTerrorismo.CodiceLiquidazione, vittimeTerrorismo.MontanteContributivo, vittimeTerrorismo.ImportoContributivo, vittimeTerrorismo.NSettRideterminato);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertVittimeTerrorismo");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaVittimeTerrorismoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteVittimeTerrorismo(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteVittimeTerrorismo");
                }
                db.Connection.Close();
            }
        }
    }
}
