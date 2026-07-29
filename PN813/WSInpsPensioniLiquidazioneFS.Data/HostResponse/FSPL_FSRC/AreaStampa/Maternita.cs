using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostResponse.AreaStampa
{
    public class Maternita
    {
        #region Constructor
        internal Maternita()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 FLAG-MATER       PIC 9(01).
        //*                          FLAG = 1 TABELLA VALORIZZATA     5776
        //     02 ANNO-MATER       PIC 9(04)               OCCURS 2 TIMES.
        //*                          ANNO MATERNITA'                  5777
        //     02 SETT-MATER       PIC 9(04)               OCCURS 2 TIMES.
        //*                          NUMERO SETTIMANE                 5785
        //     02 RMS-MATER        PIC S9(07)V9(04) COMP-3 OCCURS 2 TIMES.
        //*                          RETRIBUZ.MEDIA SETT.             5793
        //     02 PENS-MATER       PIC S9(07)V9(04) COMP-3 OCCURS 2 TIMES.
        //*                          IMP.MENSILE PENSIONE             5805
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// FLAG_MATER 9(01)  
        /// </summary>
        [HisFieldInfoMapping(0, 1, CobolType = CobolType.Unsigned)]
        public short FLAG_MATER { get; set; }

        // *                          FLAG = 1 TABELLA VALORIZZATA     5776

        [HisComplexAreaInfoMapping(1, ListCount = 2)]
        public List<AnnoMaternita> LISTAnnoMaternita { get; set; }

        [HisComplexAreaInfoMapping(2, ListCount = 2)]
        public List<Settimane> LISTSettimane { get; set; }

        [HisComplexAreaInfoMapping(3, ListCount = 2)]
        public List<RMS> LISTRMS { get; set; }

        [HisComplexAreaInfoMapping(4, ListCount = 2)]
        public List<ImportoMensile> LISTImportoMensile { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class AnnoMaternita
        {
            #region Constructor
            internal AnnoMaternita()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 ANNO-MATER       PIC 9(04)               OCCURS 2 TIMES.
            //*                          ANNO MATERNITA'                  5777
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// ANNO_MATER 9(04)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short ANNO_MATER { get; set; }

            // *                          ANNO MATERNITA'                  5777
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Settimane
        {
            #region Constructor
            internal Settimane()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 SETT-MATER       PIC 9(04)               OCCURS 2 TIMES.
            //*                          NUMERO SETTIMANE                 5785
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// SETT_MATER 9(04)  
            /// </summary>
            [HisFieldInfoMapping(2, 4, CobolType = CobolType.Unsigned)]
            public short SETT_MATER { get; set; }

            // *                          NUMERO SETTIMANE                 5785
            #endregion Tracciato Host

            #endregion Properties
        }

        public class RMS
        {
            #region Constructor
            internal RMS()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 RMS-MATER        PIC S9(07)V9(04) COMP-3 OCCURS 2 TIMES.
            //*                          RETRIBUZ.MEDIA SETT.             5793
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// RMS_MATER S9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(3, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal RMS_MATER { get; set; }

            // *                          RETRIBUZ.MEDIA SETT.             5793
            #endregion Tracciato Host

            #endregion Properties
        }

        public class ImportoMensile
        {
            #region Constructor
            internal ImportoMensile()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 PENS-MATER       PIC S9(07)V9(04) COMP-3 OCCURS 2 TIMES.
            //*                          IMP.MENSILE PENSIONE             5805
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// PENS_MATER S9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(4, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal PENS_MATER { get; set; }

            // *                          IMP.MENSILE PENSIONE             5805
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}

