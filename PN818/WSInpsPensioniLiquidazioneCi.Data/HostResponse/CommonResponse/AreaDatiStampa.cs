using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    public class AreaDatiStampa
    {
        #region Constructor
        public AreaDatiStampa()
        {
            this.AreaSede = new Sede();
            this.AreaPatronato = new Patronato();
        }
        #endregion Constructor

        #region tracciato COBOL
        //        02  FILLER               PIC X(1920).                                                          
        //03  DATA-STA.
        //     08  DATA-STA-GG              PIC XX.
        //     08  DATA-STA-MM              PIC XX.
        //     08  DATA-STA-AA              PIC XXXX.
        //03  DATI-SEDE.                                               
        //            05  CODICE-SEDE   PIC X(4).                      
        //            05  NOME-SEDE     PIC X(22).                     
        //            05  VIA-SEDE      PIC X(32).                     
        //            05  CAP-SEDE      PIC X(5).                      
        //            05  CITTA-SEDE    PIC X(22).                     
        //            05  PROV-SEDE     PIC X(3).                      
        //03  NOME-FIRMA.                                              
        //            05  NOME-DIRIGENTE  PIC X(32).                   
        //03  DATI-PATRONATO.                                          
        //            05  CODICE-PATR   PIC X(2).                      
        //            05  ZONA-PATR     PIC X.                         
        //            05  NOME-PATR     PIC X(12).                     
        //            05  VIA-PATR      PIC X(32).                     
        //            05  CAP-PATR      PIC X(5).                      
        //            05  CITTA-PATR    PIC X(22).                     
        //            05  PROV-PATR     PIC X(3).                     
        //03  DATI-ISTITUZIONI.                                        
        //  04  ISTITUZIONE OCCURS 4.                                  
        //            05  CODICE-STATO       PIC X(2).                 
        //            05  CODICE-ISTITUZ     PIC X(3).                 
        //            05  DENOM-STATO        PIC X(22).                
        //            05  DENOM-MONETA       PIC X(16).                
        //            05  DENOM-ISTITUZ-A    PIC X(40).                
        //            05  DENOM-ISTITUZ-B    PIC X(40).                
        //            05  DENOM-ISTITUZ-C    PIC X(40).                
        //            05  INDIR-ISTITUZ-A    PIC X(40).                
        //            05  INDIR-ISTITUZ-B    PIC X(40).                
        //            05  ABBRE-ISTITUZ      PIC X(15).                
        //            05  CITTA-ISTITUZ      PIC X(15). 

        #endregion tracciato COBOL

        #region Tracciato Host
        // 02  FILLER               PIC X(1920).
        // 03  DATA-STA.
        /// <summary>
        /// DATA_STA_GG XX  
        /// </summary>
        [HisFieldInfoMapping(0, 2)]
        public string DATA_STA_GG { get; set; }

        /// <summary>
        /// DATA_STA_MM XX  
        /// </summary>
        [HisFieldInfoMapping(1, 2)]
        public string DATA_STA_MM { get; set; }

        /// <summary>
        /// DATA_STA_AA XXXX  
        /// </summary>
        [HisFieldInfoMapping(2, 4)]
        public string DATA_STA_AA { get; set; }

        [HisComplexAreaInfoMapping(3)]
        public Sede AreaSede { get; set; }

        // 03  NOME-FIRMA.
        /// <summary>
        /// NOME_DIRIGENTE X(32)  
        /// </summary>
        [HisFieldInfoMapping(4, 32)]
        public string NOME_DIRIGENTE { get; set; }

        [HisComplexAreaInfoMapping(5)]
        public Patronato AreaPatronato { get; set; }

        [HisComplexAreaInfoMapping(6, ListCount = 4)]
        public List<Istituzione> ISTITUZIONI { get; set; }
        #endregion Tracciato Host

        #region Nested class
        public class Sede
        {
            #region tracciato COBOL
            //03  DATI-SEDE.                                               
            //            05  CODICE-SEDE   PIC X(4).                      
            //            05  NOME-SEDE     PIC X(22).                     
            //            05  VIA-SEDE      PIC X(32).                     
            //            05  CAP-SEDE      PIC X(5).                      
            //            05  CITTA-SEDE    PIC X(22).                     
            //            05  PROV-SEDE     PIC X(3).      

            #endregion tracciato COBOL

            #region Tracciato Host
            // 03  DATI-SEDE.
            /// <summary>
            /// CODICE_SEDE X(4)  
            /// </summary>
            [HisFieldInfoMapping(0, 4)]
            public string CODICE_SEDE { get; set; }

            /// <summary>
            /// NOME_SEDE X(22)  
            /// </summary>
            [HisFieldInfoMapping(1, 22)]
            public string NOME_SEDE { get; set; }

            /// <summary>
            /// VIA_SEDE X(32)  
            /// </summary>
            [HisFieldInfoMapping(2, 32)]
            public string VIA_SEDE { get; set; }

            /// <summary>
            /// CAP_SEDE X(5)  
            /// </summary>
            [HisFieldInfoMapping(3, 5)]
            public string CAP_SEDE { get; set; }

            /// <summary>
            /// CITTA_SEDE X(22)  
            /// </summary>
            [HisFieldInfoMapping(4, 22)]
            public string CITTA_SEDE { get; set; }

            /// <summary>
            /// PROV_SEDE X(3)  
            /// </summary>
            [HisFieldInfoMapping(5, 3)]
            public string PROV_SEDE { get; set; }
            #endregion Tracciato Host
        }

        public class Patronato
        {
            #region tracciato COBOL                 
            //03  DATI-PATRONATO.                                          
            //            05  CODICE-PATR   PIC X(2).                      
            //            05  ZONA-PATR     PIC X.                         
            //            05  NOME-PATR     PIC X(12).                     
            //            05  VIA-PATR      PIC X(32).                     
            //            05  CAP-PATR      PIC X(5).                      
            //            05  CITTA-PATR    PIC X(22).                     
            //            05  PROV-PATR     PIC X(3).      

            #endregion tracciato COBOL

            #region Tracciato Host
            // 03  DATI-PATRONATO.
            /// <summary>
            /// CODICE_PATR X(2)  
            /// </summary>
            [HisFieldInfoMapping(0, 2)]
            public string CODICE_PATR { get; set; }

            /// <summary>
            /// ZONA_PATR X  
            /// </summary>
            [HisFieldInfoMapping(1, 1)]
            public string ZONA_PATR { get; set; }

            /// <summary>
            /// NOME_PATR X(12)  
            /// </summary>
            [HisFieldInfoMapping(2, 12)]
            public string NOME_PATR { get; set; }

            /// <summary>
            /// VIA_PATR X(32)  
            /// </summary>
            [HisFieldInfoMapping(3, 32)]
            public string VIA_PATR { get; set; }

            /// <summary>
            /// CAP_PATR X(5)  
            /// </summary>
            [HisFieldInfoMapping(4, 5)]
            public string CAP_PATR { get; set; }

            /// <summary>
            /// CITTA_PATR X(22)  
            /// </summary>
            [HisFieldInfoMapping(5, 22)]
            public string CITTA_PATR { get; set; }

            /// <summary>
            /// PROV_PATR X(3)  
            /// </summary>
            [HisFieldInfoMapping(6, 3)]
            public string PROV_PATR { get; set; }
            #endregion Tracciato Host
        }

        public class Istituzione
        {
            #region tracciato COBOL                   
            //03  DATI-ISTITUZIONI.                                        
            //  04  ISTITUZIONE OCCURS 4.                                  
            //            05  CODICE-STATO       PIC X(2).                 
            //            05  CODICE-ISTITUZ     PIC X(3).                 
            //            05  DENOM-STATO        PIC X(22).                
            //            05  DENOM-MONETA       PIC X(16).                
            //            05  DENOM-ISTITUZ-A    PIC X(40).                
            //            05  DENOM-ISTITUZ-B    PIC X(40).                
            //            05  DENOM-ISTITUZ-C    PIC X(40).                
            //            05  INDIR-ISTITUZ-A    PIC X(40).                
            //            05  INDIR-ISTITUZ-B    PIC X(40).                
            //            05  ABBRE-ISTITUZ      PIC X(15).                
            //            05  CITTA-ISTITUZ      PIC X(15). 

            #endregion tracciato COBOL

            #region Tracciato Host
            // 03  DATI-ISTITUZIONI.
            // 04  ISTITUZIONE OCCURS 4.
            /// <summary>
            /// CODICE_STATO X(2)  
            /// </summary>
            [HisFieldInfoMapping(0, 2)]
            public string CODICE_STATO { get; set; }

            /// <summary>
            /// CODICE_ISTITUZ X(3)  
            /// </summary>
            [HisFieldInfoMapping(1, 3)]
            public string CODICE_ISTITUZ { get; set; }

            /// <summary>
            /// DENOM_STATO X(22)  
            /// </summary>
            [HisFieldInfoMapping(2, 22)]
            public string DENOM_STATO { get; set; }

            /// <summary>
            /// DENOM_MONETA X(16)  
            /// </summary>
            [HisFieldInfoMapping(3, 16)]
            public string DENOM_MONETA { get; set; }

            /// <summary>
            /// DENOM_ISTITUZ_A X(40)  
            /// </summary>
            [HisFieldInfoMapping(4, 40)]
            public string DENOM_ISTITUZ_A { get; set; }

            /// <summary>
            /// DENOM_ISTITUZ_B X(40)  
            /// </summary>
            [HisFieldInfoMapping(5, 40)]
            public string DENOM_ISTITUZ_B { get; set; }

            /// <summary>
            /// DENOM_ISTITUZ_C X(40)  
            /// </summary>
            [HisFieldInfoMapping(6, 40)]
            public string DENOM_ISTITUZ_C { get; set; }

            /// <summary>
            /// INDIR_ISTITUZ_A X(40)  
            /// </summary>
            [HisFieldInfoMapping(7, 40)]
            public string INDIR_ISTITUZ_A { get; set; }

            /// <summary>
            /// INDIR_ISTITUZ_B X(40)  
            /// </summary>
            [HisFieldInfoMapping(8, 40)]
            public string INDIR_ISTITUZ_B { get; set; }

            /// <summary>
            /// ABBRE_ISTITUZ X(15)  
            /// </summary>
            [HisFieldInfoMapping(9, 15)]
            public string ABBRE_ISTITUZ { get; set; }

            /// <summary>
            /// CITTA_ISTITUZ X(15)  
            /// </summary>
            [HisFieldInfoMapping(10, 15)]
            public string CITTA_ISTITUZ { get; set; }
            #endregion Tracciato Host
        }
        #endregion Nested class
    }
}
