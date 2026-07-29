using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class DatiLiquidazionePensioneStorico
    {
        #region public members
        public DateTime? DecorrenzaOriginaria { get; set; }
        public DateTime? InizioAssicurazione { get; set; }
        public DateTime? FineAssicurazione { get; set; }
        public decimal? RetribuzioneSettimanaleAgoQuotaA { get; set; }
        public decimal? RetribuzioneSettimanaleAgoQuotaB { get; set; }
        public byte? TipoCalcolo { get; set; }
        public char? CodiceComunicazioneCampo3 { get; set; }
        public long? CodiceParticolareSoggettoDerogato { get; set; }
        #endregion public members
    }
}
