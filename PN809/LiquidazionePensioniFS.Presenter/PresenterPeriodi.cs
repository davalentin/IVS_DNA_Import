using System;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterPeriodi
    {
        public void GetAreaPeriodi(IPeriodi periodi)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaPeriodi areaPeriodi = null;
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(periodi.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = periodi.domanda.ProgStorico;

            try
            {
                AreaEsito esito = objWS.GetAreaPeriodiByDomanda(out areaPeriodi, areaRichiestaDomanda);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    periodi.HasError = true;
                    periodi.ErrorMessage = esito.Messaggio;
                    return;
                }
                periodi.areaPeriodi = areaPeriodi;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterPeriodi, Errore nel metodo GetAreaPeriodi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaDatiPeriodi(IPeriodi periodi)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaPeriodi areaPeriodi = periodi.areaPeriodi;
            long ndomanda = Int64.Parse(periodi.domanda.NumeroDomanda);
            try
            {
                AreaEsito esito = objWS.SalvaDatiPeriodiByDomanda(ndomanda, areaPeriodi);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    periodi.HasError = true;
                    periodi.ErrorMessage = esito.Messaggio;
                    return;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterPeriodi, Errore nel metodo SalvaDatiPeriodi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void StorePeriodi(IPeriodi periodi)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaPeriodi areaPeriodi = periodi.areaPeriodi;
            long ndomanda = Int64.Parse(periodi.domanda.NumeroDomanda);
            try
            {
                AreaEsito esito = objWS.StorePeriodi(ndomanda, areaPeriodi);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    periodi.HasError = true;
                    periodi.ErrorMessage = esito.Messaggio;
                    return;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterPeriodi, Errore nel metodo StorePeriodi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaDatiPeriodi(IPeriodi periodi)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaPeriodi areaPeriodi = periodi.areaPeriodi;
            long ndomanda = Int64.Parse(periodi.domanda.NumeroDomanda);
            try
            {
                AreaEsito esito = objWS.DeleteDatiPeriodi(ndomanda, ref areaPeriodi);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    periodi.HasError = true;
                    periodi.ErrorMessage = esito.Messaggio;
                    return;
                }
                periodi.areaPeriodi = areaPeriodi;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterPeriodi, Errore nel metodo EliminaDatiPeriodi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
