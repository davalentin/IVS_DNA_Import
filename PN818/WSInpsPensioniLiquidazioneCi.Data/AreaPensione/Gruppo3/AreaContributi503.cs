using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaContributi503
    {
        #region tracciato COBOL
        //  04  CAMPI503.
        //****DATI RETRIBUTIVI PER CALCOLO CON DEC. DAL 01.01.1993******
        //         05  IW1STOBG             PIC 9(5)            COMP-3.
        //* N. SETTIMANE DI CONTRIBUZIONE OBG
        //         05  IW1RETOBG            PIC S9(7)V9(6)   COMP-3.
        //*EURO  RETRIBUZIONE MEDIA SETTIMANALE OBG
        //         05  IW1STCDM             PIC 9(5)            COMP-3.
        //* N. SETTIMANE DI CONTRIBUZIONE CDM
        //         05  IW1RETCDM            PIC S9(7)V9(6)   COMP-3.
        //*EURO  RETRIBUZIONE MEDIA SETTIMANALE CDM
        //         05  IW1START             PIC 9(5)            COMP-3.
        //* N. SETTIMANE DI CONTRIBUZIONE ART
        //         05  IW1RETART            PIC S9(7)V9(6)   COMP-3.
        //*EURO  RETRIBUZIONE MEDIA SETTIMANALE ART
        //         05  IW1STCOM             PIC 9(5)            COMP-3.
        //* N. SETTIMANE DI CONTRIBUZIONE COM
        //         05  IW1RETCOM            PIC S9(7)V9(6)   COMP-3.
        //*EURO  RETRIBUZIONE MEDIA SETTIMANALE COM
        //         05  IW1SETMIN            PIC 9(5)            COMP-3.
        //* N. SETTIMANE DI CONTRIBUZIONE MIN
        //         05  IW1RETMIN            PIC S9(7)V9(6)   COMP-3.
        //*EURO  RETRIBUZIONE MEDIA SETTIMANALE MIN
        //         05  ICI1VVOBG             PIC 9(5)            COMP-3.
        //* N. SETTIMANE VERS.VOL.OBG-503
        #endregion tracciato COBOL

        #region Tracciato Host
        // 04  CAMPI503.
        //****DATI RETRIBUTIVI PER CALCOLO CON DEC. DAL 01.01.1993******
        /// <summary>
        /// IW1STOBG 9(5) COMP-3 
        /// * N. SETTIMANE DI CONTRIBUZIONE OBG
        /// </summary>
        [HisFieldInfoMapping(0, 3, CobolType= CobolType.Comp3Unsigned)]
        public int IW1STOBG { get; set; }

        /// <summary>
        /// IW1RETOBG S9(7)V9(6) COMP-3 
        /// *EURO  RETRIBUZIONE MEDIA SETTIMANALE OBG
        /// </summary>
        [HisFieldInfoMapping(1, 7, Scale = 6, CobolType = CobolType.Comp3)]
        public decimal IW1RETOBG { get; set; }

        /// <summary>
        /// IW1STCDM 9(5) COMP-3 
        /// * N. SETTIMANE DI CONTRIBUZIONE CDM
        /// </summary>
        [HisFieldInfoMapping(2, 3, CobolType = CobolType.Comp3Unsigned)]
        public int IW1STCDM { get; set; }

        /// <summary>
        /// IW1RETCDM S9(7)V9(6) COMP-3 
        /// *EURO  RETRIBUZIONE MEDIA SETTIMANALE CDM
        /// </summary>
        [HisFieldInfoMapping(3, 7, Scale = 6, CobolType = CobolType.Comp3)]
        public decimal IW1RETCDM { get; set; }

        /// <summary>
        /// IW1START 9(5) COMP-3 
        /// * N. SETTIMANE DI CONTRIBUZIONE ART
        /// </summary>
        [HisFieldInfoMapping(4, 3, CobolType = CobolType.Comp3Unsigned)]
        public int IW1START { get; set; }

        /// <summary>
        /// IW1RETART S9(7)V9(6) COMP-3 
        /// *EURO  RETRIBUZIONE MEDIA SETTIMANALE ART
        /// </summary>
        [HisFieldInfoMapping(5, 7, Scale = 6, CobolType = CobolType.Comp3)]
        public decimal IW1RETART { get; set; }

        /// <summary>
        /// IW1STCOM 9(5) COMP-3 
        /// * N. SETTIMANE DI CONTRIBUZIONE COM
        /// </summary>
        [HisFieldInfoMapping(6, 3, CobolType = CobolType.Comp3Unsigned)]
        public int IW1STCOM { get; set; }

        /// <summary>
        /// IW1RETCOM S9(7)V9(6) COMP-3 
        /// *EURO  RETRIBUZIONE MEDIA SETTIMANALE COM
        /// </summary>
        [HisFieldInfoMapping(7, 7, Scale = 6, CobolType = CobolType.Comp3)]
        public decimal IW1RETCOM { get; set; }

        /// <summary>
        /// IW1SETMIN 9(5) COMP-3 
        /// * N. SETTIMANE DI CONTRIBUZIONE MIN
        /// </summary>
        [HisFieldInfoMapping(8, 3, CobolType = CobolType.Comp3Unsigned)]
        public int IW1SETMIN { get; set; }

        /// <summary>
        /// IW1RETMIN S9(7)V9(6) COMP-3 
        /// *EURO  RETRIBUZIONE MEDIA SETTIMANALE MIN
        /// </summary>
        [HisFieldInfoMapping(9, 7, Scale = 6, CobolType = CobolType.Comp3)]
        public decimal IW1RETMIN { get; set; }

        /// <summary>
        /// ICI1VVOBG 9(5) COMP-3 
        /// * N. SETTIMANE VERS.VOL.OBG-503
        /// </summary>
        [HisFieldInfoMapping(10, 3, CobolType = CobolType.Comp3Unsigned)]
        public int ICI1VVOBG { get; set; }

        #endregion Tracciato Host
    }
}
