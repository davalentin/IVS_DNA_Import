using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Context;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Configuration;
using EntityBLCommon = INPS.Pensioni.Liquidazione.BLCommon.Entity;
using System.Reflection;
using System.Diagnostics;

namespace INPS.Pensioni.LiquidazioneAgo
{
    public class GestioneCalcoloDomanda
    {
        #region public members
        public static void CalcolaDomanda(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, string matricolaOperatore, short sedeOperatore,
            short centroOperativoOperatore, DateTime dataSistema, int annoCompetenza, bool? isNuovoCalcolo, out string statoPensione, out bool esito, out string messaggioVideo, out string transactionId)
        {
            esito = false;
            statoPensione = "";
            messaggioVideo = "";
            Data.GAPL_GARC AreaCalcolo = null;
            Guid guid = Guid.NewGuid();
            ValorizzaAreaCalcolo(ref contenitore, ref contenitoreDecodifica, matricolaOperatore, sedeOperatore, centroOperativoOperatore, dataSistema, annoCompetenza, out AreaCalcolo);

            transactionId = "";
            string jsonStringRequest = string.Empty;
            string erroriNuovo = string.Empty;
            string codiciErrore = string.Empty;
            string jsonStringResponse = string.Empty;
            string eccezioni = string.Empty;
            if (isNuovoCalcolo.GetValueOrDefault())
            {
                GestioneDecodifica.StatoEstero statoEstero = null;
                GestioneDecodifica.GetStatoEsteroPerCodiceCatastale(contenitore.DatiAreaTitolare.Anagrafica.CodiceComuneNascita.Trim(), out statoEstero);
                DateTimeOffset dataNuovo = new DateTimeOffset();
                dataNuovo = DateTimeOffset.Now;
                DateTimeOffset startTime = DateTimeOffset.UtcNow;
                transactionId = AreaCalcolo.CallMiddleware(contenitore.DatiPensione, statoEstero != null && statoEstero.Descrizione != null ? statoEstero.Descrizione.Trim() : "ITALIA", contenitore.ListaAnagraficaFamiliari, out jsonStringRequest, out erroriNuovo, out codiciErrore, out eccezioni, out jsonStringResponse);
                DateTimeOffset endTime = DateTimeOffset.UtcNow;
                if (string.IsNullOrEmpty(transactionId))
                {
                    esito = false;
                    if (!string.IsNullOrEmpty(erroriNuovo))
                        messaggioVideo = erroriNuovo;
                    else if (!string.IsNullOrEmpty(eccezioni))
                        messaggioVideo = "Si è verificato un errore imprevisto, contattare il supporto tecnico";
                    else
                        messaggioVideo = "Non è stato possibile effettuare la richiesta di calcolo";

                    GestioneLogSoap.SalvaLogSoap(messaggioVideo, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.IvsInvocation, Utility.SOAPLogDirection.OUT, contenitore.DatiPensione.NDomus.ToString(), guid);

                    //Log Abaco
                    try
                    {
                        List<GestioneNuovoCalcolo.FlowConf> lstConfFiltrata;
                        GestioneNuovoCalcolo.FlowConf confFiltrata;
                        Utility.IsPerimetroNuovoCalcoloConfDinamica(contenitore.DatiPensione, out lstConfFiltrata, contenitore.DatiPensione.FlagVerify.GetValueOrDefault());
                        var flagVerify = contenitore.DatiPensione.FlagVerify.HasValue ? contenitore.DatiPensione.FlagVerify.Value ? "1" : "0" : "1";
                        confFiltrata = lstConfFiltrata != null ? lstConfFiltrata.Find(x => x.TipoRichiesta == flagVerify && x.SistemiInvocati == "NEW") : null;
                        AreaCalcolo.CallAbaco(contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare, transactionId, jsonStringRequest, "", "", startTime, endTime, dataNuovo, esito, erroriNuovo, codiciErrore, confFiltrata, eccezioni, jsonStringResponse);
                    }
                    catch
                    {
                        //ignora
                    }
                }
                else
                    esito = true;
            }
            else
            {
                Utility.MetodoServizio? metodoServizio = (Utility.MetodoServizio)Utility.GetValueFromDescription<Utility.MetodoServizio>(AreaCalcolo.TransactionName);
                GestioneLogSoap.SalvaLogSoap(AreaCalcolo.Request, Utility.Servizio.SrvLiquidazioneAgo, metodoServizio.Value, Utility.SOAPLogDirection.IN, contenitore.DatiPensione.NDomus.ToString(), guid);

                bool doppiaChiamata = false;
                DateTimeOffset dataNuovo = new DateTimeOffset();
                GestioneNuovoCalcolo.FlowConf confFiltrata;
                if (Utility.IsDoppiaChiamataConfDinamica(contenitore.DatiPensione, contenitore.DatiPensione.FlagVerify.GetValueOrDefault(), out confFiltrata))
                {
                    dataNuovo = DateTimeOffset.Now;
                    doppiaChiamata = true;
                    GestioneDecodifica.StatoEstero statoEstero = null;
                    GestioneDecodifica.GetStatoEsteroPerCodiceCatastale(contenitore.DatiAreaTitolare.Anagrafica.CodiceComuneNascita.Trim(), out statoEstero);
                    transactionId = AreaCalcolo.CallMiddleware(contenitore.DatiPensione, statoEstero != null && statoEstero.Descrizione != null ? statoEstero.Descrizione.Trim() : "ITALIA", contenitore.ListaAnagraficaFamiliari, out jsonStringRequest, out erroriNuovo, out codiciErrore, out eccezioni, out jsonStringResponse);
                }

                DateTimeOffset startTime = DateTimeOffset.UtcNow;
                EseguiCalcolo(AreaCalcolo);
                DateTimeOffset endTime = DateTimeOffset.UtcNow;

                if (!string.IsNullOrEmpty(AreaCalcolo.MessaggioDaLoggare))
                    GestioneLogGenerico.SalvaLogGenerico(contenitore.DatiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, AreaCalcolo.MessaggioDaLoggare, null, null);

                if (AreaCalcolo.HasError)
                    GestioneLogSoap.SalvaLogSoap(AreaCalcolo.Messaggio, Utility.Servizio.SrvLiquidazioneAgo, metodoServizio.Value, Utility.SOAPLogDirection.OUT, contenitore.DatiPensione.NDomus.ToString(), guid);
                else
                    GestioneLogSoap.SalvaLogSoap(AreaCalcolo.Response, Utility.Servizio.SrvLiquidazioneAgo, metodoServizio.Value, Utility.SOAPLogDirection.OUT, contenitore.DatiPensione.NDomus.ToString(), guid);

                ControllaEsitoCalcolo(contenitore.DatiPensione.NDomus, AreaCalcolo, out statoPensione, out esito, out messaggioVideo);

                if (doppiaChiamata)
                {
                    string codErrore = "";
                    string descrError = "";
                    if (AreaCalcolo.CodiciErrore != null && AreaCalcolo.CodiciErrore.Count > 0)
                    {
                        codErrore = string.Join(";", AreaCalcolo.CodiciErrore.Select(e => e.ToString()).ToArray());
                        descrError = GetDettaglioErroreMonitoraggio(AreaCalcolo.CodiciErrore);
                    }
                    if (!string.IsNullOrEmpty(AreaCalcolo.MessaggioDaLoggare))
                        descrError = descrError + AreaCalcolo.MessaggioDaLoggare + (!string.IsNullOrEmpty(AreaCalcolo.Messaggio) ? AreaCalcolo.Messaggio : "");

                    AreaCalcolo.CallMainframe(contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare, transactionId, jsonStringRequest, codErrore, descrError, startTime, endTime, dataNuovo, esito, confFiltrata);
                    if (!string.IsNullOrEmpty(erroriNuovo))
                    {
                        AreaCalcolo.CallAbaco(contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare, transactionId, jsonStringRequest, codErrore, descrError, startTime, endTime, dataNuovo, esito, erroriNuovo, codiciErrore, confFiltrata, eccezioni, jsonStringResponse);
                    }
                }
            }
        }

        public static void CalcolaDomandaNuovoTracciato(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, string matricolaOperatore, short sedeOperatore,
            short centroOperativoOperatore, DateTime dataSistema, int annoCompetenza, bool? isNuovoCalcolo, out string statoPensione, out bool esito, out string messaggioVideo, out string transactionId, out string messaggioEccezione)
        {
            esito = false;
            statoPensione = "";
            messaggioVideo = "";
            Data.GAPL_GARC_New AreaCalcolo = null;
            Guid guid = Guid.NewGuid();
            messaggioEccezione = string.Empty;
            ValorizzaAreaCalcoloNuovoTracciato(ref contenitore, ref contenitoreDecodifica, matricolaOperatore, sedeOperatore, centroOperativoOperatore, dataSistema, annoCompetenza, out AreaCalcolo, out messaggioEccezione);

            transactionId = "";
            string jsonStringRequest = string.Empty;
            string erroriNuovo = string.Empty;
            string codiciErrore = string.Empty;
            string jsonStringResponse = string.Empty;
            string eccezioni = string.Empty;
            if (isNuovoCalcolo.GetValueOrDefault())
            {
                GestioneDecodifica.StatoEstero statoEstero = null;
                GestioneDecodifica.GetStatoEsteroPerCodiceCatastale(contenitore.DatiAreaTitolare.Anagrafica.CodiceComuneNascita.Trim(), out statoEstero);
                DateTimeOffset dataNuovo = new DateTimeOffset();
                dataNuovo = DateTimeOffset.Now;
                DateTimeOffset startTime = DateTimeOffset.UtcNow;
                transactionId = AreaCalcolo.CallMiddleware(contenitore.DatiPensione, statoEstero != null && statoEstero.Descrizione != null ? statoEstero.Descrizione.Trim() : "ITALIA", contenitore.ListaAnagraficaFamiliari, out jsonStringRequest, out erroriNuovo, out codiciErrore, out eccezioni, out jsonStringResponse);
                DateTimeOffset endTime = DateTimeOffset.UtcNow;
                if (string.IsNullOrEmpty(transactionId))
                {
                    esito = false;
                    if (!string.IsNullOrEmpty(erroriNuovo))
                        messaggioVideo = erroriNuovo;
                    else if (!string.IsNullOrEmpty(eccezioni))
                        messaggioVideo = "Si è verificato un errore imprevisto, contattare il supporto tecnico";
                    else
                        messaggioVideo = "Non è stato possibile effettuare la richiesta di calcolo";

                    GestioneLogSoap.SalvaLogSoap(messaggioVideo, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.IvsInvocation, Utility.SOAPLogDirection.OUT, contenitore.DatiPensione.NDomus.ToString(), guid);

                    //Log Abaco
                    try
                    {
                        List<GestioneNuovoCalcolo.FlowConf> lstConfFiltrata;
                        GestioneNuovoCalcolo.FlowConf confFiltrata;
                        Utility.IsPerimetroNuovoCalcoloConfDinamica(contenitore.DatiPensione, out lstConfFiltrata, contenitore.DatiPensione.FlagVerify.GetValueOrDefault());
                        var flagVerify = contenitore.DatiPensione.FlagVerify.HasValue ? contenitore.DatiPensione.FlagVerify.Value ? "1" : "0" : "1";
                        confFiltrata = lstConfFiltrata != null ? lstConfFiltrata.Find(x => x.TipoRichiesta == flagVerify && x.SistemiInvocati == "NEW") : null;
                        AreaCalcolo.CallAbaco(contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare, transactionId, jsonStringRequest, "", "", startTime, endTime, dataNuovo, esito, erroriNuovo, codiciErrore, confFiltrata, eccezioni, jsonStringResponse);
                    }
                    catch
                    {
                        //ignora
                    }
                }
                else
                    esito = true;
            }
            else
            {
                Utility.MetodoServizio? metodoServizio = (Utility.MetodoServizio)Utility.GetValueFromDescription<Utility.MetodoServizio>(AreaCalcolo.TransactionName);
                GestioneLogSoap.SalvaLogSoap(AreaCalcolo.RequestNew, Utility.Servizio.SrvLiquidazioneAgo, metodoServizio.Value, Utility.SOAPLogDirection.IN, contenitore.DatiPensione.NDomus.ToString(), guid);

                bool doppiaChiamata = false;
                DateTimeOffset dataNuovo = new DateTimeOffset();
                GestioneNuovoCalcolo.FlowConf confFiltrata;
                if (Utility.IsDoppiaChiamataConfDinamica(contenitore.DatiPensione, contenitore.DatiPensione.FlagVerify.GetValueOrDefault(), out confFiltrata))
                {
                    dataNuovo = DateTimeOffset.Now;
                    doppiaChiamata = true;
                    GestioneDecodifica.StatoEstero statoEstero = null;
                    GestioneDecodifica.GetStatoEsteroPerCodiceCatastale(contenitore.DatiAreaTitolare.Anagrafica.CodiceComuneNascita.Trim(), out statoEstero);
                    transactionId = AreaCalcolo.CallMiddleware(contenitore.DatiPensione, statoEstero != null && statoEstero.Descrizione != null ? statoEstero.Descrizione.Trim() : "ITALIA", contenitore.ListaAnagraficaFamiliari, out jsonStringRequest, out erroriNuovo, out codiciErrore, out eccezioni, out jsonStringResponse);
                }

                DateTimeOffset startTime = DateTimeOffset.UtcNow;
                EseguiCalcoloNuovoTracciato(AreaCalcolo);
                DateTimeOffset endTime = DateTimeOffset.UtcNow;

                if (!string.IsNullOrEmpty(AreaCalcolo.MessaggioDaLoggare))
                    GestioneLogGenerico.SalvaLogGenerico(contenitore.DatiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, AreaCalcolo.MessaggioDaLoggare, null, null);

                if (AreaCalcolo.HasError)
                    GestioneLogSoap.SalvaLogSoap(AreaCalcolo.Messaggio, Utility.Servizio.SrvLiquidazioneAgo, metodoServizio.Value, Utility.SOAPLogDirection.OUT, contenitore.DatiPensione.NDomus.ToString(), guid);
                else
                    GestioneLogSoap.SalvaLogSoap(AreaCalcolo.Response, Utility.Servizio.SrvLiquidazioneAgo, metodoServizio.Value, Utility.SOAPLogDirection.OUT, contenitore.DatiPensione.NDomus.ToString(), guid);

                ControllaEsitoCalcoloNuovoTracciato(contenitore.DatiPensione.NDomus, AreaCalcolo, out statoPensione, out esito, out messaggioVideo);

                if (doppiaChiamata)
                {
                    string codErrore = "";
                    string descrError = "";
                    if (AreaCalcolo.CodiciErrore != null && AreaCalcolo.CodiciErrore.Count > 0)
                    {
                        codErrore = string.Join(";", AreaCalcolo.CodiciErrore.Select(e => e.ToString()).ToArray());
                        descrError = GetDettaglioErroreMonitoraggio(AreaCalcolo.CodiciErrore);
                    }
                    if (!string.IsNullOrEmpty(AreaCalcolo.MessaggioDaLoggare))
                        descrError = descrError + AreaCalcolo.MessaggioDaLoggare + (!string.IsNullOrEmpty(AreaCalcolo.Messaggio) ? AreaCalcolo.Messaggio : "");

                    AreaCalcolo.CallMainframe(contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare, transactionId, jsonStringRequest, codErrore, descrError, startTime, endTime, dataNuovo, esito, confFiltrata);
                    if (!string.IsNullOrEmpty(erroriNuovo))
                    {
                        AreaCalcolo.CallAbaco(contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare, transactionId, jsonStringRequest, codErrore, descrError, startTime, endTime, dataNuovo, esito, erroriNuovo, codiciErrore, confFiltrata, eccezioni, jsonStringResponse);
                    }
                }
            }
        }

        public static bool ControlsDatiCalcolaDomanda(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, DateTime dataSistema,
            int annoCompetenza, bool isRiaperturaDomanda, string matricolaOperatore, bool isConsultazioniANFVerificate, out List<GestioneFamiliari.ConsultazioneUnificataANF> listaConsultazioni, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            listaConsultazioni = null;

            #region GetData

            GestioneEnpals.DatiEnpals datiENPALS = null;
            List<EntityBLCommon.DatiSupplementiENPALS> listaSupplementiEnpals = null;
            List<EntityBLCommon.DatiSuppRecordENPALS> listaSuppRecordEnpals = null;
            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
            {
                datiENPALS = contenitore.DatiEnpals;
                listaSupplementiEnpals = contenitore.ListaDatiSupplementiENPALS;
                listaSuppRecordEnpals = contenitore.ListaDatiSuppRecordENPALS;
            }

            List<GestioneFamiliari.Familiare> listaFamiliari = contenitore.ListaFamiliari.ToList();
            List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficheFamiliari = contenitore.ListaAnagraficaFamiliari.ToList();
            //ENG - Spacchettate SOPGI
            if (Utility.IsDomandaSpacchettamentoENPALS(contenitore.DatiPensione) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa)
                || Utility.IsDomandaSpacchettamentoSO(contenitore.DatiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOART(contenitore.DatiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOCOM(contenitore.DatiPensione, isRiaperturaDomanda)
                || Utility.IsDomandaSpacchettamentoSR(contenitore.DatiPensione, isRiaperturaDomanda))
            {
                string codiceFiscaleTitolare = contenitore.DatiAreaTitolare.Anagrafica.CodiceFiscale;
                listaFamiliari = listaFamiliari.FindAll(x => x.CodiceFiscale != codiceFiscaleTitolare);
            }

            List<GestioneBancheFideiussione.DecBancaFideiussione> listaDecBancaFideiussione = null;
            if (Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria))
                listaDecBancaFideiussione = contenitoreDecodifica.ElencoDecBancaFideiussione;

            List<GestioneCalcolo.QuotePensione> lQuotePensione = null;
            List<GestioneCalcolo.TrattenuteQuotePensione> lTrattenuteQuotePensione = null;
            List<GestioneDecodifica.DecEnteGestioneFondo> listaDecEnteGestioneFondo = null;
            List<GestioneDecodifica.DecodificaEnteCassaProfessionale> listaDecodificaEnteCassaProfessionale = null;
            if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria))
            {
                lQuotePensione = contenitore.ListaQuotePensione;
                if (Utility.IsRicostituzioneCumuloProgressiva(contenitore.DatiPensione) && lQuotePensione != null && lQuotePensione.Count > 0 &&
                    contenitore.ListaQuotePensioneStorico != null && contenitore.ListaQuotePensioneStorico.Count > 0)
                {
                    foreach (GestioneCalcolo.QuotePensione quota in lQuotePensione)
                    {
                        GestioneCalcolo.QuotePensione quotaStorico = contenitore.ListaQuotePensioneStorico.FirstOrDefault(x => x.EnteGestioneFondo == quota.EnteGestioneFondo);
                        if (quotaStorico != null && quotaStorico.Decorrenza.HasValue && quotaStorico.Decorrenza.Equals(new DateTime(9999, 1, 1)) &&
                            quotaStorico.Importo != null && quotaStorico.Importo <= 0.02m)
                            quota.IsQuotaProgressiva = true;
                    }
                }
                lTrattenuteQuotePensione = contenitore.ListaTrattenuteQuotePensione;
                listaDecEnteGestioneFondo = contenitoreDecodifica.ElencoDecEnteGestioneFondo;
                listaDecodificaEnteCassaProfessionale = contenitoreDecodifica.ElencoDecodificaEnteCassaProfessionale;
            }

            List<CtrlDecorrenzaRetrExINPDAI> lstCtrlDecorrenza = null;
            if (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria))
                lstCtrlDecorrenza = contenitoreDecodifica.ElencoCtrlDecorrenzaRetrExINPDAI;

            char? derogaTraduzioneSuGP = null;
            if (contenitore.DatiIstruttoria != null && contenitore.DatiIstruttoria.CodiceParticolareSoggettoDerogato.HasValue)
            {
                if (contenitoreDecodifica.ElencoCodiceParticolare != null && contenitoreDecodifica.ElencoCodiceParticolare.Count > 0)
                {
                    long valueToCompare = contenitore.DatiIstruttoria.CodiceParticolareSoggettoDerogato.Value;
                    GestioneDecodifica.CodiceParticolare codiceParticolare = contenitoreDecodifica.ElencoCodiceParticolare.Find(x => x.Id == valueToCompare);
                    if (codiceParticolare != null)
                        derogaTraduzioneSuGP = codiceParticolare.TraduzioneSuGp;
                }
            }

            string soggettoBeneficiarioTraduzioneSuGP = string.Empty;
            if (contenitore.DatiBeneficioVittimeTerrorismo != null && contenitore.DatiBeneficioVittimeTerrorismo.SoggettoBeneficiario.HasValue &&
                contenitoreDecodifica.ElencoSoggettoBeneficiario != null && contenitoreDecodifica.ElencoSoggettoBeneficiario.Count > 0)
            {
                long valueToCompare = contenitore.DatiBeneficioVittimeTerrorismo.SoggettoBeneficiario.Value;
                GestioneDecodifica.SoggettoBeneficiario soggettoBeneficiario = contenitoreDecodifica.ElencoSoggettoBeneficiario.Find(x => x.Id == valueToCompare);
                if (soggettoBeneficiario != null)
                    soggettoBeneficiarioTraduzioneSuGP = soggettoBeneficiario.TraduzioneSuGP;
            }

            GestioneDecodificaAzienda.DecAzienda codiceBancaEsodati = null;
            GestioneAziendeVESO33.DecAziendeVESO33 azVESO33 = null;
            GestioneAziendeCredito.DecAziendeCredito azCredito = null;

            if (contenitore.DatiPensione.CodiceBancaEsodati.HasValue)
            {
                short valueToCompare = contenitore.DatiPensione.CodiceBancaEsodati.Value;
                if (contenitoreDecodifica.ElencoDecAzienda != null && contenitoreDecodifica.ElencoDecAzienda.Count > 0)
                    codiceBancaEsodati = contenitoreDecodifica.ElencoDecAzienda.Find(x => x.Id == valueToCompare);

                if (Utility.IsDomandaVESO33(contenitore.DatiPensione.SiglaCategoria))
                {
                    if (codiceBancaEsodati != null)
                        GestioneAziendeVESO33.GetDecodificaAziendaVESO33ByIdCodiceAzienda(codiceBancaEsodati.Id, out azVESO33);
                }
                else if (Utility.IsDomandaVOCOOP_COOP28(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVOCRED_CRED27(contenitore.DatiPensione.SiglaCategoria))
                {
                    if (codiceBancaEsodati != null)
                        GestioneAziendeCredito.GetDecodificaAziendaCreditoByIdCodiceAzienda(codiceBancaEsodati.Id, out azCredito);
                }
            }

            List<GestioneAziendeVOCRED_DAP.DecAziendeVOCRED_DAP> listaAziendeVOCRED_DAPAmmesse = null;
            if (Utility.IsDomandaVOCRED_CRED27__DAP(contenitore.DatiPensione))
                listaAziendeVOCRED_DAPAmmesse = contenitoreDecodifica.ElencoDecAziendeVOCRED_DAP;

            List<GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA> listaAziendeScadenzaAssegnoGGmmAAAA = null;
            if (Utility.IsDomandaVESO92_L92(contenitore.DatiPensione) || Utility.IsDomandaVESO92WithGP2BB05(contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.Gruppo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null))
            {
                string siglaCatPensione = contenitore.DatiPensione.SiglaCategoria.Trim();
                if (contenitoreDecodifica.ElencoDecAziendeScadenzaAssegnoGGmmAAAA != null && contenitoreDecodifica.ElencoDecAziendeScadenzaAssegnoGGmmAAAA.Exists(x => x.SiglaCatPensione.Trim() == siglaCatPensione))
                    listaAziendeScadenzaAssegnoGGmmAAAA = contenitoreDecodifica.ElencoDecAziendeScadenzaAssegnoGGmmAAAA;
            }

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto);

            List<Entity.AltraPensione> listaAltraPensione = null;
            GestioneBititolarita.GetDatiAltraPensioneByIdPensione(ref contenitore, out listaAltraPensione);
            //ENG - MEMO 50/2023
            GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneMemo50_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo50_2023", out ctrlAbilitazioneMemo50_2023);
            List<EntityBLCommon.DatiSupplementi> listaSupplementi = new List<EntityBLCommon.DatiSupplementi>();
            if (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo == "SI" && Utility.IsRicostituzione_MotiviContributivi(contenitore.DatiPensione) && !Utility.IsDomandaENPALS(contenitore.DatiPensione.SiglaCategoria) &&
                !Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && !(Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, isRiaperturaDomanda) != null))
                listaSupplementi = contenitore.ListaDatiSupplementiNoStorico;
            else
                listaSupplementi = contenitore.ListaDatiSupplementi;
            #endregion GetData

            #region Controlli Preliminari
            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
            {
                if (!GestioneControlli.ControlsCapienzaAreaHostENPALS(contenitore.DatiPensione, listaSuppRecordEnpals, contenitore.DatiCalcoloContributivoENPALS,
                    listaSupplementiEnpals, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Supplementi:<br />" + messaggioVideo;
                    return false;
                }
            }

            if (!contenitore.DatiPensione.FlagVerify.GetValueOrDefault())
            {
                if (Utility.IsPrepensionamentoEditoriaFiltroEAA(contenitore.DatiPensione))
                {
                    if (!GestioneControlli.ControlsDomandaPrepensionamentoEditoriaCalcolabile(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda,
                        contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceAziendaEditoria : null, out messaggioVideo))
                        return false;
                }
                else if (Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(contenitore.DatiPensione))
                {
                    if (!GestioneControlli.ControlsDomandaPrepensionamentoEditoriaPerTipo0171Calcolabile(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda,
                            contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceAziendaEditoriaPerTipo0171 : null, out messaggioVideo))
                        return false;
                }
                else if (Utility.IsPrepensionamentoEditoriaFiltroEBA(contenitore.DatiPensione)) //ENG - Prepensionamento Editoria Filtro EBA
                {
                    if (!GestioneControlli.ControlsDomandaPrepensionamentoEditoriaFiltroEBACalcolabile(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda,
                           contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceAziendaEditoriaLetteraB : null, out messaggioVideo))
                        return false;
                }
            }

            if (Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(contenitore.DatiPensione))
            {
                if (!GestioneControlli.ControlsDomandaPrepensionamentoEditoriaPerTipo0179Calcolabile(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda,
                    contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceAziendaEditoriaPerTipo0179 : null, out messaggioVideo))
                    return false;
            }


            if (GestioneCtrlControlliApplicativi.CheckControlloApplicativoAttivoByData(GestioneCtrlControlliApplicativi.EnumNomeControllo.AGO.BLOCCOCALCOLO_CUMULO_ESTERNO, dataSistema) &&
                Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.TipoCumulo.HasValue &&
                !contenitore.DatiPensioniDatiGenerici.TipoCumulo.Value)
            {
                messaggioVideo = "Invio al calcolo temporaneamente non disponibile per domande in cumulo esterno.";
                return false;
            }

            if (GestioneCtrlControlliApplicativi.CheckControlloApplicativoAttivoByData(GestioneCtrlControlliApplicativi.EnumNomeControllo.AGO.BLOCCOCALCOLO_TOTALIZZAZIONE_ESTERNO, dataSistema) &&
               Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria) && GestioneControlli.IsCassaEsternaTotalizzazione(contenitore.DatiPensione, contenitore.ListaQuotePensione, contenitoreDecodifica))
            {
                messaggioVideo = "Invio al calcolo temporaneamente non disponibile per domande in totalizzazione cassa esterna.";
                return false;
            }

            if (GestioneCtrlControlliApplicativi.CheckControlloApplicativoAttivoByData(GestioneCtrlControlliApplicativi.EnumNomeControllo.AGO.BLOCCOCALCOLO_ESTERO, dataSistema) &&
                GestioneCrossControls.ALL_VerificaBloccoCalcoloEstero(contenitore.DatiAreaTitolare.Anagrafica.CodiceComuneResidenza, contenitore.DatiPagamento))
            {
                messaggioVideo = "Invio al calcolo temporaneamente non disponibile per domande con titolare residente all'estero e/o avente modalità di pagamento estera.";
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaBloccoCalcoloAnticipata2019(contenitore.DatiPensione, contenitore.TipoAppartenenza, dataSistema, out messaggioVideo))
                return false;

            //RINNOVO RIC/TRF
            GestioneControlliDinamici.ControlloDinamico controlloDinamicoInterregno = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataInizioInterregno", out controlloDinamicoInterregno);
            DateTime dataInizioInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataFineInterregno", out controlloDinamicoInterregno);
            DateTime dataFineInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

            // se è una RIC o TRF e ci troviamo in fase di interregno, isRicRinnovata deve essere true se no scatta il controllo
            if ((Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) || contenitore.IsRiaperturaDomanda) && dataSistema != null && dataInizioInterregno != null && dataFineInterregno != null
                && Utility.DataSuccessivaA(dataSistema, dataInizioInterregno) && !Utility.DataStrettamenteSuccessivaA(dataSistema, dataFineInterregno) && !contenitore.DatiPensione.IsRicRinnovata.HasValue)
            {
                messaggioVideo = "Pensione non rinnovata cancellare e riprelevare la domanda.";
                return false;
            }

            #endregion Controlli Preliminari

            #region Anagrafica

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.TipoAppartenenza.AGO;
            DateTime? dataValiditaInferiore = null;

            if (Utility.IsPrepensionamentoEditoriaFiltroEBA(contenitore.DatiPensione))
            {
                DateTime? dataCompare = null;
                GestioneControlliDinamici.ControlloDinamico ctrlDataCalcoloFiltroEBA = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataCalcoloPoligraficiEBA", out ctrlDataCalcoloFiltroEBA);
                if (ctrlDataCalcoloFiltroEBA != null)
                    dataCompare = Utility.DataFromString(ctrlDataCalcoloFiltroEBA.ValoreControllo, Utility.FormatoData.AAAAmmGG);

                if (dataCompare.HasValue && Utility.DataStrettamenteSuccessivaA(contenitore.DatiPensione.DataPresentazioneDomanda, dataCompare.Value))
                {
                    messaggioVideo = "Domanda momentaneamente non definibile, in attesa della verifica della capienza del fondo contrattuale con finalità sociali istituito con accordo contrattuale FNSI-FIEG";
                    return false;
                }
            }

            if (!GestioneCrossControls.ALL_VerificaBloccoDecorrenzaPensione(contenitore.DatiPensione, isRiaperturaDomanda, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Titolare:<br />" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_ControlsDecorrenzaOriginariaWithDataMorteTitolare(contenitore.DatiPensione.DecorrenzaOriginaria, contenitore.DatiAreaTitolare.Anagrafica.DataMorte,
                out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Titolare:<br />" + messaggioVideo;
                return false;
            }

            if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(contenitore.DatiPensione))
            {
                //Pensioni ai superstiti o sue ricostituzioni
                if (!GestioneCrossControls.ALL_ControlsDecorrenzaOriginariaPerIndirette(contenitore.DatiPensione.DecorrenzaOriginaria, contenitore.DatiAreaTitolare.Anagrafica.CodiceFiscale,
                    contenitore.DatiAreaTitolare.Anagrafica.DataNascita, contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.DataMorte : null, listaFamiliari, contenitore.DatiPensione, contenitore.DatiDanteCausa, contenitore.ListaCodMaggFamiliari, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Titolare:<br />" + messaggioVideo;
                    return false;
                }
            }

            if (!GestioneCrossControls.ALL_VerificaResidenzaEsteroTitolare(contenitore.DatiAreaTitolare.Anagrafica.ResidenzaEstero, contenitore.DatiAreaTitolare.Anagrafica.CodiceComuneResidenza,
                contenitore.DatiAreaTitolare.Anagrafica.FrazioneResidenza, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaProvinciaTitolare(contenitore.DatiAreaTitolare.Anagrafica.ProvinciaResidenza, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            bool? isDecorrenzaValida = Utility.ControllaDataDecorrenzaInferiore(contenitore.DatiPensione, Utility.IsDomandaReversibilitaOrRicostituzione(contenitore.DatiPensione, contenitore.DatiDanteCausa),
                contenitore.DatiPensione.DecorrenzaOriginaria, out dataValiditaInferiore);
            if ((!isDecorrenzaValida.HasValue || !isDecorrenzaValida.Value) && !(Utility.IsDomandaVOST(contenitore.DatiPensione.SiglaCategoria) || Utility.IsRenditaFacoltativa(contenitore.DatiPensione) || Utility.IsRenditaCasalinghe(contenitore.DatiPensione) || Utility.IsDomandaSPED(contenitore.DatiPensione) || Utility.IsDomandaBancari(contenitore.DatiPensione.SiglaCategoria)))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>Decorrenza Pensione antecedente il " +
                    (dataValiditaInferiore.HasValue ? dataValiditaInferiore.Value.Month.ToString() + "/" + dataValiditaInferiore.Value.Year.ToString() : "limite minimo");
                return false;
            }

            #region Controlli Salvaguardie
            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia214(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia135(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensionePerfReqSalvaguardia122(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria,
                contenitore.DatiPensione.DataPerfezionamentoRequisiti, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia228(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia124(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia124Art11Bis(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia147(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneUsuranti(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneEsuberiPA(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia147_2014(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia208_2015(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia178_2020(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }
            #endregion Controlli Salvaguardie

            if (!Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
            {
                if (!GestioneCrossControls.AGO_VerificaDecorrenzaPensione(contenitore.DatiPensione.DecorrenzaOriginaria, contenitore.DatiPensione.DataPerfezionamentoRequisiti, contenitore.DatiPensione,
                    contenitore.DatiAreaTitolare.Anagrafica, contenitore.DatiDanteCausa, contenitore.DatiLavorazione, contenitore.DatiEliminazione, contenitore.DatiPensioniDatiGenerici, dataSistema, isRiaperturaDomanda, contenitore.DatiPensione.DataCondizioniPerComputo, contenitore.DatiPensione, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Titolare:<br />" + messaggioVideo;
                    return false;
                }

                if (!GestioneCrossControls.AGO_VerificaPerfRequisiti(contenitore.DatiPensione.DataPerfezionamentoRequisiti, contenitore.DatiPensione.DecorrenzaOriginaria,
                    contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno : null, contenitore.DatiPensione, contenitore.DatiIstruttoria, contenitore.DatiPensione.DataCondizioniPerComputo, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Titolare:<br />" + messaggioVideo;
                    return false;
                }

                if (!GestioneCrossControls.AGO_VerificaCodiceSindacato(contenitore.DatiAreaTitolare.Sindacato, contenitore.DatiPensione.DecorrenzaOriginaria, contenitore.DatiPensione, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Titolare:<br />" + messaggioVideo;
                    return false;
                }

                if (contenitore.DatiPensioniDatiGenerici != null)
                {
                    if (!GestioneCrossControls.AGO_FS_VerificaDipendenzaPerfezRequisitiRiduzioneRetributiva(contenitore.DatiPensione, contenitore.DatiPensioniDatiGenerici.RiduzioneRetributiva,
                        contenitore.TipoCalcolo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Istruttoria / Dati Titolare:<br/>La riduzione retributiva è incompatibile con la data perfezionamento requisiti.";
                        return false;
                    }
                }
            }

            if (contenitore.DatiAreaTitolare.Sindacato != null && Utility.IsSindacatoPresente(contenitore.DatiAreaTitolare.Sindacato.CodiceSindacato) &&
                !GestioneControlli.VerificaSindacatoAttivo(contenitore.DatiAreaTitolare.Sindacato, contenitore.DatiPensione.SiglaCategoria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_ControlsDecorrenzaOriginariaSperimentaleDonna(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_ControlsPerfezionamentoRequisitiSperimentaleDonna(contenitore.DatiPensione, contenitore.DatiAreaTitolare.Anagrafica,
                contenitore.DatiPensione.DataPerfezionamentoRequisiti, tipoAppartenenza, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_ControlsDecorrenzaOriginariaOpzioneDonna_Legge197_2022_Art1_Comma292(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, contenitore.DatiPensione.DataPerfezionamentoRequisiti, tipoAppartenenza, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            //ENG - memo 13 - opzionedonna2023 ddlFigli valorizzabile da view
            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaOpzioneDonna_Legge197_2022_Art1_Comma292(contenitore.DatiPensione, contenitore.DatiPensione.DataPerfezionamentoRequisiti, contenitore.DatiPensione.NumeroFigli, contenitore.DatiAreaTitolare.Anagrafica, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria))
            {
                if (tipoDomanda != Utility.TipoDomanda.Ricostituzione && tipoDomanda != Utility.TipoDomanda.Ripristino && tipoDomanda != Utility.TipoDomanda.RipristinoSuperstiti &&
                        !contenitore.IsRiaperturaDomanda)
                {
                    if (!GestioneControlli.VerificaDecorrenzaOriginariaInabilitaCumulo(contenitore.DatiPensione,
                        contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.EnteIstruttoreExInpdap : null, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                        return false;
                    }
                }
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensionePerfRequisitiSperimentaleDonna(contenitore.DatiPensione, tipoAppartenenza, contenitore.DatiPensione.DecorrenzaOriginaria,
                contenitore.DatiPensione.DataPerfezionamentoRequisiti, contenitore.DatiAreaTitolare.Anagrafica.DataNascita, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!Utility.IsDomandaVOPGI_AGI(contenitore.DatiPensione))
            {
                if (!GestioneCrossControls.ALL_VerificaDecorPensione(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, contenitore.DatiPensione.DataPerfezionamentoRequisiti,
                   out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                    return false;
                }
            }

            if (!GestioneCrossControls.AGO_VerificaDecorrenzaPensioneSuperioreVESO33(contenitore.DatiPensione.DecorrenzaOriginaria, azVESO33, codiceBancaEsodati, contenitore.DatiPensione.SiglaCategoria,
                out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.AGO_VerificaDecorrenzaPensioneSuperioreVOCRED_CRED27(contenitore.DatiPensione.DecorrenzaOriginaria, azCredito, codiceBancaEsodati,
                contenitore.DatiPensione.SiglaCategoria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.AGO_VerificaDecorrenzaPensioneSuperioreVOCOOP_COOP28(contenitore.DatiPensione.DecorrenzaOriginaria, azCredito, codiceBancaEsodati,
                contenitore.DatiPensione.SiglaCategoria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.VerificaDecorrenzaOriginariaVESO92(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria,
                codiceBancaEsodati != null ? codiceBancaEsodati.TraduzioneSuGP : null, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.AnnoBancaFideiussoria : null,
                contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.ProgressivoBancaFideiussoria : null,
                contenitore.DatiAreaTitolare.Anagrafica != null ? contenitore.DatiAreaTitolare.Anagrafica.Cognome : null,
                contenitore.DatiAreaTitolare.Anagrafica != null ? contenitore.DatiAreaTitolare.Anagrafica.CodiceFiscale : null, listaDecBancaFideiussione, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaUnioniCiviliSuperstiti(contenitore.DatiPensione, listaFamiliari, contenitore.DatiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensioneAPEPrecoce(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, contenitore.DatiAreaTitolare.Anagrafica,
                out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.AGO_ControlsCompartoScuola(contenitore.DatiPensione, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.EnteCassa : null,
                contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.EnteIstruttoreExInpdap : null, lQuotePensione, derogaTraduzioneSuGP, contenitore.IsRiaperturaDomanda,
                out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_VerificaDecorrenzaPensioneInabilitaAmianto(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, out messaggioVideo))
                return false;

            //ENG - Vecchiaia in Computo
            if (!GestioneCrossControls.VerificaPerfezionamentoRequisitiVecchiaiaInComputo(contenitore.DatiPensione, contenitore.DatiPensione, tipoAppartenenza, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_ControlsRequisitoEta(contenitore.DatiPensione, tipoAppartenenza, isRiaperturaDomanda, contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare.DataNascita, contenitore.DatiAnagraficiTitolare.Sesso,
                contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.Legge44997 : null, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceParticolareSoggettoDerogato : null, derogaTraduzioneSuGP, contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, null, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico : null, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }
            bool isWarning = false;
            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaPerTipoContributivo(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare.DataNascita,
                contenitore.DatiAnagraficiTitolare.Sesso, contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, out isWarning, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaSPED(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaINDCOM(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaPerfezionamentoRequisitiQuota100(contenitore.DatiPensione, contenitore.DatiPensione.DataPerfezionamentoRequisiti, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensioneQuota100(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, contenitore.DatiPensione.DataPerfezionamentoRequisiti,
                contenitore.DatiPensione.LavoratorePubblico, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaQuota100(contenitore.DatiPensione, contenitore.DatiPensione.DataPerfezionamentoRequisiti, contenitore.DatiAreaTitolare.Anagrafica.DataNascita,
                out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensionePrecoci(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, contenitore.DatiPensione.DataPerfezionamentoRequisiti,
                out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneControlli.VerificaRequisitoEtaENAV(contenitore.DatiPensione, contenitore.DatiPensione.DataPerfezionamentoRequisiti, contenitore.DatiAnagraficiTitolare != null ? contenitore.DatiAnagraficiTitolare.DataNascita : null, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensioneQuota102(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, contenitore.DatiPensione.DataPerfezionamentoRequisiti,
             contenitore.DatiPensione.LavoratorePubblico, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaQuota102(contenitore.DatiPensione, contenitore.DatiPensione.DataPerfezionamentoRequisiti, contenitore.DatiAreaTitolare.Anagrafica.DataNascita, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.AGO_VerificaRequisitoEtaDomandaVMP(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, contenitore.DatiAreaTitolare.Anagrafica.DataNascita, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaAnticipataFlessibile(contenitore.DatiPensione, contenitore.DatiPensione.DataPerfezionamentoRequisiti, contenitore.DatiAreaTitolare.Anagrafica.DataNascita, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            //ENG - Memo 123/2024
            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaAnticipataFlessibileLeggeDiBilancio2024(contenitore.DatiPensione, contenitore.DatiPensione.DataPerfezionamentoRequisiti, contenitore.DatiAreaTitolare.Anagrafica.DataNascita, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensioneAnticipataFlessibile(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, contenitore.DatiPensione.DataPerfezionamentoRequisiti, contenitore.DatiPensione.LavoratorePubblico, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            //ENG - Memo 123/2024
            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensioneAnticipataFlessibileLeggeDiBilancio2024(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, contenitore.DatiPensione.DataPerfezionamentoRequisiti, contenitore.DatiPensione.LavoratorePubblico, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.AGO_VerificaRequisitoEtaPrepensionamentoEBA(contenitore.DatiPensione, contenitore.DatiPensione.DataPerfezionamentoRequisiti, contenitore.DatiAreaTitolare.Anagrafica.DataNascita, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            #endregion Anagrafica

            #region DanteCausa
            if (Utility.GetTipoDomanda(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto) == Utility.TipoDomanda.Superstiti)
            {
                if (!GestioneCrossControls.AGO_FS_ControlsDataMatrimonioWithGradoParentelaAndDataMorte(contenitore.DatiPensione,
                    contenitore.DatiAnagraficiDanteCausa != null ? contenitore.DatiAnagraficiDanteCausa.DataMatrimonio : null,
                    contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.DataMorte : null, contenitore.DatiAnagraficiDanteCausa != null ? contenitore.DatiAnagraficiDanteCausa.DataNascita : null,
                    listaFamiliari, listaAnagraficheFamiliari, contenitore.DatiAreaTitolare.Anagrafica, Utility.TipoAppartenenza.AGO, isRiaperturaDomanda, contenitore.DatiDanteCausa, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                    return false;
                }

                if (contenitore.ListaDatiRedditoSentenza495_93 != null && contenitore.ListaDatiRedditoSentenza495_93.Count > 0)
                {
                    if (!GestioneCrossControls.AGO_CI_ControlsRedditiSentenza495_93(contenitore.ListaDatiRedditoSentenza495_93, contenitore.DatiPensione.DecorrenzaOriginaria,
                        contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.DataMorte : null, contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.DecorrenzaPensione : null,
                        contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.ProvenienzaPensione : null, contenitore.DatiPensione, listaFamiliari, tipoAppartenenza,
                        contenitore.DatiDanteCausa, isRiaperturaDomanda, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                        return false;
                    }
                }

                if (!GestioneCrossControls.AGO_CI_ControlsDecorrenzaResidenzaDanteCausa(contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.StatoEEResidenza : string.Empty,
                    contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.DecorrenzaResidenza : null, contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.DataMorte : null,
                    contenitore.DatiAnagraficiDanteCausa != null ? contenitore.DatiAnagraficiDanteCausa.DataNascita : null, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                    return false;
                }

                if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria))
                {
                    if (!GestioneCrossControls.AGO_ControlsDataMortePerCumulo(contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.DataMorte : null, out messaggioVideo))
                        return false;
                }
            }

            if (!GestioneCrossControls.ALL_VerificaDataMatrimonioDC(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda,
                contenitore.DatiAnagraficiDanteCausa != null ? contenitore.DatiAnagraficiDanteCausa.DataMatrimonio : null, listaFamiliari, tipoAppartenenza, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                return false;
            }

            if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(contenitore.DatiPensione) &&
                tipoAppartenenza.HasValue && (tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO || tipoAppartenenza.Value == Utility.TipoAppartenenza.CI) &&
                !(Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, isRiaperturaDomanda) && (Utility.IsDomandaSOSPED(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaPMO(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaPSO(contenitore.DatiPensione.SiglaCategoria))))
            {
                //TODO rimuovere per ante96
                if (!GestioneCrossControls.AGO_CI_ControlsProvenienzaPensione(contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.ProvenienzaPensione : null, out messaggioVideo))
                    return false;
            }

            #endregion DanteCausa

            #region Stati Civili
            if (!GestioneCrossControls.ALL_VerificaStatiCivili(contenitore.DatiPensione.DecorrenzaOriginaria, contenitore.DatiPensione, contenitore.DatiAreaTitolare.ElencoStatiCivili,
                contenitore.ListaFamiliari, contenitore.ListaAnagraficaFamiliari, contenitore.ListaCodMaggFamiliari, contenitore.DatiAreaTitolare.Anagrafica.DataNascita,
                contenitore.DatiAnagraficiDanteCausa != null ? contenitore.DatiAnagraficiDanteCausa.DataMatrimonio : null, contenitore.DatiAreaTitolare.Anagrafica.Sesso,
                contenitore.DatiAnagraficiDanteCausa != null ? contenitore.DatiAnagraficiDanteCausa.Sesso : null, contenitore.DatiAreaTitolare.Anagrafica.CodiceFiscale, dataSistema, out messaggioVideo))
                return false;

            //controlli unioni civili
            if (!GestioneCrossControls.ALL_VerificaDecorrenzaUnioniCivili(contenitore.DatiAreaTitolare.ElencoStatiCivili, contenitore.DatiPensione, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_VerificaStatiCiviliConDataMorteFamiliari(contenitore.DatiAreaTitolare.ElencoStatiCivili, contenitore.ListaFamiliari, contenitore.ListaCodMaggFamiliari,
                out messaggioVideo))
                return false;
            #endregion Stati Civili

            #region Residenze Estere
            if (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) &&
                !GestioneCrossControls.ALL_VerificaResidenzeEstereWithAnagrafica(contenitore.DatiAreaTitolare.Anagrafica, contenitore.DatiAreaTitolare.ElencoResidenzeEstere, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }
            #endregion Residenze Estere

            #region LiquidazionePensione
            if (GestioneLiquidazionePensione.IsDatiIstruttoriaPresenti(ref contenitore))
            {
                if (contenitore.DatiPensione.TipoCalcolo.HasValue)
                {
                    /////// IMPORTANTE: Per ENPALS bypassiamo il controllo perchè manca ancora l'analisi della tab istruttoria
                    if (!Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
                        if (!GestioneCrossControls.AGO_ControlsRiduzioneRetributiva(contenitore.DatiPensione.TipoCalcolo,
                            contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.RiduzioneRetributiva : false,
                            contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.RiduzioneRetributivaPercentuale : null,
                            contenitore.IsRiaperturaDomanda, contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null, codiceBancaEsodati != null ? codiceBancaEsodati.TraduzioneSuGP : null, out messaggioVideo))
                            return false;
                }
            }

            if (!GestioneControlli.ControlsCodiciNatura(contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, contenitore.DatiPensione.NaturaPensione, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceDomandaRicorso : null,
                contenitore.DatiPensione.AttivitaEconomica, contenitore.DatiPensione.ProfessioneIndividuale, contenitore.DatiDanteCausa, contenitore.DatiPensione.TipoCalcolo, contenitore.DatiPensione.Contributivo, contenitore.DatiPensione, contenitore.DatiLavorazione,
                contenitore.DatiPensioniDatiGenerici, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NSettimaneOBG : null, contenitore.IsRiaperturaDomanda, contenitore.ListaDatiSupplementi,
                contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.TipoPensioneExInpdai : null, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCodiceArretrati(contenitore.DatiPensione.CodiceArretrati, contenitore.DatiEliminazione != null ? contenitore.DatiEliminazione.CodiceMotivo : null,
                contenitore.DatiPensione, contenitore.IsRiaperturaDomanda, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.TipoCumulo : null,
                contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.CumuloEsterno : null, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Generici:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneControlli.ControlsDecorrenzaArretrati(contenitore.DatiPensione.DecorrenzaCalcoloArretrati, contenitore.DatiPensione.DataInizioCalcolo, contenitore.DatiPensione,
                contenitore.DatiPensione.CausaCarico, annoCompetenza, contenitore.IsRiaperturaDomanda, contenitore.DatiPensione.CodiceBancaEsodati, contenitore.DatiDanteCausa, out messaggioVideo))
                return false;

            if (!Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria))
            {
                if (!GestioneControlli.ControlsDataCompletezza(contenitore.DatiPensione.DataCompletezza, contenitore.DatiPensione, dataSistema, contenitore.IsRiaperturaDomanda, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsDataInteressiLegali(contenitore.DatiPensione.DataInteressiLegali, contenitore.DatiPensione.DataCompletezza,
                    contenitore.DatiPensione.CausaCarico, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceDomandaRicorso : null, contenitore.DatiPensione.DataInizioCalcolo,
                    contenitore.DatiPensione, contenitore.IsRiaperturaDomanda, out messaggioVideo))
                    return false;
            }

            if (!GestioneControlli.ControlsCausaCarico(contenitore.DatiPensione, contenitore.DatiPensione.CausaCarico, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceDomandaRicorso : null,
                contenitore.DatiPensione.DataInizioCalcolo, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsDataRipristino(contenitore.DatiPensione.DataInizioCalcolo, contenitore.DatiPensione.CausaCarico, contenitore.DatiPensione, dataSistema, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCodiceLiquidazione(contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceLiquidazione : null,
                contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.DecorrenzaPensione : (DateTime?)null, contenitore.DatiPensione, contenitore.DatiDanteCausa, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCodiceMobilita(contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceMobilita : null, contenitore.DatiPensione.NaturaPensione,
                contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.Legge44997 : null, contenitore.DatiPensione, out messaggioVideo))
                return false;

            if (!Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaVESO92_L92(contenitore.DatiPensione) && !Utility.IsDomandaVOCRED_CRED27__DAP(contenitore.DatiPensione) &&
                !Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria) && !(Utility.IsDomandaVESO29(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.GetFiltro() == "FS") &&
                !(Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.GetFiltro() == "FS") &&
                !Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaESOAMB_L26(contenitore.DatiPensione) && !Utility.IsDomandaSPED(contenitore.DatiPensione) && !Utility.IsDomandaINDCOM(contenitore.DatiPensione.SiglaCategoria)
                && !Utility.IsIsoPensioneRicWithGP2BB05(contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.Gruppo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null)
                && !Utility.IsDomandaINDCOM(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria)
                && !(Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.GetFiltro() == "ESA" && Utility.IsAssegnoStraordinarioRiscossioneTributiErariali(contenitore.DatiPensione)) && !Utility.IsDomandaESPA_L26(contenitore.DatiPensione)
                && !Utility.IsRenditaCasalinghe(contenitore.DatiPensione) && !Utility.IsRenditaFacoltativa(contenitore.DatiPensione) && !Utility.IsDomandaVOST(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaPSO(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaVESO33_DAP(contenitore.DatiPensione) &&
                (!(Utility.IsDomandaBancari(contenitore.DatiPensione.SiglaCategoria) && Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo))))
                if (!GestioneControlli.ControlsTipoCalcolo(contenitore.DatiPensione, contenitore.DatiStoricoGP, contenitore.DatiDanteCausa, contenitore.DatiPensione.TipoCalcolo, isRiaperturaDomanda, out messaggioVideo))
                    return false;

            if (!GestioneControlli.ControlsScadenzaRevSan(contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.ScadenzaRevisioneSanitaria : null, contenitore.DatiPensione.InizioAssicurazione,
                contenitore.DatiEliminazione != null ? contenitore.DatiEliminazione.DecorrenzaEliminazione : null, contenitore.DatiPensione,
                contenitore.DatiAreaTitolare.Anagrafica != null ? contenitore.DatiAreaTitolare.Anagrafica.Sesso : null,
                contenitore.DatiAreaTitolare.Anagrafica != null ? contenitore.DatiAreaTitolare.Anagrafica.DataNascita : null, dataSistema, contenitore.IsRiaperturaDomanda,
                contenitoreDecodifica.ElencoCtrlScadenzaIndennizzoINDCOM, contenitore.DatiEliminazione, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsConfermaInvalidita(contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NRiconoscimentiInvalidita : null, contenitore.DatiPensione.NaturaPensione,
                contenitore.DatiPensione, dataSistema, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsTrattenutaINPDAP(contenitore.DatiPagamento != null ? contenitore.DatiPagamento.TrattenutaInpdap : null,
                contenitore.DatiPagamento != null ? contenitore.DatiPagamento.DataRinunciaTrattenutaInpdap : null,
                contenitore.DatiEliminazione != null ? contenitore.DatiEliminazione.DecorrenzaEliminazione : null, contenitore.DatiPensione,
                contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.DataRinunciaTrattenutaInpdap : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsProvvisoriaWithDataMorteTitolare(contenitore.DatiPensione, contenitore.DatiNuoveLiquidate != null ? contenitore.DatiNuoveLiquidate.FlagProvvisoria : null,
                contenitore.DatiAreaTitolare.Anagrafica.DataMorte, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsFlagProvvisoria(contenitore.DatiNuoveLiquidate != null ? contenitore.DatiNuoveLiquidate.FlagProvvisoria : null, contenitore.DatiPensione, annoCompetenza,
                contenitore.IsRiaperturaDomanda, contenitore.DatiControlloFelpe != null ? contenitore.DatiControlloFelpe.InizioBonus : null, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaIncongruenzaEsenzioneFiscaleToDB(contenitore.DatiPensione, contenitore.DatiAreaTitolare.Anagrafica.CodiceComuneResidenza, contenitore.DatiDetrazioni,
                contenitore.IsRiaperturaDomanda, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceComunicazioneCampo4 : null, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_CI_ControlsEsenzioneFiscaleDoppiaImposizione(contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare != null ? contenitore.DatiAnagraficiTitolare.CodiceComuneResidenza : string.Empty,
                isRiaperturaDomanda, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceComunicazioneCampo4 : null, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_CI_ControlsTipoBeneficioWithCodNatura(contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio : string.Empty,
                contenitore.DatiPensione.NaturaPensione, true, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_CI_ControlsConfermaInvalidita(contenitore.DatiPensione, contenitore.DatiEliminazione != null ? contenitore.DatiEliminazione.DataEvento : null,
                contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NRiconoscimentiInvalidita : null, dataSistema, contenitore.IsRiaperturaDomanda, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Generici:<br/>" + messaggioVideo;
                return false;
            }
            if (!GestioneCrossControls.AGO_CI_ControlsEliminazioneConfermaInvalidita(contenitore.DatiPensione, contenitore.DatiEliminazione != null ? contenitore.DatiEliminazione.DataEvento : null,
                contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NRiconoscimentiInvalidita : null, dataSistema, isRiaperturaDomanda, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Generici:<br/>" + messaggioVideo;
                return false;
            }

            if (contenitore.DatiEliminazione != null && !GestioneControlli.ControlsDecorArretratiWithDecorEliminazione(contenitore.DatiPensione.DecorrenzaCalcoloArretrati,
                contenitore.DatiEliminazione.DecorrenzaEliminazione, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Generici:<br/>" + messaggioVideo;
                return false;
            }

            #region Cumulo L.228/2012

            if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaSOCUM(contenitore.DatiPensione.SiglaCategoria))
            {
                if (!GestioneControlli.ControlsDatiGenericiObbligatoriPerCumulo(contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.EnteCassa : null,
                    contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.EnteIstruttoreExInpdap : null, out messaggioVideo))
                    return false;
            }

            if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria))
            {
                if (!GestioneControlli.ControlsDecorrenzaPensioneEnteIstruttorePerCumulo(contenitore.DatiPensione,
                    contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.EnteIstruttoreExInpdap : null,
                    contenitore.IsRiaperturaDomanda, out messaggioVideo))
                    return false;
            }

            #endregion Cumulo L.228/2012

            #region VESO
            if (!GestioneCrossControls.AGO_VerificaPerfezionamentoRequisiti_Decorrenza_ScadenzaAssegno(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria,
                contenitore.DatiPensione.DataPerfezionamentoRequisiti, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno : null, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Generici:<br/>" + messaggioVideo;
                return false;
            }
            #endregion VESO
            if (!((Utility.IsDomandaBeneficioTerrorismoOver80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo) || Utility.IsDomandaBeneficioTerrorismoUnder80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo))
               && Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo)))
            {
                if (!GestioneControlli.VerificaCoerenzaTipoCalcoloConTerrorismo(contenitore.DatiPensione.TipoCalcolo, contenitore.ListaDatiCalcoloVittimeTerrorismo, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Generici:<br/>" + messaggioVideo;
                    return false;
                }
            }

            if (!GestioneControlli.ControlsNaturaPensionePerIDAI(contenitore.DatiPensione, contenitore.ListaDatiRetributivi, contenitore.ListaDatiContributivi, contenitore.DatiPensione.NaturaPensione,
                out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Generici:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(contenitore.DatiPensione, GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_AGO.BYPASS_ASS_INDCOM))
            {
                if (!Utility.IsDomandaBancRicAnte1991(contenitore.DatiPensione, contenitore.DatiDanteCausa) && !Utility.IsDomandaRicOrTrf_PSO_PMO_DAIAnte2003(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico : null)
                    && Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, isRiaperturaDomanda) == null)
                {
                    if (!GestioneControlli.ControlsInizioFineAssicurazione(contenitore.DatiPensione.InizioAssicurazione, contenitore.DatiPensione.FineAssicurazione, contenitore.DatiPensione.NaturaPensione,
                    contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.ProvenienzaPensione : (byte?)null,
                    contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.DecorrenzaPensione : (DateTime?)null,
                    contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceLiquidazione : (char?)null, contenitore.DatiPensione, contenitore.DatiPensioniDatiGenerici,
                    lQuotePensione, contenitore.TipoCalcolo, contenitore.IsRiaperturaDomanda, true, contenitore.DatiControlloFelpe != null ? contenitore.DatiControlloFelpe.InizioBonus : null,
                    contenitore.ListaDatiContributivi, ref contenitoreDecodifica, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.FacoltaComputo : null,
                    contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NSettimaneOBG : null, contenitore.DatiAreaTitolare.Anagrafica.DataNascita, out messaggioVideo) &&
                    !(Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && Utility.IsDomandaBancari(contenitore.DatiPensione.SiglaCategoria)))
                        return false;
                }

                if (!Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) && !Utility.IsDomandaRicOrTrf_PSO_PMO_DAIAnte2003(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico : null))
                {


                    if (!GestioneControlli.ControlsNSettimaneOBG(contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NSettimaneOBG.GetValueOrDefault() : 0,
                        contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NContributiVolontari.GetValueOrDefault() : 0,
                        contenitore.DatiPensione.NaturaPensione, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceLiquidazione : (char?)null, contenitore.DatiPensione.TipoCalcolo,
                        contenitore.DatiPensione.InizioAssicurazione, contenitore.DatiPensione, contenitore.DatiDanteCausa, contenitore.DatiAreaTitolare.Anagrafica, contenitore.DatiBeneficioVittimeTerrorismo,
                        contenitore.DatiPensioniDatiGenerici, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceAziendaEditoria : null, contenitore.ListaDatiBeneficiParticolari,
                        contenitoreDecodifica.ElencoDecodAnagraficaAccordi, contenitoreDecodifica != null ? contenitoreDecodifica.ElencoDecodificaEnteCassaProfessionale : null, contenitore.DatiPensione != null ? contenitore.DatiPensione.Contributivo : null,
                        isRiaperturaDomanda, contenitore.DatiMaggiorazioniBenefici, out messaggioVideo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null, codiceBancaEsodati != null ? codiceBancaEsodati.TraduzioneSuGP : null,
                        contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NSettimaneOI.GetValueOrDefault() : 0))
                        return false;

                    if (!(Utility.IsDomandaBancari(contenitore.DatiPensione.SiglaCategoria) && Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo)) && Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, isRiaperturaDomanda) == null)
                    {
                        //20141128 Per ENPALS non )sono inseribili i campi 'AttivitàEconomica' e 'ProfessioneIndividuale'
                        if (!GestioneControlli.ControlsAttivitaEconomica(contenitore.DatiPensione.AttivitaEconomica, contenitore.DatiPensione.NaturaPensione,
                        contenitore.DatiPensione.ProfessioneIndividuale, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceLiquidazione : (char?)null, contenitore.DatiPensione,
                        isRiaperturaDomanda, contenitore.DatiDanteCausa, out messaggioVideo))
                            return false;
                    }

                    var idEnteCassa = contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.EnteCassa.HasValue ? contenitore.DatiPensioniDatiGenerici.EnteCassa.Value : 0;
                    var enteCassa = contenitoreDecodifica.ElencoDecodificaEnteCassaProfessionale != null ? contenitoreDecodifica.ElencoDecodificaEnteCassaProfessionale.Where(x => x.Id == idEnteCassa).Select(x => x.TraduzioneSuGP).FirstOrDefault() : null;
                    enteCassa = !string.IsNullOrEmpty(enteCassa) ? enteCassa.ToString().PadLeft(4, '0') : enteCassa;
                    if (!GestioneControlli.ControlsProfessioneIndividuale(contenitore.DatiPensione.ProfessioneIndividuale, contenitore.DatiPensione.AttivitaEconomica, contenitore.DatiPensione.NaturaPensione,
                        contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceLiquidazione : (char?)null, contenitore.DatiPensione, contenitoreDecodifica != null ? contenitoreDecodifica.ElencoCtrlEnteCassaCodiceGestione : null, enteCassa, isRiaperturaDomanda, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.ControlsNContributiVolontari(contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NContributiVolontari.GetValueOrDefault() : 0,
                        contenitore.DatiPensione.NaturaPensione, contenitore.DatiPensione, isRiaperturaDomanda, codiceBancaEsodati != null ? codiceBancaEsodati.TraduzioneSuGP : null, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null,
                        contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NContributiVVAnzianita.GetValueOrDefault() : 0, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.ControlsNContributiVVAnzianita(contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NContributiVVAnzianita.GetValueOrDefault() : 0,
                        contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NContributiVolontari.GetValueOrDefault() : 0,
                        contenitore.DatiPensione.NaturaPensione, contenitore.DatiPensione, isRiaperturaDomanda, codiceBancaEsodati != null ? codiceBancaEsodati.TraduzioneSuGP : null, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.ControlsInizioFineUltimoLavoro(contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.InizioUltimoLavoro : null,
                        contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.FineUltimoLavoro : null, contenitore.DatiPensione.InizioAssicurazione,
                        contenitore.DatiPensione.FineAssicurazione, contenitore.DatiPensione, out messaggioVideo))
                        return false;


                    if (!GestioneCrossControls.AGO_ControlsScadenzaAssegno(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria,
                        contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno : null,
                        codiceBancaEsodati != null ? codiceBancaEsodati.TraduzioneSuGP : null,
                        contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.AnnoBancaFideiussoria : null,
                        contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.ProgressivoBancaFideiussoria : null,
                        contenitore.DatiAreaTitolare.Anagrafica.CodiceFiscale, isRiaperturaDomanda, contenitore.DatiAnagraficiTitolare.DataNascita, derogaTraduzioneSuGP,
                        contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.ScadenzaAssegno : null, contenitore.DatiEliminazione != null ? contenitore.DatiEliminazione.CodiceMotivo : null,
                        contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.IsScadenzaAssegnoConGiorno : null, contenitore.DatiQuadroLiquidazionePensione, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Generici:<br/>" + messaggioVideo;
                        return false;
                    }
                }

                if (!GestioneCrossControls.ALL_ControlsInizioAssicurazioneSperimentaleDonna(contenitore.DatiPensione, contenitore.DatiPensione.InizioAssicurazione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsBeneficioWithAttEconomicaProfessioneInd(contenitore.DatiPensione, contenitore.DatiPensione.AttivitaEconomica,
                    contenitore.DatiPensione.ProfessioneIndividuale, contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio : string.Empty,
                    contenitore.IsRiaperturaDomanda, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsRequisiti(contenitore.DatiPensione.RequisitiVecchiaiaAl1294, contenitore.DatiPensione.RequisitiAl1294, contenitore.DatiPensione.RequisitiAl996,
                    contenitore.DatiPensione.NaturaPensione, contenitore.DatiPensione, isRiaperturaDomanda, out messaggioVideo))
                    return false;

                if (!GestioneCrossControls.ALL_VerificaFineAssicurazioneForReversibilita(tipoDomanda, contenitore.DatiPensione.FineAssicurazione, contenitore.DatiPensione.DecorrenzaOriginaria,
                    contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.DecorrenzaPensione : null, tipoAppartenenza, contenitore.DatiPensione.SiglaCategoria, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsInabilitaWithAttivitaEconomicaAndProfessioneIndividuale(contenitore.DatiPensione, contenitore.DatiPensione.AttivitaEconomica,
                    contenitore.DatiPensione.ProfessioneIndividuale, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.EnteCassa : null, out messaggioVideo))
                    return false;

            }
            #region Cumulo L.228/2012

            if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria))
            {
                int? NSettimane = contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NSettimaneOBG : null;
                if (Utility.IsDomandaOrganizzazioniInternazionali(contenitore.DatiPensione))
                    NSettimane += contenitore.DatiIstruttoria.NSettimaneOI;

                if (!GestioneControlli.ControlsDatiAssicurativiPerCumulo(contenitore.DatiPensione, NSettimane,
                    contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NContributiVolontari : null, contenitore.DatiAreaTitolare.Anagrafica.DataNascita, contenitore.DatiMaggiorazioniBenefici, out messaggioVideo))
                {
                    messaggioVideo = "Controlli incrociati - Dati Assicurativi:</br>" + messaggioVideo;
                    return false;
                }

                if (!Utility.IsDomandaConBeneficioAmianto181(contenitore.DatiPensione.AttivitaEconomica, contenitore.DatiPensione.ProfessioneIndividuale))
                {
                    if (!GestioneControlli.ControlsAttivitaEconomicaProfessioneIndividualePerCumulo(contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.EnteCassa : null,
                        contenitore.DatiPensione.AttivitaEconomica, contenitore.DatiPensione.ProfessioneIndividuale, out messaggioVideo, listaDecodificaEnteCassaProfessionale))
                    {
                        messaggioVideo = "Controlli incrociati - Dati Assicurativi:</br>" + messaggioVideo;
                        return false;
                    }
                }
            }

            if (Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria))
            {
                if (!GestioneControlli.ControlsDatiAssicurativiPerVAPE(contenitore.DatiPensione, contenitore.DatiAreaTitolare.Anagrafica, contenitore.DatiPensioniDatiGenerici,
                    contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NSettimaneOBG : null, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NContributiVolontari : null,
                    out messaggioVideo))
                {
                    messaggioVideo = "Controlli incrociati - Dati Assicurativi:</br>" + messaggioVideo;
                    return false;
                }
            }

            if (Utility.IsDomandaAPEPrecoci(contenitore.DatiPensione))
            {
                int numSettimaneTipoContibutivo = 0;

                //GestioneEnpals.DatiEnpals datiEnpals = null;

                if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
                {
                    //GestioneEnpals.GetDatiEnpalsByIdPensione(datiPensione.Id, out datiEnpals);
                    //ENPALS = ANNI * 52 + MESI * 4,333
                    double numSettimaneDaAnni = datiENPALS != null ? datiENPALS.AADiritto.GetValueOrDefault() * 52 : 0;
                    double numSettimaneDaMesi = datiENPALS != null ? datiENPALS.MMDiritto.GetValueOrDefault() * 4.333 : 0;

                    numSettimaneTipoContibutivo = numSettimaneTipoContibutivo + Convert.ToInt32(numSettimaneDaAnni);
                    numSettimaneTipoContibutivo = numSettimaneTipoContibutivo + Convert.ToInt32(numSettimaneDaMesi);
                }
                else
                    numSettimaneTipoContibutivo = numSettimaneTipoContibutivo + contenitore.DatiIstruttoria.NSettimaneOBG.GetValueOrDefault(); //SETTIMANE OBG DIRITTO

                if (!GestioneControlli.ControlsDatiAssicurativiPerAPEPrecoci(contenitore.DatiPensione, contenitore.DatiPensione.InizioAssicurazione, numSettimaneTipoContibutivo,
                    contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NContributiVolontari : null, datiENPALS, out messaggioVideo))
                {
                    messaggioVideo = "Controlli incrociati - Dati Assicurativi:</br>" + messaggioVideo;
                    return false;
                }
            }

            #endregion Cumulo L.228/2012

            if (GestioneLiquidazionePensione.IsDatiProvenienzaPresenti(ref contenitore) && !Utility.IsDomandaRicOrTrf_PSO_PMO_DAIAnte2003(contenitore.DatiPensione, isRiaperturaDomanda, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico : null))
            {
                if (!GestioneControlli.ControlsProvenienza(contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceP18PrecedentePensione : null, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CertificatoPrecedentePensione : null,
                    contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.DecorrenzaOriginariaAltraPensione : null, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.SedePrecedentePensione : null, contenitore.DatiPensione.DataInteressiLegali, contenitore.DatiPensione, contenitore.DatiDanteCausa,
                    contenitore.DatiPensione.AttivitaEconomica, contenitore.DatiPensione.ProfessioneIndividuale, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceDomandaRicorso : null, isRiaperturaDomanda, out messaggioVideo))
                    return false;
            }

            if (GestioneLiquidazionePensione.IsDatiIstruttoriaPresenti(ref contenitore))
            {
                if (!GestioneControlli.ControlsCodiceRequisitiRidotti(contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.Legge44997 : null,
                    contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceMobilita : null, contenitore.DatiPensione.NaturaPensione, contenitore.DatiPensione, contenitore.TipoCalcolo, contenitore.DatiDanteCausa, isRiaperturaDomanda,
                    out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsAliquotaTFResodati(contenitore.DatiPensione.AliquotaTFREsodati, contenitore.DatiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsAttivitaUsuranti(contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.Attivitausuranti : null,
                    contenitore.DatiPensione.NaturaPensione, contenitore.DatiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsAziendaPerEditoria(contenitore.DatiPensione.CodiceBancaEsodati, contenitore.DatiPensione.NaturaPensione, contenitore.DatiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsAziendaPerEsodati(codiceBancaEsodati, contenitore.DatiPensione, listaAziendeVOCRED_DAPAmmesse, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsBanchePerSede(contenitore.DatiPensione.CodiceBancaEsodati, contenitore.DatiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsAziendaPerEditoriaWithCodNatura3(contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.CodiceBancaEsodati, contenitore.DatiPensione.NaturaPensione,
                    out messaggioVideo))
                    return false;

                if (!GestioneCrossControls.ALL_VerificaRiduzioneRetributiva(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.IsRiaperturaDomanda,
                    contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.RiduzioneRetributiva : false,
                    contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.RiduzioneRetributivaPercentuale : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsBancaFideiussione(contenitore.DatiPensione, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.AnnoBancaFideiussoria : null,
                    contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.ProgressivoBancaFideiussoria : null, codiceBancaEsodati, listaAziendeScadenzaAssegnoGGmmAAAA,
                    out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Liquidazione Pensione / Dati Istruttoria:<br/>" + messaggioVideo;
                    return false;
                }

                if (!GestioneCrossControls.AGO_ControlsRiduzioneRetributivaVOCRED(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, contenitore.DatiAreaTitolare.Anagrafica.DataNascita,
                    contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno : null,
                    contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.RiduzioneRetributiva : false,
                    contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.RiduzioneRetributivaPercentuale : null, isRiaperturaDomanda, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Liquidazione Pensione / Dati Istruttoria:<br/>" + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaCoerenzaRiduzioneAssegno_RiduzioneRetributivaVOCRED(contenitore.DatiPensione,
                    contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.RiduzioneAssegno : null,
                    contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.RiduzioneRetributiva : false,
                    contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.RiduzioneRetributivaPercentuale : null, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Liquidazione Pensione / Dati Istruttoria:<br/>" + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.IsRiduzioneAssegnoAmmissibile(ref contenitore, ref contenitoreDecodifica, contenitore.ListaDatiContributivi, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Liquidazione Pensione / Dati Istruttoria:<br/>" + messaggioVideo;
                    return false;
                }
            }

            //ENG - Aggiornamento Memo 68/2022 IOPGI
            if (!Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) && !(Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione))
                && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa))
                if (!GestioneCrossControls.AGO_ControlsTipoCalcoloForDatiContributivi(contenitore.DatiPensione.TipoCalcolo, contenitore.DatiPensione.FineAssicurazione, contenitore.DatiPensione,
                    contenitore.ListaDatiContributivi, contenitore.ListaDatiRetributivi, Utility.IsPensioneInabilitaPost2012(contenitore.DatiPensione), false) 
                    && !Utility.IsDomandaOrganizzazioniInternazionali(contenitore.DatiPensione))
                {
                    var isAnte96 = Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, isRiaperturaDomanda);
                    if (contenitore.ListaDatiRetributivi == null && (isAnte96 == Utility.TipoAnte96.Ante96Miste || isAnte96 == Utility.TipoAnte96.Ante96Retributive))
                        messaggioVideo = "I dati calcolo salvati sono incongruenti: mancano i dati retributivi.";
                    else
                        messaggioVideo = "I dati calcolo salvati sono incongruenti con il 'Tipo Calcolo'.";
                    return false;
                }

            //ENG - Aggiornamento Memo 68/2022 IOPGI
            if (!Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) 
                && !Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) 
                && !Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) 
                && !Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) 
                && !(Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) 
                && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione))
                && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa)
                && Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, isRiaperturaDomanda) == null
                && !Utility.IsDomandaOrganizzazioniInternazionali(contenitore.DatiPensione))
            {
                if (!GestioneControlli.ControlsFineAssicurazioneForDatiContributivi(contenitore.DatiPensione.TipoCalcolo, contenitore.DatiPensione.FineAssicurazione, contenitore.ListaDatiContributivi,
                    contenitore.ListaDatiRetributivi, Utility.IsPensioneInabilitaGenericaPost2012(contenitore.DatiPensione), Utility.IsDomandaUnicarpe(contenitore.DatiPensione, true), null, false))
                {
                    messaggioVideo = "I dati calcolo salvati sono incongruenti con la data 'Fine Assicurazione'.";
                    return false;
                }
            }

            if (!GestioneControlli.ControlsIncrociatiSoloPreCalcolo(contenitore.DatiPensione, contenitore.DatiIstruttoria, contenitore.DatiNuoveLiquidate, contenitore.DatiAreaTitolare.Anagrafica,
                contenitore.DatiDanteCausa, contenitore.DatiMaggiorazioniBenefici, contenitore.TipoCalcolo, dataSistema, annoCompetenza, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaDecorrenzaArretratiConDataAssunzioneCaricoDAI(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda, contenitore.DatiPensione.DecorrenzaCalcoloArretrati,
                contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico : null, out messaggioVideo))
                return false;

            int? NSettimaneOBG = contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NSettimaneOBG.GetValueOrDefault() : 0;
            if (Utility.IsDomandaOrganizzazioniInternazionali(contenitore.DatiPensione))
            {
                if (contenitore.DatiIstruttoria != null)
                    NSettimaneOBG += (contenitore.DatiIstruttoria.NSettimaneOI.HasValue ? contenitore.DatiIstruttoria.NSettimaneOI.Value : 0);
            }

            if (!GestioneControlli.VerificaDataPerfezionamentoPerPensioneTipoContributivo(contenitore.DatiPensione,
                NSettimaneOBG.Value,
                contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NContributiVolontari.GetValueOrDefault() : 0,
                datiENPALS, contenitore.DatiAreaTitolare.Anagrafica, dataSistema, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaDataPerfezionamentoPerTrasfAOI(contenitore.DatiPensione,
                NSettimaneOBG.Value,
                contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NContributiVolontari.GetValueOrDefault() : 0,
                datiENPALS, contenitore.DatiAreaTitolare.Anagrafica, dataSistema,
                contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.FacoltaComputo : null, out messaggioVideo))
                return false;

            if (Utility.IsPrepensionamentoEditoriaFiltroEAA(contenitore.DatiPensione))
            {
                if (!GestioneControlli.ControlsPrepensionamentoEditoriaCodiceAnagraficaAccordi(contenitore.DatiPensione, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceAziendaEditoria : null, ref contenitoreDecodifica, out messaggioVideo))
                    return false;
            }
            else if (Utility.IsPrepensionamentoEditoriaFiltroEBA(contenitore.DatiPensione))
            {
                if (!GestioneControlli.ControlsPrepensionamentoEditoriaLetteraBCodiceAnagraficaAccordi(contenitore.DatiPensione, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceAziendaEditoriaLetteraB : null, ref contenitoreDecodifica, out messaggioVideo))
                    return false;
            }
            else if (Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(contenitore.DatiPensione))
            {
                if (!GestioneControlli.ControlsPrepensionamentoEditoriaPerTipo0171CodiceAnagraficaAccordi(ref contenitore, ref contenitoreDecodifica,
                    contenitore.DatiIstruttoria.CodiceAziendaEditoriaPerTipo0171 != null ? contenitore.DatiIstruttoria.CodiceAziendaEditoriaPerTipo0171 : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsPrepensionamentoEditoriaPerTipo0171OneriAzienda(ref contenitore, ref contenitoreDecodifica,
                    contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceAziendaEditoriaPerTipo0171 : null, out messaggioVideo))
                    return false;
            }
            else if (Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(contenitore.DatiPensione))
            {
                if (!GestioneControlli.ControlsPrepensionamentoEditoriaPerTipo0179CodiceAnagraficaAccordi(ref contenitore, ref contenitoreDecodifica,
                    contenitore.DatiIstruttoria.CodiceAziendaEditoriaPerTipo0179 != null ? contenitore.DatiIstruttoria.CodiceAziendaEditoriaPerTipo0179 : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsPrepensionamentoEditoriaPerTipo0179OneriAzienda(ref contenitore, ref contenitoreDecodifica,
                    contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceAziendaEditoriaPerTipo0179 : null, out messaggioVideo))
                    return false;
            }

            if (!GestioneControlli.ControlsNSettimanePerRequisitoAnticipatoArt1(contenitore.DatiPensione, contenitore.DatiEnpals != null ? contenitore.DatiEnpals.AADiritto : null, contenitore.DatiEnpals != null ? contenitore.DatiEnpals.MMDiritto : null,
                contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NSettimaneOBG : null, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NContributiVolontari : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsNSettimanePerQuota100(contenitore.DatiPensione, contenitore.DatiEnpals != null ? contenitore.DatiEnpals.AADiritto : null,
                contenitore.DatiEnpals != null ? contenitore.DatiEnpals.MMDiritto : null, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NSettimaneOBG : null,
                contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NContributiVolontari : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsNSettimanePerQuota102(contenitore.DatiPensione, contenitore.DatiEnpals != null ? contenitore.DatiEnpals.AADiritto : null,
                contenitore.DatiEnpals != null ? contenitore.DatiEnpals.MMDiritto : null, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NSettimaneOBG : null,
                contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NContributiVolontari : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsNSettimanePerSperimentaleDonna_DL_4_2019(contenitore.DatiPensione, contenitore.DatiEnpals != null ? contenitore.DatiEnpals.AADiritto : null,
                contenitore.DatiEnpals != null ? contenitore.DatiEnpals.MMDiritto : null, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NSettimaneOBG : null,
                contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NContributiVolontari : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsNSettimanePerPerOpzioneDonna_Legge197_2022_Art1_Comma292(contenitore.DatiPensione, contenitore.DatiEnpals != null ? contenitore.DatiEnpals.AADiritto : null,
                contenitore.DatiEnpals != null ? contenitore.DatiEnpals.MMDiritto : null, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NSettimaneOBG : null,
                contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NContributiVolontari : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsNSettimanePerAnzianitaPerLeggeBilancio2019(contenitore.DatiPensione, contenitore.DatiEnpals != null ? contenitore.DatiEnpals.AADiritto : null,
                contenitore.DatiEnpals != null ? contenitore.DatiEnpals.MMDiritto : null, NSettimaneOBG,
                contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NContributiVolontari : null, contenitore.DatiAreaTitolare.Anagrafica.Sesso, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaNaturaPensioneEAssicurazione_PensioneOpzioneContributivo(contenitore.DatiPensione, contenitore.DatiPensione.NaturaPensione, contenitore.DatiPensione.InizioAssicurazione, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaBeneficiPerOpzioneTipoContributivo(contenitore.DatiPensione, contenitore.DatiPensione.Benefici, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsRequisitoEta_VOAUT(contenitore.DatiPensione, contenitore.DatiMaggiorazioniBenefici, contenitore.DatiAnagraficiTitolare.DataNascita, contenitore.DatiAnagraficiTitolare.Sesso,
                contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NSettimaneOBG : null, contenitore.DatiPensione.NaturaPensione, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico : null, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaPerVOMIN(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare != null ? contenitore.DatiAnagraficiTitolare.DataNascita : null,
                        contenitore.DatiAnagraficiTitolare != null ? contenitore.DatiAnagraficiTitolare.Sesso : null, contenitore.DatiPensione.NaturaPensione, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaPerVOMIN(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare != null ? contenitore.DatiAnagraficiTitolare.DataNascita : null,
                        contenitore.DatiAnagraficiTitolare != null ? contenitore.DatiAnagraficiTitolare.Sesso : null, contenitore.DatiPensione.NaturaPensione, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsNSettimanePerAnticipateFlessibili(contenitore.DatiPensione, contenitore.DatiEnpals != null ? contenitore.DatiEnpals.AADiritto : null,
              contenitore.DatiEnpals != null ? contenitore.DatiEnpals.MMDiritto : null, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NSettimaneOBG : null,
              contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NContributiVolontari : null, out messaggioVideo))
                return false;

            //ENG - Memo 48_2023
            //Segnalazione 33631 spostare il controllo dal pannello "Titolare" al pannello "Liquidazione Pensione" dati generici alla selezione "Esenzione Fiscale Residente Estero".
            if (contenitore.DatiIstruttoria != null && contenitore.DatiIstruttoria.CodiceComunicazioneCampo4 != null && contenitore.DatiIstruttoria.CodiceComunicazioneCampo4 == 2)
            {
                if (!GestioneCrossControls.VerificaResidenzaCittadinanzaTitolareBulgaria(contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare != null ? contenitore.DatiAnagraficiTitolare.Cittadinanza : null, contenitore.DatiAnagraficiTitolare != null ? contenitore.DatiAnagraficiTitolare.CodiceComuneResidenza : null, out messaggioVideo))
                    return false;
            }

            #endregion LiquidazionePensione

            #region DatiCalcolo

            if (!GestioneContrib.ControlsDatiCalcoloINPDAIAlCalcolo(ref contenitore, ref contenitoreDecodifica,
                contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.AnzAl95 : null,
                contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.QuotaAl95 : null,
                contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, contenitore.TipoCalcolo, out messaggioVideo))
                return false;

            if (contenitore.ListaDatiRetributivi != null && contenitore.ListaDatiRetributivi.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloRetributivo retr in contenitore.ListaDatiRetributivi)
                {
                    if (!GestioneControlli.ControlsDatiRetributivi(retr, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceLiquidazione : (char?)null, contenitore.DatiPensione,
                        contenitoreDecodifica.ElencoCodeGestioneCalcoloRetributivo, contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.DataMorte : (DateTime?)null, contenitore.TipoCalcolo,
                        contenitore.IsRiaperturaDomanda, contenitore.ListaDatiContributivi, contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo, contenitore.DatiDanteCausa, out messaggioVideo))
                        return false;
                }
                {
                    List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiCalcoloRetributivo = contenitore.ListaDatiRetributivi;
                    if (!GestioneControlli.ControlsDatiRetributiviFinal(ref listaDatiCalcoloRetributivo, ref contenitore, ref contenitoreDecodifica, null, contenitore.ListaDatiContributivi, out messaggioVideo))
                        return false;
                    contenitore.ListaDatiRetributivi = listaDatiCalcoloRetributivo;
                }

                //ENG - Aggiornamento Memo 68/2022 IOPGI
                //ENG - Spacchettate SOPGI
                if ((contenitore.DatiPensione.TipoCalcolo == 2 || contenitore.DatiPensione.TipoCalcolo == 21) && !Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) && !(Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione))
                    && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa))
                {
                    if (!GestioneContrib.ControlsQuotaRetributivaAAlCalcolo(contenitore.ListaDatiRetributivi, contenitore.DatiPensione.InizioAssicurazione, contenitore.ListaDatiContributivi, contenitore.DatiPensione, contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo, out messaggioVideo))
                        return false;

                    if (!GestioneContrib.ControlsPresenzaDatiRetributiviAlCalcolo(contenitore.ListaDatiRetributivi, contenitore.ListaDatiContributivi, contenitore.DatiPensione, contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo, out messaggioVideo))
                        return false;
                }

                //ENG - Aggiornamento Memo 68/2022 IOPGI
                //ENG - Spacchettate SOPGI
                if (contenitore.DatiPensione.TipoCalcolo == 2 && !Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) && !(Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione))
                    && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa))
                {
                    if (Utility.IsDomandaUnicarpe(contenitore.DatiPensione, true) != Utility.TipoUnicarpe.Automatica &&
                            !(Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, Utility.IsRiaperturaDomanda(contenitore.DatiPensione.Id)) != null
                            && (contenitore.DatiPensione.SiglaCategoria.StartsWith("S") || contenitore.DatiPensione.SiglaCategoria.StartsWith("I")) && contenitore.DatiPensione.NaturaPensione != null
                            && (contenitore.DatiPensione.NaturaPensione.Substring(0, 1) == "3" || contenitore.DatiPensione.NaturaPensione.Substring(0, 1) == "4")))
                    {
                        if (!GestioneContrib.ControlsQuotaRetributivaBAlCalcolo(contenitore.ListaDatiRetributivi, contenitore.DatiPensione.FineAssicurazione, contenitore.ListaDatiContributivi, contenitore.DatiPensione, contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo, out messaggioVideo))
                            return false;
                    }
                }
            }

            if (contenitore.ListaDatiContributivi != null && contenitore.ListaDatiContributivi.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloContributivo contr in contenitore.ListaDatiContributivi)
                {
                    if (!GestioneControlli.ControlsDatiContributivi(contr, contenitore.DatiPensione, contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo,
                        contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.ProvenienzaPensione : (byte?)0,
                        contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.SiglaCategoria : string.Empty,
                        contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio : string.Empty, contenitore.TipoCalcolo, contenitore.ListaDatiContributivi, contenitore.DatiDanteCausa, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                        if (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto) &&
                           (Utility.IsDomandaFPLD(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaGestioneAutonomi(contenitore.DatiPensione.SiglaCategoria) ||
                            Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaAUT(contenitore.DatiPensione)))
                            messaggioVideo = messaggioVideo + "Controllare le informazioni memorizzate ed eventualmente variarle con una Ricostituzione contributiva.";
                        return false;
                    }
                }

                List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiCalcoloContributivo = contenitore.ListaDatiContributivi;
                if (!GestioneControlli.ControlsDatiContributiviFinal(ref listaDatiCalcoloContributivo, contenitore.DatiPensione, contenitore.DatiDanteCausa,
                    contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NSettimaneOBG.GetValueOrDefault() : 0,
                    contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.NSettimaneBeneficio : null, contenitore.TipoCalcolo, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                    if (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto) &&
                       (Utility.IsDomandaFPLD(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaGestioneAutonomi(contenitore.DatiPensione.SiglaCategoria) ||
                        Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaAUT(contenitore.DatiPensione)))
                        messaggioVideo = messaggioVideo + "Controllare le informazioni memorizzate ed eventualmente variarle con una Ricostituzione contributiva.";
                    return false;
                }
                contenitore.ListaDatiContributivi = listaDatiCalcoloContributivo;

                //ENG - Aggiornamento Memo 68/2022 IOPGI
                //ENG - Spacchettate SOPGI 
                if (contenitore.DatiPensione.TipoCalcolo == 21 && !Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) && !(Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione))
                    && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa))
                {
                    if (!GestioneContrib.ControlsPresenzaQuotaCAlCalcolo(contenitore.DatiPensione, listaDatiCalcoloContributivo, contenitore.ListaDatiRetributivi, ref contenitoreDecodifica, ref contenitore, out messaggioVideo))
                        return false;
                }
            }

            if (!Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria))
            {
                if (!GestioneControlli.ControlsDatiCalcoloFinal(contenitore.ListaDatiRetributivi, contenitore.ListaDatiContributivi, contenitoreDecodifica.ElencoCodeGestioneCalcoloRetributivo,
                    contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo, contenitore.DatiPensione,
                    contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NSettimaneOBG.GetValueOrDefault() : 0,
                    contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NContributiVolontari.GetValueOrDefault() : 0, contenitore.TipoCalcolo, contenitore.DatiDanteCausa,
                    out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                    if (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto) &&
                       (Utility.IsDomandaFPLD(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaGestioneAutonomi(contenitore.DatiPensione.SiglaCategoria) ||
                        Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaAUT(contenitore.DatiPensione)))
                        messaggioVideo = messaggioVideo + "Controllare le informazioni memorizzate ed eventualmente variarle con una Ricostituzione contributiva.";
                    return false;
                }

                if (!GestioneControlli.ControlsDatiContribRetribFinal(contenitore.ListaDatiContributivi, contenitore.ListaDatiRetributivi, contenitore.DatiPensione,
                    contenitore.ListaDatiCalcoloVittimeTerrorismo, contenitore.DatiBeneficioVittimeTerrorismo, lstCtrlDecorrenza,
                    contenitoreDecodifica.ElencoCodeGestioneCalcoloRetributivo, contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo,
                    contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, contenitore.TipoCalcolo, contenitore.DatiDanteCausa, contenitore.DatiIntegrazioneArt11, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                    if (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto) &&
                       (Utility.IsDomandaFPLD(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaGestioneAutonomi(contenitore.DatiPensione.SiglaCategoria) ||
                        Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaAUT(contenitore.DatiPensione)))
                        messaggioVideo = messaggioVideo + "Controllare le informazioni memorizzate ed eventualmente variarle con una Ricostituzione contributiva.";
                    return false;
                }
            }

            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
            {
                if (!GestioneControlli.ControlsDatiRetributiviENPALS(contenitore.DatiCalcoloRetributivoENPALS, datiENPALS.ImportoPensione, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.ControlsDatiContributiviENPALS(contenitore.DatiCalcoloContributivoENPALS, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                    return false;
                }

                if (contenitore.DatiCalcoloRetributivoENPALS != null)
                {
                    if (!GestioneControlli.ControlsProRataTemporisWithNaturaPensione(contenitore.DatiPensione, contenitore.DatiPensione.NaturaPensione,
                        contenitore.DatiCalcoloRetributivoENPALS.ImportoProRataTemporis, out messaggioVideo))
                        return false;
                }
            }
            if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria))
            {
                if (!GestioneContrib.ControlsDatiCalcoloQuotePensioneAlCalcolo(ref contenitore, ref contenitoreDecodifica, lQuotePensione, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Quote Pensione:<br/>" + messaggioVideo;
                    return false;
                }

                if (!GestioneContrib.ControlsDatiCalcoloTrattenuteQuotePensioneAlCalcolo(ref contenitore, ref contenitoreDecodifica, lTrattenuteQuotePensione, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Trattenute Quote Pensione:<br/>" + messaggioVideo;
                    return false;
                }
            }

            if (!GestioneControlli.ControlsCoerenzaDatiCalcoloVittimeTerrorismo(contenitore.DatiPensione, contenitore.ListaDatiCalcoloVittimeTerrorismo, contenitore.DatiBeneficioVittimeTerrorismo,
                out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Calcolo Vittime:<br/>" + messaggioVideo;
                if (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto) &&
                   (Utility.IsDomandaFPLD(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaGestioneAutonomi(contenitore.DatiPensione.SiglaCategoria) ||
                    Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaAUT(contenitore.DatiPensione)))
                    messaggioVideo = messaggioVideo + "Controllare le informazioni memorizzate ed eventualmente variarle con una Ricostituzione contributiva.";
                return false;
            }

            if (!GestioneControlli.ControlsDecorrenzaBeneficioVittimeTerrorismo(contenitore.DatiPensione, contenitore.ListaDatiCalcoloVittimeTerrorismo, contenitore.DatiBeneficioVittimeTerrorismo,
                contenitore.DatiDanteCausa, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Calcolo Vittime:<br/>" + messaggioVideo;
                if (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto) &&
                   (Utility.IsDomandaFPLD(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaGestioneAutonomi(contenitore.DatiPensione.SiglaCategoria) ||
                    Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaAUT(contenitore.DatiPensione)))
                    messaggioVideo = messaggioVideo + "Controllare le informazioni memorizzate ed eventualmente variarle con una Ricostituzione contributiva.";
                return false;
            }

            if (!GestioneControlli.ControlsBeneficioTerrorismo(contenitore.DatiPensione, contenitore.ListaDatiCalcoloVittimeTerrorismo, contenitore.DatiBeneficioVittimeTerrorismo, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Calcolo Vittime:<br/>" + messaggioVideo;
                if (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto) &&
                   (Utility.IsDomandaFPLD(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaGestioneAutonomi(contenitore.DatiPensione.SiglaCategoria) ||
                    Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaAUT(contenitore.DatiPensione)))
                    messaggioVideo = messaggioVideo + "Controllare le informazioni memorizzate ed eventualmente variarle con una Ricostituzione contributiva.";
                return false;
            }

            if (!GestioneControlli.ControlsDatiCalcoloWithBeneficioTerrorismo(contenitore.DatiPensione, contenitore.ListaDatiCalcoloVittimeTerrorismo, contenitore.ListaDatiRetributivi,
                contenitore.ListaDatiContributivi, contenitoreDecodifica.ElencoCodeGestioneCalcoloRetributivo, contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo,
                contenitore.DatiBeneficioVittimeTerrorismo, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Calcolo Vittime:<br/>" + messaggioVideo;
                if (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto) &&
                   (Utility.IsDomandaFPLD(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaGestioneAutonomi(contenitore.DatiPensione.SiglaCategoria) ||
                    Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaAUT(contenitore.DatiPensione)))
                    messaggioVideo = messaggioVideo + "Controllare le informazioni memorizzate ed eventualmente variarle con una Ricostituzione contributiva.";
                return false;
            }

            if (!Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo))
            {
                if (!GestioneControlli.ControlsDatiCalcoloVittimeTerrorismoWithVisibility(contenitore.DatiPensione, contenitore.ListaDatiContributivi, contenitore.ListaDatiCalcoloVittimeTerrorismo,
                    contenitore.DatiBeneficioVittimeTerrorismo, contenitore.TipoCalcolo, contenitore.DatiBeneficioVittimeTerrorismo != null ? contenitore.DatiBeneficioVittimeTerrorismo.SoggettoBeneficiario : null,
                    contenitore.DatiBeneficioVittimeTerrorismo != null ? contenitore.DatiBeneficioVittimeTerrorismo.TipologiaPrestazione : null,
                    contenitore.DatiBeneficioVittimeTerrorismo != null ? contenitore.DatiBeneficioVittimeTerrorismo.TipologiaBeneficio : null, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Calcolo Vittime:<br/>" + messaggioVideo;
                    if (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto) &&
                       (Utility.IsDomandaFPLD(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaGestioneAutonomi(contenitore.DatiPensione.SiglaCategoria) ||
                        Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaAUT(contenitore.DatiPensione)))
                        messaggioVideo = messaggioVideo + "Controllare le informazioni memorizzate ed eventualmente variarle con una Ricostituzione contributiva.";
                    return false;
                }
            }

            if (!GestioneControlli.ControlsDatiCalcoloVittimeTerrorismoINPDAI(contenitore.DatiPensione, contenitore.ListaDatiCalcoloVittimeTerrorismo, contenitore.ListaDatiRetributivi,
                contenitore.ListaDatiContributivi, contenitore.DatiBeneficioVittimeTerrorismo, contenitoreDecodifica.ElencoCodeGestioneCalcoloRetributivo,
                contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo, contenitoreDecodifica.ElencoCtrlDecorrenzaRetrExINPDAI,
                contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, contenitore.TipoCalcolo, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Calcolo Vittime:<br/>" + messaggioVideo;
                if (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto))
                    messaggioVideo = messaggioVideo + "Controllare le informazioni memorizzate ed eventualmente variarle con una Ricostituzione contributiva.";
                return false;
            }

            if (Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria))
            {
                if (!GestioneContrib.ControlsDatiCalcoloVAPE(contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.ImportoLordo : (decimal?)null, contenitore.DatiPensione, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                    return false;
                }
            }

            //ENG- Memo 68/2022 aggiornato al 12/03/2025
            if ((Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa) && (Utility.IsDomandaPensioneIndiretta(contenitore.DatiPensione) || Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda)))) &&
                !((Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) || (Utility.IsDomandaSOPGI(contenitore.DatiPensione.SiglaCategoria) && Utility.IsDomandaReversibilita(contenitore.DatiPensione) && !contenitore.IsRiaperturaDomanda)) && contenitore.DatiPensione.GP1AV91B == "2"))
            {
                GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneModificheMemoINPGI_20250312 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneModificheMemoINPGI_20250312 ", out ctrlAbilitazioneModificheMemoINPGI_20250312);
                if (ctrlAbilitazioneModificheMemoINPGI_20250312 != null && ctrlAbilitazioneModificheMemoINPGI_20250312.ValoreControllo == "SI")
                {
                    if (contenitore.ListaDatiContributiviINPGI != null && contenitore.ListaDatiContributiviINPGI.Count > 0 && (contenitore.DatiPensioniDatiGenerici == null || (contenitore.DatiPensioniDatiGenerici != null && !contenitore.DatiPensioniDatiGenerici.PL_Coeftrasf.HasValue))
                        && !((Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) || Utility.IsDomandaRipristino(contenitore.DatiPensione).Value) && contenitore.DatiPensione.GP1AV91B == "2"))
                    {
                        messaggioVideo = "Coefficiente: campo obbligatorio";
                        return false;
                    }
                }
            }
            #endregion DatiCalcolo

            #region Deleghe/Tutele
            if (!GestioneCrossControls.ALL_VerificaDelegheTuteleByIdPensione(contenitore.DatiPensione,
                contenitore.DatiAnagraficiDelegato != null ? contenitore.DatiAnagraficiDelegato.CodiceFiscale : string.Empty,
                contenitore.DatiAnagraficiTutore != null ? contenitore.DatiAnagraficiTutore.CodiceFiscale : string.Empty,
                contenitore.DatiAnagraficiTutore != null ? contenitore.DatiAnagraficiTutore.CodiceTutore : (char?)null,
                contenitore.DatiAnagraficiTutore != null ? contenitore.DatiAnagraficiTutore.CessValAmmSost : (DateTime?)null, contenitore.DatiAreaTitolare.Anagrafica.CodiceFiscale,
                contenitore.IsRiaperturaDomanda, out messaggioVideo))
                return false;
            #endregion Deleghe/Tutele

            #region Familiari
            if (!ControlsDatiFamiliari(contenitore, dataSistema, annoCompetenza, isRiaperturaDomanda, listaFamiliari, tipoAppartenenza, out messaggioVideo))
            {
                //ENG - TRF IOPGI non devono avere il TabFamiliari rosso;
                if (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, isRiaperturaDomanda) && !(Utility.IsRiaperturaDomanda(contenitore.DatiPensione.Id) && Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria)))
                    GestioneFamiliari.SbloccaFamiliari(contenitore.DatiPensione, listaFamiliari);
                return false;
            }

            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ConsultazioneANFAttivaAGO", out ctrl);
            if (ctrl != null && ctrl.ValoreControllo == "SI")
            {
                if (!isConsultazioniANFVerificate)
                {
                    if (!ControlsConsultazioneANF(contenitore, matricolaOperatore, out listaConsultazioni, out messaggioVideo))
                        return false;
                    if (listaConsultazioni != null && listaConsultazioni.Count > 0)
                        return true;
                }
            }
            #endregion Familiari

            #region MaggiorazioniBenefici
            if (contenitore.DatiMaggiorazioniBenefici != null)
            {
                if (!GestioneControlli.ControlsSettimaneIncremento(contenitore.DatiMaggiorazioniBenefici.NSettimaneIncremento1Percento, contenitore.DatiMaggiorazioniBenefici.NSettimaneIncremento05Percento,
                    contenitore.DatiPensione, contenitore.DatiDanteCausa, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsBeneficio(contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio, contenitore.DatiMaggiorazioniBenefici.NSettimaneBeneficio,
                    contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceLiquidazione : (char?)null,
                    contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.ProvenienzaPensione : (byte?)null,
                    contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.DecorrenzaPensione : (DateTime?)null,
                    contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NSettimaneOBG : (int?)null,
                    contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NContributiVolontari : null,
                    contenitore.DatiPensione, contenitore.DatiAreaTitolare.Anagrafica.Sesso, contenitore.DatiAreaTitolare.Anagrafica.DataNascita, datiENPALS, contenitore.IsRiaperturaDomanda,
                    contenitore.DatiMaggiorazioniBenefici.SettAnzContribPost311295, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico : (DateTime?)null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsSentenze(contenitore.DatiMaggiorazioniBenefici.Sentenza495240, contenitore.DatiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsDecMaggiorazioneSociale(contenitore.DatiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale, contenitore.DatiPensione,
                    contenitore.DatiAreaTitolare.Anagrafica, dataSistema, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.DecorrenzaMaggiorazioneSociale : null, contenitore.DatiDanteCausa, contenitore.IsRiaperturaDomanda, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsCessMaggiorazioneSociale(contenitore.DatiMaggiorazioniBenefici.CessazioneMaggiorazioneSociale, dataSistema, out messaggioVideo))
                    return false;

                if (!contenitore.DatiMaggiorazioniBenefici.IsMaggiorazioniAGONull() && !GestioneControlli.ControlsMaggiorazioneSocialeCUM(ref contenitore, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsAnniRiduzioneBenefici(dataSistema, contenitore.DatiMaggiorazioniBenefici.AnniRiduzioneBeneficiArt38Legge02,
                    contenitore.DatiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale, contenitore.DatiPensione, contenitore.IsRiaperturaDomanda, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsExCombattente(contenitore.DatiMaggiorazioniBenefici.CodiceCieco, contenitore.DatiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6,
                    contenitore.DatiPensione, out messaggioVideo))
                    return false;

                if (!Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria))
                    if (!GestioneControlli.ControlsSettimaneBeneficioWithDatiContribQuotaD(contenitore.DatiMaggiorazioniBenefici.NSettimaneBeneficio, contenitore.DatiPensione,
                        contenitore.ListaDatiContributivi, out messaggioVideo))
                        return false;

                if (!GestioneControlli.ControlsTipoBeneficiForPensioneInabilitaIndiretta(
                    !string.IsNullOrEmpty(contenitore.DatiPensione.NaturaPensione) ? contenitore.DatiPensione.NaturaPensione.Substring(0, 1)[0] : ' ', !string.IsNullOrEmpty(contenitore.DatiPensione.NaturaPensione) ? contenitore.DatiPensione.NaturaPensione.Substring(2, 1)[0] : ' ', contenitore.DatiPensione, isRiaperturaDomanda,
                    contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsTipoBeneficiForCumulo(
                   !string.IsNullOrEmpty(contenitore.DatiPensione.NaturaPensione) ? contenitore.DatiPensione.NaturaPensione.Substring(0, 1)[0] : ' ', contenitore.DatiPensione, isRiaperturaDomanda,
                   contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsTipoBeneficiForPensioneAUT(
                    !string.IsNullOrEmpty(contenitore.DatiPensione.NaturaPensione) ? contenitore.DatiPensione.NaturaPensione.Substring(0, 1)[0] : ' ', contenitore.DatiPensione, isRiaperturaDomanda,
                    contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, out messaggioVideo))
                    return false;

                if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(contenitore.DatiPensione, GestioneBypassControllo.NomeBypass.MaggiorazioniBenefici_Maggiorazioni_AGO.MAGG_SOCIALE_DATA_PRESENT) &&
                    !GestioneCrossControls.ALL_ControlsDecorrenzaMaggiorazioneWithDataPresentazione(contenitore.DatiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale, contenitore.DatiPensione,
                    contenitore.DatiAnagraficiTitolare.DataNascita, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.DecorrenzaMaggiorazioneSociale.HasValue : false, contenitore.DatiDanteCausa, isRiaperturaDomanda, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Maggiorazione e Benefici/Maggiorazione:<br/>" + messaggioVideo;
                    return false;
                }

                if (!GestioneCrossControls.ALL_ControlsDecorrenzaExCombattenteWithDataPresentazione(contenitore.DatiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6, contenitore.DatiPensione,
                    out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Maggiorazione e Benefici/Cieco ExCombattente:<br/>" + messaggioVideo;
                    return false;
                }

                if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
                {
                    if (!GestioneCrossControls.ALL_ControlsLavoratoriNonVedenti(contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio, contenitore.DatiMaggiorazioniBenefici.NSettimaneBeneficio,
                        contenitore.DatiMaggiorazioniBenefici.SettAnzContribPost311295, contenitore.DatiPensione, contenitore.DatiDanteCausa, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Maggiorazione e Benefici/Cieco ExCombattente:<br/>" + messaggioVideo;
                        return false;
                    }
                }
                else
                {
                    if (!GestioneCrossControls.ALL_ControlsLavoratoriNonVedenti(contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio, contenitore.DatiMaggiorazioniBenefici.NSettimaneBeneficio,
                        contenitore.DatiMaggiorazioniBenefici.SettAnzContribPost311295, contenitore.DatiPensione, contenitore.DatiDanteCausa, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Maggiorazione e Benefici/Benefici:<br/>" + messaggioVideo;
                        return false;
                    }
                }
            }

            if (contenitore.DatiBeneficioVittimeTerrorismo != null)
            {
                if (!GestioneControlli.ControlsDecorrenzaEventoTerroristico(contenitore.DatiBeneficioVittimeTerrorismo.DataEventoTerroristico, contenitore.DatiPensione.DataPresentazioneDomanda,
                    contenitore.DatiBeneficioVittimeTerrorismo.CodiceEvento, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsCoerenzaBeneficioVittimeTerrorismo(contenitore.DatiBeneficioVittimeTerrorismo.TipologiaPrestazione, contenitore.DatiBeneficioVittimeTerrorismo.TipologiaBeneficio,
                    contenitore.DatiBeneficioVittimeTerrorismo.SoggettoBeneficiario, soggettoBeneficiarioTraduzioneSuGP, out messaggioVideo))
                    return false;

                if (!Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo))
                {
                    if (!GestioneControlli.ControlsDatiCalcoloVittimeTerrorismoWithVisibility(contenitore.DatiPensione, contenitore.ListaDatiContributivi, contenitore.ListaDatiCalcoloVittimeTerrorismo,
                        contenitore.DatiBeneficioVittimeTerrorismo, contenitore.TipoCalcolo, contenitore.DatiBeneficioVittimeTerrorismo.SoggettoBeneficiario,
                        contenitore.DatiBeneficioVittimeTerrorismo.TipologiaPrestazione, contenitore.DatiBeneficioVittimeTerrorismo.TipologiaBeneficio, out messaggioVideo))
                        return false;
                }
            }
            #endregion MaggiorazioniBenefici

            #region Supplementi
            var IsPannelloSupplementiAnte96 = Utility.IsPannelloSupplementiAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, contenitore.IsRiaperturaDomanda);
            if (!IsPannelloSupplementiAnte96 && !Utility.IsDomandaRiliquidazioneIndiretta(contenitore.DatiPensione))
            {
                if (listaSupplementi != null && listaSupplementi.Count > 0)
                {
                    if (!GestioneCrossControls.AGO_VerificaSupplementi(listaSupplementi, contenitore.DatiPensione, out messaggioVideo))
                    {
                        messaggioVideo = "Supplementi / Supplementi:<br/>" + messaggioVideo;
                        return false;
                    }

                    if (!GestioneCrossControls.AGO_VerificaSupplementiDecorrenza(listaSupplementi, contenitore.DatiPensione, contenitore.DatiDanteCausa, contenitore.DatiAreaTitolare.Anagrafica,
                        Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(contenitore.DatiPensione.DecorrenzaOriginaria,
                        contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.DecorrenzaPensione : null), contenitore.IsRiaperturaDomanda, false, out messaggioVideo))
                    {
                        messaggioVideo = "Supplementi / Supplementi:<br/>" + messaggioVideo;
                        return false;
                    }
                }
                else if (contenitore.DatiStoricoGP != null && contenitore.DatiStoricoGP.GP1ALB1 == 2)
                {
                    messaggioVideo = "Supplementi / Supplementi:<br/>ATTENZIONE trattasi di pensione con GP1ALB1 = 2. È necessario acquisire i supplementi.";
                    return false;
                }

                if (contenitore.DatiSupplementiBase != null)
                {
                    if (!GestioneCrossControls.AGO_VerificaSupplementiBase(contenitore.DatiSupplementiBase.RenditaFacoltativaOrdinaria, contenitore.DatiSupplementiBase.RenditaFacoltativaConvenzionale, contenitore.DatiPensione,
                        out messaggioVideo))
                    {
                        messaggioVideo = "Supplementi / Supplementi:<br/>" + messaggioVideo;
                        return false;
                    }
                }

                if (contenitore.DatiIntegrazioneArt11 != null)
                {
                    EntityBLCommon.IntegrazioneArt11 intArt11 = new EntityBLCommon.IntegrazioneArt11();
                    intArt11.ImportoIVS = contenitore.DatiIntegrazioneArt11.ImportoIVS;
                    intArt11.Decorrenza = contenitore.DatiIntegrazioneArt11.Decorrenza;
                    if (!GestioneCrossControls.AGO_VerificaIntArt11(intArt11, listaSupplementi, contenitore.DatiPensione, out messaggioVideo))
                    {
                        messaggioVideo = "Supplementi / Supplementi:<br/>" + messaggioVideo;
                        return false;
                    }
                }
                if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) && listaSupplementiEnpals != null && listaSupplementiEnpals.Count > 0)
                {
                    if (!GestioneCrossControls.AGO_ControlDatiSupplementiEnpalsByDatiPensione(contenitore.DatiPensione, listaSupplementiEnpals, out messaggioVideo))
                    {
                        messaggioVideo = "Supplementi / Supplementi:<br/>" + messaggioVideo;
                        return false;
                    }
                }

                if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
                    if (listaSuppRecordEnpals != null)
                        foreach (EntityBLCommon.DatiSuppRecordENPALS dSuppRecordEnpals in listaSuppRecordEnpals)
                        {
                            if (!GestioneCrossControls.AGO_VerificaDecorrenzaSupplementoDecorrenzaPensioneENPALS(dSuppRecordEnpals, contenitore.DatiPensione))
                            {
                                messaggioVideo = "La decorrenza deve essere pari alla decorrenza della pensione del titolare superstite.";
                                return false;
                            }
                        }
            }
            else if (IsPannelloSupplementiAnte96)
            {
                if (!GestioneCrossControls.AGO_VerificaSupplementiAnte96Incrociati(listaSupplementi, contenitore.DatiPensione, contenitore.DatiDanteCausa,
                       Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(contenitore.DatiPensione.DecorrenzaOriginaria,
                       contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.DecorrenzaPensione : null), contenitore.IsRiaperturaDomanda, out messaggioVideo))
                {
                    messaggioVideo = "Supplementi / Supplementi:<br/>" + messaggioVideo;
                    return false;
                }
            }
            #endregion Supplementi

            #region Oneri
            if (!GestioneCrossControls.AGO_FS_ControlsOneriSperDonna(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda, contenitore.ListaDatiOneri, out messaggioVideo))
            {
                messaggioVideo = "Controlli incrociati - Oneri / Oneri:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaOneri(contenitore.DatiPensione, contenitore.ListaDatiOneri, derogaTraduzioneSuGP, contenitore.IsRiaperturaDomanda, contenitore.DatiAnagraficiTitolare, tipoAppartenenza, out messaggioVideo))
            {
                messaggioVideo = "Controlli incrociati - Oneri / Oneri:<br/>" + messaggioVideo;
                return false;
            }

            //FG - Per tutte le domande APE (automatiche e non) è richiesto l’inserimento di un controllo prima dell’invio al calcolo, 
            //che verifichi che la data di scadenza dell’indennità (Istruttoria) sia pari alla data di cessazione del beneficio dell’onere (Oneri).
            if (!GestioneCrossControls.AGO_ControlsOneriDataCessazioneAndScadenzaIndennita(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda,
                contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno : null, contenitore.ListaDatiOneri, out messaggioVideo))
            {
                messaggioVideo = "Controlli incrociati - Oneri / Oneri:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaPresenzaOneriObbligatori(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda, contenitore.ListaDatiOneri, contenitoreDecodifica.ElencoDecCodeGruppoOnere, out messaggioVideo))
            {
                messaggioVideo = "Controlli incrociati - Oneri / Oneri:<br/>" + messaggioVideo;
                return false;
            }
            #endregion Oneri

            #region Usuranti
            if (!GestioneCrossControls.ALL_VerificaCodNaturaUsuranti(contenitore.DatiPensione, out messaggioVideo))
                return false;
            #endregion Usuranti

            #region AltrePensioni
            if (Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, contenitore.IsRiaperturaDomanda) == null)
            {
                if (contenitore.ListaAltraPensione != null && contenitore.ListaAltraPensione.Count > 0)
                {
                    if (!GestioneControlli.ControlsBititolarita(contenitore.DatiPensione, listaAltraPensione, contenitoreDecodifica, out messaggioVideo))
                        return false;
                }
            }
            else
            {
                if (!GestioneControlli.ControlsBititolaritaAnte96(contenitore.DatiPensione, listaAltraPensione, out messaggioVideo))
                    return false;
            }
            #endregion AltrePensioni

            #region Eliminazione
            if (contenitore.DatiEliminazione != null)
            {
                if (!GestioneCrossControls.AGO_CI_ControlsDatiEliminazione(contenitore.DatiPensione, contenitore.DatiEliminazione.CodiceMotivo, contenitore.DatiEliminazione.DecorrenzaEliminazione,
                    contenitore.DatiEliminazione.DataEvento, contenitore.DatiEliminazione.DataFineCalcoloArretrati,
                    contenitore.DatiNuoveLiquidate != null ? contenitore.DatiNuoveLiquidate.FlagProvvisoria : null,
                    contenitore.DatiPagamento != null ? contenitore.DatiPagamento.DataRinunciaTrattenutaInpdap : null,
                    contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.ScadenzaRevisioneSanitaria : null,
                    contenitore.DatiPensione.DecorrenzaCalcoloArretrati, contenitore.IsRiaperturaDomanda, contenitore.DatiDanteCausa, out messaggioVideo))
                {
                    messaggioVideo = "Controlli incrociati - Dati Eliminazione:<br/>" + messaggioVideo;
                    return false;
                }

                if (!GestioneCrossControls.ALL_VerificaDecorrenzaEliminazioneWithRedditi(contenitore.ListaRedditoDRedd, contenitore.DatiEliminazione.DataEvento, out messaggioVideo))
                {
                    messaggioVideo = "Controlli incrociati - Dati Eliminazione:<br/>" + messaggioVideo;
                    return false;
                }
            }
            #endregion Eliminazione

            #region Periodi
            //ENG - Spacchettate SOPGI
            if (Utility.IsDomandaSpacchettamentoENPALS(contenitore.DatiPensione) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa) ||
                Utility.IsDomandaSpacchettamentoSO(contenitore.DatiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOART(contenitore.DatiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOCOM(contenitore.DatiPensione, isRiaperturaDomanda)
                || Utility.IsDomandaSpacchettamentoSR(contenitore.DatiPensione, isRiaperturaDomanda))
            {
                if (contenitore.ListaAventiDiritto != null && contenitore.ListaAventiDiritto.Count > 0)
                {
                    long idAnagraficaTitolare = contenitore.DatiAreaTitolare.Anagrafica.Id;
                    foreach (var aventeDiritto in contenitore.ListaAventiDiritto.FindAll(x => x.IdAnagrafica == idAnagraficaTitolare))
                    {
                        GestioneAnagrafica.DatiAnagrafici anagraficaAventeDiritto = contenitore.ListaAnagraficaAventiDiritto.Find(x => x.Id == aventeDiritto.IdAnagrafica);
                        List<GestionePeriodiAventiDiritto.PeriodoAventiDiritto> periodoAventiDiritto = contenitore.ListaPeriodoAventiDiritto.FindAll(x => x.IdAventeDiritto == aventeDiritto.Id);

                        if (!GestioneCrossControls.AGO_ControlsGradoParentelaPeriodi(periodoAventiDiritto, anagraficaAventeDiritto.DataNascita.Value, out messaggioVideo))
                            return false;

                        if (!GestioneCrossControls.AGO_ControlsDecorrenzaPeriodi(contenitore.DatiPensione, aventeDiritto, periodoAventiDiritto, contenitore.IsRiaperturaDomanda, out messaggioVideo))
                            return false;

                        if (!GestioneCrossControls.AGO_ControlsCessazionePeriodoAventiDiritto(periodoAventiDiritto, out messaggioVideo))
                            return false;

                        if (!GestioneCrossControls.AGO_ControlsSovrapposizionePeriodiAventiDiritto(periodoAventiDiritto, out messaggioVideo))
                            return false;

                        if (!GestioneCrossControls.AGO_ControlsSessoPeriodiAventiDiritto(periodoAventiDiritto, anagraficaAventeDiritto,
                            contenitore.DatiAnagraficiDanteCausa != null ? contenitore.DatiAnagraficiDanteCausa.Sesso : null, out messaggioVideo))
                            return false;
                    }
                }
            }
            #endregion Periodi

            #region Modalità Pagamento
            if (!Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
            {
                if (contenitore.DatiPagamento == null && Utility.IsDomandaPL(contenitore.DatiPensione) && contenitore.DatiPensione.TipoAutomazione != null && contenitore.DatiPensione.TipoAutomazione == 2)
                {
                    messaggioVideo = "Dati di pagamento assenti, verificarne la corretta acquisizione";
                    return false;
                }

                if (!GestioneCrossControls.ALL_ControlsBancaItalia(contenitore.DatiPensione, contenitore.DatiPagamento.ABI, contenitore.DatiPagamento.CAB, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Modalità Pagamento:<br/>" + messaggioVideo;
                    return false;
                }

                if (!GestioneCrossControls.ALL_ControlsDatiPagamento(contenitore.DatiPagamento.TipoPagamento, contenitore.DatiPagamento.ModalitaPagamento, contenitore.DatiPagamento.IBAN,
                    contenitore.DatiPagamento.Libretto, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Modalità Pagamento:<br/>" + messaggioVideo;
                    return false;
                }

                if (!GestioneCrossControls.AGO_ControlsDatiPagamentoByDatiCalcolo(contenitore.DatiPensione, contenitore.DatiPagamento.TipoPagamento, contenitore.DatiPagamento.ModalitaPagamento, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Modalità Pagamento:<br/>" + messaggioVideo;
                    return false;
                }

            }
            #endregion Modalità Pagamento

            #region Aventi Diritto
            if (!GestioneCrossControls.AGO_ControlsSessoAventiDirittoWithParentela(contenitore.ListaAventiDiritto, contenitore.ListaAnagraficaAventiDiritto,
                contenitore.DatiAnagraficiDanteCausa != null ? contenitore.DatiAnagraficiDanteCausa.Sesso : null, out messaggioVideo))
                return false;
            #endregion Aventi Diritto

            return true;
        }

        private static bool ControlsConsultazioneANF(EntityBLCommon.ContenitoreObject contenitore, string matricolaOperatore, out List<GestioneFamiliari.ConsultazioneUnificataANF> listaConsultazioni, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            listaConsultazioni = null;
            List<GestioneFamiliari.Familiare> listaFamiliari = contenitore.ListaFamiliari;
            List<GestioneFamiliari.DatiRichiestaRicercaDomandeANF> listaRichieste = contenitore.ListaRichiesteRicercaDomandeANF;
            if (listaRichieste != null && listaRichieste.Count > 0 && listaFamiliari != null && listaFamiliari.Count > 0)
            {
                listaConsultazioni = new List<GestioneFamiliari.ConsultazioneUnificataANF>();
                foreach (GestioneFamiliari.DatiRichiestaRicercaDomandeANF richiesta in listaRichieste)
                {
                    string codiceFiscale = listaFamiliari.FirstOrDefault(x => x.IdAnagrafica == richiesta.IdAnagrafica).CodiceFiscale;
                    string rispostaConsultazione = string.Empty;
                    GestioneFamiliari.ConsultazioneUnificataANF consultazioneANF = null;
                    if (!GestioneANF.RichiediRispostaById(contenitore.DatiPensione.NDomus.ToString(), codiceFiscale, richiesta.Guid, matricolaOperatore, out rispostaConsultazione, out messaggioVideo))
                    {
                        listaConsultazioni = null;
                        return false;
                    }
                    if (!GestioneFamiliari.ControllaRispostaANF(rispostaConsultazione, out consultazioneANF, out messaggioVideo))
                    {
                        listaConsultazioni = null;
                        return false;
                    }
                    if (consultazioneANF != null)
                        listaConsultazioni.Add(consultazioneANF);
                }
            }

            return true;
        }

        private static bool ControlsDatiFamiliari(EntityBLCommon.ContenitoreObject contenitore, DateTime dataSistema, int annoCompetenza, bool isRiaperturaDomanda, List<GestioneFamiliari.Familiare> listaFamiliari, Utility.TipoAppartenenza? tipoAppartenenza, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (listaFamiliari != null && listaFamiliari.Count > 0)
            {
                if (!GestioneCrossControls.AGO_VerificaFamiliari(string.Empty, listaFamiliari, contenitore.ListaCodMaggFamiliari, contenitore.DatiPensione,
                    contenitore.DatiAreaTitolare.ElencoStatiCivili, contenitore.DatiDanteCausa, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.CumuloEsterno : null,
                    dataSistema, contenitore.IsRiaperturaDomanda, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Familiari:<br/>" + messaggioVideo;
                    return false;
                }
            }

            if (!GestioneCrossControls.ALL_VerificaSovrapposizioneCodMaggFamiliariConiugi(listaFamiliari, contenitore.ListaCodMaggFamiliari, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_ControlsFamiliariWithStatiCivili(listaFamiliari, contenitore.ListaCodMaggFamiliari, contenitore.DatiAreaTitolare.ElencoStatiCivili, tipoAppartenenza,
                contenitore.DatiAreaTitolare.Anagrafica.DataMorte, out messaggioVideo))
                return false;

            //Per ciascun familiare presente verificare che il codice fiscale del Titolare  sia diverso dal codice fiscale del familiare 
            if (!GestioneCrossControls.ALL_VerificaFamiliariTitolare(listaFamiliari, contenitore.DatiAreaTitolare, contenitore.DatiPensione, tipoAppartenenza,
                contenitore.IsRiaperturaDomanda, contenitore.DatiDanteCausa))
            {
                messaggioVideo = "Controlli Incrociati - Dati Familiari:<br/>Il titolare pensione non può essere presente nell'elenco dei familiari.";
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaFamiliariMorti(listaFamiliari, contenitore.ListaCodMaggFamiliari, contenitore.DatiPensione.DecorrenzaOriginaria, tipoAppartenenza, out messaggioVideo, contenitore.DatiPensione, contenitore.DatiEliminazione))
                return false;

            if (!GestioneCrossControls.ALL_VerificaCessazioneCodMagg(listaFamiliari, contenitore.ListaCodMaggFamiliari, dataSistema, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Familiari:<br/>" + messaggioVideo;
                return false;
            }

            if (listaFamiliari != null && listaFamiliari.Count > 0)
            {
                foreach (GestioneFamiliari.Familiare fam in listaFamiliari)
                {
                    List<GestioneFamiliari.CodMaggFamiliari> LcodMaggFam = contenitore.ListaCodMaggFamiliari.FindAll(x => x.IdAnagrafica == fam.IdAnagrafica);
                    if (LcodMaggFam != null && LcodMaggFam.Count > 0 &&
                        LcodMaggFam.Exists(x => x.Decorrenza.HasValue && x.Cessazione.HasValue && !Utility.DataStrettamenteSuccessivaA(x.Cessazione.Value, x.Decorrenza.Value)))
                    {
                        messaggioVideo = "Per il familiare " + fam.CodiceFiscale + " la data fine carico non può essere inferiore alla data decorrenza carico";
                        return false;
                    }
                }
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaCarico(listaFamiliari, contenitore.ListaCodMaggFamiliari, contenitore.DatiPensione, contenitore.DatiDanteCausa, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Familiari:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaFamiliariConiugiTitolareConiugato(contenitore.DatiPensione, contenitore.DatiAreaTitolare, listaFamiliari, true, contenitore.DatiDanteCausa, contenitore.IsRiaperturaDomanda, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_CI_VerificaFamiliariConiugiRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.DatiEliminazione, listaFamiliari, contenitore.ListaCodMaggFamiliari, isRiaperturaDomanda, out messaggioVideo))
                return false;

            if (!Utility.IsDomandaPensioneSuperstitiOrRicostituzione(contenitore.DatiPensione) &&
                !GestioneCrossControls.ALL_VerificaDecorrenzaCodMaggFamiliariNipoti(listaFamiliari, contenitore.ListaCodMaggFamiliari, dataSistema))
            {
                messaggioVideo = "Controlli Incrociati - Dati Familiari: Non è possibile inserire per i nipoti una data fine carico successiva a Gennaio " + (dataSistema.Year + 1).ToString();
                return false;
            }

            if (!GestioneCrossControls.AGO_ControlsQuotaContitolaritaNipote(contenitore.DatiPensione, tipoAppartenenza, listaFamiliari, contenitore.ListaCodMaggFamiliari, annoCompetenza, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_ControlsDataCessazioneFamiliari(contenitore.DatiPensione, tipoAppartenenza, listaFamiliari, contenitore.ListaCodMaggFamiliari, contenitore.DatiPensione.SiglaCategoria, contenitore.DatiEliminazione, out messaggioVideo))
                return false;

            if (listaFamiliari != null && listaFamiliari.Count > 0 && contenitore.ListaCodMaggFamiliari != null && contenitore.ListaCodMaggFamiliari.Count > 0)
            {
                foreach (GestioneFamiliari.Familiare fam in listaFamiliari)
                {
                    List<GestioneFamiliari.CodMaggFamiliari> LcodMaggFam = contenitore.ListaCodMaggFamiliari.FindAll(x => x.IdAnagrafica == fam.IdAnagrafica);
                    if (!GestioneCrossControls.ALL_VerificaDecorrenzaCessazioneFamiliari(contenitore.DatiPensione, tipoAppartenenza, fam, LcodMaggFam))
                    {
                        messaggioVideo = "Non è consentito l'inserimento del 'SI' diritto da 03/2022 a nessun familiare che abbia sigla U, S, M, L. Cambiare codice maggiorazione o data inizio/fine carico";
                        return false;
                    }

                    if (fam.SiglaFamiliare == 'N' || fam.SiglaFamiliare == 'J')
                    {
                        if (!GestioneCrossControls.ALL_VerificaMaggiorazioneFamiliariNeJ(contenitore.DatiPensione, tipoAppartenenza, fam, LcodMaggFam, out messaggioVideo))
                        {
                            return false;
                        }
                    }
                }
            }

            //ENG - Memo 22/2024
            if (!GestioneCrossControls.ALL_VerificaPlurimeRegistrazioniConiugeUnitoCivile(contenitore.DatiPensione, tipoAppartenenza, listaFamiliari, contenitore.ListaCodMaggFamiliari, out messaggioVideo))
            {
                return false;
            }

            return true;
        }
        #endregion public members

        #region private members
        private static void ValorizzaAreaCalcolo(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, string matricolaOperatore,
            short sedeOperatore, short centroOperativoOperatore, DateTime dataSistema, int annoCompetenza, out Data.GAPL_GARC AreaCalcolo)
        {
            if (contenitore.DatiPensione == null)
                throw new INPS.DNA.DnaApplicationException("Nessuna pensione associata al numero di domanda.");
            string transazione = "GAPL";
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto);
            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || contenitore.IsRiaperturaDomanda)
            {
                string activeCARC = ConfigurationManager.AppSettings["CARC"];
                if (!string.IsNullOrEmpty(activeCARC) && activeCARC == "SI")
                    transazione = "CARC";
                else
                    transazione = "GARC";
            }
            INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = Utility.GetOfficeByAspnCode(sedeOperatore.ToString().PadLeft(4, '0') + centroOperativoOperatore.ToString().PadLeft(2, '0'));

            Data.HostRequest.GAPL_GARCRequest richiesta = null;

            if (contenitore.DatiPensione.IsRicRinnovata.GetValueOrDefault())
                annoCompetenza = dataSistema.Year + 1;

            MappingVersoHost.ValorizzaRichiesta(matricolaOperatore, sedeOperatore, ref contenitore, ref contenitoreDecodifica, dataSistema, annoCompetenza, out richiesta);

            AreaCalcolo = new INPS.Pensioni.LiquidazioneAgo.Data.GAPL_GARC(transazione);
            AreaCalcolo.Request = richiesta;
        }

        private static void ValorizzaAreaCalcoloNuovoTracciato(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, string matricolaOperatore,
            short sedeOperatore, short centroOperativoOperatore, DateTime dataSistema, int annoCompetenza, out Data.GAPL_GARC_New AreaCalcolo, out string messaggioEccezione)
        {
            if (contenitore.DatiPensione == null)
                throw new INPS.DNA.DnaApplicationException("Nessuna pensione associata al numero di domanda.");
            string transazione = "GAPL";
            messaggioEccezione = string.Empty;
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto);
            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || contenitore.IsRiaperturaDomanda)
            {
                string activeCARC = ConfigurationManager.AppSettings["CARC"];
                if (!string.IsNullOrEmpty(activeCARC) && activeCARC == "SI")
                    transazione = "CARC";
                else
                    transazione = "GARC";
            }
            INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = Utility.GetOfficeByAspnCode(sedeOperatore.ToString().PadLeft(4, '0') + centroOperativoOperatore.ToString().PadLeft(2, '0'));

            Data.HostRequest.GAPL_GARCRequestNew richiesta = null;

            if (contenitore.DatiPensione.IsRicRinnovata.GetValueOrDefault())
                annoCompetenza = dataSistema.Year + 1;

            MappingVersoHostNew.ValorizzaRichiesta(matricolaOperatore, sedeOperatore, ref contenitore, ref contenitoreDecodifica, dataSistema, annoCompetenza, out richiesta, out messaggioEccezione);

            AreaCalcolo = new INPS.Pensioni.LiquidazioneAgo.Data.GAPL_GARC_New(transazione);
            AreaCalcolo.RequestNew = richiesta;
        }

        private static void EseguiCalcolo(Data.GAPL_GARC AreaCalcolo)
        {
            if (AreaCalcolo.TransactionName == "CARC")
            {
                Data.HostRequest.CopericonRequest richiesta = null;
                MappingVersoHost.ValorizzaRichiesta(AreaCalcolo, out richiesta);
                Data.Copericon copericon = new Data.Copericon();
                copericon.Request = richiesta;
                copericon.Invoke();
                if (!copericon.Response.Esito)
                {
                    AreaCalcolo.HasError = true;
                    AreaCalcolo.Messaggio = copericon.Response.Messaggio;
                    return;
                }
            }
            AreaCalcolo.Invoke();
        }

        private static void EseguiCalcoloNuovoTracciato(Data.GAPL_GARC_New AreaCalcolo)
        {
            if (AreaCalcolo.TransactionName == "CARC")
            {
                Data.HostRequest.CopericonRequest richiesta = null;
                MappingVersoHostNew.ValorizzaRichiesta(AreaCalcolo, out richiesta);
                Data.Copericon copericon = new Data.Copericon();
                copericon.Request = richiesta;
                copericon.Invoke();
                if (!copericon.Response.Esito)
                {
                    AreaCalcolo.HasError = true;
                    AreaCalcolo.Messaggio = copericon.Response.Messaggio;
                    return;
                }
            }

            AreaCalcolo.Invoke();
        }

        private static void ControllaEsitoCalcolo(long numeroDomanda, Data.GAPL_GARC AreaCalcolo, out string statoPensione, out bool esito, out string messaggioVideo)
        {
            esito = false;
            statoPensione = null;
            messaggioVideo = AreaCalcolo.Messaggio;

            //recupero il dettagli dell'errore 
            if (AreaCalcolo.CodiciErrore != null && AreaCalcolo.CodiciErrore.Count > 0)
            {
                messaggioVideo += GetDettaglioErrore(AreaCalcolo.CodiciErrore);
            }

            //// Questa Get viene eseguita per evitare di avere dati sporchi modificati dalla valorizzazione area calcolo
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, null, out datiPensione);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneEsitoCalcolo.DatiEsitoCalcolo datiEsitoCalcolo = new GestioneEsitoCalcolo.DatiEsitoCalcolo();
                datiEsitoCalcolo.DettaglioEsito = messaggioVideo;

                if (AreaCalcolo.Response != null && AreaCalcolo.Response.Controllo != null)
                {
                    if (AreaCalcolo.Response.Controllo.FLAG_INDEB != null && AreaCalcolo.Response.Controllo.FLAG_INDEB.Trim() != "0")
                        datiPensione.FlagIndebito = AreaCalcolo.Response.Controllo.FLAG_INDEB;

                    switch (AreaCalcolo.Response.Controllo.PER_CODESITO)
                    {
                        case "A":
                        case "E":
                        case "P":
                            if (datiPensione.FlagVerify.HasValue && !datiPensione.FlagVerify.Value)
                                //CALCOLATA
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoWebDom;
                            else
                                //CALCOLO VERIFY
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcoloVerify;
                            if (AreaCalcolo.Response.Controllo.DATA_CALCA != 0 && AreaCalcolo.Response.Controllo.DATA_CALCM != 0 && AreaCalcolo.Response.Controllo.DATA_CALCG != 0)
                            {
                                string dataCalcolo = AreaCalcolo.Response.Controllo.DATA_CALCA.ToString().PadLeft(4, '0') +
                                    AreaCalcolo.Response.Controllo.DATA_CALCM.ToString().PadLeft(2, '0') +
                                    AreaCalcolo.Response.Controllo.DATA_CALCG.ToString().PadLeft(2, '0');
                                datiPensione.DataElaborazione = Utility.DataFromString(dataCalcolo, Utility.FormatoData.AAAAmmGG);
                            }
                            GestionePensione.SalvaPensione(datiPensione);
                            esito = true;
                            break;
                        case "S":
                            if (datiPensione.FlagVerify.HasValue && !datiPensione.FlagVerify.Value)
                                //SCARTO DA CALCOLO
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.ScartoDaCalcolo;
                            else
                                //SCARTO VERIFY
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.ScartoVerify;
                            GestionePensione.SalvaPensione(datiPensione);
                            esito = false;
                            break;
                        default:
                            esito = false;
                            break;
                    }
                }
                if (esito)
                {
                    datiEsitoCalcolo.Esito = "OK";
                    if (datiPensione.StatoPensione == (int)Utility.StatoPensione.CalcolataNoWebDom)
                        GestioneLogGenerico.EliminaLogGenerico(numeroDomanda);
                }
                else
                    datiEsitoCalcolo.Esito = "KO";

                GestioneEsitoCalcolo.SalvaEsitoCalcolo(datiPensione.Id, datiEsitoCalcolo);
                transactionScope.Complete();
            }

            GestioneDecodifica.GetStatoPensioneById(datiPensione.StatoPensione.Value, out statoPensione);
        }

        private static void ControllaEsitoCalcoloNuovoTracciato(long numeroDomanda, Data.GAPL_GARC_New AreaCalcolo, out string statoPensione, out bool esito, out string messaggioVideo)
        {
            esito = false;
            statoPensione = null;
            messaggioVideo = AreaCalcolo.Messaggio;

            //recupero il dettagli dell'errore 
            if (AreaCalcolo.CodiciErrore != null && AreaCalcolo.CodiciErrore.Count > 0)
            {
                messaggioVideo += GetDettaglioErrore(AreaCalcolo.CodiciErrore);
            }

            //// Questa Get viene eseguita per evitare di avere dati sporchi modificati dalla valorizzazione area calcolo
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, null, out datiPensione);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneEsitoCalcolo.DatiEsitoCalcolo datiEsitoCalcolo = new GestioneEsitoCalcolo.DatiEsitoCalcolo();
                datiEsitoCalcolo.DettaglioEsito = messaggioVideo;

                if (AreaCalcolo.Response != null && AreaCalcolo.Response.Controllo != null)
                {

                    if (AreaCalcolo.Response.Controllo.FLAG_INDEB != null && AreaCalcolo.Response.Controllo.FLAG_INDEB.Trim() != "0")
                        datiPensione.FlagIndebito = AreaCalcolo.Response.Controllo.FLAG_INDEB;

                    switch (AreaCalcolo.Response.Controllo.PER_CODESITO)
                    {
                        case "A":
                        case "E":
                        case "P":
                            if (datiPensione.FlagVerify.HasValue && !datiPensione.FlagVerify.Value)
                                //CALCOLATA
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoWebDom;
                            else
                                //CALCOLO VERIFY
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcoloVerify;
                            if (AreaCalcolo.Response.Controllo.DATA_CALCA != 0 && AreaCalcolo.Response.Controllo.DATA_CALCM != 0 && AreaCalcolo.Response.Controllo.DATA_CALCG != 0)
                            {
                                string dataCalcolo = AreaCalcolo.Response.Controllo.DATA_CALCA.ToString().PadLeft(4, '0') +
                                    AreaCalcolo.Response.Controllo.DATA_CALCM.ToString().PadLeft(2, '0') +
                                    AreaCalcolo.Response.Controllo.DATA_CALCG.ToString().PadLeft(2, '0');
                                datiPensione.DataElaborazione = Utility.DataFromString(dataCalcolo, Utility.FormatoData.AAAAmmGG);
                            }
                            GestionePensione.SalvaPensione(datiPensione);
                            esito = true;
                            break;
                        case "S":
                            if (datiPensione.FlagVerify.HasValue && !datiPensione.FlagVerify.Value)
                                //SCARTO DA CALCOLO
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.ScartoDaCalcolo;
                            else
                                //SCARTO VERIFY
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.ScartoVerify;
                            GestionePensione.SalvaPensione(datiPensione);
                            esito = false;
                            break;
                        default:
                            esito = false;
                            break;
                    }
                }
                if (esito)
                {
                    datiEsitoCalcolo.Esito = "OK";
                    if (datiPensione.StatoPensione == (int)Utility.StatoPensione.CalcolataNoWebDom)
                        GestioneLogGenerico.EliminaLogGenerico(numeroDomanda);
                }
                else
                    datiEsitoCalcolo.Esito = "KO";

                GestioneEsitoCalcolo.SalvaEsitoCalcolo(datiPensione.Id, datiEsitoCalcolo);
                transactionScope.Complete();
            }

            GestioneDecodifica.GetStatoPensioneById(datiPensione.StatoPensione.Value, out statoPensione);
        }

        /// <summary>
        /// Dati i codici di errore restituisce il messaggio testuale con la descrizione degli errori. 
        /// </summary>
        /// <param name="codiciErrore">codici errori</param>
        /// <returns>messaggio con descrizioni</returns>
        private static string GetDettaglioErrore(List<short> codiciErrore)
        {
            StringBuilder messaggioErrore = new StringBuilder();
            foreach (short codErr in codiciErrore)
            {
                GestioneErroriCalcolo.ErroriCalcolo erroreCalcolo = null;
                try
                {
                    GestioneErroriCalcolo.GetErroriCalcolo(codErr, GestioneErroriCalcolo.Procedura.ALL, GestioneErroriCalcolo.Gestione.ALL, out erroreCalcolo);
                }
                catch (Exception ex)
                {
                    INPS.DNA.Logging.Logger.WriteError(ex.Message);
                }
                messaggioErrore.Append(" - ");
                if (erroreCalcolo != null)
                    messaggioErrore.Append(erroreCalcolo.Descrizione);
                messaggioErrore.Append("<br/>");
            }
            return messaggioErrore.ToString();
        }

        private static string GetDettaglioErroreMonitoraggio(List<short> codiciErrore)
        {
            StringBuilder messaggioErrore = new StringBuilder();
            foreach (short codErr in codiciErrore)
            {
                GestioneErroriCalcolo.ErroriCalcolo erroreCalcolo = null;
                try
                {
                    GestioneErroriCalcolo.GetErroriCalcolo(codErr, GestioneErroriCalcolo.Procedura.ALL, GestioneErroriCalcolo.Gestione.ALL, out erroreCalcolo);
                }
                catch (Exception ex)
                {
                    INPS.DNA.Logging.Logger.WriteError(ex.Message);
                }

                if (erroreCalcolo != null)
                    messaggioErrore.Append(erroreCalcolo.Descrizione);
                messaggioErrore.Append(";");
            }
            return messaggioErrore.ToString();
        }
        #endregion private members
    }
}
