using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostResponse.AreaStampa
{
    public class Pensioni_Cumulate
    {
        #region Constructor
        internal Pensioni_Cumulate()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 CUM-CATE         PIC 9(03)             OCCURS 10 TIMES.
        //*                          CATEGORIA                        2220
        //     02 CUM-SEDE         PIC 9(04)             OCCURS 10 TIMES.
        //*                          SEDE                             2250
        //     02 CUM-CERT         PIC 9(08)             OCCURS 10 TIMES.
        //*                          CERTIFICATO                      2290
        //     02 FILLER           PIC X(30).
        //*                          LIBERI                           2370
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 10)]
        public List<Categoria> LISTCategoria { get; internal set; }

        [HisComplexAreaInfoMapping(1, ListCount = 10)]
        public List<Sede> LISTSede { get; internal set; }

        [HisComplexAreaInfoMapping(2, ListCount = 10)]
        public List<Certificato> LISTCertificato { get; internal set; }

        /// <summary>
        /// FILLER X(30)  
        /// </summary>
        [HisFieldInfoMapping(3, 30)]
        public string FILLER { get; set; }

        // *                          LIBERI                           2370
        #endregion Tracciato Host

        #region nested class
        public class Categoria
        {
            #region Constructor
            internal Categoria()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 CUM-CATE         PIC 9(03)             OCCURS 10 TIMES.
            //*                          CATEGORIA                        2220
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// CUM-CATE 9(03)  
            /// </summary>
            [HisFieldInfoMapping(0, 3, CobolType = CobolType.Unsigned)]
            public short CUM_CATE { get; set; }

            // *                          CATEGORIA                        2220
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Sede
        {
            #region Constructor
            internal Sede()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 CUM-SEDE         PIC 9(04)             OCCURS 10 TIMES.
            //*                          SEDE                             2250
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// CUM-SEDE 9(04)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short CUM_SEDE { get; set; }

            // *                          SEDE                             2250
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Certificato
        {
            #region Constructor
            internal Certificato()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 CUM-CERT         PIC 9(08)             OCCURS 10 TIMES.
            //*                          CERTIFICATO                      2290
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// CUM-CERT 9(08)  
            /// </summary>
            [HisFieldInfoMapping(0, 8, CobolType = CobolType.Unsigned)]
            public int CUM_CERT { get; set; }

            // *                          CERTIFICATO                      2290
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}

