using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterAziendeESOPMI
    {
        #region public members
        public void CaricaAreaBancheAziende(IBancheFideiussioneESOPMI bncFideius)
        {
            bncFideius.HasError = false;
            bncFideius.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaBancaFideiussioneESOPMI bncF = null;
                AreaEsito esito = objWS.GetAllBancheFideiussioneESOPMI(out bncF);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    bncFideius.HasError = true;
                    bncFideius.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    bncFideius.AziendeESOPMI = bncF;
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
                bncFideius.HasError = true;
                bncFideius.ErrorMessage = "Errore nel recupero delle Banche Fideiussione ESOPMI";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void InserisciBancheFideiussione(IBancheFideiussioneESOPMI bncFideius)
        {
            bncFideius.HasError = false;
            bncFideius.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaBancaFideiussioneESOPMI bncf = bncFideius.AziendeESOPMI;
                AreaEsito esito = objWS.SalvaBancheFideiussioneESOPMI(ref bncf);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    bncFideius.HasError = true;
                    bncFideius.ErrorMessage = esito.Messaggio;
                }
                else
                    bncFideius.AziendeESOPMI = bncf;
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
                bncFideius.HasError = true;
                bncFideius.ErrorMessage = "Errore nell'inserimento delle Banche Fideiussione ESOPMI";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaBancheFideiussione(IBancheFideiussioneESOPMI bncFideius)
        {
            bncFideius.HasError = false;
            bncFideius.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaBancaFideiussioneESOPMI bncf = bncFideius.AziendeESOPMI;
                AreaEsito esito = objWS.EliminaBancheFideiussioneESOPMI(ref bncf);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    bncFideius.HasError = true;
                    bncFideius.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    bncFideius.AziendeESOPMI = bncf;
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
                bncFideius.HasError = true;
                bncFideius.ErrorMessage = "Errore nell'eliminazione delle Banche Fideiussione ESOPMI";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void InserisciAziende(IBancheFideiussioneESOPMI aziendFideius)
        {
            aziendFideius.HasError = false;
            aziendFideius.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaBancaFideiussioneESOPMI azFid = aziendFideius.AziendeESOPMI;
                AreaEsito esito = objWS.SalvaAziendaESOPMI(ref azFid);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    aziendFideius.HasError = true;
                    aziendFideius.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    aziendFideius.AziendeESOPMI = azFid;
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
                aziendFideius.HasError = true;
                aziendFideius.ErrorMessage = "Errore nell'inserimento delle Aziende ESOPMI";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void InserisciAziendeGGmmAAAA(IBancheFideiussioneESOPMI aziendaGmA)
        {
            aziendaGmA.HasError = false;
            aziendaGmA.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                AreaBancaFideiussioneESOPMI azGmA = aziendaGmA.AziendeESOPMI;
                AreaEsito esito = objWS.SalvaAziendaESOPMIGGmmAAAA(ref azGmA);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    aziendaGmA.HasError = true;
                    aziendaGmA.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    aziendaGmA.AziendeESOPMI = azGmA;
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
                aziendaGmA.ErrorMessage = "Errore nell'inserimento delle Aziende ESOPMI";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaAziendeGGmmAAAA(IBancheFideiussioneESOPMI aziendaGmA)
        {
            aziendaGmA.HasError = false;
            aziendaGmA.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaBancaFideiussioneESOPMI azGmA = aziendaGmA.AziendeESOPMI;
                AreaEsito esito = objWS.EliminaAziendaESOPMIGGmmAAAA(ref azGmA);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    aziendaGmA.HasError = true;
                    aziendaGmA.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    aziendaGmA.AziendeESOPMI = azGmA;
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
                aziendaGmA.ErrorMessage = "Errore nell'eliminazione delle Aziende ESOPMI con Scadenza Assegno in formato giorno/mese/anno";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        #endregion public members
    }
}