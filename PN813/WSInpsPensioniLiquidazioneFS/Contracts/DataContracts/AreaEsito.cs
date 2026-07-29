using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Data;
using INPS.Pensioni.LiquidazioneFs.Entity;
using INPS.Pensioni.LiquidazioneFs;


namespace INPS.Pensioni.LiquidazioneFs.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaEsito
    {
        #region private properties
        private TipoEsito _RisultatoOperazione;
        private string _Messaggio;
        #endregion private properties

        #region public data member
        [DataMember]
        public TipoEsito RisultatoOperazione { get { return _RisultatoOperazione; } set { _RisultatoOperazione = value; } }
        [DataMember]
        public string Messaggio { get { return _Messaggio; } set { _Messaggio = value; } }
        #endregion public data member

        #region nested class
        public enum TipoEsito
        {
            OK,
            KO
        };
        #endregion nested class
    }
}