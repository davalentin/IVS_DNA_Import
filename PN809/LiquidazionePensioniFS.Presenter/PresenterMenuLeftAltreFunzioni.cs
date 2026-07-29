using System;
using System.ServiceModel;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterMenuLeftAltreFunzioni
    {
        public void GetAbilitazioniAltreFunzioni(IMenuLeftAltreFunzioni iMenuAltreFunzioni)
        {
            Presenter.SvrLiquidazione.AreaEsito esito = new Presenter.SvrLiquidazione.AreaEsito();
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new SvrLiquidazione.ServizioLiquidazioneClient();
            try
            {
                AreaAltreFunzioni altreFunzioni = null;
                esito = objWS.GetAltreFunzioniByMatricola(out altreFunzioni, Utility.GetMatricolaOperatore());
                if (altreFunzioni == null)
                    altreFunzioni = new AreaAltreFunzioni();
                iMenuAltreFunzioni.AltreFunzioni = altreFunzioni;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMenuLeftAltreFunzioni, Errore nel metodo GetAbilitazioniAltreFunzioni");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
