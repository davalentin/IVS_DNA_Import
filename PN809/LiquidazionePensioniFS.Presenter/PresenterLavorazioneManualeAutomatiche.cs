using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterLavorazioneManualeAutomatiche
    {
        public void CaricaLavorazioneManualeAutomatiche(ILavorazioneManualeAutomatiche lavorazioneManualeAutomatiche)
        {
            lavorazioneManualeAutomatiche.HasError = false;
            lavorazioneManualeAutomatiche.ErrorMessage = string.Empty;

            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaLavorazioneManualeAutomatiche lavMan = null;
                AreaEsito esito = objWS.GetAllPensioniLavorazioneManualeAutomatiche(out lavMan, lavorazioneManualeAutomatiche.tipoAppRuolo);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    lavorazioneManualeAutomatiche.HasError = true;
                    lavorazioneManualeAutomatiche.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    lavorazioneManualeAutomatiche.LavorazioneManualeAutomatiche = lavMan;
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
                lavorazioneManualeAutomatiche.HasError = true;
                lavorazioneManualeAutomatiche.ErrorMessage = "Errore nel recupero delle lavorazioni manuali delle domande automatiche dei controlli";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void CaricaLavorazioneManualeAutomaticheByCodiceSede(string utente, List<Int16> sediAbilitate, ILavorazioneManualeAutomatiche lavorazioneManualeAutomatiche)
        {
            lavorazioneManualeAutomatiche.HasError = false;
            lavorazioneManualeAutomatiche.ErrorMessage = string.Empty;

            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaLavorazioneManualeAutomatiche lavMan = null;
                Int16[] sediAbilitateArray = sediAbilitate.ToArray();

                AreaEsito esito = objWS.GetAllPensioniLavorazioneManualeAutomaticheByCodiceSede(out lavMan, utente, lavorazioneManualeAutomatiche.tipoAppRuolo, sediAbilitateArray);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    lavorazioneManualeAutomatiche.HasError = true;
                    lavorazioneManualeAutomatiche.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    lavorazioneManualeAutomatiche.LavorazioneManualeAutomatiche = lavMan;
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
                lavorazioneManualeAutomatiche.HasError = true;
                lavorazioneManualeAutomatiche.ErrorMessage = "Errore nel recupero delle lavorazioni manuali delle domande automatiche dei controlli";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaLavorazioneManualeAutomatiche(ILavorazioneManualeAutomatiche lavorazioneManualeAutomatiche)
        {
            lavorazioneManualeAutomatiche.ErrorMessage = string.Empty;
            lavorazioneManualeAutomatiche.HasError = false;

            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                if (lavorazioneManualeAutomatiche != null && lavorazioneManualeAutomatiche.datiLavorazioneManualeAutomatiche != null)
                {
                    AreaEsito esito = objWS.StoreLavorazioneManualeAutomatiche(lavorazioneManualeAutomatiche.datiLavorazioneManualeAutomatiche);

                    if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                    {
                        lavorazioneManualeAutomatiche.ErrorMessage = esito.Messaggio;
                        lavorazioneManualeAutomatiche.HasError = true;
                    }
                }
                else
                {
                    lavorazioneManualeAutomatiche.ErrorMessage = "Nessun record da salvare";
                    lavorazioneManualeAutomatiche.HasError = true;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLavorazioneManualeAutomatiche, Errore nel metodo SalvaLavorazioneManualeAutomatiche");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public AreaEsito SalvaLavorazioneManualeAutomatiche(AreaLavorazioneManualeAutomatiche.LavorazioneManualeAutomatiche lavorazioneManualeAutomatiche, out string messaggio)
        {
            messaggio = String.Empty;
            AreaEsito esito = new AreaEsito();
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                if (lavorazioneManualeAutomatiche != null)
                {
                    esito = objWS.StoreLavorazioneManualeAutomatiche(lavorazioneManualeAutomatiche);
                    if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                    {
                        messaggio = esito.Messaggio;
                    }
                }
                else
                {
                    messaggio = "Nessun record da salvare";
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLavorazioneManualeAutomatiche, Errore nel metodo SalvaLavorazioneManualeAutomatiche");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
            return esito;
        }
    }
}
