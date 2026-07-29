using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterAziendeCredito
    {
        /// <summary>
        /// CaricaArea, riempie area con dati dal service e valorizza area esito
        /// </summary>
        /// <param name="azCredito"></param>
        public void CaricaAreaAziendeCredito(string categoriaAzienda, IAziendeCredito azCredito)
        {
            azCredito.HasError = false;
            azCredito.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeCredito aAzCredito = null;
                AreaEsito esito = objWS.GetAllAziendeCredito(out aAzCredito, categoriaAzienda);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azCredito.HasError = true;
                    azCredito.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    azCredito.AziendeCredito = aAzCredito;
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
                azCredito.HasError = true;
                azCredito.ErrorMessage = "Errore nel recupero delle Aziende Credito";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaAziendeCredito(string categoriaAzienda, IAziendeCredito azCredito)
        {
            azCredito.HasError = false;
            azCredito.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeCredito aAzCredito = azCredito.AziendeCredito;

                AreaEsito esito = objWS.EliminaAziendeCredito(categoriaAzienda, ref aAzCredito);

               

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azCredito.HasError = true;
                    azCredito.ErrorMessage = esito.Messaggio;
                }

                azCredito.AziendeCredito = aAzCredito;
                
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
                azCredito.HasError = true;
                azCredito.ErrorMessage = "Errore nell'eliminazione delle AziendeCredito";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void InserisciAziendeCredito(string categoriaAzienda, IAziendeCredito interfAz)
        {
            interfAz.HasError = false;
            interfAz.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeCredito aAzCredito = interfAz.AziendeCredito;
                AreaEsito esito = objWS.SalvaAziendeCredito(categoriaAzienda, ref aAzCredito);


                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    interfAz.HasError = true;
                    interfAz.ErrorMessage = esito.Messaggio;
                }

                interfAz.AziendeCredito = aAzCredito;
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
                interfAz.ErrorMessage = "Errore nell'inserimento delle Banche Fideiussione";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
