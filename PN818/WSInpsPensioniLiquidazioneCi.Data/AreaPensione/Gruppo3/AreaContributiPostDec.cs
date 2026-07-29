using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaContributiPostDec
    {
        #region tracciato COBOL
        //  04  CAMPI-SETT-POST-ORIG.
        //*CONTRIBUTI POST DEC ORIG
        //         05 DEC-RMS-IVS     OCCURS 2.
        //*DEC. CONTRIBUTI POST DEC ORIG
        //                15  IDECRICA         PIC 9999.
        //                15  IDECRICM         PIC 99.
        //            10  INSOBGRIC            PIC 9999.
        //*NUMERO CONTRIBUTI X CALC RETR. POST DEC ORIG
        //            10  INSVVRIC             PIC 9999.
        //*NUMERO CONTRIBUTI V.V. X CALC RETR. POST DEC ORIG
        //            10  IRMSRIC              PIC 9(7)V9(6) COMP-3.
        //*EURO RETR.MEDIA SETT. X CALC RETRIB. POST DEC ORIG
        //            10  INSIVSRIC            PIC 9999.
        //*NUMERO CONTRIBUTI X CALC CONTR. POST DEC ORIG
        //            10  IIVSRIC              PIC 9(5)V9(6) COMP-3.
        //*EURO IMPORTO IVS. X CALC CONTR. POST DEC ORIG
        #endregion tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 2)]
        public List<Contributo> CONTRIBUTI { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class Contributo
        {
            #region tracciato COBOL
            //  04  CAMPI-SETT-POST-ORIG.
            //*CONTRIBUTI POST DEC ORIG
            //         05 DEC-RMS-IVS     OCCURS 2.
            //*DEC. CONTRIBUTI POST DEC ORIG
            //                15  IDECRICA         PIC 9999.
            //                15  IDECRICM         PIC 99.
            //            10  INSOBGRIC            PIC 9999.
            //*NUMERO CONTRIBUTI X CALC RETR. POST DEC ORIG
            //            10  INSVVRIC             PIC 9999.
            //*NUMERO CONTRIBUTI V.V. X CALC RETR. POST DEC ORIG
            //            10  IRMSRIC              PIC 9(7)V9(6) COMP-3.
            //*EURO RETR.MEDIA SETT. X CALC RETRIB. POST DEC ORIG
            //            10  INSIVSRIC            PIC 9999.
            //*NUMERO CONTRIBUTI X CALC CONTR. POST DEC ORIG
            //            10  IIVSRIC              PIC 9(5)V9(6) COMP-3.
            //*EURO IMPORTO IVS. X CALC CONTR. POST DEC ORIG
            #endregion tracciato COBOL

            #region Tracciato Host
            // 04  CAMPI-SETT-POST-ORIG.
            // *CONTRIBUTI POST DEC ORIG
            // 05 DEC-RMS-IVS     OCCURS 2.
            /// <summary>
            /// IDECRICA 9999  
            /// *DEC. CONTRIBUTI POST DEC ORIG
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public short IDECRICA { get; set; }

            /// <summary>
            /// IDECRICM 99  
            /// *DEC. CONTRIBUTI POST DEC ORIG
            /// </summary>
            [HisFieldInfoMapping(1, 2)]
            public short IDECRICM { get; set; }

            /// <summary>
            /// INSOBGRIC 9999  
            /// *NUMERO CONTRIBUTI X CALC RETR. POST DEC ORIG
            /// </summary>
            [HisFieldInfoMapping(2, 4)]
            public short INSOBGRIC { get; set; }

            /// <summary>
            /// INSVVRIC 9999  
            /// *NUMERO CONTRIBUTI V.V. X CALC RETR. POST DEC ORIG
            /// </summary>
            [HisFieldInfoMapping(3, 4)]
            public short INSVVRIC { get; set; }

            /// <summary>
            /// IRMSRIC 9(7)V9(6) COMP-3 
            /// *EURO RETR.MEDIA SETT. X CALC RETRIB. POST DEC ORIG
            /// </summary>
            [HisFieldInfoMapping(4, 7, Scale = 6, CobolType = CobolType.Comp3Unsigned)]
            public decimal IRMSRIC { get; set; }

            /// <summary>
            /// INSIVSRIC 9999  
            /// *NUMERO CONTRIBUTI X CALC CONTR. POST DEC ORIG
            /// </summary>
            [HisFieldInfoMapping(5, 4)]
            public short INSIVSRIC { get; set; }

            /// <summary>
            /// IIVSRIC 9(5)V9(6) COMP-3 
            /// *EURO IMPORTO IVS. X CALC CONTR. POST DEC ORIG
            /// </summary>
            [HisFieldInfoMapping(6, 6, Scale = 6, CobolType = CobolType.Comp3Unsigned)]
            public decimal IIVSRIC { get; set; }


            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
