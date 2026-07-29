using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneEsitoAggiornamentoFelpe
    {
        public static void GetEsitoAggiornamentoFelpe(Utility.TipoAppartenenza tipoApp, out List<EsitoAggiornamentiFelpe> lstEsitoAggiornamentoFelpe)
        {
            lstEsitoAggiornamentoFelpe = null;
            List<INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoFelpe> lstDb;
            DataCommon.DAEsitoAggiornamentoFelpe.GetEsitoAggiornamentoFelpe(tipoApp.ToString(), out lstDb);
            if (lstDb != null && lstDb.Count > 0)
                lstEsitoAggiornamentoFelpe = lstDb.Select(x => { var y = new EsitoAggiornamentiFelpe(); Utility.ValorizzaOggetti(x, y); return y; }).ToList();
        }

        public static void SalvaEsitoAggiornamentoFelpe(EsitoAggiornamentiFelpe esitoAggFelpe)
        {
            INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoFelpe objDb = new INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoFelpe();
            Utility.ValorizzaOggetti(esitoAggFelpe, objDb);
            DataCommon.DAEsitoAggiornamentoFelpe.SalvaEsitoAggiornamentoFelpe(objDb);
        }

        public static void EliminaEsitoAggiornamentoFelpeByTipoApp(Utility.TipoAppartenenza tipoApp)
        {
            DataCommon.DAEsitoAggiornamentoFelpe.EliminaEsitoAggiornamentoFelpe(tipoApp.ToString());
        }

        #region Nested Class
        public class EsitoAggiornamentiFelpe
        {
            public long Ndomus { get; set; }
            public byte? ProgStorico { get; set; }
            public string TipoApp { get; set; }
            public System.Nullable<bool> Esito { get; set; }
            public string Errore { get; set; }
            public System.Nullable<System.DateTime> Timestamp { get; set; }
        }

        #endregion Nestled Class
    }
}
