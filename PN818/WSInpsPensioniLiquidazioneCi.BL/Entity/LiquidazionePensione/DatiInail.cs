using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneCi.Entity
{
    public class DatiInail
    {
        #region public properties

        public long? IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
        public DateTime? DecorrenzaRenditaInail { get { return _DecorrenzaRenditaInail; } set { _DecorrenzaRenditaInail = value; } }
        public decimal? ImportoMensileInail { get { return _ImportoMensileInail; } set { _ImportoMensileInail = value; } }
        public bool? Evento { get { return _Evento; } set { _Evento = value; } }

        #endregion public properties

        #region private properties

        private long? _IdPensione;
        private DateTime? _DecorrenzaRenditaInail;
        private decimal? _ImportoMensileInail;
        private bool? _Evento;

        #endregion private properties

    }
}
