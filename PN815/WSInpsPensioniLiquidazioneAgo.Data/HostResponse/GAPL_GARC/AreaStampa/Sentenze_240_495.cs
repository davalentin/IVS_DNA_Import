using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostResponse.AreaStampa
{
    public class Sentenze_240_495
    {
        #region Constructor
        internal Sentenze_240_495()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 FLAG-SENT        PIC 9(01).
        //*                          FLAG APPLICAZIONE DI SENTENZA    2400
        //*                          NELL'ATTUALE RICOST.
        //*       0 = NO SENTENZE         1 = SENTENZA 495/93
        //*       2 = SENTENZA 240/94     3 = SENTENZA 495/93 E 240/94
        //     02 PENS-SENT        PIC 9(01).
        //*                          FLAG APPLICAZIONE DI SENTENZA    2401
        //*                          NEL CORSO DELLA PENS.
        //*       0 = NO SENTENZE         1 = SENTENZA 495/93
        //*       2 = SENTENZA 240/94     3 = SENTENZA 495/93 E 240/94
        //     02 ARR-ANTE96       PIC 9(07)V9(04) COMP-3.
        //*                          IMP.ARR. ANTE 1/1/96             2402
        //     02 ARR-POST95       PIC 9(07)V9(04) COMP-3.
        //*                            IMP.ARR. POST 1/1/96           2408
        //     02 FILLER           PIC X(36).
        //*                          LIBERI                           2414
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// FLAG_SENT 9(01)  
        /// </summary>
        [HisFieldInfoMapping(0, 1, CobolType = CobolType.Unsigned)]
        public short FLAG_SENT { get; set; }

        // *                          FLAG APPLICAZIONE DI SENTENZA    2400
        // *                          NELL'ATTUALE RICOST.
        // *       0 = NO SENTENZE         1 = SENTENZA 495/93
        // *       2 = SENTENZA 240/94     3 = SENTENZA 495/93 E 240/94
        /// <summary>
        /// PENS_SENT 9(01)  
        /// </summary>
        [HisFieldInfoMapping(1, 1, CobolType = CobolType.Unsigned)]
        public short PENS_SENT { get; set; }

        // *                          FLAG APPLICAZIONE DI SENTENZA    2401
        // *                          NEL CORSO DELLA PENS.
        // *       0 = NO SENTENZE         1 = SENTENZA 495/93
        // *       2 = SENTENZA 240/94     3 = SENTENZA 495/93 E 240/94
        /// <summary>
        /// ARR_ANTE96 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(2, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal ARR_ANTE96 { get; set; }

        // *                          IMP.ARR. ANTE 1/1/96             2402
        /// <summary>
        /// ARR_POST95 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(3, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal ARR_POST95 { get; set; }

        // *                            IMP.ARR. POST 1/1/96           2408
        /// <summary>
        /// FILLER X(36)  
        /// </summary>
        [HisFieldInfoMapping(4, 36)]
        public string FILLER { get; set; }

        // *                          LIBERI                           2414
        #endregion Tracciato Host

        #region nested class

        #endregion nested class

        #endregion Properties
    }
}

