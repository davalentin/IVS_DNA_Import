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
    public class DAGestioneEsitoCalcolo
    {
        public static void GetEsitoCalcoloByIdPensione(Int64 idPensione, out EsitoCalcolo esitoCalcolo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    esitoCalcolo = (from eC in db.EsitoCalcolos
                                    where eC.IdPensione == idPensione
                                    select eC).SingleOrDefault<EsitoCalcolo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaEsitoCalcolo(EsitoCalcolo esitoCalcolo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertEsitoCalcolo(esitoCalcolo.IdPensione, esitoCalcolo.Esito, esitoCalcolo.DettaglioEsito);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertEsitoCalcolo");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaEsitoCalcoloByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteEsitoCalcolo(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteEsitoCalcolo");
                }
                db.Connection.Close();
            }
        }
    }
}

