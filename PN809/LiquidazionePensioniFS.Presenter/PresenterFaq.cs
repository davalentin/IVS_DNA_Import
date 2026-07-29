using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterFaq
    {
        public void GetFAQ(IFaq Faq)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaFAQ areaFAQ = null;
            try
            {
                AreaEsito esito = objWS.GetFAQ(out areaFAQ, Faq.tipoApp.GetValueOrDefault());
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    Faq.HasError = true;
                    Faq.ErrorMessage = esito.Messaggio;
                    return;
                }
                Faq.areaFAQ = areaFAQ;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterFaq, Errore nel metodo GetFAQ");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaFAQ(IFaq Faq)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaFAQ areaFAQ = Faq.areaFAQ;
            try
            {
                AreaEsito esito = objWS.SalvaFAQ(ref areaFAQ);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    Faq.HasError = true;
                    Faq.ErrorMessage = esito.Messaggio;
                    return;
                }
                Faq.areaFAQ = areaFAQ;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterFaq, Errore nel metodo SalvaFAQ");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void DeleteFAQ(IFaq Faq)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaFAQ areaFAQ = Faq.areaFAQ;
            try
            {
                AreaEsito esito = objWS.DeleteFAQ(Faq.tipoApp.GetValueOrDefault(), ref areaFAQ);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    Faq.HasError = true;
                    Faq.ErrorMessage = esito.Messaggio;
                    return;
                }
                Faq.areaFAQ = areaFAQ;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterFaq, Errore nel metodo DeleteFAQ");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void CaricaPdfFAQ(IFaq Faq)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaFAQ areaFAQ = null;
            try
            {
                AreaEsito esito = objWS.CaricaPdfFaq(out areaFAQ, Faq.tipoApp.GetValueOrDefault());
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    Faq.HasError = true;
                    Faq.ErrorMessage = esito.Messaggio;
                    return;
                }
                Faq.areaFAQ = areaFAQ;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterFaq, Errore nel metodo CaricaPdfFAQ");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
