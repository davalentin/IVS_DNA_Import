using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostResponse.AreaStampa
{
    public class Patronato_Sindacato
    {
        #region Constructor
        internal Patronato_Sindacato()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 AA-PATRO         PIC 9(02).
        //*                            ANNO ELABORAZIONE              1750
        //     02 MM-PATRO         PIC 9(02).
        //*                            MESE ELABORAZIONE              1752
        //     02 COD-PATRO        PIC 9(02).
        //*                            CODICE PATRONATO               1754
        //     02 CODZO-PATRO      PIC X(01).
        //*                            CODICE ZONA                    1756
        //     02 PATRO            PIC X(10).
        //*                            PATRONATO IN CHIARO            1757
        //     02 NUM-PRAT-PATRO   PIC 9(07).
        //*                            NUM.PRATICA PATR.              1767
        //     02 COD-SIND         PIC X(02).
        //*                            CODICE SINDACATO               1774
        //     02 COD-TELE         PIC X(02).
        //*                            CODICE PATRONATO TELEMATICO    1776
        //     02 FILLER           PIC X(12).
        //*                            LIBERI                         1778
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// AA_PATRO 9(02)  
        /// </summary>
        [HisFieldInfoMapping(0, 2, CobolType = CobolType.Unsigned)]
        public short AA_PATRO { get; set; }

        // *                            ANNO ELABORAZIONE              1750
        /// <summary>
        /// MM_PATRO 9(02)  
        /// </summary>
        [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
        public short MM_PATRO { get; set; }

        // *                            MESE ELABORAZIONE              1752
        /// <summary>
        /// COD_PATRO 9(02)  
        /// </summary>
        [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
        public short COD_PATRO { get; set; }

        // *                            CODICE PATRONATO               1754
        /// <summary>
        /// CODZO_PATRO X(01)  
        /// </summary>
        [HisFieldInfoMapping(3, 1)]
        public string CODZO_PATRO { get; set; }

        // *                            CODICE ZONA                    1756
        /// <summary>
        /// PATRO X(10)  
        /// </summary>
        [HisFieldInfoMapping(4, 10)]
        public string PATRO { get; set; }

        // *                            PATRONATO IN CHIARO            1757
        /// <summary>
        /// NUM_PRAT_PATRO 9(07)  
        /// </summary>
        [HisFieldInfoMapping(5, 7, CobolType = CobolType.Unsigned)]
        public int NUM_PRAT_PATRO { get; set; }

        // *                            NUM.PRATICA PATR.              1767
        /// <summary>
        /// COD_SIND X(02)  
        /// </summary>
        [HisFieldInfoMapping(6, 2)]
        public string COD_SIND { get; set; }

        // *                            CODICE SINDACATO               1774
        /// <summary>
        /// COD_TELE X(02)  
        /// </summary>
        [HisFieldInfoMapping(7, 2)]
        public string COD_TELE { get; set; }

        // *                            CODICE PATRONATO TELEMATICO    1776
        /// <summary>
        /// FILLER X(12)  
        /// </summary>
        [HisFieldInfoMapping(8, 12)]
        public string FILLER { get; set; }

        // *                            LIBERI                         1778
        #endregion Tracciato Host

        #region nested class

        #endregion nested class

        #endregion Properties
    }
}

