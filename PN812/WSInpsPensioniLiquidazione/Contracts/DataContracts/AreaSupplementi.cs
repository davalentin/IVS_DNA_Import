using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Data;
using INPS.Pensioni.Liquidazione.BLCommon.Entity;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaSupplementi
    {
        public AreaSupplementi()
        {
            _ListDatiSupplementi = new List<DatiSupplementi>();
            _ListaTipoSupplementi = new List<TipoSupplementi>();
            _ListDatiSupplementiENPALS = new List<DatiSupplementiENPALS>();
            ListaDecodificaTipoQuota = new List<Entity.TipoQuota>();
        }

        #region private properties

        private List<DatiSupplementi> _ListDatiSupplementi;
        private List<TipoSupplementi> _ListaTipoSupplementi;
        private SupplementiBase _SupplementiBase;
        private IntegrazioneArt11 _IntegrazioneArt11;
        private List<DatiSupplementiENPALS> _ListDatiSupplementiENPALS;
        private List<DatiSuppRecordENPALS> _ListaDatiSuppRecordENPALS;
        private bool _IsTipoCalcoloModificato;

        #endregion private properties

        #region public data member

        [DataMember]
        public List<DatiSupplementi> ListDatiSupplementi { get { return _ListDatiSupplementi; } set { _ListDatiSupplementi = value; } }

        [DataMember]
        public List<TipoSupplementi> ListTipoSupplementi { get { return _ListaTipoSupplementi; } set { _ListaTipoSupplementi = value; } }

        [DataMember]
        public SupplementiBase SupplementiBase { get { return _SupplementiBase; } set { _SupplementiBase = value; } }

        [DataMember]
        public IntegrazioneArt11 IntegrazioneArt11 { get { return _IntegrazioneArt11; } set { _IntegrazioneArt11 = value; } }

        [DataMember]
        public List<DatiSupplementiENPALS> ListDatiSupplementiENPALS { get { return _ListDatiSupplementiENPALS; } set { _ListDatiSupplementiENPALS = value; } }

        [DataMember]
        public BLCommon.Entity.DatiContribuzioneEnpals DatiContribuzioneEnpalsSAS { get; set; }

        [DataMember]
        public List<DatiSuppRecordENPALS> ListaDatiSuppRecordENPALS { get { return _ListaDatiSuppRecordENPALS; } set { _ListaDatiSuppRecordENPALS = value; } }

        [DataMember]
        public DatiSuppRecordENPALS DatiSuppRecordENPALS { get; set; }

        [DataMember]
        public bool IsDomandaSperimentaleDonna { get; set; }

        [DataMember]
        public bool IsContribuzioneEnpalsRetributivaVisible { get; set; }

        [DataMember]
        public bool IsContribuzioneEnpalsContributivaVisible { get; set; }

        [DataMember]
        public List<Entity.TipoQuota> ListaDecodificaTipoQuota { get; set; }

        [DataMember]
        public DateTime? DecorrenzaPensioneDanteCausa { get; set; }

        [DataMember]
        public List<DatiSupplementiCumulo> ListaDatiSupplementiCumulo { get; set; }

        [DataMember]
        public List<Entity.DecEnteGestioneFondo> ListaDecEnteGestioneFondo { get; set; }

        [DataMember]
        public bool IsReversibilitaOrRicostituzione { get; set; }

        [DataMember]
        public bool IsPannelloSupplementiAnte96 { get; set; }

        [DataMember]
        public bool? IsTipoCalcoloModificato { get; set; }

        //ENG - Memo 32_a/2018
        [DataMember]
        public List<DatiSupplementiCumulo> ListaDatiSupplementiCumuloStorico { get; set; }
        #endregion public data member

    }

}


