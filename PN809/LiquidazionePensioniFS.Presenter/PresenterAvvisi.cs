using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterAvvisi
    {
        public void GetAvvisi(IAvvisi Avvisi)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaAvvisi areaAvvisi = null;
            try
            {
                AreaEsito esito = objWS.GetAvvisi(out areaAvvisi, Avvisi.tipoApp.GetValueOrDefault());
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    Avvisi.HasError = true;
                    Avvisi.ErrorMessage = esito.Messaggio;
                    return;
                }
                Avvisi.areaAvvisi = areaAvvisi;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterAvvisi: errore nel metodo GetAvvisi: ");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaAvviso(IAvvisi Avvisi)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaAvvisi areaAvvisi = Avvisi.areaAvvisi;
            try
            {
                AreaEsito esito = objWS.SalvaAvviso(ref areaAvvisi);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    Avvisi.HasError = true;
                    Avvisi.ErrorMessage = esito.Messaggio;
                    return;
                }
                Avvisi.areaAvvisi = areaAvvisi;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterAvvisi: errore nel metodo SalvaAvviso: ");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void DeleteAvviso(IAvvisi Avvisi)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaAvvisi areaAvvisi = Avvisi.areaAvvisi;
            try
            {
                AreaEsito esito = objWS.DeleteAvviso(Avvisi.tipoApp.GetValueOrDefault(), ref areaAvvisi);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    Avvisi.HasError = true;
                    Avvisi.ErrorMessage = esito.Messaggio;
                    return;
                }
                Avvisi.areaAvvisi = areaAvvisi;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterAvvisi: errore nel metodo DeleteAvviso: ");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
