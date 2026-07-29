using System;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.DNA;
using System.ServiceModel;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi;


namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterMaggiorazioneBenefici
    {
        #region IMaggiorazionePensione
        public SvrLiquidazioneFs.AreaMaggiorazioniBenefici maggiorazioneBenefici { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda areaRiepilogoDomanda { get; set; }
        #endregion IMaggiorazionePensione

        #region FS

        public void GetMaggiorazioneBenefici(IMaggiorazioneBenefici maggBen)
        {
            SvrLiquidazioneFs.AreaMaggiorazioniBenefici areaMaggiorazioneBenefici = new SvrLiquidazioneFs.AreaMaggiorazioniBenefici();
            Presenter.SvrLiquidazioneFs.AreaEsito esitoFs = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
            Presenter.SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            Presenter.SvrLiquidazioneFs.AreaRichiestaDomanda areaRichiestaDomanda = new Presenter.SvrLiquidazioneFs.AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(maggBen.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = maggBen.domanda.ProgStorico;

            try
            {
                esitoFs = objWS.GetMaggiorazioniBeneficiByDomanda(out areaMaggiorazioneBenefici, areaRichiestaDomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo GetMaggiorazioneBenefici");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (esitoFs.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
            {
                //throw new DnaApplicationException("PresenterMaggiorazioneBenefici:" + esitoFs.Messaggio);
                maggBen.HasError = true;
                maggBen.ErrorMessage = "Errore tecnico nel recupero dei dati del quadro Maggiorazioni/Benefici";
            }
            else
                maggBen.areaMaggiorazioneBenefici = areaMaggiorazioneBenefici;
        }

        public void SalvaMaggiorazioniBenefici(IMaggiorazioneBenefici maggBen)
        {
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFs = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = Int64.Parse(maggBen.domanda.NumeroDomanda);
                esitoFs = objWS.StoreMaggiorazioniBenefici(ndomus, maggBen.areaMaggiorazioneBenefici);

                if (esitoFs.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoFs.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo SalvaMaggiorazioniBenefici");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        #region Dati Ex Combattente

        public void SalvaExCombattente(IMaggiorazioneBenefici maggBen)
        {
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFs = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = Int64.Parse(maggBen.domanda.NumeroDomanda);
                esitoFs = objWS.StoreDatiExCombattente(ndomus, maggBen.areaMaggiorazioneBenefici);

                if (esitoFs.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoFs.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo SalvaExCombattente");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaExCombattente(IMaggiorazioneBenefici maggBen)
        {
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFs = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                SvrLiquidazioneFs.AreaMaggiorazioniBenefici areaMaggiorazioniBenefici = null;
                long ndomus = Int64.Parse(maggBen.domanda.NumeroDomanda);
                esitoFs = objWS.CancelDatiExCombattente(out areaMaggiorazioniBenefici, ndomus);

                if (esitoFs.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoFs.Messaggio;
                }
                else
                {
                    maggBen.HasError = false;
                    maggBen.ErrorMessage = "";
                    maggBen.areaMaggiorazioneBenefici = new SvrLiquidazioneFs.AreaMaggiorazioniBenefici();
                    maggBen.areaMaggiorazioneBenefici.DatiExCombattente = areaMaggiorazioniBenefici.DatiExCombattente;
                    maggBen.areaMaggiorazioneBenefici.ListaCodiceCieco = areaMaggiorazioniBenefici.ListaCodiceCieco;
                    maggBen.areaMaggiorazioneBenefici.ListaCodiceMaggiorazioneExCombattente = areaMaggiorazioniBenefici.ListaCodiceMaggiorazioneExCombattente;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo EliminaExCombattente");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        #endregion Dati Ex Combattente

        #region Dati Benefici

        public void SalvaBenefici(IMaggiorazioneBenefici maggBen)
        {
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFs = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = Int64.Parse(maggBen.domanda.NumeroDomanda);
                esitoFs = objWS.StoreDatiBenefici(ndomus, maggBen.areaMaggiorazioneBenefici);

                if (esitoFs.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoFs.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo SalvaBenefici");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaBenefici(IMaggiorazioneBenefici maggBen)
        {
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFs = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                SvrLiquidazioneFs.AreaMaggiorazioniBenefici areaMaggiorazioniBenefici = null;
                long ndomus = Int64.Parse(maggBen.domanda.NumeroDomanda);
                esitoFs = objWS.CancelDatiBenefici(out areaMaggiorazioniBenefici, ndomus);

                if (esitoFs.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoFs.Messaggio;
                }
                else
                {
                    maggBen.HasError = false;
                    maggBen.ErrorMessage = "";
                    maggBen.areaMaggiorazioneBenefici = areaMaggiorazioniBenefici;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo EliminaBenefici");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        #endregion Dati Benefici

        #region Dati DL407

        public void SalvaTabDatiDL407(IMaggiorazioneBenefici maggiorazioneBenefici)
        {
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFs = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = Int64.Parse(maggiorazioneBenefici.domanda.NumeroDomanda);
                esitoFs = objWS.StoreDatiDL407(ndomus, maggiorazioneBenefici.areaMaggiorazioneBenefici);

                if (esitoFs.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    maggiorazioneBenefici.HasError = true;
                    maggiorazioneBenefici.ErrorMessage = esitoFs.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo SalvaTabDatiDL407");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaTabDatiDL407(IMaggiorazioneBenefici maggBen)
        {
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFs = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = Int64.Parse(maggBen.domanda.NumeroDomanda);
                esitoFs = objWS.CancelDatiDL407(ndomus);

                if (esitoFs.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoFs.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo EliminaTabDatiDL407");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
        
        #endregion Dati DL407

        #region Dati Privilegiate

        public void SalvaTabDatiPrivilegiate(IMaggiorazioneBenefici maggiorazioneBenefici)
        {
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFs = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = Int64.Parse(maggiorazioneBenefici.domanda.NumeroDomanda);
                esitoFs = objWS.StoreDatiPrivilegiate(ndomus, maggiorazioneBenefici.areaMaggiorazioneBenefici);

                if (esitoFs.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    maggiorazioneBenefici.HasError = true;
                    maggiorazioneBenefici.ErrorMessage = esitoFs.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo SalvaTabDatiPrivilegiate");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaTabDatiPrivilegiate(IMaggiorazioneBenefici maggBen)
        {
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFs = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                SvrLiquidazioneFs.AreaMaggiorazioniBenefici areaMaggiorazioniBenefici = null;
                long ndomus = Int64.Parse(maggBen.domanda.NumeroDomanda);
                esitoFs = objWS.CancelDatiPrivilegiate(out areaMaggiorazioniBenefici, ndomus);

                if (esitoFs.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoFs.Messaggio;
                }
                else
                {
                    maggBen.HasError = false;
                    maggBen.ErrorMessage = "";
                    maggBen.areaMaggiorazioneBenefici = new SvrLiquidazioneFs.AreaMaggiorazioniBenefici();
                    maggBen.areaMaggiorazioneBenefici.DatiPrivilegiate = areaMaggiorazioniBenefici.DatiPrivilegiate;
                    maggBen.areaMaggiorazioneBenefici.ListaCodicePensioniPrivilegiate = areaMaggiorazioniBenefici.ListaCodicePensioniPrivilegiate;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo EliminaTabDatiPrivilegiate");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        #endregion Dati Privilegiate

        #region Dati Articolo2

        public void SalvaTabDatiArticolo2(IMaggiorazioneBenefici maggiorazioneBenefici)
        {
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFs = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = Int64.Parse(maggiorazioneBenefici.domanda.NumeroDomanda);
                esitoFs = objWS.StoreDatiArticolo2(ndomus, maggiorazioneBenefici.areaMaggiorazioneBenefici);

                if (esitoFs.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    maggiorazioneBenefici.HasError = true;
                    maggiorazioneBenefici.ErrorMessage = esitoFs.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo SalvaTabDatiArticolo2");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaTabDatiArticolo2(IMaggiorazioneBenefici maggBen)
        {
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFs = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                SvrLiquidazioneFs.AreaMaggiorazioniBenefici areaMaggiorazioniBenefici = null;
                long ndomus = Int64.Parse(maggBen.domanda.NumeroDomanda);
                esitoFs = objWS.CancelDatiArticolo2(out areaMaggiorazioniBenefici, ndomus);

                if (esitoFs.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoFs.Messaggio;
                }
                else
                {
                    maggBen.HasError = false;
                    maggBen.ErrorMessage = "";
                    maggBen.areaMaggiorazioneBenefici = new SvrLiquidazioneFs.AreaMaggiorazioniBenefici();
                    maggBen.areaMaggiorazioneBenefici.DatiArticolo2 = areaMaggiorazioniBenefici.DatiArticolo2;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo EliminaTabDatiArticolo2");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        #endregion Dati Articolo2

        #region Dati Vittime

        public void SalvaVittimeFs(IMaggiorazioneBenefici maggBen)
        {
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWSFs = new ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFs = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = Int64.Parse(maggBen.domanda.NumeroDomanda);
                esitoFs = objWSFs.StoreDatiBeneficioVittimeTerrorismo(ndomus, maggBen.areaMaggiorazioneBenefici);

                if (esitoFs.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoFs.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo SalvaVittime");
            }
            finally
            {
                Utility.CloseClient(objWSFs);
            }
        }

        public void EliminaVittimeFs(IMaggiorazioneBenefici maggBen)
        {
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWSFs = new ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaMaggiorazioniBenefici areaMaggiorazioniBenefici = null;
                SvrLiquidazioneFs.AreaEsito esitoFs = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = Int64.Parse(maggBen.domanda.NumeroDomanda);
                esitoFs = objWSFs.CancelDatiBeneficioVittimeTerrorismo(out areaMaggiorazioniBenefici, ndomus);

                if (esitoFs.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoFs.Messaggio;
                }

                if (areaMaggiorazioniBenefici != null)
                    maggBen.areaMaggiorazioneBenefici = areaMaggiorazioniBenefici;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo EliminaVittime");
            }
            finally
            {
                Utility.CloseClient(objWSFs);
            }
        }

        #endregion Dati Vittime

        #endregion FS

        #region CI

        public void GetMaggiorazioneBeneficiCi(IMaggiorazioneBeneficiCi maggBen)
        {
            SvrLiquidazioneCi.AreaMaggiorazioniBenefici areaMaggiorazioneBenefici = new SvrLiquidazioneCi.AreaMaggiorazioniBenefici();
            Presenter.SvrLiquidazioneCi.AreaEsito esitoCi = new Presenter.SvrLiquidazioneCi.AreaEsito();
            ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            Presenter.SvrLiquidazioneCi.AreaRichiestaDomanda areaRichiestaDomanda = new Presenter.SvrLiquidazioneCi.AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(maggBen.areaRiepilogoDomanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = maggBen.areaRiepilogoDomanda.ProgStorico;
            try
            {
                esitoCi = objWSCi.GetMaggiorazioniBeneficiByDomanda(out areaMaggiorazioneBenefici, areaRichiestaDomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo GetMaggiorazioneBeneficiCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }

            if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
            {
                maggBen.HasError = true;
                maggBen.ErrorMessage = "Errore tecnico nel recupero dei dati del quadro Maggiorazioni/Benefici";
            }
            else
                maggBen.areaMaggiorazioneBenefici = areaMaggiorazioneBenefici;
        }

        public void SalvaMaggiorazioniBeneficiCi(IMaggiorazioneBeneficiCi maggBen)
        {
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(maggBen.areaRiepilogoDomanda.NumeroDomanda);
                esitoCi = objWSCi.StoreMaggiorazioniBenefici(ndomus, maggBen.areaMaggiorazioneBenefici);

                if (esitoCi.RisultatoOperazione == SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo SalvaMaggiorazioniBeneficiCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        #region Dati Benefici

        public void SalvaBeneficiCi(IMaggiorazioneBeneficiCi maggBen)
        {
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(maggBen.areaRiepilogoDomanda.NumeroDomanda);
                esitoCi = objWSCi.StoreDatiBenefici(ndomus, maggBen.areaMaggiorazioneBenefici);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo SalvaBeneficiCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        public void EliminaBeneficiCi(IMaggiorazioneBeneficiCi maggBen)
        {
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(maggBen.areaRiepilogoDomanda.NumeroDomanda);
                esitoCi = objWSCi.CancelDatiBenefici(ndomus);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo EliminaBeneficiCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        #endregion Dati Benefici

        #region Dati Ex Combattente

        public void SalvaExCombattenteCi(IMaggiorazioneBeneficiCi maggBen)
        {
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(maggBen.areaRiepilogoDomanda.NumeroDomanda);
                esitoCi = objWSCi.StoreDatiExCombattente(ndomus, maggBen.areaMaggiorazioneBenefici);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo SalvaExCombattenteCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        public void EliminaExCombattenteCi(IMaggiorazioneBeneficiCi maggBen)
        {
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(maggBen.areaRiepilogoDomanda.NumeroDomanda);
                esitoCi = objWSCi.CancelDatiExCombattente(ndomus);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo EliminaExCombattenteCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        #endregion Dati Ex Combattente

        #region Dati Maggiorazioni

        public void SalvaMaggiorazioniCi(IMaggiorazioneBeneficiCi maggBen)
        {
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(maggBen.areaRiepilogoDomanda.NumeroDomanda);
                esitoCi = objWSCi.StoreDatiMaggiorazioni(ndomus, maggBen.areaMaggiorazioneBenefici);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo SalvaMaggiorazioniCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        public void EliminaMaggiorazioniCi(IMaggiorazioneBeneficiCi maggBen)
        {
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaMaggiorazioniBenefici areaMaggiorazioniBenefici = null;
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(maggBen.areaRiepilogoDomanda.NumeroDomanda);
                esitoCi = objWSCi.CancelDatiMaggiorazioni(out areaMaggiorazioniBenefici, ndomus);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo EliminaMaggiorazioniCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        #endregion Dati Maggiorazioni

        #region Dati Vittime

        public void SalvaVittimeCi(IMaggiorazioneBeneficiCi maggBen)
        {
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(maggBen.areaRiepilogoDomanda.NumeroDomanda);
                esitoCi = objWSCi.StoreDatiBeneficioVittimeTerrorismo(ndomus, maggBen.areaMaggiorazioneBenefici);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo SalvaVittime");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        public void EliminaVittimeCi(IMaggiorazioneBeneficiCi maggBen)
        {
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaMaggiorazioniBenefici areaMaggiorazioniBenefici = null;
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(maggBen.areaRiepilogoDomanda.NumeroDomanda);
                esitoCi = objWSCi.CancelDatiBeneficioVittimeTerrorismo(out areaMaggiorazioniBenefici, ndomus);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoCi.Messaggio;
                }

                if (areaMaggiorazioniBenefici != null)
                    maggBen.areaMaggiorazioneBenefici = areaMaggiorazioniBenefici;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo EliminaVittime");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        #endregion Dati Vittime

        #endregion CI

        #region AGO

        public void GetMaggiorazioneBeneficiAgo(IMaggiorazioneBeneficiAgo maggBen)
        {
            SvrLiquidazioneAgo.AreaMaggiorazioniBenefici areaMaggiorazioneBenefici = new SvrLiquidazioneAgo.AreaMaggiorazioniBenefici();
            Presenter.SvrLiquidazioneAgo.AreaEsito esitoAgo = new Presenter.SvrLiquidazioneAgo.AreaEsito();
            ServizioLiquidazioneAgoClient objWSAgo = new ServizioLiquidazioneAgoClient();
            Presenter.SvrLiquidazioneAgo.AreaRichiestaDomanda areaRichiestaDomanda = new Presenter.SvrLiquidazioneAgo.AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(maggBen.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = maggBen.domanda.ProgStorico;
            try
            {
                esitoAgo = objWSAgo.GetMaggiorazioniBeneficiByDomanda(out areaMaggiorazioneBenefici, areaRichiestaDomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo GetMaggiorazioneBeneficiAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }

            if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
            {
                maggBen.HasError = true;
                maggBen.ErrorMessage = "Errore tecnico nel recupero dei dati del quadro Maggiorazioni/Benefici";
            }
            else
                maggBen.areaMaggiorazioneBenefici = areaMaggiorazioneBenefici;
        }

        public void SalvaMaggiorazioniBeneficiAgo(IMaggiorazioneBeneficiAgo maggBen)
        {
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(maggBen.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.StoreMaggiorazioniBenefici(ndomus, maggBen.areaMaggiorazioneBenefici);

                if (esitoAgo.RisultatoOperazione == SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoAgo.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo SalvaMaggiorazioniBeneficiAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }

        #region Dati Benefici

        public void SalvaBeneficiAgo(IMaggiorazioneBeneficiAgo maggBen)
        {
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(maggBen.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.StoreDatiBenefici(ndomus, maggBen.areaMaggiorazioneBenefici);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoAgo.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo SalvaBeneficiAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }

        public void EliminaBeneficiAgo(IMaggiorazioneBeneficiAgo maggBen)
        {
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaMaggiorazioniBenefici areaMaggiorazioniBenefici = null;
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(maggBen.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.CancelDatiBenefici(out areaMaggiorazioniBenefici, ndomus);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoAgo.Messaggio;
                }

                if (areaMaggiorazioniBenefici != null)
                    maggBen.areaMaggiorazioneBenefici = areaMaggiorazioniBenefici;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo EliminaBeneficiAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }

        #endregion Dati Benefici

        #region Dati Maggiorazioni

        public void SalvaMaggiorazioniAgo(IMaggiorazioneBeneficiAgo maggBen)
        {
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(maggBen.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.StoreDatiMaggiorazioni(ndomus, maggBen.areaMaggiorazioneBenefici);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoAgo.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo SalvaMaggiorazioniAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }

        public void EliminaMaggiorazioniAgo(IMaggiorazioneBeneficiAgo maggBen)
        {
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaMaggiorazioniBenefici areaMaggiorazioniBenefici = null;
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(maggBen.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.CancelDatiMaggiorazioni(out areaMaggiorazioniBenefici, ndomus);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoAgo.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo EliminaMaggiorazioniAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }

        #endregion Dati Maggiorazioni

        #region Dati Ex Combattente

        public void SalvaExCombattenteAgo(IMaggiorazioneBeneficiAgo maggBen)
        {
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(maggBen.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.StoreDatiExCombattente(ndomus, maggBen.areaMaggiorazioneBenefici);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoAgo.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo SalvaExCombattenteAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }

        public void EliminaExCombattenteAgo(IMaggiorazioneBeneficiAgo maggBen)
        {
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaMaggiorazioniBenefici areaMaggiorazioniBenefici = null;
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(maggBen.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.CancelDatiExCombattente(out areaMaggiorazioniBenefici, ndomus);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoAgo.Messaggio;
                }

                if (areaMaggiorazioniBenefici != null)
                    maggBen.areaMaggiorazioneBenefici = areaMaggiorazioniBenefici;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo EliminaExCombattenteAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }

        #endregion Dati Ex Combattente

        #region Dati Vittime

        public void SalvaVittime(IMaggiorazioneBeneficiAgo maggBen)
        {
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(maggBen.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.StoreDatiBeneficioVittimeTerrorismo(ndomus, maggBen.areaMaggiorazioneBenefici);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoAgo.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo SalvaVittime");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }

        public void EliminaVittime(IMaggiorazioneBeneficiAgo maggBen)
        {
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaMaggiorazioniBenefici areaMaggiorazioniBenefici = null;
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(maggBen.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.CancelDatiBeneficioVittimeTerrorismo(out areaMaggiorazioniBenefici, ndomus);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    maggBen.HasError = true;
                    maggBen.ErrorMessage = esitoAgo.Messaggio;
                }

                if (areaMaggiorazioniBenefici != null)
                    maggBen.areaMaggiorazioneBenefici = areaMaggiorazioniBenefici;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMaggiorazioneBenefici, Errore nel metodo EliminaVittime");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }

        #endregion Dati Vittime

        #endregion AGO
    }
}
