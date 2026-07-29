using INPS.Pensioni.Liquidazione.DataCommon;
using System.Collections.Generic;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneCtrlSedePolarizzata_ENPALS
    {
        #region public members
        public static void GetCtrlSedePolarizzata_ENPALS(out List<SedePolarizzata_ENPALS> ctrl)
        {
            ctrl = null;
            List<CtrlSedePolarizzata_ENPAL> ctrlDA = null;
            DAGestioneCtrlSedePolarizzata_ENPALS.GetCtrlSedePolarizzata_ENPALS(out ctrlDA);
            if (ctrlDA != null && ctrlDA.Count > 0)
            {
                ctrl = new List<SedePolarizzata_ENPALS>();
                foreach (var item in ctrlDA)
                {
                    SedePolarizzata_ENPALS app = new SedePolarizzata_ENPALS();
                    Utility.ValorizzaOggetti(item, app);
                    ctrl.Add(app);
                }
            }
        }
        #endregion public members

        #region nested classes
        public class SedePolarizzata_ENPALS
        {
            #region public properties
            public short CodiceSede { get; set; }
            public byte? CentroOperativo { get; set; }
            #endregion public properties
        }
        #endregion nested classes
    }
}
