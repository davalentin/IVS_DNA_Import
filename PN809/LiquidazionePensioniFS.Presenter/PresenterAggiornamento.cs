using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterAggiornamento
    {
        public void GetAggiornamento(IAggiornamento aggiornamento)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaAggiornamento areaAggiornamento = null;
            try
            {
                AreaEsito esito = objWS.GetAreaAggiornamento(out areaAggiornamento, aggiornamento.TipoApp.GetValueOrDefault());
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    aggiornamento.HasError = true;
                    aggiornamento.ErrorMessage = esito.Messaggio;
                    return;
                }
                aggiornamento.areaAggiornamento = areaAggiornamento;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterAggiornamento: errore nel metodo GetAggiornamento: ");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void ElaboraAggiornamento(IAggiornamento aggiornamento)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                objWS.Endpoint.Binding.CloseTimeout = new TimeSpan(0, 1, 0);
                objWS.Endpoint.Binding.OpenTimeout = new TimeSpan(0, 1, 0);
                objWS.Endpoint.Binding.SendTimeout = new TimeSpan(0, 1, 0);
                objWS.Endpoint.Binding.ReceiveTimeout = new TimeSpan(0, 1, 0);
                if (aggiornamento.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.WebDom)
                    objWS.ElaboraAggiornamentoWebDom(aggiornamento.TipoApp.GetValueOrDefault());
                else if (aggiornamento.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.Felpe)
                    objWS.ElaboraAggiornamentoFelpe(aggiornamento.TipoApp.GetValueOrDefault());
                else if (aggiornamento.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.Oneri)
                    objWS.ElaboraAggiornamentoOneri(aggiornamento.TipoApp.GetValueOrDefault());
                else if (aggiornamento.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.Cumulo)
                    objWS.ElaboraAggiornamentoCumulo(aggiornamento.TipoApp.GetValueOrDefault());
                else if (aggiornamento.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.SAI)
                    objWS.ElaboraAggiornamentoSAI(aggiornamento.TipoApp.GetValueOrDefault());
                else if (aggiornamento.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.INPDAP)
                    objWS.ElaboraAggiornamentoINPDAP(aggiornamento.TipoApp.GetValueOrDefault());
                else if (aggiornamento.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.Tot)
                    objWS.ElaboraAggiornamentoTot(aggiornamento.TipoApp.GetValueOrDefault());
                else if (aggiornamento.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.NoteDiDebito)
                    objWS.ElaboraAggiornamentoNoteDiDebito(aggiornamento.TipoApp.GetValueOrDefault());
                else if (aggiornamento.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.PianiDiPagamento)
                    objWS.ElaboraAggiornamentoPianiDiPagamento(aggiornamento.TipoApp.GetValueOrDefault());
            }
            catch (TimeoutException)
            {
                aggiornamento.HasError = true;
            }
            catch (System.ServiceModel.CommunicationException ex)
            {
                if (ex.InnerException.Message.Contains("timeout"))
                    aggiornamento.HasError = true;
                else
                    throw;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterAggiornamento: errore nel metodo ElaboraAggiornamento: ");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void CaricaPdfAggiornamento(IAggiornamento aggiornamento)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaAggiornamento areaAggiornamento = null;
            try
            {
                AreaEsito esito = new AreaEsito();
                if (aggiornamento.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.WebDom)
                    esito = objWS.CaricaPdfAggiornamentoWebDom(out areaAggiornamento, aggiornamento.TipoApp.GetValueOrDefault());
                else if (aggiornamento.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.Felpe)
                    esito = objWS.CaricaPdfAggiornamentoFelpe(out areaAggiornamento, aggiornamento.TipoApp.GetValueOrDefault());
                else if (aggiornamento.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.Oneri)
                    esito = objWS.CaricaPdfAggiornamentoOneri(out areaAggiornamento, aggiornamento.TipoApp.GetValueOrDefault());
                else if (aggiornamento.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.SAI)
                    esito = objWS.CaricaPdfAggiornamentoSAI(out areaAggiornamento, aggiornamento.TipoApp.GetValueOrDefault());
                else if (aggiornamento.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.Cumulo)
                    esito = objWS.CaricaPdfAggiornamentoCumulo(out areaAggiornamento, aggiornamento.TipoApp.GetValueOrDefault());
                else if (aggiornamento.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.INPDAP)
                    esito = objWS.CaricaPdfAggiornamentoINPDAP(out areaAggiornamento, aggiornamento.TipoApp.GetValueOrDefault());
                else if (aggiornamento.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.Tot)
                    esito = objWS.CaricaPdfAggiornamentoTot(out areaAggiornamento, aggiornamento.TipoApp.GetValueOrDefault());
                else if (aggiornamento.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.NoteDiDebito)
                    esito = objWS.CaricaPdfAggiornamentoNoteDiDebito(out areaAggiornamento, aggiornamento.TipoApp.GetValueOrDefault());
                else if (aggiornamento.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.PianiDiPagamento)
                    esito = objWS.CaricaPdfAggiornamentoPianiDiPagamento(out areaAggiornamento, aggiornamento.TipoApp.GetValueOrDefault());

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    aggiornamento.HasError = true;
                    aggiornamento.ErrorMessage = esito.Messaggio;
                    return;
                }
                aggiornamento.areaAggiornamento = areaAggiornamento;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterAggiornamento: errore nel metodo CaricaPdfAggiornamento: ");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
