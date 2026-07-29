using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostResponse.AreaStampa
{
    public class Coda
    {
        #region Constructor
        internal Coda()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 INTEGR-ART11     PIC 9(07)V9(04).
        //*                                                          17600
        //     02 DECORR-SUPPL     PIC 9(08).
        //*                                  (AAAAMMGG)              17611
        //     02 DECORR-SUPPL336  PIC 9(08).
        //*                                  (AAAAMMGG)              17619
        //     02 RMS-L7290        PIC 9(07)V9(04).
        //*                                                          17627
        //     02 RMS-DPCM1289     PIC 9(07)V9(04).
        //*                                                          17638
        //     02 F-TRT25-PI       PIC 9.
        //*                                                          17649
        //     02 F-TABSP          PIC 99.
        //*                                                          17650
        //     02 F-SUPFIT-PI      PIC 9.
        //*                                                          17652
        //     02 F-PI             PIC 9999.
        //     02 F-GEST-PI REDEFINES F-PI  PIC 9 OCCURS 4 TIMES.
        //*                           POSIZIONALE (OBG-CDCM-ART-COM)
        //*                                                          17653
        //     02 ANNI-RISC-PI     PIC 9(02).
        //*                           ANNI RISCATTO FONDO PI         17657
        //     02 MESI-RISC-PI     PIC 9(02).
        //*                           MESI RISCATTO FONDO PI         17659
        //     02 GIORNI-RISC-PI   PIC 9(02).
        //*                           GIORNI RISCATTO FONDO PI       17661
        //     02 QUALIF-PI        PIC X(05).
        //*                           QUALIFICA                      17663
        //     02 FILLER           PIC X(47).
        //*                                                          17668
        //     02 RETR-ULT-12      PIC 9(07)V9(04) COMP-3.
        //*                       RETRIBUZIONE ULTIMI 12 MESI        17715
        //*                       CALCOLO RETR. FONDO TT QUOTA A
        //     02 RETR-ULT-36      PIC 9(07)V9(04) COMP-3.
        //*                       RETRIBUZIONE MEDIA ULT.36 MESI     17721
        //*                           CALCOLO RETR. FONDO TT QUOTA A
        //     02 FILLER           PIC X(02).
        //*                                                          17727
        //     02 GP2-ELSA         PIC 9(07)V9(04) COMP-3.
        //*                            QUOTA A   A.G.O.              17729
        //     02 GP2-ELSB         PIC 9(07)V9(04) COMP-3.
        //*                            QUOTA B   A.G.O.              17735
        //     02 GP2-ELRSA        PIC 9(07)V9(04) COMP-3.
        //*                            RMS A     A.G.O.              17741
        //     02 GP2-ELRSB        PIC 9(07)V9(04) COMP-3.
        //*                            RMS B     A.G.O.              17747
        //     02 IMP-CAPMS        PIC 9(07)V9(04) COMP-3.
        //*                            CAPITALIZZAZIONE MENSILE      17753
        //     02 IMP-CAPTOT       PIC 9(07)V9(04) COMP-3.
        //*                            CAPITALIZZAZIONE TOTALE       17759
        //     02 ANNI-ANTE-0797   PIC 9(02).
        //*                           ANNI PER QUOTA DAL 1.1.96 AL   17765
        //*                           30.6.97 X CALC.MONT.FONDO VL
        //     02 ANNI-POST-0697   PIC 9(02).
        //*                           ANNI PER QUOTA DAL 1.7.97 AL   17767
        //*                           TERMINE X CALC.MONT.FONDO VL
        //     02 MESI-ANTE-0797   PIC 9(04).
        //*                           MESI PER QUOTA DAL 1.1.96 AL   17769
        //*                           30.6.97 X CALC.MONT.FONDO VL
        //     02 MESI-POST-0697   PIC 9(04).
        //*                           MESI PER QUOTA DAL 1.7.97 AL   17773
        //*                           TERMINE X CALC.MONT.FONDO VL
        //     02 GIOR-ANTE-0797   PIC 9(05).
        //*                           GIORNI PER QUOTA DAL 1.1.96    17777
        //*                           AL 30.6.97 X CALC.MONT.FONDO VL
        //     02 GIOR-POST-0697   PIC 9(05).
        //*                            GIORNI PER QUOTA DAL 1.7.97   17782
        //*                            AL TERMINE X CALC.MONT.FONDO VL
        //     02 IMPO-CONT-TOTA   PIC 9(07)V9(04) COMP-3.
        //*                            IMPORTO CONTRIBUTIVO TOTALE   17787
        //*                            PER I FONDI EL-TT-ET.
        //*                            PER I FONDI GA ED ES E' MON
        //*                            TANTE ESCLUSIVO.
        //     02 FILLER           PIC X(01).
        //*                                                          17793
        //     02 ANNI-ASS-VL      PIC 9(02).
        //*                            ANNI ASSICURAZIONE FONDO VL   17794
        //     02 PERC-IRPEF-VL    PIC 99V99.
        //*                                                          17796
        //     02 FILLER           PIC X(15).
        //*                                                          17800
        //     02 SETT-ANTE93-ET   PIC 9(04).
        //*                            SETTIMANE ANTE 93             17815
        //     02 SETT-POST92-ET   PIC 9(04).
        //*                            SETTIMANE POST 92             17819
        //     02 RMS-ANTE93-ET    PIC 9(07)V9(04) COMP-3.
        //*                            RETRIBUZ.MEDIA SETT. ANTE 93  17823
        //     02 RMS-POST92-ET    PIC 9(07)V9(04) COMP-3.
        //*                            RETRIBUZ.MEDIA SETT. POST 92  17829
        //     02 FILLER           PIC X(15).
        //*                                                          17835
        //     02 SETT-PI-A        PIC 9(04).
        //*                                                          17850
        //     02 SETT-PI-B        PIC 9(04).
        //*                                                          17854
        //     02 COD-CIECO-PI     PIC 9.
        //*                                                          17858
        //     02 PERC-CAPITAL     PIC 99V99.
        //*                                                          17859
        //     02 PERC-OMOGEN      PIC 99V99.
        //*                                                          17863
        //     02 FILLER           PIC X(03).
        //*                                                          17867
        //     02 ANNI-FS          PIC 9(02) OCCURS 5 TIMES.
        //*                                                          17870
        //     02 MESI-FS          PIC 9(02) OCCURS 5 TIMES.
        //*                                                          15880
        //     02 GIOR-FS          PIC 9(02) OCCURS 5 TIMES.
        //*                                                          17890
        //     02 FL-DATA-SW       PIC X(01).
        //*                                  (0 = NESSUNA SCRITTA)   17900
        //*                                  (1 = PENS.PROPOSTA PER)
        //*                                  (2 = PENS.PROPOSTA DA)
        //     02 SEDE-DATA-SW     PIC X(22).
        //*                                  (NOME SEDE IN CHIARO)   17901
        //     02 SEDE-N-SW.
        //       03 SEDE-SW        PIC 9(04).
        //       03 CO-SW          PIC 9(02).
        //*                                  (COD.SEDE COMPETENTE)   17923
        //      02 CAUSALE          PIC X(71).
        //*                                                          17929
        //      02 FILLER           PIC X(2000).
        //      02 FILLER           PIC X(9000).
        //      02 FILLER           PIC X(5518).
        //*                                                          32518
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// INTEGR_ART11 9(07)V9(04)  
        /// </summary>
        [HisFieldInfoMapping(0, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal INTEGR_ART11 { get; set; }

        // *                                                          17600
        /// <summary>
        /// DECORR_SUPPL 9(08)  
        /// </summary>
        [HisFieldInfoMapping(1, 8, CobolType = CobolType.Unsigned)]
        public int DECORR_SUPPL { get; set; }

        // *                                  (AAAAMMGG)              17611
        /// <summary>
        /// DECORR_SUPPL336 9(08)  
        /// </summary>
        [HisFieldInfoMapping(2, 8, CobolType = CobolType.Unsigned)]
        public int DECORR_SUPPL336 { get; set; }

        // *                                  (AAAAMMGG)              17619
        /// <summary>
        /// RMS_L7290 9(07)V9(04)  
        /// </summary>
        [HisFieldInfoMapping(3, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal RMS_L7290 { get; set; }

        // *                                                          17627
        /// <summary>
        /// RMS_DPCM1289 9(07)V9(04)  
        /// </summary>
        [HisFieldInfoMapping(4, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal RMS_DPCM1289 { get; set; }

        // *                                                          17638
        /// <summary>
        /// F_TRT25_PI 9  
        /// </summary>
        [HisFieldInfoMapping(5, 1, CobolType = CobolType.Unsigned)]
        public short F_TRT25_PI { get; set; }

        // *                                                          17649
        /// <summary>
        /// F_TABSP 99  
        /// </summary>
        [HisFieldInfoMapping(6, 2, CobolType = CobolType.Unsigned)]
        public short F_TABSP { get; set; }

        // *                                                          17650
        /// <summary>
        /// F_SUPFIT_PI 9  
        /// </summary>
        [HisFieldInfoMapping(7, 1, CobolType = CobolType.Unsigned)]
        public short F_SUPFIT_PI { get; set; }

        // *                                                          17652
        /// <summary>
        /// F_PI 9999  
        /// </summary>
        [HisFieldInfoMapping(8, 4, CobolType = CobolType.Unsigned)]
        public short F_PI { get; set; }

        // *                           POSIZIONALE (OBG-CDCM-ART-COM)
        // *                                                          17653
        /// <summary>
        /// ANNI_RISC_PI 9(02)  
        /// </summary>
        [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
        public short ANNI_RISC_PI { get; set; }

        // *                           ANNI RISCATTO FONDO PI         17657
        /// <summary>
        /// MESI_RISC_PI 9(02)  
        /// </summary>
        [HisFieldInfoMapping(10, 2, CobolType = CobolType.Unsigned)]
        public short MESI_RISC_PI { get; set; }

        // *                           MESI RISCATTO FONDO PI         17659
        /// <summary>
        /// GIORNI_RISC_PI 9(02)  
        /// </summary>
        [HisFieldInfoMapping(11, 2, CobolType = CobolType.Unsigned)]
        public short GIORNI_RISC_PI { get; set; }

        // *                           GIORNI RISCATTO FONDO PI       17661
        /// <summary>
        /// QUALIF_PI X(05)  
        /// </summary>
        [HisFieldInfoMapping(12, 5)]
        public string QUALIF_PI { get; set; }

        // *                           QUALIFICA                      17663
        /// <summary>
        /// FILLER X(47)  
        /// </summary>
        [HisFieldInfoMapping(13, 47)]
        public string FILLER1 { get; set; }

        // *                                                          17668
        /// <summary>
        /// RETR_ULT_12 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(14, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal RETR_ULT_12 { get; set; }

        // *                       RETRIBUZIONE ULTIMI 12 MESI        17715
        // *                       CALCOLO RETR. FONDO TT QUOTA A
        /// <summary>
        /// RETR_ULT_36 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(15, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal RETR_ULT_36 { get; set; }

        // *                       RETRIBUZIONE MEDIA ULT.36 MESI     17721
        // *                           CALCOLO RETR. FONDO TT QUOTA A
        /// <summary>
        /// FILLER X(02)  
        /// </summary>
        [HisFieldInfoMapping(16, 2)]
        public string FILLER2 { get; set; }

        // *                                                          17727
        /// <summary>
        /// GP2_ELSA 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(17, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal GP2_ELSA { get; set; }

        // *                            QUOTA A   A.G.O.              17729
        /// <summary>
        /// GP2_ELSB 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(18, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal GP2_ELSB { get; set; }

        // *                            QUOTA B   A.G.O.              17735
        /// <summary>
        /// GP2_ELRSA 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(19, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal GP2_ELRSA { get; set; }

        // *                            RMS A     A.G.O.              17741
        /// <summary>
        /// GP2_ELRSB 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(20, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal GP2_ELRSB { get; set; }

        // *                            RMS B     A.G.O.              17747
        /// <summary>
        /// IMP_CAPMS 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(21, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal IMP_CAPMS { get; set; }

        // *                            CAPITALIZZAZIONE MENSILE      17753
        /// <summary>
        /// IMP_CAPTOT 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(22, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal IMP_CAPTOT { get; set; }

        // *                            CAPITALIZZAZIONE TOTALE       17759
        /// <summary>
        /// ANNI_ANTE_0797 9(02)  
        /// </summary>
        [HisFieldInfoMapping(23, 2, CobolType = CobolType.Unsigned)]
        public short ANNI_ANTE_0797 { get; set; }

        // *                           ANNI PER QUOTA DAL 1.1.96 AL   17765
        // *                           30.6.97 X CALC.MONT.FONDO VL
        /// <summary>
        /// ANNI_POST_0697 9(02)  
        /// </summary>
        [HisFieldInfoMapping(24, 2, CobolType = CobolType.Unsigned)]
        public short ANNI_POST_0697 { get; set; }

        // *                           ANNI PER QUOTA DAL 1.7.97 AL   17767
        // *                           TERMINE X CALC.MONT.FONDO VL
        /// <summary>
        /// MESI_ANTE_0797 9(04)  
        /// </summary>
        [HisFieldInfoMapping(25, 4, CobolType = CobolType.Unsigned)]
        public short MESI_ANTE_0797 { get; set; }

        // *                           MESI PER QUOTA DAL 1.1.96 AL   17769
        // *                           30.6.97 X CALC.MONT.FONDO VL
        /// <summary>
        /// MESI_POST_0697 9(04)  
        /// </summary>
        [HisFieldInfoMapping(26, 4, CobolType = CobolType.Unsigned)]
        public short MESI_POST_0697 { get; set; }

        // *                           MESI PER QUOTA DAL 1.7.97 AL   17773
        // *                           TERMINE X CALC.MONT.FONDO VL
        /// <summary>
        /// GIOR_ANTE_0797 9(05)  
        /// </summary>
        [HisFieldInfoMapping(27, 5, CobolType = CobolType.Unsigned)]
        public int GIOR_ANTE_0797 { get; set; }

        // *                           GIORNI PER QUOTA DAL 1.1.96    17777
        // *                           AL 30.6.97 X CALC.MONT.FONDO VL
        /// <summary>
        /// GIOR_POST_0697 9(05)  
        /// </summary>
        [HisFieldInfoMapping(28, 5, CobolType = CobolType.Unsigned)]
        public int GIOR_POST_0697 { get; set; }

        // *                            GIORNI PER QUOTA DAL 1.7.97   17782
        // *                            AL TERMINE X CALC.MONT.FONDO VL
        /// <summary>
        /// IMPO_CONT_TOTA 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(29, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal IMPO_CONT_TOTA { get; set; }

        // *                            IMPORTO CONTRIBUTIVO TOTALE   17787
        // *                            PER I FONDI EL-TT-ET.
        // *                            PER I FONDI GA ED ES E' MON
        // *                            TANTE ESCLUSIVO.
        /// <summary>
        /// FILLER X(01)  
        /// </summary>
        [HisFieldInfoMapping(30, 1)]
        public string FILLER3 { get; set; }

        // *                                                          17793
        /// <summary>
        /// ANNI_ASS_VL 9(02)  
        /// </summary>
        [HisFieldInfoMapping(31, 2, CobolType = CobolType.Unsigned)]
        public short ANNI_ASS_VL { get; set; }

        // *                            ANNI ASSICURAZIONE FONDO VL   17794
        /// <summary>
        /// PERC_IRPEF_VL 99V9(2)  
        /// </summary>
        [HisFieldInfoMapping(32, 4, Scale = 2, CobolType = CobolType.Unsigned)]
        public decimal PERC_IRPEF_VL { get; set; }

        // *                                                          17796
        /// <summary>
        /// FILLER X(15)  
        /// </summary>
        [HisFieldInfoMapping(33, 15)]
        public string FILLER4 { get; set; }

        // *                                                          17800
        /// <summary>
        /// SETT_ANTE93_ET 9(04)  
        /// </summary>
        [HisFieldInfoMapping(34, 4, CobolType = CobolType.Unsigned)]
        public short SETT_ANTE93_ET { get; set; }

        // *                            SETTIMANE ANTE 93             17815
        /// <summary>
        /// SETT_POST92_ET 9(04)  
        /// </summary>
        [HisFieldInfoMapping(35, 4, CobolType = CobolType.Unsigned)]
        public short SETT_POST92_ET { get; set; }

        // *                            SETTIMANE POST 92             17819
        /// <summary>
        /// RMS_ANTE93_ET 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(36, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal RMS_ANTE93_ET { get; set; }

        // *                            RETRIBUZ.MEDIA SETT. ANTE 93  17823
        /// <summary>
        /// RMS_POST92_ET 9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(37, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal RMS_POST92_ET { get; set; }

        // *                            RETRIBUZ.MEDIA SETT. POST 92  17829
        /// <summary>
        /// FILLER X(15)  
        /// </summary>
        [HisFieldInfoMapping(38, 15)]
        public string FILLER5 { get; set; }

        // *                                                          17835
        /// <summary>
        /// SETT_PI_A 9(04)  
        /// </summary>
        [HisFieldInfoMapping(39, 4, CobolType = CobolType.Unsigned)]
        public short SETT_PI_A { get; set; }

        // *                                                          17850
        /// <summary>
        /// SETT_PI_B 9(04)  
        /// </summary>
        [HisFieldInfoMapping(40, 4, CobolType = CobolType.Unsigned)]
        public short SETT_PI_B { get; set; }

        // *                                                          17854
        /// <summary>
        /// COD_CIECO_PI 9  
        /// </summary>
        [HisFieldInfoMapping(41, 1, CobolType = CobolType.Unsigned)]
        public short COD_CIECO_PI { get; set; }

        // *                                                          17858
        /// <summary>
        /// PERC_CAPITAL 99V9(2)  
        /// </summary>
        [HisFieldInfoMapping(42, 4, Scale = 2, CobolType = CobolType.Unsigned)]
        public decimal PERC_CAPITAL { get; set; }

        // *                                                          17859
        /// <summary>
        /// PERC_OMOGEN 99V9(2)  
        /// </summary>
        [HisFieldInfoMapping(43, 4, Scale = 2, CobolType = CobolType.Unsigned)]
        public decimal PERC_OMOGEN { get; set; }

        // *                                                          17863
        /// <summary>
        /// FILLER X(03)  
        /// </summary>
        [HisFieldInfoMapping(44, 3)]
        public string FILLER6 { get; set; }

        // *                                                          17867

        [HisComplexAreaInfoMapping(45, ListCount = 5)]
        public List<AnniFS> LISTAnniFS { get; internal set; }

        [HisComplexAreaInfoMapping(46, ListCount = 5)]
        public List<MesiFS> LISTMesiFS { get; internal set; }

        [HisComplexAreaInfoMapping(47, ListCount = 5)]
        public List<GiorniFS> LISTGiorniFS { get; internal set; }

        // <summary>
        /// FL_DATA_SW X(01)  
        /// </summary>
        [HisFieldInfoMapping(48, 1)]
        public string FL_DATA_SW { get; set; }

        // *                                  (0 = NESSUNA SCRITTA)   17900
        // *                                  (1 = PENS.PROPOSTA PER)
        // *                                  (2 = PENS.PROPOSTA DA)
        /// <summary>
        /// SEDE_DATA_SW X(22)  
        /// </summary>
        [HisFieldInfoMapping(49, 22)]
        public string SEDE_DATA_SW { get; set; }

        // *                                  (NOME SEDE IN CHIARO)   17901
        // 02 SEDE-N-SW.
        /// <summary>
        /// SEDE_SW 9(04)  
        /// </summary>
        [HisFieldInfoMapping(50, 4, CobolType = CobolType.Unsigned)]
        public short SEDE_SW { get; set; }

        /// <summary>
        /// CO_SW 9(02)  
        /// </summary>
        [HisFieldInfoMapping(51, 2, CobolType = CobolType.Unsigned)]
        public short CO_SW { get; set; }

        // *                                  (COD.SEDE COMPETENTE)   17923
        /// <summary>
        /// CAUSALE X(71)  
        /// </summary>
        [HisFieldInfoMapping(52, 71)]
        public string CAUSALE { get; set; }

        // *                                                          17929
        /// <summary>
        /// FILLER X(2000)  
        /// </summary>
        [HisFieldInfoMapping(53, 2000)]
        public string FILLER7 { get; set; }

        /// <summary>
        /// FILLER X(9000)  
        /// </summary>
        [HisFieldInfoMapping(54, 9000)]
        public string FILLER8 { get; set; }

        /// <summary>
        /// FILLER X(5518)  
        /// </summary>
        [HisFieldInfoMapping(55, 5518)]
        public string FILLER9 { get; set; }

        // *                                                          32518
        #endregion Tracciato Host

        #region nested class
        public class AnniFS
        {
            #region Constructor
            internal AnniFS()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 ANNI-FS          PIC 9(02) OCCURS 5 TIMES.
            //*                                                          17870
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// ANNI_FS 9(02)  
            /// </summary>
            [HisFieldInfoMapping(0, 2, CobolType = CobolType.Unsigned)]
            public short ANNI_FS { get; set; }

            // *                                                          17870
            #endregion Tracciato Host

            #endregion Properties
        }

        public class MesiFS
        {
            #region Constructor
            internal MesiFS()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 MESI-FS          PIC 9(02) OCCURS 5 TIMES.
            //*                                                          15880
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// MESI_FS 9(02)  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short MESI_FS { get; set; }

            // *                                                          15880
            #endregion Tracciato Host

            #endregion Properties
        }

        public class GiorniFS
        {
            #region Constructor
            internal GiorniFS()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 GIOR-FS          PIC 9(02) OCCURS 5 TIMES.
            //*                                                          17890
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// GIOR_FS 9(02)  
            /// </summary>
            [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
            public short GIOR_FS { get; set; }

            // *                                                          17890
            #endregion Tracciato Host

            #endregion Properties
        }

        #endregion nested class

        #endregion Properties
    }
}

