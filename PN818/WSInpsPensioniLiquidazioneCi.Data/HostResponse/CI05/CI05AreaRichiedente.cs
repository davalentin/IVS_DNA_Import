using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class CI05AreaRichiedente
    {
        #region Constructor
        internal CI05AreaRichiedente()
		{
			this.EAD75 = new Area_EAD75();
			this.CI2005 = new Area_CI2005();
            this.StatiEsteri = new List<Area_StatiEsteri>();
            this.DatiVari = new Area_Vari();
		}
        #endregion Constructor

        #region Properties
        [HisComplexAreaInfoMapping(0)]
		public Area_EAD75 EAD75 {get; set; }

		[HisComplexAreaInfoMapping(1)]
		public Area_CI2005 CI2005 { get; set; }

        [HisComplexAreaInfoMapping(2, ListCount = 4)]
        public List<Area_StatiEsteri> StatiEsteri { get; set; }

        [HisComplexAreaInfoMapping(3)]
		public Area_Vari DatiVari { get; set; }
        #endregion Properties

        #region nested class
        public class Area_EAD75
        {
            #region Constructor
            internal Area_EAD75()
            {
            }
            #endregion Constructor

            #region tracciato COBOL
            //      03  A-AREA-EAD75.
            //       05  A-IDO                     PIC 9.
            //       05  A-KKK2                    PIC 9(8).
            //       05  A-KEY-DOM.
            //           10  A-DATAINSE                PIC 9(8).
            //           10  A-KDOMUS                  PIC 9(13).
            //*CHIAVE DOMUS
            //       05  A-SEDE6                       PIC 9(6).
            //       05  A-CATEG                   PIC X(6).
            //       05  A-COGNOME                 PIC X(32).
            //       05  A-SESSO                   PIC X.
            //       05  A-NOMEACQ                 PIC X(13).
            //       05  A-DATANAS                 PIC 9(8).
            //       05  A-COMNAS                  PIC X(23).
            //       05  A-DISPON                  PIC X(6).
            //       05  A-PROVNAS                 PIC X(3).
            //       05  A-INDIRIZ                 PIC X(32).
            //       05  A-XCAP                    PIC X(9).
            //       05  A-COMRES                  PIC X(22).
            //       05  A-PROVRES                 PIC X(3).
            //       05  A-STATRES                 PIC X(3).
            //       05  A-TIPODOM                 PIC X.
            //       05  A-CODPAT0                 PIC X(3).
            //       05  A-NPRATPA                 PIC X(8).
            //       05  A-DATADOM                 PIC 9(8).
            //       05  A-COFASE                  PIC 9.
            //       05  A-COINFA                  PIC 9(8).
            //       05  A-DATALGD                 PIC 9(8).
            //       05  A-DATDALG                 PIC 9(8).
            //       05  A-DATESIT                 PIC 9(8).
            //       05  A-CODESIT                 PIC X.
            //       05  FILLER                    PIC X.
            //       05  A-CODISITU                PIC X(4).
            //       05  A-SEDERIC                 PIC 9(4).
            //       05  A-CAT-CER.
            //           10  A-CATEGRI             PIC X(6).
            //           10  A-CERTRIC             PIC 9(8)
            //       05  A-PRINTF3                 PIC X(3).
            //       05  A-DECPEN                  PIC 9(6).
            //       05  A-DIOTI                   PIC X.
            #endregion tracciato COBOL

            #region Tracciato Host
            // 03  A-AREA-EAD75.
            /// <summary>
            /// A_IDO 9  
            /// </summary>
            [HisFieldInfoMapping(0, 1)]
            public short A_IDO { get; set; }

            /// <summary>
            /// A_KKK2 9(8)  
            /// </summary>
            [HisFieldInfoMapping(1, 8)]
            public int A_KKK2 { get; set; }

            // 05  A-KEY-DOM.
            /// <summary>
            /// A_DATAINSE 9(8)  
            /// </summary>
            [HisFieldInfoMapping(2, 8)]
            public int A_DATAINSE { get; set; }

            /// <summary>
            /// A_KDOMUS 9(13)  
            /// </summary>
            [HisFieldInfoMapping(3, 13)]
            public long A_KDOMUS { get; set; }

            // *CHIAVE DOMUS
            /// <summary>
            /// A_SEDE6 9(6)  
            /// </summary>
            [HisFieldInfoMapping(4, 6)]
            public int A_SEDE6 { get; set; }

            /// <summary>
            /// A_CATEG X(6)  
            /// </summary>
            [HisFieldInfoMapping(5, 6)]
            public string A_CATEG { get; set; }

            /// <summary>
            /// A_COGNOME X(32)  
            /// </summary>
            [HisFieldInfoMapping(6, 32)]
            public string A_COGNOME { get; set; }

            /// <summary>
            /// A_SESSO X  
            /// </summary>
            [HisFieldInfoMapping(7, 1)]
            public string A_SESSO { get; set; }

            /// <summary>
            /// A_NOMEACQ X(13)  
            /// </summary>
            [HisFieldInfoMapping(8, 13)]
            public string A_NOMEACQ { get; set; }

            /// <summary>
            /// A_DATANAS 9(8)  
            /// </summary>
            [HisFieldInfoMapping(9, 8)]
            public int A_DATANAS { get; set; }

            /// <summary>
            /// A_COMNAS X(23)  
            /// </summary>
            [HisFieldInfoMapping(10, 23)]
            public string A_COMNAS { get; set; }

            /// <summary>
            /// A_DISPON X(6)  
            /// </summary>
            [HisFieldInfoMapping(11, 6)]
            public string A_DISPON { get; set; }

            /// <summary>
            /// A_PROVNAS X(3)  
            /// </summary>
            [HisFieldInfoMapping(12, 3)]
            public string A_PROVNAS { get; set; }

            /// <summary>
            /// A_INDIRIZ X(32)  
            /// </summary>
            [HisFieldInfoMapping(13, 32)]
            public string A_INDIRIZ { get; set; }

            /// <summary>
            /// A_XCAP X(9)  
            /// </summary>
            [HisFieldInfoMapping(14, 9)]
            public string A_XCAP { get; set; }

            /// <summary>
            /// A_COMRES X(22)  
            /// </summary>
            [HisFieldInfoMapping(15, 22)]
            public string A_COMRES { get; set; }

            /// <summary>
            /// A_PROVRES X(3)  
            /// </summary>
            [HisFieldInfoMapping(16, 3)]
            public string A_PROVRES { get; set; }

            /// <summary>
            /// A_STATRES X(3)  
            /// </summary>
            [HisFieldInfoMapping(17, 3)]
            public string A_STATRES { get; set; }

            /// <summary>
            /// A_TIPODOM X  
            /// </summary>
            [HisFieldInfoMapping(18, 1)]
            public string A_TIPODOM { get; set; }

            /// <summary>
            /// A_CODPAT0 X(3)  
            /// </summary>
            [HisFieldInfoMapping(19, 3)]
            public string A_CODPAT0 { get; set; }

            /// <summary>
            /// A_NPRATPA X(8)  
            /// </summary>
            [HisFieldInfoMapping(20, 8)]
            public string A_NPRATPA { get; set; }

            /// <summary>
            /// A_DATADOM 9(8)  
            /// </summary>
            [HisFieldInfoMapping(21, 8)]
            public int A_DATADOM { get; set; }

            /// <summary>
            /// A_COFASE 9  
            /// </summary>
            [HisFieldInfoMapping(22, 1)]
            public short A_COFASE { get; set; }

            /// <summary>
            /// A_COINFA 9(8)  
            /// </summary>
            [HisFieldInfoMapping(23, 8)]
            public int A_COINFA { get; set; }

            /// <summary>
            /// A_DATALGD 9(8)  
            /// </summary>
            [HisFieldInfoMapping(24, 8)]
            public int A_DATALGD { get; set; }

            /// <summary>
            /// A_DATDALG 9(8)  
            /// </summary>
            [HisFieldInfoMapping(25, 8)]
            public int A_DATDALG { get; set; }

            /// <summary>
            /// A_DATESIT 9(8)  
            /// </summary>
            [HisFieldInfoMapping(26, 8)]
            public int A_DATESIT { get; set; }

            /// <summary>
            /// A_CODESIT X  
            /// </summary>
            [HisFieldInfoMapping(27, 1)]
            public string A_CODESIT { get; set; }

            /// <summary>
            /// FILLER X  
            /// </summary>
            [HisFieldInfoMapping(28, 1)]
            public string FILLER { get; set; }

            /// <summary>
            /// A_CODISITU X(4)  
            /// </summary>
            [HisFieldInfoMapping(29, 4)]
            public string A_CODISITU { get; set; }

            /// <summary>
            /// A_SEDERIC 9(4)  
            /// </summary>
            [HisFieldInfoMapping(30, 4)]
            public short A_SEDERIC { get; set; }

            // 05  A-CAT-CER.
            /// <summary>
            /// A_CATEGRI X(6)  
            /// </summary>
            [HisFieldInfoMapping(31, 6)]
            public string A_CATEGRI { get; set; }

            /// <summary>
            // 10  A-CERTRIC             PIC 9(8)
            /// <summary>
            [HisFieldInfoMapping(32, 8)]
            public int A_CERTRIC { get; set; }

            /// <summary>
            /// A_PRINTF3 X(3)  
            /// </summary>
            [HisFieldInfoMapping(33, 3)]
            public string A_PRINTF3 { get; set; }

            /// <summary>
            /// A_DECPEN 9(6)  
            /// </summary>
            [HisFieldInfoMapping(34, 6)]
            public int A_DECPEN { get; set; }

            /// <summary>
            /// A_DIOTI X  
            /// </summary>
            [HisFieldInfoMapping(35, 1)]
            public string A_DIOTI { get; set; }
            #endregion Tracciato Host
        }

        public class Area_CI2005
        {
            #region Constructor
            internal Area_CI2005()
            {
            }
            #endregion Constructor

            #region tracciato COBOL
            //          03  A-AREA-CI2005.
            //             05  A-CITT1                   PIC X(3).
            //             05  A-STATC                   PIC X.
            //             05  A-DATACAR                 PIC 9(8).
            //             05  A-PRESTP                  PIC X.
            //***********************************************************************
            //***********   INTERESSATO
            //***********************************************************************
            //             05  A-ERREOSI                 PIC X.
            //             05  A-DOCRICI                 PIC X(10).
            //             05  A-DATRICI                 PIC 9(8).
            //***********************************************************************
            //************   SEDE
            //***********************************************************************
            //             05  A-SEDEC106                PIC 9(6).
            //             05  A-ERREOSS                 PIC X.
            //             05  A-DOCRICS                 PIC X(10).
            //             05  A-DATRICS                 PIC 9(8).
            //***********************************************************************
            //***********   ALTRI
            //***********************************************************************
            //             05  A-ERREOSA                 PIC X.
            //             05  A-DATRICA                 PIC 9(8).
            //***********************************************************************
            //************   ESITO
            //***********************************************************************
            //             05  A-CODEXIT                 PIC X.
            //             05  A-DATEXIT                 PIC 9(8).
            #endregion tracciato COBOL

            #region Tracciato Host
            // 03  A-AREA-CI2005.
            /// <summary>
            /// A_CITT1 X(3)  
            /// </summary>
            [HisFieldInfoMapping(0, 3)]
            public string A_CITT1 { get; set; }

            /// <summary>
            /// A_STATC X  
            /// </summary>
            [HisFieldInfoMapping(1, 1)]
            public string A_STATC { get; set; }

            /// <summary>
            /// A_DATACAR 9(8)  
            /// </summary>
            [HisFieldInfoMapping(2, 8)]
            public int A_DATACAR { get; set; }

            /// <summary>
            /// A_PRESTP X  
            /// </summary>
            [HisFieldInfoMapping(3, 1)]
            public string A_PRESTP { get; set; }

            //***********************************************************************
            //***********   INTERESSATO
            //***********************************************************************
            /// <summary>
            /// A_ERREOSI X  
            /// </summary>
            [HisFieldInfoMapping(4, 1)]
            public string A_ERREOSI { get; set; }

            /// <summary>
            /// A_DOCRICI X(10)  
            /// </summary>
            [HisFieldInfoMapping(5, 10)]
            public string A_DOCRICI { get; set; }

            /// <summary>
            /// A_DATRICI 9(8)  
            /// </summary>
            [HisFieldInfoMapping(6, 8)]
            public int A_DATRICI { get; set; }

            //***********************************************************************
            //************   SEDE
            //***********************************************************************
            /// <summary>
            /// A_SEDEC106 9(6)  
            /// </summary>
            [HisFieldInfoMapping(7, 6)]
            public int A_SEDEC106 { get; set; }

            /// <summary>
            /// A_ERREOSS X  
            /// </summary>
            [HisFieldInfoMapping(8, 1)]
            public string A_ERREOSS { get; set; }

            /// <summary>
            /// A_DOCRICS X(10)  
            /// </summary>
            [HisFieldInfoMapping(9, 10)]
            public string A_DOCRICS { get; set; }

            /// <summary>
            /// A_DATRICS 9(8)  
            /// </summary>
            [HisFieldInfoMapping(10, 8)]
            public int A_DATRICS { get; set; }

            //***********************************************************************
            //***********   ALTRI
            //***********************************************************************
            /// <summary>
            /// A_ERREOSA X  
            /// </summary>
            [HisFieldInfoMapping(11, 1)]
            public string A_ERREOSA { get; set; }

            /// <summary>
            /// A_DATRICA 9(8)  
            /// </summary>
            [HisFieldInfoMapping(12, 8)]
            public int A_DATRICA { get; set; }

            //***********************************************************************
            //************   ESITO
            //***********************************************************************
            /// <summary>
            /// A_CODEXIT X  
            /// </summary>
            [HisFieldInfoMapping(13, 1)]
            public string A_CODEXIT { get; set; }

            /// <summary>
            /// A_DATEXIT 9(8)  
            /// </summary>
            [HisFieldInfoMapping(14, 8)]
            public int A_DATEXIT { get; set; }


            #endregion Tracciato Host
        }

        public class Area_StatiEsteri
        {
            #region Constructor
            internal Area_StatiEsteri()
            {
            }
            #endregion Constructor

            #region tracciato COBOL
            //            03  A-AREA-STATI-ESTERI.
            //             05  A-STIS01                  PIC 9(6).
            //             05  A-MATRE1                  PIC X(16).
            //             05  A-PI1                     PIC X.
            //             05  A-MOT1                    PIC XX.
            //             05  A-MVV1                    PIC XX.
            //             05  A-FASE-ST1                PIC 999.
            //             05  A-MOD-2051                PIC X.
            //             05  A-INICOL1                 PIC 9(8).
            //             05  A-INVFOR1                 PIC 9(8).
            //             05  A-PARAG1                  PIC X(10).
            //             05  A-DATC111                 PIC 9(8).
            //             05  A-DATRIS1                 PIC 9(8).
            //             05  A-PRESTE1                 PIC X.
            //             05  A-RISULT1                 PIC 9(8).
            //             05  A-NOTE-ST1                PIC X(35).
            //***********************************************************************
            //************           2*   ISTITUZIONE ESTERA
            //***********************************************************************
            //             05  A-STIS02                  PIC 9(6).
            //             05  A-MATRE2                  PIC X(16).
            //             05  A-PI2                     PIC X.
            //             05  A-MOT2                    PIC XX.
            //             05  A-MVV2                    PIC XX.
            //             05  A-FASE-ST2                PIC 999.
            //             05  A-MOD-2052                PIC X.
            //             05  A-INICOL2                 PIC 9(8).
            //             05  A-INVFOR2                 PIC 9(8).
            //             05  A-PARAG2                  PIC X(10).
            //             05  A-DATC112                 PIC 9(8).
            //             05  A-DATRIS2                 PIC 9(8).
            //             05  A-PRESTE2                 PIC X.
            //             05  A-RISULT2                 PIC 9(8).
            //             05  A-NOTE-ST2                PIC X(35).
            //      *****************************************************************
            //      ******           3*   ISTITUZIONE ESTERA
            //      *****************************************************
            //             05  A-STIS03                  PIC 9(6).
            //             05  A-MATRE3                  PIC X(16).
            //             05  A-PI3                     PIC X.
            //             05  A-MOT3                    PIC XX.
            //             05  A-MVV3                    PIC XX.
            //             05  A-FASE-ST3                PIC 999.
            //             05  A-MOD-2053                PIC X.
            //             05  A-INICOL3                 PIC 9(8).
            //             05  A-INVFOR3                 PIC 9(8).
            //             05  A-PARAG3                  PIC X(10).
            //             05  A-DATC113                 PIC 9(8).
            //             05  A-DATRIS3                 PIC 9(8).
            //             05  A-PRESTE3                 PIC X.
            //             05  A-RISULT3                 PIC 9(8).
            //             05  A-NOTE-ST3                PIC X(35).
            //      *****************************************************
            //      ******           4*   ISTITUZIONE ESTERA
            //      *****************************************************
            //             05  A-STIS04                  PIC 9(6).
            //             05  A-MATRE4                  PIC X(16).
            //             05  A-PI4                     PIC X.
            //             05  A-MOT4                    PIC XX.
            //             05  A-MVV4                    PIC XX.
            //             05  A-FASE-ST4                PIC 999.
            //             05  A-MOD-2054                PIC X.
            //             05  A-INICOL4                 PIC 9(8).
            //             05  A-INVFOR4                 PIC 9(8).
            //             05  A-PARAG4                  PIC X(10).
            //             05  A-DATC114                 PIC 9(8).
            //             05  A-DATRIS4                 PIC 9(8).
            //             05  A-PRESTE4                 PIC X.
            //             05  A-RISULT4                 PIC 9(8).
            //             05  A-NOTE-ST4                PIC X(35).
            #endregion tracciato COBOL

            #region Tracciato Host
            // 03  A-AREA-STATI-ESTERI.
            /// <summary>
            /// A_STIS01 9(6)  
            /// </summary>
            [HisFieldInfoMapping(0, 6)]
            public int A_STIS { get; set; }

            /// <summary>
            /// A_MATRE1 X(16)  
            /// </summary>
            [HisFieldInfoMapping(1, 16)]
            public string A_MATRE { get; set; }

            /// <summary>
            /// A_PI1 X  
            /// </summary>
            [HisFieldInfoMapping(2, 1)]
            public string A_PI { get; set; }

            /// <summary>
            /// A_MOT1 XX  
            /// </summary>
            [HisFieldInfoMapping(3, 2)]
            public string A_MOT { get; set; }

            /// <summary>
            /// A_MVV1 XX  
            /// </summary>
            [HisFieldInfoMapping(4, 2)]
            public string A_MVV { get; set; }

            /// <summary>
            /// A_FASE_ST1 999  
            /// </summary>
            [HisFieldInfoMapping(5, 3)]
            public short A_FASE_ST { get; set; }

            /// <summary>
            /// A_MOD_2051 X  
            /// </summary>
            [HisFieldInfoMapping(6, 1)]
            public string A_MOD_205 { get; set; }

            /// <summary>
            /// A_INICOL1 9(8)  
            /// </summary>
            [HisFieldInfoMapping(7, 8)]
            public int A_INICOL { get; set; }

            /// <summary>
            /// A_INVFOR1 9(8)  
            /// </summary>
            [HisFieldInfoMapping(8, 8)]
            public int A_INVFOR1 { get; set; }

            /// <summary>
            /// A_PARAG1 X(10)  
            /// </summary>
            [HisFieldInfoMapping(9, 10)]
            public string A_PARAG { get; set; }

            /// <summary>
            /// A_DATC111 9(8)  
            /// </summary>
            [HisFieldInfoMapping(10, 8)]
            public int A_DATC11 { get; set; }

            /// <summary>
            /// A_DATRIS1 9(8)  
            /// </summary>
            [HisFieldInfoMapping(11, 8)]
            public int A_DATRIS { get; set; }

            /// <summary>
            /// A_PRESTE1 X  
            /// </summary>
            [HisFieldInfoMapping(12, 1)]
            public string A_PRESTE { get; set; }

            /// <summary>
            /// A_RISULT1 9(8)  
            /// </summary>
            [HisFieldInfoMapping(13, 8)]
            public int A_RISULT { get; set; }

            /// <summary>
            /// A_NOTE_ST1 X(35)  
            /// </summary>
            [HisFieldInfoMapping(14, 35)]
            public string A_NOTE_ST { get; set; }
            #endregion Tracciato Host
        }

        public class Area_Vari
        {
            #region Constructor
            internal Area_Vari()
            {
            }
            #endregion Constructor

            #region tracciato COBOL
            //03  A-AREA-VARI.
            //  05  A-MATRIC-MIGR             PIC X(10).
            //  05  A-RESEST                  PIC X.
            //  05  A-RIGA75                  PIC X(75).
            //  05  A-STAZ                    PIC 99.
            //  05  A-TIP-IST                 PIC X.
            //  05  A-CODFIS                  PIC X(16).
            //  05  A-UNIPROC                 PIC X(3).
            //  05  A-MAIL                    PIC X(50).
            //  05  A-COGNOME-INTERO          PIC X(36).
            //  05  A-NOME-INTERO             PIC X(36).
            //  05  A-COGNOACQ-INTERO         PIC X(36).
            //  05  A-ALTRI-DATI-STATI .
            //      10 A-ALTRI-DATI-STATI-OCC   OCCURS 4.
            //         15 A-CI14              PIC X.
            //         15 A-TIPC              PIC XX.
            //         15 A-MODEL             PIC X(7).
            //  05  A-DATACANC                PIC 9(8).
            //  05  FILLER                    PIC X(59).
            #endregion tracciato COBOL

            #region Tracciato Host
            // 03  A-AREA-VARI.
            /// <summary>
            /// A_MATRIC_MIGR X(10)  
            /// </summary>
            [HisFieldInfoMapping(0, 10)]
            public string A_MATRIC_MIGR { get; set; }

            /// <summary>
            /// A_RESEST X  
            /// </summary>
            [HisFieldInfoMapping(1, 1)]
            public string A_RESEST { get; set; }

            /// <summary>
            /// A_RIGA75 X(75)  
            /// </summary>
            [HisFieldInfoMapping(2, 75)]
            public string A_RIGA75 { get; set; }

            /// <summary>
            /// A_STAZ 99  
            /// </summary>
            [HisFieldInfoMapping(3, 2)]
            public short A_STAZ { get; set; }

            /// <summary>
            /// A_TIP_IST X  
            /// </summary>
            [HisFieldInfoMapping(4, 1)]
            public string A_TIP_IST { get; set; }

            /// <summary>
            /// A_CODFIS X(16)  
            /// </summary>
            [HisFieldInfoMapping(5, 16)]
            public string A_CODFIS { get; set; }

            /// <summary>
            /// A_UNIPROC X(3)  
            /// </summary>
            [HisFieldInfoMapping(6, 3)]
            public string A_UNIPROC { get; set; }

            /// <summary>
            /// A_MAIL X(50)  
            /// </summary>
            [HisFieldInfoMapping(7, 50)]
            public string A_MAIL { get; set; }

            /// <summary>
            /// A_COGNOME_INTERO X(36)  
            /// </summary>
            [HisFieldInfoMapping(8, 36)]
            public string A_COGNOME_INTERO { get; set; }

            /// <summary>
            /// A_NOME_INTERO X(36)  
            /// </summary>
            [HisFieldInfoMapping(9, 36)]
            public string A_NOME_INTERO { get; set; }

            /// <summary>
            /// A_COGNOACQ_INTERO X(36)  
            /// </summary>
            [HisFieldInfoMapping(10, 36)]
            public string A_COGNOACQ_INTERO { get; set; }

            //10 A-ALTRI-DATI-STATI-OCC   OCCURS 4.
            [HisComplexAreaInfoMapping(11, ListCount = 4)]
            public List<AltriDatiStatiEsteri> ALTRIDATI { get; set; }

            /// <summary>
            /// A_DATACANC 9(8)  
            /// </summary>
            [HisFieldInfoMapping(12, 8)]
            public int A_DATACANC { get; set; }

            /// <summary>
            /// FILLER X(59)  
            /// </summary>
            [HisFieldInfoMapping(13, 59)]
            public string FILLER { get; set; }
            #endregion Tracciato Host
        }

        public class AltriDatiStatiEsteri
        {
            #region Constructor
            internal AltriDatiStatiEsteri()
            {
            }
            #endregion Constructor

            #region tracciato COBOL
            //         15 A-CI14              PIC X.
            //         15 A-TIPC              PIC XX.
            //         15 A-MODEL             PIC X(7).
            #endregion tracciato COBOL

            #region tracciato Host
            /// <summary>
            /// A_CI14 X  
            /// </summary>
            [HisFieldInfoMapping(0, 1)]
            public string A_CI14 { get; set; }

            /// <summary>
            /// A_TIPC XX  
            /// </summary>
            [HisFieldInfoMapping(1, 2)]
            public string A_TIPC { get; set; }

            /// <summary>
            /// A_MODEL X(7)  
            /// </summary>
            [HisFieldInfoMapping(2, 7)]
            public string A_MODEL { get; set; }
            #endregion Tracciato Host
        }
        #endregion nested class
    }
}
