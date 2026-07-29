using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestionePensioniLavorazioneManualeAutomatiche
    {
        #region PensioniLavorazioneManualeAutomatiche
        public static void GetAllPensioniLavorazioneManualeAutomatiche(string tipoApp, out List<PensioniLavorazioneManualeAutomatiche> lstPensioniLavorazioneManualeAutomatiche)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lstPensioniLavorazioneManualeAutomatiche = (from b in db.PensioniLavorazioneManualeAutomatiches
                                                                where b.TipoApp == tipoApp
                                                                orderby b.AutorizzazioneManuale == null descending, b.Id descending
                                                                select b).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetAllPensioniLavorazioneManualeAutomaticheByCodiceSede(string utente, string tipoApp, List<Int16> codiceSede, out List<PensioniLavorazioneManualeAutomatiche> lstPensioniLavorazioneManualeAutomatiche)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lstPensioniLavorazioneManualeAutomatiche = (from b in db.PensioniLavorazioneManualeAutomatiches
                                                                where codiceSede.Contains(b.CodiceSede) && b.TipoApp == tipoApp  && (utente == null || b.MatricolaUtente == utente)
                                                                orderby b.Id descending
                                                                  select b).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetAllPensioniLavorazioneManualeAutomaticheByNDomus(string gruppo, string prodotto, string tipo, long nDomus, out List<PensioniLavorazioneManualeAutomatiche> lstPensioniLavorazioneManualeAutomatiche)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lstPensioniLavorazioneManualeAutomatiche = (from b in db.PensioniLavorazioneManualeAutomatiches
                                                                where b.NDomus == nDomus && b.Tipo == tipo && b.Gruppo == gruppo && b.Prodotto == prodotto
                                                                orderby b.Id descending
                                                                  select b).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void InsertPensioniLavorazioneManualeAutomatiche(PensioniLavorazioneManualeAutomatiche pensioniLavorazioneManualeAutomatiche)
        {
            PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
            int result = db.InsertPensioniLavorazioneManualeAutomatiche(pensioniLavorazioneManualeAutomatiche.NDomus, pensioniLavorazioneManualeAutomatiche.SiglaCategoria, 
                pensioniLavorazioneManualeAutomatiche.CodiceSede, pensioniLavorazioneManualeAutomatiche.Gruppo, pensioniLavorazioneManualeAutomatiche.Prodotto,
                pensioniLavorazioneManualeAutomatiche.Tipo, pensioniLavorazioneManualeAutomatiche.DecorrenzaOriginaria, pensioniLavorazioneManualeAutomatiche.AutorizzazioneManuale,
                pensioniLavorazioneManualeAutomatiche.MatricolaUtente, pensioniLavorazioneManualeAutomatiche.Id, pensioniLavorazioneManualeAutomatiche.TipoApp);
            if (result != 0)
            {
                throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensioniLavorazioneManualeAutomatiche");
            }
            db.Connection.Close();
        }
        #endregion PensioniLavorazioneManualeAutomatiche
    }
}
