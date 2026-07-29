using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterAziendeVESO29
    {
        /// <summary>
        /// CaricaArea, riempie area con dati dal service e valorizza area esito
        /// </summary>
        /// <param name="azVESO29"></param>
        public void CaricaAreaAziendeVESO29(IAziendeVESO29 azVESO29)
        {
            azVESO29.HasError = false;
            azVESO29.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeVESO29 azV29 = null;
                AreaEsito esito = objWS.GetAllAziendeVESO29(out azV29);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azVESO29.HasError = true;
                    azVESO29.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    azVESO29.AziendeVESO29 = azV29;
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
                azVESO29.HasError = true;
                azVESO29.ErrorMessage = "Errore nel recupero delle Aziende VESO29";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaAziendeVESO29(IAziendeVESO29 azVESO29)
        {
            azVESO29.HasError = false;
            azVESO29.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeVESO29 av29 = azVESO29.AziendeVESO29;

                AreaEsito esito = objWS.EliminaAziendeVESO29(ref av29);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azVESO29.HasError = true;
                    azVESO29.ErrorMessage = esito.Messaggio;
                }

                azVESO29.AziendeVESO29 = av29;
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
                azVESO29.HasError = true;
                azVESO29.ErrorMessage = "Errore nell'eliminazione delle Aziende VESO29";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void InserisciAziendeVESO29(IAziendeVESO29 interfAz)
        {
            interfAz.HasError = false;
            interfAz.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeVESO29 azV29 = interfAz.AziendeVESO29;
                AreaEsito esito = objWS.SalvaAziendeVESO29(ref azV29);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    interfAz.HasError = true;
                    interfAz.ErrorMessage = esito.Messaggio;
                }

                interfAz.AziendeVESO29 = azV29;
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
                interfAz.ErrorMessage = "Errore nell'inserimento dell'azienda VESO29";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
