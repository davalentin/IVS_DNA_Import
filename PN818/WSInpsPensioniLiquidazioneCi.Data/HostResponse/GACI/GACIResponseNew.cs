using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

using INPS.Pensioni.LiquidazioneCi.Data.HostRequest;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class GACIResponseNew
    {
        #region Constructor
        public GACIResponseNew()
		{
            this.Dati = new AreaDati();
            this.DatiDecompressi = new GACIAreaDecompressa();
		}
        #endregion Constructor

        #region Properties
        [HisComplexAreaInfoMapping(0)]
		public AreaDati Dati { get; set; }

        public GACIAreaDecompressa DatiDecompressi { get; set; }
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
            //03  REC-RISP-1 REDEFINES REC-RISP.
            //05  FILLER                PIC X(8).
            //05  W-RISP-1.
            // 10  RIS-CAT-ALF       PIC X(8).                         
            // 10  RIS-SEDE          PIC X(4).                         
            // 10  RIS-CERT          PIC X(8).                         
            // 10  FILLER            PIC X(223).                       
            //05  FILLER                PIC X(30949).
            #endregion tracciato COBOL

            #region Tracciato Host
            //// 03  REC-RISP-1 REDEFINES REC-RISP.
            ///// <summary>
            ///// FILLER X(8)  
            ///// </summary>
            [HisFieldInfoMapping(0, 8)]
            public string FILLER1 { get; set; }

            //// 05  W-RISP-1.
            ///// <summary>
            ///// RIS_CAT_ALF X(8)  
            ///// </summary>
            //[HisFieldInfoMapping(1, 8)]
            //public string RIS_CAT_ALF { get; set; }

            ///// <summary>
            ///// RIS_SEDE X(4)  
            ///// </summary>
            //[HisFieldInfoMapping(2, 4)]
            //public string RIS_SEDE { get; set; }

            ///// <summary>
            ///// RIS_CERT X(8)  
            ///// </summary>
            //[HisFieldInfoMapping(3, 8)]
            //public string RIS_CERT { get; set; }

            ///// <summary>
            ///// FILLER X(223)  
            ///// </summary>
            //[HisFieldInfoMapping(4, 223)]
            //public string FILLER2 { get; set; }

            /// <summary>
            /// FILLER X(30949)  
            /// </summary>
            [HisFieldInfoMapping(1, 31192, CobolType= CobolType.Untraslate)]
            public byte[] RISP_COMPR { get; set; }

            #endregion Tracciato Host

            #region Properties
            public bool PresenzaPensione { get; set; }
            #endregion Properties
        }

        #endregion Nested class
    }
}
