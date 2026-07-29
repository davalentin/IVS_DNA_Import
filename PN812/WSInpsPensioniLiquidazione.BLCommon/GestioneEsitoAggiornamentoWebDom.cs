using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.BLCommon
{

    public static class GestioneEsitoAggiornamentoWebDom
    {
        public static void GetEsitoAggiornamentoWebDom(Utility.TipoAppartenenza tipoApp, out List<EsitoAggiornamentiWebDom> lstEsitoAggiornamentoWebDom)
        {
            lstEsitoAggiornamentoWebDom = null;
            List<INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoWebDom> lstDb;
            DataCommon.DAEsitoAggiornamentoWebDom.GetEsitoAggiornamentoWebDom(tipoApp.ToString(), out lstDb);
            if (lstDb != null && lstDb.Count > 0)
                lstEsitoAggiornamentoWebDom = lstDb.Select(x => { var y = new EsitoAggiornamentiWebDom(); Utility.ValorizzaOggetti(x, y); return y; }).ToList();
        }


        public static void SalvaEsitoAggiornamentoWebDom(EsitoAggiornamentiWebDom esitoAggWebDom)
        {
            INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoWebDom objDb = new INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoWebDom();
            Utility.ValorizzaOggetti(esitoAggWebDom, objDb);
            DataCommon.DAEsitoAggiornamentoWebDom.SalvaEsitoAggiornamentoWebDom(objDb);
        }

        public static void EliminaEsitoAggiornamentoWebDomByTipoApp(Utility.TipoAppartenenza tipoApp)
        {
            DataCommon.DAEsitoAggiornamentoWebDom.EliminaEsitoAggiornamentoWebDom(tipoApp.ToString());
        }

        #region Nestled Class
        public class EsitoAggiornamentiWebDom
        {
            public long Ndomus { get; set; }
            public string TipoApp { get; set; }
            public System.Nullable<bool> Esito { get; set; }
            public string Errore { get; set; }
            public System.Nullable<System.DateTime> Timestamp { get; set; }

            public EsitoAggiornamentiWebDom() { }

            public EsitoAggiornamentiWebDom(EsitoAggiornamentiWebDom app)
            {
                Utility.ValorizzaOggetti(app, this);
            }
        }

        #endregion Nestled Class


    }

}
