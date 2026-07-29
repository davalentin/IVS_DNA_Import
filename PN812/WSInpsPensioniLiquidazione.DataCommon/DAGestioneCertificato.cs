using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Collections;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCertificato
    {
        public static void GetNumCertificato(string sede, string categoria, out string numCertificato)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                String certificato = (from a in db.NumCertificatos where a.Sede == sede && a.Categoria == categoria select a.Certificato).FirstOrDefault();
                if (!string.IsNullOrEmpty(certificato))
                {
                    int num = Convert.ToInt32(certificato) + 1;
                    db.InsertNumCertificato(sede, categoria, num.ToString());
                    certificato = num.ToString();
                }
                numCertificato = certificato;
                db.Connection.Close();
            }
        }

        public static void getCategories(out List<string> categories)
        {
            using (new MethodExecutionTracer())
            {
                categories = new List<string>();
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                var cateva = (from a in db.NumCertificatos select a.Categoria.Trim()).Distinct();

                foreach (var v in cateva)
                {
                    categories.Add(v);
                }

            }
        }
    }
}