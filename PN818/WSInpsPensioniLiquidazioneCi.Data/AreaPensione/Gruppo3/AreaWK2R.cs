using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaWK2R
    {
        #region tracciato COBOL
        //             04  IARWK2R.
        //     05  IABTQFI    PIC S9(5)V9(4) COMP-3.
        //*EURO +IMPORTO TOTALE QUOTE FISSE A DEC. CALCOLO
        //*QUESTO CAMPO FORSE NON SERVE
        //                15  ICI2DECSEC        PIC 99.
        //                15  ICI2DECAA     PIC 99.
        //             10  ICI2DECAM     PIC 99.
        //     05  ICI2QUOTEST                PIC S9(7)V9(4) COMP-3.
        //*EURO +IMPORTO QUOTA ESTERA
        //     05  ICI2COEF                PIC S9V9(5) COMP-3.
        //* COEFFICENTE DI RIDUZIONE
        //     05  IABBATOPC    PIC S9(4)V9(7) COMP-3.
        //*EURO +IMPORTO BASE
        //     05  IABMAIM      PIC S9(7)V9(4) COMP-3.
        //*EURO +IMPORTO TOTALE LORDO PENSIONE IN PAGAM.
        //     05  IABML1Q      PIC S9(7)V9(4) COMP-3.
        //*EURO IMPORTO PENS. DEL DANTE CAUSA IN PAGAM. AL NETTO DI MAGG.544
        //**DEVE ESSERE ACQUISITO CON SO DI PENSIONATO CON 1 IN IW1C495
        //** SARA' PORTATO IN ABML1Q(150) AL NETTO ANCHE DI ART.6/140
        //     05  IABML2Q      PIC S9(7)V9(4) COMP-3.
        //*EURO +IMPORTO TOTALE LORDO PENSIONE IN PAGAM.II°QUOTA
        //     05  IABMCP       PIC S9(7)V9(4) COMP-3.
        //*EURO +IMPORTO ADEGUATA ARR. ALLA LIRA
        //     05  IABIITM      PIC S9(5)V9(4) COMP-3.
        //*EURO +IMPORTO INTEGRAZIONE
        //     05  IABTQMAG     PIC S9(5)V9(4) COMP-3.
        //*EURO +IMPORTO TOTALE AF/AF+MAG/ANF
        //     05  IABMMS1      PIC 9(5)V9(4) COMP-3.
        //*EURO +IMPORTO ART.1/140 E 544 (MAGG.SOC)
        //     05  IABMM345     PIC 9(5)V9(4) COMP-3.
        //*EURO +IMPORTO ART.3.4.5/140 E '8' DPCM
        //     05  IABMMEX6     PIC 9(3)V9(4) COMP-3.
        //*EURO +IMPORTO ART.6/140
        //     05  IABADE1X     PIC S9(7)V9(4) COMP-3.
        //*EURO +IMPORTO PRIMO FONDO RIDEFINITO PERCHE' NON SERVE
        //*EURO +IMPORTO VUOTO1 NO
        //     05  IABADE2X     PIC S9(7)V9(4) COMP-3.
        //*EURO +IMPORTO VUOTO2 NO
        //     05  IABADE3X     PIC S9(7)V9(4) COMP-3.
        //*EURO +IMPORTO VUOTO3 NO
        //     05  IABADE4X     PIC S9(7)V9(4) COMP-3.
        //*EURO +IMPORTO VUOTO4 NO
        //     05  IABADE5X     PIC S9(7)V9(4) COMP-3.
        //*EURO +IMPORTO ADEGUATA DEL D.C. ALLA DEC.ORIG
        //     05  IABQOBGM        PIC S9(7)V9(4)  COMP-3.
        //*EURO  QUOTA OBG MINATORI
        //     05  IABAUDPCM          PIC S9(5)V9(4)  COMP-3.
        //*EURO  IMPORTO AUM. ART.2 DPCM 16/12/1989
        //     05  IABMM409             PIC S9(5)V9(4)  COMP-3.
        //*EURO  IMPORTO AUM. ART.1 L 409 /90 PENSIONE D'ANNATA
        #endregion tracciato COBOL

        #region Tracciato Host
        // 04  IARWK2R.
        /// <summary>
        /// IABTQFI S9(5)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IABTQFI { get; set; }

        // *EURO +IMPORTO TOTALE QUOTE FISSE A DEC. CALCOLO
        // *QUESTO CAMPO FORSE NON SERVE
        /// <summary>
        /// ICI2DECSEC 99  
        /// </summary>
        [HisFieldInfoMapping(1, 2)]
        public short ICI2DECSEC { get; set; }

        /// <summary>
        /// ICI2DECAA 99  
        /// </summary>
        [HisFieldInfoMapping(2, 2)]
        public short ICI2DECAA { get; set; }

        /// <summary>
        /// ICI2DECAM 99  
        /// </summary>
        [HisFieldInfoMapping(3, 2)]
        public short ICI2DECAM { get; set; }

        /// <summary>
        /// ICI2QUOTEST S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(4, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICI2QUOTEST { get; set; }

        // *EURO +IMPORTO QUOTA ESTERA
        /// <summary>
        /// ICI2COEF S9V9(5) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(5, 4, Scale = 5, CobolType = CobolType.Comp3)]
        public decimal ICI2COEF { get; set; }

        // * COEFFICENTE DI RIDUZIONE
        /// <summary>
        /// IABBATOPC S9(4)V9(7) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(6, 6, Scale = 7, CobolType = CobolType.Comp3)]
        public decimal IABBATOPC { get; set; }

        // *EURO +IMPORTO BASE
        /// <summary>
        /// IABMAIM S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(7, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IABMAIM { get; set; }

        // *EURO +IMPORTO TOTALE LORDO PENSIONE IN PAGAM.
        /// <summary>
        /// IABML1Q S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(8, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IABML1Q { get; set; }

        // *EURO IMPORTO PENS. DEL DANTE CAUSA IN PAGAM. AL NETTO DI MAGG.544
        //**DEVE ESSERE ACQUISITO CON SO DI PENSIONATO CON 1 IN IW1C495
        //** SARA' PORTATO IN ABML1Q(150) AL NETTO ANCHE DI ART.6/140
        /// <summary>
        /// IABML2Q S9(7)V9(4) COMP-3 
        /// *EURO +IMPORTO TOTALE LORDO PENSIONE IN PAGAM.II°QUOTA
        /// </summary>
        [HisFieldInfoMapping(9, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IABML2Q { get; set; }

        /// <summary>
        /// IABMCP S9(7)V9(4) COMP-3 
        /// *EURO +IMPORTO ADEGUATA ARR. ALLA LIRA
        /// </summary>
        [HisFieldInfoMapping(10, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IABMCP { get; set; }

        /// <summary>
        /// IABIITM S9(5)V9(4) COMP-3 
        /// *EURO +IMPORTO INTEGRAZIONE
        /// </summary>
        [HisFieldInfoMapping(11, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IABIITM { get; set; }

        /// <summary>
        /// IABTQMAG S9(5)V9(4) COMP-3 
        /// *EURO +IMPORTO TOTALE AF/AF+MAG/ANF
        /// </summary>
        [HisFieldInfoMapping(12, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IABTQMAG { get; set; }

        /// <summary>
        /// IABMMS1 9(5)V9(4) COMP-3 
        /// *EURO +IMPORTO ART.1/140 E 544 (MAGG.SOC)
        /// </summary>
        [HisFieldInfoMapping(13, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal IABMMS1 { get; set; }

        /// <summary>
        /// IABMM345 9(5)V9(4) COMP-3 
        /// *EURO +IMPORTO ART.3.4.5/140 E '8' DPCM
        /// </summary>
        [HisFieldInfoMapping(14, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal IABMM345 { get; set; }

        /// <summary>
        /// IABMMEX6 9(3)V9(4) COMP-3 
        /// *EURO +IMPORTO ART.6/140
        /// </summary>
        [HisFieldInfoMapping(15, 4, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal IABMMEX6 { get; set; }

        /// <summary>
        /// IABADE1X S9(7)V9(4) COMP-3 
        /// *EURO +IMPORTO PRIMO FONDO RIDEFINITO PERCHE' NON SERVE
        /// *EURO +IMPORTO VUOTO1 NO
        /// </summary>
        [HisFieldInfoMapping(16, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IABADE1X { get; set; }

        /// <summary>
        /// IABADE2X S9(7)V9(4) COMP-3 
        /// *EURO +IMPORTO VUOTO2 NO
        /// </summary>
        [HisFieldInfoMapping(17, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IABADE2X { get; set; }

        /// <summary>
        /// IABADE3X S9(7)V9(4) COMP-3 
        /// *EURO +IMPORTO VUOTO3 NO
        /// </summary>
        [HisFieldInfoMapping(18, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IABADE3X { get; set; }

        /// <summary>
        /// IABADE4X S9(7)V9(4) COMP-3 
        /// *EURO +IMPORTO VUOTO4 NO
        /// </summary>
        [HisFieldInfoMapping(19, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IABADE4X { get; set; }

        /// <summary>
        /// IABADE5X  S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(20, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IABADE5X { get; set; }

        /// <summary>
        /// IABQOBGM S9(7)V9(4) COMP-3 
        /// *EURO  QUOTA OBG MINATORI
        /// </summary>
        [HisFieldInfoMapping(21, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IABQOBGM { get; set; }

        /// <summary>
        /// IABAUDPCM S9(5)V9(4) COMP-3 
        /// *EURO  IMPORTO AUM. ART.2 DPCM 16/12/1989
        /// </summary>
        [HisFieldInfoMapping(22, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IABAUDPCM { get; set; }

        /// <summary>
        /// IABMM409 S9(5)V9(4) COMP-3 
        /// *EURO  IMPORTO AUM. ART.1 L 409 /90 PENSIONE D'ANNATA
        /// </summary>
        [HisFieldInfoMapping(23, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IABMM409 { get; set; }


        #endregion Tracciato Host
    }
}
