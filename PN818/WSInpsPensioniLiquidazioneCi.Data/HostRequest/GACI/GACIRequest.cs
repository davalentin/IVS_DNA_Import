using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostRequest
{
    public class GACIRequest
    {
        #region Properties

        #region Tracciato COBOL
        //01  W-AREA-INV.
        //           03  REC-TRAS.
        //              05  FILLER PIC X(3).
        //              05  W-DATI-X.
        //                  07 W-DATI-DSO         PIC X(8).
        //                  07 W-DATI-CAT         PIC X(3).
        //                  07 W-DATI-SEDE        PIC X(4).
        //                  07 W-DATI-CERT        PIC X(8).
        //                  07 W-DATI-AF          PIC X(2).
        //                  07 W-DATI-AS          PIC X(2).
        //                  07 W-DATI             PIC X(300).
        #endregion Tracciato COBOL

        #region Tracciato Host
        //// 01  W-AREA-INV.
        //// 03  REC-TRAS.
        ///// <summary>
        ///// FILLER PIC X(3).  
        ///// </summary>
        [HisFieldInfoMapping(0, 3)]
        public short FILLER { get; set; }


        // 05  W-DATI-X.
        /// <summary>
        /// W_DATI_DSO X(8)  
        /// </summary>
        [HisFieldInfoMapping(1, 8)]
        public string W_DATI_DSO { get; set; }

        /// <summary>
        /// W_DATI_CAT X(3)  
        /// </summary>
        [HisFieldInfoMapping(2, 3)]
        public string W_DATI_CAT { get; set; }

        /// <summary>
        /// W_DATI_SEDE X(4)  
        /// </summary>
        [HisFieldInfoMapping(3, 4)]
        public string W_DATI_SEDE { get; set; }

        /// <summary>
        /// W_DATI_CERT X(8)  
        /// </summary>
        [HisFieldInfoMapping(4, 8)]
        public string W_DATI_CERT { get; set; }

        /// <summary>
        /// W_DATI_AF X(2)  
        /// </summary>
        [HisFieldInfoMapping(5, 2)]
        public string W_DATI_AF { get; set; }

        /// <summary>
        /// W_DATI_AS X(2)  
        /// </summary>
        [HisFieldInfoMapping(6, 2)]
        public string W_DATI_AS { get; set; }

        /// <summary>
        /// W_DATI X(300)  
        /// </summary>
        [HisFieldInfoMapping(7, 14)]
        public string W_DATI { get; set; }

        /// <summary>
        /// ANNO_COMPETENZA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(8, 4)]
        public int ANNO_COMPETENZA { get; set; }

        [HisFieldInfoMapping(9, 218)]
        public short FILLER1 { get; set; }
        #endregion Tracciato Host

        #endregion Properties

    }
}
