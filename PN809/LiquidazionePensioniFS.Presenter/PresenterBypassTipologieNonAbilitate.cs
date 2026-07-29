using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA;
using INPS.DNA.Logging;
using INPS.DNA.Services;
using INPS.DNA.Services.FaultContract;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterBypassTipologieNonAbilitate 
    {
       
        public void CaricaTipologieNonAbilitate(IBypassTipologieNonAbilitate tipNonAbilitate)
        {
            tipNonAbilitate.HasError = false;
            tipNonAbilitate.ErrorMessage = string.Empty;

            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaCtrlBypassTipologieNonAbilitate tna = null;
                AreaEsito esito = objWS.GetAllCtrlBypassTipologieNonAbilitate(out tna, tipNonAbilitate.tipoAppRuolo);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    tipNonAbilitate.HasError = true;
                    tipNonAbilitate.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    tipNonAbilitate.AreaBypassTipologieNonAbilitate = tna;
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
                tipNonAbilitate.HasError = true;
                tipNonAbilitate.ErrorMessage = "Errore nel recupero delle tipologie non abilitate";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaTipologiaNonAbilitata(IBypassTipologieNonAbilitate tipNonAbilitate)
        {
            tipNonAbilitate.ErrorMessage = string.Empty;
            tipNonAbilitate.HasError = false;

            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                if (tipNonAbilitate != null && tipNonAbilitate.datiBypassTipologiaNonAbilitata != null)
                {
                    AreaEsito esito = objWS.DeleteCtrlTipologieNonAbilitate(tipNonAbilitate.datiBypassTipologiaNonAbilitata);

                    if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                    {
                        tipNonAbilitate.ErrorMessage = esito.Messaggio;
                        tipNonAbilitate.HasError = true;
                    }
                }
                else
                {
                    tipNonAbilitate.ErrorMessage = "Nessun record da eliminare";
                    tipNonAbilitate.HasError = true;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterTipologieNonAbilitate, Errore nel metodo EliminaTipologiaNonAbilitata");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaTipologiaNonAbilitata(IBypassTipologieNonAbilitate tipNonAbilitate)
        {
            tipNonAbilitate.ErrorMessage = string.Empty;
            tipNonAbilitate.HasError = false;

            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                if (tipNonAbilitate != null && tipNonAbilitate.datiBypassTipologiaNonAbilitata != null)
                {
                    AreaEsito esito = objWS.StoreCtrlBypassTipologieNonAbilitate(tipNonAbilitate.datiBypassTipologiaNonAbilitata);

                    if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                    {
                        tipNonAbilitate.ErrorMessage = esito.Messaggio;
                        tipNonAbilitate.HasError = true;
                    }
                }
                else
                {
                    tipNonAbilitate.ErrorMessage = "Nessun record da salvare";
                    tipNonAbilitate.HasError = true;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterTipologieNonAbilitate, Errore nel metodo SalvaTipologiaNonAbilitata");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
