using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterAziendeEditoriali
    {
        #region public members

        public void CaricaAreaAziendeEditoriali(IAziendeEditoriali azEditoriali)
        {
            azEditoriali.HasError = false;
            azEditoriali.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeEditoriali aAzEd = null;
                AreaEsito esito = objWS.GetAllAziendeEditoriali(out aAzEd);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azEditoriali.HasError = true;
                    azEditoriali.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    azEditoriali.AziendeEditoriali = aAzEd;
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
                azEditoriali.HasError = true;
                azEditoriali.ErrorMessage = "Errore nel recupero delle Aziende Editoriali";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void InserisciAnagraficaAccordi(IAziendeEditoriali azEditoriali)
        {
            azEditoriali.HasError = false;
            azEditoriali.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeEditoriali aAzEd = azEditoriali.AziendeEditoriali;
                AreaEsito esito = objWS.SalvaAnagraficaAccordi(ref aAzEd);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azEditoriali.HasError = true;
                    azEditoriali.ErrorMessage = esito.Messaggio;
                }
                else
                    azEditoriali.AziendeEditoriali = aAzEd;
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
                azEditoriali.HasError = true;
                azEditoriali.ErrorMessage = "Errore nell'inserimento delle Anagrafica Accordi";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaAnagraficaAccordi(IAziendeEditoriali azEditoriali)
        {
            azEditoriali.HasError = false;
            azEditoriali.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeEditoriali aAzEd = azEditoriali.AziendeEditoriali;
                AreaEsito esito = objWS.EliminaAnagraficaAccordi(ref aAzEd);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azEditoriali.HasError = true;
                    azEditoriali.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    azEditoriali.AziendeEditoriali = aAzEd;
                    //FG - Se ho un messaggio lo mostro a video anche in caso di OK
                    if (esito.Messaggio != null && esito.Messaggio != string.Empty)
                        azEditoriali.ErrorMessage = esito.Messaggio;
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
                azEditoriali.HasError = true;
                azEditoriali.ErrorMessage = "Errore nell'eliminazione delle Anagrafica Accordi";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void InserisciAnagraficaAziende(IAziendeEditoriali azEditoriali)
        {
            azEditoriali.HasError = false;
            azEditoriali.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeEditoriali aAzEd = azEditoriali.AziendeEditoriali;
                AreaEsito esito = objWS.SalvaAnagraficaAziende(ref aAzEd);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azEditoriali.HasError = true;
                    azEditoriali.ErrorMessage = esito.Messaggio;
                }
                else
                    azEditoriali.AziendeEditoriali = aAzEd;
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
                azEditoriali.HasError = true;
                azEditoriali.ErrorMessage = "Errore nell'inserimento delle Anagrafica Aziende";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaAnagraficaAziende(IAziendeEditoriali azEditoriali)
        {
            azEditoriali.HasError = false;
            azEditoriali.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeEditoriali aAzEd = azEditoriali.AziendeEditoriali;
                AreaEsito esito = objWS.EliminaAnagraficaAziende(ref aAzEd);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azEditoriali.HasError = true;
                    azEditoriali.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    azEditoriali.AziendeEditoriali = aAzEd;
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
                azEditoriali.HasError = true;
                azEditoriali.ErrorMessage = "Errore nell'eliminazione delle Anagrafica Aziende";
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