using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostRequest
{
    [Serializable] 
    public class GAINRequest
    {
        #region Constructor
        internal GAINRequest()
        {
            this.Controllo = new AreaControllo();
        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //           03 FILLER PIC X(11).
        //    03 RICH-INPUT.
        //       05 TIPO-RICHIESTA      PIC  X(02).
        //* CODICE DELLA PENSIONE
        //       05 CODICE-IDENTIFICATIVO.
        //          07 COD-CATEGORIA    PIC  X(03).
        //          07 COD-SEDE         PIC  9(04).
        //          07 CERTIFICATO      PIC  9(08).
        //          07 CATEGORIA-CHIARO PIC  X(06).
        //    03 RICH-OUTPUT.
        //*
        //* CODICE E MESSAGGIO DI RITORNO, IMPOSTATI DAI PGM:
        //* RC021500 / RC021501 / PDBBITX3 / PDBCOMXE
        //* - VALORI CHE PUO' ASSUMERE IL CAMPO "COD-RIT" :
        //*  - '00'= OK
        //*  - '01'= ERRORE RILEVATO DAL PGM RC021500/RC021501/PDBBITX3
        //*  - '02'= ERRORE RILEVATO DAL PGM PDBCOMXE(VUOL DIRE CHE ABBIAMO
        //*           ULTERIORI INFORMAZIONI NEI CAMPI "MSG-PDBCOMXE")
        //*  - '03'= ERRORE RILEVATO DAL PGM APPC SU AS400 (PGM: PRCAPPC)
        //*
        //       05 COD-RIT             PIC  X(02).
        //       05 MSG-RIT             PIC  X(80).
        //*
        //* FINE-MSG = 'SI' (VUOL DIRE CHE NON CI SONO PIU' CODE DA
        //*                  RICEVERE DA HOST)
        //       05 FINE-MSG            PIC  XX.
        //*
        //       05 MSG-PDBCOMXE.
        //* CODICE ERRORE                       (RICEVUTO DAL PGM PDBCOMXE)
        //          07 RETCODE          PIC  99999.
        //*
        //* TIPO ERRORE                         (RICEVUTO DAL PGM PDBCOMXE)
        //*   --->  W=WARNING   E=ERRORE BLOCCANTE   S=SQLCODE
        //          07 TIPO-RETCODE     PIC  X(01).
        //*
        //* DESCRIZIONE DELL'ERRORE RISCONTRATO (RICEVUTO DAL PGM PDBCOMXE)
        //          07 DESC-ERRORE      PIC  X(70).
        //*
        //          07 TIPO-ACCESSO     PIC  X(06).
        //*
        //          07 TAB-ERRORE       PIC  X(08).
        //*
        //          07 PGM-ERRORE       PIC  X(08).
        //*
        //* BYTE NON UTILIZZATI (A DISPOSIZIONE).
        //    03 FILLER                 PIC  X(34).
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// FILLER X(1)  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string FILLER { get; set; }

        [HisComplexAreaInfoMapping(1)]
        public AreaControllo Controllo { get; internal set; }

        #endregion Tracciato Host

        #region nested class
        public class AreaControllo
        {
            #region Constructor
            internal AreaControllo()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //03 FILLER PIC X(10).
            //    03 RICH-INPUT.
            //       05 TIPO-RICHIESTA      PIC  X(02).
            //* CODICE DELLA PENSIONE
            //       05 CODICE-IDENTIFICATIVO.
            //          07 COD-CATEGORIA    PIC  X(03).
            //          07 COD-SEDE         PIC  9(04).
            //          07 CERTIFICATO      PIC  9(08).
            //          07 CATEGORIA-CHIARO PIC  X(06).
            //    03 RICH-OUTPUT.
            //*
            //* CODICE E MESSAGGIO DI RITORNO, IMPOSTATI DAI PGM:
            //* RC021500 / RC021501 / PDBBITX3 / PDBCOMXE
            //* - VALORI CHE PUO' ASSUMERE IL CAMPO "COD-RIT" :
            //*  - '00'= OK
            //*  - '01'= ERRORE RILEVATO DAL PGM RC021500/RC021501/PDBBITX3
            //*  - '02'= ERRORE RILEVATO DAL PGM PDBCOMXE(VUOL DIRE CHE ABBIAMO
            //*           ULTERIORI INFORMAZIONI NEI CAMPI "MSG-PDBCOMXE")
            //*  - '03'= ERRORE RILEVATO DAL PGM APPC SU AS400 (PGM: PRCAPPC)
            //*
            //       05 COD-RIT             PIC  X(02).
            //       05 MSG-RIT             PIC  X(80).
            //*
            //* FINE-MSG = 'SI' (VUOL DIRE CHE NON CI SONO PIU' CODE DA
            //*                  RICEVERE DA HOST)
            //       05 FINE-MSG            PIC  XX.
            //*
            //       05 MSG-PDBCOMXE.
            //* CODICE ERRORE                       (RICEVUTO DAL PGM PDBCOMXE)
            //          07 RETCODE          PIC  99999.
            //*
            //* TIPO ERRORE                         (RICEVUTO DAL PGM PDBCOMXE)
            //*   --->  W=WARNING   E=ERRORE BLOCCANTE   S=SQLCODE
            //          07 TIPO-RETCODE     PIC  X(01).
            //*
            //* DESCRIZIONE DELL'ERRORE RISCONTRATO (RICEVUTO DAL PGM PDBCOMXE)
            //          07 DESC-ERRORE      PIC  X(70).
            //*
            //          07 TIPO-ACCESSO     PIC  X(06).
            //*
            //          07 TAB-ERRORE       PIC  X(08).
            //*
            //          07 PGM-ERRORE       PIC  X(08).
            //*
            //* BYTE NON UTILIZZATI (A DISPOSIZIONE).
            //    03 FILLER                 PIC  X(34).
            #endregion Tracciato COBOL

            #region Tracciato Host
            [HisFieldInfoMapping(0, 10)]
            public string FILLER1 { get; set; }

            // 03 RICH-INPUT.
            /// <summary>
            /// TIPO_RICHIESTA X(02)  
            /// </summary>
            [HisFieldInfoMapping(1, 2)]
            public string TIPO_RICHIESTA { get; set; }

            // * CODICE DELLA PENSIONE
            // 05 CODICE-IDENTIFICATIVO.
            /// <summary>
            /// COD_CATEGORIA X(03)  
            /// </summary>
            [HisFieldInfoMapping(2, 3)]
            public string COD_CATEGORIA { get; set; }

            /// <summary>
            /// COD_SEDE 9(04)  
            /// </summary>
            [HisFieldInfoMapping(3, 4, CobolType = CobolType.Unsigned)]
            public short COD_SEDE { get; set; }

            /// <summary>
            /// CERTIFICATO 9(08)  
            /// </summary>
            [HisFieldInfoMapping(4, 8, CobolType = CobolType.Unsigned)]
            public int CERTIFICATO { get; set; }

            /// <summary>
            /// CATEGORIA_CHIARO X(06)  
            /// </summary>
            [HisFieldInfoMapping(5, 6)]
            public string CATEGORIA_CHIARO { get; set; }

            // 03 RICH-OUTPUT.
            //*
            // * CODICE E MESSAGGIO DI RITORNO, IMPOSTATI DAI PGM:
            // * RC021500 / RC021501 / PDBBITX3 / PDBCOMXE
            // * - VALORI CHE PUO' ASSUMERE IL CAMPO "COD-RIT" :
            // *  - '00'= OK
            // *  - '01'= ERRORE RILEVATO DAL PGM RC021500/RC021501/PDBBITX3
            // *  - '02'= ERRORE RILEVATO DAL PGM PDBCOMXE(VUOL DIRE CHE ABBIAMO
            // *           ULTERIORI INFORMAZIONI NEI CAMPI "MSG-PDBCOMXE")
            // *  - '03'= ERRORE RILEVATO DAL PGM APPC SU AS400 (PGM: PRCAPPC)
            //*
            /// <summary>
            /// COD_RIT X(02)  
            /// </summary>
            [HisFieldInfoMapping(6, 2)]
            public string COD_RIT { get; set; }

            /// <summary>
            /// MSG_RIT X(80)  
            /// </summary>
            [HisFieldInfoMapping(7, 80)]
            public string MSG_RIT { get; set; }

            //*
            // * FINE-MSG = 'SI' (VUOL DIRE CHE NON CI SONO PIU' CODE DA
            // *                  RICEVERE DA HOST)
            /// <summary>
            /// FINE_MSG XX  
            /// </summary>
            [HisFieldInfoMapping(8, 2)]
            public string FINE_MSG { get; set; }

            //*
            // 05 MSG-PDBCOMXE.
            // * CODICE ERRORE                       (RICEVUTO DAL PGM PDBCOMXE)
            /// <summary>
            /// RETCODE 99999  
            /// </summary>
            [HisFieldInfoMapping(9, 5, CobolType = CobolType.Unsigned)]
            public int RETCODE { get; set; }

            //*
            // * TIPO ERRORE                         (RICEVUTO DAL PGM PDBCOMXE)
            // *   --->  W=WARNING   E=ERRORE BLOCCANTE   S=SQLCODE
            /// <summary>
            /// TIPO_RETCODE X(01)  
            /// </summary>
            [HisFieldInfoMapping(10, 1)]
            public string TIPO_RETCODE { get; set; }

            //*
            // * DESCRIZIONE DELL'ERRORE RISCONTRATO (RICEVUTO DAL PGM PDBCOMXE)
            /// <summary>
            /// DESC_ERRORE X(70)  
            /// </summary>
            [HisFieldInfoMapping(11, 70)]
            public string DESC_ERRORE { get; set; }

            //*
            /// <summary>
            /// TIPO_ACCESSO X(06)  
            /// </summary>
            [HisFieldInfoMapping(12, 6)]
            public string TIPO_ACCESSO { get; set; }

            //*
            /// <summary>
            /// TAB_ERRORE X(08)  
            /// </summary>
            [HisFieldInfoMapping(13, 8)]
            public string TAB_ERRORE { get; set; }

            //*
            /// <summary>
            /// PGM_ERRORE X(08)  
            /// </summary>
            [HisFieldInfoMapping(14, 8)]
            public string PGM_ERRORE { get; set; }

            //*
            /// <summary>
            /// ANNO-ELAB 9(04)  
            /// </summary>
            [HisFieldInfoMapping(15, 4)]
            public int ANNO_ELAB { get; set; }

            //*
            // * BYTE NON UTILIZZATI (A DISPOSIZIONE).
            /// <summary>
            /// FILLER X(30)  
            /// </summary>
            [HisFieldInfoMapping(16, 30)]
            public string FILLER2 { get; set; }
            #endregion Tracciato Host
            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
