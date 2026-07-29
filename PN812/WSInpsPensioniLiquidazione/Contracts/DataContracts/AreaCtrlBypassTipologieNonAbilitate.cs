using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaCtrlBypassTipologieNonAbilitate
    {
        public AreaCtrlBypassTipologieNonAbilitate()
        {
            this._ElencoCtrlBypassTipologieNonAbilitate = new List<CtrlBypassTipologieNonAbilitate>();
        }

        #region Private Properties

        private List<CtrlBypassTipologieNonAbilitate> _ElencoCtrlBypassTipologieNonAbilitate;
        private List<GestioneDecodifica.Gruppo> _ElencoGruppo;
        private List<GestioneDecodifica.Prodotto> _ElencoProdotto;
        private List<GestioneDecodifica.Tipo> _ElencoTipo;
        private List<GestioneDecodifica.Filtro> _ElencoFiltro;
        private Utility.TipoAppartenenza _TipoAppOperatore;


        #endregion Private Properties

        #region Public Properties

        [DataMember]
        public List<CtrlBypassTipologieNonAbilitate> ElencoCtrlBypassTipologieNonAbilitate { get { return _ElencoCtrlBypassTipologieNonAbilitate; } set { _ElencoCtrlBypassTipologieNonAbilitate = value; } }
        [DataMember]
        public List<GestioneDecodifica.Gruppo> ElencoGruppo { get { return _ElencoGruppo; } set { _ElencoGruppo = value; } }
        [DataMember]
        public List<GestioneDecodifica.Prodotto> ElencoProdotto { get { return _ElencoProdotto; } set { _ElencoProdotto = value; } }
        [DataMember]
        public List<GestioneDecodifica.Tipo> ElencoTipo { get { return _ElencoTipo; } set { _ElencoTipo = value; } }
        [DataMember]
        public List<GestioneDecodifica.Filtro> ElencoFiltro { get { return _ElencoFiltro; } set { _ElencoFiltro = value; } }
        [DataMember]
        public Utility.TipoAppartenenza TipoAppOperatore { get { return _TipoAppOperatore; } set { _TipoAppOperatore = value; } }

        #endregion Public Properties


        #region Nested Class

        [DataContract]
        public class CtrlBypassTipologieNonAbilitate
        {
            public CtrlBypassTipologieNonAbilitate()
            { }

            public CtrlBypassTipologieNonAbilitate(GestioneCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate datiCtrlBypassTipologieNonAbilitate)
            {
                _Tipologia = datiCtrlBypassTipologieNonAbilitate.Tipologia;
                _Fondo = datiCtrlBypassTipologieNonAbilitate.Fondo;
                _Gruppo = datiCtrlBypassTipologieNonAbilitate.Gruppo;
                _Prodotto = datiCtrlBypassTipologieNonAbilitate.Prodotto;
                _Tipo = datiCtrlBypassTipologieNonAbilitate.Tipo;
                _Filtro = datiCtrlBypassTipologieNonAbilitate.Filtro;
                _Categoria = datiCtrlBypassTipologieNonAbilitate.Categoria;
                _Sede = datiCtrlBypassTipologieNonAbilitate.Sede;
            }

            #region Private Properties

            private string _Tipologia;
            private string _Fondo;
            private string _Gruppo;
            private string _Prodotto;
            private string _Tipo;
            private string _Filtro;
            private string _Categoria;
            private short _Sede;

            #endregion Private Properties

            #region Public Properties

            [DataMember]
            public string Tipologia { get { return _Tipologia; } set { _Tipologia = value; } }
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
            public string Categoria { get { return _Categoria; } set { _Categoria = value; } }
            [DataMember]
            public short Sede { get { return _Sede; } set { _Sede = value; } }

            #endregion Public Properties
        }

        #endregion Nested Class
    }
}