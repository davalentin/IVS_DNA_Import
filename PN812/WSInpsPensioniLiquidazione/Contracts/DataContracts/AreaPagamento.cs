using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Data;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaPagamento
    {
        public AreaPagamento()
        {

        }
        public AreaPagamento(GestioneAreaPagamento.DatiPagamento pagamentoBL)
        {
            this._Pagamento = new GestioneAreaPagamento.DatiPagamento();
            BLCommon.Utility.ValorizzaOggetti(pagamentoBL, this._Pagamento);
        }

        #region private properties
        private GestioneAreaPagamento.DatiPagamento _Pagamento;
        private List<GestioneAreaPagamento.DatiCassaSede> _ListCassaSede;
        private List<GestioneAreaPagamento.DatiStatoEstero> _ListStatiEsteri;
        #endregion private properties

        #region public data member
        [DataMember]
        public GestioneAreaPagamento.DatiPagamento Pagamento { get { return _Pagamento; } set { _Pagamento = value; } }

        [DataMember]
        public List<GestioneAreaPagamento.DatiCassaSede> ListCassaSede { get { return _ListCassaSede; } set { _ListCassaSede = value; } }

        [DataMember]
        public List<GestioneAreaPagamento.DatiStatoEstero> ListStatiEsteri { get { return _ListStatiEsteri; } set { _ListStatiEsteri = value; } }

        [DataMember]
        public bool IsBancaItaliaFromWebDom { get; set; }

        [DataMember]
        public short? CodiceSedeDestinazione { get; set; }

        [DataMember]
        public byte? CentroOperativoDestinazione { get; set; }

        [DataMember]
        public bool IsPolarizzazionePerGestioneENPALSAttiva { get; set; }
        #endregion public data member
    }
}