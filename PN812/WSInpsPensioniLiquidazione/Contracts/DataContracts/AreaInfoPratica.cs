using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaInfoPratica
    {
        #region private properties
        private string _StatoPensione;
        private string _MatricolaUtenteAcquisizione;
        private bool _IsMatchMatricola;
        private bool _IsCalcoloAbilitato;
        private AreaQuadri _AreaQuadri;
        private string _MatricolaOperatore;
        private short _SedeOperatore;
        private List<AreaQuadri.Tab> _ElencoTab;
        #endregion private properties

        #region public properties
        [DataMember]
        public string StatoPensione { get { return _StatoPensione; } set { _StatoPensione = value; } } // output
        [DataMember]
        public string MatricolaUtenteAcquisizione { get { return _MatricolaUtenteAcquisizione; } set { _MatricolaUtenteAcquisizione = value; } } // output
        [DataMember]
        public bool IsMatchMatricola { get { return _IsMatchMatricola; } set { _IsMatchMatricola = value; } } // output
        [DataMember]
        public bool IsCalcoloAbilitato { get { return _IsCalcoloAbilitato; } set { _IsCalcoloAbilitato = value; } } // output
        [DataMember]
        public AreaQuadri AreaQuadri { get { return _AreaQuadri; } set { _AreaQuadri = value; } } // input - output
        [DataMember]
        public string MatricolaOperatore { get { return _MatricolaOperatore; } set { _MatricolaOperatore = value; } } // input
        [DataMember]
        public short SedeOperatore { get { return _SedeOperatore; } set { _SedeOperatore = value; } } // input
        [DataMember]
        public List<AreaQuadri.Tab> ElencoTab { get { return _ElencoTab; } set { _ElencoTab = value; } } // input
        #endregion public properties
    }
}
