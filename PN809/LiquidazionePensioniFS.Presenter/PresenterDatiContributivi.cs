using System;

using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;


namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterDatiContributivi
    {
        public void GetDatiContributivi(IDatiContributivi datiContributivi) 
        {
            AreaDatiContributivi areaDatiContributivi = new AreaDatiContributivi();
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = datiContributivi.domanda.ProgStorico;
            AreaEsito risultatoGetDatiContributiviByDomanda = null;

            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            try
            {
                risultatoGetDatiContributiviByDomanda = objWS.GetDatiContributiviByDomanda(out areaDatiContributivi, areaRichiestaDomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo GetDatiContributivi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoGetDatiContributiviByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = risultatoGetDatiContributiviByDomanda.Messaggio;

                GestioneContribTipoCalcolo tipoCalcoloPrecedente = GestioneContribTipoCalcolo.NonValido;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null && datiContributivi.areaDatiContributivi.DatiCalcolo != null)
                    tipoCalcoloPrecedente = datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo;
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null &&
                    datiContributivi.areaDatiContributivi.DatiCalcolo != null && datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.NonValido)
                    datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo = tipoCalcoloPrecedente;
                return;
            }
            else
            {
                datiContributivi.HasError = false;
                datiContributivi.ErrorMessage = "";
                GestioneContribTipoCalcolo tipoCalcoloPrecedente = GestioneContribTipoCalcolo.NonValido;
                if (areaDatiContributivi != null && areaDatiContributivi.DatiCalcolo != null)
                    tipoCalcoloPrecedente = areaDatiContributivi.DatiCalcolo.TipoCalcolo;
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                if (areaDatiContributivi != null && areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.NonValido)
                    datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo = tipoCalcoloPrecedente;

                return;
            }
        }

        public void SalvaDatiContributivi(IDatiContributivi datiContributivi)
        {
            AreaDatiContributivi areaDatiContributivi = new AreaDatiContributivi();
            Presenter.SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            Int64 ndomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaEsito risultatoSalvaDatiContributiviByDomanda = new AreaEsito();
            areaDatiContributivi = datiContributivi.areaDatiContributivi;
            try
            {
                risultatoSalvaDatiContributiviByDomanda = objWS.StoreDatiContributiviByDomanda(ndomanda, ref areaDatiContributivi);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo SalvaDatiContributivi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoSalvaDatiContributiviByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = risultatoSalvaDatiContributiviByDomanda.Messaggio;
                return;
            }
            else
            {
                datiContributivi.HasError = false;
                datiContributivi.ErrorMessage = "";
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                return;
            }
        }

        #region Dati Calcolo
        public void SalvaTabDatiCalcolo(IDatiContributivi datiContributivi)
        {
            AreaDatiContributivi areaDatiContributivi = new AreaDatiContributivi();
            Presenter.SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            Int64 ndomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaEsito risultatoSalvaDatiCalcoloByDomanda = new AreaEsito();
            areaDatiContributivi = datiContributivi.areaDatiContributivi;
            try
            {
                risultatoSalvaDatiCalcoloByDomanda = objWS.StoreDatiCalcoloByDomanda(ndomanda, areaDatiContributivi);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo SalvaTabDatiCalcolo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoSalvaDatiCalcoloByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = risultatoSalvaDatiCalcoloByDomanda.Messaggio;
                return;
            }
            else
            {
                datiContributivi.HasError = false;
                datiContributivi.ErrorMessage = "";
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                return;
            }
        }

        public void EliminaTabDatiCalcolo(IDatiContributivi datiContributivi)
        {
            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            Int64 ndomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaDatiContributivi areaDatiContributivi = new AreaDatiContributivi();
            AreaEsito risultatoCancelTabDatiCalcoloByDomanda = new AreaEsito();
            try
            {
                risultatoCancelTabDatiCalcoloByDomanda = objWS.CancelDatiCalcoloByDomanda(out areaDatiContributivi, ndomanda);

            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo EliminaTabDatiCalcolo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoCancelTabDatiCalcoloByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = risultatoCancelTabDatiCalcoloByDomanda.Messaggio;

                GestioneContribTipoCalcolo tipoCalcoloPrecedente = GestioneContribTipoCalcolo.NonValido;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null && datiContributivi.areaDatiContributivi.DatiCalcolo != null)
                    tipoCalcoloPrecedente = datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo;
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null &&
                    datiContributivi.areaDatiContributivi.DatiCalcolo != null && datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.NonValido)
                    datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo = tipoCalcoloPrecedente;
                return;
            }
            else
            {
                if (!String.IsNullOrEmpty(risultatoCancelTabDatiCalcoloByDomanda.Messaggio))
                    datiContributivi.ErrorMessage = risultatoCancelTabDatiCalcoloByDomanda.Messaggio;
                else
                    datiContributivi.ErrorMessage = "";
                datiContributivi.HasError = false;
                
                GestioneContribTipoCalcolo tipoCalcoloPrecedente = GestioneContribTipoCalcolo.NonValido;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null && datiContributivi.areaDatiContributivi.DatiCalcolo != null)
                    tipoCalcoloPrecedente = datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo;
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null &&
                    datiContributivi.areaDatiContributivi.DatiCalcolo != null && datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.NonValido)
                    datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo = tipoCalcoloPrecedente;
                return;
            }
        }
        #endregion Dati Calcolo

        #region Dati Calcolo 707
        public void SalvaTabDatiCalcolo707(IDatiContributivi datiContributivi)
        {
            AreaDatiContributivi areaDatiContributivi = new AreaDatiContributivi();
            Presenter.SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            Int64 ndomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaEsito risultatoSalvaDatiCalcolo707ByDomanda = new AreaEsito();
            areaDatiContributivi = datiContributivi.areaDatiContributivi;
            try
            {
                risultatoSalvaDatiCalcolo707ByDomanda = objWS.StoreDatiCalcolo707ByDomanda(ndomanda, areaDatiContributivi);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo SalvaTabDatiCalcolo707");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoSalvaDatiCalcolo707ByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = risultatoSalvaDatiCalcolo707ByDomanda.Messaggio;
                return;
            }
            else
            {
                datiContributivi.HasError = false;
                datiContributivi.ErrorMessage = "";
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                return;
            }
        }

        public void EliminaTabDatiCalcolo707(IDatiContributivi datiContributivi)
        {
            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            Int64 ndomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaDatiContributivi areaDatiContributivi = new AreaDatiContributivi();
            AreaEsito risultatoCancelTabDatiCalcolo707ByDomanda = new AreaEsito();
            try
            {
                risultatoCancelTabDatiCalcolo707ByDomanda = objWS.CancelDatiCalcolo707ByDomanda(out areaDatiContributivi, ndomanda);

            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo EliminaTabDatiCalcolo707");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoCancelTabDatiCalcolo707ByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = risultatoCancelTabDatiCalcolo707ByDomanda.Messaggio;

                return;
            }
            else
            {
                if (!String.IsNullOrEmpty(risultatoCancelTabDatiCalcolo707ByDomanda.Messaggio))
                    datiContributivi.ErrorMessage = risultatoCancelTabDatiCalcolo707ByDomanda.Messaggio;
                else
                    datiContributivi.ErrorMessage = "";
                datiContributivi.HasError = false;
                datiContributivi.areaDatiContributivi = areaDatiContributivi;

               return;
            }
        }
        #endregion Dati Calcolo 707


        #region Dati Fondo GAS
        public void SalvaTabDatiFondoGAS(IDatiContributivi datiContributivi)
        {
            AreaDatiContributivi areaDatiContributivi = new AreaDatiContributivi();
            Presenter.SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            Int64 ndomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaEsito risultatoSalvaDatiFondoGASByDomanda = new AreaEsito();
            areaDatiContributivi = datiContributivi.areaDatiContributivi;
            try
            {
                risultatoSalvaDatiFondoGASByDomanda = objWS.StoreDatiFondoByDomanda(ndomanda, areaDatiContributivi);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo SalvaTabDatiFondoGAS");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoSalvaDatiFondoGASByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = risultatoSalvaDatiFondoGASByDomanda.Messaggio;
                return;
            }
            else
            {
                datiContributivi.HasError = false;
                datiContributivi.ErrorMessage = "";
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                return;
            }
        }

        public void EliminaTabDatiFondoGAS(IDatiContributivi datiContributivi)
        {
            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            Int64 ndomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaDatiContributivi areaDatiContributivi = new AreaDatiContributivi();
            AreaEsito risultatoCancelTabDatiFondoGASByDomanda = new AreaEsito();
            try
            {
                risultatoCancelTabDatiFondoGASByDomanda = objWS.CancelDatiFondoByDomanda(out areaDatiContributivi, ndomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo EliminaTabDatiFondoGAS");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoCancelTabDatiFondoGASByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = risultatoCancelTabDatiFondoGASByDomanda.Messaggio;

                GestioneContribTipoCalcolo tipoCalcoloPrecedente = GestioneContribTipoCalcolo.NonValido;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null && datiContributivi.areaDatiContributivi.DatiCalcolo != null)
                    tipoCalcoloPrecedente = datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo;
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null &&
                    datiContributivi.areaDatiContributivi.DatiCalcolo != null && datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.NonValido)
                    datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo = tipoCalcoloPrecedente;
                return;
            }
            else
            {
                if (!String.IsNullOrEmpty(risultatoCancelTabDatiFondoGASByDomanda.Messaggio))
                    datiContributivi.ErrorMessage = risultatoCancelTabDatiFondoGASByDomanda.Messaggio;
                else
                    datiContributivi.ErrorMessage = "";
                datiContributivi.HasError = false;

                GestioneContribTipoCalcolo tipoCalcoloPrecedente = GestioneContribTipoCalcolo.NonValido;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null && datiContributivi.areaDatiContributivi.DatiCalcolo != null)
                    tipoCalcoloPrecedente = datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo;
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null &&
                    datiContributivi.areaDatiContributivi.DatiCalcolo != null && datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.NonValido)
                    datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo = tipoCalcoloPrecedente;
                return;
            }
        }
        #endregion Dati Fondo GAS

        #region Dati Ago GAS
        public void SalvaTabDatiAgoGAS(IDatiContributivi datiContributivi)
        {
            AreaDatiContributivi areaDatiContributivi = new AreaDatiContributivi();
            Presenter.SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            Int64 ndomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaEsito risultatoSalvaDatiAgoGASByDomanda = new AreaEsito();
            areaDatiContributivi = datiContributivi.areaDatiContributivi;
            try
            {
                risultatoSalvaDatiAgoGASByDomanda = objWS.StoreDatiCalcoloByDomanda(ndomanda, areaDatiContributivi);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo SalvaTabDatiAgoGAS");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoSalvaDatiAgoGASByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = risultatoSalvaDatiAgoGASByDomanda.Messaggio;
                return;
            }
            else
            {
                datiContributivi.HasError = false;
                datiContributivi.ErrorMessage = "";
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                return;
            }
        }

        public void EliminaTabDatiAgoGAS(IDatiContributivi datiContributivi)
        {
            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            Int64 ndomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaDatiContributivi areaDatiContributivi = new AreaDatiContributivi();
            AreaEsito risultatoCancelTabDatiAgoGASByDomanda = new AreaEsito();
            try
            {
                risultatoCancelTabDatiAgoGASByDomanda = objWS.CancelDatiCalcoloByDomanda(out areaDatiContributivi, ndomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo EliminaTabDatiAgoGAS");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoCancelTabDatiAgoGASByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = risultatoCancelTabDatiAgoGASByDomanda.Messaggio;

                GestioneContribTipoCalcolo tipoCalcoloPrecedente = GestioneContribTipoCalcolo.NonValido;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null && datiContributivi.areaDatiContributivi.DatiCalcolo != null)
                    tipoCalcoloPrecedente = datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo;
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null &&
                    datiContributivi.areaDatiContributivi.DatiCalcolo != null && datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.NonValido)
                    datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo = tipoCalcoloPrecedente;
                return;
            }
            else
            {
                if (!String.IsNullOrEmpty(risultatoCancelTabDatiAgoGASByDomanda.Messaggio))
                    datiContributivi.ErrorMessage = risultatoCancelTabDatiAgoGASByDomanda.Messaggio;
                else
                    datiContributivi.ErrorMessage = "";
                datiContributivi.HasError = false;

                GestioneContribTipoCalcolo tipoCalcoloPrecedente = GestioneContribTipoCalcolo.NonValido;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null && datiContributivi.areaDatiContributivi.DatiCalcolo != null)
                    tipoCalcoloPrecedente = datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo;
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null &&
                    datiContributivi.areaDatiContributivi.DatiCalcolo != null && datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.NonValido)
                    datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo = tipoCalcoloPrecedente;
                return;
            }
        }
        #endregion Dati Ago GAS

        #region Dati Art 11 e 14 GAS
        public void SalvaTabDatiArt11_14GAS(IDatiContributivi datiContributivi)
        {
            AreaDatiContributivi areaDatiContributivi = new AreaDatiContributivi();
            Presenter.SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            Int64 ndomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaEsito risultatoSalvaDatiArt11_14GASByDomanda = new AreaEsito();
            areaDatiContributivi = datiContributivi.areaDatiContributivi;
            try
            {
                risultatoSalvaDatiArt11_14GASByDomanda = objWS.StoreDatiArt14e11ByDomanda(ndomanda, areaDatiContributivi);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo SalvaTabDatiArt11_14GAS");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoSalvaDatiArt11_14GASByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = risultatoSalvaDatiArt11_14GASByDomanda.Messaggio;
                return;
            }
            else
            {
                datiContributivi.HasError = false;
                datiContributivi.ErrorMessage = "";
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                return;
            }
        }

        public void EliminaTabDatiArt11_14GAS(IDatiContributivi datiContributivi)
        {
            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            Int64 ndomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaDatiContributivi areaDatiContributivi = new AreaDatiContributivi();
            AreaEsito risultatoCancelTabDatiArt11_14GASByDomanda = new AreaEsito();
            try
            {
                risultatoCancelTabDatiArt11_14GASByDomanda = objWS.CancelDatiArt14e11ByDomanda(out areaDatiContributivi, ndomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo EliminaTabDatiArt11_14GAS");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoCancelTabDatiArt11_14GASByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = risultatoCancelTabDatiArt11_14GASByDomanda.Messaggio;

                GestioneContribTipoCalcolo tipoCalcoloPrecedente = GestioneContribTipoCalcolo.NonValido;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null && datiContributivi.areaDatiContributivi.DatiCalcolo != null)
                    tipoCalcoloPrecedente = datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo;
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null &&
                    datiContributivi.areaDatiContributivi.DatiCalcolo != null && datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.NonValido)
                    datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo = tipoCalcoloPrecedente;
                return;
            }
            else
            {
                if (!String.IsNullOrEmpty(risultatoCancelTabDatiArt11_14GASByDomanda.Messaggio))
                    datiContributivi.ErrorMessage = risultatoCancelTabDatiArt11_14GASByDomanda.Messaggio;
                else
                    datiContributivi.ErrorMessage = "";
                datiContributivi.HasError = false;

                GestioneContribTipoCalcolo tipoCalcoloPrecedente = GestioneContribTipoCalcolo.NonValido;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null && datiContributivi.areaDatiContributivi.DatiCalcolo != null)
                    tipoCalcoloPrecedente = datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo;
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null &&
                    datiContributivi.areaDatiContributivi.DatiCalcolo != null && datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.NonValido)
                    datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo = tipoCalcoloPrecedente;
                return;
            }
        }
        #endregion Dati Art 11 e 14 GAS

        #region Dati Ante 67
        public void SalvaTabDatiAnte67(IDatiContributivi datiContributivi)
        {
            AreaDatiContributivi areaDatiContributivi = new AreaDatiContributivi();
            Presenter.SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            Int64 ndomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaEsito risultatoSalvaDatiAnte67_ByDomanda = new AreaEsito();
            areaDatiContributivi = datiContributivi.areaDatiContributivi;
            try
            {
                risultatoSalvaDatiAnte67_ByDomanda = objWS.StoreAnte67ByDomanda(ndomanda, areaDatiContributivi);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo SalvaTabDatiAnte67");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoSalvaDatiAnte67_ByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = risultatoSalvaDatiAnte67_ByDomanda.Messaggio;
                return;
            }
            else
            {
                datiContributivi.HasError = false;
                datiContributivi.ErrorMessage = "";
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                return;
            }
        }

        public void EliminaTabDatiAnte67(IDatiContributivi datiContributivi)
        {
            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            Int64 ndomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaDatiContributivi areaDatiContributivi = new AreaDatiContributivi();
            AreaEsito risultatoCancelTabDatiByDomanda = null;
            try
            {
                risultatoCancelTabDatiByDomanda = objWS.CancelDatiAnte67ByDomanda(out areaDatiContributivi, ndomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo EliminaTabDatiAnte67");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoCancelTabDatiByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = risultatoCancelTabDatiByDomanda.Messaggio;

                GestioneContribTipoCalcolo tipoCalcoloPrecedente = GestioneContribTipoCalcolo.NonValido;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null && datiContributivi.areaDatiContributivi.DatiCalcolo != null)
                    tipoCalcoloPrecedente = datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo;
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null &&
                    datiContributivi.areaDatiContributivi.DatiCalcolo != null && datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.NonValido)
                    datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo = tipoCalcoloPrecedente;
                return;
            }
            else
            {
                if (!String.IsNullOrEmpty(risultatoCancelTabDatiByDomanda.Messaggio))
                    datiContributivi.ErrorMessage = risultatoCancelTabDatiByDomanda.Messaggio;
                else
                    datiContributivi.ErrorMessage = "";
                datiContributivi.HasError = false;

                GestioneContribTipoCalcolo tipoCalcoloPrecedente = GestioneContribTipoCalcolo.NonValido;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null && datiContributivi.areaDatiContributivi.DatiCalcolo != null)
                    tipoCalcoloPrecedente = datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo;
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null &&
                    datiContributivi.areaDatiContributivi.DatiCalcolo != null && datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.NonValido)
                    datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo = tipoCalcoloPrecedente;
                return;
            }
        }

        #endregion Dati Ante 67

        #region Dati SL 336

        public void SalvaTabDatiSL336(IDatiContributivi datiContributivi)
        {
            AreaDatiContributivi areaDatiContributivi = new AreaDatiContributivi();
            Presenter.SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            Int64 ndomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaEsito risultatoSalvaDati_ByDomanda = new AreaEsito();
            areaDatiContributivi = datiContributivi.areaDatiContributivi;
            try
            {
                risultatoSalvaDati_ByDomanda = objWS.StoreSL336ByDomanda(ndomanda, areaDatiContributivi);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo SalvaTabDatiSL336");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoSalvaDati_ByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = risultatoSalvaDati_ByDomanda.Messaggio;
                return;
            }
            else
            {
                datiContributivi.HasError = false;
                datiContributivi.ErrorMessage = "";
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                return;
            }
        }

        public void EliminaTabDatiSL336(IDatiContributivi datiContributivi)
        {
            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            Int64 ndomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaDatiContributivi areaDatiContributivi = new AreaDatiContributivi();
            AreaEsito risultatoCancelTabDatiByDomanda = null;
            try
            {
                risultatoCancelTabDatiByDomanda = objWS.CancelSL336ByDomanda(out areaDatiContributivi, ndomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo EliminaTabDatiSL336");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoCancelTabDatiByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = risultatoCancelTabDatiByDomanda.Messaggio;

                GestioneContribTipoCalcolo tipoCalcoloPrecedente = GestioneContribTipoCalcolo.NonValido;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null && datiContributivi.areaDatiContributivi.DatiCalcolo != null)
                    tipoCalcoloPrecedente = datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo;
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null &&
                    datiContributivi.areaDatiContributivi.DatiCalcolo != null && datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.NonValido)
                    datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo = tipoCalcoloPrecedente;
                return;
            }
            else
            {
                if (!String.IsNullOrEmpty(risultatoCancelTabDatiByDomanda.Messaggio))
                    datiContributivi.ErrorMessage = risultatoCancelTabDatiByDomanda.Messaggio;
                else
                    datiContributivi.ErrorMessage = "";
                datiContributivi.HasError = false;

                GestioneContribTipoCalcolo tipoCalcoloPrecedente = GestioneContribTipoCalcolo.NonValido;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null && datiContributivi.areaDatiContributivi.DatiCalcolo != null)
                    tipoCalcoloPrecedente = datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo;
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                if (datiContributivi != null && datiContributivi.areaDatiContributivi != null &&
                    datiContributivi.areaDatiContributivi.DatiCalcolo != null && datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.NonValido)
                    datiContributivi.areaDatiContributivi.DatiCalcolo.TipoCalcolo = tipoCalcoloPrecedente;
                return;
            }
        }
        
        #endregion Dati SL 336

        #region Dati Ago Altra Pensione
        public void SalvaTabDatiAgoAltraPensione(IDatiContributivi datiContributivi)
        {
            
            AreaDatiContributivi areaDatiContributivi = new AreaDatiContributivi();
            Presenter.SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            Int64 ndomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaEsito result = new AreaEsito();
            areaDatiContributivi = datiContributivi.areaDatiContributivi;
            try
            {
                result = objWS.StoreAltraPensioneDatiAgoByDomanda(ndomanda, areaDatiContributivi);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo SalvaTabDatiAgoAltraPensione");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (result.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = result.Messaggio;
                return;
            }
            else
            {
                datiContributivi.HasError = false;
                datiContributivi.ErrorMessage = "";
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                return;
            }
        }

        public void EliminaTabDatiAgoAltraPensione(IDatiContributivi datiContributivi)
        {
            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            Int64 ndomanda = Int64.Parse(datiContributivi.domanda.NumeroDomanda);
            AreaDatiContributivi areaDatiContributivi = new AreaDatiContributivi();
            AreaEsito result = new AreaEsito();
            try
            {
                result = objWS.CancelDatiAgoAltraPensioneByDomanda(out areaDatiContributivi, ndomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo EliminaTabDatiAgoAltraPensione");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (result.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiContributivi.HasError = true;
                datiContributivi.ErrorMessage = result.Messaggio;

            }
            else
            {
                datiContributivi.HasError = false;
                datiContributivi.ErrorMessage = "";
                datiContributivi.areaDatiContributivi = areaDatiContributivi;
                return;
            }
        }
        #endregion Dati Ago GAS

        #region Dati Ago Fondo PI

        public void GetDatiAgoFondoPi(IDatiAgoFondoPI datiAgoFondoPI)
        {
            AreaDatiAgoFondoPI areaDatiAgoFondoPI = new AreaDatiAgoFondoPI();
            AreaEsito esito = null;

            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            try
            {
                if(datiAgoFondoPI.IdDatiAgoFondoPI == null)
                {
                    datiAgoFondoPI.HasError = true;
                    datiAgoFondoPI.ErrorMessage = "Ricaricare la pagina";
                    datiAgoFondoPI.areaDatiAgoFondoPI = areaDatiAgoFondoPI;
                    return;
                }

                long id = (long)datiAgoFondoPI.IdDatiAgoFondoPI;

                esito = objWS.GetDatiAgoFondoPIById(out areaDatiAgoFondoPI, id);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo GetDatiAgoFondoPi");
                datiAgoFondoPI.HasError = true;
                datiAgoFondoPI.ErrorMessage = "Errore nel recupero dei dati.";
                return;
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (esito != null && esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiAgoFondoPI.HasError = true;
                datiAgoFondoPI.ErrorMessage = esito.Messaggio;
                datiAgoFondoPI.areaDatiAgoFondoPI = areaDatiAgoFondoPI;
                return;
            }

            datiAgoFondoPI.HasError = false;
            datiAgoFondoPI.ErrorMessage = "";
            datiAgoFondoPI.areaDatiAgoFondoPI = areaDatiAgoFondoPI;
        }

        public void StoreDatiAgoFondoPi(IDatiAgoFondoPI datiAgoFondoPI)
        {
            AreaEsito esito = null;

            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            try
            {
                AreaDatiAgoFondoPI area = datiAgoFondoPI.areaDatiAgoFondoPI;
                area.Id = datiAgoFondoPI.IdDatiAgoFondoPI;
                if (area == null)
                {
                    datiAgoFondoPI.HasError = true;
                    datiAgoFondoPI.ErrorMessage = "Dati non presenti per il salvataggio.";
                    return;
                }

                esito = objWS.StoreDatiAgoFondoPIById(area);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo StoreDatiAgoFondoPi");
                datiAgoFondoPI.HasError = true;
                datiAgoFondoPI.ErrorMessage = "Errore nel salvataggio dei dati.";
                return;
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (esito != null && esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiAgoFondoPI.HasError = true;
                datiAgoFondoPI.ErrorMessage = esito.Messaggio;
                return;
            }

            datiAgoFondoPI.HasError = false;
            datiAgoFondoPI.ErrorMessage = "";
        }

        public void EliminaDatiAgoFondoPIById(IDatiAgoFondoPI view)
        {
            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();

            try
            {
                if (view == null || view.IdDatiAgoFondoPI == null || view.IdDatiAgoFondoPI == 0)
                {
                    view.HasError = true;
                    view.ErrorMessage = "Dati non presenti";
                    return;
                }

                AreaEsito esito = objWS.CancelDatiAgoPensioneFondoPI((long)view.IdDatiAgoFondoPI);

                if (esito != null && esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    view.HasError = true;
                    view.ErrorMessage = esito.Messaggio;
                    return;
                }

                view.HasError = false;
                view.ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo EliminaDatiPensioneFondoPIByIdRecordFondo");

                view.HasError = true;
                view.ErrorMessage = "Errore nel salvataggio dei dati.";
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
        #endregion

        #region Dati Fondo PI

        public void GetDatiPensioneFondoPi(IDatiPensioneFondoPI datiPensioneFondoPI)
        {
            AreaDatiPensioneFondoPI areaDatiPensioneFondoPI = new AreaDatiPensioneFondoPI();
            AreaEsito esito = null;

            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            try
            {
                if (datiPensioneFondoPI.IdRecordFondo.HasValue)
                {
                    long id = (long)datiPensioneFondoPI.IdRecordFondo;
                    esito = objWS.GetDatiPensioneFondoPIById(out areaDatiPensioneFondoPI, id);
                }
                else
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo GetDatiPensioneFondoPi");
                datiPensioneFondoPI.HasError = true;
                datiPensioneFondoPI.ErrorMessage = "Errore nel recupero dei dati.";
                return;
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (esito != null && esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiPensioneFondoPI.HasError = true;
                datiPensioneFondoPI.ErrorMessage = esito.Messaggio;
                datiPensioneFondoPI.areaDatiPensioneFondoPI = areaDatiPensioneFondoPI;
                return;
            }

            datiPensioneFondoPI.HasError = false;
            datiPensioneFondoPI.ErrorMessage = "";
            datiPensioneFondoPI.areaDatiPensioneFondoPI = areaDatiPensioneFondoPI;
        }

        public void StoreDatiPensioneFondoPi(IDatiPensioneFondoPI view)
        {
            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();

            try
            {
                if (view == null || view.areaDatiPensioneFondoPI == null)
                {
                    view.HasError = true;
                    view.ErrorMessage = "Dati non presenti per il salvataggio.";
                    return;
                }

                AreaDatiPensioneFondoPI area = view.areaDatiPensioneFondoPI;

                area.IdFondo = view.IdFondo;
                area.IdRecordFondo = view.IdRecordFondo != 0 ? view.IdRecordFondo : null;
                area.NumDomanda = Int64.Parse(view.NumDomanda);
                area.ControCodiceRetribuzione = view.ControCodiceRetribuzione;
                AreaEsito esito = objWS.StoreDatiPensioneFondoPIByIdRecord(area);

                if (esito != null && esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    view.HasError = true;
                    view.ErrorMessage = esito.Messaggio;
                    return;
                }

                view.HasError = false;
                view.ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo StoreDatiPensioneFondoPi");

                view.HasError = true;
                view.ErrorMessage = "Errore nel salvataggio dei dati.";
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaDatiPensioneFondoPIByIdRecordFondo(IDatiPensioneFondoPI view) 
        {
            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();

            try
            {
                if (view == null || view.IdRecordFondo == null || view.IdRecordFondo == 0)
                {
                    view.HasError = true;
                    view.ErrorMessage = "Dati non presenti";
                    return;
                }

                AreaEsito esito = objWS.CancelDatiFondoPensioneFondoPI((long)view.IdRecordFondo);

                if (esito != null && esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    view.HasError = true;
                    view.ErrorMessage = esito.Messaggio;
                    return;
                }

                view.HasError = false;
                view.ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiContributivi, Errore nel metodo EliminaDatiPensioneFondoPIByIdRecordFondo");

                view.HasError = true;
                view.ErrorMessage = "Errore nel salvataggio dei dati.";
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
        #endregion
    }
}
