using System;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterOneri
    {
        public void GetAreaOneri(IOneri oneri)
        {
            SvrLiquidazione.AreaOneri areaOneri = new SvrLiquidazione.AreaOneri();
            Presenter.SvrLiquidazione.AreaEsito esito = new Presenter.SvrLiquidazione.AreaEsito();
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(oneri.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = oneri.domanda.ProgStorico;

            try
            {
                esito = objWS.GetOneriByDomanda(out areaOneri, areaRichiestaDomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterOneri, Errore nel metodo GetAreaOneri");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
            {
                oneri.HasError = true;
                oneri.ErrorMessage = "Errore tecnico nel recupero dei dati del quadro Oneri";
            }
            else
                oneri.areaOneri = areaOneri;
        }

        public void SalvaQuadroOneri(IOneri oneri)
        {
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                SvrLiquidazione.AreaEsito esito = new SvrLiquidazione.AreaEsito();
                long ndomus = Int64.Parse(oneri.domanda.NumeroDomanda);
                esito = objWS.StoreOneri(ndomus, oneri.areaOneri);

                if (esito.RisultatoOperazione == SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    oneri.HasError = true;
                    oneri.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterOneri, Errore nel metodo SalvaQuadroOneri");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        #region Dati Oneri - Benefici Particolari

        public void SalvaOneriBeneficiParticolari(IOneri oneri)
        {
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                SvrLiquidazione.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito();
                long ndomus = Int64.Parse(oneri.domanda.NumeroDomanda);
                esito = objWS.StoreDatiOneriBeneficiParticolari(ndomus, oneri.areaOneri);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    oneri.HasError = true;
                    oneri.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterOneri, Errore nel metodo SalvaOneriBeneficiParticolari");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaOneriBeneficiParticolari(IOneri oneri)
        {
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                SvrLiquidazione.AreaOneri areaOneri = null;
                SvrLiquidazione.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito();
                long ndomus = Int64.Parse(oneri.domanda.NumeroDomanda);
                esito = objWS.CancelDatiOneriBeneficiParticolari(out areaOneri, ndomus);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    oneri.HasError = true;
                    oneri.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterOneri, Errore nel metodo EliminaOneriBeneficiParticolari");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        #endregion Dati Oneri - Benefici Particolari

        #region Dati Prepensionamento
        public void SalvaPrepensionamento(IOneri oneri)
        {
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                SvrLiquidazione.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito();
                long ndomus = Int64.Parse(oneri.domanda.NumeroDomanda);
                esito = objWS.StoreDatiPrepensionamento(ndomus, oneri.areaOneri);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    oneri.HasError = true;
                    oneri.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterOneri, Errore nel metodo SalvaPrepensionamento");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaPrepensionamento(IOneri oneri)
        {
            string sErrore;
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                SvrLiquidazione.AreaOneri areaOneri = null;
                SvrLiquidazione.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito();
                long ndomus = Int64.Parse(oneri.domanda.NumeroDomanda);
                esito = objWS.CancelDatiPrepensionamento(out areaOneri, ndomus);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esito.Messaggio;
                    oneri.HasError = true;
                    oneri.ErrorMessage = esito.Messaggio;
                }

                if (areaOneri != null)
                    oneri.areaOneri = areaOneri;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterOneri, Errore nel metodo EliminaPrepensionamento");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
        #endregion Dati Prepensionamento
    }
}
