using System;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class Domanda
    {
        #region private properties
        private string _NumeroDomanda;
        private byte? _ProgStorico;
        private string _DescProdotto;
        private string _DescTipo;
        private string _Categoria;
        private string _Sede;
        private string _CentroOperativo;
        private string _Certificato;
        private string _Tipo;
        private string _Stato;
        private string _MatricolaUtenteAcquisizione;
        private bool _IsMatchMatricola;
        protected string _DecorrenzaFinestra;
        protected string _CodFase;
        #endregion private properties

        #region public data member
        public string NumeroDomanda { get { return _NumeroDomanda; } set { _NumeroDomanda = value; } }
        public byte? ProgStorico { get { return _ProgStorico; } set { _ProgStorico = value; } }
        public string DescProdotto { get { return _DescProdotto; } set { _DescProdotto = value; } }
        public string DescTipo { get { return _DescTipo; } set { _DescTipo = value; } }
        public string Categoria { get { return _Categoria; } set { _Categoria = value; } }
        public string Sede { get { return _Sede; } set { _Sede = value; } }
        public string CentroOperativo { get { return _CentroOperativo; } set { _CentroOperativo = value; } }
        public string Certificato { get { return _Certificato; } set { _Certificato = value; } }
        public string Tipo { get { return _Tipo; } set { _Tipo = value; } }
        public string Stato { get { return _Stato; } set { _Stato = value; } }
        public string MatricolaUtenteAcquisizione { get { return _MatricolaUtenteAcquisizione; } set { _MatricolaUtenteAcquisizione = value; } }
        public bool IsMatchMatricola { get { return _IsMatchMatricola; } set { _IsMatchMatricola = value; } }
        public string CodGruppo { get; set; }
        public string CodProdotto { get; set; }
        public string CodTipo { get; set; }
        public string DescrizioneIstanza { get; set; }
        public string SiglaCategoriaPensione { get; set; }
        public string SedePensione { get; set; }
        public string CertificatoPensione { get; set; }
        public string SedeDestinazione { get; set; }
        public string CentroOperativoDestinazione { get; set; }
        public string CodiceTipoRichiesta { get; set; }
        public int? GP1ALB1 { get; set; }
        public string GP2BB05 { get; set; }
        public short? GP1AXE3 { get; set; }
        public bool? IsScadenzaAssegnoConGiorno { get; set; }
        public DateTime? DataEliminazioneContabile { get; set; }
        public DateTime? DataCalcoloDefinitivoINDCOM { get; set; }  
        public string SedeDaVisualizzare { get; set; }
        public string CentroOperativoDaVisualizzare { get; set; }
        public bool isConsultazioneDomandaTRF { get; set; }
        public DateTime? DataAcquisizione { get; set; }

        public DateTime? DataPresentazionePreAcquisizione { get; set; }
        public string DecorrenzaFinestra { get { return _DecorrenzaFinestra; } set { _DecorrenzaFinestra = value; } }
        public string CodFase { get { return _CodFase; } set { _CodFase = value; } }

        public int? TipoAutomazione { get; set; }

        //ENG - Implementazione Meta processo
        public short? CodiceSedeLavorazione { get; set; }
        #endregion public data member
    }
}
