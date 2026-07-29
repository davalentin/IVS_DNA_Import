using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;


namespace INPS.Pensioni.LiquidazionePensione.Presenter.Contract
{
    [Serializable]
    public class RicercaPosizione
    {
        #region private properties
        private string _strDomanda;
        private string _strProgStorico;
        private string _strCodiceFiscale;
        private string _strNome;
        private string _strCognome;
        private string _dtDataNascita;
        private Utility.TipoRicerca _intSelezione;
        private string _strErrore;
        #endregion private properties

        #region public properties
        public string Domanda { get { return _strDomanda; } set { _strDomanda = value; } }
        public string ProgStorico { get { return _strProgStorico; } set { _strProgStorico = value; } }
        public string CodiceFiscale { get { return _strCodiceFiscale;} set {_strCodiceFiscale= value;}}
        public string Nome { get { return _strNome;} set {_strNome = value;}}
        public string Cognome { get { return _strCognome; } set { _strCognome = value; } }
        public string DataNascita { get { return _dtDataNascita;} set {_dtDataNascita = value;}}
        public Utility.TipoRicerca Selezione { get { return _intSelezione; } set { _intSelezione = value; } }
        public string Errore { get { return _strErrore; } set { _strErrore = value; } }
        #endregion public properties
    }
}
