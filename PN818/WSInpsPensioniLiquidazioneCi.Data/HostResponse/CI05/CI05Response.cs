using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

using INPS.Pensioni.LiquidazioneCi.Data.HostRequest;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class CI05Response
	{
		#region Constructor
        public CI05Response()
		{
            this.Dati = new AreaDati();
            this.DatiDecompressi = new CI05AreaDecompressa();
		}
        #endregion Constructor

        #region Properties
        [HisComplexAreaInfoMapping(0)]
		public AreaDati Dati { get; set; }

        public CI05AreaDecompressa DatiDecompressi { get; set; }
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
                this.AREA_CONTROLLO = new CI05RequestBase.AreaControllo();
			}
            #endregion Constructor

            #region tracciato COBOL
            //01   DATI-DA-HOST.
            //         02   CODICE-OK                             PIC X(2).
            //         02   RET-CODE                              PIC X(4).
            //         02   DEC-CODE                              PIC X(46).
            //         02   DEC-CODE-2                            PIC X(75).
            //         02   DATI-RISPOSTA.
            //               05   DATI-RISP1                    PIC X(251).
            //               05   DATI-RISPN    OCCURS  9 TIMES PIC X(256).
            //               05   FILLER                        PIC X(5).
            //         02   FILLER REDEFINES DATI-RISPOSTA.
            //               05   FILLER.
            //                 10   FILLER                PIC X(22).
            //                 10   RISP-RC               PIC 9(2).
            //                 10   FILLER                PIC X(103).
            //               05   RISP-LNG                PIC X(4).
            //               05   RISP-COMPR              PIC X(2429).
            #endregion tracciato COBOL

            #region Tracciato Host

            //AREA-CONTROLLO
            [HisComplexAreaInfoMapping(0)]
            public  CI05RequestBase.AreaControllo AREA_CONTROLLO { get; set; }

            //// 01   DATI-DA-HOST.
            ///// <summary>
            ///// CODICE_OK X(2)  
            ///// </summary>
            //[HisFieldInfoMapping(1, 2)]
            //public string CODICE_OK { get; set; }

            ///// <summary>
            ///// RET_CODE X(4)  
            ///// </summary>
            //[HisFieldInfoMapping(2, 4)]
            //public string RET_CODE { get; set; }

            ///// <summary>
            ///// DEC_CODE X(46)  
            ///// </summary>
            //[HisFieldInfoMapping(3, 46)]
            //public string DEC_CODE { get; set; }

            ///// <summary>
            ///// DEC_CODE_2 X(75)  
            ///// </summary>
            //[HisFieldInfoMapping(4, 75)]
            //public string DEC_CODE_2 { get; set; }

            /// <summary>
            /// FILLER X(127)  
            /// </summary>
            [HisFieldInfoMapping(1, 22)]
            public string FILLER1 { get; set; }

            /// <summary>
            /// RISP_NUM X(4)  
            /// </summary>
            [HisFieldInfoMapping(2, 2)]
            public short RISP_RC { get; set; }

            /// <summary>
            /// FILLER X(103)  
            /// </summary>
            [HisFieldInfoMapping(3, 103)]
            public string FILLER2 { get; set; }

            /// <summary>
            /// RISP_LNG X(4)  
            /// </summary>
            [HisFieldInfoMapping(4, 4)]
            public string RISP_LNG { get; set; }

            /// <summary>
            /// RISP_COMPR X(2429)  
            /// </summary>
            [HisFieldInfoMapping(5, 2429, CobolType = CobolType.Untraslate)]
            public byte[] RISP_COMPR { get; set; }
            #endregion Tracciato Host
        }


        #endregion Nested class
    }
}
