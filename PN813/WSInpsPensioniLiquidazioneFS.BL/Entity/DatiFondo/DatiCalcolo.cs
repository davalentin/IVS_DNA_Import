using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class DatiCalcolo
    {
        public byte? Semaforo { get; set; }
        public List<GestioneDatiServizioUtile.ServizioUtile> lDatiServizioUtile { get; set; }
        public List<GestioneDatiServizioUtileINPDAP.ServizioUtile> lDatiServizioUtileINPDAP { get; set; }
        public decimal? PensioneAnnuaLorda { get; set; }
        public short? ServizioUtileDirittoAA { get; set; }
        public short? ServizioUtileDirittoMM { get; set; }
        public short? ServizioUtileDirittoGG { get; set; }
        public decimal? RMSSenzaLegge33670QA { get; set; }
        public decimal? Montante { get; set; }
        public GestioneContrib.TipoCalcolo TipoCalcolo { get; set; }
        public decimal? ImportoContributivoTotale { get; set; }
        public decimal? MontanteContributivo { get; set; }
        public int? NSettimane { get; set; }
        public decimal? MontanteQuotaDL214 { get; set; }
        public decimal? ImportoContribTotaleQuotaDL214 { get; set; }
        public int? NSettimaneQuotaDL214 { get; set; }
        public decimal? QuotaContributivaAnnua { get; set; }
        public byte? Divisore { get; set; }
        public string Capitolo { get; set; }
        public decimal? CoefficienteTrasformazione { get; set; }
        public List<Entity.DecCapitolo> lDecCapitolo { get; set; }
        public decimal? PensioneAnnuaLorda214 { get; set; }
        //ENG - PL Reversibilita 024
        public bool? IsPensioneAnnuaLordaDaPrelievo { get; set; }
        public short? ServizioUtileDirittoOIAA { get; set; }
        public short? ServizioUtileDirittoOIMM { get; set; }
        public short? ServizioUtileDirittoOIGG { get; set; }


        public bool IsQuotaDL214Presente()
        {
            if (this.NSettimaneQuotaDL214.HasValue || this.MontanteQuotaDL214.HasValue || this.ImportoContribTotaleQuotaDL214.HasValue || this.QuotaContributivaAnnua.HasValue)
                return true;

            return false;
        }
    }
}
