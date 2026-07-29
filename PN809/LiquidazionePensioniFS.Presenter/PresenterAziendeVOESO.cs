using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterAziendeVOESO
    {
        /// <summary>
        /// CaricaArea, riempie area con dati dal service e valorizza area esito
        /// </summary>
        /// <param name="azVOESO"></param>
        public void CaricaAreaAziendeVOESO(string tipoAzienda, IAziendeVOESO azVOESO)
        {
            azVOESO.HasError = false;
            azVOESO.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeVOESO aAzVOESO = null;
                AreaEsito esito = objWS.GetAllAziendeVOESO(out aAzVOESO, tipoAzienda);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azVOESO.HasError = true;
                    azVOESO.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    azVOESO.AziendeVOESO = aAzVOESO;
                }
            }

            catch (FaultException<DNA.Services.FaultContract.DnaApplicationFaultContract>)
            {
                throw;
            }
            catch (FaultException<DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
            {
                throw;
            }
            catch (FaultException<DNA.Services.FaultContract.DnaSecurityFaultContract>)
            {
                throw;
            }
            catch (EndpointNotFoundException)
            {
                throw;
            }
            catch (CommunicationException)
            {
                throw;
            }
            catch (Exception Ex)
            {
                azVOESO.HasError = true;
                azVOESO.ErrorMessage = "Errore nel recupero delle Aziende VOESO";
                DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaAziendeVOESO(string tipoAzienda, IAziendeVOESO azVOESO)
        {
            azVOESO.HasError = false;
            azVOESO.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeVOESO aAzVOESO = azVOESO.AziendeVOESO;

                AreaEsito esito = objWS.EliminaAziendeVOESO(tipoAzienda, ref aAzVOESO);
                
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azVOESO.HasError = true;
                    azVOESO.ErrorMessage = esito.Messaggio;
                }

                azVOESO.AziendeVOESO = aAzVOESO;

            }
            catch (FaultException<DNA.Services.FaultContract.DnaApplicationFaultContract>)
            {
                throw;
            }
            catch (FaultException<DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
            {
                throw;
            }
            catch (FaultException<DNA.Services.FaultContract.DnaSecurityFaultContract>)
            {
                throw;
            }
            catch (EndpointNotFoundException)
            {
                throw;
            }
            catch (CommunicationException)
            {
                throw;
            }
            catch (Exception Ex)
            {
                azVOESO.HasError = true;
                azVOESO.ErrorMessage = "Errore nell'eliminazione delle Aziende VOESO";
                DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void InserisciAziendeVOESO(string tipoAzienda, IAziendeVOESO interfAz)
        {
            interfAz.HasError = false;
            interfAz.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeVOESO aAzVOESO = interfAz.AziendeVOESO;
                AreaEsito esito = objWS.SalvaAziendeVOESO(tipoAzienda, ref aAzVOESO);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    interfAz.HasError = true;
                    interfAz.ErrorMessage = esito.Messaggio;
                }

                interfAz.AziendeVOESO = aAzVOESO;
            }
            catch (FaultException<DNA.Services.FaultContract.DnaApplicationFaultContract>)
            {
                throw;
            }
            catch (FaultException<DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
            {
                throw;
            }
            catch (FaultException<DNA.Services.FaultContract.DnaSecurityFaultContract>)
            {
                throw;
            }
            catch (EndpointNotFoundException)
            {
                throw;
            }
            catch (CommunicationException)
            {
                throw;
            }
            catch (Exception Ex)
            {
                interfAz.HasError = true;
                interfAz.ErrorMessage = "Errore nell'inserimento dell'azienda VOESO";
                DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
