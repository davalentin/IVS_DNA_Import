using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneAventiDiritto
    {
        public static void GetAventiDirittoByIdPensione(long idPensione, out List<AventiDiritto> listaAventiDiritto)
        {
            listaAventiDiritto = null;
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                listaAventiDiritto = (from a in db.AventiDirittos
                                      where a.IdPensione == idPensione
                                      select a).ToList<AventiDiritto>();
                db.Connection.Close();
            }
        }

        public static void GetAventiDirittoConAnagraficheByIdPensione(long idPensione, out List<AventiDiritto> listaAventiDiritto, out List<Anagrafica> listaAnagrafiche)
        {
            listaAnagrafiche = null;
            listaAventiDiritto = null;
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                var results = (from a in db.AventiDirittos
                               join an in db.Anagraficas on a.IdAnagrafica equals an.Id
                               where a.IdPensione == idPensione
                               select new { AventeDiritto = a, Anagrafica = an });

                foreach (var item in results)
                {
                    if (listaAnagrafiche == null)
                        listaAnagrafiche = new List<Anagrafica>();

                    if (listaAventiDiritto == null)
                        listaAventiDiritto = new List<AventiDiritto>();

                    listaAnagrafiche.Add(item.Anagrafica);
                    listaAventiDiritto.Add(item.AventeDiritto);
                }
                db.Connection.Close();
            }
        }

        public static void SalvaAventeDiritto(AventiDiritto aventeDiritto)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                long? idAventeDiritto = null;
                int result = db.InsertAventiDiritto(aventeDiritto.Id, aventeDiritto.IdPensione, aventeDiritto.IdAnagrafica, aventeDiritto.DecParentelaDA, aventeDiritto.NucleoTitolare,
                    aventeDiritto.PresenzaWebDom, aventeDiritto.PresenzaGP, aventeDiritto.CategoriaPensione, aventeDiritto.SedePensione, aventeDiritto.CertificatoPensione, 
                    aventeDiritto.DataMatrimonio, aventeDiritto.CSog, aventeDiritto.CodiceNucleoFromGP, aventeDiritto.CodiceNucleo, aventeDiritto.TipoUnione, ref idAventeDiritto);

                if (result != 0)
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertAventiDiritto");

                if (idAventeDiritto.HasValue)
                    aventeDiritto.Id = idAventeDiritto.Value;

                db.Connection.Close();
            }
        }

        public static void DeleteAventeDirittoById(long idAventeDiritto)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAventeDiritto(idAventeDiritto);
                if (result != 0)
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAventeDiritto");

                db.Connection.Close();
            }
        }

        public static void DeleteAllAventiDirittoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllAventiDiritto(idPensione);
                if (result != 0)
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllAventiDiritto");

                db.Connection.Close();
            }
        }
    }
}
