using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;
using INPS.DNA;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneDelegato
    {
        public static void GetAnagraficaDelegatoByIdPensione(long idPensione, out Anagrafica anagrafica, out char codiceDelegato)
        {
            anagrafica = null;
            codiceDelegato = default(char);
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                var anagraficaDelegato = (from d in db.Delegatos where d.IdPensione == idPensione select new { d.Anagrafica, d.CodiceDelegato }).FirstOrDefault();
                if (anagraficaDelegato != null)
                {
                    anagrafica = anagraficaDelegato.Anagrafica;
                    codiceDelegato = anagraficaDelegato.CodiceDelegato;
                }
                db.Connection.Close();
            }
        }

        public static void SalvaDelegato(long idPensione, Anagrafica anagrafica, char? codiceDelegato)
        {
            PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
            DAGestioneAnagrafica.SalvaAnagrafica(anagrafica);
            db.InsertDelegato(anagrafica.Id, idPensione, codiceDelegato);
            db.Connection.Close();
        }

        public static void CancellaDelegatoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                db.DeleteDelegato(idPensione);
                db.Connection.Close();
            }
        }
    }
}
