using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class Coda
    {
        #region Constructor
        public Coda()
        {
            this.AreaDati2006 = new Dati2006();
            this.AreaDati2007 = new Dati2007();
            this.AreaDati2008 = new Dati2008();
            this.AreaDati2009 = new Dati2009();
            this.AreaDati2010 = new Dati2010();
            this.AreaDati2011 = new Dati2011();
            this.AreaDati2012 = new Dati2012();
            this.AreaDati2013 = new Dati2013();
            this.AreaDati2014 = new Dati2014();
            this.AreaDati2015 = new Dati2015();
            this.AreaDati2016 = new Dati2016();
            this.AreaDati2017 = new Dati2017();
            this.AreaDati2018 = new Dati2018();
            this.AreaDati2019 = new Dati2019();
            this.AreaDati2020 = new Dati2020();
            this.AreaDati2021 = new Dati2021();
        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 T-DATI2006.
        //        03 FILLER-LEA-LIPE           PIC X(240).
        //        03 T-LIPE-GP3                PIC X(330).
        //        03 T-PER-LIPE                PIC X(240).
        //        03 T-STATOESTERO             PIC X(37).
        //*
        //     02 T-DATI2007.
        //        03 T-GP2BN03              PIC 9(4).
        //        03 T-GP2BN04              PIC 9(4).
        //        03 T-GP2BACF.
        //           04 T-GP2BACFAA         PIC 9(4).
        //           04 T-GP2BACFMM         PIC 9(2).
        //        03 T-GP2BACFZ.
        //           04 T-GP2BACFZAA        PIC 9(4).
        //           04 T-GP2BACFZMM        PIC 9(2).
        //        03 T-GP2BD08              PIC X.
        //        03 T-GP1AF17.
        //           04 T-GP1AF17AA         PIC 9(4).
        //           04 T-GP1AF17MM         PIC 9(2).
        //        03 T-GP1AV56.
        //           04 T-GP1AV56AA         PIC 9(4).
        //           04 T-GP1AV56MM         PIC 9(2).
        //        03     T-GP1IBAN          PIC X(34).
        //        03     T-GP1BIC           PIC X(11).
        //        03     T-GP1AXE3B         PIC X.
        //        03     T-GP1AXE3C         PIC X.
        //        03     T-GP1AN87A         PIC X(2).
        //        03     T-GP1AN87B         PIC X(2).
        //        03     T-GP1AN87C         PIC X.
        //        03     T-GP1AN87D         PIC X(12).
        //        03     T-GP1FREQ1         PIC X.
        //        03     T-GP1FREQ2         PIC X.
        //        03     T-GP1FREQ3         PIC X.
        //*
        //        03 T-GP7LC.
        //           04 T-ELTAB-GP7LC OCCURS 6.
        //              05 T-GP7LC61        PIC X(3).
        //              05 T-GP7LC62.
        //                 06 T-GP7LC62A    PIC 9(4).
        //                 06 T-GP7LC62M    PIC 9(2).
        //*
        //     02 T-DATI2008.
        //        03 T-PATRONATI.
        //           04 T-GP1RICDOM.
        //              05 T-GP1RICDOMG     PIC 9(2).
        //              05 T-GP1RICDOMM     PIC 9(2).
        //              05 T-GP1RICDOMA     PIC 9(4).
        //           04 T-GP1RICPTUFF       PIC 9(3).
        //           04 T-GP1RICPCOD        PIC 9(3).
        //           04 T-GP1RICPZON        PIC X(10).
        //           04 T-GP1RICPNUM        PIC 9(8).
        //        03 T-GP2BM00.
        //           04 T-GP2BM04.
        //              05 T-GP2BM04G       PIC 9(2).
        //              05 T-GP2BM04M       PIC 9(2).
        //              05 T-GP2BM04A       PIC 9(4).
        //           04 T-GP2BM05.
        //              05 T-GP2BM05G       PIC 9(2).
        //              05 T-GP2BM05M       PIC 9(2).
        //              05 T-GP2BM05A       PIC 9(4).
        //        03 T-GP2PB00.
        //           04 T-ELTAB-GP2PB OCCURS 8.
        //              05 T-GP2PBPVAR.
        //                 06 T-GP2PBPVARA  PIC 9(4).
        //                 06 T-GP2PBPVARM  PIC 9(2).
        //              05 T-GP2PBCES.
        //                 06 T-GP2PBCESG   PIC 9(2).
        //                 06 T-GP2PBCESM   PIC 9(2).
        //                 06 T-GP2PBCESA   PIC 9(4).
        //              05 T-GP2PBPLEG      PIC 9(4).
        //              05 T-GP2PBPLEG1     PIC 9(4).
        //              05 T-GP2PBPSET      PIC 9(4).
        //              05 T-GP2PBPONR      PIC S9(7)V9(4) COMP-3.
        //              05 T-GP2PBBPAR      PIC 9(2).
        //              05 T-GP2PBBSET      PIC 9(4).
        //              05 T-GP2PBB80       PIC 9(2).
        //              05 T-GP2PBNFGL      PIC X(2).
        //        03 T-GP1INTLEG.
        //           04 T-GP1INTLEGG        PIC 9(2).
        //           04 T-GP1INTLEGM        PIC 9(2).
        //           04 T-GP1INTLEGA        PIC 9(4).
        //*
        //        03 T-AMBIENTE             PIC X.
        //*
        //     02 T-DATI2009.
        //        03 T-TERRORISMO.
        //           04 T-GP1AP35.
        //              05 T-GP1AP35G       PIC 9(2).
        //              05 T-GP1AP35M       PIC 9(2).
        //              05 T-GP1AP35A       PIC 9(4).
        //*
        //           04 T-GP1AC02.
        //              05 T-GP1AC021           PIC X.
        //              05 T-GP1AC022           PIC X.
        //              05 T-GP1AC023           PIC X.
        //*
        //     02 T-DATI2010.
        //        03 T-GP1ENTELIQ.
        //           04 T-GP1ENTELIQA        PIC 9(4).
        //           04 T-GP1ENTELIQM        PIC 9(2).
        //           04 T-GP1ENTELIQG        PIC 9(2).
        //        03 T-GP1ENTERIF            PIC X(20).           
        //        03 T-GP1OLDTOT             PIC X.           
        //        03 T-GP1TRAFTM             PIC X.
        //        03 T-ESENZVITTIME          PIC X(2).                     
        //        03 T-ESENZESTERO           PIC X(2).

        //        03 T-UNICARPE-V            PIC X.     
        //*LOMAR 03/11/2010 - I        
        //*    02 FILLER                     PIC X(2652).

        //     02 T-DATI2011.
        //        03 T-GP3FOPRDTR            PIC X. 
        //        03 T-GP3DDTIVRC.
        //           04 T-GP3DDTIVRCA        PIC 9(4).
        //           04 T-GP3DDTIVRCM        PIC 9(2).
        //           04 T-GP3DDTIVRCG        PIC 9(2).
        //     02	T-DATI2012
        //         03	T-GP1AE01	PIC X(3).
        //         03	T-GP1ALZ6	PIC X(6).
        //         03	T-GP7CAUNCFCC	PIC X(3).
        //         03	T-GP7NAUNPRG	PIC 9(8) BINARY.
        //         03	T-GP7LC42
        //             04	T-GP7LC42G	PIC 9(2).
        //             04	T-GP7LC42M	PIC 9(2).
        //             04	T-GP7LC42A	PIC 9(4).
        //         03	T-TABLAV
        //             04	T-GP2BM10 OCCURS 50
        //                 05	T-GP2BM11
        //                     06	T-GP2BM11G	PIC 9(2).
        //                     06	T-GP2BM11M	PIC 9(2).
        //                     06	T-GP2BM11A	PIC 9(4).
        //                 05	T-GP2BM12
        //                     06	T-GP2BM12G	PIC 9(2).
        //                     06	T-GP2BM12M	PIC 9(2).
        //                     06	T-GP2BM12A	PIC 9(4).
        //                 05    T-GP2BMTA PIC X(2).
        //                 05 T-GP2BM13         PIC S9(9)V9(2) COMP-3.
        //     02 T-DATI2013.
        //          03 T-GP1AJ10              PIC X.
        //          03 T-GP2BACCZ.
        //             04	T-GP2BACCZA         PIC X(4).
        //             04	T-GP2BACCZM         PIC X(2).
        //          03 T-GP2PC00.
        //             04 T-GP2PCANT          PIC X.
        //             04	T-GP2PCPER          PIC S9(2)V9(2) COMP-3.
        //          03 T-TABPEREST.
        //             04 T-GP2IC30 OCCURS 50.
        //                05 T-GP2IC31.
        //                   06 T-GP2IC31G    PIC X(2).
        //                   06 T-GP2IC31M    PIC X(2).
        //                   06 T-GP2IC31A    PIC X(4).
        //                05 T-GP2IC32.
        //                   06 T-GP2IC32G    PIC X(2).
        //                   06 T-GP2IC32M    PIC X(2).
        //                   06 T-GP2IC32A    PIC X(4).
        //                05	T-GP2IC33       PIC X(4).
        //          03 T-TABFREQ.
        //             04	T-GP2IC40 OCCURS 5.
        //                05 T-GP2IC41.
        //                   06 T-GP2IC41G    PIC X(2).
        //                   06 T-GP2IC41M    PIC X(2).
        //                   06 T-GP2IC41A    PIC X(4).
        //                05 T-GP2IC42.
        //                   06 T-GP2IC42G    PIC X(2).
        //                   06 T-GP2IC42M    PIC X(2).
        //                   06 T-GP2IC42A    PIC X(4).
        //                05 T-GP2IC43        PIC 9(11).
        //                05 T-GP2IC44        PIC X.
        //          03 T-GP7LC49              PIC X(3).
        //          03 T-GP7LC59              PIC X(3).
        //          03 T-GP7LC69.
        //             04 T-GP7LC69G          PIC X(2).
        //             04 T-GP7LC69M          PIC X(2).
        //             04	T-GP7LC69A          PIC X(4).
        //      02 T-DATI2014.
        //         03 T-GP1AJ20	            PIC X.
        //         03 T-GP1AJ21	            PIC X(11).
        //         03 T-GP1AJ22	            PIC X(4).
        //         03 T-GP1AJ23	            PIC X(8).
        //         03 T-GP1AJ24	            PIC X(10).
        //*           04 T-GP1AJ24G	
        //*           04 T-GP1AJ24M	
        //*           04 T-GP1AJ24A	
        //*           04 T-GP1AJ24BOH	
        //         03 T-GP1TB60.	
        //            04 T-GP1TB61          PIC X.	
        //            04 T-GP1TBCOGNOME	    PIC X(36).
        //            04 T-GP1TBNOME	    PIC X(36).
        //            04 T-GP1APB62.	
        //               05 T-GP1APB62G     PIC 9(2).	
        //               05 T-GP1APB62M	    PIC 9(2).
        //               05 T-GP1APB62A	    PIC 9(4).
        //            04 T-GP1APB63         PIC 9(5).	
        //            04 T-GP1APB64	        PIC X(60).
        //            04 T-GP1APB65	        PIC X(3).
        //            04 T-GP1APB66	        PIC X(16).
        //            04 T-GP1APB67	        PIC X.
        //            04 T-GP1APB68	        PIC X(3).
        //            04 T-GP1APB69	        PIC 9(8) BINARY.
        //            04 T-GP1APB70.	
        //               05 T-GP1APB70M     PIC 9(2).	
        //               05 T-GP1APB70A	    PIC 9(4).
        //            04 T-GP1TBDECESSO.	
        //               05 T-GP1TBDECESG   PIC 9(2).	
        //               05 T-GP1TBDECESM   PIC 9(2).	
        //               05 T-GP1TBDECESA   PIC 9(4).	
        //            04 T-GP1TBRESIDOM     PIC X.	
        //            04 T-GP1TBIND.	
        //               05 T-GP1TBINDIRIZZ PIC X(52).	
        //               05 T-GP1TBINDIRIZB	PIC X(52).
        //               05 T-GP1TBINDIRIZC	PIC X(52).
        //            04 T-GP1TBCIVICO      PIC X(18).	
        //            04 T-GP1TBFRAZIONE	PIC X(35).
        //            04 T-GP1TBINDIRIZD    PIC X(52).    	
        //            04 T-GP1TBCODCOM	    PIC X(4).
        //            04 T-GP1TBCOMUNE	    PIC X(37).
        //            04 T-GP1TBPROV	    PIC X(3).
        //            04 T-GP1TBCAP	        PIC X(9).
        //         03 T-GP1ABIFIDJ          PIC X(5).	
        //         03 T-GP1CABFIDJ	        PIC X(7).
        //         03 T-GP1PRESO	        PIC 9(4).
        //         03 T-GP1AAESO	        PIC 9(4).
        //         03 T-DATITOTAL.	
        //            04 T-TABTRATTOT OCCURS 6.	
        //               05 T-GESTOT	    PIC X(2).
        //               05 T-CONTRIB OCCURS 10.	
        //                  06 T-ANNOTOT	PIC 9(4).
        //                  06 T-TRATTOT	PIC S9(8)V9(7) COMP-3.
        //                  06 T-CODTRAT    PIC X(3).
        //      02 T-DATI2015.
        //         03 T-GP1AV91E            PIC 9.
        //         03 T-GP1AV91F            PIC 9.
        //         03 T-GP1AD03.
        //            04 T-GP1AD03G         PIC 9(2).
        //            04 T-GP1AD03M         PIC 9(2).
        //            04 T-GP1AD03A         PIC 9(4).        
        //         03 T-GP2PCARM            PIC X.
        //         03 T-DATIINGR. 
        //            04 T-TABINGR OCCURS 2.         
        //               05 T-BUOINGR       PIC S9(7)V9(2) COMP-3.
        //               05 T-BUOUSCI       PIC S9(7)V9(2) COMP-3.
        //      02	T-DATI2017		
        //         03    T-GP1CENTINT	Cod. ente/fondo int.9(4)	N
        //         03	T-DATI-DOMANDA
        //            04	T-GP1DGRP	Codice Gruppo       X(4)	A/N
        //            04	T-GP1DPRD	Codice Prodotto 	X(4)	A/N
        //            04    T-GP1DTIP	Codice Tipo			X(4)	A/N
        //            04	T-GP1DTIPOL	Codice Tipologia	X(4)	A/N
        //            04	T-GP1DFASE	Codice Fase			X(4)	A/N
        //            04	T-GP1DELFLG	Codici FLAG Elab.   X(25)	A/N
        //            04	T-GP1DELIMP	Importo esito elab. S9(7)V9(4) COMP-3
        //      02	T-DATI2018.
        //         03 T-GP1CARPE            PIC X.
        //         03 T-GP1AV91C            PIC 9.
        //         03 T-SEDE-DOMANDA        PIC X(6).
        //         03 T-DES-SEDE-DOMANDA    PIC X(22).
        //      02	T-DATI2019.
        //         03 T-GP1TPCLC            PIC X(8).
        //      02  T-DATI2020
        //         03 T-GP1AJ10OLD          PIC X.
        //         03 T-GP1AJ10Z            PIC X(6).

        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0)]
        public Dati2006 AreaDati2006 { get; set; }

        [HisComplexAreaInfoMapping(1)]
        public Dati2007 AreaDati2007 { get; set; }

        [HisComplexAreaInfoMapping(2)]
        public Dati2008 AreaDati2008 { get; set; }

        [HisComplexAreaInfoMapping(3)]
        public Dati2009 AreaDati2009 { get; set; }

        [HisComplexAreaInfoMapping(4)]
        public Dati2010 AreaDati2010 { get; set; }

        [HisComplexAreaInfoMapping(5)]
        public Dati2011 AreaDati2011 { get; set; }

        [HisComplexAreaInfoMapping(6)]
        public Dati2012 AreaDati2012 { get; set; }

        [HisComplexAreaInfoMapping(7)]
        public Dati2013 AreaDati2013 { get; set; }

        [HisComplexAreaInfoMapping(8)]
        public Dati2014 AreaDati2014 { get; set; }

        [HisComplexAreaInfoMapping(9)]
        public Dati2015 AreaDati2015 { get; set; }

        /// <summary>
        /// T_ENPALS X
        /// </summary>
        [HisFieldInfoMapping(10, 1)]
        public string T_ENPALS { get; set; }

        [HisComplexAreaInfoMapping(11)]
        public Dati2016 AreaDati2016 { get; set; }

        [HisComplexAreaInfoMapping(12)]
        public Dati2017 AreaDati2017 { get; set; }

        [HisComplexAreaInfoMapping(13)]
        public Dati2018 AreaDati2018 { get; set; }

        [HisComplexAreaInfoMapping(14)]
        public Dati2019 AreaDati2019 { get; set; }

        [HisComplexAreaInfoMapping(15)]
        public Dati2020 AreaDati2020 { get; set; }

        [HisComplexAreaInfoMapping(16)]
        public Dati2021 AreaDati2021 { get; set; }

        /// <summary>
        /// FILLER X(411)  
        /// </summary>
        [HisFieldInfoMapping(17, 411)]
        public string FILLER { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class Dati2006
        {
            #region Properties
            #region Tracciato COBOL
            //     02 T-DATI2006.
            //        03 FILLER-LEA-LIPE           PIC X(240).
            //        03 T-LIPE-GP3                PIC X(330).
            //        03 T-PER-LIPE                PIC X(240).
            //        03 T-STATOESTERO             PIC X(37).
            #endregion Tracciato COBOL
            #region Tracciato HOST
            /// <summary>
            /// FILLER_LEA_LIPE X(240)  
            /// </summary>
            [HisFieldInfoMapping(0, 240)]
            public string FILLER_LEA_LIPE { get; set; }

            /// <summary>
            /// T_LIPE_GP3 X(330)  
            /// </summary>
            [HisFieldInfoMapping(1, 330)]
            public string T_LIPE_GP3 { get; set; }

            /// <summary>
            /// T_PER_LIPE X(240)  
            /// </summary>
            [HisFieldInfoMapping(2, 240)]
            public string T_PER_LIPE { get; set; }

            /// <summary>
            /// T_STATOESTERO X(37)  
            /// </summary>
            [HisFieldInfoMapping(3, 37)]
            public string T_STATOESTERO { get; set; }
            #endregion Tracciato HOST
            #endregion Properties
        }

        public class Dati2007
        {
            #region Properties

            #region Tracciato COBOL
            //         02 T-DATI2007.
            //03 T-GP2BN03              PIC 9(4).
            //03 T-GP2BN04              PIC 9(4).
            //03 T-GP2BACF.
            //   04 T-GP2BACFAA         PIC 9(4).
            //   04 T-GP2BACFMM         PIC 9(2).
            //03 T-GP2BACFZ.
            //   04 T-GP2BACFZAA        PIC 9(4).
            //   04 T-GP2BACFZMM        PIC 9(2).
            //03 T-GP2BD08              PIC X.
            //03 T-GP1AF17.
            //   04 T-GP1AF17AA         PIC 9(4).
            //   04 T-GP1AF17MM         PIC 9(2).
            //03 T-GP1AV56.
            //   04 T-GP1AV56AA         PIC 9(4).
            //   04 T-GP1AV56MM         PIC 9(2).
            //03     T-GP1IBAN          PIC X(34).
            //03     T-GP1BIC           PIC X(11).
            //03     T-GP1AXE3B         PIC X.
            //03     T-GP1AXE3C         PIC X.
            //03     T-GP1AN87A         PIC X(2).
            //03     T-GP1AN87B         PIC X(2).
            //03     T-GP1AN87C         PIC X.
            //03     T-GP1AN87D         PIC X(12).
            //03     T-GP1FREQ1         PIC X.
            //03     T-GP1FREQ2         PIC X.
            //03     T-GP1FREQ3         PIC X.
            //03 T-GP7LC.
            //   04 T-ELTAB-GP7LC OCCURS 6.
            //      05 T-GP7LC61        PIC X(3).
            //      05 T-GP7LC62.
            //         06 T-GP7LC62A    PIC 9(4).
            //         06 T-GP7LC62M    PIC 9(2).
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 T-DATI2007.
            /// <summary>
            /// T_GP2BN03 9(4)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BN03 { get; set; }

            /// <summary>
            /// T_GP2BN04 9(4)  
            /// </summary>
            [HisFieldInfoMapping(1, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BN04 { get; set; }

            // 03 T-GP2BACF.
            /// <summary>
            /// T_GP2BACFAA 9(4)  
            /// </summary>
            [HisFieldInfoMapping(2, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BACFAA { get; set; }

            /// <summary>
            /// T_GP2BACFMM 9(2)  
            /// </summary>
            [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BACFMM { get; set; }

            // 03 T-GP2BACFZ.
            /// <summary>
            /// T_GP2BACFZAA 9(4)  
            /// </summary>
            [HisFieldInfoMapping(4, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BACFZAA { get; set; }

            /// <summary>
            /// T_GP2BACFZMM 9(2)  
            /// </summary>
            [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BACFZMM { get; set; }

            /// <summary>
            /// T_GP2BD08 X  
            /// </summary>
            [HisFieldInfoMapping(6, 1)]
            public string T_GP2BD08 { get; set; }

            // 03 T-GP1AF17.
            /// <summary>
            /// T_GP1AF17AA 9(4)  
            /// </summary>
            [HisFieldInfoMapping(7, 4, CobolType = CobolType.Unsigned)]
            public short T_GP1AF17AA { get; set; }

            /// <summary>
            /// T_GP1AF17MM 9(2)  
            /// </summary>
            [HisFieldInfoMapping(8, 2, CobolType = CobolType.Unsigned)]
            public short T_GP1AF17MM { get; set; }

            // 03 T-GP1AV56.
            /// <summary>
            /// T_GP1AV56AA 9(4)  
            /// </summary>
            [HisFieldInfoMapping(9, 4, CobolType = CobolType.Unsigned)]
            public short T_GP1AV56AA { get; set; }

            /// <summary>
            /// T_GP1AV56MM 9(2)  
            /// </summary>
            [HisFieldInfoMapping(10, 2, CobolType = CobolType.Unsigned)]
            public short T_GP1AV56MM { get; set; }

            /// <summary>
            /// T_GP1IBAN X(34)  
            /// </summary>
            [HisFieldInfoMapping(11, 34)]
            public string T_GP1IBAN { get; set; }

            /// <summary>
            /// T_GP1BIC X(11)  
            /// </summary>
            [HisFieldInfoMapping(12, 11)]
            public string T_GP1BIC { get; set; }

            /// <summary>
            /// T_GP1AXE3B X  
            /// </summary>
            [HisFieldInfoMapping(13, 1)]
            public string T_GP1AXE3B { get; set; }

            /// <summary>
            /// T_GP1AXE3C X  
            /// </summary>
            [HisFieldInfoMapping(14, 1)]
            public string T_GP1AXE3C { get; set; }

            /// <summary>
            /// T_GP1AN87A X(2)  
            /// </summary>
            [HisFieldInfoMapping(15, 2)]
            public string T_GP1AN87A { get; set; }

            /// <summary>
            /// T_GP1AN87B X(2)  
            /// </summary>
            [HisFieldInfoMapping(16, 2)]
            public string T_GP1AN87B { get; set; }

            /// <summary>
            /// T_GP1AN87C X  
            /// </summary>
            [HisFieldInfoMapping(17, 1)]
            public string T_GP1AN87C { get; set; }

            /// <summary>
            /// T_GP1AN87D X(12)  
            /// </summary>
            [HisFieldInfoMapping(18, 12)]
            public string T_GP1AN87D { get; set; }

            /// <summary>
            /// T_GP1FREQ1 X  
            /// </summary>
            [HisFieldInfoMapping(19, 1)]
            public string T_GP1FREQ1 { get; set; }

            /// <summary>
            /// T_GP1FREQ2 X  
            /// </summary>
            [HisFieldInfoMapping(20, 1)]
            public string T_GP1FREQ2 { get; set; }

            /// <summary>
            /// T_GP1FREQ3 X  
            /// </summary>
            [HisFieldInfoMapping(21, 1)]
            public string T_GP1FREQ3 { get; set; }

            [HisComplexAreaInfoMapping(22, ListCount = 6)]
            public List<T_ELTAB_GP7LC> LISTT_ELTAB_GP7LC { get; set; }
            #endregion Tracciato Host

            #region nested class
            public class T_ELTAB_GP7LC
            {
                #region Properties

                #region Tracciato COBOL
                //03 T-GP7LC.
                //   04 T-ELTAB-GP7LC OCCURS 6.
                //      05 T-GP7LC61        PIC X(3).
                //      05 T-GP7LC62.
                //         06 T-GP7LC62A    PIC 9(4).
                //         06 T-GP7LC62M    PIC 9(2).
                #endregion Tracciato COBOL

                #region Tracciato Host
                // 03 T-GP7LC.
                // 04 T-ELTAB-GP7LC OCCURS 6.
                /// <summary>
                /// T_GP7LC61 X(3)  
                /// </summary>
                [HisFieldInfoMapping(0, 3)]
                public string T_GP7LC61 { get; set; }

                // 05 T-GP7LC62.
                /// <summary>
                /// T_GP7LC62A 9(4)  
                /// </summary>
                [HisFieldInfoMapping(1, 4, CobolType = CobolType.Unsigned)]
                public short T_GP7LC62A { get; set; }

                /// <summary>
                /// T_GP7LC62M 9(2)  
                /// </summary>
                [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
                public short T_GP7LC62M { get; set; }
                #endregion Tracciato Host

                #endregion Properties
            }
            #endregion nested class

            #endregion Properties
        }

        public class Dati2008
        {
            #region Properties

            #region Tracciato COBOL
            //         02 T-DATI2008.
            //03 T-PATRONATI.
            //   04 T-GP1RICDOM.
            //      05 T-GP1RICDOMG     PIC 9(2).
            //      05 T-GP1RICDOMM     PIC 9(2).
            //      05 T-GP1RICDOMA     PIC 9(4).
            //   04 T-GP1RICPTUFF       PIC 9(3).
            //   04 T-GP1RICPCOD        PIC 9(3).
            //   04 T-GP1RICPZON        PIC X(10).
            //   04 T-GP1RICPNUM        PIC 9(8).
            //03 T-GP2BM00.
            //   04 T-GP2BM04.
            //      05 T-GP2BM04G       PIC 9(2).
            //      05 T-GP2BM04M       PIC 9(2).
            //      05 T-GP2BM04A       PIC 9(4).
            //   04 T-GP2BM05.
            //      05 T-GP2BM05G       PIC 9(2).
            //      05 T-GP2BM05M       PIC 9(2).
            //      05 T-GP2BM05A       PIC 9(4).
            //03 T-GP2PB00.
            //   04 T-ELTAB-GP2PB OCCURS 8.
            //      05 T-GP2PBPVAR.
            //         06 T-GP2PBPVARA  PIC 9(4).
            //         06 T-GP2PBPVARM  PIC 9(2).
            //      05 T-GP2PBCES.
            //         06 T-GP2PBCESG   PIC 9(2).
            //         06 T-GP2PBCESM   PIC 9(2).
            //         06 T-GP2PBCESA   PIC 9(4).
            //      05 T-GP2PBPLEG      PIC 9(4).
            //      05 T-GP2PBPLEG1     PIC 9(4).
            //      05 T-GP2PBPSET      PIC 9(4).
            //      05 T-GP2PBPONR      PIC S9(7)V9(4) COMP-3.
            //      05 T-GP2PBBPAR      PIC 9(2).
            //      05 T-GP2PBBSET      PIC 9(4).
            //      05 T-GP2PBB80       PIC 9(2).
            //      05 T-GP2PBNFGL      PIC X(2).
            //03 T-GP1INTLEG.
            //   04 T-GP1INTLEGG        PIC 9(2).
            //   04 T-GP1INTLEGM        PIC 9(2).
            //   04 T-GP1INTLEGA        PIC 9(4).
            //03 T-AMBIENTE             PIC X.
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 T-DATI2008.
            // 03 T-PATRONATI.
            // 04 T-GP1RICDOM.
            /// <summary>
            /// T_GP1RICDOMG 9(2)  
            /// </summary>
            [HisFieldInfoMapping(0, 2, CobolType = CobolType.Unsigned)]
            public short T_GP1RICDOMG { get; set; }

            /// <summary>
            /// T_GP1RICDOMM 9(2)  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short T_GP1RICDOMM { get; set; }

            /// <summary>
            /// T_GP1RICDOMA 9(4)  
            /// </summary>
            [HisFieldInfoMapping(2, 4, CobolType = CobolType.Unsigned)]
            public short T_GP1RICDOMA { get; set; }

            /// <summary>
            /// T_GP1RICPTUFF 9(3)  
            /// </summary>
            [HisFieldInfoMapping(3, 3, CobolType = CobolType.Unsigned)]
            public short T_GP1RICPTUFF { get; set; }

            /// <summary>
            /// T_GP1RICPCOD 9(3)  
            /// </summary>
            [HisFieldInfoMapping(4, 3, CobolType = CobolType.Unsigned)]
            public short T_GP1RICPCOD { get; set; }

            /// <summary>
            /// T_GP1RICPZON X(10)  
            /// </summary>
            [HisFieldInfoMapping(5, 10)]
            public string T_GP1RICPZON { get; set; }

            /// <summary>
            /// T_GP1RICPNUM 9(8)  
            /// </summary>
            [HisFieldInfoMapping(6, 8, CobolType = CobolType.Unsigned)]
            public int T_GP1RICPNUM { get; set; }

            // 03 T-GP2BM00.
            // 04 T-GP2BM04.
            /// <summary>
            /// T_GP2BM04G 9(2)  
            /// </summary>
            [HisFieldInfoMapping(7, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BM04G { get; set; }

            /// <summary>
            /// T_GP2BM04M 9(2)  
            /// </summary>
            [HisFieldInfoMapping(8, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BM04M { get; set; }

            /// <summary>
            /// T_GP2BM04A 9(4)  
            /// </summary>
            [HisFieldInfoMapping(9, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BM04A { get; set; }

            // 04 T-GP2BM05.
            /// <summary>
            /// T_GP2BM05G 9(2)  
            /// </summary>
            [HisFieldInfoMapping(10, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BM05G { get; set; }

            /// <summary>
            /// T_GP2BM05M 9(2)  
            /// </summary>
            [HisFieldInfoMapping(11, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BM05M { get; set; }

            /// <summary>
            /// T_GP2BM05A 9(4)  
            /// </summary>
            [HisFieldInfoMapping(12, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BM05A { get; set; }

            [HisComplexAreaInfoMapping(13, ListCount = 8)]
            public List<T_ELTAB_GP2PB> LISTT_ELTAB_GP2PB { get; set; }

            // 03 T-GP1INTLEG.
            /// <summary>
            /// T_GP1INTLEGG 9(2)  
            /// </summary>
            [HisFieldInfoMapping(14, 2, CobolType = CobolType.Unsigned)]
            public short T_GP1INTLEGG { get; set; }

            /// <summary>
            /// T_GP1INTLEGM 9(2)  
            /// </summary>
            [HisFieldInfoMapping(15, 2, CobolType = CobolType.Unsigned)]
            public short T_GP1INTLEGM { get; set; }

            /// <summary>
            /// T_GP1INTLEGA 9(4)  
            /// </summary>
            [HisFieldInfoMapping(16, 4, CobolType = CobolType.Unsigned)]
            public short T_GP1INTLEGA { get; set; }

            /// <summary>
            /// T_AMBIENTE X  
            /// </summary>
            [HisFieldInfoMapping(17, 1)]
            public string T_AMBIENTE { get; set; }
            #endregion Tracciato Host

            #region nested class
            public class T_ELTAB_GP2PB
            {
                #region Properties

                #region Tracciato COBOL
                //03 T-GP2PB00.
                //   04 T-ELTAB-GP2PB OCCURS 8.
                //      05 T-GP2PBPVAR.
                //         06 T-GP2PBPVARA  PIC 9(4).
                //         06 T-GP2PBPVARM  PIC 9(2).
                //      05 T-GP2PBCES.
                //         06 T-GP2PBCESG   PIC 9(2).
                //         06 T-GP2PBCESM   PIC 9(2).
                //         06 T-GP2PBCESA   PIC 9(4).
                //      05 T-GP2PBPLEG      PIC 9(4).
                //      05 T-GP2PBPLEG1     PIC 9(4).
                //      05 T-GP2PBPSET      PIC 9(4).
                //      05 T-GP2PBPONR      PIC S9(7)V9(4) COMP-3.
                //      05 T-GP2PBBPAR      PIC 9(2).
                //      05 T-GP2PBBSET      PIC 9(4).
                //      05 T-GP2PBB80       PIC 9(2).
                //      05 T-GP2PBNFGL      PIC X(2).
                #endregion Tracciato COBOL

                #region Tracciato Host
                // 03 T-GP2PB00.
                // 04 T-ELTAB-GP2PB OCCURS 8.
                // 05 T-GP2PBPVAR.
                /// <summary>
                /// T_GP2PBPVARA 9(4)  
                /// </summary>
                [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
                public short T_GP2PBPVARA { get; set; }

                /// <summary>
                /// T_GP2PBPVARM 9(2)  
                /// </summary>
                [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
                public short T_GP2PBPVARM { get; set; }

                // 05 T-GP2PBCES.
                /// <summary>
                /// T_GP2PBCESG 9(2)  
                /// </summary>
                [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
                public short T_GP2PBCESG { get; set; }

                /// <summary>
                /// T_GP2PBCESM 9(2)  
                /// </summary>
                [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
                public short T_GP2PBCESM { get; set; }

                /// <summary>
                /// T_GP2PBCESA 9(4)  
                /// </summary>
                [HisFieldInfoMapping(4, 4, CobolType = CobolType.Unsigned)]
                public short T_GP2PBCESA { get; set; }

                /// <summary>
                /// T_GP2PBPLEG 9(4)  
                /// </summary>
                [HisFieldInfoMapping(5, 4, CobolType = CobolType.Unsigned)]
                public short T_GP2PBPLEG { get; set; }

                /// <summary>
                /// T_GP2PBPLEG1 9(4)  
                /// </summary>
                [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
                public short T_GP2PBPLEG1 { get; set; }

                /// <summary>
                /// T_GP2PBPSET 9(4)  
                /// </summary>
                [HisFieldInfoMapping(7, 4, CobolType = CobolType.Unsigned)]
                public short T_GP2PBPSET { get; set; }

                /// <summary>
                /// T_GP2PBPONR S9(7)V9(4) COMP-3 
                /// </summary>
                [HisFieldInfoMapping(8, 6, Scale = 4, CobolType = CobolType.Comp3)]
                public decimal T_GP2PBPONR { get; set; }

                /// <summary>
                /// T_GP2PBBPAR 9(2)  
                /// </summary>
                [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
                public short T_GP2PBBPAR { get; set; }

                /// <summary>
                /// T_GP2PBBSET 9(4)  
                /// </summary>
                [HisFieldInfoMapping(10, 4, CobolType = CobolType.Unsigned)]
                public short T_GP2PBBSET { get; set; }

                /// <summary>
                /// T_GP2PBB80 9(2)  
                /// </summary>
                [HisFieldInfoMapping(11, 2, CobolType = CobolType.Unsigned)]
                public short T_GP2PBB80 { get; set; }

                /// <summary>
                /// T_GP2PBNFGL X(2)  
                /// </summary>
                [HisFieldInfoMapping(12, 2)]
                public short T_GP2PBNFGL { get; set; }
                #endregion Tracciato Host

                #region nested class
                #endregion nested class

                #endregion Properties
            }
            #endregion nested class

            #endregion Properties
        }

        public class Dati2009
        {
            #region Properties

            #region Tracciato COBOL
            //         02 T-DATI2009.
            //03 T-TERRORISMO.
            //   04 T-GP1AP35.
            //      05 T-GP1AP35G       PIC 9(2).
            //      05 T-GP1AP35M       PIC 9(2).
            //      05 T-GP1AP35A       PIC 9(4).
            //   04 T-GP1AC02.
            //      05 T-GP1AC021           PIC X.
            //      05 T-GP1AC022           PIC X.
            //      05 T-GP1AC023           PIC X.
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 T-DATI2009.
            // 03 T-TERRORISMO.
            // 04 T-GP1AP35.
            /// <summary>
            /// T_GP1AP35G 9(2)  
            /// </summary>
            [HisFieldInfoMapping(0, 2, CobolType = CobolType.Unsigned)]
            public short T_GP1AP35G { get; set; }

            /// <summary>
            /// T_GP1AP35M 9(2)  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short T_GP1AP35M { get; set; }

            /// <summary>
            /// T_GP1AP35A 9(4)  
            /// </summary>
            [HisFieldInfoMapping(2, 4, CobolType = CobolType.Unsigned)]
            public short T_GP1AP35A { get; set; }

            // 04 T-GP1AC02.
            /// <summary>
            /// T_GP1AC021 X  
            /// </summary>
            [HisFieldInfoMapping(3, 1)]
            public string T_GP1AC021 { get; set; }

            /// <summary>
            /// T_GP1AC022 X  
            /// </summary>
            [HisFieldInfoMapping(4, 1)]
            public string T_GP1AC022 { get; set; }

            /// <summary>
            /// T_GP1AC023 X  
            /// </summary>
            [HisFieldInfoMapping(5, 1)]
            public string T_GP1AC023 { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Dati2010
        {
            #region Properties

            #region Tracciato COBOL
            //         02 T-DATI2010.
            //03 T-GP1ENTELIQ.
            //   04 T-GP1ENTELIQA        PIC 9(4).
            //   04 T-GP1ENTELIQM        PIC 9(2).
            //   04 T-GP1ENTELIQG        PIC 9(2).
            //03 T-GP1ENTERIF            PIC X(20).           
            //03 T-GP1OLDTOT             PIC X.           
            //03 T-GP1TRAFTM             PIC X.
            //03 T-ESENZVITTIME          PIC X(2).                     
            //03 T-ESENZESTERO           PIC X(2).
            //03 T-UNICARPE-V            PIC X.  
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 T-DATI2010.
            // 03 T-GP1ENTELIQ.
            /// <summary>
            /// T_GP1ENTELIQA 9(4)  
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short T_GP1ENTELIQA { get; set; }

            /// <summary>
            /// T_GP1ENTELIQM 9(2)  
            /// </summary>
            [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
            public short T_GP1ENTELIQM { get; set; }

            /// <summary>
            /// T_GP1ENTELIQG 9(2)  
            /// </summary>
            [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
            public short T_GP1ENTELIQG { get; set; }

            /// <summary>
            /// T_GP1ENTERIF X(20)  
            /// </summary>
            [HisFieldInfoMapping(3, 20)]
            public string T_GP1ENTERIF { get; set; }

            /// <summary>
            /// T_GP1OLDTOT X  
            /// </summary>
            [HisFieldInfoMapping(4, 1)]
            public string T_GP1OLDTOT { get; set; }

            /// <summary>
            /// T_GP1TRAFTM X  
            /// </summary>
            [HisFieldInfoMapping(5, 1)]
            public string T_GP1TRAFTM { get; set; }

            /// <summary>
            /// T_ESENZVITTIME X(2)  
            /// </summary>
            [HisFieldInfoMapping(6, 2)]
            public string T_ESENZVITTIME { get; set; }

            /// <summary>
            /// T_ESENZESTERO X(2)  
            /// </summary>
            [HisFieldInfoMapping(7, 2)]
            public string T_ESENZESTERO { get; set; }

            /// <summary>
            /// T_UNICARPE_V X  
            /// </summary>
            [HisFieldInfoMapping(8, 1)]
            public string T_UNICARPE_V { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Dati2011
        {
            #region Properties

            #region Tracciato COBOL
            //         02 T-DATI2011.
            //03 T-GP3FOPRDTR            PIC X. 
            //03 T-GP3DDTIVRC.
            //   04 T-GP3DDTIVRCA        PIC 9(4).
            //   04 T-GP3DDTIVRCM        PIC 9(2).
            //   04 T-GP3DDTIVRCG        PIC 9(2).
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 T-DATI2011.
            /// <summary>
            /// T_GP3FOPRDTR X  
            /// </summary>
            [HisFieldInfoMapping(0, 1)]
            public string T_GP3FOPRDTR { get; set; }

            // 03 T-GP3DDTIVRC.
            /// <summary>
            /// T_GP3DDTIVRCA 9(4)  
            /// </summary>
            [HisFieldInfoMapping(1, 4, CobolType = CobolType.Unsigned)]
            public short T_GP3DDTIVRCA { get; set; }

            /// <summary>
            /// T_GP3DDTIVRCM 9(2)  
            /// </summary>
            [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
            public short T_GP3DDTIVRCM { get; set; }

            /// <summary>
            /// T_GP3DDTIVRCG 9(2)  
            /// </summary>
            [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
            public short T_GP3DDTIVRCG { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }

        public class Dati2012
        {
            #region Properties

            #region Tracciato COBOL
            //02	T-DATI2012
            //    03	T-GP1AE01	PIC X(3).
            //    03	T-GP1ALZ6	PIC X(6).
            //    03	T-GP7CAUNCFCC	PIC X(3).
            //    03	T-GP7NAUNPRG	PIC 9(8) BINARY.
            //    03	T-GP7LC42
            //        04	T-GP7LC42G	PIC 9(2).
            //        04	T-GP7LC42M	PIC 9(2).
            //        04	T-GP7LC42A	PIC 9(4).
            //    03	T-TABLAV
            //        04	T-GP2BM10 OCCURS 50
            //            05	T-GP2BM11
            //                06	T-GP2BM11G	PIC 9(2).
            //                06	T-GP2BM11M	PIC 9(2).
            //                06	T-GP2BM11A	PIC 9(4).
            //            05	T-GP2BM12
            //                06	T-GP2BM12G	PIC 9(2).
            //                06	T-GP2BM12M	PIC 9(2).
            //                06	T-GP2BM12A	PIC 9(4).
            //            05    T-GP2BMTA PIC X(2).
            //            05 T-GP2BM13         PIC S9(9)V9(2) COMP-3.
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 02 T-DATI2012.
            /// <summary>
            /// T_GP1AE01 X (3) 
            /// </summary>
            [HisFieldInfoMapping(0, 3)]
            public string T_GP1AE01 { get; set; }

            /// <summary>
            /// T_GP1ALZ6 X (6) 
            /// </summary>
            [HisFieldInfoMapping(1, 6)]
            public string T_GP1ALZ6 { get; set; }

            /// <summary>
            /// T_GP7CAUNCFCC	X(3).
            /// <summary>
            [HisFieldInfoMapping(2, 3)]
            public string T_GP7CAUNCFCC { get; set; }

            ///<summary>
            /// T_GP7NAUNPRG 9(8) BINARY.
            /// <summary>
            [HisFieldInfoMapping(3, 4, CobolType = CobolType.Binary)]
            public int T_GP7NAUNPRG { get; set; }

            /// <summary>
            /// T_GP7LC42G 9(2).
            /// <summary>
            [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
            public short T_GP7LC42G { get; set; }

            /// <summary>
            /// T_GP7LC42M 9(2).
            /// <summary>
            [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
            public short T_GP7LC42M { get; set; }

            /// <summary>
            /// T_GP7LC42A 9(4).
            /// <summary>
            [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
            public short T_GP7LC42A { get; set; }
            //03 T-TABLAV
            /// <summary>
            /// T_GP2BM10 OCCURS 50
            /// </summary>
            [HisComplexAreaInfoMapping(7, ListCount = 50)]
            public List<T_GP2BM10> LISTT_GP2BM10 { get; set; }
            #endregion Tracciato Host
            #endregion Properties

            #region nested class
            public class T_GP2BM10
            {
                #region Properties

                #region Tracciato COBOL
                //05 T-GP2BM11
                /// <summary>
                /// T_GP2BM11G 9(2)  
                /// </summary>
                [HisFieldInfoMapping(0, 2, CobolType = CobolType.Unsigned)]
                public short T_GP2BM11G { get; set; }

                /// <summary>
                /// T_GP2BM11M 9(2)  
                /// </summary>
                [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
                public short T_GP2BM11M { get; set; }

                /// <summary>
                /// T_GP2BM11A 9(4)  
                /// </summary>
                [HisFieldInfoMapping(2, 4, CobolType = CobolType.Unsigned)]
                public short T_GP2BM11A { get; set; }

                //05 T-GP2BM12
                /// <summary>
                /// T_GP2BM12G 9(2)  
                /// </summary>
                [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
                public short T_GP2BM12G { get; set; }

                /// <summary>
                /// T_GP2BM12M 9(2)  
                /// </summary>
                [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
                public short T_GP2BM12M { get; set; }

                /// <summary>
                /// T_GP2BM12A 9(4)  
                /// </summary>
                [HisFieldInfoMapping(5, 4, CobolType = CobolType.Unsigned)]
                public short T_GP2BM12A { get; set; }

                /// <summary>
                /// T_GP2BMTA X(2)
                /// </summary>
                [HisFieldInfoMapping(6, 2)]
                public string T_GP2BMTA { get; set; }

                /// <summary>
                /// T_GP2BM13 S9(9)V9(2) COMP-3
                /// <summary>
                [HisFieldInfoMapping(7, 6, Scale = 2, CobolType = CobolType.Comp3)]
                public decimal T_GP2BM13 { get; set; }
                #endregion Tracciato Host

                #endregion Properties
            }
            #endregion nested class
        }

        public class Dati2013
        {
            #region Properties

            #region Tracciato COBOL
            //02 T-DATI2013.
            //03 T-GP1AJ10              PIC X.
            //03 T-GP2BACCZ.
            //   04	T-GP2BACCZA         PIC X(4).
            //   04	T-GP2BACCZM         PIC X(2).
            //03 T-GP2PC00.
            //   04 T-GP2PCANT          PIC X.
            //   04	T-GP2PCPER          PIC S9(2)V9(2) COMP-3.
            //03 T-TABPEREST.
            //   04 T-GP2IC30 OCCURS 50.
            //      05 T-GP2IC31.
            //         06 T-GP2IC31G    PIC X(2).
            //         06 T-GP2IC31M    PIC X(2).
            //         06 T-GP2IC31A    PIC X(4).
            //      05 T-GP2IC32.
            //         06 T-GP2IC32G    PIC X(2).
            //         06 T-GP2IC32M    PIC X(2).
            //         06 T-GP2IC32A    PIC X(4).
            //      05	T-GP2IC33       PIC X(4).
            //03 T-TABFREQ.
            //   04	T-GP2IC40 OCCURS 5.
            //      05 T-GP2IC41.
            //         06 T-GP2IC41G    PIC X(2).
            //         06 T-GP2IC41M    PIC X(2).
            //         06 T-GP2IC41A    PIC X(4).
            //      05 T-GP2IC42.
            //         06 T-GP2IC42G    PIC X(2).
            //         06 T-GP2IC42M    PIC X(2).
            //         06 T-GP2IC42A    PIC X(4).
            //      05 T-GP2IC43        PIC 9(11).
            //      05 T-GP2IC44        PIC X.
            //03 T-GP7LC49              PIC X(3).
            //03 T-GP7LC59              PIC X(3).
            //03 T-GP7LC69.
            //   04 T-GP7LC69G          PIC X(2).
            //   04 T-GP7LC69M          PIC X(2).
            //   04	T-GP7LC69A          PIC X(4).
            // 03 T-GP2BH01E PIC S9(7)V9(4) COMP-3.
            // 03 T-GP2BL01E PIC S9(7)V9(4) COMP-3.
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// T_GP1AJ10 X
            /// <summary>
            [HisFieldInfoMapping(0, 1)]
            public string T_GP1AJ10 { get; set; }

            /// <summary>
            /// T_GP2BACCZA X(4)
            /// <summary>
            [HisFieldInfoMapping(1, 4)]
            public string T_GP2BACCZA { get; set; }

            /// <summary>
            /// T_GP2BACCZM X(2)
            /// <summary>
            [HisFieldInfoMapping(2, 2)]
            public string T_GP2BACCZM { get; set; }

            /// <summary>
            /// T_GP2PCANT X
            /// <summary>
            [HisFieldInfoMapping(3, 1)]
            public string T_GP2PCANT { get; set; }

            /// <summary>
            ///	T_GP2PCPER S9(2)V9(2) COMP-3
            /// <summary>
            [HisFieldInfoMapping(4, 3, Scale = 2, CobolType = CobolType.Comp3)]
            public decimal T_GP2PCPER { get; set; }

            /// <summary>
            /// T_GP2IC30 OCCURS 50
            /// </summary>
            [HisComplexAreaInfoMapping(5, ListCount = 50)]
            public List<T_GP2IC30> LISTT_GP2IC30 { get; set; }

            /// <summary>
            /// T_GP2IC40 OCCURS 5.
            /// </summary>
            [HisComplexAreaInfoMapping(6, ListCount = 5)]
            public List<T_GP2IC40> LISTT_GP2IC40 { get; set; }

            /// </summary>
            /// T_GP7LC49 X(3)
            /// </summary>
            [HisFieldInfoMapping(7, 3)]
            public string T_GP7LC49 { get; set; }

            /// </summary>
            /// T_GP7LC59 X(3)
            /// </summary>
            [HisFieldInfoMapping(8, 3)]
            public string T_GP7LC59 { get; set; }

            /// </summary>
            /// T_GP7LC69G X(2)
            /// </summary>
            [HisFieldInfoMapping(9, 2)]
            public string T_GP7LC69G { get; set; }

            /// </summary>
            /// T_GP7LC69M X(2)
            /// </summary>
            [HisFieldInfoMapping(10, 2)]
            public string T_GP7LC69M { get; set; }

            /// </summary>
            /// T_GP7LC69A X(4)
            /// </summary>
            [HisFieldInfoMapping(11, 4)]
            public string T_GP7LC69A { get; set; }

            /// <summary>
            // 03 T_GP2BH01E S9(7)V9(4) COMP-3
            /// <summary>
            [HisFieldInfoMapping(13, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal T_GP2BH01E { get; set; }

            /// <summary>
            // 03 T_GP2BL01E S9(7)V9(4) COMP-3
            /// <summary>
            [HisFieldInfoMapping(14, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal T_GP2BL01E { get; set; }

            /// <summary>
            //03 T_GP2BL10E S9(7)V9(4) COMP-3
            /// <summary>
            [HisFieldInfoMapping(15, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal T_GP2BL10E { get; set; }
            #endregion Tracciato Host
            #endregion Properties

            #region nested class
            public class T_GP2IC30
            {
                #region Properties

                #region Tracciato COBOL
                //   04 T-GP2IC30 OCCURS 50.
                //      05 T-GP2IC31.
                //         06 T-GP2IC31G    PIC X(2).
                //         06 T-GP2IC31M    PIC X(2).
                //         06 T-GP2IC31A    PIC X(4).
                //      05 T-GP2IC32.
                //         06 T-GP2IC32G    PIC X(2).
                //         06 T-GP2IC32M    PIC X(2).
                //         06 T-GP2IC32A    PIC X(4).
                //      05	T-GP2IC33       PIC X(4).
                #endregion Tracciato COBOL

                #region Tracciato Host
                /// <summary>
                /// T_GP2IC31G X(2)
                /// <summary>
                [HisFieldInfoMapping(0, 2)]
                public string T_GP2IC31G { get; set; }

                /// <summary>
                /// T_GP2IC31M X(2)
                /// <summary>
                [HisFieldInfoMapping(1, 2)]
                public string T_GP2IC31M { get; set; }

                /// <summary>
                /// T_GP2IC31A X(4)
                /// <summary>
                [HisFieldInfoMapping(2, 4)]
                public string T_GP2IC31A { get; set; }

                /// <summary>
                /// T_GP2IC32G X(2)
                /// <summary>
                [HisFieldInfoMapping(3, 2)]
                public string T_GP2IC32G { get; set; }

                /// <summary>
                /// T_GP2IC32M X(2)
                /// <summary>
                [HisFieldInfoMapping(4, 2)]
                public string T_GP2IC32M { get; set; }

                /// <summary>
                /// T_GP2IC32A X(4)
                /// <summary>
                [HisFieldInfoMapping(5, 4)]
                public string T_GP2IC32A { get; set; }

                /// <summary>
                /// T_GP2IC33 X(4)
                /// <summary>
                [HisFieldInfoMapping(6, 4)]
                public string T_GP2IC33 { get; set; }
                #endregion Tracciato Host

                #endregion Properties
            }

            public class T_GP2IC40
            {
                #region Properties

                #region Tracciato COBOL
                //   04	T-GP2IC40 OCCURS 5.
                //      05 T-GP2IC41.
                //         06 T-GP2IC41G    PIC X(2).
                //         06 T-GP2IC41M    PIC X(2).
                //         06 T-GP2IC41A    PIC X(4).
                //      05 T-GP2IC42.
                //         06 T-GP2IC42G    PIC X(2).
                //         06 T-GP2IC42M    PIC X(2).
                //         06 T-GP2IC42A    PIC X(4).
                //      05 T-GP2IC43        PIC 9(11).
                //      05 T-GP2IC44        PIC X.
                #endregion Tracciato COBOL

                #region Tracciato Host
                /// <summary>
                /// T_GP2IC41G X(2)
                /// <summary>
                [HisFieldInfoMapping(0, 2)]
                public string T_GP2IC41G { get; set; }

                /// <summary>
                /// T_GP2IC41M X(2)
                /// <summary>
                [HisFieldInfoMapping(1, 2)]
                public string T_GP2IC41M { get; set; }

                /// <summary>
                /// T_GP2IC41A X(4)
                /// <summary>
                [HisFieldInfoMapping(2, 4)]
                public string T_GP2IC41A { get; set; }

                /// <summary>
                /// T_GP2IC42G X(2)
                /// <summary>
                [HisFieldInfoMapping(3, 2)]
                public string T_GP2IC42G { get; set; }

                /// <summary>
                /// T_GP2IC42M X(2)
                /// <summary>
                [HisFieldInfoMapping(4, 2)]
                public string T_GP2IC42M { get; set; }

                /// <summary>
                /// T_GP2IC42A X(4)
                /// <summary>
                [HisFieldInfoMapping(5, 4)]
                public string T_GP2IC42A { get; set; }

                /// <summary>
                /// T_GP2IC43 9(11)
                /// <summary>
                [HisFieldInfoMapping(6, 11, CobolType = CobolType.Unsigned)]
                public long T_GP2IC43 { get; set; }

                /// <summary>
                /// T_GP2IC44 X
                /// <summary>
                [HisFieldInfoMapping(7, 1)]
                public string T_GP2IC44 { get; set; }
                #endregion Tracciato Host

                #endregion Properties
            }
            #endregion nested class
        }

        public class Dati2014
        {
            #region Properties

            #region Tracciato COBOL
            //02 T-DATI2014.
            //         03 T-GP1AJ20	            PIC X.
            //         03 T-GP1AJ21	            PIC X(11).
            //         03 T-GP1AJ22	            PIC X(4).
            //         03 T-GP1AJ23	            PIC X(8).
            //         03 T-GP1AJ24	            PIC X(10).
            //*           04 T-GP1AJ24G	
            //*           04 T-GP1AJ24M	
            //*           04 T-GP1AJ24A	
            //*           04 T-GP1AJ24BOH	
            //         03 T-GP1TB60.	
            //            04 T-GP1TB61          PIC X.	
            //            04 T-GP1TBCOGNOME	    PIC X(36).
            //            04 T-GP1TBNOME	    PIC X(36).
            //            04 T-GP1APB62.	
            //               05 T-GP1APB62G     PIC 9(2).	
            //               05 T-GP1APB62M	    PIC 9(2).
            //               05 T-GP1APB62A	    PIC 9(4).
            //            04 T-GP1APB63         PIC 9(5).	
            //            04 T-GP1APB64	        PIC X(60).
            //            04 T-GP1APB65	        PIC X(3).
            //            04 T-GP1APB66	        PIC X(16).
            //            04 T-GP1APB67	        PIC X.
            //            04 T-GP1APB68	        PIC X(3).
            //            04 T-GP1APB69	        PIC 9(8) BINARY.
            //            04 T-GP1APB70.	
            //               05 T-GP1APB70M     PIC 9(2).	
            //               05 T-GP1APB70A	    PIC 9(4).
            //            04 T-GP1TBDECESSO.	
            //               05 T-GP1TBDECESG   PIC 9(2).	
            //               05 T-GP1TBDECESM   PIC 9(2).	
            //               05 T-GP1TBDECESA   PIC 9(4).	
            //            04 T-GP1TBRESIDOM     PIC X.	
            //            04 T-GP1TBIND.	
            //               05 T-GP1TBINDIRIZZ PIC X(52).	
            //               05 T-GP1TBINDIRIZB	PIC X(52).
            //               05 T-GP1TBINDIRIZC	PIC X(52).
            //            04 T-GP1TBCIVICO      PIC X(18).	
            //            04 T-GP1TBFRAZIONE	PIC X(35).
            //            04 T-GP1TBINDIRIZD    PIC X(52).    	
            //            04 T-GP1TBCODCOM	    PIC X(4).
            //            04 T-GP1TBCOMUNE	    PIC X(37).
            //            04 T-GP1TBPROV	    PIC X(3).
            //            04 T-GP1TBCAP	        PIC X(9).
            //         03 T-GP1ABIFIDJ          PIC X(5).	
            //         03 T-GP1CABFIDJ	        PIC X(7).
            //         03 T-GP1PRESO	        PIC 9(4).
            //         03 T-GP1AAESO	        PIC 9(4).
            //         03 T-DATITOTAL.	
            //            04 T-TABTRATTOT OCCURS 6.	
            //               05 T-GESTOT	    PIC X(2).
            //               05 T-CONTRIB OCCURS 10.	
            //                  06 T-ANNOTOT	PIC 9(4).
            //                  06 T-TRATTOT	PIC S9(8)V9(7) COMP-3.
            //                  06 T-CODTRAT    PIC X(3).
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// </summary>
            /// T_GP1AJ20 X
            /// </summary>
            [HisFieldInfoMapping(0, 1)]
            public string T_GP1AJ20 { get; set; }

            /// </summary>
            /// T_GP1AJ21 X(11)
            /// </summary>
            [HisFieldInfoMapping(1, 11)]
            public string T_GP1AJ21 { get; set; }

            /// </summary>
            /// T_GP1AJ22 X(4)
            /// </summary>
            [HisFieldInfoMapping(2, 4)]
            public string T_GP1AJ22 { get; set; }

            /// </summary>
            /// T_GP1AJ23 X(8)
            /// </summary>
            [HisFieldInfoMapping(3, 8)]
            public string T_GP1AJ23 { get; set; }

            /// </summary>
            /// T_GP1AJ24 X(10)
            /// </summary>
            [HisFieldInfoMapping(4, 10)]
            public string T_GP1AJ24 { get; set; }

            /// </summary>
            /// T_GP1TB61 X
            /// </summary>
            [HisFieldInfoMapping(5, 1)]
            public string T_GP1TB61 { get; set; }

            /// </summary>
            /// T_GP1TBCOGNOME X(36)
            /// </summary>
            [HisFieldInfoMapping(6, 36)]
            public string T_GP1TBCOGNOME { get; set; }

            /// </summary>
            /// T_GP1TBNOME X(36)
            /// </summary>
            [HisFieldInfoMapping(7, 36)]
            public string T_GP1TBNOME { get; set; }

            /// <summary>
            /// T_GP1APB62G 9(2)  
            /// </summary>
            [HisFieldInfoMapping(8, 2, CobolType = CobolType.Unsigned)]
            public short T_GP1APB62G { get; set; }

            /// <summary>
            /// T_GP1APB62M 9(2)  
            /// </summary>
            [HisFieldInfoMapping(9, 2, CobolType = CobolType.Unsigned)]
            public short T_GP1APB62M { get; set; }

            /// <summary>
            /// T_GP1APB62A 9(4)  
            /// </summary>
            [HisFieldInfoMapping(10, 4, CobolType = CobolType.Unsigned)]
            public short T_GP1APB62A { get; set; }

            /// <summary>
            /// T_GP1APB63 9(5)  
            /// </summary>
            [HisFieldInfoMapping(11, 5, CobolType = CobolType.Unsigned)]
            public int T_GP1APB63 { get; set; }

            /// </summary>
            /// T_GP1APB64 X(60)
            /// </summary>
            [HisFieldInfoMapping(12, 60)]
            public string T_GP1APB64 { get; set; }

            /// </summary>
            /// T_GP1APB65 X(3)
            /// </summary>
            [HisFieldInfoMapping(13, 3)]
            public string T_GP1APB65 { get; set; }

            /// </summary>
            /// T_GP1APB66 X(16)
            /// </summary>
            [HisFieldInfoMapping(14, 16)]
            public string T_GP1APB66 { get; set; }

            /// </summary>
            /// T_GP1APB67 X
            /// </summary>
            [HisFieldInfoMapping(15, 1)]
            public string T_GP1APB67 { get; set; }

            /// </summary>
            /// T_GP1APB68 X(3)
            /// </summary>
            [HisFieldInfoMapping(16, 3)]
            public string T_GP1APB68 { get; set; }

            /// <summary>
            //  T_GP1APB69 9(8) BINARY
            /// <summary>
            [HisFieldInfoMapping(17, 4, CobolType = CobolType.Binary)]
            public int T_GP1APB69 { get; set; }

            /// <summary>
            /// T_GP1APB70M 9(2)  
            /// </summary>
            [HisFieldInfoMapping(18, 2, CobolType = CobolType.Unsigned)]
            public short T_GP1APB70M { get; set; }

            /// <summary>
            /// T_GP1APB70A 9(4)  
            /// </summary>
            [HisFieldInfoMapping(19, 4, CobolType = CobolType.Unsigned)]
            public short T_GP1APB70A { get; set; }

            /// <summary>
            /// T_GP1TBDECESG 9(2)  
            /// </summary>
            [HisFieldInfoMapping(20, 2, CobolType = CobolType.Unsigned)]
            public short T_GP1TBDECESG { get; set; }

            /// <summary>
            /// T_GP1TBDECESM 9(2)  
            /// </summary>
            [HisFieldInfoMapping(21, 2, CobolType = CobolType.Unsigned)]
            public short T_GP1TBDECESM { get; set; }

            /// <summary>
            /// T_GP1TBDECESA 9(4)  
            /// </summary>
            [HisFieldInfoMapping(22, 4, CobolType = CobolType.Unsigned)]
            public short T_GP1TBDECESA { get; set; }

            /// </summary>
            /// T_GP1TBRESIDOM X
            /// </summary>
            [HisFieldInfoMapping(23, 1)]
            public string T_GP1TBRESIDOM { get; set; }

            /// </summary>
            /// T_GP1TBINDIRIZZ X(52)
            /// </summary>
            [HisFieldInfoMapping(24, 52)]
            public string T_GP1TBINDIRIZZ { get; set; }

            /// </summary>
            /// T_GP1TBINDIRIZB X(52)
            /// </summary>
            [HisFieldInfoMapping(25, 52)]
            public string T_GP1TBINDIRIZB { get; set; }

            /// </summary>
            /// T_GP1TBINDIRIZC X(52)
            /// </summary>
            [HisFieldInfoMapping(26, 52)]
            public string T_GP1TBINDIRIZC { get; set; }

            /// </summary>
            /// T_GP1TBCIVICO X(18)
            /// </summary>
            [HisFieldInfoMapping(27, 18)]
            public string T_GP1TBCIVICO { get; set; }

            /// </summary>
            /// T_GP1TBFRAZIONE X(35)
            /// </summary>
            [HisFieldInfoMapping(28, 35)]
            public string T_GP1TBFRAZIONE { get; set; }

            /// </summary>
            /// T_GP1TBINDIRIZD X(52)
            /// </summary>
            [HisFieldInfoMapping(29, 52)]
            public string T_GP1TBINDIRIZD { get; set; }

            /// </summary>
            /// T_GP1TBCODCOM X
            /// </summary>
            [HisFieldInfoMapping(30, 4)]
            public string T_GP1TBCODCOM { get; set; }

            /// </summary>
            /// T_GP1TBCOMUNE X(37)
            /// </summary>
            [HisFieldInfoMapping(31, 37)]
            public string T_GP1TBCOMUNE { get; set; }

            /// </summary>
            /// T_GP1TBPROV X(3)
            /// </summary>
            [HisFieldInfoMapping(32, 3)]
            public string T_GP1TBPROV { get; set; }

            /// </summary>
            /// T_GP1TBCAP X(9)
            /// </summary>
            [HisFieldInfoMapping(33, 9)]
            public string T_GP1TBCAP { get; set; }

            /// </summary>
            /// T_GP1ABIFIDJ X(5)
            /// </summary>
            [HisFieldInfoMapping(34, 5)]
            public string T_GP1ABIFIDJ { get; set; }

            /// </summary>
            /// T_GP1CABFIDJ X
            /// </summary>
            [HisFieldInfoMapping(35, 7)]
            public string T_GP1CABFIDJ { get; set; }

            /// <summary>
            /// T_GP1PRESO 9(4)  
            /// </summary>
            [HisFieldInfoMapping(36, 4, CobolType = CobolType.Unsigned)]
            public short T_GP1PRESO { get; set; }

            /// <summary>
            /// T_GP1AAESO 9(4)  
            /// </summary>
            [HisFieldInfoMapping(37, 4, CobolType = CobolType.Unsigned)]
            public short T_GP1AAESO { get; set; }

            /// <summary>
            /// T_TABTRATTOT OCCURS 6.
            /// </summary>
            [HisComplexAreaInfoMapping(38, ListCount = 6)]
            public List<T_TABTRATTOT> LISTT_TABTRATTOT { get; set; }
            #endregion Tracciato Host
            #endregion Properties

            #region nested class
            public class T_TABTRATTOT
            {
                #region Properties

                #region Tracciato COBOL
                //04 T_TABTRATTOT OCCURS 6.	
                //   05 T_GESTOT	    PIC X(2).
                //   05 T_CONTRIB OCCURS 10.	
                //      06 T_ANNOTOT	PIC 9(4).
                //      06 T_TRATTOT	PIC S9(8)V9(7) COMP_3.
                //      06 T_CODTRAT    PIC X(3).
                #endregion Tracciato COBOL

                #region Tracciato Host
                /// </summary>
                /// T_GESTOT X(2)
                /// </summary>
                [HisFieldInfoMapping(0, 2)]
                public string T_GESTOT { get; set; }

                /// <summary>
                /// T_CONTRIB OCCURS 6.
                /// </summary>
                [HisComplexAreaInfoMapping(1, ListCount = 10)]
                public List<T_CONTRIB> LISTT_CONTRIB { get; set; }
                #endregion Tracciato Host

                #region nested class
                public class T_CONTRIB
                {
                    #region Properties

                    #region Tracciato COBOL
                    //   05 T_CONTRIB OCCURS 10.	
                    //      06 T_ANNOTOT	PIC 9(4).
                    //      06 T_TRATTOT	PIC S9(8)V9(7) COMP_3.
                    //      06 T_CODTRAT    PIC X(3).
                    #endregion Tracciato COBOL

                    #region Tracciato Host
                    /// <summary>
                    /// T_ANNOTOT 9(4)  
                    /// </summary>
                    [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
                    public short T_ANNOTOT { get; set; }

                    /// <summary>
                    //  T_TRATTOT PIC S9(8)V9(7) COMP_3.
                    /// <summary>
                    [HisFieldInfoMapping(1, 8, Scale = 7, CobolType = CobolType.Comp3)]
                    public decimal T_TRATTOT { get; set; }

                    /// </summary>
                    /// T_CODTRAT X(3)
                    /// </summary>
                    [HisFieldInfoMapping(2, 3)]
                    public string T_CODTRAT { get; set; }
                    #endregion Tracciato Host

                    #endregion Properties
                }
                #endregion nested class
                #endregion Properties
            }
            #endregion nested class
        }

        public class Dati2015
        {
            #region Properties

            #region Tracciato COBOL
            //02 T-DATI2015.
            //   03 T-GP1AV91E            PIC 9.
            //   03 T-GP1AV91F            PIC 9.
            //   03 T-GP1AD03.
            //      04 T-GP1AD03G         PIC 9(2).
            //      04 T-GP1AD03M         PIC 9(2).
            //      04 T-GP1AD03A         PIC 9(4).        
            //   03 T-GP2PCARM            PIC X.
            //   03 T-DATIINGR. 
            //      04 T-TABINGR OCCURS 2.         
            //         05 T-BUOINGR       PIC S9(7)V9(2) COMP-3.
            //         05 T-BUOUSCI       PIC S9(7)V9(2) COMP-3.
            #endregion Tracciato COBOL

            #region Tracciato Host
            /// <summary>
            /// T_GP1AV91E            PIC 9.
            /// <summary>
            [HisFieldInfoMapping(0, 1, CobolType = CobolType.Unsigned)]
            public short T_GP1AV91E { get; set; }

            /// <summary>
            /// T_GP1AV91F            PIC 9.
            /// <summary>
            [HisFieldInfoMapping(1, 1, CobolType = CobolType.Unsigned)]
            public short T_GP1AV91F { get; set; }

            /// <summary>
            /// T_GP1AD03G         PIC 9(2).
            /// <summary>
            [HisFieldInfoMapping(2, 2, CobolType = CobolType.Unsigned)]
            public short T_GP1AD03G { get; set; }

            /// <summary>
            /// T_GP1AD03M         PIC 9(2).
            /// <summary>
            [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
            public short T_GP1AD03M { get; set; }

            /// <summary>
            /// T_GP1AD03A         PIC 9(4).
            /// <summary>
            [HisFieldInfoMapping(4, 4, CobolType = CobolType.Unsigned)]
            public short T_GP1AD03A { get; set; }

            /// <summary>
            /// T_GP2PCARM            PIC X.
            /// <summary>
            [HisFieldInfoMapping(5, 1)]
            public string T_GP2PCARM { get; set; }

            /// <summary>
            /// T_TABINGR OCCURS 2.
            /// </summary>
            [HisComplexAreaInfoMapping(6, ListCount = 2)]
            public List<T_TABINGR> LISTT_TABINGR { get; set; }
            #endregion Tracciato Host
            #endregion Properties

            #region nested class
            public class T_TABINGR
            {
                #region Properties

                #region Tracciato COBOL
                //      04 T-TABINGR OCCURS 2.         
                //         05 T-BUOINGR       PIC S9(7)V9(2) COMP-3.
                //         05 T-BUOUSCI       PIC S9(7)V9(2) COMP-3.
                #endregion Tracciato COBOL

                #region Tracciato Host
                /// <summary>
                /// T_BUOINGR S9(7)V9(2) COMP-3 
                /// </summary>
                [HisFieldInfoMapping(0, 5, Scale = 2, CobolType = CobolType.Comp3)]
                public decimal T_BUOINGR { get; set; }

                /// <summary>
                /// T_BUOUSCI S9(7)V9(2) COMP-3 
                /// </summary>
                [HisFieldInfoMapping(1, 5, Scale = 2, CobolType = CobolType.Comp3)]
                public decimal T_BUOUSCI { get; set; }
                #endregion Tracciato Host

                #endregion Properties
            }
            #endregion nested class
        }

        public class Dati2016
        {
            #region Properties
            #region Tracciato COBOL
            //02	T-DATI2017		
            //03    T-GP1CENTINT	Cod. ente/fondo int.9(4)	N
            //03	T-DATI-DOMANDA
            //    04	T-GP1DGRP	Codice Gruppo       X(4)	A/N
            //    04	T-GP1DPRD	Codice Prodotto 	X(4)	A/N
            //    04    T-GP1DTIP	Codice Tipo			X(4)	A/N
            //    04	T-GP1DTIPOL	Codice Tipologia	X(4)	A/N
            //    04	T-GP1DFASE	Codice Fase			X(4)	A/N
            //    04	T-GP1DELFLG	Codici FLAG Elab.
            //    04 T-GP1FLAGS OCCURS 25.         
            //       05 T-GP1FLAG       PIC X.
            //    04	T-GP1DELIMP	Importo esito elab. S9(7)V9(4) COMP-3
            #endregion Tracciato COBOL
            #region Tracciato HOST
            /// <summary>
            /// T_GP1CENTINT         PIC 9(4).
            /// </summary>
            [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
            public short T_GP1CENTINT { get; set; }

            /// <summary>
            /// T_GP1DGRP            PIC X(4).
            /// </summary>
            [HisFieldInfoMapping(1, 4)]
            public string T_GP1DGRP { get; set; }

            /// <summary>
            /// T_GP1DPRD            PIC X(4).
            /// </summary>
            [HisFieldInfoMapping(2, 4)]
            public string T_GP1DPRD { get; set; }

            /// <summary>
            /// T_GP1DTIP            PIC X(4).
            /// </summary>
            [HisFieldInfoMapping(3, 4)]
            public string T_GP1DTIP { get; set; }

            /// <summary>
            /// T_GP1DTIPOL X(4)
            /// </summary>
            [HisFieldInfoMapping(4, 4)]
            public string T_GP1DTIPOL { get; set; }

            /// <summary>
            /// T_GP1DFASE            PIC X(4).
            /// </summary>
            [HisFieldInfoMapping(5, 4)]
            public string T_GP1DFASE { get; set; }

            /// <summary>
            /// T_GP1FLAGS OCCURS 25.
            /// </summary>
            [HisComplexAreaInfoMapping(6, ListCount = 25)]
            public List<T_GP1FLAGS> LISTGP1FLAGS { get; set; }

            /// <summary>
            /// T_GP1DELIMP S9(7)V9(4) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(7, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal T_GP1DELIMP { get; set; }
            #endregion Tracciato Host
            #endregion Properties

            #region nested class
            public class T_GP1FLAGS
            {
                #region Properties

                #region Tracciato COBOL
                //      04 T-GP1FLAGS OCCURS 25.         
                //         05 T-GP1FLAG       PIC X.
                #endregion Tracciato COBOL

                #region Tracciato Host
                /// <summary>
                /// T_GP1FLAG            PIC X.
                /// </summary>
                [HisFieldInfoMapping(0, 1)]
                public string T_GP1FLAG { get; set; }
                #endregion Tracciato Host

                #endregion Properties
            }
            #endregion
        }

        public class Dati2017
        {
            #region Properties
            #region Tracciato COBOL
            //  02	T-DATI2018.
            //      03  T-GP1CARPE  PIC X.
            //      03  T-GP1AV91C  PIC 9.
            //      03  T-SEDE-DOMANDA PIC X(6).
            //      03  T-DES-SEDE-DOMANDA PIC X(22).
            #endregion Tracciato COBOL
            #region Tracciato HOST
            /// <summary>
            /// T-GP1CARPE  PIC X.
            /// </summary>
            [HisFieldInfoMapping(0, 1)]
            public string T_GP1CARPE { get; set; }

            /// <summary>
            /// T-GP1AV91C  PIC 9.
            /// </summary>
            [HisFieldInfoMapping(1, 1)]
            public short T_GP1AV91C { get; set; }

            /// <summary>
            /// T-SEDE-DOMANDA    PIC X(6).
            /// </summary>
            [HisFieldInfoMapping(2, 6)]
            public string T_SEDE_DOMANDA { get; set; }

            /// <summary>
            /// T-DES-SEDE-DOMANDA    PIC X(22).
            /// </summary>
            [HisFieldInfoMapping(3, 22)]
            public string T_DES_SEDE_DOMANDA { get; set; }
            #endregion Tracciato Host
            #endregion Properties
        }

        public class Dati2018
        {
            #region Properties
            #region Tracciato COBOL
            //  02	T-DATI2019.
            //      03  T-GP1TPCLC  PIC X(8).
            #endregion Tracciato COBOL
            #region Tracciato HOST
            /// <summary>
            /// T-GP1TPCLC  PIC X(8).
            /// </summary>
            [HisFieldInfoMapping(0, 8)]
            public string T_GP1TPCLC { get; set; }
            #endregion Tracciato Host
            #endregion Properties
        }

        public class Dati2019
        {
            #region Properties
            #region Tracciato COBOL
            //      02  T-DATI2020
            //         03 T-GP1AJ10OLD   PIC X.
            //         03 T-GP1AJ10Z     PIC X(6).
            #endregion Tracciato COBOL
            #region Tracciato HOST
            /// <summary>
            /// T-GP1AJ10OLD  PIC X.
            /// </summary>
            [HisFieldInfoMapping(0, 1)]
            public string T_GP1AJ10OLD { get; set; }

            /// <summary>
            /// T-GP1AJ10Z  PIC X(6).
            /// </summary>
            [HisFieldInfoMapping(1, 6)]
            public string T_GP1AJ10Z { get; set; }
            #endregion Tracciato HOST
            #endregion Properties
        }

        public class Dati2020
        {
            #region Properties
            #region Tracciato COBOL
            //      02  T-DATI2021		 			
            //         03 T-GP1NUMDECR		PIC X(10)
            //         03 T-GP1DATDECR		PIC X(8)
            //         03 T-GP2BB10-UNICO   PIC 9(5)V(4) COMP-3
            #endregion Tracciato COBOL
            #region Tracciato HOST
            /// <summary>
            /// T-GP1NUMDECR		PIC X(10)
            /// </summary>
            [HisFieldInfoMapping(0, 10)]
            public string T_GP1NUMDECR { get; set; }

            /// <summary>
            /// T-GP1DATDECR		PIC X(8)
            /// </summary>
            [HisFieldInfoMapping(1, 8)]
            public string T_GP1DATDECR { get; set; }

            /// <summary>
            /// T-GP1DATDECR		PIC 9(5)V(4) COMP-3
            /// </summary>
            [HisFieldInfoMapping(2, 5, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal T_GP2BB10_UNICO { get; set; }
            #endregion Tracciato HOST
            #endregion Properties
        }

        public class Dati2021
        {
            #region Properties
            #region Tracciato COBOL
            //      02  T-DATI2022		 			
            //         03 T-GP1AN87E		PIC X(8)
            //         03 T-GP1AJTIPCUM		PIC X(1)
            #endregion Tracciato COBOL
            #region Tracciato HOST
            /// <summary>
            /// T-GP1AN87E		PIC X(8)
            /// </summary>
            [HisFieldInfoMapping(0, 8)]
            public string T_GP1AN87E { get; set; }

            /// <summary>
            /// T-GP1AJTIPCUM		PIC X(1)
            /// </summary>
            [HisFieldInfoMapping(1, 1)]
            public string T_GP1AJTIPCUM { get; set; }
            #endregion Tracciato HOST
            #endregion Properties
        }
        #endregion nested class

        #endregion Properties
    }
}
