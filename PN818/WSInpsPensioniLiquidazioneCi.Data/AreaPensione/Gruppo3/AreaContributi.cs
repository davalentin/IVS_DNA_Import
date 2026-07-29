using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaContributi
    {
        #region tracciato COBOL
        //  04  CONTRIBUTI.
        //        05  TP1NUA                 PIC S9(4) COMP-3.
        //* NUM. COMTRIBUTI TOTALI PER DIRITTO
        //        05  TP1NUB                 PIC S9(4) COMP-3.
        //* NUM. CONTRIBUTI VOLONTARI PER DIRITTO
        //        05  TP1NUC                 PIC S9(4) COMP-3.
        //* NUM. CONTRIBUTI AGRICOLI
        //        05  TP1DIFN                PIC 99.
        //* DIFFERIMENTO     16.12.99: NON PIU' USATO;
        //        05  TP1MUT                 PIC S9(3) COMP-3.
        //* MUTUALITA' SCOLASTICA
        //        05  IW1NSAUT               PIC 9(5)  COMP-3.
        //*+SET.ANZ.TOTALI AUTONOMI PER MISURA
        //        05  IW1NSOBG               PIC 9(5)  COMP-3.
        //*+CONTRIBUTI OBG + FIG. PER MISURA
        //        05  IW1VVMISURA            PIC 9(5)  COMP-3.
        //*+CONTRIBUTI V.V. OBG PER MISURA
        //        05  IABNSAVV               PIC S9(5) COMP-3.
        //*+SETT. ANZ. CON VV  PER MISURA
        //        05  IABREMSVV              PIC S9(7)V9(6) COMP-3.
        //*EURO +R.M.S CON VV
        //        05  I1SETIVS               PIC 9999.
        //*TOTALE NUMERO CONTRIBUTI X VECCHIO CALCOLO CONTRIBUTIVO
        //        05  IW1IVSTOT              PIC 9(5)V9(6)  COMP-3.
        //*EURO +IMPORTO IVS TOTALE CONTR
        //        05  IABAR11VV              PIC S9(3)V9(6) COMP-3.
        //*EURO +IMP.IVS ART.11 DEI VV
        //        05  IW1FFAA                PIC 9(5) COMP-3.
        //*+N. SETT. EFF. DI CONTRIBUZIONE IN
        //*                        COSTANZA DI RAPPORTO DI LAVORO SVOLTO IN
        //*                        ITALIA (ART. 7 L. 407/90)
        //        05  IABNSASS               PIC S9(5) COMP-3.
        //*+SETT.GOD.ASSEGNO
        //        05  ICI2SETFIT             PIC 9(4).
        //*+SETT.FITTIZIE
        //        05  ICI2SETTEST            PIC 9(4).
        //*+TOTALE SETTIMANE ESTERE
        //        05  ICOEF-DIFF             PIC 99V9(3).
        #endregion tracciato COBOL

        #region Tracciato Host
        // 04  CONTRIBUTI.
        /// <summary>
        /// TP1NUA S9(4) COMP-3 
        /// * NUM. COMTRIBUTI TOTALI PER DIRITTO
        /// </summary>
        [HisFieldInfoMapping(0, 3, CobolType = CobolType.Comp3)]
        public int TP1NUA { get; set; }

        /// <summary>
        /// TP1NUB S9(4) COMP-3 
        /// * NUM. CONTRIBUTI VOLONTARI PER DIRITTO
        /// </summary>
        [HisFieldInfoMapping(1, 3, CobolType = CobolType.Comp3)]
        public int TP1NUB { get; set; }

        /// <summary>
        /// TP1NUC S9(4) COMP-3 
        /// * NUM. CONTRIBUTI AGRICOLI
        /// </summary>
        [HisFieldInfoMapping(2, 3, CobolType = CobolType.Comp3)]
        public int TP1NUC { get; set; }

        /// <summary>
        /// TP1DIFN 99  
        /// * DIFFERIMENTO     16.12.99: NON PIU' USATO;
        /// </summary>
        [HisFieldInfoMapping(3, 2)]
        public short TP1DIFN { get; set; }

        /// <summary>
        /// TP1MUT S9(3) COMP-3 
        /// * MUTUALITA' SCOLASTICA
        /// </summary>
        [HisFieldInfoMapping(4, 2, CobolType = CobolType.Comp3)]
        public int TP1MUT { get; set; }

        /// <summary>
        /// IW1NSAUT 9(5) COMP-3 
        /// *+SET.ANZ.TOTALI AUTONOMI PER MISURA
        /// </summary>
        [HisFieldInfoMapping(5, 3, CobolType = CobolType.Comp3Unsigned)]
        public int IW1NSAUT { get; set; }

        /// <summary>
        /// IW1NSOBG 9(5) COMP-3 
        /// *+CONTRIBUTI OBG + FIG. PER MISURA
        /// </summary>
        [HisFieldInfoMapping(6, 3, CobolType = CobolType.Comp3Unsigned)]
        public int IW1NSOBG { get; set; }

        /// <summary>
        /// IW1VVMISURA 9(5) COMP-3 
        /// *+CONTRIBUTI V.V. OBG PER MISURA
        /// </summary>
        [HisFieldInfoMapping(7, 3, CobolType = CobolType.Comp3Unsigned)]
        public int IW1VVMISURA { get; set; }

        /// <summary>
        /// IABNSAVV S9(5) COMP-3 
        /// *+SETT. ANZ. CON VV  PER MISURA
        /// </summary>
        [HisFieldInfoMapping(8, 3, CobolType = CobolType.Comp3)]
        public int IABNSAVV { get; set; }

        /// <summary>
        /// IABREMSVV S9(7)V9(6) COMP-3 
        /// *EURO +R.M.S CON VV
        /// </summary>
        [HisFieldInfoMapping(9, 7, Scale = 6, CobolType = CobolType.Comp3)]
        public decimal IABREMSVV { get; set; }

        /// <summary>
        /// I1SETIVS 9999  
        /// *TOTALE NUMERO CONTRIBUTI X VECCHIO CALCOLO CONTRIBUTIVO
        /// </summary>
        [HisFieldInfoMapping(10, 4)]
        public short I1SETIVS { get; set; }

        /// <summary>
        /// IW1IVSTOT 9(5)V9(6) COMP-3 
        /// *EURO +IMPORTO IVS TOTALE CONTR
        /// </summary>
        [HisFieldInfoMapping(11, 6, Scale = 6, CobolType = CobolType.Comp3Unsigned)]
        public decimal IW1IVSTOT { get; set; }

        /// <summary>
        /// IABAR11VV S9(3)V9(6) COMP-3 
        /// *EURO +IMP.IVS ART.11 DEI VV
        /// </summary>
        [HisFieldInfoMapping(12, 5, Scale = 6, CobolType = CobolType.Comp3)]
        public decimal IABAR11VV { get; set; }

        /// <summary>
        /// IW1FFAA 9(5) COMP-3
        /// *+N. SETT. EFF. DI CONTRIBUZIONE IN
        /// *                        COSTANZA DI RAPPORTO DI LAVORO SVOLTO IN
        /// *                        ITALIA (ART. 7 L. 407/90)
        /// </summary>
        [HisFieldInfoMapping(13, 3, CobolType = CobolType.Comp3Unsigned)]
        public int IW1FFAA { get; set; }

        /// <summary>
        /// IABNSASS S9(5) COMP-3 
        /// *+SETT.GOD.ASSEGNO
        /// </summary>
        [HisFieldInfoMapping(14, 3, CobolType = CobolType.Comp3)]
        public int IABNSASS { get; set; }

        /// <summary>
        /// ICI2SETFIT 9(4) 
        /// *+SETT.FITTIZIE 
        /// </summary>
        [HisFieldInfoMapping(15, 4)]
        public short ICI2SETFIT { get; set; }

        /// <summary>
        /// ICI2SETTEST 9(4)  
        /// *+TOTALE SETTIMANE ESTERE
        /// </summary>
        [HisFieldInfoMapping(16, 4)]
        public short ICI2SETTEST { get; set; }

        /// <summary>
        /// ICOEF_DIFF 99V9(3)  
        /// </summary>
        [HisFieldInfoMapping(17, 5, Scale = 3)]
        public decimal ICOEF_DIFF { get; set; }
        #endregion Tracciato Host
    }
}
