using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneFAQ
    {
        public static void GetFAQs(string tipoApp, out List<FAQ> listaFAQ)
        {
            listaFAQ = null;
            if (!string.IsNullOrEmpty(tipoApp))
            {
                using (new MethodExecutionTracer())
                {
                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                    {
                        PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                        listaFAQ = (from f in db.FAQs where f.TipoApp == tipoApp select f).ToList<FAQ>();
                        db.Connection.Close();
                        transactionScope.Complete();
                    }
                }
            }
        }

        public static void SalvaFAQ(FAQ faq)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertFAQ(faq.Id, faq.Domanda, faq.Risposta, faq.TipoApp, faq.Codice, faq.Tipologia, faq.Visibilita);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("ErrorSPInsertFAQ");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteFAQ(long idFAQ)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteFAQ(idFAQ);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("ErrorSPDeleteFAQ");
                }
                db.Connection.Close();
            }
        }

        public static void UpdateContatoreFAQ(string codice)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.UpdateContatoreFAQ(codice);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("ErrorSPUpdateContatoreFAQ");
                }
                db.Connection.Close();
            }
        }
    }
}
