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
    public class DAGestioneDatiServizioUtile
    {
        /// <summary>
        /// Recupera i record NON di Storico dalla tabella DatiServizioUtile
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="lDatiServizioUtile"></param>
        public static void GetDatiServizioUtileByIdPensione(Int64 idPensione, out List<DatiServizioUtile> lDatiServizioUtile)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lDatiServizioUtile = (from su in db.DatiServizioUtiles
                                         join fdg in db.PensioneFondoDatiGenericis on su.IdFondo equals fdg.Id
                                          where fdg.IdPensione == idPensione && !su.IsStorico
                                         select su).ToList<DatiServizioUtile>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        /// <summary>
        /// Recupera i record di Storico dalla tabella DatiServizioUtile
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="lDatiServizioUtile"></param>
        public static void GetDatiServizioUtileStoricoByIdPensione(Int64 idPensione, out List<DatiServizioUtile> lDatiServizioUtile)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lDatiServizioUtile = (from su in db.DatiServizioUtiles
                                          join fdg in db.PensioneFondoDatiGenericis on su.IdFondo equals fdg.Id
                                          where fdg.IdPensione == idPensione && su.IsStorico
                                          select su).ToList<DatiServizioUtile>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDatiServizioUtileByIdRecordFondo(Int64 idRecordFondo, out List<DatiServizioUtile> lDatiServizioUtile)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lDatiServizioUtile = (from su in db.DatiServizioUtiles
                                          where su.IdRecordFondo == idRecordFondo && !su.IsStorico
                                          select su).ToList<DatiServizioUtile>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDatiServizioUtileStoricoByIdRecordFondo(Int64 idRecordFondo, out List<DatiServizioUtile> lDatiServizioUtile)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lDatiServizioUtile = (from su in db.DatiServizioUtiles
                                          where su.IdRecordFondo == idRecordFondo && su.IsStorico
                                          select su).ToList<DatiServizioUtile>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaDatiServizioUtileByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteDatiServizioUtile(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteDatiServizioUtile");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaDatiServizioUtileByIdRecordFondo(long idRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteDatiServizioUtileRecordFondo(idRecordFondo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteDatiServizioUtileRecordFondo");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaDatiServizioUtile(DatiServizioUtile datiServizioUtile)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertDatiServizioUtile(datiServizioUtile.IdFondo, datiServizioUtile.Quota, datiServizioUtile.ServizioUtileAA,
                    datiServizioUtile.ServizioUtileMM, datiServizioUtile.ServizioUtileGG, datiServizioUtile.RetribuzionePensionabile, datiServizioUtile.ControCodiceRetributivo,
                    datiServizioUtile.Retribuzione, datiServizioUtile.QuoteArt14, datiServizioUtile.ImportoIndennitaIntegrativaSpeciale, datiServizioUtile.ServizioUtileCessazioneAA, 
                    datiServizioUtile.ServizioUtileCessazioneMM, datiServizioUtile.ServizioUtileCessazioneGG, datiServizioUtile.QuotaPensioneRetributivaAnnua, datiServizioUtile.IsStorico);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertDatiServizioUtile");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaDatiServizioUtileRecordFondo(DatiServizioUtile datiServizioUtile)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertDatiServizioUtileRecordFondo(datiServizioUtile.IdFondo, datiServizioUtile.IdRecordFondo, datiServizioUtile.Quota, datiServizioUtile.ServizioUtileAA,
                    datiServizioUtile.ServizioUtileMM, datiServizioUtile.ServizioUtileGG, datiServizioUtile.RetribuzionePensionabile, datiServizioUtile.ControCodiceRetributivo,
                    datiServizioUtile.Retribuzione, datiServizioUtile.QuoteArt14, datiServizioUtile.ImportoIndennitaIntegrativaSpeciale, datiServizioUtile.ServizioUtileCessazioneAA,
                    datiServizioUtile.ServizioUtileCessazioneMM, datiServizioUtile.ServizioUtileCessazioneGG, datiServizioUtile.QuotaPensioneRetributivaAnnua);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertDatiServizioUtileRecordFondo");
                }
                db.Connection.Close();
            }
        }
    }
}
