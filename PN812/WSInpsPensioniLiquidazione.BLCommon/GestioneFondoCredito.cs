using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneFondoCredito
    {
        public static bool VerificaAdesioneFondoCredito(string codiceFiscaleTitolare)
        {
            if (String.IsNullOrEmpty(codiceFiscaleTitolare))
                return false;            

            return DAGestioneFondoCredito.VerificaAdesioneFondoCredito(codiceFiscaleTitolare);            
        }
    }
}
