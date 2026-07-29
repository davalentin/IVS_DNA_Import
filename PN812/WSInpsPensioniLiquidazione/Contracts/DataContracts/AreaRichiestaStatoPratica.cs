using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
	public class AreaRichiestaStatoPratica : AreaRichiestaRiepilogo
	{
		public AreaRichiestaStatoPratica()
		{
			_TipoRecupero = TipoRicerca.StatoPratica;
			_Certificato = null;
			_StatoPensione = null;
		}

		#region private variables
		private string _Categoria;
		private string _Tipo;
		private string _Fondo;
		private string _Cassa;
		private string _Sede;
		private short? _StatoPensione;
		private int? _Certificato;
		private DateTime _DataPresentazioneDomandaMin;
		private DateTime _DataElaborazioneDomandaMin;
		private DateTime _DataPresentazioneDomandaMax;
		private DateTime _DataElaborazioneDomandaMax;
        private string _Matricola;
        private GestioneStatoPratica.TipoDomanda _TipoDomandaInLavorazione;
        private GestioneStatoPratica.TipoDomanda _TipoDomandaLavorata;
        private string _Gruppo;
        private string _Prodotto;
		#endregion private variables


		[DataMember]
		public string Categoria
		{
			get { return _Categoria; }
			set { _Categoria = value; }
		}

		[DataMember]
		public string Tipo
		{
			get { return _Tipo; }
			set { _Tipo = value; }
		}

		[DataMember]
		public string Fondo
		{
			get { return _Fondo; }
			set { _Fondo = value; }
		}

		[DataMember]
		public string Cassa
		{
			get { return _Cassa; }
			set { _Cassa = value; }
		}

		[DataMember]
		public string Sede
		{
			get { return _Sede; }
			set { _Sede = value; }
		}

		[DataMember]
		public short? StatoPensione
		{
			get { return _StatoPensione; }
			set { _StatoPensione = value; }
		}

		[DataMember]
		public int? Certificato
		{
			get { return _Certificato; }
			set { _Certificato = value; }
		}

		[DataMember]
		public DateTime DataPresentazioneDomandaMin
		{
			get { return _DataPresentazioneDomandaMin; }
			set { _DataPresentazioneDomandaMin = value; }
		}

		[DataMember]
		public DateTime DataElaborazioneDomandaMin
		{
			get { return _DataElaborazioneDomandaMin; }
			set { _DataElaborazioneDomandaMin = value; }
		}

		[DataMember]
		public DateTime DataPresentazioneDomandaMax
		{
			get { return _DataPresentazioneDomandaMax; }
			set { _DataPresentazioneDomandaMax = value; }
		}

		[DataMember]
		public DateTime DataElaborazioneDomandaMax
		{
			get { return _DataElaborazioneDomandaMax; }
			set { _DataElaborazioneDomandaMax = value; }
		}

        [DataMember]
        public string Matricola
        {
            get { return _Matricola; }
            set { _Matricola = value; }
        }

        [DataMember]
        public GestioneStatoPratica.TipoDomanda TipoDomandaInLavorazione
        {
            get { return _TipoDomandaInLavorazione; }
            set { _TipoDomandaInLavorazione = value; }
        }

        [DataMember]
        public GestioneStatoPratica.TipoDomanda TipoDomandaLavorata
        {
            get { return _TipoDomandaLavorata; }
            set { _TipoDomandaLavorata = value; }
        }

        [DataMember]
        public string Gruppo
        {
            get { return _Gruppo; }
            set { _Gruppo = value; }
        }

        [DataMember]
        public string Prodotto
        {
            get { return _Prodotto; }
            set { _Prodotto = value; }
        }
	}
}
