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
    public class AreaDetrazioni
    {
        public AreaDetrazioni()
        {

        }
        public void ValorizzaArea(GestioneDetrazioni.RispostaDetrazioni detrazioniBL)
        {
            this._Detrazioni = new DatiDetrazioni();
            BLCommon.Utility.ValorizzaOggetti(detrazioniBL.Detrazioni, this._Detrazioni);

            switch (detrazioniBL.Esito)
            { 
                case GestioneDetrazioni.TipoRitornoDetrazioni.Errore:
                    this._EsitoDetrazioni = RitornoDetrazioni.Errore;
                    break;
                case GestioneDetrazioni.TipoRitornoDetrazioni.Informativa:
                    this._EsitoDetrazioni = RitornoDetrazioni.Informativa;
                    break;
                case GestioneDetrazioni.TipoRitornoDetrazioni.NessunErrore:
                    this._EsitoDetrazioni = RitornoDetrazioni.NessunErrore;
                    break;                
            }

            this._Messaggio = detrazioniBL.MessaggioRitorno;
            this._Url = detrazioniBL.Url;
        }

        #region private properties
        private DatiDetrazioni _Detrazioni;

        private string _Url;

        private RitornoDetrazioni _EsitoDetrazioni;

        private string _Messaggio;
        #endregion private properties

        #region public data member

        [DataMember]
        public DatiDetrazioni Detrazioni { get { return _Detrazioni; } set { _Detrazioni = value; } }

        [DataMember]
        public string Url { get { return _Url; } set { _Url = value; } }

        [DataMember]
        public RitornoDetrazioni EsitoDetrazioni { get { return _EsitoDetrazioni; } set { _EsitoDetrazioni = value; } }

        [DataMember]
        public string Messaggio { get { return _Messaggio; } set { _Messaggio = value; } }

        [DataMember]
        public AreaInput DatiInput { get; set; }

        [DataMember]
        public List<GestioneDetrazioni.Soggetto> ElencoSoggetti { get; set; }

        [DataMember]
        public bool IsVariazioneDetrazioni { get; set; }
        #endregion public data member

        #region nested class

        [DataContract]
        public class DatiDetrazioni
        {
            #region private properties

            private System.Nullable<byte> _DetrazioniReddito;

            private System.Nullable<byte> _AgevolazionePensionati;

            private System.Nullable<byte> _ConiugeOFiglio;

            private System.Nullable<byte> _FigliMinori3AnniNoHandicap100;

            private System.Nullable<byte> _FigliMinori3AnniNoHandicap50;

            private System.Nullable<byte> _FigliMinori3AnniHandicap100;

            private System.Nullable<byte> _FigliMinori3AnniHandicap50;

            private System.Nullable<byte> _FigliMaggiori3AnniNoHandicap100;

            private System.Nullable<byte> _FigliMaggiori3AnniNoHandicap50;

            private System.Nullable<byte> _FigliMaggiori3AnniHandicap100;

            private System.Nullable<byte> _FigliMaggiori3AnniHandicap50;

            private System.Nullable<byte> _AltriFamiliari100;

            private System.Nullable<byte> _AltriFamiliari50;

            private System.Nullable<byte> _AddizionaleLombardiaVeneto;

            private System.Nullable<byte> _NonResidenteSchumacker;

            private System.Nullable<byte> _ConvDoppieImposizioni;

            private System.Nullable<System.DateTime> _DecorrenzaDetrazioneImposte;

            private bool _IsStorico;

            #endregion private properties

            #region public data member
            [DataMember]
            public System.Nullable<byte> DetrazioniReddito { get { return _DetrazioniReddito; } set { _DetrazioniReddito = value; } }
            [DataMember]
            public System.Nullable<byte> AgevolazionePensionati { get { return _AgevolazionePensionati; } set { _AgevolazionePensionati = value; } }
            [DataMember]
            public System.Nullable<byte> ConiugeOFiglio { get { return _ConiugeOFiglio; } set { _ConiugeOFiglio = value; } }
            [DataMember]
            public System.Nullable<byte> FigliMinori3AnniNoHandicap100 { get { return _FigliMinori3AnniNoHandicap100; } set { _FigliMinori3AnniNoHandicap100 = value; } }
            [DataMember]
            public System.Nullable<byte> FigliMinori3AnniNoHandicap50 { get { return _FigliMinori3AnniNoHandicap50; } set { _FigliMinori3AnniNoHandicap50 = value; } }
            [DataMember]
            public System.Nullable<byte> FigliMinori3AnniHandicap100 { get { return _FigliMinori3AnniHandicap100; } set { _FigliMinori3AnniHandicap100 = value; } }
            [DataMember]
            public System.Nullable<byte> FigliMinori3AnniHandicap50 { get { return _FigliMinori3AnniHandicap50; } set { _FigliMinori3AnniHandicap50 = value; } }
            [DataMember]
            public System.Nullable<byte> FigliMaggiori3AnniNoHandicap100 { get { return _FigliMaggiori3AnniNoHandicap100; } set { _FigliMaggiori3AnniNoHandicap100 = value; } }
            [DataMember]
            public System.Nullable<byte> FigliMaggiori3AnniNoHandicap50 { get { return _FigliMaggiori3AnniNoHandicap50; } set { _FigliMaggiori3AnniNoHandicap50 = value; } }
            [DataMember]
            public System.Nullable<byte> FigliMaggiori3AnniHandicap100 { get { return _FigliMaggiori3AnniHandicap100; } set { _FigliMaggiori3AnniHandicap100 = value; } }
            [DataMember]
            public System.Nullable<byte> FigliMaggiori3AnniHandicap50 { get { return _FigliMaggiori3AnniHandicap50; } set { _FigliMaggiori3AnniHandicap50 = value; } }
            [DataMember]
            public System.Nullable<byte> AltriFamiliari100 { get { return _AltriFamiliari100; } set { _AltriFamiliari100 = value; } }
            [DataMember]
            public System.Nullable<byte> AltriFamiliari50 { get { return _AltriFamiliari50; } set { _AltriFamiliari50 = value; } }
            [DataMember]
            public System.Nullable<byte> AddizionaleLombardiaVeneto { get { return _AddizionaleLombardiaVeneto; } set { _AddizionaleLombardiaVeneto = value; } }
            [DataMember]
            public System.Nullable<byte> NonResidenteSchumacker { get { return _NonResidenteSchumacker; } set { _NonResidenteSchumacker = value; } }
            [DataMember]
            public System.Nullable<byte> ConvDoppieImposizioni { get { return _ConvDoppieImposizioni; } set { _ConvDoppieImposizioni = value; } }
            [DataMember]
            public System.Nullable<System.DateTime> DecorrenzaDetrazioneImposte { get { return _DecorrenzaDetrazioneImposte; } set { _DecorrenzaDetrazioneImposte = value; } }
            [DataMember]
            public bool IsStorico { get { return _IsStorico; } set { _IsStorico = value; } }
            #endregion public data member
        }

        [DataContract]
        public class AreaInput
        {
            #region public data member
            [DataMember]
            public long NumeroDomanda { get; set; }
            [DataMember]
            public byte? ProgStorico { get; set; }
            [DataMember]
            public string CodiceFiscale { get; set; }
            #endregion public data member
        }
        #endregion nested class

        public enum RitornoDetrazioni
        {
            NessunErrore,
            Errore,
            Informativa
        };
    }
}