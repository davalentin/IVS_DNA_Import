using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneEsitoAggiornamentoOneri
    {
        public static void GetEsitoAggiornamentoOneri(Utility.TipoAppartenenza tipoApp, out List<EsitoAggiornamentiOneri> lstEsitoAggiornamentoOneri)
        {
            lstEsitoAggiornamentoOneri = null;
            List<INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoOneri> lstDb;
            DataCommon.DAEsitoAggiornamentoOneri.GetEsitoAggiornamentoOneri(tipoApp.ToString(), out lstDb);
            if (lstDb != null && lstDb.Count > 0)
                lstEsitoAggiornamentoOneri = lstDb.Select(x => { var y = new EsitoAggiornamentiOneri(); Utility.ValorizzaOggetti(x, y); return y; }).ToList();
        }


        public static void SalvaEsitoAggiornamentoOneri(EsitoAggiornamentiOneri esitoAggOneri)
        {
            INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoOneri objDb = new INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoOneri();
            Utility.ValorizzaOggetti(esitoAggOneri, objDb);
            DataCommon.DAEsitoAggiornamentoOneri.SalvaEsitoAggiornamentoOneri(objDb);
        }

        public static void EliminaEsitoAggiornamentoOneriByTipoApp(Utility.TipoAppartenenza tipoApp)
        {
            DataCommon.DAEsitoAggiornamentoOneri.EliminaEsitoAggiornamentoOneri(tipoApp.ToString());
        }

        #region Nestled Class
        public class EsitoAggiornamentiOneri
        {
            public long Ndomus { get; set; }
            public byte? ProgStorico { get; set; }
            public string TipoApp { get; set; }
            public System.Nullable<bool> Esito { get; set; }
            public string Errore { get; set; }
            public System.Nullable<System.DateTime> Timestamp { get; set; }

            public EsitoAggiornamentiOneri() { }

            public EsitoAggiornamentiOneri(EsitoAggiornamentiOneri app)
            {
                Utility.ValorizzaOggetti(app, this);
            }
        }

        #endregion Nestled Class
    }
}
