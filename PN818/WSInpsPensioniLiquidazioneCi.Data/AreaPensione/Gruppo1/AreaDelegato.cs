using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaDelegato
    {
        #region tracciato COBOL
        //        *20.07.2001   AREA DELEGATO LNG 488 BYTES
        //           05  D-TP1DTCOD                       PIC X.
        //      * CODICE DELEGATO/TUTORE  D/T
        //           05  D-TP1DTFISC                      PIC X(16).
        //      * CODICE FISCALE DELEGATO
        //           05  D-TP1DTCOG                       PIC X(36).
        //           05  D-TP1DTNOM                       PIC X(36).
        //      * COGNOME NOME DELEGATO
        //           05  D-TP1DTNASC                      PIC 9(8).
        //      * DATA NASCITA DELEGATO GG MM AAAA
        //           05  D-TP1DTSES                       PIC X.
        //      * SESSO DELEGATO
        //           05  D-TP1GP1AP28                     PIC X(3).
        //           05  D-TP1GP1AP29                     PIC X(4).
        //           05  D-TP1GP1DRESIDOM                 PIC X.
        //      *CODICE RESIDENZA
        //           05  D-TP1VIA1                        PIC X(52).
        //           05  D-TP1VIA2                        PIC X(52).
        //           05  D-TP1VIA3                        PIC X(52).
        //      * VIA
        //           05  D-TP1CIVICO                      PIC X(18).
        //      * NUM. CIVICO -
        //           05  D-TP1FRAZIO                      PIC X(34).
        //      * FRAZIONE
        //           05  D-TP1ESTITA                    PIC X(1).
        //      * 1=INDIR.ITA    9=IND.EST
        //           05  D-TP1VIA4                        PIC X(52).
        //           05  D-TP1DTCCOM                      PIC X(4).
        //      * CODICE COMUNE DI RESID. D
        //           05  D-TP1COMUNE                      PIC X(37).
        //      * COMUNE DI RESID. D
        //           05  D-TP1DTPROR                      PIC X(3).
        //      * PROV DI RESID D
        //           05  D-TP1CAPRS                       PIC X(9).
        //      * C.A.P.
        //           05  D-TP1AP23                        PIC 9(5).
        //      * CODICE COMUNE DI NASCITA
        //           05  D-TP1AP24                        PIC X(60).
        //      * COMUNE DI NASC ESTESO
        //           05  D-TP1GP1AP25                     PIC XXX.
        //      * PROVINCIA DI NASC ESTESO
        //      *
        #endregion tracciato COBOL

        #region Tracciato Host
        // *20.07.2001   AREA DELEGATO LNG 488 BYTES
        /// <summary>
        /// D_TP1DTCOD X  
        /// * CODICE DELEGATO/TUTORE  D/T
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string D_TP1DTCOD { get; set; }

        /// <summary>
        /// D_TP1DTFISC X(16)  
        /// * CODICE FISCALE DELEGATO
        /// </summary>
        [HisFieldInfoMapping(1, 16)]
        public string D_TP1DTFISC { get; set; }

        /// <summary>
        /// D_TP1DTCOG X(36)  
        /// * COGNOME DELEGATO
        /// </summary>
        [HisFieldInfoMapping(2, 36)]
        public string D_TP1DTCOG { get; set; }

        /// <summary>
        /// D_TP1DTNOM X(36) 
        /// * NOME DELEGATO 
        /// </summary>
        [HisFieldInfoMapping(3, 36)]
        public string D_TP1DTNOM { get; set; }

        /// <summary>
        /// D_TP1DTNASC 9(8)  
        /// * DATA NASCITA DELEGATO GG MM AAAA
        /// </summary>
        [HisFieldInfoMapping(4, 8)]
        public int D_TP1DTNASC { get; set; }

        /// <summary>
        /// D_TP1DTSES X  
        /// * SESSO DELEGATO
        /// </summary>
        [HisFieldInfoMapping(5, 1)]
        public string D_TP1DTSES { get; set; }

        /// <summary>
        /// D_TP1GP1AP28 X(3)  
        /// </summary>
        [HisFieldInfoMapping(6, 3)]
        public string D_TP1GP1AP28 { get; set; }

        /// <summary>
        /// D_TP1GP1AP29 X(4)  
        /// </summary>
        [HisFieldInfoMapping(7, 4)]
        public string D_TP1GP1AP29 { get; set; }

        /// <summary>
        /// D_TP1GP1DRESIDOM X  
        /// *CODICE RESIDENZA
        /// </summary>
        [HisFieldInfoMapping(8, 1)]
        public string D_TP1GP1DRESIDOM { get; set; }

        /// <summary>
        /// D_TP1VIA1 X(52) 
        /// * VIA 
        /// </summary>
        [HisFieldInfoMapping(9, 52)]
        public string D_TP1VIA1 { get; set; }

        /// <summary>
        /// D_TP1VIA2 X(52)  
        /// * VIA
        /// </summary>
        [HisFieldInfoMapping(10, 52)]
        public string D_TP1VIA2 { get; set; }

        /// <summary>
        /// D_TP1VIA3 X(52) 
        /// * VIA 
        /// </summary>
        [HisFieldInfoMapping(11, 52)]
        public string D_TP1VIA3 { get; set; }

        /// <summary>
        /// D_TP1CIVICO X(18)  
        /// * NUM. CIVICO -
        /// </summary>
        [HisFieldInfoMapping(12, 18)]
        public string D_TP1CIVICO { get; set; }

        /// <summary>
        /// D_TP1FRAZIO X(34)  
        /// * FRAZIONE
        /// </summary>
        [HisFieldInfoMapping(13, 34)]
        public string D_TP1FRAZIO { get; set; }

        /// <summary>
        /// D_TP1ESTITA X(1)  
        /// * 1=INDIR.ITA    9=IND.EST
        /// </summary>
        [HisFieldInfoMapping(14, 1)]
        public string D_TP1ESTITA { get; set; }

        /// <summary>
        /// D_TP1VIA4 X(52)  
        /// </summary>
        [HisFieldInfoMapping(15, 52)]
        public string D_TP1VIA4 { get; set; }

        /// <summary>
        /// D_TP1DTCCOM X(4)  
        /// * CODICE COMUNE DI RESID. D
        /// </summary>
        [HisFieldInfoMapping(16, 4)]
        public string D_TP1DTCCOM { get; set; }

        /// <summary>
        /// D_TP1COMUNE X(37)  
        /// * COMUNE DI RESID. D
        /// </summary>
        [HisFieldInfoMapping(17, 37)]
        public string D_TP1COMUNE { get; set; }

        /// <summary>
        /// D_TP1DTPROR X(3)  
        /// * PROV DI RESID D
        /// </summary>
        [HisFieldInfoMapping(18, 3)]
        public string D_TP1DTPROR { get; set; }

        /// <summary>
        /// D_TP1CAPRS X(9)  
        /// * C.A.P.
        /// </summary>
        [HisFieldInfoMapping(19, 9)]
        public string D_TP1CAPRS { get; set; }

        /// <summary>
        /// D_TP1AP23 9(5)  
        /// * CODICE COMUNE DI NASCITA
        /// </summary>
        [HisFieldInfoMapping(20, 5)]
        public int D_TP1AP23 { get; set; }

        /// <summary>
        /// D_TP1AP24 X(60)  
        /// * COMUNE DI NASC ESTESO
        /// </summary>
        [HisFieldInfoMapping(21, 60)]
        public string D_TP1AP24 { get; set; }

        /// <summary>
        /// D_TP1GP1AP25 XXX  
        /// * PROVINCIA DI NASC ESTESO
        /// </summary>
        [HisFieldInfoMapping(22, 3)]
        public string D_TP1GP1AP25 { get; set; }

        #endregion Tracciato Host
    }
}
