using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class AnagraficaDC
    {
        // tabella DanteCausa
        private DateTime? _DecorrenzaResidenza;
        private string _StatoEEResidenza;
        private byte? _ParentelaDC;
        private DateTime? _DataMorte;
        private DateTime? _DataMorteOrigine;
        private bool? _StatoEEResidenzaByArca;
        private bool? _CittadinanzaByArca;
        private bool? _DataMatrimonioByPrelievo;
        private char? _SiglaFamiliare;
        private byte? _ProvenienzaPensione;

        // tabella Anagrafica
        private long _IdAnagrafica;
        private string _CodiceFiscale;
        private string _Cognome;
        private string _Nome;
        private char? _Sesso;
        private DateTime? _DataNascita;
        private string _ComuneNascita;
        private string _CodiceComuneNascita;
        private string _ProvinciaNascita;
        private string _Cittadinanza;
        private DateTime? _DataMatrimonio;
        private DateTime? _DataNascitaContitolareConiuge;

        // Proprietà utili
        private bool? _IsResidenzaEE_DalEnabled;
        private bool? _IsContitolareConiuge;

        public DateTime? DecorrenzaResidenza { get { return _DecorrenzaResidenza; } set { _DecorrenzaResidenza = value; } }
        public string StatoEEResidenza { get { return _StatoEEResidenza; } set { _StatoEEResidenza = value; } }
        public byte? ParentelaDC { get { return _ParentelaDC; } set { _ParentelaDC = value; } }
        public DateTime? DataMorte { get { return _DataMorte; } set { _DataMorte = value; } }
        public long IdAnagrafica { get { return _IdAnagrafica; } set { _IdAnagrafica = value; } }
        public string CodiceFiscale { get { return _CodiceFiscale; } set { _CodiceFiscale = value; } }
        public string Cognome { get { return _Cognome; } set { _Cognome = value; } }
        public string Nome { get { return _Nome; } set { _Nome = value; } }
        public char? Sesso { get { return _Sesso; } set { _Sesso = value; } }
        public DateTime? DataNascita { get { return _DataNascita; } set { _DataNascita = value; } }
        public string ComuneNascita { get { return _ComuneNascita; } set { _ComuneNascita = value; } }
        public string CodiceComuneNascita { get { return _CodiceComuneNascita; } set { _CodiceComuneNascita = value; } }
        public string ProvinciaNascita { get { return _ProvinciaNascita; } set { _ProvinciaNascita = value; } }
        public string Cittadinanza { get { return _Cittadinanza; } set { _Cittadinanza = value; } }
        public DateTime? DataMorteOrigine { get { return _DataMorteOrigine; } set { _DataMorteOrigine = value; } }
        public DateTime? DataMatrimonio { get { return _DataMatrimonio; } set { _DataMatrimonio = value; } }
        public bool? IsResidenzaEE_DalEnabled { get { return _IsResidenzaEE_DalEnabled; } set { _IsResidenzaEE_DalEnabled = value; } }
        public bool? StatoEEResidenzaByArca { get { return _StatoEEResidenzaByArca; } set { _StatoEEResidenzaByArca = value; } }
        public bool? CittadinanzaByArca { get { return _CittadinanzaByArca; } set { _CittadinanzaByArca = value; } }
        public bool? IsContitolareConiuge { get { return _IsContitolareConiuge; } set { _IsContitolareConiuge = value; } }
        public DateTime? DataNascitaContitolareConiuge { get { return _DataNascitaContitolareConiuge; } set { _DataNascitaContitolareConiuge = value; } }
        public short? CategoriaFascicolo { get; set; }
        public short? SedeFascicolo { get; set; }
        public int? NumeroFascicolo { get; set; }
        public bool? DataMatrimonioByPrelievo { get { return _DataMatrimonioByPrelievo; } set { _DataMatrimonioByPrelievo = value; } }
        public char? SiglaFamiliare { get { return _SiglaFamiliare; } set { _SiglaFamiliare = value; } }
        public byte? ProvenienzaPensione { get { return _ProvenienzaPensione; } set { _ProvenienzaPensione = value; } }
    }
}
