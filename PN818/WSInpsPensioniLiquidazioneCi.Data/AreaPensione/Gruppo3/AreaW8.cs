using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaW8
    {
        #region tracciato COBOL
        //        *
        //*****************************************************************
        //*  DATI REDDITUALI PER INTEGRAZIONE VIRTUALE   9*10 = 90
        //*****************************************************************
        //     04 AREAWK8.
        //*DATI REDDITUALI PER INTEGRAZIONE VIRTUALE
        //         05  IWK8.
        //             10  IELWK8 OCCURS 10.
        //                 15  IW8DEC        PIC 9999.
        //*ANNO DI RIFERIMENTO
        //                 15  IW8RED        PIC S9(7)V9(4)   COMP-3.
        //*EURO REDDITO PER VIRTUALE
        #endregion tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 10)]
        public List<RedditiIntegrazioneVirtuale> REDDITIINTEGRAZIONEVIRTUALE { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class RedditiIntegrazioneVirtuale
        {
            #region tracciato COBOL
            //        *
            //*****************************************************************
            //*  DATI REDDITUALI PER INTEGRAZIONE VIRTUALE   9*10 = 90
            //*****************************************************************
            //     04 AREAWK8.
            //*DATI REDDITUALI PER INTEGRAZIONE VIRTUALE
            //         05  IWK8.
            //             10  IELWK8 OCCURS 10.
            //                 15  IW8DEC        PIC 9999.
            //*ANNO DI RIFERIMENTO
            //                 15  IW8RED        PIC S9(7)V9(4)   COMP-3.
            //*EURO REDDITO PER VIRTUALE
            #endregion tracciato COBOL

            #region Tracciato Host
            //*
            //*****************************************************************
            // *  DATI REDDITUALI PER INTEGRAZIONE VIRTUALE   9*10 = 90
            //*****************************************************************
            // 04 AREAWK8.
            // *DATI REDDITUALI PER INTEGRAZIONE VIRTUALE
            // 05  IWK8.
            // 10  IELWK8 OCCURS 10.
            /// <summary>
            /// IW8DEC 9999  
            // *ANNO DI RIFERIMENTO
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public short IW8DEC { get; set; }

            /// <summary>
            /// IW8RED S9(7)V9(4) COMP-3
            /// *EURO REDDITO PER VIRTUALE 
            /// </summary>
            [HisFieldInfoMapping(1, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal IW8RED { get; set; }

            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
