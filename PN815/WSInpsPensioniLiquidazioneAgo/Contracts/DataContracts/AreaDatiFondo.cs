using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace INPS.Pensioni.LiquidazioneAgo.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaDatiFondo
    {
        private Entity.DatiRegistrazioneFondo _datiRegistrazioniFondo;
        private long _IdRecordFondo;
        private bool? _isPrimoRecord;
        private Entity.DatiPrivilegiate _DatiPrivilegiate;
        private Entity.DatiArticolo2 _DatiArticolo2;
        private Entity.DatiCalcolo _DatiCalcolo;
        private Entity.DatiFondo _DatiFondo;
        private bool? _IsDecPensAnteAgosto95;
        private DateTime? _DecorrenzaPensioneDirettaDC;
        private List<Entity.CodicePensioniPrivilegiate> _ListaCodicePensioniPrivilegiate;

        [DataMember]
        public long IdRecordFondo { get { return _IdRecordFondo; } set { _IdRecordFondo = value; } }
        [DataMember]
        public Entity.DatiPrivilegiate DatiPrivilegiate { get { return _DatiPrivilegiate; } set { _DatiPrivilegiate = value; } }
        [DataMember]
        public Entity.DatiArticolo2 DatiArticolo2 { get { return _DatiArticolo2; } set { _DatiArticolo2 = value; } }
        [DataMember]
        public Entity.DatiCalcolo DatiCalcolo { get { return _DatiCalcolo; } set { _DatiCalcolo = value; } }
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
    }
}