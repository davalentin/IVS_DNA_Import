using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.BLCommon.Entity
{
    public class AreaTitolare
    {
        public AreaTitolare()
        {
            _ElencoStatiCivili = new List<GestioneAnagrafica.DatiStatoCivile>();
            _ElencoResidenzeEstere = new List<GestioneAnagrafica.DatiResidenzaEstero>();
            _Anagrafica = new GestioneAnagrafica.DatiAnagrafici();
            _Patronato = new GestionePensione.DatiPatronato();
            _Sindacato = new GestionePensione.DatiSindacato();
            _Pensione = new GestionePensione.DatiPensione();
        }

        #region private properties
        private List<GestioneAnagrafica.DatiStatoCivile> _ElencoStatiCivili;

        private List<GestioneAnagrafica.DatiResidenzaEstero> _ElencoResidenzeEstere;

        private GestioneAnagrafica.DatiAnagrafici _Anagrafica;

        private GestionePensione.DatiPatronato _Patronato;

        private GestionePensione.DatiSindacato _Sindacato;

        private GestionePensione.DatiPensione _Pensione;

        #endregion private properties

        #region public properties
        public List<GestioneAnagrafica.DatiStatoCivile> ElencoStatiCivili { get { return _ElencoStatiCivili; } set { _ElencoStatiCivili = value; } }

        public List<GestioneAnagrafica.DatiResidenzaEstero> ElencoResidenzeEstere { get { return _ElencoResidenzeEstere; } set { _ElencoResidenzeEstere = value; } }

        public GestioneAnagrafica.DatiAnagrafici Anagrafica { get { return _Anagrafica; } set { _Anagrafica = value; } }

        public GestionePensione.DatiPatronato Patronato { get { return _Patronato; } set { _Patronato = value; } }

        public GestionePensione.DatiSindacato Sindacato { get { return _Sindacato; } set { _Sindacato = value; } }

        public GestionePensione.DatiPensione Pensione { get { return _Pensione; } set { _Pensione = value; } }

        #endregion public properties
    }
}
