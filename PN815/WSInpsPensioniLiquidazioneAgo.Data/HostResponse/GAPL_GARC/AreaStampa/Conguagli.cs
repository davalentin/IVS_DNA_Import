using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostResponse.AreaStampa
{
    public class Conguagli
    {
        #region Constructor
        internal Conguagli()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 CONG-NUM        PIC 9(02).
        //     02 CONG-ANNO       PIC 9(04)            OCCURS 50 TIMES.
        //*                          ANNO DI RIFERIMENTO              4202
        //     02 CONG-PENS       PIC S9(07)V9(04) COMP-3 OCCURS 50 TIMES.
        //*                          IMPORTO ANNUO PENSIONE           4402
        //     02 CONG-FAM        PIC S9(07)V9(04) COMP-3 OCCURS 50 TIMES.
        //*                          TRATTAMENTI DI FAMIGLIA          4702
        //     02 CONG-SOC        PIC S9(07)V9(04) COMP-3 OCCURS 50 TIMES.
        //*                          MAGGIORAZIONE SOCIALE            5002
        //*    02 RTCOMB
        //* LA MAGGIORAZIONE EX-COMBATTENTI E' UN DI CUI DI CONG-PENS
        //*
        //     02 CONG-ACC        PIC S9(07)V9(04) COMP-3  OCCURS 50 TIMES.
        //*                            ASSEGNO ACCOMPAGNO             5302
        //     02 FILLER          PIC X(98).
        //*                          LIBERI                           5602
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// CONG_NUM 9(02)  
        /// </summary>
        [HisFieldInfoMapping(0, 2, CobolType = CobolType.Unsigned)]
        public short CONG_NUM { get; set; }

        [HisComplexAreaInfoMapping(1, ListCount = 50)]
        public List<AnnoRiferimento> LISTAnnoRiferimento { get; internal set; }

        [HisComplexAreaInfoMapping(2, ListCount = 50)]
        public List<ImportoAnnuo> LISTImportoAnnuo { get; internal set; }

        [HisComplexAreaInfoMapping(3, ListCount = 50)]
        public List<TrattamentoFamiglia> LISTTrattamentoFamiglia { get; internal set; }

        [HisComplexAreaInfoMapping(4, ListCount = 50)]
        public List<MaggiorazioneSociale> LISTMaggiorazioneSociale { get; internal set; }

        [HisComplexAreaInfoMapping(5, ListCount = 50)]
        public List<AssegnoAccompagnamento> LISTAssegnoAccompagnamento { get; internal set; }
        /// <summary>
        /// FILLER X(98)  
        /// </summary>
        [HisFieldInfoMapping(6, 98)]
        public string FILLER { get; set; }

        // *                          LIBERI                           5602
        #endregion Tracciato Host

        #region nested class
        public class AnnoRiferimento
        {
            #region Constructor
            internal AnnoRiferimento()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 CONG-ANNO       PIC 9(04)            OCCURS 50 TIMES.
            //*                          ANNO DI RIFERIMENTO              4202
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// CONG_ANNO 9(04)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short CONG_ANNO { get; set; }

            // *                          ANNO DI RIFERIMENTO              4202
            #endregion Tracciato Host

            #endregion Properties
        }

        public class ImportoAnnuo
        {
            #region Constructor
            internal ImportoAnnuo()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 CONG-PENS       PIC S9(07)V9(04) COMP-3 OCCURS 50 TIMES.
            //*                          IMPORTO ANNUO PENSIONE           4402
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// CONG_PENS S9(07)V9(04) COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal CONG_PENS { get; set; }

            // *                          IMPORTO ANNUO PENSIONE           4402
            #endregion Tracciato Host

            #endregion Properties
        }

        public class TrattamentoFamiglia
        {
            #region Constructor
            internal TrattamentoFamiglia()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 CONG-FAM        PIC S9(07)V9(04) COMP-3 OCCURS 50 TIMES.
            //*                          TRATTAMENTI DI FAMIGLIA          4702
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// CONG_FAM S9(07)V9(04) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal CONG_FAM { get; set; }

            // *                          TRATTAMENTI DI FAMIGLIA          4702
            #endregion Tracciato Host

            #endregion Properties
        }

        public class MaggiorazioneSociale
        {
            #region Constructor
            internal MaggiorazioneSociale()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 CONG-SOC        PIC S9(07)V9(04) COMP-3 OCCURS 50 TIMES.
            //*                          MAGGIORAZIONE SOCIALE            5002
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// CONG_SOC S9(07)V9(04) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal CONG_SOC { get; set; }

            // *                          MAGGIORAZIONE SOCIALE            5002
            #endregion Tracciato Host

            #endregion Properties
        }

        public class AssegnoAccompagnamento
        {
            #region Constructor
            internal AssegnoAccompagnamento()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 CONG-ACC        PIC S9(07)V9(04) COMP-3  OCCURS 50 TIMES.
            //*                            ASSEGNO ACCOMPAGNO             5302
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// CONG_ACC S9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal CONG_ACC { get; set; }

            // *                            ASSEGNO ACCOMPAGNO             5302
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}

