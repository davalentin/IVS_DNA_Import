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
    public class DAGestioneDatiControlloFelpe
    {
        public static void GetDatiControlloFelpeByIdPensione(Int64 idPensione, out DatiControlloFelpe datiControlloFelpe)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    datiControlloFelpe = (from d in db.DatiControlloFelpes
                                          where d.IdPensione == idPensione
                                          select d).SingleOrDefault<DatiControlloFelpe>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaDatiControlloFelpe(DatiControlloFelpe datiControlloFelpe)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertDatiControlloFelpe(datiControlloFelpe.IdPensione, datiControlloFelpe.IsProvvisoria, datiControlloFelpe.InizioBonus, datiControlloFelpe.FineBonus);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertDatiControlloFelpe");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaDatiControlloFelpeByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteDatiControlloFelpe(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteDatiControlloFelpe");
                }
                db.Connection.Close();
            }
        }
    }
}
