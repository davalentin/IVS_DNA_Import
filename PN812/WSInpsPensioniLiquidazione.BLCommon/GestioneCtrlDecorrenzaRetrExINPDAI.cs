using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public static class GestioneCtrlDecorrenzaRetrExINPDAI
    {
        public static void GetCtrlDecorrenzaRetrExINPDAI(out List<CtrlDecorrenzaRetrExINPDAI> elencoCtrlDecorrenzaRetrExINPDAI)
        {
            elencoCtrlDecorrenzaRetrExINPDAI = null;
            List<DataCommon.CtrlDecorrenzaRetrExINPDAI> elencoDb = null;
            DAGestioneCtrlDecorrenzaRetrExINPDAI.GetDecorrenzaRetrExINPDAI(out elencoDb);
            if (elencoDb != null && elencoDb.Count > 0)
            {
                elencoCtrlDecorrenzaRetrExINPDAI = elencoDb.Select(x => { var r = new CtrlDecorrenzaRetrExINPDAI(); Utility.ValorizzaOggetti(x, r); return r; }).ToList();
            }
        }

     

    }
    #region Nestled class
    public class CtrlDecorrenzaRetrExINPDAI
    {
        public string Gestione { get; set; }

        public System.Nullable<char> Quota { get; set; }

        public string TipoQuota { get; set; }

        public System.Nullable<byte> CodiceDecorrenza { get; set; }

        public string Periodi { get; set; }

    }
    #endregion Nested class


}
