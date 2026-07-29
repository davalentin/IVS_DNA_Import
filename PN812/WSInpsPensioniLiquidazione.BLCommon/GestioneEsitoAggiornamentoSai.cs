using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public static class GestioneEsitoAggiornamentoSAI
    {
        public static void GetEsitoAggiornamentoSAI(Utility.TipoAppartenenza tipoApp, out List<EsitoAggiornamentiSAI> lstEsitoAggiornamentoSAI)
        {
            lstEsitoAggiornamentoSAI = null;
            List<INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoSAI> lstDb;
            DataCommon.DAEsitoAggiornamentoSAI.GetEsitoAggiornamentoSAI(tipoApp.ToString(), out lstDb);
            if (lstDb != null && lstDb.Count > 0)
                lstEsitoAggiornamentoSAI = lstDb.Select(x => { var y = new EsitoAggiornamentiSAI(); Utility.ValorizzaOggetti(x, y); return y; }).ToList();
        }

        public static void SalvaEsitoAggiornamentoSAI(EsitoAggiornamentiSAI esitoAggSAI)
        {
            INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoSAI objDb = new INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoSAI();
            Utility.ValorizzaOggetti(esitoAggSAI, objDb);
            DataCommon.DAEsitoAggiornamentoSAI.SalvaEsitoAggiornamentoSAI(objDb);
        }

        public static void EliminaEsitoAggiornamentoSAIByTipoApp(Utility.TipoAppartenenza tipoApp)
        {
            DataCommon.DAEsitoAggiornamentoSAI.EliminaEsitoAggiornamentoSAI(tipoApp.ToString());
        }

        #region Nestled Class
        public class EsitoAggiornamentiSAI
        {
            public long Ndomus { get; set; }
            public string TipoApp { get; set; }
            public byte? ProgStorico { get; set; }
            public System.Nullable<bool> Esito { get; set; }
            public string Errore { get; set; }
            public System.Nullable<System.DateTime> Timestamp { get; set; }

            public EsitoAggiornamentiSAI() { }

            public EsitoAggiornamentiSAI(EsitoAggiornamentiSAI app)
            {
                Utility.ValorizzaOggetti(app, this);
            }
        }

        #endregion Nestled Class
    }
}
