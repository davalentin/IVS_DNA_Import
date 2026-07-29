using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class AreaPresenzaREDD: ITransactionInfo
    {
        #region Constructor
        public AreaPresenzaREDD()
        {
            this.AreaRED_RED = new PCIRED4.AreaRED_RED();
            this.AreaRED_KE = new PCIRED4.AreaRED_KE();
            this.AreaRED_KF = new PCIRED4.AreaRED_KF();
            this.AreaRED_KM = new PCIRED4.AreaRED_KM();
            this.AreaRED_BAUN = new PCIRED4.AreaRED_BAUN();
            this.AreaRED_BLE = new PCIRED4.AreaRED_BLE();
        }
        #endregion Constructor

        #region Properties
        [HisComplexAreaInfoMapping(0)]
        public PCIRED4.AreaRED_RED AreaRED_RED { get; set; }

        [HisComplexAreaInfoMapping(1)]
        public PCIRED4.AreaRED_KE AreaRED_KE { get; set; }

        [HisComplexAreaInfoMapping(2)]
        public PCIRED4.AreaRED_KF AreaRED_KF { get; set; }

        [HisComplexAreaInfoMapping(3)]
        public PCIRED4.AreaRED_KM AreaRED_KM { get; set; }

        [HisComplexAreaInfoMapping(4)]
        public PCIRED4.AreaRED_BAUN AreaRED_BAUN { get; set; }

        [HisComplexAreaInfoMapping(5)]
        public PCIRED4.AreaRED_BLE AreaRED_BLE { get; set; }

        public string TransactionName
        {
            get { return "Area Redditi tradotta"; }
        }
        #endregion Properties

        #region Nested class

        #endregion Nested class
    }
}
