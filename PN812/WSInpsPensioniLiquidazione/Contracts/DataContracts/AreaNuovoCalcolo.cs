using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaNuovoCalcolo
    {
        public AreaNuovoCalcolo()
        {
            this.Esito = new AreaEsito();
        }

        #region private properties
        protected AreaEsito _Esito;
        protected DatiNuovoCalcolo _EsitoNuovoCalcolo;
        #endregion private properties

        #region public data member
        [DataMember]
        public AreaEsito Esito { get { return _Esito; } set { _Esito = value; } }
        [DataMember]
        public DatiNuovoCalcolo EsitoNuovoCalcolo { get { return _EsitoNuovoCalcolo; } set { _EsitoNuovoCalcolo = value; } }
        #endregion public data member

        [DataContract]
        public class DatiNuovoCalcolo
        {
            public DatiNuovoCalcolo()
            {
            }

            internal DatiNuovoCalcolo(BLCommon.GestioneNuovoCalcolo.DatiEsitoNuovoCalcolo esitoNuovoCalcolo)
            {
                this._NDomus = esitoNuovoCalcolo.NDomus;
                this._TransactionId = esitoNuovoCalcolo.TransactionId;
                this._DataInserimento = esitoNuovoCalcolo.DataInserimento;
                this._Risposta = esitoNuovoCalcolo.Risposta;
                this._DataRisposta = esitoNuovoCalcolo.DataRisposta;
            }

            #region private properties

            private long? _Id;
            private long? _NDomus;
            private string _TransactionId;
            private string _Risposta;
            private DateTime? _DataInserimento;
            private DateTime? _DataRisposta;
            private DateTime? _DataCalc;
            private string _StatoPensione;

            #endregion private properties

            #region public data member

            [DataMember]
            public long? NDomus { get { return _NDomus; } set { _NDomus = value; } }

            [DataMember]
            public string TransactionId { get { return _TransactionId; } set { _TransactionId = value; } }

            [DataMember]
            public DateTime? DataInserimento { get { return _DataInserimento; } set { _DataInserimento = value; } }

            [DataMember]
            public string Risposta { get { return _Risposta; } set { _Risposta = value; } }

            [DataMember]
            public DateTime? DataRisposta { get { return _DataRisposta; } set { _DataRisposta = value; } }

            [DataMember]
            public DateTime? DataCalc { get { return _DataCalc; } set { _DataCalc = value; } }

            [DataMember]
            public string StatoPensione { get { return _StatoPensione; } set { _StatoPensione = value; } }
            #endregion public data member
        }
    }

}