using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione.ServiceReferences.AMGFelpe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAMGFelpe
    {
        public static bool GetDatiPECO_AMG(GestionePensione.DatiPensione datiPensione, ref csAggiornamentoPECO_Fondi_AMG dati, out string errore)
        {
            errore = string.Empty;
            if (!GetDatiPECO_AMGbyNDomus(datiPensione, ref dati, out errore))
                return false;

            return true;
        }

        private static bool GetDatiPECO_AMGbyNDomus(GestionePensione.DatiPensione datiPensione, ref csAggiornamentoPECO_Fondi_AMG dati, out string errore)
        {
            errore = string.Empty;
            try
            {
                dati = new csAggiornamentoPECO_Fondi_AMG();
                dati.A_Funzione = "L";
                dati.A_Numdomus = Convert.ToString(datiPensione.NDomus);
                Aggiornamento_PECO_Fondi_AMG(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_AMG"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_AMG"], ref dati, out errore);
                if (!String.IsNullOrEmpty(errore))
                {
                    dati = null;
                    return false;
                }
                if (dati.A_Return_Code != 0)
                    dati = null;
                return true;
            }
            catch (Exception ex)
            {
                errore = ex.Message;
                return false;
            }
        }

        private static void Aggiornamento_PECO_Fondi_AMG(string ProgrChiamante, string AppChiamante, ref csAggiornamentoPECO_Fondi_AMG dati, out string errori)
        {
            errori = string.Empty;
            GestionePecoServiceClient proxy = new GestionePecoServiceClient();
            Guid guid = Guid.NewGuid();

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(dati, Utility.Servizio.SrvAMGFelpe, Utility.MetodoServizio.Aggiornamento_PECO_Fondi_AMG, Utility.SOAPLogDirection.IN, dati.A_Numdomus, guid);

                    proxy.Aggiornamento_PECO_Fondi_AMG(ProgrChiamante, AppChiamante, ref dati);
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
                    errori = "Si è verificato un errore di sicurezza nel consumo del servizio AGG_PEC_FS, method Aggiornamento_PECO_FS";
                    INPS.DNA.Logging.Logger.WriteError(errori);
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = "Puntamento errato al servizio AGG_PEC_FS, method Aggiornamento_PECO_FS";
                    INPS.DNA.Logging.Logger.LogException(Ex);
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = "Errore di comunicazione con il servizio AGG_PEC_FS, method Aggiornamento_PECO_FS";
                    INPS.DNA.Logging.Logger.LogException(Ex);
                }
                catch (Exception Ex)
                {
                    errori = "Errore nella chiamata al servizio AGG_PEC_FS, method Aggiornamento_PECO_FS: " + Ex.Message;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                }
                finally
                {
                    GestioneLogSoap.SalvaLogSoap(dati, Utility.Servizio.SrvAMGFelpe, Utility.MetodoServizio.Aggiornamento_PECO_Fondi_AMG, Utility.SOAPLogDirection.OUT, dati.A_Numdomus, guid);

                    Utility.CloseClient(proxy);
                }
            }
        }
    }
}
