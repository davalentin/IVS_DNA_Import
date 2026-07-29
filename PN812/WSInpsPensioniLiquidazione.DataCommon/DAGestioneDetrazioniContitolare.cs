using INPS.DNA.Data;
using INPS.DNA.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneDetrazioniContitolare
    {
        public static void GetDetrazioniImpostaContitolareBySoggetto(long idPensione, long idAnagrafica, out DetrazioniImpostaContitolare detrazioniImposta)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    detrazioniImposta = (from d in db.DetrazioniImpostaContitolares
                                          where d.IdPensione == idPensione && d.IdAnagrafica == idAnagrafica
                                          select d).FirstOrDefault<DetrazioniImpostaContitolare>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDetrazioniImpostaContitolareByIdPensione(long idPensione, out List<DetrazioniImpostaContitolare> detrazioniImposta)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    detrazioniImposta = (from d in db.DetrazioniImpostaContitolares
                                          where d.IdPensione == idPensione
                                          select d).ToList<DetrazioniImpostaContitolare>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaDetrazioniImpostaContitolareBySoggetto(long idPensione, long idAnagrafica)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteDetrazioniImpostaContitolare(idPensione, idAnagrafica);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteDetrazioniImpostaContitolare");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaDetrazioniImpostaContitolareNoStoricoBySoggetto(long idPensione, long idAnagrafica)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteDetrazioniImpostaContitolareNoStorico(idPensione, idAnagrafica);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteDetrazioniImpostaContitolareNoStorico");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaDetrazioniImpostaContitolareByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllDetrazioniImpostaContitolare(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllDetrazioniImpostaContitolare");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaDetrazioniImpostaContitolareNoStoricoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllDetrazioniImpostaContitolareNoStorico(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllDetrazioniImpostaContitolareNoStorico");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaDetrazioniImposta(DetrazioniImpostaContitolare detrazioniImposta)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertDetrazioniImpostaContitolare(detrazioniImposta.IdPensione, detrazioniImposta.IdAnagrafica, detrazioniImposta.DetrazioniReddito, detrazioniImposta.AgevolazionePensionati, 
                    detrazioniImposta.ConiugeOFiglio, detrazioniImposta.FigliMinori3AnniNoHandicap100, detrazioniImposta.FigliMinori3AnniNoHandicap50, detrazioniImposta.FigliMinori3AnniHandicap100, 
                    detrazioniImposta.FigliMinori3AnniHandicap50, detrazioniImposta.FigliMaggiori3AnniNoHandicap100, detrazioniImposta.FigliMaggiori3AnniNoHandicap50, 
                    detrazioniImposta.FigliMaggiori3AnniHandicap100, detrazioniImposta.FigliMaggiori3AnniHandicap50, detrazioniImposta.AltriFamiliari100, detrazioniImposta.AltriFamiliari50, 
                    detrazioniImposta.AddizionaleLombardiaVeneto, detrazioniImposta.NonResidenteSchumacker, detrazioniImposta.ConvDoppieImposizioni, detrazioniImposta.DecorrenzaDetrazioneImposte, 
                    detrazioniImposta.IsStorico);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertDetrazioniImpostaContitolare");
                }
                db.Connection.Close();
            }
        }
    }
}
