using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using INPS.DNA;
using INPS.DNA.Logging;
using INPS.DNA.Services;
using INPS.DNA.Services.FaultContract;

using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterDatiContributiviAGO
    {
        public bool GetDatiContributivi(IDatiContributiviAgo datiContributivi)
        {
            bool isDataFromDb = false;

            short sedeOperatore = Utility.GetSedeOperatore();
            short centroOperativoOperatore = Utility.GetCentroOperativoOperatore();
            string matricola = Utility.GetMatricolaOperatore();
            AreaDatiContributivi areaDatiContributivi = new AreaDatiContributivi();
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = datiContributivi.domanda.ProgStorico;
            AreaEsito risultatoGetDatiContributiviByDomanda = null;

            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();
            try
            {
                risultatoGetDatiContributiviByDomanda = objWS.GetDatiContributiviByDomanda(out areaDatiContributivi, out isDataFromDb, areaRichiestaDomanda, matricola, sedeOperatore, centroOperativoOperatore);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviAGO, Errore nel metodo GetDatiContributivi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoGetDatiContributiviByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = risultatoGetDatiContributiviByDomanda.Messaggio;
                //return;
            }
            else
            {
                datiContributivi.HasError = false;
                datiContributivi.ErrorMessage = "";
                GestioneContribTipoCalcolo tipoCalcoloPrecedente = GestioneContribTipoCalcolo.NonValido;

                if (datiContributivi != null && datiContributivi.areaDatiContributiviAgo != null && datiContributivi.areaDatiContributiviAgo.DatiCalcolo != null)
                    tipoCalcoloPrecedente = datiContributivi.areaDatiContributiviAgo.DatiCalcolo.TipoCalcolo;

                datiContributivi.areaDatiContributiviAgo = areaDatiContributivi;

                if (datiContributivi != null && datiContributivi.areaDatiContributiviAgo != null && datiContributivi.areaDatiContributiviAgo.DatiCalcolo != null &&
                    datiContributivi.areaDatiContributiviAgo.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.NonValido)
                    datiContributivi.areaDatiContributiviAgo.DatiCalcolo.TipoCalcolo = tipoCalcoloPrecedente;
                //return;
            }

            return isDataFromDb;
        }

        public void SalvaDatiContributivi(IDatiContributiviAgo datiContributivi)
        {
            Int64 numeroDomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaEsito esito = new AreaEsito();

            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();
            try
            {
                AreaDatiContributivi areaDatiContrib = datiContributivi.areaDatiContributiviAgo;
                short sedeOperatore = Utility.GetSedeOperatore();
                short centroOperativoOperatore = Utility.GetCentroOperativoOperatore();
                string matricola = Utility.GetMatricolaOperatore();
                esito = objWS.StoreDatiContributiviByDomanda(numeroDomanda, matricola, sedeOperatore, centroOperativoOperatore, ref areaDatiContrib);
                datiContributivi.areaDatiContributiviAgo = areaDatiContrib;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviAGO, Errore nel metodo SalvaDatiContributivi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = esito.Messaggio;
                return;
            }
            else
            {
                datiContributivi.HasError = false;
                datiContributivi.ErrorMessage = "";
                return;
            }
        }

        public void SalvaDatiCalcolo(IDatiContributiviAgo datiContributivi)
        {
            Int64 numeroDomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaEsito esito = new AreaEsito();

            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();
            try
            {
                AreaDatiContributivi areaDatiContrib = datiContributivi.areaDatiContributiviAgo;
                short sedeOperatore = Utility.GetSedeOperatore();
                short centroOperativoOperatore = Utility.GetCentroOperativoOperatore();
                string matricola = Utility.GetMatricolaOperatore();
                esito = objWS.StoreDatiCalcoloByDomanda(numeroDomanda, matricola, sedeOperatore, centroOperativoOperatore, ref areaDatiContrib);
                datiContributivi.areaDatiContributiviAgo = areaDatiContrib;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviAGO, Errore nel metodo SalvaDatiCalcolo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = esito.Messaggio;
                return;
            }
            else
            {
                datiContributivi.HasError = false;
                datiContributivi.ErrorMessage = "";
                return;
            }
        }

        public void EliminaDatiCalcolo(IDatiContributiviAgo datiContributivi)
        {
            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();
            Int64 ndomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaEsito risultatoCancelDatiContributiviByDomanda = new AreaEsito();
            try
            {
                risultatoCancelDatiContributiviByDomanda = objWS.CancelDatiContributiviByDomanda(ndomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviAGO, Errore nel metodo EliminaDatiCalcolo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoCancelDatiContributiviByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = risultatoCancelDatiContributiviByDomanda.Messaggio;
                return;
            }
            else
            {
                datiContributivi.HasError = false;
                datiContributivi.ErrorMessage = "";
                datiContributivi.areaDatiContributiviAgo = new AreaDatiContributivi();
                return;
            }
        }

        public void SalvaDatiMiglioramenti(IDatiContributiviAgo datiContributivi)
        {
            Int64 numeroDomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaEsito esito = new AreaEsito();

            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();
            try
            {
                AreaDatiContributivi areaDatiContrib = datiContributivi.areaDatiContributiviAgo;
                short sedeOperatore = Utility.GetSedeOperatore();
                short centroOperativoOperatore = Utility.GetCentroOperativoOperatore();
                string matricola = Utility.GetMatricolaOperatore();
                esito = objWS.StoreDatiQuoteMiglioramentiContrattualiByDomanda(numeroDomanda, matricola, sedeOperatore, centroOperativoOperatore, ref areaDatiContrib);
                datiContributivi.areaDatiContributiviAgo = areaDatiContrib;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviAGO, Errore nel metodo SalvaDatiCalcolo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = esito.Messaggio;
                return;
            }
            else
            {
                datiContributivi.HasError = false;
                datiContributivi.ErrorMessage = "";
                return;
            }
        }

        public void SalvaDatiCalcoloVittime(IDatiContributiviAgo datiContributivi)
        {
            Int64 numeroDomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaEsito esito = new AreaEsito();

            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();
            try
            {
                AreaDatiContributivi areaDatiContrib = datiContributivi.areaDatiContributiviAgo;
                esito = objWS.StoreDatiVittimeTerrorismo(numeroDomanda, areaDatiContrib);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviAGO, Errore nel metodo SalvaDatiCalcoloVittime");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = esito.Messaggio;
                return;
            }
            else
            {
                datiContributivi.HasError = false;
                datiContributivi.ErrorMessage = "";
                return;
            }
        }

        public void EliminaDatiCalcoloVittime(IDatiContributiviAgo datiContributivi)
        {
            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();
            Int64 ndomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaEsito risultatoCancelDatiContributiviByDomanda = new AreaEsito();
            try
            {
                risultatoCancelDatiContributiviByDomanda = objWS.CancelDatiVittimeTerrorismo(ndomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviAGO, Errore nel metodo SalvaDatiCalcoloVittime");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoCancelDatiContributiviByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = risultatoCancelDatiContributiviByDomanda.Messaggio;
                return;
            }
            else
            {
                datiContributivi.HasError = false;
                datiContributivi.ErrorMessage = "";
                datiContributivi.areaDatiContributiviAgo = new AreaDatiContributivi();
                return;
            }
        }

        public void SalvaDatiCalcoloQuotaFondoIntegrativo(IDatiContributiviAgo datiContributivi)
        {
            Int64 numeroDomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaEsito esito = new AreaEsito();

            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();
            try
            {
                AreaDatiContributivi areaDatiContrib = datiContributivi.areaDatiContributiviAgo;
                esito = objWS.StoreDatiQuotaFondoIntegrativo(numeroDomanda, areaDatiContrib);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviAGO, Errore nel metodo SalvaDatiCalcoloQuotaFondoIntegrativo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = esito.Messaggio;
                return;
            }
            else
            {
                datiContributivi.HasError = false;
                datiContributivi.ErrorMessage = "";
                return;
            }
        }

        public void EliminaDatiCalcoloQuotaFondoIntegrativo(IDatiContributiviAgo datiContributivi)
        {
            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();
            Int64 ndomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaEsito risultatoCancelDatiContributiviByDomanda = new AreaEsito();
            try
            {
                risultatoCancelDatiContributiviByDomanda = objWS.CancelDatiQuotaFondoIntegrativo(ndomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviAGO, Errore nel metodo EliminaDatiCalcoloQuotaFondoIntegrativo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoCancelDatiContributiviByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = risultatoCancelDatiContributiviByDomanda.Messaggio;
                return;
            }
            else
            {
                datiContributivi.HasError = false;
                datiContributivi.ErrorMessage = "";
                datiContributivi.areaDatiContributiviAgo = new AreaDatiContributivi();
                return;
            }
        }

        public void SalvaDatiCalcoloQuotaFondoINPGI(IDatiContributiviAgo datiContributivi)
        {
            Int64 numeroDomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaEsito esito = new AreaEsito();

            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();
            try
            {
                AreaDatiContributivi areaDatiContrib = datiContributivi.areaDatiContributiviAgo;
                esito = objWS.StoreDatiQuotaFondoINPGI(numeroDomanda, areaDatiContrib);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviAGO, Errore nel metodo SalvaDatiCalcoloQuotaFondoINPGI");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = esito.Messaggio;
                return;
            }
            else
            {
                datiContributivi.HasError = false;
                datiContributivi.ErrorMessage = "";
                return;
            }
        }

        public void EliminaDatiCalcoloQuotaFondoINPGI(IDatiContributiviAgo datiContributivi)
        {
            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();
            Int64 ndomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaEsito risultatoCancelDatiContributiviByDomanda = new AreaEsito();
            try
            {
                risultatoCancelDatiContributiviByDomanda = objWS.CancelDatiQuotaFondoINPGI(ndomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviAGO, Errore nel metodo EliminaDatiCalcoloQuotaFondoINPGI");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoCancelDatiContributiviByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = risultatoCancelDatiContributiviByDomanda.Messaggio;
                return;
            }
            else
            {
                datiContributivi.HasError = false;
                datiContributivi.ErrorMessage = "";
                datiContributivi.areaDatiContributiviAgo = new AreaDatiContributivi();
                return;
            }
        }

        public void SalvaTabDatiEsteri(IDatiContributiviAgo datiContributivi)
        {
            string sErrore;
            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
                short sedeOperatore = Utility.GetSedeOperatore();
                short centroOperativoOperatore = Utility.GetCentroOperativoOperatore();
                string matricola = Utility.GetMatricolaOperatore();
                AreaDatiContributivi areaDatiContributiviAgo = datiContributivi.areaDatiContributiviAgo;
                esitoAgo = objWS.StoreDatiProRata(ndomus, matricola, sedeOperatore, centroOperativoOperatore, ref areaDatiContributiviAgo);
                datiContributivi.areaDatiContributiviAgo = areaDatiContributiviAgo;

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoAgo.Messaggio;
                    datiContributivi.HasError = true;
                    datiContributivi.ErrorMessage = esitoAgo.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviAGO, Errore nel metodo SalvaTabDatiEsteri");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaDatiEsteri(IDatiContributiviAgo datiContributivi)
        {
            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                short sedeOperatore = Utility.GetSedeOperatore();
                short centroOperativoOperatore = Utility.GetCentroOperativoOperatore();
                string matricola = Utility.GetMatricolaOperatore();
                long ndomus = Int64.Parse(datiContributivi.domanda.NumeroDomanda);

                esitoAgo = objWS.CancelProRata(ndomus, matricola, sedeOperatore, centroOperativoOperatore);
                if (esitoAgo.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    datiContributivi.HasError = true;
                    datiContributivi.ErrorMessage = esitoAgo.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviAGO, Errore nel metodo EliminaDatiEsteri");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaStatoEstero(long id, IDatiContributiviAgo datiContributivi)
        {
            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                //this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                long ndomus = Int64.Parse(datiContributivi.domanda.NumeroDomanda);

                esitoAgo = objWS.CancelProRataSingolo(id, ndomus);
                if (esitoAgo.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    datiContributivi.HasError = true;
                    datiContributivi.ErrorMessage = esitoAgo.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviAGO, Errore nel metodo EliminaDatiEsteri");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public AreaEsito RecuperaStatiEsteri(string stato, string codIstituzione, out string nomeStato, out string istituzione, out string descCittà, IDatiContributiviAgo datiContributivi)
        {
            nomeStato = string.Empty;
            istituzione = string.Empty;
            descCittà = string.Empty;
            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();
            AreaEsito esito = new AreaEsito();
           
            try
            {
                AreaDatiContributivi areaDatiContributiviAgo = datiContributivi.areaDatiContributiviAgo;
                esito = objWS.RecuperaStatiEsteri(out nomeStato, out istituzione, out descCittà, stato, codIstituzione, ref areaDatiContributiviAgo);
                datiContributivi.areaDatiContributiviAgo = areaDatiContributiviAgo;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviAGO, Errore nel metodo RecuperaStatiEsteri");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            return esito;
        }

        public AreaEsito ControlsCompatibilitàCodiceConvenzioneWithStatoEstero(IDatiContributiviAgo datiContributivi, GestioneContribStatoEsteroCumulo stato)
        {
            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();
            AreaEsito esito = new AreaEsito();

            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = datiContributivi.domanda.ProgStorico;
            try
            {
                esito = objWS.CompatibilitàCodiceConvenzioneWithStatoEstero(areaRichiestaDomanda, stato);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviAGO, Errore nel metodo ControlsCompatibilitàCodiceConvenzioneWithStatoEstero");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            return esito;
        }
    }
}
