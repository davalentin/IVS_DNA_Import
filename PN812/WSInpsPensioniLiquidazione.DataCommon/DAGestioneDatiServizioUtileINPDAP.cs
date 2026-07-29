using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneDatiServizioUtileINPDAP
    {
        /// <summary>
        /// Recupera i record NON di Storico dalla tabella DatiServizioUtile
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="lDatiServizioUtile"></param>
        public static void GetDatiServizioUtileByIdPensione(Int64 idPensione, out List<DatiServizioUtileINPDAP> lDatiServizioUtile)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lDatiServizioUtile = (from su in db.DatiServizioUtileINPDAPs
                                          where su.IdPensione == idPensione
                                          select su).ToList<DatiServizioUtileINPDAP>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDatiServizioUtileByIdRecordFondo(Int64 idRecordFondo, out List<DatiServizioUtileINPDAP> lDatiServizioUtile)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lDatiServizioUtile = (from su in db.DatiServizioUtileINPDAPs
                                          where su.IdRecordFondo == idRecordFondo
                                          select su).ToList<DatiServizioUtileINPDAP>();
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
                int result = db.DeleteAllDatiServizioUtileINPDAP(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllDatiServizioUtileINPDAP");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaDatiServizioUtileByIdRecordFondo(long idRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteDatiServizioUtileINPDAPRecordFondo(idRecordFondo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteDatiServizioUtileINPDAPRecordFondo");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaDatiServizioUtile(DatiServizioUtileINPDAP datiServizioUtile)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertDatiServizioUtileINPDAP(datiServizioUtile.IdPensione, datiServizioUtile.IdRecordFondo, datiServizioUtile.Quota, datiServizioUtile.ServizioUtileAA,
                    datiServizioUtile.ServizioUtileMM, datiServizioUtile.ServizioUtileGG, datiServizioUtile.RetribuzionePensionabile, 
                    datiServizioUtile.Retribuzione, datiServizioUtile.QuoteArt14, datiServizioUtile.ImportoIndennitaIntegrativaSpeciale, datiServizioUtile.ServizioUtileCessazioneAA,
                    datiServizioUtile.ServizioUtileCessazioneMM, datiServizioUtile.ServizioUtileCessazioneGG, datiServizioUtile.QuotaPensioneRetributivaAnnua);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertDatiServizioUtileINPDAP");
                }
                db.Connection.Close();
            }
        }
    }
}
