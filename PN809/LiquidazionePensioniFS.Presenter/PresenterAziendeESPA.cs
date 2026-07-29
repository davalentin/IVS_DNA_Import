using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterAziendeESPA
    {
        #region public members
        public void CaricaAreaBancheAziende(IBancheFideiussioneESPA bncFideius)
        {
            bncFideius.HasError = false;
            bncFideius.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaBancaFideiussioneESPA bncF = null;
                AreaEsito esito = objWS.GetAllBancheFideiussioneESPA(out bncF);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    bncFideius.HasError = true;
                    bncFideius.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    bncFideius.AziendeESPA = bncF;
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
                bncFideius.ErrorMessage = "Errore nel recupero delle Banche Fideiussione ESPA";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void InserisciBancheFideiussione(IBancheFideiussioneESPA bncFideius)
        {
            bncFideius.HasError = false;
            bncFideius.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaBancaFideiussioneESPA bncf = bncFideius.AziendeESPA;
                AreaEsito esito = objWS.SalvaBancheFideiussioneESPA(ref bncf);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    bncFideius.HasError = true;
                    bncFideius.ErrorMessage = esito.Messaggio;
                }
                else
                    bncFideius.AziendeESPA = bncf;
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
                bncFideius.ErrorMessage = "Errore nell'inserimento delle Banche Fideiussione ESPA";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaBancheFideiussione(IBancheFideiussioneESPA bncFideius)
        {
            bncFideius.HasError = false;
            bncFideius.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaBancaFideiussioneESPA bncf = bncFideius.AziendeESPA;
                AreaEsito esito = objWS.EliminaBancheFideiussioneESPA(ref bncf);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    bncFideius.HasError = true;
                    bncFideius.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    bncFideius.AziendeESPA = bncf;
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
                bncFideius.ErrorMessage = "Errore nell'eliminazione delle Banche Fideiussione ESPA";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void InserisciAziende(IBancheFideiussioneESPA aziendFideius)
        {
            aziendFideius.HasError = false;
            aziendFideius.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaBancaFideiussioneESPA azFid = aziendFideius.AziendeESPA;
                AreaEsito esito = objWS.SalvaAziendaESPA(ref azFid);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    aziendFideius.HasError = true;
                    aziendFideius.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    aziendFideius.AziendeESPA = azFid;
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
                aziendFideius.ErrorMessage = "Errore nell'inserimento delle Aziende ESPA";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void InserisciAziendeGGmmAAAA(IBancheFideiussioneESPA aziendaGmA)
        {
            aziendaGmA.HasError = false;
            aziendaGmA.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try 
            {
                AreaBancaFideiussioneESPA azGmA = aziendaGmA.AziendeESPA;
                AreaEsito esito = objWS.SalvaAziendaESPAGGmmAAAA(ref azGmA);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    aziendaGmA.HasError = true;
                    aziendaGmA.ErrorMessage = esito.Messaggio;
                }
                else 
                {
                    aziendaGmA.AziendeESPA = azGmA;
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
                aziendaGmA.ErrorMessage = "Errore nell'inserimento delle Aziende ESPA";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaAziendeGGmmAAAA(IBancheFideiussioneESPA aziendaGmA)
        {
            aziendaGmA.HasError = false;
            aziendaGmA.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaBancaFideiussioneESPA azGmA = aziendaGmA.AziendeESPA;
                AreaEsito esito = objWS.EliminaAziendaESPAGGmmAAAA(ref azGmA);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    aziendaGmA.HasError = true;
                    aziendaGmA.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    aziendaGmA.AziendeESPA = azGmA;
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
                aziendaGmA.ErrorMessage = "Errore nell'eliminazione delle Aziende ESPA con Scadenza Assegno in formato giorno/mese/anno";
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