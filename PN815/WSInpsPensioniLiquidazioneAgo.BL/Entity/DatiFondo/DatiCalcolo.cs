using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DatiCalcolo
    {
        public byte? Semaforo { get; set; }
        public List<GestioneDatiServizioUtileINPDAP.ServizioUtile> lDatiServizioUtile { get; set; }
        public decimal? PensioneAnnuaLorda { get; set; }
        public short? ServizioUtileDiritto { get; set; }
        public decimal? RMSSenzaLegge33670QA { get; set; }
        public decimal? Montante { get; set; }
        public GestioneContrib.TipoCalcolo TipoCalcolo { get; set; }
    }
}
