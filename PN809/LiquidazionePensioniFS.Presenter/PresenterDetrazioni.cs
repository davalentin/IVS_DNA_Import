using System;
using INPS.DNA;

using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterDetrazioni
    {
        public void GetDetrazioni(IDetrazioni detrazioni)
        {
            AreaDetrazioni areaDetrazioni = new AreaDetrazioni();
            areaDetrazioni.DatiInput = new AreaDetrazioni.AreaInput();
            areaDetrazioni.DatiInput.NumeroDomanda = Int64.Parse(detrazioni.domanda.NumeroDomanda);
            areaDetrazioni.DatiInput.ProgStorico = detrazioni.domanda.ProgStorico;
            areaDetrazioni.DatiInput.CodiceFiscale = detrazioni.detrazioniPensione != null && detrazioni.detrazioniPensione.DatiInput != null ? detrazioni.detrazioniPensione.DatiInput.CodiceFiscale : null;
            AreaEsito risultatoGetDetrazioniByDomanda = new AreaEsito();

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                risultatoGetDetrazioniByDomanda = objWS.GetDetrazioniByDomanda(ref areaDetrazioni);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDetrazioni, Errore nel metodo getDetrazioni");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoGetDetrazioniByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                detrazioni.HasError = true;
                detrazioni.ErrorMessage = risultatoGetDetrazioniByDomanda.Messaggio;
                if (detrazioni.detrazioniPensione == null)
                    detrazioni.detrazioniPensione = new AreaDetrazioni();
                detrazioni.detrazioniPensione.EsitoDetrazioni = AreaDetrazioni.RitornoDetrazioni.Errore;
                detrazioni.detrazioniPensione.Messaggio = risultatoGetDetrazioniByDomanda.Messaggio;
            }
            else
            {
                if (areaDetrazioni.EsitoDetrazioni == AreaDetrazioni.RitornoDetrazioni.NessunErrore)
                {
                    detrazioni.HasError = false;
                    detrazioni.ErrorMessage = "";
                    detrazioni.detrazioniPensione = areaDetrazioni;
                }
                else if (areaDetrazioni.EsitoDetrazioni == AreaDetrazioni.RitornoDetrazioni.Informativa || areaDetrazioni.EsitoDetrazioni == AreaDetrazioni.RitornoDetrazioni.Errore)
                {
                    detrazioni.HasError = true;
                    detrazioni.ErrorMessage = areaDetrazioni.Messaggio;
                    detrazioni.detrazioniPensione = areaDetrazioni;
                }
            }
        }

        public void SalvaDetrazioni(IDetrazioni detrazioni)
        {
            AreaDetrazioni areaDetrazioni = new AreaDetrazioni();
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            areaDetrazioni.DatiInput = new AreaDetrazioni.AreaInput();
            areaDetrazioni.DatiInput.NumeroDomanda = Int64.Parse(detrazioni.domanda.NumeroDomanda);
            areaDetrazioni.DatiInput.CodiceFiscale = detrazioni.detrazioniPensione.DatiInput != null ? detrazioni.detrazioniPensione.DatiInput.CodiceFiscale : null;
            areaDetrazioni.Detrazioni = detrazioni.detrazioniPensione.Detrazioni;
            AreaEsito risultatoVerifyDetrazioniByDomanda = new AreaEsito();
            try
            {
                risultatoVerifyDetrazioniByDomanda = objWS.VerifyDetrazioniByDomanda(ref areaDetrazioni);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDetrazioni, Errore nel metodo SalvaDetrazioni");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            try
            {
                if (risultatoVerifyDetrazioniByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    //throw new INPS.DNA.DnaApplicationException("PresenterDetrazioni, Errore nel metodo SalvaDetrazioni" + risultatoVerifyDetrazioniByDomanda.Messaggio);
                    detrazioni.HasError = true;
                    detrazioni.ErrorMessage = "Errore tecnico nella verifica delle detrazioni d'imposta";
                    detrazioni.detrazioniPensione = areaDetrazioni;
                    if (detrazioni.detrazioniPensione == null)
                        detrazioni.detrazioniPensione = new AreaDetrazioni();
                    detrazioni.detrazioniPensione.EsitoDetrazioni = AreaDetrazioni.RitornoDetrazioni.Errore;
                    detrazioni.detrazioniPensione.Messaggio = detrazioni.ErrorMessage;
                }
                else
                {

                    if (areaDetrazioni.EsitoDetrazioni == AreaDetrazioni.RitornoDetrazioni.NessunErrore)
                    {
                        detrazioni.HasError = false;
                        detrazioni.ErrorMessage = "";
                        detrazioni.detrazioniPensione = areaDetrazioni;

                    }
                    else if (areaDetrazioni.EsitoDetrazioni == AreaDetrazioni.RitornoDetrazioni.Informativa || areaDetrazioni.EsitoDetrazioni == AreaDetrazioni.RitornoDetrazioni.Errore)
                    {
                        detrazioni.HasError = true;
                        detrazioni.ErrorMessage = areaDetrazioni.Messaggio;
                        detrazioni.detrazioniPensione = areaDetrazioni;
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DnaApplicationException("PresenterDetrazioni, Errore nel metodo SalvaDetrazioni" + ex);
            }
        }

        public void GetSoggettiDetrazioni(IDetrazioni detrazioni)
        {
            AreaDetrazioni areaDetrazioni = new AreaDetrazioni();
            areaDetrazioni.DatiInput = new AreaDetrazioni.AreaInput();
            areaDetrazioni.DatiInput.NumeroDomanda = Int64.Parse(detrazioni.domanda.NumeroDomanda);
            areaDetrazioni.DatiInput.ProgStorico = detrazioni.domanda.ProgStorico;
            AreaEsito risultatoGetDetrazioniByDomanda = new AreaEsito();

            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                risultatoGetDetrazioniByDomanda = objWS.GetSoggettiDetrazioniByDomanda(ref areaDetrazioni);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDetrazioni, Errore nel metodo GetSoggettiDetrazioni");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoGetDetrazioniByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                detrazioni.HasError = true;
                detrazioni.ErrorMessage = risultatoGetDetrazioniByDomanda.Messaggio;
                if (detrazioni.detrazioniPensione == null)
                    detrazioni.detrazioniPensione = new AreaDetrazioni();
                detrazioni.detrazioniPensione.EsitoDetrazioni = AreaDetrazioni.RitornoDetrazioni.Errore;
                detrazioni.detrazioniPensione.Messaggio = risultatoGetDetrazioniByDomanda.Messaggio;
            }
            else
            {
                if (areaDetrazioni.EsitoDetrazioni == AreaDetrazioni.RitornoDetrazioni.NessunErrore)
                {
                    detrazioni.HasError = false;
                    detrazioni.ErrorMessage = "";
                    detrazioni.detrazioniPensione = areaDetrazioni;
                }
                else if (areaDetrazioni.EsitoDetrazioni == AreaDetrazioni.RitornoDetrazioni.Informativa || areaDetrazioni.EsitoDetrazioni == AreaDetrazioni.RitornoDetrazioni.Errore)
                {
                    detrazioni.HasError = true;
                    detrazioni.ErrorMessage = areaDetrazioni.Messaggio;
                    detrazioni.detrazioniPensione = areaDetrazioni;
                }
            }
        }
    }
}
