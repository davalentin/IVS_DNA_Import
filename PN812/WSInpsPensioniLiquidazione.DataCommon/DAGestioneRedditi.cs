using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneRedditi
    {
        public static void GetRedditiDReddByIdPensione(Int64 idPensione, out List<RedditiDRedd> redditiDRedd)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    redditiDRedd = (from d in db.RedditiDRedds
                                    where d.IdPensione == idPensione
                                    select d).OrderBy(x => x.AnnoReddito).ToList<RedditiDRedd>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaAllRedditiDReddByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllRedditiDRedd(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllRedditiDRedd");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaRedditoDRedd(long idPensione, RedditiDRedd redditoDRedd)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteRedditoDRedd(idPensione, redditoDRedd.AnnoReddito, redditoDRedd.Rilevanza);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteRedditoDRedd");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaRedditiDRedd(long idPensione, List<RedditiDRedd> redditiDRedd, List<RedditiDRedd> redditiDReddOriginali)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = null;
                db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                //E' necessario, trattandosi di una lista, eliminare dal db prima gli eventuali Id non presenti in listaRecordFondo ma presenti sul db
                //prima di procedere con il salvataggio della lista
                if (redditiDReddOriginali != null && redditiDReddOriginali.Count > 0)
                {
                    List<RedditiDRedd> listaRedditiDReddDaRimuovere = new List<RedditiDRedd>();
                    foreach (RedditiDRedd redditoDReddOriginale in redditiDReddOriginali)
                    {
                        bool isPresente = false;
                        if (redditiDRedd != null)
                        {
                            foreach (RedditiDRedd redditoDRedd in redditiDRedd)
                            {
                                if (redditoDRedd.Equals(redditoDReddOriginale))
                                    isPresente = true;
                            }
                        }
                        if (!isPresente)
                            listaRedditiDReddDaRimuovere.Add(redditoDReddOriginale);
                    }

                    foreach (RedditiDRedd redditoDaEliminare in listaRedditiDReddDaRimuovere)
                    {
                        EliminaRedditoDRedd(idPensione, redditoDaEliminare);
                    }
                }
                if (redditiDRedd != null)
                {
                    foreach (RedditiDRedd redditoDRedd in redditiDRedd)
                        db.InsertRedditiDRedd(idPensione, redditoDRedd.AnnoReddito, redditoDRedd.Rilevanza);
                }

                db.Connection.Close();
            }
        }
    }
}
