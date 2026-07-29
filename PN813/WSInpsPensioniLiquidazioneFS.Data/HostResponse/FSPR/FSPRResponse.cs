using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

using INPS.Pensioni.LiquidazioneFs.Data.HostRequest;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostResponse
{
    public class FSPRResponse
    {
        #region Constructor
        public FSPRResponse()
        {
            this.Dati = new AreaDati();
        }
        #endregion Constructor

        #region Properties
        [HisComplexAreaInfoMapping(0)]
        public AreaDati Dati { get; set; }
        #endregion Properties

        #region Nested class

        /// <summary>
        ///  Definizione del tracciato di output
        /// </summary>
        public class AreaDati
        {
            #region Constructor
            internal AreaDati()
            {
            }
            #endregion Constructor

            #region tracciato COBOL
            //01  AREA-RICEZIONE.
            //   03 FILLER         PIC X(8) VALUE "DSOYAAAA".
            //02 AREA-CONTROLLO-PRENOTAZIONE.
            //   03 RR-TIPO        PIC X(3).
            //   03 RR-CATE        PIC 9(3).
            //   03 RR-SEDE        PIC 9(2).
            //   03 RR-ZONA        PIC 9(2).
            //   03 RR-CERT        PIC 9(8).
            //   03 RR-OPSE        PIC 9(2).
            //   03 RR-OPZO        PIC 9(2).
            //   03 RR-LAVO        PIC X(3).
            //   03 RR-ESITO       PIC X(3)
            #endregion tracciato COBOL

            #region Tracciato Host
            // 01  AREA-RICEZIONE.
            /// <summary>
            /// FILLER X(8)  
            /// </summary>
            [HisFieldInfoMapping(0, 8)]
            public string FILLER { get; set; }

            // 02 AREA-CONTROLLO-PRENOTAZIONE.
            /// <summary>
            /// RR_TIPO X(3)  
            /// </summary>
            [HisFieldInfoMapping(1, 3)]
            public string RR_TIPO { get; set; }

            /// <summary>
            /// RR_CATE 9(3)  
            /// </summary>
            [HisFieldInfoMapping(2, 3, CobolType = CobolType.Unsigned)]
            public short RR_CATE { get; set; }

            /// <summary>
            /// RR_SEDE 9(2)  
            /// </summary>
            [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
            public short RR_SEDE { get; set; }

            /// <summary>
            /// RR_ZONA 9(2)  
            /// </summary>
            [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
            public short RR_ZONA { get; set; }

            /// <summary>
            /// RR_CERT 9(8)  
            /// </summary>
            [HisFieldInfoMapping(5, 8, CobolType = CobolType.Unsigned)]
            public int RR_CERT { get; set; }

            /// <summary>
            /// RR_OPSE 9(2)  
            /// </summary>
            [HisFieldInfoMapping(6, 2, CobolType = CobolType.Unsigned)]
            public short RR_OPSE { get; set; }

            /// <summary>
            /// RR_OPZO 9(2)  
            /// </summary>
            [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
            public short RR_OPZO { get; set; }

            /// <summary>
            /// RR_LAVO X(3)  
            /// </summary>
            [HisFieldInfoMapping(8, 3)]
            public string RR_LAVO { get; set; }

            /// <summary>
            /// RR_ESITO X(3)  
            /// </summary>
            [HisFieldInfoMapping(9, 3)]
            public string RR_ESITO { get; set; }

            [HisFieldInfoMapping(10, 19964, CobolType = CobolType.Untraslate)]
            public byte[] RISP_COMPR { get; set; }
            #endregion Tracciato Host
        }

        #endregion Nested class
    }
}
