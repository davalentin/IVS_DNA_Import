using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Configuration;
using INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneFs;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;
using System.Reflection;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneLiquidazioneFs
    {
        #region internal members

        #endregion internal members

        #region public members
        internal static bool PrelevaDomanda(string numDomanda, short sede, short categoria, int certificato, short sedeOperatore, short centroOperativoOperatore, string gruppo, string prodotto, string tipo, bool isRiapertura,
            string siglaCategoria, out AreaPrelievo prelievo, out string errori)
        {
            bool erroreTecnico = false;
            errori = "";
            prelievo = null;
            ServizioLiquidazioneFsClient proxy = new ServizioLiquidazioneFsClient();
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
                    Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(gruppo, prodotto);
                    if (isRiapertura || tipoDomanda == Utility.TipoDomanda.Ricostituzione)
                        prelievo.Richiesta._TipoDomanda = GestionePrelievoTipoDomanda.Ricostituzione;
                    else if (tipoDomanda == Utility.TipoDomanda.Superstiti)
                        prelievo.Richiesta._TipoDomanda = GestionePrelievoTipoDomanda.Reversibilità;
                    else if (tipoDomanda == Utility.TipoDomanda.Ripristino)
                        prelievo.Richiesta._TipoDomanda = GestionePrelievoTipoDomanda.Ripristino;
                    else if (tipoDomanda == Utility.TipoDomanda.RipristinoSuperstiti)
                        prelievo.Richiesta._TipoDomanda = GestionePrelievoTipoDomanda.RipristinoSuperstiti;
                    else if (tipoDomanda == Utility.TipoDomanda.Riliquidazione)
                        prelievo.Richiesta._TipoDomanda = GestionePrelievoTipoDomanda.Riliquidazione;
                    else if (tipoDomanda == Utility.TipoDomanda.RiliquidazioneSuperstiti)
                        prelievo.Richiesta._TipoDomanda = GestionePrelievoTipoDomanda.RiliquidazioneSuperstiti;
                    prelievo.Richiesta._Prodotto = prodotto;
                    prelievo.Richiesta._SiglaCategoria = siglaCategoria;
                    prelievo.Richiesta._Tipo = tipo;
                    prelievo.Richiesta._Gruppo = gruppo;

                    GestioneLogSoap.SalvaLogSoap(prelievo, Utility.Servizio.SrvLiquidazioneFs, Utility.MetodoServizio.PrelevaDomanda, Utility.SOAPLogDirection.IN, numDomanda, guid);

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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio LiquidazioneFs | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio LiquidazioneFs | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio LiquidazioneFs | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio LiquidazioneFs: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
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
                    GestioneLogSoap.SalvaLogSoap(prelievo, Utility.Servizio.SrvLiquidazioneFs, Utility.MetodoServizio.PrelevaDomanda, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
                return true;
            }
        }

        internal static bool EseguiSprenotazione(string numDomanda, short sede, short categoria, int certificato, short sedeOperatore, short centroOperativoOperatore, string gruppo, string prodotto, bool isRiapertura, out string errori)
        {
            bool erroreTecnico = false;
            errori = "";
            AreaPrelievo sprenotazione = null;
            ServizioLiquidazioneFsClient proxy = new ServizioLiquidazioneFsClient();
            string stackTrace = null;
            Guid guid = Guid.NewGuid();

            using (new MethodExecutionTracer())
            {
                try
                {
                    sprenotazione = new AreaPrelievo();
                    sprenotazione.Richiesta = new GestionePrelievoRichiestaPrelievo();
                    sprenotazione.Richiesta._NumDomanda = numDomanda;
                    sprenotazione.Richiesta._Sede = sede;
                    sprenotazione.Richiesta._Categoria = categoria;
                    sprenotazione.Richiesta._Certificato = certificato;
                    sprenotazione.Richiesta._SedeOperatore = sedeOperatore;
                    sprenotazione.Richiesta._CentroOperativoOperatore = centroOperativoOperatore;
                    Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(gruppo, prodotto);
                    if (isRiapertura || tipoDomanda == Utility.TipoDomanda.Ricostituzione)
                        sprenotazione.Richiesta._TipoDomanda = GestionePrelievoTipoDomanda.Ricostituzione;
                    else if (tipoDomanda == Utility.TipoDomanda.Superstiti)
                        sprenotazione.Richiesta._TipoDomanda = GestionePrelievoTipoDomanda.Reversibilità;

                    GestioneLogSoap.SalvaLogSoap(sprenotazione, Utility.Servizio.SrvLiquidazioneFs, Utility.MetodoServizio.EseguiSprenotazione, Utility.SOAPLogDirection.IN, numDomanda, guid);

                    AreaEsito Esito = proxy.EseguiSprenotazione(sprenotazione);

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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio LiquidazioneFs | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio LiquidazioneFs | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio LiquidazioneFs | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio LiquidazioneFs: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante la sprenotazione dei dati della pensione";
                        string parametri = string.Format("Numero domanda: {0}; Sede: {1}; Categoria: {2}; Certificato: {3}: Sede operatore: {4}; Centro operativo operatore: {5}; Gruppo: {6}; Prodotto: {7}: E' una riapertura: {8}",
                            numDomanda, sede, categoria, certificato, sedeOperatore, centroOperativoOperatore, gruppo, prodotto, isRiapertura ? "SI" : "NO");
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(sprenotazione, Utility.Servizio.SrvLiquidazioneFs, Utility.MetodoServizio.EseguiSprenotazione, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
                return true;
            }
        }
        #endregion public members
    }
}


