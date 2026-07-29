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
    public class RichiestaUfficiPagatori
    {
        public RichiestaUfficiPagatori()
        {
            this._Abi = 0;
            this._Cab = 0;
            this._Frazionario = 0;
            this._Iban = "";
            this._Bic = "";
            this._Cap = "";
            this._StatoEstero = "";
            this._Tipo = TipoRicerca.Abi_Cab;
            this._ModalitaPagamento = "";
            this._Libretto = "";
            this._CodCatastaleEstero = string.Empty;
        }

        public RichiestaUfficiPagatori(int abi, int cab, int frazionario, string iban, string bic, string cap, string statoEstero, TipoRicerca tipo, string modalitaPagamento, string libretto, 
            string codCatastale)
        {
            this._Abi = abi;
            this._Cab = cab;
            this._Frazionario = frazionario;
            this._Iban = iban;
            this._Bic = bic;
            this._Cap = cap;
            this._StatoEstero = statoEstero;
            this._Tipo = tipo;
            this._ModalitaPagamento = modalitaPagamento;
            this._Libretto = libretto;
            this._CodCatastaleEstero = codCatastale;
        }

        #region private properties

        private int _Abi;
        private int _Cab;
        private int _Frazionario;
        private string _Iban;
        private string _Bic;
        private string _Cap;
        private string _StatoEstero;
        private TipoRicerca _Tipo;
        private string _ModalitaPagamento;
        private string _Libretto;
        private string _CodCatastaleEstero;
        #endregion private properties

        #region public data members
        [DataMember]
        public int Abi { get { return _Abi; } set { _Abi = value; } }
        [DataMember]
        public int Cab { get { return _Cab; } set { _Cab = value; } }
        [DataMember]
        public int Frazionario { get { return _Frazionario; } set { _Frazionario = value; } }
        [DataMember]
        public string Iban { get { return _Iban; } set { _Iban = value; } }
        [DataMember]
        public string Bic { get { return _Bic; } set { _Bic = value; } }
        [DataMember]
        public string Cap { get { return _Cap; } set { _Cap = value; } }
        [DataMember]
        public string StatoEstero { get { return _StatoEstero; } set { _StatoEstero = value; } }
        [DataMember]
        public TipoRicerca Tipo { get { return _Tipo; } set { _Tipo = value; } }
        [DataMember]
        public string ModalitaPagamento { get { return _ModalitaPagamento; } set { _ModalitaPagamento = value; } }
        [DataMember]
        public string Libretto { get { return _Libretto; } set { _Libretto = value; } }
        [DataMember]
        public string CodCatastaleEstero { get { return _CodCatastaleEstero; } set { _CodCatastaleEstero = value; } }
        #endregion public data members


        public enum TipoRicerca
        {
            Abi_Cab,
            Abi_Frazionario,
            Abi_Cap,
            Iban_Banca,
            Iban_Posta,
            Estero,
            Cassa
        };
    }

    [DataContract]
    public class UfficioPagatore
    {
        public UfficioPagatore(string nome, string agenzia, string cap, string citta, string indirizzo, string codiceMeccanizzazione, int abi, int cab, int frazionario)
        {
            this._Nome = nome;
            this._Agenzia = agenzia;
            this._Cap = cap;
            this._Citta = citta;
            this._Indirizzo = indirizzo;
            this._CodiceMeccanizzazione = codiceMeccanizzazione;
            this._Abi = abi;
            this._Cab = cab;
            this._Frazionario = frazionario;
        }

        public UfficioPagatore()
        {
        }

        #region private properties

        private string _Nome;
        private string _Agenzia;
        private string _Cap;
        private string _Citta;
        private string _Indirizzo;
        private string _CodiceMeccanizzazione;
        private int _Abi;
        private int _Cab;
        private int _Frazionario;
        #endregion private properties

        #region public data members
        [DataMember]
        public string Nome { get { return _Nome; } set { _Nome = value; } }
        [DataMember]
        public string Agenzia { get { return _Agenzia; } set { _Agenzia = value; } }
        [DataMember]
        public string Cap { get { return _Cap; } set { _Cap = value; } }
        [DataMember]
        public string Citta { get { return _Citta; } set { _Citta = value; } }
        [DataMember]
        public string Indirizzo { get { return _Indirizzo; } set { _Indirizzo = value; } }
        [DataMember]
        public string CodiceMeccanizzazione { get { return _CodiceMeccanizzazione; } set { _CodiceMeccanizzazione = value; } }
        [DataMember]
        public int Abi { get { return _Abi; } set { _Abi = value; } }
        [DataMember]
        public int Cab { get { return _Cab; } set { _Cab = value; } }
        [DataMember]
        public int Frazionario { get { return _Frazionario; } set { _Frazionario = value; } }
        #endregion public data members
    }
}