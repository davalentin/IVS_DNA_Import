using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaControlliDinamici
    {
        public static void GetListaVersioni(out Dictionary<string, string> listaVersioni)
        {
            listaVersioni = null;
            try
            {
                GestioneControlliDinamici.GetListaVersioni(out listaVersioni);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception Ex)
            {
                throw new DnaApplicationException("Errore nel metodo GetListaVersioni.", Ex);
            }
        }
    }
}
