using INPS.Pensioni.Liquidazione.BLCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace INPS.Pensioni.LiquidazioneFs.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaDatiPensioneFondoPI
    {
        #region private properties
        private long _IdFondo;
        private long? _IdRecordFondo;
        private long _NumDomanda;
        private short? _ControCodiceRetribuzione;
        private GestioneFondo.DatiFondoPI _DettaglioDatiPensioneFondoPi;
        private GestioneRecordFondo.DatiRecordFondo _DatiRecordFondo;
        #endregion

        #region public data member

        [DataMember]
        public long? IdRecordFondo { get { return _IdRecordFondo; } set { _IdRecordFondo = value; } }

        [DataMember]
        public long IdFondo { get { return _IdFondo; } set { _IdFondo = value; } }

        [DataMember]
        public long NumDomanda { get { return _NumDomanda; } set { _NumDomanda = value; } }

        [DataMember]
        public short? ControCodiceRetribuzione { get { return _ControCodiceRetribuzione; } set { _ControCodiceRetribuzione = value; } }


        [DataMember]
        public GestioneFondo.DatiFondoPI DettaglioDatiPensioneFondoPi { get { return _DettaglioDatiPensioneFondoPi; } set { _DettaglioDatiPensioneFondoPi = value; } }

        [DataMember]
        public GestioneRecordFondo.DatiRecordFondo DatiRecordFondo { get { return _DatiRecordFondo; } set { _DatiRecordFondo = value; } }
        #endregion
    }
}