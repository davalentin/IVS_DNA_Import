using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostRequest
{
    public class CI05RequestBase
    {
        #region Properties
        #endregion Properties

        #region Nested class
        /// <summary>
        /// Definizione della copy CXXAC0D1 - 200 BYTES
        /// </summary>
        public class AreaControllo
        {
            #region Copy COBOL
            //05  AC.
            //    10  NLMXARECOM                         PIC  9(05).
            //    10  CDAS                               PIC  X(06).
            //    10  CDIP                               PIC  X(10).
            //    10  CUNILGC                            PIC  X(10).
            //    10  CSED                               PIC  X(02).
            //    10  CZON                               PIC  X(02).
            //    10  CCOP                               PIC  X(02).
            //    10  FMRE                               PIC  9(01).
            //        88  FMRE-FINE-DATI                 VALUE 0.
            //        88  FMRE-ALTRI-DATI                VALUE 1.
            //    10  FRCRR02                            PIC  X(01).
            //        88  FRCRR02-OK                     VALUE "0".
            //        88  FRCRR02-KO                     VALUE "1".
            //    10  CRCRDAS                            PIC  9(06).
            //    10  HTMSLOG                            PIC  X(26).
            //    10  FSGNDBG                            PIC  X(01).
            //        88  FSGNDBG-KO                     VALUE "0".
            //        88  FSGNDBG-OK                     VALUE "1".
            //    10  CALTINDCIX                         PIC  X(08).
            //    10  CALTTIPCOM                         PIC  X(01).
            //    10  NLMXTOT                            PIC  9(07).
            //    10  CNOMTSY                            PIC  X(08).
            //    10  NITYTSY                            PIC  9(03).
            //    10  NLMXAREINP                         PIC  9(07).
            //    10  CTRNIMS                            PIC  X(08).
            //    10  CPGM                               PIC  X(12).
            //    10  FCHS                               PIC  X(01).
            //        88 FCHS-COLLEGATO                  VALUE " ".
            //        88 FCHS-SCOLLEGATO                 VALUE "1".
            //    10  FLCK                               PIC  X(01).
            //        88 FLCK-NON-VINCOLATO              VALUE " ".
            //        88 FLCK-VINCOLATO                  VALUE "1".
            //    10  FABN                               PIC  X(01).
            //        88 FABN-ATTESA-RISPOSTA            VALUE "0".
            //        88 FABN-CHIUSURA-NORMALE           VALUE "1".
            //    10  FRCM                               PIC  X(01).
            //        88 FRCM-NO-COMMIT                  VALUE "0".
            //        88 FRCM-SI-COMMIT                  VALUE "1".
            //    10  NLMXEFFDAS                         PIC  9(07).
            //    10  NELEDAS                            PIC  9(03).
            //    10  CPSW                               PIC  X(08).
            //    10  CSWRREL                            PIC  X(02).
            //    10  AC-FILLER                          PIC  X(50).
            #endregion Copy COBOL

            #region Tracciato Host
            /// <summary>
            /// NLMXARECOM PIC 9(5)
            /// </summary>
            [HisFieldInfoMapping(0, 5)]
            public int NLMXARECOM { get; set; }

            /// <summary>
            /// CDAS PIC X(6)
            /// </summary>
            [HisFieldInfoMapping(1, 6)]
            public string CDAS { get; set; }

            /// <summary>
            /// CDIP - CUNILGC
            /// </summary>
            [HisFieldInfoMapping(2, 20)]
            public string Filler1 { get; set; }

            /// <summary>
            /// CSED PIC XX
            /// </summary>
            [HisFieldInfoMapping(3, 2)]
            public string CSED { get; set; }

            /// <summary>
            /// CZON PIC XX
            /// </summary>
            [HisFieldInfoMapping(4, 2)]
            public string CZON { get; set; }

            /// <summary>
            /// CCO PIC XX
            /// </summary>
            [HisFieldInfoMapping(5, 2)]
            public string CCO { get; set; }

            /// <summary>
            /// FMRE PIC 9.
            /// 0: fine dati
            /// 1: altri dati
            /// </summary>
            [HisFieldInfoMapping(6, 1)]
            public short FMRE { get; set; }

            /// <summary>
            /// FRCRR02 PIC X
            /// "0": OK
            /// "1": KO
            /// </summary>
            [HisFieldInfoMapping(7, 1)]
            public string FRCRR02 { get; set; }

            /// <summary>
            /// CRCRDAS PIC 9(6)
            /// Vedi enum ErroreHost
            /// </summary>
            [HisFieldInfoMapping(8, 6)]
            public int CRCRDAS { get; set; }

            /// <summary>
            /// HTMSLOG PIC X(26)
            /// </summary>
            [HisFieldInfoMapping(9, 26)]
            public string HTMSLOG { get; set; }

            /// <summary>
            /// FSGNDBG PIC X
            /// "0": OK
            /// "1": KO
            /// </summary>
            [HisFieldInfoMapping(10, 1)]
            public string FSGNDBG { get; set; }

            /// <summary>
            /// CALTINDCIX PIC X(8)
            /// </summary>
            [HisFieldInfoMapping(11, 8)]
            public string CALTINDCIX { get; set; }

            /// <summary>
            /// CALTTIPCOM PIC X
            /// </summary>
            [HisFieldInfoMapping(12, 1)]
            public string CALTTIPCOM { get; set; }

            /// <summary>
            /// NLMXTOT PIC 9(7)
            /// </summary>
            [HisFieldInfoMapping(13, 7)]
            public int NLMXTOT { get; set; }

            /// <summary>
            /// CNOMTSY PIC X(8)
            /// </summary>
            [HisFieldInfoMapping(14, 8)]
            public string CNOMTSY { get; set; }

            /// <summary>
            /// NITYTSY PIC 999
            /// </summary>
            [HisFieldInfoMapping(15, 3)]
            public short NITYTSY { get; set; }

            /// <summary>
            /// NLMXAREINP  PIC 9(7)
            /// </summary>
            [HisFieldInfoMapping(16, 7)]
            public int NLMXAREINP { get; set; }

            /// <summary>
            /// CTRNIMS PIC X(8)
            /// </summary>
            [HisFieldInfoMapping(17, 8)]
            public string CTRNIMS { get; set; }

            /// <summary>
            /// CPGM PIC X(12)
            /// </summary>
            [HisFieldInfoMapping(18, 12)]
            public string CPGM { get; set; }

            /// <summary>
            /// FCHS PIC X
            /// </summary>
            [HisFieldInfoMapping(19, 1)]
            public string FCHS { get; set; }

            /// <summary>
            /// FLCK PIC X
            /// </summary>
            [HisFieldInfoMapping(20, 1)]
            public string FLCK { get; set; }

            /// <summary>
            /// FABN PIC X
            /// </summary>
            [HisFieldInfoMapping(21, 1)]
            public string FABN { get; set; }

            /// <summary>
            /// FRCM PIC X
            /// </summary>
            [HisFieldInfoMapping(22, 1)]
            public string FRCM { get; set; }

            /// <summary>
            /// NLMXEFFDAS PIC 9(7)
            /// </summary>
            [HisFieldInfoMapping(23, 7)]
            public int NLMXEFFDAS { get; set; }

            /// <summary>
            /// NELEDAS PIC 999
            /// </summary>
            [HisFieldInfoMapping(24, 3)]
            public short NELEDAS { get; set; }

            /// <summary>
            /// CPSW PIC X(8)
            /// </summary>
            [HisFieldInfoMapping(25, 8)]
            public string CPSW { get; set; }

            /// <summary>
            /// CSWRREL PIC XX
            /// </summary>
            [HisFieldInfoMapping(26, 2)]
            public string CSWRREL { get; set; }

            /// <summary>
            /// AC-FILLER PIC X(50)
            /// </summary>
            [HisFieldInfoMapping(27, 50)]
            public string Filler2 { get; set; }
            #endregion Tracciato Host
        }
        #endregion Nested class
    }
}

