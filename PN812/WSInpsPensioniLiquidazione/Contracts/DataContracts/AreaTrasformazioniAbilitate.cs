using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaTrasformazioniAbilitate
    {
        public AreaTrasformazioniAbilitate()
        {
            this._ElencoTrasformazioniAbilitate = new List<DatiTrasformazioneAbilitata>();
            this._ElencoTipologie = new List<DatiTrasformazioneAbilitata.Tipo> { DatiTrasformazioneAbilitata.Tipo.FS, DatiTrasformazioneAbilitata.Tipo.CI, DatiTrasformazioneAbilitata.Tipo.AGO };
            this._ElencoSigleCategorie = new List<string>();
            this._ElencoSedi = new List<INPS.DNA.Office>();
        }

        #region private properties

        private List<DatiTrasformazioneAbilitata> _ElencoTrasformazioniAbilitate;

        private List<DatiTrasformazioneAbilitata.Tipo> _ElencoTipologie;

        private List<string> _ElencoSigleCategorie;

        private List<INPS.DNA.Office> _ElencoSedi;

        #endregion private properties

        #region public data member

        [DataMember]
        public List<DatiTrasformazioneAbilitata> ElencoTrasformazioniAbilitate { get { return _ElencoTrasformazioniAbilitate; } set { _ElencoTrasformazioniAbilitate = value; } }

        [DataMember]
        public List<DatiTrasformazioneAbilitata.Tipo> ElencoTipologie { get { return _ElencoTipologie; } set { _ElencoTipologie = value; } }

        [DataMember]
        public List<string> ElencoSigleCategorie { get { return _ElencoSigleCategorie; } set { _ElencoSigleCategorie = value; } }

        [DataMember]
        public List<INPS.DNA.Office> ElencoSedi { get { return _ElencoSedi; } set { _ElencoSedi = value; } }

        #endregion public data member

        #region nested class

        [DataContract]
        public class DatiTrasformazioneAbilitata
        {
            public DatiTrasformazioneAbilitata()
            {
            }

            internal DatiTrasformazioneAbilitata(GestioneTrasformazioniAbilitate.TrasformazioneAbilitata trasformazioneAbilitata)
            {
                this._SiglaCategoria = trasformazioneAbilitata.SiglaCategoria;
                this._Sede = trasformazioneAbilitata.Sede.HasValue ? trasformazioneAbilitata.Sede.Value.ToString().PadLeft(4, '0').PadRight(6, '0') : string.Empty;
                switch (trasformazioneAbilitata.Tipologia)
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
                }
            }

            #region private properties
            private string _SiglaCategoria;

            private string _Sede;

            private Tipo _Tipologia;

            #endregion private properties

            #region public data member
            [DataMember]
            public string SiglaCategoria { get { return _SiglaCategoria; } set { _SiglaCategoria = value; } }

            [DataMember]
            public string Sede { get { return _Sede; } set { _Sede = value; } }

            [DataMember]
            public Tipo Tipologia { get { return _Tipologia; } set { _Tipologia = value; } }

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