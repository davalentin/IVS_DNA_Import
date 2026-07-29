using System;
using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Transactions;
using INPS.DNA.Data;
using System.Reflection;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaAventiDiritto
    {
        #region public methods
        public static bool GetAventiDirittoByDatiPensione(GestionePensione.DatiPensione datiPensione, out Entity.AventiDiritto areaAventiDiritto, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            areaAventiDiritto = new Entity.AventiDiritto();
            try
            {
                List<GestioneAventiDiritto.AventiDiritto> listaAventiDiritto = null;
                GestioneAventiDiritto.GetAventiDirittoByIdPensione(datiPensione.Id, out listaAventiDiritto);
                if (listaAventiDiritto != null && listaAventiDiritto.Count > 0)
                {
                    List<GestionePeriodiAventiDiritto.PeriodoAventiDiritto> listaPeriodi = null;
                    GestionePeriodiAventiDiritto.GetPeriodiAventiDiritto(datiPensione.Id, null, out listaPeriodi);
                    if (listaPeriodi != null && listaPeriodi.Count > 0)
                        listaAventiDiritto.ForEach(x => x.ListaPeriodi = listaPeriodi.FindAll(y => y.IdAventeDiritto == x.Id));
                }

                areaAventiDiritto.ListaAventiDiritto = listaAventiDiritto;
                return true;
            }
            catch (Exception Ex)
            {
                messaggioVideo = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
        }

        public static bool GetAventiDirittoConAnagraficheByDatiPensione(GestionePensione.DatiPensione datiPensione, out Entity.AventiDiritto areaAventiDiritto, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            areaAventiDiritto = new Entity.AventiDiritto();
            GestioneAnagrafica.DatiAnagrafici anagraficaTitolare = null;
            try
            {
                List<GestioneAventiDiritto.AventiDiritto> listaAventiDiritto = null;
                List<GestioneAnagrafica.DatiAnagrafici> listaAnagrafiche = null;
                GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out anagraficaTitolare);
                GestioneAventiDiritto.GetAventiDirittoConAnagraficheByIdPensione(datiPensione.Id, out listaAventiDiritto, out listaAnagrafiche);
                if (listaAventiDiritto != null && listaAventiDiritto.Count > 0)
                {
                    List<GestionePeriodiAventiDiritto.PeriodoAventiDiritto> listaPeriodi = null;
                    GestionePeriodiAventiDiritto.GetPeriodiAventiDiritto(datiPensione.Id, null, out listaPeriodi);
                    if (listaPeriodi != null && listaPeriodi.Count > 0)
                        listaAventiDiritto.ForEach(x => x.ListaPeriodi = listaPeriodi.FindAll(y => y.IdAventeDiritto == x.Id));
                }

                GestioneAventiDiritto.SortAventiDiritto(anagraficaTitolare.CodiceFiscale, ref listaAventiDiritto, listaAnagrafiche);

                areaAventiDiritto.ListaAventiDiritto = listaAventiDiritto;
                areaAventiDiritto.ListaAnagrafiche = listaAnagrafiche;
                return true;
            }
            catch (Exception Ex)
            {
                messaggioVideo = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
        }

        public static bool ControlsDatiAventiDiritto(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici anagraficaTitolare, Entity.AventiDiritto areaAventiDiritto,
            GestioneAnagrafica.DatiAnagrafici anagraficaDanteCausa, bool isRiaperturaDomanda, BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (areaAventiDiritto == null)
            {
                messaggioVideo = "Nessun dato da salvare.";
                return false;
            }

            if (areaAventiDiritto.ListaAventiDiritto == null || areaAventiDiritto.ListaAventiDiritto.Count == 0)
            {
                messaggioVideo = "Nessun Avente Diritto da salvare.";
                return false;
            }

            if (areaAventiDiritto.ListaAnagrafiche == null ||
                areaAventiDiritto.ListaAventiDiritto.Exists(x => !areaAventiDiritto.ListaAnagrafiche.Exists(y => y.Id == x.IdAnagrafica)))
            {
                messaggioVideo = "Anagrafiche degli Aventi Diritto non presenti.";
                return false;
            }

            if (!areaAventiDiritto.ListaAnagrafiche.Exists(x => x.CodiceFiscale == anagraficaTitolare.CodiceFiscale))
            {
                messaggioVideo = "Il titolare non è presente tra gli Aventi Diritto.";
                return false;
            }

            foreach (var anag in areaAventiDiritto.ListaAnagrafiche)
            {
                if (areaAventiDiritto.ListaAventiDiritto.Count(x => x.IdAnagrafica == anag.Id) > 1 && !areaAventiDiritto.ListaAventiDiritto.Exists(x => x.IsSelezionato.GetValueOrDefault()))
                {
                    messaggioVideo = string.Format("Selezionare almeno una checkbox per l'avente diritto {0}", anag.CodiceFiscale);
                    return false;
                }
            }

            if (!GestioneCrossControls.AGO_ControlsSessoAventiDirittoWithParentela(areaAventiDiritto.ListaAventiDiritto, areaAventiDiritto.ListaAnagrafiche,
                anagraficaDanteCausa != null ? anagraficaDanteCausa.Sesso : null, out messaggioVideo))
                return false;

            bool isAventeDirittoTitolareIncongruente = areaAventiDiritto.ListaAventiDiritto.Count(x => x.IdAnagrafica == anagraficaTitolare.Id) > 1;

            // Effettuo i controlli sui periodi solo per il titolare
            foreach (GestioneAventiDiritto.AventiDiritto aventeDiritto in areaAventiDiritto.ListaAventiDiritto.FindAll(x => x.IdAnagrafica == anagraficaTitolare.Id))
            {
                if (aventeDiritto.ListaPeriodi != null && aventeDiritto.ListaPeriodi.Count > 0)
                {
                    GestioneAnagrafica.DatiAnagrafici anagraficaAventeDiritto = areaAventiDiritto.ListaAnagrafiche.Find(x => x.Id == aventeDiritto.IdAnagrafica);

                    if (!GestioneAreaPeriodiAventiDiritto.ControlsDatiPeriodiAventiDiritto(datiPensione, aventeDiritto.ListaPeriodi, anagraficaAventeDiritto, aventeDiritto,
                        anagraficaDanteCausa, isAventeDirittoTitolareIncongruente, isRiaperturaDomanda, danteCausa, out messaggioVideo))
                    {
                        messaggioVideo = "Errore per il soggetto " + anagraficaAventeDiritto.CodiceFiscale + ": " + messaggioVideo;
                        return false;
                    }
                }
            }

            if (!GestioneCrossControls.AGO_ControlsCombinazioneCodiceNucleo(areaAventiDiritto.ListaAventiDiritto, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_ControlsVincoliAventiDiritto(areaAventiDiritto.ListaAventiDiritto, out messaggioVideo))
                return false;

            foreach (GestioneAventiDiritto.AventiDiritto aventeDiritto in areaAventiDiritto.ListaAventiDiritto)
            {
                if (string.IsNullOrEmpty(aventeDiritto.CodiceNucleo))
                {
                    messaggioVideo = "Codice nucleo obbligatorio.";
                    return false;
                }
            }

            return true;
        }

        public static void StoreAventiDiritto(GestionePensione.DatiPensione datiPensione, Entity.AventiDiritto areaAventiDiritto)
        {
            if (areaAventiDiritto != null && areaAventiDiritto.ListaAventiDiritto != null && areaAventiDiritto.ListaAventiDiritto.Count > 0)
            {
                GestioneQuadri.DatiQuadroAventiDiritto datiQuadroAventiDiritto = null;
                GestioneQuadri.GetQuadroAventiDirittoByDatiPensione(datiPensione, out datiQuadroAventiDiritto);

                List<GestioneAventiDiritto.AventiDiritto> listaAventiDirittoMultipli = areaAventiDiritto.ListaAventiDiritto.FindAll(x => areaAventiDiritto.ListaAventiDiritto.Count(y => y.IdAnagrafica == x.IdAnagrafica) > 1);
                List<GestioneAventiDiritto.AventiDiritto> listaAventiDirittoToDelete = listaAventiDirittoMultipli.FindAll(x => !x.IsSelezionato.GetValueOrDefault());
                areaAventiDiritto.ListaAventiDiritto.RemoveAll(x => listaAventiDirittoMultipli.Exists(y => x.IdAnagrafica == y.IdAnagrafica && !x.IsSelezionato.GetValueOrDefault()));

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    foreach (var aventeDiritto in listaAventiDirittoToDelete)
                    {
                        GestionePeriodiAventiDiritto.DeletePeriodiAventiDirittoByIdAventeDiritto(aventeDiritto.Id);
                        GestioneAventiDiritto.DeleteAventeDirittoById(aventeDiritto.Id);
                    }

                    foreach (var aventeDiritto in areaAventiDiritto.ListaAventiDiritto)
                    {
                        GestionePeriodiAventiDiritto.DeletePeriodiAventiDirittoByIdAventeDiritto(aventeDiritto.Id);
                        aventeDiritto.ListaPeriodi.ForEach(x => x.IdAventeDiritto = aventeDiritto.Id);
                        GestionePeriodiAventiDiritto.SavePeriodiAventiDiritto(aventeDiritto.IdPensione, aventeDiritto.ListaPeriodi);

                        //Salvo i dati aventi diritto compreso il codice nucleo.
                        GestioneAventiDiritto.SalvaAventeDiritto(aventeDiritto);
                    }

                    datiQuadroAventiDiritto.TabAventiDiritto = 2;

                    GestioneQuadri.SalvaQuadroAventiDiritto(datiPensione.Id, datiQuadroAventiDiritto);

                    transactionScope.Complete();
                }
            }
        }

        public static bool AggiornaAventiDirittoFromWebDom(Entity.ParametriARCA parametriArca, GestionePensione.DatiPensione datiPensione, string codiceFiscaleTitolare, out string errori)
        {
            errori = string.Empty;
            ServiceReferences.WebDom.DatiDomanda datiDomanda = null;
            List<GestioneAventiDiritto.AventeDirittoRecuperato> listaAventiDirittoFromWebDom = null;
            List<GestioneAventiDiritto.AventeDirittoRecuperato> listaAventiDirittoDB = null;
            bool isListaPeriodiTitolareChanged = false;

            try
            {
                // Recupero gli Aventi Diritto da WebDom
                GestioneWebDom.GetDomandaPerDomus(datiPensione.NDomus.ToString(), out datiDomanda, out errori);
                if (!string.IsNullOrEmpty(errori))
                    return false;
                if (GestioneAreaRiepilogo.RecuperaAventiDirittoFromWebDom(datiDomanda, out listaAventiDirittoFromWebDom, out errori))
                    return false;

                // Recupero gli Aventi Diritto presenti a DB
                GestioneAventiDiritto.GetAventiDirittoRecuperatoByIdPensione(datiPensione.Id, codiceFiscaleTitolare, out listaAventiDirittoDB);

                GestioneQuadri.DatiQuadroPeriodi datiQuadroPeriodi = null;
                GestioneQuadri.GetQuadroPeriodiByDatiPensione(datiPensione, out datiQuadroPeriodi);

                GestioneQuadri.DatiQuadroAventiDiritto datiQuadroAventiDiritto = null;
                GestioneQuadri.GetQuadroAventiDirittoByDatiPensione(datiPensione, out datiQuadroAventiDiritto);

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {

                    if (!MergeAndSaveAventiDiritto(datiPensione, null, listaAventiDirittoFromWebDom, listaAventiDirittoDB, EnumTipoMerge.WebDom_DB, parametriArca, out isListaPeriodiTitolareChanged, out errori))
                        return false;

                    if (isListaPeriodiTitolareChanged)
                    {
                        datiQuadroPeriodi.TabPeriodi = 0;
                        GestioneQuadri.SalvaQuadroPeriodi(datiPensione.Id, datiQuadroPeriodi);
                    }

                    datiQuadroAventiDiritto.TabAventiDiritto = 0;
                    GestioneQuadri.SalvaQuadroAventiDiritto(datiPensione.Id, datiQuadroAventiDiritto);

                    transactionScope.Complete();
                }
            }
            catch (Exception Ex)
            {
                string messaggio = Utility.GetMessageFromException(Ex);
                errori = "Errore tecnico nel recupero degli aventi diritto";
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, Ex.StackTrace);
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }

            return true;
        }

        public static bool AggiornaAventiDirittoFromArchivioPensione(Entity.ParametriARCA parametriArca, GestionePensione.DatiPensione datiPensione, BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa,
            string codiceFiscaleTitolare, short sedeOperatore, short centroOperativoOperatore, out string errori)
        {
            errori = string.Empty;
            List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficaAventiDiritto = null;
            List<GestioneAventiDiritto.AventeDirittoRecuperato> listaAventiDirittoFromArchivioPensione = null;
            List<GestioneAventiDiritto.AventeDirittoRecuperato> listaAventiDirittoDB = null;
            bool isListaPeriodiTitolareChanged = false;

            try
            {
                // Recupero le informazioni
                string codiceFascicolo = datiDanteCausa != null ? datiDanteCausa.CategoriaFascicolo.GetValueOrDefault().ToString().PadLeft(3, '0') + datiDanteCausa.SedeFascicolo.GetValueOrDefault().ToString().PadLeft(4, '0') +
                            datiDanteCausa.NumeroFascicolo.GetValueOrDefault().ToString().PadLeft(8, '0') : null;
                if (!GestioneDatiPensioni.GetDatiTGP4ByCodiceFascicolo(datiPensione.NDomus, codiceFascicolo, codiceFiscaleTitolare, parametriArca, out listaAventiDirittoFromArchivioPensione,
                    out listaAnagraficaAventiDiritto, out errori))
                    return false;

                // Recupero gli Aventi Diritto presenti a DB
                GestioneAventiDiritto.GetAventiDirittoRecuperatoByIdPensione(datiPensione.Id, codiceFiscaleTitolare, out listaAventiDirittoDB);

                GestioneQuadri.DatiQuadroPeriodi datiQuadroPeriodi = null;
                GestioneQuadri.GetQuadroPeriodiByDatiPensione(datiPensione, out datiQuadroPeriodi);

                GestioneQuadri.DatiQuadroAventiDiritto datiQuadroAventiDiritto = null;
                GestioneQuadri.GetQuadroAventiDirittoByDatiPensione(datiPensione, out datiQuadroAventiDiritto);

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    // Faccio il merge con i dati già presenti a DB
                    if (!MergeAndSaveAventiDiritto(datiPensione, listaAnagraficaAventiDiritto, listaAventiDirittoDB, listaAventiDirittoFromArchivioPensione, EnumTipoMerge.DB_GP, parametriArca,
                        out isListaPeriodiTitolareChanged, out errori))
                        return false;

                    if (isListaPeriodiTitolareChanged)
                    {
                        datiQuadroPeriodi.TabPeriodi = 0;
                        GestioneQuadri.SalvaQuadroPeriodi(datiPensione.Id, datiQuadroPeriodi);
                    }

                    datiQuadroAventiDiritto.TabAventiDiritto = 0;
                    GestioneQuadri.SalvaQuadroAventiDiritto(datiPensione.Id, datiQuadroAventiDiritto);

                    transactionScope.Complete();
                }
            }
            catch (Exception Ex)
            {
                errori = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Unisce e salva gli aventi diritto e i periodi provenienti dalle due liste
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <param name="listaAventiDirittoMaster">
        ///     Elenco proveniente da WebDom o dal DB
        /// </param>
        /// <param name="listaAventiDirittoDaPrelievo">
        ///     Elenco proveniente dal GP o dal DB
        /// </param>
        /// <param name="tipoMerge"></param>
        /// <param name="parametriArca"></param>
        internal static bool MergeAndSaveAventiDiritto(GestionePensione.DatiPensione datiPensione, List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficaAventiDiritto,
            List<GestioneAventiDiritto.AventeDirittoRecuperato> listaAventiDirittoMaster, List<GestioneAventiDiritto.AventeDirittoRecuperato> listaAventiDirittoDaPrelievo, EnumTipoMerge tipoMerge,
            Entity.ParametriARCA parametriArca, out bool isListaPeriodiTitolareChanged, out string errori)
        {
            errori = string.Empty;
            isListaPeriodiTitolareChanged = false;

            List<GestioneAventiDiritto.AventeDirittoRecuperato> listaAventiDirittoToSave = new List<GestioneAventiDiritto.AventeDirittoRecuperato>();
            List<GestioneAventiDiritto.AventeDirittoRecuperato> listaAventiDirittoToDelete = new List<GestioneAventiDiritto.AventeDirittoRecuperato>();

            GestioneARCA.RichiestaARCA richiestaArca = new GestioneARCA.RichiestaARCA();
            richiestaArca.Applicazione = parametriArca.Applicazione;
            richiestaArca.Matricola = parametriArca.Matricola;
            richiestaArca.Provenienza = parametriArca.Provenienza;
            richiestaArca.Ruolo = parametriArca.Ruolo;

            switch (tipoMerge)
            {
                case EnumTipoMerge.WebDom_DB:
                    if (listaAventiDirittoDaPrelievo != null)
                        listaAventiDirittoDaPrelievo.ForEach(x => x.PresenzaWebDom = false);
                    break;
                case EnumTipoMerge.DB_GP:
                    if (listaAventiDirittoMaster != null)
                        listaAventiDirittoMaster.ForEach(x => x.PresenzaGP = false);
                    break;
            }

            if (listaAventiDirittoMaster != null && listaAventiDirittoMaster.Count > 0)
            {
                // Verifico le incongruenze tra Host e WebDom
                if (listaAventiDirittoDaPrelievo != null && listaAventiDirittoDaPrelievo.Count > 0)
                {
                    // Recupero tutti i soggetti presenti su WebDom e non su Host
                    listaAventiDirittoToSave.AddRange(listaAventiDirittoMaster.FindAll(x => !listaAventiDirittoDaPrelievo.Exists(y => y.CodiceFiscale == x.CodiceFiscale)));
                    // Recupero tutti i soggetti presenti su Host e non su WebDom
                    listaAventiDirittoToSave.AddRange(listaAventiDirittoDaPrelievo.FindAll(x => !listaAventiDirittoMaster.Exists(y => y.CodiceFiscale == x.CodiceFiscale)));
                    // Recupero tutti i soggetti presenti sia su WebDom che su Host
                    List<GestioneAventiDiritto.AventeDirittoRecuperato> listaApp = listaAventiDirittoMaster.FindAll(x => listaAventiDirittoDaPrelievo.Exists(y => y.CodiceFiscale == x.CodiceFiscale));
                    List<string> listaCodiciFiscali = listaApp != null ? listaApp.GroupBy(x => x.CodiceFiscale).Select(x => x.First().CodiceFiscale).ToList() : new List<string>();

                    foreach (string codiceFiscale in listaCodiciFiscali)
                    {
                        GestioneAventiDiritto.AventeDirittoRecuperato aventeDirittoWebDom = listaAventiDirittoMaster.Count(x => x.CodiceFiscale == codiceFiscale) > 1 ?
                            listaAventiDirittoMaster.Find(x => x.CodiceFiscale == codiceFiscale && x.PresenzaWebDom) :
                            listaAventiDirittoMaster.Find(x => x.CodiceFiscale == codiceFiscale);
                        GestioneAventiDiritto.AventeDirittoRecuperato aventeDirittoDaPrelievo = listaAventiDirittoDaPrelievo.Count(x => x.CodiceFiscale == codiceFiscale) > 1 ?
                            listaAventiDirittoDaPrelievo.Find(x => x.CodiceFiscale == codiceFiscale && x.PresenzaGP) :
                            listaAventiDirittoDaPrelievo.Find(x => x.CodiceFiscale == codiceFiscale);

                        if (aventeDirittoWebDom != null && aventeDirittoDaPrelievo != null)
                        {
                            // Verifico se ci sono differenze nei periodi
                            if (tipoMerge == EnumTipoMerge.DB_GP && aventeDirittoWebDom.IsTitolare)
                            {
                                // Se il numero di elementi delle due liste è diverso oppure
                                // se esiste un elemento x che non ha una corrispondenza negli elementi y
                                if (aventeDirittoWebDom.ListaPeriodi.Count != aventeDirittoDaPrelievo.ListaPeriodi.Count ||
                                    aventeDirittoWebDom.ListaPeriodi.Exists(x => !aventeDirittoDaPrelievo.ListaPeriodi.Exists(y => x.Equals(y))) ||
                                    aventeDirittoDaPrelievo.ListaPeriodi.Exists(x => !aventeDirittoWebDom.ListaPeriodi.Exists(y => x.Equals(y))))
                                    isListaPeriodiTitolareChanged = true;
                            }

                            //TODO: Verificare sull'Aggiorna da Archivio Pensione come andranno recuperati i periodi per il titolare
                            // Ricerco gli aventi diritto per cui esiste una incongruenza sul Nucleo Titolare
                            if ((((tipoMerge == EnumTipoMerge.WebDom_GP || tipoMerge == EnumTipoMerge.WebDom_DB) && aventeDirittoDaPrelievo.PresenzaGP) || (tipoMerge == EnumTipoMerge.DB_GP && aventeDirittoWebDom.PresenzaWebDom)) &&
                                aventeDirittoWebDom.NucleoTitolare != aventeDirittoDaPrelievo.NucleoTitolare)
                            {
                                // Se non ho già inserito il doppio record oppure sto acquisendo la domanda
                                if (tipoMerge == EnumTipoMerge.WebDom_GP ||
                                    (tipoMerge == EnumTipoMerge.DB_GP && listaAventiDirittoMaster.Count(x => x.CodiceFiscale == codiceFiscale) == 1) ||
                                    (tipoMerge == EnumTipoMerge.WebDom_DB && listaAventiDirittoDaPrelievo.Count(x => x.CodiceFiscale == codiceFiscale) == 1))
                                {
                                    // I Periodi arrivano da Host, quindi li inserisco in entrambi gli oggetti
                                    aventeDirittoWebDom.ListaPeriodi = aventeDirittoDaPrelievo.ListaPeriodi;
                                    if (tipoMerge == EnumTipoMerge.WebDom_GP || tipoMerge == EnumTipoMerge.WebDom_DB)
                                        aventeDirittoDaPrelievo.PresenzaWebDom = false;
                                    else if (tipoMerge == EnumTipoMerge.DB_GP)
                                        aventeDirittoWebDom.PresenzaGP = false;
                                    listaAventiDirittoToSave.Add(aventeDirittoWebDom);
                                    listaAventiDirittoToSave.Add(aventeDirittoDaPrelievo);
                                }
                                else
                                {
                                    // Prendo l'oggetto che ho salvato precedentemente a DB e aggiorno i dati
                                    // Fase di aggiornamento del record che presentava incongruenza precedentemente acquisito
                                    GestioneAventiDiritto.AventeDirittoRecuperato aventeDirittoDaAggiornare = null;
                                    switch (tipoMerge)
                                    {
                                        case EnumTipoMerge.DB_GP:
                                            aventeDirittoDaAggiornare = listaAventiDirittoMaster.Find(x => x.CodiceFiscale == codiceFiscale && !x.PresenzaWebDom);
                                            aventeDirittoDaAggiornare.DecParentelaDA = aventeDirittoDaPrelievo.DecParentelaDA;
                                            aventeDirittoDaAggiornare.NucleoTitolare = aventeDirittoDaPrelievo.NucleoTitolare;
                                            aventeDirittoDaAggiornare.PercGiudice = aventeDirittoDaPrelievo.PercGiudice;
                                            aventeDirittoDaAggiornare.ListaPeriodi = aventeDirittoDaPrelievo.ListaPeriodi.ToList();
                                            aventeDirittoDaAggiornare.CategoriaPensione = aventeDirittoDaPrelievo.CategoriaPensione;
                                            aventeDirittoDaAggiornare.CertificatoPensione = aventeDirittoDaPrelievo.CertificatoPensione;
                                            aventeDirittoDaAggiornare.CodiceNucleoFromGP = aventeDirittoDaPrelievo.CodiceNucleoFromGP;
                                            aventeDirittoDaAggiornare.CSog = aventeDirittoDaPrelievo.CSog;
                                            aventeDirittoDaAggiornare.DataMatrimonio = aventeDirittoDaPrelievo.DataMatrimonio;
                                            aventeDirittoDaAggiornare.ScadenzaRevisioneSanitaria = aventeDirittoDaPrelievo.ScadenzaRevisioneSanitaria;
                                            aventeDirittoDaAggiornare.SedePensione = aventeDirittoDaPrelievo.SedePensione;
                                            aventeDirittoDaAggiornare.PresenzaGP = aventeDirittoDaPrelievo.PresenzaGP;
                                            listaAventiDirittoToSave.Add(aventeDirittoDaAggiornare);
                                            break;
                                        case EnumTipoMerge.WebDom_DB:
                                            aventeDirittoDaAggiornare = listaAventiDirittoDaPrelievo.Find(x => x.CodiceFiscale == codiceFiscale && !x.PresenzaGP);
                                            aventeDirittoDaAggiornare.DecParentelaDA = aventeDirittoWebDom.DecParentelaDA;
                                            aventeDirittoDaAggiornare.NucleoTitolare = aventeDirittoWebDom.NucleoTitolare;
                                            aventeDirittoDaAggiornare.PercGiudice = aventeDirittoWebDom.PercGiudice;
                                            aventeDirittoDaAggiornare.PresenzaWebDom = aventeDirittoWebDom.PresenzaWebDom;
                                            listaAventiDirittoToSave.Add(aventeDirittoDaAggiornare);
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                switch (tipoMerge)
                                {
                                    case EnumTipoMerge.WebDom_GP:
                                        // Fase: Acquisizione della domanda
                                        aventeDirittoDaPrelievo.PresenzaWebDom = aventeDirittoWebDom.PresenzaWebDom;
                                        listaAventiDirittoToSave.Add(aventeDirittoDaPrelievo);
                                        break;
                                    case EnumTipoMerge.WebDom_DB:
                                        // Fase: Aggiorna da WebDom
                                        aventeDirittoWebDom.ListaPeriodi = aventeDirittoDaPrelievo.ListaPeriodi.ToList();
                                        aventeDirittoWebDom.CategoriaPensione = aventeDirittoDaPrelievo.CategoriaPensione;
                                        aventeDirittoWebDom.CertificatoPensione = aventeDirittoDaPrelievo.CertificatoPensione;
                                        aventeDirittoWebDom.CodiceNucleoFromGP = aventeDirittoDaPrelievo.CodiceNucleoFromGP;
                                        aventeDirittoWebDom.CSog = aventeDirittoDaPrelievo.CSog;
                                        aventeDirittoWebDom.DataMatrimonio = aventeDirittoDaPrelievo.DataMatrimonio;
                                        aventeDirittoWebDom.IdAnagrafica = aventeDirittoDaPrelievo.IdAnagrafica;
                                        aventeDirittoWebDom.IdPensione = aventeDirittoDaPrelievo.IdPensione;
                                        aventeDirittoWebDom.ScadenzaRevisioneSanitaria = aventeDirittoDaPrelievo.ScadenzaRevisioneSanitaria;
                                        aventeDirittoWebDom.SedePensione = aventeDirittoDaPrelievo.SedePensione;
                                        aventeDirittoWebDom.Id = aventeDirittoDaPrelievo.Id;
                                        aventeDirittoWebDom.PresenzaGP = aventeDirittoDaPrelievo.PresenzaGP;
                                        listaAventiDirittoToSave.Add(aventeDirittoWebDom);
                                        listaAventiDirittoToDelete.AddRange(listaAventiDirittoDaPrelievo.FindAll(x => x.CodiceFiscale == codiceFiscale && x.Id != aventeDirittoWebDom.Id));
                                        break;
                                    case EnumTipoMerge.DB_GP:
                                        // Fase: Aggiorna da Archivio Pensione
                                        aventeDirittoDaPrelievo.PresenzaWebDom = aventeDirittoWebDom.PresenzaWebDom;
                                        aventeDirittoDaPrelievo.IdAnagrafica = aventeDirittoWebDom.IdAnagrafica;
                                        aventeDirittoDaPrelievo.IdPensione = aventeDirittoWebDom.IdPensione;
                                        aventeDirittoDaPrelievo.Id = aventeDirittoWebDom.Id;
                                        //TODO: verificare, a valle delle analisi, come andrà gestito il salvataggio dei periodi
                                        // Per il titolare mantengo i periodi che ho salvato
                                        if (aventeDirittoDaPrelievo.IsTitolare)
                                            aventeDirittoDaPrelievo.ListaPeriodi = aventeDirittoWebDom.ListaPeriodi;
                                        listaAventiDirittoToSave.Add(aventeDirittoDaPrelievo);
                                        listaAventiDirittoToDelete.AddRange(listaAventiDirittoMaster.FindAll(x => x.CodiceFiscale == codiceFiscale && x.Id != aventeDirittoDaPrelievo.Id));
                                        break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    switch (tipoMerge)
                    {
                        case EnumTipoMerge.WebDom_DB:
                        // Arrivano le informazioni da WebDom, ma a DB non c'è nulla
                        case EnumTipoMerge.WebDom_GP:
                            // Da Host non è arrivato nessun record, quindi salvo solo i record di WebDom
                            listaAventiDirittoToSave.AddRange(listaAventiDirittoMaster);
                            break;
                        case EnumTipoMerge.DB_GP:
                            // Dal GP non è arrivato nessun record, quindi se non esiste la presenza WebDom allora elimino i record
                            listaAventiDirittoToDelete.AddRange(listaAventiDirittoMaster.FindAll(x => !x.PresenzaWebDom));
                            listaAventiDirittoToSave.AddRange(listaAventiDirittoMaster.FindAll(x => x.PresenzaWebDom));
                            break;
                    }
                }
            }
            else
            {
                switch (tipoMerge)
                {
                    case EnumTipoMerge.DB_GP:
                    // Arrivano le informazioni dal GP, ma a DB non c'è nulla
                    case EnumTipoMerge.WebDom_GP:
                        // Da WebDom non è arrivato nessun record, quindi salvo solo i record del GP
                        if (listaAventiDirittoDaPrelievo != null && listaAventiDirittoDaPrelievo.Count > 0)
                            listaAventiDirittoToSave.AddRange(listaAventiDirittoDaPrelievo);
                        break;
                    case EnumTipoMerge.WebDom_DB:
                        // Da WebDom non è arrivato nessun record, quindi se non esiste la presenza GP allora elimino i record
                        if (listaAventiDirittoDaPrelievo != null && listaAventiDirittoDaPrelievo.Count > 0)
                        {
                            listaAventiDirittoToDelete.AddRange(listaAventiDirittoDaPrelievo.FindAll(x => !x.PresenzaGP));
                            listaAventiDirittoToSave.AddRange(listaAventiDirittoDaPrelievo.FindAll(x => x.PresenzaGP));
                        }
                        break;
                }
            }

            // Aggiungo i periodi in base alla relazione di parentela con il dante causa solo per il titolare proveniente da WebDom
            foreach (var aventeDirittoFromWebDomToSave in listaAventiDirittoToSave.FindAll(x => x.IsTitolare && x.PresenzaWebDom))
            {
                if (aventeDirittoFromWebDomToSave.ListaPeriodi == null)
                    aventeDirittoFromWebDomToSave.ListaPeriodi = new List<GestionePeriodiAventiDiritto.PeriodoAventiDiritto>();
                aventeDirittoFromWebDomToSave.ListaPeriodi.ForEach(x => x.IsFromWebDom = false);

                if (!aventeDirittoFromWebDomToSave.ListaPeriodi.Exists(x => x.GradoParentela == aventeDirittoFromWebDomToSave.DecParentelaDA && x.TipoUnione == aventeDirittoFromWebDomToSave.TipoUnione))
                {
                    GestionePeriodiAventiDiritto.PeriodoAventiDiritto periodo = new GestionePeriodiAventiDiritto.PeriodoAventiDiritto();
                    periodo.GradoParentela = aventeDirittoFromWebDomToSave.DecParentelaDA;
                    periodo.TipoUnione = aventeDirittoFromWebDomToSave.TipoUnione;
                    periodo.PercGiudice = aventeDirittoFromWebDomToSave.DecParentelaDA == 'R' ? aventeDirittoFromWebDomToSave.PercGiudice : null;
                    periodo.DecorrenzaPeriodo = datiPensione.DecorrenzaOriginaria;
                    periodo.IsFromWebDom = true;
                    aventeDirittoFromWebDomToSave.ListaPeriodi.Add(periodo);
                    isListaPeriodiTitolareChanged = true;
                }
            }

            // Per tutti gli aventi diritto presenti su WebDom, sul periodo corrispondente alla relazione di parentela con il DC indico che è proveniente da WebDom
            listaAventiDirittoToSave.FindAll(x => x.PresenzaWebDom && x.ListaPeriodi != null && x.ListaPeriodi.Count > 0).ForEach(a => a.ListaPeriodi.FindAll(x => x.GradoParentela == a.DecParentelaDA && x.TipoUnione == a.TipoUnione).ForEach(x => x.IsFromWebDom = true));

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (listaAventiDirittoToSave != null && listaAventiDirittoToSave.Count(x => x.PresenzaGP || x.PresenzaWebDom) > 0)
                {
                    foreach (var aventeDiritto in listaAventiDirittoToSave)
                    {
                        GestioneAnagrafica.DatiAnagrafici anagraficaDB = null;
                        if (listaAnagraficaAventiDiritto != null && listaAnagraficaAventiDiritto.Count > 0)
                            anagraficaDB = listaAnagraficaAventiDiritto.FirstOrDefault(x => x.CodiceFiscale == aventeDiritto.CodiceFiscale);

                        if (anagraficaDB == null)
                        {
                            richiestaArca.CodiceFiscaleRichiedente = aventeDiritto.CodiceFiscale;
                            richiestaArca.CodiceFiscale = aventeDiritto.CodiceFiscale;

                            Entity.Anagrafica anagrafica = null;
                            if (!string.IsNullOrEmpty(richiestaArca.CodiceFiscale))
                            {
                                GestioneARCA.GetAnagraficaArcaByCodiceFiscale(richiestaArca, datiPensione.NDomus.ToString(), out anagrafica, out errori);
                                if (!string.IsNullOrEmpty(errori))
                                    return false;
                            }

                            anagraficaDB = new GestioneAnagrafica.DatiAnagrafici();
                            Utility.ValorizzaOggetti(anagrafica, anagraficaDB);
                            GestioneAnagrafica.SalvaAnagrafica(anagraficaDB);

                            if (!aventeDiritto.CSog.HasValue)
                            {
                                int codiceSoggetto = 0;
                                GestioneArcaMan.GetCodiceSoggettoByArcaMan(aventeDiritto.CodiceFiscale, datiPensione.NDomus.ToString(), out codiceSoggetto, out errori);
                                if (!string.IsNullOrEmpty(errori))
                                    return false;
                                aventeDiritto.CSog = codiceSoggetto;
                            }
                        }

                        GestioneAventiDiritto.AventiDiritto aventeDirittoDB = new GestioneAventiDiritto.AventiDiritto();
                        Utility.ValorizzaOggetti(aventeDiritto, aventeDirittoDB);
                        aventeDirittoDB.IdAnagrafica = anagraficaDB.Id;
                        aventeDirittoDB.IdPensione = datiPensione.Id;

                        GestioneAventiDiritto.SalvaAventeDiritto(aventeDirittoDB);
                        aventeDiritto.Id = aventeDirittoDB.Id;

                        if (aventeDiritto.IsTitolare)
                        {
                            // Il Familiare serve per le informazioni relative al quadro Periodi
                            GestioneFamiliari.Familiare familiareTitolare = new GestioneFamiliari.Familiare();
                            familiareTitolare.FlagTitolare = true;
                            familiareTitolare.IdAnagrafica = anagraficaDB.Id;
                            familiareTitolare.IdPensione = datiPensione.Id;
                            familiareTitolare.TipoComponente = 'T';
                            familiareTitolare.SiglaFamiliare = aventeDiritto.DecParentelaDA;
                            familiareTitolare.TipoUnione = aventeDiritto.TipoUnione;
                            familiareTitolare.ScadenzaRevisioneSanitaria = aventeDiritto.ScadenzaRevisioneSanitaria;
                            if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione)
                                familiareTitolare.Confermato = true;
                            if (aventeDiritto.PresenzaGP)
                                familiareTitolare.Provenienza = 'P';
                            else
                                familiareTitolare.Provenienza = 'W';

                            GestioneFamiliari.SalvaFamiliare(familiareTitolare, null, anagraficaDB, null, datiPensione.Id, datiPensione.SiglaCategoria);
                        }

                        if (aventeDiritto.ListaPeriodi != null && aventeDiritto.ListaPeriodi.Count > 0)
                        {
                            aventeDiritto.ListaPeriodi.ForEach(x => x.IdAventeDiritto = aventeDirittoDB.Id);
                            GestionePeriodiAventiDiritto.SavePeriodiAventiDiritto(datiPensione.Id, aventeDiritto.ListaPeriodi);
                        }
                    }
                }

                // Rimuovo gli aventi diritto non più presenti
                listaAventiDirittoToDelete.AddRange(listaAventiDirittoToSave.FindAll(x => !x.PresenzaGP && !x.PresenzaWebDom));
                foreach (var aventeDiritto in listaAventiDirittoToDelete)
                {
                    GestionePeriodiAventiDiritto.DeletePeriodiAventiDirittoByIdAventeDiritto(aventeDiritto.Id);
                    GestioneAventiDiritto.DeleteAventeDirittoById(aventeDiritto.Id);
                }

                transactionScope.Complete();
            }

            return true;
        }
        #endregion public methods

        #region enum
        public enum EnumTipoMerge
        {
            WebDom_GP,
            WebDom_DB,
            DB_GP
        }
        #endregion enum
    }
}
