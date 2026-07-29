using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaLavEsteroPrePens
    {
        #region tracciato COBOL
        //                04  AREABLE.
        //         05  IELBLE  OCCURS 10.
        //             10  IELBLE1AA                  PIC 9(4).
        //*ANN0
        //*MESE DAL E MESE AL
        //                 15  IELBLE2M1                  PIC 99.
        //                 15  IELBLE2M2                  PIC 99.
        //*PERIODI DI LAVORO ALL'ESTERO POST PENSIONAMENTO
        //*PER I REDDITI DEL CORRISPONDENTE PERIODO VEDI IELBLE3RED
        #endregion tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 10)]
        public List<LavoroEstero> LAVORIESTERI { get; set; }
        #endregion Tracciato Host


        #region nested class
        public class LavoroEstero
        {
            #region tracciato COBOL
            //                04  AREABLE.
            //         05  IELBLE  OCCURS 10.
            //             10  IELBLE1AA                  PIC 9(4).
            //*ANN0
            //*MESE DAL E MESE AL
            //                 15  IELBLE2M1                  PIC 99.
            //                 15  IELBLE2M2                  PIC 99.
            //*PERIODI DI LAVORO ALL'ESTERO POST PENSIONAMENTO
            //*PER I REDDITI DEL CORRISPONDENTE PERIODO VEDI IELBLE3RED
            #endregion tracciato COBOL

            #region Tracciato Host
            // 04  AREABLE.
            // 05  IELBLE  OCCURS 10.
            /// <summary>
            /// IELBLE1AA 9(4)  
            /// *ANN0
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public short IELBLE1AA { get; set; }

            /// <summary>
            /// IELBLE2M1 99  
            /// *MESE DAL 
            /// </summary>
            [HisFieldInfoMapping(1, 2)]
            public short IELBLE2M1 { get; set; }

            /// <summary>
            /// IELBLE2M2 99 E MESE AL
            /// *PERIODI DI LAVORO ALL'ESTERO POST PENSIONAMENTO
            /// *PER I REDDITI DEL CORRISPONDENTE PERIODO VEDI IELBLE3RED 
            /// </summary>
            [HisFieldInfoMapping(2, 2)]
            public short IELBLE2M2 { get; set; }

            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
