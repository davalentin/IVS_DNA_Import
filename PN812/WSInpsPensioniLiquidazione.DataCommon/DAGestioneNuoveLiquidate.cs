using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneNuoveLiquidate
    {
        public static void GetNuoveLiquidateByIdPensione(long IdPensione, out NuoveLiquidate nuoveLiquidate)
        {
            nuoveLiquidate = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    nuoveLiquidate = (from nl in db.NuoveLiquidates
                                      where nl.IdPensione == IdPensione
                                      select nl).SingleOrDefault<NuoveLiquidate>();
                }
            }
        }

        public static void SalvaNuoveLiquidate(NuoveLiquidate nuoveLiquidate)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));

                int result = db.InsertNuoveLiquidate(nuoveLiquidate.IdPensione, nuoveLiquidate.FlagProvvisoria, nuoveLiquidate.FlagContributiva,
                                        nuoveLiquidate.Affini, nuoveLiquidate.Coniuge, nuoveLiquidate.Figli, nuoveLiquidate.CodiceCategoriaReversibilita,
                                        nuoveLiquidate.SedeReversibilita, nuoveLiquidate.CertificatoReversibilita, nuoveLiquidate.CodiceProcesso,
                                        nuoveLiquidate.DataPresaInCarico, nuoveLiquidate.CodiceProcessoDestinazione, nuoveLiquidate.CodiceProcessoGP1ALZ6, nuoveLiquidate.IsFlagProvvisoriaFromCumulo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertNuoveLiquidate");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteNuoveLiquidateByIdPensione(long IdPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteNuoveLiquidate(IdPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteNuoveLiquidate");
                }
                db.Connection.Close();
            }
        }
    }
}
