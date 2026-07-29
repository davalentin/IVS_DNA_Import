using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostResponse.AreaStampa
{
    public class Quote
    {
        #region Constructor
        internal Quote()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 TIPO-QUOTA       PIC X(02) OCCURS 8 TIMES.
        //*                                                          17200
        //     02 ANNI-QUOTA       PIC 9(02) OCCURS 8 TIMES.
        //*                                                          17216
        //     02 MESI-QUOTA       PIC 9(02) OCCURS 8 TIMES.
        //*                                                          17232
        //     02 GIOR-QUOTA       PIC 9(02) OCCURS 8 TIMES.
        //*                                                          17248
        //     02 SETT-QUOTA       PIC 9(04) OCCURS 8 TIMES.
        //*                                                          17264
        //     02 SETT-ESCL-Q      PIC 9(04) OCCURS 8 TIMES.
        //*                                                          17296
        //     02 ESCL-ART24       PIC 9(04) OCCURS 8 TIMES.
        //*                                                          17328
        //     02 ESCL-ART57       PIC 9(04) OCCURS 8 TIMES.
        //*                                                          17360
        //     02 RETR-ANNUA-P     PIC 9(07)V9(04) COMP-3 OCCURS 8 TIMES.
        //*                                                          17392
        //     02 RETR-MEDIA-S     PIC 9(07)V9(04) COMP-3 OCCURS 8 TIMES.
        //*                                                          17440
        //     02 FILLER           PIC X(112).
        //*                                                          17488
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 8)]
        public List<TipoQuota> LISTTipoQuota { get; set; }

        [HisComplexAreaInfoMapping(1, ListCount = 8)]
        public List<AnniQuota> LISTAnniQuota { get; set; }

        [HisComplexAreaInfoMapping(2, ListCount = 8)]
        public List<MesiQuota> LISTMesiQuota { get; set; }

        [HisComplexAreaInfoMapping(3, ListCount = 8)]
        public List<GiorniQuota> LISTGiorniQuota { get; set; }

        [HisComplexAreaInfoMapping(4, ListCount = 8)]
        public List<SettimaneQuota> LISTSettimaneQuota { get; set; }

        [HisComplexAreaInfoMapping(5, ListCount = 8)]
        public List<SettimaneEsclusiveQuota> LISTSettimaneEsclusiveQuota { get; set; }

        [HisComplexAreaInfoMapping(6, ListCount = 8)]
        public List<Articolo24> LISTArticolo24 { get; set; }

        [HisComplexAreaInfoMapping(7, ListCount = 8)]
        public List<Articolo57> LISTArticolo57 { get; set; }

        [HisComplexAreaInfoMapping(8, ListCount = 8)]
        public List<RetribuzioneAnnua> LISTRetribuzioneAnnua { get; set; }

        [HisComplexAreaInfoMapping(9, ListCount = 8)]
        public List<RetribuzioneMedia> LISTRetribuzioneMedia { get; set; }

        /// <summary>
        /// FILLER X(112)  
        /// </summary>
        [HisFieldInfoMapping(10, 112)]
        public string FILLER { get; set; }

        // *                                                          17488
        #endregion Tracciato Host

        #region nested class
        public class TipoQuota
        {
            #region Constructor
            internal TipoQuota()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 TIPO-QUOTA       PIC X(02) OCCURS 8 TIMES.
            //*                                                          17200
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// TIPO_QUOTA X(02)  
            /// </summary>
            [HisFieldInfoMapping(0, 2)]
            public string TIPO_QUOTA { get; set; }

            // *                                                          17200
            #endregion Tracciato Host

            #endregion Properties
        }

        public class AnniQuota
        {
            #region Constructor
            internal AnniQuota()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 ANNI-QUOTA       PIC 9(02) OCCURS 8 TIMES.
            //*                                                          17216
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// ANNI_QUOTA 9(02)  
            /// </summary>
            [HisFieldInfoMapping(0, 2, CobolType = CobolType.Unsigned)]
            public short ANNI_QUOTA { get; set; }

            // *                                                          17216
            #endregion Tracciato Host

            #endregion Properties
        }

        public class MesiQuota
        {
            #region Constructor
            internal MesiQuota()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 MESI-QUOTA       PIC 9(02) OCCURS 8 TIMES.
            //*                                                          17232
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// MESI_QUOTA 9(02)  
            /// </summary>
            [HisFieldInfoMapping(0, 2, CobolType = CobolType.Unsigned)]
            public short MESI_QUOTA { get; set; }

            // *                                                          17232
            #endregion Tracciato Host

            #endregion Properties
        }

        public class GiorniQuota
        {
            #region Constructor
            internal GiorniQuota()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 GIOR-QUOTA       PIC 9(02) OCCURS 8 TIMES.
            //*                                                          17248
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GIOR_QUOTA 9(02)  
            /// </summary>
            [HisFieldInfoMapping(0, 2, CobolType = CobolType.Unsigned)]
            public short GIOR_QUOTA { get; set; }

            // *                                                          17248
            #endregion Tracciato Host

            #endregion Properties
        }

        public class SettimaneQuota
        {
            #region Constructor
            internal SettimaneQuota()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 SETT-QUOTA       PIC 9(04) OCCURS 8 TIMES.
            //*                                                          17264
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// SETT_QUOTA 9(04)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short SETT_QUOTA { get; set; }

            // *                                                          17264
            #endregion Tracciato Host

            #endregion Properties
        }

        public class SettimaneEsclusiveQuota
        {
            #region Constructor
            internal SettimaneEsclusiveQuota()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 SETT-ESCL-Q      PIC 9(04) OCCURS 8 TIMES.
            //*                                                          17296
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// SETT_ESCL_Q 9(04)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short SETT_ESCL_Q { get; set; }

            // *                                                          17296
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Articolo24
        {
            #region Constructor
            internal Articolo24()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 ESCL-ART24       PIC 9(04) OCCURS 8 TIMES.
            //*                                                          17328
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// ESCL_ART24 9(04)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short ESCL_ART24 { get; set; }

            // *                                                          17328
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Articolo57
        {
            #region Constructor
            internal Articolo57()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 ESCL-ART57       PIC 9(04) OCCURS 8 TIMES.
            //*                                                          17360
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// ESCL_ART57 9(04)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short ESCL_ART57 { get; set; }

            // *                                                          17360
            #endregion Tracciato Host

            #endregion Properties
        }

        public class RetribuzioneAnnua
        {
            #region Constructor
            internal RetribuzioneAnnua()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 RETR-ANNUA-P     PIC 9(07)V9(04) COMP-3 OCCURS 8 TIMES.
            //*                                                          17392
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// RETR_ANNUA_P 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal RETR_ANNUA_P { get; set; }

            // *                                                          17392
            #endregion Tracciato Host

            #endregion Properties
        }

        public class RetribuzioneMedia
        {
            #region Constructor
            internal RetribuzioneMedia()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 RETR-MEDIA-S     PIC 9(07)V9(04) COMP-3 OCCURS 8 TIMES.
            //*                                                          17440
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// RETR_MEDIA_S 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal RETR_MEDIA_S { get; set; }

            // *                                                          17440
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
