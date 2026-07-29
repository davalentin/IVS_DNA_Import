using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using INPS.DNA.Logging;
using System.Configuration;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione.ServiceReferences.GeneraCertificati;
using System.Reflection;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneGeneraCertificati
    {
        internal static bool GeneraFascicolo(string numDomanda, string codiceCategoria, string codiceSede, string codiceFiscaleDanteCausa, out FascicoloOutput areaRisposta, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            areaRisposta = null;
            ServiceCertificatiClient proxy = null;
            RichiestaFascicolo request = new RichiestaFascicolo();
            RispostaFascicolo response = null;
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    request.AreaControllo = new Controllo();
                    request.AreaControllo.AppNameChiamante = ConfigurationManager.AppSettings["GENERACERTIFICATI-CODE"] != null ? ConfigurationManager.AppSettings["GENERACERTIFICATI-CODE"] : "";
                    request.AreaRichiesta = new FascicoloInput();
                    request.AreaRichiesta.CodiceCategoria = codiceCategoria;
                    request.AreaRichiesta.CodiceFiscaleDanteCausa = codiceFiscaleDanteCausa;
                    request.AreaRichiesta.CodiceSede = codiceSede;

                    GestioneLogSoap.SalvaLogSoap(request, Utility.Servizio.SrvGeneraCertificati, Utility.MetodoServizio.GeneraFascicolo, Utility.SOAPLogDirection.IN, numDomanda, guid);

                    proxy = new ServiceCertificatiClient();
                    response = proxy.GeneraCodiceFascicolo(request);
                    if (response.AreaEsito != null && response.AreaEsito.RisultatoOperazione == EnumsTipoEsito.KO)
                    {
                        errori = "Errore dal servizio GeneraCertificati: " + response.AreaEsito.Messaggio;
                        return false;
                    }
                    else
                        areaRisposta = response.AreaRisposta;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(exception);
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio GeneraCertificati | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio GeneraCertificati | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio GeneraCertificati | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio GeneraCertificati: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante la generazione del codice fascicolo";
                        string parametri = string.Format("Codice categoria: {0}; Codice Sede: {1}; Codice fiscale Dante Causa: {2}", codiceCategoria, codiceSede, codiceFiscaleDanteCausa);
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(response, Utility.Servizio.SrvGeneraCertificati, Utility.MetodoServizio.GeneraFascicolo, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        internal static bool GeneraCertificato(string numDomanda, CertificatoInput areaInput, out CertificatoOutput areaRisposta, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            areaRisposta = null;
            ServiceCertificatiClient proxy = null;
            RichiestaCertificato request = new RichiestaCertificato();
            RispostaCertificato response = null;
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    request.AreaControllo = new Controllo();
                    request.AreaControllo.AppNameChiamante = ConfigurationManager.AppSettings["GENERACERTIFICATI-CODE"] != null ? ConfigurationManager.AppSettings["GENERACERTIFICATI-CODE"] : "";
                    request.AreaRichiesta = new CertificatoInput();
                    request.AreaRichiesta.SiglaCategoria = areaInput.SiglaCategoria;
                    request.AreaRichiesta.CodiceSede = areaInput.CodiceSede;

                    GestioneLogSoap.SalvaLogSoap(request, Utility.Servizio.SrvGeneraCertificati, Utility.MetodoServizio.GeneraCertificato, Utility.SOAPLogDirection.IN, numDomanda, guid);

                    proxy = new ServiceCertificatiClient();
                    response = proxy.GeneraCertificato(request);
                    if (response.AreaEsito != null && response.AreaEsito.RisultatoOperazione == EnumsTipoEsito.KO)
                    {
                        errori = "Errore dal servizio GeneraCertificati: " + response.AreaEsito.Messaggio;
                        return false;
                    }
                    else
                        areaRisposta = response.AreaRisposta;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(exception);
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio GeneraCertificati | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio GeneraCertificati | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio GeneraCertificati | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio GeneraCertificati: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante la generazione del certificato";
                        string parametri = string.Format("Sigla categoria: {0}; Codice Sede: {1}", 
                            areaInput != null ? areaInput.SiglaCategoria : null, 
                            areaInput != null ? areaInput.CodiceSede : null);
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(response, Utility.Servizio.SrvGeneraCertificati, Utility.MetodoServizio.GeneraCertificato, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }
    }
}
