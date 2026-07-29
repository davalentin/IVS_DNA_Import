using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostResponse.AreaStampa
{
    public class Pensioni_Fiscali
    {
        #region Constructor
        internal Pensioni_Fiscali()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 ABB-CATE         PIC 9(03)             OCCURS 5 TIMES.
        //*                          CATEGORIA                        2130
        //     02 ABB-SEDE         PIC 9(04)             OCCURS 5 TIMES.
        //*                          SEDE                             2145
        //     02 ABB-CERT         PIC 9(08)             OCCURS 5 TIMES.
        //*                          CERTIFICATO                      2165
        //     02 FILLER           PIC X(15).
        //*                          LIBERI                           2205
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 5)]
        public List<Categoria> LISTCategoria { get; set; }

        [HisComplexAreaInfoMapping(1, ListCount = 5)]
        public List<Sede> LISTSede { get; set; }

        [HisComplexAreaInfoMapping(2, ListCount = 5)]
        public List<Certificato> LISTCertificato { get; set; }

        /// <summary>
        /// FILLER X(15)  
        /// </summary>
        [HisFieldInfoMapping(3, 15)]
        public string FILLER { get; set; }

        // *                          LIBERI                           2205
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
            //     02 ABB-CATE         PIC 9(03)             OCCURS 5 TIMES.
            //*                          CATEGORIA                        2130
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// ABB_CATE 9(03)  
            /// </summary>
            [HisFieldInfoMapping(0, 3, CobolType = CobolType.Unsigned)]
            public short ABB_CATE { get; set; }

            // *                          CATEGORIA                        2130
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
            //     02 ABB-SEDE         PIC 9(04)             OCCURS 5 TIMES.
            //*                          SEDE                             2145
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// ABB_SEDE 9(04)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short ABB_SEDE { get; set; }

            // *                          SEDE                             2145
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
            //     02 ABB-CERT         PIC 9(08)             OCCURS 5 TIMES.
            //*                          CERTIFICATO                      2165
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// ABB_CERT 9(08)  
            /// </summary>
            [HisFieldInfoMapping(0, 8, CobolType = CobolType.Unsigned)]
            public int ABB_CERT { get; set; }

            // *                          CERTIFICATO                      2165
            #endregion Tracciato Host

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}

