using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

using INPS.Pensioni.LiquidazioneAgo.Data.HostRequest;
using System.Runtime.Serialization;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostResponse
{
    [Serializable] 
    public class GAPL_GARCResponse
    {
        #region Constructor
        public GAPL_GARCResponse()
        {
            this.Controllo = new AreaControllo();
            this.Stampa = new AreaStampa();
        }
        #endregion Constructor

        #region Properties

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0)]
        public AreaControllo Controllo { get; internal set; }

        [HisComplexAreaInfoMapping(1)]
        public AreaStampa Stampa { get; internal set; }
        #endregion Tracciato Host

        #region nested class
        public class AreaStampa
        {
            #region Constructor
            internal AreaStampa()
            {
                this.Intestazione = new Data.HostResponse.AreaStampa.Intestazione();
                this.Anagrafica = new Data.HostResponse.AreaStampa.Anagrafica();
                this.DanteCausa = new Data.HostResponse.AreaStampa.DanteCausa();
                this.Pagamento = new Data.HostResponse.AreaStampa.Pagamento();
                this.Patronato_Sindacato = new Data.HostResponse.AreaStampa.Patronato_Sindacato();
                this.Decorrenza_Cessazione = new Data.HostResponse.AreaStampa.Decorrenza_Cessazione();
                this.Codici_Pensione = new Data.HostResponse.AreaStampa.Codici_Pensione();
                this.Conguagli_Arretrati = new Data.HostResponse.AreaStampa.Conguagli_Arretrati();
                this.Importi = new Data.HostResponse.AreaStampa.Importi();
                this.Addizionali_IRPEF = new Data.HostResponse.AreaStampa.Addizionali_IRPEF();
                this.Codici_Procedura = new Data.HostResponse.AreaStampa.Codici_Procedura();
                this.Pensioni_Fiscali = new Data.HostResponse.AreaStampa.Pensioni_Fiscali();
                this.Pensioni_Cumulate = new Data.HostResponse.AreaStampa.Pensioni_Cumulate();
                this.Sentenze_240_495 = new Data.HostResponse.AreaStampa.Sentenze_240_495();
                this.Dati_Retributivi = new Data.HostResponse.AreaStampa.Dati_Retributivi();
                this.Supplementi_Retributivi = new Data.HostResponse.AreaStampa.Supplementi_Retributivi();
                this.Dati_Contributivi_Old = new Data.HostResponse.AreaStampa.Dati_Contributivi_Old();
                this.Dati_Contributivi_New = new Data.HostResponse.AreaStampa.Dati_Contributivi_New();
                this.Dati_Contributivi_New1 = new Data.HostResponse.AreaStampa.Dati_Contributivi_New1();
                this.Fondo_Spedizionieri = new Data.HostResponse.AreaStampa.Fondo_Spedizionieri();
                this.Interessi_Legali = new Data.HostResponse.AreaStampa.Interessi_Legali();
                this.Conguagli = new Data.HostResponse.AreaStampa.Conguagli();
                this.Recupero_Crediti = new Data.HostResponse.AreaStampa.Recupero_Crediti();
                this.Maternita = new Data.HostResponse.AreaStampa.Maternita();
                this.ACNE_CENGIO = new Data.HostResponse.AreaStampa.ACNE_CENGIO();
                this.Contributi = new Data.HostResponse.AreaStampa.Contributi();
                this.Familiari_Carico = new Data.HostResponse.AreaStampa.Familiari_Carico();
                this.IRPEF = new Data.HostResponse.AreaStampa.IRPEF();
                this.Decorrenze = new Data.HostResponse.AreaStampa.Decorrenze();
                this.Trattenute = new Data.HostResponse.AreaStampa.Trattenute();
                this.INPDAI = new Data.HostResponse.AreaStampa.INPDAI();
                this.Maggiorazione_Sociale = new Data.HostResponse.AreaStampa.Maggiorazione_Sociale();
                this.Trattenute_Virtuali = new Data.HostResponse.AreaStampa.Trattenute_Virtuali();
                this.Decorrenze_FS = new Data.HostResponse.AreaStampa.Decorrenze_FS();
                this.Quote = new Data.HostResponse.AreaStampa.Quote();
                this.Coda = new Data.HostResponse.AreaStampa.Coda();
            }
            #endregion Constructor

            #region Tracciato Host

            [HisComplexAreaInfoMapping(0)]
            public Data.HostResponse.AreaStampa.Intestazione Intestazione { get; internal set; }

            [HisComplexAreaInfoMapping(1)]
            public Data.HostResponse.AreaStampa.Anagrafica Anagrafica { get; internal set; }

            [HisComplexAreaInfoMapping(2)]
            public Data.HostResponse.AreaStampa.DanteCausa DanteCausa { get; internal set; }

            [HisComplexAreaInfoMapping(3)]
            public Data.HostResponse.AreaStampa.Pagamento Pagamento { get; internal set; }

            [HisComplexAreaInfoMapping(4)]
            public Data.HostResponse.AreaStampa.Patronato_Sindacato Patronato_Sindacato { get; internal set; }

            [HisComplexAreaInfoMapping(5)]
            public Data.HostResponse.AreaStampa.Decorrenza_Cessazione Decorrenza_Cessazione { get; internal set; }

            [HisComplexAreaInfoMapping(6)]
            public Data.HostResponse.AreaStampa.Codici_Pensione Codici_Pensione { get; internal set; }

            [HisComplexAreaInfoMapping(7)]
            public Data.HostResponse.AreaStampa.Conguagli_Arretrati Conguagli_Arretrati { get; internal set; }

            [HisComplexAreaInfoMapping(8)]
            public Data.HostResponse.AreaStampa.Importi Importi { get; internal set; }

            [HisComplexAreaInfoMapping(9)]
            public Data.HostResponse.AreaStampa.Addizionali_IRPEF Addizionali_IRPEF { get; internal set; }

            [HisComplexAreaInfoMapping(10)]
            public Data.HostResponse.AreaStampa.Codici_Procedura Codici_Procedura { get; internal set; }

            [HisComplexAreaInfoMapping(11)]
            public Data.HostResponse.AreaStampa.Pensioni_Fiscali Pensioni_Fiscali { get; internal set; }

            [HisComplexAreaInfoMapping(12)]
            public Data.HostResponse.AreaStampa.Pensioni_Cumulate Pensioni_Cumulate { get; internal set; }

            [HisComplexAreaInfoMapping(13)]
            public Data.HostResponse.AreaStampa.Sentenze_240_495 Sentenze_240_495 { get; internal set; }

            [HisComplexAreaInfoMapping(14)]
            public Data.HostResponse.AreaStampa.Dati_Retributivi Dati_Retributivi { get; internal set; }

            [HisComplexAreaInfoMapping(15)]
            public Data.HostResponse.AreaStampa.Supplementi_Retributivi Supplementi_Retributivi { get; internal set; }

            [HisComplexAreaInfoMapping(16)]
            public Data.HostResponse.AreaStampa.Dati_Contributivi_Old Dati_Contributivi_Old { get; internal set; }

            [HisComplexAreaInfoMapping(17)]
            public Data.HostResponse.AreaStampa.Dati_Contributivi_New Dati_Contributivi_New { get; internal set; }

            [HisComplexAreaInfoMapping(18)]
            public Data.HostResponse.AreaStampa.Dati_Contributivi_New1 Dati_Contributivi_New1 { get; internal set; }

            [HisComplexAreaInfoMapping(19)]
            public Data.HostResponse.AreaStampa.Fondo_Spedizionieri Fondo_Spedizionieri { get; internal set; }

            [HisComplexAreaInfoMapping(20)]
            public Data.HostResponse.AreaStampa.Interessi_Legali Interessi_Legali { get; internal set; }

            [HisComplexAreaInfoMapping(21)]
            public Data.HostResponse.AreaStampa.Conguagli Conguagli { get; internal set; }

            [HisComplexAreaInfoMapping(22)]
            public Data.HostResponse.AreaStampa.Recupero_Crediti Recupero_Crediti { get; internal set; }

            [HisComplexAreaInfoMapping(23)]
            public Data.HostResponse.AreaStampa.Maternita Maternita { get; internal set; }

            [HisComplexAreaInfoMapping(24)]
            public Data.HostResponse.AreaStampa.ACNE_CENGIO ACNE_CENGIO { get; internal set; }

            [HisComplexAreaInfoMapping(25)]
            public Data.HostResponse.AreaStampa.Contributi Contributi { get; internal set; }

            [HisComplexAreaInfoMapping(26)]
            public Data.HostResponse.AreaStampa.Familiari_Carico Familiari_Carico { get; internal set; }

            [HisComplexAreaInfoMapping(27)]
            public Data.HostResponse.AreaStampa.IRPEF IRPEF { get; internal set; }

            [HisComplexAreaInfoMapping(28)]
            public Data.HostResponse.AreaStampa.Decorrenze Decorrenze { get; internal set; }

            [HisComplexAreaInfoMapping(29)]
            public Data.HostResponse.AreaStampa.Trattenute Trattenute { get; internal set; }

            [HisComplexAreaInfoMapping(30)]
            public Data.HostResponse.AreaStampa.INPDAI INPDAI { get; internal set; }

            [HisComplexAreaInfoMapping(31)]
            public Data.HostResponse.AreaStampa.Maggiorazione_Sociale Maggiorazione_Sociale { get; internal set; }

            [HisComplexAreaInfoMapping(32)]
            public Data.HostResponse.AreaStampa.Trattenute_Virtuali Trattenute_Virtuali { get; internal set; }

            [HisComplexAreaInfoMapping(33)]
            public Data.HostResponse.AreaStampa.Decorrenze_FS Decorrenze_FS { get; internal set; }

            [HisComplexAreaInfoMapping(34)]
            public Data.HostResponse.AreaStampa.Quote Quote { get; internal set; }

            [HisComplexAreaInfoMapping(35)]
            public Data.HostResponse.AreaStampa.Coda Coda { get; internal set; }

            #endregion Tracciato Host
        }
        public class AreaControllo
        {
            #region Constructor
            internal AreaControllo()
            { }
            #endregion Constructor
            #region Properties

            #region Tracciato COBOL
            //             01  FORM-RICEZIONE.
            //     02 PER-TESTATA.
            //* Tipo Procedura PL = Prime Liquidate - RC = Ricostituzioni
            //         03 PER-PROCEDURA               PIC XX.
            //* Numero della domanda EAD75
            //         03 FILLER                      PIC X(8).
            //* Categoria
            //         03 PER-CATEGORIA               PIC X(6).
            //* Certificato
            //         03 PER-CERTIFICATO             PIC 9(8).
            //* Codice di ritorno e tecnici
            //* da 0 a 9 = 0 è Ok
            //         03 PER-CODTECNICI              PIC 9.
            //* Codice esito (errata/esatta/esatta in prova etc.)
            //*               A = Esatta
            //*               E = Esatta con errori
            //*               P = Esatta in prova
            //*               S = Errata
            //         03 PER-CODESITO                PIC X.
            //* Codice esito per la domanda EAD75
            //         03 PER-CODESITOEAD75           PIC X.
            //* Tabella errori
            //         03 PER-TABERR.
            //            04 PER-ERR  OCCURS 10       PIC 9(3).
            //         03 PER-ERRORI-QRED.
            //            04 PER-TABERR-QRED OCCURS 3.
            //               05 PER-KEYERR-QRED       PIC 9(15).
            //               05 PER-ANNOERR-QRED      PIC 9(4).
            //               05 PER-TIPOERR-QRED      PIC X.
            //* 
            //         03 PER-DOMANDA                 PIC 9(13).
            //         03 PER-WEBDOM.
            //            04 WEB-COD                  PIC X(2).
            //            04 WEB-ERR                  PIC X(50).

            //         03 DATA-CALCOLO.
            //            04 DATA-CALCA               PIC 9(4).
            //            04 DATA-CALCM               PIC 9(2).
            //            04 DATA-CALCG               PIC 9(2).
            //*
            //*LOMAR 23/09/2009 - I
            //*
            //*        03 FILLER                      PIC X(50).
            //         03 GP6-MANCANTE.
            //            04 ANNO-GP6-MANCANTE        PIC 9(4).              
            //            04 MESE-GP6-MANCANTE        PIC 9(2).
            //         03 FILLER                      PIC X(44).
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 01  FORM-RICEZIONE.
            // 02 PER-TESTATA.
            // * Tipo Procedura PL = Prime Liquidate - RC = Ricostituzioni
            /// <summary>
            /// PER_PROCEDURA XX  
            /// </summary>
            [HisFieldInfoMapping(0, 2)]
            public string PER_PROCEDURA { get; set; }

            // * Numero della domanda EAD75
            /// <summary>
            /// FILLER X(8)  
            /// </summary>
            [HisFieldInfoMapping(1, 8)]
            public string FILLER1 { get; set; }

            // * Categoria
            /// <summary>
            /// PER_CATEGORIA X(6)  
            /// </summary>
            [HisFieldInfoMapping(2, 6)]
            public string PER_CATEGORIA { get; set; }

            // * Certificato
            /// <summary>
            /// PER_CERTIFICATO 9(8)  
            /// </summary>
            [HisFieldInfoMapping(3, 8, CobolType = CobolType.Unsigned)]
            public int PER_CERTIFICATO { get; set; }

            // * Codice di ritorno e tecnici
            // * da 0 a 9 = 0 è Ok
            /// <summary>
            /// PER_CODTECNICI 9  
            /// </summary>
            [HisFieldInfoMapping(4, 1, CobolType = CobolType.Unsigned)]
            public short PER_CODTECNICI { get; set; }

            // * Codice esito (errata/esatta/esatta in prova etc.)
            // *               A = Esatta
            // *               E = Esatta con errori
            // *               P = Esatta in prova
            // *               S = Errata
            /// <summary>
            /// PER_CODESITO X  
            /// </summary>
            [HisFieldInfoMapping(5, 1)]
            public string PER_CODESITO { get; set; }

            // * Codice esito per la domanda EAD75
            /// <summary>
            /// PER_CODESITOEAD75 X  
            /// </summary>
            [HisFieldInfoMapping(6, 1)]
            public string PER_CODESITOEAD75 { get; set; }

            [HisComplexAreaInfoMapping(7, ListCount = 10)]
            public List<PER_TABERR> LISTPER_TABERR { get; set; }

            [HisComplexAreaInfoMapping(8, ListCount = 3)]
            public List<PER_TABERR_QRED> LISTPER_TABERR_QRED { get; set; }

            /// <summary>
            /// PER_DOMANDA 9(13)  
            /// </summary>
            [HisFieldInfoMapping(9, 13, CobolType = CobolType.Unsigned)]
            public long PER_DOMANDA { get; set; }

            // 03 PER-WEBDOM.
            /// <summary>
            /// WEB_COD X(2)  
            /// </summary>
            [HisFieldInfoMapping(10, 2)]
            public string WEB_COD { get; set; }

            /// <summary>
            /// WEB_ERR X(50)  
            /// </summary>
            [HisFieldInfoMapping(11, 50)]
            public string WEB_ERR { get; set; }

            // 03 DATA-CALCOLO.
            /// <summary>
            /// DATA_CALCA 9(4)  
            /// </summary>
            [HisFieldInfoMapping(12, 4, CobolType = CobolType.Unsigned)]
            public short DATA_CALCA { get; set; }

            /// <summary>
            /// DATA_CALCM 9(2)  
            /// </summary>
            [HisFieldInfoMapping(13, 2, CobolType = CobolType.Unsigned)]
            public short DATA_CALCM { get; set; }

            /// <summary>
            /// DATA_CALCG 9(2)  
            /// </summary>
            [HisFieldInfoMapping(14, 2, CobolType = CobolType.Unsigned)]
            public short DATA_CALCG { get; set; }

            // 03 GP6-MANCANTE.
            /// <summary>
            /// ANNO_GP6_MANCANTE 9(4)  
            /// </summary>
            [HisFieldInfoMapping(15, 4, CobolType = CobolType.Unsigned)]
            public short ANNO_GP6_MANCANTE { get; set; }

            /// <summary>
            /// MESE_GP6_MANCANTE 9(2)  
            /// </summary>
            [HisFieldInfoMapping(16, 2, CobolType = CobolType.Unsigned)]
            public short MESE_GP6_MANCANTE { get; set; }

            [HisFieldInfoMapping(17, 1)]
            public string FLAG_INDEB { get; set; }
            
            /// <summary>
            /// FILLER X(44)  
            /// </summary>
            [HisFieldInfoMapping(18, 43)]
            public string FILLER2 { get; set; }
            #endregion Tracciato Host

            #region nested class
            public class PER_TABERR
            {
                #region Constructor
                internal PER_TABERR()
                { }
                #endregion Constructor
                #region Properties

                #region Tracciato COBOL
                //             03 PER-TABERR.
                //04 PER-ERR  OCCURS 10       PIC 9(3)
                #endregion Tracciato COBOL
                #region Tracciato Host
                // 03 PER-TABERR.
                /// <summary>
                /// PER_ERR 9(3)  
                /// </summary>
                [HisFieldInfoMapping(0, 3, CobolType = CobolType.Unsigned)]
                public short PER_ERR { get; set; }
                #endregion Tracciato Host
                #endregion Properties
            }

            public class PER_TABERR_QRED
            {
                #region Constructor
                internal PER_TABERR_QRED()
                { }
                #endregion Constructor
                #region Properties

                #region Tracciato COBOL
                //         03 PER-ERRORI-QRED.
                //04 PER-TABERR-QRED OCCURS 3.
                //   05 PER-KEYERR-QRED       PIC 9(15).
                //   05 PER-ANNOERR-QRED      PIC 9(4).
                //   05 PER-TIPOERR-QRED      PIC X.
                #endregion Tracciato COBOL
                #region Tracciato Host
                // 03 PER-ERRORI-QRED.
                // 04 PER-TABERR-QRED OCCURS 3.
                /// <summary>
                /// PER_KEYERR_QRED 9(15)  
                /// </summary>
                [HisFieldInfoMapping(0, 15, CobolType = CobolType.Unsigned)]
                public long PER_KEYERR_QRED { get; set; }

                /// <summary>
                /// PER_ANNOERR_QRED 9(4)  
                /// </summary>
                [HisFieldInfoMapping(1, 4, CobolType = CobolType.Unsigned)]
                public short PER_ANNOERR_QRED { get; set; }

                /// <summary>
                /// PER_TIPOERR_QRED X  
                /// </summary>
                [HisFieldInfoMapping(2, 1)]
                public string PER_TIPOERR_QRED { get; set; }
                #endregion Tracciato Host
                #endregion Properties
            }
            #endregion nested class

            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}

