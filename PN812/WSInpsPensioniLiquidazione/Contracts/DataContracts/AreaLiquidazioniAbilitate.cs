using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Data;
using INPS.Pensioni.Liquidazione.BLCommon;


namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaLiquidazioniAbilitate
    {
        public AreaLiquidazioniAbilitate()
        {
            this._ElencoLiquidazioniAbilitate = new List<DatiLiquidazioneAbilitata>();
            this._ElencoTipologie = new List<DatiLiquidazioneAbilitata.Tipo> { DatiLiquidazioneAbilitata.Tipo.FS, DatiLiquidazioneAbilitata.Tipo.CI, DatiLiquidazioneAbilitata.Tipo.AGO};
            this._ElencoSigleCategorie = new List<string>();
            this._ElencoSedi = new List<INPS.DNA.Office>();
            this._ElencoSigleCategorieINPDAP = new List<string>();
        }

        #region private properties

        private List<DatiLiquidazioneAbilitata> _ElencoLiquidazioniAbilitate;

        private List<DatiLiquidazioneAbilitata.Tipo> _ElencoTipologie;

        private List<string> _ElencoSigleCategorie;

        private List<INPS.DNA.Office> _ElencoSedi;

        private List<string> _ElencoSigleCategorieINPDAP;

        #endregion private properties

        #region public data member

        [DataMember]
        public List<DatiLiquidazioneAbilitata> ElencoLiquidazioniAbilitate { get { return _ElencoLiquidazioniAbilitate; } set { _ElencoLiquidazioniAbilitate = value; } }

        [DataMember]
        public List<DatiLiquidazioneAbilitata.Tipo> ElencoTipologie { get { return _ElencoTipologie; } set { _ElencoTipologie = value; } }

        [DataMember]
        public List<string> ElencoSigleCategorie { get { return _ElencoSigleCategorie; } set { _ElencoSigleCategorie = value; } }

        [DataMember]
        public List<INPS.DNA.Office> ElencoSedi { get { return _ElencoSedi; } set { _ElencoSedi = value; } }

        [DataMember]
        public List<string> ElencoSigleCategorieINPDAP { get { return _ElencoSigleCategorieINPDAP; } set { _ElencoSigleCategorieINPDAP = value; } }

        #endregion public data member

        #region nested class

        [DataContract]
        public class DatiLiquidazioneAbilitata
        {
            public DatiLiquidazioneAbilitata()
            {
            }

            internal DatiLiquidazioneAbilitata(GestioneLiquidazioniAbilitate.LiquidazioneAbilitata liquidazioneAbilitata)
            {
                this._SiglaCategoria = liquidazioneAbilitata.SiglaCategoria;
                this._Sede = liquidazioneAbilitata.Sede.HasValue ? liquidazioneAbilitata.Sede.Value.ToString().PadLeft(4, '0').PadRight(6, '0') : string.Empty;
                switch (liquidazioneAbilitata.Tipologia)
                {
                    case "FS":
                        this._Tipologia = Tipo.FS;
                        break;
                    case "CI":
                        this._Tipologia = Tipo.CI;
                        break;
                    case "AGO":
                        this._Tipologia = Tipo.AGO;
                        break;
                    default:
                        break;
                }
                this._Ricostituzione = liquidazioneAbilitata.Ricostituzione.HasValue ? liquidazioneAbilitata.Ricostituzione.Value: false;
                this._AbilitazioneManuale = liquidazioneAbilitata.AbilitazioneManuale;
                this._RicostituzioneDaAutomatica = liquidazioneAbilitata.RicostituzioneDaAutomatica;
                this._AbilitazioneAutomatica = liquidazioneAbilitata.AbilitazioneAutomatica;
            }

            #region private properties
            private string _SiglaCategoria;

            private string _Sede;

            private Tipo _Tipologia;

            private bool _Ricostituzione;

            private bool _AbilitazioneManuale;

            private bool? _RicostituzioneDaAutomatica;

            private bool? _AbilitazioneAutomatica;
            #endregion private properties

            #region public data member
            [DataMember]
            public string SiglaCategoria { get { return _SiglaCategoria; } set { _SiglaCategoria = value; } }

            [DataMember]
            public string Sede { get { return _Sede; } set { _Sede = value; } }

            [DataMember]
            public Tipo Tipologia { get { return _Tipologia; } set { _Tipologia = value; } }

            [DataMember]
            public bool Ricostituzione { get { return _Ricostituzione; } set { _Ricostituzione = value; } }

            [DataMember]
            public bool AbilitazioneManuale { get { return _AbilitazioneManuale; } set { _AbilitazioneManuale = value; } }

            [DataMember]
            public bool? RicostituzioneDaAutomatica { get { return _RicostituzioneDaAutomatica; } set { _RicostituzioneDaAutomatica = value; } }

            [DataMember]
            public bool? AbilitazioneAutomatica { get { return _AbilitazioneAutomatica; } set { _AbilitazioneAutomatica = value; } }
            #endregion public data member

            public enum Tipo
            {
                AGO,
                FS,
                CI
            };
        }

        #endregion nested class
    }
}