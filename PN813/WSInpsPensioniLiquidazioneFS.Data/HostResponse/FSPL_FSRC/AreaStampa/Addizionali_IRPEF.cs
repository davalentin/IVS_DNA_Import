using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostResponse.AreaStampa
{
    public class Addizionali_IRPEF
    {
        #region Constructor
        internal Addizionali_IRPEF()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 IRPEF-COM        PIC S9(05)V9(04) COMP-3.
        //*       GP3CE2A              IMP.MENS.ADDIZIONALE COMUNALE  2050
        //     02 IRPEF-PROV       PIC S9(05)V9(04) COMP-3.
        //*       (USO FUTURO)         IMP.MENS.ADDIZIONALE PROVINCIA 2055
        //     02 IRPEF-REG        PIC S9(05)V9(04) COMP-3.
        //*       GP3CE24              IMP.MENS.ADDIZIONALE REGIONALE 2060
        //     02 FILLER           PIC X(05).
        //*                            LIBERI                         2065
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// IRPEF_COM S9(05)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IRPEF_COM { get; set; }

        // *       GP3CE2A              IMP.MENS.ADDIZIONALE COMUNALE  2050
        /// <summary>
        /// IRPEF_PROV S9(05)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(1, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IRPEF_PROV { get; set; }

        // *       (USO FUTURO)         IMP.MENS.ADDIZIONALE PROVINCIA 2055
        /// <summary>
        /// IRPEF_REG S9(05)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(2, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IRPEF_REG { get; set; }

        // *       GP3CE24              IMP.MENS.ADDIZIONALE REGIONALE 2060
        /// <summary>
        /// FILLER X(05)  
        /// </summary>
        [HisFieldInfoMapping(3, 5)]
        public string FILLER { get; set; }

        // *                            LIBERI                         2065
        #endregion Tracciato Host

        #region nested class

        #endregion nested class

        #endregion Properties
    }
}

