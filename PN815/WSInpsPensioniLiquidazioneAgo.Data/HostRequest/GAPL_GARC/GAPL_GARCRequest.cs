using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostRequest
{
    public class GAPL_GARCRequest: ITransactionInfo
    {
        #region Constructor
        public GAPL_GARCRequest()
        {
            this.Bititolarieta = new CAREPET.Bititolarieta();
            this.Coda = new CAREPET.Coda();
            this.DanteCausa = new CAREPET.DanteCausa();
            this.DatiGenerici = new CAREPET.DatiGenerici();
            this.DatiNuovi = new CAREPET.DatiNuovi();
            this.DatiRetributivi_Contributivi = new CAREPET.DatiRetributivi_Contributivi();
            this.Delegato = new CAREPET.Delegato();
            this.Errori = new CAREPET.Errori();
            this.Familiari = new CAREPET.Familiari();
            this.INAIL_Accompagnamento = new CAREPET.INAIL_Accompagnamento();
            this.IntegrazioneArticolo11 = new CAREPET.IntegrazioneArticolo11();
            this.Intestazione = new CAREPET.Intestazione();
            this.Invciv = new CAREPET.Invciv();
            this.Istruttoria = new CAREPET.Istruttoria();
            this.Pagamento = new CAREPET.Pagamento();
            this.PannelloContributivo = new CAREPET.PannelloContributivo();
            this.Pensionato = new CAREPET.Pensionato();
            this.PensioniAbbinate = new CAREPET.PensioniAbbinate();
            this.Redditi = new CAREPET.Redditi();
            this.ResidenzeEstero = new CAREPET.ResidenzeEstero();
            this.Ricoveri = new CAREPET.Ricoveri();
            this.Sentenze = new CAREPET.Sentenze();
            this.StatoCivile = new CAREPET.StatoCivile();
            this.Supplementi = new CAREPET.Supplementi();
            this.Tutore = new CAREPET.Tutore();
            this.SPRDSC21 = new CAREPET.SPRDSC21();
            this.DatiRetributiviBIS = new CAREPET.DatiRetributiviBIS();
            this.NuoviDati2024 = new CAREPET.NuoviDati2024();
        }
        #endregion Constructor

        #region Properties

        #region Tracciato Host
        //[HisFieldInfoMapping(0, 8)]
        //public string FILLER { get; set; }

        [HisComplexAreaInfoMapping(1)]
        public CAREPET.Intestazione Intestazione { get; set; }

        [HisComplexAreaInfoMapping(2)]
        public CAREPET.DatiGenerici DatiGenerici { get; set; }

        [HisComplexAreaInfoMapping(3)]
        public CAREPET.Pensionato Pensionato { get; set; }

        [HisComplexAreaInfoMapping(4)]
        public CAREPET.Istruttoria Istruttoria { get; set; }

        [HisComplexAreaInfoMapping(5)]
        public CAREPET.Pagamento Pagamento { get; set; }

        [HisComplexAreaInfoMapping(6)]
        public CAREPET.StatoCivile StatoCivile { get; set; }

        [HisComplexAreaInfoMapping(7)]
        public CAREPET.Sentenze Sentenze { get; set; }

        [HisComplexAreaInfoMapping(8)]
        public CAREPET.INAIL_Accompagnamento INAIL_Accompagnamento { get; set; }

        [HisComplexAreaInfoMapping(9)]
        public CAREPET.PensioniAbbinate PensioniAbbinate { get; set; }

        [HisComplexAreaInfoMapping(10)]
        public CAREPET.ResidenzeEstero ResidenzeEstero { get; set; }

        [HisComplexAreaInfoMapping(11)]
        public CAREPET.DanteCausa DanteCausa { get; set; }

        [HisComplexAreaInfoMapping(12)]
        public CAREPET.DatiRetributivi_Contributivi DatiRetributivi_Contributivi { get; set; }

        [HisComplexAreaInfoMapping(13)]
        public CAREPET.IntegrazioneArticolo11 IntegrazioneArticolo11 { get; set; }

        [HisComplexAreaInfoMapping(14)]
        public CAREPET.PannelloContributivo PannelloContributivo { get; set; }

        [HisComplexAreaInfoMapping(15)]
        public CAREPET.Supplementi Supplementi { get; set; }

        [HisComplexAreaInfoMapping(16)]
        public CAREPET.Bititolarieta Bititolarieta { get; set; }

        [HisComplexAreaInfoMapping(17)]
        public CAREPET.Redditi Redditi { get; set; }

        [HisComplexAreaInfoMapping(18)]
        public CAREPET.Invciv Invciv { get; set; }

        [HisComplexAreaInfoMapping(19)]
        public CAREPET.Ricoveri Ricoveri { get; set; }

        [HisComplexAreaInfoMapping(20)]
        public CAREPET.Delegato Delegato { get; set; }

        [HisComplexAreaInfoMapping(21)]
        public CAREPET.Tutore Tutore { get; set; }

        [HisComplexAreaInfoMapping(22)]
        public CAREPET.Familiari Familiari { get; set; }

        [HisComplexAreaInfoMapping(23)]
        public CAREPET.Errori Errori { get; set; }

        [HisComplexAreaInfoMapping(24)]
        public CAREPET.DatiNuovi DatiNuovi { get; set; }

        [HisComplexAreaInfoMapping(25)]
        public CAREPET.Coda Coda { get; set; }

        [HisComplexAreaInfoMapping(26)]
        public CAREPET.SPRDSC21 SPRDSC21 { get; set; }

        [HisComplexAreaInfoMapping(27)]
        public CAREPET.DatiRetributiviBIS DatiRetributiviBIS { get; set; }

        [HisComplexAreaInfoMapping(28)]
        public CAREPET.NuoviDati2024 NuoviDati2024 { get; set; }
        //SPAZIO RESIDUO 
        /// 13545 - 8 = 13537
        /// RIDOTTO DI 8 PERCHE' SULLA GARC VENGONO AGGIUNTI 8 BYTE
        #endregion Tracciato Host

        public string TransactionName
        {
            get { return "Input GAPL_GARC"; }
        }
        #endregion Properties
    }
}

