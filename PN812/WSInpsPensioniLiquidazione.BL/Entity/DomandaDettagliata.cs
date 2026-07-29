using System;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class DomandaDettagliata : Domanda
	{
		#region private variables
		private string _Nome;
		private string _Cognome;
		private string _CodiceFiscale;
		private string _Fondo;
		private DateTime _DataPresentazioneDomanda;
		private DateTime _DataElaborazioneDomanda;
        private Utility.TipoAppartenenza? _TipoAppartenenza;
        private Utility.TipoFondo? _TipoFondo;
        private string _Matricola;
		#endregion private variables

		#region Properties
		public string Cognome
		{
			get { return _Cognome; }
			set { _Cognome = value; }
		}

		public string CodiceFiscale
		{
			get { return _CodiceFiscale; }
			set { _CodiceFiscale = value; }
		}

		public string Fondo
		{
			get { return _Fondo; }
			set { _Fondo = value; }
		}

		public DateTime DataPresentazioneDomanda
		{
			get { return _DataPresentazioneDomanda; }
			set { _DataPresentazioneDomanda = value; }
		}

		public DateTime DataElaborazioneDomanda
		{
			get { return _DataElaborazioneDomanda; }
			set { _DataElaborazioneDomanda = value; }
		}

		public string Nome
		{
			get { return _Nome; }
			set { _Nome = value; }
		}

        public Utility.TipoAppartenenza? TipoAppartenenza
        {
            get { return _TipoAppartenenza; }
            set { _TipoAppartenenza = value; }
        }

        public Utility.TipoFondo? TipoFondo
        {
            get { return _TipoFondo; }
            set { _TipoFondo = value; }
        }

        public string Matricola
        {
            get { return _Matricola; }
            set { _Matricola = value; }
        }

		#endregion Properties
	}
}
