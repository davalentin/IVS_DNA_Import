using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaCodiciStampa
    {
        #region tracciato COBOL
        //  04  CODICI-STAMPA.
        //         10  CI281.
        //             15  CI281A                    PIC X.
        //             15  CI281B                    PIC X.
        //         10  CI282.
        //             15  CI282A                    PIC X.
        //             15  CI282B                    PIC X.
        //         10  CI325.
        //             15  CI351                     PIC X.
        //             15  CI352                     PIC X.
        //         10  E211                          PIC X.
        //             15 CI28DALA                  PIC 9999.
        //             15 CI28DALMM                 PIC 99.
        //             15 CI28ALA                   PIC 9999.
        //             15 CI28ALMM                  PIC 99.
        //* VALIDI SOLO PER PC
        //         10  CI21                          PIC X.
        #endregion tracciato COBOL

        #region Tracciato Host
        // 04  CODICI-STAMPA.
        // 10  CI281.
        /// <summary>
        /// CI281A X 
        /// CI281B X
        /// </summary>
        [HisFieldInfoMapping(0, 2)]
        public string CI281 { get; set; }

        // 10  CI282.
        /// <summary>
        /// CI282A X 
        /// CI282B X
        /// </summary>
        [HisFieldInfoMapping(1, 2)]
        public string CI282 { get; set; }

        // 10  CI325.
        /// <summary>
        /// CI351 X  
        /// </summary>
        [HisFieldInfoMapping(2, 1)]
        public string CI351 { get; set; }

        /// <summary>
        /// CI352 X  
        /// </summary>
        [HisFieldInfoMapping(3, 1)]
        public string CI352 { get; set; }

        /// <summary>
        /// E211 X  
        /// </summary>
        [HisFieldInfoMapping(4, 1)]
        public string E211 { get; set; }

        /// <summary>
        /// CI28DALA 9999  
        /// </summary>
        [HisFieldInfoMapping(5, 4)]
        public short CI28DALA { get; set; }

        /// <summary>
        /// CI28DALMM 99  
        /// </summary>
        [HisFieldInfoMapping(6, 2)]
        public short CI28DALMM { get; set; }

        /// <summary>
        /// CI28ALA 9999  
        /// </summary>
        [HisFieldInfoMapping(7, 4)]
        public short CI28ALA { get; set; }

        /// <summary>
        /// CI28ALMM 99  
        /// </summary>
        [HisFieldInfoMapping(8, 2)]
        public short CI28ALMM { get; set; }

        /// <summary>
        /// CI21 X  
        /// </summary>
        [HisFieldInfoMapping(9, 1)]
        public string CI21 { get; set; }


        // * VALIDI SOLO PER PC

        #endregion Tracciato Host
    }
}
