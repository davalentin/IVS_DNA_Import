using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostResponse.AreaStampa
{
    public class Trattenute
    {
        #region Constructor
        internal Trattenute()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 ONPI             PIC 9(01)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          O.N.P.I.                         9508
        //     02 ONPI-OLD         PIC 9(01)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          O.N.P.I.                         9604
        //     02 TRT-SIND         PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          QUOTA ASS. SINDACALE             9700
        //     02 TRT-SIND-OLD     PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          QUOTA SINDACALE OLD              9860
        //     02 TRT-DIP          PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          TRATTENUTA REDDITI LAVORO DIP.  10020
        //*                          LEGGE 335/95 COMMA 42
        //     02 TRT-DIP-OLD      PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          TRATT.REDDITI LAVORO DIP. OLD   10212
        //*                          LEGGE 335/95 COMMA 42
        //     02 TRT-INF          PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          TRATTENUTA RENDITA INFORTUNI    10404
        //*                          LEGGE 335/95 COMMA 43
        //     02 TRT-INF-OLD      PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          TRATT.RENDITA INFORTUNI OLD     10596
        //*                          LEGGE 335/95 COMMA 43
        //     02 TRT-AUT          PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          TRATT.LAVORO AUTONOMO           10788
        //     02 TRT-AUT-OLD      PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          TRATT.LAVORO AUT.OLD            10980
        //     02 TRT-SOLID        PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          TRATT.CONTRIBUTO SOLIDARIETA'   11172
        //*                          GP6/GP5HG01 FONDO 196
        //     02 TRT-SOLID-OLD    PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                         TRATT.CONTRIBUTO SOLIDARIETA'OLD 11364
        //*                          GP6/GP5HG01 FONDO 196
        //     02 TRT-SOLID-PI     PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          TRATT.CONTRIBUTO SOLIDARIETA'   11556
        //*                          FONDO 173
        //     02 TRT-SOL-PI-OLD    PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                         TRATT.CONTRIBUTO SOLIDARIETA'OLD 11748
        //*                          FONDO 173
        //     02 TRT-ESTERO       PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          TRATTENUTA LAVORO DIP.ESTERO    11940
        //*                          GP6/GP5HG01 FONDO 87
        //     02 TRT-EST-OLD      PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          TRATT.LAVORO DIP.ESTERO OLD     12132
        //*                          GP6/GP5HG01 FONDO 87
        //*                                                          12324
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 32)]
        public List<ONPI> LISTONPI { get; set; }

        [HisComplexAreaInfoMapping(1, ListCount = 32)]
        public List<ONPI_Old> LISTONPI_Old { get; set; }

        [HisComplexAreaInfoMapping(2, ListCount = 32)]
        public List<TrattenutaQuotaSindacale> LISTTrattenutaQuotaSindacale { get; set; }

        [HisComplexAreaInfoMapping(3, ListCount = 32)]
        public List<TrattenutaQuotaSindacaleOld> LISTTrattenutaQuotaSindacaleOld { get; set; }

        [HisComplexAreaInfoMapping(4, ListCount = 32)]
        public List<TrattenutaDipendente> LISTTrattenutaDipendente { get; set; }

        [HisComplexAreaInfoMapping(5, ListCount = 32)]
        public List<TrattenutaDipendenteOld> LISTTrattenutaDipendenteOld { get; set; }

        [HisComplexAreaInfoMapping(6, ListCount = 32)]
        public List<TrattenutaInfortuni> LISTTrattenutaInfortuni { get; set; }

        [HisComplexAreaInfoMapping(7, ListCount = 32)]
        public List<TrattenutaInfortuniOld> LISTTrattenutaInfortuniOld { get; set; }

        [HisComplexAreaInfoMapping(8, ListCount = 32)]
        public List<TrattenutaAutonomo> LISTTrattenutaAutonomo { get; set; }

        [HisComplexAreaInfoMapping(9, ListCount = 32)]
        public List<TrattenutaAutonomoOld> LISTTrattenutaAutonomoOld{ get; set; }

        [HisComplexAreaInfoMapping(10, ListCount = 32)]
        public List<TrattenutaSolidarieta> LISTTrattenutaSolidarieta { get; set; }

        [HisComplexAreaInfoMapping(11, ListCount = 32)]
        public List<TrattenutaSolidarietaOld> LISTTrattenutaSolidarietaOld { get; set; }

        [HisComplexAreaInfoMapping(12, ListCount = 32)]
        public List<TrattenutaSolidarietaPI> LISTTrattenutaSolidarietaPI { get; set; }

        [HisComplexAreaInfoMapping(13, ListCount = 32)]
        public List<TrattenutaSolidarietaOldPI> LISTTrattenutaSolidarietaOldPI { get; set; }

        [HisComplexAreaInfoMapping(14, ListCount = 32)]
        public List<TrattenutaEstero> LISTTrattenutaEstero { get; set; }

        [HisComplexAreaInfoMapping(15, ListCount = 32)]
        public List<TrattenutaEsteroOld> LISTTrattenutaEsteroOld { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class ONPI
        {
            #region Constructor
            internal ONPI()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 ONPI             PIC 9(01)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          O.N.P.I.                         9508
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// ONPI 9(01)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 3, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal O_N_P_I { get; set; }

            // *                          O.N.P.I.                         9508
            #endregion Tracciato Host

            #endregion Properties
        }

        public class ONPI_Old
        {
            #region Constructor
            internal ONPI_Old()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 ONPI-OLD         PIC 9(01)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          O.N.P.I.                         9604
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// ONPI_OLD 9(01)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 3, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal O_N_P_I_OLD { get; set; }

            // *                          O.N.P.I.                         9604
            #endregion Tracciato Host

            #endregion Properties
        }

        public class TrattenutaQuotaSindacale
        {
            #region Constructor
            internal TrattenutaQuotaSindacale()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 TRT-SIND         PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          QUOTA ASS. SINDACALE             9700
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// TRT_SIND 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal TRT_SIND { get; set; }

            // *                          QUOTA ASS. SINDACALE             9700
            #endregion Tracciato Host

            #endregion Properties
        }

        public class TrattenutaQuotaSindacaleOld
        {
            #region Constructor
            internal TrattenutaQuotaSindacaleOld()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 TRT-SIND-OLD     PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          QUOTA SINDACALE OLD              9860
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// TRT_SIND_OLD 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal TRT_SIND_OLD { get; set; }

            // *                          QUOTA SINDACALE OLD              9860
            #endregion Tracciato Host

            #endregion Properties
        }

        public class TrattenutaDipendente
        {
            #region Constructor
            internal TrattenutaDipendente()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 TRT-DIP          PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          TRATTENUTA REDDITI LAVORO DIP.  10020
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// TRT_DIP 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal TRT_DIP { get; set; }

            // *                          TRATTENUTA REDDITI LAVORO DIP.  10020
            // *                          LEGGE 335/95 COMMA 42
            #endregion Tracciato Host

            #endregion Properties
        }

        public class TrattenutaDipendenteOld
        {
            #region Constructor
            internal TrattenutaDipendenteOld()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 TRT-DIP-OLD      PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          TRATT.REDDITI LAVORO DIP. OLD   10212
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// TRT_DIP_OLD 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal TRT_DIP_OLD { get; set; }

            // *                          TRATT.REDDITI LAVORO DIP. OLD   10212
            // *                          LEGGE 335/95 COMMA 42
            #endregion Tracciato Host

            #endregion Properties
        }

        public class TrattenutaInfortuni
        {
            #region Constructor
            internal TrattenutaInfortuni()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 TRT-INF          PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          TRATTENUTA RENDITA INFORTUNI    10404
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// TRT_INF 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal TRT_INF { get; set; }

            // *                          TRATTENUTA RENDITA INFORTUNI    10404
            // *                          LEGGE 335/95 COMMA 43
            #endregion Tracciato Host

            #endregion Properties
        }

        public class TrattenutaInfortuniOld
        {
            #region Constructor
            internal TrattenutaInfortuniOld()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 TRT-INF-OLD      PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          TRATT.RENDITA INFORTUNI OLD     10596
            //*                          LEGGE 335/95 COMMA 43
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// TRT_INF_OLD 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal TRT_INF_OLD { get; set; }

            // *                          TRATT.RENDITA INFORTUNI OLD     10596
            // *                          LEGGE 335/95 COMMA 43
            #endregion Tracciato Host

            #endregion Properties
        }

        public class TrattenutaAutonomo
        {
            #region Constructor
            internal TrattenutaAutonomo()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 TRT-AUT          PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          TRATT.LAVORO AUTONOMO           10788
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// TRT_AUT 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal TRT_AUT { get; set; }

            // *                          TRATT.LAVORO AUTONOMO           10788
            #endregion Tracciato Host

            #endregion Properties
        }

        public class TrattenutaAutonomoOld
        {
            #region Constructor
            internal TrattenutaAutonomoOld()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 TRT-AUT-OLD      PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          TRATT.LAVORO AUT.OLD            10980
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// TRT_AUT_OLD 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal TRT_AUT_OLD { get; set; }

            // *                          TRATT.LAVORO AUT.OLD            10980
            #endregion Tracciato Host

            #endregion Properties
        }

        public class TrattenutaSolidarieta
        {
            #region Constructor
            internal TrattenutaSolidarieta()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 TRT-SOLID        PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          TRATT.CONTRIBUTO SOLIDARIETA'   11172
            //*                          GP6/GP5HG01 FONDO 196
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// TRT_SOLID 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal TRT_SOLID { get; set; }

            // *                          TRATT.CONTRIBUTO SOLIDARIETA'   11172
            // *                          GP6/GP5HG01 FONDO 196
            #endregion Tracciato Host

            #endregion Properties
        }

        public class TrattenutaSolidarietaOld
        {
            #region Constructor
            internal TrattenutaSolidarietaOld()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 TRT-SOLID-OLD    PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                         TRATT.CONTRIBUTO SOLIDARIETA'OLD 11364
            //*                          GP6/GP5HG01 FONDO 196
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// TRT_SOLID_OLD 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal TRT_SOLID_OLD { get; set; }

            // *                         TRATT.CONTRIBUTO SOLIDARIETA'OLD 11364
            // *                          GP6/GP5HG01 FONDO 196
            #endregion Tracciato Host

            #endregion Properties
        }

        public class TrattenutaSolidarietaPI
        {
            #region Constructor
            internal TrattenutaSolidarietaPI()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 TRT-SOLID-PI     PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          TRATT.CONTRIBUTO SOLIDARIETA'   11556
            //*                          FONDO 173
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// TRT_SOLID_PI 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal TRT_SOLID_PI { get; set; }

            // *                          TRATT.CONTRIBUTO SOLIDARIETA'   11556
            // *                          FONDO 173
            #endregion Tracciato Host

            #endregion Properties
        }

        public class TrattenutaSolidarietaOldPI
        {
            #region Constructor
            internal TrattenutaSolidarietaOldPI()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 TRT-SOL-PI-OLD    PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                         TRATT.CONTRIBUTO SOLIDARIETA'OLD 11748
            //*                          FONDO 173
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// TRT_SOL_PI_OLD 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal TRT_SOL_PI_OLD { get; set; }

            // *                         TRATT.CONTRIBUTO SOLIDARIETA'OLD 11748
            // *                          FONDO 173
            #endregion Tracciato Host

            #endregion Properties
        }

        public class TrattenutaEstero
        {
            #region Constructor
            internal TrattenutaEstero()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 TRT-ESTERO       PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          TRATTENUTA LAVORO DIP.ESTERO    11940
            //*                          GP6/GP5HG01 FONDO 87
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// TRT_ESTERO 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal TRT_ESTERO { get; set; }

            // *                          TRATTENUTA LAVORO DIP.ESTERO    11940
            // *                          GP6/GP5HG01 FONDO 87
            #endregion Tracciato Host

            #endregion Properties
        }

        public class TrattenutaEsteroOld
        {
            #region Constructor
            internal TrattenutaEsteroOld()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 TRT-EST-OLD      PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          TRATT.LAVORO DIP.ESTERO OLD     12132
            //*                          GP6/GP5HG01 FONDO 87
            //*                                                          12324
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// TRT_EST_OLD 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal TRT_EST_OLD { get; set; }

            // *                          TRATT.LAVORO DIP.ESTERO OLD     12132
            // *                          GP6/GP5HG01 FONDO 87
            // *                                                          12324
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
