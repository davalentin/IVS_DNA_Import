using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo
{
    public class PT : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  XFS-RECFOND-BIS REDEFINES COMUNE-RECFOND.
        //          02  XFS-RECFOND.
        //              03 XFSTIPOR                      PIC X.
        //              03 XFSFONDO                      PIC X(3).
        //              03 XFSTPENS                      PIC 9.
        //              03 XFSNATUR.
        //                 04 XFSNATU1                   PIC X.                   
        //                 04 XFSNATU2                   PIC X.
        //                 04 XFSNATU3                   PIC X.
        //              03 XFSDECOR.
        //                 04 XFSDECAA                   PIC 9999.                
        //                 04 XFSDECMM                   PIC 99.
        //                 04 XFSDECGG                   PIC 99.
        //              03 XFSSCADE.
        //                 04 XFSSCAAA                   PIC 9999.                
        //                 04 XFSSCAMM                   PIC 99.
        //              03 XFSDECEC.
        //                 04 XFSDECECAA                 PIC 9999.                
        //                 04 XFSDECECMM                 PIC 99.
        //                 04 XFSDECECGG                 PIC 99.
        //              03 XFSASSUN.
        //                 04 XFSASSAA                   PIC 9999.                
        //                 04 XFSASSMM                   PIC 99.
        //                 04 XFSASSGG                   PIC 99.
        //              03 XFSCESSA.
        //                 04 XFSCESAA                   PIC 9999.                
        //                 04 XFSCESMM                   PIC 99.
        //                 04 XFSCESGG                   PIC 99.
        //              03 XFSMATR                       PIC X(7).
        //              03 XFSCSPEC                      PIC X.
        //              03 XFSCAUSA                      PIC 9(4).
        //              03 XFSPROF                       PIC X(4).
        //FSMOD         03 XFSNCALC                      PIC X.
        //              03 XFSPAL                        PIC 9(6)V9999.
        //              03 XFSFLINP                      PIC X.
        //              03 XFSDIIS                       PIC 9.
        //              03 XFSF13ME                      PIC 9.
        //              03 XFSFAAGO                      PIC 9.
        //              03 XFSASSAC                      PIC 9(8).
        //              03 XFSSU92                       PIC 9(5).
        //              03 XFSSU94                       PIC 9(5).
        //              03 XFSSU95                       PIC 9(5).
        //              03 XFSSU97                       PIC 9(5).
        //              03 XFSSUCE                       PIC 9(5).
        //              03 XFSSUAN                       PIC 9(2).
        //              03 XFSRETR                       PIC 9(6)V9999.
        //              03 XFSQA14                       PIC 9(4)V9999.
        //FSMOD         03 XFSIIS                        PIC 9(6)V9999.
        //              03 XFSNO336                      PIC 9(6)V9999.
        //              03 XFSRETRM                      PIC 9(9)V9999.
        //FSMOD *       03 XFSMIIS                       PIC 9(9).
        //FSMOD *       03 XFSECCA                       PIC 9(9).
        //              03 XFSIISL                       PIC 9(6)V9999.
        //      *       PRIMO BYTE DEL FILLER UTILIZZATO  DA ASSAC9
        //              03 FILLER                        PIC 9(8).
        //              03 XFSPOLO                       PIC 9(4).
        //              03 XFSPROGR                      PIC 9(2).
        //FSNEW         03 XFSDECAL                      PIC 9(8).
        //FSNEW         03 XFSRID                        PIC X.
        //FSNEW         03 XFSCONG                       PIC XX.
        //FSNEW         03 XFSPAL335                     PIC 9(6)V9999.
        //GD0711*CAMPO PER RIPARTITA INPDAP IPOST
        //              03 XRIPINPDAP                    PIC 9(3)V9999. 
        //* CAMPI PER LEGGE 4/60 IPOST
        //              03 XFSIMPC                     PIC 9(7)V9999.
        //              03 XFSPENS                     PIC 9(15) COMP-3.
        //              03 XFSMESIRIS                  PIC 9(3) COMP-3.
        //              03 XFSMESITOT                  PIC 9(3) COMP-3.
        //              03 XFSONEREMEF                 PIC 9.
        //FSNEW      02 FILLER                             PIC X(30).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  XFS-RECFOND-BIS REDEFINES COMUNE-RECFOND.
        // 02  XFS-RECFOND.
        /// <summary>
        /// XFSTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string XFSTIPOR { get; set; }

        /// <summary>
        /// XFSFONDO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string XFSFONDO { get; set; }

        /// <summary>
        /// XFSTPENS 9  
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short XFSTPENS { get; set; }

        // 03 XFSNATUR.
        /// <summary>
        /// XFSNATU1 X  
        /// </summary>
        [HisFieldInfoMapping(3, 1)]
        public string XFSNATU1 { get; set; }

        /// <summary>
        /// XFSNATU2 X  
        /// </summary>
        [HisFieldInfoMapping(4, 1)]
        public string XFSNATU2 { get; set; }

        /// <summary>
        /// XFSNATU3 X  
        /// </summary>
        [HisFieldInfoMapping(5, 1)]
        public string XFSNATU3 { get; set; }

        // 03 XFSDECOR.
        /// <summary>
        /// XFSDECAA 9999  
        /// </summary>
        [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
        public short XFSDECAA { get; set; }

        /// <summary>
        /// XFSDECMM 99  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short XFSDECMM { get; set; }

        /// <summary>
        /// XFSDECGG 99  
        /// </summary>
        [HisFieldInfoMapping(8, 2, CobolType = CobolType.Unsigned)]
        public short XFSDECGG { get; set; }

        // 03 XFSSCADE.
        /// <summary>
        /// XFSSCAAA 9999  
        /// </summary>
        [HisFieldInfoMapping(9, 4, CobolType = CobolType.Unsigned)]
        public short XFSSCAAA { get; set; }

        /// <summary>
        /// XFSSCAMM 99  
        /// </summary>
        [HisFieldInfoMapping(10, 2, CobolType = CobolType.Unsigned)]
        public short XFSSCAMM { get; set; }

        // 03 XFSDECEC.
        /// <summary>
        /// XFSDECECAA 9999  
        /// </summary>
        [HisFieldInfoMapping(11, 4, CobolType = CobolType.Unsigned)]
        public short XFSDECECAA { get; set; }

        /// <summary>
        /// XFSDECECMM 99  
        /// </summary>
        [HisFieldInfoMapping(12, 2, CobolType = CobolType.Unsigned)]
        public short XFSDECECMM { get; set; }

        /// <summary>
        /// XFSDECECGG 99  
        /// </summary>
        [HisFieldInfoMapping(13, 2, CobolType = CobolType.Unsigned)]
        public short XFSDECECGG { get; set; }

        // 03 XFSASSUN.
        /// <summary>
        /// XFSASSAA 9999  
        /// </summary>
        [HisFieldInfoMapping(14, 4, CobolType = CobolType.Unsigned)]
        public short XFSASSAA { get; set; }

        /// <summary>
        /// XFSASSMM 99  
        /// </summary>
        [HisFieldInfoMapping(15, 2, CobolType = CobolType.Unsigned)]
        public short XFSASSMM { get; set; }

        /// <summary>
        /// XFSASSGG 99  
        /// </summary>
        [HisFieldInfoMapping(16, 2, CobolType = CobolType.Unsigned)]
        public short XFSASSGG { get; set; }

        // 03 XFSCESSA.
        /// <summary>
        /// XFSCESAA 9999  
        /// </summary>
        [HisFieldInfoMapping(17, 4, CobolType = CobolType.Unsigned)]
        public short XFSCESAA { get; set; }

        /// <summary>
        /// XFSCESMM 99  
        /// </summary>
        [HisFieldInfoMapping(18, 2, CobolType = CobolType.Unsigned)]
        public short XFSCESMM { get; set; }

        /// <summary>
        /// XFSCESGG 99  
        /// </summary>
        [HisFieldInfoMapping(19, 2, CobolType = CobolType.Unsigned)]
        public short XFSCESGG { get; set; }

        /// <summary>
        /// XFSMATR X(7)  
        /// </summary>
        [HisFieldInfoMapping(20, 7)]
        public string XFSMATR { get; set; }

        /// <summary>
        /// XFSCSPEC X  
        /// </summary>
        [HisFieldInfoMapping(21, 1)]
        public string XFSCSPEC { get; set; }

        /// <summary>
        /// XFSCAUSA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(22, 4, CobolType = CobolType.Unsigned)]
        public short XFSCAUSA { get; set; }

        /// <summary>
        /// XFSPROF X(4)  
        /// </summary>
        [HisFieldInfoMapping(23, 4)]
        public string XFSPROF { get; set; }

        /// <summary>
        /// XFSNCALC X  
        /// </summary>
        [HisFieldInfoMapping(24, 1)]
        public string XFSNCALC { get; set; }

        /// <summary>
        /// XFSPAL 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(25, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XFSPAL { get; set; }

        /// <summary>
        /// XFSFLINP X  
        /// </summary>
        [HisFieldInfoMapping(26, 1)]
        public string XFSFLINP { get; set; }

        /// <summary>
        /// XFSDIIS 9  
        /// </summary>
        [HisFieldInfoMapping(27, 1, CobolType = CobolType.Unsigned)]
        public short XFSDIIS { get; set; }

        /// <summary>
        /// XFSF13ME 9  
        /// </summary>
        [HisFieldInfoMapping(28, 1, CobolType = CobolType.Unsigned)]
        public short XFSF13ME { get; set; }

        /// <summary>
        /// XFSFAAGO 9  
        /// </summary>
        [HisFieldInfoMapping(29, 1, CobolType = CobolType.Unsigned)]
        public short XFSFAAGO { get; set; }

        /// <summary>
        /// XFSASSAC 9(8)  
        /// </summary>
        [HisFieldInfoMapping(30, 8, CobolType = CobolType.Unsigned)]
        public int XFSASSAC { get; set; }

        /// <summary>
        /// XFSSU92 9(5)  
        /// </summary>
        [HisFieldInfoMapping(31, 5, CobolType = CobolType.Unsigned)]
        public int XFSSU92 { get; set; }

        /// <summary>
        /// XFSSU94 9(5)  
        /// </summary>
        [HisFieldInfoMapping(32, 5, CobolType = CobolType.Unsigned)]
        public int XFSSU94 { get; set; }

        /// <summary>
        /// XFSSU95 9(5)  
        /// </summary>
        [HisFieldInfoMapping(33, 5, CobolType = CobolType.Unsigned)]
        public int XFSSU95 { get; set; }

        /// <summary>
        /// XFSSU97 9(5)  
        /// </summary>
        [HisFieldInfoMapping(34, 5, CobolType = CobolType.Unsigned)]
        public int XFSSU97 { get; set; }

        /// <summary>
        /// XFSSUCE 9(5)  
        /// </summary>
        [HisFieldInfoMapping(35, 5, CobolType = CobolType.Unsigned)]
        public int XFSSUCE { get; set; }

        /// <summary>
        /// XFSSUAN 9(2)  
        /// </summary>
        [HisFieldInfoMapping(36, 2, CobolType = CobolType.Unsigned)]
        public short XFSSUAN { get; set; }

        /// <summary>
        /// XFSRETR 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(37, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XFSRETR { get; set; }

        /// <summary>
        /// XFSQA14 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(38, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XFSQA14 { get; set; }

        /// <summary>
        /// XFSIIS 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(39, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XFSIIS { get; set; }

        /// <summary>
        /// XFSNO336 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(40, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XFSNO336 { get; set; }

        /// <summary>
        /// XFSRETRM 9(9)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(41, 13, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XFSRETRM { get; set; }

        /// <summary>
        /// XFSIISL 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(42, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XFSIISL { get; set; }

        // *       PRIMO BYTE DEL FILLER UTILIZZATO  DA ASSAC9
        /// <summary>
        /// FILLER 9(8)  
        /// </summary>
        [HisFieldInfoMapping(43, 8, CobolType = CobolType.Unsigned)]
        public int FILLER { get; set; }

        /// <summary>
        /// XFSPOLO 9(4)  
        /// </summary>
        [HisFieldInfoMapping(44, 4, CobolType = CobolType.Unsigned)]
        public short XFSPOLO { get; set; }

        /// <summary>
        /// XFSPROGR 9(2)  
        /// </summary>
        [HisFieldInfoMapping(45, 2, CobolType = CobolType.Unsigned)]
        public short XFSPROGR { get; set; }

        /// <summary>
        /// XFSDECAL 9(8)  
        /// </summary>
        [HisFieldInfoMapping(46, 8, CobolType = CobolType.Unsigned)]
        public int XFSDECAL { get; set; }

        /// <summary>
        /// XFSRID X  
        /// </summary>
        [HisFieldInfoMapping(47, 1)]
        public string XFSRID { get; set; }

        /// <summary>
        /// XFSCONG XX  
        /// </summary>
        [HisFieldInfoMapping(48, 2)]
        public string XFSCONG { get; set; }

        /// <summary>
        /// XFSPAL335 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(49, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XFSPAL335 { get; set; }

        /// <summary>
        /// XRIPINPDAP 9(3)V9999.
        /// </summary>
        [HisFieldInfoMapping(50, 7, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XRIPINPDAP { get; set; }

        /// <summary>
        //XFSIMPC 9(7)V9999.
        /// <summary>
        [HisFieldInfoMapping(51, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal XFSIMPC { get; set; }

        /// <summary>
        //XFSPENS 9(15) COMP-3.
        /// <summary>
        [HisFieldInfoMapping(52, 8, CobolType = CobolType.Comp3Unsigned)]
        public long XFSPENS { get; set; }

        /// <summary>
        //XFSMESIRIS 9(3) COMP-3.
        /// <summary>
        [HisFieldInfoMapping(53, 2, CobolType = CobolType.Comp3Unsigned)]
        public long XFSMESIRIS { get; set; }
        /// <summary>
        //XFSMESITOT 9(3) COMP-3.
        /// <summary>
        [HisFieldInfoMapping(54, 2, CobolType = CobolType.Comp3Unsigned)]
        public long XFSMESITOT { get; set; }

        /// <summary>
        /// XFSONEREMEF 9  
        /// </summary>
        [HisFieldInfoMapping(55, 1, CobolType = CobolType.Unsigned)]
        public short XFSONEREMEF { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "PT"; }
        }
        #endregion Properties
    }
}
