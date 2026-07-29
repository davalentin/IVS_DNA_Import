using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class Istruttoria
    {
        #region Properties

        #region Tracciato COBOL
        //        *DATI DEL PANNELLO MRCAN30 (INFORMAZIONI ISTRUTTORIE 1)
        //     02 T-GPAN30.
        //        03 T-GP1AJ01-V            PIC 9.
        //        03 T-GP1AF02              PIC X(3).
        //        03 T-GP1AD00.
        //        04 T-GP1AD01.
        //           05 T-GP1AD01A          PIC 9(4).
        //           05 T-GP1AD01M          PIC 9(2).
        //        04 T-GP1AD02           PIC 9(2).
        //        03 T-GP1AD00-O-V.
        //        04 T-GP1AD01-O-V.
        //           05 T-GP1AD01-OA-V      PIC 9(4).
        //           05 T-GP1AD01-OM-V      PIC 9(2).
        //        04 T-GP1AD02-V         PIC 9(2).
        //        03 T-GP1AT03.
        //           04 T-GP1AT03A          PIC 9(4).
        //           04 T-GP1AT03M          PIC 9(2).
        //        03 T-GP1AJ11              PIC X.
        //        03 T-GP1AF01.
        //           04 T-GP1AF01G          PIC 9(2).
        //           04 T-GP1AF01M          PIC 9(2).
        //           04 T-GP1AF01A          PIC 9(4).
        //        03 T-GP1AG02.
        //           04 T-GP1AG02G          PIC 9(2).
        //           04 T-GP1AG02M          PIC 9(2).
        //           04 T-GP1AG02A          PIC 9(4).
        //        03 T-GP1AG03.
        //           04 T-GP1AG03A          PIC 9(4).
        //           04 T-GP1AG03M          PIC 9(2).
        //        03 T-GP1AN07              PIC 9.
        //        03 T-GP1CPOSLVR           PIC X(2).
        //        03 T-GP1AJ03              PIC 9.
        //        03 T-GP2BG10 OCCURS 4.
        //           04 T-GP2BG11-V         PIC X(2).
        //           04 T-GP2BG12-V.
        //              05 T-GP2BG12A-V     PIC 9(4).
        //              05 T-GP2BG12M-V     PIC 9(2).
        //           04 T-GP2BG13-V.
        //              05 T-GP2BG13A-V     PIC 9(4).
        //              05 T-GP2BG13M-V     PIC 9(2).
        //        03 T-GP1AXA4-V.
        //           04 T-GP1AXA4A-V        PIC 9(4).
        //           04 T-GP1AXA4M-V        PIC 9(2).
        //        03 T-GP1AF03-V            PIC X.
        //        03 T-GP2BN53.
        //           04 T-GP2BN53A          PIC 9(4).
        //           04 T-GP2BN53M          PIC 9(2).
        //        03 T-GP1AV04              PIC 9(3) COMP-3.
        //        03 T-GP1AV05              PIC 9(3) COMP-3.
        //        03 T-TP1CLIV.
        //           04 T-GP1AV06           PIC 9(3).
        //           04 T-GP1AV07           PIC 9(3).
        //        03 T-GP1AZ11F             PIC 9.
        //        03 T-TP1ILEGR.
        //           04 T-TP1ILEGG          PIC 9(2).
        //           04 T-TP1ILEGM          PIC 9(2).
        //           04 T-TP1ILEGA          PIC 9(4).
        //        03 T-GP1AJ05              PIC 9.
        //        03 T-TP1NOARC             PIC X.
        //*DATI ESODATI
        //        03 T-TP1ESODATI.
        //           04 T-TP1CONCORR-V      PIC X.
        //           04 T-GP1CENTCRD-V      PIC 9(4).
        //           04 T-GP1ALA1-V         PIC 9(2)V9(2).
        //           04 T-GP1AXB8-V         PIC S9(7)V9(4) COMP-3.
        //*DATI DEL PANNELLO MRCAN31 (INFORMAZIONI ISTRUTTORIE 2)
        //     02 T-GPAN31.
        //        03 T-GP2BM01.
        //           04 T-GP2BM01G          PIC 9(2).
        //           04 T-GP2BM01M          PIC 9(2).
        //           04 T-GP2BM01A          PIC 9(4).
        //        03 T-GP2BM02.
        //           04 T-GP2BM02G          PIC 9(2).
        //           04 T-GP2BM02M          PIC 9(2).
        //           04 T-GP2BM02A          PIC 9(4).
        //        03 T-GP2BN02              PIC 9(5) COMP-3.
        //        03 T-GP1AV08              PIC 9(5) COMP-3.
        //        03 T-GP1AV09              PIC 9(5) COMP-3.
        //        03 T-GP1AV10              PIC 9(5) COMP-3.
        //        03 T-GP1AF08              PIC 9(2).
        //        03 T-GP1AXF1              PIC 9(3) COMP-3.
        //        03 T-GP1AXF2              PIC 9(3) COMP-3.
        //        03 T-GP1NSETBEN           PIC S9(5) COMP-3.
        //        03 T-GP1AV61              PIC X(2).
        //        03 T-GP1FREQPAR           PIC X.
        //        03 T-GP1MFINBEN           PIC X(6).
        //        03 T-GP1AXF3              PIC X.
        //        03 T-TP1REQRID1.
        //           04 T-GP1AP47           PIC 9.
        //           04 T-GP1AP49           PIC 9.
        //        03 T-GP1AF06-V.
        //           04 T-GP1AF06A-V        PIC 9(4).
        //           04 T-GP1AF06M-V        PIC 9(2).
        //        03 T-GP1AV71-V.
        //           04 T-GP1AV71A-V        PIC 9(4).
        //           04 T-GP1AV71M-V        PIC 9(2).
        //        03 T-GP1AV72-V            PIC 9(2).
        //        03 T-GP1AXE3              PIC 9.
        //        03 T-GP2BM03Z.
        //           04 T-GP2BM03G          PIC 9(2).
        //           04 T-GP2BM03M          PIC 9(2).
        //           04 T-GP2BM03A          PIC 9(4).
        //*DATI DEL PANNELLO MRCAN32 (INFORMAZIONI ISTRUTTORIE 3)
        //     02 T-GPAN32.
        //        03 T-GP1AK03-V            PIC 9(7)V9(4) COMP-3.
        //        03 T-GP1AK04-V            PIC 9(7)V9(4) COMP-3.
        //        03 T-TP1ASSINV.
        //           04 T-GP2INV1-V         PIC S9(7)V9(4) COMP-3.
        //           04 T-GP2INV2-V         PIC S9(7)V9(4) COMP-3.
        //           04 T-GP2INV3-V         PIC S9(7)V9(4) COMP-3.
        //        03 T-GP1AV51.
        //           04 T-GP1AV51A          PIC 9(4).
        //           04 T-GP1AV51M          PIC 9(2).
        //        03 T-GP1AV53              PIC 9(3).
        //        03 T-GP1AV54              PIC 9(4).
        //        03 T-GP1AV55              PIC 9(8).
        //        03 T-GP1AZ11A-V           PIC 9.
        //        03 T-GP1AZ11B-V           PIC 9.
        //        03 T-GP1AZ11C-V           PIC 9.
        //        03 T-GP1AZ11D-V           PIC 9.
        //        03 T-GP2BL20-V            PIC X.
        //        03 T-GP5KE06-V            PIC X.
        //        03 T-GP6KE06-V            PIC X.
        //        03 T-TP1GPVARI.
        //           04 T-GP1AJ02           PIC 9.
        //           04 T-TP1DETR-V.
        //              05 T-GP3CDTI-V      PIC 9(14).
        //              05 T-GP3DDTIVRC-V.
        //                 06 T-GP3DDTIVRCA-V   PIC 9(4).
        //                 06 T-GP3DDTIVRCM-V   PIC 99.
        //                 06 T-GP3DDTIVRCG-V   PIC 99.
        //           04 T-GP1AV01           PIC 9(2).
        //           04 T-GP1AV02           PIC X.
        //           04 T-GP1AZ11E-V        PIC X.
        //           04 T-GP1AV91I          PIC 9.
        //           04 T-GP1AJ08-V         PIC 9.
        //           04 T-GP1AV11-V         PIC 9(2).
        //           04 T-TP1COLIQ          PIC X.
        //           04 T-TP1MENT           PIC 9(2).
        //           04 T-GP1AN06.
        //              05 T-GP1AN06G       PIC 9(2).
        //              05 T-GP1AN06M       PIC 9(2).
        //              05 T-GP1AN06A       PIC 9(4).
        //           04 T-TP1REQRID.
        //              05 T-GP1ALB1        PIC 9(7) COMP-3.
        //              05 T-GP1ALB2        PIC 9(7) COMP-3.
        //           04 T-TP1SENT           PIC 9.
        #endregion Tracciato COBOL

        #region Tracciato Host
        // *DATI DEL PANNELLO MRCAN30 (INFORMAZIONI ISTRUTTORIE 1)
        // 02 T-GPAN30.
        /// <summary>
        /// T_GP1AJ01_V 9  
        /// </summary>
        [HisFieldInfoMapping(0, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1AJ01_V { get; set; }

        /// <summary>
        /// T_GP1AF02 X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string T_GP1AF02 { get; set; }

        // 04 T-GP1AD01.
        /// <summary>
        /// T_GP1AD01A 9(4)  
        /// </summary>
        [HisFieldInfoMapping(2, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1AD01A { get; set; }

        /// <summary>
        /// T_GP1AD01M 9(2)  
        /// </summary>
        [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AD01M { get; set; }

        /// <summary>
        /// T_GP1AD02 9(2)  
        /// </summary>
        [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AD02 { get; set; }

        // 04 T-GP1AD01-O-V.
        /// <summary>
        /// T_GP1AD01_OA_V 9(4)  
        /// </summary>
        [HisFieldInfoMapping(5, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1AD01_OA_V { get; set; }

        /// <summary>
        /// T_GP1AD01_OM_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(6, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AD01_OM_V { get; set; }
        
        /// <summary>
        /// T_GP1AD02_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AD02_V { get; set; }

        // 03 T-GP1AT03.
        /// <summary>
        /// T_GP1AT03A 9(4)  
        /// </summary>
        [HisFieldInfoMapping(8, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1AT03A { get; set; }

        /// <summary>
        /// T_GP1AT03M 9(2)  
        /// </summary>
        [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AT03M { get; set; }

        /// <summary>
        /// T_GP1AJ11 X  
        /// </summary>
        [HisFieldInfoMapping(10, 1)]
        public string T_GP1AJ11 { get; set; }

        // 03 T-GP1AF01.
        /// <summary>
        /// T_GP1AF01G 9(2)  
        /// </summary>
        [HisFieldInfoMapping(11, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AF01G { get; set; }

        /// <summary>
        /// T_GP1AF01M 9(2)  
        /// </summary>
        [HisFieldInfoMapping(12, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AF01M { get; set; }

        /// <summary>
        /// T_GP1AF01A 9(4)  
        /// </summary>
        [HisFieldInfoMapping(13, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1AF01A { get; set; }

        // 03 T-GP1AG02.
        /// <summary>
        /// T_GP1AG02G 9(2)  
        /// </summary>
        [HisFieldInfoMapping(14, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AG02G { get; set; }

        /// <summary>
        /// T_GP1AG02M 9(2)  
        /// </summary>
        [HisFieldInfoMapping(15, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AG02M { get; set; }

        /// <summary>
        /// T_GP1AG02A 9(4)  
        /// </summary>
        [HisFieldInfoMapping(16, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1AG02A { get; set; }

        // 03 T-GP1AG03.
        /// <summary>
        /// T_GP1AG03A 9(4)  
        /// </summary>
        [HisFieldInfoMapping(17, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1AG03A { get; set; }

        /// <summary>
        /// T_GP1AG03M 9(2)  
        /// </summary>
        [HisFieldInfoMapping(18, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AG03M { get; set; }

        /// <summary>
        /// T_GP1AN07 9  
        /// </summary>
        [HisFieldInfoMapping(19, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1AN07 { get; set; }

        /// <summary>
        /// T_GP1CPOSLVR X(2)  
        /// </summary>
        [HisFieldInfoMapping(20, 2)]
        public string T_GP1CPOSLVR { get; set; }

        /// <summary>
        /// T_GP1AJ03 9  
        /// </summary>
        [HisFieldInfoMapping(21, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1AJ03 { get; set; }

        [HisComplexAreaInfoMapping(22, ListCount = 4)]
        public List<T_GP2BG10> LISTT_GP2BG10 { get; set; }

        // 03 T-GP1AXA4-V.
        /// <summary>
        /// T_GP1AXA4A_V 9(4)  
        /// </summary>
        [HisFieldInfoMapping(23, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1AXA4A_V { get; set; }

        /// <summary>
        /// T_GP1AXA4M_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(24, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AXA4M_V { get; set; }

        /// <summary>
        /// T_GP1AF03_V X  
        /// </summary>
        [HisFieldInfoMapping(25, 1)]
        public string T_GP1AF03_V { get; set; }

        // 03 T-GP2BN53.
        /// <summary>
        /// T_GP2BN53A 9(4)  
        /// </summary>
        [HisFieldInfoMapping(26, 4, CobolType = CobolType.Unsigned)]
        public short T_GP2BN53A { get; set; }

        /// <summary>
        /// T_GP2BN53M 9(2)  
        /// </summary>
        [HisFieldInfoMapping(27, 2, CobolType = CobolType.Unsigned)]
        public short T_GP2BN53M { get; set; }

        /// <summary>
        /// T_GP1AV04 9(3) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(28, 2, CobolType = CobolType.Comp3Unsigned)]
        public int T_GP1AV04 { get; set; }

        /// <summary>
        /// T_GP1AV05 9(3) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(29, 2, CobolType = CobolType.Comp3Unsigned)]
        public int T_GP1AV05 { get; set; }

        // 03 T-TP1CLIV.
        /// <summary>
        /// T_GP1AV06 9(3)  
        /// </summary>
        [HisFieldInfoMapping(30, 3, CobolType = CobolType.Unsigned)]
        public short T_GP1AV06 { get; set; }

        /// <summary>
        /// T_GP1AV07 9(3)  
        /// </summary>
        [HisFieldInfoMapping(31, 3, CobolType = CobolType.Unsigned)]
        public short T_GP1AV07 { get; set; }

        /// <summary>
        /// T_GP1AZ11F 9  
        /// </summary>
        [HisFieldInfoMapping(32, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1AZ11F { get; set; }

        // 03 T-TP1ILEGR.
        /// <summary>
        /// T_TP1ILEGG 9(2)  
        /// </summary>
        [HisFieldInfoMapping(33, 2, CobolType = CobolType.Unsigned)]
        public short T_TP1ILEGG { get; set; }

        /// <summary>
        /// T_TP1ILEGM 9(2)  
        /// </summary>
        [HisFieldInfoMapping(34, 2, CobolType = CobolType.Unsigned)]
        public short T_TP1ILEGM { get; set; }

        /// <summary>
        /// T_TP1ILEGA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(35, 4, CobolType = CobolType.Unsigned)]
        public short T_TP1ILEGA { get; set; }

        /// <summary>
        /// T_GP1AJ05 9  
        /// </summary>
        [HisFieldInfoMapping(36, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1AJ05 { get; set; }

        /// <summary>
        /// T_TP1NOARC X  
        /// </summary>
        [HisFieldInfoMapping(37, 1)]
        public string T_TP1NOARC { get; set; }

        // *DATI ESODATI
        // 03 T-TP1ESODATI.
        /// <summary>
        /// T_TP1CONCORR_V X  
        /// </summary>
        [HisFieldInfoMapping(38, 1)]
        public string T_TP1CONCORR_V { get; set; }

        /// <summary>
        /// T_GP1CENTCRD_V 9(4)  
        /// </summary>
        [HisFieldInfoMapping(39, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1CENTCRD_V { get; set; }

        /// <summary>
        /// T_GP1ALA1_V 9(2)V9(2)  
        /// </summary>
        [HisFieldInfoMapping(40, 4, Scale = 2, CobolType = CobolType.Unsigned)]
        public decimal T_GP1ALA1_V { get; set; }

        /// <summary>
        /// T_GP1AXB8_V S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(41, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal T_GP1AXB8_V { get; set; }

        // *DATI DEL PANNELLO MRCAN31 (INFORMAZIONI ISTRUTTORIE 2)
        // 02 T-GPAN31.
        // 03 T-GP2BM01.
        /// <summary>
        /// T_GP2BM01G 9(2)  
        /// </summary>
        [HisFieldInfoMapping(42, 2, CobolType = CobolType.Unsigned)]
        public short T_GP2BM01G { get; set; }

        /// <summary>
        /// T_GP2BM01M 9(2)  
        /// </summary>
        [HisFieldInfoMapping(43, 2, CobolType = CobolType.Unsigned)]
        public short T_GP2BM01M { get; set; }

        /// <summary>
        /// T_GP2BM01A 9(4)  
        /// </summary>
        [HisFieldInfoMapping(44, 4, CobolType = CobolType.Unsigned)]
        public short T_GP2BM01A { get; set; }

        // 03 T-GP2BM02.
        /// <summary>
        /// T_GP2BM02G 9(2)  
        /// </summary>
        [HisFieldInfoMapping(45, 2, CobolType = CobolType.Unsigned)]
        public short T_GP2BM02G { get; set; }

        /// <summary>
        /// T_GP2BM02M 9(2)  
        /// </summary>
        [HisFieldInfoMapping(46, 2, CobolType = CobolType.Unsigned)]
        public short T_GP2BM02M { get; set; }

        /// <summary>
        /// T_GP2BM02A 9(4)  
        /// </summary>
        [HisFieldInfoMapping(47, 4, CobolType = CobolType.Unsigned)]
        public short T_GP2BM02A { get; set; }

        /// <summary>
        /// T_GP2BN02 9(5) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(48, 3, CobolType = CobolType.Comp3Unsigned)]
        public int T_GP2BN02 { get; set; }

        /// <summary>
        /// T_GP1AV08 9(5) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(49, 3, CobolType = CobolType.Comp3Unsigned)]
        public int T_GP1AV08 { get; set; }

        /// <summary>
        /// T_GP1AV09 9(5) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(50, 3, CobolType = CobolType.Comp3Unsigned)]
        public int T_GP1AV09 { get; set; }

        /// <summary>
        /// T_GP1AV10 9(5) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(51, 3, CobolType = CobolType.Comp3Unsigned)]
        public int T_GP1AV10 { get; set; }

        /// <summary>
        /// T_GP1AF08 9(2)  
        /// </summary>
        [HisFieldInfoMapping(52, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AF08 { get; set; }

        /// <summary>
        /// T_GP1AXF1 9(3) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(53, 2, CobolType = CobolType.Comp3Unsigned)]
        public int T_GP1AXF1 { get; set; }

        /// <summary>
        /// T_GP1AXF2 9(3) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(54, 2, CobolType = CobolType.Comp3Unsigned)]
        public int T_GP1AXF2 { get; set; }

        /// <summary>
        /// T_GP1NSETBEN S9(5) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(55, 3, CobolType = CobolType.Comp3)]
        public int T_GP1NSETBEN { get; set; }

        /// <summary>
        /// T_GP1AV61 X(2)  
        /// </summary>
        [HisFieldInfoMapping(56, 2)]
        public string T_GP1AV61 { get; set; }

        /// <summary>
        /// T_GP1FREQPAR X  
        /// </summary>
        [HisFieldInfoMapping(57, 1)]
        public string T_GP1FREQPAR { get; set; }

        /// <summary>
        /// T_GP1MFINBEN X(6)  
        /// </summary>
        [HisFieldInfoMapping(58, 6)]
        public string T_GP1MFINBEN { get; set; }

        /// <summary>
        /// T_GP1AXF3 X  
        /// </summary>
        [HisFieldInfoMapping(59, 1)]
        public string T_GP1AXF3 { get; set; }

        // 03 T-TP1REQRID1.
        /// <summary>
        /// T_GP1AP47 9  
        /// </summary>
        [HisFieldInfoMapping(60, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1AP47 { get; set; }

        /// <summary>
        /// T_GP1AP49 9  
        /// </summary>
        [HisFieldInfoMapping(61, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1AP49 { get; set; }

        // 03 T-GP1AF06-V.
        /// <summary>
        /// T_GP1AF06A_V 9(4)  
        /// </summary>
        [HisFieldInfoMapping(62, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1AF06A_V { get; set; }

        /// <summary>
        /// T_GP1AF06M_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(63, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AF06M_V { get; set; }

        // 03 T-GP1AV71-V.
        /// <summary>
        /// T_GP1AV71A_V 9(4)  
        /// </summary>
        [HisFieldInfoMapping(64, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1AV71A_V { get; set; }

        /// <summary>
        /// T_GP1AV71M_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(65, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AV71M_V { get; set; }

        /// <summary>
        /// T_GP1AV72_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(66, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AV72_V { get; set; }

        /// <summary>
        /// T_GP1AXE3 9  
        /// </summary>
        [HisFieldInfoMapping(67, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1AXE3 { get; set; }

        // 03 T-GP2BM03Z.
        /// <summary>
        /// T_GP2BM03G 9(2)  
        /// </summary>
        [HisFieldInfoMapping(68, 2, CobolType = CobolType.Unsigned)]
        public short T_GP2BM03G { get; set; }

        /// <summary>
        /// T_GP2BM03M 9(2)  
        /// </summary>
        [HisFieldInfoMapping(69, 2, CobolType = CobolType.Unsigned)]
        public short T_GP2BM03M { get; set; }

        /// <summary>
        /// T_GP2BM03A 9(4)  
        /// </summary>
        [HisFieldInfoMapping(70, 4, CobolType = CobolType.Unsigned)]
        public short T_GP2BM03A { get; set; }

        // *DATI DEL PANNELLO MRCAN32 (INFORMAZIONI ISTRUTTORIE 3)
        // 02 T-GPAN32.
        /// <summary>
        /// T_GP1AK03_V 9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(71, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal T_GP1AK03_V { get; set; }

        /// <summary>
        /// T_GP1AK04_V 9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(72, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal T_GP1AK04_V { get; set; }

        // 03 T-TP1ASSINV.
        /// <summary>
        /// T_GP2INV1_V S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(73, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal T_GP2INV1_V { get; set; }

        /// <summary>
        /// T_GP2INV2_V S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(74, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal T_GP2INV2_V { get; set; }

        /// <summary>
        /// T_GP2INV3_V S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(75, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal T_GP2INV3_V { get; set; }

        // 03 T-GP1AV51.
        /// <summary>
        /// T_GP1AV51A 9(4)  
        /// </summary>
        [HisFieldInfoMapping(76, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1AV51A { get; set; }

        /// <summary>
        /// T_GP1AV51M 9(2)  
        /// </summary>
        [HisFieldInfoMapping(77, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AV51M { get; set; }

        /// <summary>
        /// T_GP1AV53 9(3)  
        /// </summary>
        [HisFieldInfoMapping(78, 3, CobolType = CobolType.Unsigned)]
        public short T_GP1AV53 { get; set; }

        /// <summary>
        /// T_GP1AV54 9(4)  
        /// </summary>
        [HisFieldInfoMapping(79, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1AV54 { get; set; }

        /// <summary>
        /// T_GP1AV55 9(8)  
        /// </summary>
        [HisFieldInfoMapping(80, 8, CobolType = CobolType.Unsigned)]
        public int T_GP1AV55 { get; set; }

        /// <summary>
        /// T_GP1AZ11A_V 9  
        /// </summary>
        [HisFieldInfoMapping(81, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1AZ11A_V { get; set; }

        /// <summary>
        /// T_GP1AZ11B_V 9  
        /// </summary>
        [HisFieldInfoMapping(82, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1AZ11B_V { get; set; }

        /// <summary>
        /// T_GP1AZ11C_V 9  
        /// </summary>
        [HisFieldInfoMapping(83, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1AZ11C_V { get; set; }

        /// <summary>
        /// T_GP1AZ11D_V 9  
        /// </summary>
        [HisFieldInfoMapping(84, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1AZ11D_V { get; set; }

        /// <summary>
        /// T_GP2BL20_V X  
        /// </summary>
        [HisFieldInfoMapping(85, 1)]
        public string T_GP2BL20_V { get; set; }

        /// <summary>
        /// T_GP5KE06_V X  
        /// </summary>
        [HisFieldInfoMapping(86, 1)]
        public string T_GP5KE06_V { get; set; }

        /// <summary>
        /// T_GP6KE06_V X  
        /// </summary>
        [HisFieldInfoMapping(87, 1)]
        public string T_GP6KE06_V { get; set; }

        // 03 T-TP1GPVARI.
        /// <summary>
        /// T_GP1AJ02 9  
        /// </summary>
        [HisFieldInfoMapping(88, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1AJ02 { get; set; }

        // 04 T-TP1DETR-V.
        /// <summary>
        /// T_GP3CDTI_V 9(14)  
        /// </summary>
        [HisFieldInfoMapping(89, 14, CobolType = CobolType.Unsigned)]
        public long T_GP3CDTI_V { get; set; }

        // 05 T-GP3DDTIVRC-V.
        /// <summary>
        /// T_GP3DDTIVRCA_V 9(4)  
        /// </summary>
        [HisFieldInfoMapping(90, 4, CobolType = CobolType.Unsigned)]
        public short T_GP3DDTIVRCA_V { get; set; }

        /// <summary>
        /// T_GP3DDTIVRCM_V 99  
        /// </summary>
        [HisFieldInfoMapping(91, 2, CobolType = CobolType.Unsigned)]
        public short T_GP3DDTIVRCM_V { get; set; }

        /// <summary>
        /// T_GP3DDTIVRCG_V 99  
        /// </summary>
        [HisFieldInfoMapping(92, 2, CobolType = CobolType.Unsigned)]
        public short T_GP3DDTIVRCG_V { get; set; }

        /// <summary>
        /// T_GP1AV01 9(2)  
        /// </summary>
        [HisFieldInfoMapping(93, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AV01 { get; set; }

        /// <summary>
        /// T_GP1AV02 X  
        /// </summary>
        [HisFieldInfoMapping(94, 1)]
        public string T_GP1AV02 { get; set; }

        /// <summary>
        /// T_GP1AZ11E_V X  
        /// </summary>
        [HisFieldInfoMapping(95, 1)]
        public string T_GP1AZ11E_V { get; set; }

        /// <summary>
        /// T_GP1AV91I 9  
        /// </summary>
        [HisFieldInfoMapping(96, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1AV91I { get; set; }

        /// <summary>
        /// T_GP1AJ08_V 9  
        /// </summary>
        [HisFieldInfoMapping(97, 1, CobolType = CobolType.Unsigned)]
        public short T_GP1AJ08_V { get; set; }

        /// <summary>
        /// T_GP1AV11_V 9(2)  
        /// </summary>
        [HisFieldInfoMapping(98, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AV11_V { get; set; }

        /// <summary>
        /// T_TP1COLIQ X  
        /// </summary>
        [HisFieldInfoMapping(99, 1)]
        public string T_TP1COLIQ { get; set; }

        /// <summary>
        /// T_TP1MENT 9(2)  
        /// </summary>
        [HisFieldInfoMapping(100, 2, CobolType = CobolType.Unsigned)]
        public short T_TP1MENT { get; set; }

        // 04 T-GP1AN06.
        /// <summary>
        /// T_GP1AN06G 9(2)  
        /// </summary>
        [HisFieldInfoMapping(101, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AN06G { get; set; }

        /// <summary>
        /// T_GP1AN06M 9(2)  
        /// </summary>
        [HisFieldInfoMapping(102, 2, CobolType = CobolType.Unsigned)]
        public short T_GP1AN06M { get; set; }

        /// <summary>
        /// T_GP1AN06A 9(4)  
        /// </summary>
        [HisFieldInfoMapping(103, 4, CobolType = CobolType.Unsigned)]
        public short T_GP1AN06A { get; set; }

        // 04 T-TP1REQRID.
        /// <summary>
        /// T_GP1ALB1 9(7) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(104, 4, CobolType = CobolType.Comp3Unsigned)]
        public int T_GP1ALB1 { get; set; }

        /// <summary>
        /// T_GP1ALB2 9(7) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(105, 4, CobolType = CobolType.Comp3Unsigned)]
        public int T_GP1ALB2 { get; set; }

        /// <summary>
        /// T_TP1SENT 9  
        /// </summary>
        [HisFieldInfoMapping(106, 1, CobolType = CobolType.Unsigned)]
        public short T_TP1SENT { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class T_GP2BG10
        {
            #region Properties

            #region Tracciato COBOL
            //        03 T-GP2BG10 OCCURS 4.
            //04 T-GP2BG11-V         PIC X(2).
            //04 T-GP2BG12-V.
            //   05 T-GP2BG12A-V     PIC 9(4).
            //   05 T-GP2BG12M-V     PIC 9(2).
            //04 T-GP2BG13-V.
            //   05 T-GP2BG13A-V     PIC 9(4).
            //   05 T-GP2BG13M-V     PIC 9(2).
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 03 T-GP2BG10 OCCURS 4.
            /// <summary>
            /// T_GP2BG11_V X(2)  
            /// </summary>
            [HisFieldInfoMapping(0, 2)]
            public string T_GP2BG11_V { get; set; }

            // 04 T-GP2BG12-V.
            /// <summary>
            /// T_GP2BG12A_V 9(4)  
            /// </summary>
            [HisFieldInfoMapping(1, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BG12A_V { get; set; }

            /// <summary>
            /// T_GP2BG12M_V 9(2)  
            /// </summary>
            [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BG12M_V { get; set; }

            // 04 T-GP2BG13-V.
            /// <summary>
            /// T_GP2BG13A_V 9(4)  
            /// </summary>
            [HisFieldInfoMapping(3, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BG13A_V { get; set; }

            /// <summary>
            /// T_GP2BG13M_V 9(2)  
            /// </summary>
            [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BG13M_V { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
