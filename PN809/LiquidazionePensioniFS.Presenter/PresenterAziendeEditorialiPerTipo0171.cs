using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterAziendeEditorialiPerTipo0171
    {
        #region public members

        public void CaricaAreaAziendeEditoriali(IAziendeEditorialiPerTipo0171 azEditoriali)
        {
            azEditoriali.HasError = false;
            azEditoriali.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeEditorialiPerTipo0171 aAzEd = null;
                AreaEsito esito = objWS.GetAllAziendeEditorialiPerTipo0171(out aAzEd);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azEditoriali.HasError = true;
                    azEditoriali.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    azEditoriali.AreaAziendeEditorialiPerTipo0171 = aAzEd;
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
                azEditoriali.ErrorMessage = "Errore nel recupero delle Aziende Editoriali art.1 c. 154 legge 205/2017";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void InserisciAnagraficaAccordi(IAziendeEditorialiPerTipo0171 azEditoriali)
        {
            azEditoriali.HasError = false;
            azEditoriali.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeEditorialiPerTipo0171 aAzEd = azEditoriali.AreaAziendeEditorialiPerTipo0171;
                AreaEsito esito = objWS.SalvaAnagraficaAccordiPerTipo0171(ref aAzEd);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azEditoriali.HasError = true;
                    azEditoriali.ErrorMessage = esito.Messaggio;
                }
                else
                    azEditoriali.AreaAziendeEditorialiPerTipo0171 = aAzEd;
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
                azEditoriali.ErrorMessage = "Errore nell'inserimento delle Anagrafica Accordi per Aziende Editoriali art.1 c. 154 legge 205/2017";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaAnagraficaAccordi(IAziendeEditorialiPerTipo0171 azEditoriali)
        {
            azEditoriali.HasError = false;
            azEditoriali.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeEditorialiPerTipo0171 aAzEd = azEditoriali.AreaAziendeEditorialiPerTipo0171;
                AreaEsito esito = objWS.EliminaAnagraficaAccordiPerTipo0171(ref aAzEd);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azEditoriali.HasError = true;
                    azEditoriali.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    azEditoriali.AreaAziendeEditorialiPerTipo0171 = aAzEd;
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
                azEditoriali.ErrorMessage = "Errore nell'eliminazione delle Anagrafica Accordi per Aziende Editoriali art.1 c. 154 legge 205/2017";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void InserisciAnagraficaAziende(IAziendeEditorialiPerTipo0171 azEditoriali)
        {
            azEditoriali.HasError = false;
            azEditoriali.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeEditorialiPerTipo0171 aAzEd = azEditoriali.AreaAziendeEditorialiPerTipo0171;
                AreaEsito esito = objWS.SalvaAnagraficaAziendePerTipo0171(ref aAzEd);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azEditoriali.HasError = true;
                    azEditoriali.ErrorMessage = esito.Messaggio;
                }
                else
                    azEditoriali.AreaAziendeEditorialiPerTipo0171 = aAzEd;
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
                azEditoriali.ErrorMessage = "Errore nell'inserimento delle Anagrafica Aziende per Aziende Editoriali art.1 c. 154 legge 205/2017";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaAnagraficaAziende(IAziendeEditorialiPerTipo0171 azEditoriali)
        {
            azEditoriali.HasError = false;
            azEditoriali.ErrorMessage = string.Empty;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaAziendeEditorialiPerTipo0171 aAzEd = azEditoriali.AreaAziendeEditorialiPerTipo0171;
                AreaEsito esito = objWS.EliminaAnagraficaAziendePerTipo0171(ref aAzEd);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    azEditoriali.HasError = true;
                    azEditoriali.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    azEditoriali.AreaAziendeEditorialiPerTipo0171 = aAzEd;
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
                azEditoriali.ErrorMessage = "Errore nell'eliminazione delle Anagrafica Aziende per Aziende Editoriali art.1 c. 154 legge 205/2017";
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
