using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class Gruppo3
    {
        #region Constructor
        public Gruppo3()
        {
            this.AreaW8 = new AreaW8();
            this.AreaEX_W240 = new AreaEX_W240();
            this.AreaWK1R = new AreaWK1R();
            this.AreaW2CIR = new AreaW2CIR();
            this.AreaWK2R = new AreaWK2R();
            this.AreaAltriCampi = new AreaAltriCampi();
            this.AreaUlterioriDati = new AreaUlterioriDati();
            this.AreaAssegnoAccompagnamento = new AreaAssegnoAccompagnamento();
            this.AreaAssegnoAltroEnte = new AreaAssegnoAltroEnte();
            this.AreaSentenze = new AreaSentenze();
            this.AreaLavEsteroPrePens = new AreaLavEsteroPrePens();
            this.AreaContributi = new AreaContributi();
            this.AreaContributi233 = new AreaContributi233();
            this.AreaSettimaneEst = new AreaSettimaneEst();
            this.AreaContributi503 = new AreaContributi503();
            this.AreaContributi335 = new AreaContributi335();
            this.AreaContributiPostDec = new AreaContributiPostDec();
            this.AreaSpazio = new AreaSpazio();
            this.AreaSicurezza = new AreaSicurezza();
            this.AreaCodiciStampa = new AreaCodiciStampa();
        }
        #endregion Constructor

        #region tracciato COBOL
        #endregion tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0)]
        public AreaW8 AreaW8 { get; set; }

        [HisComplexAreaInfoMapping(1)]
        public AreaEX_W240 AreaEX_W240 { get; set; }

        [HisComplexAreaInfoMapping(2)]
        public AreaWK1R AreaWK1R { get; set; }

        [HisComplexAreaInfoMapping(3)]
        public AreaW2CIR AreaW2CIR { get; set; }

        [HisComplexAreaInfoMapping(4)]
        public AreaWK2R AreaWK2R { get; set; }

        [HisComplexAreaInfoMapping(5)]
        public AreaAltriCampi AreaAltriCampi { get; set; }

        [HisComplexAreaInfoMapping(6)]
        public AreaUlterioriDati AreaUlterioriDati { get; set; }

        [HisComplexAreaInfoMapping(7)]
        public AreaAssegnoAccompagnamento AreaAssegnoAccompagnamento { get; set; }

        [HisComplexAreaInfoMapping(8)]
        public AreaAssegnoAltroEnte AreaAssegnoAltroEnte { get; set; }

        [HisComplexAreaInfoMapping(9)]
        public AreaSentenze AreaSentenze { get; set; }

        [HisComplexAreaInfoMapping(10)]
        public AreaLavEsteroPrePens AreaLavEsteroPrePens { get; set; }

        [HisComplexAreaInfoMapping(11)]
        public AreaContributi AreaContributi { get; set; }

        [HisComplexAreaInfoMapping(12)]
        public AreaContributi233 AreaContributi233 { get; set; }

        [HisComplexAreaInfoMapping(13)]
        public AreaSettimaneEst AreaSettimaneEst { get; set; }

        [HisComplexAreaInfoMapping(14)]
        public AreaContributi503 AreaContributi503 { get; set; }

        [HisComplexAreaInfoMapping(15)]
        public AreaContributi335 AreaContributi335 { get; set; }

        [HisComplexAreaInfoMapping(16)]
        public AreaContributiPostDec AreaContributiPostDec { get; set; }

        [HisComplexAreaInfoMapping(17)]
        public AreaSpazio AreaSpazio { get; set; }

        [HisComplexAreaInfoMapping(18)]
        public AreaSicurezza AreaSicurezza { get; set; }

        /// <summary>
        /// FILLER2021-1 X(325)        SOSTITUISCE N-INAIL SPOSTATO E AMPLIATO
        /// </summary>
        [HisFieldInfoMapping(19, 325)]
        public string FILLER2021_1 { get; set; }

        [HisComplexAreaInfoMapping(20)]
        public AreaCodiciStampa AreaCodiciStampa { get; set; }
        #endregion Tracciato Host
    }
}
