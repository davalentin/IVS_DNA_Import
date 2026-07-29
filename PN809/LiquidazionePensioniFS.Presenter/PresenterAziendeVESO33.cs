using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterAziendeVESO33
    {
        /// <summary>
        /// CaricaArea, riempie area con dati dal service e valorizza area esito
        /// </summary>
        /// <param name="azVESO33"></param>
        public void CaricaAreaAziendeVESO33(IAziendeVESO33 azVESO33)
        {
            azVESO33.HasError = false;
            azVESO33.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeVESO33 azV33 = null;
                AreaEsito esito = objWS.GetAllAziendeVESO33(out azV33);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azVESO33.HasError = true;
                    azVESO33.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    azVESO33.AziendeVESO33 = azV33;
                }
            }

            catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract>)
            {
                throw;
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                throw;
            }
            catch (System.ServiceModel.CommunicationException)
            {
                throw;
            }
            catch (Exception Ex)
            {
                azVESO33.HasError = true;
                azVESO33.ErrorMessage = "Errore nel recupero delle Aziende VESO33";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaAziendeVESO33(IAziendeVESO33 azVESO33)
        {
            azVESO33.HasError = false;
            azVESO33.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeVESO33 av33 = azVESO33.AziendeVESO33;

                AreaEsito esito = objWS.EliminaAziendeVESO33(ref av33);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azVESO33.HasError = true;
                    azVESO33.ErrorMessage = esito.Messaggio;
                }

                azVESO33.AziendeVESO33 = av33;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract>)
            {
                throw;
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                throw;
            }
            catch (System.ServiceModel.CommunicationException)
            {
                throw;
            }
            catch (Exception Ex)
            {
                azVESO33.HasError = true;
                azVESO33.ErrorMessage = "Errore nell'eliminazione delle AziendeVESO33";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void InserisciAziendeVESO33(IAziendeVESO33 interfAz)
        {
            interfAz.HasError = false;
            interfAz.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeVESO33 azV33 = interfAz.AziendeVESO33;
                AreaEsito esito = objWS.SalvaAziendeVESO33(ref azV33);
                
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    interfAz.HasError = true;
                    interfAz.ErrorMessage = esito.Messaggio;
                }

                interfAz.AziendeVESO33 = azV33;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract>)
            {
                throw;
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                throw;
            }
            catch (System.ServiceModel.CommunicationException)
            {
                throw;
            }
            catch (Exception Ex)
            {
                interfAz.HasError = true;
                interfAz.ErrorMessage = "Errore nell'inserimento dell'azienda VESO33.";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
