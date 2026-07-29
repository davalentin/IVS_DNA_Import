using System;
using System.ServiceModel;
using INPS.DNA;

using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterRedditi
    {
        public void GetRedditi(IRedditi redditi)
        {
            AreaRedditi areaRedditi = null;
            short sede = Utility.GetSedeOperatore();
            string matricola = Utility.GetMatricolaOperatore();
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(redditi.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = redditi.domanda.ProgStorico;
            AreaEsito risultatoGetRedditiByDomanda = null;

            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                risultatoGetRedditiByDomanda = objWS.GetRedditiByDomanda(out areaRedditi, areaRichiestaDomanda, matricola, sede);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterRedditi, Errore nel metodo GetRedditi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoGetRedditiByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                redditi.HasError = true;
                redditi.ErrorMessage = "#_SERVICE_ERROR_#" + risultatoGetRedditiByDomanda.Messaggio;
                redditi.areaRedditi = areaRedditi;
            }
            else
            {
                if (areaRedditi.Redditi.Esito == GestioneRedditiTipoRitornoRedditi.NessunErrore)
                {
                    redditi.HasError     = false;
                    redditi.ErrorMessage = "";
                    redditi.areaRedditi  = areaRedditi;

                }
                else if (areaRedditi.Redditi.Esito == GestioneRedditiTipoRitornoRedditi.Informativa || areaRedditi.Redditi.Esito == GestioneRedditiTipoRitornoRedditi.Errore)
                {
                    redditi.HasError = true;
                    redditi.ErrorMessage = areaRedditi.Redditi.MessaggioVideo;
                    redditi.areaRedditi = areaRedditi;
                }
            }
        }

        public void SalvaRedditi(IRedditi redditi)
        {
            AreaRedditi areaRedditi = null;
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            short sede = Utility.GetSedeOperatore();
            string matricola = Utility.GetMatricolaOperatore();
            Int64 ndomanda = Int64.Parse(redditi.domanda.NumeroDomanda);
            AreaEsito risultatoVerifyRedditiByDomanda = null;
            try
            {
                risultatoVerifyRedditiByDomanda = objWS.VerifyRedditiByDomanda(out areaRedditi, ndomanda, matricola, sede, redditi.IsSalvataggio, redditi.areaRedditi);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterRedditi, Errore nel metodo SalvaRedditi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            try
            {
                if (risultatoVerifyRedditiByDomanda != null && risultatoVerifyRedditiByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    redditi.HasError = true;
                    redditi.ErrorMessage = "#_SERVICE_ERROR_#" + risultatoVerifyRedditiByDomanda.Messaggio;
                    redditi.areaRedditi = areaRedditi;
                }
                else
                {
                    if (areaRedditi.Redditi.Esito == GestioneRedditiTipoRitornoRedditi.NessunErrore)
                    {
                        redditi.HasError = false;
                        redditi.ErrorMessage = "";
                        redditi.areaRedditi = areaRedditi;
                    }
                    else if (areaRedditi.Redditi.Esito == GestioneRedditiTipoRitornoRedditi.Informativa || areaRedditi.Redditi.Esito == GestioneRedditiTipoRitornoRedditi.Errore)
                    {
                        redditi.HasError = true;
                        redditi.ErrorMessage = areaRedditi.Redditi.MessaggioVideo;
                        redditi.areaRedditi = areaRedditi;
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("PresenterRedditi, Errore nel metodo SalvaRedditi" + ex);
            }
        }

        public void EliminaRedditi(IRedditi redditi)
        {
            AreaRedditi areaRedditi = null;
            short sede = Utility.GetSedeOperatore();
            string matricola = Utility.GetMatricolaOperatore();
            Int64 numeroDomanda = Int64.Parse(redditi.domanda.NumeroDomanda);
            AreaEsito risultatoEliminaRedditiByDomanda = null;

            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                risultatoEliminaRedditiByDomanda = objWS.EliminaRedditiByDomanda(out areaRedditi, numeroDomanda, matricola, sede);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterRedditi, Errore nel metodo EliminaRedditi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            try
            {
                if (risultatoEliminaRedditiByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    redditi.HasError = true;
                    redditi.ErrorMessage = "#_SERVICE_ERROR_#" + risultatoEliminaRedditiByDomanda.Messaggio;
                    redditi.areaRedditi = areaRedditi;
                }
                else
                {
                    if (areaRedditi.Redditi.Esito == GestioneRedditiTipoRitornoRedditi.NessunErrore)
                    {
                        redditi.HasError = false;
                        redditi.ErrorMessage = "";
                        redditi.areaRedditi = areaRedditi;
                    }
                    else if (areaRedditi.Redditi.Esito == GestioneRedditiTipoRitornoRedditi.Informativa || areaRedditi.Redditi.Esito == GestioneRedditiTipoRitornoRedditi.Errore)
                    {
                        redditi.HasError = true;
                        redditi.ErrorMessage = areaRedditi.Redditi.MessaggioVideo;
                        redditi.areaRedditi = areaRedditi;
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("PresenterRedditi, Errore nel metodo EliminaRedditi" + ex);
            }
        }
    }
}

