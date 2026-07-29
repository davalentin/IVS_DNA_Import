using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaINAIL
    {
        #region tracciato COBOL
        //             04 N-INAIL.
        //         05 N-RENINAIL           OCCURS 25.
        //* DECORRENZA RENDITA INAIL
        //                15  N-IDECINAA              PIC 9999.
        //                15  N-IDECINAM              PIC 99.
        //            10  N-ICODINAIL                 PIC X.
        //* CODICE RENDITA INAIL  1=SI  2=NO
        //            10  N-IIMPINAIL                 PIC 9(7)V9(4) COMP-3.
        //*EURO  IMPORTO RENDITA INAIL
        #endregion tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 45)]
        public List<RenditaINAIL> RENDITAINAIL { get; set; }

        [HisComplexAreaInfoMapping(1)]
        public Sentenza_IGP1AV91A SENTENZA_IGP1AV91A { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class RenditaINAIL
        {
            #region tracciato COBOL
            //             04 N-INAIL.
            //         05 N-RENINAIL           OCCURS 45.
            //* DECORRENZA RENDITA INAIL
            //                15  N-IDECINAA              PIC 9999.
            //                15  N-IDECINAM              PIC 99.
            //            10  N-ICODINAIL                 PIC X.
            //* CODICE RENDITA INAIL  1=SI  2=NO
            //            10  N-IIMPINAIL                 PIC 9(7)V9(4) COMP-3.
            //*EURO  IMPORTO RENDITA INAIL
            #endregion tracciato COBOL

            #region Tracciato Host
            // 04 N-INAIL.
            // 05 N-RENINAIL           OCCURS 45.
            /// <summary>
            /// N_IDECINAA 9999  
            /// * DECORRENZA RENDITA INAIL
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public short N_IDECINAA { get; set; }

            /// <summary>
            /// N_IDECINAM 99  
            /// * DECORRENZA RENDITA INAIL
            /// </summary>
            [HisFieldInfoMapping(1, 2)]
            public short N_IDECINAM { get; set; }

            /// <summary>
            /// N_ICODINAIL X  
            /// * CODICE RENDITA INAIL  1=SI  2=NO
            /// </summary>
            [HisFieldInfoMapping(2, 1)]
            public string N_ICODINAIL { get; set; }

            /// <summary>
            /// N_IIMPINAIL 9(7)V9(4) COMP-3 
            /// *EURO  IMPORTO RENDITA INAIL
            /// </summary>
            [HisFieldInfoMapping(3, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal N_IIMPINAIL { get; set; }
            #endregion Tracciato Host
        }

        public class Sentenza_IGP1AV91A
        {
            #region Properties

            #region Tracciato COBOL
            //     04 IGP1AV91A           PIC 9.
            #endregion Tracciato COBOL

            #region Tracciato HOST
            /// <summary>
            /// IGP1AV91A           PIC 9.
            /// </summary>
            [HisFieldInfoMapping(0, 1)]
            public short IGP1AV91A { get; set; }
            #endregion Tracciato HOST

            #endregion Properties
        }
        #endregion nested class
    }
}
