using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaTP12
    {
        #region tracciato COBOL
        //      04  AREATP12.
        //      05  TP1COFI                        PIC X(16).
        // * CODICE FISCALE
        //           10  TP1CODEN.
        // *CODICE DETRAZIONE IMPOSTA NUOVI 12/2002
        //               15 CO1N                         PIC 9.
        //               15 CO2N                         PIC 9.
        //               15 CO3N                         PIC 9.
        //               15 CO4N                         PIC 9.
        //               15 CO5N                         PIC 9.
        //               15 CO6N                         PIC 9.
        //               15 CO7N                         PIC 9.
        //               15 CO8N                         PIC 9.
        //               15 CO9N                         PIC 9.
        //               15 CO10N                        PIC 9.
        //               15 CO11N                        PIC 9.
        //               15 CO12N                        PIC 9.
        //               15 CO13N                        PIC 9.
        //               15 CO14N                        PIC 9.
        //           10  FILLER                     PIC X(4).
        // *
        //      05  TP1CORIC                       PIC 9.
        // * 1=PRIMA ISTANZA 2=RICORSO 7=RIESAME 5=CAUSA ACCOLTA DALLA MAGIST
        // *    05  TP1TRSE                        PIC S9(9) COMP-3.
        //      05  TP1TRSE                        PIC S9(7)V9(4) COMP-3.
        // *EURO TRATTENUTE TRATTENUTE DEDUCIBILI IRPEF ANNO IN CORSO
        // *    05  TP1IACC                        PIC S9(9) COMP-3.
        //      05  TP1IACC                        PIC S9(7)V9(4) COMP-3.
        // *EURO ACCONTI /  IMPORTO DEL RECUPERO DA EFFETTUERE
        //      05  TP1ACC                         PIC 9.
        // * COD. ACCANTONAMENTO 1/3
        //      05  TP1CDCM                        PIC XX.
        // * CD/CM/MR
        //      05  TP1ATEC                        PIC S9(2) COMP-3.
        // * ATTIVITA ECONOMICA
        //      05  TP1PRIN                        PIC S9(3) COMP-3.
        // * PROFESSIONE INDIVIDUALE
        //      05  TP1CLIV.
        //          10  TP1CLIV1                   PIC 9(3).
        // * 1° NUMERO NOSOLOGICO
        //          10  TP1CLIV2                   PIC 9(3).
        // * 2° NUMERO NOSOLOGICO (IN MANCANZA 777)
        //      05  TP1REVR.
        //          10  TP1REVA                    PIC 9999.
        //          10  TP1REVM                    PIC 99.
        // * SCADENZA REVISIONE MEDICA SE DIVERSA DA SCADENZA TRIENNALE
        //      05  TP1RESDC                       PIC X(3).
        // * SIGLA STAT O DI DECESSO DEL DANTE CAUSA
        //      05  TP1COGDC                       PIC X(32).
        // * COGNOME DANTE CAUSA
        //      05  TP1NOMDC                       PIC X(32).
        // * NOME DANTE CAUSA
        //      05  TP1COMDC                       PIC 9(5).
        // * CODICE COMUNE DANTE CAUSA
        //      05  TP1SEDED                       PIC 9(4).
        // * SEDE DIRETTA
        //      05  TP1CATD                        PIC 9(3).
        // * CATEGORIA PENSIONE DIRETTA
        //      05  TP1CERTD                       PIC 9(8).
        // * CERT. PENSIONE DIRETTA
        //      05  TP1ELIMR.
        //          10  TP1ELIMA                   PIC 9999.
        //          10  TP1ELIMM                   PIC 99.
        // * DATA ELIMINAZIONE
        //      05  TP1URR.
        //          10  TP1URA                     PIC 9999.
        //          10  TP1URM                     PIC 99.
        //          10  TP1URG                     PIC 99.
        // * DATA ULTIMA RISCOSSIONE
        // *    05  TP1MENT                        PIC S9(8) COMP-3.
        // *MENSILE PENSIONE DIRETTA
        //      05  TP1ILEGR.
        //          10  TP1ILEGA                   PIC 9999.
        //          10  TP1ILEGM                   PIC 99.
        //          10  TP1ILEGG                   PIC 99.
        // * DEC. INTERESSI LEGALI
        // *
        //          10  TP1RED30RA                 PIC 9999.
        //          10  TP1RED30RM                 PIC 99.
        // * DATA SOSPENSIONE (MMAAAA)
        //          10  TP1RED40RA                 PIC 9999.
        //          10  TP1RED40RM                 PIC 99.
        // * DATA RIPRISTINO (MMAAAA)
        #endregion tracciato COBOL

        #region Tracciato Host
        // * CITTADINANZA
        // 04  AREATP12.
        /// <summary>
        /// TP1COFI X(16)  
        /// * CODICE FISCALE
        /// </summary>
        [HisFieldInfoMapping(0, 16)]
        public string TP1COFI { get; set; }

        // 10  TP1CODEN.
        // *CODICE DETRAZIONE IMPOSTA NUOVI 12/2002
        /// <summary>
        /// CO1N 9  
        /// </summary>
        [HisFieldInfoMapping(1, 1)]
        public short CO1N { get; set; }

        /// <summary>
        /// CO2N 9  
        /// </summary>
        [HisFieldInfoMapping(2, 1)]
        public short CO2N { get; set; }

        /// <summary>
        /// CO3N 9  
        /// </summary>
        [HisFieldInfoMapping(3, 1)]
        public short CO3N { get; set; }

        /// <summary>
        /// CO4N 9  
        /// </summary>
        [HisFieldInfoMapping(4, 1)]
        public short CO4N { get; set; }

        /// <summary>
        /// CO5N 9  
        /// </summary>
        [HisFieldInfoMapping(5, 1)]
        public short CO5N { get; set; }

        /// <summary>
        /// CO6N 9  
        /// </summary>
        [HisFieldInfoMapping(6, 1)]
        public short CO6N { get; set; }

        /// <summary>
        /// CO7N 9  
        /// </summary>
        [HisFieldInfoMapping(7, 1)]
        public short CO7N { get; set; }

        /// <summary>
        /// CO8N 9  
        /// </summary>
        [HisFieldInfoMapping(8, 1)]
        public short CO8N { get; set; }

        /// <summary>
        /// CO9N 9  
        /// </summary>
        [HisFieldInfoMapping(9, 1)]
        public short CO9N { get; set; }

        /// <summary>
        /// CO10N 9  
        /// </summary>
        [HisFieldInfoMapping(10, 1)]
        public short CO10N { get; set; }

        /// <summary>
        /// CO11N 9  
        /// </summary>
        [HisFieldInfoMapping(11, 1)]
        public short CO11N { get; set; }

        /// <summary>
        /// CO12N 9  
        /// </summary>
        [HisFieldInfoMapping(12, 1)]
        public short CO12N { get; set; }

        /// <summary>
        /// CO13N 9  
        /// </summary>
        [HisFieldInfoMapping(13, 1)]
        public short CO13N { get; set; }

        /// <summary>
        /// CO14N 9  
        /// </summary>
        [HisFieldInfoMapping(14, 1)]
        public short CO14N { get; set; }

        /// <summary>
        /// FILLER X(4)  
        /// </summary>
        [HisFieldInfoMapping(15, 4)]
        public string FILLER { get; set; }

        //*
        /// <summary>
        /// TP1CORIC 9  
        /// * 1=PRIMA ISTANZA 2=RICORSO 7=RIESAME 5=CAUSA ACCOLTA DALLA MAGIST
        /// </summary>
        [HisFieldInfoMapping(16, 1)]
        public short TP1CORIC { get; set; }

        /// <summary>
        /// TP1TRSE S9(7)V9(4) COMP-3 
        /// *EURO TRATTENUTE TRATTENUTE DEDUCIBILI IRPEF ANNO IN CORSO
        /// </summary>
        [HisFieldInfoMapping(17, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal TP1TRSE { get; set; }

        /// <summary>
        /// TP1IACC S9(7)V9(4) COMP-3 
        /// *EURO ACCONTI /  IMPORTO DEL RECUPERO DA EFFETTUERE
        /// </summary>
        [HisFieldInfoMapping(18, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal TP1IACC { get; set; }

        /// <summary>
        /// TP1ACC 9  
        /// * COD. ACCANTONAMENTO 1/3
        /// </summary>
        [HisFieldInfoMapping(19, 1)]
        public short TP1ACC { get; set; }

        /// <summary>
        /// TP1CDCM XX  
        /// * CD/CM/MR
        /// </summary>
        [HisFieldInfoMapping(20, 2)]
        public string TP1CDCM { get; set; }

        /// <summary>
        /// TP1ATEC S9(2) COMP-3 
        /// * ATTIVITA ECONOMICA
        /// </summary>
        [HisFieldInfoMapping(21, 2, CobolType = CobolType.Comp3)]
        public int TP1ATEC { get; set; }

        /// <summary>
        /// TP1PRIN S9(3) COMP-3 
        /// * PROFESSIONE INDIVIDUALE
        /// </summary>
        [HisFieldInfoMapping(22, 2, CobolType = CobolType.Comp3)]
        public int TP1PRIN { get; set; }

        // 05  TP1CLIV.
        /// <summary>
        /// TP1CLIV1 9(3)  
        /// * 1° NUMERO NOSOLOGICO
        /// </summary>
        [HisFieldInfoMapping(23, 3)]
        public short TP1CLIV1 { get; set; }

        /// <summary>
        /// TP1CLIV2 9(3)  
        /// * 2° NUMERO NOSOLOGICO (IN MANCANZA 777)
        /// </summary>
        [HisFieldInfoMapping(24, 3)]
        public short TP1CLIV2 { get; set; }

        // 05  TP1REVR.
        /// <summary>
        /// TP1REVA 9999  
        /// * SCADENZA REVISIONE MEDICA SE DIVERSA DA SCADENZA TRIENNALE
        /// </summary>
        [HisFieldInfoMapping(25, 4)]
        public short TP1REVA { get; set; }

        /// <summary>
        /// TP1REVM 99  
        /// * SCADENZA REVISIONE MEDICA SE DIVERSA DA SCADENZA TRIENNALE
        /// </summary>
        [HisFieldInfoMapping(26, 2)]
        public short TP1REVM { get; set; }

        /// <summary>
        /// TP1RESDC X(3)  
        /// * SIGLA STAT O DI DECESSO DEL DANTE CAUSA
        /// </summary>
        [HisFieldInfoMapping(27, 3)]
        public string TP1RESDC { get; set; }

        /// <summary>
        /// TP1COGDC X(32)  
        /// * COGNOME DANTE CAUSA
        /// </summary>
        [HisFieldInfoMapping(28, 32)]
        public string TP1COGDC { get; set; }

        /// <summary>
        /// TP1NOMDC X(32)  
        /// * NOME DANTE CAUSA
        /// </summary>
        [HisFieldInfoMapping(29, 32)]
        public string TP1NOMDC { get; set; }

        /// <summary>
        /// TP1COMDC 9(5)  
        /// * CODICE COMUNE DANTE CAUSA
        /// </summary>
        [HisFieldInfoMapping(30, 5)]
        public int TP1COMDC { get; set; }

        /// <summary>
        /// TP1SEDED 9(4)  
        /// * SEDE DIRETTA
        /// </summary>
        [HisFieldInfoMapping(31, 4)]
        public short TP1SEDED { get; set; }

        /// <summary>
        /// TP1CATD 9(3)  
        /// * CATEGORIA PENSIONE DIRETTA
        /// </summary>
        [HisFieldInfoMapping(32, 3)]
        public short TP1CATD { get; set; }

        /// <summary>
        /// TP1CERTD 9(8)  
        /// * CERT. PENSIONE DIRETTA
        /// </summary>
        [HisFieldInfoMapping(33, 8)]
        public int TP1CERTD { get; set; }

        // 05  TP1ELIMR.
        /// <summary>
        /// TP1ELIMA 9999  
        /// * DATA ELIMINAZIONE
        /// </summary>
        [HisFieldInfoMapping(34, 4)]
        public short TP1ELIMA { get; set; }

        /// <summary>
        /// TP1ELIMM 99  
        /// * DATA ELIMINAZIONE
        /// </summary>
        [HisFieldInfoMapping(35, 2)]
        public short TP1ELIMM { get; set; }

        // 05  TP1URR.
        /// <summary>
        /// TP1URA 9999  
        /// * DATA ULTIMA RISCOSSIONE
        /// </summary>
        [HisFieldInfoMapping(36, 4)]
        public short TP1URA { get; set; }

        /// <summary>
        /// TP1URM 99  
        /// * DATA ULTIMA RISCOSSIONE
        /// </summary>
        [HisFieldInfoMapping(37, 2)]
        public short TP1URM { get; set; }

        /// <summary>
        /// TP1URG 99  
        /// * DATA ULTIMA RISCOSSIONE
        /// </summary>
        [HisFieldInfoMapping(38, 2)]
        public short TP1URG { get; set; }

        // 05  TP1ILEGR.
        /// <summary>
        /// TP1ILEGA 9999  
        /// * DEC. INTERESSI LEGALI
        /// </summary>
        [HisFieldInfoMapping(39, 4)]
        public short TP1ILEGA { get; set; }

        /// <summary>
        /// TP1ILEGM 99  
        /// * DEC. INTERESSI LEGALI
        /// </summary>
        [HisFieldInfoMapping(40, 2)]
        public short TP1ILEGM { get; set; }

        /// <summary>
        /// TP1ILEGG 99  
        /// * DEC. INTERESSI LEGALI
        /// </summary>
        [HisFieldInfoMapping(41, 2)]
        public short TP1ILEGG { get; set; }

        //*
        /// <summary>
        /// TP1RED30RA 9999  
        /// * DATA SOSPENSIONE (MMAAAA)
        /// </summary>
        [HisFieldInfoMapping(42, 4)]
        public short TP1RED30RA { get; set; }

        /// <summary>
        /// TP1RED30RM 99  
        /// * DATA SOSPENSIONE (MMAAAA)
        /// </summary>
        [HisFieldInfoMapping(43, 2)]
        public short TP1RED30RM { get; set; }

        /// <summary>
        /// TP1RED40RA 9999  
        /// * DATA RIPRISTINO (MMAAAA)
        /// </summary>
        [HisFieldInfoMapping(44, 4)]
        public short TP1RED40RA { get; set; }

        /// <summary>
        /// TP1RED40RM 99  
        /// * DATA RIPRISTINO (MMAAAA)
        /// </summary>
        [HisFieldInfoMapping(45, 2)]
        public short TP1RED40RM { get; set; }


        #endregion Tracciato Host
    }
}

