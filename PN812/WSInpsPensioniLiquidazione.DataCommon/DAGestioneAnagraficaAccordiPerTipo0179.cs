using INPS.DNA.Data;
using INPS.DNA.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneAnagraficaAccordiPerTipo0179
    {
        /// <summary>
        /// metodo che fa la get dalla tabella DecAnagraficaAccordi
        /// </summary>
        /// <param name="elencoDecAnagraficaAccordi"></param>
        public static void GetDecAnagraficaAccordi(out List<DecAnagraficaAccordiPerTipo0179> elencoDecAnagraficaAccordi)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecAnagraficaAccordi = (from d in db.DecAnagraficaAccordiPerTipo0179s select d).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        /// <summary>
        /// metodo che richiama la stored procedure di insert e update di AnagraficaAccordi
        /// </summary>
        /// <param name="anagraficaAccordi"></param>
        public static void SalvaAnagraficaAccordi(DecAnagraficaAccordiPerTipo0179 anagraficaAccordi)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertAnagraficaAccordiPerTipo0179(anagraficaAccordi.Id, anagraficaAccordi.Abilitata, anagraficaAccordi.Codice, anagraficaAccordi.DenominazioneAzienda, 
                    anagraficaAccordi.DataAccordi, anagraficaAccordi.DomandeLiquidabili, anagraficaAccordi.DomandeLiquidate);
                if (result == -1)
                {
                    throw new DNA.DnaValidationException("Record in uso, impossibile modificare");
                }

                if (result != 0)
                {
                    throw new DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertAnagraficaAccordiPerTipo0179");
                }
                db.Connection.Close();
            }
        }

        /// <summary>
        /// metodo che richiama la stored procedure DeleteAnagraficaAccordi
        /// </summary>
        /// <param name="anagraficaAccordi"></param>
        public static int DeleteAnagraficaAccordi(DecAnagraficaAccordiPerTipo0179 anagraficaAccordi)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAnagraficaAccordiPerTipo0179(anagraficaAccordi.Id, anagraficaAccordi.Codice);
                if (result == -1)
                {
                    throw new DNA.DnaValidationException("Record in uso, impossibile eliminare");
                }
                else if (result == -2)
                {
                    //Errore gestito - Faccio UPDATE e mostro a video: "Record in uso, impossibile eliminare"
                }
                else if (result != 0)
                {
                    throw new DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAnagraficaAccordiPerTipo0179");
                }
                db.Connection.Close();

                return result;
            }
        }

        /// <summary>
        /// metodo che richiama la stored procedure UpdateCountLiquidate_AnagraficaAccordi
        /// </summary>
        /// <param name="codiceAziendaEditoria"></param>
        public static void UpdateCountLiquidate_AnagraficaAccordi(short? codiceAziendaEditoria, bool isIncremento)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.UpdateCountLiquidate_AnagraficaAccordiPerTipo0179(codiceAziendaEditoria, isIncremento);

                if (result != 0)
                {
                    throw new DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure UpdateCountLiquidate_AnagraficaAccordiPerTipo0179");
                }
                db.Connection.Close();
            }
        }
    }
}
