using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostResponse.AreaStampa
{
    public class Pagamento
    {
        #region Constructor
        internal Pagamento()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 COD-STATO        PIC X(02).
        //*       GP1CCST              CODICE STATO                   1700
        //     02 MOD-PAG          PIC X(02).
        //*       GP1CTIPPAG           MODALITA' PAGAMENTO            1702
        //     02 CONTROCOD        PIC X(02).
        //*       GP1CCIN              CONTROCODICE (CIN)             1704
        //     02 UFF-PAG          PIC X(03).
        //*       DF28                 CODICE UFF.PAGATORE            1706
        //     02 ABI              PIC 9(05).
        //*       GP1CABI              CODICE ABI                     1709
        //     02 CAB              PIC 9(07).
        //*       GP1CCAB              CODICE CAB                     1714
        //     02 CONTO-CORR       PIC X(12).
        //*       GP1CNCC              CONTO CORRENTE                 1721
        //     02 AA-INDE          PIC 9(02).
        //*                            ANNO ELABORAZIONE              1733
        //     02 MM-INDE          PIC 9(02).
        //*                            MESE ELABORAZIONE              1735
        //     02 INDEBITO         PIC 9(08)V9(04).
        //*                          IMPORTO TOTALE INDEBITO          1737
        //     02 FILLER           PIC X(01).
        //*                            LIBERI                         1749
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// COD_STATO X(02)  
        /// </summary>
        [HisFieldInfoMapping(0, 2)]
        public string COD_STATO { get; set; }

        // *       GP1CCST              CODICE STATO                   1700
        /// <summary>
        /// MOD_PAG X(02)  
        /// </summary>
        [HisFieldInfoMapping(1, 2)]
        public string MOD_PAG { get; set; }

        // *       GP1CTIPPAG           MODALITA' PAGAMENTO            1702
        /// <summary>
        /// CONTROCOD X(02)  
        /// </summary>
        [HisFieldInfoMapping(2, 2)]
        public string CONTROCOD { get; set; }

        // *       GP1CCIN              CONTROCODICE (CIN)             1704
        /// <summary>
        /// UFF_PAG X(03)  
        /// </summary>
        [HisFieldInfoMapping(3, 3)]
        public string UFF_PAG { get; set; }

        // *       DF28                 CODICE UFF.PAGATORE            1706
        /// <summary>
        /// ABI 9(05)  
        /// </summary>
        [HisFieldInfoMapping(4, 5, CobolType = CobolType.Unsigned)]
        public int ABI { get; set; }

        // *       GP1CABI              CODICE ABI                     1709
        /// <summary>
        /// CAB 9(07)  
        /// </summary>
        [HisFieldInfoMapping(5, 7, CobolType = CobolType.Unsigned)]
        public int CAB { get; set; }

        // *       GP1CCAB              CODICE CAB                     1714
        /// <summary>
        /// CONTO_CORR X(12)  
        /// </summary>
        [HisFieldInfoMapping(6, 12)]
        public string CONTO_CORR { get; set; }

        // *       GP1CNCC              CONTO CORRENTE                 1721
        /// <summary>
        /// AA_INDE 9(02)  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short AA_INDE { get; set; }

        // *                            ANNO ELABORAZIONE              1733
        /// <summary>
        /// MM_INDE 9(02)  
        /// </summary>
        [HisFieldInfoMapping(8, 2, CobolType = CobolType.Unsigned)]
        public short MM_INDE { get; set; }

        // *                            MESE ELABORAZIONE              1735
        /// <summary>
        /// INDEBITO 9(08)V9(04)  
        /// </summary>
        [HisFieldInfoMapping(9, 12, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal INDEBITO { get; set; }

        // *                          IMPORTO TOTALE INDEBITO          1737
        /// <summary>
        /// FILLER X(01)  
        /// </summary>
        [HisFieldInfoMapping(10, 1)]
        public string FILLER { get; set; }

        // *                            LIBERI                         1749
        #endregion Tracciato Host

        #region nested class

        #endregion nested class

        #endregion Properties
    }
}

