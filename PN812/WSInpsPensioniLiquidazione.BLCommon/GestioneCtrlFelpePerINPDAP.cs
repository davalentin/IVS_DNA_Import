using INPS.Pensioni.Liquidazione.DataCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneCtrlFelpePerINPDAP
    {

        public static bool IsFlussoFelpePerINPDAP(string siglaCategoria, string gruppo, string prodotto)
        {
            return DAGestioneCtrlFelpePerINPDAP.IsFlussoFelpePerINPDAP(siglaCategoria, gruppo, prodotto);          
        }     
    }
}

