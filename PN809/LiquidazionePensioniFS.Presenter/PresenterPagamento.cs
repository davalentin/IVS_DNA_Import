using System;
using System.ServiceModel;
using INPS.DNA;

using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterPagamento{

        public void RicercaDatiPagamento(IPagamento pagamento)
        {
            AreaPagamento areaPagamento = new AreaPagamento();
            AreaEsito esito = new AreaEsito();
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(pagamento.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = pagamento.domanda.ProgStorico;
            try
            {
                esito = objWS.GetPagamentoByNumeroDomanda(out areaPagamento, areaRichiestaDomanda, pagamento.pagamentoPensione.Pagamento.ABI.Value);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterPagamento, Errore nel metodo RicercaDatiPagamento");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                pagamento.HasError = true;
                pagamento.ErrorMessage = esito.Messaggio;
                if (pagamento.pagamentoPensione == null)
                    pagamento.pagamentoPensione = new AreaPagamento();
                if (pagamento.pagamentoPensione.Pagamento == null)
                    pagamento.pagamentoPensione.Pagamento = new GestioneAreaPagamentoDatiPagamento();
            }
            else 
            {
                if (areaPagamento != null)
                {
                    pagamento.pagamentoPensione.ListCassaSede = areaPagamento.ListCassaSede;
                    pagamento.pagamentoPensione.ListStatiEsteri = areaPagamento.ListStatiEsteri;
                    if (areaPagamento.Pagamento != null)
                        pagamento.pagamentoPensione.Pagamento = areaPagamento.Pagamento;

                    pagamento.pagamentoPensione.IsBancaItaliaFromWebDom = areaPagamento.IsBancaItaliaFromWebDom;
                    pagamento.pagamentoPensione.IsPolarizzazionePerGestioneENPALSAttiva = areaPagamento.IsPolarizzazionePerGestioneENPALSAttiva;
                }
           }
        }

        public void RicercaUfficioPagatore(IPagamento pagamento) {
            UfficioPagatore[] listaUfficioPagatore = null;
            AreaEsito esito = new AreaEsito();
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                esito = objWS.GetUfficiPagatori(out listaUfficioPagatore,  pagamento.richiestaUfficiPagatori);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterPagamento, Errore nel metodo RicercaUfficioPagatore");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                pagamento.HasError = true;
                pagamento.ErrorMessage = "#_SERVICE_ERROR_#" + esito.Messaggio;
            }
            else
            {
                if (listaUfficioPagatore == null)
                {
                    pagamento.HasError = true;
                    pagamento.ErrorMessage = "Nessun risultato trovato per i parametri di ricerca inseriti";
                }
                else
                    pagamento.ufficioPagatore = listaUfficioPagatore;
            }
        }

        public void SalvaDatiPagamento(IPagamento pagamento) {
            AreaEsito esito = new AreaEsito();
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            string sede = Utility.GetSedeOperatore().ToString().PadLeft(4, '0');
            string matricola = Utility.GetMatricolaOperatore();
            Int64 ndomanda = Int64.Parse(pagamento.domanda.NumeroDomanda);
            AreaPagamento pagamentoPensione = pagamento.pagamentoPensione;
            try
            {
                esito = objWS.StorePagamento(ndomanda, ref pagamentoPensione, matricola, sede);
                pagamento.pagamentoPensione = pagamentoPensione;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterPagamento, Errore nel metodo SalvaDatiPagamento");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                pagamento.HasError = true;
                pagamento.ErrorMessage = esito.Messaggio; // "Errore tecnico nel salvataggio del Pagamento";
            }
        }

        public void EliminaDatiPagamento(IPagamento pagamento)
        {
            AreaEsito esito = new AreaEsito();
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            Int64 ndomanda = Int64.Parse(pagamento.domanda.NumeroDomanda);
            try
            {
                esito = objWS.CancelPagamentoByNumeroDomanda(ndomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterPagamento, Errore nel metodo EliminaDatiPagamento");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                pagamento.HasError = true;
                pagamento.ErrorMessage = "Errore tecnico nell'eliminazione del Pagamento";
            }
        }
    }
}
