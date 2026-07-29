using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneAgo.Data.CAREPET
{
    public class NuoviDati2024
    {
        #region Constructor
        public NuoviDati2024()
        {
            this.AreaDati2024 = new Dati2024();
            this.AreaDatiGP2BO00 = new DatiGP2BO00();
            this.LISTT_GP2BR00 = new List<DatiGP2BR00>();
            
        }
        #endregion Constructor

        #region Properties

        #region Tracciato COBOL
        // T-DATI2024		
        //    T-GP1AJ10ZD       Data Computo
        //    T-GP1AV91A		Gestione TM su sentenza	  
        //
        // T-GP2BO00 		Dati per pensioni in CI	
        //    T-GP2BO01		    Codice convenzione	
        //    T-GP2BO02		    Codice regime convenzione	
        //    T-GP2BO04   		Anno di livello “Legge 335/1995”	
        //    T-GP2BO05E	    	Contributi Italiani ed esteri	PIC S9(7)V9(4)
        //    T-GP2BO06   		Numero di EAD 75	
        //    T-GP2BO08   		Contribuzione estera totale	PIC 9(5) 
        //    T-GP2BO09	    	Totale settimane estere utili per diritto	
        //
        // T-GP2BR00 OCCURS 6 Dati PRO-RATA estera	
        //    T-GP2BR01			
        //    T-GP2BR02		    Codice stato	
        //    T-GP2BR03		    Codice istruzione	
        //    T-GP2BR04		    Matricola estera	
        //    T-GP2BR05		    Numero settimane estero	PIC S9(5)
        //    T-GP2BR06		    Numero settimane al ricalcolo	PIC S9(5)
        //    T-GP2BR07RZ		    Data AAAAMM del ricalcolo	
        //    T-GP2BR07SA		    Anno	
        //    T-GP2BR07M		    Mese	
        //    T-GP2BR08		    Numero settimane utili	
        //    T-GP2BR09		    Sospensione cautelativa TM	
        //    T-GP2BR0A		    Età da cui sospensione	
        //    T-GP2BR0B		    Applicazione "REG. CEE 1408/1971 art. 48"	
        //    T-GP2BR0CRZ		    Data decorrenza AAAAMM "REG. CEE 1408/1971 art. 48"	
        //    T-GP2BR0CSA		    Anno	
        //    T-GP2BR0CM		    Mese	
        //    T-GP2BR10N OCCURS 70		
        //    T-GP2BR12RZ		    Decorrenza prestazione estera AAAAMM	
        //    T-GP2BR12SA		    Anno	
        //    T-GP2BR12M		    Mese	
        //    T-GP2BR13RZ		    Cessazione prestazione estera AAAAMM	
        //    T-GP2BR13SA		    Anno	
        //    T-GP2BR13M		    Mese	
        //    T-GP2BR14N		    Importo in valuta della prestazione	PIC S9(9)V9(8)
        //    T-GP2BR15		    Cambio Natura pensione	
        //    T-GP2BR16		    Codice aggiornamento	

        #endregion Tracciato COBOL

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0)]
        public Dati2024 AreaDati2024 { get; set; }

        [HisComplexAreaInfoMapping(1)]
        public DatiGP2BO00 AreaDatiGP2BO00 { get; set; }

        [HisComplexAreaInfoMapping(2, ListCount = 6)]
        public List<DatiGP2BR00> LISTT_GP2BR00 { get; set; }
        #endregion Tracciato Host

        #region nested class
        public class Dati2024
        {
            #region Properties

            #region Tracciato COBOL
            //  02 T-DATI2024.
            //     03 T-GP1AV91A           PIC 9.
            #endregion Tracciato COBOL

            #region Tracciato HOST
            // 02 T-DATI2024
            /// <summary>
            /// T-GP1AJ10ZD          PIC X(8).
            /// </summary>
            [HisFieldInfoMapping(0, 8)]
            public string T_GP1AJ10ZD { get; set; }

            /// <summary>
            /// T-GP1AV91A           PIC 9.
            /// </summary>
            [HisFieldInfoMapping(1, 1, CobolType = CobolType.Unsigned)]
            public short T_GP1AV91A { get; set; }
            #endregion Tracciato HOST

            #endregion Properties
        }

        public class DatiGP2BO00
        {
            #region Properties

            #region Tracciato COBOL
            //  03 T-GP2BO00 		
            //     04 T-GP2BO01		PIC 99
            //     04 T-GP2BO02		PIC X
            //     04 T-GP2BO04		PIC 9(4)	
            //     04 T-GP2BO05E		PIC S9(7)V9(4) COMP-3
            //     04 T-GP2BO06		PIC 9(8)
            //     04 T-GP2BO08		PIC 9(5) COMP-3	
            //     04 T-GP2BO09		PIC 9(4)
            #endregion Tracciato COBOL

            #region Tracciato Host
            // 03 T-GP2BO00.
            /// <summary>
            /// T-GP2BO01		PIC 99
            /// </summary>
            [HisFieldInfoMapping(0, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BO01 { get; set; }

            /// <summary>
            /// T-GP2BO02		PIC X 
            /// </summary>
            [HisFieldInfoMapping(1, 1)]
            public string T_GP2BO02 { get; set; }

            /// <summary>
            /// T-GP2BO04	     PIC 9(4)  
            /// </summary>
            [HisFieldInfoMapping(2, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BO04 { get; set; }

            /// <summary>
            /// T-GP2BO05E	 PIC S9(7)V9(4) COMP-3  
            /// </summary>
            [HisFieldInfoMapping(3, 6, Scale = 4, CobolType = CobolType.Comp3)]
            public decimal T_GP2BO05E { get; set; }

            /// <summary>
            /// T-GP2BO06		 PIC 9(8)  
            /// </summary>
            [HisFieldInfoMapping(4, 8, CobolType = CobolType.Unsigned)]
            public int T_GP2BO06 { get; set; }

            /// <summary>
            /// T-GP2BO08		PIC 9(5) COMP-3 
            /// </summary>
            [HisFieldInfoMapping(5, 3, CobolType = CobolType.Comp3)]
            public int T_GP2BO08 { get; set; }

            /// <summary>
            /// T-GP2BO09		PIC 9(4)  
            /// </summary>
            [HisFieldInfoMapping(6, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BO09 { get; set; }
            #endregion Tracciato Host

            #endregion Properties
        }

        public class DatiGP2BR00
        {
            #region Properties

            #region Tracciato COBOL
            //  03 T-GP2BR00 OCCURS 6
            //     04 T-GP2BR01
            //       05 T-GP2BR02       PIC 99
            //       05 T-GP2BR03       PIC 999
            //       05 T-GP2BR04       PIC X(16)
            //       05 T-GP2BR05       PIC S9(5) COMP-3
            //       05 T-GP2BR06       PIC S9(5) COMP-3
            //     05 T-GP2BR07RZ     
            //       06 T-GP2BR07SA     PIC 9(4)	
            //       06 T-GP2BR07M      PIC 99
            //     05 T-GP2BR08       PIC 9(4)
            //     05 T-GP2BR09       PIC X	
            //     05 T-GP2BR0A       PIC X(2)
            //     05 T-GP2BR0B       PIC X	
            //     05 T-GP2BR0CRZ
            //       06 T-GP2BR0CSA     PIC 9(4)	
            //       06 T-GP2BR0CM      PIC 99
            //     05 T-GP2BR10N OCCURS 70
            //     06 T-GP2BR12RZ
            //       07 T-GP2BR12SA     PIC 9(4)
            //       07 T-GP2BR12M      PIC 99
            //     06 T-GP2BR13RZ
            //       07 T-GP2BR13SA     PIC 9(4)	
            //       07 T-GP2BR13M      PIC 99
            //     06 T-GP2BR14N      PIC S9(9)V9(8) COMP-3	
            //     06 T-GP2BR15       PIC X
            //     06 T-GP2BR16       PIC X
            #endregion Tracciato COBOL

            #region Tracciato Host
            //  03 T-GP2BR00 OCCURS 6
            //     04 T-GP2BR01
            /// <summary>
            /// T-GP2BR02       PIC 99
            /// </summary>
            [HisFieldInfoMapping(0, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BR02 { get; set; }

            /// <summary>
            /// T-GP2BR03       PIC 999  
            /// </summary>
            [HisFieldInfoMapping(1, 3, CobolType = CobolType.Unsigned)]
            public short T_GP2BR03 { get; set; }

            /// <summary>
            /// T-GP2BR04		  PIC X(16)
            /// </summary>
            [HisFieldInfoMapping(2, 16)]
            public string T_GP2BR04 { get; set; }

            /// <summary>
            /// T-GP2BR05		  PIC S9(5) COMP-3  
            /// </summary>
            [HisFieldInfoMapping(3, 3, CobolType = CobolType.Comp3)]
            public int T_GP2BR05 { get; set; }

            /// <summary>
            /// T-GP2BR06		  PIC S9(5) COMP-3  
            /// </summary>
            [HisFieldInfoMapping(4, 3, CobolType = CobolType.Comp3)]
            public int T_GP2BR06 { get; set; }

            // 05 T-GP2BR07RZ     	
            /// <summary>
            /// T-GP2BR07SA    PIC 9(4) 
            /// </summary>
            [HisFieldInfoMapping(5, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BR07SA { get; set; }

            /// <summary>
            /// T-GP2BR07M	PIC 99
            /// </summary>
            [HisFieldInfoMapping(6, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BR07M { get; set; }

            /// <summary>
            /// T-GP2BR08	    PIC 9(4)	  
            /// </summary>
            [HisFieldInfoMapping(7, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BR08 { get; set; }

            /// <summary>
            /// T-GP2BR09		PIC X 
            /// </summary>
            [HisFieldInfoMapping(8, 1)]
            public string T_GP2BR09 { get; set; }

            /// <summary>
            /// T-GP2BR0A	    PIC X(2)  
            /// </summary>
            [HisFieldInfoMapping(9, 2)]
            public string T_GP2BR0A { get; set; }

            /// <summary>
            /// T-GP2BR0B	    PIC X
            /// </summary>
            [HisFieldInfoMapping(10, 1)]
            public string T_GP2BR0B { get; set; }

            // 05 T-GP2BR0CRZ
            /// <summary>
            /// T-GP2BR0CSA   PIC 9(4)	  
            /// </summary>
            [HisFieldInfoMapping(11, 4, CobolType = CobolType.Unsigned)]
            public short T_GP2BR0CSA { get; set; }

            /// <summary>
            /// T-GP2BR0CM    PIC 99 
            /// </summary>
            [HisFieldInfoMapping(12, 2, CobolType = CobolType.Unsigned)]
            public short T_GP2BR0CM { get; set; }

            /// <summary>
            ///  05 T-GP2BR10N OCCURS 70
            /// <summary>
            [HisComplexAreaInfoMapping(13, ListCount = 70)]
            public List<T_GP2BR10N> LISTT_GP2BR10N { get; set; }

            public class T_GP2BR10N
            {
                //    06 T-GP2BR12RZ
                /// <summary>
                /// T-GP2BR12SA	PIC 9(4)
                /// </summary>
                [HisFieldInfoMapping(0, 4, CobolType = CobolType.Unsigned)]
                public short T_GP2BR12SA { get; set; }

                /// <summary>
                /// T-GP2BR12M	PIC 99  
                /// </summary>
                [HisFieldInfoMapping(1, 2, CobolType = CobolType.Unsigned)]
                public short T_GP2BR12M { get; set; }

                // 06 T-GP2BR13RZ
                /// <summary>
                /// T-GP2BR13SA	PIC 9(4)  
                /// </summary>
                [HisFieldInfoMapping(2, 4, CobolType = CobolType.Unsigned)]
                public short T_GP2BR13SA { get; set; }

                /// <summary>
                /// T-GP2BR13M	PIC 99
                /// </summary>
                [HisFieldInfoMapping(3, 2, CobolType = CobolType.Unsigned)]
                public short T_GP2BR13M { get; set; }

                /// <summary>
                /// T-GP2BR14N    PIC S9(9)V9(8) COMP-3 
                /// </summary>
                [HisFieldInfoMapping(4, 9, Scale = 8, CobolType = CobolType.Comp3)]
                public decimal T_GP2BR14N { get; set; }

                /// <summary>
                /// T-GP2BR15		PIC X
                /// </summary>
                [HisFieldInfoMapping(5, 1)]
                public string T_GP2BR15 { get; set; }

                /// <summary>
                /// T-GP2BR16		PIC X	
                /// </summary>
                [HisFieldInfoMapping(6, 1)]
                public string T_GP2BR16 { get; set; }
            }
            #endregion Tracciato Host

            #endregion Properties
        }

        #endregion nested class

        #endregion Properties
    }
}
