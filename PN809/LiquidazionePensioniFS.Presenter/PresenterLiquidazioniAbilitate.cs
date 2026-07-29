using System;
using System.ServiceModel;

using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterLiquidazioniAbilitate
    {
        #region public members
        public void CaricaLiquidazioniAbilitate(ILiquidazioniAbilitate liqAb)
        {
            liqAb.HasError = false;
            liqAb.ErrorMessage = string.Empty;

            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaLiquidazioniAbilitate la = null;
                AreaEsito esito = objWS.GetAllLiquidazioniAbilitate(out la, liqAb.tipoAppRuolo);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    liqAb.HasError = true;
                    liqAb.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    liqAb.LiquidazioniAbilitate = la;
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
                liqAb.HasError = true;
                liqAb.ErrorMessage = "Errore nel recupero delle liquidazioni abilitate";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaLiquidazioneAbilitata(ILiquidazioniAbilitate liqAb)
        {
            liqAb.ErrorMessage = string.Empty;
            liqAb.HasError = false;
            
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                if (liqAb != null && liqAb.datiLiquidazioneAbilitata != null)
                {
                    AreaEsito esito = objWS.StoreLiquidazioneAbilitata(liqAb.datiLiquidazioneAbilitata);

                    if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                    {
                        liqAb.ErrorMessage = esito.Messaggio;
                        liqAb.HasError = true;
                    }
                }
                else
                {
                    liqAb.ErrorMessage = "Nessun record da salvare";
                    liqAb.HasError = true;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo EliminaDatiIstruttoriaCi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaLiquidazioniAbilitateSuTutteLeSedi(ILiquidazioniAbilitate liqAb)
        {
            liqAb.ErrorMessage = string.Empty;
            liqAb.HasError = false;

            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                if (liqAb != null && liqAb.datiLiquidazioneAbilitata != null)
                {
                    AreaEsito esito = objWS.StoreLiquidazioniAbilitateSuTutteLeSedi(liqAb.datiLiquidazioneAbilitata);

                    if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                    {
                        liqAb.ErrorMessage = esito.Messaggio;
                        liqAb.HasError = true;
                    }
                }
                else
                {
                    liqAb.ErrorMessage = "Nessun record da salvare";
                    liqAb.HasError = true;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaLiquidazioniAbilitateSuTutteLeSedi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaLiquidazioneAbilitata(ILiquidazioniAbilitate liqAb)
        {
            liqAb.ErrorMessage = string.Empty;
            liqAb.HasError = false;

            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                if (liqAb != null && liqAb.datiLiquidazioneAbilitata != null)
                {
                    AreaEsito esito = objWS.DeleteLiquidazioneAbilitata(liqAb.datiLiquidazioneAbilitata);

                    if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                    {
                        liqAb.ErrorMessage = esito.Messaggio;
                        liqAb.HasError = true;
                    }
                }
                else
                {
                    liqAb.ErrorMessage = "Nessun record da eliminare";
                    liqAb.HasError = true;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo EliminaLiquidazioneAbilitata");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaLiquidazioniAbilitateSuTutteLeSedi(ILiquidazioniAbilitate liqAb)
        {
            liqAb.ErrorMessage = string.Empty;
            liqAb.HasError = false;

            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                if (liqAb != null && liqAb.datiLiquidazioneAbilitata != null)
                {
                    AreaEsito esito = objWS.DeleteLiquidazioniAbilitateSuTutteLeSedi(liqAb.datiLiquidazioneAbilitata);

                    if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                    {
                        liqAb.ErrorMessage = esito.Messaggio;
                        liqAb.HasError = true;
                    }
                }
                else
                {
                    liqAb.ErrorMessage = "Nessun record da eliminare";
                    liqAb.HasError = true;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo EliminaLiquidazioniAbilitateSuTutteLeSedi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
        #endregion public members
    }
}
