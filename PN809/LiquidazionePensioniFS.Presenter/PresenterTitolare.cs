using System;
using System.ServiceModel;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterTitolare
    {
        public void SalvaDatiTitolare(ITitolarePensione titolare, out bool isTabAnagraficaSaved, out bool isWarning)
        {
            isTabAnagraficaSaved = false;
            isWarning = false;
            try
            {
                string sErrore;
                bool myFlagUnicarpe;
                SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
                if (titolare.TitolarePensione.Pensione.FlagUnicarpe == null)
                    myFlagUnicarpe = false;
                else
                    myFlagUnicarpe = (bool)titolare.TitolarePensione.Pensione.FlagUnicarpe;
                if ((Utility.CheckNTelefono(titolare.TitolarePensione.Anagrafica.Tel, out  sErrore)) &&
                    (Utility.CheckNTelefono(titolare.TitolarePensione.Anagrafica.Cell, out sErrore)) &&
                    (Utility.CheckEmail(titolare.TitolarePensione.Anagrafica.EMail, out sErrore))&&
                    (Utility.CheckDecorrenzaPensione(titolare.TitolarePensione.Pensione.DecorrenzaOriginaria, out sErrore)))
                {
                    try
                    {
                        titolare.TitolarePensione.Esito = objWS.StoreAreaTitolare(out isTabAnagraficaSaved, out isWarning, titolare.TitolarePensione);
                        if (titolare.TitolarePensione.Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                        {
                            titolare.ErrorMessage = "Errore tecnico nel salvataggio del quadro Titolare";
                            titolare.HasError = true;
                        }
                        else if (!String.IsNullOrEmpty(titolare.TitolarePensione.Esito.Messaggio))
                        {
                            titolare.ErrorMessage = titolare.TitolarePensione.Esito.Messaggio;
                            titolare.HasError = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Utility.ExceptionHandler(ex, "PresenterTitolare, Errore nel metodo SalvaDatiTitolare");
                    }
                    finally
                    {
                        Utility.CloseClient(objWS);
                    }
                }
                else
                {
                    titolare.ErrorMessage = sErrore;
                    titolare.HasError = true;
                }
                return;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("PresenterTitolare, Errore nel metodo SalvaDatiAnagrafica" + ex);
            }
        }

        public AreaTitolare CaricaTitolare(ITitolarePensione titolare)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(titolare.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = titolare.domanda.ProgStorico;

            try
            {
                titolare.TitolarePensione = objWS.GetAreaTitolareByDomanda(areaRichiestaDomanda);

                if (titolare.TitolarePensione.Esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    titolare.HasError = true;
                    titolare.ErrorMessage = titolare.TitolarePensione.Esito.Messaggio;
                }
                else
                {
                    titolare.HasError = false;
                    titolare.ErrorMessage = string.Empty;
                }

                return titolare.TitolarePensione;
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
                titolare.HasError = true;
                titolare.ErrorMessage = string.Format("Errore CaricaTitolare: {0}", Ex.Message);
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            return titolare.TitolarePensione;
        }

        public void SalvaDatiTabAnagrafica(ITitolarePensione titolare, out bool isWarning)
        {
            string sErrore = string.Empty;
            isWarning = false;
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                if ((Utility.CheckNTelefono(titolare.TitolarePensione.Anagrafica.Tel, out  sErrore)) &&
                    (Utility.CheckNTelefono(titolare.TitolarePensione.Anagrafica.Cell, out sErrore)) &&
                    (Utility.CheckEmail(titolare.TitolarePensione.Anagrafica.EMail, out sErrore)) &&
                    (Utility.CheckDecorrenzaPensione(titolare.TitolarePensione.Pensione.DecorrenzaOriginaria, out sErrore)))
                {
                    titolare.TitolarePensione.Esito = objWS.StoreAnagrafica(out isWarning, titolare.TitolarePensione);

                    if (titolare.TitolarePensione.Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                    {
                        titolare.ErrorMessage = "Errore tecnico nel salvataggio del tab Anagrafica";
                        titolare.HasError = true;
                    }
                    else if (!String.IsNullOrEmpty(titolare.TitolarePensione.Esito.Messaggio))
                    {
                        titolare.ErrorMessage = titolare.TitolarePensione.Esito.Messaggio;
                        titolare.HasError = true;
                    }
                }
                else
                {
                    titolare.ErrorMessage = sErrore;
                    titolare.HasError = true;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterTitolare, Errore nel metodo SalvaDatiTabAnagrafica");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaDatiTabStatoCivile(ITitolarePensione titolare)
        {
            string sErrore = string.Empty;
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                titolare.TitolarePensione.Esito = objWS.StoreStatoCivile(titolare.TitolarePensione);

                if (titolare.TitolarePensione.Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    titolare.HasError = true;
                    titolare.ErrorMessage = "Errore tecnico nel salvataggio del tab StatoCivile";
                }
                else if (titolare.TitolarePensione.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK && titolare.TitolarePensione.Esito.Messaggio != string.Empty)
                {
                    titolare.ErrorMessage = titolare.TitolarePensione.Esito.Messaggio;
                    titolare.HasError = true;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterTitolare, Errore nel metodo SalvaDatiTabStatoCivile");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaDatiTabResidenzeEstere(ITitolarePensione titolare)
        {
            string sErrore = string.Empty;
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                titolare.TitolarePensione.Esito = objWS.StoreResidenzeEstere(titolare.TitolarePensione);

                if (titolare.TitolarePensione.Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    titolare.ErrorMessage = "Errore tecnico nel salvataggio del tab ResidenzeEstere";
                    titolare.HasError = true;
                }
                else if (titolare.TitolarePensione.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK && titolare.TitolarePensione.Esito.Messaggio != string.Empty)
                {
                    titolare.ErrorMessage = titolare.TitolarePensione.Esito.Messaggio;
                    titolare.HasError = true;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterTitolare, Errore nel metodo SalvaDatiTabResidenzeEstere");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaDatiTabResidenzeEstere(ITitolarePensione titolare)
        {
            string sErrore = string.Empty;
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                Int64 numeroDomanda = Int64.Parse(titolare.domanda.NumeroDomanda);
                titolare.TitolarePensione = new AreaTitolare();
                titolare.TitolarePensione.Esito = objWS.DeleteResidenzeEstere(numeroDomanda);

                if (titolare.TitolarePensione.Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    sErrore = titolare.TitolarePensione.Esito.Messaggio;
                    throw new INPS.DNA.DnaApplicationException("PresenterTitolare, Errore nel metodo EliminaDatiTabResidenzeEstere" + sErrore);
                }
                else if (titolare.TitolarePensione.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK && titolare.TitolarePensione.Esito.Messaggio != string.Empty)
                {
                    titolare.ErrorMessage = titolare.TitolarePensione.Esito.Messaggio;
                    titolare.HasError = true;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterTitolare, Errore nel metodo EliminaDatiTabResidenzeEstere");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void AggiornaDaARCA(ITitolarePensione titolare)
        {
            try
            {
                ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

                try
                {
                    long numeroDomanda = 0;
                    long.TryParse(titolare.domanda.NumeroDomanda, out numeroDomanda);
                    AreaRispostaRiepilogo.DatiRiepilogoAnagrafica anagrafica = titolare.TitolarePensione.Anagrafica;
                    titolare.TitolarePensione.Esito = objWS.AggiornaAnagraficaTitolareByArca(numeroDomanda, Utility.GetSedeOperatore(), Utility.GetMatricolaOperatore(), ref anagrafica);
                    if (titolare.TitolarePensione.Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO || !String.IsNullOrEmpty(titolare.TitolarePensione.Esito.Messaggio))
                    {
                        titolare.ErrorMessage = titolare.TitolarePensione.Esito.Messaggio;
                        titolare.HasError = true;
                    }
                    titolare.TitolarePensione.Anagrafica = anagrafica;
                }
                catch (Exception ex)
                {
                    Utility.ExceptionHandler(ex, "PresenterTitolare, Errore nel metodo AggiornaDaARCA");
                }
                finally
                {
                    Utility.CloseClient(objWS);
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("PresenterTitolare, Errore nel metodo AggiornaDaARCA: " + ex);
            }
        }
    }
}