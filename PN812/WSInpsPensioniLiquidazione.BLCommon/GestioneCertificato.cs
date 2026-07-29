using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneCertificato
    {
        public static void GetCertificato(string sede, string categoria, string codiceGestione, string gruppo, out string numCertificato)
        {
            string certificato = String.Empty;
            List<string> catL = new List<string>();
            DAGestioneCertificato.getCategories(out catL);
            if (string.Compare(codiceGestione, "007", StringComparison.InvariantCulture) == 0)
            {
                categoria = "FondiS";
                DAGestioneCertificato.GetNumCertificato(sede, categoria, out certificato);
                numCertificato = certificato;
            }

            else if (catL.Contains(categoria.Trim()))
            {
                DAGestioneCertificato.GetNumCertificato(sede, categoria, out certificato);
                numCertificato = certificato;

            }
            else
            {
                numCertificato = "0";
            }

        }
    }
}
