using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostRequest
{
    public class CI05RequestAggiornamento: CI05RequestBase
    {
        #region Constructor
        public CI05RequestAggiornamento()
        {
            this.Controllo = new AreaControllo();
            this.Dati = new AreaDati();
        }
        #endregion Constructor

        #region Properties
        [HisComplexAreaInfoMapping(0)]
        public AreaControllo Controllo { get; set; }

        [HisComplexAreaInfoMapping(1)]
        public AreaDati Dati { get; set; }
        #endregion Properties

        #region Nested class
        /// <summary>
        ///  Definizione del tracciato di input
        /// </summary>
        public class AreaDati
        {
            #region Constructor
            public AreaDati()
            {
            }
            #endregion Constructor

            #region Tracciato COBOL
            //  01   DATI-PER-HOST.
            //10  CPGMDAS              PIC X(8).
            //10  MATRICOLA-OP          PIC X(8).
            //10  SEDE                  PIC X(6).
            //10  ERRORE-RITORNO        PIC 99.
            //10  ALTRI-D               PIC X.
            //10  CHEFARE               PIC X.
            //10  PROVA                 PIC X.
            //10  ALTRI                 PIC X(100).
            //10  CHIAVI.
            //  15  NUMEDOMA          PIC X(13).
            //  15  TIPOELAB          PIC XX.
            //  15  ESITOCI           PIC X(2).
            //  15  FILLER            PIC X(3).
            //  15  FILLER            PIC X(55).
            //  15  DATAESITO             PIC 9(8).
            //  15  REDEFINES DATAESITO.
            //    25  DATAAA       PIC 9999.
            //    25  DATAMM       PIC 99.
            //    25  DATAGG       PIC 99.
            //  15  FILLER                 PIC X(17).
            //  15  TAPPO            PIC X(9000).
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 01   DATI-PER-HOST.
            /// <summary>
            /// CPGMDAS X(8)  
            /// </summary>
            [HisFieldInfoMapping(0, 8)]
            public string CPGMDAS { get; set; }

            /// <summary>
            /// MATRICOLA_OP X(8)  
            /// </summary>
            [HisFieldInfoMapping(1, 8)]
            public string MATRICOLA_OP { get; set; }

            /// <summary>
            /// SEDE X(6)  
            /// </summary>
            [HisFieldInfoMapping(2, 6)]
            public string SEDE { get; set; }

            /// <summary>
            /// ERRORE_RITORNO 99  
            /// </summary>
            [HisFieldInfoMapping(3, 2)]
            public short ERRORE_RITORNO { get; set; }

            /// <summary>
            /// ALTRI_D X  
            /// </summary>
            [HisFieldInfoMapping(4, 1)]
            public string ALTRI_D { get; set; }

            /// <summary>
            /// CHEFARE X  
            /// </summary>
            [HisFieldInfoMapping(5, 1)]
            public string CHEFARE { get; set; }

            /// <summary>
            /// PROVA X  
            /// </summary>
            [HisFieldInfoMapping(6, 1)]
            public string PROVA { get; set; }

            /// <summary>
            /// ALTRI X(100)  
            /// </summary>
            [HisFieldInfoMapping(7, 100)]
            public string ALTRI { get; set; }

            // 10  CHIAVI.
            /// <summary>
            /// NUMEDOMA X(13)  
            /// </summary>
            [HisFieldInfoMapping(8, 13)]
            public string NUMEDOMA { get; set; }

            /// <summary>
            /// TIPOELAB XX  
            /// </summary>
            [HisFieldInfoMapping(9, 2)]
            public string TIPOELAB { get; set; }

            /// <summary>
            /// ESITOCI X(2)  
            /// </summary>
            [HisFieldInfoMapping(10, 2)]
            public string ESITOCI { get; set; }

            /// <summary>
            /// FILLER X(3)  
            /// </summary>
            [HisFieldInfoMapping(11, 3)]
            public string FILLER1 { get; set; }

            /// <summary>
            /// FILLER X(55)  
            /// </summary>
            [HisFieldInfoMapping(12, 55)]
            public string FILLER2 { get; set; }

            /// <summary>
            /// DATAAA 9999  
            /// </summary>
            [HisFieldInfoMapping(13, 4)]
            public short DATAAA { get; set; }

            /// <summary>
            /// DATAMM 99  
            /// </summary>
            [HisFieldInfoMapping(14, 2)]
            public short DATAMM { get; set; }

            /// <summary>
            /// DATAGG 99  
            /// </summary>
            [HisFieldInfoMapping(15, 2)]
            public short DATAGG { get; set; }

            /// <summary>
            /// FILLER X(17)  
            /// </summary>
            [HisFieldInfoMapping(16, 17)]
            public string FILLER3 { get; set; }

            /// <summary>
            /// TAPPO X(9000)  
            /// </summary>
            [HisFieldInfoMapping(17, 9000)]
            public string TAPPO { get; set; }

            #endregion Tracciato Host
        }
        #endregion Nested class
    }
}
