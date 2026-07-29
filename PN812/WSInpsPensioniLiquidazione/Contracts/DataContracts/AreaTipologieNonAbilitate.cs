using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaTipologieNonAbilitate
    {
        public AreaTipologieNonAbilitate()
        {
            this._ElencoTipologieNonAbilitate = new List<TipologieNonAbilitate>();
        }

        #region Private Properties

        private List<TipologieNonAbilitate> _ElencoTipologieNonAbilitate;
        private List<GestioneTipologieNonAbilitate.Gruppo> _ElencoGruppo;
        private List<GestioneTipologieNonAbilitate.Prodotto> _ElencoProdotto;
        private List<GestioneTipologieNonAbilitate.Tipo> _ElencoTipo;
        private List<GestioneTipologieNonAbilitate.Filtro> _ElencoFiltro;
        private Utility.TipoAppartenenza _TipoAppOperatore;
        
        #endregion Private Properties

        #region Public Properties

        [DataMember]
        public List<TipologieNonAbilitate> ElencoTipologieNonAbilitate { get { return _ElencoTipologieNonAbilitate; } set { _ElencoTipologieNonAbilitate = value; } }
        [DataMember]
        public List<GestioneTipologieNonAbilitate.Gruppo> ElencoGruppo { get { return _ElencoGruppo; } set { _ElencoGruppo = value; } }
        [DataMember]
        public List<GestioneTipologieNonAbilitate.Prodotto> ElencoProdotto { get { return _ElencoProdotto; } set { _ElencoProdotto = value; } }
        [DataMember]
        public List<GestioneTipologieNonAbilitate.Tipo> ElencoTipo { get { return _ElencoTipo; } set { _ElencoTipo = value; } }
        [DataMember]
        public List<GestioneTipologieNonAbilitate.Filtro> ElencoFiltro { get { return _ElencoFiltro; } set { _ElencoFiltro = value; } }
        [DataMember]
        public Utility.TipoAppartenenza TipoAppOperatore { get { return _TipoAppOperatore; } set { _TipoAppOperatore = value; } }

        #endregion Public Properties


        #region Nested Class

        [DataContract]
        public class TipologieNonAbilitate
        {
            public TipologieNonAbilitate()
            { }

            public TipologieNonAbilitate(GestioneTipologieNonAbilitate.DatiTipologieNonAbilitate datiTipologieNonAbilitate)
            {
                _TipoApp = datiTipologieNonAbilitate.TipoApp;
                _Fondo = datiTipologieNonAbilitate.Fondo;
                _Gruppo = datiTipologieNonAbilitate.Gruppo;
                _Prodotto = datiTipologieNonAbilitate.Prodotto;
                _Tipo = datiTipologieNonAbilitate.Tipo;
                _Filtro = datiTipologieNonAbilitate.Filtro;
                _SiglaCategoria = datiTipologieNonAbilitate.SiglaCategoria;
            }

            #region Private Properties

            private string _TipoApp;
            private string _Fondo;
            private string _Gruppo;
            private string _Prodotto;
            private string _Tipo;
            private string _Filtro;
            private string _SiglaCategoria;

            #endregion Private Properties

            #region Public Properties

            [DataMember]
            public string TipoApp { get { return _TipoApp; } set { _TipoApp = value; } }
            [DataMember]
            public string Fondo { get { return _Fondo; } set { _Fondo = value; } }
            [DataMember]
            public string Gruppo { get { return _Gruppo; } set { _Gruppo = value; } }
            [DataMember]
            public string Prodotto { get { return _Prodotto; } set { _Prodotto = value; } }
            [DataMember]
            public string Tipo { get { return _Tipo; } set { _Tipo = value; } }
            [DataMember]
            public string Filtro { get { return _Filtro; } set { _Filtro = value; } }
            [DataMember]
            public string SiglaCategoria { get { return _SiglaCategoria; } set { _SiglaCategoria = value; } }

            #endregion Public Properties
        }

        [DataContract]
        public class Gruppo
        {
            #region Private Properties
            private string _Codice;
            private string _Descrizone;
            #endregion Private Properties

            #region Public Properties
            public string Codice { get { return _Codice; } set { _Codice = value; } }
            public string Descrizone { get { return _Descrizone; } set { _Descrizone = value; } }
            #endregion Public Properties
        }

        [DataContract]
        public class Prodotto
        {
            #region Private Properties
            private string _Codice;
            private string _Descrizone;
            #endregion Private Properties

            #region Public Properties
            public string Codice { get { return _Codice; } set { _Codice = value; } }
            public string Descrizone { get { return _Descrizone; } set { _Descrizone = value; } }
            #endregion Public Properties
        }

        [DataContract]
        public class Tipo
        {
            #region Private Properties
            private string _Codice;
            private string _Descrizone;
            #endregion Private Properties

            #region Public Properties
            public string Codice { get { return _Codice; } set { _Codice = value; } }
            public string Descrizone { get { return _Descrizone; } set { _Descrizone = value; } }
            #endregion Public Properties
        }

        [DataContract]
        public class Filtro
        {
            #region Private Properties
            private string _Codice;
            private string _Descrizone;
            #endregion Private Properties

            #region Public Properties
            public string Codice { get { return _Codice; } set { _Codice = value; } }
            public string Descrizone { get { return _Descrizone; } set { _Descrizone = value; } }
            #endregion Public Properties
        }

        #endregion Nested Class
    }
}
