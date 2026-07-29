using System;
using System.ServiceModel;
using INPS.DNA;

using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterSbloccoDomanda
    {
        public void SbloccoDomanda(ISbloccoDomanda sbloccoDomanda)
        {
            AreaEsito esito = new AreaEsito();
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            short sedeOperatore = Utility.GetSedeOperatore();
            short centroOperativoOperatore = Utility.GetCentroOperativoOperatore();
            string sedeDiversa = string.Empty;
            try
            {
                esito = objWS.SbloccoDomanda(out sedeDiversa, sbloccoDomanda.numDomanda, sbloccoDomanda.tipoAppRuolo, sedeOperatore, centroOperativoOperatore);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterSbloccoDomanda, Errore nel metodo SbloccoDomanda");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                sbloccoDomanda.HasError = true;
                sbloccoDomanda.ErrorMessage = esito.Messaggio;
                sbloccoDomanda.sedeDiversa = sedeDiversa;
            }
        }
    }
}

