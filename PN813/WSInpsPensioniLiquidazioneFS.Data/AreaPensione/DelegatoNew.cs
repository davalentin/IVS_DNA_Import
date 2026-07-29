using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA
{
    public class DelegatoNew : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  TRB-DELEG.       
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  TRB-DELEG.
        /// <summary>
        /// TRBTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string TRBTIPOR { get; set; }

        /// <summary>
        /// TRBIBAN X(34)  
        /// </summary>
        [HisFieldInfoMapping(1, 34)]
        public string TRBIBAN { get; set; }

        /// <summary>
        /// TRBBIC X(11)  
        /// </summary>
        [HisFieldInfoMapping(2, 11)]
        public string TRBBIC { get; set; }

        /// <summary>
        /// TRBPAESE X(1)  
        /// </summary>
        [HisFieldInfoMapping(3, 1)]
        public string TRBPAESE { get; set; }

        /// <summary>
        /// TRBASTERISCHI X(2)  
        /// </summary>
        [HisFieldInfoMapping(4, 2)]
        public string TRBASTERISCHI { get; set; }

        // <summary>
        /// TRBINPDAP X(2)  
        /// </summary>
        [HisFieldInfoMapping(5, 2)]
        public string TRBINPDAP { get; set; }

        /// <summary>
        /// TRBMESEINPDAP 9(2)  
        /// </summary>
        [HisFieldInfoMapping(6, 2, CobolType = CobolType.Unsigned)]
        public short TRBMESEINPDAP { get; set; }

        /// <summary>
        /// TRBANNOINPDAP 9(4)  
        /// </summary>
        [HisFieldInfoMapping(7, 4, CobolType = CobolType.Unsigned)]
        public short TRBANNOINPDAP { get; set; }

        /// <summary>
        /// TRBTIPOENTEPAT X(3)  
        /// </summary>
        [HisFieldInfoMapping(8, 3)]
        public string TRBTIPOENTEPAT { get; set; }

        /// <summary>
        /// TRBTIPOUFFPAT X(3)  
        /// </summary>
        [HisFieldInfoMapping(9, 3)]
        public string TRBTIPOUFFPAT { get; set; }
        
        /// <summary>
        /// TRBOLDEAD X(8)  
        /// </summary>
        [HisFieldInfoMapping(10, 8)]
        public string TRBOLDEAD { get; set; }

        /// <summary>
        /// TRBUFFZONALE X(10)  
        /// </summary>
        [HisFieldInfoMapping(11, 10)]
        public string TRBUFFZONALE { get; set; }

        /// <summary>
        /// TRBNUMPRATICA X(8)  
        /// </summary>
        [HisFieldInfoMapping(12, 8)]
        public string TRBNUMPRATICA { get; set; }

        /// <summary>
        /// TRBSENTI X(1)  
        /// </summary>
        [HisFieldInfoMapping(13, 1)]
        public string TRBSENTI { get; set; }

        /// <summary>
        /// TRBTELEM X(1)  
        /// </summary>
        [HisFieldInfoMapping(14, 1)]
        public string TRBTELEM { get; set; }

        /// <summary>
        /// TRBPROCESSO X(3)  
        /// </summary>
        [HisFieldInfoMapping(15, 3)]
        public string TRBPROCESSO { get; set; }

        /// <summary>
        /// TRBFILLER X(3)  
        /// </summary>
        [HisFieldInfoMapping(16, 3)]
        public string TRBFILLER { get; set; }

        /// <summary>
        /// TRBFASE X(3)  
        /// </summary>
        [HisFieldInfoMapping(17, 3)]
        public string TRBFASE { get; set; }

        /// <summary>
        /// TRBUNICARPE X  
        /// </summary>
        [HisFieldInfoMapping(18, 1)]
        public string TRBUNICARPE { get; set; }

        /// <summary>
        /// TRBONERI1 9(4)V99 COMP-3 
        /// </summary>
        [HisFieldInfoMapping(19, 4, Scale = 2, CobolType = CobolType.Comp3Unsigned)]
        public decimal TRBONERI1 { get; set; }

        /// <summary>
        /// TRBONERI2 9(4)V99 COMP-3 
        /// </summary>
        [HisFieldInfoMapping(20, 4, Scale = 2, CobolType = CobolType.Comp3Unsigned)]
        public decimal TRBONERI2 { get; set; }

        /// <summary>
        /// TRBONERI3 9(4)V99 COMP-3 
        /// </summary>
        [HisFieldInfoMapping(21, 4, Scale = 2, CobolType = CobolType.Comp3Unsigned)]
        public decimal TRBONERI3 { get; set; }

        /// <summary>
        /// TRBBONUS X(2)  
        /// </summary>
        [HisFieldInfoMapping(22, 2)]
        public string TRBBONUS { get; set; }

        /// <summary>
        /// TRBMESEDALBONUS 9(2)  
        /// </summary>
        [HisFieldInfoMapping(23, 2, CobolType = CobolType.Unsigned)]
        public short TRBMESEDALBONUS { get; set; }

        /// <summary>
        /// TRBANNODALBONUS 9(4)  
        /// </summary>
        [HisFieldInfoMapping(24, 4, CobolType = CobolType.Unsigned)]
        public short TRBANNODALBONUS { get; set; }

        /// <summary>
        /// TRBMESEALBONUS 9(2)  
        /// </summary>
        [HisFieldInfoMapping(25, 2, CobolType = CobolType.Unsigned)]
        public short TRBMESEALBONUS { get; set; }

        /// <summary>
        /// TRBANNOALBONUS 9(4)  
        /// </summary>
        [HisFieldInfoMapping(26, 4, CobolType = CobolType.Unsigned)]
        public short TRBANNOALBONUS { get; set; }

        /// <summary>
        /// TRBCOABI 9(5) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(27, 3, CobolType = CobolType.Comp3Unsigned)]
        public int TRBCOABI { get; set; }

        /// <summary>
        /// TRBCOCAB 9(7) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(28, 4, CobolType = CobolType.Comp3Unsigned)]
        public int TRBCOCAB { get; set; }

        /// <summary>
        /// TRBCOCON 9(12) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(29, 7, CobolType = CobolType.Comp3Unsigned)]
        public long TRBCOCON { get; set; }

        /// <summary>
        /// TRBCOVAL X  
        /// </summary>
        [HisFieldInfoMapping(30, 1)]
        public string TRBCOVAL { get; set; }

        /// <summary>
        /// TRBCOPAG X  
        /// </summary>
        [HisFieldInfoMapping(31, 1)]
        public string TRBCOPAG { get; set; }

        /// <summary>
        /// TRBLG140 9(6)  
        /// </summary>
        [HisFieldInfoMapping(32, 6, CobolType = CobolType.Unsigned)]
        public int TRBLG140 { get; set; }

        /// <summary>
        /// TRBSEN140 9  
        /// </summary>
        [HisFieldInfoMapping(33, 1, CobolType = CobolType.Unsigned)]
        public short TRBSEN140 { get; set; }


        #endregion Tracciato Host

        public string TransactionName
        {
            get { return "Delegato"; }
        }

        #endregion Properties
    }
}
