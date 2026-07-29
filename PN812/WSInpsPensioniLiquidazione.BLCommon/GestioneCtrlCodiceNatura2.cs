using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneCtrlCodiceNatura2
    {

        public static void GetCtrlCodiceNatura2ByCodiceTipoRichiesta(string codiceTipoRichiesta, out CtrlCodiceNatura2 ctrlCodiceNatura2)
        {
            INPS.Pensioni.Liquidazione.DataCommon.CtrlCodiceNatura2 ctrlCodiceNatura2DB = null;
            INPS.Pensioni.Liquidazione.DataCommon.DAGestioneCtrlCodiceNatura2.GetCodiciNatura2ByCodiceTipoRichiesta(codiceTipoRichiesta, out ctrlCodiceNatura2DB);

            ctrlCodiceNatura2 = null;
            if (ctrlCodiceNatura2DB != null)
            {
                ctrlCodiceNatura2 = new GestioneCtrlCodiceNatura2.CtrlCodiceNatura2();
                Utility.ValorizzaOggetti(ctrlCodiceNatura2DB, ctrlCodiceNatura2);
            }
        }

        #region nested classes
        public class CtrlCodiceNatura2
        {
            #region private properties

            private string _CodiceTipoRichiesta;
            private char _CodiceNatura2;

            #endregion private properties

            #region public properties

            public string CodiceTipoRichiesta
            {
                get { return _CodiceTipoRichiesta; }
                set { _CodiceTipoRichiesta = value; }
            }

            public char CodiceNatura2
            {
                get { return _CodiceNatura2; }
                set { _CodiceNatura2 = value; }
            }

            #endregion public properties
        }
        #endregion nested classes
    }
}
