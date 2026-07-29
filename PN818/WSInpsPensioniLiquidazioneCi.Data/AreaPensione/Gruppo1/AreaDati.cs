using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaDati
    {
        #region tracciato COBOL
        //              * 1996 SPAZIO CREATO DALLO SPOSTAMENTO DI INAIL E ASS.ACCOMPAGN.
        //      * 1996 SOSTITUITO CON CAMPI VARI DAL 1996
        //           04  DATI1.
        //      *1999    CAMPI NUOVI AL POSTO DI FIL PIC X(57) DEL 1998
        //--         05  TP1CONTO                  PIC X(12).
        //      *NUMERO CONTO CORRENTE
        //--         05  TP1ABI                    PIC 9(5).
        //      *CODICE ABI
        //--         05  TP1CABOLD                 PIC 9(6).
        //      *COORDINATE BANCARIE
        //           05  TP1REQRID                 PIC 9(1).
        //      *REQUISITO RIDOTTO
        //           05  TP1CONTRATTO              PIC 9(4).
        //      *CODICE CONTRATTO
        //           05  TP1LIVELLO                PIC 9(4).
        //      *CODICE LIVELLO
        //           05  TP1MOBILITA               PIC 9(1).
        //      *CODICE MOBILITA'
        //           05  TP1USURA                  PIC 9(1).
        //      *ATTIVITA' USURANTE
        //           05  TP1MODPAG                 PIC X(1).
        //      *MODALITA' DI PAGAMENTO A/C/L/S
        //--           10  TP1LIRE-EURO              PIC X(1).
        //      *MODALITA' DI PAGAMENTO IN LIRE O IN EURO L/E
        //--           10  TP1SEDEUP                 PIC X(4).
        //      *CODICE SEDE DELL'UFFICIO PAGATORE
        //--           10 TP1CIN                    PIC X(1).
        //--           10 TP1COSTA                  PIC X(1).
        //      *
        //               10  TP1NDOM-SIAL-S         PIC 9(4).
        //               10  TP1NDOM-SIAL-G         PIC 9(4).
        //               10  TP1NDOM-SIAL-P         PIC 9(5).
        //      * NUMERO SIAL2000 DELLA DOMANDA
        //           05  TP1VARTIT                 PIC X.
        //      *FLAG VARIAZIONE TITOLARE = S
        //           05  NUMFAMIL                  PIC 9.
        //      *NUMERO FAMILIARI PERVENUTI DAL CENTRO
        //           05  ICINOESTERO          PIC XX.
        //      * ANNI DI INTEGRABILITA' FRA 60 E 65 ANNI SENZA QUOTA ESTERA
        //           05  TP1TRSEAP            PIC S9(7)V9(4) COMP-3.
        //      *EURO TRATTENUTE DEDUCIBILI IRPEF ANNI PRECEDENTI
        //           05  TP1CODELIM           PIC X.
        //      *1997 CODICE ELIMINAZIONE DI TP1ELIM
        //           05  RICO-SI-ELIM         PIC X(2).
        //      *1997 RICOSTITUZIONE SU PENSIONE ELIMINATA DEVE ESSERE 'SI'
        //           05  DAT4218              PIC X(3).
        //      *CAMPI PER COSTA.
        //           05  IW1CODOPZ           PIC 9.
        //      *+1996  COD.TIPO OPZIONE
        //      *+1996  DATA DOMANDA DELLA OPZIONE.
        //               10  IW1OPZAN        PIC 9999.
        //               10  IW1OPZMM        PIC 99.
        //               10  IW1OPZGG        PIC 99.
        #endregion tracciato COBOL

        #region Tracciato Host
        // * 1996 SPAZIO CREATO DALLO SPOSTAMENTO DI INAIL E ASS.ACCOMPAGN.
        // * 1996 SOSTITUITO CON CAMPI VARI DAL 1996
        // 04  DATI1.
        // *1999    CAMPI NUOVI AL POSTO DI FIL PIC X(57) DEL 1998
        /// <summary>
        /// TP1CONTO X(12)  
        /// *NUMERO CONTO CORRENTE
        /// </summary>
        [HisFieldInfoMapping(0, 12)]
        public string TP1CONTO { get; set; }

        /// <summary>
        /// TP1ABI 9(5)  
        /// *CODICE ABI
        /// </summary>
        [HisFieldInfoMapping(1, 5)]
        public int TP1ABI { get; set; }

        /// <summary>
        /// TP1CABOLD 9(6)  
        /// *COORDINATE BANCARIE
        /// </summary>
        [HisFieldInfoMapping(2, 6)]
        public int TP1CABOLD { get; set; }

        /// <summary>
        /// TP1REQRID 9(1)  
        /// *REQUISITO RIDOTTO
        /// </summary>
        [HisFieldInfoMapping(3, 1)]
        public short TP1REQRID { get; set; }

        /// <summary>
        /// TP1CONTRATTO 9(4)  
        /// *CODICE CONTRATTO
        /// </summary>
        [HisFieldInfoMapping(4, 4)]
        public short TP1CONTRATTO { get; set; }

        /// <summary>
        /// TP1LIVELLO 9(4) 
        /// *CODICE LIVELLO 
        /// </summary>
        [HisFieldInfoMapping(5, 4)]
        public short TP1LIVELLO { get; set; }

        /// <summary>
        /// TP1MOBILITA 9(1)  
        /// *CODICE MOBILITA'
        /// </summary>
        [HisFieldInfoMapping(6, 1)]
        public short TP1MOBILITA { get; set; }

        /// <summary>
        /// TP1USURA 9(1)  
        /// *ATTIVITA' USURANTE
        /// </summary>
        [HisFieldInfoMapping(7, 1)]
        public short TP1USURA { get; set; }

        /// <summary>
        /// TP1MODPAG X(1)  
        /// *MODALITA' DI PAGAMENTO A/C/L/S
        /// </summary>
        [HisFieldInfoMapping(8, 1)]
        public string TP1MODPAG { get; set; }

        /// <summary>
        /// TP1LIRE_EURO X(1)  
        /// *MODALITA' DI PAGAMENTO IN LIRE O IN EURO L/E
        /// </summary>
        [HisFieldInfoMapping(9, 1)]
        public string TP1LIRE_EURO { get; set; }

        /// <summary>
        /// TP1SEDEUP X(4)  
        /// *CODICE SEDE DELL'UFFICIO PAGATORE
        /// </summary>
        [HisFieldInfoMapping(10, 4)]
        public string TP1SEDEUP { get; set; }

        /// <summary>
        /// TP1CIN X(1)  
        /// </summary>
        [HisFieldInfoMapping(11, 1)]
        public string TP1CIN { get; set; }

        /// <summary>
        /// TP1COSTA X(1)  
        /// </summary>
        [HisFieldInfoMapping(12, 1)]
        public string TP1COSTA { get; set; }

        //*
        /// <summary>
        /// TP1NDOM_SIAL_S 9(4)  
        /// </summary>
        [HisFieldInfoMapping(13, 4)]
        public short TP1NDOM_SIAL_S { get; set; }

        /// <summary>
        /// TP1NDOM_SIAL_G 9(4)  
        /// </summary>
        [HisFieldInfoMapping(14, 4)]
        public short TP1NDOM_SIAL_G { get; set; }

        /// <summary>
        /// TP1NDOM_SIAL_P 9(5)  
        /// * NUMERO SIAL2000 DELLA DOMANDA
        /// </summary>
        [HisFieldInfoMapping(15, 5)]
        public int TP1NDOM_SIAL_P { get; set; }

        /// <summary>
        /// TP1VARTIT X  
        /// *FLAG VARIAZIONE TITOLARE = S
        /// </summary>
        [HisFieldInfoMapping(16, 1)]
        public string TP1VARTIT { get; set; }

        /// <summary>
        /// NUMFAMIL 9  
        /// *NUMERO FAMILIARI PERVENUTI DAL CENTRO
        /// </summary>
        [HisFieldInfoMapping(17, 1)]
        public short NUMFAMIL { get; set; }

        /// <summary>
        /// ICINOESTERO XX  
        /// * ANNI DI INTEGRABILITA' FRA 60 E 65 ANNI SENZA QUOTA ESTERA
        /// </summary>
        [HisFieldInfoMapping(18, 2)]
        public string ICINOESTERO { get; set; }

        /// <summary>
        /// TP1TRSEAP S9(7)V9(4) COMP-3 
        /// *EURO TRATTENUTE DEDUCIBILI IRPEF ANNI PRECEDENTI
        /// </summary>
        [HisFieldInfoMapping(19, 6, Scale = 4, CobolType=CobolType.Comp3)]
        public decimal TP1TRSEAP { get; set; }

        /// <summary>
        /// TP1CODELIM X  
        /// *1997 CODICE ELIMINAZIONE DI TP1ELIM
        /// </summary>
        [HisFieldInfoMapping(20, 1)]
        public string TP1CODELIM { get; set; }

        /// <summary>
        /// RICO_SI_ELIM X(2)  
        /// *1997 RICOSTITUZIONE SU PENSIONE ELIMINATA DEVE ESSERE 'SI'
        /// </summary>
        [HisFieldInfoMapping(21, 2)]
        public string RICO_SI_ELIM { get; set; }

        /// <summary>
        /// DAT4218 X(3)  NONE
        /// *CAMPI PER COSTA.
        /// </summary>
        [HisFieldInfoMapping(22, 3, CobolType = CobolType.Untraslate)]
        public int DAT4218 { get; set; }

        /// <summary>
        /// IW1CODOPZ 9  
        /// *+1996  COD.TIPO OPZIONE
        /// </summary>
        [HisFieldInfoMapping(23, 1)]
        public short IW1CODOPZ { get; set; }

        /// <summary>
        /// IW1OPZAN 9999  
        /// *+1996  DATA DOMANDA DELLA OPZIONE.
        /// </summary>
        [HisFieldInfoMapping(24, 4)]
        public short IW1OPZAN { get; set; }

        /// <summary>
        /// IW1OPZMM 99  
        /// *+1996  DATA DOMANDA DELLA OPZIONE.
        /// </summary>
        [HisFieldInfoMapping(25, 2)]
        public short IW1OPZMM { get; set; }

        /// <summary>
        /// IW1OPZGG 99  
        /// *+1996  DATA DOMANDA DELLA OPZIONE.
        /// </summary>
        [HisFieldInfoMapping(26, 2)]
        public short IW1OPZGG { get; set; }

        ///// <summary>
        ///// nel caso di valore null occorre far riferimento ai valori precedenti
        ///// </summary>
        //[HisFieldInfoMapping(27, 8, Offset= -8,DateFormat = "yyyyMMdd")]
        //public DateTime? IW1OPZ { get; set; }
        #endregion Tracciato Host
    }
}
