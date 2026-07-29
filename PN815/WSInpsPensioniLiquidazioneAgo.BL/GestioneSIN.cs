using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazioneAgo.ServiceReferences.SIN;
using System.Configuration;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazioneAgo
{
    public class GestioneSIN
    {
        #region public methods
        public static bool GetDatiPeco_INPDAP_NumeroDomus(GestionePensione.DatiPensione datiPensione, ref csTBPECOTOTALE_INPDAP dati, out string errori)
        {
            errori = string.Empty;

            try
            {
                dati = new csTBPECOTOTALE_INPDAP();
                LetturaPeco_INPDAP_NumeroDomus(ConfigurationManager.AppSettings["ChiaveApplicazioneSIN"], ConfigurationManager.AppSettings["ChiaveApplicazioneSIN"], datiPensione.NDomus.ToString(), ref dati, out errori);
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
        #endregion public methods

        #region private methods
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
                }
            }
        }
        #endregion private methods
    }
}
