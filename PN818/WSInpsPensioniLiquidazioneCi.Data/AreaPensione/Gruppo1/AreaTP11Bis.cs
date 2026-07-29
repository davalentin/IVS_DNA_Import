using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaTP11Bis
    {
        #region tracciato COBOL
        //             04  AREATP11.
        //     05  TP1SEDE                        PIC 9(4) COMP-3.  3
        //     05  TP1NDOM                        PIC 9(8) COMP-3.  5 
        //* NUMERO DOMANDA
        //     05  TP1ELABR.
        //         10  TP1ELABA                   PIC 9999.
        //         10  TP1ELABM                   PIC 99.
        //         10  TP1ELABG                   PIC 99.
        //* DATA ELABORAZIONE
        //     05  TP1CPATR                       PIC 99.
        //* COD. PATRONATO
        //     05  TP1ZPATR                       PIC X.
        //* ZONA PATRONATO
        //     05  TP1NFAM                        PIC 9(2).
        //      05  TP1COG1                       PIC X(32).
        // * COGNOME
        //      05  TP1NOM1                       PIC X(32).
        // * NOME
        //      05  TP1COAC                       PIC X(32).
        // * COGNOME ACQUISITO
        //      05  TP1STACIV                      PIC X.
        // * STATO CIVILE
        //      05  TP1PR-EX                          PIC XX.
        // * COD. PROVINCIA
        //      05  TP1CO                          PIC 9(5).
        // * COD. COMUNE
        // *
        // *20.07.2001
        //      05  INDIRIZZO-TIT.
        // *INDIRIZZO TITOLARE
        //         10  TP1VIA1                      PIC X(52).
        //         10  TP1VIA2                      PIC X(52).
        //         10  TP1VIA3                      PIC X(52).
        //         10  TP1VIA4                      PIC X(52).
        // * VIA
        //         10  TP1CIVICO                    PIC X(18).
        // * NUM. CIVICO -
        //         10  TP1FRAZIO                    PIC X(34).
        // * FRAZIONE
        //         10  TP1ESTITA                    PIC X(1).
        // * 1=INDIR.ITA    9=IND.EST
        //         10  TP1CAPRS                     PIC X(9).
        // * C.A.P.
        //         10  TP1COMUN                     PIC X(37).
        // * COMUNE DI RESIDENZA
        //         10  TP1PROV                      PIC XXX.
        // * PROVINCIA DI RESIDENZA
        //         10  TP1STATO                     PIC XXX.
        // * STATO DI RESIDENZA
        //         10  TP1CITT1                     PIC XXX.
        // * CITTADINANZA
        #endregion tracciato COBOL

        #region Tracciato Host
        // 04  AREATP11.
        /// <summary>
        /// TP1SEDE 9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(0, 10)]
        public string ESITO { get; set; }

        /// <summary>
        /// TP1NDOM 9(8) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(1, 2)]
        public string PRESENZA_PENSIONE { get; set; }

        // * NUMERO DOMANDA
        // 05  TP1ELABR.
        /// <summary>
        /// TP1ELABA 9999  
        /// </summary>
        [HisFieldInfoMapping(2, 4)]
        public short TP1ELABA { get; set; }

        /// <summary>
        /// TP1ELABM 99  
        /// </summary>
        [HisFieldInfoMapping(3, 2)]
        public short TP1ELABM { get; set; }

        /// <summary>
        /// TP1ELABG 99  
        /// </summary>
        [HisFieldInfoMapping(4, 2)]
        public short TP1ELABG { get; set; }

        // * DATA ELABORAZIONE
        /// <summary>
        /// TP1CPATR 99  
        /// </summary>
        [HisFieldInfoMapping(5, 2)]
        public short TP1CPATR { get; set; }

        // * COD. PATRONATO
        /// <summary>
        /// TP1ZPATR X  
        /// </summary>
        [HisFieldInfoMapping(6, 1)]
        public string TP1ZPATR { get; set; }

        // * ZONA PATRONATO
        /// <summary>
        /// TP1NFAM 9(2)  
        /// </summary>
        [HisFieldInfoMapping(7, 2)]
        public short TP1NFAM { get; set; }

        /// <summary>
        /// TP1COG1 X(32)  
        /// * COGNOME
        /// </summary>
        [HisFieldInfoMapping(8, 32)]
        public string TP1COG1 { get; set; }

        /// <summary>
        /// TP1NOM1 X(32)  
        /// * NOME
        /// </summary>
        [HisFieldInfoMapping(9, 32)]
        public string TP1NOM1 { get; set; }

        /// <summary>
        /// TP1COAC X(32)  
        /// * COGNOME ACQUISITO
        /// </summary>
        [HisFieldInfoMapping(10, 32)]
        public string TP1COAC { get; set; }

        /// <summary>
        /// TP1STACIV X  
        /// * STATO CIVILE
        /// </summary>
        [HisFieldInfoMapping(11, 1)]
        public string TP1STACIV { get; set; }

        /// <summary>
        /// TP1PR_EX XX  
        /// * COD. PROVINCIA
        /// </summary>
        [HisFieldInfoMapping(12, 2)]
        public string TP1PR_EX { get; set; }

        /// <summary>
        /// TP1CO 9(5)  
        /// * COD. COMUNE
        /// </summary>
        [HisFieldInfoMapping(13, 5)]
        public int TP1CO { get; set; }

        //*
        // *20.07.2001
        // 05  INDIRIZZO-TIT.
        // *INDIRIZZO TITOLARE
        /// <summary>
        /// TP1VIA1 X(52)  
        /// * VIA
        /// </summary>
        [HisFieldInfoMapping(14, 52)]
        public string TP1VIA1 { get; set; }

        /// <summary>
        /// TP1VIA2 X(52)
        /// * VIA
        /// </summary>
        [HisFieldInfoMapping(15, 52)]
        public string TP1VIA2 { get; set; }

        /// <summary>
        /// TP1VIA3 X(52)  
        /// * VIA
        /// </summary>
        [HisFieldInfoMapping(16, 52)]
        public string TP1VIA3 { get; set; }

        /// <summary>
        /// TP1VIA4 X(52) 
        /// * VIA
        /// </summary>
        [HisFieldInfoMapping(17, 52)]
        public string TP1VIA4 { get; set; }

        /// <summary>
        /// TP1CIVICO X(18)  
        /// * NUM. CIVICO -
        /// </summary>
        [HisFieldInfoMapping(18, 18)]
        public string TP1CIVICO { get; set; }

        /// <summary>
        /// TP1FRAZIO X(34) 
        /// * FRAZIONE 
        /// </summary>
        [HisFieldInfoMapping(19, 34)]
        public string TP1FRAZIO { get; set; }

        /// <summary>
        /// TP1ESTITA X(1)  
        /// * 1=INDIR.ITA    9=IND.EST
        /// </summary>
        [HisFieldInfoMapping(20, 1)]
        public string TP1ESTITA { get; set; }

        /// <summary>
        /// TP1CAPRS X(9)  
        /// * C.A.P.
        /// </summary>
        [HisFieldInfoMapping(21, 9)]
        public string TP1CAPRS { get; set; }

        /// <summary>
        /// TP1COMUN X(37)  
        /// * COMUNE DI RESIDENZA
        /// </summary>
        [HisFieldInfoMapping(22, 37)]
        public string TP1COMUN { get; set; }

        /// <summary>
        /// TP1PROV XXX  
        /// * PROVINCIA DI RESIDENZA
        /// </summary>
        [HisFieldInfoMapping(23, 3)]
        public string TP1PROV { get; set; }

        /// <summary>
        /// TP1STATO XXX  
        /// * STATO DI RESIDENZA
        /// </summary>
        [HisFieldInfoMapping(24, 3)]
        public string TP1STATO { get; set; }

        /// <summary>
        /// TP1CITT1 XXX  
        /// * CITTADINANZA
        /// </summary>
        [HisFieldInfoMapping(25, 3)]
        public string TP1CITT1 { get; set; }

        #endregion Tracciato Host
    }
}
