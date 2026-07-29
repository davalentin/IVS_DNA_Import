using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaSettimaneEst
    {
        #region tracciato COBOL
        //       04 SET233.
        //* SETT.ESTERE X 233/503/335 A (RI)CALCOLO POST 0790:
        //        05 SETOR233     OCCURS 3.
        //           10 SETOR233Y    OCCURS 2.
        //                 15 DEC233X.
        //                     20 DEC233A         PIC 9(4).
        //                     20 DEC233M         PIC 9(2).
        //*DECORRENZA DELLE SETTIMANE ESTERE PER 233, 503, 335
        //                 15 GEST233         PIC 9(2).
        //*CODICE GESTIONE DELLE SETTIMANE ESTERE PER 233, 503, 335:
        //* 71 72 73 74 PER 233
        //* 61 62 63 64 PER 503
        //* 01 02 03 04 PER 335
        //                 15 SETRI233        PIC S9(5) COMP-3.
        //*SETTIMANE ESTERE PER 233, 503, 335 ALLE VARIE DECORRENZE
        #endregion tracciato COBOL

        #region Tracciato Host

        [HisComplexAreaInfoMapping(0, ListCount = 6)]
        public List<SettimaneEstere> SETTIMANEESTERE { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class SettimaneEstere
        {
            #region tracciato COBOL
            //       04 SET233.
            //* SETT.ESTERE X 233/503/335 A (RI)CALCOLO POST 0790:
            //        05 SETOR233     OCCURS 3.
            //           10 SETOR233Y    OCCURS 2.
            //                 15 DEC233X.
            //                     20 DEC233A         PIC 9(4).
            //                     20 DEC233M         PIC 9(2).
            //*DECORRENZA DELLE SETTIMANE ESTERE PER 233, 503, 335
            //                 15 GEST233         PIC 9(2).
            //*CODICE GESTIONE DELLE SETTIMANE ESTERE PER 233, 503, 335:
            //* 71 72 73 74 PER 233
            //* 61 62 63 64 PER 503
            //* 01 02 03 04 PER 335
            //                 15 SETRI233        PIC S9(5) COMP-3.
            //*SETTIMANE ESTERE PER 233, 503, 335 ALLE VARIE DECORRENZE
            #endregion tracciato COBOL

            #region Tracciato Host
            // 04 SET233.
            // * SETT.ESTERE X 233/503/335 A (RI)CALCOLO POST 0790:
            // 05 SETOR233     OCCURS 3.
            // 10 SETOR233Y    OCCURS 2.
            // 15 DEC233X.
            /// <summary>
            /// DEC233A 9(4)  
            /// *DECORRENZA DELLE SETTIMANE ESTERE PER 233, 503, 335
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public short DEC233A { get; set; }

            /// <summary>
            /// DEC233M 9(2)  
            /// *DECORRENZA DELLE SETTIMANE ESTERE PER 233, 503, 335
            /// </summary>
            [HisFieldInfoMapping(1, 2)]
            public short DEC233M { get; set; }

            /// <summary>
            /// GEST233 9(2)  
            /// *CODICE GESTIONE DELLE SETTIMANE ESTERE PER 233, 503, 335:
            /// </summary>
            [HisFieldInfoMapping(2, 2)]
            public short GEST233 { get; set; }

            // * 71 72 73 74 PER 233
            // * 61 62 63 64 PER 503
            // * 01 02 03 04 PER 335
            /// <summary>
            /// SETRI233 S9(5) COMP-3 
            /// *SETTIMANE ESTERE PER 233, 503, 335 ALLE VARIE DECORRENZE
            /// </summary>
            [HisFieldInfoMapping(3, 3, CobolType = CobolType.Comp3)]
            public int SETRI233 { get; set; }


            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
