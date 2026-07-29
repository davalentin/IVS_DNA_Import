using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DatiLiquidazionePensioneStorico
    {
        public int? AttivitaEconomica { get; set; }
        public byte? CodiceMobilita { get; set; }
        public long? CodiceParticolareSoggettoDerogato { get; set; }
        public DateTime? DecorrenzaOriginaria { get; set; }
        public DateTime? FineAssicurazione { get; set; }
        public DateTime? FineUltimoLavoro { get; set; }
        public DateTime? InizioAssicurazione { get; set; }
        public DateTime? InizioUltimoLavoro { get; set; }
        public byte? Legge44997 { get; set; }
        public string ModalitaLiquidazione { get; set; }
        public int? NContributiVolontari { get; set; }
        public int? NContributiVVAnzianita { get; set; }
        public int? NSettimaneOBG { get; set; }
        public int? ProfessioneIndividuale { get; set; }
        public bool RiduzioneRetributiva { get; set; }
        public decimal? RiduzioneRetributivaPercentuale { get; set; }
        public DateTime? ScadenzaRevisioneSanitaria { get; set; }
        public byte? TipoCalcolo { get; set; }
        public char? Contributivo { get; set; }
        public string NaturaPensione { get; set; }
        public DateTime DecorrenzaMaggiorazioneSociale { get; set; }
    }
}
