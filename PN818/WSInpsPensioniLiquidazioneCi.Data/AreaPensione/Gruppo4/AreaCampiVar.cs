using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaCampiVar
    {
        #region tracciato COBOL
        //  04  CAMPI-VAR.                                                       
        //         05  AGG-QE-DB.                                                   
        //             10 AGG-QE     OCCURS 6.                                      
        //                15 AGG-QE-PROC OCCURS 50  PIC X(1).                      
        //*CODICE PROCEDURA CHE AGGIORNA LA QUOTA ESTERA:                           
        //*A=ELABORAZIONE DEC.105 O LG.335 (DA APES)                                
        //*B=ELABORAZIONE REDDITUALE                                                
        //*C=DA SEDE: PRIMA LIQUIDAZIONE E RICOSTITUZIONE                           
        //*D=                                                                       
        //*E=                                                                       
        //*F=                                                                       
        //*                    05  CAMPI-STAMPA-MOD.
        //CG2015             10 STAMOD-X      OCCURS 6   PIC X.
        //      * VALORI DI STAMOD-X:
        //      * 1 = E210
        //      * 2 = E211
        //      * 3 = CI32
        //      * 4 = CI35
        //      *
        //      *LOMAR 23/10/2015 - I

        //CG2015****     05  DISPONIBILE          PIC X(2851).   (-6)
        //CG2015*        05  DISPONIBILE          PIC X(2845).

        //      * Occorre inserire per competenza 2016
        //      * FLAG2016    PIC 9(2)            =  2 bytes      =  2 (=16 se valorizzati i campi)
        //      * GP7LI01E    PIC S9(7)V9(4) C-3  =  6 bytes      =  6
        //      * GP7LI02E    PIC S9(7)V9(4) C-3  =  6 bytes      =  6
        //      * GP7LI03E    PIC S9(7)V9(4) C-3  =  6 bytes      =  6
        //      * GP2BC08     PIC S9(5) C-3       =  3 bytes x 16 = 48
        //      *                 TOTALE ------------------------>  68                 
        //      **********************************************************

        //               05 DATI-2016
        //                  06 FLAG2016           PIC 99.
        //                  06 GP7LE01E           PIC S9(7)V9(4) COMP-3.
        //                  06 GP7LE02E           PIC S9(7)V9(4) COMP-3.
        //                  06 GP7LE03E           PIC S9(7)V9(4) COMP-3.
        //                  06 GP2BC10OBGA        PIC S9(5) COMP-3.
        //                  06 GP2BC10OBGB        PIC S9(5) COMP-3.
        //                  06 GP2BC10ARTA        PIC S9(5) COMP-3.
        //                  06 GP2BC10ARTB        PIC S9(5) COMP-3.
        //                  06 GP2BC10COMA        PIC S9(5) COMP-3.
        //                  06 GP2BC10COMB        PIC S9(5) COMP-3.
        //                  06 GP2BC10CDMA        PIC S9(5) COMP-3.
        //                  06 GP2BC10CDMB        PIC S9(5) COMP-3.
        //                  06 GP2BC101           PIC S9(5) COMP-3.
        //                  06 GP2BC102           PIC S9(5) COMP-3.
        //                  06 GP2BC103           PIC S9(5) COMP-3.
        //                  06 GP2BC104           PIC S9(5) COMP-3.
        //                  06 GP2BC105           PIC S9(5) COMP-3.            
        //                  06 GP2BC106           PIC S9(5) COMP-3.
        //                  06 GP2BC107           PIC S9(5) COMP-3.
        //                  06 GP2BC108           PIC S9(5) COMP-3.
        //             04 DATI-2017. 
        //*    AREA DEI REDDITI PER LAVORO AUTONOMO (700 bytes)
        //        05 AREA-WKAUT.
        //           10 IELWKAUT OCCURS 50.
        //              15 IWAUTDEC         PIC 9999.
        //*             ANNO RED.DA LAVORO AUTONOMO
        //              15 IWAUTRED         PIC S9(7)V9(4)   COMP-3.
        //*             EURO +RED. ANNUO IN MIGLIAIA
        //              15 IWAUTDAL-AL.
        //                 20 IWAUTDALM     PIC 99.
        //*                DAL MESE  DI LAVORO AUTONOMO
        //                 20 IWAUTALM      PIC 99.
        //*                AL MESE  DI LAVORO AUTONOMO

        //*    AREA DEI REDDITI PER 240   (500 Bytes)
        //        05 AREAW240.
        //           10 ELEMENTO240 OCCURS 50.
        //              15 I240DEC          PIC 9999.
        //*             ANNO DEL REDDITO PER 240
        //              15 I240RED          PIC S9(7)V9(4)   COMP-3.
        //*             EURO REDDITO PER 240

        //        05 DATI-DOMANDA.
        //           10 GP1DGRP             PIC X(4).
        //           10 GP1DPRD             PIC X(4).
        //           10 GP1DTIP             PIC X(4).           
        //           10 GP1DTIPOL           PIC X(4).
        //           10 GP1DFASE            PIC X(4).

        //        05 GP1DELFLG              PIC X(25).
        //        05 GP1ELIMP               PIC S9(7)V9(4) COMP-3.
        //        05 GP1CENTINT             PIC 9(4).    
        //        05 TP1PR                  PIC X(3).
        //              05  FILLER2017_3          PIC X(1519).
        #endregion tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, ListCount = 6)]
        public List<QuotaEstera> QUOTEESTERE { get; set; }

        [HisComplexAreaInfoMapping(1, ListCount = 6)]
        public List<ModelloStampa> MODELLISTAMPA { get; set; }

        [HisComplexAreaInfoMapping(2)]
        public Dati2016 Dati_2016 { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class QuotaEstera
        {
            #region tracciato COBOL
            //  04  CAMPI-VAR.                                                       
            //         05  AGG-QE-DB.                                                   
            //             10 AGG-QE     OCCURS 6.                                      
            //                15 AGG-QE-PROC OCCURS 50  PIC X(1).                      
            //*CODICE PROCEDURA CHE AGGIORNA LA QUOTA ESTERA:                           
            //*A=ELABORAZIONE DEC.105 O LG.335 (DA APES)                                
            //*B=ELABORAZIONE REDDITUALE                                                
            //*C=DA SEDE: PRIMA LIQUIDAZIONE E RICOSTITUZIONE                           
            //*D=                                                                       
            //*E=                                                                       
            //*F=                                                                       
            //*  
            #endregion tracciato COBOL

            #region Tracciato Host
            [HisComplexAreaInfoMapping(0, ListCount = 50)]
            public List<FlagQuotaEstera> FLAGQUOTAESTERA { get; set; }
            #endregion Tracciato Host

            #region nested class
            public class FlagQuotaEstera
            {
                #region tracciato COBOL
                //15 AGG-QE-PROC OCCURS 50  PIC X(1).                      
                //*CODICE PROCEDURA CHE AGGIORNA LA QUOTA ESTERA:                           
                //*A=ELABORAZIONE DEC.105 O LG.335 (DA APES)                                
                //*B=ELABORAZIONE REDDITUALE                                                
                //*C=DA SEDE: PRIMA LIQUIDAZIONE E RICOSTITUZIONE                           
                //*D=                                                                       
                //*E=                                                                       
                //*F=                                                                       
                //*  
                #endregion tracciato COBOL

                #region Tracciato Host
                // 04  CAMPI-VAR.
                // 05  AGG-QE-DB.
                // 10 AGG-QE     OCCURS 6.
                /// <summary>
                /// AGG_QE_PROC X(1)  
                /// *CODICE PROCEDURA CHE AGGIORNA LA QUOTA ESTERA:
                /// *A=ELABORAZIONE DEC.105 O LG.335 (DA APES)
                /// *B=ELABORAZIONE REDDITUALE
                /// *C=DA SEDE: PRIMA LIQUIDAZIONE E RICOSTITUZIONE
                /// *D=
                /// *E=
                /// *F=
                /// </summary>
                [HisFieldInfoMapping(0, 1)]
                public string AGG_QE_PROC { get; set; }

                #endregion Tracciato Host
            }
            #endregion nested class
        }

        public class ModelloStampa
        {
            #region tracciato COBOL
            //         05  CAMPO-STAMPA-MOD.                                      
            //             10 STAMOD-X OCCURS 6  PIC X(1).
            #endregion tracciato COBOL

            #region Tracciato Host
            [HisFieldInfoMapping(0, 1)]
            public string STAMOD_X { get; set; }
            #endregion Tracciato Host
        }

        public class Dati2016
        {
            #region tracciato COBOL

            //05 DATI-2016
            //   06 FLAG2016           PIC 99.
            //   06 GP7LE01E           PIC S9(7)V9(4) COMP-3.
            //   06 GP7LE02E           PIC S9(7)V9(4) COMP-3.
            //   06 GP7LE03E           PIC S9(7)V9(4) COMP-3.
            //   06 GP2BC10OBGA        PIC S9(5) COMP-3.
            //   06 GP2BC10OBGB        PIC S9(5) COMP-3.
            //   06 GP2BC10ARTA        PIC S9(5) COMP-3.
            //   06 GP2BC10ARTB        PIC S9(5) COMP-3.
            //   06 GP2BC10COMA        PIC S9(5) COMP-3.
            //   06 GP2BC10COMB        PIC S9(5) COMP-3.
            //   06 GP2BC10CDMA        PIC S9(5) COMP-3.
            //   06 GP2BC10CDMB        PIC S9(5) COMP-3.
            //   06 GP2BC101           PIC S9(5) COMP-3.
            //   06 GP2BC102           PIC S9(5) COMP-3.
            //   06 GP2BC103           PIC S9(5) COMP-3.
            //   06 GP2BC104           PIC S9(5) COMP-3.
            //   06 GP2BC105           PIC S9(5) COMP-3.
            //   06 GP2BC106           PIC S9(5) COMP-3.
            //   06 GP2BC107           PIC S9(5) COMP-3.
            //   06 GP2BC108           PIC S9(5) COMP-3.
            #endregion tracciato COBOL

            #region Tracciato Host
            [HisFieldInfoMapping(0, 2)]
            public string FLAG2016 { get; set; }

            [HisFieldInfoMapping(1, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal GP7LE01E { get; set; }

            [HisFieldInfoMapping(2, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal GP7LE02E { get; set; }

            [HisFieldInfoMapping(3, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal GP7LE03E { get; set; }

            [HisFieldInfoMapping(4, 3, CobolType = CobolType.Comp3)]
            public int GP2BC10OBGA { get; set; }

            [HisFieldInfoMapping(5, 3, CobolType = CobolType.Comp3)]
            public int GP2BC10OBGB { get; set; }

            [HisFieldInfoMapping(6, 3, CobolType = CobolType.Comp3)]
            public int GP2BC10ARTA { get; set; }

            [HisFieldInfoMapping(7, 3, CobolType = CobolType.Comp3)]
            public int GP2BC10ARTB { get; set; }

            [HisFieldInfoMapping(8, 3, CobolType = CobolType.Comp3)]
            public int GP2BC10COMA { get; set; }

            [HisFieldInfoMapping(9, 3, CobolType = CobolType.Comp3)]
            public int GP2BC10COMB { get; set; }

            [HisFieldInfoMapping(10, 3, CobolType = CobolType.Comp3)]
            public int GP2BC10CDMA { get; set; }

            [HisFieldInfoMapping(11, 3, CobolType = CobolType.Comp3)]
            public int GP2BC10CDMB { get; set; }

            [HisFieldInfoMapping(12, 3, CobolType = CobolType.Comp3)]
            public int GP2BC101 { get; set; }

            [HisFieldInfoMapping(13, 3, CobolType = CobolType.Comp3)]
            public int GP2BC102 { get; set; }

            [HisFieldInfoMapping(14, 3, CobolType = CobolType.Comp3)]
            public int GP2BC103 { get; set; }

            [HisFieldInfoMapping(15, 3, CobolType = CobolType.Comp3)]
            public int GP2BC104 { get; set; }

            [HisFieldInfoMapping(16, 3, CobolType = CobolType.Comp3)]
            public int GP2BC105 { get; set; }

            [HisFieldInfoMapping(17, 3, CobolType = CobolType.Comp3)]
            public int GP2BC106 { get; set; }

            [HisFieldInfoMapping(18, 3, CobolType = CobolType.Comp3)]
            public int GP2BC107 { get; set; }

            [HisFieldInfoMapping(19, 3, CobolType = CobolType.Comp3)]
            public int GP2BC108 { get; set; }
            #endregion Tracciato Host
        }


        #endregion nested class
    }
}
