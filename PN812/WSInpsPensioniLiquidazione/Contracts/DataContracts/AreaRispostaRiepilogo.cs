using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Data;
using INPS.Pensioni.Liquidazione.Entity;
using INPS.Pensioni.Liquidazione.BLCommon;


namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaRispostaRiepilogo
    {
        public AreaRispostaRiepilogo()
        {
            this.Esito = new AreaEsito();
        }

        #region private properties
        protected AreaEsito _Esito;
        protected List<DatiRiepilogoDomanda> _ElencoDomande;
        protected List<DatiRiepilogoPensione> _ElencoPensioni;
        protected List<DatiRiepilogoSinonimo> _ElencoSinonimi;
        protected DatiRiepilogoAnagrafica _AnagraficaTitolare;
        protected DatiEsitoCalcolo _EsitoCalcolo;
        protected bool _IsDomandaDB;
        protected bool _IsDomandaCalcolataProvvisoria;
        protected string _SedeDiversa;
        protected bool _IsRicercaManualeDA;
        protected bool _IsNuovoCertificatoGeneratoEnpals;
        protected string _DecorrenzaFinestra;
        protected string _CodFase;
        //ENG - Pensioni Ovunque: gestione nuovo pannello
        protected bool _MostraPanelloMessBloccantePensioniOvunque;
        protected string _SedePensioneGP1ALZ6;
        protected string _CodCategoriaPensione;
        protected string _CertificatoInseguimentoPensione;
        //ENG - Gestione Popup per Memo 239
        protected bool _MostraPopupMemo239;
        //ENG - Gestione Popup per Memo 31/2023
        protected bool _MostraPopupMemo312023;
        #endregion private properties

        #region public data member
        [DataMember]
        public AreaEsito Esito { get { return _Esito; } set { _Esito = value; } }

        [DataMember]
        public List<DatiRiepilogoDomanda> ElencoDomande { get { return _ElencoDomande; } set { _ElencoDomande = value; } }

        [DataMember]
        public List<DatiRiepilogoPensione> ElencoPensioni { get { return _ElencoPensioni; } set { _ElencoPensioni = value; } }

        [DataMember]
        public List<DatiRiepilogoSinonimo> ElencoSinonimi { get { return _ElencoSinonimi; } set { _ElencoSinonimi = value; } }

        [DataMember]
        public DatiRiepilogoAnagrafica AnagraficaTitolare { get { return _AnagraficaTitolare; } set { _AnagraficaTitolare = value; } }

        [DataMember]
        public DatiEsitoCalcolo EsitoCalcolo { get { return _EsitoCalcolo; } set { _EsitoCalcolo = value; } }

        [DataMember]
        public bool IsDomandaDB { get { return _IsDomandaDB; } set { _IsDomandaDB = value; } }

        [DataMember]
        public bool IsDomandaCalcolataProvvisoria { get { return _IsDomandaCalcolataProvvisoria; } set { _IsDomandaCalcolataProvvisoria = value; } }

        [DataMember]
        public string SedeDiversa { get { return _SedeDiversa; } set { _SedeDiversa = value; } }

        [DataMember]
        public bool IsRicercaManualeDA { get { return _IsRicercaManualeDA; } set { _IsRicercaManualeDA = value; } }

        [DataMember]
        public bool IsNuovoCertificatoGeneratoEnpals { get { return _IsNuovoCertificatoGeneratoEnpals; } set { _IsNuovoCertificatoGeneratoEnpals = value; } }

        [DataMember]
        public string DecorrenzaFinestra { get { return _DecorrenzaFinestra; } set { _DecorrenzaFinestra = value; } }

        [DataMember]
        public string CodFase { get { return _CodFase; } set { _CodFase = value; } }

        //ENG - Pensioni Ovunque: gestione nuovo pannello
        [DataMember]
        public bool MostraPanelloMessBloccantePensioniOvunque { get { return _MostraPanelloMessBloccantePensioniOvunque; } set { _MostraPanelloMessBloccantePensioniOvunque = value; } }
        [DataMember]
        public string SedePensioneGP1ALZ6 { get { return _SedePensioneGP1ALZ6; } set { _SedePensioneGP1ALZ6 = value; } }
        [DataMember]
        public string CodCategoriaPensione { get { return _CodCategoriaPensione; } set { _CodCategoriaPensione = value; } }
        [DataMember]
        public string CertificatoInseguimentoPensione { get { return _CertificatoInseguimentoPensione; } set { _CertificatoInseguimentoPensione = value; } }
        //ENG - Gestione Popup per Memo 239
        [DataMember]
        public bool MostraPopupMemo239 { get { return _MostraPopupMemo239; } set { _MostraPopupMemo239 = value; } }
        //ENG - Gestione Popup per Memo 31/2023
        [DataMember]
        public bool MostraPopupMemo312023 { get { return _MostraPopupMemo312023; } set { _MostraPopupMemo312023 = value; } }

        #endregion public data member

        #region nested class

        [DataContract]
        public class DatiRiepilogoDomanda
        {
            public DatiRiepilogoDomanda()
            {
            }

            internal DatiRiepilogoDomanda(Domanda domanda, Utility.TipoAppartenenza? tipoAppartenenza, Utility.TipoFondo? tipoFondo, string urlDPI, bool isDomandaENPALS, bool isDomandaINPDAP, bool isDomandaRiapertura)
            {
                this._NumeroDomanda = domanda.NumeroDomanda;
                this._ProgStorico = domanda.ProgStorico;
                this._Categoria = domanda.Categoria;
                this._Sede = domanda.Sede;
                this._CentroOperativo = domanda.CentroOperativo;
                this._Certificato = domanda.Certificato;
                this._Tipo = domanda.Tipo;
                this._Stato = domanda.Stato;
                this._MatricolaUtenteAcquisizione = domanda.MatricolaUtenteAcquisizione;
                this._IsMatchMatricola = domanda.IsMatchMatricola;
                this._DescProdotto = domanda.DescProdotto;
                this._DescTipo = domanda.DescTipo;
                this._UrlDPI = urlDPI;
                this._IsDomandaENPALS = isDomandaENPALS;
                this._IsDomandaINPDAP = isDomandaINPDAP;
                this._IsDomandaRiapertura = isDomandaRiapertura;
                this.CodGruppo = domanda.CodGruppo;
                this.CodProdotto = domanda.CodProdotto;
                this.CodTipo = domanda.CodTipo;
                this.DescrizioneIstanza = domanda.DescrizioneIstanza;
                this.SiglaCategoriaPensione = domanda.SiglaCategoriaPensione;
                this.SedePensione = domanda.SedePensione;
                this.CertificatoPensione = domanda.CertificatoPensione;
                this.SedeDestinazione = domanda.SedeDestinazione;
                this.CentroOperativoDestinazione = domanda.CentroOperativoDestinazione;
                this.CodiceTipoRichiesta = domanda.CodiceTipoRichiesta;
                this.GP1ALB1 = domanda.GP1ALB1;
                this.GP2BB05 = domanda.GP2BB05;
                this.GP1AXE3 = domanda.GP1AXE3;
                this.DataEliminazioneContabile = domanda.DataEliminazioneContabile;
                this.IsScadenzaAssegnoConGiorno = domanda.IsScadenzaAssegnoConGiorno;
                this.DataCalcoloDefinitivoINDCOM = domanda.DataCalcoloDefinitivoINDCOM;
                this.SedeDaVisualizzare = domanda.SedeDaVisualizzare;
                this.CentroOperativoDaVisualizzare = domanda.CentroOperativoDaVisualizzare;
                this.IsConsultazioneDomandaTRF = domanda.isConsultazioneDomandaTRF;
                this.DataPresentazionePreAcquisizione = domanda.DataPresentazionePreAcquisizione;
                this.DecorrenzaFinestra = domanda.DecorrenzaFinestra;
                this.CodFase = domanda.CodFase;
                this.TipoAutomazione = domanda.TipoAutomazione;
                //sostituito da verifica valorizzazione area JSON (datiPensione.IsDatiAggiuntiviFromJSON)
                //if (!Utility.IsRicostituzione(domanda.CodGruppo) && !isDomandaRiapertura &&
                //(Utility.IsDomandaCRED27(domanda.Categoria) ||
                //Utility.IsDomandaCOOP28(domanda.Categoria) ||
                //Utility.IsDomandaVESO29(domanda.Categoria) ||
                //Utility.IsDomandaVESO33(domanda.Categoria) ||
                //Utility.IsDomandaVESO92(domanda.Categoria) ||
                //Utility.IsDomandaESOTEL(domanda.Categoria) ||
                //Utility.IsDomandaESPA(domanda.Categoria) ||
                //Utility.IsDomandaESOAMB(domanda.Categoria)))
                //{
                //    GestioneControlliDinamici.ControlloDinamico ctrl = null;
                //    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneDatiWebDomJSON", out ctrl);
                //    if (!(ctrl == null || ctrl.ValoreControllo != "SI"))
                //        this.IsAbilitazioneDatiWebDomJSON = true;
                //}

                if (tipoAppartenenza != null)
                {
                    switch (tipoAppartenenza)
                    {
                        case Utility.TipoAppartenenza.AGO:
                            this._TipoAppartenenza = TipoApp.AGO;
                            this._Tipofondo = null;
                            break;
                        case Utility.TipoAppartenenza.CI:
                            this._TipoAppartenenza = TipoApp.CI;
                            this._Tipofondo = null;
                            break;
                        case Utility.TipoAppartenenza.FS:

                            this._TipoAppartenenza = TipoApp.FS;

                            if (tipoFondo.HasValue)
                            {
                                this._Tipofondo = (DatiRiepilogoDomanda.TipoFondo)tipoFondo.Value;
                            }
                            else
                                this._Tipofondo = null;

                            break;
                    }

                }
                else
                {
                    this._TipoAppartenenza = null;
                    this._Tipofondo = null;
                }

                this.DataAcquisizione = domanda.DataAcquisizione;
                this.CodiceSedeLavorazione = domanda.CodiceSedeLavorazione;
            }

            #region private properties
            protected string _NumeroDomanda;
            protected byte? _ProgStorico;
            protected string _Categoria;
            protected string _Sede;
            protected string _CentroOperativo;
            protected string _Certificato;
            protected string _Tipo;
            protected string _Stato;
            protected TipoApp? _TipoAppartenenza;
            protected TipoFondo? _Tipofondo;
            protected string _MatricolaUtenteAcquisizione;
            protected bool _IsMatchMatricola;
            protected bool _IsCalcoloAbilitato;
            protected string _DescProdotto;
            protected string _DescTipo;
            protected string _UrlDPI;
            protected bool _IsDomandaENPALS;
            protected bool _IsDomandaINPDAP;
            protected bool _IsDomandaRiapertura;
            protected string _DecorrenzaFinestra;
            protected string _CodFase;
            protected int? _TipoAutomazione;
            #endregion private properties

            #region public data member
            [DataMember]
            public string NumeroDomanda { get { return _NumeroDomanda; } set { _NumeroDomanda = value; } }
            [DataMember]
            public byte? ProgStorico { get { return _ProgStorico; } set { _ProgStorico = value; } }
            [DataMember]
            public string Categoria { get { return _Categoria; } set { _Categoria = value; } }
            [DataMember]
            public string Sede { get { return _Sede; } set { _Sede = value; } }
            [DataMember]
            public string CentroOperativo { get { return _CentroOperativo; } set { _CentroOperativo = value; } }
            [DataMember]
            public string Certificato { get { return _Certificato; } set { _Certificato = value; } }
            [DataMember]
            public string Tipo { get { return _Tipo; } set { _Tipo = value; } }
            [DataMember]
            public string Stato { get { return _Stato; } set { _Stato = value; } }
            [DataMember]
            public TipoApp? TipoAppartenenza { get { return _TipoAppartenenza; } set { _TipoAppartenenza = value; } }
            [DataMember]
            public TipoFondo? Tipofondo { get { return _Tipofondo; } set { _Tipofondo = value; } }
            [DataMember]
            public string MatricolaUtenteAcquisizione { get { return _MatricolaUtenteAcquisizione; } set { _MatricolaUtenteAcquisizione = value; } }
            [DataMember]
            public bool IsMatchMatricola { get { return _IsMatchMatricola; } set { _IsMatchMatricola = value; } }
            [DataMember]
            public bool IsCalcoloAbilitato { get { return _IsCalcoloAbilitato; } set { _IsCalcoloAbilitato = value; } }
            [DataMember]
            public string DescProdotto { get { return _DescProdotto; } set { _DescProdotto = value; } }
            [DataMember]
            public string DescTipo { get { return _DescTipo; } set { _DescTipo = value; } }
            [DataMember]
            public string UrlDPI { get { return _UrlDPI; } set { _UrlDPI = value; } }
            [DataMember]
            public bool IsDomandaENPALS { get { return _IsDomandaENPALS; } set { _IsDomandaENPALS = value; } }
            [DataMember]
            public bool IsDomandaINPDAP { get { return _IsDomandaINPDAP; } set { _IsDomandaINPDAP = value; } }
            [DataMember]
            public bool IsDomandaRiapertura { get { return _IsDomandaRiapertura; } set { _IsDomandaRiapertura = value; } }
            [DataMember]
            public string CodGruppo { get; set; }
            [DataMember]
            public string CodProdotto { get; set; }
            [DataMember]
            public string CodTipo { get; set; }
            [DataMember]
            public string DescrizioneIstanza { get; set; }
            [DataMember]
            public string SiglaCategoriaPensione { get; set; }
            [DataMember]
            public string SedePensione { get; set; }
            [DataMember]
            public string CertificatoPensione { get; set; }
            [DataMember]
            public string SedeDestinazione { get; set; }
            [DataMember]
            public string CentroOperativoDestinazione { get; set; }
            [DataMember]
            public string CodiceTipoRichiesta { get; set; }
            [DataMember]
            public int? GP1ALB1 { get; set; }
            [DataMember]
            public string GP2BB05 { get; set; }

            [DataMember]
            public bool? IsScadenzaAssegnoConGiorno { get; set; }

            [DataMember]
            public short? GP1AXE3 { get; set; }

            [DataMember]
            public DateTime? DataEliminazioneContabile { get; set; }

            [DataMember]
            public DateTime? DataCalcoloDefinitivoINDCOM { get; set; }

            [DataMember]
            public string SedeDaVisualizzare { get; set; }

            [DataMember]
            public string CentroOperativoDaVisualizzare { get; set; }

            [DataMember]
            public bool IsConsultazioneDomandaTRF { get; set; }

            [DataMember]
            public DateTime? DataAcquisizione { get; set; }

            [DataMember]
            public DateTime? DataPresentazionePreAcquisizione { get; set; }

            [DataMember]
            public string DecorrenzaFinestra { get; set; }

            [DataMember]
            public string CodFase { get; set; }

            [DataMember]
            public int? TipoAutomazione { get; set; }

            //sostituito da verifica valorizzazione area JSON (datiPensione.IsDatiAggiuntiviFromJSON)
            //[DataMember]
            //public bool? IsAbilitazioneDatiWebDomJSON { get; set; }

            //ENG - Implementazione Meta Processo
            [DataMember]
            public short? CodiceSedeLavorazione { get; set; }

            #endregion public data member

            public enum TipoApp
            {
                FS,
                AGO,
                CI
            };

            public enum TipoFondo
            {
                FS,
                PM,
                PMS,
                VL,
                ES,
                ET,
                TT,
                DZ,
                GAS,
                EL,
                CL,
                PMO,
                PI,
                PL,
                PT
            };
        }

        [DataContract]
        public class DatiRiepilogoPensione
        {
            public DatiRiepilogoPensione()
            {
            }

            internal DatiRiepilogoPensione(string certificato, string categoria, string sede, DateTime? dataCalcolo,
                char tipoComponente, string eliminazione, TipoOperazione tipo)
            {
                this._Certificato = certificato;
                this._Categoria = categoria;
                this._Sede = sede;
                this._DataCalcolo = dataCalcolo.HasValue ? dataCalcolo.Value.ToString("MM-yyyy") : null;
                this._TipoComponente = GetTipoComponente(tipoComponente);
                this._Eliminazione = eliminazione;
                this._Tipo = tipo;
            }

            internal DatiRiepilogoPensione(Entity.Pensione pensione)
            {
                this._Certificato = pensione.Certificato;
                this._Categoria = pensione.Categoria;
                this._Sede = pensione.Sede;
                this._DataCalcolo = pensione.DataCalcolo.HasValue ? pensione.DataCalcolo.Value.ToString("MM-yyyy") : null;
                this._TipoComponente = GetTipoComponente(pensione.TipoComponente);
                this._Eliminazione = pensione.Eliminazione;
                if (pensione.IsRicostituibile)
                    this._Tipo = TipoOperazione.Ricostituibile;
                else
                    this._Tipo = TipoOperazione.NonRicostituibile;
            }

            #region private properties
            private string _Certificato;
            private string _Categoria;
            private string _Sede;
            private string _DataCalcolo;
            private string _TipoComponente;
            private string _Eliminazione;
            private TipoOperazione _Tipo;
            #endregion private properties

            #region public data member
            [DataMember]
            public string Certificato { get { return _Certificato; } set { _Certificato = value; } }
            [DataMember]
            public string Categoria { get { return _Categoria; } set { _Categoria = value; } }
            [DataMember]
            public string Sede { get { return _Sede; } set { _Sede = value; } }
            [DataMember]
            public string DataCalcolo { get { return _DataCalcolo; } set { _DataCalcolo = value; } }
            [DataMember]
            public string TipoComponente { get { return _TipoComponente; } set { _TipoComponente = value; } }
            [DataMember]
            public string Eliminazione { get { return _Eliminazione; } set { _Eliminazione = value; } }
            [DataMember]
            public TipoOperazione Tipo { get { return _Tipo; } set { _Tipo = value; } }
            #endregion public data member

            #region nested class
            public enum TipoOperazione
            {
                Ricostituibile,
                NonRicostituibile
            };
            #endregion nested class

            #region private members
            private static string GetTipoComponente(char? tipoComponente)
            {
                string descrizione = string.Empty;
                GestioneDecodifica.GetTipoComponenteByCode(tipoComponente.GetValueOrDefault(), out descrizione);
                return descrizione;
            }
            #endregion private members
        }

        [DataContract]
        public class DatiRiepilogoSinonimo
        {
            public DatiRiepilogoSinonimo()
            {
            }

            internal DatiRiepilogoSinonimo(string matricolaARCA, string codiceFiscale, string cognome, string nome, DateTime? dataNascita)
            {
                this._MatricolaARCA = matricolaARCA;
                this._CodiceFiscale = codiceFiscale;
                this._Cognome = cognome;
                this._Nome = nome;
                this._DataNascita = dataNascita;
            }

            internal DatiRiepilogoSinonimo(Entity.Anagrafica anagrafica)
            {
                this._MatricolaARCA = anagrafica.MatricolaArca;
                this._CodiceFiscale = anagrafica.CodiceFiscale;
                this._Cognome = anagrafica.Cognome;
                this._Nome = anagrafica.Nome;
                this._DataNascita = anagrafica.DataNascita;
            }

            #region private properties
            private string _MatricolaARCA;
            private string _CodiceFiscale;
            private string _Cognome;
            private string _Nome;
            private DateTime? _DataNascita;
            #endregion private properties

            #region public data member
            [DataMember]
            public string MatricolaARCA { get { return _MatricolaARCA; } set { _MatricolaARCA = value; } }

            [DataMember]
            public string CodiceFiscale { get { return _CodiceFiscale; } set { _CodiceFiscale = value; } }

            [DataMember]
            public string Cognome { get { return _Cognome; } set { _Cognome = value; } }

            [DataMember]
            public string Nome { get { return _Nome; } set { _Nome = value; } }

            [DataMember]
            public DateTime? DataNascita { get { return _DataNascita; } set { _DataNascita = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiRiepilogoAnagrafica
        {
            public DatiRiepilogoAnagrafica()
            {
            }

            internal DatiRiepilogoAnagrafica(Entity.Anagrafica anagrafica)
            {
                this._CodiceFiscale = anagrafica.CodiceFiscale;
                this._Cognome = anagrafica.Cognome;
                this._Nome = anagrafica.Nome;
                this._DataNascita = anagrafica.DataNascita;
                this._Sesso = anagrafica.Sesso;
                this._ComuneNascita = anagrafica.ComuneNascita;
                this._ProvinciaNascita = anagrafica.ProvinciaNascita;
                this._Indirizzo = anagrafica.Indirizzo;
                this._NumeroCivico = anagrafica.NCivico;
                this._Cap = anagrafica.CAP;
                this._ComuneResidenza = anagrafica.ComuneResidenza;
                this._ProvinciaResidenza = anagrafica.ProvinciaResidenza;
                this._Tel = anagrafica.Tel;
                this._Cell = anagrafica.Cell;
                this._EMail = anagrafica.EMail;
                this._CodiceStatoCivile = anagrafica.CodiceStatoCivile;
                this._DecorrenzaStatoCivile = anagrafica.DecorrenzaStatoCivile;
                this._IsNatoInItalia = anagrafica.IsNatoInItalia;
                this._IsResidenteInItalia = anagrafica.IsResidenteInItalia;
                this._CodiceDelegato = anagrafica.CodiceDelegato;
                this._CodiceTutore = anagrafica.CodiceTutore;
                this._CessValAmmSost = anagrafica.CessValAmmSost;
                this._MatricolaArca = anagrafica.MatricolaArca;
                this._CognomeAcquisito = anagrafica.CognomeAcquisito;
                this._CodiceComuneNascita = anagrafica.CodiceComuneNascita;
                this._Cittadinanza = anagrafica.Cittadinanza;
                this._CodiceComuneResidenza = anagrafica.CodiceComuneResidenza;
                this._FrazioneResidenza = anagrafica.FrazioneResidenza;
                this._DomicilioEstero = anagrafica.DomicilioEstero;
                this._ResidenzaEstero = anagrafica.ResidenzaEstero;
                this._Codice1Arca = anagrafica.Codice1Arca;
                this._Codice2Arca = anagrafica.Codice2Arca;
                this._DataMorte = anagrafica.DataMorte;
            }

            #region private properties
            private string _CodiceFiscale;
            private string _Cognome;
            private string _Nome;
            private DateTime? _DataNascita;
            private char? _Sesso;
            private string _ComuneNascita;
            private string _ProvinciaNascita;
            private string _Indirizzo;
            private string _NumeroCivico;
            private string _Cap;
            private string _ComuneResidenza;
            private string _ProvinciaResidenza;
            private string _Tel;
            private string _Cell;
            private string _EMail;
            private char? _CodiceStatoCivile;
            private DateTime? _DecorrenzaStatoCivile;
            private bool _IsNatoInItalia;
            private bool _IsResidenteInItalia;
            private char? _CodiceDelegato;
            private char? _CodiceTutore;
            private System.Nullable<DateTime> _CessValAmmSost;
            private string _MatricolaArca;
            private string _CognomeAcquisito;
            private string _CodiceComuneNascita;
            private string _Cittadinanza;
            private string _CodiceComuneResidenza;
            private string _FrazioneResidenza;
            private System.Nullable<bool> _DomicilioEstero;
            private System.Nullable<bool> _ResidenzaEstero;
            private string _Codice1Arca;
            private string _Codice2Arca;
            private DateTime? _DataMorte;
            #endregion private properties

            #region public data member

            [DataMember]
            public string CodiceFiscale { get { return _CodiceFiscale; } set { _CodiceFiscale = value; } }

            [DataMember]
            public string Cognome { get { return _Cognome; } set { _Cognome = value; } }

            [DataMember]
            public string Nome { get { return _Nome; } set { _Nome = value; } }

            [DataMember]
            public DateTime? DataNascita { get { return _DataNascita; } set { _DataNascita = value; } }

            [DataMember]
            public char? Sesso { get { return _Sesso; } set { _Sesso = value; } }

            [DataMember]
            public string ComuneNascita { get { return _ComuneNascita; } set { _ComuneNascita = value; } }

            [DataMember]
            public string ProvinciaNascita { get { return _ProvinciaNascita; } set { _ProvinciaNascita = value; } }

            [DataMember]
            public string Indirizzo { get { return _Indirizzo; } set { _Indirizzo = value; } }

            [DataMember]
            public string NumeroCivico { get { return _NumeroCivico; } set { _NumeroCivico = value; } }

            [DataMember]
            public string Cap { get { return _Cap; } set { _Cap = value; } }

            [DataMember]
            public string ComuneResidenza { get { return _ComuneResidenza; } set { _ComuneResidenza = value; } }

            [DataMember]
            public string ProvinciaResidenza { get { return _ProvinciaResidenza; } set { _ProvinciaResidenza = value; } }

            [DataMember]
            public string Tel { get { return _Tel; } set { _Tel = value; } }

            [DataMember]
            public string Cell { get { return _Cell; } set { _Cell = value; } }

            [DataMember]
            public string EMail { get { return _EMail; } set { _EMail = value; } }

            [DataMember]
            public char? CodiceStatoCivile { get { return _CodiceStatoCivile; } set { _CodiceStatoCivile = value; } }

            [DataMember]
            public DateTime? DecorrenzaStatoCivile { get { return _DecorrenzaStatoCivile; } set { _DecorrenzaStatoCivile = value; } }

            [DataMember]
            public bool IsNatoInItalia { get { return _IsNatoInItalia; } set { _IsNatoInItalia = value; } }

            [DataMember]
            public bool IsResidenteInItalia { get { return _IsResidenteInItalia; } set { _IsResidenteInItalia = value; } }

            [DataMember]
            public char? CodiceDelegato { get { return _CodiceDelegato; } set { _CodiceDelegato = value; } }

            [DataMember]
            public char? CodiceTutore { get { return _CodiceTutore; } set { _CodiceTutore = value; } }

            [DataMember]
            public DateTime? CessValAmmSost { get { return _CessValAmmSost; } set { _CessValAmmSost = value; } }

            [DataMember]
            public string MatricolaArca { get { return _MatricolaArca; } set { _MatricolaArca = value; } }

            [DataMember]
            public string CognomeAcquisito { get { return _CognomeAcquisito; } set { _CognomeAcquisito = value; } }

            [DataMember]
            public string CodiceComuneNascita { get { return _CodiceComuneNascita; } set { _CodiceComuneNascita = value; } }

            [DataMember]
            public string Cittadinanza { get { return _Cittadinanza; } set { _Cittadinanza = value; } }

            [DataMember]
            public string CodiceComuneResidenza { get { return _CodiceComuneResidenza; } set { _CodiceComuneResidenza = value; } }

            [DataMember]
            public string FrazioneResidenza { get { return _FrazioneResidenza; } set { _FrazioneResidenza = value; } }

            [DataMember]
            public System.Nullable<bool> DomicilioEstero { get { return _DomicilioEstero; } set { _DomicilioEstero = value; } }

            [DataMember]
            public System.Nullable<bool> ResidenzaEstero { get { return _ResidenzaEstero; } set { _ResidenzaEstero = value; } }

            [DataMember]
            public string Codice1Arca { get { return _Codice1Arca; } set { _Codice1Arca = value; } }

            [DataMember]
            public string Codice2Arca { get { return _Codice2Arca; } set { _Codice2Arca = value; } }

            [DataMember]
            public DateTime? DataMorte { get { return _DataMorte; } set { _DataMorte = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiEsitoCalcolo
        {
            public DatiEsitoCalcolo()
            {
            }

            internal DatiEsitoCalcolo(BLCommon.GestioneEsitoCalcolo.DatiEsitoCalcolo esitoCalcolo)
            {
                this._Esito = esitoCalcolo.Esito;
                this._DettaglioEsito = esitoCalcolo.DettaglioEsito;
            }
            #region private properties
            private string _Esito;

            private string _DettaglioEsito;
            #endregion private properties

            #region public data member
            [DataMember]
            public string Esito { get { return _Esito; } set { _Esito = value; } }

            [DataMember]
            public string DettaglioEsito { get { return _DettaglioEsito; } set { _DettaglioEsito = value; } }
            #endregion public data member
        }

        #endregion nested class
    }
}
