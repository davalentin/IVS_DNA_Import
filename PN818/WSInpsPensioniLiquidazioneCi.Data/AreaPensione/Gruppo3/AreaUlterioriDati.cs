using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaUlterioriDati
    {
        #region tracciato COBOL
        //             04  IULTERDATI.
        //       05  IIMPPAG91                 PIC S9(5)V9(4) COMP-3.
        //*EURO  IMPORTO IN PAGAMENTO IN ITALIA AL 02/1991 X CRIST91
        //       05  PRECSEDE                  PIC XXXX.
        //       05  PRECCAT                   PIC XXX.
        //       05  PRECCER                   PIC X(8).
        //* PRECEDENTE SEDE CATEGORIA E CERTIFICATO (PER CAUSACARICO 3/5/9)


        //       05  INEWALTRAPEN.
        //        10  IALTRAPEN  OCCURS 5.
        //* DATI ALTRA PENSIONE                         TOT. 28 X 5 = 140
        //         15  IAPNUMP      PIC 9(9) COMP-3.
        //*+NUMERO  ALTRA PENSIONE
        //         15  IAPCATEG     PIC X(3).
        //*+CATEGORIA  ALTRA PENS.
        //         15  IAPENTE      PIC X.
        //*+ENTE  ALTRA PENSIONE
        //         15  IAPUNIC      PIC X.
        //*+ U = UNICO; C = +CONTITOLARI
        //         15  IAPCODIMP    PIC 9.
        //*+CODICE IMPORTO ALTRA P.
        //         15  IAPIMPO      PIC 9(7)V9(4) COMP-3.
        //*EURO +IMPORTO ALTRA PENSIONE
        //*+DEC. ALTRA PENSIONE
        //             20  IAPDECORA    PIC 9(4).
        //             20  IAPDECORM    PIC 99.
        //*+CESS. ALTRA PENSIONE
        //             20  IAPCESSAA    PIC 9(4).
        //             20  IAPCESSAM    PIC 99.



        //       05  IREQ311294       PIC X.
        //* REQUISITO 31.12.94 PER TRATTENUTE LAVORO
        //       05  IW1AS72B         PIC 9(5)V9(4)  COMP-3.
        //*EURO  AUMENTO PER SENTENZA N. 72/90 POST ART.2 DPCM
        //* DECORRENZA ELIMINAZIONE PER MORTE O ALTRO.
        //             10  DECELIMA     PIC 9999.
        //             10  DECELIMM     PIC 99.
        //             10  DECELIMG     PIC 99.
        //       05  IRELPAR         PIC X(8).
        //*RELAZIONE DI PARENTELA CON L'ASSICURATO
        //       05  TP1PROVDC       PIC X(2).
        //*CODICE PROVINCIA DI NASCITA DELL'ASSICURATO (ASSIEME A TP1COMDC)
        //*METTERLO PIC 9 QUANDO VERRA' SPOSTATO AL SUO POSTO (IN IW3..)
        #endregion tracciato COBOL

        #region Tracciato Host
        // 04  IULTERDATI.
        /// <summary>
        /// IIMPPAG91 S9(5)V9(4) COMP-3 
        /// *EURO  IMPORTO IN PAGAMENTO IN ITALIA AL 02/1991 X CRIST91
        /// </summary>
        [HisFieldInfoMapping(0, 5, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IIMPPAG91 { get; set; }

        /// <summary>
        /// PRECSEDE XXXX   
        /// * PRECEDENTE SEDE (PER CAUSACARICO 3/5/9)
        /// </summary>
        [HisFieldInfoMapping(1, 4)]
        public string PRECSEDE { get; set; }

        /// <summary>
        /// PRECCAT XXX 
        /// * PRECEDENTE CATEGORIA (PER CAUSACARICO 3/5/9)
        /// </summary>
        [HisFieldInfoMapping(2, 3)]
        public string PRECCAT { get; set; }

        /// <summary>
        /// PRECCER X(8)  
        /// * PRECEDENTE CERTIFICATO (PER CAUSACARICO 3/5/9)
        /// </summary>
        [HisFieldInfoMapping(3, 8)]
        public string PRECCER { get; set; }

        [HisComplexAreaInfoMapping(4, ListCount = 5)]
        public List<AltraPensione> ALTRAPENSIONE { get; set; }

        // 05  IREQ311294       PIC X.
        /// * REQUISITO 31.12.94 PER TRATTENUTE LAVORO
        [HisFieldInfoMapping(5, 1)]
        public string IREQ311294 { get; set; }
        /// <summary>
        /// IW1AS72B 9(5)V9(4) COMP-3 
        /// *EURO  AUMENTO PER SENTENZA N. 72/90 POST ART.2 DPCM
        /// </summary>
        [HisFieldInfoMapping(6, 5, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
        public decimal IW1AS72B { get; set; }

        /// <summary>
        /// DECELIMA 9999  
        /// * DECORRENZA ELIMINAZIONE PER MORTE O ALTRO.
        /// </summary>
        [HisFieldInfoMapping(7, 4)]
        public short DECELIMA { get; set; }

        /// <summary>
        /// DECELIMM 99  
        /// * DECORRENZA ELIMINAZIONE PER MORTE O ALTRO.
        /// </summary>
        [HisFieldInfoMapping(8, 2)]
        public short DECELIMM { get; set; }

        /// <summary>
        /// DECELIMG 99  
        /// * DECORRENZA ELIMINAZIONE PER MORTE O ALTRO.
        /// </summary>
        [HisFieldInfoMapping(9, 2)]
        public short DECELIMG { get; set; }

        /// <summary>
        /// IRELPAR X(8)  
        /// *RELAZIONE DI PARENTELA CON L'ASSICURATO
        /// </summary>
        [HisFieldInfoMapping(10, 8)]
        public string IRELPAR { get; set; }

        /// <summary>
        /// TP1PROVDC X(2)  
        /// *CODICE PROVINCIA DI NASCITA DELL'ASSICURATO (ASSIEME A TP1COMDC)
        /// *METTERLO PIC 9 QUANDO VERRA' SPOSTATO AL SUO POSTO (IN IW3..)
        /// </summary>
        [HisFieldInfoMapping(11, 2)]
        public string TP1PROVDC { get; set; }

        #endregion Tracciato Host

        #region nested class
        public class AltraPensione
        {
            #region tracciato COBOL
            //       05  INEWALTRAPEN.
            //        10  IALTRAPEN  OCCURS 5.
            //* DATI ALTRA PENSIONE                         TOT. 28 X 5 = 140
            //         15  IAPNUMP      PIC 9(9) COMP-3.
            //*+NUMERO  ALTRA PENSIONE
            //         15  IAPCATEG     PIC X(3).
            //*+CATEGORIA  ALTRA PENS.
            //         15  IAPENTE      PIC X.
            //*+ENTE  ALTRA PENSIONE
            //         15  IAPUNIC      PIC X.
            //*+ U = UNICO; C = +CONTITOLARI
            //         15  IAPCODIMP    PIC 9.
            //*+CODICE IMPORTO ALTRA P.
            //         15  IAPIMPO      PIC 9(7)V9(4) COMP-3.
            //*EURO +IMPORTO ALTRA PENSIONE
            //*+DEC. ALTRA PENSIONE
            //             20  IAPDECORA    PIC 9(4).
            //             20  IAPDECORM    PIC 99.
            //*+CESS. ALTRA PENSIONE
            //             20  IAPCESSAA    PIC 9(4).
            //             20  IAPCESSAM    PIC 99.
            #endregion tracciato COBOL

            #region Tracciato Host
            // 05  INEWALTRAPEN.
            // 10  IALTRAPEN  OCCURS 5.
            // * DATI ALTRA PENSIONE                         TOT. 28 X 5 = 140
            /// <summary>
            /// IAPNUMP 9(9) COMP-3 
            /// *+NUMERO  ALTRA PENSIONE
            /// </summary>
            [HisFieldInfoMapping(0, 5, CobolType = CobolType.Comp3Unsigned)]
            public int IAPNUMP { get; set; }

            /// <summary>
            /// IAPCATEG X(3)  
            /// *+CATEGORIA  ALTRA PENS.
            /// </summary>
            [HisFieldInfoMapping(1, 3)]
            public string IAPCATEG { get; set; }

            /// <summary>
            /// IAPENTE X  
            /// *+ENTE  ALTRA PENSIONE
            /// </summary>
            [HisFieldInfoMapping(2, 1)]
            public string IAPENTE { get; set; }

            /// <summary>
            /// IAPUNIC X  
            /// *+ U = UNICO; C = +CONTITOLARI
            /// </summary>
            [HisFieldInfoMapping(3, 1)]
            public string IAPUNIC { get; set; }

            /// <summary>
            /// IAPCODIMP 9  
            /// *+CODICE IMPORTO ALTRA P.
            /// </summary>
            [HisFieldInfoMapping(4, 1)]
            public short IAPCODIMP { get; set; }

            /// <summary>
            /// IAPIMPO 9(7)V9(4) COMP-3 
            /// *EURO +IMPORTO ALTRA PENSIONE
            /// </summary>
            [HisFieldInfoMapping(5, 6, Scale = 4, CobolType = CobolType.Comp3Unsigned)]
            public decimal IAPIMPO { get; set; }


            /// <summary>
            /// IAPDECORA 9(4)              
            /// *+DEC. ALTRA PENSIONE
            /// </summary>
            [HisFieldInfoMapping(6, 4)]
            public short IAPDECORA { get; set; }

            /// <summary>
            /// IAPDECORM 99               
            /// *+DEC. ALTRA PENSIONE 
            /// </summary>
            [HisFieldInfoMapping(7, 2)]
            public short IAPDECORM { get; set; }

            /// <summary>
            /// IAPCESSAA 9(4)  
            /// *+CESS. ALTRA PENSIONE
            /// </summary>
            [HisFieldInfoMapping(8, 4)]
            public short IAPCESSAA { get; set; }

            /// <summary>
            /// IAPCESSAM 99  
            /// *+CESS. ALTRA PENSIONE
            /// </summary>
            [HisFieldInfoMapping(9, 2)]
            public short IAPCESSAM { get; set; }

            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
