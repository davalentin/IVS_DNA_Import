using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class PeriodoAventiDiritto
    {
        public GestioneAnagrafica.DatiAnagrafici DatiAnagraficiAventeDiritto { get; set; }
        public GestioneFamiliari.Familiare DatiFamiliareAventeDiritto { get; set; }
        public List<GestionePeriodiAventiDiritto.PeriodoAventiDiritto> ListaPeriodiAventeDiritto { get; set; }
        public long IdAventeDiritto { get; set; }
    }
}
