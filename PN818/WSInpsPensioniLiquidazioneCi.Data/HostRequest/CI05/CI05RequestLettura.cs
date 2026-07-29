using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostRequest
{
    public class CI05RequestLettura : CI05RequestBase
{
		#region Constructor
        public CI05RequestLettura()
		{
			this.Dati = new AreaDati();
            this.Controllo = new AreaControllo();
		}
        #endregion Constructor

        #region Properties
        [HisComplexAreaInfoMapping(0)]
		public AreaControllo Controllo {get; set; }

		[HisComplexAreaInfoMapping(1)]
		public AreaDati Dati { get; set; }
        #endregion Properties

        #region Nested class
        /// <summary>
        ///  Definizione del tracciato di input
        /// </summary>
        public class AreaDati
		{
            #region Tracciato COBOL
            //       01   DATI-PER-HOST.
            //          10  CPGMDAS              PIC X(8).
            //          10  MATRICOLA-OP          PIC X(8).
            //          10  SEDE                  PIC X(6).
            //          10  ERRORE-RITORNO        PIC 99.
            //          10  ALTRI-D               PIC X.
            //          10  CHEFARE               PIC X.
            //          10  PROVA                 PIC X.
            //          10  ALTRI                 PIC X(100).
            //          10  CHIAVI.
            //            15  KEY-DA              PIC 9(8).
            //            15  KEY-A               PIC 9(8).
            //            15  NUMEDOMA            PIC 9(13).
            //            15  FILTRO01            PIC X.
            //************15  TAPPO            PIC X(200)
            //            15  TAPPO            PIC X(9000)
            //               VALUE '0000000000000000000000000000000000000'.
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
            /// KEY_DA 9(8)  
            /// </summary>
            [HisFieldInfoMapping(8, 8)]
            public int KEY_DA { get; set; }

            /// <summary>
            /// KEY_A 9(8)  
            /// </summary>
            [HisFieldInfoMapping(9, 8)]
            public int KEY_A { get; set; }

            /// <summary>
            /// NUMEDOMA 9(13)  
            /// </summary>
            [HisFieldInfoMapping(10, 13)]
            public long NUMEDOMA { get; set; }

            /// <summary>
            /// FILTRO01 X  
            /// </summary>
            [HisFieldInfoMapping(11, 1)]
            public string FILTRO01 { get; set; }

            /// <summary>
            /// TAPPO X(9000)  
            /// </summary>
            [HisFieldInfoMapping(12, 9000)]
            public string TAPPO { get; set; }

            /// <summary>
            /// riempimento 70 byte mancanti 
            /// </summary>
            [HisFieldInfoMapping(13, 70)]
            public string FILLER { get; set; }
            #endregion Tracciato Host
        }
        #endregion Nested class
    }
}
