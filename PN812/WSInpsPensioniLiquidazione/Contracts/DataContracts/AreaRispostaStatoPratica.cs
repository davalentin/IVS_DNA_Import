using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
	public class AreaRispostaStatoPratica : AreaRispostaRiepilogo
	{
		#region private properties
		private List<DatiStatoPratica> _ElencoDatiStatoPratica;
        #endregion private properties

        [DataMember]
		public List<DatiStatoPratica> ElencoDatiStatoPratica
		{
			get { return _ElencoDatiStatoPratica; }
			set { _ElencoDatiStatoPratica = value; }
		}

		[DataContract]
		public class DatiStatoPratica : AreaRispostaRiepilogo.DatiRiepilogoDomanda
		{
			public DatiStatoPratica()
			{ }

			internal DatiStatoPratica(Entity.DomandaDettagliata domandaDettagliata)
				: base(domandaDettagliata, null, null, string.Empty, false, false, false)
			{
				this._Cognome = domandaDettagliata.Cognome;
				this._Nome = domandaDettagliata.Nome;
				this._CodiceFiscale = domandaDettagliata.CodiceFiscale;
				this._Fondo = domandaDettagliata.Fondo;
				this._DataPresentazioneDomanda = domandaDettagliata.DataPresentazioneDomanda;
				this._DataElaborazioneDomanda = domandaDettagliata.DataElaborazioneDomanda;
                this._Matricola = domandaDettagliata.Matricola;
                //this._DescProdotto = domandaDettagliata.DescProdotto;
                //this._DescTipo = domandaDettagliata.DescTipo;
                

                if (domandaDettagliata.TipoAppartenenza != null)
                {
                    switch (domandaDettagliata.TipoAppartenenza)
                    {
                        case Utility.TipoAppartenenza.AGO:
                            this._TipoAppartenenza = TipoApp.AGO;
                            this._Tipofondo = null;
                            break;
                        case Utility.TipoAppartenenza.FS:
                            this._TipoAppartenenza = TipoApp.FS;
                            if (domandaDettagliata.TipoFondo.HasValue)
                            {
                                this._Tipofondo = (DatiRiepilogoDomanda.TipoFondo)domandaDettagliata.TipoFondo.Value;
                            }
                            else
                                this._Tipofondo = null;
                            break;
                        case Utility.TipoAppartenenza.CI:
                            this._TipoAppartenenza = TipoApp.CI;
                            this._Tipofondo = null;
                            break;
                        default:
                            this._TipoAppartenenza = null;
                            this._Tipofondo = null;
                            break;
                    }
                }
                else
                {
                    this._TipoAppartenenza = null;
                    this._Tipofondo = null;
                }
			}

			#region private properties
			private string _Cognome;
			private string _Nome;
			private string _CodiceFiscale;
			private string _Fondo;
			private DateTime _DataPresentazioneDomanda;
			private DateTime _DataElaborazioneDomanda;
            private string _Matricola;
            //private string _DescProdotto;
            //private string _DescTipo;
            #endregion private properties

            #region public data member
            [DataMember]
			public string Cognome
			{
				get { return _Cognome; }
				set { _Cognome = value; }
			}

			[DataMember]
			public string Nome
			{
				get { return _Nome; }
				set { _Nome = value; }
			}

			[DataMember]
			public string CodiceFiscale
			{
				get { return _CodiceFiscale; }
				set { _CodiceFiscale = value; }
			}

			[DataMember]
			public string Fondo
			{
				get { return _Fondo; }
				set { _Fondo = value; }
			}

			[DataMember]
			public DateTime DataPresentazioneDomanda
			{
				get { return _DataPresentazioneDomanda; }
				set { _DataPresentazioneDomanda = value; }
			}

			[DataMember]
			public DateTime DataElaborazioneDomanda
			{
				get { return _DataElaborazioneDomanda; }
				set { _DataElaborazioneDomanda = value; }
			}

            [DataMember]
            public string Matricola
            {
                get { return _Matricola; }
                set { _Matricola = value; }
            }

            //[DataMember]
            //public string DescProdotto
            //{
            //    get { return _DescProdotto; }
            //    set { _DescProdotto = value; }
            //}

            //[DataMember]
            //public string DescTipo
            //{
            //    get { return _DescTipo; }
            //    set { _DescTipo = value; }
            //}
            #endregion public data member
        }

    }
}
