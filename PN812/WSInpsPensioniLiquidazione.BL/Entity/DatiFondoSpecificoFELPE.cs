using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class DatiFondoSpecificoFELPE
    {
        public GestioneFondo.DatiFondoTT DatiFondoTT { get; set; }
        public GestioneFondo.DatiFondoVL DatiFondoVL { get; set; }
        public GestioneFondo.DatiFondoET DatiFondoET { get; set; }
        public GestioneFondo.DatiFondoFST DatiFondoFST { get; set; }
        public GestioneFondo.DatiFondoPT DatiFondoPT { get; set; }
        public GestioneFondo.DatiFondoDZ DatiFondoDZ { get; set; }
    }
}
