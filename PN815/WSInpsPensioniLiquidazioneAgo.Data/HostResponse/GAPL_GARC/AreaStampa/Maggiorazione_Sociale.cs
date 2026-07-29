using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostResponse.AreaStampa
{
    public class Maggiorazione_Sociale
    {
        #region Constructor
        internal Maggiorazione_Sociale()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //   02 MAGG-SOC         PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
        //*                            MAGG.SOCIALE                  12640
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 32)]
        public List<Maggiorazione> LISTMaggiorazione { get; internal set; }
        #endregion Tracciato Host

        #region nested class
        public class Maggiorazione
        {
            #region Constructor
            internal Maggiorazione()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //   02 MAGG-SOC         PIC 9(05)V9(04) COMP-3 OCCURS 32 TIMES.
            //*                            MAGG.SOCIALE                  12640
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// MAGG_SOC 9(05)V9(04)  COMP-3
            /// </summary>
            [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal MAGG_SOC { get; set; }

            // *                            MAGG.SOCIALE                  12640
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}

