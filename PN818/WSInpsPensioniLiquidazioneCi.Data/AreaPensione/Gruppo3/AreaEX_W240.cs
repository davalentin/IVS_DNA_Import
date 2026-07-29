using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaEX_W240
    {
        #region tracciato COBOL

        //******************************************************
        //*AREA 240 SPOSTATA                TOTALE 180 LIBERI DAL 2005
        //     04  EX-AREAW240.
        //         05  IREQSENT1             PIC X.
        //*REQUISITO IN BASE A SENTENZE 1=DIRITTO INTEGR. CON -52/260/520
        //*                                                                 
        //*26/05/2008 VITTIME TERRORISMO 09/2004 TOTALE 128 BYTE            
        //         05 VITTIME-TERRORISMO.                                   
        //            10 VT-TIPO-PRES           PIC 9.                      
        //*GP1AC01 1 BITE: TIPO PRESTAZIONE 1/2/3                           
        //            10 VT-COD-EVEN            PIC X.                      
        //*GP1AC01 2 BITE: CODICE BENEFICIO I/E                             
        //            10 VT-TIPO-BEN            PIC X.                      
        //*GP1AC01 3 BITE: TIPO -BENEFICIO  1/2/3/4/5/6/7                   
        //*GP1AP35: DATA EVENTO TERRORISTICO                 
        //                 15 VT-DATA-EVA       PIC 9(4).                   
        //                 15 VT-DATA-EVM       PIC 9(2).                   
        //                 15 VT-DATA-EVG       PIC 9(2).                   
        //            10 VT-BENEFICIARIO        PIC X.                       
        //*GP1AC02: CODICE BENEFICIARIO                                  
        //            10 VT-DATI-CONTRIB.                                
        //*DATI CONTRIB/RETRIB. PER ART 2-3                              
        //                15 VT-ELEM OCCURS 3.                          
        //*GP2BB04Z/GP2BC01Z DECORRENZA BENEFICIO 09/2004 01/2007          
        //                      25 VT-DEC-EVA        PIC 9(4).           
        //                      25 VT-DEC-EVM        PIC 9(2).          
        //*GP2BB05N/GP2BC09 CODICE GESTIONE          
        //                      25 VT-COGEST1        PIC X.              
        //*CODICE GESTIONE 1=OBG  2=CD  3=ART  4=COM                     
        //                      25 VT-COGEST2        PIC X.              
        //*W=DAL 09/2004  Y=DAL 01/2007                                  
        //                   20 VT-CODLIQ            PIC X.                 
        //*GP2BC0A CODICE LIQUIDAZIONE RETRIBUTIVA: 1=QUOTA A  2=QUOTA B    
        //                   20 VT-MONT-RMS          PIC 9(7)V9(4).         
        //*GP2BB06E/GP2BC3E MONTANTE CONTRIBUTI / RMS RID.                  
        //                   20 VT-IMPCONTR-IVS      PIC 9(7)V9(4).         
        //*GP2BB07E/ IMPORTO CONTRIBUTI / IMPORTO IVS RID.                  
        //                   20 VT-NUM-SET           PIC 9(4).              
        //*GP2BB08/GP2BC02 NUMERO SETTIMANE RIDETERMINATO                   

        //*25 BYTE X 3 = 75                                                 
        //            10 IABTERROR                PIC 9(5)V9(6).            
        //*IMP. A DEC. CALCOLO VITT. TERR.(PER RINNOVO TIPO 2 0 1)          
        //*TOTALE BYTES PER VITTIME DEL TERRORISMO: 12+105+11 = 128         
        //*******************************************************           
        //******   05  FILLER                     PIC X(51). 

        //         05  IGP1AJ11                   PIC X.                   
        //*14.08.2008: SOGGETTO DEROGATO: ESODATI E MOBILITA' 1/2          

        //         05  FILLER                     PIC X(50).  
        #endregion tracciato COBOL

        #region Tracciato Host
        //******************************************************
        // *AREA 240 SPOSTATA                TOTALE 180 LIBERI DAL 2005
        // 04  EX-AREAW240.
        /// <summary>
        /// IREQSENT1 X  
        /// *REQUISITO IN BASE A SENTENZE 1=DIRITTO INTEGR. CON -52/260/520
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string IREQSENT1 { get; set; }

        // *26/05/2008 VITTIME TERRORISMO 09/2004 TOTALE 128 BYTE
        // 05 VITTIME-TERRORISMO.
        /// <summary>
        /// VT_TIPO_PRES 9  
        /// *GP1AC01 1 BITE: TIPO PRESTAZIONE 1/2/3
        /// </summary>
        [HisFieldInfoMapping(1, 1)]
        public short VT_TIPO_PRES { get; set; }

        /// <summary>
        /// VT_COD_EVEN X  
        /// *GP1AC01 2 BITE: CODICE BENEFICIO I/E
        /// </summary>
        [HisFieldInfoMapping(2, 1)]
        public string VT_COD_EVEN { get; set; }

        /// <summary>
        /// VT_TIPO_BEN X  
        /// *GP1AC01 3 BITE: TIPO -BENEFICIO  1/2/3/4/5/6/7
        /// </summary>
        [HisFieldInfoMapping(3, 1)]
        public string VT_TIPO_BEN { get; set; }

        /// <summary>
        /// VT_DATA_EVA 9(4)  
        /// *GP1AP35: DATA EVENTO TERRORISTICO
        /// </summary>
        [HisFieldInfoMapping(4, 4)]
        public short VT_DATA_EVA { get; set; }

        /// <summary>
        /// VT_DATA_EVM 9(2)  
        /// *GP1AP35: DATA EVENTO TERRORISTICO
        /// </summary>
        [HisFieldInfoMapping(5, 2)]
        public short VT_DATA_EVM { get; set; }

        /// <summary>
        /// VT_DATA_EVG 9(2)  
        /// *GP1AP35: DATA EVENTO TERRORISTICO
        /// </summary>
        [HisFieldInfoMapping(6, 2)]
        public short VT_DATA_EVG { get; set; }

        /// <summary>
        /// VT_BENEFICIARIO X  
        /// *GP1AC02: CODICE BENEFICIARIO
        /// </summary>
        [HisFieldInfoMapping(7, 1)]
        public string VT_BENEFICIARIO { get; set; }

        [HisComplexAreaInfoMapping(8, ListCount = 3)]
        public List<DatiArt2_3> DATIART2_3 { get; set; }

        /// <summary>
        /// IABTERROR 9(5)V9(6)  
        /// *IMP. A DEC. CALCOLO VITT. TERR.(PER RINNOVO TIPO 2 0 1)
        /// </summary>
        [HisFieldInfoMapping(9, 11, Scale = 6)]
        public decimal IABTERROR { get; set; }
        // *TOTALE BYTES PER VITTIME DEL TERRORISMO: 12+105+11 = 128
        //*******************************************************
        /// <summary>
        /// IGP1AJ11 X  
        /// *14.08.2008: SOGGETTO DEROGATO: ESODATI E MOBILITA' 1/2
        /// </summary>
        [HisFieldInfoMapping(10, 1)]
        public string IGP1AJ11 { get; set; }

        /// <summary>
        /// FILLER X(50)  
        /// </summary>
        [HisFieldInfoMapping(11, 50)]
        public string FILLER { get; set; }

        #endregion Tracciato Host

        #region nested class

        public class DatiArt2_3
        {
            #region tracciato COBOL
            //      10 VT-DATI-CONTRIB.                                
            //*DATI CONTRIB/RETRIB. PER ART 2-3                              
            //                15 VT-ELEM OCCURS 3.                          
            //*GP2BB04Z/GP2BC01Z DECORRENZA BENEFICIO 09/2004 01/2007          
            //                      25 VT-DEC-EVA        PIC 9(4).           
            //                      25 VT-DEC-EVM        PIC 9(2).          
            //*GP2BB05N/GP2BC09 CODICE GESTIONE          
            //                      25 VT-COGEST1        PIC X.              
            //*CODICE GESTIONE 1=OBG  2=CD  3=ART  4=COM                     
            //                      25 VT-COGEST2        PIC X.              
            //*W=DAL 09/2004  Y=DAL 01/2007                                  
            //                   20 VT-CODLIQ            PIC X.                 
            //*GP2BC0A CODICE LIQUIDAZIONE RETRIBUTIVA: 1=QUOTA A  2=QUOTA B    
            //                   20 VT-MONT-RMS          PIC 9(7)V9(4).         
            //*GP2BB06E/GP2BC3E MONTANTE CONTRIBUTI / RMS RID.                  
            //                   20 VT-IMPCONTR-IVS      PIC 9(7)V9(4).         
            //*GP2BB07E/ IMPORTO CONTRIBUTI / IMPORTO IVS RID.                  
            //                   20 VT-NUM-SET           PIC 9(4).              
            //*GP2BB08/GP2BC02 NUMERO SETTIMANE RIDETERMINATO                   

            //*25 BYTE X 3 = 75  
            #endregion tracciato COBOL

            #region Tracciato Host
            // 10 VT-DATI-CONTRIB.
            // *DATI CONTRIB/RETRIB. PER ART 2-3
            // 15 VT-ELEM OCCURS 3.
            /// <summary>
            /// VT_DEC_EVA 9(4)  
            /// *GP2BB04Z/GP2BC01Z DECORRENZA BENEFICIO 09/2004 01/2007
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public short VT_DEC_EVA { get; set; }

            /// <summary>
            /// VT_DEC_EVM 9(2)  
            /// 
            /// *GP2BB04Z/GP2BC01Z DECORRENZA BENEFICIO 09/2004 01/2007
            /// </summary>
            [HisFieldInfoMapping(1, 2)]
            public short VT_DEC_EVM { get; set; }

            /// <summary>
            /// VT_COGEST1 X  
            /// *CODICE GESTIONE 1=OBG  2=CD  3=ART  4=COM
            /// </summary>
            [HisFieldInfoMapping(2, 1)]
            public string VT_COGEST1 { get; set; }

            /// <summary>
            /// VT_COGEST2 X  
            /// *W=DAL 09/2004  Y=DAL 01/2007
            /// </summary>
            [HisFieldInfoMapping(3, 1)]
            public string VT_COGEST2 { get; set; }

            /// <summary>
            /// VT_CODLIQ X  
            /// *GP2BC0A CODICE LIQUIDAZIONE RETRIBUTIVA: 1=QUOTA A  2=QUOTA B
            /// </summary>
            [HisFieldInfoMapping(4, 1)]
            public string VT_CODLIQ { get; set; }

            /// <summary>
            /// VT_MONT_RMS 9(7)V9(4)  
            /// *GP2BB06E/GP2BC3E MONTANTE CONTRIBUTI / RMS RID.
            /// </summary>
            [HisFieldInfoMapping(5, 11, Scale = 4)]
            public decimal VT_MONT_RMS { get; set; }

            /// <summary>
            /// VT_IMPCONTR_IVS 9(7)V9(4)  
            /// *GP2BB07E/ IMPORTO CONTRIBUTI / IMPORTO IVS RID.
            /// </summary>
            [HisFieldInfoMapping(6, 11, Scale = 4)]
            public decimal VT_IMPCONTR_IVS { get; set; }

            /// <summary>
            /// VT_NUM_SET 9(4)  
            /// *GP2BB08/GP2BC02 NUMERO SETTIMANE RIDETERMINATO
            /// </summary>
            [HisFieldInfoMapping(7, 4)]
            public short VT_NUM_SET { get; set; }

            // *25 BYTE X 3 = 75

            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
