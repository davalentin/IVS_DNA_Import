using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA
{
    public class Deleghe_Tutele : ITransactionInfo
    {
        #region Properties

        #region Tracciato COBOL
        //03  REC-K.
        //    05  TIPO-REC-K             	 	PIC X.
        //    05  REC-K-DELEGA.
        //        07  K-CODDEL-GP1AP01  		PIC X.
        //        07  K-CODFIS-GP1AP26   		PIC X(16).
        //        07  K-COGN-GP1DCOGNOME  	PIC X(36).
        //        07  K-NOME-GP1DNOME   		PIC X(36).
        //        07  K-SESSO-GP1AP27   		PIC X.
        //        07  K-DATANAS-GP1AP22       PIC X(8).
        //        07  K-COMCOD-GP1AP23   		PIC 9(5).
        //        07  K-COMUNAS-GP1AP24   	PIC X(60).
        //        07  K-PROVNAS-GP1AP25   	PIC XXX.
        //        07  K-INDI1-GP1DINDIRIZZ   	PIC X(52).
        //        07  K-INDI2-GP1DINDIRIZB   	PIC X(52).
        //        07  K-INDI3-GP1DINDIRIZC   	PIC X(52).
        //        07  K-NUM-GP1DCIVICO   		PIC X(18).
        //        07  K-CAP-GP1DCAP   		PIC X(9).
        //        07  K-COMRES-GP1DCOMUNE   	PIC X(37).
        //        07  K-PROVRES-GP1DPROV   	PIC X(3).
        //        07  K-FRA-GP1DFRAZIONE   	PIC X(35).
        //        07  K-EST-GP1DRESIDOM    	PIC X.
        //        07  K-ARCA1-GP1AP28      	PIC X(3).
        //        07  K-ARCA2-GP1AP29      	PIC 9(8) COMP.
        //    05  REC-K-TUTELA.
        //        07  K-CODDEL-GP1AP61  		PIC X.
        //        07  K-CODFIS-GP1AP66   		PIC X(16).
        //        07  K-COGN-GP1TCOGNOME  	PIC X(36).
        //        07  K-NOME-GP1TNOME   		PIC X(36).
        //        07  K-SESSO-GP1AP67   		PIC X.
        //        07  K-DATANAS-GP1AP62       PIC X(8).
        //        07  K-COMCOD-GP1AP63   		PIC 9(5).
        //        07  K-COMUNAS-GP1AP64   	PIC X(60).
        //        07  K-PROVNAS-GP1AP65   	PIC XXX.
        //        07  K-INDI1-GP1TINDIRIZZ   	PIC X(52).
        //        07  K-INDI2-GP1TINDIRIZB   	PIC X(52).
        //        07  K-INDI3-GP1TINDIRIZC   	PIC X(52).
        //        07  K-NUM-GP1TCIVICO   		PIC X(18).
        //        07  K-CAP-GP1TCAP   		PIC X(9).
        //        07  K-COMRES-GP1TCOMUNE   	PIC X(37).
        //        07  K-PROVRES-GP1TPROV   	PIC X(3).
        //        07  K-FRA-GP1TFRAZIONE   	PIC X(35).
        //        07  K-EST-GP1TRESIDOM    	PIC X.
        //        07  K-DATACES-GP1AP70A      PIC 9(6).
        //        07  K-ARCA1-GP1AP68      	PIC X(3).
        //        07  K-ARCA2-GP1AP69      	PIC 9(8) COMP.
        //    05  FILLER                      PIC X(1129).   

        #endregion Tracciato COBOL

        #region Tracciato Host
        //03  REC-K.

        /// <summary>
        ///TIPO-REC-K PIC X.
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string TRKTIPOR { get; set; }

        //05  REC-K-DELEGA.
         
        /// <summary>
        ///07  K-CODDEL-GP1AP01  		PIC X.
        /// </summary>
        [HisFieldInfoMapping(1, 1)]
        public string CODDEL_GP1AP01 { get; set; }

        
        /// <summary>
        ///07  K-CODFIS-GP1AP26   		PIC X(16).
        /// </summary>
        [HisFieldInfoMapping(2, 16)]
        public string CODFIS_GP1AP26  { get; set; }

      
        /// <summary>
        ///07  K-COGN-GP1DCOGNOME  	PIC X(36).
        /// </summary>
        [HisFieldInfoMapping(3, 36)]
        public string COGN_GP1DCOGNOME  { get; set; }

        /// <summary>
        ///07  K-NOME-GP1DNOME  	PIC X(36).
        /// </summary>
        [HisFieldInfoMapping(4, 36)]
        public string NOME_GP1DNOME  { get; set; }
    
        /// <summary>
        ///07  K-SESSO-GP1AP27   		PIC X.
        /// </summary>
        [HisFieldInfoMapping(5, 1)]
        public string SESSO_GP1AP27 { get; set; }

        //07  K-DATANAS-GP1AP22       PIC X(8).
        /// <summary>
        ///DATANAS_GP1AP22G
        /// </summary>
        [HisFieldInfoMapping(6, 2)]
        public string DATANAS_GP1AP22G { get; set; }

        /// <summary>
        ///DATANAS_GP1AP22M
        /// </summary>
        [HisFieldInfoMapping(7, 2)]
        public string DATANAS_GP1AP22M { get; set; }

        /// <summary>
        ///DATANAS-GP1AP22SA
        /// </summary>
        [HisFieldInfoMapping(8, 4)]
        public string DATANAS_GP1AP22SA { get; set; }

        /// <summary>
        /// 07  K-COMCOD-GP1AP23   		PIC 9(5).
        /// </summary>
        [HisFieldInfoMapping(9, 5, CobolType = CobolType.Unsigned)]
        public int COMCOD_GP1AP23 { get; set; }
        
        /// <summary>
        ///07  K-COMUNAS-GP1AP24   	PIC X(60).
        /// </summary>
        [HisFieldInfoMapping(10, 60)]
        public string COMUNAS_GP1AP24 { get; set; }
       
        /// <summary>
        ///07  K-PROVNAS-GP1AP25   	PIC XXX.
        /// </summary>
        [HisFieldInfoMapping(11, 3)]
        public string PROVNAS_GP1AP25 { get; set; }
       
        /// <summary>
        ///07  K-INDI1-GP1DINDIRIZZ   	PIC X(52).
        /// </summary>
        [HisFieldInfoMapping(12, 52)]
        public string INDI1_GP1DINDIRIZZ { get; set; }

        /// <summary>
        ///07 K-INDI2-GP1DINDIRIZB   	PIC X(52).
        /// </summary>
        [HisFieldInfoMapping(13, 52)]
        public string INDI2_GP1DINDIRIZB { get; set; }
 
        /// <summary>
        ///07 K-INDI3-GP1DINDIRIZC   	PIC X(52).
        /// </summary>
        [HisFieldInfoMapping(14, 52)]
        public string INDI3_GP1DINDIRIZC { get; set; }

        /// <summary>
        ///07 K-NUM-GP1DCIVICO   		PIC X(18).
        /// </summary>
        [HisFieldInfoMapping(15, 18)]
        public string NUM_GP1DCIVICO { get; set; }

        /// <summary>
        /// 07   K-CAP-GP1DCAP PIC X(9).
        /// </summary>
        [HisFieldInfoMapping(16, 9)]
        public string CAP_GP1DCAP { get; set; }

        /// <summary>
        /// 07   K-COMRES-GP1DCOMUNE   	PIC X(37).
        /// </summary>
        [HisFieldInfoMapping(17, 37)]
        public string COMRES_GP1DCOMUNE { get; set; }
 
        /// <summary>
        /// 07   K-PROVRES-GP1DPROV   	PIC X(3).
        /// </summary>
        [HisFieldInfoMapping(18, 3)]
        public string PROVRES_GP1DPROV { get; set; }
      
        /// <summary>
        /// 07  K-FRA-GP1DFRAZIONE   	PIC X(35).
        /// </summary>
        [HisFieldInfoMapping(19, 35)]
        public string FRA_GP1DFRAZIONE  { get; set; }

        /// <summary>
        /// 07  K-EST-GP1DRESIDOM    	PIC X.
        /// </summary>
        [HisFieldInfoMapping(20, 1)]
        public string EST_GP1DRESIDOM  { get; set; }
   
        /// <summary>
        /// 07  K-ARCA1-GP1AP28    	PIC X(3).
        /// </summary>
        [HisFieldInfoMapping(21, 3)]
        public string ARCA1_GP1AP28 { get; set; }       
       
        /// <summary>
        /// 07  K-ARCA2-GP1AP29      	PIC 9(8) COMP.
        /// </summary>
        [HisFieldInfoMapping(22, 4, CobolType = CobolType.Binary)]
        public int ARCA2_GP1AP29   { get; set; }

        /// <summary>
        ///07  K-CODDEL-GP1AP61  		PIC X.
        /// </summary>
        [HisFieldInfoMapping(23, 1)]
        public string CODDEL_GP1AP61 { get; set; }

        /// <summary>
        ///07  K-CODFIS-GP1AP66   		PIC X(16).
        /// </summary>
        [HisFieldInfoMapping(24, 16)]
        public string CODFIS_GP1AP66 { get; set; }

        /// <summary>
        ///07  K-COGN-GP1TCOGNOME  	PIC X(36).
        /// </summary>
        [HisFieldInfoMapping(25, 36)]
        public string COGN_GP1TCOGNOME { get; set; }

        /// <summary>
        ///07  K-NOME-GP1TNOME  	PIC X(36).
        /// </summary>
        [HisFieldInfoMapping(26, 36)]
        public string NOME_GP1TNOME { get; set; }

        /// <summary>
        ///07  K-SESSO-GP1AP67   		PIC X.
        /// </summary>
        [HisFieldInfoMapping(27, 1)]
        public string SESSO_GP1AP67 { get; set; }

        //07  K-DATANAS-GP1AP62       PIC X(8).
        /// <summary>
        ///DATANAS_GP1AP62G
        /// </summary>
        [HisFieldInfoMapping(28, 2)]
        public string DATANAS_GP1AP62G { get; set; }

        /// <summary>
        ///DATANAS_GP1AP62M
        /// </summary>
        [HisFieldInfoMapping(29, 2)]
        public string DATANAS_GP1AP62M { get; set; }

        /// <summary>
        ///DATANAS_GP1AP62SA
        /// </summary>
        [HisFieldInfoMapping(30, 4)]
        public string DATANAS_GP1AP62SA { get; set; }

        /// <summary>
        /// 07  K-COMCOD-GP1AP63   		PIC 9(5).
        /// </summary>
        [HisFieldInfoMapping(31, 5, CobolType = CobolType.Unsigned)]
        public int COMCOD_GP1AP63 { get; set; }

        /// <summary>
        ///07  K-COMUNAS-GP1AP64   	PIC X(60).
        /// </summary>
        [HisFieldInfoMapping(32, 60)]
        public string COMUNAS_GP1AP64 { get; set; }

        /// <summary>
        ///07  K-PROVNAS-GP1AP65   	PIC XXX.
        /// </summary>
        [HisFieldInfoMapping(33, 3)]
        public string PROVNAS_GP1AP65 { get; set; }

        /// <summary>
        ///07  K-INDI1-GP1TINDIRIZZ   	PIC X(52).
        /// </summary>
        [HisFieldInfoMapping(34, 52)]
        public string INDI1_GP1TINDIRIZZ { get; set; }

        /// <summary>
        ///07 K-INDI2-GP1TINDIRIZB   	PIC X(52).
        /// </summary>
        [HisFieldInfoMapping(35, 52)]
        public string INDI2_GP1TINDIRIZB { get; set; }

        /// <summary>
        ///07 K-INDI3-GP1TINDIRIZC   	PIC X(52).
        /// </summary>
        [HisFieldInfoMapping(36, 52)]
        public string INDI3_GP1TINDIRIZC { get; set; }

        /// <summary>
        ///07 K-NUM-GP1TCIVICO   		PIC X(18).
        /// </summary>
        [HisFieldInfoMapping(37, 18)]
        public string NUM_GP1TCIVICO { get; set; }

        /// <summary>
        /// 07   K-CAP-GP1TCAP PIC X(9).
        /// </summary>
        [HisFieldInfoMapping(38, 9)]
        public string CAP_GP1TCAP { get; set; }


        /// <summary>
        /// 07   K-COMRES-GP1TCOMUNE  	PIC X(37).
        /// </summary>
        [HisFieldInfoMapping(39, 37)]
        public string COMRES_GP1TCOMUNE { get; set; }

        /// <summary>
        /// 07   K-PROVRES-GP1TPROV   	PIC X(3).
        /// </summary>
        [HisFieldInfoMapping(40, 3)]
        public string PROVRES_GP1TPROV { get; set; }

        /// <summary>
        /// 07  K-FRA-GP1TFRAZIONE   	PIC X(35).
        /// </summary>
        [HisFieldInfoMapping(41, 35)]
        public string FRA_GP1TFRAZIONE { get; set; }


        /// <summary>
        /// 07  K-EST-GP1TRESIDOM    	PIC X.
        /// </summary>
        [HisFieldInfoMapping(42, 1)]
        public string EST_GP1TRESIDOM { get; set; }

        // 07  K-DATACES-GP1AP70A      PIC 9(6).
        /// <summary>
        ///GP1AP70A
        /// </summary>
        [HisFieldInfoMapping(43, 4, CobolType = CobolType.Unsigned)]
        public int DATACES_GP1AP70A { get; set; }

        /// <summary>
        ///GP1AP70M
        /// </summary>
        [HisFieldInfoMapping(44, 2, CobolType = CobolType.Unsigned)]
        public int DATACES_GP1AP70M { get; set; }
        
        /// <summary>
        /// 07  K-ARCA1-GP1AP28    	PIC X(3).
        /// </summary>
        [HisFieldInfoMapping(45, 3)]
        public string ARCA1_GP1AP68 { get; set; }
        //        
        //
        /// <summary>
        /// 07  K-ARCA2-GP1AP69      	PIC 9(8) COMP.
        /// </summary>
        [HisFieldInfoMapping(46, 4, CobolType = CobolType.Binary)]
        public int ARCA2_GP1AP69 { get; set; }


        /// <summary>
        ///  FILLER                      PIC X(1129).    
        /// </summary>
        [HisFieldInfoMapping(47, 1129)]
        public string FILLER { get; set; }

        #endregion Tracciato Host
        public string TransactionName
        {
            get { return "Deleghe_Tutele"; }
        }
        #endregion Properties
    }
}
