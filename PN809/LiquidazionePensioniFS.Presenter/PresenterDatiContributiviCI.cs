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
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterDatiContributiviCI
    {
        public void GetDatiContributivi(IDatiContributiviCi datiContributiviCi)
        {
            AreaDatiContributivi areaDatiContributivi = new AreaDatiContributivi();

            short sedeOperatore = Utility.GetSedeOperatore();
            short centroOperativoOperatore = Utility.GetCentroOperativoOperatore();
            string matricola = Utility.GetMatricolaOperatore();
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(datiContributiviCi.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = datiContributiviCi.domanda.ProgStorico;
            AreaEsito risultatoGetDatiContributiviCiByDomanda = new AreaEsito();
            Presenter.SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWS = new ServizioLiquidazioneCiClient();
            try
            {
                risultatoGetDatiContributiviCiByDomanda = objWS.GetDatiContributiviByDomanda(out areaDatiContributivi, areaRichiestaDomanda, matricola, sedeOperatore, centroOperativoOperatore);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviCI, Errore nel metodo GetDatiContributivi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoGetDatiContributiviCiByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributiviCi.HasError = true;
                datiContributiviCi.ErrorMessage = risultatoGetDatiContributiviCiByDomanda.Messaggio;
                return;
            }
            else
            {
                datiContributiviCi.HasError = false;
                datiContributiviCi.ErrorMessage = "";
                datiContributiviCi.areaDatiContributiviCi = areaDatiContributivi;
                //this.areaDatiContributiviCi = areaDatiContributivi;
                return;
            }
        }        

        public void SalvaDatiContributiviCi(IDatiContributiviCi datiContributiviCi)
        {
            string sErrore;
            ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(datiContributiviCi.domanda.NumeroDomanda);
                short sedeOperatore = Utility.GetSedeOperatore();
                short centroOperativoOperatore = Utility.GetCentroOperativoOperatore();
                string matricola = Utility.GetMatricolaOperatore();
                AreaDatiContributivi areaDatiContributiviCi = datiContributiviCi.areaDatiContributiviCi;
                esitoCi = objWSCi.StoreDatiContributivi(ndomus, matricola, sedeOperatore, centroOperativoOperatore, ref areaDatiContributiviCi);
                datiContributiviCi.areaDatiContributiviCi = areaDatiContributiviCi;

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    datiContributiviCi.HasError = true;
                    datiContributiviCi.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviCI, Errore nel metodo SalvaDatiContributiviCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        #region ProRata

        public void SalvaTabProrata(IDatiContributiviCi datiContributiviCi)
        {
            string sErrore;
            ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(datiContributiviCi.domanda.NumeroDomanda);
                short sedeOperatore = Utility.GetSedeOperatore();
                short centroOperativoOperatore = Utility.GetCentroOperativoOperatore();
                string matricola = Utility.GetMatricolaOperatore();
                AreaDatiContributivi areaDatiContributiviCi = datiContributiviCi.areaDatiContributiviCi;
                esitoCi = objWSCi.StoreDatiProRata(ndomus, matricola, sedeOperatore, centroOperativoOperatore, ref areaDatiContributiviCi);
                datiContributiviCi.areaDatiContributiviCi = areaDatiContributiviCi;

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    datiContributiviCi.HasError = true;
                    datiContributiviCi.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviCI, Errore nel metodo SalvaTabProrata");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        public void EliminaDatiProRata(IDatiContributiviCi datiContributiviCi)
        {
            ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                short sedeOperatore = Utility.GetSedeOperatore();
                short centroOperativoOperatore = Utility.GetCentroOperativoOperatore();
                string matricola = Utility.GetMatricolaOperatore();
                long ndomus = Int64.Parse(datiContributiviCi.domanda.NumeroDomanda);

                esitoCi = objWSCi.CancelProRata(ndomus, matricola, sedeOperatore, centroOperativoOperatore);
                if (esitoCi.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    datiContributiviCi.HasError = true;
                    datiContributiviCi.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviCI, Errore nel metodo EliminaDatiProRata");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        #endregion ProRata

        #region Maternità / Acna

        public void SalvaTabMaternitaAcnaCi(IDatiContributiviCi datiContributiviCi)
        {
            string sErrore;
            ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(datiContributiviCi.domanda.NumeroDomanda);

                esitoCi = objWSCi.StoreDatiMaternitaAcna(ndomus, datiContributiviCi.areaDatiContributiviCi);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    datiContributiviCi.HasError = true;
                    datiContributiviCi.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviCI, Errore nel metodo SalvaTabMaternitaAcnaCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        public void EliminaTabMaternitaAcnaCi(IDatiContributiviCi datiContributiviCi)
        {
            string sErrore;
            ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(datiContributiviCi.domanda.NumeroDomanda);

                esitoCi = objWSCi.CancelDatiMaternitaAcna(ndomus);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    datiContributiviCi.HasError = true;
                    datiContributiviCi.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviCI, Errore nel metodo EliminaTabMaternitaAcnaCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        #endregion Maternità / Acna

        #region Importi Esteri

        public void SalvaTabImportiEsteriCi(IDatiContributiviCi datiContributiviCi)
        {
            string sErrore;
            ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(datiContributiviCi.domanda.NumeroDomanda);

                esitoCi = objWSCi.StoreDatiImportiEsteri(ndomus, datiContributiviCi.areaDatiContributiviCi);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    datiContributiviCi.HasError = true;
                    datiContributiviCi.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviCI, Errore nel metodo SalvaTabImportiEsteriCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        public void EliminaTabImportiEsteriCi(IDatiContributiviCi datiContributiviCi)
        {
            string sErrore;
            ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(datiContributiviCi.domanda.NumeroDomanda);

                esitoCi = objWSCi.CancelDatiImportiEsteri(ndomus);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    datiContributiviCi.HasError = true;
                    datiContributiviCi.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviCI, Errore nel metodo EliminaTabImportiEsteriCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        #endregion Importi Esteri

        #region Dati Calcolo

        public void SalvaTabDatiCalcoloCi(IDatiContributiviCi datiContributiviCi)
        {
            ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(datiContributiviCi.domanda.NumeroDomanda);

                esitoCi = objWSCi.StoreDatiCalcolo(ndomus, datiContributiviCi.areaDatiContributiviCi);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    datiContributiviCi.HasError = true;
                    datiContributiviCi.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviCI, Errore nel metodo SalvaTabDatiCalcoloCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        public void EliminaTabDatiCalcoloCi(IDatiContributiviCi datiContributiviCi)
        {
            ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(datiContributiviCi.domanda.NumeroDomanda);

                esitoCi = objWSCi.CancelDatiCalcolo(ndomus);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    datiContributiviCi.HasError = true;
                    datiContributiviCi.ErrorMessage = esitoCi.Messaggio;
                }
                else if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.OK && !string.IsNullOrEmpty(esitoCi.Messaggio))
                {
                    datiContributiviCi.HasError = false;
                    datiContributiviCi.ErrorMessage = esitoCi.Messaggio;
                }
                else if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.OK)
                {
                    datiContributiviCi.HasError = false;
                    datiContributiviCi.ErrorMessage = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviCI, Errore nel metodo EliminaTabDatiCalcoloCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        #endregion Dati Calcolo

        #region Dati Post Dec Originaria
        public void SalvaTabDatiPostDecOriginariaCi(IDatiContributiviCi datiContributiviCi)
        {
            string sErrore;
            ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(datiContributiviCi.domanda.NumeroDomanda);

                esitoCi = objWSCi.StoreDatiPostDecOriginaria(ndomus, datiContributiviCi.areaDatiContributiviCi);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    datiContributiviCi.HasError = true;
                    datiContributiviCi.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviCI, Errore nel metodo SalvaTabDatiPostDecOriginariaCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        public void EliminaTabDatiPostDecOriginariaCi(IDatiContributiviCi datiContributiviCi)
        {
            string sErrore;
            ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(datiContributiviCi.domanda.NumeroDomanda);

                esitoCi = objWSCi.CancelDatiPostDecOriginaria(ndomus);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    datiContributiviCi.HasError = true;
                    datiContributiviCi.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviCI, Errore nel metodo EliminaTabDatiPostDecOriginariaCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }
        #endregion Dati Post Dec Originaria

        #region IntegrazioneVirtuale

        public void SalvaTabIntegrazioneVirtuale(IDatiContributiviCi datiContributiviCi)
        {
            string sErrore;
            ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(datiContributiviCi.domanda.NumeroDomanda);
                AreaDatiContributivi areaDatiContributiviCi = datiContributiviCi.areaDatiContributiviCi;
                esitoCi = objWSCi.StoreRedditiPerIntegrazioneVirtuale(ndomus, ref areaDatiContributiviCi);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    datiContributiviCi.HasError = true;
                    datiContributiviCi.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviCI, Errore nel metodo SalvaTabIntegrazioneVirtuale");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        public void EliminaTabIntegrazioneVirtuale(IDatiContributiviCi datiContributiviCi)
        {
            string sErrore;
            ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            AreaDatiContributivi areaDatiContributivi = new AreaDatiContributivi();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(datiContributiviCi.domanda.NumeroDomanda);

                esitoCi = objWSCi.CancelRedditiPerIntegrazioneVirtuale(out areaDatiContributivi, ndomus);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    datiContributiviCi.HasError = true;
                    datiContributiviCi.ErrorMessage = esitoCi.Messaggio;
                }
                else
                {
                    datiContributiviCi.HasError = false;
                    datiContributiviCi.ErrorMessage = "";
                    datiContributiviCi.areaDatiContributiviCi = areaDatiContributivi;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributiviCI, Errore nel metodo EliminaTabIntegrazioneVirtuale");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        #endregion IntegrazioneVirtuale
    }
}
