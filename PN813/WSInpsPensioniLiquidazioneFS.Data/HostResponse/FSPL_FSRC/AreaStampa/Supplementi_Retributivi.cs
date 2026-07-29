using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostResponse.AreaStampa
{
    public class Supplementi_Retributivi
    {
        #region Constructor
        internal Supplementi_Retributivi()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 SUPPL-RETR       PIC 9(01).
        //*                          FLAG = 1  TABELLA VALORIZZATA    2750
        //     02 GEST-SUPPL-R     PIC X(05)              OCCURS 15 TIMES.
        //*                          OBG-A CDM-A ART-A COM-A (ANTE93) 2751
        //*                          OBG-P CDM-P ART-P COM-P (POST92)
        //*                          CMB   FIT   MAR-A MAR-P
        //*                          (DATI NON POSIZIONALI)
        //     02 SETT-SUPPL-R     PIC 9(04)              OCCURS 15 TIMES.
        //*                          NUMERO SETTIMANE SUPPL.          2826
        //     02 SETT-ESCL-R      PIC 9(04)              OCCURS 15 TIMES.
        //*                          NUMERO SETTIMANE ESCLUSIVE       2886
        //     02 RMS-SUPPL-R      PIC 9(07)V9(04) COMP-3 OCCURS 15 TIMES.
        //*                          RETRIBUZ.MEDIA SETT.             2946
        //     02 PENS-SUPPL-R     PIC 9(07)V9(04) COMP-3 OCCURS 15 TIMES.
        //*                          IMP.MENSILE SUPPLEMENTO          3036
        //     02 FILLER           PIC X(24).
        //*                          LIBERI                           3126
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// SUPPL_RETR 9(01)  
        /// </summary>
        [HisFieldInfoMapping(0, 1, CobolType = CobolType.Unsigned)]
        public short SUPPL_RETR { get; set; }

        // *                          FLAG = 1  TABELLA VALORIZZATA    2750

        [HisComplexAreaInfoMapping(1, ListCount = 15)]
        public List<Gestione> LISTGestione { get; set; }

        [HisComplexAreaInfoMapping(2, ListCount = 15)]
        public List<Settimane> LISTSettimane { get; set; }

        [HisComplexAreaInfoMapping(3, ListCount = 15)]
        public List<SettimaneEsclusive> LISTSettimaneEsclusive { get; set; }

        [HisComplexAreaInfoMapping(4, ListCount = 15)]
        public List<RMS> LISTRMS { get; set; }

        [HisComplexAreaInfoMapping(5, ListCount = 15)]
        public List<Imponibile> LISTImponibile { get; set; }

        /// <summary>
        /// FILLER X(24)  
        /// </summary>
        [HisFieldInfoMapping(6, 24)]
        public string FILLER { get; set; }

        // *                          LIBERI                           3126
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
            //     02 GEST-SUPPL-R     PIC X(05)              OCCURS 15 TIMES.
            //*                          OBG-A CDM-A ART-A COM-A (ANTE93) 2751
            //*                          OBG-P CDM-P ART-P COM-P (POST92)
            //*                          CMB   FIT   MAR-A MAR-P
            //*                          (DATI NON POSIZIONALI)
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GEST_SUPPL_R X(05)  
            /// </summary>
            [HisFieldInfoMapping(1, 5)]
            public string GEST_SUPPL_R { get; set; }

            // *                          OBG-A CDM-A ART-A COM-A (ANTE93) 2751
            // *                          OBG-P CDM-P ART-P COM-P (POST92)
            // *                          CMB   FIT   MAR-A MAR-P
            // *                          (DATI NON POSIZIONALI)
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
            //     02 SETT-SUPPL-R     PIC 9(04)              OCCURS 15 TIMES.
            //*                          NUMERO SETTIMANE SUPPL.          2826
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// SETT_SUPPL_R 9(04)  
            /// </summary>
            [HisFieldInfoMapping(2, 4, CobolType = CobolType.Unsigned)]
            public short SETT_SUPPL_R { get; set; }

            // *                          NUMERO SETTIMANE SUPPL.          2826
            #endregion Tracciato Host

            #endregion Properties
        }

        public class SettimaneEsclusive
        {
            #region Constructor
            internal SettimaneEsclusive()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //    02 SETT-ESCL-R      PIC 9(04)              OCCURS 15 TIMES.
            //*                          NUMERO SETTIMANE ESCLUSIVE       2886
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// SETT_ESCL_R 9(04)  
            /// </summary>
            [HisFieldInfoMapping(3, 4, CobolType = CobolType.Unsigned)]
            public short SETT_ESCL_R { get; set; }

            // *                          NUMERO SETTIMANE ESCLUSIVE       2886
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
            //      02 RMS-SUPPL-R      PIC 9(07)V9(04) COMP-3 OCCURS 15 TIMES.
            //*                          RETRIBUZ.MEDIA SETT.             2946
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// RMS_SUPPL_R 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(4, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal RMS_SUPPL_R { get; set; }

            // *                          RETRIBUZ.MEDIA SETT.             2946
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Imponibile
        {
            #region Constructor
            internal Imponibile()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //      02 PENS-SUPPL-R     PIC 9(07)V9(04) COMP-3 OCCURS 15 TIMES.
            //*                          IMP.MENSILE SUPPLEMENTO          3036
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// PENS_SUPPL_R 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(5, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal PENS_SUPPL_R { get; set; }

            // *                          IMP.MENSILE SUPPLEMENTO          3036
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}

