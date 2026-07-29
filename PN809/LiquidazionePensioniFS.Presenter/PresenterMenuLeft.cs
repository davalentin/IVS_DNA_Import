using System;
using System.ServiceModel;

using INPS.DNA;

using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterMenuLeft
    {
        public void GetInfoQuadri(IQuadriSemafori quadro)
        {
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(quadro.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = quadro.domanda.ProgStorico;
            AreaQuadri areaQuadri = new AreaQuadri();
            SvrLiquidazione.QuadriClient quadriClient = new QuadriClient();
            try
            {
                areaQuadri = quadriClient.GetQuadriByDomanda(areaRichiestaDomanda);
                quadro.areaQuadri = areaQuadri;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMenuLeft, Errore nel metodo GetInfoQuadri");
            }
            finally
            {
                Utility.CloseClient(quadriClient);
            }
        }

        public void GetQuadri(IQuadriSemafori quadro)
        {
            AreaEsito esito = new AreaEsito();
            AreaInfoPratica areaInfoPratica = quadro.areaInfoPratica;
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(quadro.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = quadro.domanda.ProgStorico;
            SvrLiquidazione.QuadriClient quadriClient = new QuadriClient();
            try
            {
                esito = quadriClient.AggiornaQuadri(areaRichiestaDomanda, ref areaInfoPratica);
                quadro.areaInfoPratica = areaInfoPratica;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMenuLeft, Errore nel metodo GetQuadri");
            }
            finally
            {
                Utility.CloseClient(quadriClient);
            }

            if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                throw new DnaApplicationException(esito.Messaggio);
        }

        public void GetQuadroTitolare(IQuadriSemafori quadro)
        {
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(quadro.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = quadro.domanda.ProgStorico;
            AreaQuadri.DatiQuadroTitolare areaQuadriTitolare = new AreaQuadri.DatiQuadroTitolare();
            QuadriClient quadriClient = new QuadriClient();
            try
            {
                areaQuadriTitolare = quadriClient.GetQuadroTitolareByDomanda(areaRichiestaDomanda);
                quadro.areaQuadri.QuadroTitolare = areaQuadriTitolare;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMenuLeft, Errore nel metodo GetQuadroTitolare");
            }
            finally
            {
                Utility.CloseClient(quadriClient);
            }
        }

        public void GetQuadroDetrazioni(IQuadriSemafori quadro)
        {
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(quadro.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = quadro.domanda.ProgStorico;
            AreaQuadri.DatiQuadroDetrazioni areaQuadriDetrazioni = new AreaQuadri.DatiQuadroDetrazioni();
            SvrLiquidazione.QuadriClient quadriClient = new QuadriClient();
            try
            {
                areaQuadriDetrazioni = quadriClient.GetQuadroDetrazioniByDomanda(areaRichiestaDomanda);
                quadro.areaQuadri.QuadroDetrazioni = areaQuadriDetrazioni;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMenuLeft, Errore nel metodo GetQuadroDetrazioni");
            }
            finally
            {
                Utility.CloseClient(quadriClient);
            }
        }

        public void GetQuadroPagamenti(IQuadriSemafori quadro)
        {
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(quadro.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = quadro.domanda.ProgStorico;
            AreaQuadri.DatiQuadroPagamento areaQuadriPagamento = new AreaQuadri.DatiQuadroPagamento();
            SvrLiquidazione.QuadriClient quadriClient = new QuadriClient();
            try
            {
                areaQuadriPagamento = quadriClient.GetQuadroPagamentoByDomanda(areaRichiestaDomanda);
                quadro.areaQuadri.QuadroPagamento = areaQuadriPagamento;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMenuLeft, Errore nel metodo GetQuadroPagamenti");
            }
            finally
            {
                Utility.CloseClient(quadriClient);
            }
        }

        public void GetQuadroMaggiorazioneBenefici(IQuadriSemafori quadro)
        {
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(quadro.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = quadro.domanda.ProgStorico;
            AreaQuadri.DatiQuadroMaggiorazioniBenefici areaQuadriMaggiorazioneBenefici = new AreaQuadri.DatiQuadroMaggiorazioniBenefici();
            SvrLiquidazione.QuadriClient quadriClient = new QuadriClient();
            try
            {
                areaQuadriMaggiorazioneBenefici = quadriClient.GetQuadroMaggiorazioniBeneficiByDomanda(areaRichiestaDomanda);
                quadro.areaQuadri.QuadroMaggiorazioniBenefici = areaQuadriMaggiorazioneBenefici;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMenuLeft, Errore nel metodo GetQuadroMaggiorazioneBenefici");
            }
            finally
            {
                Utility.CloseClient(quadriClient);
            }
        }

        public void GetQuadroDelegatoTutore(IQuadriSemafori quadro)
        {
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(quadro.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = quadro.domanda.ProgStorico;
            AreaQuadri.DatiQuadroDelegatoTutore areaQuadriDelegatoTutore = new AreaQuadri.DatiQuadroDelegatoTutore();
            SvrLiquidazione.QuadriClient quadriClient = new QuadriClient();
            try
            {
                areaQuadriDelegatoTutore = quadriClient.GetQuadroDelegatoTutoreByDomanda(areaRichiestaDomanda);
                quadro.areaQuadri.QuadroDelegatoTutore = areaQuadriDelegatoTutore;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMenuLeft, Errore nel metodo GetQuadroDelegatoTutore");
            }
            finally
            {
                Utility.CloseClient(quadriClient);
            }
        }

        public void GetQuadroLiquidazionePensione(IQuadriSemafori quadro)
        {
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(quadro.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = quadro.domanda.ProgStorico;
            AreaQuadri.DatiQuadroLiquidazionePensione areaQuadriLiquidazionePensione = new AreaQuadri.DatiQuadroLiquidazionePensione();
            SvrLiquidazione.QuadriClient quadriClient = new QuadriClient();
            try
            {
                areaQuadriLiquidazionePensione = quadriClient.GetQuadroLiquidazionePensioneByDomanda(areaRichiestaDomanda);
                quadro.areaQuadri.QuadroLiquidazionePensione = areaQuadriLiquidazionePensione;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMenuLeft, Errore nel metodo GetQuadroLiquidazionePensione");
            }
            finally
            {
                Utility.CloseClient(quadriClient);
            }
        }

        public void GetQuadroDatiContributivi(IQuadriSemafori quadro)
        {
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(quadro.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = quadro.domanda.ProgStorico;
            AreaQuadri.DatiQuadroDatiContributivi areaQuadriDatiContributivi = new AreaQuadri.DatiQuadroDatiContributivi();
            SvrLiquidazione.QuadriClient quadriClient = new QuadriClient();
            try
            {
                areaQuadriDatiContributivi = quadriClient.GetQuadroDatiContributiviByDomanda(areaRichiestaDomanda);
                quadro.areaQuadri.QuadroDatiContributivi = areaQuadriDatiContributivi;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMenuLeft, Errore nel metodo GetQuadroDatiContributivi");
            }
            finally
            {
                Utility.CloseClient(quadriClient);
            }
        }

        public void GetQuadroRedditi(IQuadriSemafori quadro)
        {
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(quadro.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = quadro.domanda.ProgStorico;
            AreaQuadri.DatiQuadroRedditi areaQuadriRedditi = new AreaQuadri.DatiQuadroRedditi();
            SvrLiquidazione.QuadriClient quadriClient = new QuadriClient();
            try
            {
                areaQuadriRedditi = quadriClient.GetQuadroRedditiByDomanda(areaRichiestaDomanda);
                quadro.areaQuadri.QuadroRedditi = areaQuadriRedditi;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMenuLeft, Errore nel metodo GetQuadroRedditi");
            }
            finally
            {
                Utility.CloseClient(quadriClient);
            }
        }

        public void GetQuadroDanteCausa(IQuadriSemafori quadro)
        {
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(quadro.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = quadro.domanda.ProgStorico;
            AreaQuadri.DatiQuadroDanteCausa areaQuadriDanteCausa = new AreaQuadri.DatiQuadroDanteCausa();
            SvrLiquidazione.QuadriClient quadriClient = new QuadriClient();
            try
            {
                areaQuadriDanteCausa = quadriClient.GetQuadroDanteCausaByDomanda(areaRichiestaDomanda);
                quadro.areaQuadri.QuadroDanteCausa = areaQuadriDanteCausa;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMenuLeft, Errore nel metodo GetQuadroDanteCausa");
            }
            finally
            {
                Utility.CloseClient(quadriClient);
            }
        }
        public void GetQuadroFamiliari(IQuadriSemafori quadro)
        {
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(quadro.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = quadro.domanda.ProgStorico;
            AreaQuadri.DatiQuadroFamiliari areaQuadriFamiliari = new AreaQuadri.DatiQuadroFamiliari();
            SvrLiquidazione.QuadriClient quadriClient = new QuadriClient();
            try
            {
                areaQuadriFamiliari = quadriClient.GetQuadroFamiliariByDomanda(areaRichiestaDomanda);
                quadro.areaQuadri.QuadroFamiliari = areaQuadriFamiliari;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMenuLeft, Errore nel metodo GetQuadroFamiliari");
            }
            finally
            {
                Utility.CloseClient(quadriClient);
            }
        }

        public void GetQuadroSupplementi(IQuadriSemafori quadro)
        {
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(quadro.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = quadro.domanda.ProgStorico;
            AreaQuadri.DatiQuadroSupplementi areaQuadriSupplementi = new AreaQuadri.DatiQuadroSupplementi();
            SvrLiquidazione.QuadriClient quadriClient = new QuadriClient();
            try
            {
                areaQuadriSupplementi = quadriClient.GetQuadroSupplementiByDomanda(areaRichiestaDomanda);
                quadro.areaQuadri.QuadroSupplementi = areaQuadriSupplementi;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMenuLeft, Errore nel metodo GetQuadroSupplementi");
            }
            finally
            {
                Utility.CloseClient(quadriClient);
            }
        }

        public void GetQuadroBititolarita(IQuadriSemafori quadro)
        {
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(quadro.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = quadro.domanda.ProgStorico;
            AreaQuadri.DatiQuadroBititolarita areaQuadriBititolarita = new AreaQuadri.DatiQuadroBititolarita();
            SvrLiquidazione.QuadriClient quadriClient = new QuadriClient();
            try
            {
                areaQuadriBititolarita = quadriClient.GetQuadroBititolaritaByDomanda(areaRichiestaDomanda);
                quadro.areaQuadri.QuadroBititolarita = areaQuadriBititolarita;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMenuLeft, Errore nel metodo GetQuadroBititolarita");
            }
            finally
            {
                Utility.CloseClient(quadriClient);
            }
        }

        public void GetListaVersioni(IVersioni versioni)
        {
            AreaVersioni ctrl = new AreaVersioni();
            Presenter.SvrLiquidazione.AreaEsito esito = new Presenter.SvrLiquidazione.AreaEsito();
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                esito = objWS.GetListaVersioni(out ctrl, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMenuLeft, Errore nel metodo GetListaVersioni");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (ctrl != null)
                versioni.listaVersioni = ctrl.ListaVersioni;
        }

        public void GetListaVersioniFS(IVersioni versioni)
        {
            Presenter.SvrLiquidazioneFs.AreaVersioni versione = new Presenter.SvrLiquidazioneFs.AreaVersioni();
            Presenter.SvrLiquidazioneFs.AreaEsito esito = new Presenter.SvrLiquidazioneFs.AreaEsito();
            SvrLiquidazioneFs.ServizioLiquidazioneFsClient objWS = new SvrLiquidazioneFs.ServizioLiquidazioneFsClient();
            try
            {
                esito = objWS.GetListaVersioniFS(out versione);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMenuLeft, Errore nel metodo GetListaVersioniFS");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (versione != null)
                versioni.listaVersioni = versione.ListaVersioni;
        }

        public void GetListaVersioniAGO(IVersioni versioni)
        {
            Presenter.SvrLiquidazioneAgo.AreaVersioni versione = new Presenter.SvrLiquidazioneAgo.AreaVersioni();
            Presenter.SvrLiquidazioneAgo.AreaEsito esito = new Presenter.SvrLiquidazioneAgo.AreaEsito();
            SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient objWS = new SvrLiquidazioneAgo.ServizioLiquidazioneAgoClient();
            try
            {
                esito = objWS.GetListaVersioniAGO(out versione);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMenuLeft, Errore nel metodo GetListaVersioniAGO");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (versione != null)
                versioni.listaVersioni = versione.ListaVersioni;
        }

        public void GetListaVersioniCI(IVersioni versioni)
        {
            Presenter.SvrLiquidazioneCi.AreaVersioni versione = new Presenter.SvrLiquidazioneCi.AreaVersioni();
            Presenter.SvrLiquidazioneCi.AreaEsito esito = new Presenter.SvrLiquidazioneCi.AreaEsito();
            SvrLiquidazioneCi.ServizioLiquidazioneCiClient objWS = new SvrLiquidazioneCi.ServizioLiquidazioneCiClient();
            try
            {
                esito = objWS.GetListaVersioniCI(out versione);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMenuLeft, Errore nel metodo GetListaVersioniCI");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (versione != null)
                versioni.listaVersioni = versione.ListaVersioni;
        }

        public void GetAreaAvvisiMessaggi(IHomePage iHomePage)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaHomepage areaHomePage = null;
            try
            {
                AreaEsito esito = objWS.GetAreaHomepage(out areaHomePage, iHomePage.TipoApp);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    iHomePage.HasError = true;
                    iHomePage.ErrorMessage = esito.Messaggio;
                    return;
                }
                iHomePage.AreaHomePage = areaHomePage;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterMenuLeft, Errore nel metodo GetAreaAvvisiMessaggi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
