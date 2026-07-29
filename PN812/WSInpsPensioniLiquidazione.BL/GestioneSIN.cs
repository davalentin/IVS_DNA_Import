using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.ServiceReferences.SIN;
using System.ServiceModel;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Configuration;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneSIN
    {
        #region public methods
        public static bool GetDatiPECO_INPDAP(GestionePensione.DatiPensione datiPensione, string codiceFiscale, Utility.TipoSalvaguardia TipoSalvaguardia, ref csTBPECOTOTALE_INPDAP dati, out Utility.TipoUnicarpe tipoUnicarpe, 
            out string errori)
        {
            errori = string.Empty;
            tipoUnicarpe = Utility.TipoUnicarpe.Not;
            if (!GetDatiPeco_INPDAP_NumeroDomus(datiPensione.NDomus.ToString(), ref dati, out errori))
                return false;

            if (dati != null)
                tipoUnicarpe = Utility.TipoUnicarpe.Automatica;

            if (dati == null && TipoSalvaguardia != Utility.TipoSalvaguardia.Nessuna)
            {
                if (!GetDatiPeco_INPDAP_CodiceFiscale(codiceFiscale, datiPensione.NDomus.ToString(), ref dati, out errori))
                    return false;

                if (dati != null)
                    tipoUnicarpe = Utility.TipoUnicarpe.Manuale;
            }
            return true;
        }
        #endregion public methods

        #region private methods
        private static bool GetDatiPeco_INPDAP_CodiceFiscale(string codiceFiscale, string nDomus, ref csTBPECOTOTALE_INPDAP dati, out string errori)
        {
            errori = string.Empty;

            try
            {
                dati = new csTBPECOTOTALE_INPDAP();
                LetturaPeco_INPDAP_CodiceFiscale(ConfigurationManager.AppSettings["ChiaveApplicazioneSIN"], ConfigurationManager.AppSettings["ChiaveApplicazioneSIN"], codiceFiscale, nDomus, ref dati, out errori);
                if (!String.IsNullOrEmpty(errori))
                {
                    dati = null;
                    return false;
                }
                if (dati.Item_1 != "0")
                    dati = null;
                return true;
            }
            catch (Exception ex)
            {
                errori = ex.Message;
                return false;
            }
        }
        
        private static bool GetDatiPeco_INPDAP_NumeroDomus(string nDomus, ref csTBPECOTOTALE_INPDAP dati, out string errori)
        {
            errori = string.Empty;

            try
            {
                dati = new csTBPECOTOTALE_INPDAP();
                LetturaPeco_INPDAP_NumeroDomus(ConfigurationManager.AppSettings["ChiaveApplicazioneSIN"], ConfigurationManager.AppSettings["ChiaveApplicazioneSIN"], nDomus, ref dati, out errori);
                if (!String.IsNullOrEmpty(errori))
                {
                    dati = null;
                    return false;
                }
                if (dati.Item_1 != "0")
                    dati = null;
                return true;
            }
            catch (Exception ex)
            {
                errori = ex.Message;
                return false;
            }
        }

        private static void LetturaPeco_INPDAP_CodiceFiscale(string progChiamante, string appChiamante, string codiceFiscale, string nDomus, ref csTBPECOTOTALE_INPDAP dati, out string errori)
        {
            errori = string.Empty;

            FelpePerSINServiceClient proxy = new FelpePerSINServiceClient();
            Guid guid = Guid.NewGuid();

            try
            {
                GestioneLogSoap.SalvaLogSoap(dati, Utility.Servizio.SrvSIN, Utility.MetodoServizio.LetturaPECO_INPDAP_CodiceFiscale, Utility.SOAPLogDirection.IN, nDomus, guid);

                string flag_Tabella = "1111111111111111";

                proxy.LetturaPECO_INPDAP_CodiceFiscale(progChiamante, appChiamante, flag_Tabella, codiceFiscale, ref dati);
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
            {
                errori = exception.Message;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract>)
            {
                errori = "Si è verificato un errore di sicurezza nel consumo del servizio SIN, method LetturaPeco_INPDAP_CodiceFiscale";
                INPS.DNA.Logging.Logger.WriteError(errori);
            }
            catch (System.ServiceModel.EndpointNotFoundException Ex)
            {
                errori = "Puntamento errato al servizio SIN, method LetturaPeco_INPDAP_CodiceFiscale";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            catch (System.ServiceModel.CommunicationException Ex)
            {
                errori = "Errore di comunicazione con il servizio SIN, method LetturaPeco_INPDAP_CodiceFiscale";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            catch (Exception Ex)
            {
                errori = "Errore nella chiamata al servizio SIN, method LetturaPeco_INPDAP_CodiceFiscale: " + Ex.Message;
                INPS.DNA.Logging.Logger.WriteError(errori);
            }
            finally
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(dati, Utility.Servizio.SrvSIN, Utility.MetodoServizio.LetturaPECO_INPDAP_CodiceFiscale, Utility.SOAPLogDirection.OUT, nDomus, guid);

                    if (proxy.State != CommunicationState.Closed && proxy.State != CommunicationState.Faulted)
                        proxy.Close();
                    else
                        proxy.Abort();
                }
                catch (CommunicationException)
                {
                    proxy.Abort();
                }
                catch (Exception)
                {
                    //Eccezione ignorata
                }
            }
        }

        private static void LetturaPeco_INPDAP_NumeroDomus(string progChiamante, string appChiamante, string nDomus, ref csTBPECOTOTALE_INPDAP dati, out string errori)
        {
            errori = string.Empty;

            FelpePerSINServiceClient proxy = new FelpePerSINServiceClient();
            Guid guid = Guid.NewGuid();

            try
            {
                GestioneLogSoap.SalvaLogSoap(dati, Utility.Servizio.SrvSIN, Utility.MetodoServizio.LetturaPECO_INPDAP_NumeroDomus, Utility.SOAPLogDirection.IN, nDomus, guid);

                string flag_Tabella = "1111111111111111";

                proxy.LetturaPECO_INPDAP_NumeroDomus(progChiamante, appChiamante, flag_Tabella, nDomus, ref dati);
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
            {
                errori = exception.Message;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract>)
            {
                errori = "Si è verificato un errore di sicurezza nel consumo del servizio SIN, method LetturaPECO_INPDAP_NumeroDomus";
                INPS.DNA.Logging.Logger.WriteError(errori);
            }
            catch (System.ServiceModel.EndpointNotFoundException Ex)
            {
                errori = "Puntamento errato al servizio SIN, method LetturaPECO_INPDAP_NumeroDomus";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            catch (System.ServiceModel.CommunicationException Ex)
            {
                errori = "Errore di comunicazione con il servizio SIN, method LetturaPECO_INPDAP_NumeroDomus";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            catch (Exception Ex)
            {
                errori = "Errore nella chiamata al servizio SIN, method LetturaPECO_INPDAP_NumeroDomus: " + Ex.Message;
                INPS.DNA.Logging.Logger.WriteError(errori);
            }
            finally
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(dati, Utility.Servizio.SrvSIN, Utility.MetodoServizio.LetturaPECO_INPDAP_NumeroDomus, Utility.SOAPLogDirection.OUT, nDomus, guid);

                    if (proxy.State != CommunicationState.Closed && proxy.State != CommunicationState.Faulted)
                        proxy.Close();
                    else
                        proxy.Abort();
                }
                catch (CommunicationException)
                {
                    proxy.Abort();
                }
                catch (Exception)
                {
                    //Eccezione ignorata
                }
            }
        }
        #endregion private methods
    }
}
