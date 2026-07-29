using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Data;
using INPS.Pensioni.LiquidazioneCi.Entity;


namespace INPS.Pensioni.LiquidazioneCi.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaLiquidazionePensione
    {
        #region private properties
        private DatiGenerici _DatiGenerici;
        private DatiAssicurativi _DatiAssicurativi;
        private DatiOpzione _DatiOpzione;
        private DatiIstruttoria _DatiIstruttoria;
        private DatiProvenienza _DatiProvenienza;

        private List<Mobilita> _lMobilita;
        //private List<CDCMMR> _lCDCMMR;
        private List<CodiceParticolare> _lCodiceParticolare;
        private List<DecodificaLegge44997> _lDecodificaLegge44997;
        private List<DomandaRicorso> _lDomandaRicorso;
        private List<CodiciNatura> _lCodiciNatura;
        private List<DecModalitaLiquidazione> _lModalitaLiquidazione;
        private List<OpzioneRiliquidazione> _lOpzioneRiliquidazione;
        private List<CodiceVirtuale> _lCodiceVirtuale;
        private List<CodiceCi21> _lCodiceCi21;
        private List<CodiceCi28> _lCodiceCi28;
        private List<DecodificaRiconoscimentiInvalidita> _lDecodificaRiconoscimentiInvalidita;
        private List<CodiceRequisitiParticolari> _lCodiceRequisitiParticolari;

        private bool? _IsEsenzioneFiscaleEstero;
        private bool? _IsEsenzioneFiscaleVittima;
        private bool? _IsRiduzioneRetribVisible;
        private GestioneLiquidazionePensione.TipoSalvaguardia? _TipologiaSalvaguardia;
        private bool? _IsUsuranti;
        private bool? _IsGestioneNormale;
        private bool? _IsVecchiaiaInvaliditaSupplementare;
        private bool? _IsImportoIVSVisible;
        private bool? _IsRipristino;
        private bool? _IsRiduzioneRetributivaEnabled;

        #endregion private properties

        #region public data member
        [DataMember]
        public DatiGenerici DatiGenerici { get { return _DatiGenerici; } set { _DatiGenerici = value; } }
        [DataMember]
        public DatiAssicurativi DatiAssicurativi { get { return _DatiAssicurativi; } set { _DatiAssicurativi = value; } }
        [DataMember]
        public DatiOpzione DatiOpzione { get { return _DatiOpzione; } set { _DatiOpzione = value; } }
        [DataMember]
        public DatiIstruttoria DatiIstruttoria { get { return _DatiIstruttoria; } set { _DatiIstruttoria = value; } }
        [DataMember]
        public DatiProvenienza DatiProvenienza { get { return _DatiProvenienza; } set { _DatiProvenienza = value; } }
        [DataMember]
        public List<Mobilita> listaMobilita { get { return _lMobilita; } set { _lMobilita = value; } }
        //[DataMember]
        //public List<CDCMMR> listaCDCMMR { get { return _lCDCMMR; } set { _lCDCMMR = value; } }
        [DataMember]
        public List<CodiceParticolare> listaCodiceParticolare { get { return _lCodiceParticolare; } set { _lCodiceParticolare = value; } }
        [DataMember]
        public List<DecodificaLegge44997> listaDecodificaLegge44997 { get { return _lDecodificaLegge44997; } set { _lDecodificaLegge44997 = value; } }
        [DataMember]
        public List<DomandaRicorso> listaDomandaRicorso { get { return _lDomandaRicorso; } set { _lDomandaRicorso = value; } }
        [DataMember]
        public List<CodiciNatura> listaCodiciNatura { get { return _lCodiciNatura; } set { _lCodiciNatura = value; } }
        [DataMember]
        public List<DecModalitaLiquidazione> listaModalitaLiquidazione { get { return _lModalitaLiquidazione; } set { _lModalitaLiquidazione = value; } }
        [DataMember]
        public List<OpzioneRiliquidazione> lOpzioneRiliquidazione { get { return _lOpzioneRiliquidazione; } set { _lOpzioneRiliquidazione = value; } }
        [DataMember]
        public List<CodiceVirtuale> lCodiceVirtuale { get { return _lCodiceVirtuale; } set { _lCodiceVirtuale = value; } }
        [DataMember]
        public List<CodiceCi21> lCodiceCi21 { get { return _lCodiceCi21; } set { _lCodiceCi21 = value; } }
        [DataMember]
        public List<CodiceCi28> lCodiceCi28 { get { return _lCodiceCi28; } set { _lCodiceCi28 = value; } }
        [DataMember]
        public List<DecodificaRiconoscimentiInvalidita> listaRiconoscimentiInvalidita { get { return _lDecodificaRiconoscimentiInvalidita; } set { _lDecodificaRiconoscimentiInvalidita = value; } }
        [DataMember]
        public List<CodiceRequisitiParticolari> listaCodiceRequisitiParticolari { get { return _lCodiceRequisitiParticolari; } set { _lCodiceRequisitiParticolari = value; } }
        [DataMember]
        public bool? IsEsenzioneFiscaleEstero { get { return _IsEsenzioneFiscaleEstero; } set { _IsEsenzioneFiscaleEstero = value; } }
        [DataMember]
        public bool? IsEsenzioneFiscaleVittima { get { return _IsEsenzioneFiscaleVittima; } set { _IsEsenzioneFiscaleVittima = value; } }
        [DataMember]
        public bool? IsRiduzioneRetribVisible { get { return _IsRiduzioneRetribVisible; } set { _IsRiduzioneRetribVisible = value; } }
        [DataMember]
        public GestioneLiquidazionePensione.TipoSalvaguardia? TipologiaSalvaguardia { get { return _TipologiaSalvaguardia; } set { _TipologiaSalvaguardia = value; } }
        [DataMember]
        public bool? IsUsuranti { get { return _IsUsuranti; } set { _IsUsuranti = value; } }
        [DataMember]
        public bool? IsGestioneNormale { get { return _IsGestioneNormale; } set { _IsGestioneNormale = value; } }
        [DataMember]
        public bool? IsVecchiaiaInvaliditaSupplementare { get { return _IsVecchiaiaInvaliditaSupplementare; } set { _IsVecchiaiaInvaliditaSupplementare = value; } }
        [DataMember]
        public bool? IsImportoIVSVisible { get { return _IsImportoIVSVisible; } set { _IsImportoIVSVisible = value; } }
        [DataMember]
        public bool? IsRipristino { get { return _IsRipristino; } set { _IsRipristino = value; } }
        [DataMember]
        public bool? IsRiduzioneRetributivaEnabled { get { return _IsRiduzioneRetributivaEnabled; } set { _IsRiduzioneRetributivaEnabled = value; } }
        [DataMember]
        public bool? IsTrasformazioneInvalidita { get; set; }
        [DataMember]
        public bool? IsBeneficioArt24Comma15BisFromFELPE { get; set; }
        [DataMember]
        public bool? IsPensioneTipoContributivo { get; set; }
        [DataMember]
        public bool? IsPensioneTipoContributivoConOpzione { get; set; }
        [DataMember]
        public bool? IsSperimentaleDonna { get; set; }
        [DataMember]
        public bool? IsBeneficioApePrecociFromFELPE { get; set; }
        [DataMember]
        public bool? IsPensioneAnzianitaOrRicostituzione { get; set; }
        [DataMember]
        public bool? IsPensioneVecchiaiaOrRicostituzione { get; set; }
        [DataMember]
        public bool? IsEsenzioneFiscaleEsteroFromDetrazioni { get; set; }
        [DataMember]
        public bool? IsRichiestaBonusBookingAbilitata { get; set; }
        [DataMember]
        public bool? IsBeneficioNonVedente { get; set; }
        [DataMember]
        public bool? IsDataRinunciaTrattenutaInpdapStorico { get; set; }
        [DataMember]
        public bool? IsBeneficioNonVedenteFromStorico { get; set; }
        [DataMember]
        public bool? IsRichiestaBonus154Abilitata { get; set; }
        [DataMember]
        public bool? IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione { get; set; }
        [DataMember]
        public bool? IsPensioneTipoContributivoAnzianitàVecchiaia { get; set; }
        [DataMember]
        public bool? IsAnte96 { get; set; }

        //ENG - Aggiornamento Memo86
        [DataMember]
        public bool? IsPresenteTrattenutaFondoCreditoDaPrelievo { get; set; }
        [DataMember]
        public DateTime? DataPrelievoDomanda { get; set; }
        //ENG - Reversibilità: dati Inail
        [DataMember]
        public List<DatiInail> DatiInail { get; set; } 
        #endregion public data member
    }
}
