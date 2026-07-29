using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts;
using INPS.Pensioni.Liquidazione.Entity;
using System.Threading;
using EntityBLCommon = INPS.Pensioni.Liquidazione.BLCommon.Entity;
using System.Reflection;
using INPS.Pensioni.Liquidazione.ServiceReferences.AggPec;
using INPS.Pensioni.Liquidazione.Service_Reference;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace INPS.Pensioni.Liquidazione.Service
{
    [INPS.DNA.Exceptions.Services.ExceptionShielding]
    public class ServizioLiquidazione : INPS.DNA.Services.ServiceBase, IServizioLiquidazione, IDecodifica, IQuadri
    {
        #region IServizioLiquidazione members

        #region public IServizioLiquidazione members

        #region Culture
        private static void SetCulture()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("it-IT");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("it-IT");
        }
        #endregion Culture

        #region Get Dati Pensione

        public long GetIdPensioneByNumeroDomanda(AreaRichiestaDomanda areaRichiestaDomanda)
        {
            long IdPensione = 0;
            GestionePensione.GetIdPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico, out IdPensione);
            return IdPensione;
        }

        public GestionePensione.DatiPensione GetDatiPensioneByNumeroDomanda(Int64 numeroDomanda, byte? progStorico)
        {
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, progStorico, out datiPensione);
            return datiPensione;
        }

        #endregion Get Dati Pensione

        #region Area Versioni

        public AreaEsito GetListaVersioni(long currentVersionWA, out AreaVersioni areaVersioni)
        {
            SetCulture();

            areaVersioni = new AreaVersioni();
            areaVersioni.ListaVersioni = new Dictionary<string, string>();
            AreaEsito esito = new AreaEsito();

            try
            {
                List<GestioneVersioni.DatiVersioni> elencoVersioni = null;
                GestioneVersioni.GetVersioni(out elencoVersioni);

                Utility.GetListaVersioni(ref elencoVersioni, Utility.ChiaviVersioni.WA, currentVersionWA);
                Utility.GetListaVersioni(ref elencoVersioni, Utility.ChiaviVersioni.WCF, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision);

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

        #endregion Area Versioni

        #region AreaRiepilogo
        public AreaRispostaRiepilogo GetRiepilogoByKey(AreaRichiestaRiepilogo areaRichiestaDomande)
        {
            SetCulture();

            AreaRispostaRiepilogo risposta = new AreaRispostaRiepilogo();
            risposta.Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            risposta.Esito.Messaggio = "";
            string msgNonBloccante = string.Empty;
            try
            {
                String errori = "";
                List<Entity.Anagrafica> elencoAnagrafiche = null;
                List<Entity.Domanda> elencoDomande = null;
                List<Entity.Pensione> elencoPensioni = null;
                GestioneEsitoCalcolo.DatiEsitoCalcolo datiEsitoCalcolo = null;
                Utility.TipoAppartenenza? tipoAppartenenza = null;
                Utility.TipoFondo? tipoFondo = null;
                bool isDomandaDB = false;
                string urlDPI = string.Empty;
                bool isDomandaENPALS = false;
                bool isDomandaINPDAP = false;
                bool isDomandaRiapertura = false;
                bool isDomandaCalcolataProvvisoria = false;
                bool isRicercaManualeDA = false;
                string sedeDiversa = string.Empty;
                bool isNuovoCertificatoGeneratoEnpals = false;
                //ENG - Pensioni Ovunque: gestione nuovo pannello
                bool mostraPanelloMessBloccantePensioniOvunque = false;
                string sedePensioneGP1ALZ6 = string.Empty;
                string codCategoriaPensione = string.Empty;
                string certificatoInseguimentoPensione = string.Empty;
                //ENG - Gestione Popup per Memo 239
                bool mostraPopupMemo239 = false;
                //ENG - Gestione Popup per Memo 239
                bool mostraPopupMemo312023 = false;

                if (areaRichiestaDomande == null)
                    throw new INPS.DNA.DnaValidationException("Errore nella valorizzazione dell'area di richiesta");

                #region valorizzazione parametri ARCA
                Entity.ParametriARCA parametriArca = null;
                if (!ValorizzaParametriARCA(areaRichiestaDomande.MatricolaOperatore, out parametriArca))
                {
                    string messaggio = "Errore nella valorizzazione dei parametri ARCA";
                    string parametri = string.Format("Matricola Operatore: {0}", areaRichiestaDomande.MatricolaOperatore);
                    long numDomanda = 0;
                    long.TryParse(areaRichiestaDomande.NumeroDomanda, out numDomanda);
                    GestioneLogGenerico.SalvaLogGenerico(numDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, null);
                    throw new INPS.DNA.DnaValidationException("Errore nel recupero delle informazioni anagrafiche");
                }
                #endregion valorizzazione parametri ARCA

                if (areaRichiestaDomande.TipoRecupero == AreaRichiestaRiepilogo.TipoRicerca.DatiPersonaliParziali)
                {
                    if (areaRichiestaDomande.DatiParziali == null)
                        throw new INPS.DNA.DnaValidationException("Chiave di ricerca non assegnata correttamente. Non è possibile recuperare i dati richiesti");
                    if (String.IsNullOrEmpty(areaRichiestaDomande.DatiParziali.CodiceFiscale))
                        areaRichiestaDomande.DatiParziali.CodiceFiscale = parametriArca.CodiceFiscaleRichiedente;

                    // ricerca per dati anagrafici parziali
                    if (!GestioneAreaRiepilogo.GetRiepilogo(parametriArca, areaRichiestaDomande.SedeOperatore, areaRichiestaDomande.CentroOperativoOperatore,
                        areaRichiestaDomande.MatricolaOperatore, areaRichiestaDomande.DatiParziali, areaRichiestaDomande.TipoAppRuolo, areaRichiestaDomande.Ruolo,
                        areaRichiestaDomande.IsPaginaConferma, areaRichiestaDomande.IsConsultazione, areaRichiestaDomande.DatiParzialiDanteCausa, out elencoAnagrafiche,
                        out elencoDomande, out elencoPensioni, out isDomandaDB, out urlDPI, out isDomandaENPALS, out isDomandaINPDAP, out isDomandaCalcolataProvvisoria,
                        out isDomandaRiapertura, out sedeDiversa, out isRicercaManualeDA, out isNuovoCertificatoGeneratoEnpals, out errori, out msgNonBloccante,
                        out mostraPanelloMessBloccantePensioniOvunque, out sedePensioneGP1ALZ6, out codCategoriaPensione, out certificatoInseguimentoPensione,
                        areaRichiestaDomande.IsPaginaVisualizzazioneStatoPratiche, out mostraPopupMemo239, out mostraPopupMemo312023))
                        throw new INPS.DNA.DnaValidationException(errori);

                    if (!String.IsNullOrEmpty(errori))
                    {
                        risposta.Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        risposta.Esito.Messaggio = errori;
                        risposta.SedeDiversa = sedeDiversa;
                        //ENG - Pensioni Ovunque: gestione nuovo pannello
                        risposta.MostraPanelloMessBloccantePensioniOvunque = mostraPanelloMessBloccantePensioniOvunque;
                        risposta.SedePensioneGP1ALZ6 = sedePensioneGP1ALZ6;
                        risposta.CodCategoriaPensione = codCategoriaPensione;
                        risposta.CertificatoInseguimentoPensione = certificatoInseguimentoPensione;
                        //ENG  - Gestione Popup Memo 239                     
                        risposta.MostraPopupMemo239 = mostraPopupMemo239 = false;
                        //ENG  - Gestione Popup Memo 31/2023   
                        risposta.MostraPopupMemo312023 = mostraPopupMemo312023 = false;
                        return risposta;
                    }
                }
                else if (areaRichiestaDomande.TipoRecupero == AreaRichiestaRiepilogo.TipoRicerca.CodiceFiscale)
                {
                    // ricerca per codice fiscale
                    if (!GestioneAreaRiepilogo.GetRiepilogo(parametriArca, areaRichiestaDomande.SedeOperatore, areaRichiestaDomande.CentroOperativoOperatore,
                        areaRichiestaDomande.MatricolaOperatore, areaRichiestaDomande.CodiceFiscale, areaRichiestaDomande.TipoAppRuolo, areaRichiestaDomande.Ruolo,
                        areaRichiestaDomande.IsPaginaConferma, areaRichiestaDomande.IsConsultazione, null, areaRichiestaDomande.DatiParzialiDanteCausa,
                        out elencoAnagrafiche, out elencoDomande, out elencoPensioni, out datiEsitoCalcolo, out tipoAppartenenza, out tipoFondo, out isDomandaDB,
                        out urlDPI, out isDomandaENPALS, out isDomandaINPDAP, out isDomandaCalcolataProvvisoria, out isDomandaRiapertura, out sedeDiversa,
                        out isRicercaManualeDA, out isNuovoCertificatoGeneratoEnpals, out errori, out msgNonBloccante,
                        out mostraPanelloMessBloccantePensioniOvunque, out sedePensioneGP1ALZ6, out codCategoriaPensione, out certificatoInseguimentoPensione,
                        areaRichiestaDomande.IsPaginaVisualizzazioneStatoPratiche, out mostraPopupMemo239, out mostraPopupMemo312023))
                        throw new INPS.DNA.DnaValidationException(errori);

                    if (!String.IsNullOrEmpty(errori))
                    {
                        risposta.Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        risposta.Esito.Messaggio = errori;
                        //return risposta;

                        //ENG - Pensioni Ovunque: gestione nuovo pannello
                        risposta.MostraPanelloMessBloccantePensioniOvunque = mostraPanelloMessBloccantePensioniOvunque;
                        risposta.SedePensioneGP1ALZ6 = sedePensioneGP1ALZ6;
                        risposta.CodCategoriaPensione = codCategoriaPensione;
                        risposta.CertificatoInseguimentoPensione = certificatoInseguimentoPensione;
                        //ENG  - Gestione Popup Memo 239
                        risposta.MostraPopupMemo239 = mostraPopupMemo239 = false;
                        //ENG  - Gestione Popup Memo 31/2023   
                        risposta.MostraPopupMemo312023 = mostraPopupMemo312023 = false;
                    }
                }
                else
                {
                    // ricerca per numero domanda
                    if (!GestioneAreaRiepilogo.GetRiepilogo(parametriArca, areaRichiestaDomande.SedeOperatore, areaRichiestaDomande.CentroOperativoOperatore,
                        areaRichiestaDomande.MatricolaOperatore, areaRichiestaDomande.NumeroDomanda, areaRichiestaDomande.TipoAppRuolo, areaRichiestaDomande.Ruolo,
                        areaRichiestaDomande.IsPaginaConferma, areaRichiestaDomande.IsConsultazione, areaRichiestaDomande.ProgStorico, areaRichiestaDomande.DatiParzialiDanteCausa,
                        out elencoAnagrafiche, out elencoDomande, out elencoPensioni, out datiEsitoCalcolo, out tipoAppartenenza, out tipoFondo, out isDomandaDB, out urlDPI,
                        out isDomandaENPALS, out isDomandaINPDAP, out isDomandaCalcolataProvvisoria, out isDomandaRiapertura, out sedeDiversa,
                        out isRicercaManualeDA, out isNuovoCertificatoGeneratoEnpals, out errori, out msgNonBloccante,
                        out mostraPanelloMessBloccantePensioniOvunque, out sedePensioneGP1ALZ6, out codCategoriaPensione, out certificatoInseguimentoPensione,
                        areaRichiestaDomande.IsPaginaVisualizzazioneStatoPratiche, out mostraPopupMemo239, out mostraPopupMemo312023))
                        throw new INPS.DNA.DnaValidationException(errori);

                    risposta.IsRicercaManualeDA = isRicercaManualeDA;

                    if (!String.IsNullOrEmpty(errori))
                    {
                        risposta.Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        risposta.Esito.Messaggio = errori;
                        risposta.SedeDiversa = sedeDiversa;
                        //ENG - Pensioni Ovunque: gestione nuovo pannello
                        risposta.MostraPanelloMessBloccantePensioniOvunque = mostraPanelloMessBloccantePensioniOvunque;
                        risposta.SedePensioneGP1ALZ6 = sedePensioneGP1ALZ6;
                        risposta.CodCategoriaPensione = codCategoriaPensione;
                        risposta.CertificatoInseguimentoPensione = certificatoInseguimentoPensione;
                        //ENG  - Gestione Popup Memo 239
                        risposta.MostraPopupMemo239 = mostraPopupMemo239 = false;
                        //ENG  - Gestione Popup Memo 31/2023   
                        risposta.MostraPopupMemo312023 = mostraPopupMemo312023 = false;
                        return risposta;
                    }
                }

                if (elencoDomande != null && elencoDomande.Count > 0)
                {
                    risposta.ElencoDomande = new List<AreaRispostaRiepilogo.DatiRiepilogoDomanda>();
                    foreach (Entity.Domanda d in elencoDomande)
                    {
                        AreaRispostaRiepilogo.DatiRiepilogoDomanda dom = new AreaRispostaRiepilogo.DatiRiepilogoDomanda(d, tipoAppartenenza, tipoFondo, urlDPI, isDomandaENPALS, isDomandaINPDAP, isDomandaRiapertura);
                        risposta.ElencoDomande.Add(dom);
                    }
                }

                if (elencoPensioni != null && elencoPensioni.Count > 0)
                {
                    risposta.ElencoPensioni = new List<AreaRispostaRiepilogo.DatiRiepilogoPensione>();
                    foreach (Entity.Pensione p in elencoPensioni)
                    {
                        AreaRispostaRiepilogo.DatiRiepilogoPensione pen = new AreaRispostaRiepilogo.DatiRiepilogoPensione(p);
                        risposta.ElencoPensioni.Add(pen);
                    }
                }

                if (elencoAnagrafiche != null && elencoAnagrafiche.Count == 1)
                {
                    risposta.AnagraficaTitolare = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica(elencoAnagrafiche[0]);
                }
                else if (elencoAnagrafiche != null && elencoAnagrafiche.Count > 1)
                {
                    risposta.ElencoSinonimi = new List<AreaRispostaRiepilogo.DatiRiepilogoSinonimo>();
                    foreach (Entity.Anagrafica s in elencoAnagrafiche)
                    {
                        AreaRispostaRiepilogo.DatiRiepilogoSinonimo sin = new AreaRispostaRiepilogo.DatiRiepilogoSinonimo(s);
                        risposta.ElencoSinonimi.Add(sin);
                    }
                }

                if (datiEsitoCalcolo != null)
                {
                    risposta.EsitoCalcolo = new AreaRispostaRiepilogo.DatiEsitoCalcolo(datiEsitoCalcolo);
                }

                risposta.IsDomandaDB = isDomandaDB;
                risposta.IsDomandaCalcolataProvvisoria = isDomandaCalcolataProvvisoria;
                risposta.SedeDiversa = sedeDiversa;
                risposta.IsNuovoCertificatoGeneratoEnpals = isNuovoCertificatoGeneratoEnpals;
                //ENG - Pensioni Ovunque: gestione nuovo pannello
                risposta.MostraPanelloMessBloccantePensioniOvunque = mostraPanelloMessBloccantePensioniOvunque;
                risposta.SedePensioneGP1ALZ6 = sedePensioneGP1ALZ6;
                risposta.CodCategoriaPensione = codCategoriaPensione;
                risposta.CertificatoInseguimentoPensione = certificatoInseguimentoPensione;
                //ENG - Gestione Popup Memo 239
                risposta.MostraPopupMemo239 = mostraPopupMemo239;
                //ENG  - Gestione Popup Memo 31/2023   
                risposta.MostraPopupMemo312023 = mostraPopupMemo312023;

                if (!String.IsNullOrEmpty(msgNonBloccante))
                {
                    risposta.Esito.MsgNonBloccante = msgNonBloccante;
                }
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                risposta.Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                risposta.Esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                risposta.Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                risposta.Esito.Messaggio = Ex.Message;
                GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, Ex != null ? Ex.Message : string.Empty, areaRichiestaDomande != null ? areaRichiestaDomande.NumeroDomanda : string.Empty, Ex.StackTrace);
                INPS.DNA.Logging.Logger.LogException(Ex);
            }

            return risposta;
        }
        #endregion AreaRiepilogo

        #region AreaTitolare
        public AreaTitolare GetAreaTitolareByDomanda(AreaRichiestaDomanda areaRichiestaDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);
            string errori = "";
            AreaTitolare areaTitolare = new AreaTitolare();
            EntityBLCommon.AreaTitolare areaTitolareBL = null;

            List<EntityBLCommon.Sindacato> listSindacati = null;
            Anagrafica anagrafica = null;

            //ENG - Reversibilita 024
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            try
            {
                GestioneAreaTitolare.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolareBL, out anagrafica, out errori);

                if (areaTitolareBL != null)
                {
                    if (areaTitolareBL.Anagrafica != null)
                        areaTitolare.Anagrafica = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica(anagrafica);

                    if (areaTitolareBL.ElencoStatiCivili != null && areaTitolareBL.ElencoStatiCivili.Count > 0)
                    {
                        areaTitolare.ElencoStatiCiviliTitolare = new List<AreaTitolare.DatiStatoCivileTitolare>();
                        foreach (BLCommon.GestioneAnagrafica.DatiStatoCivile statoCivileBL in areaTitolareBL.ElencoStatiCivili)
                            areaTitolare.ElencoStatiCiviliTitolare.Add(new AreaTitolare.DatiStatoCivileTitolare(statoCivileBL));
                    }
                    else
                    {
                        if (areaTitolare.Anagrafica != null && areaTitolare.Anagrafica.CodiceStatoCivile.HasValue)
                        {
                            areaTitolare.ElencoStatiCiviliTitolare = new List<AreaTitolare.DatiStatoCivileTitolare>();
                            AreaTitolare.DatiStatoCivileTitolare statoCivile = new AreaTitolare.DatiStatoCivileTitolare();
                            statoCivile.Codice = areaTitolare.Anagrafica.CodiceStatoCivile.Value;
                            if (datiPensione != null && datiPensione.DecorrenzaOriginaria.HasValue)
                                statoCivile.Decorrenza = datiPensione.DecorrenzaOriginaria;
                            areaTitolare.ElencoStatiCiviliTitolare.Add(statoCivile);
                        }
                    }

                    if (areaTitolareBL.ElencoResidenzeEstere != null && areaTitolareBL.ElencoResidenzeEstere.Count > 0)
                    {
                        areaTitolare.ElencoResidenzeEstereTitolare = new List<AreaTitolare.DatiResidenzaEsteroTitolare>();
                        foreach (BLCommon.GestioneAnagrafica.DatiResidenzaEstero residenzaEsteroBL in areaTitolareBL.ElencoResidenzeEstere)
                            areaTitolare.ElencoResidenzeEstereTitolare.Add(new AreaTitolare.DatiResidenzaEsteroTitolare(residenzaEsteroBL));
                    }

                    if (areaTitolareBL.Sindacato != null)
                        areaTitolare.Sindacato = new AreaTitolare.DatiSindacato(areaTitolareBL.Sindacato);

                    if (areaTitolareBL.Pensione != null)
                        areaTitolare.Pensione = new AreaTitolare.DatiPensione(areaTitolareBL.Pensione);

                    if (areaTitolareBL.Patronato != null)
                        areaTitolare.Patronato = new AreaTitolare.DatiPatronato(areaTitolareBL.Patronato);
                }

                Utility.StatoPensione? stato = Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.GetValueOrDefault());
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
                    case Utility.StatoPensione.CalcolataNoNoteDebito:
                    case Utility.StatoPensione.CalcolataNo6Scatti:
                        if (areaTitolare.Sindacato != null && !string.IsNullOrEmpty(areaTitolare.Sindacato.CodiceSindacato))
                        {
                            if (!string.IsNullOrEmpty(areaTitolare.Sindacato.DescrizioneSindacato))
                            {
                                BLCommon.Entity.Sindacato s = new BLCommon.Entity.Sindacato();
                                s.Id = areaTitolare.Sindacato.CodiceSindacato.Trim();
                                s.Sigla = areaTitolare.Sindacato.DescrizioneSindacato;
                                s.Descrizione = areaTitolare.Sindacato.DescrizioneSindacato;
                                s.Stato = Utility.StatoSindacato.Attivo;
                                listSindacati = new List<BLCommon.Entity.Sindacato>();
                                listSindacati.Add(s);
                            }
                            else
                                GestioneAreaTitolare.GetElencoSindacatiForCategoria_Codice(areaTitolareBL.Pensione, areaTitolareBL.Sindacato != null ? areaTitolareBL.Sindacato.CodiceSindacato : string.Empty, out listSindacati, out errori);
                        }
                        break;
                    default:
                        GestioneAreaTitolare.GetElencoSindacatiForCategoria_Codice(areaTitolareBL.Pensione, areaTitolareBL.Sindacato != null ? areaTitolareBL.Sindacato.CodiceSindacato : string.Empty, out listSindacati, out errori);
                        break;
                }

                areaTitolare.ElencoSindacati = listSindacati;

                List<GestioneFamiliari.Familiare> listaFamiliari = null;
                List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficaFamiliari = null;
                GestioneFamiliari.GetFamiliariByIdPensione(datiPensione.Id, out listaFamiliari, out listaAnagraficaFamiliari);
                if (listaFamiliari != null && listaFamiliari.Count > 0)
                {
                    //coniuge del dante causa è contitolare
                    if (listaFamiliari.Exists(x => x.CodiceFiscale == anagrafica.CodiceFiscale && x.IsConiugeOrUnitoCivile()))
                        areaTitolare.IsContitolareConiuge = true;
                    else
                        areaTitolare.IsContitolareConiuge = false;

                    //Ex coniuge del dante causa è contitolare
                    if (listaFamiliari.Exists(x => x.CodiceFiscale == anagrafica.CodiceFiscale && x.IsExConiugeOrScioltoDallUnione()))
                        areaTitolare.IsContitolareExConiuge = true;
                    else
                        areaTitolare.IsContitolareExConiuge = false;

                    //ascendente (genitore) del dante causa è contitolare
                    if (listaFamiliari.Exists(x => x.CodiceFiscale == anagrafica.CodiceFiscale && x.IsAscendenteOrGenitore()))
                        areaTitolare.IsContitolareAscendente = true;
                    else
                        areaTitolare.IsContitolareAscendente = false;
                }

                //ENG  - Spacchettate SOPGI
                bool isSpacchettamentoSOPGI = false;
                if (Utility.IsDomandaSOPGI(datiPensione.SiglaCategoria))
                {
                    BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                    BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);
                    isSpacchettamentoSOPGI = Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, datiDanteCausa);
                }

                //ENG - Reversibilita 024
                if (Utility.IsDomandaPensioneIndiretta(datiPensione) ||
                    (Utility.IsDomandaReversibilita(datiPensione) && (Utility.IsDomandaENPALS(datiPensione.Gestione) || Utility.IsDomandaINPDAP(datiPensione.Gestione) || tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT || isSpacchettamentoSOPGI || Utility.IsDomandaSpacchettamentoSO(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) || Utility.IsDomandaSpacchettamentoSOART(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id))
                    || Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) || Utility.IsDomandaSpacchettamentoSR(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)))) || Utility.IsDomandaIndennitaUnaTantum_AGO(datiPensione) || Utility.IsDomandaPMO(datiPensione.SiglaCategoria))
                {
                    BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                    BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

                    if (datiDanteCausa != null)
                    {
                        areaTitolare.IsDecorrenzaDisabledPerSuperstiti = true;
                        if (!areaTitolare.Pensione.DecorrenzaOriginaria.HasValue && datiDanteCausa.DataMorte.HasValue)
                            areaTitolare.Pensione.DecorrenzaOriginaria = Utility.FirstDayOfMonth(datiDanteCausa.DataMorte.Value.AddMonths(1));

                        //figli postumi
                        if (listaFamiliari != null && listaFamiliari.Count > 0)
                        {
                            GestioneFamiliari.Familiare titolare = listaFamiliari.FirstOrDefault(f => f.CodiceFiscale == anagrafica.CodiceFiscale);
                            if (titolare != null)
                            {
                                if (!titolare.SiglaFamiliare.HasValue)
                                    areaTitolare.IsDecorrenzaDisabledPerSuperstiti = false;
                                else
                                {
                                    bool isTitolareMinore = (titolare.SiglaFamiliare == 'M');
                                    if (isTitolareMinore && anagrafica.DataNascita > datiDanteCausa.DataMorte)
                                        areaTitolare.Pensione.DecorrenzaOriginaria = Utility.FirstDayOfMonth(anagrafica.DataNascita.Value.AddMonths(1));
                                }
                            }
                        }
                    }
                }

                if (Utility.IsDomandaRiliquidazione(datiPensione).GetValueOrDefault() && !Utility.IsRiaperturaDomanda(datiPensione.Id))
                {
                    BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                    BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

                    if (datiDanteCausa != null)
                    {
                        if (!(Utility.IsDomandaRipristinoOrRiliquidazioneSuperstiti(datiPensione) && (datiPensione.Tipo == "0026" || datiPensione.Tipo == "0027")))
                        {
                            if (!areaTitolare.Pensione.DecorrenzaOriginaria.HasValue && datiDanteCausa.DataMorte.HasValue)
                                areaTitolare.Pensione.DecorrenzaOriginaria = Utility.FirstDayOfMonth(datiDanteCausa.DataMorte.Value.AddMonths(1));
                        }
                    }
                }

                if (Utility.IsGestioneLavoratriciMadri(datiPensione) && areaTitolare.Anagrafica.Sesso.GetValueOrDefault() == 'F')
                {
                    areaTitolare.IsSceltaLavoratriciMadriVisible = true;
                }
                else
                    areaTitolare.IsSceltaLavoratriciMadriVisible = false;

                if (Utility.IsDomandaVOPGI(datiPensione.SiglaCategoria))
                {
                    if (areaTitolare.Pensione != null)
                    {
                        areaTitolare.Pensione.DirittoAutonomo = datiPensione.DirittoAutonomo;
                        areaTitolare.Pensione.GP1AJ11 = datiPensione.GP1AJ11;
                    }
                }

                if (Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == Utility.TipoAppartenenza.FS)
                {
                    if (tipoFondo == Utility.TipoFondo.DZ || tipoFondo == Utility.TipoFondo.ES || ((tipoFondo == Utility.TipoFondo.PI || tipoFondo == Utility.TipoFondo.PL) && Utility.IsDomandaReversibilita(datiPensione)))
                    {
                        BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                        BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

                        if (datiDanteCausa != null)
                        {
                            if (!areaTitolare.Pensione.DecorrenzaOriginaria.HasValue && datiDanteCausa.DataMorte.HasValue)
                                areaTitolare.Pensione.DecorrenzaOriginaria = Utility.FirstDayOfMonth(datiDanteCausa.DataMorte.Value.AddMonths(1));
                        }
                    }

                    GestioneFondo.DatiFondo datiFondo = null;
                    GestioneFondo.GetFondoDatiGenericiByIdPensione(datiPensione.Id, out datiFondo);

                    if (datiFondo != null && datiFondo.CodiceSpecifico.HasValue)
                    {
                        if (areaTitolare.Pensione != null)
                            areaTitolare.Pensione.CodiceSpecifico = datiFondo.CodiceSpecifico;
                    }
                }

                //ENG - RIC/TRF SPACCHETTATE AGO
                if (Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == Utility.TipoAppartenenza.AGO)
                {
                    if (Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)))
                    {
                        if (!String.IsNullOrEmpty(datiPensione.SiglaCategoria) && !String.IsNullOrEmpty(datiPensione.SiglaCategoria.Trim())
                            && (datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "SO" || datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "SOCOM"
                            || datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "SOART" || datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "SR"))
                        {
                            areaTitolare.Pensione.GP1AJSP = datiPensione.GP1AJSP;
                        }

                    }
                }

                GetCrossProperties(datiPensione, ref areaTitolare);

                //Implementa la gestione della Decorrenza Indiretta Memo Albania
                //Temporaneamente commentata per rilascio urgente 24/04/2025
                //bool flagCheckIndiretta = Utility.IsDomandaPensioneIndiretta(datiPensione);
                //AreaDanteCausa danteCausa;
                //AreaRichiestaDomanda areaRichiestaDomandaDatiDC = new AreaRichiestaDomanda()
                //{
                //    NumeroDomanda = datiPensione.NDomus,
                //    ProgStorico = datiPensione.ProgStorico
                //};
                //GetDanteCausaByDomanda(areaRichiestaDomandaDatiDC, out danteCausa);
                //List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere = null;
                //GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPrestazioniEstere);

                //if (danteCausa == null)
                //{
                //    areaTitolare.Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                //    areaTitolare.Esito.Messaggio = "Errore nel recupero Dati Dante Causa durante i controlli per il Memo Albania";
                //}
                //else if (listaPrestazioniEstere == null)
                //{
                //    areaTitolare.Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                //    areaTitolare.Esito.Messaggio = "Errore nel recupero Dati Prestazioni Estere durante i controlli per il Memo Albania";
                //}
                //else
                //{
                //    if (flagCheckIndiretta)
                //    {
                //        AnagraficaDC anagraficaDC = danteCausa.AnagraficaDC;
                //        if (anagraficaDC == null || !anagraficaDC.DataMorte.HasValue)
                //        {
                //            areaTitolare.Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                //            areaTitolare.Esito.Messaggio = "Attenzione: Impossibile Precedere Al Controllo Della Decorrenza Della Pensione Indiretta. Data Morte Dante Causa Mancante";
                //        }
                //        else
                //        {
                //            bool flagCheckDataMorteDC = DateTime.Compare((DateTime)anagraficaDC.DataMorte, new DateTime(2025, 06, 01)) < 0;
                //            bool controlloAlbania = false;
                //            foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE statoEstero in listaPrestazioniEstere)
                //            {
                //                bool controlloCondiceStatoEE = statoEstero.CodiceStatoEE == "59";
                //                bool controlloCodiceConvenzione = statoEstero.CodiceConvenzione == 61;
                //                if (controlloCondiceStatoEE && controlloCodiceConvenzione)
                //                    controlloAlbania = true;
                //            }
                //            if (flagCheckIndiretta && flagCheckDataMorteDC && controlloAlbania)
                //                areaTitolare.Pensione.DecorrenzaOriginaria = new DateTime(2025, 07, 01);
                //        }
                //    }
                //}

                if (!String.IsNullOrEmpty(errori))
                    throw new INPS.DNA.DnaValidationException(errori);

                if (!GestioneCrossControls.ALL_VerificaResidenzaEsteroTitolare(areaTitolareBL.Anagrafica.ResidenzaEstero, areaTitolareBL.Anagrafica.CodiceComuneResidenza,
                    areaTitolareBL.Anagrafica.FrazioneResidenza, out errori))
                    throw new INPS.DNA.DnaValidationException(errori);

                if (!GestioneCrossControls.ALL_VerificaProvinciaTitolare(areaTitolareBL.Anagrafica.ProvinciaResidenza, out errori))
                    throw new INPS.DNA.DnaValidationException(errori);

                areaTitolare.Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                areaTitolare.Esito.Messaggio = "";
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                areaTitolare.Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                areaTitolare.Esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                areaTitolare.Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                areaTitolare.Esito.Messaggio = "Errore tecnico durante il recupero delle informazioni del titolare";
                string messaggio = Utility.GetMessageFromException(Ex);
                GestioneLogGenerico.SalvaLogGenerico(areaRichiestaDomanda.NumeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, Ex.StackTrace);
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            return areaTitolare;
        }

        public AreaEsito StoreAreaTitolare(AreaTitolare areaTitolare, out bool isTabAnagraficaSaved, out bool isWarning)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaTitolare.Pensione.NDomus, null);
            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(tipoAppartenenza);
            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            AreaEsito Esito = new AreaEsito();
            isTabAnagraficaSaved = false;
            isWarning = false;
            string tmpMsg = string.Empty;

            Esito = StoreAnagrafica(datiPensione, datiAnagrafici, areaTitolare, false, isRiaperturaDomanda, dataSistema, out isTabAnagraficaSaved, out isWarning);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO || (!String.IsNullOrEmpty(Esito.Messaggio) && !isWarning))
                return Esito;

            if (isWarning)
                tmpMsg = Esito.Messaggio;

            Esito = StoreStatoCivile(datiPensione, areaTitolare, false, dataSistema, isRiaperturaDomanda);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO || !String.IsNullOrEmpty(Esito.Messaggio))
                return Esito;

            Esito = StoreResidenzeEstere(datiPensione, areaTitolare, false);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO || !String.IsNullOrEmpty(Esito.Messaggio))
                return Esito;

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = tmpMsg;

            return Esito;
        }

        public AreaEsito AggiornaAnagraficaTitolareByArca(long numeroDomanda, short sedeOperatore, string matricolaOperatore, ref AreaRispostaRiepilogo.DatiRiepilogoAnagrafica anagrafica)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = null;
            GestioneDetrazioniImposta.GetDetrazioniByIdPensione(datiPensione.Id, out datiDetrazioni);

            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiPensioniDatiGenerici);

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(tipoAppartenenza);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            AreaEsito Esito = new AreaEsito();

            try
            {
                Dictionary<Utility.TabAggArca, byte?> listaTabAggARCA = null;
                Entity.Anagrafica anagraficaBL = null;
                #region valorizzazione parametri ARCA
                Entity.ParametriARCA parametriArca = null;
                if (!ValorizzaParametriARCA(matricolaOperatore, out parametriArca))
                {
                    string messaggio = "Errore nella valorizzazione dei parametri ARCA";
                    string parametri = string.Format("Matricola Operatore: {0}", matricolaOperatore);
                    GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, null);
                    throw new INPS.DNA.DnaApplicationException("Errore nel recupero delle informazioni anagrafiche");
                }
                #endregion valorizzazione parametri ARCA
                string errori = "";
                if (!GestioneSoggetti.AggiornaSoggettoByArca(parametriArca, anagrafica.CodiceFiscale, datiPensione.NDomus.ToString(), out anagraficaBL, out errori))
                    throw new INPS.DNA.DnaValidationException(errori);
                if (!string.IsNullOrEmpty(errori))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = errori;
                    return Esito;
                }

                if (anagraficaBL != null)
                {
                    Utility.TipoAppartenenza? tipoApp = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                    GestioneAreaTitolare.ControlsDatiAnagraficaDopoAggiornaARCA(datiPensione, datiIstruttoria, datiDetrazioni, datiPensioniDatiGenerici, tipoApp, anagraficaBL.CodiceComuneResidenza,
                        anagrafica.CodiceComuneResidenza, anagraficaBL.ResidenzaEstero, anagrafica.ResidenzaEstero, anagraficaBL.DataMorte, isRiaperturaDomanda, dataSistema, out listaTabAggARCA);

                    GestionePensione.DatiTitolare titolare = null;
                    GestionePensione.GetTitolareByIdPensione(datiPensione.Id, out titolare);
                    titolare.DataMorte = anagraficaBL.DataMorte;
                    GestionePensione.SalvaTitolare(titolare);

                    GestioneAnagrafica.DatiAnagrafici datiAnagrafica = new GestioneAnagrafica.DatiAnagrafici();
                    Utility.ValorizzaOggetti(anagraficaBL, datiAnagrafica);
                    GestioneAnagrafica.SalvaAnagrafica(datiAnagrafica);

                    #region Aggiornamento semafori ed eliminazione residenze estere e detrazioni
                    GestioneAreaTitolare.AggiornaSemaforiDopoARCA(datiPensione, isRiaperturaDomanda, listaTabAggARCA);

                    foreach (KeyValuePair<Utility.TabAggArca, byte?> tabAggARCA in listaTabAggARCA)
                    {
                        switch (tabAggARCA.Key)
                        {
                            case Utility.TabAggArca.EsenzioneFiscale:
                                if (tabAggARCA.Value == null)
                                    GestioneAreaTitolare.EliminaEsenzioneFiscale(datiPensione.Id, datiIstruttoria);
                                break;
                            case Utility.TabAggArca.ResidenzaEstero:
                                if (tabAggARCA.Value == null)
                                {
                                    long idAnagrafica = 0;
                                    GestioneAnagrafica.GetIdAnagraficaByCodiceFiscale(datiAnagrafica.CodiceFiscale, out idAnagrafica);
                                    GestioneAnagrafica.EliminaResidenzeEstero(idAnagrafica, datiPensione.Id);
                                }
                                break;
                        }
                    }
                    #endregion Aggiornamento semafori ed eliminazione residenze estere e detrazioni

                    anagrafica = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica(anagraficaBL);
                }

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = "";
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            return Esito;
        }

        private void GetCrossProperties(GestionePensione.DatiPensione datiPensione, ref AreaTitolare areaTitolare)
        {
            Dictionary<string, bool> lCrossProperties = GestioneAreaTitolare.GetCrossProperties(datiPensione);

            if (areaTitolare == null)
                areaTitolare = new AreaTitolare();

            areaTitolare.IsEnteIstruttoreFondoExINPDAP = lCrossProperties["IsEnteIstruttoreFondoExINPDAP"];
            areaTitolare.IsRicVoautNoFiltroPavAssunzioneCaricoEntro042024 = lCrossProperties["IsRicVoautNoFiltroPavAssunzioneCaricoEntro042024"];
        }

        #endregion AreaTitolare

        #region AreaStatoCivile

        private AreaEsito StoreStatoCivile(GestionePensione.DatiPensione datiPensione, AreaTitolare areaTitolare, bool IsSingleTabSaved, DateTime dataSistema, bool isRiaperturaDomanda)
        {
            string errori = string.Empty;

            AreaEsito Esito = new AreaEsito();
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            try
            {
                BLCommon.Entity.AreaTitolare areaTitolareBL = new BLCommon.Entity.AreaTitolare();

                if (areaTitolare.ElencoStatiCiviliTitolare != null && areaTitolare.ElencoStatiCiviliTitolare.Count > 0)
                {
                    foreach (AreaTitolare.DatiStatoCivileTitolare statoCivile in areaTitolare.ElencoStatiCiviliTitolare)
                    {
                        areaTitolareBL.ElencoStatiCivili.Add(new BLCommon.GestioneAnagrafica.DatiStatoCivile(statoCivile.Decorrenza, statoCivile.Codice));
                    }
                }
                areaTitolareBL.Pensione.NDomus = areaTitolare.Pensione.NDomus;
                areaTitolareBL.Pensione.DecorrenzaOriginaria = areaTitolare.Pensione.DecorrenzaOriginaria;
                areaTitolareBL.Anagrafica.CodiceFiscale = areaTitolare.Anagrafica.CodiceFiscale;

                if (!GestioneAreaTitolare.SalvaStatoCivile(datiPensione, areaTitolareBL, IsSingleTabSaved, dataSistema, isRiaperturaDomanda, out errori))
                    throw new INPS.DNA.DnaApplicationException(errori);
                else
                {
                    if (!String.IsNullOrEmpty(errori))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        Esito.Messaggio = errori;
                    }
                    else
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        Esito.Messaggio = string.Empty;
                    }
                }
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }

            return Esito;
        }

        public AreaEsito StoreStatoCivile(AreaTitolare areaTitolare)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaTitolare.Pensione.NDomus, null);
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(tipoAppartenenza);
            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            AreaEsito Esito = new AreaEsito();
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            Esito = StoreStatoCivile(datiPensione, areaTitolare, true, dataSistema, isRiaperturaDomanda);

            return Esito;
        }

        public AreaEsito DeleteStatoCivile(long numeroDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            AreaEsito Esito = new AreaEsito();
            string errori = string.Empty;
            try
            {
                if (GestioneAreaTitolare.DeleteStatoCivileByDatiPensione(datiPensione, out errori))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = string.Empty;
                }
                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = errori;
                    throw new INPS.DNA.DnaApplicationException(errori);
                }
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            return Esito;
        }

        #endregion AreaStatoCivile

        #region AreaResidenzeEstere

        private AreaEsito StoreResidenzeEstere(GestionePensione.DatiPensione datiPensione, AreaTitolare areaTitolare, bool IsSingleTabSaved)
        {
            AreaEsito Esito = new AreaEsito();
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            string errori = string.Empty;

            try
            {
                BLCommon.Entity.AreaTitolare areaTitolareBL = new INPS.Pensioni.Liquidazione.BLCommon.Entity.AreaTitolare();
                if (areaTitolare.ElencoResidenzeEstereTitolare != null && areaTitolare.ElencoResidenzeEstereTitolare.Count > 0)
                {
                    foreach (AreaTitolare.DatiResidenzaEsteroTitolare residenzaEstero in areaTitolare.ElencoResidenzeEstereTitolare)
                    {
                        areaTitolareBL.ElencoResidenzeEstere.Add(new BLCommon.GestioneAnagrafica.DatiResidenzaEstero(residenzaEstero.Decorrenza, residenzaEstero.CodCatastaleStatoEE));
                    }
                }
                areaTitolareBL.Pensione.NDomus = areaTitolare.Pensione.NDomus;
                areaTitolareBL.Pensione.DecorrenzaOriginaria = areaTitolare.Pensione.DecorrenzaOriginaria;
                areaTitolareBL.Anagrafica.CodiceFiscale = areaTitolare.Anagrafica.CodiceFiscale;
                areaTitolareBL.Anagrafica.CodiceComuneResidenza = areaTitolare.Anagrafica.CodiceComuneResidenza;

                if (!GestioneAreaTitolare.SalvaResidenzeEstereByDatiPensione(datiPensione, areaTitolareBL, IsSingleTabSaved, out errori))
                    throw new INPS.DNA.DnaApplicationException(errori);
                else
                {
                    if (!String.IsNullOrEmpty(errori))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        Esito.Messaggio = errori;
                    }
                    else
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        Esito.Messaggio = string.Empty;
                    }
                }
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }

            return Esito;
        }

        public AreaEsito StoreResidenzeEstere(AreaTitolare areaTitolare)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaTitolare.Pensione.NDomus, null);
            AreaEsito Esito = new AreaEsito();
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            Esito = StoreResidenzeEstere(datiPensione, areaTitolare, true);

            return Esito;
        }

        public AreaEsito DeleteResidenzeEstere(long numDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numDomanda, null);
            AreaEsito Esito = new AreaEsito();
            string errori = string.Empty;
            try
            {
                if (GestioneAreaTitolare.DeleteResidenzeEstereByDatiPensione(datiPensione, out errori))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = string.Empty;
                }
                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = errori;
                    throw new INPS.DNA.DnaApplicationException(errori);
                }
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            return Esito;
        }

        #endregion AreaResidenzeEstere

        #region AreaDetrazioni
        public AreaEsito GetSoggettiDetrazioniByDomanda(ref AreaDetrazioni areaDetrazioni)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaDetrazioni.DatiInput.NumeroDomanda, areaDetrazioni.DatiInput.ProgStorico);

            try
            {
                string errori = null;
                List<GestioneDetrazioni.Soggetto> elencoSoggetti = null;
                if (!GestioneDetrazioni.GetElencoSoggettiByDatiPensione(datiPensione, out elencoSoggetti, out errori))
                    throw new DNA.DnaValidationException(errori);

                if (areaDetrazioni == null)
                    areaDetrazioni = new AreaDetrazioni();

                areaDetrazioni.ElencoSoggetti = elencoSoggetti;

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = "";
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                DNA.Logging.Logger.LogException(Ex);
            }
            return Esito;
        }

        public AreaEsito GetDetrazioniByDomanda(ref AreaDetrazioni areaDetrazioni)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();

            if (areaDetrazioni == null || areaDetrazioni.DatiInput == null)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Area di input vuota";
                return Esito;
            }

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaDetrazioni.DatiInput.NumeroDomanda, areaDetrazioni.DatiInput.ProgStorico);

            try
            {
                string errori = "";
                string codiceFiscale = null;
                long idAnagrafica = 0; // il campo non serve nel caso del soggetto titolare
                bool isContitolare = false;
                GestioneDetrazioni.RispostaDetrazioni detrazioniBL = null;
                GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
                GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);
                if (!string.IsNullOrEmpty(areaDetrazioni.DatiInput.CodiceFiscale))
                {
                    codiceFiscale = areaDetrazioni.DatiInput.CodiceFiscale;
                    if (codiceFiscale == datiAnagraficiTitolare.CodiceFiscale)
                        isContitolare = false;
                    else
                    {
                        GestioneAnagrafica.DatiAnagrafici datiAnagraficiContitolare = null;
                        GestioneAnagrafica.GetAnagraficaByCodiceFiscale(codiceFiscale, out datiAnagraficiContitolare);
                        idAnagrafica = datiAnagraficiContitolare.Id;
                    }
                }
                else
                {
                    codiceFiscale = datiAnagraficiTitolare.CodiceFiscale;
                    isContitolare = false;
                }

                if (!GestioneDetrazioni.GetDetrazioniByDatiPensione(datiPensione, codiceFiscale, isContitolare, idAnagrafica, out detrazioniBL, out errori))
                    throw new DNA.DnaValidationException(errori);

                if (areaDetrazioni == null)
                    areaDetrazioni = new AreaDetrazioni();

                areaDetrazioni.ValorizzaArea(detrazioniBL);
                areaDetrazioni.IsVariazioneDetrazioni = !Utility.IsEsenzioneFiscaleEsteroAutonomi(datiPensione, datiAnagraficiTitolare != null ? datiAnagraficiTitolare.CodiceComuneResidenza : null);

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = "";
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                DNA.Logging.Logger.LogException(Ex);
            }
            return Esito;
        }

        public AreaEsito VerifyDetrazioniByDomanda(ref AreaDetrazioni areaDetrazioni)
        {
            SetCulture();
            AreaEsito Esito = new AreaEsito();

            if (areaDetrazioni == null || areaDetrazioni.DatiInput == null || areaDetrazioni.Detrazioni == null)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Area di input vuota";
                return Esito;
            }

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaDetrazioni.DatiInput.NumeroDomanda, null);
            //ENG - REVERSIBILITA FS (NO INPDAP/024)
            GestioneControlliDinamici.ControlloDinamico controlloDinamicoDetrazioniContitolari;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DisabilitaDetrazioniObbligatorieContitolariFS", out controlloDinamicoDetrazioniContitolari);
            int annoCompetenzaFS = 0;
            GestioneControlliDinamici.GetAnnoCompetenza(Utility.TipoAppartenenza.FS, out annoCompetenzaFS);
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);
            BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);

            try
            {
                string errori = "";
                GestioneDetrazioni.RispostaDetrazioni ultimaDetrazioniBL = null;

                GestioneDetrazioniImposta.DatiDetrazioni detrazioniBL =
                    new GestioneDetrazioniImposta.DatiDetrazioni(areaDetrazioni.Detrazioni.DetrazioniReddito,
                        areaDetrazioni.Detrazioni.AgevolazionePensionati, areaDetrazioni.Detrazioni.ConiugeOFiglio, areaDetrazioni.Detrazioni.FigliMinori3AnniNoHandicap100,
                        areaDetrazioni.Detrazioni.FigliMinori3AnniNoHandicap50, areaDetrazioni.Detrazioni.FigliMinori3AnniHandicap100, areaDetrazioni.Detrazioni.FigliMinori3AnniHandicap50,
                        areaDetrazioni.Detrazioni.FigliMaggiori3AnniNoHandicap100, areaDetrazioni.Detrazioni.FigliMaggiori3AnniNoHandicap50,
                        areaDetrazioni.Detrazioni.FigliMaggiori3AnniHandicap100, areaDetrazioni.Detrazioni.FigliMaggiori3AnniHandicap50, areaDetrazioni.Detrazioni.AltriFamiliari100,
                        areaDetrazioni.Detrazioni.AltriFamiliari50, areaDetrazioni.Detrazioni.AddizionaleLombardiaVeneto, areaDetrazioni.Detrazioni.NonResidenteSchumacker,
                        areaDetrazioni.Detrazioni.ConvDoppieImposizioni, areaDetrazioni.Detrazioni.DecorrenzaDetrazioneImposte);

                string codiceFiscale = null;
                bool isContitolare = true;
                long idAnagrafica = 0; // il campo non serve nel caso del soggetto titolare
                GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
                GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);
                if (!string.IsNullOrEmpty(areaDetrazioni.DatiInput.CodiceFiscale))
                {
                    codiceFiscale = areaDetrazioni.DatiInput.CodiceFiscale;
                    if (codiceFiscale == datiAnagraficiTitolare.CodiceFiscale)
                        isContitolare = false;
                    else
                    {
                        GestioneAnagrafica.DatiAnagrafici datiAnagraficiContitolare = null;
                        GestioneAnagrafica.GetAnagraficaByCodiceFiscale(codiceFiscale, out datiAnagraficiContitolare);
                        idAnagrafica = datiAnagraficiContitolare.Id;
                    }
                }
                else
                {
                    codiceFiscale = datiAnagraficiTitolare.CodiceFiscale;
                    isContitolare = false;
                }

                List<GestioneDetrazioni.Soggetto> elencoSoggetti = null;
                if (!GestioneDetrazioni.GetElencoSoggettiByDatiPensione(datiPensione, out elencoSoggetti, out errori))
                    throw new DNA.DnaValidationException(errori);
                if (elencoSoggetti != null && elencoSoggetti.Count() > 0)
                    elencoSoggetti.FindAll(x => x.CodiceFiscale == codiceFiscale).ForEach(x => x.Confermato = true);
                bool isSemaforoVerde = elencoSoggetti != null ? !elencoSoggetti.Exists(x => !x.Confermato) : false;

                //ENG - REVERSIBILITA FS (NO INPDAP/024)
                if (controlloDinamicoDetrazioniContitolari != null && !String.IsNullOrEmpty(controlloDinamicoDetrazioniContitolari.ValoreControllo)
                    && controlloDinamicoDetrazioniContitolari.ValoreControllo.ToUpperInvariant() == "SI")
                {
                    if (tipoAppartenenza == Utility.TipoAppartenenza.FS && Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa)
                        && !Utility.IsDomandaINPDAP(datiPensione.Gestione) && tipoFondo != Utility.TipoFondo.FS && tipoFondo != Utility.TipoFondo.PT)
                    {
                        if (elencoSoggetti != null && elencoSoggetti.Count > 0)
                        {
                            List<GestioneDetrazioni.Soggetto> elencoSoggettiDetrazioniObbligatorie = elencoSoggetti.FindAll(x => !x.IsContitolare || (x.IsContitolare && !x.DataCessazione.HasValue) || (x.IsContitolare && x.DataCessazione.HasValue && x.DataCessazione.Value.Year >= annoCompetenzaFS));
                            isSemaforoVerde = elencoSoggettiDetrazioniObbligatorie != null ? !elencoSoggettiDetrazioniObbligatorie.Exists(x => !x.Confermato) : false;
                        }
                    }
                }

                if (!GestioneDetrazioni.VerificaDetrazioniByDatiPensione(datiPensione, codiceFiscale, idAnagrafica, isContitolare, detrazioniBL, isSemaforoVerde, out ultimaDetrazioniBL, out errori))
                    throw new DNA.DnaValidationException(errori);

                areaDetrazioni.ValorizzaArea(ultimaDetrazioniBL);

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = "";
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            return Esito;
        }
        #endregion AreaDetrazioni

        #region AreaAnagrafica
        public AreaEsito GetAnagraficaSoggettoByCodiceFiscale(string codiceFiscale, short sedeOperatore, string matricolaOperatore, string numDomanda,
            out AreaRispostaRiepilogo.DatiRiepilogoAnagrafica anagrafica)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            anagrafica = null;
            try
            {
                Entity.Anagrafica anagraficaBL = null;
                #region valorizzazione parametri ARCA
                Entity.ParametriARCA parametriArca = null;
                if (!ValorizzaParametriARCA(matricolaOperatore, out parametriArca))
                {
                    string messaggio = "Errore nella valorizzazione dei parametri ARCA";
                    string parametri = string.Format("Matricola Operatore: {0}", matricolaOperatore);
                    long numeroDomanda = 0;
                    long.TryParse(numDomanda, out numeroDomanda);
                    GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, null);
                    throw new INPS.DNA.DnaApplicationException("Errore nel recupero delle informazioni anagrafiche");
                }
                #endregion valorizzazione parametri ARCA
                string errori = "";
                if (!GestioneSoggetti.GetSoggettoPerCodiceFiscale(parametriArca, codiceFiscale, numDomanda, out anagraficaBL, out errori))
                    throw new INPS.DNA.DnaValidationException(errori);
                if (!string.IsNullOrEmpty(errori))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = errori;
                    return Esito;
                }

                if (anagraficaBL != null)
                    anagrafica = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica(anagraficaBL);

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = "";
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            return Esito;
        }

        public AreaEsito AggiornaAnagraficaSoggetto(string codiceFiscale, short sedeOperatore, string matricolaOperatore, string numDomanda, out AreaRispostaRiepilogo.DatiRiepilogoAnagrafica anagrafica)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            anagrafica = null;
            try
            {
                Entity.Anagrafica anagraficaBL = null;
                #region valorizzazione parametri ARCA
                Entity.ParametriARCA parametriArca = null;
                if (!ValorizzaParametriARCA(matricolaOperatore, out parametriArca))
                {
                    string messaggio = "Errore nella valorizzazione dei parametri ARCA";
                    string parametri = string.Format("Matricola Operatore: {0}", matricolaOperatore);
                    long numeroDomanda = 0;
                    long.TryParse(numDomanda, out numeroDomanda);
                    GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, null);
                    throw new INPS.DNA.DnaApplicationException("Errore nel recupero delle informazioni anagrafiche");
                }
                #endregion valorizzazione parametri ARCA
                string errori = "";
                if (!GestioneSoggetti.AggiornaSoggettoByArca(parametriArca, codiceFiscale, numDomanda, out anagraficaBL, out errori))
                    throw new INPS.DNA.DnaValidationException(errori);
                if (!string.IsNullOrEmpty(errori))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = errori;
                    return Esito;
                }

                if (anagraficaBL != null)
                    anagrafica = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica(anagraficaBL);

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = "";
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            return Esito;
        }

        private AreaEsito StoreAnagrafica(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagrafici, AreaTitolare areaTitolare, bool IsSingleTabSaved, bool isRiaperturaDomanda, DateTime dataSistema,
            out bool isTabAnagraficaSaved, out bool isWarning)
        {
            string errori = string.Empty;

            AreaEsito Esito = new AreaEsito();
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            isTabAnagraficaSaved = false;
            isWarning = false;

            try
            {
                BLCommon.Entity.AreaTitolare areaTitolareBL = new BLCommon.Entity.AreaTitolare();

                areaTitolareBL.Anagrafica = new BLCommon.GestioneAnagrafica.DatiAnagrafici(areaTitolare.Anagrafica.CodiceFiscale, areaTitolare.Anagrafica.Cittadinanza,
                                            areaTitolare.Anagrafica.Tel, areaTitolare.Anagrafica.Cell, areaTitolare.Anagrafica.EMail, areaTitolare.Anagrafica.DataNascita,
                                            areaTitolare.Anagrafica.ResidenzaEstero, areaTitolare.Anagrafica.CodiceComuneResidenza, areaTitolare.Anagrafica.FrazioneResidenza,
                                            areaTitolare.Anagrafica.ProvinciaResidenza, areaTitolare.Anagrafica.Sesso, areaTitolare.Anagrafica.DataMorte, areaTitolare.Anagrafica.Cognome);

                if (areaTitolare.Pensione != null)
                    areaTitolareBL.Pensione = new BLCommon.GestionePensione.DatiPensione(areaTitolare.Pensione.NDomus, areaTitolare.Pensione.DecorrenzaOriginaria, areaTitolare.Pensione.DataPerfezionamentoRequisiti, areaTitolare.Pensione.CodiceSedeDestinazione, areaTitolare.Pensione.LavoratorePubblico, areaTitolare.Pensione.NumeroFigli, areaTitolare.Pensione.SceltaLavoratriciMadri, areaTitolare.Pensione.DataCondizioniPerComputo, areaTitolare.Pensione.NCertificato);

                if (areaTitolare.Sindacato != null)
                    areaTitolareBL.Sindacato = new BLCommon.GestionePensione.DatiSindacato(areaTitolare.Sindacato.CodiceSindacato, areaTitolare.Sindacato.DescrizioneSindacato,
                        areaTitolare.Sindacato.DecorrenzaSindacato, areaTitolare.Sindacato.CessazioneSindacato, areaTitolare.Sindacato.Stato, areaTitolare.Sindacato.IsFromService);

                if (!GestioneAreaTitolare.SalvaAnagrafica(datiPensione, datiAnagrafici, areaTitolareBL, IsSingleTabSaved, out isTabAnagraficaSaved, isRiaperturaDomanda, dataSistema, out isWarning, out errori))
                    throw new INPS.DNA.DnaApplicationException(errori);
                else
                {
                    if (!String.IsNullOrEmpty(errori))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        Esito.Messaggio = errori;
                    }
                    else
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        if (isWarning)
                            Esito.Messaggio = "Si evidenzia che alla decorrenza pensione risulta perfezionato il requisito anagrafico previsto per la generalità delle lavoratrici.";
                        else
                            Esito.Messaggio = string.Empty;
                    }
                }
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }

            return Esito;
        }

        public AreaEsito StoreAnagrafica(AreaTitolare areaTitolare, out bool isWarning)
        {
            SetCulture();

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaTitolare.Pensione.NDomus, null);
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(tipoAppartenenza);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            AreaEsito Esito = new AreaEsito();
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            bool isTabAnagraficaSaved = false;
            isWarning = false;
            Esito = StoreAnagrafica(datiPensione, datiAnagrafici, areaTitolare, true, isRiaperturaDomanda, dataSistema, out isTabAnagraficaSaved, out isWarning);

            return Esito;
        }

        #endregion AreaAnagrafica

        #region AreaPagamento
        public AreaEsito GetPagamentoByNumeroDomanda(AreaRichiestaDomanda areaRichiestaDomanda, int abiCassaSede, out AreaPagamento areaPagamento)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            AreaEsito Esito = new AreaEsito();
            areaPagamento = null;
            try
            {
                string errori = string.Empty;
                GestioneAreaPagamento.DatiPagamento pagamentoBL = null;
                if (!GestioneAreaPagamento.GetPagamentoByDatiPensione(datiPensione, out pagamentoBL, out errori))
                    throw new INPS.DNA.DnaApplicationException(errori);

                if (pagamentoBL != null)
                    areaPagamento = new AreaPagamento(pagamentoBL);

                if (areaPagamento == null)
                    areaPagamento = new AreaPagamento();

                List<GestioneAreaPagamento.DatiCassaSede> LCassaSede = null;
                GestioneAreaPagamento.GetListCassaSede(datiPensione, abiCassaSede, out LCassaSede, out errori);
                if (!string.IsNullOrEmpty(errori))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = errori;
                    return Esito;
                }
                else
                    areaPagamento.ListCassaSede = LCassaSede;

                List<GestioneAreaPagamento.DatiStatoEstero> LStatoEstero = null;
                GestioneAreaPagamento.GetListStatiEsteri(out LStatoEstero, out errori);
                if (!string.IsNullOrEmpty(errori))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = errori;
                    return Esito;
                }
                else
                    areaPagamento.ListStatiEsteri = LStatoEstero;

                GetCrossProperties(datiPensione, areaPagamento.Pagamento, ref areaPagamento);

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            return Esito;
        }

        public AreaEsito StorePagamento(Int64 numeroDomanda, ref AreaPagamento areaPagamento, string matricola, string sede)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            AreaEsito Esito = new AreaEsito();
            try
            {
                string errori = "";
                if (!GestioneAreaPagamento.StorePagamentoByDatiPensione(ref datiPensione, areaPagamento.Pagamento, matricola, sede, out errori))
                    throw new INPS.DNA.DnaValidationException(errori);

                AggiornaDatiSedePensionePerENPALS(datiPensione, ref areaPagamento);

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = "";
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            return Esito;
        }

        public AreaEsito CancelPagamentoByNumeroDomanda(Int64 numeroDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            AreaEsito Esito = new AreaEsito();
            try
            {
                string errori = string.Empty;

                if (!GestioneAreaPagamento.CancelPagamentoByDatiPensione(datiPensione, out errori))
                    throw new INPS.DNA.DnaApplicationException(errori);

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            return Esito;
        }

        public AreaEsito GetUfficiPagatori(RichiestaUfficiPagatori richiesta, out List<UfficioPagatore> elencoUfficiPagatori)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            elencoUfficiPagatori = null;
            try
            {
                string errori = "";
                if (richiesta == null)
                    throw new INPS.DNA.DnaValidationException("Area di richiesta uffici pagatori non valorizzata correttamente");

                List<GestioneUfficiPagatori.AreaUfficioPagatore> elencoUfficiPagatoriBL = null;
                switch (richiesta.Tipo)
                {
                    case RichiestaUfficiPagatori.TipoRicerca.Abi_Cab:
                    case RichiestaUfficiPagatori.TipoRicerca.Cassa:
                        if (richiesta.Abi == 0 || richiesta.Cab == 0)
                        {
                            string msg = string.Empty;
                            if (richiesta.Tipo == RichiestaUfficiPagatori.TipoRicerca.Abi_Cab)
                                msg = "Abi e cab non valorizzati correttamente";
                            else
                                msg = "Abi e/o cassa non valorizzati correttamente";

                            throw new INPS.DNA.DnaValidationException(msg);
                        }
                        //Banca d'Italia
                        if (richiesta.Abi == 1000 && richiesta.Cab == 03203)
                        {
                            richiesta.Cab = 6603203;
                        }
                        if (!GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(richiesta.ModalitaPagamento, richiesta.Iban, richiesta.Bic, richiesta.Abi.ToString(),
                            richiesta.Cab.ToString(), richiesta.Libretto, out errori))
                            throw new INPS.DNA.DnaValidationException(errori);

                        if (!GestioneUfficiPagatori.GetUfficiPagatoriNew(richiesta.Abi, richiesta.Cab, out elencoUfficiPagatoriBL, out errori))
                            throw new INPS.DNA.DnaValidationException(errori);
                        break;
                    case RichiestaUfficiPagatori.TipoRicerca.Abi_Frazionario:
                        if (richiesta.Abi == 0 || richiesta.Frazionario == 0)
                            throw new INPS.DNA.DnaValidationException("Abi e frazionario non valorizzati correttamente");
                        if (!GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(richiesta.ModalitaPagamento, richiesta.Iban, richiesta.Bic, richiesta.Abi.ToString(),
                            richiesta.Frazionario.ToString(), richiesta.Libretto, out errori))
                            throw new INPS.DNA.DnaValidationException(errori);

                        if (!GestioneUfficiPagatori.GetUfficiPagatoriNew(richiesta.Abi, richiesta.Frazionario, out elencoUfficiPagatoriBL, out errori))
                            throw new INPS.DNA.DnaValidationException(errori);
                        break;
                    case RichiestaUfficiPagatori.TipoRicerca.Iban_Banca:
                        if (String.IsNullOrEmpty(richiesta.Iban) || richiesta.Iban.Length < 27)
                            throw new INPS.DNA.DnaValidationException("Iban non valorizzato correttamente");
                        if (!GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(richiesta.ModalitaPagamento, richiesta.Iban, richiesta.Bic, richiesta.Iban.Substring(5, 5),
                            richiesta.Iban.Substring(10, 5), richiesta.Libretto, out errori))
                            throw new INPS.DNA.DnaValidationException(errori);
                        richiesta.Abi = int.Parse(richiesta.Iban.Substring(5, 5));
                        richiesta.Cab = int.Parse(richiesta.Iban.Substring(10, 5));

                        if (!GestioneUfficiPagatori.GetUfficiPagatoriNew(richiesta.Abi, richiesta.Cab, out elencoUfficiPagatoriBL, out errori))
                            throw new INPS.DNA.DnaValidationException(errori);
                        break;
                    case RichiestaUfficiPagatori.TipoRicerca.Iban_Posta:
                        if (String.IsNullOrEmpty(richiesta.Iban) || richiesta.Iban.Length < 27 || !string.IsNullOrEmpty(richiesta.Libretto))
                            throw new INPS.DNA.DnaValidationException("Iban non valorizzato correttamente");
                        else if (richiesta.Iban.ToUpperInvariant().Substring(5, 5) != "07601" &&
                            !(richiesta.Iban.ToUpperInvariant().Substring(5, 5) == "36081" && richiesta.Iban.ToUpperInvariant().Substring(10, 5) == "05138"))
                            throw new INPS.DNA.DnaValidationException("L'Iban inserito non è di tipo postale ma bancario");
                        else if (richiesta.Iban.ToUpperInvariant().Substring(5, 5) == "07601" && richiesta.Frazionario == 0)
                            throw new INPS.DNA.DnaValidationException("Frazionario non valorizzato correttamente");
                        else if (richiesta.Iban.ToUpperInvariant().Substring(5, 5) == "36081" && richiesta.Iban.ToUpperInvariant().Substring(10, 5) == "05138" && richiesta.Frazionario != 0)
                            throw new INPS.DNA.DnaValidationException("Il frazionario non deve essere inserito");
                        else if (richiesta.ModalitaPagamento != "L" && richiesta.Iban.ToUpperInvariant().Substring(10, 5) == "03384")
                            throw new INPS.DNA.DnaValidationException("L'Iban inserito non è valido. Trattasi di libretto");
                        else if (richiesta.ModalitaPagamento != "K" && richiesta.Iban.ToUpperInvariant().Substring(10, 5) == "05138")
                            throw new INPS.DNA.DnaValidationException("L'Iban inserito non è valido. Trattasi di Postepay Evolution.");
                        else if ((richiesta.ModalitaPagamento == "L" && richiesta.Iban.ToUpperInvariant().Substring(10, 5) != "03384") ||
                            (richiesta.ModalitaPagamento == "K" && richiesta.Iban.ToUpperInvariant().Substring(10, 5) != "05138"))
                            throw new INPS.DNA.DnaValidationException("L'Iban inserito non è valido. Trattasi di conto corrente");

                        int? cab_frazionario = null;
                        if (richiesta.Iban.ToUpperInvariant().Substring(5, 5) == "07601")
                            cab_frazionario = richiesta.Frazionario;
                        else
                        {
                            int app = 0;
                            if (int.TryParse(richiesta.Iban.ToUpperInvariant().Substring(10, 5), out app))
                                cab_frazionario = app;
                        }
                        if (!GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(richiesta.ModalitaPagamento, richiesta.Iban, richiesta.Bic, richiesta.Iban.Substring(5, 5),
                            cab_frazionario.ToString(), richiesta.Libretto, out errori))
                            throw new INPS.DNA.DnaValidationException(errori);
                        richiesta.Abi = int.Parse(richiesta.Iban.Substring(5, 5));

                        if (!GestioneUfficiPagatori.GetUfficiPagatoriNew(richiesta.Abi, cab_frazionario.GetValueOrDefault(), out elencoUfficiPagatoriBL, out errori))
                            throw new INPS.DNA.DnaValidationException(errori);
                        break;
                    case RichiestaUfficiPagatori.TipoRicerca.Estero:
                        if (String.IsNullOrEmpty(richiesta.StatoEstero))
                            throw new INPS.DNA.DnaValidationException("Stato Estero non valorizzato correttamente");
                        if (richiesta.Iban != null)
                        {
                            if (!GestioneUfficiPagatori.ValidaUfficioPagatorePerEstero(richiesta.ModalitaPagamento, richiesta.Iban, richiesta.StatoEstero, richiesta.Bic, richiesta.CodCatastaleEstero, out errori))
                                throw new INPS.DNA.DnaValidationException(errori);
                        }
                        if (!GestioneUfficiPagatori.GetUfficiPagatoriNew(richiesta.Abi, richiesta.Cab, out elencoUfficiPagatoriBL, out errori))
                            throw new INPS.DNA.DnaValidationException(errori);
                        break;
                    default:
                        break;
                }

                if (elencoUfficiPagatoriBL != null && elencoUfficiPagatoriBL.Count > 0)
                {
                    elencoUfficiPagatori = new List<UfficioPagatore>();
                    foreach (GestioneUfficiPagatori.AreaUfficioPagatore uffPag in elencoUfficiPagatoriBL)
                        elencoUfficiPagatori.Add(new UfficioPagatore(uffPag.Nome,
                            uffPag.Agenzia, uffPag.Cap, uffPag.Citta, uffPag.Indirizzo, uffPag.CodiceMeccanizzazione, uffPag.Abi, uffPag.Cab, uffPag.Frazionario));
                }

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = "";
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                string messaggio = Utility.GetMessageFromException(Ex);
                Esito.Messaggio = "Errore tecnico in fase di validazione dell'ufficio pagatore";
                string parametri = null;
                try
                {
                    parametri = Utility.GetXmlFromObject(richiesta);
                }
                catch (Exception)
                {
                    // Eccezione ignorata
                }
                GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, Ex.StackTrace);
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            return Esito;
        }

        private void GetCrossProperties(GestionePensione.DatiPensione datiPensione, GestioneAreaPagamento.DatiPagamento datiPagamento, ref AreaPagamento areaPagamento)
        {
            Dictionary<string, bool?> lCrossProperties = GestioneAreaPagamento.GetCrossProperties(datiPensione, datiPagamento);

            if (areaPagamento == null)
                areaPagamento = new AreaPagamento();

            areaPagamento.IsBancaItaliaFromWebDom = lCrossProperties["IsBancaItaliaFromWebDom"].GetValueOrDefault();
            areaPagamento.IsPolarizzazionePerGestioneENPALSAttiva = lCrossProperties["IsPolarizzazionePerGestioneENPALSAttiva"].GetValueOrDefault();
        }

        private void AggiornaDatiSedePensionePerENPALS(GestionePensione.DatiPensione datiPensione, ref AreaPagamento areaPagamento)
        {
            if (datiPensione != null && areaPagamento != null && Utility.IsDomandaENPALS(datiPensione.Gestione))
            {
                areaPagamento.CodiceSedeDestinazione = datiPensione.CodiceSedeDestinazione;
                areaPagamento.CentroOperativoDestinazione = datiPensione.CentroOperativoDestinazione;
            }
        }
        #endregion AreaPagamento

        #region AreaFamiliari
        public AreaEsito GetFamiliareByNumeroDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out List<GestioneAreaFamiliari.AreaFamiliare> elencoFamiliari, out List<Entity.Anagrafica> elencoAnagrafiche,
            out GestioneAreaFamiliari.AreaDecFam areaDecodifica)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            AreaEsito esito = new AreaEsito();
            elencoFamiliari = null;
            elencoAnagrafiche = null;
            areaDecodifica = null;

            GestioneAreaFamiliari.GetFamiliariByDatiPensione(ref contenitore, out elencoFamiliari, out elencoAnagrafiche);
            GestioneAreaFamiliari.GetAreaDecodificaByDatiPensione(ref contenitore, ref contenitoreDecodifica, out areaDecodifica);
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = "";
            return esito;
        }

        public AreaEsito SalvaFamiliari(long numeroDomanda, string cfFamiliareAttuale, string matricolaOperatore, ref List<GestioneAreaFamiliari.AreaFamiliare> elencoFamiliari, List<string> elencoFamiliariDaRimuovere, ref List<Entity.Anagrafica> elencoAnagrafiche, out GestioneFamiliari.ConsultazioneUnificataANF consultazioneANF)
        {
            SetCulture();
            consultazioneANF = null;
            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);
            bool isRiaperturaDomanda = contenitore.IsRiaperturaDomanda;
            Utility.TipoAppartenenza? tipoAppartenenza = contenitore.TipoAppartenenza;
            string messaggioInfo = string.Empty;
            AreaEsito esito = new AreaEsito();
            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ConsultazioneANFAttiva" + tipoAppartenenza.GetValueOrDefault().ToString(), out ctrl);
            try
            {
                GestioneAreaFamiliari.StoreFamiliari(contenitore.DatiPensione, isRiaperturaDomanda, cfFamiliareAttuale, elencoFamiliari, elencoFamiliariDaRimuovere, elencoAnagrafiche, out messaggioInfo);

                if (ctrl != null && ctrl.ValoreControllo == "SI")
                {
                    if (string.IsNullOrEmpty(messaggioInfo) && !isRiaperturaDomanda)
                        GestioneAreaFamiliari.RicercaDomandeANF(ref contenitore, isRiaperturaDomanda, matricolaOperatore, out messaggioInfo);
                }
                esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                esito.Messaggio = messaggioInfo;
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                elencoFamiliari = null;
                elencoAnagrafiche = null;
                GestioneAreaFamiliari.GetFamiliariByDatiPensione(ref contenitore, out elencoFamiliari, out elencoAnagrafiche);
                if (ctrl != null && ctrl.ValoreControllo == "SI")
                {
                    if (string.IsNullOrEmpty(esito.Messaggio))
                    {
                        GestioneAreaFamiliari.RispostaRicercaDomandeANFSingola(ref contenitore, elencoFamiliari, cfFamiliareAttuale, matricolaOperatore, out consultazioneANF, out messaggioInfo);
                        if (!string.IsNullOrEmpty(messaggioInfo))
                            esito.Messaggio = messaggioInfo;
                    }
                }
            }
            return esito;
        }

        public AreaEsito CancelFamiliari(long numeroDomanda, out List<GestioneAreaFamiliari.AreaFamiliare> elencoFamiliari, out List<Entity.Anagrafica> elencoAnagrafiche)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);
            AreaEsito esito = new AreaEsito();
            elencoFamiliari = null;
            elencoAnagrafiche = null;
            try
            {
                GestioneAreaFamiliari.DeleteFamiliariByDatiPensione(contenitore.DatiPensione);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                esito.Messaggio = "";
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }

            GestioneAreaFamiliari.GetFamiliariByDatiPensione(ref contenitore, out elencoFamiliari, out elencoAnagrafiche);

            return esito;
        }
        #endregion AreaFamiliari

        #region AreaStatoPratica
        public AreaRispostaStatoPratica GetStatoPraticaByKey(AreaRichiestaStatoPratica areaRichiestaStatoPratica)
        {
            SetCulture();

            AreaRispostaStatoPratica risposta = new AreaRispostaStatoPratica();
            try
            {
                String errori = "";
                List<Entity.DomandaDettagliata> elencoDomandeDettagliate = null;

                List<GestioneDecodifica.DecSede> elencoDecSede = null;
                GestioneDecodifica.GetElencoDecSede(out elencoDecSede);

                if (areaRichiestaStatoPratica.DatiParziali == null)
                    areaRichiestaStatoPratica.DatiParziali = new Entity.DatiPersonaliParziali();

                if (ConfigurationManager.AppSettings["BypassControlloMatricola"] == null ||
                    ConfigurationManager.AppSettings["BypassControlloMatricola"] != "SI")
                {
                    if (areaRichiestaStatoPratica.Ruolo == Utility.Ruolo.UTENTE && string.IsNullOrEmpty(areaRichiestaStatoPratica.Matricola))
                        areaRichiestaStatoPratica.Matricola = areaRichiestaStatoPratica.MatricolaOperatore;
                }

                if (areaRichiestaStatoPratica.Ruolo != Utility.Ruolo.AMMINISTRATORE)
                    areaRichiestaStatoPratica.Sede = areaRichiestaStatoPratica.SedeOperatore.ToString().PadLeft(4, '0') +
                        areaRichiestaStatoPratica.CentroOperativoOperatore.ToString().PadLeft(2, '0');

                GestioneAreaStatoPratica.GetStatoPratica(out elencoDomandeDettagliate, areaRichiestaStatoPratica.DatiParziali.Nome, areaRichiestaStatoPratica.DatiParziali.Cognome,
                    areaRichiestaStatoPratica.CodiceFiscale, areaRichiestaStatoPratica.Sede, areaRichiestaStatoPratica.Categoria, areaRichiestaStatoPratica.Tipo, areaRichiestaStatoPratica.Fondo,
                    areaRichiestaStatoPratica.StatoPensione, areaRichiestaStatoPratica.NumeroDomanda, areaRichiestaStatoPratica.Certificato, areaRichiestaStatoPratica.DataPresentazioneDomandaMin,
                    areaRichiestaStatoPratica.DataPresentazioneDomandaMax, areaRichiestaStatoPratica.DataElaborazioneDomandaMin, areaRichiestaStatoPratica.DataElaborazioneDomandaMax,
                    areaRichiestaStatoPratica.Matricola, areaRichiestaStatoPratica.TipoAppRuolo, areaRichiestaStatoPratica.Ruolo, areaRichiestaStatoPratica.TipoDomandaInLavorazione,
                    areaRichiestaStatoPratica.TipoDomandaLavorata, areaRichiestaStatoPratica.Gruppo, areaRichiestaStatoPratica.Prodotto, areaRichiestaStatoPratica.Cassa, areaRichiestaStatoPratica.SedeDiAppartenenzaOperatore, out errori);

                risposta.ElencoDatiStatoPratica = new List<AreaRispostaStatoPratica.DatiStatoPratica>();
                if (elencoDomandeDettagliate != null && elencoDomandeDettagliate.Count > 0)
                {
                    foreach (Entity.DomandaDettagliata dd in elencoDomandeDettagliate)
                    {
                        if (areaRichiestaStatoPratica.Ruolo != Utility.Ruolo.AMMINISTRATORE)
                        {
                            //31-01-2022: verifico se la sede della domanda è chiusa e si trova nella stessa provincia della sede di appartenenza dell'operatore
                            GestioneDecodifica.DecSede decSedeChiusa = null;
                            bool isSedeChiusaStessaProvinciaOperatore = false;
                            if (elencoDecSede != null && elencoDecSede.Count > 0)
                            {
                                decSedeChiusa = elencoDecSede.FindAll(x => !String.IsNullOrEmpty(dd.Sede) && !String.IsNullOrEmpty(x.CodProvincia) && dd.Sede.PadLeft(4, '0').Substring(0, 2) == x.CodProvincia.PadLeft(3, '0').Substring(1, 2)
                                     && !String.IsNullOrEmpty(x.CodZona) && dd.Sede.PadLeft(4, '0').Substring(2, 2) == x.CodZona.PadLeft(3, '0').Substring(1, 2)
                                     && !String.IsNullOrEmpty(dd.CentroOperativo) && !String.IsNullOrEmpty(x.CodCentroOperativo) && dd.CentroOperativo.PadLeft(2, '0').Substring(0, 2) == x.CodCentroOperativo.PadLeft(3, '0').Substring(1, 2)
                                     && x.CodAttivitaSede.GetValueOrDefault() == '0').FirstOrDefault();
                                isSedeChiusaStessaProvinciaOperatore = (decSedeChiusa != null && !String.IsNullOrEmpty(decSedeChiusa.CodProvincia)) ? decSedeChiusa.CodProvincia.PadLeft(3, '0').Substring(1, 2) == areaRichiestaStatoPratica.SedeDiAppartenenzaOperatore.ToString().PadLeft(6, '0').Substring(0, 2) : false;
                            }

                            //10-05-2012: inserito controllo per restituire solo domande con sede + co pari alla sede + co selezionata dall'operatore
                            //15-07-2013: inserito controllo per restituire solo domande con tipoAppartenenza uguale a tipoAppartenenzaOperatore
                            //31-01-2022: inserito controllo per restituire anche le domande che hanno sede chiusa e stessa provincia della sede di appartenenza dell'operatore
                            if (((dd.Sede == areaRichiestaStatoPratica.SedeOperatore.ToString().PadLeft(4, '0') &&
                                dd.CentroOperativo == areaRichiestaStatoPratica.CentroOperativoOperatore.ToString().PadLeft(2, '0')) || isSedeChiusaStessaProvinciaOperatore)
                                && dd.TipoAppartenenza.GetValueOrDefault() == areaRichiestaStatoPratica.TipoAppRuolo)
                            {
                                AreaRispostaStatoPratica.DatiStatoPratica statoPratica = new AreaRispostaStatoPratica.DatiStatoPratica(dd);
                                risposta.ElencoDatiStatoPratica.Add(statoPratica);
                            }
                        }
                        else
                        {
                            AreaRispostaStatoPratica.DatiStatoPratica statoPratica = new AreaRispostaStatoPratica.DatiStatoPratica(dd);
                            risposta.ElencoDatiStatoPratica.Add(statoPratica);
                        }
                    }
                }


                risposta.Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                risposta.Esito.Messaggio = "";
            }
            catch (Exception Ex)
            {
                risposta.Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                risposta.Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            return risposta;
        }

        #endregion AreaStatoPratica

        #region AreaDelegatoTutore
        public AreaEsito GetDelegatoByNumeroDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaRispostaRiepilogo.DatiRiepilogoAnagrafica anagrafica)
        {
            SetCulture();

            long idPensione = GetIdPensioneByNumeroDomanda(areaRichiestaDomanda);
            AreaEsito Esito = new AreaEsito();
            anagrafica = null;

            try
            {
                Entity.Anagrafica anagraficaBL = null;
                GestioneAreaDelegatoTutore.GetDelegatoByIdPensione(idPensione, out anagraficaBL);
                if (anagraficaBL != null)
                    anagrafica = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica(anagraficaBL);

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = "";
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            return Esito;
        }

        public AreaEsito GetTutoreByNumeroDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaRispostaRiepilogo.DatiRiepilogoAnagrafica anagrafica)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            anagrafica = null;

            long idPensione = GetIdPensioneByNumeroDomanda(areaRichiestaDomanda);

            try
            {
                Entity.Anagrafica anagraficaBL = null;
                GestioneAreaDelegatoTutore.GetTutoreByIdPensione(idPensione, out anagraficaBL);
                if (anagraficaBL != null)
                    anagrafica = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica(anagraficaBL);

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = "";
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            return Esito;
        }

        public void GetAnagraficaByDatiPersonaliParziali(short sedeOperatore, string matricolaOperatore, Entity.DatiPersonaliParziali datiPersonaliParziali, string numDomanda,
            out AreaRispostaRiepilogo risposta)
        {
            SetCulture();

            risposta = new AreaRispostaRiepilogo();

            #region
            try
            {
                String errori = "";
                List<Entity.Anagrafica> elencoAnagrafiche = null;

                #region valorizzazione parametri ARCA
                Entity.ParametriARCA parametriArca = null;
                if (!ValorizzaParametriARCA(matricolaOperatore, out parametriArca))
                {
                    string messaggio = "Errore nella valorizzazione dei parametri ARCA";
                    string parametri = string.Format("Matricola Operatore: {0}", matricolaOperatore);
                    long numeroDomanda = 0;
                    long.TryParse(numDomanda, out numeroDomanda);
                    GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, null);
                    throw new INPS.DNA.DnaApplicationException("Errore nel recupero delle informazioni anagrafiche");
                }
                #endregion valorizzazione parametri ARCA

                if (datiPersonaliParziali == null)
                    throw new INPS.DNA.DnaValidationException("Chiave di ricerca non assegnata correttamente. Non è possibile recuperare i dati richiesti");
                if (String.IsNullOrEmpty(datiPersonaliParziali.CodiceFiscale))
                    datiPersonaliParziali.CodiceFiscale = parametriArca.CodiceFiscaleRichiedente;

                // ricerca per dati anagrafici parziali
                if (!GestioneAreaRiepilogo.GetDatiParziali(parametriArca, datiPersonaliParziali.Nome, datiPersonaliParziali.Cognome,
                    datiPersonaliParziali.DataNascita, null, numDomanda, out elencoAnagrafiche, out errori))
                    throw new INPS.DNA.DnaApplicationException(errori);

                if (!String.IsNullOrEmpty(errori))
                {
                    risposta.Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    risposta.Esito.Messaggio = errori;
                    return;
                }

                if (elencoAnagrafiche != null && elencoAnagrafiche.Count == 1)
                {
                    risposta.AnagraficaTitolare = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica(elencoAnagrafiche[0]);
                }
                else if (elencoAnagrafiche != null && elencoAnagrafiche.Count > 1)
                {
                    risposta.ElencoSinonimi = new List<AreaRispostaRiepilogo.DatiRiepilogoSinonimo>();
                    foreach (Entity.Anagrafica s in elencoAnagrafiche)
                    {
                        AreaRispostaRiepilogo.DatiRiepilogoSinonimo sin = new AreaRispostaRiepilogo.DatiRiepilogoSinonimo(s);
                        risposta.ElencoSinonimi.Add(sin);
                    }
                }

                risposta.Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                risposta.Esito.Messaggio = "";
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                risposta.Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                risposta.Esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                risposta.Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                risposta.Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            #endregion
        }

        public AreaEsito StoreDelegato(long numeroDomanda, AreaRispostaRiepilogo.DatiRiepilogoAnagrafica datiRiepilogoAnagrafica)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            AreaEsito Esito = new AreaEsito();
            string errore = string.Empty;
            bool isRiapertura = Utility.IsRiaperturaDomanda(datiPensione.Id);
            EntityBLCommon.AreaTitolare areaTitolare = null;
            GestioneAnagrafica.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolare);

            try
            {
                Entity.Anagrafica delegato = ValorizzaAnagrafica(datiRiepilogoAnagrafica);
                if (GestioneAreaDelegatoTutore.ControlsDelegatoTutoreByDatiPensione(datiPensione, delegato, null, areaTitolare.Anagrafica.CodiceFiscale, isRiapertura, out errore))
                {
                    if (delegato != null)
                        GestioneAreaDelegatoTutore.SalvaDelegatoByDatiPensione(datiPensione, delegato);
                    else
                        throw new INPS.DNA.DnaApplicationException("Oggetto dati anagrafici non valido");

                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = "";
                }
                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = errore;
                }
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            return Esito;
        }

        public AreaEsito StoreTutore(long numeroDomanda, AreaRispostaRiepilogo.DatiRiepilogoAnagrafica datiRiepilogoAnagrafica)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            string errore = string.Empty;
            bool isRiapertura = Utility.IsRiaperturaDomanda(datiPensione.Id);
            EntityBLCommon.AreaTitolare areaTitolare = null;
            GestioneAnagrafica.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolare);

            try
            {
                Entity.Anagrafica tutore = ValorizzaAnagrafica(datiRiepilogoAnagrafica);
                if (GestioneAreaDelegatoTutore.ControlsDelegatoTutoreByDatiPensione(datiPensione, null, tutore, areaTitolare.Anagrafica.CodiceFiscale, isRiapertura, out errore))
                {
                    if (tutore != null)
                        GestioneAreaDelegatoTutore.SalvaTutoreByDatiPensione(datiPensione, tutore);
                    else
                        throw new INPS.DNA.DnaValidationException("Oggetto dati anagrafici non valido");

                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = "";
                }
                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = errore;
                }
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            return Esito;
        }

        public AreaEsito StoreDelegatoTutore(long numeroDomanda, AreaRispostaRiepilogo.DatiRiepilogoAnagrafica datiAnagraficaDelegato, AreaRispostaRiepilogo.DatiRiepilogoAnagrafica datiAnagraficaTutore)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            AreaEsito Esito = new AreaEsito();
            string errore = string.Empty;
            bool isRiapertura = Utility.IsRiaperturaDomanda(datiPensione.Id);
            EntityBLCommon.AreaTitolare areaTitolare = null;
            GestioneAnagrafica.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolare);

            try
            {
                Entity.Anagrafica anagraficaDelegato = ValorizzaAnagrafica(datiAnagraficaDelegato);
                Entity.Anagrafica anagraficaTutore = ValorizzaAnagrafica(datiAnagraficaTutore);

                if (GestioneAreaDelegatoTutore.ControlsDelegatoTutoreByDatiPensione(datiPensione, anagraficaDelegato, anagraficaTutore, areaTitolare.Anagrafica.CodiceFiscale, isRiapertura, out errore))
                {
                    if (anagraficaDelegato != null)
                        GestioneAreaDelegatoTutore.SalvaDelegatoByDatiPensione(datiPensione, anagraficaDelegato);

                    if (anagraficaTutore != null)
                        GestioneAreaDelegatoTutore.SalvaTutoreByDatiPensione(datiPensione, anagraficaTutore);

                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = string.Empty;
                }
                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = errore;
                }
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            return Esito;
        }

        public AreaEsito DeleteDelegato(long numeroDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            GestioneDelegatoTutore.EliminaDelegatoByDatiPensione(datiPensione);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito DeleteTutore(long numeroDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            GestioneDelegatoTutore.EliminaTutoreByDatiPensione(datiPensione);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito IsNotDelegatoOrTutorePresent(AreaRichiestaDomanda areaRichiestaDomanda, bool bDelegato)
        {
            SetCulture();

            long idPensione = GetIdPensioneByNumeroDomanda(areaRichiestaDomanda);

            AreaEsito Esito = new AreaEsito();
            try
            {
                if (bDelegato)  // delegato
                {
                    Entity.Anagrafica delegato = null;
                    GestioneAreaDelegatoTutore.GetDelegatoByIdPensione(idPensione, out delegato);
                    if (delegato.Id != 0)  // delegato presente nel db
                    {
                        Esito.Messaggio = "Attenzione! Delegato già presente, il salvataggio del nuovo Delegato sovrascriverà il precedente.";
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    }
                    else
                    {
                        Esito.Messaggio = string.Empty;
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    }
                }
                else // tutore
                {
                    Entity.Anagrafica tutore = null;
                    GestioneAreaDelegatoTutore.GetTutoreByIdPensione(idPensione, out tutore);
                    if (tutore.Id != 0)  // tutore presente nel db
                    {
                        Esito.Messaggio = "Attenzione! Tutore già presente, il salvataggio del nuovo Tutore sovrascriverà il precedente.";
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    }
                    else
                    {
                        Esito.Messaggio = string.Empty;
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    }
                }
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            return Esito;
        }

        #endregion AreaDelegatoTutore

        #region AreaPensione
        public AreaEsito EliminaPensioneByNumeroDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, Utility.TipoAppartenenza tipoAppRuolo, Utility.Ruolo ruolo, int sedeDiAppartenenzaOperatore)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);
            AreaEsito Esito = new AreaEsito();
            string errore = string.Empty;
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            bool updateAnagraficaAccordi = false;
            short? CodiceAziendaEditoriaPerTipo0179 = 0;
            if (Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensione) && datiPensione.DataElaborazione.HasValue)
            {
                GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
                GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);
                if (datiIstruttoria.CodiceAziendaEditoriaPerTipo0179.HasValue)
                {
                    updateAnagraficaAccordi = true;
                    CodiceAziendaEditoriaPerTipo0179 = datiIstruttoria.CodiceAziendaEditoriaPerTipo0179;
                }
            }

            if (tipoAppartenenza == Utility.TipoAppartenenza.AGO && !Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) &&
                datiPensione.FlagVerify.HasValue && !datiPensione.FlagVerify.Value)
            {
                if (!GestioneDatiPensioni.IsDomandaConPensioneLiquidata(datiPensione, isRiaperturaDomanda, out errore))
                {
                    if (!string.IsNullOrEmpty(errore))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = errore;
                        return Esito;
                    }
                }
                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = "La domanda risulta già liquidata. Effettuare il calcolo DEFINITIVO.";
                    return Esito;
                }
            }

            AreaEsito esitoEliminazioneBypassDinamici = DeleteAllBypassControlloDinamiciByNDomus(numeroDomanda);
            if (esitoEliminazioneBypassDinamici.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = esitoEliminazioneBypassDinamici.Messaggio;
                return Esito;
            }

            if (!GestioneAreaPensione.EliminaPensione(numeroDomanda, datiPensione, matricolaOperatore, sedeOperatore, centroOperativoOperatore, tipoAppRuolo, ruolo, sedeDiAppartenenzaOperatore, out errore))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = errore;
            }
            else
            {
                GestioneBypassControllo.SetAllUnlock(numeroDomanda);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = "";
                if (updateAnagraficaAccordi)
                {
                    GestioneAnagraficaAccordiPerTipo0179.UpdateCountLiquidate_AnagraficaAccordi(CodiceAziendaEditoriaPerTipo0179, false);
                }
            }
            return Esito;
        }
        #endregion AreaPensione

        #region AreaRedditi
        public AreaEsito GetRedditiByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, string matricolaOperatore, short sedeOperatore, out AreaRedditi areaRedditi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            AreaEsito Esito = new AreaEsito();
            areaRedditi = null;
            GestioneRedditi.AreaRedditi redditiBL = null;
            Utility.StatoPensione? stato = Utility.GetStatoPensioneByCodice(contenitore.DatiPensione.StatoPensione.GetValueOrDefault());
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
                    if (GestioneRedditi.GetRedditiDB(ref contenitore, out redditiBL))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        Esito.Messaggio = string.Empty;
                    }
                    else
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = redditiBL.MessaggioVideo;
                    }
                    break;

                default:
                    if (GestioneRedditi.GetRedditiByDatiPensione(ref contenitore, ref contenitoreDecodifica, matricolaOperatore, sedeOperatore, out redditiBL))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        Esito.Messaggio = string.Empty;
                    }
                    else
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = redditiBL.MessaggioVideo;
                    }
                    break;
            }
            areaRedditi = new AreaRedditi();
            areaRedditi.Redditi = redditiBL;

            return Esito;
        }

        public AreaEsito VerifyRedditiByDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, bool IsSalvataggio, AreaRedditi areaRedditiOriginali, out AreaRedditi areaRedditiLast)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiEliminazione datiEliminazione = null;
            GestioneAreaEliminazione.GetDatiEliminazioneByIdPensione(ref contenitore, out datiEliminazione);

            AreaEsito Esito = new AreaEsito();
            areaRedditiLast = null;
            GestioneRedditi.AreaRedditi redditiOriginaliBL = null;
            redditiOriginaliBL = areaRedditiOriginali.Redditi;
            GestioneRedditi.AreaRedditi redditiLastBL = null;
            if (GestioneRedditi.VerifyRedditiByDatiPensione(ref contenitore, matricolaOperatore, sedeOperatore, IsSalvataggio, redditiOriginaliBL, datiEliminazione, out redditiLastBL))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            else
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = redditiLastBL.MessaggioVideo;
            }
            areaRedditiLast = new AreaRedditi();
            areaRedditiLast.Redditi = redditiLastBL;

            //Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            //Esito.Messaggio = "";
            return Esito;
        }

        public AreaEsito EliminaRedditiByDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, out AreaRedditi areaRedditi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            AreaEsito Esito = new AreaEsito();
            areaRedditi = null;
            GestioneRedditi.AreaRedditi redditiBL = null;

            try
            {
                GestioneRedditi.EliminaRedditiByDatiPensione(ref contenitore);
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }

            if (GestioneRedditi.GetRedditiByDatiPensione(ref contenitore, ref contenitoreDecodifica, matricolaOperatore, sedeOperatore, out redditiBL))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            else
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = redditiBL.MessaggioVideo;
            }
            areaRedditi = new AreaRedditi();
            areaRedditi.Redditi = redditiBL;

            return Esito;
        }
        #endregion AreaRedditi

        #region AreaSupplementi

        // get di tutta l'area Supplementi
        public AreaEsito GetSupplementiByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaSupplementi areaSupplementi)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);
            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            AreaEsito Esito = new AreaEsito();
            areaSupplementi = new AreaSupplementi();

            List<BLCommon.Entity.DatiSupplementiENPALS> listDatiSupplementiEnpals = null;

            //ENG - RIC REVERSIBILITA 024: implementazione flusso per riconoscere le reversibilità "vecchie" 
            GestioneLavorazione.DatiLavorazione datiLavorazione = null;
            GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);

            if (Utility.IsDomandaENPALS(datiPensione.Gestione))
            {
                List<BLCommon.Entity.DatiSuppRecordENPALS> listDatiSuppRecordEnpals = null;
                GestioneAreaSupplementi.GetDatiSuppRecordEnpalsByIdPensione(datiPensione.Id, out listDatiSuppRecordEnpals);
                if (listDatiSuppRecordEnpals != null)
                    areaSupplementi.ListaDatiSuppRecordENPALS = listDatiSuppRecordEnpals;

                BLCommon.Entity.DatiContribuzioneEnpals datiContribuzioneEnpals = null;
                GestioneContribuzioneEnpals.GetDatiContribuzioneEnpalsByIdPensioneAndTipologia(datiPensione.Id, TipologiaContribuzioneEnpals.SAS, out datiContribuzioneEnpals);
                if (datiContribuzioneEnpals != null)
                    areaSupplementi.DatiContribuzioneEnpalsSAS = datiContribuzioneEnpals;

                BLCommon.GestioneSupplementi.GetDatiSupplementiEnpalsByIdPensione(datiPensione.Id, out listDatiSupplementiEnpals);
            }
            else if (Utility.IsDomandaCumulo(datiPensione.SiglaCategoria) || Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria))
            {
                List<BLCommon.Entity.DatiSupplementiCumulo> listDatiSuppCumulo = null;
                GestioneAreaSupplementi.GetDatiSupplementiCumuloByIdPensione(datiPensione.Id, out listDatiSuppCumulo);
                if (listDatiSuppCumulo != null)
                    areaSupplementi.ListaDatiSupplementiCumulo = listDatiSuppCumulo;

                //ENG - Memo 32_a/2018
                if (Utility.IsRicostituzioneMotiviContributiviVariazioneDatiSupplementiCumulo(datiPensione))
                {
                    List<BLCommon.Entity.DatiSupplementiCumulo> listDatiSuppCumuloStorico = null;
                    GestioneAreaSupplementi.GetDatiSupplementiCumuloStoricoByIdPensione(datiPensione.Id, out listDatiSuppCumuloStorico);
                    if (listDatiSuppCumuloStorico != null)
                        areaSupplementi.ListaDatiSupplementiCumuloStorico = listDatiSuppCumuloStorico;
                }
            }
            else
            {
                List<BLCommon.Entity.DatiSupplementi> listDatiSupplementi = null;
                //ENG - MEMO 50/2023
                if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO && Utility.IsRicostituzione_MotiviContributivi(datiPensione) && datiPensione.Tipo == "0001" &&
                    !(Utility.IsDomandaAnte96(datiPensione, datiPensione, datiDanteCausa, Utility.IsRiaperturaDomanda(datiPensione.Id)) != null))
                    GestioneAreaSupplementi.GetDatiSupplementiNoStoricoByIdPensione(datiPensione.Id, out listDatiSupplementi);
                else
                    GestioneAreaSupplementi.GetDatiSupplementiByIdPensione(datiPensione.Id, out listDatiSupplementi);

                if (listDatiSupplementi != null)
                    areaSupplementi.ListDatiSupplementi = listDatiSupplementi;

                List<BLCommon.Entity.TipoSupplementi> listaTipoSupplementi = null;
                GestioneAreaSupplementi.GetListaTipoSupplementiByDatiPensione(datiPensione, datiDanteCausa, out listaTipoSupplementi);
                if (listaTipoSupplementi != null)
                    areaSupplementi.ListTipoSupplementi = listaTipoSupplementi;

                BLCommon.Entity.SupplementiBase supplementoBase = null;
                GestioneAreaSupplementi.GetDatiSupplementiBaseByIdPensione(datiPensione.Id, out supplementoBase);
                if (supplementoBase != null)
                    areaSupplementi.SupplementiBase = supplementoBase;

                BLCommon.Entity.IntegrazioneArt11 integrazioneArt11 = null;
                GestioneAreaSupplementi.GetDatiIntegrazioneArt11ByIdPensione(datiPensione.Id, out integrazioneArt11);
                if (integrazioneArt11 != null)
                    areaSupplementi.IntegrazioneArt11 = integrazioneArt11;
            }

            GetCrossProperties(datiPensione, datiDanteCausa, listDatiSupplementiEnpals, datiLavorazione, ref areaSupplementi);
            GetListeDecodificaAreaSupplementi(datiPensione, ref areaSupplementi);

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        // salvataggio di tutta l'area Supplementi
        public AreaEsito SalvaSupplementiByDomanda(long numDomanda, AreaSupplementi areaSupplementi)
        {
            SetCulture();
            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numDomanda, null);
            AreaEsito Esito = new AreaEsito();
            if (Utility.IsDomandaCumulo(datiPensione.SiglaCategoria) || Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria))
                Esito = StoreDatiSupplementiCumulo(numDomanda, areaSupplementi);
            else
                Esito = StoreDatiSupplementi(numDomanda, areaSupplementi);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            return Esito;
        }

        #region DatiSupplementi

        public AreaEsito StoreDatiSupplementi(long numeroDomanda, AreaSupplementi areaSupplementi)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            BLCommon.GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            BLCommon.GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;
            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            try
            {

                GestioneAreaSupplementi.ControlDatiSupplementiBaseByDatiPensione(datiPensione, areaSupplementi.SupplementiBase, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    if (Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) &&
                        !Utility.IsRicostituzione_MotiviDocumentali(datiPensione) && !Utility.IsRicostituzione_Supplemento(datiPensione) &&
                        (Utility.IsDomandaFPLD(datiPensione.SiglaCategoria) || Utility.IsDomandaGestioneAutonomi(datiPensione.SiglaCategoria) ||
                        Utility.IsDomandaINPDAI(datiPensione.SiglaCategoria) || Utility.IsDomandaAUT(datiPensione)))
                        messaggioControllo = messaggioControllo + " Controllare le informazioni memorizzate ed eventualmente variarle con una Ricostituzione contributiva/documentale.";

                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }

                GestioneAreaSupplementi.ControlDatiIntegrazioneArt11ByDatiPensione(datiPensione, areaSupplementi.IntegrazioneArt11, areaSupplementi.ListDatiSupplementi, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }

                GestioneAreaSupplementi.ControlDatiSupplementiByDatiPensione(datiPensione, datiDanteCausa, datiAnagrafici, areaSupplementi.ListDatiSupplementi, isRiaperturaDomanda, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    GestioneQuadri.DatiQuadroSupplementi datiQuadroSupplementi = null;
                    GestioneQuadri.GetQuadroSupplementiByDatiPensione(datiPensione, out datiQuadroSupplementi);
                    if (datiQuadroSupplementi != null && datiQuadroSupplementi.TabSupplementi != 2)
                        GestioneBypassControllo.SetUnlock(datiPensione.NDomus, typeof(GestioneBypassControllo.NomeBypass.Supplementi_Supplementi_AGO));

                    if (Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) &&
                        !Utility.IsRicostituzione_MotiviDocumentali(datiPensione) && !Utility.IsRicostituzione_Supplemento(datiPensione) &&
                        (Utility.IsDomandaFPLD(datiPensione.SiglaCategoria) || Utility.IsDomandaGestioneAutonomi(datiPensione.SiglaCategoria) ||
                        Utility.IsDomandaINPDAI(datiPensione.SiglaCategoria) || Utility.IsDomandaAUT(datiPensione)))
                        messaggioControllo = messaggioControllo + " Controllare le informazioni memorizzate ed eventualmente variarle con una Ricostituzione contributiva/documentale.";
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }


                GestioneAreaSupplementi.StoreDatiSupplementiByDatiPensione(datiPensione, areaSupplementi.ListDatiSupplementi, areaSupplementi.SupplementiBase, areaSupplementi.IntegrazioneArt11);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            catch (Exception ex)
            {
                try
                {
                    GestioneQuadri.DatiQuadroSupplementi datiQuadroSupplementi = null;
                    GestioneQuadri.GetQuadroSupplementiByDatiPensione(datiPensione, out datiQuadroSupplementi);
                    if (datiQuadroSupplementi != null && datiQuadroSupplementi.TabSupplementi != 2)
                        GestioneBypassControllo.SetUnlock(datiPensione.NDomus, typeof(GestioneBypassControllo.NomeBypass.Supplementi_Supplementi_AGO));
                }
                catch (Exception)
                {
                    // Eccezione ignorata
                }

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico durante il salvataggio dei supplementi";
            }

            return Esito;
        }

        public AreaEsito DeleteDatiSupplementiByDomanda(long numeroDomanda, out AreaSupplementi areaSupplementi)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            AreaEsito Esito = new AreaEsito();

            bool isRicContributivaTipoOrdinarioMemo50 = false;
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo50_2023", out ctrl);
            //ENG - MEMO 50/2023
            if (ctrl != null && ctrl.ValoreControllo == "SI" && tipoAppartenenza.HasValue && tipoAppartenenza.Value != Utility.TipoAppartenenza.CI &&
                Utility.IsRicostituzione_MotiviContributivi(datiPensione) && datiPensione.Tipo == "0001" && !Utility.IsDomandaCumulo(datiPensione.SiglaCategoria) &&
                !Utility.IsDomandaENPALS(datiPensione.Gestione) && !Utility.IsDomandaINPDAP(datiPensione.Gestione))
                isRicContributivaTipoOrdinarioMemo50 = true;

            if (!isRicContributivaTipoOrdinarioMemo50)
            {
                if (Utility.IsDomandaENPALS(datiPensione.Gestione))
                    GestioneAreaSupplementi.EliminaDatiSuppRecordEnpalsByIdPensione(datiPensione);
                else
                    GestioneAreaSupplementi.EliminaSupplementiByIdPensione(datiPensione);
            }
            GetSupplementiByDomanda(new AreaRichiestaDomanda { NumeroDomanda = numeroDomanda, ProgStorico = null }, out areaSupplementi);

            if (isRicContributivaTipoOrdinarioMemo50)
            {
                Esito = DeleteDatiSupplementi(numeroDomanda);
            }
            else
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = "";
            }
            return Esito;
        }
        //ENG - MEMO 50/2023
        public AreaEsito DeleteDatiSupplementi(long numeroDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;


            GestioneAreaSupplementi.ControlSupplementiPerRicContributivaPura(datiPensione, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            GestioneAreaSupplementi.EliminaSupplementiByIdPensione(datiPensione);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        #region Supplementi Enpals

        public AreaEsito StoreRecordSupplementoEnpals(long numeroDomanda, ref AreaSupplementi areaSupplementi)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();

            #region Get
            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            List<BLCommon.Entity.DatiSuppRecordENPALS> lstSuppRecordEnpalsDb = null;
            GestioneSupplementi.GetDatiSuppRecordEnpalsByIdPensione(datiPensione.Id, out lstSuppRecordEnpalsDb);
            #endregion Get

            string messaggioVideo;
            if (!GestioneAreaSupplementi.ControlsStoreDatiSuppRecordEnpals(datiPensione, areaSupplementi.DatiSuppRecordENPALS, lstSuppRecordEnpalsDb, out messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }
            long? idRecord;
            GestioneAreaSupplementi.StoreDatiSuppRecordEnpals(datiPensione, areaSupplementi.DatiSuppRecordENPALS, lstSuppRecordEnpalsDb, out idRecord);
            areaSupplementi.DatiSuppRecordENPALS.IdSuppRecordEnpals = idRecord.GetValueOrDefault();

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito DeleteRecordSupplementoEnpals(long numeroDomanda, long idRecord)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            AreaEsito Esito = new AreaEsito();

            GestioneAreaSupplementi.EliminaDatiSuppRecordEnpalsByIdRecord(datiPensione, idRecord);

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito GetDatiSupplementoDettaglioEnpals(AreaRichiestaDomanda areaRichiestaDomanda, long idRecord, out AreaSupplementi areaSupplementi)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            areaSupplementi = new AreaSupplementi();

            BLCommon.Entity.DatiSuppRecordENPALS dSuppRecordENPALS = null;
            List<BLCommon.Entity.DatiSupplementiENPALS> lstSupplementiEnpals = null;
            BLCommon.Entity.IntegrazioneArt11 dIntegrazioneArt11 = null;

            GestioneAreaSupplementi.GetDatiSupplementiDettaglioEnpalsByIdRecord(idRecord, out dSuppRecordENPALS, out lstSupplementiEnpals, out dIntegrazioneArt11);
            areaSupplementi.DatiSuppRecordENPALS = dSuppRecordENPALS;
            areaSupplementi.IntegrazioneArt11 = dIntegrazioneArt11;
            areaSupplementi.ListDatiSupplementiENPALS = lstSupplementiEnpals;

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        public AreaEsito DeleteSupplementoDettaglioEnpals(long numeroDomanda, long idRecord, out AreaSupplementi areaSupplementi)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            AreaEsito Esito = new AreaEsito();

            areaSupplementi = null;
            GestioneAreaSupplementi.EliminaDatiSupplementiDettagliEnpalsByIdRecord(datiPensione, idRecord);

            GetDatiSupplementoDettaglioEnpals(new AreaRichiestaDomanda { NumeroDomanda = numeroDomanda, ProgStorico = null }, idRecord, out areaSupplementi);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito StoreSupplementoDettaglioEnpals(long numeroDomanda, ref AreaSupplementi areaSupplementi)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            AreaEsito Esito = new AreaEsito();
            string messaggioVideo;

            if (!GestioneAreaSupplementi.ControlsStoreDatiSupplementiDettaglioEnpals(datiPensione, areaSupplementi.DatiSuppRecordENPALS, areaSupplementi.ListDatiSupplementiENPALS,
                areaSupplementi.IntegrazioneArt11, out messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }
            var record = areaSupplementi.DatiSuppRecordENPALS;
            GestioneAreaSupplementi.StoreDatiSupplementiDettaglioEnpals(datiPensione, ref record, areaSupplementi.ListDatiSupplementiENPALS, areaSupplementi.IntegrazioneArt11);

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        #endregion Supplementi Enpals

        #region Supplementi Cumulo
        public AreaEsito DeleteDatiSupplementiCumuloByDomanda(long numeroDomanda, out AreaSupplementi areaSupplementi)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            AreaEsito Esito = new AreaEsito();

            GestioneAreaSupplementi.EliminaSupplementiCumuloByIdPensione(datiPensione);
            GetSupplementiByDomanda(new AreaRichiestaDomanda { NumeroDomanda = numeroDomanda, ProgStorico = null }, out areaSupplementi);

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        public AreaEsito StoreDatiSupplementiCumulo(long numeroDomanda, AreaSupplementi areaSupplementi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;
            try
            {
                GestioneAreaSupplementi.ControlsDatiSupplementiCumulo(contenitore.DatiPensione, areaSupplementi.ListaDatiSupplementiCumulo, contenitoreDecodifica.ElencoDecEnteGestioneFondo,
                    contenitore.ListaQuotePensione, contenitore.IsRiaperturaDomanda, contenitore.DatiDanteCausa, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }

                GestioneAreaSupplementi.StoreDatiSupplementiCumuloByDatiPensione(contenitore.DatiPensione, areaSupplementi.ListaDatiSupplementiCumulo);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            catch (Exception)
            {
                try
                {
                    GestioneQuadri.DatiQuadroSupplementi datiQuadroSupplementi = null;
                    GestioneQuadri.GetQuadroSupplementiByDatiPensione(contenitore.DatiPensione, out datiQuadroSupplementi);
                    if (datiQuadroSupplementi != null && datiQuadroSupplementi.TabSupplementi != 2)
                        GestioneBypassControllo.SetUnlock(contenitore.DatiPensione.NDomus, typeof(GestioneBypassControllo.NomeBypass.Supplementi_Supplementi_AGO));
                }
                catch (Exception)
                {
                    // Eccezione ignorata
                }

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico durante il salvataggio dei supplementi";
            }

            return Esito;
        }
        #endregion Supplementi Cumulo

        private void GetCrossProperties(GestionePensione.DatiPensione datiPensione, BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, List<BLCommon.Entity.DatiSupplementiENPALS> listaDatiSupplementiENPALS, GestioneLavorazione.DatiLavorazione datiLavorazione, ref AreaSupplementi areaSupplementi)
        {
            DateTime? decorrenzaPensioneDC = null;
            Dictionary<string, bool?> lCrossProperties = GestioneAreaSupplementi.GetCrossProperties(datiPensione, datiDanteCausa, listaDatiSupplementiENPALS, datiLavorazione, out decorrenzaPensioneDC);

            if (areaSupplementi == null)
                areaSupplementi = new AreaSupplementi();

            areaSupplementi.IsDomandaSperimentaleDonna = lCrossProperties["IsDomandaSperimentaleDonna"].GetValueOrDefault();
            areaSupplementi.IsContribuzioneEnpalsRetributivaVisible = lCrossProperties["IsContribuzioneEnpalsRetributivaVisible"].GetValueOrDefault();
            areaSupplementi.IsContribuzioneEnpalsContributivaVisible = lCrossProperties["IsContribuzioneEnpalsContributivaVisible"].GetValueOrDefault();
            areaSupplementi.IsReversibilitaOrRicostituzione = lCrossProperties["IsReversibilitaOrRicostituzione"].GetValueOrDefault();
            areaSupplementi.DecorrenzaPensioneDanteCausa = decorrenzaPensioneDC;
            areaSupplementi.IsPannelloSupplementiAnte96 = lCrossProperties["IsPannelloSupplementiAnte96"].GetValueOrDefault();
            areaSupplementi.IsTipoCalcoloModificato = lCrossProperties["IsTipoCalcoloModificato"].GetValueOrDefault();
        }

        private void GetListeDecodificaAreaSupplementi(GestionePensione.DatiPensione datiPensione, ref AreaSupplementi areaSupplementi)
        {
            if (Utility.IsDomandaINPDAI(datiPensione.SiglaCategoria))
            {
                List<GestioneDecodifica.DecodificaTipoQuota> lstDecTipoQuotaBl;
                GestioneDecodifica.GetDecodificaCodiceTipoQuota(out lstDecTipoQuotaBl);
                if (lstDecTipoQuotaBl != null && lstDecTipoQuotaBl.Count > 0)
                {
                    areaSupplementi.ListaDecodificaTipoQuota = lstDecTipoQuotaBl.Select(x => { var r = new TipoQuota { Codice = x.Codice, Decodifica = x.Decodifica }; return r; }).ToList();
                }
            }
            else if (Utility.IsDomandaCumulo(datiPensione.SiglaCategoria) || Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria))
            {
                List<GestioneDecodifica.DecEnteGestioneFondo> listaDecEnteGestioneFondo = null;
                GestioneDecodifica.GetDecEnteGestioneFondo(out listaDecEnteGestioneFondo);
                if (listaDecEnteGestioneFondo != null && listaDecEnteGestioneFondo.Count > 0)
                {
                    GetDecEnteGestioneFondoSupplementiCustom(datiPensione, ref listaDecEnteGestioneFondo);

                    areaSupplementi.ListaDecEnteGestioneFondo = new List<DecEnteGestioneFondo>();
                    foreach (GestioneDecodifica.DecEnteGestioneFondo decEnteBL in listaDecEnteGestioneFondo)
                    {
                        Entity.DecEnteGestioneFondo decEnte = new DecEnteGestioneFondo();
                        Utility.ValorizzaOggetti(decEnteBL, decEnte);
                        areaSupplementi.ListaDecEnteGestioneFondo.Add(decEnte);
                    }
                }
            }
        }

        private void GetDecEnteGestioneFondoSupplementiCustom(GestionePensione.DatiPensione datiPensione, ref List<GestioneDecodifica.DecEnteGestioneFondo> listaDecEnteGestioneFondo)
        {
            if (Utility.IsDomandaCumulo(datiPensione.SiglaCategoria) || Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria))
            {
                var listaDaRimuovere = new List<string> { "C0", "D0", "E0", "SP" };

                listaDecEnteGestioneFondo.RemoveAll(x => listaDaRimuovere.Contains(x.Codice));
            }

            if (Utility.IsDomandaCumulo(datiPensione.SiglaCategoria))
            {
                var listaDaRimuovere = new List<string> { "B3", "B5" };

                listaDecEnteGestioneFondo.RemoveAll(x => listaDaRimuovere.Contains(x.Codice));
            }
        }
        #endregion DatiSupplementi

        #endregion AreaSupplementi

        #region AreaDanteCausa

        public AreaEsito GetDanteCausaByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaDanteCausa areaDanteCausa)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            AreaEsito Esito = new AreaEsito();
            areaDanteCausa = null;
            DanteCausaEntity entityDanteCausa = null;
            GestioneDanteCausa.GetDanteCausaEntityByDatiPensione(datiPensione, ref entityDanteCausa);

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            if (entityDanteCausa != null)
            {

                EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(areaRichiestaDomanda.NumeroDomanda, null);
                var datiDanteCausa = contenitore.DatiDanteCausa;

                GestioneDanteCausa.GetDatiAnagraficaDCByIdPensione(datiPensione.Id, ref entityDanteCausa);

                if (entityDanteCausa.AnagraficaDC != null)
                {
                    if (areaDanteCausa == null)
                        areaDanteCausa = new AreaDanteCausa();
                    areaDanteCausa.AnagraficaDC = entityDanteCausa.AnagraficaDC;
                }

                if (areaDanteCausa == null)
                    areaDanteCausa = new AreaDanteCausa();

                if (areaDanteCausa.AnagraficaDC == null)
                    areaDanteCausa.AnagraficaDC = new AnagraficaDC();

                AnagraficaDC anagraficaDC = areaDanteCausa.AnagraficaDC;
                GestioneDanteCausa.GetCrossPropertiesAnagraficaDC(datiPensione.Id, tipoAppartenenza, datiPensione.SiglaCategoria, ref anagraficaDC);
                areaDanteCausa.AnagraficaDC = anagraficaDC;

                if (entityDanteCausa.DatiPensioneDiretta != null)
                {
                    GestioneDanteCausa.GetDatiPensioneDiretta(ref entityDanteCausa);
                    if (areaDanteCausa == null)
                        areaDanteCausa = new AreaDanteCausa();
                    areaDanteCausa.DatiPensioneDiretta = entityDanteCausa.DatiPensioneDiretta;
                    areaDanteCausa.ElencoMaggiorazione780 = new List<AreaDanteCausa.DatiMaggiorazione780>();

                    foreach (Entity.DanteCausaEntity.DatiMaggiorazione781 m780 in entityDanteCausa.ElencoMaggiorazione781)
                        areaDanteCausa.ElencoMaggiorazione780.Add(new AreaDanteCausa.DatiMaggiorazione780 { Id = m780.Id, Descrizione = m780.Descrizione });
                }

                if (entityDanteCausa.AltraPensioneDC != null)
                {
                    GestioneDanteCausa.GetDatiAltraPensioneByDatiPensione(datiPensione, ref entityDanteCausa);
                    if (areaDanteCausa == null)
                        areaDanteCausa = new AreaDanteCausa();
                    areaDanteCausa.AltraPensioneDC = entityDanteCausa.AltraPensioneDC;
                }

                if (entityDanteCausa.DatiPensioneCI != null)
                {
                    GestioneDanteCausa.GetDatiPensioneCIByDatiPensione(datiPensione, ref entityDanteCausa);
                    if (areaDanteCausa == null)
                        areaDanteCausa = new AreaDanteCausa();
                    areaDanteCausa.DatiPensioneCI = entityDanteCausa.DatiPensioneCI;
                }

                if (entityDanteCausa.DatiRedditiSentenza495_93 != null && entityDanteCausa.DatiRedditiSentenza495_93.LredditiSentenza495_93 != null && entityDanteCausa.DatiRedditiSentenza495_93.LredditiSentenza495_93.Count > 0)
                {
                    if (areaDanteCausa == null)
                        areaDanteCausa = new AreaDanteCausa();

                    if (areaDanteCausa.DatiRedditiSentenza495_93 == null)
                        areaDanteCausa.DatiRedditiSentenza495_93 = new INPS.Pensioni.Liquidazione.Entity.DatiRedditiSentenza495_93();

                    areaDanteCausa.DatiRedditiSentenza495_93.LredditiSentenza495_93 = entityDanteCausa.DatiRedditiSentenza495_93.LredditiSentenza495_93;

                }

                if (entityDanteCausa.DatiRedditiSentenza495_93 != null && tipoAppartenenza.HasValue &&
                    (tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO || tipoAppartenenza.Value == Utility.TipoAppartenenza.CI))
                {
                    if (areaDanteCausa == null)
                        areaDanteCausa = new AreaDanteCausa();

                    if (areaDanteCausa.DatiRedditiSentenza495_93 == null)
                        areaDanteCausa.DatiRedditiSentenza495_93 = new DatiRedditiSentenza495_93();

                    DatiRedditiSentenza495_93 datiRedditiSentenza495_93 = areaDanteCausa.DatiRedditiSentenza495_93;
                    GestioneDanteCausa.GetCrossPropertiesDatiRedditiSentenza495_93(entityDanteCausa.AnagraficaDC.DataMorte, entityDanteCausa.DatiPensioneDiretta != null ? entityDanteCausa.DatiPensioneDiretta.DecorrenzaPensione : null, ref datiRedditiSentenza495_93);
                }

                List<GestioneDecodifica.CodiciNatura> listaCodiciNatura_AGO_BL = null;
                GestioneDecodifica.GetCodiciNatura_AGO_CI(out listaCodiciNatura_AGO_BL);
                if (listaCodiciNatura_AGO_BL != null)
                {
                    List<CodiciNatura> listaCodiciNatura_AGO = new List<CodiciNatura>();
                    foreach (GestioneDecodifica.CodiciNatura codiciNaturaBL in listaCodiciNatura_AGO_BL)
                    {
                        CodiciNatura CodiciNatura_AGO = new CodiciNatura();
                        Utility.ValorizzaOggettiNew(codiciNaturaBL, CodiciNatura_AGO);
                        listaCodiciNatura_AGO.Add(CodiciNatura_AGO);
                    }
                    if (areaDanteCausa == null)
                        areaDanteCausa = new AreaDanteCausa();
                    areaDanteCausa.ElencoCodiciNatura = listaCodiciNatura_AGO;
                }

                List<CodiceEliminazione> listaCodiceEliminazioneBL = null;
                GestioneDanteCausa.GetListaCodiceEliminazione(out listaCodiceEliminazioneBL, tipoAppartenenza);
                if (listaCodiceEliminazioneBL != null)
                {
                    List<CodiceEliminazione> listaCodiceEliminazione = new List<CodiceEliminazione>();
                    foreach (CodiceEliminazione codiceEliminazioneBL in listaCodiceEliminazioneBL)
                    {
                        CodiceEliminazione codiceEliminazione = new CodiceEliminazione();
                        Utility.ValorizzaOggettiNew(codiceEliminazioneBL, codiceEliminazione);
                        listaCodiceEliminazione.Add(codiceEliminazione);
                    }

                    if (areaDanteCausa == null)
                        areaDanteCausa = new AreaDanteCausa();
                    areaDanteCausa.ElencoCodiceEliminazione = listaCodiceEliminazione;
                }

                //ENG - Gestione Pensione Estera e redditi Sentenza 495
                GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiAgoCi = null;
                GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiGenericiAgoCi);
                if (datiGenericiAgoCi != null)
                    areaDanteCausa.ImportoMensilePensioneEstera = datiGenericiAgoCi.ImportoMensilePensioneEstera;

                if (datiDanteCausa != null)
                    areaDanteCausa.IsFascicoloGenerato = datiDanteCausa.IsFascicoloGenerato;

                areaDanteCausa.IsAnte96 = Utility.IsDomandaAnte96(datiPensione, datiPensione, datiDanteCausa, Utility.IsRiaperturaDomanda(datiPensione.Id));

                if (areaDanteCausa == null)
                    areaDanteCausa = new AreaDanteCausa();

                areaDanteCausa.DataAssunzioneCarico = contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico : null;

                areaDanteCausa.Id = entityDanteCausa.IdDC;

                areaDanteCausa.IsPresenteBypassNessunDanteCausa = GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Dante_Causa_FS.NESSUN_DANTE_CAUSA) || GestioneBypassControllo.CheckBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Dante_Causa_FS.NESSUN_DANTE_CAUSA_DINAMICO);

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            else
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Attenzione impossibile reperire i dati anagrafici del Dante Causa.";
            }

            return Esito;
        }

        public AreaEsito StoreDanteCausa(long numeroDomanda, AreaDanteCausa areaAreaDanteCausa)
        {
            SetCulture();

            string errore = string.Empty;
            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = null;
            GestioneMaggiorazioniBenefici.GetMaggiorazioniBeneficiByIdPensione(datiPensione.Id, out datiMaggiorazioniBenefici);

            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);

            List<GestioneFamiliari.Familiare> listaFamiliari = null;
            List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficaFamiliari = null;
            GestioneFamiliari.GetFamiliariByIdPensione(datiPensione.Id, out listaFamiliari, out listaAnagraficaFamiliari);

            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiAgoCi = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiGenericiAgoCi);

            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausaDB = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausaDB);

            GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC = null;
            if (datiDanteCausaDB != null)
                GestioneAnagrafica.GetAnagraficaByIdAnagrafica(datiDanteCausaDB.IdAnagrafica, out datiAnagraficiDC);

            GestioneQuadri.DatiQuadroDanteCausa datiQuadroQuadroDanteCausa = null;
            GestioneQuadri.GetQuadroDanteCausaByDatiPensione(datiPensione, out datiQuadroQuadroDanteCausa);

            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere = null;
            GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPrestazioniEstere);

            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            AreaEsito Esito = new AreaEsito();
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            if (!StoreDatiAnagraficaDC(datiPensione, areaAreaDanteCausa, datiMaggiorazioniBenefici, datiAnagraficiTitolare, listaFamiliari, listaAnagraficaFamiliari, ref datiAnagraficiDC, ref datiDanteCausaDB,
                ref datiQuadroQuadroDanteCausa, isRiaperturaDomanda, false, out errore))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = errore;
                return Esito;
            }

            if (!StoreDatiAltraPensione(datiPensione, areaAreaDanteCausa, datiAnagraficiDC, ref datiDanteCausaDB, ref datiQuadroQuadroDanteCausa, false, out errore))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = errore;
                return Esito;
            }

            if (!StoreDatiPensioneCI(ref datiPensione, areaAreaDanteCausa, ref datiDanteCausaDB, ref datiMaggiorazioniBenefici, datiGenericiAgoCi, ref datiQuadroQuadroDanteCausa, false, out errore))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = errore;
                return Esito;
            }

            if (!StoreDatiPensioneDiretta(datiPensione, areaAreaDanteCausa, datiGenericiAgoCi, listaPrestazioniEstere, datiMaggiorazioniBenefici, datiIstruttoria, datiAnagraficiDC, listaFamiliari, ref datiDanteCausaDB, ref datiQuadroQuadroDanteCausa,
                false, out errore))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = errore;
                return Esito;
            }

            if (!StoreDatiRedditiSentenza49593(datiPensione, areaAreaDanteCausa, datiAnagraficiDC, ref datiDanteCausaDB, ref datiQuadroQuadroDanteCausa, false, isRiaperturaDomanda, listaFamiliari, datiGenericiAgoCi, out errore))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = errore;
                return Esito;
            }

            return Esito;
        }

        public AreaEsito CancelDanteCausa(long numeroDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            GestioneDanteCausa.EliminaDanteCausaByIdPensione(datiPensione.Id);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito StoreDatiAnagraficaDC(long numeroDomanda, AreaDanteCausa areaDanteCausa)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            string errore = string.Empty;

            GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = null;
            GestioneMaggiorazioniBenefici.GetMaggiorazioniBeneficiByIdPensione(datiPensione.Id, out datiMaggiorazioniBenefici);

            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);

            List<GestioneFamiliari.Familiare> listaFamiliari = null;
            List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficaFamiliari = null;
            GestioneFamiliari.GetFamiliariByIdPensione(datiPensione.Id, out listaFamiliari, out listaAnagraficaFamiliari);

            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausaDB = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausaDB);

            GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC = null;
            if (datiDanteCausaDB != null)
                GestioneAnagrafica.GetAnagraficaByIdAnagrafica(datiDanteCausaDB.IdAnagrafica, out datiAnagraficiDC);

            GestioneQuadri.DatiQuadroDanteCausa datiQuadroQuadroDanteCausa = null;
            GestioneQuadri.GetQuadroDanteCausaByDatiPensione(datiPensione, out datiQuadroQuadroDanteCausa);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            AreaEsito Esito = new AreaEsito();
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            if (!StoreDatiAnagraficaDC(datiPensione, areaDanteCausa, datiMaggiorazioniBenefici, datiAnagraficiTitolare, listaFamiliari, listaAnagraficaFamiliari, ref datiAnagraficiDC, ref datiDanteCausaDB, ref datiQuadroQuadroDanteCausa,
                isRiaperturaDomanda, true, out errore))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = errore;
            }
            return Esito;
        }

        public AreaEsito StoreDatiAltraPensione(long numeroDomanda, AreaDanteCausa areaDanteCausa)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            string errore = string.Empty;

            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausaDB = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausaDB);

            GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC = null;
            if (datiDanteCausaDB != null)
                GestioneAnagrafica.GetAnagraficaByIdAnagrafica(datiDanteCausaDB.IdAnagrafica, out datiAnagraficiDC);

            GestioneQuadri.DatiQuadroDanteCausa datiQuadroQuadroDanteCausa = null;
            GestioneQuadri.GetQuadroDanteCausaByDatiPensione(datiPensione, out datiQuadroQuadroDanteCausa);

            AreaEsito Esito = new AreaEsito();
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            if (!StoreDatiAltraPensione(datiPensione, areaDanteCausa, datiAnagraficiDC, ref datiDanteCausaDB, ref datiQuadroQuadroDanteCausa, true, out errore))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = errore;
            }
            return Esito;
        }

        public AreaEsito StoreDatiPensioneCI(long numeroDomanda, AreaDanteCausa areaDanteCausa)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            string errore = string.Empty;

            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausaDB = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausaDB);

            GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = null;
            GestioneMaggiorazioniBenefici.GetMaggiorazioniBeneficiByIdPensione(datiPensione.Id, out datiMaggiorazioniBenefici);

            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiAgoCi = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiGenericiAgoCi);

            GestioneQuadri.DatiQuadroDanteCausa datiQuadroQuadroDanteCausa = null;
            GestioneQuadri.GetQuadroDanteCausaByDatiPensione(datiPensione, out datiQuadroQuadroDanteCausa);

            AreaEsito Esito = new AreaEsito();
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            if (!StoreDatiPensioneCI(ref datiPensione, areaDanteCausa, ref datiDanteCausaDB, ref datiMaggiorazioniBenefici, datiGenericiAgoCi, ref datiQuadroQuadroDanteCausa, true, out errore))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = errore;
            }

            return Esito;
        }

        public AreaEsito StoreDatiPensioneDiretta(long numeroDomanda, AreaDanteCausa areaDanteCausa)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            string errore = string.Empty;

            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiAgoCi = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiGenericiAgoCi);

            List<GestioneFamiliari.Familiare> listaFamiliari = null;
            List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficaFamiliari = null;
            GestioneFamiliari.GetFamiliariByIdPensione(datiPensione.Id, out listaFamiliari, out listaAnagraficaFamiliari);

            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere = null;
            GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPrestazioniEstere);

            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

            GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = null;
            GestioneMaggiorazioniBenefici.GetMaggiorazioniBeneficiByIdPensione(datiPensione.Id, out datiMaggiorazioniBenefici);

            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausaDB = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausaDB);

            GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC = null;
            if (datiDanteCausaDB != null)
                GestioneAnagrafica.GetAnagraficaByIdAnagrafica(datiDanteCausaDB.IdAnagrafica, out datiAnagraficiDC);

            GestioneQuadri.DatiQuadroDanteCausa datiQuadroQuadroDanteCausa = null;
            GestioneQuadri.GetQuadroDanteCausaByDatiPensione(datiPensione, out datiQuadroQuadroDanteCausa);

            AreaEsito Esito = new AreaEsito();
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            if (!StoreDatiPensioneDiretta(datiPensione, areaDanteCausa, datiGenericiAgoCi, listaPrestazioniEstere, datiMaggiorazioniBenefici, datiIstruttoria, datiAnagraficiDC, listaFamiliari, ref datiDanteCausaDB, ref datiQuadroQuadroDanteCausa,
                true, out errore))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = errore;
            }

            return Esito;
        }

        public AreaEsito StoreDatiRedditiSentenza49593(long numeroDomanda, AreaDanteCausa areaDanteCausa)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            string errore = string.Empty;

            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausaDB = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausaDB);

            GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC = null;
            if (datiDanteCausaDB != null)
                GestioneAnagrafica.GetAnagraficaByIdAnagrafica(datiDanteCausaDB.IdAnagrafica, out datiAnagraficiDC);

            GestioneQuadri.DatiQuadroDanteCausa datiQuadroQuadroDanteCausa = null;
            GestioneQuadri.GetQuadroDanteCausaByDatiPensione(datiPensione, out datiQuadroQuadroDanteCausa);

            List<GestioneFamiliari.Familiare> listaFamiliari = null;
            List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficaFamiliari = null;
            GestioneFamiliari.GetFamiliariByIdPensione(datiPensione.Id, out listaFamiliari, out listaAnagraficaFamiliari);

            //ENG - Gestione Pensione Estera e redditi Sentenza 495
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiAgoCi = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiGenericiAgoCi);

            AreaEsito Esito = new AreaEsito();
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            if (!StoreDatiRedditiSentenza49593(datiPensione, areaDanteCausa, datiAnagraficiDC, ref datiDanteCausaDB, ref datiQuadroQuadroDanteCausa, true, false, listaFamiliari, datiGenericiAgoCi, out errore))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = errore;
            }

            return Esito;
        }

        public AreaEsito CancelDanteSentenza495_93(long numeroDomanda, out AreaDanteCausa areaDanteCausa)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            GestioneDanteCausa.GetDanteCausaByIdPensione(datiPensione.Id, out datiDanteCausa);

            //ENG - Gestione Pensione Estera e redditi Sentenza 495
            BLCommon.GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiAgoCi = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiGenericiAgoCi);

            if (datiGenericiAgoCi != null)
                datiGenericiAgoCi.ImportoMensilePensioneEstera = null;

            AreaEsito Esito = new AreaEsito();
            GestioneDanteCausa.EliminaDatiRedditiSentenza495_93(datiPensione);
            GestioneDatiGenericiAgoCi.SalvaDatiGenerici(datiPensione.Id, datiGenericiAgoCi);

            areaDanteCausa = null;

            Utility.TipoAppartenenza? tipo = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            if (tipo == Utility.TipoAppartenenza.AGO || tipo == Utility.TipoAppartenenza.CI)
            {
                areaDanteCausa = new AreaDanteCausa();
                areaDanteCausa.DatiRedditiSentenza495_93 = new INPS.Pensioni.Liquidazione.Entity.DatiRedditiSentenza495_93();

                DatiRedditiSentenza495_93 datiRedditiSentenza495_93 = areaDanteCausa.DatiRedditiSentenza495_93;
                GestioneDanteCausa.GetCrossPropertiesDatiRedditiSentenza495_93(datiDanteCausa.DataMorte, datiDanteCausa.DecorrenzaPensione, ref datiRedditiSentenza495_93);
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        #endregion AreaDanteCausa

        #region AreaMaggiorazioniBenefici
        /*
        public AreaEsito GetMaggiorazioniBeneficiByDomanda(long numeroDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            AreaEsito Esito = new AreaEsito();
            areaMaggiorazioniBenefici = null;

            BLCommon.Entity.DatiLegge140 datiLegge140 = null;
            GestioneMaggiorazioniBenefici.GetDatiLegge140(numeroDomanda, out datiLegge140);
            if (datiLegge140 != null)
            {
                areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.DatiLegge140 = datiLegge140;
            }

            BLCommon.Entity.DatiAltriBenefici datiAltriBenefici = null;
            GestioneMaggiorazioniBenefici.GetDatiAltriBenefici(numeroDomanda, out datiAltriBenefici);
            if (datiAltriBenefici != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.DatiAltriBenefici = datiAltriBenefici;
            }

            BLCommon.Entity.DatiAgevolazioni datiAgevolazioni = null;
            GestioneMaggiorazioniBenefici.GetDatiAgevolazioni(numeroDomanda, out datiAgevolazioni);
            if (datiAgevolazioni != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.DatiAgevolazioni = datiAgevolazioni;
            }

            BLCommon.Entity.DatiLegge336 datiLegge336 = null;
            GestioneMaggiorazioniBenefici.GetDatiLegge336(numeroDomanda, out datiLegge336);
            if (datiLegge336 != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.DatiLegge336 = datiLegge336;
            }

            BLCommon.Entity.DatiDL407 datiDL407 = null;
            GestioneMaggiorazioniBenefici.GetDatiDL407(numeroDomanda, out datiDL407);
            if (datiDL407 != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.DatiDL407 = datiDL407;
            }

            List<Entity.CodiceCieco> listaCodiceCieco = null;
            GestioneMaggiorazioniBenefici.GetListaCodiceCieco(out listaCodiceCieco);
            if (listaCodiceCieco != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaCodiceCieco = listaCodiceCieco;
            }

            List<Entity.TipoBenefici> listaTipoBenefici = null;
            GestioneMaggiorazioniBenefici.GetListaTipoBenefici(out listaTipoBenefici);
            if (listaTipoBenefici != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaTipoBenefici = listaTipoBenefici;
            }

            List<Entity.CodiceMobilita> listaCodiceMobilita = null;
            GestioneMaggiorazioniBenefici.GetListaCodiceMobilita(out listaCodiceMobilita);
            if (listaCodiceMobilita != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaCodiceMobilita = listaCodiceMobilita;
            }

            List<Entity.CodiceRequisitiLegge50392> listaCodiceRequisitiLegge50392 = null;
            GestioneMaggiorazioniBenefici.GetListaCodiceRequisitiLegge50392(out listaCodiceRequisitiLegge50392);
            if (listaCodiceRequisitiLegge50392 != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaCodiceRequisitiLegge50392 = listaCodiceRequisitiLegge50392;
            }

            List<Entity.CodiceMaggiorazioneExCombattente> listaCodiceMaggiorazioneExCombattente = null;
            GestioneMaggiorazioniBenefici.GetListaCodiceMaggiorazioneExCombattente(out listaCodiceMaggiorazioneExCombattente);
            if (listaCodiceMaggiorazioneExCombattente != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaCodiceMaggiorazioneExCombattente = listaCodiceMaggiorazioneExCombattente;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        public AreaEsito StoreMaggiorazioniBenefici(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            AreaEsito Esito = new AreaEsito();

            Esito = StoreDatiLegge140Private(numeroDomanda, areaMaggiorazioniBenefici, false);
            //Esito = StoreDatiLegge140(numeroDomanda, areaMaggiorazioniBenefici);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            //Esito = StoreDatiLegge336(numeroDomanda, areaMaggiorazioniBenefici);
            Esito = StoreDatiLegge336Private(numeroDomanda, areaMaggiorazioniBenefici, false);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            Esito = StoreDatiAltriBenefici(numeroDomanda, areaMaggiorazioniBenefici);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            Esito = StoreDatiAgevolazioni(numeroDomanda, areaMaggiorazioniBenefici);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        public AreaEsito CancelMaggiorazioniBenefici(long numeroDomanda)
        {
            AreaEsito Esito = new AreaEsito();
            GestioneMaggiorazioniBenefici.EliminaMaggiorazioniBenefici(numeroDomanda);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        #region DatiLegge140

        public AreaEsito StoreDatiLegge140(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            Esito = StoreDatiLegge140Private(numeroDomanda, areaMaggiorazioniBenefici, true);
            return Esito;
        }       

        public AreaEsito CancelDatiLegge140(long numeroDomanda)
        {
            AreaEsito Esito = new AreaEsito();
            GestioneMaggiorazioniBenefici.EliminaDatiLegge140(numeroDomanda);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito StoreDatiLegge140Private(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici, bool SingleTab)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            GestioneMaggiorazioniBenefici.ControlDatiLegge140(numeroDomanda, areaMaggiorazioniBenefici.DatiLegge140, areaMaggiorazioniBenefici.DatiLegge336, out messaggioControllo, SingleTab);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }
            GestioneMaggiorazioniBenefici.StoreDatiLegge140(numeroDomanda, areaMaggiorazioniBenefici.DatiLegge140);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }     

        #endregion DatiLegge140

        #region DatiAltriBenefici

        public AreaEsito StoreDatiAltriBenefici(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            GestioneMaggiorazioniBenefici.ControlDatiAltriBenefici(numeroDomanda, areaMaggiorazioniBenefici.DatiAltriBenefici, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            GestioneMaggiorazioniBenefici.StoreDatiAltriBenefici(numeroDomanda, areaMaggiorazioniBenefici.DatiAltriBenefici);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        public AreaEsito CancelDatiAltriBenefici(long numeroDomanda)
        {
            AreaEsito Esito = new AreaEsito();
            GestioneMaggiorazioniBenefici.EliminaDatiAltriBenefici(numeroDomanda);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        #endregion DatiAltriBenefici

        #region DatiAgevolazioni

        public AreaEsito StoreDatiAgevolazioni(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            GestioneMaggiorazioniBenefici.ControlDatiAgevolazioni(numeroDomanda, areaMaggiorazioniBenefici.DatiAgevolazioni, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            GestioneMaggiorazioniBenefici.StoreDatiAgevolazioni(numeroDomanda, areaMaggiorazioniBenefici.DatiAgevolazioni);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        public AreaEsito CancelDatiAgevolazioni(long numeroDomanda)
        {
            AreaEsito Esito = new AreaEsito();
            GestioneMaggiorazioniBenefici.EliminaDatiAgevolazioni(numeroDomanda);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        #endregion DatiAgevolazioni

        #region DatiLegge336

        public AreaEsito StoreDatiLegge336(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            AreaEsito Esito = new AreaEsito();

            Esito = StoreDatiLegge336Private(numeroDomanda, areaMaggiorazioniBenefici, true);
            return Esito;
        }

        public AreaEsito CancelDatiLegge336(long numeroDomanda)
        {
            AreaEsito Esito = new AreaEsito();
            GestioneMaggiorazioniBenefici.EliminaDatiLegge336(numeroDomanda);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        private AreaEsito StoreDatiLegge336Private(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici, bool singleTab)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            GestioneMaggiorazioniBenefici.ControlDatiLegge336(numeroDomanda, areaMaggiorazioniBenefici.DatiLegge336, areaMaggiorazioniBenefici.DatiLegge140, out messaggioControllo, singleTab);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }
            GestioneMaggiorazioniBenefici.StoreDatiLegge336(numeroDomanda, areaMaggiorazioniBenefici.DatiLegge336);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        } 

        #endregion DatiLegge336

        #region DatiDL407

        public AreaEsito StoreDatiDL407(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            AreaEsito Esito = new AreaEsito();

            Esito = StoreDatiDL407Private(numeroDomanda, areaMaggiorazioniBenefici, true);
            return Esito;
        }

        public AreaEsito CancelDatiDL407(long numeroDomanda)
        {
            AreaEsito Esito = new AreaEsito();
            GestioneMaggiorazioniBenefici.EliminaDL407(numeroDomanda);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        private AreaEsito StoreDatiDL407Private(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici, bool singleTab)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            //GestioneMaggiorazioniBenefici.ControlDatiLegge336(numeroDomanda, areaMaggiorazioniBenefici.DatiLegge336, areaMaggiorazioniBenefici.DatiLegge140, out messaggioControllo, singleTab);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }
            GestioneMaggiorazioniBenefici.StoreDatiDL407(numeroDomanda, areaMaggiorazioniBenefici.DatiDL407);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        } 

        #endregion DatiDL407
        */
        #endregion AreaMaggiorazioniBenefici

        #region AreaCalcoloDomanda
        public AreaEsito CalcolaDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, bool isVerify, bool isReingegnerizzato, AreaQuadri areaQuadri, bool isConsultazioniANFVerificate, out string statoPensione, out int certificato, out string chiavePensione, out List<GestioneFamiliari.ConsultazioneUnificataANF> listaConsultazioniANF, out List<GestioneAnniRichiestaBonus.DatiPrenotazioneElaborazioni> listaPrenotazioneElaborazioni, out string transactionId, out string flagIndennizzo)
        {
            SetCulture();

            bool isCodiceEsito9 = false;
            flagIndennizzo = null;
            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            statoPensione = string.Empty;
            certificato = 0;
            chiavePensione = string.Empty;
            listaConsultazioniANF = null;
            listaPrenotazioneElaborazioni = null;
            AreaEsito Esito = new AreaEsito();
            transactionId = null;

            try
            {
                //ENG - Meta Processo: inserito blocco per le domande con CodiceSedeLavorazione non valorizzato. Per CI vale solo per le RIC/TRF. Per FS/AGO vale per tutte le domande
                GestioneControlliDinamici.ControlloDinamico ctrlSbloccaMetaProcesso = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("SbloccaMetaProcesso", out ctrlSbloccaMetaProcesso);
                bool bloccaInvioCalcoloDomanda = false;

                if (ctrlSbloccaMetaProcesso != null && !String.IsNullOrEmpty(ctrlSbloccaMetaProcesso.ValoreControllo) && ctrlSbloccaMetaProcesso.ValoreControllo.Trim().ToUpperInvariant() == "SI")
                {
                    if (!datiPensione.CodiceSedeLavorazione.HasValue)
                    {
                        Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

                        if (tipoAppartenenza == Utility.TipoAppartenenza.CI)
                        {
                            if (Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id))
                            {
                                bloccaInvioCalcoloDomanda = true;
                            }

                        }
                        else if (tipoAppartenenza == Utility.TipoAppartenenza.AGO || tipoAppartenenza == Utility.TipoAppartenenza.FS)
                        {
                            bloccaInvioCalcoloDomanda = true;
                        }
                    }

                    if (bloccaInvioCalcoloDomanda)
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = "Ai fini della corretta attribuzione della sede di lavorazione sulle comunicazioni. Si prega di cancellare e ri-prelevare la domanda";
                        return Esito;
                    }
                }


                if (!VerificaAbilitaCalcolo(areaQuadri))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = "Non è possibile inviare la domanda al calcolo. Non tutti i quadri obbligatori sono stati acquisiti.";
                    return Esito;
                }

                if (datiPensione.TipoAutomazione == null && GestioneCtrlMatricoleAutomazione.IsMatricolaForAutomazione(matricolaOperatore))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = "Domanda non automatizzata che non è processabile da prodotti di RPA.";
                    return Esito;
                }

                Entity.ParametriARCA parametriArca = null;
                if (!ValorizzaParametriARCA(matricolaOperatore, out parametriArca))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = "Errore nel recupero delle informazioni anagrafiche";
                    string messaggio = "Errore nella valorizzazione dei parametri ARCA";
                    string parametri = string.Format("Matricola Operatore: {0}", matricolaOperatore);
                    GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, null);
                    return Esito;
                }

                string messaggioVideo;
                byte? statoPrecedente = datiPensione != null ? datiPensione.StatoPensione : null;
                if (GestioneCalcoloDomanda.CalcolaDomandaByDatiPensione(parametriArca, datiPensione, matricolaOperatore, sedeOperatore, centroOperativoOperatore, isVerify,
                    isConsultazioniANFVerificate, isReingegnerizzato, out statoPensione, out certificato, out chiavePensione, out listaConsultazioniANF, out listaPrenotazioneElaborazioni, out transactionId, out flagIndennizzo, out messaggioVideo, out isCodiceEsito9) && !isCodiceEsito9)
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = messaggioVideo;
                }
                else if (isCodiceEsito9)
                {

                    GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
                    GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);

                    string codiceFiscale = (datiAnagraficiTitolare != null && datiAnagraficiTitolare.CodiceFiscale != null) ? datiAnagraficiTitolare.CodiceFiscale : "";
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = string.Format("Aggiornamento Piani Di Pagamento eseguito correttamente. Per il soggetto pensionato identificato dal CF: {0} risulta già essere presente in banca dati un conguaglio associato alla pensione {1} il cui piano di recupero è caratterizzato dal medesimo numero di rate complessive e dal medesimo importo rata", codiceFiscale, chiavePensione);

                }
                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                }
                //SCRIWO       
                Utility.StatoPensione? stato = Utility.GetStatoPensioneByDescrizione(statoPensione);
                if (stato.HasValue && datiPensione != null) datiPensione.StatoPensione = (byte)stato;
                GestioneWSSCRIWO.AggiornaStatoLavorazione(datiPensione, statoPrecedente, matricolaOperatore, sedeOperatore);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nel calcolo della domanda. Riprovare più tardi";
            }
            return Esito;
        }

        public AreaEsito GetIsDomandaVerify(long numeroDomanda, out bool isVerify)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            isVerify = false;
            AreaEsito Esito = new AreaEsito();
            try
            {
                isVerify = GestioneCalcoloDomanda.IsCalcoloVerify(datiPensione);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nel recupero delle informazioni riguardanti la tipologia di calcolo. Riprovare più tardi";
            }
            return Esito;
        }

        public AreaEsito IsNuovoCalcolo(long numeroDomanda, bool isVerify, out bool isNuovoCalcolo, out bool esitoInattesa)
        {
            SetCulture();
            AreaEsito Esito = new AreaEsito();
            isNuovoCalcolo = false;
            esitoInattesa = false;
            try
            {
                EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
                EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);
                GestioneNuovoCalcolo.FlowConf confDomanda;
                isNuovoCalcolo = Utility.IsNuovoCalcolo(contenitore.DatiPensione, isVerify, out confDomanda);

                if (isNuovoCalcolo)
                {
                    string messaggioTimeout1 = "Timeout utilizzato: ";
                    string messaggioTimeout2 = "default ";
                    Guid guid = Guid.NewGuid();
                    //gestione timeout
                    GestioneControlliDinamici.ControlloDinamico controlloDinamico;
                    int defaultTimeout = 0;
                    //setto subito il default in quanto per le domande puntuali non ho la riga di configurazione e devo applicare quello
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DefaultTimeoutNuovoCalcolo", out controlloDinamico);
                    if (controlloDinamico != null) int.TryParse(controlloDinamico.ValoreControllo, out defaultTimeout);

                    if (confDomanda != null && confDomanda.TimeoutElaborazione.HasValue)
                    {
                        //se ho la riga di configurazione con timeout settato, aggiorno il default
                        defaultTimeout = confDomanda.TimeoutElaborazione.Value;
                        messaggioTimeout2 = "configurazione ";
                    }

                    //defaultTimeout = 300; //TEST, CANCELLARE
                    esitoInattesa = GestioneNuovoCalcolo.GetRisposteValideNuovoCalcoloByNDomus(contenitore.DatiPensione.NDomus, defaultTimeout);

                    messaggioTimeout2 = messaggioTimeout2 + defaultTimeout.ToString();
                    GestioneLogSoap.SalvaLogSoap(messaggioTimeout1 + messaggioTimeout2, Utility.Servizio.SrvNuovoCalcolo, Utility.MetodoServizio.IsNuovoCalcolo, Utility.SOAPLogDirection.IN, numeroDomanda.ToString(), guid);

                }

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;

            }
            catch (Exception Ex)
            {
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, Utility.GetMessageFromException(Ex), null, Ex.StackTrace);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nella verifica della domanda. Riprovare più tardi";
            }
            return Esito;
        }

        #endregion AreaCalcoloDomanda

        #region AreaStampaDomanda
        public AreaEsito GetStampaDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out MemoryStream msPDF)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            msPDF = null;
            AreaEsito Esito = new AreaEsito();
            try
            {
                string errore = string.Empty;
                if (GestioneAreaStampa.GetStampaByDatiPensione(datiPensione, out msPDF, out errore))
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                else
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = errore;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nel recupero della stampa della domanda. Riprovare più tardi";
            }
            return Esito;
        }

        public AreaEsito GetStampaDomandaByChiavePensione(AreaRichiestaStampa areaStampa, out MemoryStream msPDF)
        {
            SetCulture();

            msPDF = null;
            AreaEsito Esito = new AreaEsito();
            try
            {
                string errore = string.Empty;

                if (areaStampa == null)
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = "Valorizzazione della richiesta errata.";
                }

                if (GestioneAreaStampa.GetStampaByChiavePensione(areaStampa.SiglaCategoria, areaStampa.CodiceSede, areaStampa.Certificato, out msPDF, out errore))
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                else
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = errore;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nel recupero della stampa della domanda. Riprovare più tardi";
            }
            return Esito;
        }

        public AreaEsito DeleteStampaWeb(AreaRichiestaDomanda areaRichiestaDomanda)
        {
             AreaEsito areaEsito = new AreaEsito();
            try
            {
                string messaggioErrore = string.Empty;

                GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

                if (datiPensione == null)
                {
                    areaEsito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    areaEsito.Messaggio = string.Format("Dati Pensione Non Trovati Per Domus: {0} e ProgressivoStorico: {1}", areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);
                    GestioneLogGenerico.SalvaLogGenerico(areaRichiestaDomanda.NumeroDomanda, "DeleteStampaWeb", Utility.TipoLogGenerico.ErroreApplicativo, "Errore Chiamata DeleteStampaWeb", JsonConvert.SerializeObject(areaEsito, new StringEnumConverter()), string.Empty);
                    return areaEsito;
                }

                bool flagRisultato = GestioneAreaStampa.CancelStampaByIdPensione(datiPensione.Id, out messaggioErrore);

                if (!flagRisultato)
                {
                    areaEsito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    areaEsito.Messaggio = messaggioErrore;
                    GestioneLogGenerico.SalvaLogGenerico(areaRichiestaDomanda.NumeroDomanda, "DeleteStampaWeb", Utility.TipoLogGenerico.ErroreApplicativo, "Errore Chiamata DeleteStampaWeb", JsonConvert.SerializeObject(areaEsito, new StringEnumConverter()), string.Empty);
                }
                else
                {
                    areaEsito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    areaEsito.Messaggio = messaggioErrore;
                }
            }
            catch (Exception Ex)
            {
                areaEsito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                areaEsito.Messaggio = "Errore tecnico nella cancellazione della stampa della domanda. Riprovare più tardi";
                GestioneLogGenerico.SalvaLogGenerico(areaRichiestaDomanda.NumeroDomanda, "DeleteStampaWeb", Utility.TipoLogGenerico.ErroreApplicativo, Ex.Message, JsonConvert.SerializeObject(areaRichiestaDomanda, new StringEnumConverter()), Ex.StackTrace);
            }

            return areaEsito;
        }
        #endregion AreaStampaDomanda

        #region AreaLiquidazioniAbilitate
        public AreaEsito GetAllLiquidazioniAbilitate(Utility.TipoAppartenenza tipoAppRuolo, out AreaLiquidazioniAbilitate areaLiquidazioniAbilitate)
        {
            SetCulture();

            areaLiquidazioniAbilitate = new AreaLiquidazioniAbilitate();
            AreaEsito Esito = new AreaEsito();
            try
            {
                List<GestioneLiquidazioniAbilitate.LiquidazioneAbilitata> elencoLiquidazioniAbilitate = null;
                GestioneAreaLiquidazioniAbilitate.GetAllLiquidazioniAbilitate(out elencoLiquidazioniAbilitate);

                if (elencoLiquidazioniAbilitate != null && elencoLiquidazioniAbilitate.Count > 0)
                {
                    elencoLiquidazioniAbilitate = elencoLiquidazioniAbilitate.FindAll(x => x.Tipologia == tipoAppRuolo.ToString());
                    foreach (GestioneLiquidazioniAbilitate.LiquidazioneAbilitata la in elencoLiquidazioniAbilitate)
                        areaLiquidazioniAbilitate.ElencoLiquidazioniAbilitate.Add(new AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata(la));
                }

                List<string> elencoSigleCat = null;
                Utility.GetListaSigleCategoriePerTipoApp(out elencoSigleCat, tipoAppRuolo.ToString());
                areaLiquidazioniAbilitate.ElencoSigleCategorie = elencoSigleCat;

                List<INPS.DNA.Office> elencoSediProvinciali = null;
                GestioneLiquidazioniAbilitate.GetSediAmmesse(out elencoSediProvinciali);
                areaLiquidazioniAbilitate.ElencoSedi = elencoSediProvinciali;
                areaLiquidazioniAbilitate.ElencoSigleCategorieINPDAP = Utility.GetListaSigleCategorieINPDAP();
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nel recupero dell'elenco di liquidazioni abilitate. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito StoreLiquidazioneAbilitata(AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata datiLiquidazioneAbilitata)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            try
            {
                if (datiLiquidazioneAbilitata == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna liquidazione abilitata da salvare");

                GestioneLiquidazioniAbilitate.LiquidazioneAbilitata liquidazioneAbilitata = new GestioneLiquidazioniAbilitate.LiquidazioneAbilitata();
                liquidazioneAbilitata.SiglaCategoria = datiLiquidazioneAbilitata.SiglaCategoria.Trim().ToUpperInvariant();
                liquidazioneAbilitata.Sede = Utility.StringToNullableShort(datiLiquidazioneAbilitata.Sede.PadLeft(4, '0').Substring(0, 4));
                switch (datiLiquidazioneAbilitata.Tipologia)
                {
                    case AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo.FS:
                        liquidazioneAbilitata.Tipologia = "FS";
                        break;
                    case AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo.AGO:
                        liquidazioneAbilitata.Tipologia = "AGO";
                        break;
                    case AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo.CI:
                        liquidazioneAbilitata.Tipologia = "CI";
                        break;
                }
                liquidazioneAbilitata.Ricostituzione = datiLiquidazioneAbilitata.Ricostituzione;
                liquidazioneAbilitata.AbilitazioneManuale = datiLiquidazioneAbilitata.AbilitazioneManuale;
                liquidazioneAbilitata.RicostituzioneDaAutomatica = datiLiquidazioneAbilitata.RicostituzioneDaAutomatica;
                liquidazioneAbilitata.AbilitazioneAutomatica = datiLiquidazioneAbilitata.AbilitazioneAutomatica;

                GestioneAreaLiquidazioniAbilitate.StoreLiquidazioneAbilitata(liquidazioneAbilitata);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                return Esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nel salvataggio. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito DeleteLiquidazioneAbilitata(AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata datiLiquidazioneAbilitata)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            try
            {
                if (datiLiquidazioneAbilitata == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna liquidazione abilitata da eliminare");

                GestioneLiquidazioniAbilitate.LiquidazioneAbilitata liquidazioneAbilitata = new GestioneLiquidazioniAbilitate.LiquidazioneAbilitata();
                liquidazioneAbilitata.SiglaCategoria = datiLiquidazioneAbilitata.SiglaCategoria.Trim().ToUpperInvariant();
                liquidazioneAbilitata.Sede = Utility.StringToNullableShort(datiLiquidazioneAbilitata.Sede.PadLeft(4, '0').Substring(0, 4));
                switch (datiLiquidazioneAbilitata.Tipologia)
                {
                    case AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo.FS:
                        liquidazioneAbilitata.Tipologia = "FS";
                        break;
                    case AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo.AGO:
                        liquidazioneAbilitata.Tipologia = "AGO";
                        break;
                    case AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo.CI:
                        liquidazioneAbilitata.Tipologia = "CI";
                        break;
                }
                liquidazioneAbilitata.Ricostituzione = datiLiquidazioneAbilitata.Ricostituzione;
                liquidazioneAbilitata.AbilitazioneManuale = datiLiquidazioneAbilitata.AbilitazioneManuale;

                GestioneAreaLiquidazioniAbilitate.CancelLiquidazioneAbilitata(liquidazioneAbilitata);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                return Esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nell'eliminazione. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito StoreLiquidazioniAbilitateSuTutteLeSedi(AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata datiLiquidazioneAbilitata)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            try
            {
                if (datiLiquidazioneAbilitata == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna liquidazione abilitata da salvare");

                GestioneLiquidazioniAbilitate.LiquidazioneAbilitata liquidazioneAbilitata = new GestioneLiquidazioniAbilitate.LiquidazioneAbilitata();
                liquidazioneAbilitata.SiglaCategoria = datiLiquidazioneAbilitata.SiglaCategoria.Trim().ToUpperInvariant();
                switch (datiLiquidazioneAbilitata.Tipologia)
                {
                    case AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo.FS:
                        liquidazioneAbilitata.Tipologia = "FS";
                        break;
                    case AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo.AGO:
                        liquidazioneAbilitata.Tipologia = "AGO";
                        break;
                    case AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo.CI:
                        liquidazioneAbilitata.Tipologia = "CI";
                        break;
                }
                liquidazioneAbilitata.Ricostituzione = datiLiquidazioneAbilitata.Ricostituzione;
                liquidazioneAbilitata.AbilitazioneManuale = datiLiquidazioneAbilitata.AbilitazioneManuale;

                GestioneAreaLiquidazioniAbilitate.StoreLiquidazioniAbilitateSuTutteLeSedi(liquidazioneAbilitata);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                return Esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nel salvataggio su tutte le sedi. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito DeleteLiquidazioniAbilitateSuTutteLeSedi(AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata datiLiquidazioneAbilitata)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            try
            {
                if (datiLiquidazioneAbilitata == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna liquidazione abilitata da eliminare");

                GestioneLiquidazioniAbilitate.LiquidazioneAbilitata liquidazioneAbilitata = new GestioneLiquidazioniAbilitate.LiquidazioneAbilitata();
                liquidazioneAbilitata.SiglaCategoria = datiLiquidazioneAbilitata.SiglaCategoria.Trim().ToUpperInvariant();
                switch (datiLiquidazioneAbilitata.Tipologia)
                {
                    case AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo.FS:
                        liquidazioneAbilitata.Tipologia = "FS";
                        break;
                    case AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo.AGO:
                        liquidazioneAbilitata.Tipologia = "AGO";
                        break;
                    case AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo.CI:
                        liquidazioneAbilitata.Tipologia = "CI";
                        break;
                }
                liquidazioneAbilitata.Ricostituzione = datiLiquidazioneAbilitata.Ricostituzione;
                liquidazioneAbilitata.AbilitazioneManuale = datiLiquidazioneAbilitata.AbilitazioneManuale;

                GestioneAreaLiquidazioniAbilitate.CancelLiquidazioneAbilitata(liquidazioneAbilitata);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                return Esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nell'eliminazione su tutte le sedi. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }
        #endregion AreaLiquidazioniAbilitate

        #region AreaTrasformazioniAbilitate
        public AreaEsito GetAllTrasformazioniAbilitate(Utility.TipoAppartenenza tipoAppRuolo, out AreaTrasformazioniAbilitate areaTrasformazioniAbilitate)
        {
            SetCulture();

            areaTrasformazioniAbilitate = new AreaTrasformazioniAbilitate();
            AreaEsito Esito = new AreaEsito();
            try
            {
                List<GestioneTrasformazioniAbilitate.TrasformazioneAbilitata> elencoTrasformazioniAbilitate = null;
                GestioneAreaTrasformazioniAbilitate.GetAllTrasformazioniAbilitate(out elencoTrasformazioniAbilitate);

                if (elencoTrasformazioniAbilitate != null && elencoTrasformazioniAbilitate.Count > 0)
                {
                    elencoTrasformazioniAbilitate = elencoTrasformazioniAbilitate.FindAll(x => x.Tipologia == tipoAppRuolo.ToString());
                    foreach (GestioneTrasformazioniAbilitate.TrasformazioneAbilitata tra in elencoTrasformazioniAbilitate)
                        areaTrasformazioniAbilitate.ElencoTrasformazioniAbilitate.Add(new AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata(tra));
                }

                List<string> elencoSigleCat = null;
                Utility.GetListaSigleCategoriePerTipoApp(out elencoSigleCat, tipoAppRuolo.ToString());
                areaTrasformazioniAbilitate.ElencoSigleCategorie = elencoSigleCat;

                List<INPS.DNA.Office> elencoSediProvinciali = null;
                GestioneTrasformazioniAbilitate.GetSediAmmesse(out elencoSediProvinciali);
                areaTrasformazioniAbilitate.ElencoSedi = elencoSediProvinciali;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nel recupero dell'elenco di trasformazioni abilitate. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito StoreTrasformazioneAbilitata(AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata datiTrasformazioneAbilitata)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            try
            {
                if (datiTrasformazioneAbilitata == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna trasformazione abilitata da salvare");

                GestioneTrasformazioniAbilitate.TrasformazioneAbilitata trasformazioneAbilitata = new GestioneTrasformazioniAbilitate.TrasformazioneAbilitata();
                trasformazioneAbilitata.SiglaCategoria = datiTrasformazioneAbilitata.SiglaCategoria.Trim().ToUpperInvariant();
                trasformazioneAbilitata.Sede = Utility.StringToNullableShort(datiTrasformazioneAbilitata.Sede.PadLeft(4, '0').Substring(0, 4));
                switch (datiTrasformazioneAbilitata.Tipologia)
                {
                    case AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata.Tipo.FS:
                        trasformazioneAbilitata.Tipologia = "FS";
                        break;
                    case AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata.Tipo.AGO:
                        trasformazioneAbilitata.Tipologia = "AGO";
                        break;
                    case AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata.Tipo.CI:
                        trasformazioneAbilitata.Tipologia = "CI";
                        break;
                }

                GestioneAreaTrasformazioniAbilitate.StoreTrasformazioneAbilitata(trasformazioneAbilitata);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                return Esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nel salvataggio. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito DeleteTrasformazioneAbilitata(AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata datiTrasformazioneAbilitata)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            try
            {
                if (datiTrasformazioneAbilitata == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna trasformazione abilitata da eliminare");

                GestioneTrasformazioniAbilitate.TrasformazioneAbilitata trasformazioneAbilitata = new GestioneTrasformazioniAbilitate.TrasformazioneAbilitata();
                trasformazioneAbilitata.SiglaCategoria = datiTrasformazioneAbilitata.SiglaCategoria.Trim().ToUpperInvariant();
                trasformazioneAbilitata.Sede = Utility.StringToNullableShort(datiTrasformazioneAbilitata.Sede.PadLeft(4, '0').Substring(0, 4));
                switch (datiTrasformazioneAbilitata.Tipologia)
                {
                    case AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata.Tipo.FS:
                        trasformazioneAbilitata.Tipologia = "FS";
                        break;
                    case AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata.Tipo.AGO:
                        trasformazioneAbilitata.Tipologia = "AGO";
                        break;
                    case AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata.Tipo.CI:
                        trasformazioneAbilitata.Tipologia = "CI";
                        break;
                }

                GestioneAreaTrasformazioniAbilitate.CancelTrasformazioneAbilitata(trasformazioneAbilitata);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                return Esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nell'eliminazione. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito StoreTrasformazioniAbilitateSuTutteLeSedi(AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata datitrasformazioneAbilitata)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            try
            {
                if (datitrasformazioneAbilitata == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna trasformazione abilitata da salvare");

                GestioneTrasformazioniAbilitate.TrasformazioneAbilitata trasformazioneAbilitata = new GestioneTrasformazioniAbilitate.TrasformazioneAbilitata();
                trasformazioneAbilitata.SiglaCategoria = datitrasformazioneAbilitata.SiglaCategoria.Trim().ToUpperInvariant();
                switch (datitrasformazioneAbilitata.Tipologia)
                {
                    case AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata.Tipo.FS:
                        trasformazioneAbilitata.Tipologia = "FS";
                        break;
                    case AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata.Tipo.AGO:
                        trasformazioneAbilitata.Tipologia = "AGO";
                        break;
                    case AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata.Tipo.CI:
                        trasformazioneAbilitata.Tipologia = "CI";
                        break;
                }

                GestioneAreaTrasformazioniAbilitate.StoreTrasaformazioniAbilitateSuTutteLeSedi(trasformazioneAbilitata);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                return Esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nel salvataggio su tutte le sedi. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito DeleteTrasformazioniAbilitateSuTutteLeSedi(AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata datiTrasformazioneAbilitata)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            try
            {
                if (datiTrasformazioneAbilitata == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna trasformazione abilitata da eliminare");

                GestioneTrasformazioniAbilitate.TrasformazioneAbilitata trasformazioneAbilitata = new GestioneTrasformazioniAbilitate.TrasformazioneAbilitata();
                trasformazioneAbilitata.SiglaCategoria = datiTrasformazioneAbilitata.SiglaCategoria.Trim().ToUpperInvariant();
                switch (datiTrasformazioneAbilitata.Tipologia)
                {
                    case AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata.Tipo.FS:
                        trasformazioneAbilitata.Tipologia = "FS";
                        break;
                    case AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata.Tipo.AGO:
                        trasformazioneAbilitata.Tipologia = "AGO";
                        break;
                    case AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata.Tipo.CI:
                        trasformazioneAbilitata.Tipologia = "CI";
                        break;
                }

                GestioneAreaTrasformazioniAbilitate.CancelTrasformazioneAbilitata(trasformazioneAbilitata);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                return Esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nell'eliminazione su tutte le sedi. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }
        #endregion AreaTrasformazioniAbilitate

        #region AreaAggiornamentoCI05
        public AreaEsito AggiornaCI05(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, out string statoPensione)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);
            statoPensione = string.Empty;
            AreaEsito Esito = new AreaEsito();
            try
            {
                string messaggioVideo = string.Empty;
                byte? statoPrecedente = datiPensione != null ? datiPensione.StatoPensione : null;
                if (!GestioneAllegatiConvenzioni.AggiornaCI05(datiPensione, datiDanteCausa, matricolaOperatore, sedeOperatore, centroOperativoOperatore, ref statoPensione, ref messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                }
                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = "Aggiornamento CI05 eseguito correttamente";
                }
                //SCRIWO
                GestioneWSSCRIWO.AggiornaStatoLavorazione(datiPensione, statoPrecedente, matricolaOperatore, sedeOperatore);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nell'aggiornamento CI05";
            }
            return Esito;
        }
        #endregion AreaAggiornamentoCI05

        #region AreaAggiornamentoWebDom
        public AreaEsito AggiornaWebDom(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, out string statoPensione)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);
            statoPensione = string.Empty;
            AreaEsito Esito = new AreaEsito();
            try
            {
                byte? statoPrecedente = datiPensione != null ? datiPensione.StatoPensione : null;
                string messaggioVideo = string.Empty;
                if (!GestioneWebDom.AggiornaWebDom(datiPensione, datiDanteCausa, matricolaOperatore, sedeOperatore, out statoPensione, out messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                }
                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = "Aggiornamento WebDom eseguito correttamente";
                }
                //SCRIWO
                GestioneWSSCRIWO.AggiornaStatoLavorazione(datiPensione, statoPrecedente, matricolaOperatore, sedeOperatore);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nell'aggiornamento WebDom";
            }
            return Esito;
        }
        #endregion AreaAggiornamentoWebDom

        #region AreaAggiornamentoFelpe
        public AreaEsito AggiornaFelpe(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, out string statoPensione)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            statoPensione = string.Empty;
            AreaEsito Esito = new AreaEsito();
            try
            {
                string messaggioVideo = string.Empty;
                byte? statoPrecedente = datiPensione != null ? datiPensione.StatoPensione : null;
                if (!GestioneAggiornamentoPECO.AggiornaFelpe(datiPensione, datiDanteCausa, matricolaOperatore, sedeOperatore, out statoPensione, out messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                }
                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = "Aggiornamento Felpe eseguito correttamente";
                }
                //SCRIWO
                GestioneWSSCRIWO.AggiornaStatoLavorazione(datiPensione, statoPrecedente, matricolaOperatore, sedeOperatore);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nell'aggiornamento Felpe";
            }
            return Esito;
        }
        #endregion AreaAggiornamentoFelpe

        #region AreaAggiornamentoOneri
        public AreaEsito AggiornaOneri(long numeroDomanda, out string statoPensione)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);
            statoPensione = string.Empty;
            AreaEsito Esito = new AreaEsito();
            try
            {
                string messaggioVideo = string.Empty;
                byte? statoPrecedente = datiPensione != null ? datiPensione.StatoPensione : null;
                if (!GestioneOneriPrepensionamento.AggiornaOneri(datiPensione, datiDanteCausa, out statoPensione, out messaggioVideo))
                {
                    if (messaggioVideo.StartsWith("Si è verificato un errore durante l'esecuzione della scrittura sulla tabella DB2 TOPPL03"))
                        messaggioVideo = "Si è verificato un errore durante l'esecuzione della scrittura sulla tabella DB2 TOPPL03";
                    else if (messaggioVideo.StartsWith("Si è verificato un errore durante l'esecuzione di lettura sulla tabella DB2 TOPPL03"))
                        messaggioVideo = "Si è verificato un errore durante l'esecuzione di lettura sulla tabella DB2 TOPPL03";
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                }
                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = "Aggiornamento Oneri eseguito correttamente";
                }
                //SCRIWO
                GestioneWSSCRIWO.AggiornaStatoLavorazione(datiPensione, statoPrecedente, string.Empty, 0);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nell'aggiornamento Oneri";
            }
            return Esito;
        }
        #endregion AreaAggiornamentoOneri

        #region AreaAggiornamentoSai
        public AreaEsito AggiornaSai(long numeroDomanda, out string statoPensione)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);
            statoPensione = string.Empty;
            AreaEsito Esito = new AreaEsito();
            try
            {
                string messaggioVideo = string.Empty;
                byte? statoPrecedente = datiPensione != null ? datiPensione.StatoPensione : null;
                if (!GestioneSAI.AggiornaSAI(datiPensione, datiDanteCausa, out statoPensione, out messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                }
                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = "Aggiornamento SAI eseguito correttamente";
                }
                //SCRIWO
                GestioneWSSCRIWO.AggiornaStatoLavorazione(datiPensione, statoPrecedente, string.Empty, 0);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nell'aggiornamento SAI";
            }
            return Esito;
        }
        #endregion AreaAggiornamentoSai

        #region AreaAggiornamentoINPDAP
        public AreaEsito AggiornaINPDAP(long numeroDomanda, out string statoPensione)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            statoPensione = string.Empty;
            AreaEsito Esito = new AreaEsito();
            try
            {
                string messaggioVideo = string.Empty;
                byte? statoPrecedente = datiPensione != null ? datiPensione.StatoPensione : null;
                if (!GestioneINPDAP.AggiornaINPDAP(datiPensione, out statoPensione, out messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                }
                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = "Aggiornamento INPDAP eseguito correttamente";
                }
                //SCRIWO
                GestioneWSSCRIWO.AggiornaStatoLavorazione(datiPensione, statoPrecedente, string.Empty, 0);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nell'aggiornamento INPDAP";
            }
            return Esito;
        }

        public AreaEsito AggiornaNoteDiDebito(long numeroDomanda, out string statoPensione)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            statoPensione = string.Empty;
            AreaEsito Esito = new AreaEsito();
            try
            {
                string messaggioVideo = string.Empty;
                byte? statoPrecedente = datiPensione != null ? datiPensione.StatoPensione : null;
                if (!GestioneINPDAP.AggiornaNoteDiDebito(datiPensione, out statoPensione, out messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                }
                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = "Aggiornamento Note di debito eseguito correttamente";
                }
                //SCRIWO
                GestioneWSSCRIWO.AggiornaStatoLavorazione(datiPensione, statoPrecedente, string.Empty, 0);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nell'aggiornamento Note di debito";
            }
            return Esito;
        }
        public AreaEsito AggiornaPianiDiPagamento(long numeroDomanda, out string statoPensione)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            statoPensione = string.Empty;
            AreaEsito Esito = new AreaEsito();
            bool isCodiceEsito9 = false;
            try
            {
                string messaggioVideo = string.Empty;
                byte? statoPrecedente = datiPensione != null ? datiPensione.StatoPensione : null;
                if (!GestioneINPDAP.AggiornaPianiDiPagamento(datiPensione, out statoPensione, out messaggioVideo, out isCodiceEsito9))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                }
                else if (isCodiceEsito9 == true)
                {
                    GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
                    GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);
                    string chiavePensione = datiPensione.GetCodCategoria().Substring(1) + (datiPensione.CodiceSedeDestinazione.HasValue ? datiPensione.CodiceSedeDestinazione.Value.ToString().PadLeft(4, '0') :
                                         datiPensione.CodiceSede.ToString().PadLeft(4, '0')) + (datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value.ToString().PadLeft(8, '0') : "");
                    string codiceFiscale = (datiAnagraficiTitolare != null && datiAnagraficiTitolare.CodiceFiscale != null) ? datiAnagraficiTitolare.CodiceFiscale : "";
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = string.Format("Aggiornamento Piani Di Pagamento eseguito correttamente. Per il soggetto pensionato identificato dal CF: {0} risulta già essere presente in banca dati un conguaglio associato alla pensione {1} il cui piano di recupero è caratterizzato dal medesimo numero di rate complessive e dal medesimo importo rata", codiceFiscale, chiavePensione);
                }
                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = "Aggiornamento Piani Di Pagamento eseguito correttamente";
                }
                //SCRIWO
                GestioneWSSCRIWO.AggiornaStatoLavorazione(datiPensione, statoPrecedente, string.Empty, 0);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nell'aggiornamento Piani Di Pagamento";
            }
            return Esito;
        }

        public AreaEsito AggiornaEquoInd(long numeroDomanda, out string statoPensione)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            statoPensione = string.Empty;
            AreaEsito Esito = new AreaEsito();

            try
            {
                string messaggioVideo = string.Empty;
                byte? statoPrecedente = datiPensione != null ? datiPensione.StatoPensione : null;
                if (!GestioneINPDAP.AggiornaEquoIndennizzo(datiPensione, out statoPensione, out messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                }

                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = "Aggiornamento Piani Di Pagamento eseguito correttamente";
                }
                //SCRIWO
                GestioneWSSCRIWO.AggiornaStatoLavorazione(datiPensione, statoPrecedente, string.Empty, 0);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nell'aggiornamento Piani Di Pagamento";
            }
            return Esito;
        }

        public AreaEsito AggiornaIndennSpec(long numeroDomanda, out string statoPensione)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            statoPensione = string.Empty;
            AreaEsito Esito = new AreaEsito();

            try
            {
                string messaggioVideo = string.Empty;
                byte? statoPrecedente = datiPensione != null ? datiPensione.StatoPensione : null;
                bool isCodiceEsito9 = false;
                if (!GestioneINPDAP.AggiornaIndennitaSpeciale(datiPensione, out statoPensione, out messaggioVideo, out isCodiceEsito9))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                }

                else if (isCodiceEsito9 == true)
                {
                    GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
                    GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);
                    string chiavePensione = datiPensione.GetCodCategoria().Substring(1) + (datiPensione.CodiceSedeDestinazione.HasValue ? datiPensione.CodiceSedeDestinazione.Value.ToString().PadLeft(4, '0') :
                                         datiPensione.CodiceSede.ToString().PadLeft(4, '0')) + (datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value.ToString().PadLeft(8, '0') : "");
                    string codiceFiscale = (datiAnagraficiTitolare != null && datiAnagraficiTitolare.CodiceFiscale != null) ? datiAnagraficiTitolare.CodiceFiscale : "";
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = string.Format("Aggiornamento Piani Di Pagamento eseguito correttamente. Per il soggetto pensionato identificato dal CF: {0} risulta già essere presente in banca dati un conguaglio associato alla pensione {1} il cui piano di recupero è caratterizzato dal medesimo numero di rate complessive e dal medesimo importo rata", codiceFiscale, chiavePensione);
                }

                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = "Aggiornamento Piani Di Pagamento eseguito correttamente";
                }
                //SCRIWO
                GestioneWSSCRIWO.AggiornaStatoLavorazione(datiPensione, statoPrecedente, string.Empty, 0);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nell'aggiornamento Piani Di Pagamento";
            }
            return Esito;
        }

        #endregion AreaAggiornamentoSai

        #region AreaAggiornamentoTotal
        public AreaEsito AggiornaTotal(long numeroDomanda, out string statoPensione)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            statoPensione = string.Empty;
            AreaEsito Esito = new AreaEsito();
            try
            {
                string messaggioVideo = string.Empty;
                byte? statoPrecedente = datiPensione != null ? datiPensione.StatoPensione : null;
                if (!GestioneTotalIvs.AggiornaTotalIVS(datiPensione, out statoPensione, out messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                }
                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = "Aggiornamento Total eseguito correttamente";
                }
                //SCRIWO
                GestioneWSSCRIWO.AggiornaStatoLavorazione(datiPensione, statoPrecedente, string.Empty, 0);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nell'aggiornamento Total";
            }
            return Esito;
        }

        public AreaEsito AggiornaTotalPerTot(long numeroDomanda, out string statoPensione)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            statoPensione = string.Empty;
            AreaEsito Esito = new AreaEsito();
            try
            {
                string messaggioVideo = string.Empty;
                byte? statoPrecedente = datiPensione != null ? datiPensione.StatoPensione : null;
                if (!GestioneTotalIvs.AggiornaTotIVS(datiPensione, out statoPensione, out messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                }
                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = "Aggiornamento Total eseguito correttamente";
                }
                //SCRIWO
                GestioneWSSCRIWO.AggiornaStatoLavorazione(datiPensione, statoPrecedente, string.Empty, 0);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nell'aggiornamento Total";
            }
            return Esito;
        }
        #endregion AreaAggiornamentoTotal

        #region AreaAggiornamentoBooking
        public AreaEsito AggiornaBooking(long numeroDomanda, string matricolaOperatore, short sedeOperatore, out string statoPensione, out List<GestioneAnniRichiestaBonus.DatiPrenotazioneElaborazioni> listaPrenotazioneElaborazioni)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            statoPensione = string.Empty;
            listaPrenotazioneElaborazioni = new List<GestioneAnniRichiestaBonus.DatiPrenotazioneElaborazioni>();
            AreaEsito Esito = new AreaEsito();
            try
            {
                string messaggioVideo = string.Empty;
                byte? statoPrecedente = datiPensione != null ? datiPensione.StatoPensione : null;
                List<GestioneAnniRichiestaBonus.DatiAnniRichiestaBonus> datiAnniRichiestaBonus = null;
                GestioneRichiestaBonus.AreaRichiestaBonus richiestaBonus = new GestioneRichiestaBonus.AreaRichiestaBonus();
                //ENG - Booking FS-AGO
                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

                GestioneAnniRichiestaBonus.GetAnniRichiestaBonus(datiPensione.Id, out datiAnniRichiestaBonus);
                richiestaBonus.Certificato = datiPensione.NCertificato.Value.ToString().PadLeft(8, '0');
                richiestaBonus.Categoria = datiPensione.GetCodCategoria().Substring(1, 3);
                if ((tipoAppartenenza == Utility.TipoAppartenenza.FS || tipoAppartenenza == Utility.TipoAppartenenza.AGO) && Utility.IsRicostituzione_Reddituale(datiPensione) && datiPensione.CodiceSedeDestinazione.HasValue)
                    richiestaBonus.Sede = datiPensione.CodiceSedeDestinazione.Value.ToString().PadLeft(4, '0');
                else
                    richiestaBonus.Sede = datiPensione.CodiceSede.ToString().PadLeft(4, '0');
                int[] listAnniRichiestaBonus = datiAnniRichiestaBonus.Where(x => x.IsRichiestaBonus == true).Select(x => x.Anno).ToArray();
                string anniRichiestaBonus = string.Empty;
                for (int i = 0; i < listAnniRichiestaBonus.Length; i++)
                {
                    anniRichiestaBonus += listAnniRichiestaBonus[i];
                    if (i < listAnniRichiestaBonus.Length - 1)
                        anniRichiestaBonus += '|';
                }
                richiestaBonus.Anni = anniRichiestaBonus;
                if (datiPensione.Tipo == "0167")
                {
                    richiestaBonus.TipoBonus = "B14_I";
                }
                else
                {
                    richiestaBonus.TipoBonus = "B154_I";
                }
                richiestaBonus.NumDomanda = datiPensione.NDomus.ToString();
                if (!GestioneRichiestaBonus.GetPrenotazioneElaborazioni(ref richiestaBonus, matricolaOperatore, sedeOperatore.ToString(), datiPensione.Id))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = richiestaBonus.MessaggioVideo;
                }
                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = "Aggiornamento Booking eseguito correttamente";
                    GestioneAnniRichiestaBonus.SalvaPrenotazioneElaborazioni(datiPensione.Id, richiestaBonus.DatiPrenotazioneElaborazioni);
                    listaPrenotazioneElaborazioni = richiestaBonus.DatiPrenotazioneElaborazioni;

                    GestioneQuadri.DatiQuadroRichiestaBonus quadroRichiestaBonus = null;
                    GestioneQuadri.GetQuadroRichiestaBonusByDatiPensione(datiPensione, out quadroRichiestaBonus);
                    quadroRichiestaBonus.TabEsitoPrenotazione = 0;
                    GestioneQuadri.SalvaQuadroRichiestaBonus(datiPensione.Id, quadroRichiestaBonus);

                    datiPensione.StatoPensione = (int)Utility.StatoPensione.Calcolata;
                    GestionePensione.SalvaPensione(datiPensione);

                    statoPensione = Utility.GetDescription(Utility.StatoPensione.Calcolata);
                }

                //SCRIWO
                GestioneWSSCRIWO.AggiornaStatoLavorazione(datiPensione, statoPrecedente, string.Empty, 0);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nell'aggiornamento Booking";
            }
            return Esito;
        }
        #endregion AreaAggiornamentoBooking

        #region AreaSbloccoDomanda
        public AreaEsito SbloccoDomanda(long numeroDomanda, Utility.TipoAppartenenza tipoAppRuolo, short sedeOperatore, short centroOperativoOperatore, out string sedeDiversa)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            AreaEsito Esito = new AreaEsito();
            sedeDiversa = string.Empty;
            try
            {
                string messaggioVideo = string.Empty;
                if (!GestioneAreaSbloccoDomanda.SbloccoDomandaByDatiPensione(datiPensione, tipoAppRuolo, sedeOperatore, centroOperativoOperatore, out sedeDiversa, out messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                }
                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = "Sblocco domanda eseguito correttamente";
                }
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nello sblocco della domanda";
            }
            return Esito;
        }
        #endregion AreaSbloccoDomanda

        #region AreaRiassegnazioneDomanda
        public AreaEsito RiassegnazioneDomanda(ref AreaRiassegnazioneDomanda areaInputRiassegnazioneDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaInputRiassegnazioneDomanda.NumeroDomanda, null);
            AreaEsito Esito = new AreaEsito();
            try
            {
                string messaggioVideo = string.Empty;
                GestioneAreaRiassegnazioneDomanda.DatiRiassegnazioneDomanda datiRiassegnazioneDomanda = new GestioneAreaRiassegnazioneDomanda.DatiRiassegnazioneDomanda();

                Utility.ValorizzaOggetti(areaInputRiassegnazioneDomanda, datiRiassegnazioneDomanda);
                GestioneAreaRiassegnazioneDomanda.RicercaDomanda(ref datiRiassegnazioneDomanda, datiPensione, out messaggioVideo);

                if (!String.IsNullOrEmpty(messaggioVideo))
                {
                    if (!string.IsNullOrEmpty(datiRiassegnazioneDomanda.SedeDiversa))
                    {
                        Utility.ValorizzaOggetti(datiRiassegnazioneDomanda, areaInputRiassegnazioneDomanda);
                    }
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                    return Esito;
                }

                if (datiRiassegnazioneDomanda.TipoOperazione == Utility.TipoOperazione.UPDATE)
                {
                    GestioneAreaRiassegnazioneDomanda.AggiornaDomanda(datiRiassegnazioneDomanda.NuovaMatricola, datiPensione, out datiRiassegnazioneDomanda);
                }

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;

                Utility.ValorizzaOggetti(datiRiassegnazioneDomanda, areaInputRiassegnazioneDomanda);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nella riassegnazione della domanda";
            }

            return Esito;
        }
        #endregion AreaRiassegnazioneDomanda

        #region AreaTipologieNonAbilitate

        public AreaEsito GetAllTipologieNonAbilitate(Utility.TipoAppartenenza tipoAppRuolo, out AreaTipologieNonAbilitate areaTipologieNonAbilitate)
        {
            SetCulture();

            areaTipologieNonAbilitate = new AreaTipologieNonAbilitate();
            AreaEsito Esito = new AreaEsito();
            try
            {
                List<GestioneTipologieNonAbilitate.DatiTipologieNonAbilitate> elencoTipologieNonAbilitate = null;
                GestioneAreaTipologieNonAbilitate.GetAllTipologieNonAbilitate(out elencoTipologieNonAbilitate);

                if (elencoTipologieNonAbilitate != null && elencoTipologieNonAbilitate.Count > 0)
                {
                    elencoTipologieNonAbilitate = elencoTipologieNonAbilitate.FindAll(x => x.TipoApp.Trim() == tipoAppRuolo.ToString().Trim());
                    elencoTipologieNonAbilitate.ForEach(x =>
                    {
                        x.TipoApp = x.TipoApp.Trim();
                        x.Fondo = !string.IsNullOrEmpty(x.Fondo) ? x.Fondo.Trim() : x.Fondo;
                        x.Gruppo = x.Gruppo.Trim();
                        x.Prodotto = x.Prodotto.Trim();
                        x.Tipo = x.Tipo.Trim();
                        x.Filtro = x.Filtro.Trim();
                        x.SiglaCategoria = x.SiglaCategoria.Trim();
                    });
                    elencoTipologieNonAbilitate = elencoTipologieNonAbilitate.OrderBy(x => x.TipoApp)
                                                                            .ThenBy(x => x.Fondo)
                                                                            .ThenBy(x => x.Gruppo)
                                                                            .ThenBy(x => x.Prodotto)
                                                                            .ThenBy(x => x.Tipo)
                                                                            .ThenBy(x => x.Filtro)
                                                                            .ThenBy(x => x.SiglaCategoria)
                                                                        .ToList();
                    foreach (GestioneTipologieNonAbilitate.DatiTipologieNonAbilitate tipNonAbil in elencoTipologieNonAbilitate)
                        areaTipologieNonAbilitate.ElencoTipologieNonAbilitate.Add(new AreaTipologieNonAbilitate.TipologieNonAbilitate(tipNonAbil));
                }

                List<GestioneTipologieNonAbilitate.Gruppo> elencoGruppo = null;
                GestioneAreaTipologieNonAbilitate.GetListaGruppo(out elencoGruppo);

                List<GestioneTipologieNonAbilitate.Prodotto> elencoProdotto = null;
                GestioneAreaTipologieNonAbilitate.GetListaProdotto(out elencoProdotto);

                List<GestioneTipologieNonAbilitate.Tipo> elencoTipo = null;
                GestioneAreaTipologieNonAbilitate.GetListaTipo(out elencoTipo);

                List<GestioneTipologieNonAbilitate.Filtro> elencoFiltro = null;
                GestioneAreaTipologieNonAbilitate.GetListaFiltro(out elencoFiltro);

                if (elencoGruppo != null && elencoGruppo.Count > 0)
                {
                    if (areaTipologieNonAbilitate.ElencoGruppo == null)
                        areaTipologieNonAbilitate.ElencoGruppo = new List<GestioneTipologieNonAbilitate.Gruppo>();

                    areaTipologieNonAbilitate.ElencoGruppo = elencoGruppo;
                }

                if (elencoProdotto != null && elencoProdotto.Count > 0)
                {
                    if (areaTipologieNonAbilitate.ElencoProdotto == null)
                        areaTipologieNonAbilitate.ElencoProdotto = new List<GestioneTipologieNonAbilitate.Prodotto>();

                    areaTipologieNonAbilitate.ElencoProdotto = elencoProdotto;
                }

                if (elencoTipo != null && elencoTipo.Count > 0)
                {
                    if (areaTipologieNonAbilitate.ElencoTipo == null)
                        areaTipologieNonAbilitate.ElencoTipo = new List<GestioneTipologieNonAbilitate.Tipo>();

                    areaTipologieNonAbilitate.ElencoTipo = elencoTipo;
                }

                if (elencoFiltro != null && elencoFiltro.Count > 0)
                {
                    if (areaTipologieNonAbilitate.ElencoFiltro == null)
                        areaTipologieNonAbilitate.ElencoFiltro = new List<GestioneTipologieNonAbilitate.Filtro>();

                    areaTipologieNonAbilitate.ElencoFiltro = elencoFiltro;
                }

            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nel recupero dell'elenco delle tipologie non abilitate. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito StoreTipologieNonAbilitate(AreaTipologieNonAbilitate.TipologieNonAbilitate datiTipologieNonAbilitate)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            try
            {
                string messaggioVideo = string.Empty;

                //Commentato in quanto il controllo viene effettuato in ControlTipologieNonAbilitate
                //if (datiTipologieNonAbilitate == null)
                //    throw new INPS.DNA.DnaValidationException("Nessuna tipologia non abilitata da salvare");

                GestioneTipologieNonAbilitate.DatiTipologieNonAbilitate tipologieNonAbilitate = new GestioneTipologieNonAbilitate.DatiTipologieNonAbilitate();
                Utility.ValorizzaOggetti(datiTipologieNonAbilitate, tipologieNonAbilitate);

                GestioneAreaTipologieNonAbilitate.StoreTipologieNonAbilitate(tipologieNonAbilitate, out messaggioVideo);

                if (!String.IsNullOrEmpty(messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                    return Esito;
                }
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                return Esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nel salvataggio. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito DeleteTipologieNonAbilitate(AreaTipologieNonAbilitate.TipologieNonAbilitate datiTipologieNonAbilitate)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            try
            {
                if (datiTipologieNonAbilitate == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna tipologia non abilitata da eliminare");

                GestioneTipologieNonAbilitate.DatiTipologieNonAbilitate tipologieNonAbilitate = new GestioneTipologieNonAbilitate.DatiTipologieNonAbilitate();
                Utility.ValorizzaOggetti(datiTipologieNonAbilitate, tipologieNonAbilitate);
                GestioneAreaTipologieNonAbilitate.DeleteTipologieNonAbilitate(tipologieNonAbilitate);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                return Esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nell'eliminazione. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        #endregion AreaTipologieNonAbilitate

        #region AreaInvioSegnalazione
        public AreaEsito InvioSegnalazione(AreaInvioSegnalazione areaInvioSegnalazione)
        {
            SetCulture();

            AreaEsito esito = InvioSegnalazionePrivate(areaInvioSegnalazione.Segnalazione);
            return esito;
        }
        #endregion AreaInvioSegnalazione

        #region AreaByPassCancellazione
        public AreaEsito SbloccoCancellazione(AreaSbloccoCancellazione areaSbloccaCancellazione)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            string messaggioVideo = string.Empty;

            try
            {
                if (!GestioneAreaSbloccoCancellazione.SbloccoCancellazioneDomanda(areaSbloccaCancellazione.NumeroDomanda, areaSbloccaCancellazione.CodiceSede,
                    areaSbloccaCancellazione.CentroOperativo, areaSbloccaCancellazione.SiglaCategoria, areaSbloccaCancellazione.TipoOperazione, out messaggioVideo))
                {
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    esito.Messaggio = messaggioVideo;
                    return esito;
                }

                esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                esito.Messaggio = string.Empty;

            }
            catch (Exception)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore tecnico nell'inserimento delle informazioni riguardanti la domanda";
            }

            return esito;
        }

        #endregion AreaByPassCancellazione

        #region Area Avvisi
        #region public methods
        public AreaEsito GetAvvisi(Utility.TipoAppartenenza? tipoApp, out AreaAvvisi areaAvvisi)
        {
            SetCulture();

            areaAvvisi = null;
            AreaEsito areaEsito = new AreaEsito();
            areaEsito = GetAvvisiPrivate(false, tipoApp, out areaAvvisi);

            return areaEsito;
        }

        public AreaEsito SalvaAvviso(ref AreaAvvisi areaAvvisi)
        {
            SetCulture();

            AreaEsito areaEsito = new AreaEsito();
            if (areaAvvisi == null || areaAvvisi.ElencoAvvisi == null || areaAvvisi.ElencoAvvisi.Count == 0)
            {
                areaEsito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                areaEsito.Messaggio = "Nessun avviso da salvare";
                return areaEsito;
            }

            areaEsito = SalvaAvvisoPrivate(ref areaAvvisi);

            return areaEsito;
        }

        public AreaEsito DeleteAvviso(Utility.TipoAppartenenza? tipoApp, ref AreaAvvisi areaAvvisi)
        {
            SetCulture();

            AreaEsito areaEsito = new AreaEsito();
            if (areaAvvisi == null || areaAvvisi.ElencoAvvisi == null || areaAvvisi.ElencoAvvisi.Count == 0)
            {
                areaEsito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                areaEsito.Messaggio = "Nessun avviso da eliminare";
                return areaEsito;
            }

            areaEsito = DeleteAvvisoPrivate(tipoApp, ref areaAvvisi);

            return areaEsito;
        }
        #endregion public methods

        #region private methods
        private AreaEsito GetAvvisiPrivate(bool recuperaAttivi, Utility.TipoAppartenenza? tipoApp, out AreaAvvisi areaAvvisi)
        {
            areaAvvisi = new AreaAvvisi();
            AreaEsito esito = new AreaEsito();
            try
            {
                List<Avvisi> elencoAvvisi = null;
                GestioneAvvisi.GetAvvisi(recuperaAttivi, tipoApp.GetValueOrDefault().ToString(), out elencoAvvisi);
                areaAvvisi.ElencoAvvisi = elencoAvvisi;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        private AreaEsito SalvaAvvisoPrivate(ref AreaAvvisi areaAvvisi)
        {
            AreaEsito esito = new AreaEsito();
            try
            {
                GestioneAvvisi.StoreAvviso(areaAvvisi.ElencoAvvisi[0]);

                List<Avvisi> elencoAvvisi = null;
                GestioneAvvisi.GetAvvisi(false, areaAvvisi.ElencoAvvisi[0].Tipologia, out elencoAvvisi);
                areaAvvisi.ElencoAvvisi = elencoAvvisi;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        private AreaEsito DeleteAvvisoPrivate(Utility.TipoAppartenenza? tipoApp, ref AreaAvvisi areaAvvisi)
        {
            AreaEsito esito = new AreaEsito();
            try
            {
                GestioneAvvisi.DeleteAvviso(areaAvvisi.ElencoAvvisi[0]);

                List<Avvisi> elencoAvvisi = null;
                GestioneAvvisi.GetAvvisi(false, tipoApp.GetValueOrDefault().ToString(), out elencoAvvisi);
                areaAvvisi.ElencoAvvisi = elencoAvvisi;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }
        #endregion private methods
        #endregion Area Avvisi

        #region Area Messaggi
        #region public methods
        public AreaEsito GetMessaggiHermes(Utility.TipoAppartenenza? tipoApp, out AreaMessaggiHermes areaMessaggiHermes)
        {
            SetCulture();

            areaMessaggiHermes = null;
            AreaEsito areaEsito = new AreaEsito();
            areaEsito = GetMessaggiHermesPrivate(false, tipoApp, out areaMessaggiHermes);

            return areaEsito;
        }

        public AreaEsito SalvaMessaggioHermes(ref AreaMessaggiHermes areaMessaggiHermes)
        {
            SetCulture();

            AreaEsito areaEsito = new AreaEsito();
            if (areaMessaggiHermes == null || areaMessaggiHermes.ElencoMessaggiHermes == null || areaMessaggiHermes.ElencoMessaggiHermes.Count == 0)
            {
                areaEsito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                areaEsito.Messaggio = "Nessun messaggio Hermes da salvare";
                return areaEsito;
            }

            areaEsito = SalvaMessaggioHermesPrivate(ref areaMessaggiHermes);

            return areaEsito;
        }

        public AreaEsito DeleteMessaggioHermes(Utility.TipoAppartenenza? tipoApp, ref AreaMessaggiHermes areaMessaggiHermes)
        {
            SetCulture();

            AreaEsito areaEsito = new AreaEsito();
            if (areaMessaggiHermes == null || areaMessaggiHermes.ElencoMessaggiHermes == null || areaMessaggiHermes.ElencoMessaggiHermes.Count == 0)
            {
                areaEsito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                areaEsito.Messaggio = "Nessun messaggio Hermes da eliminare";
                return areaEsito;
            }

            areaEsito = DeleteMessaggioHermesPrivate(tipoApp, ref areaMessaggiHermes);

            return areaEsito;
        }
        #endregion public methods

        #region private methods
        private AreaEsito GetMessaggiHermesPrivate(bool recuperaAttivi, Utility.TipoAppartenenza? tipoApp, out AreaMessaggiHermes areaMessaggiHermes)
        {
            areaMessaggiHermes = new AreaMessaggiHermes();
            AreaEsito esito = new AreaEsito();
            try
            {
                List<MessaggiHermes> elencoMessaggiHermes = null;
                GestioneMessaggiHermes.GetMessaggiHermes(recuperaAttivi, tipoApp.GetValueOrDefault().ToString(), out elencoMessaggiHermes);
                areaMessaggiHermes.ElencoMessaggiHermes = elencoMessaggiHermes;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        private AreaEsito SalvaMessaggioHermesPrivate(ref AreaMessaggiHermes areaMessaggiHermes)
        {
            AreaEsito esito = new AreaEsito();
            try
            {
                GestioneMessaggiHermes.StoreMessaggioHermes(areaMessaggiHermes.ElencoMessaggiHermes[0]);

                List<MessaggiHermes> elencoMessaggiHermes = null;
                GestioneMessaggiHermes.GetMessaggiHermes(false, areaMessaggiHermes.ElencoMessaggiHermes[0].Tipologia, out elencoMessaggiHermes);
                areaMessaggiHermes.ElencoMessaggiHermes = elencoMessaggiHermes;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        private AreaEsito DeleteMessaggioHermesPrivate(Utility.TipoAppartenenza? tipoApp, ref AreaMessaggiHermes areaMessaggiHermes)
        {
            AreaEsito esito = new AreaEsito();
            try
            {
                GestioneMessaggiHermes.DeleteMessaggioHermes(areaMessaggiHermes.ElencoMessaggiHermes[0]);

                List<MessaggiHermes> elencoMessaggiHermes = null;
                GestioneMessaggiHermes.GetMessaggiHermes(false, tipoApp.GetValueOrDefault().ToString(), out elencoMessaggiHermes);
                areaMessaggiHermes.ElencoMessaggiHermes = elencoMessaggiHermes;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }
        #endregion private methods
        #endregion Area Messaggi

        #region Gestione AvvisiMessaggi
        #region public members
        public AreaEsito GetAreaHomepage(Utility.TipoAppartenenza? tipoApp, out AreaHomepage areaHomepage)
        {
            SetCulture();

            areaHomepage = null;
            AreaEsito areaEsitoWCF = new AreaEsito();
            areaEsitoWCF = GetAreaAvvisiMessaggiPrivate(tipoApp, out areaHomepage);

            return areaEsitoWCF;
        }
        #endregion public members

        #region private members
        private AreaEsito GetAreaAvvisiMessaggiPrivate(Utility.TipoAppartenenza? tipoApp, out AreaHomepage areaAvvisiMessaggi)
        {
            areaAvvisiMessaggi = new AreaHomepage();
            AreaEsito esito = new AreaEsito();
            try
            {
                AreaAvvisi areaAvvisi = new AreaAvvisi();
                GetAvvisiPrivate(true, tipoApp.GetValueOrDefault(), out areaAvvisi);
                areaAvvisiMessaggi.AreaAvvisi = areaAvvisi;

                AreaMessaggiHermes areaMessaggiHermes = new AreaMessaggiHermes();
                GetMessaggiHermesPrivate(true, tipoApp.GetValueOrDefault(), out areaMessaggiHermes);
                areaAvvisiMessaggi.AreaMessaggiHermes = areaMessaggiHermes;

                AreaAggiornamenti areaAggiornamenti = new AreaAggiornamenti();
                GetAggiornamentiPrivate(true, tipoApp.GetValueOrDefault(), out areaAggiornamenti);
                areaAvvisiMessaggi.AreaAggiornamenti = areaAggiornamenti;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }
        #endregion private members
        #endregion Gestione AvvisiMessaggi

        #region Area Eliminazione
        public AreaEsito GetEliminazioneByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaEliminazione areaEliminazione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            AreaEsito esito = new AreaEsito();
            areaEliminazione = null;

            bool isRiaperturaDomanda = contenitore.IsRiaperturaDomanda;

            GestionePensione.DatiEliminazione datiEliminazione = null;
            GestioneAreaEliminazione.GetDatiEliminazioneByIdPensione(ref contenitore, out datiEliminazione);

            GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = null;
            GestioneDatiStoricoGP.GetDatiStoricoGPByIdPensione(contenitore.DatiPensione.Id, out datiStoricoGP);

            if (datiEliminazione != null)
            {
                areaEliminazione = new AreaEliminazione();
                areaEliminazione.DatiEliminazione = datiEliminazione;
            }

            if (datiStoricoGP != null)
            {
                if (areaEliminazione == null)
                    areaEliminazione = new AreaEliminazione();

                areaEliminazione.DataFineCalcoloArretratiStorico = datiStoricoGP.DataFineCalcoloArretrati;
            }

            List<GestioneAreaEliminazione.CodiceEliminazione> listaCodiceEliminazione = null;
            GestioneAreaEliminazione.GetListaCodiceEliminazione(Utility.GetTipoAppartenenza(contenitore.DatiPensione.IndConvInt, contenitore.DatiPensione.Gestione), contenitore.DatiPensione, contenitore.IsRiaperturaDomanda, out listaCodiceEliminazione);
            if (listaCodiceEliminazione != null && listaCodiceEliminazione.Count > 0)
            {
                if (areaEliminazione == null)
                    areaEliminazione = new AreaEliminazione();
                areaEliminazione.ListaCodiceEliminazione = listaCodiceEliminazione;
            }

            if (areaEliminazione == null)
                areaEliminazione = new AreaEliminazione();
            GetCrossProperties(contenitore.DatiPensione, isRiaperturaDomanda, ref areaEliminazione);

            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = "";
            return esito;
        }

        public AreaEsito SalvaDatiEliminazioneByDomanda(long numeroDomanda, AreaEliminazione areaEliminazione)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            GestioneNuoveLiquidate.NuoveLiquidate nuoveLiquidate = null;
            GestionePagamento.DatiPagamento datiPagamento = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;

            GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(datiPensione.Id, out nuoveLiquidate);
            GestionePagamento.GetPagamentoByIdPensione(datiPensione.Id, out datiPagamento);
            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiPensioniDatiGenerici);

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(tipoAppartenenza);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            Esito = StoreDatiEliminazionePrivate(numeroDomanda, datiPensione, nuoveLiquidate, datiPagamento, datiIstruttoria, datiPensioniDatiGenerici, dataSistema, isRiaperturaDomanda, areaEliminazione);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            return Esito;
        }

        public AreaEsito StoreDatiEliminazione(long numeroDomanda, AreaEliminazione areaEliminazione)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            GestioneNuoveLiquidate.NuoveLiquidate nuoveLiquidate = null;
            GestionePagamento.DatiPagamento datiPagamento = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;
            GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(datiPensione.Id, out nuoveLiquidate);
            GestionePagamento.GetPagamentoByIdPensione(datiPensione.Id, out datiPagamento);
            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiPensioniDatiGenerici);

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(tipoAppartenenza);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            Esito = StoreDatiEliminazionePrivate(numeroDomanda, datiPensione, nuoveLiquidate, datiPagamento, datiIstruttoria, datiPensioniDatiGenerici, dataSistema, isRiaperturaDomanda, areaEliminazione);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            return Esito;
        }

        private AreaEsito StoreDatiEliminazionePrivate(long numeroDomanda, GestionePensione.DatiPensione datiPensione, GestioneNuoveLiquidate.NuoveLiquidate nuoveLiquidate,
            GestionePagamento.DatiPagamento datiPagamento, GestioneIstruttoria.DatiIstruttoria datiIstruttoria, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici,
            DateTime dataSistema, bool isRiaperturaDomanda, AreaEliminazione areaEliminazione)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioVideo = string.Empty;

            if (areaEliminazione != null)
            {
                //ENG - Spacchettate SOPGI
                BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa = null;
                BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);

                if (!GestioneAreaEliminazione.ControlsDatiEliminazione(areaEliminazione.DatiEliminazione, datiPensione, datiPensioniDatiGenerici, nuoveLiquidate != null ? nuoveLiquidate.FlagProvvisoria : null,
                    datiPagamento != null ? datiPagamento.DataRinunciaTrattenutaInpdap : null, datiIstruttoria != null ? datiIstruttoria.ScadenzaRevisioneSanitaria : null,
                    datiIstruttoria != null ? datiIstruttoria.NRiconoscimentiInvalidita : null, dataSistema, isRiaperturaDomanda, danteCausa, out messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                    return Esito;
                }
                GestioneAreaEliminazione.StoreDatiEliminazione(datiPensione, areaEliminazione.DatiEliminazione);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }

            return Esito;
        }

        public AreaEsito DeleteDatiEliminazione(long numeroDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            GestionePensione.DatiTitolare datiTitolare = null;
            GestionePensione.GetTitolareByIdPensione(datiPensione.Id, out datiTitolare);
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria;
            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiPensioniDatiGenerici);
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(tipoAppartenenza);
            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            AreaEsito Esito = new AreaEsito();

            GestioneAreaEliminazione.DeleteDatiEliminazione(datiPensione, datiTitolare, datiIstruttoria, datiPensioniDatiGenerici, dataSistema, isRiaperturaDomanda);

            return Esito;
        }

        private void GetCrossProperties(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, ref AreaEliminazione areaEliminazione)
        {
            DateTime? dataFineCalcoloArretratiCalcolata = null;
            Dictionary<string, bool> lCrossProperties = GestioneAreaEliminazione.GetCrossProperties(datiPensione, areaEliminazione.DatiEliminazione, isRiaperturaDomanda, out dataFineCalcoloArretratiCalcolata);

            areaEliminazione.DataFineCalcoloArretratiCalcolata = dataFineCalcoloArretratiCalcolata;
            GestioneControlliDinamici.ControlloDinamico controlloDinamico = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo102", out controlloDinamico);
            areaEliminazione.IsMemo102Abilitato = controlloDinamico != null && controlloDinamico.ValoreControllo == "SI" ? true : false;
        }

        #endregion Area Eliminazione

        #region AreaLavorazioneManualeAutomatiche

        public AreaEsito GetAllPensioniLavorazioneManualeAutomatiche(Utility.TipoAppartenenza tipoApp, out AreaLavorazioneManualeAutomatiche areaLavorazioneManualeAutomatiche)
        {
            SetCulture();

            areaLavorazioneManualeAutomatiche = new AreaLavorazioneManualeAutomatiche();
            AreaEsito Esito = new AreaEsito();
            try
            {
                areaLavorazioneManualeAutomatiche.ListLavorazioneManualeAutomatiche = new List<AreaLavorazioneManualeAutomatiche.LavorazioneManualeAutomatiche>();
                List<GestioneLavorazioneManualeAutomatiche.DatiLavorazioneManualeAutomatiche> elencoGestioneLavorazioneManualeAutomatiche = null;
                GestioneAreaLavorazioneManualeAutomatiche.GetAllPensioniLavorazioneManualeAutomatiche(tipoApp.ToString(), out elencoGestioneLavorazioneManualeAutomatiche);
                if (elencoGestioneLavorazioneManualeAutomatiche != null && elencoGestioneLavorazioneManualeAutomatiche.Count > 0)
                {
                    foreach (var objBl in elencoGestioneLavorazioneManualeAutomatiche)
                    {
                        AreaLavorazioneManualeAutomatiche.LavorazioneManualeAutomatiche objArea = new AreaLavorazioneManualeAutomatiche.LavorazioneManualeAutomatiche();
                        Utility.ValorizzaOggetti(objBl, objArea);
                        areaLavorazioneManualeAutomatiche.ListLavorazioneManualeAutomatiche.Add(objArea);
                    }
                }
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nel recupero dell'elenco delle Pensioni di Tipo Automatico, con Lavorazione Manuale. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito GetAllPensioniLavorazioneManualeAutomaticheByCodiceSede(string utente, Utility.TipoAppartenenza tipoApp, List<Int16> codSede, out AreaLavorazioneManualeAutomatiche areaLavorazioneManualeAutomatiche)
        {
            SetCulture();

            areaLavorazioneManualeAutomatiche = new AreaLavorazioneManualeAutomatiche();
            AreaEsito Esito = new AreaEsito();
            try
            {
                areaLavorazioneManualeAutomatiche.ListLavorazioneManualeAutomatiche = new List<AreaLavorazioneManualeAutomatiche.LavorazioneManualeAutomatiche>();
                List<GestioneLavorazioneManualeAutomatiche.DatiLavorazioneManualeAutomatiche> elencoGestioneLavorazioneManualeAutomatiche = null;
                GestioneAreaLavorazioneManualeAutomatiche.GetAllPensioniLavorazioneManualeAutomaticheByCodiceSede(utente, tipoApp.ToString(), codSede, out elencoGestioneLavorazioneManualeAutomatiche);
                if (elencoGestioneLavorazioneManualeAutomatiche != null && elencoGestioneLavorazioneManualeAutomatiche.Count > 0)
                {
                    foreach (var objBl in elencoGestioneLavorazioneManualeAutomatiche)
                    {
                        AreaLavorazioneManualeAutomatiche.LavorazioneManualeAutomatiche objArea = new AreaLavorazioneManualeAutomatiche.LavorazioneManualeAutomatiche();
                        Utility.ValorizzaOggetti(objBl, objArea);
                        areaLavorazioneManualeAutomatiche.ListLavorazioneManualeAutomatiche.Add(objArea);
                    }
                }
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nel recupero dell'elenco delle Pensioni di Tipo Automatico, con Lavorazione Manuale. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito StoreLavorazioneManualeAutomatiche(AreaLavorazioneManualeAutomatiche.LavorazioneManualeAutomatiche lavorazioneManualeAutomatiche)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            try
            {
                string messaggio = string.Empty;
                if (lavorazioneManualeAutomatiche == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Lavorazione Manuale Automatica da salvare");

                GestioneLavorazioneManualeAutomatiche.DatiLavorazioneManualeAutomatiche datiLavorazioneManualeAutomatiche = new GestioneLavorazioneManualeAutomatiche.DatiLavorazioneManualeAutomatiche();
                Utility.ValorizzaOggetti(lavorazioneManualeAutomatiche, datiLavorazioneManualeAutomatiche);
                GestioneAreaLavorazioneManualeAutomatiche.StoreLavorazioneManualeAutomatiche(datiLavorazioneManualeAutomatiche, out messaggio);
                if (messaggio != string.Empty)
                {
                    //INPS.DNA.Logging.Logger.LogException(Ex);
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggio;
                    return Esito;
                }

            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                return Esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nel salvataggio. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        #endregion AreaLavorazioneManualeAutomatiche

        #region AreaBypassControllo

        public AreaEsito GetAllBypassControllo(Utility.TipoAppartenenza tipoApp, out AreaBypassControllo areaBypassControllo)
        {
            SetCulture();

            areaBypassControllo = new AreaBypassControllo();
            AreaEsito Esito = new AreaEsito();
            try
            {
                //bypass controllo
                areaBypassControllo.ListBypassControllo = new List<AreaBypassControllo.BypassControllo>();
                List<GestioneBypassControllo.DatiBypassControllo> elencoGestioneBypassControllo = null;
                GestioneAreaBypassControllo.GetAllBypassControlloByTipoApp(tipoApp.ToString(), out elencoGestioneBypassControllo);
                if (elencoGestioneBypassControllo != null && elencoGestioneBypassControllo.Count > 0)
                {
                    foreach (var objBl in elencoGestioneBypassControllo)
                    {
                        AreaBypassControllo.BypassControllo objArea = new AreaBypassControllo.BypassControllo();
                        Utility.ValorizzaOggetti(objBl, objArea);
                        areaBypassControllo.ListBypassControllo.Add(objArea);
                    }
                }
                //decodifica
                areaBypassControllo.ListDecBypassControllo = new List<AreaBypassControllo.DecBypassControllo>();
                List<GestioneBypassControllo.DatiDecBypassControllo> elencoDecGestioneBypassControllo = null;
                GestioneAreaBypassControllo.GetDecBypassControlloByTipoApp(tipoApp.ToString(), out elencoDecGestioneBypassControllo);
                if (elencoDecGestioneBypassControllo != null && elencoDecGestioneBypassControllo.Count > 0)
                {
                    foreach (var objBl in elencoDecGestioneBypassControllo)
                    {
                        AreaBypassControllo.DecBypassControllo objArea = new AreaBypassControllo.DecBypassControllo();
                        Utility.ValorizzaOggetti(objBl, objArea);
                        areaBypassControllo.ListDecBypassControllo.Add(objArea);
                    }
                }
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nel recupero dell'elenco dei bypass di controllo. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito DeleteBypassControllo(long idBypassControllo)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            try
            {
                GestioneAreaBypassControllo.DeleteBypassControlloById(idBypassControllo);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                return Esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nell'eliminazione. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito DeleteAllBypassControlloDinamiciByNDomus(long idBypassControllo)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            try
            {
                GestioneAreaBypassControllo.DeleteAllBypassControlloByDomus(idBypassControllo);
            }
            catch (Exception Ex)
            {
                GestioneLogGenerico.SalvaLogGenerico(idBypassControllo, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, Ex != null ? Ex.Message : null, null, Ex != null ? Ex.StackTrace : null);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nell'eliminazione. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito StoreBypassControllo(Utility.TipoAppartenenza tipoApp, AreaBypassControllo.BypassControllo bypassControllo)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            try
            {
                string messaggio = string.Empty;
                //Commentato in quanto il controllo viene effettuato in ControlTipologieNonAbilitate
                if (bypassControllo == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna bypass di controllo da salvare");

                GestioneBypassControllo.DatiBypassControllo datiBypassControllo = new GestioneBypassControllo.DatiBypassControllo();
                Utility.ValorizzaOggetti(bypassControllo, datiBypassControllo);
                GestioneAreaBypassControllo.StoreBypassControllo(datiBypassControllo, tipoApp, out messaggio);
                if (messaggio != string.Empty)
                {
                    //INPS.DNA.Logging.Logger.LogException(Ex);
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggio;
                    return Esito;
                }

            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                return Esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nel salvataggio. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }
        #endregion AreaBypassControllo

        #region AreaDataLimiteINDOCOM

        public AreaEsito SetDataCalcoloDefinitivoINDCOM(Utility.TipoAppartenenza? tipoAppartenenza, AreaStoricoDataLimiteDomandeINDCOM.StoricoDataLimiteDomandeINDCOM storicoDataLimiteINDCOM)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            try
            {
                string messaggio = string.Empty;
                if (storicoDataLimiteINDCOM == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna dato da salvare");

                BLCommon.GestioneControlliDinamici.SetDataCalcoloDefinitivoINDCOM(storicoDataLimiteINDCOM.DataLimiteDomandeINDCOM);

                GestioneStoricoDataLimiteDomandeINDCOM.DatiStoricoDataLimiteDomandeINDCOM datiStorico = new GestioneStoricoDataLimiteDomandeINDCOM.DatiStoricoDataLimiteDomandeINDCOM();
                Utility.ValorizzaOggetti(storicoDataLimiteINDCOM, datiStorico);
                GestioneAreaStoricoDataLimiteDomandeINDCOM.SalvaStorico(datiStorico, out messaggio);
                if (messaggio != string.Empty)
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggio;
                    return Esito;
                }
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                return Esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito GetStoricoDataLimiteINDCOM(out AreaStoricoDataLimiteDomandeINDCOM areaStoricoDataLimiteDomandeINDCOM)
        {
            SetCulture();

            areaStoricoDataLimiteDomandeINDCOM = new AreaStoricoDataLimiteDomandeINDCOM();
            AreaEsito Esito = new AreaEsito();
            try
            {
                areaStoricoDataLimiteDomandeINDCOM.ListStoricoDataLimiteDomandeINDCOM = new List<AreaStoricoDataLimiteDomandeINDCOM.StoricoDataLimiteDomandeINDCOM>();
                List<GestioneStoricoDataLimiteDomandeINDCOM.DatiStoricoDataLimiteDomandeINDCOM> elencoGestioneStoricoDataLimiteINDCOM = null;

                GestioneAreaStoricoDataLimiteDomandeINDCOM.GetStoricoDataLimiteIDCOM(out elencoGestioneStoricoDataLimiteINDCOM);
                if (elencoGestioneStoricoDataLimiteINDCOM != null && elencoGestioneStoricoDataLimiteINDCOM.Count > 0)
                {
                    foreach (var objBl in elencoGestioneStoricoDataLimiteINDCOM)
                    {
                        AreaStoricoDataLimiteDomandeINDCOM.StoricoDataLimiteDomandeINDCOM objArea = new AreaStoricoDataLimiteDomandeINDCOM.StoricoDataLimiteDomandeINDCOM();
                        Utility.ValorizzaOggetti(objBl, objArea);
                        areaStoricoDataLimiteDomandeINDCOM.ListStoricoDataLimiteDomandeINDCOM.Add(objArea);
                    }
                }
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nel recupero dello storico delle modifiche della data limite domande INDOCM. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito UpdateNoteStoricoDataLimiteINDCOM(int id, string note)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();

            try
            {
                string messaggio = string.Empty;
                GestioneAreaStoricoDataLimiteDomandeINDCOM.UpdateNoteStoricoDataLimiteINDCOM(id, note, out messaggio);
                if (messaggio != string.Empty)
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggio;
                    return Esito;
                }
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                return Esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        //public AreaEsito InsertStoricoDataLimiteINDCOM(AreaStoricoDataLimiteDomandeINDCOM.StoricoDataLimiteDomandeINDCOM storicoDataLimiteINDCOM)
        //{
        //    SetCulture();

        //    AreaEsito Esito = new AreaEsito();
        //    try
        //    {
        //        string messaggio = string.Empty;
        //        if (storicoDataLimiteINDCOM == null)
        //            throw new INPS.DNA.DnaValidationException("Nessuna dato da salvare");

        //        GestioneStoricoDataLimiteDomandeINDCOM.DatiStoricoDataLimiteDomandeINDCOM datiStorico = new GestioneStoricoDataLimiteDomandeINDCOM.DatiStoricoDataLimiteDomandeINDCOM();
        //        Utility.ValorizzaOggetti(storicoDataLimiteINDCOM, datiStorico);
        //        GestioneAreaStoricoDataLimiteDomandeINDCOM.SalvaStorico(datiStorico, out messaggio);
        //        if (messaggio != string.Empty)
        //        {
        //            //INPS.DNA.Logging.Logger.LogException(Ex);
        //            Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
        //            Esito.Messaggio = messaggio;
        //            return Esito;
        //        }

        //    }
        //    catch (INPS.DNA.DnaValidationException Ex)
        //    {
        //        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
        //        Esito.Messaggio = Ex.Message;
        //        return Esito;
        //    }
        //    catch (Exception Ex)
        //    {
        //        INPS.DNA.Logging.Logger.LogException(Ex);
        //        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
        //        Esito.Messaggio = "Errore nel salvataggio. Riprovare più tardi";
        //        return Esito;
        //    }

        //    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
        //    Esito.Messaggio = string.Empty;
        //    return Esito;
        //}

        #endregion AreaDataLimiteINDOCOM

        #region AreaDataLimitePoligraficiLetteraB

        public AreaEsito SetDataCalcoloPoligraficiLetteraB(Utility.TipoAppartenenza? tipoAppartenenza, AreaStoricoDataLimitePrepensionementoLetteraB.StoricoDataLimiteDomandePrepensionementoLetteraB storicoDataLimitePoligraficiLetteraB)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            try
            {
                string messaggio = string.Empty;
                if (storicoDataLimitePoligraficiLetteraB == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna dato da salvare");

                BLCommon.GestioneControlliDinamici.SetDataCalcoloPoligraficiLetteraB(storicoDataLimitePoligraficiLetteraB.DataLimitePoligraficiLetteraB);

                GestioneStoricoDataLimitePoligraficiLetteraB.DatiStoricoDataLimitePoligraficiLetteraB datiStorico = new GestioneStoricoDataLimitePoligraficiLetteraB.DatiStoricoDataLimitePoligraficiLetteraB();
                Utility.ValorizzaOggetti(storicoDataLimitePoligraficiLetteraB, datiStorico);
                GestioneAreaStoricoDataLimiteDomandePoligraficiLetteraB.SalvaStorico(datiStorico, out messaggio);
                if (messaggio != string.Empty)
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggio;
                    return Esito;
                }
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                return Esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito GetStoricoDataLimitePoligraficiLetteraB(out AreaStoricoDataLimitePrepensionementoLetteraB areaStoricoDataLimiteDomandePoligraficiLetteraB)
        {
            SetCulture();

            areaStoricoDataLimiteDomandePoligraficiLetteraB = new AreaStoricoDataLimitePrepensionementoLetteraB();
            AreaEsito Esito = new AreaEsito();
            try
            {
                areaStoricoDataLimiteDomandePoligraficiLetteraB.ListStoricoDataLimiteDomandePrepensionementoLetteraB = new List<AreaStoricoDataLimitePrepensionementoLetteraB.StoricoDataLimiteDomandePrepensionementoLetteraB>();
                List<GestioneStoricoDataLimitePoligraficiLetteraB.DatiStoricoDataLimitePoligraficiLetteraB> elencoGestioneStoricoDataLimitePoligraficiLetteraB = null;

                GestioneAreaStoricoDataLimiteDomandePoligraficiLetteraB.GetStoricoDataLimitePoligraficiLetteraB(out elencoGestioneStoricoDataLimitePoligraficiLetteraB);
                if (elencoGestioneStoricoDataLimitePoligraficiLetteraB != null && elencoGestioneStoricoDataLimitePoligraficiLetteraB.Count > 0)
                {
                    foreach (var objBl in elencoGestioneStoricoDataLimitePoligraficiLetteraB)
                    {
                        AreaStoricoDataLimitePrepensionementoLetteraB.StoricoDataLimiteDomandePrepensionementoLetteraB objArea = new AreaStoricoDataLimitePrepensionementoLetteraB.StoricoDataLimiteDomandePrepensionementoLetteraB();
                        Utility.ValorizzaOggetti(objBl, objArea);
                        areaStoricoDataLimiteDomandePoligraficiLetteraB.ListStoricoDataLimiteDomandePrepensionementoLetteraB.Add(objArea);
                    }
                }
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nel recupero dello storico delle modifiche della data limite domande Aziende Editoriali art. 37 legge 416/1981, lettera b). Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito UpdateNoteStoricoDataLimitePoligraficiLetteraB(int id, string note)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();

            try
            {
                string messaggio = string.Empty;
                GestioneAreaStoricoDataLimiteDomandePoligraficiLetteraB.UpdateNoteStoricoDataLimitePoligraficiLetteraB(id, note, out messaggio);
                if (messaggio != string.Empty)
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggio;
                    return Esito;
                }
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                return Esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        #endregion AreaDataLimitePoligraficiLetteraB

        #region AreaControlliDinamici
        public AreaEsito GetDataSistema(Utility.TipoAppartenenza? tipoAppartenenza, out AreaControlliDinamici areaControlliDinamici)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            areaControlliDinamici = new AreaControlliDinamici();
            try
            {
                DateTime? dataSistema = GestioneControlliDinamici.GetDataSistema(tipoAppartenenza);
                areaControlliDinamici.DataSistema = dataSistema;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }

            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return esito;
        }

        public AreaEsito SetDataSistema(Utility.TipoAppartenenza? tipoAppartenenza, AreaControlliDinamici areaControlliDinamici)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                BLCommon.GestioneControlliDinamici.SetDataSistema(tipoAppartenenza, areaControlliDinamici.DataSistema);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }

            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return esito;
        }

        public AreaEsito GetControlloDinamicoByNomeControllo(ref AreaControlliDinamici areaControlliDinamici)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                BLCommon.GestioneControlliDinamici.ControlloDinamico ctrl = null;
                BLCommon.GestioneControlliDinamici.GetControlloDinamicoByNomeControllo(areaControlliDinamici.NomeControllo, out ctrl);
                if (ctrl != null)
                    areaControlliDinamici.ValoreControllo = ctrl.ValoreControllo;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }

            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return esito;
        }

        public AreaEsito GetAnnoCompetenza(Utility.TipoAppartenenza? tipoAppartenenza, out AreaControlliDinamici areaControlliDinamici)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            areaControlliDinamici = new AreaControlliDinamici();
            try
            {
                int annoCompetenza = 0;
                GestioneControlliDinamici.GetAnnoCompetenza(tipoAppartenenza, out annoCompetenza);
                areaControlliDinamici.NomeControllo = "AnnoCompetenza";
                areaControlliDinamici.ValoreControllo = annoCompetenza.ToString();
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }

            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return esito;
        }
        #endregion AreaControlliDinamici

        #region Area FAQ
        #region public methods
        public AreaEsito GetFAQ(Utility.TipoAppartenenza? tipoApp, out AreaFAQ areaFAQ)
        {
            SetCulture();

            areaFAQ = null;
            AreaEsito areaEsito = new AreaEsito();
            areaEsito = GetFAQPrivate(tipoApp, out areaFAQ);

            return areaEsito;
        }

        public AreaEsito SalvaFAQ(ref AreaFAQ areaFAQ)
        {
            SetCulture();

            AreaEsito areaEsito = new AreaEsito();
            if (areaFAQ == null || areaFAQ.ElencoFAQ == null || areaFAQ.ElencoFAQ.Count == 0)
            {
                areaEsito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                areaEsito.Messaggio = "Nessuna FAQ da salvare";
                return areaEsito;
            }

            areaEsito = SalvaFAQPrivate(ref areaFAQ);

            return areaEsito;
        }

        public AreaEsito DeleteFAQ(Utility.TipoAppartenenza? tipoApp, ref AreaFAQ areaFAQ)
        {
            SetCulture();

            AreaEsito areaEsito = new AreaEsito();
            if (areaFAQ == null || areaFAQ.ElencoFAQ == null || areaFAQ.ElencoFAQ.Count == 0)
            {
                areaEsito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                areaEsito.Messaggio = "Nessuna FAQ da eliminare";
                return areaEsito;
            }

            areaEsito = DeleteFAQPrivate(tipoApp, ref areaFAQ);

            return areaEsito;
        }

        public AreaEsito CaricaPdfFaq(Utility.TipoAppartenenza? tipoApp, out AreaFAQ areaFAQ)
        {
            SetCulture();

            areaFAQ = null;
            AreaEsito areaEsito = new AreaEsito();
            areaEsito = CaricaPdfFaqPrivate(tipoApp, out areaFAQ);

            return areaEsito;
        }
        #endregion public methods

        #region private methods
        private AreaEsito GetFAQPrivate(Utility.TipoAppartenenza? tipoApp, out AreaFAQ areaFAQ)
        {
            areaFAQ = new AreaFAQ();
            AreaEsito esito = new AreaEsito();
            try
            {
                List<FAQ> elencoFAQ = null;
                GestioneFAQ.GetFAQ(tipoApp.GetValueOrDefault().ToString(), out elencoFAQ);
                areaFAQ.ElencoFAQ = elencoFAQ;

                List<BLCommon.GestioneDecodifica.TipologiaFAQ> elencoTipologiaFAQ = null;
                GestioneDecodifica.GetTipologiaFAQ(out elencoTipologiaFAQ);
                if (elencoTipologiaFAQ != null && elencoTipologiaFAQ.Count > 0)
                {
                    foreach (BLCommon.GestioneDecodifica.TipologiaFAQ tipologiaFAQDB in elencoTipologiaFAQ)
                    {
                        TipologiaFAQ tipologiaFAQ = new TipologiaFAQ();
                        Utility.ValorizzaOggetti(tipologiaFAQDB, tipologiaFAQ);
                        if (areaFAQ.ElencoTipologiaFAQ == null)
                            areaFAQ.ElencoTipologiaFAQ = new List<TipologiaFAQ>();
                        areaFAQ.ElencoTipologiaFAQ.Add(tipologiaFAQ);
                    }
                }
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        private AreaEsito SalvaFAQPrivate(ref AreaFAQ areaFAQ)
        {
            AreaEsito esito = new AreaEsito();
            try
            {
                GestioneFAQ.StoreFAQ(areaFAQ.ElencoFAQ[0]);

                List<FAQ> elencoFAQ = null;
                GestioneFAQ.GetFAQ(areaFAQ.ElencoFAQ[0].TipoApp, out elencoFAQ);
                areaFAQ.ElencoFAQ = elencoFAQ;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        private AreaEsito DeleteFAQPrivate(Utility.TipoAppartenenza? tipoApp, ref AreaFAQ areaFAQ)
        {
            AreaEsito esito = new AreaEsito();
            try
            {
                GestioneFAQ.DeleteFAQ(areaFAQ.ElencoFAQ[0]);

                List<FAQ> elencoFAQ = null;
                GestioneFAQ.GetFAQ(tipoApp.GetValueOrDefault().ToString(), out elencoFAQ);
                areaFAQ.ElencoFAQ = elencoFAQ;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        private AreaEsito CaricaPdfFaqPrivate(Utility.TipoAppartenenza? tipoApp, out AreaFAQ areaFAQ)
        {
            areaFAQ = new AreaFAQ();
            AreaEsito esito = new AreaEsito();
            try
            {
                MemoryStream memStream = null;
                GestioneStampeFAQ.CaricaPdfFAQ(tipoApp.GetValueOrDefault().ToString(), out memStream);
                areaFAQ.PdfDoc = memStream;
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }
        #endregion private methods
        #endregion Area FAQ

        #region Area Pulizia Domanda
        public AreaEsito GetPuliziaDomandaByDomanda(long numeroDomanda, short sedeOperatore, short centroOperativoOperatore, Utility.TipoAppartenenza tipoAppRuolo, Utility.Ruolo ruolo,
            out AreaPuliziaDomanda areaPuliziaDomanda)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            areaPuliziaDomanda = new AreaPuliziaDomanda();
            Entity.PuliziaDomanda entityPuliziaDomanda = null;
            bool IsPuliziaDisponibile = false;
            string messaggioVideo = string.Empty;
            string sedeDiversa = string.Empty;

            try
            {
                GestionePuliziaDomanda.GetPuliziaDomandaByDomanda(numeroDomanda, sedeOperatore, centroOperativoOperatore, tipoAppRuolo, ruolo, out entityPuliziaDomanda, out sedeDiversa,
                    out IsPuliziaDisponibile, out messaggioVideo);
                areaPuliziaDomanda.EntityPuliziaDomanda = entityPuliziaDomanda;
                areaPuliziaDomanda.IsPuliziaDisponibile = IsPuliziaDisponibile;
                areaPuliziaDomanda.SedeDiversa = sedeDiversa;

                if (!string.IsNullOrEmpty(messaggioVideo))
                {
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    esito.Messaggio = messaggioVideo;
                    return esito;
                }
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return esito;
            }

            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito EseguiPuliziaDomandaByDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, Utility.TipoAppartenenza tipoAppRuolo,
            Utility.Ruolo ruolo, out AreaPuliziaDomanda areaPuliziaDomanda)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            areaPuliziaDomanda = new AreaPuliziaDomanda();
            Entity.PuliziaDomanda entityPuliziaDomanda = null;
            bool IsPuliziaDisponibile = false;
            string messaggioVideo = string.Empty;
            string sedeDiversa = string.Empty;

            try
            {
                string messaggioVideoPulizia = string.Empty;
                GestionePuliziaDomanda.EseguiPuliziaDomandaByDomanda(numeroDomanda, matricolaOperatore, sedeOperatore, centroOperativoOperatore, tipoAppRuolo, out sedeDiversa, out entityPuliziaDomanda, out messaggioVideoPulizia);

                GestionePuliziaDomanda.GetPuliziaDomandaByDomanda(numeroDomanda, sedeOperatore, centroOperativoOperatore, tipoAppRuolo, ruolo, out entityPuliziaDomanda, out sedeDiversa, out IsPuliziaDisponibile,
                    out messaggioVideo);
                areaPuliziaDomanda.EntityPuliziaDomanda = entityPuliziaDomanda;
                areaPuliziaDomanda.IsPuliziaDisponibile = IsPuliziaDisponibile;
                areaPuliziaDomanda.SedeDiversa = sedeDiversa;

                if (!string.IsNullOrEmpty(messaggioVideoPulizia))
                {
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    esito.Messaggio = messaggioVideoPulizia;
                    return esito;
                }

                if (!string.IsNullOrEmpty(messaggioVideo))
                {
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    esito.Messaggio = messaggioVideo;
                    return esito;
                }
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return esito;
            }

            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }
        #endregion Area Pulizia Domanda

        #region private IServizioLiquidazione members

        private bool StoreDatiAnagraficaDC(GestionePensione.DatiPensione datiPensione, AreaDanteCausa areaDanteCausa, GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare, List<GestioneFamiliari.Familiare> listaFamiliari, List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficaFamiliari,
            ref GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC, ref BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausaDB, ref GestioneQuadri.DatiQuadroDanteCausa datiQuadroQuadroDanteCausa,
            bool isRiaperturaDomanda, bool IsSingleTabSaved, out string errore)
        {
            errore = string.Empty;

            errore = GestioneDanteCausa.StoreDatiAnagraficaDC(datiPensione, areaDanteCausa.AnagraficaDC, areaDanteCausa.DatiPensioneDiretta, datiMaggiorazioniBenefici, datiAnagraficiTitolare, listaFamiliari,
                listaAnagraficaFamiliari, ref datiAnagraficiDC, ref datiDanteCausaDB, ref datiQuadroQuadroDanteCausa, isRiaperturaDomanda, IsSingleTabSaved);

            if (String.IsNullOrEmpty(errore))
                return true;
            else
                return false;
        }

        private bool StoreDatiAltraPensione(GestionePensione.DatiPensione datiPensione, AreaDanteCausa areaDanteCausa, GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC, ref BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausaDB,
            ref GestioneQuadri.DatiQuadroDanteCausa datiQuadroQuadroDanteCausa, bool IsSingleTabSaved, out string errore)
        {
            errore = string.Empty;
            errore = GestioneDanteCausa.StoreDatiAltraPensioneByDatiPensione(datiPensione, areaDanteCausa.AltraPensioneDC, areaDanteCausa.AnagraficaDC, areaDanteCausa.DatiPensioneDiretta, datiAnagraficiDC,
                ref datiDanteCausaDB, ref datiQuadroQuadroDanteCausa, IsSingleTabSaved);
            if (String.IsNullOrEmpty(errore))
                return true;
            else
                return false;
        }

        private bool StoreDatiPensioneCI(ref GestionePensione.DatiPensione datiPensione, AreaDanteCausa areaDanteCausa, ref BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa,
            ref GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiAgoCi, ref GestioneQuadri.DatiQuadroDanteCausa datiQuadroQuadroDanteCausa,
            bool IsSingleTabSaved, out string errore)
        {
            errore = string.Empty;
            errore = GestioneDanteCausa.StoreDatiPensioneCI(ref datiPensione, areaDanteCausa.DatiPensioneCI, ref datiDanteCausa, ref datiMaggiorazioniBenefici, datiGenericiAgoCi, ref datiQuadroQuadroDanteCausa, IsSingleTabSaved);
            if (String.IsNullOrEmpty(errore))
                return true;
            else
                return false;
        }

        private bool StoreDatiPensioneDiretta(GestionePensione.DatiPensione datiPensione, AreaDanteCausa areaDanteCausa, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiAgoCi,
            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere, GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria, GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC, List<GestioneFamiliari.Familiare> listaFamiliari, ref BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa,
            ref GestioneQuadri.DatiQuadroDanteCausa datiQuadroQuadroDanteCausa, bool IsSingleTabSaved, out string errore)
        {
            errore = string.Empty;
            errore = GestioneDanteCausa.StoreDatiPensioneDirettaByDatiPensione(datiPensione, areaDanteCausa.DatiPensioneDiretta, areaDanteCausa.AnagraficaDC, areaDanteCausa.DatiPensioneCI, areaDanteCausa.AltraPensioneDC,
                datiGenericiAgoCi, listaPrestazioniEstere, datiMaggiorazioniBenefici, datiIstruttoria, datiAnagraficiDC, listaFamiliari, ref datiDanteCausa, ref datiQuadroQuadroDanteCausa, IsSingleTabSaved);
            if (String.IsNullOrEmpty(errore))
                return true;
            else
                return false;
        }

        private bool StoreDatiRedditiSentenza49593(GestionePensione.DatiPensione datiPensione, AreaDanteCausa areaDanteCausa, GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC,
            ref BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, ref GestioneQuadri.DatiQuadroDanteCausa datiQuadroQuadroDanteCausa, bool IsSingleTabSaved, bool isRiaperturaDomanda, List<GestioneFamiliari.Familiare> listaFamiliari, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenerici, out string errore)
        {
            errore = string.Empty;
            errore = GestioneDanteCausa.StoreDatiRedditiSentenza495_93ByDatiPensione(datiPensione, areaDanteCausa.DatiRedditiSentenza495_93, areaDanteCausa.DatiPensioneDiretta, areaDanteCausa.AnagraficaDC, datiAnagraficiDC,
                ref datiDanteCausa, ref datiQuadroQuadroDanteCausa, IsSingleTabSaved, isRiaperturaDomanda, listaFamiliari, areaDanteCausa.ImportoMensilePensioneEstera, datiGenerici);
            if (String.IsNullOrEmpty(errore))
                return true;
            else
                return false;
        }

        private bool ValorizzaParametriARCA(string matricolaOperatore, out Entity.ParametriARCA parametriArca)
        {
            parametriArca = null;
            try
            {
                parametriArca = new INPS.Pensioni.Liquidazione.Entity.ParametriARCA();
                parametriArca.Applicazione = ConfigurationManager.AppSettings["SvrARCA.APP"];

                //BypassMatricolaARCA: se la matricola è alfanumerica ed è presente il bypass prelevo matricola di bypass
                int matricolaNumerica = 0;
                if (!int.TryParse(matricolaOperatore, out matricolaNumerica) && ConfigurationManager.AppSettings["BypassMatricolaARCA"] != null)
                    parametriArca.Matricola = matricolaOperatore = ConfigurationManager.AppSettings["BypassMatricolaARCA"];
                else
                    parametriArca.Matricola = matricolaOperatore;

                parametriArca.Provenienza = ConfigurationManager.AppSettings["SvrARCA.PROV"];
                parametriArca.Ruolo = ConfigurationManager.AppSettings["SvrARCA.RUOLO"];
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private Entity.Anagrafica ValorizzaAnagrafica(AreaRispostaRiepilogo.DatiRiepilogoAnagrafica datiRiepilogoAnagrafica)
        {
            if (datiRiepilogoAnagrafica == null)
                return null;

            Entity.Anagrafica anagrafica = new INPS.Pensioni.Liquidazione.Entity.Anagrafica();
            anagrafica.CAP = datiRiepilogoAnagrafica.Cap;
            anagrafica.Cell = datiRiepilogoAnagrafica.Cell;
            anagrafica.CodiceDelegato = datiRiepilogoAnagrafica.CodiceDelegato;
            anagrafica.CodiceFiscale = datiRiepilogoAnagrafica.CodiceFiscale;
            anagrafica.CodiceStatoCivile = datiRiepilogoAnagrafica.CodiceStatoCivile;
            anagrafica.Cognome = datiRiepilogoAnagrafica.Cognome;
            anagrafica.ComuneNascita = datiRiepilogoAnagrafica.ComuneNascita;
            anagrafica.ComuneResidenza = datiRiepilogoAnagrafica.ComuneResidenza;
            anagrafica.DataNascita = datiRiepilogoAnagrafica.DataNascita;
            anagrafica.DecorrenzaStatoCivile = datiRiepilogoAnagrafica.DecorrenzaStatoCivile;
            anagrafica.EMail = datiRiepilogoAnagrafica.EMail;
            anagrafica.Indirizzo = datiRiepilogoAnagrafica.Indirizzo;
            anagrafica.NCivico = datiRiepilogoAnagrafica.NumeroCivico;
            anagrafica.Nome = datiRiepilogoAnagrafica.Nome;
            anagrafica.ProvinciaNascita = datiRiepilogoAnagrafica.ProvinciaNascita;
            anagrafica.ProvinciaResidenza = datiRiepilogoAnagrafica.ProvinciaResidenza;
            anagrafica.Sesso = datiRiepilogoAnagrafica.Sesso;
            anagrafica.Tel = datiRiepilogoAnagrafica.Tel;
            anagrafica.IsNatoInItalia = datiRiepilogoAnagrafica.IsNatoInItalia;
            anagrafica.IsResidenteInItalia = datiRiepilogoAnagrafica.IsResidenteInItalia;
            anagrafica.CodiceTutore = datiRiepilogoAnagrafica.CodiceTutore;
            anagrafica.CessValAmmSost = datiRiepilogoAnagrafica.CessValAmmSost;
            anagrafica.MatricolaArca = datiRiepilogoAnagrafica.MatricolaArca;
            anagrafica.CognomeAcquisito = datiRiepilogoAnagrafica.CognomeAcquisito;
            anagrafica.CodiceComuneNascita = datiRiepilogoAnagrafica.CodiceComuneNascita;
            anagrafica.Cittadinanza = datiRiepilogoAnagrafica.Cittadinanza;
            anagrafica.CodiceComuneResidenza = datiRiepilogoAnagrafica.CodiceComuneResidenza;
            anagrafica.FrazioneResidenza = datiRiepilogoAnagrafica.FrazioneResidenza;
            anagrafica.DomicilioEstero = datiRiepilogoAnagrafica.DomicilioEstero;
            anagrafica.ResidenzaEstero = datiRiepilogoAnagrafica.ResidenzaEstero;
            anagrafica.Codice1Arca = datiRiepilogoAnagrafica.Codice1Arca;
            anagrafica.Codice2Arca = datiRiepilogoAnagrafica.Codice2Arca;
            anagrafica.DataMorte = datiRiepilogoAnagrafica.DataMorte;

            return anagrafica;
        }

        private static AreaEsito InvioSegnalazionePrivate(INPS.Pensioni.Liquidazione.Entity.Segnalazione segnalazione)
        {
            AreaEsito areaEsito = new AreaEsito();
            areaEsito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            areaEsito.Messaggio = string.Empty;
            string errori = string.Empty;

            if (!GestioneMail.ControlsDatiMail(segnalazione, out errori))
            {
                areaEsito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                areaEsito.Messaggio = errori;
                return areaEsito;
            }

            if (!GestioneMail.NotificaSegnalazione(segnalazione, out errori))
            {
                areaEsito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                areaEsito.Messaggio = errori;
            }
            return areaEsito;
        }

        #endregion private IServizioLiquidazione members

        #region IDecodifica members
        public AreaDecodifica GetDecodifica()
        {
            SetCulture();

            AreaDecodifica decodifica = null;
            try
            {
                #region Recupero dati decodifica da BL
                List<BLCommon.GestioneDecodifica.StatoCivile> elencoStatiCiviliBL = null;
                BLCommon.GestioneDecodifica.GetStatiCivili(out elencoStatiCiviliBL);

                List<BLCommon.GestioneDecodifica.StatoEstero> elencoStatiEsteriBL = null;
                BLCommon.GestioneDecodifica.GetStatiEsteri(out elencoStatiEsteriBL);

                List<BLCommon.GestioneDecodifica.Provincia> elencoProvinceBL = null;
                BLCommon.GestioneDecodifica.GetProvince(out elencoProvinceBL);

                List<BLCommon.GestioneDecodifica.ConiugeOFiglio> elencoConiugeOFiglioBL = null;
                BLCommon.GestioneDecodifica.GetConiugeOFiglio(out elencoConiugeOFiglioBL);

                List<BLCommon.GestioneDecodifica.DetrazioniReddito> elencoDetrazioniRedditoBL = null;
                BLCommon.GestioneDecodifica.GetDetrazioniReddito(out elencoDetrazioniRedditoBL);

                List<BLCommon.GestioneDecodifica.Tutore> elencoTutoreBL = null;
                BLCommon.GestioneDecodifica.GetTutore(out elencoTutoreBL);

                List<BLCommon.GestioneDecodifica.Delegato> elencoDelegatoBL = null;
                BLCommon.GestioneDecodifica.GetDelegato(out elencoDelegatoBL);

                List<BLCommon.GestioneDecodifica.ModalitaPagamento> elencoModalitaPagamentoBL = null;
                BLCommon.GestioneDecodifica.GetModalitaPagamento(out elencoModalitaPagamentoBL);

                List<BLCommon.GestioneDecodifica.TipoPagamento> elencoTipoPagamentoBL = null;
                BLCommon.GestioneDecodifica.GetTipoPagamento(out elencoTipoPagamentoBL);

                List<BLCommon.GestioneDecodifica.TipoCalcolo> elencoTipoCalcoloBL = null;
                BLCommon.GestioneDecodifica.GetTipoCalcolo(out elencoTipoCalcoloBL);

                List<BLCommon.GestioneDecodifica.ParentelaDC> elencoParentelaDC = null;
                BLCommon.GestioneDecodifica.GetParentelaDC(out elencoParentelaDC);

                List<BLCommon.GestioneDecodifica.CodiceProvenienza> elencoCodiceProvenienza = null;
                BLCommon.GestioneDecodifica.GetCodiceProvenienza(out elencoCodiceProvenienza);

                List<BLCommon.GestioneDecodifica.CodiciVari> elencoCodiciVari = null;
                BLCommon.GestioneDecodifica.GetCodiciVariDC(out elencoCodiciVari);

                List<BLCommon.GestioneDecodifica.CausaCarico> elencoCausaCaricoBL = null;
                BLCommon.GestioneDecodifica.GetCausaCarico(out elencoCausaCaricoBL);

                List<BLCommon.GestioneDecodifica.CodiceCristallizzazione> elencoCodiceCristallizzazioneBL = null;
                BLCommon.GestioneDecodifica.GetCodiceCristallizzazione(out elencoCodiceCristallizzazioneBL);

                List<BLCommon.GestioneDecodifica.TipoPensione> elencoTipoPensioneBL = null;
                BLCommon.GestioneDecodifica.GetTipoPensione(out elencoTipoPensioneBL);

                List<BLCommon.GestioneDecodifica.CodiceAzienda> elencoCodiceAziendaBL = null;
                BLCommon.GestioneDecodifica.GetCodiceAzienda(out elencoCodiceAziendaBL);

                List<BLCommon.GestioneDecodifica.GradoInvalidita> elencoGradoInvaliditaBL = null;
                BLCommon.GestioneDecodifica.GetGradoInvalidita(out elencoGradoInvaliditaBL);

                List<BLCommon.GestioneDecodifica.ProrataEnel> elencoProrataEnelBL = null;
                BLCommon.GestioneDecodifica.GetProrataEnel(out elencoProrataEnelBL);

                List<BLCommon.GestioneDecodifica.ComunicazioneCampi1_2> elencoComunicazioneCampi1_2BL = null;
                BLCommon.GestioneDecodifica.GetComunicazioneCampi1_2(out elencoComunicazioneCampi1_2BL);

                List<BLCommon.GestioneDecodifica.ComunicazioneCampo3> elencoComunicazioneCampo3BL = null;
                BLCommon.GestioneDecodifica.GetComunicazioneCampo3(out elencoComunicazioneCampo3BL);

                List<BLCommon.GestioneDecodifica.ComunicazioneCampo4> elencoComunicazioneCampo4BL = null;
                BLCommon.GestioneDecodifica.GetComunicazioneCampo4(out elencoComunicazioneCampo4BL);

                List<BLCommon.GestioneDecodifica.CodiciNatura> elencoCodiciNaturaBL = null;
                BLCommon.GestioneDecodifica.GetCodiciNatura(out elencoCodiciNaturaBL);

                List<BLCommon.GestioneDecodifica.CategoriaPensione> elencoCategoriePensioneBL = null;
                BLCommon.GestioneDecodifica.GetCategoriePensione(out elencoCategoriePensioneBL);

                List<BLCommon.GestioneDecodifica.FondoPensione> elencoFondiPensioneBL = null;
                BLCommon.GestioneDecodifica.GetFondiPensione(out elencoFondiPensioneBL);

                List<BLCommon.GestioneDecodifica.FondoPensione> elencoCasseGDPBL = null;
                BLCommon.GestioneDecodifica.GetCasseGDP(out elencoCasseGDPBL);

                List<BLCommon.GestioneDecodifica.StatoPensione> elencoStatiPensioneBL = null;
                BLCommon.GestioneDecodifica.GetStatiPensione(out elencoStatiPensioneBL);

                List<BLCommon.GestioneDecodifica.ImportoAltraPensione> elencoImportoAltraPensione = null;
                BLCommon.GestioneDecodifica.GetCodiceImportoAltraPensione(out elencoImportoAltraPensione);

                List<BLCommon.GestioneDecodifica.CodeGestioneCalcoloRetributivo> elencoCodeGestioneCalcoloRetr = null;
                BLCommon.GestioneDecodifica.GetCodeGestioneCalcoloRetributivo(out elencoCodeGestioneCalcoloRetr);

                List<BLCommon.GestioneDecodifica.CodeGestioneCalcoloContributivo> elencoCodeGestioneCalcoloContrib = null;
                BLCommon.GestioneDecodifica.GetCodeGestioneCalcoloContributivo(out elencoCodeGestioneCalcoloContrib);

                List<BLCommon.GestioneDecodifica.Mobilita> elencoCodeMobilitaBL = null;
                BLCommon.GestioneDecodifica.GetCodiceMobilita(out elencoCodeMobilitaBL);

                List<BLCommon.GestioneDecodifica.CodeGestione> elencoCodeGestioneBL = null;
                BLCommon.GestioneDecodifica.GetCodiceGestione(out elencoCodeGestioneBL);

                List<BLCommon.GestioneDecodifica.CtrlRicercaGPT> elencoRicercaGPT = null;
                BLCommon.GestioneDecodifica.GetCtrlRicercaGPT(out elencoRicercaGPT);

                List<BLCommon.GestioneDecodifica.CatEnteAltraPensione> elencoCatEnteAltrePensioni = null;
                BLCommon.GestioneDecodifica.GetCatEnteAltrePensioni(out elencoCatEnteAltrePensioni);

                List<GestioneDecodifica.CtrlEnteCassaCodiceGestione> elencoCtrlEnteCassaCodiceGestione = null;
                BLCommon.GestioneDecodifica.GetCtrlEnteCassaCodiceGestione(out elencoCtrlEnteCassaCodiceGestione);

                List<GestioneDecodifica.CtrlCatAdeguata> elencoCtrlCatAdeguata = null;
                BLCommon.GestioneDecodifica.GetCtrlCatAdeguata(out elencoCtrlCatAdeguata);

                List<GestioneDecodifica.DecComparto> elencoDecComparto = null;
                BLCommon.GestioneDecodifica.GetElencoDecComparto(out elencoDecComparto);

                List<GestioneDecodifica.DecRuolo> elencoDecRuolo = null;
                BLCommon.GestioneDecodifica.GetElencoDecRuolo(out elencoDecRuolo);

                List<GestioneDecodifica.DecSettore> elencoDecSettore = null;
                BLCommon.GestioneDecodifica.GetElencoDecSettore(out elencoDecSettore);

                List<GestioneDecodifica.DecSede> elencoDecSede = null;
                BLCommon.GestioneDecodifica.GetElencoDecSede(out elencoDecSede);
                #endregion Recupero dati decodifica da BL

                #region Valorizzazione dati decodifica
                decodifica = new AreaDecodifica();

                if (elencoCodeGestioneBL != null && elencoCodeGestioneBL.Count > 0)
                {
                    decodifica.ElencoCodiceGestione = new List<AreaDecodifica.CodeGestione>();
                    foreach (BLCommon.GestioneDecodifica.CodeGestione codeGestione in elencoCodeGestioneBL)
                        decodifica.ElencoCodiceGestione.Add(new AreaDecodifica.CodeGestione(codeGestione));
                }

                if (elencoCodeMobilitaBL != null && elencoCodeMobilitaBL.Count > 0)
                {
                    decodifica.ElencoCodeMobilita = new List<AreaDecodifica.DatiCodeMobilita>();
                    foreach (BLCommon.GestioneDecodifica.Mobilita codeGestioneMobilitaBL in elencoCodeMobilitaBL)
                        decodifica.ElencoCodeMobilita.Add(new AreaDecodifica.DatiCodeMobilita(codeGestioneMobilitaBL));
                }

                if (elencoCodeGestioneCalcoloContrib != null && elencoCodeGestioneCalcoloContrib.Count > 0)
                {
                    decodifica.ElencoCodeGestioneCalcoloContrib = new List<AreaDecodifica.DatiCodeGestioneCalcoloContrib>();
                    foreach (BLCommon.GestioneDecodifica.CodeGestioneCalcoloContributivo codeGestioneCalcoloContributivoBL in elencoCodeGestioneCalcoloContrib)
                        decodifica.ElencoCodeGestioneCalcoloContrib.Add(new AreaDecodifica.DatiCodeGestioneCalcoloContrib(codeGestioneCalcoloContributivoBL));
                }

                if (elencoCodeGestioneCalcoloRetr != null && elencoCodeGestioneCalcoloRetr.Count > 0)
                {
                    decodifica.ElencoCodeGestioneCalcoloRetrib = new List<AreaDecodifica.DatiCodeGestioneCalcoloRetrib>();
                    foreach (BLCommon.GestioneDecodifica.CodeGestioneCalcoloRetributivo codeGestioneCalcoloRetributivoBL in elencoCodeGestioneCalcoloRetr)
                        decodifica.ElencoCodeGestioneCalcoloRetrib.Add(new AreaDecodifica.DatiCodeGestioneCalcoloRetrib(codeGestioneCalcoloRetributivoBL));
                }


                if (elencoImportoAltraPensione != null && elencoImportoAltraPensione.Count > 0)
                {
                    decodifica.ElencoCodiciImportoAltraPensione = new List<AreaDecodifica.DatiCodiciImportoAltraPensione>();
                    foreach (BLCommon.GestioneDecodifica.ImportoAltraPensione importoAltraPensioneBL in elencoImportoAltraPensione)
                        decodifica.ElencoCodiciImportoAltraPensione.Add(new AreaDecodifica.DatiCodiciImportoAltraPensione(importoAltraPensioneBL));
                }

                if (elencoCodiceProvenienza != null && elencoCodiceProvenienza.Count > 0)
                {
                    decodifica.ElencoCodiciProvenienza = new List<AreaDecodifica.DatiCodiciProvenienza>();
                    foreach (BLCommon.GestioneDecodifica.CodiceProvenienza provenienzaBL in elencoCodiceProvenienza)
                        decodifica.ElencoCodiciProvenienza.Add(new AreaDecodifica.DatiCodiciProvenienza(provenienzaBL));
                }

                if (elencoStatiCiviliBL != null && elencoStatiCiviliBL.Count > 0)
                {
                    decodifica.ElencoStatiCivili = new List<AreaDecodifica.DatiStatoCivile>();
                    foreach (BLCommon.GestioneDecodifica.StatoCivile statoCivileBL in elencoStatiCiviliBL)
                        decodifica.ElencoStatiCivili.Add(new AreaDecodifica.DatiStatoCivile(statoCivileBL));
                }

                if (elencoStatiEsteriBL != null && elencoStatiEsteriBL.Count > 0)
                {
                    decodifica.ElencoStatiEsteri = new List<AreaDecodifica.DatiStatoEstero>();
                    foreach (BLCommon.GestioneDecodifica.StatoEstero statoEsteroBL in elencoStatiEsteriBL)
                        decodifica.ElencoStatiEsteri.Add(new AreaDecodifica.DatiStatoEstero(statoEsteroBL));
                }

                if (elencoProvinceBL != null && elencoProvinceBL.Count > 0)
                {
                    decodifica.ElencoProvince = new List<AreaDecodifica.DatiProvincia>();
                    foreach (BLCommon.GestioneDecodifica.Provincia provinciaBL in elencoProvinceBL)
                        decodifica.ElencoProvince.Add(new AreaDecodifica.DatiProvincia(provinciaBL));
                }

                if (elencoConiugeOFiglioBL != null && elencoConiugeOFiglioBL.Count > 0)
                {
                    decodifica.ElencoConiugeOFiglio = new List<AreaDecodifica.DatiConiugeOFiglio>();
                    foreach (BLCommon.GestioneDecodifica.ConiugeOFiglio coniugeOFiglioBL in elencoConiugeOFiglioBL)
                        decodifica.ElencoConiugeOFiglio.Add(new AreaDecodifica.DatiConiugeOFiglio(coniugeOFiglioBL));
                }

                if (elencoDetrazioniRedditoBL != null && elencoDetrazioniRedditoBL.Count > 0)
                {
                    decodifica.ElencoDetrazioniReddito = new List<AreaDecodifica.DatiDetrazioniReddito>();
                    foreach (BLCommon.GestioneDecodifica.DetrazioniReddito detrazioniRedditoBL in elencoDetrazioniRedditoBL)
                        decodifica.ElencoDetrazioniReddito.Add(new AreaDecodifica.DatiDetrazioniReddito(detrazioniRedditoBL));
                }

                if (elencoTutoreBL != null && elencoTutoreBL.Count > 0)
                {
                    decodifica.ElencoTutore = new List<AreaDecodifica.DatiTutore>();
                    foreach (BLCommon.GestioneDecodifica.Tutore tutoreBL in elencoTutoreBL)
                        decodifica.ElencoTutore.Add(new AreaDecodifica.DatiTutore(tutoreBL));
                }

                if (elencoDelegatoBL != null && elencoDelegatoBL.Count > 0)
                {
                    decodifica.ElencoDelegato = new List<AreaDecodifica.DatiDelegato>();
                    foreach (BLCommon.GestioneDecodifica.Delegato delegatoBL in elencoDelegatoBL)
                        decodifica.ElencoDelegato.Add(new AreaDecodifica.DatiDelegato(delegatoBL));
                }

                if (elencoModalitaPagamentoBL != null && elencoModalitaPagamentoBL.Count > 0)
                {
                    decodifica.ElencoModalitaPagamento = new List<AreaDecodifica.DatiModalitaPagamento>();
                    foreach (BLCommon.GestioneDecodifica.ModalitaPagamento modalitaPagamentoBL in elencoModalitaPagamentoBL)
                        decodifica.ElencoModalitaPagamento.Add(new AreaDecodifica.DatiModalitaPagamento(modalitaPagamentoBL));
                }

                if (elencoTipoPagamentoBL != null && elencoTipoPagamentoBL.Count > 0)
                {
                    decodifica.ElencoTipoPagamento = new List<AreaDecodifica.DatiTipoPagamento>();
                    foreach (BLCommon.GestioneDecodifica.TipoPagamento tipoPagamentoBL in elencoTipoPagamentoBL)
                        decodifica.ElencoTipoPagamento.Add(new AreaDecodifica.DatiTipoPagamento(tipoPagamentoBL));
                }

                if (elencoTipoCalcoloBL != null && elencoTipoCalcoloBL.Count > 0)
                {
                    decodifica.ElencoTipoCalcolo = new List<AreaDecodifica.DatiTipoCalcolo>();
                    foreach (BLCommon.GestioneDecodifica.TipoCalcolo tipoCalcoloBL in elencoTipoCalcoloBL)
                        decodifica.ElencoTipoCalcolo.Add(new AreaDecodifica.DatiTipoCalcolo(tipoCalcoloBL));
                }

                if (elencoCausaCaricoBL != null && elencoCausaCaricoBL.Count > 0)
                {
                    decodifica.ElencoCausaCarico = new List<AreaDecodifica.DatiCausaCarico>();
                    foreach (BLCommon.GestioneDecodifica.CausaCarico causaCaricoBL in elencoCausaCaricoBL)
                        decodifica.ElencoCausaCarico.Add(new AreaDecodifica.DatiCausaCarico(causaCaricoBL));
                }

                if (elencoCodiceCristallizzazioneBL != null && elencoCodiceCristallizzazioneBL.Count > 0)
                {
                    decodifica.ElencoCodiceCristallizzazione = new List<AreaDecodifica.DatiCodiceCristallizzazione>();
                    foreach (BLCommon.GestioneDecodifica.CodiceCristallizzazione codiceCristallizzazioneBL in elencoCodiceCristallizzazioneBL)
                        decodifica.ElencoCodiceCristallizzazione.Add(new AreaDecodifica.DatiCodiceCristallizzazione(codiceCristallizzazioneBL));
                }

                if (elencoTipoPensioneBL != null && elencoTipoPensioneBL.Count > 0)
                {
                    decodifica.ElencoTipoPensione = new List<AreaDecodifica.DatiTipoPensione>();
                    foreach (BLCommon.GestioneDecodifica.TipoPensione tipoPensioneBL in elencoTipoPensioneBL)
                        decodifica.ElencoTipoPensione.Add(new AreaDecodifica.DatiTipoPensione(tipoPensioneBL));
                }

                if (elencoCodiceAziendaBL != null && elencoCodiceAziendaBL.Count > 0)
                {
                    decodifica.ElencoCodiceAzienda = new List<AreaDecodifica.DatiCodiceAzienda>();
                    foreach (BLCommon.GestioneDecodifica.CodiceAzienda codiceAziendaBL in elencoCodiceAziendaBL)
                        decodifica.ElencoCodiceAzienda.Add(new AreaDecodifica.DatiCodiceAzienda(codiceAziendaBL));
                }

                if (elencoGradoInvaliditaBL != null && elencoGradoInvaliditaBL.Count > 0)
                {
                    decodifica.ElencoGradoInvalidita = new List<AreaDecodifica.DatiGradoInvalidita>();
                    foreach (BLCommon.GestioneDecodifica.GradoInvalidita gradoInvaliditaBL in elencoGradoInvaliditaBL)
                        decodifica.ElencoGradoInvalidita.Add(new AreaDecodifica.DatiGradoInvalidita(gradoInvaliditaBL));
                }

                if (elencoProrataEnelBL != null && elencoProrataEnelBL.Count > 0)
                {
                    decodifica.ElencoProrataEnel = new List<AreaDecodifica.DatiProrataEnel>();
                    foreach (BLCommon.GestioneDecodifica.ProrataEnel prorataEnelBL in elencoProrataEnelBL)
                        decodifica.ElencoProrataEnel.Add(new AreaDecodifica.DatiProrataEnel(prorataEnelBL));
                }

                if (elencoCodiciVari != null && elencoCodiciVari.Count > 0)
                {
                    decodifica.ElencoCodiciVari = new List<AreaDecodifica.DatiCodiciVari>();
                    foreach (BLCommon.GestioneDecodifica.CodiciVari variBL in elencoCodiciVari)
                        decodifica.ElencoCodiciVari.Add(new AreaDecodifica.DatiCodiciVari(variBL));
                }

                if (elencoComunicazioneCampi1_2BL != null && elencoComunicazioneCampi1_2BL.Count > 0)
                {
                    decodifica.ElencoComunicazioneCampi1_2 = new List<AreaDecodifica.DatiComunicazioneCampi1_2>();
                    foreach (BLCommon.GestioneDecodifica.ComunicazioneCampi1_2 comunicazioneCampi1_2BL in elencoComunicazioneCampi1_2BL)
                        decodifica.ElencoComunicazioneCampi1_2.Add(new AreaDecodifica.DatiComunicazioneCampi1_2(comunicazioneCampi1_2BL));
                }

                if (elencoComunicazioneCampo3BL != null && elencoComunicazioneCampo3BL.Count > 0)
                {
                    decodifica.ElencoComunicazioneCampo3 = new List<AreaDecodifica.DatiComunicazioneCampo3>();
                    foreach (BLCommon.GestioneDecodifica.ComunicazioneCampo3 comunicazioneCampo3BL in elencoComunicazioneCampo3BL)
                        decodifica.ElencoComunicazioneCampo3.Add(new AreaDecodifica.DatiComunicazioneCampo3(comunicazioneCampo3BL));
                }

                if (elencoComunicazioneCampo4BL != null && elencoComunicazioneCampo4BL.Count > 0)
                {
                    decodifica.ElencoComunicazioneCampo4 = new List<AreaDecodifica.DatiComunicazioneCampo4>();
                    foreach (BLCommon.GestioneDecodifica.ComunicazioneCampo4 comunicazioneCampo4BL in elencoComunicazioneCampo4BL)
                        decodifica.ElencoComunicazioneCampo4.Add(new AreaDecodifica.DatiComunicazioneCampo4(comunicazioneCampo4BL));
                }

                if (elencoCodiciNaturaBL != null && elencoCodiciNaturaBL.Count > 0)
                {
                    decodifica.ElencoCodiciNatura = new List<AreaDecodifica.DatiCodiciNatura>();
                    foreach (BLCommon.GestioneDecodifica.CodiciNatura codiciNaturaBL in elencoCodiciNaturaBL)
                        decodifica.ElencoCodiciNatura.Add(new AreaDecodifica.DatiCodiciNatura(codiciNaturaBL));
                }

                if (elencoCategoriePensioneBL != null && elencoCategoriePensioneBL.Count > 0)
                {
                    decodifica.ElencoCategoriePensione = new List<AreaDecodifica.DatiCategoriaPensione>();
                    foreach (BLCommon.GestioneDecodifica.CategoriaPensione categoriaBL in elencoCategoriePensioneBL)
                        decodifica.ElencoCategoriePensione.Add(new AreaDecodifica.DatiCategoriaPensione(categoriaBL));
                }

                if (elencoFondiPensioneBL != null && elencoFondiPensioneBL.Count > 0)
                {
                    decodifica.ElencoFondiPensione = new List<AreaDecodifica.DatiFondoPensione>();
                    foreach (BLCommon.GestioneDecodifica.FondoPensione fondoBL in elencoFondiPensioneBL)
                        decodifica.ElencoFondiPensione.Add(new AreaDecodifica.DatiFondoPensione(fondoBL));
                }

                if (elencoCasseGDPBL != null && elencoCasseGDPBL.Count > 0)
                {
                    decodifica.ElencoCasseGDP = new List<AreaDecodifica.DatiFondoPensione>();
                    foreach (BLCommon.GestioneDecodifica.FondoPensione casseBL in elencoCasseGDPBL)
                        decodifica.ElencoCasseGDP.Add(new AreaDecodifica.DatiFondoPensione(casseBL));
                }

                if (elencoStatiPensioneBL != null && elencoStatiPensioneBL.Count > 0)
                {
                    decodifica.ElencoStatiPensione = new List<AreaDecodifica.DatiStatoPensione>();
                    foreach (BLCommon.GestioneDecodifica.StatoPensione statoBL in elencoStatiPensioneBL)
                        decodifica.ElencoStatiPensione.Add(new AreaDecodifica.DatiStatoPensione(statoBL));
                }

                if (elencoParentelaDC != null && elencoParentelaDC.Count > 0)
                {
                    decodifica.ElencoParentelaDC = new List<AreaDecodifica.DatiParentelaDC>();
                    foreach (BLCommon.GestioneDecodifica.ParentelaDC parentelaDC in elencoParentelaDC)
                        decodifica.ElencoParentelaDC.Add(new AreaDecodifica.DatiParentelaDC(parentelaDC));
                }

                if (elencoRicercaGPT != null && elencoRicercaGPT.Count > 0)
                {
                    decodifica.ElencoRicercaGPT = new List<AreaDecodifica.DatiRicercaGPT>();
                    foreach (BLCommon.GestioneDecodifica.CtrlRicercaGPT ricerca in elencoRicercaGPT)
                        decodifica.ElencoRicercaGPT.Add(new AreaDecodifica.DatiRicercaGPT(ricerca));
                }

                if (elencoCatEnteAltrePensioni != null && elencoCatEnteAltrePensioni.Count > 0)
                {
                    decodifica.ElencoCategorieAltraPensione = new List<AreaDecodifica.DatiCategoriaAltraPensione>();
                    foreach (BLCommon.GestioneDecodifica.CatEnteAltraPensione categoria in elencoCatEnteAltrePensioni)
                        decodifica.ElencoCategorieAltraPensione.Add(new AreaDecodifica.DatiCategoriaAltraPensione(categoria));
                }
                if (elencoCtrlEnteCassaCodiceGestione != null && elencoCtrlEnteCassaCodiceGestione.Count > 0)
                {
                    decodifica.ElencoCtrlEnteCassaCodiceGestione = new List<AreaDecodifica.DatiCtrlEnteCassaCodiceGestione>();
                    foreach (BLCommon.GestioneDecodifica.CtrlEnteCassaCodiceGestione categoria in elencoCtrlEnteCassaCodiceGestione)
                        decodifica.ElencoCtrlEnteCassaCodiceGestione.Add(new AreaDecodifica.DatiCtrlEnteCassaCodiceGestione(categoria));
                }

                if (elencoCtrlCatAdeguata != null && elencoCtrlCatAdeguata.Count > 0)
                {
                    decodifica.ElencoCtrlCatAdeguata = new List<AreaDecodifica.DatiCtrlCatAdeguata>();
                    foreach (BLCommon.GestioneDecodifica.CtrlCatAdeguata categoria in elencoCtrlCatAdeguata)
                        decodifica.ElencoCtrlCatAdeguata.Add(new AreaDecodifica.DatiCtrlCatAdeguata(categoria));
                }

                if (elencoDecComparto != null && elencoDecComparto.Count > 0)
                {
                    decodifica.ElencoDecComparto = new List<AreaDecodifica.DatiDecComparto>();
                    foreach (BLCommon.GestioneDecodifica.DecComparto dec in elencoDecComparto)
                        decodifica.ElencoDecComparto.Add(new AreaDecodifica.DatiDecComparto(dec));
                }

                if (elencoDecSettore != null && elencoDecSettore.Count > 0)
                {
                    decodifica.ElencoDecSettore = new List<AreaDecodifica.DatiDecSettore>();
                    foreach (BLCommon.GestioneDecodifica.DecSettore dec in elencoDecSettore)
                        decodifica.ElencoDecSettore.Add(new AreaDecodifica.DatiDecSettore(dec));
                }

                if (elencoDecRuolo != null && elencoDecRuolo.Count > 0)
                {
                    decodifica.ElencoDecRuolo = new List<AreaDecodifica.DatiDecRuolo>();
                    foreach (BLCommon.GestioneDecodifica.DecRuolo dec in elencoDecRuolo)
                        decodifica.ElencoDecRuolo.Add(new AreaDecodifica.DatiDecRuolo(dec));
                }

                if (elencoDecSede != null && elencoDecSede.Count > 0)
                {
                    decodifica.ElencoDecSede = new List<AreaDecodifica.DatiDecSede>();
                    foreach (BLCommon.GestioneDecodifica.DecSede dec in elencoDecSede)
                        decodifica.ElencoDecSede.Add(new AreaDecodifica.DatiDecSede(dec));
                }

                #endregion Valorizzazione dati decodifica
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            return decodifica;
        }

        public List<AreaDecodifica.DatiComune> GetComuniPerProvincia(string siglaProvincia)
        {
            SetCulture();

            List<AreaDecodifica.DatiComune> elencoComuni = null;
            try
            {
                List<BLCommon.GestioneDecodifica.Comune> elencoComuniBL = null;
                BLCommon.GestioneDecodifica.GetComuniPerProvincia(siglaProvincia, out elencoComuniBL);

                if (elencoComuniBL != null && elencoComuniBL.Count > 0)
                {
                    elencoComuni = new List<AreaDecodifica.DatiComune>();
                    foreach (BLCommon.GestioneDecodifica.Comune comuneBL in elencoComuniBL)
                        elencoComuni.Add(new AreaDecodifica.DatiComune(comuneBL));
                }
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            return elencoComuni;
        }
        #endregion IDecodifica members

        #region IQuadri members
        #region public IQuadri members
        public AreaEsito AggiornaQuadri(AreaRichiestaDomanda areaRichiestaDomanda, ref AreaInfoPratica areaInfoPratica)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            bool isCalcoloAbilitato = false;
            string statoPensione = string.Empty;
            string matricolaUtenteAcquisizione = string.Empty;
            bool isMatchMatricola = false;
            string errori = string.Empty;

            if (areaInfoPratica.AreaQuadri == null)
            {
                areaInfoPratica.AreaQuadri = GetQuadriByDatiPensione(datiPensione);
            }
            else
            {
                foreach (AreaQuadri.Tab tab in areaInfoPratica.ElencoTab)
                {
                    switch (tab)
                    {
                        case AreaQuadri.Tab.Bititolarita:
                            areaInfoPratica.AreaQuadri.QuadroBititolarita = GetQuadroBititolaritaByDatiPensione(datiPensione);
                            break;
                        case AreaQuadri.Tab.DanteCausa:
                            areaInfoPratica.AreaQuadri.QuadroDanteCausa = GetQuadroDanteCausaByDatiPensione(datiPensione);
                            break;
                        case AreaQuadri.Tab.DelegatoTutore:
                            areaInfoPratica.AreaQuadri.QuadroDelegatoTutore = GetQuadroDelegatoTutoreByDatiPensione(datiPensione);
                            break;
                        case AreaQuadri.Tab.Detrazioni:
                            areaInfoPratica.AreaQuadri.QuadroDetrazioni = GetQuadroDetrazioniByDatiPensione(datiPensione);
                            break;
                        case AreaQuadri.Tab.Familiare:
                            areaInfoPratica.AreaQuadri.QuadroFamiliari = GetQuadroFamiliariByDatiPensione(datiPensione);
                            break;
                        case AreaQuadri.Tab.LiquidazionePensione:
                            areaInfoPratica.AreaQuadri.QuadroLiquidazionePensione = GetQuadroLiquidazionePensioneByDatiPensione(datiPensione);
                            break;
                        case AreaQuadri.Tab.MaggiorazioniEBenefici:
                            areaInfoPratica.AreaQuadri.QuadroMaggiorazioniBenefici = GetQuadroMaggiorazioniBeneficiByDatiPensione(datiPensione);
                            break;
                        case AreaQuadri.Tab.Pagamento:
                            areaInfoPratica.AreaQuadri.QuadroPagamento = GetQuadroPagamentoByDatiPensione(datiPensione);
                            break;
                        case AreaQuadri.Tab.Redditi:
                            areaInfoPratica.AreaQuadri.QuadroRedditi = GetQuadroRedditiByDatiPensione(datiPensione);
                            break;
                        case AreaQuadri.Tab.Supplementi:
                            areaInfoPratica.AreaQuadri.QuadroSupplementi = GetQuadroSupplementiByDatiPensione(datiPensione);
                            break;
                        case AreaQuadri.Tab.Titolare:
                            areaInfoPratica.AreaQuadri.QuadroTitolare = GetQuadroTitolareByDatiPensione(datiPensione);
                            break;
                        case AreaQuadri.Tab.DatiCalcolo:
                            areaInfoPratica.AreaQuadri.QuadroDatiContributivi = GetQuadroDatiContributiviByDatiPensione(datiPensione);
                            break;
                        case AreaQuadri.Tab.Eliminazione:
                            areaInfoPratica.AreaQuadri.QuadroEliminazione = GetQuadroEliminazioneByDatiPensione(datiPensione);
                            break;
                        case AreaQuadri.Tab.Oneri:
                            areaInfoPratica.AreaQuadri.QuadroOneri = GetQuadroOneriByDatiPensione(datiPensione);
                            break;
                        case AreaQuadri.Tab.DatiFondo:
                            areaInfoPratica.AreaQuadri.QuadroDatiFondo = GetQuadroDatiFondoByDatiPensione(datiPensione);
                            break;
                        case AreaQuadri.Tab.DatiNoCalcolo:
                            areaInfoPratica.AreaQuadri.QuadroDatiNoCalcolo = GetQuadroDatiNoCalcoloByDatiPensione(datiPensione);
                            break;
                        case AreaQuadri.Tab.Periodi:
                            areaInfoPratica.AreaQuadri.QuadroPeriodi = GetQuadroPeriodiByDatiPensione(datiPensione);
                            break;
                        case AreaQuadri.Tab.AventiDiritto:
                            areaInfoPratica.AreaQuadri.QuadroAventiDiritto = GetQuadroAventiDirittoByDatiPensione(datiPensione);
                            break;
                        case AreaQuadri.Tab.AltreDomandeCollegate:
                            areaInfoPratica.AreaQuadri.QuadroAltreDomandeCollegate = GetQuadroAltreDomandeCollegateByDatiPensione(datiPensione);
                            break;
                        case AreaQuadri.Tab.RichiestaBonus:
                            areaInfoPratica.AreaQuadri.QuadroRichiestaBonus = GetQuadroRichiestaBonusByDatiPensione(datiPensione);
                            break;
                    }
                }
            }

            ControllaInfoPratica(datiPensione, areaInfoPratica.AreaQuadri, areaInfoPratica.MatricolaOperatore, areaInfoPratica.SedeOperatore, out isCalcoloAbilitato, out statoPensione, out matricolaUtenteAcquisizione, out isMatchMatricola, out errori);

            areaInfoPratica.IsCalcoloAbilitato = isCalcoloAbilitato;
            areaInfoPratica.StatoPensione = statoPensione;
            areaInfoPratica.MatricolaUtenteAcquisizione = matricolaUtenteAcquisizione;
            areaInfoPratica.IsMatchMatricola = isMatchMatricola;

            if (!string.IsNullOrEmpty(errori))
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
            else
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;

            Esito.Messaggio = errori;
            return Esito;
        }

        public AreaQuadri GetQuadriByDomanda(AreaRichiestaDomanda areaRichiestaDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);
            AreaQuadri quadri = null;
            try
            {
                #region Valorizzazione dati quadri
                quadri = new AreaQuadri();
                quadri.QuadroTitolare = GetQuadroTitolareByDatiPensione(datiPensione);
                quadri.QuadroDetrazioni = GetQuadroDetrazioniByDatiPensione(datiPensione);
                quadri.QuadroPagamento = GetQuadroPagamentoByDatiPensione(datiPensione);
                quadri.QuadroLiquidazionePensione = GetQuadroLiquidazionePensioneByDatiPensione(datiPensione);
                quadri.QuadroDelegatoTutore = GetQuadroDelegatoTutoreByDatiPensione(datiPensione);
                quadri.QuadroDatiContributivi = GetQuadroDatiContributiviByDatiPensione(datiPensione);
                quadri.QuadroRedditi = GetQuadroRedditiByDatiPensione(datiPensione);
                quadri.QuadroFamiliari = GetQuadroFamiliariByDatiPensione(datiPensione);
                quadri.QuadroDanteCausa = GetQuadroDanteCausaByDatiPensione(datiPensione);
                quadri.QuadroMaggiorazioniBenefici = GetQuadroMaggiorazioniBeneficiByDatiPensione(datiPensione);
                quadri.QuadroSupplementi = GetQuadroSupplementiByDatiPensione(datiPensione);
                quadri.QuadroBititolarita = GetQuadroBititolaritaByDatiPensione(datiPensione);
                quadri.QuadroEliminazione = GetQuadroEliminazioneByDatiPensione(datiPensione);
                quadri.QuadroOneri = GetQuadroOneriByDatiPensione(datiPensione);
                quadri.QuadroDatiFondo = GetQuadroDatiFondoByDatiPensione(datiPensione);
                quadri.QuadroDatiNoCalcolo = GetQuadroDatiNoCalcoloByDatiPensione(datiPensione);
                quadri.QuadroPeriodi = GetQuadroPeriodiByDatiPensione(datiPensione);
                quadri.QuadroAventiDiritto = GetQuadroAventiDirittoByDatiPensione(datiPensione);
                quadri.QuadroAltreDomandeCollegate = GetQuadroAltreDomandeCollegateByDatiPensione(datiPensione);
                quadri.QuadroRichiestaBonus = GetQuadroRichiestaBonusByDatiPensione(datiPensione);
                #endregion Valorizzazione dati quadri
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            return quadri;
        }

        private AreaQuadri GetQuadriByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            AreaQuadri quadri = null;
            try
            {
                #region Valorizzazione dati quadri
                quadri = new AreaQuadri();
                quadri.QuadroTitolare = GetQuadroTitolareByDatiPensione(datiPensione);
                quadri.QuadroDetrazioni = GetQuadroDetrazioniByDatiPensione(datiPensione);
                quadri.QuadroPagamento = GetQuadroPagamentoByDatiPensione(datiPensione);
                quadri.QuadroLiquidazionePensione = GetQuadroLiquidazionePensioneByDatiPensione(datiPensione);
                quadri.QuadroDelegatoTutore = GetQuadroDelegatoTutoreByDatiPensione(datiPensione);
                quadri.QuadroDatiContributivi = GetQuadroDatiContributiviByDatiPensione(datiPensione);
                quadri.QuadroRedditi = GetQuadroRedditiByDatiPensione(datiPensione);
                quadri.QuadroFamiliari = GetQuadroFamiliariByDatiPensione(datiPensione);
                quadri.QuadroDanteCausa = GetQuadroDanteCausaByDatiPensione(datiPensione);
                quadri.QuadroMaggiorazioniBenefici = GetQuadroMaggiorazioniBeneficiByDatiPensione(datiPensione);
                quadri.QuadroSupplementi = GetQuadroSupplementiByDatiPensione(datiPensione);
                quadri.QuadroBititolarita = GetQuadroBititolaritaByDatiPensione(datiPensione);
                quadri.QuadroEliminazione = GetQuadroEliminazioneByDatiPensione(datiPensione);
                quadri.QuadroOneri = GetQuadroOneriByDatiPensione(datiPensione);
                quadri.QuadroDatiFondo = GetQuadroDatiFondoByDatiPensione(datiPensione);
                quadri.QuadroDatiNoCalcolo = GetQuadroDatiNoCalcoloByDatiPensione(datiPensione);
                quadri.QuadroPeriodi = GetQuadroPeriodiByDatiPensione(datiPensione);
                quadri.QuadroAventiDiritto = GetQuadroAventiDirittoByDatiPensione(datiPensione);
                quadri.QuadroAltreDomandeCollegate = GetQuadroAltreDomandeCollegateByDatiPensione(datiPensione);
                quadri.QuadroRichiestaBonus = GetQuadroRichiestaBonusByDatiPensione(datiPensione);
                #endregion Valorizzazione dati quadri
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            return quadri;
        }

        public AreaQuadri.DatiQuadroTitolare GetQuadroTitolareByDomanda(AreaRichiestaDomanda areaRichiestaDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            return GetQuadroTitolareByDatiPensione(datiPensione);
        }

        private AreaQuadri.DatiQuadroTitolare GetQuadroTitolareByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            BLCommon.GestioneQuadri.DatiQuadroTitolare quadroTitolareBL = null;
            BLCommon.GestioneQuadri.GetQuadroTitolareByDatiPensione(datiPensione, out quadroTitolareBL);

            return new AreaQuadri.DatiQuadroTitolare(quadroTitolareBL);
        }

        public AreaQuadri.DatiQuadroDetrazioni GetQuadroDetrazioniByDomanda(AreaRichiestaDomanda areaRichiestaDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            return GetQuadroDetrazioniByDatiPensione(datiPensione);
        }

        private AreaQuadri.DatiQuadroDetrazioni GetQuadroDetrazioniByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            BLCommon.GestioneQuadri.DatiQuadroDetrazioni quadroDetrazioniBL = null;
            BLCommon.GestioneQuadri.GetQuadroDetrazioniByDatiPensione(datiPensione, out quadroDetrazioniBL);

            return new AreaQuadri.DatiQuadroDetrazioni(quadroDetrazioniBL);
        }

        public AreaQuadri.DatiQuadroPagamento GetQuadroPagamentoByDomanda(AreaRichiestaDomanda areaRichiestaDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            return GetQuadroPagamentoByDatiPensione(datiPensione);
        }

        private AreaQuadri.DatiQuadroPagamento GetQuadroPagamentoByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            BLCommon.GestioneQuadri.DatiQuadroPagamento quadroPagamentoBL = null;
            BLCommon.GestioneQuadri.GetQuadroPagamentoByDatiPensione(datiPensione, out quadroPagamentoBL);

            return new AreaQuadri.DatiQuadroPagamento(quadroPagamentoBL);
        }

        public AreaQuadri.DatiQuadroLiquidazionePensione GetQuadroLiquidazionePensioneByDomanda(AreaRichiestaDomanda areaRichiestaDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            return GetQuadroLiquidazionePensioneByDatiPensione(datiPensione);
        }

        private AreaQuadri.DatiQuadroLiquidazionePensione GetQuadroLiquidazionePensioneByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            BLCommon.GestioneQuadri.DatiQuadroLiquidazionePensione quadroLiquidazionePensioneBL = null;
            BLCommon.GestioneQuadri.GetQuadroLiquidazionePensioneByDatiPensione(datiPensione, out quadroLiquidazionePensioneBL);

            return new AreaQuadri.DatiQuadroLiquidazionePensione(quadroLiquidazionePensioneBL);
        }

        public AreaQuadri.DatiQuadroDelegatoTutore GetQuadroDelegatoTutoreByDomanda(AreaRichiestaDomanda areaRichiestaDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            return GetQuadroDelegatoTutoreByDatiPensione(datiPensione);
        }

        private AreaQuadri.DatiQuadroDelegatoTutore GetQuadroDelegatoTutoreByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            BLCommon.GestioneQuadri.DatiQuadroDelegatoTutore quadroDelegatoTutoreBL = null;
            BLCommon.GestioneQuadri.GetQuadroDelegatoTutoreByDatiPensione(datiPensione, out quadroDelegatoTutoreBL);

            return new AreaQuadri.DatiQuadroDelegatoTutore(quadroDelegatoTutoreBL);
        }

        public AreaQuadri.DatiQuadroDatiContributivi GetQuadroDatiContributiviByDomanda(AreaRichiestaDomanda areaRichiestaDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            return GetQuadroDatiContributiviByDatiPensione(datiPensione);
        }

        private AreaQuadri.DatiQuadroDatiContributivi GetQuadroDatiContributiviByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            BLCommon.GestioneQuadri.DatiQuadroDatiContributivi quadroDatiContributiviBL = null;
            BLCommon.GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributiviBL);

            return new AreaQuadri.DatiQuadroDatiContributivi(quadroDatiContributiviBL);
        }

        public AreaQuadri.DatiQuadroRedditi GetQuadroRedditiByDomanda(AreaRichiestaDomanda areaRichiestaDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            return GetQuadroRedditiByDatiPensione(datiPensione);
        }

        private AreaQuadri.DatiQuadroRedditi GetQuadroRedditiByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            BLCommon.GestioneQuadri.DatiQuadroRedditi quadroRedditiBL = null;
            BLCommon.GestioneQuadri.GetQuadroRedditiByIdPensione(datiPensione, out quadroRedditiBL);

            return new AreaQuadri.DatiQuadroRedditi(quadroRedditiBL);
        }

        public AreaQuadri.DatiQuadroFamiliari GetQuadroFamiliariByDomanda(AreaRichiestaDomanda areaRichiestaDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            return GetQuadroFamiliariByDatiPensione(datiPensione);
        }

        private AreaQuadri.DatiQuadroFamiliari GetQuadroFamiliariByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            BLCommon.GestioneQuadri.DatiQuadroFamiliari quadroFamiliariBL = null;
            BLCommon.GestioneQuadri.GetQuadroFamiliariByDatiPensione(datiPensione, out quadroFamiliariBL);

            return new AreaQuadri.DatiQuadroFamiliari(quadroFamiliariBL);
        }

        public AreaQuadri.DatiQuadroDanteCausa GetQuadroDanteCausaByDomanda(AreaRichiestaDomanda areaRichiestaDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            return GetQuadroDanteCausaByDatiPensione(datiPensione);
        }

        private AreaQuadri.DatiQuadroDanteCausa GetQuadroDanteCausaByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            BLCommon.GestioneQuadri.DatiQuadroDanteCausa quadroDanteCausaBL = null;
            BLCommon.GestioneQuadri.GetQuadroDanteCausaByDatiPensione(datiPensione, out quadroDanteCausaBL);

            return new AreaQuadri.DatiQuadroDanteCausa(quadroDanteCausaBL);
        }

        public AreaQuadri.DatiQuadroMaggiorazioniBenefici GetQuadroMaggiorazioniBeneficiByDomanda(AreaRichiestaDomanda areaRichiestaDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            return GetQuadroMaggiorazioniBeneficiByDatiPensione(datiPensione);
        }

        private AreaQuadri.DatiQuadroMaggiorazioniBenefici GetQuadroMaggiorazioniBeneficiByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            BLCommon.GestioneQuadri.DatiQuadroMaggiorazioniBenefici quadroMaggiorazioniBeneficiBL = null;
            BLCommon.GestioneQuadri.GetQuadroMaggiorazioniBeneficiByDatiPensione(datiPensione, out quadroMaggiorazioniBeneficiBL);

            return new AreaQuadri.DatiQuadroMaggiorazioniBenefici(quadroMaggiorazioniBeneficiBL);
        }

        public AreaQuadri.DatiQuadroSupplementi GetQuadroSupplementiByDomanda(AreaRichiestaDomanda areaRichiestaDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            return GetQuadroSupplementiByDatiPensione(datiPensione);
        }

        private AreaQuadri.DatiQuadroSupplementi GetQuadroSupplementiByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            BLCommon.GestioneQuadri.DatiQuadroSupplementi quadroSupplementiBL = null;
            BLCommon.GestioneQuadri.GetQuadroSupplementiByDatiPensione(datiPensione, out quadroSupplementiBL);

            return new AreaQuadri.DatiQuadroSupplementi(quadroSupplementiBL);
        }

        public AreaQuadri.DatiQuadroBititolarita GetQuadroBititolaritaByDomanda(AreaRichiestaDomanda areaRichiestaDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            return GetQuadroBititolaritaByDatiPensione(datiPensione);
        }

        private AreaQuadri.DatiQuadroBititolarita GetQuadroBititolaritaByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            BLCommon.GestioneQuadri.DatiQuadroBititolarita quadroBititolaritaBL = null;
            BLCommon.GestioneQuadri.GetQuadroBititolaritaByDatiPensione(datiPensione, out quadroBititolaritaBL);

            return new AreaQuadri.DatiQuadroBititolarita(quadroBititolaritaBL);
        }

        public AreaQuadri.DatiQuadroEliminazione GetQuadroEliminazioneByDomanda(AreaRichiestaDomanda areaRichiestaDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            return GetQuadroEliminazioneByDatiPensione(datiPensione);
        }

        private AreaQuadri.DatiQuadroEliminazione GetQuadroEliminazioneByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            BLCommon.GestioneQuadri.DatiQuadroEliminazione quadroEliminazioneBL = null;
            BLCommon.GestioneQuadri.GetQuadroEliminazioneByDatiPensione(datiPensione, out quadroEliminazioneBL);

            return new AreaQuadri.DatiQuadroEliminazione(quadroEliminazioneBL);
        }

        public AreaQuadri.DatiQuadroOneri GetQuadroOneriByDomanda(AreaRichiestaDomanda areaRichiestaDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);
            return GetQuadroOneriByDatiPensione(datiPensione);
        }

        private AreaQuadri.DatiQuadroOneri GetQuadroOneriByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            BLCommon.GestioneQuadri.DatiQuadroOneri quadroOneriBL = null;
            BLCommon.GestioneQuadri.GetQuadroOneriByDatiPensione(datiPensione, out quadroOneriBL);

            return new AreaQuadri.DatiQuadroOneri(quadroOneriBL);
        }

        private AreaQuadri.DatiQuadroDatiFondo GetQuadroDatiFondoByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            BLCommon.GestioneQuadri.DatiQuadroDatiFondo quadroDatiFondoBL = null;
            BLCommon.GestioneQuadri.GetQuadroDatiFondoByDatiPensione(datiPensione, out quadroDatiFondoBL);

            return new AreaQuadri.DatiQuadroDatiFondo(quadroDatiFondoBL);
        }

        private AreaQuadri.DatiQuadroDatiNoCalcolo GetQuadroDatiNoCalcoloByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            BLCommon.GestioneQuadri.DatiQuadroDatiNoCalcolo quadroDatiNoCalcolo = null;
            BLCommon.GestioneQuadri.GetQuadroDatiNoCalcoloByDatiPensione(datiPensione, out quadroDatiNoCalcolo);

            return new AreaQuadri.DatiQuadroDatiNoCalcolo(quadroDatiNoCalcolo);
        }

        private AreaQuadri.DatiQuadroPeriodi GetQuadroPeriodiByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            BLCommon.GestioneQuadri.DatiQuadroPeriodi quadroPeriodi = null;
            BLCommon.GestioneQuadri.GetQuadroPeriodiByDatiPensione(datiPensione, out quadroPeriodi);

            return new AreaQuadri.DatiQuadroPeriodi(quadroPeriodi);
        }

        private AreaQuadri.DatiQuadroAventiDiritto GetQuadroAventiDirittoByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            BLCommon.GestioneQuadri.DatiQuadroAventiDiritto quadroAventiDiritto = null;
            BLCommon.GestioneQuadri.GetQuadroAventiDirittoByDatiPensione(datiPensione, out quadroAventiDiritto);

            return new AreaQuadri.DatiQuadroAventiDiritto(quadroAventiDiritto);
        }

        private AreaQuadri.DatiQuadroAltreDomandeCollegate GetQuadroAltreDomandeCollegateByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            BLCommon.GestioneQuadri.DatiQuadroAltreDomandeCollegate quadroAltreDomandeCollegate = null;
            BLCommon.GestioneQuadri.GetQuadroAltreDomandeCollegateByDatiPensione(datiPensione, out quadroAltreDomandeCollegate);

            return new AreaQuadri.DatiQuadroAltreDomandeCollegate(quadroAltreDomandeCollegate);
        }

        public AreaQuadri.DatiQuadroRichiestaBonus GetQuadroRichiestaBonusByDomanda(AreaRichiestaDomanda areaRichiestaDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);
            return GetQuadroRichiestaBonusByDatiPensione(datiPensione);
        }

        private AreaQuadri.DatiQuadroRichiestaBonus GetQuadroRichiestaBonusByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            BLCommon.GestioneQuadri.DatiQuadroRichiestaBonus quadroRichiestaBonusBL = null;
            BLCommon.GestioneQuadri.GetQuadroRichiestaBonusByDatiPensione(datiPensione, out quadroRichiestaBonusBL);

            return new AreaQuadri.DatiQuadroRichiestaBonus(quadroRichiestaBonusBL);
        }

        #endregion public IQuadri members

        #region private IQuadri members
        private static void ControllaInfoPratica(GestionePensione.DatiPensione datiPensione, AreaQuadri areaQuadri, string matricolaOperatore, short sedeOperatore, out bool IsCalcoloAbilitato, out string statoPensione, out string matricolaUtenteAcquisizione, out bool isMatchMatricola, out string errori)
        {
            statoPensione = string.Empty;
            errori = string.Empty;
            matricolaUtenteAcquisizione = string.Empty;
            isMatchMatricola = false;

            if (areaQuadri != null)
            {
                IsCalcoloAbilitato = VerificaAbilitaCalcolo(areaQuadri);

                GestioneAreaStatoPratica.ControllaInfoPratica(datiPensione, matricolaOperatore, sedeOperatore, ref IsCalcoloAbilitato, out statoPensione, out matricolaUtenteAcquisizione, out errori);

                isMatchMatricola = (!string.IsNullOrEmpty(matricolaUtenteAcquisizione) && matricolaOperatore == matricolaUtenteAcquisizione);

            }
            else
                IsCalcoloAbilitato = false;
        }

        private static bool VerificaAbilitaCalcolo(AreaQuadri areaQuadri)
        {
            if ((areaQuadri.QuadroTitolare != null && areaQuadri.QuadroTitolare.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato) ||
                    (areaQuadri.QuadroDetrazioni != null && areaQuadri.QuadroDetrazioni.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato) ||
                    (areaQuadri.QuadroPagamento != null && areaQuadri.QuadroPagamento.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato) ||
                    (areaQuadri.QuadroLiquidazionePensione != null && areaQuadri.QuadroLiquidazionePensione.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato) ||
                    (areaQuadri.QuadroDelegatoTutore != null && areaQuadri.QuadroDelegatoTutore.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato) ||
                    (areaQuadri.QuadroDatiContributivi != null && areaQuadri.QuadroDatiContributivi.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato) ||
                    (areaQuadri.QuadroRedditi != null && areaQuadri.QuadroRedditi.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato) ||
                    (areaQuadri.QuadroFamiliari != null && areaQuadri.QuadroFamiliari.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato) ||
                    (areaQuadri.QuadroDanteCausa != null && areaQuadri.QuadroDanteCausa.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato) ||
                    (areaQuadri.QuadroMaggiorazioniBenefici != null && areaQuadri.QuadroMaggiorazioniBenefici.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato) ||
                    (areaQuadri.QuadroSupplementi != null && areaQuadri.QuadroSupplementi.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato) ||
                    (areaQuadri.QuadroBititolarita != null && areaQuadri.QuadroBititolarita.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato) ||
                    (areaQuadri.QuadroEliminazione != null && areaQuadri.QuadroEliminazione.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato) ||
                    (areaQuadri.QuadroOneri != null && areaQuadri.QuadroOneri.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato) ||
                    (areaQuadri.QuadroDatiFondo != null && areaQuadri.QuadroDatiFondo.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato) ||
                    (areaQuadri.QuadroDatiNoCalcolo != null && areaQuadri.QuadroDatiNoCalcolo.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato) ||
                    (areaQuadri.QuadroPeriodi != null && areaQuadri.QuadroPeriodi.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato) ||
                    (areaQuadri.QuadroAventiDiritto != null && areaQuadri.QuadroAventiDiritto.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato) ||
                    (areaQuadri.QuadroAltreDomandeCollegate != null && areaQuadri.QuadroAltreDomandeCollegate.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato) ||
                    (areaQuadri.QuadroRichiestaBonus != null && areaQuadri.QuadroRichiestaBonus.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato)
                )
                return false;
            else
                return true;
        }

        #endregion private IQuadri members
        #endregion IQuadri members

        #region AreaCambioStatoDomanda
        public AreaEsito CambioStatoDomanda(ref AreaCambioStatoDomanda areaCambioStatoDomanda)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            try
            {
                string messaggioVideo = string.Empty;


                if (!areaCambioStatoDomanda.IsUpdateOperation)
                {
                    //Search Operation
                    GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaCambioStatoDomanda.NumeroDomanda, null);
                    GestioneAreaCambioStatoDomanda.DatiCambiaStatoDomanda datiCambioStatoDomanda = new GestioneAreaCambioStatoDomanda.DatiCambiaStatoDomanda();
                    datiCambioStatoDomanda.NumeroDomanda = areaCambioStatoDomanda.NumeroDomanda;
                    datiCambioStatoDomanda.Ruolo = areaCambioStatoDomanda.Ruolo;
                    datiCambioStatoDomanda.Sede = areaCambioStatoDomanda.Sede;
                    datiCambioStatoDomanda.TipoAppOperatore = areaCambioStatoDomanda.TipoAppOperatore;

                    GestioneAreaCambioStatoDomanda.RicercaDomanda(ref datiCambioStatoDomanda, datiPensione, out messaggioVideo);
                    if (messaggioVideo == string.Empty || !string.IsNullOrEmpty(datiCambioStatoDomanda.SedeDiversa))
                    {
                        areaCambioStatoDomanda.NumeroDomandaUpdate = datiCambioStatoDomanda.NumeroDomanda;
                        areaCambioStatoDomanda.StatoPensione = datiCambioStatoDomanda.StatoPensione;
                        areaCambioStatoDomanda.NCertificato = datiCambioStatoDomanda.NCertificato;
                        areaCambioStatoDomanda.DataElaborazioneWebdom = datiCambioStatoDomanda.DataElaborazioneWebdom;
                        if (!string.IsNullOrEmpty(datiCambioStatoDomanda.SedeDiversa))
                            areaCambioStatoDomanda.SedeDiversa = datiCambioStatoDomanda.SedeDiversa;
                    }
                }
                else
                {
                    //Update Operation
                    GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaCambioStatoDomanda.NumeroDomandaUpdate, null);
                    byte? statoPrecedente = datiPensione != null ? datiPensione.StatoPensione : null;
                    GestioneAreaCambioStatoDomanda.AggiornaDomanda(areaCambioStatoDomanda.NuovoStatoPensione, areaCambioStatoDomanda.NuovoNCertificato, areaCambioStatoDomanda.NuovaDataElaborazioneWebdom, datiPensione, out messaggioVideo);
                    if (messaggioVideo == string.Empty)
                    {
                        areaCambioStatoDomanda.NumeroDomandaUpdate = datiPensione.NDomus;
                        areaCambioStatoDomanda.StatoPensione = areaCambioStatoDomanda.NuovoStatoPensione;
                        //SCRIWO
                        GestioneWSSCRIWO.AggiornaStatoLavorazione(datiPensione, statoPrecedente, string.Empty, 0);
                    }
                }
                if (!String.IsNullOrEmpty(messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                    return Esito;
                }
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nella durante il cambio stato domanda";
            }
            return Esito;
        }
        #endregion AreaCambioStatoDomanda

        #region AreaOneri

        public AreaEsito GetOneriByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaOneri areaOneri)
        {
            SetCulture();

            Entity.Oneri.DatiPrepensionamento datiPrepensionamento = null;

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            GestionePrepensionamento.DatiPrepensionamento datiPrepensionamentoCommon = null;
            GestionePrepensionamento.GetDatiPrepensionamentoByIdPensione(datiPensione.Id, out datiPrepensionamentoCommon);

            GestioneAreaOneri.ValorizzaDatiPrepensionamentoByDatiPensione(datiPensione, datiPrepensionamentoCommon, out datiPrepensionamento);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            //ENG - Memo 121_2023
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            GestioneControlliDinamici.ControlloDinamico ctrlEliminazioneScartoOneri0031_0105_0112 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("EliminazioneScartoOneri0031_0105_0112", out ctrlEliminazioneScartoOneri0031_0105_0112);

            AreaEsito Esito = new AreaEsito();
            areaOneri = null;

            #region Oneri - Benefici Particolari

            #region Oneri

            List<Entity.Oneri.DatiOneriBenefParticolari.DatiOneri> lDatiOneri = null;
            List<Entity.CodiciOneri.GruppoOneri> listaGruppoOneri = null;
            List<Entity.CodiciOneri.SottoGruppoOneri> listaSottoGruppoOneri = null;
            GestioneAreaOneri.GetDatiOneri(datiPensione, isRiaperturaDomanda, out lDatiOneri, out listaGruppoOneri, out listaSottoGruppoOneri, datiAnagraficiTitolare);
            if (lDatiOneri != null && lDatiOneri.Count > 0)
            {
                if (areaOneri == null)
                    areaOneri = new AreaOneri();

                if (areaOneri.DatiOneriBenefParticolari == null)
                    areaOneri.DatiOneriBenefParticolari = new Entity.Oneri.DatiOneriBenefParticolari();
                areaOneri.DatiOneriBenefParticolari.ListaDatiOneri = lDatiOneri;
            }
            if (listaGruppoOneri != null)
            {
                if (areaOneri == null)
                    areaOneri = new AreaOneri();
                areaOneri.ListaGruppoOneri = listaGruppoOneri;
            }
            if (listaSottoGruppoOneri != null)
            {
                if (areaOneri == null)
                    areaOneri = new AreaOneri();
                areaOneri.ListaSottoGruppoOneri = listaSottoGruppoOneri;
            }

            #endregion Oneri

            #region Oneri Storico
            List<Entity.Oneri.DatiOneriBenefParticolari.DatiOneri> lDatiOneriStorico = null;
            GestioneAreaOneri.GetDatiOneriStorico(datiPensione.Id, out lDatiOneriStorico);

            if (lDatiOneriStorico != null && lDatiOneriStorico.Count > 0)
            {
                if (areaOneri == null)
                    areaOneri = new AreaOneri();

                if (areaOneri.DatiOneriBenefParticolariStorico == null)
                    areaOneri.DatiOneriBenefParticolariStorico = new Entity.Oneri.DatiOneriBenefParticolari();
                areaOneri.DatiOneriBenefParticolariStorico.ListaDatiOneri = lDatiOneriStorico;

                if (tipoAppartenenza == Utility.TipoAppartenenza.AGO && ctrlEliminazioneScartoOneri0031_0105_0112 != null && !String.IsNullOrEmpty(ctrlEliminazioneScartoOneri0031_0105_0112.ValoreControllo)
                    && ctrlEliminazioneScartoOneri0031_0105_0112.ValoreControllo.Trim().ToUpperInvariant() == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione))
                {
                    if (lDatiOneri != null && lDatiOneri.Count() > 0)
                    {
                        foreach (Entity.Oneri.DatiOneriBenefParticolari.DatiOneri onereCorrente in lDatiOneri)
                        {
                            if (lDatiOneriStorico.Exists(x => x.IdCodeGruppo.GetValueOrDefault() == onereCorrente.IdCodeGruppo.GetValueOrDefault() && x.IdCodeSottoGruppo.GetValueOrDefault() == onereCorrente.IdCodeSottoGruppo.GetValueOrDefault()))
                            {
                                onereCorrente.IsFromPrelievo = true;
                            }
                        }
                    }
                }
            }

            #endregion Oneri Storico

            #region Benefici Particolari

            List<Entity.Oneri.DatiOneriBenefParticolari.DatiBeneficiParticolari> listDatiBeneficiParticolari = null;
            GestioneAreaOneri.GetDatiBeneficiParticolariByIdPensione(datiPensione.Id, datiPensione, out listDatiBeneficiParticolari);
            GestioneAreaOneri.ValorizzaDatiBeneficiParticolariForPrepensionamento(datiPensione, ref listDatiBeneficiParticolari);

            if (listDatiBeneficiParticolari != null)
            {
                if (areaOneri == null)
                    areaOneri = new AreaOneri();

                if (areaOneri.DatiOneriBenefParticolari == null)
                    areaOneri.DatiOneriBenefParticolari = new Entity.Oneri.DatiOneriBenefParticolari();

                areaOneri.DatiOneriBenefParticolari.ListaDatiBeneficiParticolari = listDatiBeneficiParticolari;
            }

            #endregion Benefici Particolari

            #region Benefici Particolari Storico

            List<Entity.Oneri.DatiOneriBenefParticolari.DatiBeneficiParticolari> listDatiBeneficiParticolariStorico = null;
            GestioneAreaOneri.GetDatiBeneficiParticolariStoricoByIdPensione(datiPensione.Id, out listDatiBeneficiParticolariStorico);

            if (listDatiBeneficiParticolariStorico != null && listDatiBeneficiParticolariStorico.Count > 0)
            {
                if (areaOneri == null)
                    areaOneri = new AreaOneri();

                if (areaOneri.DatiOneriBenefParticolariStorico == null)
                    areaOneri.DatiOneriBenefParticolariStorico = new Entity.Oneri.DatiOneriBenefParticolari();

                areaOneri.DatiOneriBenefParticolariStorico.ListaDatiBeneficiParticolari = listDatiBeneficiParticolariStorico;
            }

            #endregion Benefici Particolari Storico

            #endregion Oneri - Benefici Particolari

            #region Prepensionamento

            GestioneAreaOneri.ValorizzaDatiPrepensionamentoForPrepensionamento(datiPensione, ref datiPrepensionamento);
            if (datiPrepensionamento != null)
            {
                if (areaOneri == null)
                    areaOneri = new AreaOneri();
                areaOneri.DatiPrepensionamento = datiPrepensionamento;
            }

            #endregion Prepensionamento

            GetCrossProperties(datiPensione, isRiaperturaDomanda, ref areaOneri);

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        public AreaEsito StoreOneri(long numeroDomanda, AreaOneri areaOneri)
        {
            SetCulture();

            char? derogaTraduzioneSuGP = null;
            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

            if (datiIstruttoria != null && datiIstruttoria.CodiceParticolareSoggettoDerogato.HasValue)
            {
                List<GestioneDecodifica.CodiceParticolare> elencoCodiceParticolareSoggettoDerogato = null;
                GestioneDecodifica.GetCodiciParticolari(out elencoCodiceParticolareSoggettoDerogato);
                if (elencoCodiceParticolareSoggettoDerogato != null && elencoCodiceParticolareSoggettoDerogato.Count > 0)
                {
                    GestioneDecodifica.CodiceParticolare codiceParticolare = elencoCodiceParticolareSoggettoDerogato.Find(x => x.Id == datiIstruttoria.CodiceParticolareSoggettoDerogato.Value);
                    if (codiceParticolare != null)
                        derogaTraduzioneSuGP = codiceParticolare.TraduzioneSuGp;
                }
            }

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;
            GestioneBeneficioVittimeTerrorismo.GetBeneficioVittimeTerrorismoByIdPensione(datiPensione.Id, out datiBeneficioVittimeTerrorismo);
            bool isBeneficioVittimeTerrorismo = Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo);

            GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = null;
            GestioneMaggiorazioniBenefici.GetMaggiorazioniBeneficiByIdPensione(datiPensione.Id, out datiMaggiorazioniBenefici);
            bool isBeneficioENAV = Utility.IsDomandaVecchiaiaENAV(datiPensione);
            bool isBeneficioNonVedente = datiMaggiorazioniBenefici != null && datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "01";

            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            AreaEsito Esito = new AreaEsito();

            #region Oneri - Benefici Particolari

            Esito = StoreDatiOneriBeneficiParticolariPrivate(datiPensione, areaOneri, datiIstruttoria, derogaTraduzioneSuGP, isRiaperturaDomanda, true, isBeneficioVittimeTerrorismo, isBeneficioENAV, isBeneficioNonVedente, datiAnagraficiTitolare, tipoAppartenenza);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            #endregion Oneri - Benefici Particolari

            #region Prepensionamento

            Esito = StoreDatiPrepensionamentoPrivate(datiPensione, areaOneri, isRiaperturaDomanda, false, isBeneficioVittimeTerrorismo, isBeneficioENAV, isBeneficioNonVedente);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            #endregion Prepensionamento

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        #region Oneri

        public AreaEsito StoreDatiOneriBeneficiParticolari(long numeroDomanda, AreaOneri areaOneri)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            char? derogaTraduzioneSuGP = null;

            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

            if (datiIstruttoria != null && datiIstruttoria.CodiceParticolareSoggettoDerogato.HasValue)
            {
                List<GestioneDecodifica.CodiceParticolare> elencoCodiceParticolareSoggettoDerogato = null;
                GestioneDecodifica.GetCodiciParticolari(out elencoCodiceParticolareSoggettoDerogato);
                if (elencoCodiceParticolareSoggettoDerogato != null && elencoCodiceParticolareSoggettoDerogato.Count > 0)
                {
                    GestioneDecodifica.CodiceParticolare codiceParticolare = elencoCodiceParticolareSoggettoDerogato.Find(x => x.Id == datiIstruttoria.CodiceParticolareSoggettoDerogato.Value);
                    if (codiceParticolare != null)
                        derogaTraduzioneSuGP = codiceParticolare.TraduzioneSuGp;
                }
            }

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;
            GestioneBeneficioVittimeTerrorismo.GetBeneficioVittimeTerrorismoByIdPensione(datiPensione.Id, out datiBeneficioVittimeTerrorismo);
            bool isBeneficioVittimeTerrorismo = Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo);

            GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = null;
            GestioneMaggiorazioniBenefici.GetMaggiorazioniBeneficiByIdPensione(datiPensione.Id, out datiMaggiorazioniBenefici);
            bool isBeneficioENAV = Utility.IsDomandaVecchiaiaENAV(datiPensione);
            bool isBeneficioNonVedente = datiMaggiorazioniBenefici != null && datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "01";

            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            AreaEsito Esito = new AreaEsito();
            Esito = StoreDatiOneriBeneficiParticolariPrivate(datiPensione, areaOneri, datiIstruttoria, derogaTraduzioneSuGP, isRiaperturaDomanda, true, isBeneficioVittimeTerrorismo, isBeneficioENAV, isBeneficioNonVedente, datiAnagraficiTitolare, tipoAppartenenza);
            return Esito;
        }

        private AreaEsito StoreDatiOneriBeneficiParticolariPrivate(GestionePensione.DatiPensione datiPensione, AreaOneri areaOneri, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            char? derogaTraduzioneSuGP, bool isRiaperturaDomanda, bool singleTab, bool isBeneficioVittimeTerrorismo, bool isBeneficioENAV, bool isBeneficioNonVedente, GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare, Utility.TipoAppartenenza? tipoAppartenenza)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            if (!singleTab && !GestioneAreaOneri.ControlsVisibleTabs(datiPensione, true, null, isRiaperturaDomanda, isBeneficioVittimeTerrorismo, isBeneficioENAV, isBeneficioNonVedente))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            else
            {
                #region Oneri

                GestioneAreaOneri.ControlsDatiOneri(datiPensione, datiIstruttoria, areaOneri.DatiOneriBenefParticolari.ListaDatiOneri, derogaTraduzioneSuGP, isRiaperturaDomanda, datiAnagraficiTitolare, tipoAppartenenza, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }

                #endregion Oneri

                #region Benefici Particolari

                GestioneAreaOneri.ControlsDatiBeneficiParticolari(areaOneri.DatiOneriBenefParticolari.ListaDatiBeneficiParticolari, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }

                #endregion Benefici Particolari

                GestioneAreaOneri.StoreDatiOneriBeneficiParticolari(datiPensione, isRiaperturaDomanda, areaOneri.DatiOneriBenefParticolari);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }

            return Esito;
        }

        public AreaEsito CancelDatiOneriBeneficiParticolari(long numeroDomanda, out AreaOneri areaOneri)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            areaOneri = new AreaOneri();
            AreaEsito Esito = new AreaEsito();
            GestioneAreaOneri.EliminaDatiOneriBeneficiPaticolari(datiPensione);

            List<Entity.Oneri.DatiOneriBenefParticolari.DatiOneri> listaDatiOneri = null;
            List<Entity.CodiciOneri.GruppoOneri> listaGruppoOneri = null;
            List<Entity.CodiciOneri.SottoGruppoOneri> listaSottoGruppoOneri = null;

            //ENG - Memo 121_2023
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);

            GestioneAreaOneri.GetDatiOneri(datiPensione, isRiaperturaDomanda, out listaDatiOneri, out listaGruppoOneri, out listaSottoGruppoOneri, datiAnagraficiTitolare);
            if (listaDatiOneri != null)
            {
                areaOneri.DatiOneriBenefParticolari.ListaDatiOneri = listaDatiOneri;
            }
            if (listaGruppoOneri != null)
            {
                areaOneri.ListaGruppoOneri = listaGruppoOneri;
            }
            if (listaSottoGruppoOneri != null)
            {
                areaOneri.ListaSottoGruppoOneri = listaSottoGruppoOneri;
            }
            List<Entity.Oneri.DatiOneriBenefParticolari.DatiBeneficiParticolari> listDatiBeneficiParticolari = null;
            GestioneAreaOneri.GetDatiBeneficiParticolariByIdPensione(datiPensione.Id, datiPensione, out listDatiBeneficiParticolari);

            if (listDatiBeneficiParticolari != null)
            {
                if (areaOneri.DatiOneriBenefParticolari == null)
                    areaOneri.DatiOneriBenefParticolari = new Entity.Oneri.DatiOneriBenefParticolari();

                areaOneri.DatiOneriBenefParticolari.ListaDatiBeneficiParticolari = listDatiBeneficiParticolari;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        #endregion Oneri

        #region DatiPrepensionamento

        public AreaEsito StoreDatiPrepensionamento(long numeroDomanda, AreaOneri areaMaggiorazioniBenefici)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;
            GestioneBeneficioVittimeTerrorismo.GetBeneficioVittimeTerrorismoByIdPensione(datiPensione.Id, out datiBeneficioVittimeTerrorismo);
            bool isBeneficioVittimeTerrorismo = Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo);

            GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = null;
            GestioneMaggiorazioniBenefici.GetMaggiorazioniBeneficiByIdPensione(datiPensione.Id, out datiMaggiorazioniBenefici);
            bool isBeneficioENAV = Utility.IsDomandaVecchiaiaENAV(datiPensione);
            bool isBeneficioNonVedente = datiMaggiorazioniBenefici != null && datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "01";

            AreaEsito Esito = new AreaEsito();
            Esito = StoreDatiPrepensionamentoPrivate(datiPensione, areaMaggiorazioniBenefici, isRiaperturaDomanda, true, isBeneficioVittimeTerrorismo, isBeneficioENAV, isBeneficioNonVedente);
            return Esito;
        }

        private AreaEsito StoreDatiPrepensionamentoPrivate(GestionePensione.DatiPensione datiPensione, AreaOneri areaOneri, bool isRiaperturaDomanda, bool singleTab, bool isBeneficioVittimeTerrorismo, bool isBeneficioENAV, bool isBeneficioNonVedente)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            if (!singleTab && !GestioneAreaOneri.ControlsVisibleTabs(datiPensione, null, true, isRiaperturaDomanda, isBeneficioVittimeTerrorismo, isBeneficioENAV, isBeneficioNonVedente))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            else
            {
                GestioneAreaOneri.ControlDatiPrepensionamento(datiPensione, areaOneri.DatiPrepensionamento, false, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }

                GestioneAreaOneri.StoreDatiPrepensionamento(datiPensione, areaOneri.DatiPrepensionamento);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            return Esito;
        }

        public AreaEsito CancelDatiPrepensionamento(long numeroDomanda, out AreaOneri areaMaggiorazioniBenefici)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.GetMaggiorazioniBeneficiByIdPensione(datiPensione.Id, out datiMaggiorazioniBeneficiCommon);

            areaMaggiorazioniBenefici = new AreaOneri();
            AreaEsito Esito = new AreaEsito();
            GestioneAreaOneri.EliminaDatiPrepensionamento(datiPensione);

            Entity.Oneri.DatiPrepensionamento datiPrepensionamento = null;
            GestioneAreaOneri.ValorizzaDatiPrepensionamentoForPrepensionamento(datiPensione, ref datiPrepensionamento);
            if (datiPrepensionamento != null)
                areaMaggiorazioniBenefici.DatiPrepensionamento = datiPrepensionamento;

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        #endregion DatiPrepensionamento

        private void GetCrossProperties(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, ref AreaOneri areaOneri)
        {
            Dictionary<string, bool> lCrossProperties = GestioneAreaOneri.GetCrossProperties(datiPensione, isRiaperturaDomanda);

            if (areaOneri == null)
                areaOneri = new AreaOneri();

            areaOneri.IsBeneficioAmianto = lCrossProperties["IsBeneficioAmianto"];
            areaOneri.IsOneriSperDonnaObbligatori = lCrossProperties["IsOneriSperDonnaObbligatori"];
            areaOneri.IsBeneficioVittimeTerrorismo = lCrossProperties["IsBeneficioVittimeTerrorismo"];
            areaOneri.IsPrepensionamentoEditoriaArt1c154L205_2017 = lCrossProperties["IsPrepensionamentoEditoriaArt1c154L205_2017"];
            areaOneri.IsPrepensionamentoEditoriaArt1c500L160_2019 = lCrossProperties["IsPrepensionamentoEditoriaArt1c500L160_2019"];
            areaOneri.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione = lCrossProperties["IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione"];
            areaOneri.IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione = lCrossProperties["IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione"];
            areaOneri.IsPrepensionamentoEditoria = lCrossProperties["IsPrepensionamentoEditoria"];
            areaOneri.IsOpzioneDonna_Legge197_2022_Art1_Comma292 = lCrossProperties["IsOpzioneDonna_Legge197_2022_Art1_Comma292"];
            areaOneri.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione = lCrossProperties["IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione"];
            areaOneri.IsPrepensionamentoEditoriaLetteraB = lCrossProperties["IsPrepensionamentoEditoriaLetteraB"];
            areaOneri.IsOneriPresentiDaAzienda = lCrossProperties["IsOneriPresentiDaAzienda"];
            areaOneri.IsRicVOPGIMigrataFiltroEBA = lCrossProperties["IsRicVOPGIMigrataFiltroEBA"];
        }

        #endregion AreaOneri

        #region Area RichiestaBonus
        public AreaEsito GetRichiestaBonusByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, string matricolaOperatore, short sedeOperatore, out AreaRichiestaBonus areaRichiestaBonus, out bool IsDataFromDB)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            AreaEsito Esito = new AreaEsito();
            areaRichiestaBonus = null;
            IsDataFromDB = false;
            //ENG - Booking FS-AGO
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(contenitore.DatiPensione.IndConvInt, contenitore.DatiPensione.Gestione);
            List<GestioneAnniRichiestaBonus.DatiAnniRichiestaBonus> datiAnniRichiestaBonus = null;
            GestioneRichiestaBonus.AreaRichiestaBonus richiestaBonus = new GestioneRichiestaBonus.AreaRichiestaBonus();
            richiestaBonus.Certificato = contenitore.DatiPensione.NCertificato.Value.ToString().PadLeft(8, '0');
            richiestaBonus.Categoria = contenitore.DatiPensione.GetCodCategoria().Substring(1, 3);
            if ((tipoAppartenenza == Utility.TipoAppartenenza.FS || tipoAppartenenza == Utility.TipoAppartenenza.AGO) && Utility.IsRicostituzione_Reddituale(contenitore.DatiPensione) && contenitore.DatiPensione.CodiceSedeDestinazione.HasValue)
                richiestaBonus.Sede = contenitore.DatiPensione.CodiceSedeDestinazione.Value.ToString().PadLeft(4, '0');
            else
                richiestaBonus.Sede = contenitore.DatiPensione.CodiceSede.ToString().PadLeft(4, '0');
            if (contenitore.DatiPensione.Tipo == "0167")
            {
                richiestaBonus.TipoBonus = "B14_I";
            }
            else
            {
                richiestaBonus.TipoBonus = "B154_I";
            }

            int annoInizioBonus = 0;
            Int32.TryParse(contenitore.DatiPensione.AnnoDecorrenzaBonus, out annoInizioBonus);

            if (GestioneAnniRichiestaBonus.GetAnniRichiestaBonus(contenitore.DatiPensione.Id, out datiAnniRichiestaBonus))
            {
                IsDataFromDB = true;
                richiestaBonus.DatiAnniRichiestaBonus = datiAnniRichiestaBonus;
                if (contenitore.DatiPensione.IsRichiestaBonus.Value && Utility.GetStatoPensioneByCodice(contenitore.DatiPensione.StatoPensione.Value) == Utility.StatoPensione.Calcolata)
                {
                    List<GestioneAnniRichiestaBonus.DatiPrenotazioneElaborazioni> datiPrenotazioneElaborazioni = null;
                    GestioneAnniRichiestaBonus.GetPrenotazioneElaborazioni(contenitore.DatiPensione.Id, out datiPrenotazioneElaborazioni);
                    richiestaBonus.DatiPrenotazioneElaborazioni = datiPrenotazioneElaborazioni;
                }
            }
            else
            {
                if (GestioneRichiestaBonus.GetAnniDirittoAlBonus(ref richiestaBonus, areaRichiestaDomanda.NumeroDomanda.ToString(), contenitore.DatiPensione.Id))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = string.Empty;

                    AggiornaDatiAnniRichiestaBonus(ref richiestaBonus, contenitore.DatiPensione);
                }
                else
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = richiestaBonus.MessaggioVideo;
                }
            }

            areaRichiestaBonus = new AreaRichiestaBonus();
            areaRichiestaBonus.RichiestaBonus = richiestaBonus;
            areaRichiestaBonus.RichiestaBonus.AnnoInizioBonus = annoInizioBonus;
            areaRichiestaBonus.RichiestaBonus.IsDataFromDB = IsDataFromDB;
            return Esito;
        }

        public AreaEsito StoreDatiRichiestaBonus(long numeroDomanda, ref AreaRichiestaBonus areaRichiestaBonus)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            int annoInizioBonus = 0;
            Int32.TryParse(datiPensione.AnnoDecorrenzaBonus, out annoInizioBonus);

            if (areaRichiestaBonus != null)
            {
                try
                {
                    GestioneAnniRichiestaBonus.SalvaAnniRichiestaBonus(datiPensione.Id, areaRichiestaBonus.RichiestaBonus.DatiAnniRichiestaBonus);
                    List<GestioneAnniRichiestaBonus.DatiAnniRichiestaBonus> datiAnniRichiestaBonus = null;
                    if (GestioneAnniRichiestaBonus.GetAnniRichiestaBonus(datiPensione.Id, out datiAnniRichiestaBonus))
                    {
                        areaRichiestaBonus.RichiestaBonus.DatiAnniRichiestaBonus = datiAnniRichiestaBonus;
                        if (datiAnniRichiestaBonus != null && datiAnniRichiestaBonus.Count > 0)
                            areaRichiestaBonus.RichiestaBonus.IsDataFromDB = true;
                    }

                    areaRichiestaBonus.RichiestaBonus.AnnoInizioBonus = annoInizioBonus;
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = string.Empty;
                }
                catch (Exception Ex)
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = Ex.Message;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                }

            }

            return Esito;
        }

        public AreaEsito EliminaRichiestaBonusByDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, out AreaRichiestaBonus areaRichiestaBonus)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            AreaEsito Esito = new AreaEsito();
            areaRichiestaBonus = null;
            GestioneRichiestaBonus.AreaRichiestaBonus richiestaBonus = new GestioneRichiestaBonus.AreaRichiestaBonus();
            //ENG - Booking FS-AGO
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(contenitore.DatiPensione.IndConvInt, contenitore.DatiPensione.Gestione);

            richiestaBonus.Certificato = contenitore.DatiPensione.NCertificato.Value.ToString().PadLeft(8, '0');
            richiestaBonus.Categoria = contenitore.DatiPensione.GetCodCategoria().Substring(1, 3);
            if ((tipoAppartenenza == Utility.TipoAppartenenza.FS || tipoAppartenenza == Utility.TipoAppartenenza.AGO) && Utility.IsRicostituzione_Reddituale(contenitore.DatiPensione) && contenitore.DatiPensione.CodiceSedeDestinazione.HasValue)
                richiestaBonus.Sede = contenitore.DatiPensione.CodiceSedeDestinazione.Value.ToString().PadLeft(4, '0');
            else
                richiestaBonus.Sede = contenitore.DatiPensione.CodiceSede.ToString().PadLeft(4, '0');
            if (contenitore.DatiPensione.Tipo == "0167")
            {
                richiestaBonus.TipoBonus = "B14_I";
            }
            else
            {
                richiestaBonus.TipoBonus = "B154_I";
            }

            try
            {
                GestioneAnniRichiestaBonus.EliminaAnniRichiestaBonusByIdPensione(contenitore.DatiPensione.Id);
            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }

            if (GestioneRichiestaBonus.GetAnniDirittoAlBonus(ref richiestaBonus, numeroDomanda.ToString(), contenitore.DatiPensione.Id))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;

                AggiornaDatiAnniRichiestaBonus(ref richiestaBonus, contenitore.DatiPensione);
            }
            else
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = richiestaBonus.MessaggioVideo;
            }

            int annoInizioBonus = 0;
            Int32.TryParse(contenitore.DatiPensione.AnnoDecorrenzaBonus, out annoInizioBonus);

            areaRichiestaBonus = new AreaRichiestaBonus();
            areaRichiestaBonus.RichiestaBonus = richiestaBonus;
            areaRichiestaBonus.RichiestaBonus.AnnoInizioBonus = annoInizioBonus;
            areaRichiestaBonus.RichiestaBonus.IsDataFromDB = false;


            return Esito;
        }

        private void AggiornaDatiAnniRichiestaBonus(ref GestioneRichiestaBonus.AreaRichiestaBonus areaRichiestaBonus, GestionePensione.DatiPensione datiPensione)
        {
            int annoInizioBonus = 0;
            Int32.TryParse(datiPensione.AnnoDecorrenzaBonus, out annoInizioBonus);

            if (areaRichiestaBonus.DatiAnniRichiestaBonus != null && areaRichiestaBonus.DatiAnniRichiestaBonus.Count() > 0)
            {
                foreach (GestioneAnniRichiestaBonus.DatiAnniRichiestaBonus annoRichiestaBonus in areaRichiestaBonus.DatiAnniRichiestaBonus)
                {
                    if (annoRichiestaBonus.Anno >= annoInizioBonus && annoRichiestaBonus.Anno <= DateTime.Today.Year)
                        annoRichiestaBonus.IsRichiestaBonus = true;
                }
            }
        }

        #endregion Area RichiestaBonus

        #region Cross Entity Contribuzione Enpals

        public AreaEsito GetDatiContributiviEnpalsByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, TipologiaContribuzioneEnpals tipologia, out BLCommon.Entity.DatiContribuzioneEnpals datiContribuzioneEnpals)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            datiContribuzioneEnpals = null;
            try
            {
                GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);
                GestioneContribuzioneEnpals.GetDatiContribuzioneEnpalsByIdPensioneAndTipologia(datiPensione.Id, tipologia, out datiContribuzioneEnpals);
                if (datiContribuzioneEnpals == null)
                {
                    esito.Messaggio = "Dati Contribuzione Enpals non presenti.";
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;

                }
            }
            catch (Exception ex)
            {
                INPS.DNA.Logging.Logger.LogException(ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore recupero dei dati.";
            }
            return esito;
        }

        public AreaEsito StoreDatiContributiviEnpals(long numeroDomanda, BLCommon.Entity.DatiContribuzioneEnpals datiContribuzioneEnpals)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            try
            {
                GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
                GestioneContribuzioneEnpals.SalvaEntityDatiContributizioneEnpals(datiPensione, datiContribuzioneEnpals);
            }
            catch (Exception ex)
            {
                INPS.DNA.Logging.Logger.LogException(ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore salvataggio dei dati.";
            }
            return esito;
        }

        #endregion Cross Entity Contribuzione Enpals

        #region Area CtrlBypassTipologieNonAbilitate

        public AreaEsito GetAllCtrlBypassTipologieNonAbilitate(Utility.TipoAppartenenza tipoAppRuolo, out AreaCtrlBypassTipologieNonAbilitate areaCtrlBypassTipologieNonAbilitate)
        {
            SetCulture();

            areaCtrlBypassTipologieNonAbilitate = new AreaCtrlBypassTipologieNonAbilitate();
            AreaEsito Esito = new AreaEsito();
            try
            {
                List<BLCommon.GestioneCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate> elencoCtrlBypassTipologieNonAbilitate = null;
                BLCommon.GestioneCtrlBypassTipologieNonAbilitate.GetCtrlBypassTipologieNonAbilitate(out elencoCtrlBypassTipologieNonAbilitate);

                if (elencoCtrlBypassTipologieNonAbilitate != null && elencoCtrlBypassTipologieNonAbilitate.Count > 0)
                {
                    elencoCtrlBypassTipologieNonAbilitate = elencoCtrlBypassTipologieNonAbilitate.FindAll(x => x.Tipologia.Trim() == tipoAppRuolo.ToString().Trim());
                    foreach (BLCommon.GestioneCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate ctrl in elencoCtrlBypassTipologieNonAbilitate)
                        areaCtrlBypassTipologieNonAbilitate.ElencoCtrlBypassTipologieNonAbilitate.Add(new AreaCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate(ctrl));
                }

                List<GestioneDecodifica.Gruppo> elencoGruppo = null;
                GestioneDecodifica.GetGruppo(out elencoGruppo);

                List<GestioneDecodifica.Prodotto> elencoProdotto = null;
                GestioneDecodifica.GetProdotto(out elencoProdotto);

                List<GestioneDecodifica.Tipo> elencoTipo = null;
                GestioneDecodifica.GetTipo(out elencoTipo);

                List<GestioneDecodifica.Filtro> elencoFiltro = null;
                GestioneDecodifica.GetFiltro(out elencoFiltro);

                if (elencoGruppo != null && elencoGruppo.Count > 0)
                {
                    if (areaCtrlBypassTipologieNonAbilitate.ElencoGruppo == null)
                        areaCtrlBypassTipologieNonAbilitate.ElencoGruppo = new List<GestioneDecodifica.Gruppo>();

                    areaCtrlBypassTipologieNonAbilitate.ElencoGruppo = elencoGruppo;
                }

                if (elencoProdotto != null && elencoProdotto.Count > 0)
                {
                    if (areaCtrlBypassTipologieNonAbilitate.ElencoProdotto == null)
                        areaCtrlBypassTipologieNonAbilitate.ElencoProdotto = new List<GestioneDecodifica.Prodotto>();

                    areaCtrlBypassTipologieNonAbilitate.ElencoProdotto = elencoProdotto;
                }

                if (elencoTipo != null && elencoTipo.Count > 0)
                {
                    if (areaCtrlBypassTipologieNonAbilitate.ElencoTipo == null)
                        areaCtrlBypassTipologieNonAbilitate.ElencoTipo = new List<GestioneDecodifica.Tipo>();

                    areaCtrlBypassTipologieNonAbilitate.ElencoTipo = elencoTipo;
                }

                if (elencoFiltro != null && elencoFiltro.Count > 0)
                {
                    if (areaCtrlBypassTipologieNonAbilitate.ElencoFiltro == null)
                        areaCtrlBypassTipologieNonAbilitate.ElencoFiltro = new List<GestioneDecodifica.Filtro>();

                    areaCtrlBypassTipologieNonAbilitate.ElencoFiltro = elencoFiltro;
                }

            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nel recupero dell'elenco dei bypass tipologie non abilitate. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito StoreCtrlBypassTipologieNonAbilitate(AreaCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate datiCtrlBypassTipologieNonAbilitate)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            try
            {
                string messaggioVideo = string.Empty;

                BLCommon.GestioneCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate ctrlBypassTipologieNonAbilitate = new BLCommon.GestioneCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate();
                Utility.ValorizzaOggetti(datiCtrlBypassTipologieNonAbilitate, ctrlBypassTipologieNonAbilitate);

                GestioneAreaCtrlBypassTipologieNonAbilitate.StoreCtrlBypassTipologieNonAbilitate(ctrlBypassTipologieNonAbilitate, out messaggioVideo);

                if (!String.IsNullOrEmpty(messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                    return Esito;
                }
            }
            catch (INPS.DNA.DnaValidationException ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                return Esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nel salvataggio. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito DeleteCtrlTipologieNonAbilitate(AreaCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate datiCtrlBypassTipologieNonAbilitate)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            try
            {
                if (datiCtrlBypassTipologieNonAbilitate == null)
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = "Nessun bypass da eliminare";
                    return Esito;
                }

                BLCommon.GestioneCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate ctrlBypassTipologieNonAbilitate = new BLCommon.GestioneCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate();
                Utility.ValorizzaOggetti(datiCtrlBypassTipologieNonAbilitate, ctrlBypassTipologieNonAbilitate);
                BLCommon.GestioneCtrlBypassTipologieNonAbilitate.EliminaCtrlBypassTipologieNonAbilitate(ctrlBypassTipologieNonAbilitate);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore nell'eliminazione. Riprovare più tardi";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        #endregion Area CtrlBypassTipologieNonAbilitate

        #region Area Aggiornamenti
        #region public methods
        public AreaEsito GetAggiornamenti(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamenti areaAggiornamenti)
        {
            SetCulture();

            areaAggiornamenti = null;
            AreaEsito areaEsito = new AreaEsito();
            areaEsito = GetAggiornamentiPrivate(false, tipoApp, out areaAggiornamenti);

            return areaEsito;
        }

        public AreaEsito SalvaAggiornamento(ref AreaAggiornamenti areaAggiornamenti)
        {
            SetCulture();

            AreaEsito areaEsito = new AreaEsito();
            if (areaAggiornamenti == null || areaAggiornamenti.ElencoAggiornamenti == null || areaAggiornamenti.ElencoAggiornamenti.Count == 0)
            {
                areaEsito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                areaEsito.Messaggio = "Nessun avviso da salvare";
                return areaEsito;
            }

            areaEsito = SalvaAggiornamentiPrivate(ref areaAggiornamenti);

            return areaEsito;
        }

        public AreaEsito DeleteAggiornamento(Utility.TipoAppartenenza? tipoApp, ref AreaAggiornamenti areaAggiornamenti)
        {
            SetCulture();

            AreaEsito areaEsito = new AreaEsito();
            if (areaAggiornamenti == null || areaAggiornamenti.ElencoAggiornamenti == null || areaAggiornamenti.ElencoAggiornamenti.Count == 0)
            {
                areaEsito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                areaEsito.Messaggio = "Nessun aggiornamento da eliminare";
                return areaEsito;
            }

            areaEsito = DeleteAggiornamentiPrivate(tipoApp, ref areaAggiornamenti);

            return areaEsito;
        }
        #endregion public methods

        #region private methods
        private AreaEsito GetAggiornamentiPrivate(bool recuperaAttivi, Utility.TipoAppartenenza? tipoApp, out AreaAggiornamenti areaAggiornamento)
        {
            areaAggiornamento = new AreaAggiornamenti();
            AreaEsito esito = new AreaEsito();
            try
            {
                List<Aggiornamenti> elencoAggiornamenti = null;
                GestioneAggiornamenti.GetAggiornamenti(recuperaAttivi, tipoApp.GetValueOrDefault().ToString(), out elencoAggiornamenti);
                areaAggiornamento.ElencoAggiornamenti = elencoAggiornamenti;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;

        }

        private AreaEsito SalvaAggiornamentiPrivate(ref AreaAggiornamenti areaAggiornamenti)
        {
            AreaEsito esito = new AreaEsito();
            try
            {
                GestioneAggiornamenti.StoreAggiornamenti(areaAggiornamenti.ElencoAggiornamenti.First());

                List<Aggiornamenti> elencoAgg = null;
                GestioneAggiornamenti.GetAggiornamenti(false, areaAggiornamenti.ElencoAggiornamenti.First().Tipologia, out elencoAgg);
                areaAggiornamenti.ElencoAggiornamenti = elencoAgg;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;

        }

        private AreaEsito DeleteAggiornamentiPrivate(Utility.TipoAppartenenza? tipoApp, ref AreaAggiornamenti areaAggiornamenti)
        {
            AreaEsito esito = new AreaEsito();
            try
            {
                GestioneAggiornamenti.DeleteAggiornamento(areaAggiornamenti.ElencoAggiornamenti.First());

                List<Aggiornamenti> elencoAgg = null;
                GestioneAggiornamenti.GetAggiornamenti(false, tipoApp.GetValueOrDefault().ToString(), out elencoAgg);
                areaAggiornamenti.ElencoAggiornamenti = elencoAgg;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;

        }
        #endregion private methods
        #endregion Area Aggiornamenti

        #region Area Aggiornamento
        public AreaEsito GetAreaAggiornamento(Utility.TipoAppartenenza tipoApp, out AreaAggiornamento areaAggiornamento)
        {
            SetCulture();

            bool isAggiornamentoInCorso;
            int? domandeDaElaborare, domandeElaborate, domandeElaborateConErrore, domandeTotali;
            string messaggioVideo = string.Empty;

            areaAggiornamento = new AreaAggiornamento();

            areaAggiornamento.AreaAggiornamentoWebDom = new AreaAggiornamento.AreaAggiornamentoGeneric();
            AreaEsito esito = new AreaEsito();
            if (GestioneAreaAggiornamento.GetAreaAggiornamentoWebDom(tipoApp, out isAggiornamentoInCorso, out domandeDaElaborare, out domandeElaborate, out domandeElaborateConErrore, out domandeTotali,
                out messaggioVideo))
            {
                areaAggiornamento.AreaAggiornamentoWebDom.DomandeDaElaborare = domandeDaElaborare;
                areaAggiornamento.AreaAggiornamentoWebDom.DomandeElaborate = domandeElaborate;
                areaAggiornamento.AreaAggiornamentoWebDom.DomandeElaborateConErrore = domandeElaborateConErrore;
                areaAggiornamento.AreaAggiornamentoWebDom.DomandeDomandeTotali = domandeTotali;
                if (isAggiornamentoInCorso)
                {
                    areaAggiornamento.IsAggiornamentoInCorso = isAggiornamentoInCorso;
                    areaAggiornamento.TipoAggiornamentoInCorso = AreaAggiornamento.TipoAggiornamento.WebDom;
                }
                esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            }
            else
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = messaggioVideo;
            }

            if (!isAggiornamentoInCorso)
            {
                areaAggiornamento.AreaAggiornamentoFelpe = new AreaAggiornamento.AreaAggiornamentoGeneric();
                esito = new AreaEsito();
                if (GestioneAreaAggiornamento.GetAreaAggiornamentoFelpe(tipoApp, out isAggiornamentoInCorso, out domandeDaElaborare, out domandeElaborate, out domandeElaborateConErrore, out domandeTotali,
                    out messaggioVideo))
                {
                    areaAggiornamento.AreaAggiornamentoFelpe.DomandeDaElaborare = domandeDaElaborare;
                    areaAggiornamento.AreaAggiornamentoFelpe.DomandeElaborate = domandeElaborate;
                    areaAggiornamento.AreaAggiornamentoFelpe.DomandeElaborateConErrore = domandeElaborateConErrore;
                    areaAggiornamento.AreaAggiornamentoFelpe.DomandeDomandeTotali = domandeTotali;
                    if (isAggiornamentoInCorso)
                    {
                        areaAggiornamento.IsAggiornamentoInCorso = isAggiornamentoInCorso;
                        areaAggiornamento.TipoAggiornamentoInCorso = AreaAggiornamento.TipoAggiornamento.Felpe;
                    }
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                }
                else
                {
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    esito.Messaggio = messaggioVideo;
                }
            }

            if (!isAggiornamentoInCorso)
            {
                areaAggiornamento.AreaAggiornamentoOneri = new AreaAggiornamento.AreaAggiornamentoGeneric();
                esito = new AreaEsito();
                if (GestioneAreaAggiornamento.GetAreaAggiornamentoOneri(tipoApp, out isAggiornamentoInCorso, out domandeDaElaborare, out domandeElaborate, out domandeElaborateConErrore, out domandeTotali,
                    out messaggioVideo))
                {
                    areaAggiornamento.AreaAggiornamentoOneri.DomandeDaElaborare = domandeDaElaborare;
                    areaAggiornamento.AreaAggiornamentoOneri.DomandeElaborate = domandeElaborate;
                    areaAggiornamento.AreaAggiornamentoOneri.DomandeElaborateConErrore = domandeElaborateConErrore;
                    areaAggiornamento.AreaAggiornamentoOneri.DomandeDomandeTotali = domandeTotali;
                    if (isAggiornamentoInCorso)
                    {
                        areaAggiornamento.IsAggiornamentoInCorso = isAggiornamentoInCorso;
                        areaAggiornamento.TipoAggiornamentoInCorso = AreaAggiornamento.TipoAggiornamento.Oneri;
                    }
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                }
                else
                {
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    esito.Messaggio = messaggioVideo;
                }
            }

            if (!isAggiornamentoInCorso)
            {
                areaAggiornamento.AreaAggiornamentoCumulo = new AreaAggiornamento.AreaAggiornamentoGeneric();
                esito = new AreaEsito();
                if (GestioneAreaAggiornamento.GetAreaAggiornamentoCumulo(tipoApp, out isAggiornamentoInCorso, out domandeDaElaborare, out domandeElaborate, out domandeElaborateConErrore, out domandeTotali,
                    out messaggioVideo))
                {
                    areaAggiornamento.AreaAggiornamentoCumulo.DomandeDaElaborare = domandeDaElaborare;
                    areaAggiornamento.AreaAggiornamentoCumulo.DomandeElaborate = domandeElaborate;
                    areaAggiornamento.AreaAggiornamentoCumulo.DomandeElaborateConErrore = domandeElaborateConErrore;
                    areaAggiornamento.AreaAggiornamentoCumulo.DomandeDomandeTotali = domandeTotali;
                    if (isAggiornamentoInCorso)
                    {
                        areaAggiornamento.IsAggiornamentoInCorso = isAggiornamentoInCorso;
                        areaAggiornamento.TipoAggiornamentoInCorso = AreaAggiornamento.TipoAggiornamento.Cumulo;
                    }
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                }
                else
                {
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    esito.Messaggio = messaggioVideo;
                }
            }

            if (!isAggiornamentoInCorso)
            {
                areaAggiornamento.AreaAggiornamentoTot = new AreaAggiornamento.AreaAggiornamentoGeneric();
                esito = new AreaEsito();
                if (GestioneAreaAggiornamento.GetAreaAggiornamentoTot(tipoApp, out isAggiornamentoInCorso, out domandeDaElaborare, out domandeElaborate, out domandeElaborateConErrore, out domandeTotali,
                    out messaggioVideo))
                {
                    areaAggiornamento.AreaAggiornamentoTot.DomandeDaElaborare = domandeDaElaborare;
                    areaAggiornamento.AreaAggiornamentoTot.DomandeElaborate = domandeElaborate;
                    areaAggiornamento.AreaAggiornamentoTot.DomandeElaborateConErrore = domandeElaborateConErrore;
                    areaAggiornamento.AreaAggiornamentoTot.DomandeDomandeTotali = domandeTotali;
                    if (isAggiornamentoInCorso)
                    {
                        areaAggiornamento.IsAggiornamentoInCorso = isAggiornamentoInCorso;
                        areaAggiornamento.TipoAggiornamentoInCorso = AreaAggiornamento.TipoAggiornamento.Tot;
                    }
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                }
                else
                {
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    esito.Messaggio = messaggioVideo;
                }
            }

            if (!isAggiornamentoInCorso)
            {
                areaAggiornamento.AreaAggiornamentoSAI = new AreaAggiornamento.AreaAggiornamentoGeneric();
                esito = new AreaEsito();
                if (GestioneAreaAggiornamento.GetAreaAggiornamentoSAI(tipoApp, out isAggiornamentoInCorso, out domandeDaElaborare, out domandeElaborate, out domandeElaborateConErrore, out domandeTotali,
                    out messaggioVideo))
                {
                    areaAggiornamento.AreaAggiornamentoSAI.DomandeDaElaborare = domandeDaElaborare;
                    areaAggiornamento.AreaAggiornamentoSAI.DomandeElaborate = domandeElaborate;
                    areaAggiornamento.AreaAggiornamentoSAI.DomandeElaborateConErrore = domandeElaborateConErrore;
                    areaAggiornamento.AreaAggiornamentoSAI.DomandeDomandeTotali = domandeTotali;
                    if (isAggiornamentoInCorso)
                    {
                        areaAggiornamento.IsAggiornamentoInCorso = isAggiornamentoInCorso;
                        areaAggiornamento.TipoAggiornamentoInCorso = AreaAggiornamento.TipoAggiornamento.SAI;
                    }
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                }
                else
                {
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    esito.Messaggio = messaggioVideo;
                }
            }

            if (!isAggiornamentoInCorso)
            {
                areaAggiornamento.AreaAggiornamentoINPDAP = new AreaAggiornamento.AreaAggiornamentoGeneric();
                esito = new AreaEsito();
                if (GestioneAreaAggiornamento.GetAreaAggiornamentoINPDAP(tipoApp, out isAggiornamentoInCorso, out domandeDaElaborare, out domandeElaborate, out domandeElaborateConErrore, out domandeTotali,
                    out messaggioVideo))
                {
                    areaAggiornamento.AreaAggiornamentoINPDAP.DomandeDaElaborare = domandeDaElaborare;
                    areaAggiornamento.AreaAggiornamentoINPDAP.DomandeElaborate = domandeElaborate;
                    areaAggiornamento.AreaAggiornamentoINPDAP.DomandeElaborateConErrore = domandeElaborateConErrore;
                    areaAggiornamento.AreaAggiornamentoINPDAP.DomandeDomandeTotali = domandeTotali;
                    if (isAggiornamentoInCorso)
                    {
                        areaAggiornamento.IsAggiornamentoInCorso = isAggiornamentoInCorso;
                        areaAggiornamento.TipoAggiornamentoInCorso = AreaAggiornamento.TipoAggiornamento.INPDAP;
                    }
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                }
                else
                {
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    esito.Messaggio = messaggioVideo;
                }
            }

            if (!isAggiornamentoInCorso)
            {
                areaAggiornamento.AreaAggiornamentoNoteDiDebito = new AreaAggiornamento.AreaAggiornamentoGeneric();
                esito = new AreaEsito();
                if (GestioneAreaAggiornamento.GetAreaAggiornamentoNoteDiDebito(tipoApp, out isAggiornamentoInCorso, out domandeDaElaborare, out domandeElaborate, out domandeElaborateConErrore, out domandeTotali,
                    out messaggioVideo))
                {
                    areaAggiornamento.AreaAggiornamentoNoteDiDebito.DomandeDaElaborare = domandeDaElaborare;
                    areaAggiornamento.AreaAggiornamentoNoteDiDebito.DomandeElaborate = domandeElaborate;
                    areaAggiornamento.AreaAggiornamentoNoteDiDebito.DomandeElaborateConErrore = domandeElaborateConErrore;
                    areaAggiornamento.AreaAggiornamentoNoteDiDebito.DomandeDomandeTotali = domandeTotali;
                    if (isAggiornamentoInCorso)
                    {
                        areaAggiornamento.IsAggiornamentoInCorso = isAggiornamentoInCorso;
                        areaAggiornamento.TipoAggiornamentoInCorso = AreaAggiornamento.TipoAggiornamento.NoteDiDebito;
                    }
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                }
                else
                {
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    esito.Messaggio = messaggioVideo;
                }
            }

            if (!isAggiornamentoInCorso)
            {
                areaAggiornamento.AreaAggiornamentoPianiDiPagamento = new AreaAggiornamento.AreaAggiornamentoGeneric();
                esito = new AreaEsito();
                if (GestioneAreaAggiornamento.GetAreaAggiornamentoPianiDiPagamento(tipoApp, out isAggiornamentoInCorso, out domandeDaElaborare, out domandeElaborate, out domandeElaborateConErrore, out domandeTotali,
                    out messaggioVideo))
                {
                    areaAggiornamento.AreaAggiornamentoPianiDiPagamento.DomandeDaElaborare = domandeDaElaborare;
                    areaAggiornamento.AreaAggiornamentoPianiDiPagamento.DomandeElaborate = domandeElaborate;
                    areaAggiornamento.AreaAggiornamentoPianiDiPagamento.DomandeElaborateConErrore = domandeElaborateConErrore;
                    areaAggiornamento.AreaAggiornamentoPianiDiPagamento.DomandeDomandeTotali = domandeTotali;
                    if (isAggiornamentoInCorso)
                    {
                        areaAggiornamento.IsAggiornamentoInCorso = isAggiornamentoInCorso;
                        areaAggiornamento.TipoAggiornamentoInCorso = AreaAggiornamento.TipoAggiornamento.PianiDiPagamento;
                    }
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                }
                else
                {
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    esito.Messaggio = messaggioVideo;
                }
            }

            return esito;
        }

        public AreaEsito CaricaPdfAggiornamentoWebDom(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamento)
        {
            SetCulture();

            areaAggiornamento = null;
            AreaEsito areaEsito = new AreaEsito();
            areaEsito = CaricaPdfAggiornamentoWebDomPrivate(tipoApp, out areaAggiornamento);

            return areaEsito;
        }

        private AreaEsito CaricaPdfAggiornamentoWebDomPrivate(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamento)
        {
            areaAggiornamento = new AreaAggiornamento();
            areaAggiornamento.AreaAggiornamentoWebDom = new AreaAggiornamento.AreaAggiornamentoGeneric();
            AreaEsito esito = new AreaEsito();
            try
            {
                MemoryStream memStream = null;
                GestioneStampeFAQ.CaricaPdfAggiornamento<GestioneEsitoAggiornamentoWebDom.EsitoAggiornamentiWebDom>(tipoApp.GetValueOrDefault(), out memStream);
                areaAggiornamento.AreaAggiornamentoWebDom.PdfDoc = memStream;
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito CaricaPdfAggiornamentoFelpe(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamento)
        {
            SetCulture();

            areaAggiornamento = null;
            AreaEsito areaEsito = new AreaEsito();
            areaEsito = CaricaPdfAggiornamentoFelpePrivate(tipoApp, out areaAggiornamento);

            return areaEsito;
        }

        private AreaEsito CaricaPdfAggiornamentoFelpePrivate(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamento)
        {
            areaAggiornamento = new AreaAggiornamento();
            areaAggiornamento.AreaAggiornamentoFelpe = new AreaAggiornamento.AreaAggiornamentoGeneric();
            AreaEsito esito = new AreaEsito();
            try
            {
                MemoryStream memStream = null;
                GestioneStampeFAQ.CaricaPdfAggiornamento<GestioneEsitoAggiornamentoFelpe.EsitoAggiornamentiFelpe>(tipoApp.GetValueOrDefault(), out memStream);
                areaAggiornamento.AreaAggiornamentoFelpe.PdfDoc = memStream;
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito CaricaPdfAggiornamentoOneri(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamento)
        {
            SetCulture();

            areaAggiornamento = null;
            AreaEsito areaEsito = new AreaEsito();
            areaEsito = CaricaPdfAggiornamentoOneriPrivate(tipoApp, out areaAggiornamento);

            return areaEsito;
        }

        private AreaEsito CaricaPdfAggiornamentoOneriPrivate(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamento)
        {
            areaAggiornamento = new AreaAggiornamento();
            areaAggiornamento.AreaAggiornamentoOneri = new AreaAggiornamento.AreaAggiornamentoGeneric();
            AreaEsito esito = new AreaEsito();
            try
            {
                MemoryStream memStream = null;
                GestioneStampeFAQ.CaricaPdfAggiornamento<GestioneEsitoAggiornamentoOneri.EsitoAggiornamentiOneri>(tipoApp.GetValueOrDefault(), out memStream);
                areaAggiornamento.AreaAggiornamentoOneri.PdfDoc = memStream;
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito CaricaPdfAggiornamentoCumulo(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamento)
        {
            SetCulture();

            areaAggiornamento = null;
            AreaEsito areaEsito = new AreaEsito();
            areaEsito = CaricaPdfAggiornamentoCumuloPrivate(tipoApp, out areaAggiornamento);

            return areaEsito;
        }

        public AreaEsito CaricaPdfAggiornamentoTot(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamento)
        {
            SetCulture();

            areaAggiornamento = null;
            AreaEsito areaEsito = new AreaEsito();
            areaEsito = CaricaPdfAggiornamentoTotPrivate(tipoApp, out areaAggiornamento);

            return areaEsito;
        }

        private AreaEsito CaricaPdfAggiornamentoCumuloPrivate(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamento)
        {
            areaAggiornamento = new AreaAggiornamento();
            areaAggiornamento.AreaAggiornamentoCumulo = new AreaAggiornamento.AreaAggiornamentoGeneric();
            AreaEsito esito = new AreaEsito();
            try
            {
                MemoryStream memStream = null;
                GestioneStampeFAQ.CaricaPdfAggiornamento<GestioneEsitoAggiornamentoCumulo.EsitoAggiornamentiCumulo>(tipoApp.GetValueOrDefault(), out memStream);
                areaAggiornamento.AreaAggiornamentoCumulo.PdfDoc = memStream;
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        private AreaEsito CaricaPdfAggiornamentoTotPrivate(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamento)
        {
            areaAggiornamento = new AreaAggiornamento();
            areaAggiornamento.AreaAggiornamentoTot = new AreaAggiornamento.AreaAggiornamentoGeneric();
            AreaEsito esito = new AreaEsito();
            try
            {
                MemoryStream memStream = null;
                GestioneStampeFAQ.CaricaPdfAggiornamento<GestioneEsitoAggiornamentoTot.EsitoAggiornamentiTot>(tipoApp.GetValueOrDefault(), out memStream);
                areaAggiornamento.AreaAggiornamentoTot.PdfDoc = memStream;
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito CaricaPdfAggiornamentoSAI(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamento)
        {
            SetCulture();

            areaAggiornamento = null;
            AreaEsito areaEsito = new AreaEsito();
            areaEsito = CaricaPdfAggiornamentoSAIPrivate(tipoApp, out areaAggiornamento);

            return areaEsito;
        }

        private AreaEsito CaricaPdfAggiornamentoSAIPrivate(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamento)
        {
            areaAggiornamento = new AreaAggiornamento();
            areaAggiornamento.AreaAggiornamentoSAI = new AreaAggiornamento.AreaAggiornamentoGeneric();
            AreaEsito esito = new AreaEsito();
            try
            {
                MemoryStream memStream = null;
                GestioneStampeFAQ.CaricaPdfAggiornamento<GestioneEsitoAggiornamentoSAI.EsitoAggiornamentiSAI>(tipoApp.GetValueOrDefault(), out memStream);
                areaAggiornamento.AreaAggiornamentoSAI.PdfDoc = memStream;
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito CaricaPdfAggiornamentoINPDAP(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamento)
        {
            SetCulture();

            areaAggiornamento = null;
            AreaEsito areaEsito = new AreaEsito();
            areaEsito = CaricaPdfAggiornamentoINPDAPPrivate(tipoApp, out areaAggiornamento);

            return areaEsito;
        }

        private AreaEsito CaricaPdfAggiornamentoINPDAPPrivate(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamento)
        {
            areaAggiornamento = new AreaAggiornamento();
            areaAggiornamento.AreaAggiornamentoINPDAP = new AreaAggiornamento.AreaAggiornamentoGeneric();
            AreaEsito esito = new AreaEsito();
            try
            {
                MemoryStream memStream = null;
                GestioneStampeFAQ.CaricaPdfAggiornamento<GestioneEsitoAggiornamentoINPDAP.EsitoAggiornamentiINPDAP>(tipoApp.GetValueOrDefault(), out memStream);
                areaAggiornamento.AreaAggiornamentoINPDAP.PdfDoc = memStream;
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito CaricaPdfAggiornamentoNoteDiDebito(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamento)
        {
            SetCulture();

            areaAggiornamento = null;
            AreaEsito areaEsito = new AreaEsito();
            areaEsito = CaricaPdfAggiornamentoNoteDiDebitoPrivate(tipoApp, out areaAggiornamento);

            return areaEsito;
        }

        private AreaEsito CaricaPdfAggiornamentoNoteDiDebitoPrivate(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamento)
        {
            areaAggiornamento = new AreaAggiornamento();
            areaAggiornamento.AreaAggiornamentoNoteDiDebito = new AreaAggiornamento.AreaAggiornamentoGeneric();
            AreaEsito esito = new AreaEsito();
            try
            {
                MemoryStream memStream = null;
                GestioneStampeFAQ.CaricaPdfAggiornamento<GestioneEsitoAggiornamentoNoteDiDebito.EsitoAggiornamentiNoteDiDebito>(tipoApp.GetValueOrDefault(), out memStream);
                areaAggiornamento.AreaAggiornamentoNoteDiDebito.PdfDoc = memStream;
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito CaricaPdfAggiornamentoPianiDiPagamento(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamento)
        {
            SetCulture();

            areaAggiornamento = null;
            AreaEsito areaEsito = new AreaEsito();
            areaEsito = CaricaPdfAggiornamentoPianiDiPagamentoPrivate(tipoApp, out areaAggiornamento);

            return areaEsito;
        }

        private AreaEsito CaricaPdfAggiornamentoPianiDiPagamentoPrivate(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamento)
        {
            areaAggiornamento = new AreaAggiornamento();
            areaAggiornamento.AreaAggiornamentoPianiDiPagamento = new AreaAggiornamento.AreaAggiornamentoGeneric();
            AreaEsito esito = new AreaEsito();
            try
            {
                MemoryStream memStream = null;
                GestioneStampeFAQ.CaricaPdfAggiornamento<GestioneEsitoAggiornamentoPianiDiPagamento.EsitoAggiornamentiPianiDiPagamento>(tipoApp.GetValueOrDefault(), out memStream);
                areaAggiornamento.AreaAggiornamentoPianiDiPagamento.PdfDoc = memStream;
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public void ElaboraAggiornamentoWebDom(Utility.TipoAppartenenza tipoApp)
        {
            SetCulture();

            GestioneAreaAggiornamento.ElaboraDomandeWebDom(tipoApp);
        }

        public void ElaboraAggiornamentoFelpe(Utility.TipoAppartenenza tipoApp)
        {
            SetCulture();

            GestioneAreaAggiornamento.ElaboraDomandeFelpe(tipoApp);
        }

        public void ElaboraAggiornamentoOneri(Utility.TipoAppartenenza tipoApp)
        {
            SetCulture();

            GestioneAreaAggiornamento.ElaboraDomandeOneri(tipoApp);
        }

        public void ElaboraAggiornamentoCumulo(Utility.TipoAppartenenza tipoApp)
        {
            SetCulture();

            GestioneAreaAggiornamento.ElaboraDomandeCumulo(tipoApp);
        }

        public void ElaboraAggiornamentoTot(Utility.TipoAppartenenza tipoApp)
        {
            SetCulture();

            GestioneAreaAggiornamento.ElaboraDomandeTot(tipoApp);
        }

        public void ElaboraAggiornamentoSAI(Utility.TipoAppartenenza tipoApp)
        {
            SetCulture();

            GestioneAreaAggiornamento.ElaboraDomandeSAI(tipoApp);
        }

        public void ElaboraAggiornamentoINPDAP(Utility.TipoAppartenenza tipoApp)
        {
            SetCulture();

            GestioneAreaAggiornamento.ElaboraDomandeINPDAP(tipoApp);
        }

        public void ElaboraAggiornamentoNoteDiDebito(Utility.TipoAppartenenza tipoApp)
        {
            SetCulture();

            GestioneAreaAggiornamento.ElaboraDomandeNoteDiDebito(tipoApp);
        }
        public void ElaboraAggiornamentoPianiDiPagamento(Utility.TipoAppartenenza tipoApp)
        {
            SetCulture();

            GestioneAreaAggiornamento.ElaboraDomandePianiDiPagamento(tipoApp);
        }
        #endregion Area Aggiornamento

        #region Area Periodi
        public AreaEsito GetAreaPeriodiByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaPeriodi areaPeriodi)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            areaPeriodi = new AreaPeriodi();
            string messaggioVideo = string.Empty;

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            Entity.PeriodoAventiDiritto datiPeriodiAventiDiritto = null;
            GestioneAreaPeriodiAventiDiritto.GetAreaPeriodiAventiDirittoByDatiPensione(datiPensione, out datiPeriodiAventiDiritto, out messaggioVideo);

            areaPeriodi.DatiPeriodi = datiPeriodiAventiDiritto;

            GetListeDecodificaAreaPeriodi(datiPensione, ref areaPeriodi);

            if (!string.IsNullOrEmpty(messaggioVideo))
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = messaggioVideo;
            }
            else
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                esito.Messaggio = string.Empty;
            }

            return esito;
        }

        private void GetListeDecodificaAreaPeriodi(GestionePensione.DatiPensione datiPensione, ref AreaPeriodi areaPeriodi)
        {
            List<GestioneAreaFamiliari.AreaDecFam.DatiSiglaFamiliare> elencoGradoParentela = null;
            GestioneAreaPeriodiAventiDiritto.GetDecodificaGradoParentela(datiPensione, out elencoGradoParentela);
            if (elencoGradoParentela != null && elencoGradoParentela.Count > 0)
            {
                if (areaPeriodi == null)
                    areaPeriodi = new AreaPeriodi();
                areaPeriodi.ElencoGradiParentela = elencoGradoParentela;
            }
        }

        public AreaEsito SalvaDatiPeriodiByDomanda(long numeroDomanda, AreaPeriodi areaPeriodi)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            Esito = StoreDatiPeriodiPrivate(datiPensione, areaPeriodi);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            return Esito;
        }

        /// <summary>
        /// Metodo MultiTab per il salvataggio del quadro Periodi
        /// </summary>
        /// <param name="numeroDomanda"></param>
        /// <param name="areaPeriodi"></param>
        /// <returns></returns>
        public AreaEsito StorePeriodi(long numeroDomanda, AreaPeriodi areaPeriodi)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            Esito = StoreDatiPeriodiPrivate(datiPensione, areaPeriodi);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            return Esito;
        }

        private AreaEsito StoreDatiPeriodiPrivate(GestionePensione.DatiPensione datiPensione, AreaPeriodi areaPeriodi)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioVideo = string.Empty;

            List<GestioneAventiDiritto.AventiDiritto> listaAventiDiritto = null;
            GestioneAventiDiritto.GetAventiDirittoByIdPensione(datiPensione.Id, out listaAventiDiritto);

            GestioneAnagrafica.DatiAnagrafici anagraficaDanteCausa = null;
            BLCommon.GestioneDanteCausa.GetAnagraficaDanteCausabyIdPensione(datiPensione.Id, out anagraficaDanteCausa);

            //ENG - Spacchettate SOPGI
            BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            if (areaPeriodi != null)
            {
                bool isAventeDirittoTitolareIncongruente = listaAventiDiritto.Count(x => x.IdAnagrafica == areaPeriodi.DatiPeriodi.DatiAnagraficiAventeDiritto.Id) > 1;
                GestioneAventiDiritto.AventiDiritto aventeDiritto = isAventeDirittoTitolareIncongruente ? listaAventiDiritto.Find(x => x.IdAnagrafica == areaPeriodi.DatiPeriodi.DatiAnagraficiAventeDiritto.Id && x.PresenzaWebDom) :
                    listaAventiDiritto.Find(x => x.IdAnagrafica == areaPeriodi.DatiPeriodi.DatiAnagraficiAventeDiritto.Id);

                if (!GestioneAreaPeriodiAventiDiritto.ControlsDatiPeriodiAventiDiritto(datiPensione, areaPeriodi.DatiPeriodi.ListaPeriodiAventeDiritto, areaPeriodi.DatiPeriodi.DatiAnagraficiAventeDiritto,
                    aventeDiritto, anagraficaDanteCausa, isAventeDirittoTitolareIncongruente, isRiaperturaDomanda, danteCausa, out messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                    return Esito;
                }
                GestioneAreaPeriodiAventiDiritto.StoreDatiPeriodi(datiPensione, areaPeriodi.DatiPeriodi);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }

            return Esito;
        }

        public AreaEsito DeleteDatiPeriodi(long numeroDomanda, ref AreaPeriodi areaPeriodi)
        {
            SetCulture();

            string messaggioVideo = string.Empty;
            AreaEsito Esito = new AreaEsito();

            try
            {
                GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

                GestioneAreaPeriodiAventiDiritto.DeleteDatiPeriodi(datiPensione, areaPeriodi.DatiPeriodi);

                Entity.PeriodoAventiDiritto datiPeriodiAventiDiritto = null;
                GestioneAreaPeriodiAventiDiritto.GetAreaPeriodiAventiDirittoByDatiPensione(datiPensione, out datiPeriodiAventiDiritto, out messaggioVideo);

                areaPeriodi.DatiPeriodi = datiPeriodiAventiDiritto;
                GetListeDecodificaAreaPeriodi(datiPensione, ref areaPeriodi);
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }

            if (!string.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
            }
            else
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }

            return Esito;
        }
        #endregion Area Periodi

        #region Aventi Diritto
        public AreaEsito GetAreaAventiDirittoByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaAventiDiritto areaAventiDiritto)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            areaAventiDiritto = new AreaAventiDiritto();
            string messaggioVideo = string.Empty;

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            GestioneDanteCausa.GetDanteCausaByIdPensione(datiPensione.Id, out datiDanteCausa);

            if (datiDanteCausa != null)
                areaAventiDiritto.IsFascicoloGenerato = datiDanteCausa.IsFascicoloGenerato.GetValueOrDefault();

            Entity.AventiDiritto areaAventiDirittoBL = null;
            GestioneAreaAventiDiritto.GetAventiDirittoConAnagraficheByDatiPensione(datiPensione, out areaAventiDirittoBL, out messaggioVideo);
            areaAventiDiritto.DatiAventiDiritto = areaAventiDirittoBL;

            GetListeDecodificaAreaAventiDiritto(datiPensione, ref areaAventiDiritto);

            if (!string.IsNullOrEmpty(messaggioVideo))
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = messaggioVideo;
            }
            else
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                esito.Messaggio = string.Empty;
            }
            return esito;
        }

        private void GetListeDecodificaAreaAventiDiritto(GestionePensione.DatiPensione datiPensione, ref AreaAventiDiritto areaAventiDiritto)
        {
            List<GestioneAreaFamiliari.AreaDecFam.DatiSiglaFamiliare> elencoGradoParentela = null;
            GestioneAreaPeriodiAventiDiritto.GetDecodificaGradoParentela(datiPensione, out elencoGradoParentela);
            if (elencoGradoParentela != null && elencoGradoParentela.Count > 0)
            {
                if (areaAventiDiritto == null)
                    areaAventiDiritto = new AreaAventiDiritto();
                areaAventiDiritto.ElencoGradiParentela = elencoGradoParentela;
            }
        }

        public AreaEsito SalvaDatiAventiDirittoByDomanda(long numeroDomanda, ref AreaAventiDiritto areaAventiDiritto)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            GestioneAnagrafica.DatiAnagrafici anagraficaTitolare = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out anagraficaTitolare);

            GestioneAnagrafica.DatiAnagrafici anagraficaDanteCausa = null;
            BLCommon.GestioneDanteCausa.GetAnagraficaDanteCausabyIdPensione(datiPensione.Id, out anagraficaDanteCausa);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);

            Esito = StoreDatiAventiDirittoPrivate(datiPensione, anagraficaTitolare, anagraficaDanteCausa, areaAventiDiritto, isRiaperturaDomanda, danteCausa);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            string messaggioVideo = string.Empty;
            Entity.AventiDiritto areaAventiDirittoBL = null;
            GestioneAreaAventiDiritto.GetAventiDirittoConAnagraficheByDatiPensione(datiPensione, out areaAventiDirittoBL, out messaggioVideo);
            areaAventiDiritto.DatiAventiDiritto = areaAventiDirittoBL;

            GetListeDecodificaAreaAventiDiritto(datiPensione, ref areaAventiDiritto);

            if (!string.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
            }
            else
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }

            return Esito;
        }

        /// <summary>
        /// Metodo MultiTab per il salvataggio del quadro Periodi
        /// </summary>
        /// <param name="numeroDomanda"></param>
        /// <param name="areaAventiDiritto"></param>
        /// <returns></returns>
        public AreaEsito StoreAventiDiritto(long numeroDomanda, ref AreaAventiDiritto areaAventiDiritto)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            GestioneAnagrafica.DatiAnagrafici anagraficaTitolare = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out anagraficaTitolare);

            GestioneAnagrafica.DatiAnagrafici anagraficaDanteCausa = null;
            BLCommon.GestioneDanteCausa.GetAnagraficaDanteCausabyIdPensione(datiPensione.Id, out anagraficaDanteCausa);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);

            Esito = StoreDatiAventiDirittoPrivate(datiPensione, anagraficaTitolare, anagraficaDanteCausa, areaAventiDiritto, isRiaperturaDomanda, danteCausa);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            string messaggioVideo = string.Empty;
            Entity.AventiDiritto areaAventiDirittoBL = null;
            GestioneAreaAventiDiritto.GetAventiDirittoConAnagraficheByDatiPensione(datiPensione, out areaAventiDirittoBL, out messaggioVideo);
            areaAventiDiritto.DatiAventiDiritto = areaAventiDirittoBL;

            GetListeDecodificaAreaAventiDiritto(datiPensione, ref areaAventiDiritto);

            if (!string.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
            }
            else
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }

            return Esito;
        }

        private AreaEsito StoreDatiAventiDirittoPrivate(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici anagraficaTitolare, GestioneAnagrafica.DatiAnagrafici anagraficaDanteCausa,
            AreaAventiDiritto areaAventiDiritto, bool isRiaperturaDomanda, BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioVideo = string.Empty;

            if (areaAventiDiritto != null)
            {
                if (!GestioneAreaAventiDiritto.ControlsDatiAventiDiritto(datiPensione, anagraficaTitolare, areaAventiDiritto.DatiAventiDiritto, anagraficaDanteCausa, isRiaperturaDomanda, danteCausa, out messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                    return Esito;
                }
                GestioneAreaAventiDiritto.StoreAventiDiritto(datiPensione, areaAventiDiritto.DatiAventiDiritto);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }

            return Esito;
        }

        public AreaEsito AggiornaAventiDirittoFromWebDom(long numeroDomanda, short sedeOperatore, string matricolaOperatore, out AreaAventiDiritto areaAventiDiritto)
        {
            SetCulture();

            AreaEsito esito = null;
            areaAventiDiritto = new AreaAventiDiritto();
            string messaggioVideo = string.Empty;

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            #region valorizzazione parametri ARCA
            Entity.ParametriARCA parametriArca = null;
            if (!ValorizzaParametriARCA(matricolaOperatore, out parametriArca))
            {
                esito = new AreaEsito();
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero delle informazioni anagrafiche";
                string messaggio = "Errore nella valorizzazione dei parametri ARCA";
                string parametri = string.Format("Matricola Operatore: {0}", matricolaOperatore);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, null);
                return esito;
            }
            #endregion valorizzazione parametri ARCA

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);

            if (!GestioneAreaAventiDiritto.AggiornaAventiDirittoFromWebDom(parametriArca, datiPensione, datiAnagrafici.CodiceFiscale, out messaggioVideo))
            {
                esito = new AreaEsito();
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = messaggioVideo;
            }

            Entity.AventiDiritto areaAventiDirittoBL = null;
            GestioneAreaAventiDiritto.GetAventiDirittoConAnagraficheByDatiPensione(datiPensione, out areaAventiDirittoBL, out messaggioVideo);
            areaAventiDiritto.DatiAventiDiritto = areaAventiDirittoBL;

            GetListeDecodificaAreaAventiDiritto(datiPensione, ref areaAventiDiritto);

            if (esito == null)
            {
                esito = new AreaEsito();
                if (!string.IsNullOrEmpty(messaggioVideo))
                {
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    esito.Messaggio = messaggioVideo;
                }
                else
                {
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    esito.Messaggio = string.Empty;
                }
            }
            return esito;
        }

        public AreaEsito AggiornaAventiDirittoFromArchivioPensione(long numeroDomanda, short sedeOperatore, short centroOperativoOperatore, string matricolaOperatore, out AreaAventiDiritto areaAventiDiritto)
        {
            SetCulture();

            AreaEsito esito = null;
            areaAventiDiritto = new AreaAventiDiritto();
            string messaggioVideo = string.Empty;

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            #region valorizzazione parametri ARCA
            Entity.ParametriARCA parametriArca = null;
            if (!ValorizzaParametriARCA(matricolaOperatore, out parametriArca))
            {
                esito = new AreaEsito();
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero delle informazioni anagrafiche";
                string messaggio = "Errore nella valorizzazione dei parametri ARCA";
                string parametri = string.Format("Matricola Operatore: {0}", matricolaOperatore);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, null);
                return esito;
            }
            #endregion valorizzazione parametri ARCA

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);

            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            if (!GestioneAreaAventiDiritto.AggiornaAventiDirittoFromArchivioPensione(parametriArca, datiPensione, datiDanteCausa, datiAnagrafici.CodiceFiscale, sedeOperatore, centroOperativoOperatore, out messaggioVideo))
            {
                esito = new AreaEsito();
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = messaggioVideo;
            }

            Entity.AventiDiritto areaAventiDirittoBL = null;
            GestioneAreaAventiDiritto.GetAventiDirittoConAnagraficheByDatiPensione(datiPensione, out areaAventiDirittoBL, out messaggioVideo);
            areaAventiDiritto.DatiAventiDiritto = areaAventiDirittoBL;

            GetListeDecodificaAreaAventiDiritto(datiPensione, ref areaAventiDiritto);

            if (esito == null)
            {
                esito = new AreaEsito();
                if (!string.IsNullOrEmpty(messaggioVideo))
                {
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    esito.Messaggio = messaggioVideo;
                }
                else
                {
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    esito.Messaggio = string.Empty;
                }
            }
            return esito;
        }

        #endregion Aventi Diritto

        #region Altre Domanda Collegate
        public AreaEsito GetAreaAltreDomandeCollegateByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaAltreDomandeCollegate areaAltreDomandeCollegate)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            areaAltreDomandeCollegate = new AreaAltreDomandeCollegate();
            string messaggioVideo = string.Empty;

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            List<Entity.DomandeCollegate> elencoAltreDomandeCollegate = null;
            GestioneAreaAltreDomandeCollegate.GetAreaDomandeCollegate(datiPensione, out elencoAltreDomandeCollegate, out messaggioVideo);

            areaAltreDomandeCollegate.ElencoDomandeCollegate = elencoAltreDomandeCollegate;

            if (!string.IsNullOrEmpty(messaggioVideo))
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = messaggioVideo;
            }
            else
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                esito.Messaggio = string.Empty;
            }

            return esito;
        }

        public AreaEsito GetAventiDirittoDomandaCollegataByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, long numeroDomandaAventeDiritto, short sedeOperatore, string matricolaOperatore,
            out AreaAltreDomandeCollegate areaAltreDomandeCollegate)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            areaAltreDomandeCollegate = new AreaAltreDomandeCollegate();
            string messaggioVideo = string.Empty;

            #region valorizzazione parametri ARCA
            Entity.ParametriARCA parametriArca = null;
            if (!ValorizzaParametriARCA(matricolaOperatore, out parametriArca))
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero delle informazioni anagrafiche";
                string messaggio = "Errore nella valorizzazione dei parametri ARCA";
                string parametri = string.Format("Matricola Operatore: {0}", matricolaOperatore);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomandaAventeDiritto, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, null);
                return esito;
            }
            #endregion valorizzazione parametri ARCA

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            Entity.AventiDiritto areaAventiDiritto = null;
            GestioneAreaAltreDomandeCollegate.GetAventiDirittoDomandaCollegata(numeroDomandaAventeDiritto, parametriArca, areaRichiestaDomanda.NumeroDomanda.ToString(), out areaAventiDiritto, out messaggioVideo);
            areaAltreDomandeCollegate.AreaAventiDiritto = areaAventiDiritto;

            GetListeDecodificaAreaAltreDomandeCollegate(datiPensione, ref areaAltreDomandeCollegate);

            if (!string.IsNullOrEmpty(messaggioVideo))
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = messaggioVideo;
            }
            else
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                esito.Messaggio = string.Empty;
            }

            return esito;
        }

        private void GetListeDecodificaAreaAltreDomandeCollegate(GestionePensione.DatiPensione datiPensione, ref AreaAltreDomandeCollegate areaAltreDomandeCollegate)
        {
            List<GestioneAreaFamiliari.AreaDecFam.DatiSiglaFamiliare> elencoGradoParentela = null;
            GestioneAreaPeriodiAventiDiritto.GetDecodificaGradoParentela(datiPensione, out elencoGradoParentela);
            if (elencoGradoParentela != null && elencoGradoParentela.Count > 0)
            {
                if (areaAltreDomandeCollegate == null)
                    areaAltreDomandeCollegate = new AreaAltreDomandeCollegate();
                areaAltreDomandeCollegate.ElencoGradiParentela = elencoGradoParentela;
            }
        }
        #endregion Altre Domande Collegate

        #region Banche Fideiussione, Aziende e AziendeGGmmAAAA

        public AreaEsito GetAllBancheFideiussione(out AreaBancaFideiussione areaBancaFideiussione)
        {
            SetCulture();

            areaBancaFideiussione = new AreaBancaFideiussione();
            AreaEsito esito = new AreaEsito();
            try
            {
                #region banche
                //Caricamento Banche
                List<GestioneBancheFideiussione.DecBancaFideiussione> elencoBancheFideiuss = null;
                GestioneAreaBancaFideiussione.GetDecodificaBancaFideiussione(out elencoBancheFideiuss);
                if (elencoBancheFideiuss != null && elencoBancheFideiuss.Count > 0)
                {
                    if (areaBancaFideiussione.ElencoBancheFideiussione == null)
                        areaBancaFideiussione.ElencoBancheFideiussione = new List<GestioneBancheFideiussione.DecBancaFideiussione>();
                    foreach (GestioneBancheFideiussione.DecBancaFideiussione bf in elencoBancheFideiuss)
                        areaBancaFideiussione.ElencoBancheFideiussione.Add(bf);
                }
                #endregion banche

                #region aziende
                //Caricamento Aziende
                List<GestioneDecodificaAzienda.DecAzienda> elencoAziende = null;
                GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria("VESO92", null, out elencoAziende);

                if (elencoAziende != null && elencoAziende.Count > 0)
                {
                    if (areaBancaFideiussione.ElencoAziende == null)
                        areaBancaFideiussione.ElencoAziende = new List<GestioneDecodificaAzienda.DecAzienda>();
                    foreach (GestioneDecodificaAzienda.DecAzienda a in elencoAziende)
                        areaBancaFideiussione.ElencoAziende.Add(a);
                }
                #endregion aziende

                #region aziendeGGmmAAAA
                //caricamento aziende gg mm aaaa
                List<GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA> elencoAziendeGGmmAAAA = null;
                GestioneAziendeScadenzaAssegnoGGmmAAAA.GetDecodificaAziendeScadenzaAssegnoGGmmAAAA(out elencoAziendeGGmmAAAA);
                if (elencoAziendeGGmmAAAA != null && elencoAziendeGGmmAAAA.Count > 0)
                {
                    if (areaBancaFideiussione.ElencoAziendeAssegnoGGmmAAAA == null)
                        areaBancaFideiussione.ElencoAziendeAssegnoGGmmAAAA = new List<GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA>();
                    foreach (GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA aGGmmAAAA in elencoAziendeGGmmAAAA.FindAll(x => x.SiglaCatPensione.Trim() == "VESO92"))
                        areaBancaFideiussione.ElencoAziendeAssegnoGGmmAAAA.Add(aGGmmAAAA);
                }
                #endregion aziendeGGmmAAAA
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero dell'elenco delle Banche Fideiussorie";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito SalvaBancheFideiussione(ref AreaBancaFideiussione areaBancaFideiussione)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaBancaFideiussione == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Banca Fideiussoria da salvare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaBancaFideiussione.SalvaBancheFideiussione(areaBancaFideiussione.BancaFideiussione, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }

                ////Caricamento Banche
                //List<GestioneBancheFideiussione.DecBancaFideiussione> elencoBancheFideiuss = null;
                //GestioneAreaBancaFideiussione.GetDecodificaBancaFideiussione(out elencoBancheFideiuss);
                //if (elencoBancheFideiuss != null && elencoBancheFideiuss.Count > 0)
                //{
                //    if (areaBancaFideiussione.ElencoBancheFideiussione == null)
                //        areaBancaFideiussione.ElencoBancheFideiussione = new List<GestioneBancheFideiussione.DecBancaFideiussione>();
                //    foreach (GestioneBancheFideiussione.DecBancaFideiussione bf in elencoBancheFideiuss)
                //        areaBancaFideiussione.ElencoBancheFideiussione.Add(bf);
                //}

                ////Caricamento Aziende
                //List<GestioneDecodificaAzienda.DecAzienda> elencoAziende = null;
                //GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria("VESO92", out elencoAziende);
                //if (elencoAziende != null && elencoAziende.Count > 0)
                //{
                //    if (areaBancaFideiussione.ElencoAziende == null)
                //        areaBancaFideiussione.ElencoAziende = new List<GestioneDecodificaAzienda.DecAzienda>();
                //    foreach (GestioneDecodificaAzienda.DecAzienda a in elencoAziende)
                //        areaBancaFideiussione.ElencoAziende.Add(a);
                //}



                //carica tutte le liste
                GetAllBancheFideiussione(out areaBancaFideiussione);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento delle Banche Fideiussorie";
                return esito;
            }

            return esito;
        }

        public AreaEsito EliminaBancheFideiussione(ref AreaBancaFideiussione areaBancaFideiussione)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaBancaFideiussione == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Banca Fideiussoria da eliminare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaBancaFideiussione.DeleteBancheFideiussione(areaBancaFideiussione.BancaFideiussione, out messaggioVideo);
                }

                ////Caricamento Banche
                //List<GestioneBancheFideiussione.DecBancaFideiussione> elencoBancheFideiuss = null;
                //GestioneAreaBancaFideiussione.GetDecodificaBancaFideiussione(out elencoBancheFideiuss);
                //if (elencoBancheFideiuss != null && elencoBancheFideiuss.Count > 0)
                //{
                //    if (areaBancaFideiussione.ElencoBancheFideiussione == null)
                //        areaBancaFideiussione.ElencoBancheFideiussione = new List<GestioneBancheFideiussione.DecBancaFideiussione>();
                //    foreach (GestioneBancheFideiussione.DecBancaFideiussione bf in elencoBancheFideiuss)
                //        areaBancaFideiussione.ElencoBancheFideiussione.Add(bf);
                //}

                ////Caricamento Aziende
                //List<GestioneDecodificaAzienda.DecAzienda> elencoAziende = null;
                //GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria("VESO92", out elencoAziende);

                //if (elencoAziende != null && elencoAziende.Count > 0)
                //{
                //    if (areaBancaFideiussione.ElencoAziende == null)
                //        areaBancaFideiussione.ElencoAziende = new List<GestioneDecodificaAzienda.DecAzienda>();
                //    foreach (GestioneDecodificaAzienda.DecAzienda a in elencoAziende)
                //        areaBancaFideiussione.ElencoAziende.Add(a);
                //}

                //caricamento di tute le le liste
                GetAllBancheFideiussione(out areaBancaFideiussione);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nella cancellazione delle Banche Fideiussorie";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito SalvaAzienda(ref AreaBancaFideiussione areaBancaFideiussioneAzienda)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaBancaFideiussioneAzienda == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Azienda da salvare");
                else
                {
                    string messaggioVideo = null;

                    GestioneAreaBancaFideiussione.SalvaAziende(areaBancaFideiussioneAzienda.Azienda, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }


                ////Caricamento Banche
                //List<GestioneBancheFideiussione.DecBancaFideiussione> elencoBancheFideiuss = null;
                //GestioneAreaBancaFideiussione.GetDecodificaBancaFideiussione(out elencoBancheFideiuss);
                //if (elencoBancheFideiuss != null && elencoBancheFideiuss.Count > 0)
                //{
                //    if (areaBancaFideiussioneAzienda.ElencoBancheFideiussione == null)
                //        areaBancaFideiussioneAzienda.ElencoBancheFideiussione = new List<GestioneBancheFideiussione.DecBancaFideiussione>();
                //    foreach (GestioneBancheFideiussione.DecBancaFideiussione bf in elencoBancheFideiuss)
                //        areaBancaFideiussioneAzienda.ElencoBancheFideiussione.Add(bf);
                //}

                ////Caricamento Aziende
                //List<GestioneDecodificaAzienda.DecAzienda> elencoAziende = null;
                //GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria("VESO92", out elencoAziende);

                //if (elencoAziende != null && elencoAziende.Count > 0)
                //{
                //    if (areaBancaFideiussioneAzienda.ElencoAziende == null)
                //        areaBancaFideiussioneAzienda.ElencoAziende = new List<GestioneDecodificaAzienda.DecAzienda>();
                //    foreach (GestioneDecodificaAzienda.DecAzienda a in elencoAziende)
                //        areaBancaFideiussioneAzienda.ElencoAziende.Add(a);
                //}


                //caricamento di tute le liste
                GetAllBancheFideiussione(out areaBancaFideiussioneAzienda);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento delle Aziende";
                return esito;
            }

            return esito;
        }

        public AreaEsito SalvaAziendaGGmmAAAA(ref AreaBancaFideiussione areaBancaFideiussione)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaBancaFideiussione == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Azienda da salvare");
                else
                {
                    string messaggioVideo = null;

                    GestioneAreaBancaFideiussione.SalvaAziendeScadAssegnoGGmmAAAA(areaBancaFideiussione.AziendaGGmmAAAA, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }

                ////Caricamento Banche
                //List<GestioneBancheFideiussione.DecBancaFideiussione> elencoBancheFideiuss = null;
                //GestioneAreaBancaFideiussione.GetDecodificaBancaFideiussione(out elencoBancheFideiuss);
                //if (elencoBancheFideiuss != null && elencoBancheFideiuss.Count > 0)
                //{
                //    if (areaBancaFideiussioneAziendaGGmmAAAA.ElencoBancheFideiussione == null)
                //        areaBancaFideiussioneAziendaGGmmAAAA.ElencoBancheFideiussione = new List<GestioneBancheFideiussione.DecBancaFideiussione>();
                //    foreach (GestioneBancheFideiussione.DecBancaFideiussione bf in elencoBancheFideiuss)
                //        areaBancaFideiussioneAziendaGGmmAAAA.ElencoBancheFideiussione.Add(bf);
                //}

                ////Caricamento Aziende
                //List<GestioneDecodificaAzienda.DecAzienda> elencoAziende = null;
                //GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria("VESO92", out elencoAziende);

                //if (elencoAziende != null && elencoAziende.Count > 0)
                //{
                //    if (areaBancaFideiussioneAziendaGGmmAAAA.ElencoAziende == null)
                //        areaBancaFideiussioneAziendaGGmmAAAA.ElencoAziende = new List<GestioneDecodificaAzienda.DecAzienda>();
                //    foreach (GestioneDecodificaAzienda.DecAzienda a in elencoAziende)
                //        areaBancaFideiussioneAziendaGGmmAAAA.ElencoAziende.Add(a);
                //}

                GetAllBancheFideiussione(out areaBancaFideiussione);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento delle AziendeGGmmAAAA";
                return esito;
            }

            return esito;
        }

        public AreaEsito EliminaAziendaGGmmAAAA(ref AreaBancaFideiussione areaBancaFideiussioneAziendaGGmmAAAA)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaBancaFideiussioneAziendaGGmmAAAA == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Banca Fideiussoria da eliminare");
                else
                {
                    string messaggioVideo = null;

                    GestioneAreaBancaFideiussione.DeleteAziendeScadAssegnoGGmmAAAA(areaBancaFideiussioneAziendaGGmmAAAA.AziendaGGmmAAAA, out messaggioVideo);
                }

                //    //Caricamento Banche
                //    List<GestioneBancheFideiussione.DecBancaFideiussione> elencoBancheFideiuss = null;
                //    GestioneAreaBancaFideiussione.GetDecodificaBancaFideiussione(out elencoBancheFideiuss);
                //    if (elencoBancheFideiuss != null && elencoBancheFideiuss.Count > 0)
                //    {
                //        if (areaBancaFideiussione.ElencoBancheFideiussione == null)
                //            areaBancaFideiussione.ElencoBancheFideiussione = new List<GestioneBancheFideiussione.DecBancaFideiussione>();
                //        foreach (GestioneBancheFideiussione.DecBancaFideiussione bf in elencoBancheFideiuss)
                //            areaBancaFideiussione.ElencoBancheFideiussione.Add(bf);
                //    }

                //    //Caricamento Aziende
                //    List<GestioneDecodificaAzienda.DecAzienda> elencoAziende = null;
                //    GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria("VESO92", out elencoAziende);

                //    if (elencoAziende != null && elencoAziende.Count > 0)
                //    {
                //        if (areaBancaFideiussione.ElencoAziende == null)
                //            areaBancaFideiussione.ElencoAziende = new List<GestioneDecodificaAzienda.DecAzienda>();
                //        foreach (GestioneDecodificaAzienda.DecAzienda a in elencoAziende)
                //            areaBancaFideiussione.ElencoAziende.Add(a);
                //    }

                //caricamento di ttue le liste
                GetAllBancheFideiussione(out areaBancaFideiussioneAziendaGGmmAAAA);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nella cancellazione delle Aziende con Scadenza assegno in formato GGmmAAAA";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;

        }

        #endregion Banche Fideiussione, Aziende e AziendeGGmmAAAA

        #region Banche Fideiussione, Aziende e AziendeGGmmAAAA ESPA

        public AreaEsito GetAllBancheFideiussioneESPA(out AreaBancaFideiussioneESPA areaBancaFideiussione)
        {
            SetCulture();

            areaBancaFideiussione = new AreaBancaFideiussioneESPA();
            AreaEsito esito = new AreaEsito();
            try
            {
                #region banche
                //Caricamento Banche
                List<GestioneBancheFideiussioneESPA.DecBancaFideiussione> elencoBancheFideiuss = null;
                GestioneAreaBancaFideiussioneESPA.GetDecodificaBancaFideiussione(out elencoBancheFideiuss);
                if (elencoBancheFideiuss != null && elencoBancheFideiuss.Count > 0)
                {
                    if (areaBancaFideiussione.ElencoBancheFideiussione == null)
                        areaBancaFideiussione.ElencoBancheFideiussione = new List<GestioneBancheFideiussioneESPA.DecBancaFideiussione>();
                    foreach (GestioneBancheFideiussioneESPA.DecBancaFideiussione bf in elencoBancheFideiuss)
                        areaBancaFideiussione.ElencoBancheFideiussione.Add(bf);
                }
                #endregion banche

                #region aziende
                //Caricamento Aziende
                List<GestioneDecodificaAzienda.DecAzienda> elencoAziende = null;
                GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria("ESPA", null, out elencoAziende);

                if (elencoAziende != null && elencoAziende.Count > 0)
                {
                    if (areaBancaFideiussione.ElencoAziende == null)
                        areaBancaFideiussione.ElencoAziende = new List<GestioneDecodificaAzienda.DecAzienda>();
                    foreach (GestioneDecodificaAzienda.DecAzienda a in elencoAziende)
                        areaBancaFideiussione.ElencoAziende.Add(a);
                }
                #endregion aziende

                #region aziendeGGmmAAAA
                //caricamento aziende gg mm aaaa
                List<GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA> elencoAziendeGGmmAAAA = null;
                GestioneAziendeScadenzaAssegnoGGmmAAAA.GetDecodificaAziendeScadenzaAssegnoGGmmAAAA(out elencoAziendeGGmmAAAA);
                if (elencoAziendeGGmmAAAA != null && elencoAziendeGGmmAAAA.Count > 0)
                {
                    if (areaBancaFideiussione.ElencoAziendeAssegnoGGmmAAAA == null)
                        areaBancaFideiussione.ElencoAziendeAssegnoGGmmAAAA = new List<GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA>();
                    foreach (GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA aGGmmAAAA in elencoAziendeGGmmAAAA.FindAll(x => x.SiglaCatPensione.Trim() == "ESPA"))
                        areaBancaFideiussione.ElencoAziendeAssegnoGGmmAAAA.Add(aGGmmAAAA);
                }
                #endregion aziendeGGmmAAAA
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero dell'elenco delle Banche Fideiussorie ESPA";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito SalvaBancheFideiussioneESPA(ref AreaBancaFideiussioneESPA areaBancaFideiussione)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaBancaFideiussione == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Banca Fideiussoria da salvare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaBancaFideiussioneESPA.SalvaBancheFideiussione(areaBancaFideiussione.BancaFideiussione, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }

                //carica tutte le liste
                GetAllBancheFideiussioneESPA(out areaBancaFideiussione);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento delle Banche Fideiussorie ESPA";
                return esito;
            }

            return esito;
        }

        public AreaEsito EliminaBancheFideiussioneESPA(ref AreaBancaFideiussioneESPA areaBancaFideiussione)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaBancaFideiussione == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Banca Fideiussoria ESPA da eliminare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaBancaFideiussioneESPA.DeleteBancheFideiussione(areaBancaFideiussione.BancaFideiussione, out messaggioVideo);
                }

                //caricamento di tute le le liste
                GetAllBancheFideiussioneESPA(out areaBancaFideiussione);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nella cancellazione delle Banche Fideiussorie ESPA";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito SalvaAziendaESPA(ref AreaBancaFideiussioneESPA areaBancaFideiussioneAzienda)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaBancaFideiussioneAzienda == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Azienda ESPA da salvare");
                else
                {
                    string messaggioVideo = null;

                    GestioneAreaBancaFideiussioneESPA.SalvaAziende(areaBancaFideiussioneAzienda.Azienda, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }

                //caricamento di tute le liste
                GetAllBancheFideiussioneESPA(out areaBancaFideiussioneAzienda);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento delle Aziende ESPA";
                return esito;
            }

            return esito;
        }

        public AreaEsito SalvaAziendaESPAGGmmAAAA(ref AreaBancaFideiussioneESPA areaBancaFideiussione)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaBancaFideiussione == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Azienda ESPA da salvare");
                else
                {
                    string messaggioVideo = null;

                    GestioneAreaBancaFideiussioneESPA.SalvaAziendeScadAssegnoGGmmAAAA(areaBancaFideiussione.AziendaGGmmAAAA, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }

                GetAllBancheFideiussioneESPA(out areaBancaFideiussione);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento delle AziendeGGmmAAAA ESPA";
                return esito;
            }

            return esito;
        }

        public AreaEsito EliminaAziendaESPAGGmmAAAA(ref AreaBancaFideiussioneESPA areaBancaFideiussioneAziendaGGmmAAAA)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaBancaFideiussioneAziendaGGmmAAAA == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Banca Fideiussoria da eliminare");
                else
                {
                    string messaggioVideo = null;

                    GestioneAreaBancaFideiussioneESPA.DeleteAziendeScadAssegnoGGmmAAAA(areaBancaFideiussioneAziendaGGmmAAAA.AziendaGGmmAAAA, out messaggioVideo);
                }

                //caricamento di ttue le liste
                GetAllBancheFideiussioneESPA(out areaBancaFideiussioneAziendaGGmmAAAA);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nella cancellazione delle Aziende ESPA con Scadenza assegno in formato GGmmAAAA";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;

        }

        #endregion Banche Fideiussione, Aziende e AziendeGGmmAAAA ESPA

        public AreaEsito GetAllSediMatricola(string sede, out AreaSediMatricola sediMatricola)
        {
            SetCulture();

            sediMatricola = new AreaSediMatricola();
            AreaEsito esito = new AreaEsito();
            try
            {
                List<Entity.SediMatricola> listaSediMatricole = null;
                GestioneAreaSediMatricola.GetDecodificaSediMatricola(sede, out listaSediMatricole);
                if (listaSediMatricole != null && listaSediMatricole.Count > 0)
                {
                    if (sediMatricola.elencoSediMatricole == null)
                        sediMatricola.elencoSediMatricole = new List<Entity.SediMatricola>();
                    foreach (Entity.SediMatricola sm in listaSediMatricole)
                        sediMatricola.elencoSediMatricole.Add(sm);
                }
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero delle informazioni";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        #region Area Automazione
        public AreaEsito IsMatricolaForAutomazione(string matricola, out bool isMatricolaForAutomazione)
        {
            SetCulture();

            isMatricolaForAutomazione = false;
            AreaEsito esito = new AreaEsito();
            try
            {
                if (matricola != null)
                    isMatricolaForAutomazione = GestioneCtrlMatricoleAutomazione.IsMatricolaForAutomazione(matricola);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero delle informazioni";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }
        #endregion Area Automazione

        #region AziendeVESO33

        public AreaEsito GetAllAziendeVESO33(out AreaAziendeVESO33 areaAziendaVESO33)
        {
            SetCulture();

            areaAziendaVESO33 = new AreaAziendeVESO33();
            AreaEsito esito = new AreaEsito();
            try
            {
                List<Entity.AziendeVESO33> listaAziende = null;
                GestioneAreaAziendeEAziendeVESO33.GetDecodificaAziendeEAziendeVESO33("VESO33", out listaAziende);
                if (listaAziende != null && listaAziende.Count > 0)
                {
                    if (areaAziendaVESO33.elencoAziendeVESO33 == null)
                        areaAziendaVESO33.elencoAziendeVESO33 = new List<Entity.AziendeVESO33>();
                    foreach (Entity.AziendeVESO33 la in listaAziende)
                        areaAziendaVESO33.elencoAziendeVESO33.Add(la);
                }
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero delle aziende VESO33";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito SalvaAziendeVESO33(ref AreaAziendeVESO33 areaAziendaVESO33)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaAziendaVESO33 == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Azienda VESO33 da salvare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEAziendeVESO33.SalvaAziendeVESO33(areaAziendaVESO33.AziendaVESO33, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {

                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }

            }

            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento delle Aziende VESO33";
            }
            try
            {
                //caricamento aziende veso33
                List<Entity.AziendeVESO33> elencoAziendeVESO33 = null;
                GestioneAreaAziendeEAziendeVESO33.GetDecodificaAziendeEAziendeVESO33("VESO33", out elencoAziendeVESO33);
                if (elencoAziendeVESO33 != null && elencoAziendeVESO33.Count > 1)
                {
                    if (areaAziendaVESO33.elencoAziendeVESO33 == null)
                        areaAziendaVESO33.elencoAziendeVESO33 = new List<AziendeVESO33>();
                    foreach (Entity.AziendeVESO33 aV33 in elencoAziendeVESO33)
                        areaAziendaVESO33.elencoAziendeVESO33.Add(aV33);
                }
            }
            catch (Exception)
            {
                //Eccezione ignorata
            }
            return esito;
        }

        public AreaEsito EliminaAziendeVESO33(ref AreaAziendeVESO33 areaAziendaVESO33)
        {
            SetCulture();

            AreaEsito esito = null;
            try
            {
                if (areaAziendaVESO33 == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna AziendaVESO33 da eliminare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEAziendeVESO33.DeleteAziendeVESO33(areaAziendaVESO33.AziendaVESO33, out messaggioVideo);
                }
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito = new AreaEsito();
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                esito = new AreaEsito();
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nella cancellazione delle AziendeVESO33";

            }
            try
            {
                //caricamento aziende veso33
                List<Entity.AziendeVESO33> elencoAziendeVESO33 = null;
                GestioneAreaAziendeEAziendeVESO33.GetDecodificaAziendeEAziendeVESO33("VESO33", out elencoAziendeVESO33);
                if (elencoAziendeVESO33 != null && elencoAziendeVESO33.Count > 1)
                {
                    if (areaAziendaVESO33.elencoAziendeVESO33 == null)
                        areaAziendaVESO33.elencoAziendeVESO33 = new List<AziendeVESO33>();
                    foreach (Entity.AziendeVESO33 aV33 in elencoAziendeVESO33)
                        areaAziendaVESO33.elencoAziendeVESO33.Add(aV33);
                }
            }
            catch (Exception)
            {
                //Eccezione ignorata
            }

            if (esito == null)
            {
                esito = new AreaEsito();
                esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                esito.Messaggio = string.Empty;
            }
            return esito;
        }

        #endregion AziendeVESO33

        #region AziendeVESO29

        public AreaEsito GetAllAziendeVESO29(out AreaAziendeVESO29 areaAziendaVESO29)
        {
            SetCulture();

            areaAziendaVESO29 = new AreaAziendeVESO29();
            AreaEsito esito = new AreaEsito();
            try
            {
                List<Entity.AziendeVESO29> listaAziende = null;
                GestioneAreaAziendeEAziendeVESO29.GetDecodificaAziendeEAziendeVESO29(out listaAziende);
                if (listaAziende != null && listaAziende.Count > 0)
                {
                    if (areaAziendaVESO29.elencoAziendeVESO29 == null)
                        areaAziendaVESO29.elencoAziendeVESO29 = new List<Entity.AziendeVESO29>();
                    foreach (Entity.AziendeVESO29 la in listaAziende)
                        areaAziendaVESO29.elencoAziendeVESO29.Add(la);
                }
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero delle aziende VESO29";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito SalvaAziendeVESO29(ref AreaAziendeVESO29 areaAziendaVESO29)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaAziendaVESO29 == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Azienda VESO29 da salvare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEAziendeVESO29.SalvaAziendeVESO29(areaAziendaVESO29.AziendaVESO29, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento delle Aziende VESO29";
            }
            try
            {
                //caricamento aziende veso29
                List<Entity.AziendeVESO29> elencoAziendeVESO29 = null;
                GestioneAreaAziendeEAziendeVESO29.GetDecodificaAziendeEAziendeVESO29(out elencoAziendeVESO29);
                if (elencoAziendeVESO29 != null && elencoAziendeVESO29.Count > 1)
                {
                    if (areaAziendaVESO29.elencoAziendeVESO29 == null)
                        areaAziendaVESO29.elencoAziendeVESO29 = new List<AziendeVESO29>();
                    foreach (Entity.AziendeVESO29 aV29 in elencoAziendeVESO29)
                        areaAziendaVESO29.elencoAziendeVESO29.Add(aV29);
                }
            }
            catch (Exception)
            {
                //Eccezione ignorata
            }

            return esito;
        }

        public AreaEsito EliminaAziendeVESO29(ref AreaAziendeVESO29 areaAziendaVESO29)
        {
            SetCulture();

            AreaEsito esito = null;
            try
            {
                if (areaAziendaVESO29 == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna AziendaVESO29 da eliminare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEAziendeVESO29.DeleteAziendeVESO29(areaAziendaVESO29.AziendaVESO29, out messaggioVideo);
                }
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito = new AreaEsito();
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                esito = new AreaEsito();
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nella cancellazione delle AziendeVESO29";
            }

            try
            {
                //caricamento aziende veso29
                List<Entity.AziendeVESO29> elencoAziendeVESO29 = null;
                GestioneAreaAziendeEAziendeVESO29.GetDecodificaAziendeEAziendeVESO29(out elencoAziendeVESO29);
                if (elencoAziendeVESO29 != null && elencoAziendeVESO29.Count > 1)
                {
                    if (areaAziendaVESO29.elencoAziendeVESO29 == null)
                        areaAziendaVESO29.elencoAziendeVESO29 = new List<AziendeVESO29>();
                    foreach (Entity.AziendeVESO29 aV29 in elencoAziendeVESO29)
                        areaAziendaVESO29.elencoAziendeVESO29.Add(aV29);
                }
            }
            catch (Exception)
            {
                //Eccezione ignorata
            }

            if (esito == null)
            {
                esito = new AreaEsito();
                esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                esito.Messaggio = string.Empty;
            }
            return esito;
        }

        #endregion AziendeVESO29

        #region AziendeCredito

        public AreaEsito GetAllAziendeCredito(string categoriaAzienda, out AreaAziendeCredito areaAziendaCredito)
        {
            SetCulture();

            areaAziendaCredito = new AreaAziendeCredito();
            AreaEsito esito = new AreaEsito();
            try
            {
                List<Entity.AziendeCredito> listaAziende = null;
                GestioneAreaAziendeEAziendeCredito.GetDecodificaAziendeEAziendeCredito(categoriaAzienda, out listaAziende);
                if (listaAziende != null && listaAziende.Count > 0)
                {
                    if (areaAziendaCredito.elencoAziendeCredito == null)
                        areaAziendaCredito.elencoAziendeCredito = new List<Entity.AziendeCredito>();
                    foreach (Entity.AziendeCredito la in listaAziende)
                        areaAziendaCredito.elencoAziendeCredito.Add(la);
                }
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero delle aziende Credito";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito SalvaAziendeCredito(string categoriaAzienda, ref AreaAziendeCredito areaAziendaCredito)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaAziendaCredito == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Azienda Credito da salvare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEAziendeCredito.SalvaAziendeCredito(areaAziendaCredito.AziendaCredito, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {

                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }

            }

            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento delle Aziende Credito";
            }
            try
            {
                //caricamento aziende Credito
                List<Entity.AziendeCredito> elencoAziendeCredito = null;
                GestioneAreaAziendeEAziendeCredito.GetDecodificaAziendeEAziendeCredito(categoriaAzienda, out elencoAziendeCredito);
                if (elencoAziendeCredito != null && elencoAziendeCredito.Count > 1)
                {
                    if (areaAziendaCredito.elencoAziendeCredito == null)
                        areaAziendaCredito.elencoAziendeCredito = new List<AziendeCredito>();
                    foreach (Entity.AziendeCredito aCredito in elencoAziendeCredito)
                        areaAziendaCredito.elencoAziendeCredito.Add(aCredito);
                }
            }
            catch (Exception)
            {
                //Eccezione ignorata
            }
            return esito;
        }

        public AreaEsito EliminaAziendeCredito(string categoriaAzienda, ref AreaAziendeCredito areaAziendaCredito)
        {
            SetCulture();

            AreaEsito esito = null;
            try
            {
                if (areaAziendaCredito == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Azienda Credito da eliminare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEAziendeCredito.DeleteAziendeCredito(areaAziendaCredito.AziendaCredito, out messaggioVideo);
                }
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito = new AreaEsito();
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                esito = new AreaEsito();
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nella cancellazione delle Aziende Credito";

            }
            try
            {
                //caricamento aziende Credito
                List<Entity.AziendeCredito> elencoAziendeCredito = null;
                GestioneAreaAziendeEAziendeCredito.GetDecodificaAziendeEAziendeCredito(categoriaAzienda, out elencoAziendeCredito);
                if (elencoAziendeCredito != null && elencoAziendeCredito.Count > 1)
                {
                    if (areaAziendaCredito.elencoAziendeCredito == null)
                        areaAziendaCredito.elencoAziendeCredito = new List<AziendeCredito>();
                    foreach (Entity.AziendeCredito aCredito in elencoAziendeCredito)
                        areaAziendaCredito.elencoAziendeCredito.Add(aCredito);
                }
            }
            catch (Exception)
            {
                //Eccezione ignorata
            }

            if (esito == null)
            {
                esito = new AreaEsito();
                esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                esito.Messaggio = string.Empty;
            }
            return esito;
        }

        #endregion AziendeCredito

        #region Aziende Editoriali

        public AreaEsito GetAllAziendeEditoriali(out AreaAziendeEditoriali areaAziendeEditoriali)
        {
            SetCulture();

            areaAziendeEditoriali = new AreaAziendeEditoriali();
            AreaEsito esito = new AreaEsito();
            try
            {
                #region anagraficaAccordi

                //Caricamento AnagraficaAccordi
                List<GestioneAnagraficaAccordi.DecodAnagraficaAccordi> elencoAnagraficaAccordi = null;
                GestioneAnagraficaAccordi.GetDecAnagraficaAccordi(out elencoAnagraficaAccordi);
                if (elencoAnagraficaAccordi != null && elencoAnagraficaAccordi.Count > 0)
                {
                    if (areaAziendeEditoriali.ElencoAnagraficheAccordi == null)
                        areaAziendeEditoriali.ElencoAnagraficheAccordi = new List<GestioneAnagraficaAccordi.DecodAnagraficaAccordi>();
                    foreach (GestioneAnagraficaAccordi.DecodAnagraficaAccordi accordo in elencoAnagraficaAccordi)
                        areaAziendeEditoriali.ElencoAnagraficheAccordi.Add(accordo);
                }
                #endregion anagraficaAccordi

                #region anagraficaAziende
                //Caricamento AnagraficaAziende
                List<GestioneAnagraficaAziende.DecodAnagraficaAziende> elencoAnagraficaAziende = null;
                GestioneAnagraficaAziende.GetDecAnagraficaAziende(out elencoAnagraficaAziende);

                if (elencoAnagraficaAziende != null && elencoAnagraficaAziende.Count > 0)
                {
                    if (areaAziendeEditoriali.ElencoAnagraficheAziende == null)
                        areaAziendeEditoriali.ElencoAnagraficheAziende = new List<GestioneAnagraficaAziende.DecodAnagraficaAziende>();
                    foreach (GestioneAnagraficaAziende.DecodAnagraficaAziende azienda in elencoAnagraficaAziende)
                        areaAziendeEditoriali.ElencoAnagraficheAziende.Add(azienda);
                }
                #endregion anagraficaAziende
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero dell'elenco delle AnagraficheAccordi";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito SalvaAnagraficaAccordi(ref AreaAziendeEditoriali areaAziendeEditoriali)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaAziendeEditoriali == null || areaAziendeEditoriali.AnagraficheAccordi == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Anagrafica Accordi da salvare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEditoriali.SalvaAnagraficaAccordi(areaAziendeEditoriali.AnagraficheAccordi, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }

                //carica tutte le liste
                GetAllAziendeEditoriali(out areaAziendeEditoriali);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento delle Anagrafica Accordi";
                return esito;
            }

            return esito;
        }

        public AreaEsito EliminaAnagraficaAccordi(ref AreaAziendeEditoriali areaAziendeEditoriali)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            string messaggioVideo = null;
            try
            {
                if (areaAziendeEditoriali == null || areaAziendeEditoriali.AnagraficheAccordi == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Anagrafica Accordi da eliminare");
                else
                {
                    GestioneAreaAziendeEditoriali.DeleteAnagraficaAccordi(areaAziendeEditoriali.AnagraficheAccordi, out messaggioVideo);
                }

                //caricamento di tute le le liste
                GetAllAziendeEditoriali(out areaAziendeEditoriali);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nella cancellazione delle Anagrafica Accordi";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = messaggioVideo;
            return esito;
        }

        public AreaEsito SalvaAnagraficaAziende(ref AreaAziendeEditoriali areaAziendeEditoriali)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaAziendeEditoriali == null || areaAziendeEditoriali.AnagraficheAziende == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Anagrafica Aziende da salvare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEditoriali.SalvaAnagraficaAziende(areaAziendeEditoriali.AnagraficheAziende, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }

                //carica tutte le liste
                GetAllAziendeEditoriali(out areaAziendeEditoriali);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento delle Anagrafica Aziende";
                return esito;
            }

            return esito;
        }

        public AreaEsito EliminaAnagraficaAziende(ref AreaAziendeEditoriali areaAziendeEditoriali)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaAziendeEditoriali == null || areaAziendeEditoriali.AnagraficheAziende == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Anagrafica Aziende da eliminare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEditoriali.DeleteAnagraficaAziende(areaAziendeEditoriali.AnagraficheAziende, out messaggioVideo);
                }

                //caricamento di tute le le liste
                GetAllAziendeEditoriali(out areaAziendeEditoriali);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nella cancellazione delle Anagrafica Aziende";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        #endregion Aziende Editoriali

        #region Aziende Editoriali LetteraB

        public AreaEsito GetAllAziendeEditorialiLetteraB(out AreaAziendeEditorialiLetteraB areaAziendeEditoriali)
        {
            SetCulture();

            areaAziendeEditoriali = new AreaAziendeEditorialiLetteraB();
            AreaEsito esito = new AreaEsito();
            try
            {
                #region anagraficaAccordi

                //Caricamento AnagraficaAccordi
                List<GestioneAnagraficaAccordiLetteraB.DecodAnagraficaAccordiLetteraB> elencoAnagraficaAccordi = null;
                GestioneAnagraficaAccordiLetteraB.GetDecAnagraficaAccordi(out elencoAnagraficaAccordi);
                if (elencoAnagraficaAccordi != null && elencoAnagraficaAccordi.Count > 0)
                {
                    if (areaAziendeEditoriali.ElencoAnagraficheAccordi == null)
                        areaAziendeEditoriali.ElencoAnagraficheAccordi = new List<GestioneAnagraficaAccordiLetteraB.DecodAnagraficaAccordiLetteraB>();
                    foreach (GestioneAnagraficaAccordiLetteraB.DecodAnagraficaAccordiLetteraB accordo in elencoAnagraficaAccordi)
                        areaAziendeEditoriali.ElencoAnagraficheAccordi.Add(accordo);
                }
                #endregion anagraficaAccordi

                #region anagraficaAziende
                //Caricamento AnagraficaAziende
                List<GestioneAnagraficaAziendeLetteraB.DecodAnagraficaAziendeLetteraB> elencoAnagraficaAziende = null;
                GestioneAnagraficaAziendeLetteraB.GetDecAnagraficaAziende(out elencoAnagraficaAziende);

                if (elencoAnagraficaAziende != null && elencoAnagraficaAziende.Count > 0)
                {
                    if (areaAziendeEditoriali.ElencoAnagraficheAziende == null)
                        areaAziendeEditoriali.ElencoAnagraficheAziende = new List<GestioneAnagraficaAziendeLetteraB.DecodAnagraficaAziendeLetteraB>();
                    foreach (GestioneAnagraficaAziendeLetteraB.DecodAnagraficaAziendeLetteraB azienda in elencoAnagraficaAziende)
                        areaAziendeEditoriali.ElencoAnagraficheAziende.Add(azienda);
                }
                #endregion anagraficaAziende
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero dell'elenco delle AnagraficheAccordi";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito SalvaAnagraficaAccordiLetteraB(ref AreaAziendeEditorialiLetteraB areaAziendeEditoriali)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaAziendeEditoriali == null || areaAziendeEditoriali.AnagraficheAccordi == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Anagrafica Accordi da salvare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEditorialiLetteraB.SalvaAnagraficaAccordi(areaAziendeEditoriali.AnagraficheAccordi, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }

                //carica tutte le liste
                GetAllAziendeEditorialiLetteraB(out areaAziendeEditoriali);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento delle Anagrafica Accordi";
                return esito;
            }

            return esito;
        }

        public AreaEsito EliminaAnagraficaAccordiLetteraB(ref AreaAziendeEditorialiLetteraB areaAziendeEditoriali)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            string messaggioVideo = null;
            try
            {
                if (areaAziendeEditoriali == null || areaAziendeEditoriali.AnagraficheAccordi == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Anagrafica Accordi da eliminare");
                else
                {
                    GestioneAreaAziendeEditorialiLetteraB.DeleteAnagraficaAccordi(areaAziendeEditoriali.AnagraficheAccordi, out messaggioVideo);
                }

                //caricamento di tute le le liste
                GetAllAziendeEditorialiLetteraB(out areaAziendeEditoriali);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nella cancellazione delle Anagrafica Accordi";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = messaggioVideo;
            return esito;
        }

        public AreaEsito SalvaAnagraficaAziendeLetteraB(ref AreaAziendeEditorialiLetteraB areaAziendeEditoriali)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaAziendeEditoriali == null || areaAziendeEditoriali.AnagraficheAziende == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Anagrafica Aziende da salvare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEditorialiLetteraB.SalvaAnagraficaAziende(areaAziendeEditoriali.AnagraficheAziende, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }

                //carica tutte le liste
                GetAllAziendeEditorialiLetteraB(out areaAziendeEditoriali);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento delle Anagrafica Aziende";
                return esito;
            }

            return esito;
        }

        public AreaEsito EliminaAnagraficaAziendeLetteraB(ref AreaAziendeEditorialiLetteraB areaAziendeEditoriali)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaAziendeEditoriali == null || areaAziendeEditoriali.AnagraficheAziende == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Anagrafica Aziende da eliminare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEditorialiLetteraB.DeleteAnagraficaAziende(areaAziendeEditoriali.AnagraficheAziende, out messaggioVideo);
                }

                //caricamento di tute le le liste
                GetAllAziendeEditorialiLetteraB(out areaAziendeEditoriali);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nella cancellazione delle Anagrafica Aziende";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        #endregion Aziende Editoriali LetteraB

        #region Aziende Editoriali art.1 c. 154 legge 205/2017

        public AreaEsito GetAllAziendeEditorialiPerTipo0171(out AreaAziendeEditorialiPerTipo0171 areaAziendeEditoriali)
        {
            SetCulture();

            areaAziendeEditoriali = new AreaAziendeEditorialiPerTipo0171();
            AreaEsito esito = new AreaEsito();
            try
            {
                #region anagraficaAccordi

                //Caricamento AnagraficaAccordi
                List<Entity.AnagraficaAccordoPerTipo0171> elencoAnagraficaAccordi = null;
                GestioneAreaAziendeEditorialiPerTipo0171.GetDecodificaAnagraficaAccordi(out elencoAnagraficaAccordi);
                if (elencoAnagraficaAccordi != null && elencoAnagraficaAccordi.Count > 0)
                    areaAziendeEditoriali.ElencoAnagraficheAccordi = elencoAnagraficaAccordi;
                #endregion anagraficaAccordi

                #region anagraficaAziende
                //Caricamento AnagraficaAziende
                List<Entity.AnagraficaAziendaPerTipo0171> elencoAnagraficaAziende = null;
                GestioneAreaAziendeEditorialiPerTipo0171.GetDecodificaAnagraficaAziende(out elencoAnagraficaAziende);
                if (elencoAnagraficaAziende != null && elencoAnagraficaAziende.Count > 0)
                    areaAziendeEditoriali.ElencoAnagraficheAziende = elencoAnagraficaAziende;
                #endregion anagraficaAziende
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero dell'elenco delle Aziende Editoriali art.1 c. 154 legge 205/2017";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito SalvaAnagraficaAccordiPerTipo0171(ref AreaAziendeEditorialiPerTipo0171 areaAziendeEditoriali)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaAziendeEditoriali == null || areaAziendeEditoriali.AnagraficheAccordi == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Anagrafica Accordi da salvare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEditorialiPerTipo0171.SalvaAnagraficaAccordi(areaAziendeEditoriali.AnagraficheAccordi, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }

                //carica tutte le liste
                GetAllAziendeEditorialiPerTipo0171(out areaAziendeEditoriali);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento dell'Anagrafica Accordi per Aziende Editoriali art.1 c. 154 legge 205/2017";
                return esito;
            }

            return esito;
        }

        public AreaEsito EliminaAnagraficaAccordiPerTipo0171(ref AreaAziendeEditorialiPerTipo0171 areaAziendeEditoriali)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            string messaggioVideo = null;
            try
            {
                if (areaAziendeEditoriali == null || areaAziendeEditoriali.AnagraficheAccordi == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Anagrafica Accordi da eliminare");
                else
                {
                    GestioneAreaAziendeEditorialiPerTipo0171.DeleteAnagraficaAccordi(areaAziendeEditoriali.AnagraficheAccordi, out messaggioVideo);
                }

                //caricamento di tute le le liste
                GetAllAziendeEditorialiPerTipo0171(out areaAziendeEditoriali);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nella cancellazione dell'Anagrafica Accordi per Aziende Editoriali art.1 c. 154 legge 205/2017";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = messaggioVideo;
            return esito;
        }

        public AreaEsito SalvaAnagraficaAziendePerTipo0171(ref AreaAziendeEditorialiPerTipo0171 areaAziendeEditoriali)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaAziendeEditoriali == null || areaAziendeEditoriali.AnagraficheAziende == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Anagrafica Aziende da salvare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEditorialiPerTipo0171.SalvaAnagraficaAziende(areaAziendeEditoriali.AnagraficheAziende, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }

                //carica tutte le liste
                GetAllAziendeEditorialiPerTipo0171(out areaAziendeEditoriali);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento dell'Anagrafica Aziende per Aziende Editoriali art.1 c. 154 legge 205/2017";
                return esito;
            }

            return esito;
        }

        public AreaEsito EliminaAnagraficaAziendePerTipo0171(ref AreaAziendeEditorialiPerTipo0171 areaAziendeEditoriali)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaAziendeEditoriali == null || areaAziendeEditoriali.AnagraficheAziende == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Anagrafica Aziende da eliminare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEditorialiPerTipo0171.DeleteAnagraficaAziende(areaAziendeEditoriali.AnagraficheAziende, out messaggioVideo);
                }

                //caricamento di tute le le liste
                GetAllAziendeEditorialiPerTipo0171(out areaAziendeEditoriali);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nella cancellazione dell'Anagrafica Aziende per Aziende Editoriali art.1 c. 154 legge 205/2017";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        #endregion Aziende Editoriali art.1 c. 154 legge 205/2017

        #region Aziende Editoriali art. 1 c. 500 L.160/2019

        public AreaEsito GetAllAziendeEditorialiPerTipo0179(out AreaAziendeEditorialiPerTipo0179 areaAziendeEditoriali)
        {
            SetCulture();

            areaAziendeEditoriali = new AreaAziendeEditorialiPerTipo0179();
            AreaEsito esito = new AreaEsito();
            try
            {
                #region anagraficaAccordi

                //Caricamento AnagraficaAccordi
                List<Entity.AnagraficaAccordoPerTipo0179> elencoAnagraficaAccordi = null;
                GestioneAreaAziendeEditorialiPerTipo0179.GetDecodificaAnagraficaAccordi(out elencoAnagraficaAccordi);
                if (elencoAnagraficaAccordi != null && elencoAnagraficaAccordi.Count > 0)
                    areaAziendeEditoriali.ElencoAnagraficheAccordi = elencoAnagraficaAccordi;
                #endregion anagraficaAccordi

                #region anagraficaAziende
                //Caricamento AnagraficaAziende
                List<Entity.AnagraficaAziendaPerTipo0179> elencoAnagraficaAziende = null;
                GestioneAreaAziendeEditorialiPerTipo0179.GetDecodificaAnagraficaAziende(out elencoAnagraficaAziende);
                if (elencoAnagraficaAziende != null && elencoAnagraficaAziende.Count > 0)
                    areaAziendeEditoriali.ElencoAnagraficheAziende = elencoAnagraficaAziende;
                #endregion anagraficaAziende
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero dell'elenco delle Aziende Editoriali art. 1 c. 500 L.160/2019";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito SalvaAnagraficaAccordiPerTipo0179(ref AreaAziendeEditorialiPerTipo0179 areaAziendeEditoriali)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaAziendeEditoriali == null || areaAziendeEditoriali.AnagraficheAccordi == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Anagrafica Accordi da salvare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEditorialiPerTipo0179.SalvaAnagraficaAccordi(areaAziendeEditoriali.AnagraficheAccordi, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }

                //carica tutte le liste
                GetAllAziendeEditorialiPerTipo0179(out areaAziendeEditoriali);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento dell'Anagrafica Accordi per Aziende Editoriali art. 1 c. 500 L.160/2019";
                return esito;
            }

            return esito;
        }

        public AreaEsito EliminaAnagraficaAccordiPerTipo0179(ref AreaAziendeEditorialiPerTipo0179 areaAziendeEditoriali)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            string messaggioVideo = null;
            try
            {
                if (areaAziendeEditoriali == null || areaAziendeEditoriali.AnagraficheAccordi == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Anagrafica Accordi da eliminare");
                else
                {
                    GestioneAreaAziendeEditorialiPerTipo0179.DeleteAnagraficaAccordi(areaAziendeEditoriali.AnagraficheAccordi, out messaggioVideo);
                }

                //caricamento di tute le le liste
                GetAllAziendeEditorialiPerTipo0179(out areaAziendeEditoriali);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nella cancellazione dell'Anagrafica Accordi per Aziende Editoriali art. 1 c. 500 L.160/2019";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = messaggioVideo;
            return esito;
        }

        public AreaEsito SalvaAnagraficaAziendePerTipo0179(ref AreaAziendeEditorialiPerTipo0179 areaAziendeEditoriali)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaAziendeEditoriali == null || areaAziendeEditoriali.AnagraficheAziende == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Anagrafica Aziende da salvare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEditorialiPerTipo0179.SalvaAnagraficaAziende(areaAziendeEditoriali.AnagraficheAziende, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }

                //carica tutte le liste
                GetAllAziendeEditorialiPerTipo0179(out areaAziendeEditoriali);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento dell'Anagrafica Aziende per Aziende Editoriali art. 1 c. 500 L.160/2019";
                return esito;
            }

            return esito;
        }

        public AreaEsito EliminaAnagraficaAziendePerTipo0179(ref AreaAziendeEditorialiPerTipo0179 areaAziendeEditoriali)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaAziendeEditoriali == null || areaAziendeEditoriali.AnagraficheAziende == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Anagrafica Aziende da eliminare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEditorialiPerTipo0179.DeleteAnagraficaAziende(areaAziendeEditoriali.AnagraficheAziende, out messaggioVideo);
                }

                //caricamento di tute le le liste
                GetAllAziendeEditorialiPerTipo0179(out areaAziendeEditoriali);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nella cancellazione dell'Anagrafica Aziende per Aziende Editoriali art. 1 c. 500 L.160/2019";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        #endregion Aziende Editoriali art. 1 c. 500 L.160/2019

        #region ProvvisoriePerCoefficienti
        public AreaEsito GetDataDecorrenzaProvvisorieObbligatoriePerCoefficienti(Utility.TipoAppartenenza? tipoAppartenenza, out AreaProvvisoriePerCoefficienti areaProvvisoriePerCoefficienti)
        {
            SetCulture();

            areaProvvisoriePerCoefficienti = new AreaProvvisoriePerCoefficienti();
            AreaEsito esito = new AreaEsito();

            try
            {
                DateTime? dataDecorrenzaProvvisoriaObbligatoria = null;
                GestioneAreaProvvisoriePerCoefficienti.GetDecorrenzaProvvisoriaObbligatoriaPerTipoAppartenenza(tipoAppartenenza, out dataDecorrenzaProvvisoriaObbligatoria);
                areaProvvisoriePerCoefficienti.DataDecorrenzaProvvisoriaObbligatoria = dataDecorrenzaProvvisoriaObbligatoria;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }

            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return esito;
        }

        public AreaEsito SetDataDecorrenzaProvvisorieObbligatoriePerCoefficienti(Utility.TipoAppartenenza? tipoAppartenenza, AreaProvvisoriePerCoefficienti areaProvvisoriePerCoefficienti)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();

            try
            {
                if (areaProvvisoriePerCoefficienti == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Data Decorrenza Provvisoria da salvare");
                else
                {
                    string messaggioVideo = null;

                    GestioneAreaProvvisoriePerCoefficienti.SetDecorrenzaProvvisoriaObbligatoriaPerTipoAppartenenza(tipoAppartenenza, areaProvvisoriePerCoefficienti.DataDecorrenzaProvvisoriaObbligatoria, out messaggioVideo);
                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }

            return esito;
        }
        #endregion ProvvisoriePerCoefficienti

        #region Area Abilitazione Servizi
        public AreaEsito GetAreaAbilitazioneServizi(out AreaAbilitazioneServizi areaAbilitazioneServizi)
        {
            SetCulture();

            areaAbilitazioneServizi = new AreaAbilitazioneServizi();
            AreaEsito esito = new AreaEsito();
            try
            {
                Dictionary<string, bool> elencoServizi = null;
                GestioneAreaAbilitazioneServizi.GetAreaServizi(out elencoServizi);

                bool value;
                elencoServizi.TryGetValue(GestioneControlliDinamici.Keys.PolarizzazioneENPALSAttiva, out value);
                areaAbilitazioneServizi.IsPolarizzazioneENPALSAbilitata = value;

                elencoServizi.TryGetValue(GestioneControlliDinamici.Keys.PolarizzazioneSuperstitiENPALSAttiva, out value);
                areaAbilitazioneServizi.IsPolarizzazioneSuperstitiENPALSAbilitata = value;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero delle abilitazioni dei servizi.";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito SetPolarizzazioneENPALSAttivo(AreaAbilitazioneServizi areaAbilitazioneServizi)
        {
            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaAbilitazioneServizi == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna selezione di Abilitazione nuova polarizzazione delle domande ENPALS da salvare");

                else
                {
                    string messaggioVideo = null;
                    bool isPolarizzazioneENPALS = areaAbilitazioneServizi.IsPolarizzazioneENPALSAbilitata;
                    bool isPolarizzazioneSuperstitiENPALS = areaAbilitazioneServizi.IsPolarizzazioneSuperstitiENPALSAbilitata;
                    GestioneAreaAbilitazioneServizi.SetAbilitazionePolarizzazioneENPALS(isPolarizzazioneENPALS, isPolarizzazioneSuperstitiENPALS, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }

            return esito;
        }
        #endregion Area Abilitazione Servizi

        #region AziendeVOESO

        public AreaEsito GetAllAziendeVOESO(string tipoAzienda, out AreaAziendeVOESO areaAziendaVOESO)
        {
            SetCulture();

            areaAziendaVOESO = new AreaAziendeVOESO();
            AreaEsito esito = new AreaEsito();
            try
            {
                List<AziendeVOESO> listaAziende = null;
                GestioneAreaAziendeEAziendeVOESO.GetDecodificaAziendeEAziendeVOESO(tipoAzienda, out listaAziende);
                if (listaAziende != null && listaAziende.Count > 0)
                {
                    if (areaAziendaVOESO.ElencoAziendeVOESO == null)
                        areaAziendaVOESO.ElencoAziendeVOESO = new List<AziendeVOESO>();
                    foreach (AziendeVOESO la in listaAziende)
                        areaAziendaVOESO.ElencoAziendeVOESO.Add(la);
                }
            }
            catch (Exception Ex)
            {
                DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero delle aziende VOESO";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito SalvaAziendeVOESO(string tipoAzienda, ref AreaAziendeVOESO areaAziendaVOESO)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaAziendaVOESO == null)
                    throw new DNA.DnaValidationException("Nessuna Azienda VOESO da salvare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEAziendeVOESO.SalvaAziendeVOESO(areaAziendaVOESO.AziendaVOESO, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }
            }
            catch (DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento delle Aziende VOESO";
            }
            try
            {
                //caricamento aziende VOESO
                List<AziendeVOESO> elencoAziendeVOESO = null;
                GestioneAreaAziendeEAziendeVOESO.GetDecodificaAziendeEAziendeVOESO(tipoAzienda, out elencoAziendeVOESO);
                if (elencoAziendeVOESO != null && elencoAziendeVOESO.Count > 0)
                {
                    if (areaAziendaVOESO.ElencoAziendeVOESO == null)
                        areaAziendaVOESO.ElencoAziendeVOESO = new List<AziendeVOESO>();
                    foreach (AziendeVOESO aVOESO in elencoAziendeVOESO)
                        areaAziendaVOESO.ElencoAziendeVOESO.Add(aVOESO);
                }
            }
            catch (Exception)
            {
                //Eccezione ignorata
            }
            return esito;
        }

        public AreaEsito EliminaAziendeVOESO(string tipoAzienda, ref AreaAziendeVOESO areaAziendaVOESO)
        {
            SetCulture();

            AreaEsito esito = null;
            try
            {
                if (areaAziendaVOESO == null)
                    throw new DNA.DnaValidationException("Nessuna Azienda VOESO da eliminare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEAziendeVOESO.DeleteAziendeVOESO(areaAziendaVOESO.AziendaVOESO, out messaggioVideo);
                }
            }
            catch (DNA.DnaValidationException Ex)
            {
                esito = new AreaEsito();
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                esito = new AreaEsito();
                DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nella cancellazione delle Aziende VOESO";

            }
            try
            {
                //caricamento aziende VOESO
                List<AziendeVOESO> elencoAziendeVOESO = null;
                GestioneAreaAziendeEAziendeVOESO.GetDecodificaAziendeEAziendeVOESO(tipoAzienda, out elencoAziendeVOESO);
                if (elencoAziendeVOESO != null && elencoAziendeVOESO.Count > 0)
                {
                    if (areaAziendaVOESO.ElencoAziendeVOESO == null)
                        areaAziendaVOESO.ElencoAziendeVOESO = new List<AziendeVOESO>();
                    foreach (AziendeVOESO aVOESO in elencoAziendeVOESO)
                        areaAziendaVOESO.ElencoAziendeVOESO.Add(aVOESO);
                }
            }
            catch (Exception)
            {
                //Eccezione ignorata
            }

            if (esito == null)
            {
                esito = new AreaEsito();
                esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                esito.Messaggio = string.Empty;
            }
            return esito;
        }

        #endregion AziendeVOESO

        #region AziendeESOTEL
        public AreaEsito GetAllAziendeESOTEL(out AreaAziendeESOTEL areaAziendaESOTEL)
        {
            SetCulture();

            areaAziendaESOTEL = new AreaAziendeESOTEL();
            AreaEsito esito = new AreaEsito();
            try
            {
                List<Entity.AziendeESOTEL> listaAziende = null;
                GestioneAreaAziendeEAziendeESOTEL.GetDecodificaAziendeEAziendeESOTEL(out listaAziende);
                if (listaAziende != null && listaAziende.Count > 0)
                {
                    if (areaAziendaESOTEL.elencoAziendeESOTEL == null)
                        areaAziendaESOTEL.elencoAziendeESOTEL = new List<Entity.AziendeESOTEL>();
                    foreach (Entity.AziendeESOTEL la in listaAziende)
                        areaAziendaESOTEL.elencoAziendeESOTEL.Add(la);
                }
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero delle aziende ESOTEL";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito SalvaAziendeESOTEL(ref AreaAziendeESOTEL areaAziendaESOTEL)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaAziendaESOTEL == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Azienda ESOTEL da salvare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEAziendeESOTEL.SalvaAziendeESOTEL(areaAziendaESOTEL.AziendaESOTEL, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento delle Aziende ESOTEL";
            }
            try
            {
                //caricamento aziende ESOTEL
                List<Entity.AziendeESOTEL> elencoAziendeESOTEL = null;
                GestioneAreaAziendeEAziendeESOTEL.GetDecodificaAziendeEAziendeESOTEL(out elencoAziendeESOTEL);
                if (elencoAziendeESOTEL != null && elencoAziendeESOTEL.Count > 0)
                {
                    if (areaAziendaESOTEL.elencoAziendeESOTEL == null)
                        areaAziendaESOTEL.elencoAziendeESOTEL = new List<AziendeESOTEL>();
                    foreach (Entity.AziendeESOTEL aESOTEL in elencoAziendeESOTEL)
                        areaAziendaESOTEL.elencoAziendeESOTEL.Add(aESOTEL);
                }
            }
            catch (Exception)
            {
                //Eccezione ignorata
            }

            return esito;
        }

        public AreaEsito EliminaAziendeESOTEL(ref AreaAziendeESOTEL areaAziendaESOTEL)
        {
            SetCulture();

            AreaEsito esito = null;
            try
            {
                if (areaAziendaESOTEL == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna AziendaESOTEL da eliminare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEAziendeESOTEL.DeleteAziendeESOTEL(areaAziendaESOTEL.AziendaESOTEL, out messaggioVideo);
                }
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito = new AreaEsito();
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                esito = new AreaEsito();
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nella cancellazione delle AziendeESOTEL";
            }

            try
            {
                //caricamento aziende ESOTEL
                List<Entity.AziendeESOTEL> elencoAziendeESOTEL = null;
                GestioneAreaAziendeEAziendeESOTEL.GetDecodificaAziendeEAziendeESOTEL(out elencoAziendeESOTEL);
                if (elencoAziendeESOTEL != null && elencoAziendeESOTEL.Count > 0)
                {
                    if (areaAziendaESOTEL.elencoAziendeESOTEL == null)
                        areaAziendaESOTEL.elencoAziendeESOTEL = new List<AziendeESOTEL>();
                    foreach (Entity.AziendeESOTEL azESOTEL in elencoAziendeESOTEL)
                        areaAziendaESOTEL.elencoAziendeESOTEL.Add(azESOTEL);
                }
            }
            catch (Exception)
            {
                //Eccezione ignorata
            }

            if (esito == null)
            {
                esito = new AreaEsito();
                esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                esito.Messaggio = string.Empty;
            }
            return esito;
        }
        #endregion AziendeESOTEL

        #region AziendeESOAMB
        public AreaEsito GetAllAziendeESOAMB(out AreaAziendeESOAMB areaAziendaESOAMB)
        {
            SetCulture();

            areaAziendaESOAMB = new AreaAziendeESOAMB();
            AreaEsito esito = new AreaEsito();
            try
            {
                #region Aziende ESOAMB
                List<Entity.AziendeESOAMB> listaAziende = null;
                GestioneAreaAziendeEAziendeESOAMB.GetDecodificaAziendeEAziendeESOAMB(out listaAziende);
                if (listaAziende != null && listaAziende.Count > 0)
                {
                    if (areaAziendaESOAMB.ElencoAziendeESOAMB == null)
                        areaAziendaESOAMB.ElencoAziendeESOAMB = new List<Entity.AziendeESOAMB>();
                    foreach (Entity.AziendeESOAMB la in listaAziende)
                        areaAziendaESOAMB.ElencoAziendeESOAMB.Add(la);
                }
                #endregion Aziende ESOAMB

                #region aziendeGGmmAAAA
                //caricamento aziende gg mm aaaa
                List<GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA> elencoAziendeGGmmAAAA = null;
                GestioneAziendeScadenzaAssegnoGGmmAAAA.GetDecodificaAziendeScadenzaAssegnoGGmmAAAA(out elencoAziendeGGmmAAAA);
                if (elencoAziendeGGmmAAAA != null && elencoAziendeGGmmAAAA.Count > 0)
                {
                    if (areaAziendaESOAMB.ElencoAziendeAssegnoGGmmAAAA == null)
                        areaAziendaESOAMB.ElencoAziendeAssegnoGGmmAAAA = new List<GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA>();
                    foreach (GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA aGGmmAAAA in elencoAziendeGGmmAAAA.FindAll(x => x.SiglaCatPensione.Trim() == "ESOAMB"))
                        areaAziendaESOAMB.ElencoAziendeAssegnoGGmmAAAA.Add(aGGmmAAAA);
                }
                #endregion aziendeGGmmAAAA
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero delle aziende ESOAMB";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito SalvaAziendeESOAMB(ref AreaAziendeESOAMB areaAziendaESOAMB)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaAziendaESOAMB == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Azienda ESOAMB da salvare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEAziendeESOAMB.SalvaAziendeESOAMB(areaAziendaESOAMB.AziendaESOAMB, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento delle Aziende ESOAMB";
            }
            try
            {
                //caricamento aziende ESOAMB
                List<Entity.AziendeESOAMB> elencoAziendeESOAMB = null;
                GestioneAreaAziendeEAziendeESOAMB.GetDecodificaAziendeEAziendeESOAMB(out elencoAziendeESOAMB);
                if (elencoAziendeESOAMB != null && elencoAziendeESOAMB.Count > 0)
                {
                    if (areaAziendaESOAMB.ElencoAziendeESOAMB == null)
                        areaAziendaESOAMB.ElencoAziendeESOAMB = new List<AziendeESOAMB>();
                    foreach (Entity.AziendeESOAMB aESOAMB in elencoAziendeESOAMB)
                        areaAziendaESOAMB.ElencoAziendeESOAMB.Add(aESOAMB);
                }

                #region aziendeGGmmAAAA
                //caricamento aziende gg mm aaaa
                List<GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA> elencoAziendeGGmmAAAA = null;
                GestioneAziendeScadenzaAssegnoGGmmAAAA.GetDecodificaAziendeScadenzaAssegnoGGmmAAAA(out elencoAziendeGGmmAAAA);
                if (elencoAziendeGGmmAAAA != null && elencoAziendeGGmmAAAA.Count > 0)
                {
                    if (areaAziendaESOAMB.ElencoAziendeAssegnoGGmmAAAA == null)
                        areaAziendaESOAMB.ElencoAziendeAssegnoGGmmAAAA = new List<GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA>();
                    foreach (GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA aGGmmAAAA in elencoAziendeGGmmAAAA.FindAll(x => x.SiglaCatPensione.Trim() == "ESOAMB"))
                        areaAziendaESOAMB.ElencoAziendeAssegnoGGmmAAAA.Add(aGGmmAAAA);
                }
                #endregion aziendeGGmmAAAA
            }
            catch (Exception)
            {
                //Eccezione ignorata
            }

            return esito;
        }

        public AreaEsito EliminaAziendeESOAMB(ref AreaAziendeESOAMB areaAziendaESOAMB)
        {
            SetCulture();

            AreaEsito esito = null;
            try
            {
                if (areaAziendaESOAMB == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna AziendaESOAMB da eliminare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaAziendeEAziendeESOAMB.DeleteAziendeESOAMB(areaAziendaESOAMB.AziendaESOAMB, out messaggioVideo);
                }
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito = new AreaEsito();
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                esito = new AreaEsito();
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nella cancellazione delle AziendeESOAMB";
            }

            try
            {
                //caricamento aziende ESOAMB
                List<Entity.AziendeESOAMB> elencoAziendeESOAMB = null;
                GestioneAreaAziendeEAziendeESOAMB.GetDecodificaAziendeEAziendeESOAMB(out elencoAziendeESOAMB);
                if (elencoAziendeESOAMB != null && elencoAziendeESOAMB.Count > 0)
                {
                    if (areaAziendaESOAMB.ElencoAziendeESOAMB == null)
                        areaAziendaESOAMB.ElencoAziendeESOAMB = new List<AziendeESOAMB>();
                    foreach (Entity.AziendeESOAMB azESOAMB in elencoAziendeESOAMB)
                        areaAziendaESOAMB.ElencoAziendeESOAMB.Add(azESOAMB);
                }
                #region aziendeGGmmAAAA
                //caricamento aziende gg mm aaaa
                List<GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA> elencoAziendeGGmmAAAA = null;
                GestioneAziendeScadenzaAssegnoGGmmAAAA.GetDecodificaAziendeScadenzaAssegnoGGmmAAAA(out elencoAziendeGGmmAAAA);
                if (elencoAziendeGGmmAAAA != null && elencoAziendeGGmmAAAA.Count > 0)
                {
                    if (areaAziendaESOAMB.ElencoAziendeAssegnoGGmmAAAA == null)
                        areaAziendaESOAMB.ElencoAziendeAssegnoGGmmAAAA = new List<GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA>();
                    foreach (GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA aGGmmAAAA in elencoAziendeGGmmAAAA.FindAll(x => x.SiglaCatPensione.Trim() == "ESOAMB"))
                        areaAziendaESOAMB.ElencoAziendeAssegnoGGmmAAAA.Add(aGGmmAAAA);
                }
                #endregion aziendeGGmmAAAA
            }
            catch (Exception)
            {
                //Eccezione ignorata
            }

            if (esito == null)
            {
                esito = new AreaEsito();
                esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                esito.Messaggio = string.Empty;
            }
            return esito;
        }

        public AreaEsito SalvaAziendaESOAMBGGmmAAAA(ref AreaAziendeESOAMB areaAziendaESOAMB)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaAziendaESOAMB == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Azienda da salvare");
                else
                {
                    string messaggioVideo = null;

                    GestioneAreaAziendeEAziendeESOAMB.SalvaAziendeScadAssegnoGGmmAAAA(areaAziendaESOAMB.AziendaGGmmAAAA, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }

                GetAllAziendeESOAMB(out areaAziendaESOAMB);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento delle AziendeGGmmAAAA";
                return esito;
            }

            return esito;
        }

        public AreaEsito EliminaAziendaESOAMBGGmmAAAA(ref AreaAziendeESOAMB areaAziendaESOAMB)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaAziendaESOAMB == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Banca Fideiussoria da eliminare");
                else
                {
                    string messaggioVideo = null;

                    GestioneAreaBancaFideiussione.DeleteAziendeScadAssegnoGGmmAAAA(areaAziendaESOAMB.AziendaGGmmAAAA, out messaggioVideo);
                }

                GetAllAziendeESOAMB(out areaAziendaESOAMB);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nella cancellazione delle Aziende con Scadenza assegno in formato GGmmAAAA";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;

        }
        #endregion AziendeESOAMB

        #region Area Altre Funzioni
        public AreaEsito GetAltreFunzioniByMatricola(string matricola, out AreaAltreFunzioni areaAltreFunzioni)
        {
            SetCulture();
            AreaEsito esito = new AreaEsito();
            areaAltreFunzioni = new AreaAltreFunzioni();
            try
            {
                AltreFunzioni abilitazioni = null;
                GestioneAreaAltreFunzioni.GetAbilitazioniByMatricola(matricola, out abilitazioni);
                areaAltreFunzioni.Abilitazioni = abilitazioni;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero delle aziende VESO33";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }
        #endregion Area Altre Funzioni

        #region AdesioneFondoCredito
        public AreaEsito VerificaAdesioneFondoCredito(string codiceFiscaleTitolare)
        {
            SetCulture();
            AreaEsito esito = new AreaEsito();
            esito.Messaggio = string.Empty;
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;

            try
            {
                bool esisteAdesioneFondoCredito = GestioneFondoCredito.VerificaAdesioneFondoCredito(codiceFiscaleTitolare);
                if (!esisteAdesioneFondoCredito)
                {
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    esito.Messaggio = "Non risulta iscrizione al Fondo credito";
                }
            }
            catch (Exception ex)
            {
                INPS.DNA.Logging.Logger.LogException(ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore durante la verifica di Adesione al Fondo Credito";
                return esito;
            }

            return esito;
        }
        #endregion

        #region Banche Fideiussione, Aziende e AziendeGGmmAAAA ESOPMI

        public AreaEsito GetAllBancheFideiussioneESOPMI(out AreaBancaFideiussioneESOPMI areaBancaFideiussione)
        {
            SetCulture();

            areaBancaFideiussione = new AreaBancaFideiussioneESOPMI();
            AreaEsito esito = new AreaEsito();
            try
            {
                #region banche
                //Caricamento Banche
                List<GestioneBancheFideiussioneESOPMI.DecBancaFideiussione> elencoBancheFideiuss = null;
                GestioneAreaBancaFideiussioneESOPMI.GetDecodificaBancaFideiussione(out elencoBancheFideiuss);
                if (elencoBancheFideiuss != null && elencoBancheFideiuss.Count > 0)
                {
                    if (areaBancaFideiussione.ElencoBancheFideiussione == null)
                        areaBancaFideiussione.ElencoBancheFideiussione = new List<GestioneBancheFideiussioneESOPMI.DecBancaFideiussione>();
                    foreach (GestioneBancheFideiussioneESOPMI.DecBancaFideiussione bf in elencoBancheFideiuss)
                        areaBancaFideiussione.ElencoBancheFideiussione.Add(bf);
                }
                #endregion banche

                #region aziende
                //Caricamento Aziende
                List<GestioneDecodificaAzienda.DecAzienda> elencoAziende = null;
                GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria("ESOPMI", null, out elencoAziende);

                if (elencoAziende != null && elencoAziende.Count > 0)
                {
                    if (areaBancaFideiussione.ElencoAziende == null)
                        areaBancaFideiussione.ElencoAziende = new List<GestioneDecodificaAzienda.DecAzienda>();
                    foreach (GestioneDecodificaAzienda.DecAzienda a in elencoAziende)
                        areaBancaFideiussione.ElencoAziende.Add(a);
                }
                #endregion aziende

                #region aziendeGGmmAAAA
                //caricamento aziende gg mm aaaa
                List<GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA> elencoAziendeGGmmAAAA = null;
                GestioneAziendeScadenzaAssegnoGGmmAAAA.GetDecodificaAziendeScadenzaAssegnoGGmmAAAA(out elencoAziendeGGmmAAAA);
                if (elencoAziendeGGmmAAAA != null && elencoAziendeGGmmAAAA.Count > 0)
                {
                    if (areaBancaFideiussione.ElencoAziendeAssegnoGGmmAAAA == null)
                        areaBancaFideiussione.ElencoAziendeAssegnoGGmmAAAA = new List<GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA>();
                    foreach (GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA aGGmmAAAA in elencoAziendeGGmmAAAA.FindAll(x => x.SiglaCatPensione.Trim() == "ESOPMI"))
                        areaBancaFideiussione.ElencoAziendeAssegnoGGmmAAAA.Add(aGGmmAAAA);
                }
                #endregion aziendeGGmmAAAA
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero dell'elenco delle Banche Fideiussorie ESOPMI";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito SalvaBancheFideiussioneESOPMI(ref AreaBancaFideiussioneESOPMI areaBancaFideiussione)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaBancaFideiussione == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Banca Fideiussoria da salvare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaBancaFideiussioneESOPMI.SalvaBancheFideiussione(areaBancaFideiussione.BancaFideiussione, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }

                //carica tutte le liste
                GetAllBancheFideiussioneESOPMI(out areaBancaFideiussione);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento delle Banche Fideiussorie ESOPMI";
                return esito;
            }

            return esito;
        }

        public AreaEsito EliminaBancheFideiussioneESOPMI(ref AreaBancaFideiussioneESOPMI areaBancaFideiussione)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaBancaFideiussione == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Banca Fideiussoria ESOPMI da eliminare");
                else
                {
                    string messaggioVideo = null;
                    GestioneAreaBancaFideiussioneESOPMI.DeleteBancheFideiussione(areaBancaFideiussione.BancaFideiussione, out messaggioVideo);
                }

                //caricamento di tute le le liste
                GetAllBancheFideiussioneESOPMI(out areaBancaFideiussione);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nella cancellazione delle Banche Fideiussorie ESOPMI";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito SalvaAziendaESOPMI(ref AreaBancaFideiussioneESOPMI areaBancaFideiussioneAzienda)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaBancaFideiussioneAzienda == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Azienda ESOPMI da salvare");
                else
                {
                    string messaggioVideo = null;

                    GestioneAreaBancaFideiussioneESOPMI.SalvaAziende(areaBancaFideiussioneAzienda.Azienda, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }

                //caricamento di tute le liste
                GetAllBancheFideiussioneESOPMI(out areaBancaFideiussioneAzienda);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento delle Aziende ESOPMI";
                return esito;
            }

            return esito;
        }

        public AreaEsito SalvaAziendaESOPMIGGmmAAAA(ref AreaBancaFideiussioneESOPMI areaBancaFideiussione)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaBancaFideiussione == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Azienda ESOPMI da salvare");
                else
                {
                    string messaggioVideo = null;

                    GestioneAreaBancaFideiussioneESOPMI.SalvaAziendeScadAssegnoGGmmAAAA(areaBancaFideiussione.AziendaGGmmAAAA, out messaggioVideo);

                    if (!string.IsNullOrEmpty(messaggioVideo))
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        esito.Messaggio = messaggioVideo;
                    }
                    else
                    {
                        esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                        esito.Messaggio = string.Empty;
                    }
                }

                GetAllBancheFideiussioneESOPMI(out areaBancaFideiussione);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell'inserimento delle AziendeGGmmAAAA ESOPMI";
                return esito;
            }

            return esito;
        }

        public AreaEsito EliminaAziendaESOPMIGGmmAAAA(ref AreaBancaFideiussioneESOPMI areaBancaFideiussioneAziendaGGmmAAAA)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            try
            {
                if (areaBancaFideiussioneAziendaGGmmAAAA == null)
                    throw new INPS.DNA.DnaValidationException("Nessuna Banca Fideiussoria da eliminare");
                else
                {
                    string messaggioVideo = null;

                    GestioneAreaBancaFideiussioneESOPMI.DeleteAziendeScadAssegnoGGmmAAAA(areaBancaFideiussioneAziendaGGmmAAAA.AziendaGGmmAAAA, out messaggioVideo);
                }

                //caricamento di ttue le liste
                GetAllBancheFideiussioneESOPMI(out areaBancaFideiussioneAziendaGGmmAAAA);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                return esito;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nella cancellazione delle Aziende ESOPMI con Scadenza assegno in formato GGmmAAAA";
                return esito;
            }
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;

        }

        #endregion Banche Fideiussione, Aziende e AziendeGGmmAAAA ESOPMI

        #region AreaNuovoCalcolo
        public AreaEsito InsertOrUpdateNuovoCalcolo(AreaNuovoCalcolo areaNuovoCalcolo)
        {
            SetCulture();

            AreaEsito esito = new AreaEsito();
            esito.Messaggio = "";
            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            GestioneNuovoCalcolo.DatiEsitoNuovoCalcolo datiNuovoCalcolo = new GestioneNuovoCalcolo.DatiEsitoNuovoCalcolo();
            try
            {
                string errori = "";
                Utility.ValorizzaOggetti(areaNuovoCalcolo.EsitoNuovoCalcolo, datiNuovoCalcolo);
                GestioneAreaNuovoCalcolo.InsertOrUpdateNuovoCalcolo(datiNuovoCalcolo, out errori);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                GestioneLogGenerico.SalvaLogGenerico(datiNuovoCalcolo.NDomus != null ? (long)datiNuovoCalcolo.NDomus : 0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, Ex != null ? Ex.Message : null, null, Ex != null ? Ex.StackTrace : null);
            }
            catch (Exception Ex)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                GestioneLogGenerico.SalvaLogGenerico(datiNuovoCalcolo.NDomus != null ? (long)datiNuovoCalcolo.NDomus : 0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, Ex != null ? Ex.Message : null, null, Ex != null ? Ex.StackTrace : null);
            }

            return esito;
        }

        public void GetEsitoNuovoCalcolo(long? Ndomus, string TransactionId, out AreaNuovoCalcolo areaNuovoCalcolo)
        {
            SetCulture();

            GestioneNuovoCalcolo.RispostaJson rispostaNuovoCalcolo = null;
            areaNuovoCalcolo = new AreaNuovoCalcolo();

            try
            {
                GestioneNuovoCalcolo.DatiEsitoNuovoCalcolo datiEsitoNuovoCalcolo = new GestioneNuovoCalcolo.DatiEsitoNuovoCalcolo();
                string errori = "";

                var res = GestioneAreaNuovoCalcolo.GetEsitoNuovoCalcolo(Ndomus, TransactionId, out datiEsitoNuovoCalcolo, out rispostaNuovoCalcolo, out errori);

                if (string.IsNullOrEmpty(errori))
                {
                    //Utility.ValorizzaOggetti(datiEsitoNuovoCalcolo, areaNuovoCalcolo.EsitoNuovoCalcolo);                    
                    GestionePensione.DatiPensione datiPensione = null;
                    GestionePensione.GetPensioneByNumeroDomandaAndProg(Ndomus != null ? (long)Ndomus : 0, null, out datiPensione); //rivedere lo storico?
                    areaNuovoCalcolo.Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    areaNuovoCalcolo.Esito.Messaggio = "Calcolo eseguito correttamente";
                    if (areaNuovoCalcolo.EsitoNuovoCalcolo == null)
                        areaNuovoCalcolo.EsitoNuovoCalcolo = new AreaNuovoCalcolo.DatiNuovoCalcolo();

                    areaNuovoCalcolo.EsitoNuovoCalcolo.StatoPensione = datiPensione != null ? Utility.GetDescription((Utility.StatoPensione)datiPensione.StatoPensione) : "CALCOLO VERIFY";
                }
                else
                {
                    areaNuovoCalcolo.Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    areaNuovoCalcolo.Esito.Messaggio = errori;
                    if (areaNuovoCalcolo.EsitoNuovoCalcolo == null)
                        areaNuovoCalcolo.EsitoNuovoCalcolo = new AreaNuovoCalcolo.DatiNuovoCalcolo();
                    if (res)
                    {
                        //da cambiare con i dati db scritti dallo scodatore
                        areaNuovoCalcolo.EsitoNuovoCalcolo.StatoPensione = "SCARTO VERIFY";
                    }
                    else
                        areaNuovoCalcolo.EsitoNuovoCalcolo.StatoPensione = "DA CALCOLARE";
                }
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                areaNuovoCalcolo.Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                areaNuovoCalcolo.Esito.Messaggio = Ex.Message;
            }
            catch (Exception Ex)
            {
                areaNuovoCalcolo.Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                areaNuovoCalcolo.Esito.Messaggio = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
        }


        #endregion AreaNuovoCalcolo

        #region AreaFunzioneC

        public AreaEsito CleanTipoSpecECaratterizzazione(string nDomus, ref string Caratterizzazione, out string errore)
        {
            AreaEsito esito = new AreaEsito();
            errore = string.Empty;
            bool retVal = false;
            try
            {
                retVal = GestioneAggiornamentoPECO.CleanTipoSpecECaratterizzazione(nDomus, ref Caratterizzazione);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero delle informazioni";
                return esito;
            }
            if (retVal)
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                esito.Messaggio = string.Empty;
            }
            else
            {
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore durante il Clean TipoSpec e Caratterizzazione.";
            }

            return esito;
        }


        public AreaEsito GetDatiPECO_FunzioneC(string nDomus, string codFisc, string Appartenenza, string Gestione, string Fondo, ref string Caratterizzazione, out string errore)
        {
            AreaEsito esito = new AreaEsito();
            errore = string.Empty;
            bool retVal = false;
            try
            {
                switch (Appartenenza)
                {
                    case "AGO":
                        retVal = GestioneAggiornamentoPECO.GetDatiPECO_AGO_FunzioneC(nDomus, codFisc, ref Caratterizzazione, out errore);
                        break;
                    case "FS":
                        List<string> ListaAMGFunzioneC = new List<string>() { "006", "014" };
                        switch (Gestione)
                        {
                            case "019":
                                retVal = GestioneAggiornamentoPECO.GetDatiPECO_AMG_FunzioneC(nDomus, codFisc, ref Caratterizzazione, out errore);
                                break;
                            case "007":
                                if (ListaAMGFunzioneC.Contains(Fondo))
                                    retVal = GestioneAggiornamentoPECO.GetDatiPECO_AMG_FunzioneC(nDomus, codFisc, ref Caratterizzazione, out errore);
                                else
                                    retVal = GestioneAggiornamentoPECO.GetDatiPECO_FS_FunzioneC(nDomus, codFisc, ref Caratterizzazione, out errore);
                                break;
                            default:
                                retVal = GestioneAggiornamentoPECO.GetDatiPECO_FS_FunzioneC(nDomus, codFisc, ref Caratterizzazione, out errore);
                                break;
                        }
                        break;
                    case "CI":
                        retVal = GestioneAggiornamentoPECO.GetDatiPECO_CI_FunzioneC(nDomus, codFisc, ref Caratterizzazione, out errore);
                        break;
                }

                errore = string.Empty;
                if (retVal)
                {
                    errore = "ATTENZIONE! Presenza di certificazione posticipo pensionamento: verificare i mesi interessati dall’esonero contributivo al fine di una corretta compilazione dei dati di calcolo. Circolare 82/2023. In tal caso le pensioni dovranno essere liquidate con il codice di provvisorietà";
                }
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero delle informazioni";
                return esito;
            }

            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        public AreaEsito GetCodFaseByNDomus(string nDomus, out string codFase)
        {
            AreaEsito esito = new AreaEsito();
            codFase = string.Empty;
            try
            {
                Int64 numeroDomanda = Convert.ToInt64(nDomus);
                Int64 idPensione;
                GestionePensione.GetIdPensioneByNumeroDomanda(numeroDomanda, null, out idPensione);
                GestioneLavorazione.DatiLavorazione datiLavorazione;
                GestioneLavorazione.GetLavorazioneByIdPensione(idPensione, out datiLavorazione);
                codFase = datiLavorazione.CodFase;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero delle informazioni";
                return esito;
            }

            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;
            return esito;
        }

        #endregion AreaFunzioneC

        #region AreaLog
        public void SalvaLog(long numdomanda, string methodname, string errore)
        {
            GestioneLogGenerico.SalvaLogGenerico(numdomanda, methodname, Utility.TipoLogGenerico.Informativo, errore, "", "");
        }

        //public void SalvaLogDebug(long numdomanda, string methodname, string errore)
        //{
        //    GestioneLogDebug.SalvaLogDebug(numdomanda, methodname, Utility.TipoLogDebug.Informativo, errore, "", "");
        //}

        #endregion AreaLog

        #region Indebiti
        public AreaEsito GetFlagIndebitoByDomusAndProgressivoStorico(Int64 NDomus, byte? ProgressivoStorico, out string FlagIndebito)
        {
            AreaEsito esito = new AreaEsito();
            FlagIndebito = null;
            try
            {
                GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(NDomus, ProgressivoStorico);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                if (datiPensione == null || datiPensione.FlagIndebito == null || datiPensione.FlagIndebito.Trim().Length == 0)
                {
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    return esito;
                }
                FlagIndebito = datiPensione.FlagIndebito.Trim();
                return esito;
            }
            catch (Exception Ex)
            {
                DNA.Logging.Logger.LogException(Ex);
                GestioneLogGenerico.SalvaLogGenerico(NDomus, "GetFlagIndebitoByDomusAndProgressivoStorico", Utility.TipoLogGenerico.ErroreApplicativo, Ex.Message, string.Empty, Ex.StackTrace);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero delle informazioni FlagIndebito";
            }
            return esito;
        }

        public AreaEsito GetAnteprimaDebito(long numeroDomanda, string matricola, out RootIndebitoDto indebito)
        {
            AreaEsito esito = new AreaEsito();
            indebito = null;
            try
            {
                GestioneMsIndebiti.GetAnteprimaDebito(numeroDomanda, matricola, out indebito);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                if (!indebito.Success)
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
            }
            catch (Exception Ex)
            {
                DNA.Logging.Logger.LogException(Ex);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, "GetAnteprimaDebito", Utility.TipoLogGenerico.ErroreApplicativo, Ex.Message, string.Empty, Ex.StackTrace);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero delle informazioni";
            }
            return esito;
        }

        public AreaEsito AggiornaCasuali(long numeroDomanda, string matricola, IndebitoDto indebito, bool flagCi, short sedeOperatore,
            short centroOperativoOperatore)
        {
            AreaEsito esito = new AreaEsito();
            bool success;
            try
            {
                GestioneMsIndebiti.AggiornaCasuali(numeroDomanda, matricola, indebito.ContiRic, out success);

                if (!success)
                {
                    return GestioneRitornoErrore(
                        numeroDomanda.ToString(),
                        "Errore nel ritorno chiamata AggiornaCasuali microservizio Indebiti",
                        "AggiornaCasuali",
                        string.Format("numeroDomanda: {0}, matricola: {1}", numeroDomanda, matricola),
                        string.Empty
                    );
                }

                GestioneMsIndebiti.NotificaTE08(numeroDomanda, matricola, true, out success);

                if (!success)
                {
                    return GestioneRitornoErrore(
                        numeroDomanda.ToString(),
                        "Errore nel ritorno chiamata NotificaTE08, dopo successo chiamata AggiornaCasuali, microservizio Indebiti",
                        "AggiornaCasuali",
                        string.Format("numeroDomanda: {0}, matricola: {1}", numeroDomanda, matricola),
                        string.Empty
                    );
                }

                AreaEsito esitoSalvataggioIndebito = SalvaIndebito(indebito);

                if (esitoSalvataggioIndebito.RisultatoOperazione.Equals(AreaEsito.TipoEsito.KO))
                {
                    return GestioneRitornoErrore(
                        numeroDomanda.ToString(),
                        "Errore nel salvataggio dati Indebito, dopo successo chiamata AggiornaCasuali e Notifica TE08",
                        "AggiornaCasuali",
                        string.Format("numeroDomanda: {0}, matricola: {1}", numeroDomanda, matricola),
                        string.Empty
                    );
                }

                esito = RichiamoNormaleCicloInvio(numeroDomanda.ToString(), matricola, sedeOperatore.ToString(), centroOperativoOperatore.ToString(), flagCi, "AggiornaCasuali");
                return esito;
            }
            catch (Exception Ex)
            {
                DNA.Logging.Logger.LogException(Ex);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, "AggiornaCasuali", Utility.TipoLogGenerico.ErroreApplicativo, Ex.Message, string.Empty, Ex.StackTrace);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell' operazione di aggiornamento delle causali";
            }
            return esito;
        }

        public AreaEsito NotificaTE08(long numeroDomanda, string matricola, bool flagCi, short sedeOperatore,
            short centroOperativoOperatore)
        {
            AreaEsito esito = new AreaEsito();
            bool success;
            try
            {
                GestioneMsIndebiti.NotificaTE08(numeroDomanda, matricola, false, out success);

                if (!success)
                {
                    return GestioneRitornoErrore(
                        numeroDomanda.ToString(),
                        "Errore nel ritorno chiamata NotificaTE08 microservizio Indebiti",
                        "NotificaTE08",
                        string.Format("numeroDomanda: {0}, matricola: {1}", numeroDomanda, matricola),
                        string.Empty
                    );
                }

                esito = RichiamoNormaleCicloInvio(numeroDomanda.ToString(), matricola, sedeOperatore.ToString(), centroOperativoOperatore.ToString(), flagCi, "NotificaTE08");
                return esito;
            }
            catch (Exception Ex)
            {
                DNA.Logging.Logger.LogException(Ex);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, "NotificaTE08", Utility.TipoLogGenerico.ErroreApplicativo, Ex.Message, string.Empty, Ex.StackTrace);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nell' operazione di notifica";
            }
            return esito;
        }

        public AreaEsito SalvaIndebito(IndebitoDto indebito)
        {
            return new AreaEsito()
            {
                RisultatoOperazione = GestioneIndebiti.SalvaIndebito(indebito) ? AreaEsito.TipoEsito.OK : AreaEsito.TipoEsito.KO
            };
        }
        #endregion

        #endregion public IServizioLiquidazione members

        #endregion IServizioLiquidazione members

        #region PrivateMethods
        private AreaEsito GestioneRitornoErrore(string nDomus, string errore, string methodName, string parametri, string stackTrace)
        {
            AreaEsito esito = new AreaEsito();
            esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
            esito.Messaggio = errore;
            GestioneLogGenerico.SalvaLogGenerico(long.Parse(nDomus), methodName, Utility.TipoLogGenerico.ErroreApplicativo, errore, parametri, stackTrace);
            return esito;
        }

        private AreaEsito RichiamoNormaleCicloInvio(
            string numeroDomanda,
            string matricola,
            string sedeOperatore,
            string centroOperativoOperatore,
            bool flagCi,
            string nomeChiamante)
        {
            //Cambio stato domanda
            var inputCambioStato = new AreaCambioStatoDomanda
            {
                IsUpdateOperation = true,
                NumeroDomandaUpdate = long.Parse(numeroDomanda),
                NuovoStatoPensione = flagCi ? "CALCOLO NO STAZ. LAVORO" : "CALCOLO NO WEBDOM"
            };

            var areaEsito = CambioStatoDomanda(ref inputCambioStato);
            if (areaEsito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                return GestioneRitornoErrore(
                    numeroDomanda,
                    "Errore nel ritorno chiamata CambioStatoDomanda",
                    nomeChiamante,
                    string.Format(
                        "IsUpdateOperation: {0}, NumeroDomandaUpdate: {1}, NuovoStatoPensione: {2}",
                        inputCambioStato.IsUpdateOperation,
                        inputCambioStato.NumeroDomandaUpdate,
                        inputCambioStato.NuovoStatoPensione
                    ),
                    string.Empty
                );
            }

            //AggiornaCI05 o AggiornaWebDom in base al FlagCi (con operatore ternario)
            string statoPensione;
            AreaEsito esitoAggiornaWebDom = flagCi
                ? AggiornaCI05(long.Parse(numeroDomanda), matricola, short.Parse(sedeOperatore), short.Parse(centroOperativoOperatore), out statoPensione)
                : AggiornaWebDom(long.Parse(numeroDomanda), matricola, short.Parse(sedeOperatore), short.Parse(centroOperativoOperatore), out statoPensione);

            if (esitoAggiornaWebDom.RisultatoOperazione == AreaEsito.TipoEsito.KO || statoPensione.Equals(inputCambioStato.NuovoStatoPensione))
            {
                return GestioneRitornoErrore(
                    numeroDomanda,
                    "Errore nella chiamata al calcolo metodi AggiornaCI05/AggiornaWebDom",
                    nomeChiamante,
                    string.Format(
                        "numeroDomanda: {0}, matricola: {1}, sedeOperatore: {2}, centroOperativoOperatore: {3}",
                        numeroDomanda,
                        matricola,
                        sedeOperatore,
                        centroOperativoOperatore
                    ),
                    string.Empty
                );
            }

            return new AreaEsito { RisultatoOperazione = AreaEsito.TipoEsito.OK };
        }
        #endregion
    }
}

