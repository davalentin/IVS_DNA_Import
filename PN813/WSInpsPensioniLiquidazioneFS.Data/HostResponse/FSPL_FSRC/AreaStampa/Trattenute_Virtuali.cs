using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostResponse.AreaStampa
{
    public class Trattenute_Virtuali
    {
        #region Constructor
        internal Trattenute_Virtuali()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 NOCUM-DIP        PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          TRATT.VIRTUALE LAVORO DIP.      12800
        //     02 NOCUM-DIP-OLD    PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          TRATT.VIRTUALE LAVORO DIP.OLD   12992
        //     02 NOCUM-AUT        PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          TRATT.VIRTUALE LAVOR.AUTONOMI   13184
        //     02 NOCUM-AUT-OLD    PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                          TRATT.VIRTUALE AUTONOMI OLD     13376
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 32)]
        public List<LavoroDipendente> LISTLavoroDipendente { get; set; }

        [HisComplexAreaInfoMapping(1, ListCount = 32)]
        public List<LavoroDipendenteOld> LISTLavoroDipendenteOld { get; set; }

        [HisComplexAreaInfoMapping(2, ListCount = 32)]
        public List<LavoroAutonomo> LISTLavoroAutonomo { get; set; }

        [HisComplexAreaInfoMapping(3, ListCount = 32)]
        public List<LavoroAutonomoOld> LISTLavoroAutonomoOld { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class LavoroDipendente
        {
            #region Constructor
            internal LavoroDipendente()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 NOCUM-DIP        PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          TRATT.VIRTUALE LAVORO DIP.      12800
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// NOCUM_DIP 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal NOCUM_DIP { get; set; }

            // *                          TRATT.VIRTUALE LAVORO DIP.      12800
            #endregion Tracciato Host

            #endregion Properties
        }

        public class LavoroDipendenteOld
        {
            #region Constructor
            internal LavoroDipendenteOld()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 NOCUM-DIP-OLD    PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          TRATT.VIRTUALE LAVORO DIP.OLD   12992
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// NOCUM_DIP_OLD 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal NOCUM_DIP_OLD { get; set; }

            // *                          TRATT.VIRTUALE LAVORO DIP.OLD   12992
            #endregion Tracciato Host

            #endregion Properties
        }

        public class LavoroAutonomo
        {
            #region Constructor
            internal LavoroAutonomo()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 NOCUM-AUT        PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          TRATT.VIRTUALE LAVOR.AUTONOMI   13184
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// NOCUM_AUT 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal NOCUM_AUT { get; set; }

            // *                          TRATT.VIRTUALE LAVOR.AUTONOMI   13184
            #endregion Tracciato Host

            #endregion Properties
        }

        public class LavoroAutonomoOld
        {
            #region Constructor
            internal LavoroAutonomoOld()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 NOCUM-AUT-OLD    PIC 9(07)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                          TRATT.VIRTUALE AUTONOMI OLD     13376
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// NOCUM_AUT_OLD 9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal NOCUM_AUT_OLD { get; set; }

            // *                          TRATT.VIRTUALE AUTONOMI OLD     13376
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
