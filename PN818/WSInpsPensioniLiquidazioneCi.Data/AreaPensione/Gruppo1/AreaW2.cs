using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaW2
    {
        #region tracciato COBOL
        //     04  AREAW2.
        //     05  IABTIPEN     PIC X.
        //*+TIPO PENSIONE (1,2,3,4,5,6)
        //     05  IABCONA1     PIC X.
        //     05  IABCONA2     PIC X.
        //*+CODICE NATURA PENSIONE
        //     05  IABCONA3     PIC X.
        //*+PRIMA LETTERA NAT.PEN.
        //     05  IABCONA4     PIC X.
        //     05  IABCOSIND    PIC XX.
        //*+CODICE SINDACATO
        #endregion tracciato COBOL

        #region Tracciato Host
        // 04  AREAW2.
        /// <summary>
        /// IABTIPEN X  
        /// *+TIPO PENSIONE (1,2,3,4,5,6)
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string IABTIPEN { get; set; }

        /// <summary>
        /// IABCONA1 X  
        /// </summary>
        [HisFieldInfoMapping(1, 1)]
        public string IABCONA1 { get; set; }

        /// <summary>
        /// IABCONA2 X  
        /// *+CODICE NATURA PENSIONE
        /// </summary>
        [HisFieldInfoMapping(2, 1)]
        public string IABCONA2 { get; set; }

        /// <summary>
        /// IABCONA3 X  
        /// *+PRIMA LETTERA NAT.PEN.
        /// </summary>
        [HisFieldInfoMapping(3, 1)]
        public string IABCONA3 { get; set; }

        /// <summary>
        /// IABCONA4 X  
        /// </summary>
        [HisFieldInfoMapping(4, 1)]
        public string IABCONA4 { get; set; }

        /// <summary>
        /// IABCOSIND XX 
        /// *+CODICE SINDACATO 
        /// </summary>
        [HisFieldInfoMapping(5, 2)]
        public string IABCOSIND { get; set; }


        #endregion Tracciato Host
    }
}
