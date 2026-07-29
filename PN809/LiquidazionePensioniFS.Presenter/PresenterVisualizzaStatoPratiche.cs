using System;
using System.Linq;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.DNA;
using System.ServiceModel;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterVisualizzaStatoPratiche
    {
        public void RicercaPratiche(IView.IStatoPratiche statoPratiche) { 
            Presenter.SvrLiquidazione.AreaRichiestaStatoPratica  richiesta = new AreaRichiestaStatoPratica();
            Presenter.SvrLiquidazione.AreaRispostaStatoPratica risposta = new AreaRispostaStatoPratica();

            try
            {
                richiesta.TipoRecupero = AreaRichiestaStatoPratica.TipoRicerca.StatoPratica;

                richiesta.NumeroDomanda = statoPratiche.StatoPratica.NumeroDomanda;
                richiesta.StatoPensione = statoPratiche.StatoPratica.SPratica;
                if (statoPratiche.StatoPratica.SPratica == 0) {
                    richiesta.StatoPensione = null;
                }
                richiesta.Categoria = statoPratiche.StatoPratica.CategoriaPensione;
                richiesta.CodiceFiscale = statoPratiche.StatoPratica.CodiceFiscale;
                richiesta.Fondo = statoPratiche.StatoPratica.Fondo;
                richiesta.Cassa = statoPratiche.StatoPratica.Cassa;
                richiesta.Sede = statoPratiche.StatoPratica.Sede;
                if(statoPratiche.StatoPratica.DataElaborazioneMax != null)
                    richiesta.DataElaborazioneDomandaMax = Utility.ConvertString2Data_withMinValue(statoPratiche.StatoPratica.DataElaborazioneMax);
                if (statoPratiche.StatoPratica.DataElaborazioneMin != null)
                    richiesta.DataElaborazioneDomandaMin = Utility.ConvertString2Data_withMinValue(statoPratiche.StatoPratica.DataElaborazioneMin);
                if (statoPratiche.StatoPratica.DataPresentazioneMax != null)
                    richiesta.DataPresentazioneDomandaMax = Utility.ConvertString2Data_withMinValue(statoPratiche.StatoPratica.DataPresentazioneMax);
                if (statoPratiche.StatoPratica.DataPresentazioneMin != null)
                    richiesta.DataPresentazioneDomandaMin = Utility.ConvertString2Data_withMinValue(statoPratiche.StatoPratica.DataPresentazioneMin);
                richiesta.DatiParziali = new DatiPersonaliParziali();
                richiesta.DatiParziali.Cognome = statoPratiche.StatoPratica.Cognome;
                richiesta.DatiParziali.Nome = statoPratiche.StatoPratica.Nome;
                richiesta.Matricola = statoPratiche.StatoPratica.Matricola;
                richiesta.SedeOperatore = Utility.GetSedeOperatore();
                richiesta.CentroOperativoOperatore = Utility.GetCentroOperativoOperatore();
                richiesta.MatricolaOperatore = Utility.GetMatricolaOperatore();
                richiesta.TipoAppRuolo = statoPratiche.TipoAppRuolo;
                richiesta.Ruolo = statoPratiche.Ruolo;
                richiesta.TipoDomandaInLavorazione = statoPratiche.StatoPratica.TipoDomandaInLavorazione;
                richiesta.TipoDomandaLavorata = statoPratiche.StatoPratica.TipoDomandaLavorata;
                richiesta.Gruppo = statoPratiche.StatoPratica.Gruppo;
                richiesta.Prodotto = statoPratiche.StatoPratica.Prodotto;
                richiesta.Tipo = statoPratiche.StatoPratica.Tipo;
                richiesta.SedeDiAppartenenzaOperatore = Utility.GetSedeDiAppartenenzaOperatore();
                GetRiepilogo(richiesta, risposta, statoPratiche);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("PresenterVisualizzaStatoPratiche, Errore nel metodo RicercaPratiche" + ex);
            }
            return;
        }
        
        internal bool GetRiepilogo(AreaRichiestaStatoPratica richiesta, AreaRispostaStatoPratica risposta, IView.IStatoPratiche statoPratiche)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                risposta = objWS.GetStatoPraticaByKey(richiesta);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterVisualizzaStatoPratiche, Errore nel metodo GetRiepilogo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            try
            {
                if (risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    statoPratiche.HasError = true;
                    statoPratiche.ErrorMessage = "Errore tecnico nella ricerca: "+risposta.Esito.Messaggio;
                }
                else     //risposta del WS OK
                {
                    if(risposta.ElencoDatiStatoPratica != null)
                    {
                        statoPratiche.ElencoStatoPratiche = risposta.ElencoDatiStatoPratica.ToList();
                    }
                    else  //controllare se vuoto o con 0 elementi nell'array
                    {
                        statoPratiche.HasError = true;
                        statoPratiche.ErrorMessage = "Nessuna posizione trovata per il criterio inserito";
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("PresenterVisualizzaStatoPratiche, Errore nel metodo GetRiepilogo" + ex);
            }
            return false;
        }
        
        public void EliminaPratica(IStatoPratiche statoPratiche)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                AreaEsito Esito = objWS.EliminaPensioneByNumeroDomanda(Int64.Parse(statoPratiche.StatoPratica.NumeroDomanda), Utility.GetMatricolaOperatore(), Utility.GetSedeOperatore(), Utility.GetCentroOperativoOperatore(), 
                    statoPratiche.TipoAppRuolo, statoPratiche.Ruolo, Utility.GetSedeDiAppartenenzaOperatore());
                if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    statoPratiche.HasError = true;
                    statoPratiche.ErrorMessage = Esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterVisualizzaStatoPratiche, Errore nel metodo EliminaPratica");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
