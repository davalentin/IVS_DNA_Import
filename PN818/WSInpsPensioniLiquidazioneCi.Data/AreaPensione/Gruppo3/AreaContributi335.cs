using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaContributi335
    {
        #region tracciato COBOL
        //   04  CAMPI335.
        //**1996**DATI RETRIBUTIVI PER CALCOLO CON DEC. DAL 01.02.1996**
        //         10  ICISTOBG335          PIC 9(5)     COMP-3.
        //* N. SETTIMANE DI CONTRIBUZIONE OBG
        //         10  ICICONOBG335         PIC S9(9)V9(4)   COMP-3.
        //*EURO AMMONTARE DEI CONTRIBUTI OBG
        //         10  ICIRETOBG335         PIC S9(9)V9(4)   COMP-3.
        //*EURO  MONTANTE OBG
        //         10  ICISTCDM335          PIC 9(5)     COMP-3.
        //* N. SETTIMANE DI CONTRIBUZIONE CDM
        //         10  ICICONCDM335         PIC S9(9)V9(4)   COMP-3.
        //*EURO AMMONTARE DEI CONTRIBUTI CDM
        //         10  ICIRETCDM335         PIC S9(9)V9(4)   COMP-3.
        //*EURO  MONTANTE CDM
        //         10  ICISTART335          PIC 9(5)     COMP-3.
        //* N. SETTIMANE DI CONTRIBUZIONE ART
        //         10  ICICONART335         PIC S9(9)V9(4)   COMP-3.
        //*EURO AMMONTARE DEI CONTRIBUTI ART
        //         10  ICIRETART335         PIC S9(9)V9(4)   COMP-3.
        //*EURO  MONTANTE ART
        //         10  ICISTCOM335          PIC 9(5)     COMP-3.
        //* N. SETTIMANE DI CONTRIBUZIONE COM
        //         10  ICICONCOM335         PIC S9(9)V9(4)   COMP-3.
        //*EURO AMMONTARE DEI CONTRIBUTI COM
        //         10  ICIRETCOM335         PIC S9(9)V9(4)   COMP-3.
        //*EURO  MONTANTE COM
        //         10  ICIMMF               PIC S9(9)V9(4)   COMP-3.
        //*EURO  MONTANTE MEDIO FITTIZIE 335
        //         10  ICISET1X100          PIC 9(4).
        //* N. SETTIMANE CON DIRITTO AUMENTO DEL 1%
        //         10  ICISET05X100         PIC 9(4).
        ////* N. SETTIMANE CON DIRITTO AUMENTO DEL 0,5%
        #endregion tracciato COBOL

        #region Tracciato Host
        // 04  CAMPI335.
        //**1996**DATI RETRIBUTIVI PER CALCOLO CON DEC. DAL 01.02.1996**
        /// <summary>
        /// ICISTOBG335 9(5) COMP-3 
        /// * N. SETTIMANE DI CONTRIBUZIONE OBG
        /// </summary>
        [HisFieldInfoMapping(0, 3, CobolType = CobolType.Comp3Unsigned)]
        public int ICISTOBG335 { get; set; }

        /// <summary>
        /// ICICONOBG335 S9(9)V9(4) COMP-3 
        /// *EURO AMMONTARE DEI CONTRIBUTI OBG
        /// </summary>
        [HisFieldInfoMapping(1, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICICONOBG335 { get; set; }

        /// <summary>
        /// ICIRETOBG335 S9(9)V9(4) COMP-3 
        /// *EURO  MONTANTE OBG
        /// </summary>
        [HisFieldInfoMapping(2, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICIRETOBG335 { get; set; }

        /// <summary>
        /// ICISTCDM335 9(5) COMP-3 
        /// * N. SETTIMANE DI CONTRIBUZIONE CDM
        /// </summary>
        [HisFieldInfoMapping(3, 3, CobolType = CobolType.Comp3Unsigned)]
        public int ICISTCDM335 { get; set; }

        /// <summary>
        /// ICICONCDM335 S9(9)V9(4) COMP-3 
        /// *EURO AMMONTARE DEI CONTRIBUTI CDM
        /// </summary>
        [HisFieldInfoMapping(4, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICICONCDM335 { get; set; }

        /// <summary>
        /// ICIRETCDM335 S9(9)V9(4) COMP-3 
        /// *EURO  MONTANTE CDM
        /// </summary>
        [HisFieldInfoMapping(5, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICIRETCDM335 { get; set; }

        /// <summary>
        /// ICISTART335 9(5) COMP-3 
        /// * N. SETTIMANE DI CONTRIBUZIONE ART
        /// </summary>
        [HisFieldInfoMapping(6, 3, CobolType = CobolType.Comp3Unsigned)]
        public int ICISTART335 { get; set; }

        /// <summary>
        /// ICICONART335 S9(9)V9(4) COMP-3 
        /// *EURO AMMONTARE DEI CONTRIBUTI ART
        /// </summary>
        [HisFieldInfoMapping(7, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICICONART335 { get; set; }

        /// <summary>
        /// ICIRETART335 S9(9)V9(4) COMP-3 
        /// *EURO  MONTANTE ART
        /// </summary>
        [HisFieldInfoMapping(8, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICIRETART335 { get; set; }

        /// <summary>
        /// ICISTCOM335 9(5) COMP-3 
        /// //* N. SETTIMANE DI CONTRIBUZIONE COM
        /// </summary>
        [HisFieldInfoMapping(9, 3, CobolType = CobolType.Comp3Unsigned)]
        public int ICISTCOM335 { get; set; }

        /// <summary>
        /// ICICONCOM335 S9(9)V9(4) COMP-3 
        /// //*EURO AMMONTARE DEI CONTRIBUTI COM
        /// </summary>
        [HisFieldInfoMapping(10, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICICONCOM335 { get; set; }

        /// <summary>
        /// ICIRETCOM335 S9(9)V9(4) COMP-3 
        /// //*EURO  MONTANTE COM
        /// </summary>
        [HisFieldInfoMapping(11, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICIRETCOM335 { get; set; }

        /// <summary>
        /// ICIMMF S9(9)V9(4) COMP-3 
        /// //*EURO  MONTANTE MEDIO FITTIZIE 335
        /// </summary>
        [HisFieldInfoMapping(12, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICIMMF { get; set; }

        /// <summary>
        /// ICISET1X100 9(4)  
        /// //* N. SETTIMANE CON DIRITTO AUMENTO DEL 1%
        /// </summary>
        [HisFieldInfoMapping(13, 4)]
        public int ICISET1X100 { get; set; }

        /// <summary>
        /// ICISET05X100 9(4)  
        /// ////* N. SETTIMANE CON DIRITTO AUMENTO DEL 0,5%
        /// </summary>
        [HisFieldInfoMapping(14, 4)]
        public int ICISET05X100 { get; set; }

        #endregion Tracciato Host
    }
}
