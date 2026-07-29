using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterCambioDataLimiteINDCOMEStrorico
    {
        public AreaEsito SetDataCalcoloDefinitivoINDCOM(IDataLimiteINDCOM dataLimiteINDCOM)
        {
            //    dataLimiteINDCOM.ErrorMessage = string.Empty;
            //    dataLimiteINDCOM.HasError = false;

            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaEsito esito = new AreaEsito();
            try
            {
                if (dataLimiteINDCOM != null && dataLimiteINDCOM.dataLimiteDomandeINDCOM != null && dataLimiteINDCOM.dataLimiteDomandeINDCOM.DataLimiteDomandeINDCOM != null)
                {
                    esito = objWS.SetDataCalcoloDefinitivoINDCOM(dataLimiteINDCOM.tipoAppRuolo, dataLimiteINDCOM.dataLimiteDomandeINDCOM);

                    if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                    {
                        dataLimiteINDCOM.ErrorMessage = esito.Messaggio;
                        dataLimiteINDCOM.HasError = true;
                    }
                }
                else
                {
                    dataLimiteINDCOM.ErrorMessage = "Nessun record da salvare";
                    dataLimiteINDCOM.HasError = true;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterCambioDataLimiteINDCOMEStrorico, Errore nel metodo SetDataCalcoloDefinitivoINDCOM");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            return esito;
        }

        public AreaEsito GetDataCalcoloDefinitivoINDCOM(out AreaStoricoDataLimiteDomandeINDCOM storico)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaEsito esito = new AreaEsito();
            storico = null;

            try
            {
                esito = objWS.GetStoricoDataLimiteINDCOM(out storico);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterCambioDataLimiteINDCOMEStrorico, Errore nel metodo GetDataCalcoloDefinitivoINDCOM");
            }

            return esito;
        }

        public AreaEsito UpdateNote(int id, string note)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaEsito esito = new AreaEsito();

            try
            {
                esito = objWS.UpdateNoteStoricoDataLimiteINDCOM(id, note);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterCambioDataLimiteINDCOMEStrorico, Errore nel metodo UpdateNote");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            return esito;
        }
    }
}
