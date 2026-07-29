using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostResponse.AreaStampa
{
    public class Intestazione
    {
        #region Constructor
        internal Intestazione()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        // 01  REC01.
        //*
        //     02 CHIAVE1.
        //*                             CHIAVE PRIMARIA                000
        //        03 TIPO-ELAB     PIC X(02).
        //*                             TIPO ELAB.( PL-RI-FL-FR-VL-VR) 002
        //*                             PL = PRIME LIQUIDATE FONDI AGO
        //*                             RI = RICOSTITUZIONI FONDI AGO
        //*                             FL = PRIME LIQUIDATE FONDI SPEC.
        //*                             FR = RICOSTITUZIONI FONDI SPEC.
        //*                             VL = VERIFY P.L. FONDI SPEC.
        //*                             VR = VERIFY RICOST. FONDI SPEC.
        //        03 COD-CAT       PIC 9(03).
        //*                             CODICE CATEGORIA               005
        //        03 CERT          PIC 9(08).
        //*                             CERTIFICATO                    013
        //        03 DATA-CALC     PIC 9(08).
        //*                             DATA ELAB.(AAAAMMGG)           021
        //     02 CATEGO           PIC X(08).
        //*                             CATEGORIA IN CHIARO            029
        //     02 TIPO-ERR         PIC X(04).
        //*                                                            033
        //     02 DESCR-ERR        PIC X(17).
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 01  REC01.
        //*
        // 02 CHIAVE1.
        // *                             CHIAVE PRIMARIA                000
        /// <summary>
        /// TIPO_ELAB X(02)  
        /// </summary>
        [HisFieldInfoMapping(0, 2)]
        public string TIPO_ELAB { get; set; }

        // *                             TIPO ELAB.( PL-RI-FL-FR-VL-VR) 002
        // *                             PL = PRIME LIQUIDATE FONDI AGO
        // *                             RI = RICOSTITUZIONI FONDI AGO
        // *                             FL = PRIME LIQUIDATE FONDI SPEC.
        // *                             FR = RICOSTITUZIONI FONDI SPEC.
        // *                             VL = VERIFY P.L. FONDI SPEC.
        // *                             VR = VERIFY RICOST. FONDI SPEC.
        /// <summary>
        /// COD_CAT 9(03)  
        /// </summary>
        [HisFieldInfoMapping(1, 3, CobolType = CobolType.Unsigned)]
        public short COD_CAT { get; set; }

        // *                             CODICE CATEGORIA               005
        /// <summary>
        /// CERT 9(08)  
        /// </summary>
        [HisFieldInfoMapping(2, 8, CobolType = CobolType.Unsigned)]
        public int CERT { get; set; }

        // *                             CERTIFICATO                    013
        /// <summary>
        /// DATA_CALC 9(08)  
        /// </summary>
        [HisFieldInfoMapping(3, 8, CobolType = CobolType.Unsigned)]
        public int DATA_CALC { get; set; }

        // *                             DATA ELAB.(AAAAMMGG)           021
        /// <summary>
        /// CATEGO X(08)  
        /// </summary>
        [HisFieldInfoMapping(4, 8)]
        public string CATEGO { get; set; }

        // *                             CATEGORIA IN CHIARO            029
        /// <summary>
        /// TIPO_ERR X(04)  
        /// </summary>
        [HisFieldInfoMapping(5, 4)]
        public string TIPO_ERR { get; set; }

        // *                                                            033
        /// <summary>
        /// DESCR_ERR X(17)  
        /// </summary>
        [HisFieldInfoMapping(6, 17)]
        public string DESCR_ERR { get; set; }

        #endregion Tracciato Host

        #region nested class

        #endregion nested class

        #endregion Properties
    }
}

