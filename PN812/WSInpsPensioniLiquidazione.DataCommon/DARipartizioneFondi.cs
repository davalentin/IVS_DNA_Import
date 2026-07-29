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
    public class DARipartizioneFondi
    {
        public static void GetRipartizioneFondiByIdPensione(long idPensione, out List<RipartizioneFondi> LripartizioneFondi)
        {
            LripartizioneFondi = new List<RipartizioneFondi>();
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    LripartizioneFondi = (from p in db.RipartizioneFondis where p.IdPensione == idPensione select p).ToList<RipartizioneFondi>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaRipartizioneFondi(RipartizioneFondi ripartizioneFondi)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertRipartizioneFondi(ripartizioneFondi.IdPensione,
                                                            ripartizioneFondi.CodiceAltroFondo,
                                                            ripartizioneFondi.Importo,
                                                            ripartizioneFondi.Progressivo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertRipartizioneFondi");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaRipartizioneFondiByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteRipartizioneFondi(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteRipartizioneFondi");
                }
                db.Connection.Close();
            }
        }
    }
}
