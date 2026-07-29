using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostResponse.AreaStampa
{
    public class INPDAI
    {
        #region Constructor
        internal INPDAI()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 FLAG-INPDAI      PIC 9.
        //*                          FLAG = 1 (TABELLA VALORIZZATA)  12324
        //     02 POST-2003        PIC 9.
        //*                          FLAG = 1 (ANTE 02/2003)         12325
        //*                          FLAG = 2 (POST 01/2003)
        //     02 AAAA-INPDAI      PIC 9(4).
        //*                          ANNO                            12326
        //     02 ANZ-INPDAI       PIC 9(5) COMP-3 OCCURS 20 TIMES.
        //*                          GIORNI/SETTIMANE                12330
        //     02 RETR-INPDAI      PIC 9(7)V9(4) COMP-3 OCCURS 20 TIMES.
        //*                          RETR. GIORNALIERA/SETTIMANALE   12390
        //     02 IMP-INPDAI       PIC 9(7)V9(4) COMP-3 OCCURS 20 TIMES.
        //*                          IMPORTO PENSIONE                12510
        //     02 FILLER           PIC X(10).
        //*                                                          12630
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// FLAG_INPDAI 9  
        /// </summary>
        [HisFieldInfoMapping(0, 1, CobolType = CobolType.Unsigned)]
        public short FLAG_INPDAI { get; set; }

        // *                          FLAG = 1 (TABELLA VALORIZZATA)  12324
        /// <summary>
        /// POST_2003 9  
        /// </summary>
        [HisFieldInfoMapping(1, 1, CobolType = CobolType.Unsigned)]
        public short POST_2003 { get; set; }

        // *                          FLAG = 1 (ANTE 02/2003)         12325
        // *                          FLAG = 2 (POST 01/2003)
        /// <summary>
        /// AAAA_INPDAI 9(4)  
        /// </summary>
        [HisFieldInfoMapping(2, 4, CobolType = CobolType.Unsigned)]
        public short AAAA_INPDAI { get; set; }

        // *                          ANNO                            12326

        [HisComplexAreaInfoMapping(3, ListCount = 20)]
        public List<Giorni_Settimane> LISTGiorni_Settimane { get; set; }

        [HisComplexAreaInfoMapping(4, ListCount = 20)]
        public List<Retribuzione> LISTRetribuzione { get; set; }

        [HisComplexAreaInfoMapping(5, ListCount = 20)]
        public List<Importo> LISTImporto { get; set; }

        /// <summary>
        /// FILLER X(10)  
        /// </summary>
        [HisFieldInfoMapping(6, 10)]
        public string FILLER { get; set; }

        // *                                                          12630
        #endregion Tracciato Host

        #region nested class
        public class Giorni_Settimane
        {
            #region Constructor
            internal Giorni_Settimane()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 ANZ-INPDAI       PIC 9(5) COMP-3 OCCURS 20 TIMES.
            //*                          GIORNI/SETTIMANE                12330
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// ANZ_INPDAI 9(5)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 3, CobolType = CobolType.Comp3Unsigned)]
            public int ANZ_INPDAI { get; set; }

            // *                          GIORNI/SETTIMANE                12330
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Retribuzione
        {
            #region Constructor
            internal Retribuzione()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 RETR-INPDAI      PIC 9(7)V9(4) COMP-3 OCCURS 20 TIMES.
            //*                          RETR. GIORNALIERA/SETTIMANALE   12390
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// RETR_INPDAI 9(7)V9(4)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal RETR_INPDAI { get; set; }

            // *                          RETR. GIORNALIERA/SETTIMANALE   12390
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Importo
        {
            #region Constructor
            internal Importo()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 IMP-INPDAI       PIC 9(7)V9(4) COMP-3 OCCURS 20 TIMES.
            //*                          IMPORTO PENSIONE                12510
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// IMP_INPDAI 9(7)V9(4)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal IMP_INPDAI { get; set; }

            // *                          IMPORTO PENSIONE                12510
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}

