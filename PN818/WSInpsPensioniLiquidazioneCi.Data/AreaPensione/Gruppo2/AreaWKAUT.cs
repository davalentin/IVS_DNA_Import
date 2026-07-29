using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaWKAUT
    {
        #region Constructor
        public AreaWKAUT()
        { }
        #endregion

        #region tracciato COBOL
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
        #endregion

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 50)]
        public List<RedditoLavoroAutonomo> REDDITILAVOROAUTONOMO { get; set; }
        #endregion

        #region nested class

        public class RedditoLavoroAutonomo
        {

            #region Constructor
            public RedditoLavoroAutonomo()
            { }
            #endregion

            #region tracciato COBOL

            //   04 AREA-WKAUT.
            //*2002 PORTATO A 15 L'OCCURS (NECESSARI 210 BYTE) (14*15)
            //         05  IELWKAUT OCCURS 15.
            //             10  IWAUTDEC             PIC 9999.
            //*1997+ANNO RED.DA LAVORO AUTONOMO
            //             10  IWAUTRED             PIC S9(7)V9(4)   COMP-3.
            //*EURO +RED. ANNUO IN MIGLIAIA
            //             10  IWAUTDAL-AL.
            //                 15  IWAUTDALM        PIC 99.
            //*1997+DAL MESE  DI LAVORO AUTONOMO
            //                 15  IWAUTALM         PIC 99.
            //*1997+AL MESE  DI LAVORO AUTONOMO
            #endregion

            #region Tracciato Host

            // 04 AREA-WKAUT.
            // *2002 PORTATO A 15 L'OCCURS (NECESSARI 210 BYTE) (14*15)
            // 05  IELWKAUT OCCURS 15.
            /// <summary>
            /// IWAUTDEC 9999  
            /// *1997+ANNO RED.DA LAVORO AUTONOMO
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public short IWAUTDEC { get; set; }

            /// <summary>
            /// IWAUTRED S9(7)V9(4) COMP-3 
            /// *EURO +RED. ANNUO IN MIGLIAIA
            /// </summary>
            [HisFieldInfoMapping(1, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal IWAUTRED { get; set; }

            // 10  IWAUTDAL-AL.
            /// <summary>
            /// IWAUTDALM 99  
            /// *1997+DAL MESE  DI LAVORO AUTONOMO
            /// </summary>
            [HisFieldInfoMapping(2, 2)]
            public short IWAUTDALM { get; set; }

            /// <summary>
            /// IWAUTALM 99  
            /// *1997+AL MESE  DI LAVORO AUTONOMO
            /// </summary>
            [HisFieldInfoMapping(3, 2)]
            public short IWAUTALM { get; set; }


            #endregion
        }
        #endregion
    }
}
