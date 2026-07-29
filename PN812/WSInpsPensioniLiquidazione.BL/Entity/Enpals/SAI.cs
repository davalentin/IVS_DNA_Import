using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class SAI
    {
        #region Properties
        #region Tracciato COBOL
        // 01 GETSAI.
        //       02 GETSAI-DATI-INPUT.
        //            03 OBM-CM-TIPO-RIC         	    PIC X(6) value ‘GETSAI’.
        //            03 OBM-CM-COD-FISC         	    PIC X(16).
        //            03 OBM-CM-NUM-DOM-INPS         	    PIC X(13).
        //       02 GETSAI-DATI-OUTPUT.
        //            03 GETSAI-ESITO         	     	    PIC X(1).
        //            03 GETSAI-DES-ERRORE      	PIC X(80).
        //            03 GETSAI-DT-DECORRENZA              PIC XX/XX/XXXX.
        //            03 GETSAI-DT-RAG-REQ        	    PIC XX/XX/XXXX.
        //            03 GETSAI-ETA-MAT-DIR        	    PIC 9(4). (AAMM)
        //            03 GETSAI-ETA-MAT-MIS        	    PIC 9(4). (AAMM)
        //            03 GETSAI-QUAL-PREV		PIC X(3).  
        //            03 OBM-CM-DT-FINESTRA        	    PIC XX/XX/XXXX.
        //            03 GETSAI-NR-CTB-MIS		    PIC 9(4).
        //            03 GETSAI-NR-CTB-DIR		    PIC 9(4).
        //            03 GETSAI-TOT-CTB-QUAL		    PIC 9(4).
        //            03 GETSAI-TOT-CTB-QUAL-QNQ	PIC 9(4).
        //            03 GETSAI-TOT-CTB-QUAL-TRI	PIC 9(4).
        //            03 GETSAI-NR-CTB-NL222		    PIC 9(4).
        //            03 GETSAI-NR-CTB-NL155		    PIC 9(4).
        //            03 GETSAI-NR-CTB-NVV		    PIC 9(4).
        //            03 GETSAI-DT-PRI-CTB		    PIC XX/XX/XXXX.
        //            03 GETSAI-AAMM-TRA-DIR		    PIC 9(4). (AAMM)
        //            03 GETSAI-RAG-PREV	PIC X(1).  
        //            03 GETSAI-GRU-PREV	PIC X(1).  
        //            03 GETSAI-GRU-DIR	PIC X(1).  
        //            03 GETSAI-NR-TOT-CTB		    PIC 9(4).
        //            03 GETSAI-NR-TOT-CTB-OBG	PIC 9(4).
        //            03 GETSAI-NR-CTB-ANTE		    PIC 9(4).
        //            03 GETSAI-NR-CTB-QUOA		    PIC 9(4).
        //            03 GETSAI-RTB-MED-540		    PIC 9(9)V9(3).
        //            03 GETSAI-IMP-QUA        	PIC 9(9)V9(3).
        //            03 GETSAI-NR-CTB-POST		    PIC 9(4).
        //            03 GETSAI-NR-CTB-QUOB		    PIC 9(4).
        //            03 GETSAI-RTB-MED-POST		    PIC 9(9)V9(3).
        //            03 GETSAI-IMP-QUB        	PIC 9(9)V9(3).
        //            03 OBM-CM-IMP-RTV	        	    PIC 9(9)V9(3).
        //            03 GETSAI-IMP-CONTR        	    PIC 9(9)V9(3).	
        //            03 GETSAI-IMP-PRT	PIC 9(9)V9(3).
        //            03 GETSAI-MONT-CMP	PIC 9(12)V9(3).
        //            03 GETSAI-COEFF-TRASF		    PIC 9(4)V9(6).
        //            03 GETSAI-IND-IBT	PIC X.
        //            03 GETSAI-DES-DEROGA1		    PIC X(40).
        //            03 GETSAI-DES-DEROGA2		    PIC X(40).
        //            03 GETSAI-DES-DEROGA3		    PIC X(40).
        //            03 GETSAI-DES-DEROGA4		    PIC X(40).
        //            03 GETSAI_NUM_CTB_ENP_ANTE
        //            03 GETSAI_NUM_CTB_ENP_POST
        //            03 GETSAI_NUM_CTB_ENP_CONT
        //            03 GETSAI_NUM_CTB_FIG_ANTE
        //            03 GETSAI_NUM-CTB_FIG_POST
        //            03 GETSAI_NUM_CTB_FIG_CONT
        //            03 GETSAI_NUM_CTB_UFF_ANTE
        //            03 GETSAI_NUM_CTB_UFF_POST
        //            03 GETSAI_NUM_CTB_UFF_CONT
        //            03 GETSAI_NUM_CTB_INPS_ANTE
        //            03 GETSAI_NUM_CTB_INPS_POST
        //            03 GETSAI_NUM_CTB_INPS_CONT
        //            03 GETSAI_NUM_CTB_VV_ANTE
        //            03 GETSAI_NUM_CTB_VV_POST
        //            03 GETSAI_NUM_CTB_VV_CONT
        //            03 GETSAI_NUM_CTB_EST_ANTE
        //            03 GETSAI_NUM_CTB_EST_POST
        //            03 GETSAI_NUM_CTB_EST_CONT
        //            03 GETSAI-CTB-RETRIB OCCURS 35.
        //                   05 GETSAI-ELR-TIPO-CTB		    PIC X(1).
        //                   05 GETSAI-ELR-COD-CTB		    PIC X(2).
        //                   05 GETSAI-ELR-COMPETENZA	    PIC X(1).
        //                   05 GETSAI-ELR-DT-PRI-CTB	    PIC XX/XX/XXXX.
        //                   05 GETSAI-ELR-DT-UTL-CTB	    PIC XX/XX/XXXX.
        //                   05 GETSAI-ELR-NUM-TOT-CTB	    PIC 9(4).
        //                   05 GETSAI-ELR-NUM-TOT-CTB-QNQ	    PIC 9(4).
        //                   05 GETSAI-ELR-NUM-TOT-CTB-TRI	    PIC 9(4).
        //            03 GETSAI-CTB-CONTRIB OCCURS 55.
        //                   05 GETSAI-ELC-ANNO	PIC X(4).
        //                   05 GETSAI-ELC-COD-CTB		    PIC X(2).
        //                   05 GETSAI-ELC-NUM-TOT-CTB	    PIC 9(4).
        //                   05 GETSAI-ELC-IMP-RTB-ANNUA       PIC 9(9)V99.
        //                   05 GETSAI-ELC-COEFF-RIV		    PIC 9(4)V9(6).
        //                   05 GETSAI-ELC-MONT-IND  	    PIC 9(12)V9(3).
        #endregion Tracciato COBOL

        #region Tracciato Host
        //01 GETSAI.
        #region private properties
        private char _GETSAI_ESITO;
        private string _GETSAI_DES_ERRORE;
        private string _GETSAI_DT_DECORRENZA;
        private string _GETSAI_DT_RAG_REQ;
        private short _GETSAI_ETA_MAT_DIR;
        private short _GETSAI_ETA_MAT_MIS;
        private string _GETSAI_QUAL_PREV;
        private string _OBM_CM_DT_FINESTRA;
        private short _GETSAI_NR_CTB_MIS;
        private short _GETSAI_NR_CTB_DIR;
        private short _GETSAI_TOT_CTB_QUAL;
        private short _GETSAI_TOT_CTB_QUAL_QNQ;
        private short _GETSAI_TOT_CTB_QUAL_TRI;
        private short _GETSAI_NR_CTB_NL222;
        private short _GETSAI_NR_CTB_NL155;
        private short _GETSAI_NR_CTB_NVV;
        private string _GETSAI_DT_PRI_CTB;
        private short _GETSAI_AAMM_TRA_DIR;
        private char _GETSAI_RAG_PREV;
        private char _GETSAI_GRU_PREV;
        private char _GETSAI_GRU_DIR;
        private short _GETSAI_NR_TOT_CTB;
        private short _GETSAI_NR_TOT_CTB_OBG;
        private short _GETSAI_NR_CTB_ANTE;
        private short _GETSAI_NR_CTB_QUOA;
        private decimal _GETSAI_RTB_MED_540;
        private decimal _GETSAI_IMP_QUA;
        private short _GETSAI_NR_CTB_POST;
        private short _GETSAI_NR_CTB_QUOB;
        private decimal _GETSAI_RTB_MED_POST;
        private decimal _GETSAI_IMP_QUB;
        private decimal _OBM_CM_IMP_RTV;
        private decimal _GETSAI_IMP_CONTR;
        private decimal _GETSAI_IMP_PRT;
        private decimal _GETSAI_MONT_CMP;
        private decimal _GETSAI_COEFF_TRASF;
        private char _GETSAI_IND_IBT;
        private string _GETSAI_DES_DEROGA1;
        private string _GETSAI_DES_DEROGA2;
        private string _GETSAI_DES_DEROGA3;
        private string _GETSAI_DES_DEROGA4;
        private string _GETSAI_DT_FIN_ASS;
        private string _GETSAI_DT_FIN_SUP;
        private string _GETSAI_DT_INIS_SUP;
        private decimal _GETSAI_IMP_PENS;
        private decimal _GETSAI_IMP_SUP;
        private char _GETSAI_SISTEMA_CALCOLO;
        private char _GETSAI_TIP_LIQ;
        private int _GETSAI_NUM_CTB_ENP_ANTE;
        private int _GETSAI_NUM_CTB_ENP_POST;
        private int _GETSAI_NUM_CTB_ENP_CONT;
        private int _GETSAI_NUM_CTB_FIG_ANTE;
        private int _GETSAI_NUM_CTB_FIG_POST;
        private int _GETSAI_NUM_CTB_FIG_CONT;
        private int _GETSAI_NUM_CTB_UFF_ANTE;
        private int _GETSAI_NUM_CTB_UFF_POST;
        private int _GETSAI_NUM_CTB_UFF_CONT;
        private int _GETSAI_NUM_CTB_INPS_ANTE;
        private int _GETSAI_NUM_CTB_INPS_POST;
        private int _GETSAI_NUM_CTB_INPS_CONT;
        private int _GETSAI_NUM_CTB_VV_ANTE;
        private int _GETSAI_NUM_CTB_VV_POST;
        private int _GETSAI_NUM_CTB_VV_CONT;
        private int _GETSAI_NUM_CTB_EST_ANTE;
        private int _GETSAI_NUM_CTB_EST_POST;
        private int _GETSAI_NUM_CTB_EST_CONT;
        private List<DatiRetributiviSAI> _GETSAI_CTB_RETRIB;
        private List<DatiContributiviSAI> _GETSAI_CTB_CONTRIB;
        private string _GETSAI_COD_TIP_DOM;
        private string _GETSAI_TIP_PEN;
        private short _GETSAI_NR_CTB_POST_707;
        private decimal _GETSAI_IMP_QUA_707;
        private decimal _GETSAI_IMP_QUB_707;
        private decimal _GETSAI_IMP_PENS_707;
        private short _GETSAI_ANZ_CONTR;
        #endregion private properties

        #region public properties
        public char GETSAI_ESITO { get { return _GETSAI_ESITO; } set { _GETSAI_ESITO = value; } }
        public string GETSAI_DES_ERRORE { get { return _GETSAI_DES_ERRORE; } set { _GETSAI_DES_ERRORE = value; } }
        public string GETSAI_DT_DECORRENZA { get { return _GETSAI_DT_DECORRENZA; } set { _GETSAI_DT_DECORRENZA = value; } }
        public string GETSAI_DT_RAG_REQ { get { return _GETSAI_DT_RAG_REQ; } set { _GETSAI_DT_RAG_REQ = value; } }
        public short GETSAI_ETA_MAT_DIR { get { return _GETSAI_ETA_MAT_DIR; } set { _GETSAI_ETA_MAT_DIR = value; } }
        public short GETSAI_ETA_MAT_MIS { get { return _GETSAI_ETA_MAT_MIS; } set { _GETSAI_ETA_MAT_MIS = value; } }
        public string GETSAI_QUAL_PREV { get { return _GETSAI_QUAL_PREV; } set { _GETSAI_QUAL_PREV = value; } }
        public string OBM_CM_DT_FINESTRA { get { return _OBM_CM_DT_FINESTRA; } set { _OBM_CM_DT_FINESTRA = value; } }
        public short GETSAI_NR_CTB_MIS { get { return _GETSAI_NR_CTB_MIS; } set { _GETSAI_NR_CTB_MIS = value; } }
        public short GETSAI_NR_CTB_DIR { get { return _GETSAI_NR_CTB_DIR; } set { _GETSAI_NR_CTB_DIR = value; } }
        public short GETSAI_TOT_CTB_QUAL { get { return _GETSAI_TOT_CTB_QUAL; } set { _GETSAI_TOT_CTB_QUAL = value; } }
        public short GETSAI_TOT_CTB_QUAL_QNQ { get { return _GETSAI_TOT_CTB_QUAL_QNQ; } set { _GETSAI_TOT_CTB_QUAL_QNQ = value; } }
        public short GETSAI_TOT_CTB_QUAL_TRI { get { return _GETSAI_TOT_CTB_QUAL_TRI; } set { _GETSAI_TOT_CTB_QUAL_TRI = value; } }
        public short GETSAI_NR_CTB_NL222 { get { return _GETSAI_NR_CTB_NL222; } set { _GETSAI_NR_CTB_NL222 = value; } }
        public short GETSAI_NR_CTB_NL155 { get { return _GETSAI_NR_CTB_NL155; } set { _GETSAI_NR_CTB_NL155 = value; } }
        public short GETSAI_NR_CTB_NVV { get { return _GETSAI_NR_CTB_NVV; } set { _GETSAI_NR_CTB_NVV = value; } }
        public string GETSAI_DT_PRI_CTB { get { return _GETSAI_DT_PRI_CTB; } set { _GETSAI_DT_PRI_CTB = value; } }
        public short GETSAI_AAMM_TRA_DIR { get { return _GETSAI_AAMM_TRA_DIR; } set { _GETSAI_AAMM_TRA_DIR = value; } }
        public char GETSAI_RAG_PREV { get { return _GETSAI_RAG_PREV; } set { _GETSAI_RAG_PREV = value; } }
        public char GETSAI_GRU_PREV { get { return _GETSAI_GRU_PREV; } set { _GETSAI_GRU_PREV = value; } }
        public char GETSAI_GRU_DIR { get { return _GETSAI_GRU_DIR; } set { _GETSAI_GRU_DIR = value; } }
        public short GETSAI_NR_TOT_CTB { get { return _GETSAI_NR_TOT_CTB; } set { _GETSAI_NR_TOT_CTB = value; } }
        public short GETSAI_NR_TOT_CTB_OBG { get { return _GETSAI_NR_TOT_CTB_OBG; } set { _GETSAI_NR_TOT_CTB_OBG = value; } }
        public short GETSAI_NR_CTB_ANTE { get { return _GETSAI_NR_CTB_ANTE; } set { _GETSAI_NR_CTB_ANTE = value; } }
        public short GETSAI_NR_CTB_QUOA { get { return _GETSAI_NR_CTB_QUOA; } set { _GETSAI_NR_CTB_QUOA = value; } }
        public decimal GETSAI_RTB_MED_540 { get { return _GETSAI_RTB_MED_540; } set { _GETSAI_RTB_MED_540 = value; } }
        public decimal GETSAI_IMP_QUA { get { return _GETSAI_IMP_QUA; } set { _GETSAI_IMP_QUA = value; } }
        public short GETSAI_NR_CTB_POST { get { return _GETSAI_NR_CTB_POST; } set { _GETSAI_NR_CTB_POST = value; } }
        public short GETSAI_NR_CTB_QUOB { get { return _GETSAI_NR_CTB_QUOB; } set { _GETSAI_NR_CTB_QUOB = value; } }
        public decimal GETSAI_RTB_MED_POST { get { return _GETSAI_RTB_MED_POST; } set { _GETSAI_RTB_MED_POST = value; } }
        public decimal GETSAI_IMP_QUB { get { return _GETSAI_IMP_QUB; } set { _GETSAI_IMP_QUB = value; } }
        public decimal OBM_CM_IMP_RTV { get { return _OBM_CM_IMP_RTV; } set { _OBM_CM_IMP_RTV = value; } }
        public decimal GETSAI_IMP_CONTR { get { return _GETSAI_IMP_CONTR; } set { _GETSAI_IMP_CONTR = value; } }
        public decimal GETSAI_IMP_PRT { get { return _GETSAI_IMP_PRT; } set { _GETSAI_IMP_PRT = value; } }
        public decimal GETSAI_MONT_CMP { get { return _GETSAI_MONT_CMP; } set { _GETSAI_MONT_CMP = value; } }
        public decimal GETSAI_COEFF_TRASF { get { return _GETSAI_COEFF_TRASF; } set { _GETSAI_COEFF_TRASF = value; } }
        public char GETSAI_IND_IBT { get { return _GETSAI_IND_IBT; } set { _GETSAI_IND_IBT = value; } }
        public string GETSAI_DES_DEROGA1 { get { return _GETSAI_DES_DEROGA1; } set { _GETSAI_DES_DEROGA1 = value; } }
        public string GETSAI_DES_DEROGA2 { get { return _GETSAI_DES_DEROGA2; } set { _GETSAI_DES_DEROGA2 = value; } }
        public string GETSAI_DES_DEROGA3 { get { return _GETSAI_DES_DEROGA3; } set { _GETSAI_DES_DEROGA3 = value; } }
        public string GETSAI_DES_DEROGA4 { get { return _GETSAI_DES_DEROGA4; } set { _GETSAI_DES_DEROGA4 = value; } }
        public string GETSAI_DT_FIN_ASS { get { return _GETSAI_DT_FIN_ASS; } set { _GETSAI_DT_FIN_ASS = value; } }
        public string GETSAI_DT_FIN_SUP { get { return _GETSAI_DT_FIN_SUP; } set { _GETSAI_DT_FIN_SUP = value; } }
        public string GETSAI_DT_INIS_SUP { get { return _GETSAI_DT_INIS_SUP; } set { _GETSAI_DT_INIS_SUP = value; } }
        public decimal GETSAI_IMP_PENS { get { return _GETSAI_IMP_PENS; } set { _GETSAI_IMP_PENS = value; } }
        public decimal GETSAI_IMP_SUP { get { return _GETSAI_IMP_SUP; } set { _GETSAI_IMP_SUP = value; } }
        public char GETSAI_SISTEMA_CALCOLO { get { return _GETSAI_SISTEMA_CALCOLO; } set { _GETSAI_SISTEMA_CALCOLO = value; } }
        public char GETSAI_TIP_LIQ { get { return _GETSAI_TIP_LIQ; } set { _GETSAI_TIP_LIQ = value; } }
        public int GETSAI_NUM_CTB_ENP_ANTE { get { return _GETSAI_NUM_CTB_ENP_ANTE; } set { _GETSAI_NUM_CTB_ENP_ANTE = value; } }
        public int GETSAI_NUM_CTB_ENP_POST { get { return _GETSAI_NUM_CTB_ENP_POST; } set { _GETSAI_NUM_CTB_ENP_POST = value; } }
        public int GETSAI_NUM_CTB_ENP_CONT { get { return _GETSAI_NUM_CTB_ENP_CONT; } set { _GETSAI_NUM_CTB_ENP_CONT = value; } }
        public int GETSAI_NUM_CTB_FIG_ANTE { get { return _GETSAI_NUM_CTB_FIG_ANTE; } set { _GETSAI_NUM_CTB_FIG_ANTE = value; } }
        public int GETSAI_NUM_CTB_FIG_POST { get { return _GETSAI_NUM_CTB_FIG_POST; } set { _GETSAI_NUM_CTB_FIG_POST = value; } }
        public int GETSAI_NUM_CTB_FIG_CONT { get { return _GETSAI_NUM_CTB_FIG_CONT; } set { _GETSAI_NUM_CTB_FIG_CONT = value; } }
        public int GETSAI_NUM_CTB_UFF_ANTE { get { return _GETSAI_NUM_CTB_UFF_ANTE; } set { _GETSAI_NUM_CTB_UFF_ANTE = value; } }
        public int GETSAI_NUM_CTB_UFF_POST { get { return _GETSAI_NUM_CTB_UFF_POST; } set { _GETSAI_NUM_CTB_UFF_POST = value; } }
        public int GETSAI_NUM_CTB_UFF_CONT { get { return _GETSAI_NUM_CTB_UFF_CONT; } set { _GETSAI_NUM_CTB_UFF_CONT = value; } }
        public int GETSAI_NUM_CTB_INPS_ANTE { get { return _GETSAI_NUM_CTB_INPS_ANTE; } set { _GETSAI_NUM_CTB_INPS_ANTE = value; } }
        public int GETSAI_NUM_CTB_INPS_POST { get { return _GETSAI_NUM_CTB_INPS_POST; } set { _GETSAI_NUM_CTB_INPS_POST = value; } }
        public int GETSAI_NUM_CTB_INPS_CONT { get { return _GETSAI_NUM_CTB_INPS_CONT; } set { _GETSAI_NUM_CTB_INPS_CONT = value; } }
        public int GETSAI_NUM_CTB_VV_ANTE { get { return _GETSAI_NUM_CTB_VV_ANTE; } set { _GETSAI_NUM_CTB_VV_ANTE = value; } }
        public int GETSAI_NUM_CTB_VV_POST { get { return _GETSAI_NUM_CTB_VV_POST; } set { _GETSAI_NUM_CTB_VV_POST = value; } }
        public int GETSAI_NUM_CTB_VV_CONT { get { return _GETSAI_NUM_CTB_VV_CONT; } set { _GETSAI_NUM_CTB_VV_CONT = value; } }
        public int GETSAI_NUM_CTB_EST_ANTE { get { return _GETSAI_NUM_CTB_EST_ANTE; } set { _GETSAI_NUM_CTB_EST_ANTE = value; } }
        public int GETSAI_NUM_CTB_EST_POST { get { return _GETSAI_NUM_CTB_EST_POST; } set { _GETSAI_NUM_CTB_EST_POST = value; } }
        public int GETSAI_NUM_CTB_EST_CONT { get { return _GETSAI_NUM_CTB_EST_CONT; } set { _GETSAI_NUM_CTB_EST_CONT = value; } }
        public List<DatiRetributiviSAI> GETSAI_CTB_RETRIB { get { return _GETSAI_CTB_RETRIB; } set { _GETSAI_CTB_RETRIB = value; } }
        public List<DatiContributiviSAI> GETSAI_CTB_CONTRIB { get { return _GETSAI_CTB_CONTRIB; } set { _GETSAI_CTB_CONTRIB = value; } }
        public string GETSAI_COD_TIP_DOM { get { return _GETSAI_COD_TIP_DOM; } set { _GETSAI_COD_TIP_DOM = value; } }
        public string GETSAI_TIP_PEN { get { return _GETSAI_TIP_PEN; } set { _GETSAI_TIP_PEN = value; } }
        public short GETSAI_NR_CTB_POST_707 { get { return _GETSAI_NR_CTB_POST_707; } set { _GETSAI_NR_CTB_POST_707 = value; } }
        public decimal GETSAI_IMP_QUA_707 { get { return _GETSAI_IMP_QUA_707; } set { _GETSAI_IMP_QUA_707 = value; } }
        public decimal GETSAI_IMP_QUB_707 { get { return _GETSAI_IMP_QUB_707; } set { _GETSAI_IMP_QUB_707 = value; } }
        public decimal GETSAI_IMP_PENS_707 { get { return _GETSAI_IMP_PENS_707; } set { _GETSAI_IMP_PENS_707 = value; } }
        public short GETSAI_ANZ_CONTR { get { return _GETSAI_ANZ_CONTR; } set { _GETSAI_ANZ_CONTR = value; } }
        
        #endregion public properties

        #endregion Tracciato Host

        #region nested class
        //03 GETSAI-CTB-RETRIB OCCURS 35.
        public class DatiRetributiviSAI
        {
            #region private properties
            private char _GETSAI_ELR_TIPO_CTB;
            private string _GETSAI_ELR_COD_CTB;
            private string _GETSAI_ELR_COMPETENZA;
            private string _GETSAI_ELR_DT_PRI_CTB;
            private string _GETSAI_ELR_DT_UTL_CTB;
            private short _GETSAI_ELR_NUM_TOT_CTB;
            private short _GETSAI_ELR_NUM_TOT_CTB_QNQ;
            private short _GETSAI_ELR_NUM_TOT_CTB_TRI;
            #endregion private properties

            #region public properties
            public char GETSAI_ELR_TIPO_CTB { get { return _GETSAI_ELR_TIPO_CTB; } set { _GETSAI_ELR_TIPO_CTB = value; } }
            public string GETSAI_ELR_COD_CTB { get { return _GETSAI_ELR_COD_CTB; } set { _GETSAI_ELR_COD_CTB = value; } }
            public string GETSAI_ELR_COMPETENZA { get { return _GETSAI_ELR_COMPETENZA; } set { _GETSAI_ELR_COMPETENZA = value; } }
            public string GETSAI_ELR_DT_PRI_CTB { get { return _GETSAI_ELR_DT_PRI_CTB; } set { _GETSAI_ELR_DT_PRI_CTB = value; } }
            public string GETSAI_ELR_DT_UTL_CTB { get { return _GETSAI_ELR_DT_UTL_CTB; } set { _GETSAI_ELR_DT_UTL_CTB = value; } }
            public short GETSAI_ELR_NUM_TOT_CTB { get { return _GETSAI_ELR_NUM_TOT_CTB; } set { _GETSAI_ELR_NUM_TOT_CTB = value; } }
            public short GETSAI_ELR_NUM_TOT_CTB_QNQ { get { return _GETSAI_ELR_NUM_TOT_CTB_QNQ; } set { _GETSAI_ELR_NUM_TOT_CTB_QNQ = value; } }
            public short GETSAI_ELR_NUM_TOT_CTB_TRI { get { return _GETSAI_ELR_NUM_TOT_CTB_TRI; } set { _GETSAI_ELR_NUM_TOT_CTB_TRI = value; } }
            #endregion public properties
        }

        //03 GETSAI-CTB-CONTRIB OCCURS 55.
        public class DatiContributiviSAI
        {
            #region private properties
            private string _GETSAI_ELC_ANNO;
            private string _GETSAI_ELC_COD_CTB;
            private short _GETSAI_ELC_NUM_TOT_CTB;
            private decimal _GETSAI_ELC_IMP_RTB_ANNUA;
            private decimal _GETSAI_ELC_COEFF_RIV;
            private decimal _GETSAI_ELC_MONT_IND;
            #endregion private properties

            #region public properties
            public string GETSAI_ELC_ANNO { get { return _GETSAI_ELC_ANNO; } set { _GETSAI_ELC_ANNO = value; } }
            public string GETSAI_ELC_COD_CTB { get { return _GETSAI_ELC_COD_CTB; } set { _GETSAI_ELC_COD_CTB = value; } }
            public short GETSAI_ELC_NUM_TOT_CTB { get { return _GETSAI_ELC_NUM_TOT_CTB; } set { _GETSAI_ELC_NUM_TOT_CTB = value; } }
            public decimal GETSAI_ELC_IMP_RTB_ANNUA { get { return _GETSAI_ELC_IMP_RTB_ANNUA; } set { _GETSAI_ELC_IMP_RTB_ANNUA = value; } }
            public decimal GETSAI_ELC_COEFF_RIV { get { return _GETSAI_ELC_COEFF_RIV; } set { _GETSAI_ELC_COEFF_RIV = value; } }
            public decimal GETSAI_ELC_MONT_IND { get { return _GETSAI_ELC_MONT_IND; } set { _GETSAI_ELC_MONT_IND = value; } }
            #endregion public properties
        }
        #endregion nested class

        #endregion Properties

      
    }
}
