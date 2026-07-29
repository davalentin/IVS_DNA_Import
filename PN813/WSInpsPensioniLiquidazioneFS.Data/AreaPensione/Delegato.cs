using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA
{
    public class Delegato: ITransactionInfo
    {
        #region Constructor
        internal Delegato()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //01  TRB-DELEG.
        //          02 TRBTIPOR            PIC X VALUE "B".
        //          02 TRBCODIC            PIC X.
        //          02 TRBCONOM            PIC X(32).
        //          02 TRBDTNAS.
        //             03 TRBAANAS         PIC 9(4).
        //             03 TRBMMNAS         PIC 99.
        //             03 TRBGGNAS         PIC 99.
        //          02 TRBCOFIS            PIC X(16).
        //          02 TRBCFSIT            PIC X.
        //          02 TRBINDIR            PIC X(32).
        //          02 TRBCORES            PIC X(22).
        //          02 TRBPRRES            PIC XX.
        //          02 TRBCAPPP            PIC 9(5).
        //          02 TRBCONAS            PIC 9(5).
        //          02 TRBPRNAS            PIC 99.
        //          02 TRBCOABI            PIC 9(5)  COMP-3.
        //          02 TRBCOCAB            PIC 9(7)  COMP-3.
        //          02 TRBCOCON            PIC 9(12) COMP-3.
        //          02 TRBCOVAL            PIC X.
        //          02 TRBCOPAG            PIC X.
        //D2NEW     02 TRBLG140            PIC 9(6).                              
        //D2NEW     02 TRBSEN140           PIC 9.        
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  TRB-DELEG.
        /// <summary>
        /// TRBTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string TRBTIPOR { get; set; }

        /// <summary>
        /// TRBCODIC X  
        /// </summary>
        [HisFieldInfoMapping(1, 1)]
        public string TRBCODIC { get; set; }

        /// <summary>
        /// TRBCONOM X(32)  
        /// </summary>
        [HisFieldInfoMapping(2, 32)]
        public string TRBCONOM { get; set; }

        // 02 TRBDTNAS.
        /// <summary>
        /// TRBAANAS 9(4)  
        /// </summary>
        [HisFieldInfoMapping(3, 4, CobolType = CobolType.Unsigned)]
        public short TRBAANAS { get; set; }

        /// <summary>
        /// TRBMMNAS 99  
        /// </summary>
        [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
        public short TRBMMNAS { get; set; }

        /// <summary>
        /// TRBGGNAS 99  
        /// </summary>
        [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
        public short TRBGGNAS { get; set; }

        /// <summary>
        /// TRBCOFIS X(16)  
        /// </summary>
        [HisFieldInfoMapping(6, 16)]
        public string TRBCOFIS { get; set; }

        /// <summary>
        /// TRBCFSIT X  
        /// </summary>
        [HisFieldInfoMapping(7, 1)]
        public string TRBCFSIT { get; set; }

        /// <summary>
        /// TRBINDIR X(32)  
        /// </summary>
        [HisFieldInfoMapping(8, 32)]
        public string TRBINDIR { get; set; }

        /// <summary>
        /// TRBCORES X(22)  
        /// </summary>
        [HisFieldInfoMapping(9, 22)]
        public string TRBCORES { get; set; }

        /// <summary>
        /// TRBPRRES XX  
        /// </summary>
        [HisFieldInfoMapping(10, 2)]
        public string TRBPRRES { get; set; }

        /// <summary>
        /// TRBCAPPP 9(5)  
        /// </summary>
        [HisFieldInfoMapping(11, 5, CobolType = CobolType.Unsigned)]
        public int TRBCAPPP { get; set; }

        /// <summary>
        /// TRBCONAS 9(5)  
        /// </summary>
        [HisFieldInfoMapping(12, 5, CobolType = CobolType.Unsigned)]
        public int TRBCONAS { get; set; }

        /// <summary>
        /// TRBPRNAS 99  
        /// </summary>
        [HisFieldInfoMapping(13, 2, CobolType = CobolType.Unsigned)]
        public short TRBPRNAS { get; set; }

        /// <summary>
        /// TRBCOABI 9(5) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(14, 3, CobolType = CobolType.Comp3Unsigned)]
        public int TRBCOABI { get; set; }

        /// <summary>
        /// TRBCOCAB 9(7) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(15, 4, CobolType = CobolType.Comp3Unsigned)]
        public int TRBCOCAB { get; set; }

        /// <summary>
        /// TRBCOCON 9(12) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(16, 7, CobolType = CobolType.Comp3Unsigned)]
        public long TRBCOCON { get; set; }

        /// <summary>
        /// TRBCOVAL X  
        /// </summary>
        [HisFieldInfoMapping(17, 1)]
        public string TRBCOVAL { get; set; }

        /// <summary>
        /// TRBCOPAG X  
        /// </summary>
        [HisFieldInfoMapping(18, 1)]
        public string TRBCOPAG { get; set; }

        /// <summary>
        /// TRBLG140 9(6)  
        /// </summary>
        [HisFieldInfoMapping(19, 6, CobolType = CobolType.Unsigned)]
        public int TRBLG140 { get; set; }

        /// <summary>
        /// TRBSEN140 9  
        /// </summary>
        [HisFieldInfoMapping(20, 1, CobolType = CobolType.Unsigned)]
        public short TRBSEN140 { get; set; }

        #endregion Tracciato Host

        public string TransactionName
        {
            get { return "Delegato"; }
        }

        #endregion Properties
    }
}
