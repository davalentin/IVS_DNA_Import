using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA
{
    public class TrattenuteLavAutonomi : ITransactionInfo
    {
        #region Constructor
        internal TrattenuteLavAutonomi()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //01  TRM-AUTON.
        //02 TRMTIPOR            PIC X VALUE "M".
        //02 TRM-AUTON1 OCCURS 30 TIMES.
        //   03 TRMAARIF            PIC 9(4).
        //   03 TRMREDD             PIC 9(6)V99.
        //   03 TRMMMDAL            PIC 9(2).
        //   03 TRMMMAL             PIC 9(2).
        //02 TRMDISPO            PIC X(71).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  TRM-AUTON.
        /// <summary>
        /// TRMTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string TRMTIPOR { get; set; }

        /// <summary>
        /// TRM-AUTON1 OCCURS 30 TIMES.
        /// </summary>
        [HisComplexAreaInfoMapping(1, ListCount = 30)]
        public List<TRM_AUTON1> LISTTRM_AUTON1 { get; set; }

        /// <summary>
        /// TRMDISPO X(71)  
        /// </summary>
        [HisFieldInfoMapping(2, 71)]
        public string TRMDISPO { get; set; }
        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "TrattenuteLavAutonomi"; }
        }
        #endregion Properties

        #region nested class
        public class TRM_AUTON1
        {
            #region Constructor
            internal TRM_AUTON1()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //02 TRM-AUTON1 OCCURS 30 TIMES.
            //   03 TRMAARIF            PIC 9(4).
            //   03 TRMREDD             PIC 9(6)V99.
            //   03 TRMMMDAL            PIC 9(2).
            //   03 TRMMMAL             PIC 9(2).
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 TRM-AUTON1 OCCURS 30 TIMES.
            /// <summary>
            /// TRMAARIF 9(4)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short TRMAARIF { get; set; }

            /// <summary>
            /// TRMREDD 9(6)V9(2)  
            /// </summary>
            [HisFieldInfoMapping(1, 8, Scale = 2, CobolType = CobolType.Unsigned)]
            public decimal TRMREDD { get; set; }

            /// <summary>
            /// TRMMMDAL 9(2)  
            /// </summary>
            [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
            public short TRMMMDAL { get; set; }

            /// <summary>
            /// TRMMMAL 9(2)  
            /// </summary>
            [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
            public short TRMMMAL { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class
    }
}
