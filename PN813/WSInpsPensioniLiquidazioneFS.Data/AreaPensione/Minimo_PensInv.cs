using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA
{
    public class Minimo_PensInv : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //       01  TRG-REDMI.
        //          02 TRGTIPOR            PIC X VALUE "G".
        //          02 TRGELERD  OCCURS 24 TIMES.
        //D2NEW        03 TRGRIF01         PIC 9999.                              
        //             03 TRGCOD01         PIC X.
        //             03 TRGRED01         PIC 9(5)V99.
        //             03 TRGCD101         PIC 9.
        //             03 TRGCD201         PIC 9.
        //             03 TRGCD301         PIC 9.
        //             03 TRGCD401         PIC 9.
        //             03 TRGCD501         PIC 9.
        //             03 TRGNDI01         PIC X.
        //      *DATI CONIUGE
        //             03 TRGSTC01         PIC 9.
        //             03 TRGREC01         PIC 9(5)V99.
        //             03 TRGREL01         PIC 9(5)V99.
        //             03 TRGDIS01         PIC XX.
        //D2000     02 TRGDECDI.
        //D2NEW        03 TRGDIAA          PIC 9999.                              
        //             03 TRGDIMM          PIC 99.
        //D2000     02 TRGDECCE.
        //D2NEW        03 TRGCEAA          PIC 9999.                              
        //             03 TRGCEMM          PIC 99.
        //D2000     02 TRGDECSO.
        //D2NEW        03 TRGSOAA          PIC 9999.                              
        //             03 TRGSOMM          PIC 99.
        //D2000     02 TRGDECRI.
        //D2NEW        03 TRGRIAA          PIC 9999.                              
        //             03 TRGRIMM          PIC 99.
        //          02 TRGRECUP            PIC 9(5)V99.
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  TRG-REDMI.
        /// <summary>
        /// TRGTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string TRGTIPOR { get; set; }

        /// <summary>
        /// TRGELERD  OCCURS 24 TIMES.
        /// </summary>
        [HisComplexAreaInfoMapping(1, ListCount = 24)]
        public List<TRGELERD> LISTTRGELERD { get; set; }

        // D2000     02 TRGDECDI.
        /// <summary>
        /// TRGDIAA 9999  
        /// </summary>
        [HisFieldInfoMapping(2, 4, CobolType = CobolType.Unsigned)]
        public short TRGDIAA { get; set; }

        /// <summary>
        /// TRGDIMM 99  
        /// </summary>
        [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
        public short TRGDIMM { get; set; }

        // D2000     02 TRGDECCE.
        /// <summary>
        /// TRGCEAA 9999  
        /// </summary>
        [HisFieldInfoMapping(4, 4, CobolType = CobolType.Unsigned)]
        public short TRGCEAA { get; set; }

        /// <summary>
        /// TRGCEMM 99  
        /// </summary>
        [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
        public short TRGCEMM { get; set; }

        // D2000     02 TRGDECSO.
        /// <summary>
        /// TRGSOAA 9999  
        /// </summary>
        [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
        public short TRGSOAA { get; set; }

        /// <summary>
        /// TRGSOMM 99  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short TRGSOMM { get; set; }

        // D2000     02 TRGDECRI.
        /// <summary>
        /// TRGRIAA 9999  
        /// </summary>
        [HisFieldInfoMapping(8, 4, CobolType = CobolType.Unsigned)]
        public short TRGRIAA { get; set; }

        /// <summary>
        /// TRGRIMM 99  
        /// </summary>
        [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
        public short TRGRIMM { get; set; }

        /// <summary>
        /// TRGRECUP 9(5)V9(2)  
        /// </summary>
        [HisFieldInfoMapping(10, 7, Scale = 2, CobolType = CobolType.Unsigned)]
        public decimal TRGRECUP { get; set; }


        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "Minimo_PensInv"; }
        }
        #endregion Properties

        #region nested class
        public class TRGELERD
        {
            #region Properties

            #region Tracciato COBOL
            //          02 TRGELERD  OCCURS 24 TIMES.
            //D2NEW        03 TRGRIF01         PIC 9999.                              
            //             03 TRGCOD01         PIC X.
            //             03 TRGRED01         PIC 9(5)V99.
            //             03 TRGCD101         PIC 9.
            //             03 TRGCD201         PIC 9.
            //             03 TRGCD301         PIC 9.
            //             03 TRGCD401         PIC 9.
            //             03 TRGCD501         PIC 9.
            //             03 TRGNDI01         PIC X.
            //      *DATI CONIUGE
            //             03 TRGSTC01         PIC 9.
            //             03 TRGREC01         PIC 9(5)V99.
            //             03 TRGREL01         PIC 9(5)V99.
            //             03 TRGDIS01         PIC XX.
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 TRGELERD  OCCURS 24 TIMES.
            /// <summary>
            /// TRGRIF01 9999  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short TRGRIF01 { get; set; }

            /// <summary>
            /// TRGCOD01 X  
            /// </summary>
            [HisFieldInfoMapping(1, 1)]
            public string TRGCOD01 { get; set; }

            /// <summary>
            /// TRGRED01 9(5)V9(2)  
            /// </summary>
            [HisFieldInfoMapping(2, 7, Scale = 2, CobolType = CobolType.Unsigned)]
            public decimal TRGRED01 { get; set; }

            /// <summary>
            /// TRGCD101 9  
            /// </summary>
            [HisFieldInfoMapping(3, 1, CobolType = CobolType.Unsigned)]
            public short TRGCD101 { get; set; }

            /// <summary>
            /// TRGCD201 9  
            /// </summary>
            [HisFieldInfoMapping(4, 1, CobolType = CobolType.Unsigned)]
            public short TRGCD201 { get; set; }

            /// <summary>
            /// TRGCD301 9  
            /// </summary>
            [HisFieldInfoMapping(5, 1, CobolType = CobolType.Unsigned)]
            public short TRGCD301 { get; set; }

            /// <summary>
            /// TRGCD401 9  
            /// </summary>
            [HisFieldInfoMapping(6, 1, CobolType = CobolType.Unsigned)]
            public short TRGCD401 { get; set; }

            /// <summary>
            /// TRGCD501 9  
            /// </summary>
            [HisFieldInfoMapping(7, 1, CobolType = CobolType.Unsigned)]
            public short TRGCD501 { get; set; }

            /// <summary>
            /// TRGNDI01 X  
            /// </summary>
            [HisFieldInfoMapping(8, 1)]
            public string TRGNDI01 { get; set; }

            // *DATI CONIUGE
            /// <summary>
            /// TRGSTC01 9  
            /// </summary>
            [HisFieldInfoMapping(9, 1, CobolType = CobolType.Unsigned)]
            public short TRGSTC01 { get; set; }

            /// <summary>
            /// TRGREC01 9(5)V9(2)  
            /// </summary>
            [HisFieldInfoMapping(10, 7, Scale = 2, CobolType = CobolType.Unsigned)]
            public decimal TRGREC01 { get; set; }

            /// <summary>
            /// TRGREL01 9(5)V9(2)  
            /// </summary>
            [HisFieldInfoMapping(11, 7, Scale = 2, CobolType = CobolType.Unsigned)]
            public decimal TRGREL01 { get; set; }

            /// <summary>
            /// TRGDIS01 XX  
            /// </summary>
            [HisFieldInfoMapping(12, 2)]
            public string TRGDIS01 { get; set; }

            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class
    }
}
