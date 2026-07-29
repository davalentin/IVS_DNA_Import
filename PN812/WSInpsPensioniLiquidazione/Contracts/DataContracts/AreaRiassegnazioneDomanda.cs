using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaRiassegnazioneDomanda
    {
        #region Private Properties

        #region Input/Output Parameters
        private long _NumeroDomanda;
        private string _StatoPensione;
        private string _VecchiaMatricola;
        #endregion Input/Output Parameters

        #region Input Parameters
        private string _NuovaMatricola;
        private Utility.TipoOperazione? _TipoOperazione;
        private Utility.TipoAppartenenza? _TipoAppOperatore;
        private Utility.Ruolo? _Ruolo;
        private int? _Sede;
        #endregion Input Parameters

        #endregion Private Properties

        #region Public Properties

        #region Input/Output Parameters
        [DataMember]
        public long NumeroDomanda { get { return _NumeroDomanda; } set { _NumeroDomanda = value; } }
        [DataMember]
        public string StatoPensione { get { return _StatoPensione; } set { _StatoPensione = value; } }
        [DataMember]
        public string VecchiaMatricola { get { return _VecchiaMatricola; } set { _VecchiaMatricola = value; } }
        [DataMember]
        public string SedeDiversa { get ; set; }
        #endregion Input/Output Parameters

        #region Input Parameters
        [DataMember]
        public string NuovaMatricola { get { return _NuovaMatricola; } set { _NuovaMatricola = value; } }
        [DataMember]
        public Utility.TipoOperazione? TipoOperazione { get { return _TipoOperazione; } set { _TipoOperazione = value; } }
        [DataMember]
        public Utility.TipoAppartenenza? TipoAppOperatore { get { return _TipoAppOperatore; } set { _TipoAppOperatore = value; } }
        [DataMember]
        public Utility.Ruolo? Ruolo { get { return _Ruolo; } set { _Ruolo = value; } }
        [DataMember]
        public int? Sede { get { return _Sede; } set { _Sede = value; } }
        #endregion Input Parameters

        #endregion Public Properties
    }
}
