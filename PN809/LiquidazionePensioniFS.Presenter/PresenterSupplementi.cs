using System;
using System.ServiceModel;

using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterSupplementi
    {
        public void RicercaSupplementiByNumDomanda(ISupplementi isup)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
                areaRichiestaDomanda.NumeroDomanda = Int64.Parse(isup.domanda.NumeroDomanda);
                areaRichiestaDomanda.ProgStorico = isup.domanda.ProgStorico;

                AreaSupplementi areaRisponse;
                AreaEsito esito = new AreaEsito();
                
                esito = objWS.GetSupplementiByDomanda(out areaRisponse, areaRichiestaDomanda);
                isup.risposta = areaRisponse;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterSupplementi, Errore nel metodo RicercaSupplementiByNumDomanda");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaSupplementiByDomanda(ISupplementi isup)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                Presenter.SvrLiquidazione.AreaEsito esito = new AreaEsito();
                esito = objWS.SalvaSupplementiByDomanda(Int64.Parse(isup.domanda.NumeroDomanda), isup.lstSupplementi);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    isup.HasError = true;
                    isup.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterSupplementi, Errore nel metodo SalvaSupplementiByDomanda");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaTabSupplementiByDomanda(ISupplementi isup)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                Presenter.SvrLiquidazione.AreaEsito esito = new AreaEsito();
                esito = objWS.StoreDatiSupplementi(Int64.Parse(isup.domanda.NumeroDomanda), isup.lstSupplementi);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    isup.HasError = true;
                    isup.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterSupplementi, Errore nel metodo SalvaSupplementiByDomanda");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaTabSupplementiByDomanda(ISupplementi isup)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                Presenter.SvrLiquidazione.AreaEsito esito = new AreaEsito();
                AreaSupplementi areaSupplementi = null;
                esito = objWS.DeleteDatiSupplementiByDomanda(out areaSupplementi, Int64.Parse(isup.domanda.NumeroDomanda));

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    isup.HasError = true;
                    isup.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    isup.risposta = areaSupplementi;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterSupplementi, Errore nel metodo SalvaSupplementiByDomanda");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaDettaglioSupplementi(ISupplementi isup)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaSupplementi areaSupplementi = isup.lstSupplementi;
            try
            {
                AreaEsito esito = objWS.StoreSupplementoDettaglioEnpals(long.Parse(isup.domanda.NumeroDomanda), ref areaSupplementi);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    isup.HasError = true;
                    isup.ErrorMessage = esito.Messaggio;
                    return;
                }
                isup.lstSupplementi = areaSupplementi;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterSupplementi, Errore nel metodo SalvaDettaglioSupplementi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaDettaglioSupplementi(ISupplementi isup)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaSupplementi areaSupplementi = isup.lstSupplementi;
            try
            {
                AreaEsito esito = objWS.DeleteSupplementoDettaglioEnpals(out areaSupplementi, long.Parse(isup.domanda.NumeroDomanda), isup.lstSupplementi.DatiSuppRecordENPALS.IdSuppRecordEnpals);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    isup.HasError = true;
                    isup.ErrorMessage = esito.Messaggio;
                    return;
                }
                isup.lstSupplementi = areaSupplementi;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterSupplementi, Errore nel metodo EliminaDettaglioSupplementi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
        
        public void SalvaRecordSupplemento(ISupplementi isup)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaSupplementi areaSupplementi = isup.lstSupplementi;
            try
            {
                AreaEsito esito = objWS.StoreRecordSupplementoEnpals(long.Parse(isup.domanda.NumeroDomanda), ref areaSupplementi);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    isup.HasError = true;
                    isup.ErrorMessage = esito.Messaggio;
                    return;
                }
                isup.lstSupplementi = areaSupplementi;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterSupplementi, Errore nel metodo SalvaRecordSupplemento");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaRecordSupplemento(ISupplementi isup)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                AreaEsito esito = objWS.DeleteRecordSupplementoEnpals(long.Parse(isup.domanda.NumeroDomanda), isup.lstSupplementi.DatiSuppRecordENPALS.IdSuppRecordEnpals);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    isup.HasError = true;
                    isup.ErrorMessage = esito.Messaggio;
                    return;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterSupplementi, Errore nel metodo EliminaRecordSupplemento");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void GetDettagliSupplementiEnpals(ISupplementi isup)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaSupplementi areaSupp;
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(isup.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = isup.domanda.ProgStorico;

            try
            {
                AreaEsito esito = objWS.GetDatiSupplementoDettaglioEnpals(out areaSupp, areaRichiestaDomanda, isup.lstSupplementi.DatiSuppRecordENPALS.IdSuppRecordEnpals);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    isup.HasError = true;
                    isup.ErrorMessage = esito.Messaggio;
                    return;
                }
                isup.lstSupplementi = areaSupp;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterSupplementi, Errore nel metodo GetDettagliSupplementiEnpals");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        #region Contribuzione Enpals

        public void SalvaContribuzioneEnpals(ISupplementi isup)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                Presenter.SvrLiquidazione.AreaEsito esito = new AreaEsito();
                esito = objWS.StoreDatiSupplementi(Int64.Parse(isup.domanda.NumeroDomanda), isup.lstSupplementi);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    isup.HasError = true;
                    isup.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterSupplementi, Errore nel metodo SalvaContribuzioneEnpals");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
        #endregion Contribuzione Enpals

        #region Cumulo
        public void SalvaSupplementiCumulo(ISupplementi isup)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                Presenter.SvrLiquidazione.AreaEsito esito = new AreaEsito();
                esito = objWS.StoreDatiSupplementiCumulo(Int64.Parse(isup.domanda.NumeroDomanda), isup.lstSupplementi);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    isup.HasError = true;
                    isup.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterSupplementi, Errore nel metodo SalvaSupplementiCumulo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaSupplementiCumulo(ISupplementi isup)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            try
            {
                Presenter.SvrLiquidazione.AreaEsito esito = new AreaEsito();
                AreaSupplementi areaSupplementi = null;
                esito = objWS.DeleteDatiSupplementiCumuloByDomanda(out areaSupplementi, Int64.Parse(isup.domanda.NumeroDomanda));

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    isup.HasError = true;
                    isup.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    isup.risposta = areaSupplementi;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterSupplementi, Errore nel metodo EliminaSupplementiCumulo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
        #endregion
    }
}
