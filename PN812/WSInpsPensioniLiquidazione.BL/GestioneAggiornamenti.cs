using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.Entity;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAggiornamenti
    {
        #region public methods
       
        public static void GetAggiornamenti(bool getAggiornamentiAttivi, string tipoApp, out List<Aggiornamenti> elencoAggiornamenti)
        {
            elencoAggiornamenti = null;
            List<BLCommon.GestioneAggiornamenti.DatiAggiornamenti> elencoAggiornamentiBL = null;
            if (getAggiornamentiAttivi)
                BLCommon.GestioneAggiornamenti.GetAggiornamentiAttivi(tipoApp, out elencoAggiornamentiBL);
            else
                BLCommon.GestioneAggiornamenti.GetAllAggiornamenti(tipoApp, out elencoAggiornamentiBL);

            if (elencoAggiornamentiBL != null && elencoAggiornamentiBL.Count > 0)
            {
                elencoAggiornamenti = elencoAggiornamentiBL.Select(x => { var y = new Aggiornamenti(); Utility.ValorizzaOggetti(x, y); return y; }).ToList();
            }
        }

   
        public static void StoreAggiornamenti(Aggiornamenti agg)
        {
            agg.TimeStamp = DateTime.Now;
            BLCommon.GestioneAggiornamenti.DatiAggiornamenti avvisoBL = new BLCommon.GestioneAggiornamenti.DatiAggiornamenti();
            Utility.ValorizzaOggetti(agg, avvisoBL);
            BLCommon.GestioneAggiornamenti.SalvaAggiornamento(avvisoBL);
        }

       
        public static void DeleteAggiornamento(Aggiornamenti agg)
        {
            BLCommon.GestioneAggiornamenti.DeleteAggiornamento(agg.Id);
        }
        #endregion public methods
    }
}
