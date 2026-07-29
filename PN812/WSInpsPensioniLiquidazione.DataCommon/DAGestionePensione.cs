using INPS.DNA.Data;
using INPS.DNA.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestionePensione
    {
        public static void GetPensioneByNumeroDomandaAndProg(Int64 numeroDomanda, Expression<Func<Pensione, bool>> whereCondition, out Pensione pensione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    var query = (from p in db.Pensiones where p.NDomus == numeroDomanda select p);
                    pensione = query.Where(whereCondition).SingleOrDefault<Pensione>();

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetPensioniByNumeroDomanda(Int64 numeroDomanda, out List<Pensione> elencoPensioni)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoPensioni = (from p in db.Pensiones where p.NDomus == numeroDomanda select p).ToList<Pensione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetPensioneByIdPensione(Int64 IdPensione, out Pensione pensione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    pensione = (from p in db.Pensiones where p.Id == IdPensione select p).SingleOrDefault<Pensione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetIdPensioneByNumeroDomandaAndProg(Int64 numeroDomanda, Expression<Func<Pensione, bool>> whereCondition, out long idPensione)
        {
            try
            {
                using (new MethodExecutionTracer())
                {
                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                    {
                        PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                        var query = (from p in db.Pensiones where p.NDomus == numeroDomanda select p);
                        idPensione = query.Where(whereCondition).Select(p => p.Id).SingleOrDefault<long>();

                        db.Connection.Close();
                        transactionScope.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public static void GetPensioniByCodiceFiscale(string codiceFiscale, out List<Pensione> pensioni)
        {
            using (new MethodExecutionTracer())
            {
                pensioni = null;
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    var elencoPensioni = (from p in db.Pensiones
                                          join t in db.Titolares on p.Id equals t.IdPensione
                                          join a in db.Anagraficas on t.IdAnagrafica equals a.Id
                                          where a.CodiceFiscale == codiceFiscale
                                          select p);
                    foreach (Pensione p in elencoPensioni)
                    {
                        if (pensioni == null)
                            pensioni = new List<Pensione>();
                        pensioni.Add(p);
                    }
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetSindacatoByIdPensione(Int64 idPensione, out Sindacato sindacato)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    sindacato = (from s in db.Sindacatos
                                 where s.IdPensione == idPensione
                                 select s).SingleOrDefault<Sindacato>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetPatronatoByIdPensione(Int64 idPensione, out Patronato patronato)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    patronato = (from p in db.Patronatos
                                 where p.IdPensione == idPensione
                                 select p).SingleOrDefault<Patronato>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetEliminazioneByIdPensione(Int64 idPensione, out Eliminazione eliminazione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    eliminazione = (from e in db.Eliminaziones
                                    where e.IdPensione == idPensione
                                    select e).SingleOrDefault<Eliminazione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetTitolareByIdPensione(Int64 idPensione, out Titolare titolare)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    titolare = (from t in db.Titolares
                                where t.IdPensione == idPensione
                                select t).SingleOrDefault<Titolare>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetPensioneByChiavePensione(string siglaCategoria, short sede, int certificato, out List<Pensione> elencoPensioni)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoPensioni = (from p in db.Pensiones
                                      where p.SiglaCategoria == siglaCategoria &&
                                      ((p.CodiceSedeDestinazione != null && p.CodiceSedeDestinazione == sede) || (p.CodiceSedeDestinazione == null && p.CodiceSede == sede)) &&
                                p.NCertificato == certificato
                                      select p).ToList<Pensione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetPensioneByChiavePensionePerFondo(string siglaCategoria, short sede, int certificato, out List<Pensione> elencoPensioni)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoPensioni = (from p in db.Pensiones
                                      where p.SiglaCategoria.Substring(1) == siglaCategoria &&
                                      ((p.CodiceSedeDestinazione != null && p.CodiceSedeDestinazione == sede) || (p.CodiceSedeDestinazione == null && p.CodiceSede == sede)) &&
                                p.NCertificato == certificato
                                      select p).ToList<Pensione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaPensione(Pensione pensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                long? idPensione = null;

                int result = db.InsertPensione(pensione.NDomus, pensione.SiglaCategoria, pensione.CodiceSede, pensione.CodiceSedeDestinazione, pensione.CentroOperativoDestinazione, pensione.NCertificato,
                    pensione.NCertificatoProvvisorio, pensione.DataPresentazioneDomanda, pensione.TipoElaborazione, pensione.DecorrenzaOriginaria, pensione.DecorrenzaOriginariaPrima,
                    pensione.NaturaPensione, pensione.TipoCalcolo, pensione.CausaCarico, pensione.CodiceArretrati, pensione.AttivitaEconomica, pensione.ProfessioneIndividuale,
                    pensione.AttivitaEconomicaFELPE, pensione.ProfessioneIndividualeFELPE,
                    pensione.DataInteressiLegali, pensione.DataCompletezza, pensione.DecorrenzaCalcoloArretratiNUTS, pensione.InizioAssicurazione, pensione.FineAssicurazione,
                    pensione.DataPerfezionamentoRequisiti, pensione.RequisitiVecchiaiaAl1294, pensione.RequisitiAl1294,
                    pensione.RequisitiAl996, pensione.DataInizioCalcolo, pensione.RequisitiLegge50392Art2, pensione.AccertamentoAutomatico,
                    pensione.AliquotaTFREsodati, pensione.DecorrenzaCalcoloArretrati, pensione.MatricolaUtenteAcquisizione, pensione.Isola, pensione.CodiceProcedura,
                    pensione.FlagInCalcolo, pensione.CentroOperativo, pensione.DataAcquisizione, pensione.DataElaborazione, pensione.FlagVerify,
                    pensione.Versione, pensione.AggancioQred, pensione.CodiceBancaEsodati, pensione.AttivitaConcorrenzialeEsodante, pensione.DataRicostituzione,
                    pensione.DataRicezionePrenotazioneCentrale, pensione.DataPrimaDomanda, pensione.StatoPensione, pensione.TrasformazioneAOI,
                    pensione.AgevolazioniLegge, pensione.ExCombattente, pensione.Gruppo, pensione.Prodotto, pensione.Tipo,
                    pensione.Gestione, pensione.Fondo, pensione.Ente, pensione.FlagUnicarpe, pensione.TipoLetturaUnicarpe, pensione.IndConvInt,
                    pensione.CodiceTipoRichiesta, pensione.Benefici, pensione.DataPerfezionamentoRequisitiUnicarpe, pensione.Maggiorazioni, pensione.Contributivo, pensione.Amianto181Unicarpe,
                    pensione.IsDatiENPALSRecuperati, pensione.NDomusPrincipale, pensione.IsCumuloAutomatica, pensione.LinkIntranet, pensione.IsRicAPEPrecoci,
                    pensione.DataTentativoCalcoloDefinitivo, pensione.LavoratorePubblico, pensione.IsRicSperimentaleDonna_DL_4_2019, pensione.IsRicAnzianitaPerLeggeBilancio2019,
                    pensione.IdTipoPLPerRIC, pensione.NumeroFigli, pensione.SceltaLavMadri, pensione.DataOpzione, pensione.DataRaggiungimentoOpzione, pensione.IsPLUnicarpe, pensione.TipoFelpe, pensione.AnnoDecorrenzaBonus, pensione.IsRichiestaBonus, pensione.IsDatiAggiuntiviFromJSON, pensione.IsTotAutomatica, pensione.DataCondizioniPerComputo, pensione.Flag5000,
                    pensione.DirittoAutonomo, pensione.IsPLInvalidita, pensione.IsRicRinnovata, pensione.IsRicExtracalcolo, pensione.TipoAutomazione, pensione.CodiceSedeGP1ALZ6, pensione.CentroOperativoGP1ALZ6, pensione.GP1AV11, pensione.Ante96ByDatiCalcolo, pensione.MaxDecDatiCalcoloAnte96, pensione.GP1AV91A, pensione.IsTentataAutomazione, pensione.Caratterizzazione, pensione.CodProPE, pensione.AnnoMonitoraggio, pensione.GP1AV91B, pensione.IsNuovoCalcolo,
                    pensione.GP1AJ11, pensione.DataEstrazioneRata, pensione.IdNota, pensione.SbloccaPannelliAnte96, pensione.FlagIndebito, pensione.GP1AJSP , pensione.CodiceSedeLavorazione,ref idPensione);


                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensione");
                }
                pensione.Id = idPensione.HasValue ? idPensione.Value : 0;
                db.Connection.Close();
            }
        }

        public static void SalvaSindacato(Sindacato sindacato)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertSindacato(sindacato.IdPensione, sindacato.CodiceSindacato, sindacato.DescrizioneSindacato,
                    sindacato.DecorrenzaSindacato, sindacato.CessazioneSindacato, sindacato.IsFromService);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertSindacato");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaPatronato(Patronato patronato)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPatronato(patronato.IdPensione, patronato.CodiceEnte, patronato.CodiceUfficio, patronato.NPratica, patronato.TipoUfficio);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPatronato");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaTitolare(Titolare titolare)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertTitolare(titolare.IdAnagrafica, titolare.IdPensione, titolare.DataMorte);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertTitolare");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaEliminazione(Eliminazione eliminazione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertEliminazione(eliminazione.IdPensione, eliminazione.CodiceMotivo, eliminazione.DecorrenzaEliminazione,
                    eliminazione.DataEvento, eliminazione.DataComunicazione, eliminazione.DataUltimaRiscossione, eliminazione.DataFineCalcoloArretrati,
                    eliminazione.CodiceRinnovo, eliminazione.CodiceTipoMovimentazione, eliminazione.DataUltimaMovimentazione, eliminazione.DataCessazioneDiritto, eliminazione.DataComunicazioneEliminazione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertEliminazione");
                }
                db.Connection.Close();
            }
        }

        /*Gestione Log Cancellazione*/
        private static LogGenerico NewLogGenerico(Int64 NumDomanda, string Message)
        { 
            LogGenerico Log = new LogGenerico();
            Log.NumDomanda = NumDomanda;
            Log.LogType = "Informativo";
            Log.MethodName = "DAGestionePensione.EliminaPensione";
            Log.Message = Message;
            Log.Parameters = string.Empty;
            Log.StackTrace = string.Empty;
            Log.Progressivo = 0;
            
            return Log;
        }

        public static void EliminaPensione(long idPensione, Int64 NumDomanda, bool scriviLog)
        {
            try
            {
                if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "PrimaFase"));
                using (new MethodExecutionTracer())
                {
                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                    {
                        PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteTitolare"));
                        db.DeleteTitolare(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteCalcoloContributivo"));
                        db.DeleteCalcoloContributivo(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteCalcoloContributivoENPALS"));
                        db.DeleteCalcoloContributivoENPALS(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteCalcoloRetributivo"));
                        db.DeleteCalcoloRetributivo(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteCalcoloRetributivoENPALS"));
                        db.DeleteCalcoloRetributivoENPALS(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteDetrazioniImposta"));
                        db.DeleteDetrazioniImposta(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteEliminazione"));
                        db.DeleteEliminazione(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteIntegrazioneArt11"));
                        db.DeleteIntegrazioneArt11(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteIstruttoria"));
                        db.DeleteIstruttoria(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteMaggiorazioniBenefici"));
                        db.DeleteMaggiorazioniBenefici(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteNuoveLiquidate"));
                        db.DeleteNuoveLiquidate(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePagamento"));
                        db.DeletePagamento(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllRichiestaRicercaDomandeANF"));
                        db.DeleteAllRichiestaRicercaDomandeANF(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllCodMaggiorazioneFamiliari"));
                        db.DeleteAllCodMaggiorazioneFamiliari(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteFamiliari"));
                        db.DeleteFamiliari(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensioneFondoGAS"));
                        db.DeletePensioneFondoGAS(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensioneFondoET"));
                        db.DeletePensioneFondoET(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensioneFondoEL"));
                        db.DeletePensioneFondoEL(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensioneFondoES"));
                        db.DeletePensioneFondoES(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensioneFondoDZ"));
                        db.DeletePensioneFondoDZ(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensioneFondoCL"));
                        db.DeletePensioneFondoCL(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensioneFondoVL"));
                        db.DeletePensioneFondoVL(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensioneFondoTT"));
                        db.DeletePensioneFondoTT(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensioneFondoFST"));
                        db.DeletePensioneFondoFST(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensioneFondoPT"));
                        db.DeletePensioneFondoPT(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensioneFondopms"));
                        db.DeletePensioneFondopms(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensioneFondopm"));
                        db.DeletePensioneFondopm(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteDatiAgoPensioneFondoPI"));
                        db.DeleteDatiAgoPensioneFondoPI(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteDatiAgoPensioneFondoPM"));
                        db.DeleteDatiAgoPensioneFondoPM(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteDatiAgoTeoricoPensioneFondoPI"));
                        db.DeleteDatiAgoTeoricoPensioneFondoPI(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensioneFondoPI"));
                        db.DeletePensioneFondoPI(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteDatiServizioUtile"));
                        db.DeleteDatiServizioUtile(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteDatiServizioUtile707"));
                        db.DeleteDatiServizioUtile707(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteDatiServizioUtileINPDAP707"));
                        db.DeleteDatiServizioUtileINPDAP707(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensioneFondoDatiGenerici"));
                        db.DeletePensioneFondoDatiGenerici(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensioniAbbinate"));
                        db.DeletePensioniAbbinate(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensioniDatiGenerici"));
                        db.DeletePensioniDatiGenerici(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllPensioniCiImportiEsteri"));
                        db.DeleteAllPensioniCiImportiEsteri(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensioniCiImportiValuta"));
                        db.DeletePensioniCiImportiValuta(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllPensioniCiPrestazioniEE"));
                        db.DeleteAllPensioniCiPrestazioniEE(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensioniINAIL"));
                        db.DeletePensioniINAIL(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteInabilita"));
                        db.DeleteInabilita(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePatronato"));
                        db.DeletePatronato(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteSindacato"));
                        db.DeleteSindacato(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePsAsInvCiv"));
                        db.DeletePsAsInvCiv(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteRedditiEstero"));
                        db.DeleteRedditiEstero(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteRedditiFamiliari"));
                        db.DeleteRedditiFamiliari(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteRedditiIntegrazioni"));
                        db.DeleteRedditiIntegrazioni(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteRedditiLavoroAutonomo"));
                        db.DeleteRedditiLavoroAutonomo(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteRedditiMaggiorazioni"));
                        db.DeleteRedditiMaggiorazioni(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteReddS24094"));
                        db.DeleteReddS24094(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteReddS49593"));
                        db.DeleteReddS49593(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteRicoveri"));
                        db.DeleteRicoveri(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteSentenze"));
                        db.DeleteSentenze(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllSupplementi"));
                        db.DeleteAllSupplementi(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllSupplementiBase"));
                        db.DeleteAllSupplementiBase(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteSupplementiENPALS"));
                        db.DeleteSupplementiENPALS(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllSupplementiRecordENPALS"));
                        db.DeleteAllSupplementiRecordENPALS(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteVittimeTerrorismo"));
                        db.DeleteVittimeTerrorismo(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteDelegato"));
                        db.DeleteDelegato(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteTutore"));
                        db.DeleteTutore(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuadroTitolare"));
                        db.DeleteQuadroTitolare(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuadroDetrazioni"));
                        db.DeleteQuadroDetrazioni(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuadroPagamento"));
                        db.DeleteQuadroPagamento(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuadroLiquidazionePensione"));
                        db.DeleteQuadroLiquidazionePensione(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuadroDelegatoTutore"));
                        db.DeleteQuadroDelegatoTutore(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuadroDatiContributivi"));
                        db.DeleteQuadroDatiContributivi(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuadroRedditi"));
                        db.DeleteQuadroRedditi(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuadroFamiliari"));
                        db.DeleteQuadroFamiliari(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuadroDanteCausa"));
                        db.DeleteQuadroDanteCausa(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuadroMaggiorazioniBenefici"));
                        db.DeleteQuadroMaggiorazioniBenefici(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuadroSupplementi"));
                        db.DeleteQuadroSupplementi(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuadroBititolarita"));
                        db.DeleteQuadroBititolarita(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuadroEliminazione"));
                        db.DeleteQuadroEliminazione(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuadroDatiNoCalcolo"));
                        db.DeleteQuadroDatiNoCalcolo(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuadroOneri"));
                        db.DeleteQuadroOneri(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuadroAventiDiritto"));
                        db.DeleteQuadroAventiDiritto(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuadroPeriodi"));
                        db.DeleteQuadroPeriodi(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuadroAltreDomandeCollegate"));
                        db.DeleteQuadroAltreDomandeCollegate(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuadroRichiestaBonus"));
                        db.DeleteQuadroRichiestaBonus(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllQuadroDatiRecordFondo"));
                        db.DeleteAllQuadroDatiRecordFondo(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuadroDatiFondo"));
                        db.DeleteQuadroDatiFondo(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensioneINPDAP"));
                        db.DeletePensioneINPDAP(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllDatiServizioUtileINPDAP"));
                        db.DeleteAllDatiServizioUtileINPDAP(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllRecordDatiFondoINPDAP"));
                        db.DeleteAllRecordDatiFondoINPDAP(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllRecordFondo"));
                        db.DeleteAllRecordFondo(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllRedditiDRedd"));
                        db.DeleteAllRedditiDRedd(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteMaggiorazioniBenefici"));
                        db.DeleteMaggiorazioniBenefici(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensioniEstereDC"));
                        db.DeletePensioniEstereDC(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllDanteCausa"));
                        db.DeleteAllDanteCausa(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllResidenzeEstero"));
                        db.DeleteAllResidenzeEstero(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllStatiCivili"));
                        db.DeleteAllStatiCivili(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteLavorazione"));
                        db.DeleteLavorazione(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteEsitoCalcolo"));
                        db.DeleteEsitoCalcolo(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteMaternitaAcna"));
                        db.DeleteMaternitaAcna(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensioniCiContributiEE"));
                        db.DeletePensioniCiContributiEE(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteRipartizioneFondi"));
                        db.DeleteRipartizioneFondi(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteDL407"));
                        db.DeleteDL407(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteStampa"));
                        db.DeleteStampa(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteOneri"));
                        db.DeleteOneri(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAltrePensioni"));
                        db.DeleteAltrePensioni(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllBeneficiParticolari"));
                        db.DeleteAllBeneficiParticolari(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteDatiControlloFelpe"));
                        db.DeleteDatiControlloFelpe(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllDatiPostDecOriginaria"));
                        db.DeleteAllDatiPostDecOriginaria(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteEnpals"));
                        db.DeleteEnpals(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePrepensionamento"));
                        db.DeletePrepensionamento(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllContribuzioneEnpals"));
                        db.DeleteAllContribuzioneEnpals(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteTrattenuteQuotePensione"));
                        db.DeleteTrattenuteQuotePensione(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuotePensione"));
                        db.DeleteQuotePensione(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuoteMiglioramentiContrattuali"));
                        db.DeleteQuoteMiglioramentiContrattuali(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteMiglioramentiContrattuali"));
                        db.DeleteMiglioramentiContrattuali(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllQuadroDatiRecordNoCalcolo"));
                        db.DeleteAllQuadroDatiRecordNoCalcolo(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllComponentiFamiliari"));
                        db.DeleteAllComponentiFamiliari(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllRecordDatiNoCalcolo"));
                        db.DeleteAllRecordDatiNoCalcolo(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteBeneficioVittimeTerrorismo"));
                        db.DeleteBeneficioVittimeTerrorismo(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteCalcoloVittimeTerrorismo"));
                        db.DeleteCalcoloVittimeTerrorismo(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteDatiStoricoGP"));
                        db.DeleteDatiStoricoGP(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllPeriodiAventiDiritto"));
                        db.DeleteAllPeriodiAventiDiritto(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllAventiDiritto"));
                        db.DeleteAllAventiDiritto(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteRipartizioneINPDAP"));
                        db.DeleteRipartizioneINPDAP(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllInteressiLegali"));
                        db.DeleteAllInteressiLegali(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteSentenzaArt4"));
                        db.DeleteSentenzaArt4(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllDetrazioniImpostaContitolare"));
                        db.DeleteAllDetrazioniImpostaContitolare(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteSupplementiCumulo"));
                        db.DeleteSupplementiCumulo(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAnniRichiestaBonus"));
                        db.DeleteAnniRichiestaBonus(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteRedditiPerIntegrazioneVirtuale"));
                        db.DeleteRedditiPerIntegrazioneVirtuale(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteQuotaFondoIntegrativo"));
                        db.DeleteQuotaFondoIntegrativo(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteCalcoloContributivoINPGI"));
                        db.DeleteCalcoloContributivoINPGI(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteCalcoloRetributivoINPGI"));
                        db.DeleteCalcoloRetributivoINPGI(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeleteAllPensioneImportiEsteriCumulo"));
                        db.DeleteAllPensioneImportiEsteriCumulo(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensioneEsteraCumuloNoStorico"));
                        db.DeletePensioneEsteraCumuloNoStorico(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "DeletePensione"));
                        db.DeletePensione(idPensione);
                        if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "Fine Cancellazione"));

                        db.Connection.Close();
                        transactionScope.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                if (scriviLog) DAGestioneLogGenerico.SalvaLogGenerico(NewLogGenerico(NumDomanda, "Exception: " + ex.Message));
                throw;
            }

        }

        public static void EliminaPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    db.DeleteTitolare(idPensione);
                    db.DeleteCalcoloContributivo(idPensione);
                    db.DeleteCalcoloContributivoENPALS(idPensione);
                    db.DeleteCalcoloRetributivo(idPensione);
                    db.DeleteCalcoloRetributivoENPALS(idPensione);
                    db.DeleteDetrazioniImposta(idPensione);
                    db.DeleteEliminazione(idPensione);
                    db.DeleteIntegrazioneArt11(idPensione);
                    db.DeleteIstruttoria(idPensione);
                    db.DeleteMaggiorazioniBenefici(idPensione);
                    db.DeleteNuoveLiquidate(idPensione);
                    db.DeletePagamento(idPensione);
                    db.DeleteAllRichiestaRicercaDomandeANF(idPensione);
                    db.DeleteAllCodMaggiorazioneFamiliari(idPensione);
                    db.DeleteFamiliari(idPensione);
                    db.DeletePensioneFondoGAS(idPensione);
                    db.DeletePensioneFondoET(idPensione);
                    db.DeletePensioneFondoEL(idPensione);
                    db.DeletePensioneFondoES(idPensione);
                    db.DeletePensioneFondoDZ(idPensione);
                    db.DeletePensioneFondoCL(idPensione);
                    db.DeletePensioneFondoVL(idPensione);
                    db.DeletePensioneFondoTT(idPensione);
                    db.DeletePensioneFondoFST(idPensione);
                    db.DeletePensioneFondoPT(idPensione);
                    db.DeletePensioneFondopms(idPensione);
                    db.DeletePensioneFondopm(idPensione);
                    db.DeleteDatiAgoPensioneFondoPI(idPensione);
                    db.DeleteDatiAgoPensioneFondoPM(idPensione);
                    db.DeleteDatiAgoTeoricoPensioneFondoPI(idPensione);
                    db.DeletePensioneFondoPI(idPensione);
                    db.DeleteDatiServizioUtile(idPensione);
                    db.DeleteDatiServizioUtile707(idPensione);
                    db.DeleteDatiServizioUtileINPDAP707(idPensione);
                    db.DeletePensioneFondoDatiGenerici(idPensione);
                    db.DeletePensioniAbbinate(idPensione);
                    db.DeletePensioniDatiGenerici(idPensione);
                    db.DeleteAllPensioniCiImportiEsteri(idPensione);
                    db.DeletePensioniCiImportiValuta(idPensione);
                    db.DeleteAllPensioniCiPrestazioniEE(idPensione);
                    db.DeletePensioniINAIL(idPensione);
                    db.DeleteInabilita(idPensione);
                    db.DeletePatronato(idPensione);
                    db.DeleteSindacato(idPensione);
                    db.DeletePsAsInvCiv(idPensione);
                    db.DeleteRedditiEstero(idPensione);
                    db.DeleteRedditiFamiliari(idPensione);
                    db.DeleteRedditiIntegrazioni(idPensione);
                    db.DeleteRedditiLavoroAutonomo(idPensione);
                    db.DeleteRedditiMaggiorazioni(idPensione);
                    db.DeleteReddS24094(idPensione);
                    db.DeleteReddS49593(idPensione);
                    db.DeleteRicoveri(idPensione);
                    db.DeleteSentenze(idPensione);
                    db.DeleteAllSupplementi(idPensione);
                    db.DeleteAllSupplementiBase(idPensione);
                    db.DeleteSupplementiENPALS(idPensione);
                    db.DeleteAllSupplementiRecordENPALS(idPensione);
                    db.DeleteVittimeTerrorismo(idPensione);
                    db.DeleteDelegato(idPensione);
                    db.DeleteTutore(idPensione);
                    db.DeleteQuadroTitolare(idPensione);
                    db.DeleteQuadroDetrazioni(idPensione);
                    db.DeleteQuadroPagamento(idPensione);
                    db.DeleteQuadroLiquidazionePensione(idPensione);
                    db.DeleteQuadroDelegatoTutore(idPensione);
                    db.DeleteQuadroDatiContributivi(idPensione);
                    db.DeleteQuadroRedditi(idPensione);
                    db.DeleteQuadroFamiliari(idPensione);
                    db.DeleteQuadroDanteCausa(idPensione);
                    db.DeleteQuadroMaggiorazioniBenefici(idPensione);
                    db.DeleteQuadroSupplementi(idPensione);
                    db.DeleteQuadroBititolarita(idPensione);
                    db.DeleteQuadroEliminazione(idPensione);
                    db.DeleteQuadroDatiNoCalcolo(idPensione);
                    db.DeleteQuadroOneri(idPensione);
                    db.DeleteQuadroAventiDiritto(idPensione);
                    db.DeleteQuadroPeriodi(idPensione);
                    db.DeleteQuadroAltreDomandeCollegate(idPensione);
                    db.DeleteQuadroRichiestaBonus(idPensione);
                    db.DeleteAllQuadroDatiRecordFondo(idPensione);
                    db.DeleteQuadroDatiFondo(idPensione);
                    db.DeletePensioneINPDAP(idPensione);
                    db.DeleteAllDatiServizioUtileINPDAP(idPensione);
                    db.DeleteAllRecordDatiFondoINPDAP(idPensione);
                    db.DeleteAllRecordFondo(idPensione);
                    db.DeleteAllRedditiDRedd(idPensione);
                    db.DeleteMaggiorazioniBenefici(idPensione);
                    db.DeletePensioniEstereDC(idPensione);
                    db.DeleteAllDanteCausa(idPensione);
                    db.DeleteAllResidenzeEstero(idPensione);
                    db.DeleteAllStatiCivili(idPensione);
                    db.DeleteLavorazione(idPensione);
                    db.DeleteEsitoCalcolo(idPensione);
                    db.DeleteMaternitaAcna(idPensione);
                    db.DeletePensioniCiContributiEE(idPensione);
                    db.DeleteRipartizioneFondi(idPensione);
                    db.DeleteDL407(idPensione);
                    db.DeleteStampa(idPensione);
                    db.DeleteOneri(idPensione);
                    db.DeleteAltrePensioni(idPensione);
                    db.DeleteAllBeneficiParticolari(idPensione);
                    db.DeleteDatiControlloFelpe(idPensione);
                    db.DeleteAllDatiPostDecOriginaria(idPensione);
                    db.DeleteEnpals(idPensione);
                    db.DeletePrepensionamento(idPensione);
                    db.DeleteAllContribuzioneEnpals(idPensione);
                    db.DeleteTrattenuteQuotePensione(idPensione);
                    db.DeleteQuotePensione(idPensione);
                    db.DeleteQuoteMiglioramentiContrattuali(idPensione);
                    db.DeleteMiglioramentiContrattuali(idPensione);
                    db.DeleteAllQuadroDatiRecordNoCalcolo(idPensione);
                    db.DeleteAllComponentiFamiliari(idPensione);
                    db.DeleteAllRecordDatiNoCalcolo(idPensione);
                    db.DeleteBeneficioVittimeTerrorismo(idPensione);
                    db.DeleteCalcoloVittimeTerrorismo(idPensione);
                    db.DeleteDatiStoricoGP(idPensione);
                    db.DeleteAllPeriodiAventiDiritto(idPensione);
                    db.DeleteAllAventiDiritto(idPensione);
                    db.DeleteRipartizioneINPDAP(idPensione);
                    db.DeleteAllInteressiLegali(idPensione);
                    db.DeleteSentenzaArt4(idPensione);
                    db.DeleteAllDetrazioniImpostaContitolare(idPensione);
                    db.DeleteSupplementiCumulo(idPensione);
                    db.DeleteAnniRichiestaBonus(idPensione);
                    db.DeleteRedditiPerIntegrazioneVirtuale(idPensione);
                    db.DeleteQuotaFondoIntegrativo(idPensione);
                    db.DeleteCalcoloContributivoINPGI(idPensione);
                    db.DeleteCalcoloRetributivoINPGI(idPensione);
                    db.DeleteAllPensioneImportiEsteriCumulo(idPensione);
                    db.DeletePensioneEsteraCumuloNoStorico(idPensione);
                    db.DeletePensione(idPensione);

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaPatronato(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePatronato(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePatronato");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaSindacati(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteSindacato(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteSindacato");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaEliminazione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteEliminazione(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteEliminazione");
                }
                db.Connection.Close();
            }
        }

        public static bool IsDomandaLavorabile(string tipoAppartenza, string fondo, string gruppo, string prodotto, string tipo, string filtro, string siglaCategoria)
        {
            int count = 0;
            filtro = filtro.PadRight(3, ' ');

            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));

                    if (!String.IsNullOrEmpty(fondo))
                        count = SelectTipologieNonAbilitate(db, tipoAppartenza, fondo, gruppo, prodotto, tipo, filtro, siglaCategoria);
                    else
                        count = SelectTipologieNonAbilitate(db, tipoAppartenza, gruppo, prodotto, tipo, filtro, siglaCategoria);

                    db.Connection.Close();
                    transactionScope.Complete();

                    return (count == 0);
                }
            }
        }

        private static int SelectTipologieNonAbilitate(PensioniDataContext db, string tipoAppartenza, string fondo, string gruppo, string prodotto, string tipo, string filtro, string siglaCategoria)
        {
            int count = 0;

            count = (from t in db.TipologieNonAbilitates
                     where t.TipoApp == tipoAppartenza && t.Fondo == fondo && t.Gruppo == gruppo && (t.Prodotto == "ALL" || t.Prodotto == prodotto) && (t.Tipo == "ALL" || t.Tipo == tipo) &&
                        (t.Filtro == "ALL" || t.Filtro == filtro) && (t.SiglaCategoria == "ALL" || t.SiglaCategoria == siglaCategoria)
                     select t).Count();

            return count;
        }

        private static int SelectTipologieNonAbilitate(PensioniDataContext db, string tipoAppartenza, string gruppo, string prodotto, string tipo, string filtro, string siglaCategoria)
        {
            int count = 0;

            count = (from t in db.TipologieNonAbilitates
                     where t.TipoApp == tipoAppartenza && t.Gruppo == gruppo && (t.Prodotto == "ALL" || t.Prodotto == prodotto) && (t.Tipo == "ALL" || t.Tipo == tipo) && (t.Filtro == "ALL" || t.Filtro == filtro) &&
                         (t.SiglaCategoria == "ALL" || t.SiglaCategoria == siglaCategoria)
                     select t).Count();

            return count;
        }

        public static void GetByPassCancellazione(long nDomus, short codiceSede, byte centroOperativo, string siglaCategoria, out ByPassCancellazione byPassCancellazione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    byPassCancellazione = (from s in db.ByPassCancellaziones
                                           where s.NDomus == nDomus && s.CodiceSede == codiceSede && s.CentroOperativo == centroOperativo && s.SiglaCategoria == siglaCategoria
                                           select s).SingleOrDefault<ByPassCancellazione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void DeleteByPassCancellazione(ByPassCancellazione byPassCancellazione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteByPassCancellazione(byPassCancellazione.NDomus, byPassCancellazione.CodiceSede, byPassCancellazione.CentroOperativo, byPassCancellazione.SiglaCategoria);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteByPassCancellazione");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaByPassCancellazione(ByPassCancellazione byPassCancellazione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));

                int result = db.InsertByPassCancellazione(byPassCancellazione.NDomus, byPassCancellazione.CodiceSede, byPassCancellazione.CentroOperativo, byPassCancellazione.SiglaCategoria);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertByPassCancellazione");
                }
                db.Connection.Close();
            }
        }

        public static void GetDataMorteByIdAnagrafica(long idAnagrafica, out DateTime? dataMorteTitolare)
        {
            dataMorteTitolare = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    List<Titolare> listaTitolare = (from t in db.Titolares
                                                    where t.IdAnagrafica == idAnagrafica
                                                    select t).ToList();

                    if (listaTitolare != null && listaTitolare.Count > 0)
                    {
                        dataMorteTitolare = listaTitolare.Exists(x => x.DataMorte.HasValue) ? listaTitolare.Find(x => x.DataMorte.HasValue).DataMorte : null;
                    }

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void UpdateProgStorico(long numeroDomanda)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.UpdateProgStorico(numeroDomanda);

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure UpdateProgStorico");
                }
                db.Connection.Close();
            }
        }

        //ENG - RIC REVERSIBILITA ENPALS
        public static DateTime? GetPLReversibilitaEnpals(string siglacategoria, int numeroCertificato, long idAnagraficaTitolare)
        {
            DateTime? scadenzaBeneficio = null;

            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    List<Oneri> listaOneri = (from t in db.Pensiones
                                              join t1 in db.Titolares on t.Id equals t1.IdPensione
                                              join t3 in db.Oneris on t.Id equals t3.IdPensione
                                              join t4 in db.Lavoraziones on t.Id equals t4.IdPensione
                                              where t.Gruppo == "0003" && t.Prodotto == "0021"
                                              && t.SiglaCategoria == siglacategoria
                                              && t.NCertificato == numeroCertificato
                                              && t1.IdAnagrafica == idAnagraficaTitolare
                                              && t3.ScadenzaBeneficio != null
                                              && t4.CodFase != "0060" && t4.CodFase != "0062" && t4.CodFase != "0063"
                                              select t3).ToList();

                    if (listaOneri != null && listaOneri.Count > 0)
                    {
                        if (listaOneri.Where(x => x.ScadenzaBeneficio.HasValue) != null && listaOneri.Where(x => x.ScadenzaBeneficio.HasValue).Count() > 0)
                        {
                            scadenzaBeneficio = listaOneri.Where(x => x.ScadenzaBeneficio.HasValue).FirstOrDefault().ScadenzaBeneficio;
                        }
                    }

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }

            return scadenzaBeneficio;
        }
    }
}

