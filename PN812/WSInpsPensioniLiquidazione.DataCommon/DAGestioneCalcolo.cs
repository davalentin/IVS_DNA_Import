using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCalcolo
    {
        #region Calcolo Contributivo

        /// <summary>
        /// Recupera i record NON di Storico dalla tabella CalcoloContributivo per FS
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="calcoloContributivo"></param>
        public static void GetCalcoloContributivoByIdPensione(Int64 idPensione, out CalcoloContributivo calcoloContributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    calcoloContributivo = (from cc in db.CalcoloContributivos where cc.IdPensione == idPensione && !cc.IsStorico select cc).FirstOrDefault<CalcoloContributivo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        /// <summary>
        /// Recupera i record NON di Storico dalla tabella CalcoloContributivo per FS nei casi di domande con il quadro Record Fondo
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="listaCalcoloContributivo"></param>
        public static void GetCalcoloContributivoRecordFondoByIdPensione(Int64 idPensione, out List<CalcoloContributivo> listaCalcoloContributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    listaCalcoloContributivo = (from cc in db.CalcoloContributivos where cc.IdPensione == idPensione && !cc.IsStorico select cc).ToList<CalcoloContributivo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCalcoloContributivoByIdRecordFondo(Int64 idRecordFondo, out CalcoloContributivo calcoloContributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    calcoloContributivo = (from cc in db.CalcoloContributivos where cc.IdRecordFondo == idRecordFondo && !cc.IsStorico select cc).SingleOrDefault<CalcoloContributivo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        /// <summary>
        /// Recupera i record di Storico dalla tabella CalcoloContributivo per FS
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="calcoloContributivo"></param>
        public static void GetCalcoloContributivoStoricoByIdPensione(Int64 idPensione, out CalcoloContributivo calcoloContributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    calcoloContributivo = (from cc in db.CalcoloContributivos where cc.IdPensione == idPensione && cc.IsStorico select cc).SingleOrDefault<CalcoloContributivo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        /// <summary>
        /// Recupera i record di Storico dalla tabella CalcoloContributivo per FS nei casi di domande con il quadro Record Fondo
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="listaCalcoloContributivo"></param>
        public static void GetCalcoloContributivoStoricoRecordFondoByIdPensione(Int64 idPensione, out List<CalcoloContributivo> listaCalcoloContributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    listaCalcoloContributivo = (from cc in db.CalcoloContributivos where cc.IdPensione == idPensione && cc.IsStorico select cc).ToList<CalcoloContributivo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaCalcoloContributivo(CalcoloContributivo calcoloContributivo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertCalcoloContributivo(calcoloContributivo.IdPensione, calcoloContributivo.DecorrenzaCalcoloContibutivo,
                                calcoloContributivo.CodiceGestione, calcoloContributivo.ImportoBase, calcoloContributivo.MontanteContributivo,
                                calcoloContributivo.Montante, calcoloContributivo.ImportoContributivoTotale, calcoloContributivo.NSettimane,
                                calcoloContributivo.ImportoIVS, calcoloContributivo.Contributi, calcoloContributivo.NSettimaneLegge335,
                                calcoloContributivo.MontanteInvalidita, calcoloContributivo.QuotaFacoltativaMensile, calcoloContributivo.MontanteAnte0697,
                                calcoloContributivo.AnzianitaAnte0697AA, calcoloContributivo.AnzianitaAnte0697MM, calcoloContributivo.AnzianitaAnte0697GG,
                                calcoloContributivo.AnzianitaPost0697AA, calcoloContributivo.AnzianitaPost0697MM, calcoloContributivo.AnzianitaPost0697GG,
                                calcoloContributivo.MontanteQuotaDL214, calcoloContributivo.ImportoContribTotaleQuotaDL214, calcoloContributivo.NSettimaneQuotaDL214,
                                calcoloContributivo.MontanteEsclusivo, calcoloContributivo.MontanteEsclusivoQuotaDL214, calcoloContributivo.QuotaContributivaAnnua,
                                calcoloContributivo.PL_Quotac, calcoloContributivo.IsStorico);

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertCalcoloContributivo");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaCalcoloContributivoRecordFondo(CalcoloContributivo calcoloContributivo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertCalcoloContributivoRecordFondo(calcoloContributivo.IdPensione, calcoloContributivo.IdRecordFondo, calcoloContributivo.DecorrenzaCalcoloContibutivo,
                                calcoloContributivo.CodiceGestione, calcoloContributivo.ImportoBase, calcoloContributivo.MontanteContributivo,
                                calcoloContributivo.Montante, calcoloContributivo.ImportoContributivoTotale, calcoloContributivo.NSettimane,
                                calcoloContributivo.ImportoIVS, calcoloContributivo.Contributi, calcoloContributivo.NSettimaneLegge335,
                                calcoloContributivo.MontanteInvalidita, calcoloContributivo.QuotaFacoltativaMensile, calcoloContributivo.MontanteAnte0697,
                                calcoloContributivo.AnzianitaAnte0697AA, calcoloContributivo.AnzianitaAnte0697MM, calcoloContributivo.AnzianitaAnte0697GG,
                                calcoloContributivo.AnzianitaPost0697AA, calcoloContributivo.AnzianitaPost0697MM, calcoloContributivo.AnzianitaPost0697GG,
                                calcoloContributivo.MontanteQuotaDL214, calcoloContributivo.ImportoContribTotaleQuotaDL214, calcoloContributivo.NSettimaneQuotaDL214,
                                calcoloContributivo.MontanteEsclusivo, calcoloContributivo.MontanteEsclusivoQuotaDL214, calcoloContributivo.QuotaContributivaAnnua, calcoloContributivo.IsStorico);

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertCalcoloContributivoRecordFondo");
                }
                db.Connection.Close();
            }
        }

        /// <summary>
        /// Elimina tutti i record della tabella CalcoloContributivo afferenti alla pensione
        /// </summary>
        /// <param name="idPensione"></param>
        public static void EliminaCalcoloContributivoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteCalcoloContributivo(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteCalcoloContributivo");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaCalcoloContributivoByIdRecordFondo(long idRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteCalcoloContributivoByIdRecordFondo(idRecordFondo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteCalcoloContributivoByIdRecordFondo");
                }
                db.Connection.Close();
            }
        }

        /// <summary>
        /// Elimina tutti i record, tranne quelli di Storico, della tabella CalcoloContributivo afferenti alla pensione
        /// </summary>
        /// <param name="idPensione"></param>
        public static void EliminaCalcoloContributivoNoStoricoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteCalcoloContributivoNoStorico(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteCalcoloContributivoNoStorico");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaCalcoloContributivoNoStoricoByIdRecordFondo(long idRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteCalcoloContributivoNoStoricoByIdRecordFondo(idRecordFondo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteCalcoloContributivoNoStoricoByIdRecordFondo");
                }
                db.Connection.Close();
            }
        }

        /// <summary>
        /// Recupera i record NON di Storico dalla tabella CalcoloContributivo per AGO e CI
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="lcalcoloContributivo"></param>
        public static void GetCalcoloContributivoCI_AGOByIdPensione(Int64 idPensione, out List<CalcoloContributivo> lcalcoloContributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lcalcoloContributivo = (from cc in db.CalcoloContributivos where cc.IdPensione == idPensione && !cc.IsStorico select cc).ToList<CalcoloContributivo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
        
        /// <summary>
        /// Recupera i record di Storico dalla tabella CalcoloContributivo per AGO e CI
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="lcalcoloContributivo"></param>
        public static void GetCalcoloContributivoStoricoCI_AGOByIdPensione(Int64 idPensione, out List<CalcoloContributivo> lcalcoloContributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lcalcoloContributivo = (from cc in db.CalcoloContributivos where cc.IdPensione == idPensione && cc.IsStorico select cc).ToList<CalcoloContributivo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaCalcoloContributivoCI_AGO(CalcoloContributivo calcoloContributivo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertCalcoloContributivoCI_AGO(calcoloContributivo.IdPensione, calcoloContributivo.DecorrenzaCalcoloContibutivo,
                                calcoloContributivo.CodiceGestione, calcoloContributivo.ImportoBase, calcoloContributivo.MontanteContributivo,
                                calcoloContributivo.Montante, calcoloContributivo.ImportoContributivoTotale, calcoloContributivo.NSettimane,
                                calcoloContributivo.ImportoIVS, calcoloContributivo.Contributi, calcoloContributivo.NSettimaneLegge335,
                                calcoloContributivo.MontanteInvalidita, calcoloContributivo.QuotaFacoltativaMensile, calcoloContributivo.MontanteAnte0697,
                                calcoloContributivo.AnzianitaAnte0697AA, calcoloContributivo.AnzianitaAnte0697MM, calcoloContributivo.AnzianitaAnte0697GG,
                                calcoloContributivo.AnzianitaPost0697AA, calcoloContributivo.AnzianitaPost0697MM, calcoloContributivo.AnzianitaPost0697GG,
                                calcoloContributivo.MontanteQuotaDL214, calcoloContributivo.ImportoContribTotaleQuotaDL214, calcoloContributivo.NSettimaneQuotaDL214,
                                calcoloContributivo.PL_Quotac, calcoloContributivo.IsStorico);

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertCalcoloContributivoCI_AGO");
                }
                db.Connection.Close();
            }
        }

        #endregion Calcolo Contributivo

        #region Calcolo Retributivo

        /// <summary>
        /// Recupera i record NON di Storico dalla tabella CalcoloRetributivo per FS
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="calcoloRetributivo"></param>
        public static void GetCalcoloRetributivoByIdPensione(Int64 idPensione, out CalcoloRetributivo calcoloRetributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    calcoloRetributivo = (from cc in db.CalcoloRetributivos where cc.IdPensione == idPensione && !cc.IsStorico select cc).SingleOrDefault<CalcoloRetributivo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        /// <summary>
        /// Recupera i record NON di Storico dalla tabella CalcoloRetributivo per FS nei casi di domande con il quadro Record Fondo
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="listaCalcoloRetributivo"></param>
        public static void GetCalcoloRetributivoRecordFondoByIdPensione(Int64 idPensione, out List<CalcoloRetributivo> listaCalcoloRetributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    listaCalcoloRetributivo = (from cc in db.CalcoloRetributivos where cc.IdPensione == idPensione && !cc.IsStorico select cc).ToList<CalcoloRetributivo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCalcoloRetributivoByIdRecordFondo(Int64 idRecordFondo, out CalcoloRetributivo calcoloContributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    calcoloContributivo = (from cc in db.CalcoloRetributivos where cc.IdRecordFondo == idRecordFondo && !cc.IsStorico select cc).SingleOrDefault<CalcoloRetributivo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
        /// <summary>
        /// Recupera i record di Storico dalla tabella CalcoloRetributivo per FS
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="calcoloRetributivo"></param>
        public static void GetCalcoloRetributivoStoricoByIdPensione(Int64 idPensione, out CalcoloRetributivo calcoloRetributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    calcoloRetributivo = (from cc in db.CalcoloRetributivos where cc.IdPensione == idPensione && cc.IsStorico select cc).SingleOrDefault<CalcoloRetributivo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaCalcoloRetributivo(CalcoloRetributivo calcoloRetributivo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertCalcoloRetributivo(calcoloRetributivo.IdPensione, calcoloRetributivo.CodiceLiquidazione, calcoloRetributivo.DecorrenzaOriginariaPensione,
                                calcoloRetributivo.NSettimaneAnzianita, calcoloRetributivo.NSettimaneQuotaA, calcoloRetributivo.NSettimaneQuotaA2, calcoloRetributivo.NSettimaneQuotaB,
                                calcoloRetributivo.RMS, calcoloRetributivo.RMSQuotaA, calcoloRetributivo.RMSQuotaB, calcoloRetributivo.NSettimaneExCombattente,
                                calcoloRetributivo.NSettimaneQuotaC, calcoloRetributivo.NSettimaneQuotaC2, calcoloRetributivo.RMSExCombattente, calcoloRetributivo.MontanteContributivoAGO,
                                calcoloRetributivo.RetribuzionePonderataAnnua, calcoloRetributivo.RMSQuotaD, calcoloRetributivo.RetribuzioneAGO,
                                calcoloRetributivo.NSettAnzianitaVL, calcoloRetributivo.NSettAnzianitaVV, calcoloRetributivo.MeseDiRiferimentoQuotaBDZ,
                                calcoloRetributivo.NSettimaneQuotaD, calcoloRetributivo.CodiceGestione, calcoloRetributivo.QuotePrimeLiquidate,
                                calcoloRetributivo.NSettimaneEsclusiveQuotaA, calcoloRetributivo.NSettimaneEsclusiveQuotaB, calcoloRetributivo.PL_Quotar, calcoloRetributivo.PL_Quotar707, calcoloRetributivo.IsStorico);


                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertCalcoloRetributivo");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaCalcoloRetributivoRecordFondo(CalcoloRetributivo calcoloRetributivo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertCalcoloRetributivoRecordFondo(calcoloRetributivo.IdPensione, calcoloRetributivo.IdRecordFondo, calcoloRetributivo.CodiceLiquidazione, calcoloRetributivo.DecorrenzaOriginariaPensione,
                                calcoloRetributivo.NSettimaneAnzianita, calcoloRetributivo.NSettimaneQuotaA, calcoloRetributivo.NSettimaneQuotaA2, calcoloRetributivo.NSettimaneQuotaB,
                                calcoloRetributivo.RMS, calcoloRetributivo.RMSQuotaA, calcoloRetributivo.RMSQuotaB, calcoloRetributivo.NSettimaneExCombattente,
                                calcoloRetributivo.NSettimaneQuotaC, calcoloRetributivo.NSettimaneQuotaC2, calcoloRetributivo.RMSExCombattente, calcoloRetributivo.MontanteContributivoAGO,
                                calcoloRetributivo.RetribuzionePonderataAnnua, calcoloRetributivo.RMSQuotaD, calcoloRetributivo.RetribuzioneAGO,
                                calcoloRetributivo.NSettAnzianitaVL, calcoloRetributivo.NSettAnzianitaVV, calcoloRetributivo.MeseDiRiferimentoQuotaBDZ,
                                calcoloRetributivo.NSettimaneQuotaD, calcoloRetributivo.CodiceGestione, calcoloRetributivo.QuotePrimeLiquidate,
                                calcoloRetributivo.NSettimaneEsclusiveQuotaA, calcoloRetributivo.NSettimaneEsclusiveQuotaB, calcoloRetributivo.PL_Quotar, calcoloRetributivo.PL_Quotar707, calcoloRetributivo.IsStorico);

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertCalcoloContributivoRecordFondo");
                }
                db.Connection.Close();
            }
        }

        /// <summary>
        /// Elimina tutti i record della tabella CalcoloRetributivo afferenti alla pensione
        /// </summary>
        /// <param name="idPensione"></param>
        public static void EliminaCalcoloRetributivoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteCalcoloRetributivo(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteCalcoloRetributivo");
                }
                db.Connection.Close();
            }
        }

        /// <summary>
        /// Elimina tutti i record, tranne quelli di Storico, della tabella CalcoloRetributivo afferenti alla pensione
        /// </summary>
        /// <param name="idPensione"></param>
        public static void EliminaCalcoloRetributivoNoStoricoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteCalcoloRetributivoNoStorico(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteCalcoloRetributivoNoStorico");
                }
                db.Connection.Close();
            }
        }

        /// <summary>
        /// Recupera i record NON di Storico dalla tabella CalcoloRetributivo per AGO e CI
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="lcalcoloRetributivo"></param>
        public static void GetCalcoloRetributivoCI_AGOByIdPensione(Int64 idPensione, out List<CalcoloRetributivo> lcalcoloRetributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lcalcoloRetributivo = (from cc in db.CalcoloRetributivos where cc.IdPensione == idPensione && !cc.IsStorico select cc).ToList<CalcoloRetributivo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        /// <summary>
        /// Recupera i record di Storico dalla tabella CalcoloRetributivo per AGO e CI
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="lcalcoloRetributivo"></param>
        public static void GetCalcoloRetributivoStoricoCI_AGOByIdPensione(Int64 idPensione, out List<CalcoloRetributivo> lcalcoloRetributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lcalcoloRetributivo = (from cc in db.CalcoloRetributivos where cc.IdPensione == idPensione && cc.IsStorico select cc).ToList<CalcoloRetributivo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaCalcoloRetributivoCI_AGO(CalcoloRetributivo calcoloRetributivo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertCalcoloRetributivoCI_AGO(calcoloRetributivo.IdPensione, calcoloRetributivo.CodiceLiquidazione, calcoloRetributivo.DecorrenzaOriginariaPensione,
                                calcoloRetributivo.NSettimaneAnzianita, calcoloRetributivo.NSettimaneQuotaA, calcoloRetributivo.NSettimaneQuotaA2, calcoloRetributivo.NSettimaneQuotaB,
                                calcoloRetributivo.RMS, calcoloRetributivo.RMSQuotaA, calcoloRetributivo.RMSQuotaB, calcoloRetributivo.NSettimaneExCombattente,
                                calcoloRetributivo.NSettimaneQuotaC, calcoloRetributivo.NSettimaneQuotaC2, calcoloRetributivo.RMSExCombattente, calcoloRetributivo.MontanteContributivoAGO,
                                calcoloRetributivo.RetribuzionePonderataAnnua, calcoloRetributivo.RMSQuotaD, calcoloRetributivo.RetribuzioneAGO,
                                calcoloRetributivo.NSettAnzianitaVL, calcoloRetributivo.NSettAnzianitaVV, calcoloRetributivo.MeseDiRiferimentoQuotaBDZ,
                                calcoloRetributivo.NSettimaneQuotaD, calcoloRetributivo.CodiceGestione, calcoloRetributivo.QuotePrimeLiquidate, calcoloRetributivo.CodiceTipoQuota, 
                                calcoloRetributivo.NSettimane707, calcoloRetributivo.PL_Quotar, calcoloRetributivo.PL_Quotar707, calcoloRetributivo.IsStorico);


                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertCalcoloRetributivoCI_AGO");
                }
                db.Connection.Close();
            }
        }

        #endregion Calcolo Retributivo

        #region ContributiEsteri

        public static void GetPensioniCiContributiEE_ByIdPensione(Int64 idPensione, out List<PensioniCiContributiEE> lcalcoloContributivoEE)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lcalcoloContributivoEE = (from cc in db.PensioniCiContributiEEs where cc.IdPensione == idPensione select cc).ToList<PensioniCiContributiEE>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaPensioniCiContributiEE(PensioniCiContributiEE pensioniCiContributiEE)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPensioniCiContributiEE(pensioniCiContributiEE.IdPensione, pensioniCiContributiEE.CodiceGestione, pensioniCiContributiEE.Decorrenza, pensioniCiContributiEE.Settimane);

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure SalvaPensioniCiContributiEE");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaPensioniCiContributiEEByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioniCiContributiEE(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure EliminaPensioniCiContributiEE");
                }
                db.Connection.Close();
            }
        }

        #endregion ContributiEsteri

        #region Enpals

        #region Contributivo

        public static void GetCalcoloContributivoENPALSByIdPensione(Int64 idPensione, out CalcoloContributivoENPAL calcoloContributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    calcoloContributivo = (from cc in db.CalcoloContributivoENPALs where cc.IdPensione == idPensione && !cc.IsStorico select cc).SingleOrDefault<CalcoloContributivoENPAL>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCalcoloContributivoENPALSStoricoByIdPensione(Int64 idPensione, out CalcoloContributivoENPAL calcoloContributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    calcoloContributivo = (from cc in db.CalcoloContributivoENPALs where cc.IdPensione == idPensione && cc.IsStorico select cc).SingleOrDefault<CalcoloContributivoENPAL>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaCalcoloContributivoEnpals(CalcoloContributivoENPAL calcoloContributivo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertCalcoloContributivoENPALS(calcoloContributivo.IdPensione, calcoloContributivo.NumeroContributiTotale, calcoloContributivo.CoefficienteTrasformazione,
                                calcoloContributivo.ImportoContributivoTotale, calcoloContributivo.Montante, calcoloContributivo.Quota, calcoloContributivo.Decorrenza, calcoloContributivo.IsStorico);

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertCalcoloContributivoENPALS");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaCalcoloContributivoEnpalsByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteCalcoloContributivoENPALS(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteCalcoloContributivoENPALS");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaCalcoloContributivoEnpalsNoStoricoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteCalcoloContributivoENPALSNoStorico(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteCalcoloContributivoENPALSNoStorico");
                }
                db.Connection.Close();
            }
        }

        #endregion Contributivo

        #region Retributivo

        public static void GetCalcoloRetributivoEnpalsByIdPensione(Int64 idPensione, out CalcoloRetributivoENPAL calcoloRetributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    calcoloRetributivo = (from cc in db.CalcoloRetributivoENPALs where cc.IdPensione == idPensione && !cc.IsStorico select cc).SingleOrDefault<CalcoloRetributivoENPAL>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCalcoloRetributivoEnpalsStoricoByIdPensione(Int64 idPensione, out CalcoloRetributivoENPAL calcoloRetributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    calcoloRetributivo = (from cc in db.CalcoloRetributivoENPALs where cc.IdPensione == idPensione && cc.IsStorico select cc).SingleOrDefault<CalcoloRetributivoENPAL>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaCalcoloRetributivoEnpals(CalcoloRetributivoENPAL calcoloRetributivo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertCalcoloRetributivoENPALS(calcoloRetributivo.IdPensione, calcoloRetributivo.PeriodiQuotaA, calcoloRetributivo.PeriodiQuotaB,
                                calcoloRetributivo.NTotaleContributiCalcoloQuotaA, calcoloRetributivo.NTotaleContributiCalcoloQuotaB, calcoloRetributivo.RMQuotaA, calcoloRetributivo.RMQuotaB,
                                calcoloRetributivo.ImportoQuotaA, calcoloRetributivo.ImportoQuotaB, calcoloRetributivo.ImportoProRataTemporis, calcoloRetributivo.ImportoQuotaRetributivaInMisto,
                                calcoloRetributivo.GiorniQuotaA707, calcoloRetributivo.ImportoQuotaA707, calcoloRetributivo.GiorniQuotaB707, calcoloRetributivo.ImportoQuotaB707, 
                                calcoloRetributivo.DecorrenzaQuotaA, calcoloRetributivo.DecorrenzaQuotaB, calcoloRetributivo.IsStorico);


                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertCalcoloRetributivoENPALS");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaCalcoloRetributivoByIdRecordFondo(long idRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteCalcoloRetributivoByIdRecordFondo(idRecordFondo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteCalcoloRetributivoByIdRecordFondo");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaCalcoloRetributivoEnpalsByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteCalcoloRetributivoENPALS(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteCalcoloRetributivoENPALS");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaCalcoloRetributivoEnpalsNoStoricoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteCalcoloRetributivoENPALSNoStorico(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteCalcoloRetributivoENPALSNoStorico");
                }
                db.Connection.Close();
            }
        }

        #endregion Retributivo

        #endregion Enpals

        #region Cumulo L.228/2012

        /// <summary>
        /// Recupera i record NON di Storico dalla tabella QuotePensione
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="lQuotePensione"></param>
        public static void GetQuotePensioneByIdPensione(Int64 idPensione, out List<QuotePensione> lQuotePensione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lQuotePensione = (from cc in db.QuotePensiones where cc.IdPensione == idPensione && !cc.IsStorico select cc).ToList<QuotePensione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        /// <summary>
        /// Recupera i record di Storico dalla tabella QuotePensione
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="lQuotePensione"></param>
        public static void GetQuotePensioneStoricoByIdPensione(Int64 idPensione, out List<QuotePensione> lQuotePensione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lQuotePensione = (from cc in db.QuotePensiones where cc.IdPensione == idPensione && cc.IsStorico select cc).ToList<QuotePensione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaQuotePensione(QuotePensione quotePensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuotePensione(quotePensione.IdPensione, quotePensione.EnteGestioneFondo, quotePensione.Settimane, quotePensione.Importo, quotePensione.Decorrenza, quotePensione.IsStorico);

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuotePensione");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaQuotePensioneByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuotePensione(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuotePensione");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaQuotePensioneNoStoricoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuotePensioneNoStorico(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuotePensioneNoStorico");
                }
                db.Connection.Close();
            }
        }

        /// <summary>
        /// Recupera i record NON di Storico dalla tabella TrattenuteQuotePensione
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="lTrattenuteQuotePensione"></param>
        public static void GetTrattenuteQuotePensioneByIdPensione(Int64 idPensione, out List<TrattenuteQuotePensione> lTrattenuteQuotePensione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lTrattenuteQuotePensione = (from tQp in db.TrattenuteQuotePensiones where tQp.IdPensione == idPensione && !tQp.IsStorico select tQp).ToList<TrattenuteQuotePensione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        /// <summary>
        /// Recupera i record di Storico dalla tabella TrattenuteQuotePensione
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="lTrattenuteQuotePensione"></param>
        public static void GetTrattenuteQuotePensioneStoricoByIdPensione(Int64 idPensione, out List<TrattenuteQuotePensione> lTrattenuteQuotePensione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lTrattenuteQuotePensione = (from tQp in db.TrattenuteQuotePensiones where tQp.IdPensione == idPensione && tQp.IsStorico select tQp).ToList<TrattenuteQuotePensione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }


        public static void SalvaTrattenuteQuotePensione(TrattenuteQuotePensione trattenute)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertTrattenuteQuotePensione(trattenute.IdPensione, trattenute.EnteGestioneFondoQuote, trattenute.AnnoCompetenza, trattenute.CodiceTrattenute, trattenute.ImportoTrattenute, trattenute.IsStorico);

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertTrattenuteQuotePensione");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaTrattenuteQuotePensioneByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteTrattenuteQuotePensione(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteTrattenuteQuotePensione");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaTrattenuteQuotePensioneNoStoricoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteTrattenuteQuotePensioneNoStorico(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteTrattenuteQuotePensioneNoStorico");
                }
                db.Connection.Close();
            }
        }
        #endregion Cumulo L.228/2012

        #region Dati Calcolo 707

        public static void GetDatiServizioUtile707ByIdPensione(Int64 idPensione, out List<DatiServizioUtile707> lDatiServizioUtile)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lDatiServizioUtile = (from su in db.DatiServizioUtile707s
                                          join fdg in db.PensioneFondoDatiGenericis on su.IdFondo equals fdg.Id
                                          where fdg.IdPensione == idPensione && !su.IsStorico
                                          select su).ToList<DatiServizioUtile707>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDatiServizioUtile707ByIdRecordFondo(Int64 idRecordFondo, out List<DatiServizioUtile707> lDatiServizioUtile)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lDatiServizioUtile = (from su in db.DatiServizioUtile707s
                                          where su.IdRecordFondo == idRecordFondo
                                          select su).ToList<DatiServizioUtile707>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaDatiServizioUtile707ByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteDatiServizioUtile707(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteDatiServizioUtile707");
                }
                db.Connection.Close();
            }
        }
        
        public static void EliminaDatiServizioUtile707ByIdRecordFondo(long idRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteDatiServizioUtile707RecordFondo(idRecordFondo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteDatiServizioUtile707RecordFondo");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaDatiServizioUtile707(DatiServizioUtile707 datiServizioUtile)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertDatiServizioUtile707(datiServizioUtile.IdFondo, datiServizioUtile.Quota, datiServizioUtile.ServizioUtileAA,
                    datiServizioUtile.ServizioUtileMM, datiServizioUtile.ServizioUtileGG, datiServizioUtile.ServizioUtileCessazioneAA,
                    datiServizioUtile.ServizioUtileCessazioneMM, datiServizioUtile.ServizioUtileCessazioneGG, datiServizioUtile.QuotaPensioneRetributivaAnnua, datiServizioUtile.IsStorico);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertDatiServizioUtile707");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaDatiServizioUtile707RecordFondo(DatiServizioUtile707 datiServizioUtile)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertDatiServizioUtile707RecordFondo(datiServizioUtile.IdFondo, datiServizioUtile.IdRecordFondo, datiServizioUtile.Quota, datiServizioUtile.ServizioUtileAA,
                    datiServizioUtile.ServizioUtileMM, datiServizioUtile.ServizioUtileGG, datiServizioUtile.ServizioUtileCessazioneAA,
                    datiServizioUtile.ServizioUtileCessazioneMM, datiServizioUtile.ServizioUtileCessazioneGG, datiServizioUtile.QuotaPensioneRetributivaAnnua, datiServizioUtile.IsStorico);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertDatiServizioUtile707RecordFondo");
                }
                db.Connection.Close();
            }
        }
        #endregion Dati Calcolo 707

        #region Dati Calcolo 707 INPDAP

        public static void GetDatiServizioUtileINPDAP707ByIdPensione(Int64 idPensione, out List<DatiServizioUtileINPDAP707> lDatiServizioUtile)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lDatiServizioUtile = (from su in db.DatiServizioUtileINPDAP707s
                                          where su.IdPensione == idPensione
                                          select su).ToList<DatiServizioUtileINPDAP707>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDatiServizioUtileINPDAP707ByIdRecordFondo(Int64 idRecordFondo, out List<DatiServizioUtileINPDAP707> lDatiServizioUtile)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lDatiServizioUtile = (from su in db.DatiServizioUtileINPDAP707s
                                          where su.IdRecordFondo == idRecordFondo
                                          select su).ToList<DatiServizioUtileINPDAP707>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaDatiServizioUtileINPDAP707ByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteDatiServizioUtileINPDAP707(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteDatiServizioUtileINPDAP707");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaDatiServizioUtileINPDAP707ByIdRecordFondo(long idRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteDatiServizioUtileINPDAP707RecordFondo(idRecordFondo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteDatiServizioUtileINPDAP707RecordFondo");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaDatiServizioUtileINPDAP707(long idPensione, DatiServizioUtileINPDAP707 datiServizioUtile)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertDatiServizioUtileINPDAP707(idPensione, datiServizioUtile.IdRecordFondo, datiServizioUtile.Quota, datiServizioUtile.ServizioUtileAA,
                    datiServizioUtile.ServizioUtileMM, datiServizioUtile.ServizioUtileGG, datiServizioUtile.ServizioUtileCessazioneAA,
                    datiServizioUtile.ServizioUtileCessazioneMM, datiServizioUtile.ServizioUtileCessazioneGG, datiServizioUtile.QuotaPensioneRetributivaAnnua); 
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertDatiServizioUtileINPDAP707");
                }
                db.Connection.Close();
            }
        }

        #endregion Dati Calcolo 707 INPDAP
    }
}