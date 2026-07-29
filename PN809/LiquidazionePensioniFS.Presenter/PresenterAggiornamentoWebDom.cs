using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterAggiornamentoWebDom
    {
        public void GetAggiornamentoWebDom(IAggiornamentoWebDom aggiornamentoWebDom)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaAggiornamentoWebDom areaAggiornamentoWebDom = null;
            try
            {
                AreaEsito esito = objWS.GetAreaAggiornamentoWebDom(out areaAggiornamentoWebDom, aggiornamentoWebDom.TipoApp.GetValueOrDefault());
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    aggiornamentoWebDom.HasError = true;
                    aggiornamentoWebDom.ErrorMessage = esito.Messaggio;
                    return;
                }
                aggiornamentoWebDom.areaAggiornamentoWebDom = areaAggiornamentoWebDom;
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
                throw new INPS.DNA.DnaApplicationException("PresenterAggiornamentoWebDom: errore nel metodo GetAggiornamentoWebDom: ", Ex);
            }
            finally
            {
                try
                {
                    if (objWS.State != CommunicationState.Closed && objWS.State != CommunicationState.Faulted)
                    {
                        objWS.Close(); // may throw exception while closing
                    }
                    else
                    {
                        objWS.Abort();
                    }
                }
                catch (CommunicationException)
                {
                    objWS.Abort();
                }
                catch (Exception)
                { }
            }
        }

        public void ElaboraAggiornamentoWebDom(IAggiornamentoWebDom aggiornamentoWebDom)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                objWS.Endpoint.Binding.CloseTimeout = new TimeSpan(0, 1, 0);
                objWS.Endpoint.Binding.OpenTimeout = new TimeSpan(0, 1, 0);
                objWS.Endpoint.Binding.SendTimeout = new TimeSpan(0, 1, 0);
                objWS.Endpoint.Binding.ReceiveTimeout = new TimeSpan(0, 1, 0);
                objWS.ElaboraAggiornamentoWebDom(aggiornamentoWebDom.TipoApp.GetValueOrDefault());
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
            catch (TimeoutException)
            {
                aggiornamentoWebDom.HasError = true;
            }
            catch (System.ServiceModel.CommunicationException ex)
            {
                if (ex.InnerException.Message.Contains("timeout"))
                    aggiornamentoWebDom.HasError = true;
                else
                    throw;
            }
            catch (Exception Ex)
            {
                throw new INPS.DNA.DnaApplicationException("PresenterAggiornamentoWebDom: errore nel metodo ElaboraAggiornamentoWebDom: ", Ex);
            }
            finally
            {
                try
                {
                    if (objWS.State != CommunicationState.Closed && objWS.State != CommunicationState.Faulted)
                    {
                        objWS.Close(); // may throw exception while closing
                    }
                    else
                    {
                        objWS.Abort();
                    }
                }
                catch (CommunicationException)
                {
                    objWS.Abort();
                }
                catch (Exception)
                { }
            }
        }

        public void CaricaPdfAggiornamentoWebDom(IAggiornamentoWebDom aggiornamentoWebDom)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaAggiornamentoWebDom areaAggiornamentoWebDom = null;
            try
            {
                AreaEsito esito = objWS.CaricaPdfAggiornamentoWebDom(out areaAggiornamentoWebDom, aggiornamentoWebDom.TipoApp.GetValueOrDefault());
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    aggiornamentoWebDom.HasError = true;
                    aggiornamentoWebDom.ErrorMessage = esito.Messaggio;
                    return;
                }
                aggiornamentoWebDom.areaAggiornamentoWebDom = areaAggiornamentoWebDom;
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
                throw new INPS.DNA.DnaApplicationException("PresenterAggiornamentoWebDom: errore nel metodo CaricaPdfAggiornamentoWebDom: ", Ex);
            }
            finally
            {
                try
                {
                    if (objWS.State != CommunicationState.Closed && objWS.State != CommunicationState.Faulted)
                    {
                        objWS.Close(); // may throw exception while closing
                    }
                    else
                    {
                        objWS.Abort();
                    }
                }
                catch (CommunicationException)
                {
                    objWS.Abort();
                }
                catch (Exception)
                { }
            }
        }
    }
}
