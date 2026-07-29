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
    public class DAGestioneDetrazioni
    {
        public static void GetDetrazioniImpostaByIdPensione(Int64 idPensione, out DetrazioniImposta detrazioniiImposta)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    detrazioniiImposta = (from d in db.DetrazioniImpostas
                                          where d.IdPensione == idPensione && !d.IsStorico
                                          select d).SingleOrDefault<DetrazioniImposta>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        /// <summary>
        /// Recupera i record di Storico dalla tabella DetrazioniImposta
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="calcoloContributivo"></param>
        public static void GetDetrazioniImpostaStoricoByIdPensione(Int64 idPensione, out DetrazioniImposta detrazioniImposta)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    detrazioniImposta = (from cc in db.DetrazioniImpostas where cc.IdPensione == idPensione && cc.IsStorico select cc).SingleOrDefault<DetrazioniImposta>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        /// <summary>
        /// Elimina tutti i record, tranne quelli di Storico, della tabella DetrazioniImposta afferenti alla pensione
        /// </summary>
        /// <param name="idPensione"></param>
        public static void EliminaDetrazioniImpostaNoStoricoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteDetrazioniImpostaNoStorico(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteDetrazioniImpostaNoStorico");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaDetrazioniImpostaByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteDetrazioniImposta(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteDetrazioniImposta");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaDetrazioniImposta(DetrazioniImposta detrazioniImposta)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertDetrazioniImposta(detrazioniImposta.IdPensione, detrazioniImposta.DetrazioniReddito, detrazioniImposta.AgevolazionePensionati, detrazioniImposta.ConiugeOFiglio,
                    detrazioniImposta.FigliMinori3AnniNoHandicap100, detrazioniImposta.FigliMinori3AnniNoHandicap50, detrazioniImposta.FigliMinori3AnniHandicap100, detrazioniImposta.FigliMinori3AnniHandicap50,
                    detrazioniImposta.FigliMaggiori3AnniNoHandicap100, detrazioniImposta.FigliMaggiori3AnniNoHandicap50, detrazioniImposta.FigliMaggiori3AnniHandicap100, detrazioniImposta.FigliMaggiori3AnniHandicap50,
                    detrazioniImposta.AltriFamiliari100, detrazioniImposta.AltriFamiliari50, detrazioniImposta.AddizionaleLombardiaVeneto, detrazioniImposta.NonResidenteSchumacker, detrazioniImposta.ConvDoppieImposizioni, 
                    detrazioniImposta.DecorrenzaDetrazioneImposte, detrazioniImposta.IsStorico);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertDetrazioniImposta");
                }
                db.Connection.Close();
            }
        }
    }
}

