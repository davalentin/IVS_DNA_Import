using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.DNA;
using INPS.DNA.Logging;
using INPS.DNA.Services;
using INPS.DNA.Services.FaultContract;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterBititolarita
    {
        #region AGO
        public void GetBititolaritaAgo(IBititolarita bititolaritaAgo)
        {
            SvrLiquidazioneAgo.AreaDatiBititolarita areaDatiBititolaritaAgo = new SvrLiquidazioneAgo.AreaDatiBititolarita();
            Presenter.SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
            Presenter.SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAGO = new Presenter.SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            Presenter.SvrLiquidazioneAgo.AreaRichiestaDomanda areaRichiestaDomanda = new Presenter.SvrLiquidazioneAgo.AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(bititolaritaAgo.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = bititolaritaAgo.domanda.ProgStorico;

            try
            {
                esitoAgo = objWSAGO.GetBititolaritaByDomanda(out areaDatiBititolaritaAgo, areaRichiestaDomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterBititolarita - errore nel metodo GetBititolaritaAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAGO);
            }

            if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
            {
                bititolaritaAgo.HasError = true;
                bititolaritaAgo.ErrorMessage = "Errore tecnico nel recupero dei dati del quadro BititolaritàAgo";
            }
            else
            {
                bititolaritaAgo.areaDatiBititolaritaAgo = areaDatiBititolaritaAgo;
            }
        }

        public void SalvaBititolaritaAgo(IBititolarita bititolaritaAgo)
        {
            string sErrore;
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(bititolaritaAgo.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.StoreBititolarita(ndomus, bititolaritaAgo.areaDatiBititolaritaAgo);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoAgo.Messaggio;
                    bititolaritaAgo.HasError = true;
                    bititolaritaAgo.ErrorMessage = esitoAgo.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterBititolarita, Errore nel metodo SalvaBititolaritaAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }

        #region Altre Pensioni
        public void SalvaAltrePensioniAgo(IBititolarita bititolaritaAgo)
        {
            string sErrore;
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(bititolaritaAgo.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.StoreAltraPensione(ndomus, bititolaritaAgo.areaDatiBititolaritaAgo);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoAgo.Messaggio;
                    bititolaritaAgo.HasError = true;
                    bititolaritaAgo.ErrorMessage = esitoAgo.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterBititolarita, Errore nel metodo SalvaAltrePensioniAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }

        public void EliminaAltrePensioniAgo(IBititolarita bititolaritaAgo)
        {
            SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
            SvrLiquidazioneAgo.AreaDatiBititolarita areaDatiBititolarita = new SvrLiquidazioneAgo.AreaDatiBititolarita();
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            try
            {
                long ndomus = Int64.Parse(bititolaritaAgo.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.CancelAltraPensione(out areaDatiBititolarita, ndomus);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterBititolarita, Errore nel metodo EliminaAltrePensioniAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }

            if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.OK)
            {
                bititolaritaAgo.HasError = false;
                bititolaritaAgo.ErrorMessage = esitoAgo.Messaggio;
                bititolaritaAgo.areaDatiBititolaritaAgo = areaDatiBititolarita;
            }

            if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
            {
                bititolaritaAgo.HasError = true;
                bititolaritaAgo.ErrorMessage = esitoAgo.Messaggio;
            }
        }
        #endregion Altre Pensioni
        #endregion AGO

        #region CI
        public void GetBititolaritaCi(IBititolarita bititolaritaCi)
        {
            SvrLiquidazioneCi.AreaDatiBititolarita areaDatiBititolaritaCi = new SvrLiquidazioneCi.AreaDatiBititolarita();
            Presenter.SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
            Presenter.SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCI = new Presenter.SvrLiquidazioneCi.ServizioLiquidazioneCiClient();
            Presenter.SvrLiquidazioneCi.AreaRichiestaDomanda areaRichiestaDomanda = new Presenter.SvrLiquidazioneCi.AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(bititolaritaCi.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = bititolaritaCi.domanda.ProgStorico;
            try
            {
                esitoCi = objWSCI.GetBititolaritaByDomanda(out areaDatiBititolaritaCi, areaRichiestaDomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterBititolarita - errore nel metodo GetBititolaritaCi");
            }
            finally
            {
                Utility.CloseClient(objWSCI);
            }

            if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
            {
                bititolaritaCi.HasError = true;
                bititolaritaCi.ErrorMessage = "Errore tecnico nel recupero dei dati del quadro BititolaritàCi";
            }
            else
            {
                bititolaritaCi.areaDatiBititolaritaCi = areaDatiBititolaritaCi;
            }
        }

        public void SalvaBititolaritaCi(IBititolarita bititolaritaCi)
        {
            string sErrore;
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCI = new SvrLiquidazioneCi.ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(bititolaritaCi.domanda.NumeroDomanda);
                esitoCi = objWSCI.StoreBititolarita(ndomus, bititolaritaCi.areaDatiBititolaritaCi);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    bititolaritaCi.HasError = true;
                    bititolaritaCi.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterBititolarita, Errore nel metodo SalvaBititolaritaCi");
            }
            finally
            {
                Utility.CloseClient(objWSCI);
            }
        }

        #region Altre Pensioni
        public void SalvaAltrePensioniCi(IBititolarita bititolaritaCi)
        {
            string sErrore;
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCI = new SvrLiquidazioneCi.ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(bititolaritaCi.domanda.NumeroDomanda);
                esitoCi = objWSCI.StoreAltraPensione(ndomus, bititolaritaCi.areaDatiBititolaritaCi);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    bititolaritaCi.HasError = true;
                    bititolaritaCi.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterBititolarita, Errore nel metodo SalvaAltrePensioniCi");
            }
            finally
            {
                Utility.CloseClient(objWSCI);
            }
        }

        public void EliminaAltrePensioniCi(IBititolarita bititolaritaCi)
        {
            SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
            SvrLiquidazioneCi.AreaDatiBititolarita areaDatiBititolarita = new SvrLiquidazioneCi.AreaDatiBititolarita();
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCI = new SvrLiquidazioneCi.ServizioLiquidazioneCiClient();
            try
            {
                long ndomus = Int64.Parse(bititolaritaCi.domanda.NumeroDomanda);
                esitoCi = objWSCI.CancelAltraPensione(out areaDatiBititolarita, ndomus);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterBititolarita, Errore nel metodo EliminaAltrePensioniCi");
            }
            finally
            {
                Utility.CloseClient(objWSCI);
            }

            if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.OK)
            {
                bititolaritaCi.HasError = false;
                bititolaritaCi.ErrorMessage = esitoCi.Messaggio;
                bititolaritaCi.areaDatiBititolaritaCi = areaDatiBititolarita;
            }

            if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
            {
                bititolaritaCi.HasError = true;
                bititolaritaCi.ErrorMessage = esitoCi.Messaggio;
            }
        }
        #endregion Altre Pensioni
        #endregion CI
    }
}
