using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class AnagraficaAccordoPerTipo0179
    {
        public long Id { get; set; }
        public bool? Abilitata { get; set; }
        public string AbilitataTxt { get; set; }
        public short? Codice { get; set; }
        public long? DenominazioneAzienda { get; set; }
        public DateTime? DataAccordi { get; set; }
        public int? DomandeLiquidabili { get; set; }
        public int? DomandeLiquidate { get; set; }
    }
}
