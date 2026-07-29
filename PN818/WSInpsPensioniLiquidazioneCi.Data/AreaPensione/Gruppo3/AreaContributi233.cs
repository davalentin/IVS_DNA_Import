using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaContributi233
    {
        #region tracciato COBOL
        //  04 CAMPI233.
        //*CONTRIBUTI ITALIANI LG.233/90
        //        05  IW1SAOBG             PIC 9(5)       COMP-3.
        //* N. SETTIMANE DI CONTRIBUZIONE OBG
        //        05  IW1RMSOBG            PIC S9(7)V9(6)   COMP-3.
        //*EURO  RETRIBUZIONE MEDIA SETTIMANALE OBG
        //        05  TP1IVSOBG            PIC S9(3)V9(6)  COMP-3.
        //*EURO  IVS OBG
        //        05  IW1SACDM             PIC 9(5)       COMP-3.
        //* N. SETTIMANE DI CONTRIBUZIONE CD. CM.
        //        05  IW1RMSCDM            PIC S9(7)V9(6)   COMP-3.
        //*EURO  RETRIBUZIONE MEDIA SETTIMANALE CD. CM.
        //        05  TP1IVSCDM            PIC S9(3)V9(6)  COMP-3.
        //*EURO  IVS CD CM
        //        05  IW1SAART             PIC 9(5)       COMP-3.
        //* N. SETTIMANE DI CONTRIBUZIONE ART
        //        05  IW1RMSART            PIC S9(7)V9(6)   COMP-3.
        //*EURO  RETRIBUZIONE MEDIA SETTIMANALE ART
        //        05  TP1IVSART            PIC S9(3)V9(6)  COMP-3.
        //*EURO  IVS ART
        //        05  IW1SACOM             PIC 9(5)       COMP-3.
        //* N. SETTIMANE DI CONTRIBUZIONE COM
        //        05  IW1RMSCOM            PIC S9(7)V9(6)   COMP-3.
        //*EURO  RETRIBUZIONE MEDIA SETTIMANALE COM
        //        05  TP1IVSCOM            PIC S9(3)V9(4)  COMP-3.
        //*EURO  IVS COM
        #endregion tracciato COBOL

        #region Tracciato Host
        // 04 CAMPI233.
        // *CONTRIBUTI ITALIANI LG.233/90
        /// <summary>
        /// IW1SAOBG 9(5) COMP-3 
        /// * N. SETTIMANE DI CONTRIBUZIONE OBG
        /// </summary>
        [HisFieldInfoMapping(0, 3, CobolType = CobolType.Comp3Unsigned)]
        public int IW1SAOBG { get; set; }

        /// <summary>
        /// IW1RMSOBG S9(7)V9(6) COMP-3 
        /// *EURO  RETRIBUZIONE MEDIA SETTIMANALE OBG
        /// </summary>
        [HisFieldInfoMapping(1, 7, Scale = 6, CobolType = CobolType.Comp3)]
        public decimal IW1RMSOBG { get; set; }

        /// <summary>
        /// TP1IVSOBG S9(3)V9(6) COMP-3 
        /// *EURO  IVS OBG
        /// </summary>
        [HisFieldInfoMapping(2, 5, Scale = 6, CobolType = CobolType.Comp3)]
        public decimal TP1IVSOBG { get; set; }

        /// <summary>
        /// IW1SACDM 9(5) COMP-3 
        /// * N. SETTIMANE DI CONTRIBUZIONE CD. CM.
        /// </summary>
        [HisFieldInfoMapping(3, 3, CobolType = CobolType.Comp3Unsigned)]
        public int IW1SACDM { get; set; }

        /// <summary>
        /// IW1RMSCDM S9(7)V9(6) COMP-3 
        /// *EURO  RETRIBUZIONE MEDIA SETTIMANALE CD. CM.
        /// </summary>
        [HisFieldInfoMapping(4, 7, Scale = 6, CobolType = CobolType.Comp3)]
        public decimal IW1RMSCDM { get; set; }

        /// <summary>
        /// TP1IVSCDM S9(3)V9(6) COMP-3 
        /// *EURO  IVS CD CM
        /// </summary>
        [HisFieldInfoMapping(5, 5, Scale = 6, CobolType = CobolType.Comp3)]
        public decimal TP1IVSCDM { get; set; }

        /// <summary>
        /// IW1SAART 9(5) COMP-3 
        /// * N. SETTIMANE DI CONTRIBUZIONE ART
        /// </summary>
        [HisFieldInfoMapping(6, 3, CobolType = CobolType.Comp3Unsigned)]
        public int IW1SAART { get; set; }

        /// <summary>
        /// IW1RMSART S9(7)V9(6) COMP-3 
        /// *EURO  RETRIBUZIONE MEDIA SETTIMANALE ART
        /// </summary>
        [HisFieldInfoMapping(7, 7, Scale = 6, CobolType = CobolType.Comp3)]
        public decimal IW1RMSART { get; set; }

        /// <summary>
        /// TP1IVSART S9(3)V9(6) COMP-3 
        /// *EURO  IVS ART
        /// </summary>
        [HisFieldInfoMapping(8, 5, Scale = 6, CobolType = CobolType.Comp3)]
        public decimal TP1IVSART { get; set; }

        /// <summary>
        /// IW1SACOM 9(5) COMP-3 
        /// * N. SETTIMANE DI CONTRIBUZIONE COM
        /// </summary>
        [HisFieldInfoMapping(9, 3, CobolType = CobolType.Comp3Unsigned)]
        public int IW1SACOM { get; set; }

        /// <summary>
        /// IW1RMSCOM S9(7)V9(6) COMP-3 
        /// *EURO  RETRIBUZIONE MEDIA SETTIMANALE COM
        /// </summary>
        [HisFieldInfoMapping(10, 7, Scale = 6, CobolType = CobolType.Comp3)]
        public decimal IW1RMSCOM { get; set; }

        /// <summary>
        /// TP1IVSCOM S9(3)V9(4) COMP-3 
        /// *EURO  IVS COM
        /// </summary>
        [HisFieldInfoMapping(11, 4, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal TP1IVSCOM { get; set; }


        #endregion Tracciato Host
    }
}
