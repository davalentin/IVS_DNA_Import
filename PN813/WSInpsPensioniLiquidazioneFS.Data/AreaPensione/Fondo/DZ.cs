using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo
{
    public class DZ : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  XDZ-RECFOND-BIS REDEFINES COMUNE-RECFOND.
        //          02  XDZ-RECFOND.
        //              03 XDZTIPOR                      PIC X.
        //              03 XDZFONDO                      PIC X(3).
        //              03 XDZTPENS                      PIC 9.
        //              03 XDZNATUR.
        //                 04 XDZNATU1                   PIC 9.
        //                 04 XDZNATU2                   PIC X.
        //                 04 XDZNATU3                   PIC X.
        //              03 XDZDECOR.
        //                 04 XDZDECAA                   PIC 9999.                
        //                 04 XDZDECMM                   PIC 99.
        //              03 XDZSOSPE.
        //                 04 XDZSOSAA                   PIC 9999.                
        //                 04 XDZSOSMM                   PIC 99.
        //              03 XDZPVERS.
        //                   04 XDZPVRAA                   PIC 9999.              
        //                   04 XDZPVRMM                   PIC 99.
        //                   04 XDZPVRGG                   PIC 99.
        //              03 XDZUVERS.
        //                   04 XDZUVRAA                   PIC 9999.              
        //                   04 XDZUVRMM                   PIC 99.
        //                   04 XDZUVRGG                   PIC 99.
        //              03 XDZDATUT.
        //                  04 XDZUTIAA                    PIC 99.
        //                  04 XDZUTIMM                    PIC 99.
        //              03 XDZRETRI                        PIC 9(6)V9999.
        //              03 XDZCONVE                        PIC X.
        //              03 XDZNOCAL                        PIC 9.
        //              03 XDZCOMBA                        PIC XX.
        //              03 XDZNO336                        PIC 9(6)V9999.
        //              03 XDZFISSE                        PIC 9.
        //              03 XDZDARIS.
        //                  04 XDZRISAA                    PIC 99.
        //                  04 XDZRISMM                    PIC 99.
        //D2000         03 XDZDAPRI.
        //                  04 XDZPRIVA                    PIC 99.
        //                  04 XDZPRIVM                    PIC 99.
        //              03 XDZCPANE                        PIC 9.
        //              03 XDZCODIG                        PIC 9.
        //              03 XDZCODDZ                        PIC 9.
        //              03 XDZANBAS                        PIC 9(4)V99999.
        //              03 XDZ50CLA                        PIC 99.
        //              03 XDZDACES.
        //                  04 XDZCESAA                    PIC 9999.              
        //                  04 XDZCESMM                    PIC 99.
        //                  04 XDZCESGG                    PIC 99.
        //              03 XDZCODIT                        PIC 9(4).
        //              03 XDZPERCE                        PIC 9(5).
        //              03 XDZCODES                        PIC 9.
        //              03 XDZDATES.
        //                  04 XDZANZAA                    PIC 99.
        //                  04 XDZANZMM                    PIC 99.
        //              03 XDZRETNO                        PIC 9(6)V9999.
        //              03 XDZNONVE                        PIC 9.
        //              03 XDZSPECI                        PIC X.
        //      *-L.503
        //              03 XDZREQU1                        PIC X.
        //              03 XDZREQU2                        PIC 9.
        //              03 XDZDA2UT.
        //                 04 XDZUT2AA                     PIC 99.
        //                 04 XDZUT2MM                     PIC 99.
        //              03 XDZRETR2                        PIC 9(6)V9999.
        //              03 XDZN2336                        PIC 9(6)V9999.
        //              03 XDZPROGR                        PIC 99.
        //           02 FILLER                             PIC X(110).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  XDZ-RECFOND-BIS REDEFINES COMUNE-RECFOND.
        // 02  XDZ-RECFOND.
        /// <summary>
        /// XDZTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string XDZTIPOR { get; set; }

        /// <summary>
        /// XDZFONDO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string XDZFONDO { get; set; }

        /// <summary>
        /// XDZTPENS 9  
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short XDZTPENS { get; set; }

        // 03 XDZNATUR.
        /// <summary>
        /// XDZNATU1 9  
        /// </summary>
        [HisFieldInfoMapping(3, 1, CobolType = CobolType.Unsigned)]
        public short XDZNATU1 { get; set; }

        /// <summary>
        /// XDZNATU2 X  
        /// </summary>
        [HisFieldInfoMapping(4, 1)]
        public string XDZNATU2 { get; set; }

        /// <summary>
        /// XDZNATU3 X  
        /// </summary>
        [HisFieldInfoMapping(5, 1)]
        public string XDZNATU3 { get; set; }

        // 03 XDZDECOR.
        /// <summary>
        /// XDZDECAA 9999  
        /// </summary>
        [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
        public short XDZDECAA { get; set; }

        /// <summary>
        /// XDZDECMM 99  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short XDZDECMM { get; set; }

        // 03 XDZSOSPE.
        /// <summary>
        /// XDZSOSAA 9999  
        /// </summary>
        [HisFieldInfoMapping(8, 4, CobolType = CobolType.Unsigned)]
        public short XDZSOSAA { get; set; }

        /// <summary>
        /// XDZSOSMM 99  
        /// </summary>
        [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
        public short XDZSOSMM { get; set; }

        // 03 XDZPVERS.
        /// <summary>
        /// XDZPVRAA 9999  
        /// </summary>
        [HisFieldInfoMapping(10, 4, CobolType = CobolType.Unsigned)]
        public short XDZPVRAA { get; set; }

        /// <summary>
        /// XDZPVRMM 99  
        /// </summary>
        [HisFieldInfoMapping(11, 2, CobolType = CobolType.Unsigned)]
        public short XDZPVRMM { get; set; }

        /// <summary>
        /// XDZPVRGG 99  
        /// </summary>
        [HisFieldInfoMapping(12, 2, CobolType = CobolType.Unsigned)]
        public short XDZPVRGG { get; set; }

        // 03 XDZUVERS.
        /// <summary>
        /// XDZUVRAA 9999  
        /// </summary>
        [HisFieldInfoMapping(13, 4, CobolType = CobolType.Unsigned)]
        public short XDZUVRAA { get; set; }

        /// <summary>
        /// XDZUVRMM 99  
        /// </summary>
        [HisFieldInfoMapping(14, 2, CobolType = CobolType.Unsigned)]
        public short XDZUVRMM { get; set; }

        /// <summary>
        /// XDZUVRGG 99  
        /// </summary>
        [HisFieldInfoMapping(15, 2, CobolType = CobolType.Unsigned)]
        public short XDZUVRGG { get; set; }

        // 03 XDZDATUT.
        /// <summary>
        /// XDZUTIAA 99  
        /// </summary>
        [HisFieldInfoMapping(16, 2, CobolType = CobolType.Unsigned)]
        public short XDZUTIAA { get; set; }

        /// <summary>
        /// XDZUTIMM 99  
        /// </summary>
        [HisFieldInfoMapping(17, 2, CobolType = CobolType.Unsigned)]
        public short XDZUTIMM { get; set; }

        /// <summary>
        /// XDZRETRI 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(18, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XDZRETRI { get; set; }

        /// <summary>
        /// XDZCONVE X  
        /// </summary>
        [HisFieldInfoMapping(19, 1)]
        public string XDZCONVE { get; set; }

        /// <summary>
        /// XDZNOCAL 9  
        /// </summary>
        [HisFieldInfoMapping(20, 1, CobolType = CobolType.Unsigned)]
        public short XDZNOCAL { get; set; }

        /// <summary>
        /// XDZCOMBA XX  
        /// </summary>
        [HisFieldInfoMapping(21, 2)]
        public string XDZCOMBA { get; set; }

        /// <summary>
        /// XDZNO336 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(22, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XDZNO336 { get; set; }

        /// <summary>
        /// XDZFISSE 9  
        /// </summary>
        [HisFieldInfoMapping(23, 1, CobolType = CobolType.Unsigned)]
        public short XDZFISSE { get; set; }

        // 03 XDZDARIS.
        /// <summary>
        /// XDZRISAA 99  
        /// </summary>
        [HisFieldInfoMapping(24, 2, CobolType = CobolType.Unsigned)]
        public short XDZRISAA { get; set; }

        /// <summary>
        /// XDZRISMM 99  
        /// </summary>
        [HisFieldInfoMapping(25, 2, CobolType = CobolType.Unsigned)]
        public short XDZRISMM { get; set; }

        // D2000         03 XDZDAPRI.
        /// <summary>
        /// XDZPRIVA 99  
        /// </summary>
        [HisFieldInfoMapping(26, 2, CobolType = CobolType.Unsigned)]
        public short XDZPRIVA { get; set; }

        /// <summary>
        /// XDZPRIVM 99  
        /// </summary>
        [HisFieldInfoMapping(27, 2, CobolType = CobolType.Unsigned)]
        public short XDZPRIVM { get; set; }

        /// <summary>
        /// XDZCPANE 9  
        /// </summary>
        [HisFieldInfoMapping(28, 1, CobolType = CobolType.Unsigned)]
        public short XDZCPANE { get; set; }

        /// <summary>
        /// XDZCODIG 9  
        /// </summary>
        [HisFieldInfoMapping(29, 1, CobolType = CobolType.Unsigned)]
        public short XDZCODIG { get; set; }

        /// <summary>
        /// XDZCODDZ 9  
        /// </summary>
        [HisFieldInfoMapping(30, 1, CobolType = CobolType.Unsigned)]
        public short XDZCODDZ { get; set; }

        /// <summary>
        /// XDZANBAS 9(4)V9(5)  
        /// </summary>
        [HisFieldInfoMapping(31, 9, Scale = 5, CobolType = CobolType.Unsigned)]
        public decimal XDZANBAS { get; set; }

        /// <summary>
        /// XDZ50CLA 99  
        /// </summary>
        [HisFieldInfoMapping(32, 2, CobolType = CobolType.Unsigned)]
        public short XDZ50CLA { get; set; }

        // 03 XDZDACES.
        /// <summary>
        /// XDZCESAA 9999  
        /// </summary>
        [HisFieldInfoMapping(33, 4, CobolType = CobolType.Unsigned)]
        public short XDZCESAA { get; set; }

        /// <summary>
        /// XDZCESMM 99  
        /// </summary>
        [HisFieldInfoMapping(34, 2, CobolType = CobolType.Unsigned)]
        public short XDZCESMM { get; set; }

        /// <summary>
        /// XDZCESGG 99  
        /// </summary>
        [HisFieldInfoMapping(35, 2, CobolType = CobolType.Unsigned)]
        public short XDZCESGG { get; set; }

        /// <summary>
        /// XDZCODIT 9(4)  
        /// </summary>
        [HisFieldInfoMapping(36, 4, CobolType = CobolType.Unsigned)]
        public short XDZCODIT { get; set; }

        /// <summary>
        /// XDZPERCE 9(5)  
        /// </summary>
        [HisFieldInfoMapping(37, 5, CobolType = CobolType.Unsigned)]
        public int XDZPERCE { get; set; }

        /// <summary>
        /// XDZCODES 9  
        /// </summary>
        [HisFieldInfoMapping(38, 1, CobolType = CobolType.Unsigned)]
        public short XDZCODES { get; set; }

        // 03 XDZDATES.
        /// <summary>
        /// XDZANZAA 99  
        /// </summary>
        [HisFieldInfoMapping(39, 2, CobolType = CobolType.Unsigned)]
        public short XDZANZAA { get; set; }

        /// <summary>
        /// XDZANZMM 99  
        /// </summary>
        [HisFieldInfoMapping(40, 2, CobolType = CobolType.Unsigned)]
        public short XDZANZMM { get; set; }

        /// <summary>
        /// XDZRETNO 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(41, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XDZRETNO { get; set; }

        /// <summary>
        /// XDZNONVE 9  
        /// </summary>
        [HisFieldInfoMapping(42, 1, CobolType = CobolType.Unsigned)]
        public short XDZNONVE { get; set; }

        /// <summary>
        /// XDZSPECI X  
        /// </summary>
        [HisFieldInfoMapping(43, 1)]
        public string XDZSPECI { get; set; }

        // *-L.503
        /// <summary>
        /// XDZREQU1 X  
        /// </summary>
        [HisFieldInfoMapping(44, 1)]
        public string XDZREQU1 { get; set; }

        /// <summary>
        /// XDZREQU2 9  
        /// </summary>
        [HisFieldInfoMapping(45, 1, CobolType = CobolType.Unsigned)]
        public short XDZREQU2 { get; set; }

        // 03 XDZDA2UT.
        /// <summary>
        /// XDZUT2AA 99  
        /// </summary>
        [HisFieldInfoMapping(46, 2, CobolType = CobolType.Unsigned)]
        public short XDZUT2AA { get; set; }

        /// <summary>
        /// XDZUT2MM 99  
        /// </summary>
        [HisFieldInfoMapping(47, 2, CobolType = CobolType.Unsigned)]
        public short XDZUT2MM { get; set; }

        /// <summary>
        /// XDZRETR2 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(48, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XDZRETR2 { get; set; }

        /// <summary>
        /// XDZN2336 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(49, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XDZN2336 { get; set; }

        /// <summary>
        /// XDZPROGR 99  
        /// </summary>
        [HisFieldInfoMapping(50, 2, CobolType = CobolType.Unsigned)]
        public short XDZPROGR { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "DZ"; }
        }
        #endregion Properties
    }
}
