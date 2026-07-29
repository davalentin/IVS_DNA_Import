using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public static class GestioneEsitoAggiornamentoNoteDiDebito
    {
        public static void GetEsitoAggiornamentoNoteDiDebito(Utility.TipoAppartenenza tipoApp, out List<EsitoAggiornamentiNoteDiDebito> lstEsitoAggiornamentoNoteDiDebito)
        {
            lstEsitoAggiornamentoNoteDiDebito = null;
            List<INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoNoteDiDebito> lstDb;
            DataCommon.DAEsitoAggiornamentoNoteDiDebito.GetEsitoAggiornamentoNoteDiDebito(tipoApp.ToString(), out lstDb);
            if (lstDb != null && lstDb.Count > 0)
                lstEsitoAggiornamentoNoteDiDebito = lstDb.Select(x => { var y = new EsitoAggiornamentiNoteDiDebito(); Utility.ValorizzaOggetti(x, y); return y; }).ToList();
        }

        public static void SalvaEsitoAggiornamentoNoteDiDebito(EsitoAggiornamentiNoteDiDebito esitoAggNoteDiDebito)
        {
            INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoNoteDiDebito objDb = new INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoNoteDiDebito();
            Utility.ValorizzaOggetti(esitoAggNoteDiDebito, objDb);
            DataCommon.DAEsitoAggiornamentoNoteDiDebito.SalvaEsitoAggiornamentoNoteDiDebito(objDb);
        }

        public static void EliminaEsitoAggiornamentoNoteDiDebitoByTipoApp(Utility.TipoAppartenenza tipoApp)
        {
            DataCommon.DAEsitoAggiornamentoNoteDiDebito.EliminaEsitoAggiornamentoNoteDiDebito(tipoApp.ToString());
        }

        #region Nestled Class
        public class EsitoAggiornamentiNoteDiDebito
        {
            public long Ndomus { get; set; }
            public string TipoApp { get; set; }
            public byte? ProgStorico { get; set; }
            public System.Nullable<bool> Esito { get; set; }
            public string Errore { get; set; }
            public System.Nullable<System.DateTime> Timestamp { get; set; }

            public EsitoAggiornamentiNoteDiDebito() { }

            public EsitoAggiornamentiNoteDiDebito(EsitoAggiornamentiNoteDiDebito app)
            {
                Utility.ValorizzaOggetti(app, this);
            }
        }

        #endregion Nestled Class
    }
}
