using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaLavorazioneManualeAutomatiche
    {

        public static void GetAllPensioniLavorazioneManualeAutomatiche(string tipoApp, out List<GestioneLavorazioneManualeAutomatiche.DatiLavorazioneManualeAutomatiche> elencoLavorazioneManualeAutomatiche)
        {
            elencoLavorazioneManualeAutomatiche = null;
            GestioneLavorazioneManualeAutomatiche.GetAllPensioniLavorazioneManualeAutomatiche(tipoApp, out elencoLavorazioneManualeAutomatiche);
        }

        public static void GetAllPensioniLavorazioneManualeAutomaticheByCodiceSede(string utente, string tipoApp, List<Int16> codSede, out List<GestioneLavorazioneManualeAutomatiche.DatiLavorazioneManualeAutomatiche> elencoLavorazioneManualeAutomatiche)
        {
            elencoLavorazioneManualeAutomatiche = null;
            GestioneLavorazioneManualeAutomatiche.GetAllPensioniLavorazioneManualeAutomaticheByCodiceSede(utente, tipoApp, codSede, out elencoLavorazioneManualeAutomatiche);
        }

        public static void GetAllPensioniLavorazioneManualeAutomaticheByNDomus(string gruppo, string prodotto, string tipo, long nDomus, out List<GestioneLavorazioneManualeAutomatiche.DatiLavorazioneManualeAutomatiche> elencoLavorazioneManualeAutomatiche)
        {
            elencoLavorazioneManualeAutomatiche = null;
            GestioneLavorazioneManualeAutomatiche.GetAllPensioniLavorazioneManualeAutomaticheByNDomus(gruppo, prodotto, tipo, nDomus, out elencoLavorazioneManualeAutomatiche);
        }

        public static void GetAllTipologieAutomaticheUnicarpe(out List<GestioneLavorazioneManualeAutomatiche.TipologiaAutomaticaUnicarpe> elencoTipologieAutomaticheUnicarpe)
        {
            elencoTipologieAutomaticheUnicarpe = null;
            GestioneLavorazioneManualeAutomatiche.GetAllTipologieAutomaticheUnicarpe(out elencoTipologieAutomaticheUnicarpe);
        }

        public static void StoreLavorazioneManualeAutomatiche(GestioneLavorazioneManualeAutomatiche.DatiLavorazioneManualeAutomatiche datiLavorazioneManualeAutomatiche, out string messaggio)
        {
            messaggio = string.Empty;
            GestioneLavorazioneManualeAutomatiche.SalvaLavorazioneManualeAutomatiche(datiLavorazioneManualeAutomatiche);
        }
    }
}
