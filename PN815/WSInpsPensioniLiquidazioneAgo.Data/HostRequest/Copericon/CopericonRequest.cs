using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostRequest
{
    public class CopericonRequest
    {
        public int TipoProcedura { get; set; }
        public decimal CodFondo { get; set; }
        public decimal CodFondoStorico { get; set; }
        public decimal ImportoTrattenuteErarialiAP { get; set; }
        public string CodCategoria { get; set; }
        public string CodSede { get; set; }
        public string Certificato { get; set; }
        public long CodEliminazione { get; set; }
        public DateTime DataEliminazione { get; set; }
        public short AnnoDecorrenza { get; set; }
        public short MeseDecorrenza { get; set; }
        public long MeseEstrazioneRata { get; set; }
        public string CodBeneficiLegge2062004 { get; set; }
        public string CodParticolareRinnovo { get; set; }
        public string CodMovimentazione { get; set; }
        public DateTime? DataMovimentazione { get; set; }
        public bool InvioMail { get; set; }
        public DateTime? DataPrelievo { get; set; }
        public int MatricolaOperatore { get; set; }
    }
}
