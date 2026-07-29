using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using INPS.Pensioni.LiquidazioneCi.ServiceReferences.DelegheSindacali;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;
using System.Configuration;
using System.Reflection;

namespace INPS.Pensioni.LiquidazioneCi
{
    public class GestioneDelegheSindacali
    {
        #region public methods

        public static void GetElencoSindacatiPerCategoria(string IdCategoria, out List<Liquidazione.BLCommon.Entity.Sindacato> elencoSindacati, out string errori)
        {
            errori = string.Empty;
            elencoSindacati = null;
            RispostaElencoSindacati risposta = null;

            GetElencoSindacatiPerCategoria(IdCategoria, out risposta, out errori);
            if (!String.IsNullOrEmpty(errori))
                return;

            elencoSindacati = new List<Liquidazione.BLCommon.Entity.Sindacato>();
            foreach (Sindacato sindacatoWS in risposta.sindacato.ToList())
            {
                Liquidazione.BLCommon.Entity.Sindacato sindacatoBL = new Liquidazione.BLCommon.Entity.Sindacato();
                sindacatoBL.Id = sindacatoWS.sCodice;
                sindacatoBL.Progressivo = sindacatoWS.sProgressivo;
                sindacatoBL.Sigla = sindacatoWS.sSigla;
                sindacatoBL.Stato = (Utility.StatoSindacato)sindacatoWS.sStato;
                sindacatoBL.Descrizione = sindacatoWS.sDescrizione;

                elencoSindacati.Add(sindacatoBL);
            }
        }

        public static bool VerificaCodiceSindacato(string IdCategoria, int certificato, string codiceSindacato, DateTime? decorrenzaPensione, out string errori)
        {
            errori = string.Empty;

            bool isCompatibile = false;
            VerificaSindacato(IdCategoria, certificato, codiceSindacato, decorrenzaPensione, out isCompatibile, out errori);
            if (!String.IsNullOrEmpty(errori))
                return false;
            if (!isCompatibile)
            {
                errori = "Sindacato non compatibile";
                return false;
            }
            return true;
        }

        public static void DecodificaCodiceSindacato(string codeSindacato, out Liquidazione.BLCommon.Entity.Sindacato sindacatoBL, out string errori)
        {
            errori = string.Empty;
            sindacatoBL = null;
            ServiceReferences.DelegheSindacali.RispostaSindacato sindacatoWS = null;
            DecodificaCodiceSindacato(codeSindacato, out sindacatoWS, out errori);

            if (!String.IsNullOrEmpty(errori))
                return;

            sindacatoBL = new Liquidazione.BLCommon.Entity.Sindacato();
            sindacatoBL.Id = sindacatoWS.sindacato.sCodice;
            sindacatoBL.Progressivo = sindacatoWS.sindacato.sProgressivo;
            sindacatoBL.Sigla = sindacatoWS.sindacato.sSigla;
            sindacatoBL.Stato = (Liquidazione.BLCommon.Utility.StatoSindacato)sindacatoWS.sindacato.sStato;
            sindacatoBL.Descrizione = sindacatoWS.sindacato.sDescrizione;
        }

        public static bool VerificaCompatibilita(string IdCategoria, int certificato, DateTime? decorrenzaPensione, string codiceSindacato, out string errori)
        {
            errori = string.Empty;
            DateTime? decorrenzaSindacato = null;
            ElencoSindacatiCompatibili risposta = null;

            GetElencoSindacatiCompatibili(IdCategoria, certificato, decorrenzaPensione, out risposta, out errori);
            if (!String.IsNullOrEmpty(errori))
                return false;

            if (risposta != null && risposta.SindacatiCompatibili != null && risposta.SindacatiCompatibili.Length > 0)
            {
                SindacatoCompatibile s = risposta.SindacatiCompatibili.ToList().Find(x => x.CodiceSindacato == codiceSindacato);
                if (s != null)
                    decorrenzaSindacato = Utility.DataFromString(s.DecorrenzaSindacato, Utility.FormatoData.AAAAmmGG);

                if (Utility.DataSuccessivaA(decorrenzaPensione.GetValueOrDefault(), decorrenzaSindacato.GetValueOrDefault()))
                    return true;
            }

            return false;
        }

        #endregion public methods


        #region private methods

        private static void GetElencoSindacatiPerCategoria(string categoria, out RispostaElencoSindacati risposta, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            risposta = null;
            wsDelegheSindacaliSoapClient proxy = new wsDelegheSindacaliSoapClient();
            Identity identity = new Identity();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    risposta = proxy.ElencoSindacatiPerCategoria(ref identity, ELENCO_CAMPI.CODICE, categoria);
                    if (risposta.esito.sCodiceEsito != "0")
                    {
                        errori = !String.IsNullOrEmpty(risposta.esito.sDescrizioneEsito) ? risposta.esito.sDescrizioneEsito.ToLowerInvariant() : string.Empty;
                        risposta.sindacato = null;
                    }
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di infrastruttura nel consumo del servizio DelegheSindacali, method ElencoSindacatiPerCategoria | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio DelegheSindacali, method ElencoSindacatiPerCategoria | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio DelegheSindacali, method ElencoSindacatiPerCategoria | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio DelegheSindacali, method ElencoSindacatiPerCategoria | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio DelegheSindacali, method ElencoSindacatiPerCategoria: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il recupero dei sindacati";
                        string parametri = string.Format("Categoria: {0}", categoria);
                        GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }
            }
        }

        private static void VerificaSindacato(string categoria, int certificato, string codiceSindacato, DateTime? decorrenzaPensione, out bool isCompatibile, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            isCompatibile = false;
            RispostaCompatibilita risposta = null;
            wsDelegheSindacaliSoapClient proxy = new wsDelegheSindacaliSoapClient();
            Identity identity = new Identity();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    risposta = proxy.VerificaCompatibilita(ref identity, categoria, certificato.ToString().PadLeft(8, '0'), codiceSindacato, decorrenzaPensione.GetValueOrDefault().ToString("yyyyMMdd"));
                    if (risposta.esito.sCodiceEsito != "00")
                    {
                        errori = !String.IsNullOrEmpty(risposta.esito.sDescrizioneEsito) ? risposta.esito.sDescrizioneEsito.ToLowerInvariant() : string.Empty;
                    }
                    if (risposta.Compatibilità == "0")
                        isCompatibile = true;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio DelegheSindacali, method VerificaCompatibilita | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori =string.Format("Puntamento errato al servizio DelegheSindacali, method VerificaCompatibilita | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio DelegheSindacali, method VerificaCompatibilita | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio DelegheSindacali, method VerificaCompatibilita: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante la verifica del sindacato";
                        string parametri = string.Format("Categoria: {0}; Certificato: {1}, Codice Sindacato: {2}; Decorrenza Pensione: {3:dd/MM/yyyy}", categoria, certificato, codiceSindacato, decorrenzaPensione);
                        GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }
            }
        }

        private static void DecodificaCodiceSindacato(string codice, out ServiceReferences.DelegheSindacali.RispostaSindacato Sindacato, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            Sindacato = null;
            wsDelegheSindacaliSoapClient proxy = new wsDelegheSindacaliSoapClient();
            Identity identity = new Identity();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    Sindacato = proxy.DecodificaCodiceSindacato(ref identity, codice);
                    if (Sindacato.esito.sCodiceEsito != "0")
                    {
                        errori = !String.IsNullOrEmpty(Sindacato.esito.sDescrizioneEsito) ? Sindacato.esito.sDescrizioneEsito.ToLowerInvariant() : string.Empty;
                        Sindacato.sindacato = null;
                    }
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio DelegheSindacali, method DecodificaCodiceSindacato | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio DelegheSindacali, method DecodificaCodiceSindacato | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio DelegheSindacali, method DecodificaCodiceSindacato | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio DelegheSindacali, method DecodificaCodiceSindacato: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il recupero della decodifica del sindacato";
                        string parametri = string.Format("Codice sindacato: {0}", codice);
                        GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }
            }
        }

        private static void GetElencoSindacatiCompatibili(string categoria, int certificato, DateTime? decorrenzaPensione, out ElencoSindacatiCompatibili risposta, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            risposta = null;
            wsDelegheSindacaliSoapClient proxy = new wsDelegheSindacaliSoapClient();
            Identity identity = new Identity();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    risposta = proxy.ElencoSindacatiCompatibiliperCategoria(ref identity, categoria, certificato.ToString().PadLeft(8, '0'), decorrenzaPensione.GetValueOrDefault().ToString("yyyyMMdd"));
                    if (risposta.esito.sCodiceEsito != "00")
                    {
                        errori = !String.IsNullOrEmpty(risposta.esito.sDescrizioneEsito) ? risposta.esito.sDescrizioneEsito.ToLowerInvariant() : string.Empty;
                        risposta.SindacatiCompatibili = null;
                    }
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio DelegheSindacali, method ElencoSindacatiCompatibiliperCategoria | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio DelegheSindacali, method ElencoSindacatiCompatibiliperCategoria | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio DelegheSindacali, method ElencoSindacatiCompatibiliperCategoria | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio DelegheSindacali, method ElencoSindacatiCompatibiliperCategoria: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il recupero dei sindacati compatibili";
                        string parametri = string.Format("Categoria: {0}; Certificato: {1}, Decorrenza pensione: {2}", categoria, certificato, decorrenzaPensione);
                        GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }
            }
        }
        #endregion private methods

    }
}

