using INPS.DNA.Data;
using INPS.DNA.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneAnagraficaAziendePerTipo0179
    {
        /// <summary>
        /// metodo che fa la get dalla tabella DecAnagraficaAziende
        /// </summary>
        /// <param name="elencoDecAnagraficaAziende"></param>
        public static void GetDecAnagraficaAziende(out List<DecAnagraficaAziendePerTipo0179> elencoDecAnagraficaAziende)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecAnagraficaAziende = (from d in db.DecAnagraficaAziendePerTipo0179s select d).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        /// <summary>
        /// metodo che richiama la stored procedure di insert e update di AnagraficaAziende
        /// </summary>
        /// <param name="anagraficaAziende"></param>
        public static void SalvaAnagraficaAziende(DecAnagraficaAziendePerTipo0179 anagraficaAziende)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertAnagraficaAziendePerTipo0179(anagraficaAziende.Id, anagraficaAziende.DenominazioneAzienda, anagraficaAziende.SottogruppoPrimoOnere, 
                    anagraficaAziende.SottogruppoSecondoOnere);
                if (result == -1)
                {
                    throw new DNA.DnaValidationException("Record in uso, impossibile modificare");
                }

                if (result != 0)
                {
                    throw new DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertAnagraficaAziendePerTipo0179");
                }
                db.Connection.Close();
            }
        }

        /// <summary>
        /// metodo che richiama la stored procedure DeleteAnagraficaAziende
        /// </summary>
        /// <param name="anagraficaAziende"></param>
        public static void DeleteAnagraficaAziende(DecAnagraficaAziendePerTipo0179 anagraficaAziende)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAnagraficaAziendePerTipo0179(anagraficaAziende.Id);
                if (result == -1)
                {
                    throw new DNA.DnaValidationException("Record in uso, impossibile eliminare");
                }

                else if (result != 0)
                {
                    throw new DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAnagraficaAziendePerTipo0179");
                }
                db.Connection.Close();
            }
        }
    }
}
