using INPS.Pensioni.Liquidazione.BLCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace INPS.Pensioni.LiquidazioneFs.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaDatiAgoFondoPI
    {
        #region private properties

        private long? _Id;
        private GestioneFondo.DatiAgoPI _DettaglioDatiAgoFondoPi;
        #endregion

        #region public data member

        [DataMember]
        public GestioneFondo.DatiAgoPI DettaglioDatiAgoFondoPi { get { return _DettaglioDatiAgoFondoPi; } set { _DettaglioDatiAgoFondoPi = value; } }

        [DataMember]
        public long? Id { get { return _Id; } set { _Id = value; } }

        #endregion
    }
}