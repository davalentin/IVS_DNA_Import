using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostResponse.AreaStampa
{
    public class Decorrenze
    {
        #region Constructor
        internal Decorrenze()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 DECOR-NUM        PIC 9(02).
        //*                          NUMERO DECORRENZE                7700
        //     02 FILLER           PIC X(06).
        //*                                                           7702
        //     02 DECOR-DATA                              OCCURS 32 TIMES.
        //        03 DECOR-ANNO    PIC 9(04).
        //        03 DECOR-MESE    PIC 9(02).
        //*                          DA AAAAMM                        7708
        //     02 IMP-CALC         PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          IMP.A CALCOLO            (1)     7900
        //     02 IMP-CALC-OLD     PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          IMP.A CALCOLO OLD        (2)     8092
        //     02 IMP-MIN          PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          IMP.INTEGRATA MINIMO     (3)     8284
        //*                          KC05 FINO AL 8/95 - KC10 DAL 9/95
        //*                          PER LE PS/PSO/AS SEMPRE KC10
        //     02 IMP-MIN-OLD      PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          IMP.INTEGRATA MINIMO OLD (4)     8444
        //*                          KC05 FINO AL 8/95 - KC10 DAL 9/95
        //*                          PER LE PS/PSO/AS SEMPRE KC10
        //     02 IMP-ACCOMP       PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          IMP.ACCOMPAGNO           (5)     8604
        //     02 IMP-ACCOMP-OLD   PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          IMP.ACCOMPAGNO OLD       (6)     8764
        //     02 IMP-FAM          PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          IMP.TRATT.FAMIGLIA       (7)     8924
        //     02 IMP-FAM-OLD      PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          IMP.FAMIGLIA OLD         (8)     9084
        //* TOTALE NEW = (1 O 3) +5 +7   -    TOTALE OLD = (2 O 4) +6 +8
        //*
        //     02 MAGG-439         PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                            ULT.MAGG.SOCIALE               9244
        //     02 FILLER           PIC X(104).
        //*                                                           9404
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// DECOR_NUM 9(02)  
        /// </summary>
        [HisFieldInfoMapping(0, 2, CobolType = CobolType.Unsigned)]
        public short DECOR_NUM { get; set; }

        // *                          NUMERO DECORRENZE                7700
        /// <summary>
        /// FILLER X(06)  
        /// </summary>
        [HisFieldInfoMapping(1, 6)]
        public string FILLER1 { get; set; }

        // *                                                           7702

        [HisComplexAreaInfoMapping(2, ListCount = 32)]
        public List<DataDecorrenza> LISTDataDecorrenza { get; set; }

        [HisComplexAreaInfoMapping(3, ListCount = 32)]
        public List<ImportoCalcolo> LISTImportoCalcolo { get; set; }

        [HisComplexAreaInfoMapping(4, ListCount = 32)]
        public List<ImportoCalcoloOld> LISTImportoCalcoloOld { get; set; }

        [HisComplexAreaInfoMapping(5, ListCount = 32)]
        public List<ImportoMinimo> LISTImportoMinimo { get; set; }

        [HisComplexAreaInfoMapping(6, ListCount = 32)]
        public List<ImportoMinimoOld> LISTImportoMinimoOld { get; set; }

        [HisComplexAreaInfoMapping(7, ListCount = 32)]
        public List<ImportoAccompagno> LISTImportoAccompagno { get; set; }

        [HisComplexAreaInfoMapping(8, ListCount = 32)]
        public List<ImportoAccompagnoOld> LISTImportoAccompagnoOld { get; set; }

        [HisComplexAreaInfoMapping(9, ListCount = 32)]
        public List<ImportoTrattFamiglia> LISTImportoTrattFamiglia { get; set; }

        [HisComplexAreaInfoMapping(10, ListCount = 32)]
        public List<ImportoTrattFamigliaOld> LISTImportoTrattFamigliaOld { get; set; }

        [HisComplexAreaInfoMapping(11, ListCount = 32)]
        public List<MaggiorazioneSociale> LISTMaggiorazioneSociale { get; set; }

        /// <summary>
        /// FILLER X(104)  
        /// </summary>
        [HisFieldInfoMapping(12, 104)]
        public string FILLER2 { get; set; }

        // *                                                           9404
        #endregion Tracciato Host

        #region nested class
        public class DataDecorrenza
        {
            #region Constructor
            internal DataDecorrenza()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 DECOR-DATA                              OCCURS 32 TIMES.
            //        03 DECOR-ANNO    PIC 9(04).
            //        03 DECOR-MESE    PIC 9(02).
            //*                          DA AAAAMM                        7708
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 DECOR-DATA                              OCCURS 32 TIMES.
            /// <summary>
            /// DECOR_ANNO 9(04)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short DECOR_ANNO { get; set; }

            /// <summary>
            /// DECOR_MESE 9(02)  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short DECOR_MESE { get; set; }

            // *                          DA AAAAMM                        7708
            #endregion Tracciato Host

            #endregion Properties
        }

        public class ImportoCalcolo
        {
            #region Constructor
            internal ImportoCalcolo()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 IMP-CALC         PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          IMP.A CALCOLO            (1)     7900
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// IMP_CALC 9(07)V9(04) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal IMP_CALC { get; set; }

            // *                          IMP.A CALCOLO            (1)     7900
            #endregion Tracciato Host

            #endregion Properties
        }

        public class ImportoCalcoloOld
        {
            #region Constructor
            internal ImportoCalcoloOld()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 IMP-CALC-OLD     PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          IMP.A CALCOLO OLD        (2)     8092
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// IMP_CALC_OLD 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal IMP_CALC_OLD { get; set; }

            // *                          IMP.A CALCOLO OLD        (2)     8092
            #endregion Tracciato Host

            #endregion Properties
        }

        public class ImportoMinimo
        {
            #region Constructor
            internal ImportoMinimo()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 IMP-MIN          PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          IMP.INTEGRATA MINIMO     (3)     8284
            //*                          KC05 FINO AL 8/95 - KC10 DAL 9/95
            //*                          PER LE PS/PSO/AS SEMPRE KC10
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// IMP_MIN 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal IMP_MIN { get; set; }

            // *                          IMP.INTEGRATA MINIMO     (3)     8284
            // *                          KC05 FINO AL 8/95 - KC10 DAL 9/95
            // *                          PER LE PS/PSO/AS SEMPRE KC10
            #endregion Tracciato Host

            #endregion Properties
        }

        public class ImportoMinimoOld
        {
            #region Constructor
            internal ImportoMinimoOld()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 IMP-MIN-OLD      PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          IMP.INTEGRATA MINIMO OLD (4)     8444
            //*                          KC05 FINO AL 8/95 - KC10 DAL 9/95
            //*                          PER LE PS/PSO/AS SEMPRE KC10
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// IMP_MIN_OLD 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal IMP_MIN_OLD { get; set; }

            // *                          IMP.INTEGRATA MINIMO OLD (4)     8444
            // *                          KC05 FINO AL 8/95 - KC10 DAL 9/95
            // *                          PER LE PS/PSO/AS SEMPRE KC10
            #endregion Tracciato Host

            #endregion Properties
        }

        public class ImportoAccompagno
        {
            #region Constructor
            internal ImportoAccompagno()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 IMP-ACCOMP       PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          IMP.ACCOMPAGNO           (5)     8604
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// IMP_ACCOMP 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal IMP_ACCOMP { get; set; }

            // *                          IMP.ACCOMPAGNO           (5)     8604
            #endregion Tracciato Host

            #endregion Properties
        }

        public class ImportoAccompagnoOld
        {
            #region Constructor
            internal ImportoAccompagnoOld()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 IMP-ACCOMP-OLD   PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          IMP.ACCOMPAGNO OLD       (6)     8764
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// IMP_ACCOMP_OLD 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal IMP_ACCOMP_OLD { get; set; }

            // *                          IMP.ACCOMPAGNO OLD       (6)     8764
            #endregion Tracciato Host

            #endregion Properties
        }

        public class ImportoTrattFamiglia
        {
            #region Constructor
            internal ImportoTrattFamiglia()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 IMP-FAM          PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          IMP.TRATT.FAMIGLIA       (7)     8924
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// IMP_FAM 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal IMP_FAM { get; set; }

            // *                          IMP.TRATT.FAMIGLIA       (7)     8924
            #endregion Tracciato Host

            #endregion Properties
        }

        public class ImportoTrattFamigliaOld
        {
            #region Constructor
            internal ImportoTrattFamigliaOld()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 IMP-FAM-OLD      PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          IMP.FAMIGLIA OLD         (8)     9084
            //* TOTALE NEW = (1 O 3) +5 +7   -    TOTALE OLD = (2 O 4) +6 +8
            //*
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// IMP_FAM_OLD 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal IMP_FAM_OLD { get; set; }

            // *                          IMP.FAMIGLIA OLD         (8)     9084
            // * TOTALE NEW = (1 O 3) +5 +7   -    TOTALE OLD = (2 O 4) +6 +8
            //*
            #endregion Tracciato Host

            #endregion Properties
        }

        public class MaggiorazioneSociale
        {
            #region Constructor
            internal MaggiorazioneSociale()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 MAGG-439         PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                            ULT.MAGG.SOCIALE               9244
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// MAGG_439 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal MAGG_439 { get; set; }

            // *                            ULT.MAGG.SOCIALE               9244
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}

