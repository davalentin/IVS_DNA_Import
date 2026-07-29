using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaTutore
    {
        #region tracciato COBOL
        //      *20.07.2001 TUTORE COME DELEGATO LNG 488 BYTES
        //           05  T-TP1DTCOD                       PIC X.
        //      * CODICE TUTORE  T
        //           05  T-TP1DTFISC                      PIC X(16).
        //      * CODICE FISCALE TUTORE
        //           05  T-TP1DTCOG                       PIC X(36).
        //           05  T-TP1DTNOM                       PIC X(36).
        //      *    05  T-TP1DTCOAC                      PIC X(32).
        //      * NOME TUTORE
        //           05  T-TP1DTNASC                      PIC 9(8).
        //      * DATA NASCITA TUTORE GG MM AAAA
        //           05  T-TP1DTSES                       PIC X.
        //      * SESSO TUTORE
        //           05  T-TP1GP1AP28                     PIC X(3).
        //           05  T-TP1GP1AP29                     PIC X(4).
        //           05  T-TP1GP1DRESIDOM                 PIC X.
        //      *CODICE RESIDENZA
        //           05  T-TP1VIA1                        PIC X(52).
        //           05  T-TP1VIA2                        PIC X(52).
        //           05  T-TP1VIA3                        PIC X(52).
        //      * VIA
        //           05  T-TP1CIVICO                      PIC X(18).
        //      * NUM. CIVICO -
        //           05  T-TP1FRAZIO                      PIC X(34).
        //      * FRAZIONE
        //           05  T-TP1ESTITA                    PIC X(1).
        //      * 1=INDIR.ITA    9=IND.EST
        //           05  T-TP1VIA4                        PIC X(52).
        //           05  T-TP1DTCCOM                      PIC X(4).
        //      * CODICE COMUNE DI RESID.T
        //           05  T-TP1COMUNE                      PIC X(37).
        //      * COMUNE DI RESID. T
        //           05  T-TP1DTPROR                      PIC X(3).
        //      * PROV DI RESID T
        //           05  T-TP1CAPRS                       PIC X(9).
        //      * C.A.P.
        //           05  T-TP1AP23                        PIC 9(5).
        //      * CODICE COMUNE DI NASCITA
        //           05  T-TP1AP24                        PIC X(60).
        //      * COMUNE DI NASC ESTESO
        //           05  T-TP1GP1AP25                     PIC XXX.
        //      * PROVINCIA DI NASCITA
        #endregion tracciato COBOL

        #region Tracciato Host
        //*
        // *20.07.2001 TUTORE COME DELEGATO LNG 488 BYTES
        /// <summary>
        /// T_TP1DTCOD X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string T_TP1DTCOD { get; set; }

        // * CODICE TUTORE  T
        /// <summary>
        /// T_TP1DTFISC X(16)  
        /// * CODICE FISCALE TUTORE
        /// </summary>
        [HisFieldInfoMapping(1, 16)]
        public string T_TP1DTFISC { get; set; }

        /// <summary>
        /// T_TP1DTCOG X(36)  
        /// </summary>
        [HisFieldInfoMapping(2, 36)]
        public string T_TP1DTCOG { get; set; }

        /// <summary>
        /// T_TP1DTNOM X(36)  
        /// </summary>
        [HisFieldInfoMapping(3, 36)]
        public string T_TP1DTNOM { get; set; }

        // * NOME TUTORE
        /// <summary>
        /// T_TP1DTNASC 9(8)  
        /// * DATA NASCITA TUTORE GG MM AAAA
        /// </summary>
        [HisFieldInfoMapping(4, 8)]
        public int T_TP1DTNASC { get; set; }

        /// <summary>
        /// T_TP1DTSES X  
        /// * SESSO TUTORE
        /// </summary>
        [HisFieldInfoMapping(5, 1)]
        public string T_TP1DTSES { get; set; }

        /// <summary>
        /// T_TP1GP1AP28 X(3)  
        /// </summary>
        [HisFieldInfoMapping(6, 3)]
        public string T_TP1GP1AP28 { get; set; }

        /// <summary>
        /// T_TP1GP1AP29 X(4)  
        /// </summary>
        [HisFieldInfoMapping(7, 4)]
        public string T_TP1GP1AP29 { get; set; }

        /// <summary>
        /// T_TP1GP1DRESIDOM X  
        /// *CODICE RESIDENZA
        /// </summary>
        [HisFieldInfoMapping(8, 1)]
        public string T_TP1GP1DRESIDOM { get; set; }

        /// <summary>
        /// T_TP1VIA1 X(52)  
        /// * VIA
        /// </summary>
        [HisFieldInfoMapping(9, 52)]
        public string T_TP1VIA1 { get; set; }

        /// <summary>
        /// T_TP1VIA2 X(52)  
        /// * VIA
        /// </summary>
        [HisFieldInfoMapping(10, 52)]
        public string T_TP1VIA2 { get; set; }

        /// <summary>
        /// T_TP1VIA3 X(52)  
        /// * VIA
        /// </summary>
        [HisFieldInfoMapping(11, 52)]
        public string T_TP1VIA3 { get; set; }

        /// <summary>
        /// T_TP1CIVICO X(18)  
        /// * NUM. CIVICO -
        /// </summary>
        [HisFieldInfoMapping(12, 18)]
        public string T_TP1CIVICO { get; set; }

        /// <summary>
        /// T_TP1FRAZIO X(34)  
        /// * FRAZIONE
        /// </summary>
        [HisFieldInfoMapping(13, 34)]
        public string T_TP1FRAZIO { get; set; }

        /// <summary>
        /// T_TP1ESTITA X(1)  
        /// * 1=INDIR.ITA    9=IND.EST
        /// </summary>
        [HisFieldInfoMapping(14, 1)]
        public string T_TP1ESTITA { get; set; }

        /// <summary>
        /// T_TP1VIA4 X(52)  
        /// </summary>
        [HisFieldInfoMapping(15, 52)]
        public string T_TP1VIA4 { get; set; }

        /// <summary>
        /// T_TP1DTCCOM X(4)  
        /// * CODICE COMUNE DI RESID.T
        /// </summary>
        [HisFieldInfoMapping(16, 4)]
        public string T_TP1DTCCOM { get; set; }

        /// <summary>
        /// T_TP1COMUNE X(37)  
        /// * COMUNE DI RESID. T
        /// </summary>
        [HisFieldInfoMapping(17, 37)]
        public string T_TP1COMUNE { get; set; }

        /// <summary>
        /// T_TP1DTPROR X(3)  
        /// * PROV DI RESID T
        /// </summary>
        [HisFieldInfoMapping(18, 3)]
        public string T_TP1DTPROR { get; set; }

        /// <summary>
        /// T_TP1CAPRS X(9) 
        /// * C.A.P. 
        /// </summary>
        [HisFieldInfoMapping(19, 9)]
        public string T_TP1CAPRS { get; set; }

        /// <summary>
        /// T_TP1AP23 9(5)  
        /// * CODICE COMUNE DI NASCITA
        /// </summary>
        [HisFieldInfoMapping(20, 5)]
        public int T_TP1AP23 { get; set; }

        /// <summary>
        /// T_TP1AP24 X(60)  
        /// * COMUNE DI NASC ESTESO
        /// </summary>
        [HisFieldInfoMapping(21, 60)]
        public string T_TP1AP24 { get; set; }

        /// <summary>
        /// T_TP1GP1AP25 XXX 
        /// * PROVINCIA DI NASCITA 
        /// </summary>
        [HisFieldInfoMapping(22, 3)]
        public string T_TP1GP1AP25 { get; set; }


        #endregion Tracciato Host
    }
}
