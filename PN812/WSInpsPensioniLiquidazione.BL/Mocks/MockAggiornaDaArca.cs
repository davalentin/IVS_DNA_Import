using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Mocks
{
    public class MockAggiornaDaArca
    {
        public static void AggiornaDaArca_Mocks(ref Entity.Anagrafica anagraficaBL)
        {
            anagraficaBL.CodiceComuneResidenza = "H501";
            anagraficaBL.ComuneResidenza = "ROMA";
            anagraficaBL.IsResidenteInItalia = true;
            anagraficaBL.ResidenzaEstero = false;
            anagraficaBL.CodiceStatoCivile = '1';

            //anagraficaBL.CodiceComuneResidenza = "C351";
            //anagraficaBL.ComuneResidenza = "CATANIA";
            //anagraficaBL.IsResidenteInItalia = true;
            //anagraficaBL.ResidenzaEstero = false;

            //anagraficaBL.CodiceComuneResidenza = "Z110";
            //anagraficaBL.ComuneResidenza = "FRANCIA";
            //anagraficaBL.IsResidenteInItalia = false;
            //anagraficaBL.ResidenzaEstero = true;

            //anagraficaBL.CodiceComuneResidenza = "Z114";
            //anagraficaBL.ComuneResidenza = "REGNO UNITO";
            //anagraficaBL.IsResidenteInItalia = false;
            //anagraficaBL.ResidenzaEstero = true;

            //anagraficaBL.CodiceComuneResidenza = "Z112";
            //anagraficaBL.ComuneResidenza = "GERMANIA";
            //anagraficaBL.IsResidenteInItalia = false;
            //anagraficaBL.ResidenzaEstero = true;
            //anagraficaBL.CodiceStatoCivile = 3;

            //anagraficaBL.CodiceComuneResidenza = "Z101";
            //anagraficaBL.ComuneResidenza = "ANDORRA";
            //anagraficaBL.IsResidenteInItalia = false;
            //anagraficaBL.ResidenzaEstero = true;
        }
    }
}
