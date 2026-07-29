using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneAnagraficaAccordi
    {
        /// <summary>
        /// metodo che fa la get dalla tabella DecAnagraficaAccordi
        /// </summary>
        /// <param name="elencoDecAnagraficaAccordi"></param>
        public static void GetDecAnagraficaAccordi(out List<DecAnagraficaAccordi> elencoDecAnagraficaAccordi)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecAnagraficaAccordi = (from d in db.DecAnagraficaAccordis select d).ToList<DecAnagraficaAccordi>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        /// <summary>
        /// metodo che richiama la stored procedure di insert e update di AnagraficaAccordi
        /// </summary>
        /// <param name="anagraficaAccordi"></param>
        public static void SalvaAnagraficaAccordi(DecAnagraficaAccordi anagraficaAccordi)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertAnagraficaAccordi(anagraficaAccordi.Id, anagraficaAccordi.Abilitata, anagraficaAccordi.Codice, anagraficaAccordi.DenominazioneAzienda, anagraficaAccordi.DataAccordi, anagraficaAccordi.Decreto, anagraficaAccordi.DomandeLiquidabili, anagraficaAccordi.DomandeLiquidate);
                if (result == -1)
                {
                    throw new INPS.DNA.DnaValidationException("Record in uso, impossibile modificare");
                }

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertAnagraficaAccordi");
                }
                db.Connection.Close();
            }
        }

        /// <summary>
        /// metodo che richiama la stored procedure DeleteAnagraficaAccordi
        /// </summary>
        /// <param name="anagraficaAccordi"></param>
        public static int DeleteAnagraficaAccordi(DecAnagraficaAccordi anagraficaAccordi)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAnagraficaAccordi(anagraficaAccordi.Id, anagraficaAccordi.Codice);
                if (result == -1)
                {
                    throw new INPS.DNA.DnaValidationException("Record in uso, impossibile eliminare");
                }
                else if (result == -2)
                {
                    //Errore gestito - Faccio UPDATE e mostro a video: "Record in uso, impossibile eliminare"
                }
                else if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAnagraficaAccordi");
                }
                db.Connection.Close();

                return result;
            }
        }

        /// <summary>
        /// metodo che richiama la stored procedure UpdateCountLiquidate_AnagraficaAccordi
        /// </summary>
        /// <param name="codiceAziendaEditoria"></param>
        public static void UpdateCountLiquidate_AnagraficaAccordi(short? codiceAziendaEditoria)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.UpdateCountLiquidate_AnagraficaAccordi(codiceAziendaEditoria);

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure UpdateCountLiquidate_AnagraficaAccordi");
                }
                db.Connection.Close();
            }
        }
    }
}
