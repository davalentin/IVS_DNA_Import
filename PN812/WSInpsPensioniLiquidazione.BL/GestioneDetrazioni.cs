using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Configuration;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.ServiceReferences.UniDetra;
using System.Transactions;
using INPS.DNA.Data;
using System.Reflection;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneDetrazioni
    {
        #region public members
        public static bool GetDetrazioniByDatiPensione(GestionePensione.DatiPensione datiPensione, string codiceFiscale, bool isContitolare, long idAnagrafica, out RispostaDetrazioni risposta, out string errori)
        {
            risposta = null;
            errori = "";
            try
            {
                Utility.StatoPensione? stato = Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.GetValueOrDefault());
                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                switch (stato.GetValueOrDefault())
                {
                    case Utility.StatoPensione.Calcolata:
                    case Utility.StatoPensione.CalcolataNoWebDom:
                    case Utility.StatoPensione.CalcolataNoFelpe:
                    case Utility.StatoPensione.CalcolataNoOneri:
                    case Utility.StatoPensione.CalcolataNoSAI:
                    case Utility.StatoPensione.CalcolataNoStazLavoro:
                    case Utility.StatoPensione.CalcolataNoTotal:
                    case Utility.StatoPensione.CalcolataNoTot:
                    case Utility.StatoPensione.CalcolataNoSIN:
                    case Utility.StatoPensione.CalcolataNoNoteDebito:
                    case Utility.StatoPensione.CalcolataNo6Scatti:
                        if (isContitolare)
                        {
                            GestioneDetrazioniContitolare.DatiDetrazioniContitolare datiDetrazioni = null;
                            GestioneDetrazioniContitolare.GetDetrazioniBySoggetto(datiPensione.Id, idAnagrafica, out datiDetrazioni);
                            risposta = new RispostaDetrazioni();
                            risposta.Esito = TipoRitornoDetrazioni.NessunErrore;
                            risposta.Detrazioni = new GestioneDetrazioniImposta.DatiDetrazioni(datiDetrazioni.DetrazioniReddito, datiDetrazioni.AgevolazionePensionati, datiDetrazioni.ConiugeOFiglio,
                                datiDetrazioni.FigliMinori3AnniNoHandicap100, datiDetrazioni.FigliMinori3AnniNoHandicap50, datiDetrazioni.FigliMinori3AnniHandicap100,
                                datiDetrazioni.FigliMinori3AnniHandicap50, datiDetrazioni.FigliMaggiori3AnniNoHandicap100, datiDetrazioni.FigliMaggiori3AnniNoHandicap50,
                                datiDetrazioni.FigliMaggiori3AnniHandicap100, datiDetrazioni.FigliMaggiori3AnniHandicap50, datiDetrazioni.AltriFamiliari100, datiDetrazioni.AltriFamiliari50,
                                datiDetrazioni.AddizionaleLombardiaVeneto, datiDetrazioni.NonResidenteSchumacker, datiDetrazioni.ConvDoppieImposizioni, datiDetrazioni.DecorrenzaDetrazioneImposte);
                        }
                        else
                        {
                            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = null;
                            GestioneDetrazioniImposta.GetDetrazioniByIdPensione(datiPensione.Id, out datiDetrazioni);
                            risposta = new RispostaDetrazioni();
                            risposta.Esito = TipoRitornoDetrazioni.NessunErrore;
                            risposta.Detrazioni = datiDetrazioni;
                        }
                        break;

                    default:
                        RichiestaDetrazioni richiesta = null;
                        ValorizzaRichiestaByDatiPensione(datiPensione, codiceFiscale, out richiesta);
                        if (richiesta == null)
                        {
                            errori = "Domanda non presente";
                            return false;
                        }

                        if (!GetDetrazioniUniDetra(richiesta, out risposta, out errori, tipoAppartenenza))
                            return false;

                        string url = "";
                        RecuperaUrlUniDetra(richiesta, risposta != null && risposta.Detrazioni != null, out url);
                        risposta.Url = url;

                        break;
                }
            }
            catch (Exception Ex)
            {
                errori = "Errore nella chiamata al servizio Detrazioni: " + Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }

        public static bool VerificaDetrazioniByDatiPensione(GestionePensione.DatiPensione datiPensione, string codiceFiscale, long idAnagrafica, bool isContitolare,
            GestioneDetrazioniImposta.DatiDetrazioni detrazione, bool isSemaforoVerde, out RispostaDetrazioni ultimaDetrazione, out string errori)
        {
            ultimaDetrazione = null;
            errori = "";
            try
            {
                RichiestaDetrazioni richiesta = null;
                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                ValorizzaRichiestaByDatiPensione(datiPensione, codiceFiscale, out richiesta);
                if (richiesta == null)
                {
                    errori = "Domanda non presente";
                    return false;
                }

                if (!VerificaDetrazioni(richiesta, detrazione, out ultimaDetrazione, out errori, tipoAppartenenza))
                    return false;

                string url = "";
                RecuperaUrlUniDetra(richiesta, ultimaDetrazione != null && ultimaDetrazione.Detrazioni != null, out url);
                ultimaDetrazione.Url = url;

                if (ultimaDetrazione.Esito != TipoRitornoDetrazioni.Errore)
                    SalvaDetrazioni(datiPensione.Id, idAnagrafica, isContitolare, isSemaforoVerde, ultimaDetrazione.Detrazioni);
            }
            catch (Exception Ex)
            {
                errori = "Errore nella verifica delle detrazioni per numero domanda: " + Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }

        public static bool GetElencoSoggettiByDatiPensione(GestionePensione.DatiPensione datiPensione, out List<Soggetto> elencoSoggetti, out string errori)
        {
            errori = string.Empty;
            elencoSoggetti = null;

            try
            {
                GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
                GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);

                GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioniTitolare = null;
                GestioneDetrazioniImposta.GetDetrazioniByIdPensione(datiPensione.Id, out datiDetrazioniTitolare);

                GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamicoSpacchettate024);

                //ENG - REVERSIBILITA FS (NO INPDAP/024)   
                List<GestioneFamiliari.CodMaggFamiliari> listaMaggiorazioniFamiliari = null;
                GestioneFamiliari.GetCodMaggiorazioneFamiliariByIdPensione(datiPensione.Id, out listaMaggiorazioniFamiliari);

                elencoSoggetti = new List<Soggetto>();
                Soggetto soggettoTitolare = new Soggetto();
                Utility.ValorizzaOggetti(datiAnagraficiTitolare, soggettoTitolare);
                soggettoTitolare.IdAnagrafica = datiAnagraficiTitolare.Id;
                soggettoTitolare.Confermato = datiDetrazioniTitolare != null;
                elencoSoggetti.Add(soggettoTitolare);

                if (Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == Utility.TipoAppartenenza.FS && Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) && !Utility.IsDomandaSpacchettamentoINPDAP(datiPensione)
                    && !(controlloDinamicoSpacchettate024 != null && !String.IsNullOrEmpty(controlloDinamicoSpacchettate024.ValoreControllo) && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id))))
                {
                    List<GestioneFamiliari.Familiare> listaContitolari = null;
                    List<GestioneAnagrafica.DatiAnagrafici> listaAnagrafiche = null;
                    GestioneFamiliari.GetFamiliariByIdPensione(datiPensione.Id, out listaContitolari, out listaAnagrafiche);

                    List<GestioneDetrazioniContitolare.DatiDetrazioniContitolare> listaDatiDetrazioniContitolare = null;
                    GestioneDetrazioniContitolare.GetDetrazioniByIdPensione(datiPensione.Id, out listaDatiDetrazioniContitolare);

                    if (listaAnagrafiche != null && listaAnagrafiche.Count > 0)
                        foreach (var anag in listaAnagrafiche)
                        {
                            if (!elencoSoggetti.Exists(x => x.IdAnagrafica == anag.Id))
                            {
                                Soggetto soggettoContitolare = new Soggetto();
                                Utility.ValorizzaOggetti(anag, soggettoContitolare);
                                soggettoContitolare.IdAnagrafica = anag.Id;

                                //ENG - REVERSIBILITA FS (NO INPDAP/024)   
                                soggettoContitolare.IsContitolare = true;
                                if (listaMaggiorazioniFamiliari != null && listaMaggiorazioniFamiliari.Exists(x => x.IdAnagrafica == soggettoContitolare.IdAnagrafica && x.Cessazione.HasValue))
                                {
                                    soggettoContitolare.DataCessazione = listaMaggiorazioniFamiliari.FindAll(x => x.IdAnagrafica == soggettoContitolare.IdAnagrafica && x.Cessazione.HasValue).OrderByDescending(x => x.Cessazione).First().Cessazione;
                                }

                                elencoSoggetti.Add(soggettoContitolare);
                            }
                        }
                    foreach (var detrazione in listaDatiDetrazioniContitolare)
                    {
                        Soggetto soggetto = elencoSoggetti.FirstOrDefault(x => x.IdAnagrafica == detrazione.IdAnagrafica);
                        if (soggetto != null)
                            soggetto.Confermato = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                errori = "Errore nella chiamata al servizio Detrazioni: " + Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }
        #endregion public members

        #region internal members
        internal static bool GetDetrazioniUniDetra(RichiestaDetrazioni richiesta, out RispostaDetrazioni risposta, out string errori, Utility.TipoAppartenenza? tipoAppartenenza)
        {
            risposta = null;
            errori = "";
            try
            {
                if (richiesta == null)
                {
                    errori = "Area richiesta delle detrazioni non valorizzata";
                    return false;
                }

                if (!GetDetrazioniFromSrvDetrazioniUniDetra(richiesta, out risposta, out errori, tipoAppartenenza))
                    return false;
                //controllo esito
                if (risposta.Esito != TipoRitornoDetrazioni.NessunErrore)
                    return true;

                risposta.MessaggioRitorno = "";
            }
            catch (Exception Ex)
            {
                errori = "Errore tecnico nel recupero delle detrazioni";
                string messaggio = string.Format("Errore nel recupero delle detrazioni per numero domanda: {0}", Utility.GetMessageFromException(Ex));
                string parametri = null;
                try
                {
                    parametri = Utility.GetXmlFromObject(richiesta);
                }
                catch (Exception)
                {
                    // Eccezione ignorata
                }
                GestioneLogGenerico.SalvaLogGenerico(richiesta != null ? richiesta.NDomus : 0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, Ex.StackTrace);
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }

        internal static bool VerificaDetrazioni(RichiestaDetrazioni richiesta, BLCommon.GestioneDetrazioniImposta.DatiDetrazioni detrazione, out RispostaDetrazioni ultimaDetrazione, out string errori,
            Utility.TipoAppartenenza? tipoAppartenenza)
        {
            errori = "";
            ultimaDetrazione = null;
            try
            {
                if (!GetDetrazioniUniDetra(richiesta, out ultimaDetrazione, out errori, tipoAppartenenza))
                    return false;

                if (ultimaDetrazione.Esito == TipoRitornoDetrazioni.Errore)
                    return true;
                //if (!detrazione.Equals(ultimaDetrazione.Detrazioni))
                if (!Utility.ConfrontaOggetti(detrazione, ultimaDetrazione.Detrazioni))
                {
                    ultimaDetrazione.Esito = TipoRitornoDetrazioni.Errore;
                    ultimaDetrazione.MessaggioRitorno = "Salvataggio non avvenuto. Effettuare l'aggiornamento e salvare.";
                }
            }
            catch (Exception Ex)
            {
                errori = "Errore nella chiamata al servizio Detrazioni: " + Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }
        #endregion internal members

        #region private members
        private static bool GetDetrazioniFromSrvDetrazioniUniDetra(RichiestaDetrazioni richiesta, out RispostaDetrazioni risposta, out string errori,
            Utility.TipoAppartenenza? tipoAppartenenza)
        {
            bool erroreTecnico = false;
            risposta = null;
            errori = "";
            string stackTrace = null;

            DetrazioniClient proxy = new DetrazioniClient();
            EsitoRicercaDetrazione esito = null;
            Guid guid = Guid.NewGuid();

            using (new MethodExecutionTracer())
            {
                try
                {
                    RichiestaRicercaDetrazione richiestaRicerca = new RichiestaRicercaDetrazione
                    {
                        //conversione oggetto dell'applicativo in oggetto del servizio per valorizzazione  della richiesta
                        CodiceFiscale = richiesta.CodiceFiscale,
                        Decorrenza = richiesta.AnnoFiscale,
                        Sicurezza = new Sicurezza()
                    };
                    richiestaRicerca.Sicurezza.CsAppKey = richiesta.CsAppKey;
                    richiestaRicerca.Sicurezza.CsAppName = richiesta.CsAppName;
                    //opzionale
                    //richiestaRicerca.Sicurezza.Username = richiesta.CodiceFiscale;

                    GestioneLogSoap.SalvaLogSoap(richiestaRicerca, Utility.Servizio.SrvUniDetra, Utility.MetodoServizio.Ricerca, Utility.SOAPLogDirection.IN, richiesta.NDomus.ToString(), guid);

                    esito = proxy.Ricerca(richiestaRicerca);

                    try
                    {
                        risposta = new RispostaDetrazioni();

                        switch (esito.Stato)
                        {

                            case WSStato.NoPresente:
                            case WSStato.OK:

                                if (esito.Titolare == null)
                                {
                                    ControlliCategoriePensione(richiesta, out risposta);
                                    return true;
                                }
                                else
                                {
                                    risposta = new RispostaDetrazioni();
                                    risposta.Esito = TipoRitornoDetrazioni.NessunErrore;
                                    if (esito.Messaggi != null && esito.Messaggi.Length > 0)
                                        risposta.MessaggioRitorno = string.Join("; ", esito.Messaggi);
                                }
                                break;

                            case WSStato.AutFallita:
                                risposta.Esito = TipoRitornoDetrazioni.Errore;
                                risposta.MessaggioRitorno = "Autenticazione fallita. ";
                                if (esito.Messaggi != null && esito.Messaggi.Length > 0)
                                    risposta.MessaggioRitorno += string.Join("; ", esito.Messaggi);
                                return true;

                            case WSStato.AutInsufficienti:
                                risposta.Esito = TipoRitornoDetrazioni.Errore;
                                risposta.MessaggioRitorno = "Dati per l'identificazione non sufficienti. ";
                                if (esito.Messaggi != null && esito.Messaggi.Length > 0)
                                    risposta.MessaggioRitorno += string.Join("; ", esito.Messaggi);
                                return true;

                            case WSStato.PosizioneInvalida:
                            case WSStato.ErrGenerico:
                            case WSStato.ND:
                            default:

                                risposta.MessaggioRitorno = "Si è verificato un errore. ";
                                if (esito.Messaggi != null && esito.Messaggi.Length > 0)
                                    risposta.MessaggioRitorno += string.Join("; ", esito.Messaggi);
                                risposta.Esito = TipoRitornoDetrazioni.Errore;
                                return true;
                        }
                    }
                    catch (Exception)
                    {
                        risposta.Esito = TipoRitornoDetrazioni.Errore;
                        risposta.MessaggioRitorno = "Mancano parametri di input";
                        return true;
                    }

                    FormattaDetrazioniNewUniDetra(esito, risposta, richiesta, tipoAppartenenza);
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio UniDetra | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio UniDetra | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio UniDetra | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio UniDetra: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico nel recupero delle detrazioni";
                        string parametri = null;
                        try
                        {
                            parametri = Utility.GetXmlFromObject(richiesta);
                        }
                        catch (Exception)
                        {
                            // Eccezione ignorata
                        }
                        GestioneLogGenerico.SalvaLogGenerico(richiesta != null ? richiesta.NDomus : 0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(esito, Utility.Servizio.SrvUniDetra, Utility.MetodoServizio.Ricerca, Utility.SOAPLogDirection.OUT, richiesta.NDomus.ToString(), guid);
                    Utility.CloseClient(proxy);
                }
                return true;
            }
        }

        private static void FormattaDetrazioniNewUniDetra(EsitoRicercaDetrazione esito, RispostaDetrazioni risposta, RichiestaDetrazioni richiesta,
            Utility.TipoAppartenenza? tipoAppartenenza)
        {
            if (esito != null && esito.Stato == WSStato.OK)
            {
                DateTime? decorrenza = Utility.DataFromInt(richiesta.AnnoFiscale, 1, 1);
                if (esito.Titolare != null && esito.Titolare.Presentazione != null)
                    decorrenza = esito.Titolare.Presentazione.Data;

                // le detrazioni vengono impostate tutte a zero per unificare la visualizzazione sull'applicazione LIQPENS
                risposta.Detrazioni = new GestioneDetrazioniImposta.DatiDetrazioni(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, decorrenza);

                if (esito.Titolare != null)
                {
                    #region 1 byte
                    if (esito.Titolare.DetrazioneArt13 != null &&
                        esito.Titolare.DetrazioneArt13.NoApplicazioneArt13)
                        risposta.Detrazioni.DetrazioniReddito = 0;
                    else
                        risposta.Detrazioni.DetrazioniReddito = 1;
                    #endregion 1 byte

                    #region 2 byte
                    if (esito.Titolare.DetrazioneArt11 != null &&
                        esito.Titolare.DetrazioneArt11.NoApplicazioneArt11)
                        risposta.Detrazioni.AgevolazionePensionati = 0;
                    else
                        risposta.Detrazioni.AgevolazionePensionati = 1;
                    #endregion 2 byte

                    if (esito.Titolare.NucleoFamiliare != null)
                    {
                        //coniugi e primi figli
                        //ENG - Il campo "ConiugeOFiglio" non deve essere impostato ad 1 se il campo "NoApplicazioneArt12" è true
                        #region 3 byte
                        risposta.Detrazioni.ConiugeOFiglio = 0;
                        if (esito.Titolare.NucleoFamiliare.Coniugi != null && esito.Titolare.NucleoFamiliare.Coniugi.Exists(x => !string.IsNullOrEmpty(x.CodFiscale))
                            && !(tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO && esito.Titolare.DetrazioneArt12 != null && esito.Titolare.DetrazioneArt12.NoApplicazioneArt12))
                            risposta.Detrazioni.ConiugeOFiglio = 1;
                        else if (esito.Titolare.NucleoFamiliare.PrimiFigli != null && esito.Titolare.NucleoFamiliare.PrimiFigli.FindAll(x => x.AssenzaConiuge).Count > 0)
                        {
                            if (esito.Titolare.NucleoFamiliare.PrimiFigli.Exists(x => x.AssenzaConiuge && x.MesiMinore == 0 && !x.Disabile))
                                risposta.Detrazioni.ConiugeOFiglio = 2;
                            else if (esito.Titolare.NucleoFamiliare.PrimiFigli.Exists(x => x.AssenzaConiuge && x.MesiMinore == 0 && x.Disabile))
                                risposta.Detrazioni.ConiugeOFiglio = 3;
                            else if (esito.Titolare.NucleoFamiliare.PrimiFigli.Exists(x => x.AssenzaConiuge && x.MesiMinore > 0 && !x.Disabile))
                                risposta.Detrazioni.ConiugeOFiglio = 4;
                            else if (esito.Titolare.NucleoFamiliare.PrimiFigli.Exists(x => x.AssenzaConiuge && x.MesiMinore > 0 && x.Disabile))
                                risposta.Detrazioni.ConiugeOFiglio = 5;
                        }
                        #endregion 3 byte

                        // altri figli minori 3 anni
                        #region 4 byte
                        // MesiMinore > 0, Disabile = false, Perc = 100
                        if ((esito.Titolare.NucleoFamiliare.AltriFigli != null && esito.Titolare.NucleoFamiliare.AltriFigli.Exists(x => x.MesiMinore > 0 && !x.Disabile && x.Perc == 100)) ||
                            (esito.Titolare.NucleoFamiliare.PrimiFigli != null && esito.Titolare.NucleoFamiliare.PrimiFigli.Exists(x => !x.AssenzaConiuge && x.MesiMinore > 0 && !x.Disabile && x.Perc == 100)))
                            risposta.Detrazioni.FigliMinori3AnniNoHandicap100 = (byte)((esito.Titolare.NucleoFamiliare.AltriFigli != null ? esito.Titolare.NucleoFamiliare.AltriFigli.Count(x => x.MesiMinore > 0 && !x.Disabile && x.Perc == 100) : 0) +
                                (esito.Titolare.NucleoFamiliare.PrimiFigli != null ? esito.Titolare.NucleoFamiliare.PrimiFigli.Count(x => !x.AssenzaConiuge && x.MesiMinore > 0 && !x.Disabile && x.Perc == 100) : 0));
                        else
                            risposta.Detrazioni.FigliMinori3AnniNoHandicap100 = 0;
                        #endregion 4 byte

                        #region 5 byte
                        // MesiMinore > 0, Disabile = false, Perc = 50
                        if ((esito.Titolare.NucleoFamiliare.AltriFigli != null && esito.Titolare.NucleoFamiliare.AltriFigli.Exists(x => x.MesiMinore > 0 && !x.Disabile && x.Perc == 50)) ||
                            (esito.Titolare.NucleoFamiliare.PrimiFigli != null && esito.Titolare.NucleoFamiliare.PrimiFigli.Exists(x => !x.AssenzaConiuge && x.MesiMinore > 0 && !x.Disabile && x.Perc == 50)))
                            risposta.Detrazioni.FigliMinori3AnniNoHandicap50 = (byte)((esito.Titolare.NucleoFamiliare.AltriFigli != null ? esito.Titolare.NucleoFamiliare.AltriFigli.Count(x => x.MesiMinore > 0 && !x.Disabile && x.Perc == 50) : 0) +
                                (esito.Titolare.NucleoFamiliare.PrimiFigli != null ? esito.Titolare.NucleoFamiliare.PrimiFigli.Count(x => !x.AssenzaConiuge && x.MesiMinore > 0 && !x.Disabile && x.Perc == 50) : 0));
                        else
                            risposta.Detrazioni.FigliMinori3AnniNoHandicap50 = 0;
                        #endregion 5 byte

                        #region 6 byte
                        // MesiMinore > 0, Disabile = true, Perc = 100
                        if ((esito.Titolare.NucleoFamiliare.AltriFigli != null && esito.Titolare.NucleoFamiliare.AltriFigli.Exists(x => x.MesiMinore > 0 && x.Disabile && x.Perc == 100)) ||
                            (esito.Titolare.NucleoFamiliare.PrimiFigli != null && esito.Titolare.NucleoFamiliare.PrimiFigli.Exists(x => !x.AssenzaConiuge && x.MesiMinore > 0 && x.Disabile && x.Perc == 100)))
                            risposta.Detrazioni.FigliMinori3AnniHandicap100 = (byte)((esito.Titolare.NucleoFamiliare.AltriFigli != null ? esito.Titolare.NucleoFamiliare.AltriFigli.Count(x => x.MesiMinore > 0 && x.Disabile && x.Perc == 100) : 0) +
                                (esito.Titolare.NucleoFamiliare.PrimiFigli != null ? esito.Titolare.NucleoFamiliare.PrimiFigli.Count(x => !x.AssenzaConiuge && x.MesiMinore > 0 && x.Disabile && x.Perc == 100) : 0));
                        else
                            risposta.Detrazioni.FigliMinori3AnniHandicap100 = 0;
                        #endregion 6 byte

                        #region 7 byte
                        // MesiMinore > 0, Disabile = true, Perc = 50
                        if ((esito.Titolare.NucleoFamiliare.AltriFigli != null && esito.Titolare.NucleoFamiliare.AltriFigli.Exists(x => x.MesiMinore > 0 && x.Disabile && x.Perc == 50)) ||
                            (esito.Titolare.NucleoFamiliare.PrimiFigli != null && esito.Titolare.NucleoFamiliare.PrimiFigli.Exists(x => !x.AssenzaConiuge && x.MesiMinore > 0 && x.Disabile && x.Perc == 50)))
                            risposta.Detrazioni.FigliMinori3AnniHandicap50 = (byte)((esito.Titolare.NucleoFamiliare.AltriFigli != null ? esito.Titolare.NucleoFamiliare.AltriFigli.Count(x => x.MesiMinore > 0 && x.Disabile && x.Perc == 50) : 0) +
                                (esito.Titolare.NucleoFamiliare.PrimiFigli != null ? esito.Titolare.NucleoFamiliare.PrimiFigli.Count(x => !x.AssenzaConiuge && x.MesiMinore > 0 && x.Disabile && x.Perc == 50) : 0));
                        else
                            risposta.Detrazioni.FigliMinori3AnniHandicap50 = 0;
                        #endregion 7 byte

                        // altri figli maggiori 3 anni
                        #region 8 byte
                        // MesiMinore == 0, Disabile = false, Perc = 100
                        if ((esito.Titolare.NucleoFamiliare.AltriFigli != null && esito.Titolare.NucleoFamiliare.AltriFigli.Exists(x => x.MesiMinore == 0 && !x.Disabile && x.Perc == 100)) ||
                            (esito.Titolare.NucleoFamiliare.PrimiFigli != null && esito.Titolare.NucleoFamiliare.PrimiFigli.Exists(x => !x.AssenzaConiuge && x.MesiMinore == 0 && !x.Disabile && x.Perc == 100)))
                            risposta.Detrazioni.FigliMaggiori3AnniNoHandicap100 = (byte)((esito.Titolare.NucleoFamiliare.AltriFigli != null ? esito.Titolare.NucleoFamiliare.AltriFigli.Count(x => x.MesiMinore == 0 && !x.Disabile && x.Perc == 100) : 0) +
                                (esito.Titolare.NucleoFamiliare.PrimiFigli != null ? esito.Titolare.NucleoFamiliare.PrimiFigli.Count(x => !x.AssenzaConiuge && x.MesiMinore == 0 && !x.Disabile && x.Perc == 100) : 0));
                        else
                            risposta.Detrazioni.FigliMaggiori3AnniNoHandicap100 = 0;
                        #endregion 8 byte

                        #region 9 byte
                        // MesiMinore == 0, Disabile = false, Perc = 50
                        if ((esito.Titolare.NucleoFamiliare.AltriFigli != null && esito.Titolare.NucleoFamiliare.AltriFigli.Exists(x => x.MesiMinore == 0 && !x.Disabile && x.Perc == 50)) ||
                            (esito.Titolare.NucleoFamiliare.PrimiFigli != null && esito.Titolare.NucleoFamiliare.PrimiFigli.Exists(x => !x.AssenzaConiuge && x.MesiMinore == 0 && !x.Disabile && x.Perc == 50)))
                            risposta.Detrazioni.FigliMaggiori3AnniNoHandicap50 = (byte)((esito.Titolare.NucleoFamiliare.AltriFigli != null ? esito.Titolare.NucleoFamiliare.AltriFigli.Count(x => x.MesiMinore == 0 && !x.Disabile && x.Perc == 50) : 0) +
                                (esito.Titolare.NucleoFamiliare.PrimiFigli != null ? esito.Titolare.NucleoFamiliare.PrimiFigli.Count(x => !x.AssenzaConiuge && x.MesiMinore == 0 && !x.Disabile && x.Perc == 50) : 0));
                        else
                            risposta.Detrazioni.FigliMaggiori3AnniNoHandicap50 = 0;
                        #endregion 9 byte

                        #region 10 byte
                        // MesiMinore == 0, Disabile = true, Perc = 100
                        if ((esito.Titolare.NucleoFamiliare.AltriFigli != null && esito.Titolare.NucleoFamiliare.AltriFigli.Exists(x => x.MesiMinore == 0 && x.Disabile && x.Perc == 100)) ||
                            (esito.Titolare.NucleoFamiliare.PrimiFigli != null && esito.Titolare.NucleoFamiliare.PrimiFigli.Exists(x => !x.AssenzaConiuge && x.MesiMinore == 0 && x.Disabile && x.Perc == 100)))
                            risposta.Detrazioni.FigliMaggiori3AnniHandicap100 = (byte)((esito.Titolare.NucleoFamiliare.AltriFigli != null ? esito.Titolare.NucleoFamiliare.AltriFigli.Count(x => x.MesiMinore == 0 && x.Disabile && x.Perc == 100) : 0) +
                                (esito.Titolare.NucleoFamiliare.PrimiFigli != null ? esito.Titolare.NucleoFamiliare.PrimiFigli.Count(x => !x.AssenzaConiuge && x.MesiMinore == 0 && x.Disabile && x.Perc == 100) : 0));
                        else
                            risposta.Detrazioni.FigliMaggiori3AnniHandicap100 = 0;
                        #endregion 10 byte

                        #region 11 byte
                        // MesiMinore == 0, Disabile = true, Perc = 50
                        if ((esito.Titolare.NucleoFamiliare.AltriFigli != null && esito.Titolare.NucleoFamiliare.AltriFigli.Exists(x => x.MesiMinore == 0 && x.Disabile && x.Perc == 50)) ||
                            (esito.Titolare.NucleoFamiliare.PrimiFigli != null && esito.Titolare.NucleoFamiliare.PrimiFigli.Exists(x => !x.AssenzaConiuge && x.MesiMinore == 0 && x.Disabile && x.Perc == 50)))
                            risposta.Detrazioni.FigliMaggiori3AnniHandicap50 = (byte)((esito.Titolare.NucleoFamiliare.AltriFigli != null ? esito.Titolare.NucleoFamiliare.AltriFigli.Count(x => x.MesiMinore == 0 && x.Disabile && x.Perc == 50) : 0) +
                                (esito.Titolare.NucleoFamiliare.PrimiFigli != null ? esito.Titolare.NucleoFamiliare.PrimiFigli.Count(x => !x.AssenzaConiuge && x.MesiMinore == 0 && x.Disabile && x.Perc == 50) : 0));
                        else
                            risposta.Detrazioni.FigliMaggiori3AnniHandicap50 = 0;
                        #endregion 11 byte

                        if (esito.Titolare.NucleoFamiliare.AltriFamiliari != null)
                        {
                            #region 12 byte
                            if (esito.Titolare.NucleoFamiliare.AltriFamiliari.Exists(x => x.Perc == 100))
                                risposta.Detrazioni.AltriFamiliari100 = (byte)esito.Titolare.NucleoFamiliare.AltriFamiliari.Count(x => x.Perc == 100);
                            else
                                risposta.Detrazioni.AltriFamiliari100 = 0;
                            #endregion 12 byte

                            #region 13 byte
                            if (esito.Titolare.NucleoFamiliare.AltriFamiliari.Exists(x => x.Perc == 50))
                                risposta.Detrazioni.AltriFamiliari50 = (byte)esito.Titolare.NucleoFamiliare.AltriFamiliari.Count(x => x.Perc == 50);
                            else
                                risposta.Detrazioni.AltriFamiliari50 = 0;
                            #endregion 13 byte
                        }
                    }

                    if (esito.Titolare.AltreDetrazioni != null)
                    {
                        #region 14 byte
                        if (esito.Titolare.AltreDetrazioni.CasiParticolari == true)
                            risposta.Detrazioni.AddizionaleLombardiaVeneto = 1;
                        else
                            risposta.Detrazioni.AddizionaleLombardiaVeneto = 0;
                        #endregion 14 byte

                        #region 15 byte nuovo
                        risposta.Detrazioni.NonResidenteSchumacker = esito.Titolare.AltreDetrazioni.NonResidenteSchumacker ? (byte)1 : (byte)0;
                        #endregion 15 byte nuovo

                        #region 16 byte nuovo
                        risposta.Detrazioni.ConvDoppieImposizioni = esito.Titolare.AltreDetrazioni.ConvDoppieImposizioni ? (byte)1 : (byte)0;
                        #endregion 16 byte nuovo
                    }

                    risposta.Esito = TipoRitornoDetrazioni.NessunErrore;
                    return;
                }
            }

            else
            {
                risposta.MessaggioRitorno = "Non esistono detrazioni associate al soggetto.";
                risposta.Esito = TipoRitornoDetrazioni.Informativa;
                return;
            }
        }

        private static void ValorizzaRichiestaByDatiPensione(GestionePensione.DatiPensione datiPensione, string codiceFiscale, out RichiestaDetrazioni richiesta)
        {
            richiesta = null;

            if (datiPensione == null)
                return;

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            richiesta = new RichiestaDetrazioni();
            richiesta.NDomus = datiPensione.NDomus;
            richiesta.DataDecorrenza = datiPensione.DecorrenzaOriginaria;
            string categoriaNumerica = datiPensione.GetCodCategoria();
            richiesta.CategoriaPensione = short.Parse(categoriaNumerica);
            richiesta.SedePensione = datiPensione.CodiceSede;
            richiesta.CertificatoPensione = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value : 0;

            richiesta.CodiceFiscale = codiceFiscale;

            int annoCompetenza = 0;
            GestioneControlliDinamici.GetAnnoCompetenza(tipoAppartenenza, out annoCompetenza);
            richiesta.AnnoFiscale = annoCompetenza;

            richiesta.CodiceProcedura = 103;

            richiesta.CsAppName = INPS.DNA.Context.ApplicationInfo.Name;
            richiesta.CsAppKey = ConfigurationManager.AppSettings["identityKey"];
        }

        private static void SalvaDetrazioni(long idPensione, long idAnagrafica, bool isContitolare, bool isSemaforoVerde, BLCommon.GestioneDetrazioniImposta.DatiDetrazioni detrazioni)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                if (isContitolare)
                {
                    GestioneDetrazioniContitolare.DatiDetrazioniContitolare detrazioniContitolare = new GestioneDetrazioniContitolare.DatiDetrazioniContitolare(idAnagrafica, detrazioni.DetrazioniReddito,
                        detrazioni.AgevolazionePensionati, detrazioni.ConiugeOFiglio, detrazioni.FigliMinori3AnniNoHandicap100, detrazioni.FigliMinori3AnniNoHandicap50, detrazioni.FigliMinori3AnniHandicap100,
                        detrazioni.FigliMinori3AnniHandicap50, detrazioni.FigliMaggiori3AnniNoHandicap100, detrazioni.FigliMaggiori3AnniNoHandicap50, detrazioni.FigliMaggiori3AnniHandicap100,
                        detrazioni.FigliMaggiori3AnniHandicap50, detrazioni.AltriFamiliari100, detrazioni.AltriFamiliari50, detrazioni.AddizionaleLombardiaVeneto, detrazioni.NonResidenteSchumacker,
                        detrazioni.ConvDoppieImposizioni, detrazioni.DecorrenzaDetrazioneImposte);
                    GestioneDetrazioniContitolare.SalvaDetrazioni(idPensione, idAnagrafica, detrazioniContitolare);
                }
                else
                    GestioneDetrazioniImposta.SalvaDetrazioni(idPensione, detrazioni);

                GestioneQuadri.DatiQuadroDetrazioni datiQuadroDetrazioni = new GestioneQuadri.DatiQuadroDetrazioni();
                if (isSemaforoVerde)
                    datiQuadroDetrazioni.TabDetrazioni = 2;
                GestioneQuadri.SalvaQuadroDetrazioni(idPensione, datiQuadroDetrazioni);

                transactionScope.Complete();
            }
        }

        private static void ControlliCategoriePensione(RichiestaDetrazioni richiesta, out RispostaDetrazioni risposta)
        {
            risposta = new RispostaDetrazioni();
            if (richiesta.CategoriaPensione == 44 || richiesta.CategoriaPensione == 77 ||
                                  richiesta.CategoriaPensione == 78 || richiesta.CategoriaPensione == 32 ||
                                  richiesta.CategoriaPensione == 33 || richiesta.CategoriaPensione == 34)
            {
                risposta.MessaggioRitorno = "Non esistono detrazioni associate al soggetto.";
                risposta.Esito = TipoRitornoDetrazioni.Informativa;
            }

            else
            {
                risposta.MessaggioRitorno = "Non esistono detrazioni associate al soggetto. E' necessario acquisirle.";
                risposta.Esito = TipoRitornoDetrazioni.Errore;
            }
        }

        private static void RecuperaUrlUniDetra(RichiestaDetrazioni richiesta, bool isDetrazioniRecuperate, out string url)
        {
            url = "";
            if (ConfigurationManager.AppSettings["UrlDetrazioni"] != null)
            {
                url = ConfigurationManager.AppSettings["UrlDetrazioni"];
                url += "?codfiscale=" + richiesta.CodiceFiscale;
                url += "&esistente=" + isDetrazioniRecuperate;
                url += "&decorrenza=" + richiesta.AnnoFiscale;
            }
        }

        #endregion private members

        #region nested class
        public class RichiestaDetrazioni
        {
            public RichiestaDetrazioni()
            { }
            public RichiestaDetrazioni(short codiceProcedura, string codiceFiscale, int annoFiscale, short categoriaPensione, short sedePensione, Int32 certificatoPensione, DateTime? dataDecorrenza)
            {
                this._CodiceProcedura = codiceProcedura;
                this._CodiceFiscale = codiceFiscale;
                this._AnnoFiscale = annoFiscale;
                this._CategoriaPensione = categoriaPensione;
                this._DataDecorrenza = dataDecorrenza;
                this._SedePensione = sedePensione;
                this._CertificatoPensione = certificatoPensione;
            }
            #region private properties
            private long _NDomus;
            private short _CodiceProcedura;
            private string _CodiceFiscale;
            private int _AnnoFiscale;
            private short _CategoriaPensione;
            private short _SedePensione;
            private Int32 _CertificatoPensione;
            private System.Nullable<DateTime> _DataDecorrenza;
            private string _CsAppName;
            private string _CsAppKey;
            #endregion private properties

            #region public properties
            public long NDomus { get { return _NDomus; } set { _NDomus = value; } }
            public short CodiceProcedura { get { return _CodiceProcedura; } set { _CodiceProcedura = value; } }
            public string CodiceFiscale { get { return _CodiceFiscale; } set { _CodiceFiscale = value; } }
            public int AnnoFiscale { get { return _AnnoFiscale; } set { _AnnoFiscale = value; } }
            public short CategoriaPensione { get { return _CategoriaPensione; } set { _CategoriaPensione = value; } }
            public short SedePensione { get { return _SedePensione; } set { _SedePensione = value; } }
            public Int32 CertificatoPensione { get { return _CertificatoPensione; } set { _CertificatoPensione = value; } }
            public System.Nullable<DateTime> DataDecorrenza { get { return _DataDecorrenza; } set { _DataDecorrenza = value; } }
            public string CsAppName { get { return _CsAppName; } set { _CsAppName = value; } }
            public string CsAppKey { get { return _CsAppKey; } set { _CsAppKey = value; } }
            #endregion public properties
        }

        public class RispostaDetrazioni
        {
            public RispostaDetrazioni()
            {
                this._ElencoDetrazioni = new List<string>();
            }

            #region private properties
            private string _Url;
            private string _MessaggioRitorno;
            private List<string> _ElencoDetrazioni;
            private BLCommon.GestioneDetrazioniImposta.DatiDetrazioni _Detrazioni;
            private TipoRitornoDetrazioni _Esito;
            #endregion private properties

            #region public properties
            public string Url { get { return _Url; } set { _Url = value; } }
            public string MessaggioRitorno { get { return _MessaggioRitorno; } set { _MessaggioRitorno = value; } }
            internal List<string> ElencoDetrazioni { get { return _ElencoDetrazioni; } set { _ElencoDetrazioni = value; } }
            public BLCommon.GestioneDetrazioniImposta.DatiDetrazioni Detrazioni { get { return _Detrazioni; } set { _Detrazioni = value; } }
            public TipoRitornoDetrazioni Esito { get { return _Esito; } set { _Esito = value; } }
            #endregion public properties
        }

        public class Soggetto
        {
            #region public data member
            public long IdAnagrafica { get; set; }

            public string CodiceFiscale { get; set; }

            public string Cognome { get; set; }

            public string Nome { get; set; }

            public DateTime? DataNascita { get; set; }

            public bool Confermato { get; set; }

            //ENG - REVERSIBILITA FS (NO INPDAP/024)   
            public bool IsContitolare { get; set; }

            public DateTime? DataCessazione { get; set; }

            #endregion public data member
        }

        public enum TipoRitornoDetrazioni
        {
            NessunErrore,
            Errore,
            Informativa
        };
        #endregion nested class
    }
}
