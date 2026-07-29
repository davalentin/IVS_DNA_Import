using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class DatiRegistrazioneFondo
    {
        public List<DatiRecordFondo> lRecordFondo { get; set; }

        public class DatiRecordFondo 
        {
            public DateTime? DecorrenzaValiditaDati { get; set; }
            public long IdRecordFondo { get; set; }
            public byte? TabDatiCalcoloDZ { get; set; }
            public byte? TabArticolo2 { get; set; }
            public byte? TabDatiCalcolo { get; set; }
            public byte? TabDatiFondo { get; set; }
            public byte? TabDatiCalcolo707 { get; set; }
            public byte? TabLegge460 { get; set; }
            public byte? TabPrivilegiate { get; set; }
            public byte? TabQuoteMiglioramentiContrattuali { get; set; }
        }
    }
}
