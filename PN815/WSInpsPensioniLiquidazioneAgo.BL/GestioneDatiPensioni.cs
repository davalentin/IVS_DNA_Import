using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Context;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.LiquidazioneAgo.ServiceReferences.DatiPensioni;
using System.ServiceModel;
using System.Reflection;

namespace INPS.Pensioni.LiquidazioneAgo
{
    public class GestioneDatiPensioni
    {
        #region public members

        public static bool GetDatiTGP1ByChiavePensione(long nDomus, string chiavePensione, out DatiTGP1Response risposta, out string errori)
        {
            errori = string.Empty;
            risposta = null;

            try
            {
                DatiTGP1Request input = new DatiTGP1Request();

                input.ChiavePensione = chiavePensione;

                GetDatiTGP1(nDomus.ToString(), input, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK")
                {
                    errori = risposta.Esito.Descrizione;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                errori = "Errore tecnico nel recupero delle informazioni della pensione";
                string messaggio = Utility.GetMessageFromException(ex);
                string parametri = string.Format("Chiave pensione: {0}", chiavePensione);
                GestioneLogGenerico.SalvaLogGenerico(nDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }

        public static bool GetDatiTGP2ByChiavePensione(long nDomus, string chiavePensione, GestionePensione.DatiPensione datiPensione, out List<GestioneCalcolo.DatiCalcoloRetributivo> listaCalcoloRetributivo,
            out List<GestioneCalcolo.DatiCalcoloContributivo> listaCalcoloContributivo, ref GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiAgoCi, out string errori)
        {
            errori = string.Empty;
            DatiTGP2Response risposta = null;
            listaCalcoloRetributivo = null;
            listaCalcoloContributivo = null;

            try
            {
                DatiTGP2Request input = new DatiTGP2Request();

                input.ChiavePensione = chiavePensione;

                GetDatiTGP2(nDomus.ToString(), input, Utility.MetodoServizio.GetDatiTGP2ByChiavePensione, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK")
                {
                    errori = risposta.Esito.Descrizione;
                    return false;
                }

                NormalizzaDatiGP2ToDB(datiPensione, risposta, out listaCalcoloRetributivo, out listaCalcoloContributivo, ref datiGenericiAgoCi);

                return true;
            }
            catch (Exception ex)
            {
                errori = "Errore tecnico nel recupero delle informazioni della pensione";
                string messaggio = Utility.GetMessageFromException(ex);
                string parametri = string.Format("Chiave pensione: {0}", chiavePensione);
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }

        public static bool GetDatiTGP5ByChiavePensione(long nDomus, string chiavePensione, out DatiTGP5Response risposta, out string errori)
        {
            errori = string.Empty;
            risposta = null;

            try
            {
                DatiTGP5Request input = new DatiTGP5Request();

                input.ChiavePensione = chiavePensione;

                GetDatiTGP5(nDomus.ToString(), input, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK")
                {
                    errori = risposta.Esito.Descrizione;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                errori = "Errore tecnico nel recupero delle informazioni della pensione";
                string messaggio = Utility.GetMessageFromException(ex);
                string parametri = string.Format("Chiave pensione: {0}", chiavePensione);
                GestioneLogGenerico.SalvaLogGenerico(nDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }

        public static bool GetDatiTGP6ByChiavePensione(long nDomus, string chiavePensione, out DatiTGP6Response risposta, out string errori)
        {
            errori = string.Empty;
            risposta = null;

            try
            {
                DatiTGP6Request input = new DatiTGP6Request();

                input.ChiavePensione = chiavePensione;

                GetDatiTGP6(nDomus.ToString(), input, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK")
                {
                    errori = risposta.Esito.Descrizione;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                errori = "Errore tecnico nel recupero delle informazioni della pensione";
                string messaggio = Utility.GetMessageFromException(ex);
                string parametri = string.Format("Chiave pensione: {0}", chiavePensione);
                GestioneLogGenerico.SalvaLogGenerico(nDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }

        public static bool GetDatiTGP8ByChiavePensione(long nDomus, string chiavePensione, out DatiTGP8Response risposta, out string errori)
        {
            errori = string.Empty;
            risposta = null;

            try
            {
                DatiTGP8Request input = new DatiTGP8Request();

                input.ChiavePensione = chiavePensione;

                GetDatiTGP8(nDomus.ToString(), input, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK")
                {
                    errori = risposta.Esito.Descrizione;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                errori = "Errore tecnico nel recupero delle informazioni della pensione";
                string messaggio = Utility.GetMessageFromException(ex);
                string parametri = string.Format("Chiave pensione: {0}", chiavePensione);
                GestioneLogGenerico.SalvaLogGenerico(nDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }

        #endregion public members

        #region private members

        private static bool GetDatiTGP1(string numDomanda, DatiTGP1Request datiTGP1Request, out DatiTGP1Response datiTGP1Response, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            datiTGP1Response = null;

            DatiPensioniClient proxy = new DatiPensioniClient();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(datiTGP1Request, Utility.Servizio.SrvDatiPensioni, Utility.MetodoServizio.GetDatiTGP1, Utility.SOAPLogDirection.IN, numDomanda);
                    datiTGP1Response = proxy.GetDatiTGP1(datiTGP1Request);
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio DatiPensioni: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico nel recupero delle informazioni della pensione";
                        string parametri = string.Format("GUID per LogSoap: {0}", guid);
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(datiTGP1Response, Utility.Servizio.SrvDatiPensioni, Utility.MetodoServizio.GetDatiTGP1, Utility.SOAPLogDirection.OUT, numDomanda);
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        private static bool GetDatiTGP2(string numDomanda, DatiTGP2Request datiTGP2Request, Utility.MetodoServizio metodoServizio, out DatiTGP2Response datiTGP2Response, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            datiTGP2Response = null;

            DatiPensioniClient proxy = new DatiPensioniClient();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(datiTGP2Request, Utility.Servizio.SrvDatiPensioni, metodoServizio, Utility.SOAPLogDirection.IN, numDomanda, guid);
                    datiTGP2Response = proxy.GetDatiTGP2(datiTGP2Request);
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio DatiPensioni: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico nel recupero delle informazioni della pensione";
                        string parametri = string.Format("GUID per LogSoap: {0}", guid);
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(datiTGP2Response, Utility.Servizio.SrvDatiPensioni, metodoServizio, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        private static bool GetDatiTGP5(string numDomanda, DatiTGP5Request datiTGP5Request, out DatiTGP5Response datiTGP5Response, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            datiTGP5Response = null;

            DatiPensioniClient proxy = new DatiPensioniClient();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(datiTGP5Request, Utility.Servizio.SrvDatiPensioni, Utility.MetodoServizio.GetDatiTGP5, Utility.SOAPLogDirection.IN, numDomanda);
                    datiTGP5Response = proxy.GetDatiTGP5(datiTGP5Request);
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio DatiPensioni: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico nel recupero delle informazioni della pensione";
                        string parametri = string.Format("GUID per LogSoap: {0}", guid);
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(datiTGP5Response, Utility.Servizio.SrvDatiPensioni, Utility.MetodoServizio.GetDatiTGP5, Utility.SOAPLogDirection.OUT, numDomanda);
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        private static bool GetDatiTGP6(string numDomanda, DatiTGP6Request datiTGP6Request, out DatiTGP6Response datiTGP6Response, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            datiTGP6Response = null;

            DatiPensioniClient proxy = new DatiPensioniClient();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(datiTGP6Request, Utility.Servizio.SrvDatiPensioni, Utility.MetodoServizio.GetDatiTGP6, Utility.SOAPLogDirection.IN, numDomanda);
                    datiTGP6Response = proxy.GetDatiTGP6(datiTGP6Request);
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio DatiPensioni: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico nel recupero delle informazioni della pensione";
                        string parametri = string.Format("GUID per LogSoap: {0}", guid);
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(datiTGP6Response, Utility.Servizio.SrvDatiPensioni, Utility.MetodoServizio.GetDatiTGP6, Utility.SOAPLogDirection.OUT, numDomanda);
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        private static bool GetDatiTGP8(string numDomanda, DatiTGP8Request datiTGP8Request, out DatiTGP8Response datiTGP8Response, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            datiTGP8Response = null;

            DatiPensioniClient proxy = new DatiPensioniClient();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(datiTGP8Request, Utility.Servizio.SrvDatiPensioni, Utility.MetodoServizio.GetDatiTGP8, Utility.SOAPLogDirection.IN, numDomanda);
                    datiTGP8Response = proxy.GetDatiTGP8(datiTGP8Request);
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio DatiPensioni: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico nel recupero delle informazioni della pensione";
                        string parametri = string.Format("GUID per LogSoap: {0}", guid);
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(datiTGP8Response, Utility.Servizio.SrvDatiPensioni, Utility.MetodoServizio.GetDatiTGP8, Utility.SOAPLogDirection.OUT, numDomanda);
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        private static void NormalizzaDatiGP2ToDB(GestionePensione.DatiPensione datiPensione, DatiTGP2Response datiTGP2Response, out List<GestioneCalcolo.DatiCalcoloRetributivo> listaCalcoloRetributivo,
            out List<GestioneCalcolo.DatiCalcoloContributivo> listaCalcoloContributivo, ref GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiAgoCi)
        {
            listaCalcoloRetributivo = new List<GestioneCalcolo.DatiCalcoloRetributivo>();
            listaCalcoloContributivo = new List<GestioneCalcolo.DatiCalcoloContributivo>();

            //ENG - Miglioramento controllo per IOPGI
            List<GestioneDecodifica.CodeGestioneQuotaFondoINPGI> elencoCodeGestioneQuotaFondoINPGI = null;
            GestioneDecodifica.GetCodeGestioneQuotaFondoINPGI(out elencoCodeGestioneQuotaFondoINPGI);

            if (datiTGP2Response != null && datiTGP2Response.ElementoTGP2 != null)
            {
                //Calcolo Retributivo
                if (datiTGP2Response.ElementoTGP2.GP2BC00 != null && datiTGP2Response.ElementoTGP2.GP2BC00.Count() > 0)
                {
                    int meseDecorrenza = 0;
                    int annoDecRetr = 0;
                    short meseDecRetr = 0;

                    try
                    {
                        GP2BC00Type objMeseDecorrenza = datiTGP2Response.ElementoTGP2.GP2BC00.ToList().Find(x => Utility.StringToNullableInt(GetValueDatoGP(x.GP2BC01Z).Substring(4, 2)) < 13);
                        if (objMeseDecorrenza != null)
                            meseDecorrenza = Utility.StringToNullableInt(GetValueDatoGP(objMeseDecorrenza.GP2BC01Z).Substring(4, 2)).GetValueOrDefault();
                    }
                    catch
                    {
                        //Eccezione ignorata
                    }

                    //Necessario la valorizzazione della proprietà CodiceTipoQuota per ex-inpdai. 
                    //Succeviamente verrà impostato a null per tutte le pensioni diverse da ex-inpdai. 
                    List<CtrlDecorrenzaRetrExINPDAI> ctrlExInpdai = null;
                    GestioneCtrlDecorrenzaRetrExINPDAI.GetCtrlDecorrenzaRetrExINPDAI(out ctrlExInpdai);
                    /////////////////////////////////////////////////////////////////////////////////////

                    List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> elencoCodeGestioneCalcoloRetributivo = null;
                    GestioneDecodifica.GetCodeGestioneCalcoloRetributivo(out elencoCodeGestioneCalcoloRetributivo);

                    foreach (GP2BC00Type retr in datiTGP2Response.ElementoTGP2.GP2BC00.ToList().FindAll(x => !(!string.IsNullOrEmpty(GetValueDatoGP(x.GP2BC09)) && GetValueDatoGP(x.GP2BC09).Length > 1 && new List<string> { "X", "Y", "W", "Z" }.Contains(GetValueDatoGP(x.GP2BC09).Substring(1, 1)))))
                    {
                        try
                        {
                            annoDecRetr = Utility.StringToNullableInt(GetValueDatoGP(retr.GP2BC01Z).Substring(0, 4)).GetValueOrDefault();
                            meseDecRetr = Utility.StringToNullableShort(GetValueDatoGP(retr.GP2BC01Z).Substring(4, 2)).GetValueOrDefault();
                        }
                        catch
                        {
                            //Eccezione ignorata
                        }

                        if (Utility.StringToNullableInt(GetValueDatoGP(retr.GP2BC02)).GetValueOrDefault() != 0 ||
                            Utility.StringToNullableDecimalPoint(GetValueDatoGP(retr.GP2BC03E)).GetValueOrDefault() != 0M ||
                            !string.IsNullOrEmpty(GetValueDatoGP(retr.GP2BC09)))
                        {
                            //ENG - Miglioramento controllo per IOPGI
                            if (!Utility.IsDomandaINPGI(datiPensione.SiglaCategoria) || (Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) && elencoCodeGestioneQuotaFondoINPGI != null && !elencoCodeGestioneQuotaFondoINPGI.Exists(x => x.TraduzioneSuGP == GetValueDatoGP(retr.GP2BC09).Trim() && x.TipoQuota == "R")))
                            {
                                GestioneCalcolo.DatiCalcoloRetributivo datiRetr = new GestioneCalcolo.DatiCalcoloRetributivo();

                                char? quota = Utility.StringToNullableChar(GetValueDatoGP(retr.GP2BC0B));
                                if (!quota.HasValue || string.IsNullOrEmpty(quota.Value.ToString().Trim()))
                                {
                                    GetQuotaByDecorrRetr(meseDecRetr, out quota);
                                }
                                datiRetr.QuotePrimeLiquidate = quota;

                                //Necessario per ex-inpdai
                                string codiceTipoQuota;
                                GetCodiceTipoQuotaByDecorrRetr(meseDecRetr, ctrlExInpdai, out codiceTipoQuota);
                                datiRetr.CodiceTipoQuota = codiceTipoQuota;

                                if (quota.HasValue && quota.Value == 'A')
                                {
                                    datiRetr.NSettimaneQuotaA = Utility.StringToNullableInt(GetValueDatoGP(retr.GP2BC02)).GetValueOrDefault();
                                    datiRetr.RMSQuotaA = Utility.StringToNullableDecimalPoint(GetValueDatoGP(retr.GP2BC03E)).GetValueOrDefault();
                                }
                                else if (quota.HasValue && quota.Value == 'B')
                                {
                                    datiRetr.NSettimaneQuotaB = Utility.StringToNullableInt(GetValueDatoGP(retr.GP2BC02)).GetValueOrDefault();
                                    datiRetr.RMSQuotaB = Utility.StringToNullableDecimalPoint(GetValueDatoGP(retr.GP2BC03E)).GetValueOrDefault();
                                }

                                if (!string.IsNullOrEmpty(GetValueDatoGP(retr.GP2BC09)))
                                {
                                    retr.GP2BC09.Valore.Codice = GetValueDatoGP(retr.GP2BC09).Replace("0", " ");

                                    if (elencoCodeGestioneCalcoloRetributivo != null && elencoCodeGestioneCalcoloRetributivo.Count > 0)
                                    {
                                        GestioneDecodifica.CodeGestioneCalcoloRetributivo codeGestioneCalcoloRetributivo = elencoCodeGestioneCalcoloRetributivo.Find(x => x.TraduzioneSuGP == GetValueDatoGP(retr.GP2BC09).Trim() && !x.IsFondo);
                                        if (codeGestioneCalcoloRetributivo != null)
                                            datiRetr.CodiceGestione = codeGestioneCalcoloRetributivo.Id;
                                    }
                                }

                                if (!datiRetr.CodiceGestione.HasValue)
                                {
                                    if (datiPensione.SiglaCategoria.Trim() == "VO" || datiPensione.SiglaCategoria.Trim() == "IO" || datiPensione.SiglaCategoria.Trim() == "SO")
                                    {
                                        if (elencoCodeGestioneCalcoloRetributivo != null && elencoCodeGestioneCalcoloRetributivo.Count > 0)
                                        {
                                            GestioneDecodifica.CodeGestioneCalcoloRetributivo codeGestioneCalcoloRetributivo = elencoCodeGestioneCalcoloRetributivo.Find(x => x.TraduzioneSuGP == "1" && !x.IsFondo);
                                            if (codeGestioneCalcoloRetributivo != null)
                                                datiRetr.CodiceGestione = codeGestioneCalcoloRetributivo.Id;
                                        }
                                    }
                                    else
                                    {
                                        short gestioneApp = meseDecRetr;
                                        if (quota == 'A')
                                            gestioneApp -= 70;
                                        else
                                            gestioneApp -= 60;

                                        if (elencoCodeGestioneCalcoloRetributivo != null && elencoCodeGestioneCalcoloRetributivo.Count > 0)
                                        {
                                            GestioneDecodifica.CodeGestioneCalcoloRetributivo codeGestioneCalcoloRetributivo = elencoCodeGestioneCalcoloRetributivo.Find(x => x.TraduzioneSuGP == gestioneApp.ToString() && !x.IsFondo);
                                            if (codeGestioneCalcoloRetributivo != null)
                                                datiRetr.CodiceGestione = codeGestioneCalcoloRetributivo.Id;
                                        }
                                    }
                                }

                                if (Utility.StringToNullableInt(GetValueDatoGP(retr.GP2BC10)).GetValueOrDefault() != 0)
                                    datiRetr.NSettimane707 = Utility.StringToNullableInt(GetValueDatoGP(retr.GP2BC10)).GetValueOrDefault();

                                if (Utility.StringToNullableDecimalPoint(GetValueDatoGP(retr.GP2BC0D)).GetValueOrDefault() != 0)
                                    datiRetr.PL_Quotar = Utility.StringToNullableDecimalPoint(GetValueDatoGP(retr.GP2BC0D)).GetValueOrDefault();

                                if (Utility.StringToNullableDecimalPoint(GetValueDatoGP(retr.GP2BC0F)).GetValueOrDefault() != 0)
                                    datiRetr.PL_Quotar707 = Utility.StringToNullableDecimalPoint(GetValueDatoGP(retr.GP2BC0F)).GetValueOrDefault();

                                if (annoDecRetr != 0)
                                    datiRetr.DecorrenzaOriginariaPensione = Utility.DataFromInt(annoDecRetr, meseDecorrenza, 1);

                                listaCalcoloRetributivo.Add(datiRetr);
                            }
                        }
                    }
                }

                //Calcolo Contributivo
                if (datiTGP2Response.ElementoTGP2.GP2BB00 != null && datiTGP2Response.ElementoTGP2.GP2BB00.Count() > 0)
                {
                    foreach (GP2BB00Type contr in datiTGP2Response.ElementoTGP2.GP2BB00.ToList().FindAll(x => !(!string.IsNullOrEmpty(GetValueDatoGP(x.GP2BB05N)) && GetValueDatoGP(x.GP2BB05N).Length == 2 && new List<string> { "X", "Y", "W", "Z" }.Contains(GetValueDatoGP(x.GP2BB05N).Substring(1, 1)))))
                    {
                        if (Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB06E)).GetValueOrDefault() != 0M ||
                            Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB07E)).GetValueOrDefault() != 0M ||
                            Utility.StringToNullableInt(GetValueDatoGP(contr.GP2BB08)).GetValueOrDefault() != 0 ||
                            !string.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB05N)) ||
                            Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB09E)).GetValueOrDefault() != 0M)
                        {
                            //ENG - Miglioramento controllo per IOPGI
                            if (!Utility.IsDomandaINPGI(datiPensione.SiglaCategoria) || (Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) && elencoCodeGestioneQuotaFondoINPGI != null && !elencoCodeGestioneQuotaFondoINPGI.Exists(x => x.TraduzioneSuGP == GetValueDatoGP(contr.GP2BB05N).Trim() && x.TipoQuota == "C")))
                            {
                                GestioneCalcolo.DatiCalcoloContributivo datiContr = new GestioneCalcolo.DatiCalcoloContributivo();
                                if ((!(string.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB0B))) && GetValueDatoGP(contr.GP2BB0B) == "D") ||
                                    (!(string.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB0A))) && GetValueDatoGP(contr.GP2BB0A) == "4"))
                                {
                                    datiContr.NSettimaneQuotaDL214 = Utility.StringToNullableInt(GetValueDatoGP(contr.GP2BB08)).GetValueOrDefault();
                                    datiContr.MontanteQuotaDL214 = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB06E)).GetValueOrDefault();
                                    datiContr.ImportoContribTotaleQuotaDL214 = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB07E)).GetValueOrDefault();
                                }
                                else if (!(string.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB0A)) && GetValueDatoGP(contr.GP2BB0A) == "K"))
                                {
                                    if (Utility.StringToNullableInt(GetValueDatoGP(contr.GP2BB08)) > 0)
                                        datiContr.NSettimane = Utility.StringToNullableInt(GetValueDatoGP(contr.GP2BB08)).GetValueOrDefault();
                                    if (Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB06E)) > 0)
                                        datiContr.Montante = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB06E)).GetValueOrDefault();
                                    if (Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB07E)) > 0)
                                        datiContr.ImportoContributivoTotale = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB07E)).GetValueOrDefault();
                                }
                                else
                                {
                                    datiContr.NSettimane = Utility.StringToNullableInt(GetValueDatoGP(contr.GP2BB08)).GetValueOrDefault();
                                    datiContr.Montante = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB06E)).GetValueOrDefault();
                                    datiContr.ImportoContributivoTotale = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB07E)).GetValueOrDefault();
                                }
                                if (!string.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB05N)))
                                {
                                    List<GestioneDecodifica.CodeGestioneCalcoloContributivo> elencoCodeGestioneCalcoloContributivo = null;
                                    GestioneDecodifica.GetCodeGestioneCalcoloContributivo(out elencoCodeGestioneCalcoloContributivo);
                                    if (elencoCodeGestioneCalcoloContributivo != null && elencoCodeGestioneCalcoloContributivo.Count > 0)
                                    {
                                        GestioneDecodifica.CodeGestioneCalcoloContributivo codeGestioneCalcoloContributivo = elencoCodeGestioneCalcoloContributivo.Find(x => x.TraduzioneSuGP.Trim() == GetValueDatoGP(contr.GP2BB05N).Trim() && !x.IsFondo);
                                        if (codeGestioneCalcoloContributivo != null)
                                            datiContr.CodiceGestione = codeGestioneCalcoloContributivo.Id;
                                    }
                                }

                                if (Utility.IsDomandaINPDAI(datiPensione.SiglaCategoria))
                                {
                                    if (datiGenericiAgoCi == null)
                                        datiGenericiAgoCi = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                                    datiGenericiAgoCi.AnzAl95 = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BH01E));
                                }

                                if (GetValueDatoGP(contr.GP2BB04Z) != "0")
                                    datiContr.DecorrenzaCalcoloContibutivo = Utility.DataFromString(GetValueDatoGP(contr.GP2BB04Z) + "01", Utility.FormatoData.AAAAmmGG);

                                if (Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB0D)).GetValueOrDefault() != 0)
                                    datiContr.PL_Quotac = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB0D)).GetValueOrDefault();

                                listaCalcoloContributivo.Add(datiContr);
                            }
                        }
                    }
                }

                if (Utility.IsDomandaINPDAI(datiPensione.SiglaCategoria))
                {
                    if (datiGenericiAgoCi == null)
                        datiGenericiAgoCi = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                    datiGenericiAgoCi.QuotaAl95 = Utility.StringToNullableDecimalPoint(GetValueDatoGP(datiTGP2Response.ElementoTGP2.GP2BL01E));
                }
            }
        }

        private static void GetQuotaByDecorrRetr(short meseDecRetr, out char? quota)
        {
            quota = null;

            if (meseDecRetr == 61 || meseDecRetr == 62 || meseDecRetr == 63 || meseDecRetr == 64 || meseDecRetr == 16 ||
                meseDecRetr == 21 || meseDecRetr == 31 || meseDecRetr == 41 || meseDecRetr == 51 || meseDecRetr == 91 ||
                meseDecRetr == 92 || meseDecRetr == 93 || meseDecRetr == 94)
                quota = 'B';
            else
                quota = 'A';
        }

        private static void GetCodiceTipoQuotaByDecorrRetr(short meseDecRetr, List<CtrlDecorrenzaRetrExINPDAI> listaCtrlDecorrenzaRetrExInpdai, out string codiceTipoQuota)
        {
            codiceTipoQuota = null;

            if (listaCtrlDecorrenzaRetrExInpdai == null || listaCtrlDecorrenzaRetrExInpdai.Count == 0)
                return;

            CtrlDecorrenzaRetrExINPDAI ctrl = null;

            if (meseDecRetr == 1 || meseDecRetr == 2 || meseDecRetr == 3 || meseDecRetr == 4 ||
                meseDecRetr == 5 || meseDecRetr == 6 || meseDecRetr == 7 || meseDecRetr == 8 ||
                meseDecRetr == 9 || meseDecRetr == 10 || meseDecRetr == 11 || meseDecRetr == 12)
                ctrl = listaCtrlDecorrenzaRetrExInpdai.Find(x => x.CodiceDecorrenza == 76);
            else
                ctrl = listaCtrlDecorrenzaRetrExInpdai.Find(x => x.CodiceDecorrenza == meseDecRetr);

            if (ctrl != null)
                codiceTipoQuota = ctrl.TipoQuota;
        }

        private static string GetValueDatoGP(DatoGP datoGP)
        {
            if (datoGP != null && datoGP.Valore != null && !string.IsNullOrEmpty(datoGP.Valore.Codice))
                return datoGP.Valore.Codice;

            return string.Empty;
        }
        #endregion private members
    }
}
