using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Configuration;
using INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneCi;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;
using System.Reflection;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneLiquidazioneCi
    {
        #region internal members

        #endregion internal members

        #region public members
        internal static bool PrelevaDomanda(string numDomanda, short sede, short categoria, int certificato, short sedeOperatore, short centroOperativoOperatore,
            string gruppo, string prodotto, string dataMorteDC, bool isRiapertura, out AreaPrelievo prelievo, out string errori)
        {
            bool erroreTecnico = false;
            errori = "";
            prelievo = null;
            ServizioLiquidazioneCiClient proxy = new ServizioLiquidazioneCiClient();
            string stackTrace = null;

            Guid guid = Guid.NewGuid();

            using (new MethodExecutionTracer())
            {
                try
                {
                    prelievo = new AreaPrelievo();
                    prelievo.Richiesta = new GestionePrelievoRichiestaPrelievo();
                    prelievo.Richiesta._NumDomanda = numDomanda;
                    prelievo.Richiesta._Sede = sede;
                    prelievo.Richiesta._Categoria = categoria;
                    prelievo.Richiesta._Certificato = certificato;
                    prelievo.Richiesta._SedeOperatore = sedeOperatore;
                    prelievo.Richiesta._CentroOperativoOperatore = centroOperativoOperatore;
                    prelievo.Richiesta._CodiceAs = "AS";
                    Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(gruppo, prodotto);
                    if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura)
                        prelievo.Richiesta._TipoDomanda = GestionePrelievoTipoDomanda.Ricostituzione;
                    else if (tipoDomanda == Utility.TipoDomanda.Superstiti)
                    {
                        prelievo.Richiesta._TipoDomanda = GestionePrelievoTipoDomanda.Reversibilità;
                        prelievo.Richiesta._CodiceAf = "SO";
                        prelievo.Richiesta._AltriDati = !string.IsNullOrEmpty(dataMorteDC) ? dataMorteDC.Substring(0, 6) + dataMorteDC : null;
                    }
                    prelievo.Richiesta._IsRiaperturaDomanda = isRiapertura;

                    GestioneLogSoap.SalvaLogSoap(prelievo, Utility.Servizio.SrvLiquidazioneCi, Utility.MetodoServizio.PrelevaDomanda, Utility.SOAPLogDirection.IN, numDomanda, guid);

                    AreaEsito Esito = proxy.PrelevaDomanda(ref prelievo);
                    if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                    {
                        errori = Esito.Messaggio;
                        return false;
                    }
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio LiquidazioneCi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio LiquidazioneCi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio LiquidazioneCi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio LiquidazioneCi: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il prelievo dei dati della pensione";
                        string parametri = string.Format("Numero domanda: {0}; Sede: {1}; Categoria: {2}; Certificato: {3}: Sede operatore: {4}; Centro operativo operatore: {5}; Gruppo: {6}; Prodotto: {7}: E' una riapertura: {8}",
                            numDomanda, sede, categoria, certificato, sedeOperatore, centroOperativoOperatore, gruppo, prodotto, isRiapertura ? "SI" : "NO");
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(prelievo, Utility.Servizio.SrvLiquidazioneCi, Utility.MetodoServizio.PrelevaDomanda, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
                return true;
            }
        }

        internal static bool GetStatiEsteri(GestionePensione.DatiPensione datiPensione, short codiceSede, short centroOperativo, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore,
            List<INPS.Pensioni.Liquidazione.ServiceReferences.AggPec.CI_ISTITUZIONI> istituzioniEsterePECO, out List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEE, 
            out List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteri, out string cittadinanzaTitolare, out string errori)
        {
            bool erroreTecnico = false;
            errori = "";
            listaPrestazioniEE = null;
            listaImportiEsteri = null;
            cittadinanzaTitolare = string.Empty;
            ServizioLiquidazioneCiClient proxy = new ServizioLiquidazioneCiClient();
            string stackTrace = null;
            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneContribStatoEstero[] listaStatiEsteri = null;
                    AreaEsito Esito = proxy.GetStatiEsteri(out listaStatiEsteri, out cittadinanzaTitolare, datiPensione.NDomus, codiceSede, centroOperativo, matricolaOperatore, sedeOperatore, centroOperativoOperatore);
                    if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                    {
                        errori = Esito.Messaggio;
                        return false;
                    }

                    if (listaStatiEsteri != null && listaStatiEsteri.Length > 0)
                    {
                        listaPrestazioniEE = new List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE>();
                        listaImportiEsteri = new List<GestioneDatiContributiviCi.PensioniCiImportiEsteri>();
                        foreach (GestioneContribStatoEstero statoEstero in listaStatiEsteri)
                        {
                            INPS.Pensioni.Liquidazione.ServiceReferences.AggPec.CI_ISTITUZIONI istituzionePECO = null;
                            if (istituzioniEsterePECO != null && istituzioniEsterePECO.Count > 0)
                                istituzionePECO = istituzioniEsterePECO.Find(x => x.CI_Istit == statoEstero.PrestazioneEstera.CodiceIstituzione && x.CI_Stato == statoEstero.PrestazioneEstera.CodiceStatoEE);
                            if (istituzionePECO != null)
                            {
                                if (!Utility.IsDoubleEquals(istituzionePECO.CI_Misest, 0))
                                    statoEstero.PrestazioneEstera.ContributiEEDecorrenzaOriginaria = (int)istituzionePECO.CI_Misest;

                                if (!Utility.IsDoubleEquals(istituzionePECO.CI_Direst, 0))
                                    statoEstero.PrestazioneEstera.ContributiEEDiritto = (int)istituzionePECO.CI_Direst;
                            }

                            statoEstero.PrestazioneEstera.IdPensione = datiPensione.Id;
                            if (String.IsNullOrEmpty(statoEstero.PrestazioneEstera.MatricolaIstituzioneEE))
                                statoEstero.PrestazioneEstera.MatricolaIstituzioneEE = statoEstero.PrestazioneEstera.MatricolaIstituzione;
                            listaPrestazioniEE.Add(statoEstero.PrestazioneEstera);
                        }
                    }
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio LiquidazioneCi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio LiquidazioneCi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio LiquidazioneCi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio LiquidazioneCi: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il prelievo dei dati della pensione";
                        string parametri = string.Format("Numero domanda: {0}; Codice sede: {1}; Centro operativo: {2}; Matricola operatore: {3}: Sede operatore: {4}; Centro operativo operatore: {5}",
                             datiPensione.NDomus, codiceSede, centroOperativo, matricolaOperatore, sedeOperatore, centroOperativoOperatore);
                        GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }
                return true;
            }
        }
        //ENG - RIC/TRF: aggiunta la gestione per il recupero degli stati(se presenti e diversi da quelli provenienti da prelievo) dal servizio Naci o AllegatiConvenzioni
        internal static bool GetStatiEsteriRicTrf(GestionePensione.DatiPensione datiPensione, short codiceSede, short centroOperativo, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore,
            List<INPS.Pensioni.Liquidazione.ServiceReferences.AggPec.CI_ISTITUZIONI> istituzioniEsterePECO, out List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEE,
            out List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteri, out string cittadinanzaTitolare, out string errori)
        {
            bool erroreTecnico = false;
            errori = "";
            listaPrestazioniEE = null;
            listaImportiEsteri = null;
            cittadinanzaTitolare = string.Empty;
            ServizioLiquidazioneCiClient proxy = new ServizioLiquidazioneCiClient();
            string stackTrace = null;
            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneContribStatoEstero[] listaStatiEsteri = null;
                    AreaEsito Esito = proxy.GetStatiEsteriRicTrf(out listaStatiEsteri, out cittadinanzaTitolare, datiPensione.NDomus, codiceSede, centroOperativo, matricolaOperatore, sedeOperatore, centroOperativoOperatore);
                    if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                    {
                        errori = Esito.Messaggio;
                        return false;
                    }

                    if (listaStatiEsteri != null && listaStatiEsteri.Length > 0)
                    {
                        listaPrestazioniEE = new List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE>();
                        listaImportiEsteri = new List<GestioneDatiContributiviCi.PensioniCiImportiEsteri>();
                        foreach (GestioneContribStatoEstero statoEstero in listaStatiEsteri)
                        {
                            INPS.Pensioni.Liquidazione.ServiceReferences.AggPec.CI_ISTITUZIONI istituzionePECO = null;
                            if (istituzioniEsterePECO != null && istituzioniEsterePECO.Count > 0)
                                istituzionePECO = istituzioniEsterePECO.Find(x => x.CI_Istit == statoEstero.PrestazioneEstera.CodiceIstituzione && x.CI_Stato == statoEstero.PrestazioneEstera.CodiceStatoEE);
                            if (istituzionePECO != null)
                            {
                                if (!Utility.IsDoubleEquals(istituzionePECO.CI_Misest, 0))
                                    statoEstero.PrestazioneEstera.ContributiEEDecorrenzaOriginaria = (int)istituzionePECO.CI_Misest;

                                if (!Utility.IsDoubleEquals(istituzionePECO.CI_Direst, 0))
                                    statoEstero.PrestazioneEstera.ContributiEEDiritto = (int)istituzionePECO.CI_Direst;
                            }

                            statoEstero.PrestazioneEstera.IdPensione = datiPensione.Id;
                            if (String.IsNullOrEmpty(statoEstero.PrestazioneEstera.MatricolaIstituzioneEE))
                                statoEstero.PrestazioneEstera.MatricolaIstituzioneEE = statoEstero.PrestazioneEstera.MatricolaIstituzione;
                            listaPrestazioniEE.Add(statoEstero.PrestazioneEstera);
                        }
                    }
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio LiquidazioneCi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio LiquidazioneCi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio LiquidazioneCi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio LiquidazioneCi: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il prelievo dei dati della pensione";
                        string parametri = string.Format("Numero domanda: {0}; Codice sede: {1}; Centro operativo: {2}; Matricola operatore: {3}: Sede operatore: {4}; Centro operativo operatore: {5}",
                             datiPensione.NDomus, codiceSede, centroOperativo, matricolaOperatore, sedeOperatore, centroOperativoOperatore);
                        GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }
                return true;
            }
        }
        #endregion public members
    }
}