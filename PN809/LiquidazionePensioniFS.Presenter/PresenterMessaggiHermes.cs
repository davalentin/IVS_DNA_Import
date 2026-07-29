using System;
using System.ServiceModel;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterMessaggiHermes
    {
        public void GetMessaggiHermes(IMessaggiHermes MessaggiHermes)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaMessaggiHermes areaMessaggiHermes = null;
            try
            {
                AreaEsito esito = objWS.GetMessaggiHermes(out areaMessaggiHermes, MessaggiHermes.tipoApp.GetValueOrDefault());
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    MessaggiHermes.HasError = true;
                    MessaggiHermes.ErrorMessage = esito.Messaggio;
                    return;
                }
                MessaggiHermes.areaMessaggiHermes = areaMessaggiHermes;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMessaggiHermes, Errore nel metodo GetMessaggiHermes");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaMessaggioHermes(IMessaggiHermes MessaggiHermes)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaMessaggiHermes areaMessaggiHermes = MessaggiHermes.areaMessaggiHermes;
            try
            {
                AreaEsito esito = objWS.SalvaMessaggioHermes(ref areaMessaggiHermes);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    MessaggiHermes.HasError = true;
                    MessaggiHermes.ErrorMessage = esito.Messaggio;
                    return;
                }
                MessaggiHermes.areaMessaggiHermes = areaMessaggiHermes;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMessaggiHermes, Errore nel metodo SalvaMessaggioHermes");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void DeleteMessaggioHermes(IMessaggiHermes MessaggiHermes)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaMessaggiHermes areaMessaggiHermes = MessaggiHermes.areaMessaggiHermes;
            try
            {
                AreaEsito esito = objWS.DeleteMessaggioHermes(MessaggiHermes.tipoApp.GetValueOrDefault(), ref areaMessaggiHermes);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    MessaggiHermes.HasError = true;
                    MessaggiHermes.ErrorMessage = esito.Messaggio;
                    return;
                }
                MessaggiHermes.areaMessaggiHermes = areaMessaggiHermes;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMessaggiHermes, Errore nel metodo DeleteMessaggioHermes");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
