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
    public class AreaPrelievo
    {
        #region private properties
        private GestionePrelievo.RichiestaPrelievo _Richiesta;
        private GestionePrelievo.RispostaPrelievo _Risposta;
        #endregion private properties

        #region public data member
        [DataMember]
        public GestionePrelievo.RichiestaPrelievo Richiesta { get { return _Richiesta; } set { _Richiesta = value; } }
        [DataMember]
        public GestionePrelievo.RispostaPrelievo Risposta { get { return _Risposta; } set { _Risposta = value; } }
        #endregion public data member

        #region nested class

        #endregion nested class
    }
}