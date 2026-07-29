using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Data;
using INPS.Pensioni.LiquidazioneAgo.Entity;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneAgo.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaDatiContributivi
    {
        #region private properties
        private GestioneContrib.DatiCalcolo _DatiCalcolo;
        private GestioneContrib.DatiCalcolo _DatiCalcoloStorico;
        private bool _IsFineAssicurazionePost2012;
        private bool _IsPensioneInabilitaPost2012;
        private GestioneContrib.DatiCalcoloENPALS _DatiCalcoloENPALS;
        private List<DecodificaGestioneCalcoloRetributivo> _lDecodificaGestioneCalcoloRetributivo;
        private List<DecodificaGestioneCalcoloContributivo> _lDecodificaGestioneCalcoloContributivo;
        private List<DecodificaGestioneQuotaFondoIntegrativo> _lDecodificaGestioneQuotaFondoIntegrativo;
        private List<DecodificaGestioneQuotaFondoINPGI> _lDecodificaGestioneQuotaFondoINPGI;
        private GestioneContrib.ProRata _ProRata;
        private GestioneContrib.PrestazioneEsteraCumulo _PrestazioneEstera;
        private GestioneContrib.StatoEsteroCumulo _StatoEsteroCumulo;
        private GestioneContrib.DatiQuotaFondoINPGI _DatiQuotaFondoINPGIStorico;
        #endregion private properties

        #region public data member
        [DataMember]
        public GestioneContrib.DatiCalcolo DatiCalcolo { get { return _DatiCalcolo; } set { _DatiCalcolo = value; } }
        [DataMember]
        public GestioneContrib.DatiCalcolo DatiCalcoloStorico { get { return _DatiCalcoloStorico; } set { _DatiCalcoloStorico = value; } }
        [DataMember]
        public bool IsFineAssicurazionePost2012 { get { return _IsFineAssicurazionePost2012; } set { _IsFineAssicurazionePost2012 = value; } }
        [DataMember]
        public bool IsPensioneInabilitaPost2012 { get { return _IsPensioneInabilitaPost2012; } set { _IsPensioneInabilitaPost2012 = value; } }
        [DataMember]
        public GestioneContrib.DatiCalcoloENPALS DatiCalcoloENPALS { get { return _DatiCalcoloENPALS; } set { _DatiCalcoloENPALS = value; } }
        [DataMember]
        public GestioneContrib.DatiExINPDAI DatiExINPDAI { get; set; }
        [DataMember]
        public GestioneContrib.DatiExINPDAI DatiExINPDAIStorico { get; set; }
        [DataMember]
        public GestioneContrib.DatiCalcoloQuotePensione DatiCalcoloQuotePensione { get; set; }
        [DataMember]
        public GestioneContrib.DatiCalcoloQuotePensione DatiCalcoloQuotePensioneStorico { get; set; }
        [DataMember]
        public GestioneContrib.DatiCalcoloQuoteMiglioramentiContrattuali DatiCalcoloQuoteMiglioramentiContrattuali { get; set; }
        [DataMember]
        public GestioneContrib.DatiCalcoloQuoteMiglioramentiContrattuali DatiCalcoloQuoteMiglioramentiContrattualiStorico { get; set; }
        [DataMember]
        public List<DecodificaGestioneCalcoloRetributivo> listaDecodificaGestioneCalcoloRetributivo { get { return _lDecodificaGestioneCalcoloRetributivo; } set { _lDecodificaGestioneCalcoloRetributivo = value; } }
        [DataMember]
        public List<DecodificaGestioneCalcoloContributivo> listaDecodificaGestioneCalcoloContributivo { get { return _lDecodificaGestioneCalcoloContributivo; } set { _lDecodificaGestioneCalcoloContributivo = value; } }
        [DataMember]
        public List<DecEnteGestioneFondo> listaDecEnteGestioneFondo { get; set; }
        [DataMember]
        public List<DecCodiceTrattenute> ListaDecCodiceTrattenute { get; set; }
        [DataMember]
        public GestioneContrib.DatiCalcoloVittimeTerrorismo DatiCalcoloVittimeTerrorismo { get; set; }
        [DataMember]
        public GestioneContrib.DatiQuotaFondoIntegrativo DatiQuotaFondoIntegrativo { get; set; }
        [DataMember]
        public List<DecodificaGestioneQuotaFondoIntegrativo> listaDecodificaGestioneQuotaFondoIntegrativo { get { return _lDecodificaGestioneQuotaFondoIntegrativo; } set { _lDecodificaGestioneQuotaFondoIntegrativo = value; } }
        [DataMember]
        public GestioneContrib.DatiQuotaFondoINPGI DatiQuotaFondoINPGI { get; set; }
        [DataMember]
        public List<DecodificaGestioneQuotaFondoINPGI> listaDecodificaGestioneQuotaFondoINPGI { get { return _lDecodificaGestioneQuotaFondoINPGI; } set { _lDecodificaGestioneQuotaFondoINPGI = value; } }
        [DataMember]
        public GestioneContrib.ProRata ProRata { get { return _ProRata; } set { _ProRata = value; } }
        [DataMember]
        public GestioneContrib.PrestazioneEsteraCumulo PrestazioneEstera { get { return _PrestazioneEstera; } set { _PrestazioneEstera = value; } }
        [DataMember]
        public GestioneContrib.StatoEsteroCumulo StatoEsteroCumulo { get { return _StatoEsteroCumulo; } set { _StatoEsteroCumulo = value; } }
        [DataMember]
        public GestioneContrib.DatiQuotaFondoINPGI DatiQuotaFondoINPGIStorico { get { return _DatiQuotaFondoINPGIStorico; } set { _DatiQuotaFondoINPGIStorico = value; } }

        [DataMember]
        public bool IsPnlImportoLordoAllaDecVisible { get; set; }
        [DataMember]
        public bool IsSettimane707Visible { get; set; }
        [DataMember]
        public bool IsDatiRetributiviVittimeVisible { get; set; }
        [DataMember]
        public bool IsDatiContributiviVittimeVisible { get; set; }
        [DataMember]
        public bool IsDatiImportoPensioneVittimeVisible { get; set; }
        [DataMember]
        public bool IsBeneficioImportoPensioneX { get; set; }
        [DataMember]
        public bool IsSettimaneImportoPensioneLocked { get; set; }
        [DataMember]
        public long? SoggettoBeneficiario { get; set; }
        [DataMember]
        public long? TipologiaPrestazione { get; set; }
        [DataMember]
        public long? TipologiaBeneficio { get; set; }
        [DataMember]
        public string GestioneImportoLordoAllaDec { get; set; }
        [DataMember]
        public char? TipoCalcoloVincenteUnicarpe { get; set; }
        [DataMember]
        public List<Entity.TipoCalcoloVincenteDAI> ListaTipoCalcoloVincenteDAI { get; set; }
        [DataMember]
        public bool? TipoCumulo { get; set; }
        [DataMember]
        public bool? IsBeneficioVittimeTerrorismo { get; set; }
        [DataMember]
        public bool? IsScaricoTrattenuteCumulo { get; set; }
        [DataMember]
        public bool IsSettimane707INPGIVisible { get; set; }
        [DataMember]
        public Utility.TipoAnte96? IsAnte96 { get; set; }
        [DataMember]
        public bool? IsDomandaVOPGIFiltroAGI { get; set; }
        [DataMember]
        public bool IsEliminataPerCauseVarie { get; set; }
        [DataMember]
        public bool? IsMemo102Abilitato { get; set; }
        [DataMember]
        public bool? MostraQuotaAnte96 { get; set; }
        [DataMember]
        public bool? IsDatiEsteriFromServices { get; set; }
        [DataMember]
        public bool? IsMemo74_2023Abilitato { get; set; }
        [DataMember]
        public bool? IsRicOTrfEsattoriali { get; set; }
        [DataMember]
        public DateTime? InizioAssicurazione { get; set; }
        [DataMember]
        public bool? IsDomandaINPGIFineAssicurazionePost30062022 { get; set; }
        [DataMember]
        public bool? Bypass_LIMITE7_INTERI_MONT_AMM { get; set; }
        [DataMember]
        public bool? IsRicOTrfAutmaticaINPGI{ get; set; }
        #endregion public data member

    }
}
