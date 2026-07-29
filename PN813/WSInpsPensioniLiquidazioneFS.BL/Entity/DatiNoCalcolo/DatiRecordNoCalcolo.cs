using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class DatiRecordNoCalcolo
    {
        public string Decorrenza { get; set; }

        public long IdRecordNoCalcolo { get; set; }

        public byte? TabNoCalcolo { get; set; }
    }
}
