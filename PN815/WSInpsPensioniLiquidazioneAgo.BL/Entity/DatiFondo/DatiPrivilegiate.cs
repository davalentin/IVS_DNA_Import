using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DatiPrivilegiate
    {
        public DatiPrivilegiate()
        { }

        #region public properties
        public byte? Semaforo { get; set; }
        public int? IndennitaAusiliaria { get; set; }
        public int? IndennitaParaplegici { get; set; }
        public int? IndennitaSpeciale { get; set; }
        #endregion public properties

        public bool IsDatiPrivilegiateNull()
        {
            if (!this.IndennitaAusiliaria.HasValue &&
                !this.IndennitaParaplegici.HasValue &&
                !this.IndennitaSpeciale.HasValue
               )
                return true;
            else
                return false;
        }
    }
}
