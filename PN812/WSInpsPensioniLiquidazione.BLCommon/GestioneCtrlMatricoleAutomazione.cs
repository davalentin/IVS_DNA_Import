using INPS.Pensioni.Liquidazione.DataCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneCtrlMatricoleAutomazione
    {
        public static bool IsMatricolaForAutomazione(string matricola)
        {

            CtrlMatricoleAutomazione matricolaDB = null;
            DAGestioneCtrlMatricoleAutomazione.IsMatricolaForAutomazione(matricola, out matricolaDB);
            if (matricolaDB != null)
            {
                return true;
            }

            return false;
        }
    }
}
