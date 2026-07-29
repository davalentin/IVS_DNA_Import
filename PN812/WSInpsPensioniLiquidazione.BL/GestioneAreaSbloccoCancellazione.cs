using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaSbloccoCancellazione
    {
        public static bool SbloccoCancellazioneDomanda(long numeroDomanda, short codiceSede, byte centroOperativo, string siglaCategoria, Utility.TipoOperazione? tipoOperazione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            try
            {
                GestionePensione.DatiByPassCancellazione datiByPassCancellazione = new GestionePensione.DatiByPassCancellazione();
                datiByPassCancellazione.NDomus = numeroDomanda;
                datiByPassCancellazione.CodiceSede = codiceSede;
                datiByPassCancellazione.CentroOperativo = centroOperativo;
                datiByPassCancellazione.SiglaCategoria = siglaCategoria;

                GestionePensione.DatiPensione datiPensione = null;
                GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, null, out datiPensione);
                if (!ControlsSbloccoCancellazioneDomanda(datiPensione, codiceSede, centroOperativo, siglaCategoria, tipoOperazione, out messaggioVideo))
                    return false;

                switch (tipoOperazione)
                {
                    case Utility.TipoOperazione.INSERIMENTO:
                        GestionePensione.SalvaByPassCancellazione(datiByPassCancellazione);
                        break;
                    case Utility.TipoOperazione.CANCELLAZIONE:
                        GestionePensione.EliminaByPassCancellazione(datiByPassCancellazione);
                        break;
                }
            }
            catch (Exception Ex)
            {
                messaggioVideo = "Errore nel metodo SbloccoCancellazioneDomanda: " + Ex.Message;
                return false;
            }

            return true;
        }

        private static bool ControlsSbloccoCancellazioneDomanda(GestionePensione.DatiPensione datiPensione, short codiceSede, byte centroOperativo,
            string siglaCategoria, Utility.TipoOperazione? tipoOperazione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiPensione == null || Utility.GetCodiceSedeLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) != codiceSede || Utility.GetCentroOperativoLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) != centroOperativo || !datiPensione.SiglaCategoria.Trim().Equals(siglaCategoria))
            {
                switch (tipoOperazione)
                {
                    case Utility.TipoOperazione.INSERIMENTO:
                        messaggioVideo = "Nessuna domanda trovata per i criteri di ricerca inseriti";
                        break;
                    case Utility.TipoOperazione.CANCELLAZIONE:
                        messaggioVideo = "Nessuna domanda da cancellare";
                        break;
                }

                return false;
            }

            return true;
        }
    }
}
