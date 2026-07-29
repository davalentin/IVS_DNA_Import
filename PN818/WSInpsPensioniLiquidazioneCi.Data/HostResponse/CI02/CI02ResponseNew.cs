using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class CI02ResponseNew
    {
        #region Constructor
        public CI02ResponseNew()
        {
            this.RecordZeroCentro = new AreaRecordZeroCentro();
            this.Record_RA = new CI02Record_RA();
            this.Record_RB = new CI02Record_RB();
            this.Record_RDA = new CI02Record_RDA();
            this.Record_RE = new CI02Record_RE();
        }
        #endregion Constructor

        #region Properties
        [HisFieldInfoMapping(0,8)]
        public string FILLER1 { get; set; }

        [HisComplexAreaInfoMapping(1)]
        public AreaRecordZeroCentro RecordZeroCentro { get; set; }

        [HisComplexAreaInfoMapping(2)]
        public CI02Record_RA Record_RA { get; set; }

        [HisComplexAreaInfoMapping(3)]
        public CI02Record_RB Record_RB { get; set; }

        [HisComplexAreaInfoMapping(4)]
        public CI02Record_RDA Record_RDA { get; set; }

        [HisComplexAreaInfoMapping(5)]
        public CI02Record_RE Record_RE { get; set; }

        [HisFieldInfoMapping(6, 3561, CobolType = CobolType.Untraslate)]
        public byte[] RISP_UNTRASLATED { get; set; }

        public AreaPresenzaREDD PresenzaREDD { get; set; }

        public CI02NonPresenzaREDD NonPresenzaREDD { get; set; }
        #endregion Properties

        #region Nested class

        #endregion Nested class
    }
}
