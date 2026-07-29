using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago
{
    public class PM : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  YPM-RECAGO-BIS REDEFINES COMUNE-RECAGO.
        //          02  YPM-RECAGO.
        //              03 YPMTIPOR                      PIC X.
        //              03 YPMFONDO                      PIC X(3).
        //              03 YPMTIPEN                      PIC 9.
        //              03 YPMTIPLQ                      PIC 9.
        //D2NEW         03 YPMDECAA                      PIC 9999.                
        //              03 YPMDECMM                      PIC 99.
        //D2NEW         03 YPMSCAAA                      PIC 9999.                
        //              03 YPMSCAMM                      PIC 99.
        //D2NEW         03 YPMTEOAA                      PIC 9999.                
        //              03 YPMTEOMM                      PIC 99.
        //              03 YPMRETPN                      PIC 9(6)V9999.
        //              03 YPMANZTO                      PIC 9(5).
        //              03 YPMANZES                      PIC 9(5).
        //              03 YPMSETVV                      PIC 9(5).
        //              03 YPMCTRTO                      PIC 9(5)V9999.
        //              03 YPMCTRES                      PIC 9(5)V9999.
        //              03 YPMC14TO                      PIC 9(3)V9999.
        //              03 YPMC14ES                      PIC 9(3)V9999.
        //              03 YPMC11TO                      PIC 9(3)V9999.
        //              03 YPMC11ES                      PIC 9(4)V9999.
        //              03 YPMANNIR                      PIC 9(3).
        //              03 YPMETAMA                      PIC 99.
        //D2000         03 YPMDPCDC.
        //D2NEW            04 YPMCDCAA                   PIC 9999.                
        //                 04 YPMCDCMM                   PIC 99.
        //              03 YPMDPCRT                      PIC 9(5)V9999.
        //              03 YPMS72RT                      PIC 9(5)V9999.
        //D2NEW         03 YPMCB140                      PIC 9(6).                
        //      *-503
        //              03 YPMSPECI                      PIC X.
        //              03 YPMRETP1                      PIC 9(6)V9999.
        //              03 YPMANZT1                      PIC 9(4).
        //              03 YPMANZE1                      PIC 9(4).
        //              03 YPM503ET                      PIC X.
        //              03 YPM503AS                      PIC 9.
        //      *503
        //              03 YPMTPCOD                      PIC X.
        //              03 YPMDECSS                      PIC 99.
        //              03 YPMSOSSS                      PIC 99.
        //              03 YPMAUTON                      PIC XX.
        //      * - 233
        //              03 YPMCOULT                      PIC 9.
        //              03 YPMCODCD                      PIC 9.
        //              03 YPMIVSCD                      PIC 9(5)V9999.
        //              03 YPMRCDA                       PIC 9(6)V9999.
        //              03 YPMACDA                       PIC 9(4).
        //              03 YPMRCDB                       PIC 9(6)V9999.
        //              03 YPMACDB                       PIC 9(4).
        //              03 YPMATCD                       PIC 9(4).
        //              03 YPMCODAR                      PIC 9.
        //              03 YPMIVSAR                      PIC 9(5)V9999.
        //              03 YPMRARTA                      PIC 9(6)V9999.
        //              03 YPMAARTA                      PIC 9(4).
        //              03 YPMRARTB                      PIC 9(6)V9999.
        //              03 YPMAARTB                      PIC 9(4).
        //              03 YPMATART                      PIC 9(4).
        //              03 YPMCODCO                      PIC 9.
        //              03 YPMIVSCO                      PIC 9(5)V9999.
        //              03 YPMRCOMA                      PIC 9(6)V9999.
        //              03 YPMACOMA                      PIC 9(4).
        //              03 YPMRCOMB                      PIC 9(6)V9999.
        //              03 YPMACOMB                      PIC 9(4).
        //              03 YPMATCOM                      PIC 9(4).
        //              03 YPMMONTA                      PIC 9(7)V9999.
        //              03 YPMESCLU                      PIC 9(7)V9999.
        //GD1109        03 YPMSETTE                      PIC 9(4).  
        //GD0212        03 YPMIMPCRT                     PIC 9(6)V9999.  
        //GD0212        03 YPMMONTA2012                  PIC 9(7)V9999.      
        //GD0212        03 YPMSETT2012                   PIC 9(4). 
        //GD0517        03 YPMSETA-707                   PIC 9(4).
        //              03 YPMSETB-707                   PIC 9(4).
        //              03 YPMCALC707                    PIC X(01).   
        //              03 YPMPROGR                      PIC 99.
        //           02 YPMDISPO                         PIC X(56).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  YPM-RECAGO-BIS REDEFINES COMUNE-RECAGO.
        // 02  YPM-RECAGO.
        /// <summary>
        /// YPMTIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string YPMTIPOR { get; set; }

        /// <summary>
        /// YPMFONDO X(3)  
        /// </summary>
        [HisFieldInfoMapping(1, 3)]
        public string YPMFONDO { get; set; }

        /// <summary>
        /// YPMTIPEN 9  
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short YPMTIPEN { get; set; }

        /// <summary>
        /// YPMTIPLQ 9  
        /// </summary>
        [HisFieldInfoMapping(3, 1, CobolType = CobolType.Unsigned)]
        public short YPMTIPLQ { get; set; }

        /// <summary>
        /// YPMDECAA 9999  
        /// </summary>
        [HisFieldInfoMapping(4, 4, CobolType = CobolType.Unsigned)]
        public short YPMDECAA { get; set; }

        /// <summary>
        /// YPMDECMM 99  
        /// </summary>
        [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
        public short YPMDECMM { get; set; }

        /// <summary>
        /// YPMSCAAA 9999  
        /// </summary>
        [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
        public short YPMSCAAA { get; set; }

        /// <summary>
        /// YPMSCAMM 99  
        /// </summary>
        [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
        public short YPMSCAMM { get; set; }

        /// <summary>
        /// YPMTEOAA 9999  
        /// </summary>
        [HisFieldInfoMapping(8, 4, CobolType = CobolType.Unsigned)]
        public short YPMTEOAA { get; set; }

        /// <summary>
        /// YPMTEOMM 99  
        /// </summary>
        [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
        public short YPMTEOMM { get; set; }

        /// <summary>
        /// YPMRETPN 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(10, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMRETPN { get; set; }

        /// <summary>
        /// YPMANZTO 9(5)  
        /// </summary>
        [HisFieldInfoMapping(11, 5, CobolType = CobolType.Unsigned)]
        public int YPMANZTO { get; set; }

        /// <summary>
        /// YPMANZES 9(5)  
        /// </summary>
        [HisFieldInfoMapping(12, 5, CobolType = CobolType.Unsigned)]
        public int YPMANZES { get; set; }

        /// <summary>
        /// YPMSETVV 9(5)  
        /// </summary>
        [HisFieldInfoMapping(13, 5, CobolType = CobolType.Unsigned)]
        public int YPMSETVV { get; set; }

        /// <summary>
        /// YPMCTRTO 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(14, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMCTRTO { get; set; }

        /// <summary>
        /// YPMCTRES 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(15, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMCTRES { get; set; }

        /// <summary>
        /// YPMC14TO 9(3)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(16, 7, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMC14TO { get; set; }

        /// <summary>
        /// YPMC14ES 9(3)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(17, 7, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMC14ES { get; set; }

        /// <summary>
        /// YPMC11TO 9(3)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(18, 7, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMC11TO { get; set; }

        /// <summary>
        /// YPMC11ES 9(4)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(19, 8, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMC11ES { get; set; }

        /// <summary>
        /// YPMANNIR 9(3)  
        /// </summary>
        [HisFieldInfoMapping(20, 3, CobolType = CobolType.Unsigned)]
        public short YPMANNIR { get; set; }

        /// <summary>
        /// YPMETAMA 99  
        /// </summary>
        [HisFieldInfoMapping(21, 2, CobolType = CobolType.Unsigned)]
        public short YPMETAMA { get; set; }

        // D2000         03 YPMDPCDC.
        /// <summary>
        /// YPMCDCAA 9999  
        /// </summary>
        [HisFieldInfoMapping(22, 4, CobolType = CobolType.Unsigned)]
        public short YPMCDCAA { get; set; }

        /// <summary>
        /// YPMCDCMM 99  
        /// </summary>
        [HisFieldInfoMapping(23, 2, CobolType = CobolType.Unsigned)]
        public short YPMCDCMM { get; set; }

        /// <summary>
        /// YPMDPCRT 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(24, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMDPCRT { get; set; }

        /// <summary>
        /// YPMS72RT 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(25, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMS72RT { get; set; }

        /// <summary>
        /// YPMCB140 9(6)  
        /// </summary>
        [HisFieldInfoMapping(26, 6, CobolType = CobolType.Unsigned)]
        public int YPMCB140 { get; set; }

        // *-503
        /// <summary>
        /// YPMSPECI X  
        /// </summary>
        [HisFieldInfoMapping(27, 1)]
        public string YPMSPECI { get; set; }

        /// <summary>
        /// YPMRETP1 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(28, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMRETP1 { get; set; }

        /// <summary>
        /// YPMANZT1 9(4)  
        /// </summary>
        [HisFieldInfoMapping(29, 4, CobolType = CobolType.Unsigned)]
        public short YPMANZT1 { get; set; }

        /// <summary>
        /// YPMANZE1 9(4)  
        /// </summary>
        [HisFieldInfoMapping(30, 4, CobolType = CobolType.Unsigned)]
        public short YPMANZE1 { get; set; }

        /// <summary>
        /// YPM503ET X  
        /// </summary>
        [HisFieldInfoMapping(31, 1)]
        public string YPM503ET { get; set; }

        /// <summary>
        /// YPM503AS 9  
        /// </summary>
        [HisFieldInfoMapping(32, 1, CobolType = CobolType.Unsigned)]
        public short YPM503AS { get; set; }

        // *503
        /// <summary>
        /// YPMTPCOD X  
        /// </summary>
        [HisFieldInfoMapping(33, 1)]
        public string YPMTPCOD { get; set; }

        /// <summary>
        /// YPMDECSS 99  
        /// </summary>
        [HisFieldInfoMapping(34, 2, CobolType = CobolType.Unsigned)]
        public short YPMDECSS { get; set; }

        /// <summary>
        /// YPMSOSSS 99  
        /// </summary>
        [HisFieldInfoMapping(35, 2, CobolType = CobolType.Unsigned)]
        public short YPMSOSSS { get; set; }

        /// <summary>
        /// YPMAUTON XX  
        /// </summary>
        [HisFieldInfoMapping(36, 2)]
        public string YPMAUTON { get; set; }

        // * - 233
        /// <summary>
        /// YPMCOULT 9  
        /// </summary>
        [HisFieldInfoMapping(37, 1, CobolType = CobolType.Unsigned)]
        public short YPMCOULT { get; set; }

        /// <summary>
        /// YPMCODCD 9  
        /// </summary>
        [HisFieldInfoMapping(38, 1, CobolType = CobolType.Unsigned)]
        public short YPMCODCD { get; set; }

        /// <summary>
        /// YPMIVSCD 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(39, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMIVSCD { get; set; }

        /// <summary>
        /// YPMRCDA 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(40, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMRCDA { get; set; }

        /// <summary>
        /// YPMACDA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(41, 4, CobolType = CobolType.Unsigned)]
        public short YPMACDA { get; set; }

        /// <summary>
        /// YPMRCDB 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(42, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMRCDB { get; set; }

        /// <summary>
        /// YPMACDB 9(4)  
        /// </summary>
        [HisFieldInfoMapping(43, 4, CobolType = CobolType.Unsigned)]
        public short YPMACDB { get; set; }

        /// <summary>
        /// YPMATCD 9(4)  
        /// </summary>
        [HisFieldInfoMapping(44, 4, CobolType = CobolType.Unsigned)]
        public short YPMATCD { get; set; }

        /// <summary>
        /// YPMCODAR 9  
        /// </summary>
        [HisFieldInfoMapping(45, 1, CobolType = CobolType.Unsigned)]
        public short YPMCODAR { get; set; }

        /// <summary>
        /// YPMIVSAR 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(46, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMIVSAR { get; set; }

        /// <summary>
        /// YPMRARTA 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(47, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMRARTA { get; set; }

        /// <summary>
        /// YPMAARTA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(48, 4, CobolType = CobolType.Unsigned)]
        public short YPMAARTA { get; set; }

        /// <summary>
        /// YPMRARTB 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(49, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMRARTB { get; set; }

        /// <summary>
        /// YPMAARTB 9(4)  
        /// </summary>
        [HisFieldInfoMapping(50, 4, CobolType = CobolType.Unsigned)]
        public short YPMAARTB { get; set; }

        /// <summary>
        /// YPMATART 9(4)  
        /// </summary>
        [HisFieldInfoMapping(51, 4, CobolType = CobolType.Unsigned)]
        public short YPMATART { get; set; }

        /// <summary>
        /// YPMCODCO 9  
        /// </summary>
        [HisFieldInfoMapping(52, 1, CobolType = CobolType.Unsigned)]
        public short YPMCODCO { get; set; }

        /// <summary>
        /// YPMIVSCO 9(5)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(53, 9, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMIVSCO { get; set; }

        /// <summary>
        /// YPMRCOMA 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(54, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMRCOMA { get; set; }

        /// <summary>
        /// YPMACOMA 9(4)  
        /// </summary>
        [HisFieldInfoMapping(55, 4, CobolType = CobolType.Unsigned)]
        public short YPMACOMA { get; set; }

        /// <summary>
        /// YPMRCOMB 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(56, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMRCOMB { get; set; }

        /// <summary>
        /// YPMACOMB 9(4)  
        /// </summary>
        [HisFieldInfoMapping(57, 4, CobolType = CobolType.Unsigned)]
        public short YPMACOMB { get; set; }

        /// <summary>
        /// YPMATCOM 9(4)  
        /// </summary>
        [HisFieldInfoMapping(58, 4, CobolType = CobolType.Unsigned)]
        public short YPMATCOM { get; set; }

        /// <summary>
        /// YPMMONTA 9(7)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(59, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMMONTA { get; set; }

        /// <summary>
        /// YPMESCLU 9(7)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(60, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMESCLU { get; set; }

        /// <summary>
        /// YPMSETTE 9(4)  
        /// </summary>
        [HisFieldInfoMapping(61, 4, CobolType = CobolType.Unsigned)]
        public short YPMSETTE { get; set; }

        /// <summary>
        /// YPMIMPCRT 9(6)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(62, 10, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMIMPCRT { get; set; }

        /// <summary>
        /// YPMMONTA2012 9(7)V9(4)  
        /// </summary>
        [HisFieldInfoMapping(63, 11, Scale = 4, CobolType = CobolType.Unsigned)]
        public decimal YPMMONTA2012 { get; set; }

        /// <summary>
        /// YPMSETT2012 9(4)  
        /// </summary>
        [HisFieldInfoMapping(64, 4, CobolType = CobolType.Unsigned)]
        public short YPMSETT2012 { get; set; }

        /// <summary>
        /// YPMSETA-707 9(4)
        /// </summary>
        [HisFieldInfoMapping(65, 4, CobolType = CobolType.Unsigned)]
        public short YPMSETA_707 { get; set; }

        /// <summary>
        /// YPMSETB-707 9(4)
        /// </summary>
        [HisFieldInfoMapping(66, 4, CobolType = CobolType.Unsigned)]
        public short YPMSETB_707 { get; set; }

        /// <summary>
        /// YPMCALC707 X  
        /// </summary>
        [HisFieldInfoMapping(67, 1)]
        public string YPMCALC707 { get; set; }

        /// <summary>
        /// YPMPROGR 99  
        /// </summary>
        [HisFieldInfoMapping(68, 2, CobolType = CobolType.Unsigned)]
        public short YPMPROGR { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "PM"; }
        }
        #endregion Properties
    }
}