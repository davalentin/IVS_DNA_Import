using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterEliminazione
    {
        public void EliminaDatiEliminazione(IEliminazione datiEliminazione)
        {
            string sErrore;
            AreaEliminazione areaEliminazione = new AreaEliminazione();
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.ServizioLiquidazioneClient();
            try
            {
                SvrLiquidazione.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito();
                long ndomus = Int64.Parse(datiEliminazione.domanda.NumeroDomanda);
                esito = objWS.DeleteDatiEliminazione(ndomus);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esito.Messaggio;
                    datiEliminazione.HasError = true;
                    datiEliminazione.ErrorMessage = esito.Messaggio;
                }

                datiEliminazione.areaEliminazione = areaEliminazione;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterEliminazione, Errore nel metodo EliminaDatiEliminazione");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void GetDatiEliminazione(IEliminazione DatiEliminazione)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaEliminazione areaEliminazione;
                Presenter.SvrLiquidazione.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito();
                AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
                areaRichiestaDomanda.NumeroDomanda = Int64.Parse(DatiEliminazione.domanda.NumeroDomanda);
                areaRichiestaDomanda.ProgStorico = DatiEliminazione.domanda.ProgStorico;
                esito = objWS.GetEliminazioneByDomanda(out areaEliminazione, areaRichiestaDomanda);
                DatiEliminazione.areaEliminazione = areaEliminazione;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterEliminazione, Errore nel metodo GetDatiEliminazione");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaDatiEliminazione(IEliminazione eliminazione)
        {
            string sErrore;
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.ServizioLiquidazioneClient();
            try
            {
                SvrLiquidazione.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito();
                long ndomus = Int64.Parse(eliminazione.domanda.NumeroDomanda);
                esito = objWS.StoreDatiEliminazione(ndomus, eliminazione.areaEliminazione);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esito.Messaggio;
                    eliminazione.HasError = true;
                    eliminazione.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterEliminazione, Errore nel metodo SalvaDatiEliminazione");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void GetDatiDanteCausa(IEliminazione DatiEliminazione, out AreaDanteCausa datiDanteCausa)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            datiDanteCausa = new AreaDanteCausa();
            try
            {
                Presenter.SvrLiquidazione.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito();
                Presenter.SvrLiquidazione.AreaRichiestaDomanda areaRichiestaDomanda = new Presenter.SvrLiquidazione.AreaRichiestaDomanda();
                areaRichiestaDomanda.NumeroDomanda = Int64.Parse(DatiEliminazione.domanda.NumeroDomanda);
                areaRichiestaDomanda.ProgStorico = DatiEliminazione.domanda.ProgStorico;

                esito = objWS.GetDanteCausaByDomanda(out datiDanteCausa, areaRichiestaDomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterEliminazione, Errore nel metodo GetDatiEliminazione");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
