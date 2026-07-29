using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago
{
    public class VL : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  YVL-RECAGO-BIS REDEFINES COMUNE-RECAGO.
        //    02  YVL-RECAGO.
        //        03 YVLTIPOR                      PIC X.
        //        03 YVLFONDO                      PIC X(3).
        //        03 YVLTIPEN                      PIC 9.
        //        03 YVLTIPLQ                      PIC 9.
        //        03 YVLDECSS                      PIC 99.
        //        03 YVLDECAA                      PIC 99.
        //        03 YVLDECMM                      PIC 99.
        //        03 YVLSCASS                      PIC 99.
        //        03 YVLSCAAA                      PIC 99.
        //        03 YVLSCAMM                      PIC 99.
        //*
        //* IMPORTO TOTALE CONTRIBUTI                           IN A4SQ
        //        03 YVLCONTR                      PIC 9(6)V9999.
        //* MONTANTE                                            IN A4TQ
        //        03 YVLMONTA                      PIC 9(8)V9999.
        //*
        //*-ANZIANITA' 1ø PERIODO                               IN A4VQ
        //        03 YVLANZ1A                      PIC 9(2).
        //        03 YVLANZ1M                      PIC 9(2).
        //        03 YVLANZ1G                      PIC 9(2).
        //* MONTANTE DAL 7-97 IN POI                            IN A4QMONC
        //        03 YVLMONT2                      PIC 9(8)V9999.
        //*-ANZIANITA' 2ø PERIODO                               IN A4QRIFC
        //        03 YVLANZ2A                      PIC 99.
        //        03 YVLANZ2M                      PIC 99.
        //        03 YVLANZ2G                      PIC 99.
        //* MONTANTE CONTRIBUTIVO AGO                           IN A4QMAGO1
        //        03 YVLMONT3                      PIC 9(8)V9999.
        //*-RETRIBUZIONE MEDIA SETTIMANALE (A)                  IN A4LQ
        //        03 YVLRSETA                      PIC 9(6)V9999.
        //*-SETTIMANE DI CONTRIBUZIONE (A1) AL  26-11-88        IN A4IQ
        //        03 YVLSET1A                      PIC 9(5).
        //*-SETTIMANE DI CONTRIBUZIONE (A2) AL  31-12-92        IN A4QA2
        //        03 YVLSET2A                      PIC 9(5).
        //*-RETRIBUZIONE MEDIA SETTIMANALE (B)                  IN A4NQ
        //        03 YVLRSETB                      PIC 9(6)V9999.
        //*-SETTIMANE DI CONTRIBUZIONE (B)  AL  31-12-94        IN A4MQ
        //        03 YVLSETTB                      PIC 9(5).
        //*-SETTIMANE DI CONTRIBUZIONE (C1) AL  30-06-97        IN A4OQ
        //        03 YVLSET1C                      PIC 9(5).
        //*-SETTIMANE DI CONTRIBUZIONE (C2) AL  31-12-97        IN A4QC2
        //        03 YVLSET2C                      PIC 9(5).
        //*-RETRIBUZIONE MEDIA SETTIMANALE (D)                  IN A4PQ
        //        03 YVLRSETD                      PIC 9(6)V9999.
        //*-SETTIMANE DI CONTRIBUZIONE (D)  DAL 01-01-98        IN A4QQ
        //        03 YVLSETTD                      PIC 9(5).
        //*-RETRIBUZIONE AGO PER TETTO PENSIONE.                A4RQ
        //        03 YVLTETTO                      PIC 9(6)V9999.
        //*-CAMPO A DISPOSIZIONE.                               A4QPRECO
        //        03 YVLPRECO                      PIC X.
        //GD0212        03 YVLIMPCRT                     PIC 9(6)V9999.  
        //GD0212        03 YVLMONTA2012                  PIC 9(8)V9999.      
        //GD0212        03 YVLSETT2012                   PIC 9(4).   
        //GD1012        03 YVLFLAG214                    PIC X.
        //GD1012        03 YVLPERC214                    PIC 99V99.
        //*-SETTIMANE 707
        //              03 YVLSETA1707                    PIC 9(4). 
        //              03 YVLSETA2707                    PIC 9(4).          
        //              03 YVLSETB707                     PIC 9(4).          
        //              03 YVLSETC1707                    PIC 9(4). 
        //              03 YVLSETC2707                    PIC 9(4).         
        //              03 YVLSETD707                     PIC 9(4).          
        //              03 YVLCALC707                     PIC X(01).
        ///////////////////////////////////////////////////
        //        03 YVLSETDIR                     PIC 9(4).
        //*-PROGRESSIVO RECORD
        //        03 YVLPROGR                      PIC 99.
        //*-AREA A DISPOSIZIONE
        //        03 YVLFILLER                     PIC X(199).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  YVL-RECAGO-BIS REDEFINES COMUNE-RECAGO.
        // 02  YVL-RECAGO.
        /// <summary>
        /// YVLTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string YVLTIPOR { get; set; }

        /// <summary>
        /// YVLFONDO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string YVLFONDO { get; set; }

        /// <summary>
        /// YVLTIPEN 9  
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short YVLTIPEN { get; set; }

        /// <summary>
        /// YVLTIPLQ 9  
        /// </summary>
        [HisFieldInfoMapping(3, 1, CobolType = CobolType.Unsigned)]
        public short YVLTIPLQ { get; set; }

        /// <summary>
        /// YVLDECSS 99  
        /// </summary>
        [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
        public short YVLDECSS { get; set; }

        /// <summary>
        /// YVLDECAA 99  
        /// </summary>
        [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
        public short YVLDECAA { get; set; }

        /// <summary>
        /// YVLDECMM 99  
        /// </summary>
        [HisFieldInfoMapping(6, 2, CobolType = CobolType.Unsigned)]
        public short YVLDECMM { get; set; }

        /// <summary>
        /// YVLSCASS 99  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short YVLSCASS { get; set; }

        /// <summary>
        /// YVLSCAAA 99  
        /// </summary>
        [HisFieldInfoMapping(8, 2, CobolType = CobolType.Unsigned)]
        public short YVLSCAAA { get; set; }

        /// <summary>
        /// YVLSCAMM 99  
        /// </summary>
        [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
        public short YVLSCAMM { get; set; }

        //*
        // * IMPORTO TOTALE CONTRIBUTI                           IN A4SQ
        /// <summary>
        /// YVLCONTR 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(10, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YVLCONTR { get; set; }

        // * MONTANTE                                            IN A4TQ
        /// <summary>
        /// YVLMONTA 9(8)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(11, 12, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YVLMONTA { get; set; }

        //*
        // *-ANZIANITA' 1ø PERIODO                               IN A4VQ
        /// <summary>
        /// YVLANZ1A 9(2)  
        /// </summary>
        [HisFieldInfoMapping(12, 2, CobolType = CobolType.Unsigned)]
        public short YVLANZ1A { get; set; }

        /// <summary>
        /// YVLANZ1M 9(2)  
        /// </summary>
        [HisFieldInfoMapping(13, 2, CobolType = CobolType.Unsigned)]
        public short YVLANZ1M { get; set; }

        /// <summary>
        /// YVLANZ1G 9(2)  
        /// </summary>
        [HisFieldInfoMapping(14, 2, CobolType = CobolType.Unsigned)]
        public short YVLANZ1G { get; set; }

        // * MONTANTE DAL 7-97 IN POI                            IN A4QMONC
        /// <summary>
        /// YVLMONT2 9(8)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(15, 12, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YVLMONT2 { get; set; }

        // *-ANZIANITA' 2ø PERIODO                               IN A4QRIFC
        /// <summary>
        /// YVLANZ2A 99  
        /// </summary>
        [HisFieldInfoMapping(16, 2, CobolType = CobolType.Unsigned)]
        public short YVLANZ2A { get; set; }

        /// <summary>
        /// YVLANZ2M 99  
        /// </summary>
        [HisFieldInfoMapping(17, 2, CobolType = CobolType.Unsigned)]
        public short YVLANZ2M { get; set; }

        /// <summary>
        /// YVLANZ2G 99  
        /// </summary>
        [HisFieldInfoMapping(18, 2, CobolType = CobolType.Unsigned)]
        public short YVLANZ2G { get; set; }

        // * MONTANTE CONTRIBUTIVO AGO                           IN A4QMAGO1
        /// <summary>
        /// YVLMONT3 9(8)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(19, 12, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YVLMONT3 { get; set; }

        // *-RETRIBUZIONE MEDIA SETTIMANALE (A)                  IN A4LQ
        /// <summary>
        /// YVLRSETA 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(20, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YVLRSETA { get; set; }

        // *-SETTIMANE DI CONTRIBUZIONE (A1) AL  26-11-88        IN A4IQ
        /// <summary>
        /// YVLSET1A 9(5)  
        /// </summary>
        [HisFieldInfoMapping(21, 5, CobolType = CobolType.Unsigned)]
        public int YVLSET1A { get; set; }

        // *-SETTIMANE DI CONTRIBUZIONE (A2) AL  31-12-92        IN A4QA2
        /// <summary>
        /// YVLSET2A 9(5)  
        /// </summary>
        [HisFieldInfoMapping(22, 5, CobolType = CobolType.Unsigned)]
        public int YVLSET2A { get; set; }

        // *-RETRIBUZIONE MEDIA SETTIMANALE (B)                  IN A4NQ
        /// <summary>
        /// YVLRSETB 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(23, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YVLRSETB { get; set; }

        // *-SETTIMANE DI CONTRIBUZIONE (B)  AL  31-12-94        IN A4MQ
        /// <summary>
        /// YVLSETTB 9(5)  
        /// </summary>
        [HisFieldInfoMapping(24, 5, CobolType = CobolType.Unsigned)]
        public int YVLSETTB { get; set; }

        // *-SETTIMANE DI CONTRIBUZIONE (C1) AL  30-06-97        IN A4OQ
        /// <summary>
        /// YVLSET1C 9(5)  
        /// </summary>
        [HisFieldInfoMapping(25, 5, CobolType = CobolType.Unsigned)]
        public int YVLSET1C { get; set; }

        // *-SETTIMANE DI CONTRIBUZIONE (C2) AL  31-12-97        IN A4QC2
        /// <summary>
        /// YVLSET2C 9(5)  
        /// </summary>
        [HisFieldInfoMapping(26, 5, CobolType = CobolType.Unsigned)]
        public int YVLSET2C { get; set; }

        // *-RETRIBUZIONE MEDIA SETTIMANALE (D)                  IN A4PQ
        /// <summary>
        /// YVLRSETD 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(27, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YVLRSETD { get; set; }

        // *-SETTIMANE DI CONTRIBUZIONE (D)  DAL 01-01-98        IN A4QQ
        /// <summary>
        /// YVLSETTD 9(5)  
        /// </summary>
        [HisFieldInfoMapping(28, 5, CobolType = CobolType.Unsigned)]
        public int YVLSETTD { get; set; }

        // *-RETRIBUZIONE AGO PER TETTO PENSIONE.                A4RQ
        /// <summary>
        /// YVLTETTO 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(29, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YVLTETTO { get; set; }

        // *-CAMPO A DISPOSIZIONE.                               A4QPRECO
        /// <summary>
        /// YVLPRECO X  
        /// </summary>
        [HisFieldInfoMapping(30, 1)]
        public string YVLPRECO { get; set; }

        /// </summary>
        /// YVLIMPCRT 9(6)V9999
        /// </summary>
        [HisFieldInfoMapping(31, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YVLIMPCRT { get; set; }

        /// </summary>
        /// YVLMONTA2012 9(8)V9999      
        /// </summary>
        [HisFieldInfoMapping(32, 12, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YVLMONTA2012 { get; set; }

        /// </summary>
        /// YVLSETT2012 9(4)   
        /// </summary>
        [HisFieldInfoMapping(33, 4, CobolType = CobolType.Unsigned)]
        public int YVLSETT2012 { get; set; }

        /// </summary>
        /// YVLFLAG214 X
        /// </summary>
        [HisFieldInfoMapping(34, 1)]
        public string YVLFLAG214 { get; set; }

        /// </summary>
        /// YVLPERC214 99V99
        /// </summary>
        [HisFieldInfoMapping(35, 4, Scale = 2, CobolType = CobolType.Unsigned)]
        public decimal YVLPERC214 { get; set; }

        /// <summary>
        /// YVLSETA1707 9(4).
        /// </summary>
        [HisFieldInfoMapping(36, 4, CobolType = CobolType.Unsigned)]
        public short YVLSETA1707 { get; set; }

        /// <summary>
        /// YVLSETA2707 9(4).
        /// </summary>
        [HisFieldInfoMapping(37, 4, CobolType = CobolType.Unsigned)]
        public short YVLSETA2707 { get; set; }

        /// <summary>
        /// YVLSETB707 9(4)
        /// </summary>
        [HisFieldInfoMapping(38, 4, CobolType = CobolType.Unsigned)]
        public short YVLSETB707 { get; set; }

        /// <summary>
        /// YVLSETC1707 9(4)
        /// </summary>
        [HisFieldInfoMapping(39, 4, CobolType = CobolType.Unsigned)]
        public short YVLSETC1707 { get; set; }

        /// <summary>
        /// YVLSETC2707 9(4)
        /// </summary>
        [HisFieldInfoMapping(40, 4, CobolType = CobolType.Unsigned)]
        public short YVLSETC2707 { get; set; }

        /// <summary>
        /// YVLSETD707 9(4)
        /// </summary>
        [HisFieldInfoMapping(41, 4, CobolType = CobolType.Unsigned)]
        public short YVLSETD707 { get; set; }

        /// <summary>
        /// YVLCALC707 X(01)
        /// </summary>
        [HisFieldInfoMapping(42, 1)]
        public string YVLCALC707 { get; set; }

        /// <summary>
        /// YVLSETDIR 9(4)     
        /// <summary>
        [HisFieldInfoMapping(43, 4, CobolType = CobolType.Unsigned)]
        public int YVLSETDIR { get; set; }

        // *-PROGRESSIVO RECORD
        /// <summary>
        /// YVLPROGR 99  
        /// </summary>
        [HisFieldInfoMapping(44, 2, CobolType = CobolType.Unsigned)]
        public short YVLPROGR { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "VL"; }
        }
        #endregion Properties
    }
}
