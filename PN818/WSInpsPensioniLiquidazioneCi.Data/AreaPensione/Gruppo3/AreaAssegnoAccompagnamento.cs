using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaAssegnoAccompagnamento
    {
        #region tracciato COBOL
        //        *******DATI ALTRI ASSEGNI O RENDITE
        //* 1996 ASSEGNO DI ACCOMPAGNAMENTO.     TOT. 36 BYTE
        //     04  IACCOMPA.
        //        05  IACCOMP       OCCURS 3.
        //* DECORRENZA
        //                 20  IDECACCA           PIC 9999.
        //                 20  IDECACCM           PIC 99.
        //* CESSAZIONE
        //                 20  ICESACCA           PIC 9999.
        //                 20  ICESACCM           PIC 99.
        #endregion tracciato COBOL

        #region Tracciato Host

        [HisComplexAreaInfoMapping(0, ListCount = 3)]
        public List<Accompagnamento> ACCOMPAGNAMENTI { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class Accompagnamento
        {
            #region tracciato COBOL
            //        *******DATI ALTRI ASSEGNI O RENDITE
            //* 1996 ASSEGNO DI ACCOMPAGNAMENTO.     TOT. 36 BYTE
            //     04  IACCOMPA.
            //        05  IACCOMP       OCCURS 3.
            //* DECORRENZA
            //                 20  IDECACCA           PIC 9999.
            //                 20  IDECACCM           PIC 99.
            //* CESSAZIONE
            //                 20  ICESACCA           PIC 9999.
            //                 20  ICESACCM           PIC 99.
            #endregion tracciato COBOL

            #region Tracciato Host
            //*******DATI ALTRI ASSEGNI O RENDITE
            // * 1996 ASSEGNO DI ACCOMPAGNAMENTO.     TOT. 36 BYTE
            // 04  IACCOMPA.
            // 05  IACCOMP       OCCURS 3.
            /// <summary>
            /// IDECACCA 9999  
            /// 
            /// * DECORRENZA
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public short IDECACCA { get; set; }

            /// <summary>
            /// IDECACCM 99  
            /// 
            /// * DECORRENZA
            /// </summary>
            [HisFieldInfoMapping(1, 2)]
            public short IDECACCM { get; set; }

            /// <summary>
            /// ICESACCA 9999 
            /// * CESSAZIONE 
            /// </summary>
            [HisFieldInfoMapping(2, 4)]
            public short ICESACCA { get; set; }

            /// <summary>
            /// ICESACCM 99  
            /// * CESSAZIONE
            /// </summary>
            [HisFieldInfoMapping(3, 2)]
            public short ICESACCM { get; set; }
            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
