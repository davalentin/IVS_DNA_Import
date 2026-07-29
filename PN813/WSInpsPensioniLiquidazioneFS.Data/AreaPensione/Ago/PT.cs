using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago
{
    public class PT : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //       01  YFS-RECAGO-BIS REDEFINES COMUNE-RECAGO.
        //          02  YFS-RECAGO.
        //              03 YFSTIPRC PIC X.
        //              03 YFSFONDO PIC X(3).
        //              03 YFSTPENS PIC 9.
        //              03 YFSDECAA PIC 9999.
        //              03 YFSDECMM PIC 99.
        //              03 YFSSCAAA PIC 9999.
        //              03 YFSSCAMM PIC 99.
        //              03 YFSSETTC PIC 9(4).        
        //              03 YFSCONTR PIC 9(10)V9999.
        //              03 YFSMONTA PIC 9(10)V9999.
        //              03 YFSQUOTAC PIC 9(9)V9999.
        //GD0212        03 YFSSETT2012 PIC 9(4).              
        //GD0212        03 YFSCONTR2012 PIC 9(10)V9999.
        //GD0212        03 YFSMONTA2012 PIC 9(10)V9999.
        //              03 YFSQUOTA2012 PIC 9(9)V9999.
        //GD1012        03 YFSFLAG214 PIC X.
        //GD1012        03 YFSPERC214 PIC 99V99.
        //GD0119        03 YFSPAL707 PIC 9(7)V9999.
        //GD0119        03 YFSSU92-707                   PIC 9(4).
        //GD0119        03 YFSQUOTA92-707                PIC 9(9)V9999.
        //GD0119        03 YFSSU94-707                   PIC 9(4).
        //GD0119        03 YFSQUOTA94-707                PIC 9(9)V9999.
        //GD0119        03 YFSSU95-707                   PIC 9(4).
        //GD0119        03 YFSQUOTA95-707                PIC 9(9)V9999.
        //GD0119        03 YFSSU97-707                   PIC 9(4).
        //GD0119        03 YFSQUOTA97-707                PIC 9(9)V9999.
        //GD0119        03 YFSSUCE0707 PIC 9(4).
        //GD0119        03 YFSQUOTACE-707                PIC 9(9)V9999.
        //GD0119        03 YFSQUOTA92 PIC 9(9)V9999.
        //GD0119        03 YFSQUOTA94 PIC 9(9)V9999.
        //GD0119        03 YFSQUOTA95 PIC 9(9)V9999.
        //GD0119        03 YFSQUOTA97 PIC 9(9)V9999.
        //GD0119        03 YFSQUOTACE PIC 9(9)V9999.
        //              03 YFSCOEFTRA PIC 9(2)V(4).
        //              03 YFSTIPCALC X  
        //              03 YFSPROGR PIC 99.
        //FSNEW         03 YFSDISPO PIC X(118).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  YFS-RECAGO-BIS REDEFINES COMUNE-RECAGO.
        // 02  YFS-RECAGO.
        /// <summary>
        /// YFSTIPRC X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string YFSTIPRC { get; set; }

        /// <summary>
        /// YFSFONDO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string YFSFONDO { get; set; }

        /// <summary>
        /// YFSTPENS 9  
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short YFSTPENS { get; set; }

        /// <summary>
        /// YFSDECAA 9999  
        /// </summary>
        [HisFieldInfoMapping(3, 4, CobolType = CobolType.Unsigned)]
        public short YFSDECAA { get; set; }

        /// <summary>
        /// YFSDECMM 99  
        /// </summary>
        [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
        public short YFSDECMM { get; set; }

        /// <summary>
        /// YFSSCAAA 9999  
        /// </summary>
        [HisFieldInfoMapping(5, 4, CobolType = CobolType.Unsigned)]
        public short YFSSCAAA { get; set; }

        /// <summary>
        /// YFSSCAMM 99  
        /// </summary>
        [HisFieldInfoMapping(6, 2, CobolType = CobolType.Unsigned)]
        public short YFSSCAMM { get; set; }

        /// <summary>
        /// YFSSETTC 9(4).
        /// </summary>
        [HisFieldInfoMapping(7, 4, CobolType = CobolType.Unsigned)]
        public short YFSSETTC { get; set; }

        /// <summary>
        /// YFSCONTR 9(10)V9999 
        /// </summary>
        [HisFieldInfoMapping(8, 14, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YFSCONTR { get; set; }

        /// <summary>
        /// YFSMONTA 9(10)V9999.
        /// </summary>
        [HisFieldInfoMapping(9, 14, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YFSMONTA { get; set; }

        /// <summary>
        /// YFSQUOTAC 9(9)V9999
        /// <summary>
        [HisFieldInfoMapping(10, 13, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YFSQUOTAC { get; set; }

        /// <summary>
        /// YFSSETT2012 9(4)
        /// <summary>
        [HisFieldInfoMapping(11, 4, CobolType = CobolType.Unsigned)]
        public short YFSSETT2012 { get; set; }

        /// <summary>
        /// YFSCONTR2012 9(10)V9999
        /// </summary>
        [HisFieldInfoMapping(12, 14, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YFSCONTR2012 { get; set; }

        /// <summary>
        /// YFSMONTA2012 9(10)V9999
        /// <summary>
        [HisFieldInfoMapping(13, 14, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YFSMONTA2012 { get; set; }

        /// <summary>
        /// YFSQUOTA2012 9(9)V9999
        /// <summary>
        [HisFieldInfoMapping(14, 13, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YFSQUOTA2012 { get; set; }

        /// <summary>
        /// YFSFLAG214 X
        /// <summary>
        [HisFieldInfoMapping(15, 1)]
        public string YFSFLAG214 { get; set; }

        /// <summary>
        /// YFSPERC214 99V99
        /// <summary>
        [HisFieldInfoMapping(16, 4, Scale = 2, CobolType = CobolType.Unsigned)]
        public decimal YFSPERC214 { get; set; }

        /// <summary>
        /// YFSPAL707 9(7)V9999
        /// </summary>
        [HisFieldInfoMapping(17, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YFSPAL707 { get; set; }

        /// <summary>
        /// YFSSU92-707 9(4)
        /// </summary>
        [HisFieldInfoMapping(18, 4, CobolType = CobolType.Unsigned)]
        public short YFSSU92_707 { get; set; }

        /// <summary>
        /// YFSQUOTA92-707 9(9)V9999
        /// </summary>
        [HisFieldInfoMapping(19, 13, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YFSQUOTA92_707 { get; set; }

        /// <summary>
        /// YFSSU94-707 9(4)
        /// </summary>
        [HisFieldInfoMapping(20, 4, CobolType = CobolType.Unsigned)]
        public short YFSSU94_707 { get; set; }

        /// <summary>
        /// YFSQUOTA94-707 9(9)V9999
        /// </summary>
        [HisFieldInfoMapping(21, 13, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YFSQUOTA94_707 { get; set; }

        /// <summary>
        /// YFSSU95-707 9(4)
        /// </summary>
        [HisFieldInfoMapping(22, 4, CobolType = CobolType.Unsigned)]
        public short YFSSU95_707 { get; set; }

        /// <summary>
        /// YFSQUOTA95-707 9(9)V9999
        /// </summary>
        [HisFieldInfoMapping(23, 13, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YFSQUOTA95_707 { get; set; }

        /// <summary>
        /// YFSSU97-707 9(4)
        /// </summary>
        [HisFieldInfoMapping(24, 4, CobolType = CobolType.Unsigned)]
        public short YFSSU97_707 { get; set; }

        /// <summary>
        /// YFSQUOTA97-707 9(9)V9999
        /// </summary>
        [HisFieldInfoMapping(25, 13, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YFSQUOTA97_707 { get; set; }

        /// <summary>
        /// YFSSUCE0707 9(4)
        /// </summary>
        [HisFieldInfoMapping(26, 4, CobolType = CobolType.Unsigned)]
        public short YFSSUCE_707 { get; set; }

        /// <summary>
        /// YFSQUOTACE-707 9(9)V9999
        /// </summary>
        [HisFieldInfoMapping(27, 13, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YFSQUOTACE_707 { get; set; }

        /// <summary>
        /// YFSQUOTA92 9(9)V9999
        /// </summary>
        [HisFieldInfoMapping(28, 13, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YFSQUOTA92 { get; set; }

        /// <summary>
        /// YFSQUOTA94 9(9)V9999
        /// </summary>
        [HisFieldInfoMapping(29, 13, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YFSQUOTA94 { get; set; }

        /// <summary>
        /// YFSQUOTA95 9(9)V9999
        /// </summary>
        [HisFieldInfoMapping(30, 13, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YFSQUOTA95 { get; set; }

        /// <summary>
        /// YFSQUOTA97 9(9)V9999
        /// </summary>
        [HisFieldInfoMapping(31, 13, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YFSQUOTA97 { get; set; }

        /// <summary>
        /// YFSQUOTACE 9(9)V9999
        /// </summary>
        [HisFieldInfoMapping(32, 13, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YFSQUOTACE { get; set; }

        /// <summary>
        /// YFSCOEFTRA 9(2)V(4)
        /// </summary>
        [HisFieldInfoMapping(33, 6, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YFSCOEFTRA { get; set; }

        /// YFSTIPCALC X  
        /// </summary>
        [HisFieldInfoMapping(34, 1)]
        public string YFSTIPCALC { get; set; }

        /// <summary>
        /// PAL214-FS-EURO 9(7)V9999
        /// </summary>
        [HisFieldInfoMapping(35, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YFSPAL214 { get; set; }

        /// <summary>
        /// YFSPROGR 99  
        /// </summary>
        [HisFieldInfoMapping(36, 2, CobolType = CobolType.Unsigned)]
        public short YFSPROGR { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "PT"; }
        }
        #endregion Properties
    }
}
