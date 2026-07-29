using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterCambiaStatoDomanda
    {
        public void CambioStatoDomanda(ICambioStatoDomanda cambioStatoDomanda)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaEsito esitoCambioStatoDomanda = new AreaEsito();
            AreaCambioStatoDomanda areaSvc = cambioStatoDomanda.areaCambioStatoDomanda;
            try
            {
                esitoCambioStatoDomanda = objWS.CambioStatoDomanda(ref areaSvc);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterCambioStatoDomanda, Errore nel metodo CambioStatoDomanda");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (esitoCambioStatoDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                cambioStatoDomanda.HasError = true;
                cambioStatoDomanda.ErrorMessage = esitoCambioStatoDomanda.Messaggio;
            }
            else
            {
                cambioStatoDomanda.HasError = false;
                cambioStatoDomanda.ErrorMessage = "";
            }

            cambioStatoDomanda.areaCambioStatoDomanda = areaSvc;
        }
    }
}
