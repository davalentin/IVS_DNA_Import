using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterContribuzioneEnpals
    {

        public void SalvaContribuzioneEnpalsByDomanda(ICrossContribuzioneEnpals iContribEnpals)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                long numeroDomanda = long.Parse(iContribEnpals.domanda.NumeroDomanda);

                Presenter.SvrLiquidazione.AreaEsito esito = new AreaEsito();

                esito = objWS.StoreDatiContributiviEnpals(numeroDomanda, iContribEnpals.DatiContribuzioneEnpals);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    iContribEnpals.HasError = true;
                    iContribEnpals.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterContribuzioneEnpals, Errore nel metodo SalvaContribuzioneEnpalsByDomanda");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }


        public void GetContribuzioneEnpalsByDomanda(ICrossContribuzioneEnpals iContribEnpals)
        {
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(iContribEnpals.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = iContribEnpals.domanda.ProgStorico;
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                DatiContribuzioneEnpals entity;
                Presenter.SvrLiquidazione.AreaEsito esito = new AreaEsito();
                
                esito = objWS.GetDatiContributiviEnpalsByDomanda(out entity, areaRichiestaDomanda, iContribEnpals.Tipologia);
                
                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    iContribEnpals.HasError = true;
                    iContribEnpals.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterContribuzioneEnpals, Errore nel metodo GetContribuzioneEnpalsByDomanda");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
