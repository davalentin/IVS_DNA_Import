using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class Familiari
    {
        #region Properties

        #region Tracciato COBOL
        //             *DATI DEL PANNELLO MRCFAM0 (FAMILIARI)
        //          02 T-GPFAM0.
        //             03 T-GP3 OCCURS 15.
        //                04 T-GP3CB00.
        //                   05 T-GP3CB02        PIC X(32).
        //                   05 T-GP3CB03        PIC X(32).
        //                   05 T-GP3CB04        PIC X(31).
        //                   05 T-GP3CB05        PIC X.
        //                   05 T-GP3CB06.
        //                      06 T-GP3CB06G    PIC 9(2).
        //                      06 T-GP3CB06M    PIC 9(2).
        //                      06 T-GP3CB06A    PIC 9(4).
        //                   05 T-GP3CB07        PIC 9(5).
        //                   05 T-GP3CB08        PIC X(16).
        //                   05 T-GP3CB09-V      PIC X.
        //                   05 T-GP3CB17        PIC X(36).
        //                   05 T-GP3CB18        PIC X.
        //                   05 T-GP3CB27        PIC X(3).
        //                   05 T-GP3CB10        PIC X(3).
        //BL23A              05 T-GP3CB11        PIC 9(8) BINARY.
        //                04 T-GP3CK20.
        //                   05 T-GP3CK20A       PIC 9(4).
        //                   05 T-GP3CK20M       PIC 9(2).
        //                04 T-GP3FTITPRN        PIC X.
        //                04 T-GP3CK OCCURS 10.
        //                   05 T-GP3CH01           PIC X.
        //                   05 T-GP3CH01B          PIC X.
        //                   05 T-GP3CK04        PIC 9.
        //                   05 T-GP3CK01.
        //                      06 T-GP3CK01A    PIC 9(4).
        //                      06 T-GP3CK01M    PIC 9(2).
        //                   05 T-GP3CK02.
        //                      06 T-GP3CK02A    PIC 9(4).
        //                      06 T-GP3CK02M    PIC 9(2).
        //                   05 T-GP3CK06        PIC X.
        //                04 T-GP3CB12-V.
        //                   05 T-GP3CB12A-V     PIC 9(4).
        //                   05 T-GP3CB12M-V     PIC 9(2).
        //                   05 T-GP3CB12G-V     PIC 9(2).
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 15)]
        public List<T_GP3> LISTT_GP3 { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class T_GP3
        {
            #region Properties

            #region Tracciato COBOL
            //             *DATI DEL PANNELLO MRCFAM0 (FAMILIARI)
            //          02 T-GPFAM0.
            //             03 T-GP3 OCCURS 15.
            //                04 T-GP3CB00.
            //                   05 T-GP3CB02        PIC X(32).
            //                   05 T-GP3CB03        PIC X(32).
            //                   05 T-GP3CB04        PIC X(31).
            //                   05 T-GP3CB05        PIC X.
            //                   05 T-GP3CB06.
            //                      06 T-GP3CB06G    PIC 9(2).
            //                      06 T-GP3CB06M    PIC 9(2).
            //                      06 T-GP3CB06A    PIC 9(4).
            //                   05 T-GP3CB07        PIC 9(5).
            //                   05 T-GP3CB08        PIC X(16).
            //                   05 T-GP3CB09-V      PIC X.
            //                   05 T-GP3CB17        PIC X(36).
            //                   05 T-GP3CB18        PIC X.
            //                   05 T-GP3CB27        PIC X(3).
            //                   05 T-GP3CB10        PIC X(3).
            //BL23A              05 T-GP3CB11        PIC 9(8) BINARY.
            //                04 T-GP3CK20.
            //                   05 T-GP3CK20A       PIC 9(4).
            //                   05 T-GP3CK20M       PIC 9(2).
            //                04 T-GP3FTITPRN        PIC X.
            //                04 T-GP3CK OCCURS 10.
            //                   05 T-GP3CH01           PIC X.
            //                   05 T-GP3CH01B          PIC X.
            //                   05 T-GP3CK04        PIC 9.
            //                   05 T-GP3CK01.
            //                      06 T-GP3CK01A    PIC 9(4).
            //                      06 T-GP3CK01M    PIC 9(2).
            //                   05 T-GP3CK02.
            //                      06 T-GP3CK02A    PIC 9(4).
            //                      06 T-GP3CK02M    PIC 9(2).
            //                      05 T-GP3CK06     PIC X.
            //                04 T-GP3CB12-V.
            //                   05 T-GP3CB12A-V     PIC 9(4).
            //                   05 T-GP3CB12M-V     PIC 9(2).
            //                   05 T-GP3CB12G-V     PIC 9(2).
            #endregion Tracciato COBOL

            #region Tracciato Host
            // *DATI DEL PANNELLO MRCFAM0 (FAMILIARI)
            // 02 T-GPFAM0.
            // 03 T-GP3 OCCURS 15.
            // 04 T-GP3CB00.
            /// <summary>
            /// T_GP3CB02 X(32)  
            /// </summary>
            [HisFieldInfoMapping(0, 32)]
            public string T_GP3CB02 { get; set; }

            /// <summary>
            /// T_GP3CB03 X(32)  
            /// </summary>
            [HisFieldInfoMapping(1, 32)]
            public string T_GP3CB03 { get; set; }

            /// <summary>
            /// T_GP3CB04 X(31)  
            /// </summary>
            [HisFieldInfoMapping(2, 31)]
            public string T_GP3CB04 { get; set; }

            /// <summary>
            /// T_GP3CB05 X  
            /// </summary>
            [HisFieldInfoMapping(3, 1)]
            public string T_GP3CB05 { get; set; }

            // 05 T-GP3CB06.
            /// <summary>
            /// T_GP3CB06G 9(2)  
            /// </summary>
            [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
            public short T_GP3CB06G { get; set; }

            /// <summary>
            /// T_GP3CB06M 9(2)  
            /// </summary>
            [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
            public short T_GP3CB06M { get; set; }

            /// <summary>
            /// T_GP3CB06A 9(4)  
            /// </summary>
            [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
            public short T_GP3CB06A { get; set; }

            /// <summary>
            /// T_GP3CB07 9(5)  
            /// </summary>
            [HisFieldInfoMapping(7, 5, CobolType = CobolType.Unsigned)]
            public int T_GP3CB07 { get; set; }

            /// <summary>
            /// T_GP3CB08 X(16)  
            /// </summary>
            [HisFieldInfoMapping(8, 16)]
            public string T_GP3CB08 { get; set; }

            /// <summary>
            /// T_GP3CB09_V X  
            /// </summary>
            [HisFieldInfoMapping(9, 1)]
            public string T_GP3CB09_V { get; set; }

            /// <summary>
            /// T_GP3CB17 X(36)  
            /// </summary>
            [HisFieldInfoMapping(10, 36)]
            public string T_GP3CB17 { get; set; }

            /// <summary>
            /// T_GP3CB18 X  
            /// </summary>
            [HisFieldInfoMapping(11, 1)]
            public string T_GP3CB18 { get; set; }

            /// <summary>
            /// T_GP3CB27 X(3)  
            /// </summary>
            [HisFieldInfoMapping(12, 3)]
            public string T_GP3CB27 { get; set; }

            /// <summary>
            /// T_GP3CB10 X(3)  
            /// </summary>
            [HisFieldInfoMapping(13, 3)]
            public string T_GP3CB10 { get; set; }

            /// <summary>
            /// T_GP3CB11 9(8)  BINARY
            /// </summary>
            [HisFieldInfoMapping(14, 4, CobolType = CobolType.Binary)]
            public int T_GP3CB11 { get; set; }

            // 04 T-GP3CK20.
            /// <summary>
            /// T_GP3CK20A 9(4)  
            /// </summary>
            [HisFieldInfoMapping(15, 4, CobolType = CobolType.Unsigned)]
            public short T_GP3CK20A { get; set; }

            /// <summary>
            /// T_GP3CK20M 9(2)  
            /// </summary>
            [HisFieldInfoMapping(16, 2, CobolType = CobolType.Unsigned)]
            public short T_GP3CK20M { get; set; }

            /// <summary>
            /// T_GP3FTITPRN X  
            /// </summary>
            [HisFieldInfoMapping(17, 1)]
            public string T_GP3FTITPRN { get; set; }

            [HisComplexAreaInfoMapping(18, ListCount = 10)]
            public List<T_GP3CK> LISTT_GP3CK { get; set; }
            
            
            // 04 T-GP3CB12-V.
            /// <summary>
            /// T_GP3CB12A_V 9(4)  
            /// </summary>
            [HisFieldInfoMapping(19, 4, CobolType = CobolType.Unsigned)]
            public short T_GP3CB12A_V { get; set; }

            /// <summary>
            /// T_GP3CB12M_V 9(2)  
            /// </summary>
            [HisFieldInfoMapping(20, 2, CobolType = CobolType.Unsigned)]
            public short T_GP3CB12M_V { get; set; }

            /// <summary>
            /// T_GP3CB12G_V 9(2)  
            /// </summary>
            [HisFieldInfoMapping(21, 2, CobolType = CobolType.Unsigned)]
            public short T_GP3CB12G_V { get; set; }
            #endregion Tracciato Host

            #region nested class
            public class T_GP3CK
            {
                #region Properties

                #region Tracciato COBOL
                //                04 T-GP3CK OCCURS 10.
                //                   05 T-GP3CH01        PIC X.
                //                   05 T-GP3CH01B       PIC X.
                //                   05 T-GP3CK04        PIC 9.
                //                   05 T-GP3CK01.
                //                      06 T-GP3CK01A    PIC 9(4).
                //                      06 T-GP3CK01M    PIC 9(2).
                //                   05 T-GP3CK02.
                //                      06 T-GP3CK02A    PIC 9(4).
                //                      06 T-GP3CK02M    PIC 9(2).
                //                   05 T-GP3CK06        PIC X.
                #endregion Tracciato COBOL

                #region Tracciato Host
                // 04 T-GP3CK OCCURS 10.

                /// <summary>
                /// T_GP3CH01 X  
                /// </summary>
                [HisFieldInfoMapping(0, 1)]
                public string T_GP3CH01 { get; set; }

                /// <summary>
                /// T_GP3CH01B X  
                /// </summary>
                [HisFieldInfoMapping(1, 1)]
                public string T_GP3CH01B { get; set; }

                /// <summary>
                /// T_GP3CK04 9  
                /// </summary>
                [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
                public short T_GP3CK04 { get; set; }

                // 05 T-GP3CK01.
                /// <summary>
                /// T_GP3CK01A 9(4)  
                /// </summary>
                [HisFieldInfoMapping(3, 4, CobolType = CobolType.Unsigned)]
                public short T_GP3CK01A { get; set; }

                /// <summary>
                /// T_GP3CK01M 9(2)  
                /// </summary>
                [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
                public short T_GP3CK01M { get; set; }

                // 05 T-GP3CK02.
                /// <summary>
                /// T_GP3CK02A 9(4)  
                /// </summary>
                [HisFieldInfoMapping(5, 4, CobolType = CobolType.Unsigned)]
                public short T_GP3CK02A { get; set; }

                /// <summary>
                /// T_GP3CK02M 9(2)  
                /// </summary>
                [HisFieldInfoMapping(6, 2, CobolType = CobolType.Unsigned)]
                public short T_GP3CK02M { get; set; }

                /// <summary>
                /// T_GP3CK06 X
                /// </summary>
                [HisFieldInfoMapping(7, 1)]
                public string T_GP3CK06 { get; set; }
                #endregion Tracciato Host

                #region nested class

                #endregion nested class

                #endregion Properties
            }
            #endregion nested class

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
