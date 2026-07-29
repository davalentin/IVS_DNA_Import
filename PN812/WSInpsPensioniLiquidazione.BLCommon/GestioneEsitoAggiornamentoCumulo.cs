using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneEsitoAggiornamentoCumulo
    {
        public static void GetEsitoAggiornamentoCumulo(Utility.TipoAppartenenza tipoApp, out List<EsitoAggiornamentiCumulo> lstEsitoAggiornamentoCumulo)
        {
            lstEsitoAggiornamentoCumulo = null;
            List<INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoCumulo> lstDb;
            DataCommon.DAEsitoAggiornamentoCumulo.GetEsitoAggiornamentoCumulo(tipoApp.ToString(), out lstDb);
            if (lstDb != null && lstDb.Count > 0)
                lstEsitoAggiornamentoCumulo = lstDb.Select(x => { var y = new EsitoAggiornamentiCumulo(); Utility.ValorizzaOggetti(x, y); return y; }).ToList();
        }


        public static void SalvaEsitoAggiornamentoCumulo(EsitoAggiornamentiCumulo esitoAggCumulo)
        {
            INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoCumulo objDb = new INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoCumulo();
            Utility.ValorizzaOggetti(esitoAggCumulo, objDb);
            DataCommon.DAEsitoAggiornamentoCumulo.SalvaEsitoAggiornamentoCumulo(objDb);
        }

        public static void EliminaEsitoAggiornamentoCumuloByTipoApp(Utility.TipoAppartenenza tipoApp)
        {
            DataCommon.DAEsitoAggiornamentoCumulo.EliminaEsitoAggiornamentoCumulo(tipoApp.ToString());
        }

        #region Nestled Class
        public class EsitoAggiornamentiCumulo
        {
            public long Ndomus { get; set; }
            public byte? ProgStorico { get; set; }
            public string TipoApp { get; set; }
            public System.Nullable<bool> Esito { get; set; }
            public string Errore { get; set; }
            public System.Nullable<System.DateTime> Timestamp { get; set; }

            public EsitoAggiornamentiCumulo() { }

            public EsitoAggiornamentiCumulo(EsitoAggiornamentiCumulo app)
            {
                Utility.ValorizzaOggetti(app, this);
            }
        }

        #endregion Nestled Class
    }
}
