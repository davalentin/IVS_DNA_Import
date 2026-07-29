using System;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterTrasformazioniAbilitate
    {
        #region public members
        public void CaricaTrasformazioniAbilitate(ITrasformazioniAbilitate traAb)
        {
            traAb.HasError = false;
            traAb.ErrorMessage = string.Empty;

            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaTrasformazioniAbilitate tra = null;
                AreaEsito esito = objWS.GetAllTrasformazioniAbilitate(out tra, traAb.tipoAppRuolo);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    traAb.HasError = true;
                    traAb.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    traAb.TrasformazioniAbilitate = tra;
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
                traAb.HasError = true;
                traAb.ErrorMessage = "Errore nel recupero delle trasformazioni abilitate";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaTrasformazioneAbilitata(ITrasformazioniAbilitate traAb)
        {
            traAb.ErrorMessage = string.Empty;
            traAb.HasError = false;

            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                if (traAb != null && traAb.datiTrasformazioneAbilitata != null)
                {
                    AreaEsito esito = objWS.StoreTrasformazioneAbilitata(traAb.datiTrasformazioneAbilitata);

                    if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                    {
                        traAb.ErrorMessage = esito.Messaggio;
                        traAb.HasError = true;
                    }
                }
                else
                {
                    traAb.ErrorMessage = "Nessun record da salvare";
                    traAb.HasError = true;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterTitolare, Errore nel metodo SalvaTrasformazioneAbilitata");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaTrasformazioniAbilitateSuTutteLeSedi(ITrasformazioniAbilitate traAb)
        {
            traAb.ErrorMessage = string.Empty;
            traAb.HasError = false;

            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                if (traAb != null && traAb.datiTrasformazioneAbilitata != null)
                {
                    AreaEsito esito = objWS.StoreTrasformazioniAbilitateSuTutteLeSedi(traAb.datiTrasformazioneAbilitata);

                    if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                    {
                        traAb.ErrorMessage = esito.Messaggio;
                        traAb.HasError = true;
                    }
                }
                else
                {
                    traAb.ErrorMessage = "Nessun record da salvare";
                    traAb.HasError = true;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterTitolare, Errore nel metodo SalvaTrasformazioniAbilitateSuTutteLeSedi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaTrasformazioneAbilitata(ITrasformazioniAbilitate traAb)
        {
            traAb.ErrorMessage = string.Empty;
            traAb.HasError = false;

            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                if (traAb != null && traAb.datiTrasformazioneAbilitata != null)
                {
                    AreaEsito esito = objWS.DeleteTrasformazioneAbilitata(traAb.datiTrasformazioneAbilitata);

                    if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                    {
                        traAb.ErrorMessage = esito.Messaggio;
                        traAb.HasError = true;
                    }
                }
                else
                {
                    traAb.ErrorMessage = "Nessun record da eliminare";
                    traAb.HasError = true;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterTitolare, Errore nel metodo EliminaTrasformazioneAbilitata");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaTrasformazioniAbilitateSuTutteLeSedi(ITrasformazioniAbilitate traAb)
        {
            traAb.ErrorMessage = string.Empty;
            traAb.HasError = false;

            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                if (traAb != null && traAb.datiTrasformazioneAbilitata != null)
                {
                    AreaEsito esito = objWS.DeleteTrasformazioniAbilitateSuTutteLeSedi(traAb.datiTrasformazioneAbilitata);

                    if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                    {
                        traAb.ErrorMessage = esito.Messaggio;
                        traAb.HasError = true;
                    }
                }
                else
                {
                    traAb.ErrorMessage = "Nessun record da eliminare";
                    traAb.HasError = true;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterTitolare, Errore nel metodo EliminaTrasformazioniAbilitateSuTutteLeSedi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
        #endregion public members
    }
}
