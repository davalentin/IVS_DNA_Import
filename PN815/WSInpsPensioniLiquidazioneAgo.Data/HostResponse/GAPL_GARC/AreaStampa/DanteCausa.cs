using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.HostResponse.AreaStampa
{
    public class DanteCausa
    {
        #region Constructor
        internal DanteCausa()
        {

        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        //     02 COGN-DANTE       PIC X(36).
        //*                             COGNOME DANTE CAUSA           1550
        //     02 NOME-DANTE       PIC X(36).
        //*                             NOME DANTE CAUSA              1586
        //     02 SEX-DANTE        PIC X(01).
        //*                             SESSO                         1622
        //     02 DNASC-DANTE.
        //*                             DATA NASC.DANTE CAUSA         1623
        //        03 DNT-GG        PIC 9(02).
        //*                             GG
        //        03 DNT-MM        PIC 9(02).
        //*                             MM
        //        03 DNT-AA        PIC 9(04).
        //*                             AAAA
        //     02 COD-FISC-DANTE   PIC X(16).
        //*                             COD.FISC.DANTE CAUSA          1631
        //     02 SED-DANTE        PIC 9(04).
        //*                             SEDE DANTE-CAUSA              1647
        //     02 CAT-DANTE        PIC 9(03).
        //*                             CATEG.DANTE-CAUSA             1651
        //     02 CERT-DANTE       PIC 9(08).
        //*                             CERT.DANTE-CAUSA              1654
        //     02 DT-DEC-DANTE     PIC 9(06).
        //*                             DATA DECESSO DANTE (AAAAMM)   1662
        //     02 IMP-PENS-DANTE   PIC S9(7)V9(4) COMP-3.
        //*                             IMP.PENS.DANTE CAUSA          1668
        //     02 SIGLA-DANTE      PIC X(02).
        //*                             R4                            1674
        //     02 CAT-DANTE-FONDI  PIC X(03).
        //*                             CATEG.DANTE-CAUSA IN CHIARO   1676
        //     02 CATEGO-DANTE     PIC X(08).
        //*                             CATEGORIA IN CHIARO (AGO)     1679
        //     02 FILLER           PIC X(02).
        //*                             LIBERI                        1687
        //     02 GP1AVCRD.
        //       03 GP1CRD1          PIC 9(02).
        //       03 GP1CRD2          PIC 9(02).
        //*                           GP1CENTCRD SE 99 = EX DIP.MON.  1689
        //     02 FILLER           PIC X(07).
        //*                             LIBERI                        1693
        #endregion Tracciato COBOL

        #region Tracciato Host
        /// <summary>
        /// COGN_DANTE X(36)  
        /// </summary>
        [HisFieldInfoMapping(0, 36)]
        public string COGN_DANTE { get; set; }

        // *                             COGNOME DANTE CAUSA           1550
        /// <summary>
        /// NOME_DANTE X(36)  
        /// </summary>
        [HisFieldInfoMapping(1, 36)]
        public string NOME_DANTE { get; set; }

        // *                             NOME DANTE CAUSA              1586
        /// <summary>
        /// SEX_DANTE X(01)  
        /// </summary>
        [HisFieldInfoMapping(2, 1)]
        public string SEX_DANTE { get; set; }

        // *                             SESSO                         1622
        // 02 DNASC-DANTE.
        // *                             DATA NASC.DANTE CAUSA         1623
        /// <summary>
        /// DNT_GG 9(02)  
        /// </summary>
        [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
        public short DNT_GG { get; set; }

        // *                             GG
        /// <summary>
        /// DNT_MM 9(02)  
        /// </summary>
        [HisFieldInfoMapping(4, 2, CobolType = CobolType.Unsigned)]
        public short DNT_MM { get; set; }

        // *                             MM
        /// <summary>
        /// DNT_AA 9(04)  
        /// </summary>
        [HisFieldInfoMapping(5, 4, CobolType = CobolType.Unsigned)]
        public short DNT_AA { get; set; }

        // *                             AAAA
        /// <summary>
        /// COD_FISC_DANTE X(16)  
        /// </summary>
        [HisFieldInfoMapping(6, 16)]
        public string COD_FISC_DANTE { get; set; }

        // *                             COD.FISC.DANTE CAUSA          1631
        /// <summary>
        /// SED_DANTE 9(04)  
        /// </summary>
        [HisFieldInfoMapping(7, 4, CobolType = CobolType.Unsigned)]
        public short SED_DANTE { get; set; }

        // *                             SEDE DANTE-CAUSA              1647
        /// <summary>
        /// CAT_DANTE 9(03)  
        /// </summary>
        [HisFieldInfoMapping(8, 3, CobolType = CobolType.Unsigned)]
        public short CAT_DANTE { get; set; }

        // *                             CATEG.DANTE-CAUSA             1651
        /// <summary>
        /// CERT_DANTE 9(08)  
        /// </summary>
        [HisFieldInfoMapping(9, 8, CobolType = CobolType.Unsigned)]
        public int CERT_DANTE { get; set; }

        // *                             CERT.DANTE-CAUSA              1654
        /// <summary>
        /// DT_DEC_DANTE 9(06)  
        /// </summary>
        [HisFieldInfoMapping(10, 6, CobolType = CobolType.Unsigned)]
        public int DT_DEC_DANTE { get; set; }

        // *                             DATA DECESSO DANTE (AAAAMM)   1662
        /// <summary>
        /// IMP_PENS_DANTE S9(7)V9(4) COMP-3 
        /// </summary>
        [HisFieldInfoMapping(11, 6, Scale = 4, CobolType = CobolType.Comp3)]
        public decimal IMP_PENS_DANTE { get; set; }

        // *                             IMP.PENS.DANTE CAUSA          1668
        /// <summary>
        /// SIGLA_DANTE X(02)  
        /// </summary>
        [HisFieldInfoMapping(12, 2)]
        public string SIGLA_DANTE { get; set; }

        // *                             R4                            1674
        /// <summary>
        /// CAT_DANTE_FONDI X(03)  
        /// </summary>
        [HisFieldInfoMapping(13, 3)]
        public string CAT_DANTE_FONDI { get; set; }

        // *                             CATEG.DANTE-CAUSA IN CHIARO   1676
        /// <summary>
        /// CATEGO_DANTE X(08)  
        /// </summary>
        [HisFieldInfoMapping(14, 8)]
        public string CATEGO_DANTE { get; set; }

        // *                             CATEGORIA IN CHIARO (AGO)     1679
        /// <summary>
        /// FILLER X(02)  
        /// </summary>
        [HisFieldInfoMapping(15, 2)]
        public string FILLER { get; set; }

        // *                             LIBERI                        1687
        // 02 GP1AVCRD.
        /// <summary>
        /// GP1CRD1 9(02)  
        /// </summary>
        [HisFieldInfoMapping(16, 2, CobolType = CobolType.Unsigned)]
        public short GP1CRD1 { get; set; }

        /// <summary>
        /// GP1CRD2 9(02)  
        /// </summary>
        [HisFieldInfoMapping(17, 2, CobolType = CobolType.Unsigned)]
        public short GP1CRD2 { get; set; }

        // *                           GP1CENTCRD SE 99 = EX DIP.MON.  1689
        /// <summary>
        /// FILLER X(07)  
        /// </summary>
        [HisFieldInfoMapping(18, 7)]
        public string FILLER1 { get; set; }

        // *                             LIBERI                        1693
        #endregion Tracciato Host

        #region nested class

        #endregion nested class

        #endregion Properties
    }
}
