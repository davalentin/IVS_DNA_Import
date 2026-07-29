using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using INPS.Pensioni.LiquidazioneCi.Entity;

namespace INPS.Pensioni.LiquidazioneCi.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaMaggiorazioniBenefici
    {
        #region private properties

        private DatiExCombattente _DatiExCombattente;
        private DatiBenefici _DatiBenefici;
        private DatiMaggiorazioni _DatiMaggiorazioni;
        private DatiBeneficioVittimeTerrorismo _DatiBeneficioVittimeTerrorismo;
        private List<CodiceCieco> _ListaCodiceCieco;
        private List<TipoBenefici> _ListaTipoBenefici;
        private List<CodiceMaggiorazioneExCombattente> _ListaCodiceMaggiorazioneExCombattente;
        private List<SoggettoBeneficiario> _ListaSoggettoBeneficiario;
        private List<TipologiaPrestazione> _ListaTipologiaPrestazione;
        private List<TipologiaBeneficioTerrorismo> _ListaTipologiaBeneficioTerrorismo;
        private List<CodiceRequisitiLegge50392> _ListaCodiceRequisitiLegge50392;

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
        public List<CodiceCieco> ListaCodiceCieco { get { return _ListaCodiceCieco; } set { _ListaCodiceCieco = value; } }
        [DataMember]
        public List<CodiceMaggiorazioneExCombattente> ListaCodiceMaggiorazioneExCombattente { get { return _ListaCodiceMaggiorazioneExCombattente; } set { _ListaCodiceMaggiorazioneExCombattente = value; } }
        [DataMember]
        public List<SoggettoBeneficiario> ListaSoggettoBeneficiario { get { return _ListaSoggettoBeneficiario; } set { _ListaSoggettoBeneficiario = value; } }
        [DataMember]
        public List<TipologiaPrestazione> ListaTipologiaPrestazione { get { return _ListaTipologiaPrestazione; } set { _ListaTipologiaPrestazione = value; } }
        [DataMember]
        public List<TipologiaBeneficioTerrorismo> ListaTipologiaBeneficioTerrorismo { get { return _ListaTipologiaBeneficioTerrorismo; } set { _ListaTipologiaBeneficioTerrorismo = value; } }
        [DataMember]
        public List<TipoBenefici> ListaTipoBenefici { get { return _ListaTipoBenefici; } set { _ListaTipoBenefici = value; } }
        [DataMember]
        public List<CodiceRequisitiLegge50392> ListaCodiceRequisitiLegge50392 { get { return _ListaCodiceRequisitiLegge50392; } set { _ListaCodiceRequisitiLegge50392 = value; } }
        [DataMember]
        public bool? IsBeneficioExArt80 { get; set; }
        [DataMember]
        public bool? IsBeneficioArt24Comma15BisFromFELPE { get; set; }
        [DataMember]
        public bool? IsBeneficioApePrecociFromFELPE { get; set; }
        [DataMember]
        public bool? IsDomandaPensioneInabilita { get; set; }
        [DataMember]
        public bool? IsBeneficioVittimeTerrorismo { get; set; }
        [DataMember]
        public bool? IsBeneficioMaggiorazioneAmiantoLegge208_2015 { get; set; }
        [DataMember]
        public int? Settimane { get; set; }
        #endregion public data member
    }
}
