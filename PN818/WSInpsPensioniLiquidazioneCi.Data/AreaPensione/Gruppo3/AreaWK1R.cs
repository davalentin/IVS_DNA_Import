using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaWK1R
    {
        #region tracciato COBOL
        //        *   SETTORE   INPRIC.
        //* AREA RIEMPITA DA RICOSTITUZIONI E RINNOVO
        //     04  ARWK1R.
        //     05  IW1CM345            PIC 9.
        //*+COD. ART.3,4,5/140/DPCM
        //     05  IW1TM345            PIC 9(5)V9(4) COMP-3.
        //*EURO +TOTALE ART. 3,4,5 E DPCM
        //     05  IW1DART5            PIC S9(5)V9(4)  COMP-3.
        //*EURO +ECCED.ART.5 140 AL 8801
        //     05  IW1RILAUT           PIC 9.
        //* CODICE RILIQUIDAZIONE PENSIONI AUTONOMI ART.6 638/83
        //* 0 = NO RILIQUIDATA       1 = SI RILIQUIDATA
        //     05  IW1ART3SO           PIC 9.
        //* CODICE PER REVERSIBILI CON ART.3/140
        //* 1=AUMENTO IN MISURA INTERA      2=AUMENTO RIDOTTO IN ALIQUOTA
        //     05  IW1RILAR3           PIC 9.
        //* CODICE PER AVVENUTA RILIQUIDAZIONE DELLE PENSIONI CON ART.3/140
        //* 1=SI'            2=NO
        //     05  IW1RMSS72            PIC S9(7)V9(6)      COMP-3.
        //*EURO +RMS PER APPLICAZIONE DELLA SENTENZA N. 72/90
        //     05  IW1A11S72            PIC S9(3)V9(6) COMP-3.
        //*EURO +IVS ART.11/488 PER APPLIC. SENTENZA N. 72/90
        //     05  IW1CSEN72            PIC 9.
        //* CODICE PER APPLICAZIONE    SENTENZA N. 72/90
        //*                        (0 = NO; 1 = SI)
        //     05  IW1AS72A             PIC 9(5)V9(4)       COMP-3.
        //*EURO  AUMENTO PER SENTENZA N. 72/90
        //     05  IVUOTO               PIC 9(5)V9(4)       COMP-3.
        //*EURO  VUOTO AL POSTO DI FISSE EX SENTENZA 34
        //     05  IW1FDPCM             PIC 9.
        //* FLAG SEGNALAZIONE RILIQ. ART.2 DPCM 16/12/89
        //     05  IW1RMSAR2            PIC S9(7)V9(6)      COMP-3.
        //*EURO +RMS PER RILIQUIDAZIONE ART. 2 DPCM 16/12/89
        //     05  IW1A11AR2            PIC S9(3)V9(6)      COMP-3.
        //*EURO +IVS ART.11/488 RILIQ.  ART. 2 DPCM 16/12/89
        //     05  IW1ADPCM             PIC 9(5)V9(4)       COMP-3.
        //*EURO  AUMENTO  ART.2 DPCM 16/12/89
        //*+(AAMM) DECORRENZA ART.2 DPCM 16/12/89
        //          10 IW1DDPCMA        PIC 9(4).
        //          10 IW1DDPCMM        PIC 9(2).
        //     05  IW1CM409             PIC 9.
        //*+(=1) CODICE APPLICAZIONE ART. 1 D. L . 409/90
        //     05  IW1TM409             PIC 9(5)V9(4)       COMP-3.
        //*EURO  AUMENTO TOTALE EX ART. 1 D.L. 409/90
        #endregion tracciato COBOL

        #region Tracciato Host
        // *   SETTORE   INPRIC.
        // * AREA RIEMPITA DA RICOSTITUZIONI E RINNOVO
        // 04  ARWK1R.
        /// <summary>
        /// IW1CM345 9  
        /// *+COD. ART.3,4,5/140/DPCM
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public short IW1CM345 { get; set; }

        /// <summary>
        /// IW1TM345 9(5)V9(4) COMP-3 
        /// *EURO +TOTALE ART. 3,4,5 E DPCM
        /// </summary>
        [HisFieldInfoMapping(1, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal IW1TM345 { get; set; }

        /// <summary>
        /// IW1DART5 S9(5)V9(4) COMP-3 
        /// *EURO +ECCED.ART.5 140 AL 8801
        /// </summary>
        [HisFieldInfoMapping(2, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IW1DART5 { get; set; }

        /// <summary>
        /// IW1RILAUT 9  
        /// * CODICE RILIQUIDAZIONE PENSIONI AUTONOMI ART.6 638/83
        /// * 0 = NO RILIQUIDATA       1 = SI RILIQUIDATA
        /// </summary>
        [HisFieldInfoMapping(3, 1)]
        public short IW1RILAUT { get; set; }

        /// <summary>
        /// IW1ART3SO 9  
        /// * CODICE PER REVERSIBILI CON ART.3/140
        /// * 1=AUMENTO IN MISURA INTERA      2=AUMENTO RIDOTTO IN ALIQUOTA
        /// </summary>
        [HisFieldInfoMapping(4, 1)]
        public short IW1ART3SO { get; set; }

        /// <summary>
        /// IW1RILAR3 9  
        /// * CODICE PER AVVENUTA RILIQUIDAZIONE DELLE PENSIONI CON ART.3/140
        /// * 1=SI'            2=NO
        /// </summary>
        [HisFieldInfoMapping(5, 1)]
        public short IW1RILAR3 { get; set; }

        /// <summary>
        /// IW1RMSS72 S9(7)V9(6) COMP-3 
        /// *EURO +RMS PER APPLICAZIONE DELLA SENTENZA N. 72/90
        /// </summary>
        [HisFieldInfoMapping(6, 7, Scale = 6, CobolType = CobolType.Comp3)]
        public decimal IW1RMSS72 { get; set; }

        /// <summary>
        /// IW1A11S72 S9(3)V9(6) COMP-3 
        /// *EURO +IVS ART.11/488 PER APPLIC. SENTENZA N. 72/90
        /// </summary>
        [HisFieldInfoMapping(7, 5, Scale = 6, CobolType = CobolType.Comp3)]
        public decimal IW1A11S72 { get; set; }

        /// <summary>
        /// IW1CSEN72 9  
        /// * CODICE PER APPLICAZIONE    SENTENZA N. 72/90
        /// *                        (0 = NO; 1 = SI)
        /// </summary>
        [HisFieldInfoMapping(8, 1)]
        public short IW1CSEN72 { get; set; }

        /// <summary>
        /// IW1AS72A 9(5)V9(4) COMP-3 
        /// *EURO  AUMENTO PER SENTENZA N. 72/90
        /// </summary>
        [HisFieldInfoMapping(9, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal IW1AS72A { get; set; }

        /// <summary>
        /// IVUOTO 9(5)V9(4) COMP-3 
        /// *EURO  VUOTO AL POSTO DI FISSE EX SENTENZA 34
        /// </summary>
        [HisFieldInfoMapping(10, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal IVUOTO { get; set; }

        /// <summary>
        /// IW1FDPCM 9  
        /// * FLAG SEGNALAZIONE RILIQ. ART.2 DPCM 16/12/89
        /// </summary>
        [HisFieldInfoMapping(11, 1)]
        public short IW1FDPCM { get; set; }

        /// <summary>
        /// IW1RMSAR2 S9(7)V9(6) COMP-3 
        /// *EURO +RMS PER RILIQUIDAZIONE ART. 2 DPCM 16/12/89
        /// </summary>
        [HisFieldInfoMapping(12, 7, Scale = 6, CobolType = CobolType.Comp3)]
        public decimal IW1RMSAR2 { get; set; }

        /// <summary>
        /// IW1A11AR2 S9(3)V9(6) COMP-3 
        /// *EURO +IVS ART.11/488 RILIQ.  ART. 2 DPCM 16/12/89
        /// </summary>
        [HisFieldInfoMapping(13, 5, Scale = 6, CobolType = CobolType.Comp3)]
        public decimal IW1A11AR2 { get; set; }

        /// <summary>
        /// IW1ADPCM 9(5)V9(4) COMP-3 
        /// *EURO  AUMENTO  ART.2 DPCM 16/12/89
        /// </summary>
        [HisFieldInfoMapping(14, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal IW1ADPCM { get; set; }

        /// <summary>
        /// IW1DDPCMA 9(4)  
        /// *+(AAMM) DECORRENZA ART.2 DPCM 16/12/89
        /// </summary>
        [HisFieldInfoMapping(15, 4)]
        public short IW1DDPCMA { get; set; }

        /// <summary>
        /// IW1DDPCMM 9(2)  
        /// *+(AAMM) DECORRENZA ART.2 DPCM 16/12/89
        /// </summary>
        [HisFieldInfoMapping(16, 2)]
        public short IW1DDPCMM { get; set; }

        /// <summary>
        /// IW1CM409 9  
        /// *+(=1) CODICE APPLICAZIONE ART. 1 D. L . 409/90
        /// </summary>
        [HisFieldInfoMapping(17, 1)]
        public short IW1CM409 { get; set; }

        /// <summary>
        /// IW1TM409 9(5)V9(4) COMP-3 
        /// *EURO  AUMENTO TOTALE EX ART. 1 D.L. 409/90
        /// </summary>
        [HisFieldInfoMapping(18, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal IW1TM409 { get; set; }


        #endregion Tracciato Host
    }
}
