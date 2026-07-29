using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class DatiBeneficioVittimeTerrorismo
    {
        #region public properties
        public long? SoggettoBeneficiario { get; set; }
        public char? CodiceEvento { get; set; }
        public DateTime? DataEventoTerroristico { get; set; }
        public long? TipologiaPrestazione { get; set; }
        public long? TipologiaBeneficio { get; set; }
        #endregion public properties

        #region public methods

        public bool IsDatiBeneficioVittimeTerrorismoNull()
        {
            if (this.SoggettoBeneficiario.HasValue || this.CodiceEvento.HasValue || this.DataEventoTerroristico.HasValue || this.TipologiaPrestazione.HasValue || this.TipologiaBeneficio.HasValue)
                return false;
            else
                return true;
        }

        #endregion public methods
    }
}