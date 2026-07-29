using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using INPS.Pensioni.Liquidazione.ServiceReferences.AssegnazioneCertificato;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;
using System.Configuration;
using System.Reflection;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAssegnazioneCertificato
    {
        #region public members
        #endregion public members

        #region internal members
        internal static bool GetCertificato(long numeroDomanda, RichiestaCertificato richiestaCertificato, out int certificato, out string errori)
        {
            bool erroreTecnico = false;
            certificato = 0;
            errori = string.Empty;

            richiestaAssegnazioneCertificato richiesta = new richiestaAssegnazioneCertificato();
            ServicesSoapClient proxy = new ServicesSoapClient();
            Guid guid = Guid.NewGuid();
            esitoAssegnazioneCertificato risposta = new esitoAssegnazioneCertificato();
            string stackTrace = null;
            
            using (new MethodExecutionTracer())
            {
                try
                {
                    richiesta.codiceSede = richiestaCertificato.CodiceSede.ToString().PadLeft(4,'0');
                    richiesta.categoria = richiestaCertificato.SiglaCategoria;
                    try
                    {
                        if (ConfigurationManager.AppSettings.AllKeys.Contains("AssegnazioneCertificato-UserID"))
                        {
                            proxy.ClientCredentials.Windows.ClientCredential.UserName = ConfigurationManager.AppSettings["AssegnazioneCertificato-UserID"];
                            proxy.ClientCredentials.Windows.ClientCredential.Password = ConfigurationManager.AppSettings["AssegnazioneCertificato-Password"];
                            proxy.ClientCredentials.Windows.ClientCredential.Domain = ConfigurationManager.AppSettings["AssegnazioneCertificato-Domain"];
                            proxy.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
                        }
                    }
                    catch (Exception)
                    {
                        errori = "Utenze mancanti per il consumo del servizio AssegnazioneCertificato";
                    }

                    GestioneLogSoap.SalvaLogSoap(richiesta, Utility.Servizio.SrvAssegnazioneCertificato, Utility.MetodoServizio.AssegnazioneCertificato, Utility.SOAPLogDirection.IN, numeroDomanda.ToString(), guid);

                    risposta = proxy.AssegnazioneCertificato(richiesta);
                    if (risposta != null && risposta.codiceRisposta == "1" && string.IsNullOrEmpty(risposta.descrErrore))
                    {
                        int res = 0;
                        int.TryParse(risposta.certificato, out res);
                        certificato = res;
                    }
                    else
                    {
                        errori = string.Format("Errore nell'assegnazione del certificato per la categoria {0} su sede {1}: ", richiesta.categoria, richiesta.codiceSede) + risposta.descrErrore;
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio AssegnazioneCertificato | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio AssegnazioneCertificato | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio AssegnazioneCertificato | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio AssegnazioneCertificato: {0}", Utility.GetMessageFromException(Ex));
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
                        errori = string.Format("Errore nell'assegnazione del certificato per la categoria {0} su sede {1}",
                            richiesta != null ? richiesta.categoria : null,
                            richiesta != null ? richiesta.codiceSede : null);
                        string parametri = string.Format("Categoria: {0}; Sede: {1}", 
                            richiesta != null ? richiesta.categoria : null,
                            richiesta != null ? richiesta.codiceSede : null);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(risposta, Utility.Servizio.SrvAssegnazioneCertificato, Utility.MetodoServizio.AssegnazioneCertificato, Utility.SOAPLogDirection.OUT, numeroDomanda.ToString(), guid);
                    Utility.CloseClient(proxy);
                }
            }
            return true;
        }
        #endregion internal members

        #region nested class
        public class RichiestaCertificato
        {
            #region private properties
            private short _CodiceSede;
            private string _SiglaCategoria;
            #endregion private properties

            #region public properties
            public short CodiceSede { get { return _CodiceSede; } set { _CodiceSede = value; } }
            public string SiglaCategoria { get { return _SiglaCategoria; } set { _SiglaCategoria = value; } }
            #endregion public properties
        }
        #endregion nested class
    }
}

