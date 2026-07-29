using System;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class CtrlScadenzaIndennizzoINDCOM
    {
        public string Tipologia { get; set; }
        public string Sesso { get; set; }
        public DateTime? DataNascitaDal { get; set; }
        public DateTime? DataNascitaAl { get; set; }
        public byte? PrepopolaAnni { get; set; }
        public byte? PrepopolaMesi { get; set; }
        public byte? PrepopolaGiorni { get; set; }
        public byte? ControlloAnni { get; set; }
        public byte? ControlloMesi { get; set; }
        public byte? ControlloGiorni { get; set; }
    }
}
