using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostResponse
{
    public class FSPRAreaCompressa : ITransactionInfo
    {
        #region Constructor
        internal FSPRAreaCompressa()
        {

        }
        #endregion Constructor

        #region Properties


        #region Tracciato COBOL
        //02 AREA-CONTROLLO-DATI.
        //03 FILLER         PIC X(2).
        //03 RR-TIPO        PIC X(3).
        //03 RR-SUBT        PIC X.
        //03 RR-FASE        PIC X.
        //03 RR-LNGR        PIC 9(5) COMP-3.
        //03 RR-ACCO        PIC X OCCURS 60.
        //03 RR-DATA        PIC 9(8).
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// FILLER X(2)  
        /// </summary>
        [HisFieldInfoMapping(0, 2)]
        public string FILLER { get; set; }

        /// <summary>
        /// RR_TIPO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string RR_TIPO { get; set; }

        /// <summary>
        /// RR_SUBT X  
        /// </summary>
        [HisFieldInfoMapping(2, 1)]
        public string RR_SUBT { get; set; }

        /// <summary>
        /// RR_FASE X  
        /// </summary>
        [HisFieldInfoMapping(3, 1)]
        public string RR_FASE { get; set; }

        /// <summary>
        /// RR_LNGR 9(5) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(4, 3, CobolType = CobolType.Comp3Unsigned)]
        public int RR_LNGR { get; set; }

        //03 RR-ACCO        PIC X OCCURS 60.
        [HisComplexAreaInfoMapping(5, ListCount = 60)]
        public List<RR_ACCO> LISTARR_ACCO { get; set; }

        /// <summary>
        /// RR_DATA 9(8)  
        /// </summary>
        [HisFieldInfoMapping(6, 8, CobolType = CobolType.Unsigned)]
        public int RR_DATA { get; set; }

        [HisFieldInfoMapping(7, 19886, CobolType = CobolType.Untraslate)]
        public byte[] RISP_COMPR { get; set; }
        #endregion Tracciato Host

        public string TransactionName
        {
            get { return "Area compressa tradotta"; }
        }

        #endregion Properties

        #region nested class
        public class RR_ACCO
        {
            #region Constructor
            internal RR_ACCO()
            {
            }
            #endregion Constructor

            #region tracciato COBOL
            //03 RR-ACCO        PIC X OCCURS 60.
            #endregion tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// RR_ACCO X  
            /// </summary>
            [HisFieldInfoMapping(0, 1)]
            public string TIPO_BLOCCO { get; set; }
            #endregion Tracciato Host
        }
        #endregion nested class

    }
}
