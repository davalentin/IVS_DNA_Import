using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Data;
using INPS.Pensioni.LiquidazioneFs.Entity;
using INPS.Pensioni.LiquidazioneFs;

namespace INPS.Pensioni.LiquidazioneFs.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaMaggiorazioniBenefici
    {
        #region private properties

        private DatiExCombattente _DatiExCombattente;
        private DatiBenefici _DatiBenefici;
        private DatiBeneficioVittimeTerrorismo _DatiBeneficioVittimeTerrorismo;
        private DatiDL407 _DatiDL407;
        private DatiArticolo2 _DatiArticolo2;
        private DatiPrivilegiate _DatiPrivilegiate;
        private List<CodiceCieco> _ListaCodiceCieco;
        private List<TipoBenefici> _ListaTipoBenefici;
        private List<CodiceMaggiorazioneExCombattente> _ListaCodiceMaggiorazioneExCombattente;
        private List<SoggettoBeneficiario> _ListaSoggettoBeneficiario;
        private List<TipologiaPrestazione> _ListaTipologiaPrestazione;
        private List<TipologiaBeneficioTerrorismo> _ListaTipologiaBeneficioTerrorismo;
        private List<CodicePensioniPrivilegiate> _ListaCodicePensioniPrivilegiate;
        private bool? _IsNuovaGestioneDL407ForAnteArm;


        #endregion private properties

        #region public data member

        [DataMember]
        public DatiExCombattente DatiExCombattente { get { return _DatiExCombattente; } set { _DatiExCombattente = value; } }
        [DataMember]
        public DatiBenefici DatiBenefici { get { return _DatiBenefici; } set { _DatiBenefici = value; } }
        [DataMember]
        public DatiBeneficioVittimeTerrorismo DatiBeneficioVittimeTerrorismo { get { return _DatiBeneficioVittimeTerrorismo; } set { _DatiBeneficioVittimeTerrorismo = value; } }
        [DataMember]
        public DatiDL407 DatiDL407 { get { return _DatiDL407; } set { _DatiDL407 = value; } }
        [DataMember]
        public DatiArticolo2 DatiArticolo2 { get { return _DatiArticolo2; } set { _DatiArticolo2 = value; } }
        [DataMember]
        public DatiPrivilegiate DatiPrivilegiate { get { return _DatiPrivilegiate; } set { _DatiPrivilegiate = value; } }
        [DataMember]
        public List<CodiceCieco> ListaCodiceCieco { get { return _ListaCodiceCieco; } set { _ListaCodiceCieco = value; } }
        [DataMember]
        public List<CodiceMaggiorazioneExCombattente> ListaCodiceMaggiorazioneExCombattente { get { return _ListaCodiceMaggiorazioneExCombattente; } set { _ListaCodiceMaggiorazioneExCombattente = value; } }
        [DataMember]
        public List<TipoBenefici> ListaTipoBenefici { get { return _ListaTipoBenefici; } set { _ListaTipoBenefici = value; } }
        [DataMember]
        public List<CodicePensioniPrivilegiate> ListaCodicePensioniPrivilegiate { get { return _ListaCodicePensioniPrivilegiate; } set { _ListaCodicePensioniPrivilegiate = value; } }
        [DataMember]
        public List<SoggettoBeneficiario> ListaSoggettoBeneficiario { get { return _ListaSoggettoBeneficiario; } set { _ListaSoggettoBeneficiario = value; } }
        [DataMember]
        public List<TipologiaPrestazione> ListaTipologiaPrestazione { get { return _ListaTipologiaPrestazione; } set { _ListaTipologiaPrestazione = value; } }
        [DataMember]
        public List<TipologiaBeneficioTerrorismo> ListaTipologiaBeneficioTerrorismo { get { return _ListaTipologiaBeneficioTerrorismo; } set { _ListaTipologiaBeneficioTerrorismo = value; } }
        [DataMember]
        public bool? IsNuovaGestioneDL407ForAnteArm { get { return _IsNuovaGestioneDL407ForAnteArm; } set { _IsNuovaGestioneDL407ForAnteArm = value; } }
        [DataMember]
        public bool? IsBeneficioArt24Comma15BisFromFELPE { get; set; }
        [DataMember]
        public bool? IsBeneficioApePrecociFromFELPE { get; set; }
        [DataMember]
        public bool? IsDomandaPensioneInabilita { get; set; }
        [DataMember]
        public bool? IsBeneficioVittimeTerrorismo { get; set; }
        [DataMember]
        public bool? IsMaggiorazioniForMemo72 { get; set; }
        [DataMember]
        public int? Settimane { get; set; }
        #endregion public data member
    }
}
