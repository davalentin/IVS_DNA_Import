using INPS.Pensioni.Liquidazione.DataCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public static class GestioneEsitoAggiornamentoPianiDiPagamento
    {
        public static void GetEsitoAggiornamentoPianiDiPagamento(Utility.TipoAppartenenza tipoApp, out List<EsitoAggiornamentiPianiDiPagamento> lstEsitoAggiornamentoPianiDiPagamento)
        {
            lstEsitoAggiornamentoPianiDiPagamento = null;
            List<INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoPianiDiPagamento> lstDb;
            DataCommon.DAEsitoAggiornamentoPianiDiPagamento.GetEsitoAggiornamentoPianiDiPagamento(tipoApp.ToString(), out lstDb);
            if (lstDb != null && lstDb.Count > 0)
                lstEsitoAggiornamentoPianiDiPagamento = lstDb.Select(x => { var y = new EsitoAggiornamentiPianiDiPagamento(); Utility.ValorizzaOggetti(x, y); return y; }).ToList();
        }

        public static void SalvaEsitoAggiornamentoPianiDiPagamento(EsitoAggiornamentiPianiDiPagamento esitoAggPianiDiPagamento)
        {
            INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoPianiDiPagamento objDb = new INPS.Pensioni.Liquidazione.DataCommon.EsitoAggiornamentoPianiDiPagamento();
            Utility.ValorizzaOggetti(esitoAggPianiDiPagamento, objDb);
            DataCommon.DAEsitoAggiornamentoPianiDiPagamento.SalvaEsitoAggiornamentoPianiDiPagamento(objDb);
        }

        public static void EliminaEsitoAggiornamentoPianiDiPagamentoByTipoApp(Utility.TipoAppartenenza tipoApp)
        {
            DataCommon.DAEsitoAggiornamentoPianiDiPagamento.EliminaEsitoAggiornamentoPianiDiPagamento(tipoApp.ToString());
        }

        #region Nestled Class
        public class EsitoAggiornamentiPianiDiPagamento
        {
            public long Ndomus { get; set; }
            public string TipoApp { get; set; }
            public byte? ProgStorico { get; set; }
            public System.Nullable<bool> Esito { get; set; }
            public string Errore { get; set; }
            public System.Nullable<System.DateTime> Timestamp { get; set; }

            public EsitoAggiornamentiPianiDiPagamento() { }

            public EsitoAggiornamentiPianiDiPagamento(EsitoAggiornamentiPianiDiPagamento app)
            {
                Utility.ValorizzaOggetti(app, this);
            }
        }

        #endregion Nestled Class
    }
}
