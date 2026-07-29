using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7
{
    public class AreaW2CL
    {
        #region tracciato COBOL
        //             04  AREAW2CL.
        //* TS1WK2CI.CPY
        //***        APPENDICE ALL'AREA WK2 PER CONVENZIONI        ***
        //***        ULTIMO AGGIORNAMENTO SETTEMBRE 1990           ***
        //***        LUNGHEZZA TOTALE AREA WK2CI =       BYTES     ***
        //***                                                      ***
        //     05 ICI2CONV                   PIC 99.
        //*+CODICE CONVENZIONE
        //     05 ICI2REGLIQ                 PIC X.
        //*+REGIME LIQUIDAZIONE
        //*         A=AUTONOMA (NON IN CONVENZIONE)
        //*         B=AUTONOMA RES. ESTERO CON PAGAMENTO IN ITALIA
        //*         P=PENSIONE IN PRORATA (CON PAGAMENTO NEL PAESE DI RESIDE
        //*         D=PEN IN PRORATA DI ANZ.(CON PAGAMENTO NEL PAESE DI RESI
        //*         C=RES. ESTERO CON PAGAMENTO IN ITALIA
        //*         E=PENS. ANZ. RES. ESTERO CON PAGAMENTO IN ITALIA
        //*
        //*+DATA DOMANDA
        //                 20  ICI2DADASEC           PIC 99.
        //                 20  ICI2DADAA             PIC 99.
        //            15  ICI2DAMM              PIC 99.
        //         10  ICI2DAGG              PIC 99.
        //     05  ICI2RESEST   PIC XXX.
        //*+ SIGLA STATO DI RESIDENZA
        //     05  ICI2LAVORA   PIC 9.
        //*+ 1=NO 2=SI 3=NON SO  (SUI GPX DEVE ANDARE SOLO 0/1)
        #endregion tracciato COBOL

        #region Tracciato Host
        // 04  AREAW2CL.
        // * TS1WK2CI.CPY
        //***        APPENDICE ALL'AREA WK2 PER CONVENZIONI        ***
        //***        ULTIMO AGGIORNAMENTO SETTEMBRE 1990           ***
        //***        LUNGHEZZA TOTALE AREA WK2CI =       BYTES     ***
        //***                                                      ***
        /// <summary>
        /// ICI2CONV 99  
        /// *+CODICE CONVENZIONE
        /// </summary>
        [HisFieldInfoMapping(0, 2)]
        public short ICI2CONV { get; set; }

        /// <summary>
        /// ICI2REGLIQ X          // *+REGIME LIQUIDAZIONE
        /// *         A=AUTONOMA (NON IN CONVENZIONE)
        /// *         B=AUTONOMA RES. ESTERO CON PAGAMENTO IN ITALIA
        /// *         P=PENSIONE IN PRORATA (CON PAGAMENTO NEL PAESE DI RESIDE
        /// *         D=PEN IN PRORATA DI ANZ.(CON PAGAMENTO NEL PAESE DI RESI
        /// *         C=RES. ESTERO CON PAGAMENTO IN ITALIA
        /// *         E=PENS. ANZ. RES. ESTERO CON PAGAMENTO IN ITALIA
        /// </summary>
        [HisFieldInfoMapping(1, 1)]
        public string ICI2REGLIQ { get; set; }

        /// <summary>
        /// ICI2DADASEC 99          //*
        /// *+DATA DOMANDA
        /// </summary>
        [HisFieldInfoMapping(2, 2)]
        public short ICI2DADASEC { get; set; }

        /// <summary>
        /// ICI2DADAA 99          //*
        /// *+DATA DOMANDA
        /// </summary>
        [HisFieldInfoMapping(3, 2)]
        public short ICI2DADAA { get; set; }

        /// <summary>
        /// ICI2DAMM 99          //*
        /// *+DATA DOMANDA
        /// </summary>
        [HisFieldInfoMapping(4, 2)]
        public short ICI2DAMM { get; set; }

        /// <summary>
        /// ICI2DAGG 99          //*
        /// *+DATA DOMANDA
        /// </summary>
        [HisFieldInfoMapping(5, 2)]
        public short ICI2DAGG { get; set; }

        /// <summary>
        /// ICI2RESEST XXX  
        /// *+ SIGLA STATO DI RESIDENZA
        /// </summary>
        [HisFieldInfoMapping(6, 3)]
        public string ICI2RESEST { get; set; }

        /// <summary>
        /// ICI2LAVORA 9  
        /// *+ 1=NO 2=SI 3=NON SO  (SUI GPX DEVE ANDARE SOLO 0/1)
        /// </summary>
        [HisFieldInfoMapping(7, 1)]
        public short ICI2LAVORA { get; set; }


        #endregion Tracciato Host
    }
}
