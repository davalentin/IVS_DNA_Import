using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterAggiornamenti
    {
        public void GetAggiornamenti(IAggiornamenti Aggiornamenti)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaAggiornamenti areaAggiornamenti = null;
            try
            {
                AreaEsito esito = objWS.GetAggiornamenti(out areaAggiornamenti, Aggiornamenti.tipoApp.GetValueOrDefault());
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    Aggiornamenti.HasError = true;
                    Aggiornamenti.ErrorMessage = esito.Messaggio;
                    return;
                }
                Aggiornamenti.areaAggiornamenti = areaAggiornamenti;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterAggiornamenti: errore nel metodo GetAggiornamenti: ");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaAggiornamento(IAggiornamenti Aggiornamenti)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaAggiornamenti areaAggiornamenti = Aggiornamenti.areaAggiornamenti;
            try
            {
                AreaEsito esito = objWS.SalvaAggiornamento(ref areaAggiornamenti);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    Aggiornamenti.HasError = true;
                    Aggiornamenti.ErrorMessage = esito.Messaggio;
                    return;
                }
                Aggiornamenti.areaAggiornamenti = areaAggiornamenti;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterAggiornamenti: errore nel metodo SalvaAggiornamento: ");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void DeleteAggiornamento(IAggiornamenti Aggiornamenti)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaAggiornamenti areaAggiornamenti = Aggiornamenti.areaAggiornamenti;
            try
            {
                AreaEsito esito = objWS.DeleteAggiornamento(Aggiornamenti.tipoApp.GetValueOrDefault(), ref areaAggiornamenti);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    Aggiornamenti.HasError = true;
                    Aggiornamenti.ErrorMessage = esito.Messaggio;
                    return;
                }
                Aggiornamenti.areaAggiornamenti = areaAggiornamenti;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterAggiornamenti: errore nel metodo DeleteAggiornamento: ");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
