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
    public class DAGestioneFondo
    {
        public static void GetIdFondoByIdPensione(Int64 idPensione, out long idFondo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    idFondo = (from p in db.PensioneFondoDatiGenericis where p.IdPensione == idPensione select p.Id).SingleOrDefault<long>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetFondoDatiGenericiByIdPensione(Int64 idPensione, out PensioneFondoDatiGenerici fondoDatiGenerici)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    fondoDatiGenerici = (from f in db.PensioneFondoDatiGenericis
                                         where f.IdPensione == idPensione
                                         select f).SingleOrDefault<PensioneFondoDatiGenerici>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaFondoDatiGenerici(PensioneFondoDatiGenerici fondoDatiGenerici)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                long? idFondo = null;

                int result = db.InsertPensioneFondoDatiGenerici(fondoDatiGenerici.IdPensione, fondoDatiGenerici.AliquotaIrpef,
                    fondoDatiGenerici.CapitalizzazioneNetta, fondoDatiGenerici.TipoRecord, fondoDatiGenerici.DataEliminazione,
                    fondoDatiGenerici.DataUltimaRicostituzione, fondoDatiGenerici.DataRipristinoPagamento, fondoDatiGenerici.CodiceCategoriaPensioneSospesa,
                    fondoDatiGenerici.CodiceSedePensioneSospesa, fondoDatiGenerici.NCertificatoPensioneSospesa, fondoDatiGenerici.CodicePensionePrecedente,
                    fondoDatiGenerici.CodiceCristallizzazione,
                    fondoDatiGenerici.TipoPensione, fondoDatiGenerici.AttivitaSvolta, fondoDatiGenerici.Decorrenza, fondoDatiGenerici.DecorrenzaValiditaDati,
                    fondoDatiGenerici.DataSospensione, fondoDatiGenerici.ServizioUtileAAMM, fondoDatiGenerici.ServizioUtileGG,
                    fondoDatiGenerici.RetribuzionePensionabile, fondoDatiGenerici.CodiceNatura, fondoDatiGenerici.CodiceDirittoQuoteFisse,
                    fondoDatiGenerici.RetribuzionePensioneExCombattente,
                    fondoDatiGenerici.AttribuzioneBonus, fondoDatiGenerici.InizioBonus, fondoDatiGenerici.FineBonus,
                    fondoDatiGenerici.CodiceSpecifico, fondoDatiGenerici.CodiceRequisiti1, fondoDatiGenerici.CodiceRequisiti2, fondoDatiGenerici.ChkDL407,
                    fondoDatiGenerici.Articolo2, fondoDatiGenerici.Privilegiate, fondoDatiGenerici.RiduzioneRetributiva, fondoDatiGenerici.RiduzioneRetributivaPercentuale,
                    fondoDatiGenerici.QuotaA707, fondoDatiGenerici.QuotaA2707, fondoDatiGenerici.QuotaB707, fondoDatiGenerici.QuotaC707, fondoDatiGenerici.QuotaC2707, fondoDatiGenerici.QuotaD707,
                    fondoDatiGenerici.QuotaA707AA, fondoDatiGenerici.QuotaA707MM, fondoDatiGenerici.QuotaA707GG, fondoDatiGenerici.QuotaB707AA, fondoDatiGenerici.QuotaB707MM, fondoDatiGenerici.QuotaB707GG,
                    fondoDatiGenerici.QuotaC707AA, fondoDatiGenerici.QuotaC707MM, fondoDatiGenerici.QuotaC707GG,
                    fondoDatiGenerici.RetribuzionePonderataAGO707, fondoDatiGenerici.RetrPondAnnuaAGOLimite,
                    fondoDatiGenerici.QuotaAES707, fondoDatiGenerici.QuotaBES707, fondoDatiGenerici.SettimaneUtiliDiritto, fondoDatiGenerici.PersonaleViaggiante, fondoDatiGenerici.CodiceSpecificoGP, fondoDatiGenerici.SettimaneUtiliDirittoOI, ref idFondo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensioneFondoDatiGenerici");
                }
                fondoDatiGenerici.Id = idFondo.HasValue ? idFondo.Value : 0;
                db.Connection.Close();
            }
        }

        public static void EliminaFondoDatiGenericiByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioneFondoDatiGenerici(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioneFondoDatiGenerici");
                }
                db.Connection.Close();
            }
        }

        #region FondoEL

        public static void GetFondoELByIdPensione(Int64 idPensione, out PensioneFondoEL fondoEL)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    fondoEL = (from f in db.PensioneFondoELs
                               join fdg in db.PensioneFondoDatiGenericis on f.IdFondo equals fdg.Id
                               where fdg.IdPensione == idPensione
                               select f).SingleOrDefault<PensioneFondoEL>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaFondoELByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioneFondoEL(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioneFondoEL");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaFondoEL(PensioneFondoEL fondoEL)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPensioneFondoEL(fondoEL.IdFondo, fondoEL.TettoAgo, fondoEL.CodiceAzienda,
                    fondoEL.AnnoAnzianitaPregressa, fondoEL.MeseAnzianitaPregressa, fondoEL.AnnoRiscatti, fondoEL.MeseRiscatti,
                    fondoEL.DecorrenzaTeorica, fondoEL.AnnoArt3Legge107971, fondoEL.MeseArt3Legge107971,
                    fondoEL.AnnoServizioMilitare, fondoEL.MeseServizioMilitare, fondoEL.MaggiorazioneSenzaLegge33670, fondoEL.Decorrenza, fondoEL.ProRataEnel,
                    fondoEL.GradoInvalidita, fondoEL.PercentualeMaggiorazione, fondoEL.PercentualeRiduzione, fondoEL.ConvenzioneInternazionale,
                    fondoEL.Requisiti247_243, fondoEL.NumeroTriSemRequisiti, fondoEL.AnnoRequisiti, fondoEL.AnzianitaAnni);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensioneFondoEL");
                }
                db.Connection.Close();
            }
        }

        #endregion FondoEL

        #region FondoTT

        public static void GetFondoTTByIdPensione(Int64 idPensione, out PensioneFondoTT fondoTT)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    fondoTT = (from f in db.PensioneFondoTTs
                               join fdg in db.PensioneFondoDatiGenericis on f.IdFondo equals fdg.Id
                               where fdg.IdPensione == idPensione
                               select f).SingleOrDefault<PensioneFondoTT>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaFondoTTByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioneFondoTT(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioneFondoTT");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaFondoTT(PensioneFondoTT fondoTT)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPensioneFondoTT(fondoTT.IdFondo, fondoTT.RiscattiContributiFissiAnni, fondoTT.RiscattiContributiFissiMesi, fondoTT.RiscattiContributiFissiGiorni,
                    fondoTT.RiscattiRiservaMatematicaAnni, fondoTT.RiscattiRiservaMatematicaMesi, fondoTT.RiscattiRiservaMatematicaGiorni, fondoTT.PeriodiFigurativiAnni, fondoTT.PeriodiFigurativiMesi,
                    fondoTT.PeriodiFigurativiGiorni, fondoTT.Decorrenza, fondoTT.DecorrenzaTeorica, fondoTT.SupplementoLegge58367, fondoTT.PensioneMensileAl53, fondoTT.RetribuzioneUltimoAnnoQuotaA,
                    fondoTT.RetribuzioneBiennio, fondoTT.ElementiAccessori, fondoTT.RenditaInailAnnua, fondoTT.RetribuzioneMensileInail, fondoTT.PensioneDirettaGenitori, fondoTT.RetribuzioneSupplementi,
                    fondoTT.Requisiti247_243, fondoTT.NumeroTriSemRequisiti, fondoTT.AnnoRequisiti, fondoTT.AnzianitaAnni, fondoTT.ConvenzioneInternazionale, fondoTT.Ditta, fondoTT.CodiceArt5L58,
                    fondoTT.DimissioniAnte97);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensioneFondoTT");
                }
                db.Connection.Close();
            }
        }

        #endregion FondoTT

        #region FondoET

        public static void GetFondoETByIdPensione(Int64 idPensione, out PensioneFondoET fondoET)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    fondoET = (from f in db.PensioneFondoETs
                               join fdg in db.PensioneFondoDatiGenericis on f.IdFondo equals fdg.Id
                               where fdg.IdPensione == idPensione
                               select f).SingleOrDefault<PensioneFondoET>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaFondoETByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioneFondoET(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioneFondoET");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaFondoET(PensioneFondoET fondoET)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPensioneFondoET(fondoET.IdFondo, fondoET.CodAzienda, fondoET.DataEsonero, fondoET.DecorrenzaTeorica, fondoET.ContributiAgoLegge140830, fondoET.ContributiAgoLegge40245,
                    fondoET.CodiceRateo66, fondoET.RetribuzioneEsodo, fondoET.GGInterruzione, fondoET.NSettimaneLeva, fondoET.NSettimaneRichiamato, fondoET.Stipendio, fondoET.Importo13ma,
                    fondoET.Importo14ma, fondoET.ElementiAccessori, fondoET.Competenze40Percento, fondoET.GradoInvalidita, fondoET.ImportoRenditaInail, fondoET.RetribuzioneEffettiva, fondoET.PartTime,
                    fondoET.AAInterruzione, fondoET.MMInterruzione, fondoET.CodiceServizioMilitare, fondoET.CodiceEsodo, fondoET.Requisiti247_243, fondoET.NumeroTriSemRequisiti, fondoET.AnnoRequisiti,
                    fondoET.AnzianitaAnni, fondoET.PersonaleViaggiante, fondoET.SetAnzTotAltraPensione, fondoET.BaseAltraPensione, fondoET.CategoriaAltraPensione, fondoET.CertificatoAltraPensione,
                    fondoET.RmsImpAltraPensione, fondoET.DecorrenzaAltraPensione, fondoET.RevAltraPensione, fondoET.TipoLiquidazione, fondoET.DecorrenzaPrimoSupplemento, fondoET.ImpContribPrimoSupplemento,
                    fondoET.DecorrenzaSecondoSupplemento, fondoET.ImpContribSecondoSupplemento);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensioneFondoET");
                }
                db.Connection.Close();
            }
        }

        #endregion FondoET

        #region FondoVL

        public static void GetFondoVLByIdPensione(Int64 idPensione, out PensioneFondoVL fondoVL)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    fondoVL = (from f in db.PensioneFondoVLs
                               join fdg in db.PensioneFondoDatiGenericis on f.IdFondo equals fdg.Id
                               where fdg.IdPensione == idPensione
                               select f).SingleOrDefault<PensioneFondoVL>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaFondoVLByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioneFondoVL(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioneFondoVL");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaFondoVL(PensioneFondoVL fondoVL)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPensioneFondoVL(fondoVL.IdFondo, fondoVL.AliquotaIrpef, fondoVL.DataInvalidita, fondoVL.ImportoPensione1977, fondoVL.DecorrenzaPensione,
                    fondoVL.DecorrenzaPensioneAgo, fondoVL.ImportoPensioneAgo, fondoVL.ImportoPensioneAgoSupplementare, fondoVL.Decorrenza, fondoVL.CodiceArt22,
                    fondoVL.ServizioUtileQuotaA1, fondoVL.ServizioUtileQuotaB, fondoVL.ServizioUtileQuotaA2, fondoVL.ServizioUtileQuotaC, fondoVL.ProsecuzioneVolontariaAA,
                    fondoVL.RiscattiRicongiunzioniAA, fondoVL.CodiceCapitalizzazione, fondoVL.ImportoPercentualeCapitalizzazione, fondoVL.RetribuzioneSettimanaleAgoQuotaA, fondoVL.RetribuzioneSettimanaleAgoQuotaB,
                    fondoVL.QuotaMensileCapitalizzazione, fondoVL.CapitaleErogato, fondoVL.ProsecuzioneVolontariaMM, fondoVL.ProsecuzioneVolontariaGG, fondoVL.RiscattiRicongiunzioniMM,
                    fondoVL.RiscattiRicongiunzioniGG, fondoVL.ConvenzioneInternazionale, fondoVL.LavoratorePrecoce, fondoVL.Requisiti247_243, fondoVL.NumeroTriSemRequisiti, fondoVL.AnnoRequisiti, fondoVL.AnzianitaAnni);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensioneFondoVL");
                }
                db.Connection.Close();
            }
        }

        #endregion FondoVL

        #region Fondo FS

        public static void GetFondoFSTByIdPensione(Int64 idPensione, out PensioneFondoFST fondoFST)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    fondoFST = (from f in db.PensioneFondoFSTs
                                join fdg in db.PensioneFondoDatiGenericis on f.IdFondo equals fdg.Id
                                where fdg.IdPensione == idPensione
                                select f).FirstOrDefault<PensioneFondoFST>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetFondoFSTRecordFondoByIdPensione(Int64 idPensione, out List<PensioneFondoFST> listaFondoFST)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    listaFondoFST = (from f in db.PensioneFondoFSTs
                                     join fdg in db.PensioneFondoDatiGenericis on f.IdFondo equals fdg.Id
                                     where fdg.IdPensione == idPensione
                                     select f).ToList<PensioneFondoFST>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetFondoFSTByIdRecordFondo(Int64 idRecordFondo, out PensioneFondoFST fondoFST)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    fondoFST = (from f in db.PensioneFondoFSTs
                                where f.IdRecordFondo == idRecordFondo
                                select f).SingleOrDefault<PensioneFondoFST>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaFondoFSTByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioneFondoFST(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioneFondoFST");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaFondoFSTByIdRecordFondo(long idRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioneFondoFSTRecordFondo(idRecordFondo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioneFondoFSTRecordFondo");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaFondoFST(PensioneFondoFST fondoFST)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPensioneFondoFST(fondoFST.IdFondo, fondoFST.RequisitiAnte247, fondoFST.TrimesteRequisiti, fondoFST.AnzianitaAnni,
                    fondoFST.CausaCessazione, fondoFST.PagamentoIndennitaIntegrativaSpeciale, fondoFST.IndennitaIntegrativaSpecialeConglobata,
                    fondoFST.TrediciMensilita, fondoFST.DecorrenzaCalcolo, fondoFST.TitolareAltraPensione, fondoFST.PensioneAnnuaLorda, fondoFST.ServizioUtileDirittoAA, fondoFST.ServizioUtileDirittoMM,
                    fondoFST.ServizioUtileDirittoGG, fondoFST.PrivilegiataSuperinvaliditaIndennita, fondoFST.AssegnoIntegrativo, fondoFST.IntegrazioneIndennitaAssistenza,
                    fondoFST.IndennitaAccompagnamentoAggiuntiva, fondoFST.CumuloInfermita, fondoFST.Categoria2aInfermita, fondoFST.AssegnoCura, fondoFST.IndennitaSpecialeAnnua, fondoFST.DecorrenzaEconomica,
                    fondoFST.AnnoRequisiti, fondoFST.DirittoIndennitaIntegrativaSpeciale, fondoFST.IntegrazioneMinimo, fondoFST.RiduzioneL537, fondoFST.IISAbbattimentoAnni,
                    fondoFST.VVUtiliDiritto, fondoFST.VVUtiliMisura, fondoFST.PensioneAnnuaLorda707, fondoFST.CoefficienteTrasformazione, fondoFST.PensioneAnnuaLorda214, fondoFST.IndennitaIntegrativaSpecialeLorda);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensioneFondoFST");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaFondoFSTRecordFondo(PensioneFondoFST fondoFST)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPensioneFondoFSTRecordFondo(fondoFST.IdFondo, fondoFST.IdRecordFondo, fondoFST.RequisitiAnte247, fondoFST.TrimesteRequisiti, fondoFST.AnzianitaAnni,
                    fondoFST.CausaCessazione, fondoFST.PagamentoIndennitaIntegrativaSpeciale, fondoFST.IndennitaIntegrativaSpecialeConglobata,
                    fondoFST.TrediciMensilita, fondoFST.DecorrenzaCalcolo, fondoFST.TitolareAltraPensione, fondoFST.PensioneAnnuaLorda, fondoFST.ServizioUtileDirittoAA, fondoFST.ServizioUtileDirittoMM,
                    fondoFST.ServizioUtileDirittoGG, fondoFST.PrivilegiataSuperinvaliditaIndennita, fondoFST.AssegnoIntegrativo, fondoFST.IntegrazioneIndennitaAssistenza,
                    fondoFST.IndennitaAccompagnamentoAggiuntiva, fondoFST.CumuloInfermita, fondoFST.Categoria2aInfermita, fondoFST.AssegnoCura, fondoFST.IndennitaSpecialeAnnua, fondoFST.DecorrenzaEconomica,
                    fondoFST.AnnoRequisiti, fondoFST.DirittoIndennitaIntegrativaSpeciale, fondoFST.IntegrazioneMinimo, fondoFST.RiduzioneL537, fondoFST.IISAbbattimentoAnni, fondoFST.Montante,
                    fondoFST.RMSSenzaLegge33670QA, fondoFST.ScadenzaBenefici, fondoFST.PALConBenefici, fondoFST.ScadenzaIllimitata, fondoFST.VVUtiliDiritto, fondoFST.VVUtiliMisura,
                    fondoFST.PensioneAnnuaLorda707, fondoFST.CoefficienteTrasformazione, fondoFST.PensioneAnnuaLorda214, fondoFST.IsPensioneAnnuaLordaDaPrelievo, fondoFST.TipologiaPensione,
                    fondoFST.IsPensioneAnnuaLorda707DaPrelievo, fondoFST.IndennitaIntegrativaSpecialeLorda, fondoFST.ServizioUtileDirittoOIAA, fondoFST.ServizioUtileDirittoOIMM, fondoFST.ServizioUtileDirittoOIGG, fondoFST.XFSFAAGO);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensioneFondoFSTRecordFondo");
                }
                db.Connection.Close();
            }
        }

        #endregion Fondo FS

        #region Fondo PT

        public static void GetFondoPTByIdPensione(Int64 idPensione, out PensioneFondoPT fondoPT)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    fondoPT = (from f in db.PensioneFondoPTs
                               join fdg in db.PensioneFondoDatiGenericis on f.IdFondo equals fdg.Id
                               where fdg.IdPensione == idPensione
                               select f).FirstOrDefault<PensioneFondoPT>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetFondoPTRecordFondoByIdPensione(Int64 idPensione, out List<PensioneFondoPT> listaFondoPT)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    listaFondoPT = (from f in db.PensioneFondoPTs
                                    join fdg in db.PensioneFondoDatiGenericis on f.IdFondo equals fdg.Id
                                    where fdg.IdPensione == idPensione
                                    select f).ToList<PensioneFondoPT>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetFondoPTByIdRecordFondo(Int64 idRecordFondo, out PensioneFondoPT fondoPT)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    fondoPT = (from f in db.PensioneFondoPTs
                               where f.IdRecordFondo == idRecordFondo
                               select f).SingleOrDefault<PensioneFondoPT>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaFondoPTByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioneFondoPT(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioneFondoPT");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaFondoPTByIdRecordFondo(long idRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioneFondoPTRecordFondo(idRecordFondo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioneFondoPTRecordFondo");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaFondoPT(PensioneFondoPT fondoPT)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPensioneFondoPT(fondoPT.IdFondo, fondoPT.FinestraMobile, fondoPT.RequisitiAnte247, fondoPT.TrimesteRequisiti, fondoPT.AnzianitaAnni,
                    fondoPT.CausaCessazione, fondoPT.PagamentoIndennitaIntegrativaSpeciale, fondoPT.IndennitaIntegrativaSpecialeConglobata,
                    fondoPT.TrediciMensilita, fondoPT.DecorrenzaCalcolo, fondoPT.SiglaCategoria, fondoPT.CodiceSede, fondoPT.Ncertificato, fondoPT.NMesiRiscattati,
                    fondoPT.NMesiTotali, fondoPT.PensioneAnnuaLorda, fondoPT.ServizioUtileDirittoAA, fondoPT.ServizioUtileDirittoMM, fondoPT.ServizioUtileDirittoGG,
                    fondoPT.PrivilegiataSuperinvaliditaIndennita, fondoPT.AssegnoIntegrativo, fondoPT.IntegrazioneIndennitaAssistenza, fondoPT.IndennitaAccompagnamentoAggiuntiva, fondoPT.CumuloInfermita,
                    fondoPT.Categoria2aInfermita, fondoPT.AssegnoCura, fondoPT.IndennitaSpecialeAnnua, fondoPT.DataInzioBeneficioArt2, fondoPT.DataFineBeneficioArt2, fondoPT.DecorrenzaEconomica,
                    fondoPT.AnnoRequisiti, fondoPT.DirittoIndennitaIntegrativaSpeciale, fondoPT.IntegrazioneMinimo, fondoPT.RiduzioneL537, fondoPT.IISAbbattimentoAnni, fondoPT.DecorrenzaSecondaria,
                    fondoPT.OnereMEF, fondoPT.RipartizioneInpdap, fondoPT.IsOnereMefFromGpUgualeSI, fondoPT.VVUtiliDiritto, fondoPT.VVUtiliMisura, fondoPT.PensioneAnnuaLorda707,
                    fondoPT.CoefficienteTrasformazione, fondoPT.PensioneAnnuaLorda214, fondoPT.RMSSenzaLegge33670QA, fondoPT.IndennitaIntegrativaSpecialeLorda, fondoPT.ServizioUtileDirittoOIAA, fondoPT.ServizioUtileDirittoOIMM, fondoPT.ServizioUtileDirittoOIGG);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensioneFondoPT");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaFondoPTRecordFondo(PensioneFondoPT fondoPT)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPensioneFondoPTRecordFondo(fondoPT.IdFondo, fondoPT.IdRecordFondo, fondoPT.FinestraMobile, fondoPT.RequisitiAnte247, fondoPT.TrimesteRequisiti, fondoPT.AnzianitaAnni,
                    fondoPT.CausaCessazione, fondoPT.PagamentoIndennitaIntegrativaSpeciale, fondoPT.IndennitaIntegrativaSpecialeConglobata,
                    fondoPT.TrediciMensilita, fondoPT.DecorrenzaCalcolo, fondoPT.SiglaCategoria, fondoPT.CodiceSede, fondoPT.Ncertificato, fondoPT.NMesiRiscattati,
                    fondoPT.NMesiTotali, fondoPT.PensioneAnnuaLorda, fondoPT.ServizioUtileDirittoAA, fondoPT.ServizioUtileDirittoMM, fondoPT.ServizioUtileDirittoGG,
                    fondoPT.PrivilegiataSuperinvaliditaIndennita, fondoPT.AssegnoIntegrativo, fondoPT.IntegrazioneIndennitaAssistenza, fondoPT.IndennitaAccompagnamentoAggiuntiva, fondoPT.CumuloInfermita,
                    fondoPT.Categoria2aInfermita, fondoPT.AssegnoCura, fondoPT.IndennitaSpecialeAnnua, fondoPT.DataInzioBeneficioArt2, fondoPT.DataFineBeneficioArt2, fondoPT.DecorrenzaEconomica,
                    fondoPT.AnnoRequisiti, fondoPT.DirittoIndennitaIntegrativaSpeciale, fondoPT.IntegrazioneMinimo, fondoPT.RiduzioneL537, fondoPT.IISAbbattimentoAnni, fondoPT.DecorrenzaSecondaria,
                    fondoPT.OnereMEF, fondoPT.RipartizioneInpdap, fondoPT.ScadenzaBenefici, fondoPT.PALConBenefici, fondoPT.Montante, fondoPT.IncrementoContrattuale, fondoPT.ScadenzaIllimitata,
                    fondoPT.IsOnereMefFromGpUgualeSI, fondoPT.VVUtiliDiritto, fondoPT.VVUtiliMisura, fondoPT.PensioneAnnuaLorda707, fondoPT.TitolareAltraPensione,
                    fondoPT.CoefficienteTrasformazione, fondoPT.PensioneAnnuaLorda214, fondoPT.RMSSenzaLegge33670QA, fondoPT.IsPensioneAnnuaLordaDaPrelievo, fondoPT.TipologiaPensione,
                    fondoPT.IsPensioneAnnuaLorda707DaPrelievo, fondoPT.IndennitaIntegrativaSpecialeLorda, fondoPT.ServizioUtileDirittoOIAA, fondoPT.ServizioUtileDirittoOIMM, fondoPT.ServizioUtileDirittoOIGG, fondoPT.XFSFAAGO);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensioneFondoPTRecordFondo");
                }
                db.Connection.Close();
            }
        }

        #endregion Fondo PT

        #region Fondo PI

        public static void GetFondoPIByIdPensione(Int64 idPensione, out PensioneFondoPI fondoPI)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    fondoPI = (from f in db.PensioneFondoPIs
                               join fdg in db.PensioneFondoDatiGenericis on f.IdFondo equals fdg.Id
                               where fdg.IdPensione == idPensione
                               select f).FirstOrDefault<PensioneFondoPI>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetFondoPIRecordFondoByIdPensione(Int64 idPensione, out List<PensioneFondoPI> fondoPI)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    fondoPI = (from f in db.PensioneFondoPIs
                               join fdg in db.PensioneFondoDatiGenericis on f.IdFondo equals fdg.Id
                               where fdg.IdPensione == idPensione
                               select f).ToList<PensioneFondoPI>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetFondoPIRecordFondoByIdPensione_pretabella(long idPensione, out object fondoPI)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope =
                    TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(
                        ConnectionFactory.GetConnection("PensioniConnectionString"));

                    fondoPI =
                        from f in db.PensioneFondoPIs
                        join fdg in db.PensioneFondoDatiGenericis
                            on f.IdFondo equals fdg.Id
                        join rf in db.RecordFondos
                            on f.IdRecordFondo equals rf.Id
                        where fdg.IdPensione == idPensione
                        select new
                        {
                            IdFondo = f.IdFondo,
                            IdRecordFondo = f.IdRecordFondo,
                            SemaforoRecord = f.SemaforoRecord,
                            DecorrenzaValiditaDati = rf.DecorrenzaValiditaDati
                        };

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
        public static void GetPensioneFondoPIByIdRecord(long idRecord, out PensioneFondoPI fondoPI)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    fondoPI = (from f in db.PensioneFondoPIs
                               where f.IdRecordFondo == idRecord
                               select f).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaFondoPIByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioneFondoPI(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioneFondoPI");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaFondoPIRecordFondo(PensioneFondoPI fondoPI)
        {
            long? idPensioneFondoPI = null;
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPensioneFondoPIRecordFondo(fondoPI.IdFondo, fondoPI.IdRecordFondo, fondoPI.TipoPensione, fondoPI.DecorrenzaPrescrizione, fondoPI.TipoLiquidazione, fondoPI.DirittoQuoteFisse, fondoPI.NonVedente,
                    fondoPI.NumeroMatricola, fondoPI.DecorrenzaPensioneEliminata, fondoPI.Qualifica, fondoPI.ImportoIIS, fondoPI.DirittoIIS, fondoPI.PensioneFacoltativaMensile, fondoPI.StipendioAnnuo,
                    fondoPI.IndennitaMedica, fondoPI.RiscattiAA, fondoPI.RiscattiMM, fondoPI.RiscattiGG, fondoPI.Requisiti247_243, fondoPI.NumeroTriSemRequisiti, fondoPI.AnnoRequisiti,
                    fondoPI.AnzianitaAnni, fondoPI.ServizioNonUtileAA, fondoPI.ServizioNonUtileMM, fondoPI.ServizioNonUtileGG, fondoPI.Livello, fondoPI.SettimaneMaggiorazione, fondoPI.SettimaneEsclusive,
                    fondoPI.SettimaneINPDAI, fondoPI.CodiceCategoria, fondoPI.Sede, fondoPI.Certificato, fondoPI.StipendioBase, fondoPI.AttCon, fondoPI.PercentualeCapitalizzazione, fondoPI.CodiceMaggiorazione, fondoPI.PensComplRiv1_95, fondoPI.RMSQuotaA,
                    fondoPI.RMSQuotaB, fondoPI.NSettimaneQuotaA, fondoPI.NSettimaneQuotaB, fondoPI.TipoRegolamento, fondoPI.IncrementoDPR346, fondoPI.Ass7_62, fondoPI.AssPers, fondoPI.Scatti, fondoPI.SedeServ, fondoPI.Fisse, fondoPI.SemaforoRecord, ref idPensioneFondoPI);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensioneFondoPI");
                }
                fondoPI.Id = idPensioneFondoPI.HasValue ? idPensioneFondoPI.Value : 0;
                db.Connection.Close();
            }
        }

        public static void SalvaFondoPIEmpty(long idFondo, string listaIdRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPensioneFondoPIEmpty(idFondo, listaIdRecordFondo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure SalvaFondoPIEmpty");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaFondoPIRecordFondo(long idRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioneFondoPIRecordFondo(idRecordFondo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure EliminaFondoPIRecordFondo");
                }
                db.Connection.Close();
            }
        }


        #endregion Fondo PI

        #region Dati AGO PI

        public static void GetDatiAgoPIById(long idDatiAgo, out DatiAgoPensioneFondoPI datiAgoPI)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope =
                    TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(
                        ConnectionFactory.GetConnection("PensioniConnectionString"));

                    datiAgoPI =
                        (from d in db.DatiAgoPensioneFondoPIs
                         where d.Id == idDatiAgo
                         select d).FirstOrDefault<DatiAgoPensioneFondoPI>();

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetListaDatiAgoPIByIdPensione(long idPensione, out List<DatiAgoPensioneFondoPI> listaDatiAgoPI)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope =
                    TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(
                        ConnectionFactory.GetConnection("PensioniConnectionString"));

                    listaDatiAgoPI =
                     (from g in db.PensioneFondoDatiGenericis
                      join ago in db.DatiAgoPensioneFondoPIs
                          on g.Id equals ago.IdFondo
                      where g.IdPensione == idPensione
                      select ago
                        ).OrderBy(x => x.Id).ToList();

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetListaSemaforiDatiAgoPIByIdFondo(long idFondo, out List<byte?> listaSemafori)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope =
                    TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(
                        ConnectionFactory.GetConnection("PensioniConnectionString"));

                    listaSemafori = db.DatiAgoPensioneFondoPIs
                        .Where(x => x.IdPensioneFondoPI == idFondo)
                        .Select(x => x.SemaforoRecord)
                        .ToList();

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaDatiAgoPIByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(
                    ConnectionFactory.GetConnection("PensioniConnectionString"));

                int result = db.DeleteDatiAgoPensioneFondoPI(idPensione);

                if (result != 0)
                {
                    throw new Exception(
                        "Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteDatiAgoPensioneFondoPI");
                }

                db.Connection.Close();
            }
        }

        public static void SalvaDatiAgoPIRecordFondo(DatiAgoPensioneFondoPI datiAgoPI)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(
                    ConnectionFactory.GetConnection("PensioniConnectionString"));

                int result = db.InsertDatiAgoPensioneFondoPIRecordFondo(
                    datiAgoPI.Id,
                    datiAgoPI.IdPensioneFondoPI,
                    datiAgoPI.IdFondo,
                    datiAgoPI.DecorrenzaDatiAgo,
                    datiAgoPI.CodiceSpecificoAgo,
                    datiAgoPI.TipoLiquidazione,
                    datiAgoPI.SospensioneAgo,
                    datiAgoPI.SettimaneVV,
                    datiAgoPI.CodiceNatura,
                    datiAgoPI.RMSQuotaA,
                    datiAgoPI.NSettimaneQuotaA,
                    datiAgoPI.NSettimaneEsclusiveQuotaA,
                    datiAgoPI.RMSQuotaB,
                    datiAgoPI.NSettimaneQuotaB,
                    datiAgoPI.NSettimaneEsclusiveQuotaB,
                    datiAgoPI.Montante,
                    datiAgoPI.RMSQuotaAOmogenea,
                    datiAgoPI.RMSQuotaBOmogenea,
                    datiAgoPI.CausaCarico,
                    datiAgoPI.SemaforoRecord,
                    datiAgoPI.DirittoQuoteFisse,
                    datiAgoPI.Ctres,
                    datiAgoPI.NSettimaneExCombattente,
                    datiAgoPI.RMSRetributiva
                );

                if (result != 0)
                {
                    throw new Exception(
                        "Si è verificato un errore durante l'esecuzione della Stored Procedure InsertDatiAgoPensioneFondoPI");
                }

                db.Connection.Close();
            }
        }

        public static void EliminaDatiAgoPIById(long idDatiAgoPI)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(
                    ConnectionFactory.GetConnection("PensioniConnectionString"));

                int result = db.DeleteDatiAgoPensioneFondoPIById(idDatiAgoPI);

                if (result != 0)
                {
                    throw new Exception(
                        "Si è verificato un errore durante l'esecuzione della Stored Procedure EliminaDatiAgoPIById");
                }

                db.Connection.Close();
            }
        }

        #endregion Dati AGO PI

        #region Dati AGO TEORICO PI

        public static void GetListaDatiAgoTeoricoPIByIdPensione(
            long idPensione,
            out List<DatiAgoTeoricoPensioneFondoPI> listaDatiAgoTeoricoPI)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope =
                    TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(
                        ConnectionFactory.GetConnection("PensioniConnectionString"));

                    listaDatiAgoTeoricoPI =
                        (from g in db.PensioneFondoDatiGenericis
                         join ago in db.DatiAgoTeoricoPensioneFondoPIs
                             on g.Id equals ago.IdFondo
                         where g.IdPensione == idPensione
                         select ago
                        ).OrderBy(x => x.Id).ToList();

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetListaDatiAgoTeoricoPIByIdPensione_old(
            long idPensione,
            out List<DatiAgoTeoricoPensioneFondoPI> listaDatiAgoTeoricoPI)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope =
                    TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(
                        ConnectionFactory.GetConnection("PensioniConnectionString"));

                    listaDatiAgoTeoricoPI =
                        (from g in db.PensioneFondoDatiGenericis
                         join f in db.PensioneFondoPIs
                             on g.Id equals f.IdFondo
                         join ago in db.DatiAgoTeoricoPensioneFondoPIs
                             on f.Id equals ago.IdPensioneFondoPI
                         where g.IdPensione == idPensione
                         select ago
                        ).OrderBy(x => x.Id).ToList();

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }


        public static void EliminaDatiAgoTeoricoPIByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(
                    ConnectionFactory.GetConnection("PensioniConnectionString"));

                int result = db.DeleteDatiAgoTeoricoPensioneFondoPI(idPensione);

                if (result != 0)
                {
                    throw new Exception(
                        "Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteDatiAgoTeoricoPensioneFondoPI");
                }

                db.Connection.Close();
            }
        }

        public static void SalvaDatiAgoTeoricoPI(DatiAgoTeoricoPensioneFondoPI datiAgoTeoricoPI)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(
                    ConnectionFactory.GetConnection("PensioniConnectionString"));

                int result = db.InsertDatiAgoTeoricoPensioneFondoPI(
                    datiAgoTeoricoPI.Id,
                    datiAgoTeoricoPI.IdPensioneFondoPI,
                    datiAgoTeoricoPI.DecorrenzaDatiAgoTeorico,
                    datiAgoTeoricoPI.TipoLiquidazione,
                    datiAgoTeoricoPI.SospensioneAGOTeorica,

                    datiAgoTeoricoPI.RMSQuotaA,
                    datiAgoTeoricoPI.NSettimaneTotaliQuotaA,
                    datiAgoTeoricoPI.NSettimaneEsclusiveQuotaA,

                    datiAgoTeoricoPI.RMSQuotaB,
                    datiAgoTeoricoPI.NSettimaneTotaliQuotaB,
                    datiAgoTeoricoPI.NSettimaneEsclusiveQuotaB,

                    datiAgoTeoricoPI.RMSOmogeneaQuotaA,
                    datiAgoTeoricoPI.RMSOmogeneaQuotaB
                );

                if (result != 0)
                {
                    throw new Exception(
                        "Si è verificato un errore durante l'esecuzione della Stored Procedure InsertDatiAgoTeoricoPensioneFondoPI");
                }

                db.Connection.Close();
            }
        }

        public static void SalvaDatiAgoTeoricoPIRecordFondo(DatiAgoTeoricoPensioneFondoPI datiAgoTeoricoPI)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(
                    ConnectionFactory.GetConnection("PensioniConnectionString"));

                int result = db.InsertDatiAgoTeoricoPensioneFondoPIRecordFondo(
                    datiAgoTeoricoPI.Id,
                    datiAgoTeoricoPI.IdPensioneFondoPI,
                    datiAgoTeoricoPI.IdFondo,
                    datiAgoTeoricoPI.DecorrenzaDatiAgoTeorico,
                    datiAgoTeoricoPI.TipoLiquidazione,
                    datiAgoTeoricoPI.SospensioneAGOTeorica,

                    datiAgoTeoricoPI.RMSQuotaA,
                    datiAgoTeoricoPI.NSettimaneTotaliQuotaA,
                    datiAgoTeoricoPI.NSettimaneEsclusiveQuotaA,

                    datiAgoTeoricoPI.RMSQuotaB,
                    datiAgoTeoricoPI.NSettimaneTotaliQuotaB,
                    datiAgoTeoricoPI.NSettimaneEsclusiveQuotaB,

                    datiAgoTeoricoPI.RMSOmogeneaQuotaA,
                    datiAgoTeoricoPI.RMSOmogeneaQuotaB
                );

                if (result != 0)
                {
                    throw new Exception(
                        "Si è verificato un errore durante l'esecuzione della Stored Procedure InsertDatiAgoTeoricoPensioneFondoPI");
                }

                db.Connection.Close();
            }
        }

        #endregion Dati AGO TEORICO PI


        #region Fondo GAS

        public static void GetFondoGASByIdPensione(Int64 idPensione, out PensioneFondoGA fondoGAS)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    fondoGAS = (from f in db.PensioneFondoGAs
                                join fdg in db.PensioneFondoDatiGenericis on f.IdFondo equals fdg.Id
                                where fdg.IdPensione == idPensione
                                select f).SingleOrDefault<PensioneFondoGA>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaFondoGASByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioneFondoGAS(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioneFondoGAS");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaFondoGAS(PensioneFondoGA fondoGAS)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPensioneFondoGAS(fondoGAS.IdFondo, fondoGAS.DecorrenzaOriginariaPensione, fondoGAS.EtaMaturazioneRequisiti, fondoGAS.DecorrenzaDatiAgo,
                    fondoGAS.CodiceTipoLiquidazione, fondoGAS.SettimaneAnzianitaEsclusiva, fondoGAS.ImportoContributiEsclusivi, fondoGAS.ContribuzioneEsclusiva, fondoGAS.ContributiTotaliSupplementoDPR143271,
                    fondoGAS.ContribuzioneEsclusivaDPR143271, fondoGAS.MesiUtiliIndennitaAggiuntiva, fondoGAS.MesiNonUtiliIndennitaAggiuntiva, fondoGAS.ServizioUtileIndennitaAggiuntiva,
                    fondoGAS.Retribuzione, fondoGAS.Importo, fondoGAS.CodicePensioneRidotta, fondoGAS.Conguaglio, fondoGAS.DecorrenzaValiditaDati,
                    fondoGAS.MesiAnte46, fondoGAS.AnzianitaUtileDal46, fondoGAS.CodiceDimissioni, fondoGAS.PercentualeRiduzione, fondoGAS.DecorrenzaTeorica, fondoGAS.Ditta,
                    fondoGAS.Convenzione, fondoGAS.Requisiti247_243, fondoGAS.NumeroTriSemRequisiti, fondoGAS.AnnoRequisiti, fondoGAS.AnzianitaAnni,
                    fondoGAS.CCTotaliArt14, fondoGAS.DecDPCM, fondoGAS.RMSArt14, fondoGAS.RMSSent72, fondoGAS.CCTotaliArt11, fondoGAS.CCEsclusivaArt11, fondoGAS.SospensioneAGO, fondoGAS.AnniDifferimento,
                    fondoGAS.CodiceSpecificoAgo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensioneFondoGAS");
                }
                db.Connection.Close();
            }
        }

        #endregion Fondo GAS

        #region Fondo CL
        public static void GetFondoCLByIdPensione(Int64 idPensione, out PensioneFondoCL fondoCL)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    fondoCL = (from f in db.PensioneFondoCLs
                               join fdg in db.PensioneFondoDatiGenericis on f.IdFondo equals fdg.Id
                               where fdg.IdPensione == idPensione
                               select f).SingleOrDefault<PensioneFondoCL>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaFondoCLByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioneFondoCL(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioneFondoCL");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaFondoCL(PensioneFondoCL fondoCL)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPensioneFondoCL(fondoCL.IdFondo, fondoCL.DataPerfezionamentoRequisiti, fondoCL.CodicePensioneSenzaRequisiti, fondoCL.AnniDifferimento,
                    fondoCL.EtaPerfezionamentoRequisiti, fondoCL.ImportoAltraPensione, fondoCL.ContrProvv);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensioneFondoCL");
                }
                db.Connection.Close();
            }
        }
        #endregion Fondo CL

        #region Fondo DZ

        public static void GetFondoDZByIdPensione(Int64 idPensione, out PensioneFondoDZ fondoDZ)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    fondoDZ = (from f in db.PensioneFondoDZs
                               join fdg in db.PensioneFondoDatiGenericis on f.IdFondo equals fdg.Id
                               where fdg.IdPensione == idPensione
                               select f).FirstOrDefault<PensioneFondoDZ>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetFondoDZRecordFondoByIdPensione(Int64 idPensione, out List<PensioneFondoDZ> listaFondoDZ)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    listaFondoDZ = (from f in db.PensioneFondoDZs
                                    join fdg in db.PensioneFondoDatiGenericis on f.IdFondo equals fdg.Id
                                    where fdg.IdPensione == idPensione
                                    select f).ToList<PensioneFondoDZ>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetFondoDZByIdRecordFondo(Int64 idRecordFondo, out PensioneFondoDZ fondoDZ)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    fondoDZ = (from f in db.PensioneFondoDZs
                               where f.IdRecordFondo == idRecordFondo
                               select f).SingleOrDefault<PensioneFondoDZ>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaFondoDZByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioneFondoDZ(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioneFondoCL");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaFondoDZByIdRecordFondo(long idRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioneFondoDZRecordFondo(idRecordFondo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioneFondoDZRecordFondo");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaFondoDZ(PensioneFondoDZ fondoDZ)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPensioneFondoDZ(fondoDZ.IdFondo, fondoDZ.RiscattiAA, fondoDZ.RiscattiMM, fondoDZ.CodiceCaroPane,
                    fondoDZ.CodiceBenefici, fondoDZ.CodiceDZ, fondoDZ.MaggiorazionePensionePrivilegiataAA, fondoDZ.MaggiorazionePensionePrivilegiataMM,
                    fondoDZ.CodiceEsodo, fondoDZ.MaggiorazioneAnzianitaEsodoAA, fondoDZ.MaggiorazioneAnzianitaEsodoMM,
                    fondoDZ.RetribuzioneAlNettoBeneficiEsodo, fondoDZ.DataCessazioneServizio, fondoDZ.ClasseAnte50,
                    fondoDZ.PercentualeLiquidazionePensione, fondoDZ.PensioneBaseAnnua, fondoDZ.Ditta, fondoDZ.Sospensione, fondoDZ.Requisiti247_243, fondoDZ.NumeroTriSemRequisiti,
                    fondoDZ.AnnoRequisiti, fondoDZ.AnzianitaAnni, fondoDZ.RaggiuntoRequisiti311297);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensioneFondoDZ");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaFondoDZRecordFondo(PensioneFondoDZ fondoDZ)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPensioneFondoDZRecordFondo(fondoDZ.IdFondo, fondoDZ.IdRecordFondo, fondoDZ.RiscattiAA, fondoDZ.RiscattiMM, fondoDZ.CodiceCaroPane, fondoDZ.CodiceBenefici, fondoDZ.CodiceDZ, fondoDZ.MaggiorazionePensionePrivilegiataAA, fondoDZ.MaggiorazionePensionePrivilegiataMM, fondoDZ.CodiceEsodo,
                fondoDZ.MaggiorazioneAnzianitaEsodoAA, fondoDZ.MaggiorazioneAnzianitaEsodoMM, fondoDZ.RetribuzioneAlNettoBeneficiEsodo, fondoDZ.DataCessazioneServizio, fondoDZ.ClasseAnte50, fondoDZ.PercentualeLiquidazionePensione,
                fondoDZ.PensioneBaseAnnua, fondoDZ.Ditta, fondoDZ.Sospensione, fondoDZ.Requisiti247_243, fondoDZ.NumeroTriSemRequisiti, fondoDZ.AnnoRequisiti, fondoDZ.AnzianitaAnni, fondoDZ.RaggiuntoRequisiti311297);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensioneFondoDZRecordFondo");
                }
                db.Connection.Close();
            }
        }

        #endregion Fondo DZ

        #region Fondo ES

        public static void GetFondoESByIdPensione(Int64 idPensione, out PensioneFondoES fondoES)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    fondoES = (from f in db.PensioneFondoES
                               join fdg in db.PensioneFondoDatiGenericis on f.IdFondo equals fdg.Id
                               where fdg.IdPensione == idPensione
                               select f).SingleOrDefault<PensioneFondoES>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaFondoESByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioneFondoES(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioneFondoES");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaFondoES(PensioneFondoES fondoES)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPensioneFondoES(fondoES.IdFondo, fondoES.TipoPensione, fondoES.DecorrenzaRequisitiAnzianita, fondoES.EtaRequisiti, fondoES.Decorrenza, fondoES.TipoLiquidazione,
                    fondoES.ContributiDifferimentoQuota, fondoES.NSettimaneLegge37758Art24, fondoES.NSettimaneLegge37758Art57, fondoES.ImportoContributiLegge37758Art24, fondoES.ImportoContributiLegge37758Art57,
                    fondoES.ImportoContributiLegge143271Art14, fondoES.BaseAltraPensione, fondoES.CategoriaAltraPensione, fondoES.NSettimaneSenzaLegge33670Art24QuotaA, fondoES.NSettimaneSenzaLegge33670Art57QuotaA,
                    fondoES.ContributiTotaliSenzaLegge33670, fondoES.ContributiSupplementoLegge143271, fondoES.ContributiSupplementoAgo, fondoES.ContributiSupplementoFondo,
                    fondoES.CodiceAzienda, fondoES.MesiRiscatti, fondoES.UnaTantum6901, fondoES.PensioneFondoAl67, fondoES.DecorrenzaArticolo24, fondoES.ContributiLegge37758Art24, fondoES.DecorrenzaLegge37758Art57Pre67Periodo1,
                    fondoES.ContributiLegge37758Art57Periodo1, fondoES.DecorrenzaLegge37758Art57Pre67Periodo2, fondoES.ContributiLegge37758Art57Periodo2, fondoES.DecorrenzaLegge37758Art57Pre67Periodo3,
                    fondoES.ContributiLegge37758Art57Periodo3, fondoES.ImportoInPagamentoPre67, fondoES.CodicePensioneInPagamentoPre67, fondoES.DecorrenzaDati, fondoES.CodiceOnOff, fondoES.ClassePensioneAnte50,
                    fondoES.MMServizioUtile, fondoES.Retribuzione, fondoES.MMServizioUtile2, fondoES.Retribuzione2, fondoES.MMServizioUtile3, fondoES.Retribuzione3, fondoES.MMServizioUtile4, fondoES.Retribuzione4,
                    fondoES.MaggiorazioneInvalidita, fondoES.AnnoUtile, fondoES.Articolo58, fondoES.Articolo59, fondoES.CodiciRetributivi, fondoES.CodiceEsattoria, fondoES.CodiceDz, fondoES.Optanti,
                    fondoES.MaggiorazionePrivilegiata, fondoES.Promiscui, fondoES.Saltuari, fondoES.IntegrazioneArticolo11, fondoES.AnniDifferimento, fondoES.ConvenzioneInternazionale, fondoES.AnniRiscatti,
                    fondoES.EtaMaturazioneRequisiti, fondoES.SettimaneArt24QA, fondoES.SettimaneArt24QB, fondoES.Sospensione, fondoES.CodiceSpecificoAgo, fondoES.DecorrenzaTeorica, fondoES.CodiceTipoLiquidazione,
                    fondoES.Requisiti247_243, fondoES.NumeroTriSemRequisiti, fondoES.AnnoRequisiti, fondoES.AnzianitaAnni, fondoES.RmsDPCM, fondoES.RMSSent72, fondoES.DecDPCM, fondoES.CCArt14SenzaLegge33670, fondoES.NSettimaneAnzianitaTotaliSenzaLegge33670, fondoES.RMSSenzaLegge33670QA, fondoES.RMSSenzaLegge33670QB);


                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensioneFondoES");
                }
                db.Connection.Close();
            }
        }
        #endregion Fondo ES

        #region Fondo PM
        public static void GetFondoPMByIdPensione(Int64 idPensione, out PensioneFondoPM fondoPM)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    fondoPM = (from f in db.PensioneFondoPMs
                               join fdg in db.PensioneFondoDatiGenericis on f.IdFondo equals fdg.Id
                               where fdg.IdPensione == idPensione
                               select f).FirstOrDefault<PensioneFondoPM>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetFondoPMRecordFondoByIdPensione(Int64 idPensione, out List<PensioneFondoPM> listaFondoPM)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    listaFondoPM = (from f in db.PensioneFondoPMs
                                    join fdg in db.PensioneFondoDatiGenericis on f.IdFondo equals fdg.Id
                                    where fdg.IdPensione == idPensione
                                    select f).ToList<PensioneFondoPM>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }


        public static void EliminaFondoPMByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioneFondopm(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioneFondoPM");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaFondoPM(PensioneFondoPM fondoPM)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPensioneFondoPM(fondoPM.IdFondo, fondoPM.CodiceTipo, fondoPM.DecorrenzaOriginariaAgo, fondoPM.EtaMaturazioneRequisiti, fondoPM.DecorrenzaAgo,
                    fondoPM.CodiceTipoLiquidazione, fondoPM.AnzianitaEsclusiva, fondoPM.ContributiEsclusiviArt11, fondoPM.ContribuzioneEsclusiva, fondoPM.ContributiTotaliLegge143271,
                    fondoPM.ContributiEsclusiviLegge143271, fondoPM.DirittoAgo, fondoPM.AnnoUtileUltimoDecennio, fondoPM.ConvenzioneInternazionale, fondoPM.Decorrenza, fondoPM.MesiNavigazioneEffettiva,
                    fondoPM.GiorniNavigazioneEffettiva, fondoPM.MesiRadePilotaggio, fondoPM.GiorniRadePilotaggio, fondoPM.MesiTBCDS, fondoPM.GiorniTBCDS, fondoPM.MesiMalattia, fondoPM.GiorniMalattia,
                    fondoPM.MesiNavigazioneEE, fondoPM.GiorniNavigazioneEE, fondoPM.MesiAltriServizi, fondoPM.GiorniAltriServizi, fondoPM.MesiNavigazioneMilitare, fondoPM.GiorniNavigazioneMilitare,
                    fondoPM.MesiDoppioMilitare, fondoPM.GiorniDoppioMilitare, fondoPM.MesiDoppioMercantile, fondoPM.GiorniDoppioMercantile, fondoPM.MesiServizioMilitareATerra,
                    fondoPM.GiorniServizioMilitareATerra, fondoPM.CodiceDifferimentoPrivilegiato, fondoPM.PeriodoDifferimento1, fondoPM.PeriodoDifferimento2, fondoPM.TelegrafistaServizioMacchinaAA,
                    fondoPM.TelegrafistaServizioMacchinaMM, fondoPM.TipoLiquidazione, fondoPM.GestioneSpeciale1Supplemento, fondoPM.GestioneSpeciale2Supplemento, fondoPM.RMSDPCM161289, fondoPM.RMS7290,
                    fondoPM.CL413, fondoPM.AttivitaSvolta2, fondoPM.NumeroTriSemRequisiti, fondoPM.AnnoRequisiti, fondoPM.AnzianitaAnni, fondoPM.CorresponsioneIP, fondoPM.MesiRiparametrazione, fondoPM.AnniRiparametrazione,
                    fondoPM.CodiceEsclusione, fondoPM.Stato, fondoPM.Rendimento, fondoPM.CodiceDPCDC);

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensioneFondoES");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaFondoPMRecordFondo(PensioneFondoPM fondoPM)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPensioneFondoPMRecordFondo(fondoPM.IdFondo, fondoPM.IdRecordFondo, fondoPM.CodiceTipo, fondoPM.DecorrenzaOriginariaAgo, fondoPM.EtaMaturazioneRequisiti, fondoPM.DecorrenzaAgo,
                    fondoPM.CodiceTipoLiquidazione, fondoPM.AnzianitaEsclusiva, fondoPM.ContributiEsclusiviArt11, fondoPM.ContribuzioneEsclusiva, fondoPM.ContributiTotaliLegge143271,
                    fondoPM.ContributiEsclusiviLegge143271, fondoPM.DirittoAgo, fondoPM.AnnoUtileUltimoDecennio, fondoPM.ConvenzioneInternazionale, fondoPM.Decorrenza, fondoPM.MesiNavigazioneEffettiva,
                    fondoPM.GiorniNavigazioneEffettiva, fondoPM.MesiRadePilotaggio, fondoPM.GiorniRadePilotaggio, fondoPM.MesiTBCDS, fondoPM.GiorniTBCDS, fondoPM.MesiMalattia, fondoPM.GiorniMalattia,
                    fondoPM.MesiNavigazioneEE, fondoPM.GiorniNavigazioneEE, fondoPM.MesiAltriServizi, fondoPM.GiorniAltriServizi, fondoPM.MesiNavigazioneMilitare, fondoPM.GiorniNavigazioneMilitare,
                    fondoPM.MesiDoppioMilitare, fondoPM.GiorniDoppioMilitare, fondoPM.MesiDoppioMercantile, fondoPM.GiorniDoppioMercantile, fondoPM.MesiServizioMilitareATerra,
                    fondoPM.GiorniServizioMilitareATerra, fondoPM.CodiceDifferimentoPrivilegiato, fondoPM.PeriodoDifferimento1, fondoPM.PeriodoDifferimento2, fondoPM.TelegrafistaServizioMacchinaAA,
                    fondoPM.TelegrafistaServizioMacchinaMM, fondoPM.TipoLiquidazione, fondoPM.GestioneSpeciale1Supplemento, fondoPM.GestioneSpeciale2Supplemento, fondoPM.RMSDPCM161289, fondoPM.RMS7290,
                    fondoPM.CL413, fondoPM.AttivitaSvolta2, fondoPM.NumeroTriSemRequisiti, fondoPM.AnnoRequisiti, fondoPM.AnzianitaAnni, fondoPM.CorresponsioneIP, fondoPM.MesiRiparametrazione, fondoPM.AnniRiparametrazione,
                    fondoPM.CodiceEsclusione, fondoPM.Stato, fondoPM.Rendimento, fondoPM.CodiceDPCDC);

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensioneFondoES");
                }
                db.Connection.Close();
            }
        }
        #endregion Fondo PM

        #region Dati AGO PM 

        public static void GetListaDatiAgoPMByIdPensione(long idPensione, out List<DatiAgoPensioneFondoPM> listaDatiAgoPM)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope =
                    TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(
                        ConnectionFactory.GetConnection("PensioniConnectionString"));

                    listaDatiAgoPM =
                     (from g in db.PensioneFondoDatiGenericis
                      join ago in db.DatiAgoPensioneFondoPMs
                          on g.Id equals ago.IdFondo
                      where g.IdPensione == idPensione
                      select ago
                        ).OrderBy(x => x.Id).ToList();

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaDatiAgoPMRecordFondo(DatiAgoPensioneFondoPM datiAgoPM)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(
                    ConnectionFactory.GetConnection("PensioniConnectionString"));

                int result = db.InsertDatiAgoPensioneFondoPMRecordFondo(
                    datiAgoPM.Id,
                    datiAgoPM.IdFondo,
                     datiAgoPM.TipoLiquidazione,
    datiAgoPM.DecorrenzaContributiva,
    datiAgoPM.SospensionePensione,
    datiAgoPM.DecorrenzaReversibileAgo,
    datiAgoPM.RMSQuotaA,
    datiAgoPM.NSettimaneQuotaA,
    datiAgoPM.NsettimaneEsclusiveQuotaA,
    datiAgoPM.NSettimaneAnzianitaVV,
    datiAgoPM.ImportoContrLegge3351995,
    datiAgoPM.ImportoContrLegge3771958art24,
    datiAgoPM.ImportoContrSupLegge14321971art14,
    datiAgoPM.BaseAltraPensione,
    datiAgoPM.ImportoDPR4881968art11,
    datiAgoPM.ImportoContEsclusiviSupDPR4881968art11,
    datiAgoPM.YPMANNIR,
    datiAgoPM.EtaMaturazioneRequisiti,
    datiAgoPM.DecDPCM16121989art2,
    datiAgoPM.RMSDPCM,
    datiAgoPM.CodiceSpecificoLiquidazioneAgo,
    datiAgoPM.RMSQuotaB,
    datiAgoPM.NSettimaneQuotaB,
    datiAgoPM.NSettimaneEsclusiveQuotaB,
    datiAgoPM.YPM503ET,
    datiAgoPM.YPM503AS,
    datiAgoPM.YPMTPCOD,
    datiAgoPM.YPMDECSS,
    datiAgoPM.YPMSOSSS,
    datiAgoPM.YPMAUTON,
    datiAgoPM.MontanteLegge3351995,
    datiAgoPM.NSettimaneContributive,
    datiAgoPM.ImportoContrQuotaD,
    datiAgoPM.MontanteQuotaD,
    datiAgoPM.NSettimaneContributiveQuotaD,
    datiAgoPM.NSettimane707A,
    datiAgoPM.NSettimane707B,
    datiAgoPM.YPMCALC707,
    datiAgoPM.YPMPROGR

                );

                if (result != 0)
                {
                    throw new Exception(
                        "Si è verificato un errore durante l'esecuzione della Stored Procedure InsertDatiAgoPensioneFondoPI");
                }

                db.Connection.Close();
            }
        }


        public static void EliminaDatiAgoPMByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(
                    ConnectionFactory.GetConnection("PensioniConnectionString"));

                int result = db.DeleteDatiAgoPensioneFondoPM(idPensione);

                if (result != 0)
                {
                    throw new Exception(
                        "Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteDatiAgoPensioneFondoPI");
                }

                db.Connection.Close();
            }
        }


        #endregion Dati Ago PM
    }
}
