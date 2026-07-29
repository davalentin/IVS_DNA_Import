using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Data;

namespace INPS.Pensioni.LiquidazioneCi.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaDatiContributivi
    {
        #region private properties

        private GestioneContrib.ProRata _ProRata;
        private GestioneContrib.DatiCalcolo _DatiCalcolo;
        
        private GestioneContrib.LavoratoriAutonomi _LavoratoriAutonomi;

        private List<GestioneContrib.MaternitaAcna> _LMaternitaAcna;
        private List<GestioneContrib.PensioniCiImportiValuta> _LimportiEsteriValuta;
        private List<GestioneContrib.CodiceConvenzione> _ElencoCodiceConvenzione;
        private List<GestioneContrib.CodiceVirtuale> _ElencoCodiceVirtuale;
        private List<GestioneContrib.RegimeLiquidazione> _ElencoRegimeLiquidazione;
        private List<GestioneContrib.DatiPostDecOriginaria> _LDatiPostDecOriginaria;

        private bool _IsFineAssicurazionePost2012;
        private bool _IsInizioAssicurazionePost1995;
        private bool _IsPensioneTipoContributivo;

        private List<Entity.DecodificaGestioneCalcoloRetributivo> _listaDecodificaGestioneCalcoloRetributivo;

        private List<Entity.DecodificaGestioneCalcoloContributivo> _listaDecodificaGestioneCalcoloContributivo;
        private List<Entity.DecodificaCodeGestione> _listaDecodificaCodeGestione;

        private bool _IsSettimane707Visible;

        private List<GestioneContrib.RedditiPerIntegrazioneVirtuale> _LRedditiPerIntegrazioneVirtuale;

        #endregion private properties

        #region public properties

        [DataMember]
        public GestioneContrib.ProRata ProRata { get { return _ProRata; } set { _ProRata = value; } }

        [DataMember]
        public GestioneContrib.DatiCalcolo DatiCalcolo { get { return _DatiCalcolo; } set { _DatiCalcolo = value; } }

        [DataMember]
        public List<GestioneContrib.PensioniCiImportiValuta> LimportiEsteriValuta { get { return _LimportiEsteriValuta; } set { _LimportiEsteriValuta = value; } }

        [DataMember]
        public List<GestioneContrib.MaternitaAcna> LMaternitaAcna { get { return _LMaternitaAcna; } set { _LMaternitaAcna = value; } }

        [DataMember]
        public List<GestioneContrib.DatiPostDecOriginaria> LDatiPostDecOriginaria { get { return _LDatiPostDecOriginaria; } set { _LDatiPostDecOriginaria = value; } }

        [DataMember]
        public GestioneContrib.LavoratoriAutonomi LavoratoriAutonomi { get { return _LavoratoriAutonomi; } set { _LavoratoriAutonomi = value; } }

        [DataMember]
        public List<GestioneContrib.CodiceConvenzione> ElencoCodiceConvenzione { get { return _ElencoCodiceConvenzione; } set { _ElencoCodiceConvenzione = value; } }

        [DataMember]
        public List<GestioneContrib.CodiceVirtuale> ElencoCodiceVirtuale { get { return _ElencoCodiceVirtuale; } set { _ElencoCodiceVirtuale = value; } }

        [DataMember]
        public List<GestioneContrib.RegimeLiquidazione> ElencoRegimeLiquidazione { get { return _ElencoRegimeLiquidazione; } set { _ElencoRegimeLiquidazione = value; } }

        [DataMember]
        public bool IsFineAssicurazionePost2012 { get { return _IsFineAssicurazionePost2012; } set { _IsFineAssicurazionePost2012 = value; } }

        [DataMember]
        public bool IsInizioAssicurazionePost1995 { get { return _IsInizioAssicurazionePost1995; } set { _IsInizioAssicurazionePost1995 = value; } }
        [DataMember]
        public bool IsPensioneTipoContributivo { get { return _IsPensioneTipoContributivo; } set { _IsPensioneTipoContributivo = value; } }

        [DataMember]
        public List<Entity.DecodificaGestioneCalcoloRetributivo> ListaDecodificaGestioneCalcoloRetributivo 
        { 
            get { return _listaDecodificaGestioneCalcoloRetributivo; } 
            set { _listaDecodificaGestioneCalcoloRetributivo = value; }
        }
        
        [DataMember]
        public List<Entity.DecodificaGestioneCalcoloContributivo> ListaDecodificaGestioneCalcoloContributivo
        {
            get { return _listaDecodificaGestioneCalcoloContributivo; }
            set { _listaDecodificaGestioneCalcoloContributivo = value; }
        }
        
        [DataMember]
        public List<Entity.DecodificaCodeGestione> ListaDecodificaCodeGestione
        {
            get { return _listaDecodificaCodeGestione; }
            set { _listaDecodificaCodeGestione = value; }
        }

        [DataMember]
        public bool IsSettimane707Visible { get { return _IsSettimane707Visible; } set { _IsSettimane707Visible = value; } }

        [DataMember]
        public List<GestioneContrib.RedditiPerIntegrazioneVirtuale> LRedditiPerIntegrazioneVirtuale { get { return _LRedditiPerIntegrazioneVirtuale; } set { _LRedditiPerIntegrazioneVirtuale = value; } }

        #endregion public properties

        /*

        #region private properties
        private List<GestioneDatiContributivi.StatoEstero> _ElencoStatiEsteri;

        private List<GestioneDatiContributivi.CodiceConvenzione> _ElencoCodiceConvenzione;

        private List<GestioneDatiContributivi.CodiceVirtuale> _ElencoCodiceVirtuale;

        private List<GestioneDatiContributivi.RegimeLiquidazione> _ElencoRegimeLiquidazione;
        #endregion private properties

        #region public data member
        [DataMember]
        public List<GestioneDatiContributivi.StatoEstero> ElencoStatiEsteri { get { return _ElencoStatiEsteri; } set { _ElencoStatiEsteri = value; } }

        [DataMember]
        public List<GestioneDatiContributivi.CodiceConvenzione> ElencoCodiceConvenzione { get { return _ElencoCodiceConvenzione; } set { _ElencoCodiceConvenzione = value; } }

        [DataMember]
        public List<GestioneDatiContributivi.CodiceVirtuale> ElencoCodiceVirtuale { get { return _ElencoCodiceVirtuale; } set { _ElencoCodiceVirtuale = value; } }

        [DataMember]
        public List<GestioneDatiContributivi.RegimeLiquidazione> ElencoRegimeLiquidazione { get { return _ElencoRegimeLiquidazione; } set { _ElencoRegimeLiquidazione = value; } }
        #endregion public data member
         
         
         */


    }
}