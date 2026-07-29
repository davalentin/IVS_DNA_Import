using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostResponse.AreaStampa
{
    public class Dati_Retributivi
    {
        #region Constructor
        internal Dati_Retributivi()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 FLAG-RETR        PIC 9(01).
        //*                          FLAG = 1  TABELLA VALORIZZATA    2450
        //     02 GEST-RETR        PIC X(03)              OCCURS 12 TIMES.
        //*                          OBG-CDM-ART-COM (ANTE93,POST92)  2451
        //*                          CMB-FIT-MAR(ANTE93,POST92)
        //*                          (DATI POSIZIONALI NELL'ORDINE)
        //     02 SETT-RETR        PIC 9(04)              OCCURS 12 TIMES.
        //*                          NUMERO SETTIMANE                 2487
        //     02 SETT-ESCL        PIC 9(04)              OCCURS 12 TIMES.
        //*                          NUMERO SETTIMANE ESCLUSIVE F.S.  2535
        //     02 RMS-RETR         PIC 9(07)V9(04) COMP-3 OCCURS 12 TIMES.
        //*                          RETRIBUZ.MEDIA SETT.             2583
        //     02 PENS-RETR        PIC 9(07)V9(04) COMP-3 OCCURS 12 TIMES.
        //*                          IMP.MENSILE PENSIONE             2655
        //     02 FILLER           PIC X(23).
        //*                          LIBERI                           2727
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// FLAG_RETR 9(01)  
        /// </summary>
        [HisFieldInfoMapping(0, 1, CobolType = CobolType.Unsigned)]
        public short FLAG_RETR { get; set; }

        [HisComplexAreaInfoMapping(1, ListCount = 12)]
        public List<Gestione> LISTGestione { get; internal set; }

        [HisComplexAreaInfoMapping(2, ListCount = 12)]
        public List<Settimane> LISTSettimane { get; internal set; }

        [HisComplexAreaInfoMapping(3, ListCount = 12)]
        public List<SettimaneEsclusive> LISTSettimaneEsclusive { get; internal set; }

        [HisComplexAreaInfoMapping(4, ListCount = 12)]
        public List<RMS> LISTRMS { get; internal set; }

        [HisComplexAreaInfoMapping(5, ListCount = 12)]
        public List<Imponibile> LISTImponibile { get; internal set; }

        /// <summary>
        /// FILLER X(23)  
        /// </summary>
        [HisFieldInfoMapping(6, 23)]
        public string FILLER { get; set; }
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
            //  02 GEST-RETR        PIC X(03)              OCCURS 12 TIMES.
            //*                          OBG-CDM-ART-COM (ANTE93,POST92)  2451
            //*                          CMB-FIT-MAR(ANTE93,POST92)
            //*                          (DATI POSIZIONALI NELL'ORDINE)
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GEST_RETR X(03)  
            /// </summary>
            [HisFieldInfoMapping(0, 3)]
            public string GEST_RETR { get; set; }

            // *                          OBG-CDM-ART-COM (ANTE93,POST92)  2451
            // *                          CMB-FIT-MAR(ANTE93,POST92)
            // *                          (DATI POSIZIONALI NELL'ORDINE)
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
            //     02 SETT-RETR        PIC 9(04)              OCCURS 12 TIMES.
            //*                          NUMERO SETTIMANE                 2487
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// SETT_RETR 9(04)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short SETT_RETR { get; set; }

            // *                          NUMERO SETTIMANE                 2487
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
            //   02 SETT-ESCL        PIC 9(04)              OCCURS 12 TIMES.
            //*                          NUMERO SETTIMANE ESCLUSIVE F.S.  2535
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// SETT_ESCL 9(04)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short SETT_ESCL { get; set; }

            // *                          NUMERO SETTIMANE ESCLUSIVE F.S.  2535
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
            //  02 RMS-RETR         PIC 9(07)V9(04) COMP-3 OCCURS 12 TIMES.
            //*                          RETRIBUZ.MEDIA SETT.             2583
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// RMS_RETR 9(07)V9(04) COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal RMS_RETR { get; set; }

            // *                          RETRIBUZ.MEDIA SETT.             2583
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
            //  02 PENS-RETR        PIC 9(07)V9(04) COMP-3 OCCURS 12 TIMES.
            //*                          IMP.MENSILE PENSIONE             2655
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// PENS_RETR 9(07)V9(04) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal PENS_RETR { get; set; }

            // *                          IMP.MENSILE PENSIONE             2655
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}

