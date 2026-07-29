using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostResponse.AreaStampa
{
    public class Dati_Contributivi_New1
    {
        #region Constructor
        internal Dati_Contributivi_New1()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 FLAG-CONTR1      PIC 9.
        //*                          FLAG = 1 (TABELLA VALORIZZATA)   3520
        //     02 GEST-CONTR1      PIC X(03)              OCCURS 10 TIMES.
        //*                          OBG-CDM-ART-COM-PAR(POSIZIONALE) 3521
        //*                          DAI-DAI             GP2BB05N
        //     02 DEC-CONTR1       PIC 9(06)              OCCURS 10 TIMES.
        //*                          DECORRENZA (AAAAMM)              3551
        //*                          GP2BB04
        //     02 MONT-CONTR1      PIC 9(07)V9(04) COMP-3 OCCURS 10 TIMES.
        //*                          MONTANTE CONTRIB.  (C4)          3611
        //*                          GP2BB06
        //     02 MONT-ESCL1       PIC 9(07)V9(04) COMP-3 OCCURS 10 TIMES.
        //*                          MONTANTE CONTRIB.ESCLUSIVO F.S.  3671
        //*                          GP2BB06
        //     02 IMP-CONTR1       PIC 9(07)V9(04) COMP-3 OCCURS 10 TIMES.
        //*                          IMPORTO CONTRIBUTI (C5)          3731
        //*                          GP2BB07
        //     02 IMP-ESCL1        PIC 9(07)V9(04) COMP-3 OCCURS 10 TIMES.
        //*                          IMPORTO CONTRIBUTI ESCLUSIVI F.S.3791
        //*                          GP2BB07
        //     02 COEF-CONTR1                             OCCURS 10 TIMES.
        //        03 COEF-INT1     PIC 99.
        //        03 COEF-DEC1     PIC 9999.
        //*              GP2BB02     COEFF.COMMISURAZ.(C6)            3851
        //     02 QUO-CONTR1       PIC 9(07)V9(04) COMP-3 OCCURS 10 TIMES.
        //*                            QUOTA CONTRIBUTIVA (C7)        3911
        //*                          DA SVILUPPO DEL CALCOLO
        //     02 SETT-CONTR1      PIC 9(04)              OCCURS 10 TIMES.
        //*                          SETTIMANE CONTRIBUTIVA           3971
        //     02 FILLER           PIC X(139).
        //*                          LIBERI                           4011
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// FLAG_CONTR1 9  
        /// </summary>
        [HisFieldInfoMapping(0, 1, CobolType = CobolType.Unsigned)]
        public short FLAG_CONTR1 { get; set; }

        // *                          FLAG = 1 (TABELLA VALORIZZATA)   3520

        [HisComplexAreaInfoMapping(1, ListCount = 10)]
        public List<Gestione> LISTGestione { get; set; }

        [HisComplexAreaInfoMapping(2, ListCount = 10)]
        public List<Decorrenza> LISTDecorrenza { get; set; }

        [HisComplexAreaInfoMapping(3, ListCount = 10)]
        public List<Montante> LISTMontante { get; set; }

        [HisComplexAreaInfoMapping(4, ListCount = 10)]
        public List<MontanteEsclusivo> LISTMontanteEsclusivo { get; set; }

        [HisComplexAreaInfoMapping(5, ListCount = 10)]
        public List<Importo> LISTImporto { get; set; }

        [HisComplexAreaInfoMapping(6, ListCount = 10)]
        public List<ImportoEsclusivo> LISTImportoEsclusivo { get; set; }

        [HisComplexAreaInfoMapping(7, ListCount = 10)]
        public List<Commisurazione> LISTCommisurazione { get; set; }

        [HisComplexAreaInfoMapping(8, ListCount = 10)]
        public List<Quota> LISTQuota { get; set; }

        [HisComplexAreaInfoMapping(9, ListCount = 10)]
        public List<Settimane> LISTSettimane { get; set; }

        /// <summary>
        /// FILLER X(139)  
        /// </summary>
        [HisFieldInfoMapping(10, 139)]
        public string FILLER { get; set; }

        // *                          LIBERI                           4011
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
            //     02 GEST-CONTR1       PIC X(03)               OCCURS 10 TIMES.
            //*                          OBG-CDM-ART-COM-PAR(POSIZIONALE) 3521
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GEST_CONTR1 X(03)  
            /// </summary>
            [HisFieldInfoMapping(0, 3)]
            public string GEST_CONTR1 { get; set; }

            // *                          OBG-CDM-ART-COM-PAR(POSIZIONALE) 3521
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
            //     02 DEC-CONTR1        PIC 9(06)               OCCURS 10 TIMES.
            //*                          DECORRENZA (AAAAMM)              3551
            //*                          GP2BB04
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// DEC_CONTR1 9(06)  
            /// </summary>
            [HisFieldInfoMapping(0, 6, CobolType = CobolType.Unsigned)]
            public int DEC_CONTR1 { get; set; }

            // *                          DECORRENZA (AAAAMM)              3551
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
            //     02 MONT-CONTR1       PIC 9(07)V9(04) COMP-3  OCCURS 10 TIMES.
            //*                          MONTANTE CONTRIB.  (C4)          3611
            //*                          GP2BB06
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// MONT_CONTR1 9(07)V9(04) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal MONT_CONTR1 { get; set; }

            // *                          MONTANTE CONTRIB.  (C4)          3611
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
            //     02 MONT-ESCL1        PIC 9(07)V9(04) COMP-3  OCCURS 10 TIMES.
            //*                          MONTANTE CONTRIB.ESCLUSIVO F.S.  3671
            //*                          GP2BB06
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// MONT_ESCL1 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal MONT_ESCL1 { get; set; }

            // *                          MONTANTE CONTRIB.ESCLUSIVO F.S.  3671
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
            //     02 IMP-CONTR1        PIC 9(07)V9(04) COMP-3  OCCURS 10 TIMES.
            //*                          IMPORTO CONTRIBUTI (C5)          3731
            //*                          GP2BB07
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// IMP_CONTR1 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal IMP_CONTR1 { get; set; }

            // *                          IMPORTO CONTRIBUTI (C5)          3731
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
            //     02 IMP-ESCL1         PIC 9(07)V9(04) COMP-3  OCCURS 10 TIMES.
            //*                          IMPORTO CONTRIBUTI ESCLUSIVI F.S.3791
            //*                          GP2BB07
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// IMP_ESCL1 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal IMP_ESCL1 { get; set; }

            // *                          IMPORTO CONTRIBUTI ESCLUSIVI F.S.3791
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
            //     02 COEF-CONTR1                               OCCURS 10 TIMES.
            //        03 COEF-INT1      PIC 99.
            //        03 COEF-DEC1      PIC 9999.
            //*              GP2BB02     COEFF.COMMISURAZ.(C6)            3851
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 COEF-CONTR1                               OCCURS 10 TIMES.
            /// <summary>
            /// COEF_INT1 99  
            /// </summary>
            [HisFieldInfoMapping(0, 2, CobolType = CobolType.Unsigned)]
            public short COEF_INT1 { get; set; }
            /// <summary>
            /// COEF_DEC1 9999  
            /// </summary>
            [HisFieldInfoMapping(1, 4, CobolType = CobolType.Unsigned)]
            public short COEF_DEC1 { get; set; }

            // *              GP2BB02     COEFF.COMMISURAZ.(C6)            3851
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
            //     02 QUO-CONTR1        PIC 9(07)V9(04) COMP-3  OCCURS 10 TIMES.
            //*                            QUOTA CONTRIBUTIVA (C7)        3911
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// QUO_CONTR1 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal QUO_CONTR1 { get; set; }

            // *                            QUOTA CONTRIBUTIVA (C7)        3911
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
            //     02 SETT-CONTR1       PIC 9(04)               OCCURS 10 TIMES.
            //*                          OBG-CDM-ART-COM-PAR(POSIZIONALE) 3971
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// SETT_CONTR1 9(04)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short SETT_CONTR1 { get; set; }

            // *                          OBG-CDM-ART-COM-PAR(POSIZIONALE) 3971
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}


