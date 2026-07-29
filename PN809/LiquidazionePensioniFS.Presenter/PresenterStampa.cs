using System;
using System.IO;
using System.ServiceModel;
using INPS.DNA;

using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterStampa
    {
        public void GetStampaDomanda(IStampa infoStampa)
        {
            if (infoStampa.datiPensione == null)
            {
                infoStampa.areaEsito = new AreaEsito();
                infoStampa.areaEsito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                infoStampa.areaEsito.Messaggio = "Dati pensione mancanti. Non è possibile generare la stampa";
                return;
            }

            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = infoStampa.datiPensione.NDomus;
            areaRichiestaDomanda.ProgStorico = infoStampa.datiPensione.ProgStorico;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                MemoryStream msPDF = null;
                
                infoStampa.areaEsito = objWS.GetStampaDomanda(out msPDF, areaRichiestaDomanda);
                infoStampa.msPDF = msPDF;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterStampa, Errore nel metodo GetStampaDomanda");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void CancellaStampa(IStampa infoStampa)
        {
            if (infoStampa.datiPensione == null)
            {
                infoStampa.areaEsito = new AreaEsito();
                infoStampa.areaEsito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                infoStampa.areaEsito.Messaggio = "Dati pensione mancanti. Non è possibile cancellare la stampa";
                return;
            }

            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = infoStampa.datiPensione.NDomus;
            areaRichiestaDomanda.ProgStorico = infoStampa.datiPensione.ProgStorico;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                infoStampa.areaEsito = objWS.DeleteStampaWeb(areaRichiestaDomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterStampa, Errore nel metodo CancellaStampa");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void GetStampaDomandaByChiavePensione(IStampa infoStampa)
        {
            if (infoStampa.domanda == null)
            {
                infoStampa.areaEsito = new AreaEsito();
                infoStampa.areaEsito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                infoStampa.areaEsito.Messaggio = "Dati domanda mancanti. Non è possibile generare la stampa";
                return;
            }

            AreaRichiestaStampa areaRichiestaStampa = new AreaRichiestaStampa();
            areaRichiestaStampa.SiglaCategoria = infoStampa.domanda.SiglaCategoriaPensione;
            areaRichiestaStampa.CodiceSede = infoStampa.domanda.SedePensione;
            areaRichiestaStampa.Certificato = infoStampa.domanda.CertificatoPensione;

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                MemoryStream msPDF = null;

                infoStampa.areaEsito = objWS.GetStampaDomandaByChiavePensione(out msPDF, areaRichiestaStampa);
                infoStampa.msPDF = msPDF;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterStampa, Errore nel metodo GetStampaDomanda");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
