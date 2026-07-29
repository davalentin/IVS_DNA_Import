using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostResponse.AreaStampa
{
    public class Decorrenze_FS
    {
        #region Constructor
        internal Decorrenze_FS()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 GP5HG02-23       PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          PENSIONE AGO                    13568
        //     02 GP5HG02-23OLD    PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          PENSIONE AGO OLD                13760
        //     02 GP5HG02-24       PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          PENSIONE FONDO                  13952
        //     02 GP5HG02-24OLD    PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          PENSIONE FONDO OLD              14144
        //     02 GP5HG02-25       PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          QUOTA ART.24 2°COMMA L.337      14336
        //     02 GP5HG02-25OLD    PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          QUOTA ART.24 2°COMMA L.337 OLD  14496
        //     02 GP5HG02-26       PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          DIFF. ART.16 1°COMMA L.903      14656
        //     02 GP5HG02-26OLD    PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          DIFF. ART.16 1°COMMA L.903 OLD  14816
        //     02 GP5HG02-27       PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          SCALA MOBILE ETC.               14976
        //     02 GP5HG02-27OLD    PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          SCALA MOBILE OLD ETC.           15136
        //     02 GP5HG02-28       PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          ECCEDENZA AGO SU FONDO          15296
        //     02 GP5HG02-28OLD    PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          ECCEDENZA AGO SU FONDO OLD      15456
        //     02 GP5HG02-29       PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          SUPPL.SERVIZIO MILITARE         15616
        //     02 GP5HG02-29OLD    PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          SUPPL.SERVIZIO MILITARE OLD     15776
        //     02 GP5HG02-31       PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          SUPPL.ART.57 L.377              15936
        //     02 GP5HG02-31OLD    PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          SUPPL.ART.57 L.337 OLD          16096
        //     02 GP5HG02-32       PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          TRATTENUTA INAIL                16256
        //     02 GP5HG02-32OLD    PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          TRATTENUTA INAIL OLD            16416
        //     02 GP5HG02-33       PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          TRATTENUTA INAIL                16576
        //     02 GP5HG02-33OLD    PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          TRATTENUTA INAIL OLD            16768
        //     02 INI-VERS         PIC 9(08).
        //*                          DA AAAAMMGG (INIZIO VERSAMENTI) 16960
        //     02 FINE-VERS        PIC 9(08).
        //*                          A AAAAMMGG (FINE VERSAMENTI)    16968
        //     02 ANNI-DIFF        PIC 9(02).
        //*                          ANNI DIFFERIMENTO               16976
        //     02 ANNI-UTILI       PIC 9(02).
        //*                          ANNI UTILI AL DIRITTO           16978
        //     02 ETA-REQUIS       PIC 9(02).
        //*                          ETA' PERFEZIONAMENTO REQUISITI  16980
        //     02 MONT-CONTR-FS    PIC 9(07)V9(04) COMP-3.
        //*                          MONTANTE CONTRIB.  (C4)         16982
        //*                          GP2BB06
        //     02 IMP-CONTR-TOT    PIC 9(07)V9(04) COMP-3.
        //*                          IMPORTO CONTRIBUTIVO TOTALE     16988
        //*                          GP2BB06
        //     02 IMP-PENS-LORDO   PIC 9(07)V9(04) COMP-3.
        //*                          IMP. PENSIONE ANNUO LORDO       16994
        //*                          FONDO FS
        //     02 CAP-GP2NO29E     PIC 9(05)V9(04) COMP-3.
        //*                                                          17000
        //     02 FILLER           PIC X(195).
        //*                                                          17005
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 32)]
        public List<Ago> LISTAgo { get; internal set; }

        [HisComplexAreaInfoMapping(1, ListCount = 32)]
        public List<AgoOld> LISTAgoOld { get; internal set; }

        [HisComplexAreaInfoMapping(2, ListCount = 32)]
        public List<Fondo> LISTFondo { get; internal set; }

        [HisComplexAreaInfoMapping(3, ListCount = 32)]
        public List<FondoOld> LISTFondoOld { get; internal set; }

        [HisComplexAreaInfoMapping(4, ListCount = 32)]
        public List<Quota> LISTQuota { get; internal set; }

        [HisComplexAreaInfoMapping(5, ListCount = 32)]
        public List<QuotaOld> LISTQuotaOld { get; internal set; }

        [HisComplexAreaInfoMapping(6, ListCount = 32)]
        public List<Diff> LISTDiff { get; internal set; }

        [HisComplexAreaInfoMapping(7, ListCount = 32)]
        public List<DiffOld> LISTDiffOld { get; internal set; }

        [HisComplexAreaInfoMapping(8, ListCount = 32)]
        public List<ScalaMobile> LISTScalaMobile { get; internal set; }

        [HisComplexAreaInfoMapping(9, ListCount = 32)]
        public List<ScalaMobileOld> LISTScalaMobileOld { get; internal set; }

        [HisComplexAreaInfoMapping(10, ListCount = 32)]
        public List<Eccedenza> LISTEccedenza { get; internal set; }

        [HisComplexAreaInfoMapping(11, ListCount = 32)]
        public List<EccedenzaOld> LISTEccedenzaOld { get; internal set; }

        [HisComplexAreaInfoMapping(12, ListCount = 32)]
        public List<ServizioMilitare> LISTServizioMilitare { get; internal set; }

        [HisComplexAreaInfoMapping(13, ListCount = 32)]
        public List<ServizioMilitareOld> LISTServizioMilitareOld { get; internal set; }

        [HisComplexAreaInfoMapping(14, ListCount = 32)]
        public List<Articolo57> LISTArticolo57 { get; internal set; }

        [HisComplexAreaInfoMapping(15, ListCount = 32)]
        public List<Articolo57Old> LISTArticolo57Old { get; internal set; }

        [HisComplexAreaInfoMapping(16, ListCount = 32)]
        public List<TrattenutaInail> LISTTrattenutaInail { get; internal set; }

        [HisComplexAreaInfoMapping(17, ListCount = 32)]
        public List<TrattenutaInailOld> LISTTrattenutaInailOld { get; internal set; }

        [HisComplexAreaInfoMapping(18, ListCount = 32)]
        public List<TrattenutaInail2> LISTTrattenutaInail2 { get; internal set; }

        [HisComplexAreaInfoMapping(19, ListCount = 32)]
        public List<TrattenutaInail2Old> LISTTrattenutaInail2Old { get; internal set; }

        /// <summary>
        /// INI_VERS 9(08)  
        /// </summary>
        [HisFieldInfoMapping(20, 8, CobolType = CobolType.Unsigned)]
        public int INI_VERS { get; set; }

        // *                          DA AAAAMMGG (INIZIO VERSAMENTI) 16960
        /// <summary>
        /// FINE_VERS 9(08)  
        /// </summary>
        [HisFieldInfoMapping(21, 8, CobolType = CobolType.Unsigned)]
        public int FINE_VERS { get; set; }

        // *                          A AAAAMMGG (FINE VERSAMENTI)    16968
        /// <summary>
        /// ANNI_DIFF 9(02)  
        /// </summary>
        [HisFieldInfoMapping(22, 2, CobolType = CobolType.Unsigned)]
        public short ANNI_DIFF { get; set; }

        // *                          ANNI DIFFERIMENTO               16976
        /// <summary>
        /// ANNI_UTILI 9(02)  
        /// </summary>
        [HisFieldInfoMapping(23, 2, CobolType = CobolType.Unsigned)]
        public short ANNI_UTILI { get; set; }

        // *                          ANNI UTILI AL DIRITTO           16978
        /// <summary>
        /// ETA_REQUIS 9(02)  
        /// </summary>
        [HisFieldInfoMapping(24, 2, CobolType = CobolType.Unsigned)]
        public short ETA_REQUIS { get; set; }

        // *                          ETA' PERFEZIONAMENTO REQUISITI  16980
        /// <summary>
        /// MONT_CONTR_FS 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(25, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal MONT_CONTR_FS { get; set; }

        // *                          MONTANTE CONTRIB.  (C4)         16982
        // *                          GP2BB06
        /// <summary>
        /// IMP_CONTR_TOT 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(26, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal IMP_CONTR_TOT { get; set; }

        // *                          IMPORTO CONTRIBUTIVO TOTALE     16988
        // *                          GP2BB06
        /// <summary>
        /// IMP_PENS_LORDO 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(27, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal IMP_PENS_LORDO { get; set; }

        // *                          IMP. PENSIONE ANNUO LORDO       16994
        // *                          FONDO FS
        /// <summary>
        /// CAP_GP2NO29E 9(05)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(28, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal CAP_GP2NO29E { get; set; }

        // *                                                          17000
        /// <summary>
        /// FILLER X(195)  
        /// </summary>
        [HisFieldInfoMapping(29, 195)]
        public string FILLER { get; set; }

        // *                                                          17005
        #endregion Tracciato Host

        #region nested class
        public class Ago
        {
            #region Constructor
            internal Ago()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 GP5HG02-23       PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          PENSIONE AGO                    13568
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP5HG02_23 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal GP5HG02_23 { get; set; }

            // *                          PENSIONE AGO                    13568
            #endregion Tracciato Host

            #endregion Properties
        }

        public class AgoOld
        {
            #region Constructor
            internal AgoOld()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 GP5HG02-23OLD    PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          PENSIONE AGO OLD                13760
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP5HG02_23OLD 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal GP5HG02_23OLD { get; set; }

            // *                          PENSIONE AGO OLD                13760
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Fondo
        {
            #region Constructor
            internal Fondo()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 GP5HG02-24       PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          PENSIONE FONDO                  13952
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP5HG02_24 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal GP5HG02_24 { get; set; }

            // *                          PENSIONE FONDO                  13952
            #endregion Tracciato Host

            #endregion Properties
        }

        public class FondoOld
        {
            #region Constructor
            internal FondoOld()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 GP5HG02-24OLD    PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          PENSIONE FONDO OLD              14144
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP5HG02_24OLD 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal GP5HG02_24OLD { get; set; }

            // *                          PENSIONE FONDO OLD              14144
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Quota
        {
            #region Constructor
            internal Quota()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 GP5HG02-25       PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          QUOTA ART.24 2°COMMA L.337      14336
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP5HG02_25 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal GP5HG02_25 { get; set; }

            // *                          QUOTA ART.24 2°COMMA L.337      14336
            #endregion Tracciato Host

            #endregion Properties
        }

        public class QuotaOld
        {
            #region Constructor
            internal QuotaOld()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 GP5HG02-25OLD    PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          QUOTA ART.24 2°COMMA L.337 OLD  14496
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP5HG02_25OLD 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal GP5HG02_25OLD { get; set; }

            // *                          QUOTA ART.24 2°COMMA L.337 OLD  14496
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Diff
        {
            #region Constructor
            internal Diff()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 GP5HG02-26       PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          DIFF. ART.16 1°COMMA L.903      14656
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP5HG02_26 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal GP5HG02_26 { get; set; }

            // *                          DIFF. ART.16 1°COMMA L.903      14656
            #endregion Tracciato Host

            #endregion Properties
        }

        public class DiffOld
        {
            #region Constructor
            internal DiffOld()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 GP5HG02-26OLD    PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          DIFF. ART.16 1°COMMA L.903 OLD  14816
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP5HG02_26OLD 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal GP5HG02_26OLD { get; set; }

            // *                          DIFF. ART.16 1°COMMA L.903 OLD  14816
            #endregion Tracciato Host

            #endregion Properties
        }

        public class ScalaMobile
        {
            #region Constructor
            internal ScalaMobile()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 GP5HG02-27       PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          SCALA MOBILE ETC.               14976
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP5HG02_27 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal GP5HG02_27 { get; set; }

            // *                          SCALA MOBILE ETC.               14976
            #endregion Tracciato Host

            #endregion Properties
        }

        public class ScalaMobileOld
        {
            #region Constructor
            internal ScalaMobileOld()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 GP5HG02-27OLD    PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          SCALA MOBILE OLD ETC.           15136
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP5HG02_27OLD 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal GP5HG02_27OLD { get; set; }

            // *                          SCALA MOBILE OLD ETC.           15136
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Eccedenza
        {
            #region Constructor
            internal Eccedenza()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 GP5HG02-28       PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          ECCEDENZA AGO SU FONDO          15296
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP5HG02_28 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal GP5HG02_28 { get; set; }

            // *                          ECCEDENZA AGO SU FONDO          15296
            #endregion Tracciato Host

            #endregion Properties
        }

        public class EccedenzaOld
        {
            #region Constructor
            internal EccedenzaOld()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 GP5HG02-28OLD    PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          ECCEDENZA AGO SU FONDO OLD      15456
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP5HG02_28OLD 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal GP5HG02_28OLD { get; set; }

            // *                          ECCEDENZA AGO SU FONDO OLD      15456
            #endregion Tracciato Host

            #endregion Properties
        }

        public class ServizioMilitare
        {
            #region Constructor
            internal ServizioMilitare()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 GP5HG02-29       PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          SUPPL.SERVIZIO MILITARE         15616
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP5HG02_29 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal GP5HG02_29 { get; set; }

            // *                          SUPPL.SERVIZIO MILITARE         15616
            #endregion Tracciato Host

            #endregion Properties
        }

        public class ServizioMilitareOld
        {
            #region Constructor
            internal ServizioMilitareOld()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 GP5HG02-29OLD    PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          SUPPL.SERVIZIO MILITARE OLD     15776
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP5HG02_29OLD 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal GP5HG02_29OLD { get; set; }

            // *                          SUPPL.SERVIZIO MILITARE OLD     15776
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
            //     02 GP5HG02-31       PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          SUPPL.ART.57 L.377              15936
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP5HG02_31 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal GP5HG02_31 { get; set; }

            // *                          SUPPL.ART.57 L.377              15936
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Articolo57Old
        {
            #region Constructor
            internal Articolo57Old()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 GP5HG02-31OLD    PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          SUPPL.ART.57 L.337 OLD          16096
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP5HG02_31OLD 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal GP5HG02_31OLD { get; set; }

            // *                          SUPPL.ART.57 L.337 OLD          16096
            #endregion Tracciato Host

            #endregion Properties
        }

        public class TrattenutaInail
        {
            #region Constructor
            internal TrattenutaInail()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 GP5HG02-32       PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          TRATTENUTA INAIL                16256
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP5HG02_32 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal GP5HG02_32 { get; set; }

            // *                          TRATTENUTA INAIL                16256
            #endregion Tracciato Host

            #endregion Properties
        }

        public class TrattenutaInailOld
        {
            #region Constructor
            internal TrattenutaInailOld()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 GP5HG02-32OLD    PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          TRATTENUTA INAIL OLD            16416
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP5HG02_32OLD 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal GP5HG02_32OLD { get; set; }

            // *                          TRATTENUTA INAIL OLD            16416
            #endregion Tracciato Host

            #endregion Properties
        }

        public class TrattenutaInail2
        {
            #region Constructor
            internal TrattenutaInail2()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 GP5HG02-33       PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          TRATTENUTA INAIL                16576
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP5HG02_33 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal GP5HG02_33 { get; set; }

            // *                          TRATTENUTA INAIL                16576
            #endregion Tracciato Host

            #endregion Properties
        }

        public class TrattenutaInail2Old
        {
            #region Constructor
            internal TrattenutaInail2Old()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 GP5HG02-33OLD    PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          TRATTENUTA INAIL OLD            16768
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GP5HG02_33OLD 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal GP5HG02_33OLD { get; set; }

            // *                          TRATTENUTA INAIL OLD            16768
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}

