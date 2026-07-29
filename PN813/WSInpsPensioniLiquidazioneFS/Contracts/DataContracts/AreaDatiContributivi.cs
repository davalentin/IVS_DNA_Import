using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Data;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneFs.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaDatiContributivi
    {
        #region private properties
        private GestioneContrib.DatiCalcolo _DatiCalcolo;
        private Entity.DatiCalcolo707 _DatiCalcolo707;
        private GestioneContrib.EntityDatiFondo _DatiFondo;
        private GestioneContrib.DatiArt11e14 _DatiArt11e14;
        private GestioneContrib.DatiAnte67 _DatiAnte67;
        private GestioneContrib.DatiAgoAltraPensione _DatiAgoAltraPensione;

        private List<GestioneFondo.PretabellaDatiAgoFondoPI> _ElencoDatiAgo;
        private List<GestioneFondo.PretabellaPensioneFondoPI> _ElencoDatiPensioneFondoPI;

        private GestioneLiquidazionePensione.TipoSalvaguardia? _TipologiaSalvaguardia;
        private Dictionary<string, char?> _TipoPensione;

        private bool? _IsRiduzioneRetribVisible;
        private bool? _IsContribL214Visible;
        private bool? _IsAnzianita;
        private bool? _IsVecchiaiaSpecifica;
        private bool? _IsInvaliditaSpecifica;
        private bool? _IsUsuranti;
        private bool? _IsAltraPensioneVisible;
        private bool? _IsRiduzioneRetributivaEnabled;
        private bool? _IsSettimane707Visible;
        private bool? _IsAnteArmonizzazione;



        private List<Entity.TipoLiquidazioneGAS> _ListaTipoLiquidazioneGAS;
        private List<Entity.TipoLiquidazionePI> _ListaTipoLiquidazionePI;
        private List<Entity.AttCon> _ListaAttCon;

        private Utility.CategoriaFondoPI? _CategoriaFondoPI;

        //ENG - PL CONTRIBUZIONE POST 2011
        private bool? _IsContribuzioneL335NonObbligatoria;

        private bool? _IsPIAPIBAnte99;
        private long? _IdFondo;

        #endregion private properties

        #region public data member
        [DataMember]
        public GestioneContrib.DatiCalcolo DatiCalcolo { get { return _DatiCalcolo; } set { _DatiCalcolo = value; } }
        [DataMember]
        public Entity.DatiCalcolo707 DatiCalcolo707 { get { return _DatiCalcolo707; } set { _DatiCalcolo707 = value; } }
        [DataMember]
        public GestioneContrib.EntityDatiFondo DatiFondo { get { return _DatiFondo; } set { _DatiFondo = value; } }
        [DataMember]
        public GestioneContrib.DatiArt11e14 DatiArt11e14 { get { return _DatiArt11e14; } set { _DatiArt11e14 = value; } }
        [DataMember]
        public GestioneContrib.DatiAnte67 DatiAnte67 { get { return _DatiAnte67; } set { _DatiAnte67 = value; } }
        [DataMember]
        public GestioneContrib.DatiSL33670 DatiSL336 { get; set; }
        [DataMember]
        public GestioneContrib.DatiAgoAltraPensione DatiAgoAltraPensione { get { return _DatiAgoAltraPensione; } set { _DatiAgoAltraPensione = value; } }
        [DataMember]
        public GestioneContrib.DatiCalcolo DatiCalcoloStorico { get; set; }

        [DataMember]
        public bool? IsRiduzioneRetribVisible { get { return _IsRiduzioneRetribVisible; } set { _IsRiduzioneRetribVisible = value; } }
        [DataMember]
        public bool? IsContribL214Visible { get { return _IsContribL214Visible; } set { _IsContribL214Visible = value; } }
        [DataMember]
        public bool? IsAnzianita { get { return _IsAnzianita; } set { _IsAnzianita = value; } }
        [DataMember]
        public bool? IsVecchiaiaSpecifica { get { return _IsVecchiaiaSpecifica; } set { _IsVecchiaiaSpecifica = value; } }
        [DataMember]
        public bool? IsInvaliditaSpecifica { get { return _IsInvaliditaSpecifica; } set { _IsInvaliditaSpecifica = value; } }
        [DataMember]
        public bool? IsUsuranti { get { return _IsUsuranti; } set { _IsUsuranti = value; } }
        [DataMember]
        public GestioneLiquidazionePensione.TipoSalvaguardia? TipologiaSalvaguardia { get { return _TipologiaSalvaguardia; } set { _TipologiaSalvaguardia = value; } }
        [DataMember]
        public Dictionary<string, char?> TipoPensione { get { return _TipoPensione; } set { _TipoPensione = value; } }
        [DataMember]
        public bool? IsAltraPensioneVisible { get { return _IsAltraPensioneVisible; } set { _IsAltraPensioneVisible = value; } }
        [DataMember]
        public bool? IsDecorrenzaSuccSett1989 { get; set; }
        [DataMember]
        public bool? IsRiduzioneRetributivaEnabled { get { return _IsRiduzioneRetributivaEnabled; } set { _IsRiduzioneRetributivaEnabled = value; } }
        [DataMember]
        public bool? IsSettimane707Visible { get { return _IsSettimane707Visible; } set { _IsSettimane707Visible = value; } }
        [DataMember]
        public Utility.CategoriaFondoPI? CategoriaFondoPI { get { return _CategoriaFondoPI; } set { _CategoriaFondoPI = value; } }
        [DataMember]
        public bool? IsAnteArmonizzazione { get { return _IsAnteArmonizzazione; } set { _IsAnteArmonizzazione = value; } }

        [DataMember]
        public List<Entity.TipoLiquidazioneGAS> ListaTipoLiquidazioneGAS { get { return _ListaTipoLiquidazioneGAS; } set { _ListaTipoLiquidazioneGAS = value; } }

        [DataMember]
        public List<Entity.TipoLiquidazionePI> ListaTipoLiquidazionePI { get { return _ListaTipoLiquidazionePI; } set { _ListaTipoLiquidazionePI = value; } }
        [DataMember]
        public List<Entity.AttCon> ListaAttCon { get { return _ListaAttCon; } set { _ListaAttCon = value; } }

        //ENG - PL CONTRIBUZIONE POST 2011
        [DataMember]
        public bool? IsContribuzioneL335NonObbligatoria { get { return _IsContribuzioneL335NonObbligatoria; } set { _IsContribuzioneL335NonObbligatoria = value; } }

        [DataMember]
        public bool? IsPIAPIBAnte99 { get { return _IsPIAPIBAnte99; } set { _IsPIAPIBAnte99 = value; } }
        
        [DataMember]
        public List<GestioneFondo.PretabellaDatiAgoFondoPI> ElencoDatiAgo { get { return _ElencoDatiAgo; } set { _ElencoDatiAgo = value; } }


        [DataMember]
        public List<GestioneFondo.PretabellaPensioneFondoPI> ElencoDatiPensioneFondoPI { get { return _ElencoDatiPensioneFondoPI; } set { _ElencoDatiPensioneFondoPI = value; } }

        [DataMember]
        public long? IdFondo { get { return _IdFondo; } set { _IdFondo = value;  } }
        #endregion public data member


    }
}