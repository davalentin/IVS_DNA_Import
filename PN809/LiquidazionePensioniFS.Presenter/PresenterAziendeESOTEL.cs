using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterAziendeESOTEL
    {
        /// <summary>
        /// CaricaArea, riempie area con dati dal service e valorizza area esito
        /// </summary>
        /// <param name="azESOTEL"></param>
        public void CaricaAreaAziendeESOTEL(IAziendeESOTEL azESOTEL)
        {
            azESOTEL.HasError = false;
            azESOTEL.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeESOTEL azV29 = null;
                AreaEsito esito = objWS.GetAllAziendeESOTEL(out azV29);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azESOTEL.HasError = true;
                    azESOTEL.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    azESOTEL.AziendeESOTEL = azV29;
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
                azESOTEL.HasError = true;
                azESOTEL.ErrorMessage = "Errore nel recupero delle Aziende ESOTEL";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaAziendeESOTEL(IAziendeESOTEL azESOTEL)
        {
            azESOTEL.HasError = false;
            azESOTEL.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeESOTEL av29 = azESOTEL.AziendeESOTEL;

                AreaEsito esito = objWS.EliminaAziendeESOTEL(ref av29);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azESOTEL.HasError = true;
                    azESOTEL.ErrorMessage = esito.Messaggio;
                }

                azESOTEL.AziendeESOTEL = av29;
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
                azESOTEL.HasError = true;
                azESOTEL.ErrorMessage = "Errore nell'eliminazione delle Aziende ESOTEL";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void InserisciAziendeESOTEL(IAziendeESOTEL interfAz)
        {
            interfAz.HasError = false;
            interfAz.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeESOTEL azV29 = interfAz.AziendeESOTEL;
                AreaEsito esito = objWS.SalvaAziendeESOTEL(ref azV29);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    interfAz.HasError = true;
                    interfAz.ErrorMessage = esito.Messaggio;
                }

                interfAz.AziendeESOTEL = azV29;
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
                interfAz.ErrorMessage = "Errore nell'inserimento dell'azienda ESOTEL";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
