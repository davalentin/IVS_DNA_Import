using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Data;


namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaDecodifica
    {
        #region Private properties

        private List<DatiStatoCivile> _ElencoStatiCivili;

        private List<DatiStatoEstero> _ElencoStatiEsteri;

        private List<DatiProvincia> _ElencoProvince;

        private List<DatiConiugeOFiglio> _ElencoConiugeOFiglio;

        private List<DatiDetrazioniReddito> _ElencoDetrazioniReddito;

        private List<DatiTutore> _ElencoTutore;

        private List<DatiDelegato> _ElencoDelegato;

        private List<DatiModalitaPagamento> _ElencoModalitaPagamento;

        private List<DatiTipoPagamento> _ElencoTipoPagamento;

        private List<DatiTipoCalcolo> _ElencoTipoCalcolo;

        private List<DatiCausaCarico> _ElencoCausaCarico;

        private List<DatiCodiceCristallizzazione> _ElencoCodiceCristallizzazione;

        private List<DatiTipoPensione> _ElencoTipoPensione;

        private List<DatiCodiceAzienda> _ElencoCodiceAzienda;

        private List<DatiGradoInvalidita> _ElencoGradoInvalidita;

        private List<DatiProrataEnel> _ElencoProrataEnel;

        private List<DatiComunicazioneCampi1_2> _ElencoComunicazioneCampi1_2;

        private List<DatiComunicazioneCampo3> _ElencoComunicazioneCampo3;

        private List<DatiComunicazioneCampo4> _ElencoComunicazioneCampo4;

        private List<DatiCodiciNatura> _ElencoCodiciNatura;

        private List<DatiCategoriaPensione> _ElencoCategoriePensione;

        private List<DatiFondoPensione> _ElencoFondiPensione;

        private List<DatiFondoPensione> _ElencoCasseGDP;

        private List<DatiStatoPensione> _ElencoStatiPensione;

        private List<DatiParentelaDC> _ElencoParentelaDC;

        private List<DatiCodiciProvenienza> _ElencoCodiciProvenienza;

        private List<DatiCodiciImportoAltraPensione> _ElencoCodiciImportoAltraPensione;

        private List<DatiCodiciVari> _ElencoCodiciVari;

        private List<DatiCodeGestioneCalcoloContrib> _ElencoCodeGestioneCalcoloContrib;

        private List<DatiCodeGestioneCalcoloRetrib> _ElencoCodeGestioneCalcoloRetrib;

        private List<DatiCodeMobilita> _ElencoCodeMobilita;

        private List<CodeGestione> _ElencoCodiceGestione;

        private List<DatiRicercaGPT> _ElencoRicercaGPT;

        private List<DatiCategoriaAltraPensione> _ElencoCategorieAltraPensione;

        private List<DatiCtrlEnteCassaCodiceGestione> _ElencoCtrlEnteCassaCodiceGestione;

        private List<DatiCtrlCatAdeguata> _ElencoCtrlCatAdeguata;

        private List<DatiDecComparto> _ElencoDecComparto;

        private List<DatiDecSettore> _ElencoDecSettore;

        private List<DatiDecRuolo> _ElencoDecRuolo;

        private List<DatiDecSede> _ElencoDecSede;

        #endregion Private properties

        #region Public data member

        [DataMember]
        public List<DatiCodeMobilita> ElencoCodeMobilita { get { return _ElencoCodeMobilita; } set { _ElencoCodeMobilita = value; } }

        [DataMember]
        public List<DatiStatoCivile> ElencoStatiCivili { get { return _ElencoStatiCivili; } set { _ElencoStatiCivili = value; } }

        [DataMember]
        public List<DatiStatoEstero> ElencoStatiEsteri { get { return _ElencoStatiEsteri; } set { _ElencoStatiEsteri = value; } }

        [DataMember]
        public List<DatiProvincia> ElencoProvince { get { return _ElencoProvince; } set { _ElencoProvince = value; } }

        [DataMember]
        public List<DatiConiugeOFiglio> ElencoConiugeOFiglio { get { return _ElencoConiugeOFiglio; } set { _ElencoConiugeOFiglio = value; } }

        [DataMember]
        public List<DatiDetrazioniReddito> ElencoDetrazioniReddito { get { return _ElencoDetrazioniReddito; } set { _ElencoDetrazioniReddito = value; } }

        [DataMember]
        public List<DatiTutore> ElencoTutore { get { return _ElencoTutore; } set { _ElencoTutore = value; } }

        [DataMember]
        public List<DatiDelegato> ElencoDelegato { get { return _ElencoDelegato; } set { _ElencoDelegato = value; } }

        [DataMember]
        public List<DatiModalitaPagamento> ElencoModalitaPagamento { get { return _ElencoModalitaPagamento; } set { _ElencoModalitaPagamento = value; } }

        [DataMember]
        public List<DatiTipoPagamento> ElencoTipoPagamento { get { return _ElencoTipoPagamento; } set { _ElencoTipoPagamento = value; } }

        [DataMember]
        public List<DatiTipoCalcolo> ElencoTipoCalcolo { get { return _ElencoTipoCalcolo; } set { _ElencoTipoCalcolo = value; } }

        [DataMember]
        public List<DatiCausaCarico> ElencoCausaCarico { get { return _ElencoCausaCarico; } set { _ElencoCausaCarico = value; } }

        [DataMember]
        public List<DatiCodiceCristallizzazione> ElencoCodiceCristallizzazione { get { return _ElencoCodiceCristallizzazione; } set { _ElencoCodiceCristallizzazione = value; } }

        [DataMember]
        public List<DatiTipoPensione> ElencoTipoPensione { get { return _ElencoTipoPensione; } set { _ElencoTipoPensione = value; } }

        [DataMember]
        public List<DatiCodiceAzienda> ElencoCodiceAzienda { get { return _ElencoCodiceAzienda; } set { _ElencoCodiceAzienda = value; } }

        [DataMember]
        public List<DatiGradoInvalidita> ElencoGradoInvalidita { get { return _ElencoGradoInvalidita; } set { _ElencoGradoInvalidita = value; } }

        [DataMember]
        public List<DatiProrataEnel> ElencoProrataEnel { get { return _ElencoProrataEnel; } set { _ElencoProrataEnel = value; } }

        [DataMember]
        public List<DatiComunicazioneCampi1_2> ElencoComunicazioneCampi1_2 { get { return _ElencoComunicazioneCampi1_2; } set { _ElencoComunicazioneCampi1_2 = value; } }

        [DataMember]
        public List<DatiComunicazioneCampo3> ElencoComunicazioneCampo3 { get { return _ElencoComunicazioneCampo3; } set { _ElencoComunicazioneCampo3 = value; } }

        [DataMember]
        public List<DatiComunicazioneCampo4> ElencoComunicazioneCampo4 { get { return _ElencoComunicazioneCampo4; } set { _ElencoComunicazioneCampo4 = value; } }

        [DataMember]
        public List<DatiCodiciNatura> ElencoCodiciNatura { get { return _ElencoCodiciNatura; } set { _ElencoCodiciNatura = value; } }

        [DataMember]
        public List<DatiCategoriaPensione> ElencoCategoriePensione { get { return _ElencoCategoriePensione; } set { _ElencoCategoriePensione = value; } }

        [DataMember]
        public List<DatiFondoPensione> ElencoFondiPensione { get { return _ElencoFondiPensione; } set { _ElencoFondiPensione = value; } }

        [DataMember]
        public List<DatiFondoPensione> ElencoCasseGDP { get { return _ElencoCasseGDP; } set { _ElencoCasseGDP = value; } }

        [DataMember]
        public List<DatiStatoPensione> ElencoStatiPensione { get { return _ElencoStatiPensione; } set { _ElencoStatiPensione = value; } }

        [DataMember]
        public List<DatiParentelaDC> ElencoParentelaDC { get { return _ElencoParentelaDC; } set { _ElencoParentelaDC = value; } }

        [DataMember]
        public List<DatiCodiciProvenienza> ElencoCodiciProvenienza { get { return _ElencoCodiciProvenienza; } set { _ElencoCodiciProvenienza = value; } }

        [DataMember]
        public List<DatiCodiciImportoAltraPensione> ElencoCodiciImportoAltraPensione { get { return _ElencoCodiciImportoAltraPensione; } set { _ElencoCodiciImportoAltraPensione = value; } }

        [DataMember]
        public List<DatiCodiciVari> ElencoCodiciVari { get { return _ElencoCodiciVari; } set { _ElencoCodiciVari = value; } }

        [DataMember]
        public List<DatiCodeGestioneCalcoloContrib> ElencoCodeGestioneCalcoloContrib { get { return _ElencoCodeGestioneCalcoloContrib; } set { _ElencoCodeGestioneCalcoloContrib = value; } }

        [DataMember]
        public List<DatiCodeGestioneCalcoloRetrib> ElencoCodeGestioneCalcoloRetrib { get { return _ElencoCodeGestioneCalcoloRetrib; } set { _ElencoCodeGestioneCalcoloRetrib = value; } }

        [DataMember]
        public List<CodeGestione> ElencoCodiceGestione { get { return _ElencoCodiceGestione; } set { _ElencoCodiceGestione = value; } }

        [DataMember]
        public List<DatiRicercaGPT> ElencoRicercaGPT { get { return _ElencoRicercaGPT; } set { _ElencoRicercaGPT = value; } }

        [DataMember]
        public List<DatiCategoriaAltraPensione> ElencoCategorieAltraPensione { get { return _ElencoCategorieAltraPensione; } set { _ElencoCategorieAltraPensione = value; } }

        [DataMember]
        public List<DatiCtrlEnteCassaCodiceGestione> ElencoCtrlEnteCassaCodiceGestione { get { return _ElencoCtrlEnteCassaCodiceGestione; } set { _ElencoCtrlEnteCassaCodiceGestione = value; } }

        [DataMember]
        public List<DatiCtrlCatAdeguata> ElencoCtrlCatAdeguata { get { return _ElencoCtrlCatAdeguata; } set { _ElencoCtrlCatAdeguata = value; } }

        [DataMember]
        public List<DatiDecComparto> ElencoDecComparto { get { return _ElencoDecComparto; } set { _ElencoDecComparto = value; } }

        [DataMember]
        public List<DatiDecSettore> ElencoDecSettore { get { return _ElencoDecSettore; } set { _ElencoDecSettore = value; } }

        [DataMember]
        public List<DatiDecRuolo> ElencoDecRuolo { get { return _ElencoDecRuolo; } set { _ElencoDecRuolo = value; } }

        [DataMember]
        public List<DatiDecSede> ElencoDecSede { get { return _ElencoDecSede; } set { _ElencoDecSede = value; } }

        #endregion Public data member

        #region nested class

        [DataContract]
        public class DatiCodeMobilita
        {
            public DatiCodeMobilita()
            {
            }

            internal DatiCodeMobilita(BLCommon.GestioneDecodifica.Mobilita mobilita)
            {
                this._Id = mobilita.Id;
                this._Descrizione = mobilita.Descrizione;
            }

            #region Private properties
            private string _Id;

            private string _Descrizione;
            #endregion Private properties

            #region Public data member
            [DataMember]
            public string Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion Public data member
        }

        [DataContract]
        public class DatiStatoCivile
        {
            public DatiStatoCivile()
            {
            }

            internal DatiStatoCivile(BLCommon.GestioneDecodifica.StatoCivile statoCivile)
            {
                this._Id = statoCivile.Id;
                this._Descrizione = statoCivile.Descrizione;
            }

            #region Private properties
            private char _Id;

            private string _Descrizione;
            #endregion Private properties

            #region Public data member
            [DataMember]
            public char Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion Public data member
        }

        [DataContract]
        public class DatiStatoEstero
        {
            public DatiStatoEstero()
            {
            }

            internal DatiStatoEstero(BLCommon.GestioneDecodifica.StatoEstero statoEstero)
            {
                this._CodCatastale = statoEstero.CodCatastale;
                this._Descrizione = statoEstero.Descrizione;
            }

            #region Private properties
            private string _CodCatastale;

            private string _Descrizione;
            #endregion Private properties

            #region Public data member
            [DataMember]
            public string CodCatastale { get { return _CodCatastale; } set { _CodCatastale = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion Public data member
        }

        [DataContract]
        public class DatiProvincia
        {
            public DatiProvincia()
            {
            }

            internal DatiProvincia(BLCommon.GestioneDecodifica.Provincia provincia)
            {
                this._SiglaProvincia = provincia.SiglaProvincia;
                this._DescrizioneProvincia = provincia.DescrizioneProvincia;
                this._DescrizioneRegione = provincia.DescrizioneRegione;
            }

            #region Private properties
            private string _SiglaProvincia;

            private string _DescrizioneProvincia;

            private string _DescrizioneRegione;
            #endregion Private properties

            #region Public data member
            [DataMember]
            public string SiglaProvincia { get { return _SiglaProvincia; } set { _SiglaProvincia = value; } }
            [DataMember]
            public string DescrizioneProvincia { get { return _DescrizioneProvincia; } set { _DescrizioneProvincia = value; } }
            [DataMember]
            public string DescrizioneRegione { get { return _DescrizioneRegione; } set { _DescrizioneRegione = value; } }
            #endregion Public data member
        }

        [DataContract]
        public class DatiComune
        {
            public DatiComune()
            {
            }

            internal DatiComune(BLCommon.GestioneDecodifica.Comune comune)
            {
                this._CodCatastale = comune.CodCatastale;
                this._Descrizione = comune.Descrizione;
                this._SiglaProvincia = comune.SiglaProvincia;
                this._Cap = comune.Cap;
            }

            #region Private properties
            private string _CodCatastale;

            private string _Descrizione;

            private string _SiglaProvincia;

            private string _Cap;
            #endregion Private properties

            #region Public data member
            [DataMember]
            public string CodCatastale { get { return _CodCatastale; } set { _CodCatastale = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            [DataMember]
            public string SiglaProvincia { get { return _SiglaProvincia; } set { _SiglaProvincia = value; } }
            [DataMember]
            public string Cap { get { return _Cap; } set { _Cap = value; } }
            #endregion Public data member
        }

        [DataContract]
        public class DatiConiugeOFiglio
        {
            public DatiConiugeOFiglio()
            {
            }

            internal DatiConiugeOFiglio(BLCommon.GestioneDecodifica.ConiugeOFiglio coniugeOFiglio)
            {
                this._Id = coniugeOFiglio.Id;
                this._Descrizione = coniugeOFiglio.Descrizione;
            }

            #region Private properties
            private string _Id;

            private string _Descrizione;
            #endregion Private properties

            #region Public data member
            [DataMember]
            public string Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion Public data member
        }

        [DataContract]
        public class DatiDetrazioniReddito
        {
            public DatiDetrazioniReddito()
            {
            }

            internal DatiDetrazioniReddito(BLCommon.GestioneDecodifica.DetrazioniReddito detrazioniReddito)
            {
                this._Id = detrazioniReddito.Id;
                this._Descrizione = detrazioniReddito.Descrizione;
            }

            #region Private properties
            private string _Id;

            private string _Descrizione;
            #endregion Private properties

            #region Public data member
            [DataMember]
            public string Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion Public data member
        }

        [DataContract]
        public class DatiTutore
        {
            public DatiTutore()
            {
            }

            internal DatiTutore(BLCommon.GestioneDecodifica.Tutore tutore)
            {
                this._Id = tutore.Id;
                this._Descrizione = tutore.Descrizione;
            }

            #region Private properties
            private string _Id;

            private string _Descrizione;
            #endregion Private properties

            #region public data member
            [DataMember]
            public string Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiDelegato
        {
            public DatiDelegato()
            {
            }

            internal DatiDelegato(BLCommon.GestioneDecodifica.Delegato delegato)
            {
                this._Id = delegato.Id;
                this._Descrizione = delegato.Descrizione;
            }

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties

            #region public data member
            [DataMember]
            public string Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiModalitaPagamento
        {
            public DatiModalitaPagamento()
            {
            }

            internal DatiModalitaPagamento(BLCommon.GestioneDecodifica.ModalitaPagamento modalitaPagamento)
            {
                this._Id = modalitaPagamento.Id;
                this._Descrizione = modalitaPagamento.Descrizione;
            }

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties

            #region public data member
            [DataMember]
            public string Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiTipoPagamento
        {
            public DatiTipoPagamento()
            {
            }

            internal DatiTipoPagamento(BLCommon.GestioneDecodifica.TipoPagamento tipoPagamento)
            {
                this._Id = tipoPagamento.Id;
                this._Descrizione = tipoPagamento.Descrizione;
            }

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties

            #region public data member
            [DataMember]
            public string Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiTipoCalcolo
        {
            public DatiTipoCalcolo()
            {
            }

            internal DatiTipoCalcolo(BLCommon.GestioneDecodifica.TipoCalcolo tipoCalcolo)
            {
                this._Id = tipoCalcolo.Id;
                this._Descrizione = tipoCalcolo.Descrizione;
                this._TraduzioneSuGP = tipoCalcolo.TraduzioneSuGP;
                this._Tipo = tipoCalcolo.Tipo;
                this._Tipologia = tipoCalcolo.Tipologia;
            }

            #region private properties
            private string _Id;

            private string _Descrizione;

            private System.Nullable<byte> _TraduzioneSuGP;

            private string _Tipo;

            private string _Tipologia;
            #endregion private properties

            #region public data member
            [DataMember]
            public string Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            [DataMember]
            public System.Nullable<byte> TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }
            [DataMember]
            public string Tipo { get { return _Tipo; } set { _Tipo = value; } }
            [DataMember]
            public string Tipologia { get { return _Tipologia; } set { _Tipologia = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiCausaCarico
        {
            public DatiCausaCarico()
            {
            }

            internal DatiCausaCarico(BLCommon.GestioneDecodifica.CausaCarico causaCarico)
            {
                this._Id = causaCarico.Id;
                this._Descrizione = causaCarico.Descrizione;
            }

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties

            #region public data member
            [DataMember]
            public string Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiCodiceCristallizzazione
        {
            public DatiCodiceCristallizzazione()
            {
            }

            internal DatiCodiceCristallizzazione(BLCommon.GestioneDecodifica.CodiceCristallizzazione codiceCristallizzazione)
            {
                this._Id = codiceCristallizzazione.Id;
                this._Descrizione = codiceCristallizzazione.Descrizione;
            }

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties

            #region public data member
            [DataMember]
            public string Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiTipoPensione
        {
            public DatiTipoPensione()
            {
            }

            internal DatiTipoPensione(BLCommon.GestioneDecodifica.TipoPensione tipoPensione)
            {
                this._Id = tipoPensione.Id;
                this._Descrizione = tipoPensione.Descrizione;
            }

            #region private properties
            private char _Id;

            private string _Descrizione;
            #endregion private properties

            #region public data member
            [DataMember]
            public char Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiCodiceAzienda
        {
            public DatiCodiceAzienda()
            {
            }

            internal DatiCodiceAzienda(BLCommon.GestioneDecodifica.CodiceAzienda codiceAzienda)
            {
                this._Id = codiceAzienda.Id;
                this._TraduzioneGp = codiceAzienda.TraduzioneGp;
                this._Descrizione = codiceAzienda.Descrizione != null ? codiceAzienda.Descrizione.Trim() : "";
                this._Fondo = codiceAzienda.Fondo;
            }

            #region public properties
            [DataMember]
            public long Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string TraduzioneGp { get { return _TraduzioneGp; } set { _TraduzioneGp = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            [DataMember]
            public string Fondo { get { return _Fondo; } set { _Fondo = value; } }
            #endregion public properties

            #region private properties
            private long _Id;
            private string _TraduzioneGp;
            private string _Descrizione;
            private string _Fondo;
            #endregion private properties
        }

        [DataContract]
        public class DatiGradoInvalidita
        {
            public DatiGradoInvalidita()
            {
            }

            internal DatiGradoInvalidita(BLCommon.GestioneDecodifica.GradoInvalidita gradoInvalidita)
            {
                this._Id = gradoInvalidita.Id;
                this._Descrizione = gradoInvalidita.Descrizione;
            }

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties

            #region public data member
            [DataMember]
            public string Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiProrataEnel
        {
            public DatiProrataEnel()
            {
            }

            internal DatiProrataEnel(BLCommon.GestioneDecodifica.ProrataEnel prorataEnel)
            {
                this._Id = prorataEnel.Id;
                this._Descrizione = prorataEnel.Descrizione;
            }

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties

            #region public data member
            [DataMember]
            public string Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiComunicazioneCampi1_2
        {
            public DatiComunicazioneCampi1_2()
            {
            }

            internal DatiComunicazioneCampi1_2(BLCommon.GestioneDecodifica.ComunicazioneCampi1_2 comunicazioneCampi1_2)
            {
                this._Campo1 = comunicazioneCampi1_2.Campo1;
                this._Campo2 = comunicazioneCampi1_2.Campo2;
                this._Descrizione = comunicazioneCampi1_2.Descrizione != null ? comunicazioneCampi1_2.Descrizione.Trim() : "";
                this.Tipologia = comunicazioneCampi1_2.Tipologia;
            }

            #region private properties
            private System.Nullable<byte> _Campo1;

            private System.Nullable<char> _Campo2;

            private string _Descrizione;

            private string _Tipologia;
            #endregion private properties

            #region public data member
            [DataMember]
            private System.Nullable<byte> Campo1 { get { return _Campo1; } set { _Campo1 = value; } }
            [DataMember]
            private System.Nullable<char> Campo2 { get { return _Campo2; } set { _Campo2 = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            [DataMember]
            public string Tipologia { get { return _Tipologia; } set { _Tipologia = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiComunicazioneCampo3
        {
            public DatiComunicazioneCampo3()
            {
            }

            internal DatiComunicazioneCampo3(BLCommon.GestioneDecodifica.ComunicazioneCampo3 comunicazioneCampo3)
            {
                this._Id = comunicazioneCampo3.Id;
                this._Descrizione = comunicazioneCampo3.Descrizione;
            }

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties

            #region public data member
            [DataMember]
            public string Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiComunicazioneCampo4
        {
            public DatiComunicazioneCampo4()
            {
            }

            internal DatiComunicazioneCampo4(BLCommon.GestioneDecodifica.ComunicazioneCampo4 comunicazioneCampo4)
            {
                this._Id = comunicazioneCampo4.Id;
                this._Descrizione = comunicazioneCampo4.Descrizione;
            }

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties

            #region public data member
            [DataMember]
            public string Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiCodiciNatura
        {
            public DatiCodiciNatura()
            {
            }

            internal DatiCodiciNatura(BLCommon.GestioneDecodifica.CodiciNatura codiciNatura)
            {
                this._TraduzioneSuGP = codiciNatura.TraduzioneSuGP;
                this._Posizione = codiciNatura.Posizione;
                this._Descrizione = codiciNatura.Descrizione != null ? codiciNatura.Descrizione.Trim() : "";
                this._Tipologia = codiciNatura.Tipologia != null ? codiciNatura.Tipologia.Trim() : "";
                this._Fondo = codiciNatura.Fondo != null ? codiciNatura.Fondo.Trim() : "";
            }

            #region private properties
            private System.Nullable<char> _TraduzioneSuGP;

            private System.Nullable<byte> _Posizione;

            private string _Descrizione;

            private string _Tipologia;

            private string _Fondo;
            #endregion private properties

            #region public data member
            [DataMember]
            public System.Nullable<char> TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }
            [DataMember]
            public System.Nullable<byte> Posizione { get { return _Posizione; } set { _Posizione = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            [DataMember]
            public string Tipologia { get { return _Tipologia; } set { _Tipologia = value; } }
            [DataMember]
            public string Fondo { get { return _Fondo; } set { _Fondo = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiCategoriaPensione
        {
            public DatiCategoriaPensione()
            { }

            internal DatiCategoriaPensione(BLCommon.GestioneDecodifica.CategoriaPensione categoriaPensione)
            {
                _Codice = categoriaPensione.CodCatPensione;
                _Sigla = categoriaPensione.SiglaCatPensione;
                _Tipo = categoriaPensione.TipoCatPensione;
                _Appartenenza = categoriaPensione.AppartenenzaCatPensione;
            }

            #region private properties
            private string _Codice;
            private string _Sigla;
            private char _Tipo;
            private string _Appartenenza;
            #endregion private properties

            #region public data member
            [DataMember]
            public string Codice
            {
                get { return _Codice; }
                set { _Codice = value; }
            }

            [DataMember]
            public string Sigla
            {
                get { return _Sigla; }
                set { _Sigla = value; }
            }

            [DataMember]
            public char Tipo
            {
                get { return _Tipo; }
                set { _Tipo = value; }
            }

            [DataMember]
            public string Appartenenza
            {
                get { return _Appartenenza; }
                set { _Appartenenza = value; }
            }
            #endregion public properties
        }

        [DataContract]
        public class DatiFondoPensione
        {
            public DatiFondoPensione()
            { }

            internal DatiFondoPensione(BLCommon.GestioneDecodifica.FondoPensione fondoPensione)
            {
                _CodFondo = fondoPensione.CodFondo;
                _CodGestione = fondoPensione.CodGestione;
                _DescFondo = fondoPensione.DescFondo;
                _DescGestione = fondoPensione.DescGestione;
            }

            #region private properties
            private string _CodFondo;
            private string _CodGestione;
            private string _DescFondo;
            private string _DescGestione;
            #endregion private properties

            #region public data member
            [DataMember]
            public string CodFondo
            {
                get { return _CodFondo; }
                set { _CodFondo = value; }
            }

            [DataMember]
            public string CodGestione
            {
                get { return _CodGestione; }
                set { _CodGestione = value; }
            }

            [DataMember]
            public string DescFondo
            {
                get { return _DescFondo; }
                set { _DescFondo = value; }
            }

            [DataMember]
            public string DescGestione
            {
                get { return _DescGestione; }
                set { _DescGestione = value; }
            }
            #endregion data member
        }

        public class DatiStatoPensione
        {
            public DatiStatoPensione()
            { }

            internal DatiStatoPensione(BLCommon.GestioneDecodifica.StatoPensione statoPensione)
            {
                _DecodificaStato = statoPensione.DecodificaStato;
                _CodiceStato = statoPensione.CodiceStato;
            }

            #region private properties
            private string _DecodificaStato;
            private string _CodiceStato;
            #endregion private properties


            #region public data member
            [DataMember]
            public string DecodificaStato
            {
                get { return _DecodificaStato; }
                set { _DecodificaStato = value; }
            }
            [DataMember]
            public string CodiceStato
            {
                get { return _CodiceStato; }
                set { _CodiceStato = value; }
            }
            #endregion public data member
        }

        [DataContract]
        public class DatiParentelaDC
        {
            public DatiParentelaDC()
            {
            }

            internal DatiParentelaDC(BLCommon.GestioneDecodifica.ParentelaDC ParentelaDC)
            {
                this._Id = ParentelaDC.Id;
                this._Descrizione = ParentelaDC.Descrizione;
            }

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties

            #region public data member
            [DataMember]
            public string Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiCodiciProvenienza
        {
            public DatiCodiciProvenienza()
            {
            }

            internal DatiCodiciProvenienza(BLCommon.GestioneDecodifica.CodiceProvenienza Provenienza)
            {
                this._Id = Provenienza.Id;
                this._Descrizione = Provenienza.Descrizione;
            }

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties

            #region public data member
            [DataMember]
            public string Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiCodiciImportoAltraPensione
        {
            public DatiCodiciImportoAltraPensione()
            {
            }

            internal DatiCodiciImportoAltraPensione(BLCommon.GestioneDecodifica.ImportoAltraPensione importoAltraPensione)
            {
                this._Id = importoAltraPensione.Id;
                this._Descrizione = importoAltraPensione.Descrizione;
            }

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties

            #region public data member
            [DataMember]
            public string Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiCodiciVari
        {
            public DatiCodiciVari()
            {
            }

            internal DatiCodiciVari(BLCommon.GestioneDecodifica.CodiciVari codicivari)
            {
                this._Id = codicivari.Id;
                this._Descrizione = codicivari.Descrizione;
            }

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties

            #region public data member
            [DataMember]
            public string Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiCodeGestioneCalcoloRetrib
        {
            public DatiCodeGestioneCalcoloRetrib(BLCommon.GestioneDecodifica.CodeGestioneCalcoloRetributivo codeGestioneCalcoloRetributivo)
            {
                this._Id = codeGestioneCalcoloRetributivo.Id;
                this._Descrizione = codeGestioneCalcoloRetributivo.Descrizione;
                this._TraduzioneSuGP = codeGestioneCalcoloRetributivo.TraduzioneSuGP;
                this._IsFondo = codeGestioneCalcoloRetributivo.IsFondo;
            }

            #region private properties

            private long _Id;
            private string _Descrizione;
            private string _TraduzioneSuGP;
            private bool _IsFondo;

            #endregion private properties

            #region public properties

            [DataMember]
            public long Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            [DataMember]
            public string TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }
            [DataMember]
            public bool IsFondo { get { return _IsFondo; } set { _IsFondo = value; } }

            #endregion public properties
        }

        [DataContract]
        public class DatiCodeGestioneCalcoloContrib
        {
            public DatiCodeGestioneCalcoloContrib(BLCommon.GestioneDecodifica.CodeGestioneCalcoloContributivo codeGestioneCalcoloContributivo)
            {
                this._Id = codeGestioneCalcoloContributivo.Id;
                this._Descrizione = codeGestioneCalcoloContributivo.Descrizione;
                this._TraduzioneSuGP = codeGestioneCalcoloContributivo.TraduzioneSuGP;
                this._IsFondo = codeGestioneCalcoloContributivo.IsFondo;
            }

            #region private properties

            private long _Id;
            private string _Descrizione;
            private string _TraduzioneSuGP;
            private bool _IsFondo;

            #endregion private properties

            #region public properties

            [DataMember]
            public long Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            [DataMember]
            public string TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }
            [DataMember]
            public bool IsFondo { get { return _IsFondo; } set { _IsFondo = value; } }

            #endregion public properties
        }

        [DataContract]
        public class CodeGestione
        {
            public CodeGestione(BLCommon.GestioneDecodifica.CodeGestione codeGestione)
            {
                this._Id = codeGestione.Id;
                this._Descrizione = codeGestione.Descrizione;
                this._TraduzioneSuGP = codeGestione.TraduzioneSuGP;
                this._Legge = codeGestione.Legge;
            }

            #region private properties

            private long _Id;
            private string _Descrizione;
            private short? _TraduzioneSuGP;
            private string _Legge;

            #endregion private properties

            #region public properties

            [DataMember]
            public long Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            [DataMember]
            public short? TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }
            [DataMember]
            public string Legge { get { return _Legge; } set { _Legge = value; } }

            #endregion public properties
        }

        [DataContract]
        public class DatiRicercaGPT
        {
            public DatiRicercaGPT(BLCommon.GestioneDecodifica.CtrlRicercaGPT ricerca)
            {
                this._Codice = ricerca.Codice;
                this._GPT = ricerca.GPT;
            }

            #region private properties
            private string _Codice { get; set; }
            private char _GPT { get; set; }

            #endregion private properties

            #region public properties
            [DataMember]
            public string Codice { get { return _Codice; } set { _Codice = value; } }
            [DataMember]
            public char GPT { get { return _GPT; } set { _GPT = value; } }
            #endregion public properties
        }

        [DataContract]
        public class DatiCategoriaAltraPensione
        {
            public DatiCategoriaAltraPensione()
            { }

            internal DatiCategoriaAltraPensione(BLCommon.GestioneDecodifica.CatEnteAltraPensione catAltraPensione)
            {
                _CodCategoria = catAltraPensione.CodCategoria;
                _CodEnte = catAltraPensione.CodEnte;
                _Appartenenza = catAltraPensione.TipoApp;
            }

            #region private properties
            private string _CodCategoria;
            private char _CodEnte;
            private string _Appartenenza;
            #endregion private properties

            #region public data member
            [DataMember]
            public string CodCategoria
            {
                get { return _CodCategoria; }
                set { _CodCategoria = value; }
            }

            [DataMember]
            public char CodEnte
            {
                get { return _CodEnte; }
                set { _CodEnte = value; }
            }

            [DataMember]
            public string Appartenenza
            {
                get { return _Appartenenza; }
                set { _Appartenenza = value; }
            }
            #endregion public properties
        }

        [DataContract]
        public class DatiCtrlEnteCassaCodiceGestione
        {
            public DatiCtrlEnteCassaCodiceGestione()
            { }

            internal DatiCtrlEnteCassaCodiceGestione(BLCommon.GestioneDecodifica.CtrlEnteCassaCodiceGestione ctrlEnteCassaCodiceGestione)
            {
                _CodiceCategoria = ctrlEnteCassaCodiceGestione.CodiceCategoria;
                _TraduzioneSuGP = ctrlEnteCassaCodiceGestione.CodiciGestione;
                _TraduzioneSuGP = ctrlEnteCassaCodiceGestione.TraduzioneSuGP;
                _Professione = ctrlEnteCassaCodiceGestione.Professione;
            }

            #region private properties
            private string _CodiceCategoria { get; set; }
            private string _TraduzioneSuGP { get; set; }
            private string _CodiciGestione { get; set; }
            private string _Professione { get; set; }
            #endregion private properties

            #region public data member
            [DataMember]
            public string CodiceCategoria
            {
                get { return _CodiceCategoria; }
                set { _CodiceCategoria = value; }
            }

            [DataMember]
            public string TraduzioneSuGP
            {
                get { return _TraduzioneSuGP; }
                set { _TraduzioneSuGP = value; }
            }

            [DataMember]
            public string CodiciGestione
            {
                get { return _CodiciGestione; }
                set { _CodiciGestione = value; }
            }

            [DataMember]
            public string Professione
            {
                get { return _Professione; }
                set { _Professione = value; }
            }
            #endregion public properties
        }

        [DataContract]
        public class DatiCtrlCatAdeguata
        {
            public DatiCtrlCatAdeguata()
            { }

            internal DatiCtrlCatAdeguata(BLCommon.GestioneDecodifica.CtrlCatAdeguata ctrlCatAdeguata)
            {
                _CodCategoria = ctrlCatAdeguata.CodCategoria;
                _CodGruppo = ctrlCatAdeguata.CodGruppo;
                _CodProdotto = ctrlCatAdeguata.CodTipo;
                _CodTipo = ctrlCatAdeguata.CodTipo;
                _IsTrasfRic = ctrlCatAdeguata.IsTrasfRic;
                _DataInizio = ctrlCatAdeguata.DataInizio;
                _DataFine = ctrlCatAdeguata.DataFine;
            }

            #region private properties
            private string _CodCategoria;
            private string _CodGruppo;
            private string _CodProdotto;
            private string _CodTipo;
            public bool? _IsTrasfRic;
            private DateTime? _DataInizio;
            private DateTime? _DataFine;
            #endregion private properties

            #region public data member
            [DataMember]
            public string CodCategoria
            {
                get { return _CodCategoria; }
                set { _CodCategoria = value; }
            }
            [DataMember]
            public string CodGruppo
            {
                get { return _CodGruppo; }
                set { _CodGruppo = value; }
            }
            [DataMember]
            public string CodProdotto
            {
                get { return _CodProdotto; }
                set { _CodProdotto = value; }
            }
            [DataMember]
            public string CodTipo
            {
                get { return _CodTipo; }
                set { _CodTipo = value; }
            }
            [DataMember]
            public bool? IsTrasfRic
            {
                get { return _IsTrasfRic; }
                set { _IsTrasfRic = value; }
            }
            [DataMember]
            public DateTime? DataInizio
            {
                get { return _DataInizio; }
                set { _DataInizio = value; }
            }
            [DataMember]
            public DateTime? DataFine
            {
                get { return _DataFine; }
                set { _DataFine = value; }
            }
            #endregion public properties
        }

        [DataContract]
        public class DatiDecComparto
        {
            public DatiDecComparto()
            { }

            internal DatiDecComparto(BLCommon.GestioneDecodifica.DecComparto decComparto)
            {
                _Codice = decComparto.Codice;
                _Descrizione = decComparto.Descrizione;
            }

            #region private properties
            private int _Codice;
            private string _Descrizione;
            #endregion private properties

            #region public data member
            [DataMember]
            public int Codice
            {
                get { return _Codice; }
                set { _Codice = value; }
            }
            [DataMember]
            public string Descrizione
            {
                get { return _Descrizione; }
                set { _Descrizione = value; }
            }
            #endregion public properties
        }

        [DataContract]
        public class DatiDecSettore
        {
            public DatiDecSettore()
            { }

            internal DatiDecSettore(BLCommon.GestioneDecodifica.DecSettore decSettore)
            {
                _Codice = decSettore.Codice;
                _Descrizione = decSettore.Descrizione;
            }

            #region private properties
            private int _Codice;
            private string _Descrizione;
            #endregion private properties

            #region public data member
            [DataMember]
            public int Codice
            {
                get { return _Codice; }
                set { _Codice = value; }
            }
            [DataMember]
            public string Descrizione
            {
                get { return _Descrizione; }
                set { _Descrizione = value; }
            }
            #endregion public properties
        }

        [DataContract]
        public class DatiDecRuolo
        {
            public DatiDecRuolo()
            { }

            internal DatiDecRuolo(BLCommon.GestioneDecodifica.DecRuolo decRuolo)
            {
                _Codice = decRuolo.Codice;
                _Descrizione = decRuolo.Descrizione;
            }

            #region private properties
            private int _Codice;
            private string _Descrizione;
            #endregion private properties

            #region public data member
            [DataMember]
            public int Codice
            {
                get { return _Codice; }
                set { _Codice = value; }
            }
            [DataMember]
            public string Descrizione
            {
                get { return _Descrizione; }
                set { _Descrizione = value; }
            }
            #endregion public properties
        }


        [DataContract]
        public class DatiDecSede
        {
            public DatiDecSede() { }

            internal DatiDecSede(BLCommon.GestioneDecodifica.DecSede decSede)
            {
                _CodiceSedeMeta = decSede.CodiceSedeMeta;
                _DescSede = decSede.DescSede;
                _SiglaProvincia = decSede.SiglaProvincia;
                _NomeDirettore = decSede.NomeDirettore;
                _CodProvincia = decSede.CodProvincia;
                _CodZona = decSede.CodZona;
                _CodCentroOperativo = decSede.CodCentroOperativo;
                _DataUltimaModifica = decSede.DataUltimaModifica;
                _CodTipoSede = decSede.CodTipoSede;
                _CAPSede = decSede.CAPSede;
                _IndirizzoSede = decSede.IndirizzoSede;
                _ComuneSede = decSede.ComuneSede;
                _ProvinciaSede = decSede.ProvinciaSede;
                _IndirizzoEMail = decSede.IndirizzoEMail;
                _Codice6 = decSede.Codice6;
                _CodAttivitaSede = decSede.CodAttivitaSede;
            }

            #region private properties
            private string _CodiceSedeMeta { get; set; }
            private string _DescSede { get; set; }
            private string _SiglaProvincia { get; set; }
            private string _NomeDirettore { get; set; }
            private string _CodProvincia { get; set; }
            private string _CodZona { get; set; }
            private string _CodCentroOperativo { get; set; }
            private string _DataUltimaModifica { get; set; }
            private string _CodTipoSede { get; set; }
            private string _CAPSede { get; set; }
            private string _IndirizzoSede { get; set; }
            private string _ComuneSede { get; set; }
            private string _ProvinciaSede { get; set; }
            private string _IndirizzoEMail { get; set; }
            private string _Codice6 { get; set; }
            private char? _CodAttivitaSede { get; set; }
            #endregion private properties

            #region public data member
            [DataMember]
            public string CodiceSedeMeta
            {
                get { return _CodiceSedeMeta; }
                set { _CodiceSedeMeta = value; }
            }

            [DataMember]
            public string DescSede
            {
                get { return _DescSede; }
                set { _DescSede = value; }
            }

            [DataMember]
            public string SiglaProvincia
            {
                get { return _SiglaProvincia; }
                set { _SiglaProvincia = value; }
            }

            [DataMember]
            public string NomeDirettore
            {
                get { return _NomeDirettore; }
                set { _NomeDirettore = value; }
            }

            [DataMember]
            public string CodProvincia
            {
                get { return _CodProvincia; }
                set { _CodProvincia = value; }
            }

            [DataMember]
            public string CodZona
            {
                get { return _CodZona; }
                set { _CodZona = value; }
            }

            [DataMember]
            public string CodCentroOperativo
            {
                get { return _CodCentroOperativo; }
                set { _CodCentroOperativo = value; }
            }

            [DataMember]
            public string DataUltimaModifica
            {
                get { return _DataUltimaModifica; }
                set { _DataUltimaModifica = value; }
            }

            [DataMember]
            public string CodTipoSede
            {
                get { return _CodTipoSede; }
                set { _CodTipoSede = value; }
            }

            [DataMember]
            public string CAPSede
            {
                get { return _CAPSede; }
                set { _CAPSede = value; }
            }

            [DataMember]
            public string IndirizzoSede
            {
                get { return _IndirizzoSede; }
                set { _IndirizzoSede = value; }
            }

            [DataMember]
            public string ComuneSede
            {
                get { return _ComuneSede; }
                set { _ComuneSede = value; }
            }

            [DataMember]
            public string ProvinciaSede
            {
                get { return _ProvinciaSede; }
                set { _ProvinciaSede = value; }
            }

            [DataMember]
            public string IndirizzoEMail
            {
                get { return _IndirizzoEMail; }
                set { _IndirizzoEMail = value; }
            }

            [DataMember]
            public string Codice6
            {
                get { return _Codice6; }
                set { _Codice6 = value; }
            }

            [DataMember]
            public char? CodAttivitaSede
            {
                get { return _CodAttivitaSede; }
                set { _CodAttivitaSede = value; }
            }

            #endregion public properties
        }
        #endregion nested class
    }
}
