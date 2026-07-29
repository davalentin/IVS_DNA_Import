using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class CI05AreaDanteCausa
    {
        #region Constructor
        internal CI05AreaDanteCausa()
        {
        }
        #endregion Constructor

        #region tracciato COBOL
        //  02  A-AREA-DANTE-CAUSA.
        //       03  A-FLAG-DC                       PIC X.
        //  *PRESENZA DATI DANTE CAUSA: 1=DATI PRESENTI
        //       03  CIEADDC.
        //          05 A-DATI-DC.
        //  * DATI DANTE CAUSA O CONIUGE
        //           10  A-COGDC                   PIC X(32).
        //  * COGNOME DANTE CAUSA
        //           10  A-NOMDC                   PIC X(32).
        //  * NOME DANTE CAUSA
        //           10  A-COMDC                   PIC X(20).
        //  * COMUNE NASCITA DANTE CAUSA
        //           10  A-PRODC                   PIC XXX.
        //  * PROVINCIA NASCITA DANTE CAUSA
        //           10  A-STATODC                 PIC X(3).
        //  * STATO DI NASCITA DANTE CAUSA
        //           10  A-DNASDC                  PIC 9(8).
        //           10  A-DMOR                    PIC 9(8).
        //           10  A-LUMOR                   PIC X(20).
        //  *+LUOGO DI MORTE D.C.
        //           10 A-STATO-MORTE-DC           PIC X(3).
        //  *SIGLA STATO DI MORTE DEL DANTE CAUSA
        //           10  A-SESDC                   PIC X.
        //  *+SESSO DANTE CAUSA
        //           10  A-CITTADIN-DC             PIC X(3).
        //  * CITTADINANZA DANTE CAUSA
        //           10  A-STATC-DC                PIC X.
        //  *STATO CIVILE DEL DANTE CAUSA
        //           10  A-DMAT                    PIC 9(8).
        //  *+DATA DI MATRIMONIO O DECORRENZA STATO CIVILE
        //           10  A-RELPAR                  PIC X(8).
        //  *+RELAZIONE DI PARENTELA
        //           10  A-TP1SEDED                PIC 9(4).
        //*** SEDE DIRETTA
        //           10  A-TP1CATD                 PIC 9(3).
        //*** CATEGORIA PENSIONE DIRETTA
        //           10  A-TP1CERTD                PIC 9(8).
        //*** CERT. PENSIONE DIRETTA
        //           10  A-IW1DIRET                PIC 9(6).
        //***+DEC. PENSIONE DIRETTA
        //           10  A-CODFISDC                PIC X(16).
        //           10  A-DATA-SITU                PIC 9(8).
        //           10  A-FLAG-107                 PIC X.
        //*****     0 = NON C'E' NE C'E MAI STATO UNA DATA CARICO
        //*****     1 = PRESENTE DATA CARICO IN FASE CORRENTE
        //*****     2 = PRESENTE DATA CARICO IN FASE PRECEDENTE
        //           10  FILLER                     PIC X(3).
        #endregion tracciato COBOL

        #region Tracciato Host
        // 02  A-AREA-DANTE-CAUSA.
        /// <summary>
        /// A_FLAG_DC X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string A_FLAG_DC { get; set; }

        // *PRESENZA DATI DANTE CAUSA: 1=DATI PRESENTI
        // 03  CIEADDC.
        // 05 A-DATI-DC.
        // * DATI DANTE CAUSA O CONIUGE
        /// <summary>
        /// A_COGDC X(32)  
        /// </summary>
        [HisFieldInfoMapping(1, 32)]
        public string A_COGDC { get; set; }

        // * COGNOME DANTE CAUSA
        /// <summary>
        /// A_NOMDC X(32)  
        /// </summary>
        [HisFieldInfoMapping(2, 32)]
        public string A_NOMDC { get; set; }

        // * NOME DANTE CAUSA
        /// <summary>
        /// A_COMDC X(20)  
        /// </summary>
        [HisFieldInfoMapping(3, 20)]
        public string A_COMDC { get; set; }

        // * COMUNE NASCITA DANTE CAUSA
        /// <summary>
        /// A_PRODC XXX  
        /// </summary>
        [HisFieldInfoMapping(4, 3)]
        public string A_PRODC { get; set; }

        // * PROVINCIA NASCITA DANTE CAUSA
        /// <summary>
        /// A_STATODC X(3)  
        /// </summary>
        [HisFieldInfoMapping(5, 3)]
        public string A_STATODC { get; set; }

        // * STATO DI NASCITA DANTE CAUSA
        /// <summary>
        /// A_DNASDC 9(8)  
        /// </summary>
        [HisFieldInfoMapping(6, 8)]
        public int A_DNASDC { get; set; }

        /// <summary>
        /// A_DMOR 9(8)  
        /// </summary>
        [HisFieldInfoMapping(7, 8)]
        public int A_DMOR { get; set; }

        /// <summary>
        /// A_LUMOR X(20)  
        /// </summary>
        [HisFieldInfoMapping(8, 20)]
        public string A_LUMOR { get; set; }

        // *+LUOGO DI MORTE D.C.
        /// <summary>
        /// A_STATO_MORTE_DC X(3)  
        /// </summary>
        [HisFieldInfoMapping(9, 3)]
        public string A_STATO_MORTE_DC { get; set; }

        // *SIGLA STATO DI MORTE DEL DANTE CAUSA
        /// <summary>
        /// A_SESDC X  
        /// </summary>
        [HisFieldInfoMapping(10, 1)]
        public string A_SESDC { get; set; }

        // *+SESSO DANTE CAUSA
        /// <summary>
        /// A_CITTADIN_DC X(3)  
        /// </summary>
        [HisFieldInfoMapping(11, 3)]
        public string A_CITTADIN_DC { get; set; }

        // * CITTADINANZA DANTE CAUSA
        /// <summary>
        /// A_STATC_DC X  
        /// </summary>
        [HisFieldInfoMapping(12, 1)]
        public string A_STATC_DC { get; set; }

        // *STATO CIVILE DEL DANTE CAUSA
        /// <summary>
        /// A_DMAT 9(8)  
        /// </summary>
        [HisFieldInfoMapping(13, 8)]
        public int A_DMAT { get; set; }

        // *+DATA DI MATRIMONIO O DECORRENZA STATO CIVILE
        /// <summary>
        /// A_RELPAR X(8)  
        /// </summary>
        [HisFieldInfoMapping(14, 8)]
        public string A_RELPAR { get; set; }

        // *+RELAZIONE DI PARENTELA
        /// <summary>
        /// A_TP1SEDED 9(4)  
        /// </summary>
        [HisFieldInfoMapping(15, 4)]
        public short A_TP1SEDED { get; set; }

        // *** SEDE DIRETTA
        /// <summary>
        /// A_TP1CATD 9(3)  
        /// </summary>
        [HisFieldInfoMapping(16, 3)]
        public short A_TP1CATD { get; set; }

        // *** CATEGORIA PENSIONE DIRETTA
        /// <summary>
        /// A_TP1CERTD 9(8)  
        /// </summary>
        [HisFieldInfoMapping(17, 8)]
        public int A_TP1CERTD { get; set; }

        // *** CERT. PENSIONE DIRETTA
        /// <summary>
        /// A_IW1DIRET 9(6)  
        /// </summary>
        [HisFieldInfoMapping(18, 6)]
        public int A_IW1DIRET { get; set; }

        // ***+DEC. PENSIONE DIRETTA
        /// <summary>
        /// A_CODFISDC X(16)  
        /// </summary>
        [HisFieldInfoMapping(19, 16)]
        public string A_CODFISDC { get; set; }

        /// <summary>
        /// A_DATA_SITU 9(8)  
        /// </summary>
        [HisFieldInfoMapping(20, 8)]
        public int A_DATA_SITU { get; set; }

        /// <summary>
        /// A_FLAG_107 X  
        /// </summary>
        [HisFieldInfoMapping(21, 1)]
        public string A_FLAG_107 { get; set; }

        //*****     0 = NON C'E' NE C'E MAI STATO UNA DATA CARICO
        //*****     1 = PRESENTE DATA CARICO IN FASE CORRENTE
        //*****     2 = PRESENTE DATA CARICO IN FASE PRECEDENTE
        /// <summary>
        /// FILLER X(3)  
        /// </summary>
        [HisFieldInfoMapping(22, 3)]
        public string FILLER { get; set; }


        #endregion Tracciato Host

    }
}
