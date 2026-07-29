using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaCampi2017
    {
        #region tracciato COBOL
        //     04 DATI-2017. 
        //*    AREA DEI REDDITI PER LAVORO AUTONOMO (700 bytes)
        //        05 AREA-WKAUT.
        //           10 IELWKAUT OCCURS 50.
        //              15 IWAUTDEC         PIC 9999.
        //*             ANNO RED.DA LAVORO AUTONOMO
        //              15 IWAUTRED         PIC S9(7)V9(4)   COMP-3.
        //*             EURO +RED. ANNUO IN MIGLIAIA
        //              15 IWAUTDAL-AL.
        //                 20 IWAUTDALM     PIC 99.
        //*                DAL MESE  DI LAVORO AUTONOMO
        //                 20 IWAUTALM      PIC 99.
        //*                AL MESE  DI LAVORO AUTONOMO

        //*    AREA DEI REDDITI PER 240   (500 Bytes)
        //        05 AREAW240.
        //           10 ELEMENTO240 OCCURS 50.
        //              15 I240DEC          PIC 9999.
        //*             ANNO DEL REDDITO PER 240
        //              15 I240RED          PIC S9(7)V9(4)   COMP-3.
        //*             EURO REDDITO PER 240

        //        05 DATI-DOMANDA.
        //           10 GP1DGRP             PIC X(4).
        //           10 GP1DPRD             PIC X(4).
        //           10 GP1DTIP             PIC X(4).           
        //           10 GP1DTIPOL           PIC X(4).
        //           10 GP1DFASE            PIC X(4).
        //        05 GP1FLAGS OCCURS 25
        //          10 GP1FLAG              PIC X.
        //        05 GP1ELIMP               PIC S9(7)V9(4) COMP-3.
        //        05 GP1CENTINT             PIC 9(4).    
        //        05 TP1PR                  PIC X(3).
        #endregion tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0)]
        public AreaWKAUT AreaWKAUT { get; set; }

        [HisComplexAreaInfoMapping(1, ListCount = 50)]
        public List<Reddito240> REDDITI240 { get; set; }

        /// <summary>
        /// GP1DGRP X(4)  
        /// </summary>
        [HisFieldInfoMapping(2, 4)]
        public string GP1DGRP { get; set; }

        /// <summary>
        /// GP1DPRD X(4)  
        /// </summary>
        [HisFieldInfoMapping(3, 4)]
        public string GP1DPRD { get; set; }

        /// <summary>
        /// GP1DTIP X(4)  
        /// </summary>
        [HisFieldInfoMapping(4, 4)]
        public string GP1DTIP { get; set; }

        /// <summary>
        /// GP1DTIPOL X(4)  
        /// </summary>
        [HisFieldInfoMapping(5, 4)]
        public string GP1DTIPOL { get; set; }

        /// <summary>
        /// GP1DFASE X(4)  
        /// </summary>
        [HisFieldInfoMapping(6, 4)]
        public string GP1DFASE { get; set; }

        /// <summary>
        ///  GP1FLAGS  OCCURS 25 
        /// </summary>
        [HisComplexAreaInfoMapping(7, ListCount = 25)]
        public List<GP1FLAG> T_GP1FLAG { get; set; }

        /// <summary>
        /// GP1ELIMP S9(7)V9(4) COMP-3.  
        /// </summary>
        [HisFieldInfoMapping(8, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal GP1ELIMP { get; set; }

        /// <summary>
        /// GP1CENTINT 9(4)  
        /// </summary>
        [HisFieldInfoMapping(9, 4)]
        public short GP1CENTINT { get; set; }

        /// <summary>
        /// TP1PR X(3)  
        /// </summary>
        [HisFieldInfoMapping(10, 3)]
        public string TP1PR { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class Reddito240
        {
            #region tracciato COBOL
            //04  CAMPI-2004.
            //******************************************************
            //*    AREA DEI REDDITI PER 240   (500 Bytes)
            //        05 AREAW240.
            //           10 ELEMENTO240 OCCURS 50.
            //              15 I240DEC          PIC 9999.
            //*             ANNO DEL REDDITO PER 240
            //              15 I240RED          PIC S9(7)V9(4)   COMP-3.
            //*             EURO REDDITO PER 240
            #endregion tracciato COBOL

            #region Tracciato Host
            // 04  CAMPI-2004.
            //******************************************************
            // *AREA 240 SPOSTATA TOTALE 180 LIBERI
            // 05  AREAW240.
            // *AREA DEI REDDITI PER 240
            // 10  ELEMENTO240     OCCURS 30.
            /// <summary>
            /// I240DEC 9999  
            /// *1999 ANNO DEL REDDITO PER 240
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public short I240DEC { get; set; }

            /// <summary>
            /// I240RED S9(7)V9(4) COMP-3 
            /// *EURO REDDITO PER 240
            /// </summary>
            [HisFieldInfoMapping(1, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal I240RED { get; set; }

            //*******************************************************
            #endregion Tracciato Host
        }

        public class GP1FLAG
        {
            #region tracciato COBOL
            //        05 GP1FLAGS OCCURS 25
            //          10 GP1FLAG              PIC X. 
            #endregion tracciato COBOL

            #region Tracciato Host
            [HisFieldInfoMapping(0, 1)]
            public string FLAG { get; set; }
            //*******************************************************
            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
