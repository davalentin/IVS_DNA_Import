using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostResponse.AreaStampa
{
    public class ACNE_CENGIO
    {
        #region Constructor
        internal ACNE_CENGIO()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 FLAG-ACNA        PIC 9(01).
        //*                          FLAG = 1 TABELLA VALORIZZATA     5817
        //     02 ANNO-ACNA        PIC 9(04).
        //*                          ANNO                             5818
        //     02 SETT-ACNA        PIC 9(04).
        //*                          NUMERO SETTIMANE     - QUOTA A   5822
        //     02 SETT-ACNB        PIC 9(04).
        //*                          NUMERO SETTIMANE     - QUOTA B   5826
        //     02 RMS-ACNA         PIC S9(07)V9(04) COMP-3.
        //*                          RETRIBUZ.MEDIA SETT. - QUOTA A   5830
        //     02 RMS-ACNB         PIC S9(07)V9(04) COMP-3.
        //*                          RETRIBUZ.MEDIA SETT. - QUOTA B   5836
        //     02 PENS-ACNA        PIC S9(07)V9(04) COMP-3.
        //*                          IMP.MENSILE PENSIONE - QUOTA A   5842
        //     02 PENS-ACNB        PIC S9(07)V9(04) COMP-3.
        //*                          IMP.MENSILE PENSIONE - QUOTA B   5848
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// FLAG_ACNA 9(01)  
        /// </summary>
        [HisFieldInfoMapping(0, 1, CobolType = CobolType.Unsigned)]
        public short FLAG_ACNA { get; set; }

        // *                          FLAG = 1 TABELLA VALORIZZATA     5817
        /// <summary>
        /// ANNO_ACNA 9(04)  
        /// </summary>
        [HisFieldInfoMapping(1, 4, CobolType = CobolType.Unsigned)]
        public short ANNO_ACNA { get; set; }

        // *                          ANNO                             5818
        /// <summary>
        /// SETT_ACNA 9(04)  
        /// </summary>
        [HisFieldInfoMapping(2, 4, CobolType = CobolType.Unsigned)]
        public short SETT_ACNA { get; set; }

        // *                          NUMERO SETTIMANE     - QUOTA A   5822
        /// <summary>
        /// SETT_ACNB 9(04)  
        /// </summary>
        [HisFieldInfoMapping(3, 4, CobolType = CobolType.Unsigned)]
        public short SETT_ACNB { get; set; }

        // *                          NUMERO SETTIMANE     - QUOTA B   5826
        /// <summary>
        /// RMS_ACNA S9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(4, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal RMS_ACNA { get; set; }

        // *                          RETRIBUZ.MEDIA SETT. - QUOTA A   5830
        /// <summary>
        /// RMS_ACNB S9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(5, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal RMS_ACNB { get; set; }

        // *                          RETRIBUZ.MEDIA SETT. - QUOTA B   5836
        /// <summary>
        /// PENS_ACNA S9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(6, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal PENS_ACNA { get; set; }

        // *                          IMP.MENSILE PENSIONE - QUOTA A   5842
        /// <summary>
        /// PENS_ACNB S9(07)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(7, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal PENS_ACNB { get; set; }

        // *                          IMP.MENSILE PENSIONE - QUOTA B   5848
        #endregion Tracciato Host

        #region nested class

        #endregion nested class

        #endregion Properties
    }
}

