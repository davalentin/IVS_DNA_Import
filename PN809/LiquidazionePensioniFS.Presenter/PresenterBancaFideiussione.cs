using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterBancaFideiussione
    {
        #region public members
        public void CaricaAreaBancheAziende(IBancheFideiussione bncFideius)
        {
            bncFideius.HasError = false;
            bncFideius.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaBancaFideiussione bncF = null;
                AreaEsito esito = objWS.GetAllBancheFideiussione(out bncF);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    bncFideius.HasError = true;
                    bncFideius.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    bncFideius.BancheFideiussione = bncF;
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
                bncFideius.ErrorMessage = "Errore nel recupero delle Banche Fideiussione";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void InserisciBancheFideiussione(IBancheFideiussione bncFideius)
        {
            bncFideius.HasError = false;
            bncFideius.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaBancaFideiussione bncf = bncFideius.BancheFideiussione;
                AreaEsito esito = objWS.SalvaBancheFideiussione(ref bncf);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    bncFideius.HasError = true;
                    bncFideius.ErrorMessage = esito.Messaggio;
                }
                else
                    bncFideius.BancheFideiussione = bncf;
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
                bncFideius.ErrorMessage = "Errore nell'inserimento delle Banche Fideiussione";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaBancheFideiussione(IBancheFideiussione bncFideius)
        {
            bncFideius.HasError = false;
            bncFideius.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaBancaFideiussione bncf = bncFideius.BancheFideiussione;
                AreaEsito esito = objWS.EliminaBancheFideiussione(ref bncf);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    bncFideius.HasError = true;
                    bncFideius.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    bncFideius.BancheFideiussione = bncf;
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
                bncFideius.ErrorMessage = "Errore nell'eliminazione delle Banche Fideiussione";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void InserisciAziende(IBancheFideiussione aziendFideius)
        {
            aziendFideius.HasError = false;
            aziendFideius.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaBancaFideiussione azFid = aziendFideius.BancheFideiussione;
                AreaEsito esito = objWS.SalvaAzienda(ref azFid);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    aziendFideius.HasError = true;
                    aziendFideius.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    aziendFideius.BancheFideiussione = azFid;
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
                aziendFideius.ErrorMessage = "Errore nell'inserimento delle Aziende";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void InserisciAziendeGGmmAAAA(IBancheFideiussione aziendaGmA)
        {
            aziendaGmA.HasError = false;
            aziendaGmA.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try 
            {
                AreaBancaFideiussione azGmA = aziendaGmA.BancheFideiussione;
                AreaEsito esito = objWS.SalvaAziendaGGmmAAAA(ref azGmA);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    aziendaGmA.HasError = true;
                    aziendaGmA.ErrorMessage = esito.Messaggio;
                }
                else 
                {
                    aziendaGmA.BancheFideiussione = azGmA;
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
                aziendaGmA.ErrorMessage = "Errore nell'inserimento delle Aziende";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
        
        public void EliminaAziendeGGmmAAAA(IBancheFideiussione aziendaGmA)
        {
            aziendaGmA.HasError = false;
            aziendaGmA.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaBancaFideiussione azGmA = aziendaGmA.BancheFideiussione;
                AreaEsito esito = objWS.EliminaAziendaGGmmAAAA(ref azGmA);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    aziendaGmA.HasError = true;
                    aziendaGmA.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    aziendaGmA.BancheFideiussione = azGmA;
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

        #endregion public members
    }
}