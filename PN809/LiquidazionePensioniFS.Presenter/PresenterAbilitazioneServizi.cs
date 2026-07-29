using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterAbilitazioneServizi
    {
        
        public void GetAbilitazioneServizi(IAbilitazioneServizi IAbilitazioneServizi)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaEsito esito;
            AreaAbilitazioneServizi areaAbilitazioneServizi = null;
            try
            {
                esito = objWS.GetAreaAbilitazioneServizi(out areaAbilitazioneServizi);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    IAbilitazioneServizi.HasError = true;
                    IAbilitazioneServizi.ErrorMessage = esito.Messaggio;
                }
                else if (areaAbilitazioneServizi != null)
                    IAbilitazioneServizi.areaAbilitazioneServizi = areaAbilitazioneServizi;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterAbilitazioneServizi, Errore nel metodo GetAbilitazioneServizi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SetAbilitazionePolarizzazioneENPALS(IAbilitazioneServizi IAbilitazioneServizi)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaEsito esito;
            AreaAbilitazioneServizi areaAbilitazioneServizi = new AreaAbilitazioneServizi();
            areaAbilitazioneServizi.IsPolarizzazioneENPALSAbilitata = IAbilitazioneServizi.areaAbilitazioneServizi.IsPolarizzazioneENPALSAbilitata;
            areaAbilitazioneServizi.IsPolarizzazioneSuperstitiENPALSAbilitata = IAbilitazioneServizi.areaAbilitazioneServizi.IsPolarizzazioneSuperstitiENPALSAbilitata;
            try
            {
                esito = objWS.SetPolarizzazioneENPALSAttivo(areaAbilitazioneServizi);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    IAbilitazioneServizi.HasError = true;
                    IAbilitazioneServizi.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterAbilitazioneServizi, Errore nel metodo SetPolarizzazioneENPALSAttivo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
