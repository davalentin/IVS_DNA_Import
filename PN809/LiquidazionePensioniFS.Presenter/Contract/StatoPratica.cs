using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.Contract
{
    [Serializable]
    public class StatoPratica
    {
        #region private properties
        private string _strNumeroDomanda;
        private string _strCategoriaPensione;
        private short _shortStatoPratica;
        private string _strPeriodoGiacenza;
        private string _strFondo;
        private string _strCassa;
        private string _strSede;
        private string _strCognome;
        private string _strNome;
        private string _strCodiceFiscale;
        private string _strDataPresentazioneMin;
        private string _strDataPresentazioneMax;
        private string _strDataElaborazioneMin;
        private string _strDataElaborazioneMax;
        private string _strMatricola;
        private string _strErrore;
        private GestioneStatoPraticaTipoDomanda _TipoDomandaInLavorazione;
        private GestioneStatoPraticaTipoDomanda _TipoDomandaLavorata;
        private string _strGruppo;
        private string _strProdotto;
        private string _strTipo;
        private Utility.CriterioRicercaStatoPratica _Criterio;
        #endregion private properties

        #region public properties
        public string NumeroDomanda { get { return _strNumeroDomanda; } set { _strNumeroDomanda = value; } }
        public string CategoriaPensione { get { return _strCategoriaPensione; } set { _strCategoriaPensione = value; } }
        public short SPratica { get { return _shortStatoPratica; } set { _shortStatoPratica = value; } }
        public string PeriodoGiacenza { get { return _strPeriodoGiacenza; } set { _strPeriodoGiacenza = value; } }
        public string Fondo { get { return _strFondo; } set { _strFondo = value; } }
        public string Cassa { get { return _strCassa; } set { _strCassa = value; } }
        public string Sede { get { return _strSede; } set { _strSede = value; } }
        public string Cognome { get { return _strCognome; } set { _strCognome = value; } }
        public string Nome { get { return _strNome;} set {_strNome = value;}}
        public string CodiceFiscale { get { return _strCodiceFiscale;} set {_strCodiceFiscale= value;}}        
        public string DataPresentazioneMin  { get { return _strDataPresentazioneMin; } set { _strDataPresentazioneMin = value; } }
        public string DataPresentazioneMax { get { return _strDataPresentazioneMax; } set { _strDataPresentazioneMax = value; } }
        public string DataElaborazioneMin  { get { return _strDataElaborazioneMin; } set { _strDataElaborazioneMin = value; } }
        public string DataElaborazioneMax { get { return _strDataElaborazioneMax; } set { _strDataElaborazioneMax = value; } }
        public string Matricola { get { return _strMatricola; } set { _strMatricola = value; } }
        public GestioneStatoPraticaTipoDomanda TipoDomandaInLavorazione { get { return _TipoDomandaInLavorazione; } set { _TipoDomandaInLavorazione = value; } }
        public GestioneStatoPraticaTipoDomanda TipoDomandaLavorata { get { return _TipoDomandaLavorata; } set { _TipoDomandaLavorata = value; } }
        public string Gruppo { get { return _strGruppo; } set { _strGruppo = value; } }
        public string Prodotto { get { return _strProdotto; } set { _strProdotto = value; } }
        public string Tipo { get { return _strTipo; } set { _strTipo = value; } }
        public Utility.CriterioRicercaStatoPratica  Criterio { get { return _Criterio; } set { _Criterio = value; } }
        public string Errore { get { return _strErrore; } set { _strErrore = value; } }
        #endregion public properties
    }
}
