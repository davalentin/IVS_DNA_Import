using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazioneAgo.ServiceReferences.ANF;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.ServiceModel;
using System.Reflection;
using System.Configuration;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace INPS.Pensioni.LiquidazioneAgo
{
    public class GestioneANF
    {
        #region public members
        public static bool RicercaDomandeANFByCodiceFiscale(string numeroDomanda, string codiceFiscale, string matricolaOperatore, out string risposta, out string errori)
        {
            errori = string.Empty;
            risposta = string.Empty;
            Guid guid = Guid.NewGuid();
            string stackTrace = null;
            bool erroreTecnico = false;
            AnfWSClient proxy = new AnfWSClient();
            using (new MethodExecutionTracer())
            {
                try
                {
                    string richiesta = ValorizzaRichiestaRicercaDomande(codiceFiscale, matricolaOperatore);
                    GestioneLogSoap.SalvaLogSoap(richiesta, Utility.Servizio.SrvANF, Utility.MetodoServizio.RicercaDomandeANF_Beneficiario_Asincrona, Utility.SOAPLogDirection.IN, numeroDomanda, guid, codiceFiscale);
                    risposta = proxy.RicercaDomandeANF_Beneficiario_Asincrona(richiesta);
                    Utility.CloseClient(proxy);
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio ANF | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio ANF | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio ANF | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio ANF: {0}", Utility.GetMessageFromException(Ex));
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
                        errori = "Errore tecnico durante il recupero delle informazioni relative all'assegno del nucleo familiare";
                        string parametri = string.Format("Codice Fiscale: {0}", codiceFiscale);
                        long numDomanda = 0;
                        long.TryParse(numeroDomanda, out numDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(risposta, Utility.Servizio.SrvANF, Utility.MetodoServizio.RicercaDomandeANF_Beneficiario_Asincrona, Utility.SOAPLogDirection.OUT, numeroDomanda, guid, codiceFiscale);
                }
            }
            return true;
        }

        public static bool RichiediRispostaById(string numeroDomanda, string codicefiscale, string id, string matricolaOperatore, out string risposta, out string errori)
        {
            errori = string.Empty;
            risposta = string.Empty;
            Guid guid = Guid.NewGuid();
            string stackTrace = null;
            bool erroreTecnico = false;
            AnfWSClient proxy = new AnfWSClient();

            using (new MethodExecutionTracer())
            {
                try
                {                   
                    GestioneLogSoap.SalvaLogSoap(id, Utility.Servizio.SrvANF, Utility.MetodoServizio.RichiediRispostaRicercaAsincrona, Utility.SOAPLogDirection.IN, numeroDomanda, guid, codicefiscale);
                    risposta = proxy.RichiediRispostaRicercaAsincrona(id);
                    Utility.CloseClient(proxy);
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio ANF | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio ANF | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio ANF | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio ANF: {0}", Utility.GetMessageFromException(Ex));
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
                        errori = "Errore tecnico durante il recupero delle informazioni relative all'assegno del nucleo familiare";
                        string parametri = string.Format("Id: {0}", id);
                        long numDomanda = 0;
                        long.TryParse(numeroDomanda, out numDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(risposta, Utility.Servizio.SrvANF, Utility.MetodoServizio.RichiediRispostaRicercaAsincrona, Utility.SOAPLogDirection.OUT, numeroDomanda, guid, codicefiscale);
                }
            }
            return true;
        }
        #endregion public members

        #region private members
        private static string ValorizzaRichiestaRicercaDomande(string codiceFiscale, string matricolaOperatore)
        {
            Richiesta richiesta = new Richiesta();
            richiesta.Operatore = new Richiesta.Operator();
            richiesta.Operatore.CodiceFiscale = codiceFiscale;
            richiesta.Operatore.Matricola = matricolaOperatore;
            richiesta.Operatore.SistemaChiamante = ConfigurationManager.AppSettings["SistemaChiamante"];
            richiesta.ParametriRichiesta = new Richiesta.ParamRichiesta();
            richiesta.ParametriRichiesta.CodiceFiscale = codiceFiscale;
            richiesta.ParametriRichiesta.PeriodoDa = string.Empty;
            richiesta.ParametriRichiesta.PeriodoA = string.Empty;
            richiesta.ParametriRichiesta.DettaglioDatiAnagrafici = "0";
            return Utility.SerializeObject<Richiesta>(richiesta);
        }
        #endregion private members

        public class Richiesta
        {
            public Operator Operatore { get; set; }
            public ParamRichiesta ParametriRichiesta { get; set; }

            public class Operator
            {
                public string SistemaChiamante { get; set; }
                public string CodiceFiscale { get; set; }
                public string Matricola { get; set; }
            }

            public class ParamRichiesta
            {
                public string CodiceFiscale { get; set; }
                public string PeriodoDa { get; set; }
                public string PeriodoA { get; set; }
                public string DettaglioDatiAnagrafici { get; set; }
            }
        }
    }
}
