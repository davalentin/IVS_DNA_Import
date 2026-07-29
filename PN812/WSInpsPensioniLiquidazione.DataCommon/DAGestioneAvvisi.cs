using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneAvvisi
    {
        public static void GetAvvisiAttivi(string tipoApp, out List<Avvisi> elencoAvvisi)
        {
            elencoAvvisi = null;
            if (!string.IsNullOrEmpty(tipoApp))
            {
                using (new MethodExecutionTracer())
                {
                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                    {
                        PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                        elencoAvvisi = (from a in db.Avvisis where a.Attivo && a.Tipologia == tipoApp select a).OrderByDescending(x => x.TimeStamp).ToList<Avvisi>();
                        db.Connection.Close();
                        transactionScope.Complete();
                    }
                }
            }
        }

        public static void GetAllAvvisi(string tipoApp, out List<Avvisi> elencoAvvisi)
        {
            elencoAvvisi = null;
            if (!string.IsNullOrEmpty(tipoApp))
            {
                using (new MethodExecutionTracer())
                {
                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                    {
                        PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                        elencoAvvisi = (from a in db.Avvisis where a.Tipologia == tipoApp select a).OrderByDescending(x => x.TimeStamp).ToList<Avvisi>();
                        db.Connection.Close();
                        transactionScope.Complete();
                    }
                }
            }
        }
        public static void SalvaAvviso(Avvisi avviso)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString", ConnectionTypeEnum.Application));
                int result = db.InsertAvviso(avviso.Id, avviso.Titolo, avviso.Testo, avviso.Attivo, avviso.Tipologia, avviso.TimeStamp);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("ErrorSPInsertAvviso");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteAvviso(long idAvviso)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString", ConnectionTypeEnum.Application));
                int result = db.DeleteAvviso(idAvviso);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("ErrorSPDeleteAvviso");
                }
                db.Connection.Close();
            }
        }
    }
}
