using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostResponse.AreaStampa
{
    public class Dati_Contributivi_New
    {
        #region Constructor
        internal Dati_Contributivi_New()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 FLAG-CONTR       PIC 9.
        //*                          FLAG = 1 (TABELLA VALORIZZATA)   3270
        //     02 GEST-CONTR       PIC X(03)               OCCURS 5 TIMES.
        //*                          OBG-CDM-ART-COM-PAR(POSIZIONALE) 3271
        //*                          GP2BB05N
        //     02 DEC-CONTR        PIC 9(06)               OCCURS 5 TIMES.
        //*                          DECORRENZA (AAAAMM)              3286
        //*                          GP2BB04
        //     02 MONT-CONTR       PIC 9(07)V9(04) COMP-3  OCCURS 5 TIMES.
        //*                          MONTANTE CONTRIB.  (C4)          3316
        //*                          GP2BB06
        //     02 MONT-ESCL        PIC 9(07)V9(04) COMP-3  OCCURS 5 TIMES.
        //*                          MONTANTE CONTRIB.ESCLUSIVO F.S.  3346
        //*                          GP2BB06
        //     02 IMP-CONTR        PIC 9(07)V9(04) COMP-3  OCCURS 5 TIMES.
        //*                          IMPORTO CONTRIBUTI (C5)          3376
        //*                          GP2BB07
        //     02 IMP-ESCL         PIC 9(07)V9(04) COMP-3  OCCURS 5 TIMES.
        //*                          IMPORTO CONTRIBUTI ESCLUSIVI F.S.3406
        //*                          GP2BB07
        //     02 COEF-CONTR                               OCCURS 5 TIMES.
        //        03 COEF-INT      PIC 99.
        //        03 COEF-DEC      PIC 9999.
        //*              GP2BB02     COEFF.COMMISURAZ.(C6)            3436
        //     02 QUO-CONTR        PIC 9(07)V9(04) COMP-3  OCCURS 5 TIMES.
        //*                            QUOTA CONTRIBUTIVA (C7)        3466
        //*                          DA SVILUPPO DEL CALCOLO
        //     02 SETT-CONTR       PIC 9(04)               OCCURS 5 TIMES.
        //*                          OBG-CDM-ART-COM-PAR(POSIZIONALE) 3496
        //     02 FILLER           PIC X(04).
        //*                          LIBERI                           3516
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// FLAG_CONTR 9  
        /// </summary>
        [HisFieldInfoMapping(0, 1, CobolType = CobolType.Unsigned)]
        public short FLAG_CONTR { get; set; }

        // *                          FLAG = 1 (TABELLA VALORIZZATA)   3270

        [HisComplexAreaInfoMapping(1, ListCount = 5)]
        public List<Gestione> LISTGestione { get; set; }

        [HisComplexAreaInfoMapping(2, ListCount = 5)]
        public List<Decorrenza> LISTDecorrenza { get; set; }

        [HisComplexAreaInfoMapping(3, ListCount = 5)]
        public List<Montante> LISTMontante { get; set; }

        [HisComplexAreaInfoMapping(4, ListCount = 5)]
        public List<MontanteEsclusivo> LISTMontanteEsclusivo { get; set; }

        [HisComplexAreaInfoMapping(5, ListCount = 5)]
        public List<Importo> LISTImporto { get; set; }

        [HisComplexAreaInfoMapping(6, ListCount = 5)]
        public List<ImportoEsclusivo> LISTImportoEsclusivo { get; set; }

        [HisComplexAreaInfoMapping(7, ListCount = 5)]
        public List<Commisurazione> LISTCommisurazione { get; set; }

        [HisComplexAreaInfoMapping(8, ListCount = 5)]
        public List<Quota> LISTQuota { get; set; }

        [HisComplexAreaInfoMapping(9, ListCount = 5)]
        public List<Settimane> LISTSettimane { get; set; }

        /// <summary>
        /// FILLER X(04)  
        /// </summary>
        [HisFieldInfoMapping(10, 4)]
        public string FILLER { get; set; }

        // *                          LIBERI                           3516
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
            //     02 GEST-CONTR       PIC X(03)               OCCURS 5 TIMES.
            //*                          OBG-CDM-ART-COM-PAR(POSIZIONALE) 3271
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GEST_CONTR X(03)  
            /// </summary>
            [HisFieldInfoMapping(0, 3)]
            public string GEST_CONTR { get; set; }

            // *                          OBG-CDM-ART-COM-PAR(POSIZIONALE) 3271
            // *                          GP2BB05N
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Decorrenza
        {
            #region Constructor
            internal Decorrenza()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 DEC-CONTR        PIC 9(06)               OCCURS 5 TIMES.
            //*                          DECORRENZA (AAAAMM)              3286
            //*                          GP2BB04
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// DEC_CONTR 9(06)  
            /// </summary>
            [HisFieldInfoMapping(0, 6, CobolType = CobolType.Unsigned)]
            public int DEC_CONTR { get; set; }

            // *                          DECORRENZA (AAAAMM)              3286
            // *                          GP2BB04
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Montante
        {
            #region Constructor
            internal Montante()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 MONT-CONTR       PIC 9(07)V9(04) COMP-3  OCCURS 5 TIMES.
            //*                          MONTANTE CONTRIB.  (C4)          3316
            //*                          GP2BB06
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// MONT_CONTR 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal MONT_CONTR { get; set; }

            // *                          MONTANTE CONTRIB.  (C4)          3316
            // *                          GP2BB06
            #endregion Tracciato Host

            #endregion Properties
        }

        public class MontanteEsclusivo
        {
            #region Constructor
            internal MontanteEsclusivo()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 MONT-ESCL        PIC 9(07)V9(04) COMP-3  OCCURS 5 TIMES.
            //*                          MONTANTE CONTRIB.ESCLUSIVO F.S.  3346
            //*                          GP2BB06
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// MONT_ESCL 9(07)V9(04) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal MONT_ESCL { get; set; }

            // *                          MONTANTE CONTRIB.ESCLUSIVO F.S.  3346
            // *                          GP2BB06
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
            //     02 IMP-CONTR        PIC 9(07)V9(04) COMP-3  OCCURS 5 TIMES.
            //*                          IMPORTO CONTRIBUTI (C5)          3376
            //*                          GP2BB07
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// IMP_CONTR 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal IMP_CONTR { get; set; }

            // *                          IMPORTO CONTRIBUTI (C5)          3376
            // *                          GP2BB07
            #endregion Tracciato Host

            #endregion Properties
        }

        public class ImportoEsclusivo
        {
            #region Constructor
            internal ImportoEsclusivo()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 IMP-ESCL         PIC 9(07)V9(04) COMP-3  OCCURS 5 TIMES.
            //*                          IMPORTO CONTRIBUTI ESCLUSIVI F.S.3406
            //*                          GP2BB07
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// IMP_ESCL 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal IMP_ESCL { get; set; }

            // *                          IMPORTO CONTRIBUTI ESCLUSIVI F.S.3406
            // *                          GP2BB07
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Commisurazione
        {
            #region Constructor
            internal Commisurazione()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 COEF-CONTR                               OCCURS 5 TIMES.
            //        03 COEF-INT      PIC 99.
            //        03 COEF-DEC      PIC 9999.
            //*              GP2BB02     COEFF.COMMISURAZ.(C6)            3436
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 COEF-CONTR                               OCCURS 5 TIMES.
            /// <summary>
            /// COEF_INT 99  
            /// </summary>
            [HisFieldInfoMapping(0, 2, CobolType = CobolType.Unsigned)]
            public short COEF_INT { get; set; }
            /// <summary>
            /// COEF_DEC 9999  
            /// </summary>
            [HisFieldInfoMapping(1, 4, CobolType = CobolType.Unsigned)]
            public short COEF_DEC { get; set; }

            // *              GP2BB02     COEFF.COMMISURAZ.(C6)            3436
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
            //     02 QUO-CONTR        PIC 9(07)V9(04) COMP-3  OCCURS 5 TIMES.
            //*                            QUOTA CONTRIBUTIVA (C7)        3466
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// QUO_CONTR 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal QUO_CONTR { get; set; }

            // *                            QUOTA CONTRIBUTIVA (C7)        3466
            // *                          DA SVILUPPO DEL CALCOLO
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
            //     02 SETT-CONTR       PIC 9(04)               OCCURS 5 TIMES.
            //*                          OBG-CDM-ART-COM-PAR(POSIZIONALE) 3496
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// SETT_CONTR 9(04)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short SETT_CONTR { get; set; }

            // *                          OBG-CDM-ART-COM-PAR(POSIZIONALE) 3496
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}

