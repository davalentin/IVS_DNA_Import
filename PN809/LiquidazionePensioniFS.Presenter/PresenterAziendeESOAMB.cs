using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterAziendeESOAMB
    {
        /// <summary>
        /// CaricaArea, riempie area con dati dal service e valorizza area esito
        /// </summary>
        /// <param name="azESOAMB"></param>
        public void CaricaAreaAziendeESOAMB(IAziendeESOAMB azESOAMB)
        {
            azESOAMB.HasError = false;
            azESOAMB.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeESOAMB az = null;
                AreaEsito esito = objWS.GetAllAziendeESOAMB(out az);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azESOAMB.HasError = true;
                    azESOAMB.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    azESOAMB.AziendeESOAMB = az;
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
                azESOAMB.HasError = true;
                azESOAMB.ErrorMessage = "Errore nel recupero delle Aziende ESOAMB";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaAziendeESOAMB(IAziendeESOAMB azESOAMB)
        {
            azESOAMB.HasError = false;
            azESOAMB.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeESOAMB az = azESOAMB.AziendeESOAMB;

                AreaEsito esito = objWS.EliminaAziendeESOAMB(ref az);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azESOAMB.HasError = true;
                    azESOAMB.ErrorMessage = esito.Messaggio;
                }

                azESOAMB.AziendeESOAMB = az;
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
                azESOAMB.HasError = true;
                azESOAMB.ErrorMessage = "Errore nell'eliminazione delle Aziende ESOAMB";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void InserisciAziendeESOAMB(IAziendeESOAMB azESOAMB)
        {
            azESOAMB.HasError = false;
            azESOAMB.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeESOAMB az = azESOAMB.AziendeESOAMB;
                AreaEsito esito = objWS.SalvaAziendeESOAMB(ref az);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azESOAMB.HasError = true;
                    azESOAMB.ErrorMessage = esito.Messaggio;
                }

                azESOAMB.AziendeESOAMB = az;
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
                azESOAMB.HasError = true;
                azESOAMB.ErrorMessage = "Errore nell'inserimento dell'azienda ESOAMB";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void InserisciAziendeGGmmAAAA(IAziendeESOAMB aziendaGmA)
        {
            aziendaGmA.HasError = false;
            aziendaGmA.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                AreaAziendeESOAMB azGmA = aziendaGmA.AziendeESOAMB;
                AreaEsito esito = objWS.SalvaAziendaESOAMBGGmmAAAA(ref azGmA);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    aziendaGmA.HasError = true;
                    aziendaGmA.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    aziendaGmA.AziendeESOAMB = azGmA;
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
                aziendaGmA.HasError = true;
                aziendaGmA.ErrorMessage = "Errore nell'inserimento delle Aziende con Scadenza Assegno in formato giorno/mese/anno";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaAziendeGGmmAAAA(IAziendeESOAMB aziendaGmA)
        {
            aziendaGmA.HasError = false;
            aziendaGmA.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeESOAMB azGmA = aziendaGmA.AziendeESOAMB;
                AreaEsito esito = objWS.EliminaAziendaESOAMBGGmmAAAA(ref azGmA);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    aziendaGmA.HasError = true;
                    aziendaGmA.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    aziendaGmA.AziendeESOAMB = azGmA;
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
                aziendaGmA.HasError = true;
                aziendaGmA.ErrorMessage = "Errore nell'eliminazione delle Aziende con Scadenza Assegno in formato giorno/mese/anno";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
