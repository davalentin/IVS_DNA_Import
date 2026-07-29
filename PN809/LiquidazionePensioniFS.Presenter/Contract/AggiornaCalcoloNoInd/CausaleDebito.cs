using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.Contract.AggiornaCalcoloNoInd
{
    [Serializable]
    public class CausaleDebito
    {
        public int Id { get; set; }
        public int CausaleSintetica { get; set; }
        public int CausaleAnalitica { get; set; }
        public string Descrizione { get; set; }
        public string ContoRecupero { get; set; }
        public decimal Importo { get; set; }
        public CausaleDtoLite[] CasualiAmmesse { get; set; }
    }
}
