using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostRequest
{
    public class FSPL_FSRCRequest
    {
        #region Constructor
        internal FSPL_FSRCRequest()
		{

		}
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //    03 FILLER         PIC X(11) VALUE "   DSPYAAAA".
        // 02 AREA-CONTROLLO.
        //    03 AR-TIPO        PIC X(3).
        //    03 AR-SUBT        PIC X.
        //    03 AR-FASE        PIC X.
        //    03 AR-LNGR        PIC 9(5) COMP-3.
        //    03 AR-ACCO        PIC X OCCURS 60.
        //    03 AR-DATA        PIC 9(8) VALUE 01012004.
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// FILLER X(11)  
        /// </summary>
        [HisFieldInfoMapping(0, 11)]
        public string FILLER { get; set; }

        // 02 AREA-CONTROLLO.
        /// <summary>
        /// AR_TIPO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string AR_TIPO { get; set; }

        /// <summary>
        /// AR_SUBT X  
        /// </summary>
        [HisFieldInfoMapping(2, 1)]
        public string AR_SUBT { get; set; }

        /// <summary>
        /// AR_FASE X  
        /// </summary>
        [HisFieldInfoMapping(3, 1)]
        public string AR_FASE { get; set; }

        /// <summary>
        /// AR_LNGR 9(5) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(4, 3, CobolType = CobolType.Comp3Unsigned)]
        public int AR_LNGR { get; set; }

        /// <summary>
        /// AR_ACCO X  OCCURS 60
        /// </summary>
        [HisComplexAreaInfoMapping(5, ListCount = 60)]
        public List<BLOCCO> LISTBLOCCO { get; set; }

        /// <summary>
        /// AR_DATA 9(8)  
        /// </summary>
        [HisFieldInfoMapping(6, 8, CobolType = CobolType.Unsigned)]
        public int AR_DATA { get; set; }

        [HisFieldInfoMapping(7, 32916, CobolType = CobolType.Untraslate)]
        public byte[] DATI_INPUT { get; set; }
        #endregion Tracciato Host

        #endregion Properties

        #region nested class
        public class BLOCCO
        {
            #region Constructor
            public BLOCCO()
            {

            }
            public BLOCCO(string blocco)
            {
                this.AR_ACCO = blocco;
            }
            #endregion Constructor
            #region Properties

            #region Tracciato COBOL
            //    03 AR-ACCO        PIC X OCCURS 60.
            #endregion Tracciato COBOL

            #region Tracciato Host
            [HisFieldInfoMapping(0, 1)]
            public string AR_ACCO { get; set; }
            #endregion Tracciato Host
            #endregion Properties
        }
        #endregion nested class
    }
}
