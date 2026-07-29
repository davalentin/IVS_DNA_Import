using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaW2CIR
    {
        #region tracciato COBOL
        //   04  ARW2CIR.
        //* TS1WK2CI.CPY
        //     05  ICI2105                    PIC 9999.
        //*+CODICE DECISIONE 105
        //*+DATA PRECEDENTE LIQUIDAZIONE DIRETTA PER VIRT SOS.
        //         10 ICI2DAPLIQA            PIC 9999.
        //         10 ICI2DAPLIQM            PIC 99.
        //     05 ICI2IMPCRIS34              PIC S9(5)V9(4) COMP-3.
        //*EURO +IMP. CRIST. S. 34.
        //     05 ICI2IMPCRIS91              PIC S9(5)V9(4) COMP-3.
        //*EURO +IMP. CRIST. 01/91 X -52.
        //     05 ICI2VIRT                   PIC 9(5)V9(4)  COMP-3.
        //*EURO +VIRT.INTEGRATA DEC.CALC.
        //     05 ICI2VINTERA                PIC 9(5)V9(4)  COMP-3.
        //*EURO +VIRT.INTERA A DEC.CALC.
        //     05 ICI2INCR                   PIC 9(5)V9(4) COMP-3.
        //*EURO +IMP.INCREM.INAB.222/84
        //     05 ICI2ADEG4                  PIC S9(5)V9(4) COMP-3.
        //*EURO +ADEG. PURA PER ART 4.140
        //     05 ICI2DEC638                 PIC 9(6).
        //*+DEC. PEREQUAZ. PER 638
        //     05 ICI2ADEG638                PIC S9(5)V9(4) COMP-3.
        //*EURO +IMPORTO ADEGUATA PER 638
        //     05 ICI2SUP                    PIC S9(5)V9(4) COMP-3.
        //*EURO  TOTALE IMPORTO SUPPLEMENTI
        //     05 ICI2ADEG                   PIC S9(7)V9(4) COMP-3.
        //*EURO  IMPORTO ADEGUATA SOTTOSTANTE ART.4/140 (CONV. 13 E 17)

        #endregion tracciato COBOL

        #region Tracciato Host
        // 04  ARW2CIR.
        // * TS1WK2CI.CPY
        /// <summary>
        /// ICI2105 9999  
        /// *+CODICE DECISIONE 105
        /// </summary>
        [HisFieldInfoMapping(0, 4)]
        public short ICI2105 { get; set; }

        /// <summary>
        /// ICI2DAPLIQA 9999  
        /// *+DATA PRECEDENTE LIQUIDAZIONE DIRETTA PER VIRT SOS.
        /// </summary>
        [HisFieldInfoMapping(1, 4)]
        public short ICI2DAPLIQA { get; set; }

        /// <summary>
        /// *+DATA PRECEDENTE LIQUIDAZIONE DIRETTA PER VIRT SOS.
        /// ICI2DAPLIQM 99  
        /// </summary>
        [HisFieldInfoMapping(2, 2)]
        public short ICI2DAPLIQM { get; set; }

        /// <summary>
        /// ICI2IMPCRIS34 S9(5)V9(4) COMP-3 
        /// *EURO +IMP. CRIST. S. 34.
        /// </summary>
        [HisFieldInfoMapping(3, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICI2IMPCRIS34 { get; set; }

        /// <summary>
        /// ICI2IMPCRIS91 S9(5)V9(4) COMP-3 
        /// *EURO +IMP. CRIST. 01/91 X -52.
        /// </summary>
        [HisFieldInfoMapping(4, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICI2IMPCRIS91 { get; set; }

        /// <summary>
        /// ICI2VIRT 9(5)V9(4) COMP-3 
        /// *EURO +VIRT.INTEGRATA DEC.CALC.
        /// </summary>
        [HisFieldInfoMapping(5, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal ICI2VIRT { get; set; }

        /// <summary>
        /// ICI2VINTERA 9(5)V9(4) COMP-3 
        /// *EURO +VIRT.INTERA A DEC.CALC.
        /// </summary>
        [HisFieldInfoMapping(6, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal ICI2VINTERA { get; set; }

        /// <summary>
        /// ICI2INCR 9(5)V9(4) COMP-3 
        // *EURO +IMP.INCREM.INAB.222/84
        /// </summary>
        [HisFieldInfoMapping(7, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal ICI2INCR { get; set; }

        /// <summary>
        /// ICI2ADEG4 S9(5)V9(4) COMP-3 
        /// *EURO +ADEG. PURA PER ART 4.140
        /// </summary>
        [HisFieldInfoMapping(8, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICI2ADEG4 { get; set; }

        /// <summary>
        /// ICI2DEC638 9(6)  
        /// *+DEC. PEREQUAZ. PER 638
        /// </summary>
        [HisFieldInfoMapping(9, 6)]
        public int ICI2DEC638 { get; set; }

        /// <summary>
        /// ICI2ADEG638 S9(5)V9(4) COMP-3 
        /// *EURO +IMPORTO ADEGUATA PER 638
        /// </summary>
        [HisFieldInfoMapping(10, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICI2ADEG638 { get; set; }

        /// <summary>
        /// ICI2SUP S9(5)V9(4) COMP-3 
        /// *EURO  TOTALE IMPORTO SUPPLEMENTI
        /// </summary>
        [HisFieldInfoMapping(11, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICI2SUP { get; set; }

        /// <summary>
        /// ICI2ADEG S9(7)V9(4) COMP-3 
        /// *EURO  IMPORTO ADEGUATA SOTTOSTANTE ART.4/140 (CONV. 13 E 17)
        /// </summary>
        [HisFieldInfoMapping(12, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal ICI2ADEG { get; set; }


        #endregion Tracciato Host
    }
}
