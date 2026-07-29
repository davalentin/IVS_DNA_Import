using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

using INPS.Pensioni.LiquidazioneFs.Data.HostRequest;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostResponse
{
    public class FSPL_FSRCResponseNew
    {
        #region Constructor
        public FSPL_FSRCResponseNew()
        {
            this.Dati = new AreaDati();
        }
        #endregion Constructor

        #region Properties
        [HisComplexAreaInfoMapping(0)]
        public AreaDati Dati { get; set; }
        #endregion Properties

        #region Nested class

        /// <summary>
        ///  Definizione del tracciato di output
        /// </summary>
        public class AreaDati
        {
            #region Constructor
            internal AreaDati()
            {
                this.Stampa = new AreaStampa();
            }
            #endregion Constructor

            #region tracciato COBOL
            //01 MSG-RIS.
            //02 COD-TRAN                   PIC X(8).
            //02 COD-RISP.
            //  03 RZ-TPRIC                 PIC X(3).
            //  03 RZ-SUBTI                 PIC X VALUE 'A'.
            //  03 RZ-NUMDO                 PIC 9(8).
            //  03 RZ-ESITO                 PIC 99.
            //  03 RZ-CODES                 PIC X(3) OCCURS 10 TIMES.
            //  03 RZ-DTVER                 PIC 9(6).
            //02 SEZ-MSGE                   PIC X(100).
            //02 SEZ-STAM                   PIC X(18000). CEMRICPL
            //02 FILLER                     PIC X(442).
            #endregion tracciato COBOL

            #region Tracciato Host
            // 01 MSG-RIS.
            /// <summary>
            /// COD_TRAN X(8)  
            /// </summary>
            [HisFieldInfoMapping(0, 8)]
            public string COD_TRAN { get; set; }

            // 02 COD-RISP.
            /// <summary>
            /// RZ_TPRIC X(3)  
            /// </summary>
            [HisFieldInfoMapping(1, 3)]
            public string RZ_TPRIC { get; set; }

            /// <summary>
            /// RZ_SUBTI X  
            /// </summary>
            [HisFieldInfoMapping(2, 1)]
            public string RZ_SUBTI { get; set; }

            /// <summary>
            /// RZ_NUMDO 9(8)  
            /// </summary>
            [HisFieldInfoMapping(3, 8, CobolType = CobolType.Unsigned)]
            public int RZ_NUMDO { get; set; }

            /// <summary>
            /// RZ_ESITO 99  
            /// </summary>
            [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
            public short RZ_ESITO { get; set; }

            /// <summary>
            /// 03 RZ-CODES   PIC X(3) OCCURS 10 TIMES. 
            /// </summary>
            [HisComplexAreaInfoMapping(5, ListCount = 10)]
            public List<Codice> LISTCodice { get; set; }

            // <summary>
            /// RZ_DTVER 9(6)  
            /// </summary>
            [HisFieldInfoMapping(6, 6, CobolType = CobolType.Unsigned)]
            public int RZ_DTVER { get; set; }

            /// <summary>
            /// SEZ_MSGE X(100)  
            /// </summary>
            [HisFieldInfoMapping(7, 100)]
            public string SEZ_MSGE { get; set; }

            [HisComplexAreaInfoMapping(8)]
            public AreaStampa Stampa { get; set; }

            /// <summary>
            /// FILLER X(442)  
            /// </summary>
            [HisFieldInfoMapping(9, 442)]
            public string FILLER { get; set; }
            #endregion Tracciato Host

            #region Nested class
            public class Codice
            {
                #region Constructor
                internal Codice()
                {
                }
                #endregion Constructor

                #region tracciato COBOL
                //  03 RZ-CODES   PIC X(3) OCCURS 10 TIMES.
                #endregion tracciato COBOL

                #region Tracciato Host
                /// <summary>
                /// 03 RZ-CODES  X(3)  
                /// </summary>
                [HisFieldInfoMapping(0, 3)]
                public string RZ_CODES { get; set; }
                #endregion Tracciato Host
            }

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
                public Data.HostResponse.AreaStampa.Intestazione  Intestazione { get; set; }

                [HisComplexAreaInfoMapping(1)]
                public Data.HostResponse.AreaStampa.Anagrafica Anagrafica { get; set; }

                [HisComplexAreaInfoMapping(2)]
                public Data.HostResponse.AreaStampa.DanteCausa DanteCausa { get; set; }

                [HisComplexAreaInfoMapping(3)]
                public Data.HostResponse.AreaStampa.Pagamento Pagamento { get; set; }

                [HisComplexAreaInfoMapping(4)]
                public Data.HostResponse.AreaStampa.Patronato_Sindacato Patronato_Sindacato { get; set; }

                [HisComplexAreaInfoMapping(5)]
                public Data.HostResponse.AreaStampa.Decorrenza_Cessazione Decorrenza_Cessazione { get; set; }

                [HisComplexAreaInfoMapping(6)]
                public Data.HostResponse.AreaStampa.Codici_Pensione Codici_Pensione { get; set; }

                [HisComplexAreaInfoMapping(7)]
                public Data.HostResponse.AreaStampa.Conguagli_Arretrati Conguagli_Arretrati { get; set; }

                [HisComplexAreaInfoMapping(8)]
                public Data.HostResponse.AreaStampa.Importi Importi { get; set; }

                [HisComplexAreaInfoMapping(9)]
                public Data.HostResponse.AreaStampa.Addizionali_IRPEF Addizionali_IRPEF { get; set; }

                [HisComplexAreaInfoMapping(10)]
                public Data.HostResponse.AreaStampa.Codici_Procedura Codici_Procedura { get; set; }

                [HisComplexAreaInfoMapping(11)]
                public Data.HostResponse.AreaStampa.Pensioni_Fiscali Pensioni_Fiscali { get; set; }

                [HisComplexAreaInfoMapping(12)]
                public Data.HostResponse.AreaStampa.Pensioni_Cumulate Pensioni_Cumulate { get; set; }

                [HisComplexAreaInfoMapping(13)]
                public Data.HostResponse.AreaStampa.Sentenze_240_495 Sentenze_240_495 { get; set; }

                [HisComplexAreaInfoMapping(14)]
                public Data.HostResponse.AreaStampa.Dati_Retributivi Dati_Retributivi { get; set; }

                [HisComplexAreaInfoMapping(15)]
                public Data.HostResponse.AreaStampa.Supplementi_Retributivi Supplementi_Retributivi { get; set; }

                [HisComplexAreaInfoMapping(16)]
                public Data.HostResponse.AreaStampa.Dati_Contributivi_Old Dati_Contributivi_Old { get; set; }

                [HisComplexAreaInfoMapping(17)]
                public Data.HostResponse.AreaStampa.Dati_Contributivi_New Dati_Contributivi_New { get; set; }

                [HisComplexAreaInfoMapping(18)]
                public Data.HostResponse.AreaStampa.Dati_Contributivi_New1 Dati_Contributivi_New1 { get; set; }

                [HisComplexAreaInfoMapping(19)]
                public Data.HostResponse.AreaStampa.Fondo_Spedizionieri Fondo_Spedizionieri { get; set; }

                [HisComplexAreaInfoMapping(20)]
                public Data.HostResponse.AreaStampa.Interessi_Legali Interessi_Legali { get; set; }

                [HisComplexAreaInfoMapping(21)]
                public Data.HostResponse.AreaStampa.Conguagli Conguagli { get; set; }

                [HisComplexAreaInfoMapping(22)]
                public Data.HostResponse.AreaStampa.Recupero_Crediti Recupero_Crediti { get; set; }

                [HisComplexAreaInfoMapping(23)]
                public Data.HostResponse.AreaStampa.Maternita Maternita { get; set; }

                [HisComplexAreaInfoMapping(24)]
                public Data.HostResponse.AreaStampa.ACNE_CENGIO ACNE_CENGIO { get; set; }

                [HisComplexAreaInfoMapping(25)]
                public Data.HostResponse.AreaStampa.Contributi Contributi { get; set; }

                [HisComplexAreaInfoMapping(26)]
                public Data.HostResponse.AreaStampa.Familiari_Carico Familiari_Carico { get; set; }

                [HisComplexAreaInfoMapping(27)]
                public Data.HostResponse.AreaStampa.IRPEF IRPEF { get; set; }

                [HisComplexAreaInfoMapping(28)]
                public Data.HostResponse.AreaStampa.Decorrenze Decorrenze { get; set; }

                [HisComplexAreaInfoMapping(29)]
                public Data.HostResponse.AreaStampa.Trattenute Trattenute { get; set; }

                [HisComplexAreaInfoMapping(30)]
                public Data.HostResponse.AreaStampa.INPDAI INPDAI { get; set; }

                [HisComplexAreaInfoMapping(31)]
                public Data.HostResponse.AreaStampa.Maggiorazione_Sociale Maggiorazione_Sociale { get; set; }

                [HisComplexAreaInfoMapping(32)]
                public Data.HostResponse.AreaStampa.Trattenute_Virtuali Trattenute_Virtuali { get; set; }

                [HisComplexAreaInfoMapping(33)]
                public Data.HostResponse.AreaStampa.Decorrenze_FS Decorrenze_FS { get; set; }

                [HisComplexAreaInfoMapping(34)]
                public Data.HostResponse.AreaStampa.Quote Quote { get; set; }

                [HisComplexAreaInfoMapping(35)]
                public Data.HostResponse.AreaStampa.Coda Coda { get; set; }

                #endregion Tracciato Host
            }
            #endregion Nested class
        }

        #endregion Nested class
    }
}
