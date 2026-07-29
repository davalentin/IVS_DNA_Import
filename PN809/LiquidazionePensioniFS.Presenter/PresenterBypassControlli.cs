using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterBypassControlli
    {
        public void CaricaBypassControlli(IBypassControlli bypassControlli)
        {
            bypassControlli.HasError = false;
            bypassControlli.ErrorMessage = string.Empty;

            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaBypassControllo bypass = null;
                AreaEsito esito = objWS.GetAllBypassControllo(out bypass, bypassControlli.tipoAppRuolo);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    bypassControlli.HasError = true;
                    bypassControlli.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    bypassControlli.BypassControllo = bypass;
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
                bypassControlli.HasError = true;
                bypassControlli.ErrorMessage = "Errore nel recupero dei bypass dei controlli";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaBypassControlli(IBypassControlli bypassControlli)
        {
            bypassControlli.ErrorMessage = string.Empty;
            bypassControlli.HasError = false;

            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                if (bypassControlli != null && bypassControlli.datiBypassControllo != null)
                {
                    AreaEsito esito = objWS.DeleteBypassControllo(bypassControlli.datiBypassControllo.Id);

                    if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                    {
                        bypassControlli.ErrorMessage = esito.Messaggio;
                        bypassControlli.HasError = true;
                    }
                }
                else
                {
                    bypassControlli.ErrorMessage = "Nessun record da eliminare";
                    bypassControlli.HasError = true;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterBypassControlli, Errore nel metodo EliminaBypassControlli");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaBypassControlli(IBypassControlli bypassControlli)
        {
            bypassControlli.ErrorMessage = string.Empty;
            bypassControlli.HasError = false;

            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                if (bypassControlli != null && bypassControlli.datiBypassControllo != null)
                {
                    AreaEsito esito = objWS.StoreBypassControllo(bypassControlli.tipoAppRuolo, bypassControlli.datiBypassControllo);

                    if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                    {
                        bypassControlli.ErrorMessage = esito.Messaggio;
                        bypassControlli.HasError = true;
                    }
                }
                else
                {
                    bypassControlli.ErrorMessage = "Nessun record da salvare";
                    bypassControlli.HasError = true;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterBypassControlli, Errore nel metodo SalvaBypassControlli");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
