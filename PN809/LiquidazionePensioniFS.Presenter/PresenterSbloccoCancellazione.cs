using System;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterSbloccoCancellazione
    {
        public void SbloccoCancellazione(ISbloccoCancellazione sbloccoCancellazione)
        {
            AreaEsito esito = new AreaEsito();
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaSbloccoCancellazione areaSbloccoCancellazione = new AreaSbloccoCancellazione();

            areaSbloccoCancellazione.NumeroDomanda = sbloccoCancellazione.areaSbloccoCancellazione.NumeroDomanda;
            areaSbloccoCancellazione.CodiceSede = sbloccoCancellazione.areaSbloccoCancellazione.CodiceSede;
            areaSbloccoCancellazione.CentroOperativo = sbloccoCancellazione.areaSbloccoCancellazione.CentroOperativo;
            areaSbloccoCancellazione.SiglaCategoria = sbloccoCancellazione.areaSbloccoCancellazione.SiglaCategoria;
            areaSbloccoCancellazione.TipoOperazione = sbloccoCancellazione.areaSbloccoCancellazione.TipoOperazione;

            try
            {
                esito = objWS.SbloccoCancellazione(areaSbloccoCancellazione);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterSbloccoCancellazione, Errore nel metodo SbloccoCancellazione");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                sbloccoCancellazione.HasError = true;
                sbloccoCancellazione.ErrorMessage = esito.Messaggio;
            }
            else
            {
                sbloccoCancellazione.HasError = false;
                sbloccoCancellazione.ErrorMessage = string.Empty;
            }
        }
    }
}
