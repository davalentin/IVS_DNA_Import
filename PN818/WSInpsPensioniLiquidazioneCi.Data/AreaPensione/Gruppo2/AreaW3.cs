using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaW3
    {
        #region tracciato COBOL
        //   04  AREAW3.
        //     05 IELEMENTO-WK3 OCCURS 15 TIMES.
        //         10 IW3COGEST          PIC XX.
        //*+2007: 2 BYTES:  CODICE GEST. DEL SUPPL.
        //         10 IW3DEC-SUPPL.
        //             15 IW3DESUPA      PIC 9999.
        //             15 IW3DESUPM      PIC 99.
        //*+1999 DECORRENZA DEL SUPPL.
        //         10 IW3BASE            PIC 9(7)V9(6) COMP-3.
        //*EURO +IMP. BASE DEL SUPPL.
        //         10 IW3IVS             PIC 9(7)V9(6) COMP-3.
        //*EURO +1999 IMP.IVS DEL SUPPL. /RMS 72/90 (W3RETSET > 0)/ MONTANTE
        //         10 IW3IVSSOS          PIC 9(7)V9(6) COMP-3.
        //*EURO 1999 IMP.IVS SUP.SOST STATO /RMS ART 2/DPCM 161289/ TOT.CONT
        //         10 IW3RETSET          PIC 9(7)V9(6) COMP-3.
        //*EURO +R.M.S. SUPPL. RETR.
        //         10 IW3SETANZ          PIC 9(5) COMP-3.
        //*+SETT.ANZ. SUPPL.RETR.
        //         10 IW3TIPSUP          PIC X.
        //*SEGNAL. SUPPL.: '1'=NUOVA NORM.LG.503  ALTRIMENTI VECCHIA NORM.
        #endregion tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 15)]
        public List<Supplemento> SUPPLEMENTI { get; set; }
        #endregion Tracciato Host
        public class Supplemento
        {
            #region tracciato COBOL
            //   04  AREAW3.
            //     05 IELEMENTO-WK3 OCCURS 15 TIMES.
            //         10 IW3COGEST          PIC XX.
            //*+2007: 2 BYTES:  CODICE GEST. DEL SUPPL.
            //         10 IW3DEC-SUPPL.
            //             15 IW3DESUPA      PIC 9999.
            //             15 IW3DESUPM      PIC 99.
            //*+1999 DECORRENZA DEL SUPPL.
            //         10 IW3BASE            PIC 9(7)V9(6) COMP-3.
            //*EURO +IMP. BASE DEL SUPPL.
            //         10 IW3IVS             PIC 9(7)V9(6) COMP-3.
            //*EURO +1999 IMP.IVS DEL SUPPL. /RMS 72/90 (W3RETSET > 0)/ MONTANTE
            //         10 IW3IVSSOS          PIC 9(7)V9(6) COMP-3.
            //*EURO 1999 IMP.IVS SUP.SOST STATO /RMS ART 2/DPCM 161289/ TOT.CONT
            //         10 IW3RETSET          PIC 9(7)V9(6) COMP-3.
            //*EURO +R.M.S. SUPPL. RETR.
            //         10 IW3SETANZ          PIC 9(5) COMP-3.
            //*+SETT.ANZ. SUPPL.RETR.
            //         10 IW3TIPSUP          PIC X.
            //*SEGNAL. SUPPL.: '1'=NUOVA NORM.LG.503  ALTRIMENTI VECCHIA NORM.
            #endregion tracciato COBOL

            #region Tracciato Host
            // 04  AREAW3.
            // 05 IELEMENTO-WK3 OCCURS 15 TIMES.
            /// <summary>
            /// IW3COGEST XX  
            /// *+2007: 2 BYTES:  CODICE GEST. DEL SUPPL.
            /// </summary>
            [HisFieldInfoMapping(0, 2)]
            public string IW3COGEST { get; set; }

            // 10 IW3DEC-SUPPL.
            /// <summary>
            /// IW3DESUPA 9999  
            /// *+1999 DECORRENZA DEL SUPPL.
            /// </summary>
            [HisFieldInfoMapping(1, 4)]
            public short IW3DESUPA { get; set; }

            /// <summary>
            /// IW3DESUPM 99  
            /// *+1999 DECORRENZA DEL SUPPL.
            /// </summary>
            [HisFieldInfoMapping(2, 2)]
            public short IW3DESUPM { get; set; }

            /// <summary>
            /// IW3BASE 9(7)V9(6) COMP-3 
            // *EURO +IMP. BASE DEL SUPPL.
            /// </summary>
            [HisFieldInfoMapping(3, 7, Scale = 6, CobolType = CobolType.Comp3Unsigned)]
            public decimal IW3BASE { get; set; }

            /// <summary>
            /// IW3IVS 9(7)V9(6) COMP-3 
            /// *EURO +1999 IMP.IVS DEL SUPPL. /RMS 72/90 (W3RETSET > 0)/ MONTANTE
            /// </summary>
            [HisFieldInfoMapping(4, 7, Scale = 6, CobolType = CobolType.Comp3Unsigned)]
            public decimal IW3IVS { get; set; }

            /// <summary>
            /// IW3IVSSOS 9(7)V9(6) COMP-3 
            /// *EURO 1999 IMP.IVS SUP.SOST STATO /RMS ART 2/DPCM 161289/ TOT.CONT
            /// </summary>
            [HisFieldInfoMapping(5, 7, Scale = 6, CobolType = CobolType.Comp3Unsigned)]
            public decimal IW3IVSSOS { get; set; }

            /// <summary>
            /// IW3RETSET 9(7)V9(6) COMP-3 
            /// *EURO +R.M.S. SUPPL. RETR.
            /// </summary>
            [HisFieldInfoMapping(6, 7, Scale = 6, CobolType = CobolType.Comp3Unsigned)]
            public decimal IW3RETSET { get; set; }

            /// <summary>
            /// IW3SETANZ 9(5) COMP-3 
            /// *+SETT.ANZ. SUPPL.RETR.
            /// </summary>
            [HisFieldInfoMapping(7, 3, CobolType = CobolType.Comp3Unsigned)]
            public int IW3SETANZ { get; set; }

            /// <summary>
            /// IW3TIPSUP X  
            /// *SEGNAL. SUPPL.: '1'=NUOVA NORM.LG.503  ALTRIMENTI VECCHIA NORM.
            /// </summary>
            [HisFieldInfoMapping(8, 1)]
            public string IW3TIPSUP { get; set; }
            #endregion Tracciato Host
        }
    }
}
