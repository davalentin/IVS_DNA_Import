using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DatiRegistrazioneFondo
    {
        public List<DatiRecordFondo> lRecordFondo { get; set; }

        public class DatiRecordFondo
        {
            public DatiRecordFondo()
            { }
            public DateTime? DecorrenzaValiditaDati { get; set; }
            public long IdRecordFondo { get; set; }
            public byte? TabArticolo2 { get; set; }
            public byte? TabDatiCalcolo { get; set; }
            public byte? TabDatiFondo { get; set; }
            public byte? TabPrivilegiate { get; set; }
        }
    }
}
