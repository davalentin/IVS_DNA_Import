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
    public class DAGestioneLiquidazioniAbilitate
    {
        public static void GetLiquidazioneAbilitata(LiquidazioniAbilitate liquidazioneAbilitata, out LiquidazioniAbilitate liquidazioneAbilitataResult)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    liquidazioneAbilitataResult = (from l in db.LiquidazioniAbilitates 
                                          where l.SiglaCategoria == liquidazioneAbilitata.SiglaCategoria &&
                                          l.Sede == liquidazioneAbilitata.Sede &&
                                          l.Tipologia == liquidazioneAbilitata.Tipologia select l).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetAllLiquidazioniAbilitate(out List<LiquidazioniAbilitate> elencoLiquidazioniAbilitate)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoLiquidazioniAbilitate = (from l in db.LiquidazioniAbilitates select l).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaLiquidazioneAbilitata(LiquidazioniAbilitate liquidazioneAbilitata)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteLiquidazioneAbilitata(liquidazioneAbilitata.SiglaCategoria, liquidazioneAbilitata.Sede, liquidazioneAbilitata.Tipologia, liquidazioneAbilitata.Ricostituzione, 
                    liquidazioneAbilitata.AbilitazioneManuale, liquidazioneAbilitata.RicostituzioneDaAutomatica, liquidazioneAbilitata.AbilitazioneAutomatica);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteLiquidazioneAbilitata");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaLiquidazioneAbilitata(LiquidazioniAbilitate liquidazioneAbilitata)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertLiquidazioneAbilitata(liquidazioneAbilitata.SiglaCategoria, liquidazioneAbilitata.Sede, liquidazioneAbilitata.Tipologia, liquidazioneAbilitata.Ricostituzione,
                                liquidazioneAbilitata.AbilitazioneManuale, liquidazioneAbilitata.RicostituzioneDaAutomatica, liquidazioneAbilitata.AbilitazioneAutomatica);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertLiquidazioneAbilitata");
                }
                db.Connection.Close();
            }
        }
    }
}
