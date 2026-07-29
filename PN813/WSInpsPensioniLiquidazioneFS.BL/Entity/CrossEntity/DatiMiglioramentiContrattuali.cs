using INPS.Pensioni.Liquidazione.BLCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity.CrossEntity
{
    public class DatiMiglioramentiContrattuali
    {
        #region private

        private List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> _LDatiQuoteMiglioramentiContrattuali;

        #endregion private

        #region public

        public byte? Semaforo { get; set; }
        public List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> LDatiQuoteMiglioramentiContrattuali { get { return _LDatiQuoteMiglioramentiContrattuali; } set { _LDatiQuoteMiglioramentiContrattuali = value; } }
        #endregion public

    }
}
