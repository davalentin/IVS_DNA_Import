using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.DNA;
using INPS.DNA.Logging;
using INPS.DNA.Services;
using INPS.DNA.Services.FaultContract;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterLiquidazionePensione
    {
        #region FS

        public void GetLiquidazionePensione(ILiquidazionePensione liquidazionePensione)
        {
            SvrLiquidazioneFs.AreaLiquidazionePensione areaLiquidazionePensioneFS = new SvrLiquidazioneFs.AreaLiquidazionePensione();
            Presenter.SvrLiquidazioneFs.AreaEsito esitoFS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
            Presenter.SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWSFS = new ServizioLiquidazioneFsClient();
            Presenter.SvrLiquidazioneFs.AreaRichiestaDomanda areaRichiestaDomanda = new Presenter.SvrLiquidazioneFs.AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(liquidazionePensione.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = liquidazionePensione.domanda.ProgStorico;
            try
            {
                esitoFS = objWSFS.GetLiquidazionePensioneByDomanda(out areaLiquidazionePensioneFS, areaRichiestaDomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo GetLiquidazionePensione");
            }
            finally
            {
                Utility.CloseClient(objWSFS);
            }

            if (esitoFS.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
            {
                liquidazionePensione.HasError = true;
                liquidazionePensione.ErrorMessage = "Errore tecnico nel recupero dei dati del quadro LiquidazionePensione";
            }
            else
            {
                liquidazionePensione.HasError = false;
                liquidazionePensione.ErrorMessage = string.Empty;
                liquidazionePensione.areaLiquidazionePensioneFS = areaLiquidazionePensioneFS;
            }
        }

        public void SalvaDatiGenerici(ILiquidazionePensione liquidazione)
        {
            string sErrore;
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWSFS = new ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoFS = objWSFS.StoreDatiGenerici(ndomus, liquidazione.areaLiquidazionePensioneFS);

                if (esitoFS.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoFS.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoFS.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaDatiGenerici");
            }
            finally
            {
                Utility.CloseClient(objWSFS);
            }
        }

        public void EliminaDatiGenerici(ILiquidazionePensione liquidazione)
        {
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWSFS = new ServizioLiquidazioneFsClient();
            AreaLiquidazionePensione areaLiquidazionePensione = new AreaLiquidazionePensione();
            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoFS = objWSFS.CancelDatiGenerici(out areaLiquidazionePensione, ndomus);
                if (esitoFS.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoFS.Messaggio;
                }
                else
                {
                    liquidazione.HasError = false;
                    liquidazione.ErrorMessage = "";
                    liquidazione.areaLiquidazionePensioneFS = new AreaLiquidazionePensione();
                    liquidazione.areaLiquidazionePensioneFS = areaLiquidazionePensione;
                }

            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo EliminaDatiGenerici");
            }
            finally
            {
                Utility.CloseClient(objWSFS);
            }
        }

        public void SalvaPrecedentePensione(ILiquidazionePensione liquidazione)
        {
            string sErrore;
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWSFS = new ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoFS = objWSFS.StoreDatiPrecedentePensione(ndomus, liquidazione.areaLiquidazionePensioneFS);

                if (esitoFS.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoFS.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoFS.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaPrecedentePensione");
            }
            finally
            {
                Utility.CloseClient(objWSFS);
            }
        }

        public void EliminaPrecedentePensione(ILiquidazionePensione liquidazione)
        {
            string sErrore;
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWSFS = new ServizioLiquidazioneFsClient();

            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoFS = objWSFS.CancelDatiPrecedentePensione(ndomus);

                if (esitoFS.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoFS.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoFS.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo EliminaPrecedentePensione");
            }
            finally
            {
                Utility.CloseClient(objWSFS);
            }
        }

        public void SalvaLiquidazionePensione(ILiquidazionePensione liquidazione)
        {
            string sErrore;
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWSFS = new ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoFS = objWSFS.StoreLiquidazionePensione(ndomus, liquidazione.areaLiquidazionePensioneFS);

                if (esitoFS.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoFS.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoFS.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaLiquidazionePensione");
            }
            finally
            {
                Utility.CloseClient(objWSFS);
            }
        }

        public void SalvaDatiAssicurativiFS(ILiquidazionePensione liquidazione)
        {
            string sErrore;
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWSFS = new ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoFS = objWSFS.StoreDatiAssicurativi(ndomus, liquidazione.areaLiquidazionePensioneFS);

                if (esitoFS.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoFS.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoFS.Messaggio;
                }
                else
                    liquidazione.HasError = false;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaDatiAssicurativiFS");
            }
            finally
            {
                Utility.CloseClient(objWSFS);
            }
        }

        public void EliminaDatiAssicurativiFS(ILiquidazionePensione liquidazione)
        {
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWSFS = new ServizioLiquidazioneFsClient();
            AreaLiquidazionePensione areaLiquidazionePensione = null;
            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoFS = objWSFS.CancelDatiAssicurativi(out areaLiquidazionePensione, ndomus);

                if (esitoFS.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoFS.Messaggio;
                }
                else
                {
                    liquidazione.HasError = false;
                    liquidazione.ErrorMessage = "";
                    liquidazione.areaLiquidazionePensioneFS = new AreaLiquidazionePensione();
                    liquidazione.areaLiquidazionePensioneFS = areaLiquidazionePensione;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo EliminaDatiAssicurativiFS");
            }
            finally
            {
                Utility.CloseClient(objWSFS);
            }
        }

        public void SalvaBititolaritaInail(ILiquidazionePensione liquidazione)
        {
            string sErrore;
            ServizioLiquidazioneFsClient objFSWS = new ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esito = objFSWS.StoreDatiBititolaritaInail(ndomus, liquidazione.areaLiquidazionePensioneFS);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esito.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaBititolaritaInail");
            }
            finally
            {
                Utility.CloseClient(objFSWS);
            }
        }

        public void EliminaBititolaritaInail(ILiquidazionePensione liquidazione)
        {
            string sErrore;
            ServizioLiquidazioneFsClient objFSWS = new ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esito = new SvrLiquidazioneFs.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esito = objFSWS.CancelDatiBititolaritaInail(ndomus);

                if (esito.RisultatoOperazione == SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esito.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo EliminaBititolaritaInail");
            }
            finally
            {
                Utility.CloseClient(objFSWS);
            }
        }

        public void SalvaDatiLegge460(ILiquidazionePensione liquidazione)
        {
            string sErrore;
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWSFS = new ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoFS = objWSFS.StoreDatiLegge460(ndomus, liquidazione.areaLiquidazionePensioneFS);

                if (esitoFS.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoFS.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoFS.Messaggio;
                }
                else
                {
                    liquidazione.HasError = false;

                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaDatiLegge460");
            }
            finally
            {
                Utility.CloseClient(objWSFS);
            }
        }

        public void EliminaDatiLegge460(ILiquidazionePensione liquidazione)
        {
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWSFS = new ServizioLiquidazioneFsClient();
            AreaLiquidazionePensione areaLiquidazionePensione = null;
            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoFS = objWSFS.CancelDatiLegge460(ndomus);

                if (esitoFS.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoFS.Messaggio;
                }
                else
                {
                    liquidazione.HasError = false;
                    liquidazione.ErrorMessage = "";
                    liquidazione.areaLiquidazionePensioneFS = new AreaLiquidazionePensione();
                    liquidazione.areaLiquidazionePensioneFS = areaLiquidazionePensione;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo EliminaDatiLegge460");
            }
            finally
            {
                Utility.CloseClient(objWSFS);
            }
        }

        public void SalvaDatiIstruttoriaFS(ILiquidazionePensione liquidazione)
        {
            string sErrore;
            ServizioLiquidazioneFsClient objWSFS = new ServizioLiquidazioneFsClient();
            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFS = new SvrLiquidazioneFs.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoFS = objWSFS.StoreDatiIstruttoria(ndomus, liquidazione.areaLiquidazionePensioneFS);

                if (esitoFS.RisultatoOperazione == SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoFS.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoFS.Messaggio;
                }
                else
                {
                    liquidazione.HasError = false;

                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaDatiIstruttoriaFS");
            }
            finally
            {
                Utility.CloseClient(objWSFS);
            }
        }

        public void EliminaDatiIstruttoriaFS(ILiquidazionePensione liquidazione)
        {
            ServizioLiquidazioneFsClient objWSFS = new ServizioLiquidazioneFsClient();
            AreaLiquidazionePensione areaLiquidazionePensione = null;
            try
            {
                SvrLiquidazioneFs.AreaEsito esitoFS = new SvrLiquidazioneFs.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoFS = objWSFS.CancelDatiIstruttoria(out areaLiquidazionePensione, ndomus);

                if (esitoFS.RisultatoOperazione == SvrLiquidazioneFs.AreaEsito.TipoEsito.KO)
                {
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoFS.Messaggio;
                }
                else
                {
                    liquidazione.HasError = false;
                    liquidazione.ErrorMessage = "";
                    liquidazione.areaLiquidazionePensioneFS = new AreaLiquidazionePensione();
                    liquidazione.areaLiquidazionePensioneFS = areaLiquidazionePensione;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo EliminaDatiIstruttoria");
            }
            finally
            {
                Utility.CloseClient(objWSFS);
            }
        }
        
        public void VerificaAdesioneFondoCredito(string codiceFiscaleTitolare, ILiquidazionePensione liquidazionePensioneFs)
        {
            try
            {
                SvrLiquidazione.AreaEsito esito = VerificaAdesioneFondoCredito(codiceFiscaleTitolare);
                if (esito.RisultatoOperazione == SvrLiquidazione.AreaEsito.TipoEsito.OK)
                {
                    liquidazionePensioneFs.HasError = false;
                    liquidazionePensioneFs.ErrorMessage = string.Empty;
                }
                else
                {
                    liquidazionePensioneFs.HasError = true;
                    liquidazionePensioneFs.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo VerificaAdesioneFondoCredito");
            }
        }

        #endregion FS

        #region AGO

        public void GetLiquidazionePensioneAgo(ILiquidazionePensioneAgo liquidazionePensioneAgo)
        {
            SvrLiquidazioneAgo.AreaLiquidazionePensione areaLiquidazionePensioneAgo = new SvrLiquidazioneAgo.AreaLiquidazionePensione();
            Presenter.SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
            Presenter.SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAGO = new Presenter.SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            Presenter.SvrLiquidazioneAgo.AreaRichiestaDomanda areaRichiestaDomanda = new Presenter.SvrLiquidazioneAgo.AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(liquidazionePensioneAgo.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = liquidazionePensioneAgo.domanda.ProgStorico;
            try
            {
                esitoAgo = objWSAGO.GetLiquidazionePensioneByDomanda(out areaLiquidazionePensioneAgo, areaRichiestaDomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo GetLiquidazionePensioneAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAGO);
            }

            if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
            {
                liquidazionePensioneAgo.HasError = true;
                liquidazionePensioneAgo.ErrorMessage = "Errore tecnico nel recupero dei dati del quadro LiquidazionePensione";
            }
            else
            {
                liquidazionePensioneAgo.areaLiquidazionePensioneAgo = areaLiquidazionePensioneAgo;
            }
        }

        public void SalvaLiquidazionePensioneAgo(ILiquidazionePensioneAgo liquidazione)
        {
            string sErrore;
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.StoreDatiLiquidazionePensione(ndomus, liquidazione.areaLiquidazionePensioneAgo);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoAgo.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoAgo.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaLiquidazionePensioneAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }

        #region Dati Generici
        public void SalvaDatiGenericiAgo(ILiquidazionePensioneAgo liquidazione)
        {
            string sErrore;
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.StoreDatiGenerici(ndomus, liquidazione.areaLiquidazionePensioneAgo);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoAgo.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoAgo.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaDatiGenericiAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }

        public void EliminaDatiGenericiAgo(ILiquidazionePensioneAgo liquidazione)
        {
            SvrLiquidazioneAgo.AreaLiquidazionePensione areaLiquidazionePensioneAgo = new SvrLiquidazioneAgo.AreaLiquidazionePensione();
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.CancelDatiGenerici(out areaLiquidazionePensioneAgo, ndomus);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.OK && !String.IsNullOrEmpty(esitoAgo.Messaggio))
                {
                    liquidazione.HasError = false;
                    liquidazione.ErrorMessage = esitoAgo.Messaggio;
                }
                else if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.OK)
                {
                    liquidazione.HasError = false;
                    liquidazione.ErrorMessage = string.Empty;
                }
                else if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoAgo.Messaggio;
                }

                liquidazione.areaLiquidazionePensioneAgo = areaLiquidazionePensioneAgo;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo EliminaDatiGenericiAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }

        public void VerificaAdesioneFondoCredito(string codiceFiscaleTitolare, ILiquidazionePensioneAgo liquidazionePensioneAgo)
        {
            try
            {
                SvrLiquidazione.AreaEsito esito = VerificaAdesioneFondoCredito(codiceFiscaleTitolare);
                if (esito.RisultatoOperazione == SvrLiquidazione.AreaEsito.TipoEsito.OK)
                {
                    liquidazionePensioneAgo.HasError = false;
                    liquidazionePensioneAgo.ErrorMessage = string.Empty;
                }
                else
                {
                    liquidazionePensioneAgo.HasError = true;
                    liquidazionePensioneAgo.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo VerificaAdesioneFondoCredito");
            }

        }

        #endregion Dati Generici

        #region Dati Assicurativi
        public void SalvaDatiAssicurativiAgo(ILiquidazionePensioneAgo liquidazione)
        {
            string sErrore;
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.StoreDatiAssicurativi(ndomus, liquidazione.areaLiquidazionePensioneAgo);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoAgo.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoAgo.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaDatiAssicurativiAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }

        public void EliminaDatiAssicurativiAgo(ILiquidazionePensioneAgo liquidazione)
        {
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaLiquidazionePensione areaLiquidazionePensioneAgo = new SvrLiquidazioneAgo.AreaLiquidazionePensione();
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.CancelDatiAssicurativi(out areaLiquidazionePensioneAgo, ndomus);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.OK && !String.IsNullOrEmpty(esitoAgo.Messaggio))
                {
                    liquidazione.HasError = false;
                    liquidazione.ErrorMessage = esitoAgo.Messaggio;
                }
                else if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.OK)
                {
                    liquidazione.HasError = false;
                    liquidazione.ErrorMessage = string.Empty;
                }
                else if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoAgo.Messaggio;
                }

                liquidazione.areaLiquidazionePensioneAgo = areaLiquidazionePensioneAgo;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo EliminaDatiAssicurativiAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }
        #endregion Dati Assicurativi

        #region Dati Opzione
        public void SalvaDatiOpzioneAgo(ILiquidazionePensioneAgo liquidazione)
        {
            string sErrore;
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.StoreDatiOpzione(ndomus, liquidazione.areaLiquidazionePensioneAgo);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoAgo.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoAgo.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaDatiOpzioneAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }

        public void EliminaDatiOpzioneAgo(ILiquidazionePensioneAgo liquidazione)
        {
            string sErrore;
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.CancelDatiOpzione(ndomus);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoAgo.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoAgo.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo EliminaDatiOpzioneAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }
        #endregion Dati Opzione

        #region Dati Precedente Pensione
        public void SalvaDatiPrecedentePensioneAgo(ILiquidazionePensioneAgo liquidazione)
        {
            string sErrore;
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.StoreDatiProvenienza(ndomus, liquidazione.areaLiquidazionePensioneAgo);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoAgo.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoAgo.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaDatiPrecedentePensioneAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }

        public void EliminaDatiPrecedentePensioneAgo(ILiquidazionePensioneAgo liquidazione)
        {
            string sErrore;
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.CancelDatiProvenienza(ndomus);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoAgo.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoAgo.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo EliminaDatiPrecedentePensioneAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }
        #endregion Dati Precedente Pensione

        #region Dati Istruttoria
        public void SalvaDatiIstruttoriaAgo(ILiquidazionePensioneAgo liquidazione)
        {
            string sErrore;
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.StoreDatiIstruttoria(ndomus, liquidazione.areaLiquidazionePensioneAgo);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoAgo.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoAgo.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaDatiIstruttoriaAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }

        public void EliminaDatiIstruttoriaAgo(ILiquidazionePensioneAgo liquidazione)
        {
            string sErrore;
            SvrLiquidazioneAgo.AreaLiquidazionePensione areaLiquidazionePensioneAgo = new SvrLiquidazioneAgo.AreaLiquidazionePensione();
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.CancelDatiIstruttoria(out areaLiquidazionePensioneAgo, ndomus);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoAgo.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoAgo.Messaggio;
                }
                else
                {
                    liquidazione.HasError = false;
                    liquidazione.ErrorMessage = "";
                    liquidazione.areaLiquidazionePensioneAgo = new SvrLiquidazioneAgo.AreaLiquidazionePensione();
                    liquidazione.areaLiquidazionePensioneAgo = areaLiquidazionePensioneAgo;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo EliminaDatiIstruttoriaAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }
        #endregion Dati Istruttoria

        #region Dati INAIL
        public void SalvaDatiInailAgo(ILiquidazionePensioneAgo liquidazione)
        {
            string sErrore;
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.StoreDatiInail(ndomus, liquidazione.areaLiquidazionePensioneAgo);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoAgo.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoAgo.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaDatiInailAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }

        public void EliminaDatiInailAgo(ILiquidazionePensioneAgo liquidazione)
        {
            string sErrore;
            SvrLiquidazioneAgo.AreaLiquidazionePensione areaLiquidazionePensioneAgo = new SvrLiquidazioneAgo.AreaLiquidazionePensione();
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.CancelDatiInail(ndomus);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoAgo.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoAgo.Messaggio;
                }
                else
                {
                    liquidazione.HasError = false;
                    liquidazione.ErrorMessage = "";
                    liquidazione.areaLiquidazionePensioneAgo = new SvrLiquidazioneAgo.AreaLiquidazionePensione();
                    liquidazione.areaLiquidazionePensioneAgo = areaLiquidazionePensioneAgo;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo EliminaDatiInailAgo");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }
        #endregion Dati INAIL

        #region Sentenza Art. 4
        public void SalvaDatiTabSentenzaArt4(ILiquidazionePensioneAgo liquidazione)
        {
            string sErrore;
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.StoreDatiSentenzaArt4(ndomus, liquidazione.areaLiquidazionePensioneAgo);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoAgo.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoAgo.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaDatiTabSentenzaArt4");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }

        public void EliminaDatiTabSentenzaArt4(ILiquidazionePensioneAgo liquidazione)
        {
            string sErrore;
            SvrLiquidazioneAgo.AreaLiquidazionePensione areaLiquidazionePensioneAgo = new SvrLiquidazioneAgo.AreaLiquidazionePensione();
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.CancelDatiSentenzaArt4(out areaLiquidazionePensioneAgo, ndomus);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoAgo.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoAgo.Messaggio;
                }
                else
                {
                    liquidazione.HasError = false;
                    liquidazione.ErrorMessage = "";
                    liquidazione.areaLiquidazionePensioneAgo = new SvrLiquidazioneAgo.AreaLiquidazionePensione();
                    liquidazione.areaLiquidazionePensioneAgo = areaLiquidazionePensioneAgo;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo EliminaDatiTabSentenzaArt4");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }
        #endregion Sentenza Art. 4

        #region Sentenze
        public void SalvaDatiTabSentenze(ILiquidazionePensioneAgo liquidazione)
        {
            string sErrore;
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.StoreDatiSentenze(ndomus, liquidazione.areaLiquidazionePensioneAgo);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoAgo.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoAgo.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaDatiTabSentenze");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }

        public void EliminaDatiTabSentenze(ILiquidazionePensioneAgo liquidazione)
        {
            string sErrore;
            SvrLiquidazioneAgo.AreaLiquidazionePensione areaLiquidazionePensioneAgo = new SvrLiquidazioneAgo.AreaLiquidazionePensione();
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWSAgo = new SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            try
            {
                SvrLiquidazioneAgo.AreaEsito esitoAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.domanda.NumeroDomanda);
                esitoAgo = objWSAgo.CancelDatiSentenze(ndomus);

                if (esitoAgo.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoAgo.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoAgo.Messaggio;
                }
                else
                {
                    liquidazione.HasError = false;
                    liquidazione.ErrorMessage = "";
                    liquidazione.areaLiquidazionePensioneAgo = new SvrLiquidazioneAgo.AreaLiquidazionePensione();
                    liquidazione.areaLiquidazionePensioneAgo = areaLiquidazionePensioneAgo;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo EliminaDatiTabSentenze");
            }
            finally
            {
                Utility.CloseClient(objWSAgo);
            }
        }

        #endregion Sentenze

        #endregion AGO

        #region CI

        public void GetLiquidazionePensioneCi(ILiquidazionePensioneCi liquidazionePensioneCi)
        {
            SvrLiquidazioneCi.AreaLiquidazionePensione areaLiquidazionePensioneCi = new SvrLiquidazioneCi.AreaLiquidazionePensione();
            Presenter.SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
            Presenter.SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new Presenter.SvrLiquidazioneCi.ServizioLiquidazioneCiClient();
            Presenter.SvrLiquidazioneCi.AreaRichiestaDomanda areaRichiestaDomanda = new Presenter.SvrLiquidazioneCi.AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(liquidazionePensioneCi.areaRiepilogoDomanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = liquidazionePensioneCi.areaRiepilogoDomanda.ProgStorico;
            try
            {
                esitoCi = objWSCi.GetLiquidazionePensioneByDomanda(out areaLiquidazionePensioneCi, areaRichiestaDomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo GetLiquidazionePensioneCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }

            if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
            {
                liquidazionePensioneCi.HasError = true;
                liquidazionePensioneCi.ErrorMessage = "Errore tecnico nel recupero dei dati del quadro LiquidazionePensione";
            }
            else
            {
                liquidazionePensioneCi.areaLiquidazionePensioneCi = areaLiquidazionePensioneCi;
            }
        }

        public void SalvaLiquidazionePensioneCi(ILiquidazionePensioneCi liquidazione)
        {
            string sErrore;
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new SvrLiquidazioneCi.ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.areaRiepilogoDomanda.NumeroDomanda);
                esitoCi = objWSCi.StoreDatiLiquidazionePensione(ndomus, liquidazione.areaLiquidazionePensioneCi);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaLiquidazionePensioneCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        #region Dati Generici
        public void SalvaDatiGenericiCi(ILiquidazionePensioneCi liquidazione)
        {
            string sErrore;
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new SvrLiquidazioneCi.ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.areaRiepilogoDomanda.NumeroDomanda);
                esitoCi = objWSCi.StoreDatiGenerici(ndomus, liquidazione.areaLiquidazionePensioneCi);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaDatiGenericiCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        public void EliminaDatiGenericiCi(ILiquidazionePensioneCi liquidazione)
        {
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new SvrLiquidazioneCi.ServizioLiquidazioneCiClient();
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                SvrLiquidazioneCi.AreaLiquidazionePensione areaLiquidazionePensioneCi = new SvrLiquidazioneCi.AreaLiquidazionePensione();
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.areaRiepilogoDomanda.NumeroDomanda);
                esitoCi = objWSCi.CancelDatiGenerici(out areaLiquidazionePensioneCi, ndomus);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.OK && !String.IsNullOrEmpty(esitoCi.Messaggio))
                {
                    liquidazione.HasError = false;
                    liquidazione.ErrorMessage = esitoCi.Messaggio;
                }
                else if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.OK)
                {
                    liquidazione.HasError = false;
                    liquidazione.ErrorMessage = string.Empty;
                }
                else if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoCi.Messaggio;
                }

                liquidazione.areaLiquidazionePensioneCi = areaLiquidazionePensioneCi;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo EliminaDatiGenericiCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        public void VerificaAdesioneFondoCredito(string codiceFiscaleTitolare, ILiquidazionePensioneCi liquidazionePensioneCi)
        {
            try
            {
                SvrLiquidazione.AreaEsito esito = VerificaAdesioneFondoCredito(codiceFiscaleTitolare);
                if (esito.RisultatoOperazione == SvrLiquidazione.AreaEsito.TipoEsito.OK)
                {
                    liquidazionePensioneCi.HasError = false;
                    liquidazionePensioneCi.ErrorMessage = string.Empty;
                }
                else
                {
                    liquidazionePensioneCi.HasError = true;
                    liquidazionePensioneCi.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo VerificaAdesioneFondoCredito");
            }
        }

        #endregion Dati Generici

        #region Dati Assicurativi
        public void SalvaDatiAssicurativiCi(ILiquidazionePensioneCi liquidazione)
        {
            string sErrore;
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new SvrLiquidazioneCi.ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.areaRiepilogoDomanda.NumeroDomanda);
                esitoCi = objWSCi.StoreDatiAssicurativi(ndomus, liquidazione.areaLiquidazionePensioneCi);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaDatiAssicurativiCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        public void EliminaDatiAssicurativiCi(ILiquidazionePensioneCi liquidazione)
        {
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new SvrLiquidazioneCi.ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaLiquidazionePensione areaLiquidazionePensioneCi = new SvrLiquidazioneCi.AreaLiquidazionePensione();
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.areaRiepilogoDomanda.NumeroDomanda);
                esitoCi = objWSCi.CancelDatiAssicurativi(out areaLiquidazionePensioneCi, ndomus);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.OK && !String.IsNullOrEmpty(esitoCi.Messaggio))
                {
                    liquidazione.HasError = false;
                    liquidazione.ErrorMessage = esitoCi.Messaggio;
                }
                else if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.OK)
                {
                    liquidazione.HasError = false;
                    liquidazione.ErrorMessage = string.Empty;
                }
                else if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoCi.Messaggio;
                }

                liquidazione.areaLiquidazionePensioneCi = areaLiquidazionePensioneCi;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo EliminaDatiAssicurativiCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }
        #endregion Dati Assicurativi

        #region Dati Opzione
        public void SalvaDatiOpzioneCi(ILiquidazionePensioneCi liquidazione)
        {
            string sErrore;
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new SvrLiquidazioneCi.ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.areaRiepilogoDomanda.NumeroDomanda);
                esitoCi = objWSCi.StoreDatiOpzione(ndomus, liquidazione.areaLiquidazionePensioneCi);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaDatiOpzioneCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        public void EliminaDatiOpzioneCi(ILiquidazionePensioneCi liquidazione)
        {
            string sErrore;
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new SvrLiquidazioneCi.ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.areaRiepilogoDomanda.NumeroDomanda);
                esitoCi = objWSCi.CancelDatiOpzione(ndomus);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo EliminaDatiOpzioneCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }
        #endregion Dati Opzione

        #region Dati Precedente Pensione
        public void SalvaDatiPrecedentePensioneCi(ILiquidazionePensioneCi liquidazione)
        {
            string sErrore;
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new SvrLiquidazioneCi.ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.areaRiepilogoDomanda.NumeroDomanda);
                esitoCi = objWSCi.StoreDatiProvenienza(ndomus, liquidazione.areaLiquidazionePensioneCi);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaDatiPrecedentePensioneCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        public void EliminaDatiPrecedentePensioneCi(ILiquidazionePensioneCi liquidazione)
        {
            string sErrore;
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new SvrLiquidazioneCi.ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.areaRiepilogoDomanda.NumeroDomanda);
                esitoCi = objWSCi.CancelDatiProvenienza(ndomus);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo EliminaDatiPrecedentePensioneCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }
        #endregion Dati Precedente Pensione

        #region Dati Istruttoria
        public void SalvaDatiIstruttoriaCi(ILiquidazionePensioneCi liquidazione)
        {
            string sErrore;
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new SvrLiquidazioneCi.ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.areaRiepilogoDomanda.NumeroDomanda);
                esitoCi = objWSCi.StoreDatiIstruttoria(ndomus, liquidazione.areaLiquidazionePensioneCi);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaDatiIstruttoriaCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        public void EliminaDatiIstruttoriaCi(ILiquidazionePensioneCi liquidazione)
        {
            string sErrore;
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new SvrLiquidazioneCi.ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaLiquidazionePensione areaLiquidazionePensioneCi = new SvrLiquidazioneCi.AreaLiquidazionePensione();
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.areaRiepilogoDomanda.NumeroDomanda);
                esitoCi = objWSCi.CancelDatiIstruttoria(out areaLiquidazionePensioneCi, ndomus);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoCi.Messaggio;
                }
                else
                {
                    liquidazione.HasError = false;
                    liquidazione.ErrorMessage = "";
                    liquidazione.areaLiquidazionePensioneCi = new SvrLiquidazioneCi.AreaLiquidazionePensione();
                    liquidazione.areaLiquidazionePensioneCi = areaLiquidazionePensioneCi;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo EliminaDatiIstruttoriaCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }
        #endregion Dati Istruttoria

        #region Dati Inail
        public void SalvaDatiInailCi(ILiquidazionePensioneCi liquidazione)
        {
            string sErrore;
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new SvrLiquidazioneCi.ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.areaRiepilogoDomanda.NumeroDomanda);
                esitoCi = objWSCi.StoreDatiInail(ndomus, liquidazione.areaLiquidazionePensioneCi);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo SalvaDatiInailCi");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }

        public void EliminaDatiInaiCi(ILiquidazionePensioneCi liquidazione) 
        {
            string sErrore;
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWSCi = new SvrLiquidazioneCi.ServizioLiquidazioneCiClient();
            try
            {
                SvrLiquidazioneCi.AreaEsito esitoCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
                long ndomus = Int64.Parse(liquidazione.areaRiepilogoDomanda.NumeroDomanda);
                esitoCi = objWSCi.CancelDatiInail(ndomus);

                if (esitoCi.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esitoCi.Messaggio;
                    liquidazione.HasError = true;
                    liquidazione.ErrorMessage = esitoCi.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo Elimina Dati Inail");
            }
            finally
            {
                Utility.CloseClient(objWSCi);
            }
        }
        #endregion Dati Inail

        #endregion CI

        #region ALL
        private SvrLiquidazione.AreaEsito VerificaAdesioneFondoCredito(string codiceFiscaleTitolare)
        {
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new SvrLiquidazione.ServizioLiquidazioneClient();
            SvrLiquidazione.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito();
            try
            {
                esito = objWS.VerificaAdesioneFondoCredito(codiceFiscaleTitolare);           
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterLiquidazionePensione, Errore nel metodo VerificaAdesioneFondoCredito");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            return esito;
        }

        #endregion

        public Boolean CheckCampi(ILiquidazionePensione liquidazionePensione)
        {
            try
            {
                string sErrore = String.Empty;
                if (!Utility.checkVerify("SI", out sErrore))
                {
                    SetError(liquidazionePensione, sErrore);
                    return false;
                }
                if (!Utility.CheckDecorrenzaOriginaria("12/2010", out sErrore))
                {
                    SetError(liquidazionePensione, sErrore);
                    return false;
                }

                return true;
            }
            catch (Exception Ex)
            {
                throw new INPS.DNA.DnaApplicationException("PresenterLiquidazionePensione, Errore nel metodo CheckCampi", Ex);
            }
        }

        private void SetError(ILiquidazionePensione liquidazionePensione, String sErrore)
        {
            liquidazionePensione.ErrorMessage = sErrore;
            liquidazionePensione.HasError = true;
            return;
        }
    }
}
