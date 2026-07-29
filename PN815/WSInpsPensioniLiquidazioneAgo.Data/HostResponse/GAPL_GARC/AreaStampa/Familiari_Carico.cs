using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostResponse.AreaStampa
{
    public class Familiari_Carico
    {
        #region Constructor
        internal Familiari_Carico()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 FAM-COMP.
        //*                          COMPOSIZIONE FAMILIARE           5960
        //        03 NUM-FRA       PIC 9(01).
        //*                          ASCENDENTI/FRATELLI              5960
        //        03 CONIUGE       PIC 9(01).
        //*                          CONIUGE                          5961
        //        03 NUM-FIGLI     PIC 9(02).
        //*                          FIGLI                            5962
        //     02 FAM-NUM          PIC 9(02).
        //*                          NUMERO FAMILIARI                 5964
        //     02 FILLER           PIC X(04).
        //*                          LIBERI                           5966
        //     02 FAM-COGNOME      PIC X(36)              OCCURS 15 TIMES.
        //*                          COGNOME FAMILIARE                5970
        //     02 FAM-NOME         PIC X(36)              OCCURS 15 TIMES.
        //*                          NOME FAMILIARE                   6510
        //     02 FAM-SEX          PIC X(01)              OCCURS 15 TIMES.
        //*                          SESSO                            7050
        //     02 FAM-SIGLA        PIC X(01)              OCCURS 15 TIMES.
        //*                          SIGLA                            7065
        //     02 FAM-NASC         PIC 9(08)              OCCURS 15 TIMES.
        //*                          DATA NASC.(GGMMAAAA)             7080
        //     02 FAM-MAGG         PIC 9(05)V9(04) COMP-3 OCCURS 15 TIMES.
        //*                          AGGIUNTA DI FAMIGLIA             7200
        //     02 FAM-SCAD                                OCCURS 15 TIMES.
        //        03 FAM-ASCAD     PIC 9(04).
        //        03 FAM-MSCAD     PIC 9(02).
        //*                          SCADENZA (AAAAMM)                7275
        //     02 FAM-DEC                                 OCCURS 15 TIMES.
        //        03 FAM-ADEC      PIC 9(04).
        //        03 FAM-MDEC      PIC 9(02).
        //*                          DECORRENZA (AAAAMM)              7365
        //     02 FILLER           PIC X(45).
        //*                          LIBERI                           7455
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 02 FAM-COMP.
        // *                          COMPOSIZIONE FAMILIARE           5960
        /// <summary>
        /// NUM_FRA 9(01)  
        /// </summary>
        [HisFieldInfoMapping(0, 1, CobolType = CobolType.Unsigned)]
        public short NUM_FRA { get; set; }

        // *                          ASCENDENTI/FRATELLI              5960
        /// <summary>
        /// CONIUGE 9(01)  
        /// </summary>
        [HisFieldInfoMapping(1, 1, CobolType = CobolType.Unsigned)]
        public short CONIUGE { get; set; }

        // *                          CONIUGE                          5961
        /// <summary>
        /// NUM_FIGLI 9(02)  
        /// </summary>
        [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
        public short NUM_FIGLI { get; set; }

        // *                          FIGLI                            5962
        /// <summary>
        /// FAM_NUM 9(02)  
        /// </summary>
        [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
        public short FAM_NUM { get; set; }

        // *                          NUMERO FAMILIARI                 5964
        /// <summary>
        /// FILLER X(04)  
        /// </summary>
        [HisFieldInfoMapping(4, 4)]
        public string FILLER1 { get; set; }

        // *                          LIBERI                           5966

        [HisComplexAreaInfoMapping(5, ListCount = 15)]
        public List<Cognome> LISTCognome { get; internal set; }

        [HisComplexAreaInfoMapping(6, ListCount = 15)]
        public List<Nome> LISTNome { get; internal set; }

        [HisComplexAreaInfoMapping(7, ListCount = 15)]
        public List<Sesso> LISTSesso { get; internal set; }

        [HisComplexAreaInfoMapping(8, ListCount = 15)]
        public List<Sigla> LISTSigla { get; internal set; }

        [HisComplexAreaInfoMapping(9, ListCount = 15)]
        public List<DataNascita> LISTDataNascita { get; internal set; }

        [HisComplexAreaInfoMapping(10, ListCount = 15)]
        public List<Aggiunta> LISTAggiunta { get; internal set; }

        [HisComplexAreaInfoMapping(11, ListCount = 15)]
        public List<Scadenza> LISTScadenza { get; internal set; }

        [HisComplexAreaInfoMapping(12, ListCount = 15)]
        public List<Decorrenza> LISTDecorrenza { get; internal set; }

        /// <summary>
        /// FILLER X(45)  
        /// </summary>
        [HisFieldInfoMapping(13, 45)]
        public string FILLER2 { get; set; }

        // *                          LIBERI                           7455

        #endregion Tracciato Host

        #region nested class
        public class Cognome
        {
            #region Constructor
            internal Cognome()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 FAM-COGNOME      PIC X(36)              OCCURS 15 TIMES.
            //*                          COGNOME FAMILIARE                5970
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// FAM_COGNOME X(36)  
            /// </summary>
            [HisFieldInfoMapping(0, 36)]
            public string FAM_COGNOME { get; set; }

            // *                          COGNOME FAMILIARE                5970
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Nome
        {
            #region Constructor
            internal Nome()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 FAM-NOME         PIC X(36)              OCCURS 15 TIMES.
            //*                          NOME FAMILIARE                   6510
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// FAM_NOME X(36)  
            /// </summary>
            [HisFieldInfoMapping(0, 36)]
            public string FAM_NOME { get; set; }

            // *                          NOME FAMILIARE                   6510
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Sesso
        {
            #region Constructor
            internal Sesso()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 FAM-SEX          PIC X(01)              OCCURS 15 TIMES.
            //*                          SESSO                            7050
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// FAM_SEX X(01)  
            /// </summary>
            [HisFieldInfoMapping(0, 1)]
            public string FAM_SEX { get; set; }

            // *                          SESSO                            7050
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Sigla
        {
            #region Constructor
            internal Sigla()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 FAM-SIGLA        PIC X(01)              OCCURS 15 TIMES.
            //*                          SIGLA                            7065
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// FAM_SIGLA X(01)  
            /// </summary>
            [HisFieldInfoMapping(0, 1)]
            public string FAM_SIGLA { get; set; }

            // *                          SIGLA                            7065
            #endregion Tracciato Host

            #endregion Properties
        }

        public class DataNascita
        {
            #region Constructor
            internal DataNascita()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 FAM-NASC         PIC 9(08)              OCCURS 15 TIMES.
            //*                          DATA NASC.(GGMMAAAA)             7080
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// FAM_NASC 9(08)  
            /// </summary>
            [HisFieldInfoMapping(0, 8, CobolType = CobolType.Unsigned)]
            public int FAM_NASC { get; set; }

            // *                          DATA NASC.(GGMMAAAA)             7080
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Aggiunta
        {
            #region Constructor
            internal Aggiunta()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 FAM-MAGG         PIC 9(05)V9(04) COMP-3 OCCURS 15 TIMES.
            //*                          AGGIUNTA DI FAMIGLIA             7200
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// FAM_MAGG 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal FAM_MAGG { get; set; }

            // *                          AGGIUNTA DI FAMIGLIA             7200
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Scadenza
        {
            #region Constructor
            internal Scadenza()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 FAM-SCAD                                OCCURS 15 TIMES.
            //        03 FAM-ASCAD     PIC 9(04).
            //        03 FAM-MSCAD     PIC 9(02).
            //*                          SCADENZA (AAAAMM)                7275
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 FAM-SCAD                                OCCURS 15 TIMES.
            /// <summary>
            /// FAM_ASCAD 9(04)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short FAM_ASCAD { get; set; }

            /// <summary>
            /// FAM_MSCAD 9(02)  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short FAM_MSCAD { get; set; }

            // *                          SCADENZA (AAAAMM)                7275
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Decorrenza
        {
            #region Constructor
            internal Decorrenza()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 FAM-DEC                                 OCCURS 15 TIMES.
            //        03 FAM-ADEC      PIC 9(04).
            //        03 FAM-MDEC      PIC 9(02).
            //*                          DECORRENZA (AAAAMM)              7365
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 FAM-DEC                                 OCCURS 15 TIMES.
            /// <summary>
            /// FAM_ADEC 9(04)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short FAM_ADEC { get; set; }

            /// <summary>
            /// FAM_MDEC 9(02)  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short FAM_MDEC { get; set; }

            // *                          DECORRENZA (AAAAMM)              7365
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
