using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostResponse.AreaStampa
{
    public class Dati_Contributivi_Old
    {
        #region Constructor
        internal Dati_Contributivi_Old()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 FLAG-OLD-CONTR   PIC 9.
        //*                          FLAG = 1 (TABELLA VALORIZZATA)   3150
        //     02 GEST-OLDCONTR    PIC X(03)             OCCURS 4 TIMES.
        //*                          OBG-CDM-ART-COM (POSIZIONALI)    3151
        //     02 ADEG-OLDCONTR    PIC 9(07)V9(04) COMP-3  OCCURS 4 TIMES.
        //*                          ADEGUATA                         3163
        //     02 IVS-OLDCONTR     PIC 9(07)V9(04) COMP-3  OCCURS 4 TIMES.
        //*                          IVS                              3187
        //     02 BASE-OLDCONTR    PIC 9(07)V9(04) COMP-3  OCCURS 4 TIMES.
        //*                          BASE                             3211
        //     02 FILLER           PIC X(35).
        //*                          LIBERI                           3235
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// 02 FLAG-OLD-CONTR   PIC 9.  
        /// </summary>
        [HisFieldInfoMapping(0, 1, CobolType = CobolType.Unsigned)]
        public short FLAG_OLD_CONTR { get; set; }

        // *                          FLAG = 1 (TABELLA VALORIZZATA)   3150

        [HisComplexAreaInfoMapping(1, ListCount = 4)]
        public List<Gestione> LISTGestione { get; internal set; }

        [HisComplexAreaInfoMapping(2, ListCount = 4)]
        public List<Adeguata> LISTAdeguata { get; internal set; }

        [HisComplexAreaInfoMapping(3, ListCount = 4)]
        public List<IVS> LISTIVS { get; internal set; }

        [HisComplexAreaInfoMapping(4, ListCount = 4)]
        public List<Base> LISTBase { get; internal set; }

        /// <summary>
        /// FILLER X(35)  
        /// </summary>
        [HisFieldInfoMapping(5, 35)]
        public string FILLER { get; set; }

        // *                          LIBERI                           3235
        #endregion Tracciato Host

        #region nested class
        public class Gestione
        {
            #region Constructor
            internal Gestione()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //    02 GEST-OLDCONTR    PIC X(03)             OCCURS 4 TIMES.
            //*                          OBG-CDM-ART-COM (POSIZIONALI)    3151
            #endregion Tracciato COBOL

            #region Tracciato Host
            // <summary>
            /// GEST_OLDCONTR X(03)  
            /// </summary>
            [HisFieldInfoMapping(0, 3)]
            public string GEST_OLDCONTR { get; set; }

            // *                          OBG-CDM-ART-COM (POSIZIONALI)    3151
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Adeguata
        {
            #region Constructor
            internal Adeguata()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //    02 ADEG-OLDCONTR    PIC 9(07)V9(04) COMP-3  OCCURS 4 TIMES.
            //*                          ADEGUATA                         3163
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// ADEG_OLDCONTR 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal ADEG_OLDCONTR { get; set; }

            // *                          ADEGUATA                         3163
            #endregion Tracciato Host

            #endregion Properties
        }

        public class IVS
        {
            #region Constructor
            internal IVS()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //   02 IVS-OLDCONTR     PIC 9(07)V9(04) COMP-3  OCCURS 4 TIMES.
            //*                          IVS                              3187
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// IVS_OLDCONTR 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal IVS_OLDCONTR { get; set; }

            // *                          IVS                              3187
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Base
        {
            #region Constructor
            internal Base()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //    02 BASE-OLDCONTR    PIC 9(07)V9(04) COMP-3  OCCURS 4 TIMES.
            //*                          BASE                             3211
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// BASE_OLDCONTR 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal BASE_OLDCONTR { get; set; }

            // *                          BASE                             3211
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}

