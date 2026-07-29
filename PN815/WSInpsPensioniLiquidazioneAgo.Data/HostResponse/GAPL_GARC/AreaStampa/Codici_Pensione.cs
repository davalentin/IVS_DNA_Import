using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostResponse.AreaStampa
{
    public class Codici_Pensione
    {
        #region Constructor
        internal Codici_Pensione()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 NAT-PENS.
        //        03 NAT-PENS1     PIC X(01).
        //        03 NAT-PENS2     PIC X(01).
        //        03 NAT-PENS3     PIC X(01).
        //*                            NATURA PENSIONE                1860
        //     02 FILLER           PIC X(01).
        //*                                                           1863
        //     02 CONF-INVAL       PIC 9(01).
        //*                            CONFERMA INVAL.(GP1AV72)       1864
        //*        0 = 1° RICONOSC. 2 = 2° RICONOSC. - 3 = DEFINITIVO
        //     02 TIPO-CALC        PIC X(01).
        //*                            GP1AF03
        //*       1 = VECCHIO SISTEMA CONTRIBUTIVO                    1865
        //*       2 = RETRIBUTIVA
        //*       3 = RETRIBUTIVA PER OPZIONE ART.13 L.153/69
        //*       4 = RETRIBUTIVA PER OPZIONE ART.14 D.P.R. 488/68
        //*       5 = RILIQUIDAZIONE PENSIONE ANTICIPATA ART.22 L.153/69
        //*       6 = PENSIONE LAVORATORI AUT.CALCOLATA EX LEGE 638/1983
        //*       7 = PENS.VECCHIAIA DA TRASF.ASS.INVAL.CON MAGGIOR IMP.
        //*       8 = PENSIONE CONTRIBUTIVA EX LEGE N.335/1995
        //*       9 = PENSIONE MISTA
        //*       Z = CONTRIBUTIVO PER OPZIONE
        //*
        //     02 LIQ-PROVV        PIC X(01).
        //*                            CODICE LIQ.PROVV.(GP1AZ11E)    1866
        //* PENSIONESUBITO   PV/LP1
        //*       4            0     = PENS.LIQ.CON ACQUISIZ.NORMALE
        //*       5            1     = PROVVISORIA CON CAMPO 10
        //*       6            2     = PROVVISORIA CON CAMPI 11 E 12
        //*       7            3     = DEFINITIVA DA LP1
        //*       9            8     = PROVVISORIA - IVS74
        //*
        //     02 COD-DETR.
        //        03 DETR-1        PIC 9.
        //        03 DETR-2        PIC 9.
        //        03 DETR-3        PIC 9.
        //        03 DETR-4        PIC 9.
        //        03 DETR-5        PIC 9.
        //        03 DETR-67       PIC 99.
        //        03 DETR-89       PIC 99.
        //        03 DETR-1011     PIC 99.
        //        03 DETR-12       PIC 9.
        //        03 DETR-13       PIC 9.
        //        03 DETR-14       PIC 9.
        //*       GP3CDTI              CODICI DETRAZIONE D'IMPOSTA    1867
        //     02 COD-ELIM         PIC 9(01).
        //*                            FLAG ELIMINAZIONE (GP1AM01)    1881
        //     02 COD-INVAL        PIC X(01).
        //*                            CODICE INVALIDITA'(GP1AV12)    1882
        //     02 COD-INVCIV       PIC 9(02).
        //*                            CODICE FASCIA (GP2IC12)        1883
        //     02 DEC-INVCIV       PIC 9(08).
        //*                            DECORRENZA (GGMMAAAA)          1885
        //     02 COD-PSAS         PIC 9(01).
        //*                            CODICE PENSIONI SOCIALI        1893
        //     02 PROV-PENS        PIC 9(01).
        //*                            PROV.PENSIONE (GP7LC04)        1894
        //*                            0 = DA ASSIC. - 1 = DA PENSIONE
        //     02 ALIQ-SUPERS.
        //        03 ALIQ-INT      PIC 99.
        //        03 ALIQ-DEC      PIC 99.
        //*                            ALIQUOTA SUPERSTITI            1895
        //     02 FS13             PIC 9(01).
        //*                            FLAG FONDI SPECIALI            1899
        //     02 F-CATLIQ         PIC X(02).
        //*                            FLAG FONDI SPECIALI            1900
        //     02 F-NOCALC         PIC 9(01).
        //*                            FONDI SPECIALI                 1902
        //*                            ( = 1 NON STAMPA DATI CALCOLO)
        //     02 F-EXCOMB         PIC 9(01).
        //*                            FONDI SPECIALI                 1903
        //*                            ( = 1 ART.6 L.140)
        //*                            ( = 2 L.336 )
        //     02 F-L407           PIC 9(01).
        //*                            FONDI SPECIALI                 1904
        //*                            ( = 1 L.407 )
        //     02 F-SUPPL          PIC 9(01).
        //*                            FONDI SPECIALI                 1905
        //     02 F-RILIQ          PIC 9(01).
        //*                            FONDI SPECIALI                 1906
        //     02 F-CONSOL         PIC 9(01).
        //*                            FONDI SPECIALI                 1907
        //*                            ( = 1 CONTRIBUTO SOLIDARIETA' PI)
        //     02 TIPO-FS          PIC X(01).
        //*       GP1AV37N             FONDI SPECIALI                 1908
        //*                            (CODICE SPECIFICO TIPO PENSIONE)
        //     02 GP1-ALA1.
        //        03 ALA1-INT      PIC 99.
        //        03 ALA1-DEC      PIC 99.
        //*                                                           1909
        //     02 F-TRT-SOL        PIC 9(01).
        //*                            TRATTENUTE DI SOLIDARIETÀ      1913
        //     02 FL-ELIM          PIC 9(01).
        //*                            FLAG ELIMINAZIONE CONTESTUALE  1914
        //     02 GP2IC23Z         PIC 9.
        //*                            CODICE RICOVERO                1915
        //     02 F-SOSP           PIC 9.
        //*                            ( = 1 SOSPESA POSTEL )         1916
        //     02 GP3CDST          PIC X(01).
        //*                            GP3CUD-CDST SE = E ESODATI     1917
        //     02 FILLER           PIC X(02).
        //*                                                           1916
        #endregion Tracciato COBOL

        #region Tracciato Host
        // 02 NAT-PENS.
        /// <summary>
        /// NAT_PENS1 X(01)  
        /// </summary>
        [HisFieldInfoMapping(0, 1)]
        public string NAT_PENS1 { get; set; }

        /// <summary>
        /// NAT_PENS2 X(01)  
        /// </summary>
        [HisFieldInfoMapping(1, 1)]
        public string NAT_PENS2 { get; set; }

        /// <summary>
        /// NAT_PENS3 X(01)  
        /// </summary>
        [HisFieldInfoMapping(2, 1)]
        public string NAT_PENS3 { get; set; }

        // *                            NATURA PENSIONE                1860
        /// <summary>
        /// FILLER X(01)  
        /// </summary>
        [HisFieldInfoMapping(3, 1)]
        public string FILLER { get; set; }

        // *                                                           1863
        /// <summary>
        /// CONF_INVAL 9(01)  
        /// </summary>
        [HisFieldInfoMapping(4, 1, CobolType = CobolType.Unsigned)]
        public short CONF_INVAL { get; set; }

        // *                            CONFERMA INVAL.(GP1AV72)       1864
        // *        0 = 1° RICONOSC. 2 = 2° RICONOSC. - 3 = DEFINITIVO
        /// <summary>
        /// TIPO_CALC X(01)  
        /// </summary>
        [HisFieldInfoMapping(5, 1)]
        public string TIPO_CALC { get; set; }

        // *                            GP1AF03
        // *       1 = VECCHIO SISTEMA CONTRIBUTIVO                    1865
        // *       2 = RETRIBUTIVA
        // *       3 = RETRIBUTIVA PER OPZIONE ART.13 L.153/69
        // *       4 = RETRIBUTIVA PER OPZIONE ART.14 D.P.R. 488/68
        // *       5 = RILIQUIDAZIONE PENSIONE ANTICIPATA ART.22 L.153/69
        // *       6 = PENSIONE LAVORATORI AUT.CALCOLATA EX LEGE 638/1983
        // *       7 = PENS.VECCHIAIA DA TRASF.ASS.INVAL.CON MAGGIOR IMP.
        // *       8 = PENSIONE CONTRIBUTIVA EX LEGE N.335/1995
        // *       9 = PENSIONE MISTA
        // *       Z = CONTRIBUTIVO PER OPZIONE
        //*
        /// <summary>
        /// LIQ_PROVV X(01)  
        /// </summary>
        [HisFieldInfoMapping(6, 1)]
        public string LIQ_PROVV { get; set; }

        // *                            CODICE LIQ.PROVV.(GP1AZ11E)    1866
        // * PENSIONESUBITO   PV/LP1
        // *       4            0     = PENS.LIQ.CON ACQUISIZ.NORMALE
        // *       5            1     = PROVVISORIA CON CAMPO 10
        // *       6            2     = PROVVISORIA CON CAMPI 11 E 12
        // *       7            3     = DEFINITIVA DA LP1
        // *       9            8     = PROVVISORIA - IVS74
        //*
        // 02 COD-DETR.
        /// <summary>
        /// DETR_1 9  
        /// </summary>
        [HisFieldInfoMapping(7, 1, CobolType = CobolType.Unsigned)]
        public short DETR_1 { get; set; }

        /// <summary>
        /// DETR_2 9  
        /// </summary>
        [HisFieldInfoMapping(8, 1, CobolType = CobolType.Unsigned)]
        public short DETR_2 { get; set; }

        /// <summary>
        /// DETR_3 9  
        /// </summary>
        [HisFieldInfoMapping(9, 1, CobolType = CobolType.Unsigned)]
        public short DETR_3 { get; set; }

        /// <summary>
        /// DETR_4 9  
        /// </summary>
        [HisFieldInfoMapping(10, 1, CobolType = CobolType.Unsigned)]
        public short DETR_4 { get; set; }

        /// <summary>
        /// DETR_5 9  
        /// </summary>
        [HisFieldInfoMapping(11, 1, CobolType = CobolType.Unsigned)]
        public short DETR_5 { get; set; }

        /// <summary>
        /// DETR_67 99  
        /// </summary>
        [HisFieldInfoMapping(12, 2, CobolType = CobolType.Unsigned)]
        public short DETR_67 { get; set; }

        /// <summary>
        /// DETR_89 99  
        /// </summary>
        [HisFieldInfoMapping(13, 2, CobolType = CobolType.Unsigned)]
        public short DETR_89 { get; set; }

        /// <summary>
        /// DETR_1011 99  
        /// </summary>
        [HisFieldInfoMapping(14, 2, CobolType = CobolType.Unsigned)]
        public short DETR_1011 { get; set; }

        /// <summary>
        /// DETR_12 9  
        /// </summary>
        [HisFieldInfoMapping(15, 1, CobolType = CobolType.Unsigned)]
        public short DETR_12 { get; set; }

        /// <summary>
        /// DETR_13 9  
        /// </summary>
        [HisFieldInfoMapping(16, 1, CobolType = CobolType.Unsigned)]
        public short DETR_13 { get; set; }

        /// <summary>
        /// DETR_14 9  
        /// </summary>
        [HisFieldInfoMapping(17, 1, CobolType = CobolType.Unsigned)]
        public short DETR_14 { get; set; }

        // *       GP3CDTI              CODICI DETRAZIONE D'IMPOSTA    1867
        /// <summary>
        /// COD_ELIM 9(01)  
        /// </summary>
        [HisFieldInfoMapping(18, 1, CobolType = CobolType.Unsigned)]
        public short COD_ELIM { get; set; }

        // *                            FLAG ELIMINAZIONE (GP1AM01)    1881
        /// <summary>
        /// COD_INVAL X(01)  
        /// </summary>
        [HisFieldInfoMapping(19, 1)]
        public string COD_INVAL { get; set; }

        // *                            CODICE INVALIDITA'(GP1AV12)    1882
        /// <summary>
        /// COD_INVCIV 9(02)  
        /// </summary>
        [HisFieldInfoMapping(20, 2, CobolType = CobolType.Unsigned)]
        public short COD_INVCIV { get; set; }

        // *                            CODICE FASCIA (GP2IC12)        1883
        /// <summary>
        /// DEC_INVCIV 9(08)  
        /// </summary>
        [HisFieldInfoMapping(21, 8, CobolType = CobolType.Unsigned)]
        public int DEC_INVCIV { get; set; }

        // *                            DECORRENZA (GGMMAAAA)          1885
        /// <summary>
        /// COD_PSAS 9(01)  
        /// </summary>
        [HisFieldInfoMapping(22, 1, CobolType = CobolType.Unsigned)]
        public short COD_PSAS { get; set; }

        // *                            CODICE PENSIONI SOCIALI        1893
        /// <summary>
        /// PROV_PENS 9(01)  
        /// </summary>
        [HisFieldInfoMapping(23, 1, CobolType = CobolType.Unsigned)]
        public short PROV_PENS { get; set; }

        // *                            PROV.PENSIONE (GP7LC04)        1894
        // *                            0 = DA ASSIC. - 1 = DA PENSIONE
        // 02 ALIQ-SUPERS.
        /// <summary>
        /// ALIQ_INT 99  
        /// </summary>
        [HisFieldInfoMapping(24, 2, CobolType = CobolType.Unsigned)]
        public short ALIQ_INT { get; set; }

        /// <summary>
        /// ALIQ_DEC 99  
        /// </summary>
        [HisFieldInfoMapping(25, 2, CobolType = CobolType.Unsigned)]
        public short ALIQ_DEC { get; set; }

        // *                            ALIQUOTA SUPERSTITI            1895
        /// <summary>
        /// FS13 9(01)  
        /// </summary>
        [HisFieldInfoMapping(26, 1, CobolType = CobolType.Unsigned)]
        public short FS13 { get; set; }

        // *                            FLAG FONDI SPECIALI            1899
        /// <summary>
        /// F_CATLIQ X(02)  
        /// </summary>
        [HisFieldInfoMapping(27, 2)]
        public string F_CATLIQ { get; set; }

        // *                            FLAG FONDI SPECIALI            1900
        /// <summary>
        /// F_NOCALC 9(01)  
        /// </summary>
        [HisFieldInfoMapping(28, 1, CobolType = CobolType.Unsigned)]
        public short F_NOCALC { get; set; }

        // *                            FONDI SPECIALI                 1902
        // *                            ( = 1 NON STAMPA DATI CALCOLO)
        /// <summary>
        /// F_EXCOMB 9(01)  
        /// </summary>
        [HisFieldInfoMapping(29, 1, CobolType = CobolType.Unsigned)]
        public short F_EXCOMB { get; set; }

        // *                            FONDI SPECIALI                 1903
        // *                            ( = 1 ART.6 L.140)
        // *                            ( = 2 L.336 )
        /// <summary>
        /// F_L407 9(01)  
        /// </summary>
        [HisFieldInfoMapping(30, 1, CobolType = CobolType.Unsigned)]
        public short F_L407 { get; set; }

        // *                            FONDI SPECIALI                 1904
        // *                            ( = 1 L.407 )
        /// <summary>
        /// F_SUPPL 9(01)  
        /// </summary>
        [HisFieldInfoMapping(31, 1, CobolType = CobolType.Unsigned)]
        public short F_SUPPL { get; set; }

        // *                            FONDI SPECIALI                 1905
        /// <summary>
        /// F_RILIQ 9(01)  
        /// </summary>
        [HisFieldInfoMapping(32, 1, CobolType = CobolType.Unsigned)]
        public short F_RILIQ { get; set; }

        // *                            FONDI SPECIALI                 1906
        /// <summary>
        /// F_CONSOL 9(01)  
        /// </summary>
        [HisFieldInfoMapping(33, 1, CobolType = CobolType.Unsigned)]
        public short F_CONSOL { get; set; }

        // *                            FONDI SPECIALI                 1907
        // *                            ( = 1 CONTRIBUTO SOLIDARIETA' PI)
        /// <summary>
        /// TIPO_FS X(01)  
        /// </summary>
        [HisFieldInfoMapping(34, 1)]
        public string TIPO_FS { get; set; }

        // *       GP1AV37N             FONDI SPECIALI                 1908
        // *                            (CODICE SPECIFICO TIPO PENSIONE)
        // 02 GP1-ALA1.
        /// <summary>
        /// ALA1_INT 99  
        /// </summary>
        [HisFieldInfoMapping(35, 2, CobolType = CobolType.Unsigned)]
        public short ALA1_INT { get; set; }

        /// <summary>
        /// ALA1_DEC 99  
        /// </summary>
        [HisFieldInfoMapping(36, 2, CobolType = CobolType.Unsigned)]
        public short ALA1_DEC { get; set; }

        // *                                                           1909
        /// <summary>
        /// F_TRT_SOL 9(01)  
        /// </summary>
        [HisFieldInfoMapping(37, 1, CobolType = CobolType.Unsigned)]
        public short F_TRT_SOL { get; set; }

        // *                            TRATTENUTE DI SOLIDARIETÀ      1913
        /// <summary>
        /// FL_ELIM 9(01)  
        /// </summary>
        [HisFieldInfoMapping(38, 1, CobolType = CobolType.Unsigned)]
        public short FL_ELIM { get; set; }

        // *                            FLAG ELIMINAZIONE CONTESTUALE  1914
        /// <summary>
        /// GP2IC23Z 9  
        /// </summary>
        [HisFieldInfoMapping(39, 1, CobolType = CobolType.Unsigned)]
        public short GP2IC23Z { get; set; }

        // *                            CODICE RICOVERO                1915
        /// <summary>
        /// F_SOSP 9  
        /// </summary>
        [HisFieldInfoMapping(40, 1, CobolType = CobolType.Unsigned)]
        public short F_SOSP { get; set; }

        // *                            ( = 1 SOSPESA POSTEL )         1916
        /// <summary>
        /// GP3CDST X(01)  
        /// </summary>
        [HisFieldInfoMapping(41, 1)]
        public string GP3CDST { get; set; }

        // *                            GP3CUD-CDST SE = E ESODATI     1917
        /// <summary>
        /// FILLER X(02)  
        /// </summary>
        [HisFieldInfoMapping(42, 2)]
        public string FILLER2 { get; set; }

        // *                                                           1916
        #endregion Tracciato Host

        #region nested class

        #endregion nested class

        #endregion Properties
    }
}

