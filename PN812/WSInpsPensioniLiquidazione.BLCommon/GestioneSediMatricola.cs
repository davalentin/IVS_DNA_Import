using INPS.Pensioni.Liquidazione.DataCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
   public class GestioneSediMatricola
    {
       

        public static void GetDecodificaSediMatricole(string sede,out List<DecSediMatricola> elencoSediMatricole)
        {
            elencoSediMatricole = null;
            List<CtrlSediMatricola> elencoDecodificaSediMatricoleDB = null;
            DAGestioneCtrlSediMatricola.GetDecodificaSediMatricole(sede, out elencoDecodificaSediMatricoleDB);
            if (elencoDecodificaSediMatricoleDB != null && elencoDecodificaSediMatricoleDB.Count > 0)
            {
                elencoSediMatricole = new List<DecSediMatricola>();
                foreach (CtrlSediMatricola decodificaSediMatricolaDB in elencoDecodificaSediMatricoleDB)
                {
                    DecSediMatricola decSediMatricola = new DecSediMatricola();
                    Utility.ValorizzaOggetti(decodificaSediMatricolaDB, decSediMatricola);
                    elencoSediMatricole.Add(decSediMatricola);
                }
            }
        }

        public class DecSediMatricola
        {
           

            public string Sede { get; set; }
            public string Matricola{ get; set; }
          

            
        }
    }
}
