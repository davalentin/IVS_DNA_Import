using INPS.Pensioni.Liquidazione.DataCommon;
using System.Collections.Generic;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneCtrlAttEconProfInd_PensProv
    {
        #region public members
        public static void GetCtrlAttEconProfInd_PensProv(out List<AttEconProfInd_PensProv> ctrl)
        {
            ctrl = null;
            List<CtrlAttEconProfInd_PensProv> ctrlDA = null;
            DAGestioneCtrlAttEconProfInd_PensProv.GetCtrlAttEconProfInd_PensProv(out ctrlDA);
            if (ctrlDA != null && ctrlDA.Count > 0)
            {
                ctrl = new List<AttEconProfInd_PensProv>();
                foreach (var item in ctrlDA)
                {
                    AttEconProfInd_PensProv app = new AttEconProfInd_PensProv();
                    Utility.ValorizzaOggetti(item, app);
                    ctrl.Add(app);
                }
            }
        }
        #endregion public members

        #region nested classes
        public class AttEconProfInd_PensProv
        {
            #region public properties
            public int AttivitaEconomica { get; set; }
            public int ProfessioneIndividuale { get; set; }
            #endregion public properties
        }
        #endregion nested classes
    }
}
