using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostResponse.AreaStampa
{
    public class Codici_Procedura
    {
        #region Constructor
        internal Codici_Procedura()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 FILLER           PIC X(02).
        //*                            LIBERI                         2070
        //     02 T-CODPRO         PIC X(02).
        //*                            LIBERI                         2072
        //     02 COD-RISP         PIC 9(01).
        //*                            CODICE RISPOSTA DAL CALCOLO    2074
        //     02 COD-ERRORE       PIC X(03).
        //*                                                           2075
        //     02 COD-SEDE         PIC 9(04).
        //*                            COD.SEDE                       2078
        //     02 COD-CO           PIC 9(02).
        //*                            COD.CENTRO OPERATIVO           2082
        //     02 COD-IS           PIC X(02).
        //*                            COD.ISOLA                      2084
        //     02 MATR-OP          PIC 9(8).
        //*                            MATRICOLA OPERATORE            2086
        //     02 COD-ARCA1        PIC XXX.
        //*                            CODICE ARCA                    2094
        //     02 COD-ARCA2        PIC 9(08).
        //*                            CODICE ARCA.                   2097
        //     02 FLAG-EM          PIC X(02).
        //*                            FLAG EMETTERE COMUNICAZIONI    2105
        //     02 TE09             PIC 9(01).
        //*                            SE = 1 STAMPA TE09             2107
        //     02 TE10             PIC 9(01).
        //*                            SE = 1 STAMPA TE10             2108
        //     02 FILLER           PIC X(21).
        //*                            LIBERI                         2109
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// FILLER X(02)  
        /// </summary>
        [HisFieldInfoMapping(0, 2)]
        public string FILLER1 { get; set; }

        // *                            LIBERI                         2070
        /// <summary>
        /// T_CODPRO X(02)  
        /// </summary>
        [HisFieldInfoMapping(1, 2)]
        public string T_CODPRO { get; set; }

        // *                            LIBERI                         2072
        /// <summary>
        /// COD_RISP 9(01)  
        /// </summary>
        [HisFieldInfoMapping(2, 1, CobolType = CobolType.Unsigned)]
        public short COD_RISP { get; set; }

        // *                            CODICE RISPOSTA DAL CALCOLO    2074
        /// <summary>
        /// COD_ERRORE X(03)  
        /// </summary>
        [HisFieldInfoMapping(3, 3)]
        public string COD_ERRORE { get; set; }

        // *                                                           2075
        /// <summary>
        /// COD_SEDE 9(04)  
        /// </summary>
        [HisFieldInfoMapping(4, 4, CobolType = CobolType.Unsigned)]
        public short COD_SEDE { get; set; }

        // *                            COD.SEDE                       2078
        /// <summary>
        /// COD_CO 9(02)  
        /// </summary>
        [HisFieldInfoMapping(5, 2, CobolType = CobolType.Unsigned)]
        public short COD_CO { get; set; }

        // *                            COD.CENTRO OPERATIVO           2082
        /// <summary>
        /// COD_IS X(02)  
        /// </summary>
        [HisFieldInfoMapping(6, 2)]
        public string COD_IS { get; set; }

        // *                            COD.ISOLA                      2084
        /// <summary>
        /// MATR_OP 9(8)  
        /// </summary>
        [HisFieldInfoMapping(7, 8, CobolType = CobolType.Unsigned)]
        public int MATR_OP { get; set; }

        // *                            MATRICOLA OPERATORE            2086
        /// <summary>
        /// COD_ARCA1 XXX  
        /// </summary>
        [HisFieldInfoMapping(8, 3)]
        public string COD_ARCA1 { get; set; }

        // *                            CODICE ARCA                    2094
        /// <summary>
        /// COD_ARCA2 9(08)  
        /// </summary>
        [HisFieldInfoMapping(9, 8, CobolType = CobolType.Unsigned)]
        public int COD_ARCA2 { get; set; }

        // *                            CODICE ARCA.                   2097
        /// <summary>
        /// FLAG_EM X(02)  
        /// </summary>
        [HisFieldInfoMapping(10, 2)]
        public string FLAG_EM { get; set; }

        // *                            FLAG EMETTERE COMUNICAZIONI    2105
        /// <summary>
        /// TE09 9(01)  
        /// </summary>
        [HisFieldInfoMapping(11, 1, CobolType = CobolType.Unsigned)]
        public short TE09 { get; set; }

        // *                            SE = 1 STAMPA TE09             2107
        /// <summary>
        /// TE10 9(01)  
        /// </summary>
        [HisFieldInfoMapping(12, 1, CobolType = CobolType.Unsigned)]
        public short TE10 { get; set; }

        // *                            SE = 1 STAMPA TE10             2108
        /// <summary>
        /// FILLER X(21)  
        /// </summary>
        [HisFieldInfoMapping(13, 21)]
        public string FILLER2 { get; set; }

        // *                            LIBERI                         2109
        #endregion Tracciato Host

        #region nested class

        #endregion nested class

        #endregion Properties
    }
}
