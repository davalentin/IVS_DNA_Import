using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class DatiFondo
    {
        public byte? Semaforo { get; set; }

        public string TipoPensione {  get; set; }
        public DateTime? DecorrenzaCalcolo {  get; set; }
        public decimal? IncrementoContrattuale { get; set;}
        public DateTime? DecorrenzaValidita { get; set; }
        public bool? TrediciMensilita { get; set; }
        public bool? PagamentoIndennitaIntegrativaSpeciale { get; set; }
        public bool? DirittoIndennitaIntegrativaSpeciale { get; set; }
        public bool? IntegrazioneMinimo { get; set; }
        public bool? IndennitaIntegrativaSpecialeConglobata { get; set; }
        public bool? TitolareAltraPensione { get; set; }
        public int? NumeroRate { get; set; }
        public decimal? ImportoSingolaRata { get; set; }
        public string CodInd { get; set; }
        public DateTime? DataInizioInd { get; set; }
        public decimal? ImpInd { get; set; }
        public DateTime? DataCessInd { get; set; }
        public decimal? ImpRataIniz { get; set; }
        public decimal? ImpRataOrd { get; set; }
        public decimal? ImpRataFin { get; set; }
        public int? NumRate { get; set; }
        public decimal? IndennitaIntegrativaSpecialeLorda { get; set; }


        public bool IsNull()
        {
            return !DecorrenzaValidita.HasValue && !IncrementoContrattuale.HasValue && !TrediciMensilita.HasValue && !PagamentoIndennitaIntegrativaSpeciale.HasValue && 
                !DirittoIndennitaIntegrativaSpeciale.HasValue && !IntegrazioneMinimo.HasValue && !IndennitaIntegrativaSpecialeConglobata.HasValue &&
                !TitolareAltraPensione.HasValue;
        }
 
    }
}
