using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaW4
    {
        #region tracciato COBOL
        //  04  AREAW4.
        //     05  IEWK4A OCCURS 15 TIMES.
        //       07 DATI-ANAGR.
        //         10  TP1NOM3.
        //             15  TP1COGNF                   PIC X(32).
        //***** COGNOME
        //             15  TP1NOMEF                   PIC X(32).
        //***** NOME
        //         10  TP1COACF                       PIC X(32).
        //* COGNOME ACQUISITO
        //         10  TP1PRF                         PIC XXX.
        //* COD. PROVINCIA
        //         10  TP1COF                         PIC 9(5).
        //* COD. COMUNE
        //       07 DATI-FAM.
        //         10        IDATANAS.
        //             15        IW4NASCA                PIC 9999.
        //             15        IW4NASCM                PIC 99.
        //             15        IW4NASCG                PIC 99.
        //*+1999 CESSAZIONE AAFF
        //         10        IW4SES                      PIC X.
        //*+SESSO AAFF
        //         10        IW4COCON                    PIC X.
        //*+COD.CONTITOLARE AAFF
        //         10        IW4COMP                     PIC X.
        //*+COD.COMPONENTE AAFF
        //         10        IW4CENAOL                   PIC 9.
        //*+CODICE ENAOLI AAFF
        //         10        IW4PRGEM                    PIC 9.
        //*+PROGRESSIVO GEMELLI AAFF
        //         10        IW4COFA.
        //             15    IW4FRAT                     PIC 9.
        //*+CF AF FRATELLI
        //             15    IW4GEN                      PIC 9.
        //*+CF AF GENITORI
        //             15    IW4CON                      PIC 9.
        //*+CF AF CONIUGE
        //             15    IW4FIG                      PIC 99.
        //*+CF AF FIGLI
        //*        10        FILLER                      PIC X.
        //*VUOTO
        //cg2015         10 AREAW4GP3CK.
        //cg2015          15 GP3CK       OCCURS 10.
        //cg2015           20        IDECOR-ACQ.
        //cg2015             25        IW4ACQA                 PIC 9999.
        //cg2015             25        IW4ACQM                 PIC 99.
        //cg2015           20        IW4ACQ REDEFINES IDECOR-ACQ      PIC 9(6).
        //cg2015*+1999 DECORRENZA AAFF
        //cg2015           20        IDECORCES.
        //cg2015             25        IW4CESA                 PIC 9999.
        //cg2015             25        IW4CESM                 PIC 99.
        //cg2015           20        IW4CES REDEFINES IDECORCES       PIC 9(6).
        //cg2015*+1999 CESSAZIONE AAFF
        //cg2015           20        IW4SIG                      PIC X.
        //cg2015*+SIGLA AAFF
        //                 20        GP3CH01B                    PIC X.
        //cg2015           20        IW4CMAG                     PIC 9.
        //cg2015*+COD.MAGGIORAZIONE AAFF


        //************* CODICI FISCALI DEI FAMILIARI *******************
        //     05 AREAW4COFI.
        //         10 IW4COFI     OCCURS 15      PIC X(16).
        //*1998 CODICE FISCALE DEI FAMILIARI    16 X 15 = 240
        //*1999 ANNO DI 4 BYTE
        //**************************************************************
        //* 1996 IN TS7WK5AA.CPY     40 X 30 = 1200 X WK5!!!
        //* 2002 IN INPUTWKL.CPY     40 X 30 = 1200
        //* VALIDA DAL 2002

        #endregion tracciato COBOL

        #region Tracciato Host

        [HisComplexAreaInfoMapping(0, ListCount = 15)]
        public List<DatiFamiliari> DATIFAMILIARI { get; set; }

        [HisComplexAreaInfoMapping(1, ListCount = 15)]
        public List<CodiciFiscaliFamiliari> CODICIFISCALIFAMILIARI { get; set; }

        #endregion Tracciato Host

        #region nested class

        public class DatiFamiliari
        {
            #region tracciato COBOL
            //     05  IEWK4A OCCURS 15 TIMES.
            //       07 DATI-ANAGR.
            //         10  TP1NOM3.
            //             15  TP1COGNF                   PIC X(32).
            //***** COGNOME
            //             15  TP1NOMEF                   PIC X(32).
            //***** NOME
            //         10  TP1COACF                       PIC X(32).
            //* COGNOME ACQUISITO
            //         10  TP1PRF                         PIC XXX.
            //* COD. PROVINCIA
            //         10  TP1COF                         PIC 9(5).
            //* COD. COMUNE
            //       07 DATI-FAM.
            //         10        IDATANAS.
            //             15        IW4NASCA                PIC 9999.
            //             15        IW4NASCM                PIC 99.
            //             15        IW4NASCG                PIC 99.
            //*+1999 DATA NASCITA AAFF
            //         10        IDECOR-ACQ.
            //             15        IW4ACQA                 PIC 9999.
            //             15        IW4ACQM                 PIC 99.
            //*+1999 DECORRENZA AAFF
            //         10        IDECORCES.
            //             15        IW4CESA                 PIC 9999.
            //             15        IW4CESM                 PIC 99.
            //*+1999 CESSAZIONE AAFF
            //         10        IW4SES                      PIC X.
            //*+SESSO AAFF
            //         10        IW4SIG                      PIC X.
            //*+SIGLA AAFF
            //         20        GP3CH01B                    PIC X.
            //         10        IW4CMAG                     PIC 9.
            //*+COD.MAGGIORAZIONE AAFF
            //         10        IW4COCON                    PIC X.
            //*+COD.CONTITOLARE AAFF
            //         10        IW4COMP                     PIC X.
            //*+COD.COMPONENTE AAFF
            //         10        IW4CENAOL                   PIC 9.
            //*+CODICE ENAOLI AAFF
            //         10        IW4PRGEM                    PIC 9.
            //*+PROGRESSIVO GEMELLI AAFF
            //         10        IW4COFA.
            //             15    IW4FRAT                     PIC 9.
            //*+CF AF FRATELLI
            //             15    IW4GEN                      PIC 9.
            //*+CF AF GENITORI
            //             15    IW4CON                      PIC 9.
            //*+CF AF CONIUGE
            //             15    IW4FIG                      PIC 99.
            //*+CF AF FIGLI
            //*        10        FILLER                      PIC X.
            //*VUOTO
            #endregion tracciato COBOL

            #region Tracciato Host
            // 04  AREAW4.
            // 05  IEWK4A OCCURS 15 TIMES.
            // 07 DATI-ANAGR.
            // 10  TP1NOM3.
            /// <summary>
            /// TP1COGNF X(32)  
            ///***** COGNOME
            /// </summary>
            [HisFieldInfoMapping(0, 32)]
            public string TP1COGNF { get; set; }

            /// <summary>
            /// TP1NOMEF X(32)  
            ///***** NOME
            /// </summary>
            [HisFieldInfoMapping(1, 32)]
            public string TP1NOMEF { get; set; }

            /// <summary>
            /// TP1COACF X(32)  
            /// * COGNOME ACQUISITO
            /// </summary>
            [HisFieldInfoMapping(2, 32)]
            public string TP1COACF { get; set; }

            /// <summary>
            /// TP1PRF XXX  
            /// * COD. PROVINCIA
            /// </summary>
            [HisFieldInfoMapping(3, 3)]
            public string TP1PRF { get; set; }

            /// <summary>
            /// TP1COF 9(5)  
            /// * COD. COMUNE
            /// </summary>
            [HisFieldInfoMapping(4, 5)]
            public int TP1COF { get; set; }

            // 07 DATI-FAM.
            // 10        IDATANAS.
            /// <summary>
            /// IW4NASCA 9999  
            /// *+1999 DATA NASCITA AAFF
            /// </summary>
            [HisFieldInfoMapping(5, 4)]
            public short IW4NASCA { get; set; }

            /// <summary>
            /// IW4NASCM 99  
            /// *+1999 DATA NASCITA AAFF
            /// </summary>
            [HisFieldInfoMapping(6, 2)]
            public short IW4NASCM { get; set; }

            /// <summary>
            /// IW4NASCG 99  
            /// *+1999 DATA NASCITA AAFF
            /// </summary>
            [HisFieldInfoMapping(7, 2)]
            public short IW4NASCG { get; set; }

            /// <summary>
            /// IW4SES X  
            /// *+SESSO AAFF
            /// </summary>
            [HisFieldInfoMapping(8, 1)]
            public string IW4SES { get; set; }

            /// <summary>
            /// IW4COCON X  
            /// *+COD.CONTITOLARE AAFF
            /// </summary>
            [HisFieldInfoMapping(9, 1)]
            public string IW4COCON { get; set; }

            /// <summary>
            /// IW4COMP X  
            /// *+COD.COMPONENTE AAFF
            /// </summary>
            [HisFieldInfoMapping(10, 1)]
            public string IW4COMP { get; set; }

            /// <summary>
            /// IW4CENAOL 9  
            /// *+CODICE ENAOLI AAFF
            /// </summary>
            [HisFieldInfoMapping(11, 1)]
            public short IW4CENAOL { get; set; }

            /// <summary>
            /// IW4PRGEM 9  
            /// *+PROGRESSIVO GEMELLI AAFF
            /// </summary>
            [HisFieldInfoMapping(12, 1)]
            public short IW4PRGEM { get; set; }

            // 10        IW4COFA.
            /// <summary>
            /// IW4FRAT 9  
            /// *+CF AF FRATELLI
            /// </summary>
            [HisFieldInfoMapping(13, 1)]
            public short IW4FRAT { get; set; }

            /// <summary>
            /// IW4GEN 9  
            /// *+CF AF GENITORI
            /// </summary>
            [HisFieldInfoMapping(14, 1)]
            public short IW4GEN { get; set; }

            /// <summary>
            /// IW4CON 9  
            /// *+CF AF CONIUGE
            /// </summary>
            [HisFieldInfoMapping(15, 1)]
            public short IW4CON { get; set; }

            /// <summary>
            /// IW4FIG 99  
            /// *+CF AF FIGLI
            /// </summary>
            [HisFieldInfoMapping(16, 2)]
            public short IW4FIG { get; set; }

            [HisComplexAreaInfoMapping(17, ListCount = 10)]
            public List<GP3CK> LIST_GP3CK { get; set; }

            #endregion Tracciato Host

        }

        public class CodiciFiscaliFamiliari
        {
            #region tracciato COBOL
            //************* CODICI FISCALI DEI FAMILIARI *******************
            //     05 AREAW4COFI.
            //         10 IW4COFI     OCCURS 15      PIC X(16).
            //*1998 CODICE FISCALE DEI FAMILIARI    16 X 15 = 240
            //*1999 ANNO DI 4 BYTE
            //**************************************************************
            //* 1996 IN TS7WK5AA.CPY     40 X 30 = 1200 X WK5!!!
            //* 2002 IN INPUTWKL.CPY     40 X 30 = 1200
            //* VALIDA DAL 2002
            #endregion tracciato COBOL

            #region Tracciato Host
            //************* CODICI FISCALI DEI FAMILIARI *******************
            // 05 AREAW4COFI.
            /// <summary>
            /// IW4COFI X(16)  
            /// *1998 CODICE FISCALE DEI FAMILIARI    16 X 15 = 240
            /// </summary>
            [HisFieldInfoMapping(0, 16)]
            public string IW4COFI { get; set; }

            // *1999 ANNO DI 4 BYTE
            //**************************************************************
            // * 1996 IN TS7WK5AA.CPY     40 X 30 = 1200 X WK5!!!
            // * 2002 IN INPUTWKL.CPY     40 X 30 = 1200
            // * VALIDA DAL 2002

            #endregion Tracciato Host
        }

        public class GP3CK
        {
            #region tracciato COBOL
            //cg2015          15 GP3CK       OCCURS 10.
            //cg2015           20        IDECOR-ACQ.
            //cg2015             25        IW4ACQA                 PIC 9999.
            //cg2015             25        IW4ACQM                 PIC 99.
            //cg2015           20        IW4ACQ REDEFINES IDECOR-ACQ      PIC 9(6).
            //cg2015*+1999 DECORRENZA AAFF
            //cg2015           20        IDECORCES.
            //cg2015             25        IW4CESA                 PIC 9999.
            //cg2015             25        IW4CESM                 PIC 99.
            //cg2015           20        IW4CES REDEFINES IDECORCES       PIC 9(6).
            //cg2015*+1999 CESSAZIONE AAFF
            //cg2015           20        IW4SIG                      PIC X.
            //cg2015*+SIGLA AAFF
            //                 20        GP3CH01B                    PIC X.
            //cg2015           20        IW4CMAG                     PIC 9.
            //cg2015*+COD.MAGGIORAZIONE AAFF
            #endregion tracciato COBOL

            #region Tracciato Host
            // 10        IDECOR-ACQ.
            /// <summary>
            /// IW4ACQA 9999  
            /// *+1999 DECORRENZA AAFF
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public short IW4ACQA { get; set; }

            /// <summary>
            /// IW4ACQM 99  
            /// *+1999 DECORRENZA AAFF
            /// </summary>
            [HisFieldInfoMapping(1, 2)]
            public short IW4ACQM { get; set; }

            // 10        IDECORCES.
            /// <summary>
            /// IW4CESA 9999  
            /// *+1999 CESSAZIONE AAFF
            /// </summary>
            [HisFieldInfoMapping(2, 4)]
            public short IW4CESA { get; set; }

            /// <summary>
            /// IW4CESM 99  
            /// *+1999 CESSAZIONE AAFF
            /// </summary>
            [HisFieldInfoMapping(3, 2)]
            public short IW4CESM { get; set; }

            /// <summary>
            /// IW4SIG X  
            /// *+SIGLA AAFF
            /// </summary>
            [HisFieldInfoMapping(4, 1)]
            public string IW4SIG { get; set; }

            /// <summary>
            /// GP3CH01B X  
            /// </summary>
            [HisFieldInfoMapping(5, 1)]
            public string GP3CH01B { get; set; }

            /// <summary>
            /// IW4CMAG 9  
            /// *+COD.MAGGIORAZIONE AAFF
            /// </summary>
            [HisFieldInfoMapping(6, 1)]
            public short IW4CMAG { get; set; }
            #endregion Tracciato Host
        }

        #endregion nested class
    }
}
