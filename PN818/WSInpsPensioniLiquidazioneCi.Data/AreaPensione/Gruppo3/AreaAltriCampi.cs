using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaAltriCampi
    {
        #region tracciato COBOL
        //   04  ALTRI-CAMPI.
        //* DECORRENZA SINDACALI
        //             10  DECSINDA     PIC 9999.
        //             10  DECSINDM     PIC 99.
        //*
        //         05  TRESTEROUP       PIC XXX.
        //*UFFICIO PAGATORE PER TRATTENUTE PER ESTERO
        //         05  TRESTERO         PIC S9 COMP-3.
        //*TRATTENUTE PER ESTERO 1=SI


        //*****************************************************************
        //*  DECORRENZA PRECEDENTE LIQUIDAZIONE DEGLI STATI ESTERI
        //*****************************************************************
        //         05  V-IPLIQ.
        //              10  V-IPRECLIQ OCCURS 4.
        //*+DATA PRECEDENTE LIQUIDAZIONE
        //                     20 V-IDAPLIQA            PIC 9999.
        //                     20 V-IDAPLIQM            PIC 99.
        //*2004 CAMPI V- SPOSTATI NELL'AREA STATI:   RESTANO 24 FILLER


        //         05  INIFINASS.
        //*+DATA INIZIO ASSICURAZIONE
        //                  15 INIASSA                     PIC 9999.
        //                  15 INIASSM                     PIC 99.
        //                  15 INIASSG                     PIC 99.
        //*+DATA FINE ASSICURAZIONE
        //                  15 FINASSA                     PIC 9999.
        //                  15 FINASSM                     PIC 99.
        //                  15 FINASSG                     PIC 99.
        //         05  NRICONOSC                     PIC 9.
        //* NUMERO RICONASCIMENTI ASSEGNO DI INVALIDITA'
        //         05  RSCADASS.
        //             10  SCADASSA                  PIC 9(4).
        //             10  SCADASSM                  PIC 99.
        //* DECORRENZA SCADENZA ASSEGNO DI INVALIDITA'
        //         05  IDECNAT3X.
        //             10  IDECNAT3A            PIC 9(4).
        //             10  IDECNAT3M            PIC 9(2).
        //* DECORRENZA CODICE VIRTUALE = 2 (SOLO PER RICOSTITUZIONI)
        #endregion tracciato COBOL

        #region Tracciato Host
        // 04  ALTRI-CAMPI.
        /// <summary>
        /// DECSINDA 9999  
        /// * DECORRENZA SINDACALI
        /// </summary>
        [HisFieldInfoMapping(0, 4)]
        public short DECSINDA { get; set; }

        /// <summary>
        /// DECSINDM 99  
        /// * DECORRENZA SINDACALI
        /// </summary>
        [HisFieldInfoMapping(1, 2)]
        public short DECSINDM { get; set; }

        //*
        /// <summary>
        /// TRESTEROUP XXX  
        /// *UFFICIO PAGATORE PER TRATTENUTE PER ESTERO
        /// </summary>
        [HisFieldInfoMapping(2, 3)]
        public string TRESTEROUP { get; set; }

        /// <summary>
        /// TRESTERO S9 COMP-3 
        /// *TRATTENUTE PER ESTERO 1=SI
        /// </summary>
        [HisFieldInfoMapping(3, 1, CobolType = CobolType.Comp3)]
        public int TRESTERO { get; set; }

        // DA ELIMINARE
        //[HisComplexAreaInfoMapping(4, ListCount = 4)]
        //public List<LiquidazioneStatiEsteri> LIQUIDAZIONESTATIESTERI { get; set; }

        /// <summary>
        /// FILLER X(24)
        /// </summary>
        [HisFieldInfoMapping(4, 24)]
        public string FILLER { get; set; }

        // 05  INIFINASS.
        /// <summary>
        /// INIASSA 9999  
        /// *+DATA INIZIO ASSICURAZIONE
        /// </summary>
        [HisFieldInfoMapping(5, 4)]
        public short INIASSA { get; set; }

        /// <summary>
        /// INIASSM 99  
        /// *+DATA INIZIO ASSICURAZIONE
        /// </summary>
        [HisFieldInfoMapping(6, 2)]
        public short INIASSM { get; set; }

        /// <summary>
        /// INIASSG 99  
        /// </summary>
        [HisFieldInfoMapping(7, 2)]
        public short INIASSG { get; set; }

        /// <summary>
        /// FINASSA 9999  
        /// *+DATA FINE ASSICURAZIONE
        /// </summary>
        [HisFieldInfoMapping(8, 4)]
        public short FINASSA { get; set; }

        /// <summary>
        /// FINASSM 99  
        /// *+DATA FINE ASSICURAZIONE
        /// </summary>
        [HisFieldInfoMapping(9, 2)]
        public short FINASSM { get; set; }

        /// <summary>
        /// FINASSG 99  
        /// </summary>
        [HisFieldInfoMapping(10, 2)]
        public short FINASSG { get; set; }

        /// <summary>
        /// NRICONOSC 9  
        // * NUMERO RICONASCIMENTI ASSEGNO DI INVALIDITA'
        /// </summary>
        [HisFieldInfoMapping(11, 1)]
        public short NRICONOSC { get; set; }

        // 05  RSCADASS.
        /// <summary>
        /// SCADASSA 9(4)  
        /// * DECORRENZA SCADENZA ASSEGNO DI INVALIDITA'
        /// </summary>
        [HisFieldInfoMapping(12, 4)]
        public short SCADASSA { get; set; }

        /// <summary>
        /// SCADASSM 99  
        /// * DECORRENZA SCADENZA ASSEGNO DI INVALIDITA'
        /// </summary>
        [HisFieldInfoMapping(13, 2)]
        public short SCADASSM { get; set; }

        // 05  IDECNAT3X.
        /// <summary>
        /// IDECNAT3A 9(4)  
        /// </summary>
        [HisFieldInfoMapping(14, 4)]
        public short IDECNAT3A { get; set; }

        /// <summary>
        /// IDECNAT3M 9(2)  
        /// </summary>
        [HisFieldInfoMapping(15, 2)]
        public short IDECNAT3M { get; set; }

        #endregion Tracciato Host

        // DA ELIMINARE
        //#region nested class
        //public class LiquidazioneStatiEsteri
        //{
        //    #region Constructor
        //    public LiquidazioneStatiEsteri()
        //    { }
        //    #endregion Constructor

        //    #region tracciato COBOL
        //    //*****************************************************************
        //    //*  DECORRENZA PRECEDENTE LIQUIDAZIONE DEGLI STATI ESTERI
        //    //*****************************************************************
        //    //         05  V-IPLIQ.
        //    //              10  V-IPRECLIQ OCCURS 4.
        //    //*+DATA PRECEDENTE LIQUIDAZIONE
        //    //                     20 V-IDAPLIQA            PIC 9999.
        //    //                     20 V-IDAPLIQM            PIC 99.
        //    //*2004 CAMPI V- SPOSTATI NELL'AREA STATI:   RESTANO 24 FILLER
        //    #endregion tracciato COBOL

        //    #region Tracciato Host
        //    //*****************************************************************
        //    // *  DECORRENZA PRECEDENTE LIQUIDAZIONE DEGLI STATI ESTERI
        //    //*****************************************************************
        //    // 05  V-IPLIQ.
        //    // 10  V-IPRECLIQ OCCURS 4.
        //    /// <summary>
        //    /// V_IDAPLIQA 9999  
        //    /// *+DATA PRECEDENTE LIQUIDAZIONE
        //    /// </summary>
        //    [HisFieldInfoMapping(0, 4)]
        //    public short V_IDAPLIQA { get; set; }

        //    /// <summary>
        //    /// V_IDAPLIQM 99  
        //    /// *+DATA PRECEDENTE LIQUIDAZIONE
        //    /// </summary>
        //    [HisFieldInfoMapping(1, 2)]
        //    public short V_IDAPLIQM { get; set; }

        //    #endregion Tracciato Host

        //}
        //#endregion nested class
    }
}
