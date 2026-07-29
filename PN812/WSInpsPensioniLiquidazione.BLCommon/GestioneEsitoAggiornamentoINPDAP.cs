using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public static class GestioneEsitoAggiornamentoINPDAP
    {
        public static void GetEsitoAggiornamentoINPDAP(Utility.TipoAppartenenza tipoApp, out List<EsitoAggiornamentiINPDAP> lstEsitoAggiornamentoINPDAP)
        {
            lstEsitoAggiornamentoINPDAP = null;
            List<INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoINPDAP> lstDb;
            DataCommon.DAEsitoAggiornamentoINPDAP.GetEsitoAggiornamentoINPDAP(tipoApp.ToString(), out lstDb);
            if (lstDb != null && lstDb.Count > 0)
                lstEsitoAggiornamentoINPDAP = lstDb.Select(x => { var y = new EsitoAggiornamentiINPDAP(); Utility.ValorizzaOggetti(x, y); return y; }).ToList();
        }

        public static void SalvaEsitoAggiornamentoINPDAP(EsitoAggiornamentiINPDAP esitoAggINPDAP)
        {
            INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoINPDAP objDb = new INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoINPDAP();
            Utility.ValorizzaOggetti(esitoAggINPDAP, objDb);
            DataCommon.DAEsitoAggiornamentoINPDAP.SalvaEsitoAggiornamentoINPDAP(objDb);
        }

        public static void EliminaEsitoAggiornamentoINPDAPByTipoApp(Utility.TipoAppartenenza tipoApp)
        {
            DataCommon.DAEsitoAggiornamentoINPDAP.EliminaEsitoAggiornamentoINPDAP(tipoApp.ToString());
        }

        #region Nestled Class
        public class EsitoAggiornamentiINPDAP
        {
            public long Ndomus { get; set; }
            public string TipoApp { get; set; }
            public byte? ProgStorico { get; set; }
            public System.Nullable<bool> Esito { get; set; }
            public string Errore { get; set; }
            public System.Nullable<System.DateTime> Timestamp { get; set; }

            public EsitoAggiornamentiINPDAP() { }

            public EsitoAggiornamentiINPDAP(EsitoAggiornamentiINPDAP app)
            {
                Utility.ValorizzaOggetti(app, this);
            }
        }

        #endregion Nestled Class
    }
}
