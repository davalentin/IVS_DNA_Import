using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA
{
    public class DanteCausa : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //      01  TRD-ASSIC.
        //          02 TRDTIPOR            PIC X VALUE "D".
        //          02 TRDCONOM            PIC X(32).
        //          02 TRDCOACQ            PIC X(16).
        //          02 TRDSESSO            PIC X.
        //          02 TRDAANAS            PIC 9(4).
        //          02 TRDMMNAS            PIC 99.
        //          02 TRDGGNAS            PIC 99.
        //          02 TRDCONAS            PIC 9(5).
        //          02 TRDPRNAS            PIC 99.
        //          02 TRDCOFIS            PIC X(16).
        //          02 TRDCFSIT            PIC X.
        //D2000     02 TRDMORTE.
        //D2NEW        03 TRDMORAA         PIC 9999.                              
        //             03 TRDMORMM         PIC 99.
        //             03 TRDMORGG         PIC 99.
        //          02 TRDCATEG            PIC X(6).
        //          02 TRDCERTI            PIC 9(8).
        //          02 TRDCARIC            PIC 9(4).
        //          02 TRDCODEL            PIC 9.
        //D2000     02 TRDDECDE.
        //D2NEW        03 TRDCDEAA         PIC 9999.                              
        //             03 TRDCDEMM         PIC 99.
        //D2000     02 TRDDECCN.
        //D2NEW        03 TRDCNNAA         PIC 9999.                              
        //             03 TRDCNNMM         PIC 99.
        //GD1111    02 TRDDTMATR           PIC 9(8).
        //GD1111    02 TRDDISPO            PIC X(21).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  TRD-ASSIC.
        /// <summary>
        /// TRDTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string TRDTIPOR { get; set; }

        /// <summary>
        /// TRDCONOM X(32)  
        /// </summary>
        [HisFieldInfoMapping(1, 32)]
        public string TRDCONOM { get; set; }

        /// <summary>
        /// TRDCOACQ X(16)  
        /// </summary>
        [HisFieldInfoMapping(2, 16)]
        public string TRDCOACQ { get; set; }

        /// <summary>
        /// TRDSESSO X  
        /// </summary>
        [HisFieldInfoMapping(3, 1)]
        public string TRDSESSO { get; set; }

        /// <summary>
        /// TRDAANAS 9(4)  
        /// </summary>
        [HisFieldInfoMapping(4, 4, CobolType = CobolType.Unsigned)]
        public short TRDAANAS { get; set; }

        /// <summary>
        /// TRDMMNAS 99  
        /// </summary>
        [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
        public short TRDMMNAS { get; set; }

        /// <summary>
        /// TRDGGNAS 99  
        /// </summary>
        [HisFieldInfoMapping(6, 2, CobolType = CobolType.Unsigned)]
        public short TRDGGNAS { get; set; }

        /// <summary>
        /// TRDCONAS 9(5)  
        /// </summary>
        [HisFieldInfoMapping(7, 5, CobolType = CobolType.Unsigned)]
        public int TRDCONAS { get; set; }

        /// <summary>
        /// TRDPRNAS 99  
        /// </summary>
        [HisFieldInfoMapping(8, 2, CobolType = CobolType.Unsigned)]
        public short TRDPRNAS { get; set; }

        /// <summary>
        /// TRDCOFIS X(16)  
        /// </summary>
        [HisFieldInfoMapping(9, 16)]
        public string TRDCOFIS { get; set; }

        /// <summary>
        /// TRDCFSIT X  
        /// </summary>
        [HisFieldInfoMapping(10, 1)]
        public string TRDCFSIT { get; set; }

        // D2000     02 TRDMORTE.
        /// <summary>
        /// TRDMORAA 9999  
        /// </summary>
        [HisFieldInfoMapping(11, 4, CobolType = CobolType.Unsigned)]
        public short TRDMORAA { get; set; }

        /// <summary>
        /// TRDMORMM 99  
        /// </summary>
        [HisFieldInfoMapping(12, 2, CobolType = CobolType.Unsigned)]
        public short TRDMORMM { get; set; }

        /// <summary>
        /// TRDMORGG 99  
        /// </summary>
        [HisFieldInfoMapping(13, 2, CobolType = CobolType.Unsigned)]
        public short TRDMORGG { get; set; }

        /// <summary>
        /// TRDCATEG X(6)  
        /// </summary>
        [HisFieldInfoMapping(14, 6)]
        public string TRDCATEG { get; set; }

        /// <summary>
        /// TRDCERTI 9(8)  
        /// </summary>
        [HisFieldInfoMapping(15, 8, CobolType = CobolType.Unsigned)]
        public int TRDCERTI { get; set; }

        /// <summary>
        /// TRDCARIC 9(4)  
        /// </summary>
        [HisFieldInfoMapping(16, 4, CobolType = CobolType.Unsigned)]
        public short TRDCARIC { get; set; }

        /// <summary>
        /// TRDCODEL 9  
        /// </summary>
        [HisFieldInfoMapping(17, 1, CobolType = CobolType.Unsigned)]
        public short TRDCODEL { get; set; }

        // D2000     02 TRDDECDE.
        /// <summary>
        /// TRDCDEAA 9999  
        /// </summary>
        [HisFieldInfoMapping(18, 4, CobolType = CobolType.Unsigned)]
        public short TRDCDEAA { get; set; }

        /// <summary>
        /// TRDCDEMM 99  
        /// </summary>
        [HisFieldInfoMapping(19, 2, CobolType = CobolType.Unsigned)]
        public short TRDCDEMM { get; set; }

        // D2000     02 TRDDECCN.
        /// <summary>
        /// TRDCNNAA 9999  
        /// </summary>
        [HisFieldInfoMapping(20, 4, CobolType = CobolType.Unsigned)]
        public short TRDCNNAA { get; set; }

        /// <summary>
        /// TRDCNNMM 99  
        /// </summary>
        [HisFieldInfoMapping(21, 2, CobolType = CobolType.Unsigned)]
        public short TRDCNNMM { get; set; }

        /// <summary>
        /// TRDDTMATR 9(8)
        /// </summary>
        [HisFieldInfoMapping(22, 8, CobolType = CobolType.Unsigned)]
        public int TRDDTMATR { get; set; }

        /// <summary>
        /// TRDDISPO X(21)
        /// </summary>
        [HisFieldInfoMapping(23, 21)]
        public string TRDDISPO { get; set; }
        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "DanteCausa"; }
        }
        #endregion Properties
    }
}
