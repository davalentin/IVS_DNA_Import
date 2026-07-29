using INPS.Pensioni.Liquidazione.BLCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaSediMatricola
    {
        public static void GetDecodificaSediMatricola ( string sede, out List<Entity.SediMatricola> elencoSediMatricole)
        {
            elencoSediMatricole = new List<Entity.SediMatricola>();
            List<GestioneSediMatricola.DecSediMatricola> listaSediMatricole = new List<GestioneSediMatricola.DecSediMatricola>();
            GestioneSediMatricola.GetDecodificaSediMatricole(sede, out listaSediMatricole);



        }
    }
}

