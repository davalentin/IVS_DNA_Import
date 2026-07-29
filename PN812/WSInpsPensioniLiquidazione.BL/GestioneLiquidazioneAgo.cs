using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Configuration;
using INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneAgo;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;
using System.Reflection;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneLiquidazioneAgo
    {
        #region internal members

        #endregion internal members

        #region public members
        internal static bool PrelevaDomanda(string numDomanda, short sede, short categoria, int certificato, short sedeOperatore, short centroOperativoOperatore,
            string gruppo, string prodotto, bool isRiapertura, string tipo, out AreaPrelievo prelievo, out string errori)
        {
            bool erroreTecnico = false;
            errori = "";
            prelievo = null;
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            ServizioLiquidazioneAgoClient proxy = new ServizioLiquidazioneAgoClient();
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
                    prelievo.Richiesta._Tipo = tipo;
                    prelievo.Richiesta._Prodotto = prodotto;
                    Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(gruppo, prodotto);
                    if (isRiapertura)
                        prelievo.Richiesta._TipoDomanda = GestionePrelievoTipoDomanda.Ricostituzione;
                    else if (tipoDomanda == Utility.TipoDomanda.Ricostituzione)
                    {
                        prelievo.Richiesta._TipoDomanda = GestionePrelievoTipoDomanda.Ricostituzione;
                        if (Utility.IsRicostituzione_MotiviContributivi(gruppo, prodotto))
                            prelievo.Richiesta._TipoRicostituzione = GestionePrelievoTipoRicostituzione.MotiviContributivi;
                        else
                            prelievo.Richiesta._TipoRicostituzione = GestionePrelievoTipoRicostituzione.Altro;
                    }
                    else if (tipoDomanda == Utility.TipoDomanda.Superstiti)
                        prelievo.Richiesta._TipoDomanda = GestionePrelievoTipoDomanda.Superstiti;
                    else if (tipoDomanda == Utility.TipoDomanda.Ripristino)
                        prelievo.Richiesta._TipoDomanda = GestionePrelievoTipoDomanda.Ripristino;
                    else if (tipoDomanda == Utility.TipoDomanda.RipristinoSuperstiti)
                        prelievo.Richiesta._TipoDomanda = GestionePrelievoTipoDomanda.RipristinoSuperstiti;
                    else if (tipoDomanda == Utility.TipoDomanda.Riliquidazione)
                        prelievo.Richiesta._TipoDomanda = GestionePrelievoTipoDomanda.Riliquidazione;
                    else if (tipoDomanda == Utility.TipoDomanda.RiliquidazioneSuperstiti)
                        prelievo.Richiesta._TipoDomanda = GestionePrelievoTipoDomanda.RiliquidazioneSuperstiti;

                    GestioneLogSoap.SalvaLogSoap(prelievo, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.PrelevaDomanda, Utility.SOAPLogDirection.IN, numDomanda, guid);

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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio LiquidazioneAgo | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio LiquidazioneAgo | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio LiquidazioneAgo | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio LiquidazioneAgo: {0}", Utility.GetMessageFromException(Ex));
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
                    GestioneLogSoap.SalvaLogSoap(prelievo, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.PrelevaDomanda, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
                return true;
            }
        }

        internal static bool PrelevaGP4(string numDomanda, short sede, short categoria, int certificato, short sedeOperatore, short centroOperativoOperatore, out AreaPrelievo prelievo, out string errori)
        {
            bool erroreTecnico = false;
            errori = "";
            prelievo = null;
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            ServizioLiquidazioneAgoClient proxy = new ServizioLiquidazioneAgoClient();
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
                    prelievo.Richiesta._TipoDomanda = GestionePrelievoTipoDomanda.Superstiti;
                    prelievo.Richiesta._Prodotto = "";

                    GestioneLogSoap.SalvaLogSoap(prelievo, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.PrelevaDomanda, Utility.SOAPLogDirection.IN, numDomanda, guid);

                    AreaEsito Esito = proxy.PrelevaGP4(ref prelievo);
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio LiquidazioneAgo | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio LiquidazioneAgo | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio LiquidazioneAgo | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio LiquidazioneAgo: {0}", Utility.GetMessageFromException(Ex));
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
                        string parametri = string.Format("Numero domanda: {0}; Sede: {1}; Categoria: {2}; Certificato: {3}: Sede operatore: {4}; Centro operativo operatore: {5}",
                            numDomanda, sede, categoria, certificato, sedeOperatore, centroOperativoOperatore);
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(prelievo, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.PrelevaDomanda, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
                return true;
            }
        }
        //ENG - MEMO 74_2023
        internal static bool GetStatiEsteri(GestionePensione.DatiPensione datiPensione, short codiceSede, short centroOperativo, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, out List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo> listaPrestazioniEE,
           out List<GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo> listaImportiEsteri, out string errori)
        {
            bool erroreTecnico = false;
            errori = "";
            listaPrestazioniEE = null;
            listaImportiEsteri = null;
            ServizioLiquidazioneAgoClient proxy = new ServizioLiquidazioneAgoClient();
            string stackTrace = null;
            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneContribStatoEsteroCumulo[] listaStatiEsteri = null;
                    AreaEsito Esito = proxy.GetStatiEsteri(out listaStatiEsteri, datiPensione.NDomus, codiceSede, centroOperativo, matricolaOperatore, sedeOperatore, centroOperativoOperatore, datiPensione.Id);
                    if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                    {
                        errori = Esito.Messaggio;
                        return false;
                    }

                    if (listaStatiEsteri != null && listaStatiEsteri.Length > 0)
                    {
                        listaPrestazioniEE = new List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo>();
                        listaImportiEsteri = new List<GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo>();
                        foreach (GestioneContribStatoEsteroCumulo statoEstero in listaStatiEsteri)
                        {
                            statoEstero.PrestazioneEsteraCumulo.IdPensione = datiPensione.Id;
                            if (String.IsNullOrEmpty(statoEstero.PrestazioneEsteraCumulo.MatricolaEstera))
                                statoEstero.PrestazioneEsteraCumulo.MatricolaEstera = statoEstero.PrestazioneEsteraCumulo.MatricolaIstituzione;
                            listaPrestazioniEE.Add(statoEstero.PrestazioneEsteraCumulo);
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
