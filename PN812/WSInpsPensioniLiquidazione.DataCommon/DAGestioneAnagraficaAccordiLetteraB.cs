using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneAnagraficaAccordiLetteraB
    {
          /// <summary>
        /// metodo che fa la get dalla tabella DecAnagraficaAccordiLetteraB
        /// </summary>
        /// <param name="elencoDecAnagraficaAccordi"></param>
        public static void GetDecAnagraficaAccordi(out List<DecAnagraficaAccordiLettB> elencoDecAnagraficaAccordiLetteraB)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecAnagraficaAccordiLetteraB = (from d in db.DecAnagraficaAccordiLettBs select d).ToList<DecAnagraficaAccordiLettB>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        /// <summary>
        /// metodo che richiama la stored procedure di insert e update di AnagraficaAccordiLetteraB
        /// </summary>
        /// <param name="anagraficaAccordi"></param>
        public static void SalvaAnagraficaAccordi(DecAnagraficaAccordiLettB anagraficaAccordiLetteraB)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertAnagraficaAccordiLetteraB(anagraficaAccordiLetteraB.Id, anagraficaAccordiLetteraB.Abilitata, anagraficaAccordiLetteraB.Codice, anagraficaAccordiLetteraB.DenominazioneAzienda, anagraficaAccordiLetteraB.DataAccordi, anagraficaAccordiLetteraB.Decreto, anagraficaAccordiLetteraB.DomandeLiquidabili, anagraficaAccordiLetteraB.DomandeLiquidate);
                if (result == -1)
                {
                    throw new INPS.DNA.DnaValidationException("Record in uso, impossibile modificare");
                }

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertAnagraficaAccordiLetteraB");
                }
                db.Connection.Close();
            }
        }

        /// <summary>
        /// metodo che richiama la stored procedure DeleteAnagraficaAccordiLetteraB
        /// </summary>
        /// <param name="anagraficaAccordi"></param>
        public static int DeleteAnagraficaAccordi(DecAnagraficaAccordiLettB anagraficaAccordiLetteraB)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAnagraficaAccordiLetteraB(anagraficaAccordiLetteraB.Id, anagraficaAccordiLetteraB.Codice);
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
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAnagraficaAccordiLetteraB");
                }
                db.Connection.Close();

                return result;
            }
        }

        /// <summary>
        /// metodo che richiama la stored procedure UpdateCountLiquidate_AnagraficaAccordiLetteraB
        /// </summary>
        /// <param name="codiceAziendaEditoria"></param>
        public static void UpdateCountLiquidate_AnagraficaAccordi(short? codiceAziendaEditoriaLetteraB)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.UpdateCountLiquidate_AnagraficaAccordiLetteraB(codiceAziendaEditoriaLetteraB);

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure UpdateCountLiquidate_AnagraficaAccordiLetteraB");
                }
                db.Connection.Close();
            }
        }
    }
}
