using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostResponse.AreaStampa
{
    public class Fondo_Spedizionieri
    {
        #region Constructor
        internal Fondo_Spedizionieri()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 SPED-ANNI        PIC 9(02).
        //*                          ANNI DI ANZIANITA'               4150
        //     02 SPED-QUOTA       PIC 9(07)V9(04) COMP-3.
        //*                          IMPORTO PENSIONE SPEDIZIONIERI   4152
        //*                            (58.056 X GP1AV08/52)
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// SPED_ANNI 9(02)  
        /// </summary>
        [HisFieldInfoMapping(0, 2, CobolType = CobolType.Unsigned)]
        public short SPED_ANNI { get; set; }

        // *                          ANNI DI ANZIANITA'               4150
        /// <summary>
        /// SPED_QUOTA 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(1, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal SPED_QUOTA { get; set; }

        // *                          IMPORTO PENSIONE SPEDIZIONIERI   4152
        // *                            (58.056 X GP1AV08/52)
        #endregion Tracciato Host

        #region nested class

        #endregion nested class

        #endregion Properties
    }
}

