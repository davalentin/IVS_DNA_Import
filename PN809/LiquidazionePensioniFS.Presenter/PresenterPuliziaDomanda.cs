using System;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterPuliziaDomanda
    {
        public void GetDomandaWebDom(IPuliziaDomanda iPulisciDomanda)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaPuliziaDomanda areaPuliziaDomanda = null;
            try
            {
                AreaEsito esito = objWS.GetPuliziaDomandaByDomanda(out areaPuliziaDomanda, iPulisciDomanda.numeroDomanda, Utility.GetSedeOperatore(), Utility.GetCentroOperativoOperatore(), 
                    iPulisciDomanda.TipoAppOperatore, iPulisciDomanda.RuoloOperatore);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    iPulisciDomanda.HasError = true;
                }
                iPulisciDomanda.ErrorMessage = esito.Messaggio;
                iPulisciDomanda.areaPuliziaDomanda = areaPuliziaDomanda;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterPuliziaDomanda, Errore nel metodo GetDomandaWebDom");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void PulisciDomanda(IPuliziaDomanda iPulisciDomanda)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaPuliziaDomanda areaPuliziaDomanda = null;
            try
            {
                string matricola = Utility.GetMatricolaOperatore();

                AreaEsito esito = objWS.EseguiPuliziaDomandaByDomanda(out areaPuliziaDomanda, iPulisciDomanda.numeroDomanda, Utility.GetMatricolaOperatore(), Utility.GetSedeOperatore(), 
                    Utility.GetCentroOperativoOperatore(), iPulisciDomanda.TipoAppOperatore, iPulisciDomanda.RuoloOperatore);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    iPulisciDomanda.HasError = true;
                    iPulisciDomanda.ErrorMessage = esito.Messaggio;
                    return;
                }
                iPulisciDomanda.areaPuliziaDomanda = areaPuliziaDomanda;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterPuliziaDomanda, Errore nel metodo PulisciDomanda");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
