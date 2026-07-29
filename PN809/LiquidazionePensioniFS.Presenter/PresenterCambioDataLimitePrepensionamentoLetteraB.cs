using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterCambioDataLimitePrepensionamentoLetteraB
    {
        public AreaEsito SetDataCalcoloPrepensionamentoLetteraB(IDataLimitePrepensionamentoLetteraB dataLimitePrepensionamentoLetteraB)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaEsito esito = new AreaEsito();
            try
            {
                if (dataLimitePrepensionamentoLetteraB != null && dataLimitePrepensionamentoLetteraB.dataLimitePrepensionamentoLetteraB != null && dataLimitePrepensionamentoLetteraB.dataLimitePrepensionamentoLetteraB.DataLimitePoligraficiLetteraB != null)
                {
                    esito = objWS.SetDataCalcoloPoligraficiLetteraB(dataLimitePrepensionamentoLetteraB.tipoAppRuolo, dataLimitePrepensionamentoLetteraB.dataLimitePrepensionamentoLetteraB);

                    if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                    {
                        dataLimitePrepensionamentoLetteraB.ErrorMessage = esito.Messaggio;
                        dataLimitePrepensionamentoLetteraB.HasError = true;
                    }
                }
                else
                {
                    dataLimitePrepensionamentoLetteraB.ErrorMessage = "Nessun record da salvare";
                    dataLimitePrepensionamentoLetteraB.HasError = true;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterCambioDataLimitePrepensionamentoLetteraB, Errore nel metodo SetDataCalcoloPrepensionamentoLetteraB");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            return esito;
        }

        public AreaEsito GetDataCalcoloPrepensionamentoLetteraB(out AreaStoricoDataLimitePrepensionementoLetteraB storico)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaEsito esito = new AreaEsito();
            storico = null;

            try
            {
                esito = objWS.GetStoricoDataLimitePoligraficiLetteraB(out storico);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterCambioDataLimitePrepensionamentoLetteraB, Errore nel metodo GetDataCalcoloPrepensionamentoLetteraB");
            }

            return esito;
        }

        public AreaEsito UpdateNote(int id, string note)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaEsito esito = new AreaEsito();

            try
            {
                esito = objWS.UpdateNoteStoricoDataLimitePoligraficiLetteraB(id, note);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterCambioDataLimitePrepensionamentoLetteraB, Errore nel metodo UpdateNote");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            return esito;
        }
    }
}
