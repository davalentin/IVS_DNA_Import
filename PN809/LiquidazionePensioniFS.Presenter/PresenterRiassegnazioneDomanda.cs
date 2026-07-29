using System;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using System.ServiceModel;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterRiassegnazioneDomanda
    {
        public void RiassegnaDomanda(IRiassegnazioneDomanda riassegnazioneDomanda)
        {
            AreaEsito esito = new AreaEsito();
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaRiassegnazioneDomanda areaRiassegnazioneDomanda = new AreaRiassegnazioneDomanda();

            areaRiassegnazioneDomanda.NumeroDomanda = riassegnazioneDomanda.NumeroDomanda;
            areaRiassegnazioneDomanda.TipoAppOperatore = riassegnazioneDomanda.TipoAppOperatore;
            areaRiassegnazioneDomanda.Ruolo = riassegnazioneDomanda.Ruolo;
            areaRiassegnazioneDomanda.TipoOperazione = riassegnazioneDomanda.TipoOperazione;
            areaRiassegnazioneDomanda.NuovaMatricola = riassegnazioneDomanda.NuovaMatricola;
            areaRiassegnazioneDomanda.VecchiaMatricola = riassegnazioneDomanda.VecchiaMatricola;
            areaRiassegnazioneDomanda.StatoPensione = riassegnazioneDomanda.StatoPensione;
            areaRiassegnazioneDomanda.Sede = int.Parse(Utility.GetSedeOperatore().ToString().PadLeft(4, '0') + Utility.GetCentroOperativoOperatore().ToString().PadLeft(2, '0'));

            try
            {
                esito = objWS.RiassegnazioneDomanda(ref areaRiassegnazioneDomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterRiassegnazioneDomanda, Errore nel metodo RiassegnaDomanda");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                riassegnazioneDomanda.HasError = true;
                riassegnazioneDomanda.ErrorMessage = esito.Messaggio;
            }
            else
            {
                riassegnazioneDomanda.HasError = false;
                riassegnazioneDomanda.ErrorMessage = string.Empty;
            }
            // Dopo l'aggiornamento della matricola i dati di ricerca vengono ripuliti, quindi è corretto che i dati di input sottostanti risultano null
            riassegnazioneDomanda.Ruolo = areaRiassegnazioneDomanda.Ruolo;
            riassegnazioneDomanda.NumeroDomanda = areaRiassegnazioneDomanda.NumeroDomanda;
            riassegnazioneDomanda.StatoPensione = areaRiassegnazioneDomanda.StatoPensione;
            riassegnazioneDomanda.VecchiaMatricola = areaRiassegnazioneDomanda.VecchiaMatricola;
            riassegnazioneDomanda.NuovaMatricola = areaRiassegnazioneDomanda.NuovaMatricola;
            riassegnazioneDomanda.TipoOperazione = areaRiassegnazioneDomanda.TipoOperazione;
            riassegnazioneDomanda.TipoAppOperatore = areaRiassegnazioneDomanda.TipoAppOperatore;
            riassegnazioneDomanda.SedeDiversa = areaRiassegnazioneDomanda.SedeDiversa;
        }
    }
}
