using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.LiquidazioneAgo.Entity;
using INPS.Pensioni.LiquidazioneAgo.Service.Contracts.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using EntityBLCommon = INPS.Pensioni.Liquidazione.BLCommon.Entity;

namespace INPS.Pensioni.LiquidazioneAgo.Service
{
    [INPS.DNA.Exceptions.Services.ExceptionShielding]
    public class ServizioLiquidazioneAgo : INPS.DNA.Services.ServiceBase, IServizioLiquidazioneAgo
    {
        #region Culture
        private static void SetCulture()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("it-IT");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("it-IT");
        }
        #endregion Culture

        #region Get Dati Pensione

        public long GetIdPensioneByNumeroDomanda(long numeroDomanda, byte? progStorico)
        {
            long IdPensione = 0;
            GestionePensione.GetIdPensioneByNumeroDomanda(numeroDomanda, progStorico, out IdPensione);
            return IdPensione;
        }

        public GestionePensione.DatiPensione GetDatiPensioneByNumeroDomanda(long numeroDomanda, byte? progStorico)
        {
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, progStorico, out datiPensione);
            return datiPensione;
        }

        #endregion Get Dati Pensione

        #region Versioni
        public AreaEsito GetListaVersioniAGO(out AreaVersioni areaVersioni)
        {
            SetCulture();

            areaVersioni = new AreaVersioni();
            areaVersioni.ListaVersioni = new Dictionary<string, string>();
            AreaEsito esito = new AreaEsito();

            try
            {
                List<GestioneVersioni.DatiVersioni> elencoVersioni = null;
                GestioneVersioni.GetVersioni(out elencoVersioni);

                Utility.GetListaVersioni(ref elencoVersioni, Utility.ChiaviVersioni.WCFAGO, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision);

                areaVersioni.ListaVersioni = Utility.FormattaVersioni(elencoVersioni);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero delle versioni di rilascio. Riprovare più tardi";
                return esito;
            }

            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;

            return esito;
        }
        #endregion Versioni

        #region Calcolo
        public AreaEsito CalcolaDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, bool isConsultazioniANFVerificate, bool? isNuovoCalcolo, out string statoPensione, out List<GestioneFamiliari.ConsultazioneUnificataANF> listaConsultazioni, out string transactionId)
        {
            SetCulture();

            statoPensione = string.Empty;
            listaConsultazioni = null;
            AreaEsito Esito = new AreaEsito();
            transactionId = null;
            string messaggioEccezione = string.Empty;
            try
            {
                EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
                EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

                DateTime dataSistema = Utility.DataSistemaAgo;
                int annoCompetenza = 0;
                GestioneControlliDinamici.GetAnnoCompetenza(Utility.TipoAppartenenza.AGO, out annoCompetenza);

                string messaggioVideo;
                bool esito = false;
                bool isRiaperturaDomanda = contenitore.IsRiaperturaDomanda;

                if (!GestioneCalcoloDomanda.ControlsDatiCalcolaDomanda(ref contenitore, ref contenitoreDecodifica, dataSistema, annoCompetenza, isRiaperturaDomanda, matricolaOperatore, isConsultazioniANFVerificate, out listaConsultazioni, out messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                    return Esito;
                }

                if (listaConsultazioni != null && listaConsultazioni.Count > 0)
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = "Sono presenti Consultazioni Unificate da verificare";
                    return Esito;
                }

                GestioneControlliDinamici.ControlloDinamico controlloDinamicoData = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataInizioNuovoTracciato", out controlloDinamicoData);
                DateTime dataInizioNuovoTracciato = Utility.DataFromString(controlloDinamicoData.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

                // Se è una Ric o TRF e IsRicRinnovata è 1 in tabella Pensione
                // oppure se la data sistema è maggiore uguale al 01/12/2023 viene eseguito il nuovo tracciato
                if (contenitore.DatiPensione.IsRicRinnovata.GetValueOrDefault() || Utility.DataSuccessivaA(dataSistema, dataInizioNuovoTracciato))
                    GestioneCalcoloDomanda.CalcolaDomandaNuovoTracciato(ref contenitore, ref contenitoreDecodifica, matricolaOperatore, sedeOperatore, centroOperativoOperatore, dataSistema, annoCompetenza, isNuovoCalcolo, out statoPensione,
                        out esito, out messaggioVideo, out transactionId, out messaggioEccezione);
                else
                    GestioneCalcoloDomanda.CalcolaDomanda(ref contenitore, ref contenitoreDecodifica, matricolaOperatore, sedeOperatore, centroOperativoOperatore, dataSistema, annoCompetenza, isNuovoCalcolo, out statoPensione,
                        out esito, out messaggioVideo, out transactionId);

                if (esito)
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;

                    if (Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(contenitore.DatiPensione) && contenitore.DatiPensione.FlagVerify.GetValueOrDefault() &&
                        contenitore.DatiIstruttoria != null && contenitore.DatiIstruttoria.CodiceAziendaEditoriaPerTipo0179.HasValue)
                    {
                        List<GestioneAnagraficaAccordiPerTipo0179.DecodAnagraficaAccordiPerTipo0179> listaAnagraficaAccordi = null;
                        GestioneAnagraficaAccordiPerTipo0179.GetDecAnagraficaAccordi(out listaAnagraficaAccordi);

                        if (listaAnagraficaAccordi != null && listaAnagraficaAccordi.Count > 0)
                        {
                            GestioneAnagraficaAccordiPerTipo0179.DecodAnagraficaAccordiPerTipo0179 anagraficaAccordo = listaAnagraficaAccordi.Find(x => x.Codice == contenitore.DatiIstruttoria.CodiceAziendaEditoriaPerTipo0179);
                            if (anagraficaAccordo != null)
                            {
                                if (!anagraficaAccordo.Abilitata.GetValueOrDefault())
                                {
                                    messaggioVideo = messaggioVideo + " - Non è possibile procedere al calcolo definitivo della pensione in attesa della verifica della capienza degli stanziamenti.";
                                }
                            }
                        }
                    }
                }
                else
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;

                Esito.Messaggio = messaggioVideo;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, Utility.GetMessageFromException(Ex), null, Ex.StackTrace);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                if (!String.IsNullOrEmpty(messaggioEccezione) && !String.IsNullOrEmpty(messaggioEccezione.Trim()))
                    Esito.Messaggio = messaggioEccezione;
                else
                    Esito.Messaggio = "Errore tecnico nel calcolo della domanda. Riprovare più tardi";
            }
            return Esito;
        }
        #endregion Calcolo

        #region Prelievo
        public AreaEsito PrelevaDomanda(ref AreaPrelievo areaPrelievo)
        {
            SetCulture();

            string messaggioVideo = "";
            AreaEsito Esito = new AreaEsito();
            GestionePrelievo.RispostaPrelievo risposta = null;
            try
            {
                GestionePrelievo.PrelevaDomanda(areaPrelievo.Richiesta, out risposta, out messaggioVideo);
                if (!String.IsNullOrEmpty(messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                    return Esito;
                }

                areaPrelievo.Risposta = risposta;
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                messaggioVideo = "Errore tecnico durante il prelievo dei dati della pensione per AGO";
                string parametri = string.Format("Numero domanda: {0}; Sede: {1}; Categoria: {2}; Certificato: {3}: Sede operatore: {4}; Centro operativo operatore: {5}; Tipo domanda: {6}",
                    areaPrelievo.Richiesta.NumDomanda, areaPrelievo.Richiesta.Sede, areaPrelievo.Richiesta.Categoria, areaPrelievo.Richiesta.Certificato, areaPrelievo.Richiesta.SedeOperatore,
                    areaPrelievo.Richiesta.CentroOperativoOperatore, areaPrelievo.Richiesta.TipoDomanda.ToString());
                long numeroDomanda = 0;
                long.TryParse(areaPrelievo.Richiesta.NumDomanda, out numeroDomanda);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
            }
            return Esito;
        }

        public AreaEsito PrelevaGP4(ref AreaPrelievo areaPrelievo)
        {
            SetCulture();

            string messaggioVideo = "";
            AreaEsito Esito = new AreaEsito();
            GestionePrelievo.RispostaPrelievo risposta = null;
            GestionePrelievo.PrelevaGP4(areaPrelievo.Richiesta, out risposta, out messaggioVideo);
            if (!String.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }

            areaPrelievo.Risposta = risposta;
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }
        #endregion Prelievo

        #region AreaDatiContributivi
        public AreaEsito GetDatiContributiviByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, out AreaDatiContributivi areaDatiContributivi, out bool IsDataFromDB)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            areaDatiContributivi = null;
            IsDataFromDB = false;
            AreaEsito Esito = new AreaEsito();
            string messaggioVideo = string.Empty;
            GestioneContrib.DatiCalcolo datiCalcoloBL = null;
            GestioneContrib.DatiCalcolo datiCalcoloStoricoBL = null;
            GestioneContrib.DatiCalcoloENPALS datiCalcoloENPALS = null;

            bool isRiaperturaDomanda = contenitore.IsRiaperturaDomanda;
            Utility.TipoCalcolo tipoCalcolo = Utility.GetTipoCalcolo(contenitore.DatiPensione);

            if (areaDatiContributivi == null)
                areaDatiContributivi = new AreaDatiContributivi();

            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
            {
                GestioneContrib.GetDatiCalcoloENPALSByDatiPensione(ref contenitore, out datiCalcoloENPALS);
                if (!String.IsNullOrEmpty(messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                    return Esito;
                }
                areaDatiContributivi = new AreaDatiContributivi();
                areaDatiContributivi.DatiCalcoloENPALS = datiCalcoloENPALS;
                areaDatiContributivi.DatiCalcolo = new GestioneContrib.DatiCalcolo();
                if (contenitore.DatiPensione.TipoCalcolo.HasValue)
                    areaDatiContributivi.DatiCalcolo.TipoCalcolo = (GestioneContrib.TipoCalcolo)contenitore.DatiPensione.TipoCalcolo;
            }
            else if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria))
            {
                areaDatiContributivi = new AreaDatiContributivi();

                GestioneContrib.DatiCalcoloQuotePensione datiCalcoloQuotePensioneStorico = null;
                GestioneContrib.GetQuotePensioneStoricoByDatiPensione(ref contenitore, out datiCalcoloQuotePensioneStorico);
                areaDatiContributivi.DatiCalcoloQuotePensioneStorico = datiCalcoloQuotePensioneStorico;

                GestioneContrib.DatiCalcoloQuotePensione datiCalcoloQuotePensione = null;
                GestioneContrib.GetQuotePensioneByDatiPensione(ref contenitore, out datiCalcoloQuotePensione);
                if (datiCalcoloQuotePensione != null && datiCalcoloQuotePensione.LQuotePensione != null && datiCalcoloQuotePensione.LQuotePensione.Count > 0 &&
                    datiCalcoloQuotePensioneStorico != null && datiCalcoloQuotePensioneStorico.LQuotePensione != null && datiCalcoloQuotePensioneStorico.LQuotePensione.Count > 0)
                {
                    foreach (var quota in datiCalcoloQuotePensione.LQuotePensione)
                    {
                        GestioneContrib.DatiQuotePensione quotaStorico = datiCalcoloQuotePensioneStorico.LQuotePensione.FirstOrDefault(x => x.EnteGestioneFondo == quota.EnteGestioneFondo);
                        if (quotaStorico != null && quotaStorico.Decorrenza.HasValue && quotaStorico.Decorrenza.Equals(new DateTime(9999, 1, 1)) &&
                            quotaStorico.Importo != null && quotaStorico.Importo <= 0.02m)
                            quota.IsQuotaProgressiva = true;
                    }
                }
                areaDatiContributivi.DatiCalcoloQuotePensione = datiCalcoloQuotePensione;

                GestioneContrib.DatiCalcoloQuoteMiglioramentiContrattuali datiCalcoloQuoteMiglioramentiContrattuali = null;
                GestioneContrib.GetQuoteMiglioramentiContrattualiByDatiPensione(ref contenitore, out datiCalcoloQuoteMiglioramentiContrattuali);
                areaDatiContributivi.DatiCalcoloQuoteMiglioramentiContrattuali = datiCalcoloQuoteMiglioramentiContrattuali;

                GestioneContrib.DatiCalcoloQuoteMiglioramentiContrattuali datiCalcoloQuoteMiglioramentiContrattualiStorico = null;
                GestioneContrib.GetQuoteMiglioramentiContrattualiStoricoByDatiPensione(ref contenitore, out datiCalcoloQuoteMiglioramentiContrattualiStorico);
                areaDatiContributivi.DatiCalcoloQuoteMiglioramentiContrattualiStorico = datiCalcoloQuoteMiglioramentiContrattualiStorico;

                areaDatiContributivi.TipoCumulo = contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.TipoCumulo : null;
                areaDatiContributivi.IsScaricoTrattenuteCumulo = GestioneCtrlControlliApplicativi.CheckControlloApplicativoAttivoByData(GestioneCtrlControlliApplicativi.EnumNomeControllo.AGO.SCARICO_TRATTENUTE_CUMULO, Utility.DataSistemaAgo);
            }
            else
            {
                GestioneContrib.GetDatiCalcoloByDatiPensione(ref contenitore, ref contenitoreDecodifica, isRiaperturaDomanda, out datiCalcoloBL, out messaggioVideo, out IsDataFromDB);
                if (!String.IsNullOrEmpty(messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                    return Esito;
                }
                GestioneContrib.GetDatiCalcoloStoricoByDatiPensione(ref contenitore, ref contenitoreDecodifica, out datiCalcoloStoricoBL, out messaggioVideo);

                areaDatiContributivi = new AreaDatiContributivi();
                areaDatiContributivi.DatiCalcolo = datiCalcoloBL;
                areaDatiContributivi.DatiCalcoloStorico = datiCalcoloStoricoBL;
                areaDatiContributivi.IsFineAssicurazionePost2012 = GestioneContrib.IsFineAssicurazionePost2012(contenitore.DatiPensione.FineAssicurazione);
                areaDatiContributivi.IsPensioneInabilitaPost2012 = Utility.IsPensioneInabilitaPost2012(contenitore.DatiPensione);
                areaDatiContributivi.IsPnlImportoLordoAllaDecVisible = Utility.IsDomandaVESO92_L92(contenitore.DatiPensione) || Utility.IsDomandaVOCOOP_L92(contenitore.DatiPensione) ||
                                                                       Utility.IsDomandaVOESO_L92(contenitore.DatiPensione) || Utility.IsDomandaVOCRED_CRED27__DAP(contenitore.DatiPensione) ||
                                                                       (Utility.IsDomandaVESO29(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.GetFiltro() == "FS") ||
                                                                       (Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.GetFiltro() == "FS") ||
                                                                       Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESOAMB_L26(contenitore.DatiPensione) ||
                                                                       Utility.IsIsoPensioneRicWithGP2BB05(contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.Gruppo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null)
                                                                       || (Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.GetFiltro() == "ESA" && Utility.IsAssegnoStraordinarioRiscossioneTributiErariali(contenitore.DatiPensione)) ||
                                                                       Utility.IsDomandaESPA_L26(contenitore.DatiPensione) || Utility.IsDomandaVESO33_DAP(contenitore.DatiPensione) || contenitore.DatiPensione.IsRicExtracalcolo.GetValueOrDefault() || Utility.IsDomandaESOPMI(contenitore.DatiPensione.SiglaCategoria)
                                                                       || Utility.IsDomandaCOOP28_DAP(contenitore.DatiPensione);

                if (Utility.IsDomandaVESO92_L92(contenitore.DatiPensione) || Utility.IsDomandaVOCOOP_L92(contenitore.DatiPensione) || Utility.IsDomandaVOESO_L92(contenitore.DatiPensione) ||
                    Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESOAMB_L26(contenitore.DatiPensione) ||
                    (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && contenitore.DatiStoricoGP != null && contenitore.DatiStoricoGP.GP2BB05 == "E")
                    || (Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.GetFiltro() == "ESA" && Utility.IsAssegnoStraordinarioRiscossioneTributiErariali(contenitore.DatiPensione)) ||
                    Utility.IsDomandaESPA_L26(contenitore.DatiPensione) || Utility.IsDomandaVESO33_DAP(contenitore.DatiPensione) || Utility.IsDomandaESOPMI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaCOOP28_DAP(contenitore.DatiPensione))
                    areaDatiContributivi.GestioneImportoLordoAllaDec = "E - AZIENDA";
                else if (Utility.IsDomandaVOCRED_CRED27__DAP(contenitore.DatiPensione))
                    areaDatiContributivi.GestioneImportoLordoAllaDec = "L - AZIENDA";
                else if (((Utility.IsDomandaVESO29(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria)) &&
                    contenitore.DatiPensione.GetFiltro() == "FS") || (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && contenitore.DatiStoricoGP != null && contenitore.DatiStoricoGP.GP2BB05 == "L1"))
                    areaDatiContributivi.GestioneImportoLordoAllaDec = "L1 - ESODO FS";
                if (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && (Utility.IsDomandaCRED27(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVESO33(contenitore.DatiPensione.SiglaCategoria) ||
                    Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESPA(contenitore.DatiPensione.SiglaCategoria)) && contenitore.DatiPensione.IsRicExtracalcolo.GetValueOrDefault())
                {
                    if (Utility.IsDomandaCRED27(contenitore.DatiPensione.SiglaCategoria))
                        areaDatiContributivi.GestioneImportoLordoAllaDec = "L - Extracalcolo";
                    else
                        areaDatiContributivi.GestioneImportoLordoAllaDec = "E - Extracalcolo";
                }
                areaDatiContributivi.IsSettimane707Visible = GestioneContrib.IsSettimane707Visible(contenitore.DatiPensione, ref contenitoreDecodifica, datiCalcoloBL != null ? datiCalcoloBL.lDatiRetributivi : null,
                    datiCalcoloBL != null ? datiCalcoloBL.lDatiContributivi : null, contenitore.DatiBeneficioVittimeTerrorismo, tipoCalcolo, contenitore.DatiDanteCausa);

                if (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria))
                {
                    //domanda ex-INPDAI
                    GestioneContrib.DatiExINPDAI datiExINPDAI;
                    GestioneContrib.GetDatiCalcoloExInpdaiByDatiPensione(ref contenitore, ref contenitoreDecodifica, datiCalcoloBL, out datiExINPDAI);
                    areaDatiContributivi.DatiExINPDAI = datiExINPDAI;

                    GestioneContrib.DatiExINPDAI datiExINPDAIStorico;
                    GestioneContrib.GetDatiCalcoloExInpdaiStoricoByDatiPensione(ref contenitore, ref contenitoreDecodifica, datiCalcoloBL, out datiExINPDAIStorico);
                    areaDatiContributivi.DatiExINPDAIStorico = datiExINPDAIStorico;
                    areaDatiContributivi.TipoCalcoloVincenteUnicarpe = contenitore.DatiIstruttoria.TipoCalcoloVincenteUnicarpe;
                    areaDatiContributivi.Bypass_LIMITE7_INTERI_MONT_AMM = GestioneBypassControllo.CheckBypassControlloByNomeBypass(contenitore.DatiPensione, GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_AGO.LIMITE7_INTERI_MONT_AMM);
                }
                if (Utility.IsDomandaAUT(contenitore.DatiPensione))
                {
                    bool facoltaComputoPrecedentePensione = contenitore.DatiIstruttoria != null && contenitore.DatiIstruttoria.FacoltaComputoPrecedentePensione.GetValueOrDefault() == 'F' ? true : false;
                    if (contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.FacoltaComputo.HasValue)
                    {
                        if (areaDatiContributivi.DatiCalcolo == null)
                            areaDatiContributivi.DatiCalcolo = new GestioneContrib.DatiCalcolo();
                        areaDatiContributivi.DatiCalcolo.FacoltaComputo = contenitore.DatiPensioniDatiGenerici.FacoltaComputo;
                    }
                    else
                    {
                        if (areaDatiContributivi.DatiCalcolo == null)
                            areaDatiContributivi.DatiCalcolo = new GestioneContrib.DatiCalcolo();
                        areaDatiContributivi.DatiCalcolo.FacoltaComputo = facoltaComputoPrecedentePensione;
                    }

                    if (areaDatiContributivi.DatiCalcolo == null)
                        areaDatiContributivi.DatiCalcolo = new GestioneContrib.DatiCalcolo();
                    areaDatiContributivi.DatiCalcolo.CodiceP18PrecedentePensione = contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceP18PrecedentePensione : null;
                }
                if (Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVOCOOP_L92(contenitore.DatiPensione) || Utility.IsDomandaVOESO_L92(contenitore.DatiPensione) ||
                    Utility.IsDomandaVOCRED_CRED27__DAP(contenitore.DatiPensione) || Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESOAMB_L26(contenitore.DatiPensione) ||
                    ((Utility.IsDomandaVESO29(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria)) && contenitore.DatiPensione.GetFiltro() == "FS")
                    || Utility.IsIsoPensioneRicWithGP2BB05(contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.Gruppo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null)
                    || (Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.GetFiltro() == "ESA" && Utility.IsAssegnoStraordinarioRiscossioneTributiErariali(contenitore.DatiPensione) ||
                    Utility.IsDomandaESPA(contenitore.DatiPensione.SiglaCategoria)) || Utility.IsDomandaVESO33_DAP(contenitore.DatiPensione) || Utility.IsDomandaCOOP28_DAP(contenitore.DatiPensione)
                    || ((Utility.IsDomandaCRED27(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVESO33(contenitore.DatiPensione.SiglaCategoria)) && contenitore.DatiPensione.IsRicExtracalcolo.GetValueOrDefault()))
                {
                    if (contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.ImportoLordoAllaDecorrenza.HasValue)
                    {
                        if (areaDatiContributivi.DatiCalcolo == null)
                            areaDatiContributivi.DatiCalcolo = new GestioneContrib.DatiCalcolo();
                        areaDatiContributivi.DatiCalcolo.ImportoLordoAllaDecorrenza = contenitore.DatiPensioniDatiGenerici.ImportoLordoAllaDecorrenza;
                    }
                }

                if (Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria))
                {
                    if (datiCalcoloBL == null || !datiCalcoloBL.ImportoLordo.HasValue)
                    {
                        if (contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.ImportoLordo.HasValue)
                        {
                            if (areaDatiContributivi.DatiCalcolo == null)
                                areaDatiContributivi.DatiCalcolo = new GestioneContrib.DatiCalcolo();
                            areaDatiContributivi.DatiCalcolo.ImportoLordo = contenitore.DatiPensioniDatiGenerici.ImportoLordo;
                        }
                    }

                    if (contenitore.DatiStoricoGP != null)
                    {
                        if (areaDatiContributivi.DatiCalcoloStorico == null)
                            areaDatiContributivi.DatiCalcoloStorico = new GestioneContrib.DatiCalcolo();
                        areaDatiContributivi.DatiCalcoloStorico.ImportoLordo = contenitore.DatiStoricoGP.ImportoLordo;
                    }
                }

                if (Utility.IsDomandaBeneficioTerrorismoOver80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo) ||
                    Utility.IsDomandaBeneficioTerrorismoUnder80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo))
                {
                    areaDatiContributivi.IsBeneficioVittimeTerrorismo = true;
                    GestioneContrib.DatiCalcoloVittimeTerrorismo datiCalcoloVittimeTerrorismo = null;
                    GestioneContrib.GetDatiCalcoloVittimeByDatiPensione(ref contenitore, datiCalcoloBL != null ? datiCalcoloBL.lDatiRetributivi : null,
                        datiCalcoloBL != null ? datiCalcoloBL.lDatiContributivi : null, out datiCalcoloVittimeTerrorismo);

                    long? soggettoBeneficiario = contenitore.DatiBeneficioVittimeTerrorismo != null ? contenitore.DatiBeneficioVittimeTerrorismo.SoggettoBeneficiario : null;
                    long? tipologiaPrestazione = contenitore.DatiBeneficioVittimeTerrorismo != null ? contenitore.DatiBeneficioVittimeTerrorismo.TipologiaPrestazione : null;
                    long? tipologiaBeneficio = contenitore.DatiBeneficioVittimeTerrorismo != null ? contenitore.DatiBeneficioVittimeTerrorismo.TipologiaBeneficio : null;

                    areaDatiContributivi.DatiCalcoloVittimeTerrorismo = datiCalcoloVittimeTerrorismo;
                    areaDatiContributivi.IsDatiRetributiviVittimeVisible = Utility.IsDatiRetributiviVittimeVisible(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo, tipoCalcolo);
                    areaDatiContributivi.IsDatiContributiviVittimeVisible = Utility.IsDatiContributiviVittimeVisible(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo,
                        tipoCalcolo, datiCalcoloBL != null && datiCalcoloBL.lDatiContributivi != null && datiCalcoloBL.lDatiContributivi.Exists(x => x.IsQuotaDL214Presente()));
                    areaDatiContributivi.IsDatiImportoPensioneVittimeVisible = Utility.IsDatiImportoPensioneVittimeVisible(contenitore.DatiPensione, soggettoBeneficiario, tipologiaPrestazione,
                        tipologiaBeneficio);
                    areaDatiContributivi.IsBeneficioImportoPensioneX = GestioneContrib.IsBeneficioImportoPensioneX(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo);
                    areaDatiContributivi.IsSettimaneImportoPensioneLocked = GestioneContrib.IsSettimaneImportoPensioneLocked(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo);
                    areaDatiContributivi.SoggettoBeneficiario = soggettoBeneficiario;
                    areaDatiContributivi.TipologiaPrestazione = tipologiaPrestazione;
                    areaDatiContributivi.TipologiaBeneficio = tipologiaBeneficio;
                }

                if (Utility.IsDomandaAnticipataEsattoriali(contenitore.DatiPensione) || (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && contenitore.ListaDatiQuotaFondoIntegrativoStorico != null))
                {
                    GestioneContrib.DatiQuotaFondoIntegrativo datiQuotaFondoIntegrativo = null;
                    GestioneContrib.GetDatiCalcoloQuotaFondoIntegrativoByDatiPensione(ref contenitore, ref contenitoreDecodifica, out datiQuotaFondoIntegrativo, out messaggioVideo);
                    areaDatiContributivi.DatiQuotaFondoIntegrativo = datiQuotaFondoIntegrativo;
                    //ENG - RIC Esattoriali: gestiti i flussi per il recupero dei dati dal prelievo
                    if (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
                        areaDatiContributivi.IsRicOTrfEsattoriali = contenitore.ListaDatiQuotaFondoIntegrativoStorico != null ? true : false;
                }

                if (Utility.IsRenditaCasalinghe(contenitore.DatiPensione) || Utility.IsRenditaFacoltativa(contenitore.DatiPensione))
                {
                    if (contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.ImportoMensileAllaDecorrenzaOriginaria.HasValue)
                    {
                        if (areaDatiContributivi.DatiCalcolo == null)
                            areaDatiContributivi.DatiCalcolo = new GestioneContrib.DatiCalcolo();
                        areaDatiContributivi.DatiCalcolo.ImportoMensileAllaDecorrenzaOriginaria = contenitore.DatiPensioniDatiGenerici.ImportoMensileAllaDecorrenzaOriginaria;
                    }

                    if (contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.ImportoMensileAlGennaio2001.HasValue)
                    {
                        if (areaDatiContributivi.DatiCalcolo == null)
                            areaDatiContributivi.DatiCalcolo = new GestioneContrib.DatiCalcolo();
                        areaDatiContributivi.DatiCalcolo.ImportoMensileAlGennaio2001 = contenitore.DatiPensioniDatiGenerici.ImportoMensileAlGennaio2001;
                    }
                }

                //ENG - Aggiornamento Memo 68/2022 IOPGI
                //ENG - Spacchettate SOPGI
                if (Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione))
                    || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa))
                {
                    GestioneContrib.DatiQuotaFondoINPGI datiQuotaFondoINPGI = null;
                    GestioneContrib.GetDatiCalcoloQuotaFondoINPGIByDatiPensione(ref contenitore, ref contenitoreDecodifica, out datiQuotaFondoINPGI, out messaggioVideo);
                    areaDatiContributivi.DatiQuotaFondoINPGI = datiQuotaFondoINPGI;

                    if (Utility.IsDomandaINPGI(contenitore.DatiPensione) && Utility.IsDomandaUnicarpe(contenitore.DatiPensione, true) == Utility.TipoUnicarpe.Automatica &&
                        (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda)))
                    {
                        GestioneContrib.DatiQuotaFondoINPGI datiQuotaFondoINPGIStorico = null;
                        GestioneContrib.GetDatiCalcoloQuotaFondoINPGIStoricoByDatiPensione(ref contenitore, ref contenitoreDecodifica, out datiQuotaFondoINPGIStorico, out messaggioVideo);
                        if (datiQuotaFondoINPGIStorico != null)
                            areaDatiContributivi.DatiQuotaFondoINPGIStorico = datiQuotaFondoINPGIStorico;
                        areaDatiContributivi.IsRicOTrfAutmaticaINPGI = true;
                    }

                    areaDatiContributivi.IsDomandaVOPGIFiltroAGI = Utility.IsDomandaVOPGI_AGI(contenitore.DatiPensione);
                    areaDatiContributivi.InizioAssicurazione = contenitore.DatiPensione.InizioAssicurazione;
                }
                areaDatiContributivi.DatiCalcolo.IsPrimoRecordRetrGestioneS = IsPrimoRecordRetrGestioneS(contenitore.DatiPensione);
                areaDatiContributivi.IsEliminataPerCauseVarie = contenitore.DatiEliminazione != null && (contenitore.DatiEliminazione.CodiceMotivo == 3 || contenitore.DatiEliminazione.CodiceMotivo == 28) ? true : false;
                GestioneControlliDinamici.ControlloDinamico controlloDinamico = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo102", out controlloDinamico);
                areaDatiContributivi.IsMemo102Abilitato = controlloDinamico != null && controlloDinamico.ValoreControllo == "SI" ? true : false;
            }
            //ENG - MEMO 74_2023
            List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo> listaPrestazioniEstere = new List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo>();
            GestioneDatiEsteriCumulo.GetPrestazioniEstereCumuloByIdPensione(contenitore.DatiPensione.Id, out listaPrestazioniEstere);
            GestioneControlliDinamici.ControlloDinamico ctrlMemo74_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo74_2023", out ctrlMemo74_2023);

            if (((Utility.IsDomandaVOCUM(contenitore.DatiPensione.SiglaCategoria) && ctrlMemo74_2023 != null && ctrlMemo74_2023.ValoreControllo == "SI") ||
                //ENG - Memo 116/2025
                Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(contenitore.DatiPensione) || Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(contenitore.DatiPensione) ||
                Utility.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSE(contenitore.DatiPensione)) &&
                ((!Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, isRiaperturaDomanda) && !string.IsNullOrEmpty(contenitore.DatiPensione.NaturaPensione) && contenitore.DatiPensione.NaturaPensione.Substring(2, 1) == "V") ||
                (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, isRiaperturaDomanda) && listaPrestazioniEstere != null && listaPrestazioniEstere.Count() > 0)))
            {
                GestioneControlliDinamici.ControlloDinamico ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaChiamata_Naci_AllegatiConv_Memo74_2023", out ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023);

                GestioneControlliDinamici.ControlloDinamico ctrlAbilitaChiamata_CUMUL_Memo74_2023 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaChiamata_CUMUL_Memo74_2023", out ctrlAbilitaChiamata_CUMUL_Memo74_2023);
                //ENG - Memo 116/2025
                GestioneControlliDinamici.ControlloDinamico ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaChiamata_Naci_AllegatiConv_Memo116_2025", out ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025);

                if (areaDatiContributivi.DatiCalcolo == null)
                    areaDatiContributivi.DatiCalcolo = new GestioneContrib.DatiCalcolo();
                areaDatiContributivi.IsMemo74_2023Abilitato = true;

                #region ProRata
                List<GestioneContrib.StatoEsteroCumulo> elencoStatiEsteri = null;
                GestioneContrib.GetStatiEsteri(contenitore.DatiPensione, listaPrestazioniEstere, matricolaOperatore, sedeOperatore, centroOperativoOperatore, out elencoStatiEsteri, out messaggioVideo);
                if (!string.IsNullOrEmpty(messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                    return Esito;
                }

                if (elencoStatiEsteri != null)
                {
                    areaDatiContributivi.ProRata = new GestioneContrib.ProRata();
                    areaDatiContributivi.ProRata.ElencoStatiEsteri = new List<GestioneContrib.StatoEsteroCumulo>();
                    areaDatiContributivi.ProRata.ElencoStatiEsteri = elencoStatiEsteri;
                }

                if ((ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023 != null && ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023.ValoreControllo == "SI") ||
                    (ctrlAbilitaChiamata_CUMUL_Memo74_2023 != null && ctrlAbilitaChiamata_CUMUL_Memo74_2023.ValoreControllo == "SI") ||
                    (ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025 != null && ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025.ValoreControllo == "SI"))
                {
                    areaDatiContributivi.IsDatiEsteriFromServices = true;
                }
                #endregion ProRata
            }

            GetListeDecodifica(ref contenitore, ref contenitoreDecodifica, ref areaDatiContributivi);
            if (areaDatiContributivi == null)
                areaDatiContributivi = new AreaDatiContributivi();
            areaDatiContributivi.IsAnte96 = Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, isRiaperturaDomanda);
            areaDatiContributivi.MostraQuotaAnte96 = Utility.mostraQuotaAnte96(contenitore.DatiPensione, contenitore.DatiDanteCausa, isRiaperturaDomanda, areaDatiContributivi.IsAnte96);

            //ENG- Memo 68/2022 aggiornato al 12/03/2025
            if ((Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa) && (Utility.IsDomandaPensioneIndiretta(contenitore.DatiPensione) || Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda)))) &&
                contenitore.DatiPensione.FineAssicurazione.HasValue && Utility.DataStrettamenteSuccessivaA(contenitore.DatiPensione.FineAssicurazione.Value, new DateTime(2022, 06, 30)))
            {
                GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneModificheMemoINPGI_20250312 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneModificheMemoINPGI_20250312 ", out ctrlAbilitazioneModificheMemoINPGI_20250312);
                if (ctrlAbilitazioneModificheMemoINPGI_20250312 != null && ctrlAbilitazioneModificheMemoINPGI_20250312.ValoreControllo == "SI")
                    areaDatiContributivi.IsDomandaINPGIFineAssicurazionePost30062022 = true;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        public AreaEsito StoreDatiContributiviByDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, ref AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            bool isRiaperturaDomanda = contenitore.IsRiaperturaDomanda;

            Utility.TipoCalcolo tipoCalcolo = Utility.GetTipoCalcolo(contenitore.DatiPensione);

            AreaEsito Esito = new AreaEsito();
            string messaggioVideo = "";
            try
            {
                StoreDatiCalcoloPrivate(ref contenitore, ref contenitoreDecodifica, areaDatiContributivi, false, out messaggioVideo);
                if (!String.IsNullOrEmpty(messaggioVideo))
                {
                    contenitore.DatiQuadroDatiContributivi_GetEffettuata = false;
                    if ((Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) && contenitore.DatiQuadroDatiContributivi.TabDatiCalcoloENPALS.GetValueOrDefault() == 0) ||
                        (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiQuadroDatiContributivi.TabQuotePensione.GetValueOrDefault() == 0) ||
                        (Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiQuadroDatiContributivi.TabQuotePensione.GetValueOrDefault() == 0) ||
                        (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiQuadroDatiContributivi.TabDatiCalcoloINPDAI.GetValueOrDefault() == 0) ||
                        (!Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) && !Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) &&
                            contenitore.DatiQuadroDatiContributivi.TabDatiCalcolo.GetValueOrDefault() == 0)
                        )
                        GestioneBypassControllo.SetUnlock(numeroDomanda, typeof(GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_AGO));
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                    return Esito;
                }

                if (Utility.IsDomandaBeneficioTerrorismoOver80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo) ||
                    Utility.IsDomandaBeneficioTerrorismoUnder80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo))
                {
                    StoreDatiVittimeTerrorismoPrivate(ref contenitore, ref contenitoreDecodifica, areaDatiContributivi, tipoCalcolo, false, out messaggioVideo);
                    if (!String.IsNullOrEmpty(messaggioVideo))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = messaggioVideo;
                        return Esito;
                    }
                }

                if (Utility.IsDomandaAnticipataEsattoriali(contenitore.DatiPensione) || (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && contenitore.ListaDatiQuotaFondoIntegrativoStorico != null))
                {
                    StoreDatiQuotaFondoIntegrativoPrivate(ref contenitore, ref contenitoreDecodifica, areaDatiContributivi, out messaggioVideo);
                    if (!String.IsNullOrEmpty(messaggioVideo))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = messaggioVideo;
                        return Esito;
                    }
                }

                //ENG - Aggiornamento Memo 68/2022 IOPGI
                //ENG - Spacchettate SOPGI
                if (Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione))
                    || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa))
                {
                    StoreDatiQuotaFondoINPGIPrivate(ref contenitore, ref contenitoreDecodifica, areaDatiContributivi, out messaggioVideo);
                    if (!String.IsNullOrEmpty(messaggioVideo))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = messaggioVideo;
                        return Esito;
                    }
                }

                //ENG - MEMO 74_2023
                List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo> listaPrestazioniEstere = new List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo>();
                GestioneDatiEsteriCumulo.GetPrestazioniEstereCumuloByIdPensione(contenitore.DatiPensione.Id, out listaPrestazioniEstere);
                GestioneControlliDinamici.ControlloDinamico ctrlMemo74_2023 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo74_2023", out ctrlMemo74_2023);

                if ((Utility.IsDomandaVOCUM(contenitore.DatiPensione.SiglaCategoria) && ctrlMemo74_2023 != null && ctrlMemo74_2023.ValoreControllo == "SI") ||
                    //ENG - Memo 116/2025
                    Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(contenitore.DatiPensione) || Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(contenitore.DatiPensione) ||
                    Utility.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSE(contenitore.DatiPensione) &&
                    ((!Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, isRiaperturaDomanda) && !string.IsNullOrEmpty(contenitore.DatiPensione.NaturaPensione) && contenitore.DatiPensione.NaturaPensione.Substring(2, 1) == "V") ||
                    (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, isRiaperturaDomanda) && listaPrestazioniEstere != null && listaPrestazioniEstere.Count() > 0)))
                {
                    GestioneControlliDinamici.ControlloDinamico ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023 = null;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaChiamata_Naci_AllegatiConv_Memo74_2023", out ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023);

                    GestioneControlliDinamici.ControlloDinamico ctrlAbilitaChiamata_CUMUL_Memo74_2023 = null;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaChiamata_CUMUL_Memo74_2023", out ctrlAbilitaChiamata_CUMUL_Memo74_2023);
                    //ENG - Memo 116/2025
                    GestioneControlliDinamici.ControlloDinamico ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025 = null;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaChiamata_Naci_AllegatiConv_Memo116_2025", out ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025);

                    if ((ctrlMemo74_2023 != null && ctrlMemo74_2023.ValoreControllo == "SI" && ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023 != null) ||
                        (ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023.ValoreControllo == "SI" && ctrlAbilitaChiamata_CUMUL_Memo74_2023 != null && ctrlAbilitaChiamata_CUMUL_Memo74_2023.ValoreControllo == "SI") ||
                        (ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025 != null && ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025.ValoreControllo == "SI") ||
                        Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
                    {
                        Esito = StoreDatiProRataPrivate(ref contenitore, matricolaOperatore, sedeOperatore, centroOperativoOperatore, ref areaDatiContributivi);
                        if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                            return Esito;
                    }
                }

                StoreDatiMiglioramentiContributiviPrivate(ref contenitore, ref contenitoreDecodifica, areaDatiContributivi, true, out messaggioVideo);
                if (!String.IsNullOrEmpty(messaggioVideo))
                {
                    contenitore.DatiQuadroDatiContributivi_GetEffettuata = false;
                    GestioneBypassControllo.SetUnlock(numeroDomanda, typeof(GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_AGO));
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                }
            }
            catch (Exception Ex)
            {
                contenitore.DatiQuadroDatiContributivi_GetEffettuata = false;
                if ((Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) && contenitore.DatiQuadroDatiContributivi.TabDatiCalcoloENPALS.GetValueOrDefault() == 0) ||
                    (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiQuadroDatiContributivi.TabQuotePensione.GetValueOrDefault() == 0) ||
                    (Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiQuadroDatiContributivi.TabQuotePensione.GetValueOrDefault() == 0) ||
                    (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiQuadroDatiContributivi.TabDatiCalcoloINPDAI.GetValueOrDefault() == 0) ||
                    (!Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) && !Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) &&
                        contenitore.DatiQuadroDatiContributivi.TabDatiCalcolo.GetValueOrDefault() == 0)
                    )
                    GestioneBypassControllo.SetUnlock(numeroDomanda, typeof(GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_AGO));
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nel salvataggio dei dati calcolo. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";

            return Esito;
        }

        #region Dati Calcolo

        public AreaEsito StoreDatiCalcoloByDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, ref AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            if (!(Utility.IsDomandaBeneficioTerrorismoOver80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo) ||
                Utility.IsDomandaBeneficioTerrorismoUnder80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo)))
                contenitore.DatiBeneficioVittimeTerrorismo = null;

            AreaEsito Esito = new AreaEsito();
            string messaggioVideo = "";
            try
            {
                StoreDatiCalcoloPrivate(ref contenitore, ref contenitoreDecodifica, areaDatiContributivi, true, out messaggioVideo);
                if (!String.IsNullOrEmpty(messaggioVideo))
                {
                    contenitore.DatiQuadroDatiContributivi_GetEffettuata = false;
                    if ((Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) && contenitore.DatiQuadroDatiContributivi.TabDatiCalcoloENPALS.GetValueOrDefault() == 0) ||
                        (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiQuadroDatiContributivi.TabQuotePensione.GetValueOrDefault() == 0) ||
                        (Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiQuadroDatiContributivi.TabQuotePensione.GetValueOrDefault() == 0) ||
                        (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiQuadroDatiContributivi.TabDatiCalcoloINPDAI.GetValueOrDefault() == 0) ||
                        (!Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) && !Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) &&
                            contenitore.DatiQuadroDatiContributivi.TabDatiCalcolo.GetValueOrDefault() == 0)
                        )
                        GestioneBypassControllo.SetUnlock(numeroDomanda, typeof(GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_AGO));
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                }
                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = "";
                }
            }
            catch (Exception Ex)
            {
                contenitore.DatiQuadroDatiContributivi_GetEffettuata = false;
                if ((Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) && contenitore.DatiQuadroDatiContributivi.TabDatiCalcoloENPALS.GetValueOrDefault() == 0) ||
                    (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiQuadroDatiContributivi.TabQuotePensione.GetValueOrDefault() == 0) ||
                    (Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiQuadroDatiContributivi.TabQuotePensione.GetValueOrDefault() == 0) ||
                    (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiQuadroDatiContributivi.TabDatiCalcoloINPDAI.GetValueOrDefault() == 0) ||
                    (!Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) && !Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) &&
                        contenitore.DatiQuadroDatiContributivi.TabDatiCalcolo.GetValueOrDefault() == 0)
                    )
                    GestioneBypassControllo.SetUnlock(numeroDomanda, typeof(GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_AGO));
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nel salvataggio dei dati calcolo. Riprovare più tardi";
            }
            areaDatiContributivi.IsAnte96 = Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, Utility.IsRiaperturaDomanda(contenitore.DatiPensione.Id));
            areaDatiContributivi.MostraQuotaAnte96 = Utility.mostraQuotaAnte96(contenitore.DatiPensione, contenitore.DatiDanteCausa, Utility.IsRiaperturaDomanda(contenitore.DatiPensione.Id), areaDatiContributivi.IsAnte96);
            return Esito;
        }

        public AreaEsito StoreDatiQuoteMiglioramentiContrattualiByDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, ref AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            AreaEsito Esito = new AreaEsito();
            string messaggioVideo = "";
            try
            {
                StoreDatiMiglioramentiContributiviPrivate(ref contenitore, ref contenitoreDecodifica, areaDatiContributivi, true, out messaggioVideo);
                if (!String.IsNullOrEmpty(messaggioVideo))
                {
                    contenitore.DatiQuadroDatiContributivi_GetEffettuata = false;
                    GestioneBypassControllo.SetUnlock(numeroDomanda, typeof(GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_AGO));
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                }
                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = "";
                }
            }
            catch (Exception Ex)
            {
                contenitore.DatiQuadroDatiContributivi_GetEffettuata = false;
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, Utility.GetMessageFromException(Ex), null, Ex.StackTrace);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nel salvataggio dei dati calcolo. Riprovare più tardi";
            }
            areaDatiContributivi.IsAnte96 = Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, Utility.IsRiaperturaDomanda(contenitore.DatiPensione.Id));
            areaDatiContributivi.MostraQuotaAnte96 = Utility.mostraQuotaAnte96(contenitore.DatiPensione, contenitore.DatiDanteCausa, Utility.IsRiaperturaDomanda(contenitore.DatiPensione.Id), areaDatiContributivi.IsAnte96);
            return Esito;
        }

        public AreaEsito CancelDatiContributiviByDomanda(long numeroDomanda)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            string errori = string.Empty;
            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
            {
                bool isRiaperturaDomanda = contenitore.IsRiaperturaDomanda;
                GestioneContrib.DeleteDatiCalcoloENPALSByDatiPensione(ref contenitore, isRiaperturaDomanda, out errori);
            }
            else if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria))
                GestioneContrib.DeleteDatiCalcoloQuotePensioneByDatiPensione(ref contenitore, out errori);
            else
                GestioneContrib.DeleteDatiCalcoloByDatiPensione(ref contenitore, out errori);
            if (String.IsNullOrEmpty(errori))
            {
                GestioneBypassControllo.SetUnlock(numeroDomanda, typeof(GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_AGO));

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            else
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Attenzione! Cancellazione non riuscita; riprovare";
            }
            return Esito;
        }

        private static void StoreDatiCalcoloPrivate(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, AreaDatiContributivi areaDatiContributivi,
            bool isSingleTab, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
                GestioneContrib.StoreDatiCalcoloENPALSByDatiPensione(ref contenitore, areaDatiContributivi.DatiCalcoloENPALS, out messaggioVideo);
            else if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria))
                GestioneContrib.StoreDatiCalcoloQuotePensioneByDatiPensione(ref contenitore, ref contenitoreDecodifica, areaDatiContributivi.DatiCalcoloQuotePensione, out messaggioVideo);
            else
            {
                GestioneContrib.DatiCalcolo datiCalcoloOrdinati = null;
                GestioneContrib.StoreDatiCalcoloByDatiPensione(ref contenitore, ref contenitoreDecodifica, areaDatiContributivi.DatiCalcolo, areaDatiContributivi.DatiExINPDAI,
                    areaDatiContributivi.DatiCalcoloVittimeTerrorismo, isSingleTab, out datiCalcoloOrdinati, out messaggioVideo);
                if (datiCalcoloOrdinati != null)
                {
                    if (datiCalcoloOrdinati.lDatiContributivi != null && datiCalcoloOrdinati.lDatiContributivi.Count > 0)
                        areaDatiContributivi.DatiCalcolo.lDatiContributivi = datiCalcoloOrdinati.lDatiContributivi;

                    if (datiCalcoloOrdinati.lDatiRetributivi != null && datiCalcoloOrdinati.lDatiRetributivi.Count > 0)
                        areaDatiContributivi.DatiCalcolo.lDatiRetributivi = datiCalcoloOrdinati.lDatiRetributivi;
                }
            }
        }

        private static void StoreDatiMiglioramentiContributiviPrivate(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, AreaDatiContributivi areaDatiContributivi,
            bool isSingleTab, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.IsDomandaMiglioramentiContrattuali(contenitore.DatiPensione) && areaDatiContributivi.DatiCalcoloQuoteMiglioramentiContrattuali != null && areaDatiContributivi.DatiCalcoloQuoteMiglioramentiContrattuali.LQuoteMiglioramentiContrattuali != null && areaDatiContributivi.DatiCalcoloQuoteMiglioramentiContrattuali.LQuoteMiglioramentiContrattuali.Count > 0)
                GestioneContrib.StoreDatiCalcoloQuoteMiglioramentiContributiviByDatiPensione(ref contenitore, ref contenitoreDecodifica, areaDatiContributivi.DatiCalcoloQuoteMiglioramentiContrattuali, out messaggioVideo);

        }
        #endregion Dati Calcolo

        #region Vittime Terrorismo

        public AreaEsito StoreDatiVittimeTerrorismo(long numeroDomanda, AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            Utility.TipoCalcolo tipoCalcolo = Utility.GetTipoCalcolo(contenitore.DatiPensione);

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            try
            {
                StoreDatiVittimeTerrorismoPrivate(ref contenitore, ref contenitoreDecodifica, areaDatiContributivi, tipoCalcolo, true, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nel salvataggio dei dati calcolo Vittime. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito CancelDatiVittimeTerrorismo(long numeroDomanda)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            string msgVideo = string.Empty;

            GestioneContrib.DeleteDatiCalcoloVittimeTerrorismo(ref contenitore, out msgVideo);

            if (!String.IsNullOrEmpty(msgVideo))
            {
                Esito.Messaggio = msgVideo;
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                return Esito;
            }

            Esito.Messaggio = string.Empty;
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;

            return Esito;
        }

        private void StoreDatiVittimeTerrorismoPrivate(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            AreaDatiContributivi areaDatiContributivi, Utility.TipoCalcolo tipoCalcolo, bool isSingleTab, out string messaggioControllo)
        {
            messaggioControllo = string.Empty;

            GestioneContrib.StoreDatiCalcoloVittimeTerrorismoByDatiPensione(ref contenitore, ref contenitoreDecodifica, areaDatiContributivi.DatiCalcoloVittimeTerrorismo, areaDatiContributivi.DatiCalcolo,
                tipoCalcolo, isSingleTab, out messaggioControllo);
        }

        #endregion Vittime Terrorismo

        #region Quota Fondo Integrativo

        public AreaEsito StoreDatiQuotaFondoIntegrativo(long numeroDomanda, AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            Utility.TipoCalcolo tipoCalcolo = Utility.GetTipoCalcolo(contenitore.DatiPensione);

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            try
            {
                StoreDatiQuotaFondoIntegrativoPrivate(ref contenitore, ref contenitoreDecodifica, areaDatiContributivi, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nel salvataggio dei dati calcolo Quota Fondo Integrativo. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito CancelDatiQuotaFondoIntegrativo(long numeroDomanda)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            string msgVideo = string.Empty;

            GestioneContrib.DeleteDatiCalcoloQuotaFondoIntegrativo(ref contenitore, out msgVideo);

            if (!String.IsNullOrEmpty(msgVideo))
            {
                Esito.Messaggio = msgVideo;
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                return Esito;
            }

            Esito.Messaggio = string.Empty;
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;

            return Esito;
        }

        private void StoreDatiQuotaFondoIntegrativoPrivate(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            AreaDatiContributivi areaDatiContributivi, out string messaggioControllo)
        {
            messaggioControllo = string.Empty;

            GestioneContrib.StoreDatiCalcoloQuotaFondoIntegrativoByDatiPensione(ref contenitore, ref contenitoreDecodifica, areaDatiContributivi.DatiQuotaFondoIntegrativo.lDatiQuotaFondoIntegrativo,
                areaDatiContributivi.DatiCalcolo, out messaggioControllo);
        }

        #endregion Quota Fondo Integrativo

        #region Quota Fondo INPGI

        public AreaEsito StoreDatiQuotaFondoINPGI(long numeroDomanda, AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            Utility.TipoCalcolo tipoCalcolo = Utility.GetTipoCalcolo(contenitore.DatiPensione);

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            try
            {
                StoreDatiQuotaFondoINPGIPrivate(ref contenitore, ref contenitoreDecodifica, areaDatiContributivi, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nel salvataggio dei dati calcolo Quota Fondo INPGI. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito CancelDatiQuotaFondoINPGI(long numeroDomanda)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            string msgVideo = string.Empty;

            GestioneContrib.DeleteDatiCalcoloQuotaFondoINPGI(ref contenitore, out msgVideo);

            if (!String.IsNullOrEmpty(msgVideo))
            {
                Esito.Messaggio = msgVideo;
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                return Esito;
            }

            Esito.Messaggio = string.Empty;
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;

            return Esito;
        }

        private void StoreDatiQuotaFondoINPGIPrivate(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            AreaDatiContributivi areaDatiContributivi, out string messaggioControllo)
        {
            messaggioControllo = string.Empty;

            GestioneContrib.StoreDatiCalcoloQuotaFondoINPGIByDatiPensione(ref contenitore, ref contenitoreDecodifica, areaDatiContributivi.DatiQuotaFondoINPGI,
                out messaggioControllo);
        }

        #endregion Quota Fondo INPGI

        //ENG - MEMO 74_2023
        #region Dati ProRata
        public AreaEsito StoreDatiProRata(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, ref AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();

            Esito = StoreDatiProRataPrivate(ref contenitore, matricolaOperatore, sedeOperatore, centroOperativoOperatore, ref areaDatiContributivi);
            return Esito;
        }

        private AreaEsito StoreDatiProRataPrivate(ref EntityBLCommon.ContenitoreObject contenitore, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore,
             ref AreaDatiContributivi areaDatiContributivi)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            GestioneControlliDinamici.ControlloDinamico ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaChiamata_Naci_AllegatiConv_Memo74_2023", out ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023);

            GestioneControlliDinamici.ControlloDinamico ctrlAbilitaChiamata_CUMUL_Memo74_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaChiamata_CUMUL_Memo74_2023", out ctrlAbilitaChiamata_CUMUL_Memo74_2023);
            //ENG - Memo 116/2025
            GestioneControlliDinamici.ControlloDinamico ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaChiamata_Naci_AllegatiConv_Memo116_2025", out ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025);

            if ((ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023 != null && ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023.ValoreControllo == "SI") ||
                (ctrlAbilitaChiamata_CUMUL_Memo74_2023 != null && ctrlAbilitaChiamata_CUMUL_Memo74_2023.ValoreControllo == "SI") ||
                (ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025 != null && ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025.ValoreControllo == "SI"))
            {
                if (areaDatiContributivi.ProRata == null || areaDatiContributivi.ProRata.ElencoStatiEsteri == null)
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = "Nessuno Stato Estero Presente. Non è possibile procedere con il salvataggio";
                    return Esito;
                }
            }

            if (areaDatiContributivi.ProRata != null && areaDatiContributivi.ProRata.ElencoStatiEsteri != null)
            {
                GestioneContrib.StoreStatiEsteri(ref contenitore, areaDatiContributivi.ProRata.ElencoStatiEsteri, out messaggioControllo);
            }

            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
            }
            else
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }

            #region GetData
            if ((ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023 != null && ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023.ValoreControllo == "SI") ||
                 (ctrlAbilitaChiamata_CUMUL_Memo74_2023 != null && ctrlAbilitaChiamata_CUMUL_Memo74_2023.ValoreControllo == "SI") ||
                 (ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025 != null && ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025.ValoreControllo == "SI") ||
                 Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
            {
                List<GestioneContrib.StatoEsteroCumulo> elencoStatiEsteri = null;
                List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo> listaPrestazioniEstere = new List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo>();
                GestioneDatiEsteriCumulo.GetPrestazioniEstereCumuloByIdPensione(contenitore.DatiPensione.Id, out listaPrestazioniEstere);
                GestioneContrib.GetStatiEsteri(contenitore.DatiPensione, listaPrestazioniEstere, matricolaOperatore, sedeOperatore, centroOperativoOperatore, out elencoStatiEsteri, out messaggioControllo);
                if (!string.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }

                if (elencoStatiEsteri != null)
                {
                    areaDatiContributivi.ProRata = new GestioneContrib.ProRata();
                    areaDatiContributivi.ProRata.ElencoStatiEsteri = new List<GestioneContrib.StatoEsteroCumulo>();
                    areaDatiContributivi.ProRata.ElencoStatiEsteri = elencoStatiEsteri;
                }
            }
            #endregion GetData

            return Esito;
        }

        public AreaEsito CancelProRata(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore)
        {
            SetCulture();
            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            try
            {
                GestioneContrib.EliminaStatiEsteri(contenitore.DatiPensione, matricolaOperatore, sedeOperatore, centroOperativoOperatore);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            catch (Exception)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Attenzione non è stato possibile completare la cancellazione.";
            }
            return Esito;
        }

        public AreaEsito CancelProRataSingolo(long idPrestazione, long numeroDomanda)
        {
            SetCulture();
            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            try
            {
                GestioneContrib.EliminaStatoEsteroSingolo(idPrestazione, contenitore.DatiPensione);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            catch (Exception)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Attenzione non è stato possibile completare la cancellazione.";
            }
            return Esito;
        }

        #endregion Dati ProRata
        #region StatiEsteri
        public AreaEsito GetStatiEsteri(long numeroDomanda, short codiceSede, short centroOperativo, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore,
           long idPensione, out List<GestioneContrib.StatoEsteroCumulo> elencoStatiEsteri)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            elencoStatiEsteri = null;
            string messaggioVideo = "";

            GestioneContrib.GetStatiEsteriFromService(numeroDomanda, codiceSede, centroOperativo, matricolaOperatore, sedeOperatore, centroOperativoOperatore, idPensione, out elencoStatiEsteri, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";

            return Esito;
        }

        public AreaEsito RecuperaStatiEsteri(string codStato, string codIstituzione, out string descCodStato, out string descCodIstituzione, out string descCittà, ref AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            string messaggioVideo = "";

            List<GestioneContrib.StatoEsteroCumulo> elencoStatiEsteri = null;
            GestioneContrib.RecuperaDescrizioneStatiEsteri(codStato, codIstituzione, out descCodStato, out descCodIstituzione, out descCittà, out elencoStatiEsteri, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }

            if (elencoStatiEsteri != null)
            {
                areaDatiContributivi.ProRata = new GestioneContrib.ProRata();
                areaDatiContributivi.ProRata.ElencoStatiEsteri = new List<GestioneContrib.StatoEsteroCumulo>();
                areaDatiContributivi.ProRata.ElencoStatiEsteri = elencoStatiEsteri;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";

            return Esito;
        }

        //ENG - MEMO 74_2023
        //ENG - Memo 116/2025
        public AreaEsito CompatibilitàCodiceConvenzioneWithStatoEstero(AreaRichiestaDomanda areaRichiestaDomanda, GestioneContrib.StatoEsteroCumulo stato)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            string messaggioVideo = string.Empty;
            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            GestioneContrib.ControlsCompatibilitàCodiceConvenzioneWithStatoEstero(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico, stato, contenitore.DatiPensioniDatiGenerici.CodiceConvenzioneAgo, contenitore.DatiPensione.DecorrenzaOriginaria, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }
            else
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = "";
            }

            return Esito;
        }


        #endregion StatiEsteri

        #endregion AreaDatiContributivi

        #region AreaLiquidazionePensione

        public AreaEsito GetLiquidazionePensioneByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(contenitore.DatiPensione.Id);

            AreaEsito Esito = new AreaEsito();
            areaLiquidazionePensione = null;
            DatiGenerici datiGenerici = null;
            DatiAssicurativi datiAssicurativi = null;
            DatiIstruttoria datiIstruttoriaEntity = null;
            DatiOpzione datiOpzione = null;
            DatiProvenienza datiProvenienza = null;
            DatiInail datiInail = null;
            EntityBLCommon.DatiContribuzioneEnpals datiContribuzineEnpals = null;
            DatiSentenzaArt4 datiSentenzaArt4 = null;
            DatiSentenze datiSentenze = null;
            DatiLiquidazionePensioneStorico datiLiquidazionePensioneStorico = null;
            List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi = null;
            List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiRetributivi = null;
            GestioneLiquidazionePensione.GetLiquidazionePensione(ref contenitore, ref contenitoreDecodifica, isRiaperturaDomanda, out datiGenerici, out datiAssicurativi, out datiIstruttoriaEntity,
                out datiOpzione, out datiProvenienza, out datiInail, out datiContribuzineEnpals, out datiSentenzaArt4, out datiLiquidazionePensioneStorico, out datiSentenze, out listaDatiContributivi, out listaDatiRetributivi);

            if (datiGenerici != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.DatiGenerici = datiGenerici;
            }

            if (datiAssicurativi != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.DatiAssicurativi = datiAssicurativi;
            }

            if (datiIstruttoriaEntity != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.DatiIstruttoria = datiIstruttoriaEntity;
            }

            if (datiOpzione != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.DatiOpzione = datiOpzione;
            }

            if (datiProvenienza != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.DatiProvenienza = datiProvenienza;
            }

            if (datiInail != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.DatiInail = datiInail;
            }

            if (datiContribuzineEnpals != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.DatiContribuzioneEnpals = datiContribuzineEnpals;
            }

            if (datiLiquidazionePensioneStorico != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.DatiLiquidazionePensioneStorico = datiLiquidazionePensioneStorico;
            }

            if (datiSentenzaArt4 != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.DatiSentenzaArt4 = datiSentenzaArt4;
            }

            if (datiSentenze != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.DatiSentenze = datiSentenze;
            }

            if (listaDatiContributivi != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.IsDatiContributiviPresenti = listaDatiContributivi.Count() > 0 ? true : false;
            }

            if (listaDatiRetributivi != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.IsDatiRetributiviPresenti = listaDatiRetributivi.Count() > 0 ? true : false;
            }


            if (areaLiquidazionePensione == null)
                areaLiquidazionePensione = new AreaLiquidazionePensione();


            GetListeDecodifica(ref contenitore, ref contenitoreDecodifica, ref areaLiquidazionePensione);
            GetCrossProperties(ref contenitore, isRiaperturaDomanda, ref areaLiquidazionePensione);

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        public AreaEsito StoreDatiLiquidazionePensione(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            DateTime dataSistema = Utility.DataSistemaAgo;
            int annoCompetenza = 0;
            GestioneControlliDinamici.GetAnnoCompetenza(Utility.TipoAppartenenza.AGO, out annoCompetenza);

            DatiExCombattente datiExCombattente = null;
            DatiBenefici datiBenefici = null;
            DatiMaggiorazioni datiMaggiorazioni = null;

            ValorizzaDatiForMaggiorazioniBenefici(ref contenitore, out datiExCombattente, out datiBenefici, out datiMaggiorazioni);

            bool isRiaperturaDomanda = contenitore.IsRiaperturaDomanda;
            Utility.TipoCalcolo tipoCalcoloDB = Utility.GetTipoCalcoloById(contenitore.DatiPensione.TipoCalcolo, contenitore.DatiPensione, Utility.TipoAppartenenza.AGO);
            Utility.TipoCalcolo tipoCalcoloView = Utility.GetTipoCalcoloById(areaLiquidazionePensione.DatiGenerici.TipoCalcolo, contenitore.DatiPensione, Utility.TipoAppartenenza.AGO);

            //ENG - VOPGI
            DateTime? dataInizioAssicurazioneDB = contenitore.DatiPensione.InizioAssicurazione.HasValue ? contenitore.DatiPensione.InizioAssicurazione : null;
            DateTime? dataInizioAssicurazioneView = areaLiquidazionePensione != null && areaLiquidazionePensione.DatiAssicurativi != null && areaLiquidazionePensione.DatiAssicurativi.InizioAssicurazione.HasValue ? areaLiquidazionePensione.DatiAssicurativi.InizioAssicurazione : null;

            if (tipoCalcoloDB != tipoCalcoloView) { }
            //bool p2 = areaLiquidazionePensione.TipoCalcolo != datiPensione.TipoCalcolo;

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            StoreDatiGenericiPrivate(ref contenitore, ref contenitoreDecodifica, areaLiquidazionePensione, false, datiExCombattente, datiBenefici, datiMaggiorazioni, dataSistema, annoCompetenza, tipoCalcoloDB,
                tipoCalcoloView, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            StoreDatiAssicurativiPrivate(ref contenitore, ref contenitoreDecodifica, areaLiquidazionePensione, datiBenefici, dataSistema, isRiaperturaDomanda, false, dataInizioAssicurazioneDB,
               dataInizioAssicurazioneView, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                contenitore.DatiQuadroLiquidazionePensione_GetEffettuata = false;
                if (contenitore.DatiQuadroLiquidazionePensione.TabDatiAssicurativi.GetValueOrDefault() == 0)
                    GestioneBypassControllo.SetUnlock(numeroDomanda, typeof(GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_AGO));
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            StoreDatiIstruttoriaPrivate(ref contenitore, ref contenitoreDecodifica, areaLiquidazionePensione, dataSistema, false, isRiaperturaDomanda,
                out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            StoreDatiOpzionePrivate(ref contenitore, areaLiquidazionePensione, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            StoreDatiProvenienzaPrivate(ref contenitore, ref contenitoreDecodifica, areaLiquidazionePensione, false, isRiaperturaDomanda, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            StoreDatiInailPrivate(ref contenitore, areaLiquidazionePensione, dataSistema, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            StoreDatiSentenzaArt4Private(ref contenitore, areaLiquidazionePensione, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            StoreDatiSentenzePrivate(ref contenitore, areaLiquidazionePensione, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) && areaLiquidazionePensione.DatiContribuzioneEnpals != null)
                GestioneContribuzioneEnpals.SalvaEntityDatiContributizioneEnpals(contenitore.DatiPensione, areaLiquidazionePensione.DatiContribuzioneEnpals);

            if (!string.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        #region Dati Generici

        public AreaEsito StoreDatiGenerici(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            DateTime dataSistema = Utility.DataSistemaAgo;
            int annoCompetenza = 0;
            GestioneControlliDinamici.GetAnnoCompetenza(Utility.TipoAppartenenza.AGO, out annoCompetenza);

            DatiExCombattente datiExCombattente = null;
            DatiBenefici datiBenefici = null;
            DatiMaggiorazioni datiMaggiorazioni = null;

            ValorizzaDatiForMaggiorazioniBenefici(ref contenitore, out datiExCombattente, out datiBenefici, out datiMaggiorazioni);

            Utility.TipoCalcolo tipoCalcoloDB = Utility.GetTipoCalcoloById(contenitore.DatiPensione.TipoCalcolo, contenitore.DatiPensione, Utility.TipoAppartenenza.AGO);
            Utility.TipoCalcolo tipoCalcoloView = Utility.GetTipoCalcoloById(areaLiquidazionePensione.DatiGenerici.TipoCalcolo, contenitore.DatiPensione, Utility.TipoAppartenenza.AGO);

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            StoreDatiGenericiPrivate(ref contenitore, ref contenitoreDecodifica, areaLiquidazionePensione, true, datiExCombattente, datiBenefici, datiMaggiorazioni, dataSistema, annoCompetenza, tipoCalcoloDB,
                tipoCalcoloView, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        public AreaEsito CancelDatiGenerici(long numeroDomanda, out AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            DatiExCombattente datiExCombattente = null;
            DatiBenefici datiBenefici = null;
            DatiMaggiorazioni datiMaggiorazioni = null;

            areaLiquidazionePensione = new AreaLiquidazionePensione();

            ValorizzaDatiForMaggiorazioniBenefici(ref contenitore, out datiExCombattente, out datiBenefici, out datiMaggiorazioni);

            bool isRiaperturaDomanda = contenitore.IsRiaperturaDomanda;
            Utility.TipoCalcolo tipoCalcolo = Utility.GetTipoCalcolo(contenitore.DatiPensione);
            DateTime dataSistema = Utility.DataSistemaAgo;

            AreaEsito Esito = new AreaEsito();
            string msgVideo = string.Empty;
            Entity.DatiGenerici datiGenerici = null;
            GestioneLiquidazionePensione.EliminaDatiGenerici(ref contenitore, ref contenitoreDecodifica, datiExCombattente, datiBenefici, datiMaggiorazioni, dataSistema, tipoCalcolo, out msgVideo);
            if (!String.IsNullOrEmpty(msgVideo))
                Esito.Messaggio = msgVideo;

            string msgGet = string.Empty;
            GestioneLiquidazionePensione.GetDatiGenerici(ref contenitore, ref contenitoreDecodifica, isRiaperturaDomanda, out datiGenerici, out msgGet);
            if (!String.IsNullOrEmpty(msgGet))
                Esito.Messaggio = string.Format("{0}<br />{1}", msgVideo, msgGet);

            areaLiquidazionePensione.DatiGenerici = datiGenerici;

            GetListeDecodifica(ref contenitore, ref contenitoreDecodifica, ref areaLiquidazionePensione);
            GetCrossProperties(ref contenitore, isRiaperturaDomanda, ref areaLiquidazionePensione);

            if (!string.IsNullOrEmpty(Esito.Messaggio))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return Esito;
        }

        private void StoreDatiGenericiPrivate(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            AreaLiquidazionePensione areaLiquidazionePensione, bool IsSingleTab, DatiExCombattente datiExCombattente, DatiBenefici datiBenefici,
            DatiMaggiorazioni datiMaggiorazioni, DateTime dataSistema, int annoCompetenza, Utility.TipoCalcolo tipoCalcoloDB, Utility.TipoCalcolo tipoCalcoloView, out string messaggioControllo)
        {
            messaggioControllo = string.Empty;
            DatiAssicurativi datiAssicurativi = null;
            if (IsSingleTab)
            {
                GestioneLiquidazionePensione.GetDatiAssicurativi(ref contenitore, out datiAssicurativi, out messaggioControllo);
                if (!string.IsNullOrEmpty(messaggioControllo))
                    return;
            }
            else
                datiAssicurativi = areaLiquidazionePensione.DatiAssicurativi;

            GestioneLiquidazionePensione.ControlDatiGenerici(ref contenitore, ref contenitoreDecodifica, IsSingleTab, areaLiquidazionePensione.DatiGenerici, datiAssicurativi, areaLiquidazionePensione.DatiProvenienza,
                areaLiquidazionePensione.DatiIstruttoria, datiExCombattente, datiBenefici, datiMaggiorazioni, dataSistema, annoCompetenza, out messaggioControllo);

            if (!String.IsNullOrEmpty(messaggioControllo))
                return;

            GestioneLiquidazionePensione.StoreDatiGenerici(ref contenitore, ref contenitoreDecodifica, areaLiquidazionePensione.DatiGenerici, datiAssicurativi,
                datiExCombattente, datiBenefici, datiMaggiorazioni, areaLiquidazionePensione.DatiIstruttoria, dataSistema, tipoCalcoloDB, tipoCalcoloView, IsSingleTab, false);
        }

        #endregion Dati Generici

        #region Dati Assicurativi

        public AreaEsito StoreDatiAssicurativi(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            DatiBenefici datiBenefici = null;
            GestioneMaggiorazioniBenefici.ValorizzaDatiBeneficiByDatiPensione(ref contenitore, out datiBenefici);

            bool isRiaperturaDomanda = contenitore.IsRiaperturaDomanda;
            DateTime dataSistema = Utility.DataSistemaAgo;

            //ENG - VOPGI
            DateTime? dataInizioAssicurazioneDB = contenitore.DatiPensione != null && contenitore.DatiPensione.InizioAssicurazione.HasValue ? contenitore.DatiPensione.InizioAssicurazione : null;
            DateTime? dataInizioAssicurazioneView = areaLiquidazionePensione != null && areaLiquidazionePensione.DatiAssicurativi != null && areaLiquidazionePensione.DatiAssicurativi.InizioAssicurazione.HasValue ? areaLiquidazionePensione.DatiAssicurativi.InizioAssicurazione : null;

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            StoreDatiAssicurativiPrivate(ref contenitore, ref contenitoreDecodifica, areaLiquidazionePensione, datiBenefici, dataSistema, isRiaperturaDomanda, true, dataInizioAssicurazioneDB,
                dataInizioAssicurazioneView, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                contenitore.DatiQuadroLiquidazionePensione_GetEffettuata = false;
                if (contenitore.DatiQuadroLiquidazionePensione.TabDatiAssicurativi.GetValueOrDefault() == 0)
                    GestioneBypassControllo.SetUnlock(numeroDomanda, typeof(GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_AGO));
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        public AreaEsito CancelDatiAssicurativi(long numeroDomanda, out AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            DatiBenefici datiBenefici = null;
            GestioneMaggiorazioniBenefici.ValorizzaDatiBeneficiByDatiPensione(ref contenitore, out datiBenefici);
            bool isRiaperturaDomanda = contenitore.IsRiaperturaDomanda;

            AreaEsito Esito = new AreaEsito();
            string msgVideo = string.Empty;
            Entity.DatiAssicurativi datiAssicurativi = null;
            areaLiquidazionePensione = new AreaLiquidazionePensione();

            GestioneLiquidazionePensione.EliminaDatiAssicurativi(ref contenitore, ref contenitoreDecodifica, datiBenefici, isRiaperturaDomanda, out msgVideo);
            if (!String.IsNullOrEmpty(msgVideo))
            {
                Esito.Messaggio = msgVideo;
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                return Esito;
            }
            GestioneLiquidazionePensione.GetDatiAssicurativi(ref contenitore, out datiAssicurativi, out msgVideo);
            if (!String.IsNullOrEmpty(msgVideo))
                Esito.Messaggio = msgVideo;

            areaLiquidazionePensione.DatiAssicurativi = datiAssicurativi;

            GetCrossProperties(ref contenitore, isRiaperturaDomanda, ref areaLiquidazionePensione);

            Esito.Messaggio = "";
            if (!String.IsNullOrEmpty(msgVideo))
            {
                Esito.Messaggio = msgVideo;
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
            }
            else
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;

            GestioneBypassControllo.SetUnlock(numeroDomanda, typeof(GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_AGO));

            return Esito;
        }

        private void StoreDatiAssicurativiPrivate(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            AreaLiquidazionePensione areaLiquidazionePensione, DatiBenefici datiBenefici, DateTime dataSistema, bool isRiaperturaDomanda, bool IsSingleTab, DateTime? dataInizioAssicurazioneDB,
            DateTime? dataInizioAssicurazioneView, out string messaggioControllo)
        {
            messaggioControllo = string.Empty;
            DatiGenerici datiGenerici = null;

            if (IsSingleTab)
            {
                GestioneLiquidazionePensione.GetDatiGenerici(ref contenitore, ref contenitoreDecodifica, isRiaperturaDomanda, out datiGenerici, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                    return;
            }
            else
                datiGenerici = areaLiquidazionePensione.DatiGenerici;
            if (!Utility.IsDomandaRicOrTrf_PSO_PMO_DAIAnte2003(contenitore.DatiPensione, isRiaperturaDomanda, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico : null)
                && !GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(contenitore.DatiPensione, GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_AGO.BYPASS_ASS_INDCOM))
            {
                GestioneLiquidazionePensione.ControlDatiAssicurativi(ref contenitore, ref contenitoreDecodifica, IsSingleTab, isRiaperturaDomanda, dataSistema, areaLiquidazionePensione.DatiAssicurativi,
                datiGenerici, out messaggioControllo);
            }

            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                contenitore.DatiQuadroLiquidazionePensione_GetEffettuata = false;
                if (contenitore.DatiQuadroLiquidazionePensione.TabDatiAssicurativi.GetValueOrDefault() == 0)
                    GestioneBypassControllo.SetUnlock(contenitore.DatiPensione.NDomus, typeof(GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_AGO));
                return;
            }

            try
            {
                GestioneLiquidazionePensione.StoreDatiAssicurativi(ref contenitore, ref contenitoreDecodifica, areaLiquidazionePensione.DatiAssicurativi, datiGenerici, datiBenefici, isRiaperturaDomanda,
                    IsSingleTab, false, dataInizioAssicurazioneDB, dataInizioAssicurazioneView);
            }
            catch (Exception)
            {
                contenitore.DatiQuadroLiquidazionePensione_GetEffettuata = false;
                if (contenitore.DatiQuadroLiquidazionePensione.TabDatiAssicurativi.GetValueOrDefault() == 0)
                    GestioneBypassControllo.SetUnlock(contenitore.DatiPensione.NDomus, typeof(GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_AGO));
                throw;
            }

        }

        #endregion Dati Assicurativi

        #region Dati Istruttoria

        public AreaEsito StoreDatiIstruttoria(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            DateTime dataSistema = Utility.DataSistemaAgo;

            bool isRiaperturaDomanda = contenitore.IsRiaperturaDomanda;
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            StoreDatiIstruttoriaPrivate(ref contenitore, ref contenitoreDecodifica, areaLiquidazionePensione, dataSistema, true, isRiaperturaDomanda,
                out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        public AreaEsito CancelDatiIstruttoria(long numeroDomanda, out AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            DateTime dataSistema = Utility.DataSistemaAgo;
            bool isRiaperturaDomanda = contenitore.IsRiaperturaDomanda;

            AreaEsito Esito = new AreaEsito();
            Entity.DatiIstruttoria datiIstruttoria = null;
            GestioneLiquidazionePensione.EliminaDatiIstruttoria(ref contenitore, ref contenitoreDecodifica, dataSistema, isRiaperturaDomanda);
            GestioneLiquidazionePensione.GetDatiIstruttoria(ref contenitore, ref contenitoreDecodifica, out datiIstruttoria);

            areaLiquidazionePensione = new AreaLiquidazionePensione();

            areaLiquidazionePensione.DatiIstruttoria = datiIstruttoria;

            GetListeDecodifica(ref contenitore, ref contenitoreDecodifica, ref areaLiquidazionePensione);
            GetCrossProperties(ref contenitore, isRiaperturaDomanda, ref areaLiquidazionePensione);

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";

            return Esito;
        }

        private void StoreDatiIstruttoriaPrivate(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            AreaLiquidazionePensione areaLiquidazionePensione, DateTime dataSistema, bool IsSingleTab, bool isRiaperturaDomanda, out string messaggioControllo)
        {
            messaggioControllo = string.Empty;
            GestioneLiquidazionePensione.ControlDatiIstruttoria(ref contenitore, ref contenitoreDecodifica, areaLiquidazionePensione.DatiIstruttoria, areaLiquidazionePensione.DatiGenerici,
                areaLiquidazionePensione.DatiAssicurativi, IsSingleTab, isRiaperturaDomanda, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
                return;
            GestioneLiquidazionePensione.StoreDatiIstruttoria(ref contenitore, ref contenitoreDecodifica, areaLiquidazionePensione.DatiIstruttoria, areaLiquidazionePensione.DatiGenerici, dataSistema,
                IsSingleTab, false, isRiaperturaDomanda);
        }

        #endregion Dati Istruttoria

        #region Dati Opzione

        public AreaEsito StoreDatiOpzione(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            StoreDatiOpzionePrivate(ref contenitore, areaLiquidazionePensione, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        public AreaEsito CancelDatiOpzione(long numeroDomanda)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            GestioneLiquidazionePensione.EliminaDatiOpzione(ref contenitore);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";

            return Esito;
        }

        private void StoreDatiOpzionePrivate(ref EntityBLCommon.ContenitoreObject contenitore, AreaLiquidazionePensione areaLiquidazionePensione, out string messaggioControllo)
        {
            messaggioControllo = string.Empty;
            GestioneLiquidazionePensione.ControlDatiOpzione(areaLiquidazionePensione.DatiOpzione, out messaggioControllo);

            if (!String.IsNullOrEmpty(messaggioControllo))
                return;
            GestioneLiquidazionePensione.StoreDatiOpzione(ref contenitore, areaLiquidazionePensione.DatiOpzione, false);
        }

        #endregion Dati Opzione

        #region Dati Provenienza

        public AreaEsito StoreDatiProvenienza(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            bool isRiaperturaDomanda = contenitore.IsRiaperturaDomanda;

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            StoreDatiProvenienzaPrivate(ref contenitore, ref contenitoreDecodifica, areaLiquidazionePensione, true, isRiaperturaDomanda, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        public AreaEsito CancelDatiProvenienza(long numeroDomanda)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            GestioneLiquidazionePensione.EliminaDatiProvenienza(ref contenitore);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";

            return Esito;
        }

        private void StoreDatiProvenienzaPrivate(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            AreaLiquidazionePensione areaLiquidazionePensione, bool IsSingleTab, bool isRiaperturaDomanda, out string messaggioControllo)
        {
            messaggioControllo = string.Empty;
            DatiGenerici datiGenerici = null;
            DatiAssicurativi datiAssicurativi = null;
            if (IsSingleTab)
            {
                GestioneLiquidazionePensione.GetDatiGenerici(ref contenitore, ref contenitoreDecodifica, isRiaperturaDomanda, out datiGenerici, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                    return;
                GestioneLiquidazionePensione.GetDatiAssicurativi(ref contenitore, out datiAssicurativi, out messaggioControllo);
                if (!string.IsNullOrEmpty(messaggioControllo))
                    return;
            }
            else
            {
                datiGenerici = areaLiquidazionePensione.DatiGenerici;
                datiAssicurativi = areaLiquidazionePensione.DatiAssicurativi;
            }

            if (!(Utility.IsDomandaBancRicAnte1991(contenitore.DatiPensione, contenitore.DatiDanteCausa)) && !Utility.IsDomandaRicOrTrf_PSO_PMO_DAIAnte2003(contenitore.DatiPensione, isRiaperturaDomanda, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico : null))
            {
                GestioneLiquidazionePensione.ControlDatiProvenienza(ref contenitore, ref contenitoreDecodifica, datiGenerici, areaLiquidazionePensione.DatiProvenienza,
                    IsSingleTab, isRiaperturaDomanda, datiAssicurativi, out messaggioControllo);
            }

            if (!String.IsNullOrEmpty(messaggioControllo))
                return;
            GestioneLiquidazionePensione.StoreDatiProvenienza(ref contenitore, areaLiquidazionePensione.DatiProvenienza, false);
        }
        #endregion Dati Provenienza

        #region Dati Inail

        public AreaEsito StoreDatiInail(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            DateTime dataSistema = Utility.DataSistemaAgo;

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            StoreDatiInailPrivate(ref contenitore, areaLiquidazionePensione, dataSistema, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito CancelDatiInail(long numeroDomanda)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            GestioneLiquidazionePensione.EliminaDatiInail(ref contenitore);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        private void StoreDatiInailPrivate(ref EntityBLCommon.ContenitoreObject contenitore, AreaLiquidazionePensione areaLiquidazionePensione, DateTime dataSistema, out string messaggioControllo)
        {
            messaggioControllo = string.Empty;
            GestioneLiquidazionePensione.ControlDatiInail(contenitore.DatiPensione.DecorrenzaOriginaria, areaLiquidazionePensione.DatiInail, dataSistema, out messaggioControllo);

            if (!String.IsNullOrEmpty(messaggioControllo))
                return;
            GestioneLiquidazionePensione.StoreDatiInail(ref contenitore, areaLiquidazionePensione.DatiInail);
        }

        #endregion Dati Inail

        #region Dati Sentenza Art. 4
        public AreaEsito StoreDatiSentenzaArt4(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            string messaggioVideo = string.Empty;

            StoreDatiSentenzaArt4Private(ref contenitore, areaLiquidazionePensione, out messaggioVideo);

            if (!String.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        public AreaEsito CancelDatiSentenzaArt4(long numeroDomanda, out AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            bool isRiaperturaDomanda = contenitore.IsRiaperturaDomanda;

            AreaEsito Esito = new AreaEsito();
            string msgVideo = string.Empty;
            Entity.DatiSentenzaArt4 datiSentenzaArt4 = null;
            areaLiquidazionePensione = new AreaLiquidazionePensione();

            GestioneLiquidazionePensione.EliminaDatiSentenzaArt4(ref contenitore);
            GestioneLiquidazionePensione.GetDatiSentenzaArt4(ref contenitore, out datiSentenzaArt4);

            areaLiquidazionePensione.DatiSentenzaArt4 = datiSentenzaArt4;

            GetCrossProperties(ref contenitore, isRiaperturaDomanda, ref areaLiquidazionePensione);

            Esito.Messaggio = "";
            if (!String.IsNullOrEmpty(msgVideo))
                Esito.Messaggio = msgVideo;

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;

            return Esito;
        }

        private void StoreDatiSentenzaArt4Private(ref EntityBLCommon.ContenitoreObject contenitore, AreaLiquidazionePensione areaLiquidazionePensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestioneLiquidazionePensione.StoreDatiSentenzaArt4(ref contenitore, areaLiquidazionePensione.DatiSentenzaArt4, out messaggioVideo);
        }
        #endregion Dati Sentenza Art. 4

        #region Dati Sentenze
        public AreaEsito CancelDatiSentenze(long numeroDomanda)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            GestioneLiquidazionePensione.EliminaDatiSentenze(ref contenitore);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";

            return Esito;
        }

        public AreaEsito StoreDatiSentenze(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            string messaggioVideo = string.Empty;

            StoreDatiSentenzePrivate(ref contenitore, areaLiquidazionePensione, out messaggioVideo);

            if (!String.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        private void StoreDatiSentenzePrivate(ref EntityBLCommon.ContenitoreObject contenitore, AreaLiquidazionePensione areaLiquidazionePensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            GestioneLiquidazionePensione.ControlDatiSentenze(ref contenitore, areaLiquidazionePensione.DatiSentenze, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
                return;
            GestioneLiquidazionePensione.StoreDatiSentenze(ref contenitore, areaLiquidazionePensione.DatiSentenze, out messaggioVideo);
        }
        #endregion Dati Sentenze

        #endregion AreaLiquidazionePensione

        #region AreaMaggiorazioneBenefici

        public AreaEsito GetMaggiorazioniBeneficiByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            DatiExCombattente datiExCombattente = null;
            DatiBenefici datiBenefici = null;
            DatiMaggiorazioni datiMaggiorazioni = null;
            DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;
            DatiMaggiorazioneBeneficiStorico datiMaggiorazioneBeneficiStorico = null;

            GestioneMaggiorazioniBenefici.GetMaggiorazioneBenefici(ref contenitore, out datiExCombattente, out datiBenefici, out datiMaggiorazioni, out datiBeneficioVittimeTerrorismo,
                out datiMaggiorazioneBeneficiStorico);

            bool isRiaperturaDomanda = contenitore.IsRiaperturaDomanda;

            AreaEsito Esito = new AreaEsito();
            areaMaggiorazioniBenefici = null;

            #region Ex Combattente

            if (datiExCombattente != null)
            {
                areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.DatiExCombattente = datiExCombattente;
            }

            #endregion Ex Combattente

            #region Benefici

            GestioneMaggiorazioniBenefici.ValorizzaDatiBeneficiForPrepensionamento(ref contenitore, ref datiBenefici);

            if (datiBenefici != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.DatiBenefici = datiBenefici;
            }

            #endregion Benefici

            #region Maggiorazioni

            if (datiMaggiorazioni != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.DatiMaggiorazioni = datiMaggiorazioni;
            }

            #endregion Maggiorazioni

            #region Beneficio Vittime Terrorismo

            if (datiBeneficioVittimeTerrorismo != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.DatiBeneficioVittimeTerrorismo = datiBeneficioVittimeTerrorismo;
            }

            #endregion Beneficio Vittime Terrorismo

            #region Storico
            if (datiMaggiorazioneBeneficiStorico != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.DatiMaggiorazioneBeneficiStorico = datiMaggiorazioneBeneficiStorico;
            }
            #endregion Storico



            GetListeDecodifica(ref contenitore, ref contenitoreDecodifica, ref areaMaggiorazioniBenefici);

            GetCrossProperties(ref contenitore, datiBenefici, datiBeneficioVittimeTerrorismo, isRiaperturaDomanda, ref areaMaggiorazioniBenefici);

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        public AreaEsito StoreMaggiorazioniBenefici(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            bool isRiaperturaDomanda = contenitore.IsRiaperturaDomanda;
            Utility.TipoCalcolo tipoCalcolo = Utility.GetTipoCalcolo(contenitore.DatiPensione);
            DateTime dataSistema = Utility.DataSistemaAgo;

            AreaEsito Esito = new AreaEsito();

            #region Ex Combattente

            Esito = StoreDatiExCombattentePrivate(ref contenitore, areaMaggiorazioniBenefici, false);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            #endregion Ex Combattente

            #region Benefici

            Esito = StoreDatiBeneficiPrivate(ref contenitore, ref contenitoreDecodifica, areaMaggiorazioniBenefici, false);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            #endregion Benefici

            #region Maggiorazioni

            Esito = StoreDatiMaggiorazioniPrivate(ref contenitore, areaMaggiorazioniBenefici, dataSistema, isRiaperturaDomanda, false);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            #endregion Maggiorazioni

            #region Beneficio Vittime Terrorismo

            Esito = StoreDatiBeneficioVittimeTerrorismoPrivate(ref contenitore, ref contenitoreDecodifica, areaMaggiorazioniBenefici, tipoCalcolo, false);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            #endregion Beneficio Vittime Terrorismo

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        #region DatiBenefici

        public AreaEsito StoreDatiBenefici(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            AreaEsito Esito = new AreaEsito();
            Esito = StoreDatiBeneficiPrivate(ref contenitore, ref contenitoreDecodifica, areaMaggiorazioniBenefici, true);
            return Esito;
        }

        private AreaEsito StoreDatiBeneficiPrivate(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici, bool singleTab)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            if (!singleTab && !GestioneMaggiorazioniBenefici.ControlsVisibleTabs(contenitore.DatiPensione, null, true, null, null))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            else
            {
                GestioneMaggiorazioniBenefici.ControlDatiBenefici(ref contenitore, areaMaggiorazioniBenefici.DatiBenefici, false, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }

                GestioneMaggiorazioniBenefici.StoreDatiBenefici(ref contenitore, ref contenitoreDecodifica, areaMaggiorazioniBenefici.DatiBenefici);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            return Esito;
        }

        public AreaEsito CancelDatiBenefici(long numeroDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            DatiExCombattente datiExCombattente = null;
            DatiBenefici datiBenefici = null;
            DatiMaggiorazioni datiMaggiorazioni = null;

            bool isRiaperturaDomanda = contenitore.IsRiaperturaDomanda;

            GestioneMaggiorazioniBenefici.EliminaDatiBenefici(ref contenitore);

            ValorizzaDatiForMaggiorazioniBenefici(ref contenitore, out datiExCombattente, out datiBenefici, out datiMaggiorazioni);

            AreaEsito Esito = new AreaEsito();

            areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();

            GestioneMaggiorazioniBenefici.ValorizzaDatiBeneficiForPrepensionamento(ref contenitore, ref datiBenefici);

            if (datiBenefici != null)
                areaMaggiorazioniBenefici.DatiBenefici = datiBenefici;

            GetListeDecodifica(ref contenitore, ref contenitoreDecodifica, ref areaMaggiorazioniBenefici);

            DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;
            GestioneMaggiorazioniBenefici.GetDatiBeneficioVittimeTerrorismo(ref contenitore, out datiBeneficioVittimeTerrorismo);

            GetCrossProperties(ref contenitore, datiBenefici, datiBeneficioVittimeTerrorismo, isRiaperturaDomanda, ref areaMaggiorazioniBenefici);

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        #endregion DatiBenefici

        #region DatiExCombattente

        public AreaEsito StoreDatiExCombattente(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            Esito = StoreDatiExCombattentePrivate(ref contenitore, areaMaggiorazioniBenefici, true);
            return Esito;
        }

        public AreaEsito CancelDatiExCombattente(long numeroDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
            AreaEsito Esito = new AreaEsito();
            GestioneMaggiorazioniBenefici.EliminaDatiExCombattente(ref contenitore);

            DatiExCombattente datiExCombattente = null;
            GestioneMaggiorazioniBenefici.ValorizzaDatiExCombattente(ref contenitore, out datiExCombattente);
            if (datiExCombattente != null)
                areaMaggiorazioniBenefici.DatiExCombattente = datiExCombattente;

            GetListeDecodifica(ref contenitore, ref contenitoreDecodifica, ref areaMaggiorazioniBenefici);

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        private AreaEsito StoreDatiExCombattentePrivate(ref EntityBLCommon.ContenitoreObject contenitore, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici, bool singleTab)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;


            if (!singleTab && !GestioneMaggiorazioniBenefici.ControlsVisibleTabs(contenitore.DatiPensione, true, null, null, null))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            else
            {
                GestioneMaggiorazioniBenefici.ControlDatiExCombattente(ref contenitore, areaMaggiorazioniBenefici.DatiExCombattente, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }
                GestioneMaggiorazioniBenefici.StoreDatiExCombattente(ref contenitore, areaMaggiorazioniBenefici.DatiExCombattente);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            return Esito;
        }

        #endregion DatiExCombattente

        #region DatiMaggiorazioni

        public AreaEsito StoreDatiMaggiorazioni(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            bool isRiaperturaDomanda = contenitore.IsRiaperturaDomanda;
            DateTime dataSistema = Utility.DataSistemaAgo;

            AreaEsito Esito = new AreaEsito();
            Esito = StoreDatiMaggiorazioniPrivate(ref contenitore, areaMaggiorazioniBenefici, dataSistema, isRiaperturaDomanda, true);
            return Esito;
        }

        private AreaEsito StoreDatiMaggiorazioniPrivate(ref EntityBLCommon.ContenitoreObject contenitore, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici, DateTime dataSistema, bool isRiaperturaDomanda,
            bool singleTab)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            if (!singleTab && !GestioneMaggiorazioniBenefici.ControlsVisibleTabs(contenitore.DatiPensione, null, null, true, null))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            else
            {
                try
                {
                    GestioneMaggiorazioniBenefici.ControlDatiMaggiorazioni(ref contenitore, areaMaggiorazioniBenefici.DatiMaggiorazioni, false, isRiaperturaDomanda, dataSistema, out messaggioControllo);
                    if (!String.IsNullOrEmpty(messaggioControllo))
                    {
                        contenitore.DatiQuadroMaggiorazioniBenefici_GetEffettuata = false;
                        if (contenitore.DatiQuadroMaggiorazioniBenefici != null && contenitore.DatiQuadroMaggiorazioniBenefici.TabMaggiorazioni != 2)
                            GestioneBypassControllo.SetUnlock(contenitore.DatiPensione.NDomus, typeof(GestioneBypassControllo.NomeBypass.MaggiorazioniBenefici_Maggiorazioni_AGO));
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = messaggioControllo;
                        return Esito;
                    }

                    GestioneMaggiorazioniBenefici.StoreDatiMaggiorazioni(ref contenitore, areaMaggiorazioniBenefici.DatiMaggiorazioni);
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = string.Empty;
                }
                catch (Exception)
                {
                    try
                    {
                        contenitore.DatiQuadroMaggiorazioniBenefici_GetEffettuata = false;
                        if (contenitore.DatiQuadroMaggiorazioniBenefici != null && contenitore.DatiQuadroMaggiorazioniBenefici.TabMaggiorazioni != 2)
                            GestioneBypassControllo.SetUnlock(contenitore.DatiPensione.NDomus, typeof(GestioneBypassControllo.NomeBypass.MaggiorazioniBenefici_Maggiorazioni_AGO));
                    }
                    catch (Exception)
                    {
                        // Eccezione ignorata
                    }
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = "Errore tecnico durante il salvataggio dei dati Maggiorazioni.";
                    return Esito;
                }
            }
            return Esito;
        }

        public AreaEsito CancelDatiMaggiorazioni(long numeroDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
            AreaEsito Esito = new AreaEsito();
            GestioneMaggiorazioniBenefici.EliminaDatiMaggiorazioni(ref contenitore);

            DatiMaggiorazioni datiMaggiorazioni = null;
            GestioneMaggiorazioniBenefici.ValorizzaDatiMaggiorazioni(ref contenitore, out datiMaggiorazioni);
            if (datiMaggiorazioni != null)
                areaMaggiorazioniBenefici.DatiMaggiorazioni = datiMaggiorazioni;

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        #endregion DatiMaggiorazioni

        #region DatiBeneficioVittimeTerrorismo

        public AreaEsito StoreDatiBeneficioVittimeTerrorismo(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            Utility.TipoCalcolo tipoCalcolo = Utility.GetTipoCalcolo(contenitore.DatiPensione);

            AreaEsito Esito = new AreaEsito();
            try
            {
                Esito = StoreDatiBeneficioVittimeTerrorismoPrivate(ref contenitore, ref contenitoreDecodifica, areaMaggiorazioniBenefici, tipoCalcolo, true);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nel salvataggio dei dati Vittime. Riprovare più tardi";
            }

            return Esito;
        }

        private AreaEsito StoreDatiBeneficioVittimeTerrorismoPrivate(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            AreaMaggiorazioniBenefici areaMaggiorazioniBenefici, Utility.TipoCalcolo tipoCalcolo, bool singleTab)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            bool isBeneficioVittimeTerrorismo = Utility.IsDomandaBeneficioTerrorismoOver80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo) ||
                Utility.IsDomandaBeneficioTerrorismoUnder80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo);

            if (!singleTab && !GestioneMaggiorazioniBenefici.ControlsVisibleTabs(contenitore.DatiPensione, null, null, null, isBeneficioVittimeTerrorismo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            else
            {
                try
                {
                    GestioneMaggiorazioniBenefici.ControlDatiBeneficioVittimeTerrorismo(ref contenitore, ref contenitoreDecodifica, areaMaggiorazioniBenefici.DatiBeneficioVittimeTerrorismo,
                        tipoCalcolo, out messaggioControllo);
                    if (!String.IsNullOrEmpty(messaggioControllo))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = messaggioControllo;
                        return Esito;
                    }

                    GestioneMaggiorazioniBenefici.StoreDatiBeneficioVittimeTerrorismo(ref contenitore, areaMaggiorazioniBenefici.DatiBeneficioVittimeTerrorismo, tipoCalcolo);
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = string.Empty;
                }
                catch (Exception Ex)
                {
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = "Errore tecnico nel salvataggio dei dati Vittime. Riprovare più tardi";
                }
            }
            return Esito;
        }

        public AreaEsito CancelDatiBeneficioVittimeTerrorismo(long numeroDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            AreaEsito Esito = new AreaEsito();
            areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
            try
            {
                GestioneMaggiorazioniBenefici.EliminaDatiBeneficioVittimeTerrorismo(ref contenitore);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nel salvataggio dei dati calcolo. Riprovare più tardi";
            }

            GetListeDecodifica(ref contenitore, ref contenitoreDecodifica, ref areaMaggiorazioniBenefici);

            return Esito;
        }

        #endregion DatiBeneficioVittimeTerrorismo

        #endregion AreaMaggiorazioneBenefici

        #region AreaBititolarita

        public AreaEsito GetBititolaritaByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaDatiBititolarita areaBititolarita)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            AreaEsito Esito = new AreaEsito();
            areaBititolarita = null;

            #region AltrePensioni
            List<Entity.AltraPensione> LdatiAltraPensione = null;
            GestioneBititolarita.GetDatiAltraPensioneByIdPensione(ref contenitore, out LdatiAltraPensione);
            if (LdatiAltraPensione != null && LdatiAltraPensione.Count() > 0)
            {
                areaBititolarita = new AreaDatiBititolarita();
                areaBititolarita.ElencoAltraPensione = LdatiAltraPensione;
            }

            #endregion AltrePensioni

            #region Liste

            List<GestioneBititolarita.DecodificaEnte> ElencoEnte = null;
            GestioneBititolarita.GetListeDecodificaEnte(ref contenitoreDecodifica, out ElencoEnte);
            if (ElencoEnte != null && ElencoEnte.Count > 0)
            {
                if (areaBititolarita == null)
                    areaBititolarita = new AreaDatiBititolarita();
                areaBititolarita.ElencoDecodificaEnte = ElencoEnte;
            }

            List<GestioneBititolarita.DecCatEnte> ElencoCatEnte = null;
            GestioneBititolarita.GetListeDecCatEnte(ref contenitoreDecodifica, out ElencoCatEnte);
            if (ElencoCatEnte != null && ElencoCatEnte.Count > 0)
            {
                if (areaBititolarita == null)
                    areaBititolarita = new AreaDatiBititolarita();
                areaBititolarita.ElencoCatEnte = ElencoCatEnte;
            }
            #endregion Liste

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        public AreaEsito StoreBititolarita(long numeroDomanda, AreaDatiBititolarita areaBititolarita)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            #region AltraPensione

            Esito = StoreAltraPensionePrivate(numeroDomanda, areaBititolarita, out messaggioControllo);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            #endregion AltraPensione

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        #region AltraPensione

        public AreaEsito StoreAltraPensione(long numeroDomanda, AreaDatiBititolarita areaBititolarita)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            Esito = StoreAltraPensionePrivate(numeroDomanda, areaBititolarita, out messaggioControllo);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;

        }

        private AreaEsito StoreAltraPensionePrivate(long numeroDomanda, AreaDatiBititolarita areaBititolarita, out string messaggioControllo)
        {
            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            messaggioControllo = string.Empty;
            AreaEsito Esito = new AreaEsito();

            GestioneBititolarita.ControlsDatiAltraPensione(ref contenitore, ref contenitoreDecodifica, areaBititolarita.ElencoAltraPensione, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }
            GestioneBititolarita.StoreDatiAltraPensione(ref contenitore, areaBititolarita.ElencoAltraPensione);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito CancelAltraPensione(long numeroDomanda, out AreaDatiBititolarita areaBititolarita)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            areaBititolarita = null;
            AreaEsito Esito = new AreaEsito();
            GestioneBititolarita.DeleteDatiAltraPensione(ref contenitore);

            #region Liste

            List<GestioneBititolarita.DecodificaEnte> ElencoEnte = null;
            GestioneBititolarita.GetListeDecodificaEnte(ref contenitoreDecodifica, out ElencoEnte);
            if (ElencoEnte != null && ElencoEnte.Count > 0)
            {
                areaBititolarita = new AreaDatiBititolarita();
                areaBititolarita.ElencoDecodificaEnte = ElencoEnte;
            }

            List<GestioneBititolarita.DecCatEnte> ElencoCatEnte = null;
            GestioneBititolarita.GetListeDecCatEnte(ref contenitoreDecodifica, out ElencoCatEnte);
            if (ElencoCatEnte != null && ElencoCatEnte.Count > 0)
            {
                if (areaBititolarita == null)
                    areaBititolarita = new AreaDatiBititolarita();
                areaBititolarita.ElencoCatEnte = ElencoCatEnte;
            }
            #endregion Liste

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        #endregion AltraPensione

        #endregion AreaBititolarita

        #region Get Dati & Liste Decodifica & Cross Properties

        private void GetListeDecodifica(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, ref AreaLiquidazionePensione areaLiquidazionePensione)
        {
            List<CDCMMR> listaCDCMMR = null;
            GestioneLiquidazionePensione.GetListaCDCMMR(ref contenitoreDecodifica, out listaCDCMMR);
            if (listaCDCMMR != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.listaCDCMMR = listaCDCMMR;
            }

            List<CodiceParticolare> listaCodiceParticolare = null;
            GestioneLiquidazionePensione.GetListaCodiceParticolare(contenitore.DatiPensione, ref contenitoreDecodifica, out listaCodiceParticolare);
            if (listaCodiceParticolare != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.listaCodiceParticolare = listaCodiceParticolare;
            }

            List<DecodificaLegge44997> listaDecodificaLegge44997 = null;
            GestioneLiquidazionePensione.GetListaCodiceLegge44997(contenitore.DatiPensione, ref contenitoreDecodifica, out listaDecodificaLegge44997);
            if (listaDecodificaLegge44997 != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.listaDecodificaLegge44997 = listaDecodificaLegge44997;
            }

            List<DomandaRicorso> listaDomandaRicorso = null;
            GestioneLiquidazionePensione.GetListaCodiciDomandaRicorso(ref contenitoreDecodifica, out listaDomandaRicorso);
            if (listaDomandaRicorso != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.listaDomandaRicorso = listaDomandaRicorso;
            }

            List<Mobilita> listaMobilita = null;
            GestioneLiquidazionePensione.GetListaCodiciMobilita(ref contenitoreDecodifica, out listaMobilita);
            if (listaMobilita != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.listaMobilita = listaMobilita;
            }

            List<CodiciNatura> listaCodiciNatura_AGO = null;
            GestioneLiquidazionePensione.GetListaCodicNatura(ref contenitore, ref contenitoreDecodifica, out listaCodiciNatura_AGO);
            if (listaCodiciNatura_AGO != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.listaCodiciNatura = listaCodiciNatura_AGO;
            }

            List<DecModalitaLiquidazione> listaDecModalitaLiquidazione = null;
            GestioneLiquidazionePensione.GetListaCodiceModalitaLiquidazione(ref contenitoreDecodifica, out listaDecModalitaLiquidazione);
            if (listaDecModalitaLiquidazione != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.listaModalitaLiquidazione = listaDecModalitaLiquidazione;
            }

            List<DecodificaAzienda> listaDecodificaAziendaEditoria = null;
            GestioneLiquidazionePensione.GetListaAziendaEditoria(ref contenitoreDecodifica, out listaDecodificaAziendaEditoria);
            if (listaDecodificaAziendaEditoria != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.listaAziendaEditoria = listaDecodificaAziendaEditoria;
            }

            List<DecodificaRiconoscimentiInvalidita> listaDecodificaRiconoscimentiInvalidita = null;
            GestioneLiquidazionePensione.GetListaRiconoscimentiInvalidita(ref contenitoreDecodifica, out listaDecodificaRiconoscimentiInvalidita);
            if (listaDecodificaRiconoscimentiInvalidita != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.listaRiconoscimentiInvalidita = listaDecodificaRiconoscimentiInvalidita;
            }

            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
            {
                List<DecodificaDerogaENPALS> listaDecodificaDerogaENPALS = null;
                GestioneLiquidazionePensione.GetListaDecodificaDerogaENPALS(ref contenitoreDecodifica, out listaDecodificaDerogaENPALS);
                if (listaDecodificaDerogaENPALS != null)
                {
                    if (areaLiquidazionePensione == null)
                        areaLiquidazionePensione = new AreaLiquidazionePensione();
                    areaLiquidazionePensione.listaDecodificaDerogaENPALS = listaDecodificaDerogaENPALS;
                }
            }

            if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria))
            {
                List<DecodificaEnteCassaProfessionale> listaDecodificaEnteCassaProfessionale = null;
                GestioneLiquidazionePensione.GetListaDecodificaEnteCassaProfessionale(contenitore.DatiPensione, ref contenitoreDecodifica, out listaDecodificaEnteCassaProfessionale);
                if (listaDecodificaEnteCassaProfessionale != null && listaDecodificaEnteCassaProfessionale.Count > 0)
                {
                    if (areaLiquidazionePensione == null)
                        areaLiquidazionePensione = new AreaLiquidazionePensione();
                    areaLiquidazionePensione.listaDecodificaEnteCassaProfessionale = listaDecodificaEnteCassaProfessionale;
                }
            }

            if (Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria))
            {
                List<DecBancaFideiussione> listaDecodificaBancaFideiussione = null;
                GestioneLiquidazionePensione.GetListaDecodificaBancaFideiussione(contenitore.DatiPensione, ref contenitoreDecodifica, out listaDecodificaBancaFideiussione);
                if (listaDecodificaBancaFideiussione != null && listaDecodificaBancaFideiussione.Count > 0)
                {
                    if (areaLiquidazionePensione == null)
                        areaLiquidazionePensione = new AreaLiquidazionePensione();
                    areaLiquidazionePensione.listaDecodificaBancaFideiussione = listaDecodificaBancaFideiussione;
                }
            }

            if (Utility.IsDomandaESPA(contenitore.DatiPensione.SiglaCategoria))
            {
                List<DecBancaFideiussione> listaDecodificaBancaFideiussione = null;
                GestioneLiquidazionePensione.GetListaDecodificaBancaFideiussioneESPA(contenitore.DatiPensione, ref contenitoreDecodifica, out listaDecodificaBancaFideiussione);
                if (listaDecodificaBancaFideiussione != null && listaDecodificaBancaFideiussione.Count > 0)
                {
                    if (areaLiquidazionePensione == null)
                        areaLiquidazionePensione = new AreaLiquidazionePensione();
                    areaLiquidazionePensione.listaDecodificaBancaFideiussione = listaDecodificaBancaFideiussione;
                }
            }

            List<Entity.DecAziendeScadenzaAssegnoGGmmAAAA> listaAziendeScadenzaAssegnoGGMMAAAA = null;
            GestioneLiquidazionePensione.GetListaAziendeScadenzaAssegnoGGMMAAAA(ref contenitoreDecodifica, out listaAziendeScadenzaAssegnoGGMMAAAA);
            if (listaAziendeScadenzaAssegnoGGMMAAAA != null && listaAziendeScadenzaAssegnoGGMMAAAA.Count > 0)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.ListaAziendeScadenzaAssegnoGGMMAAAA = listaAziendeScadenzaAssegnoGGMMAAAA;
            }

            if (Utility.IsPrepensionamentoEditoriaFiltroEAA(contenitore.DatiPensione))
            {
                List<Entity.DecAnagraficaAccordi> listaAnagraficaAccordi = null;
                GestioneLiquidazionePensione.GetAnagraficaAccordi(ref contenitoreDecodifica, out listaAnagraficaAccordi);
                if (listaAnagraficaAccordi != null && listaAnagraficaAccordi.Count > 0)
                {
                    if (areaLiquidazionePensione == null)
                        areaLiquidazionePensione = new AreaLiquidazionePensione();
                    areaLiquidazionePensione.ListaDecAnagraficaAccordi = listaAnagraficaAccordi;
                }

                List<Entity.DecAnagraficaAziende> listaAnagraficaAziende = null;
                GestioneLiquidazionePensione.GetAnagraficaAziende(ref contenitoreDecodifica, out listaAnagraficaAziende);
                if (listaAnagraficaAziende != null && listaAnagraficaAziende.Count > 0)
                {
                    if (areaLiquidazionePensione == null)
                        areaLiquidazionePensione = new AreaLiquidazionePensione();
                    areaLiquidazionePensione.ListaDecAnagraficaAziende = listaAnagraficaAziende;
                }
            }
            else if (Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(contenitore.DatiPensione))
            {
                List<Entity.DecAnagraficaAccordiPerTipo0171> listaAnagraficaAccordi = null;
                GestioneLiquidazionePensione.GetAnagraficaAccordi(ref contenitoreDecodifica, out listaAnagraficaAccordi);
                if (listaAnagraficaAccordi != null && listaAnagraficaAccordi.Count > 0)
                {
                    if (areaLiquidazionePensione == null)
                        areaLiquidazionePensione = new AreaLiquidazionePensione();
                    areaLiquidazionePensione.ListaDecAnagraficaAccordiPerTipo0171 = listaAnagraficaAccordi;
                }

                List<Entity.DecAnagraficaAziendePerTipo0171> listaAnagraficaAziende = null;
                GestioneLiquidazionePensione.GetAnagraficaAziende(ref contenitoreDecodifica, out listaAnagraficaAziende);
                if (listaAnagraficaAziende != null && listaAnagraficaAziende.Count > 0)
                {
                    if (areaLiquidazionePensione == null)
                        areaLiquidazionePensione = new AreaLiquidazionePensione();
                    areaLiquidazionePensione.ListaDecAnagraficaAziendePerTipo0171 = listaAnagraficaAziende;
                }
            }
            else if (Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(contenitore.DatiPensione))
            {
                List<Entity.DecAnagraficaAccordiPerTipo0179> listaAnagraficaAccordi = null;
                GestioneLiquidazionePensione.GetAnagraficaAccordi(ref contenitoreDecodifica, out listaAnagraficaAccordi);
                if (listaAnagraficaAccordi != null && listaAnagraficaAccordi.Count > 0)
                {
                    if (areaLiquidazionePensione == null)
                        areaLiquidazionePensione = new AreaLiquidazionePensione();
                    areaLiquidazionePensione.ListaDecAnagraficaAccordiPerTipo0179 = listaAnagraficaAccordi;
                }

                List<Entity.DecAnagraficaAziendePerTipo0179> listaAnagraficaAziende = null;
                GestioneLiquidazionePensione.GetAnagraficaAziende(ref contenitoreDecodifica, out listaAnagraficaAziende);
                if (listaAnagraficaAziende != null && listaAnagraficaAziende.Count > 0)
                {
                    if (areaLiquidazionePensione == null)
                        areaLiquidazionePensione = new AreaLiquidazionePensione();
                    areaLiquidazionePensione.ListaDecAnagraficaAziendePerTipo0179 = listaAnagraficaAziende;
                }
            }
            else if (Utility.IsPrepensionamentoEditoriaFiltroEBA(contenitore.DatiPensione))
            {
                List<Entity.DecAnagraficaAccordiLetteraB> listaAnagraficaAccordi = null;
                GestioneLiquidazionePensione.GetAnagraficaAccordi(ref contenitoreDecodifica, out listaAnagraficaAccordi);
                if (listaAnagraficaAccordi != null && listaAnagraficaAccordi.Count > 0)
                {
                    if (areaLiquidazionePensione == null)
                        areaLiquidazionePensione = new AreaLiquidazionePensione();
                    areaLiquidazionePensione.ListaDecAnagraficaAccordiLetteraB = listaAnagraficaAccordi;
                }

                List<Entity.DecAnagraficaAziendeLetteraB> listaAnagraficaAziende = null;
                GestioneLiquidazionePensione.GetAnagraficaAziende(ref contenitoreDecodifica, out listaAnagraficaAziende);
                if (listaAnagraficaAziende != null && listaAnagraficaAziende.Count > 0)
                {
                    if (areaLiquidazionePensione == null)
                        areaLiquidazionePensione = new AreaLiquidazionePensione();
                    areaLiquidazionePensione.ListaDecAnagraficaAziendeLetteraB = listaAnagraficaAziende;
                }
            }

            if (Utility.IsDomandaBancari(contenitore.DatiPensione.SiglaCategoria))
            {
                List<Entity.DecodificaBanchePerSede> listaBanchePerSede = null;
                GestioneLiquidazionePensione.GetListaDecodificaBanchePerSede(ref contenitoreDecodifica, out listaBanchePerSede);
                if (listaBanchePerSede != null && listaBanchePerSede.Count > 0)
                {
                    if (areaLiquidazionePensione == null)
                        areaLiquidazionePensione = new AreaLiquidazionePensione();
                    areaLiquidazionePensione.ListaDecodificaBanchePerSede = listaBanchePerSede;
                }
            }

            if (Utility.IsDomandaINDCOM175(contenitore.DatiPensione) || Utility.IsDomandaINDCOM156(contenitore.DatiPensione) || Utility.IsDomandaINDCOM129(contenitore.DatiPensione))
            {
                List<CtrlScadenzaIndennizzoINDCOM> listaCtrlScadenzaIndennizzoINDCOM;
                GestioneLiquidazionePensione.GetListaCtrlScadenzaIndennizzoINDCOM(ref contenitoreDecodifica, out listaCtrlScadenzaIndennizzoINDCOM);
                if (listaCtrlScadenzaIndennizzoINDCOM != null && listaCtrlScadenzaIndennizzoINDCOM.Count > 0)
                {
                    if (areaLiquidazionePensione == null)
                        areaLiquidazionePensione = new AreaLiquidazionePensione();
                    areaLiquidazionePensione.ListaCtrlScadenzaIndennizzoINDCOM = listaCtrlScadenzaIndennizzoINDCOM;
                }
            }
        }

        private void GetListeDecodifica(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, ref AreaDatiContributivi areaDatiContributivi)
        {
            if (!Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) && !Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria))
            {
                List<DecodificaGestioneCalcoloRetributivo> listaDecodificaGestioneCalcoloRetributivo = null;
                GestioneContrib.GetListaDecodificaGestioneCalcoloRetributivo(ref contenitore, ref contenitoreDecodifica, out listaDecodificaGestioneCalcoloRetributivo);
                if (listaDecodificaGestioneCalcoloRetributivo != null)
                {
                    if (areaDatiContributivi == null)
                        areaDatiContributivi = new AreaDatiContributivi();
                    areaDatiContributivi.listaDecodificaGestioneCalcoloRetributivo = listaDecodificaGestioneCalcoloRetributivo;
                }

                List<DecodificaGestioneCalcoloContributivo> listaDecodificaGestioneCalcoloContributivo = null;
                GestioneContrib.GetListaDecodificaGestioneCalcoloContributivo(ref contenitore, ref contenitoreDecodifica, out listaDecodificaGestioneCalcoloContributivo, areaDatiContributivi.DatiCalcolo);
                if (listaDecodificaGestioneCalcoloContributivo != null)
                {
                    if (areaDatiContributivi == null)
                        areaDatiContributivi = new AreaDatiContributivi();
                    areaDatiContributivi.listaDecodificaGestioneCalcoloContributivo = listaDecodificaGestioneCalcoloContributivo;
                }
            }

            if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria))
            {
                List<DecEnteGestioneFondo> listaDecEnteGestioneFondo = null;
                GestioneContrib.GetListaDecEnteGestioneFondo(contenitore, ref contenitoreDecodifica, out listaDecEnteGestioneFondo);
                if (listaDecEnteGestioneFondo != null && listaDecEnteGestioneFondo.Count > 0)
                {
                    if (areaDatiContributivi == null)
                        areaDatiContributivi = new AreaDatiContributivi();
                    areaDatiContributivi.listaDecEnteGestioneFondo = listaDecEnteGestioneFondo;
                }

                List<DecCodiceTrattenute> listaDecCodiceTrattenute = null;
                GestioneContrib.GetListaDecCodiceTrattenute(ref contenitoreDecodifica, out listaDecCodiceTrattenute);
                if (listaDecCodiceTrattenute != null && listaDecCodiceTrattenute.Count > 0)
                {
                    if (areaDatiContributivi == null)
                        areaDatiContributivi = new AreaDatiContributivi();
                    areaDatiContributivi.ListaDecCodiceTrattenute = listaDecCodiceTrattenute;
                }
            }

            if (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria))
            {
                List<TipoCalcoloVincenteDAI> listaTipoCalcoloVincente = null;
                GestioneContrib.GetListaTipoCalcoloVincenteDAI(ref contenitoreDecodifica, out listaTipoCalcoloVincente);
                if (listaTipoCalcoloVincente != null && listaTipoCalcoloVincente.Count > 0)
                {
                    if (areaDatiContributivi == null)
                        areaDatiContributivi = new AreaDatiContributivi();
                    areaDatiContributivi.ListaTipoCalcoloVincenteDAI = listaTipoCalcoloVincente;
                }
            }

            if (Utility.IsDomandaAnticipataEsattoriali(contenitore.DatiPensione) || (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && contenitore.ListaDatiQuotaFondoIntegrativoStorico != null))
            {
                List<DecodificaGestioneQuotaFondoIntegrativo> listaDecodificaGestioneQuotaFondoIntegrativo = null;

                GestioneContrib.GetListaDecodificaGestioneQuotaFondoIntegrativo(ref contenitore, ref contenitoreDecodifica, out listaDecodificaGestioneQuotaFondoIntegrativo);
                if (listaDecodificaGestioneQuotaFondoIntegrativo != null)
                {
                    if (areaDatiContributivi == null)
                        areaDatiContributivi = new AreaDatiContributivi();
                    areaDatiContributivi.listaDecodificaGestioneQuotaFondoIntegrativo = listaDecodificaGestioneQuotaFondoIntegrativo;
                }
            }

            //ENG - Aggiornamento Memo 68/2022 IOPGI
            //ENG - Spacchettate SOPGI
            if (Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione))
                || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa))
            {
                List<DecodificaGestioneQuotaFondoINPGI> listaDecodificaGestioneQuotaFondoINPGI = null;
                GestioneContrib.GetListaDecodificaGestioneQuotaFondoINPGI(ref contenitore, ref contenitoreDecodifica, out listaDecodificaGestioneQuotaFondoINPGI);
                if (areaDatiContributivi == null)
                    areaDatiContributivi = new AreaDatiContributivi();
                if (listaDecodificaGestioneQuotaFondoINPGI != null)
                {
                    areaDatiContributivi.listaDecodificaGestioneQuotaFondoINPGI = listaDecodificaGestioneQuotaFondoINPGI;
                }
                areaDatiContributivi.IsSettimane707INPGIVisible = GestioneContrib.IsSettimane707INPGIVisible(contenitore.DatiPensione, contenitore.TipoCalcolo, contenitore.DatiDanteCausa);
            }
        }

        private void GetListeDecodifica(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, ref AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            List<Entity.CodiceCieco> listaCodiceCieco = null;
            GestioneMaggiorazioniBenefici.GetListaCodiceCieco(ref contenitoreDecodifica, out listaCodiceCieco);
            if (listaCodiceCieco != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaCodiceCieco = listaCodiceCieco;
            }

            List<Entity.TipoBenefici> listaTipoBenefici = null;
            GestioneMaggiorazioniBenefici.GetListaTipoBenefici(ref contenitore, ref contenitoreDecodifica, out listaTipoBenefici);
            if (listaTipoBenefici != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaTipoBenefici = listaTipoBenefici;
            }

            List<Entity.CodiceMaggiorazioneExCombattente> listaCodiceMaggiorazioneExCombattente = null;
            GestioneMaggiorazioniBenefici.GetListaCodiceMaggiorazioneExCombattente(ref contenitoreDecodifica, out listaCodiceMaggiorazioneExCombattente);
            if (listaCodiceMaggiorazioneExCombattente != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaCodiceMaggiorazioneExCombattente = listaCodiceMaggiorazioneExCombattente;
            }

            List<Entity.SoggettoBeneficiario> listaSoggettoBeneficiario = null;
            GestioneMaggiorazioniBenefici.GetListaSoggettoBeneficiario(ref contenitore, ref contenitoreDecodifica, out listaSoggettoBeneficiario);
            if (listaSoggettoBeneficiario != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaSoggettoBeneficiario = listaSoggettoBeneficiario;
            }

            List<Entity.TipologiaPrestazione> listaTipologiaPrestazione = null;
            GestioneMaggiorazioniBenefici.GetListaTipologiaPrestazione(ref contenitoreDecodifica, out listaTipologiaPrestazione);
            if (listaTipologiaPrestazione != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaTipologiaPrestazione = listaTipologiaPrestazione;
            }

            List<Entity.TipologiaBeneficioTerrorismo> listaTipologiaBeneficioTerrorismo = null;
            GestioneMaggiorazioniBenefici.GetListaTipologiaBeneficioTerrorismo(ref contenitoreDecodifica, out listaTipologiaBeneficioTerrorismo);
            if (listaTipologiaBeneficioTerrorismo != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaTipologiaBeneficioTerrorismo = listaTipologiaBeneficioTerrorismo;
            }
        }

        private void GetCrossProperties(ref EntityBLCommon.ContenitoreObject contenitore, bool isRiaperturaDomanda, ref AreaLiquidazionePensione areaLiquidazionePensione)
        {
            GestioneLiquidazionePensione.TipoSalvaguardia? TipologiaSalvaguardia = null;
            string codiceAziendaFromPatronato = null;
            DateTime? DecorrenzaPensioneDirettaDC = null;
            Dictionary<string, byte?> TipoPensione = null;
            DateTime? dataAssunzioneCarico = null;
            //ENG - Aggiornamento Memo86
            DateTime? dataPrelievoDomanda = null;
            //ENG - RIC REVERSIBILITA
            string tipoSettimaneBeneficio = null;
            Dictionary<string, bool?> lCrossProperties = GestioneLiquidazionePensione.GetCrossProperties(ref contenitore, isRiaperturaDomanda, out TipologiaSalvaguardia, out codiceAziendaFromPatronato,
                out DecorrenzaPensioneDirettaDC, out TipoPensione, out dataAssunzioneCarico, out dataPrelievoDomanda, out tipoSettimaneBeneficio);

            if (areaLiquidazionePensione == null)
                areaLiquidazionePensione = new AreaLiquidazionePensione();

            areaLiquidazionePensione.IsEsenzioneFiscaleEstero = lCrossProperties["IsEsenzioneFiscaleEstero"];
            areaLiquidazionePensione.IsAliquotaTfrEsodati = lCrossProperties["IsAliquotaTfrEsodati"];
            areaLiquidazionePensione.IsRiduzioneRetribVisible = lCrossProperties["IsRiduzioneRetributiva"];
            areaLiquidazionePensione.IsGestioneCOM = lCrossProperties["isGestioneCOM"];
            areaLiquidazionePensione.IsCodiceNatura2Enabled = lCrossProperties["IsCodiceNatura2Enabled"];
            areaLiquidazionePensione.IsSperimentaleDonna = lCrossProperties["IsSperimentaleDonna"];
            areaLiquidazionePensione.IsUsuranti = lCrossProperties["Usuranti"];
            areaLiquidazionePensione.TipologiaSalvaguardia = TipologiaSalvaguardia;
            areaLiquidazionePensione.IsRimpatriatiAlbania = lCrossProperties["IsRimpatriatiAlbania"];
            areaLiquidazionePensione.IsVecchiaiaInvaliditaSupplementare = lCrossProperties["IsVecchiaiaInvaliditaSupplementare"];
            areaLiquidazionePensione.IsDatiExCombattenteENPALSPresenti = lCrossProperties["IsDatiExCombattenteENPALSPresenti"];
            areaLiquidazionePensione.IsDatiBeneficiENPALSPresenti = lCrossProperties["IsDatiBeneficiENPALSPresenti"];
            areaLiquidazionePensione.IsTabPrepensionamentoVisible = lCrossProperties["IsTabPrepensionamentoVisible"];
            areaLiquidazionePensione.IsFlagProvvisoriaCheckedAndEnabled = lCrossProperties["IsFlagProvvisoriaCheckedAndEnabled"];
            areaLiquidazionePensione.IsRipristino = lCrossProperties["IsRipristino"];
            areaLiquidazionePensione.IsRiduzioneRetributivaEnabled = lCrossProperties["IsRiduzioneRetributivaEnabled"];
            areaLiquidazionePensione.IsDomandaTrasformazioneInvalidita = lCrossProperties["IsDomandaTrasformazioneInvalidita"];
            areaLiquidazionePensione.IsDomandaAmianto181FromUnicarpe = lCrossProperties["IsDomandaAmianto181FromUnicarpe"];
            areaLiquidazionePensione.IsDatiBeneficiSalvati = lCrossProperties["IsDatiBeneficiSalvati"];
            areaLiquidazionePensione.IsDomandaVESO92WithFiltroL92 = lCrossProperties["IsDomandaVESO92WithFiltroL92"];
            areaLiquidazionePensione.CodiceAziendaFromPatronato = codiceAziendaFromPatronato;
            areaLiquidazionePensione.IsDatiCalcoloDAIAltraGestionePresent = lCrossProperties["IsDatiCalcoloDAIAltraGestionePresent"];
            areaLiquidazionePensione.IsContribuzioneEnpalsRetributivaVisible = lCrossProperties["IsContribuzioneEnpalsRetributivaVisible"].GetValueOrDefault();
            areaLiquidazionePensione.IsContribuzioneEnpalsContributivaVisible = lCrossProperties["IsContribuzioneEnpalsContributivaVisible"].GetValueOrDefault();
            areaLiquidazionePensione.IsEsenzioneFiscaleVittima = lCrossProperties["IsEsenzioneFiscaleVittima"].GetValueOrDefault();
            areaLiquidazionePensione.IsRequisitiL247_L243Enable = lCrossProperties["IsRequisitiL247_L243Enable"];
            areaLiquidazionePensione.IsCodiceComunicazione3Visible = lCrossProperties["IsCodiceComunicazione3Visible"];
            areaLiquidazionePensione.IsProvvisoriaVisible = lCrossProperties["IsProvvisoriaVisible"];
            areaLiquidazionePensione.DecorrenzaPensioneDirettaDC = DecorrenzaPensioneDirettaDC;
            areaLiquidazionePensione.TipoPensione = TipoPensione;
            areaLiquidazionePensione.IsDecPensAnteAgosto95 = lCrossProperties["DecPensAnteAgosto95"];
            areaLiquidazionePensione.IsBeneficioArt24Comma15BisFromFELPE = lCrossProperties["IsBeneficioArt24Comma15BisFromFELPE"];
            areaLiquidazionePensione.IsPensioneTipoContributivo = lCrossProperties["IsPensioneTipoContributivo"];
            areaLiquidazionePensione.IsPensioneTipoContributivoConOpzione = lCrossProperties["IsPensioneTipoContributivoConOpzione"];
            areaLiquidazionePensione.IsPrepensionamentoEditoriaFiltroEAA = lCrossProperties["IsPrepensionamentoEditoriaFiltroEAA"];
            areaLiquidazionePensione.IsPrepensionamentoEditoriaArt1c154L205_2017 = lCrossProperties["IsPrepensionamentoEditoriaArt1c154L205_2017"];
            areaLiquidazionePensione.IsPrepensionamentoEditoriaArt1c500L160_2019 = lCrossProperties["IsPrepensionamentoEditoriaArt1c500L160_2019"];
            areaLiquidazionePensione.IsBeneficioApePrecociFromFELPE = lCrossProperties["IsBeneficioApePrecociFromFELPE"];
            areaLiquidazionePensione.IsDomandaCasellario = lCrossProperties["IsDomandaCasellario"];
            areaLiquidazionePensione.IsEsenzioneFiscaleEsteroFromDetrazioni = lCrossProperties["IsEsenzioneFiscaleEsteroFromDetrazioni"];
            areaLiquidazionePensione.IsDomandaInabilitaSpecificaENPALS = lCrossProperties["IsDomandaInabilitaSpecificaENPALS"];
            areaLiquidazionePensione.IsPensioneInvaliditaInabilitaENPALSOrCasellario = lCrossProperties["IsPensioneInvaliditaInabilitaENPALSOrCasellario"];
            areaLiquidazionePensione.IsBeneficioInabilitaByPrimoCodiceNatura = lCrossProperties["IsBeneficioInabilitaByPrimoCodiceNatura"];
            areaLiquidazionePensione.IsRichiestaBonusBookingAbilitata = lCrossProperties["IsRichiestaBonusBookingAbilitata"];
            areaLiquidazionePensione.IsRiaperturaPerCausaPersa = lCrossProperties["IsRiaperturaPerCausaPersa"];
            areaLiquidazionePensione.IsScadenzaStoricoValorizzata = lCrossProperties["IsScadenzaStoricoValorizzata"];
            areaLiquidazionePensione.IsRicEnpalsMotiviContributivi = lCrossProperties["IsRicEnpalsMotiviContributivi"];
            areaLiquidazionePensione.IsBeneficioNonVedente = lCrossProperties["IsBeneficioNonVedente"];
            areaLiquidazionePensione.IsDataRinunciaTrattenutaInpdapStorico = lCrossProperties["IsDataRinunciaTrattenutaInpdapStorico"];
            areaLiquidazionePensione.IsBeneficioNonVedenteFromStorico = lCrossProperties["IsBeneficioNonVedenteFromStorico"];
            areaLiquidazionePensione.IsAnte96 = Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, isRiaperturaDomanda);
            areaLiquidazionePensione.IsRichiestaBonus154Abilitata = lCrossProperties["IsRichiestaBonus154Abilitata"];
            areaLiquidazionePensione.IsDomandaESPAFiltroL26 = lCrossProperties["IsDomandaESPAFiltroL26"];
            areaLiquidazionePensione.IsDomandaVESO33FiltroDAP = lCrossProperties["IsDomandaVESO33FiltroDAP"];
            areaLiquidazionePensione.IsDomandaRicTrfCred27GestioneL = lCrossProperties["IsDomandaRicTrfCred27GestioneL"];
            areaLiquidazionePensione.IsEliminataPerCauseVarie = lCrossProperties["IsEliminataPerCauseVarie"];
            areaLiquidazionePensione.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione = lCrossProperties["IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione"];
            areaLiquidazionePensione.IsPrepensionamentoEditoriaFiltroEBA = lCrossProperties["IsPrepensionamentoEditoriaFiltroEBA"];
            areaLiquidazionePensione.IsRicConTerzoCodNaturaZAttEconomica67ProfIndividuale11 = lCrossProperties["IsRicConTerzoCodNaturaZAttEconomica67ProfIndividuale11"];
            areaLiquidazionePensione.DataAssunzioneCarico = dataAssunzioneCarico;

            //ENG - Aggiornamento Memo86
            areaLiquidazionePensione.IsPresenteTrattenutaFondoCreditoDaPrelievo = lCrossProperties["IsPresenteTrattenutaFondoCreditoDaPrelievo"];
            areaLiquidazionePensione.DataPrelievoDomanda = dataPrelievoDomanda;

            //ENG - RIC REVERSIBILITA
            areaLiquidazionePensione.TipoSettimaneBeneficio = tipoSettimaneBeneficio;

            //ENG - RIC/TRF (NO ENPALS): rendere non obbligatori i campi "Attivita Economica" e "Professione Individuale" se dal prelievo arrivano vuoti
            areaLiquidazionePensione.IsAttivitaEconomicaDaPrelievo = lCrossProperties["IsAttivitaEconomicaDaPrelievo"];
            areaLiquidazionePensione.IsProfessioneIndividualeDaPrelievo = lCrossProperties["IsProfessioneIndividualeDaPrelievo"];

            areaLiquidazionePensione.IsMemo74_2023Abilitato = lCrossProperties["IsMemo74_2023Abilitato"];

            //ENG - Memo 108_2024
            areaLiquidazionePensione.IsFlagProvvisoriaFromCumulo = lCrossProperties["IsFlagProvvisoriaFromCumulo"];

            areaLiquidazionePensione.IsBypassCompartoScuolaAttivo = lCrossProperties["IsBypassCompartoScuolaAttivo"];
            areaLiquidazionePensione.IsDomandaCOOP28FiltroDAP = lCrossProperties["IsDomandaCOOP28FiltroDAP"];
        }

        private void GetCrossProperties(ref EntityBLCommon.ContenitoreObject contenitore, DatiBenefici datiBenefici, DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo, bool isRiaperturaDomanda,
            ref AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {

            int? settimane = null;
            Dictionary<string, bool?> lCrossProperties = GestioneMaggiorazioniBenefici.GetCrossProperties(ref contenitore, datiBenefici, datiBeneficioVittimeTerrorismo, isRiaperturaDomanda, out settimane);

            if (areaMaggiorazioniBenefici == null)
                areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();

            areaMaggiorazioniBenefici.IsBeneficioBloccato = lCrossProperties["IsBeneficioBloccato"];
            areaMaggiorazioniBenefici.IsBeneficioExArt80 = lCrossProperties["IsBeneficioExArt80"];
            areaMaggiorazioniBenefici.IsBeneficioMinatori = lCrossProperties["IsBeneficioMinatori"];
            areaMaggiorazioniBenefici.IsDomandaInabilitaIndiretta = lCrossProperties["IsDomandaInabilitaIndiretta"];
            areaMaggiorazioniBenefici.IsVisiblePerSuperstitiOrPMO = lCrossProperties["IsVisiblePerSuperstitiOrPMO"];
            areaMaggiorazioniBenefici.IsBeneficioAmianto181 = lCrossProperties["IsBeneficioAmianto181"];
            areaMaggiorazioniBenefici.IsNumSettimaneBeneficioEnabled = lCrossProperties["IsNumSettimaneBeneficioEnabled"];
            areaMaggiorazioniBenefici.IsBeneficioArt24Comma15BisFromFELPE = lCrossProperties["IsBeneficioArt24Comma15BisFromFELPE"];
            areaMaggiorazioniBenefici.IsPrepensionamentoEditoria = lCrossProperties["IsPrepensionamentoEditoria"];
            areaMaggiorazioniBenefici.IsPrepensionamentoEditoriaArt1c154L205_2017 = lCrossProperties["IsPrepensionamentoEditoriaArt1c154L205_2017"];
            areaMaggiorazioniBenefici.IsPrepensionamentoEditoriaArt1c500L160_2019 = lCrossProperties["IsPrepensionamentoEditoriaArt1c500L160_2019"];
            areaMaggiorazioniBenefici.IsBeneficioApePrecociFromFELPE = lCrossProperties["IsBeneficioApePrecociFromFELPE"];
            areaMaggiorazioniBenefici.IsDomandaPensioneInabilita = lCrossProperties["IsDomandaPensioneInabilita"];
            areaMaggiorazioniBenefici.IsBeneficioVittimeTerrorismo = lCrossProperties["IsBeneficioVittimeTerrorismo"];
            areaMaggiorazioniBenefici.IsBeneficioInabilitaByPrimoCodiceNatura = lCrossProperties["IsBeneficioInabilitaByPrimoCodiceNatura"];
            areaMaggiorazioniBenefici.IsBeneficioUsuranti = lCrossProperties["IsBeneficioUsuranti"];
            areaMaggiorazioniBenefici.IsBeneficioMaggiorazioneAmiantoLegge208_2015 = lCrossProperties["IsBeneficioMaggiorazioneAmiantoLegge208_2015"];
            areaMaggiorazioniBenefici.IsBeneficioNonVedenteByPrimoCodiceNatura = lCrossProperties["IsBeneficioNonVedenteByPrimoCodiceNatura"];
            areaMaggiorazioniBenefici.Settimane = settimane;
            areaMaggiorazioniBenefici.IsPrepensionamentoEditoriaFiltroEBA = lCrossProperties["IsPrepensionamentoEditoriaFiltroEBA"];
        }

        private void GetDatiDBCommon(long numeroDomanda, byte? progStorico, out GestionePensione.DatiPensione datiPensione, out GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon,
            out Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon, out GestionePrepensionamento.DatiPrepensionamento datiPrepensionamentoCommon, bool datiIstruttoriaRequired, bool datiMaggBenRequired, bool datiPrepensionamentoRequired)
        {
            datiPensione = null;
            datiIstruttoriaCommon = null;
            datiMaggiorazioniBeneficiCommon = null;
            datiPrepensionamentoCommon = null;

            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, progStorico, out datiPensione);

            if (datiIstruttoriaRequired)
                GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoriaCommon);

            if (datiMaggBenRequired)
                Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.GetMaggiorazioniBeneficiByIdPensione(datiPensione.Id, out datiMaggiorazioniBeneficiCommon);

            if (datiPrepensionamentoRequired)
                GestionePrepensionamento.GetDatiPrepensionamentoByIdPensione(datiPensione.Id, out datiPrepensionamentoCommon);
        }

        private void ValorizzaDatiForMaggiorazioniBenefici(ref EntityBLCommon.ContenitoreObject contenitore, out DatiExCombattente datiExCombattente, out DatiBenefici datiBenefici,
            out DatiMaggiorazioni datiMaggiorazioni)
        {
            datiExCombattente = null;
            GestioneMaggiorazioniBenefici.ValorizzaDatiExCombattente(ref contenitore, out datiExCombattente);

            datiBenefici = null;
            GestioneMaggiorazioniBenefici.ValorizzaDatiBeneficiByDatiPensione(ref contenitore, out datiBenefici);

            datiMaggiorazioni = null;
            GestioneMaggiorazioniBenefici.ValorizzaDatiMaggiorazioni(ref contenitore, out datiMaggiorazioni);
        }

        private static bool IsPrimoRecordRetrGestioneS(GestionePensione.DatiPensione datiPensione)
        {
            bool ret = false;
            List<string> sigleCategorieAmmesse = new List<string> { "VO", "VOART", "VR", "VOCOM", "VOBANC", "VOP", "VOMIN" };
            if (!Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) && datiPensione.TipoCalcolo != 1 &&
                datiPensione.NaturaPensione != null && datiPensione.NaturaPensione.Substring(1, 1).ToUpperInvariant() == "Y" && sigleCategorieAmmesse.Contains(datiPensione.SiglaCategoria.Trim()))
                ret = true;
            return ret;
        }

        #endregion Get Dati & Liste Decodifica & Cross Properties
    }
}
