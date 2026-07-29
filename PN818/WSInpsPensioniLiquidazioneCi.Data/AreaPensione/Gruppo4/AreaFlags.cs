using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaFlags
    {
        #region tracciato COBOL
        //  04  FLAGS.
        //* VALIDI SOLO PER PC
        //         05  TIPO-CALC                     PIC X.
        //         05  FLAG-FASE                     PIC 99.
        //         05  FLAG-CK                       PIC 9.
        //* 0=INCOMPLETA  1=COMPLETA   2=COMPLETA E TRASMESSA
        //         05  FLAG-PROVA-ESTESA             PIC 9.
        //         05  FLAG-STAMPA                   PIC 9.
        //* 0=DA STAMPARE SU PC 1=GIA' STAMPATA
        //* DECORRENZA ORIGINARIA-ORIGINARIA
        //             10  IW1ORIGA        PIC 9999.
        //             10  IW1ORIGM        PIC 99.
        //             15  W-CAMPO36        PIC XXX.
        //             15  FILLER           PIC X.
        #endregion tracciato COBOL

        #region Tracciato Host
        // 04  FLAGS.
        // * VALIDI SOLO PER PC
        /// <summary>
        /// TIPO_CALC X  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string TIPO_CALC { get; set; }

        /// <summary>
        /// FLAG_FASE 99  
        /// </summary>
        [HisFieldInfoMapping(1, 2)]
        public short FLAG_FASE { get; set; }

        /// <summary>
        /// FLAG_CK 9  
        /// * 0=INCOMPLETA  1=COMPLETA   2=COMPLETA E TRASMESSA
        /// </summary>
        [HisFieldInfoMapping(2, 1)]
        public short FLAG_CK { get; set; }

        /// <summary>
        /// FLAG_PROVA_ESTESA 9  
        /// </summary>
        [HisFieldInfoMapping(3, 1)]
        public short FLAG_PROVA_ESTESA { get; set; }

        /// <summary>
        /// FLAG_STAMPA 9  
        /// * 0=DA STAMPARE SU PC 1=GIA' STAMPATA
        /// </summary>
        [HisFieldInfoMapping(4, 1)]
        public short FLAG_STAMPA { get; set; }

        /// <summary>
        /// IW1ORIGA 9999  
        /// * DECORRENZA ORIGINARIA-ORIGINARIA
        /// </summary>
        [HisFieldInfoMapping(5, 4)]
        public short IW1ORIGA { get; set; }

        /// <summary>
        /// IW1ORIGM 99  
        /// * DECORRENZA ORIGINARIA-ORIGINARIA
        /// </summary>
        [HisFieldInfoMapping(6, 2)]
        public short IW1ORIGM { get; set; }

        /// <summary>
        /// W_CAMPO36 XXX  
        /// </summary>
        [HisFieldInfoMapping(7, 3)]
        public string W_CAMPO36 { get; set; }

        /// <summary>
        /// FILLER X  
        /// </summary>
        [HisFieldInfoMapping(8, 1)]
        public string FILLER { get; set; }


        #endregion Tracciato Host
    }
}
