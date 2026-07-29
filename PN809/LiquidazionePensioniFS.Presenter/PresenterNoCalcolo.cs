using System;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterNoCalcolo
    {
        public void GetRecordNoCalcolo(IDatiNoCalcolo iDatiNoCalcolo)
        {
            string sErrore;
            AreaNoCalcolo areaNoCalcolo = new AreaNoCalcolo();
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
                areaRichiestaDomanda.NumeroDomanda = Int64.Parse(iDatiNoCalcolo.domanda.NumeroDomanda);
                areaRichiestaDomanda.ProgStorico = iDatiNoCalcolo.domanda.ProgStorico;
                esito = objWS.GetQuadroDatiRecordNoCalcoloByDomanda(out areaNoCalcolo, areaRichiestaDomanda);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esito.Messaggio;
                    iDatiNoCalcolo.HasError = true;
                    iDatiNoCalcolo.ErrorMessage = esito.Messaggio;
                }

                iDatiNoCalcolo.AreaNoCalcolo = areaNoCalcolo;
                iDatiNoCalcolo.IdRecordNoCalcolo = areaNoCalcolo.IdRecordNoCalcolo.GetValueOrDefault();
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterNoCalcolo, Errore nel metodo GetRecordNoCalcolo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void AddRecordNoCalcolo(IDatiNoCalcolo iDatiNoCalcolo)
        {
            string sErrore;
            AreaNoCalcolo areaNoCalcolo = new AreaNoCalcolo();
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = long.Parse(iDatiNoCalcolo.domanda.NumeroDomanda);
                esito = objWS.AddRecordNoCalcoloByDomanda(out areaNoCalcolo, ndomus);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esito.Messaggio;
                    iDatiNoCalcolo.HasError = true;
                    iDatiNoCalcolo.ErrorMessage = esito.Messaggio;
                    return;
                }
                iDatiNoCalcolo.AreaNoCalcolo = areaNoCalcolo;
                iDatiNoCalcolo.IdRecordNoCalcolo = areaNoCalcolo.IdRecordNoCalcolo.Value;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterNoCalcolo, Errore nel metodo AddRecordNoCalcolo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void GetDatiNoCalcoloByIdRecord(IDatiNoCalcolo iDatiNoCalcolo)
        {
            string sErrore;
            AreaNoCalcolo areaNoCalcolo = new AreaNoCalcolo();
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
                areaRichiestaDomanda.NumeroDomanda = Int64.Parse(iDatiNoCalcolo.domanda.NumeroDomanda);
                areaRichiestaDomanda.ProgStorico = iDatiNoCalcolo.domanda.ProgStorico;
                long idRecord = iDatiNoCalcolo.IdRecordNoCalcolo;
                esito = objWS.GetDatiNoCalcoloByIdRecord(out areaNoCalcolo, areaRichiestaDomanda, idRecord);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esito.Messaggio;
                    iDatiNoCalcolo.HasError = true;
                    iDatiNoCalcolo.ErrorMessage = esito.Messaggio;
                }
                iDatiNoCalcolo.AreaNoCalcolo = areaNoCalcolo;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterNoCalcolo, Errore nel metodo GetDatiNoCalcoloByIdRecord");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void StoreDatiNoCalcoloByIdRecord(IDatiNoCalcolo iDatiNoCalcolo)
        {
            string sErrore;
            AreaNoCalcolo areaNoCalcolo = iDatiNoCalcolo.AreaNoCalcolo;
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = long.Parse(iDatiNoCalcolo.domanda.NumeroDomanda);
                long idRecord = iDatiNoCalcolo.IdRecordNoCalcolo;
                esito = objWS.StoreDatiNoCalcolo(ndomus, idRecord,ref areaNoCalcolo);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esito.Messaggio;
                    iDatiNoCalcolo.HasError = true;
                    iDatiNoCalcolo.ErrorMessage = esito.Messaggio;
                }
                iDatiNoCalcolo.AreaNoCalcolo = areaNoCalcolo;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterNoCalcolo, Errore nel metodo StoreDatiNoCalcoloByIdRecord");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void DeleteDatiNoCalcoloByIdRecord(IDatiNoCalcolo iDatiNoCalcolo)
        {
            string sErrore;
            AreaNoCalcolo areaNoCalcolo = iDatiNoCalcolo.AreaNoCalcolo;
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = long.Parse(iDatiNoCalcolo.domanda.NumeroDomanda);
                long idRecord = iDatiNoCalcolo.IdRecordNoCalcolo;
                esito = objWS.DeleteDatiNoCalcolo(out areaNoCalcolo,ndomus, idRecord);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esito.Messaggio;
                    iDatiNoCalcolo.HasError = true;
                    iDatiNoCalcolo.ErrorMessage = esito.Messaggio;
                }
                iDatiNoCalcolo.AreaNoCalcolo = areaNoCalcolo;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterNoCalcolo, Errore nel metodo DeleteDatiNoCalcoloByIdRecord");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void DeleteRecordNoCalcolo(IDatiNoCalcolo iDatiNoCalcolo)
        {
            string sErrore;
            AreaNoCalcolo areaNoCalcolo = new AreaNoCalcolo();
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = long.Parse(iDatiNoCalcolo.domanda.NumeroDomanda);
                long idRecordNoCalcolo = iDatiNoCalcolo.IdRecordNoCalcolo;
                esito = objWS.CancelRecordDatiNoCalcolo(out areaNoCalcolo, ndomus, idRecordNoCalcolo);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esito.Messaggio;
                    iDatiNoCalcolo.HasError = true;
                    iDatiNoCalcolo.ErrorMessage = esito.Messaggio;
                }

                iDatiNoCalcolo.AreaNoCalcolo = areaNoCalcolo;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterNoCalcolo, Errore nel metodo DeleteRecordNoCalcolo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void DeleteAllRecordNoCalcolo(IDatiNoCalcolo iDatiNoCalcolo)
        {
            string sErrore;
            AreaNoCalcolo areaNoCalcolo = new AreaNoCalcolo();
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = long.Parse(iDatiNoCalcolo.domanda.NumeroDomanda);
                esito = objWS.CancelAllRecordDatiNoCalcolo(out areaNoCalcolo, ndomus);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esito.Messaggio;
                    iDatiNoCalcolo.HasError = true;
                    iDatiNoCalcolo.ErrorMessage = esito.Messaggio;
                }

                iDatiNoCalcolo.AreaNoCalcolo = areaNoCalcolo;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterNoCalcolo, Errore nel metodo DeleteAllRecordNoCalcolo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
