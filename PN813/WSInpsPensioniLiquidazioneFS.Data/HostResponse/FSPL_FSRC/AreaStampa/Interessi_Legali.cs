using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostResponse.AreaStampa
{
    public class Interessi_Legali
    {
        #region Constructor
        internal Interessi_Legali()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 INT-CRED         PIC S9(09)V9(04) COMP-3.
        //*                                                           4158
        //     02 INT-DEB          PIC S9(09)V9(04) COMP-3.
        //*                                                           4165
        //     02 INT-SALDO        PIC S9(09)V9(04) COMP-3.
        //*                                                           4172
        //     02 RIVAL-CRED       PIC S9(09)V9(04) COMP-3.
        //*                                                           4179
        //     02 RIVAL-DEB        PIC S9(09)V9(04) COMP-3.
        //*                                                           4186
        //     02 RIVAL-SALDO      PIC S9(09)V9(04) COMP-3.
        //*                                                           4193
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// INT_CRED S9(09)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(0, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal INT_CRED { get; set; }

        // *                                                           4158
        /// <summary>
        /// INT_DEB S9(09)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(1, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal INT_DEB { get; set; }

        // *                                                           4165
        /// <summary>
        /// INT_SALDO S9(09)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(2, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal INT_SALDO { get; set; }

        // *                                                           4172
        /// <summary>
        /// RIVAL_CRED S9(09)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(3, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal RIVAL_CRED { get; set; }

        // *                                                           4179
        /// <summary>
        /// RIVAL_DEB S9(09)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(4, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal RIVAL_DEB { get; set; }

        // *                                                           4186
        /// <summary>
        /// RIVAL_SALDO S9(09)V9(04) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(5, 7, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal RIVAL_SALDO { get; set; }

        // *                                                           4193
        #endregion Tracciato Host

        #region nested class

        #endregion nested class

        #endregion Properties
    }
}

