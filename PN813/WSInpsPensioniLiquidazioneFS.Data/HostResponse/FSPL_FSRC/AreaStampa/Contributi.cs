using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostResponse.AreaStampa
{
    public class Contributi
    {
        #region Constructor
        internal Contributi()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 C-GP1AV08        PIC 9(04).
        //*                          N.CONTRIBUTI OBG E FIGURATIVI    5854
        //     02 C-GP1AV09        PIC 9(04).
        //*                          N.CONTRIBUTI VOLONTARI           5858
        //     02 GP1AZ11F-O       PIC 9.
        //*                          TRASF. PROVV. IN DEFINITIVA      5862
        //     02 GP1AZ11F-N       PIC 9.
        //*                          TRASF. PROVV. IN DEFINITIVA      5863
        //     02 FILLER           PIC X(96).
        //*                          LIBERI                           5864
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// C_GP1AV08 9(04)  
        /// </summary>
        [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
        public short C_GP1AV08 { get; set; }

        // *                          N.CONTRIBUTI OBG E FIGURATIVI    5854
        /// <summary>
        /// C_GP1AV09 9(04)  
        /// </summary>
        [HisFieldInfoMapping(1, 4, CobolType = CobolType.Unsigned)]
        public short C_GP1AV09 { get; set; }

        // *                          N.CONTRIBUTI VOLONTARI           5858
        /// <summary>
        /// GP1AZ11F_O 9  
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short GP1AZ11F_O { get; set; }

        // *                          TRASF. PROVV. IN DEFINITIVA      5862
        /// <summary>
        /// GP1AZ11F_N 9  
        /// </summary>
        [HisFieldInfoMapping(3, 1, CobolType = CobolType.Unsigned)]
        public short GP1AZ11F_N { get; set; }

        // *                          TRASF. PROVV. IN DEFINITIVA      5863
        /// <summary>
        /// FILLER X(96)  
        /// </summary>
        [HisFieldInfoMapping(4, 96)]
        public string FILLER { get; set; }

        // *                          LIBERI                           5864
        #endregion Tracciato Host

        #region nested class

        #endregion nested class

        #endregion Properties
    }
}

