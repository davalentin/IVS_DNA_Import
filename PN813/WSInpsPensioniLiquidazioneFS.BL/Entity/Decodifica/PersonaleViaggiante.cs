using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class PersonaleViaggiante
    {
        #region public properties
        public long Id { get; set; }
        public string Descrizione { get; set; }
        public byte? TraduzioneSuGP { get; set; }
        #endregion public properties
    }
}
