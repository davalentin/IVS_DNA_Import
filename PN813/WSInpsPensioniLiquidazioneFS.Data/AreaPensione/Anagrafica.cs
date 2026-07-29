using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA
{
    public class Anagrafica : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //01  TRA-ANAGR.
        //          02 TRATIPOR            PIC X VALUE "A".
        //          02 TRANUMDO            PIC 9(15) COMP-3.
        //          02 TRACERTI            PIC 9(8) VALUE ZEROES.
        //          02 TRACATEG            PIC X(6).
        //          02 TRATIPIR            PIC X.
        //          02 TRACNTOP            PIC 99.
        //          02 TRAISOLA            PIC 99.
        //          02 TRACONOM            PIC X(32).
        //          02 TRAACQUI            PIC X(16).
        //          02 TRASESSO            PIC X.
        //          02 TRADTNAS.
        //             03 TRAAANAS            PIC 9999.
        //             03 TRAMMNAS            PIC 99.
        //             03 TRAGGNAS            PIC 99.
        //          02 TRACONAS               PIC 9(5).
        //          02 TRAPRNAS               PIC 99.
        //          02 TRACOFIS               PIC X(16).
        //          02 TRACFSIT               PIC X.
        //          02 TRADETR1               PIC 9.
        //          02 TRADETR2               PIC 9.
        //          02 TRADETR3               PIC 9.
        //          02 TRADETR4               PIC 9.
        //          02 TRADETR5               PIC 9.
        //          02 TRADETR6               PIC 99.
        //          02 TRADETR7               PIC 99.
        //          02 TRADETR8               PIC 99.
        //          02 TRADETR9               PIC 99.
        //          02 TRADET10               PIC 9.
        //          02 TRAINDIR               PIC X(32).
        //          02 TRACORES               PIC X(22).
        //          02 TRAPRRES               PIC X(3).
        //          02 TRACAPPP               PIC 9(5).
        //          02 TRAUFPAG               PIC X(3).
        //          02 TRARSEST               PIC 9.
        //          02 TRAPGEST               PIC 9.
        //          02 TRATRLAV               PIC 9.
        //          02 TRACDPAT               PIC 99.
        //          02 TRAZNPAT               PIC X.
        //          02 TRANRPAT               PIC 9(8).
        //          02 TRACODSI               PIC X.
        //D2NEW     02 TRADECSI               PIC 9(6).                           
        //          02 TRACDCOM               PIC X(6).
        //D2NEW     02 TRAAAPAG               PIC 9999.                           
        //          02 TRAMMPAG               PIC 99.
        //          02 TRARECUP               PIC 9(8).
        //          02 TRACODEL               PIC X.
        //D2NEW     02 TRADECEL               PIC 9(6).                           
        //D2NEW     02 TRACNTEL               PIC 9(6).                           
        //D2NEW     02 TRADATEV               PIC 9(8).                           
        //D2000     02 TRADIFDT.
        //D2NEW        03 TRADIFAA         PIC 9999.                              
        //             03 TRADIFMM         PIC 99.
        //             03 TRADIFGG         PIC 99.
        //D2000     02 TRASPFDT.
        //D2NEW        03 TRASPFAA         PIC 9999.                              
        //             03 TRASPFMM         PIC 99.
        //             03 TRASPFGG         PIC 99.
        //D2NEW     02 TRADIAAA            PIC 9999.                              
        //          02 TRADIAMM            PIC 99.
        //D2NEW     02 TRASPAAA            PIC 9999.                              
        //          02 TRASPAMM            PIC 99.
        //          02 TRACAUSA            PIC 9.
        //          02 TRATPLIQ            PIC X(3).
        //          02 TRASECOM            PIC 9(4).
        //          02 TRASELIQ            PIC 9(4).
        //D2NEW     02 TRAPRESE            PIC 9(8).                              
        //D2NEW     02 TRAINTLG            PIC 9(8).                              
        //D2NEW     02 TRAACQU1            PIC 9(8).                              
        //          02 TRAITER1            PIC X(5).
        //          02 TRACEDUT            PIC 99.
        //          02 TRAREQU1            PIC X.
        //          02 TRACIVIL            PIC X.
        //          02 TRA562              PIC XX.
        //          02 TRADIMISSIONI       PIC XX.
        //          02 TRAMATRI            PIC 9(8).
        //          02 TRATIPCALC          PIC X(2).
        //          02 TRACODSI1           PIC X(2).
        //          02 TRAINPDAI           PIC X.
        //          02 TRAANZ247           PIC XX.
        //          02 TRAESODAN           PIC XX.
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  TRA-ANAGR.
        /// <summary>
        /// TRATIPOR X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string TRATIPOR { get; set; }

        /// <summary>
        /// TRANUMDO 9(15) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(1, 8, CobolType = CobolType.Comp3Unsigned)]
        public long TRANUMDO { get; set; }

        /// <summary>
        /// TRACERTI 9(8)  
        /// </summary>
        [HisFieldInfoMapping(2, 8, CobolType = CobolType.Unsigned)]
        public int TRACERTI { get; set; }

        /// <summary>
        /// TRACATEG X(6)  
        /// </summary>
        [HisFieldInfoMapping(3, 6)]
        public string TRACATEG { get; set; }

        /// <summary>
        /// TRATIPIR X  
        /// </summary>
        [HisFieldInfoMapping(4, 1)]
        public string TRATIPIR { get; set; }

        /// <summary>
        /// TRACNTOP 99  
        /// </summary>
        [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
        public short TRACNTOP { get; set; }

        /// <summary>
        /// TRAISOLA 99  
        /// </summary>
        [HisFieldInfoMapping(6, 2, CobolType = CobolType.Unsigned)]
        public short TRAISOLA { get; set; }

        /// <summary>
        /// TRACONOM X(32)  
        /// </summary>
        [HisFieldInfoMapping(7, 32)]
        public string TRACONOM { get; set; }

        /// <summary>
        /// TRAACQUI X(16)  
        /// </summary>
        [HisFieldInfoMapping(8, 16)]
        public string TRAACQUI { get; set; }

        /// <summary>
        /// TRASESSO X  
        /// </summary>
        [HisFieldInfoMapping(9, 1)]
        public string TRASESSO { get; set; }

        // 02 TRADTNAS.
        /// <summary>
        /// TRAAANAS 9999  
        /// </summary>
        [HisFieldInfoMapping(10, 4, CobolType = CobolType.Unsigned)]
        public short TRAAANAS { get; set; }

        /// <summary>
        /// TRAMMNAS 99  
        /// </summary>
        [HisFieldInfoMapping(11, 2, CobolType = CobolType.Unsigned)]
        public short TRAMMNAS { get; set; }

        /// <summary>
        /// TRAGGNAS 99  
        /// </summary>
        [HisFieldInfoMapping(12, 2, CobolType = CobolType.Unsigned)]
        public short TRAGGNAS { get; set; }

        /// <summary>
        /// TRACONAS 9(5)  
        /// </summary>
        [HisFieldInfoMapping(13, 5, CobolType = CobolType.Unsigned)]
        public int TRACONAS { get; set; }

        /// <summary>
        /// TRAPRNAS 99  
        /// </summary>
        [HisFieldInfoMapping(14, 2, CobolType = CobolType.Unsigned)]
        public short TRAPRNAS { get; set; }

        /// <summary>
        /// TRACOFIS X(16)  
        /// </summary>
        [HisFieldInfoMapping(15, 16)]
        public string TRACOFIS { get; set; }

        /// <summary>
        /// TRACFSIT X  
        /// </summary>
        [HisFieldInfoMapping(16, 1)]
        public string TRACFSIT { get; set; }

        /// <summary>
        /// TRADETR1 9  
        /// </summary>
        [HisFieldInfoMapping(17, 1, CobolType = CobolType.Unsigned)]
        public short TRADETR1 { get; set; }

        /// <summary>
        /// TRADETR2 9  
        /// </summary>
        [HisFieldInfoMapping(18, 1, CobolType = CobolType.Unsigned)]
        public short TRADETR2 { get; set; }

        /// <summary>
        /// TRADETR3 9  
        /// </summary>
        [HisFieldInfoMapping(19, 1, CobolType = CobolType.Unsigned)]
        public short TRADETR3 { get; set; }

        /// <summary>
        /// TRADETR4 9  
        /// </summary>
        [HisFieldInfoMapping(20, 1, CobolType = CobolType.Unsigned)]
        public short TRADETR4 { get; set; }

        /// <summary>
        /// TRADETR5 9  
        /// </summary>
        [HisFieldInfoMapping(21, 1, CobolType = CobolType.Unsigned)]
        public short TRADETR5 { get; set; }

        /// <summary>
        /// TRADETR6 99  
        /// </summary>
        [HisFieldInfoMapping(22, 2, CobolType = CobolType.Unsigned)]
        public short TRADETR6 { get; set; }

        /// <summary>
        /// TRADETR7 99  
        /// </summary>
        [HisFieldInfoMapping(23, 2, CobolType = CobolType.Unsigned)]
        public short TRADETR7 { get; set; }

        /// <summary>
        /// TRADETR8 99  
        /// </summary>
        [HisFieldInfoMapping(24, 2, CobolType = CobolType.Unsigned)]
        public short TRADETR8 { get; set; }

        /// <summary>
        /// TRADETR9 99  
        /// </summary>
        [HisFieldInfoMapping(25, 2, CobolType = CobolType.Unsigned)]
        public short TRADETR9 { get; set; }

        /// <summary>
        /// TRADET10 9  
        /// </summary>
        [HisFieldInfoMapping(26, 1, CobolType = CobolType.Unsigned)]
        public short TRADET10 { get; set; }

        /// <summary>
        /// TRAINDIR X(32)  
        /// </summary>
        [HisFieldInfoMapping(27, 32)]
        public string TRAINDIR { get; set; }

        /// <summary>
        /// TRACORES X(22)  
        /// </summary>
        [HisFieldInfoMapping(28, 22)]
        public string TRACORES { get; set; }

        /// <summary>
        /// TRAPRRES X(3)  
        /// </summary>
        [HisFieldInfoMapping(29, 3)]
        public string TRAPRRES { get; set; }

        /// <summary>
        /// TRACAPPP 9(5)  
        /// </summary>
        [HisFieldInfoMapping(30, 5, CobolType = CobolType.Unsigned)]
        public int TRACAPPP { get; set; }

        /// <summary>
        /// TRAUFPAG X(3)  
        /// </summary>
        [HisFieldInfoMapping(31, 3)]
        public string TRAUFPAG { get; set; }

        /// <summary>
        /// TRARSEST 9  
        /// </summary>
        [HisFieldInfoMapping(32, 1, CobolType = CobolType.Unsigned)]
        public short TRARSEST { get; set; }

        /// <summary>
        /// TRAPGEST 9  
        /// </summary>
        [HisFieldInfoMapping(33, 1, CobolType = CobolType.Unsigned)]
        public short TRAPGEST { get; set; }

        /// <summary>
        /// TRATRLAV 9  
        /// </summary>
        [HisFieldInfoMapping(34, 1, CobolType = CobolType.Unsigned)]
        public short TRATRLAV { get; set; }

        /// <summary>
        /// TRACDPAT 99  
        /// </summary>
        [HisFieldInfoMapping(35, 2, CobolType = CobolType.Unsigned)]
        public short TRACDPAT { get; set; }

        /// <summary>
        /// TRAZNPAT X  
        /// </summary>
        [HisFieldInfoMapping(36, 1)]
        public string TRAZNPAT { get; set; }

        /// <summary>
        /// TRANRPAT 9(8)  
        /// </summary>
        [HisFieldInfoMapping(37, 8, CobolType = CobolType.Unsigned)]
        public int TRANRPAT { get; set; }

        /// <summary>
        /// TRACODSI X  
        /// </summary>
        [HisFieldInfoMapping(38, 1)]
        public string TRACODSI { get; set; }

        /// <summary>
        /// TRADECSI 9(6)  
        /// </summary>
        [HisFieldInfoMapping(39, 6, CobolType = CobolType.Unsigned)]
        public int TRADECSI { get; set; }

        /// <summary>
        /// TRACDCOM1 X(2)  
        /// </summary>
        [HisFieldInfoMapping(40, 2)]
        public string TRACDCOM1 { get; set; }

        /// <summary>
        /// TRACDCOM2 X(2)  
        /// </summary>
        [HisFieldInfoMapping(41, 2)]
        public string TRACDCOM2 { get; set; }

        /// <summary>
        /// TRACDCOM3 X(1)  
        /// </summary>
        [HisFieldInfoMapping(42, 1)]
        public string TRACDCOM3 { get; set; }

        /// <summary>
        /// TRACDCOM4 X(1)  
        /// </summary>
        [HisFieldInfoMapping(43, 1)]
        public string TRACDCOM4 { get; set; }

        /// <summary>
        /// TRAAAPAG 9999  
        /// </summary>
        [HisFieldInfoMapping(44, 4, CobolType = CobolType.Unsigned)]
        public short TRAAAPAG { get; set; }

        /// <summary>
        /// TRAMMPAG 99  
        /// </summary>
        [HisFieldInfoMapping(45, 2, CobolType = CobolType.Unsigned)]
        public short TRAMMPAG { get; set; }

        /// <summary>
        /// TRARECUP 9(8)  
        /// </summary>
        [HisFieldInfoMapping(46, 8, CobolType = CobolType.Unsigned)]
        public int TRARECUP { get; set; }

        /// <summary>
        /// TRACODEL X  
        /// </summary>
        [HisFieldInfoMapping(47, 1)]
        public string TRACODEL { get; set; }

        /// <summary>
        /// TRADECEL 9(6)  
        /// </summary>
        [HisFieldInfoMapping(48, 6, CobolType = CobolType.Unsigned)]
        public int TRADECEL { get; set; }

        /// <summary>
        /// TRACNTEL 9(6)  
        /// </summary>
        [HisFieldInfoMapping(49, 6, CobolType = CobolType.Unsigned)]
        public int TRACNTEL { get; set; }

        /// <summary>
        /// TRADATEV 9(8)  
        /// </summary>
        [HisFieldInfoMapping(50, 8, CobolType = CobolType.Unsigned)]
        public int TRADATEV { get; set; }

        // D2000     02 TRADIFDT.
        /// <summary>
        /// TRADIFAA 9999  
        /// </summary>
        [HisFieldInfoMapping(51, 4, CobolType = CobolType.Unsigned)]
        public short TRADIFAA { get; set; }

        /// <summary>
        /// TRADIFMM 99  
        /// </summary>
        [HisFieldInfoMapping(52, 2, CobolType = CobolType.Unsigned)]
        public short TRADIFMM { get; set; }

        /// <summary>
        /// TRADIFGG 99  
        /// </summary>
        [HisFieldInfoMapping(53, 2, CobolType = CobolType.Unsigned)]
        public short TRADIFGG { get; set; }

        // D2000     02 TRASPFDT.
        /// <summary>
        /// TRASPFAA 9999  
        /// </summary>
        [HisFieldInfoMapping(54, 4, CobolType = CobolType.Unsigned)]
        public short TRASPFAA { get; set; }

        /// <summary>
        /// TRASPFMM 99  
        /// </summary>
        [HisFieldInfoMapping(55, 2, CobolType = CobolType.Unsigned)]
        public short TRASPFMM { get; set; }

        /// <summary>
        /// TRASPFGG 99  
        /// </summary>
        [HisFieldInfoMapping(56, 2, CobolType = CobolType.Unsigned)]
        public short TRASPFGG { get; set; }

        /// <summary>
        /// TRADIAAA 9999  
        /// </summary>
        [HisFieldInfoMapping(57, 4, CobolType = CobolType.Unsigned)]
        public short TRADIAAA { get; set; }

        /// <summary>
        /// TRADIAMM 99  
        /// </summary>
        [HisFieldInfoMapping(58, 2, CobolType = CobolType.Unsigned)]
        public short TRADIAMM { get; set; }

        /// <summary>
        /// TRASPAAA 9999  
        /// </summary>
        [HisFieldInfoMapping(59, 4, CobolType = CobolType.Unsigned)]
        public short TRASPAAA { get; set; }

        /// <summary>
        /// TRASPAMM 99  
        /// </summary>
        [HisFieldInfoMapping(60, 2, CobolType = CobolType.Unsigned)]
        public short TRASPAMM { get; set; }

        /// <summary>
        /// TRACAUSA 9  
        /// </summary>
        [HisFieldInfoMapping(61, 1, CobolType = CobolType.Unsigned)]
        public short TRACAUSA { get; set; }

        /// <summary>
        /// TRATPLIQ X(3)  
        /// </summary>
        [HisFieldInfoMapping(62, 3)]
        public string TRATPLIQ { get; set; }

        /// <summary>
        /// TRASECOM 9(4)  
        /// </summary>
        [HisFieldInfoMapping(63, 4, CobolType = CobolType.Unsigned)]
        public short TRASECOM { get; set; }

        /// <summary>
        /// TRASELIQ 9(4)  
        /// </summary>
        [HisFieldInfoMapping(64, 4, CobolType = CobolType.Unsigned)]
        public short TRASELIQ { get; set; }

        /// <summary>
        /// TRAPRESE 9(8)  
        /// </summary>
        [HisFieldInfoMapping(65, 8, CobolType = CobolType.Unsigned)]
        public int TRAPRESE { get; set; }

        /// <summary>
        /// TRAINTLG 9(8)  
        /// </summary>
        [HisFieldInfoMapping(66, 8, CobolType = CobolType.Unsigned)]
        public int TRAINTLG { get; set; }

        /// <summary>
        /// TRAACQU1 9(8)  
        /// </summary>
        [HisFieldInfoMapping(67, 8, CobolType = CobolType.Unsigned)]
        public int TRAACQU1 { get; set; }

        /// <summary>
        /// TRAITER1 X(5)  
        /// </summary>
        [HisFieldInfoMapping(68, 5)]
        public string TRAITER1 { get; set; }

        /// <summary>
        /// TRACEDUT 99  
        /// </summary>
        [HisFieldInfoMapping(69, 2, CobolType = CobolType.Unsigned)]
        public short TRACEDUT { get; set; }

        /// <summary>
        /// TRAREQU1 X  
        /// </summary>
        [HisFieldInfoMapping(70, 1)]
        public string TRAREQU1 { get; set; }

        /// <summary>
        /// TRACIVIL X  
        /// </summary>
        [HisFieldInfoMapping(71, 1)]
        public string TRACIVIL { get; set; }

        /// <summary>
        /// TRA562 XX  
        /// </summary>
        [HisFieldInfoMapping(72, 2)]
        public string TRA562 { get; set; }

        /// <summary>
        /// TRADIMISSIONI XX  
        /// </summary>
        [HisFieldInfoMapping(73, 2)]
        public string TRADIMISSIONI { get; set; }

        /// <summary>
        /// TRAMATRI 9(8)  
        /// </summary>
        [HisFieldInfoMapping(74, 8, CobolType = CobolType.Unsigned)]
        public int TRAMATRI { get; set; }

        /// <summary>
        /// TRATIPCALC X(2)  
        /// </summary>
        [HisFieldInfoMapping(75, 2)]
        public string TRATIPCALC { get; set; }

        /// <summary>
        /// TRACODSI1 X(2)  
        /// </summary>
        [HisFieldInfoMapping(76, 2)]
        public string TRACODSI1 { get; set; }

        /// <summary>
        /// TRAINPDAI X  
        /// </summary>
        [HisFieldInfoMapping(77, 1)]
        public string TRAINPDAI { get; set; }

        /// <summary>
        /// TRAANZ247 XX  
        /// </summary>
        [HisFieldInfoMapping(78, 2)]
        public string TRAANZ247 { get; set; }

        /// <summary>
        /// TRAESODAN XX  
        /// </summary>
        [HisFieldInfoMapping(79, 2)]
        public string TRAESODAN { get; set; }
        #endregion Tracciato Host

        public string TransactionName
        {
            get { return "Anagrafica"; }
        }

        #endregion Properties
    }
}
