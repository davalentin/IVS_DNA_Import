using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;


namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneAnagraficaAziendeLetteraB
    {
        /// <summary>
        /// metodo che fa la get dalla tabella DecAnagraficaAziendeLetteraB
        /// </summary>
        /// <param name="elencoDecAnagraficaAziende"></param>
        public static void GetDecAnagraficaAziende(out List<DecAnagraficaAziendeLettB> elencoDecAnagraficaAziendeLetteraB)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecAnagraficaAziendeLetteraB = (from d in db.DecAnagraficaAziendeLettBs select d).ToList<DecAnagraficaAziendeLettB>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        /// <summary>
        /// metodo che richiama la stored procedure di insert e update di AnagraficaAziendeLetteraB
        /// </summary>
        /// <param name="anagraficaAziende"></param>
        public static void SalvaAnagraficaAziende(DecAnagraficaAziendeLettB anagraficaAziendeLetteraB)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertAnagraficaAziendeLetteraB(anagraficaAziendeLetteraB.Id, anagraficaAziendeLetteraB.DenominazioneAzienda, anagraficaAziendeLetteraB.SottogruppoPrimoOnere, anagraficaAziendeLetteraB.SottogruppoSecondoOnere);
                if (result == -1)
                {
                    throw new INPS.DNA.DnaValidationException("Record in uso, impossibile modificare");
                }

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertAnagraficaAziendeLetteraB");
                }
                db.Connection.Close();
            }
        }

        /// <summary>
        /// metodo che richiama la stored procedure DeleteAnagraficaAziendeLetteraB
        /// </summary>
        /// <param name="anagraficaAziende"></param>
        public static void DeleteAnagraficaAziende(DecAnagraficaAziendeLettB anagraficaAziendeLetteraB)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAnagraficaAziendeLetteraB(anagraficaAziendeLetteraB.Id);
                if (result == -1)
                {
                    throw new INPS.DNA.DnaValidationException("Record in uso, impossibile eliminare");
                }

                else if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAnagraficaAziendeLetteraB");
                }
                db.Connection.Close();
            }
        }
    }
}
