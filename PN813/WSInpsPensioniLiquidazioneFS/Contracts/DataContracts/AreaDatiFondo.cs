using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneFs.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaDatiFondo
    {
        private Entity.DatiRegistrazioneFondo _datiRegistrazioniFondo;
        private long _IdRecordFondo;
        private bool? _isPrimoRecord;
        private Entity.DatiLegge460 _DatiLegge460;
        private Entity.DatiPrivilegiate _DatiPrivilegiate;
        private Entity.DatiArticolo2ForDatiFondo _DatiArticolo2;
        private Entity.DatiCalcolo _DatiCalcolo;
        private GestioneContrib.DatiCalcolo _DatiCalcoloDZ;
        private AreaDatiContributivi _CrossDataDZ;
        private Entity.DatiCalcolo707 _DatiCalcolo707;
        private Entity.CrossEntity.DatiMiglioramentiContrattuali _QuoteMiglioramentiContrattuali;
        private Entity.DatiFondo _DatiFondo;
        private bool? _IsDecPensAnteAgosto95;
        private DateTime? _DecorrenzaPensioneDirettaDC;
        private List<Entity.CodicePensioniPrivilegiate> _ListaCodicePensioniPrivilegiate;
        private bool? _IsContribL214Visible;
        private bool? _IsDomandaSperimentaleDonna;
        private bool? _IsPensioneTipoContributivo;
        private bool? _IsPensioneTipoContributivoConOpzione;
        private char? _tipoReversibilita;
        private DateTime? _FineAssicurazione;
        private bool? _IsRiduzioneRetribVisible;
        private bool? _IsSettimane707Visible;
        private bool? _IsUsuranti;
        private GestioneLiquidazionePensione.TipoSalvaguardia? _TipologiaSalvaguardia;
        private bool? _IsRiduzioneRetributivaEnabled;
        private Dictionary<string, char?> _TipoPensione;
        private double _IndennitaIntegrativaSpecialeLorda;

        [DataMember]
        public long IdRecordFondo { get { return _IdRecordFondo; } set { _IdRecordFondo = value; } }
        [DataMember]
        public Entity.DatiLegge460 DatiLegge460 { get { return _DatiLegge460; } set { _DatiLegge460 = value; } }
        [DataMember]
        public Entity.DatiPrivilegiate DatiPrivilegiate { get { return _DatiPrivilegiate; } set { _DatiPrivilegiate = value; } }
        [DataMember]
        public Entity.DatiArticolo2ForDatiFondo DatiArticolo2 { get { return _DatiArticolo2; } set { _DatiArticolo2 = value; } }
        [DataMember]
        public Entity.DatiCalcolo DatiCalcolo { get { return _DatiCalcolo; } set { _DatiCalcolo = value; } }
        [DataMember]
        public GestioneContrib.DatiCalcolo DatiCalcoloDZ { get { return _DatiCalcoloDZ; } set { _DatiCalcoloDZ = value; } }
        [DataMember]
        public AreaDatiContributivi CrossDataDZ { get { return _CrossDataDZ; } set { _CrossDataDZ = value; } }
        [DataMember]
        public Entity.DatiCalcolo707 DatiCalcolo707 { get { return _DatiCalcolo707; } set { _DatiCalcolo707 = value; } }
        [DataMember]
        public Entity.CrossEntity.DatiMiglioramentiContrattuali QuoteMiglioramentiContrattuali { get { return _QuoteMiglioramentiContrattuali; } set { _QuoteMiglioramentiContrattuali = value; } }
        [DataMember]
        public Entity.DatiRegistrazioneFondo DatiRegistrazioniFondo { get { return _datiRegistrazioniFondo; } set { _datiRegistrazioniFondo = value; } }
        [DataMember]
        public Entity.DatiFondo DatiFondo { get { return _DatiFondo; } set { _DatiFondo = value; } }
        [DataMember]
        public bool? IsPrimoRecord { get { return _isPrimoRecord; } set { _isPrimoRecord = value; } }
        [DataMember]
        public bool? IsDecPensAnteAgosto95 { get { return _IsDecPensAnteAgosto95; } set { _IsDecPensAnteAgosto95 = value; } }
        [DataMember]
        public DateTime? DecorrenzaPensioneDirettaDC { get { return _DecorrenzaPensioneDirettaDC; } set { _DecorrenzaPensioneDirettaDC = value; } }
        [DataMember]
        public List<Entity.CodicePensioniPrivilegiate> ListaCodicePensioniPrivilegiate { get { return _ListaCodicePensioniPrivilegiate; } set { _ListaCodicePensioniPrivilegiate = value; } }
        [DataMember]
        public bool? IsContribL214Visible { get { return _IsContribL214Visible; } set { _IsContribL214Visible = value; } }
        //proprietà is214
        [DataMember]
        public bool? IsDomandaSperimentaleDonna { get { return _IsDomandaSperimentaleDonna; } set { _IsDomandaSperimentaleDonna = value; } }
        [DataMember]
        public bool? IsPensioneTipoContributivo { get { return _IsPensioneTipoContributivo; } set { _IsPensioneTipoContributivo = value; } }
        [DataMember]
        public bool? IsPensioneTipoContributivoConOpzione { get { return _IsPensioneTipoContributivoConOpzione; } set { _IsPensioneTipoContributivoConOpzione = value; } }
        [DataMember]
        public char? TipoReversibilita { get { return _tipoReversibilita; } set { _tipoReversibilita = value; } }
        [DataMember]
        public DateTime? FineAssicurazione { get { return _FineAssicurazione; } set { _FineAssicurazione = value; } }
        [DataMember]
        public bool? IsRiduzioneRetribVisible { get { return _IsRiduzioneRetribVisible; } set { _IsRiduzioneRetribVisible = value; } }
        [DataMember]
        public bool? IsSettimane707Visible { get { return _IsSettimane707Visible; } set { _IsSettimane707Visible = value; } }
        [DataMember]
        public bool? IsUsuranti { get { return _IsUsuranti; } set { _IsUsuranti = value; } }
        [DataMember]
        public GestioneLiquidazionePensione.TipoSalvaguardia? TipologiaSalvaguardia { get { return _TipologiaSalvaguardia; } set { _TipologiaSalvaguardia = value; } }
        [DataMember]
        public bool? IsRiduzioneRetributivaEnabled { get { return _IsRiduzioneRetributivaEnabled; } set { _IsRiduzioneRetributivaEnabled = value; } }
        [DataMember]
        public Dictionary<string, char?> TipoPensione { get { return _TipoPensione; } set { _TipoPensione = value; } }
        [DataMember]
        public double IndennitaIntegrativaSpecialeLorda { get { return _IndennitaIntegrativaSpecialeLorda; }  set { _IndennitaIntegrativaSpecialeLorda = value; } }
    }
}