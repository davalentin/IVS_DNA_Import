using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneAnagraficaAziende
    {
        /// <summary>
        /// metodo che fa la get dalla tabella DecAnagraficaAziende
        /// </summary>
        /// <param name="elencoDecAnagraficaAziende"></param>
        public static void GetDecAnagraficaAziende(out List<DecAnagraficaAziende> elencoDecAnagraficaAziende)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecAnagraficaAziende = (from d in db.DecAnagraficaAziendes select d).ToList<DecAnagraficaAziende>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        /// <summary>
        /// metodo che richiama la stored procedure di insert e update di AnagraficaAziende
        /// </summary>
        /// <param name="anagraficaAziende"></param>
        public static void SalvaAnagraficaAziende(DecAnagraficaAziende anagraficaAziende)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertAnagraficaAziende(anagraficaAziende.Id, anagraficaAziende.DenominazioneAzienda, anagraficaAziende.SottogruppoOnere);
                if (result == -1)
                {
                    throw new INPS.DNA.DnaValidationException("Record in uso, impossibile modificare");
                }

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertAnagraficaAziende");
                }
                db.Connection.Close();
            }
        }

        /// <summary>
        /// metodo che richiama la stored procedure DeleteAnagraficaAziende
        /// </summary>
        /// <param name="anagraficaAziende"></param>
        public static void DeleteAnagraficaAziende(DecAnagraficaAziende anagraficaAziende)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAnagraficaAziende(anagraficaAziende.Id);
                if (result == -1)
                {
                    throw new INPS.DNA.DnaValidationException("Record in uso, impossibile eliminare");
                }

                else if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAnagraficaAziende");
                }
                db.Connection.Close();
            }
        }
    }
}
