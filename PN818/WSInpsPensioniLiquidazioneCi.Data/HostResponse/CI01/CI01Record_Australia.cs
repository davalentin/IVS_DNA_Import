using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class CI01Record_Australia : ITransactionInfo
    {
        #region Constructor
        #endregion Constructor

        #region tracciato COBOL
        //        ***************************************************************** 
        //***************************************************************** 
        //**** RECORD "AUSTRALIA "             REK DCX     (16000) ******** 
        //**** LUNGHEZZA 1680 BYTES      *    PER  C.I.            ******** 
        //***************************************************************** 
        //          05  FILLER            PIC X(4).                         
        //          05  AUS-CAT-NUM       PIC 9(3).                     
        //          05  AUS-SEDE          PIC 9(4).                         
        //          05  AUS-CERT          PIC 9(8).                         
        //          05  AUS-CAT-ALF       PIC X(8).                         
        //             06 AUS-DATA-ANNO       PIC 9(4).
        //             06 AUS-DATA-MESE       PIC 99.                   
        //             06 AUS-DEOR-ANNO       PIC 9(4).
        //             06 AUS-DEOR-MESE       PIC 99.                
        //             06 AUS-DECA-ANNO       PIC 9(4).
        //             06 AUS-DECA-MESE       PIC 99.
        //          05  AUS-IMPORTO-CORR  PIC S9(7)V9(4) COMP-3.            
        //          05  AUS-ARRET-DISP    PIC S9(7)V9(4) COMP-3.            
        //          05  AUS-ARRET-INDISP  PIC S9(7)V9(4) COMP-3.            
        //          05  AUS-DATI-ANNUALI  OCCURS 20.                        
        //              10  AUS-ANNO      PIC 9(4).                         
        //              10  AUS-PAGATO    PIC 9(7)V9(4) COMP-3.             
        //              10  AUS-ADEGUATA  PIC 9(7)V9(4) COMP-3.             
        //              10  AUS-INTEGR    PIC 9(7)V9(4) COMP-3.             
        //              10  AUS-MINI      PIC 9(7)V9(4) COMP-3.             
        //              10  AUS-ASS-FIGLI PIC 9(7)V9(4) COMP-3.             
        //              10  AUS-ASS-CON   PIC 9(7)V9(4) COMP-3.             
        //              10  AUS-ART6      PIC 9(5)V9(4) COMP-3.             
        //              10  AUS-MAGG-S    PIC 9(5)V9(4) COMP-3.             
        //          05  FILLER            PIC X(29).                        
        //          05  AUS-CTR-TT        PIC 9(4).                         
        //          05  AUS-SES-TT        PIC X(1).                         
        //          05  AUS-SES-DC        PIC X(1).                         
        //          05  FILLER            PIC X(3).                         
        //          05  AUS-SEDE-CARICO   PIC X(22).                        
        //          05  AUS-COGNOME       PIC X(32).                        
        //          05  AUS-NOME          PIC X(32).                        
        //          05  AUS-COGN-ACQ      PIC X(31).                        
        //          05  AUS-INDIRIZZO     PIC X(52).                        
        //          05  AUS-COMUNE-RES    PIC X(36).                        
        //          05  AUS-PROVINCIA-RES PIC X(3).                         
        //          05  AUS-CAP           PIC X(5).                        
        //             06 AUS-DATAGG          PIC 99.
        //             06 AUS-DATAMM          PIC 99.
        //             06 AUS-DATAAAA         PIC 999.
        //          05  AUS-COMUNE-NAS    PIC X(36).                        
        //          05  FILLER            PIC X(83).                        
        //          05  FILLER            PIC X(240).  
        #endregion tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// FILLER X(4)  
        /// </summary>
        [HisFieldInfoMapping(0, 4)]
        public string FILLER { get; set; }

        /// <summary>
        /// AUS_CAT_NUM 9(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public short AUS_CAT_NUM { get; set; }

        /// <summary>
        /// AUS_SEDE 9(4)  
        /// </summary>
        [HisFieldInfoMapping(2, 4)]
        public short AUS_SEDE { get; set; }

        /// <summary>
        /// AUS_CERT 9(8)  
        /// </summary>
        [HisFieldInfoMapping(3, 8)]
        public int AUS_CERT { get; set; }

        /// <summary>
        /// AUS_CAT_ALF X(8)  
        /// </summary>
        [HisFieldInfoMapping(4, 8)]
        public string AUS_CAT_ALF { get; set; }

        /// <summary>
        /// AUS_DATA_ANNO 9(4)  
        /// </summary>
        [HisFieldInfoMapping(5, 4)]
        public short AUS_DATA_ANNO { get; set; }

        /// <summary>
        /// AUS_DATA_MESE 99  
        /// </summary>
        [HisFieldInfoMapping(6, 2)]
        public short AUS_DATA_MESE { get; set; }

        /// <summary>
        /// AUS_DEOR_ANNO 9(4)  
        /// </summary>
        [HisFieldInfoMapping(7, 4)]
        public short AUS_DEOR_ANNO { get; set; }

        /// <summary>
        /// AUS_DEOR_MESE 99  
        /// </summary>
        [HisFieldInfoMapping(8, 2)]
        public short AUS_DEOR_MESE { get; set; }

        /// <summary>
        /// AUS_DECA_ANNO 9(4)  
        /// </summary>
        [HisFieldInfoMapping(9, 4)]
        public short AUS_DECA_ANNO { get; set; }

        /// <summary>
        /// AUS_DECA_MESE 99  
        /// </summary>
        [HisFieldInfoMapping(10, 2)]
        public short AUS_DECA_MESE { get; set; }

        /// <summary>
        /// AUS_IMPORTO_CORR S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(11, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal AUS_IMPORTO_CORR { get; set; }

        /// <summary>
        /// AUS_ARRET_DISP S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(12, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal AUS_ARRET_DISP { get; set; }

        /// <summary>
        /// AUS_ARRET_INDISP S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(13, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal AUS_ARRET_INDISP { get; set; }

        [HisComplexAreaInfoMapping(14, ListCount = 20)]
        public List<DatiAnnuali> LISTADATIANNUALI { get; set; }

        /// <summary>
        /// FILLER X(29)  
        /// </summary>
        [HisFieldInfoMapping(15, 29)]
        public string FILLER1 { get; set; }

        /// <summary>
        /// AUS_CTR_TT 9(4)  
        /// </summary>
        [HisFieldInfoMapping(16, 4)]
        public short AUS_CTR_TT { get; set; }

        /// <summary>
        /// AUS_SES_TT X(1)  
        /// </summary>
        [HisFieldInfoMapping(17, 1)]
        public string AUS_SES_TT { get; set; }

        /// <summary>
        /// AUS_SES_DC X(1)  
        /// </summary>
        [HisFieldInfoMapping(18, 1)]
        public string AUS_SES_DC { get; set; }

        /// <summary>
        /// FILLER X(3)  
        /// </summary>
        [HisFieldInfoMapping(19, 3)]
        public string FILLER2 { get; set; }

        /// <summary>
        /// AUS_SEDE_CARICO X(22)  
        /// </summary>
        [HisFieldInfoMapping(20, 22)]
        public string AUS_SEDE_CARICO { get; set; }

        /// <summary>
        /// AUS_COGNOME X(32)  
        /// </summary>
        [HisFieldInfoMapping(21, 32)]
        public string AUS_COGNOME { get; set; }

        /// <summary>
        /// AUS_NOME X(32)  
        /// </summary>
        [HisFieldInfoMapping(22, 32)]
        public string AUS_NOME { get; set; }

        /// <summary>
        /// AUS_COGN_ACQ X(31)  
        /// </summary>
        [HisFieldInfoMapping(23, 31)]
        public string AUS_COGN_ACQ { get; set; }

        /// <summary>
        /// AUS_INDIRIZZO X(52)  
        /// </summary>
        [HisFieldInfoMapping(24, 52)]
        public string AUS_INDIRIZZO { get; set; }

        /// <summary>
        /// AUS_COMUNE_RES X(36)  
        /// </summary>
        [HisFieldInfoMapping(25, 36)]
        public string AUS_COMUNE_RES { get; set; }

        // 05  AUS-PROVINCIA-RES PIC X(3).
        /// <summary>
        /// AUS_CAP X(5)  
        /// </summary>
        [HisFieldInfoMapping(26, 5)]
        public string AUS_CAP { get; set; }

        /// <summary>
        /// AUS_DATAGG 99  
        /// </summary>
        [HisFieldInfoMapping(27, 2)]
        public short AUS_DATAGG { get; set; }

        /// <summary>
        /// AUS_DATAMM 99  
        /// </summary>
        [HisFieldInfoMapping(28, 2)]
        public short AUS_DATAMM { get; set; }

        /// <summary>
        /// AUS_DATAAAA 999  
        /// </summary>
        [HisFieldInfoMapping(29, 3)]
        public short AUS_DATAAAA { get; set; }

        /// <summary>
        /// AUS_COMUNE_NAS X(36)  
        /// </summary>
        [HisFieldInfoMapping(30, 36)]
        public string AUS_COMUNE_NAS { get; set; }

        /// <summary>
        /// FILLER X(83)  
        /// </summary>
        [HisFieldInfoMapping(31, 83)]
        public string FILLER3 { get; set; }

        /// <summary>
        /// FILLER X(240)  
        /// </summary>
        [HisFieldInfoMapping(32, 240)]
        public string FILLER4 { get; set; }
        #endregion Tracciato Host

        #region Properties
        public string TransactionName
        {
            get { return "Area Australia tradotta"; }
        }
        #endregion Properties

        #region nested class
        public class DatiAnnuali
        {
            #region Constructor
            internal DatiAnnuali()
            { }
            #endregion Constructor

            #region tracciato COBOL
            //          05  AUS-DATI-ANNUALI  OCCURS 20.                        
            //              10  AUS-ANNO      PIC 9(4).                         
            //              10  AUS-PAGATO    PIC 9(7)V9(4) COMP-3.             
            //              10  AUS-ADEGUATA  PIC 9(7)V9(4) COMP-3.             
            //              10  AUS-INTEGR    PIC 9(7)V9(4) COMP-3.             
            //              10  AUS-MINI      PIC 9(7)V9(4) COMP-3.             
            //              10  AUS-ASS-FIGLI PIC 9(7)V9(4) COMP-3.             
            //              10  AUS-ASS-CON   PIC 9(7)V9(4) COMP-3.             
            //              10  AUS-ART6      PIC 9(5)V9(4) COMP-3.             
            //              10  AUS-MAGG-S    PIC 9(5)V9(4) COMP-3. 
            #endregion tracciato COBOL

            #region Tracciato Host
            // 05  AUS-DATI-ANNUALI  OCCURS 20.
            /// <summary>
            /// AUS_ANNO 9(4)  
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public short AUS_ANNO { get; set; }

            /// <summary>
            /// AUS_PAGATO 9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(1, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal AUS_PAGATO { get; set; }

            /// <summary>
            /// AUS_ADEGUATA 9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(2, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal AUS_ADEGUATA { get; set; }

            /// <summary>
            /// AUS_INTEGR 9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(3, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal AUS_INTEGR { get; set; }

            /// <summary>
            /// AUS_MINI 9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(4, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal AUS_MINI { get; set; }

            /// <summary>
            /// AUS_ASS_FIGLI 9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(5, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal AUS_ASS_FIGLI { get; set; }
            /// <summary>
            /// AUS_ASS_CON 9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(6, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal AUS_ASS_CON { get; set; }

            /// <summary>
            /// AUS_ART6 9(5)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(7, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal AUS_ART6 { get; set; }

            /// <summary>
            /// AUS_MAGG_S 9(5)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(8, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal AUS_MAGG_S { get; set; }
            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
