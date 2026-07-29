using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostResponse.AreaStampa
{
    public class Recupero_Crediti
    {
        #region Constructor
        internal Recupero_Crediti()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 COD-REC-CRED     PIC X(01).
        //*                          CODICE RECUPERO CREDITI          5700
        //     02 FILLER           PIC X(02).
        //*                                                           5701
        //     02 CONG-ONPI        PIC S9(07)V9(04) COMP-3.
        //*                          CONGUAGLIO ONPI                  5703
        //     02 TOT-INDEBITO     PIC 9(07)V9(04) COMP-3.
        //*                          IMPORTO TOTALE INDEBITO          5709
        //     02 IMP-TASSATO      PIC 9(07)V9(04) COMP-3.
        //*                          IMPORTO GIA' TASSATO             5715
        //     02 CONG-SIND        PIC S9(07)V9(04) COMP-3.
        //*                          CONGUAGLIO SINDACATO             5721
        //     02 NUM-EAD75        PIC 9(08).
        //*                          NUMERO DOMANDA EAD75             5727
        //     02 DTRICH-EAD.
        //*                          DATA RICH.PREST.(GGMMAAAA)       5735
        //        03 DTRICH-GG     PIC 9(02).
        //        03 DTRICH-MM     PIC 9(02).
        //        03 DTRICH-AA     PIC 9(04).
        //     02 LORDO-ACCANT     PIC 9(07)V9(04) COMP-3.
        //*                          LORDO ARRETR.ACCANT.             5743
        //     02 RCOD-RECRED      PIC X(01).
        //*                          COD.RISP.REC.CREDITI             5749
        //     02 NUM-RECRED       PIC 9(07).
        //*                          NUM.POS.REC.CREDITI              5750
        //     02 TRAT-ERAR-AC     PIC S9(07)V9(04) COMP-3.
        //*                          TRATTENUTE ERARIALI A.C.         5757
        //     02 FILLER           PIC X(02).
        //*                                                           5763
        //     02 DT-RICH          PIC 9(08).
        //*                          DATA RICHIESTA GGMMAAAA          5765
        //     02 TIPO-PROV        PIC 9(01).
        //*                          TIPO PROVENIENZA                 5773
        //     02 MOT-1A           PIC X(01).
        //*                          1.A MOTIVAZIONE                  5774
        //*
        //     02 MOT-2A           PIC X(01).
        //*                          2.A MOTIVAZIONE                  5775
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// COD_REC_CRED X(01)  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string COD_REC_CRED { get; set; }

        // *                          CODICE RECUPERO CREDITI          5700
        /// <summary>
        /// FILLER X(02)  
        /// </summary>
        [HisFieldInfoMapping(1, 2)]
        public string FILLER1 { get; set; }

        // *                                                           5701
        /// <summary>
        /// CONG_ONPI S9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(2, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal CONG_ONPI { get; set; }

        // *                          CONGUAGLIO ONPI                  5703
        /// <summary>
        /// TOT_INDEBITO 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(3, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal TOT_INDEBITO { get; set; }

        // *                          IMPORTO TOTALE INDEBITO          5709
        /// <summary>
        /// IMP_TASSATO 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(4, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal IMP_TASSATO { get; set; }

        // *                          IMPORTO GIA' TASSATO             5715
        /// <summary>
        /// CONG_SIND S9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(5, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal CONG_SIND { get; set; }

        // *                          CONGUAGLIO SINDACATO             5721
        /// <summary>
        /// NUM_EAD75 9(08)  
        /// </summary>
        [HisFieldInfoMapping(6, 8, CobolType = CobolType.Unsigned)]
        public int NUM_EAD75 { get; set; }

        // *                          NUMERO DOMANDA EAD75             5727
        // 02 DTRICH-EAD.
        // *                          DATA RICH.PREST.(GGMMAAAA)       5735
        /// <summary>
        /// DTRICH_GG 9(02)  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short DTRICH_GG { get; set; }

        /// <summary>
        /// DTRICH_MM 9(02)  
        /// </summary>
        [HisFieldInfoMapping(8, 2, CobolType = CobolType.Unsigned)]
        public short DTRICH_MM { get; set; }

        /// <summary>
        /// DTRICH_AA 9(04)  
        /// </summary>
        [HisFieldInfoMapping(9, 4, CobolType = CobolType.Unsigned)]
        public short DTRICH_AA { get; set; }

        /// <summary>
        /// LORDO_ACCANT 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(10, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal LORDO_ACCANT { get; set; }

        // *                          LORDO ARRETR.ACCANT.             5743
        /// <summary>
        /// RCOD_RECRED X(01)  
        /// </summary>
        [HisFieldInfoMapping(11, 1)]
        public string RCOD_RECRED { get; set; }

        // *                          COD.RISP.REC.CREDITI             5749
        /// <summary>
        /// NUM_RECRED 9(07)  
        /// </summary>
        [HisFieldInfoMapping(12, 7, CobolType = CobolType.Unsigned)]
        public int NUM_RECRED { get; set; }

        // *                          NUM.POS.REC.CREDITI              5750
        /// <summary>
        /// TRAT_ERAR_AC S9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(13, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal TRAT_ERAR_AC { get; set; }

        // *                          TRATTENUTE ERARIALI A.C.         5757
        /// <summary>
        /// FILLER X(02)  
        /// </summary>
        [HisFieldInfoMapping(14, 2)]
        public string FILLER2 { get; set; }

        // *                                                           5763
        /// <summary>
        /// DT_RICH 9(08)  
        /// </summary>
        [HisFieldInfoMapping(15, 8, CobolType = CobolType.Unsigned)]
        public int DT_RICH { get; set; }

        // *                          DATA RICHIESTA GGMMAAAA          5765
        /// <summary>
        /// TIPO_PROV 9(01)  
        /// </summary>
        [HisFieldInfoMapping(16, 1, CobolType = CobolType.Unsigned)]
        public short TIPO_PROV { get; set; }

        // *                          TIPO PROVENIENZA                 5773
        /// <summary>
        /// MOT_1A X(01)  
        /// </summary>
        [HisFieldInfoMapping(17, 1)]
        public string MOT_1A { get; set; }

        // *                          1.A MOTIVAZIONE                  5774
        //*
        /// <summary>
        /// MOT_2A X(01)  
        /// </summary>
        [HisFieldInfoMapping(18, 1)]
        public string MOT_2A { get; set; }

        // *                          2.A MOTIVAZIONE                  5775
        #endregion Tracciato Host

        #region nested class

        #endregion nested class

        #endregion Properties
    }
}

