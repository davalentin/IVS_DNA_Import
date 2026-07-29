using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA
{
    public class Redditi : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  TRR-REDDI.
        //02 TRRTIPOR            PIC X VALUE "R".
        //02 TRRDATI.
        //03 TRR-WEBDOM.
        //04 TRR-GRUPPO PIC 9(4).
        //04 TRR-PROD PIC 9(4).
        //04 TRR-TIPO PIC 9(4).
        //04 TRR-TIPO-DOM PIC X(3).        
        //02 TRR-DATA-COMP PIC 9(8).
        //04 FLAGSENT_R PIC 9(1)
        //04 CITTA_R X(3)
        //04 DTREQ_R X(8)
        //02   GP1AV91B_R X(1)
        //02 GP3CB02_R X(32)
        //02 GP3CB03_R X(32) 
        //04 R-Note-TE08 PIC 9(10)
        //03 FILLER PIC X(1892).

        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  TRR-REDDI.
        /// <summary>
        /// TRRTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string TRRTIPOR { get; set; }

        /// <summary>
        /// TRR-GRUPPO 9(4)  
        /// </summary>
        [HisFieldInfoMapping(1, 4)]
        public int TRR_GRUPPO { get; set; }

        /// <summary>
        /// TRR-PROD 9(4)   
        /// </summary>
        [HisFieldInfoMapping(2, 4)]
        public int TRR_PROD { get; set; }

        /// <summary>
        /// TRR-TIPO 9(4)  
        /// </summary>
        [HisFieldInfoMapping(3, 4)]
        public int TRR_TIPO { get; set; }

        /// <summary>
        /// TRR-TIPO-DOM X(3)  
        /// </summary>
        [HisFieldInfoMapping(4, 3)]
        public string TRR_TIPO_DOM { get; set; }

        /// <summary>
        /// TRR-DATA-COMP 9(8)  
        /// </summary>
        [HisFieldInfoMapping(5, 8)]
        public int TRR_DATA_COMP { get; set; }

        /// <summary>
        /// FLAGSENT_R 9(1)  
        /// </summary>
        [HisFieldInfoMapping(6, 1)]
        public int FLAGSENT_R { get; set; }

        /// <summary>
        /// CITTA_R X(3)  
        /// </summary>
        [HisFieldInfoMapping(7, 3)]
        public string CITTA_R { get; set; }

        /// <summary>
        /// DTREQ_R  X(8)  
        /// </summary>
        [HisFieldInfoMapping(8, 8)]
        public string DTREQ_R { get; set; }

        /// <summary>
        /// GP1TPCLC_R X(8) 
        /// </summary>
        [HisFieldInfoMapping(9, 8)]
        public string GP1TPCLC_R { get; set; }

        /// <summary>
        /// GP1AV91B_R X(1) 
        /// </summary>
        [HisFieldInfoMapping(10, 1)]
        public string GP1AV91B_R { get; set; }

        /// <summary>
        /// GP3CB02-R X(32) 
        /// </summary>
        [HisFieldInfoMapping(11, 32)]
        public string GP3CB02_R { get; set; }


        /// <summary>
        /// GP3CB03-R X(32) 
        /// </summary>
        [HisFieldInfoMapping(12, 32)]
        public string GP3CB03_R { get; set; }
        
        /// <summary>
        /// R-Note-TE08(10)   
        /// </summary>
        [HisFieldInfoMapping(13, 10)]
        public int R_Note_TE08 { get; set; }

        /// <summary>
        /// GP2BM00_R OCCURS 5 TIMES.
        /// </summary>
        [HisComplexAreaInfoMapping(14, ListCount = 5)]
        public List<GP2BM00_R> LISGP2BM00_R { get; set; }

        /// <summary>
        /// SEDE_METAPROCESSO
        /// </summary>
        [HisFieldInfoMapping(15, 6)]
        public string SEDE_METAPROCESSO { get; set; }

        /// <summary>
        /// TRRDATI X(1975)  
        /// </summary>
        [HisFieldInfoMapping(16, 1846)] 
        public string FILLER { get; set; }

        

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "Redditi"; }
        }
        #endregion Properties
        
        #region nested class
        public class GP2BM00_R
        { 
            /// <summary>
            /// GP2BMTA-R
            /// </summary>
            [HisFieldInfoMapping(0, 2)]
            public string GP2BMTA_R { get; set; }

            /// <summary>
            /// GP2BM13-R   PIC S9(9)V9(2) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(1, 6, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal GP2BM13_R { get; set; }
        }
        #endregion nested class
    }
}
