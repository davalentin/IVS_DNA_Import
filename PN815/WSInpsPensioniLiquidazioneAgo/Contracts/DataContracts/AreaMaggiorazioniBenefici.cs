using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using INPS.Pensioni.LiquidazioneAgo.Entity;

namespace INPS.Pensioni.LiquidazioneAgo.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaMaggiorazioniBenefici
    {
        #region private properties

        private DatiExCombattente _DatiExCombattente;
        private DatiBenefici _DatiBenefici;
        private DatiMaggiorazioni _DatiMaggiorazioni;
        private DatiBeneficioVittimeTerrorismo _DatiBeneficioVittimeTerrorismo;
        private DatiMaggiorazioneBeneficiStorico _DatiMaggiorazioneBeneficiStorico;

        private List<CodiceCieco> _ListaCodiceCieco;
        private List<TipoBenefici> _ListaTipoBenefici;
        private List<CodiceMaggiorazioneExCombattente> _ListaCodiceMaggiorazioneExCombattente;
        private List<SoggettoBeneficiario> _ListaSoggettoBeneficiario;
        private List<TipologiaPrestazione> _ListaTipologiaPrestazione;
        private List<TipologiaBeneficioTerrorismo> _ListaTipologiaBeneficioTerrorismo;

        private bool? _IsBeneficioBloccato;
        private bool? _IsVisiblePerSuperstitiOrPMO;

        #endregion private properties

        #region public data member

        [DataMember]
        public DatiExCombattente DatiExCombattente { get { return _DatiExCombattente; } set { _DatiExCombattente = value; } }
        [DataMember]
        public DatiBenefici DatiBenefici { get { return _DatiBenefici; } set { _DatiBenefici = value; } }
        [DataMember]
        public DatiMaggiorazioni DatiMaggiorazioni { get { return _DatiMaggiorazioni; } set { _DatiMaggiorazioni = value; } }
        [DataMember]
        public DatiBeneficioVittimeTerrorismo DatiBeneficioVittimeTerrorismo { get { return _DatiBeneficioVittimeTerrorismo; } set { _DatiBeneficioVittimeTerrorismo = value; } }
        [DataMember]
        public DatiMaggiorazioneBeneficiStorico DatiMaggiorazioneBeneficiStorico { get { return _DatiMaggiorazioneBeneficiStorico; } set { _DatiMaggiorazioneBeneficiStorico = value; } }

        [DataMember]
        public List<CodiceCieco> ListaCodiceCieco { get { return _ListaCodiceCieco; } set { _ListaCodiceCieco = value; } }
        [DataMember]
        public List<CodiceMaggiorazioneExCombattente> ListaCodiceMaggiorazioneExCombattente { get { return _ListaCodiceMaggiorazioneExCombattente; } set { _ListaCodiceMaggiorazioneExCombattente = value; } }
        [DataMember]
        public List<TipoBenefici> ListaTipoBenefici { get { return _ListaTipoBenefici; } set { _ListaTipoBenefici = value; } }
        [DataMember]
        public List<SoggettoBeneficiario> ListaSoggettoBeneficiario { get { return _ListaSoggettoBeneficiario; } set { _ListaSoggettoBeneficiario = value; } }
        [DataMember]
        public List<TipologiaPrestazione> ListaTipologiaPrestazione { get { return _ListaTipologiaPrestazione; } set { _ListaTipologiaPrestazione = value; } }
        [DataMember]
        public List<TipologiaBeneficioTerrorismo> ListaTipologiaBeneficioTerrorismo { get { return _ListaTipologiaBeneficioTerrorismo; } set { _ListaTipologiaBeneficioTerrorismo = value; } }

        [DataMember]
        public bool? IsBeneficioBloccato { get { return _IsBeneficioBloccato; } set { _IsBeneficioBloccato = value; } }
        [DataMember]
        public bool? IsBeneficioExArt80 { get; set; }
        [DataMember]
        public bool? IsBeneficioMinatori { get; set; }
        [DataMember]
        public bool? IsDomandaInabilitaIndiretta { get; set; }
        [DataMember]
        public bool? IsVisiblePerSuperstitiOrPMO { get { return _IsVisiblePerSuperstitiOrPMO; } set { _IsVisiblePerSuperstitiOrPMO = value; } }
        [DataMember]
        public bool? IsBeneficioAmianto181 { get; set; }
        [DataMember]
        public bool? IsNumSettimaneBeneficioEnabled { get; set; }
        [DataMember]
        public bool? IsBeneficioArt24Comma15BisFromFELPE { get; set; }
        [DataMember]
        public bool? IsPrepensionamentoEditoria { get; set; }
        [DataMember]
        public bool? IsPrepensionamentoEditoriaArt1c154L205_2017 { get; set; }
        [DataMember]
        public bool? IsPrepensionamentoEditoriaArt1c500L160_2019 { get; set; }
        [DataMember]
        public bool? IsBeneficioApePrecociFromFELPE { get; set; }
        [DataMember]
        public bool? IsDomandaPensioneInabilita { get; set; }
        [DataMember]
        public bool? IsBeneficioVittimeTerrorismo { get; set; }
        [DataMember]
        public bool? IsBeneficioInabilitaByPrimoCodiceNatura { get; set; }
        [DataMember]
        public bool? IsBeneficioUsuranti { get; set; }
        [DataMember]
        public bool? IsBeneficioMaggiorazioneAmiantoLegge208_2015 { get; set; }
        [DataMember]
        public bool? IsBeneficioNonVedenteByPrimoCodiceNatura { get; set; }
        [DataMember]
        public int? Settimane { get; set; }
        [DataMember]
        public bool? IsPrepensionamentoEditoriaFiltroEBA { get; set; }

        #endregion public data member
    }
}
