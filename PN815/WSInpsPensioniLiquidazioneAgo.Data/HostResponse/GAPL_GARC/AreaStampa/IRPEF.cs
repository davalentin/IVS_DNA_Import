using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostResponse.AreaStampa
{
    public class IRPEF
    {
        #region Constructor
        internal IRPEF()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 IRPEF            PIC S9(07)V9(04) COMP-3 OCCURS 13 TIMES.
        //*                          IMPORTO IRPEF                    7500
        //     02 FILLER           PIC X(122).
        //*                          LIBERI                           7578
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 13)]
        public List<ImportoIRPEF> LISTImportoIRPEF { get; internal set; }

        /// <summary>
        /// FILLER X(122)  
        /// </summary>
        [HisFieldInfoMapping(1, 122)]
        public string FILLER { get; set; }

        // *                          LIBERI                           7578
        #endregion Tracciato Host

        #region nested class
        public class ImportoIRPEF
        {
            #region Constructor
            internal ImportoIRPEF()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 IRPEF            PIC S9(07)V9(04) COMP-3 OCCURS 13 TIMES.
            //*                          IMPORTO IRPEF                    7500
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// IRPEF S9(07)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal IRPEF { get; set; }

            // *                          IMPORTO IRPEF                    7500
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}

