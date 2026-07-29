using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaRichiestaRiepilogo
    {
        #region private properties
        protected string _CodiceFiscale;
		protected string _NumeroDomanda;
        protected byte? _ProgStorico;
		protected Entity.DatiPersonaliParziali _DatiParziali;
		protected TipoRicerca _TipoRecupero;
        protected short _SedeOperatore;
        protected short _CentroOperativoOperatore;
        protected string _MatricolaOperatore;
        protected Utility.TipoAppartenenza _TipoAppRuolo;
        protected Utility.Ruolo _Ruolo;
        protected bool _IsPaginaConferma;
        protected bool _IsConsultazione;
        protected Entity.DatiPersonaliParziali _DatiParzialiDanteCausa;
        protected int _SedeAppartenenzaOperatore;
        //ENG - Bypass "ELIMINAZIONE_CONTROLLO_SEDE"
        protected bool _IsPaginaVisualizzazioneStatoPratiche;
        #endregion private properties

        #region public data member
        [DataMember]
        public string CodiceFiscale { get { return _CodiceFiscale; } set { _CodiceFiscale = value; } }
        [DataMember]
        public string NumeroDomanda { get { return _NumeroDomanda; } set { _NumeroDomanda = value; } }
        [DataMember]
        public byte? ProgStorico { get { return _ProgStorico; } set { _ProgStorico = value; } }
        [DataMember]
        public Entity.DatiPersonaliParziali DatiParziali { get { return _DatiParziali; } set { _DatiParziali = value; } }
        [DataMember]
        public TipoRicerca TipoRecupero { get { return _TipoRecupero; } set { _TipoRecupero = value; } }
        [DataMember]
        public short SedeOperatore { get { return _SedeOperatore; } set { _SedeOperatore = value; } }
        [DataMember]
        public short CentroOperativoOperatore { get { return _CentroOperativoOperatore; } set { _CentroOperativoOperatore = value; } }
        [DataMember]
        public string MatricolaOperatore { get { return _MatricolaOperatore; } set { _MatricolaOperatore = value; } }
        [DataMember]
        public Utility.TipoAppartenenza TipoAppRuolo { get { return _TipoAppRuolo; } set { _TipoAppRuolo = value; } }
        [DataMember]
        public Utility.Ruolo Ruolo { get { return _Ruolo; } set { _Ruolo = value; } }
        [DataMember]
        public bool IsPaginaConferma { get { return _IsPaginaConferma; } set { _IsPaginaConferma = value; } }
        [DataMember]
        public bool IsConsultazione { get { return _IsConsultazione; } set { _IsConsultazione = value; } }
        [DataMember]
        public Entity.DatiPersonaliParziali DatiParzialiDanteCausa { get { return _DatiParzialiDanteCausa; } set { _DatiParzialiDanteCausa = value; } }
        [DataMember]
        public int SedeDiAppartenenzaOperatore { get { return _SedeAppartenenzaOperatore; } set { _SedeAppartenenzaOperatore = value; } }
        //ENG - Bypass "ELIMINAZIONE_CONTROLLO_SEDE"
        [DataMember]
        public bool IsPaginaVisualizzazioneStatoPratiche { get { return _IsPaginaVisualizzazioneStatoPratiche; } set { _IsPaginaVisualizzazioneStatoPratiche = value; } }
        #endregion public data member

        #region nested class
        public enum TipoRicerca
        {
            CodiceFiscale,
            NumeroDomanda,
            DatiPersonaliParziali,
			StatoPratica
        };
        #endregion nested class
    }
}
