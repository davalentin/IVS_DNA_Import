using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterAziendeESOTRA
    {
        /// <summary>
        /// CaricaArea, riempie area con dati dal service e valorizza area esito
        /// </summary>
        /// <param name="azESOTRA"></param>
        public void CaricaAreaAziendeESOTRA(IAziendeESOTRA azESOTRA)
        {
            azESOTRA.HasError = false;
            azESOTRA.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeESOTRA azV29 = null;
                AreaEsito esito = objWS.GetAllAziendeESOTRA(out azV29);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azESOTRA.HasError = true;
                    azESOTRA.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    azESOTRA.AziendeESOTRA = azV29;
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
                azESOTRA.HasError = true;
                azESOTRA.ErrorMessage = "Errore nel recupero delle Aziende ESOTRA";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaAziendeESOTRA(IAziendeESOTRA azESOTRA)
        {
            azESOTRA.HasError = false;
            azESOTRA.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeESOTRA av29 = azESOTRA.AziendeESOTRA;

                AreaEsito esito = objWS.EliminaAziendeESOTRA(ref av29);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azESOTRA.HasError = true;
                    azESOTRA.ErrorMessage = esito.Messaggio;
                }

                azESOTRA.AziendeESOTRA = av29;
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
                azESOTRA.HasError = true;
                azESOTRA.ErrorMessage = "Errore nell'eliminazione delle Aziende ESOTRA";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void InserisciAziendeESOTRA(IAziendeESOTRA interfAz)
        {
            interfAz.HasError = false;
            interfAz.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeESOTRA azV29 = interfAz.AziendeESOTRA;
                AreaEsito esito = objWS.SalvaAziendeESOTRA(ref azV29);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    interfAz.HasError = true;
                    interfAz.ErrorMessage = esito.Messaggio;
                }

                interfAz.AziendeESOTRA = azV29;
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
                interfAz.ErrorMessage = "Errore nell'inserimento dell'azienda ESOTRA";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
