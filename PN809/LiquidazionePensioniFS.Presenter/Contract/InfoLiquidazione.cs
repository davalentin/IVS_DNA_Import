using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.Contract
{
    [Serializable]
    public class InfoLiquidazione
    {
        #region private properties
        private String _strDomanda;
        private String _strCategoria;
        private String _strTipo;
        private String _strSede;
        private String _strCertificato;
        private String _strCodiceFiscale;
        private String _strNome;
        private String _strCognome;
        private String _strStatoDomanda;
        #endregion private properties

        #region public properties
        public String Domanda { get { return _strDomanda; } set { _strDomanda = value; } }
        public String Categoria { get { return _strCategoria; } set { _strCategoria = value; } }
        public String Tipo { get { return _strTipo; } set { _strTipo = value; } }
        public String Sede { get { return _strSede; } set { _strSede = value; } }
        public String Certificato { get { return _strCertificato; } set { _strCertificato = value; } }
        public String CodiceFiscale { get { return _strCodiceFiscale; } set { _strCodiceFiscale = value; } }
        public String Nome { get { return _strNome; } set { _strNome = value; } }
        public String Cognome { get { return _strCognome; } set { _strCognome = value; } }
        public String StatoDomanda { get { return _strStatoDomanda; } set { _strStatoDomanda = value; } }
        #endregion public properties
    }
}
