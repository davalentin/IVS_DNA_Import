using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostResponse.AreaStampa
{
    public class Anagrafica
    {
        #region Constructor
        internal Anagrafica()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //02 TIPO-RAP         PIC X(01) OCCURS 3 TIMES.
        //*                             CODICE TIPO RAPPORTO           053
        //*                             POSIZIONALE
        //*       P = PENSIONATO
        //*       D = DELEGATO / C = DELEGATO PER DOVERE D'UFFICIO
        //*       T = TUTORE   / R = RAPPRESENTANTE LEGALE
        //     02 COGNOME          PIC X(36) OCCURS 3 TIMES.
        //*                             COGNOME TITOLARE               089
        //     02 NOME             PIC X(36) OCCURS 3 TIMES.
        //*                             NOME TITOLARE                  197
        //     02 COGN-ACQ         PIC X(36) OCCURS 3 TIMES.
        //*                             COGNOME ACQUISITO              305
        //     02 DATA-NASC                  OCCURS 3 TIMES.
        //*                             DATA NASCITA (GGMMAAAA)        329
        //        03 DNASC-GG      PIC 9(02).
        //*                             GG
        //        03 DNASC-MM      PIC 9(02).
        //*                             MM
        //        03 DNASC-AA      PIC 9(04).
        //*                             AAAA
        //     02 CCOM-NASC        PIC 9(05) OCCURS 3 TIMES.
        //*       GP1RCOMUNE            COD.COMUNE NASCITA             344
        //     02 COM-NASC         PIC X(60) OCCURS 3 TIMES.
        //*       GP1RCOMUNE            COMUNE NASCITA                 524
        //     02 PROV-NASC        PIC X(03) OCCURS 3 TIMES.
        //*       GP1RPROV              SIGLA PROV.NASC./STATO ESTERO  531
        //     02 COD-FISC         PIC X(16) OCCURS 3 TIMES.
        //*                             CODICE FISCALE                 579
        //     02 SEX              PIC X(01) OCCURS 3 TIMES.
        //*                             SESSO                          582
        //     02 COD1-ARCA        PIC X(03) OCCURS 3 TIMES.
        //*       GP1CFCCC1             CODICE 1 ARCA                  591
        //     02 COD2-ARCA        PIC 9(08) OCCURS 3 TIMES.
        //*       GP1PROGR              CODICE 2 ARCA                  615
        //     02 COD-RESID        PIC X(01) OCCURS 3 TIMES.
        //*       GP1RRESIDOM           1=ITALIA 9=ESTERO              618
        //     02 INI-ESTERO                 OCCURS 3 TIMES.
        //*                            DATA INIZIO RESIDENZA ESTERO    642
        //*                            (ATTUALMENTE NON PRESENTE)
        //        03 IRES-GG       PIC 9(02).
        //*                            GG
        //        03 IRES-MM       PIC 9(02).
        //*                            MM
        //        03 IRES-AA       PIC 9(04).
        //*                            AAAA
        //     02 FIN-ESTERO                 OCCURS 3 TIMES.
        //*                            DATA FINE RESIDENZA ESTERO      666
        //*                            (ATTUALMENTE NON PRESENTE)
        //        03 FRES-GG       PIC 9(02).
        //*                            GG
        //        03 FRES-MM       PIC 9(02).
        //*                            MM
        //        03 FRES-AA       PIC 9(04).
        //*                            AAAA
        //     02 INDIRIZZO        PIC X(104) OCCURS 3 TIMES.
        //*       GP1RIND               INDIRIZZO               ***    978
        //     02 N-CIVICO         PIC X(18) OCCURS 3 TIMES.
        //*       GP1RCIVICO            NUMERO CIVICO                 1052
        //     02 FRAZIONE         PIC X(35) OCCURS 3 TIMES.
        //*       GP1RFRAZIONE          FRAZIONE/LOCAL.ESTERA         1106
        //     02 PROV-ESTER       PIC X(52) OCCURS 3 TIMES.
        //*       GP1RINDIRIZD          PROVINCIA ESTERA              1211
        //     02 CCOM-RES         PIC X(04) OCCURS 3 TIMES.
        //*       GP1RCODCOM            COD.CATAST.COMUNE/STATO EST.  1367
        //     02 COMUNE           PIC X(37) OCCURS 3 TIMES.
        //*       GP1RCOMUNE            COMUNE RES./STATO ESTERO      1379
        //     02 PROV             PIC X(03) OCCURS 3 TIMES.
        //*       GP1RPROV              SIGLA PROVINCIA/STATO ESTERO  1490
        //     02 C-A-P            PIC X(09) OCCURS 3 TIMES.
        //*       GP1RCAP               C.A.P.                        1499
        //     02 SIGLA            PIC X(02) OCCURS 3 TIMES.
        //*                                                           1526
        //*           PENSIONATO = PT
        //*           TUTORE     = PU   ETC...
        //     02 FILLER           PIC X(18).
        //*                                                           1532
        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 3)]
        public List<TipoRapporto> LISTTipoRapporto { get; internal set; }

        [HisComplexAreaInfoMapping(1, ListCount = 3)]
        public List<Cognome> LISTCognome { get; internal set; }

        [HisComplexAreaInfoMapping(2, ListCount = 3)]
        public List<Nome> LISTNome { get; internal set; }

        [HisComplexAreaInfoMapping(3, ListCount = 3)]
        public List<CognomeAcquisito> LISTCognomeAcquisito { get; internal set; }

        [HisComplexAreaInfoMapping(4, ListCount = 3)]
        public List<DataNascita> LISTDataNascita { get; internal set; }

        [HisComplexAreaInfoMapping(5, ListCount = 3)]
        public List<CodiceComuneNascita> LISTCodiceComuneNascita { get; internal set; }

        [HisComplexAreaInfoMapping(6, ListCount = 3)]
        public List<ComuneNascita> LISTComuneNascita { get; internal set; }

        [HisComplexAreaInfoMapping(7, ListCount = 3)]
        public List<ProvinciaNascita> LISTProvinciaNascita { get; internal set; }

        [HisComplexAreaInfoMapping(8, ListCount = 3)]
        public List<CodiceFiscale> LISTCodiceFiscale { get; internal set; }

        [HisComplexAreaInfoMapping(9, ListCount = 3)]
        public List<Sesso> LISTSesso { get; internal set; }

        [HisComplexAreaInfoMapping(10, ListCount = 3)]
        public List<CodiceArca1> LISTCodiceArca1 { get; internal set; }

        [HisComplexAreaInfoMapping(11, ListCount = 3)]
        public List<CodiceArca2> LISTCodiceArca2 { get; internal set; }

        [HisComplexAreaInfoMapping(12, ListCount = 3)]
        public List<CodiceResidenza> LISTCodiceResidenza { get; internal set; }

        [HisComplexAreaInfoMapping(13, ListCount = 3)]
        public List<DataInizioResidenza> LISTDataInizioResidenza { get; internal set; }

        [HisComplexAreaInfoMapping(14, ListCount = 3)]
        public List<DataFineResidenza> LISTDataFineResidenza { get; internal set; }

        [HisComplexAreaInfoMapping(15, ListCount = 3)]
        public List<Indirizzo> LISTIndirizzo { get; internal set; }

        [HisComplexAreaInfoMapping(16, ListCount = 3)]
        public List<Civico> LISTCivico { get; internal set; }

        [HisComplexAreaInfoMapping(17, ListCount = 3)]
        public List<Frazione> LISTFrazione { get; internal set; }

        [HisComplexAreaInfoMapping(18, ListCount = 3)]
        public List<ProvinciaEstera> LISTProvinciaEstera { get; internal set; }

        [HisComplexAreaInfoMapping(19, ListCount = 3)]
        public List<CodiceCatastale> LISTCodiceCatastale { get; internal set; }

        [HisComplexAreaInfoMapping(20, ListCount = 3)]
        public List<ComuneResidenza> LISTComuneResidenza { get; internal set; }

        [HisComplexAreaInfoMapping(21, ListCount = 3)]
        public List<ProvinciaResidenza> LISTProvinciaResidenza { get; internal set; }

        [HisComplexAreaInfoMapping(22, ListCount = 3)]
        public List<Cap> LISTCap { get; internal set; }

        [HisComplexAreaInfoMapping(23, ListCount = 3)]
        public List<Sigla> LISTSigla { get; internal set; }

        /// <summary>
        /// FILLER X(18)  
        /// </summary>
        [HisFieldInfoMapping(24, 18)]
        public string FILLER { get; set; }

        #endregion Tracciato Host

        #region nested class

        public class TipoRapporto
        {
            #region Constructor
            internal TipoRapporto()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //02 TIPO-RAP         PIC X(01) OCCURS 3 TIMES.
            //*                             CODICE TIPO RAPPORTO           053
            //*                             POSIZIONALE
            //*       P = PENSIONATO
            //*       D = DELEGATO / C = DELEGATO PER DOVERE D'UFFICIO
            //*       T = TUTORE   / R = RAPPRESENTANTE LEGALE
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// TIPO_RAP X(01)  
            /// </summary>
            [HisFieldInfoMapping(0, 1)]
            public string TIPO_RAP { get; set; }

            // *                             CODICE TIPO RAPPORTO           053
            // *                             POSIZIONALE
            // *       P = PENSIONATO
            // *       D = DELEGATO / C = DELEGATO PER DOVERE D'UFFICIO
            // *       T = TUTORE   / R = RAPPRESENTANTE LEGALE
            #endregion Tracciato Host
            #endregion Properties
        }

        public class Cognome
        {
            #region Constructor
            internal Cognome()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 COGNOME          PIC X(36) OCCURS 3 TIMES.
            //*                             COGNOME TITOLARE               089
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// COGNOME X(36)  
            /// </summary>
            [HisFieldInfoMapping(0, 36)]
            public string COGNOME { get; set; }

            // *                             COGNOME TITOLARE               089
            #endregion Tracciato Host
            #endregion Properties
        }

        public class Nome
        {
            #region Constructor
            internal Nome()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 NOME             PIC X(36) OCCURS 3 TIMES.
            //*                             NOME TITOLARE                  197
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// NOME X(36)  
            /// </summary>
            [HisFieldInfoMapping(0, 36)]
            public string NOME { get; set; }

            // *                             NOME TITOLARE                  197
            #endregion Tracciato Host
            #endregion Properties
        }

        public class CognomeAcquisito
        {
            #region Constructor
            internal CognomeAcquisito()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 COGN-ACQ         PIC X(36) OCCURS 3 TIMES.
            //*                             COGNOME ACQUISITO              305
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// COGN_ACQ X(36)  
            /// </summary>
            [HisFieldInfoMapping(0, 36)]
            public string COGN_ACQ { get; set; }

            // *                             COGNOME ACQUISITO              305
            #endregion Tracciato Host
            #endregion Properties
        }

        public class DataNascita
        {
            #region Constructor
            internal DataNascita()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 DATA-NASC                  OCCURS 3 TIMES.
            //*                             DATA NASCITA (GGMMAAAA)        329
            //        03 DNASC-GG      PIC 9(02).
            //*                             GG
            //        03 DNASC-MM      PIC 9(02).
            //*                             MM
            //        03 DNASC-AA      PIC 9(04).
            //*                             AAAA
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 DATA-NASC                  OCCURS 3 TIMES.
            // *                             DATA NASCITA (GGMMAAAA)        329
            /// <summary>
            /// DNASC_GG 9(02)  
            /// </summary>
            [HisFieldInfoMapping(0, 2, CobolType = CobolType.Unsigned)]
            public short DNASC_GG { get; set; }

            // *                             GG
            /// <summary>
            /// DNASC_MM 9(02)  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short DNASC_MM { get; set; }

            // *                             MM
            /// <summary>
            /// DNASC_AA 9(04)  
            /// </summary>
            [HisFieldInfoMapping(2, 4, CobolType = CobolType.Unsigned)]
            public short DNASC_AA { get; set; }

            // *                             AAAA

            #endregion Tracciato Host
            #endregion Properties
        }

        public class CodiceComuneNascita
        {
            #region Constructor
            internal CodiceComuneNascita()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 CCOM-NASC        PIC 9(05) OCCURS 3 TIMES.
            //*       GP1RCOMUNE            COD.COMUNE NASCITA             344
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// CCOM_NASC 9(05)  
            /// </summary>
            [HisFieldInfoMapping(0, 5, CobolType = CobolType.Unsigned)]
            public int CCOM_NASC { get; set; }

            // *       GP1RCOMUNE            COD.COMUNE NASCITA             344
            #endregion Tracciato Host
            #endregion Properties
        }

        public class ComuneNascita
        {
            #region Constructor
            internal ComuneNascita()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 COM-NASC         PIC X(60) OCCURS 3 TIMES.
            //*       GP1RCOMUNE            COMUNE NASCITA                 524
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// COM_NASC X(60)  
            /// </summary>
            [HisFieldInfoMapping(0, 60)]
            public string COM_NASC { get; set; }

            // *       GP1RCOMUNE            COMUNE NASCITA                 524
            #endregion Tracciato Host
            #endregion Properties
        }

        public class ProvinciaNascita
        {
            #region Constructor
            internal ProvinciaNascita()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 PROV-NASC        PIC X(03) OCCURS 3 TIMES.
            //*       GP1RPROV              SIGLA PROV.NASC./STATO ESTERO  531
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// PROV_NASC X(03)  
            /// </summary>
            [HisFieldInfoMapping(0, 3)]
            public string PROV_NASC { get; set; }

            // *       GP1RPROV              SIGLA PROV.NASC./STATO ESTERO  531
            #endregion Tracciato Host
            #endregion Properties
        }

        public class CodiceFiscale
        {
            #region Constructor
            internal CodiceFiscale()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 COD-FISC         PIC X(16) OCCURS 3 TIMES.
            //*                             CODICE FISCALE                 579
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// COD_FISC X(16)  
            /// </summary>
            [HisFieldInfoMapping(0, 16)]
            public string COD_FISC { get; set; }

            // *                             CODICE FISCALE                 579
            #endregion Tracciato Host
            #endregion Properties
        }

        public class Sesso
        {
            #region Constructor
            internal Sesso()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 SEX              PIC X(01) OCCURS 3 TIMES.
            //*                             SESSO                          582
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// SEX X(01)  
            /// </summary>
            [HisFieldInfoMapping(0, 1)]
            public string SEX { get; set; }

            // *                             SESSO                          582
            #endregion Tracciato Host
            #endregion Properties
        }

        public class CodiceArca1
        {
            #region Constructor
            internal CodiceArca1()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 COD1-ARCA        PIC X(03) OCCURS 3 TIMES.
            //*       GP1CFCCC1             CODICE 1 ARCA                  591
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// COD1_ARCA X(03)  
            /// </summary>
            [HisFieldInfoMapping(0, 3)]
            public string COD1_ARCA { get; set; }

            // *       GP1CFCCC1             CODICE 1 ARCA                  591
            #endregion Tracciato Host
            #endregion Properties
        }

        public class CodiceArca2
        {
            #region Constructor
            internal CodiceArca2()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 COD2-ARCA        PIC 9(08) OCCURS 3 TIMES.
            //*       GP1PROGR              CODICE 2 ARCA                  615
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// COD2_ARCA 9(08)  
            /// </summary>
            [HisFieldInfoMapping(0, 8, CobolType = CobolType.Unsigned)]
            public int COD2_ARCA { get; set; }

            // *       GP1PROGR              CODICE 2 ARCA                  615
            #endregion Tracciato Host
            #endregion Properties
        }

        public class CodiceResidenza
        {
            #region Constructor
            internal CodiceResidenza()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 COD-RESID        PIC X(01) OCCURS 3 TIMES.
            //*       GP1RRESIDOM           1=ITALIA 9=ESTERO              618
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// COD_RESID X(01)  
            /// </summary>
            [HisFieldInfoMapping(0, 1)]
            public string COD_RESID { get; set; }

            // *       GP1RRESIDOM           1=ITALIA 9=ESTERO              618
            #endregion Tracciato Host
            #endregion Properties
        }

        public class DataInizioResidenza
        {
            #region Constructor
            internal DataInizioResidenza()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 INI-ESTERO                 OCCURS 3 TIMES.
            //*                            DATA INIZIO RESIDENZA ESTERO    642
            //*                            (ATTUALMENTE NON PRESENTE)
            //        03 IRES-GG       PIC 9(02).
            //*                            GG
            //        03 IRES-MM       PIC 9(02).
            //*                            MM
            //        03 IRES-AA       PIC 9(04).
            //*                            AAAA
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 INI-ESTERO                 OCCURS 3 TIMES.
            // *                            DATA INIZIO RESIDENZA ESTERO    642
            // *                            (ATTUALMENTE NON PRESENTE)
            /// <summary>
            /// IRES_GG 9(02)  
            /// </summary>
            [HisFieldInfoMapping(0, 2, CobolType = CobolType.Unsigned)]
            public short IRES_GG { get; set; }

            // *                            GG
            /// <summary>
            /// IRES_MM 9(02)  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short IRES_MM { get; set; }

            // *                            MM
            /// <summary>
            /// IRES_AA 9(04)  
            /// </summary>
            [HisFieldInfoMapping(2, 4, CobolType = CobolType.Unsigned)]
            public short IRES_AA { get; set; }

            // *                            AAAA
            #endregion Tracciato Host
            #endregion Properties
        }

        public class DataFineResidenza
        {
            #region Constructor
            internal DataFineResidenza()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 FIN-ESTERO                 OCCURS 3 TIMES.
            //*                            DATA FINE RESIDENZA ESTERO      666
            //*                            (ATTUALMENTE NON PRESENTE)
            //        03 FRES-GG       PIC 9(02).
            //*                            GG
            //        03 FRES-MM       PIC 9(02).
            //*                            MM
            //        03 FRES-AA       PIC 9(04).
            //*                            AAAA
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 FIN-ESTERO                 OCCURS 3 TIMES.
            // *                            DATA FINE RESIDENZA ESTERO      666
            // *                            (ATTUALMENTE NON PRESENTE)
            /// <summary>
            /// FRES_GG 9(02)  
            /// </summary>
            [HisFieldInfoMapping(0, 2, CobolType = CobolType.Unsigned)]
            public short FRES_GG { get; set; }

            // *                            GG
            /// <summary>
            /// FRES_MM 9(02)  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short FRES_MM { get; set; }

            // *                            MM
            /// <summary>
            /// FRES_AA 9(04)  
            /// </summary>
            [HisFieldInfoMapping(2, 4, CobolType = CobolType.Unsigned)]
            public short FRES_AA { get; set; }

            // *                            AAAA
            #endregion Tracciato Host
            #endregion Properties
        }

        public class Indirizzo
        {
            #region Constructor
            internal Indirizzo()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 INDIRIZZO        PIC X(104) OCCURS 3 TIMES.
            //*       GP1RIND               INDIRIZZO               ***    978
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// INDIRIZZO X(104)  
            /// </summary>
            [HisFieldInfoMapping(0, 104)]
            public string INDIRIZZO { get; set; }

            // *       GP1RIND               INDIRIZZO               ***    978
            #endregion Tracciato Host
            #endregion Properties
        }

        public class Civico
        {
            #region Constructor
            internal Civico()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 N-CIVICO         PIC X(18) OCCURS 3 TIMES.
            //*       GP1RCIVICO            NUMERO CIVICO                 1052
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// N_CIVICO X(18)  
            /// </summary>
            [HisFieldInfoMapping(0, 18)]
            public string N_CIVICO { get; set; }

            // *       GP1RCIVICO            NUMERO CIVICO                 1052
            #endregion Tracciato Host
            #endregion Properties
        }

        public class Frazione
        {
            #region Constructor
            internal Frazione()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 FRAZIONE         PIC X(35) OCCURS 3 TIMES.
            //*       GP1RFRAZIONE          FRAZIONE/LOCAL.ESTERA         1106           
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// FRAZIONE X(35)  
            /// </summary>
            [HisFieldInfoMapping(0, 35)]
            public string FRAZIONE { get; set; }

            // *       GP1RFRAZIONE          FRAZIONE/LOCAL.ESTERA         1106
            #endregion Tracciato Host
            #endregion Properties
        }

        public class ProvinciaEstera
        {
            #region Constructor
            internal ProvinciaEstera()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 PROV-ESTER       PIC X(52) OCCURS 3 TIMES.
            //*       GP1RINDIRIZD          PROVINCIA ESTERA              1211
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// PROV_ESTER X(52)  
            /// </summary>
            [HisFieldInfoMapping(0, 52)]
            public string PROV_ESTER { get; set; }

            // *       GP1RINDIRIZD          PROVINCIA ESTERA              1211
            #endregion Tracciato Host
            #endregion Properties
        }

        public class CodiceCatastale
        {
            #region Constructor
            internal CodiceCatastale()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 CCOM-RES         PIC X(04) OCCURS 3 TIMES.
            //*       GP1RCODCOM            COD.CATAST.COMUNE/STATO EST.  1367
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// CCOM_RES X(04)  
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public string CCOM_RES { get; set; }

            // *       GP1RCODCOM            COD.CATAST.COMUNE/STATO EST.  1367
            #endregion Tracciato Host
            #endregion Properties
        }

        public class ComuneResidenza
        {
            #region Constructor
            internal ComuneResidenza()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 COMUNE           PIC X(37) OCCURS 3 TIMES.
            //*       GP1RCOMUNE            COMUNE RES./STATO ESTERO      1379
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// COMUNE X(37)  
            /// </summary>
            [HisFieldInfoMapping(0, 37)]
            public string COMUNE { get; set; }

            // *       GP1RCOMUNE            COMUNE RES./STATO ESTERO      1379
            #endregion Tracciato Host
            #endregion Properties
        }

        public class ProvinciaResidenza
        {
            #region Constructor
            internal ProvinciaResidenza()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 PROV             PIC X(03) OCCURS 3 TIMES.
            //*       GP1RPROV              SIGLA PROVINCIA/STATO ESTERO  1490
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// PROV X(03)  
            /// </summary>
            [HisFieldInfoMapping(0, 3)]
            public string PROV { get; set; }

            // *       GP1RPROV              SIGLA PROVINCIA/STATO ESTERO  1490
            #endregion Tracciato Host
            #endregion Properties
        }

        public class Cap
        {
            #region Constructor
            internal Cap()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 C-A-P            PIC X(09) OCCURS 3 TIMES.
            //*       GP1RCAP               C.A.P.                        1499
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// C_A_P X(09)  
            /// </summary>
            [HisFieldInfoMapping(0, 9)]
            public string C_A_P { get; set; }

            // *       GP1RCAP               C.A.P.                        1499
            #endregion Tracciato Host
            #endregion Properties
        }

        public class Sigla
        {
            #region Constructor
            internal Sigla()
            {

            }
            #endregion Constructor

            #region Properties

            #region Tracciato COBOL
            //     02 SIGLA            PIC X(02) OCCURS 3 TIMES.
            //*                                                           1526
            //*           PENSIONATO = PT
            //*           TUTORE     = PU   ETC...
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// SIGLA X(02)  
            /// </summary>
            [HisFieldInfoMapping(0, 2)]
            public string SIGLA { get; set; }

            // *                                                           1526
            // *           PENSIONATO = PT
            // *           TUTORE     = PU   ETC...
            #endregion Tracciato Host
            #endregion Properties
        }

        #endregion nested class

        #endregion Properties
    }
}

