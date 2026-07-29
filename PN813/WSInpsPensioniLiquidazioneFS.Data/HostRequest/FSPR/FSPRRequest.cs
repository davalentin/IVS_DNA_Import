using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostRequest
{
    public class FSPRRequest
    {
         #region Constructor
        internal FSPRRequest()
		{

		}
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //    01  AREA-TRASMISSIONE.
        //02 AREA-IMS.
        //   03 AR-LNG         PIC 9(4). non occorre inserirlo
        //   03 AR-TRA         PIC X(8) VALUE "FSPR    ". non occorre inserirlo
        //   03 FILLER         PIC X(8) VALUE "DSOYAAAA". aggiungo 3 spazi in testa per replicare il preliminary filler di 3
        //02 AREA-CONTROLLO-PRENOTAZIONE.
        //   03 PR-TIPO        PIC X(3).
        //   03 PR-CATE        PIC 9(3).
        //   03 PR-SEDE        PIC 9(2).
        //   03 PR-ZONA        PIC 9(2).
        //   03 PR-CERT        PIC 9(8).
        //   03 PR-OPSE        PIC 9(2).
        //   03 PR-OPZO        PIC 9(2).
        //   03 PR-LAVO        PIC X(3).
        //   03 PR-ESITO       PIC X(3) VALUE SPACES.
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  AREA-TRASMISSIONE.
        // 02 AREA-IMS.
        /// <summary>
        /// FILLER X(11)  
        /// </summary>
        [HisFieldInfoMapping(0, 11)]
        public string FILLER { get; set; }

        // 02 AREA-CONTROLLO-PRENOTAZIONE.
        /// <summary>
        /// PR_TIPO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string PR_TIPO { get; set; }

        /// <summary>
        /// PR_CATE 9(3)  
        /// </summary>
        [HisFieldInfoMapping(2, 3, CobolType = CobolType.Unsigned)]
        public short PR_CATE { get; set; }

        /// <summary>
        /// PR_SEDE 9(2)  
        /// </summary>
        [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
        public short PR_SEDE { get; set; }

        /// <summary>
        /// PR_ZONA 9(2)  
        /// </summary>
        [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
        public short PR_ZONA { get; set; }

        /// <summary>
        /// PR_CERT 9(8)  
        /// </summary>
        [HisFieldInfoMapping(5, 8, CobolType = CobolType.Unsigned)]
        public int PR_CERT { get; set; }

        /// <summary>
        /// PR_OPSE 9(2)  
        /// </summary>
        [HisFieldInfoMapping(6, 2, CobolType = CobolType.Unsigned)]
        public short PR_OPSE { get; set; }

        /// <summary>
        /// PR_OPZO 9(2)  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short PR_OPZO { get; set; }

        /// <summary>
        /// PR_LAVO X(3)  
        /// </summary>
        [HisFieldInfoMapping(8, 3)]
        public string PR_LAVO { get; set; }

        /// <summary>
        /// PR_ESITO X(3)  
        /// </summary>
        [HisFieldInfoMapping(9, 3)]
        public string PR_ESITO { get; set; }
        #endregion Tracciato Host

        #endregion Properties
    }
}
