using System;
using System.ServiceModel;
using INPS.Pensioni.Liquidazione.ServiceReferences.ArcaMan;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;
using System.Configuration;
using System.Reflection;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneArcaMan
    {
        #region internal members
        internal static bool GetCodiceSoggettoByArcaMan(string codiceFiscale, string numDomanda, out int codiceSoggetto, out string errori)
        {
            bool erroreTecnico = false;
            codiceSoggetto = 0;
            errori = string.Empty;
            Guid guid = Guid.NewGuid();
            ArcaManWSClient proxy = new ArcaManWSClient();
            Risposta response = null;
            string stackTrace = null;
            using (new MethodExecutionTracer())
            {
                try
                {
                    InserimentoGestione request = new InserimentoGestione();
                    request.Sicurezza = ValorizzaAreaSicurezza();
                    request.ChiaveGestionale = ValorizzaChiaveGestionale();
                    request.Ricerca = new tF001();
                    request.Ricerca.CodicePersona = codiceFiscale;
                    request.Ricerca.TipoChiavePresente = new tElenco();
                    request.Ricerca.TipoChiavePresente.Codice = "02";

                    GestioneLogSoap.SalvaLogSoap(request, Utility.Servizio.SrvArcaMan, Utility.MetodoServizio.RichiestaInserimentoGestione, Utility.SOAPLogDirection.IN, numDomanda, guid, codiceFiscale);
                    response = proxy.RichiestaInserimentoGestione(request);
                    if (response != null && response.Sicurezza != null &&
                        response.Sicurezza.Esito != null && response.Sicurezza.Esito.Codice != "WS-OK")
                    {
                        errori = string.Format("Errore tecnico nel recupero del codice soggetto per il codice fiscale {0}: {1}", codiceFiscale, response.Sicurezza.Esito.Descrizione);
                        return true;
                    }
                    else if (response.ChiaveGestionale != null && response.ChiaveGestionale.Length > 0)
                    {
                        int.TryParse(response.ChiaveGestionale[0].Chiave, out codiceSoggetto);
                        return true;
                    }
                    else
                    {
                        errori = "Codice Soggetto non presente";
                        return true;
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio ArcaMan | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio ARCA | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio ArcaMan | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio ArcaMan: {0}", Utility.GetMessageFromException(Ex));
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
                        errori = string.Format("Errore tecnico nel recupero del codice soggetto per il codice fiscale {0}", codiceFiscale);
                        string parametri = string.Format("Codice Fiscale: {0}", codiceFiscale);
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(response, Utility.Servizio.SrvArcaMan, Utility.MetodoServizio.RichiestaInserimentoGestione, Utility.SOAPLogDirection.OUT, numDomanda, guid, codiceFiscale);
                    Utility.CloseClient(proxy);
                }
            }
        }
        #endregion internal members

        #region private members
        private static tS001_inp ValorizzaAreaSicurezza()
        {
            tS001_inp sicurezza = new tS001_inp();
            sicurezza.CodfiscOperatore = null;
            sicurezza.Funzione = ConfigurationManager.AppSettings["SvrArcaMan.Funzione"];
            sicurezza.GestioneApplOrigine = ConfigurationManager.AppSettings["SvrArcaMan.GestioneApplOrigine"];
            sicurezza.IndirizzoTerminale = string.Empty;
            sicurezza.MatricolaINPS = string.Empty;
            sicurezza.PgmApplOrigine = ConfigurationManager.AppSettings["SvrArcaMan.PgmApplOrigine"];
            sicurezza.SedeApplOrigine = ConfigurationManager.AppSettings["SvrArcaMan.SedeApplOrigine"];
            sicurezza.SedeRichiesta = string.Empty;
            sicurezza.TranApplOrigine = ConfigurationManager.AppSettings["SvrArcaMan.TranApplOrigine"];
            return sicurezza;
        }

        private static tK001[] ValorizzaChiaveGestionale()
        {
            tK001[] ListaChiaveGestionale = new tK001[1];
            ListaChiaveGestionale[0] = new tK001();
            ListaChiaveGestionale[0].Archivio = ConfigurationManager.AppSettings["SvrArcaMan.GestioneApplOrigine"];
            ListaChiaveGestionale[0].Progetto = "A";
            ListaChiaveGestionale[0].Sede = ConfigurationManager.AppSettings["SvrArcaMan.SedeApplOrigine"];
            return ListaChiaveGestionale;
        }
        #endregion private members
    }
}
