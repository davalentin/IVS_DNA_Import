using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterAbilitazioneUniDetra
    {
        public void GetAbilitazioneUniDetra(IUniDetra IUnidetra)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaEsito esito;
            AreaUniDetra areaNewUniDetra = null;
            try
            {
                esito = objWS.GetUniDetraAttivo(out areaNewUniDetra);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    IUnidetra.HasError = true;
                    IUnidetra.ErrorMessage = esito.Messaggio;
                }
                else
                    if (areaNewUniDetra != null && areaNewUniDetra.IsNewUniDetraAttivo)
                {
                    IUnidetra.areaUniDetra = new AreaUniDetra();
                    IUnidetra.areaUniDetra.IsNewUniDetraAttivo = areaNewUniDetra.IsNewUniDetraAttivo;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterAbilitazioneUniDetra, Errore nel metodo GetUniDetraAttivo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SetAbilitazioneUniDetra(IUniDetra IUnidetra)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaEsito esito;
            AreaUniDetra areaUniDetra = new AreaUniDetra();
            areaUniDetra.IsNewUniDetraAttivo = IUnidetra.areaUniDetra.IsNewUniDetraAttivo;

            try
            {
                esito = objWS.SetUniDetraAttivo(areaUniDetra);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    IUnidetra.HasError = true;
                    IUnidetra.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterAbilitazioneUniDetra, Errore nel metodo SetAbilitazioneUniDetra");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
