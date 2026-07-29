using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class Anagrafica
    {
        #region private properties
        private long _Id;

        private string _MatricolaArca;

        private string _CodiceFiscale;

        private string _Cognome;

        private string _Nome;

        private string _CognomeAcquisito;

        private System.Nullable<char> _Sesso;

        private System.Nullable<System.DateTime> _DataNascita;

        private string _ComuneNascita;

        private string _CodiceComuneNascita;

        private string _ProvinciaNascita;

        private bool _IsNatoInItalia;

        private string _Cittadinanza;

        private string _ComuneResidenza;

        private string _CodiceComuneResidenza;

        private string _Indirizzo;

        private string _NCivico;

        private string _CAP;

        private string _ProvinciaResidenza;

        private string _FrazioneResidenza;

        private bool _IsResidenteInItalia;

        private System.Nullable<bool> _DomicilioEstero;

        private System.Nullable<bool> _ResidenzaEstero;

        private string _Codice1Arca;

        private string _Codice2Arca;

        private string _Tel;

        private string _Cell;

        private string _EMail;

        private char? _CodiceStatoCivile;

        private System.Nullable<System.DateTime> _DecorrenzaStatoCivile;

		private char? _CodiceDelegato;

		private char? _CodiceTutore;

        private System.Nullable<System.DateTime> _CessValAmmSost;

        private System.Nullable<System.DateTime> _DataMorte;

        private System.Nullable<System.DateTime> _DataMatrimonio;

        private bool _Confermato;

        #endregion private properties

        #region public properties
        public long Id { get { return _Id; } set { _Id = value; } }

        public string MatricolaArca { get { return _MatricolaArca; } set { _MatricolaArca = value; } }

        public string CodiceFiscale { get { return _CodiceFiscale; } set { _CodiceFiscale = value; } }

        public string Cognome { get { return _Cognome; } set { _Cognome = value; } }

        public string Nome { get { return _Nome; } set { _Nome = value; } }

        public string CognomeAcquisito { get { return _CognomeAcquisito; } set { _CognomeAcquisito = value; } }

        public System.Nullable<char> Sesso { get { return _Sesso; } set { _Sesso = value; } }

        public System.Nullable<System.DateTime> DataNascita { get { return _DataNascita; } set { _DataNascita = value; } }

        public string ComuneNascita { get { return _ComuneNascita; } set { _ComuneNascita = value; } }

        public string CodiceComuneNascita { get { return _CodiceComuneNascita; } set { _CodiceComuneNascita = value; } }

        public string ProvinciaNascita { get { return _ProvinciaNascita; } set { _ProvinciaNascita = value; } }

        public bool IsNatoInItalia { get { return _IsNatoInItalia; } set { _IsNatoInItalia = value; } }

        public string Cittadinanza { get { return _Cittadinanza; } set { _Cittadinanza = value; } }

        public string ComuneResidenza { get { return _ComuneResidenza; } set { _ComuneResidenza = value; } }

        public string CodiceComuneResidenza { get { return _CodiceComuneResidenza; } set { _CodiceComuneResidenza = value; } }

        public string Indirizzo { get { return _Indirizzo; } set { _Indirizzo = value; } }

        public string NCivico { get { return _NCivico; } set { _NCivico = value; } }

        public string CAP { get { return _CAP; } set { _CAP = value; } }

        public string ProvinciaResidenza { get { return _ProvinciaResidenza; } set { _ProvinciaResidenza = value; } }

        public string FrazioneResidenza { get { return _FrazioneResidenza; } set { _FrazioneResidenza = value; } }

        public bool IsResidenteInItalia { get { return _IsResidenteInItalia; } set { _IsResidenteInItalia = value; } }

        public System.Nullable<bool> DomicilioEstero { get { return _DomicilioEstero; } set { _DomicilioEstero = value; } }

        public System.Nullable<bool> ResidenzaEstero { get { return _ResidenzaEstero; } set { _ResidenzaEstero = value; } }

        public string Codice1Arca { get { return _Codice1Arca; } set { _Codice1Arca = value; } }

        public string Codice2Arca { get { return _Codice2Arca; } set { _Codice2Arca = value; } }

        public string Tel { get { return _Tel; } set { _Tel = value; } }

        public string Cell { get { return _Cell; } set { _Cell = value; } }

        public string EMail { get { return _EMail; } set { _EMail = value; } }

        public char? CodiceStatoCivile { get { return _CodiceStatoCivile; } set { _CodiceStatoCivile = value; } }

        public System.Nullable<System.DateTime> DecorrenzaStatoCivile { get { return _DecorrenzaStatoCivile; } set { _DecorrenzaStatoCivile = value; } }

		public char? CodiceDelegato { get { return _CodiceDelegato; } set { _CodiceDelegato = value; } }

		public char? CodiceTutore { get { return _CodiceTutore; } set { _CodiceTutore = value; } }

        public System.Nullable<System.DateTime> CessValAmmSost { get { return _CessValAmmSost; } set { _CessValAmmSost = value; } }

        public System.Nullable<System.DateTime> DataMorte { get { return _DataMorte; } set { _DataMorte = value; } }

        public System.Nullable<System.DateTime> DataMatrimonio { get { return _DataMatrimonio; } set { _DataMatrimonio = value; } }

        public bool Confermato { get { return _Confermato; } set { _Confermato = value; } }

        #endregion public properties
    }
}
