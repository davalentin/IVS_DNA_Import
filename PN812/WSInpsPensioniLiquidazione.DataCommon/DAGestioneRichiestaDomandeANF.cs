using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using INPS.DNA.Data;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneRichiestaDomandeANF
    {
        public static void GetRichiesteRicercaDomandeANF(long idPensione, out List<RichiestaRicercaDomandeANF> listaRichieste)
        {
            listaRichieste = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    listaRichieste = (from r in db.RichiestaRicercaDomandeANFs
                                      where r.IdPensione == idPensione
                                      select r).ToList<RichiestaRicercaDomandeANF>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaRichiestaRicercaDomandeANF(RichiestaRicercaDomandeANF richiesta)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertRichiestaRicercaDomandeANF(richiesta.IdPensione, richiesta.IdAnagrafica, richiesta.Guid, richiesta.DataRichiesta);
                if (result != 0)
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertRichiestaRicercaDomandeANF");
                db.Connection.Close();
            }
        }

        public static void DeleteRichiestaRicercaDomandeANFByIdAnagrafica(long idPensione, long idAnagrafica)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteRichiestaRicercaDomandeANF(idAnagrafica, idPensione);
                if (result != 0)
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteRichiestaRicercaDomandeANF");
                db.Connection.Close();
            }
        }

        public static void DeleteAllRichiestaRicercaDomandeANF(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllRichiestaRicercaDomandeANF(idPensione);
                if (result != 0)
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllRichiestaRicercaDomandeANF");
                db.Connection.Close();
            }
        }

    }
}
