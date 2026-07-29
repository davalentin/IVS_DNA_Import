using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterControlliDinamici
    {
        public AreaEsito GetDataSistema(UtilityTipoAppartenenza? tipoAppartenenza, out DateTime? dataSistema)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaEsito esito = new AreaEsito();
            AreaControlliDinamici areaControlliDinamici = null;
            dataSistema = DateTime.Now;
            try
            {
                esito = objWS.GetDataSistema(out areaControlliDinamici, tipoAppartenenza);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterControlliDinamici, Errore nel metodo GetDataSistema");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (areaControlliDinamici != null && areaControlliDinamici.DataSistema.HasValue)
                dataSistema = areaControlliDinamici.DataSistema;

            return esito;
        }

        public AreaEsito SetDataSistema(UtilityTipoAppartenenza? tipoAppartenenza, DateTime? dataSistema)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaEsito esito = new AreaEsito();
            AreaControlliDinamici areaControlliDinamici = new AreaControlliDinamici();
            areaControlliDinamici.DataSistema = dataSistema;
            try
            {
                 esito = objWS.SetDataSistema(tipoAppartenenza, areaControlliDinamici);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterControlliDinamici, Errore nel metodo SetDataSistema");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            return esito;
        }

        public AreaEsito GetControlloDinamicoByNomeControllo(string nomeControllo, out string valoreControllo)
        {
            valoreControllo = string.Empty;
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaEsito esito = new AreaEsito();
            AreaControlliDinamici areaControlliDinamici = new AreaControlliDinamici();
            areaControlliDinamici.NomeControllo = nomeControllo;
            try
            {
                esito = objWS.GetControlloDinamicoByNomeControllo(ref areaControlliDinamici);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterControlliDinamici, Errore nel metodo SetDataSistema");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (areaControlliDinamici != null)
                valoreControllo = areaControlliDinamici.ValoreControllo;

            return esito;
        }

        public AreaEsito GetAnnoCompetenza(UtilityTipoAppartenenza? tipoAppartenenza, out string valoreControllo)
        {
            valoreControllo = string.Empty;
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaEsito esito = new AreaEsito();
            AreaControlliDinamici areaControlliDinamici = new AreaControlliDinamici();
            try
            {
                esito = objWS.GetAnnoCompetenza(out areaControlliDinamici, tipoAppartenenza);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterControlliDinamici, Errore nel metodo GetAnnoCompetenza");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (areaControlliDinamici != null)
                valoreControllo = areaControlliDinamici.ValoreControllo;

            return esito;
        }
    }
}
