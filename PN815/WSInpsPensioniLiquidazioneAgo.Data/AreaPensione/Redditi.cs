using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class Redditi
    {
        #region Constructor
        public Redditi()
        {
            this.RedditiSentenza495_93 = new Sentenza495_93();
            this.RedditiSentenza240_94 = new Sentenza240_94();
            this.RedditiMaggiorazione = new Maggiorazione();
        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //*DATI DEL PANNELLO MRCRED1 (REDDITI PER SENTENZA 495/93)
        //     02 T-GPRED1.
        //        03 T-GP7LKE0Z OCCURS 50.
        //           04 T-GP7LKE1           PIC 9(4).
        //           04 T-GP7LKE2           PIC S9(7)V9(2) COMP-3.
        //           04 T-GP7LKE3           PIC S9(7)V9(2) COMP-3.
        //*LOMAR 03/11/2010 - I           
        //           04 T-GP7LKE2CP         PIC X.
        //           04 T-GP7LKE2CD         PIC X.
        //           04 T-GP7LKE2P          PIC S9(7)V9(2) COMP-3.
        //           04 T-GP7LKE2D          PIC S9(7)V9(2) COMP-3.
        //           04 T-GP7LKE3CP         PIC X.
        //           04 T-GP7LKE3CD         PIC X.
        //           04 T-GP7LKE3P          PIC S9(7)V9(2) COMP-3.
        //           04 T-GP7LKE3D          PIC S9(7)V9(2) COMP-3.
        //           04 T-GP7LKE4.
        //              05 T-GP7LKE4A       PIC 9. 
        //              05 T-GP7LKE4B       PIC 99.
        //              05 T-GP7LKE4C       PIC 99.
        //              05 T-GP7LKE4D       PIC 99.
        //*LOMAR 03/11/2010 - F

        //*DATI DEL PANNELLO MRCRED2 (REDDITI PER SENTENZA 240/94)
        //     02 T-GPRED2.
        //        03 T-GP2RSEN.
        //           04 T-GP2RS00  OCCURS 30 .
        //              05 T-GP2RS01        PIC 9(4).
        //              05 T-GP2RS02        PIC S9(7)V9(2) COMP-3.
        //              05 T-GP2RS11.
        //                 06 T-GP2RS11A    PIC 9.
        //                 06 T-GP2RS11B    PIC 9(2).
        //                 06 T-GP2RS11C    PIC 9(2).
        //                 06 T-GP2RS11D    PIC 9(2).
        //              05 T-GP2RSRIL       PIC X(20).
        //*DATI DEL PANNELLO MRCRMS0 (REDDITI PER MAGGIORAZIONI)
        //     02 T-GPRMS0.
        //        03 T-GP2KM50  OCCURS 30.
        //           04 T-GP2KM51           PIC 9(4).
        //           04 T-GP2KM5A.
        //              05 T-GP2KM5AA       PIC 9(4).
        //              05 T-GP2KM5AM       PIC 9(2).
        //           04 T-GP2KM52           PIC 9(2).
        //           04 T-GP2KM53           PIC S9(7)V9(2) COMP-3.
        //           04 T-GP2KM54           PIC S9(7)V9(2) COMP-3.
        //           04 T-GP2KM11.
        //              05 T-GP2KM11A       PIC 9.
        //              05 T-GP2KM11B       PIC 9(2).
        //              05 T-GP2KM11C       PIC 9(2).
        //              05 T-GP2KM11D       PIC 9(2).
        //           04 T-GP2KMRIL          PIC X(20).
        //        03 T-GP2BN51.
        //           04 T-GP2BN51A          PIC 9(4).
        //           04 T-GP2BN51M          PIC 9(2).
        //        03 T-GP1AF07.
        //           04 T-GP1AF07A          PIC 9(4).
        //           04 T-GP1AF07M          PIC 9(2).
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0)]
        public Sentenza495_93 RedditiSentenza495_93 { get; set; }

        [HisComplexAreaInfoMapping(1)]
        public Sentenza240_94 RedditiSentenza240_94 { get; set; }

        [HisComplexAreaInfoMapping(2)]
        public Maggiorazione RedditiMaggiorazione { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class Sentenza495_93
        {
            #region Properties

            #region Tracciato COBOL
            //            *DATI DEL PANNELLO MRCRED1 (REDDITI PER SENTENZA 495/93)
            //     02 T-GPRED1.
            //        03 T-GP7LKE0Z OCCURS 50.
            //           04 T-GP7LKE1           PIC 9(4).
            //           04 T-GP7LKE2           PIC S9(7)V9(2) COMP-3.
            //           04 T-GP7LKE3           PIC S9(7)V9(2) COMP-3.
            //*LOMAR 03/11/2010 - I           
            //           04 T-GP7LKE2CP         PIC X.
            //           04 T-GP7LKE2CD         PIC X.
            //           04 T-GP7LKE2P          PIC S9(7)V9(2) COMP-3.
            //           04 T-GP7LKE2D          PIC S9(7)V9(2) COMP-3.
            //           04 T-GP7LKE3CP         PIC X.
            //           04 T-GP7LKE3CD         PIC X.
            //           04 T-GP7LKE3P          PIC S9(7)V9(2) COMP-3.
            //           04 T-GP7LKE3D          PIC S9(7)V9(2) COMP-3.
            //           04 T-GP7LKE4.
            //              05 T-GP7LKE4A       PIC 9. 
            //              05 T-GP7LKE4B       PIC 99.
            //              05 T-GP7LKE4C       PIC 99.
            //              05 T-GP7LKE4D       PIC 99.
            //*LOMAR 03/11/2010 - F
            #endregion Tracciato COBOL

            #region Tracciato Host
            [HisComplexAreaInfoMapping(0, ListCount = 50)]
            public List<T_GP7LKE0Z> LISTT_GP7LKE0Z { get; set; }
            #endregion Tracciato Host

            #region nested class
            public class T_GP7LKE0Z
            {
                #region Properties

                #region Tracciato COBOL
                //            *DATI DEL PANNELLO MRCRED1 (REDDITI PER SENTENZA 495/93)
                //     02 T-GPRED1.
                //        03 T-GP7LKE0Z OCCURS 50.
                //           04 T-GP7LKE1           PIC 9(4).
                //           04 T-GP7LKE2           PIC S9(7)V9(2) COMP-3.
                //           04 T-GP7LKE3           PIC S9(7)V9(2) COMP-3.
                //*LOMAR 03/11/2010 - I           
                //           04 T-GP7LKE2CP         PIC X.
                //           04 T-GP7LKE2CD         PIC X.
                //           04 T-GP7LKE2P          PIC S9(7)V9(2) COMP-3.
                //           04 T-GP7LKE2D          PIC S9(7)V9(2) COMP-3.
                //           04 T-GP7LKE3CP         PIC X.
                //           04 T-GP7LKE3CD         PIC X.
                //           04 T-GP7LKE3P          PIC S9(7)V9(2) COMP-3.
                //           04 T-GP7LKE3D          PIC S9(7)V9(2) COMP-3.
                //           04 T-GP7LKE4.
                //              05 T-GP7LKE4A       PIC 9. 
                //              05 T-GP7LKE4B       PIC 99.
                //              05 T-GP7LKE4C       PIC 99.
                //              05 T-GP7LKE4D       PIC 99.
                //*LOMAR 03/11/2010 - F
                #endregion Tracciato COBOL

                #region Tracciato Host
                //*DATI DEL PANNELLO MRCRED1 (REDDITI PER SENTENZA 495/93)
                // 02 T-GPRED1.
                // 03 T-GP7LKE0Z OCCURS 50.
                /// <summary>
                /// T_GP7LKE1 9(4)  
                /// </summary>
                [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
                public short T_GP7LKE1 { get; set; }

                /// <summary>
                /// T_GP7LKE2 S9(7)V9(2) COMP-3 
                /// </summary>
                [HisFieldInfoMapping(1, 5, Scale = 2, CobolType = CobolType.Comp3)]
                public decimal T_GP7LKE2 { get; set; }

                /// <summary>
                /// T_GP7LKE3 S9(7)V9(2) COMP-3 
                /// </summary>
                [HisFieldInfoMapping(2, 5, Scale = 2, CobolType = CobolType.Comp3)]
                public decimal T_GP7LKE3 { get; set; }

                // *LOMAR 03/11/2010 - I
                /// <summary>
                /// T_GP7LKE2CP X  
                /// </summary>
                [HisFieldInfoMapping(3, 1)]
                public string T_GP7LKE2CP { get; set; }

                /// <summary>
                /// T_GP7LKE2CD X  
                /// </summary>
                [HisFieldInfoMapping(4, 1)]
                public string T_GP7LKE2CD { get; set; }

                /// <summary>
                /// T_GP7LKE2P S9(7)V9(2) COMP-3 
                /// </summary>
                [HisFieldInfoMapping(5, 5, Scale = 2, CobolType = CobolType.Comp3)]
                public decimal T_GP7LKE2P { get; set; }

                /// <summary>
                /// T_GP7LKE2D S9(7)V9(2) COMP-3 
                /// </summary>
                [HisFieldInfoMapping(6, 5, Scale = 2, CobolType = CobolType.Comp3)]
                public decimal T_GP7LKE2D { get; set; }

                /// <summary>
                /// T_GP7LKE3CP X  
                /// </summary>
                [HisFieldInfoMapping(7, 1)]
                public string T_GP7LKE3CP { get; set; }

                /// <summary>
                /// T_GP7LKE3CD X  
                /// </summary>
                [HisFieldInfoMapping(8, 1)]
                public string T_GP7LKE3CD { get; set; }

                /// <summary>
                /// T_GP7LKE3P S9(7)V9(2) COMP-3 
                /// </summary>
                [HisFieldInfoMapping(9, 5, Scale = 2, CobolType = CobolType.Comp3)]
                public decimal T_GP7LKE3P { get; set; }

                /// <summary>
                /// T_GP7LKE3D S9(7)V9(2) COMP-3 
                /// </summary>
                [HisFieldInfoMapping(10, 5, Scale = 2, CobolType = CobolType.Comp3)]
                public decimal T_GP7LKE3D { get; set; }

                // 04 T-GP7LKE4.
                /// <summary>
                /// T_GP7LKE4A 9  
                /// </summary>
                [HisFieldInfoMapping(11, 1, CobolType = CobolType.Unsigned)]
                public short T_GP7LKE4A { get; set; }

                /// <summary>
                /// T_GP7LKE4B 99  
                /// </summary>
                [HisFieldInfoMapping(12, 2, CobolType = CobolType.Unsigned)]
                public short T_GP7LKE4B { get; set; }

                /// <summary>
                /// T_GP7LKE4C 99  
                /// </summary>
                [HisFieldInfoMapping(13, 2, CobolType = CobolType.Unsigned)]
                public short T_GP7LKE4C { get; set; }

                /// <summary>
                /// T_GP7LKE4D 99  
                /// </summary>
                [HisFieldInfoMapping(14, 2, CobolType = CobolType.Unsigned)]
                public short T_GP7LKE4D { get; set; }

                // *LOMAR 03/11/2010 - F
                #endregion Tracciato Host

                #endregion Properties
            }
            #endregion nested class

            #endregion Properties
        }

        public class Sentenza240_94
        {
            #region Properties

            #region Tracciato COBOL
            //       *DATI DEL PANNELLO MRCRED2 (REDDITI PER SENTENZA 240/94)
            //02 T-GPRED2.
            //   03 T-GP2RSEN.
            //      04 T-GP2RS00  OCCURS 30 .
            //         05 T-GP2RS01        PIC 9(4).
            //         05 T-GP2RS02        PIC S9(7)V9(2) COMP-3.
            //         05 T-GP2RS11.
            //            06 T-GP2RS11A    PIC 9.
            //            06 T-GP2RS11B    PIC 9(2).
            //            06 T-GP2RS11C    PIC 9(2).
            //            06 T-GP2RS11D    PIC 9(2).
            //         05 T-GP2RSRIL       PIC X(20).
            #endregion Tracciato COBOL

            #region Tracciato Host
            [HisComplexAreaInfoMapping(0, ListCount = 30)]
            public List<T_GP2RS00> LISTT_GP2RS00 { get; set; }
            #endregion Tracciato Host

            #region nested class
            public class T_GP2RS00
            {
                #region Properties

                #region Tracciato COBOL
                //       *DATI DEL PANNELLO MRCRED2 (REDDITI PER SENTENZA 240/94)
                //02 T-GPRED2.
                //   03 T-GP2RSEN.
                //      04 T-GP2RS00  OCCURS 30 .
                //         05 T-GP2RS01        PIC 9(4).
                //         05 T-GP2RS02        PIC S9(7)V9(2) COMP-3.
                //         05 T-GP2RS11.
                //            06 T-GP2RS11A    PIC 9.
                //            06 T-GP2RS11B    PIC 9(2).
                //            06 T-GP2RS11C    PIC 9(2).
                //            06 T-GP2RS11D    PIC 9(2).
                //         05 T-GP2RSRIL       PIC X(20).
                #endregion Tracciato COBOL

                #region Tracciato Host
                // *DATI DEL PANNELLO MRCRED2 (REDDITI PER SENTENZA 240/94)
                // 02 T-GPRED2.
                // 03 T-GP2RSEN.
                // 04 T-GP2RS00  OCCURS 30 .
                /// <summary>
                /// T_GP2RS01 9(4)  
                /// </summary>
                [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
                public short T_GP2RS01 { get; set; }

                /// <summary>
                /// T_GP2RS02 S9(7)V9(2) COMP-3 
                /// </summary>
                [HisFieldInfoMapping(1, 5, Scale = 2, CobolType = CobolType.Comp3)]
                public decimal T_GP2RS02 { get; set; }

                // 05 T-GP2RS11.
                /// <summary>
                /// T_GP2RS11A 9  
                /// </summary>
                [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
                public short T_GP2RS11A { get; set; }

                /// <summary>
                /// T_GP2RS11B 9(2)  
                /// </summary>
                [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
                public short T_GP2RS11B { get; set; }

                /// <summary>
                /// T_GP2RS11C 9(2)  
                /// </summary>
                [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
                public short T_GP2RS11C { get; set; }

                /// <summary>
                /// T_GP2RS11D 9(2)  
                /// </summary>
                [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
                public short T_GP2RS11D { get; set; }

                /// <summary>
                /// T_GP2RSRIL X(20)  
                /// </summary>
                [HisFieldInfoMapping(6, 20)]
                public string T_GP2RSRIL { get; set; }
                #endregion Tracciato Host

                #endregion Properties
            }
            #endregion nested class

            #endregion Properties
        }

        public class Maggiorazione
        {
            #region Properties

            #region Tracciato COBOL
            //       *DATI DEL PANNELLO MRCRMS0 (REDDITI PER MAGGIORAZIONI)
            //02 T-GPRMS0.
            //   03 T-GP2KM50  OCCURS 30.
            //      04 T-GP2KM51           PIC 9(4).
            //      04 T-GP2KM5A.
            //         05 T-GP2KM5AA       PIC 9(4).
            //         05 T-GP2KM5AM       PIC 9(2).
            //      04 T-GP2KM52           PIC 9(2).
            //      04 T-GP2KM53           PIC S9(7)V9(2) COMP-3.
            //      04 T-GP2KM54           PIC S9(7)V9(2) COMP-3.
            //      04 T-GP2KM11.
            //         05 T-GP2KM11A       PIC 9.
            //         05 T-GP2KM11B       PIC 9(2).
            //         05 T-GP2KM11C       PIC 9(2).
            //         05 T-GP2KM11D       PIC 9(2).
            //      04 T-GP2KMRIL          PIC X(20).
            //   03 T-GP2BN51.
            //      04 T-GP2BN51A          PIC 9(4).
            //      04 T-GP2BN51M          PIC 9(2).
            //   03 T-GP1AF07.
            //      04 T-GP1AF07A          PIC 9(4).
            //      04 T-GP1AF07M          PIC 9(2).
            #endregion Tracciato COBOL

            #region Tracciato Host
            [HisComplexAreaInfoMapping(0, ListCount = 30)]
            public List<T_GP2KM50> LISTT_GP2KM50 { get; set; }

            // 03 T-GP2BN51.
            /// <summary>
            /// T_GP2BN51A 9(4)  
            /// </summary>
            [HisFieldInfoMapping(1, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BN51A { get; set; }

            /// <summary>
            /// T_GP2BN51M 9(2)  
            /// </summary>
            [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BN51M { get; set; }

            // 03 T-GP1AF07.
            /// <summary>
            /// T_GP1AF07A 9(4)  
            /// </summary>
            [HisFieldInfoMapping(3, 4, CobolType = CobolType.Unsigned)]
            public short T_GP1AF07A { get; set; }

            /// <summary>
            /// T_GP1AF07M 9(2)  
            /// </summary>
            [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
            public short T_GP1AF07M { get; set; }
            #endregion Tracciato Host

            #region nested class
            public class T_GP2KM50
            {
                #region Properties

                #region Tracciato COBOL
                //       *DATI DEL PANNELLO MRCRMS0 (REDDITI PER MAGGIORAZIONI)
                //02 T-GPRMS0.
                //   03 T-GP2KM50  OCCURS 30.
                //      04 T-GP2KM51           PIC 9(4).
                //      04 T-GP2KM5A.
                //         05 T-GP2KM5AA       PIC 9(4).
                //         05 T-GP2KM5AM       PIC 9(2).
                //      04 T-GP2KM52           PIC 9(2).
                //      04 T-GP2KM53           PIC S9(7)V9(2) COMP-3.
                //      04 T-GP2KM54           PIC S9(7)V9(2) COMP-3.
                //      04 T-GP2KM11.
                //         05 T-GP2KM11A       PIC 9.
                //         05 T-GP2KM11B       PIC 9(2).
                //         05 T-GP2KM11C       PIC 9(2).
                //         05 T-GP2KM11D       PIC 9(2).
                //      04 T-GP2KMRIL          PIC X(20).
                #endregion Tracciato COBOL

                #region Tracciato Host
                // *DATI DEL PANNELLO MRCRMS0 (REDDITI PER MAGGIORAZIONI)
                // 02 T-GPRMS0.
                // 03 T-GP2KM50  OCCURS 30.
                /// <summary>
                /// T_GP2KM51 9(4)  
                /// </summary>
                [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
                public short T_GP2KM51 { get; set; }

                // 04 T-GP2KM5A.
                /// <summary>
                /// T_GP2KM5AA 9(4)  
                /// </summary>
                [HisFieldInfoMapping(1, 4, CobolType = CobolType.Unsigned)]
                public short T_GP2KM5AA { get; set; }

                /// <summary>
                /// T_GP2KM5AM 9(2)  
                /// </summary>
                [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
                public short T_GP2KM5AM { get; set; }

                /// <summary>
                /// T_GP2KM52 9(2)  
                /// </summary>
                [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
                public short T_GP2KM52 { get; set; }

                /// <summary>
                /// T_GP2KM53 S9(7)V9(2) COMP-3 
                /// </summary>
                [HisFieldInfoMapping(4, 5, Scale = 2, CobolType = CobolType.Comp3)]
                public decimal T_GP2KM53 { get; set; }

                /// <summary>
                /// T_GP2KM54 S9(7)V9(2) COMP-3 
                /// </summary>
                [HisFieldInfoMapping(5, 5, Scale = 2, CobolType = CobolType.Comp3)]
                public decimal T_GP2KM54 { get; set; }

                // 04 T-GP2KM11.
                /// <summary>
                /// T_GP2KM11A 9  
                /// </summary>
                [HisFieldInfoMapping(6, 1, CobolType = CobolType.Unsigned)]
                public short T_GP2KM11A { get; set; }

                /// <summary>
                /// T_GP2KM11B 9(2)  
                /// </summary>
                [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
                public short T_GP2KM11B { get; set; }

                /// <summary>
                /// T_GP2KM11C 9(2)  
                /// </summary>
                [HisFieldInfoMapping(8, 2, CobolType = CobolType.Unsigned)]
                public short T_GP2KM11C { get; set; }

                /// <summary>
                /// T_GP2KM11D 9(2)  
                /// </summary>
                [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
                public short T_GP2KM11D { get; set; }

                /// <summary>
                /// T_GP2KMRIL X(20)  
                /// </summary>
                [HisFieldInfoMapping(10, 20)]
                public string T_GP2KMRIL { get; set; }
                #endregion Tracciato Host

                #endregion Properties
            }
            #endregion nested class

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
