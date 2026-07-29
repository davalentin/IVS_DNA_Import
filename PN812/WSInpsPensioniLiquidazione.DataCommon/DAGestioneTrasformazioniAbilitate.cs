using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneTrasformazioniAbilitate
    {
        public static void GetTrasformazioneAbilitata(TrasformazioniAbilitate trasformazioneAbilitata, out TrasformazioniAbilitate trasformazioneAbilitataResult)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    trasformazioneAbilitataResult = (from l in db.TrasformazioniAbilitates
                                                   where l.SiglaCategoria == trasformazioneAbilitata.SiglaCategoria &&
                                                   l.Sede == trasformazioneAbilitata.Sede &&
                                                   l.Tipologia == trasformazioneAbilitata.Tipologia
                                                   select l).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetAllTrasformazioniAbilitate(out List<TrasformazioniAbilitate> elencoTrasformazioniAbilitate)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoTrasformazioniAbilitate = (from l in db.TrasformazioniAbilitates select l).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaTrasformazioneAbilitata(TrasformazioniAbilitate trasformazioneAbilitata)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteTrasformazioneAbilitata(trasformazioneAbilitata.SiglaCategoria, trasformazioneAbilitata.Sede, trasformazioneAbilitata.Tipologia);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteTrasformazioneAbilitata");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaTrasformazioneAbilitata(TrasformazioniAbilitate trasformazioneAbilitata)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertTrasformazioneAbilitata(trasformazioneAbilitata.SiglaCategoria, trasformazioneAbilitata.Sede, trasformazioneAbilitata.Tipologia);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertTrasformazioneAbilitata");
                }
                db.Connection.Close();
            }
        }
    }
}
