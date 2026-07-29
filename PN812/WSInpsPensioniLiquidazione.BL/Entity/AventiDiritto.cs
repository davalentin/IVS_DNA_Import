using System.Collections.Generic;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class AventiDiritto
    {
        public List<GestioneAventiDiritto.AventiDiritto> ListaAventiDiritto { get; set; }
        public List<GestioneAnagrafica.DatiAnagrafici> ListaAnagrafiche { get; set; }
    }
}
