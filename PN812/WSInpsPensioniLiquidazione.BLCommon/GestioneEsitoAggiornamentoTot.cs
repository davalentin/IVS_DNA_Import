using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneEsitoAggiornamentoTot
    {
        public static void GetEsitoAggiornamentoTot(Utility.TipoAppartenenza tipoApp, out List<EsitoAggiornamentiTot> lstEsitoAggiornamentoTot)
        {
            lstEsitoAggiornamentoTot = null;
            List<INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoTot> lstDb;
            DataCommon.DAEsitoAggiornamentoTot.GetEsitoAggiornamentoTot(tipoApp.ToString(), out lstDb);
            if (lstDb != null && lstDb.Count > 0)
                lstEsitoAggiornamentoTot = lstDb.Select(x => { var y = new EsitoAggiornamentiTot(); Utility.ValorizzaOggetti(x, y); return y; }).ToList();
        }


        public static void SalvaEsitoAggiornamentoTot(EsitoAggiornamentiTot esitoAggTot)
        {
            INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoTot objDb = new INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoTot();
            Utility.ValorizzaOggetti(esitoAggTot, objDb);
            DataCommon.DAEsitoAggiornamentoTot.SalvaEsitoAggiornamentoTot(objDb);
        }

        public static void EliminaEsitoAggiornamentoTotByTipoApp(Utility.TipoAppartenenza tipoApp)
        {
            DataCommon.DAEsitoAggiornamentoTot.EliminaEsitoAggiornamentoTot(tipoApp.ToString());
        }

        #region Nestled Class
        public class EsitoAggiornamentiTot
        {
            public long Ndomus { get; set; }
            public byte? ProgStorico { get; set; }
            public string TipoApp { get; set; }
            public System.Nullable<bool> Esito { get; set; }
            public string Errore { get; set; }
            public System.Nullable<System.DateTime> Timestamp { get; set; }

            public EsitoAggiornamentiTot() { }

            public EsitoAggiornamentiTot(EsitoAggiornamentiTot app)
            {
                Utility.ValorizzaOggetti(app, this);
            }
        }

        #endregion Nestled Class
    }
}
