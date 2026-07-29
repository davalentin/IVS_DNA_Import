using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

using INPS.Pensioni.LiquidazioneAgo.Data.HostRequest;
using INPS.Pensioni.LiquidazioneAgo.Data.CAREPET;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostResponse
{
    [Serializable] 
    public class GAINResponseNew
    {
        #region Constructor
        public GAINResponseNew()
        {
            this.Bititolarieta = new Bititolarieta();
            this.Coda = new Coda();
            this.Controllo = new GAINRequest.AreaControllo();
            this.DanteCausa = new DanteCausa();
            this.DatiGenerici = new DatiGenericiNew();
            this.DatiNuovi = new DatiNuovi();
            this.DatiRetributivi_Contributivi = new DatiRetributivi_Contributivi();
            this.Delegato = new Delegato();
            this.Errori = new Errori();
            this.Familiari = new Familiari();
            this.INAIL_Accompagnamento = new INAIL_Accompagnamento();
            this.IntegrazioneArticolo11 = new IntegrazioneArticolo11();
            this.Intestazione = new Intestazione();
            this.Invciv = new Invciv();
            this.Istruttoria = new Istruttoria();
            this.Pagamento = new Pagamento();
            this.PannelloContributivo = new PannelloContributivo();
            this.Pensionato = new Pensionato();
            this.PensioniAbbinate = new PensioniAbbinate();
            this.Redditi = new Redditi();
            this.ResidenzeEstero = new ResidenzeEstero();
            this.Ricoveri = new Ricoveri();
            this.Sentenze = new Sentenze();
            this.StatoCivile = new StatoCivile();
            this.Supplementi = new Supplementi();
            this.Tutore = new Tutore();
            this.SPRDSC21 = new CAREPET.SPRDSC21New();
            this.DatiRetributiviBIS = new CAREPET.DatiRetributiviBIS();
            this.NuoviDati2024 = new CAREPET.NuoviDati2024();
        }
        #endregion Constructor

        #region Properties

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0)]
        public HostRequest.GAINRequest.AreaControllo Controllo { get; set; }

        [HisFieldInfoMapping(1, 1)]
        public string FILLER { get; set; }

        [HisComplexAreaInfoMapping(2)]
        public CAREPET.Intestazione Intestazione { get; internal set; }

        [HisComplexAreaInfoMapping(3)]
        public CAREPET.DatiGenericiNew DatiGenerici { get; internal set; }

        [HisComplexAreaInfoMapping(4)]
        public CAREPET.Pensionato Pensionato { get; internal set; }

        [HisComplexAreaInfoMapping(5)]
        public CAREPET.Istruttoria Istruttoria { get; internal set; }

        [HisComplexAreaInfoMapping(6)]
        public CAREPET.Pagamento Pagamento { get; internal set; }

        [HisComplexAreaInfoMapping(7)]
        public CAREPET.StatoCivile StatoCivile { get; internal set; }

        [HisComplexAreaInfoMapping(8)]
        public CAREPET.Sentenze Sentenze { get; internal set; }

        [HisComplexAreaInfoMapping(9)]
        public CAREPET.INAIL_Accompagnamento INAIL_Accompagnamento { get; internal set; }

        [HisComplexAreaInfoMapping(10)]
        public CAREPET.PensioniAbbinate PensioniAbbinate { get; internal set; }

        [HisComplexAreaInfoMapping(11)]
        public CAREPET.ResidenzeEstero ResidenzeEstero { get; internal set; }

        [HisComplexAreaInfoMapping(12)]
        public CAREPET.DanteCausa DanteCausa { get; internal set; }

        [HisComplexAreaInfoMapping(13)]
        public CAREPET.DatiRetributivi_Contributivi DatiRetributivi_Contributivi { get; internal set; }

        [HisComplexAreaInfoMapping(14)]
        public CAREPET.IntegrazioneArticolo11 IntegrazioneArticolo11 { get; internal set; }

        [HisComplexAreaInfoMapping(15)]
        public CAREPET.PannelloContributivo PannelloContributivo { get; internal set; }

        [HisComplexAreaInfoMapping(16)]
        public CAREPET.Supplementi Supplementi { get; internal set; }

        [HisComplexAreaInfoMapping(17)]
        public CAREPET.Bititolarieta Bititolarieta { get; internal set; }

        [HisComplexAreaInfoMapping(18)]
        public CAREPET.Redditi Redditi { get; internal set; }

        [HisComplexAreaInfoMapping(19)]
        public CAREPET.Invciv Invciv { get; internal set; }

        [HisComplexAreaInfoMapping(20)]
        public CAREPET.Ricoveri Ricoveri { get; internal set; }
        
        [HisComplexAreaInfoMapping(21)]
        public CAREPET.Delegato Delegato { get; internal set; }

        [HisComplexAreaInfoMapping(22)]
        public CAREPET.Tutore Tutore { get; internal set; }

        [HisComplexAreaInfoMapping(23)]
        public CAREPET.Familiari Familiari { get; internal set; }

        [HisComplexAreaInfoMapping(24)]
        public CAREPET.Errori Errori { get; internal set; }

        [HisComplexAreaInfoMapping(25)]
        public CAREPET.DatiNuovi DatiNuovi { get; internal set; }

        [HisComplexAreaInfoMapping(26)]
        public CAREPET.Coda Coda { get; internal set; }

        [HisComplexAreaInfoMapping(27)]
        public CAREPET.SPRDSC21New SPRDSC21 { get; internal set; }
        
        [HisComplexAreaInfoMapping(28)]
        public CAREPET.DatiRetributiviBIS DatiRetributiviBIS { get; set; }

        [HisComplexAreaInfoMapping(29)]
        public CAREPET.NuoviDati2024 NuoviDati2024 { get; set; }
        #endregion Tracciato Host

        #endregion Properties
    }
}
