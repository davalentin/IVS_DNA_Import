using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class SPRDSC21New
    {
        #region Properties
        #region Tracciato COBOL
        //01  SPRDSC21.
        //02 T-GP4DAAA	CODICE FASCICOLO	
        //    03 T-GP4DAA1	CATEGORIA FASCICOLO	9(3)
        //    03 T-GP4DAA2	IDENTIFICATIVO FASCICOLO	
        //      04 T-GP4DAA2-1	SEDE FITTIZIA FASCICOLO (“9990”)	9(4)
        //      04 T-GP4DAA2-2	PROGRESSIVO FASCICOLO	9(8)
        //02 T-GP4DB00	TABELLA CON 25 RIPETIZIONI	
        //    03 T-GP4KA01	CATEGORIA PENSIONE LIQUIDATA	X(3)	
        //    03 T-GP4KA02	SEDE PENSIONE LIQUIDATA	X(2)	
        //    03 T-GP4KA03	ZONA PENSIONE LIQUIDATA	X(2)
        //    03 T-GP4KA04	CERTIFICATO PENSIONE LIQUIDATA	X(8)	
        //    03 T-GP4DB09	CODICE FISCALE	X(16)                                                       
        //    03 T-GP4DB13 CSOG PIC 9(9).
        //    03 T-GP4DB14	DATA DI MATRIMONIO	9(8)
        //    03 T-GP4DB15	CODICE NUCLEO	X(2)            
        //    03 FILLER	A DISPOSIZIONE 	X(50) 
        //    03 T-GP4DC00	TABELLA CON 20 RIPETIZIONI 	
        //        04 T-GP4DC01	PERCENTUALE SPETTANTE 	9(3) V9(4)	
        //        04 T-GP4DC02	DECORRENZA PERIODO (AAAA/MM)	9(6)
        //        04 T-GP4DC03	CESSAZIONE PERIODO (AAAA/MM)	9(6)
        //        04 T-GP4DC04	CODICE FAMILIARE	X(2)
        //        04 T-GP4DC05	COEFFICIENTE RIDUZIONE	9(3) V9(4)
        //        04 T-GP4DC07	PERCENTUALE GIUDICE CODICE “E”	9(3) V9(4)  
        //        04 FILLER	A DISPOSIZIONE	X(50) 
        //02 FILLER  PIC X(15000).  RIDOTTO DI 8 PERCHE' SULLA GARC VENGONO AGGIUNTI 8 BYTE
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// T-GP4DAA1	CATEGORIA FASCICOLO	9(3)
        /// <summary>
        [HisFieldInfoMapping(0, 3, CobolType = CobolType.Unsigned)]
        public short T_GP4DAA1 { get; set; }

        /// <summary>
        /// T-GP4DAA2-1	SEDE FITTIZIA FASCICOLO (“9990”)	9(4)
        /// <summary>
        [HisFieldInfoMapping(1, 4, CobolType = CobolType.Unsigned)]
        public short T_GP4DAA2_1 { get; set; }

        /// <summary>
        /// T-GP4DAA2-2	PROGRESSIVO FASCICOLO	9(8)
        /// <summary>
        [HisFieldInfoMapping(2, 8, CobolType = CobolType.Unsigned)]
        public int T_GP4DAA2_2 { get; set; }

        /// <summary>
        /// T-GP4DB00	TABELLA CON 25 RIPETIZIONI
        /// <summary>
        [HisComplexAreaInfoMapping(3, ListCount = 25)]
        public List<T_GP4DB00> LISTT_GP4DB00 { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class T_GP4DB00
        {
            #region Properties

            #region Tracciato COBOL
            //02 T-GP4DB00	TABELLA CON 25 RIPETIZIONI	
            //    03 T-GP4KA01	CATEGORIA PENSIONE LIQUIDATA	X(3)	
            //    03 T-GP4KA02	SEDE PENSIONE LIQUIDATA	X(2)	
            //    03 T-GP4KA03	ZONA PENSIONE LIQUIDATA	X(2)
            //    03 T-GP4KA04	CERTIFICATO PENSIONE LIQUIDATA	X(8)	
            //    03 T-GP4DB09	CODICE FISCALE	X(16)                                                       
            //    03 T-GP4DB13 CSOG PIC 9(9).
            //    03 T-GP4DB14	DATA DI MATRIMONIO	9(8)
            //    03 T-GP4DB15	CODICE NUCLEO	X(2)            
            //    03 FILLER	A DISPOSIZIONE 	X(50) 
            //    03 T-GP4DC00	TABELLA CON 20 RIPETIZIONI 	
            //        04 T-GP4DC01	PERCENTUALE SPETTANTE 	9(3) V9(4)	
            //        04 T-GP4DC02	DECORRENZA PERIODO (AAAA/MM)	9(6)
            //        04 T-GP4DC03	CESSAZIONE PERIODO (AAAA/MM)	9(6)
            //        04 T-GP4DC04	CODICE FAMILIARE	X(2)
            //        04 T-GP4DC05	COEFFICIENTE RIDUZIONE	9(3) V9(4)
            //        04 T-GP4DC07	PERCENTUALE GIUDICE CODICE “E”	9(3) V9(4)  
            //        04 FILLER	A DISPOSIZIONE	X(50)
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// T-GP4KA01	CATEGORIA PENSIONE LIQUIDATA	X(3)
            /// <summary>
            [HisFieldInfoMapping(0, 3)]
            public string T_GP4KA01 { get; set; }

            /// <summary>
            /// T-GP4KA02	SEDE PENSIONE LIQUIDATA	X(2)
            /// <summary>
            [HisFieldInfoMapping(1, 2)]
            public string T_GP4KA02 { get; set; }

            /// <summary>
            /// TGP4KA03	ZONA PENSIONE LIQUIDATA	X(2)
            /// <summary>
            [HisFieldInfoMapping(2, 2)]
            public string T_GP4KA03 { get; set; }

            /// <summary>
            /// T-GP4KA04	CERTIFICATO PENSIONE LIQUIDATA	X(8)
            /// <summary>
            [HisFieldInfoMapping(3, 8)]
            public string T_GP4KA04 { get; set; }

            /// <summary>
            /// T-GP4DB09	CODICE FISCALE	X(16)
            /// <summary>
            [HisFieldInfoMapping(4, 16)]
            public string T_GP4DB09 { get; set; }

            /// <summary>
            /// T-GP4DB13	CSOG	9(9)
            /// <summary>
            [HisFieldInfoMapping(5, 9, CobolType = CobolType.Unsigned)]
            public int T_GP4DB13 { get; set; }

            /// <summary>
            /// T-GP4DB14	DATA DI MATRIMONIO	9(8)
            /// <summary>
            [HisFieldInfoMapping(6, 8, CobolType = CobolType.Unsigned)]
            public int T_GP4DB14 { get; set; }

            /// <summary>
            /// T-GP4DB15	CODICE NUCLEO	X(2) 
            /// <summary>
            [HisFieldInfoMapping(7, 2)]
            public string T_GP4DB15 { get; set; }

            /// <summary>
            /// FILLER	A DISPOSIZIONE 	X(50)
            /// <summary>
            [HisFieldInfoMapping(8, 50)]
            public string FILLER { get; set; }

            /// <summary>
            /// T-GP4DC00	TABELLA CON 20 RIPETIZIONI
            /// <summary>
            [HisComplexAreaInfoMapping(9, ListCount = 20)]
            public List<T_GP4DC00> LISTT_GP4DC00 { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }

        public class T_GP4DC00
        {
            #region Properties

            #region Tracciato COBOL
            //    03 T-GP4DC00	TABELLA CON 20 RIPETIZIONI 	
            //        04 T-GP4DC01	PERCENTUALE SPETTANTE 	9(3) V9(4)	
            //        04 T-GP4DC02	DECORRENZA PERIODO (AAAA/MM)	9(6)
            //        04 T-GP4DC03	CESSAZIONE PERIODO (AAAA/MM)	9(6)
            //        04 T-GP4DC04	CODICE FAMILIARE	X(2)
            //        04 T-GP4DC05	COEFFICIENTE RIDUZIONE	9(3) V9(4)
            //        04 T-GP4DC07	PERCENTUALE GIUDICE CODICE “E”	9(3) V9(4)  
            //        04 FILLER	A DISPOSIZIONE	X(50)
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// T-GP4DC01	PERCENTUALE SPETTANTE 	9(3) V9(4) 
            /// </summary>
            [HisFieldInfoMapping(0, 7, Scale = 4, CobolType = CobolType.Unsigned)]
            public decimal T_GP4DC01 { get; set; }

            /// <summary>
            /// T-GP4DC02	DECORRENZA PERIODO (AAAA/MM)	9(6)
            /// <summary>
            [HisFieldInfoMapping(1, 6)]
            public int T_GP4DC02 { get; set; }

            /// <summary>
            /// T-GP4DC03	CESSAZIONE PERIODO (AAAA/MM)	9(6)
            /// <summary>
            [HisFieldInfoMapping(2, 6)]
            public int T_GP4DC03 { get; set; }

            /// <summary>
            /// T-GP4DC04	CODICE FAMILIARE	X(2)
            /// <summary>
            [HisFieldInfoMapping(3, 2)]
            public string T_GP4DC04 { get; set; }

            /// <summary>
            /// T-GP4DC05	COEFFICIENTE RIDUZIONE	9(3) V9(4)
            /// </summary>
            [HisFieldInfoMapping(4, 7, Scale = 4, CobolType = CobolType.Unsigned)]
            public decimal T_GP4DC05 { get; set; }

            /// <summary>
            /// T-GP4DC07	PERCENTUALE GIUDICE CODICE “E”	9(3) V9(4)  
            /// </summary>
            [HisFieldInfoMapping(5, 7, Scale = 4, CobolType = CobolType.Unsigned)]
            public decimal T_GP4DC07 { get; set; }

            /// <summary>
            /// FILLER	A DISPOSIZIONE 	X(50)
            /// <summary>
            [HisFieldInfoMapping(6, 50)]
            public string FILLER { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class
        #endregion Properties
    }
}
