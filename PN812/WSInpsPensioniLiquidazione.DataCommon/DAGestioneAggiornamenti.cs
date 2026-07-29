using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneAggiornamenti
    {
        public static void GetAggiornamentiAttivi(string tipoApp, out List<Aggiornamenti> elencoAggiornamenti)
        {
            elencoAggiornamenti = null;
            if (!string.IsNullOrEmpty(tipoApp))
            {
                using (new MethodExecutionTracer())
                {
                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                    {
                        PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                        elencoAggiornamenti = (from a in db.Aggiornamentis where a.Attivo && a.Tipologia == tipoApp select a).OrderByDescending(x => x.TimeStamp).ToList<Aggiornamenti>();
                        db.Connection.Close();
                        transactionScope.Complete();
                    }
                }
            }
        }

        public static void GetAllAggiornamenti(string tipoApp, out List<Aggiornamenti> elencoAggiornamenti)
        {
            elencoAggiornamenti = null;
            if (!string.IsNullOrEmpty(tipoApp))
            {
                using (new MethodExecutionTracer())
                {
                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                    {
                        PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                        elencoAggiornamenti = (from a in db.Aggiornamentis where a.Tipologia == tipoApp select a).OrderByDescending(x => x.TimeStamp).ToList<Aggiornamenti>();
                        db.Connection.Close();
                        transactionScope.Complete();
                    }
                }
            }
        }
        public static void SalvaAggiornamento(Aggiornamenti aggiornamento)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString", ConnectionTypeEnum.Application));
                int result = db.InsertAggiornamenti(aggiornamento.Id, aggiornamento.Titolo, aggiornamento.Testo, aggiornamento.Attivo, aggiornamento.Tipologia, aggiornamento.TimeStamp);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("ErrorSPInsertAggiornamenti");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteAggiornamenti(long idAggiornamenti)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString", ConnectionTypeEnum.Application));
                int result = db.DeleteAggiornamenti(idAggiornamenti);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("ErrorSPDeleteAggiornamenti");
                }
                db.Connection.Close();
            }
        }
    }
}
